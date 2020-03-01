﻿using System.Collections;
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
    private AnimationCurve angle = null;

    [SerializeField]
    private GameObject hazardPrefab = null;

    [SerializeField]
    private Sprite shadowSprite = null;

    private bool isMaxSpeed = false;
    private bool shouldSpawn = false;

    private float curPace = 0.0f;
    private float lastTime = 0.0f;

    private float roundStartTime = 0.0f;

    private List<HazardController> hazardList;    
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
        DontDestroyOnLoad(gameObject);
        curPace = startPace;
        hazardList = new List<HazardController>();
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
            GameObject newObj = Instantiate(hazardPrefab, Vector3.zero, Quaternion.identity);
            HazardController newHazard = newObj.GetComponent<HazardController>();
            hazardList.Add(newHazard);

            Vector2 spawnPoint = new Vector2(Random.Range(-10f, 10f), Random.Range(-5f, 5f));
            newHazard.Initialize(spawnPoint, travelTime, angle);
            shouldSpawn = false;
        }
        for (int i = 0; i < hazardList.Count; i++)
        {
            hazardList[i].DoUpdate();
        }
        if (!isMaxSpeed)
        {
            curPace = Mathf.Lerp(startPace, finalPace, Time.deltaTime);

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

        hazardList.Clear();
    }
}
