using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flickering : MonoBehaviour
{
    public float rangeMax, rangeMin, lanternIntensity, glowSpeed;
    private Light lantern;
    private bool up;

    void Start()
    {
        lantern = GetComponent<Light>();
        StartCoroutine(flicker());
    }
    private void Update()
    {
        if (lantern.isActiveAndEnabled)
        {
            if (lantern.range >= rangeMax)
            {
                up = false;
            }
            else if (lantern.range <= rangeMin)
            {
                up = true;
            }
            if (up)
            {
                lantern.range += glowSpeed * Time.deltaTime;
            }
            else
            {
                lantern.range -= glowSpeed * Time.deltaTime;
            }
        }
    }

    IEnumerator flicker()
    {
        while (lantern.isActiveAndEnabled)
        {
            lantern.intensity = lanternIntensity;
            yield return new WaitForSeconds(3.0f);
            lantern.intensity = Random.Range(lanternIntensity - 0.3f, lanternIntensity);
            yield return new WaitForSeconds(0.1f);
            lantern.intensity = lanternIntensity;
        }
    }

}
