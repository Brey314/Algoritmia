using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game.Core;
using NUnit.Framework;
using UnityEngine;

namespace Game.Levels.Fire.Tests
{
    [TestFixture]
    public class FireIndicatorTests
    {
        private readonly List<UnityEngine.Object> _configs = new List<UnityEngine.Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (var config in _configs)
            {
                UnityEngine.Object.DestroyImmediate(config);
            }

            _configs.Clear();
        }

        private FireLevelConfig CreateConfig(int minimumEffectiveStrikes)
        {
            var config = ScriptableObject.CreateInstance<FireLevelConfig>();
            config.MinimumEffectiveStrikes = minimumEffectiveStrikes;
            _configs.Add(config);
            return config;
        }

        private FireIndicatorCollector CreateSystemUnderTest(int minimumEffectiveStrikes, Func<float> now) =>
            new FireIndicatorCollector(CreateConfig(minimumEffectiveStrikes), now);

        /// <summary>Reloj de prueba: una cola de instantes: se agota repitiendo el último para que
        /// llamadas de más a <see cref="FireIndicatorCollector.Complete"/> no exploten.</summary>
        private static Func<float> Clock(params float[] instants)
        {
            var queue = new Queue<float>(instants);
            var last = 0f;
            return () => last = queue.Count > 0 ? queue.Dequeue() : last;
        }

        [Test]
        [Category("Acceptance")]
        public void FireIndicators_RF45_IntentosCuentaSoloGolpesNoEfectivos()
        {
            var sut = CreateSystemUnderTest(5, Clock(0f));

            sut.RecordStrike(StrikeOutcome.SparksDied(StrikePosition.Far));
            Assert.That(sut.Complete().Attempts, Is.EqualTo(1), "el primer golpe no efectivo suma un intento");

            sut.RecordStrike(StrikeOutcome.SparksDied(StrikePosition.Near));
            Assert.That(sut.Complete().Attempts, Is.EqualTo(2), "cada golpe no efectivo suma uno más, uno por uno");

            sut.RecordStrike(StrikeOutcome.SparkLanded(1));
            Assert.That(sut.Complete().Attempts, Is.EqualTo(2), "un golpe efectivo posterior no suma a Intentos");
        }

        [Test]
        [Category("Acceptance")]
        public void FireIndicators_RF45_ErrorCorregidoExigeCambioDePosicionSeguidoDeAcierto()
        {
            var sut = CreateSystemUnderTest(10, Clock(0f));

            sut.RecordStrike(StrikeOutcome.SparkLanded(1));
            sut.RecordStrike(StrikeOutcome.SparkLanded(2));
            Assert.That(sut.Complete().CorrectedErrors, Is.Zero,
                "aciertos consecutivos, sin fallo justo antes, no cuentan como error corregido");

            sut.RecordStrike(StrikeOutcome.SparksDied(StrikePosition.Far));
            sut.RecordStrike(StrikeOutcome.SparksDied(StrikePosition.Near));
            sut.RecordStrike(StrikeOutcome.SparksDied(StrikePosition.Far));
            sut.RecordStrike(StrikeOutcome.SparkLanded(3));
            Assert.That(sut.Complete().CorrectedErrors, Is.EqualTo(1),
                "varios fallos seguidos antes del acierto solo suman un error corregido (RF-45)");
        }

        [Test]
        [Category("Acceptance")]
        public void FireIndicators_RF07_LaPausaNoSumaTiempoDeResolucion()
        {
            // 0: inicio · 10→15: ventana de pausa (5s) · 20: convergencia. Elapsed total = 20.
            var sut = CreateSystemUnderTest(3, Clock(0f, 10f, 15f, 20f));

            sut.PauseOpened();
            sut.PauseClosed();

            var actual = sut.Complete();

            Assert.That(actual.ResolutionSeconds, Is.EqualTo(15f).Within(0.001f),
                "los 5 segundos de pausa se excluyen de los 20 transcurridos, no el resto (RF-07)");
        }

