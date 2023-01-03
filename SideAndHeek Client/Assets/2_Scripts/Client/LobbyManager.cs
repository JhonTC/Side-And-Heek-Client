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

    public Transform billboardTarget;

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
        billboardTarget.SetParent(transform, false);

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
            GameManager.instance.DestroyPickupSpawners();
            PickupHandler.ClearAllActivePickups();
            ItemHandler.ClearAllActiveItems();
        }
    }

    public void OnPlayerSpawned(Player _player)
    {
        if (_player.IsLocal)
        {
            GameManager.instance.ResetLocalPlayerCamera(sceneCamera.transform.position, true);
            sceneCamera.SetActive(false);

            if (_player.isHost && GameManager.instance.networkType == NetworkType.Multiplayer)
            {
                ClientSend.GameRulesChanged(GameManager.instance.gameRules);
            }

            billboardTarget.SetParent(Camera.main.transform, false);
        }

        players.Add(_player.Id, _player);
    }
}
