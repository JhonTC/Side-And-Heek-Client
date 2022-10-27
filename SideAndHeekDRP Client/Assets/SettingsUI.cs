using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsUI : MonoBehaviour
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
}
