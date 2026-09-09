using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// Título del producto para la pantalla de inicio (RF-01). Vive en un asset porque PG-01
    /// sigue abierto: cambiar el título es editar este asset, no recompilar (RNF-18).
    /// </summary>
    [CreateAssetMenu(menuName = "Game/UI/Game Title Config", fileName = "GameTitleConfig")]
    public class GameTitleConfig : ScriptableObject
    {
        [field: SerializeField, Tooltip("Título que se muestra en la pantalla de inicio.")]
        public string Title { get; private set; } = "Algoritmia";
    }
}
