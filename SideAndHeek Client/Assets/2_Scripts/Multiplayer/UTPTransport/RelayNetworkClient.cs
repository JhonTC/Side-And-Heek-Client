using Riptide;
using Riptide.Transports;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayNetworkClient : IRelayNetwork
{
    public event Action<byte[], int, NetworkConnection> DataReceived;
    public event Action Connected;
    public event Action<NetworkConnection, DisconnectReason> Disconnected;

    public NetworkDriver clientDriver;
    private JoinAllocation clientAllocation;
    public NetworkConnection clientConnection;

    public void Update()
    {
        // Skip update logic if the Player isn't yet bound.
        if (!clientDriver.IsCreated || !clientDriver.Bound)
        {
            return;
        }

        // This keeps the binding to the Relay server alive,
        // preventing it from timing out due to inactivity.
        clientDriver.ScheduleUpdate().Complete();

        // Resolve event queue.
        NetworkEvent.Type eventType;
        while ((eventType = clientConnection.PopEvent(clientDriver, out var stream)) != NetworkEvent.Type.Empty)
        {
            switch (eventType)
            {
                // Handle Relay events.
                case NetworkEvent.Type.Data:
                    NativeArray<byte> bytes = new NativeArray<byte>(stream.ReadInt(), Allocator.Persistent);
                    stream.ReadBytes(bytes);
                    DataReceived?.Invoke(bytes.ToArray(), bytes.Length, clientConnection);
                    bytes.Dispose();
                    break;

                // Handle Connect events.
                case NetworkEvent.Type.Connect:
                    Debug.Log("Player connected to the Host");
                    Connected?.Invoke();
                    break;

                // Handle Disconnect events.
                case NetworkEvent.Type.Disconnect:
                    Debug.Log("Player got disconnected from the Host");
                    clientConnection = default(NetworkConnection);
                    Disconnected?.Invoke(clientConnection, DisconnectReason.Kicked);
                    break;
            }
        }
    }

    public async void OnJoin(string joinCode)
    {
        // Input join code in the respective input field first.
        if (String.IsNullOrEmpty(joinCode))
        {
            Debug.LogError("Please input a join code.");
            return;
        }

        Debug.Log($"Player - Joining host allocation using join code: {joinCode}. Upon success, I have 10 seconds to BIND to the Relay server that I've allocated.");

        try
        {
            clientAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            Debug.Log("Player Allocation ID: " + clientAllocation.AllocationId);

            OnBindPlayer();
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError(ex.Message + "\n" + ex.StackTrace);
        }
    }

    public void OnBindPlayer()
    {
        Debug.Log("Player - Binding to the Relay server using UTP.");

        // Extract the Relay server data from the Join Allocation response.
        var relayServerData = new RelayServerData(clientAllocation, "udp");
        // Create NetworkSettings using the Relay server data.
        var settings = new NetworkSettings();
        settings.WithRelayParameters(ref relayServerData);

        // Create the Player's NetworkDriver from the NetworkSettings object.
        clientDriver = NetworkDriver.Create(settings);

        // Bind to the Relay server.
        if (clientDriver.Bind(NetworkEndPoint.AnyIpv4) != 0)
        {
            Debug.LogError("Player client failed to bind");
        }
        else
        {
            Debug.Log("Player client bound to Relay server");

            OnConnectPlayer();
        }
    }

    public void OnConnectPlayer()
    {
        Debug.Log("Player - Connecting to Host's client.");
        // Sends a connection request to the Host Player.
        clientConnection = clientDriver.Connect();
    }

    public void End()
    {
        // This sends a disconnect event to the Host client,
        // letting them know they're disconnecting.
        clientDriver.Disconnect(clientConnection);
        clientDriver.Dispose();

        // We remove the reference to the current connection by overriding it.
        clientConnection = default(NetworkConnection);
    }

    public void Send(NativeArray<byte> bytes, NetworkConnection connection)
    {
        if (clientConnection.IsCreated)
        {
            if (clientDriver.BeginSend(connection, out var writer) == 0)
            {
                writer.WriteInt(bytes.Length);
                writer.WriteBytes(bytes);
                clientDriver.EndSend(writer);
            }
        }
    }
}
