using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SliderTextSetter : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text sliderValueDisplay;
    [SerializeField] private TMP_Text baseValueDisplay;

    [SerializeField] private float startValue;
    [SerializeField] private string valueSuffix;

    [SerializeField] private bool hostOnly = true;

    private void Start()
    {
        if (LobbyManager.instance.isHost || !hostOnly)
        {
            slider.gameObject.SetActive(true);
            if (baseValueDisplay)
            {
                baseValueDisplay.gameObject.SetActive(false);
            }
        }
        else
        {
            slider.gameObject.SetActive(false); 
            if (baseValueDisplay)
            {
                baseValueDisplay.gameObject.SetActive(true);
            }
        }
    }

    public void OnValueChanged()
    {
        float value = slider.value;
        if (!slider.wholeNumbers)
        {
            value = Mathf.Round(value * 100) / 100;
        }

        sliderValueDisplay.text = value + valueSuffix;
        if (baseValueDisplay)
        {
            baseValueDisplay.text = value + valueSuffix;
        }
    }

    public void SetValue(float value)
    {
        slider.value = value;

        sliderValueDisplay.text = value + valueSuffix;
        if (baseValueDisplay)
        {
            baseValueDisplay.text = value + valueSuffix;
        }
    }
}
