using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Game.EditorTools
{
    /// <summary>
    /// Copia los avisos de licencia de terceros junto al ejecutable: la MIT de Phosphor Icons y la
    /// OFL de las fuentes Nunito y Baloo 2 exigen que el aviso acompañe a lo que se distribuye.
    /// </summary>
    /// <remarks>
    /// Los archivos viven en <c>Assets/</c> pero no entran al build (no son recursos ni están en
    /// una escena), así que sin este paso el entregable portable los dejaría atrás (RNF-23).
    /// La lógica es <see cref="CopyTo"/>; el callback solo la llama con la carpeta del build.
    /// </remarks>
    public class LicenseNotices : IPostprocessBuildWithReport
    {
        internal const string FolderName = "Licencias";

        /// <summary>Rutas de los avisos relativas a la carpeta del proyecto (la que contiene <c>Assets/</c>).</summary>
        internal static readonly string[] Sources =
        {
            "Assets/Game/Art/UI/Common/LICENSE-Phosphor.txt",
            "Assets/Game/Art/Fonts/OFL.txt"
        };

        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.StandaloneWindows64) return;
            var buildDir = Path.GetDirectoryName(report.summary.outputPath);
            CopyTo(buildDir, Path.GetDirectoryName(Application.dataPath));
        }

        /// <summary>
        /// Copia cada aviso a <c>&lt;buildDir&gt;/Licencias/</c>, sobrescribiendo. Si falta uno
        /// falla con excepción: un entregable sin su licencia no debe pasar por bueno.
        /// </summary>
        public static void CopyTo(string buildDir, string projectRoot)
        {
            var target = Path.Combine(buildDir, FolderName);
            Directory.CreateDirectory(target);
            foreach (var relative in Sources)
            {
                var source = Path.Combine(projectRoot, relative);
                File.Copy(source, Path.Combine(target, Path.GetFileName(source)), overwrite: true);
            }
        }
    }
}
