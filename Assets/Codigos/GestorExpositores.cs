using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class Expositor
{
    public string num_iden;
    public string nom_catalogo;
    public string pabellon;
    public int nivel;
    public string stand;
}

public class GestorExpositores : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField campoBusqueda; // Campo de texto donde escribe el usuario
    public TMP_Text textoResultado;      // Texto donde se muestran los resultados

    [Header("Datos")]
    public string archivoJson = "expositores.json";

    private List<Expositor> expositores;

    void Start()
    {
        // Cargar JSON desde StreamingAssets
        string ruta = Path.Combine(Application.streamingAssetsPath, archivoJson);

        if (File.Exists(ruta))
        {
            string json = File.ReadAllText(ruta);
            expositores = JsonHelper.FromJson<Expositor>(json);
        }
        else
        {
            Debug.LogError("No se encontró el archivo JSON en: " + ruta);
        }

        // Escucha cada vez que se escribe en el InputField
        campoBusqueda.onValueChanged.AddListener(FiltrarResultados);
    }

    void FiltrarResultados(string texto)
    {
        if (string.IsNullOrEmpty(texto))
        {
            textoResultado.text = "✏️ Escribe un nombre o stand...";
            return;
        }

        // Buscar por nombre o por stand
        Expositor encontrado = expositores.Find(e =>
            e.nom_catalogo.ToLower().Contains(texto.ToLower()) ||
            e.stand.ToLower().Contains(texto.ToLower())
        );

        if (encontrado != null)
        {
            textoResultado.text =
                $"✅ {encontrado.nom_catalogo}\n" +
                $"📍 Pabellón: {encontrado.pabellon}\n" +
                $"🏢 Nivel: {encontrado.nivel}\n" +
                $"🔢 Stand: {encontrado.stand}";
        }
        else
        {
            textoResultado.text = "❌ No se encontró ningún expositor.";
        }
    }
}

public static class JsonHelper
{
    public static List<T> FromJson<T>(string json)
    {
        return JsonUtility.FromJson<Wrapper<T>>(WrapArray(json)).Items;
    }

    private static string WrapArray(string json)
    {
        return "{\"Items\":" + json + "}";
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> Items;
    }
}
