using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GM_Deathmatch : GameMode
{
    private GR_Deathmatch CustomGameRules => gameRules as GR_Deathmatch;

    private string spectatorSceneName = "Lobby";
    public Transform shrinkingArea;

    public override void Init()
    {
        friendlyName = "Deathmatch";
        sceneName = "Map_2";
    }

    public override void OnPlayerTypeChanged(Player player)
    {
        if (player.IsLocal || player == Player.LocalPlayer.activeSpectatingPlayer) //second will only be true if local player is already spectating (activeSpectatingPlayer == null otherwise)
        {
            if (player.playerType == PlayerType.Spectator) //todo: doesnt check if player is already a spectator...
            {
                Player spectatePlayer = null;

                foreach (Player otherPlayer in Player.list.Values)
                {
                    if (otherPlayer.playerType != PlayerType.Spectator)
                    {
                        spectatePlayer = otherPlayer;
                        Player.LocalPlayer.activeSpectatingPlayer = spectatePlayer;
                        UIManager.instance.DisplayPanel(UIPanelType.Spectate, true);
                        break;
                    }
                }

                Player.LocalPlayer.SpectatePlayer(spectatePlayer);
            } 
            else if (player.lastPlayerType == PlayerType.Spectator)
            {
                Player.LocalPlayer.SpectatePlayer();
            }
        }
    }

    public override void ReadGameOverMessageValues(Message message)
    {

    }

    public override void SetGameRules(GameRules _gameRules)
    {
        GR_Deathmatch newGameRules = _gameRules as GR_Deathmatch;

        if (newGameRules != null)
        {
            gameRules = newGameRules;
        }
    }

    public override void GameStart()
    {
        GameObject objectSA = GameObject.FindGameObjectWithTag("ShrinkingArea");
        if (objectSA)
        {
            shrinkingArea = objectSA.transform;
        }

        foreach (Player player in Player.list.Values)
        {
            player.TeleportPlayer(LevelManager.GetLevelManagerForScene(GameManager.instance.activeSceneName).GetNextSpawnpoint(false));
        }

        ServerSend.GameStarted();
        GameModeUtils.StartGameTimer(GameManager.instance.GameOver, (int)CustomGameRules.gameLength);
    }

    public override void FixedUpdate()
    {
        if (shrinkingArea.localScale.x >= 0.4f)
        {
            shrinkingArea.localScale -= new Vector3(CustomGameRules.shrinkSpeed, 0, CustomGameRules.shrinkSpeed) * Time.fixedDeltaTime;
        }

        //NEED TO SEND NEW SCALE TO CLIENTS
    }

    public override void GameOver()
    {
        CalculateWinners();
    }

    public override void AddGameStartMessageValues(ref Message message)
    {
        message.AddInt((int)CustomGameRules.gameLength);
    }

    public override void AddGameOverMessageValues(ref Message message)
    {

    }

    public override bool CheckForGameOver()
    {
        bool isGameOver = false;

        if (GameManager.instance.gameStarted)
        {
            int defaultCount = 0;

            foreach (Player player in Player.list.Values)
            {
                if (player.playerType == PlayerType.Default)
                {
                    defaultCount++;
                }
            }

            isGameOver = defaultCount <= 1;
        }

        return isGameOver;
    }

    private void CalculateWinners()
    {
        //todo: find remaining player

        foreach (Player player in Player.list.Values)
        {
            if (player.playerType == PlayerType.Default)
            {
                Debug.Log($"Game Over, {player.Username} Wins!");
                break;
            }
        }
    }

    public override void OnPlayerCollision(Player player, Player other)
    {
        if (other.playerType == player.playerType)
        {
            if (player.playerMotor.GetCanKnockOutOthers())
            {
                if (other.isBodyActive)
                {
                    other.playerMotor.OnCollisionWithOther(3f); //todo:promote
                }
            }
        }
    }

    public override void OnPlayerHitFallDetector(Player player)
    {
        player.SetPlayerType(PlayerType.Spectator);
        player.TeleportPlayer(LevelManager.GetLevelManagerForScene(spectatorSceneName).GetNextSpawnpoint(true));

        if (CheckForGameOver())
        {
            GameManager.instance.GameOver();
        }
    }

    public override void OnPlayerTypeSet(Player player, PlayerType playerType, bool isFirstHunter)
    {

    }
}
