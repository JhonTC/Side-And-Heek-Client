using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTextSetter : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    public Button button;

    public void SetTitle(string titleText)
    {
        title.text = titleText;
    }
}
