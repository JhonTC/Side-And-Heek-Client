using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Server;
using System;
using Riptide;

public enum CameraMode
{
    FirstPerson,
    ThirdPerson
}

public class Player : MonoBehaviour
{
    //public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

    public ushort Id { get; private set; }
    public bool IsLocal { get; private set; }
    public string Username { get; private set; }

    private void OnDestroy()
    {
        LobbyManager.players.Remove(Id);
    }

    public static void Spawn(ushort id, string username, bool isHost, bool isReady, Vector3 position, Color colour)
    {
        Player player;
        if (id == NetworkManager.Instance.Client.Id)
        {
            player = Instantiate(GameLogic.Instance.LocalPlayerPrefab, position, Quaternion.identity);
            LobbyManager.localPlayer = player;
            player.IsLocal = true;
        }
        else
        {
            player = Instantiate(GameLogic.Instance.PlayerPrefab, position, Quaternion.identity);
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

        LobbyManager.instance.OnPlayerSpawned(player);

        player.ChangeBodyColour(colour, player.playerType == PlayerType.Hunter);
    }

    public bool isReady = false;
    public bool isHost = false;
    public PlayerType playerType = PlayerType.Default;

    [SerializeField] private TextMeshProUGUI usernameText;

    public PlayerMotor playerMotor;

    public GameObject[] layersToChange;
    public Behaviour[] baseComponentsToDisable;
    public Behaviour[] extraComponentsToDisable;

    public Camera thirdPersonCamera;
    public Camera firstPersonCamera;
    public Transform firstPersonCameraHolder;

    public CameraMode cameraMode = CameraMode.ThirdPerson;
    public float sensitivity = 1;

    public Color hiderColour;
    public Color seekerColour;

    public Server.BasePickup activePickup = null;

    public float clickRange;

    [SerializeField] private float maxSoundDistance;

    [SerializeField] private AudioSource walkingAudioSource;
    [SerializeField] private AudioClip[] walkingAudioClips;

    [SerializeField] private AudioSource collidingAudioSource;
    [SerializeField] private AudioClip[] collidingAudioClips;
    [Range(0, 0.5f)][SerializeField] private float collisionVolumeMultiplier;

    [SerializeField] private ParticleSystem walkingDustParticles;
    private bool lastIsGrounded = false;

    [SerializeField] private Material defaultPlayerMaterial;
    [SerializeField] private Material defaultEyeMaterial;
    [SerializeField] private Material invisiblePlayerLocalMaterial;
    [SerializeField] private Material invisiblePlayerClientMaterial;

    public MaterialType materialType = MaterialType.Default;

    public bool isActivePickupInProgress = false;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    RaycastHit hit;
    Ray ray;
    protected virtual void Update()
    {
        if (IsLocal)
        {
            ray = thirdPersonCamera.ScreenPointToRay(Input.mousePosition);

            if (!UIManager.instance.IsUIActive())
            {
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.DrawRay(ray.origin, ray.direction);
                    /*if (hit.transform.tag == "Pickup")
                    {
                        Pickup pickup = hit.transform.GetComponent<Pickup>();

                        pickup.OnHover();

                        if (Input.GetMouseButtonDown(0))
                        {
                            if (!IsPickupInProgress())
                            {
                                pickup.OnClick(id);
                            }
                        }
                    }*/

                    if (hit.collider.tag == "GameStartObject")
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (!LobbyManager.instance.tryStartGameActive)
                            {
                                //float distance = Mathf.Abs(Vector3.Distance(root.position, hit.transform.position));
                                //Debug.DrawLine(root.position, hit.point, Color.red, clickRange);
                                //if (distance < clickRange)
                                //{
                                Debug.Log("Attempting Start Game: Sending message to server");

                                LobbyManager.instance.tryStartGameActive = true;

                                if (GameManager.instance.gameType == GameType.Multiplayer)
                                {
                                    ClientSend.TryStartGame();
                                }
                                else if(GameManager.instance.gameType == GameType.Singleplayer)
                                {
                                    LocalGameManager localGameManager = GameManager.instance as LocalGameManager;
                                    //localGameManager.TryStartGame(Id);
                                }
                            }
                            else
                            {
                                Debug.Log("Failed Start Game: Already trying to start the game");
                                //Debug.Log("Hover-StartGame"); // replace with hoverStart&Stop
                            }
                        }
                    }
                }
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        playerMotor.head.position = Vector3.Lerp(playerMotor.head.position, playerMotor.root.position, 1f);
        playerMotor.head.rotation = Quaternion.Lerp(playerMotor.head.rotation, playerMotor.root.rotation, 1f);
    }

    public virtual void Init()
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

