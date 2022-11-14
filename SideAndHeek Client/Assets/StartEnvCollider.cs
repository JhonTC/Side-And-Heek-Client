using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartEnvCollider : MonoBehaviour
{
    [SerializeField] private StartSceneController ssc;

    public Transform mapPiece;
    public int index;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BodyCollider"))
        {
            ssc.OnWalkEndHit(index);
        }
    }
}
