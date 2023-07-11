using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public interface IRelayNetwork
{
    public void Update();

    public void End();

    public void Send(NativeArray<byte> bytes, NetworkConnection connection);
}
