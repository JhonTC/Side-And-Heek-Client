using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TabView : UIPanel
{
    [SerializeField] protected Tab[] tabs;

    //[SerializeField] private Button nextButton;
    //[SerializeField] private Button previousButton;

    protected int currentTabIndex = 0;

    public override void EnablePanel()
    {
        base.EnablePanel();
        OnTabPressed(currentTabIndex);
    }

    public void OnTabPressed(int tabIndex)
    {
        currentTabIndex = tabIndex;

        if (currentTabIndex >= tabs.Length || currentTabIndex < 0)
        {
            throw new System.Exception($"ERROR: Tab for tabIndex({currentTabIndex}) does not exist.");
        }

        for (int i = 0; i < tabs.Length; i++)
        {
            if (i != tabIndex)
            {
                tabs[i].OnTabToggled(false);
            }
        }

        tabs[tabIndex].OnTabToggled(true);
        //CheckButtonsInteractable();
    }

    public void OnNextPressed(InputAction.CallbackContext value)
    {
        if (value.phase == InputActionPhase.Started && gameObject.activeSelf)
        {
            if (currentTabIndex < tabs.Length - 1)
            {
                currentTabIndex++;
                OnTabPressed(currentTabIndex);
            }
        }
    }

    public void OnPreviousPressed(InputAction.CallbackContext value)
    {
        if (value.phase == InputActionPhase.Started && gameObject.activeSelf)
        {
            if (currentTabIndex > 0)
            {
                currentTabIndex--;
                OnTabPressed(currentTabIndex);
            }
        }
    }

    /*private void CheckButtonsInteractable()
    {
        if (currentTabIndex == tabs.Length - 1)
        {
            nextButton.interactable = false;
        }
        else
        {
            nextButton.interactable = true;
        }

        if (currentTabIndex == 0)
        {
            previousButton.interactable = false;
        }
        else
        {
            previousButton.interactable = true;
        }
    }*/
}
