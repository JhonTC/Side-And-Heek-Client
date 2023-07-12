using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ServerPlayerMotor : PlayerMotor
{
    [SerializeField] private FootCollisionHandler largeGroundColliderPrefab;

    [Header("Movement Controller")]
    public SimplePlayerController movementController;

    public override void Init(Player owner)
    {
        base.Init(owner);

        movementController.Init(this);
        movementController.largeGroundCollider = Instantiate(largeGroundColliderPrefab, transform);
        movementController.feetMidpoint = owner.feetMidpoint;
        movementController.SetupBodyCollisionHandlers(owner);
    }

    protected override void FixedUpdate()
    {
        if (isAcceptingInput)
        {
            if (otherInputs[0])
            {
                movementController.OnJump();
            }

            if (otherInputs[1])
            {
                movementController.OnFlopKey(true);
            }
            else
            {
                movementController.OnFlopKeyUp();
            }

            movementController.isSneaking = otherInputs[2];
            movementController.CustomFixedUpdate(inputSpeed);
            movementController.SetRotation(rotation);
        }

        ServerSend.PlayerState(owner, this, movementController);
        ServerSend.PlayerPositions(owner, movementController);
        ServerSend.PlayerRotations(owner, movementController);
    }

    public override bool GetCanKnockOutOthers() 
    { 
        return movementController.canKnockOutOthers; 
    }
    public override void SetCanKnockOutOthers(bool canKnockOutOthers) 
    {
        movementController.canKnockOutOthers = canKnockOutOthers; 
    }

    private void OnDestroy()
    {
        if (movementController != null)
        {
            if (movementController.largeGroundCollider != null)
            {
                Destroy(movementController.largeGroundCollider);
            }
        }
    }

    public override void OnCollisionWithOther(float duration)
    {
        movementController.OnCollisionWithOther(duration);
    }

    public override void SetForwardForceMultiplier(float multiplier)
    {
        movementController.forwardForceMultipler = multiplier;
    }

    public override SimplePlayerController GetMovementController()
    {
        return movementController;
    }
}
