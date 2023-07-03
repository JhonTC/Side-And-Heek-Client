using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Camera playerCamera;
    public Transform target;
    [SerializeField] private float followSpeed;
    
    [SerializeField] private Vector3 distanceToPlayer;
    [SerializeField] private Vector3 lookRotation;
    [SerializeField] private float orthographicSize;
    [SerializeField] private bool isOrthographic;

    private void Start()
    {
        transform.position = GameManager.instance.sceneCamera.transform.position;
        transform.rotation = GameManager.instance.sceneCamera.transform.rotation;

        if (isOrthographic)
        {
            playerCamera = GetComponent<Camera>();
            playerCamera.orthographicSize = GameManager.instance.sceneCamera.GetComponent<Camera>().orthographicSize;
        }
    }

    void FixedUpdate()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        if (target)
        {
            Vector3 targetPosition = target.position + distanceToPlayer;
            targetPosition.y = Mathf.Clamp(targetPosition.y, 0, float.MaxValue);

            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed);

            if (isOrthographic)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(lookRotation), followSpeed * 1.5f);
                playerCamera.orthographicSize = Mathf.Lerp(playerCamera.orthographicSize, orthographicSize, followSpeed * 1.5f);
            }
        }
    }

    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
        TeleportCameraToTarget(target.position);
    }

    public void TeleportCameraToTarget(Vector3 position)
    {
        transform.position = position + distanceToPlayer;
        print("TeleportCameraToTarget");
    }
}
