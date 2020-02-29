using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private Rigidbody2D[] allLevelRigidbodies = null;
    private Collider2D[] allLevelColliders = null;

    //Spawn Circle Properties
    [SerializeField]
    private float circleRadius = 5f;
    [SerializeField]
    private Vector2 circleCenter = new Vector2(); 

    void OnEnable()
    {
        allLevelRigidbodies = gameObject.GetComponentsInChildren<Rigidbody2D>();
        allLevelColliders = gameObject.GetComponentsInChildren<Collider2D>();
    }

    public void DisableLevelPhysics()
    {
        if (allLevelRigidbodies != null)
        {
            for (int i = 0; i < allLevelRigidbodies.Length; ++i)
            {
                allLevelRigidbodies[i].isKinematic = true;
            }
        }

        if (allLevelColliders != null)
        {
            for (int i = 0; i < allLevelColliders.Length; ++i)
            {
                allLevelColliders[i].enabled = false;
            }
        }
    }

    public void EnableLevelPhysics()
    {
        for (int i = 0; i < allLevelRigidbodies.Length; ++i)
        {
            allLevelRigidbodies[i].isKinematic = false;
        }

        for (int i = 0; i < allLevelColliders.Length; ++i)
        {
            allLevelColliders[i].enabled = true;
        }
    }

    public List<Vector2> GetSpawnPositions(int playerCount)
    {
        List<Vector2> spawnPositions = new List<Vector2>();

        float anglePerPlayer = 360f / playerCount;

        Vector2 spawnPosition = new Vector2();

        for (int i = 0; i < playerCount; i++)
        {
            spawnPosition.x = circleCenter.x + circleRadius * Mathf.Sin(anglePerPlayer * i * Mathf.Deg2Rad);
            spawnPosition.y = circleCenter.y + circleRadius * Mathf.Cos(anglePerPlayer * i * Mathf.Deg2Rad);
            spawnPositions.Add(spawnPosition);
        }

        return spawnPositions;
    }
}
