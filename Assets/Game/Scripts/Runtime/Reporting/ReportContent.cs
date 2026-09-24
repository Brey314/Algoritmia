using Game.Core;
using UnityEngine;

namespace Game.Reporting
{
    /// <summary>
    /// El vocabulario de la tabla del informe docente (RF-46, RNF-01, OE1 §3.6.1): fuera del
    /// código (CT-05, RNF-18), como todo texto visible del proyecto.
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Reporting/Report Content", fileName = "ReportContent")]
    public class ReportContent : ScriptableObject
    {
        [field: SerializeField]
        [field: Tooltip("Nombre del Nivel 1 en la tabla.")]
        public string LevelOneLabel { get; private set; } = "Nivel 1";

        [field: SerializeField]
        [field: Tooltip("Nombre del Nivel 2 en la tabla.")]
        public string LevelTwoLabel { get; private set; } = "Nivel 2";

        [field: SerializeField]
        [field: Tooltip("Nombre del Nivel 3 en la tabla.")]
        public string LevelThreeLabel { get; private set; } = "Nivel 3";

        [field: SerializeField]
        [field: Tooltip("Formato de la etiqueta de fase; {0} es el número de fase, desde 1.")]
        public string PhaseLabelFormat { get; private set; } = "Fase {0}";

        [field: SerializeField]
        [field: Tooltip("«Intentos» (OE1 §3.6.1): acciones evaluadas que no producen avance.")]
        public string AttemptsLabel { get; private set; } = "Intentos";

        [field: SerializeField]
        [field: Tooltip("«Errores corregidos» (OE1 §3.6.1).")]
        public string CorrectedErrorsLabel { get; private set; } = "Errores corregidos";

        [field: SerializeField]
        [field: Tooltip("«Pasos utilizados» (OE1 §3.6.1).")]
        public string StepsUsedLabel { get; private set; } = "Pasos utilizados";

        [field: SerializeField]
        [field: Tooltip("«Tiempo de resolución» (OE1 §3.6.1); el valor se formatea en minutos y segundos.")]
        public string ResolutionTimeLabel { get; private set; } = "Tiempo de resolución";

        [field: SerializeField]
        [field: Tooltip("Texto de una fase sin datos: no es lo mismo que mostrar ceros (RF-46).")]
        public string NoDataLabel { get; private set; } = "Sin datos";

        [field: SerializeField]
        [field: Tooltip("Texto cuando no hay ningún perfil registrado (CU-11 FA-2a).")]
        public string NoProfilesLabel { get; private set; } = "Todavía no hay perfiles registrados en este equipo.";

        public string LevelLabel(LevelId level) => level switch
        {
            LevelId.Fire => LevelOneLabel,
            LevelId.Wheel => LevelTwoLabel,
            LevelId.River => LevelThreeLabel,
            _ => level.ToString()
        };

        public string PhaseLabel(int phase) => string.Format(PhaseLabelFormat, phase);
    }
}
