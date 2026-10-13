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

        [Test]
        public void DialogueRunner_INC160_VolverAtrasEnLaPrimeraLineaNoHaceNada()
        {
            var sut = new DialogueRunner(TresLineas, alreadySeen: false);

            sut.Back();

            Assert.That(sut.Index, Is.EqualTo(0), "la primera línea es el tope: no cruza a otra secuencia");
            Assert.That(sut.CanGoBack, Is.False, "y ahí el botón no se ofrece");
            Assert.That(sut.Current.Text, Is.EqualTo("¡Hola, familia!"));
        }

        [Test]
        public void DialogueRunner_INC160_VolverAtrasTrasAvanzarBajaUnaLinea()
        {
            var sut = new DialogueRunner(TresLineas, alreadySeen: false);
            sut.Advance();
            sut.Advance();

            sut.Back();

            Assert.That(sut.Index, Is.EqualTo(1), "baja una línea");
            Assert.That(sut.Current.Text, Is.EqualTo("¿Se fue?"), "y la línea en curso es la anterior");
            Assert.That(sut.CanGoBack, Is.True, "todavía hay una más atrás");
            Assert.That(sut.Progress, Is.EqualTo(0.5f), "el progreso de la cámara sigue a la línea");
        }

        [Test]
        public void DialogueRunner_INC160_VolverAtrasNoReabreUnDialogoTerminado()
        {
            var sut = new DialogueRunner(TresLineas, alreadySeen: false);
            sut.Advance();
            sut.Advance();
            sut.Advance();

            sut.Back();

            Assert.That(sut.IsFinished, Is.True, "terminada sigue terminada");
            Assert.That(sut.Current, Is.Null, "sin línea que releer");
            Assert.That(sut.CanGoBack, Is.False);
        }

        [Test]
        public void DialogueRunner_INC160_VolverAtrasTampocoReabreUnaEscenaOmitida()
        {
            var sut = new DialogueRunner(TresLineas, alreadySeen: true);
            sut.Advance();
            sut.Skip();

            sut.Back();

            Assert.That(sut.IsFinished, Is.True);
        }

        [Test]
        public void DialogueRunner_INC160_UnaLineaYaVistaNoVuelveASerNueva()
        {
            var sut = new DialogueRunner(TresLineas, alreadySeen: false);
            Assert.That(sut.IsNewLine, Is.True, "la primera línea es nueva");
            sut.Advance();
            Assert.That(sut.IsNewLine, Is.True, "avanzar a una línea nunca vista es nuevo");

            sut.Back();
            Assert.That(sut.IsNewLine, Is.False, "releer no repite lo que ya pasó");

            sut.Advance();
            Assert.That(sut.IsNewLine, Is.False, "volver a avanzar sobre una línea ya vista tampoco");

            sut.Advance();
            Assert.That(sut.IsNewLine, Is.True, "la primera línea que nunca se vio sí se vive completa");
        }

        [Test]
        public void DialogueRunner_RF05_ElProgresoVaDeCeroAUnoAlRitmoDeLasLineas()
        {
            var sut = new DialogueRunner(TresLineas, alreadySeen: false);

            // Es lo que mueve la cámara por la ilustración: al ritmo de la lectura, no del reloj.
            Assert.That(sut.Progress, Is.EqualTo(0f), "en la primera línea");
            sut.Advance();
            Assert.That(sut.Progress, Is.EqualTo(0.5f), "a mitad de la escena");
            sut.Advance();
            Assert.That(sut.Progress, Is.EqualTo(1f), "en la última");
            sut.Advance();
            Assert.That(sut.Progress, Is.EqualTo(1f), "y terminada sigue en el final");

            Assert.That(new DialogueRunner(Array.Empty<DialogueLine>(), false).Progress, Is.EqualTo(1f),
                "una secuencia sin líneas no tiene recorrido que hacer");
        }
    }
}
