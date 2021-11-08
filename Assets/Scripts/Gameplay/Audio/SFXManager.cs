using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

// any entry added to this enum must be assigned a sound effect in the editor
public enum SFXEnum
{
    NONE = 0,
    EnemyHit,
    SkeletonDie,
    PumpkinHit,
    BlackHoleDie,
    BolterDie,
    BomberExplode,
    MudCarrierDie,
    MudCarrierHit,
    MudlingDie,
    ShrikeDie,
    Shrike2Die,
    SpaceWormDie,
    SpaceWormSpawn,
    BomberHit,
    PumpkinDie,
    MudslingerHit,
    MudlingSpawn,
    CarrierLargeDie,
    CarrierLargeHit,
    CarrierMediumDie,
    CarrierMediumHit,
    CarrierSmallDie,
    BlackHoleHit,
    BomberSpawn,
    MudCarrierSpawn,
    SkeleSpawn1,
    SkeleSpawn2,
    ShamanSummon,
    ShamanDie,
    EnemyHit2,
    ShrikeEnter,
    ShrikeExit,
    Shrike2Enter,
    Shrike2Exit,
    BouncerDie,
    BouncerSpawn,
    MudCarrierSummon,
    Shaman2Die,
    Shaman2Summon,
    Shaman2Spawn,
    ShamanSpawn,
    GhostieDie,
    Ghostie2Die,
    CarrierSpawn,


}

[RequireComponent( typeof( AudioSource ) )]
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;
    private static float[] LogCache = new float[101];

    [HideInInspector]
    public SerializableDictionary<SFXEnum, AudioClip> AudioClipMappings = new SerializableDictionary<SFXEnum, AudioClip>();
    public SerializableDictionary<SFXEnum, float> AudioClipVolumeMultipliers = new SerializableDictionary<SFXEnum, float>();
    private Dictionary<SFXEnum, SFXRingBuffer> SFXBuffers = new Dictionary<SFXEnum, SFXRingBuffer>();

    private void Start()
    {
        Instance = this;
        // compute an array of cached log values for buffers to use in attenuation
        for( int x = 0; x <= 100; ++x )
        {
            LogCache[x] = Mathf.Log( x );
        }
        // spawn a buffer for each SFX
        foreach( SFXEnum sfx in Enum.GetValues( typeof( SFXEnum ) ) )
        {
            if( sfx == SFXEnum.NONE )
                continue;

            var buffer = new SFXRingBuffer();
            SFXBuffers[sfx] = buffer;

            AudioClip clip = null;
            AudioClipMappings.TryGetValue( sfx, out clip );
            float volume_mulitplier = 1.0f;
            AudioClipVolumeMultipliers.TryGetValue( sfx, out volume_mulitplier );
            if( clip != null )
            {
                buffer.InitalizeWithAudioClip( clip, volume_mulitplier );
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
        if( sfx == SFXEnum.NONE )
            return;

        lock( SFXBuffers )
        {
            SFXBuffers[sfx].WriteSFXToBuffer();
        }
    }

    public static float CachedLog( int value )
    {
        return LogCache[Mathf.Clamp( value, 0, 100 )];
    }
}

class SFXRingBuffer
{
    private float[] buffer = new float[50000];
    private int[] clip_counter_buffer = new int[50000]; // parallel with buffer - maintains how many clips are stored at each index for attenuation
    private int buffer_index;
    private AudioClip audio_clip;
    private float audio_clip_volume_multiplier;
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
                    * ( 1.0f + SFXManager.CachedLog( clip_counter_buffer[buffer_index] ) ) // increase volume slightly so attenuation isn't linear decrease
                    * audio_clip_volume_multiplier; // factor in clip specific volume multiplier

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

    public void InitalizeWithAudioClip( AudioClip clip, float volume_multiplier )
    {
        // store clip
        audio_clip = clip;
        audio_clip_volume_multiplier = volume_multiplier;

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
            if( sfx == SFXEnum.NONE )
                continue;

            if( !target.AudioClipMappings.ContainsKey( sfx ) )
                target.AudioClipMappings.Add( sfx, null );
            if( !target.AudioClipVolumeMultipliers.ContainsKey( sfx ) )
                target.AudioClipVolumeMultipliers.Add( sfx, 1.0f );
        }

        SectionHeader( "Sound Effect Mappings" );

        foreach( SFXEnum sfx in Enum.GetValues( typeof( SFXEnum ) ) )
        {
            if( sfx == SFXEnum.NONE )
                continue;

            AudioClip clip = target.AudioClipMappings[sfx];
            AudioClipField( ref clip, sfx.ToString() );
            target.AudioClipMappings[sfx] = clip;

            float volume_multiplier = target.AudioClipVolumeMultipliers[sfx];
            FloatSliderField( ref volume_multiplier, 0.0f, 2.0f, $"{sfx} Volume Multiplier" );
            target.AudioClipVolumeMultipliers[sfx] = volume_multiplier;

            Seperator();
        }
    }
}
#endif