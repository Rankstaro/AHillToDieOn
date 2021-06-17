using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flickering : MonoBehaviour
{
    private Light light;
    private bool up;

    void Start()
    {
        light = GetComponent<Light>();
        InvokeRepeating("flicker", .01f, 3.0f);
    }
    private void Update()
    {
        if (light.range >= 12)
        {
            up = false;
        }
        else if (light.range <= 8)
        {
            up = true;
        }
        if (up)
        {
            light.range += 0.01f;
        }
        else
        {
            light.range -= 0.01f;
        }
    }

    public void flicker()
    {
        light.intensity = Random.Range(1.7f, 2f);
        StartCoroutine(wait());
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.1f);
        light.intensity = 2f;
    }

}
