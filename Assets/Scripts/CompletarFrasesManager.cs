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
                ReiniciarOpciones(); // ✅ limpiar colores y estado
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
            int index = i; // ✅ Copia local para evitar cierre incorrecto
            if (index < opcionesMezcladas.Length)
            {
                string palabra = opcionesMezcladas[index];
                botonesOpciones[index].GetComponentInChildren<TMP_Text>().text = palabra;
                botonesOpciones[index].onClick.RemoveAllListeners();
                botonesOpciones[index].onClick.AddListener(() => Verificar(palabra, botonesOpciones[index]));
                botonesOpciones[index].gameObject.SetActive(true);

                // 🔄 Reset visual e interacción
                botonesOpciones[index].interactable = true;
                botonesOpciones[index].GetComponent<Image>().color = Color.white;
            }
            else
            {
                botonesOpciones[index].gameObject.SetActive(false);
            }
        }
    }

    void Verificar(string palabra, Button botonSeleccionado)
    {
        Color verdeClarito = new Color(0.6f, 1f, 0.6f);
        Color rojoClarito = new Color(1f, 0.6f, 0.6f);

        // Desactivar todos los botones
        foreach (Button b in botonesOpciones)
            b.interactable = false;

        if (palabra == fraseActual.respuestaCorrecta)
        {
            feedbackTexto.text = "¡Correcto!";
            feedbackTexto.color = Color.green;
            botonSeleccionado.GetComponent<Image>().color = verdeClarito;
            gameManager.GanarExperiencia(1);
        }
        else
        {
            feedbackTexto.text = "Incorrecto";
            feedbackTexto.color = Color.red;
            botonSeleccionado.GetComponent<Image>().color = rojoClarito;
        }

        if (botonVolverMenu != null)
            botonVolverMenu.gameObject.SetActive(true);
    }

    // ✅ NUEVO: reinicia el color y estado de todos los botones
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
}
