using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public int itemCount = 0;
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

    public Camera playerCamera;

    private bool fadeOutMessageText = false;
    private float fadeDuration = 1f;

    private void Awake()
    {
        DontDestroyOnLoad(this);
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
        playerCamera.enabled = false;
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
        ChangeBodyColour(isReady ? GameManager.instance.readyColour : GameManager.instance.unreadyColour);
        UIManager.instance.UpdateLobbyPanel();
    }

    private void ChangeBodyColour(Color colour)
    {
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            if (mr.tag != "Eye")
            {
                mr.material.color = colour;
            }
        }
    }

    public void SetPlayerType(PlayerType _playerType)
    {
        playerType = _playerType;

        switch(playerType)
        {
            case PlayerType.Default:
                //gameStartFailed
                break;
            case PlayerType.Hunter:
                ChangeBodyColour(GameManager.instance.hunterColour);
                break;
            case PlayerType.Hider:

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
}

public enum PlayerType
{
    Default = 0,
    Hunter,
    Hider,
    Spectator
}
