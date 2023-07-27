using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EyeAnimator
{
    [SerializeField] private Renderer[] eyes;

    [SerializeField] protected float blinkSpeed = 2f;
    [SerializeField] private float blinkingInterval = 4f;
    private float countToNextBlink = 0;
    private bool isBlinking = true;

    float lidHeight = 1;
    int direction;
    float target;
    bool canOverrideCurrentState = true;

    bool isAnimating = false;

    Action onCompleteCallback = null;

    public ushort currentStateId = 0;

    public void Update()
    {
        if (isAnimating)
        {
            lidHeight += Time.fixedDeltaTime * blinkSpeed * direction;
            lidHeight = Mathf.Clamp01(lidHeight);

            foreach (Renderer eye in eyes)
            {
                eye.material.SetFloat("_EyeLidHeight", lidHeight);
            }

            if (lidHeight == target)
            {
                isAnimating = false;

                onCompleteCallback?.Invoke();
                onCompleteCallback = null;
            }
        }

        if (isBlinking)
        {
            countToNextBlink += Time.fixedDeltaTime;

            if (countToNextBlink >= blinkingInterval)
            {
                countToNextBlink = 0f;
                Close(() =>
                {
                    if (isBlinking)
                        Open();
                });
            }
        }
    }
    public void Open(ushort overridableId, Action onCompleteCallback = null, bool canBeOverriden = true)
    {
        if (overridableId == currentStateId)
        {
            canOverrideCurrentState = true;
        }

        Open(onCompleteCallback, canBeOverriden);
    }
    public void Open(Action onCompleteCallback = null, bool canBeOverriden = true)
    {
        if (!canOverrideCurrentState) return;

        IncrementCurrentState();
        canOverrideCurrentState = canBeOverriden;
        this.onCompleteCallback = onCompleteCallback;
        direction = 1;
        target = 1f;
        isAnimating = true;
        
        countToNextBlink = 0f;
    }

    public void Close(ushort overridableId, Action onCompleteCallback = null, bool canBeOverriden = true)
    {
        if (overridableId == currentStateId)
        {
            canOverrideCurrentState = true;
        }

        Close(onCompleteCallback, canBeOverriden);
    }
    public void Close(Action onCompleteCallback = null, bool canBeOverriden = true)
    {
        if (!canOverrideCurrentState) return;

        IncrementCurrentState();
        canOverrideCurrentState = canBeOverriden;
        this.onCompleteCallback = onCompleteCallback;
        direction = -1;
        target = 0f;
        isAnimating = true;
    }

    private void IncrementCurrentState()
    {
        if (currentStateId < ushort.MaxValue)
        {
            currentStateId++;
        } else
        {
            currentStateId = 0;
        }
    }

    public IEnumerator InvokeAfterDelay(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);

        action?.Invoke();
    }
}
