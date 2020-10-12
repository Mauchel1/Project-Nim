using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

//[RequireComponent(typeof(MeshFilter))]
public static class MeshGenerator
{

    public static MeshData GenerateTerrainMesh(float[,] heighMap, float heightMultiplyer, AnimationCurve _heigthCurve, int levelOfDetail)
    {
        AnimationCurve heigthCurve = new AnimationCurve(_heigthCurve.keys);
        int width = heighMap.GetLength(0);
        int heigth = heighMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (heigth - 1) / 2f;

        int meshSimpolificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimpolificationIncrement + 1;

        MeshData meshData = new MeshData(width-1, heigth-1);

        for (int y = 0, i = 0; y < heigth; y+= meshSimpolificationIncrement)
        {
            for (int x = 0; x < width; x+= meshSimpolificationIncrement)
            {
                //float y = Mathf.PerlinNoise(x * .5f, z * 0.5f) * .8f;
                meshData.vertices[i] = new Vector3(topLeftX + x, heigthCurve.Evaluate(heighMap[x, y]) * heightMultiplyer, topLeftZ - y);
                meshData.uvs[i] = new Vector2(x / (float)width, y / (float)heigth);

                if (x < width -1 && y < heigth - 1)
                {
                    meshData.AddTriangles(i, i + verticesPerLine + 1, i + verticesPerLine);
                    meshData.AddTriangles(i + verticesPerLine + 1, i, i + 1);
                }

                i++;
            }
                
        }

        /*
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;
                tris += 6;
                vert++;
            }
            vert++;
        }*/
        return meshData;
    }
}



   public class MeshData
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;

        int triangleIndex;

        public MeshData(int xSize, int zSize)
        {
            vertices = new Vector3[(xSize + 1) * (zSize + 1)];
            uvs = new Vector2[(xSize + 1) * (zSize + 1)];
            triangles = new int[xSize * zSize * 6];
        }

        public void AddTriangles(int a, int b, int c)
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            return mesh;
        }
    }



