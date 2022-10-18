using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Server
{
    [System.Serializable]
    public class BasePickup
    {
        public PickupSO pickupSO;

        public BasePickup(PickupSO _pickupSO)
        {
            pickupSO = _pickupSO;
        }

        public virtual void PickupUsed() { }
    }

    public class SuperFlop : BasePickup
    {
        Player owner;

        public SuperFlop(PickupSO _pickupSO) : base(_pickupSO) {}
        public SuperFlop(PickupSO _pickupSO, Player _owner) : base(_pickupSO)
        {
            owner = _owner;
        }

        public override void PickupUsed()
        {
            //owner.movementController.flopForceMultiplier = pickupSO.power;
            //owner.movementController.OnFlop();
        }
    }

    public class SuperJump : BasePickup
    {
        Player owner;

        public SuperJump(PickupSO _pickupSO) : base(_pickupSO) { }
        public SuperJump(PickupSO _pickupSO, Player _owner) : base(_pickupSO)
        {
            owner = _owner;
        }

        public override void PickupUsed()
        {
            //owner.movementController.jumpForceMultiplier = pickupSO.power;
            //owner.movementController.OnJump();
        }
    }

    public class JellyBombItem : BasePickup
    {
        Player owner;

        public JellyBombItem(PickupSO _pickupSO) : base(_pickupSO) { }
        public JellyBombItem(PickupSO _pickupSO, Player _owner) : base(_pickupSO)
        {
            owner = _owner;
        }

        public override void PickupUsed()
        {
            //todo (only neeed for singleplayer mode)
        }
    }
    public class SuperSpeed : BasePickup
    {
        Player owner;
        float intialForwardForceMultipler;

        public SuperSpeed(PickupSO _pickupSO) : base(_pickupSO) { }
        public SuperSpeed(PickupSO _pickupSO, Player _owner) : base(_pickupSO)
        {
            owner = _owner;
        }

        public override void PickupUsed()
        {
            //todo (only neeed for singleplayer mode)

            //intialForwardForceMultipler = owner.movementController.forwardForceMultipler;
            //owner.movementController.forwardForceMultipler = pickupSO.power;

            //NetworkObjectsManager.instance.PerformSecondsCountdown((int)pickupSO.duration, OnComplete);
        }

        public void OnComplete()
        {
            //todo (only neeed for singleplayer mode)

            //owner.movementController.forwardForceMultipler = intialForwardForceMultipler;
        }
    }

    public class Invisibility : BasePickup
    {
        Player owner;

        public Invisibility(PickupSO _pickupSO) : base(_pickupSO) { }
        public Invisibility(PickupSO _pickupSO, Player _owner) : base(_pickupSO)
        {
            owner = _owner;
        }

        public override void PickupUsed()
        {
            //todo (only neeed for singleplayer mode)

            //ServerSend.SetPlayerColour(owner.id, new Color(1, 1, 1, 0), false);

            //NetworkObjectsManager.instance.PerformSecondsCountdown((int)pickupSO.duration, OnComplete);
        }

        public void OnComplete()
        {
            //todo (only neeed for singleplayer mode)

            //ServerSend.SetPlayerColour(owner.id, owner.activeColour, false);
        }
    }

    public class TeleportItem : BasePickup
    {
        Player owner;

        public TeleportItem(PickupSO _pickupSO) : base(_pickupSO) { }
        public TeleportItem(PickupSO _pickupSO, Player _owner) : base(_pickupSO)
        {
            owner = _owner;
        }

        public override void PickupUsed()
        {
            //todo (only neeed for singleplayer mode)

            //NetworkObjectsManager.instance.itemHandler.SpawnItem(owner.id, (int)pickupSO.pickupCode, owner.movementController.transform.position, owner.movementController.transform.rotation);
        }
    }

    public class Morph : BasePickup
    {
        Player owner;

        public Morph(PickupSO _pickupSO) : base(_pickupSO) { }
        public Morph(PickupSO _pickupSO, Player _owner) : base(_pickupSO)
        {
            owner = _owner;
        }

        public override void PickupUsed()
        {
            //todo (only neeed for singleplayer mode)

            //ServerSend.SetPlayerColour(owner.id, new Color(1, 1, 1, 0), false);

            //NetworkObjectsManager.instance.PerformSecondsCountdown((int)pickupSO.duration, OnComplete);
        }

        public void OnComplete()
        {
            //todo (only neeed for singleplayer mode)

            //ServerSend.SetPlayerColour(owner.id, owner.activeColour, false);
        }
    }
}