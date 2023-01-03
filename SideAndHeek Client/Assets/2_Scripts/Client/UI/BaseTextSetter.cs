using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseTextSetter : MonoBehaviour
{
    public TMP_Text title;
    public int id;

    [SerializeField] protected TMP_Text baseValueDisplay;

    public string valuePrefix;
    public string valueSuffix;

    [SerializeField] protected bool hostOnly = true;

    public virtual void OnDisplay(bool isLocalPlayerHost) {}

    public virtual void ChangeValue(object value) {}

    public virtual object GetValue()
    {
        return null;
    }

    public virtual void OnValueChanged() {}
}
