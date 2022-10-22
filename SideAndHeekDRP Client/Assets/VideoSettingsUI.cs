using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Linq;

public class VideoSettingsUI : MonoBehaviour
{
    private Resolution[] resolutions;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;

    private void Start()
    {
        resolutions = Screen.resolutions.Where(res => res.refreshRate == Screen.currentResolution.refreshRate).ToArray();
        resolutionDropdown.ClearOptions();

        int resWidth = Screen.currentResolution.width;
        if (PlayerPrefs.HasKey("ResWidth")) {
            resWidth = PlayerPrefs.GetInt("ResWidth");
        }
        int resHeight = Screen.currentResolution.height;
        if (PlayerPrefs.HasKey("ResHeight")) {
            resHeight = PlayerPrefs.GetInt("ResHeight");
        }

        Screen.SetResolution(resWidth, resHeight, Screen.fullScreen);

        List<string> resolutionList = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            resolutionList.Add(resolutions[i].width + " x " + resolutions[i].height);

            if (resolutions[i].width == Screen.currentResolution.width && 
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resolutionList);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = Screen.fullScreen;
        qualityDropdown.value = QualitySettings.GetQualityLevel();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("ResWidth", Screen.currentResolution.width);
        PlayerPrefs.SetInt("ResHeight", Screen.currentResolution.height);
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
