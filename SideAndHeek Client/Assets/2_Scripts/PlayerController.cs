using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float moveAccel;
    [SerializeField] private float turnAccel;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravitationalForce;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //ApplyGravity();
    }

    public void ApplyGravity()
    {
        rb.AddForce(transform.up * gravitationalForce);
    }

    public void SendInputToServer(bool[] _inputs)
    {
        //ClientSend.PlayerMovement(_inputs);
    }

    /*[Command]
    public void CmdMovePlayer(float fbInput, float lrInput, bool jumpInput)
    {
        //validate movement

        RpcMovePlayer(fbInput, lrInput, jumpInput);
    }*/

    //[ClientRpc]
    public void MovePlayer(float fbInput, float lrInput, bool jumpInput)
    {
        float inputDivisor = Mathf.Abs(fbInput) + Mathf.Abs(lrInput);

        if (inputDivisor != 0)
        {
            Vector3 localXMov = transform.forward * (fbInput / inputDivisor) * moveAccel;
            Vector3 localZMov = transform.right * (lrInput / inputDivisor) * moveAccel;
            //Vector3 localXMov = Vector3.forward * (fbInput / inputDivisor) * moveAccel;
            //Vector3 localZMov = Vector3.right * (lrInput / inputDivisor) * moveAccel;

            if (rb.velocity.magnitude < maxMoveSpeed)
                rb.AddForce(localXMov/* + localZMov*/);
        }

        /*Vector3 localYMov = Vector3.zero;
        if (jumpInput && !isJumping)
        {
            localYMov = transform.up * jumpForce;
            isJumping = true;
        }
        rb.AddForce(localYMov);*/
    }

    /*[Command]
    public void CmdRotatePlayer(Vector3 mousePos)
    {
        //validate roataion

        RpcRotatePlayer(mousePos);
    }*/

    //[ClientRpc]
    public void RotatePlayer(Vector3 mousePos)
    {
        //Vector3 nextRot = transform.rotation.eulerAngles + new Vector3(0, turnInput * turnAccel * Time.fixedDeltaTime, 0);
        //Vector3 newLerpRot = Vector3.Lerp(transform.rotation.eulerAngles, nextRot, turnSpeed);
        if (Camera.main)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 dir = mousePos - pos;
            float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

            // Lerp to target rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.up), Time.fixedDeltaTime * turnSpeed);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isJumping = false;
        }
    }
}
