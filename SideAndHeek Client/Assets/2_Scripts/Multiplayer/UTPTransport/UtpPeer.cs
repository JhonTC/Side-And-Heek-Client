using System;
using System.Net;
using System.Net.Sockets;
using Riptide;
using Riptide.Transports;
using Riptide.Transports.Udp;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Services.Relay.Models;
using System.IO;

public abstract class UtpPeer
{
    /// <inheritdoc cref="IPeer.Disconnected"/>
    public event EventHandler<Riptide.Transports.DisconnectedEventArgs> Disconnected;

    /// <summary>The default size used for the socket's send and receive buffers.</summary>
    protected const int DefaultSocketBufferSize = 1024 * 1024; // 1MB
    /// <summary>The minimum size that may be used for the socket's send and receive buffers.</summary>
    private const int MinSocketBufferSize = 256 * 1024; // 256KB
    /// <summary>How long to wait for a packet, in microseconds.</summary>
    private const int ReceivePollingTime = 500000; // 0.5 seconds

    /// <summary>Whether to create an IPv4 only, IPv6 only, or dual-mode socket.</summary>
    //protected readonly SocketMode mode;
    /// <summary>The size to use for the socket's send and receive buffers.</summary>
    private readonly int socketBufferSize;
    /// <summary>The array that incoming data is received into.</summary>
    private readonly byte[] receivedData;
    /// <summary>The socket to use for sending and receiving.</summary>
    private Socket socket;
    /// <summary>Whether or not the transport is running.</summary>
    public bool isRunning;
    /// <summary>A reusable endpoint.</summary>
    private EndPoint remoteEndPoint;

    public RelayNetworkClient Client { get; set; }
    public RelayNetworkHost Server { get; set; }

    public string hostLatestMessageReceived;
    public bool isRelayConnected = false;

    protected UtpPeer(int socketBufferSize)
    {
        if (socketBufferSize < MinSocketBufferSize)
            throw new ArgumentOutOfRangeException(nameof(socketBufferSize), $"The minimum socket buffer size is {MinSocketBufferSize}!");

        this.socketBufferSize = socketBufferSize;
        receivedData = new byte[Message.MaxSize + sizeof(ushort)];
    }

    public void Poll()
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

    internal void Send(byte[] dataBuffer, int numBytes, UtpConnection connection)
    {
        if (isRunning)
        {
            NativeArray<byte> bytes = new NativeArray<byte>(dataBuffer, Allocator.Persistent);

            if (NetworkManager.NetworkType == NetworkType.ClientServer)
            {
                Server.Send(bytes, connection.networkConnection);
            } 
            else if (NetworkManager.NetworkType == NetworkType.Client)
            {
                Client.Send(bytes, connection.networkConnection);
            }

            bytes.Dispose();
        }
    }

    protected abstract void OnDataReceived(byte[] dataBuffer, int amount, NetworkConnection networkConnection);

    protected virtual void OnDisconnected(NetworkConnection connection, DisconnectReason reason)
    {
        Disconnected?.Invoke(this, new Riptide.Transports.DisconnectedEventArgs(new UtpConnection(connection, this), reason));
    }
}
