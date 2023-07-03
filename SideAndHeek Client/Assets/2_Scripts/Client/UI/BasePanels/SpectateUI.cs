using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpectateUI : UIPanel
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    public TMP_Text playerNameField;

    private Player ActiveSpectatingPlayer 
    { 
        get 
        { 
            if (Player.LocalPlayer != null)
            {
                return Player.LocalPlayer.activeSpectatingPlayer;
            }

            return null; 
        }
    }

    public override void EnablePanel()
    {
        base.EnablePanel();

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (ActiveSpectatingPlayer != null)
        {
            playerNameField.text = ActiveSpectatingPlayer.Username;
        }

        int specatatablePlayersCount = 0;
        foreach (Player player in Player.list.Values)
        {
            if (player.playerType != PlayerType.Spectator)
            {
                specatatablePlayersCount++;
            }
        }

        nextButton.interactable = specatatablePlayersCount > 1;
        previousButton.interactable = specatatablePlayersCount > 1;
    }

    public void OnNextButtonPressed()
    {
        List<Player> spectatablePlayers = new List<Player>();
        int activeSpectatingPlayerIndex = 0;

        foreach (Player player in Player.list.Values)
        {
            if (player.playerType != PlayerType.Spectator)
            {
                spectatablePlayers.Add(player);

                if (player == Player.LocalPlayer.activeSpectatingPlayer)
                {
                    activeSpectatingPlayerIndex = spectatablePlayers.Count - 1;
                }
            }
        }

        if (spectatablePlayers.Count > 0)
        {
            activeSpectatingPlayerIndex++;
            if (activeSpectatingPlayerIndex >= spectatablePlayers.Count)
            {
                activeSpectatingPlayerIndex = 0;
            }

            Player.LocalPlayer.SpectatePlayer(spectatablePlayers[activeSpectatingPlayerIndex]);

            UpdateUI();
        } else
        {
            Debug.Log("NOBODY AVALIABLE TO SPECTATE");
        }
    }

    public void OnPreviousButtonPressed()
    {
        List<Player> spectatablePlayers = new List<Player>();
        int activeSpectatingPlayerIndex = 0;

        foreach (Player player in Player.list.Values)
        {
            if (player.playerType != PlayerType.Spectator)
            {
                spectatablePlayers.Add(player);

                if (player == Player.LocalPlayer.activeSpectatingPlayer)
                {
                    activeSpectatingPlayerIndex = spectatablePlayers.Count - 1;
                }
            }
        }

        if (spectatablePlayers.Count > 0)
        {
            activeSpectatingPlayerIndex--;
            if (activeSpectatingPlayerIndex < 0)
            {
                activeSpectatingPlayerIndex = spectatablePlayers.Count - 1;
            }

            Player.LocalPlayer.SpectatePlayer(spectatablePlayers[activeSpectatingPlayerIndex]);

            UpdateUI();
        }
        else
        {
            Debug.Log("NOBODY AVALIABLE TO SPECTATE");
        }
    }
}
