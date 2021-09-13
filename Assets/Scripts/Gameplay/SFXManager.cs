using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

// any entry added to this enum must be assigned a sound effect in the editor
public enum SFXEnum
{
    EnemyHit
}

[RequireComponent( typeof( AudioSource ) )]
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [HideInInspector]
    public SerializableDictionary<SFXEnum, AudioClip> AudioClipMappings = new SerializableDictionary<SFXEnum, AudioClip>();
    private Dictionary<SFXEnum, SFXRingBuffer> SFXBuffers = new Dictionary<SFXEnum, SFXRingBuffer>();

    private void Start()
    {
        Instance = this;
        foreach( SFXEnum sfx in Enum.GetValues( typeof( SFXEnum ) ) )
        {
            var buffer = new SFXRingBuffer();
            SFXBuffers[sfx] = buffer;

            AudioClip clip;
            if( AudioClipMappings.TryGetValue( sfx, out clip ) )
            {
                buffer.InitalizeWithAudioClip( clip );
            }
            else
            {
                Debug.LogError( $"SFXManager: sound effect enum {sfx} not assigned an audio clip" );
            }
        }
    }

    private void OnAudioFilterRead( float[] data, int channels )
    {
        lock( SFXBuffers )
        {
            foreach( var buffer in SFXBuffers.Values )
            {
                buffer.ReadFromBuffer( data, channels );
            }
        }
    }

    public void PlaySFX( SFXEnum sfx )
    {
        lock( SFXBuffers )
        {
            SFXBuffers[sfx].WriteSFXToBuffer();
        }
    }
}

class SFXRingBuffer
{
    private float[] buffer = new float[100000];
    private float[] clip_counter_buffer = new float[100000]; // parallel with buffer - maintains how many clips are stored at each index for attenuation
    private int buffer_index;
    private AudioClip audio_clip;
    private float[] audio_clip_samples;

    // read from the buffer into an output array
    // advance the internal buffer index by the size of the out array
    public void ReadFromBuffer( float[] out_array, int channels )
    {
        for( int x = 0; x < out_array.Length; ++x )
        {
            if( clip_counter_buffer[buffer_index] > 0 )
                out_array[x] += buffer[buffer_index] 
                    * ( 1.0f / clip_counter_buffer[buffer_index] ) // attenuate down based on number of samples playing
                    * ( 1.0f + Mathf.Log( clip_counter_buffer[buffer_index] ) ); // increase volume slightly so attenuation isn't linear decrease

            if( x % channels == ( channels - 1 ) )
            {
                buffer[buffer_index] = 0.0f; // clean up the buffer behind us to make it easier to write into later
                clip_counter_buffer[buffer_index] = 0;
                ++buffer_index;
                if( buffer_index >= buffer.Length )
                    buffer_index = 0;
            }
        }
    }

    // write to the buffer at the current buffer index
    public void WriteSFXToBuffer()
    {
        Debug.Assert( audio_clip != null, "SFXRingBuffer: Buffer written to without audio clip loaded" );
        if( audio_clip == null )
            return;

        int temp_buffer_index = buffer_index;
        for( int x = 0; x < audio_clip_samples.Length; ++x )
        {
            buffer[temp_buffer_index] += audio_clip_samples[x];
            ++clip_counter_buffer[temp_buffer_index];

            ++temp_buffer_index;
            if( temp_buffer_index >= buffer.Length )
                temp_buffer_index = 0;
        }
    }

    public void InitalizeWithAudioClip( AudioClip clip )
    {
        // store clip
        audio_clip = clip;

        // read out samples
        float[] temp_clip_samples = new float[clip.samples];
        bool success = clip.GetData( temp_clip_samples, 0 );
        Debug.Assert( success, $"SFXRingBuffer: Error reading samples from audio clip {clip.name}" );

        // compress samples into single channel
        int single_channel_samples = temp_clip_samples.Length / audio_clip.channels;
        audio_clip_samples = new float[single_channel_samples];
        for( int x = 0; x < single_channel_samples; ++x )
        {
            float averaged_sample = 0.0f;
            for( int y = 0; y < audio_clip.channels; ++y )
            {
                averaged_sample += temp_clip_samples[x * audio_clip.channels + y];
            }
            averaged_sample /= audio_clip.channels;
            audio_clip_samples[x] = averaged_sample;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor( typeof( SFXManager ) )]
class SFXManagerEditor : ExtendedEditor<SFXManager>
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var target = GetTarget();

        foreach( SFXEnum sfx in Enum.GetValues( typeof( SFXEnum ) ) )
        {
            if( !target.AudioClipMappings.ContainsKey( sfx ) )
                target.AudioClipMappings.Add( sfx, null );
        }

        SectionHeader( "Sound Effect Mappings" );

        foreach( SFXEnum sfx in Enum.GetValues( typeof( SFXEnum ) ) )
        {
            AudioClip clip = target.AudioClipMappings[sfx];
            AudioClipField( ref clip, sfx.ToString() );
            target.AudioClipMappings[sfx] = clip;
        }
    }
}
#endif