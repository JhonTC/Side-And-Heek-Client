using Riptide;
using Riptide.Transports;
using System;
using System.Linq;
using System.Net;
using Unity.Networking.Transport;
using UnityEngine;

public class UtpClient : UtpPeer, IClient
{
    /// <inheritdoc/>
    public event EventHandler Connected;
    /// <inheritdoc/>
    public event EventHandler ConnectionFailed;
    /// <inheritdoc/>
    public event EventHandler<DataReceivedEventArgs> DataReceived;

    /// <summary>The connection to the server.</summary>
    private UtpConnection utpConnection;

    /// <inheritdoc/>
    public UtpClient() : base()
    {
        RelayNetwork = new RelayNetworkClient();
    }

    /// <inheritdoc/>
    /// <remarks>Expects the host address to consist of an IP and port, separated by a colon. For example: <c>127.0.0.1:7777</c>.</remarks>
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

        /*if ((mode == SocketMode.IPv4Only && ip.AddressFamily == AddressFamily.InterNetworkV6) || (mode == SocketMode.IPv6Only && ip.AddressFamily == AddressFamily.InterNetwork))
        {
            // The IP address isn't in an acceptable format for the current socket mode
            if (mode == SocketMode.IPv4Only)
                connectError = "Connecting to IPv6 addresses is not allowed when running in IPv4 only mode!";
            else
                connectError = "Connecting to IPv4 addresses is not allowed when running in IPv6 only mode!";

            connection = null;
            return false;
        }

        OpenSocket();*/

        isRunning = true;
        client.OnJoin(joinCode);

        connection = utpConnection = new UtpConnection(client.clientConnection, this);
        return true;
    }

    /// <summary>Parses <paramref name="hostAddress"/> into <paramref name="ip"/> and <paramref name="port"/>, if possible.</summary>
    /// <param name="hostAddress">The host address to parse.</param>
    /// <param name="ip">The retrieved IP.</param>
    /// <param name="port">The retrieved port.</param>
    /// <returns>Whether or not <paramref name="hostAddress"/> was in a valid format.</returns>
    private bool ParseHostAddress(string hostAddress, out IPAddress ip, out ushort port)
    {
        string[] ipAndPort = hostAddress.Split(':');
        string ipString = "";
        string portString = "";
        if (ipAndPort.Length > 2)
        {
            // There was more than one ':' in the host address, might be IPv6
            ipString = string.Join(":", ipAndPort.Take(ipAndPort.Length - 1));
            portString = ipAndPort[ipAndPort.Length - 1];
        }
        else if (ipAndPort.Length == 2)
        {
            // IPv4
            ipString = ipAndPort[0];
            portString = ipAndPort[1];
        }

        port = 0; // Need to make sure a value is assigned in case IP parsing fails
        return IPAddress.TryParse(ipString, out ip) && ushort.TryParse(portString, out port);
    }

    /// <inheritdoc/>
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

    /// <summary>Invokes the <see cref="Connected"/> event.</summary>
    protected virtual void OnConnected()
    {
        RelayNetworkClient client = RelayNetwork as RelayNetworkClient;
        if (client != null)
        {
            utpConnection.SetNetworkConnection(client.clientConnection);
        }
        Connected?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Invokes the <see cref="ConnectionFailed"/> event.</summary>
    protected virtual void OnConnectionFailed()
    {
        ConnectionFailed?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    protected override void OnDataReceived(byte[] dataBuffer, int amount, NetworkConnection networkConnection)
    {
        Debug.Log($"Recieve MessageHeader: {(MessageHeader)dataBuffer[0]}");

        Debug.Log($"Do connections match: {utpConnection.networkConnection.Equals(networkConnection)}");
        Debug.Log(utpConnection.networkConnection.InternalId); Debug.Log(networkConnection.InternalId);
        if (utpConnection.networkConnection.Equals(networkConnection))
            DataReceived?.Invoke(this, new DataReceivedEventArgs(dataBuffer, amount, utpConnection));
    }
}
