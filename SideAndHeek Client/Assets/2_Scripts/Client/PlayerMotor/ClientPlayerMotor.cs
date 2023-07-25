using UnityEngine;
using UnityEngine.InputSystem;

public class ClientPlayerMotor : PlayerMotor
{
    public float turnSpeed;

    private CameraMode cameraMode;

    private Vector2 inputMovement = Vector2.zero;

    [SerializeField] private FootCollisionHandler largeGroundColliderPrefab;

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
    }

    public void MovePlayer() //for clientside prediction
    {
        //root.AddForce(inputSpeed * root.transform.forward * moveSpeed);
    }
    public void RotatePlayer() //for clientside prediction
    {
        //root.rotation = Quaternion.Lerp(root.rotation, rotation, owner.sensitivity);
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

        if (owner.IsLocal)
        {

            if (cameraMode != owner.cameraMode)
            {
                cameraMode = owner.cameraMode;
                SetupCameraMode();
            }

            float inputSpeed = 1;
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

            ClientSend.PlayerInput(inputSpeed, new bool[] { isJumping, isFlopping, isSneaking }, rotation);
        }
    }

    public override SimplePlayerController GetMovementController()
    {
        return movementController;
    }
}
