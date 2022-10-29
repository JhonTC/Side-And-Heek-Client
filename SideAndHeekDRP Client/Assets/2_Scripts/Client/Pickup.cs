using Server;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pickup : SpawnableObject
{
    public PickupSpawner spawner;
    public BoxCollider collider;

    [SerializeField] private GameObject pickupObj;

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

    [SerializeField] private Color hiderPickupColour;
    [SerializeField] private Color hunterPickupColour;
    [SerializeField] private Color defaultPickupColour;


    Camera camera;

    public void Init(ushort _pickupId, PickupSpawner _spawner, ushort _creatorId, int _code)
    {
        base.Init(_pickupId, _creatorId, _code);

        spawner = _spawner;
        
        //pickupTitle.text = activeObjectDetails.pickupSO.pickupName;
        //pickupContent.text = activeObjectDetails.pickupSO.pickupContent;
        //pickupOutline.color = activeObjectDetails.pickupSO.pickupLevel.color;

        //uiPanel.gameObject.SetActive(true);

        basePosition = transform.position;

        Color pickupColour = defaultPickupColour;
        if (activeObjectDetails.pickupSO.userType == PlayerType.Hunter)
        {
            pickupColour = hunterPickupColour;
        }
        else if (activeObjectDetails.pickupSO.userType == PlayerType.Hider)
        {
            pickupColour = hiderPickupColour;
        }

        pickupObj.GetComponent<MeshRenderer>().material.color = pickupColour;
    }

    private void Start()
    {
        camera = LobbyManager.instance.GetLocalPlayer().thirdPersonCamera;

        pickupObj.GetComponent<MeshRenderer>().material.SetFloat("_Random", Random.Range(0f, 1000f));
    }

    private void Update()
    {
        if (cursorHovering)
        {
            //uiPanel.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
        else if (!cursorHovering)
        {
            //uiPanel.localScale = new Vector3(0.02f, 0.02f, 0.02f);

            //uiPanel.transform.position = basePosition + new Vector3(0f, 0.25f * Mathf.Sin(Time.time * bobSpeed), 0f);

            pickupObj.transform.position = basePosition + new Vector3(0f, 0.25f * Mathf.Sin(Time.time * bobSpeed), 0f);
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
                ClientSend.PickupSelected(objectId);
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

        if (PickupHandler.pickups.ContainsKey(objectId))
        {
            PickupHandler.pickups.Remove(objectId);
        }

        Destroy(gameObject);
    }
}
