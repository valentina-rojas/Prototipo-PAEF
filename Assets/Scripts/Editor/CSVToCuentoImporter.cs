using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

public class CSVToCuentoImporter : EditorWindow
{
    private TextAsset csvFile;
    private string outputPath = "Assets/Scripts/Cuentos";

    [MenuItem("Tools/CSV to Cuento Importer")]
    public static void ShowWindow()
    {
        GetWindow<CSVToCuentoImporter>("CSV Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Importar CSV a BaseDeCuentos", EditorStyles.boldLabel);
        csvFile = (TextAsset)EditorGUILayout.ObjectField("Archivo CSV", csvFile, typeof(TextAsset), false);

        if (GUILayout.Button("Importar CSV"))
        {
            if (csvFile != null)
                ImportCSV();
            else
                EditorUtility.DisplayDialog("Error", "Selecciona un archivo CSV", "OK");
        }
    }

    private void ImportCSV()
    {
        string cleanedText = Regex.Replace(csvFile.text, @"\r\n|\r", "\n");
        Queue<string> linesQueue = new Queue<string>(cleanedText.Split('\n'));

        if (linesQueue.Count < 2)
        {
            EditorUtility.DisplayDialog("Error", "El archivo CSV no tiene suficientes líneas", "OK");
            return;
        }

        linesQueue.Dequeue(); // saltar encabezado

        List<Cuento> cuentosList = new List<Cuento>();

        while (linesQueue.Count > 0)
        {
            string[] values = ParseCSVLineRobust(ref linesQueue);

            if (values == null || values.Length < 6) continue;
            if (string.IsNullOrEmpty(values[0]) || string.IsNullOrEmpty(values[5]) || values[5].Length < 10)
                continue;

            cuentosList.Add(ParseCuentoFromValues(values));
        }

        if (cuentosList.Count > 0)
            CreateBaseDeCuentos(cuentosList.ToArray());
        else
            EditorUtility.DisplayDialog("Error", "No se encontraron cuentos válidos.", "OK");
    }

    private string CleanField(string field)
    {
        if (string.IsNullOrEmpty(field)) return "";
        string cleaned = field.Trim();
        if (cleaned.StartsWith("\"") && cleaned.EndsWith("\""))
            cleaned = cleaned.Substring(1, cleaned.Length - 2);
        cleaned = cleaned.Replace("\"\"", "\"");
        return cleaned.Trim();
    }

    private string[] ParseCSVLineRobust(ref Queue<string> linesQueue)
    {
        if (linesQueue.Count == 0) return null;

        StringBuilder currentField = new StringBuilder();
        List<string> fields = new List<string>();
        bool inQuotes = false;

        while (linesQueue.Count > 0)
        {
            string line = linesQueue.Dequeue();
            int i = 0;

            while (i < line.Length)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        currentField.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if ((c == '\t' || c == ',') && !inQuotes)
                {
                    fields.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }

                i++;
            }

            if (inQuotes) currentField.Append("\n");
            else break;
        }

        fields.Add(currentField.ToString());
        return fields.ToArray();
    }

    private Cuento ParseCuentoFromValues(string[] values)
    {
        Cuento cuento = new Cuento
        {
            genero = CleanField(values[0]),
            escenario = CleanField(values[1]),
            personaje = CleanField(values[2]),
            motivacion = CleanField(values[3]),
            extension = CleanField(values[4]),
            texto = CleanField(values[5]),
            cuestionario = null,
            fraseIncompleta = null,
            ordenarFrases = null
        };

        // --- Cuestionario ---
        if (values.Length >= 9 && !string.IsNullOrEmpty(values[6]))
        {
            string preguntaTexto = CleanField(values[6]);
            string opcionesRaw = CleanField(values[7]);
            string respuestaRaw = CleanField(values[8]);

            string[] opciones = opcionesRaw.Split(';');
            int respuestaCorrecta = 0;
            int.TryParse(respuestaRaw.Replace("Opción", "").Trim(), out respuestaCorrecta);
            respuestaCorrecta = Mathf.Clamp(respuestaCorrecta - 1, 0, opciones.Length - 1);

            cuento.cuestionario = new Pregunta[]
            {
                new Pregunta
                {
                    texto = preguntaTexto,
                    opciones = opciones,
                    respuestaCorrecta = respuestaCorrecta
                }
            };
        }

        // --- Frase incompleta ---
        if (values.Length >= 12 && !string.IsNullOrEmpty(values[9]))
        {
            string frase = CleanField(values[9]);
            string opcionesRaw = CleanField(values[10]);
            string respuesta = CleanField(values[11]);

            cuento.fraseIncompleta = new FraseIncompleta[]
            {
                new FraseIncompleta
                {
                    frase = frase,
                    opciones = opcionesRaw.Split(';').Select(o => o.Trim()).ToArray(),
                    respuestaCorrecta = respuesta
                }
            };
        }

        // --- Ordenar frases ---
        if (values.Length >= 13 && !string.IsNullOrEmpty(values[12]))
        {
            string ordenarRaw = CleanField(values[12]);
            cuento.ordenarFrases = new OrdenarFrases[]
            {
                new OrdenarFrases
                {
                    frasesCorrectas = ordenarRaw.Split(';').Select(f => f.Trim()).ToArray()
                }
            };
        }

        return cuento;
    }

    private void CreateBaseDeCuentos(Cuento[] cuentos)
    {
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        string assetPath = $"{outputPath}/BaseDeCuentos_AutoGenerated.asset";

        if (File.Exists(assetPath))
            AssetDatabase.DeleteAsset(assetPath);

        BaseDeCuentos baseDeCuentos = ScriptableObject.CreateInstance<BaseDeCuentos>();
        baseDeCuentos.cuentos = cuentos;

        AssetDatabase.CreateAsset(baseDeCuentos, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // --- Exportar a JSON en StreamingAssets ---
        string streamingPath = Path.Combine(Application.dataPath, "StreamingAssets");
        if (!Directory.Exists(streamingPath))
            Directory.CreateDirectory(streamingPath);

        string jsonPath = Path.Combine(streamingPath, "baseDeCuentos.json");
        string json = JsonUtility.ToJson(new BaseDeCuentosWrapper { cuentos = cuentos }, true);
        File.WriteAllText(jsonPath, json, System.Text.Encoding.UTF8);

        Debug.Log($"Base exportada: {assetPath}");
        Debug.Log($"JSON generado en: {jsonPath}");

        EditorUtility.DisplayDialog("Éxito",
            $"Base de cuentos creada ({cuentos.Length} cuentos)\nSe guardó como asset y JSON.",
            "OK");

        Selection.activeObject = baseDeCuentos;
    }

    [System.Serializable]
    private class BaseDeCuentosWrapper
    {
        public Cuento[] cuentos;
    }
}