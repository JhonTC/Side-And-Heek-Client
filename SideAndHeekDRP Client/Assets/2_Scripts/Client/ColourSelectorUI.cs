using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourSelectorUI : MonoBehaviour
{
    [SerializeField] private ColourItem colourItemPrefab;
    private List<ColourItem> colourItems = new List<ColourItem>();

    [SerializeField] private Color backgroundActiveColour;
    [SerializeField] private Color backgroundInactiveColour;

    [SerializeField] private Color outlineActiveColour;
    [SerializeField] private Color outlineInactiveColour;

    public void Init(Color[] colours)
    {
        foreach (Color colour in colours)
        {
            ColourItem newItem = Instantiate(colourItemPrefab, transform);
            newItem.Init(colour);

            colourItems.Add(newItem);
        }
    }

    public void ClearAll()
    {
        for (int i = 0; i < colourItems.Count; i++)
        {
            Destroy(colourItems[i].gameObject);
        }
        colourItems.Clear();
    }

    public void UpdateAllButtons(bool chosenByOther, ColourItem exception = null)
    {
        print("UpdateAllButtons");
        foreach (ColourItem item in colourItems)
        {
            if (item != exception)
            {
                item.SetInteractable(item != exception, chosenByOther, outlineActiveColour, backgroundActiveColour);
            }
            else
            {
                item.SetInteractable(item != exception, chosenByOther, outlineInactiveColour, backgroundInactiveColour);
            }
        }
    }
    public void UpdateAllButtons(bool chosenByOther, Color exception)
    {
        print("UpdateAllButtons");
        foreach (ColourItem item in colourItems)
        {
            if (item.colour != exception)
            {
                item.SetInteractable(item.colour != exception, chosenByOther, outlineActiveColour, backgroundActiveColour);
            } else
            {
                item.SetInteractable(item.colour != exception, chosenByOther, outlineInactiveColour, backgroundInactiveColour);
            }
        }
    }

    public void UpdateAllButtons()
    {
        foreach (ColourItem item in colourItems)
        {
            int doesPlayerOwnColour = DoesPlayerOwnColour(item.colour);
            bool isColourTaken = doesPlayerOwnColour != 0;
            bool chosenByOther = doesPlayerOwnColour == 2;

            Color outlineColour = outlineActiveColour;
            Color backgroundColour = backgroundActiveColour;
            if (isColourTaken)
            {
                outlineColour = outlineInactiveColour;
                backgroundColour = backgroundInactiveColour;
            }

            item.SetInteractable(!isColourTaken, chosenByOther, outlineColour, backgroundColour);
        }
    }

    private int DoesPlayerOwnColour(Color colour)
    {
        foreach (Player player in LobbyManager.players.Values)
        {
            if (player.hiderColour == colour)
            {
                if (player.IsLocal)
                {
                    return 1;
                } else
                {
                    return 2;
                }
            }
        }

        return 0;
    }

    public void UpdateButtonWithColour(Color colour, bool interactable, bool chosenByOther)
    {
        foreach (ColourItem item in colourItems)
        {
            if (item.colour == colour)
            {
                item.SetInteractable(interactable, chosenByOther, outlineActiveColour, backgroundActiveColour);
            }
        }
    }
}
