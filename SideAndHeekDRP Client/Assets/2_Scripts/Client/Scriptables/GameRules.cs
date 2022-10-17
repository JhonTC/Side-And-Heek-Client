using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GameRules", menuName = "Game/GameRules")]
public class GameRules : ScriptableObject
{
    public int id;

    public Map map;

    [Range(60, 360)]
    public int gameLength;
    [Range(0, 3)]
    public int hiderRespawnDelay;

    [HideInInspector] public int numberOfHunters;                     //* - requires more hunter spawns
    public CatchType catchType;
    [Range(0, 60)]
    public int hidingTime;

    public SpeedBoostType speedBoostType;
    [Range(0.8f, 1.2f)]
    public float speedMultiplier;

    public HiderFallRespawnType fallRespawnType;
    public FallRespawnLocation fallRespawnLocation;

    public bool continuousFlop;
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

public enum Map
{
    Map1,
    Map2
}