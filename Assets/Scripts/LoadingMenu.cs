using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingMenu : MonoBehaviour
{
    public RectTransform barAnimation;
    public float angle = 0;

    void Update()
    {
        angle -= 100 * Time.deltaTime;
        barAnimation.rotation = Quaternion.Euler(0, 0, angle);
    }
}
