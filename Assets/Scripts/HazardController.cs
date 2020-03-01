using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardController : MonoBehaviour
{
    [SerializeField]
    private float minTravel = 1f;

    [SerializeField]
    private float maxTravel = 10f;

    [SerializeField]
    private float dropHeight = 5f;

    private float curTime = 0.0f;
    private float startTime = 0.0f;
    private float travelTime = 0.0f;

    private Vector2 spawnPoint;
    private AnimationCurve curve;

    public void Initialize(Vector2 _pos, AnimationCurve _animCurve)
    {
        spawnPoint = _pos;
        curve = _animCurve;
        transform.position = new Vector2(spawnPoint.x + Random.Range(minTravel, maxTravel), spawnPoint.y + dropHeight);
        startTime = curTime = Time.time;
    }

    public void DoUpdate()
    {
        curTime += Time.deltaTime;
        float xPos, yPos;
        xPos = Mathf.Lerp(transform.position.x, spawnPoint.x, (curTime - startTime) / travelTime);
        yPos = curve.Evaluate((curTime - startTime) / travelTime);

        transform.position = new Vector3(xPos, yPos, 0f);
    }
}
