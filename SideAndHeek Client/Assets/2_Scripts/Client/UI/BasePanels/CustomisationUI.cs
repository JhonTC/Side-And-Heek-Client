using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomisationUI : TabView
{
    public ColourSelectorUI hiderColourSelector;

    public override void EnablePanel()
    {
        base.EnablePanel();
        hiderColourSelector.UpdateAllButtons();
    }

    public void Init(Color[] hiderColours)
    {
        hiderColourSelector.Init(hiderColours);
    }

    public void OnHiderColourChangeButtonPressed(ColourItem colourItem)
    {
        UIManager.instance.OnBackButtonPressed();

        ClientSend.SetPlayerColour(colourItem.colour, false);
    }

    public void OnSeekerColourChangeButtonPressed(ColourItem colourItem)
    {
        UIManager.instance.OnBackButtonPressed();

        Player.LocalPlayer.ChangeBodyColour(colourItem.colour, true);

        ClientSend.SetPlayerColour(colourItem.colour, true);
    }
}
