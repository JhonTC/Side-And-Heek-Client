using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

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

    [SerializeField] private Transform root;
    [SerializeField] private Transform head;
    [SerializeField] private Transform leftLeg;
    [SerializeField] private Transform rightLeg;
    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightFoot;

    public GameObject[] layersToChange;
    public Behaviour[] baseComponentsToDisable;
    public Behaviour[] extraComponentsToDisable;

    public Camera thirdPersonCamera;
    public Camera firstPersonCamera;
    public Transform firstPersonCameraHolder;
    public MeshRenderer[] headRenderers;

    public CameraMode cameraMode = CameraMode.ThirdPerson;
    public float sensitivity = 1;


    public Color hiderColour;
    public Color seekerColour;

    public List<TaskCode> activeTasks = new List<TaskCode>();
    public ItemCode activeItem = ItemCode.NULL_ITEM;

    public float clickRange;

    [System.Serializable]
    public class Task {
        public TaskCode code;
        float progress;

        public Task(TaskCode _code)
        {
            code = _code;
            progress = 0;
        }
    }

    public class Item
    {
        public ItemCode code;

        public Item(ItemCode _code)
        {
            code = _code;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    RaycastHit hit;
    Ray ray;
    private void Update()
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
                                isPickupInProgress = IsTaskInProgress(spawner.activeTask.taskCode);
                            }
                            else if (spawner.pickupType == PickupType.Item)
                            {
                                isPickupInProgress = IsItemInProgress();
                            }

                            if (!isPickupInProgress)
                            {
                                spawner.OnClick();
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
                                ClientSend.TryStartGame();
                                //} 
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

        //foreach (BaseTask task in activeTasks)
        //{
        //    task.UpdateTask();
        //}
    }

    private bool IsTaskInProgress(TaskCode _code)
    {
        foreach (TaskCode code in activeTasks)
        {
            if (code == _code)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsItemInProgress()
    {
        if (activeItem != ItemCode.NULL_ITEM)
        {
            return true;
        }

        return false;
    }

    private TaskCode GetActiveTaskWithCode(TaskCode _code)
    {
        foreach (TaskCode code in activeTasks)
        {
            if (code == _code)
            {
                return code;
            }
        }

        throw new System.Exception($"Task {_code}, is not in progress for {username}.");
    }


    private void FixedUpdate()
    {
        head.position = Vector3.Lerp(head.position, root.position, 1f);
        head.rotation = Quaternion.Lerp(head.rotation, root.rotation, 1f);
    }

    public void Init(int _id, string _username, bool _isReady, bool _hasAuthority, bool _isHunter)
    {
        id = _id;
        username = _username;
        isReady = _isReady;
        hasAuthority = _hasAuthority;

        if (username == "")
        {
            username = $"Player {id}";
        }

        usernameText.text = username;

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

    private void ChangeLayers(string layer)
    {
        foreach (GameObject layerGO in layersToChange)
        {
            layerGO.layer = LayerMask.NameToLayer(layer);
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

    private void DisableExtraComponents()
    {
        foreach (Behaviour component in extraComponentsToDisable)
        {
            component.enabled = false;
        }
    }

    public void SetRootRotation(Quaternion _rootRotation)
    {
        root.rotation = _rootRotation;
    }
    public void SetPlayerPositions(Vector3 _headPos, Vector3 _rightFootPos, Vector3 _leftFootPos, Vector3 _rightLegPos, Vector3 _leftLegPos)
    {
        root.position = _headPos;
        rightFoot.position = _rightFootPos;
        leftFoot.position = _leftFootPos;
        rightLeg.position = _rightLegPos;
        leftLeg.position = _leftLegPos;
    }
    public void SetPlayerRotations(Quaternion _rightFootRot, Quaternion _leftFootRot, Quaternion _rightLegRot, Quaternion _leftLegRot)
    {
        rightFoot.rotation = _rightFootRot;
        leftFoot.rotation = _leftFootRot;
        rightLeg.rotation = _rightLegRot;
        leftLeg.rotation = _leftLegRot;
    }

    public void SetPlayerReady() { SetPlayerReady(!isReady); }
    public void SetPlayerReady(bool _isReady)
    {
        isReady = _isReady;
        usernameText.color = isReady ? LobbyManager.instance.readyColour : LobbyManager.instance.unreadyTextColour;
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

    public void SetPlayerType(PlayerType _playerType)
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
        usernameText.gameObject.SetActive(false);
    }

    public void GameOver()
    {
        usernameText.gameObject.SetActive(true);
    }

    public void PlayerTeleported(Vector3 position)
    {
        thirdPersonCamera.GetComponent<FollowPlayer>().PlayerTeleportedToPosition(position);
    }

    public void PickupPickedUp(PickupType pickupType, int code) 
    {
        if (pickupType == PickupType.Task)
        {
            activeTasks.Add((TaskCode)code);
        }
        else if (pickupType == PickupType.Item)
        {
            activeItem = (ItemCode)code;
        }
    }

    public void TaskProgressed(TaskCode code, float progression)
    {
        Debug.Log($"Task {code} progressed by {progression}.");
    }

    public void TaskComplete(TaskCode code)
    {
        if (activeTasks.Contains(code))
        {
            activeTasks.Remove(code);
        }

        Debug.Log($"Task {code} Complete!");
    }

    public void UseItem()
    {
        if (activeItem != ItemCode.NULL_ITEM)
        {
            ClientSend.ItemUsed();
            activeItem = ItemCode.NULL_ITEM;
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
