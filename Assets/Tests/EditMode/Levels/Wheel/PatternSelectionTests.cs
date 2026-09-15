using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;

namespace Game.Levels.Wheel.Tests
{
    public class PatternSelectionTests
    {
        private const int MaximoPalabrasPorOracion = 20;

        // Valores deliberadamente distintos a los del asset del juego: si la lógica se apoyara en
        // un literal en vez de en la configuración, estas pruebas fallarían (RNF-18).
        private const int TroncosDePrueba = 3;
        private const int DistractoresDePrueba = 4;

        private static readonly ForestObject Tronco1 = new ForestObject("tronco_1", ForestObjectCategory.RoundLog);
        private static readonly ForestObject Tronco2 = new ForestObject("tronco_2", ForestObjectCategory.RoundLog);
        private static readonly ForestObject Piedra = new ForestObject("piedra_1", ForestObjectCategory.Stone);
        private static readonly ForestObject Planta = new ForestObject("planta_1", ForestObjectCategory.Plant);
        private static readonly ForestObject Herramienta = new ForestObject("hacha_1", ForestObjectCategory.Tool);

        [Test]
        public void PatternSelection_RF23_AceptaTroncoRedondoYRechazaDistractor()
        {
            var sut = new PatternSelection(ConfiguracionDePrueba());

            var acierto = sut.Select(Tronco1);
            var rechazo = sut.Select(Piedra);

            Assert.That(acierto.Accepted, Is.True, "el tronco redondo comparte el patrón");
            Assert.That(acierto.Message, Is.EqualTo("acierto"), "el acierto también responde con una frase (RF-11)");
            Assert.That(rechazo.Accepted, Is.False, "la piedra vuelve a su posición");
            Assert.That(rechazo.Message, Is.EqualTo("piedra"), "y lo hace acompañada de retroalimentación narrativa");
            Assert.That(sut.Collected, Is.EqualTo(1), "solo el tronco suma al acopio");
        }

        [Test]
        public void PatternSelection_RF23_CadaCategoriaDeDistractorDevuelveSuPropioMensaje()
        {
            var sut = new PatternSelection(ConfiguracionDePrueba());

            var mensajes = new[] { Piedra, Planta, Herramienta }
                .Select(objeto => sut.Select(objeto).Message)
                .ToArray();

            Assert.That(mensajes, Is.EqualTo(new[] { "piedra", "planta", "herramienta" }),
                "una retroalimentación por categoría de distractor (guion §6.1.2)");
            Assert.That(mensajes.Distinct().Count(), Is.EqualTo(mensajes.Length),
                "ninguna categoría comparte el mensaje de otra");
        }

        [Test]
        public void PatternSelection_RF24_ElContadorNoRetrocedeAnteUnRechazo()
        {
            var sut = new PatternSelection(ConfiguracionDePrueba());
            sut.Select(Tronco1);
            sut.Select(Tronco2);

            sut.Select(Piedra);
            sut.Select(Planta);
            sut.Select(Herramienta);

            Assert.That(sut.Collected, Is.EqualTo(2), "lo acopiado no se pierde por un fallo posterior (RF-41)");
            Assert.That(sut.CounterText, Is.EqualTo($"acopiados 2 de {TroncosDePrueba}"),
                "el contador permanente sale del formato del asset, no de una cadena en el código");
        }

        [Test]
        public void PatternSelection_CP02_UnRechazoNoPenalizaNiBloquea()
        {
            var sut = new PatternSelection(ConfiguracionDePrueba());

            for (var intento = 0; intento < 10; intento++)
            {
                var rechazo = sut.Select(Piedra);
                Assert.That(rechazo.Accepted, Is.False);
                Assert.That(rechazo.Message, Is.EqualTo("piedra"),
                    "el intento número diez responde igual que el primero: no hay límite (RF-18)");
            }

            Assert.That(sut.Collected, Is.Zero, "rechazar nunca resta");
            Assert.That(sut.Select(Tronco1).Accepted, Is.True,
                "tras diez rechazos el camino correcto sigue abierto");
        }

        [Test]
        public void PatternSelection_RF24_UnObjetoYaAcopiadoNoSeCuentaDosVeces()
        {
            var sut = new PatternSelection(ConfiguracionDePrueba());
            sut.Select(Tronco1);

            var repetido = sut.Select(Tronco1);

            Assert.That(sut.Collected, Is.EqualTo(1), "el tronco ya está en la zona de acopio");
            Assert.That(repetido.Accepted, Is.False);
            Assert.That(repetido.Message, Is.Empty, "no hubo intento que describir, así que no hay frase");
        }

