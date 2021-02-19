using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabView : MonoBehaviour
{
    [SerializeField] private GameObject[] tabs;
    [SerializeField] private GameObject[] tabContents;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    private int currentTabIndex = 0;

    private void Start()
    {
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
            if (i != currentTabIndex)
            {
                tabs[i].SetActive(false);
                tabContents[i].SetActive(false);
            }
            else
            {
                tabs[i].SetActive(true);
                tabContents[i].SetActive(true);
            }
        }

        CheckButtonsInteractable();
    }

    public void OnNextPressed()
    {
        if (currentTabIndex < tabs.Length - 1)
        {
            currentTabIndex++;
            OnTabPressed(currentTabIndex);
        } 
    }

    public void OnPreviousPressed()
    {
        if (currentTabIndex > 0)
        {
            currentTabIndex--;
            OnTabPressed(currentTabIndex);
        }
    }

    private void CheckButtonsInteractable()
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
    }
}
