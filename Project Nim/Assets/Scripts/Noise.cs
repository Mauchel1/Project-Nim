using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise 
{
    public enum NormaliseMode { Local, Global};

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeigth, NoiseSettings settings, Vector2 sampleCenter)
    {
        float[,] noiseMap = new float[mapWidth, mapHeigth];

        System.Random prng = new System.Random(settings.seed);
        Vector2[] octaveOffsets = new Vector2[settings.octaves];

        float maxPossibleHeigth = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < settings.octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCenter.x;
            float offsetY = prng.Next(-100000, 100000) - settings.offset.y - sampleCenter.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeigth += amplitude;
            amplitude *= settings.persistance;
        }

        float maxLocalNoiseHeigth = float.MinValue;
        float minLocalNoiseHeigth = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeigth = mapHeigth / 2f;

        for (int y = 0; y < mapHeigth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                amplitude = 1;
                frequency = 1;
                float noiseHeigth = 0;

                for (int i = 0; i < settings.octaves; i++)
                {

                    float sampleX = (x-halfWidth + octaveOffsets[i].x) / settings.scale * frequency ;
                    float sampleY = (y-halfHeigth + octaveOffsets[i].y) / settings.scale * frequency ;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeigth += perlinValue * amplitude;
                    
                    amplitude *= settings.persistance;
                    frequency *= settings.lacunatiry;
                }

                if(noiseHeigth > maxLocalNoiseHeigth)
                {
                    maxLocalNoiseHeigth = noiseHeigth;
                } 
                if (noiseHeigth < minLocalNoiseHeigth)
                {
                    minLocalNoiseHeigth = noiseHeigth;
                }

                noiseMap[x, y] = noiseHeigth;

                if (settings.normalisedMode == NormaliseMode.Global)
                {
                    float normalizedHeigth = (noiseMap[x, y] + 1) / (maxPossibleHeigth / 0.9f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeigth, 0, int.MaxValue);
                }

            }
        }

        if (settings.normalisedMode == NormaliseMode.Local)
        {
            for (int y = 0; y < mapHeigth; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeigth, maxLocalNoiseHeigth, noiseMap[x, y]);
                } 
            }
        }

        return noiseMap;
    }
}

[System.Serializable]
public class NoiseSettings{
    public Noise.NormaliseMode normalisedMode;

    public float scale = 40f;
    public int octaves = 4;
    [Range(0, 1)]
    public float persistance = .6f;
    public float lacunatiry = 2f;
    public Vector2 offset;
    public int seed;

    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunatiry = Mathf.Max(lacunatiry, 1);
        persistance = Mathf.Clamp01(persistance);
    }

}