using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardManager : MonoBehaviour
{
    [SerializeField]
    private float suddenDeathTime = 30f;

    [SerializeField]
    private float startPace = 5.0f;

    [SerializeField]
    private float finalPace = 1.0f;

    [SerializeField]
    private float travelTime = 5.0f;

    [SerializeField]
    private GameObject hazardPrefab = null;

    [SerializeField]
    private GameObject shadowPrefab = null;

    private bool isMaxSpeed = false;
    private bool shouldSpawn = false;

    private float curPace = 0.0f;
    private float lastTime = 0.0f;

    private float roundStartTime = 0.0f;

    private List<HazardController> hazardList;
    private List<GameObject> shadowList;
    private static HazardManager instance = null;

    public static HazardManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HazardManager>();
            }

            return instance;
        }
    }

    public void Start()
    {
        curPace = startPace;
        hazardList = new List<HazardController>();
        shadowList = new List<GameObject>();
        lastTime = Time.time;
    }

    // Update
    public void Active()
    {
        if (Time.time - roundStartTime < suddenDeathTime) // Check if sudden death has started.
        {
            return;
        }
        if (Time.time - lastTime >= curPace) // Check if we should spawn a hazard
        {
            lastTime = Time.time;
            shouldSpawn = true;
        }
        if (shouldSpawn) // If we should spawn a hazard
        {
            GameObject newHaz = Instantiate(hazardPrefab, Vector3.zero, Quaternion.identity);
            HazardController newHazard = newHaz.GetComponent<HazardController>();
            GameObject newShad = Instantiate(shadowPrefab, Vector3.zero, Quaternion.identity);

            hazardList.Add(newHazard);
            shadowList.Add(newShad);

            Vector2 spawnPoint = new Vector2(Random.Range(-10f, 10f), Random.Range(-5f, 5f));
            newHazard.Initialize(spawnPoint, travelTime);
            newShad.transform.position = spawnPoint;

            shouldSpawn = false;
        }
        for (int i = 0; i < hazardList.Count; i++)
        {
            if (hazardList[i] != null)
            {
                hazardList[i].DoUpdate();
            }
        }
        if (!isMaxSpeed)
        {
            curPace -= 0.001f;

            if (curPace <= finalPace)
            {
                curPace = finalPace;
                isMaxSpeed = true;
            }
        }
    }

    public void StartRound()
    {
        roundStartTime = Time.time;
        curPace = startPace;
        isMaxSpeed = false;
    }

    public void EndRound()
    {
        if (hazardList.Count > 0)
        {
            for (int i = 0; i < hazardList.Count; i++)
            {
                if (hazardList[i] != null)
                {
                    Destroy(hazardList[i].gameObject);
                }
            }
        }
        if (shadowList.Count > 0)
        {
            for (int i = 0; i < shadowList.Count; i++)
            {
                if (shadowList[i] != null)
                {
                    Destroy(shadowList[i].gameObject);
                }
            }
        }

        hazardList.Clear();
        shadowList.Clear();
    }
}
