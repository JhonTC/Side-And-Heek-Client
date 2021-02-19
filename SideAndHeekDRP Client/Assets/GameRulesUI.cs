using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameRulesUI : MonoBehaviour
{
    public GameRules activeGameRules;

    private int gameLength;
    private int numberOfHunters; 
    private CatchType catchType;
    private int hidingTime;
    private SpeedBoostType speedBoostType;
    private float speedMultiplier;

    public void Start()
    {
        activeGameRules = ScriptableObject.CreateInstance<GameRules>();
        gameLength = activeGameRules.gameLength;
        numberOfHunters = activeGameRules.numberOfHunters;
        catchType = activeGameRules.catchType;
        hidingTime = activeGameRules.hidingTime;
        speedBoostType = activeGameRules.speedBoostType;
        speedMultiplier = activeGameRules.speedMultiplier;
    }

    public void OnSaveButtonPressed()
    {
        GameManager.instance.gameRules = activeGameRules;
        ClientSend.GameRulesChanged(activeGameRules);
        gameObject.SetActive(false);
    }

    public void OnGameLengthChanged(Slider slider)
    {
        gameLength = Mathf.RoundToInt(slider.value);
        activeGameRules.gameLength = gameLength;
    }

    public void OnNumberOfHuntersChanged(Slider slider)
    {
        numberOfHunters = Mathf.RoundToInt(slider.value);
        activeGameRules.numberOfHunters = numberOfHunters;
    }

    public void OnCatchTypeChanged(TMP_Dropdown dropdown)
    {
        catchType = (CatchType)dropdown.value;
        activeGameRules.catchType = catchType;
    }

    public void OnHidingTimeChanged(Slider slider)
    {
        hidingTime = Mathf.RoundToInt(slider.value);
        activeGameRules.hidingTime = hidingTime;
    }

    public void OnSpeedBoostTypeChanged(TMP_Dropdown dropdown)
    {
        speedBoostType = (SpeedBoostType)dropdown.value;
        activeGameRules.speedBoostType = speedBoostType;
    }

    public void OnSpeedMultiplierChanged(Slider slider)
    {
        speedMultiplier = slider.value;
        activeGameRules.speedMultiplier = speedMultiplier;
    }
}
