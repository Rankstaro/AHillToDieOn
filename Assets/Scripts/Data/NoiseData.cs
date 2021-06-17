using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : UpdatableData
{
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public bool randomSeed;
    public int seed;
    public Vector2 offset;
    public bool useFalloff;
    public float av;
    public float bv;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
        if (av < 1)
        {
            av = 1;
        }
        if (bv < 1)
        {
            bv = 1;
        }

        base.OnValidate();
    }
#endif
}
