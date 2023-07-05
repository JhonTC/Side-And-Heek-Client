using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClientServerPlayerMotor : PlayerMotor
{
    [SerializeField] private FootCollisionHandler largeGroundColliderPrefab;

    private CameraMode cameraMode;
    private Vector2 inputMovement = Vector2.zero;

    [Header("Movement Controller")]
    public SimplePlayerController movementController;


    public override void Init(Player owner)
    {
        base.Init(owner);

        cameraMode = owner.cameraMode;
        SetupCameraMode();

        movementController.Init(this);
        movementController.largeGroundCollider = Instantiate(largeGroundColliderPrefab, transform);
        movementController.feetMidpoint = owner.feetMidpoint;
        movementController.SetupBodyCollisionHandlers(owner);
    }

    public override void OnMove(InputAction.CallbackContext value)
    {
        inputMovement = value.ReadValue<Vector2>();
    }

    public override void OnJump(InputAction.CallbackContext value)
    {
        bool jump = value.ReadValueAsButton();
        if (isJumping != jump)
        {
            isJumping = jump;
        }
    }

    public override void OnFlop(InputAction.CallbackContext value)
    {
        bool flop = value.ReadValueAsButton();
        if (isFlopping != flop)
        {
            isFlopping = flop;
        }
    }

    private void SetupCameraMode()
    {
        switch (cameraMode)
        {
            case CameraMode.ThirdPerson:
                //owner.firstPersonCamera.enabled = false;
                owner.thirdPersonCamera.enabled = true;
                break;
            case CameraMode.FirstPerson:
                //owner.firstPersonCamera.enabled = true;
                owner.thirdPersonCamera.enabled = false;
                break;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (cameraMode != owner.cameraMode)
        {
            cameraMode = owner.cameraMode;
            SetupCameraMode();
        }

        inputSpeed = 1;
        float horizontal = inputMovement.x;
        float vertical = inputMovement.y;

        if (cameraMode == CameraMode.ThirdPerson)
        {
            inputSpeed = Mathf.Clamp(Mathf.Abs(horizontal) + Mathf.Abs(vertical), 0, 1);

            float angle;
            if (horizontal != 0 || vertical != 0)
            {
                angle = Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg;
                rotation = Quaternion.AngleAxis(angle, Vector3.up);
            }
            else
            {
                rotation = root.rotation;
            }
        }

        if (isAcceptingInput)
        {
            if (isJumping)
            {
                movementController.OnJump();
            }

            if (isFlopping)
            {
                movementController.OnFlopKey(true);
            }
            else
            {
                movementController.OnFlopKeyUp();
            }

            movementController.isSneaking = isSneaking;
            movementController.CustomFixedUpdate(inputSpeed);
            movementController.SetRotation(rotation);
        }

        owner.HandlePlayerState(inputSpeed, movementController);

        //these messages are being sent to all, except the P2P client as it isnt registered as an actual client
        ServerSend.PlayerPositions(owner, movementController);
        ServerSend.PlayerRotations(owner, movementController);
        ServerSend.PlayerState(owner, this, movementController);
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
