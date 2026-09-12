using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// Los textos del resumen de fin de nivel (RF-17, RF-45): en palabras, sin cifras, y fuera
    /// del código (CT-05). Los valores por defecto son los de los mockups 13 y 13b, ya con la
    /// mecánica de fuerza y disposición del Nivel 1 (INC-47).
    /// </summary>
    [CreateAssetMenu(menuName = "Game/UI/Level Summary Messages", fileName = "LevelSummaryMessages")]
    public class LevelSummaryMessages : ScriptableObject
    {
        [field: SerializeField, TextArea(2, 4)]
        [field: Tooltip("La línea de apertura del resumen, siempre presente. Es el título de la tablilla.")]
        public string Intro { get; private set; } = "Esto es lo que pasó en la cueva:";

        [field: SerializeField, TextArea(2, 4)]
        [field: Tooltip("Lo que el estudiante descubrió (mockup 13). Va bajo el título.")]
        public string Discovery { get; private set; } = "Descubriste que el fuego necesita chispa y aire.";

        [field: SerializeField, TextArea(2, 4)]
        [field: Tooltip("Cuando hubo golpes que no prendieron (Intentos > 0).")]
        public string TriedSeveralPositions { get; private set; } =
            "Probaste golpear con varias fuerzas y desde varios sitios.";

        [field: SerializeField, TextArea(2, 4)]
        [field: Tooltip("Cuando no hubo ningún golpe fallido (Intentos == 0).")]
        public string FoundRightAway { get; private set; } =
            "Encontraste la fuerza y el sitio correctos casi de inmediato.";

        [field: SerializeField, TextArea(2, 4)]
        [field: Tooltip("Cuando un golpe efectivo siguió justo a uno que no lo fue (Errores corregidos > 0).")]
        public string CorrectedApproach { get; private set; } =
            "Cuando algo no funcionó, cambiaste la fuerza o el sitio y volviste a intentar.";

        [field: SerializeField, TextArea(2, 4)]
        [field: Tooltip("Cuando no hizo falta corregir nada (Errores corregidos == 0).")]
        public string NoNeedToCorrect { get; private set; } =
            "Seguiste el mismo camino hasta que las chispas prendieron.";

        [field: SerializeField, TextArea(2, 4)]
        [field: Tooltip("La habilidad nombrada, en el recuadro verde (mockup 13b; RF-12, CP-07).")]
        public string SkillNamed { get; private set; } =
            "Eso se llama probar y ajustar: es lo mismo que hace quien arma un plan paso a paso.";
    }
}
