using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class RelayNetworkManager : MonoBehaviour
{
    public static RelayNetworkManager _instance;

    public static RelayNetworkManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(RelayNetworkManager)} instance already exists, destroying duplicate");
                Destroy(value);
            }
        }
    }

    public RelayNetworkClient Client { get; private set; }
    public RelayNetworkHost Server { get; private set; }

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
        if (NetworkType == NetworkType.Client)
        {
            Client?.Update();
        }
        else
        {
            Server?.Update();
        }
    }

    private void OnApplicationQuit()
    {
        if (NetworkType == NetworkType.Client)
        {
            Client?.Disconnect();
        }
        else
        {
            Server?.Stop();
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

    private void SetupClient()
    {
        Client = new Riptide.Client();
        Client.Connected += DidConnect;
        Client.ConnectionFailed += FailedToConnect;
        Client.ClientDisconnected += PlayerLeft;
        Client.Disconnected += DidDisconnect;
    }
    public void Connect(string _ip)
    {
        NetworkType = NetworkType.Client;
        GameManager.instance.OnNetworkTypeSetup();
        SetupClient();

        if (_ip != "")
        {
            ip = _ip;
        }

        Client.Connect($"{ip}:{port}");
    }

    private void SetupServer()
    {
        Server = new Riptide.Server();
        Server.Start(port, maxClientCount);
        Server.ClientDisconnected += PlayerLeft;

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

    private void DidConnect(object sender, EventArgs e)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.name); //todo: move to clientSend
        message.AddString(UIManager.instance.connectPanel.GetName());
        Client.Send(message);
    }

    private void FailedToConnect(object sender, EventArgs e)
    {
        print(e.ToString());
        UIManager.instance.BackToMenu();
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        Destroy(Player.list[e.Id].gameObject);
        Player.list.Remove(e.Id);

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
        GameManager.instance.OnLocalPlayerDisconnection();
        UIManager.instance.BackToMenu();
        GameManager.instance.FadeMusic(false);
        GameManager.instance.gameStarted = false;

        NetworkObjectsManager.instance.ClearAllNetworkObjects();

        Debug.Log("Disconnected from server.");
    }
    #endregion

    #region ServerFunctions
    private void PlayerLeft(object sender, ServerDisconnectedEventArgs e)
    {
        ushort clientId = e.Client.Id;

        bool isLeavingPlayerHost = Player.list[clientId].isHost;

        if (GameManager.instance.gameStarted)
        {
            GameManager.instance.gameMode.OnPlayerLeft(Player.list[clientId]);
        }

        GameManager.instance.UnclaimHiderColour(Player.list[clientId].activeColour);

        Player.list[clientId].DespawnPlayer();
        Destroy(Player.list[clientId].gameObject);
        Player.list.Remove(clientId);

        if (NetworkType == NetworkType.ClientServer)
        {
            UIManager.instance.RemovePlayerReady(clientId);
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
