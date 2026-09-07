using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Core
{
    /// <summary>
    /// Carga de escenas. Uno de los tres únicos objetos con <c>DontDestroyOnLoad</c> del
    /// proyecto, junto a <see cref="GameFlowRunner"/> y al gestor de audio.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        /// <summary>
        /// Segundos que tardó la última carga. RNF-04 fija el presupuesto en diez segundos y su
        /// criterio de verificación es una medición, no una estimación: se anota de aquí.
        /// </summary>
        public float LastLoadSeconds { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                // RNF-16: volver a una escena ya visitada no puede dejar dos cargadores vivos.
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

        public void Load(string sceneName)
        {
            var startedAt = Time.realtimeSinceStartupAsDouble;

            // «Por qué no» un Stopwatch alrededor de SceneManager.LoadScene: esa llamada solo
            // encola la carga —se aplica al final del frame—, así que el cronómetro medía el
            // encolado (~0 s) y nunca la carga. RNF-04 exige medir el tiempo real: se usa la
            // versión asíncrona y se cierra el cronómetro en `completed`, cuando la escena ya
            // está cargada y activa.
            var operation = SceneManager.LoadSceneAsync(sceneName);
            operation.completed += _ =>
            {
                LastLoadSeconds = (float)(Time.realtimeSinceStartupAsDouble - startedAt);

                if (LastLoadSeconds > 10f)
                {
                    Debug.LogWarning($"RNF-04: «{sceneName}» tardó {LastLoadSeconds:0.0} s en cargar.");
                }
            };
        }
    }
}
