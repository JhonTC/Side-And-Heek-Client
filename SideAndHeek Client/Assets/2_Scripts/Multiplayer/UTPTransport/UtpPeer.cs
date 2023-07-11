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
    public bool isRunning;

    public IRelayNetwork RelayNetwork { get; set; }

    public string hostLatestMessageReceived;
    public bool isRelayConnected = false;

    protected UtpPeer() {}

    public void Poll()
    {
        RelayNetwork?.Update();
    }

    internal void Send(byte[] dataBuffer, int numBytes, UtpConnection connection)
    {
        if (isRunning)
        {
            NativeArray<byte> bytes = new NativeArray<byte>(dataBuffer, Allocator.Persistent);

            RelayNetwork?.Send(bytes, connection.networkConnection);

            bytes.Dispose();
        }
    }

    protected abstract void OnDataReceived(byte[] dataBuffer, int amount, NetworkConnection networkConnection);

    protected virtual void OnDisconnected(NetworkConnection connection, DisconnectReason reason)
    {
        Disconnected?.Invoke(this, new Riptide.Transports.DisconnectedEventArgs(new UtpConnection(connection, this), reason));
    }
}
