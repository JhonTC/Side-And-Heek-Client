using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private Transform playerReadyParentPanel;
    [SerializeField] private Image playerReadyPanelPrefab;
    public InputField ipField;
    public InputField usernameField;

    Dictionary<int, Image> playerReadyPanels = new Dictionary<int, Image>();

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
        lobbyPanel.SetActive(false);
    }

    public void DisplayLobbyPanel()
    {
        lobbyPanel.SetActive(true);

        foreach (PlayerManager player in GameManager.players.Values)
        {
            playerReadyPanels.Add(player.id, Instantiate(playerReadyPanelPrefab, playerReadyParentPanel));
            playerReadyPanels[player.id].color = player.isReady ? Color.green : Color.grey;
        }
    }

    public void RemovePlayerReady(int _playerId)
    {
        if (playerReadyPanels.ContainsKey(_playerId))
        {
            Destroy(playerReadyPanels[_playerId].gameObject);
            playerReadyPanels.Remove(_playerId);

            UpdateLobbyPanel();
        }
    }

    public void AddPlayerReady(int _playerId)
    {
        if (!playerReadyPanels.ContainsKey(_playerId))
        {
            playerReadyPanels.Add(_playerId, Instantiate(playerReadyPanelPrefab, playerReadyParentPanel));

            UpdateLobbyPanel();
        }
    }

    public void UpdateLobbyPanel()
    {
        foreach (KeyValuePair<int, Image> playerReadyPanel in playerReadyPanels)
        {
            playerReadyPanel.Value.color = GameManager.players[playerReadyPanel.Key].isReady ? Color.green : Color.grey;
        }
    }

    public void DisplayGamePanel()
    {
        lobbyPanel.SetActive(false);
    }



    public void OnConnectButtonPressed()
    {
        startPanel.SetActive(false);
        usernameField.interactable = false;
        ipField.interactable = false;

        Client.instance.ConnectToServer(ipField.text);
    }
}
