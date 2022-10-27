using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera camera;

    private void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        transform.LookAt(camera.transform.position, Vector3.up);
    }
}
