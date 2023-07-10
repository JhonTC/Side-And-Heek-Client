using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Riptide;
using Riptide.Utils;
using Unity.Services.Core;
using Unity.Services.Authentication;

public enum ServerToClientId : ushort
{
    welcome = 1,
    playerSpawned,
    playerPosition,
    playerRotation,
    playerState,
    createItemSpawner,
    pickupSpawned,
    pickupPickedUp,
    itemSpawned,
    networkObjectTransform,
    itemUseComplete,
    networkObjectDestroyed,
    playerReadyToggled,
    changeScene,
    unloadScene,
    setPlayerType,
    setSpecialCountdown,
    setPlayerColour,
    setPlayerMaterialType,
    sendErrorResponseCode,
    gameStart,
    gameOver,
    playerTeleported,
    gameRulesChanged,
    setPlayerHost,
    setVisualEffect,
    weatherObjectTransform //Todo: really should be part of networkObjectTransform... Its a big ol' job
}

public enum ClientToServerId : ushort
{
    name = 1,
    playerInput,
    playerReady,
    tryStartGame,
    setPlayerColour,
    pickupSelected,
    itemUsed,
    gameRulesChanged,
    command
}


public enum NetworkType
{
    Client,
    ClientServer,
    ServerOnly,
}

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager _instance;

    public static NetworkManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying duplicate");
                Destroy(value);
            }
        }
    }

    public Riptide.Client Client { get; private set; }
    public Riptide.Server Server { get; private set; }

    [SerializeField] private string ip;
    [SerializeField] private ushort port;
    [SerializeField] private ushort maxClientCount;
    public ushort localHostingClientId = ushort.MaxValue;

    public Player playerPrefab;

    public static NetworkType NetworkType;
    public static bool IsServer => NetworkType == NetworkType.ClientServer || NetworkType == NetworkType.ServerOnly;

    [SerializeField] private bool runAsServer = false; //for testing in editor

    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        ErrorResponseHandler.InitialiseErrorResponseData();
#if UNITY_SERVER
        networkType = NetworkType.ServerOnly;
        GameManager.instance.OnNetworkTypeSetup();
        SetupServer();
#else
        if (runAsServer) //for testing in editor
        {
            SetupServer();
            UIManager.instance.gameObject.SetActive(false);
        }

        await UnityServices.InitializeAsync();
        OnSignIn();
#endif
    }

    public async void OnSignIn()
    {
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        string playerId = AuthenticationService.Instance.PlayerId;

        Debug.Log($"Signed in. Player ID: {playerId}");
    }

    private void FixedUpdate()
    {
        if (NetworkManager.NetworkType == NetworkType.ClientServer)
        {
            Server?.Update();
        }
        else if (NetworkManager.NetworkType == NetworkType.Client)
        {
            Client?.Update();
        }
    }

    private void OnApplicationQuit()
    {
        if (NetworkType == NetworkType.ClientServer)
        {
            Server?.Stop();
        }
        else if (NetworkType == NetworkType.Client)
        {
            Client?.Disconnect();
        }
    }

    public bool IsLocalPlayer(ushort id)
    {
        if (NetworkType == NetworkType.Client)
        {
            return id == Client.Id;
        }

        if (NetworkType == NetworkType.ClientServer)
        {
            return id == localHostingClientId;
        }

        return false;
    }

    public void Connect(string _ip)
    {
        NetworkType = NetworkType.Client;
        GameManager.instance.OnNetworkTypeSetup();
        
        if (_ip != "")
        {
            ip = _ip;
        }

        Client.Connect(_ip);
    }

    private void SetupServer()
    {
        Server.Start();

        if (NetworkType == NetworkType.ServerOnly)
        {
            Application.targetFrameRate = 60;
        }
    }
    public void Host(string username = "")
    {
        NetworkType = NetworkType.ClientServer;
        GameManager.instance.OnNetworkTypeSetup();
        SetupServer();

        Debug.Log($"Message from server: Welcome player({username}), you are the host!");

        Player.Spawn(localHostingClientId, username);

        UIManager.instance.CloseAllPanels();
        InputHandler.instance.SwitchInput("PlayerControls");
    }

    #region ClientFunctions

    public void DidConnect()
    {

    }

    public void FailedToConnect()
    {
        UIManager.instance.BackToMenu();
    }

    public void ClientPlayerLeft(ushort playerId)
    {
        Destroy(Player.list[playerId].gameObject);
        Player.list.Remove(playerId);

        UIManager.instance.RemovePlayerReady(playerId);
        UIManager.instance.customisationPanel.hiderColourSelector.UpdateAllButtons();
        UIManager.instance.gameplayPanel.UpdatePlayerTypeViews();
    }

    public void DidDisconnect()
    {
        GameManager.instance.OnLocalPlayerDisconnection();
        UIManager.instance.BackToMenu();
        GameManager.instance.FadeMusic(false);
        GameManager.instance.gameStarted = false;

        NetworkObjectsManager.instance.ClearAllNetworkObjects();

        Debug.Log("Disconnected from server.");
    }
    #endregion

    #region ServerFunctions
    public void ServerPlayerLeft(ushort playerId)
    {
        Player leavingPlayer = Player.list[playerId];
        bool isLeavingPlayerHost = leavingPlayer.isHost;

        if (GameManager.instance.gameStarted)
        {
            GameManager.instance.gameMode.OnPlayerLeft(leavingPlayer);
        }

        GameManager.instance.UnclaimHiderColour(leavingPlayer.activeColour);

        leavingPlayer.DespawnPlayer();
        Destroy(leavingPlayer.gameObject);
        Player.list.Remove(playerId);

        if (NetworkType == NetworkType.ClientServer)
        {
            UIManager.instance.RemovePlayerReady(playerId);
            UIManager.instance.gameplayPanel.UpdatePlayerTypeViews();
        }

        GameManager.instance.CheckForGameOver();

        if (Player.list.Count > 0)
        {
            if (isLeavingPlayerHost)
            {
                //Player.AppointNewHost(); //todo: needs managing depending on whether NetworkType is ClientServer or just Server...
            }
        }
        else
        {
            //Application.Quit();
            Debug.LogWarning("Last Player left, server should close");
        }
    }
    #endregion
}

