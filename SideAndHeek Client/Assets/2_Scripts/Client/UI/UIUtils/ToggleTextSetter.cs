using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleTextSetter : BaseTextSetter
{
    public Toggle toggle;
    [SerializeField] private Image icon;

    [SerializeField] private Sprite activeIcon;
    [SerializeField] private Sprite inactiveIcon;

    public override void OnDisplay(bool isLocalPlayerHost)
    {
        if ((isLocalPlayerHost && hostOnly) || !hostOnly)
        {
            toggle.gameObject.SetActive(true);

            if (icon)
            {
                icon.gameObject.SetActive(false);
            }
        }
        else
        {
            toggle.gameObject.SetActive(false);

            if (icon)
            {
                icon.gameObject.SetActive(true);
                icon.sprite = (toggle.isOn) ? activeIcon : inactiveIcon;
            }
        }
    }

    public override void ChangeValue(object isOn)
    {
        toggle.isOn = (bool)isOn;

        if (icon)
        {
            icon.sprite = (toggle.isOn) ? activeIcon : inactiveIcon;
        }
    }

    public override object GetValue()
    {
        return toggle.isOn;
    }
}
