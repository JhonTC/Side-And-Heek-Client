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
    [SerializeField] private TMP_Dropdown fallRespawnTypeDropdown;
    [SerializeField] private TMP_Dropdown fallRespawnLocationDropdown;
    [SerializeField] private Toggle continuousFlopToggle;

    public LocalGameRules localGameRules;

    [SerializeField] private GameObject saveButton;

    [System.Serializable]
    public class LocalGameRules
    {
        public int gameLength;
        public int numberOfHunters;
        public CatchType catchType;
        public int hidingTime;
        public SpeedBoostType speedBoostType;
        public float speedMultiplier; 
        public HiderFallRespawnType fallRespawnType;
        public FallRespawnLocation fallRespawnLocation;
        public bool continuousFlop;

        public LocalGameRules(GameRules gameRules)
        {
            gameLength = gameRules.gameLength;
            numberOfHunters = gameRules.numberOfHunters;
            catchType = gameRules.catchType;
            hidingTime = gameRules.hidingTime;
            speedBoostType = gameRules.speedBoostType;
            speedMultiplier = gameRules.speedMultiplier;
            fallRespawnType = gameRules.fallRespawnType;
            fallRespawnLocation = gameRules.fallRespawnLocation;
            continuousFlop = gameRules.continuousFlop;
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
            gameRules.fallRespawnType = fallRespawnType;
            gameRules.fallRespawnLocation = fallRespawnLocation;
            gameRules.continuousFlop = continuousFlop;

            return gameRules;
        }
    }

    private void Start()
    {
        SetGameRules(GameManager.instance.gameRules);
        //UpdateUIValues();

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

        UpdateUIValues();
    }

    private void UpdateUIValues()
    {
        gameLengthSlider.value = localGameRules.gameLength;
        numberOfHuntersSlider.value = localGameRules.numberOfHunters;
        catchTypeDropdown.value = (int)localGameRules.catchType;
        hidingTimeSlider.value = localGameRules.hidingTime;
        speedBoostTypeDropdown.value = (int)localGameRules.speedBoostType;
        speedMultiplierSlider.value = localGameRules.speedMultiplier;
        fallRespawnTypeDropdown.value = (int)localGameRules.fallRespawnType;
        fallRespawnLocationDropdown.value = (int)localGameRules.fallRespawnLocation;
        continuousFlopToggle.isOn = localGameRules.continuousFlop;
    }

    public void OnSaveButtonPressed()
    {
        GameManager.instance.gameRules = localGameRules.AsGameRules();

        if (GameManager.instance.gameType == GameType.Multiplayer)
        {
            ClientSend.GameRulesChanged(GameManager.instance.gameRules);
        }

        gameObject.SetActive(false);
    }

    public void OnGameLengthChanged(float value)
    {
        localGameRules.gameLength = Mathf.RoundToInt(value);
        Debug.Log(value);
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

    public void OnFallRespawnTypeChanged(int index)
    {
        localGameRules.fallRespawnType = (HiderFallRespawnType)index;
    }

    public void OnFallRespawnLocationChanged(int index)
    {
        localGameRules.fallRespawnLocation = (FallRespawnLocation)index;
    }

    public void OnContinuousFlopChanged(bool value)
    {
        localGameRules.continuousFlop = value;
    }
}
