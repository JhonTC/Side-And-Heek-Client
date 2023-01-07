using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Riptide;
using Riptide.Utils;

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
    setPlayerHost
}

public enum ClientToServerId : ushort
{
    name = 1,
    //welcomeReceived,
    playerMovement,
    playerReady,
    tryStartGame,
    setPlayerColour,
    pickupSelected,
    itemUsed,
    gameRulesChanged
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

    [SerializeField] private string ip;
    [SerializeField] private ushort port;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        ErrorResponseHandler.InitialiseErrorResponseData();

        Client = new Riptide.Client();
        Client.Connected += DidConnect;
        Client.ConnectionFailed += FailedToConnect;
        Client.ClientDisconnected += PlayerLeft;
        Client.Disconnected += DidDisconnect;
    }

    private void FixedUpdate()
    {
        Client.Update();
    }

    private void OnApplicationQuit()
    {
        Client.Disconnect();
    }

    public void Connect(string _ip)
    {
        if (_ip != "")
        {
            ip = _ip;
        }

        Client.Connect($"{ip}:{port}");
    }

    private void DidConnect(object sender, EventArgs e)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.name); //todo: move to clientSend
        message.AddString(UIManager.instance.connectPanel.GetName());
        Client.Send(message);
    }

    private void FailedToConnect(object sender, EventArgs e)
    {
        UIManager.instance.BackToMenu();
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        Destroy(LobbyManager.players[e.Id].gameObject);
        LobbyManager.players.Remove(e.Id);

        UIManager.instance.RemovePlayerReady(e.Id);
        UIManager.instance.customisationPanel.hiderColourSelector.UpdateAllButtons();
        UIManager.instance.gameplayPanel.UpdatePlayerTypeViews();
    }

    public void DidDisconnect(object sender, EventArgs e)
    {
        OnDisconnection();
    }

    public void OnDisconnection()
    {
        LobbyManager.instance.OnLocalPlayerDisconnection();
        UIManager.instance.BackToMenu();
        GameManager.instance.FadeMusic(false);
        GameManager.instance.gameStarted = false;

        NetworkObjectsManager.instance.ClearAllNetworkObjects();

        Debug.Log("Disconnected from server.");
    }
}

