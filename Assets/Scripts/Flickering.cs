using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flickering : MonoBehaviour
{
    private Light lantern;
    private bool up;

    void Start()
    {
        lantern = GetComponent<Light>();
        InvokeRepeating("flicker", .01f, 3.0f);
    }
    private void Update()
    {
        if (lantern.range >= 12)
        {
            up = false;
        }
        else if (lantern.range <= 8)
        {
            up = true;
        }
        if (up)
        {
            lantern.range += 0.01f;
        }
        else
        {
            lantern.range -= 0.01f;
        }
    }

    public void flicker()
    {
        lantern.intensity = Random.Range(1.7f, 2f);
        StartCoroutine(wait());
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.1f);
        lantern.intensity = 2f;
    }

}
