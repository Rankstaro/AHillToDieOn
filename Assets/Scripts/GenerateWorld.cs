using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MLAPI;

public class GenerateWorld : MonoBehaviour
{
    public GameObject worldPrefab;
    [System.NonSerialized]
    public GameObject worldInstance;

    void Start()
    {
        worldInstance = Instantiate(worldPrefab, Vector3.zero, Quaternion.identity);
        Done();
    }

    void Done()
    {
        worldInstance.GetComponent<NetworkObject>().Spawn();
    }
}
