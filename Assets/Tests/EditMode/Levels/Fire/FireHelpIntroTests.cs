using System;
using System.Linq;
using Game.Core;
using NUnit.Framework;
using UnityEditor;

namespace Game.Levels.Fire.Tests
{
    /// <summary>
    /// El aviso del botón de ayuda (INC-158, sesión con estudiantes del 13/10/2026: «no se entiende
    /// para qué sirve el círculo con Algoritm»): cuándo sale y qué dice.
    /// </summary>
    public class FireHelpIntroTests
    {
        private static readonly PerformanceIndicators Indicators = new PerformanceIndicators(1, 0, 1, 10f);

        private static PlayerProfile NewProfile() =>
            PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;

        [Test]
        public void FireHelpIntro_INC158_SaleMientrasElPerfilNoHayaConfirmadoLaFase1()
        {
            Assert.That(FireHelpIntro.ShouldShow(NewProfile()), Is.True);
        }

        [Test]
        public void FireHelpIntro_INC158_NoSaleSiElPerfilYaTerminoLaPrimeraMecanica()
        {
            var profile = NewProfile();
            profile.ConfirmPhase(new PhaseId(LevelId.Fire, 1), Indicators);

            Assert.That(FireHelpIntro.ShouldShow(profile), Is.False);
        }

        [Test]
        public void FireHelpIntro_INC158_SinPerfilActivoNoSale()
        {
            // La escena abierta sin pasar por Boot y las pruebas de la cueva no tienen a quién
            // explicarle nada, y el aviso detiene el tiempo.
            Assert.That(FireHelpIntro.ShouldShow(null), Is.False);
        }

        [Test]
        public void FireHelpIntro_INC158_ConfirmarOtroNivelNoLoApaga()
        {
            // Se deriva de la fase 1 del Nivel 1 y de nada más: un perfil con otras fases
            // confirmadas pero no esa (no ocurre jugando, pero el criterio es ese) sigue viéndolo.
            var profile = NewProfile();
            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 1), Indicators);

            Assert.That(FireHelpIntro.ShouldShow(profile), Is.True);
        }

        [Test]
        public void FireMessages_INC158_ElAvisoDeLaAyudaTieneTextoYNoTrataDeCifras()
        {
            var messages = AssetDatabase.FindAssets($"t:{nameof(FireMessages)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<FireMessages>)
                .Single();

            Assert.That(messages.HelpIntro,
                Is.EqualTo("Este es el botón de ayuda. Si no recuerdas qué hacer, haz clic en él y Algoritm te repetirá la instrucción."));
            Assert.That(messages.HelpIntroClose, Is.EqualTo("Cerrar"));
            Assert.That(new[] { messages.HelpIntro, messages.HelpIntroClose },
                Has.None.Matches<string>(text => text.Any(char.IsDigit)), "CP-03: ninguna cifra");
        }
    }
}
