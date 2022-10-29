using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUIAnimationController : MonoBehaviour
{
    [SerializeField] private GameplayUI gameplayUI;

    public void OnClose()
    {
        gameplayUI.OnClose();
    }
}
