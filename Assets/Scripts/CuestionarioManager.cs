using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CuestionarioManager : MonoBehaviour
{
    [SerializeField] private TMP_Text textoPregunta;
    [SerializeField] private Button botonRespuesta1;
    [SerializeField] private Button botonRespuesta2;
    [SerializeField] private Button botonRespuesta3;

    private Dictionary<string, Pregunta> preguntasPorGenero = new Dictionary<string, Pregunta>();
    private GameManager gameManager;

    private void Start()
    {
        InicializarPreguntas();
    }

    [System.Serializable]
    public class Pregunta
    {
        public string textoPregunta;
        public string[] respuestas;
        public int respuestaCorrecta;
    }

    private void InicializarPreguntas()
    {
        preguntasPorGenero.Add("ciencia ficcion", new Pregunta
        {
            textoPregunta = "¿Qué descubrió Lina en el cometa Eon-9?",
            respuestas = new[] { "Una melodía de datos", "Un mensaje en papel", "Un robot perdido" },
            respuestaCorrecta = 0
        });

        preguntasPorGenero.Add("misterio", new Pregunta
        {
            textoPregunta = "¿Qué encontraba Don Ernesto en el libro azul cada noche?",
            respuestas = new[] { "Palabras que se reordenaban", "Un mapa secreto", "Un tesoro escondido" },
            respuestaCorrecta = 0
        });

        preguntasPorGenero.Add("aventura", new Pregunta
        {
            textoPregunta = "¿Qué hacía la brújula de Naira?",
            respuestas = new[] { "Se movía indicando un camino", "Se quedaba quieta", "Se destruía sola" },
            respuestaCorrecta = 0
        });
    }

    public void MostrarPregunta(string genero, GameManager manager)
    {
        gameManager = manager;
        if (!preguntasPorGenero.ContainsKey(genero)) return;

        Pregunta pregunta = preguntasPorGenero[genero];
        textoPregunta.text = pregunta.textoPregunta;

        botonRespuesta1.GetComponentInChildren<TMP_Text>().text = pregunta.respuestas[0];
        botonRespuesta2.GetComponentInChildren<TMP_Text>().text = pregunta.respuestas[1];
        botonRespuesta3.GetComponentInChildren<TMP_Text>().text = pregunta.respuestas[2];

        botonRespuesta1.onClick.RemoveAllListeners();
        botonRespuesta2.onClick.RemoveAllListeners();
        botonRespuesta3.onClick.RemoveAllListeners();

        botonRespuesta1.onClick.AddListener(() => VerificarRespuesta(0, pregunta.respuestaCorrecta));
        botonRespuesta2.onClick.AddListener(() => VerificarRespuesta(1, pregunta.respuestaCorrecta));
        botonRespuesta3.onClick.AddListener(() => VerificarRespuesta(2, pregunta.respuestaCorrecta));
    }

    private void VerificarRespuesta(int seleccionada, int correcta)
    {
        Button botonSeleccionado = seleccionada switch
        {
            0 => botonRespuesta1,
            1 => botonRespuesta2,
            2 => botonRespuesta3,
            _ => null
        };

        if (botonSeleccionado != null)
            StartCoroutine(PintarBotonYCerrarPanel(botonSeleccionado, seleccionada == correcta));
    }

    private IEnumerator PintarBotonYCerrarPanel(Button boton, bool correcto)
    {
        Color original = boton.image.color;
        boton.image.color = correcto ? Color.green : Color.red;

        yield return new WaitForSeconds(1f);
        boton.image.color = original;

        gameManager.ResultadoCuestionario(correcto);
    }
}