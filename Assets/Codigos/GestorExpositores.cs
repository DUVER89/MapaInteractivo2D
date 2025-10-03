using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using System.Linq; // 👈 necesario para GroupBy / Distinct

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

                campoBusqueda.onValueChanged.AddListener(FiltrarResultados);
                textoResultado.text = "✏️ Escribe un nombre o un stand (mín. 3 caracteres)...";
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
            textoResultado.text = "✏️ Escribe un nombre o un stand (mín. 3 caracteres)...";
            return;
        }

        // 📌 Filtro: mínimo 3 caracteres
        if (texto.Length < 3)
        {
            textoResultado.text = "⌛ Escribe al menos 3 caracteres para buscar...";
            return;
        }

        List<Expositor> encontrados;

        // 🔢 Si el input es solo números → buscar por Stand
        if (texto.All(char.IsDigit))
        {
            encontrados = expositores.FindAll(e =>
                !string.IsNullOrEmpty(e.stand) && e.stand.ToLower().Contains(texto.ToLower())
            );
        }
        else
        {
            // 🔤 Si contiene letras → buscar por nombre (nom_catalogo)
            encontrados = expositores.FindAll(e =>
                !string.IsNullOrEmpty(e.nom_catalogo) && e.nom_catalogo.ToLower().Contains(texto.ToLower())
            );
        }

        // ✅ Eliminar duplicados por ID
        encontrados = encontrados
            .GroupBy(e => e.num_iden)
            .Select(g => g.First())
            .ToList();

        if (encontrados.Count > 0)
        {
            // Ordenar por nombre alfabéticamente
            encontrados.Sort((a, b) => string.Compare(a.nom_catalogo, b.nom_catalogo, true));

            // Construir texto con resultados
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (Expositor e in encontrados)
            {
                sb.AppendLine(
                    $"<b>{e.nom_catalogo}</b>\n" +
                    $"• ID: {e.num_iden}\n" +
                    $"• Pabellón: {e.pabellon}\n" +
                    $"• Nivel: {e.nivel}\n" +
                    $"• Stand: {e.stand}\n"
                );
            }

            textoResultado.text = sb.ToString();
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
