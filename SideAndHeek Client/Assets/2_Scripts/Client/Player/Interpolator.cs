using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpolator : MonoBehaviour
{
    [SerializeField] private float timeElapsed = 0f;
    [SerializeField] private float timeToReachTarget = 0.05f;
    [SerializeField] private float movementThreshhold = 0.05f;

    private readonly List<TransformUpdate> futureTransformUpdates = new List<TransformUpdate>();
    private float squareMovementThreshold;
    private TransformUpdate to;
    private TransformUpdate from;
    private TransformUpdate previous;

    private void Start()
    {
        squareMovementThreshold = movementThreshhold * movementThreshhold;
        to = new TransformUpdate(NetworkManager.Instance.ClientTick, false, transform.position);
        from = new TransformUpdate(NetworkManager.Instance.ClientInterpolationTick, false, transform.position);
        previous = new TransformUpdate(NetworkManager.Instance.ClientInterpolationTick, false, transform.position);
    }

    private void Update()
    {
        for (int i = 0; i < futureTransformUpdates.Count; i++)
        {
            if (NetworkManager.Instance.ClientTick >= futureTransformUpdates[i].tick)
            {
                if (futureTransformUpdates[i].isTeleport)
                {
                    to = futureTransformUpdates[i];
                    from = to;
                    previous = to;
                    transform.position = to.position;
                } else
                {
                    previous = to;
                    to = futureTransformUpdates[i];
                    from = new TransformUpdate(NetworkManager.Instance.ClientInterpolationTick, false, transform.position);
                }

                futureTransformUpdates.RemoveAt(i);
                i--;
                timeElapsed = 0;
                timeToReachTarget = (to.tick - from.tick) * Time.fixedDeltaTime;
            }
        }

        timeElapsed += Time.deltaTime;

        InterpolatePosition(timeElapsed);
    }

    private void InterpolatePosition(float lerpAmount)
    {
        if ((to.position - previous.position).sqrMagnitude < squareMovementThreshold)
        {
            if (to.position != from.position)
            {
                transform.position = Vector3.Lerp(from.position, to.position, lerpAmount);
            }

            return;
        }

        transform.position = Vector3.LerpUnclamped(from.position, to.position, lerpAmount);
    }

    public void NewUpdate(ushort tick, bool isTeleport, Vector3 position)
    {
        if (tick <= NetworkManager.Instance.ClientInterpolationTick && !isTeleport) return;

        for (int i = 0; i < futureTransformUpdates.Count; i++)
        {
            if (tick < futureTransformUpdates[i].tick)
            {
                futureTransformUpdates.Insert(i, new TransformUpdate(tick, isTeleport, position));
                return;
            }
        }

        futureTransformUpdates.Add(new TransformUpdate(tick, isTeleport, position));
    }
}
