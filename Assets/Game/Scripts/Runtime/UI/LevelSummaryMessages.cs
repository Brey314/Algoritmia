using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// Texto del resumen de fin de nivel (RF-45, HU-14 paso 6). Vive en un asset para poder
    /// editarlo sin recompilar (RNF-18), mismo patrón que <see cref="CreditsContent"/>. Ninguna
    /// variante lleva cifras, calificaciones ni puntajes (CP-03, RF-17): describe en lenguaje
    /// narrativo, no evalúa.
    /// </summary>
    [CreateAssetMenu(menuName = "Game/UI/Level Summary Messages", fileName = "LevelSummaryMessages")]
    public class LevelSummaryMessages : ScriptableObject
    {
        [field: SerializeField, TextArea(2, 4)]
        [field: Tooltip("La línea de apertura del resumen, siempre presente.")]
        public string Intro { get; private set; } = "Esto es lo que pasó en la cueva:";

        [field: SerializeField, TextArea(2, 4)]
        [field: Tooltip("Cuando hubo golpes desde una posición no efectiva (Intentos > 0).")]
        public string TriedSeveralPositions { get; private set; } =
            "Probaste golpear desde varios lugares distintos.";

        [field: SerializeField, TextArea(2, 4)]
        [field: Tooltip("Cuando no hubo ningún golpe no efectivo (Intentos == 0).")]
        public string FoundRightAway { get; private set; } =
            "Encontraste el lugar correcto casi de inmediato.";

        [field: SerializeField, TextArea(2, 4)]
        [field: Tooltip("Cuando un golpe efectivo siguió justo a uno que no lo fue (Errores corregidos > 0).")]
        public string CorrectedApproach { get; private set; } =
            "Cuando algo no funcionó, cambiaste de lugar y volviste a intentar.";

        [field: SerializeField, TextArea(2, 4)]
        [field: Tooltip("Cuando no hizo falta corregir nada (Errores corregidos == 0).")]
        public string NoNeedToCorrect { get; private set; } =
            "Seguiste el mismo camino hasta que las chispas prendieron.";
    }
}
