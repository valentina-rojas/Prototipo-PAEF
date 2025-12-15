using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CompletarFrasesManager : MonoBehaviour
{
    public TMP_Text fraseTexto;
    public Button[] botonesOpciones;
    public TMP_Text feedbackTexto;
    [SerializeField] private Button botonVolverMenu;
    [SerializeField] private Button botonReintentar;
    [SerializeField] private GameObject panelCompletarFrase;
    [SerializeField] private GameManager gameManager;

    private FraseIncompleta fraseActual;

    private void Start()
    {
        if (botonVolverMenu != null)
        {
            botonVolverMenu.gameObject.SetActive(false);
            botonVolverMenu.onClick.AddListener(() =>
            {
                panelCompletarFrase.SetActive(false);
                if (gameManager != null)
                {
                    gameManager.VolverAlPrincipal();
                }
            });
        }

        if (botonReintentar != null)
        {
            botonReintentar.gameObject.SetActive(false);
            botonReintentar.onClick.AddListener(() =>
            {
                ReiniciarOpciones();
                botonReintentar.gameObject.SetActive(false);
            });
        }
    }

    public void MostrarFrase(FraseIncompleta frase)
    {
        if (frase == null) return;

        fraseActual = frase;
        fraseTexto.text = frase.frase;
        feedbackTexto.text = "";

        string[] opcionesMezcladas = Mezclar(frase.opciones);

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            int index = i;
            if (index < opcionesMezcladas.Length)
            {
                string palabra = opcionesMezcladas[index];
                botonesOpciones[index].GetComponentInChildren<TMP_Text>().text = palabra;

                botonesOpciones[index].onClick.RemoveAllListeners();
                botonesOpciones[index].onClick.AddListener(() =>
                    Verificar(palabra, botonesOpciones[index]));

                botonesOpciones[index].gameObject.SetActive(true);
                botonesOpciones[index].interactable = true;
                botonesOpciones[index].GetComponent<Image>().color = Color.white;
            }
            else
            {
                botonesOpciones[index].gameObject.SetActive(false);
            }
        }

        if (botonVolverMenu != null) botonVolverMenu.gameObject.SetActive(false);
        if (botonReintentar != null) botonReintentar.gameObject.SetActive(false);
    }

    void Verificar(string palabra, Button botonSeleccionado)
    {
        Color verdeClarito = new Color(0.6f, 1f, 0.6f);
        Color rojoClarito = new Color(1f, 0.6f, 0.6f);

        foreach (Button b in botonesOpciones)
            b.interactable = false;

        if (palabra == fraseActual.respuestaCorrecta)
        {
            AudioManager.instance.sonidoRespuestaCorrecta.Play();

            feedbackTexto.text = "¡Correcto!";
            feedbackTexto.color = Color.green;

            botonSeleccionado.GetComponent<Image>().color = verdeClarito;

            StartCoroutine(VibrarBoton(
                botonSeleccionado.GetComponent<RectTransform>(),
                10f, 0.15f));

            if (gameManager != null)
            {
                gameManager.GanarExperiencia(1);
                
                StartCoroutine(VolverAutomaticamente());
            }
        }
        else
        {
            AudioManager.instance.sonidoRespuestaIncorrecta.Play();

            feedbackTexto.text = "Incorrecto. ¡Intentá de nuevo!";
            feedbackTexto.color = Color.red;

            botonSeleccionado.GetComponent<Image>().color = rojoClarito;

            StartCoroutine(VibrarBoton(
                botonSeleccionado.GetComponent<RectTransform>(),
                12f, 0.18f));

            if (botonReintentar != null)
                botonReintentar.gameObject.SetActive(true);
        }
    }

    private System.Collections.IEnumerator VolverAutomaticamente()
    {
        yield return new WaitForSeconds(2f);
        
        panelCompletarFrase.SetActive(false);
        if (gameManager != null)
        {
            gameManager.VolverAlPrincipal();
        }
    }

    private void ReiniciarOpciones()
    {
        foreach (Button b in botonesOpciones)
        {
            b.interactable = true;
            b.GetComponent<Image>().color = Color.white;
        }

        feedbackTexto.text = "";
    }

    string[] Mezclar(string[] array)
    {
        string[] copia = (string[])array.Clone();
        for (int i = 0; i < copia.Length; i++)
        {
            int randomIndex = Random.Range(i, copia.Length);
            (copia[i], copia[randomIndex]) = (copia[randomIndex], copia[i]);
        }
        return copia;
    }

    private System.Collections.IEnumerator VibrarBoton(RectTransform btn, float intensidad = 10f, float duracion = 0.15f)
    {
        Vector3 posOriginal = btn.anchoredPosition;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            float x = Random.Range(-intensidad, intensidad);
            btn.anchoredPosition = posOriginal + new Vector3(x, 0, 0);

            tiempo += Time.deltaTime;
            yield return null;
        }

        btn.anchoredPosition = posOriginal;
    }
}
