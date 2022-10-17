using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameRulesUI : MonoBehaviour
{
    [SerializeField] private DropdownTextSetter mapDropdown;
    [SerializeField] private SliderTextSetter gameLengthSlider;
    [SerializeField] private SliderTextSetter hiderRespawnDelaySlider;
    [SerializeField] private SliderTextSetter numberOfHuntersSlider;
    [SerializeField] private DropdownTextSetter catchTypeDropdown;
    [SerializeField] private SliderTextSetter hidingTimeSlider;
    [SerializeField] private DropdownTextSetter speedBoostTypeDropdown;
    [SerializeField] private SliderTextSetter speedMultiplierSlider;
    [SerializeField] private DropdownTextSetter fallRespawnTypeDropdown;
    [SerializeField] private DropdownTextSetter fallRespawnLocationDropdown;
    [SerializeField] private Toggle continuousFlopToggle;

    public LocalGameRules localGameRules;

    [SerializeField] private GameObject saveButton;

    [System.Serializable]
    public class LocalGameRules
    {
        public Map map;
        public int gameLength;
        public int hiderRespawnDelay;
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
            map = gameRules.map;
            gameLength = gameRules.gameLength;
            hiderRespawnDelay = gameRules.hiderRespawnDelay;
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
            gameRules.map = map;
            gameRules.gameLength = gameLength;
            gameRules.hiderRespawnDelay = hiderRespawnDelay;
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
        mapDropdown.SetValue((int)localGameRules.map);
        gameLengthSlider.SetValue(localGameRules.gameLength);
        hiderRespawnDelaySlider.SetValue(localGameRules.hiderRespawnDelay);
        numberOfHuntersSlider.SetValue(localGameRules.numberOfHunters);
        catchTypeDropdown.SetValue((int)localGameRules.catchType);
        hidingTimeSlider.SetValue(localGameRules.hidingTime);
        speedBoostTypeDropdown.SetValue((int)localGameRules.speedBoostType);
        speedMultiplierSlider.SetValue(localGameRules.speedMultiplier);
        fallRespawnTypeDropdown.SetValue((int)localGameRules.fallRespawnType);
        fallRespawnLocationDropdown.SetValue((int)localGameRules.fallRespawnLocation);
        continuousFlopToggle.isOn = localGameRules.continuousFlop;
    }

    public void OnSaveButtonPressed()
    {
        GameManager.instance.gameRules = localGameRules.AsGameRules();

        if (GameManager.instance.gameType == GameType.Multiplayer)
        {
            ClientSend.GameRulesChanged(GameManager.instance.gameRules);
        }


        UIManager.instance.DisableAllPanels();
    }
    public void OnMapChanged(int index)
    {
        localGameRules.map = (Map)index;
    }

    public void OnGameLengthChanged(float value)
    {
        localGameRules.gameLength = Mathf.RoundToInt(value);
    }

    public void OnHiderRespawnDelayChanged(float value)
    {
        localGameRules.hiderRespawnDelay = Mathf.RoundToInt(value);
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
