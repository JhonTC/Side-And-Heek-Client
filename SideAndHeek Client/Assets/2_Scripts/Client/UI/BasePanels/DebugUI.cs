using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugUI : TabView
{
    [SerializeField] private Transform mapButtonParent;
    [SerializeField] private ButtonTextSetter buttonPrefab;
    [SerializeField] private TMP_Text activeSceneName;

    public void Init()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string sceneName = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));

            if (sceneName != "Lobby")
            {
                ButtonTextSetter newButton = Instantiate(buttonPrefab, mapButtonParent);
                newButton.SetTitle(sceneName);

                newButton.button.onClick.AddListener(() => { OnButtonClicked(sceneName); });
            }
        }
    }

    private void OnButtonClicked(string sceneName)
    {
        if (NetworkManager.NetworkType == NetworkType.Client)
        {
            ClientSend.RequestSceneChange(sceneName, true);
        }
        else if (NetworkManager.NetworkType == NetworkType.ClientServer)
        {
            GameManager.instance.ChangeScene(sceneName, true);
        }

        UIManager.instance.CloseAllPanels();
    }

    public void SetActiveSceneName(string sceneName)
    {
        activeSceneName.text = sceneName;
    }

    public override void EnablePanel()
    {
        base.EnablePanel();
    }
}
