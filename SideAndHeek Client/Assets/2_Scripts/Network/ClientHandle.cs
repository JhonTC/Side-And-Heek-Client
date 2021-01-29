﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);

        UIManager.instance.DisplayLobbyPanel();
    }

    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        bool _isReady = _packet.ReadBool();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        
        Debug.Log($"Message from server: Spawn Player with id: {_id}");
        GameManager.instance.SpawnPlayer(_id, _username, _isReady,_position, _rotation);

        UIManager.instance.AddPlayerReady(_id);
    }

    public static void PlayerPositions(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _headPosition = _packet.ReadVector3();
        Vector3 _rightFootPosition = _packet.ReadVector3();
        Vector3 _leftFootPosition = _packet.ReadVector3();
        Vector3 _rightLegPosition = _packet.ReadVector3();
        Vector3 _leftLegPosition = _packet.ReadVector3();

        Quaternion _rightFootRotation = _packet.ReadQuaternion();
        Quaternion _leftFootRotation = _packet.ReadQuaternion();
        Quaternion _rightLegRotation = _packet.ReadQuaternion();
        Quaternion _leftLegRotation = _packet.ReadQuaternion();

        GameManager.players[_id].SetPlayerPositions(_headPosition, _rightFootPosition, _leftFootPosition, _rightLegPosition, _leftLegPosition);
        GameManager.players[_id].SetPlayerRotations(_rightFootRotation, _leftFootRotation, _rightLegRotation, _leftLegRotation);
    }

    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rootRotation = _packet.ReadQuaternion();
        
        GameManager.players[_id].SetRootRotation(_rootRotation);
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();
        
        UIManager.instance.RemovePlayerReady(_id);

        Destroy(GameManager.players[_id].gameObject);
        GameManager.players.Remove(_id);
    }

    public static void CreateItemSpawner(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        TaskCode _taskCode = (TaskCode)_packet.ReadInt();
        string _taskName = _packet.ReadString();
        string _taskContent = _packet.ReadString();
        Color _taskDifficulty = _packet.ReadColour();

        bool _hasItem = _taskCode != TaskCode.NULL_TASK;

        GameManager.instance.CreateItemSpawner(_spawnerId, _position, _hasItem, _taskCode, _taskName, _taskContent, _taskDifficulty);
    }

    public static void ItemSpawned(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();
        TaskCode _taskCode = (TaskCode)_packet.ReadInt();
        string _taskName = _packet.ReadString();
        string _taskContent = _packet.ReadString();
        Color _taskDifficulty = _packet.ReadColour();

        GameManager.itemSpawners[_spawnerId].ItemSpawned(_taskCode, _taskName, _taskContent, _taskDifficulty);
    }

    public static void ItemPickedUp(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();
        int _byPlayer = _packet.ReadInt();
        TaskCode _code = (TaskCode)_packet.ReadInt();

        GameManager.itemSpawners[_spawnerId].ItemPickedUp();
        GameManager.players[_byPlayer].ItemPickedUp(_code);
    }

    public static void PlayerReadyToggled(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        bool _isReady = _packet.ReadBool();

        GameManager.players[_playerId].SetPlayerReady(_isReady);
    }

    public static void ChangeScene(Packet _packet)
    {
        string _sceneToLoad = _packet.ReadString();

        GameManager.instance.LoadScene(_sceneToLoad, LoadSceneMode.Additive);
    }

    public static void UnloadScene(Packet _packet)
    {
        string _sceneToLoad = _packet.ReadString();

        GameManager.instance.UnloadScene(_sceneToLoad);
    }

    public static void SetPlayerType(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        PlayerType _playerType = (PlayerType)_packet.ReadInt();
        
        GameManager.players[_playerId].SetPlayerType(_playerType);
    }

    private static bool lastIsCountdownActive = false;
    public static void SetSpecialCountdown(Packet _packet)
    {
        int _specialId = _packet.ReadInt();
        int _countdownValue = _packet.ReadInt();
        bool _isCountdownActive = _packet.ReadBool();
        
        if (_specialId == Client.instance.myId)
        {
            GameManager.players[_specialId].SetCountdown(_countdownValue);
        }
        else if (lastIsCountdownActive != _isCountdownActive)
        {
            if (_isCountdownActive)
            {
                GameManager.players[Client.instance.myId].SetSpecialMessage($"{GameManager.players[_specialId].username} is the hunter... Hide!");
            }
            else
            {
                GameManager.players[Client.instance.myId].SetSpecialMessage($"{GameManager.players[_specialId].username} has been released");
            }

            lastIsCountdownActive = _isCountdownActive;
        }
    }

    public static void SetPlayerColour(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        Color _colour = _packet.ReadColour();
        bool _isSeekerColour = _packet.ReadBool();

        GameManager.players[_playerId].ChangeBodyColour(_colour, _isSeekerColour);
    }

    public static void RecieveErrorResponse(Packet _packet)
    {
        ErrorResponseCode responseCode = (ErrorResponseCode)_packet.ReadInt();

        ErrorResponseHandler.HandleErrorResponse(responseCode);
    }

    public static void GameOver(Packet _packet)
    {
        bool isHunterVictory = _packet.ReadBool();

        GameManager.instance.gameStarted = false;

        Debug.Log("Game Over!");

        //disable input
        //show gameover panel
        //enable option to go back to the lobby 

        /*  - what happens to the other players?
            - stop receiveing input and teleport them all to the starting spawnpoints
                - what about one player moving the others?
                - lock the players in place?
        */

    }

    public static void PlayerTeleported(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        Vector3 _teleportPosition = _packet.ReadVector3();

        GameManager.players[_playerId].PlayerTeleported(_teleportPosition);
    }

    public static void TaskProgressed(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        TaskCode _code = (TaskCode)_packet.ReadInt();
        float _progression = _packet.ReadFloat();

        GameManager.players[_playerId].TaskProgressed(_code, _progression);
    }

    public static void TaskComplete(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        TaskCode _code = (TaskCode)_packet.ReadInt();

        GameManager.players[_playerId].TaskComplete(_code);
    }
}
