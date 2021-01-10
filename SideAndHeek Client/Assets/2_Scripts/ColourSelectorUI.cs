using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourSelectorUI : MonoBehaviour
{
    public ColourItem[] colourItems;

    [SerializeField] private Color backgroundActiveColour;
    [SerializeField] private Color backgroundInactiveColour;

    [SerializeField] private Color outlineActiveColour;
    [SerializeField] private Color outlineInactiveColour;

    public void UpdateAllButtons() { UpdateAllButtons(null); }
    public void UpdateAllButtons(ColourItem exception)
    {
        foreach (ColourItem item in colourItems)
        {
            if (item != exception)
            {
                item.SetInteractable(item != exception, outlineActiveColour, backgroundActiveColour);
            } else
            {
                item.SetInteractable(item != exception, outlineInactiveColour, backgroundInactiveColour);
            }
        }
    }
}
