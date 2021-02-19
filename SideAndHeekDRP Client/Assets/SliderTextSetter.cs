using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderTextSetter : MonoBehaviour
{
    [SerializeField] private TMP_Text sliderValueDisplay;

    [SerializeField] private float startValue;
    [SerializeField] private string valueSuffix;

    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = startValue;
        OnValueChanged();
    }

    public void OnValueChanged()
    {
        sliderValueDisplay.text = slider.value + valueSuffix;
    }
}
