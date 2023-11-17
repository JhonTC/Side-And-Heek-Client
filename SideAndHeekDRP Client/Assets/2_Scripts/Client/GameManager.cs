using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; //J: Replace with custom Singleton Class that this GameManager inherits from - this will also handle the singleton setup in the Awake call below.
    //J: Organise the variables into Public, Protected & Private with important variables first, followed by scene reference variables.
    public PickupCollection collection;

    //J: GameRules should be GameMode specific so this shouldnt exist here.
    public GameRules gameRules;

    //J: Pickups shouldnt be stored here(more on this below).
    public static Dictionary<int, PickupSpawner> pickupSpawners = new Dictionary<int, PickupSpawner>();
    public PickupSpawner pickupSpawnerPrefab; //J: NetworkManager should have an array of spawnable network prefabs - this should be in there.

    public bool gameStarted = false;
    public bool gameEndInProgress = false;

    protected int currentTime = 0;

    //J: GameType will be replaced with GameMode. 
    /*J: GameMode will be an abstract or interface type which the actual scripts inherit from (e.g. DeathMatch_GameMode). 
         It will include several variables: gameStarted, gameRules ect... 
    */
    public GameType gameType;

    public string lobbyScene; //J: Change to private & move to LevelManager(It doesnt exist but I mention it more later).

    public GameObject gameStartCollider; //J: Change to [SerialiseField] private.

    //J: Feels like this is GameMode specific, but if it refers to the colours used in all gamemodes then should be renamed playerColours.
    [HideInInspector] public Color[] hiderColours;

    //J: Create an AudioManager that the GameManager owns or has a reference to - think about single responsibility principle.
    private AudioSource music;

    private void Awake() 
    {   //J: This will be handled in the inherited singleton class.
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

    protected virtual void Start()
    {
        music = GetComponent<AudioSource>(); //J: Add class attribute [RequireComponent(typeof(AudioSource)] or make this a [SerialiseField] private variable to be attached in the inspector.
        FadeMusic(false);
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

    protected virtual void OnLevelFinishedLoading(Scene _scene, LoadSceneMode _loadSceneMode)
    {
        SceneManager.SetActiveScene(_scene);

        if (LobbyManager.players.Count > 0)
        {   //J: This is GameMode a specific action as not all gamemodes will have a PlayerType of hider. GameModes should have their own class which has an OnLevelFinishedLoading function called from here.
            if (LobbyManager.instance.GetLocalPlayer().playerType == PlayerType.Hider) //TODO: Move this to PlayerManager, called when a player is teleported
            {
                ResetLocalPlayerCamera();
            }
        }

        if (_scene.name != lobbyScene)
        {   //J: Game Starting should be controlled by the specific GameMode.
            gameStarted = true; 
            gameStartCollider.SetActive(false);

            foreach (PlayerManager player in LobbyManager.players.Values)
            {
                player.GameStart();
            }

            UIManager.instance.DisableAllPanels();
        } 
        else
        {
            gameStartCollider.SetActive(true);
        }
    }

    bool destoryItemSpawners; //J: Move to the top with the other variables and give private access modifier to make it more readable.
    protected virtual void OnLevelFinishedUnloading(Scene _scene)
    {   
        if (destoryItemSpawners)
        {
            DestroyItemSpawners();
        }

        if (SceneManager.GetActiveScene().name == lobbyScene)
        {   
            gameStartCollider.SetActive(true);
            foreach (PlayerManager player in LobbyManager.players.Values)
            {
                player.activePickup = null;
            }

            UIManager.instance.gameplayPanel.SetItemDetails();
        }
    }

    //J: Consider making a LevelManager to handle scene changes instead of GameManager - again, single responsibility principle.
    public void LoadScene(string sceneName, LoadSceneMode loadSceneMode)
    {
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            SceneManager.LoadScene(sceneName, loadSceneMode);
        }
    }
    //J: Same as above.
    public void UnloadScene(string sceneName, bool _destoryItemSpawners)
    {
        destoryItemSpawners = _destoryItemSpawners;
        SceneManager.UnloadSceneAsync(sceneName);
    }

    
    //J: Remove Functions as they are not doing anything.
    public void ResetLocalPlayerCamera() { ResetLocalPlayerCamera(Vector3.zero, false); }
    public void ResetLocalPlayerCamera(Vector3 _position, bool _useSentPosition)
    {
        if (!_useSentPosition)
        {
            if (SceneManager.GetActiveScene().name.Contains(lobbyScene))
            {
                //GetLocalPlayer().thirdPersonCamera.transform.position = new Vector3(-1000, 100, 0);
            }
            else
            {
                //GetLocalPlayer().thirdPersonCamera.transform.position = new Vector3(0, 100, 0);
            }
        } else
        {
            //GetLocalPlayer().thirdPersonCamera.transform.position = _position;
        }
    }

    //J: LevelManager/GameMode should work together to handle this - again, single responsibility principle.
    public void CreatePickupSpawner(int _spawnerId, Vector3 _position)
    {
        PickupSpawner _spawner = Instantiate(pickupSpawnerPrefab, _position, pickupSpawnerPrefab.transform.rotation);
        _spawner.Init(_spawnerId);

        pickupSpawners.Add(_spawnerId, _spawner);
    }
    //J: Same as above
    public void DestroyItemSpawners()
    {
        foreach (PickupSpawner spawner in pickupSpawners.Values)
        {
            if (spawner != null)
            {
                Destroy(spawner.gameObject);
            }
        }
        pickupSpawners.Clear();
    }

    //J: Same as above Move to static Utils class. You can use GameManager.Instance.StartCorotine to call it.
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
    //J: GameRules should be GameMode specific so this shouldnt exist here other than to call the same function on the current gamemode.
    public void GameRulesChanged(GameRules _gameRules)
    {
        gameRules = _gameRules;
        UIManager.instance.gameRulesPanel.SetGameRules(gameRules);
    }

    //J: As stated Earlier, the following functions should be in AudioManager Class and called from here.
    public void FadeMusic(bool fadeOut)
    {
        StartCoroutine(FadeMusic(fadeOut, 4));
    }

    public float musicStartVolume;  //J: Move to the top of new AudioManager class.
    float fadeTime;  //J: Move to the top of new AudioManager class and give private access modifier to make it more readable.
    IEnumerator FadeMusic(bool fadeOut, float fadeDuration)
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
}

//J: NetworkManager should have an enum for Host, Client, Local. If Local is selected then the player is playing offline.
public enum GameType
{
    Singleplayer,
    Multiplayer
}

//J: num not used here, either delete or move to a more approproiate script.
public enum FootstepType
{
    Null,
    Stone,
    Grass,
    Water
}
