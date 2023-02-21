using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SliderTextSetter : BaseTextSetter
{
    public Slider slider;
    [SerializeField] private TMP_Text sliderValueDisplay;

    public override void OnDisplay(bool isLocalPlayerHost)
    {
        if ((isLocalPlayerHost && hostOnly) || !hostOnly)
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

    public override void ChangeValue(object value)
    {
        slider.value = (float)value;
        sliderValueDisplay.text = value + valueSuffix;

        if (baseValueDisplay)
        {
            baseValueDisplay.text = value + valueSuffix;
        }
    }

    public override void OnValueChanged()
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

    public override object GetValue()
    {
        return slider.value;
    }
}
