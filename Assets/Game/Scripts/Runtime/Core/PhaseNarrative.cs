using System;
using UnityEngine;

namespace Game.Core
{
    /// <summary>
    /// La escena narrativa que presenta una fase jugable: la que el menú de pausa permite volver a
    /// ver (INC-159). Es una fila de la tabla que vive en el prefab <c>MenuPausa</c>, no un
    /// registro de lo visto: el progreso guardado es una lista cerrada (RNF-09) y no gana ningún
    /// campo por esto.
    /// </summary>
    [Serializable]
    public struct PhaseNarrative
    {
        [SerializeField, Tooltip("Nivel de la fase.")]
        private LevelId level;

        [SerializeField, Tooltip("Fase del nivel (1 a 3). La escena debe declarar esta fase como NextPhase.")]
        private int phase;

        [SerializeField, Tooltip("Id de la secuencia narrativa (NarrativeSequence.Id) que presenta la fase.")]
        private string sequenceId;

        public PhaseNarrative(LevelId level, int phase, string sequenceId)
        {
            this.level = level;
            this.phase = phase;
            this.sequenceId = sequenceId;
        }

        public LevelId Level => level;

        public int Phase => phase;

        public string SequenceId => sequenceId;
    }
}
