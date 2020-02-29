﻿using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private KeyCode playerKey = KeyCode.A;
    [SerializeField]
    private GameObject shoulder = null;
    [SerializeField]
    private GameObject fist = null;
    [SerializeField]
    private Collider2D fistCollider = null;

    [SerializeField]
    private float fistVelocity = 5;

    [SerializeField]
    private float damage = 0;
    [SerializeField]
    private float initialHitSpeed = 20;

    private float hitSpeed = 0;
    [SerializeField]
    private float hitSpeedMultiplyer = 0.1f;
    [SerializeField]
    private float hitSpeedDecreaseRate = 0;
    private Vector2 hitDirection;

    [SerializeField]
    private float shoulderRotation = 5;

    [SerializeField]
    private float fistActiveScale = 2;
    [SerializeField]
    private float fistInactiveScale = 1;

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
        if(Input.GetKeyDown(playerKey))
        {
            fist.transform.localScale = new Vector3(fistActiveScale, fistActiveScale, fistActiveScale);
            fistCollider.enabled = true;
            active = true;
        }

        if (Input.GetKeyUp(playerKey))
        {
            fist.transform.localScale = new Vector3(fistInactiveScale, fistInactiveScale, fistInactiveScale);
            fistCollider.enabled = false;
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

        if(hitSpeed != 0)
        {
            transform.Translate(hitDirection * hitSpeed * Time.deltaTime);
            hitSpeed -= hitSpeedDecreaseRate * Time.deltaTime;
            if (hitSpeed <= 0)
            {
                hitSpeed = 0;
            }
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

    private void ReflectAngle(Vector2 tangent)
    {
        float angleOfIncidence = Vector2.SignedAngle(fist.transform.position - transform.position, tangent);
        Debug.DrawLine(transform.position, fist.transform.position - transform.position, Color.green, 5);
        shoulder.transform.Rotate(new Vector3(0, 0, angleOfIncidence * 2));
        Debug.DrawLine(transform.position, fist.transform.position - transform.position, Color.cyan, 5);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D thisCollider = collision.otherCollider;
        Collider2D hitCollider = collision.collider;

        //Player Hit
        if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Fist") &&
            thisCollider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log(this.name + " hit by " + hitCollider.name);
            float angle = Vector2.SignedAngle(fist.transform.position - transform.position, collision.GetContact(0).normal);
            shoulder.transform.Rotate(new Vector3(0, 0, angle));
            damage += 20;
            hitDirection = collision.GetContact(0).normal;
            hitSpeed = initialHitSpeed + damage * hitSpeedMultiplyer;

        }
        //Hit Other
        else if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Player") &&
            thisCollider.gameObject.layer == LayerMask.NameToLayer("Fist"))
        {

        }
        else if (hitCollider.gameObject.layer == LayerMask.NameToLayer("OutOfBounds") &&
            thisCollider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            damage = 0;
            hitSpeed = 0;
            transform.position = new Vector3(0, 0, 0);            
        }
        else
        {
            ContactPoint2D contact = collision.GetContact(0);

            Debug.DrawLine(contact.point, contact.point + (new Vector2(-contact.normal.y, contact.normal.x) * 2), Color.blue, 5);

            ReflectAngle(new Vector2(-contact.normal.y, contact.normal.x));
            hitDirection = Vector2.Reflect(hitDirection, contact.normal);
        }
    }
}
