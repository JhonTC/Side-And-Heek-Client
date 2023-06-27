using Server;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler //todo:check how much of this script is being used...
{
    public static Dictionary<PickupType, int> itemLog = new Dictionary<PickupType, int>();

    private delegate BasePickup ItemHandlerDelegate(PickupSO _pickupSO, Player _player);

    private static Dictionary<PickupType, ItemHandlerDelegate> itemHandlers;

    public ItemHandler()
    {
        InitialiseItemData();
    }

    private void InitialiseItemData()
    {
        itemHandlers = new Dictionary<PickupType, ItemHandlerDelegate>()
        {
            { PickupType.NULL,  NullItem },
            { PickupType.SuperFlop, SuperFlop },
            { PickupType.SuperJump, SuperJump },
            { PickupType.JellyBomb, JellyBomb },
            { PickupType.SuperSpeed_3, SuperSpeed },
            { PickupType.SuperSpeed_6, SuperSpeed },
            { PickupType.SuperSpeed_9, SuperSpeed },
            { PickupType.Invisibility, Invisibility },
            { PickupType.Teleport, Teleport },
            { PickupType.Morph, Morph },
            { PickupType.Iceball, Iceball },
        };
    }

    public SpawnableObject SpawnItem(ushort itemId, ushort _creatorId, int _code, Vector3 _position, Quaternion _rotation, PickupSpawner _spawner = null)
    {
        SpawnableObject item = NetworkObjectsManager.instance.SpawnObject(itemId, NetworkedObjectType.Item, _position, _rotation, (PickupType)_code) as SpawnableObject;
        if (item != null)
        {
            item.Init(_creatorId, _code);
        }

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

        foreach (PickupType pickupCode in itemLog.Keys)
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

    private BasePickup Iceball(PickupSO _pickupSO, Player _player)
    {
        if (_player)
        {
            return new IceballItem(_pickupSO, _player);
        }
        else
        {
            return new IceballItem(_pickupSO);
        }
    }
}
