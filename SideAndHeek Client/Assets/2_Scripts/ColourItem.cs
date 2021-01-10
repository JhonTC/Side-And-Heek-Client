using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourItem : MonoBehaviour
{
    [SerializeField] private Image outline;
    [SerializeField] private Image background;
    [SerializeField] private Image colourImage;
    [SerializeField] private Button button;
    public Color colour;

    private void Start()
    {
        colourImage.color = colour;
    }

    public void SetInteractable(bool interactable, Color outlineColour, Color backgroundColour)
    {
        outline.color = outlineColour;
        background.color = backgroundColour;

        button.interactable = interactable;
    }
}
