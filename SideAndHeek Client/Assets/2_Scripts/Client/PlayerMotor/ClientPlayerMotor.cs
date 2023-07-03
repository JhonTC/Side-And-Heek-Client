using UnityEngine;
using UnityEngine.InputSystem;

public class ClientPlayerMotor : PlayerMotor
{
    public float turnSpeed;

    private CameraMode cameraMode;

    private Vector2 inputMovement = Vector2.zero;

    public override void Init(Player owner)
    {
        base.Init(owner);

        cameraMode = owner.cameraMode;
        SetupCameraMode();
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

    public void TogglePause(InputAction.CallbackContext value)
    {
        if (value.phase == InputActionPhase.Started)
        {
            UIManager.instance.TogglePanel(UIPanelType.Pause);
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

    private void FixedUpdate()
    {
        //if (owner.IsLocal)
        //{
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
        /*else if (cameraMode == CameraMode.FirstPerson)
        {
            inputSpeed = vertical;

            Quaternion tempRotation = Quaternion.Euler(new Vector3(0, yRot * playerManager.sensitivity, 0));
            rotation = rotation * tempRotation;
            playerManager.firstPersonCameraHolder.rotation = rotation;

            tempRotation = playerManager.firstPersonCamera.transform.localRotation * Quaternion.Euler(new Vector3(xRot * playerManager.sensitivity * -1, 0, 0));
            if (tempRotation.eulerAngles.x > -40 && tempRotation.eulerAngles.x < 40)
                playerManager.firstPersonCamera.transform.localRotation = tempRotation;
        }*/
        if (owner.IsLocal)
        {
            ClientSend.PlayerInput(inputSpeed, new bool[] { isJumping, isFlopping, isSneaking }, rotation);
        }
        //}

        //clientside prediection
        //MovePlayer();
        //RotatePlayer();
    }
}
