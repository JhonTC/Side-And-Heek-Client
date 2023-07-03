using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new GR_Deathmatch", menuName = "Game/GameRules/Deathmatch")]

public class GR_Deathmatch : GameRules
{
    [Range(10, 360)]
    public float gameLength = 180;
    [Range(0, 10)]
    public float playerLives = 0;
    [Range(0, 0.1f)]
    public float shrinkSpeed = 0.005f;

    public override void UpdateUI(ref Dictionary<int, LocalGameRule> localGameRules)
    {
        localGameRules[0].UpdateUI(gameLength);
        localGameRules[1].UpdateUI(playerLives);
        localGameRules[2].UpdateUI(continuousFlop);
    }

    public override Message AddMessageValues(Message message)
    {
        base.AddMessageValues(message);

        message.AddInt((int)gameLength);
        message.AddInt((int)playerLives);
        message.AddBool(continuousFlop);

        return message;
    }

    public override void ReadMessageValues(Message message)
    {
        gameLength = message.GetInt();
        playerLives = message.GetInt();
        continuousFlop = message.GetBool();
    }

    public override void UpdateValues(Dictionary<int, LocalGameRule> localGameRules)
    {
        gameLength = (float)localGameRules[0].value;
        playerLives = (float)localGameRules[1].value;
        continuousFlop = (bool)localGameRules[2].value;
    }

    public override void SetupUI(Transform parent, UIPanel uiPanel)
    {
        UIUtils.CreateUIForInt(     0, "Game Length", gameLength, 10, 360, uiPanel.OnFloatValueChanged, parent, "s");
        UIUtils.CreateUIForInt(     1, "Player Lives", playerLives, 1, 2, uiPanel.OnFloatValueChanged, parent);
        UIUtils.CreateUIForBool(    2, "Continuous Flop", continuousFlop, uiPanel.OnBoolValueChanged, parent);
    }

    public override Dictionary<string, object> GetListOfValues() //Used in GameManagerEditor
    {
        Dictionary<string, object> retList = new Dictionary<string, object>
        {
            { "Game Length", gameLength },
            { "Player Lives", playerLives },
            { "Continuous Flop", continuousFlop }
        };

        return retList;
    }
}
