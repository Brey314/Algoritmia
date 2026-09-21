using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game.Core;
using NUnit.Framework;

namespace Game.Levels.River.Tests
{
    /// <summary>Los cuatro indicadores del Nivel 3, una prueba por fila de OE1 §3.6.1 (R13).</summary>
    [TestFixture]
    public class RiverIndicatorTests
    {
        private static readonly string[] None = Array.Empty<string>();

        /// <summary>Reloj de prueba: una cola de instantes que repite el último al agotarse.</summary>
        private static Func<float> Clock(params float[] instants)
        {
            var queue = new Queue<float>(instants);
            var last = 0f;
            return () => last = queue.Count > 0 ? queue.Dequeue() : last;
        }

        /// <summary>Confirma sobre la balsa real y se lo cuenta al recolector como lo hace el panel.</summary>
        private static ValidationResult Confirm(RaftAssembly assembly, RiverIndicatorCollector sut)
        {
            var placedBefore = assembly.VisibleSlots
                .Where(slot => assembly.IsOpen(slot) && assembly.PlacedIn(slot.Id).HasValue)
                .Select(slot => slot.Id)
                .ToArray();
            var result = assembly.Confirm();
            sut.RecordConfirmation(result, result.WrongSlotIds.Where(placedBefore.Contains));
            return result;
        }

        [Test]
        [Category("Acceptance")]
        public void RiverIndicators_RF45_IntentosCuentaFasesRechazadasYPruebasFallidas()
        {
            var sut = new RiverIndicatorCollector(Clock(0f));

            sut.RecordConfirmation(new ValidationResult(false, new[] { "tronco_1" }, ""), None); // base rechazada
            sut.RecordConfirmation(new ValidationResult(true, null, ""), None); // base aceptada
            sut.RecordConfirmation(new ValidationResult(false, new[] { "mastil" }, ""), None); // la balsa se hunde
            sut.RecordConfirmation(new ValidationResult(false, new[] { "vela" }, ""), None); // y otra vez

            Assert.That(sut.Complete().Attempts, Is.EqualTo(3));
        }

        [Test]
        [Category("Acceptance")]
        public void RiverIndicators_RF45_PasosUtilizadosNoSuperaTres()
        {
            var assembly = RaftAssemblyTests.Sut();
            var sut = new RiverIndicatorCollector(Clock(0f));

            Confirm(assembly, sut); // la base vacía: rechazada, no es un paso
            RaftAssemblyTests.BuildBase(assembly);
            sut.RecordConfirmation(new ValidationResult(true, null, ""), None);
            RaftAssemblyTests.BuildLashing(assembly);
            sut.RecordConfirmation(new ValidationResult(true, null, ""), None);
            assembly.Place("mastil", MaterialKind.Mast);
            assembly.Place("vela", MaterialKind.Cloth);
            Confirm(assembly, sut);

            Assert.That(sut.Complete().StepsUsed, Is.EqualTo(3));
        }

        [Test]
        [Category("Acceptance")]
        public void RiverIndicators_RF45_RetomarEnUnaFaseCuentaLasConfirmadasAntes()
        {
            // Retomar en el mástil (RNF-14): la base y el amarre ya se aceptaron en otra sesión.
            var sut = new RiverIndicatorCollector(Clock(0f), confirmedPhases: 2);

            sut.RecordConfirmation(new ValidationResult(true, null, ""), None);

            Assert.That(sut.Complete().StepsUsed, Is.EqualTo(3));
        }

