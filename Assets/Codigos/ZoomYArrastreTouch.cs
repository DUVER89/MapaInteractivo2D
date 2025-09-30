using UnityEngine;
using System.Collections;

public class ZoomYArrastreImagen : MonoBehaviour
{
    [Header("Imagen a manipular")]
    public RectTransform imagen;

    [Header("Opciones de Zoom")]
    public float zoomSpeed = 0.005f;
    public float zoomMin = 0.5f;
    public float zoomMax = 3f;

    [Header("Opciones de Reset")]
    public float tiempoReset = 5f; // segundos para volver al estado inicial

    private float escala = 1f;
    private Vector3 escalaOriginal;
    private Vector2 posicionOriginal;
    private Coroutine resetCoroutine;

    void Start()
    {
        if (imagen != null)
        {
            escalaOriginal = imagen.localScale;
            posicionOriginal = imagen.anchoredPosition;
        }
    }

    void Update()
    {
        if (imagen == null) return;

        // 📌 Arrastre con un dedo
        if (Input.touchCount == 1)
        {
            Touch toque = Input.GetTouch(0);

            if (toque.phase == TouchPhase.Moved)
            {
                imagen.anchoredPosition += toque.deltaPosition;
                ReiniciarTemporizador();
            }
        }

        // 📌 Zoom con dos dedos
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0Prev = touch0.position - touch0.deltaPosition;
            Vector2 touch1Prev = touch1.position - touch1.deltaPosition;

            float prevMag = (touch0Prev - touch1Prev).magnitude;
            float currMag = (touch0.position - touch1.position).magnitude;

            float diferencia = currMag - prevMag;

            escala += diferencia * zoomSpeed;
            escala = Mathf.Clamp(escala, zoomMin, zoomMax);

            imagen.localScale = Vector3.one * escala;

            ReiniciarTemporizador();
        }
    }

    // 🔄 Restablecer al estado original
    private void ResetearImagen()
    {
        if (imagen != null)
        {
            imagen.localScale = escalaOriginal;
            imagen.anchoredPosition = posicionOriginal;
            escala = 1f;
            Debug.Log("✅ Imagen restablecida al estado original");
        }
    }

    // 🕒 Reinicia el temporizador cada vez que hay interacción
    private void ReiniciarTemporizador()
    {
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }
        resetCoroutine = StartCoroutine(TemporizadorReset());
    }

    private IEnumerator TemporizadorReset()
    {
        yield return new WaitForSeconds(tiempoReset);
        ResetearImagen();
    }
}


