using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed;

    private Vector3 initialPos;

    private void Start()
    {
        initialPos = transform.position;
    }

    void Update()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        if (target)
            transform.position = Vector3.Lerp(transform.position, target.position + initialPos, followSpeed);
    }
}
