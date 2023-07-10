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
using UnityEngine.Assertions;

public class RelayNetworkHost
{
    public event Action<byte[], int, NetworkConnection> DataReceived;

    public NetworkDriver hostDriver;
    private Allocation hostAllocation;
    private NativeList<NetworkConnection> serverConnections;

    public void Update()
    {
        // Skip update logic if the Host isn't yet bound.
        if (!hostDriver.IsCreated || !hostDriver.Bound)
        {
            return;
        }

        // This keeps the binding to the Relay server alive,
        // preventing it from timing out due to inactivity.
        hostDriver.ScheduleUpdate().Complete();

        // Clean up stale connections.
        for (int i = 0; i < serverConnections.Length; i++)
        {
            if (!serverConnections[i].IsCreated)
            {
                Debug.Log("Stale connection removed");
                serverConnections.RemoveAt(i);
                --i;
            }
        }

        // Accept incoming client connections.
        NetworkConnection incomingConnection;
        while ((incomingConnection = hostDriver.Accept()) != default(NetworkConnection))
        {
            // Adds the requesting Player to the serverConnections list.
            // This also sends a Connect event back the requesting Player,
            // as a means of acknowledging acceptance.
            Debug.Log("Accepted an incoming connection.");
            serverConnections.Add(incomingConnection);

            byte[] simulatedByteArray = new byte[1] { (byte)MessageHeader.Connect };
            DataReceived?.Invoke(simulatedByteArray, simulatedByteArray.Length, incomingConnection);
        }

        // Process events from all connections.
        for (int i = 0; i < serverConnections.Length; i++)
        {
            Assert.IsTrue(serverConnections[i].IsCreated);

            // Resolve event queue.
            NetworkEvent.Type eventType;
            while ((eventType = hostDriver.PopEventForConnection(serverConnections[i], out var stream)) != NetworkEvent.Type.Empty)
            {
                switch (eventType)
                {
                    // Handle Relay events.
                    case NetworkEvent.Type.Data:
                        NativeArray<byte> bytes = new NativeArray<byte>(stream.ReadInt(), Allocator.Persistent);
                        stream.ReadBytes(bytes);
                        DataReceived?.Invoke(bytes.ToArray(), bytes.Length, serverConnections[i]);
                        bytes.Dispose();
                        break;

                    // Handle Disconnect events.
                    case NetworkEvent.Type.Disconnect:
                        Debug.Log("Server received disconnect from client");
                        serverConnections[i] = default(NetworkConnection);
                        break;
                }
            }
        }
    }

    public async void OnAllocate()
    {
        Debug.Log("Host - Creating an allocation. Upon success, I have 10 seconds to BIND to the Relay server that I've allocated.");

        // Determine region to use (user-selected or auto-select/QoS)
        //string region = GetRegionOrQosDefault();
        //Debug.Log($"The chosen region is: {region ?? autoSelectRegionName}");

        // Set max connections. Can be up to 100, but note the more players connected, the higher the bandwidth/latency impact.
        int maxConnections = 9;

        // Important: After the allocation is created, you have ten seconds to BIND, else the allocation times out.
        try
        {
            hostAllocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            Debug.Log($"Host Allocation ID: {hostAllocation.AllocationId}, region: {hostAllocation.Region}");

            // Initialize NetworkConnection list for the server (Host).
            // This list object manages the NetworkConnections which represent connected players.
            serverConnections = new NativeList<NetworkConnection>(maxConnections, Allocator.Persistent);

            OnRecieveJoinCode();
            OnBindHost();
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError(ex.Message + "\n" + ex.StackTrace);
        }
    }

    public async void OnRecieveJoinCode()
    {
        try
        {
            string joinCode = await Relay.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);
            Debug.Log($"Recieved Join Code: {joinCode}");

            UIManager.instance.gameplayPanel.roomCodeText.text = joinCode;
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError(ex.Message + "\n" + ex.StackTrace);
        }
    }

    public void OnBindHost()
    {
        Debug.Log("Host - Binding to the Relay server using UTP.");

        // Extract the Relay server data from the Allocation response.
        var relayServerData = new RelayServerData(hostAllocation, "udp");

        // Create NetworkSettings using the Relay server data.
        var settings = new NetworkSettings();
        settings.WithRelayParameters(ref relayServerData);

        // Create the Host's NetworkDriver from the NetworkSettings.
        hostDriver = NetworkDriver.Create(settings);

        // Bind to the Relay server.
        if (hostDriver.Bind(NetworkEndPoint.AnyIpv4) != 0)
        {
            Debug.LogError("Host client failed to bind");
        }
        else
        {
            if (hostDriver.Listen() != 0)
            {
                Debug.LogError("Host client failed to listen");
            }
            else
            {
                Debug.Log("Host client bound to Relay server");
            }
        }
    }

    public void Stop()
    {
        // Simply disconnect all connected clients.
        for (int i = 0; i < serverConnections.Length; i++)
        {
            // This sends a disconnect event to the destination client,
            // letting them know they're disconnected from the Host.
            hostDriver.Disconnect(serverConnections[i]);

            // Here, we set the destination client's NetworkConnection to the default value.
            // It will be recognized in the Host's Update loop as a stale connection, and be removed.
            serverConnections[i] = default(NetworkConnection);
        }

        serverConnections.Dispose();
        hostDriver.Dispose();
    }

    public void Send(NativeArray<byte> bytes, NetworkConnection connection)
    {
        if (serverConnections.Length != 0)
        {
            if (hostDriver.BeginSend(connection, out var writer) == 0)
            {
                writer.WriteInt(bytes.Length);
                writer.WriteBytes(bytes);
                hostDriver.EndSend(writer);
            }
        }
    }
}
