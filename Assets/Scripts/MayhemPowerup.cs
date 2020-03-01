using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MayhemPowerup : MonoBehaviour
{
    [SerializeField]
    private float newScale = 3;

    [SerializeField]
    private float scaleSpeed = 3;

    [SerializeField]
    private float lifespan = 3;

    [SerializeField]
    private float affectDuration = 3;


    private Vector3 targetScale = new Vector3(1, 1, 1);
    private Vector3 startScale = new Vector3(1, 1, 1);
    private float currentCompletion = 0;
    private void Start()
    {
        targetScale = new Vector3(newScale, newScale);
        startScale = transform.localScale;
    }

    void Update()
    {
        currentCompletion += scaleSpeed * Time.deltaTime;
        transform.localScale = Vector3.Lerp(startScale, targetScale, currentCompletion);
        if (currentCompletion >= 1)
        {
            Vector3 temp = startScale;
            startScale = targetScale;
            targetScale = temp;
            currentCompletion = 0;
        }
    }
}
