using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public bool isDay = false;
    public float timeSpeed = 1f;
    public GameObject sun;
    public GameObject moon;
    public GameObject stars;

    public void Awake()
    {
        if (!isDay)
        {
            setNight();
        }
    }

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

    public void setNight()
    {
        Vector3 temp = sun.transform.position;
        sun.transform.position = moon.transform.position;
        moon.transform.position = temp;
        sun.transform.LookAt(Vector3.zero);
        moon.transform.LookAt(Vector3.zero);
    }

    public void orbit(GameObject lightSource)
    {
        lightSource.transform.RotateAround(Vector3.zero, Vector3.right, timeSpeed * Time.deltaTime);
        lightSource.transform.LookAt(Vector3.zero);
    }
}
