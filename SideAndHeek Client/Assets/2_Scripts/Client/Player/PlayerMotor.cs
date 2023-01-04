using Server;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMotor : MonoBehaviour
{
    public Transform root;
    public Transform head;
    public Transform leftLeg;
    public Transform rightLeg;
    public Transform leftFoot;
    public Transform rightFoot;

    public float turnSpeed;

    [HideInInspector] public Quaternion rotation = Quaternion.identity;
    
    [SerializeField] private bool legacyControls = false;

    public Player playerManager;
    Player player;

    public MeshRenderer[] headRenderers;

    bool isJumping = false;
    bool isFlopping = false;
    bool isSneaking = false;
    bool isUsingAbility = false;

    private CameraMode cameraMode;

    private Vector2 inputMovement = Vector2.zero;

    private void Start()
    {
        cameraMode = playerManager.cameraMode;
        SetupCameraMode();

        if (GameManager.instance.networkType != NetworkType.Multiplayer)
        {
            player = playerManager as Player;
        }
    }

    private void Update()
    {
        /*if (Input.GetMouseButtonDown(1))
        {
            //isUsingAbility = true;
            playerManager.UsePickup();
        }

        /*if (!GameManager.instance.gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                playerManager.SetPlayerReady();
                if (GameManager.instance.gameType == GameType.Multiplayer)
                {
                    ClientSend.PlayerReady(playerManager.isReady);
                }
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                UIManager.instance.DisplayPanel(UIPanelType.Customisation);
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                UIManager.instance.DisplayPanel(UIPanelType.Game_Rules);
            }
        } 

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.DisplayPanel(UIPanelType.Pause);
        }*/
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        inputMovement = value.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        bool jump = value.ReadValueAsButton();
        if (isJumping != jump)
        {
            isJumping = jump;
        }
    }

    public void OnFlop(InputAction.CallbackContext value)
    {
        bool flop = value.ReadValueAsButton();
        if (isFlopping != flop)
        {
            isFlopping = flop;
        }
    }

    public void OnUse(InputAction.CallbackContext value)
    {
        if (value.phase == InputActionPhase.Started)
        {
            playerManager.UsePickup();
        }
    }

    public void OnReady(InputAction.CallbackContext value)
    {
        if (!GameManager.instance.gameStarted)
        {
            playerManager.SetPlayerReady(!playerManager.isReady);
            if (GameManager.instance.networkType == NetworkType.Multiplayer)
            {
                ClientSend.PlayerReady(playerManager.isReady);
            }
        }
    }

    public void ToggleCustomisation(InputAction.CallbackContext value)
    {
        if (!GameManager.instance.gameStarted)
        {
            if (value.phase == InputActionPhase.Started)
            {
                UIManager.instance.TogglePanel(UIPanelType.Customisation);
            }
        }
    }

    public void ToggleGameRules(InputAction.CallbackContext value)
    {
        if (!GameManager.instance.gameStarted)
        {
            if (value.phase == InputActionPhase.Started)
            {
                UIManager.instance.TogglePanel(UIPanelType.Game_Rules);
            }
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
                playerManager.firstPersonCamera.enabled = false;
                playerManager.thirdPersonCamera.enabled = true;
                foreach (MeshRenderer item in headRenderers)
                {
                    item.enabled = true;
                }
                break;
            case CameraMode.FirstPerson:
                playerManager.firstPersonCamera.enabled = true;
                playerManager.thirdPersonCamera.enabled = false;
                foreach (MeshRenderer item in headRenderers)
                {
                    item.enabled = false;
                }
                break;
        }
    }
    
    private void FixedUpdate()
    {
        if (cameraMode != playerManager.cameraMode)
        {
            cameraMode = playerManager.cameraMode;
            SetupCameraMode();
        }

        float inputSpeed = 1;
        float horizontal = inputMovement.x;
        float vertical = inputMovement.y;

        if (cameraMode == CameraMode.ThirdPerson)
        {
            if (!legacyControls)
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
            else
            {
                inputSpeed = vertical;
                if (playerManager.thirdPersonCamera)
                {
                    Vector3 pos = playerManager.thirdPersonCamera.WorldToScreenPoint(root.position);
                    Vector3 dir = Input.mousePosition - pos;
                    float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

                    // Lerp to target rotation
                    rotation = Quaternion.AngleAxis(angle, Vector3.up);
                }
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

        if (GameManager.instance.networkType == NetworkType.Multiplayer)
        {
            ClientSend.SetInputs(inputSpeed, new bool[] { isJumping, isFlopping, isSneaking }, rotation);
        }
        else
        {
            //player.SetInput(inputSpeed, new bool[] { isJumping, isFlopping }, rotation);
        }
    }
}
