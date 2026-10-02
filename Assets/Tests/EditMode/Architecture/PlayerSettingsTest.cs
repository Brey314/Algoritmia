// Identidad y configuración del ejecutable (INC-97, decisiones D2 y D3 del 29/09/2026): «Algoritmia»,
// sin analítica ni estadísticas de hardware, sin pantalla de presentación de Unity y sin Alt+Intro.
// Se lee lo declarado en ProjectSettings/ProjectSettings.asset porque es la declaración la que
// decide qué presenta el ejecutable, y una inspección sin prueba se degrada en la siguiente sesión.

using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace Game.Architecture.Tests
{
    public class PlayerSettingsTest
    {
        private static string ProjectRoot => Directory.GetParent(Application.dataPath).FullName;

        private static string PlayerSettingsText =>
            File.ReadAllText(Path.Combine(ProjectRoot, "ProjectSettings/ProjectSettings.asset"));

        private static string UnityConnectSettingsText =>
            File.ReadAllText(Path.Combine(ProjectRoot, "ProjectSettings/UnityConnectSettings.asset"));

        [Test]
        public void Architecture_RF01_ElEjecutableSeIdentificaComoAlgoritmia()
        {
            var settings = PlayerSettingsText;

            Assert.That(settings, Does.Match(@"(?m)^  productName: Algoritmia\r?$"),
                "el ejecutable se presenta como «Algoritmia» y no como la plantilla de Unity (RF-01, D2)");
            Assert.That(settings, Does.Match(@"(?m)^  companyName: Universidad Catolica de Colombia\r?$"),
                "la empresa es «Universidad Catolica de Colombia», no «DefaultCompany» (D2)");
        }

        [Test]
        public void Architecture_RNF10_ElEjecutableNoEnviaAnaliticaNiEstadisticasDeHardware()
        {
            var settings = PlayerSettingsText;

            Assert.That(settings, Does.Match(@"submitAnalytics: 0\b"),
                "no se envían estadísticas del equipo del jugador (RNF-08, RNF-10, D3)");

            // No se comprueba la ausencia de SENTIS_ANALYTICS_ENABLED en el define de Standalone:
            // com.unity.ai.inference lo repone en cada recarga de dominio mientras la analítica
            // del Editor esté activa (Library/PackageCache/com.unity.ai.inference@*/Editor/
            // Analytics/AnalyticsDefineManager.cs); solo compila código de Editor del paquete, su
            // Runtime/ no lo usa, así que no llega al ejecutable. Retirarlo exigiría quitar el
            // paquete de Packages/manifest.json, que D3 excluye.

            var connect = UnityConnectSettingsText;
            Assert.That(connect, Does.Match(@"(?m)^  m_Enabled: 0\b"),
                "Unity Connect sigue deshabilitado: sin transmisión por red a terceros (RNF-08)");
            Assert.That(connect, Does.Match(@"UnityAnalyticsSettings:\s*\n\s+m_Enabled: 0\b"),
                "Unity Analytics sigue deshabilitado (RNF-10)");

            // DEF-SPIKE-01: el rc1 se conectó a Internet y dejó restos en LocalLow y HKCU. Vale todo
            // servicio de Unity del archivo, no solo los dos de arriba: ninguno encendido, y los tres que
            // no se llaman «Enabled» tampoco (el diagnóstico del motor viene encendido de fábrica).
            Assert.That(connect, Does.Not.Match(@"(?m)^\s+m_Enabled: [^0\s]"),
                "ningún servicio de Unity está encendido: Analytics, Insights, Performance, Purchasing, Ads (RNF-10)");
            Assert.That(connect, Does.Match(@"InsightsSettings:\s*\n\s+m_EngineDiagnosticsEnabled: 0\b"),
                "el diagnóstico del motor de Unity (Insights) sigue apagado: ni conexiones ni archivos en LocalLow (RNF-10, RNF-11)");
            Assert.That(connect, Does.Match(@"m_EnableCloudDiagnosticsReporting: 0\b"),
                "los informes de fallos a la nube de Unity siguen apagados (RNF-10)");
            Assert.That(settings, Does.Match(@"enableCrashReportAPI: 0\b"),
                "el ejecutable no guarda ni envía informes de fallos (RNF-10)");
        }

        [Test]
        public void Architecture_RNF02_ElEjecutableNoAlternaPantallaCompletaConAltIntro()
        {
            Assert.That(PlayerSettingsText, Does.Match(@"allowFullscreenSwitch: 0\b"),
                "sin combinaciones de teclas ni controles simultáneos: Alt+Intro no alterna pantalla completa (CT-06, RNF-02)");
        }

        [Test]
        public void Architecture_RF01_ElEjecutableArrancaSinPantallaDePresentacionDeUnity()
        {
            Assert.That(PlayerSettingsText, Does.Match(@"m_ShowUnitySplashScreen: 0\b"),
                "la primera pantalla es la del juego (RF-01), no la presentación de Unity");
        }
    }
}
