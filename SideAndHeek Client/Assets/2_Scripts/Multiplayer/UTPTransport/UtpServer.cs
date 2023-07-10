using Riptide;
using Riptide.Transports;
using Riptide.Transports.Udp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using UnityEngine;

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
    /// <summary>The IP address to bind the socket to, if any.</summary>
    private readonly IPAddress listenAddress;

    /// <inheritdoc/>
    public UtpServer(int socketBufferSize = DefaultSocketBufferSize) : base(socketBufferSize)
    {
        Server = new RelayNetworkHost();
    }

    /// <summary>Initializes the transport, binding the socket to a specific IP address.</summary>
    /// <param name="listenAddress">The IP address to bind the socket to.</param>
    /// <param name="socketBufferSize">How big the socket's send and receive buffers should be.</param>
    public UtpServer(IPAddress listenAddress, int socketBufferSize = DefaultSocketBufferSize) : base(socketBufferSize)
    {
        this.listenAddress = listenAddress;
        Server = new RelayNetworkHost();
    }

    /// <inheritdoc/>
    public void Start(ushort port)
    {
        Server.DataReceived += OnDataReceived;

        Port = port;
        connections = new Dictionary<NetworkConnection, Connection>();

        isRunning = true;
        Server?.OnAllocate();
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
        Server?.Stop();
        connections.Clear();

        Server.DataReceived -= OnDataReceived;
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
