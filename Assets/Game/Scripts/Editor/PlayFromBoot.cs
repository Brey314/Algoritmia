using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Game.EditorTools
{
    /// <summary>
    /// Hace que pulsar Play arranque siempre en <c>Boot</c>, sea cual sea la escena abierta.
    /// </summary>
    /// <remarks>
    /// Los tres objetos persistentes —<c>GameFlowRunner</c>, <c>SceneLoader</c> y el gestor de
    /// audio— los instancia la escena <c>Boot</c>. Sin ella no hay flujo, y pulsar Play sobre una
    /// escena suelta daba una pantalla que no navega a ninguna parte. Esto usa
    /// <c>playModeStartScene</c>, la función nativa de Unity para exactamente este caso: **solo
    /// afecta al Editor**, nunca al ejecutable, donde <c>Boot</c> ya es la primera escena de Build
    /// Settings.
    ///
    /// **Se suspende mientras corren las pruebas, y no es opcional.** El Test Framework de este
    /// proyecto (<c>@1405238725ab</c>) **no gestiona <c>playModeStartScene</c>** —no lo menciona en
    /// ningún archivo del paquete—, así que si no lo despejamos una corrida PlayMode entra a Play,
    /// <c>playModeStartScene</c> carga <c>Boot</c>, el juego arranca solo y el corredor nunca recibe
    /// el control: **la suite PlayMode se cuelga entera** (medido el 07/09/2026: 1500 s sin informe;
    /// y de nuevo el 10/09 al lanzar la suite desde el MCP de Rider). El callback
    /// <see cref="ICallbacks.RunStarted"/> no siempre llega antes de <c>ExitingEditMode</c> —depende
    /// de quién lance la corrida—, así que la guarda principal es el flag interno del corredor,
    /// consultado por reflexión en <see cref="OnPlayModeStateChanged"/>. Batchmode (por donde entra
    /// <c>unity test</c>) se despeja aparte, en el constructor.
    ///
    /// Para depurar una escena aislada, basta con vaciar el campo en Project Settings.
    /// </remarks>
    [InitializeOnLoad]
    public static class PlayFromBoot
    {
        private const string BootScenePath = "Assets/Game/Scenes/Boot.unity";

        static PlayFromBoot()
        {
            // `unity test` siempre corre en batchmode, y ahí nadie pulsa Play a mano: no hay nada
            // que facilitar y sí una suite que romper. Se limpia en vez de solo no ponerlo, por si
            // quedó fijado de otra sesión.
            if (Application.isBatchMode)
            {
                Suspend();
                return;
            }

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            var api = ScriptableObject.CreateInstance<TestRunnerApi>();
            api.RegisterCallbacks(new SuspendWhileTestsRun());

            RefreshStartScene();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            switch (change)
            {
                // Antes de que Unity consulte playModeStartScene: si el corredor de pruebas está
                // arrancando esta entrada a Play, la escena de arranque tiene que ser la suya.
                case PlayModeStateChange.ExitingEditMode when PlaymodeTestsRunning():
                    Suspend();
                    break;
                // De vuelta en el Editor tras una corrida: restablece el arranque por Boot.
                case PlayModeStateChange.EnteredEditMode:
                    RefreshStartScene();
                    break;
            }
        }

        /// <summary>
        /// <c>UnityEditor.TestTools.TestRunner.PlaymodeLauncher.IsRunning</c>: flag interno estable
        /// —lo usa también el graphics test framework para distinguir EditMode de PlayMode— sin
        /// visibilidad pública, de ahí la reflexión. Si la API desapareciera, el peor caso es
        /// volver al comportamiento anterior (la guarda de <see cref="ICallbacks.RunStarted"/>
        /// sigue puesta).
        /// </summary>
        private static bool PlaymodeTestsRunning()
        {
            var type = Type.GetType(
                "UnityEditor.TestTools.TestRunner.PlaymodeLauncher, UnityEditor.TestRunner");
            var field = type?.GetField("IsRunning", BindingFlags.Public | BindingFlags.Static);
            return field?.GetValue(null) is true;
        }

        private static void RefreshStartScene()
        {
            if (PlaymodeTestsRunning())
            {
                Suspend();
            }
            else
            {
                Apply();
            }
        }

        private static void Apply()
        {
            var boot = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootScenePath);
            if (boot != null && EditorSceneManager.playModeStartScene != boot)
            {
                EditorSceneManager.playModeStartScene = boot;
            }
        }

        private static void Suspend() => EditorSceneManager.playModeStartScene = null;

        /// <summary>Devuelve el control de la escena de arranque al corredor de pruebas.</summary>
        private class SuspendWhileTestsRun : ICallbacks
        {
            public void RunStarted(ITestAdaptor testsToRun) => Suspend();

            public void RunFinished(ITestResultAdaptor result) => RefreshStartScene();

            public void TestStarted(ITestAdaptor test)
            {
            }

            public void TestFinished(ITestResultAdaptor result)
            {
            }
        }
    }
}
