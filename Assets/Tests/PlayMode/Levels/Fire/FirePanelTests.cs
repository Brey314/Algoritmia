using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Game.Core;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Levels.Fire.Tests
{
    [Category("Integration")]
    public class FirePanelTests
    {
        private const string SceneName = "Level1_Cave";

        // N1_Config (Fases 5 y 6, 21/09/2026): fuerza efectiva 7–8 de 10, mínimo de golpes
        // efectivos = 3; cercanía de diez muescas, efectiva solo la cinco (encimadas 30 px).
        private const int MinimumEffectiveStrikes = 3;
        private const int SoftForce = 2;
        private const int HardForce = 10;
        private const int EffectiveForce = 7;
        private const int FarSpacing = 0;
        private const int EffectiveSpacing = 5;
        private const int CloseSpacing = 10;

        [TearDown]
        public void DestruirLosObjetosPersistentes()
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

        // --- reunir (T25) ------------------------------------------------------------------

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FirePanel_RF14_AlAbrirSoloHayPiezasTablillaYPista()
        {
            var controller = await LoadPanel();
            var mensaje = RectNamed("Mensaje");

            Assert.That(controller.IsGathering, Is.True, "el nivel abre reuniendo los materiales");
            Assert.That(controller.IgnitionUi, Has.All.Matches<GameObject>(element => !element.activeSelf),
                "sin deslizantes ni «Golpear» ni «Soplar» hasta reunir");
            Assert.That(controller.BlowLockedBadge.activeSelf, Is.False, "ni candado: no hay botón que candar");
            Assert.That(controller.GatherRing.gameObject.activeSelf, Is.False, "el círculo no se dibuja solo: lo pide la ayuda");
            Assert.That(controller.Pieces, Has.All.Matches<DraggablePiece>(piece => piece.isActiveAndEnabled),
                "las piezas están en el suelo y se pueden arrastrar");
            Assert.That(ReachableByRaycast((RectTransform)controller.HintButton.transform), Is.True, "«Pista» alcanzable");
            Assert.That(mensaje is not null && mensaje.gameObject.activeInHierarchy && IsWithin(mensaje, RectNamed("Panel")), Is.True,
                "tablilla de mensajes visible dentro del panel");
            Assert.That(LogLines().text, Does.Not.Match(@"\d").And.Not.Empty, "la tablilla abre con la instrucción de reunir");
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FirePanel_RF13_AlReunirLaPistaDibujaElCirculoConElBordeEnLaMitadDelBotonDeAbajo()
        {
            var controller = await LoadPanel();
            var golpear = (RectTransform)controller.StrikeButton.transform;
            var suelo = (RectTransform)controller.FireSpot.parent;

            Click(controller.HintButton);
            await Awaitable.NextFrameAsync();

            var ring = controller.GatherRing;
            Assert.That(ring.gameObject.activeSelf, Is.True, "la ayuda dibuja el contorno de la zona de reunión");
            Assert.That(ring.sprite, Is.Not.Null, "con un anillo pintado");
            Assert.That(ring.rectTransform.anchoredPosition, Is.EqualTo(controller.FireSpot.anchoredPosition), "centrado en el punto del fuego");
            // El radio llega, en horizontal, hasta la mitad del botón de abajo: el borde del
            // círculo queda alineado con el centro de «Golpear» (pedido de Santiago, 15/09/2026).
            var centroGolpear = suelo.InverseTransformPoint(golpear.TransformPoint(golpear.rect.center));
            Assert.That(ring.rectTransform.rect.width / 2f, Is.EqualTo(Mathf.Abs(centroGolpear.x)).Within(0.5f),
                "el radio es la distancia horizontal del centro a la mitad del botón");
            Assert.That(LogLines().text, Is.EqualTo(controller.Log.Latest).And.Not.Match(@"\d"),
                "y la tablilla repite la instrucción de reunir, sin cifras (CP-06)");
            Assert.That(controller.IsGathering, Is.True, "pedir ayuda no reúne nada por el estudiante");
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FirePanel_RF14_ConUnaPiezaFueraDelCirculoNoPasaAlEncendido()
        {
            var controller = await LoadPanel();
            var fuera = controller.Pieces[^1];
            foreach (var piece in controller.Pieces)
            {
                piece.MoveTo(controller.FireSpot.anchoredPosition);
            }

            fuera.MoveTo(controller.FireSpot.anchoredPosition + new Vector2(controller.GatherRadius + 20f, 0f));
            controller.TryFinishGathering();
            await Awaitable.NextFrameAsync();

            Assert.That(controller.AllGathered, Is.False, "una pieza fuera del círculo y no está todo reunido");
            Assert.That(controller.IsGathering, Is.True, "sigue reuniendo");
            Assert.That(controller.IgnitionUi, Has.All.Matches<GameObject>(element => !element.activeSelf));
            Assert.That(controller.Log.Entries, Is.Empty, "y no se regaña: no pasa nada (CP-02)");
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FirePanel_RF14_ReunirTodoDentroDelCirculoAcercaLaCamaraYArmaLaFogata()
        {
            var controller = await LoadPanel();
            Assume.That(controller.Environment, Is.Not.Empty, "la escena cablea qué acerca la cámara");

            await Reunir(controller);

            Assert.That(controller.Environment, Has.All.Matches<RectTransform>(element => Mathf.Approximately(element.localScale.x, 2f)),
                "el entorno se ve al doble (N1_Config.GatherZoom)");
            Assert.That(controller.IgnitionUi, Has.All.Matches<GameObject>(element => element.activeSelf),
                "aparece la interfaz del encendido");
            Assert.That(controller.Pieces, Has.All.Matches<DraggablePiece>(piece => !piece.enabled),
                "las piezas ya no se arrastran");

            // Las hojas sueltas se funden en el montón visto desde arriba, que queda en el punto del fuego.
            var spot = controller.FireSpot.anchoredPosition;
            Assert.That(controller.Pieces.Where(piece => piece.Kind == PieceKind.Leaf),
                Has.All.Matches<DraggablePiece>(piece => !piece.gameObject.activeSelf), "las hojas sueltas ya no se ven");
            Assert.That(controller.LeafPile.gameObject.activeSelf, Is.True, "en su lugar está el montón (prop_n1_monton_hojas_cenital)");
            Assert.That(Vector2.Distance(controller.LeafPile.rectTransform.anchoredPosition, spot),
                Is.LessThan(controller.LeafPile.rectTransform.rect.width / 2f), "sobre el punto del fuego");
            Assert.That(controller.LeafPile.color.a, Is.EqualTo(1f).Within(0.001f), "y ya del todo visible");

            // Y las piedras, a la distancia de la muesca, una a cada lado del centro.
            var silex = controller.Pieces.Single(piece => piece.Kind == PieceKind.Silex).Position;
            var pedernal = controller.Pieces.Single(piece => piece.Kind == PieceKind.Pedernal).Position;
            var distancia = controller.Spacing.Distance(controller.SelectedSpacing);
            Assert.That(pedernal.x - silex.x, Is.EqualTo(distancia).Within(0.01f), "separadas lo que marca el deslizante");
            Assert.That((silex + pedernal) / 2f, Is.EqualTo(spot), "centradas en el punto del fuego");
            Assert.That(LogLines().text, Is.EqualTo(controller.Log.Latest).And.Not.Match(@"\d"),
                "la tablilla pasa a la instrucción de golpear, sin cifras (CP-06)");
        }

        // --- encender (Fases 5 y 6) --------------------------------------------------------

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF14_AlEncenderPresentaLosDosDeslizantesGolpearYSoplar()
        {
            var controller = await LoadPanel();
            await Reunir(controller);
            Canvas.ForceUpdateCanvases();

            Assert.That(ReachableByRaycast(controller.ForceSlider.GetComponent<RectTransform>()), Is.True,
                "deslizante de fuerza alcanzable");
            Assert.That(ReachableByRaycast(controller.SpacingSlider.GetComponent<RectTransform>()), Is.True,
                "deslizante de cercanía alcanzable");
            Assert.That(ReachableByRaycast(controller.StrikeButton.GetComponent<RectTransform>()), Is.True,
                "botón «Golpear» alcanzable");
            Assert.That(ReachableByRaycast(controller.BlowButton.GetComponent<RectTransform>()), Is.True,
                "botón «Soplar» alcanzable");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF15_MoverLosDeslizantesNoEjecutaNingunGolpe()
        {
            var controller = await LoadPanel();
            await Reunir(controller);
            var antes = LogLines().text;

            controller.ForceSlider.value = EffectiveForce;
            controller.ForceSlider.value = HardForce;
            controller.ForceSlider.value = SoftForce;
            controller.SpacingSlider.value = CloseSpacing;
            controller.SpacingSlider.value = EffectiveSpacing;
            await Awaitable.NextFrameAsync();

            Assert.That(LogLines().text, Is.EqualTo(antes), "la tablilla sigue con la instrucción: nada se ejecutó");
            Assert.That(controller.Attempt?.EffectiveStrikes ?? 0, Is.EqualTo(0),
                "ningún golpe efectivo contabilizado");
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FirePanel_RF15_ElDeslizanteDeCercaniaVaDeLaMitadDeSoplarALaMitadDeGolpearYMueveLasPiedras()
        {
            var controller = await LoadPanel();
            await Reunir(controller);
            var slider = controller.SpacingSlider;
            var soplar = (RectTransform)controller.BlowButton.transform;
            var golpear = (RectTransform)controller.StrikeButton.transform;
            var silex = controller.Pieces.Single(piece => piece.Kind == PieceKind.Silex);
            var pedernal = controller.Pieces.Single(piece => piece.Kind == PieceKind.Pedernal);
            var hoja = controller.Pieces.First(piece => piece.Kind == PieceKind.Leaf);

            Assert.That(slider.wholeNumbers, Is.True, "muescas enteras");
            Assert.That(slider.minValue, Is.EqualTo(0f));
            Assert.That(slider.maxValue, Is.EqualTo(10f), "diez muescas (N1_Config.SpacingLevels)");
            Assert.That(slider.direction, Is.EqualTo(Slider.Direction.LeftToRight), "paralelo a los botones: a la izquierda lejos, a la derecha cerca");

            slider.value = FarSpacing;
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            Assert.That(ScreenX(slider.handleRect), Is.EqualTo(ScreenX(soplar)).Within(2f), "la muesca cero queda sobre la mitad de «Soplar»");
            Assert.That(pedernal.Position.x - silex.Position.x, Is.EqualTo(hoja.Width).Within(0.01f),
                "y separa las piedras la longitud de una hoja");

            slider.value = CloseSpacing;
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            Assert.That(ScreenX(slider.handleRect), Is.EqualTo(ScreenX(golpear)).Within(2f), "la muesca diez queda sobre la mitad de «Golpear»");
            Assert.That(silex.Position, Is.EqualTo(pedernal.Position), "y deja una piedra encima de la otra");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF16_GolpearEscribeEnElRegistroElMensajeDeLaFuerza()
        {
            var controller = await LoadPanel();
            await Reunir(controller);
            controller.ForceSlider.value = SoftForce;
            controller.SpacingSlider.value = EffectiveSpacing;
            var instruccion = LogLines().text;
            var entradasAntes = controller.Log.Entries.Count;

            Click(controller.StrikeButton);
            await Awaitable.NextFrameAsync();
            var trasPrimerGolpe = LogLines().text;

            Click(controller.StrikeButton);
            await Awaitable.NextFrameAsync();
            var trasSegundoGolpe = LogLines().text;

            Assert.That(trasPrimerGolpe, Is.Not.EqualTo(instruccion).And.Not.Empty, "el primer golpe escribe el mensaje de la fuerza");
            Assert.That(controller.Log.Entries, Has.Count.EqualTo(entradasAntes + 2), "el segundo golpe acumula una segunda entrada");
            Assert.That(trasSegundoGolpe, Is.EqualTo(controller.Log.Latest), "y la tablilla muestra la última");
        }

        [TestCase(FarSpacing)]
        [TestCase(CloseSpacing)]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FirePanel_RF16_ConLasPiedrasSeparadasOEncimadasElGolpeNoPrendeNiPenaliza(int muesca)
        {
            var controller = await LoadPanel();
            await Reunir(controller);
            controller.ForceSlider.value = EffectiveForce;
            controller.SpacingSlider.value = muesca;
            var entradasAntes = controller.Log.Entries.Count;

            ClickTimes(controller.StrikeButton, MinimumEffectiveStrikes);
            await Awaitable.NextFrameAsync();

            Assert.That(controller.Attempt.EffectiveStrikes, Is.Zero, "si las piedras no chocan ninguna fuerza prende");
            Assert.That(controller.BlowButton.interactable, Is.False, "y «Soplar» sigue bloqueado");
            Assert.That(controller.Log.Entries.Count, Is.GreaterThan(entradasAntes), "la tablilla cuenta lo que pasó");
            Assert.That(LogLines().text, Is.EqualTo(controller.Log.Latest).And.Not.Match(@"\d"), "sin cifras (RF-17)");
            Assert.That(controller.StrikeButton.interactable, Is.True, "y se puede seguir golpeando (CP-02)");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF19_SoplarSeHabilitaAlAlcanzarElMinimoDeGolpesEfectivos()
        {
            var controller = await LoadPanel();
            await Reunir(controller);
            controller.ForceSlider.value = EffectiveForce;
            controller.SpacingSlider.value = EffectiveSpacing;

            Assert.That(controller.BlowButton.interactable, Is.False, "«Soplar» deshabilitado antes de converger");
            Assert.That(controller.BlowLockedBadge.activeInHierarchy, Is.True, "candado visible antes");

            ClickTimes(controller.StrikeButton, MinimumEffectiveStrikes);
            await Awaitable.NextFrameAsync();

            Assert.That(controller.BlowButton.interactable, Is.True,
                "«Soplar» habilitado tras el mínimo de golpes efectivos");
            Assert.That(controller.BlowLockedBadge.activeInHierarchy, Is.False,
                "candado oculto tras converger");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF15_ElDeslizanteDeFuerzaTieneDiezMuescasYElAsaCreceYEnrojeceConLaFuerza()
        {
            var controller = await LoadPanel();
            await Reunir(controller);
            var slider = controller.ForceSlider;
            var feedback = slider.GetComponent<ForceSliderFeedback>();
            Assume.That(feedback, Is.Not.Null, "el deslizante lleva ForceSliderFeedback");

            Assert.That(slider.wholeNumbers, Is.True, "muescas enteras");
            Assert.That(slider.minValue, Is.EqualTo(0f));
            Assert.That(slider.maxValue, Is.EqualTo(10f), "diez muescas (N1_Config.ForceLevels)");

            slider.value = slider.minValue;
            await Awaitable.NextFrameAsync();
            var escalaSuave = feedback.Knob.localScale.x;
            var colorSuave = feedback.KnobFace.color;

            slider.value = slider.maxValue;
            await Awaitable.NextFrameAsync();
            var escalaFuerte = feedback.Knob.localScale.x;
            var colorFuerte = feedback.KnobFace.color;

            // Doble indicador (RNF-19): tamaño y color, no solo color.
            Assert.That(escalaFuerte, Is.GreaterThan(escalaSuave), "el asa crece con la fuerza");
            Assert.That(colorFuerte.r - colorFuerte.g, Is.GreaterThan(colorSuave.r - colorSuave.g),
                "y se vuelve más roja: el rojo domina más sobre el verde");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF13_PistaADemandaEscribeLaInstruccionEnElRegistro()
        {
            var controller = await LoadPanel();
            Assume.That(controller.HintButton, Is.Not.Null, "la escena debe traer el botón «Pista» cableado");

            Click(controller.HintButton);
            await Awaitable.NextFrameAsync();

            Assert.That(controller.Log.Entries, Has.Count.EqualTo(1), "una sola entrada: la instrucción del paso activo");
            Assert.That(LogLines().text, Is.EqualTo(controller.Log.Latest), "y la tablilla la muestra");
            Assert.That(LogLines().text, Does.Not.Match(@"\d"), "la ayuda no nombra ninguna cifra (CP-06)");
            Assert.That(controller.Attempt.EffectiveStrikes, Is.EqualTo(0), "pedir ayuda no golpea");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF13_TrasTresFallosSeguidosElRegistroSumaLaPista()
        {
            var controller = await LoadPanel();
            await Reunir(controller);
            controller.ForceSlider.value = SoftForce;
            controller.SpacingSlider.value = EffectiveSpacing;
            var entradasAntes = controller.Log.Entries.Count;

            ClickTimes(controller.StrikeButton, 2);
            await Awaitable.NextFrameAsync();
            var trasDosFallos = controller.Log.Entries.Count;

            Click(controller.StrikeButton);
            await Awaitable.NextFrameAsync();
            var trasTresFallos = controller.Log.Entries;

            Assert.That(trasDosFallos, Is.EqualTo(entradasAntes + 2), "dos fallos: solo los dos mensajes de la fuerza");
            Assert.That(trasTresFallos, Has.Count.EqualTo(entradasAntes + 4), "el tercer fallo seguido añade la pista (N1_Config: 3)");
            Assert.That(controller.Log.Latest, Does.Not.Match(@"\d"), "la pista no nombra la fuerza ni la cercanía efectivas (CP-06)");
            Assert.That(LogLines().text, Is.EqualTo(controller.Log.Latest), "y la tablilla muestra la pista");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF19_SoplarAtenuadoNoRespondeNiRegistraError()
        {
            var controller = await LoadPanel();
            await Reunir(controller);
            Assume.That(controller.BlowButton.interactable, Is.False);
            var antes = LogLines().text;
            var entradasAntes = controller.Log.Entries.Count;

            Click(controller.BlowButton);
            await Awaitable.NextFrameAsync();

            Assert.That(LogLines().text, Is.EqualTo(antes), "la tablilla no cambia");
            Assert.That(controller.Log.Entries, Has.Count.EqualTo(entradasAntes));
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RNF02_ElMapaDeControlesNoTieneNingunBindingDeTeclado()
        {
            await LoadPanel();
            var module = Object.FindAnyObjectByType<InputSystemUIInputModule>(FindObjectsInactive.Include);
            Assume.That(module, Is.Not.Null);
            Assume.That(module.actionsAsset, Is.Not.Null);

            Assert.That(module.actionsAsset.bindings,
                Has.None.Matches<InputBinding>(MentionsKeyboardOrGamepad),
                "el mapa de controles no cita Keyboard ni Gamepad");
            Assert.That(Object.FindObjectsByType<PlayerInput>(FindObjectsInactive.Include), Is.Empty,
                "la escena no lleva PlayerInput");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RNF03_LaEscenaNoPresentaListaDeTareas()
        {
            await LoadPanel();

            var listaDeTareas = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include)
                .Where(t => NameSuggestsTaskList(t.name))
                .ToArray();
            var etiquetasDeInstruccion = Object.FindObjectsByType<Text>(FindObjectsInactive.Include)
                .Where(t => t.name == "InstruccionLabel" && t.isActiveAndEnabled)
                .ToArray();

            Assert.That(listaDeTareas, Is.Empty, "ningún contenedor de lista de tareas ni casillas (INC-41)");
            Assert.That(etiquetasDeInstruccion.Length, Is.EqualTo(1), "una sola etiqueta de instrucción activa");
        }

        // Aserción de layout determinista (no verificación visual): que la tablilla absorba el
        // registro. Va con [Category("Integration")] de la clase.
        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF17_LaTablillaMuestraElUltimoMensajeSinDesbordarTrasDiezIntentos()
        {
            var controller = await LoadPanel();
            await Reunir(controller);
            controller.ForceSlider.value = SoftForce;
            controller.SpacingSlider.value = EffectiveSpacing;
            var entradasAntes = controller.Log.Entries.Count;

            ClickTimes(controller.StrikeButton, 10);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            var texto = LogLines();
            // Diez mensajes de la fuerza más una pista cada tres fallos seguidos (RF-13, N1_Config).
            Assert.That(controller.Log.Entries, Has.Count.EqualTo(entradasAntes + 10 + 10 / 3), "trece entradas más registradas");
            Assert.That(texto.text, Is.EqualTo(controller.Log.Latest), "la tablilla muestra solo el último mensaje");
            Assert.That(texto.preferredHeight, Is.LessThanOrEqualTo(texto.rectTransform.rect.height + 0.5f),
                "y cabe en ella sin desbordar");
            Assert.That(IsWithin(texto.rectTransform, RectNamed("Panel")), Is.True, "la tablilla queda dentro del panel");
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FirePanel_RNF19_SoplarAtenuadoSeDistinguePorElCandadoAdemasDelColorYSinRotuloAunNo()
        {
            var controller = await LoadPanel();
            await Reunir(controller);

            Assert.That(controller.BlowLockedBadge.activeInHierarchy, Is.True, "el candado acompaña a «Soplar» atenuado");
            Assert.That(controller.BlowLockedBadge.GetComponentsInChildren<Image>(true).Any(image => image.gameObject != controller.BlowLockedBadge),
                Is.True, "y es un icono: forma además de color (RNF-19)");
            Assert.That(Object.FindObjectsByType<Text>(FindObjectsInactive.Include)
                    .Any(text => text.text.Contains("Aún no", StringComparison.OrdinalIgnoreCase)), Is.False,
                "sin el rótulo «Aún no»: el candado ya lo dice (pedido de Santiago, 15/09/2026)");
        }

        [Test]
        [Timeout(20000)]
        [Category("VisualVerification")]
        [Description("Tras ejecutar esta prueba, revisar la captura: la cámara está al doble sobre la " +
                     "fogata; se ve el montón de hojas desde arriba con las piedras en el centro; el botón " +
                     "«Soplar» atenuado muestra solo el candado, sin rótulo; el deslizante de cercanía va " +
                     "de la mitad de «Soplar» a la mitad de «Golpear»; el contraste de la tablilla y de " +
                     "los controles es suficiente (≥ 4.5:1, RNF-20).")]
        public async Task FirePanel_RNF19_ElEncendidoSeVeComoElMockupConElCandadoSinRotulo()
        {
            var controller = await LoadPanel();
            await Reunir(controller);

            CaptureScreenshot("FirePanel_RNF19_SoplarAtenuado");
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FireLevel_RF20_SoplarEncadenaAnimacionYEscenaDeCierre()
        {
            var (controller, runner) = await LoadPanelWithProfile(NewProfile());

            await ConvergeAndBlow(controller);
            var llego = await WaitUntilAsync(() => runner.Flow.Current == GameState.Narrative, 8f); // N1_Config.IgnitionSeconds + margen

            Assert.That(llego, Is.True, "el flujo no llegó a Narrative tras soplar");
            Assert.That(runner.Flow.NarrativeSequenceId, Is.EqualTo("N1_NacimientoDelFuego"));
            Assert.That(runner.PendingIndicators.StepsUsed, Is.EqualTo(MinimumEffectiveStrikes),
                "FirePanelController ya no confirma ni guarda, pero sigue calculando los " +
                "indicadores y dejándolos listos para LevelSummary (T18)");
        }

        // FireLevel_RF04_GuardaAlCompletarLaFase y FireLevel_RF03_DesbloqueaElNivel2 se trasladaron
        // a LevelSummaryTests (T18): CompleteLevel ya no confirma/desbloquea/guarda, solo calcula
        // los indicadores y los deja en Runner.PendingIndicators — ese efecto ahora ocurre en
        // LevelSummaryController, después de la escena narrativa de cierre (CP-07).

        [Test]
        [Timeout(20000)]
        [Category("VisualVerification")]
        [Category("Acceptance")]
        [Description("Tras ejecutar esta prueba, revisar la captura: la transición de color del " +
                     "nacimiento del fuego es una única transición gradual, sin parpadeo ni " +
                     "oscilación (RNF-21).")]
        public async Task FireLevel_RNF21_SinDestellosDeAltaFrecuencia()
        {
            var (controller, _) = await LoadPanelWithProfile(NewProfile());

            await ConvergeAndBlow(controller);
            for (var i = 0; i < 10; i++)
            {
                await Awaitable.NextFrameAsync();
            }

            CaptureScreenshot("FireLevel_RNF21_ConvergenciaAMitad");
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FirePanel_CT06_LasPiezasSeArrastranConClicSostenidoYNoSalenDelSuelo()
        {
            var controller = await LoadPanel();
            var hoja = Array.Find(controller.Pieces, piece => piece.Kind == PieceKind.Leaf);
            Assume.That(hoja, Is.Not.Null, "la escena trae hojas arrastrables");
            var rect = (RectTransform)hoja.transform;
            var antes = rect.anchoredPosition;

            var origen = RectTransformUtility.WorldToScreenPoint(null, rect.position);
            var destino = origen + new Vector2(120f, -80f);
            ExecuteEvents.Execute(hoja.gameObject, Pointer(origen, origen), ExecuteEvents.beginDragHandler);
            ExecuteEvents.Execute(hoja.gameObject, Pointer(destino, origen), ExecuteEvents.dragHandler);
            ExecuteEvents.Execute(hoja.gameObject, Pointer(destino, origen), ExecuteEvents.endDragHandler);
            await Awaitable.NextFrameAsync();

            Assert.That(rect.anchoredPosition, Is.Not.EqualTo(antes), "la hoja siguió al puntero");
            var suelo = (RectTransform)rect.parent;
            hoja.MoveTo(new Vector2(9999f, -9999f));
            Assert.That(Mathf.Abs(rect.anchoredPosition.x), Is.LessThanOrEqualTo(suelo.rect.width / 2f + 0.5f),
                "y nunca sale del suelo por el lado");
            Assert.That(Mathf.Abs(rect.anchoredPosition.y), Is.LessThanOrEqualTo(suelo.rect.height / 2f + 0.5f),
                "ni por abajo");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF14_LasPiezasAparecenGiradasAlAzarYCadaHojaEsUnMonton()
        {
            var controller = await LoadPanel();

            var giros = controller.Pieces
                .Select(piece => ((RectTransform)piece.transform).localEulerAngles.z)
                .Distinct()
                .Count();
            Assert.That(giros, Is.GreaterThan(1), "no todas las piezas caen en la misma postura");
            var hojas = controller.Pieces.Where(piece => piece.Kind == PieceKind.Leaf).ToArray();
            Assert.That(hojas, Has.All.Matches<DraggablePiece>(hoja => hoja.GetComponent<LeafPile>() != null),
                "cada hoja del suelo es un montón");
            Assert.That(hojas, Has.All.Matches<DraggablePiece>(hoja =>
                    hoja.GetComponentsInChildren<Image>().Length == hoja.GetComponent<LeafPile>().Leaves),
                "con tantas hojas dibujadas como diga el montón");
        }

        [Test]
        [Timeout(30000)]
        public async Task FirePanel_RF14_LasPiezasAparecenAlAzarFueraDeLaInterfazYDelCirculo()
        {
            var controller = await LoadPanel();
            Assume.That(controller.KeepClear, Is.Not.Empty, "la escena cablea qué interfaz respetar");
            var suelo = (RectTransform)controller.FireSpot.parent;
            var interfaz = controller.KeepClear.Select(ui => EnElSuelo(ui, suelo)).ToArray();
            var spot = controller.FireSpot.anchoredPosition;

            foreach (var piece in controller.Pieces)
            {
                var rect = new Rect(piece.Position - Vector2.one * piece.Width / 2f, Vector2.one * piece.Width);
                Assert.That(interfaz, Has.None.Matches<Rect>(zona => zona.Overlaps(rect)), $"{piece.name} no queda bajo la interfaz");
                Assert.That(Vector2.Distance(piece.Position, spot), Is.GreaterThan(controller.GatherRadius),
                    $"{piece.name} empieza fuera del círculo de reunión");
            }

            var primera = controller.Pieces.Select(piece => piece.Position).ToArray();
            controller = await LoadPanel();
            Assert.That(controller.Pieces.Select(piece => piece.Position), Is.Not.EqualTo(primera), "y cada partida reparte distinto");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RNF02_TomarUnaPiezaLaLevantaYSoltarlaLaPosa()
        {
            var controller = await LoadPanel();
            var hoja = Array.Find(controller.Pieces, piece => piece.Kind == PieceKind.Leaf);
            var rect = (RectTransform)hoja.transform;
            var origen = RectTransformUtility.WorldToScreenPoint(null, rect.position);

            ExecuteEvents.Execute(hoja.gameObject, Pointer(origen, origen), ExecuteEvents.beginDragHandler);
            var levantada = await WaitUntilAsync(() => Mathf.Approximately(rect.localScale.x, hoja.LiftScale), 2f);
            Assert.That(levantada, Is.True, "al tomarla crece hasta LiftScale");

            ExecuteEvents.Execute(hoja.gameObject, Pointer(origen, origen), ExecuteEvents.endDragHandler);
            var posada = await WaitUntilAsync(() => Mathf.Approximately(rect.localScale.x, 1f), 2f);
            Assert.That(posada, Is.True, "al soltarla vuelve a su tamaño");
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FireLevel_RF20_AlSoplarPrendeLaLlamaCenitalSobreElMonton()
        {
            var (controller, _) = await LoadPanelWithProfile(NewProfile());

            await ConvergeAndBlow(controller);
            await Awaitable.NextFrameAsync();

            var llama = controller.FireFlame;
            Assert.That(llama.gameObject.activeSelf, Is.True, "la llama aparece al soplar");
            Assert.That(llama.runtimeAnimatorController.name, Is.EqualTo("prop_n1_fuego_cenital"),
                "y es el clip visto desde arriba");
            Assert.That(llama.transform.GetSiblingIndex(), Is.EqualTo(llama.transform.parent.childCount - 1),
                "encima del montón y de las piedras");
            Assert.That(controller.LeafPile.gameObject.activeSelf, Is.True, "sobre el montón, que sigue ahí");
            Assert.That(controller.Burn.Extent, Is.GreaterThan(0f), "y las hojas empiezan a quemarse desde el centro");
        }

        // --- personajes (Dirección de arte §13.3) y la pista con Algoritm (§7.6) ---------------

        [Test]
        [Timeout(20000)]
        public async Task FireLevel_DA133_LaFamiliaEsperaAtrasQuietaYElNinoObserva()
        {
            var controller = await LoadPanel();
            var suelo = controller.FireSpot.parent;
            var papa = Papa(controller);

            Assert.That(controller.Family, Has.Length.EqualTo(3), "Mamá, la Niña y el Niño");
            Assert.That(controller.Family, Has.All.Not.Null);
            Assert.That(controller.Family, Does.Contain(controller.Restless), "el inquieto es de la familia");
            Assert.That(controller.Family.Where(member => member != controller.Restless),
                Has.All.Matches<CharacterRig>(member => member.Current == ActorAction.Idle), "quietos, bien atrás (guion §1.4.2)");
            Assert.That(controller.Restless.Current, Is.EqualTo(ActorAction.Observe), "salvo el Niño, que observa (§7.4)");
            Assert.That(controller.Family.Append(papa), Has.None.Matches<CharacterRig>(rig => rig.transform.IsChildOf(suelo)),
                "ninguno dentro del suelo, que es de las piezas");
        }

        [Test]
        [Timeout(20000)]
        public async Task FireLevel_DA133_NingunPersonajeQuedaBajoLaInterfazNiLeQuitaElClic()
        {
            var controller = await LoadPanel();
            var suelo = (RectTransform)controller.FireSpot.parent;
            var personajes = controller.Family.Append(Papa(controller)).ToArray();
            var interfaz = new[] { RectNamed("Mensaje"), (RectTransform)controller.HintButton.transform, RectNamed("BotonPausa") };
            Canvas.ForceUpdateCanvases();

            Assert.That(personajes.SelectMany(rig => rig.GetComponentsInChildren<Graphic>(true)),
                Has.None.Matches<Graphic>(graphic => graphic.raycastTarget), "ningún personaje le quita el clic a una pieza o a un botón");
            Assert.That(controller.KeepClear, Is.SupersetOf(personajes.Select(rig => (RectTransform)rig.transform)),
                "las piezas no aparecen encima de ellos");
            AssertNingunoSeSolapa(personajes, interfaz, suelo, "al reunir");

            await Reunir(controller);
            Canvas.ForceUpdateCanvases();

            var encendido = interfaz
                .Concat(controller.IgnitionUi.Select(ui => (RectTransform)ui.transform))
                .Append(controller.LeafPile.rectTransform);
            AssertNingunoSeSolapa(personajes, encendido, suelo, "al encender");
        }

        [Test]
        [Timeout(20000)]
        public async Task FireLevel_DA133_PapaRecogeAlTomarUnaPiezaYVuelveAlReposo()
        {
            var controller = await LoadPanel();
            var papa = Papa(controller);
            var hoja = Array.Find(controller.Pieces, piece => piece.Kind == PieceKind.Leaf);
            var origen = RectTransformUtility.WorldToScreenPoint(null, hoja.transform.position);

            ExecuteEvents.Execute(hoja.gameObject, Pointer(origen, origen), ExecuteEvents.beginDragHandler);
            ExecuteEvents.Execute(hoja.gameObject, Pointer(origen, origen), ExecuteEvents.endDragHandler);
            var gestos = await GestosHastaElReposo(papa, 4f);

            Assert.That(gestos, Is.EqualTo(new[] { ActorAction.PickUp, ActorAction.Idle }),
                "al tomar una pieza Papá la recoge y después vuelve al reposo");
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FireLevel_DA133_PapaGolpeaAlPulsarGolpearYVuelveAlReposo()
        {
            var controller = await LoadPanel();
            var papa = Papa(controller);
            await Reunir(controller);
            controller.ForceSlider.value = EffectiveForce;
            controller.SpacingSlider.value = EffectiveSpacing;

            Click(controller.StrikeButton);
            var gestos = await GestosHastaElReposo(papa, 3f);

            Assume.That(controller.Attempt.EffectiveStrikes, Is.EqualTo(1), "el golpe saltó chispa");
            Assert.That(gestos, Is.EqualTo(new[] { ActorAction.Strike, ActorAction.Idle }),
                "Papá golpea las piedras y, con chispa, vuelve al reposo");
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FireLevel_CP02_TrasUnGolpeSinChispaPapaSeAnimaYNuncaHaceUnGestoDeDerrota()
        {
            var controller = await LoadPanel();
            var papa = Papa(controller);
            await Reunir(controller);
            controller.ForceSlider.value = SoftForce;
            controller.SpacingSlider.value = EffectiveSpacing;

            Click(controller.StrikeButton);
            var gestos = await GestosHastaElReposo(papa, 4f);

            Assume.That(controller.Attempt.EffectiveStrikes, Is.Zero, "el golpe no saltó chispa");
            Assert.That(gestos, Is.EqualTo(new[] { ActorAction.Strike, ActorAction.Encourage, ActorAction.Idle }),
                "golpea, se anima —puño arriba— y vuelve al reposo: ningún otro gesto (§7.3)");
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FireLevel_DA133_PapaSoplaYSeQuedaArrodilladoMirandoElFuego()
        {
            var controller = await LoadPanel();
            var papa = Papa(controller);

            await ConvergeAndBlow(controller);
            var alSoplar = papa.Current;
            var arrodillado = await WaitUntilAsync(() => papa.Current == ActorAction.Kneel, 3f);

            Assert.That(alSoplar, Is.EqualTo(ActorAction.Blow), "Papá sopla sobre el montón");
            Assert.That(arrodillado, Is.True, "y se queda arrodillado mientras nace el fuego (guion §1.4.4)");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_DA76_LaPistaMuestraAAlgoritmConSuFormaDeFuego()
        {
            var controller = await LoadPanel();
            var boton = (RectTransform)controller.HintButton.transform;

            var algoritm = boton.GetComponentsInChildren<Image>(true)
                .SingleOrDefault(image => image.sprite != null && image.sprite.name == "char_algoritm_n1_fuego_reposo");

            Assert.That(algoritm, Is.Not.Null, "dentro del círculo va el sprite de Algoritm del Nivel 1");
            Assert.That(algoritm.transform, Is.Not.SameAs(boton), "como imagen hija: el marco sigue siendo el círculo");
            Assert.That(algoritm.preserveAspect, Is.True, "sin deformarse");
            Assert.That(algoritm.raycastTarget, Is.False, "sin quitarle el clic al botón");
            Assert.That(IsWithin(algoritm.rectTransform, boton), Is.True, "y sin salirse del círculo");
            Assert.That(ReachableByRaycast(boton), Is.True, "que sigue alcanzable");
        }

        // --- helpers -----------------------------------------------------------------------

        private static CharacterRig Papa(FirePanelController controller)
        {
            Assert.That(controller.Player, Is.Not.Null, "la escena cablea a Papá en el panel");
            return controller.Player;
        }

        /// <summary>
        /// Lo que hace el personaje, sin repetir, cuadro a cuadro hasta volver al reposo o hasta que
        /// venza el tiempo real: la secuencia entera y no solo el final, que es lo que prueba que no
        /// hubo ningún otro gesto entre medias.
        /// </summary>
        private static async Task<List<ActorAction>> GestosHastaElReposo(CharacterRig rig, float timeoutSeconds)
        {
            var gestos = new List<ActorAction> { rig.Current };
            var deadline = Time.realtimeSinceStartup + timeoutSeconds;
            while (rig.Current != ActorAction.Idle && Time.realtimeSinceStartup < deadline)
            {
                await Awaitable.NextFrameAsync();
                if (gestos[^1] != rig.Current)
                {
                    gestos.Add(rig.Current);
                }
            }

            return gestos;
        }

        private static void AssertNingunoSeSolapa(
            IEnumerable<CharacterRig> personajes, IEnumerable<RectTransform> interfaz, RectTransform suelo, string momento)
        {
            var zonas = interfaz.ToArray();
            foreach (var rig in personajes)
            {
                var casilla = EnElSuelo((RectTransform)rig.transform, suelo);
                foreach (var ui in zonas)
                {
                    Assert.That(casilla.Overlaps(EnElSuelo(ui, suelo)), Is.False, $"{momento}: {rig.name} queda bajo {ui.name}");
                }
            }
        }

        /// <summary>Reúne todas las piezas en el punto del fuego y espera el acercamiento (T25).</summary>
        internal static async Task Reunir(FirePanelController controller)
        {
            foreach (var piece in controller.Pieces)
            {
                piece.MoveTo(controller.FireSpot.anchoredPosition);
            }

            controller.TryFinishGathering();
            while (controller.IsTransitioning)
            {
                await Awaitable.NextFrameAsync();
            }

            Assume.That(controller.IsGathering, Is.False, "el panel pasó al encendido");
            await Awaitable.NextFrameAsync(); // deja correr Awake/Start de la interfaz recién activada
        }

        private static Rect EnElSuelo(RectTransform ui, RectTransform suelo)
        {
            var esquinas = new Vector3[4];
            ui.GetWorldCorners(esquinas);
            var min = suelo.InverseTransformPoint(esquinas[0]);
            var max = suelo.InverseTransformPoint(esquinas[2]);
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        private static float ScreenX(RectTransform rect) =>
            RectTransformUtility.WorldToScreenPoint(null, rect.TransformPoint(rect.rect.center)).x;

        private static PointerEventData Pointer(Vector2 position, Vector2 pressPosition) =>
            new PointerEventData(EventSystem.current) { position = position, pressPosition = pressPosition };

        internal static async Task<FirePanelController> LoadPanel()
        {
            var load = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            while (load is { isDone: false })
            {
                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync(); // deja correr FirePanelController.Start()
            // Los objetos de la escena anterior se destruyen al final de fotograma, no en el
            // mismo en que `isDone` se vuelve verdadero: sin este fotograma extra,
            // `FindAnyObjectByType` puede devolver el controlador de la prueba anterior en vez
            // del recién cargado, y una prueba se queda comparando contra el registro de otra
            // (medido el 11/09/2026: mensajes de `FirePanel_RF16_…` colándose en otras pruebas).
            await Awaitable.NextFrameAsync();

            // Assert y no un simple FindAnyObjectByType: si alguna vez vuelve a haber dos
            // controladores vivos a la vez, esto tiene que fallar fuerte y decir por qué, no
            // devolver uno cualquiera en silencio.
            var controllers = Object.FindObjectsByType<FirePanelController>(FindObjectsInactive.Include);
            Assert.That(controllers.Length, Is.EqualTo(1),
                $"se esperaba un único FirePanelController vivo, había {controllers.Length}");
            return controllers[0];
        }

        private static PlayerProfile NewProfile() =>
            PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;

        /// <summary>
        /// Variante de <see cref="LoadPanel"/> con un <see cref="GameFlowRunner"/> real y un perfil
        /// activo (T15): las pruebas de convergencia necesitan `Flow` para observar a dónde
        /// salta «Soplar».
        /// </summary>
        private static async Task<(FirePanelController controller, GameFlowRunner runner)>
            LoadPanelWithProfile(PlayerProfile profile)
        {
            var controller = await LoadPanel();

            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync(); // GameFlowRunner.Start() navega solo a MainMenu

            runner.GoTo(GameState.ProfileSelect);
            runner.SelectProfile(profile);
            runner.StartPlaying(LevelId.Fire, 1);
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Playing),
                "el flujo no llegó a Playing: el arreglo de la prueba está roto");

            controller.Runner = runner;
            return (controller, runner);
        }

        /// <summary>Reúne, marca fuerza y cercanía efectivas, converge con el mínimo de golpes y pulsa «Soplar».</summary>
        private static async Task ConvergeAndBlow(FirePanelController controller)
        {
            await Reunir(controller);
            controller.ForceSlider.value = EffectiveForce;
            controller.SpacingSlider.value = EffectiveSpacing;
            ClickTimes(controller.StrikeButton, MinimumEffectiveStrikes);
            Click(controller.BlowButton);
        }

        /// <summary>
        /// Sondea <paramref name="condition"/> cuadro a cuadro hasta que se cumpla o venza el
        /// tiempo real: la animación de convergencia (~0.6 s) corre en tiempo real, así que un
        /// ajuste futuro de su duración no puede romper estas pruebas (Testability, plan T15).
        /// </summary>
        private static async Task<bool> WaitUntilAsync(Func<bool> condition, float timeoutSeconds)
        {
            var deadline = Time.realtimeSinceStartup + timeoutSeconds;
            while (!condition())
            {
                if (Time.realtimeSinceStartup >= deadline)
                {
                    return false;
                }

                await Awaitable.NextFrameAsync();
            }

            return true;
        }

        internal static void Click(Button button) =>
            ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);

        internal static void ClickTimes(Button button, int times)
        {
            for (var i = 0; i < times; i++)
            {
                Click(button);
            }
        }

        private static Text LogLines() => Array.Find(
            Object.FindObjectsByType<Text>(FindObjectsInactive.Include), t => t.name == "InstruccionLabel");

        private static RectTransform RectNamed(string name) => Array.Find(
            Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Include), rt => rt.name == name);

        private static bool ReachableByRaycast(RectTransform target)
        {
            var raycaster = Object.FindAnyObjectByType<GraphicRaycaster>();
            var pointer = new PointerEventData(EventSystem.current)
            {
                position = RectTransformUtility.WorldToScreenPoint(null, target.TransformPoint(target.rect.center))
            };
            var results = new List<RaycastResult>();
            raycaster.Raycast(pointer, results);
            return results.Exists(r => r.gameObject.transform == target
                                      || r.gameObject.transform.IsChildOf(target));
        }

        private static bool MentionsKeyboardOrGamepad(InputBinding binding)
        {
            var paths = (binding.path ?? string.Empty) + " " + (binding.effectivePath ?? string.Empty);
            return paths.Contains("Keyboard", StringComparison.OrdinalIgnoreCase)
                   || paths.Contains("Gamepad", StringComparison.OrdinalIgnoreCase);
        }

        private static bool NameSuggestsTaskList(string name)
        {
            var words = new[] { "Tarea", "Task", "Checklist", "Lista" };
            return words.Any(word => name.Contains(word, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsWithin(RectTransform inner, RectTransform outer)
        {
            var innerCorners = new Vector3[4];
            var outerCorners = new Vector3[4];
            inner.GetWorldCorners(innerCorners);
            outer.GetWorldCorners(outerCorners);
            var bounds = Rect.MinMaxRect(outerCorners[0].x, outerCorners[0].y,
                outerCorners[2].x, outerCorners[2].y);
            return bounds.Contains(innerCorners[0]) && bounds.Contains(innerCorners[2]);
        }

        /// <remarks>
        /// Copia del patrón de <c>LevelSelectTests</c>: una prueba de verificación visual sin este
        /// aserto «pasa» sin escribir nada. En batchmode se ignora porque <c>ScreenCapture</c>
        /// necesita la render texture de la Game View.
        /// </remarks>
        private static void CaptureScreenshot(string name)
        {
            if (Application.isBatchMode)
            {
                Assert.Ignore("La captura exige una Game View: correr desde el Editor.");
            }

            var directory = $"{Application.persistentDataPath}/TestScreenshots";
            Directory.CreateDirectory(directory);
            var path = $"{directory}/{name}.png";

            File.Delete(path);

            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.Destroy(texture);

            Assert.That(File.Exists(path), Is.True, $"no se escribió la captura en «{path}»");
            TestContext.WriteLine($"Captura: {path}");
        }
    }
}
