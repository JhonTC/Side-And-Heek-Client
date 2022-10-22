using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTypeView : MonoBehaviour
{
    [SerializeField] private Image icon;

    public void SetPlayerTypeViewColour(Color colour)
    {
        icon.color = colour;
    }
}
