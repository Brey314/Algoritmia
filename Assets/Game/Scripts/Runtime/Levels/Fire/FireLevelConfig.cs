using UnityEngine;

namespace Game.Levels.Fire
{
    /// <summary>
    /// Los cuatro parámetros ajustables jugando del Nivel 1 (guion §4.3.2). Viven en un asset y no
    /// en el código para que retocarlos —PG-06 sigue abierto— no cueste una recompilación
    /// (CT-05, RNF-18). Los valores por defecto son los propuestos por el guion.
    /// </summary>
    [CreateAssetMenu(menuName = "Algoritm/Configuración del nivel fuego", fileName = "N1_Config")]
    public class FireLevelConfig : ScriptableObject
    {
        [field: SerializeField]
        [field: Tooltip("Distancias disponibles en el control deslizante.")]
        public int AvailablePositions { get; set; } = 3;

        [field: SerializeField]
        [field: Tooltip("Única posición desde la que un golpe cuenta como efectivo.")]
        public StrikePosition EffectivePosition { get; set; } = StrikePosition.VeryClose;

        [field: SerializeField]
        [field: Tooltip("Golpes efectivos necesarios para habilitar el soplo.")]
        public int MinimumEffectiveStrikes { get; set; } = 3;

        [field: SerializeField]
        [field: Tooltip("Fallos consecutivos tras los cuales el guía ofrece una pista (RF-13).")]
        public int AttemptsBeforeHint { get; set; } = 3;
    }
}
