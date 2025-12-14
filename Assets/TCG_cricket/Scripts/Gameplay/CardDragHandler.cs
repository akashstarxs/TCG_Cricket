using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDragHandler : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform originalParent;
    private int originalSiblingIndex;

    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rect;

    private Vector2 dragOffset;

    public int CardId { get; private set; }

    public void BindCard(int id)
    {
        CardId = id;
    }

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvas = GetComponentInParent<Canvas>()?.rootCanvas;
        if (canvas == null)
            Debug.LogError("CardDragHandler requires a Canvas to work.");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.85f;

        // Capture pointer offset relative to the card BEFORE reparenting
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            eventData.position,
            eventData.pressEventCamera,
            out dragOffset
        );

        // Move to top canvas so it renders above everything
        transform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 canvasPos
        );

        // Apply offset so card stays under finger/mouse
        rect.anchoredPosition = canvasPos - dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // If no drop zone accepted it, return to original parent
        if (transform.parent == canvas.transform)
        {
            transform.SetParent(originalParent, false);
            transform.SetSiblingIndex(originalSiblingIndex);
        }
    }
}
