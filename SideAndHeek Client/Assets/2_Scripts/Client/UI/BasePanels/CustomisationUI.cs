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

        if (NetworkManager.NetworkType == NetworkType.Client)
        {
            ClientSend.SetPlayerColour(colourItem.colour, false);
        }
        else if (NetworkManager.NetworkType == NetworkType.ClientServer)
        {
            GameManager.instance.AttemptColourChange(Player.LocalPlayer.Id, colourItem.colour, false);
        }
    }

    public void OnSeekerColourChangeButtonPressed(ColourItem colourItem)
    {
        UIManager.instance.OnBackButtonPressed();

        if (NetworkManager.NetworkType == NetworkType.Client)
        {
            ClientSend.SetPlayerColour(colourItem.colour, true);
        }
        else if (NetworkManager.NetworkType == NetworkType.ClientServer)
        {
            GameManager.instance.AttemptColourChange(Player.LocalPlayer.Id, colourItem.colour, true);
        }
    }
}
