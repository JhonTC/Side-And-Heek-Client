using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static InputHandler instance;

    public PlayerInput playerInput;

    private Player localPlayer { get { return LobbyManager.localPlayer; } }

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

    public void OnMove(InputAction.CallbackContext value)
    {
        if (localPlayer != null)
        {
            localPlayer.playerMotor.OnMove(value);
        }
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        if (localPlayer != null)
        {
            localPlayer.playerMotor.OnJump(value);
        }
    }

    public void OnFlop(InputAction.CallbackContext value)
    {
        if (localPlayer != null)
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

    public void OnUse(InputAction.CallbackContext value)
    {
        if (localPlayer != null)
        {
            localPlayer.playerMotor.OnUse(value);
        }
    }

    public void OnReady(InputAction.CallbackContext value)
    {
        if (localPlayer != null)
        {
            localPlayer.playerMotor.OnReady(value);
        }
    }

    public void ToggleCustomisation(InputAction.CallbackContext value)
    {
        if (localPlayer != null)
        {
            localPlayer.playerMotor.ToggleCustomisation(value);
        }
    }

    public void ToggleGameRules(InputAction.CallbackContext value)
    {
        if (localPlayer != null)
        {
            localPlayer.playerMotor.ToggleGameRules(value);
        }
    }

    public void TogglePause(InputAction.CallbackContext value)
    {
        if (localPlayer != null)
        {
            localPlayer.playerMotor.TogglePause(value);
        }
    }
}
