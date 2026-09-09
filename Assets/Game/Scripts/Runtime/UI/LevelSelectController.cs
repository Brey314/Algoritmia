using System;
using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Menú de niveles (RF-03). Los tres niveles se muestran **siempre**; los que el perfil activo
    /// aún no alcanzó salen bloqueados —color más un candado y la palabra «Bloqueado», nunca solo
    /// color (RNF-19)— y no responden al clic. Adaptador delgado: la regla de desbloqueo vive en
    /// <see cref="LevelUnlockPolicy"/>.
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
        }

        [SerializeField] private LevelEntry[] entries;
        [SerializeField] private Button backButton;

        internal GameFlowRunner Runner { get; set; }

#if UNITY_INCLUDE_TESTS
        internal Button ButtonFor(LevelId level) => Find(level).button;
        internal bool LockBadgeShownFor(LevelId level) => Find(level).lockedBadge.activeSelf;
        private LevelEntry Find(LevelId level) => Array.Find(entries, entry => entry.level == level);
#endif

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

            Runner.StartNarrative($"N{(int)level}_Apertura");
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
            }
        }
    }
}
