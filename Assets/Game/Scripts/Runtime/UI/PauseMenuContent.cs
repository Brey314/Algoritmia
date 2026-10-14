using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// Los textos que el menú de pausa añadió con «Volver a ver la historia» y «Guardar y salir»
    /// (INC-158, INC-159): fuera del código, como todo texto visible (CT-05, RNF-18). Los rótulos
    /// de «Reanudar», «Reiniciar» y «Volver al menú de niveles» siguen en el prefab
    /// <c>MenuPausa</c>, donde ya estaban.
    /// </summary>
    [CreateAssetMenu(menuName = "Game/UI/Pause Menu Content", fileName = "PauseMenuContent")]
    public class PauseMenuContent : ScriptableObject
    {
        [field: SerializeField]
        [field: Tooltip("Rótulo del botón que lleva a la escena narrativa de la fase en curso.")]
        public string HistoryLabel { get; private set; } = "Volver a ver la historia";

        [field: SerializeField]
        [field: Tooltip("Rótulo del botón que abre la confirmación de salida del juego.")]
        public string SaveQuitLabel { get; private set; } = "Guardar y salir";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Pregunta de la confirmación de salida. Dice que lo terminado queda guardado: no es una defensa contra pérdida de datos, el avance se guarda al confirmar cada fase (RF-41).")]
        public string QuitPrompt { get; private set; } = "¿Quieres salir del juego? Lo que ya terminaste queda guardado.";

        [field: SerializeField]
        [field: Tooltip("Botón que confirma la salida (el ámbar, como «Sí, reiniciar»).")]
        public string ConfirmQuitLabel { get; private set; } = "Sí, salir";

        [field: SerializeField]
        [field: Tooltip("Botón que cierra la confirmación sin salir.")]
        public string CancelQuitLabel { get; private set; } = "Cancelar";
    }
}
