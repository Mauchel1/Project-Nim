using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class NoiseData : UpdatableData
{
    public Noise.NormaliseMode normalisedMode;

    public float noiseScale = 0.3f;
    public int octaves = 1;
    [Range(0, 1)]
    public float persistance = .5f;
    public float lacunatiry = 2f;
    public Vector2 offset;
    public int seed;

    protected override void OnValidate()
    {
        if (lacunatiry < 1)
        {
            lacunatiry = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }

        base.OnValidate();
    }
}
