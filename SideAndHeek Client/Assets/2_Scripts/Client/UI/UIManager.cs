using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public enum UIPanelType
{
    Start = 0,
    Connect,
    Gameplay,
    Settings,
    Pause,
    Quit,
    Customisation,
    Game_Rules,
    Spectate
}

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private UIPanel startPanel;
    public ConnectUI connectPanel;
    public GameplayUI gameplayPanel;
    [SerializeField] private SettingsUI settingsPanel;
    [SerializeField] private UIPanel pausePanel;
    [SerializeField] private UIPanel quitPanel;
    public CustomisationUI customisationPanel;
    public GameRulesUI gameRulesPanel;
    public SpectateUI spectatePanel;
    public CurrencyUI currencyUI;

    [SerializeField] private MeshRenderer[] readyGemRenderers;
    
    public Dictionary<int, ushort> playerReadyGems = new Dictionary<int, ushort>();

    private bool fadeOutMessageText = false;
    private float fadeDuration = 1f;
    [SerializeField] private TextMeshProUGUI messageText;

    [SerializeField] private Image backButton;

    [SerializeField] Sprite backIcon;
    [SerializeField] Sprite closeIcon;

    private Stack<UIPanel> panelHistory = new Stack<UIPanel>();
    private List<UIPanel> otherActivePanels = new List<UIPanel>();

    private Dictionary<UIPanelType, UIPanel> panelDictionary = new Dictionary<UIPanelType, UIPanel>();

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

        InitPanelDictionary();
    }

    private void Start()
    {
        DisplayPanel(startPanel);
        //DisplayCurrencyPanel();

        settingsPanel.Init();
        gameRulesPanel.Init();
    }

    private void InitPanelDictionary()
    {
        panelDictionary.Add(startPanel.panelType, startPanel);
        panelDictionary.Add(connectPanel.panelType, connectPanel);
        panelDictionary.Add(gameplayPanel.panelType, gameplayPanel);
        panelDictionary.Add(settingsPanel.panelType, settingsPanel);
        panelDictionary.Add(pausePanel.panelType, pausePanel);
        panelDictionary.Add(quitPanel.panelType, quitPanel);
        panelDictionary.Add(customisationPanel.panelType, customisationPanel);
        panelDictionary.Add(gameRulesPanel.panelType, gameRulesPanel);
        panelDictionary.Add(spectatePanel.panelType, spectatePanel);
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

    private void OnApplicationQuit()
    {
        settingsPanel.OnQuit();
    }

    public bool IsUIActive()
    {
        return panelHistory.Count > 0;
    }

    public void DisplayPanel(UIPanelType panelType, bool overrideInputToUI = false)
    {
        DisplayPanel(panelDictionary[panelType], overrideInputToUI);
    }
    public void DisplayPanel(UIPanel panel) //for ui event calls in editor
    {
        DisplayPanel(panel, false);
    }
    public void DisplayPanel(UIPanel panel, bool overrideInputToUI = false)
    {
        if (panel.autoToggle)
        {
            if (!panelHistory.Contains(panel))
            {
                if (panelHistory.Count > 0)
                {
                    panelHistory.Peek().DisablePanel();
                }

                panelHistory.Push(panel);
                panel.EnablePanel();

                UpdateBackButton();
            }

            InputHandler.instance.SwitchInput("UI");
        } else
        {
            if (!otherActivePanels.Contains(panel))
            {
                otherActivePanels.Add(panel);
                panel.EnablePanel();

                if (overrideInputToUI)
                {
                    InputHandler.instance.SwitchInput("UI");
                }
            }
        }
    }

    public void TogglePanel(UIPanelType panelType)
    {
        bool toggleValue = true;
        if (panelHistory.Count > 0)
        {
            UIPanel peekPanel = panelHistory.Peek();
            if (peekPanel.panelType == panelType)
            {
                toggleValue = false;
            }
        }

        if (toggleValue)
        {
            DisplayPanel(panelDictionary[panelType]);
        }
        else
        {
            OnBackButtonPressed();
        }
    }

    public void CloseHistoryPanels()
    {
        if (panelHistory.Count > 0)
        {
            panelHistory.Pop().gameObject.SetActive(false);
            panelHistory.Clear();
        }

        UpdateBackButton();
    }

    public void ClosePanel(UIPanelType panelType, bool overrideInputToPlayerControls = false)
    {
        UIPanel panel = panelDictionary[panelType];
        panelDictionary[panelType].gameObject.SetActive(false);
        otherActivePanels.Remove(panel);

        if (overrideInputToPlayerControls)
        {
            InputHandler.instance.SwitchInput("PlayerControls");
        }
    }

    public void CloseAllPanels()
    {
        CloseHistoryPanels();

        foreach (UIPanel panel in otherActivePanels)
        {
            panel.gameObject.SetActive(false);
        }
        otherActivePanels.Clear();
    }

    public void RemovePlayerReady(ushort _playerId)
    {
        if (playerReadyGems.ContainsValue(_playerId))
        {
            List<int> readyPlayerGemKeysToRemove = new List<int>();
            foreach (KeyValuePair<int, ushort> playerReadyGem in playerReadyGems)
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

    public void AddPlayerReady(ushort _playerId)
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

        gameplayPanel.UpdatePlayerTypeViews();
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
        DisplayPanel(connectPanel);
    }

    public void OnTutorialButtonPressed()
    {
        //SceneManager.LoadScene("S_Lobby", LoadSceneMode.Single);
        
        //DisplayGameplayPanel();

        //LocalGameManager localGameManager = GameManager.instance as LocalGameManager;
        //localGameManager.SpawnPlayer();
    }

    public void BackToMenu()
    {
        CloseAllPanels();

        DisplayPanel(connectPanel);

        customisationPanel.hiderColourSelector.ClearAll();
    }

    public void OnDisconnectButtonPressed()
    {
        NetworkManager.Instance.Client.Disconnect();
    }

    public void OnBackButtonPressed()
    {
        if (panelHistory.Count > 0)
        {
            UIPanel peekPanel = panelHistory.Peek();
            if (peekPanel != connectPanel && peekPanel != startPanel)
            {
                panelHistory.Pop().DisablePanel();
            }
        }

        if (panelHistory.Count > 0)
        {
            panelHistory.Peek().EnablePanel();
        } else
        {
            InputHandler.instance.SwitchInput("PlayerControls");
        }

        UpdateBackButton();
    }

    public void UpdateBackButton()
    {
        bool isActive = false;
        if (panelHistory.Count > 0)
        {
            UIPanel peekPanel = panelHistory.Peek();
            if (peekPanel != connectPanel && peekPanel != startPanel)
            {
                isActive = true;
                backButton.sprite = (panelHistory.Count == 1) ? closeIcon : backIcon;
            }
        }
        
        if (backButton.gameObject.activeSelf != isActive)
        {
            backButton.gameObject.SetActive(isActive);
        }
    }

    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }

    private void OnLevelFinishedLoading(Scene _scene, LoadSceneMode _loadSceneMode) //todo:remove?
    {
        if (_scene.name == "Lobby") //TODO: needs replacing with enum or Id
        {
            DisplayPanel(connectPanel);
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
