using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIUtils : MonoBehaviour
{
    public static UIUtils instance;

    public SliderTextSetter intSlider;
    public SliderTextSetter floatSlider;
    public DropdownTextSetter enumDropdown;
    public ToggleTextSetter boolToggle;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(gameObject);
        }
    }

    #region Create UI For Type

    public static BaseTextSetter CreateUIForInt(int id, string valueName, float value, int minValue, int maxValue, UnityAction<int, float, BaseTextSetter> valueChangedCallback, Transform parent, string suffix = "", bool ignoreInvoke = false)
    {
        SliderTextSetter newSlider = Instantiate(instance.intSlider, parent);
        newSlider.gameObject.name = valueName;
        newSlider.title.text = valueName;
        newSlider.id = id;
        newSlider.valueSuffix = suffix;

        newSlider.slider.maxValue = maxValue;
        newSlider.slider.minValue = minValue;
        newSlider.slider.value = value;

        newSlider.slider.onValueChanged.AddListener((float result) => {
            valueChangedCallback?.Invoke(id, (int)result, newSlider);
            });

        if (!ignoreInvoke)
        {
            valueChangedCallback?.Invoke(id, value, newSlider);
        }

        return newSlider;
    }

    public static BaseTextSetter CreateUIForFloat(int id, string valueName, float value, float minValue, float maxValue, UnityAction<int, float, BaseTextSetter> valueChangedCallback, Transform parent, string suffix = "", bool ignoreInvoke = false)
    {
        SliderTextSetter newSlider = Instantiate(instance.floatSlider, parent);
        newSlider.gameObject.name = valueName;
        newSlider.title.text = valueName;
        newSlider.id = id;
        newSlider.valueSuffix = suffix;

        newSlider.slider.maxValue = maxValue;
        newSlider.slider.minValue = minValue;
        newSlider.slider.value = value;

        newSlider.slider.onValueChanged.AddListener((float result) => {
            valueChangedCallback?.Invoke(id, result, newSlider);
        });

        if (!ignoreInvoke)
        {
            valueChangedCallback?.Invoke(id, value, newSlider);
        }

        return newSlider;
    }

    public static BaseTextSetter CreateUIForBool(int id, string valueName, bool value, UnityAction<int, bool, BaseTextSetter> valueChangedCallback, Transform parent, bool ignoreInvoke = false)
    {
        ToggleTextSetter newToggle = Instantiate(instance.boolToggle, parent);
        newToggle.gameObject.name = valueName;
        newToggle.title.text = valueName;
        newToggle.id = id;

        newToggle.toggle.isOn = value;

        newToggle.toggle.onValueChanged.AddListener((bool result) => {
            valueChangedCallback?.Invoke(id, result, newToggle);
        });

        if (!ignoreInvoke)
        {
            valueChangedCallback?.Invoke(id, value, newToggle);
        }

        return newToggle;
    }

    public static BaseTextSetter CreateUIForEnum<T>(int id, string valueName, int value, UnityAction<int, int, BaseTextSetter> valueChangedCallback, Transform parent, bool ignoreInvoke = false)
    {
        DropdownTextSetter newDropdown = Instantiate(instance.enumDropdown, parent);
        newDropdown.gameObject.name = valueName;
        newDropdown.title.text = valueName;
        newDropdown.id = id;

        string[] enumContentNames = Enum.GetNames(typeof(T));
        for (int i = 0; i < enumContentNames.Length; i++)
        {
            newDropdown.dropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(enumContentNames[i]));
        }
        ScrollRect scrollRect = newDropdown.dropdown.GetComponentInChildren<ScrollRect>(true);
        if (scrollRect != null)
        {
            RectTransform rectTrans = scrollRect.transform as RectTransform;
            rectTrans.sizeDelta = new Vector2(rectTrans.sizeDelta.x, (newDropdown.dropdown.options.Count + 1) * 30); //todo: stop using magic numbers here
        }

        newDropdown.dropdown.value = value;

        newDropdown.dropdown.onValueChanged.AddListener((int result) => {
            valueChangedCallback?.Invoke(id, result, newDropdown);
        });

        if (!ignoreInvoke)
        {
            valueChangedCallback?.Invoke(id, value, newDropdown);
        }

        return newDropdown;
    }

    #endregion
}
