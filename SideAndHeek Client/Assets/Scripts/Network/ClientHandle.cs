using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

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
        bool _hasItem = _packet.ReadBool();

        GameManager.instance.CreateItemSpawner(_spawnerId, _position, _hasItem);
    }

    public static void ItemSpawned(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();

        GameManager.itemSpawners[_spawnerId].ItemSpawned();
    }

    public static void ItemPickedUp(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();
        int _byPlayer = _packet.ReadInt();

        GameManager.itemSpawners[_spawnerId].ItemPickedUp();
        GameManager.players[_byPlayer].itemCount++;
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

        GameManager.instance.LoadScene(_sceneToLoad);
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
}
