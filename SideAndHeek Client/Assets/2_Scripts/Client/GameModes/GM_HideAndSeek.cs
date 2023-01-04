using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GM_HideAndSeek : GameMode
{
    public override void Init()
    {
        friendlyName = "Hide And Seek";
    }

    public override void ReadGameOverMessageValues(Message message)
    {
        bool isHunterVictory = message.GetBool();
    }
}
