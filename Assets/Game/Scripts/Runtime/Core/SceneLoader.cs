using System.Collections;
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

        [field: SerializeField]
        [field: Min(0f)]
        [field: Tooltip("Segundos de cada mitad del fundido a negro al pasar de una narrativa a una mecánica, de una mecánica a una narrativa o entre dos narrativas: tantos en salir y tantos en entrar. Cero = corte seco.")]
        public float FadeSeconds { get; set; } = 0.4f;

        /// <summary>Opacidad del negro que cubre la pantalla: 0 nada, 1 negro entero.</summary>
        public float FadeAlpha { get; private set; }

        /// <summary>Si hay un fundido en curso, en cualquiera de sus dos mitades.</summary>
        public bool IsFading { get; private set; }

        private Coroutine _fade;

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

        /// <summary>
        /// Carga la escena. Con <paramref name="fade"/>, primero funde a negro, carga, y funde de
        /// vuelta cuando la escena nueva ya se montó: el paso entre la historia y el juego se
        /// lee como un corte de cuento y no como un salto de pantalla.
        /// </summary>
        /// <remarks>
        /// **Tiempo sin escalar**: se sale al menú desde la pausa con <c>timeScale</c> a cero.
        /// **Si llega otra carga en medio, gana la última**: se corta la anterior y se sigue desde
        /// el negro que haya, así que nunca quedan dos escenas pidiéndose a la vez.
        /// </remarks>
        public void Load(string sceneName, bool fade = false)
        {
            if (_fade != null)
            {
                StopCoroutine(_fade);
                _fade = null;
            }

            if (!fade || FadeSeconds <= 0f)
            {
                FadeAlpha = 0f;
                IsFading = false;
                LoadNow(sceneName);
                return;
            }

            _fade = StartCoroutine(FadeAndLoad(sceneName));
        }

        private IEnumerator FadeAndLoad(string sceneName)
        {
            IsFading = true;
            while (FadeAlpha < 1f)
            {
                FadeAlpha = Mathf.MoveTowards(FadeAlpha, 1f, Time.unscaledDeltaTime / FadeSeconds);
                yield return null;
            }

            var operation = LoadNow(sceneName);
            while (!operation.isDone)
            {
                yield return null;
            }

            // Un cuadro más: los `Start` de la escena nueva montan lo que se ve, y el negro no se
            // levanta sobre una pantalla a medio armar.
            yield return null;
            while (FadeAlpha > 0f)
            {
                FadeAlpha = Mathf.MoveTowards(FadeAlpha, 0f, Time.unscaledDeltaTime / FadeSeconds);
                yield return null;
            }

            IsFading = false;
            _fade = null;
        }

        /// <summary>
        /// El negro del fundido. IMGUI y no un <c>Canvas</c>: <c>Game.Core</c> no depende de uGUI,
        /// y lo que se pinta en <c>OnGUI</c> queda por encima de todos los canvas de la escena.
        /// </summary>
        private void OnGUI()
        {
            if (FadeAlpha <= 0f)
            {
                return;
            }

            GUI.depth = -1000;
            var previous = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, FadeAlpha);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = previous;
        }

        private AsyncOperation LoadNow(string sceneName)
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
            return operation;
        }
    }
}
