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

    [SerializeField] private TextMeshProUGUI messageText;

    public Camera thirdPersonCamera;
    public Camera firstPersonCamera;
    public Transform firstPersonCameraHolder;
    public MeshRenderer[] headRenderers;

    public CameraMode cameraMode = CameraMode.ThirdPerson;
    public float sensitivity = 1;

    private bool fadeOutMessageText = false;
    private float fadeDuration = 1f;

    public Color hiderColour;
    public Color seekerColour;

    public List<TaskCode> activeTasks = new List<TaskCode>();

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

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    RaycastHit hit;
    Ray ray;
    private void Update()
    {
        ray = thirdPersonCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "ItemSpawner")
            {
                ItemSpawner spawner = hit.transform.GetComponent<ItemSpawner>();

                spawner.OnHover();

                if (Input.GetMouseButtonDown(0) && !IsTaskInProgress(spawner.activeTask.taskCode))
                {
                    spawner.OnClick();
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

        if (fadeOutMessageText)
        {
            float messageTextAlpha = messageText.color.a;
            if (messageTextAlpha > 0)
            {
                messageTextAlpha -= Time.fixedDeltaTime / fadeDuration;
                messageText.color = new Color(1, 1, 1, messageTextAlpha);
            } else
            {
                fadeOutMessageText = false;
                messageText.enabled = false;
            }
        }
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
            ChangeLayers("Player");
        } else
        {
            ChangeLayers("LocalPlayer");
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
        usernameText.color = isReady ? GameManager.instance.readyColour : GameManager.instance.unreadyTextColour;
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
                mr.material.color = isSeekerColour? seekerColour : hiderColour;
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

    public void SetCountdown(int countdownValue)
    {
        SetMessage(countdownValue.ToString(), 0.9f);
    }

    public void SetSpecialMessage(string _message)
    {
        SetMessage(_message, 4f);
    }
    
    private void SetMessage(string _message, float _duration = 1)
    {
        messageText.enabled = true;
        messageText.text = _message;
        messageText.color = new Color(1, 1, 1, 1);
        fadeDuration = _duration;
        fadeOutMessageText = true;
    }

    public void PlayerTeleported(Vector3 position)
    {
        thirdPersonCamera.GetComponent<FollowPlayer>().PlayerTeleportedToPosition(position);
    }

    public void ItemPickedUp(TaskCode code) 
    {
        activeTasks.Add(code);
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
}

public enum PlayerType
{
    Default = 0,
    Hunter,
    Hider,
    Spectator
}
