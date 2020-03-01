using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardController : MonoBehaviour
{
    private float curTime = 0.0f;
    private float travelTime = 5.0f;

    private Vector2 spawnPoint, landingPoint;
    private AnimationCurve curve;

    public void Initialize(Vector2 _pos, float _travelTime, AnimationCurve _animCurve)
    {
        landingPoint = _pos;
        curve = _animCurve;
        travelTime = _travelTime;
        spawnPoint = transform.position = new Vector3(_pos.x + Random.Range(-8f, 8f), _pos.y + 5f, 0f);
    }

    public void DoUpdate()
    {
        curTime += Time.deltaTime;
        float xPos = Map(curTime, 0f, travelTime, spawnPoint.x, landingPoint.x);
        float yPos = curve.Evaluate(Map(curTime, 0f, travelTime, 0f, 1f));
        //Debug.Log("animCurve evaluation:\ncurTime-startTime: " + (curTime - startTime).ToString()
        //    + "\ntravelTime: " + travelTime.ToString() + "\nEvaluation:" + yPos.ToString());

        transform.position = new Vector3(xPos, yPos, 0f);
    }

    private float Map(float _value, float _fromfrom, float _fromto, float _tofrom, float _toto)
    {
        return ((_value - _fromfrom) / (_fromto - _fromfrom)) * (_toto - _tofrom) + _tofrom;
    }
}
