using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Game.EditorTools
{
    /// <summary>
    /// Ningún servicio de Unity viaja en el ejecutable (RNF-10, RNF-08, RNF-11, decisión D3): antes de
    /// cada build apaga a la fuerza los interruptores de los servicios, y después comprueba en el
    /// juego compilado que no quedó ningún servidor de Unity.
    /// </summary>
    /// <remarks>
    /// El rc1 se conectaba por TCP a :443 (Analytics, Insights y el rendimiento de Unity) y dejaba
    /// archivos en LocalLow y valores en HKCU (DEF-SPIKE-01) aunque <c>UnityConnectSettings.asset</c>
    /// dijera <c>m_Enabled: 0</c> en disco: el Editor trae el interruptor general encendido en memoria
    /// y es esa copia la que el build serializa en <c>globalgamemanagers</c>. Con el general encendido
    /// Unity da por activas las estadísticas de núcleo y de dispositivo del juego compilado
    /// (<c>AnalyticsSettings.hasCoreStatsInBuild</c>), así que apagar solo Analytics no basta; además
    /// el diagnóstico del motor (Insights) viene encendido de fábrica. Por eso el interruptor general
    /// se fuerza aquí y no se confía en lo que diga el archivo.
    /// <para/>
    /// Solo se escribe en los interruptores que están encendidos: un build limpio no ensucia los
    /// ajustes del proyecto. La lógica es <see cref="ForceOff"/> y <see cref="Verify"/>; los
    /// callbacks solo la llaman con los interruptores reales y la carpeta del build.
    /// </remarks>
    public class UnityServicesOff : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        /// <summary>Servidores de Unity: ninguna cadena con este dominio debe quedar en el juego compilado.</summary>
        internal const string CloudHost = "unity3d.com";

        /// <summary>Un interruptor de servicio de Unity: cómo se lee y cómo se escribe.</summary>
        internal readonly struct Switch
        {
            public readonly string Name;
            public readonly Func<bool> Get;
            public readonly Action<bool> Set;

            public Switch(string name, Func<bool> get, Action<bool> set)
            {
                Name = name;
                Get = get;
                Set = set;
            }
        }

        // Antes que cualquier otro paso del build: ninguno debe leer los ajustes con un servicio encendido.
        public int callbackOrder => int.MinValue;

        public void OnPreprocessBuild(BuildReport report)
        {
            var turnedOff = Enforce(RealSwitches());
            if (turnedOff.Count > 0)
                UnityEngine.Debug.Log("[RNF-10] Servicios de Unity que estaban encendidos y se apagaron antes del build: "
                                      + string.Join(", ", turnedOff));
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            // Solo el ejecutable de entrega; <nombre>_Data/ es la carpeta de datos que Unity deja junto al .exe.
            if (report.summary.platform != BuildTarget.StandaloneWindows64) return;
            var exe = report.summary.outputPath;
            Verify(Path.Combine(Path.GetDirectoryName(exe), Path.GetFileNameWithoutExtension(exe) + "_Data"));
        }

        /// <summary>
        /// Los interruptores de servicio de esta versión de Unity. Los públicos van por su API (si Unity
        /// los retira, el proyecto deja de compilar y se nota); los internos por reflexión, y si dejan de
        /// existir el build falla con su nombre en vez de pasar sin apagarlos.
        /// </summary>
        internal static List<Switch> RealSwitches() => new List<Switch>
        {
            Internal("UnityConnectSettings.enabled (interruptor general de los servicios)",
                "UnityEngine.Connect.UnityConnectSettings, UnityEngine.UnityConnectModule", "enabled"),
            new Switch("AnalyticsSettings.enabled",
                () => UnityEditor.Analytics.AnalyticsSettings.enabled, v => UnityEditor.Analytics.AnalyticsSettings.enabled = v),
            new Switch("PerformanceReportingSettings.enabled",
                () => UnityEditor.Analytics.PerformanceReportingSettings.enabled, v => UnityEditor.Analytics.PerformanceReportingSettings.enabled = v),
            new Switch("CrashReportingSettings.enabled (Cloud Diagnostics)",
                () => UnityEditor.CrashReporting.CrashReportingSettings.enabled, v => UnityEditor.CrashReporting.CrashReportingSettings.enabled = v),
            new Switch("EngineDiagnosticsSettings.enabled (Insights)",
                () => UnityEditor.EngineDiagnostics.EngineDiagnosticsSettings.enabled, v => UnityEditor.EngineDiagnostics.EngineDiagnosticsSettings.enabled = v),
            new Switch("PurchasingSettings.enabled",
                () => UnityEditor.Purchasing.PurchasingSettings.enabled, v => UnityEditor.Purchasing.PurchasingSettings.enabled = v),
            new Switch("AdvertisementSettings.enabled",
                () => UnityEditor.Advertisements.AdvertisementSettings.enabled, v => UnityEditor.Advertisements.AdvertisementSettings.enabled = v),
            Internal("PlayerSettings.submitAnalytics (estadísticas del equipo del jugador)",
                "UnityEditor.PlayerSettings, UnityEditor.CoreModule", "submitAnalytics"),
            new Switch("PlayerSettings.enableCrashReportAPI",
                () => PlayerSettings.enableCrashReportAPI, v => PlayerSettings.enableCrashReportAPI = v),
        };

        private static Switch Internal(string name, string typeName, string property)
        {
            PropertyInfo Find() =>
                Type.GetType(typeName)?.GetProperty(property, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new MissingMemberException($"{typeName}.{property} no existe en este Unity");
            return new Switch(name, () => (bool)Find().GetValue(null), v => Find().SetValue(null, v));
        }

        /// <summary>
        /// Apaga los interruptores encendidos y los comprueba de nuevo. <c>TurnedOff</c> son los que estaban
        /// encendidos; <c>Stuck</c> los que siguen encendidos o no se pudieron leer o escribir, con el motivo.
        /// </summary>
        internal static (List<string> TurnedOff, List<string> Stuck) ForceOff(IEnumerable<Switch> switches)
        {
            var turnedOff = new List<string>();
            var stuck = new List<string>();
            foreach (var s in switches)
            {
                try
                {
                    if (!s.Get()) continue;
                    s.Set(false);
                    if (s.Get()) stuck.Add(s.Name + " sigue encendido tras apagarlo");
                    else turnedOff.Add(s.Name);
                }
                catch (Exception e)
                {
                    stuck.Add(s.Name + ": " + (e.InnerException ?? e).Message);
                }
            }
            return (turnedOff, stuck);
        }

        /// <summary>
        /// <see cref="ForceOff"/> y, si algún interruptor no se apaga, falla el build con la lista:
        /// un ejecutable que se conecta a Internet no debe pasar por bueno. Devuelve los que apagó.
        /// </summary>
        internal static List<string> Enforce(IEnumerable<Switch> switches)
        {
            var (turnedOff, stuck) = ForceOff(switches);
            if (stuck.Count > 0)
                throw new BuildFailedException("RNF-10: no se pudieron apagar los servicios de Unity, y el ejecutable "
                                               + "se conectaría a Internet: " + string.Join("; ", stuck));
            return turnedOff;
        }

        /// <summary>
        /// Falla si <c>globalgamemanagers</c> de la carpeta de datos del juego compilado nombra un servidor
        /// de Unity (el rc1 llevaba <c>cdp.cloud.unity3d.com</c>, <c>config.uca.cloud.unity3d.com</c>,
        /// <c>perf-events.cloud.unity3d.com</c> y <c>engine-data.unity3d.com</c>; con los servicios apagados
        /// Unity ya no los serializa) o si no puede leerlo: no verificar tampoco es pasar.
        /// </summary>
        internal static void Verify(string dataDir)
        {
            var path = Path.Combine(dataDir, "globalgamemanagers");
            if (!File.Exists(path))
                throw new BuildFailedException($"RNF-10: no se pudo verificar que el juego no nombre servidores de Unity: falta {path}");
            if (Encoding.ASCII.GetString(File.ReadAllBytes(path)).Contains(CloudHost))
                throw new BuildFailedException($"RNF-10: el juego compilado nombra servidores de Unity ({CloudHost}) en {path}; "
                                               + "algún servicio quedó encendido y el ejecutable se conectaría a Internet");
        }
    }
}
