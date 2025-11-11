using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class Mision
{
    public string descripcion;
    public MisionTipo tipo;
    public string parametro;
    public bool completada;
    public bool reclamada;
    public int recompensaEXP;
    public string id; 
}

public enum MisionTipo
{
    LeerCuento,
    LeerCuentoGenero
}

public class MisionesManager : MonoBehaviour
{
    [Header("UI Panel y Prefab")]
    [SerializeField] private GameObject panelMisiones;
    [SerializeField] private GameObject prefabMision;
    [SerializeField] private Button botonAbrirMisiones;
    [SerializeField] private Button botonCerrarMisiones;
    [SerializeField] private Transform contenedorMisiones;

    [Header("Referencia GameManager")]
    [SerializeField] private GameManager gameManager;

    private List<GameObject> misionesUI = new List<GameObject>();
    private List<Mision> todasLasMisiones = new List<Mision>();
    private List<Mision> misionesActivas = new List<Mision>();

    private void Start()
    {
        if (panelMisiones != null)
            panelMisiones.SetActive(false);

        botonAbrirMisiones?.onClick.AddListener(TogglePanel);
        botonCerrarMisiones?.onClick.AddListener(CerrarPanel);

        CrearTodasLasMisiones();
        GenerarMisionesActivas();
        MostrarMisiones(misionesActivas);
    }

    private void TogglePanel() => panelMisiones.SetActive(!panelMisiones.activeSelf);
    private void CerrarPanel() => panelMisiones.SetActive(false);

    private void CrearTodasLasMisiones()
    {
        todasLasMisiones = new List<Mision>
        {
            new Mision { 
                id = "leer_cualquier_cuento",
                descripcion = "Lee un cuento", 
                tipo = MisionTipo.LeerCuento, 
                recompensaEXP = 5 
            },
            new Mision { 
                id = "leer_ciencia_ficcion",
                descripcion = "Lee un cuento de Ciencia Ficción", 
                tipo = MisionTipo.LeerCuentoGenero, 
                parametro = "Ciencia Ficción", 
                recompensaEXP = 10 
            },
            new Mision { 
                id = "leer_aventura",
                descripcion = "Lee un cuento de Aventura", 
                tipo = MisionTipo.LeerCuentoGenero, 
                parametro = "Aventura", 
                recompensaEXP = 10 
            },
            new Mision { 
                id = "leer_misterio", 
                descripcion = "Lee un cuento de Misterio", 
                tipo = MisionTipo.LeerCuentoGenero, 
                parametro = "Misterio", 
                recompensaEXP = 10 
            }
        };
    }

    private void GenerarMisionesActivas()
    {
        misionesActivas.Clear();
        List<Mision> disponibles = todasLasMisiones.Where(m => !m.completada).ToList();
        
        while (misionesActivas.Count < 3 && disponibles.Count > 0)
        {
            int index = Random.Range(0, disponibles.Count);
            misionesActivas.Add(disponibles[index]);
            disponibles.RemoveAt(index);
        }
    }

    public void MostrarMisiones(List<Mision> misiones)
    {
        foreach (var obj in misionesUI)
            Destroy(obj);
        misionesUI.Clear();

        for (int i = 0; i < misiones.Count; i++)
        {
            Mision misionActual = misiones[i];
            
            GameObject go = Instantiate(prefabMision, contenedorMisiones);
            MisionUI ui = go.GetComponent<MisionUI>();

            if (ui == null)
            {
                Debug.LogError("El prefab de misión no tiene asignado el script MisionUI.");
                continue;
            }

            ui.textoDescripcion.text = misionActual.descripcion;
            ui.textoDescripcion.color = misionActual.completada ? Color.green : Color.white;
            ui.boton.onClick.RemoveAllListeners();
            
            Mision misionParaListener = misionActual;

            if (!misionActual.completada)
            {
                ui.textoBoton.text = "Ir a la misión";
                ui.boton.interactable = true;
                ui.boton.onClick.AddListener(() => OnBotonMisionClick(misionParaListener));
            }
            else if (misionActual.completada && !misionActual.reclamada)
            {
                ui.textoBoton.text = "Reclamar";
                ui.boton.interactable = true;
                ui.boton.onClick.AddListener(() => OnBotonReclamarClick(misionParaListener));
            }
            else
            {
                ui.textoBoton.text = "Completada";
                ui.boton.interactable = false;
            }

            misionesUI.Add(go);
        }
    }

