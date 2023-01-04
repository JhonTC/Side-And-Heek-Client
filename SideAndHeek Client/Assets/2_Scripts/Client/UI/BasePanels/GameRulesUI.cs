using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class LocalGameRule
{
    public object value = null;
    public BaseTextSetter textSetter;

    public LocalGameRule(BaseTextSetter _textSetter)
    {
        textSetter = _textSetter;
    }

    public void UpdateUI<T>(T newValue)
    {
        value = newValue;
        textSetter.ChangeValue(newValue);
    }
}

public class GameRulesUI : TabView
{
    public Transform settingsParent;
    public GameType lastGameType;
    private BaseTextSetter gameTypeTextSetter;

    public GameRules ActiveGameRules => GameManager.instance.gameRules;
    public GameType ActiveGameType => GameManager.instance.gameType;

    public Dictionary<int, LocalGameRule> localGameRules = new Dictionary<int, LocalGameRule>();

    public void Init()
    {
        OnEnumValueChanged += ValueChanged;
        OnFloatValueChanged += ValueChanged;
        OnBoolValueChanged += ValueChanged;

        gameTypeTextSetter = UIUtils.CreateUIForEnum<GameType>(-1, "Game Mode", (int)ActiveGameType, ChangeGameType, settingsParent, true);

        ActiveGameRules.SetupUI(settingsParent, this);
    }

    public void OnGameTypeChangedRemotely(bool resetLocalGameRules = true)
    {
        if (LobbyManager.localPlayer == null || (LobbyManager.localPlayer != null && !LobbyManager.localPlayer.isHost)) //update gametype of other clients but not host, as they chnaged the setting
        {
            gameTypeTextSetter.ChangeValue(ActiveGameType);
        }

        if (resetLocalGameRules)
        {
            ResetLocalGameRulesUI();
        }
    }

    public void ResetLocalGameRulesUI()
    {
        for (int i = settingsParent.childCount - 1; i >= 0; i--)
        {
            GameObject textSetterChild = settingsParent.GetChild(i).gameObject;
            if (textSetterChild != gameTypeTextSetter.gameObject)
            {
                Destroy(textSetterChild);
            }
        }
        localGameRules.Clear();

        ActiveGameRules.SetupUI(settingsParent, this);

        UpdatePanelHost();
    }

    public void ChangeGameType(int id, int newGameType, BaseTextSetter setter)
    {
        GameManager.instance.gameType = (GameType)newGameType;
        GameManager.instance.gameMode = GameMode.CreateGameModeFromType(ActiveGameType);
        GameManager.instance.gameRules = GameRules.CreateGameRulesFromType(ActiveGameType);

        ResetLocalGameRulesUI();

        print($"Change GameMode to {ActiveGameType}");
    }

    public override void EnablePanel()
    {
        base.EnablePanel();

        UpdatePanelHost();
    }

    public void OnGameRulesUpdatedRemotely(GameRules gameRules)
    {
        if (lastGameType == ActiveGameType)
        {
            gameRules.UpdateUI(ref localGameRules);
        } else
        {
            OnGameTypeChangedRemotely();

            lastGameType = ActiveGameType;
        }
    }

    public void OnSaveButtonPressed()
    {
        ActiveGameRules.UpdateValues(localGameRules);
        ClientSend.GameRulesChanged(ActiveGameRules);
        UIManager.instance.OnBackButtonPressed();
    }

    public void UpdatePanelHost()
    {
        bool isLocalPlayerHost = (LobbyManager.localPlayer != null) ? LobbyManager.localPlayer.isHost : false;

        gameTypeTextSetter.OnDisplay(isLocalPlayerHost);

        foreach (LocalGameRule localGameRule in localGameRules.Values)
        {
            localGameRule.textSetter.OnDisplay(isLocalPlayerHost);
        }
    }

    public void ValueChanged<T>(int id, T value, BaseTextSetter setter)
    {
        //print($"Value Changed at {id}: {setter.title.text} to {value}");
        if (!localGameRules.ContainsKey(id))
        {
            localGameRules.Add(id, new LocalGameRule(setter));
        }

        localGameRules[id].value = value;
    }
}
