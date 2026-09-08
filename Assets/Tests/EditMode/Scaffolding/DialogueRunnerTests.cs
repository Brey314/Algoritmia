using System;
using System.Collections.Generic;
using Game.Scaffolding;
using NUnit.Framework;

namespace Game.Scaffolding.Tests
{
    public class DialogueRunnerTests
    {
        private static readonly IReadOnlyList<DialogueLine> TresLineas = new[]
        {
            new DialogueLine("CHISPA", "¡Hola, familia!"),
            new DialogueLine("NIÑA", "¿Se fue?"),
            new DialogueLine("PAPÁ", "Sí. Pero nos dejó algo.")
        };

        [Test]
        public void DialogueRunner_RF05_AvanzaUnaLineaPorClic()
        {
            var sut = new DialogueRunner(TresLineas, alreadySeen: false);
            var recorrido = new List<string> { sut.Current.Text };

            while (sut.Advance())
            {
                recorrido.Add(sut.Current.Text);
            }

            Assert.That(recorrido, Is.EqualTo(new[]
            {
                "¡Hola, familia!", "¿Se fue?", "Sí. Pero nos dejó algo."
            }));
        }

        [Test]
        public void DialogueRunner_RF05_TerminaDespuesDeLaUltimaLinea()
        {
            var sut = new DialogueRunner(TresLineas, alreadySeen: false);

            sut.Advance();
            sut.Advance();

            Assert.That(sut.IsFinished, Is.False, "en la última línea todavía no ha terminado");
            Assert.That(sut.Advance(), Is.False, "no hay una cuarta línea que mostrar");
            Assert.That(sut.IsFinished, Is.True, "tras la última línea, la escena terminó");
        }

        [Test]
        public void DialogueRunner_RF06_NoOfreceOmitirLaPrimeraVez()
        {
            var sut = new DialogueRunner(TresLineas, alreadySeen: false);

            Assert.That(sut.CanSkip, Is.False);
        }

        [Test]
        public void DialogueRunner_RF06_OfreceOmitirSiLaEscenaYaFueVista()
        {
            var sut = new DialogueRunner(TresLineas, alreadySeen: true);

            Assert.That(sut.CanSkip, Is.True);
        }

        [Test]
        public void DialogueRunner_INC28_OmitirLaPrimeraVezNoSaltaLaEscena()
        {
            var sut = new DialogueRunner(TresLineas, alreadySeen: false);

            Assert.That(sut.Skip(), Is.False, "omitir no está permitido todavía");
            Assert.That(sut.IsFinished, Is.False, "la escena sigue donde estaba");
            Assert.That(sut.Current.Text, Is.EqualTo("¡Hola, familia!"));
        }

        [Test]
        public void DialogueRunner_INC28_OmitirUnaEscenaYaVistaLlegaAlFinal()
        {
            var sut = new DialogueRunner(TresLineas, alreadySeen: true);

            Assert.That(sut.Skip(), Is.True);
            Assert.That(sut.IsFinished, Is.True);
        }

        [Test]
        public void DialogueRunner_RF05_UnaSecuenciaSinLineasNaceTerminada()
        {
            var sut = new DialogueRunner(Array.Empty<DialogueLine>(), alreadySeen: false);

            Assert.That(sut.IsFinished, Is.True);
            Assert.That(sut.Advance(), Is.False);
        }
    }
}
