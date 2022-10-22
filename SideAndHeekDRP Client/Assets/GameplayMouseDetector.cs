using UnityEngine;
using UnityEngine.EventSystems;

public class GameplayMouseDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameplayUI owner;
    public bool pointerOver = false;

    private void Start()
    {
        owner.mouseDetectors.Add(this);
        pointerOver = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerOver = true;
        owner.OnPointerEnterItem();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerOver = false;
        owner.OnPointerExitItem();
    }
}
