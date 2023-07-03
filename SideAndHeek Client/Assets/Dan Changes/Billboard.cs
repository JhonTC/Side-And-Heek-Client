using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform target;

    private void Start()
    {
        target = GameManager.instance.billboardTarget;
    }
    
    void Update()
    {
        transform.LookAt(target.position, Vector3.up);
    }
}
