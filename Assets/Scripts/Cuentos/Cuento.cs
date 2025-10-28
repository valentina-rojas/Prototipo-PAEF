using UnityEngine;

[System.Serializable]
public class Pregunta
{
    public string texto;
    public string[] opciones;
    public int respuestaCorrecta;
}

[System.Serializable]
public class FraseIncompleta
{
    [TextArea]
    public string frase;          // Ej: "El mago vivía en un ___"
    public string[] opciones;     // Ej: ["bosque", "castillo", "cueva"]
    public string respuestaCorrecta; // Ej: "castillo"
}


[System.Serializable]
public class Cuento
{
    public string genero;
    public string escenario;
    public string personaje;
    public string motivacion;
    public string extension;
    [TextArea(5, 20)]
    public string texto;
    public Pregunta[] cuestionario;
    public FraseIncompleta[] fraseIncompleta;
}

[CreateAssetMenu(fileName = "BaseDeCuentos", menuName = "Cuentos/Base de Cuentos")]
public class BaseDeCuentos : ScriptableObject
{
    public Cuento[] cuentos;
}