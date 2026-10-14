using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Game.UI
{
    /// <summary>
    /// Tras cargar cualquier escena, la entrada no llega a su interfaz durante una ventana corta
    /// (DEF-RC3-09).
    /// </summary>
    /// <remarks>
    /// **La causa estaba en la carga y no en una pantalla.** Un doble clic rápido en «Omitir» —el cierre
    /// del Nivel 1, por ejemplo— mandaba el primer clic a salir de la narrativa y el segundo a la escena
    /// que acababa de cargar, cuyo botón «Volver al menú de niveles» quedaba justo debajo: el estudiante
    /// aparecía en el menú sin haber visto el resumen (RF-12, RF-45). Evitar la doble salida dentro de la
    /// narrativa (<c>NarrativeSceneController.Leave</c>) no podía arreglarlo, porque el segundo clic ya
    /// no era para esa escena. Cualquier par de pantallas con un botón en el mismo sitio tenía el mismo
    /// riesgo, así que se guarda en el único punto común: <see cref="SceneManager.sceneLoaded"/>.
    ///
    /// **Se apaga el módulo de entrada y no el <c>EventSystem</c>** ni los <c>GraphicRaycaster</c>: con el
    /// <c>EventSystem</c> apagado <c>EventSystem.current</c> es nulo, y apagar los raycasters deja a quien
    /// pregunta con <c>RaycastAll</c> sin respuesta. El módulo apagado, en cambio, solo deja de recibir
    /// el puntero. Se apaga en <c>sceneLoaded</c>, antes de que el <c>EventSystem</c> nuevo corra su
    /// primer <c>Update</c> y tome el módulo como el vigente.
    ///
    /// Vive en <c>Persistent</c> (<c>Boot</c>), junto a <c>SceneLoader</c>: <c>Game.Core</c> no referencia
    /// uGUI y por eso la guarda no puede estar en el cargador. No añade un cuarto singleton: no tiene
    /// <c>Instance</c> ni <c>DontDestroyOnLoad</c> propios.
    /// </remarks>
    public class InputGuard : MonoBehaviour
    {
        [field: SerializeField]
        [field: Min(0f)]
        [field: Tooltip("Segundos, tras cargar una escena, en que los clics no llegan a su interfaz: el segundo clic de un doble clic en «Omitir» no debe pulsar el botón que quede debajo en la escena siguiente. Cero = sin guarda.")]
        public float Seconds { get; set; } = 0.3f;

        private readonly List<BaseInputModule> _held = new List<BaseInputModule>();
        private float _until;

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Release();
        }

        // Tiempo real y no escalado: se sale de la pausa (timeScale a cero) a otra escena.
        private void Update() => Tick(Time.realtimeSinceStartup);

        internal void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (Seconds > 0f)
            {
                Hold(Time.realtimeSinceStartup + Seconds);
            }
        }

        /// <summary>
        /// Apaga los módulos de entrada que estén encendidos hasta <paramref name="until"/>. Una carga
        /// dentro de la ventana la amplía; lo que ya estaba apagado no se enciende al terminar.
        /// </summary>
        internal void Hold(float until)
        {
            _until = until;
            foreach (var module in FindObjectsByType<BaseInputModule>(FindObjectsInactive.Exclude))
            {
                if (module.enabled)
                {
                    module.enabled = false;
                    _held.Add(module);
                }
            }
        }

        internal void Tick(float now)
        {
            if (_held.Count > 0 && now >= _until)
            {
                Release();
            }
        }

        private void Release()
        {
            foreach (var module in _held)
            {
                if (module != null)
                {
                    module.enabled = true;
                }
            }

            _held.Clear();
        }
    }
}
