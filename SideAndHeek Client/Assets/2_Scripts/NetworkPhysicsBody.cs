using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPhysicsBody : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody root;
    [HideInInspector]
    public Transform[] bones;

    private const string rootTag = "Root";

    private void Start()
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>(false);
        bones = new Transform[rigidbodies.Length];
        for (int i = 0; i < bones.Length; i++)
        {
            bones[i] = rigidbodies[i].transform;
            if (bones[i].tag == rootTag)
            {
                root = rigidbodies[i];
            }
        }

        if (root == null && rigidbodies.Length > 0)
        {
            root = rigidbodies[0];
        }
    }

    public void DisablePhysics()
    {
        for (int i = 0; i < bones.Length; i++)
        {
            ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
            if (joint != null)
            {
                Destroy(joint);
            }

            Rigidbody rigidbody = GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = true;
            }
        }
    }

    public void AddMessageValues(ref Message message)
    {
        for (int i = 0; i < bones.Length; i++)
        {
            message.Add(bones[i].position);
            message.Add(bones[i].rotation);
        }
    }

    public void ReadMessageValues(Message message)
    {
        for (int i = 0; i < bones.Length; i++)
        {
            Vector3 _position = message.GetVector3();
            Quaternion _rotation = message.GetQuaternion();

            SetPhysicsBodyTransform(bones[i], _position, _rotation);
        }
    }

    private void SetPhysicsBodyTransform(Transform bodyTransform, Vector3 position, Quaternion rotation)
    {
        bodyTransform.position = position;
        bodyTransform.rotation = rotation;
    }
}
