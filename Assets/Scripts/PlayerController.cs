using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject shoulder = null;
    [SerializeField]
    private GameObject fist = null;

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

    private void Reflect(Vector2 tangent)
    {
        float angleOfIncidence = Vector2.SignedAngle(fist.transform.position - transform.position, tangent);

        shoulder.transform.Rotate(new Vector3(0, 0, angleOfIncidence * 2));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contact = collision.GetContact(0);

        Debug.DrawLine(contact.point, contact.point + (contact.normal*2), Color.red, 5);
        Debug.DrawLine(contact.point, contact.point + (new Vector2(-contact.normal.y, contact.normal.x) * 2), Color.blue, 5);
        
        Reflect(new Vector2(-contact.normal.y, contact.normal.x));
    }
}
