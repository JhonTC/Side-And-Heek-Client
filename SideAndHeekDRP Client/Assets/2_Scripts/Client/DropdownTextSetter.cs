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
        if (!hostOnly || isLocalPlayerHost)
        {
            dropdown.gameObject.SetActive(true);
            baseValueDisplay.gameObject.SetActive(false);
        }
        else
        {
            dropdown.gameObject.SetActive(false);
            baseValueDisplay.gameObject.SetActive(true);
        }
    }

    public void ChangeValue(int index)
    {
        dropdown.value = index;
        baseValueDisplay.text = dropdown.options[index].text + valueSuffix;
    }

    public void OnValueChanged()
    {
        int index = dropdown.value;
        baseValueDisplay.text = dropdown.options[index].text + valueSuffix;
    }

    public void SetValue(int value)
    {
        dropdown.value = value;

        baseValueDisplay.text = dropdown.options[value].text + valueSuffix;
    }
}
