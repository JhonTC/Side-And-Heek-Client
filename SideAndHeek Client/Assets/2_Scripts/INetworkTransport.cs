using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public interface INetworkTransport
{
    public void Update();

    public void SetupClient();

    public void StartServer();

    public void ConnectClient(string joinValue);

    public void StopServer();

    public void DisconnectClient();

    public bool IsLocalPlayer(ushort id);

    public void OnApplicationQuit();
}
