using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.EditorTools
{
    /// <summary>
    /// Guarda las escenas que se ensucian mientras Claude trabaja en el Editor, para que nunca
    /// aparezca el diálogo modal «Scene(s) Have Been Modified».
    /// </summary>
    /// <remarks>
    /// Ese diálogo bloquea el hilo principal y con él a los MCP: todo expira como si el Editor
    /// estuviera cargando. Lo abre el Test Runner en su segundo paso (<c>SaveModifiedSceneTask</c>,
    /// antes de cualquier callback), y también abrir otra escena o entrar a Play. Las escenas se
    /// ensucian sin que nadie las edite por dos vías medidas el 23/09/2026:
    /// <list type="bullet">
    /// <item><c>execute_script</c> de coplay marca la escena activa como modificada al terminar,
    /// aunque el script solo lea, e incluso si el propio script acaba de guardarla.</item>
    /// <item>Recompilar cuando un componente de una escena abierta cambió sus campos
    /// serializados; <c>mcp__rider__run_unity_tests</c> compila dentro de la misma llamada, así
    /// que guardar antes no alcanza.</item>
    /// </list>
    /// **Solo cuando lo lanza Claude** (decisión de Santiago, 23/09/2026): el hook
    /// <c>PreToolUse</c> de <c>.claude/settings.json</c> toca <see cref="MarkerPath"/> antes de cada
    /// herramienta de Rider o coplay, y Claude cuenta como activo durante <see cref="Window"/>.
    /// Lo que está sucio mientras Claude está inactivo es **de una persona** y no se guarda nunca;
    /// deja de serlo solo cuando alguien lo guarda. Límite conocido: si una persona edita una
    /// escena limpia mientras Claude está activo, esa edición se guarda con lo demás.
    /// </remarks>
    [InitializeOnLoad]
    public static class ClaudeSceneAutosave
    {
        /// <summary>Lo toca el hook de Claude Code. <c>Temp/</c> lo borra Unity al cerrarse.</summary>
        private const string MarkerPath = "Temp/claude-active";

        /// <summary>Cuánto dura el permiso de un toque: cubre una compilación seguida de la corrida.</summary>
        private static readonly TimeSpan Window = TimeSpan.FromMinutes(2);

        /// <summary>Cada cuánto se relee el marcador, para no tocar disco en cada frame.</summary>
        private const double MarkerPollSeconds = 0.5;

        // SessionState sobrevive a la recarga de dominio; un campo estático no.
        private const string HumanDirtyKey = "Game.EditorTools.ClaudeSceneAutosave.HumanDirty";

        private static double _nextMarkerPoll;
        private static bool _claudeActive;

        static ClaudeSceneAutosave()
        {
            // En batchmode (`unity test`, `unity build`) no hay diálogo que evitar.
            if (Application.isBatchMode)
            {
                return;
            }

            // Un guardado dentro de un execute_script no deja ningún frame con la escena limpia:
            // el evento es la única forma de enterarse de que dejó de ser de una persona.
            EditorSceneManager.sceneSaved += scene =>
                StoreHumanDirty(LoadHumanDirty().Where(path => path != scene.path));
            EditorApplication.update += Tick;
        }

        private static void Tick()
        {
            // Cada frame: el Test Runner puede pedir guardar en el mismo tick en que la escena se
            // ensucia, y un sondeo espaciado llegaría tarde.
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            var dirty = OpenScenes().Where(s => s.isDirty).ToArray();
            if (!ClaudeIsActive())
            {
                StoreHumanDirty(dirty.Select(s => s.path));
                return;
            }

            var humanDirty = LoadHumanDirty();
            foreach (var scene in dirty.Where(s => !humanDirty.Contains(s.path)))
            {
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"[Claude] Guardada {scene.path}: se ensució durante una herramienta de Claude.");
            }
        }

        private static bool ClaudeIsActive()
        {
            if (EditorApplication.timeSinceStartup >= _nextMarkerPoll)
            {
                _nextMarkerPoll = EditorApplication.timeSinceStartup + MarkerPollSeconds;
                _claudeActive = File.Exists(MarkerPath)
                                && DateTime.UtcNow - File.GetLastWriteTimeUtc(MarkerPath) < Window;
            }

            return _claudeActive;
        }

        private static HashSet<string> LoadHumanDirty() =>
            new HashSet<string>(SessionState.GetString(HumanDirtyKey, string.Empty)
                .Split('\n', StringSplitOptions.RemoveEmptyEntries));

        private static void StoreHumanDirty(IEnumerable<string> paths) =>
            SessionState.SetString(HumanDirtyKey, string.Join("\n", paths));

        private static Scene[] OpenScenes() =>
            Enumerable.Range(0, SceneManager.sceneCount)
                .Select(SceneManager.GetSceneAt)
                .Where(s => s.IsValid() && s.isLoaded && !string.IsNullOrEmpty(s.path))
                .ToArray();
    }
}
