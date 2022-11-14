using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectUI : UIPanel
{
    [SerializeField] private GameObject connectLoadingPanel;
    [SerializeField] private GameObject connectTitlePanel;
    [SerializeField] private Button connectButton;

    public TMP_InputField ipField;
    public TMP_InputField usernameField;

    private void Start()
    {
        usernameField.text = PlayerPrefs.GetString("Username", string.Empty);
        ipField.text = PlayerPrefs.GetString("LastRoomCode", string.Empty);
    }

    public override void EnablePanel()
    {
        base.EnablePanel();

        connectTitlePanel.SetActive(true);
        connectLoadingPanel.SetActive(false);
        usernameField.interactable = true;
        ipField.interactable = true;
        connectButton.interactable = true;
    }

    public void OnConnectButtonPressed()
    {
        usernameField.interactable = false;
        ipField.interactable = false;
        connectButton.interactable = false;

        PlayerPrefs.SetString("Username", usernameField.text);
        PlayerPrefs.SetString("LastRoomCode", ipField.text);
        PlayerPrefs.Save();

        //start swirler
        connectTitlePanel.SetActive(false);
        connectLoadingPanel.SetActive(true);

        NetworkManager.Instance.Connect(ipField.text);
    }

    public string GetName()
    {
        return usernameField.text;
    }
}
