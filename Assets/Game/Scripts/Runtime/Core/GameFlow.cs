using System.Collections.Generic;

namespace Game.Core
{
    /// <summary>
    /// La máquina de estados del juego. C# plano y sin dependencias de Unity a propósito: así se
    /// prueba entera en EditMode, sin escena y sin frames, que es lo que hace verificable el
    /// recorrido completo del Golden Path (RNF-13).
    /// </summary>
    /// <remarks>
    /// Ninguna transición ilegal lanza: devuelve <c>false</c> y deja el estado como estaba. Un
    /// clic a destiempo no puede dejar a un estudiante de grado cuarto en una pantalla sin salida.
    /// </remarks>
    public class GameFlow
    {
        private static readonly Dictionary<GameState, GameState[]> Allowed =
            new Dictionary<GameState, GameState[]>
            {
                [GameState.Boot] = new[] { GameState.MainMenu },
                [GameState.MainMenu] = new[]
                {
                    GameState.ProfileSelect, GameState.Credits, GameState.TeacherReport
                },
                [GameState.ProfileSelect] = new[] { GameState.LevelSelect, GameState.MainMenu },
                [GameState.LevelSelect] = new[]
                {
                    GameState.Narrative, GameState.Playing, GameState.MainMenu
                },
                [GameState.Narrative] = new[]
                {
                    GameState.Playing, GameState.LevelSummary, GameState.LevelSelect
                },
                // Playing → Playing es reiniciar el nivel o entrar a la fase siguiente (RF-07).
                [GameState.Playing] = new[]
                {
                    GameState.Playing, GameState.Narrative, GameState.LevelSummary,
                    GameState.LevelSelect, GameState.MainMenu
                },
                [GameState.LevelSummary] = new[]
                {
                    GameState.Narrative, GameState.LevelSelect, GameState.MainMenu
                },
                [GameState.Credits] = new[] { GameState.MainMenu },
                [GameState.TeacherReport] = new[] { GameState.MainMenu }
            };

        public GameState Current { get; private set; } = GameState.Boot;

        /// <summary>Perfil activo. Es quien decide qué niveles están desbloqueados (RF-03).</summary>
        public PlayerProfile ActiveProfile { get; private set; }

        /// <summary>
        /// Secuencia que está reproduciendo <see cref="GameState.Narrative"/>.
        /// </summary>
        /// <remarks>
        /// Es el identificador de la secuencia y no el <c>NarrativeSequence</c> en sí, que es lo
        /// que pedía el plan: ese ScriptableObject vive en <c>Game.Scaffolding</c>, que depende
        /// de <c>Game.Core</c>, así que Core no puede verlo sin cerrar un ciclo de assemblies.
        /// Quien resuelve el identificador a asset es el adaptador (T09/T10).
        /// </remarks>
        public string NarrativeSequenceId { get; private set; }

        /// <summary>Nivel que está jugando <see cref="GameState.Playing"/>.</summary>
        public LevelId? PlayingLevel { get; private set; }

        /// <summary>Fase del nivel que está jugando <see cref="GameState.Playing"/> (RF-04).</summary>
        public int PlayingPhase { get; private set; }

        /// <summary>Cambia de estado si la transición es legal. Devuelve si lo hizo.</summary>
        public bool TryGoTo(GameState next)
        {
            if (!Allowed.TryGetValue(Current, out var destinations)
                || System.Array.IndexOf(destinations, next) < 0)
            {
                return false;
            }

            Current = next;
            return true;
        }

        /// <summary>Fija el perfil activo y entra al menú de niveles (RF-02, CU-01).</summary>
        public bool TrySelectProfile(PlayerProfile profile)
        {
            if (profile == null || !TryGoTo(GameState.LevelSelect))
            {
                return false;
            }

            ActiveProfile = profile;
            return true;
        }

        /// <summary>Entra a una escena narrativa parametrizada por su secuencia (RF-05).</summary>
        public bool TryStartNarrative(string sequenceId)
        {
            if (string.IsNullOrEmpty(sequenceId) || !TryGoTo(GameState.Narrative))
            {
                return false;
            }

            NarrativeSequenceId = sequenceId;
            return true;
        }

        /// <summary>
        /// Entra a jugar una fase de un nivel. Rechaza el nivel que el perfil activo todavía no
        /// tiene desbloqueado (RF-03) sin cambiar de estado.
        /// </summary>
        public bool TryStartPlaying(LevelId level, int phase)
        {
            if (ActiveProfile == null || !ActiveProfile.IsUnlocked(level) || !TryGoTo(GameState.Playing))
            {
                return false;
            }

            PlayingLevel = level;
            PlayingPhase = phase;
            return true;
        }
    }
}
