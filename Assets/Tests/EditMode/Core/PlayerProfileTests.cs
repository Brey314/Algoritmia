using System;
using System.Linq;

using NUnit.Framework;

namespace Game.Core.Tests
{
    public class PlayerProfileTests
    {
        private static readonly string[] NoExistingProfiles = Array.Empty<string>();

        [Test]
        public void PlayerProfile_RF02_RechazaNombreVacioYDuplicado()
        {
            // HU-01 FA-01: campo obligatorio. Resultado tipado, no excepción.
            var empty = PlayerProfile.Create("   ", NoExistingProfiles);
            Assert.That(empty.Succeeded, Is.False);
            Assert.That(empty.Result, Is.EqualTo(ProfileCreationResult.Status.EmptyName));

            // HU-01 FA-02: nombre ya existente.
            var duplicate = PlayerProfile.Create("Ana", new[] { "Ana" });
            Assert.That(duplicate.Succeeded, Is.False);
            Assert.That(duplicate.Result, Is.EqualTo(ProfileCreationResult.Status.DuplicateName));

            var accepted = PlayerProfile.Create("Ana", new[] { "Bruno" });
            Assert.That(accepted.Succeeded, Is.True);
            Assert.That(accepted.Profile.Name, Is.EqualTo("Ana"));
        }

        [Test]
        public void PlayerProfile_RF02_RechazaUnNombreQueNoSirveComoNombreDeArchivo()
        {
            // Cada perfil es un archivo dentro de Datos/: un nombre con separadores de ruta
            // escribiría fuera de la carpeta portable y rompería RNF-07 y RNF-11.
            var result = PlayerProfile.Create("../fuera", NoExistingProfiles);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Result, Is.EqualTo(ProfileCreationResult.Status.InvalidName));
        }

        [Test]
        public void PlayerProfile_HU01_ElPerfilNuevoEmpiezaSinFasesYSoloAlcanzaElNivel1()
        {
            // HU-01 FA-03: perfil nuevo → solo el Nivel 1 habilitado.
            var sut = PlayerProfile.Create("Ana", NoExistingProfiles).Profile;

            Assert.That(sut.ReachedLevel, Is.EqualTo(LevelId.Fire));
            Assert.That(sut.ConfirmedPhases, Is.Empty);
            Assert.That(sut.IsUnlocked(LevelId.Fire), Is.True);
            Assert.That(sut.IsUnlocked(LevelId.Wheel), Is.False);
            Assert.That(sut.IsUnlocked(LevelId.River), Is.False);
        }

        [Test]
        public void PlayerProfile_RF04_ConfirmarUnaFaseGuardaSusCuatroIndicadores()
        {
            var sut = PlayerProfile.Create("Ana", NoExistingProfiles).Profile;
            var indicators = new PerformanceIndicators(attempts: 4, correctedErrors: 2, stepsUsed: 3,
                resolutionSeconds: 91.5f);

            sut.ConfirmPhase(LevelId.Fire, phase: 1, indicators);

            Assert.That(sut.IsPhaseConfirmed(LevelId.Fire, 1), Is.True);
            Assert.That(sut.IndicatorsFor(LevelId.Fire, 1), Is.EqualTo(indicators));
            Assert.That(sut.ConfirmedPhases.Count, Is.EqualTo(1));
        }

        [Test]
        public void PlayerProfile_RF41_UnaFaseConfirmadaNoSePierdeAlVolverAJugarla()
        {
            // OE1 §3.6.1 nota 4: reiniciar un nivel no borra los indicadores ya registrados,
            // «el registro del intento anterior se conserva». Manda la primera confirmación;
            // una repetición posterior ni la revoca ni la sobrescribe (RF-41, CP-02).
            var sut = PlayerProfile.Create("Ana", NoExistingProfiles).Profile;
            var first = new PerformanceIndicators(4, 2, 3, 91.5f);
            sut.ConfirmPhase(LevelId.Fire, 1, first);

            sut.ConfirmPhase(LevelId.Fire, 1, new PerformanceIndicators(1, 0, 3, 30f));

            Assert.That(sut.IsPhaseConfirmed(LevelId.Fire, 1), Is.True);
            Assert.That(sut.IndicatorsFor(LevelId.Fire, 1), Is.EqualTo(first));
            Assert.That(sut.ConfirmedPhases.Count, Is.EqualTo(1));
        }

        [Test]
        public void PlayerProfile_RF03_AlcanzarUnNivelNuncaRetrocede()
        {
            var sut = PlayerProfile.Create("Ana", NoExistingProfiles).Profile;

            sut.Reach(LevelId.River);
            sut.Reach(LevelId.Wheel);

            Assert.That(sut.ReachedLevel, Is.EqualTo(LevelId.River));
            Assert.That(sut.IsUnlocked(LevelId.Wheel), Is.True);
        }

        [Test]
        public void PlayerProfile_CP03_NoExponeNingunMiembroDePuntaje()
        {
            // CP-03 y RF-17: nada de cifras de desempeño hacia el estudiante. La razón es
            // pedagógica, no técnica: sin esta prueba, una «mejora» reintroduce el marcador.
            var members = typeof(PlayerProfile).GetMembers().Select(member => member.Name.ToLowerInvariant());

            Assert.That(members.Where(name => name.Contains("score") || name.Contains("puntaje")), Is.Empty);
        }
    }
}
