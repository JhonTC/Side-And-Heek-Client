using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NetworkObject : MonoBehaviour
{
    public ushort objectId;
    public bool sendTransform = false;
    public NetworkedObjectType networkedObjectType;

    public virtual void Init(ushort _objectId, bool _sendTransform, NetworkedObjectType _networkedObjectType)
    {
        objectId = _objectId;
        sendTransform = _sendTransform;
        networkedObjectType = _networkedObjectType;
    }

    public void SetObjectTransform(Vector3 _position, Quaternion _rotation, Vector3 _scale)
    {
        transform.position = _position;
        transform.rotation = _rotation;
        transform.localScale = _scale;
    }

    protected virtual void FixedUpdate()
    {
        if (NetworkManager.NetworkType != NetworkType.Client)
        {
            if (sendTransform)
            {
                ServerSend.NetworkObjectTransform(objectId, transform.position, transform.rotation, transform.localScale);
            }
        }
    }
}
