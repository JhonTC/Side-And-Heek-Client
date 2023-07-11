using System;
using Riptide;
using Unity.Collections;
using Unity.Networking.Transport;

public abstract class UtpPeer
{
    public event EventHandler<Riptide.Transports.DisconnectedEventArgs> Disconnected;

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
