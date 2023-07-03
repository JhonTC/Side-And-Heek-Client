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

    [SerializeField] private int columnCount;

    public void Init(Color[] colours)
    {
        for (int i = 0; i < colours.Length; i++)
        {
            ColourItem newItem = Instantiate(colourItemPrefab, transform);
            newItem.Init(colours[i]);

            colourItems.Add(newItem);
        }

        int yIndex = -1;
        for (int i = 0; i < colourItems.Count; i++)
        {
            int xIndex = i % columnCount;
            if (i == 0) { yIndex++; }

            Navigation navigation = colourItems[i].button.navigation;

            int leftIndex = i - 1;
            if (xIndex > 0 && i >= 0)
            {
                navigation.selectOnLeft = colourItems[i - 1].button;
            }

            int rightIndex = i + 1;
            if (xIndex < columnCount - 1 && i + 1 < colourItems.Count)
            {
                navigation.selectOnRight = colourItems[i + 1].button;
            }

            int upIndex = i - columnCount;
            if (upIndex >= 0)
            {
                navigation.selectOnUp = colourItems[i - columnCount].button;
            }

            int downIndex = i + columnCount;
            if (downIndex < colourItems.Count)
            {
                navigation.selectOnDown = colourItems[i + columnCount].button;
            }

            colourItems[i].button.navigation = navigation;
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
        foreach (Player player in Player.list.Values)
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
