using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardController : MonoBehaviour
{
    private float curTime = 0.0f;
    private float travelTime = 5.0f;
    private float curPos = 0.0f;

    private float explosionTime = 0.5f;
    private float curExpTime = 0.0f;

    private Vector2 spawnPoint, landingPoint;
    private Collider2D col = null;
    private SpriteRenderer explosion = null;

    public void Initialize(Vector2 _pos, float _travelTime)
    {
        landingPoint = _pos;
        travelTime = _travelTime;
        spawnPoint = transform.position = new Vector3(landingPoint.x, landingPoint.y + 5f, 0f);
        col = GetComponent<Collider2D>();
        explosion = GetComponentsInChildren<SpriteRenderer>()[1];
        explosion.enabled = false;
        col.enabled = false;
    }

    public void DoUpdate()
    {
        if (curTime >= travelTime)
        {
            Explode();
        }
        else
        {
            curTime += Time.deltaTime;
            curPos = Map(curTime, 0f, travelTime, spawnPoint.y, landingPoint.y);

            transform.position = new Vector3(landingPoint.x, curPos, 0f);
        }
    }

    private float Map(float _value, float _fromfrom, float _fromto, float _tofrom, float _toto)
    {
        return ((_value - _fromfrom) / (_fromto - _fromfrom)) * (_toto - _tofrom) + _tofrom;
    }

    public void Explode()
    {
        explosion.enabled = true;

        if (curExpTime >= explosionTime)
        {
            Destroy(gameObject);
        }

        col.enabled = true;
        curExpTime += Time.deltaTime;
    }
}
