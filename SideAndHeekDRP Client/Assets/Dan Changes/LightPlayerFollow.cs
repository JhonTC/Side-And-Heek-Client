using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPlayerFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private SimplePlayerController spc;
    [SerializeField] private float turnSpeed;

    private void Update()
    {
        transform.position = target.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, spc.rotation, Time.fixedDeltaTime * turnSpeed);
    }
}
