using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static InputHandler instance;

    public PlayerInput playerInput;

    private Player localPlayer => Player.LocalPlayer;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        } else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SwitchInput(string mapName)
    {
        playerInput.SwitchCurrentActionMap(mapName);
        print(playerInput.currentActionMap.name);
    }

    public bool CanMove()
    {
        if (localPlayer != null)
        {
            if (localPlayer.playerType != PlayerType.Spectator)
            {
                return true;
            }
        }

        return false;
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        if (CanMove())
        {
            localPlayer.playerMotor.OnMove(value);
        }
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        if (CanMove())
        {
            localPlayer.playerMotor.OnJump(value);
        }
    }

    public void OnFlop(InputAction.CallbackContext value)
    {
        if (CanMove())
        {
            localPlayer.playerMotor.OnFlop(value);
        }
    }

    public void OnAim(InputAction.CallbackContext value)
    {
        if (localPlayer != null)
        {
            localPlayer.OnAim(value);
        }
    }

    public void OnSelect(InputAction.CallbackContext value)
    {
        if (localPlayer != null)
        {
            localPlayer.OnSelect(value);
        }
    }

    public void OnUse(InputAction.CallbackContext value)
    {
        if (value.phase == InputActionPhase.Started)
        {
            localPlayer.UsePickup();
        }
    }

    public void OnReady(InputAction.CallbackContext value)
    {
        if (localPlayer != null)
        {
            if (!GameManager.instance.gameStarted)
            {
                localPlayer.SetPlayerReady(!localPlayer.isReady);

                if (NetworkManager.NetworkType == NetworkType.Client)
                {
                    ClientSend.PlayerReady(localPlayer.isReady);
                }
                else if (NetworkManager.NetworkType == NetworkType.ClientServer)
                {
                    ServerSend.PlayerReadyToggled(localPlayer.Id, localPlayer.isReady);
                }
            }
        }
    }

    public void ToggleCustomisation(InputAction.CallbackContext value)
    {
        if (!GameManager.instance.gameStarted)
        {
            if (value.phase == InputActionPhase.Started)
            {
                UIManager.instance.TogglePanel(UIPanelType.Customisation);
            }
        }
    }

    public void ToggleGameRules(InputAction.CallbackContext value)
    {
        if (!GameManager.instance.gameStarted)
        {
            if (value.phase == InputActionPhase.Started)
            {
                UIManager.instance.TogglePanel(UIPanelType.Game_Rules);
            }
        }
    }

    public void TogglePause(InputAction.CallbackContext value)
    {
        if (value.phase == InputActionPhase.Started)
        {
            UIManager.instance.TogglePanel(UIPanelType.Pause);
        }
    }
}
