using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushCollisionHandler : MonoBehaviour
{
    public bool isColliding { get { return activeCollisions.Count > 0; } }
    List<MeshRenderer> activeCollisions = new List<MeshRenderer>();
    public string compareTag = "Bush";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(compareTag))
        {
            MeshRenderer renderer = other.gameObject.GetComponent<MeshRenderer>();
            Color col = renderer.material.color;
            col.a = 0.3f;
            renderer.material.color = col;
            activeCollisions.Add(renderer);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(compareTag))
        {
            MeshRenderer renderer = other.gameObject.GetComponent<MeshRenderer>();
            Color col = renderer.material.color;
            col.a = 1f;
            renderer.material.color = col;
            activeCollisions.Remove(renderer);
        }
    }
}
