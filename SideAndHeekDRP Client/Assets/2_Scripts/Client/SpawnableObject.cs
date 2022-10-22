using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableObject : MonoBehaviour
{
    public ushort objectId;
    public ushort creatorId;

    public PickupDetails activeObjectDetails;

    public void Init(ushort _id, ushort _creatorId, int _code)
    {
        objectId = _id;
        creatorId = _creatorId;

        activeObjectDetails = GameManager.instance.collection.GetPickupByCode((PickupCode)_code);
    }

    public void SetItemTransform(Vector3 _position, Quaternion _rotation, Vector3 _scale)
    {
        transform.position = _position;
        transform.rotation = _rotation;
        transform.localScale = _scale;
    }
}
