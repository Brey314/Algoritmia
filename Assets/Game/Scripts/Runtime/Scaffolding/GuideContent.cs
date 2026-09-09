using Game.Core;
using UnityEngine;

namespace Game.Scaffolding
{
    /// <summary>
    /// Lo que el guía dice en un nivel: sus tareas con instrucción y pista (RF-13, RNF-18).
    /// </summary>
    /// <remarks>
    /// Un asset por nivel, igual que <see cref="NarrativeSequence"/> es un asset por escena
    /// narrativa: añadir una tarea es editar contenido, no escribir una rama.
    /// </remarks>
    [CreateAssetMenu(menuName = "Algoritm/Contenido del guía", fileName = "N0_Guia")]
    public class GuideContent : ScriptableObject
    {
        [field: SerializeField]
        [field: Tooltip("Nivel al que acompaña este contenido.")]
        public LevelId Level { get; private set; } = LevelId.Fire;

        [field: SerializeField]
        [field: Tooltip("Las tareas del nivel, en el orden en que se activan.")]
        public GuideStep[] Steps { get; private set; } = new GuideStep[0];
    }
}
