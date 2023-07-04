using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : UIPanel
{
    [SerializeField] private Button relaunchServerButton;

    public override void EnablePanel()
    {
        base.EnablePanel();

        relaunchServerButton.gameObject.SetActive(Player.LocalPlayer.isHost);
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
