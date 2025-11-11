using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CuestionarioManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text textoPregunta;
    [SerializeField] private Button[] botonesOpciones;
    [SerializeField] private TMP_Text feedbackTexto; 
    [SerializeField] private Button botonVolverMenu;
    [SerializeField] private Button botonReintentar; 
    [SerializeField] private GameObject panelCuestionario;

    private Pregunta[] preguntas;
    private int indiceActual;
    private GameManager gameManager;

    private void Start()
    {
        if (botonVolverMenu != null)
        {
            botonVolverMenu.gameObject.SetActive(false);
            botonVolverMenu.onClick.AddListener(() =>
            {
                panelCuestionario.SetActive(false);
                gameManager.botonCerrarGeneral.gameObject.SetActive(false);
                gameManager.botonAlimentar.interactable = true;
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

    public void MostrarCuestionario(Pregunta[] cuestionario, GameManager manager)
    {
        preguntas = cuestionario;
        gameManager = manager;
        indiceActual = 0;

        if (botonVolverMenu != null)
            botonVolverMenu.gameObject.SetActive(false);
        if (botonReintentar != null)
            botonReintentar.gameObject.SetActive(false);

        MostrarPregunta();
    }

    private void MostrarPregunta()
    {
        if (indiceActual >= preguntas.Length)
        {
            textoPregunta.text = "¡Has completado el cuestionario!";
            foreach (var btn in botonesOpciones)
                btn.gameObject.SetActive(false);

            if (botonVolverMenu != null)
                botonVolverMenu.gameObject.SetActive(true);

            feedbackTexto.text = "";
            return;
        }

        Pregunta p = preguntas[indiceActual];
        textoPregunta.text = p.texto;
        feedbackTexto.text = "";

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            if (i < p.opciones.Length)
            {
                botonesOpciones[i].gameObject.SetActive(true);
                botonesOpciones[i].GetComponentInChildren<TMP_Text>().text = p.opciones[i];
                botonesOpciones[i].interactable = true;
                botonesOpciones[i].GetComponent<Image>().color = Color.white;

                botonesOpciones[i].onClick.RemoveAllListeners();
                int opcion = i;
                botonesOpciones[i].onClick.AddListener(() => EvaluarRespuesta(opcion));
            }
            else
            {
                botonesOpciones[i].gameObject.SetActive(false);
            }
        }
    }

    private void EvaluarRespuesta(int seleccion)
    {
        Pregunta p = preguntas[indiceActual];
        Color verdeClarito = new Color(0.6f, 1f, 0.6f);
        Color rojoClarito = new Color(1f, 0.6f, 0.6f);

        // Desactivar todos los botones
        foreach (var btn in botonesOpciones)
            btn.interactable = false;

        if (seleccion == p.respuestaCorrecta)
        {
            feedbackTexto.text = "¡Correcto!";
            feedbackTexto.color = Color.green;

            botonesOpciones[seleccion].GetComponent<Image>().color = verdeClarito;

            gameManager.GanarExperiencia(1);
            if (botonVolverMenu != null) botonVolverMenu.gameObject.SetActive(true);
        }
        else
        {
            feedbackTexto.text = "Incorrecto. ¡Intentá de nuevo!";
            feedbackTexto.color = Color.red;

            botonesOpciones[seleccion].GetComponent<Image>().color = rojoClarito;

            if (botonReintentar != null) botonReintentar.gameObject.SetActive(true);
        }
    }

    private void ReiniciarOpciones()
    {
        foreach (var btn in botonesOpciones)
        {
            btn.interactable = true;
            btn.GetComponent<Image>().color = Color.white;
        }

        feedbackTexto.text = "";
    }
}
