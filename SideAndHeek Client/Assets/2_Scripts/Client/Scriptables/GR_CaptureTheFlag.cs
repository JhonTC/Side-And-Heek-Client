using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new GR_CaptureTheFlag", menuName = "Game/GameRules/CaptureTheFlag")]
public class GR_CaptureTheFlag : GameRules
{
    [Range(2, 10)]
    public float numberOfTeams = 2;
    [Range(60, 360)]
    public float gameLength = 120;
    [Range(1, 20)]
    public float maxScore = 5;
    public GameEndType gameEndType = GameEndType.Score;
    public CatchType catchType = CatchType.OnFlop;

    public override void UpdateUI(ref Dictionary<int, LocalGameRule> localGameRules)
    {
        localGameRules[0].UpdateUI(numberOfTeams);
        localGameRules[1].UpdateUI(gameLength);
        localGameRules[2].UpdateUI(maxScore);
        localGameRules[3].UpdateUI((int)gameEndType);
        localGameRules[4].UpdateUI((int)catchType);
        localGameRules[5].UpdateUI(continuousFlop);
    }

    public override Message AddMessageValues(Message message)
    {
        base.AddMessageValues(message);

        message.AddInt((int)numberOfTeams);
        message.AddInt((int)gameLength);
        message.AddInt((int)maxScore);
        message.AddInt((int)gameEndType);
        message.AddInt((int)catchType);
        message.AddBool(continuousFlop);

        return message;
    }

    public override void ReadMessageValues(Message message)
    {
        numberOfTeams = message.GetInt();
        gameLength = message.GetInt();
        maxScore = message.GetInt();
        gameEndType = (GameEndType)message.GetInt();
        catchType = (CatchType)message.GetInt();
        continuousFlop = message.GetBool();
    }

    public override void UpdateValues(Dictionary<int, LocalGameRule> localGameRules)
    {
        numberOfTeams = (float)localGameRules[0].value;
        gameLength = (float)localGameRules[1].value;
        maxScore = (float)localGameRules[2].value;
        gameEndType = (GameEndType)localGameRules[3].value;
        catchType = (CatchType)localGameRules[4].value;
        continuousFlop = (bool)localGameRules[5].value;
    }

    public override void SetupUI(Transform parent, UIPanel uiPanel)
    {
        UIUtils.CreateUIForInt(0, "Number Of Teams", numberOfTeams, 2, 10, uiPanel.OnFloatValueChanged, parent);
        UIUtils.CreateUIForInt(1, "Game Length", gameLength, 60, 360, uiPanel.OnFloatValueChanged, parent, "s");
        UIUtils.CreateUIForInt(2, "Max Score", maxScore, 1, 20, uiPanel.OnFloatValueChanged, parent, " caps");
        UIUtils.CreateUIForEnum<GameEndType>(3, "Game End Type", (int)catchType, uiPanel.OnEnumValueChanged, parent);
        UIUtils.CreateUIForEnum<CatchType>(4, "Catch Type", (int)catchType, uiPanel.OnEnumValueChanged, parent);
        UIUtils.CreateUIForBool(5, "Continuous Flop", continuousFlop, uiPanel.OnBoolValueChanged, parent);
    }

    public override Dictionary<string, object> GetListOfValues() //Used in GameManagerEditor
    {
        Dictionary<string, object> retList = new Dictionary<string, object>
        {
            { "Number Of Teams", numberOfTeams },
            { "Game Length", gameLength },
            { "Max Score", maxScore },
            { "Game End Type", gameEndType },
            { "Catch Type", catchType },
            { "Continuous Flop", continuousFlop }
        };

        return retList;
    }
}