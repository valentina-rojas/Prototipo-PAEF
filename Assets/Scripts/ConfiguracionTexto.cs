using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ConfiguracionTexto : MonoBehaviour
{
    [Header("Referencias principales")]
    [SerializeField] private TMP_Text textoCuento; 
    [SerializeField] private GameObject panelConfiguracion; 
    [SerializeField] private Button botonConfiguracion; 

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
            ForzarActualizarLayout();
        });
        dropdownFuente.onValueChanged.AddListener(CambiarFuente);
        dropdownAlineacion.onValueChanged.AddListener(CambiarAlineacion);

        InicializarOpcionesFuentes();
        InicializarOpcionesAlineacion();

        sliderTamanoFuente.value = textoCuento.fontSize;
        sliderEspaciadoLineas.value = textoCuento.lineSpacing;
    }

    private void TogglePanel()
    {
        panelVisible = !panelVisible;
        panelConfiguracion.SetActive(panelVisible);

        if (panelVisible)
        {
            sliderTamanoFuente.value = textoCuento.fontSize;
            sliderEspaciadoLineas.value = textoCuento.lineSpacing;
        }
    }

    private void InicializarOpcionesFuentes()
    {
        dropdownFuente.ClearOptions();
        var nombres = new List<string>();
        foreach (var fuente in fuentesDisponibles)
        {
            nombres.Add(fuente != null ? fuente.name : "Desconocida");
        }
        dropdownFuente.AddOptions(nombres);
    }

    private void InicializarOpcionesAlineacion()
    {
        dropdownAlineacion.ClearOptions();
        dropdownAlineacion.AddOptions(new List<string>
        {
            "Izquierda", "Centro", "Derecha", "Justificado"
        });
    }

    private void CambiarTamanoFuente(float nuevoTamano)
    {
        textoCuento.fontSize = nuevoTamano;
        ForzarActualizarLayout();
    }

    private void CambiarFuente(int indice)
    {
        if (indice >= 0 && indice < fuentesDisponibles.Length && fuentesDisponibles[indice] != null)
        {
            textoCuento.font = fuentesDisponibles[indice];
            ForzarActualizarLayout();
        }
    }

    private void CambiarAlineacion(int indice)
    {
        switch (indice)
        {
            case 0: textoCuento.alignment = TextAlignmentOptions.Left; break;
            case 1: textoCuento.alignment = TextAlignmentOptions.Center; break;
            case 2: textoCuento.alignment = TextAlignmentOptions.Right; break;
            case 3: textoCuento.alignment = TextAlignmentOptions.Justified; break;
        }
        ForzarActualizarLayout();
    }

    // Este método fuerza que el Content del ScrollView se recalcule
    private void ForzarActualizarLayout()
    {
        if (textoCuento != null && textoCuento.transform.parent != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)textoCuento.transform.parent);
        }
    }
}
