using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startPanel;
    //[SerializeField] private GameObject lobbyPanel;
    public InputField ipField;
    public InputField usernameField;

    [SerializeField] private MeshRenderer[] readyGemRenderers;
    [SerializeField] private Color unreadyColour;
    [SerializeField] private Color readyColour;


    Dictionary<int, int> playerReadyGemDictionary = new Dictionary<int, int>();

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
        //lobbyPanel.SetActive(false);
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

    public void RemovePlayerReady(int _playerId)
    {
        if (playerReadyGemDictionary.ContainsValue(_playerId))
        {
            foreach (KeyValuePair<int, int> playerReadyGem in playerReadyGemDictionary)
            {
                if (playerReadyGem.Value == _playerId)
                {
                    playerReadyGemDictionary.Remove(playerReadyGem.Key);
                }
            }

            UpdateLobbyPanel();
        }
    }

    public void AddPlayerReady(int _playerId)
    {
        if (!playerReadyGemDictionary.ContainsValue(_playerId))
        {
            playerReadyGemDictionary.Add(GetNextAvaliableGemIndex(), _playerId);
            UpdateLobbyPanel();
        }
    }

    public void UpdateLobbyPanel()
    {
        for (int i = 0; i < readyGemRenderers.Length; i++)
        {
            if (playerReadyGemDictionary.ContainsKey(i)) {
                readyGemRenderers[i].enabled = true;
                readyGemRenderers[i].material.color = GameManager.players[playerReadyGemDictionary[i]].isReady ? readyColour : unreadyColour;
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
            if (!playerReadyGemDictionary.ContainsKey(i))
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
}
