using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRules : ScriptableObject
{
    public ushort id;
    public GameType gameType;
    public string friendlyName;

    public virtual void UpdateUI(ref Dictionary<int, LocalGameRule> localGameRules) {}

    public virtual Message AddMessageValues(Message message)
    {
        message.AddInt((int)gameType);

        return message;
    }

    public virtual void ReadMessageValues(Message message) {}

    public virtual void UpdateValues(Dictionary<int, LocalGameRule> localGameRules) {}

    public static GameRules CreateGameRulesFromType(GameType _gameType)
    {
        GameRules gameRules = null;
        switch (_gameType)
        {
            case GameType.HideAndSeek:
                gameRules = CreateInstance<GR_HideAndSeek>();
                break;
            case GameType.Deathmatch:
                gameRules = CreateInstance<GR_Deathmatch>();
                break;
            case GameType.CaptureTheFlag:
                gameRules = CreateInstance<GR_CaptureTheFlag>();
                break;
        }

        if (gameRules != null)
        {
            gameRules.gameType = _gameType;
        }

        return gameRules;
    }

    public virtual void SetupUI(Transform parent, UIPanel uiPanel) {}
}

public enum GameType
{
    HideAndSeek,
    Deathmatch,
    CaptureTheFlag
}

public enum CatchType
{
    OnFlop,
    OnTouch
}

public enum SpeedBoostType
{
    FirstHunter,
    AllHunters,
    None
}

public enum FallRespawnLocation
{
    Centre,
    Random
}

public enum HiderFallRespawnType
{
    Hider,
    Hunter,
    Spectator
}

public enum GameEndType
{
    Time,
    Score
}