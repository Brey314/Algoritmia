using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core;
using NUnit.Framework;
using UnityEditor;

namespace Game.Scaffolding.Tests
{
    public class HintPolicyTests
    {
        private const int MaximoPalabrasPorOracion = 20;

        private static readonly GuideStep Golpear = new GuideStep(
            "Golpear",
            "Acerca las piedras al montón de hojas y golpéalas.",
            "¿Cuánto camino recorren las chispas antes de tocar las hojas?");

        private static readonly GuideStep Soplar = new GuideStep(
            "Soplar",
            "Sopla despacio sobre el humo.",
            "¿Qué necesita el humo para volverse llama?");

        [Test]
        public void HintPolicy_RF13_AyudaADemandaNoAlteraElEstado()
        {
            var sut = new HintPolicy(Golpear);
            sut.RegisterFailedAttempt();
            sut.RegisterFailedAttempt();

            Assert.That(sut.RequestHelp(), Is.EqualTo(Golpear.Instruction), "repite la instrucción vigente");
            Assert.That(sut.ConsecutiveFailures, Is.EqualTo(2), "pedir ayuda no cuenta como intento (HU-03 FA-02)");
            Assert.That(sut.RegisterFailedAttempt(), Is.EqualTo(Golpear.Hint), "el siguiente fallo sigue siendo el tercero");
        }

        [Test]
        public void HintPolicy_RF13_PistaSeOfreceAlTercerFalloConsecutivo()
        {
            var sut = new HintPolicy(Golpear);

            Assert.That(sut.RegisterFailedAttempt(), Is.Null, "primer fallo: todavía no hay pista");
            Assert.That(sut.RegisterFailedAttempt(), Is.Null, "segundo fallo: todavía no hay pista");
            Assert.That(sut.RegisterFailedAttempt(), Is.EqualTo(Golpear.Hint), "tercer fallo consecutivo");
            Assert.That(sut.ConsecutiveFailures, Is.Zero, "ofrecida la pista, la cuenta vuelve a empezar (guion §4.3.5 E5)");
        }

        [Test]
        public void HintPolicy_RF13_UnAciertoReiniciaLosFallosConsecutivos()
        {
            var sut = new HintPolicy(Golpear);
            sut.RegisterFailedAttempt();
            sut.RegisterFailedAttempt();

            sut.RegisterSuccessfulAttempt();

            Assert.That(sut.ConsecutiveFailures, Is.Zero);
            Assert.That(sut.RegisterFailedAttempt(), Is.Null, "los fallos anteriores ya no cuentan");
            Assert.That(sut.RegisterFailedAttempt(), Is.Null);
            Assert.That(sut.RegisterFailedAttempt(), Is.EqualTo(Golpear.Hint), "tres fallos seguidos otra vez");
        }

        [Test]
        public void HintPolicy_RNF03_CambiarDeTareaReiniciaLosFallosYLaInstruccion()
        {
            var sut = new HintPolicy(Golpear);
            sut.RegisterFailedAttempt();
            sut.RegisterFailedAttempt();

            sut.Activate(Soplar);

            Assert.That(sut.ConsecutiveFailures, Is.Zero, "los tres fallos son «en una misma tarea» (RF-13)");
            Assert.That(sut.RequestHelp(), Is.EqualTo(Soplar.Instruction), "la ayuda es de la tarea activa, no del objetivo del nivel");
        }

        // --- contenido de los assets, no de la clase ---------------------------------------
        // Los textos viven fuera del código (CT-05, RNF-18), así que CP-06 y RNF-01 hay que
        // verificarlos sobre el asset. Mismo criterio que en NarrativeSequenceTests.

        [Test]
        public void HintPolicy_CP06_LaPistaNuncaNombraLaPosicionEfectiva()
        {
            var resuelven = PasosDelNivel(LevelId.Fire)
                .Where(paso => Menciona(paso.Hint, "muy cerca"))
                .Select(paso => $"{paso.Id}: «{paso.Hint}»")
                .ToArray();

            Assert.That(resuelven, Is.Empty, "la pista orienta, no entrega la posición efectiva (guion §4.3.6)");
        }

        [Test]
        public void HintPolicy_RNF18_CadaTareaTieneInstruccionYPista()
        {
            var incompletas = TodosLosPasos()
                .Where(paso => string.IsNullOrWhiteSpace(paso.Id) ||
                               string.IsNullOrWhiteSpace(paso.Instruction) ||
                               string.IsNullOrWhiteSpace(paso.Hint))
                .Select(paso => paso.Id)
                .ToArray();

            Assert.That(incompletas, Is.Empty);
        }

        [Test]
        public void HintPolicy_RNF01_NingunaOracionDelGuiaSupera20Palabras()
        {
            var excedidas = TodosLosPasos()
                .SelectMany(paso => new[] { paso.Instruction, paso.Hint }
                    .SelectMany(Oraciones)
                    .Where(oracion => Palabras(oracion) > MaximoPalabrasPorOracion)
                    .Select(oracion => $"{paso.Id} · {Palabras(oracion)} palabras: «{oracion}»"))
                .ToArray();

            Assert.That(excedidas, Is.Empty);
        }

        // --- helpers -----------------------------------------------------------------------

        private static IEnumerable<GuideContent> TodoElContenido() =>
            AssetDatabase.FindAssets($"t:{nameof(GuideContent)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<GuideContent>)
                .Where(content => content != null);

        private static IEnumerable<GuideStep> TodosLosPasos() =>
            TodoElContenido().SelectMany(content => content.Steps);

        private static IEnumerable<GuideStep> PasosDelNivel(LevelId level) =>
            TodoElContenido().Where(content => content.Level == level).SelectMany(content => content.Steps);

        private static bool Menciona(string texto, string termino) =>
            (texto ?? string.Empty).IndexOf(termino, StringComparison.OrdinalIgnoreCase) >= 0;

        /// <summary>
        /// Parte el texto en oraciones. Los signos de apertura «¿» y «¡» no cierran nada, así que
        /// solo cuentan los de cierre más el punto y los puntos suspensivos.
        /// </summary>
        private static IEnumerable<string> Oraciones(string text) => (text ?? string.Empty)
            .Replace("…", ".")
            .Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(oracion => oracion.Trim())
            .Where(oracion => oracion.Length > 0);

        private static int Palabras(string oracion) =>
            oracion.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
