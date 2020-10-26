using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HeightMapSettings : UpdatableData
{

    public NoiseSettings noiseSettings;

    public float heigthMultiplyer = 30;
    public AnimationCurve heigthCurve;

    public float minHeight
    {
        get
        {
            return heigthMultiplyer * heigthCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get
        {
            return heigthMultiplyer * heigthCurve.Evaluate(1);
        }
    }


#if UNITY_EDITOR

    protected override void OnValidate()
    {
        noiseSettings.ValidateValues();
        base.OnValidate();
    }

#endif
}
