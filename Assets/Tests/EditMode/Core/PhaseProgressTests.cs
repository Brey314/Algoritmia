using System;
using NUnit.Framework;

namespace Game.Core.Tests
{
    /// <summary>
    /// Progreso por fase (W02). El Slice 1 guardó fases contra un nivel de una sola; el Nivel 2
    /// tiene tres, y es donde el modelo se pone a prueba: el desbloqueo del nivel siguiente deja
    /// de ser una llamada suelta y pasa a depender de que **todas** las fases estén confirmadas.
    /// </summary>
    public class PhaseProgressTests
    {
        private const string PortableRoot = "C:/Juego/Datos";
        private const string FallbackRoot = "C:/Usuario/AppData/Juego";

        private static readonly PerformanceIndicators SomeIndicators =
            new PerformanceIndicators(4, 2, 3, 91.5f);

        private static PlayerProfile NewProfile() =>
            PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;

        [Test]
        public void PhaseId_RF04_CadaNivelDeclaraCuantasFasesTiene()
        {
            Assert.That(PhaseId.PhaseCountOf(LevelId.Fire), Is.EqualTo(1));
            Assert.That(PhaseId.PhaseCountOf(LevelId.Wheel), Is.EqualTo(3));
            Assert.That(PhaseId.PhaseCountOf(LevelId.River), Is.EqualTo(3));
        }

        [Test]
        public void PhaseId_RNF09_RechazaUnaFaseQueElNivelNoTiene()
        {
            // La fase es la clave con la que se persiste el registro: una fase inexistente
            // escribiría en el guardado un dato fuera del modelo declarado.
            Assert.Throws<ArgumentOutOfRangeException>(() => new PhaseId(LevelId.Wheel, 4));
            Assert.Throws<ArgumentOutOfRangeException>(() => new PhaseId(LevelId.Fire, 0));
        }

        [Test]
        public void LevelUnlockPolicy_RF03_Nivel2BloqueadoHastaCompletarNivel1()
        {
            var profile = NewProfile();

            LevelUnlockPolicy.UnlockAfterCompleting(profile, LevelId.Fire);

            Assert.That(LevelUnlockPolicy.IsUnlocked(profile, LevelId.Wheel), Is.False,
                "sin ninguna fase confirmada el Nivel 1 no está completo");

            profile.ConfirmPhase(new PhaseId(LevelId.Fire, 1), SomeIndicators);
            LevelUnlockPolicy.UnlockAfterCompleting(profile, LevelId.Fire);

            Assert.That(LevelUnlockPolicy.IsUnlocked(profile, LevelId.Wheel), Is.True);
        }

