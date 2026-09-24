// CP-03 y RNF-09 solo se pueden cerrar con el proyecto completo (Slice 4, P11): el primero
// recorre todo el contenido visible al estudiante, y eso exige ver los tres niveles a la vez.
// Es la única razón por la que este assembly de pruebas referencia Game.Levels.Fire, .Wheel y
// .River al mismo tiempo — la regla de RNF-16 (ningún nivel referencia a otro) es sobre los
// assemblies de producción que se distribuyen, no sobre una prueba de cierre que audita el
// contenido entero.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Game.Levels.Fire;
using Game.Levels.River;
using Game.Levels.Wheel;
using Game.Scaffolding;
using Game.UI;
using NUnit.Framework;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Game.Content.Tests
{
    public class ContentInvariantTests
    {
        /// <summary>«{0}», «{1}»…: el índice de formato de un mensaje, no una cifra de desempeño.</summary>
        private static readonly Regex FormatoDeSustitucion = new Regex(@"\{\d+\}");

        /// <summary>
        /// Todo el contenido visible al estudiante. Dos exclusiones deliberadas:
        /// <c>Game.Reporting.ReportContent</c>, la única pantalla donde una cifra es correcta
        /// (RF-46, CP-03), y <c>CreditsContent</c>, que no es retroalimentación de desempeño —
        /// su año de producción no es la cifra que CP-03 prohíbe.
        /// </summary>
        private static readonly System.Type[] TiposDeContenido =
        {
            typeof(LevelSummaryMessages), typeof(NarrativeSequence), typeof(GuideContent),
            typeof(FireMessages), typeof(WheelLevelConfig), typeof(RiverLevelConfig),
            typeof(RaftAssemblyContent), typeof(AssemblyContent), typeof(MazeLayout),
            typeof(GameTitleConfig)
        };

        [Test]
        public void Content_CP03_NingunTextoVisibleAlEstudianteContieneCifrasDeDesempeno()
        {
            var infracciones = new List<string>();

            foreach (var tipo in TiposDeContenido)
            {
                foreach (var guid in AssetDatabase.FindAssets($"t:{tipo.Name}"))
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var asset = AssetDatabase.LoadAssetAtPath(path, tipo);
                    if (asset == null)
                    {
                        continue;
                    }

                    foreach (var (campo, texto) in Textos(asset, tipo, 0))
                    {
                        // El índice de un mensaje con sustitución («Troncos redondos: {0} de
                        // {1}», RF-24) no es una cifra: es sintaxis de formato, no dato.
                        var sinFormato = FormatoDeSustitucion.Replace(texto, string.Empty);
                        if (sinFormato.Any(char.IsDigit))
                        {
                            infracciones.Add($"{tipo.Name}.{campo} en {path}: «{texto}»");
                        }
                    }
                }
            }

            Assert.That(infracciones, Is.Empty,
                "CP-03: el estudiante no puede ver una cifra de desempeño en ninguna pantalla.\n" +
                string.Join("\n", infracciones));
        }

        /// <summary>
        /// Recorre el grafo serializado buscando texto, sin entrar en referencias a otro
        /// <see cref="Object"/> (Sprite, AudioClip, otro asset) ni en identificadores técnicos
        /// —cualquier campo que termine en «Id», como <c>NextSequenceId</c> o el <c>Id</c> de un
        /// objeto del catálogo—: son claves internas, nunca texto que el estudiante lea.
        /// </summary>
        private static IEnumerable<(string Campo, string Texto)> Textos(object value, System.Type declaringType, int depth)
        {
            if (value == null || depth > 6)
            {
                yield break;
            }

            foreach (var propiedad in declaringType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (propiedad.GetIndexParameters().Length > 0 || propiedad.Name.EndsWith("Id"))
                {
                    continue;
                }

                foreach (var texto in ValoresDeTexto(propiedad.GetValue(value), depth))
                {
                    yield return (propiedad.Name, texto);
                }
            }
        }

        private static IEnumerable<string> ValoresDeTexto(object value, int depth)
        {
            if (value == null || depth > 6)
            {
                yield break;
            }

            if (value is string s)
            {
                yield return s;
                yield break;
            }

            if (value is Object)
            {
                yield break; // Sprite, AudioClip, otro asset: referencia, no contenido propio.
            }

            if (value is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    foreach (var texto in ValoresDeTexto(item, depth + 1))
                    {
                        yield return texto;
                    }
                }

                yield break;
            }

            var tipo = value.GetType();
            if (tipo.IsPrimitive || tipo.IsEnum || tipo == typeof(decimal))
            {
                yield break;
            }

            // System.*/UnityEngine.* (Vector2, Rect, Color…) no llevan texto: cortar ahí evita
            // recorrer campos internos que no son de este proyecto.
            if (tipo.Namespace != null && (tipo.Namespace.StartsWith("System") || tipo.Namespace.StartsWith("UnityEngine")))
            {
                yield break;
            }

            foreach (var (_, texto) in Textos(value, tipo, depth + 1))
            {
                yield return texto;
            }
        }
    }
}
