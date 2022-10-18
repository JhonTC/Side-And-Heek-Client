using Server;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    private void Start()
    {
        cameraMode = playerManager.cameraMode;
        SetupCameraMode();

        if (GameManager.instance.gameType != GameType.Multiplayer)
        {
            player = playerManager as Player;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //isUsingAbility = true;
            playerManager.UsePickup();
        } 

        if (Input.GetAxisRaw("Jump") != 0)
        {
            isJumping = true;
        } else
        {
            isJumping = false;
        }
        if (Input.GetAxisRaw("Flop") != 0)
        {
            isFlopping = true;
        } else
        {
            isFlopping = false;
        }
        if (Input.GetAxisRaw("Sneak") != 0)
        {
            isSneaking = true;
        }
        else
        {
            isSneaking = false;
        }

        if (!GameManager.instance.gameStarted)
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
                UIManager.instance.DisplayCustomisationPanel();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                UIManager.instance.DisplayGameRulesPanel();
            }
        } 

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.DisplayPausePanel();
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
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        float yRot = Input.GetAxisRaw("Mouse X");
        float xRot = Input.GetAxisRaw("Mouse Y");

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
        else if (cameraMode == CameraMode.FirstPerson)
        {
            inputSpeed = vertical;

            Quaternion tempRotation = Quaternion.Euler(new Vector3(0, yRot * playerManager.sensitivity, 0));
            rotation = rotation * tempRotation;
            playerManager.firstPersonCameraHolder.rotation = rotation;

            tempRotation = playerManager.firstPersonCamera.transform.localRotation * Quaternion.Euler(new Vector3(xRot * playerManager.sensitivity * -1, 0, 0));
            if (tempRotation.eulerAngles.x > -40 && tempRotation.eulerAngles.x < 40)
                playerManager.firstPersonCamera.transform.localRotation = tempRotation;
        }

        if (GameManager.instance.gameType == GameType.Multiplayer)
        {
            ClientSend.SetInputs(inputSpeed, new bool[] { isJumping, isFlopping, isSneaking }, rotation);
        }
        else
        {
            //player.SetInput(inputSpeed, new bool[] { isJumping, isFlopping }, rotation);
        }
    }
}
