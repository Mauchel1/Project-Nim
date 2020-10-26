using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MeshSettings : UpdatableData
{
    public const int numSupportedLODs = 5;
    public const int numSupportedChunkSizes = 9;
    public const int numSupportedFlatshadesChunkSizes = 3;
    public static readonly int[] supportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

    public bool useFlatshading;

    public float meshScale = 2f;

    [Range(0, numSupportedChunkSizes - 1)]
    public int chunkSizeIndex;
    [Range(0, numSupportedFlatshadesChunkSizes - 1)]
    public int flatshadedChunkSizeIndex;

    //num verts per line of mesh with highest res (LOD = 0). Includes 2 extra vertices for normal calculating
    public int numVertsPerLine
    {
        get
        {
            return supportedChunkSizes[(useFlatshading) ? flatshadedChunkSizeIndex : chunkSizeIndex] + 1;
        }
    }

    public float meshWorldSize
    {
        get
        {
            return (numVertsPerLine - 3) * meshScale;
        }
    }

}
