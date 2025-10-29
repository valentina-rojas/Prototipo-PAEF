using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CompletarFrasesManager : MonoBehaviour
{
    public TMP_Text fraseTexto;
    public Button[] botonesOpciones;
    public TMP_Text feedbackTexto;
    [SerializeField] private Button botonVolverMenu;
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
                gameManager.botonAlimentar.interactable = true;
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
            if (i < opcionesMezcladas.Length)
            {
                string palabra = opcionesMezcladas[i];
                botonesOpciones[i].GetComponentInChildren<TMP_Text>().text = palabra;
                botonesOpciones[i].onClick.RemoveAllListeners();
                botonesOpciones[i].onClick.AddListener(() => Verificar(palabra));
                botonesOpciones[i].gameObject.SetActive(true);
            }
            else
            {
                botonesOpciones[i].gameObject.SetActive(false);
            }
        }
    }

    void Verificar(string palabra)
    {
        if (palabra == fraseActual.respuestaCorrecta)
        {
            feedbackTexto.text = "¡Correcto!";
            feedbackTexto.color = Color.green;
            gameManager.GanarExperiencia(1); 
        }
        else
        {
            feedbackTexto.text = "Incorrecto";
            feedbackTexto.color = Color.red;
        }

        if (botonVolverMenu != null)
                botonVolverMenu.gameObject.SetActive(true);
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
}
