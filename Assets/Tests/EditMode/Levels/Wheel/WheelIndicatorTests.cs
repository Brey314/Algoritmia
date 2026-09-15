using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game.Core;
using NUnit.Framework;

namespace Game.Levels.Wheel.Tests
{
    /// <summary>Los cuatro indicadores del Nivel 2, una prueba por celda de OE1 §3.6.1 (W15).</summary>
    [TestFixture]
    public class WheelIndicatorTests
    {
        /// <summary>Reloj de prueba: una cola de instantes que repite el último al agotarse.</summary>
        private static Func<float> Clock(params float[] instants)
        {
            var queue = new Queue<float>(instants);
            var last = 0f;
            return () => last = queue.Count > 0 ? queue.Dequeue() : last;
        }

        [Test]
        [Category("Acceptance")]
        public void WheelIndicators_RF45_IntentosSeCuentanSegunLaDefinicionDeCadaFase()
        {
            var bosque = new WheelIndicatorCollector(1, Clock(0f));
            bosque.RecordRejected(); // una piedra
            bosque.RecordAccepted(); // un tronco redondo
            bosque.RecordRejected(); // una rama
            Assert.That(bosque.Complete().Attempts, Is.EqualTo(2), "fase 1: selecciones de un objeto no válido");

            var taller = new WheelIndicatorCollector(2, Clock(0f));
            taller.RecordRejected(); // la tabla antes del eje
            taller.RecordAccepted();
            Assert.That(taller.Complete().Attempts, Is.EqualTo(1), "fase 2: acciones rechazadas por estar fuera de secuencia");

            var laberinto = new WheelIndicatorCollector(3, Clock(0f));
            laberinto.RecordExecution(reachedGoal: false, blocks: 2);
            laberinto.RecordExecution(reachedGoal: false, blocks: 3);
            laberinto.RecordExecution(reachedGoal: true, blocks: 4);
            Assert.That(laberinto.Complete().Attempts, Is.EqualTo(2), "fase 3: ejecuciones que no alcanzan el refugio");
        }

        [Test]
        [Category("Acceptance")]
        public void WheelIndicators_RF45_ErrorCorregidoEnFases1Y2EsUnRechazoSeguidoDeLaAccionCorrecta()
        {
            var sut = new WheelIndicatorCollector(1, Clock(0f));

            sut.RecordAccepted();
            sut.RecordAccepted();
            Assert.That(sut.Complete().CorrectedErrors, Is.Zero, "aciertos seguidos no corrigen nada");

            sut.RecordRejected();
            sut.RecordRejected();
            sut.RecordAccepted();
            Assert.That(sut.Complete().CorrectedErrors, Is.EqualTo(1),
                "varios rechazos seguidos y un acierto suman un solo error corregido");
        }

        [Test]
        [Category("Acceptance")]
        public void WheelIndicators_RF45_ErrorCorregidoEnFase3ExigeEdicionEntreDosEjecuciones()
        {
            var sut = new WheelIndicatorCollector(3, Clock(0f));

            sut.RecordEdit(); // componer antes de ejecutar no corrige nada
            sut.RecordExecution(reachedGoal: false, blocks: 2);
            sut.RecordExecution(reachedGoal: false, blocks: 2);
            Assert.That(sut.Complete().CorrectedErrors, Is.Zero,
                "volver a ejecutar sin tocar la secuencia no es un error corregido");

            sut.RecordEdit(); // se retira un bloque
            sut.RecordEdit(); // y se reordena otro
            sut.RecordExecution(reachedGoal: true, blocks: 3);
            Assert.That(sut.Complete().CorrectedErrors, Is.EqualTo(2),
                "cada bloque retirado o reordenado entre la ejecución fallida y la siguiente cuenta");
        }

