using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomisationUI : UIPanel
{
    public ColourSelectorUI hiderColourSelector;

    public void OnHiderColourChangeButtonPressed(ColourItem colourItem)
    {
        UIManager.instance.CloseHistoryPanels();

        if (GameManager.instance.gameType == GameType.Multiplayer)
        {
            ClientSend.SetPlayerColour(colourItem.colour, false);
        }
        else
        {
            LobbyManager.instance.GetLocalPlayer().ChangeBodyColour(colourItem.colour, false);
        }
    }

    public void OnSeekerColourChangeButtonPressed(ColourItem colourItem)
    {
        UIManager.instance.CloseHistoryPanels();

        LobbyManager.instance.GetLocalPlayer().ChangeBodyColour(colourItem.colour, true);

        if (GameManager.instance.gameType == GameType.Multiplayer)
        {
            ClientSend.SetPlayerColour(colourItem.colour, true);
        }
    }
}
