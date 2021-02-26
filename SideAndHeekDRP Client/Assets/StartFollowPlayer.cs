using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFollowPlayer : MonoBehaviour
{
    public Camera playerCamera;
    public Transform target;
    [SerializeField] private float followSpeed;

    [SerializeField] private Vector3 distanceToPlayer;
    [SerializeField] private Vector3 lookRotation;
    [SerializeField] private float orthographicSize;
    [SerializeField] private bool isOrthographic;

    void FixedUpdate()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        if (target)
        {
            Vector3 targetPos = target.position + distanceToPlayer;
            targetPos.y = distanceToPlayer.y;

            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed);

            if (isOrthographic)
            {
                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(lookRotation), followSpeed * 1.5f);
                playerCamera.orthographicSize = Mathf.Lerp(playerCamera.orthographicSize, orthographicSize, followSpeed * 1.5f);
            }
        }
    }
}
