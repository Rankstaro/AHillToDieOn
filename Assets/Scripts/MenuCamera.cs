using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    void Update()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, 5f * Time.deltaTime);
        transform.LookAt(new Vector3(0,10,0));
    }
}
