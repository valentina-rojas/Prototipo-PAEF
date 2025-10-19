using UnityEngine;

[System.Serializable]
public class Cuento
{
    public string genero;
    public string escenario;
    public string personaje;
    public string motivacion;
    public string extension;
    [TextArea(5, 20)]
    public string texto;
}

[CreateAssetMenu(fileName = "BaseDeCuentos", menuName = "Cuentos/Base de Cuentos")]
public class BaseDeCuentos : ScriptableObject
{
    public Cuento[] cuentos;
}
