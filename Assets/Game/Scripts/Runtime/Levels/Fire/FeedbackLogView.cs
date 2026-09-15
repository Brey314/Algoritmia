using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Levels.Fire
{
    /// <summary>
    /// La tablilla de mensajes del Nivel 1: muestra **la última** línea del registro (RF-16).
    /// </summary>
    /// <remarks>
    /// Hasta la Fase 5 era un área de registro con scroll y todo el historial (HU-06). Santiago la
    /// retiró el 12/09/2026 («no estaba en el mockup, estorba»): el historial sigue en
    /// <see cref="FireFeedbackLog.Entries"/> —lo lee el informe docente si hace falta—, pero el
    /// estudiante ve solo el mensaje más reciente, en la tablilla superior del mockup 7.
    /// </remarks>
    public class FeedbackLogView : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Etiqueta de la tablilla donde se escribe el último mensaje.")]
        private Text lines;

        public void Show(IReadOnlyList<string> entries)
        {
            if (entries.Count > 0)
            {
                lines.text = entries[^1];
            }
        }
    }
}
