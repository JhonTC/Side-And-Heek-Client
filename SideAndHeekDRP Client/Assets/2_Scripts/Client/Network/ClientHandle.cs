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
        LobbyManager.instance.SpawnPlayer(_id, _username, _isReady, _id == Client.instance.myId, _position, _rotation);

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

        if (LobbyManager.players.ContainsKey(_id))
        {
            LobbyManager.players[_id].SetPlayerPositions(_headPosition, _rightFootPosition, _leftFootPosition, _rightLegPosition, _leftLegPosition);
            LobbyManager.players[_id].SetPlayerRotations(_rightFootRotation, _leftFootRotation, _rightLegRotation, _leftLegRotation);
        } else
        {
            Debug.Log($"No player with id {_id}");
        }
    }

    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rootRotation = _packet.ReadQuaternion();

        if (LobbyManager.players.ContainsKey(_id))
        {
            LobbyManager.players[_id].SetRootRotation(_rootRotation);
        }
        else
        {
            Debug.Log($"No player with id {_id}");
        }
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();
        
        UIManager.instance.RemovePlayerReady(_id);

        if (LobbyManager.players.ContainsKey(_id))
        {
            if (Client.instance.myId != _id)
            {
                Destroy(LobbyManager.players[_id].gameObject);
                LobbyManager.players.Remove(_id);
            }
            else
            {
                LobbyManager.instance.OnLocalPlayerDisconnection();
            }
        }
        else
        {
            Debug.Log($"No player with id {_id}");
        }
    }

    public static void CreatePickupSpawner(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        PickupType _pickupType = (PickupType)_packet.ReadInt();
        bool _hasPickup = _packet.ReadBool();

        int _code = _packet.ReadInt();
        string _pickupName = _packet.ReadString();
        string _pickupContent = _packet.ReadString();
        Color _pickupLevel = _packet.ReadColour();

        GameManager.instance.CreatePickupSpawner(_spawnerId, _position, _pickupType, _hasPickup, _code, _pickupName, _pickupContent, _pickupLevel);
    }

    public static void PickupSpawned(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();
        PickupType _pickupType = (PickupType)_packet.ReadInt();

        int _code = _packet.ReadInt();
        string _pickupName = _packet.ReadString();
        string _pickupContent = _packet.ReadString();
        Color _pickupLevel = _packet.ReadColour();

        GameManager.pickupSpawners[_spawnerId].PickupSpawned(_pickupType, _code);
    }

    public static void PickupPickedUp(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();
        int _byPlayer = _packet.ReadInt();
        PickupType _pickupType = (PickupType)_packet.ReadInt();
        int _code = _packet.ReadInt();

        GameManager.pickupSpawners[_spawnerId].ItemPickedUp();

        BasePickup pickup = null;
        if (_pickupType == PickupType.Task)
        {
            pickup = GameManager.instance.collection.GetTaskByCode((TaskCode)_code).task;
        }
        else if (_pickupType == PickupType.Item)
        {
            pickup = GameManager.instance.collection.GetItemByCode((ItemCode)_code).item;
        }

        if (LobbyManager.players.ContainsKey(_byPlayer))
        {
            LobbyManager.players[_byPlayer].PickupPickedUp(pickup);
        }
        else
        {
            Debug.Log($"No player with id {_byPlayer}");
        }
    }

    public static void PlayerReadyToggled(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        bool _isReady = _packet.ReadBool();

        if (LobbyManager.players.ContainsKey(_playerId))
        {
            LobbyManager.players[_playerId].SetPlayerReady(_isReady);
        }
        else
        {
            Debug.Log($"No player with id {_playerId}");
        }
    }

    public static void ChangeScene(Packet _packet)
    {
        string _sceneToLoad = _packet.ReadString();

        GameManager.instance.LoadScene(_sceneToLoad, LoadSceneMode.Additive);
    }

    public static void UnloadScene(Packet _packet)
    {
        string _sceneToLoad = _packet.ReadString();

        GameManager.instance.UnloadScene(_sceneToLoad, false);
    }

    public static void SetPlayerType(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        PlayerType _playerType = (PlayerType)_packet.ReadInt();

        if (LobbyManager.players.ContainsKey(_playerId))
        {
            LobbyManager.players[_playerId].SetPlayerType(_playerType);
        }
        else
        {
            Debug.Log($"No player with id {_playerId}");
        }
    }

    private static bool lastIsCountdownActive = false;
    public static void SetSpecialCountdown(Packet _packet)
    {
        int _specialId = _packet.ReadInt();
        int _countdownValue = _packet.ReadInt();
        bool _isCountdownActive = _packet.ReadBool();
        
        if (_specialId == Client.instance.myId)
        {
            if (LobbyManager.players.ContainsKey(_specialId))
            {
                UIManager.instance.SetCountdown(_countdownValue);
            }
            else
            {
                Debug.Log($"No player with id {_specialId}");
            }
        }
        else if (lastIsCountdownActive != _isCountdownActive)
        {
            if (_isCountdownActive)
            {
                UIManager.instance.SetSpecialMessage($"{LobbyManager.players[_specialId].username} is the hunter... Hide!");
            }
            else
            {
                UIManager.instance.SetSpecialMessage($"{LobbyManager.players[_specialId].username} has been released");
            }

            lastIsCountdownActive = _isCountdownActive;
        }
    }

    public static void SetPlayerColour(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        Color _colour = _packet.ReadColour();
        bool _isSeekerColour = _packet.ReadBool();

        if (LobbyManager.players.ContainsKey(_playerId))
        {
            LobbyManager.players[_playerId].ChangeBodyColour(_colour, _isSeekerColour);
        }
        else
        {
            Debug.Log($"No player with id {_playerId}");
        }
    }

    public static void RecieveErrorResponse(Packet _packet)
    {
        ErrorResponseCode responseCode = (ErrorResponseCode)_packet.ReadInt();

        ErrorResponseHandler.HandleErrorResponse(responseCode);
    }

    public static void GameStart(Packet _packet)
    {
        int _gameDuration = _packet.ReadInt();

        Debug.Log("Game Start");
        LobbyManager.instance.tryStartGameActive = false;
        UIManager.instance.DisableAllPanels();
        GameManager.instance.StartGameTimer(_gameDuration);
    }

    public static void GameOver(Packet _packet)
    {
        bool isHunterVictory = _packet.ReadBool();

        GameManager.instance.gameStarted = false;

        foreach (PlayerManager player in LobbyManager.players.Values)
        {
            player.GameOver();
        }

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

        if (LobbyManager.players.ContainsKey(_playerId))
        {
            LobbyManager.players[_playerId].PlayerTeleported(_teleportPosition);
        }
        else
        {
            Debug.Log($"No player with id {_playerId}");
        }
    }

    public static void TaskProgressed(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        TaskCode _code = (TaskCode)_packet.ReadInt();
        float _progression = _packet.ReadFloat();

        if (LobbyManager.players.ContainsKey(_playerId))
        {
            LobbyManager.players[_playerId].TaskProgressed(_code, _progression);
        }
        else
        {
            Debug.Log($"No player with id {_playerId}");
        }
    }

    public static void TaskComplete(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        TaskCode _code = (TaskCode)_packet.ReadInt();

        if (LobbyManager.players.ContainsKey(_playerId))
        {
            LobbyManager.players[_playerId].TaskComplete(_code);
        }
        else
        {
            Debug.Log($"No player with id {_playerId}");
        }
    }

    public static void GameRulesChanged(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        GameRules gameRules = _packet.ReadGameRules();

        Debug.Log($"Game Rules Changed by player with id {_playerId}");
    }
}
