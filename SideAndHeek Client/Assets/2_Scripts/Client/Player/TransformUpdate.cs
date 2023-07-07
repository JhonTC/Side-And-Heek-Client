using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUpdate
{
    public ushort tick { get; private set; }
    public bool isTeleport { get; private set; }
    public Vector3 position { get; private set; }

    public TransformUpdate(ushort tick, bool isTeleport, Vector3 position)
    {
        this.tick = tick;
        this.isTeleport = isTeleport;
        this.position = position;
    }
}
