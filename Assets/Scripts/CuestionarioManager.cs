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
    }

    [System.Serializable]
    public class Pregunta
    {
        public string textoPregunta;
        public string[] respuestas;
        public int respuestaCorrecta;
    }
}