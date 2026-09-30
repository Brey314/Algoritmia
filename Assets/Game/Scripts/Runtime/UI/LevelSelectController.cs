using System;
using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Menú de niveles (RF-03). Los tres niveles se muestran **siempre**; los que el perfil activo
    /// aún no alcanzó salen bloqueados —color más un candado y la palabra «Bloqueado», nunca solo
    /// color (RNF-19)— y no responden al clic. Los que ya se completaron llevan además la marca
    /// «Completado» —icono y palabra, tampoco solo color (RNF-19, HU-14)— y siguen siendo
    /// jugables: completado no es lo mismo que bloqueado. Sin cifras (CP-03). Adaptador delgado:
    /// la regla de desbloqueo vive en <see cref="LevelUnlockPolicy"/> y la de completado en
    /// <see cref="PlayerProfile.IsLevelComplete"/>.
    /// </summary>
    public class LevelSelectController : MonoBehaviour
    {
        [Serializable]
        private class LevelEntry
        {
            public LevelId level;
            public Button button;

            [Tooltip("Grupo con el candado y el texto «Bloqueado». Se muestra solo si el nivel está bloqueado.")]
            public GameObject lockedBadge;

            [Tooltip("Grupo con el icono de visto y el texto «Completado». Se muestra solo si todas " +
                     "las fases del nivel están confirmadas (HU-14).")]
            public GameObject completedBadge;

            [Tooltip("Secuencia narrativa con la que abre el nivel, p. ej. «N1_Apertura».")]
            public string openingSequenceId;
        }

        [SerializeField] private LevelEntry[] entries;
        [SerializeField] private Button backButton;

        internal GameFlowRunner Runner { get; set; }

#if UNITY_INCLUDE_TESTS
        internal Button ButtonFor(LevelId level) => Find(level).button;
        internal bool LockBadgeShownFor(LevelId level) => Find(level).lockedBadge.activeSelf;
        internal bool CompletedBadgeShownFor(LevelId level) => Find(level).completedBadge.activeSelf;
#endif

        private LevelEntry Find(LevelId level) => Array.Find(entries, entry => entry.level == level);

        private void Awake() => Runner ??= GameFlowRunner.Instance;

        private void Start()
        {
            foreach (var entry in entries)
            {
                var level = entry.level;
                entry.button.onClick.AddListener(() => StartLevel(level));
            }

            backButton.onClick.AddListener(BackToMainMenu);
        }

        private void StartLevel(LevelId level)
        {
            if (!ScreenFlow.Ready(Runner, this))
            {
                return;
            }

            // Qué secuencia abre cada nivel es contenido de la ficha, no una fórmula sobre el
            // número del nivel: «N{n}_Apertura» solo acierta en el Nivel 1 —el 2 abre por la
            // escena 2.1 del guion— y pedía un id inexistente, con lo que la escena narrativa
            // se abría en blanco y el nivel quedaba inalcanzable.
            Runner.StartNarrative(Find(level).openingSequenceId);
        }

        private void BackToMainMenu()
        {
            if (!ScreenFlow.Ready(Runner, this))
            {
                return;
            }

            Runner.GoTo(GameState.MainMenu);
        }

        private void OnEnable() => Refresh();

        /// <summary>
        /// Repinta el estado bloqueado/desbloqueado de cada nivel según el perfil activo. Se llama
        /// al mostrar el menú; también al volver de completar un nivel (T15/T18), cuando el
        /// siguiente acaba de habilitarse.
        /// </summary>
        internal void Refresh()
        {
            var profile = Runner != null ? Runner.Flow.ActiveProfile : null;

            foreach (var entry in entries)
            {
                var unlocked = profile != null && LevelUnlockPolicy.IsUnlocked(profile, entry.level);
                entry.button.interactable = unlocked;
                entry.lockedBadge.SetActive(!unlocked);
                // Un nivel completado se puede repetir (interactable no cambia): la marca solo
                // informa que ya se terminó, con icono y palabra y no solo color (RNF-19), y sin
                // cifras (CP-03).
                entry.completedBadge.SetActive(profile != null && profile.IsLevelComplete(entry.level));
            }
        }
    }
}
