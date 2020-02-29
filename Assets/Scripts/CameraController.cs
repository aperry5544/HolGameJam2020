using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public SpriteRenderer map;

    void Start()
    {
        transform.position = new Vector3(0f, 0f, -1f);

        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = map.bounds.size.x / map.bounds.size.y;

        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = map.bounds.size.y / 2;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = map.bounds.size.y / 2 * differenceInSize;
        }
    }
}
