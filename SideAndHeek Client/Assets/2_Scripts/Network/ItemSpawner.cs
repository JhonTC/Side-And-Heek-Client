using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpawner : MonoBehaviour
{
    public int spawnerId;
    public bool hasItem;
    public BoxCollider collider;

    [SerializeField] private RectTransform uiPanel;
    [SerializeField] private TMP_Text taskTitle; 
    [SerializeField] private TMP_Text taskContent;
    [SerializeField] private Image taskOutline;

    public float rotationSpeed = 100f;
    public float bobSpeed = 2f;

    private Vector3 basePosition;

    public bool interractable = true;
    public bool cursorHovering = false;
    private bool lastCursorHovering = false;

    Camera camera;

    public class TaskDetails
    {
        public TaskCode taskCode;
        public string taskName;
        public string taskContent;
        public Color taskDifficulty;

        public TaskDetails(TaskCode _taskCode, string _taskName, string _taskContent, Color _taskDifficulty)
        {
            taskCode = _taskCode;
            taskName = _taskName;
            taskContent = _taskContent;
            taskDifficulty = _taskDifficulty;
        }
    }

    public TaskDetails activeTask;

    public void Init(int _spawnerId, bool _hasItem, TaskCode code, string taskName, string _taskContent, Color taskDifficulty)
    {
        if (_hasItem)
        {
            activeTask = new TaskDetails(code, taskName, _taskContent, taskDifficulty);

            taskTitle.text = activeTask.taskName;
            taskContent.text = activeTask.taskContent;
            taskOutline.color = activeTask.taskDifficulty;
        }

        spawnerId = _spawnerId;
        hasItem = _hasItem;
        uiPanel.gameObject.SetActive(_hasItem);

        basePosition = transform.position;
    }

    private void Start()
    {
        camera = GameManager.instance.GetLocalPlayer().thirdPersonCamera;
    }

    private void Update()
    {
        if (hasItem && cursorHovering) {
            uiPanel.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        } 
        else if (hasItem && !cursorHovering)
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
        if (hasItem)
        {
            cursorHovering = true;
        }
    }

    public void OnClick()
    {
        if (hasItem && interractable)
        {
            ClientSend.TaskSelected(spawnerId);
            interractable = false;
        }
    }

    public void ItemSpawned(TaskCode code, string taskName, string _taskContent, Color taskDifficulty)
    {
        activeTask = new TaskDetails(code, taskName, _taskContent, taskDifficulty);

        taskTitle.text = activeTask.taskName;
        taskContent.text = activeTask.taskContent;
        taskOutline.color = activeTask.taskDifficulty;

        hasItem = true;
        uiPanel.gameObject.SetActive(true);
        collider.enabled = true;
        interractable = true;
    }

    public void ItemPickedUp()
    {
        hasItem = false;
        uiPanel.gameObject.SetActive(false);
        collider.enabled = false;
    }
}
