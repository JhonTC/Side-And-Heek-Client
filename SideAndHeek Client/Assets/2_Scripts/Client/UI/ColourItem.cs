using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourItem : MonoBehaviour
{
    //[SerializeField] private Image outline;
    [SerializeField] private Image background;
    [SerializeField] private Image colourImage;
    [SerializeField] private Image chosenImage;
    public Button button;

    public Color colour;

    public void Init(Color _colour)
    {
        colour = _colour;
        colourImage.color = colour;

        button.onClick.AddListener(() => UIManager.instance.customisationPanel.OnHiderColourChangeButtonPressed(this));
    }

    public void SetInteractable(bool interactable, bool chosenByOther, Color outlineColour, Color backgroundColour)
    {
        //outline.color = outlineColour;
        background.color = backgroundColour;

        chosenImage.color = outlineColour; //todo: check greyscale float, determine whether to use dark or light 'x'
        chosenImage.enabled = !interactable;

        button.interactable = interactable;
        if (!interactable && !chosenByOther)
        {
            button.Select();
        }
    }
}