        [Test]
        public void PatternSelection_RF23_LaFaseTerminaAlCompletarElAcopioDelAsset()
        {
            var config = ConfiguracionDePrueba();
            var sut = new PatternSelection(config);

            for (var indice = 0; indice < config.RequiredLogs; indice++)
            {
                Assert.That(sut.IsComplete, Is.False, "todavía faltan troncos");
                sut.Select(new ForestObject($"tronco_{indice}", ForestObjectCategory.RoundLog));
            }

            Assert.That(sut.IsComplete, Is.True, "completado el acopio se habilita la colocación de la carga");
        }

        // --- contenido del asset, no de la clase -----------------------------------------------
        // Los parámetros y los textos viven fuera del código (CT-05, RNF-18), así que RF-24, CP-03
        // y RNF-01 hay que verificarlos sobre el asset. Mismo criterio que en HintPolicyTests.

        [Test]
        [Category("Internal")]
        public void WheelLevelConfig_RNF18_NingunParametroDeLaFase1EstaEnElCodigo()
        {
            var asset = ConfiguracionDelNivel2();
            var otra = ConfiguracionDePrueba();

            Assert.That(otra.RequiredLogs, Is.Not.EqualTo(asset.RequiredLogs),
                "premisa de la prueba: las dos configuraciones piden cantidades distintas");
            Assert.That(new PatternSelection(asset).Required, Is.EqualTo(asset.RequiredLogs));
            Assert.That(new PatternSelection(otra).Required, Is.EqualTo(TroncosDePrueba),
                "cambiar el asset cambia la regla sin recompilar");
            Assert.That(new PatternSelection(otra).CounterText, Does.StartWith("acopiados"),
                "hasta el texto del contador sale del asset");
        }

        [Test]
        public void WheelLevelConfig_RF24_ElAssetPideCincoTroncosEntreAlMenosOchoDistractores()
        {
            var asset = ConfiguracionDelNivel2();
            var porCategoria = asset.ForestObjects
                .GroupBy(objeto => objeto.Category)
                .ToDictionary(grupo => grupo.Key, grupo => grupo.Count());

            Assert.That(asset.RequiredLogs, Is.EqualTo(5), "cinco troncos redondos (guion §6.1.2)");
            Assert.That(asset.MinimumDistractors, Is.GreaterThanOrEqualTo(8), "al menos ocho distractores");
            Assert.That(porCategoria[ForestObjectCategory.RoundLog], Is.GreaterThanOrEqualTo(asset.RequiredLogs),
                "el catálogo trae al menos los troncos que la fase pide");
            Assert.That(asset.ForestObjects.Count(objeto => objeto.Category != ForestObjectCategory.RoundLog),
                Is.GreaterThanOrEqualTo(asset.MinimumDistractors), "y los distractores que declara");
            Assert.That(asset.ForestObjects.Select(objeto => objeto.Id).Distinct().Count(),
                Is.EqualTo(asset.ForestObjects.Length), "cada objeto tiene identidad propia");
            Assert.That(Enum.GetValues(typeof(ForestObjectCategory)).Cast<ForestObjectCategory>(),
                Is.SubsetOf(porCategoria.Keys), "las tres clases de distractor están presentes");
        }

        [Test]
        public void WheelLevelConfig_RNF23_CadaObjetoDelBosqueTraeSuIlustracion()
        {
            // El arte se cambia repuntando este campo en el asset: si alguien añade un objeto y se
            // olvida del sprite, sale aquí y no en pantalla como un hueco vacío.
            var sinArte = ConfiguracionDelNivel2().ForestObjects
                .Where(objeto => objeto.Art == null)
                .Select(objeto => objeto.Id)
                .ToArray();

            Assert.That(sinArte, Is.Empty);
        }

        [Test]
        public void WheelLevelConfig_RF11_ElAciertoDevuelveLaPreguntaQueAbreElPatron()
        {
            var acierto = ConfiguracionDelNivel2().MessageFor(ForestObjectCategory.RoundLog);

            Assert.That(acierto, Does.Contain("?"),
                "el acierto pregunta en vez de calificar: el patrón lo nombra el estudiante (§6.1.3)");
            Assert.That(acierto, Does.Not.Contain("redond"),
                "la retroalimentación no entrega la respuesta (CP-06)");
        }

        [Test]
        public void WheelLevelConfig_CP03_NingunMensajeDeLaFase1TraeCifras()
        {
            // El contador «Troncos redondos: n de 5» sí lleva cifras y está fuera de este barrido a
            // propósito: RF-24 lo exige y es estado de tarea, no desempeño. Los mensajes de
            // retroalimentación, en cambio, no llevan ninguna (CP-03, RF-17).
            var conCifras = ConfiguracionDelNivel2().Feedback
                .Where(entrada => (entrada.Message ?? string.Empty).Any(char.IsDigit))
                .Select(entrada => $"{entrada.Category}: «{entrada.Message}»")
                .ToArray();

            Assert.That(conCifras, Is.Empty);
        }

