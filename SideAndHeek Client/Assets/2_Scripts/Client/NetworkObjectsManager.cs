using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkObjectsManager : MonoBehaviour
{
    public static NetworkObjectsManager instance;

    [SerializeField] private Pickup pickupPrefab;
    [SerializeField] private List<ItemDetails> itemDetails;

    [Serializable]
    public struct ItemDetails
    {
        public SpawnableObject itemPrefab;
        public PickupCode pickupCode;
    }

    [HideInInspector] public PickupHandler pickupHandler;
    [HideInInspector] public ItemHandler itemHandler;

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

        pickupHandler = new PickupHandler();
        itemHandler = new ItemHandler();
    }

    public Pickup SpawnPickup(Vector3 _position, Quaternion _rotation)
    {
        return Instantiate(pickupPrefab, _position, _rotation, transform);
    }
    public SpawnableObject SpawnItem(PickupCode _pickupCode, Vector3 _position, Quaternion _rotation)
    {
        SpawnableObject spawnableObject = GetSpawnableObjectForPickupCode(_pickupCode);
        if (spawnableObject != null)
        {
            return Instantiate(spawnableObject, _position, _rotation, transform);
        }

        throw new Exception($"ERROR: No spawnable Object with code {_pickupCode}");
    }

    private SpawnableObject GetSpawnableObjectForPickupCode(PickupCode _pickupCode)
    {
        foreach (ItemDetails itemDetails in itemDetails)
        {
            if (itemDetails.pickupCode == _pickupCode)
            {
                return itemDetails.itemPrefab;
            }
        }

        return null;
    }
}
