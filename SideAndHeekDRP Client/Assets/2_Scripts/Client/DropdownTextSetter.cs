using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DropdownTextSetter : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private TMP_Text baseValueDisplay;

    [SerializeField] private int startValue;
    [SerializeField] private string valueSuffix;

    private void Start()
    {
        if (LobbyManager.instance.isHost)
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
