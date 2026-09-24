using System;
using System.Linq;
using System.Reflection;
using Game.Core;
using NUnit.Framework;

namespace Game.Reporting.Tests
{
    public class IndicatorReportTests
    {
        private static PlayerProfile NuevoPerfil() =>
            PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;

        [Test]
        public void IndicatorReport_INC35_AgrupaPorNivelYPorFaseNoSoloPorNivel()
        {
            var profile = NuevoPerfil();
            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 1), new PerformanceIndicators(1, 0, 3, 10f));
            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 2), new PerformanceIndicators(2, 1, 4, 20f));
            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 3), new PerformanceIndicators(3, 2, 5, 30f));

            var wheel = IndicatorReport.For(profile).Single(section => section.Level == LevelId.Wheel);

            Assert.That(wheel.Phases.Count, Is.EqualTo(3), "el Nivel 2 son tres fases, no un total");
            Assert.That(wheel.Phases[0].Indicators.Attempts, Is.EqualTo(1));
            Assert.That(wheel.Phases[1].Indicators.Attempts, Is.EqualTo(2));
            Assert.That(wheel.Phases[2].Indicators.Attempts, Is.EqualTo(3));
        }

        [Test]
        public void IndicatorReport_RNF09_EmiteExactamenteLosCuatroIndicadoresYNingunAgregadoNuevo()
        {
            // La lista es cerrada (§3.6.1): nada de promedios, porcentajes, totales ni
            // «nivel de dominio» colgado del tipo que transporta los indicadores.
            var propiedades = typeof(PerformanceIndicators)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(property => property.Name);

            Assert.That(propiedades, Is.EquivalentTo(new[]
            {
                "Attempts", "CorrectedErrors", "StepsUsed", "ResolutionSeconds"
            }));
        }

        [Test]
        public void IndicatorReport_RF46_UnNivelNoJugadoSePresentaSinDatosNoConCeros()
        {
            var profile = NuevoPerfil();
            profile.ConfirmPhase(new PhaseId(LevelId.Fire, 1), new PerformanceIndicators(2, 1, 3, 15f));

            var river = IndicatorReport.For(profile).Single(section => section.Level == LevelId.River);

            Assert.That(river.Played, Is.False, "RF-46: cero intentos y no haber jugado no son lo mismo");
            Assert.That(river.Phases, Has.All.Matches<PhaseIndicators>(phase => !phase.Played));
        }
    }
}
