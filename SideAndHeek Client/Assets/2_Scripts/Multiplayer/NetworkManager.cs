using UnityEngine;
using System;
using Riptide;
using Riptide.Utils;
using Riptide.Transports.Udp;
using Riptide.Transports;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Riptide.Transports.UnityTransport;
using System.Threading.Tasks;

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
    requestSceneChange,
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
    public static bool IsConnected => (Instance.Client != null && Instance.Client.IsConnected) || (Instance.Server != null && Instance.Server.IsRunning); 

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
            StartServer();
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
        EndActiveConnection(false);
    }

    public void EndActiveConnection(bool unsubscribe = true)
    {
        if (NetworkType == NetworkType.Client)
        {
            Client?.Disconnect();
        }
        else
        {
            Server?.Stop();
        }

        if (unsubscribe)
            UnsubscribeFromConnectionEvents();
    }

    public void UnsubscribeFromConnectionEvents()
    {
        if (NetworkType == NetworkType.Client)
        {
            if (Client != null)
            {
                Client.Connected -= DidConnect;
                Client.ConnectionFailed -= FailedToConnect;
                Client.ClientDisconnected -= PlayerLeft;
                Client.Disconnected -= DidDisconnect;
                Client = null;
            }
        }
        else
        {
            if (Server != null)
            {
                Server.ClientDisconnected -= PlayerLeft;
                Server = null;
            }
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
        SetupClient(new UdpClient());
    }
    private void SetupClient(IClient transport)
    {
        Client = new Riptide.Client(transport);
        Client.Connected += DidConnect;
        Client.ConnectionFailed += FailedToConnect;
        Client.ClientDisconnected += PlayerLeft;
        Client.Disconnected += DidDisconnect;
    }
    public async void Connect(string joinValue)
    {
        NetworkType = NetworkType.Client;
        GameManager.instance.OnNetworkTypeSetup();

        if (UIManager.instance.connectPanel.GetUseIP())
        {
            if (joinValue != "")
            {
                ip = joinValue;
            }

            SetupClient();
            Client.Connect($"{ip}:{port}");
        }
        else
        {
            UtpClient client = new UtpClient();
            await client.PrepareConnect(joinValue);
            SetupClient(client);
            Client.Connect(joinValue);
        }
    }

    private void StartServer()
    {
        StartServer(new UdpServer());
    }
    private void StartServer(IServer transport)
    {
        Server = new Riptide.Server(transport);
        Server.Start(port, maxClientCount);
        Server.ClientDisconnected += PlayerLeft;

        if (NetworkType == NetworkType.ServerOnly)
        {
            Application.targetFrameRate = 60;
            UIManager.instance.CloseAllPanels();
        }
    }

    public async void Host(string username = "")
    {
        NetworkType = NetworkType.ClientServer;
        GameManager.instance.OnNetworkTypeSetup();

        if (UIManager.instance.connectPanel.GetUseIP())
        {
            StartServer();
        }
        else
        {
            UtpServer server = new UtpServer();
            await server.PrepareStart(maxClientCount);
            StartServer(server);
        }

        Debug.Log($"Message from server: Welcome player({username}), you are the host!");

        Player.Spawn(localHostingClientId, username);

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
        UnsubscribeFromConnectionEvents();

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

