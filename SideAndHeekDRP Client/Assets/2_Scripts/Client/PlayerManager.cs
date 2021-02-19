using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Server;

public enum CameraMode
{
    FirstPerson,
    ThirdPerson
}

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public bool isReady = false;
    public bool hasAuthority = false;
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

    public List<BaseTask> activeTasks = new List<BaseTask>();
    public BaseItem activeItem = null;

    public float clickRange;


    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    RaycastHit hit;
    Ray ray;
    protected virtual void Update()
    {
        if (hasAuthority)
        {
            ray = thirdPersonCamera.ScreenPointToRay(Input.mousePosition);

            if (!UIManager.instance.isUIActive)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.DrawRay(ray.origin, ray.direction);
                    if (hit.transform.tag == "ItemSpawner")
                    {
                        PickupSpawner spawner = hit.transform.GetComponent<PickupSpawner>();

                        spawner.OnHover();

                        if (Input.GetMouseButtonDown(0))
                        {
                            bool isPickupInProgress = false;
                            if (spawner.pickupType == PickupType.Task)
                            {
                                isPickupInProgress = IsTaskInProgress(spawner.activeTaskDetails.task.taskCode);
                            }
                            else if (spawner.pickupType == PickupType.Item)
                            {
                                isPickupInProgress = IsItemInProgress();
                            }

                            if (!isPickupInProgress)
                            {
                                spawner.OnClick(id);
                            }
                        }
                    }

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
                                    localGameManager.TryStartGame(id);
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

    public virtual void Init(int _id, string _username, bool _isReady, bool _hasAuthority, bool _isHunter)
    {
        id = _id;
        username = _username;
        isReady = _isReady;
        hasAuthority = _hasAuthority;

        if (username == "")
        {
            username = $"Player {id}";
        }

        if (usernameText)
        {
            usernameText.text = username;
        }

        if (!hasAuthority)
        {
            DisableBaseComponents();
            if (!_isHunter)
            {
                DisableExtraComponents();
            }
        }

        if (!_isHunter)
        {
            //ChangeLayers("Player");
        } else
        {
            //ChangeLayers("LocalPlayer");
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

    private bool IsTaskInProgress(TaskCode _code)
    {
        foreach (BaseTask activeTask in activeTasks)
        {
            if (activeTask.task.taskCode == _code)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsItemInProgress()
    {
        if (activeItem != null)
        {
            if (activeItem.item != null)
            {
                return true;
            }
        }

        return false;
    }

    private BaseTask GetActiveTaskWithCode(TaskCode _code)
    {
        foreach (BaseTask activeTask in activeTasks)
        {
            if (activeTask.task.taskCode == _code)
            {
                return activeTask;
            }
        }

        throw new System.Exception($"Task {_code}, is not in progress for {username}.");
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

    public void ChangeBodyColour(Color colour, bool isSeekerColour)
    {
        if (isSeekerColour)
        {
            seekerColour = colour;
        }
        else
        {
            hiderColour = colour;
        }

        ChangeBodyColour(isSeekerColour);
    }
    public void ChangeBodyColour(bool isSeekerColour)
    {
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            if (mr.tag != "Eye")
            {
                mr.material.SetColor("_EmissionColor", isSeekerColour? seekerColour : hiderColour);
            }
        }
    }

    public virtual void SetPlayerType(PlayerType _playerType)
    {
        playerType = _playerType;

        switch(playerType)
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

    public void GameStart()
    {
        if (usernameText)
        {
            usernameText.gameObject.SetActive(false);
        }
    }

    public void GameOver()
    {
        if (usernameText)
        {
            usernameText.gameObject.SetActive(true);
        }
    }

    public void PlayerTeleported(Vector3 position)
    {
        thirdPersonCamera.GetComponent<FollowPlayer>().PlayerTeleportedToPosition(position);
    }

    public virtual void PickupPickedUp(BasePickup pickup)
    {
        if (pickup.pickupType == PickupType.Task)
        {
            activeTasks.Add(PickupManager.instance.HandleTask(pickup as TaskPickup));
        }
        else if (pickup.pickupType == PickupType.Item)
        {
            activeItem = PickupManager.instance.HandleItem(pickup as ItemPickup);
            UIManager.instance.gameplayPanel.SetItemDetails(pickup as ItemPickup);
        }
    }

    public void TaskProgressed(TaskCode code, float progression)
    {
        Debug.Log($"Task {code} progressed by {progression}.");
    }

    public void TaskComplete(TaskCode taskCode)
    {
        if (IsTaskInProgress(taskCode))
        {
            activeTasks.Remove(GetActiveTaskWithCode(taskCode));

            Debug.Log($"Task {taskCode} Complete!");
        }
    }

    public virtual void UseItem()
    {
        if (activeItem != null)
        {
            ClientSend.ItemUsed();
            UIManager.instance.gameplayPanel.SetItemDetails();
            activeItem = null;
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
