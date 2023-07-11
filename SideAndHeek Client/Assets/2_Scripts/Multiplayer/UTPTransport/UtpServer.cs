using Riptide;
using Riptide.Transports;
using Riptide.Utils;
using System;
using System.Collections.Generic;
using Unity.Networking.Transport;

public class UtpServer : UtpPeer, IServer
{
    public event EventHandler<ConnectedEventArgs> Connected;
    public event EventHandler<DataReceivedEventArgs> DataReceived;

    public ushort Port { get; private set; }

    private Dictionary<NetworkConnection, Connection> connections;

    public UtpServer() : base()
    {
        RelayNetwork = new RelayNetworkHost();
    }

    public void Start(ushort port)
    {
        RelayNetworkHost server = RelayNetwork as RelayNetworkHost;
        if (server == null)
        {
            RiptideLogger.Log(Riptide.Utils.LogType.Error, "RelayNetwork not of type RelayNetworkHost");
            return;
        }

        server.DataReceived += OnDataReceived;

        Port = port;
        connections = new Dictionary<NetworkConnection, Connection>();

        isRunning = true;
        server?.OnAllocate();
    }

    private bool HandleConnectionAttempt(UtpConnection connection)
    {
        if (connections.ContainsKey(connection.networkConnection))
            return false;

        connections.Add(connection.networkConnection, connection);
        OnConnected(connection);
        return true;
    }

    public void Close(Connection connection)
    {
        if (connection is UtpConnection udpConnection)
            connections.Remove(udpConnection.networkConnection);
    }

    public void Shutdown()
    {
        RelayNetwork?.End();
        connections.Clear();

        RelayNetworkHost server = RelayNetwork as RelayNetworkHost;
        if (server != null)
        {
            server.DataReceived -= OnDataReceived;
        }
    }

    protected virtual void OnConnected(Connection connection)
    {
        Connected?.Invoke(this, new ConnectedEventArgs(connection));
    }

    protected override void OnDataReceived(byte[] dataBuffer, int amount, NetworkConnection networkConnection)
    {
        if ((MessageHeader)dataBuffer[0] == MessageHeader.Connect && !HandleConnectionAttempt(new UtpConnection(networkConnection, this)))
            return;

        if (connections.TryGetValue(networkConnection, out Connection connection) && !connection.IsNotConnected)
            DataReceived?.Invoke(this, new DataReceivedEventArgs(dataBuffer, amount, connection));
    }
}
