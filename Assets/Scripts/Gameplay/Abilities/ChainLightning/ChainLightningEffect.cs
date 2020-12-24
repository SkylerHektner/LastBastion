using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ChainLightningEffect : MonoBehaviour
{
    Renderer rend;

    public float Alpha = 1.0f;
    private float alpha = 1.0f;
    public Vector2 NoiseScales = new Vector2( 70, 30 );
    private Vector2 noiseScales = new Vector2( 70, 30 );
    public Vector2 NoiseMagnitudes = new Vector2( 0.1f, 0.07f );
    private Vector2 noiseMagnitudes = new Vector2( 0.1f, 0.07f );
    public Vector2 NoiseSpeeds = new Vector2( 0.61f, 0.44f );
    private Vector2 noiseSpeeds = new Vector2( 0.61f, 0.44f );

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        if( alpha != Alpha )
        {
            alpha = Alpha;
            rend.material.SetFloat( "_Alpha", alpha );
        }

        if( noiseScales != NoiseScales )
        {
            noiseScales = NoiseScales;
            rend.material.SetVector( "_NoiseScales", noiseScales );
        }

        if( noiseMagnitudes != NoiseMagnitudes )
        {
            noiseMagnitudes = NoiseMagnitudes;
            rend.material.SetVector( "_NoiseMagnitudes", noiseMagnitudes );
        }

        if( noiseSpeeds != NoiseSpeeds )
        {
            noiseSpeeds = NoiseSpeeds;
            rend.material.SetVector( "_NoiseSpeeds", noiseSpeeds );
        }
    }

}
