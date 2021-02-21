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

    private void Start()
    {
        if (LobbyManager.instance.isHost)
        {
            slider.gameObject.SetActive(true);
            baseValueDisplay.gameObject.SetActive(false);
        }
        else
        {
            slider.gameObject.SetActive(false);
            baseValueDisplay.gameObject.SetActive(true);
        }

        slider.value = startValue;
        OnValueChanged();
    }

    public void OnValueChanged()
    {
        float value = slider.value;
        if (!slider.wholeNumbers)
        {
            value = Mathf.Round(value * 100) / 100;
        }

        sliderValueDisplay.text = value + valueSuffix;
        baseValueDisplay.text = value + valueSuffix;
    }
}
