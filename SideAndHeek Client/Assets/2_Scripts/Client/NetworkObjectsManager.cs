using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum NetworkedObjectType
{
    Static,
    Pickup,
    Item
}

public class NetworkObjectsManager : MonoBehaviour
{
    public static NetworkObjectsManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);

        pickupHandler = new PickupHandler();//move to GameManager?
        itemHandler = new ItemHandler();//move to GameManager?
    }

    [Serializable]
    public struct ItemDetails
    {
        public SpawnableObject itemPrefab;
        public PickupType pickupType;
    }

    public static Dictionary<ushort, NetworkObject> networkObjects = new Dictionary<ushort, NetworkObject>();

    [SerializeField] private Pickup pickupPrefab;
    [SerializeField] private List<ItemDetails> itemDetails;

    [HideInInspector] public PickupHandler pickupHandler;
    [HideInInspector] public ItemHandler itemHandler;

    public NetworkObject SpawnObject(ushort objectId, NetworkedObjectType networkObjectType, Vector3 position, Quaternion rotation, PickupType pickupType = PickupType.NULL)
    {
        NetworkObject prefab = GetPrefabForType(networkObjectType, pickupType);
        if (prefab == null) return null;

        NetworkObject newObject = Instantiate(prefab, position, rotation);
        newObject.Init(objectId, networkObjectType);

        RegisterNetworkedObject(newObject);

        return newObject;
    }

    public void RegisterNetworkedObject(NetworkObject networkObject)
    {
        if (!networkObjects.ContainsKey(networkObject.objectId))
        {
            networkObjects.Add(networkObject.objectId, networkObject);
        }
        else
        {
            Debug.LogWarning($"NetworkObject with id: {networkObject.objectId} already exists");
        }
    }

    public void UnregisterNetworkedObject(NetworkObject networkObject)
    {
        if (networkObjects.ContainsKey(networkObject.objectId))
        {
            networkObjects.Remove(networkObject.objectId);
        }
        else
        {
            Debug.LogWarning($"NetworkObject with id: {networkObject.objectId} doesn't exist");
        }
    }

    public void ClearAllSpawnedNetworkObjects()
    {
        var networkObjectsToDelete = networkObjects.Where(T => T.Value.networkedObjectType != NetworkedObjectType.Static).ToArray();

        foreach (var networkObject in networkObjectsToDelete)
        {
            if (networkObject.Value.networkedObjectType != NetworkedObjectType.Static)
            {
                DestoryObject(networkObject.Value);
            }
        }
    }
    public void ClearAllNetworkObjects()
    {
        ClearAllSpawnedNetworkObjects();

        networkObjects.Clear(); //only called from disconnect method so need to clear after deletion
    }

    private NetworkObject GetPrefabForType(NetworkedObjectType networkObjectType, PickupType pickupType = PickupType.NULL)
    {
        switch (networkObjectType)
        {
            case NetworkedObjectType.Pickup:
                return pickupPrefab;
            case NetworkedObjectType.Item:
                return GetSpawnableObjectForPickupCode(pickupType);
        }

        return null;
    }

    private SpawnableObject GetSpawnableObjectForPickupCode(PickupType _pickupCode)
    {
        foreach (ItemDetails itemDetails in itemDetails)
        {
            if (itemDetails.pickupType == _pickupCode)
            {
                return itemDetails.itemPrefab;
            }
        }

        throw new Exception($"ERROR: No spawnable Object with code {_pickupCode}");
    }

    public void DestoryObject(NetworkObject networkObject)
    {
        if (networkObjects.ContainsKey(networkObject.objectId))
        {
            Destroy(networkObject.gameObject);
        }

        UnregisterNetworkedObject(networkObject);
    }
}
