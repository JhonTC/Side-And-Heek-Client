﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Server;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject connectPanel;
    public GameplayUI gameplayPanel;
    [SerializeField] private SettingsUI settingsPanel;
    [SerializeField] private GameObject pausePanel;
    public CustomisationUI customisationPanel;
    public GameRulesUI gameRulesPanel;
    //[SerializeField] private GameObject lobbyPanel;
    public TMP_InputField ipField;
    public TMP_InputField usernameField;

    [SerializeField] private MeshRenderer[] readyGemRenderers;
    
    public Dictionary<int, int> playerReadyGems = new Dictionary<int, int>();

    [SerializeField] private ColourSelectorUI hiderColourSelector;
    [SerializeField] private ColourSelectorUI seekerColourSelector;

    private bool m_IsUIActive = false;

    public bool isUIActive { get { return m_IsUIActive || gameplayPanel.isActive; } }

    private bool fadeOutMessageText = false;
    private float fadeDuration = 1f;
    [SerializeField] private TextMeshProUGUI messageText;

    [SerializeField] private GameObject activePanel;
    [SerializeField] private GameObject lastActivePanel;

    [SerializeField] private GameObject connectLoadingPanel;
    [SerializeField] private GameObject connectTitlePanel;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        DisplayStartPanel();

        if (PlayerPrefs.HasKey("Username"))
        {
            usernameField.text = PlayerPrefs.GetString("Username");
        }
        if (PlayerPrefs.HasKey("LastRoomCode"))
        {
            ipField.text = PlayerPrefs.GetString("LastRoomCode");
        }
    }

    private void FixedUpdate()
    {
        if (fadeOutMessageText)
        {
            float messageTextAlpha = messageText.color.a;
            if (messageTextAlpha > 0)
            {
                messageTextAlpha -= Time.fixedDeltaTime / fadeDuration;
                messageText.color = new Color(1, 1, 1, messageTextAlpha);
            }
            else
            {
                fadeOutMessageText = false;
                messageText.enabled = false;
            }
        }
    }

    public void DisableAllPanels()
    {
        m_IsUIActive = false;
        startPanel.SetActive(false);
        connectPanel.SetActive(false);
        pausePanel.SetActive(false);
        customisationPanel.gameObject.SetActive(false);
        settingsPanel.gameObject.SetActive(false);
    }

    public void DisplayStartPanel()
    {
        DisableAllPanels();
        gameplayPanel.gameObject.SetActive(false);
        startPanel.SetActive(true);

        m_IsUIActive = true;

        lastActivePanel = startPanel;
        activePanel = startPanel;
    }

    public void DisplayConnectPanel()
    {
        DisableAllPanels();
        gameplayPanel.gameObject.SetActive(false);
        connectPanel.SetActive(true);

        connectTitlePanel.SetActive(true);
        connectLoadingPanel.SetActive(false);
        usernameField.interactable = true;
        ipField.interactable = true;

        m_IsUIActive = true;

        lastActivePanel = activePanel;
        activePanel = connectPanel;
    }

    public void DisplayGameplayPanel()
    {
        DisableAllPanels();
        gameplayPanel.gameObject.SetActive(true);

        activePanel = gameplayPanel.gameObject;
    }

    public void DisplaySettingsPanel()
    {
        DisableAllPanels();
        settingsPanel.gameObject.SetActive(true);

        m_IsUIActive = true;

        lastActivePanel = activePanel;
        activePanel = connectPanel;
    }

    public void DisplayLobbyPanel()
    {
        //lobbyPanel.SetActive(true);

        foreach (PlayerManager player in LobbyManager.players.Values)
        {
            //playerReadyPanels.Add(player.id, Instantiate(playerReadyPanelPrefab, playerReadyParentPanel));
            //playerReadyPanels[player.id].color = player.isReady ? Color.green : Color.grey;
        }

        //isUIActive = true;
    }

    public void DisplayPausePanel()
    {
        bool isActive = pausePanel.activeSelf;
        DisableAllPanels();
        if (!isActive)
        {
            pausePanel.SetActive(true);
        }

        m_IsUIActive = true;

        lastActivePanel = activePanel;
        activePanel = pausePanel;
    }

    public void DisplayCustomisationPanel()
    {
        bool isActive = customisationPanel.gameObject.activeSelf;
        DisableAllPanels();
        if (!isActive)
        {
            customisationPanel.gameObject.SetActive(true);
        }

        m_IsUIActive = true;

        lastActivePanel = activePanel;
        activePanel = customisationPanel.gameObject;
    }

    public void RemovePlayerReady(int _playerId)
    {
        if (playerReadyGems.ContainsValue(_playerId))
        {
            List<int> readyPlayerGemKeysToRemove = new List<int>();
            foreach (KeyValuePair<int, int> playerReadyGem in playerReadyGems)
            {
                if (playerReadyGem.Value == _playerId)
                {
                    readyPlayerGemKeysToRemove.Add(playerReadyGem.Key);
                }
            }

            foreach (int key in readyPlayerGemKeysToRemove)
            {
                playerReadyGems.Remove(key);
            }

            UpdateLobbyPanel();
        }
    }

    public void AddPlayerReady(int _playerId)
    {
        if (!playerReadyGems.ContainsValue(_playerId))
        {
            playerReadyGems.Add(GetNextAvaliableGemIndex(), _playerId);
            UpdateLobbyPanel();
        }
    }

    public void UpdateLobbyPanel()
    {
        for (int i = 0; i < readyGemRenderers.Length; i++)
        {
            if (playerReadyGems.ContainsKey(i)) {
                readyGemRenderers[i].enabled = true;
                readyGemRenderers[i].material.SetColor("_EmissionColor", LobbyManager.players[playerReadyGems[i]].isReady ? LobbyManager.instance.readyColour : LobbyManager.instance.unreadyColour);
            } else
            {
                readyGemRenderers[i].enabled = false;
            }
        }
    }

    public int GetNextAvaliableGemIndex()
    {
        for (int i = 0; i < readyGemRenderers.Length; i++)
        {
            if (!playerReadyGems.ContainsKey(i))
            {
                return i;
            }
        }

        throw new System.Exception("No player-ready Gem Avaliable, too many players have joined.");
    }

    public void OnPlayButtonPressed()
    {
        //SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        DisplayConnectPanel();
    }

    public void OnTutorialButtonPressed()
    {
        //SceneManager.LoadScene("S_Lobby", LoadSceneMode.Single);

        DisplayGameplayPanel();

        LocalGameManager localGameManager = GameManager.instance as LocalGameManager;
        localGameManager.SpawnPlayer();
    }

    public void OnConnectButtonPressed()
    {
        usernameField.interactable = false;
        ipField.interactable = false;

        PlayerPrefs.SetString("Username", usernameField.text);
        PlayerPrefs.SetString("LastRoomCode", ipField.text);
        PlayerPrefs.Save();

        //start swirler
        connectTitlePanel.SetActive(false);
        connectLoadingPanel.SetActive(true);

        Client.instance.ConnectToServer(ipField.text);
    }

    public void OnConnectionFailed()
    {
        DisableAllPanels();
        gameplayPanel.gameObject.SetActive(false);
        connectPanel.SetActive(true);

        connectTitlePanel.SetActive(true);
        connectLoadingPanel.SetActive(false);
        usernameField.interactable = true;
        ipField.interactable = true;

        m_IsUIActive = true;
    }

    public void OnDisconnectButtonPressed()
    {
        gameplayPanel.gameObject.SetActive(false);
        Client.instance.Disconnect();
        LobbyManager.instance.OnLocalPlayerDisconnection();

        DisplayConnectPanel();
    }

    public void OnHiderColourChangeButtonPressed(ColourItem colourItem)
    {
        DisableAllPanels();

        if (GameManager.instance.gameType == GameType.Multiplayer)
        {
            ClientSend.SetPlayerColour(colourItem.colour, false);
        } else
        {
            LobbyManager.instance.GetLocalPlayer().ChangeBodyColour(colourItem.colour, false);
        }
    }
    
    public void OnSeekerColourChangeButtonPressed(ColourItem colourItem)
    {
        seekerColourSelector.UpdateAllButtons(false, colourItem);

        DisableAllPanels();

        LobbyManager.instance.GetLocalPlayer().ChangeBodyColour(colourItem.colour, true);

        if (GameManager.instance.gameType == GameType.Multiplayer)
        {
            ClientSend.SetPlayerColour(colourItem.colour, true);
        }
    }

    public void OnBackButtonPressed()
    {
        DisableAllPanels();

        lastActivePanel.SetActive(true);

        GameObject tempActivePanel = activePanel;
        activePanel = lastActivePanel;
        lastActivePanel = tempActivePanel;

        m_IsUIActive = true;
    }

    public void OnLeaveButtonPressed()
    {
        LobbyManager.instance.OnLocalPlayerDisconnection();
        GameManager.instance.gameStarted = false;

        DisplayStartPanel();
    }

    public void OnQuitButtonPressed()
    {
        //todo: handle something?
        Application.Quit();
    }

    private void OnLevelFinishedLoading(Scene _scene, LoadSceneMode _loadSceneMode)
    {
        if (_scene.name == "Lobby") //needs replacing with enum or int
        {
            DisplayConnectPanel();
        }
    }

    public void SetCountdown(int countdownValue, bool fadeOut = true)
    {
        SetMessage(countdownValue.ToString(), 0.9f, fadeOut);
    }

    public void SetSpecialMessage(string _message)
    {
        SetMessage(_message, 4f);
    }

    public void SetMessage(string _message, float _duration = 1, bool fadeOut = true)
    {
        messageText.enabled = true;
        messageText.text = _message;
        messageText.color = new Color(1, 1, 1, 1);
        fadeDuration = _duration;
        fadeOutMessageText = fadeOut;
    }
}
