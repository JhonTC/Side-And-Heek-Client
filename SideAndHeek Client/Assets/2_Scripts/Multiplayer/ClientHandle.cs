using Riptide;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientHandle : MonoBehaviour //todo: cleanup all function calls (Welcome is a good example of a message recieved function that is way too large)
{
    [MessageHandler((ushort)ServerToClientId.welcome)]
    public static void Welcome(Message message)
    {
        string _msg = message.GetString();
        ushort _myId = message.GetUShort();
        bool isHost = message.GetBool();
        if (!isHost)
        {
            GameManager.instance.GameTypeChanged((GameType)message.GetInt());
            GameManager.instance.GameRulesChanged(message.GetGameRules());
        } else
        {
            ClientSend.GameRulesChanged(GameManager.instance.gameMode.GetGameRules());
        }

        Debug.Log($"Message from server: {_msg}");

        GameManager.instance.FadeMusic(true);

        UIManager.instance.CloseAllPanels();
        UIManager.instance.DisplayPanel(UIPanelType.Gameplay);
    }

    [MessageHandler((ushort)ServerToClientId.playerSpawned)]
    public static void SpawnPlayer(Message message)
    {
        ushort _id = message.GetUShort();


        Player.Spawn(_id, message.GetString(), message.GetBool(), message.GetBool(), message.GetVector3(), message.GetColour());
        UIManager.instance.AddPlayerReady(_id);
        InputHandler.instance.SwitchInput("PlayerControls");
    }

    [MessageHandler((ushort)ServerToClientId.playerPosition)]
    public static void PlayerPositions(Message message)
    {
        ushort _id = message.GetUShort();
        ushort _tick = message.GetUShort();
        Vector3 _headPosition = message.GetVector3();
        Vector3 _rightFootPosition = message.GetVector3();
        Vector3 _leftFootPosition = message.GetVector3();
        Vector3 _rightLegPosition = message.GetVector3();
        Vector3 _leftLegPosition = message.GetVector3();

        Quaternion _rightFootRotation = message.GetQuaternion();
        Quaternion _leftFootRotation = message.GetQuaternion();
        Quaternion _rightLegRotation = message.GetQuaternion();
        Quaternion _leftLegRotation = message.GetQuaternion();

        if (Player.list.ContainsKey(_id))
        {
            Player player = Player.list[_id];
            if (player.isBodyActive) 
            {
                player.playerMotor.SetPlayerPositions(_tick, _headPosition, _rightFootPosition, _leftFootPosition, _rightLegPosition, _leftLegPosition);
                player.playerMotor.SetPlayerRotations(_rightFootRotation, _leftFootRotation, _rightLegRotation, _leftLegRotation);
            }
            
        } else
        {
            Debug.Log($"No player with id {_id}");
        }
    }

    [MessageHandler((ushort)ServerToClientId.playerRotation)]
    public static void PlayerRotation(Message message)
    {
        ushort _id = message.GetUShort();
        Quaternion _rootRotation = message.GetQuaternion();

        if (Player.list.ContainsKey(_id))
        {
            Player player = Player.list[_id];
            if (player.isBodyActive)
            {
                player.playerMotor.SetRootRotation(_rootRotation);
            }
        }
        else
        {
            Debug.Log($"No player with id {_id}");
        }
    }

    [MessageHandler((ushort)ServerToClientId.playerState)]
    public static void PlayerState(Message message)
    {
        ushort _id = message.GetUShort();

        bool _isGrounded = message.GetBool();
        bool _headCollided = message.GetBool();
        float _collisionVolume = message.GetFloat();
        float _inputSpeed = message.GetFloat();
        bool _footCollided = message.GetBool();

        bool _isJumping = message.GetBool();
        bool _isFlopping = message.GetBool();
        bool _isSneaking = message.GetBool();

        if (Player.list.ContainsKey(_id))
        {
            Player.list[_id].SetPlayerState(_isGrounded, _inputSpeed, _isJumping, _isFlopping, _isSneaking, _headCollided, _collisionVolume, _footCollided);
        }
        else
        {
            Debug.Log($"No player with id {_id}");
        }
    }

    [MessageHandler((ushort)ServerToClientId.createItemSpawner)]
    public static void CreatePickupSpawner(Message message)
    {
        ushort _spawnerId = message.GetUShort();
        Vector3 _position = message.GetVector3();

        PickupSpawner.CreatePickupSpawner(_spawnerId, _position);

        if (message.GetBool())
        {
            ushort _pickupId = message.GetUShort();
            int _code = message.GetInt();

            PickupSpawner.pickupSpawners[_spawnerId].PickupSpawned(_pickupId, _spawnerId, _code, _position, Quaternion.identity);
        }
    }

    [MessageHandler((ushort)ServerToClientId.pickupSpawned)]
    public static void PickupSpawned(Message message)
    {
        ushort _pickupId = message.GetUShort();
        bool _bySpawner = message.GetBool();
        ushort _creatorId = message.GetUShort();
        Vector3 _position = message.GetVector3();
        Quaternion _rotation = message.GetQuaternion();

        int _code = message.GetInt();

        if (!NetworkObjectsManager.networkObjects.ContainsKey(_pickupId))
        {
            if (_bySpawner)
            {
                if (PickupSpawner.pickupSpawners.ContainsKey(_creatorId))
                {
                    PickupSpawner.pickupSpawners[_creatorId].PickupSpawned(_pickupId, _creatorId, _code, _position, _rotation); //todo these can be merged into one
                }
                else
                {
                    Debug.Log($"No spawner with id {_creatorId}");
                }
            }
            else
            {
                if (Player.list.ContainsKey(_creatorId))
                {
                    Player.list[_creatorId].PickupSpawned(_pickupId, _creatorId, _code, _position, _rotation);
                }
                else
                {
                    Debug.Log($"No player with id {_creatorId}");
                }
            }
        }
    }

    [MessageHandler((ushort)ServerToClientId.itemSpawned)]
    public static void ItemSpawned(Message message)
    {
        ushort _itemId = message.GetUShort();
        ushort _creatorId = message.GetUShort();
        Vector3 _position = message.GetVector3();
        Quaternion _rotation = message.GetQuaternion();

        int _code = message.GetInt();

        if (!NetworkObjectsManager.networkObjects.ContainsKey(_itemId))
        {
            if (Player.list.ContainsKey(_creatorId))
            {
                Player.list[_creatorId].ItemSpawned(_itemId, _creatorId, _code, _position, _rotation);
            }
            else
            {
                Debug.Log($"No player with id {_creatorId}");
            }
        }
    }

    [MessageHandler((ushort)ServerToClientId.networkObjectTransform)]
    public static void NetworkObjectTransform(Message message)
    {
        ushort _objectId = message.GetUShort();

        Vector3 _position = message.GetVector3();
        Quaternion _rotation = message.GetQuaternion();
        Vector3 _scale = message.GetVector3();

        if (NetworkObjectsManager.networkObjects.ContainsKey(_objectId))
        {
            NetworkObjectsManager.networkObjects[_objectId].SetObjectTransform(_position, _rotation, _scale);
        }
        else
        {
            Debug.Log($"No network object with id {_objectId}");
        }
    }

    [MessageHandler((ushort)ServerToClientId.networkObjectDestroyed)]
    public static void NetworkObjectDestroyed(Message message)
    {
        ushort _objectId = message.GetUShort();

        if (NetworkObjectsManager.networkObjects.ContainsKey(_objectId))
        {
            NetworkObjectsManager.instance.DestroyObject(NetworkObjectsManager.networkObjects[_objectId]);
        }
        else
        {
            Debug.Log($"No network object with id {_objectId}");
        }
    }

    [MessageHandler((ushort)ServerToClientId.itemUseComplete)]
    public static void ItemUseComplete(Message message)
    {
        ushort _playerId = message.GetUShort();

        if (Player.list.ContainsKey(_playerId))
        {
            Player.list[_playerId].activePickup = null;
            Player.list[_playerId].isActivePickupInProgress = false;
            UIManager.instance.gameplayPanel.ToggleItemDisplay(false, UIManager.instance.gameplayPanel.SetItemDetails);
        }
    }

    [MessageHandler((ushort)ServerToClientId.pickupPickedUp)]
    public static void PickupPickedUp(Message message)
    {
        ushort _pickupId = message.GetUShort();
        ushort _byPlayer = message.GetUShort();
        int _code = message.GetInt();

        if (NetworkObjectsManager.networkObjects.ContainsKey(_pickupId))
        {
            Pickup pickup = NetworkObjectsManager.networkObjects[_pickupId] as Pickup;
            if (pickup != null)
            {
                pickup.PickupPickedUp();
            }
        }

        if (Player.list.ContainsKey(_byPlayer))
        {
            Player.list[_byPlayer].PickupPickedUp(GameManager.instance.collection.GetPickupByCode((PickupType)_code).pickupSO);
        }
        else
        {
            Debug.Log($"No player with id {_byPlayer}");
        }
    }

    [MessageHandler((ushort)ServerToClientId.playerReadyToggled)]
    public static void PlayerReadyToggled(Message message)
    {
        ushort _playerId = message.GetUShort();
        bool _isReady = message.GetBool();

        if (Player.list.ContainsKey(_playerId))
        {
            Player.list[_playerId].SetPlayerReady(_isReady);
        }
        else
        {
            Debug.Log($"No player with id {_playerId}");
        }
    }

    [MessageHandler((ushort)ServerToClientId.changeScene)]
    public static void ChangeScene(Message message)
    {
        string _sceneToLoad = message.GetString();

        GameManager.instance.LoadScene(_sceneToLoad, LoadSceneMode.Additive);
    }

    [MessageHandler((ushort)ServerToClientId.unloadScene)]
    public static void UnloadScene(Message message)
    {
        string _sceneToLoad = message.GetString();

        GameManager.instance.UnloadScene(_sceneToLoad, false);
    }

    [MessageHandler((ushort)ServerToClientId.setPlayerType)]
    public static void SetPlayerType(Message message)
    {
        ushort _playerId = message.GetUShort();
        PlayerType _playerType = (PlayerType)message.GetInt();

        if (Player.list.ContainsKey(_playerId))
        {
            Player.list[_playerId].SetPlayerType(_playerType);
        }
        else
        {
            Debug.Log($"No player with id {_playerId}");
        }

        UIManager.instance.gameplayPanel.UpdatePlayerTypeViews();
    }

    private static bool lastIsCountdownActive = false; //TODO: what is this doing here?

    [MessageHandler((ushort)ServerToClientId.setSpecialCountdown)]
    public static void SetSpecialCountdown(Message message)
    {
        ushort _specialId = message.GetUShort();
        int _countdownValue = message.GetInt();
        bool _isCountdownActive = message.GetBool();
        
        if (_specialId == Player.LocalPlayer.Id)
        {
            if (Player.list.ContainsKey(_specialId))
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
                UIManager.instance.SetSpecialMessage($"{Player.list[_specialId].Username} is the hunter... Hide!");
            }
            else
            {
                UIManager.instance.SetSpecialMessage($"{Player.list[_specialId].Username} has been released");
            }

            lastIsCountdownActive = _isCountdownActive;
        }
    }

    [MessageHandler((ushort)ServerToClientId.setPlayerColour)]
    public static void SetPlayerColour(Message message)
    {
        ushort _playerId = message.GetUShort();
        Color _colour = message.GetColour();
        bool _isSeekerColour = message.GetBool();
        bool _isSpecialColour = message.GetBool();

        if (Player.list.ContainsKey(_playerId))
        {
            Player.list[_playerId].ChangeBodyColour(_colour, _isSeekerColour, _isSpecialColour);
        }
        else
        {
            Debug.Log($"No player with id {_playerId}");
        }
    }

    [MessageHandler((ushort)ServerToClientId.setPlayerMaterialType)]
    public static void SetPlayerMaterialType(Message message)
    {
        ushort _playerId = message.GetUShort();
        MaterialType _materialType = (MaterialType)message.GetInt();

        if (Player.list.ContainsKey(_playerId))
        {
            Player.list[_playerId].ChangeMaterialType(_materialType);
        }
    }

    [MessageHandler((ushort)ServerToClientId.sendErrorResponseCode)]
    public static void RecieveErrorResponse(Message message)
    {
        ErrorResponseCode responseCode = (ErrorResponseCode)message.GetInt();

        ErrorResponseHandler.HandleErrorResponse(responseCode);
    }

    [MessageHandler((ushort)ServerToClientId.gameStart)]
    public static void GameStart(Message message)
    {
        GameManager.instance.GameStart(message.GetInt());
    }

    [MessageHandler((ushort)ServerToClientId.gameOver)]
    public static void GameOver(Message message)
    {
        GameManager.instance.gameMode.ReadGameOverMessageValues(message);
        GameManager.instance.GameOver();
    }

    [MessageHandler((ushort)ServerToClientId.playerTeleported)]
    public static void PlayerTeleported(Message message) //todo: doing nothing with player... maybe move them clientside for clientside prediction
    {
        ushort _playerId = message.GetUShort();
        Vector3 _teleportPosition = message.GetVector3();

        if (Player.list.ContainsKey(_playerId))
        {
            Player.list[_playerId].PlayerTeleported(_teleportPosition);
        }
        else
        {
            Debug.Log($"No player with id {_playerId}");
        }
    }

    [MessageHandler((ushort)ServerToClientId.gameRulesChanged)]
    public static void GameRulesChanged(Message message)
    {
        GameType gameType = (GameType)message.GetInt();
        if (GameManager.instance.gameType != gameType)
        {
            GameManager.instance.GameTypeChanged(gameType); //todo: this shouldnt be here
        }

        GameRules _gameRules = message.GetGameRules();

        GameManager.instance.GameRulesChanged(_gameRules);
    }

    [MessageHandler((ushort)ServerToClientId.setPlayerHost)]
    public static void SetPlayerHost(Message message)
    {
        ushort _playerId = message.GetUShort();

        if (Player.list.ContainsKey(_playerId))
        {
            Player.list[_playerId].isHost = message.GetBool();
            if (Player.list[_playerId].IsLocal)
            {
                UIManager.instance.gameRulesPanel.UpdatePanelHost();
            }
        }
        else
        {
            Debug.Log($"No player with id {_playerId}");
        }
    }

    [MessageHandler((ushort)ServerToClientId.setVisualEffect)]
    public static void SetVisualEffect(Message message)
    {
        ushort effectId = message.GetUShort();
        bool toggleValue = message.GetBool();
        VisualEffect effect = null;
        if (toggleValue)
        {
           effect = VisualEffects.CreateVisualEffectFromMessage(message);
        }

        if (toggleValue)
        {
            VisualEffects.AddEffect(effectId, effect);
        }
        else
        {
            VisualEffects.RemoveEffect(effectId);
        }
    }

    [MessageHandler((ushort)ServerToClientId.weatherObjectTransform)]
    public static void WeatherObjectTransform(Message message)
    {
        ushort _objectId = message.GetUShort();

        if (WindVolume.WindRecievers.ContainsKey(_objectId))
        {
            WindVolume.WindRecievers[_objectId].networkPhysicsBody.ReadMessageValues(message);
        }
    }
}
