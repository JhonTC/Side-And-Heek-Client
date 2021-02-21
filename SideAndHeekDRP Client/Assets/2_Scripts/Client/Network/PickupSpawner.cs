using Server;
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
    public TaskDetails activeTaskDetails;
    public ItemDetails activeItemDetails;

    public void Init(int _spawnerId, PickupType _pickupType, bool _hasPickup, int _code)
    {
        spawnerId = _spawnerId;
        pickupType = _pickupType;
        hasPickup = _hasPickup;

        if (hasPickup)
        {
            if (pickupType == PickupType.Task)
            {
                activeTaskDetails = GameManager.instance.collection.GetTaskByCode((TaskCode)_code);

                pickupTitle.text = activeTaskDetails.task.pickupName;
                pickupContent.text = activeTaskDetails.task.pickupContent;
                pickupOutline.color = activeTaskDetails.task.pickupLevel.color;
            }
            else if (pickupType == PickupType.Item)
            {
                activeItemDetails = GameManager.instance.collection.GetItemByCode((ItemCode)_code);

                pickupTitle.text = activeItemDetails.item.pickupName;
                pickupContent.text = activeItemDetails.item.pickupContent;
                pickupOutline.color = activeItemDetails.item.pickupLevel.color;
            }
        }

        uiPanel.gameObject.SetActive(hasPickup);

        basePosition = transform.position;
    }

    protected virtual void Start()
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

    public void OnClick(int playerId)
    {
        if (hasPickup && interractable)
        {
            if (GameManager.instance.gameType == GameType.Multiplayer)
            {
                ClientSend.PickupSelected(spawnerId);
            } else
            {
                Pickup pickup = this as Pickup;
                pickup.PickupPickedUp(playerId);
                ItemPickedUp();
            }
            interractable = false;
        }
    }

    public void PickupSpawned(PickupType _pickupType, int _code)
    {
        pickupType = _pickupType;
        hasPickup = true;

        if (hasPickup)
        {
            if (pickupType == PickupType.Task)
            {
                activeTaskDetails = GameManager.instance.collection.GetTaskByCode((TaskCode)_code);

                pickupTitle.text = activeTaskDetails.task.pickupName;
                pickupContent.text = activeTaskDetails.task.pickupContent;
                pickupOutline.color = activeTaskDetails.task.pickupLevel.color;
            }
            else if (pickupType == PickupType.Item)
            {
                activeItemDetails = GameManager.instance.collection.GetItemByCode((ItemCode)_code);

                pickupTitle.text = activeItemDetails.item.pickupName;
                pickupContent.text = activeItemDetails.item.pickupContent;
                pickupOutline.color = activeItemDetails.item.pickupLevel.color;
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
