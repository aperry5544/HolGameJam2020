using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject shoulder;
    [SerializeField]
    private GameObject fist;

    [SerializeField]
    private float fistVelocity = 5;

    [SerializeField]
    private float shoulderRotation = 5;

    [SerializeField]
    private float fistActivePos = 0.5f;
    [SerializeField]
    private float fistInactivePos = 0.4f;

    private bool active = false;

    [SerializeField]
    private Vector3 fistDirection;

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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            fist.transform.localPosition = new Vector3(0, fistActivePos, 0);
            active = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            fist.transform.localPosition = new Vector3(0, fistInactivePos, 0);
            active = false;
        }


        if(active)
        {
            FireFist();
        }
        else
        {
            RotateFist();
        }
    }

    private void FireFist()
    {
        fistDirection = fist.transform.position - transform.position;
        gameObject.transform.Translate(fistDirection.normalized * fistVelocity * Time.deltaTime);
    }

    private void RotateFist()
    {
        shoulder.transform.Rotate(new Vector3(0, 0, 1), shoulderRotation * Time.deltaTime);
    }
}
