using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDragHandler : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform originalParent;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rect;

    public int CardId { get; private set; }

    public void BindCard(int id)
    {
        CardId = id;
    }

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Find top canvas
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("CardDragHandler requires a Canvas to work.");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;

        // Let raycasts pass through this card while dragging
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f;

        // Move card to top of canvas for proper layering
        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move with mouse, scaled correctly for canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 pos
        );

        rect.anchoredPosition = pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Re-enable raycasts
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // If no zone accepted the card → return to hand
        if (transform.parent == canvas.transform)
        {
            transform.SetParent(originalParent);
        }

        rect.localScale = Vector3.one;
    }
}