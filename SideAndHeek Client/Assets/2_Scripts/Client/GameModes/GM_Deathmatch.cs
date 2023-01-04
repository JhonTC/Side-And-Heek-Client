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

    public override void ReadGameOverMessageValues(Message message)
    {

    }
}
