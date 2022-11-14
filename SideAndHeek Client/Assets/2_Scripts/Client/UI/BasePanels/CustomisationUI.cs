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
        UIManager.instance.CloseHistoryPanels();

        if (GameManager.instance.gameType == GameType.Multiplayer)
        {
            ClientSend.SetPlayerColour(colourItem.colour, false);
        }
        else
        {
            LobbyManager.localPlayer.ChangeBodyColour(colourItem.colour, false);
        }
    }

    public void OnSeekerColourChangeButtonPressed(ColourItem colourItem)
    {
        UIManager.instance.CloseHistoryPanels();

        LobbyManager.localPlayer.ChangeBodyColour(colourItem.colour, true);

        if (GameManager.instance.gameType == GameType.Multiplayer)
        {
            ClientSend.SetPlayerColour(colourItem.colour, true);
        }
    }
}
