﻿using Riptide;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientHandle : MonoBehaviour
{
    
    [MessageHandler((ushort)ServerToClientId.welcome)]
    public static void Welcome(Message message)
    {
        string _msg = message.GetString();
        ushort _myId = message.GetUShort();

        GameRules _gameRules = message.GetGameRules();

        int _hiderColourCount = message.GetInt();
        Color[] _hiderColours = new Color[_hiderColourCount];
        for (int i = 0; i < _hiderColours.Length; i++)
        {
            _hiderColours[i] = message.GetColour();
        }

        Debug.Log($"Message from server: {_msg}");

        GameManager.instance.gameRules = _gameRules;
        GameManager.instance.hiderColours = _hiderColours;
        GameManager.instance.FadeMusic(true);

        UIManager.instance.DisplayGameplayPanel();
        UIManager.instance.customisationPanel.hiderColourSelector.Init(_hiderColours);
        //UIManager.instance.gameRulesPanel.SetGameRules(_gameRules, false);
    }

    /*public static void SpawnPlayer(Message message)
    {
        int _id = message.GetInt();
        string _username = message.GetString();
        bool _isReady = message.GetBool();
        bool _isHost = message.GetBool();
        Vector3 _position = message.GetVector3();
        Quaternion _rotation = message.GetQuaternion();
        Color _colour = message.GetColour();

        bool _hasAuthority = _id == Client.instance.myId;

        Debug.Log($"Message from server: Spawn Player with id: {_id}");
        //LobbyManager.instance.SpawnPlayer(_id, _username, _isReady, _hasAuthority, _isHost, _position, _rotation, _colour);

        UIManager.instance.AddPlayerReady(_id);
    }*/

    [MessageHandler((ushort)ServerToClientId.playerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        ushort _id = message.GetUShort();

        Player.Spawn(_id, message.GetString(), message.GetBool(), message.GetVector3(), message.GetColour());
        UIManager.instance.AddPlayerReady(_id);
    }

    [MessageHandler((ushort)ServerToClientId.playerPosition)]
    public static void PlayerPositions(Message message)
    {
        ushort _id = message.GetUShort();
        Vector3 _headPosition = message.GetVector3();
        Vector3 _rightFootPosition = message.GetVector3();
        Vector3 _leftFootPosition = message.GetVector3();
        Vector3 _rightLegPosition = message.GetVector3();
        Vector3 _leftLegPosition = message.GetVector3();

        Quaternion _rightFootRotation = message.GetQuaternion();
        Quaternion _leftFootRotation = message.GetQuaternion();
        Quaternion _rightLegRotation = message.GetQuaternion();
        Quaternion _leftLegRotation = message.GetQuaternion();

        if (LobbyManager.players.ContainsKey(_id))
        {
            LobbyManager.players[_id].SetPlayerPositions(_headPosition, _rightFootPosition, _leftFootPosition, _rightLegPosition, _leftLegPosition);
            LobbyManager.players[_id].SetPlayerRotations(_rightFootRotation, _leftFootRotation, _rightLegRotation, _leftLegRotation);
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

        if (LobbyManager.players.ContainsKey(_id))
        {
            LobbyManager.players[_id].SetRootRotation(_rootRotation);
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

        if (LobbyManager.players.ContainsKey(_id))
        {
            LobbyManager.players[_id].SetPlayerState(_isGrounded, _inputSpeed, _isJumping, _isFlopping, _isSneaking, _headCollided, _collisionVolume, _footCollided);
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

        GameManager.instance.CreatePickupSpawner(_spawnerId, _position);

        if (message.GetBool())
        {
            ushort _pickupId = message.GetUShort();
            int _code = message.GetInt();

            GameManager.pickupSpawners[_spawnerId].PickupSpawned(_pickupId, _spawnerId, _code, _position, Quaternion.identity);
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

    [MessageHandler((ushort)ServerToClientId.itemSpawned)]
    public static void ItemSpawned(Message message)
    {
        ushort _itemId = message.GetUShort();
        ushort _creatorId = message.GetUShort();
        Vector3 _position = message.GetVector3();
        Quaternion _rotation = message.GetQuaternion();

        int _code = message.GetInt();

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

    [MessageHandler((ushort)ServerToClientId.itemTransform)]
    public static void ItemTransform(Message message)
    {
        ushort _itemId = message.GetUShort();

        Vector3 _position = message.GetVector3();
        Quaternion _rotation = message.GetQuaternion();
        Vector3 _scale = message.GetVector3();

        if (ItemHandler.items.ContainsKey(_itemId))
        {
            ItemHandler.items[_itemId].SetItemTransform(_position, _rotation, _scale);
        }
        else
        {
            Debug.Log($"No item with id {_itemId}");
        }
    }

    [MessageHandler((ushort)ServerToClientId.itemUseComplete)]
    public static void ItemUseComplete(Message message)
    {
        ushort _playerId = message.GetUShort();

        if (LobbyManager.players.ContainsKey(_playerId))
        {
            LobbyManager.players[_playerId].activePickup = null;
            LobbyManager.players[_playerId].isActivePickupInProgress = false;
            UIManager.instance.gameplayPanel.ToggleItemDisplay(false, UIManager.instance.gameplayPanel.SetItemDetails);
        }
    }

    [MessageHandler((ushort)ServerToClientId.pickupPickedUp)]
    public static void PickupPickedUp(Message message)
    {
        ushort _pickupId = message.GetUShort();
        ushort _byPlayer = message.GetUShort();
        int _code = message.GetInt();

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

    [MessageHandler((ushort)ServerToClientId.playerReadyToggled)]
    public static void PlayerReadyToggled(Message message)
    {
        ushort _playerId = message.GetUShort();
        bool _isReady = message.GetBool();

        if (LobbyManager.players.ContainsKey(_playerId))
        {
            LobbyManager.players[_playerId].SetPlayerReady(_isReady);
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

    private static bool lastIsCountdownActive = false; //TODO: wtf is this doing here

    [MessageHandler((ushort)ServerToClientId.setSpecialCountdown)]
    public static void SetSpecialCountdown(Message message)
    {
        ushort _specialId = message.GetUShort();
        int _countdownValue = message.GetInt();
        bool _isCountdownActive = message.GetBool();
        
        if (_specialId == LobbyManager.localPlayer.Id)
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
                //UIManager.instance.SetSpecialMessage($"{LobbyManager.players[_specialId].username} is the hunter... Hide!");
            }
            else
            {
                //UIManager.instance.SetSpecialMessage($"{LobbyManager.players[_specialId].username} has been released");
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

        if (LobbyManager.players.ContainsKey(_playerId))
        {
            LobbyManager.players[_playerId].ChangeBodyColour(_colour, _isSeekerColour, _isSpecialColour);
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

        if (LobbyManager.players.ContainsKey(_playerId))
        {
            LobbyManager.players[_playerId].ChangeMaterialType(_materialType);
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
        int _gameDuration = message.GetInt();

        Debug.Log("Game Start");
        LobbyManager.instance.tryStartGameActive = false;
        UIManager.instance.DisableAllPanels();
        GameManager.instance.StartGameTimer(_gameDuration);
    }

    [MessageHandler((ushort)ServerToClientId.gameOver)]
    public static void GameOver(Message message)
    {
        bool isHunterVictory = message.GetBool();

        GameManager.instance.gameStarted = false;

        foreach (Player player in LobbyManager.players.Values)
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

    [MessageHandler((ushort)ServerToClientId.playerTeleported)]
    public static void PlayerTeleported(Message message)
    {
        ushort _playerId = message.GetUShort();
        Vector3 _teleportPosition = message.GetVector3();

        if (LobbyManager.players.ContainsKey(_playerId))
        {
            LobbyManager.players[_playerId].PlayerTeleported(_teleportPosition);
        }
        else
        {
            Debug.Log($"No player with id {_playerId}");
        }
    }

    [MessageHandler((ushort)ServerToClientId.gameRulesChanged)]
    public static void GameRulesChanged(Message message)
    {
        ushort _playerId = message.GetUShort();
        GameRules _gameRules = message.GetGameRules();
        GameManager.instance.GameRulesChanged(_gameRules);

        Debug.Log($"Game Rules Changed by player with id {_playerId}");
    }

    [MessageHandler((ushort)ServerToClientId.setPlayerHost)]
    public static void SetPlayerHost(Message message)
    {
        ushort _playerId = message.GetUShort();

        if (LobbyManager.players.ContainsKey(_playerId))
        {
            LobbyManager.players[_playerId].isHost = message.GetBool();
            if (LobbyManager.players[_playerId].IsLocal)
            {
                UIManager.instance.gameRulesPanel.OnDisplay();
            }
        }
        else
        {
            Debug.Log($"No player with id {_playerId}");
        }
    }
}
