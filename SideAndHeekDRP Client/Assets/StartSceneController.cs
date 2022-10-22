using Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartSceneController : MonoBehaviour
{
    public PlayerController playerController;

    public Dictionary<int, Transform> locations = new Dictionary<int, Transform>();
    public StartEnvCollider[] colliders;

    private int collisionCounter = 1;

    public FootCollisionHandler largeGroundCollider;
    public Transform feetMidpoint;

    int currentIndex = 1;

    private int lastHitIndex = 1;

    [SerializeField] private TMP_Text playerUsername;

    private void Start()
    {
        SetupPlayer();

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].index = i;
            locations.Add(i, colliders[i].mapPiece);
        }
    }
    public void SetupPlayer()
    {
        playerController.largeGroundCollider = largeGroundCollider;
        playerController.feetMidpoint = feetMidpoint;
        playerController.SetupBodyCollisionHandlers(null);

        if (PlayerPrefs.HasKey("Username"))
        {
            playerUsername.text = PlayerPrefs.GetString("Username");
        }
    }

    private void FixedUpdate()
    {
        playerController.CustomFixedUpdate(1);

        Vector3 dir = colliders[currentIndex].transform.position - playerController.transform.position;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        // Lerp to target rotation
        playerController.SetRotation(Quaternion.AngleAxis(angle, Vector3.up));
    }

    public void OnWalkEndHit(int index)
    {
        if (index != lastHitIndex)
        {
            playerController.OnJump();

            currentIndex = index;
            collisionCounter++;
            colliders[currentIndex].mapPiece.transform.localPosition = Vector3.right * 56.5f * collisionCounter;

            lastHitIndex = currentIndex;
        }
    }
}
