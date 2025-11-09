using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class ConfiguracionManager : MonoBehaviour
{
    [Header("Referencias principales")]
    [SerializeField] private TMP_Text textoCuento;
    [SerializeField] private RectTransform contentTransform; 
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject panelConfiguracion;
    [SerializeField] private Button botonConfiguracion;
    [SerializeField] private CuentosManager cuentosManager;

    [Header("Controles UI")]
    [SerializeField] private Slider sliderTamanoFuente;
    [SerializeField] private Slider sliderEspaciadoLineas;
    [SerializeField] private TMP_Dropdown dropdownFuente;
    [SerializeField] private TMP_Dropdown dropdownAlineacion;

    [Header("Fuentes disponibles")]
    [SerializeField] private TMP_FontAsset[] fuentesDisponibles;

    private bool panelVisible = false;

    private void Start()
    {
        panelConfiguracion.SetActive(false);
        botonConfiguracion.onClick.AddListener(TogglePanel);

        sliderTamanoFuente.onValueChanged.AddListener(CambiarTamanoFuente);
        sliderEspaciadoLineas.onValueChanged.AddListener(v =>
        {
            textoCuento.lineSpacing = v;
            ActualizarScrollView();
            cuentosManager.RecalcularPaginas();
        });

        dropdownFuente.onValueChanged.AddListener(CambiarFuente);
        dropdownAlineacion.onValueChanged.AddListener(CambiarAlineacion);

        InicializarOpcionesFuentes();
        InicializarOpcionesAlineacion();

        sliderTamanoFuente.value = textoCuento.fontSize;
        sliderEspaciadoLineas.value = textoCuento.lineSpacing;

        // Si querés detectar cuando se llega al final del scroll:
        if (scrollRect != null)
            scrollRect.onValueChanged.AddListener(OnScrollChanged);
    }

    private void TogglePanel()
    {
        panelVisible = !panelVisible;
        panelConfiguracion.SetActive(panelVisible);
    }

    private void InicializarOpcionesFuentes()
    {
        dropdownFuente.ClearOptions();
        var nombres = new List<string>();
        foreach (var fuente in fuentesDisponibles)
            nombres.Add(fuente != null ? fuente.name : "Desconocida");
        dropdownFuente.AddOptions(nombres);
    }

    private void InicializarOpcionesAlineacion()
    {
        dropdownAlineacion.ClearOptions();
        dropdownAlineacion.AddOptions(new List<string> { "Izquierda", "Centro", "Derecha", "Justificado" });
    }

    private void CambiarTamanoFuente(float nuevoTamano)
    {
        textoCuento.fontSize = nuevoTamano;
        ActualizarScrollView();
        cuentosManager.RecalcularPaginas();
    }

    private void CambiarFuente(int indice)
    {
        if (indice >= 0 && indice < fuentesDisponibles.Length && fuentesDisponibles[indice] != null)
        {
            textoCuento.font = fuentesDisponibles[indice];
            ActualizarScrollView();
            cuentosManager.RecalcularPaginas();
        }
    }

        private void CambiarAlineacion(int indice)
    {
        switch (indice)
        {
            case 0: textoCuento.alignment = TextAlignmentOptions.TopLeft; break;
            case 1: textoCuento.alignment = TextAlignmentOptions.Top; break;
            case 2: textoCuento.alignment = TextAlignmentOptions.TopRight; break;
            case 3: textoCuento.alignment = TextAlignmentOptions.TopJustified; break;
        }

        // 🔹 Aseguramos que el texto esté anclado arriba
        textoCuento.rectTransform.anchorMin = new Vector2(0, 1);
        textoCuento.rectTransform.anchorMax = new Vector2(1, 1);
        textoCuento.rectTransform.pivot = new Vector2(0.5f, 1);

        // 🔹 Reseteamos la posición local para evitar que quede desplazado
        textoCuento.rectTransform.anchoredPosition = Vector2.zero;

        ActualizarScrollView();
        cuentosManager.RecalcularPaginas();
    }


    private void ActualizarScrollView()
    {
        StopAllCoroutines();
        StartCoroutine(RefrescarScroll());
    }

    private IEnumerator RefrescarScroll()
    {
        // Esperar un frame para que TMP actualice su layout
        yield return null;

        // 🔹 Actualizar el mesh del texto
        textoCuento.ForceMeshUpdate();
        LayoutRebuilder.ForceRebuildLayoutImmediate(textoCuento.rectTransform);

        // 🔹 Forzar el layout del content
        if (contentTransform != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform);

            // Si hay ContentSizeFitter, aplicarlo manualmente
            var fitter = contentTransform.GetComponent<ContentSizeFitter>();
            if (fitter != null)
                fitter.SetLayoutVertical();
        }

        // 🔹 Forzar actualización global del Canvas
        Canvas.ForceUpdateCanvases();

        // Esperar otro frame para que se actualice el ScrollRect
        yield return null;

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform);
        Canvas.ForceUpdateCanvases();

        // 🔹 Mantener el scroll en la parte superior
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;

        Debug.Log($"Scroll actualizado -> texto: {textoCuento.preferredHeight}, content: {contentTransform.rect.height}");
    }

    // 🔹 Opcional: para detectar si se llegó al final del scroll
    private bool puedeVerificarScroll = false;
    private bool botonMostrado = false;

    private IEnumerator ActivarVerificacionScroll()
    {
        yield return null;
        puedeVerificarScroll = true;
    }

    private void OnScrollChanged(Vector2 pos)
    {
        if (puedeVerificarScroll && !botonMostrado && pos.y <= 0.05f)
        {
            botonMostrado = true;
            // acá podés activar un botón o evento al final del texto
        }
    }
}
