using Server;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickupSpawner : MonoBehaviour
{
    public static Dictionary<int, PickupSpawner> pickupSpawners = new Dictionary<int, PickupSpawner>();
    public static void CreatePickupSpawner(ushort _spawnerId, Vector3 _position)
    {
        PickupSpawner _spawner = Instantiate(GameManager.instance.pickupSpawnerPrefab, _position, Quaternion.identity);
        _spawner.Init(_spawnerId);

        pickupSpawners.Add(_spawnerId, _spawner);
    }
    public static void DestroyPickupSpawners()
    {
        foreach (PickupSpawner spawner in PickupSpawner.pickupSpawners.Values)
        {
            if (spawner != null)
            {
                Destroy(spawner.gameObject);
            }
        }
        pickupSpawners.Clear();
    }

    public ushort spawnerId;
    public bool hasPickup;

    protected Pickup activePickup;

    public void Init(ushort _spawnerId)
    {
        spawnerId = _spawnerId;
    }

    public void PickupSpawned(ushort _pickupId, ushort _creatorId, int _code, Vector3 _position, Quaternion _rotation)
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