        [Test]
        public void WheelLevelConfig_RNF01_NingunMensajeDeLaFase1Supera20Palabras()
        {
            var asset = ConfiguracionDelNivel2();
            var excedidas = asset.Feedback
                .SelectMany(entrada => Oraciones(entrada.Message)
                    .Where(oracion => Palabras(oracion) > MaximoPalabrasPorOracion)
                    .Select(oracion => $"{entrada.Category} · {Palabras(oracion)} palabras: «{oracion}»"))
                .ToArray();

            Assert.That(excedidas, Is.Empty);
            Assert.That(asset.Feedback.Select(entrada => entrada.Category),
                Is.EquivalentTo(Enum.GetValues(typeof(ForestObjectCategory)).Cast<ForestObjectCategory>()),
                "hay un mensaje por categoría y ninguno repetido (guion §6.1.2)");
        }

        [Test]
        public void WheelLevelConfig_RF23_CadaCategoriaDelBosqueSeDibujaConUnSoloSprite()
        {
            // Un sprite por categoría y no uno por objeto. Con arte generado, cinco troncos
            // «parecidos pero no iguales» son cinco oportunidades de que uno deje de leerse como
            // redondo; uno solo, repetido, no puede desmentir el patrón que RF-23 pide encontrar.
            var porCategoria = ConfiguracionDelNivel2().ForestObjects
                .GroupBy(objeto => objeto.Category)
                .ToDictionary(grupo => grupo.Key,
                    grupo => grupo.Select(objeto => objeto.Art).Distinct().ToArray());

            var conVariasIlustraciones = porCategoria
                .Where(par => par.Value.Length != 1)
                .Select(par => $"{par.Key} usa {par.Value.Length} sprites")
                .ToArray();

            Assert.That(conVariasIlustraciones, Is.Empty, string.Join(" · ", conVariasIlustraciones));

            var sprites = porCategoria.Values.SelectMany(unos => unos).ToArray();
            Assert.That(sprites.Distinct().Count(), Is.EqualTo(sprites.Length),
                "ninguna categoría comparte sprite con otra: el patrón se busca mirando (RF-23)");
        }

        [Test]
        public void WheelLevelConfig_RF22_DosObjetosDelMismoSpriteNoAparecenEnLaMismaPostura()
        {
            // Compartir sprite solo funciona si la postura cambia: catorce copias idénticas serían
            // papel pintado, no un bosque, y el ojo dejaría de recorrerlo.
            var repetidas = ConfiguracionDelNivel2().ForestObjects
                .GroupBy(objeto => objeto.Art)
                .SelectMany(porSprite => porSprite
                    .GroupBy(objeto => (objeto.RotationDegrees, objeto.Mirrored))
                    .Where(postura => postura.Count() > 1)
                    .Select(postura => string.Join(" y ", postura.Select(objeto => objeto.Id))))
                .ToArray();

            Assert.That(repetidas, Is.Empty,
                $"aparecen calcados: {string.Join(" · ", repetidas)}");
        }

        // --- helpers ---------------------------------------------------------------------------

        private static WheelLevelConfig ConfiguracionDePrueba() => WheelLevelConfig.Create(
            TroncosDePrueba,
            DistractoresDePrueba,
            "acopiados {0} de {1}",
            new[] { Tronco1, Tronco2, Piedra, Planta, Herramienta },
            new[]
            {
                new CategoryFeedback(ForestObjectCategory.RoundLog, "acierto"),
                new CategoryFeedback(ForestObjectCategory.Stone, "piedra"),
                new CategoryFeedback(ForestObjectCategory.Plant, "planta"),
                new CategoryFeedback(ForestObjectCategory.Tool, "herramienta")
            });

        private static WheelLevelConfig ConfiguracionDelNivel2()
        {
            var config = AssetDatabase.FindAssets($"t:{nameof(WheelLevelConfig)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<WheelLevelConfig>)
                .FirstOrDefault(asset => asset != null);

            Assert.That(config, Is.Not.Null, "falta el asset de configuración del Nivel 2");
            return config;
        }

        /// <summary>
        /// Parte el texto en oraciones. Los signos de apertura «¿» y «¡» no cierran nada, así que
        /// solo cuentan los de cierre más el punto y los puntos suspensivos.
        /// </summary>
        private static IEnumerable<string> Oraciones(string text) => (text ?? string.Empty)
            .Replace("…", ".")
            .Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(oracion => oracion.Trim())
            .Where(oracion => oracion.Length > 0);

        private static int Palabras(string oracion) =>
            oracion.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