    private void OnBotonMisionClick(Mision mision)
    {
        Debug.Log($"Botón clickeado para misión: {mision.descripcion}");
        CerrarPanel();

        if (mision.tipo == MisionTipo.LeerCuento || mision.tipo == MisionTipo.LeerCuentoGenero)
        {
            gameManager.MostrarPanelGeneros();
        }
    }

    private void OnBotonReclamarClick(Mision mision)
    {
        Debug.Log($"Reclamando recompensa para: {mision.descripcion}");
        ReclamarRecompensa(mision);
    }

    private void ReclamarRecompensa(Mision mision)
    {
        Mision misionCompleta = todasLasMisiones.FirstOrDefault(m => m.id == mision.id);
        if (misionCompleta != null)
        {
            misionCompleta.reclamada = true;
        }

        Mision misionActiva = misionesActivas.FirstOrDefault(m => m.id == mision.id);
        if (misionActiva != null)
        {
            misionActiva.reclamada = true;
        }

        gameManager.GanarExperiencia(mision.recompensaEXP);
        Debug.Log($"Recompensa reclamada: {mision.recompensaEXP} EXP");

        bool todasReclamadas = misionesActivas.All(x => x.completada && x.reclamada);
        Debug.Log($"Todas las misiones reclamadas: {todasReclamadas}");

        if (todasReclamadas)
        {
            Debug.Log("Generando nuevas misiones activas...");
            GenerarMisionesActivas();
        }

        MostrarMisiones(misionesActivas);
    }

    public void CompletarMision(MisionTipo tipo, string parametro = "")
    {
        Debug.Log($"Intentando completar misión: {tipo}, parámetro: {parametro}");

        bool huboCambio = false;

        foreach (var m in todasLasMisiones)
        {
            if (!m.completada && m.tipo == tipo)
            {
                if (tipo == MisionTipo.LeerCuento)
                {
                    m.completada = true;
                    huboCambio = true;
                    Debug.Log($"Misión completada: {m.descripcion}");
                }
                else if (tipo == MisionTipo.LeerCuentoGenero && Normalizar(m.parametro) == Normalizar(parametro))
                {
                    m.completada = true;
                    huboCambio = true;
                    Debug.Log($"Misión completada: {m.descripcion}");
                }
            }
        }

        foreach (var m in misionesActivas)
        {
            if (!m.completada && m.tipo == tipo)
            {
                if (tipo == MisionTipo.LeerCuento)
                {
                    m.completada = true;
                    huboCambio = true;
                    Debug.Log($"Misión activa completada: {m.descripcion}");
                }
                else if (tipo == MisionTipo.LeerCuentoGenero && Normalizar(m.parametro) == Normalizar(parametro))
                {
                    m.completada = true;
                    huboCambio = true;
                    Debug.Log($"Misión activa completada: {m.descripcion}");
                }
            }
        }

        if (huboCambio)
        {
            Debug.Log("Hubo cambios, actualizando UI de misiones");
            MostrarMisiones(misionesActivas);
        }
    }

    private string Normalizar(string texto)
    {
        if (string.IsNullOrEmpty(texto)) return "";
        string normalizado = texto.ToLowerInvariant()
            .Replace("á", "a")
            .Replace("é", "e")
            .Replace("í", "i")
            .Replace("ó", "o")
            .Replace("ú", "u")
            .Trim();
        return normalizado;
    }

    public void DebugMisiones()
    {
        Debug.Log("=== DEBUG MISIONES ===");
        Debug.Log($"Total misiones: {todasLasMisiones.Count}");
        Debug.Log($"Misiones activas: {misionesActivas.Count}");
        
        foreach (var m in misionesActivas)
        {
            Debug.Log($"Misión: {m.descripcion} - Completada: {m.completada} - Reclamada: {m.reclamada}");
        }
    }
}