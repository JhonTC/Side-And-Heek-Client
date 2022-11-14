using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{
    [SerializeField] private TabView tabView;
    [SerializeField] private int tabIndex;
    [SerializeField] private GameObject tabButton;

    public Selectable firstSelected;
    [SerializeField] private GameObject tabContent;

    public void OnTabToggled(bool value)
    {
        tabButton.SetActive(value);
        tabContent.SetActive(value);

        if (value)
        {
            if (firstSelected)
            {
                firstSelected.Select();
            }
        }
    }
}
