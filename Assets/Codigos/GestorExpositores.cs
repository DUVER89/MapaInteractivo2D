using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;

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
    public TMP_InputField campoBusqueda;
    public TMP_Text textoResultado;

    [Header("Datos")]
    public string archivoJson = "expositores.json";

    private List<Expositor> expositores;

    void Start()
    {
        if (campoBusqueda == null)
        {
            Debug.LogError("❌ No asignaste el TMP_InputField (campoBusqueda) en el Inspector.");
            return;
        }

        if (textoResultado == null)
        {
            Debug.LogError("❌ No asignaste el TMP_Text (textoResultado) en el Inspector.");
            return;
        }

        // Iniciar carga del JSON de forma compatible con Android/PC
        StartCoroutine(CargarExpositores());
        textoResultado.text = "✏️ Cargando datos...";
    }

    IEnumerator CargarExpositores()
    {
        string ruta = System.IO.Path.Combine(Application.streamingAssetsPath, archivoJson);

        UnityWebRequest request = UnityWebRequest.Get(ruta);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Error al cargar el JSON: " + request.error);
            textoResultado.text = "⚠️ No se cargaron datos de expositores.";
            expositores = new List<Expositor>();
        }
        else
        {
            string json = request.downloadHandler.text;
            try
            {
                expositores = JsonHelper.FromJson<Expositor>(json);
                Debug.Log("✅ Expositores cargados: " + expositores.Count);

                // Ya podemos escuchar cambios en el input
                campoBusqueda.onValueChanged.AddListener(FiltrarResultados);
                textoResultado.text = "✏️ Escribe un nombre o stand...";
            }
            catch (System.Exception e)
            {
                Debug.LogError("⚠️ Error al parsear el JSON: " + e.Message);
                textoResultado.text = "⚠️ Error en el formato del archivo JSON.";
                expositores = new List<Expositor>();
            }
        }
    }

    void FiltrarResultados(string texto)
    {
        if (expositores == null || expositores.Count == 0)
        {
            textoResultado.text = "⚠️ No se cargaron datos de expositores.";
            return;
        }

        if (string.IsNullOrEmpty(texto))
        {
            textoResultado.text = "✏️ Escribe un nombre o stand...";
            return;
        }

        Expositor encontrado = expositores.Find(e =>
            (!string.IsNullOrEmpty(e.nom_catalogo) && e.nom_catalogo.ToLower().Contains(texto.ToLower())) ||
            (!string.IsNullOrEmpty(e.stand) && e.stand.ToLower().Contains(texto.ToLower())) ||
            (!string.IsNullOrEmpty(e.num_iden) && e.num_iden.ToLower().Contains(texto.ToLower()))
        );

        if (encontrado != null)
        {
            textoResultado.text =
                $"✅ {encontrado.nom_catalogo}\n" +
                $"🆔 ID: {encontrado.num_iden}\n" +
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

