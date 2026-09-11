using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Core
{
    /// <summary>
    /// Adaptador entre <see cref="GameFlow"/> y las escenas de Unity.
    /// </summary>
    /// <remarks>
    /// No decide nada: pregunta a <see cref="GameFlow"/> si la transición es legal y, si lo es,
    /// traduce el estado resultante a una escena. Toda regla vive en la FSM, que es C# plano y
    /// se prueba sin escena; si una regla se colara aquí dejaría de ser verificable en EditMode.
    /// </remarks>
    public class GameFlowRunner : MonoBehaviour
    {
        /// <summary>
        /// Escena que aloja cada estado. Cada tarea añade la suya. <see cref="GameState.ProfileSelect"/>
        /// no tiene escena propia: es un panel dentro de <c>MainMenu</c>, así que apunta a esa misma
        /// escena y la transición se resuelve intercambiando paneles, no recargando.
        /// </summary>
        private static readonly Dictionary<GameState, string> Scenes =
            new Dictionary<GameState, string>
            {
                [GameState.Boot] = "Boot",
                [GameState.MainMenu] = "MainMenu",
                [GameState.ProfileSelect] = "MainMenu",
                [GameState.LevelSelect] = "LevelSelect",
                // Una sola escena para las quince escenas narrativas del guion: el estado lleva
                // el id de la secuencia y la escena la resuelve (RF-05, RNF-06).
                [GameState.Narrative] = "Narrative",
                // Playing no está aquí: su escena la elige la fase (véase PlayingScenes).
                [GameState.LevelSummary] = "LevelSummary",
                [GameState.Credits] = "Credits"
            };

        /// <summary>
        /// Escena de cada fase jugable. Va aparte de <see cref="Scenes"/> porque
        /// <see cref="GameState.Playing"/> no tiene una escena sino una por fase (RF-04): el
        /// estado lleva nivel y fase, y son ellos los que eligen.
        /// </summary>
        private static readonly Dictionary<PhaseId, string> PlayingScenes =
            new Dictionary<PhaseId, string>
            {
                [new PhaseId(LevelId.Fire, 1)] = "Level1_Cave",
                [new PhaseId(LevelId.Wheel, 1)] = "Level2_Forest"
            };

        public static GameFlowRunner Instance { get; private set; }

        public GameFlow Flow { get; } = new GameFlow();

        /// <summary>El recolector de indicadores de la fase en curso, si la hay (RF-45). Quien
        /// pausa (Game.UI) se lo notifica sin conocer Game.Levels.Fire: la mediación vive aquí.</summary>
        public ILevelReporter ActiveReporter { get; set; }

        /// <summary>Los indicadores de la fase recién resuelta, a la espera de que
        /// <c>LevelSummary</c> los confirme (RF-45, HU-14 paso 6). <see cref="GameFlow.PlayingLevel"/>/
        /// <see cref="GameFlow.PlayingPhase"/> siguen fijos mientras tanto: nada los limpia entre
        /// <see cref="GameState.Playing"/> y <see cref="GameState.LevelSummary"/>.</summary>
        public PerformanceIndicators PendingIndicators { get; set; }

        private ProfileSession _session;

        /// <summary>
        /// La sesión que persiste el progreso del perfil activo (RF-04, RF-09). Vive aquí porque
        /// este es uno de los tres objetos que sobreviven al cambio de escena. Se construye al
        /// primer uso: así una prueba que no la toca no acaba escribiendo la sonda en disco.
        /// </summary>
        public ProfileSession Session => _session ??= BuildSession();

        private ProfileSession BuildSession()
        {
            // La carpeta portable «Datos/» va junto al ejecutable (RNF-07, RNF-11); en el Editor,
            // eso es la raíz del proyecto. Si no es escribible, SaveStore cae a la ruta del
            // sistema y lo expone (INC-34).
            var portableRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Datos"))
                .Replace('\\', '/');
            return new ProfileSession(Flow,
                new SaveStore(new DiskFileSystem(), portableRoot, Application.persistentDataPath));
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                // RNF-16: volver a una escena ya visitada no puede dejar dos flujos vivos.
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            // Boot no es una pantalla: existe solo para instanciar los objetos persistentes y
            // pasar el control al inicio (RF-01).
            GoTo(GameState.MainMenu);
        }

        public bool GoTo(GameState next) => Apply(Flow.TryGoTo(next));

        public bool SelectProfile(PlayerProfile profile) => Apply(Flow.TrySelectProfile(profile));

        public bool StartNarrative(string sequenceId) => Apply(Flow.TryStartNarrative(sequenceId));

        public bool StartPlaying(LevelId level, int phase) => Apply(Flow.TryStartPlaying(level, phase));

        private bool Apply(bool transitioned)
        {
            if (!transitioned)
            {
                return false;
            }

            if (TryResolveScene(out var sceneName))
            {
                // Si la escena ya está activa, el cambio de estado es un intercambio de paneles
                // dentro de ella —lo hace la UI— y no una recarga. Sin `SceneLoader` (una prueba
                // que solo ejercita el flujo, sin la escena Boot) la transición actualiza la FSM
                // pero no toca escenas.
                //
                // Excepción: reentrar a `Playing` siempre recarga, aunque el nombre de escena no
                // cambie (T16, RF-07) — es «Reiniciar» desde el menú de pausa, y sin esto la FSM
                // aceptaba la transición sin que pasara nada. `Playing` es el único estado que se
                // tiene a sí mismo como destino legal, así que esto no afecta a ningún otro caso.
                var mustReload = Flow.Current == GameState.Playing;
                if ((sceneName != SceneManager.GetActiveScene().name || mustReload)
                    && SceneLoader.Instance != null)
                {
                    SceneLoader.Instance.Load(sceneName);
                }
            }
            else
            {
                Debug.LogWarning($"El estado {Flow.Current} todavía no tiene escena asociada.", this);
            }

            return true;
        }

        /// <summary>La escena que aloja el estado actual, si ya existe alguna.</summary>
        private bool TryResolveScene(out string sceneName)
        {
            if (Flow.Current != GameState.Playing)
            {
                return Scenes.TryGetValue(Flow.Current, out sceneName);
            }

            sceneName = null;
            return Flow.PlayingLevel.HasValue
                   && PlayingScenes.TryGetValue(
                       new PhaseId(Flow.PlayingLevel.Value, Flow.PlayingPhase), out sceneName);
        }
    }
}
