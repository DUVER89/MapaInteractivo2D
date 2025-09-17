using UnityEngine;
using UnityEngine.UI;

public class BotonesConAudio : MonoBehaviour
{
    public Button[] botones;          // Array de botones (arrastrar desde el Inspector)
    public AudioSource miAudioSource; // Arrastrar aquí el AudioSource
    public AudioClip sonidoClick;     // Arrastrar aquí el audio del click

    void Start()
    {
        // Recorremos todos los botones y les asignamos el evento
        foreach (Button b in botones)
        {
            if (b != null)
                b.onClick.AddListener(ReproducirSonido);
        }
    }

    void ReproducirSonido()
    {
        if (miAudioSource != null && sonidoClick != null)
        {
            miAudioSource.PlayOneShot(sonidoClick);
        }
    }
}
