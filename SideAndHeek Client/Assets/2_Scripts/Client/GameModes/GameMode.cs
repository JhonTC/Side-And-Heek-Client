using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameMode
{
    public string friendlyName;

    public virtual void Init()
    {

    }
    public virtual void OnPlayerTypeChanged(Player player)
    {

    }

    public virtual void ReadGameOverMessageValues(Message message)
    {

    }

    public static GameMode CreateGameModeFromType(GameType _gameType)
    {
        GameMode gameMode = null;
        switch (_gameType)
        {
            case GameType.HideAndSeek:
                gameMode = new GM_HideAndSeek();
                break;
            case GameType.Deathmatch:
                gameMode = new GM_Deathmatch();
                break;
            case GameType.CaptureTheFlag:
                gameMode = new GM_CaptureTheFlag();
                break;
        }
        gameMode.Init();

        return gameMode;
    }
}
