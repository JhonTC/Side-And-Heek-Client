using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, ItemSpawner> itemSpawners = new Dictionary<int, ItemSpawner>();
    
    public PlayerManager playerPrefab;
    public ItemSpawner itemSpawnerPrefab;

    [SerializeField] private GameObject sceneCamera;

    public bool gameStarted = false;

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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading(Scene _scene, LoadSceneMode _loadSceneMode)
    {
        if (players.Count > 0)
        {
            ResetLocalPlayerCamera();
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

    public void LoadScene(string sceneName)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        PlayerManager _player;
        _player = Instantiate(playerPrefab, _position, _rotation);
        _player.Init(_id, _username, _id == Client.instance.myId, players.Count == 0);

        if (_id == Client.instance.myId)
        {
            sceneCamera.SetActive(false);
        }
        
        players.Add(_id, _player);

        ResetLocalPlayerCamera();
    }

    private void ResetLocalPlayerCamera()
    {
        if (SceneManager.GetActiveScene().name.Contains("Lobby"))
        {
            GetLocalPlayer().camera.transform.position = new Vector3(-1000, 100, 0);
        }
        else
        {
            GetLocalPlayer().camera.transform.position = new Vector3(0, 100, 0);
        }
    }

    public void CreateItemSpawner(int _spawnerId, Vector3 _position, bool _hasItem)
    {
        ItemSpawner _spawner = Instantiate(itemSpawnerPrefab, _position, itemSpawnerPrefab.transform.rotation);
        _spawner.Init(_spawnerId, _hasItem);
        itemSpawners.Add(_spawnerId, _spawner);
    }
}
