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
            SetNight();
        }
    }

    private void Update()
    {
        Orbit(sun);
        Orbit(moon);

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
            StartCoroutine(Sunset());
        }
        else
        {
            stars.SetActive(false);
            StartCoroutine(Sunrise());
        }

    }

    public void SetNight()
    {
        Vector3 temp = sun.transform.position;
        sun.transform.position = moon.transform.position;
        moon.transform.position = temp;
        sun.transform.LookAt(Vector3.zero);
        moon.transform.LookAt(Vector3.zero);
    }

    public void Orbit(GameObject lightSource)
    {
        lightSource.transform.RotateAround(Vector3.zero, Vector3.right, timeSpeed * Time.deltaTime);
        lightSource.transform.LookAt(Vector3.zero);
    }

    IEnumerator Sunset()
    {
        while(sun.GetComponent<Light>().intensity > 0.01)
        {
            sun.GetComponent<Light>().intensity -= 0.001f;
        }
        yield return null;
    }

    IEnumerator Sunrise()
    {
        while (sun.GetComponent<Light>().intensity < 1)
        {
            sun.GetComponent<Light>().intensity += 0.001f;
        }
        yield return null;
    }
}
