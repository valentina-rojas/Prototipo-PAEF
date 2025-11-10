using UnityEngine;

public class ForceIncludeAssets : MonoBehaviour
{
    [SerializeField] private BaseDeCuentos baseDeCuentos;
    
    private void Awake()
    {
        if (baseDeCuentos != null)
        {
            Debug.Log($"Forzando inclusión de: {baseDeCuentos.name}");
        }
    }
}