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
    [SerializeField] private OrdenarFrasesManager OrdenarFrasesManager;
    [SerializeField] private MisionesManager misionesManager;

    [Header("Base de cuentos")]
    [SerializeField] private TextAsset baseDeCuentosJSON;

    private BaseDeCuentos[] basesCuentos;

    [Header("UI principales")]
    [SerializeField] public Button botonAlimentar;
    [SerializeField] private Button botonVolver;
    [SerializeField] private GameObject panelGeneros;
    [SerializeField] private GameObject panelCuento;
    [SerializeField] private GameObject panelCuestionario;
    [SerializeField] private GameObject panelCompletarFrase;
    [SerializeField] private GameObject panelOrdenarFrase;

    [Header("Botón cerrar general")]
    [SerializeField] public Button botonCerrarGeneral;

    [Header("Botones de selección")]
    [SerializeField] private TMP_Text tituloSeleccionTexto;
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

    [System.Serializable]
    private class BaseDeCuentosWrapper
    {
        public Cuento[] cuentos;
    }

    private void Start()
    {
        CargarBaseDeCuentosDesdeJSON();

        panelGeneros.SetActive(false);
        panelCuento.SetActive(false);
        panelCuestionario.SetActive(false);
        panelCompletarFrase.SetActive(false);
        panelOrdenarFrase.SetActive(false);

        if (barraExperiencia != null)
        {
            barraExperiencia.maxValue = experienciaPorNivel;
            barraExperiencia.value = experienciaActual;
        }

        ActualizarTextoExperiencia();
        ActualizarMascota();

        botonAlimentar.onClick.AddListener(MostrarPanelGeneros);
        if (botonVolver != null) botonVolver.gameObject.SetActive(false);

        if (botonCerrarGeneral != null)
        {
            botonCerrarGeneral.onClick.AddListener(VolverAlPrincipal);
            botonCerrarGeneral.gameObject.SetActive(false);
        }

    }

    private void CargarBaseDeCuentosDesdeJSON()
    {
        if (baseDeCuentosJSON == null)
        {
            Debug.LogError("No se asignó el archivo baseDeCuentos.json en el GameManager");
            return;
        }

        string json = baseDeCuentosJSON.text;
        BaseDeCuentosWrapper wrapper = JsonUtility.FromJson<BaseDeCuentosWrapper>(json);

        if (wrapper == null || wrapper.cuentos == null || wrapper.cuentos.Length == 0)
        {
            Debug.LogError("El JSON está vacío o mal formateado");
            return;
        }

        BaseDeCuentos baseRuntime = ScriptableObject.CreateInstance<BaseDeCuentos>();
        baseRuntime.cuentos = wrapper.cuentos;
        basesCuentos = new BaseDeCuentos[] { baseRuntime };

        Debug.Log($"Base de cuentos cargada ({baseRuntime.cuentos.Length} cuentos)");
    }

    #region Selección de cuentos
    public void MostrarPanelGeneros()
    {
        botonAlimentar.interactable = false;
        botonCerrarGeneral.gameObject.SetActive(true);
        panelGeneros.SetActive(true);
        MostrarOpcionesGenero();
    }

    private void MostrarOpcionesGenero()
    {
        faseActual = FaseSeleccion.Genero;
        botonVolver.gameObject.SetActive(false);
        tituloSeleccionTexto.text = "Seleccioná un género";

        if (basesCuentos == null || basesCuentos.Length == 0)
        {
            Debug.LogError("No hay bases de cuentos cargadas");
            return;
        }

        var generos = basesCuentos
            .Where(b => b != null && b.cuentos != null)
            .SelectMany(b => b.cuentos)
            .Where(c => c != null)
            .Select(c => c.genero)
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct()
            .ToList();

        Debug.Log($"Géneros disponibles: {generos.Count}");

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
        tituloSeleccionTexto.text = "Seleccioná un escenario";

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
        tituloSeleccionTexto.text = "Seleccioná un personaje";

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
        tituloSeleccionTexto.text = "Seleccioná una motivación";

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
        tituloSeleccionTexto.text = "Seleccioná la extensión del cuento";

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

        ScrollRect scrollRect = panelCuento.GetComponentInChildren<ScrollRect>(true);
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases(); 
            scrollRect.verticalNormalizedPosition = 1f; 
        }

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
            Debug.Log($"Mostrando cuento: {cuentoFinal.genero} - {cuentoFinal.personaje}");
            cuentosManager.MostrarCuento(cuentoFinal.texto, this);
        }
        else
        {
            Debug.LogError($"No se encontró cuento con los criterios seleccionados");
            botonAlimentar.interactable = true;
            panelCuento.SetActive(false);
        }
    }


    #endregion

    #region Evaluaciones, XP y Mascota
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
            Debug.LogError("No se pudo encontrar el cuento para las evaluaciones");
            botonAlimentar.interactable = true;
            return;
        }

        panelCuestionario.SetActive(false);
        panelCompletarFrase.SetActive(false);
        panelOrdenarFrase.SetActive(false);

        List<string> tiposDisponibles = new List<string>();
        if (cuentoFinal.cuestionario != null && cuentoFinal.cuestionario.Length > 0)
            tiposDisponibles.Add("Cuestionario");
        if (cuentoFinal.fraseIncompleta != null && cuentoFinal.fraseIncompleta.Length > 0)
            tiposDisponibles.Add("Completar");
        if (cuentoFinal.ordenarFrases != null && cuentoFinal.ordenarFrases.Length > 0)
            tiposDisponibles.Add("Ordenar");

        if (tiposDisponibles.Count == 0)
        {
            Debug.Log("No hay evaluaciones disponibles para este cuento");
            botonAlimentar.interactable = true;
            return;
        }

        string tipoElegido = tiposDisponibles[Random.Range(0, tiposDisponibles.Count)];

        switch (tipoElegido)
        {
            case "Cuestionario":
                panelCuestionario.SetActive(true);
                botonCerrarGeneral.gameObject.SetActive(true);
                cuestionarioManager.MostrarCuestionario(cuentoFinal.cuestionario, this);
                break;

            case "Completar":
                panelCompletarFrase.SetActive(true);
                botonCerrarGeneral.gameObject.SetActive(true);
                var fraseElegida = cuentoFinal.fraseIncompleta[Random.Range(0, cuentoFinal.fraseIncompleta.Length)];
                completarFrasesManager.MostrarFrase(fraseElegida);
                break;

            case "Ordenar":
                panelOrdenarFrase.SetActive(true);
                botonCerrarGeneral.gameObject.SetActive(true);
                OrdenarFrasesManager.MostrarFrases(cuentoFinal.ordenarFrases[0]);
                break;
        }


        if (misionesManager != null)
        {
            misionesManager.CompletarMision(MisionTipo.LeerCuento);
            misionesManager.CompletarMision(MisionTipo.LeerCuentoGenero, generoSeleccionado);
        }
        else
        {
            Debug.LogWarning("MisionesManager no está asignado en GameManager");
        }
    }

    public void GanarExperiencia(int cantidad)
    {
        experienciaActual += cantidad;

        if (experienciaActual >= experienciaPorNivel)
        {
            experienciaActual -= experienciaPorNivel;
            nivelActual++;
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
    #endregion


    public void VolverAlPrincipal()
    {
        panelGeneros.SetActive(false);
        panelCuento.SetActive(false);
        panelCuestionario.SetActive(false);
        panelCompletarFrase.SetActive(false);
        panelOrdenarFrase.SetActive(false);

        botonAlimentar.interactable = true;
        botonCerrarGeneral.gameObject.SetActive(false);
    }

}
