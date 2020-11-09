using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;

public class TerrainGenerator : MonoBehaviour
{

    const float viewerMoveTresholdForChunkUpdate = 25f;
    const float sqrtViewerMoveTresholdForChunkUpdate = viewerMoveTresholdForChunkUpdate * viewerMoveTresholdForChunkUpdate;

    public int colliderLODIndex;
    public LODInfo[] detailLevels;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureSettings;

    public Transform viewer;
    public Material mapMaterial;

    Vector2 viewerPosition;
    Vector2 viewerPositionOld;
    float meshWorldSize;
    int chunksVisibleViewDistance;

    Dictionary<Vector2, Terrainchunk> terrainChunkDict = new Dictionary<Vector2, Terrainchunk>();
    List<Terrainchunk> visibleTerrainChunks = new List<Terrainchunk>();

    void Start()
    {

        textureSettings.ApplyToMaterial(mapMaterial);
        textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

        float maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        meshWorldSize = meshSettings.meshWorldSize;
        chunksVisibleViewDistance = Mathf.RoundToInt(maxViewDistance / meshWorldSize);

        UpdateVisibleChunks();
    }

    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        if( viewerPosition != viewerPositionOld)
        {
            foreach(Terrainchunk chunk in visibleTerrainChunks)
            {
                chunk.UpdateCollisionMesh();
            }
        }

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrtViewerMoveTresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    public List<Terrainchunk> GetTerrainchunks(Vector3 nearestPoint, float radius = 0)
    {
        List<Terrainchunk> returnChunks = new List<Terrainchunk>();
        returnChunks.Add(GetNearestChunk(nearestPoint));

        //Check for Neibors
        Terrainchunk t = GetNearestChunk(new Vector3(nearestPoint.x + radius, nearestPoint.y, nearestPoint.z));
        if (returnChunks[0] != null)
        {
            //x neibors
            if (GetNearestChunk(new Vector3(nearestPoint.x + radius, nearestPoint.y, nearestPoint.z)) != returnChunks[0])
            {
                returnChunks.Add(GetNearestChunk(new Vector3(nearestPoint.x + radius, nearestPoint.y, nearestPoint.z)));
            }
            else if (GetNearestChunk(new Vector3(nearestPoint.x - radius, nearestPoint.y, nearestPoint.z)) != returnChunks[0])
            {
                returnChunks.Add(GetNearestChunk(new Vector3(nearestPoint.x - radius, nearestPoint.y, nearestPoint.z)));
            }
            //y neibors
            if (GetNearestChunk(new Vector3(nearestPoint.x, nearestPoint.y, nearestPoint.z + radius)) != returnChunks[0])
            {
                returnChunks.Add(GetNearestChunk(new Vector3(nearestPoint.x, nearestPoint.y, nearestPoint.z + radius)));
            }
            else if (GetNearestChunk(new Vector3(nearestPoint.x, nearestPoint.y, nearestPoint.z - radius)) != returnChunks[0])
            {
                returnChunks.Add(GetNearestChunk(new Vector3(nearestPoint.x, nearestPoint.y, nearestPoint.z - radius)));
            }
            if (returnChunks.Count > 2)
            {
                returnChunks.Add(GetTerrainchunkWithCoords(new Vector2(returnChunks[1].coord.x, returnChunks[2].coord.y)));
            }
        }
        return returnChunks;
    }

    public Terrainchunk GetNearestChunk(Vector3 nearestPoint)
    {
        for (int i = 0; i < visibleTerrainChunks.Count; i++)
        {
            if (Mathf.RoundToInt(nearestPoint.x / meshWorldSize) == visibleTerrainChunks[i].coord.x && Mathf.RoundToInt(nearestPoint.z / meshWorldSize) == visibleTerrainChunks[i].coord.y)
            {
                return visibleTerrainChunks[i];
            }
        }
        return null;
    }

    public Terrainchunk GetTerrainchunkWithCoords(Vector2 coords)
    {
        for (int i = 0; i < visibleTerrainChunks.Count; i++)
        {
            if (visibleTerrainChunks[i].coord.x == coords.x && visibleTerrainChunks[i].coord.y == coords.y)
            {
                return visibleTerrainChunks[i];
            }
        }
        return null;
    }

        void UpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        for (int i = visibleTerrainChunks.Count-1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

        for (int yOffset = -chunksVisibleViewDistance; yOffset <= chunksVisibleViewDistance; yOffset++)
        {
            for (int xOffset = -chunksVisibleViewDistance; xOffset <= chunksVisibleViewDistance; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                {
                    if (terrainChunkDict.ContainsKey (viewedChunkCoord))
                    {
                        terrainChunkDict[viewedChunkCoord].UpdateTerrainChunk();
                    } else
                    {
                        Terrainchunk newChunk = new Terrainchunk(viewedChunkCoord, heightMapSettings,meshSettings, detailLevels, colliderLODIndex, transform, viewer, mapMaterial);
                        terrainChunkDict.Add(viewedChunkCoord, newChunk);
                        newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
                        newChunk.Load();
                    }
                }
            }
        }
    }

    void OnTerrainChunkVisibilityChanged(Terrainchunk chunk, bool isVisible)
    {
        if (isVisible)
        {
            visibleTerrainChunks.Add(chunk);
        }
        else
        {
            visibleTerrainChunks.Remove(chunk);
        }
    }

}


    [System.Serializable]
    public struct LODInfo
    {
        [Range(0,MeshSettings.numSupportedLODs-1)] 
        public int lod;
        public float visibleDstThreshold;

        public float sqrtVisibleDistanceThreshhold
        {
            get
            {
                return visibleDstThreshold * visibleDstThreshold;
            }
        }
    }
