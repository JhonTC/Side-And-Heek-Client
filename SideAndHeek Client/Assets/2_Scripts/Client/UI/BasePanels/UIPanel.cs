using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIPanel : MonoBehaviour
{
    public bool autoToggle = true;
    public GameObject firstSelected;

    public UIPanelType panelType;

    public UnityAction<int, int, BaseTextSetter> OnEnumValueChanged;
    public UnityAction<int, float, BaseTextSetter> OnFloatValueChanged;
    public UnityAction<int, bool, BaseTextSetter> OnBoolValueChanged;

    public virtual void EnablePanel()
    {
        gameObject.SetActive(true);

        if (firstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }

    public virtual void DisablePanel()
    {
        gameObject.SetActive(false);
    }
}
