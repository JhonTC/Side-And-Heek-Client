using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Linq;
using System;

public class VideoSettingsUI : MonoBehaviour
{
    private Resolution[] resolutions;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;

    public void Init()
    {
        resolutions = Screen.resolutions.Where(res => res.refreshRate == Screen.currentResolution.refreshRate).ToArray();
        resolutionDropdown.ClearOptions();

        int resWidth = PlayerPrefs.GetInt("ResWidth", Screen.currentResolution.width);
        int resHeight = PlayerPrefs.GetInt("ResHeight", Screen.currentResolution.height);
        List<string> resolutionList = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            resolutionList.Add(resolutions[i].width + " x " + resolutions[i].height);

            if (resolutions[i].width == resWidth && resolutions[i].height == resHeight)
            {
                currentResolutionIndex = i;
            }
        }

        bool fullscreen = IntToBool(PlayerPrefs.GetInt("Fullscreen", 1));
        fullscreenToggle.isOn = fullscreen;
        Screen.fullScreen = fullscreen;

        resolutionDropdown.AddOptions(resolutionList);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        Screen.SetResolution(resWidth, resHeight, Screen.fullScreen);

        qualityDropdown.value = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("ResWidth", Screen.currentResolution.width);
        PlayerPrefs.SetInt("ResHeight", Screen.currentResolution.height);
        PlayerPrefs.SetInt("Fullscreen", BoolToInt(Screen.fullScreen));
        PlayerPrefs.SetInt("QualityLevel", QualitySettings.GetQualityLevel());
    }

    private int BoolToInt(bool value)
    {
        return value ? 1 : 0;
    }

    private bool IntToBool(int value)
    {
        return (Mathf.Clamp01(value) == 0) ? false : true;
    }

    public void OnQualityChanged(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void OnFullscreenChanged(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void OnResolutionChanged(int resolutionIndex)
    {
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, Screen.fullScreen);
    }
}
