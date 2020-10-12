using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise 
{
    public enum NormaliseMode { Local, Global};

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeigth, int seed, float scale, int octaves, float persistence, float lacunatiry, Vector2 offset, NormaliseMode normaliseMode)
    {
        float[,] noiseMap = new float[mapWidth, mapHeigth];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeigth = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeigth += amplitude;
            amplitude *= persistence;
        }


        if (scale <= 0)
        {
            scale = 0.0001f;
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

                for (int i = 0; i < octaves; i++)
                {

                    float sampleX = (x-halfWidth + octaveOffsets[i].x) / scale * frequency ;
                    float sampleY = (y-halfHeigth + octaveOffsets[i].y) / scale * frequency ;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeigth += perlinValue * amplitude;
                    
                    amplitude *= persistence;
                    frequency *= lacunatiry;
                }

                if(noiseHeigth > maxLocalNoiseHeigth)
                {
                    maxLocalNoiseHeigth = noiseHeigth;
                } else if (noiseHeigth < minLocalNoiseHeigth)
                {
                    minLocalNoiseHeigth = noiseHeigth;
                }

                noiseMap[x, y] = noiseHeigth;
            }
        }

        for (int y = 0; y < mapHeigth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (normaliseMode == NormaliseMode.Local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeigth, maxLocalNoiseHeigth, noiseMap[x, y]);
                } else
                {
                    float normalizedHeigth = (noiseMap[x, y] + 1) / (maxPossibleHeigth);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeigth, 0, int.MaxValue);
                }

            }
        }

        return noiseMap;
    }
}
