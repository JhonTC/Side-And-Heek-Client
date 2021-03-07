using Server;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pickup : MonoBehaviour
{
    public int pickupId;
    public PickupSpawner spawner;
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

    public void Init(int _pickupId, PickupSpawner _spawner, PickupType _pickupType, int _code, bool _trackMovement)
    {
        pickupId = _pickupId;
        pickupType = _pickupType;
        spawner = _spawner;

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

        basePosition = transform.position;
    }

    private void Start()
    {
        camera = LobbyManager.instance.GetLocalPlayer().thirdPersonCamera;
    }

    private void Update()
    {
        if (cursorHovering)
        {
            uiPanel.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
        else if (!cursorHovering)
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
        cursorHovering = true;
    }

    public void OnClick(int playerId)
    {
        if (interractable)
        {
            if (GameManager.instance.gameType == GameType.Multiplayer)
            {
                ClientSend.PickupSelected(pickupId);
            }
            else
            {
                //S_PickupSpawner pickup = this as S_PickupSpawner;
                //pickup.PickupPickedUp(playerId);
            }

            interractable = false;
        }
    }

    public void PickupPickedUp()
    {
        if (spawner != null)
        {
            spawner.PickupPickedUp();
        }

        if (PickupManager.pickups.ContainsKey(pickupId))
        {
            PickupManager.pickups.Remove(pickupId);
        }

        Destroy(gameObject);
    }
}
