using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
        /// Escena de cada estado. Solo están los estados que ya tienen escena; cada tarea añade
        /// la suya. <see cref="GameState.ProfileSelect"/> no lleva escena propia: es un panel
        /// dentro de <c>MainMenu</c>.
        /// </summary>
        private static readonly Dictionary<GameState, string> Scenes =
            new Dictionary<GameState, string>
            {
                [GameState.Boot] = "Boot",
                [GameState.MainMenu] = "MainMenu"
            };

        public static GameFlowRunner Instance { get; private set; }

        public GameFlow Flow { get; } = new GameFlow();

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

        public bool StartNarrative(string sequenceId) => Apply(Flow.TryStartNarrative(sequenceId));

        public bool StartPlaying(LevelId level, int phase) => Apply(Flow.TryStartPlaying(level, phase));

        private bool Apply(bool transitioned)
        {
            if (!transitioned)
            {
                return false;
            }

            if (Scenes.TryGetValue(Flow.Current, out var sceneName))
            {
                SceneLoader.Instance.Load(sceneName);
            }
            else
            {
                Debug.LogWarning($"El estado {Flow.Current} todavía no tiene escena asociada.");
            }

            return true;
        }
    }
}
