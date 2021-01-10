using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Camera playerCamera;
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed;
    
    [SerializeField] private Vector3 distanceToPlayer;
    [SerializeField] private Vector3 lookRotation;
    [SerializeField] private float orthographicSize;

    private void Start()
    {
        playerCamera = GetComponent<Camera>();

        transform.position = GameManager.instance.sceneCamera.transform.position;
        transform.rotation = GameManager.instance.sceneCamera.transform.rotation;
        playerCamera.orthographicSize = GameManager.instance.sceneCamera.GetComponent<Camera>().orthographicSize;
    }

    void FixedUpdate()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        if (target)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + distanceToPlayer, followSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(lookRotation), followSpeed * 1.5f);
            playerCamera.orthographicSize = Mathf.Lerp(playerCamera.orthographicSize, orthographicSize, followSpeed * 1.5f);
        }
    }
}
