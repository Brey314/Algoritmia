using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// Título del producto para la pantalla de inicio (RF-01): «Algoritmia», PG-01 cerrado. Vive
    /// en un asset porque todo texto visible vive fuera del código (CT-05, RNF-18).
    /// </summary>
    [CreateAssetMenu(menuName = "Game/UI/Game Title Config", fileName = "GameTitleConfig")]
    public class GameTitleConfig : ScriptableObject
    {
        [field: SerializeField, Tooltip("Título que se muestra en la pantalla de inicio.")]
        public string Title { get; private set; } = "Algoritmia";

        [field: SerializeField, Tooltip("Aviso para el docente al salir cuando la carpeta Datos " +
            "no admite escritura; debajo se muestra la carpeta real (HU-18 FA-04).")]
        public string FallbackSaveNotice { get; private set; } =
            "Este equipo no deja guardar en la carpeta Datos del juego. El progreso quedó guardado en:";
    }
}
