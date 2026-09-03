using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Game.Core
{
    /// <summary>
    /// Perfil del estudiante: nombre o alias, progreso de avance y los cuatro indicadores por
    /// fase. La lista es cerrada (RNF-09, OE1 §3.6.1 nota 5) y esto es todo lo que se persiste.
    /// </summary>
    /// <remarks>
    /// No hay campo de puntaje, y no es un olvido: CP-03 lo prohíbe y RF-17 prohíbe cifras en la
    /// retroalimentación. La razón es pedagógica, no técnica — sin esta nota, una futura
    /// «mejora» reintroduce el marcador.
    /// </remarks>
    [Serializable]
    public class PlayerProfile
    {
        // Los nombres de estos campos son las claves del JSON guardado, que se revisa a mano
        // contra la lista cerrada de RNF-09: por eso van sin guion bajo, al contrario que el
        // resto de campos privados del proyecto.
        [SerializeField] private string name;
        [SerializeField] private int reachedLevel;
        [SerializeField] private List<PhaseRecord> phases = new List<PhaseRecord>();

        /// <summary>Nombre o alias. Único dato personal que se pide (RF-02).</summary>
        public string Name => name;

        /// <summary>Nivel más avanzado que el perfil tiene habilitado (RF-03).</summary>
        public LevelId ReachedLevel => (LevelId)reachedLevel;

        /// <summary>Fases ya confirmadas, en el orden en que se confirmaron (RF-04).</summary>
        public IReadOnlyList<(LevelId Level, int Phase)> ConfirmedPhases => phases
            .Select(record => ((LevelId)record.level, record.phase))
            .ToArray();

        /// <summary>Requerido por <c>JsonUtility</c>; para crear un perfil se usa <see cref="Create"/>.</summary>
        private PlayerProfile()
        {
        }

        /// <summary>
        /// Crea un perfil validando el nombre. Devuelve un resultado tipado: los rechazos son
        /// flujos alternos de HU-01, no excepciones.
        /// </summary>
        public static ProfileCreationResult Create(string profileName, IEnumerable<string> existingNames)
        {
            if (string.IsNullOrWhiteSpace(profileName))
            {
                return ProfileCreationResult.Rejected(ProfileCreationResult.Status.EmptyName);
            }

            var trimmed = profileName.Trim();

            // Cada perfil es un archivo dentro de Datos/: un nombre con separadores de ruta
            // escribiría fuera de la carpeta portable y rompería RNF-07 y RNF-11.
            if (trimmed.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                return ProfileCreationResult.Rejected(ProfileCreationResult.Status.InvalidName);
            }

            if (existingNames.Any(existing => string.Equals(existing, trimmed,
                    StringComparison.OrdinalIgnoreCase)))
            {
                return ProfileCreationResult.Rejected(ProfileCreationResult.Status.DuplicateName);
            }

            return ProfileCreationResult.Created(new PlayerProfile
            {
                name = trimmed,
                reachedLevel = (int)LevelId.Fire // HU-01 FA-03: el perfil nuevo solo alcanza el Nivel 1.
            });
        }

        /// <summary>Si el perfil puede entrar al nivel (RF-03).</summary>
        public bool IsUnlocked(LevelId level) => level <= ReachedLevel;

        /// <summary>
        /// Habilita el nivel. Nunca retrocede: un nivel ya desbloqueado no se vuelve a bloquear
        /// (RF-03, RF-41) — tampoco al reiniciar el nivel desde la pausa (HU-17).
        /// </summary>
        public void Reach(LevelId level) => reachedLevel = Math.Max(reachedLevel, (int)level);

        /// <summary>
        /// Confirma una fase con sus indicadores (RF-04, RF-45). Si la fase ya estaba confirmada
        /// no se toca: OE1 §3.6.1 nota 4 manda conservar el registro del intento anterior.
        /// </summary>
        public void ConfirmPhase(LevelId level, int phase, PerformanceIndicators indicators)
        {
            if (IsPhaseConfirmed(level, phase))
            {
                return;
            }

            phases.Add(new PhaseRecord
            {
                level = (int)level,
                phase = phase,
                attempts = indicators.Attempts,
                correctedErrors = indicators.CorrectedErrors,
                stepsUsed = indicators.StepsUsed,
                resolutionSeconds = indicators.ResolutionSeconds
            });
        }

        public bool IsPhaseConfirmed(LevelId level, int phase) => Find(level, phase) != null;

        /// <summary>Indicadores registrados de la fase, o los de una fase sin jugar si no hay.</summary>
        public PerformanceIndicators IndicatorsFor(LevelId level, int phase)
        {
            var record = Find(level, phase);
            return record == null
                ? default
                : new PerformanceIndicators(record.attempts, record.correctedErrors, record.stepsUsed,
                    record.resolutionSeconds);
        }

        private PhaseRecord Find(LevelId level, int phase) =>
            phases.FirstOrDefault(record => record.level == (int)level && record.phase == phase);

        /// <summary>Una fase confirmada con sus cuatro indicadores. Su forma es la del JSON.</summary>
        [Serializable]
        private class PhaseRecord
        {
            public int level;
            public int phase;
            public int attempts;
            public int correctedErrors;
            public int stepsUsed;
            public float resolutionSeconds;
        }
    }
}
