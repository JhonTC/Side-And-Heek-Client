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

        relaunchServerButton.gameObject.SetActive(LobbyManager.localPlayer.isHost);
    }

    public void OnRelaunchServerButtonPressed()
    {
        ClientSend.Command("relaunch_server");
    }
}
