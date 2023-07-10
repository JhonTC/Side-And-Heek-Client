using Riptide;
using Riptide.Transports;
using Riptide.Transports.Udp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
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
    public UtpClient(int socketBufferSize = DefaultSocketBufferSize) : base(socketBufferSize) { }

    /// <inheritdoc/>
    /// <remarks>Expects the host address to consist of an IP and port, separated by a colon. For example: <c>127.0.0.1:7777</c>.</remarks>
    public bool Connect(string joinCode, out Connection connection, out string connectError)
    {
        Client.Connected += OnConnected;
        Client.DataReceived += OnDataReceived;
        Client.Disconnected += OnDisconnected;

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

        Client.OnJoin(joinCode);

        connection = utpConnection = new UtpConnection(Client.clientConnection, this);
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
        Client.Disconnect();

        Client.Connected -= OnConnected;
        Client.DataReceived -= OnDataReceived;
    }

    /// <summary>Invokes the <see cref="Connected"/> event.</summary>
    protected virtual void OnConnected()
    {
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
        if (utpConnection.networkConnection.Equals(networkConnection))
            DataReceived?.Invoke(this, new DataReceivedEventArgs(dataBuffer, amount, utpConnection));
    }
}
