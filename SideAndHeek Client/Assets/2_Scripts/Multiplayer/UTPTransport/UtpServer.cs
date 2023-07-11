using Riptide;
using Riptide.Transports;
using Riptide.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using Unity.Networking.Transport;

public class UtpServer : UtpPeer, IServer
{
    /// <inheritdoc/>
    public event EventHandler<ConnectedEventArgs> Connected;
    /// <inheritdoc/>
    public event EventHandler<DataReceivedEventArgs> DataReceived;

    /// <inheritdoc/>
    public ushort Port { get; private set; }

    /// <summary>The currently open connections, accessible by their endpoints.</summary>
    private Dictionary<NetworkConnection, Connection> connections;

    /// <inheritdoc/>
    public UtpServer() : base()
    {
        RelayNetwork = new RelayNetworkHost();
    }

    /// <inheritdoc/>
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

    /// <summary>Decides what to do with a connection attempt.</summary>
    /// <param name="fromEndPoint">The endpoint the connection attempt is coming from.</param>
    /// <returns>Whether or not the connection attempt was from a new connection.</returns>
    private bool HandleConnectionAttempt(UtpConnection connection)
    {
        if (connections.ContainsKey(connection.networkConnection))
            return false;

        connections.Add(connection.networkConnection, connection);
        OnConnected(connection);
        return true;
    }

    /// <inheritdoc/>
    public void Close(Connection connection)
    {
        if (connection is UtpConnection udpConnection)
            connections.Remove(udpConnection.networkConnection);
    }

    /// <inheritdoc/>
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

    /// <summary>Invokes the <see cref="Connected"/> event.</summary>
    /// <param name="connection">The successfully established connection.</param>
    protected virtual void OnConnected(Connection connection)
    {
        Connected?.Invoke(this, new ConnectedEventArgs(connection));
    }

    /// <inheritdoc/>
    protected override void OnDataReceived(byte[] dataBuffer, int amount, NetworkConnection networkConnection)
    {
        if ((MessageHeader)dataBuffer[0] == MessageHeader.Connect && !HandleConnectionAttempt(new UtpConnection(networkConnection, this)))
            return;

        if (connections.TryGetValue(networkConnection, out Connection connection) && !connection.IsNotConnected)
            DataReceived?.Invoke(this, new DataReceivedEventArgs(dataBuffer, amount, connection));
    }
}
