using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DropdownTextSetter : BaseTextSetter
{
    public TMP_Dropdown dropdown;

    public override void OnDisplay(bool isLocalPlayerHost)
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

    public override void ChangeValue(object index)
    {
        dropdown.value = (int)index;

        if (baseValueDisplay)
        {
            baseValueDisplay.text = dropdown.options[(int)index].text + valueSuffix;
        }
    }

    public override object GetValue()
    {
        return dropdown.value;
    }

    public override void OnValueChanged()
    {
        int index = dropdown.value;

        if (baseValueDisplay)
        {
            baseValueDisplay.text = dropdown.options[index].text + valueSuffix;
        }
    }
}
