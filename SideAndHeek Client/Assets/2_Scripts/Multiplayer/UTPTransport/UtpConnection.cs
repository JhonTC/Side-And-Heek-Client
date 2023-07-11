using Riptide;
using System;
using System.Collections.Generic;
using Unity.Networking.Transport;

public class UtpConnection : Connection, IEquatable<UtpConnection>
{
    public NetworkConnection networkConnection { get; private set; }

    private readonly UtpPeer peer;

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

    public override bool Equals(object obj) => Equals(obj as UtpConnection);
    public bool Equals(UtpConnection other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return networkConnection.Equals(other.networkConnection);
    }

    public override int GetHashCode()
    {
        return -288961498 + EqualityComparer<NetworkConnection>.Default.GetHashCode(networkConnection);
    }

#pragma warning disable CS1591
    public static bool operator ==(UtpConnection left, UtpConnection right)
#pragma warning restore CS1591
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

#pragma warning disable CS1591
    public static bool operator !=(UtpConnection left, UtpConnection right) => !(left == right);
#pragma warning restore CS1591
}
