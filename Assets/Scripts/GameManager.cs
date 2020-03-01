using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer splashImage = null;

    [SerializeField]
    private SpriteRenderer titleImage = null;

    [SerializeField]
    private SpriteRenderer winnerImage = null;

    [SerializeField]
    private GameObject startCounterCoverup = null;

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

    [SerializeField]
    private GameObject[] levels = null;

    [SerializeField]
    private float levelMoveDistance = 20;

    [SerializeField]
    private float levelMoveSpeed = 10;

    [SerializeField]
    [Range(1.1f, 10)]
    private float playerRaiseScale = 1.5f;

    [SerializeField]
    private float playerRaiseScaleSpeed = 10.0f;

    [SerializeField]
    private PlayerController.PlayerSprite[] playerArt = null;

    private int currentLevel = -1;
    private int previousLevel = -1;
    private float currentGameStartTime = 0.0f;
    private float currentWelcome = 2.0f;
    private float currentFade = 2.0f;
    private float currentPlayerScale = 1.0f;


    private Dictionary<KeyCode, Vector2> currentMapSpawnPoses = null;
    private Dictionary<KeyCode, Vector2> currentPlayerDeathPoses = null;
    private Dictionary<KeyCode, PlayerController> playerList = null;
    private List<KeyCode> alivePlayers = null;
    private Dictionary<KeyCode, int> score = null;
    [SerializeField]
    private int targetScore = 3;
    private bool gameEnd = false;
    private KeyCode winningPlayer;
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
        LoadLevelPhase1,
        LoadLevelPhase2,
        LoadLevelPhase3,
        Gameplay,
        Win,
        EndGamePhase1,
        EndGamePhase2,
    }

    public GameState CurrentGameState { get; private set; }

    [SerializeField]
    private GameObject playerPrefab = null;

    public void Start()
    {
        playerList = new Dictionary<KeyCode, PlayerController>();
        timeOfKeyUp = new Dictionary<KeyCode, float>();
        playerKeysCurrentlyDown = new List<KeyCode>();
        alivePlayers = new List<KeyCode>();
        score = new Dictionary<KeyCode, int>();
        currentMapSpawnPoses = new Dictionary<KeyCode, Vector2>();
        currentPlayerDeathPoses = new Dictionary<KeyCode, Vector2>();
        UpdateState(GameState.Welcome);
    }

    public void AddPlayer(KeyCode key)
    {
        GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        PlayerController pc = newPlayer.GetComponent<PlayerController>();
        playerList.Add(key, pc);
        score[key] = 0;
        if(playerList.Count == 1)
        {
            pc.SetKeyCode(key, playerArt[0]);
        }
        else if (playerList.Count == 2)
        {
            pc.SetKeyCode(key, playerArt[1]);
        }
        else if (playerList.Count == 3)
        {
            pc.SetKeyCode(key, playerArt[2]);
        }
        else
        {
            pc.SetKeyCode(key, playerArt[UnityEngine.Random.Range(0, playerArt.Length)]);
        }

        // Get the spawn pose of the title screen
        List<Vector2> spawnPoses = titleImage.gameObject.GetComponent<Level>().GetSpawnPositions(playerList.Count);

        int i = 0;
        // Update the position of all players
        foreach (KeyValuePair<KeyCode, PlayerController> player in playerList)
        {
            player.Value.transform.position = spawnPoses[i++];
        }

        currentPlayerDeathPoses[key] = Vector3.zero;

        Debug.Log(string.Format("Player {0} joined!", key));
    }

    public void RemovePlayer(KeyCode key)
    {
        if (playerList.ContainsKey(key))
        {
            Destroy(playerList[key].gameObject);
        }
        playerList.Remove(key);

        List<Vector2> spawnPoses = titleImage.gameObject.GetComponent<Level>().GetSpawnPositions(playerList.Count);

        int i = 0;
        // Update the position of all players
        foreach (KeyValuePair<KeyCode, PlayerController> player in playerList)
        {
            player.Value.transform.position = spawnPoses[i++];
        }

        Debug.Log(string.Format("Player {0} left!", key));

    }

    private void CheckForPlayerInput()
    {
        // Player Input
        foreach (KeyValuePair<KeyCode, PlayerController> player in playerList)
        {
            if (Input.GetKeyDown(player.Key))
            {
                player.Value.ActivateFist();
            }

            if (Input.GetKeyUp(player.Key))
            {
                player.Value.DeactivateFist();
            }
        }
    }

    private void UpdateState(GameState newState)
    {
        CurrentGameState = newState;

        switch (newState)
        {
            case GameState.Welcome:
                currentWelcome = welcomeDuration;
                splashImage.enabled = true;
                titleImage.enabled = false;
                winnerImage.enabled = false;
                gameStartText.enabled = false;
                startCounterCoverup.SetActive(false);
                break;
            case GameState.Fade:
                currentFade = fadeDuration;
                splashImage.enabled = true;
                titleImage.enabled = false;
                winnerImage.enabled = false;
                gameStartText.enabled = false;
                startCounterCoverup.SetActive(false);
                break;
            case GameState.Title:
                currentGameStartTime = gameStartTime;
                splashImage.enabled = false;
                titleImage.enabled = true;
                winnerImage.enabled = false;
                gameStartText.enabled = false;
                startCounterCoverup.SetActive(false);
                break;
            case GameState.LoadLevelPhase1:
                splashImage.enabled = false;
                titleImage.enabled = false;
                winnerImage.enabled = false;
                gameStartText.enabled = false;
                startCounterCoverup.SetActive(false);
                currentPlayerScale = 1;
                BeginLoadLevel();
                break;
            case GameState.LoadLevelPhase2:
                break;
            case GameState.LoadLevelPhase3:
                break;
            case GameState.Gameplay:
                FinishLoadLevel();
                HazardManager.Instance.StartRound();
                break;
            case GameState.Win:
                HazardManager.Instance.EndRound();
                PlayerWon();
                if(gameEnd)
                {
                    UpdateState(GameState.EndGamePhase1);
                }
                else
                {
                    UpdateState(GameState.LoadLevelPhase1);
                }
                break;
            case GameState.EndGamePhase1:
                splashImage.enabled = false;
                titleImage.enabled = false;
                winnerImage.enabled = true;
                gameStartText.enabled = false;
                startCounterCoverup.SetActive(false);
                StopGame();
                break;
            case GameState.EndGamePhase2:
                
                break;
            default:
                break;
        }
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
        }
        else
        {
            UpdateState(GameState.Title);
        }
    }

    private void OnGUI()
    {
        Event e = Event.current;
        
        // Handle players joining or leaving
        if (e != null && e.isKey && e.keyCode != KeyCode.None)
        {
            if (e.keyCode == KeyCode.Escape)
            {
                UnityEditor.EditorApplication.isPlaying = false;
                Application.Quit();
                return;
            }

            if (e.keyCode == KeyCode.ScrollLock ||
                e.keyCode == KeyCode.Pause ||
                e.keyCode == KeyCode.Insert ||
                e.keyCode == KeyCode.Delete ||
                e.keyCode == KeyCode.PageDown ||
                e.keyCode == KeyCode.PageUp ||
                e.keyCode == KeyCode.Home ||
                e.keyCode == KeyCode.End ||
                e.keyCode == KeyCode.Tab ||
                e.keyCode == KeyCode.CapsLock ||
                e.keyCode == KeyCode.LeftShift ||
                e.keyCode == KeyCode.RightShift ||
                e.keyCode == KeyCode.LeftControl ||
                e.keyCode == KeyCode.RightControl ||
                e.keyCode == KeyCode.AltGr ||
                e.keyCode == KeyCode.LeftAlt ||
                e.keyCode == KeyCode.RightAlt ||
                e.keyCode == KeyCode.KeypadEnter ||
                e.keyCode == KeyCode.Backspace ||
                e.keyCode == KeyCode.LeftArrow ||
                e.keyCode == KeyCode.RightArrow ||
                e.keyCode == KeyCode.UpArrow ||
                e.keyCode == KeyCode.DownArrow ||
                e.keyCode == KeyCode.Numlock ||
                e.keyCode == KeyCode.KeypadEnter ||
                e.keyCode == KeyCode.Return)
            {
                return;
            }

            if (CurrentGameState == GameState.Title)
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
            startCounterCoverup.SetActive(true);
            gameStartText.enabled = true;

            currentGameStartTime -= Time.deltaTime;
            if (currentGameStartTime <= 0)
            {
                UpdateState(GameState.LoadLevelPhase1);
            }

            gameStartText.text = (((int)currentGameStartTime) + 1).ToString();
            float remainder = currentGameStartTime - ((int)currentGameStartTime);
            gameStartText.fontSize = (int)Mathf.Lerp(100, 300, remainder);
        }
        else
        {
            currentGameStartTime = gameStartTime;
            gameStartText.enabled = false;
            startCounterCoverup.SetActive(false);
        }
    }

    private void BeginLoadLevel(int levelIndex = -1)
    {
        if (levelIndex == -1)
        {
            if (levels.Length < 3)
            {
                Debug.LogError("NEED AT LEAST 3 LEVELS. SELF DESTRUCTING NOW.");
                UnityEditor.EditorApplication.isPlaying = false;
            }
            while (levelIndex == previousLevel || levelIndex == currentLevel || levelIndex == -1)
            {
                levelIndex = UnityEngine.Random.Range(0, levels.Length);
            }
        }

        // Fetch new spawn poses for the players
        List<Vector2> spawnPoses = levels[levelIndex].GetComponent<Level>().GetSpawnPositions(playerList.Count);

        alivePlayers.Clear();

        int poseIndex = 0;
        // Freeze Players
        foreach (KeyValuePair<KeyCode, PlayerController> player in playerList)
        {
            currentMapSpawnPoses[player.Key] = spawnPoses[poseIndex++];
            currentPlayerDeathPoses[player.Key] = new Vector2(playerList[player.Key].transform.position.x, playerList[player.Key].transform.position.y);
            alivePlayers.Add(player.Key);

            player.Value.Frozen = true;
            player.Value.SetVisibility(true);
            player.Value.ShowWins(score[player.Key]);
        }

        // Disable current level walls
        if (currentLevel > -1)
        {
            levels[currentLevel].GetComponent<Level>().DisableLevelPhysics(); 
        }


        // Diable physics on next level
        if (levelIndex > -1)
        {
            // Enable the next level
            levels[levelIndex].gameObject.SetActive(true);
            levels[levelIndex].GetComponent<Level>().DisableLevelPhysics();
        }

        // Swap my current levels
        previousLevel = currentLevel;
        currentLevel = levelIndex;
    }

    private void FinishLoadLevel()
    {
        // Enable current level walls
        if (currentLevel > -1)
        {
            levels[currentLevel].GetComponent<Level>().EnableLevelPhysics();
        }

        if (previousLevel > -1)
        {
            // Move and Disable the old level
            levels[previousLevel].transform.position = new Vector3(levelMoveDistance, 0, 0);
            levels[previousLevel].gameObject.SetActive(false);
        }

        // UnFreeze Players
        foreach (KeyValuePair<KeyCode, PlayerController> player in playerList)
        {
            player.Value.HideWins();
            player.Value.Reset();
            player.Value.Frozen = false;
            player.Value.DeactivateFist();
        }
    }

    private void LoadLevelUpdate(int phase)
    {
        // Raise players
        if (phase == 1)
        {
            currentPlayerScale += playerRaiseScaleSpeed * Time.deltaTime;

            if (currentPlayerScale >= playerRaiseScale)
            {
                currentPlayerScale = playerRaiseScale;
            }

            float t = (currentPlayerScale - 1) / (playerRaiseScale - 1);

            foreach (KeyValuePair<KeyCode, PlayerController> player in playerList)
            {
                player.Value.transform.localScale = new Vector3(currentPlayerScale, currentPlayerScale, 1);
                player.Value.transform.position = Vector3.Lerp(currentPlayerDeathPoses[player.Key], currentMapSpawnPoses[player.Key], t);
            }

            if (currentPlayerScale >= playerRaiseScale)
            {
                currentPlayerScale = playerRaiseScale;
                UpdateState(GameState.LoadLevelPhase2);
            }
        }
        // Move levels
        else if (phase == 2)
        {
            // Update previous level
            if (previousLevel != -1)
            {
                float newX = levels[previousLevel].transform.position.x - levelMoveSpeed * Time.deltaTime;
                if (newX <= -levelMoveDistance)
                {
                    levels[previousLevel].transform.position = new Vector3(-levelMoveDistance, 0, 0);
                    if (currentLevel > -1)
                    {
                        levels[currentLevel].transform.position = new Vector3(0, 0, 0);
                    }
                    UpdateState(GameState.LoadLevelPhase3);
                    return;
                }
                levels[previousLevel].transform.position = new Vector3(newX, 0, 0);
            }

            // Update new level
            if (currentLevel != -1)
            {
                float newX = levels[currentLevel].transform.position.x - levelMoveSpeed * Time.deltaTime;
                if (newX <= 0)
                {
                    levels[currentLevel].transform.position = new Vector3(0, 0, 0);
                    if (previousLevel > -1)
                    {
                        levels[previousLevel].transform.position = new Vector3(-levelMoveDistance, 0, 0);
                    }
                    UpdateState(GameState.LoadLevelPhase3);
                    return;
                }
                levels[currentLevel].transform.position = new Vector3(newX, 0, 0);
            }
        }
        // Lower players
        else if (phase == 3)
        {
            currentPlayerScale -= playerRaiseScaleSpeed * Time.deltaTime;

            if (currentPlayerScale <= 1)
            {
                currentPlayerScale = 1;
            }

            foreach (KeyValuePair<KeyCode, PlayerController> player in playerList)
            {
                player.Value.transform.localScale = new Vector3(currentPlayerScale, currentPlayerScale, 1);
            }

            if (currentPlayerScale <= 1)
            {
                currentPlayerScale = 1;
                UpdateState(GameState.Gameplay);
            }
        }
    }

    private void GameplayUpdate()
    {
        CheckForPlayerInput();
        HazardManager.Instance.Active();
    }

    public void PlayerDied(KeyCode key)
    {
        if (alivePlayers.Contains(key))
        {
            alivePlayers.Remove(key);
        }

        if (alivePlayers.Count == 1)
        {
            UpdateState(GameState.Win);
        }
    }

    public void PlayerWon()
    {
        score[alivePlayers[0]] = score[alivePlayers[0]] + 1;
        if (score[alivePlayers[0]] >= targetScore)
        {
            gameEnd = true;
            winningPlayer = alivePlayers[0];
        }
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
            case GameState.LoadLevelPhase1:
                LoadLevelUpdate(1);
                break;
            case GameState.LoadLevelPhase2:
                LoadLevelUpdate(2);
                break;
            case GameState.LoadLevelPhase3:
                LoadLevelUpdate(3);
                break;
            case GameState.Gameplay:
                GameplayUpdate();
                break;
            case GameState.Win:
                break;
            case GameState.EndGamePhase1:
                RaiseWinner();
                break;
            case GameState.EndGamePhase2:
                EndGameUpdate();
                break;
            default:
                break;
        }
    }

    private void RaiseWinner()
    {
        // Raise players
        currentPlayerScale += playerRaiseScaleSpeed * Time.deltaTime;

        if (currentPlayerScale >= playerRaiseScale)
        {
            currentPlayerScale = playerRaiseScale;
        }

        float t = (currentPlayerScale - 1) / (playerRaiseScale - 1);

        playerList[winningPlayer].transform.localScale = new Vector3(currentPlayerScale, currentPlayerScale, 1);
        playerList[winningPlayer].transform.position = Vector3.Lerp(currentPlayerDeathPoses[winningPlayer], Vector2.zero, t);

        if (currentPlayerScale >= playerRaiseScale)
        {
            currentPlayerScale = playerRaiseScale;
            UpdateState(GameState.EndGamePhase2);
        }
    }

    private void EndGameUpdate()
    {
        if(Input.anyKeyDown)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void StopGame()
    {
        foreach (KeyValuePair<KeyCode, PlayerController> player in playerList)
        {
            player.Value.Frozen = true;
        }

        // Disable current level walls
        if (currentLevel > -1)
        {
            levels[currentLevel].GetComponent<Level>().DisableLevelPhysics();
        }
    }
}
