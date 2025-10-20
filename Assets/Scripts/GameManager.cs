using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("Referencias principales")]
    [SerializeField] private CuentosManager cuentosManager;
    [SerializeField] private CuestionarioManager cuestionarioManager;
    [SerializeField] private BaseDeCuentos[] basesCuentos;

    [Header("Botones principales")]
    [SerializeField] private Button botonAlimentar;
    [SerializeField] private Button botonVolver; 

    [Header("Paneles")]
    [SerializeField] private GameObject panelGeneros;
    [SerializeField] private GameObject panelCuento;
    [SerializeField] private GameObject panelCuestionario;

    [Header("Botones de selección")]
    [SerializeField] private Button boton1;
    [SerializeField] private Button boton2;
    [SerializeField] private Button boton3;

    [Header("Nivel")]
    [SerializeField] private TMP_Text textoNivel;
    [SerializeField] private GameObject flechaNivel;
    [SerializeField] private float duracionFlecha = 2f;

    private int nivelActual = 1;

    private enum FaseSeleccion { Genero, Escenario, Personaje, Motivacion, Extension }
    private FaseSeleccion faseActual;

    private string generoSeleccionado;
    private string escenarioSeleccionado;
    private string personajeSeleccionado;
    private string motivacionSeleccionada;
    private string extensionSeleccionada;

    private List<Button> botones => new List<Button> { boton1, boton2, boton3 };

    private void Start()
    {
        panelGeneros.SetActive(false);
        panelCuento.SetActive(false);
        panelCuestionario.SetActive(false);

        if (flechaNivel != null)
            flechaNivel.SetActive(false);

        textoNivel.text = nivelActual.ToString();
        botonAlimentar.onClick.AddListener(MostrarPanelGeneros);

        if (botonVolver != null)
            botonVolver.gameObject.SetActive(false);
    }

    private void MostrarPanelGeneros()
    {
        botonAlimentar.interactable = false;
        panelGeneros.SetActive(true);
        MostrarOpcionesGenero();
    }

    #region Flujo jerárquico
    private void MostrarOpcionesGenero()
    {
        faseActual = FaseSeleccion.Genero;
        botonVolver.gameObject.SetActive(false); 

        var generos = basesCuentos.SelectMany(b => b.cuentos)
                                  .Select(c => c.genero)
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

        var escenarios = basesCuentos.SelectMany(b => b.cuentos)
                                     .Where(c => c.genero == generoSeleccionado)
                                     .Select(c => c.escenario)
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

        var personajes = basesCuentos.SelectMany(b => b.cuentos)
                                     .Where(c => c.genero == generoSeleccionado && c.escenario == escenarioSeleccionado)
                                     .Select(c => c.personaje)
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

        var motivaciones = basesCuentos.SelectMany(b => b.cuentos)
                                       .Where(c => c.genero == generoSeleccionado && c.escenario == escenarioSeleccionado && c.personaje == personajeSeleccionado)
                                       .Select(c => c.motivacion)
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
                botones[i].GetComponentInChildren<TMP_Text>().text = opciones[i];
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

        var cuentoFinal = basesCuentos.SelectMany(b => b.cuentos)
            .FirstOrDefault(c =>
                c.genero == generoSeleccionado &&
                c.escenario == escenarioSeleccionado &&
                c.personaje == personajeSeleccionado &&
                c.motivacion == motivacionSeleccionada &&
                c.extension == extensionSeleccionada
            );

        if (cuentoFinal != null)
            cuentosManager.MostrarCuento(cuentoFinal.texto, this);
    }
    #endregion

    public void FinalizarLectura(string genero)
    {
        panelCuento.SetActive(false);
        botonAlimentar.interactable = true;
    }
}