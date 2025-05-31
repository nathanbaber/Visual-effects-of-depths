using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderWaterEffects : MonoBehaviour
{
    [Range (0.001f, 1f)]
    public float _pixelOffset;
    [Range(0.1f, 20f)]
    public float _NoiseScale;
    [Range(0.1f, 20f)]
    public float _NoiseFrequency;
    [Range(0.1f, 30f)]
    public float _NoiseSpeed;
    void Start()
    {
        
    }

    void Update()
    {
        _mat.SetFloat("_NoiseFrequency", _noiseFrequency);
        _mat.SetFloat("_NoiseSpeed", _noiseSpeed);
        _mat.SetFloat("_NoiseScale", _noiseScale);
        _mat.SetFloat("_PixelOffset", _pixelOffset);
    }
}
