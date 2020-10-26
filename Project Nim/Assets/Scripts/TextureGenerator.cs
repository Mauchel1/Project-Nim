using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int heigth)
    {
        Texture2D texture = new Texture2D(width, heigth);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromHightMap(HeightMap heigthMap)
    {
        int width = heigthMap.values.GetLength(0);
        int heigth = heigthMap.values.GetLength(1);

        Color[] colourMap = new Color[width * heigth];
        for (int y = 0; y < heigth; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(heigthMap.minValue, heigthMap.maxValue,heigthMap.values[x, y]));

            }
        }
        return TextureFromColorMap(colourMap, width, heigth);
    }
}
