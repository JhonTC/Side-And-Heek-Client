using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private PlayerController pc;
    //private Vector3 lastMousePosition;

    void Start()
    {
        pc = GetComponent<PlayerController>();

        //lastMousePosition = Input.mousePosition;
    }

    private void FixedUpdate()
    {
        //if (!hasAuthority)
            //return;

        float fbInput = Input.GetAxisRaw("Vertical");
        float lrInput = Input.GetAxisRaw("Horizontal");
        float jumpInput = Input.GetAxisRaw("Jump");
        //float turnInput = lastMousePosition.x - Input.mousePosition.x;
        
        //pc.ApplyGravity();

        //pc.SendInputToServer(new float[] { fbInput, lrInput, jumpInput });

        //pc.MovePlayer(fbInput, lrInput, jumpInput);
        pc.RotatePlayer(Input.mousePosition);
        
        //lastMousePosition = Input.mousePosition;
    }
}
