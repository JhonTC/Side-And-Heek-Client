using Server;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHandler
{
    public static Dictionary<PickupType, int> pickupLog = new Dictionary<PickupType, int>();

    private delegate BasePickup PickupHandlerDelegate(PickupSO _item, Player _player);

    private static Dictionary<PickupType, PickupHandlerDelegate> pickupHandlers;

    public PickupHandler()
    {
        InitialisePickupData();
    }

    public Pickup SpawnPickup(ushort _pickupId, ushort _creatorId, int _code, Vector3 _position, Quaternion _rotation, PickupSpawner _spawner = null)
    {
        Pickup pickup = NetworkObjectsManager.instance.SpawnObject(_pickupId, NetworkedObjectType.Pickup, _position, _rotation) as Pickup;
        if (pickup != null)
        {
            pickup.Init(_spawner, _creatorId, _code);
        }

        return pickup;
    }

    public static bool CanPickupCodeBeUsed(PickupDetails itemDetails)
    {
        if (pickupLog.ContainsKey(itemDetails.pickupSO.pickupCode))
        {
            if (pickupLog[itemDetails.pickupSO.pickupCode] >= itemDetails.numberOfUses)
            {
                return false;
            }
        }

        return true;
    }

    public static bool HaveAllPickupsBeenSpawned()
    {
        if (pickupLog.Count < GameManager.instance.collection.pickupDetails.Count)
        {
            return false;
        }

        foreach (PickupType itemCode in pickupLog.Keys)
        {
            if (pickupLog.ContainsKey(itemCode))
            {
                if (pickupLog[itemCode] < GameManager.instance.collection.GetPickupByCode(itemCode).numberOfUses)
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

    private void InitialisePickupData()
    {
        pickupHandlers = new Dictionary<PickupType, PickupHandlerDelegate>()
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
            { PickupType.Morph, Morph }
        };
    }

    public BasePickup HandlePickup(PickupSO _pickupSO)
    {
        BasePickup ret = pickupHandlers[_pickupSO.pickupCode](_pickupSO, null);
        return ret;
    }
    public BasePickup HandlePickup(PickupSO _pickupSO, Player _player)
    {
        BasePickup ret = pickupHandlers[_pickupSO.pickupCode](_pickupSO, _player);
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
}