        [Test]
        public void LevelUnlockPolicy_RF03_ElNivel3EsperaLasTresFasesDelNivel2()
        {
            var profile = NewProfile();
            profile.Reach(LevelId.Wheel);
            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 1), SomeIndicators);
            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 2), SomeIndicators);

            LevelUnlockPolicy.UnlockAfterCompleting(profile, LevelId.Wheel);

            Assert.That(LevelUnlockPolicy.IsUnlocked(profile, LevelId.River), Is.False,
                "dos de tres fases no completan el nivel");

            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 3), SomeIndicators);
            LevelUnlockPolicy.UnlockAfterCompleting(profile, LevelId.Wheel);

            Assert.That(LevelUnlockPolicy.IsUnlocked(profile, LevelId.River), Is.True);
        }

        [Test]
        public void PlayerProfile_RNF14_RetomaEnLaPrimeraFaseSinConfirmarDelNivel()
        {
            var profile = NewProfile();
            profile.Reach(LevelId.Wheel);

            Assert.That(profile.NextPendingPhase(LevelId.Wheel),
                Is.EqualTo(new PhaseId(LevelId.Wheel, 1)));

            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 1), SomeIndicators);

            Assert.That(profile.NextPendingPhase(LevelId.Wheel),
                Is.EqualTo(new PhaseId(LevelId.Wheel, 2)));

            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 2), SomeIndicators);
            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 3), SomeIndicators);

            Assert.That(profile.NextPendingPhase(LevelId.Wheel), Is.Null,
                "un nivel completo no tiene fase pendiente");
            Assert.That(profile.IsLevelComplete(LevelId.Wheel), Is.True);
        }

        [Test]
        public void SaveStore_RF04_ConfirmarFase1DelNivel2SobreviveAlCierre()
        {
            var fileSystem = new FakeFileSystem();
            var profile = NewProfile();
            profile.Reach(LevelId.Wheel);
            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 1), SomeIndicators);
            new SaveStore(fileSystem, PortableRoot, FallbackRoot).Save(profile);

            // El cierre del juego: un almacén nuevo sobre el mismo disco, como al reabrir.
            var reopened = new SaveStore(fileSystem, PortableRoot, FallbackRoot).Load("Ana");

            Assert.That(reopened.IsPhaseConfirmed(new PhaseId(LevelId.Wheel, 1)), Is.True);
            Assert.That(reopened.IndicatorsFor(new PhaseId(LevelId.Wheel, 1)),
                Is.EqualTo(SomeIndicators));
            Assert.That(reopened.NextPendingPhase(LevelId.Wheel),
                Is.EqualTo(new PhaseId(LevelId.Wheel, 2)),
                "al reabrir se retoma en la fase 2, no al principio del nivel");
        }

        [Test]
        public void SaveStore_RF04_ConfirmarUnaFaseDelNivel3SobreviveAlCierre()
        {
            // R02: el Nivel 3 guarda en sus tres fases de ensamblaje —base, amarre, mástil y
            // vela— y no al terminar la recolección (decisión del 16/09/2026, ver PhaseId).
            var fileSystem = new FakeFileSystem();
            var profile = NewProfile();
            profile.Reach(LevelId.River);
            profile.ConfirmPhase(new PhaseId(LevelId.River, 1), SomeIndicators);
            new SaveStore(fileSystem, PortableRoot, FallbackRoot).Save(profile);

            var reopened = new SaveStore(fileSystem, PortableRoot, FallbackRoot).Load("Ana");

            Assert.That(reopened.IsPhaseConfirmed(new PhaseId(LevelId.River, 1)), Is.True);
            Assert.That(reopened.IndicatorsFor(new PhaseId(LevelId.River, 1)),
                Is.EqualTo(SomeIndicators));
            Assert.That(reopened.NextPendingPhase(LevelId.River),
                Is.EqualTo(new PhaseId(LevelId.River, 2)),
                "al reabrir se retoma en el amarre, no en la recolección ni en la base");
        }

        [Test]
        public void PlayerProfile_RF41_UnaFaseAprobadaNoSePierdeTrasUnaPruebaFallida()
        {
            // Base y amarre confirmados; la prueba de la balsa falla (no se confirma la 3) y el
            // estudiante reinicia el nivel y vuelve a pasar el amarre con peores indicadores.
            // Nada de eso desconfirma ni pisa lo aprobado (RF-41, RF-43, CP-02).
            var profile = NewProfile();
            profile.Reach(LevelId.River);
            profile.ConfirmPhase(new PhaseId(LevelId.River, 1), SomeIndicators);
            profile.ConfirmPhase(new PhaseId(LevelId.River, 2), SomeIndicators);

            profile.ConfirmPhase(new PhaseId(LevelId.River, 2), new PerformanceIndicators(99, 0, 3, 2f));

            Assert.That(profile.IsPhaseConfirmed(new PhaseId(LevelId.River, 1)), Is.True);
            Assert.That(profile.IsPhaseConfirmed(new PhaseId(LevelId.River, 2)), Is.True);
            Assert.That(profile.IndicatorsFor(new PhaseId(LevelId.River, 2)), Is.EqualTo(SomeIndicators));
            Assert.That(profile.IsLevelComplete(LevelId.River), Is.False,
                "sin mástil y vela el nivel no está completo");
            Assert.That(profile.NextPendingPhase(LevelId.River),
                Is.EqualTo(new PhaseId(LevelId.River, 3)));
        }

        [Test]
        public void PlayerProfile_CP02_UnaFaseConfirmadaNoSePierdeNunca()
        {
            var profile = NewProfile();
            profile.Reach(LevelId.Wheel);
            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 1), SomeIndicators);

            // Reiniciar el nivel y volver a fallar la fase 1: ni la desconfirma ni pisa sus
            // indicadores, y el nivel alcanzado no retrocede (CP-02, RF-41, HU-17).
            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 1), new PerformanceIndicators(99, 0, 1, 2f));
            profile.Reach(LevelId.Fire);

            Assert.That(profile.IsPhaseConfirmed(new PhaseId(LevelId.Wheel, 1)), Is.True);
            Assert.That(profile.IndicatorsFor(new PhaseId(LevelId.Wheel, 1)), Is.EqualTo(SomeIndicators));
            Assert.That(profile.ReachedLevel, Is.EqualTo(LevelId.Wheel));
            Assert.That(profile.NextPendingPhase(LevelId.Wheel),
                Is.EqualTo(new PhaseId(LevelId.Wheel, 2)));
        }
    }
}
