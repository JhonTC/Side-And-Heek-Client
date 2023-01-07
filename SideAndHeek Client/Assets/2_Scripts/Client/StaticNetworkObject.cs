using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticNetworkObject : NetworkObject //todo: rename to ExistingNetworkObject/ConstantNetworkObject/AttachedNetworkObject....?
{
    void Start()
    {
        NetworkObjectsManager.instance.RegisterNetworkedObject(this);
    }
}
