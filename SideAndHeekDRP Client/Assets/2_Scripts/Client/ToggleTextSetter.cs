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
        if (!hostOnly || isLocalPlayerHost)
        {
            toggle.gameObject.SetActive(true);
            icon.gameObject.SetActive(false);
        }
        else
        {
            toggle.gameObject.SetActive(false);
            icon.gameObject.SetActive(true);
            icon.sprite = (toggle.isOn) ? activeIcon : inactiveIcon;
        }
    }

    public void ChangeValue(bool isOn)
    {
        toggle.isOn = isOn;
        icon.sprite = (toggle.isOn) ? activeIcon : inactiveIcon;
    }
}
