using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CuentosManager : MonoBehaviour
{
    [Header("Texto y navegación")]
    [SerializeField] private TMP_Text textoCuento;
    [SerializeField] private Button botonSiguiente;
    [SerializeField] private Button botonAnterior;
    [SerializeField] private Button botonFinalizar;

    private List<string> paginas;
    private int paginaActual;
    private string textoCompletoActual = "";
    private string generoActual;
    private GameManager gameManager;

    private void Start()
    {
        botonSiguiente.onClick.AddListener(PaginaSiguiente);
        botonAnterior.onClick.AddListener(PaginaAnterior);
        botonFinalizar.onClick.AddListener(FinalizarLectura);
        botonFinalizar.gameObject.SetActive(false);
    }

    public void MostrarCuento(string genero, GameManager manager)
    {
        gameManager = manager;
        generoActual = genero;

        textoCompletoActual = ObtenerTextoPorGenero(genero);
        paginas = DividirEnPaginas(textoCompletoActual);
        paginaActual = 0;
        MostrarPagina();
    }

    private string ObtenerTextoPorGenero(string genero)
    {
        switch (genero)
        {
            case "ciencia ficcion":
                return @"En el año 3025, las nubes ya no eran de vapor, sino de datos. Flotaban en el cielo, conteniendo miles de archivos, recuerdos y canciones que la humanidad había enviado allí para protegerlos del olvido.
Lina era una aprendiz de programadora en la estación espacial Órbita Azul. Le fascinaban los mensajes antiguos, esos que llegaban en forma de destellos de luz desde lo profundo del espacio. Una noche, mientras revisaba los radares, detectó un patrón extraño proveniente del cometa Eon-9: pulsos binarios, pausas y repeticiones… como si alguien tratara de hablar.
Durante semanas, Lina intentó descifrarlo. Pasaba las horas entre cables, pantallas y cafés sintéticos, convencida de que no era un error. Finalmente logró convertir las señales en sonidos: una secuencia de notas breves, casi como una melodía. Cuando reprodujo la secuencia completa, la pantalla parpadeó y una voz metálica emergió:
—“Gracias por escucharme. Soy el registro de la primera nave que soñó con volver a casa.”
Lina quedó en silencio. Aquella nave había desaparecido hacía siglos. Grabó la melodía y la subió a la nube global. Desde entonces, cada vez que Eon-9 cruza el cielo, las ciudades la reproducen a través de sus parlantes, recordando que incluso entre las estrellas, alguien siempre está intentando decirnos algo.";
            case "misterio":
                return @"La biblioteca de Villa Sombra tenía un rincón que olía a madera húmeda y papel antiguo. Nadie lo visitaba salvo Don Ernesto, su guardián de cabellos grises y mirada curiosa. Una noche, mientras revisaba los estantes, notó que un tomo de tapas azules estaba fuera de lugar. Lo colocó en su sitio, pero al día siguiente volvió a encontrarlo abierto, mostrando un párrafo distinto.
Intrigado, empezó a observarlo cada noche. Las letras parecían moverse solas, reacomodándose en silencio, como si un escritor invisible corrigiera su propia historia. A medianoche, Ernesto decidió esconderse detrás de una estantería.
Cuando el reloj sonó doce veces, el aire se volvió frío. El libro tembló y las palabras se reordenaron:
—“Hola, Ernesto. Gracias por leerme durante tantos años.”
El viejo bibliotecario se acercó con cuidado, temblando. Había pasado su vida cuidando libros, pero jamás uno le había hablado.
—“¿Quién eres?” —susurró.
—“Soy todas las historias que nunca terminaste.”
Ernesto sonrió con lágrimas en los ojos. Desde esa noche, cada lector que abre ese tomo encuentra en sus páginas una historia diferente, escrita a partir de los sueños que aún no se atreven a contar.";
            case "aventura":
                return @"Naira tenía doce años y una curiosidad que no cabía en su mochila. Durante un verano aburrido en casa de su abuela, encontró una caja vieja bajo el piso del desván. Dentro había una brújula de cobre, oxidada y cubierta de polvo. Pero su aguja no apuntaba al norte: giraba despacio, como si respirara.
Esa misma tarde la brújula comenzó a vibrar. Naira la sostuvo y la aguja se detuvo señalando hacia el bosque que rodeaba la aldea. Sin pensarlo dos veces, siguió la dirección. El sendero estaba cubierto de hojas, y a medida que avanzaba, el aire se volvía más cálido, más brillante.
En un claro, encontró un tronco hueco y, sobre él, un libro abierto. Las páginas giraban solas, mostrando ilustraciones que parecían moverse. Al tocar una de ellas, el dibujo se convirtió en una ráfaga de luz que la envolvió por completo.
Cuando volvió en sí, estaba en un mundo distinto: un valle con montañas flotantes, ríos de tinta y árboles que susurraban palabras. En el cielo, gigantescas letras se movían formando frases.
Naira entendió que el bosque era una biblioteca viva, y que cada paso que daba escribía una nueva página en su historia. Al regresar a casa, la brújula seguía girando, como si buscara su próximo cuento.
Desde entonces, cada vez que Naira abre un libro, siente que el bosque la observa… esperando a que vuelva a escribir su siguiente aventura.";
            default:
                return "";
        }
    }

    private void MostrarPagina()
    {
        if (paginas == null || paginas.Count == 0)
        {
            textoCuento.text = "";
            botonAnterior.interactable = false;
            botonSiguiente.interactable = false;
            botonFinalizar.gameObject.SetActive(false);
            return;
        }

        textoCuento.text = paginas[paginaActual];
        botonAnterior.interactable = paginaActual > 0;
        botonSiguiente.interactable = paginaActual < paginas.Count - 1;
        botonFinalizar.gameObject.SetActive(paginaActual == paginas.Count - 1);
    }

    private void PaginaSiguiente()
    {
        if (paginaActual < paginas.Count - 1)
        {
            paginaActual++;
            MostrarPagina();
        }
    }

    private void PaginaAnterior()
    {
        if (paginaActual > 0)
        {
            paginaActual--;
            MostrarPagina();
        }
    }

    private void FinalizarLectura()
    {
        gameManager.FinalizarLectura(generoActual);
    }

    public void RecalcularPaginas()
    {
        if (string.IsNullOrEmpty(textoCompletoActual)) return;

        int paginaAntes = paginaActual;
        paginas = DividirEnPaginas(textoCompletoActual);
        paginaActual = Mathf.Clamp(paginaAntes, 0, paginas.Count - 1);
        MostrarPagina();
    }

    private List<string> DividirEnPaginas(string textoCompleto)
    {
        List<string> resultado = new List<string>();
        if (string.IsNullOrEmpty(textoCompleto)) return resultado;

        textoCuento.textWrappingMode = TextWrappingModes.Normal;

        RectTransform rect = textoCuento.GetComponent<RectTransform>();
        float alturaMaxima = rect.rect.height;
        float anchoMaximo = rect.rect.width;
        float margenSeguridad = textoCuento.fontSize * 0.5f;

        string[] palabras = textoCompleto.Split(' ');
        string acumulado = "";

        foreach (string palabra in palabras)
        {
            string prueba = string.IsNullOrEmpty(acumulado) ? palabra : acumulado + " " + palabra;
            Vector2 dims = textoCuento.GetPreferredValues(prueba, anchoMaximo, 0f);

            if (dims.y + margenSeguridad > alturaMaxima)
            {
                resultado.Add(acumulado.TrimEnd());
                acumulado = palabra;
            }
            else
            {
                acumulado = prueba;
            }
        }

        if (!string.IsNullOrEmpty(acumulado))
            resultado.Add(acumulado.TrimEnd());

        if (resultado.Count == 0)
            resultado.Add(textoCompleto);

        return resultado;
    }
}
