using System.Collections.Generic;
using System.Linq;
using Game.EditorTools.Sandbox;
using NUnit.Framework;
using UnityEngine;

namespace Game.EditorTools.Tests
{
    /// <summary>
    /// La maqueta de personaje que sigue al cursor.
    /// </summary>
    /// <remarks>
    /// **El id del medio no es un RF y aquí está bien que no lo sea.** Esto no es contenido del
    /// juego sino una maqueta que solo corre en el Editor, así que no traza ningún requisito y no
    /// cuenta para CT-10; `DA131` señala el apartado §13.1 de `Direccion_de_Arte.md`, que es lo
    /// que esta maqueta pone a prueba. Lo que sí es requisito de verdad —no salirse de la
    /// pantalla— va con su RNF.
    /// </remarks>
    public class CharacterProbeMotionTests
    {
        private const float Dt = 1f / 60f;

        private static readonly Rect Pantalla = new Rect(0f, 0f, 800f, 600f);

        private static CharacterProbeMotion Maqueta() =>
            new CharacterProbeMotion(Pantalla, speed: 200f, frameSeconds: 0.1f, arriveRadius: 20f);

        [Test]
        public void CharacterProbeMotion_DA131_CaminaHaciaElCursorYSeAgachaAlLlegar()
        {
            var sut = Maqueta();
            var destino = new Vector2(700f, 300f);
            var partida = sut.Position;

            sut.Step(destino, Dt);
            Assert.That(sut.Pose, Is.EqualTo(ProbePose.Walking), "con el cursor lejos, camina");

            for (var frame = 0; frame < 300; frame++)
            {
                sut.Step(destino, Dt);
            }

            Assert.That(sut.Position.x, Is.GreaterThan(partida.x), "avanzó hacia el cursor");
            Assert.That(Vector2.Distance(sut.Position, destino), Is.LessThanOrEqualTo(20f),
                "y llegó hasta él");
            Assert.That(sut.Pose, Is.EqualTo(ProbePose.Crouching), "al llegar se agacha a recoger");
        }

        [Test]
        public void CharacterProbeMotion_DA131_SoloSeVolteaCuandoCaminaHaciaLaIzquierda()
        {
            var sut = Maqueta();

            sut.Step(new Vector2(100f, 300f), Dt);
            Assert.That(sut.Mirrored, Is.True, "yendo a la izquierda va volteada");

            sut.Step(new Vector2(700f, 300f), Dt);
            Assert.That(sut.Mirrored, Is.False, "yendo a la derecha va en su sentido dibujado");
        }

        [Test]
        public void CharacterProbeMotion_DA131_AgachadaNuncaSaleVolteada()
        {
            // Se la hace llegar desde la derecha, de modo que venga caminando volteada: si el
            // espejo se conservara al agacharse, mostraría una postura que no se dibujó nunca.
            var sut = Maqueta();
            var destino = new Vector2(60f, 300f);

            sut.Step(destino, Dt);
            Assert.That(sut.Mirrored, Is.True, "llega caminando hacia la izquierda");

            for (var frame = 0; frame < 300; frame++)
            {
                sut.Step(destino, Dt);
            }

            Assert.That(sut.Pose, Is.EqualTo(ProbePose.Crouching));
            Assert.That(sut.Mirrored, Is.False,
                "de agacharse solo existe el sentido derecho: no hay sprite que voltear");
        }

        [Test]
        public void CharacterProbeMotion_DA131_LosTresFotogramasSeAlternanEnCicloYNingunoSeSaleDeLaSecuencia()
        {
            var sut = Maqueta();
            var destino = new Vector2(700f, 300f);
            var vistos = new List<int>();

            // Medio segundo: a 0.1 s por fotograma son cinco pasos de secuencia.
            for (var frame = 0; frame < 30; frame++)
            {
                sut.Step(destino, Dt);
                vistos.Add(sut.Frame);
            }

            Assert.That(vistos.Distinct().OrderBy(indice => indice), Is.EqualTo(new[] { 0, 1, 2 }),
                "los tres fotogramas se usan, y ninguno más");
            Assert.That(vistos.First(), Is.EqualTo(0), "la secuencia arranca por el primero");
            Assert.That(vistos.Skip(1).Zip(vistos, (ahora, antes) => ahora - antes)
                    .Any(salto => salto < 0), Is.True,
                "y da la vuelta: es un ciclo, no una cuenta que crece");
        }

        [Test]
        public void CharacterProbeMotion_RNF03_NuncaSaleDeLaPantallaYSeDevuelveEnElBorde()
        {
            var sut = Maqueta();

            // Un cursor imposible, muy fuera de la pantalla: la empuja contra el borde sin parar.
            var imposible = new Vector2(10000f, 300f);
            var seDevolvio = false;

            for (var frame = 0; frame < 600; frame++)
            {
                sut.Step(imposible, Dt);

                Assert.That(sut.Position.x, Is.InRange(Pantalla.xMin, Pantalla.xMax),
                    $"frame {frame}: se salió por el costado");
                Assert.That(sut.Position.y, Is.InRange(Pantalla.yMin, Pantalla.yMax),
                    $"frame {frame}: se salió por arriba o por abajo");

                seDevolvio |= sut.Velocity.x < 0f;
            }

            Assert.That(sut.Position.x, Is.EqualTo(Pantalla.xMax).Within(0.001f),
                "se queda contra el borde, persiguiendo un cursor que no puede alcanzar");
            Assert.That(seDevolvio, Is.True,
                "y al tocarlo se devuelve, aunque persiguiendo el cursor dure un solo fotograma");
        }
    }
}
