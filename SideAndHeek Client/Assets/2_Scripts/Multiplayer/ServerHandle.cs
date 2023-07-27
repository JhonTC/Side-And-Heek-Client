using Riptide;
using System;
using UnityEngine;
using UnityEngine.Rendering;

public class ServerHandle
{
    [MessageHandler((ushort)ClientToServerId.name)]
    public static void Name(ushort fromClientId, Message message)
    {
        ServerSend.Welcome(fromClientId, "Welcome", GameManager.instance.gameMode.GetGameRules(), GameManager.instance.hiderColours);
        Player.Spawn(fromClientId, message.GetString());
    }

    [MessageHandler((ushort)ClientToServerId.playerInput)]
    public static void SetInputs(ushort fromClientId, Message message)
    {
        Debug.Log($"SetInputs fromClientId: {fromClientId}");

        float inputSpeed = message.GetFloat();
        bool[] _otherInputs = message.GetBools(3);
        Quaternion _rotation = message.GetQuaternion();

        Player player = Player.list[fromClientId];
        if (player.isBodyActive) 
        {
            Debug.Log($"InputSpeed: {inputSpeed}");
            player.playerMotor.SetInputs(inputSpeed, _otherInputs, _rotation);
        }
    }

    [MessageHandler((ushort)ClientToServerId.playerReady)]
    public static void PlayerReady(ushort fromClientId, Message message)
    {
        bool _isReady = message.GetBool();

        Player.list[fromClientId].SetPlayerReady(_isReady);
    }

    [MessageHandler((ushort)ClientToServerId.setPlayerColour)]
    public static void SetPlayerColour(ushort fromClientId, Message message)
    {
        Color _newColour = message.GetColour(); 
        bool _isSeekerColour = message.GetBool();

        GameManager.instance.AttemptColourChange(fromClientId, _newColour, _isSeekerColour);
    }

    [MessageHandler((ushort)ClientToServerId.tryStartGame)]
    public static void TryStartGame(ushort fromClientId, Message message)
    {
        GameManager.instance.TryStartGame(fromClientId);
    }

    [MessageHandler((ushort)ClientToServerId.pickupSelected)]
    public static void PickupSelected(ushort fromClientId, Message message)
    {
        ushort _pickupId = message.GetUShort();

        if (NetworkObjectsManager.networkObjects.ContainsKey(_pickupId))
        {
            Pickup pickup = NetworkObjectsManager.networkObjects[_pickupId] as Pickup;
            if (pickup != null)
            {
                pickup.PickupPickedUp(fromClientId);
            }
            else
            {
                Debug.LogWarning($"ERROR: No pickup with id {_pickupId}");
            }
        } else
        {
            Debug.LogWarning($"ERROR: No pickup with id {_pickupId}");
        }
    }

    [MessageHandler((ushort)ClientToServerId.itemUsed)]
    public static void ItemUsed(ushort fromClientId, Message message)
    {
        if (message.GetBool())
        {
            Player.list[fromClientId].shootDirection = message.GetVector3();
        }

        Player.list[fromClientId].PickupUsed();
    }

    [MessageHandler((ushort)ClientToServerId.gameRulesChanged)]
    public static void GameRulesChanged(ushort fromClientId, Message message)
    {
        GameType gameType = (GameType)message.GetInt();
        if (GameManager.instance.gameType != gameType)
        {
            GameManager.instance.GameTypeChanged(gameType); //todo: this shouldnt be here
        }

        GameRules gameRules = message.GetGameRules();

        GameManager.instance.GameRulesChanged(gameRules);
        ServerSend.GameRulesChanged(gameRules);

        Debug.Log($"Game Rules Changed by player with id {fromClientId}");
    }

    [MessageHandler((ushort)ClientToServerId.requestSceneChange)]
    public static void RequestSceneChange(ushort fromClientId, Message message)
    {
        GameManager.instance.ChangeScene(message.GetString(), message.GetBool());
    }

    [MessageHandler((ushort)ClientToServerId.command)]
    public static void Command(ushort fromClientId, Message message)
    {
        string cmd = message.GetString();
        switch (cmd) //todo: extract into separate commands class
        {
            case "relaunch_server":
                Commands.RelaunchServer();
                break;
        }
    }
}
