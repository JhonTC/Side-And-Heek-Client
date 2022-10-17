using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
            if (LobbyManager.players.ContainsKey(i))
            {
                playerTypeViews[i].gameObject.SetActive(true);
                playerTypeViews[i].SetPlayerTypeViewColour(LobbyManager.players[i].hiderColour);
            }
            else
            {
                playerTypeViews[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdatePlayerTypeViews()
    {
        List<Color> sortedPlayerColours = new List<Color>();
        foreach (PlayerManager player in LobbyManager.players.Values)
        {
            if (player.playerType == PlayerType.Hider)
            {
                sortedPlayerColours.Insert(0, player.hiderColour);
            } else if (player.playerType == PlayerType.Hunter)
            {
                sortedPlayerColours.Add(player.seekerColour);
            } else
            {
                sortedPlayerColours.Add(Color.grey);
            }
        }

        for (int i = 0; i < playerTypeViews.Length; i++)
        {
            if (i < sortedPlayerColours.Count)
            {
                playerTypeViews[i].gameObject.SetActive(true);

                playerTypeViews[i].SetPlayerTypeViewColour(sortedPlayerColours[i]);
            } else
            {
                playerTypeViews[i].gameObject.SetActive(false);
            }

            /*if (LobbyManager.players.ContainsKey(i))
            {
                playerTypeViews[i].gameObject.SetActive(true);

                if (LobbyManager.players[i].playerType != PlayerType.Default)
                {
                    playerTypeViews[i].SetPlayerTypeViewColour(LobbyManager.players[i].playerType == PlayerType.Hider? LobbyManager.players[i].hiderColour : LobbyManager.players[i].seekerColour);
                } else
                {
                    playerTypeViews[i].SetPlayerTypeViewColour(Color.grey);
                }
            }
            else
            {
                playerTypeViews[i].gameObject.SetActive(false);
            }*/
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
