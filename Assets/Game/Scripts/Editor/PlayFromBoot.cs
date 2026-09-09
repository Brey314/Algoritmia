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
    /// **Se suspende mientras corren las pruebas, y no es opcional.** El Test Framework entra a
    /// Play con una escena propia (<c>InitTestScene&lt;guid&gt;.unity</c>); si
    /// <c>playModeStartScene</c> se la cambia por <c>Boot</c>, el corredor se queda esperando una
    /// escena que nunca llega y **la suite PlayMode se cuelga entera** — medido el 07/09/2026:
    /// 1500 s sin producir informe, con `Loaded scene Boot.unity` justo después de importar la
    /// InitTestScene en el log. De ahí las dos guardas: batchmode (por donde entra `unity test`) y
    /// los callbacks del corredor (por donde entra la ventana Test Runner del Editor).
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

            Apply();

            var api = ScriptableObject.CreateInstance<TestRunnerApi>();
            api.RegisterCallbacks(new SuspendWhileTestsRun());
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

            public void RunFinished(ITestResultAdaptor result) => Apply();

            public void TestStarted(ITestAdaptor test)
            {
            }

            public void TestFinished(ITestResultAdaptor result)
            {
            }
        }
    }
}
