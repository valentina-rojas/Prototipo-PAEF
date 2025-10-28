using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("Referencias principales")]
    [SerializeField] private CuentosManager cuentosManager;
    [SerializeField] private CuestionarioManager cuestionarioManager;
    [SerializeField] private CompletarFrasesManager completarFrasesManager;
    [SerializeField] private BaseDeCuentos[] basesCuentos;

    [Header("UI principales")]
    [SerializeField] public Button botonAlimentar;
    [SerializeField] private Button botonVolver;
    [SerializeField] private GameObject panelGeneros;
    [SerializeField] private GameObject panelCuento;
    [SerializeField] private GameObject panelCuestionario;
    [SerializeField] private GameObject panelCompletarFrase;

    [Header("Botones de selección")]
    [SerializeField] private Button boton1;
    [SerializeField] private Button boton2;
    [SerializeField] private Button boton3;

    [Header("Experiencia y barra")]
    [SerializeField] private Slider barraExperiencia;
    [SerializeField] private TMP_Text textoExperiencia;
    [SerializeField] private int experienciaPorNivel = 5; 

    [Header("Mascota y evolución")]
    [SerializeField] private Image mascotaImage; 
    [SerializeField] private Sprite[] spritesEvolucion; 

    private int experienciaActual = 0;
    private int nivelActual = 1;

    private enum FaseSeleccion { Genero, Escenario, Personaje, Motivacion, Extension }
    private FaseSeleccion faseActual;

    private string generoSeleccionado;
    private string escenarioSeleccionado;
    private string personajeSeleccionado;
    private string motivacionSeleccionada;
    private string extensionSeleccionada;

    private List<Button> botones => new List<Button> { boton1, boton2, boton3 };


    [SerializeField] private BaseDeCuentos baseDeCuentosPrincipal;


    private void Start()
    {
   
        basesCuentos = new BaseDeCuentos[] { baseDeCuentosPrincipal };

        panelGeneros.SetActive(false);
        panelCuento.SetActive(false);
        panelCuestionario.SetActive(false);
        panelCompletarFrase.SetActive(false);


        if (barraExperiencia != null)
        {
            barraExperiencia.maxValue = experienciaPorNivel;
            barraExperiencia.value = experienciaActual;
        }
        ActualizarTextoExperiencia();
        ActualizarMascota();

        botonAlimentar.onClick.AddListener(MostrarPanelGeneros);
        if (botonVolver != null) botonVolver.gameObject.SetActive(false);
    }

    #region Selección de cuento
    private void MostrarPanelGeneros()
    {
        botonAlimentar.interactable = false;
        panelGeneros.SetActive(true);
        MostrarOpcionesGenero();
    }

    private void MostrarOpcionesGenero()
    {
        faseActual = FaseSeleccion.Genero;
        botonVolver.gameObject.SetActive(false);

        if (basesCuentos == null || basesCuentos.Length == 0) return;

        var generos = basesCuentos
            .Where(b => b != null && b.cuentos != null)
            .SelectMany(b => b.cuentos)
            .Where(c => c != null)
            .Select(c => c.genero)
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct()
            .ToList();

        ConfigurarBotones(generos, opcion =>
        {
            generoSeleccionado = opcion;
            MostrarOpcionesEscenario();
        });
    }

    private void MostrarOpcionesEscenario()
    {
        faseActual = FaseSeleccion.Escenario;
        botonVolver.gameObject.SetActive(true);
        botonVolver.onClick.RemoveAllListeners();
        botonVolver.onClick.AddListener(() => MostrarOpcionesGenero());

        var escenarios = basesCuentos
            .Where(b => b != null && b.cuentos != null)
            .SelectMany(b => b.cuentos)
            .Where(c => c != null && c.genero == generoSeleccionado)
            .Select(c => c.escenario)
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct()
            .ToList();

        ConfigurarBotones(escenarios, opcion =>
        {
            escenarioSeleccionado = opcion;
            MostrarOpcionesPersonaje();
        });
    }

    private void MostrarOpcionesPersonaje()
    {
        faseActual = FaseSeleccion.Personaje;
        botonVolver.gameObject.SetActive(true);
        botonVolver.onClick.RemoveAllListeners();
        botonVolver.onClick.AddListener(() => MostrarOpcionesEscenario());

        var personajes = basesCuentos
            .Where(b => b != null && b.cuentos != null)
            .SelectMany(b => b.cuentos)
            .Where(c => c != null && c.genero == generoSeleccionado && c.escenario == escenarioSeleccionado)
            .Select(c => c.personaje)
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct()
            .ToList();

        ConfigurarBotones(personajes, opcion =>
        {
            personajeSeleccionado = opcion;
            MostrarOpcionesMotivacion();
        });
    }

    private void MostrarOpcionesMotivacion()
    {
        faseActual = FaseSeleccion.Motivacion;
        botonVolver.gameObject.SetActive(true);
        botonVolver.onClick.RemoveAllListeners();
        botonVolver.onClick.AddListener(() => MostrarOpcionesPersonaje());

        var motivaciones = basesCuentos
            .Where(b => b != null && b.cuentos != null)
            .SelectMany(b => b.cuentos)
            .Where(c => c != null && c.genero == generoSeleccionado && c.escenario == escenarioSeleccionado && c.personaje == personajeSeleccionado)
            .Select(c => c.motivacion)
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct()
            .ToList();

        ConfigurarBotones(motivaciones, opcion =>
        {
            motivacionSeleccionada = opcion;
            MostrarOpcionesExtension();
        });
    }

    private void MostrarOpcionesExtension()
    {
        faseActual = FaseSeleccion.Extension;
        botonVolver.gameObject.SetActive(true);
        botonVolver.onClick.RemoveAllListeners();
        botonVolver.onClick.AddListener(() => MostrarOpcionesMotivacion());

        var extensiones = new List<string> { "Corto", "Mediano", "Largo" };
        ConfigurarBotones(extensiones, opcion =>
        {
            extensionSeleccionada = opcion;
            IniciarLecturaSeleccionada();
        });
    }

    private void ConfigurarBotones(List<string> opciones, System.Action<string> callback)
    {
        for (int i = 0; i < botones.Count; i++)
        {
            if (i < opciones.Count)
            {
                botones[i].gameObject.SetActive(true);
                var texto = botones[i].GetComponentInChildren<TMP_Text>();
                if (texto != null) texto.text = opciones[i];
                botones[i].onClick.RemoveAllListeners();
                string opcion = opciones[i];
                botones[i].onClick.AddListener(() => callback(opcion));
            }
            else
            {
                botones[i].gameObject.SetActive(false);
            }
        }
    }

    private void IniciarLecturaSeleccionada()
    {
        panelGeneros.SetActive(false);
        panelCuento.SetActive(true);
        botonVolver.gameObject.SetActive(false);

        var cuentoFinal = basesCuentos
            .Where(b => b != null && b.cuentos != null)
            .SelectMany(b => b.cuentos)
            .FirstOrDefault(c =>
                c != null &&
                c.genero == generoSeleccionado &&
                c.escenario == escenarioSeleccionado &&
                c.personaje == personajeSeleccionado &&
                c.motivacion == motivacionSeleccionada &&
                c.extension == extensionSeleccionada
            );

        if (cuentoFinal != null)
        {
            cuentosManager.MostrarCuento(cuentoFinal.texto, this);
        }
    }
    #endregion

    public void FinalizarLectura(string genero)
    {
        panelCuento.SetActive(false);

        var cuentoFinal = basesCuentos
            .Where(b => b != null && b.cuentos != null)
            .SelectMany(b => b.cuentos)
            .FirstOrDefault(c =>
                c != null &&
                c.genero == generoSeleccionado &&
                c.escenario == escenarioSeleccionado &&
                c.personaje == personajeSeleccionado &&
                c.motivacion == motivacionSeleccionada &&
                c.extension == extensionSeleccionada
            );

        if (cuentoFinal == null)
        {
            botonAlimentar.interactable = true;
            return;
        }

    if (cuentoFinal == null)
    {
        Debug.LogWarning("CuentoFinal es null");
    }
    else
    {
        Debug.Log($"Cuento encontrado: {cuentoFinal.texto}");
        Debug.Log("Frases disponibles en este cuento: " + (cuentoFinal.fraseIncompleta?.Length ?? 0));
    }


    // Determinar aleatoriamente qué tipo de evaluación mostrar
    bool tieneCuestionario = cuentoFinal.cuestionario != null && cuentoFinal.cuestionario.Length > 0;
    bool tieneFrase = cuentoFinal.fraseIncompleta != null && cuentoFinal.fraseIncompleta.Length > 0;

    if (!tieneCuestionario && !tieneFrase)
    {
        // No hay evaluación, volver al inicio
        panelCuestionario.SetActive(false);
        panelCompletarFrase.SetActive(false);
        botonAlimentar.interactable = true;
        return;
    }

    bool usarCuestionario = Random.value < 0.5f; // 50% de probabilidad

    if (usarCuestionario && tieneCuestionario)
    {
        panelCuestionario.SetActive(true);
        panelCompletarFrase.SetActive(false);
        cuestionarioManager.MostrarCuestionario(cuentoFinal.cuestionario, this);
    }
    else if (tieneFrase)
    {
        panelCuestionario.SetActive(false);
        panelCompletarFrase.SetActive(true);
        var fraseElegida = cuentoFinal.fraseIncompleta[Random.Range(0, cuentoFinal.fraseIncompleta.Length)];
        completarFrasesManager.MostrarFrase(fraseElegida);
    }
    else
    {
        // Si aleatoriamente salió cuestionario pero no tiene, mostrar frase
        panelCuestionario.SetActive(false);
        panelCompletarFrase.SetActive(false);
        botonAlimentar.interactable = true;
    }

    }


    public void GanarExperiencia(int cantidad)
    {
        experienciaActual += cantidad;

        if (experienciaActual >= experienciaPorNivel)
        {
            experienciaActual -= experienciaPorNivel;
            nivelActual++;
            Debug.Log($"🎉 ¡Mascota subió a nivel {nivelActual}!");
            ActualizarMascota();
        }

        if (barraExperiencia != null)
            barraExperiencia.value = experienciaActual;

        ActualizarTextoExperiencia();
    }

    private void ActualizarTextoExperiencia()
    {
        if (textoExperiencia != null)
            textoExperiencia.text = $"Nivel {nivelActual} - EXP {experienciaActual}/{experienciaPorNivel}";
    }

    private void ActualizarMascota()
    {
        if (mascotaImage != null && spritesEvolucion != null && spritesEvolucion.Length > 0)
        {
            int spriteIndex = Mathf.Min(nivelActual - 1, spritesEvolucion.Length - 1);
            mascotaImage.sprite = spritesEvolucion[spriteIndex];
            mascotaImage.SetNativeSize();
        }
    }
}