using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider gameplayVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider ambientVolumeSlider;

    public void Init()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 50);
        masterVolumeSlider.value = masterVolume;
        OnMasterVolumeChanged(masterVolume);

        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 50);
        musicVolumeSlider.value = musicVolume;
        OnMusicVolumeChanged(musicVolume);

        float ambientVolume = PlayerPrefs.GetFloat("AmbientVolume", 50);
        ambientVolumeSlider.value = ambientVolume;
        OnAmbientVolumeChanged(ambientVolume);

        float gameplayVolume = PlayerPrefs.GetFloat("GameplayVolume", 50);
        gameplayVolumeSlider.value = gameplayVolume;
        OnGameplayVolumeChanged(gameplayVolume);
    }

    public void OnMasterVolumeChanged(float value)
    {
        float clampedValue = value;
        if (clampedValue < 0.0001f)
        {
            clampedValue = 0.0001f;
        }

        clampedValue = Mathf.Log10(clampedValue / 50) * 50;

        mixer.SetFloat("MasterVolume", clampedValue);
    }
    public void OnMusicVolumeChanged(float value)
    {
        float clampedValue = value;
        if (clampedValue < 0.0001f)
        {
            clampedValue = 0.0001f;
        }

        clampedValue = Mathf.Log10(clampedValue / 50) * 50;

        mixer.SetFloat("MusicVolume", clampedValue);
    }
    public void OnAmbientVolumeChanged(float value)
    {
        float clampedValue = value;
        if (clampedValue < 0.0001f)
        {
            clampedValue = 0.0001f;
        }

        clampedValue = Mathf.Log10(clampedValue / 50) * 50;

        mixer.SetFloat("AmbientVolume", clampedValue);
    }
    public void OnGameplayVolumeChanged(float value)
    {
        float clampedValue = value;
        if (clampedValue < 0.0001f)
        {
            clampedValue = 0.0001f;
        }

        clampedValue = Mathf.Log10(clampedValue / 50) * 50;

        mixer.SetFloat("GameplayVolume", clampedValue);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("AmbientVolume", ambientVolumeSlider.value);
        PlayerPrefs.SetFloat("GameplayVolume", gameplayVolumeSlider.value);
    }
}
