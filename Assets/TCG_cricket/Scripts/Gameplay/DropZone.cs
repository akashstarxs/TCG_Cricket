using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum ZoneType { Hand, PlayArea }
    public ZoneType zoneType;

    public System.Action<CardDragHandler> OnCardDropped;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        var card = eventData.pointerDrag.GetComponent<CardDragHandler>();
        if (card == null) return;

        // Re-parent card to this zone
        card.transform.SetParent(transform);
        card.transform.localScale = Vector3.one;

        // Notify external systems
        OnCardDropped?.Invoke(card);

        Debug.Log($"Card {card.CardId} dropped on {zoneType}");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Optional highlight
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Optional highlight
    }
}