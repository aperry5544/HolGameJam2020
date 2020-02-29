using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Text splashText = null;

    [SerializeField]
    private Image splashImage = null;

    [SerializeField]
    private Text titleText = null;

    [SerializeField]
    private Image titleImage = null;

    [SerializeField]
    private Text gameStartText = null;

    [SerializeField]
    private float welcomeDuration = 2.0f;

    [SerializeField]
    private float fadeDuration = 2.0f;

    [SerializeField]
    private float doubleClickTimeout = 0.5f;

    [SerializeField]
    private float gameStartTime = 5.0f;

    private float currentGameStartTime = 0.0f;
    private float currentWelcome = 2.0f;
    private float currentFade = 2.0f;

    private Dictionary<KeyCode, PlayerController> playerList = null;
    private Dictionary<KeyCode, float> timeOfKeyUp = null;

    private List<KeyCode> playerKeysCurrentlyDown = null;

    private static GameManager instance = null;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }

            return instance;
        }
    }

    public enum GameState
    {
        Welcome,
        Fade,
        Title,
        Gameplay,
    }

    public GameState CurrentGameState { get; private set; }

    [SerializeField]
    private GameObject playerPrefab = null;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
        playerList = new Dictionary<KeyCode, PlayerController>();
        timeOfKeyUp = new Dictionary<KeyCode, float>();
        playerKeysCurrentlyDown = new List<KeyCode>();
        UpdateState(GameState.Welcome);
    }

    public void AddPlayer(KeyCode key)
    {
        GameObject newPlayer = Instantiate(playerPrefab);
        playerList.Add(key, newPlayer.GetComponent<PlayerController>());
        Debug.Log(string.Format("Player {0} joined!", key));
    }

    public void RemovePlayer(KeyCode key)
    {
        if (playerList.ContainsKey(key))
        {
            Destroy(playerList[key].gameObject);
        }
        playerList.Remove(key);
        Debug.Log(string.Format("Player {0} left!", key));

    }

    private void CheckForPlayerInput()
    {
        // Player Input
        foreach (KeyValuePair<KeyCode, PlayerController> player in playerList)
        {
            if (Input.GetKeyDown(player.Key))
            {
                player.Value.Activate();
            }

            if (Input.GetKeyUp(player.Key))
            {
                player.Value.Deactivate();
            }
        }
    }

    private void UpdateState(GameState newState)
    {
        switch (newState)
        {
            case GameState.Welcome:
                currentWelcome = welcomeDuration;
                splashImage.enabled = true;
                splashText.enabled = true;
                titleText.enabled = false;
                titleImage.enabled = false;
                gameStartText.enabled = false;
                break;
            case GameState.Fade:
                currentFade = fadeDuration;
                splashImage.enabled = true;
                splashText.enabled = true;
                titleText.enabled = false;
                titleImage.enabled = false;
                gameStartText.enabled = false;
                break;
            case GameState.Title:
                currentGameStartTime = gameStartTime;
                splashText.enabled = false;
                splashImage.enabled = false;
                titleText.enabled = true;
                titleImage.enabled = true;
                gameStartText.enabled = false;
                break;
            case GameState.Gameplay:
                splashImage.enabled = false;
                splashText.enabled = false;
                titleText.enabled = false;
                titleImage.enabled = false;
                gameStartText.enabled = false;
                break;
            default:
                break;
        }

        CurrentGameState = newState;
    }

    private void WelcomeUpdate()
    {
        if (currentWelcome > 0)
        {
            currentWelcome -= Time.deltaTime;
        }
        else
        {
            UpdateState(GameState.Fade);
        }
    }

    private void FadeUpdate()
    {
        if (currentFade > 0)
        {
            currentFade -= Time.deltaTime;

            float percentageComplete = (1 - Mathf.Clamp01(currentFade / (fadeDuration))) * 1.25f;

            splashText.color = Color.Lerp(Color.black, Color.white, percentageComplete);
        }
        else
        {
            UpdateState(GameState.Title);
        }
    }

    private void OnGUI()
    {
        if (CurrentGameState == GameState.Title)
        { 
        Event e = Event.current;

            // Handle players joining or leaving
            if (e != null && e.isKey && e.keyCode != KeyCode.None)
            {
                if (e.type == EventType.KeyDown)
                {
                    // If we are not an existing player, add myself to the roster
                    if (!playerList.ContainsKey(e.keyCode))
                    {
                        AddPlayer(e.keyCode);
                        timeOfKeyUp[e.keyCode] = Time.realtimeSinceStartup - 1000;
                    }
                    else if (Time.realtimeSinceStartup - timeOfKeyUp[e.keyCode] <= doubleClickTimeout)
                    {
                        RemovePlayer(e.keyCode);
                        playerKeysCurrentlyDown.Remove(e.keyCode);
                    }
                    else if (!playerKeysCurrentlyDown.Contains(e.keyCode))
                    {
                        playerKeysCurrentlyDown.Add(e.keyCode);
                    }
                }
                else if (e.type == EventType.KeyUp)
                {
                    if (playerList.ContainsKey(e.keyCode))
                    {
                        playerKeysCurrentlyDown.Remove(e.keyCode);
                        timeOfKeyUp[e.keyCode] = Time.realtimeSinceStartup;
                    }
                }
            }
        }
    }

    private void TitleUpdate()
    {
        if (playerKeysCurrentlyDown.Count == playerList.Count && playerList.Count > 1)
        {
            gameStartText.enabled = true;

            currentGameStartTime -= Time.deltaTime;
            if (currentGameStartTime <= 0)
            {
                UpdateState(GameState.Gameplay);
            }

            gameStartText.text = (((int)currentGameStartTime) + 1).ToString();
            float remainder = currentGameStartTime - ((int)currentGameStartTime);
            gameStartText.fontSize = (int)Mathf.Lerp(100, 300, remainder);
        }
        else
        {
            currentGameStartTime = gameStartTime;
            gameStartText.enabled = false;
        }
    }

    private void GameplayUpdate()
    {
        CheckForPlayerInput();
    }

    public void Update()
    {
        switch (CurrentGameState)
        {
            case GameState.Welcome:
                WelcomeUpdate();
                break;
            case GameState.Fade:
                FadeUpdate();
                break;
            case GameState.Title:
                TitleUpdate();
                break;
            case GameState.Gameplay:
                GameplayUpdate();
                break;
            default:
                break;
        }
    }
}
