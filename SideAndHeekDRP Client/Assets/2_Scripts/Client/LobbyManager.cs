using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;

    public static Dictionary<ushort, Player> players = new Dictionary<ushort, Player>();
    public static Player localPlayer;

    public GameObject sceneCamera;
    public Player playerPrefab;

    public Color unreadyColour;
    public Color unreadyTextColour;
    public Color readyColour;

    public bool tryStartGameActive = false;

    public string mapScene;

    public bool isHost = false;

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

    public void OnLocalPlayerDisconnection()
    {
        foreach (Player player in players.Values)
        {
            UIManager.instance.RemovePlayerReady(player.Id);
            Destroy(player.gameObject);
        }
        players.Clear();

        sceneCamera.SetActive(true);

        if (GameManager.instance.gameStarted)
        {
            GameManager.instance.UnloadScene(mapScene, true);
        }
        else
        {
            GameManager.instance.DestroyItemSpawners();
        }
    }

    public void OnPlayerSpawned(Player _player)
    {
        if (_player.IsLocal)
        {
            GameManager.instance.ResetLocalPlayerCamera(sceneCamera.transform.position, true);
            sceneCamera.SetActive(false);

            if (_player.isHost && GameManager.instance.gameType == GameType.Multiplayer)
            {
                ClientSend.GameRulesChanged(GameManager.instance.gameRules);
            }
        }

        players.Add(_player.Id, _player);
    }

    public Player GetLocalPlayer()
    {
        foreach (Player player in players.Values)
        {
            if (player.IsLocal)
            {
                return player;
            }
        }

        throw new System.Exception("ERROR: No PlayerManager is marked as local (hasAuthority)");
    }
}
