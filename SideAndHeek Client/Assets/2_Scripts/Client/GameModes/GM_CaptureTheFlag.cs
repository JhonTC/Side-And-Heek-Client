using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GM_CaptureTheFlag : GameMode
{
    public override void Init()
    {
        friendlyName = "Capture The Flag";
    }

    public override void ReadGameOverMessageValues(Message message)
    {
        //ushort winningTeamId = message.GetUShort();
    }
}
