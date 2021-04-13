using Server;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickupSpawner : MonoBehaviour
{
    public int spawnerId;
    public bool hasPickup;

    protected Pickup activePickup;

    public void Init(int _spawnerId)
    {
        spawnerId = _spawnerId;
    }

    public void PickupSpawned(int _pickupId, int _creatorId, int _code, Vector3 _position, Quaternion _rotation)
    {
        hasPickup = true;

        if (hasPickup)
        {
            activePickup = NetworkObjectsManager.instance.pickupHandler.SpawnPickup(_pickupId, _creatorId, _code, _position, _rotation, this);
        }
    }

    public void PickupPickedUp()
    {
        hasPickup = false;
        activePickup = null;
    }
}
