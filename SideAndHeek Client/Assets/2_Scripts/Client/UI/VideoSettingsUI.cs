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
    private int activeResolutionIndex;

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
        activeResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            resolutionList.Add(resolutions[i].width + " x " + resolutions[i].height);

            if (resolutions[i].width == resWidth && resolutions[i].height == resHeight)
            {
                activeResolutionIndex = i;
            }
        }

        bool fullscreen = IntToBool(PlayerPrefs.GetInt("Fullscreen", 1));
        fullscreenToggle.isOn = fullscreen;
        Screen.fullScreen = fullscreen;

        resolutionDropdown.AddOptions(resolutionList);
        resolutionDropdown.value = activeResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        Screen.SetResolution(resWidth, resHeight, Screen.fullScreen);

        int qualityLevel = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        qualityDropdown.value = qualityLevel;
        qualityDropdown.RefreshShownValue();
        QualitySettings.SetQualityLevel(qualityLevel);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("ResWidth", resolutions[activeResolutionIndex].width);
        PlayerPrefs.SetInt("ResHeight", resolutions[activeResolutionIndex].height);
        PlayerPrefs.SetInt("Fullscreen", BoolToInt(fullscreenToggle.isOn));
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
        activeResolutionIndex = resolutionIndex;
        Screen.SetResolution(resolutions[activeResolutionIndex].width, resolutions[activeResolutionIndex].height, Screen.fullScreen);
    }
}
