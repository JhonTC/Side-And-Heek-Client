using Unity.Collections;
using Unity.Networking.Transport;

public interface IRelayNetwork
{
    public void Update();

    public void End();

    public void Send(NativeArray<byte> bytes, NetworkConnection connection);
}
