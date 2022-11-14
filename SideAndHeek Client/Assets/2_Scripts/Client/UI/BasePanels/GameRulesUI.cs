using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameRulesUI : TabView
{
    [SerializeField] private SliderTextSetter gameLengthSlider;
    [SerializeField] private SliderTextSetter numberOfHuntersSlider;
    [SerializeField] private DropdownTextSetter catchTypeDropdown;
    [SerializeField] private SliderTextSetter hidingTimeSlider;
    [SerializeField] private DropdownTextSetter speedBoostTypeDropdown;
    [SerializeField] private SliderTextSetter speedMultiplierSlider;
    [SerializeField] private DropdownTextSetter fallRespawnTypeDropdown;
    [SerializeField] private DropdownTextSetter fallRespawnLocationDropdown;
    [SerializeField] private ToggleTextSetter continuousFlopToggle;

    public LocalGameRules localGameRules;

    [SerializeField] private GameObject saveButton;

    public bool hostOnly = true;

    public override void EnablePanel()
    {
        UpdatePanelHost();
        base.EnablePanel();
    }

    private void Start()
    {
        SetGameRules(GameManager.instance.gameRules);
    }

    public void UpdatePanelHost()
    {
        bool isLocalPlayerHost = LobbyManager.localPlayer.isHost;

        saveButton.SetActive(!hostOnly || isLocalPlayerHost);

        gameLengthSlider.OnDisplay(isLocalPlayerHost);
        numberOfHuntersSlider.OnDisplay(isLocalPlayerHost);
        catchTypeDropdown.OnDisplay(isLocalPlayerHost);
        hidingTimeSlider.OnDisplay(isLocalPlayerHost);
        speedBoostTypeDropdown.OnDisplay(isLocalPlayerHost);
        speedMultiplierSlider.OnDisplay(isLocalPlayerHost);
        fallRespawnTypeDropdown.OnDisplay(isLocalPlayerHost);
        fallRespawnLocationDropdown.OnDisplay(isLocalPlayerHost);
        continuousFlopToggle.OnDisplay(isLocalPlayerHost);
    }

    public void SetGameRules(GameRules gameRules)
    {
        localGameRules = new LocalGameRules(gameRules);

        UpdateUIValues();
    }

    private void UpdateUIValues()
    {
        gameLengthSlider.ChangeValue(localGameRules.gameLength);
        numberOfHuntersSlider.ChangeValue(localGameRules.numberOfHunters);
        catchTypeDropdown.ChangeValue((int)localGameRules.catchType);
        hidingTimeSlider.ChangeValue(localGameRules.hidingTime);
        speedBoostTypeDropdown.ChangeValue((int)localGameRules.speedBoostType);
        speedMultiplierSlider.ChangeValue(localGameRules.speedMultiplier);
        fallRespawnTypeDropdown.ChangeValue((int)localGameRules.fallRespawnType);
        fallRespawnLocationDropdown.ChangeValue((int)localGameRules.fallRespawnLocation);
        continuousFlopToggle.ChangeValue(localGameRules.continuousFlop);
    }

    public void OnSaveButtonPressed()
    {
        GameManager.instance.gameRules = localGameRules.AsGameRules();

        if (GameManager.instance.gameType == GameType.Multiplayer)
        {
            ClientSend.GameRulesChanged(GameManager.instance.gameRules);
        }

        UIManager.instance.OnBackButtonPressed();
    }

    #region OnValueChanged

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

    #endregion

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
}
