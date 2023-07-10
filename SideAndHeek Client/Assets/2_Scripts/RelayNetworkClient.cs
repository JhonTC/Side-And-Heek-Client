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

public class RelayNetworkClient
{
    public ushort Id;
    public NetworkDriver clientDriver;
    private JoinAllocation clientAllocation;
    private NetworkConnection clientConnection;
    private string playerLatestMessageReceived;

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
                    FixedString32Bytes msg = stream.ReadFixedString32();
                    Debug.Log($"Player received msg: {msg}");
                    playerLatestMessageReceived = msg.ToString();
                    break;

                // Handle Connect events.
                case NetworkEvent.Type.Connect:
                    Debug.Log("Player connected to the Host");
                    break;

                // Handle Disconnect events.
                case NetworkEvent.Type.Disconnect:
                    Debug.Log("Player got disconnected from the Host");
                    clientConnection = default(NetworkConnection);
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

    public void Disconnect()
    {
        // This sends a disconnect event to the Host client,
        // letting them know they're disconnecting.
        clientDriver.Disconnect(clientConnection);

        // We remove the reference to the current connection by overriding it.
        clientConnection = default(NetworkConnection);
    }
}
