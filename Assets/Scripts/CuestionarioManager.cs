using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CuestionarioManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text textoPregunta;
    [SerializeField] private Button[] botonesOpciones;
    [SerializeField] private Button botonVolverMenu;
    [SerializeField] private GameObject panelCuestionario;

    private Pregunta[] preguntas;
    private int indiceActual;
    private int puntaje;
    private GameManager gameManager;

    private void Start()
    {
        if (botonVolverMenu != null)
        {
            botonVolverMenu.gameObject.SetActive(false);
            botonVolverMenu.onClick.AddListener(() =>
            {
                panelCuestionario.SetActive(false);
                gameManager.botonAlimentar.interactable = true;
            });
        }
    }

    public void MostrarCuestionario(Pregunta[] cuestionario, GameManager manager)
    {
        preguntas = cuestionario;
        gameManager = manager;
        indiceActual = 0;
        puntaje = 0;

        if (botonVolverMenu != null)
            botonVolverMenu.gameObject.SetActive(false);

        MostrarPregunta();
    }

    private void MostrarPregunta()
    {
        if (indiceActual >= preguntas.Length)
        {
            textoPregunta.text = $"Has completado el cuestionario!\nPuntaje: {puntaje}/{preguntas.Length}";
            foreach (var btn in botonesOpciones)
                btn.gameObject.SetActive(false);

            if (botonVolverMenu != null)
                botonVolverMenu.gameObject.SetActive(true);

            return;
        }

        Pregunta p = preguntas[indiceActual];
        textoPregunta.text = p.texto;

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            if (i < p.opciones.Length)
            {
                botonesOpciones[i].gameObject.SetActive(true);
                botonesOpciones[i].GetComponentInChildren<TMP_Text>().text = p.opciones[i];
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
        if (seleccion == preguntas[indiceActual].respuestaCorrecta)
        {
            puntaje++;
            gameManager.GanarExperiencia(1); 
        }

        indiceActual++;
        MostrarPregunta();
    }
}

