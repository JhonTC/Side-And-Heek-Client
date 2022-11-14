using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleTextSetter : MonoBehaviour
{
    public Toggle toggle;
    [SerializeField] private Image icon;

    [SerializeField] private Sprite activeIcon;
    [SerializeField] private Sprite inactiveIcon;

    [SerializeField] private bool hostOnly = true;

    public void OnDisplay(bool isLocalPlayerHost)
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

    public void ChangeValue(bool isOn)
    {
        toggle.isOn = isOn;

        if (icon)
        {
            icon.sprite = (toggle.isOn) ? activeIcon : inactiveIcon;
        }
    }
}
