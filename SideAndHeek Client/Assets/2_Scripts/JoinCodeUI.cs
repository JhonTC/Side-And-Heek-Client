using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JoinCodeUI : MonoBehaviour
{
    public static JoinCodeUI Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(Instance);
        }
    }

    public static string InputJoinCode => Instance.joinCodeField.text;

    public TMP_Text joinCodeText;
    public TMP_InputField joinCodeField;
    public Button joinButton;

    public void SetJoinCodeText(string  joinCodeText)
    {
        Instance.joinCodeText.text = joinCodeText;
    }

    public void InitServer()
    {
        joinCodeText.gameObject.SetActive(true);
        joinCodeField.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
    }

    public void InitClient()
    {
        joinCodeText.gameObject.SetActive(false);
        joinCodeField.gameObject.SetActive(true);
        joinButton.gameObject.SetActive(true);
    }
}
