using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableObject : NetworkObject
{
    public ushort creatorId;

    public PickupDetails activeObjectDetails;

    public void Init(ushort _creatorId, int _code)
    {
        creatorId = _creatorId;

        activeObjectDetails = GameManager.instance.collection.GetPickupByCode((PickupType)_code);
    }
}
