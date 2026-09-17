using System;
using System.Linq;
using Game.Core;
using NUnit.Framework;
using UnityEditor;

namespace Game.Scaffolding.Tests
{
    /// <summary>
    /// Cuándo se ofrece omitir una escena narrativa (RF-06, INC-28) y por qué el cierre
    /// reflexivo no puede regirse por la misma señal que las demás (CP-07, RF-12).
    /// </summary>
    public class NarrativeVisitPolicyTests
    {
        private static PlayerProfile PerfilQueAcabaDeTerminarElNivel2()
        {
            var profile = PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;
            profile.Reach(LevelId.Wheel);
            foreach (var phase in PhaseId.AllOf(LevelId.Wheel))
            {
                profile.ConfirmPhase(phase, new PerformanceIndicators(2, 1, 4, 40f));
            }

            return profile;
        }

        [Test]
        public void NarrativeVisitPolicy_RF06_ElCierreDelNivel2NoEsOmitibleLaPrimeraVez()
        {
            var profile = PerfilQueAcabaDeTerminarElNivel2();

            Assert.That(NarrativeVisitPolicy.AlreadySeen(profile, Secuencia("N2_Escena21_Bosque")),
                Is.True, "las escenas del nivel ya se vieron: hay fases confirmadas");
            Assert.That(NarrativeVisitPolicy.AlreadySeen(profile, Secuencia("N2_Escena25_Cierre")),
                Is.False,
                "al cierre se llega la primera vez justo después de confirmar la última fase");

            // Segunda vuelta: el nivel ya se terminó una vez —el Nivel 3 quedó desbloqueado— y
            // entonces sí se puede saltar hacia el resumen (HU-14 FA-01).
            profile.Reach(LevelId.River);

            Assert.That(NarrativeVisitPolicy.AlreadySeen(profile, Secuencia("N2_Escena25_Cierre")),
                Is.True);
        }

        /// <summary>
        /// Dentro de la primera vuelta hay fases confirmadas —la del bosque, en cuanto se empuja
        /// la caja— y aun así las escenas que vienen después se están viendo por primera vez.
        /// </summary>
        [Test]
        public void NarrativeVisitPolicy_RF06_UnaEscenaIntermediaNoSeOmiteEnLaPrimeraVuelta()
        {
            var profile = PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;
            profile.Reach(LevelId.Wheel);
            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 1), new PerformanceIndicators(2, 1, 4, 40f));

            Assert.That(NarrativeVisitPolicy.AlreadySeen(profile, Secuencia("N2_Escena22_ElPatron")),
                Is.False, "la 2.2 se ve por primera vez justo tras confirmar la fase 1");
            Assert.That(NarrativeVisitPolicy.AlreadySeen(profile, Secuencia("N2_Escena23_Construccion")),
                Is.False, "y la 2.3, que va encadenada a ella");

            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 2), new PerformanceIndicators(2, 1, 4, 40f));

            Assert.That(NarrativeVisitPolicy.AlreadySeen(profile, Secuencia("N2_Escena24_Regreso")),
                Is.False, "tampoco la 2.4, que llega al confirmar la fase 2");

            // Terminado el nivel, la vuelta siguiente sí puede saltar: lo aprobado no se pierde
            // (RF-41), así que la última fase sigue confirmada.
            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 3), new PerformanceIndicators(2, 1, 4, 40f));

            foreach (var id in new[] { "N2_PuenteI", "N2_Escena22_ElPatron", "N2_Escena24_Regreso" })
            {
                Assert.That(NarrativeVisitPolicy.AlreadySeen(profile, Secuencia(id)), Is.True,
                    $"{id} ya se vio entera una vez");
            }
        }

        [Test]
        public void NarrativeVisitPolicy_RF06_SinPerfilActivoNuncaSeOfreceOmitir()
        {
            Assert.That(NarrativeVisitPolicy.AlreadySeen(null, Secuencia("N2_Escena21_Bosque")),
                Is.False);
        }

        private static NarrativeSequence Secuencia(string id) =>
            AssetDatabase.FindAssets($"t:{nameof(NarrativeSequence)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<NarrativeSequence>)
                .First(sequence => sequence != null && sequence.Id == id);
    }
}
