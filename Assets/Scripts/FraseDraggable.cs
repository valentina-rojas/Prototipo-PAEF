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

    public bool dragHabilitado = true;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        manager = FindObjectOfType<OrdenarFrasesManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!dragHabilitado) return;

        originalParent = transform.parent;
        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragHabilitado) return; 
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragHabilitado) return;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        Transform slotCercano = manager.GetPosicionMasCercana(transform);

        // si el slot ya tenía otra frase
        if (slotCercano.childCount > 0)
        {
            Transform otraFrase = slotCercano.GetChild(0);

            // intercambiar slots
            otraFrase.SetParent(originalParent);
            otraFrase.localPosition = Vector3.zero;
        }

        // colocar la frase arrastrada en el slot elegido
        transform.SetParent(slotCercano);
        transform.localPosition = Vector3.zero;

        // actualizar lista interna
        manager.ReordenarFrases();
    }

}
