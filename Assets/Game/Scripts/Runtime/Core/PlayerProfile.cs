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
        public IReadOnlyList<PhaseId> ConfirmedPhases => phases
            .Select(record => new PhaseId((LevelId)record.level, record.phase))
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
        public void ConfirmPhase(PhaseId phase, PerformanceIndicators indicators)
        {
            if (IsPhaseConfirmed(phase))
            {
                return;
            }

            phases.Add(new PhaseRecord
            {
                level = (int)phase.Level,
                phase = phase.Phase,
                attempts = indicators.Attempts,
                correctedErrors = indicators.CorrectedErrors,
                stepsUsed = indicators.StepsUsed,
                resolutionSeconds = indicators.ResolutionSeconds
            });
        }

        public bool IsPhaseConfirmed(PhaseId phase) => Find(phase) != null;

        /// <summary>Indicadores registrados de la fase, o los de una fase sin jugar si no hay.</summary>
        public PerformanceIndicators IndicatorsFor(PhaseId phase)
        {
            var record = Find(phase);
            return record == null
                ? default
                : new PerformanceIndicators(record.attempts, record.correctedErrors, record.stepsUsed,
                    record.resolutionSeconds);
        }

        /// <summary>Si están confirmadas todas las fases del nivel (RF-03).</summary>
        public bool IsLevelComplete(LevelId level) => PhaseId.AllOf(level).All(IsPhaseConfirmed);

        /// <summary>
        /// Primera fase del nivel sin confirmar, o <c>null</c> si el nivel está completo. Es el
        /// punto de retoma tras un cierre: se vuelve a la fase pendiente, no al principio del
        /// nivel (RNF-14, HU-14).
        /// </summary>
        public PhaseId? NextPendingPhase(LevelId level)
        {
            // Un bucle y no FirstOrDefault: PhaseId es struct, así que «no encontrado» ya es una
            // fase válida —la 0 no existe— y el nivel completo dejaría de distinguirse.
            foreach (var phase in PhaseId.AllOf(level))
            {
                if (!IsPhaseConfirmed(phase))
                {
                    return phase;
                }
            }

            return null;
        }

        private PhaseRecord Find(PhaseId phase) => phases.FirstOrDefault(record =>
            record.level == (int)phase.Level && record.phase == phase.Phase);

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
