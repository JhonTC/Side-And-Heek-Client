using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsUI : TabView
{
    [SerializeField] private VideoSettingsUI videoUI;
    [SerializeField] private AudioSettingsUI audioUI;
    [SerializeField] private ControlsSettingsUI controlsUI;

    public void Init()
    {
        videoUI.Init();
        audioUI.Init();
    }

    public void OnQuit()
    {
        videoUI.SaveSettings();
        audioUI.SaveSettings();
    }

    public void OnSaveButtonPressed()
    {
        videoUI.SaveSettings();

        UIManager.instance.OnBackButtonPressed();
    }
}
