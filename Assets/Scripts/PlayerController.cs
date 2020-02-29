using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject fist;

    [SerializeField]
    private float fistVelocity = 5;

    [SerializeField]
    private float fistRotation = 5;

    private bool active = false;

    public void Activate()
    {
        active = true;
    }
    public void Deactivate()
    {
        active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(active)
        {

        }
        else
        {
            RotateFist();
        }
    }

    private void RotateFist()
    {
        fist.transform.Rotate(new Vector3(0, 0, 1), fistRotation * Time.deltaTime);
    }
}
