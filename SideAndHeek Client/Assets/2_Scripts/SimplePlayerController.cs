﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimplePlayerController : MonoBehaviour
{
    [SerializeField] private Transform root;

    public float turnSpeed;
    public float clickRange;

    [HideInInspector] public Quaternion rotation = Quaternion.identity;
    
    [SerializeField] private bool legacyControls = false;

    [SerializeField] private PlayerManager playerManager;

    bool isJumping = false;
    bool isFlopping = false;
    
    private CameraMode cameraMode;

    private void Start()
    {
        cameraMode = playerManager.cameraMode;
        SetupCameraMode();
    }

    RaycastHit hit;
    Ray ray;
    private void Update()
    {
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


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.DisplayDisconnectPanel();
        }

        if (!GameManager.instance.gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                playerManager.SetPlayerReady();
                ClientSend.PlayerReady(playerManager.isReady);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                UIManager.instance.DisplayCustomisationPanel();
            }
        }
        
        ray = playerManager.thirdPersonCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "GameStartObject")
            {
                if (Input.GetMouseButtonDown(0))
                {
                    float distance = Mathf.Abs(Vector3.Distance(root.position, hit.transform.position));
                    Debug.DrawLine(root.position, hit.point, Color.red, clickRange);
                    if (distance < clickRange)
                    {
                        ClientSend.TryStartGame();
                    }
                }
                else
                {
                    //Debug.Log("Hover-StartGame"); // replace with hoverStart&Stop
                }
            }

            if (hit.collider.tag == "ItemSpawner")
            {
                if (Input.GetMouseButtonDown(0))
                {
                    //Debug.Log("Click-ItemSpawner");
                }
                else
                {
                    //Debug.Log("Hover-ItemSpawner"); // replace with hoverStart&Stop
                }
            }
        }
    }

    private void SetupCameraMode()
    {
        switch (cameraMode)
        {
            case CameraMode.ThirdPerson:
                playerManager.firstPersonCamera.enabled = false;
                playerManager.thirdPersonCamera.enabled = true;
                foreach (MeshRenderer item in playerManager.headRenderers)
                {
                    item.enabled = true;
                }
                break;
            case CameraMode.FirstPerson:
                playerManager.firstPersonCamera.enabled = true;
                playerManager.thirdPersonCamera.enabled = false;
                foreach (MeshRenderer item in playerManager.headRenderers)
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
        
        SendInputToServer(inputSpeed, new bool[] { isJumping, isFlopping }, rotation);
    }

    public void SendInputToServer(float _inputSpeed, bool[] _otherInputs, Quaternion rotation)
    {
        ClientSend.PlayerMovement(_inputSpeed, _otherInputs, rotation);
    }
}