        [Test]
        [Category("Acceptance")]
        public void WheelIndicators_RF45_PasosUtilizadosSigueLaDefinicionDeCadaFase()
        {
            var bosque = new WheelIndicatorCollector(1, Clock(0f));
            bosque.RecordAccepted();
            bosque.RecordAccepted();
            Assert.That(bosque.Complete().StepsUsed, Is.Zero,
                "fase 1: §3.6.1 no define los pasos del bosque; se emite «no aplica» (pregunta abierta 1)");

            var taller = new WheelIndicatorCollector(2, Clock(0f));
            for (var i = 0; i < 5; i++)
            {
                taller.RecordAccepted();
            }

            taller.RecordRejected();
            Assert.That(taller.Complete().StepsUsed, Is.EqualTo(5), "fase 2: acciones de ensamblaje ejecutadas en orden");

            var laberinto = new WheelIndicatorCollector(3, Clock(0f));
            laberinto.RecordExecution(reachedGoal: false, blocks: 6);
            laberinto.RecordExecution(reachedGoal: true, blocks: 4);
            Assert.That(laberinto.Complete().StepsUsed, Is.EqualTo(4), "fase 3: bloques de la secuencia que llegó al refugio");
        }

        [Test]
        [Category("Acceptance")]
        public void WheelIndicators_RF07_LaPausaNoSumaTiempoDeResolucion()
        {
            // 0: inicio · 10→15: pausa (5 s) · 20: completación. Transcurrido total = 20.
            var sut = new WheelIndicatorCollector(2, Clock(0f, 10f, 15f, 20f));

            sut.PauseOpened();
            sut.PauseClosed();

            Assert.That(sut.Complete().ResolutionSeconds, Is.EqualTo(15f).Within(0.001f),
                "los 5 segundos con la pausa abierta se excluyen (§3.6.1 nota 1)");
        }

        [Test]
        [Category("Acceptance")]
        public void WheelIndicators_OE1361_ReiniciarElNivelNoBorraLosIndicadoresRegistrados()
        {
            var profile = PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;
            var fase1 = new PhaseId(LevelId.Wheel, 1);
            var registrados = new WheelIndicatorCollector(1, Clock(0f, 30f));
            registrados.RecordRejected();
            profile.ConfirmPhase(fase1, registrados.Complete());

            // «Reiniciar» repite la fase con un recolector nuevo (PauseMenuPolicy no toca el
            // perfil); si alguien confirmara otra vez, el registro anterior se conserva (nota 4).
            profile.ConfirmPhase(fase1, new WheelIndicatorCollector(1, Clock(0f)).Complete());

            Assert.That(profile.IndicatorsFor(fase1).Attempts, Is.EqualTo(1));
            Assert.That(profile.IndicatorsFor(fase1).ResolutionSeconds, Is.EqualTo(30f).Within(0.001f));
        }

        [Test]
        [Category("Acceptance")]
        public void WheelIndicators_CP03_NingunIndicadorLlegaALaUIDelEstudiante()
        {
            // Mismo barrido que FireIndicatorTests: Game.UI no tiene ningún motivo para conocer el
            // recolector del Nivel 2 — ni como campo, ni como propiedad, ni como parámetro.
            var forbidden = typeof(WheelIndicatorCollector);
            var uiAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .Single(assembly => assembly.GetName().Name == "Game.UI");

            const BindingFlags members = BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

            var offenders = new List<string>();
            foreach (var type in uiAssembly.GetTypes())
            {
                offenders.AddRange(type.GetFields(members)
                    .Where(field => field.FieldType == forbidden)
                    .Select(field => $"{type.FullName}.{field.Name} (campo)"));
                offenders.AddRange(type.GetProperties(members)
                    .Where(property => property.PropertyType == forbidden)
                    .Select(property => $"{type.FullName}.{property.Name} (propiedad)"));
                foreach (var method in type.GetMethods(members))
                {
                    if (method.ReturnType == forbidden
                        || method.GetParameters().Any(parameter => parameter.ParameterType == forbidden))
                    {
                        offenders.Add($"{type.FullName}.{method.Name}");
                    }
                }
            }

            Assert.That(offenders, Is.Empty, "CP-03: " + string.Join(", ", offenders));
        }
    }
}
