using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TerrainData : UpdatableData
{

    public float meshHeigthMultiplyer = 1;
    public AnimationCurve meshHeigthCurve;

    public bool useFlatshading;

    public float uniformScale = 2f;

    public float minHeight
    {
        get
        {
            return uniformScale * meshHeigthMultiplyer * meshHeigthCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get
        {
            return uniformScale * meshHeigthMultiplyer * meshHeigthCurve.Evaluate(1);
        }
    }
}
