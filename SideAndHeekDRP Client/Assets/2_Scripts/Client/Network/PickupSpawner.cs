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

    public PickupType pickupType;

    protected Pickup activePickup;

    public void Init(int _spawnerId, PickupType _pickupType)
    {
        spawnerId = _spawnerId;
        pickupType = _pickupType;
    }

    public void PickupSpawned(int _pickupId, PickupType _pickupType, int _code, Vector3 _position, Quaternion _rotation)
    {
        hasPickup = true;

        if (hasPickup)
        {
            activePickup = PickupManager.instance.SpawnPickup(_pickupId, _pickupType, _code, _position, _rotation, this);
        }
    }

    public void PickupPickedUp()
    {
        hasPickup = false;
        activePickup = null;
    }
}
