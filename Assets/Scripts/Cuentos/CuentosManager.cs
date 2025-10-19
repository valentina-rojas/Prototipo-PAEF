using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CuentosManager : MonoBehaviour
{
    [Header("Texto y navegación")]
    [SerializeField] private TMP_Text textoCuento;
    [SerializeField] private Button botonSiguiente;
    [SerializeField] private Button botonAnterior;
    [SerializeField] private Button botonFinalizar;

    private List<string> paginas;
    private int paginaActual;
    private string textoCompletoActual = "";
    private GameManager gameManager;

    private void Start()
    {
        botonSiguiente.onClick.AddListener(PaginaSiguiente);
        botonAnterior.onClick.AddListener(PaginaAnterior);
        botonFinalizar.onClick.AddListener(FinalizarLectura);
        botonFinalizar.gameObject.SetActive(false);
    }

    public void MostrarCuento(string texto, GameManager manager)
    {
        gameManager = manager;
        textoCompletoActual = texto;
        paginas = DividirEnPaginas(textoCompletoActual);
        paginaActual = 0;
        MostrarPagina();
    }

    private void MostrarPagina()
    {
        if (paginas == null || paginas.Count == 0)
        {
            textoCuento.text = "";
            botonAnterior.interactable = false;
            botonSiguiente.interactable = false;
            botonFinalizar.gameObject.SetActive(false);
            return;
        }

        textoCuento.text = paginas[paginaActual];
        botonAnterior.interactable = paginaActual > 0;
        botonSiguiente.interactable = paginaActual < paginas.Count - 1;
        botonFinalizar.gameObject.SetActive(paginaActual == paginas.Count - 1);
    }

    private void PaginaSiguiente()
    {
        if (paginaActual < paginas.Count - 1)
        {
            paginaActual++;
            MostrarPagina();
        }
    }

    private void PaginaAnterior()
    {
        if (paginaActual > 0)
        {
            paginaActual--;
            MostrarPagina();
        }
    }

    private void FinalizarLectura()
    {
        gameManager.FinalizarLectura(""); 
    }

    public void RecalcularPaginas()
    {
        if (string.IsNullOrEmpty(textoCompletoActual)) return;

        int paginaAntes = paginaActual;
        paginas = DividirEnPaginas(textoCompletoActual);
        paginaActual = Mathf.Clamp(paginaAntes, 0, paginas.Count - 1);
        MostrarPagina();
    }

    private List<string> DividirEnPaginas(string textoCompleto)
    {
        List<string> resultado = new List<string>();
        if (string.IsNullOrEmpty(textoCompleto)) return resultado;

        textoCuento.textWrappingMode = TextWrappingModes.Normal;

        RectTransform rect = textoCuento.GetComponent<RectTransform>();
        float alturaMaxima = rect.rect.height;
        float anchoMaximo = rect.rect.width;
        float margenSeguridad = textoCuento.fontSize * 0.5f;

        string[] palabras = textoCompleto.Split(' ');
        string acumulado = "";

        foreach (string palabra in palabras)
        {
            string prueba = string.IsNullOrEmpty(acumulado) ? palabra : acumulado + " " + palabra;
            Vector2 dims = textoCuento.GetPreferredValues(prueba, anchoMaximo, 0f);

            if (dims.y + margenSeguridad > alturaMaxima)
            {
                resultado.Add(acumulado.TrimEnd());
                acumulado = palabra;
            }
            else
            {
                acumulado = prueba;
            }
        }

        if (!string.IsNullOrEmpty(acumulado))
            resultado.Add(acumulado.TrimEnd());

        if (resultado.Count == 0)
            resultado.Add(textoCompleto);

        return resultado;
    }
}