using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private Animator itemAnimator;
    [SerializeField] private GameObject itemButton;

    private bool hasItem = false;
    [SerializeField] private TMP_Text itemTitle;
    [SerializeField] private TMP_Text itemContent;
    [SerializeField] private Image itemOutline;

    [SerializeField] private UIManager uiManager;

    [HideInInspector] public List<GameplayMouseDetector> mouseDetectors = new List<GameplayMouseDetector>();

    public bool isActive = false;

    private void Start()
    {
        itemButton.SetActive(false);
    }

    public void SetItemDetails(ItemPickup pickup = null)
    {
        if (pickup != null)
        {
            hasItem = true;

            itemTitle.text = pickup.pickupName;
            itemContent.text = pickup.pickupContent;
            itemOutline.color = pickup.pickupLevel.color;

            itemButton.SetActive(true);
        } 
        else
        {
            hasItem = false;

            itemTitle.text = "";
            itemContent.text = "";

            itemButton.SetActive(false);
        }
    }

    public void ToggleItemDisplay(bool value)
    {
        isActive = value;

        if (isActive)
        {
            itemAnimator.SetTrigger("Open");
        } 
        else
        {
            itemAnimator.SetTrigger("Close");
        }
    }

    public void OnPointerEnterItem()
    {
        if (!isActive && hasItem)
        {
            ToggleItemDisplay(true);
        }
    }

    public void OnPointerExitItem()
    {
        if (isActive && hasItem)
        {
            if (!IsMouseDetectorStillActive())
            {
                ToggleItemDisplay(false);
            }
        }
    }

    private bool IsMouseDetectorStillActive()
    {
        foreach (GameplayMouseDetector mouseDetector in mouseDetectors)
        {
            if (mouseDetector.pointerOver == true)
            {
                return true;
            }
        }

        return false;
    }
}
