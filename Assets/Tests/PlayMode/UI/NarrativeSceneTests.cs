using System;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task NarrativeScene_RF05_ResuelveLasSeisSecuenciasDelNivel2SinRamas()
        {
            var ids = new[]
            {
                "N2_PuenteI", "N2_Escena21_Bosque", "N2_Escena22_ElPatron",
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
        [Timeout(30000)]
        public async Task NarrativeScene_RF05_LaPrimeraLineaEsLaDelAssetPedido()
        {
            var (controller, _) = await OpenNarrative("N1_Hallazgo");
            var esperada = SequenceNamed(controller, "N1_Hallazgo").Lines[0];

            Assert.That(controller.BodyLabel.text, Is.EqualTo(esperada.Text));
            Assert.That(controller.SpeakerLabel.text, Is.EqualTo(esperada.Speaker));
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RF05_AvanzarLlegaHastaElFinalYSaleAOtraPantalla()
        {
            var (controller, runner) = await OpenNarrative("N1_Apertura");
            var lineas = SequenceNamed(controller, "N1_Apertura").Lines.Length;

            for (var i = 0; i < lineas; i++)
            {
                Click(controller.AdvanceButton);
            }

            Assert.That(controller.Dialogue.IsFinished, Is.True, "la escena terminó");
            Assert.That(runner.Flow.Current, Is.Not.EqualTo(GameState.Narrative),
                "al terminar sale de la escena narrativa, no se queda sin salida (RNF-13)");
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
        public async Task NarrativeScene_RF05_LaSecuenciaSinFaseSiguienteVuelveAlMenuDeNiveles()
        {
            // `N2_PuenteI` no declara fase siguiente ni es cierre: su salida es el menú, y eso
            // deja el recorrido cerrado (RNF-13). Las de apertura del N1 ya entran a jugar (T14).
            var (controller, runner) = await OpenNarrative("N2_PuenteI", LevelId.Wheel);
            var lineas = SequenceNamed(controller, "N2_PuenteI").Lines.Length;

            for (var i = 0; i < lineas; i++)
            {
                Click(controller.AdvanceButton);
            }

            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.LevelSelect));
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
            await EsperarSegundos(caja.Prop.MotionSeconds + 0.2f);

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
    }
}
