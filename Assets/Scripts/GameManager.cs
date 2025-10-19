using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Referencias principales")]
    [SerializeField] private CuentosManager cuentosManager;
    [SerializeField] private CuestionarioManager cuestionarioManager;

    [Header("Botones principales")]
    [SerializeField] private Button botonAlimentar;

    [Header("Paneles")]
    [SerializeField] private GameObject panelGeneros;
    [SerializeField] private GameObject panelCuento;
    [SerializeField] private GameObject panelCuestionario;

    [Header("Botones de géneros")]
    [SerializeField] private Button botonCienciaFiccion;
    [SerializeField] private Button botonMisterio;
    [SerializeField] private Button botonAventura;

    [Header("Nivel")]
    [SerializeField] private TMP_Text textoNivel;
    [SerializeField] private GameObject flechaNivel;
    [SerializeField] private float duracionFlecha = 2f;

    private int nivelActual = 1;

    private void Start()
    {
        panelGeneros.SetActive(false);
        panelCuento.SetActive(false);
        panelCuestionario.SetActive(false);

        if (flechaNivel != null)
            flechaNivel.SetActive(false);

        textoNivel.text = nivelActual.ToString();

        botonAlimentar.onClick.AddListener(MostrarPanelGeneros);
        botonCienciaFiccion.onClick.AddListener(() => IniciarLectura("ciencia ficcion"));
        botonMisterio.onClick.AddListener(() => IniciarLectura("misterio"));
        botonAventura.onClick.AddListener(() => IniciarLectura("aventura"));
    }

    private void MostrarPanelGeneros()
    {
        botonAlimentar.interactable = false;
        panelGeneros.SetActive(true);
    }

    private void IniciarLectura(string genero)
    {
        panelGeneros.SetActive(false);
        panelCuento.SetActive(true);
        cuentosManager.MostrarCuento(genero, this);
    }

    public void FinalizarLectura(string genero)
    {
        panelCuento.SetActive(false);
        panelCuestionario.SetActive(true);
        cuestionarioManager.MostrarPregunta(genero, this);
    }

    public void ResultadoCuestionario(bool correcto)
    {
        panelCuestionario.SetActive(false);
        botonAlimentar.interactable = true;

        if (correcto)
        {
            nivelActual++;
            textoNivel.text = nivelActual.ToString();
            if (flechaNivel != null)
                StartCoroutine(MostrarFlechaTemporaria());
        }
    }

    private IEnumerator MostrarFlechaTemporaria()
    {
        flechaNivel.SetActive(true);
        yield return new WaitForSeconds(duracionFlecha);
        flechaNivel.SetActive(false);
    }
}