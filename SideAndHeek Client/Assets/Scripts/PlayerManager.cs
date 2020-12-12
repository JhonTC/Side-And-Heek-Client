using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public int itemCount = 0;
    public bool isReady = false;
    public bool hasAuthority = false;

    [SerializeField] private TMP_Text name;

    [SerializeField] private Transform root;
    [SerializeField] private Transform head;
    [SerializeField] private Transform leftLeg;
    [SerializeField] private Transform rightLeg;
    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightFoot;

    public GameObject[] layersToChange;
    public Behaviour[] baseComponentsToDisable;
    public Behaviour[] extraComponentsToDisable;
    
    [SerializeField] private Color unreadyColour;
    [SerializeField] private Color readyColour;

    public Camera camera;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void FixedUpdate()
    {
        head.position = Vector3.Lerp(head.position, root.position, 1f);
        head.rotation = Quaternion.Lerp(head.rotation, root.rotation, 1f);
    }

    public void Init(int _id, string _username, bool _hasAuthority, bool _isHunter)
    {
        id = _id;
        username = _username;
        hasAuthority = _hasAuthority;

        if (username == "")
        {
            username = $"Player {id}";
        }

        name.text = username;

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
        camera.enabled = false;
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
        ChangeBodyColour(isReady ? readyColour : unreadyColour);
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
}
