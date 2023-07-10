using Riptide;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RiptideNetworkTransport : INetworkTransport
{
    public Riptide.Client Client { get; private set; }
    public Riptide.Server Server { get; private set; }

    private NetworkManager networkManager;
    private ushort port;
    private ushort maxClientCount;

    public RiptideNetworkTransport(NetworkManager networkManager, ushort port, ushort maxClientCount)
    {
        this.networkManager = networkManager;
        this.port = port;
        this.maxClientCount = maxClientCount;
    }

    public void Update()
    {
        if (NetworkManager.NetworkType == NetworkType.Client)
        {
            Client?.Update();
        }
        else
        {
            Server?.Update();
        }
    }

    public void StartServer()
    {
        Server = new Riptide.Server();
        Server.Start(port, maxClientCount);
        Server.ClientDisconnected += PlayerLeft;
    }

    public void SetupClient()
    {
        Client = new Riptide.Client();
        Client.Connected += DidConnect;
        Client.ConnectionFailed += FailedToConnect;
        Client.ClientDisconnected += PlayerLeft;
        Client.Disconnected += DidDisconnect;
    }

    public void ConnectClient(string ip)
    {
        SetupClient();

        Client.Connect($"{ip}:{port}");
    }
    
    public void DisconnectClient()
    {
        Client.Disconnect();
    }

    public void StopServer()
    {
        Server.Stop();
    }

    public bool IsLocalPlayer(ushort id)
    {
        return id == Client.Id;
    }

    public void OnApplicationQuit()
    {
        if (NetworkManager.NetworkType == NetworkType.Client)
        {
            Client?.Disconnect();
        }
        else
        {
            Server?.Stop();
        }
    }

    #region ClientFunctions

    private void DidConnect(object sender, EventArgs e)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.name); //todo: move to clientSend
        message.AddString(UIManager.instance.connectPanel.GetName());
        Client.Send(message);

        networkManager.DidConnect();
    }

    private void FailedToConnect(object sender, EventArgs e)
    {
        Debug.Log(e.ToString());

        networkManager.FailedToConnect();
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        networkManager.ClientPlayerLeft(e.Id);
    }

    private void DidDisconnect(object sender, EventArgs e)
    {
        networkManager.DidDisconnect();
    }
    #endregion

    #region ServerFunctions
    private void PlayerLeft(object sender, ServerDisconnectedEventArgs e)
    {
        networkManager.ServerPlayerLeft(e.Client.Id);
    }
    #endregion
}
