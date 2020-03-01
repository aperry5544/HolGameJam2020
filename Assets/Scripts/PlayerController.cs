using System;
using System.Runtime.InteropServices;
using System.Text;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Serializable]
    public struct PlayerSprite
    {
        public Sprite headNormal;
        public Sprite headHurt;
        public Sprite headLeft;
        public Sprite headRight;
        public Sprite arm;
        public Sprite body;
        public Sprite fist;
    }

    //Controls
    [SerializeField]
    private KeyCode playerKey = KeyCode.A;

    //Renderers
    [SerializeField]
    private SpriteRenderer headRenderer = null;
    [SerializeField]
    private SpriteRenderer bodyRenderer = null;
    [SerializeField]
    private SpriteRenderer fistRenderer = null;
    [SerializeField]
    private SpriteRenderer armRenderer = null;

    //Parts
    [SerializeField]
    private GameObject shoulder = null;
    [SerializeField]
    private GameObject fist = null;
    [SerializeField]
    private Collider2D fistCollider = null;

    //Fist Properties
    private bool active = false;
    [SerializeField]
    private Vector3 fistDirection;
    [SerializeField]
    private float fistVelocity = 5;
    [SerializeField]
    private float shoulderRotation = 5;
    [SerializeField]
    private float fistActiveScale = 2;
    [SerializeField]
    private float fistInactiveScale = 1;

    //Health Properties
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

    //Freeze Property
    private bool frozen = false;
    public bool Frozen
    {
        get { return frozen; }
        set { frozen = value; }
    }

    [SerializeField]
    private TextMeshProUGUI winText = null;

    [SerializeField]
    private TextMeshProUGUI keyCodeText = null;

    [SerializeField]
    private AudioClip[] punchAudioClips = null;

    [SerializeField]
    private AudioClip[] hurtAudioClips = null;

    private AudioSource audioSource = null;
    private int latestHurtIndex = -1;
    private int latestPunchIndex = -1;
    private PlayerSprite playerSprite;
    private float timeLeftOnHurtOverride = 0.0f;

    private string KeyCodeToString(KeyCode keycode)
    {
        // Normal letters
        if (keycode >= KeyCode.A && keycode <= KeyCode.Z)
        {
            return keycode.ToString();
        }

        // Alpha Numbers
        if (keycode >= KeyCode.Alpha0 && keycode <= KeyCode.Alpha9)
        {
            string number = string.Empty;
            number += (char)keycode;

            return number;
        }

        // Keypad Numbers
        if (keycode >= KeyCode.Keypad0 && keycode <= KeyCode.Keypad9)
        {
            string number = string.Empty;
            int value = (int)keycode - (KeyCode.Keypad0 - KeyCode.Alpha0);
            number += 'N';
            number += (char)value;


            return number;
        }

        if (keycode == KeyCode.Space)
        {
            return "_";
        }

        if (keycode == KeyCode.BackQuote)
        {
            return "`";
        }

        if (keycode == KeyCode.Minus)
        {
            return "-";
        }

        if (keycode == KeyCode.Equals)
        {
            return "=";
        }

        if (keycode == KeyCode.KeypadDivide)
        {
            return "N/";
        }

        if (keycode == KeyCode.KeypadMultiply)
        {
            return "N*";
        }

        if (keycode == KeyCode.KeypadMinus)
        {
            return "N-";
        }

        if (keycode == KeyCode.KeypadPlus)
        {
            return "N+";
        }

        if (keycode == KeyCode.KeypadPeriod)
        {
            return "N.";
        }

        if (keycode == KeyCode.Period)
        {
            return ".";
        }

        if (keycode == KeyCode.Comma)
        {
            return ",";
        }

        if (keycode == KeyCode.Slash)
        {
            return "/";
        }

        if (keycode == KeyCode.Semicolon)
        {
            return ";";
        }

        if (keycode == KeyCode.Quote)
        {
            return "'";
        }

        if (keycode == KeyCode.LeftBracket)
        {
            return "[";
        }

        if (keycode == KeyCode.RightBracket)
        {
            return "]";
        }

        if (keycode == KeyCode.Backslash)
        {
            return "\\";
        }

        Debug.Log(string.Format("Forgot to implement an override for {0}", keycode.ToString()));
        return keycode.ToString();
    }

    private int RandomIntExcept(int min, int max, int except)
    {
        int uncheckedRandom = UnityEngine.Random.Range(min, max - 1);
        if (uncheckedRandom >= except)
        {
            uncheckedRandom += 1;
        }

        return uncheckedRandom;
    }

    private void PlayPunchSound()
    {
        if (!audioSource.isPlaying)
        {
            latestPunchIndex = RandomIntExcept(0, punchAudioClips.Length, latestPunchIndex);
            audioSource.clip = punchAudioClips[latestPunchIndex];
            audioSource.Play();
        }
    }

    private void PlayHurtSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        latestHurtIndex = RandomIntExcept(0, hurtAudioClips.Length, latestHurtIndex);
        audioSource.clip = hurtAudioClips[latestHurtIndex];
        audioSource.Play();
    }

    public void SetKeyCode(KeyCode keyCode, PlayerSprite sprite)
    {
        playerSprite = sprite;
        headRenderer.sprite = sprite.headNormal;
        armRenderer.sprite = sprite.arm;
        bodyRenderer.sprite = sprite.body;
        fistRenderer.sprite = sprite.fist;
        playerKey = keyCode;
        keyCodeText.text = KeyCodeToString(keyCode);
    }

    public void ActivateFist()
    {
        active = true;
        fist.transform.localScale = new Vector3(fistActiveScale, fistActiveScale, fistActiveScale);
        fistCollider.enabled = true;
    }

    public void DeactivateFist()
    {
        active = false;
        fist.transform.localScale = new Vector3(fistInactiveScale, fistInactiveScale, fistInactiveScale);
        fistCollider.enabled = false;
    }

    public void Reset()
    {
        damage = 0;
        hitSpeed = 0;
    }

    public void ShowWins(int numberOfWins)
    {
        winText.text = numberOfWins.ToString();
        winText.enabled = true;
    }

    public void HideWins()
    {
        winText.enabled = false;
    }

    private void OnEnable()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(frozen)
        {
            return;
        }

        if(active)
        {
            if (FireFist() == FistDirection.Left)
            {
                headRenderer.sprite = playerSprite.headLeft;
            }
            else
            {
                headRenderer.sprite = playerSprite.headRight;
            }
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

        if (timeLeftOnHurtOverride > 0)
        {
            headRenderer.sprite = playerSprite.headHurt;
            timeLeftOnHurtOverride -= Time.deltaTime;
        }
    }

    private enum FistDirection
    {
        Left,
        Right,
    }

    private FistDirection FireFist()
    {
        fistDirection = fist.transform.position - transform.position;
        gameObject.transform.Translate(fistDirection.normalized * fistVelocity * Time.deltaTime);

        if (fistDirection.x <= 0)
            return FistDirection.Left;
        else
            return FistDirection.Right;
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
            HitDecalManager.Instance.BodyHit(collision.GetContact(0).point);
            PlayHurtSound();
        }
        //Hit Other
        else if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Fist") &&
            thisCollider.gameObject.layer == LayerMask.NameToLayer("Fist"))
        {
            ContactPoint2D contact = collision.GetContact(0);

            ReflectAngle(new Vector2(-contact.normal.y, contact.normal.x));
            hitDirection = Vector2.Reflect(hitDirection, contact.normal);
            HitDecalManager.Instance.FistHit(collision.GetContact(0).point);
            PlayPunchSound();
        }
        //OutOfBounds
        else if (hitCollider.gameObject.layer == LayerMask.NameToLayer("OutOfBounds") &&
            thisCollider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayHurtSound();

            Reset();
            gameObject.SetActive(false);
            GameManager.Instance.PlayerDied(playerKey);
        }
        else
        {
            ContactPoint2D contact = collision.GetContact(0);

            Debug.DrawLine(contact.point, contact.point + (new Vector2(-contact.normal.y, contact.normal.x) * 2), Color.blue, 5);

            ReflectAngle(new Vector2(-contact.normal.y, contact.normal.x));
            hitDirection = Vector2.Reflect(hitDirection, contact.normal);

            PlayPunchSound();
        }
    }
}
