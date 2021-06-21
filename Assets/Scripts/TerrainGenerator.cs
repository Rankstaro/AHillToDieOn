using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;

public class TerrainGenerator : NetworkBehaviour
{
    public TerrainData terrainData;
    public NoiseData noiseData;
    public TextureData textureData;

    public Material terrainMaterial;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    public NetworkVariableInt hostSeed = new NetworkVariableInt(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly, ReadPermission = NetworkVariablePermission.Everyone });

    private const int mapSize = 105; // vertex limit of flatshaded mesh

    private void Start()
    {
        DrawMap();
    }

    public override void NetworkStart()
    {
        if (IsOwner)
        {
            if (!noiseData.randomSeed)
            {
                hostSeed.Value = noiseData.seed;
            }
            else
            {
                hostSeed.Value = (int)Random.Range(-999999999, 999999999);
            }
        }

        DrawMap();
    }

    public float[,] GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, mapSize, hostSeed.Value, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.offset);
        if (noiseData.useFalloff)
        {
            float[,] falloffMap = Noise.GenerateFalloffMap(mapSize, noiseData.av, noiseData.bv);
            float[,] startHill = Noise.GenerateFalloffMap(mapSize, 10, 50);

            for (int y = 0; y < mapSize; y++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] + (1 - startHill[x, y]));
                }
            }
        }

        return noiseMap;
    }

    public void DrawMap()
    {
        textureData.ApplyToMaterial(terrainMaterial);
        textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);
        float[,] noiseMap = GenerateMap();
        DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, terrainData.flatshading));
    }

    public void DrawMesh(MeshData meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshCollider.sharedMesh = meshData.CreateMesh();
    }
}