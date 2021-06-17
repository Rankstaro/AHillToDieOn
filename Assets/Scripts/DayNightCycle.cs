using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public bool isDay = false;
    public float timeSpeed = 10f;
    public GameObject sun;
    public GameObject moon;
    public GameObject stars;

    private void Update()
    {
        orbit(sun);
        orbit(moon);

        if (sun.transform.position.y <= 0)
        {
            isDay = false;
        }
        else
        {
            isDay = true;
        }
        if (!isDay)
        {
            stars.SetActive(true);
        }
        else
        {
            stars.SetActive(false);
        }

    }

    public void Awake()
    {
        if (!isDay)
        {
            swapPosition(sun);
            swapPosition(moon);
        }
    }

    public void swapPosition(GameObject lightSource)
    {
        lightSource.transform.position = new Vector3(lightSource.transform.position.x, lightSource.transform.position.y * -1, lightSource.transform.position.z);
        lightSource.transform.LookAt(Vector3.zero);
    }

    public void orbit(GameObject lightSource)
    {
        lightSource.transform.RotateAround(Vector3.zero, Vector3.right, timeSpeed * Time.deltaTime);
        lightSource.transform.LookAt(Vector3.zero);
    }
}
