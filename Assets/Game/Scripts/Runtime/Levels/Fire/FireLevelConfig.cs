using UnityEngine;

namespace Game.Levels.Fire
{
    /// <summary>
    /// Los parámetros ajustables jugando del Nivel 1 (guion §4.3.2, reescrito en la Fase 5 con
    /// fuerza en vez de distancia). Viven en un asset y no en el código para que retocarlos no
    /// cueste una recompilación (CT-05, RNF-18). Los valores por defecto son los acordados con
    /// Santiago el 12/09/2026: diez muescas, efectivas la siete y la ocho.
    /// </summary>
    [CreateAssetMenu(menuName = "Algoritm/Configuración del nivel fuego", fileName = "N1_Config")]
    public class FireLevelConfig : ScriptableObject
    {
        [field: SerializeField]
        [field: Tooltip("Muesca máxima del deslizante de fuerza (va de 0 a este valor).")]
        public int ForceLevels { get; set; } = 10;

        [field: SerializeField]
        [field: Tooltip("Fuerza mínima con la que un golpe cuenta como efectivo.")]
        public int EffectiveForceMin { get; set; } = 7;

        [field: SerializeField]
        [field: Tooltip("Fuerza máxima con la que un golpe cuenta como efectivo.")]
        public int EffectiveForceMax { get; set; } = 8;

        [field: SerializeField]
        [field: Tooltip("Radio del montón, como fracción del alto del suelo: todas las hojas deben caer dentro para que haya montón (T22).")]
        public float PileRadius { get; set; } = 0.10f;

        [field: SerializeField]
        [field: Tooltip("Radio de las piedras, como fracción del alto del suelo: el sílex y el pedernal deben caer dentro para que un golpe pueda prender (T22).")]
        public float StonesRadius { get; set; } = 0.20f;

        [field: SerializeField]
        [field: Tooltip("Golpes efectivos necesarios para habilitar el soplo.")]
        public int MinimumEffectiveStrikes { get; set; } = 3;

        [field: SerializeField]
        [field: Tooltip("Fallos consecutivos tras los cuales el guía ofrece una pista (RF-13).")]
        public int AttemptsBeforeHint { get; set; } = 3;
    }
}
