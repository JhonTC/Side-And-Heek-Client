using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DropdownTextSetter : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    [SerializeField] private TMP_Text baseValueDisplay;

    [SerializeField] private string valueSuffix;

    [SerializeField] private bool hostOnly = true;

    public void OnDisplay(bool isLocalPlayerHost)
    {
        if ((isLocalPlayerHost && hostOnly) || !hostOnly)
        {
            dropdown.gameObject.SetActive(true);

            if (baseValueDisplay)
            {
                baseValueDisplay.gameObject.SetActive(false);
            }
        }
        else
        {
            dropdown.gameObject.SetActive(false);

            if (baseValueDisplay)
            {
                baseValueDisplay.gameObject.SetActive(true);
            }
        }
    }

    public void ChangeValue(int index)
    {
        dropdown.value = index;

        if (baseValueDisplay)
        {
            baseValueDisplay.text = dropdown.options[index].text + valueSuffix;
        }
    }

    public void OnValueChanged()
    {
        int index = dropdown.value;

        if (baseValueDisplay)
        {
            baseValueDisplay.text = dropdown.options[index].text + valueSuffix;
        }
    }
}
