using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimplePlayerController : MonoBehaviour
{
    [SerializeField] private Transform root;

    public float turnSpeed;

    [HideInInspector] public Quaternion rotation = Quaternion.identity;

    [SerializeField] private Camera camera;

    [SerializeField] private bool controllerEnabled = false;

    [SerializeField] private PlayerManager playerManager;

    bool isJumping = false;
    bool isFlopping = false;
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
        
        if (!GameManager.gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                playerManager.SetPlayerReady();
                ClientSend.PlayerReady(playerManager.isReady);
            }
        }
    }

    private void FixedUpdate()
    {
        float inputSpeed;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (!controllerEnabled)
        {
            inputSpeed = vertical;
            if (camera)
            {
                Vector3 pos = camera.WorldToScreenPoint(root.position);
                Vector3 dir = Input.mousePosition - pos;
                float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

                // Lerp to target rotation
                rotation = Quaternion.AngleAxis(angle, Vector3.up);
            }
        } else
        {
            inputSpeed = Mathf.Clamp(Mathf.Abs(horizontal) + Mathf.Abs(vertical), 0, 1);

            float angle;
            if (horizontal != 0 || vertical != 0)
            {
                angle = Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg;
                rotation = Quaternion.AngleAxis(angle, Vector3.up);
            } else
            {
                rotation = root.rotation;
            }
        }
        
        SendInputToServer(inputSpeed, new bool[] { isJumping, isFlopping }, rotation);
    }

    public void SendInputToServer(float _inputSpeed, bool[] _otherInputs, Quaternion rotation)
    {
        ClientSend.PlayerMovement(_inputSpeed, _otherInputs, rotation);
    }
}