        SetPlayerReady(isReady);
    }

    private void DisableBaseComponents()
    {
        foreach (Behaviour component in baseComponentsToDisable)
        {
            component.enabled = false;
        }
        thirdPersonCamera.enabled = false;
    }

    private void DisableExtraComponents()
    {
        foreach (Behaviour component in extraComponentsToDisable)
        {
            component.enabled = false;
        }
    }

    private bool IsPickupInProgress()
    {
        if (activePickup != null)
        {
            if (activePickup.pickupSO != null)
            {
                return true;
            }
        }

        return false;
    }

    public void SetRootRotation(Quaternion _rootRotation)
    {
        playerMotor.root.rotation = _rootRotation;
    }
    public void SetPlayerPositions(Vector3 _headPos, Vector3 _rightFootPos, Vector3 _leftFootPos, Vector3 _rightLegPos, Vector3 _leftLegPos)
    {
        playerMotor.root.position = _headPos;
        playerMotor.rightFoot.position = _rightFootPos;
        playerMotor.leftFoot.position = _leftFootPos;
        playerMotor.rightLeg.position = _rightLegPos;
        playerMotor.leftLeg.position = _leftLegPos;
    }
    public void SetPlayerRotations(Quaternion _rightFootRot, Quaternion _leftFootRot, Quaternion _rightLegRot, Quaternion _leftLegRot)
    {
        playerMotor.rightFoot.rotation = _rightFootRot;
        playerMotor.leftFoot.rotation = _leftFootRot;
        playerMotor.rightLeg.rotation = _rightLegRot;
        playerMotor.leftLeg.rotation = _leftLegRot;
    }
    public void SetPlayerState(bool _isGrounded, float _inputSpeed, bool _isJumping, bool _isFlopping, bool _isSneaking, bool _headCollided, float _collisionVolume, bool _footCollided)
    {
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

        if (_headCollided)
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

        if (_footCollided)
        {
            if (!collidingAudioSource.isPlaying)
            {
                walkingAudioSource.clip = walkingAudioClips[UnityEngine.Random.Range(0, walkingAudioClips.Length)];
                walkingAudioSource.pitch = (UnityEngine.Random.Range(0.4f, 0.8f));
                walkingAudioSource.Play();
            }
        }
    }

    /*public void PlayFootstepSound(FootstepType footstepType)
    {
        if (footstepType != FootstepType.Null) {
            GameManager.SoundGroup soundGroup = GameManager.instance.GetFootstepSoundForType(footstepType);
            if (soundGroup != null)
            {
                GameManager.FootstepSound sound = soundGroup.GetRandomClip();

                walkingAudioSource.volume = sound.volume;
                walkingAudioSource.clip = sound.audioClip;
                walkingAudioSource.Play();
            }
        }
    }*/

    public void SetPlayerReady() { SetPlayerReady(!isReady); }
    public void SetPlayerReady(bool _isReady)
    {
        isReady = _isReady;
        if (usernameText)
        {
            usernameText.color = isReady ? LobbyManager.instance.readyColour : LobbyManager.instance.unreadyTextColour;
        }
        UIManager.instance.UpdateLobbyPanel();
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
        } else
        {
            ChangeBodyColour(colour);
        }

        UIManager.instance.gameplayPanel.UpdatePlayerTypeViews();
    }
    public void ChangeBodyColour(bool isSeekerColour)
    {
        if (materialType == MaterialType.Default)
        {
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                if (mr.tag != "Eye")
                {
                    mr.material.SetColor("_EmissionColor", isSeekerColour ? seekerColour : hiderColour);
                }
            }
        }
    }
    public void ChangeBodyColour(Color colour)
    {
        if (materialType == MaterialType.Default)
        {
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                if (mr.tag != "Eye")
                {
                    mr.material.SetColor("_EmissionColor", colour);
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
            } else
            {
                newMaterial = invisiblePlayerClientMaterial;
            }
        }

        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            mr.material = newMaterial;
            
            if (materialType == MaterialType.Default)
            {
                if (mr.tag != "Eye")
                {
                    mr.material.SetColor("_EmissionColor", playerType == PlayerType.Hunter ? seekerColour : hiderColour);
                }
                else
                {
                    mr.material = defaultEyeMaterial;
                }
            }
        }
    }

    public virtual void SetPlayerType(PlayerType _playerType)
    {
        playerType = _playerType;

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
        } else
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
        thirdPersonCamera.GetComponent<FollowPlayer>().PlayerTeleportedToPosition(position);
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
        activePickup = NetworkObjectsManager.instance.pickupHandler.HandlePickup(_pickupSO);

        if (IsLocal)
        {
            UIManager.instance.gameplayPanel.SetItemDetails(_pickupSO);
        }
    }

    public virtual void UsePickup()
    {
        if (activePickup != null)
        {
            if (!isActivePickupInProgress)
            {
                isActivePickupInProgress = true;
                ClientSend.ItemUsed();

                if (activePickup.pickupSO != null)
                {
                    if (activePickup.pickupSO.duration > 0)
                    {
                        UIManager.instance.gameplayPanel.StartProgressCountdown(activePickup.pickupSO.duration);
                    }
                }
            }
        }
    }
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
