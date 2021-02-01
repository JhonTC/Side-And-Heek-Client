using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> disconnectedPlayers = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, ItemSpawner> itemSpawners = new Dictionary<int, ItemSpawner>();
    
    public PlayerManager playerPrefab;
    public ItemSpawner itemSpawnerPrefab;

    public GameObject sceneCamera;

    public bool gameStarted = false;

    public Color unreadyColour;
    public Color unreadyTextColour;
    public Color unreadyEyeColour;
    public Color readyColour;

    private int currentTime = 0;

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

    private void OnLevelFinishedLoading(Scene _scene, LoadSceneMode _loadSceneMode)
    {
        SceneManager.SetActiveScene(_scene);

        if (players.Count > 0)
        {
            if (GetLocalPlayer().playerType == PlayerType.Hider) //TODO: Move this to PlayerManager, called when a player is teleported
            {
                ResetLocalPlayerCamera();
            }
        }

        if (_scene.name != "Lobby")
        {
            gameStarted = true;

            foreach (PlayerManager player in players.Values)
            {
                player.GameStart();
            }
        }
    }

    bool destoryItemSpawners;
    private void OnLevelFinishedUnloading(Scene _scene)
    {
        if (destoryItemSpawners)
        {
            DestroyItemSpawners();
        }
    }

    public PlayerManager GetLocalPlayer()
    {
        foreach(PlayerManager player in players.Values)
        {
            if (player.hasAuthority)
            {
                return player;
            }
        }

        throw new System.Exception("ERROR: No PlayerManager is marked as local (hasAuthority)");
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

    public void SpawnPlayer(int _id, string _username, bool _isReady, Vector3 _position, Quaternion _rotation, PlayerType _playerType, List<TaskCode> _activeTasks)
    {
        PlayerManager _player;
        _player = Instantiate(playerPrefab, _position, _rotation);
        _player.Init(_id, _username, _isReady, _id == Client.instance.myId, _playerType, _activeTasks);

        players.Add(_id, _player);

        if (_id == Client.instance.myId)
        {
            ResetLocalPlayerCamera(sceneCamera.transform.position, true);
            sceneCamera.SetActive(false);
        }
    }

    private void ResetLocalPlayerCamera() { ResetLocalPlayerCamera(Vector3.zero, false); }
    private void ResetLocalPlayerCamera(Vector3 _position, bool _useSentPosition)
    {
        if (!_useSentPosition)
        {
            if (SceneManager.GetActiveScene().name.Contains("Lobby"))
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

    public void CreateItemSpawner(int _spawnerId, Vector3 _position, bool _hasItem, TaskCode _code, string _taskName, string _taskContent, Color _taskDifficulty)
    {
        ItemSpawner _spawner = Instantiate(itemSpawnerPrefab, _position, itemSpawnerPrefab.transform.rotation);
        _spawner.Init(_spawnerId, _hasItem, _code, _taskName, _taskContent, _taskDifficulty);
        itemSpawners.Add(_spawnerId, _spawner);
    }

    public void OnLocalPlayerDisconnection()
    {
        foreach (PlayerManager player in players.Values)
        {
            UIManager.instance.RemovePlayerReady(player.id);
            Destroy(player.gameObject);
        }
        players.Clear();

        sceneCamera.SetActive(true);

        if (gameStarted)
        {
            UnloadScene("Map_1", true);
        } else
        {
            DestroyItemSpawners();
        }
    }

    private void DestroyItemSpawners()
    {
        foreach (ItemSpawner spawner in itemSpawners.Values)
        {
            Destroy(spawner.gameObject);
        }
        itemSpawners.Clear();
    }

    public void StartGameTimer(int _duration) 
    {
        StartCoroutine(GameTimeCountdown(_duration));
    }
    private IEnumerator GameTimeCountdown(int _duration = 240)
    {
        currentTime = _duration;
        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1.0f);

            GetLocalPlayer().SetCountdown(currentTime, currentTime <= 1);
            currentTime--;
        }
    }
}

public enum TaskCode
{
    NULL_TASK,
    TestTaskEasy,
    TestTaskNormal,
    TestTaskHard,
    TestTaskExtreme
}