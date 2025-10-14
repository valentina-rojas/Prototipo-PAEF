using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private bool botonMostrado = false;

    private void Start()
    {
     
        panelGeneros.SetActive(false);
        panelCuento.SetActive(false);
        botonFinalizar.gameObject.SetActive(false);

       
        botonAlimentar.onClick.AddListener(MostrarPanelGeneros);
        botonCienciaFiccion.onClick.AddListener(() => MostrarCuento("ciencia ficcion"));
        botonMisterio.onClick.AddListener(() => MostrarCuento("misterio"));
        botonAventura.onClick.AddListener(() => MostrarCuento("aventura"));

        if (botonFinalizar != null)
            botonFinalizar.onClick.AddListener(FinalizarLectura);

        // Detectar scroll
        if (scrollRect != null)
            scrollRect.onValueChanged.AddListener(OnScrollChanged);
    }

    private void MostrarPanelGeneros()
    {
        botonAlimentar.interactable = false;
        panelGeneros.SetActive(true);
    }

    private void MostrarCuento(string genero)
    {
        panelGeneros.SetActive(false);
        panelCuento.SetActive(true);
        botonFinalizar.gameObject.SetActive(false);
        botonMostrado = false;
        scrollRect.verticalNormalizedPosition = 1f; 

        switch (genero)
        {
            case "ciencia ficcion":
                textoCuento.text =
                "En el año 3025, las nubes ya no eran de vapor, sino de datos. Flotaban en el cielo, conteniendo miles de archivos, recuerdos y canciones que la humanidad había enviado allí para protegerlos del olvido.\n\n" +
                "Lina era una aprendiz de programadora en la estación espacial Órbita Azul. Le fascinaban los mensajes antiguos, esos que llegaban en forma de destellos de luz desde lo profundo del espacio. Una noche, mientras revisaba los radares, detectó un patrón extraño proveniente del cometa Eon-9: pulsos binarios, pausas y repeticiones… como si alguien tratara de hablar.\n\n" +
                "Durante semanas, Lina intentó descifrarlo. Pasaba las horas entre cables, pantallas y cafés sintéticos, convencida de que no era un error. Finalmente logró convertir las señales en sonidos: una secuencia de notas breves, casi como una melodía. Cuando reprodujo la secuencia completa, la pantalla parpadeó y una voz metálica emergió:\n\n" +
                "—“Gracias por escucharme. Soy el registro de la primera nave que soñó con volver a casa.”\n\n" +
                "Lina quedó en silencio. Aquella nave había desaparecido hacía siglos. Grabó la melodía y la subió a la nube global. Desde entonces, cada vez que Eon-9 cruza el cielo, las ciudades la reproducen a través de sus parlantes, recordando que incluso entre las estrellas, alguien siempre está intentando decirnos algo.";
                break;

            case "misterio":
                textoCuento.text =
                "La biblioteca de Villa Sombra tenía un rincón que olía a madera húmeda y papel antiguo. Nadie lo visitaba salvo Don Ernesto, su guardián de cabellos grises y mirada curiosa. Una noche, mientras revisaba los estantes, notó que un tomo de tapas azules estaba fuera de lugar. Lo colocó en su sitio, pero al día siguiente volvió a encontrarlo abierto, mostrando un párrafo distinto.\n\n" +
                "Intrigado, empezó a observarlo cada noche. Las letras parecían moverse solas, reacomodándose en silencio, como si un escritor invisible corrigiera su propia historia. A medianoche, Ernesto decidió esconderse detrás de una estantería.\n\n" +
                "Cuando el reloj sonó doce veces, el aire se volvió frío. El libro tembló y las palabras se reordenaron:\n\n" +
                "—“Hola, Ernesto. Gracias por leerme durante tantos años.”\n\n" +
                "El viejo bibliotecario se acercó con cuidado, temblando. Había pasado su vida cuidando libros, pero jamás uno le había hablado.\n\n" +
                "—“¿Quién eres?” —susurró.\n\n" +
                "—“Soy todas las historias que nunca terminaste.”\n\n" +
                "Ernesto sonrió con lágrimas en los ojos. Desde esa noche, cada lector que abre ese tomo encuentra en sus páginas una historia diferente, escrita a partir de los sueños que aún no se atreven a contar.";
                break;

            case "aventura":
                textoCuento.text =
                "Naira tenía doce años y una curiosidad que no cabía en su mochila. Durante un verano aburrido en casa de su abuela, encontró una caja vieja bajo el piso del desván. Dentro había una brújula de cobre, oxidada y cubierta de polvo. Pero su aguja no apuntaba al norte: giraba despacio, como si respirara.\n\n" +
                "Esa misma tarde la brújula comenzó a vibrar. Naira la sostuvo y la aguja se detuvo señalando hacia el bosque que rodeaba la aldea. Sin pensarlo dos veces, siguió la dirección. El sendero estaba cubierto de hojas, y a medida que avanzaba, el aire se volvía más cálido, más brillante.\n\n" +
                "En un claro, encontró un tronco hueco y, sobre él, un libro abierto. Las páginas giraban solas, mostrando ilustraciones que parecían moverse. Al tocar una de ellas, el dibujo se convirtió en una ráfaga de luz que la envolvió por completo.\n\n" +
                "Cuando volvió en sí, estaba en un mundo distinto: un valle con montañas flotantes, ríos de tinta y árboles que susurraban palabras. En el cielo, gigantescas letras se movían formando frases.\n\n" +
                "Naira entendió que el bosque era una biblioteca viva, y que cada paso que daba escribía una nueva página en su historia. Al regresar a casa, la brújula seguía girando, como si buscara su próximo cuento.\n\n" +
                "Desde entonces, cada vez que Naira abre un libro, siente que el bosque la observa… esperando a que vuelva a escribir su siguiente aventura.";
                break;
        }
    }

    private void OnScrollChanged(Vector2 scrollPos)
    {
        // Cuando el scroll llega al fondo, mostrar el botón Finalizar
        if (!botonMostrado && scrollRect.verticalNormalizedPosition <= 0.05f)
        {
            botonMostrado = true;
            botonFinalizar.gameObject.SetActive(true);
        }
    }

    private void FinalizarLectura()
    {
        // Acción al finalizar el cuento 
        panelCuestionario.SetActive(true);

        panelCuento.SetActive(false);
        botonFinalizar.gameObject.SetActive(false);

    }
}
