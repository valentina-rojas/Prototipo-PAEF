using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class OrdenarFrasesManager : MonoBehaviour
{
    public Transform contenedorFrases;
    public GameObject prefabFrase;
    public Button botonVerificar;
    public TMP_Text feedbackTexto;
    [SerializeField] private Button botonVolverMenu;
    [SerializeField] private Button botonReintentar;
    [SerializeField] private GameObject panelOrdenarFrase;
    [SerializeField] private GameManager gameManager;

    private List<string> frasesActuales = new List<string>();
    private List<GameObject> botonesFrases = new List<GameObject>();

    private void Start()
    {
        if (botonVolverMenu != null)
        {
            botonVolverMenu.gameObject.SetActive(false);
            botonVolverMenu.onClick.AddListener(() =>
            {
                panelOrdenarFrase.SetActive(false);
                gameManager.botonAlimentar.interactable = true;
            });
        }

        if (botonReintentar != null)
        {
            botonReintentar.gameObject.SetActive(false);
            botonReintentar.onClick.AddListener(() =>
            {
                ReiniciarOrden();
                botonReintentar.gameObject.SetActive(false);
                botonVerificar.gameObject.SetActive(true);
            });
        }
    }

    public void MostrarFrases(OrdenarFrases ordenar)
    {
        botonVerificar.gameObject.SetActive(true);
        botonVolverMenu.gameObject.SetActive(false);
        if (botonReintentar != null) botonReintentar.gameObject.SetActive(false);

        foreach (var btn in botonesFrases) Destroy(btn);
        botonesFrases.Clear();
        frasesActuales.Clear();

        frasesActuales.AddRange(ordenar.frasesCorrectas);
        frasesActuales = Mezclar(frasesActuales);

        foreach (string frase in frasesActuales)
        {
            GameObject btnGO = Instantiate(prefabFrase, contenedorFrases);
            btnGO.GetComponentInChildren<TMP_Text>().text = frase;
            botonesFrases.Add(btnGO);
        }

        feedbackTexto.text = "";
        botonVerificar.onClick.RemoveAllListeners();
        botonVerificar.onClick.AddListener(() => Verificar(ordenar.frasesCorrectas));
    }

    List<string> Mezclar(List<string> lista)
    {
        List<string> copia = new List<string>(lista);
        for (int i = 0; i < copia.Count; i++)
        {
            int r = Random.Range(i, copia.Count);
            (copia[i], copia[r]) = (copia[r], copia[i]);
        }
        return copia;
    }

    public Transform GetPosicionMasCercana(Transform arrastrado)
    {
        float distanciaMin = float.MaxValue;
        Transform masCercano = contenedorFrases.GetChild(0);

        foreach (Transform t in contenedorFrases)
        {
            float distancia = Mathf.Abs(arrastrado.position.y - t.position.y);
            if (distancia < distanciaMin)
            {
                distanciaMin = distancia;
                masCercano = t;
            }
        }

        return masCercano;
    }

    public void ReordenarFrases()
    {
        botonesFrases.Clear();
        foreach (Transform t in contenedorFrases)
        {
            botonesFrases.Add(t.gameObject);
        }
    }

    void Verificar(string[] ordenCorrecto)
    {
        bool correcto = true;
        for (int i = 0; i < ordenCorrecto.Length; i++)
        {
            if (botonesFrases[i].GetComponentInChildren<TMP_Text>().text != ordenCorrecto[i])
            {
                correcto = false;
                break;
            }
        }

        Color verdeClarito = new Color(0.6f, 1f, 0.6f);
        Color rojoClarito = new Color(1f, 0.6f, 0.6f); // 🔹 Declarada solo una vez

        if (correcto)
        {
            feedbackTexto.text = "¡Correcto!";
            feedbackTexto.color = Color.green;
            gameManager.GanarExperiencia(1);

            foreach (var boton in botonesFrases)
            {
                Image imagen = boton.GetComponent<Image>();
                if (imagen != null) imagen.color = verdeClarito;
            }

            botonVolverMenu.gameObject.SetActive(true);
            botonVerificar.gameObject.SetActive(false);
        }
        else
        {
            feedbackTexto.text = "Incorrecto. ¡Intentá de nuevo!";
            feedbackTexto.color = Color.red;

            foreach (var boton in botonesFrases)
            {
                Image imagen = boton.GetComponent<Image>();
                if (imagen != null) imagen.color = rojoClarito;

                // 🔹 Deshabilitar drag
                FraseDraggable fd = boton.GetComponent<FraseDraggable>();
                if (fd != null) fd.dragHabilitado = false;
            }

            if (botonReintentar != null) botonReintentar.gameObject.SetActive(true);
            botonVerificar.gameObject.SetActive(false);
        }
    }


   private void ReiniciarOrden()
    {
        foreach (var boton in botonesFrases)
        {
            Image imagen = boton.GetComponent<Image>();
            if (imagen != null) imagen.color = Color.white;

            FraseDraggable fd = boton.GetComponent<FraseDraggable>();
            if (fd != null) fd.dragHabilitado = true; 
        }

        feedbackTexto.text = "";
    }

}
