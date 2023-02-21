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

    [SerializeField] private GameObject jtsLoadingPanel;
    [SerializeField] private GameObject jtsTitlePanel;
    [SerializeField] private Button joinTestServerButton;

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

        jtsTitlePanel.SetActive(true);
        jtsLoadingPanel.SetActive(false);
        joinTestServerButton.interactable = true;
    }

    public void OnConnectButtonPressed()
    {
        usernameField.interactable = false;
        ipField.interactable = false;
        connectButton.interactable = false;
        joinTestServerButton.interactable = false;

        PlayerPrefs.SetString("Username", usernameField.text);
        PlayerPrefs.SetString("LastRoomCode", ipField.text);
        PlayerPrefs.Save();

        //start swirler
        connectTitlePanel.SetActive(false);
        connectLoadingPanel.SetActive(true);

        NetworkManager.Instance.Connect(ipField.text);
    }

    public void OnJoinTestServerButtonPressed()
    {
        usernameField.interactable = false;
        ipField.interactable = false;
        connectButton.interactable = false;
        joinTestServerButton.interactable = false;

        PlayerPrefs.SetString("Username", usernameField.text);
        PlayerPrefs.SetString("LastRoomCode", ipField.text);
        PlayerPrefs.Save();

        //start swirler
        jtsTitlePanel.SetActive(false);
        jtsLoadingPanel.SetActive(true);

        NetworkManager.Instance.Connect("3.10.164.103");
    }

    public string GetName()
    {
        return usernameField.text;
    }
}
