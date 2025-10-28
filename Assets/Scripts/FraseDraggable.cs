using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FraseDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform originalParent;
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private OrdenarFrasesManager manager;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        manager = FindObjectOfType<OrdenarFrasesManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(canvas.transform); // mover a la raíz del canvas para arrastrar libremente
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // Buscar la posición más cercana entre los demás elementos
        Transform nuevoPadre = manager.GetPosicionMasCercana(transform);
        transform.SetParent(nuevoPadre.parent);
        transform.SetSiblingIndex(nuevoPadre.GetSiblingIndex());

        // Reacomodar
        manager.ReordenarFrases();
    }
}
