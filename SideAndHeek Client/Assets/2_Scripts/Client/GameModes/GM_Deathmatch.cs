using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GM_Deathmatch : GameMode
{
    public override void Init()
    {
        friendlyName = "Deathmatch";
    }

    public override void OnPlayerTypeChanged(Player player)
    {
        if (player.IsLocal || player == LobbyManager.localPlayer.activeSpectatingPlayer) //second will only be true if local player is already spectating (activeSpectatingPlayer == null otherwise)
        {
            if (player.playerType == PlayerType.Spectator) //todo: doesnt check if player is already a spectator...
            {
                Player spectatePlayer = null;

                foreach (Player otherPlayer in LobbyManager.players.Values)
                {
                    if (otherPlayer.playerType != PlayerType.Spectator)
                    {
                        spectatePlayer = otherPlayer;
                        LobbyManager.localPlayer.activeSpectatingPlayer = spectatePlayer;
                        UIManager.instance.DisplayPanel(UIPanelType.Spectate, true);
                        break;
                    }
                }

                LobbyManager.localPlayer.SpectatePlayer(spectatePlayer);
            } 
            else if (player.lastPlayerType == PlayerType.Spectator)
            {
                LobbyManager.localPlayer.SpectatePlayer();
            }
        }
    }

    public override void ReadGameOverMessageValues(Message message)
    {

    }
}
