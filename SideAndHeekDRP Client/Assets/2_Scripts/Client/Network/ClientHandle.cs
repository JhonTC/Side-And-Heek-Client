using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        Client.instance.isConnected = true;

        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        GameRules _gameRules = _packet.ReadGameRules();

        int _hiderColourCount = _packet.ReadInt();
        Color[] _hiderColours = new Color[_hiderColourCount];
        for (int i = 0; i < _hiderColours.Length; i++)
        {
            _hiderColours[i] = _packet.ReadColour();
        }

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);

        GameManager.instance.gameRules = _gameRules;
        GameManager.instance.hiderColours = _hiderColours;
        GameManager.instance.FadeMusic(true);

        UIManager.instance.DisplayGameplayPanel();
        UIManager.instance.customisationPanel.hiderColourSelector.Init(_hiderColours);
        UIManager.instance.gameRulesPanel.SetGameRules(_gameRules);

        ClientSend.WelcomeReceived();
    }

    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        bool _isReady = _packet.ReadBool();
        bool _isHost = _packet.ReadBool();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        Color _colour = _packet.ReadColour();

        bool _hasAuthority = _id == Client.instance.myId;

        Debug.Log($"Message from server: Spawn Player with id: {_id}");
        LobbyManager.instance.SpawnPlayer(_id, _username, _isReady, _hasAuthority, _isHost, _position, _rotation, _colour);

        UIManager.instance.AddPlayerReady(_id);
    }

    public static void PlayerPositions(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _headPosition = _packet.ReadVector3();
        /*Vector3 _rightFootPosition = _packet.ReadVector3();
        Vector3 _leftFootPosition = _packet.ReadVector3();
        Vector3 _rightLegPosition = _packet.ReadVector3();
        Vector3 _leftLegPosition = _packet.ReadVector3();

        Quaternion _rightFootRotation = _packet.ReadQuaternion();
        Quaternion _leftFootRotation = _packet.ReadQuaternion();
        Quaternion _rightLegRotation = _packet.ReadQuaternion();
        Quaternion _leftLegRotation = _packet.ReadQuaternion();*/

        if (LobbyManager.players.ContainsKey(_id))
        {
            LobbyManager.players[_id].SetPlayerPositions(_headPosition /*, _rightFootPosition, _leftFootPosition, _rightLegPosition, _leftLegPosition*/);
            //LobbyManager.players[_id].SetPlayerRotations(_rightFootRotation, _leftFootRotation, _rightLegRotation, _leftLegRotation);
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

    public static void PlayerState(Packet _packet)
    {
        int _id = _packet.ReadInt();

        bool _isGrounded = _packet.ReadBool();
        float _inputSpeed = _packet.ReadFloat();

        bool _isJumping = _packet.ReadBool();
        bool _isFlopping = _packet.ReadBool();
        bool _isSneaking = _packet.ReadBool();

        if (LobbyManager.players.ContainsKey(_id))
        {
            LobbyManager.players[_id].SetPlayerState(_isGrounded, _inputSpeed, _isJumping, _isFlopping, _isSneaking);
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
                UIManager.instance.customisationPanel.hiderColourSelector.UpdateAllButtons();
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

        UIManager.instance.gameplayPanel.UpdatePlayerTypeViews();
    }

    public static void CreatePickupSpawner(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        GameManager.instance.CreatePickupSpawner(_spawnerId, _position);
    }

    public static void PickupSpawned(Packet _packet)
    {
        int _pickupId = _packet.ReadInt();
        bool _bySpawner = _packet.ReadBool();
        int _creatorId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        int _code = _packet.ReadInt();

        if (!PickupHandler.pickups.ContainsKey(_pickupId))
        {
            if (_bySpawner)
            {
                if (GameManager.pickupSpawners.ContainsKey(_creatorId))
                {
                    GameManager.pickupSpawners[_creatorId].PickupSpawned(_pickupId, _creatorId, _code, _position, _rotation); //todo these can be merged into one
                }
                else
                {
                    Debug.Log($"No spawner with id {_creatorId}");
                }
            }
            else
            {
                if (LobbyManager.players.ContainsKey(_creatorId))
                {
                    LobbyManager.players[_creatorId].PickupSpawned(_pickupId, _creatorId, _code, _position, _rotation);
                }
                else
                {
                    Debug.Log($"No player with id {_creatorId}");
                }
            }
        }
    }

    public static void ItemSpawned(Packet _packet)
    {
        int _itemId = _packet.ReadInt();
        int _creatorId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        int _code = _packet.ReadInt();

        if (!ItemHandler.items.ContainsKey(_itemId))
        {
            if (LobbyManager.players.ContainsKey(_creatorId))
            {
                LobbyManager.players[_creatorId].ItemSpawned(_itemId, _creatorId, _code, _position, _rotation);
            }
            else
            {
                Debug.Log($"No player with id {_creatorId}");
            }
        }
    }
    public static void ItemTransform(Packet _packet)
    {
        int _itemId = _packet.ReadInt();

        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        Vector3 _scale = _packet.ReadVector3();

        if (ItemHandler.items.ContainsKey(_itemId))
        {
            ItemHandler.items[_itemId].SetItemTransform(_position, _rotation, _scale);
        }
        else
        {
            Debug.Log($"No item with id {_itemId}");
        }
    }

    public static void ItemUseComplete(Packet _packet)
    {
        int _playerId = _packet.ReadInt();

        if (LobbyManager.players.ContainsKey(_playerId))
        {
            LobbyManager.players[_playerId].activePickup = null;
            LobbyManager.players[_playerId].isActivePickupInProgress = false;
            UIManager.instance.gameplayPanel.ToggleItemDisplay(false, UIManager.instance.gameplayPanel.SetItemDetails);
        }
    }

    public static void PickupPickedUp(Packet _packet)
    {
        int _pickupId = _packet.ReadInt();
        int _byPlayer = _packet.ReadInt();
        int _code = _packet.ReadInt();

        if (PickupHandler.pickups.ContainsKey(_pickupId))
        {
            PickupHandler.pickups[_pickupId].PickupPickedUp();
        }

        if (LobbyManager.players.ContainsKey(_byPlayer))
        {
            LobbyManager.players[_byPlayer].PickupPickedUp(GameManager.instance.collection.GetPickupByCode((PickupCode)_code).pickupSO);
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

        UIManager.instance.gameplayPanel.UpdatePlayerTypeViews();
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
        bool _isSpecialColour = _packet.ReadBool();

        if (LobbyManager.players.ContainsKey(_playerId))
        {
            LobbyManager.players[_playerId].ChangeBodyColour(_colour, _isSeekerColour, _isSpecialColour);
        }
        else
        {
            Debug.Log($"No player with id {_playerId}");
        }
    }

    public static void SetPlayerMaterialType(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        MaterialType _materialType = (MaterialType)_packet.ReadInt();

        if (LobbyManager.players.ContainsKey(_playerId))
        {
            LobbyManager.players[_playerId].ChangeMaterialType(_materialType);
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
        int _showPlayerId = _packet.ReadInt();

        GameManager.instance.gameStarted = false;
        GameManager.instance.gameEndInProgress = true;

        PlayerManager localPlayer = LobbyManager.instance.GetLocalPlayer();
        if (localPlayer.id != _showPlayerId)
        {
            localPlayer.thirdPersonCamera.GetComponent<FollowPlayer>().target = LobbyManager.players[_showPlayerId].playerMotor.root;
        }

        //disable input
        //show gameover panel
        //enable option to go back to the lobby 
    }

    public static void ResetGameAndReturnToLobby(Packet _packet)
    {

        PlayerManager localPlayer = LobbyManager.instance.GetLocalPlayer();
        localPlayer.thirdPersonCamera.GetComponent<FollowPlayer>().target = localPlayer.playerMotor.root;

        GameManager.instance.gameEndInProgress = false;

        foreach (Pickup pickup in PickupHandler.pickups.Values)
        {
            Destroy(pickup.gameObject);
        }
        PickupHandler.pickups.Clear();
        PickupHandler.pickupLog.Clear();

        foreach (SpawnableObject item in ItemHandler.items.Values)
        {
            Destroy(item.gameObject);
        }
        ItemHandler.items.Clear();
        ItemHandler.itemLog.Clear();

        foreach (PlayerManager player in LobbyManager.players.Values)
        {
            player.GameOver();
        }

        Debug.Log("Game Over!");
    }

    public static void PlayerTeleportStart(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        Vector3 _teleportPosition = _packet.ReadVector3();
        bool _hasTeleportDelay = _packet.ReadBool();

        if (LobbyManager.players.ContainsKey(_playerId))
        {
            LobbyManager.players[_playerId].PlayerTeleportStart(_teleportPosition, _hasTeleportDelay);
        }
        else
        {
            Debug.Log($"No player with id {_playerId}");
        }
    }

    public static void PlayerTeleportComplete(Packet _packet)
    {
        int _playerId = _packet.ReadInt();

        if (LobbyManager.players.ContainsKey(_playerId))
        {
            LobbyManager.players[_playerId].PlayerTeleportComplete();
        }
        else
        {
            Debug.Log($"No player with id {_playerId}");
        }
    }

    public static void GameRulesChanged(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        GameRules _gameRules = _packet.ReadGameRules();
        GameManager.instance.GameRulesChanged(_gameRules);

        Debug.Log($"Game Rules Changed by player with id {_playerId}");
    }
}
