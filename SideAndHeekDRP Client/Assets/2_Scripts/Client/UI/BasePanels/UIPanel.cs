using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel : MonoBehaviour
{
    public bool autoToggle = true;

    public virtual void EnablePanel()
    {
        gameObject.SetActive(true);
    }

    public virtual void DisablePanel()
    {
        gameObject.SetActive(false);
    }
}
