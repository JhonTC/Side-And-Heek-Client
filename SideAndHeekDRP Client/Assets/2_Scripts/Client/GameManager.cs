﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PickupCollection collection;
    public GameRules gameRules;

    public static Dictionary<int, PickupSpawner> pickupSpawners = new Dictionary<int, PickupSpawner>();
    public PickupSpawner pickupSpawnerPrefab;

    public bool gameStarted = false;

    protected int currentTime = 0;

    public GameType gameType;

    public string lobbyScene;

    public GameObject gameStartCollider;

    [HideInInspector] public Color[] hiderColours;

    private AudioSource music;

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

    protected virtual void Start()
    {
        music = GetComponent<AudioSource>();
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
        {
            if (LobbyManager.instance.GetLocalPlayer().playerType == PlayerType.Hider) //TODO: Move this to PlayerManager, called when a player is teleported
            {
                ResetLocalPlayerCamera();
            }
        }

        if (_scene.name != lobbyScene)
        {
            gameStarted = true;
            gameStartCollider.SetActive(false);

            foreach (Player player in LobbyManager.players.Values)
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

    bool destoryItemSpawners;
    protected virtual void OnLevelFinishedUnloading(Scene _scene)
    {
        if (destoryItemSpawners)
        {
            DestroyItemSpawners();
        }

        if (SceneManager.GetActiveScene().name == lobbyScene)
        {
            gameStartCollider.SetActive(true);
            foreach (Player player in LobbyManager.players.Values)
            {
                player.activePickup = null;
            }

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

    public void CreatePickupSpawner(ushort _spawnerId, Vector3 _position)
    {
        PickupSpawner _spawner = Instantiate(pickupSpawnerPrefab, _position, pickupSpawnerPrefab.transform.rotation);
        _spawner.Init(_spawnerId);

        pickupSpawners.Add(_spawnerId, _spawner);
    }

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

    public void GameRulesChanged(GameRules _gameRules)
    {
        gameRules = _gameRules;
        UIManager.instance.gameRulesPanel.SetGameRules(gameRules);
    }

    public void FadeMusic(bool fadeOut)
    {
        StartCoroutine(FadeMusic(fadeOut, 4));
    }

    public float musicStartVolume;
    float fadeTime;
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

public enum GameType
{
    Singleplayer,
    Multiplayer
}

public enum FootstepType
{
    Null,
    Stone,
    Grass,
    Water
}