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

    [SerializeField] private GameObject hostLoadingPanel;
    [SerializeField] private GameObject hostTitlePanel;
    [SerializeField] private Button hostButton;

    [SerializeField] private GameObject jtsLoadingPanel;
    [SerializeField] private GameObject jtsTitlePanel;
    [SerializeField] private Button joinTestServerButton;

    [SerializeField] private Image useIPIconImage;
    [SerializeField] private Sprite activeIcon;
    [SerializeField] private Sprite inactiveIcon;
    private bool useIP = false;

    public TMP_InputField ipField;
    public TMP_Text ipFieldPlaceholderText;
    public TMP_InputField usernameField;

    private void Start()
    {
        usernameField.text = PlayerPrefs.GetString("Username", string.Empty);
        //ipField.text = PlayerPrefs.GetString("LastRoomCode", string.Empty);

        ToggleUseIP();
    }

    public override void EnablePanel()
    {
        base.EnablePanel();

        usernameField.interactable = true;
        ipField.interactable = true;

        connectTitlePanel.SetActive(true);
        connectLoadingPanel.SetActive(false);
        connectButton.interactable = true;

        hostTitlePanel.SetActive(true);
        hostLoadingPanel.SetActive(false);
        hostButton.interactable = true;

        jtsTitlePanel.SetActive(true);
        jtsLoadingPanel.SetActive(false);
        joinTestServerButton.interactable = true;
    }

    public void OnConnectButtonPressed()
    {
        usernameField.interactable = false;
        ipField.interactable = false;
        connectButton.interactable = false;
        hostButton.interactable = false;
        joinTestServerButton.interactable = false;

        PlayerPrefs.SetString("Username", usernameField.text);
        PlayerPrefs.SetString("LastRoomCode", ipField.text);
        PlayerPrefs.Save();

        //start swirler
        connectTitlePanel.SetActive(false);
        connectLoadingPanel.SetActive(true);

        NetworkManager.Instance.Connect(ipField.text);
    }

    public void OnHostButtonPressed()
    {
        usernameField.interactable = false;
        ipField.interactable = false;
        connectButton.interactable = false;
        hostButton.interactable = false;
        joinTestServerButton.interactable = false;

        PlayerPrefs.SetString("Username", usernameField.text);
        PlayerPrefs.SetString("LastRoomCode", ipField.text);
        PlayerPrefs.Save();

        //start swirler
        hostTitlePanel.SetActive(false);
        hostLoadingPanel.SetActive(true);

        NetworkManager.Instance.Host(ipField.text);
    }

    public void OnJoinTestServerButtonPressed()
    {
        usernameField.interactable = false;
        ipField.interactable = false;
        connectButton.interactable = false;
        hostButton.interactable = false;
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

    public bool GetUseIP()
    {
        return useIP;
    }

    public void ToggleUseIP()
    {
        useIP = !useIP;

        useIPIconImage.sprite = useIP ? activeIcon : inactiveIcon;
        ipFieldPlaceholderText.text = useIP ? "IP Address..." : "Room Code...";
    }
}