        [Test]
        public void FireIndicators_RF45_PasosUtilizadosSeCongelaAlCruzarElMinimo()
        {
            var sut = CreateSystemUnderTest(3, Clock(0f));

            sut.RecordStrike(StrikeOutcome.SparkLanded(1));
            sut.RecordStrike(StrikeOutcome.SparkLanded(2));
            sut.RecordStrike(StrikeOutcome.SparkLanded(3)); // cruza el mínimo (3) aquí
            Assert.That(sut.Complete().StepsUsed, Is.EqualTo(3),
                "se congela en los golpes efectivos acumulados al cruzar el mínimo");

            sut.RecordStrike(StrikeOutcome.SparkLanded(4)); // golpe efectivo posterior
            Assert.That(sut.Complete().StepsUsed, Is.EqualTo(3),
                "un golpe efectivo posterior no mueve Pasos utilizados");
        }

        [Test]
        public void FireIndicators_RF45_PerformanceIndicatorsExponeExactamenteLosCuatroCampos()
        {
            var properties = typeof(PerformanceIndicators)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(property => property.Name);

            Assert.That(properties, Is.EquivalentTo(new[]
            {
                nameof(PerformanceIndicators.Attempts),
                nameof(PerformanceIndicators.CorrectedErrors),
                nameof(PerformanceIndicators.StepsUsed),
                nameof(PerformanceIndicators.ResolutionSeconds)
            }), "la lista de indicadores es cerrada: ninguno adicional (RNF-09 nota 5)");
        }

        [Test]
        [Category("Acceptance")]
        public void FireIndicators_CP03_NingunIndicadorLlegaALaUIDelEstudiante()
        {
            // ILevelReporter/FireIndicatorCollector: prohibidos en cualquier forma — Game.UI no
            // tiene ningún motivo legítimo para conocer el recolector de Fire (T17).
            var forbiddenEverywhere = new[] { typeof(ILevelReporter), typeof(FireIndicatorCollector) };

            // PerformanceIndicators: prohibido como campo, propiedad o retorno —eso dejaría leer
            // el struct crudo desde fuera—, pero no como parámetro de entrada.
            // LevelSummaryComposer.Compose(messages, indicators) (T18) lo recibe a propósito para
            // componer el resumen sin cifras; lo que CP-03 prohíbe es que un dígito llegue a
            // pantalla, no que Game.UI toque el tipo — eso ya lo prueba
            // LevelSummary_RF45_NoContieneNingunDigito, dueño de esa garantía.
            var forbiddenExceptAsParameter = new[] { typeof(PerformanceIndicators) };

            var uiAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .Single(assembly => assembly.GetName().Name == "Game.UI");

            const BindingFlags members = BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

            var forbiddenAsData = forbiddenEverywhere.Concat(forbiddenExceptAsParameter).ToArray();

            var offenders = new List<string>();
            foreach (var type in uiAssembly.GetTypes())
            {
                foreach (var field in type.GetFields(members).Where(IsVisible))
                {
                    if (forbiddenAsData.Contains(field.FieldType))
                    {
                        offenders.Add($"{type.FullName}.{field.Name} (campo)");
                    }
                }

                foreach (var property in type.GetProperties(members))
                {
                    var accessor = property.GetMethod ?? property.SetMethod;
                    if (accessor != null && IsVisible(accessor) && forbiddenAsData.Contains(property.PropertyType))
                    {
                        offenders.Add($"{type.FullName}.{property.Name} (propiedad)");
                    }
                }

                foreach (var method in type.GetMethods(members).Where(IsVisible))
                {
                    if (forbiddenAsData.Contains(method.ReturnType))
                    {
                        offenders.Add($"{type.FullName}.{method.Name} (retorno)");
                    }

                    foreach (var parameter in method.GetParameters())
                    {
                        if (forbiddenEverywhere.Contains(parameter.ParameterType))
                        {
                            offenders.Add($"{type.FullName}.{method.Name}({parameter.Name}) (parámetro)");
                        }
                    }
                }
            }

            Assert.That(offenders, Is.Empty,
                "CP-03: ningún indicador puede llegar a la UI del estudiante: " + string.Join(", ", offenders));
        }

        private static bool IsVisible(FieldInfo field) =>
            field.IsPublic || field.IsAssembly || field.IsFamilyOrAssembly;

        private static bool IsVisible(MethodBase method) =>
            method.IsPublic || method.IsAssembly || method.IsFamilyOrAssembly;
    }
}
