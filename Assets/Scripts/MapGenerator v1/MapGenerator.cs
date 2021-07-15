using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode {NoiseMap, Mesh, Falloff};
    public DrawMode drawMode = DrawMode.Mesh;

    public TerrainData terrainData;
    public NoiseData noiseData;
    public TextureData textureData;

    public Material terrainMaterial;

    private const int mapSize = 105; // vertex limit of flatshaded mesh

    public bool autoUpdate;

    private void Awake()
    {
        textureData.ApplyToMaterial(terrainMaterial);
        DrawMapInEditor();
    }

    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }

    void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);
    }

    public MapData GenerateMap()
    {
        int seed = noiseData.seed;
        if (noiseData.randomSeed == true)
        {
            seed = (int)Random.Range(-999999999, 999999999);
        }

        float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, mapSize, seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.offset);
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

        return new MapData(noiseMap);
    }

    public void DrawMapInEditor()
    {
        textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);
        MapData mapData = GenerateMap();
        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.noiseMap));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            //display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.noiseMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, terrainData.flatshading));
        }
        else if (drawMode == DrawMode.Falloff)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(Noise.GenerateFalloffMap(mapSize, noiseData.av, noiseData.bv)));
        }
    }

    private void OnValidate()
    {
        if (terrainData != null)
        {
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            terrainData.OnValuesUpdated += OnValuesUpdated;
        }
        if (noiseData != null)
        {
            noiseData.OnValuesUpdated -= OnValuesUpdated;
            noiseData.OnValuesUpdated += OnValuesUpdated;
        }
        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
    }
}

public struct MapData
{
    public readonly float[,] noiseMap;

    public MapData(float[,] noiseMap)
    {
        this.noiseMap = noiseMap;
    }
}