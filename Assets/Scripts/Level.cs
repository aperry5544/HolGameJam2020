using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private Rigidbody2D[] allLevelRigidbodies = null;
    private Collider2D[] allLevelColliders = null;

    void Start()
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
}
