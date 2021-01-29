using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject connectPanel;
    [SerializeField] private GameObject disconnectPanel;
    [SerializeField] private GameObject customisationPanel;
    //[SerializeField] private GameObject lobbyPanel;
    public TMP_InputField ipField;
    public TMP_InputField usernameField;

    [SerializeField] private MeshRenderer[] readyGemRenderers;
    
    public Dictionary<int, int> playerReadyGems = new Dictionary<int, int>();

    [SerializeField] private ColourSelectorUI hiderColourSelector;
    [SerializeField] private ColourSelectorUI seekerColourSelector;

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

        DisplayStartPanel();
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Username"))
        {
            usernameField.text = PlayerPrefs.GetString("Username");
        }
        if (PlayerPrefs.HasKey("LastRoomCode"))
        {
            ipField.text = PlayerPrefs.GetString("LastRoomCode");
        }
    }

    private void DisableAllPanels()
    {
        startPanel.SetActive(false);
        connectPanel.SetActive(false);
        disconnectPanel.SetActive(false);
        customisationPanel.SetActive(false);
    }

    public void DisplayStartPanel()
    {
        DisableAllPanels();
        startPanel.SetActive(true);
    }

    public void DisplayConnectPanel()
    {
        DisableAllPanels();
        connectPanel.SetActive(true);
        
        usernameField.interactable = true;
        ipField.interactable = true;
    }

    public void DisplayLobbyPanel()
    {
        //lobbyPanel.SetActive(true);

        foreach (PlayerManager player in GameManager.players.Values)
        {
            //playerReadyPanels.Add(player.id, Instantiate(playerReadyPanelPrefab, playerReadyParentPanel));
            //playerReadyPanels[player.id].color = player.isReady ? Color.green : Color.grey;
        }
    }

    public void DisplayDisconnectPanel()
    {
        bool isActive = disconnectPanel.activeSelf;
        DisableAllPanels();
        if (!isActive)
        {
            disconnectPanel.SetActive(true);
        }
    }

    public void DisplayCustomisationPanel()
    {
        bool isActive = customisationPanel.activeSelf;
        DisableAllPanels();
        if (!isActive)
        {
            customisationPanel.SetActive(true);
        }
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
                readyGemRenderers[i].material.color = GameManager.players[playerReadyGems[i]].isReady ? GameManager.instance.readyColour : GameManager.instance.unreadyColour;
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

    public void DisplayGamePanel()
    {
        //lobbyPanel.SetActive(false);
    }

    public void OnConnectButtonPressed()
    {
        DisableAllPanels();
        usernameField.interactable = false;
        ipField.interactable = false;

        PlayerPrefs.SetString("Username", usernameField.text);
        PlayerPrefs.SetString("LastRoomCode", ipField.text);
        PlayerPrefs.Save();

        Client.instance.ConnectToServer(ipField.text);
    }

    public void OnDisconnectButtonPressed()
    {
        Client.instance.Disconnect();
        GameManager.instance.OnLocalPlayerDisconnection();

        DisplayConnectPanel();
    }

    public void OnHiderColourChangeButtonPressed(ColourItem colourItem)
    {
        hiderColourSelector.UpdateAllButtons(colourItem);

        DisableAllPanels();

        GameManager.instance.GetLocalPlayer().ChangeBodyColour(colourItem.colour, false);

        ClientSend.SetPlayerColour(colourItem.colour, false);
    }
    
    public void OnSeekerColourChangeButtonPressed(ColourItem colourItem)
    {
        seekerColourSelector.UpdateAllButtons(colourItem);

        DisableAllPanels();

        GameManager.instance.GetLocalPlayer().ChangeBodyColour(colourItem.colour, true);

        ClientSend.SetPlayerColour(colourItem.colour, true);
    }

    [SerializeField] private GameObject[] tabs;
    [SerializeField] private GameObject[] tabContents;
    public void OnTabPressed(int tabIndex)
    {
        if (tabIndex >= tabs.Length || tabIndex < 0)
        {
            throw new System.Exception($"ERROR: Tab for tabIndex({tabIndex}) does not exist.");
        }

        for (int i = 0; i < tabs.Length; i++)
        {
            if (i != tabIndex)
            {
                tabs[i].SetActive(false);
                tabContents[i].SetActive(false);
            } else
            {
                tabs[i].SetActive(true);
                tabContents[i].SetActive(true);
            }
        }
    }

    private void OnLevelFinishedLoading(Scene _scene, LoadSceneMode _loadSceneMode)
    {
        if (_scene.name == "Lobby") //needs replacing with enum or int
        {
            DisplayConnectPanel();
        }
    }
}
