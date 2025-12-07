using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class BotonAnimado : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float escalaPresionado = 0.9f;
    public float velocidad = 10f;

    private Vector3 escalaOriginal;

    private void Awake()
    {
        // Buscar todos los botones en la escena y agregar este componente si no lo tienen
        Button[] botones = FindObjectsOfType<Button>(true);

        foreach (Button b in botones)
        {
            if (b.GetComponent<BotonAnimado>() == null)
            {
                b.gameObject.AddComponent<BotonAnimado>();
            }
        }
    }

    private void Start()
    {
        escalaOriginal = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(Escalar(transform, escalaOriginal * escalaPresionado));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        AudioManager.instance.sonidoPresionarBoton.Play();
        StopAllCoroutines();
        StartCoroutine(Escalar(transform, escalaOriginal));
    }

    private IEnumerator Escalar(Transform obj, Vector3 target)
    {
        while (Vector3.Distance(obj.localScale, target) > 0.001f)
        {
            obj.localScale = Vector3.Lerp(obj.localScale, target, Time.deltaTime * velocidad);
            yield return null;
        }
    }
}
