using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject optionsPanel;
    //[SerializeField] private GameObject lobbyPanel;
    public InputField ipField;
    public InputField usernameField;

    [SerializeField] private MeshRenderer[] readyGemRenderers;
    
    public Dictionary<int, int> playerReadyGems = new Dictionary<int, int>();

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

    public void DisplayStartPanel()
    {
        startPanel.SetActive(true);
        optionsPanel.SetActive(false);
        //lobbyPanel.SetActive(false);
        
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

    public void DisplayOptionsPanel()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
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
        startPanel.SetActive(false);
        usernameField.interactable = false;
        ipField.interactable = false;

        Client.instance.ConnectToServer(ipField.text);
    }

    public void OnDisconnectButtonPressed()
    {
        Client.instance.Disconnect();
        optionsPanel.SetActive(false);
    }

    private void OnLevelFinishedLoading(Scene _scene, LoadSceneMode _loadSceneMode)
    {
        if (_scene.name == "Lobby") //needs replacing with enum or int
        {
            DisplayStartPanel();
        }
    }
}
