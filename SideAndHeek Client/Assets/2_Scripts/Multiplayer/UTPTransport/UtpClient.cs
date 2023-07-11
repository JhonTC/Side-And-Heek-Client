using Riptide;
using Riptide.Transports;
using System;
using Unity.Networking.Transport;
using UnityEngine;

public class UtpClient : UtpPeer, IClient
{
    public event EventHandler Connected;
    public event EventHandler ConnectionFailed;
    public event EventHandler<DataReceivedEventArgs> DataReceived;

    private UtpConnection utpConnection;

    public UtpClient() : base()
    {
        RelayNetwork = new RelayNetworkClient();
    }

    public bool Connect(string joinCode, out Connection connection, out string connectError)
    {
        RelayNetworkClient client = RelayNetwork as RelayNetworkClient;
        if (client == null)
        {
            connectError = "RelayNetwork not of type RelayNetworkClient";
            connection = null;
            return false;
        }

        client.Connected += OnConnected;
        client.DataReceived += OnDataReceived;
        client.Disconnected += OnDisconnected;

        connectError = $"Invalid Join Code: '{joinCode}'!";

        isRunning = true;
        client.OnJoin(joinCode);

        connection = utpConnection = new UtpConnection(client.clientConnection, this);
        return true;
    }

    public void Disconnect()
    {
        RelayNetwork?.End();

        RelayNetworkClient client = RelayNetwork as RelayNetworkClient;
        if (client != null)
        {
            client.Connected -= OnConnected;
            client.DataReceived -= OnDataReceived;
            client.Disconnected -= OnDisconnected;
        }
    }

    protected virtual void OnConnected()
    {
        RelayNetworkClient client = RelayNetwork as RelayNetworkClient;
        if (client != null)
        {
            utpConnection.SetNetworkConnection(client.clientConnection);
        }
        Connected?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnConnectionFailed()
    {
        ConnectionFailed?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnDataReceived(byte[] dataBuffer, int amount, NetworkConnection networkConnection)
    {
        Debug.Log($"Recieve MessageHeader: {(MessageHeader)dataBuffer[0]}");

        Debug.Log($"Do connections match: {utpConnection.networkConnection.Equals(networkConnection)}");
        Debug.Log(utpConnection.networkConnection.InternalId); Debug.Log(networkConnection.InternalId);
        if (utpConnection.networkConnection.Equals(networkConnection))
            DataReceived?.Invoke(this, new DataReceivedEventArgs(dataBuffer, amount, utpConnection));
    }
}
