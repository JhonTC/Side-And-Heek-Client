using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public SuperFlop(PickupSO _pickupSO, Player _owner) : base(_pickupSO)
    {
        owner = _owner;
    }

    public override void PickupUsed()
    {
        SimplePlayerController movementController = owner.playerMotor.GetMovementController();
        movementController.flopForceMultiplier = pickupSO.power;
        movementController.OnFlopKey(false);

        owner.ItemUseComplete();
    }
}

public class SuperJump : BasePickup
{
    Player owner;

    public SuperJump(PickupSO _pickupSO, Player _owner) : base(_pickupSO)
    {
        owner = _owner;
    }

    public override void PickupUsed()
    {
        SimplePlayerController movementController = owner.playerMotor.GetMovementController();
        movementController.jumpForceMultiplier = pickupSO.power;
        movementController.OnJump();
        owner.ItemUseComplete();
    }
}

public class JellyBombItem : BasePickup
{
    Player owner;

    public JellyBombItem(PickupSO _pickupSO, Player _owner) : base(_pickupSO)
    {
        owner = _owner;
    }

    public override void PickupUsed()
    {
        NetworkObjectsManager.instance.itemHandler.SpawnItem(owner.Id, (int)pickupSO.pickupCode, owner.playerMotor.transform.position, owner.playerMotor.transform.rotation);
        NetworkObjectsManager.instance.PerformSecondsCountdown((int)pickupSO.duration, OnComplete);
    }
    public void OnComplete()
    {
        owner.ItemUseComplete();
    }
}

public class SuperSpeed : BasePickup
{
    Player owner;

    public SuperSpeed(PickupSO _pickupSO, Player _owner) : base(_pickupSO)
    {
        owner = _owner;
    }

    public override void PickupUsed()
    {
        SimplePlayerController movementController = owner.playerMotor.GetMovementController();
        movementController.forwardForceMultipler = pickupSO.power;

        NetworkObjectsManager.instance.PerformSecondsCountdown((int)pickupSO.duration, OnComplete);
    }

    public void OnComplete()
    {
        SimplePlayerController movementController = owner.playerMotor.GetMovementController();
        movementController.forwardForceMultipler = movementController.maxForwardForceMultipler;
        owner.ItemUseComplete();
    }
}

public class Invisibility : BasePickup
{
    Player owner;

    public Invisibility(PickupSO _pickupSO, Player _owner) : base(_pickupSO)
    {
        owner = _owner;
    }

    public override void PickupUsed()
    {
        ServerSend.SetPlayerMaterialType(owner.Id, MaterialType.Invisible);

        NetworkObjectsManager.instance.PerformSecondsCountdown((int)pickupSO.duration, OnComplete);
    }

    public void OnComplete()
    {
        ServerSend.SetPlayerMaterialType(owner.Id, MaterialType.Default);
        owner.ItemUseComplete();
    }
}

public class TeleportItem : BasePickup
{
    Player owner;

    public TeleportItem(PickupSO _pickupSO, Player _owner) : base(_pickupSO)
    {
        owner = _owner;
    }

    public override void PickupUsed()
    {
        //NetworkObjectsManager.instance.itemHandler.SpawnItem(owner.id, (int)pickupSO.pickupCode, owner.movementController.transform.position, owner.movementController.transform.rotation);
    }
}

public class Morph : BasePickup
{
    Player owner;
    Color playerOGColour;

    public Morph(PickupSO _pickupSO, Player _owner) : base(_pickupSO)
    {
        owner = _owner;
    }

    public override void PickupUsed()
    {
        playerOGColour = owner.activeColour;

        ServerSend.SetPlayerColour(owner.Id, GameManager.instance.hunterColour, false, true);

        NetworkObjectsManager.instance.PerformSecondsCountdown((int)pickupSO.duration, OnComplete);
    }

    public void OnComplete()
    {
        ServerSend.SetPlayerColour(owner.Id, playerOGColour, false, true);
        owner.ItemUseComplete();
    }
}

public class IceballItem : BasePickup
{
    Player owner;

    public IceballItem(PickupSO _pickupSO, Player _owner) : base(_pickupSO)
    {
        owner = _owner;
    }

    public override void PickupUsed()
    {
        NetworkObjectsManager.instance.itemHandler.SpawnItem(owner.Id, (int)pickupSO.pickupCode, owner.playerMotor.transform.position, owner.playerMotor.transform.rotation);
        NetworkObjectsManager.instance.PerformSecondsCountdown((int)pickupSO.duration, OnComplete);
    }

    public void OnComplete()
    {
        owner.ItemUseComplete();
    }
}