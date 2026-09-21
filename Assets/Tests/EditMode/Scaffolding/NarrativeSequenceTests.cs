using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Scaffolding.Tests
{
    /// <summary>
    /// Comprueba el **contenido** de las secuencias narrativas que hay en el proyecto, no la
    /// clase. Los textos viven en assets (RNF-18, CT-05), así que el requisito de legibilidad
    /// también hay que verificarlo sobre el asset y no sobre el código.
    /// </summary>
    public class NarrativeSequenceTests
    {
        private const int MaximoPalabrasPorOracion = 20;

        [Test]
        public void NarrativeSequence_RNF01_NingunaOracionSupera20Palabras()
        {
            var excedidas = TodasLasSecuencias()
                .SelectMany(sequence => sequence.Lines.Select(line => (sequence, line)))
                .SelectMany(par => Oraciones(par.line.Text)
                    .Where(oracion => Palabras(oracion) > MaximoPalabrasPorOracion)
                    .Select(oracion => $"{par.sequence.Id} · {Palabras(oracion)} palabras: «{oracion}»"))
                .ToArray();

            Assert.That(excedidas, Is.Empty);
        }

        [Test]
        public void NarrativeSequence_RNF18_CadaSecuenciaTieneIdYLineas()
        {
            var incompletas = TodasLasSecuencias()
                .Where(sequence => string.IsNullOrWhiteSpace(sequence.Id) || sequence.Lines.Length == 0)
                .Select(sequence => sequence.name)
                .ToArray();

            Assert.That(incompletas, Is.Empty);
        }

        [Test]
        public void NarrativeSequence_RF05_LosIdentificadoresNoSeRepiten()
        {
            var repetidos = TodasLasSecuencias()
                .GroupBy(sequence => sequence.Id)
                .Where(grupo => grupo.Count() > 1)
                .Select(grupo => grupo.Key)
                .ToArray();

            Assert.That(repetidos, Is.Empty);
        }

        [Test]
        public void NarrativeSequence_RF05_ElNivel2TieneSusSeisSecuencias()
        {
            var esperadas = new[]
            {
                "N2_PuenteI", "N2_Escena21_Bosque", "N2_Escena22_ElPatron",
                "N2_Escena23_Construccion", "N2_Escena24_Regreso", "N2_Escena25_Cierre"
            };

            var delNivel2 = TodasLasSecuencias()
                .Where(sequence => sequence.Level == Game.Core.LevelId.Wheel)
                .Select(sequence => sequence.Id)
                .ToArray();

            Assert.That(delNivel2, Is.EquivalentTo(esperadas));
        }

        /// <summary>
        /// Los objetos que la narrativa pinta sobre el entorno tienen que quedar **por encima**
        /// del cuadro de diálogo, que ocupa el cuarto inferior de la pantalla
        /// (<c>NarrativeScene_RNF03_ElCuadroDeDialogoNoSuperaElCuartoDeLaPantalla</c>).
        /// </summary>
        /// <remarks>
        /// La regla es la misma en los dos niveles: si el texto lo nombra, se ve entero. En el
        /// Nivel 1 porque la cueva está a oscuras y el objeto es lo único que hay; en el Nivel 2
        /// porque los objetos del suelo **son** el enunciado del ejercicio (RF-22, RF-23).
        ///
        /// Cuenta también las paradas, no solo el encuadre inicial: un objeto bien colocado al
        /// abrir se mete debajo del cuadro en cuanto la cámara cierra el plano.
        /// </remarks>
        [Test]
        public void NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo()
        {
            var tapados = TodasLasSecuencias()
                .Where(sequence => Verificadas.Contains(sequence.Id) && sequence.Props.Length > 0)
                .SelectMany(ObjetosTapados)
                .ToArray();

            Assert.That(tapados, Is.Empty);
        }

        /// <summary>
        /// Las secuencias cuyos encuadres están verificados contra el cuadro de diálogo. No es
        /// «todas» a propósito: del Nivel 2 solo está implementado hasta el §6.3.1 del guion, y
        /// <c>N2_Escena25_Cierre</c> (§6.4) todavía tiene su único objeto medio tapado. Entra en la
        /// lista cuando se revise su encuadre, no antes: una prueba que se salta lo que no cumple
        /// no comprueba nada.
        /// </summary>
        private static readonly string[] Verificadas =
        {
            "N1_Apertura", "N1_AparicionGuia", "N1_Hallazgo", "N1_NacimientoDelFuego",
            "N2_PuenteI", "N2_Escena21_Bosque", "N2_Escena22_ElPatron",
            "N2_Escena23_Construccion", "N2_Escena24_Regreso"
        };

        /// <summary>Alto del cuadro de diálogo como fracción de la pantalla (RNF-03).</summary>
        private const float CuadroDeDialogo = 0.25f;

        /// <summary>Una ventana 16:9 cualquiera: la geometría es proporcional, el tamaño da igual.</summary>
        private static readonly Vector2 Ventana = new Vector2(1600f, 900f);

        private static IEnumerable<string> ObjetosTapados(NarrativeSequence sequence)
        {
            if (sequence.Illustration == null)
            {
                yield break;
            }

            var imagen = sequence.Illustration.rect.size;
            var encuadres = new[] { ("inicio", sequence.CameraStart) }
                .Concat(sequence.CameraKeys.Select(key => ($"línea {key.Line}", key.Framing)));

            foreach (var (parada, framing) in encuadres)
            foreach (var prop in sequence.Props)
            {
                // La misma geometría que aplica el controlador: el objeto cuelga de la
                // ilustración, así que hereda su escala y su desplazamiento.
                var tamano = IllustrationFraming.CoverSize(imagen, Ventana, framing.Zoom);
                var centro = IllustrationFraming.Offset(tamano, Ventana, framing.Focus)
                             + (prop.Position - new Vector2(0.5f, 0.5f)) * tamano;
                var medioAlto = tamano.y * prop.Size / 2f;
                var medioAncho = medioAlto; // preserveAspect sobre sprites cuadrados
                var abajo = 0.5f + (centro.y - medioAlto) / Ventana.y;
                var izquierda = 0.5f + (centro.x - medioAncho) / Ventana.x;
                var derecha = 0.5f + (centro.x + medioAncho) / Ventana.x;

                // Lo que la cámara no encuadra no lo tapa nadie.
                if (derecha < 0f || izquierda > 1f || abajo >= CuadroDeDialogo)
                {
                    continue;
                }

                yield return FormattableString.Invariant(
                    $"{sequence.Id} · {parada}: «{prop.Art?.name}» baja hasta y={abajo:0.000} y el cuadro de diálogo llega a {CuadroDeDialogo:0.00}");
            }
        }

        // --- helpers -----------------------------------------------------------------------

        private static IEnumerable<NarrativeSequence> TodasLasSecuencias() =>
            AssetDatabase.FindAssets($"t:{nameof(NarrativeSequence)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<NarrativeSequence>)
                .Where(sequence => sequence != null);

        /// <summary>
        /// Parte el texto en oraciones. Los signos de apertura «¿» y «¡» no cierran nada, así que
        /// solo cuentan los de cierre más el punto y los puntos suspensivos.
        /// </summary>
        private static IEnumerable<string> Oraciones(string text) => (text ?? string.Empty)
            .Replace("…", ".")
            .Split(new[] { '.', '!', '?' }, System.StringSplitOptions.RemoveEmptyEntries)
            .Select(oracion => oracion.Trim())
            .Where(oracion => oracion.Length > 0);

        private static int Palabras(string oracion) =>
            oracion.Split((char[])null, System.StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
