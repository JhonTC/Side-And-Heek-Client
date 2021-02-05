using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickupSpawner : MonoBehaviour
{
    public int spawnerId;
    public bool hasPickup;
    public BoxCollider collider;

    [SerializeField] private RectTransform uiPanel;
    [SerializeField] private TMP_Text pickupTitle; 
    [SerializeField] private TMP_Text pickupContent;
    [SerializeField] private Image pickupOutline;

    public float rotationSpeed = 100f;
    public float bobSpeed = 2f;

    private Vector3 basePosition;

    public bool interractable = true;
    public bool cursorHovering = false;
    private bool lastCursorHovering = false;

    Camera camera;

    public PickupType pickupType = PickupType.NULL;
    public TaskDetails activeTask;
    public ItemDetails activeItem;

    public class PickupDetails
    {
        public string pickupName;
        public string pickupContent;
        public Color pickupDifficulty;

        public PickupDetails(string _pickupName, string _pickupContent, Color _pickupDifficulty)
        {
            pickupName = _pickupName;
            pickupContent = _pickupContent;
            pickupDifficulty = _pickupDifficulty;
        }
    }
    public class TaskDetails : PickupDetails
    {
        public TaskCode taskCode;

        public TaskDetails(TaskCode _taskCode, string _pickupName, string _pickupContent, Color _pickupDifficulty) : base(_pickupName, _pickupContent, _pickupDifficulty)
        {
            taskCode = _taskCode;
        }
    }
    public class ItemDetails: PickupDetails
    {
        public ItemCode itemCode;

        public ItemDetails(ItemCode _itemCode, string _pickupName, string _pickupContent, Color _pickupDifficulty) : base(_pickupName, _pickupContent, _pickupDifficulty)
        {
            itemCode = _itemCode;
        }
    }

    public void Init(int _spawnerId, PickupType _pickupType, bool _hasPickup, int _code, string _pickupName, string _pickupContent, Color _pickupLevel)
    {
        spawnerId = _spawnerId;
        pickupType = _pickupType;
        hasPickup = _hasPickup;

        if (hasPickup)
        {
            if (pickupType == PickupType.Task)
            {
                activeTask = new TaskDetails((TaskCode)_code, _pickupName, _pickupContent, _pickupLevel);

                pickupTitle.text = activeTask.pickupName;
                pickupContent.text = activeTask.pickupContent;
                pickupOutline.color = activeTask.pickupDifficulty;
            }
            else if (pickupType == PickupType.Item)
            {
                activeItem= new ItemDetails((ItemCode)_code, _pickupName, _pickupContent, _pickupLevel);

                pickupTitle.text = activeItem.pickupName;
                pickupContent.text = activeItem.pickupContent;
                pickupOutline.color = activeItem.pickupDifficulty;
            }
        }

        uiPanel.gameObject.SetActive(hasPickup);

        basePosition = transform.position;
    }

    private void Start()
    {
        camera = LobbyManager.instance.GetLocalPlayer().thirdPersonCamera;
    }

    private void Update()
    {
        if (hasPickup && cursorHovering) {
            uiPanel.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        } 
        else if (hasPickup && !cursorHovering)
        {
            uiPanel.localScale = new Vector3(0.02f, 0.02f, 0.02f);

            uiPanel.transform.position = basePosition + new Vector3(0f, 0.25f * Mathf.Sin(Time.time * bobSpeed), 0f);
        }

        Vector3 cameraPos = camera.transform.position; //!!
        cameraPos.x = uiPanel.transform.position.x;
        uiPanel.transform.LookAt(cameraPos);

        cursorHovering = false;
    }

    public void OnHover()
    {
        if (hasPickup)
        {
            cursorHovering = true;
        }
    }

    public void OnClick()
    {
        if (hasPickup && interractable)
        {
            ClientSend.TaskSelected(spawnerId);
            interractable = false;
        }
    }

    public void PickupSpawned(PickupType _pickupType, int _code, string _pickupName, string _pickupContent, Color _pickupLevel)
    {
        pickupType = _pickupType;
        hasPickup = true;

        if (hasPickup)
        {
            if (pickupType == PickupType.Task)
            {
                activeTask = new TaskDetails((TaskCode)_code, _pickupName, _pickupContent, _pickupLevel);

                pickupTitle.text = activeTask.pickupName;
                pickupContent.text = activeTask.pickupContent;
                pickupOutline.color = activeTask.pickupDifficulty;
            }
            else if (pickupType == PickupType.Item)
            {
                activeItem = new ItemDetails((ItemCode)_code, _pickupName, _pickupContent, _pickupLevel);

                pickupTitle.text = activeItem.pickupName;
                pickupContent.text = activeItem.pickupContent;
                pickupOutline.color = activeItem.pickupDifficulty;
            }

            uiPanel.gameObject.SetActive(true);
            collider.enabled = true;
            interractable = true;
        }
    }

    public void ItemPickedUp()
    {
        hasPickup = false;
        uiPanel.gameObject.SetActive(false);
        collider.enabled = false;
    }
}
