using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropdownAutoScroll : MonoBehaviour
{
    [SerializeField] private RectTransform scrollRect;
    [SerializeField] private RectTransform viewportRect;
    [SerializeField] private RectTransform contentRect;

    private int topPadding = 0;
    private int botPadding = 0;

    private void Start()
    {
        VerticalLayoutGroup contentVLG = contentRect.GetComponent<VerticalLayoutGroup>();
        if (contentVLG != null)
        {
            topPadding = contentVLG.padding.top;
            botPadding = contentVLG.padding.bottom;
        }
    }

    public void OnItemSelected(RectTransform item)
    {
        float distanceToTop = -contentRect.anchoredPosition.y - item.anchoredPosition.y;
        float distanceToBot = viewportRect.rect.height - distanceToTop;

        if (distanceToTop < 0)
        {
            float newYPos = contentRect.anchoredPosition.y + distanceToTop - item.rect.height / 2f - topPadding;
            contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, newYPos);
        }

        if (distanceToBot < 0)
        {
            float newYPos = contentRect.anchoredPosition.y - distanceToBot + item.rect.height / 2f + botPadding;
            contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, newYPos);
        }
    }

    public void UpdateActive() //todo:remove this from DropdownAutoScroll and rename to AutoScroll
    {
        for (int i = 0; i < contentRect.childCount; i++)
        {
            if (contentRect.GetChild(i).gameObject.activeSelf)
            {
                contentRect.GetChild(i).GetComponent<Toggle>().Select();
            }
        }
    }
}
