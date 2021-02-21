using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameRulesUI : MonoBehaviour
{
    [SerializeField] private Slider gameLengthSlider;
    [SerializeField] private Slider numberOfHuntersSlider;
    [SerializeField] private TMP_Dropdown catchTypeDropdown;
    [SerializeField] private Slider hidingTimeSlider;
    [SerializeField] private TMP_Dropdown speedBoostTypeDropdown;
    [SerializeField] private Slider speedMultiplierSlider;

    private LocalGameRules localGameRules;

    [SerializeField] private GameObject saveButton;

    public class LocalGameRules
    {
        public int gameLength;
        public int numberOfHunters;
        public CatchType catchType;
        public int hidingTime;
        public SpeedBoostType speedBoostType;
        public float speedMultiplier;

        public LocalGameRules(GameRules gameRules)
        {
            gameLength = gameRules.gameLength;
            numberOfHunters = gameRules.numberOfHunters;
            catchType = gameRules.catchType;
            hidingTime = gameRules.hidingTime;
            speedBoostType = gameRules.speedBoostType;
            speedMultiplier = gameRules.speedMultiplier;
        }

        public GameRules AsGameRules()
        {
            GameRules gameRules = ScriptableObject.CreateInstance<GameRules>();
            gameRules.gameLength = gameLength;
            gameRules.numberOfHunters = numberOfHunters;
            gameRules.catchType = catchType;
            gameRules.hidingTime = hidingTime;
            gameRules.speedBoostType = speedBoostType;
            gameRules.speedMultiplier = speedMultiplier;

            return gameRules;
        }
    }

    private void Start()
    {
        SetGameRules(GameManager.instance.gameRules);

        if (LobbyManager.instance.isHost)
        {
            saveButton.SetActive(true);
        }
        else
        {
            saveButton.SetActive(false);
        }
    }

    public void SetGameRules(GameRules gameRules)
    {
        localGameRules = new LocalGameRules(gameRules);

        gameLengthSlider.value = localGameRules.gameLength;
        numberOfHuntersSlider.value = localGameRules.numberOfHunters;
        catchTypeDropdown.value = (int)localGameRules.catchType;
        hidingTimeSlider.value = localGameRules.hidingTime;
        speedBoostTypeDropdown.value = (int)localGameRules.speedBoostType;
        speedMultiplierSlider.value = localGameRules.speedMultiplier;
    }

    public void OnSaveButtonPressed()
    {
        GameManager.instance.gameRules = localGameRules.AsGameRules();
        ClientSend.GameRulesChanged(GameManager.instance.gameRules);
        gameObject.SetActive(false);
    }

    public void OnGameLengthChanged(float value)
    {
        localGameRules.gameLength = Mathf.RoundToInt(value);
    }

    public void OnNumberOfHuntersChanged(float value)
    {
        localGameRules.numberOfHunters = Mathf.RoundToInt(value);
    }

    public void OnCatchTypeChanged(int index)
    {
        localGameRules.catchType = (CatchType)index;
    }

    public void OnHidingTimeChanged(float value)
    {
        localGameRules.hidingTime = Mathf.RoundToInt(value);
    }

    public void OnSpeedBoostTypeChanged(int index)
    {
        localGameRules.speedBoostType = (SpeedBoostType)index;
    }

    public void OnSpeedMultiplierChanged(float value)
    {
        localGameRules.speedMultiplier = value;
    }
}
