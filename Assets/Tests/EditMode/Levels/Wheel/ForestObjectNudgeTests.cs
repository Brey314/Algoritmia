using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Levels.Wheel.Tests
{
    /// <summary>
    /// El apartarse de los objetos al acercar el cursor. Es adorno con una carga pedagógica: lo
    /// redondo rueda y lo anguloso no, que es el patrón que RF-23 pide descubrir mirando.
    /// </summary>
    public class ForestObjectNudgeTests
    {
        private const float Dt = 1f / 60f;
        private const int UnSegundo = 60;

        // El suelo en fracciones, igual que `ForestObject.FloorPosition`.
        private static readonly Rect Suelo = new Rect(0f, 0f, 1f, 1f);

        private static readonly Vector2 Centro = new Vector2(0.5f, 0.5f);

        // Cursor pegado por la izquierda: empuja hacia la derecha.
        private static readonly Vector2 CursorPegado = new Vector2(0.44f, 0.5f);
        private static readonly Vector2 CursorLejos = new Vector2(0.05f, 0.05f);

        [Test]
        public void ForestObjectNudge_RF23_LoRedondoRuedaMasLejosQueLoAnguloso()
        {
            // La razón de ser del efecto: el mismo empujón mueve mucho al tronco y poco a la
            // piedra. Es RF-23 dicho con movimiento en vez de con texto.
            var tronco = Recorrido(Rodante(), UnSegundo);
            var piedra = Recorrido(Angulosa(), UnSegundo);

            Assert.That(tronco, Is.GreaterThan(piedra),
                "el tronco redondo rueda más lejos que la piedra con el mismo empujón");
        }

        [Test]
        public void ForestObjectNudge_RF23_LaPiedraSeDetieneTrasUnSoloVuelco()
        {
            var sut = new ForestObjectNudge(Angulosa(), Centro, Suelo);

            // El cursor no se aparta en ningún momento: la piedra igual se queda quieta.
            for (var frame = 0; frame < 10; frame++)
            {
                sut.Step(CursorPegado, Dt);
            }

            var trasElVuelco = sut.Offset;

            for (var frame = 0; frame < UnSegundo; frame++)
            {
                sut.Step(CursorPegado, Dt);
            }

            Assert.That(sut.Offset, Is.EqualTo(trasElVuelco),
                "vuelca hasta la siguiente esquina y ahí se queda: una piedra no rueda");

            // Exactamente **una** esquina, no «acabó parándose». Sin fijar el tamaño, una piedra
            // que volcara en cada fotograma también pasaría: se detendría sola al salirse del
            // radio del cursor, y la prueba lo daría por bueno. Medido con una mutación.
            Assert.That(trasElVuelco.magnitude, Is.EqualTo(Angulosa().StepSize).Within(1e-5f),
                "un solo vuelco, del tamaño que dice el asset");
        }

        [Test]
        public void ForestObjectNudge_RF23_LaPiedraVuelveAVolcarSiElCursorSeVaYRegresa()
        {
            var sut = new ForestObjectNudge(Angulosa(), Centro, Suelo);
            for (var frame = 0; frame < 10; frame++)
            {
                sut.Step(CursorPegado, Dt);
            }

            var primerVuelco = sut.Offset;
            sut.Step(CursorLejos, Dt); // el cursor se aleja: vuelve a armarse
            for (var frame = 0; frame < 10; frame++)
            {
                sut.Step(CursorPegado, Dt);
            }

            Assert.That(sut.Offset.magnitude, Is.GreaterThan(primerVuelco.magnitude),
                "cada acercamiento la vuelca una esquina más, no la deja muerta para siempre");
        }

        [Test]
        public void ForestObjectNudge_RF22_LaHerramientaApenasSeArrastra()
        {
            var herramienta = Recorrido(Arrastrada(), UnSegundo);
            var tronco = Recorrido(Rodante(), UnSegundo);

            Assert.That(herramienta, Is.GreaterThan(0f), "algo se mueve: acusa el paso del cursor");
            Assert.That(herramienta, Is.LessThan(tronco / 4f),
                "pero se arrastra, no rueda: bastante menos que el tronco");
        }

        [Test]
        public void ForestObjectNudge_RF22_LaHojaSeLevantaDelSueloYVuelveASuSitio()
        {
            var sut = new ForestObjectNudge(Voladora(), Centro, Suelo);
            var alturaMaxima = 0f;

            for (var frame = 0; frame < UnSegundo * 4; frame++)
            {
                sut.Step(frame < 6 ? CursorPegado : CursorLejos, Dt);
                alturaMaxima = Mathf.Max(alturaMaxima, sut.Offset.y);
            }

            Assert.That(alturaMaxima, Is.GreaterThan(0f), "se despega del piso");
            Assert.That(sut.Offset.y, Is.EqualTo(0f).Within(0.001f),
                "y la gravedad la devuelve al suelo: el vuelo es un arco, no una levitación");
        }

        [Test]
        public void ForestObjectNudge_RNF03_NingunObjetoSeSaleDelAreaJugable()
        {
            var sut = new ForestObjectNudge(Rodante(), new Vector2(0.9f, 0.5f), Suelo);

            // Cursor empujando siempre hacia el borde derecho, mucho más de lo que cabe.
            for (var frame = 0; frame < UnSegundo * 10; frame++)
            {
                sut.Step(new Vector2(sut.Position.x - 0.06f, 0.5f), Dt);

                Assert.That(EnElSuelo(sut.Position), Is.True,
                    $"frame {frame}: {sut.Position} se salió del suelo");
            }
        }

        [Test]
        public void ForestObjectNudge_RNF03_AlTocarElBordeElMovimientoRebota()
        {
            // Sin muelle y sin rozamiento: así lo único capaz de invertir el signo de la
            // velocidad es el borde. Con muelle, el tirón de vuelta a casa se haría pasar por un
            // rebote y la prueba pasaría sin que exista el rebote.
            var sinFreno = NudgeSettings.Create(ForestObjectCategory.RoundLog,
                radius: 0.10f, push: 3f, drag: 0f, spring: 0f);
            var sut = new ForestObjectNudge(sinFreno, new Vector2(0.99f, 0.5f), Suelo);

            // Un empujón fuerte hacia la derecha y el cursor se retira: lo que queda es la inercia.
            for (var frame = 0; frame < 4; frame++)
            {
                sut.Step(new Vector2(0.93f, 0.5f), Dt);
            }

            Assert.That(sut.Velocity.x, Is.GreaterThan(0f), "sale disparado hacia el borde");

            var reboto = false;
            for (var frame = 0; frame < UnSegundo && !reboto; frame++)
            {
                sut.Step(CursorLejos, Dt);
                reboto = sut.Velocity.x < 0f;
            }

            Assert.That(reboto, Is.True, "al topar con el borde de la pantalla se devuelve");
            Assert.That(EnElSuelo(sut.Position), Is.True, "y nunca llegó a salirse");
        }

        [Test]
        public void ForestObjectNudge_CP02_ElCursorLejosNoMueveNiEstorbaANadie()
        {
            var sut = new ForestObjectNudge(Rodante(), Centro, Suelo);

            for (var frame = 0; frame < UnSegundo; frame++)
            {
                sut.Step(CursorLejos, Dt);
            }

            // Nada se mueve solo. El efecto responde al cursor y no gasta intentos, no penaliza y
            // no cambia el estado del acopio: es adorno con carga pedagógica, no una mecánica.
            Assert.That(sut.Offset, Is.EqualTo(Vector2.zero), "quieto donde lo dejó el asset");
        }

        [Test]
        public void WheelLevelConfig_RF23_ElAssetDaAlTroncoMasEmpujeQueAlRestoDeCategorias()
        {
            var ajustes = ConfiguracionDelNivel2().Nudges;

            Assert.That(ajustes.Select(ajuste => ajuste.Category),
                Is.EquivalentTo(new[]
                {
                    ForestObjectCategory.RoundLog, ForestObjectCategory.Stone,
                    ForestObjectCategory.Plant, ForestObjectCategory.Tool
                }),
                "hay un ajuste por categoría y ninguno repetido");

            var tronco = ajustes.First(ajuste => ajuste.Category == ForestObjectCategory.RoundLog);

            Assert.That(ajustes.Where(ajuste => ajuste.Category != ForestObjectCategory.RoundLog)
                    .Select(ajuste => ajuste.Push),
                Is.All.LessThan(tronco.Push),
                "solo lo redondo rueda de verdad: es la contraposición que enseña el nivel");
            Assert.That(tronco.StepSize, Is.EqualTo(0f),
                "el tronco no vuelca de esquina en esquina: no las tiene");
            Assert.That(ajustes.First(ajuste => ajuste.Category == ForestObjectCategory.Stone).StepSize,
                Is.GreaterThan(0f), "la piedra sí, y por eso se detiene");
        }

        [Test]
        public void WheelLevelConfig_RNF21_ElMovimientoAmbientalEsPequenoEnTodasLasCategorias()
        {
            // `Direccion_de_Arte.md` §14.3: las animaciones ambientales van de amplitud pequeña y
            // ciclo lento. Un objeto que cruce media pantalla al pasar el ratón sería un destello
            // de movimiento, justo lo que ese apartado prohíbe.
            var excedidos = ConfiguracionDelNivel2().Nudges
                .Where(ajuste => Recorrido(ajuste, UnSegundo * 2) > 0.25f)
                .Select(ajuste => $"{ajuste.Category} recorre {Recorrido(ajuste, UnSegundo * 2):0.00}")
                .ToArray();

            Assert.That(excedidos, Is.Empty, string.Join(" · ", excedidos));
        }

        // --- helpers -------------------------------------------------------------------------

        /// <summary>
        /// Dentro del suelo con los dos bordes incluidos. No sirve <c>Rect.Contains</c>: es
        /// semiabierto en el máximo, así que un objeto detenido justo contra el borde derecho
        /// —que es exactamente lo que deja el confinamiento— daría «fuera».
        /// </summary>
        private static bool EnElSuelo(Vector2 punto) =>
            punto.x >= Suelo.xMin && punto.x <= Suelo.xMax &&
            punto.y >= Suelo.yMin && punto.y <= Suelo.yMax;

        /// <summary>Cuánto se aleja de su sitio con el cursor encima durante los frames dados.</summary>
        private static float Recorrido(NudgeSettings ajuste, int frames)
        {
            var sut = new ForestObjectNudge(ajuste, Centro, Suelo);
            var lejos = 0f;

            for (var frame = 0; frame < frames; frame++)
            {
                sut.Step(CursorPegado, Dt);
                lejos = Mathf.Max(lejos, sut.Offset.magnitude);
            }

            return lejos;
        }

        // Valores deliberadamente distintos a los del asset: si la lógica se apoyara en un literal
        // en vez de en la configuración, estas pruebas fallarían (RNF-18).
        private static NudgeSettings Rodante() => NudgeSettings.Create(
            ForestObjectCategory.RoundLog, radius: 0.10f, push: 0.9f, drag: 1.2f, spring: 0.8f);

        private static NudgeSettings Angulosa() => NudgeSettings.Create(
            ForestObjectCategory.Stone, radius: 0.10f, push: 0f, drag: 6f, spring: 0f,
            stepSize: 0.012f);

        private static NudgeSettings Arrastrada() => NudgeSettings.Create(
            ForestObjectCategory.Tool, radius: 0.10f, push: 0.05f, drag: 9f, spring: 4f);

        // El impulso tiene que superar a la gravedad o la hoja no se despega: con la fuerza del
        // cursor al 40 %, `lift` efectivo es 0.48 contra los 0.30 que tira hacia abajo.
        private static NudgeSettings Voladora() => NudgeSettings.Create(
            ForestObjectCategory.Plant, radius: 0.10f, push: 0.10f, drag: 2.5f, spring: 0f,
            lift: 1.2f, gravity: 0.3f);

        private static WheelLevelConfig ConfiguracionDelNivel2()
        {
            var config = AssetDatabase.FindAssets($"t:{nameof(WheelLevelConfig)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<WheelLevelConfig>)
                .FirstOrDefault(asset => asset != null);

            Assert.That(config, Is.Not.Null, "falta el asset de configuración del Nivel 2");
            return config;
        }
    }
}
