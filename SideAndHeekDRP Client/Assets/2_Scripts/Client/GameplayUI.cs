using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private Animator itemAnimator;
    [SerializeField] private GameObject itemButton;
    [SerializeField] private Image itemProgress;

    private bool hasItem = false;
    [SerializeField] private TMP_Text itemTitle;
    [SerializeField] private TMP_Text itemContent;
    [SerializeField] private Image itemOutline;

    [HideInInspector] public List<GameplayMouseDetector> mouseDetectors = new List<GameplayMouseDetector>();

    public bool isActive = false;
    private Action<PickupSO> onCloseCallback;

    [SerializeField] private PlayerTypeView[] playerTypeViews;

    private void Start()
    {
        //itemButton.SetActive(false);
        itemAnimator.gameObject.SetActive(false);

        for (int i = 0; i < playerTypeViews.Length; i++)
        {
            if (LobbyManager.players.Count > i)
            {
                playerTypeViews[i].gameObject.SetActive(true);
                playerTypeViews[i].SetPlayerTypeViewColour(LobbyManager.players.ElementAt(i).Value.hiderColour);
            }
            else
            {
                playerTypeViews[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdatePlayerTypeViews()
    {
        for (int i = 0; i < playerTypeViews.Length; i++)
        {
            if (LobbyManager.players.Count > i)
            {
                playerTypeViews[i].gameObject.SetActive(true);

                Player player = LobbyManager.players.ElementAt(i).Value;

                playerTypeViews[i].SetPlayerTypeViewColour((player.playerType == PlayerType.Hider || player.playerType == PlayerType.Default) ? player.hiderColour : player.seekerColour);
            }
            else
            {
                playerTypeViews[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetItemDetails(PickupSO pickup = null)
    {
        if (pickup != null)
        {
            hasItem = true;

            itemTitle.text = pickup.pickupName;
            itemContent.text = pickup.pickupContent;
            itemOutline.color = pickup.pickupLevel.color;

            //itemButton.SetActive(true);
            itemAnimator.gameObject.SetActive(true);
        } 
        else
        {
            hasItem = false;

            itemTitle.text = "";
            itemContent.text = "";

            //itemButton.SetActive(false);
            itemAnimator.gameObject.SetActive(false);
        }
    }

    public void ToggleItemDisplay(bool value, Action<PickupSO> callback = null)
    {
        isActive = value;
        onCloseCallback = callback;

        if (isActive)
        {
            //itemAnimator.SetTrigger("Open");
        } 
        else
        {
            //itemAnimator.SetTrigger("Close");
        }

        if (callback != null)
        {
            callback?.Invoke(null);
        }
    }

    public void OnClose()
    {
        if (onCloseCallback != null)
        {
            onCloseCallback?.Invoke(null);
            onCloseCallback = null;
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


    private bool progressCountdownActive = false;
    private float duration;
    private float countdownValue;
    private void FixedUpdate()
    {
        if (progressCountdownActive)
        {
            if (countdownValue > 0)
            {
                itemProgress.fillAmount = countdownValue / duration;
                countdownValue -= Time.fixedDeltaTime;
            } else
            {
                progressCountdownActive = false;
                itemProgress.gameObject.SetActive(false);
            }
        }
    }

    public void StartProgressCountdown(float _duration)
    {
        duration = _duration;
        countdownValue = duration;
        progressCountdownActive = true; 
        itemProgress.gameObject.SetActive(true);
    }
}
