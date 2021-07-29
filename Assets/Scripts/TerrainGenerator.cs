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
    public Transform terrain;

    public GameObject tree;
    public Transform treeParent;
    public GameObject monster;
    public Transform monsterParent;
    public GameObject grave;
    public Transform graveParent;

    [Range(0,105)]
    public float treeNum;
    [Range(0, 105)]
    public float monsterNum;
    [Range(0, 105)]
    public float graveNum;

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
        DrawTrees(noiseMap);
    }

    public void DrawTrees(float[,] noiseMap)
    {
        float treeFreq = treeNum / mapSize;
        float waterHeight = 0.44f;
        for (int y = 0; y < mapSize - 1; y++)
        {
            //for each noiseMap[x, y] (vertex) in the noise map
            for (int x = 0; x < mapSize - 1; x++)
            {
                float[] vertices = { noiseMap[x + 1, y], noiseMap[x + 1, y + 1], noiseMap[x, y + 1]};
                for (int i = 0; i < 3; i++)
                {
                    // if one of the points is above water
                    if (vertices[i] >= waterHeight || noiseMap[x, y] >= waterHeight)
                    {
                        float diceroll = (float)Random.Range(0f, 1f);
                        if (diceroll <= treeFreq)
                        {
                            float modifier = Random.Range(0f, 1f);
                            float lower, higher, a, b;
                            float xPos, yPos, zPos;
                            float xRot = 0;
                            float zRot = 0;
                            float angle = 0;
                            float scale = terrainData.scale;
                            
                            if (modifier == 0)
                            {
                                yPos = noiseMap[x, y];
                            }
                            else if (modifier == 1)
                            {
                                yPos = vertices[i];
                            }
                            else
                            {
                                if (noiseMap[x, y] > vertices[i])
                                {
                                    higher = noiseMap[x, y];
                                    lower = vertices[i];
                                }
                                else
                                {
                                    lower = noiseMap[x, y];
                                    higher = vertices[i];
                                }

                                a = higher - lower;

                                // diagonal
                                if (i == 1) b = 1.41421f;
                                else b = 1;

                                if (noiseMap[x, y] == lower)
                                {
                                    angle = Mathf.Atan2(a, b) * 100;
                                    yPos = (a * modifier) + lower;
                                }
                                else
                                {
                                    angle = -1 * Mathf.Atan2(a, b) * 100;
                                    yPos = (a * (1 - modifier)) + lower;
                                }
                            }

                            if (i == 0)
                            {
                                xPos = ((x - mapSize / 2) * scale) + modifier * scale;
                                zPos = (y - mapSize / 2) * scale;
                                xRot = angle;
                            }
                            else if (i == 2)
                            {
                                xPos = ((x - mapSize / 2) * scale);
                                zPos = ((y - mapSize / 2) * scale) + modifier * scale;
                                zRot = angle;
                            }
                            else // diagonal
                            {
                                xPos = ((x - mapSize / 2) * scale) + modifier * scale;
                                zPos = ((y - mapSize / 2) * scale) + modifier * scale;
                                xRot = angle;
                                zRot = angle;
                            }

                            if (yPos > waterHeight && yPos != 1)
                            {
                                yPos *= terrainData.meshHeightMultiplier;
                                Vector3 pos = new Vector3(xPos, yPos, -zPos);
                                Quaternion rot = Quaternion.Euler(zRot, 0, xRot); 
                                // pick a random tree model and spawn it
                                GameObject treeInstance = Instantiate(tree, pos, rot, treeParent);
                                treeInstance.transform.GetChild(0).rotation = Quaternion.Euler(0, (float)Random.Range(0f, 360f), 0);
                            }
                        }
                    }
                }
            }
        }
    }

    public void DrawMesh(MeshData meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshCollider.sharedMesh = meshData.CreateMesh();
    }
}