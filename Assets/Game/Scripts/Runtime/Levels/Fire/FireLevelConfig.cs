using UnityEngine;

namespace Game.Levels.Fire
{
    /// <summary>
    /// Los parámetros ajustables jugando del Nivel 1 (guion §4.3.2, reescrito en la Fase 5 con
    /// fuerza en vez de distancia y en la Fase 6 con la cercanía de las piedras en un deslizante).
    /// Viven en un asset y no en el código para que retocarlos no cueste una recompilación
    /// (CT-05, RNF-18). Los valores por defecto son los acordados con Santiago el 12/09/2026 y el
    /// 15/09/2026 y el 21/09/2026: diez muescas de fuerza, efectivas la siete y la ocho; diez
    /// muescas de cercanía, efectiva **solo la cinco** (piedras de 72 px encimadas 30 px).
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
        [field: Tooltip("Muesca máxima del deslizante de cercanía de las piedras (va de 0, lo más lejos, a este valor, una encima de otra).")]
        public int SpacingLevels { get; set; } = 10;

        [field: SerializeField]
        [field: Tooltip("Cuánto deben encimarse las piedras para que el golpe haga chispa, en píxeles del lienzo de referencia (1920×1080) antes del acercamiento. Con piedras de 72 px y hojas de 84 px, 30 es la muesca cinco y solo ella.")]
        public float EffectiveOverlap { get; set; } = 30f;

        [field: SerializeField]
        [field: Tooltip("Margen a cada lado del encimado efectivo dentro del cual el golpe sigue siendo certero, en los mismos píxeles.")]
        public float OverlapTolerance { get; set; } = 4f;

        [field: SerializeField]
        [field: Tooltip("Cuánto se acerca la cámara al entorno al reunir los materiales (2 = el doble).")]
        public float GatherZoom { get; set; } = 2f;

        [field: SerializeField]
        [field: Tooltip("Cuánto dura el acercamiento y el acomodo de las hojas en fogata, en segundos.")]
        public float GatherSeconds { get; set; } = 0.8f;

        [field: SerializeField]
        [field: Tooltip("Cuánto dura el nacimiento del fuego —la llama cenital creciendo sobre el montón mientras las hojas se queman desde el centro— antes de pasar a la escena de cierre, en segundos.")]
        public float IgnitionSeconds { get; set; } = 3.5f;

        [field: SerializeField]
        [field: Tooltip("Hasta dónde llega el quemado del montón al nacer el fuego, como fracción de su ancho (0.5 = un círculo de medio montón). Crece durante IgnitionSeconds y ahí se queda.")]
        public float BurnExtent { get; set; } = 0.5f;

        [field: SerializeField]
        [field: Tooltip("Golpes efectivos necesarios para habilitar el soplo.")]
        public int MinimumEffectiveStrikes { get; set; } = 3;

        [field: SerializeField]
        [field: Tooltip("Fallos consecutivos tras los cuales el guía ofrece una pista (RF-13).")]
        public int AttemptsBeforeHint { get; set; } = 3;
    }
}
