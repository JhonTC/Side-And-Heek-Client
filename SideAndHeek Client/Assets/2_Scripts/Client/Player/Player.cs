using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Server;
using System;
using Riptide;
using UnityEngine.InputSystem;

public enum CameraMode
{
    FirstPerson,
    ThirdPerson
}

public enum PlayerType
{
    Default = 0,
    Hunter,
    Hider,
    Spectator
}

public enum MaterialType
{
    Default = 0,
    Invisible
}

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();
    public static Player LocalPlayer;
    public ushort Id;// { get; private set; }
    public bool IsLocal;// { get; private set; }
    public string Username;// { get; private set; }

    public bool isHost = false;
    public bool isAuthority = false;
    public bool isReady = false;

    public PlayerType playerType = PlayerType.Default;
    public PlayerType lastPlayerType = PlayerType.Default;

    public PlayerMotor playerMotor;

    public Camera thirdPersonCamera;
    public Camera firstPersonCamera;
    public Transform firstPersonCameraHolder;
    public FollowPlayer followPlayerCamera;
    public Transform shootDirectionUI;

    public CameraMode cameraMode = CameraMode.ThirdPerson;
    public float sensitivity = 1;
    public Vector2 inputAim = Vector2.zero;

    public Color hiderColour;
    public Color seekerColour;

    [HideInInspector] public Color activeColour;
    [HideInInspector] public MaterialType materialType = MaterialType.Default;

    [SerializeField] private Material defaultPlayerMaterial;
    [SerializeField] private Material defaultEyeMaterial;
    [SerializeField] private Material invisiblePlayerLocalMaterial;
    [SerializeField] private Material invisiblePlayerClientMaterial;

    [SerializeField] private float maxSoundDistance;

    public AudioSource walkingAudioSource;
    [SerializeField] private AudioClip[] walkingAudioClips;

    public AudioSource collidingAudioSource;
    [SerializeField] private AudioClip[] collidingAudioClips;
    [Range(0, 0.5f)][SerializeField] private float collisionVolumeMultiplier;

    public bool isBodyActive = false;

    [HideInInspector] public bool footCollided = false;
    [HideInInspector] public bool headCollided = false;

    [SerializeField] private TextMeshProUGUI usernameText;

    public Behaviour[] baseComponentsToDisable;

    [SerializeField] private ParticleSystem walkingDustParticles;
    private bool lastIsGrounded = false;

    public ClientPlayerMotor clientPlayerMotorPrefab;
    public ServerPlayerMotor serverPlayerMotorPrefab;
    public ClientServerPlayerMotor clientServerPlayerMotorPrefab;

    public bool isActivePickupInProgress = false; 
    public BasePickup activePickup = null;

    public Player activeSpectatingPlayer = null;

    [HideInInspector]
    public List<int> activePlayerCollisionIds = new List<int>();

    [HideInInspector] public Vector3 shootDirection = Vector3.zero;
    [HideInInspector] public float throwForce = 80;

    public Transform feetMidpoint;

    private void OnDestroy()
    {
        list.Remove(Id);
    }

    public static void Spawn(ushort id, string username)
    {
        NetworkType networkType = NetworkManager.NetworkType;
        Player player;
        if (networkType == NetworkType.ServerOnly)
        {
            foreach (Player otherPlayer in list.Values)
            {
                otherPlayer.SendSpawned(id);
            }

            Transform spawnpoint = LevelManager.GetLevelManagerForScene(GameManager.instance.activeSceneName).GetNextSpawnpoint(list.Count <= 0);
            player = Spawn(id, username, spawnpoint.position, NetworkManager.Instance.playerPrefab);

            player.SendSpawned();
        }
        else if (networkType == NetworkType.ClientServer)
        {
            foreach (Player otherPlayer in list.Values)
            {
                otherPlayer.SendSpawned(id);
            }

            Transform spawnpoint = LevelManager.GetLevelManagerForScene(GameManager.instance.activeSceneName).GetNextSpawnpoint(list.Count <= 0);
            player = Spawn(id, username, spawnpoint.position, NetworkManager.Instance.playerPrefab);

            player.isAuthority = true;
            player.SendSpawned();
        }
    }

    public static Player Spawn(ushort id, string username, Vector3 position)
    {
        if (NetworkManager.NetworkType == NetworkType.ServerOnly)
        {
            Debug.LogError($"Function should not be called from server. Is used by client/p2p host");
        }

        Player player = Spawn(id, username, position, NetworkManager.Instance.playerPrefab);
        //LobbyManager.instance.OnPlayerSpawned(player);

        return player;
    }

    public static Player Spawn(ushort id, string username, Vector3 position, Player prefab)
    {
        Player player = Instantiate(prefab, position, Quaternion.identity);
        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)}";
        player.Id = id;
        player.Username = string.IsNullOrEmpty(username) ? $"Guest {id}" : username;
        player.isHost = list.Count <= 0;
        player.IsLocal = NetworkManager.Instance.IsLocalPlayer(id);

        player.Init();

        list.Add(id, player);

        if (player.IsLocal)
        {
            LocalPlayer = player;
            Camera.main.gameObject.SetActive(false);
        }

        player.SpawnBody();

        Debug.Log($"Player Spawned: {player.Username}.");

        return player;
    }

    public static void Spawn(ushort id, string username, bool isHost, bool isReady, Vector3 position, Color colour)
    {
        Player player = Instantiate(NetworkManager.Instance.playerPrefab, position, Quaternion.identity);
        if (id == NetworkManager.Instance.Client.Id)
        {
            LocalPlayer = player;
            player.IsLocal = true;
        }
        else
        {
            player.IsLocal = false;
        }

        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)}";
        player.Id = id;
        player.Username = username;
        if (player.IsLocal)
        {
            player.isHost = isHost;
        }
        player.isReady = isReady;
        player.Init();

        list.Add(id, player);

        GameManager.instance.OnPlayerSpawned(player);

        player.SpawnBody();
        player.ChangeBodyColour(colour, player.playerType == PlayerType.Hunter);
    }

    private void SendSpawned()
    {
        NetworkManager.Instance.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.Reliable, ServerToClientId.playerSpawned)));
    }
    private void SendSpawned(ushort toClientId)
    {
        NetworkManager.Instance.Server.Send(AddSpawnData(Message.Create(MessageSendMode.Reliable, ServerToClientId.playerSpawned)), toClientId);
    }
    private Message AddSpawnData(Message message)
    {
        message.AddUShort(Id);
        message.AddString(Username);
        message.AddVector3(transform.position);

        return message;
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    protected virtual void FixedUpdate()
    {
        if (isBodyActive)
        {
            usernameText.transform.position = playerMotor.root.transform.position + Vector3.up * 1.5f;
        }
    }

    public void OnAim(InputAction.CallbackContext value)
    {
        inputAim = value.ReadValue<Vector2>();
    }

    public void OnSelect(InputAction.CallbackContext value)
    {
        if (value.phase == InputActionPhase.Started)
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            mousePosition.z = 0;
            RaycastHit hit;
            Ray ray = thirdPersonCamera.ScreenPointToRay(mousePosition);

            if (!UIManager.instance.IsUIActive())
            {
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.DrawRay(ray.origin, ray.direction);
                }
            }
        }
    }

    public virtual void Init()
    {
        if (NetworkManager.NetworkType == NetworkType.Client || NetworkManager.NetworkType == NetworkType.ClientServer)
        {
            if (usernameText)
            {
                usernameText.text = Username;
            }

            if (!IsLocal)
            {
                DisableBaseComponents();
                thirdPersonCamera.gameObject.SetActive(false);
            }
        }
        else
        {
            DisableBaseComponents();
            thirdPersonCamera.gameObject.SetActive(false);
        }
    }

    private void DisableBaseComponents()
    {
        foreach (Behaviour component in baseComponentsToDisable)
        {
            component.enabled = false;
        }
        thirdPersonCamera.enabled = false;
    }

    public void SetPlayerState(bool _isGrounded, float _inputSpeed, bool _isJumping, bool _isFlopping, bool _isSneaking, bool _headCollided, float _collisionVolume, bool _footCollided)
    {
        headCollided = _headCollided;
        footCollided = _footCollided;

        if (_isGrounded && (_inputSpeed != 0 || lastIsGrounded != _isGrounded) && !_isFlopping && !_isJumping && !_isSneaking)
        {
            if (!walkingDustParticles.isPlaying)
            {
                walkingDustParticles.Play();
            }
        }
        else
        {
            if (walkingDustParticles.isPlaying)
            {
                walkingDustParticles.Stop();
            }
        }

        lastIsGrounded = _isGrounded;

        if (headCollided)
        {
            if (!collidingAudioSource.isPlaying)
            {
                collidingAudioSource.clip = collidingAudioClips[UnityEngine.Random.Range(0, collidingAudioClips.Length)];

                float volume = _collisionVolume * collisionVolumeMultiplier;
                collidingAudioSource.volume = Mathf.Clamp01(volume);

                collidingAudioSource.pitch = (UnityEngine.Random.Range(0.8f, 1.2f));
                collidingAudioSource.Play();
            }
        }

        if (footCollided)
        {
            if (!collidingAudioSource.isPlaying)
            {
                walkingAudioSource.clip = walkingAudioClips[UnityEngine.Random.Range(0, walkingAudioClips.Length)];
                walkingAudioSource.pitch = (UnityEngine.Random.Range(0.4f, 0.8f));
                walkingAudioSource.Play();
            }
        }
    }

    public void SpawnBody()
    {
        if (!isBodyActive)
        {
            PlayerMotor motorPrefab = serverPlayerMotorPrefab;
            switch (NetworkManager.NetworkType)
            {
                case NetworkType.Client:
                    motorPrefab = clientPlayerMotorPrefab;
                    break;
                case NetworkType.ClientServer:
                    if (IsLocal)
                    {
                        motorPrefab = clientServerPlayerMotorPrefab;
                    }
                    break;
            }

            playerMotor = Instantiate(motorPrefab, transform);
            playerMotor.Init(this);
            followPlayerCamera.ChangeTarget(playerMotor.root.transform);
            isBodyActive = true;
        }
    }

    public void DespawnPlayer()
    {
        if (isBodyActive)
        {
            isBodyActive = false;

            Destroy(playerMotor.gameObject);
        }
    }

    public void SetPlayerReady() { SetPlayerReady(!isReady); }
    public void SetPlayerReady(bool _isReady, bool sendMessage = true)
    {
        isReady = _isReady;

        if (NetworkManager.NetworkType != NetworkType.Client)
        {
            if (sendMessage)
            {
                ServerSend.PlayerReadyToggled(Id, isReady);
            }
        }

        if (NetworkManager.NetworkType != NetworkType.ServerOnly)
        {
            if (usernameText)
            {
                usernameText.color = isReady ? GameManager.instance.readyColour : GameManager.instance.unreadyTextColour;
            }
            UIManager.instance.UpdateLobbyPanel();
        }
    }

    public void ChangeBodyColour(Color colour, bool isSeekerColour, bool isSpecialColour = false)
    {
        if (!isSpecialColour)
        {
            if (isSeekerColour)
            {
                seekerColour = colour;
            }
            else
            {
                hiderColour = colour;
                UIManager.instance.customisationPanel.hiderColourSelector.UpdateAllButtons();
            }

            ChangeBodyColour(isSeekerColour);
        }
        else
        {
            ChangeBodyColour(colour);
        }

        UIManager.instance.gameplayPanel.UpdatePlayerTypeViews();
    }
    public void ChangeBodyColour(bool isSeekerColour = false, Material newMaterial = null)
    {
        ChangeBodyColour(isSeekerColour ? seekerColour : hiderColour, newMaterial);
    }
    public void ChangeBodyColour(Color colour, Material newMaterial = null)
    {
        if (materialType == MaterialType.Default)
        {
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                if (mr.tag != "Eye")
                {
                    if (newMaterial)
                    {
                        mr.material = newMaterial;
                    }

                    mr.material.color = colour; //mr.material.SetColor("_EmissionColor", colour);
                }
                else
                {
                    if (newMaterial)
                    {
                        mr.material = defaultEyeMaterial;
                    }
                }
            }
        }
    }

    public void ChangeMaterialType(MaterialType _materialType)
    {
        materialType = _materialType;

        Material newMaterial = defaultPlayerMaterial;
        if (materialType == MaterialType.Invisible)
        {
            if (IsLocal)
            {
                newMaterial = invisiblePlayerLocalMaterial;
            }
            else
            {
                newMaterial = invisiblePlayerClientMaterial;
            }
        }

        ChangeBodyColour(playerType == PlayerType.Hunter, newMaterial);
    }

    public virtual void SetPlayerType(PlayerType _playerType)
    {
        lastPlayerType = playerType;
        playerType = _playerType;

        //GameManager.instance.gameMode.OnPlayerTypeChanged(this);

        if (materialType == MaterialType.Default)
        {
            switch (playerType)
            {
                case PlayerType.Default:
                    ChangeBodyColour(false);
                    break;
                case PlayerType.Hunter:
                    ChangeBodyColour(true);
                    break;
                case PlayerType.Hider:
                    ChangeBodyColour(false);
                    break;
                case PlayerType.Spectator:
                    //spectator controls
                    break;
                default:
                    break;
            }
        }
        else
        {
            ChangeMaterialType(MaterialType.Default);
        }

        activePickup = null;

        if (IsLocal)
        {
            UIManager.instance.gameplayPanel.SetItemDetails();
        }
    }

    public void OnGameStart()
    {
        if (usernameText)
        {
            usernameText.gameObject.SetActive(false);
        }
    }

    public void OnGameOver()
    {
        if (usernameText)
        {
            usernameText.gameObject.SetActive(true);
        }

        SetPlayerType(PlayerType.Default);
        SetPlayerReady(false);
    }

    public void PlayerTeleported(Vector3 position)
    {
        thirdPersonCamera.GetComponent<FollowPlayer>().TeleportCameraToTarget(position);
    }

    public void PickupSpawned(ushort _pickupId, ushort _creatorId, int _code, Vector3 _position, Quaternion _rotation)
    {
        NetworkObjectsManager.instance.pickupHandler.SpawnPickup(_pickupId, _creatorId, _code, _position, _rotation);
    }
    public void ItemSpawned(ushort _itemId, ushort _creatorId, int _code, Vector3 _position, Quaternion _rotation)
    {
        NetworkObjectsManager.instance.itemHandler.SpawnItem(_itemId, _creatorId, _code, _position, _rotation);
    }

    public virtual void PickupPickedUp(PickupSO _pickupSO)
    {
        activePickup = NetworkObjectsManager.instance.pickupHandler.HandlePickup(_pickupSO, this);

        if (NetworkManager.NetworkType != NetworkType.ServerOnly)
        {
            if (IsLocal)
            {
                UIManager.instance.gameplayPanel.SetItemDetails(_pickupSO);
                if (activePickup.pickupSO.sendDirection)
                {
                    shootDirectionUI.gameObject.SetActive(true);
                }
            }
        }
    }

    public virtual void UsePickup()
    {
        if (activePickup != null)
        {
            if (!isActivePickupInProgress)
            {
                if (activePickup.pickupSO != null)
                {
                    isActivePickupInProgress = true;
                    if (activePickup.pickupSO.sendDirection)
                    {
                        ClientSend.ItemUsed(activePickup.pickupSO.sendDirection, shootDirectionUI.forward);
                        shootDirectionUI.gameObject.SetActive(false);
                    }
                    else
                    {
                        ClientSend.ItemUsed();
                    }

                    if (activePickup.pickupSO.duration > 0)
                    {
                        UIManager.instance.gameplayPanel.StartProgressCountdown(activePickup.pickupSO.duration);
                    }
                }
            }
        }
    }

    public void SpectatePlayer(Player spectatePlayer = null)
    {
        activeSpectatingPlayer = spectatePlayer;

        if (activeSpectatingPlayer == null)
        {
            UIManager.instance.ClosePanel(UIPanelType.Spectate, true);
            print($"Spectator camera reset to self");
            //thirdPersonCamera.GetComponent<FollowPlayer>().ChangeTarget(playerMotor.root);

        }
        else
        {
            print($"Spectating player: {activeSpectatingPlayer}");
            //thirdPersonCamera.GetComponent<FollowPlayer>().ChangeTarget(activeSpectatingPlayer.playerMotor.root);
        }
    }

    #region ServerFunctions
    public bool AttemptPickupItem()
    {
        if (activePickup != null)
        {
            if (activePickup.pickupSO != null)
            {
                return false;
            }
        }

        return true;
    }

    public void TeleportPlayer(Transform _spawnpoint)
    {
        DespawnPlayer();
        transform.position = _spawnpoint.position;
        ServerSend.PlayerTeleported(Id, _spawnpoint.position);
        SpawnBody();
    }

    public void SetPlayerType(PlayerType type, bool isFirstHunter = false, bool sendMessage = true)
    {
        playerType = type;

        GameManager.instance.gameMode.OnPlayerTypeSet(this, playerType, isFirstHunter);

        activePickup = null;

        if (sendMessage)
        {
            ServerSend.SetPlayerType(Id, playerType, true);
        }
    }

    public void PickupUsed()
    {
        if (activePickup != null)
        {
            if (NetworkManager.NetworkType != NetworkType.Client) // can only be called from server owners
            {
                Debug.Log("Item Used");
                activePickup.PickupUsed();
            }
        }
    }

    public void ItemUseComplete()
    {
        activePickup = null;
        ServerSend.ItemUseComplete(Id);
    }

    #endregion
}
