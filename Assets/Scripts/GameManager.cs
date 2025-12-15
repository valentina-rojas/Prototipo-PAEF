using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
    [SerializeField] public Button botonLecturaAleatoria; 
    [SerializeField] private Button botonVolver;
    [SerializeField] public GameObject panelFondoMascota;
    [SerializeField] private GameObject panelGeneros;
    [SerializeField] private GameObject panelCuento;
    [SerializeField] private GameObject panelCuestionario;
    [SerializeField] private GameObject panelCompletarFrase;
    [SerializeField] private GameObject panelOrdenarFrase;

    [Header("Botón cerrar general")]
    [SerializeField] public Button botonVolverMenu;
    [SerializeField] public Button botonCerrarJuego;

    [Header("Confirmación volver al menú")]
    [SerializeField] private GameObject panelConfirmarVolver;
    [SerializeField] private Button botonVolverSi;
    [SerializeField] private Button botonVolverNo;

    [Header("Confirmación salir del juego")]
    [SerializeField] private GameObject panelConfirmarSalir;
    [SerializeField] private Button botonSalirSi;
    [SerializeField] private Button botonSalirNo;

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
    [SerializeField] private SpriteRenderer mascotaSpriteRenderer; 
    [SerializeField] private Sprite[] spritesEvolucion;
    [SerializeField] private Image plataformaImage; 
    [SerializeField] private Sprite[] spritesPlataforma;

    [Header("Animaciones de mascota")]
    [SerializeField] private Animator mascotaAnimator;
    [SerializeField] private AnimationClip[] animacionesFase; 
    
    private int experienciaActual = 0;
    private int nivelActual = 1;
    private int nivelGuardado = 1;
    private bool necesitaActualizacion = true;
    
    private enum FaseSeleccion { Genero, Escenario, Personaje }
    private FaseSeleccion faseActual;

    private string generoSeleccionado;
    private string escenarioSeleccionado;
    private string personajeSeleccionado;
    private string motivacionAleatoria; 
    private string extensionAleatoria;  

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
        panelFondoMascota.SetActive(true);
        
        if (barraExperiencia != null)
        {
            barraExperiencia.maxValue = experienciaPorNivel;
            barraExperiencia.value = experienciaActual;
        }

        CargarProgreso();
        ActualizarTextoExperiencia();
        
        nivelActual = Mathf.Max(nivelActual, nivelGuardado);
        AplicarEvolucionSegunNivel();

        botonAlimentar.onClick.AddListener(MostrarPanelGeneros);
        
        if (botonLecturaAleatoria != null)
        {
            botonLecturaAleatoria.onClick.RemoveAllListeners();
            botonLecturaAleatoria.onClick.AddListener(IniciarLecturaCompletamenteAleatoria);
        }
        
        if (botonVolver != null) botonVolver.gameObject.SetActive(false);

        if (botonVolverMenu != null)
        {
            botonVolverMenu.onClick.RemoveAllListeners();
            botonVolverMenu.onClick.AddListener(MostrarConfirmacionVolver);
            botonVolverMenu.gameObject.SetActive(false);
        }

        if (botonCerrarJuego != null)
        {
            botonCerrarJuego.onClick.RemoveAllListeners();
            botonCerrarJuego.onClick.AddListener(MostrarConfirmacionSalir);
        }

        panelConfirmarVolver?.SetActive(false);
        panelConfirmarSalir?.SetActive(false);

        if (botonVolverSi != null) botonVolverSi.onClick.AddListener(ConfirmarVolver);
        if (botonVolverNo != null) botonVolverNo.onClick.AddListener(CerrarPanelVolver);

        if (botonSalirSi != null) botonSalirSi.onClick.AddListener(ConfirmarSalir);
        if (botonSalirNo != null) botonSalirNo.onClick.AddListener(CerrarPanelSalir);
    }

    private void Update()
    {
        if (panelFondoMascota.activeSelf && necesitaActualizacion)
        {
            AplicarEvolucionSegunNivel();
            necesitaActualizacion = false;
        }
    }

    #region Sistema de guardado y carga de progreso
    private void CargarProgreso()
    {
        if (!PlayerPrefs.HasKey("Nivel"))
        {
            if (textoExperiencia != null && !string.IsNullOrEmpty(textoExperiencia.text))
            {
                nivelGuardado = ExtraerNivelDelTexto(textoExperiencia.text);
                experienciaActual = ExtraerExperienciaDelTexto(textoExperiencia.text);
            }
            else
            {
                nivelGuardado = 1;
                experienciaActual = 0;
            }
            nivelActual = nivelGuardado; 
        }
        else
        {
            nivelGuardado = PlayerPrefs.GetInt("Nivel", 1);
            experienciaActual = PlayerPrefs.GetInt("Experiencia", 0);
            nivelActual = nivelGuardado; 
        }
        
        Debug.Log($"Progreso cargado - Nivel: {nivelActual}, XP: {experienciaActual}");
    }

    private void GuardarProgreso()
    {
        PlayerPrefs.SetInt("Nivel", nivelActual);
        PlayerPrefs.SetInt("Experiencia", experienciaActual);
        PlayerPrefs.Save();
        Debug.Log($"Progreso guardado - Nivel: {nivelActual}, XP: {experienciaActual}");
    }
    #endregion

    #region Sistema de evolución de mascota
    private void AplicarEvolucionSegunNivel()
    {
        if (!panelFondoMascota.activeSelf)
        {
            necesitaActualizacion = true;
            return;
        }

        Debug.Log($"Aplicando evolución - Nivel: {nivelActual}");
        
        if (mascotaSpriteRenderer != null && spritesEvolucion != null && spritesEvolucion.Length > 0)
        {
            int faseVisual = Mathf.Min(nivelActual, 4); 
            int spriteIndex = Mathf.Clamp(faseVisual - 1, 0, spritesEvolucion.Length - 1);
            mascotaSpriteRenderer.sprite = spritesEvolucion[spriteIndex];
            Debug.Log($"Sprite aplicado: Nivel {nivelActual}, Fase Visual {faseVisual} (índice {spriteIndex})");
        }
        
        if (plataformaImage != null && spritesPlataforma != null && spritesPlataforma.Length > 0)
        {
            int faseVisual = Mathf.Min(nivelActual, 4);
            int plataformaIndex = Mathf.Clamp(faseVisual - 1, 0, spritesPlataforma.Length - 1);
            plataformaImage.sprite = spritesPlataforma[plataformaIndex];
            plataformaImage.SetNativeSize();
            Debug.Log($"Plataforma aplicada: Nivel {nivelActual}, Fase Visual {faseVisual}");
        }
        
        if (mascotaAnimator != null && animacionesFase != null && animacionesFase.Length > 0)
        {
            int faseVisual = Mathf.Min(nivelActual, 4);
            int indiceAnimacion = Mathf.Clamp(faseVisual - 1, 0, animacionesFase.Length - 1);
            if (animacionesFase[indiceAnimacion] != null)
            {
                mascotaAnimator.Play(animacionesFase[indiceAnimacion].name);
                Debug.Log($"Animación aplicada: {animacionesFase[indiceAnimacion].name} para fase {faseVisual}");
            }
        }
        
        nivelGuardado = nivelActual;
    }

    private int ExtraerNivelDelTexto(string texto)
    {
        if (string.IsNullOrEmpty(texto)) return nivelActual;
        
        Match match = Regex.Match(texto, @"Nivel\s+(\d+)");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int nivel))
        {
            return nivel;
        }
        
        match = Regex.Match(texto, @"\b(\d+)\b");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int num))
        {
            return num;
        }
        
        return nivelActual;
    }

    private int ExtraerExperienciaDelTexto(string texto)
    {
        if (string.IsNullOrEmpty(texto)) return experienciaActual;
        
        Match match = Regex.Match(texto, @"EXP\s+(\d+)\s*/\s*\d+");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int exp))
        {
            return exp;
        }
        
        match = Regex.Match(texto, @"(\d+)\s*/\s*\d+");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int exp2))
        {
            return exp2;
        }
        
        return experienciaActual;
    }
    #endregion

    public void IniciarLecturaCompletamenteAleatoria()
    {
        Debug.Log("Iniciando lectura completamente aleatoria");
        
        if (basesCuentos == null || basesCuentos.Length == 0)
        {
            Debug.LogError("No hay base de cuentos cargada");
            return;
        }

        var todosLosCuentos = basesCuentos
            .Where(b => b != null && b.cuentos != null)
            .SelectMany(b => b.cuentos)
            .Where(c => c != null)
            .ToList();

        if (todosLosCuentos.Count == 0)
        {
            Debug.LogError("No hay cuentos disponibles");
            return;
        }

        var cuentoAleatorio = todosLosCuentos[Random.Range(0, todosLosCuentos.Count)];

        generoSeleccionado = cuentoAleatorio.genero;
        escenarioSeleccionado = cuentoAleatorio.escenario;
        personajeSeleccionado = cuentoAleatorio.personaje;
        motivacionAleatoria = cuentoAleatorio.motivacion;
        extensionAleatoria = cuentoAleatorio.extension;

        Debug.Log($"Cuento aleatorio seleccionado: {cuentoAleatorio.genero} - {cuentoAleatorio.personaje} - {cuentoAleatorio.motivacion} - {cuentoAleatorio.extension}");

        panelGeneros.SetActive(false);
        panelCuento.SetActive(true);
        botonVolver.gameObject.SetActive(false);
        botonVolverMenu.gameObject.SetActive(true);
        panelFondoMascota.SetActive(false);

        ScrollRect scrollRect = panelCuento.GetComponentInChildren<ScrollRect>(true);
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 1f;
        }

        cuentosManager.MostrarCuento(cuentoAleatorio.texto, this);

        misionesManager?.CompletarMision(MisionTipo.LeerCuento);
        misionesManager?.CompletarMision(MisionTipo.LeerCuentoGenero, generoSeleccionado);
        
        Debug.Log("Lectura aleatoria iniciada correctamente");
    }

    private void MostrarConfirmacionVolver()
    {
        panelConfirmarVolver.SetActive(true);
        panelFondoMascota.SetActive(false);
    }

    private void CerrarPanelVolver()
    {
        panelConfirmarVolver.SetActive(false);
        panelFondoMascota.SetActive(false);
    }

    private void ConfirmarVolver()
    {
        panelConfirmarVolver.SetActive(false);
        VolverAlPrincipal();
    }

    private void MostrarConfirmacionSalir()
    {
        panelConfirmarSalir.SetActive(true);
        panelFondoMascota.SetActive(false);
    }

    private void CerrarPanelSalir()
    {
        panelConfirmarSalir.SetActive(false);
        panelFondoMascota.SetActive(true);
    }

    private void ConfirmarSalir()
    {
        GuardarProgreso();
        panelConfirmarSalir.SetActive(false);

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
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

    #region Selección de cuentos - Versión simplificada (3 pasos)
    public void MostrarPanelGeneros()
    {
        botonAlimentar.interactable = false;
        botonLecturaAleatoria.interactable = false; 
        panelFondoMascota.SetActive(false);
        botonVolverMenu.gameObject.SetActive(true);
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
            .OrderBy(g => g)
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
            .OrderBy(e => e)
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
            .OrderBy(p => p)
            .ToList();

        ConfigurarBotones(personajes, opcion =>
        {
            personajeSeleccionado = opcion;
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
        botonVolverMenu.gameObject.SetActive(true);
        panelFondoMascota.SetActive(false);

        ScrollRect scrollRect = panelCuento.GetComponentInChildren<ScrollRect>(true);
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 1f;
        }

        var cuentosDisponibles = basesCuentos
            .Where(b => b != null && b.cuentos != null)
            .SelectMany(b => b.cuentos)
            .Where(c =>
                c != null &&
                c.genero == generoSeleccionado &&
                c.escenario == escenarioSeleccionado &&
                c.personaje == personajeSeleccionado
            )
            .ToList();

        if (cuentosDisponibles.Count == 0)
        {
            Debug.LogError($"No se encontraron cuentos con los criterios seleccionados");
            botonAlimentar.interactable = true;
            botonLecturaAleatoria.interactable = true;
            panelCuento.SetActive(false);
            return;
        }

        var motivaciones = cuentosDisponibles
            .Select(c => c.motivacion)
            .Where(m => !string.IsNullOrEmpty(m))
            .Distinct()
            .ToList();
        
        motivacionAleatoria = motivaciones.Count > 0 
            ? motivaciones[Random.Range(0, motivaciones.Count)]
            : cuentosDisponibles[0].motivacion;

        var extensiones = cuentosDisponibles
            .Where(c => c.motivacion == motivacionAleatoria)
            .Select(c => c.extension)
            .Where(e => !string.IsNullOrEmpty(e))
            .Distinct()
            .ToList();

        if (extensiones.Count > 0)
        {
            if (extensiones.Contains("Corto"))
                extensionAleatoria = "Corto";
            else if (extensiones.Contains("Mediano"))
                extensionAleatoria = "Mediano";
            else
                extensionAleatoria = extensiones[Random.Range(0, extensiones.Count)];
        }
        else
        {
            extensionAleatoria = "Corto"; 
        }

        var cuentoFinal = cuentosDisponibles.FirstOrDefault(c =>
            c.motivacion == motivacionAleatoria &&
            c.extension == extensionAleatoria
        );

        if (cuentoFinal == null)
        {
            cuentoFinal = cuentosDisponibles[0];
            motivacionAleatoria = cuentoFinal.motivacion;
            extensionAleatoria = cuentoFinal.extension;
            Debug.Log($"No se encontró combinación exacta. Usando primer cuento disponible: {motivacionAleatoria} - {extensionAleatoria}");
        }
        else
        {
            Debug.Log($"Cuento seleccionado: {motivacionAleatoria} - {extensionAleatoria}");
        }

        Debug.Log($"Mostrando cuento: {cuentoFinal.genero} - {cuentoFinal.personaje} - {cuentoFinal.motivacion} - {cuentoFinal.extension}");
        cuentosManager.MostrarCuento(cuentoFinal.texto, this);
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
                c.motivacion == motivacionAleatoria &&
                c.extension == extensionAleatoria
            );

        if (cuentoFinal == null)
        {
            Debug.LogError("No se pudo encontrar el cuento para las evaluaciones");
            botonAlimentar.interactable = true;
            botonLecturaAleatoria.interactable = true;
            panelFondoMascota.SetActive(true);
            AplicarEvolucionSegunNivel(); 
            return;
        }

        panelCuestionario.SetActive(false);
        panelCompletarFrase.SetActive(false);
        panelOrdenarFrase.SetActive(false);
        panelFondoMascota.SetActive(false);

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
            botonLecturaAleatoria.interactable = true;
            panelFondoMascota.SetActive(true);
            AplicarEvolucionSegunNivel(); 
            return;
        }

        string tipoElegido = tiposDisponibles[Random.Range(0, tiposDisponibles.Count)];

        switch (tipoElegido)
        {
            case "Cuestionario":
                panelCuestionario.SetActive(true);
                botonVolverMenu.gameObject.SetActive(true);
                cuestionarioManager.MostrarCuestionario(cuentoFinal.cuestionario, this);
                break;

            case "Completar":
                panelCompletarFrase.SetActive(true);
                botonVolverMenu.gameObject.SetActive(true);
                var fraseElegida = cuentoFinal.fraseIncompleta[Random.Range(0, cuentoFinal.fraseIncompleta.Length)];
                completarFrasesManager.MostrarFrase(fraseElegida);
                break;

            case "Ordenar":
                panelOrdenarFrase.SetActive(true);
                botonVolverMenu.gameObject.SetActive(true);
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
        int nivelPrevio = nivelActual;
        experienciaActual += cantidad;

        if (experienciaActual >= experienciaPorNivel)
        {
            experienciaActual -= experienciaPorNivel;
            nivelActual++;
            Debug.Log($"¡Subió de nivel! Nuevo nivel: {nivelActual}");
        }

        if (barraExperiencia != null)
            barraExperiencia.value = experienciaActual;

        ActualizarTextoExperiencia();
        
        GuardarProgreso();
        
        if (nivelPrevio != nivelActual)
        {
            necesitaActualizacion = true;
        }
    }

    private void ActualizarTextoExperiencia()
    {
        if (textoExperiencia != null)
            textoExperiencia.text = $"Nivel {nivelActual} - EXP {experienciaActual}/{experienciaPorNivel}";
    }
    #endregion

    public void VolverAlPrincipal()
    {
        panelGeneros.SetActive(false);
        panelCuento.SetActive(false);
        panelCuestionario.SetActive(false);
        panelCompletarFrase.SetActive(false);
        panelOrdenarFrase.SetActive(false);
        panelFondoMascota.SetActive(true);

        AplicarEvolucionSegunNivel();

        botonAlimentar.interactable = true;
        botonLecturaAleatoria.interactable = true; 
        botonVolverMenu.gameObject.SetActive(false);
    }

    public void LeerCuentoAleatorioPorGenero(string genero)
    {
        if (basesCuentos == null || basesCuentos.Length == 0)
        {
            Debug.LogError("No hay base de cuentos cargada");
            return;
        }

        // Buscar cuentos del género solicitado
        var cuentosDelGenero = basesCuentos
            .Where(b => b != null && b.cuentos != null)
            .SelectMany(b => b.cuentos)
            .Where(c => c.genero != null &&
                        c.genero.Trim().ToLower() == genero.Trim().ToLower())
            .ToList();

        if (cuentosDelGenero.Count == 0)
        {
            Debug.LogWarning($"No hay cuentos del género: {genero}");
            return;
        }

        // Seleccionar uno al azar
        var cuento = cuentosDelGenero[Random.Range(0, cuentosDelGenero.Count)];

        Debug.Log($"Abriendo cuento aleatorio de género {genero}");

        // Cerrar paneles
        panelGeneros.SetActive(false);
        panelCuento.SetActive(true);
        botonVolver.gameObject.SetActive(false);
        botonVolverMenu.gameObject.SetActive(true);
        panelFondoMascota.SetActive(false);
        
        // Subir scroll al inicio
        ScrollRect scrollRect = panelCuento.GetComponentInChildren<ScrollRect>(true);
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 1f;
        }

        // Mostrar cuento
        cuentosManager.MostrarCuento(cuento.texto, this);

        // Opcional: marcar misión automáticamente al leer este cuento
        misionesManager?.CompletarMision(MisionTipo.LeerCuentoGenero, genero);
    }

    public void IniciarCuentoDesdeMision(string genero)
    {
        // Buscar un cuento aleatorio del género pedido por la misión
        var cuento = basesCuentos
            .SelectMany(b => b.cuentos)
            .Where(c => c.genero == genero)
            .OrderBy(_ => Random.value)
            .FirstOrDefault();

        if (cuento == null)
        {
            Debug.LogError("No se encontró cuento para misión de género: " + genero);
            return;
        }

        // Guardar variables internas (NECESARIO PARA FINALIZAR LECTURA)
        generoSeleccionado = cuento.genero;
        escenarioSeleccionado = cuento.escenario;
        personajeSeleccionado = cuento.personaje;
        motivacionAleatoria = cuento.motivacion;
        extensionAleatoria = cuento.extension;

        // Mostrar panel del cuento
        panelCuento.SetActive(true);
        panelGeneros.SetActive(false);
        panelFondoMascota.SetActive(false);

        botonVolverMenu.gameObject.SetActive(true);

        // Mostrar texto del cuento
        cuentosManager.MostrarCuento(cuento.texto, this);
    }
}