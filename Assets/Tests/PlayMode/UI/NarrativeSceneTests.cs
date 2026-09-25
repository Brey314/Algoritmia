using System;
using System.Linq;
using System.Threading.Tasks;
using Game.Audio;
using Game.Core;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.UI.Tests
{
    [Category("Integration")]
    public class NarrativeSceneTests
    {
        private const string SceneName = "Narrative";

        [TearDown]
        public void DestruirLosObjetosPersistentes() => LimpiarObjetosPersistentes();

        /// <summary>
        /// Borra los objetos con <c>DontDestroyOnLoad</c>. No basta con hacerlo en el
        /// <c>[TearDown]</c>: sobreviven a <c>LoadSceneMode.Single</c>, así que una prueba que
        /// abre la escena **dos veces** encuentra vivo el runner de la vuelta anterior,
        /// <c>GameFlowRunner.Awake</c> destruye el duplicado (RNF-16) y la prueba se queda con una
        /// referencia muerta cuya FSM nunca salió de <c>Boot</c>.
        /// </summary>
        private static void LimpiarObjetosPersistentes()
        {
            foreach (var runner in Object.FindObjectsByType<GameFlowRunner>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(runner.gameObject);
            }

            foreach (var loader in Object.FindObjectsByType<SceneLoader>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(loader.gameObject);
            }
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RF05_ResuelveTresSecuenciasDistintasSinRamas()
        {
            var primeras = new string[3];
            var ids = new[] { "N1_Apertura", "N1_AparicionGuia", "N1_Hallazgo" };

            for (var i = 0; i < ids.Length; i++)
            {
                var (controller, _) = await OpenNarrative(ids[i]);
                primeras[i] = controller.BodyLabel.text;
            }

            // La misma escena y el mismo código resolvieron las tres: cada una empieza por su
            // propia línea y ninguna se parece a las otras.
            Assert.That(primeras, Has.All.Not.Empty);
            Assert.That(primeras.Distinct().Count(), Is.EqualTo(3));
        }

        [Test]
        [Timeout(60000)]
        public async Task NarrativeScene_RF05_ResuelveLasSieteSecuenciasDelNivel2SinRamas()
        {
            var ids = new[]
            {
                "N2_PuenteI", "N2_PuenteI_Bosque", "N2_Escena21_Bosque", "N2_Escena22_ElPatron",
                "N2_Escena23_Construccion", "N2_Escena24_Regreso", "N2_Escena25_Cierre"
            };
            var primeras = new string[ids.Length];

            for (var i = 0; i < ids.Length; i++)
            {
                var (controller, _) = await OpenNarrative(ids[i]);
                primeras[i] = controller.BodyLabel.text;

                foreach (var _ in SequenceNamed(controller, ids[i]).Lines)
                {
                    Click(controller.AdvanceButton);
                }

                Assert.That(controller.Dialogue.IsFinished, Is.True, $"{ids[i]} se recorre entera");
            }

            // Seis escenas más, cero ramas: el Nivel 2 no añadió ni un `if` al controlador.
            Assert.That(primeras, Has.All.Not.Empty);
            Assert.That(primeras.Distinct().Count(), Is.EqualTo(ids.Length));
        }

        [Test]
        [Timeout(60000)]
        public async Task NarrativeScene_RF05_ResuelveLasCincoSecuenciasDelNivel3SinRamas()
        {
            // Cinco escenas del guion en siete assets: el puente II corta dos veces —del refugio
            // en la cueva al horizonte y de ahí al río— y la ilustración es por secuencia (R04).
            var ids = new[]
            {
                "N3_PuenteII", "N3_PuenteII_Horizonte", "N3_PuenteII_Rio", "N3_Escena31_Llegada",
                "N3_Escena32_PrimerIntento", "N3_Escena33_Cruce", "N3_EscenaFinal"
            };
            var primeras = new string[ids.Length];

            for (var i = 0; i < ids.Length; i++)
            {
                var (controller, _) = await OpenNarrative(ids[i], LevelId.River);
                primeras[i] = controller.BodyLabel.text;

                foreach (var _ in SequenceNamed(controller, ids[i]).Lines)
                {
                    Click(controller.AdvanceButton);
                }

                Assert.That(controller.Dialogue.IsFinished, Is.True, $"{ids[i]} se recorre entera");
            }

            Assert.That(primeras, Has.All.Not.Empty);
            Assert.That(primeras.Distinct().Count(), Is.EqualTo(ids.Length));
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RF05_LaPrimeraLineaEsLaDelAssetPedido()
        {
            var (controller, _) = await OpenNarrative("N1_Hallazgo");
            var esperada = SequenceNamed(controller, "N1_Hallazgo").Lines[0];

            Assert.That(controller.BodyLabel.text, Is.EqualTo(esperada.Text));
            Assert.That(controller.SpeakerLabel.text, Is.EqualTo(esperada.Speaker));
        }

        [Test]
        [Timeout(60000)]
        public async Task NarrativeScene_RF10_LaAperturaEncadenaLasTresEscenasDelNivel1YEntraAJugar()
        {
            // La apertura del guion §3.1 y detrás las escenas 1.1 y 1.2 del §4, encadenadas
            // por el asset y sin un solo `if` en el controlador (RF-05).
            var esperadas = new[] { "N1_Apertura", "N1_AparicionGuia", "N1_Hallazgo" };
            var (controller, runner) = await OpenNarrative("N1_Apertura");
            var vistas = new System.Collections.Generic.List<string>();

            while (runner.Flow.Current == GameState.Narrative && vistas.Count < esperadas.Length + 1)
            {
                var id = runner.Flow.NarrativeSequenceId;
                vistas.Add(id);
                foreach (var _ in SequenceNamed(controller, id).Lines)
                {
                    Click(controller.AdvanceButton);
                }

                Assert.That(controller.Dialogue.IsFinished, Is.True, $"{id} se recorre entera");
                if (runner.Flow.Current == GameState.Narrative)
                {
                    controller.Begin(); // la escena se recarga con el id nuevo
                    await Awaitable.NextFrameAsync();
                }
            }

            Assert.That(vistas, Is.EqualTo(esperadas), "las tres, en orden");
            // Y desemboca en la cueva jugable, no en el menú: el recorrido queda cerrado (RNF-13).
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Playing), "sale a jugar");
            Assert.That(runner.Flow.PlayingLevel, Is.EqualTo(LevelId.Fire), "el nivel de la secuencia");
            Assert.That(runner.Flow.PlayingPhase, Is.EqualTo(1), "la fase que declara el asset");
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_INC28_NoMuestraOmitirLaPrimeraVezQueSeVeLaEscena()
        {
            var (controller, _) = await OpenNarrative("N1_Apertura");

            Assert.That(controller.SkipButton.gameObject.activeInHierarchy, Is.False);
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_HU17_NoHayBotonDePausaEnUnaEscenaNarrativa()
        {
            await OpenNarrative("N1_Apertura");

            var pausa = Object.FindObjectsByType<Button>(FindObjectsInactive.Include)
                .Where(button => button.GetComponentInChildren<Text>(true) is { } text
                                 && text.text.Trim().Equals("Pausa", StringComparison.OrdinalIgnoreCase));

            Assert.That(pausa, Is.Empty);
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RNF01_LaLineaMasLargaCabeEnSuCuadroDeDialogo()
        {
            var (controller, _) = await OpenNarrative("N1_Hallazgo");
            var masLarga = controller.Sequences
                .SelectMany(sequence => sequence.Lines)
                .OrderByDescending(line => line.Text.Length)
                .First();

            controller.BodyLabel.text = masLarga.Text;
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            var caja = controller.BodyLabel.rectTransform.rect;
            var generador = controller.BodyLabel.cachedTextGenerator;
            var alto = generador.GetPreferredHeight(masLarga.Text,
                controller.BodyLabel.GetGenerationSettings(caja.size)) / controller.BodyLabel.pixelsPerUnit;

            Assert.That(alto, Is.LessThanOrEqualTo(caja.height),
                $"«{masLarga.Text}» desborda su cuadro: {alto:0} px en una caja de {caja.height:0} px");
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RF10_TerminarLaEscena21EntraAJugarLaFase1DelBosque()
        {
            var (controller, runner) = await OpenNarrative("N2_Escena21_Bosque", LevelId.Wheel);
            var lineas = SequenceNamed(controller, "N2_Escena21_Bosque").Lines.Length;

            for (var i = 0; i < lineas; i++)
            {
                Click(controller.AdvanceButton);
            }

            // La escena de apertura de una fase desemboca en la fase, no en el menú: con la
            // salida provisional a `LevelSelect` el bosque era inalcanzable jugando.
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Playing), "sale a jugar");
            Assert.That(runner.Flow.PlayingLevel, Is.EqualTo(LevelId.Wheel), "el nivel de la secuencia");
            Assert.That(runner.Flow.PlayingPhase, Is.EqualTo(1), "la fase que declara el asset");
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RF05_LaEscenaPuenteEncadenaConLaEscena21()
        {
            // Hasta el 15/09/2026 `N2_PuenteI` no declaraba salida y caía al menú: el Nivel 2
            // abría en la 2.1 y el puente no se veía nunca. Desde el Checkpoint W-F el nivel abre
            // con el puente y este encadena con la 2.1 por su asset (RF-05, Camara_Narrativa_N2 §5).
            // Desde el 23/09/2026 el puente son dos assets: el amanecer junto a la cueva y, desde
            // «La familia sale a recolectar», el bosque.
            var (controller, runner) = await OpenNarrative("N2_PuenteI", LevelId.Wheel);
            var vistas = new System.Collections.Generic.List<string>();

            while (runner.Flow.Current == GameState.Narrative && runner.Flow.NarrativeSequenceId != "N2_Escena21_Bosque" && vistas.Count < 3)
            {
                var id = runner.Flow.NarrativeSequenceId;
                vistas.Add(id);
                foreach (var _ in SequenceNamed(controller, id).Lines)
                {
                    Click(controller.AdvanceButton);
                }

                if (runner.Flow.Current == GameState.Narrative)
                {
                    controller.Begin(); // la escena se recarga con el id nuevo
                    await Awaitable.NextFrameAsync();
                }
            }

            Assert.That(vistas, Is.EqualTo(new[] { "N2_PuenteI", "N2_PuenteI_Bosque" }), "el amanecer y luego el bosque");
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Narrative), "el puente no sale al menú");
            Assert.That(runner.Flow.NarrativeSequenceId, Is.EqualTo("N2_Escena21_Bosque"), "encadena con la 2.1");
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RNF23_LaIlustracionCubreLaPantallaSinDeformarse()
        {
            var (controller, _) = await OpenNarrative("N2_Escena21_Bosque", LevelId.Wheel);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            var sprite = SequenceNamed(controller, "N2_Escena21_Bosque").Illustration;
            Assert.That(sprite, Is.Not.Null, "el Nivel 2 ya tiene entorno: la secuencia lo declara");

            var caja = EnPantalla(controller.IllustrationRect);
            var pantalla = new Rect(0f, 0f, Screen.width, Screen.height);

            // Cubre —ningún borde de la ventana queda sin pintar— y no encaja: recorta lo que
            // sobre por un eje en vez de deformar el dibujo. Vale a la resolución del arte de hoy
            // y a la definitiva: solo cambia el archivo (RNF-23).
            Assert.That(caja.xMin, Is.LessThanOrEqualTo(pantalla.xMin + 0.5f), "cubre por la izquierda");
            Assert.That(caja.yMin, Is.LessThanOrEqualTo(pantalla.yMin + 0.5f), "cubre por abajo");
            Assert.That(caja.xMax, Is.GreaterThanOrEqualTo(pantalla.xMax - 0.5f), "cubre por la derecha");
            Assert.That(caja.yMax, Is.GreaterThanOrEqualTo(pantalla.yMax - 0.5f), "cubre por arriba");
            Assert.That(caja.width / caja.height, Is.EqualTo(sprite.rect.width / sprite.rect.height).Within(0.01f),
                "y conserva la proporción del dibujo");
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RF05_LaCamaraSeMuevePorElEntornoAlRitmoDeLaNarrativa()
        {
            var (controller, _) = await OpenNarrative("N2_Escena22_ElPatron", LevelId.Wheel);
            var secuencia = SequenceNamed(controller, "N2_Escena22_ElPatron");
            Assume.That(secuencia.CameraStart.Focus, Is.Not.EqualTo(secuencia.CameraEnd.Focus),
                "la escena declara un recorrido de cámara");
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            var alAbrir = controller.IllustrationRect.anchoredPosition;
            var objetivoInicial = controller.CameraTarget.Focus;

            // Sin avanzar, la cámara descansa: es la narrativa la que la mueve, no el reloj.
            await EsperarSegundos(0.4f);
            Assert.That(controller.IllustrationRect.anchoredPosition, Is.EqualTo(alAbrir),
                "sin leer no hay recorrido");

            for (var i = 0; i < secuencia.Lines.Length / 2; i++)
            {
                Click(controller.AdvanceButton);
            }

            Assert.That(controller.CameraTarget.Focus, Is.Not.EqualTo(objetivoInicial),
                "a mitad de la lectura el encuadre pedido ya es otro");
            await EsperarSegundos(0.6f);
            var aMitad = controller.IllustrationRect.anchoredPosition;

            Assert.That(aMitad, Is.Not.EqualTo(alAbrir), "y la ilustración se ha movido hacia él");
            var pantalla = new Rect(0f, 0f, Screen.width, Screen.height);
            var caja = EnPantalla(controller.IllustrationRect);
            Assert.That(caja.xMin <= pantalla.xMin + 0.5f && caja.xMax >= pantalla.xMax - 0.5f
                        && caja.yMin <= pantalla.yMin + 0.5f && caja.yMax >= pantalla.yMax - 0.5f, Is.True,
                "en pleno movimiento sigue cubriendo la pantalla: no se descubre ningún borde");
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RF05_ElCorteSecoFundeANegroSaltaAOscurasYVuelve()
        {
            var (controller, _) = await OpenNarrative("N1_Apertura");
            var secuencia = SequenceNamed(controller, "N1_Apertura");
            var corte = secuencia.CameraKeys.First(k => k.HardCut);
            var antes = secuencia.CameraKeys.Last(k => k.Line < corte.Line).Framing.Focus;
            Assume.That(corte.Framing.Focus, Is.Not.EqualTo(antes),
                "el corte seco de la apertura salta a otro sitio del lienzo");
            Assume.That(secuencia.HardCutFadeSeconds, Is.GreaterThan(0f), "la escena declara fundido");

            for (var i = 0; i < corte.Line; i++)
            {
                Click(controller.AdvanceButton);
            }

            await Awaitable.NextFrameAsync();
            Assert.That(controller.CutFade, Is.LessThan(1f), "el fundido arrancó");
            // Donde quedó la cámara al empezar el fundido: no tiene por qué ser el encuadre
            // anterior exacto, porque venía suavizándose hacia él.
            var congelado = controller.CameraCurrent.Focus;

            // La cámara nunca está a medio camino: o en el encuadre viejo o en el del corte. El
            // salto ocurre dentro del negro y por eso nadie ve recorrer el lienzo.
            var masOscuro = 1f;
            var inicio = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - inicio < secuencia.HardCutFadeSeconds + 0.2f)
            {
                masOscuro = Mathf.Min(masOscuro, controller.CutFade);
                Assert.That(controller.CameraCurrent.Focus,
                    Is.EqualTo(congelado).Or.EqualTo(corte.Framing.Focus),
                    "durante el corte la cámara no se suaviza: o quieta, o ya en el encuadre nuevo");
                await Awaitable.NextFrameAsync();
            }

            // El mínimo exacto depende de dónde caiga el cuadro respecto al punto negro: basta con
            // comprobar que la imagen se apagó de verdad, no que tocó el cero.
            Assert.That(masOscuro, Is.LessThan(0.2f), "la imagen llegó a negro por el camino");
            Assert.That(controller.CutFade, Is.EqualTo(1f), "y volvió del fundido");
            Assert.That(controller.CameraCurrent.Focus, Is.EqualTo(corte.Framing.Focus), "la cámara saltó");
            Assert.That(controller.LightCurrent.Ambient, Is.EqualTo(corte.Light.Ambient), "y la luz también");
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RF05_LaEscenaQueAbreEnNegroFundeAunqueSuPrimeraParadaSeaLaLinea0()
        {
            // El fundido de entrada y la primera parada llegan en el mismo cuadro. Cuando la
            // parada lo cancelaba, la 1.2 entraba de golpe desde el negro en que la deja el
            // apagón de Algoritm — justo el salto que el fundido viene a tapar. Las escenas 2.1
            // y 2.3 no lo notaban porque su primera parada no cae en la línea 0.
            var (controller, _) = await OpenNarrative("N1_Hallazgo");
            var secuencia = SequenceNamed(controller, "N1_Hallazgo");
            Assume.That(secuencia.OpensFromBlack, Is.True, "la escena abre en negro");
            Assume.That(secuencia.CameraKeys[0].Line, Is.Zero, "y su primera parada cae en la línea 0");

            Assert.That(controller.CutFade, Is.LessThan(1f),
                "el fundido de entrada sigue vivo después del primer cuadro");

            await EsperarSegundos(secuencia.HardCutFadeSeconds + 0.2f);
            Assert.That(controller.CutFade, Is.EqualTo(1f), "y termina de entrar");
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RF05_ElDestelloDuraSusSegundosYVuelveALaLuzAnterior()
        {
            var (controller, _) = await OpenNarrative("N1_Hallazgo");
            var secuencia = SequenceNamed(controller, "N1_Hallazgo");
            var destello = secuencia.CameraKeys.First(k => k.Light.FlashSeconds > 0f);
            var anterior = secuencia.CameraKeys.Last(k => k.Line < destello.Line);

            for (var i = 0; i < destello.Line; i++)
            {
                Click(controller.AdvanceButton);
            }

            await Awaitable.NextFrameAsync();
            Assert.That(controller.LightCurrent.Ambient, Is.EqualTo(destello.Light.Ambient),
                "el ¡CLIC! se aplica de golpe, sin suavizado");

            await EsperarSegundos(destello.Light.FlashSeconds + 0.1f);
            Assert.That(controller.LightCurrent.Ambient, Is.EqualTo(anterior.Light.Ambient).Within(0.001f),
                "y pasado su tiempo vuelve de golpe a la luz de la parada anterior");
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RNF03_ElCuadroDeDialogoNoSuperaElCuartoDeLaPantalla()
        {
            var (controller, _) = await OpenNarrative("N1_Apertura");
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            // Cuerpo → Fondo → CuadroDialogo: el cuadro entero, con su marco.
            var cuadro = (RectTransform)controller.BodyLabel.transform.parent.parent;
            var caja = EnPantalla(cuadro);

            // El texto acompaña a la imagen, no la tapa: como mucho el cuarto inferior de la pantalla.
            Assert.That(caja.yMax, Is.LessThanOrEqualTo(Screen.height * 0.25f + 0.5f),
                $"el cuadro llega a y={caja.yMax:0} y el cuarto de pantalla acaba en {Screen.height * 0.25f:0}");
            Assert.That(caja.yMin, Is.GreaterThanOrEqualTo(-0.5f), "y no se sale por abajo");
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RF20_ElCierreDelFuegoPintaElMontonConLaLlamaAnimada()
        {
            var (controller, _) = await OpenNarrative("N1_NacimientoDelFuego", LevelId.Fire);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            // Lo que dice el texto se ve: «una llama... crece despacio desde las hojas».
            Assert.That(controller.Props.Select(p => p.Prop.Art.name), Has.Some.EqualTo("prop_n1_monton_hojas"),
                "el montón de hojas visto de lado está pintado");
            var monton = controller.Props.First(p => p.Prop.Art.name == "prop_n1_monton_hojas");
            Assert.That(monton.Rect.GetComponent<BurnReveal>(), Is.Not.Null, "y se ve quemado bajo la llama");
            var animados = controller.Props
                .Select(p => p.Rect.GetComponent<Animator>())
                .Where(animator => animator != null)
                .Select(animator => animator.runtimeAnimatorController.name)
                .ToArray();
            Assert.That(animados, Has.Some.EqualTo("prop_n1_fuego_normal"),
                "la llama vista de lado está animada (prop_n1_fuego_normal)");
            // «Un hilo de humo sube despacio»: aquí el fuego nace, así que el humo nace con él.
            Assert.That(animados, Has.Some.EqualTo("fx_n1_humo_nacer"), "y echa humo desde que nace");
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RF05_LaEscena21MuestraLosObjetosRepartidosPorElSuelo()
        {
            var (controller, _) = await OpenNarrative("N2_Escena21_Bosque", LevelId.Wheel);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            var secuencia = SequenceNamed(controller, "N2_Escena21_Bosque");

            // Lo que dice el texto se ve: «objetos dispersos por el suelo... a un lado, la caja».
            Assert.That(secuencia.Props.Length, Is.GreaterThanOrEqualTo(15),
                "la escena declara los catorce objetos del bosque y la caja");

            var pintados = Enumerable.Range(0, controller.IllustrationRect.childCount)
                .Select(i => controller.IllustrationRect.GetChild(i).GetComponent<Image>())
                .ToArray();
            Assert.That(pintados.Select(p => p.sprite), Is.EqualTo(secuencia.Props.Select(p => p.Art)),
                "cada objeto declarado se pinta con su ilustración, en orden");

            var entorno = EnPantalla(controller.IllustrationRect);
            var fuera = pintados
                .Where(p => !entorno.Overlaps(EnPantalla(p.rectTransform)))
                .Select(p => p.name)
                .ToArray();
            Assert.That(fuera, Is.Empty, "y todos caen sobre el entorno, no fuera de él");
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RF05_LaEscena22NoMueveLaVistaHastaQueElNinoHablaYLuegoPaneaAPapa()
        {
            var (controller, _) = await OpenNarrative("N2_Escena22_ElPatron", LevelId.Wheel);
            var secuencia = SequenceNamed(controller, "N2_Escena22_ElPatron");
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            var alAbrir = controller.IllustrationRect.anchoredPosition;

            // Línea 0: la vista con la que terminó el bosque, y quieta.
            Assert.That(controller.CameraTarget.Focus, Is.EqualTo(secuencia.CameraStart.Focus));
            Assert.That(controller.CameraTarget.Zoom, Is.EqualTo(secuencia.CameraStart.Zoom));
            await EsperarSegundos(0.4f);
            Assert.That(controller.IllustrationRect.anchoredPosition, Is.EqualTo(alAbrir), "no se mueve sola");

            // Línea 1, «¡Este tronco rueda!»: la cámara va al niño.
            Click(controller.AdvanceButton);
            var alNino = controller.CameraTarget;
            Assert.That(alNino.Focus, Is.Not.EqualTo(secuencia.CameraStart.Focus), "al hablar el niño la vista cambia");
            Assert.That(alNino.Zoom, Is.GreaterThan(secuencia.CameraStart.Zoom), "acercándose a él");
            await EsperarSegundos(0.6f);
            var conElNino = controller.IllustrationRect.anchoredPosition;

            // Línea 2, «Pero esa piedra no…»: paneo a la derecha, hacia papá, a la misma escala.
            Click(controller.AdvanceButton);
            var aPapa = controller.CameraTarget;
            Assert.That(aPapa.Focus.x, Is.GreaterThan(alNino.Focus.x), "papá está a la derecha del niño");
            await EsperarSegundos(0.6f);
            Assert.That(controller.IllustrationRect.anchoredPosition.x, Is.LessThan(conElNino.x),
                "y la ilustración se corre a la izquierda: la cámara paneó a la derecha");

            // Línea 3, «¿Qué diferencia hay?»: sigue al este y abre, entra la familia.
            Click(controller.AdvanceButton);
            var aLaFamilia = controller.CameraTarget;
            Assert.That(aLaFamilia.Focus.x, Is.GreaterThan(aPapa.Focus.x), "la familia está a la derecha de papá");
            Assert.That(aLaFamilia.Zoom, Is.LessThan(aPapa.Zoom), "y el plano abre");

            // Línea 4, «Los que ruedan... son redondos»: micro empuje sobre la niña, que nombra el patrón.
            Click(controller.AdvanceButton);
            var aLaNina = controller.CameraTarget;
            Assert.That(aLaNina.Zoom, Is.GreaterThan(aLaFamilia.Zoom), "se acerca a la niña");

            // Línea 5 en adelante, «Acabas de encontrar un patrón»: abre y sube, y ahí se queda.
            Click(controller.AdvanceButton);
            var elPatron = controller.CameraTarget;
            Assert.That(elPatron.Zoom, Is.LessThan(aLaNina.Zoom), "el patrón es el cuadro entero");
            for (var i = 5; i < secuencia.Lines.Length - 1; i++)
            {
                Click(controller.AdvanceButton);
                Assert.That(controller.CameraTarget.Focus, Is.EqualTo(elPatron.Focus),
                    "y la vista se queda mientras el guía cierra");
            }
        }

        [Test]
        [Timeout(60000)]
        public async Task NarrativeScene_RF23_LaEscena22AnimaLoQueCadaLineaCuenta()
        {
            var (controller, _) = await OpenNarrative("N2_Escena22_ElPatron", LevelId.Wheel);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            (NarrativeProp Prop, RectTransform Rect) Animado(int linea, PropMotion motion) =>
                controller.Props.First(p => p.Prop.MotionLine == linea && p.Prop.Motion == motion);

            // Línea 0: la caja vuelve a rodar sola —es narrativa, no hay botón— y los troncos
            // giran bajo ella.
            // La caja es lo único que rueda con recorrido en la línea 0; los troncos giran en el sitio.
            var caja = controller.Props.First(p => p.Prop.MotionLine == 0 && p.Prop.Motion == PropMotion.Roll && p.Prop.MotionDistance > 0f);
            var troncosBajoLaCaja = controller.Props.Where(p => p.Prop.MotionLine == 0 && p.Prop.Motion == PropMotion.Roll && p.Prop.MotionDistance == 0f).ToArray();
            var cajaAlEmpezar = caja.Rect.anchoredPosition;
            var giroAlEmpezar = troncosBajoLaCaja.First().Rect.localEulerAngles.z;
            await EsperarSegundos(caja.Prop.MotionSeconds * 0.4f);
            Assert.That(Mathf.DeltaAngle(caja.Prop.RotationDegrees, caja.Rect.localEulerAngles.z), Is.EqualTo(0f).Within(0.5f),
                "sobre los troncos la caja se desliza sin dar vueltas: los que giran son ellos");
            await EsperarSegundos(caja.Prop.MotionSeconds * 0.6f + 0.2f);

            Assert.That(caja.Rect.anchoredPosition.x, Is.GreaterThan(cajaAlEmpezar.x), "la caja rodó a la derecha");
            Assert.That(caja.Rect.anchoredPosition.y, Is.LessThan(cajaAlEmpezar.y), "y al pasar el último tronco cayó al suelo");
            Assert.That(caja.Rect.localEulerAngles.z, Is.Not.EqualTo(0f).Within(0.5f), "ladeada, como en el bosque");
            Assert.That(troncosBajoLaCaja, Is.Not.Empty, "hay troncos bajo la caja");
            Assert.That(troncosBajoLaCaja.First().Rect.localEulerAngles.z, Is.Not.EqualTo(giroAlEmpezar).Within(0.5f),
                "y giraron bajo ella");

            // Línea 1: el niño levanta un tronco, lo suelta y rueda.
            var tronco = Animado(1, PropMotion.LiftAndRoll);
            var troncoAlEmpezar = tronco.Rect.anchoredPosition;
            Click(controller.AdvanceButton);
            await EsperarSegundos(tronco.Prop.MotionSeconds * 0.25f);
            Assert.That(tronco.Rect.anchoredPosition.y, Is.GreaterThan(troncoAlEmpezar.y), "primero lo levantan");
            await EsperarSegundos(tronco.Prop.MotionSeconds * 0.75f + 0.2f);
            Assert.That(tronco.Rect.anchoredPosition.x, Is.GreaterThan(troncoAlEmpezar.x), "y al soltarlo rueda");
            Assert.That(tronco.Rect.anchoredPosition.y, Is.EqualTo(troncoAlEmpezar.y).Within(0.5f), "por el suelo");

            // Línea 2: lo mismo con una piedra, que no rueda: cae donde la sueltan.
            var piedra = Animado(2, PropMotion.LiftAndStay);
            var piedraAlEmpezar = piedra.Rect.anchoredPosition;
            Click(controller.AdvanceButton);
            await EsperarSegundos(piedra.Prop.MotionSeconds * 0.25f);
            Assert.That(piedra.Rect.anchoredPosition.y, Is.GreaterThan(piedraAlEmpezar.y), "también la levantan");
            await EsperarSegundos(piedra.Prop.MotionSeconds * 0.75f + 0.2f);
            Assert.That(piedra.Rect.anchoredPosition, Is.EqualTo(piedraAlEmpezar).Using<Vector2>((a, b) => Vector2.Distance(a, b) < 0.5f ? 0 : 1),
                "pero cae donde estaba: lo anguloso no rueda (RF-23)");
        }

        /// <summary>
        /// Lo que se ve en la 2.2 se oye cuando pasa: cada objeto suena **al tocar el suelo**, no
        /// al empezar su línea. La caja, al terminar de caer pasado el último tronco; el tronco y la
        /// piedra, cuando los sueltan, a mitad de su movimiento.
        /// </summary>
        [Test]
        [Timeout(60000)]
        public async Task NarrativeScene_RF26_LaEscena22SuenaCuandoCadaObjetoTocaElSuelo()
        {
            var audio = new GameObject("TestAudio").AddComponent<AudioManager>();
            try
            {
                var (controller, _) = await OpenNarrative("N2_Escena22_ElPatron", LevelId.Wheel);
                var secuencia = SequenceNamed(controller, "N2_Escena22_ElPatron");
                var caja = controller.Props.First(p => p.Prop.MotionDrop > 0f);
                var tronco = controller.Props.First(p => p.Prop.MotionLine == 1 && p.Prop.Motion == PropMotion.LiftAndRoll);
                var piedra = controller.Props.First(p => p.Prop.MotionLine == 2 && p.Prop.Motion == PropMotion.LiftAndStay);
                Assert.That(caja.Prop.LandSound.name, Is.EqualTo("sfx_n2_piedra_cae"), "la caja cae con piedra_cae");
                Assert.That(tronco.Prop.LandSound.name, Is.EqualTo("sfx_n2_troncos"), "el tronco cae con troncos");
                Assert.That(piedra.Prop.LandSound.name, Is.EqualTo("sfx_n2_piedra_cae"), "la piedra cae con piedra_cae");
                Assert.That(secuencia.Lines[1].Sound, Is.Null, "la línea del niño no suena al aparecer: suena el tronco al caer");
                Assert.That(secuencia.Lines[2].Sound, Is.Null, "ni la de papá: suena la piedra al caer");

                await Suena(audio, caja.Prop, alCaer: 1f);

                Click(controller.AdvanceButton);
                await Suena(audio, tronco.Prop, alCaer: 0.5f);

                Click(controller.AdvanceButton);
                await Suena(audio, piedra.Prop, alCaer: 0.5f);
            }
            finally
            {
                Object.DestroyImmediate(audio.gameObject);
            }
        }

        /// <summary>
        /// Que el objeto no suene antes de caer y suene una vez al caer. <paramref name="alCaer"/>
        /// es la fracción de su movimiento en la que toca el suelo.
        /// </summary>
        private static async Task Suena(AudioManager audio, NarrativeProp prop, float alCaer)
        {
            var antes = audio.SfxCount;
            await EsperarSegundos(prop.MotionSeconds * alCaer * 0.6f);
            Assert.That(audio.SfxCount, Is.EqualTo(antes), $"{prop.LandSound.name} no suena antes de caer");

            await EsperarSegundos(prop.MotionSeconds * alCaer * 0.4f + 0.25f);
            Assert.That(audio.SfxCount, Is.EqualTo(antes + 1), $"{prop.LandSound.name} suena una vez al caer");
            Assert.That(audio.LastSfx, Is.SameAs(prop.LandSound));
        }

        /// <summary>
        /// Recorre las seis escenas del Nivel 2 parada por parada, con la cámara y los movimientos
        /// ya asentados, y guarda una captura y un informe de dónde queda cada objeto pintado
        /// respecto a la pantalla y al cuadro de diálogo. Es la hoja de verificación de
        /// <c>Camara_Narrativa_N2.md</c> §10 sobre el motor real, no sobre recortes.
        /// </summary>
        [TestCase("N2_PuenteI")]
        [TestCase("N2_PuenteI_Bosque")]
        [TestCase("N2_Escena21_Bosque")]
        [TestCase("N2_Escena22_ElPatron")]
        [TestCase("N2_Escena23_Construccion")]
        [TestCase("N2_Escena24_Regreso")]
        [TestCase("N2_Escena25_Cierre")]
        [Timeout(120000)]
        [Category("VisualVerification")]
        [Description("Verificar en las capturas N2_*: cada parada muestra lo que su línea cuenta, ningún objeto " +
                     "pintado queda bajo el cuadro de diálogo mientras se mueve, la línea de árboles nunca sale " +
                     "del cuadro (R1), ningún encuadre cruza el eje del espejo (R2), y el paso entre paradas es " +
                     "un solo gesto continuo.")]
        public async Task NarrativeScene_RF05_CapturaCadaParadaDeLaEscenaDelNivel2(string id)
        {
            var informe = new System.Text.StringBuilder();
            var pantalla = new Rect(0f, 0f, Screen.width, Screen.height);

            var (controller, _) = await OpenNarrative(id, LevelId.Wheel);
            var secuencia = SequenceNamed(controller, id);
            var cuadro = (RectTransform)GameObject.Find("Canvas/CuadroDialogo").transform;
            var lineas = secuencia.Lines.Length;

            for (var linea = 0; linea < lineas; linea++)
            {
                if (linea > 0)
                {
                    Click(controller.AdvanceButton);
                }

                await Asentar(controller, secuencia, linea);
                Capturar($"N2_{id}_L{linea:00}");

                var camara = controller.CameraCurrent;
                informe.AppendLine(
                    FormattableString.Invariant($"{id} L{linea} foco=({camara.Focus.x:0.000},{camara.Focus.y:0.000}) zoom={camara.Zoom:0.00} ")
                    + FormattableString.Invariant($"arriba={camara.Focus.y + 0.5f / camara.Zoom:0.000} ")
                    + FormattableString.Invariant($"[{camara.Focus.x - 0.25f / camara.Zoom:0.000},{camara.Focus.x + 0.25f / camara.Zoom:0.000}]"));
                var diálogo = EnPantalla(cuadro);
                for (var i = 0; i < controller.Props.Count; i++)
                {
                    var (prop, rect) = controller.Props[i];
                    var caja = Dibujo(rect);
                    var enPantalla = caja.Overlaps(pantalla);
                    var bajoElCuadro = enPantalla && caja.Overlaps(diálogo);
                    var seMueve = prop.Motion != PropMotion.None && prop.MotionLine == linea;
                    informe.AppendLine(
                        FormattableString.Invariant($"   prop{i} ({prop.Position.x:0.000},{prop.Position.y:0.000}) ")
                        + FormattableString.Invariant($"y=[{caja.yMin / pantalla.height:0.00},{caja.yMax / pantalla.height:0.00}] ")
                        + FormattableString.Invariant($"x=[{caja.xMin / pantalla.width:0.00},{caja.xMax / pantalla.width:0.00}] ")
                        + (enPantalla ? "visible" : "fuera") + (bajoElCuadro ? " BAJO_EL_CUADRO" : "") + (seMueve ? " SE_MUEVE" : ""));
                }
            }

            var carpeta = $"{Application.persistentDataPath}/TestScreenshots";
            System.IO.Directory.CreateDirectory(carpeta);
            System.IO.File.WriteAllText($"{carpeta}/N2_informe_{id}.txt", informe.ToString());
            TestContext.WriteLine(informe.ToString());
        }

        /// <summary>
        /// Lo que de verdad se dibuja de un objeto pintado: la casilla es cuadrada y el sprite se
        /// ajusta dentro sin deformarse, así que una barra fina ocupa una franja de la casilla y
        /// no la casilla entera. Medir la casilla acusaba al tronco largo de estar bajo el cuadro
        /// cuando lo que estaba era vacío.
        /// </summary>
        private static Rect Dibujo(RectTransform rect)
        {
            var caja = EnPantalla(rect);
            var image = rect.GetComponent<Image>();
            if (image == null || image.sprite == null || !image.preserveAspect || caja.width <= 0f || caja.height <= 0f)
            {
                return caja;
            }

            var aspecto = image.sprite.rect.width / image.sprite.rect.height;
            var ancho = Mathf.Min(caja.width, caja.height * aspecto);
            var alto = Mathf.Min(caja.height, caja.width / aspecto);
            return new Rect(caja.center.x - ancho / 2f, caja.center.y - alto / 2f, ancho, alto);
        }

        /// <summary>Espera a que la cámara llegue a su parada y a que termine cualquier movimiento de la línea.</summary>
        private static async Task Asentar(NarrativeSceneController controller, NarrativeSequence secuencia, int linea)
        {
            var movimiento = secuencia.Props
                .Where(prop => prop.Motion != PropMotion.None && prop.MotionLine == linea)
                .Select(prop => prop.MotionSeconds)
                .DefaultIfEmpty(0f)
                .Max();
            // La cámara se acerca asintóticamente: se da por llegada cuando la diferencia con
            // el objetivo ya no se ve y el movimiento de la línea terminó, con un tope **en
            // segundos** por si nunca converge — en frames no vale: el Editor corre a más de
            // 200 fps en pruebas y 600 cuadros eran menos de 3 s, que no bastaban (12/09/2026).
            var inicio = Time.realtimeSinceStartup;
            var anterior = controller.CameraCurrent;
            var saltoMaximo = 0f;
            while (Time.realtimeSinceStartup - inicio < 12f)
            {
                var actual = controller.CameraCurrent;
                var objetivo = controller.CameraTarget;
                saltoMaximo = Mathf.Max(saltoMaximo, Vector2.Distance(actual.Focus, anterior.Focus) / Mathf.Max(Time.deltaTime, 0.0001f));
                anterior = actual;
                if (Time.realtimeSinceStartup - inicio > movimiento + 0.2f
                    && Vector2.Distance(actual.Focus, objetivo.Focus) < 0.0015f
                    && Mathf.Abs(actual.Zoom - objetivo.Zoom) < 0.004f)
                {
                    break;
                }

                await Awaitable.NextFrameAsync();
            }

            // Un solo gesto continuo: la velocidad del foco nunca pasa de un canvas entero por
            // segundo, que es lo que separaría un paneo de un salto (RNF-21, §5.6 del diseño).
            Assert.That(saltoMaximo, Is.LessThan(1f),
                $"la cámara saltó ({saltoMaximo:0.00} canvas/s) en vez de deslizarse hacia la parada de la línea {linea}");

            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
        }

        /// <summary>
        /// En cada parada de las seis escenas del Nivel 2, ningún objeto que se mueve en esa línea
        /// queda bajo el cuadro de diálogo, y ninguno de los visibles queda más que un poco. Es
        /// lo que pasaba en la 2.2 con el tronco del niño y la piedra de papá (12/09/2026).
        /// </summary>
        [TestCase("N2_PuenteI")]
        [TestCase("N2_PuenteI_Bosque")]
        [TestCase("N2_Escena21_Bosque")]
        [TestCase("N2_Escena22_ElPatron")]
        [TestCase("N2_Escena23_Construccion")]
        [TestCase("N2_Escena24_Regreso")]
        [TestCase("N2_Escena25_Cierre")]
        [Timeout(120000)]
        public async Task NarrativeScene_RNF03_NingunObjetoQueSeMueveQuedaBajoElCuadroDeDialogoEnElNivel2(string id)
        {
            var pantalla = new Rect(0f, 0f, Screen.width, Screen.height);
            var (controller, _) = await OpenNarrative(id, LevelId.Wheel);
            var secuencia = SequenceNamed(controller, id);
            var cuadro = (RectTransform)GameObject.Find("Canvas/CuadroDialogo").transform;

            for (var linea = 0; linea < secuencia.Lines.Length; linea++)
            {
                if (linea > 0)
                {
                    Click(controller.AdvanceButton);
                }

                await Asentar(controller, secuencia, linea);
                var diálogo = EnPantalla(cuadro);

                // R1 y R2 sobre lo que de verdad se ve, no sobre lo escrito.
                var camara = controller.CameraCurrent;
                Assert.That(camara.Focus.y + 0.5f / camara.Zoom, Is.GreaterThanOrEqualTo(IllustrationFraming.TreeLine - 0.01f),
                    $"{id} L{linea}: la línea de árboles sale del cuadro (R1)");
                Assert.That((camara.Focus.x - 0.25f / camara.Zoom < IllustrationFraming.MirrorAxis)
                            == (camara.Focus.x + 0.25f / camara.Zoom < IllustrationFraming.MirrorAxis), Is.True,
                    $"{id} L{linea}: el cuadro cruza el eje del espejo (R2)");

                for (var i = 0; i < controller.Props.Count; i++)
                {
                    var (prop, rect) = controller.Props[i];
                    var caja = Dibujo(rect);
                    if (!caja.Overlaps(pantalla))
                    {
                        continue; // Fuera de cuadro a propósito: la piedra de la 2.4 entra después.
                    }

                    var seMueve = prop.Motion != PropMotion.None && prop.MotionLine == linea;
                    var tapado = Mathf.Max(0f, diálogo.yMax - caja.yMin);
                    if (seMueve)
                    {
                        Assert.That(tapado, Is.LessThanOrEqualTo(0f),
                            $"{id} L{linea}: el objeto {i} que se mueve en esta línea queda {tapado:0} px bajo el cuadro de diálogo");
                    }
                    else
                    {
                        Assert.That(tapado, Is.LessThanOrEqualTo(caja.height * 0.15f),
                            $"{id} L{linea}: el objeto {i} queda {tapado:0} px bajo el cuadro de diálogo, más del 15 % de su alto");
                    }
                }
            }
        }

        /// <summary>
        /// Cuando habla alguien, el cuadro de diálogo lleva su retrato; en una acotación, que no
        /// tiene hablante, no hay retrato (Interfaces §1.1.5, mockup 5).
        /// </summary>
        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RF05_ElRetratoEsElDeQuienHablaYNoHayEnLasAcotaciones()
        {
            var (controller, _) = await OpenNarrative("N1_Hallazgo", LevelId.Fire);
            var secuencia = SequenceNamed(controller, "N1_Hallazgo");

            for (var linea = 0; linea < secuencia.Lines.Length; linea++)
            {
                if (linea > 0)
                {
                    Click(controller.AdvanceButton);
                }

                var hablante = secuencia.Lines[linea].Speaker;
                if (secuencia.Lines[linea].IsStageDirection)
                {
                    Assert.That(controller.PortraitFrame.activeSelf, Is.False, $"L{linea}: una acotación no lleva retrato");
                    continue;
                }

                Assert.That(controller.PortraitFrame.activeSelf, Is.True, $"L{linea}: habla {hablante} y se ve su retrato");
                Assert.That(controller.Portrait.sprite, Is.Not.Null, $"L{linea}: el retrato de {hablante} tiene sprite");
                var enEscena = controller.Actors.Where(actor => actor.Rig.Speaks(hablante)).Select(actor => actor.Rig.Portrait).ToArray();
                if (enEscena.Length > 0)
                {
                    Assert.That(enEscena, Has.Member(controller.Portrait.sprite), $"L{linea}: es el retrato de {hablante}, que está en escena");
                }
            }
        }

        /// <summary>
        /// En cada línea, cada personaje hace exactamente lo que dice su paso —o lo que mantiene del
        /// anterior, o gesticula si la dice él— y sale de donde debe: lo que el texto cuenta se ve
        /// cuando se lee (RF-05). Recorre las dieciocho escenas.
        /// </summary>
        [TestCase("N1_Apertura", LevelId.Fire)]
        [TestCase("N1_AparicionGuia", LevelId.Fire)]
        [TestCase("N1_Hallazgo", LevelId.Fire)]
        [TestCase("N1_NacimientoDelFuego", LevelId.Fire)]
        [TestCase("N2_PuenteI", LevelId.Wheel)]
        [TestCase("N2_PuenteI_Bosque", LevelId.Wheel)]
        [TestCase("N2_Escena21_Bosque", LevelId.Wheel)]
        [TestCase("N2_Escena22_ElPatron", LevelId.Wheel)]
        [TestCase("N2_Escena23_Construccion", LevelId.Wheel)]
        [TestCase("N2_Escena24_Regreso", LevelId.Wheel)]
        [TestCase("N2_Escena25_Cierre", LevelId.Wheel)]
        [TestCase("N3_PuenteII", LevelId.River)]
        [TestCase("N3_PuenteII_Horizonte", LevelId.River)]
        [TestCase("N3_PuenteII_Rio", LevelId.River)]
        [TestCase("N3_Escena31_Llegada", LevelId.River)]
        [TestCase("N3_Escena32_PrimerIntento", LevelId.River)]
        [TestCase("N3_Escena33_Cruce", LevelId.River)]
        [TestCase("N3_EscenaFinal", LevelId.River)]
        [Timeout(240000)]
        public async Task NarrativeScene_RF05_CadaPersonajeHaceLoQueDiceSuPasoCuandoSeLeeLaLinea(string id, LevelId nivel)
        {
            var (controller, _) = await OpenNarrative(id, nivel);
            var secuencia = SequenceNamed(controller, id);
            Assert.That(controller.Actors, Is.Not.Empty, $"{id}: la familia o el guía están en la escena");

            for (var linea = 0; linea < secuencia.Lines.Length; linea++)
            {
                if (linea > 0)
                {
                    Click(controller.AdvanceButton);
                }

                var hablante = secuencia.Lines[linea].Speaker;
                foreach (var (prop, rect, rig, _) in controller.Actors)
                {
                    var cue = ActorTimeline.Cue(prop, linea, rig.Speaks(hablante));
                    Assert.That(rig.Current, Is.EqualTo(cue.During), $"{id} L{linea}: lo que hace «{prop.Actor.name}»");
                    if (!cue.Moves)
                    {
                        // Quien camina ya dio su primer paso en este mismo cuadro; quien no, está donde llegó.
                        Assert.That(Vector2.Distance(rect.anchorMin, cue.From), Is.LessThan(0.0001f),
                            $"{id} L{linea}: «{prop.Actor.name}» está donde llegó");
                    }
                }

                // Quien camina termina su camino aunque el texto avance; para comprobar la línea
                // siguiente desde un estado conocido, se espera a que todos lleguen.
                var inicio = Time.realtimeSinceStartup;
                while (controller.Actors.Any(actor => actor.Walking) && Time.realtimeSinceStartup - inicio < 15f)
                {
                    await Awaitable.NextFrameAsync();
                }

                foreach (var (prop, rect, rig, walking) in controller.Actors)
                {
                    var cue = ActorTimeline.Cue(prop, linea, rig.Speaks(hablante));
                    Assert.That(walking, Is.False, $"{id} L{linea}: «{prop.Actor.name}» llegó");
                    Assert.That(Vector2.Distance(rect.anchorMin, cue.To), Is.LessThan(0.0001f), $"{id} L{linea}: «{prop.Actor.name}» llegó a su sitio");
                    Assert.That(rig.Current, Is.EqualTo(cue.Moves ? cue.After : cue.During), $"{id} L{linea}: y hace lo de la llegada");
                }
            }
        }

        /// <summary>
        /// Una captura por línea de cada escena, con la cámara asentada y los personajes ya en su
        /// sitio, para revisar que se ve lo que el guion cuenta (en persistentDataPath/TestScreenshots).
        /// </summary>
        [TestCase("N1_Apertura", LevelId.Fire)]
        [TestCase("N1_AparicionGuia", LevelId.Fire)]
        [TestCase("N1_Hallazgo", LevelId.Fire)]
        [TestCase("N1_NacimientoDelFuego", LevelId.Fire)]
        [TestCase("N2_PuenteI", LevelId.Wheel)]
        [TestCase("N2_PuenteI_Bosque", LevelId.Wheel)]
        [TestCase("N2_Escena21_Bosque", LevelId.Wheel)]
        [TestCase("N2_Escena22_ElPatron", LevelId.Wheel)]
        [TestCase("N2_Escena23_Construccion", LevelId.Wheel)]
        [TestCase("N2_Escena24_Regreso", LevelId.Wheel)]
        [TestCase("N2_Escena25_Cierre", LevelId.Wheel)]
        [TestCase("N3_PuenteII", LevelId.River)]
        [TestCase("N3_PuenteII_Horizonte", LevelId.River)]
        [TestCase("N3_PuenteII_Rio", LevelId.River)]
        [TestCase("N3_Escena31_Llegada", LevelId.River)]
        [TestCase("N3_Escena32_PrimerIntento", LevelId.River)]
        [TestCase("N3_Escena33_Cruce", LevelId.River)]
        [TestCase("N3_EscenaFinal", LevelId.River)]
        [Timeout(240000)]
        [Category("VisualVerification")]
        [Description("Verificar en las capturas Personajes_*: cada personaje se ve entero, con los pies en el suelo " +
                     "pintado y por encima del cuadro de diálogo; hace lo que su línea cuenta; la escala entre adultos, " +
                     "niños y Algoritm es coherente; y nadie tapa lo que el texto nombra.")]
        public async Task NarrativeScene_RF05_CapturaCadaLineaConLosPersonajes(string id, LevelId nivel)
        {
            var (controller, _) = await OpenNarrative(id, nivel);
            var secuencia = SequenceNamed(controller, id);
            for (var linea = 0; linea < secuencia.Lines.Length; linea++)
            {
                if (linea > 0)
                {
                    Click(controller.AdvanceButton);
                }

                // Sin exigir que la cámara se deslice: la apertura del Nivel 1 cierra con un corte seco.
                var inicio = Time.realtimeSinceStartup;
                while (Time.realtimeSinceStartup - inicio < 15f
                       && (controller.Actors.Any(actor => actor.Walking)
                           || Vector2.Distance(controller.CameraCurrent.Focus, controller.CameraTarget.Focus) > 0.0015f
                           || Mathf.Abs(controller.CameraCurrent.Zoom - controller.CameraTarget.Zoom) > 0.004f))
                {
                    await Awaitable.NextFrameAsync();
                }

                await EsperarSegundos(0.4f); // medio ciclo del gesto: que la pose se lea
                Capturar($"Personajes_{id}_L{linea:00}");
            }
        }

        /// <summary>
        /// Quien va de camino termina su camino aunque el texto avance —la familia cruza con la
        /// balsa, que sigue deslizándose sola— y al llegar hace lo que mantiene en la línea que se
        /// esté leyendo.
        /// </summary>
        [Test]
        [Timeout(60000)]
        public async Task NarrativeScene_RF05_QuienCaminaTerminaSuCaminoAunqueElTextoAvance()
        {
            var (controller, _) = await OpenNarrative("N3_Escena33_Cruce", LevelId.River);
            var secuencia = SequenceNamed(controller, "N3_Escena33_Cruce");
            var viajeros = controller.Actors.Where(actor => actor.Walking && ActorTimeline.BeatAt(actor.Prop, 1) == null).ToArray();
            Assume.That(viajeros, Is.Not.Empty, "alguien cruza en la línea 0 sin paso nuevo en la 1");

            Click(controller.AdvanceButton);
            await Awaitable.NextFrameAsync();
            foreach (var (prop, rect, _, walking) in viajeros)
            {
                Assert.That(walking, Is.True, $"«{prop.Actor.name}» sigue cruzando al avanzar el texto");
                Assert.That(Vector2.Distance(rect.anchorMin, ActorTimeline.PositionAfter(prop, 0)), Is.GreaterThan(0.0001f),
                    "y no saltó al final");
            }

            var paso = viajeros[0].Prop.Beats.First(beat => beat.Line == 0);
            await EsperarSegundos(paso.Seconds + 0.3f);
            var (llegado, rectLlegado, rig, sigue) = controller.Actors.First(actor => actor.Prop == viajeros[0].Prop);
            var ahora = ActorTimeline.Cue(llegado, 1, rig.Speaks(secuencia.Lines[1].Speaker));
            Assert.That(sigue, Is.False, "llegó");
            Assert.That(Vector2.Distance(rectLlegado.anchorMin, paso.Destination), Is.LessThan(0.0001f), "a su sitio");
            Assert.That(rig.Current, Is.EqualTo(ahora.During), "y hace lo de la línea que se está leyendo");
        }

        /// <summary>Guarda una captura si hay Game View. En batchmode no la hay y se sigue sin ella.</summary>
        private static void Capturar(string nombre)
        {
            if (Application.isBatchMode)
            {
                return;
            }

            var carpeta = $"{Application.persistentDataPath}/TestScreenshots";
            System.IO.Directory.CreateDirectory(carpeta);
            var ruta = $"{carpeta}/{nombre}.png";
            System.IO.File.Delete(ruta);

            var textura = ScreenCapture.CaptureScreenshotAsTexture();
            System.IO.File.WriteAllBytes(ruta, textura.EncodeToPNG());
            Object.Destroy(textura);
        }

        // --- helpers -----------------------------------------------------------------------

        private static async Task EsperarSegundos(float segundos)
        {
            var inicio = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - inicio < segundos)
            {
                await Awaitable.NextFrameAsync();
            }
        }

        private static Rect EnPantalla(RectTransform rect)
        {
            var esquinas = new Vector3[4];
            rect.GetWorldCorners(esquinas);
            var minX = Mathf.Min(esquinas[0].x, esquinas[1].x, esquinas[2].x, esquinas[3].x);
            var minY = Mathf.Min(esquinas[0].y, esquinas[1].y, esquinas[2].y, esquinas[3].y);
            var maxX = Mathf.Max(esquinas[0].x, esquinas[1].x, esquinas[2].x, esquinas[3].x);
            var maxY = Mathf.Max(esquinas[0].y, esquinas[1].y, esquinas[2].y, esquinas[3].y);
            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        private static NarrativeSequence SequenceNamed(NarrativeSceneController controller, string id) =>
            controller.Sequences.First(sequence => sequence.Id == id);

        private static async Task<(NarrativeSceneController controller, GameFlowRunner runner)>
            OpenNarrative(string sequenceId, LevelId reached = LevelId.Fire)
        {
            LimpiarObjetosPersistentes();

            var load = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            while (load is { isDone: false })
            {
                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync();

            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync(); // GameFlowRunner.Start() navega solo a MainMenu

            runner.GoTo(GameState.ProfileSelect);
            var profile = PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;
            // Una secuencia que desemboca en su fase necesita el nivel desbloqueado: entrar a
            // jugar sigue pasando por RF-03 y no lo esquiva la narrativa.
            profile.Reach(reached);
            runner.SelectProfile(profile);
            runner.StartNarrative(sequenceId);
            // Assert y no Assume: un arreglo roto tiene que fallar fuerte. Con `Assume` esta
            // misma comprobación dejó la prueba en «inconclusive» —pasando sin probar nada—
            // durante una corrida entera (07/09/2026).
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Narrative),
                "el flujo no llegó a la escena narrativa: el arreglo de la prueba está roto");

            var controller = Object.FindAnyObjectByType<NarrativeSceneController>(FindObjectsInactive.Include);
            controller.Runner = runner;
            controller.Begin();
            await Awaitable.NextFrameAsync();
            return (controller, runner);
        }

        private static void Click(Button button) =>
            ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RF05_LaEscena22EncadenaConLa23YEstaEntraAlTaller()
        {
            var (controller, runner) = await OpenNarrative("N2_Escena22_ElPatron", LevelId.Wheel);
            var lineas = SequenceNamed(controller, "N2_Escena22_ElPatron").Lines.Length;

            for (var i = 0; i < lineas; i++)
            {
                Click(controller.AdvanceButton);
            }

            // Dos escenas seguidas del guion (§6.1.3 → §6.2.1): el asset declara la siguiente y
            // el flujo sigue en Narrative con el id nuevo, sin pasar por el menú.
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Narrative), "sigue en la narrativa");
            Assert.That(runner.Flow.NarrativeSequenceId, Is.EqualTo("N2_Escena23_Construccion"));

            var (construccion, runner2) = await OpenNarrative("N2_Escena23_Construccion", LevelId.Wheel);
            lineas = SequenceNamed(construccion, "N2_Escena23_Construccion").Lines.Length;

            for (var i = 0; i < lineas; i++)
            {
                Click(construccion.AdvanceButton);
            }

            Assert.That(runner2.Flow.Current, Is.EqualTo(GameState.Playing), "la 2.3 sale a jugar");
            Assert.That(runner2.Flow.PlayingLevel, Is.EqualTo(LevelId.Wheel));
            Assert.That(runner2.Flow.PlayingPhase, Is.EqualTo(2), "la fase del taller que declara el asset");
        }
    }
}
