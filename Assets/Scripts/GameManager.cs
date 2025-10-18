using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Botones principales")]
    [SerializeField] private Button botonAlimentar;

    [Header("Paneles")]
    [SerializeField] private GameObject panelGeneros;
    [SerializeField] private GameObject panelCuento;
    [SerializeField] private GameObject panelCuestionario;

    [Header("Botones de géneros")]
    [SerializeField] private Button botonCienciaFiccion;
    [SerializeField] private Button botonMisterio;
    [SerializeField] private Button botonAventura;

    [Header("Texto del cuento y scroll")]
    [SerializeField] private TMP_Text textoCuento;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Botones secundarios")]
    [SerializeField] private Button botonFinalizar;

    [Header("Cuestionario")]
    [SerializeField] private TMP_Text textoPregunta;
    [SerializeField] private Button botonRespuesta1;
    [SerializeField] private Button botonRespuesta2;
    [SerializeField] private Button botonRespuesta3;

    [Header("Nivel")]
    [SerializeField] private TMP_Text textoNivel;
    [SerializeField] private GameObject flechaNivel;
    [SerializeField] private float duracionFlecha = 2f;

    private bool botonMostrado = false;
    private bool puedeVerificarScroll = false;
    private string generoActual;
    private int nivelActual = 1;

    [System.Serializable]
    public class Pregunta
    {
        public string textoPregunta;
        public string[] respuestas;
        public int respuestaCorrecta;
    }

    private Dictionary<string, Pregunta> preguntasPorGenero = new Dictionary<string, Pregunta>();

    private void Start()
    {
        panelGeneros.SetActive(false);
        panelCuento.SetActive(false);
        panelCuestionario.SetActive(false);
        botonFinalizar.gameObject.SetActive(false);

        if (flechaNivel != null)
            flechaNivel.SetActive(false);

        textoNivel.text = nivelActual.ToString();

        botonAlimentar.onClick.AddListener(MostrarPanelGeneros);
        botonCienciaFiccion.onClick.AddListener(() => MostrarCuento("ciencia ficcion"));
        botonMisterio.onClick.AddListener(() => MostrarCuento("misterio"));
        botonAventura.onClick.AddListener(() => MostrarCuento("aventura"));

        if (botonFinalizar != null)
            botonFinalizar.onClick.AddListener(FinalizarLectura);

        InicializarPreguntas();
    }

    private void InicializarPreguntas()
    {
        preguntasPorGenero.Add("ciencia ficcion", new Pregunta
        {
            textoPregunta = "¿Qué descubrió Lina en el cometa Eon-9?",
            respuestas = new string[] { "Una melodía de datos", "Un mensaje en papel", "Un robot perdido" },
            respuestaCorrecta = 0
        });

        preguntasPorGenero.Add("misterio", new Pregunta
        {
            textoPregunta = "¿Qué encontraba Don Ernesto en el libro azul cada noche?",
            respuestas = new string[] { "Palabras que se reordenaban", "Un mapa secreto", "Un tesoro escondido" },
            respuestaCorrecta = 0
        });

        preguntasPorGenero.Add("aventura", new Pregunta
        {
            textoPregunta = "¿Qué hacía la brújula de Naira?",
            respuestas = new string[] { "Se movía indicando un camino", "Se quedaba quieta", "Se destruía sola" },
            respuestaCorrecta = 0
        });
    }

    private void MostrarPanelGeneros()
    {
        botonAlimentar.interactable = false;
        panelGeneros.SetActive(true);
    }

    private void MostrarCuento(string genero)
    {
        generoActual = genero;

        panelGeneros.SetActive(false);
        panelCuento.SetActive(true);
        botonFinalizar.gameObject.SetActive(false);
        botonMostrado = false;
        puedeVerificarScroll = false;

        scrollRect.verticalNormalizedPosition = 1f;

        switch (genero)
        {
            case "ciencia ficcion":
                textoCuento.text =
@"En el año 3025, las nubes ya no eran de vapor, sino de datos. Flotaban en el cielo, conteniendo miles de archivos, recuerdos y canciones que la humanidad había enviado allí para protegerlos del olvido.

Lina era una aprendiz de programadora en la estación espacial Órbita Azul. Le fascinaban los mensajes antiguos, esos que llegaban en forma de destellos de luz desde lo profundo del espacio. Una noche, mientras revisaba los radares, detectó un patrón extraño proveniente del cometa Eon-9: pulsos binarios, pausas y repeticiones… como si alguien tratara de hablar.

Durante semanas, Lina intentó descifrarlo. Pasaba las horas entre cables, pantallas y cafés sintéticos, convencida de que no era un error. Finalmente logró convertir las señales en sonidos: una secuencia de notas breves, casi como una melodía. Cuando reprodujo la secuencia completa, la pantalla parpadeó y una voz metálica emergió:

—“Gracias por escucharme. Soy el registro de la primera nave que soñó con volver a casa.”

Lina quedó en silencio. Aquella nave había desaparecido hacía siglos. Grabó la melodía y la subió a la nube global. Desde entonces, cada vez que Eon-9 cruza el cielo, las ciudades la reproducen a través de sus parlantes, recordando que incluso entre las estrellas, alguien siempre está intentando decirnos algo.";
                break;

            case "misterio":
                textoCuento.text =
@"La biblioteca de Villa Sombra tenía un rincón que olía a madera húmeda y papel antiguo. Nadie lo visitaba salvo Don Ernesto, su guardián de cabellos grises y mirada curiosa. Una noche, mientras revisaba los estantes, notó que un tomo de tapas azules estaba fuera de lugar. Lo colocó en su sitio, pero al día siguiente volvió a encontrarlo abierto, mostrando un párrafo distinto.

Intrigado, empezó a observarlo cada noche. Las letras parecían moverse solas, reacomodándose en silencio, como si un escritor invisible corrigiera su propia historia. A medianoche, Ernesto decidió esconderse detrás de una estantería.

Cuando el reloj sonó doce veces, el aire se volvió frío. El libro tembló y las palabras se reordenaron:

—“Hola, Ernesto. Gracias por leerme durante tantos años.”

El viejo bibliotecario se acercó con cuidado, temblando. Había pasado su vida cuidando libros, pero jamás uno le había hablado.

—“¿Quién eres?” —susurró.

—“Soy todas las historias que nunca terminaste.”

Ernesto sonrió con lágrimas en los ojos. Desde esa noche, cada lector que abre ese tomo encuentra en sus páginas una historia diferente, escrita a partir de los sueños que aún no se atreven a contar.";
                break;

            case "aventura":
                textoCuento.text =
@"Naira tenía doce años y una curiosidad que no cabía en su mochila. Durante un verano aburrido en casa de su abuela, encontró una caja vieja bajo el piso del desván. Dentro había una brújula de cobre, oxidada y cubierta de polvo. Pero su aguja no apuntaba al norte: giraba despacio, como si respirara.

Esa misma tarde la brújula comenzó a vibrar. Naira la sostuvo y la aguja se detuvo señalando hacia el bosque que rodeaba la aldea. Sin pensarlo dos veces, siguió la dirección. El sendero estaba cubierto de hojas, y a medida que avanzaba, el aire se volvía más cálido, más brillante.

En un claro, encontró un tronco hueco y, sobre él, un libro abierto. Las páginas giraban solas, mostrando ilustraciones que parecían moverse. Al tocar una de ellas, el dibujo se convirtió en una ráfaga de luz que la envolvió por completo.

Cuando volvió en sí, estaba en un mundo distinto: un valle con montañas flotantes, ríos de tinta y árboles que susurraban palabras. En el cielo, gigantescas letras se movían formando frases.

Naira entendió que el bosque era una biblioteca viva, y que cada paso que daba escribía una nueva página en su historia. Al regresar a casa, la brújula seguía girando, como si buscara su próximo cuento.

Desde entonces, cada vez que Naira abre un libro, siente que el bosque la observa… esperando a que vuelva a escribir su siguiente aventura.";
                break;
        }

        // Forzar layout para que ScrollRect actualice correctamente
        Canvas.ForceUpdateCanvases();
        StartCoroutine(ActivarVerificacionScroll());
    }

    private IEnumerator ActivarVerificacionScroll()
    {
        yield return null; // espera 1 frame
        puedeVerificarScroll = true;
    }

    private void Update()
    {
        if (puedeVerificarScroll && !botonMostrado && panelCuento.activeSelf)
        {
            if (scrollRect.verticalNormalizedPosition <= 0.05f)
            {
                botonMostrado = true;
                botonFinalizar.gameObject.SetActive(true);
            }
        }
    }

    private void FinalizarLectura()
    {
        panelCuento.SetActive(false);
        botonFinalizar.gameObject.SetActive(false);
        panelCuestionario.SetActive(true);

        MostrarPregunta(generoActual);
    }

    private void MostrarPregunta(string genero)
    {
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
        Button botonSeleccionado = null;
        switch (seleccionada)
        {
            case 0: botonSeleccionado = botonRespuesta1; break;
            case 1: botonSeleccionado = botonRespuesta2; break;
            case 2: botonSeleccionado = botonRespuesta3; break;
        }

        if (botonSeleccionado != null)
            StartCoroutine(PintarBotonYCerrarPanel(botonSeleccionado, seleccionada == correcta));
    }

    private IEnumerator PintarBotonYCerrarPanel(Button boton, bool correcto)
    {
        Color original = boton.image.color;
        boton.image.color = correcto ? Color.green : Color.red;

        yield return new WaitForSeconds(1f);

        boton.image.color = original;
        panelCuestionario.SetActive(false);
        botonAlimentar.interactable = true;

        if (correcto)
        {
            nivelActual++;
            textoNivel.text = nivelActual.ToString();

            if (flechaNivel != null)
                StartCoroutine(MostrarFlechaTemporaria());
        }
    }

    private IEnumerator MostrarFlechaTemporaria()
    {
        flechaNivel.SetActive(true);
        yield return new WaitForSeconds(duracionFlecha);
        flechaNivel.SetActive(false);
    }
}
