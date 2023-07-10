using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelayNetworkTransport : INetworkTransport
{
    public RelayNetworkClient Client { get; private set; }
    public RelayNetworkHost Server { get; private set; }

    private NetworkManager networkManager;

    public RelayNetworkTransport(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
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
        Server = new RelayNetworkHost();
        Server.OnAllocate();
    }

    public void SetupClient()
    {
        Client = new RelayNetworkClient();
    }

    public void ConnectClient(string joinCode)
    {
        SetupClient();

        Client.OnJoin(joinCode);
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
}
