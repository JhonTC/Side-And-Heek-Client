using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableUIItem : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] public Image targetImage;
    [SerializeField] public Color selectedColour;
    [SerializeField] public Color defaultColour;
    public UnityEvent onSelected;

    public bool selected = false;

    public void OnSelect(BaseEventData eventData)
    {
        targetImage.color = selectedColour;
        onSelected?.Invoke();
        selected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        targetImage.color = defaultColour;
        selected = false;
    }

    private void OnDisable()
    {
        if (selected)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
