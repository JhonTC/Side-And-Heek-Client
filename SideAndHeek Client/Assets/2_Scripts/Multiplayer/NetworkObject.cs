using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkObject : MonoBehaviour 
{
    public ushort objectId;
    public NetworkedObjectType networkedObjectType;

    public virtual void Init(ushort _objectId, NetworkedObjectType _networkedObjectType)
    {
        objectId = _objectId;
        networkedObjectType = _networkedObjectType;
    }

    public void SetObjectTransform(Vector3 _position, Quaternion _rotation, Vector3 _scale)
    {
        transform.position = _position;
        transform.rotation = _rotation;
        transform.localScale = _scale;
    }
}