        [Test]
        [Category("Acceptance")]
        public void RiverIndicators_RF45_ErrorCorregidoExigeRecolocacionCorrectaPosterior()
        {
            var assembly = RaftAssemblyTests.Sut();
            var sut = new RiverIndicatorCollector(Clock(0f));

            // Un tronco bien, el otro espacio vacío: se rechaza sin devolver nada.
            assembly.Place("tronco_1", MaterialKind.Logs);
            Confirm(assembly, sut);
            assembly.Place("tronco_2", MaterialKind.Logs);
            Confirm(assembly, sut);
            Assert.That(sut.Complete().CorrectedErrors, Is.Zero, "llenar un espacio vacío no corrige una pieza devuelta");

            // Los amarres: soga en un amarre y la tela en el otro. La tela vuelve al inventario.
            assembly.Place("amarre_1", MaterialKind.Ropes);
            assembly.Place("amarre_2", MaterialKind.Cloth);
            Confirm(assembly, sut);
            Confirm(assembly, sut); // sin recolocar nada: la devolución sola no cuenta
            Assert.That(sut.Complete().CorrectedErrors, Is.Zero, "una devolución sin acierto en el intento siguiente no cuenta");

            // Otra vez la tela, devuelta; y ahora sí la soga en el intento siguiente.
            assembly.Place("amarre_2", MaterialKind.Cloth);
            Confirm(assembly, sut);
            assembly.Place("amarre_2", MaterialKind.Ropes);
            Confirm(assembly, sut);
            Assert.That(sut.Complete().CorrectedErrors, Is.EqualTo(1), "la pieza devuelta se recolocó bien en el intento siguiente");
        }

        [Test]
        [Category("Acceptance")]
        public void RiverIndicators_RF07_LaPausaNoSumaTiempoDeResolucion()
        {
            // 0: inicio · 10→15: pausa (5 s) · 20: completación. Transcurrido total = 20.
            var sut = new RiverIndicatorCollector(Clock(0f, 10f, 15f, 20f));

            sut.PauseOpened();
            sut.PauseClosed();

            Assert.That(sut.Complete().ResolutionSeconds, Is.EqualTo(15f).Within(0.001f));
        }

        [Test]
        [Category("Acceptance")]
        public void RiverIndicators_OE1361_ElTiempoDeResolucionEsDeCadaFase()
        {
            // 0: inicio · 10: base confirmada · 25: amarre confirmado. El amarre tardó 15 s, no 25.
            var sut = new RiverIndicatorCollector(Clock(0f, 10f, 25f));

            sut.RecordConfirmation(new ValidationResult(true, null, ""), None);
            sut.Complete();
            sut.RecordConfirmation(new ValidationResult(true, null, ""), None);

            Assert.That(sut.Complete().ResolutionSeconds, Is.EqualTo(15f).Within(0.001f));
        }

        [Test]
        [Category("Acceptance")]
        public void RiverIndicators_OE1361_ReiniciarElNivelNoBorraLosIndicadoresRegistrados()
        {
            var profile = PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;
            var baseId = new PhaseId(LevelId.River, 1);
            var registrados = new RiverIndicatorCollector(Clock(0f, 30f));
            registrados.RecordConfirmation(new ValidationResult(false, new[] { "tronco_1" }, ""), None);
            profile.ConfirmPhase(baseId, registrados.Complete());

            // «Reiniciar» repite la fase con un recolector nuevo; si alguien confirmara otra vez,
            // el registro anterior se conserva (nota 4).
            profile.ConfirmPhase(baseId, new RiverIndicatorCollector(Clock(0f)).Complete());

            Assert.That(profile.IndicatorsFor(baseId).Attempts, Is.EqualTo(1), "intentos");
            Assert.That(profile.IndicatorsFor(baseId).ResolutionSeconds, Is.EqualTo(30f).Within(0.001f), "segundos");
        }

        [Test]
        [Category("Acceptance")]
        public void RiverIndicators_CP03_NingunIndicadorLlegaALaUIDelEstudiante()
        {
            // Mismo barrido que FireIndicatorTests y WheelIndicatorTests: Game.UI no tiene ningún
            // motivo para conocer el recolector del Nivel 3 — ni como campo, ni como propiedad,
            // ni como parámetro.
            var forbidden = typeof(RiverIndicatorCollector);
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
