using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    public GameObject sceneCamera;
    public PlayerManager playerPrefab;

    public Color unreadyColour;
    public Color unreadyTextColour;
    public Color readyColour;

    public bool tryStartGameActive = false;

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
        foreach (PlayerManager player in players.Values)
        {
            UIManager.instance.RemovePlayerReady(player.id);
            Destroy(player.gameObject);
        }
        players.Clear();

        sceneCamera.SetActive(true);

        if (GameManager.instance.gameStarted)
        {
            GameManager.instance.UnloadScene("Map_1", true);
        }
        else
        {
            GameManager.instance.DestroyItemSpawners();
        }
    }

    public void SpawnPlayer(int _id, string _username, bool _isReady, Vector3 _position, Quaternion _rotation)
    {
        PlayerManager _player;
        _player = Instantiate(playerPrefab, _position, _rotation);
        _player.Init(_id, _username, _isReady, _id == Client.instance.myId, players.Count == 0);

        players.Add(_id, _player);

        if (_id == Client.instance.myId)
        {
            GameManager.instance.ResetLocalPlayerCamera(sceneCamera.transform.position, true);
            sceneCamera.SetActive(false);
        }
    }

    public PlayerManager GetLocalPlayer()
    {
        foreach (PlayerManager player in players.Values)
        {
            if (player.hasAuthority)
            {
                return player;
            }
        }

        throw new System.Exception("ERROR: No PlayerManager is marked as local (hasAuthority)");
    }
}
