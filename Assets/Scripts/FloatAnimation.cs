using UnityEngine;

public class FloatAnimation : MonoBehaviour
{
    [Header("Configuración de flotación")]
    public float floatSpeed = 2f;    
    public float floatAmount = 0.2f; 

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position; 
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.position = startPosition + new Vector3(0, yOffset, 0);
    }
}
