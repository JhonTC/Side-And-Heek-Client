using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkButton : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Transform pad;
    [SerializeField] private float cooldown = 4f;

    private float cooldownCounter = 0;

    private float padLowYTarget = 0.3f;
    private float padHighYTarget = 0.65f;

    [SerializeField] private bool isActive = false;
    [SerializeField] private bool colourFlip = false;

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            if (pad.localPosition.y <= padLowYTarget)
            {
                SetIsActive(true);
            }
        }
        else
        {
            cooldownCounter += Time.deltaTime;

            if (cooldownCounter >= cooldown)
            {
                SetIsActive(false);
                cooldownCounter = 0;
            }
        }

        meshRenderer.material.SetFloat("CooldownProgress", Mathf.InverseLerp(padLowYTarget, padHighYTarget, pad.localPosition.y));
    }

    public void SetIsActive(bool value)
    {
        isActive = value;
        meshRenderer.material.SetInt("IsActive", isActive ? 1 : 0);
    }

    public void SetColourFlip(bool value)
    {
        colourFlip = value;
        meshRenderer.material.SetInt("ColourFlip", isActive ? 1 : 0);
    }
}
