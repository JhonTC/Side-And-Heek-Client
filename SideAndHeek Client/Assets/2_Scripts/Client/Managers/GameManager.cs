using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PickupCollection collection;
    public GameType gameType = GameType.HideAndSeek;
    public GameMode gameMode;

    public PickupSpawner pickupSpawnerPrefab;

    public bool gameStarted = false;

    protected int currentTime = 0;

    public string activeSceneName = "Lobby";
    public string lobbyScene;

    public GameObject gameStartCollider;

    public Color[] hiderColours;
    public Color hunterColour;

    private AudioSource music;

    public bool tryStartGameActive = false;

    public Color unreadyColour;
    public Color unreadyTextColour;
    public Color readyColour;

    public GameObject sceneCamera;

    public Transform billboardTarget;

    [System.Serializable]
    public class SelectableColour
    {
        public Color colour;
        public bool isTaken;

        public SelectableColour(Color colour, bool isTaken)
        {
            this.colour = colour;
            this.isTaken = isTaken;
        }
    }

    public SelectableColour[] chosenColours;

    [HideInInspector] public List<Player> lastMainHunterPlayers = new List<Player>(); //todo:Move somewhere?? - cant go to gamemode as it gets cleared when updated

    [SerializeField]
    private Behaviour[] serverComponentsToDisable;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        music = GetComponent<AudioSource>();
        FadeMusic(false);

        gameMode = GameMode.CreateGameModeFromType(gameType);
        gameMode.SetGameRules(GameRules.CreateGameRulesFromType(gameType)); //todo: replace with default mode?
    }

    

    private void FixedUpdate()
    {
        if (NetworkManager.NetworkType != NetworkType.Client)
        {
            if (gameStarted)
            {
                gameMode.FixedUpdate();
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (PlayerPrefs.HasKey("LastRoomCode"))
        {   
            PlayerPrefs.DeleteKey("LastRoomCode");
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        SceneManager.sceneUnloaded += OnLevelFinishedUnloading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        SceneManager.sceneUnloaded -= OnLevelFinishedUnloading;
    }

    public void OnNetworkTypeSetup()
    {
        //todo: these functions cant be called before NetworkType is setup - equally calling these multiple times will cause duplicates to be made...
        //something else to have self managed by a COLOUR MANAGER

        if (NetworkManager.NetworkType != NetworkType.Client)
        {
            chosenColours = new SelectableColour[hiderColours.Length]; 
            for (int i = 0; i < hiderColours.Length; i++)
            {
                chosenColours[i] = new SelectableColour(hiderColours[i], false);
            }
        } else
        {
            foreach (Behaviour behaviour in serverComponentsToDisable)
            {
                behaviour.enabled = false;
            }
        }

        if (NetworkManager.NetworkType != NetworkType.ServerOnly) 
        {
            UIManager.instance.customisationPanel.Init(hiderColours);
        }

        WindVolume.Instance.Init();
    }

    public void GameStart(int gameDuration) //Todo:Make a roundManager separate to gameManager.....||.....should not receive a duration, gamemode should handle incoming messgae
    {
        Debug.Log("Game Start!");

        UIManager.instance.CloseHistoryPanels();
        StartGameTimer(gameDuration); //todo:needs moving inside gameMode-util class for gamemodes without a duration
    }

    public void GameOver() //Todo:Make a roundManager separate to gameManager
    {
        gameStarted = false;
        tryStartGameActive = false;

        foreach (Player player in Player.list.Values)
        {
            if (NetworkManager.NetworkType != NetworkType.Client)
            {
                player.TeleportPlayer(LevelManager.GetLevelManagerForScene("Lobby").GetNextSpawnpoint(!gameStarted && player.isHost));
                player.SetPlayerReady(false, false);
                player.SetPlayerType(PlayerType.Default, false, false);
            }

            if (NetworkManager.NetworkType != NetworkType.ServerOnly)
            {
                player.OnGameOver();
            }
        }

        if (NetworkManager.NetworkType != NetworkType.ServerOnly)
        {
            PickupSpawner.DestroyPickupSpawners();
            NetworkObjectsManager.instance.ClearAllSpawnedNetworkObjects();
        }

        if (NetworkManager.NetworkType != NetworkType.Client)
        {
            gameMode.GameOver();
            ServerSend.GameOver();

            NetworkObjectsManager.instance.ClearAllSpawnedNetworkObjects();

            SceneManager.UnloadSceneAsync(gameMode.sceneName);
            ServerSend.UnloadScene(gameMode.sceneName);
        }

        Debug.Log("Game Over!");

        //disable input
        //show gameover panel
        //enable option to go back to the lobby 

        /*  - what happens to the other players?
            - stop receiveing input and teleport them all to the starting spawnpoints
                - what about one player moving the others?
                - lock the players in place?
        */
    }

    protected virtual void OnLevelFinishedLoading(Scene _scene, LoadSceneMode _loadSceneMode)
    {
        activeSceneName = _scene.name;
        SceneManager.SetActiveScene(_scene);

        if (NetworkManager.NetworkType != NetworkType.Client)
        {
            if (LevelManager.GetLevelManagerForScene(activeSceneName).levelType == LevelType.Map)
            {
                gameStarted = true;
                gameMode.GameStart();
            }
        }

        if (NetworkManager.NetworkType != NetworkType.ServerOnly)
        {
            if (_scene.name != lobbyScene)
            {
                gameStarted = true;
                gameStartCollider.SetActive(false);

                foreach (Player player in Player.list.Values)
                {
                    player.OnGameStart();
                }

                UIManager.instance.CloseHistoryPanels(); //move call to gamestart
            }
            else
            {
                gameStartCollider.SetActive(true);
            }
        }
    }

    bool destoryItemSpawners;
    protected virtual void OnLevelFinishedUnloading(Scene _scene)
    {
        activeSceneName = lobbyScene;

        foreach (Player player in Player.list.Values)
        {
            player.activePickup = null;
        }

        if (NetworkManager.NetworkType != NetworkType.Client)
        {
            PickupHandler.ResetPickupLog();
        }

        if (NetworkManager.NetworkType != NetworkType.ServerOnly)
        {
            gameStartCollider.SetActive(true);
            UIManager.instance.gameplayPanel.SetItemDetails();
        }
    }

    public void LoadScene(string sceneName, LoadSceneMode loadSceneMode)
    {
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            SceneManager.LoadScene(sceneName, loadSceneMode);
        }
    }

    public void UnloadScene(string sceneName, bool _destoryItemSpawners)
    {
        destoryItemSpawners = _destoryItemSpawners;
        SceneManager.UnloadSceneAsync(sceneName);
    }

    public void StartGameTimer(int _duration) 
    {
        StartCoroutine(GameTimeCountdown(_duration));
    }
    private IEnumerator GameTimeCountdown(int _duration = 240)
    {
        currentTime = _duration;
        while (currentTime > 0 && gameStarted)
        {
            yield return new WaitForSeconds(1.0f);

            UIManager.instance.SetCountdown(currentTime, currentTime <= 1);
            currentTime--;
        }

        UIManager.instance.SetMessage("Game Over", 2f, true);
    }

    public void GameTypeChanged(GameType _gameType)
    {
        gameType = _gameType;
        gameMode = GameMode.CreateGameModeFromType(gameType);

        if (NetworkManager.NetworkType != NetworkType.ServerOnly)
        {
            UIManager.instance.gameRulesPanel.OnGameTypeChangedRemotely(false);
        }
    }

    public void GameRulesChanged(GameRules _gameRules)
    {
        gameMode.SetGameRules(_gameRules);

        if (NetworkManager.NetworkType != NetworkType.ServerOnly)
        {
            UIManager.instance.gameRulesPanel.OnGameRulesUpdatedRemotely(_gameRules);
        }
    }

    public void FadeMusic(bool fadeOut)
    {
        StartCoroutine(FadeMusic(fadeOut, 4));
    }

    public float musicStartVolume; //todo: why are these down here
    float fadeTime;
    IEnumerator FadeMusic(bool fadeOut, float fadeDuration) //todo: should be in an audiomanager!
    {
        if (fadeOut)
        {
            fadeTime = musicStartVolume;
            while (fadeTime > 0)
            {
                yield return new WaitForSeconds(Time.fixedDeltaTime);

                fadeTime -= Time.fixedDeltaTime / fadeDuration;

                music.volume = fadeTime;
            }

            music.Stop();
        } else
        {
            music.Play();
            music.volume = 0;

            fadeTime = 0;
            while (fadeTime < musicStartVolume)
            {
                yield return new WaitForSeconds(Time.fixedDeltaTime);

                fadeTime += Time.fixedDeltaTime / fadeDuration;

                music.volume = fadeTime;
            }
        }
    }

    public void OnLocalPlayerDisconnection()
    {
        billboardTarget.SetParent(transform, false);

        foreach (Player player in Player.list.Values)
        {
            UIManager.instance.RemovePlayerReady(player.Id);
            Destroy(player.gameObject);
        }
        Player.list.Clear();

        sceneCamera.SetActive(true);

        if (gameStarted)
        {
            UnloadScene(SceneManager.GetActiveScene().name, true);
        }
        else
        {
            PickupSpawner.DestroyPickupSpawners();
        }
    }






    public void TryStartGame(int _fromClient)
    {
        if (!tryStartGameActive)
        {
            if (AreAllPlayersReady())
            {
                gameMode.TryGameStartSuccess();

                tryStartGameActive = true;

                LevelManager.GetLevelManagerForScene(activeSceneName).LoadScene(gameMode.sceneName, LevelType.Map);
                ServerSend.ChangeScene(gameMode.sceneName);
            }
            else
            {
                ServerSend.SendErrorResponse(ErrorResponseCode.NotAllPlayersReady);
                tryStartGameActive = false;
            }
        }
        else
        {
            ServerSend.SendErrorResponse(ErrorResponseCode.NotAllPlayersReady); //TODO:Change to different message(game already trying to start)
            tryStartGameActive = false;
        }
    }

    private bool AreAllPlayersReady()
    {
        foreach (Player player in Player.list.Values)
        {
            if (!player.isReady)
            {
                return false;
            }
        }

        return true;
    }

    public void CheckForGameOver()
    {
        if (gameMode.CheckForGameOver())
        {
            GameOver();
        }
    }

    public void AttemptColourChange(ushort playerId, Color newColour, bool isSeekerColour) //todo: put in its own colour manager
    {
        bool isColourChangeAllowed = true;
        if (!gameStarted && !isSeekerColour)
        {
            Color previousColour = Player.list[playerId].activeColour;
            isColourChangeAllowed = ClaimHiderColour(previousColour, newColour);
        }

        if (isColourChangeAllowed)
        {
            Player.list[playerId].activeColour = newColour;

            if (NetworkManager.NetworkType == NetworkType.ClientServer)
            {
                Player.list[playerId].ChangeBodyColour(newColour, isSeekerColour);
            }

            ServerSend.SetPlayerColour(playerId, newColour, isSeekerColour);
        }
        else
        {
            //todo: send error response - colour already chosen
        }
    }

    public bool DoesChosenColoursContainColour(Color colour, out int index)
    {
        for (int i = 0; i < chosenColours.Length; i++)
        {
            if (ColourCompare(chosenColours[i].colour, colour))
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    public bool IsColourAvaliable(Color newColour) //todo: put in its own colour manager
    {
        if (DoesChosenColoursContainColour(newColour, out int index))
        {
            return !chosenColours[index].isTaken;
        }

        return false;
    }

    public bool ClaimHiderColour(Color previousColour, Color newColour)
    {
        if (DoesChosenColoursContainColour(newColour, out int newIndex))
        {
            if (!chosenColours[newIndex].isTaken)
            {
                if (DoesChosenColoursContainColour(previousColour, out int previousIndex))
                {
                    if (chosenColours[previousIndex].isTaken)
                    {
                        chosenColours[previousIndex].isTaken = false;
                    }
                }

                return chosenColours[newIndex].isTaken = true;
            }
        }

        Debug.Log("Colour does not exist");
        return false;
    }

    public void UnclaimHiderColour(Color colour)
    {
        if (DoesChosenColoursContainColour(colour, out int index))
        {
            if (chosenColours[index].isTaken)
            {
                chosenColours[index].isTaken = false;
            }
        }
    }

    public Color GetNextAvaliableColour()
    {
        for (int i = 0; i < chosenColours.Length; i++)
        {
            if (!chosenColours[i].isTaken)
            {
                chosenColours[i].isTaken = true;
                return chosenColours[i].colour;
            }
        }

        throw new System.Exception("ERROR: No colours are left to choose from");
    }

    public void OnPlayerSpawned(Player _player)
    {
        if (!_player.IsLocal) return;

        UIManager.instance.CloseAllPanels();
        UIManager.instance.DisplayPanel(UIPanelType.Gameplay);

        sceneCamera.SetActive(false);

        billboardTarget.SetParent(Camera.main.transform, false);
    }

    public bool ColourCompare(Color col1, Color col2)
    {
        string col1Str = ColorUtility.ToHtmlStringRGBA(col1);
        string col2Str = ColorUtility.ToHtmlStringRGBA(col2);

        return col1Str == col2Str;
    }
}

public enum FootstepType
{
    Null,
    Stone,
    Grass,
    Water
}