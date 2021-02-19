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


    public void OnMasterVolumeChanged(float value)
    {
        float clampedValue = value;
        if (clampedValue < 0.0001f)
        {
            clampedValue = 0.0001f;
        }

        mixer.SetFloat("MasterVolume", Mathf.Log10(clampedValue / 50) * 50);
    }
    public void OnMusicVolumeChanged(float value)
    {
        float clampedValue = value;
        if (clampedValue < 0.0001f)
        {
            clampedValue = 0.0001f;
        }

        mixer.SetFloat("MusicVolume", Mathf.Log10(clampedValue / 50) * 50);
    }
    public void OnAmbientVolumeChanged(float value)
    {
        float clampedValue = value;
        if (clampedValue < 0.0001f)
        {
            clampedValue = 0.0001f;
        }

        mixer.SetFloat("AmbientVolume", Mathf.Log10(clampedValue / 50) * 50);
    }
    public void OnGameplayVolumeChanged(float value)
    {
        float clampedValue = value;
        if (clampedValue < 0.0001f)
        {
            clampedValue = 0.0001f;
        }

        mixer.SetFloat("GameplayVolume", Mathf.Log10(clampedValue / 50) * 50);
    }
}
