using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Camera camera;
    
    void Update()
    {
        transform.LookAt(camera.transform.position, Vector3.up);
    }
}
