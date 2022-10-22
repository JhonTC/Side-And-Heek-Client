using Server;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler
{
    public static Dictionary<ushort, SpawnableObject> items = new Dictionary<ushort, SpawnableObject>();

    public static Dictionary<PickupCode, int> itemLog = new Dictionary<PickupCode, int>();

    private delegate BasePickup ItemHandlerDelegate(PickupSO _pickupSO, Player _player);

    private static Dictionary<PickupCode, ItemHandlerDelegate> itemHandlers;

    public ItemHandler()
    {
        InitialiseItemData();
    }

    private void InitialiseItemData()
    {
        itemHandlers = new Dictionary<PickupCode, ItemHandlerDelegate>()
        {
            { PickupCode.NULL,  NullItem },
            { PickupCode.SuperFlop, SuperFlop },
            { PickupCode.SuperJump, SuperJump },
            { PickupCode.JellyBomb, JellyBomb },
            { PickupCode.SuperSpeed_3, SuperSpeed },
            { PickupCode.SuperSpeed_6, SuperSpeed },
            { PickupCode.SuperSpeed_9, SuperSpeed },
            { PickupCode.Invisibility, Invisibility },
            { PickupCode.Teleport, Teleport },
            { PickupCode.Morph, Morph },
            { PickupCode.BearTrap, BearTrap }
        };
        Debug.Log("Initialised packets.");
    }

    public SpawnableObject SpawnItem(ushort itemId, ushort _creatorId, int _code, Vector3 _position, Quaternion _rotation, PickupSpawner _spawner = null)
    {
        SpawnableObject item = NetworkObjectsManager.instance.SpawnItem((PickupCode)_code, _position, _rotation);
        item.Init(itemId, _creatorId, _code);
        items.Add(item.objectId, item);

        return item;
    }

    public bool CanPickupCodeBeUsed(PickupDetails pickupDetails)
    {
        if (itemLog.ContainsKey(pickupDetails.pickupSO.pickupCode))
        {
            if (itemLog[pickupDetails.pickupSO.pickupCode] >= pickupDetails.numberOfUses)
            {
                return false;
            }
        }

        return true;
    }

    public bool HaveAllPickupsBeenSpawned()
    {
        if (itemLog.Count < GameManager.instance.collection.pickupDetails.Count)
        {
            return false;
        }

        foreach (PickupCode pickupCode in itemLog.Keys)
        {
            if (itemLog.ContainsKey(pickupCode))
            {
                if (itemLog[pickupCode] < GameManager.instance.collection.GetPickupByCode(pickupCode).numberOfUses)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }


    public BasePickup HandleItem(PickupSO _pickupSO, Player _player)
    {
        BasePickup ret = itemHandlers[_pickupSO.pickupCode](_pickupSO, _player);
        return ret;
    }

    private BasePickup NullItem(PickupSO _pickupSO, Player _player)
    {
        return null;
    }
    private BasePickup SuperFlop(PickupSO _pickupSO, Player _player)
    {
        if (_player)
        {
            return new SuperFlop(_pickupSO, _player);
        }
        else
        {
            return new SuperFlop(_pickupSO);
        }
    }
    private BasePickup SuperJump(PickupSO _pickupSO, Player _player)
    {
        if (_player)
        {
            return new SuperJump(_pickupSO, _player);
        }
        else
        {
            return new SuperJump(_pickupSO);
        }
    }

    private BasePickup JellyBomb(PickupSO _pickupSO, Player _player)
    {
        if (_player)
        {
            return new JellyBombItem(_pickupSO, _player);
        }
        else
        {
            return new JellyBombItem(_pickupSO);
        }
    }
    private BasePickup SuperSpeed(PickupSO _pickupSO, Player _player)
    {
        if (_player)
        {
            return new SuperSpeed(_pickupSO, _player);
        }
        else
        {
            return new SuperSpeed(_pickupSO);
        }
    }
    private BasePickup Invisibility(PickupSO _pickupSO, Player _player)
    {
        if (_player)
        {
            return new Invisibility(_pickupSO, _player);
        }
        else
        {
            return new Invisibility(_pickupSO);
        }
    }
    private BasePickup Teleport(PickupSO _pickupSO, Player _player)
    {
        if (_player)
        {
            return new TeleportItem(_pickupSO, _player);
        }
        else
        {
            return new TeleportItem(_pickupSO);
        }
    }
    private BasePickup Morph(PickupSO _pickupSO, Player _player)
    {
        if (_player)
        {
            return new Morph(_pickupSO, _player);
        }
        else
        {
            return new Morph(_pickupSO);
        }
    }
    private BasePickup BearTrap(PickupSO _pickupSO, Player _player)
    {
        if (_player)
        {
            return new BearTrapItem(_pickupSO, _player);
        }
        else
        {
            return new BearTrapItem(_pickupSO);
        }
    }
}
