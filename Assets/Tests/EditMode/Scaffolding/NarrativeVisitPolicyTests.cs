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
