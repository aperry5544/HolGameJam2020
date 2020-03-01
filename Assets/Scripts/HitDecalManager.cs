using System.Collections.Generic;
using UnityEngine;

public class HitDecalManager : MonoBehaviour
{
    private static HitDecalManager instance = null;

    public static HitDecalManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HitDecalManager>();
            }

            return instance;
        }
    }

    [SerializeField]
    private float hitDecalStayTime = 0;

    [SerializeField]
    private List<HitDecalController> hitDecals = new List<HitDecalController>();
    HitDecalController nextDecalController = null;
    private int nextDecal;

    private Dictionary<HitDecalController, float> activeDecals = new Dictionary<HitDecalController, float>();
    private List<HitDecalController> toRemoveDecals = new List<HitDecalController>();

    [SerializeField]
    private List<string> bodyHitStrings = new List<string>();

    [SerializeField]
    private List<string> fistHitStrings = new List<string>();

    private float currentTime;

    public void BodyHit(Vector2 hitLocation)
    {
        nextDecalController = hitDecals[nextDecal];
        CheckForExistingDecal(nextDecalController);
        nextDecalController.Activate(bodyHitStrings[Random.Range(0, bodyHitStrings.Count - 1)], hitLocation, true);
        activeDecals.Add(nextDecalController, Time.time);
        GetNextDecal();
    }

    public void FistHit(Vector2 hitLocation)
    {
        nextDecalController = hitDecals[nextDecal];
        CheckForExistingDecal(nextDecalController);
        nextDecalController.Activate(fistHitStrings[Random.Range(0, fistHitStrings.Count - 1)], hitLocation, false);
        activeDecals.Add(nextDecalController, Time.time);
        GetNextDecal();
    }

    private void CheckForExistingDecal(HitDecalController nextDecalController)
    {
        if (activeDecals.ContainsKey(nextDecalController))
        {
            hitDecals[nextDecal].Deactivate();
            activeDecals.Remove(nextDecalController);
        }
    }

    private void GetNextDecal()
    {
        nextDecal++;
        if (nextDecal >= hitDecals.Count)
        {
            nextDecal = 0;
        }
    }

    private void Update()
    {
        currentTime = Time.time;
        foreach (KeyValuePair<HitDecalController, float> item in activeDecals)
        {
            if(currentTime - item.Value > hitDecalStayTime)
            {
                item.Key.Deactivate();
                toRemoveDecals.Add(item.Key);
                
            }
        }

        foreach (HitDecalController hitDecalController in toRemoveDecals)
        {
            activeDecals.Remove(hitDecalController);
        }

        toRemoveDecals.Clear();
    }

    


}
