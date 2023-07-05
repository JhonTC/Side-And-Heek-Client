using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : UIPanel
{
    [SerializeField] private Button relaunchServerButton;
    [SerializeField] private TMP_Text disconnectButtonText;

    private string disconnect = "Disconnect";
    private string closeServer = "Close Server";
    public override void EnablePanel()
    {
        base.EnablePanel();

        relaunchServerButton.gameObject.SetActive(Player.LocalPlayer.isHost);
        disconnectButtonText.text = NetworkManager.NetworkType == NetworkType.Client ? disconnect : closeServer;
    }

    public void OnRelaunchServerButtonPressed()
    {
        if (NetworkManager.NetworkType == NetworkType.Client) //todo: this allows clients can restart a ClientServer!
        {
            ClientSend.Command("relaunch_server");
        }
        else if (NetworkManager.NetworkType == NetworkType.ClientServer)
        {
            Debug.LogError($"Cannot restart when host is ClientServer");
        }
    }
}
