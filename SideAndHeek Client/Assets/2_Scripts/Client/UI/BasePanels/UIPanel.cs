using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class UIPanel : MonoBehaviour
{
    public bool autoToggle = true;
    public GameObject firstSelected;

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
