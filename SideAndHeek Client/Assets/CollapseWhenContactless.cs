using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapseWhenContactless : MonoBehaviour
{
    private Rigidbody rigidbody;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            if (rigidbody)
            {
                rigidbody.isKinematic = false;
                rigidbody.AddForce((Vector3.up + transform.position.normalized) * 100);
                rigidbody.AddTorque((Vector3.up + transform.position.normalized) * 100);
            }
        } 

        if (other.tag == "LocalFallDetector")
        {
            Destroy(gameObject);
        }
    }
}
