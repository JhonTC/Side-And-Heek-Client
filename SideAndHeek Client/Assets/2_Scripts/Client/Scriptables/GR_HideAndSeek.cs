using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new GR_HideAndSeek", menuName = "Game/GameRules/HideAndSeek")]
public class GR_HideAndSeek : GameRules
{
    [Range(60, 360)]
    public float gameLength = 180;

    [Range(1, 1)]
    public float numberOfHunters = 1;                     //* - requires more hunter spawns
    public CatchType catchType = CatchType.OnTouch;
    [Range(0, 60)]
    public float hidingTime = 20;

    public SpeedBoostType speedBoostType = SpeedBoostType.FirstHunter;
    [Range(0.8f, 1.2f)]
    public float speedMultiplier = 1.1f;

    public HiderFallRespawnType fallRespawnType = HiderFallRespawnType.Hider;
    public FallRespawnLocation fallRespawnLocation = FallRespawnLocation.Centre;

    public bool continuousFlop = false;

    public override void UpdateUI(ref Dictionary<int, LocalGameRule> localGameRules)
    {
        localGameRules[0].UpdateUI(gameLength);
        localGameRules[1].UpdateUI(numberOfHunters);
        localGameRules[2].UpdateUI((int)catchType);
        localGameRules[3].UpdateUI(hidingTime);
        localGameRules[4].UpdateUI((int)speedBoostType);
        localGameRules[5].UpdateUI(speedMultiplier);
        localGameRules[6].UpdateUI((int)fallRespawnType);
        localGameRules[7].UpdateUI((int)fallRespawnLocation);
        localGameRules[8].UpdateUI(continuousFlop);
    }

    public override Message AddMessageValues(Message message)
    {
        base.AddMessageValues(message);

        message.AddInt((int)gameLength);
        message.AddInt((int)numberOfHunters);
        message.AddInt((int)catchType);
        message.AddInt((int)hidingTime);
        message.AddInt((int)speedBoostType);
        message.AddFloat(speedMultiplier);
        message.AddInt((int)fallRespawnType);
        message.AddInt((int)fallRespawnLocation);
        message.AddBool(continuousFlop);

        return message;
    }

    public override void ReadMessageValues(Message message)
    {
        gameLength = message.GetInt();
        numberOfHunters = message.GetInt();
        catchType = (CatchType)message.GetInt();
        hidingTime = message.GetInt();
        speedBoostType = (SpeedBoostType)message.GetInt();
        speedMultiplier = message.GetFloat();
        fallRespawnType = (HiderFallRespawnType)message.GetInt();
        fallRespawnLocation = (FallRespawnLocation)message.GetInt();
        continuousFlop = message.GetBool();
    }

    public override void UpdateValues(Dictionary<int, LocalGameRule> localGameRules)
    {
        gameLength = (float)localGameRules[0].value;
        numberOfHunters = (float)localGameRules[1].value;
        catchType = (CatchType)localGameRules[2].value;
        hidingTime = (float)localGameRules[3].value;
        speedBoostType = (SpeedBoostType)localGameRules[4].value;
        speedMultiplier = (float)localGameRules[5].value;
        fallRespawnType = (HiderFallRespawnType)localGameRules[6].value;
        fallRespawnLocation = (FallRespawnLocation)localGameRules[7].value;
        continuousFlop = (bool)localGameRules[8].value;
    }

    public override void SetupUI(Transform parent, UIPanel uiPanel)
    {
        UIUtils.CreateUIForInt(                          0, "Game Length", gameLength, 60, 360, uiPanel.OnFloatValueChanged, parent, "s");
        UIUtils.CreateUIForInt(                          1, "Number Of Hunters", numberOfHunters, 1, 2, uiPanel.OnFloatValueChanged, parent);
        UIUtils.CreateUIForEnum<CatchType>(              2, "Catch Type", (int)catchType, uiPanel.OnEnumValueChanged, parent);
        UIUtils.CreateUIForInt(                          3, "Hiding Time", hidingTime, 0, 60, uiPanel.OnFloatValueChanged, parent, "s");
        UIUtils.CreateUIForEnum<SpeedBoostType>(         4, "Speed Boost Type", (int)speedBoostType, uiPanel.OnEnumValueChanged, parent);
        UIUtils.CreateUIForFloat(                        5, "Speed Multiplier", speedMultiplier, 0.8f, 1.2f, uiPanel.OnFloatValueChanged, parent, "x");
        UIUtils.CreateUIForEnum<HiderFallRespawnType>(   6, "Fall Respawn Type", (int)fallRespawnType, uiPanel.OnEnumValueChanged, parent);
        UIUtils.CreateUIForEnum<FallRespawnLocation>(    7, "Fall Respawn Location", (int)fallRespawnLocation, uiPanel.OnEnumValueChanged, parent);
        UIUtils.CreateUIForBool(                         8, "Continuous Flop", continuousFlop, uiPanel.OnBoolValueChanged, parent);
    }
}
