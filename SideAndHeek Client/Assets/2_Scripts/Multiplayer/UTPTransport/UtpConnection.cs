using Riptide;
using System;
using System.Collections.Generic;
using Unity.Networking.Transport;

public class UtpConnection : Connection, IEquatable<UtpConnection>
{
    public NetworkConnection networkConnection { get; private set; }

    /// <summary>The local peer this connection is associated with.</summary>
    private readonly UtpPeer peer;

    /// <summary>Initializes the connection.</summary>
    /// <param name="peer">The local peer this connection is associated with.</param>
    internal UtpConnection(NetworkConnection networkConnection, UtpPeer peer)
    {
        this.networkConnection = networkConnection;
        this.peer = peer;
    }

    public void SetNetworkConnection(NetworkConnection networkConnection)
    {
        this.networkConnection = networkConnection;
    }

    protected override void Send(byte[] dataBuffer, int amount)
    {
        peer.Send(dataBuffer, amount, this);
    }

    /// <inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as UtpConnection);
    /// <inheritdoc/>
    public bool Equals(UtpConnection other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return networkConnection.Equals(other.networkConnection);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return -288961498 + EqualityComparer<NetworkConnection>.Default.GetHashCode(networkConnection);
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static bool operator ==(UtpConnection left, UtpConnection right)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        if (left is null)
        {
            if (right is null)
                return true;

            return false; // Only the left side is null
        }

        // Equals handles case of null on right side
        return left.Equals(right);
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static bool operator !=(UtpConnection left, UtpConnection right) => !(left == right);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
