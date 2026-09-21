using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Levels.River.Tests
{
    /// <summary>
    /// RNF-19, RNF-20 y RNF-21 sobre el Nivel 3 (R15). RNF-19 se verifica «inspeccionando los
    /// estados de error de los niveles 2 y 3»: <c>WheelAccessibilityTests</c> cerró la primera
    /// mitad y esta es la segunda. Sin <see cref="Game.Core.GameFlowRunner"/>: la prueba fallida
    /// se queda en la escena en vez de salir a la 3.2, que es lo que permite capturarla.
    /// </summary>
    [Category("Integration")]
    public class RiverAccessibilityTests
    {
        [TearDown]
        public void TearDown() => AssemblyPanelController.ForgetLevelMemory();

        [Test]
        [Timeout(90000)]
        [Category("Acceptance")]
        [Category("VisualVerification")]
        [Description("Verificar en las capturas: la tablilla del guía lleva icono además de color en cada rechazo; " +
                     "el espacio incorrecto de la balsa tiene el icono de alerta encima, no solo otro tinte; " +
                     "tras la prueba fallida la balsa vuelve a su sitio con los dos espacios señalados.")]
        public async Task RiverLevel_RNF19_LosEstadosDeErrorSeLeenSinColor()
        {
            // Entrar a la zona sin los materiales (CU-09 FA-6a).
            var river = await RiverMovementTests.OpenRiver();
            var primero = river.Spawned.First();
            WalkTo(river, primero.Collectible.Position);
            river.CollectButton.onClick.Invoke();
            var acierto = river.MessageIcon.sprite;
            WalkTo(river, river.Config.BuildZonePosition);
            await Awaitable.NextFrameAsync();
            Assert.That(river.Zone.IsOpen, Is.False);
            AssertSegundoIndicador("zona sin materiales", river.MessageLabel, river.MessageIcon, acierto);
            Capturar("RiverLevel_RNF19_ZonaSinMateriales");

            // Colocación incorrecta en una fase: el tronco largo en la base (guion §1.8.4).
            foreach (var (collectible, _) in river.Spawned.Where(entry => entry.Image.gameObject.activeSelf).ToArray())
            {
                WalkTo(river, collectible.Position);
                river.CollectButton.onClick.Invoke();
            }

            WalkTo(river, river.Config.BuildZonePosition);
            Assume.That(river.Zone.IsOpen, Is.True);
            var panel = river.Assembly;
            await AssemblyPanelTests.WaitIdle(panel);
            acierto = river.MessageIcon.sprite;
            var troncos = panel.Slots.Values.Where(entry => entry.Slot.Phase == RaftPhase.Base).ToArray();
            await AssemblyPanelTests.Drag(panel, MaterialKind.Mast, troncos[0].Image.rectTransform);
            foreach (var entry in troncos.Skip(1))
            {
                await AssemblyPanelTests.Drag(panel, MaterialKind.Logs, entry.Image.rectTransform);
            }

            await AssemblyPanelTests.Confirm(panel);
            AssertSegundoIndicador("colocación incorrecta", river.MessageLabel, river.MessageIcon, acierto);
            AssertEspacioSenalado("colocación incorrecta", troncos[0]);
            await EsperarDesvanecido();
            Capturar("RiverLevel_RNF19_ColocacionIncorrecta");

            // Prueba de balsa fallida: mástil y vela cambiados (RF-42).
            await AssemblyPanelTests.Drag(panel, MaterialKind.Logs, troncos[0].Image.rectTransform);
            await AssemblyPanelTests.Confirm(panel);
            await AssemblyPanelTests.FillPhase(panel, RaftPhase.Lashing);
            await AssemblyPanelTests.Confirm(panel);
            acierto = river.MessageIcon.sprite;
            var mastil = panel.Slots.Values.Single(entry => entry.Slot.Accepts == MaterialKind.Mast);
            var vela = panel.Slots.Values.Single(entry => entry.Slot.Accepts == MaterialKind.Cloth);
            await AssemblyPanelTests.Drag(panel, MaterialKind.Cloth, mastil.Image.rectTransform);
            await AssemblyPanelTests.Drag(panel, MaterialKind.Mast, vela.Image.rectTransform);
            await AssemblyPanelTests.Confirm(panel);
            Assert.That(panel.Assembly.IsComplete, Is.False, "la balsa se hundió");
            AssertSegundoIndicador("prueba de balsa fallida", river.MessageLabel, river.MessageIcon, acierto);
            AssertEspacioSenalado("prueba fallida · mástil", mastil);
            AssertEspacioSenalado("prueba fallida · vela", vela);
            // La tablilla cambia el mismo cuadro en que termina el hundimiento: capturar tras pintarla.
            await EsperarDesvanecido();
            Capturar("RiverLevel_RNF19_PruebaFallida");
        }

        [Test]
        [Timeout(60000)]
        [Category("Acceptance")]
        [Category("VisualVerification")]
        [Description("Verificar en la captura en escala de grises: las tareas hechas (1 y 2) y las pendientes (3 y 4) " +
                     "se distinguen por la forma del icono sin el color.")]
        public async Task RiverLevel_RNF19_LaListaDeTareasSeLeeEnEscalaDeGrises()
        {
            var river = await RiverMovementTests.OpenRiver();
            foreach (var (collectible, _) in river.Spawned.ToArray())
            {
                WalkTo(river, collectible.Position);
                river.CollectButton.onClick.Invoke();
            }

            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            var hechas = river.Rows.Where(row => river.Tasks.IsDone(row.Task)).ToArray();
            var pendientes = river.Rows.Where(row => !river.Tasks.IsDone(row.Task)).ToArray();
            Assert.That(hechas, Has.Length.EqualTo(2), "las dos de recolección están hechas (RF-36)");
            Assert.That(pendientes, Has.Length.EqualTo(2));
            Assert.That(hechas.Select(row => row.Icon.sprite).Distinct().Single(), Is.Not.Null, "hecha tiene icono");
            Assert.That(pendientes.Select(row => row.Icon.sprite).Distinct().Single(), Is.Not.Null, "pendiente tiene icono");
            Assert.That(hechas[0].Icon.sprite, Is.Not.SameAs(pendientes[0].Icon.sprite),
                "hecha y pendiente cambian de forma, no solo de color (RNF-19)");
            Assert.That(river.Rows.All(row => row.Icon.enabled && row.Icon.gameObject.activeInHierarchy), Is.True, "los cuatro iconos se ven");
            Assert.That(river.Rows.All(row => !row.Label.text.Any(char.IsDigit)), Is.True, "ninguna tarea trae cifras (CP-03)");

            await Awaitable.EndOfFrameAsync();
            Capturar("RiverLevel_RNF19_ListaDeTareas", grises: true);
        }

        [Test]
        [Timeout(60000)]
        [Category("Acceptance")]
        public async Task RiverLevel_RNF20_ContrasteSuficienteSobreElEscenarioClaro()
        {
            var river = await RiverMovementTests.OpenRiver();
            WalkTo(river, river.Config.BuildZonePosition); // «todavía falta»: el mensaje más largo de la orilla
            await Awaitable.NextFrameAsync();

            AssertContraste("orilla · mensaje", river.MessageLabel, river.Environment);
            AssertContraste("orilla · recoger", river.CollectButton.GetComponentInChildren<Text>(true), river.Environment);
            foreach (var (task, label, _) in river.Rows)
            {
                AssertContraste($"orilla · tarea «{task}»", label, river.Environment);
            }

            AssertContraste("ensamblaje · confirmar", river.Assembly.ConfirmLabel, river.Environment);
            AssertContrasteDeLaPausa("orilla", river.Environment);

            // El botón de confirmar se deshabilita durante cada animación y vuelve con un
            // desvanecido de 0,1 s: medido antes de que termine, su cara es el tinte de
            // deshabilitado sobre el agua (3,9:1 en la captura del 21/09/2026). Lo que ve el
            // estudiante es el botón ya pintado: se comprueba sobre la cara real del renderer.
            var (armado, panel) = await AssemblyPanelTests.OpenAssembly(); // recarga la escena con todo recogido
            await EsperarDesvanecido();
            Assert.That(panel.ConfirmButton.interactable, Is.True, "con el panel en reposo el botón está habilitado");
            Assert.That(panel.ConfirmButton.image.canvasRenderer.GetColor(), Is.EqualTo(Color.white),
                "y ya sin el tinte de deshabilitado: la cara medida es la que se ve (RNF-20)");

            // Las tareas hechas cambian de color: también ese color tiene que leerse (RF-36).
            var hechas = armado.Rows.Where(row => armado.Tasks.IsDone(row.Task)).ToArray();
            Assert.That(hechas, Is.Not.Empty, "con todo recogido hay tareas hechas");
            foreach (var (task, label, _) in hechas)
            {
                AssertContraste($"orilla · tarea hecha «{task}»", label, armado.Environment);
            }
        }

        [Test]
        [Timeout(120000)]
        [Category("Acceptance")]
        [Category("VisualVerification")]
        [Description("Verificar en las capturas: el empuje de cámara al abrir el ensamblaje, el pulso de fase confirmada y el " +
                     "hundimiento son movimientos continuos; nada se apaga y enciende ni destella. (El cruce llega con R14.)")]
        public async Task RiverLevel_RNF21_NingunaAnimacionDelNivel3TieneDestellos()
        {
            var river = await RiverMovementTests.OpenRiver();

            // Recoger no anima: el material pasa de la orilla al inventario y se queda quieto.
            var (primero, imagen) = river.Spawned.First();
            WalkTo(river, primero.Position);
            river.CollectButton.onClick.Invoke();
            for (var cuadro = 0; cuadro < 3; cuadro++)
            {
                await Awaitable.NextFrameAsync();
                Assert.That(imagen.gameObject.activeSelf, Is.False, "lo recogido no vuelve a aparecer ni un cuadro (RNF-21)");
            }

            foreach (var (collectible, _) in river.Spawned.Where(entry => entry.Image.gameObject.activeSelf).ToArray())
            {
                WalkTo(river, collectible.Position);
                river.CollectButton.onClick.Invoke();
            }

            // El empuje de cámara al abrir el ensamblaje: el mundo se escala sin que nada se apague.
            WalkTo(river, river.Config.BuildZonePosition);
            Assume.That(river.Zone.IsOpen, Is.True);
            var panel = river.Assembly;
            var mundo = (RectTransform)river.Environment.transform.parent;
            await MuestrearMientrasAnima("empuje", panel, river, () => mundo.localScale.x, 0.15f, "RiverLevel_RNF21_Empuje");
            // El mundo se escala del plano de juego al de ensamblaje: la escala final es el cociente de los dos zooms.
            Assert.That(mundo.localScale.x, Is.EqualTo(panel.Content.AssemblyFraming.Zoom / river.Config.PlayFraming.Zoom).Within(0.01f),
                "lo muestreado es el mundo que la cámara empuja hasta el encuadre del asset");

            // El pulso de fase confirmada (RF-41): la balsa crece y vuelve, continua.
            await AssemblyPanelTests.FillPhase(panel, RaftPhase.Base);
            panel.ConfirmButton.onClick.Invoke();
            await MuestrearMientrasAnima("completado", panel, river, () => panel.RaftArea.localScale.x, 0.05f, "RiverLevel_RNF21_Completado");
            Assert.That(panel.Assembly.ActivePhase, Is.EqualTo(RaftPhase.Lashing));

            // El hundimiento (guion §1.8.4): gira y baja por un costado, y vuelve.
            await AssemblyPanelTests.FillPhase(panel, RaftPhase.Lashing);
            await AssemblyPanelTests.Confirm(panel);
            var mastil = panel.Slots.Values.Single(entry => entry.Slot.Accepts == MaterialKind.Mast);
            var vela = panel.Slots.Values.Single(entry => entry.Slot.Accepts == MaterialKind.Cloth);
            await AssemblyPanelTests.Drag(panel, MaterialKind.Cloth, mastil.Image.rectTransform);
            await AssemblyPanelTests.Drag(panel, MaterialKind.Mast, vela.Image.rectTransform);
            panel.ConfirmButton.onClick.Invoke();
            var alto = panel.RaftArea.rect.height;
            await MuestrearMientrasAnima("hundimiento", panel, river, () => panel.RaftArea.anchoredPosition.y / alto, 0.06f, "RiverLevel_RNF21_Hundimiento");
            Assert.That(panel.RaftArea.localRotation, Is.EqualTo(Quaternion.identity), "la balsa vuelve derecha");
            Assert.That(panel.RaftArea.anchoredPosition, Is.EqualTo(Vector2.zero), "y a su sitio");
        }

        // --- helpers -----------------------------------------------------------------------

        /// <summary>
        /// Deja terminar el desvanecido del botón de confirmar (0,1 s de <c>Selectable</c>) antes
        /// de capturar: en el corredor los cuadros duran milisegundos y una captura inmediata
        /// muestra el tinte de deshabilitado a medio camino, que no es lo que ve el estudiante.
        /// </summary>
        private static async Task EsperarDesvanecido()
        {
            await Awaitable.WaitForSecondsAsync(0.2f);
            await Awaitable.EndOfFrameAsync();
        }

        /// <summary>
        /// Sigue una animación del panel cuadro a cuadro: nada se apaga, la ilustración y la sombra
        /// no destellan, y la magnitud observada nunca salta más de <paramref name="saltoMaximo"/>
        /// entre dos cuadros (RNF-21). Captura hacia la mitad.
        /// </summary>
        private static async Task MuestrearMientrasAnima(string nombre, AssemblyPanelController panel, RiverSceneController river,
            Func<float> magnitud, float saltoMaximo, string captura)
        {
            Assume.That(panel.IsBusy, Is.True, $"{nombre}: la animación arrancó");
            var sombra = panel.Shade.color.a;
            var anterior = magnitud();
            var salto = 0f;
            var muestras = 0;
            while (panel.IsBusy)
            {
                await Awaitable.NextFrameAsync();
                salto = Mathf.Max(salto, Mathf.Abs(magnitud() - anterior));
                anterior = magnitud();
                muestras++;
                Assert.That(river.Environment.enabled && river.Environment.color.a > 0.99f, Is.True, $"{nombre}: la ilustración no destella (RNF-21)");
                Assert.That(panel.Shade.color.a, Is.EqualTo(sombra).Within(0.001f), $"{nombre}: la sombra no parpadea");
                Assert.That(panel.RaftArea.gameObject.activeSelf, Is.True, $"{nombre}: la balsa no se apaga");
                Assert.That(panel.Slots.Values.Where(entry => panel.Assembly.IsVisible(entry.Slot)).All(entry => entry.Image.gameObject.activeSelf),
                    Is.True, $"{nombre}: ningún espacio visible parpadea");
                if (muestras == 6)
                {
                    Capturar(captura);
                }
            }

            Assert.That(muestras, Is.GreaterThan(3), $"{nombre} dura varios cuadros: es una animación, no un corte");
            Assert.That(salto, Is.LessThan(saltoMaximo), $"{nombre}: ningún cuadro salta más de lo que el ojo sigue (máximo {salto:F3})");
        }

        /// <summary>Un rechazo se lee por icono y por palabras, no solo por el color de la tablilla (RNF-19).</summary>
        private static void AssertSegundoIndicador(string donde, Text label, Image icon, Sprite acierto)
        {
            Assert.That(label.text, Is.Not.Empty, $"{donde}: la tablilla describe el rechazo en palabras");
            Assert.That(icon.enabled && icon.sprite != null, Is.True, $"{donde}: el rechazo lleva icono");
            Assert.That(icon.sprite, Is.Not.SameAs(acierto), $"{donde}: el icono del rechazo no es el del acierto");
            Assert.That(label.text.Any(char.IsDigit), Is.False, $"{donde}: sin cifras (CP-03)");
        }

        /// <summary>El espacio incorrecto lleva el icono de alerta encima, además del color (RNF-19, RF-42).</summary>
        private static void AssertEspacioSenalado(string donde, (RaftSlot Slot, Image Image, Image Alert, RaftPieceHandle Handle) entry)
        {
            Assert.That(entry.Alert, Is.Not.Null, $"{donde}: el espacio tiene icono de alerta");
            Assert.That(entry.Alert.gameObject.activeInHierarchy && entry.Alert.sprite != null, Is.True, $"{donde}: el icono está encendido");
            Assert.That(entry.Alert.color, Is.Not.EqualTo(Color.white), $"{donde}: y el color acompaña");
        }

        /// <summary>
        /// Contraste del texto contra la cara sobre la que se pinta, medido en la escena (RNF-20).
        /// La cara tiene que ser una tablilla o un botón: un texto directamente sobre la ilustración
        /// no tiene contraste medible, y el escenario claro y cenital es el caso más expuesto.
        /// </summary>
        private static void AssertContraste(string donde, Text text, Image ilustracion)
        {
            Assert.That(text, Is.Not.Null, $"{donde}: no hay texto");
            var cara = text.GetComponentInParent<Image>(true); // la pausa y el panel están inactivos hasta abrirse
            Assert.That(cara, Is.Not.Null, $"{donde}: el texto no cuelga de ninguna cara con Image");
            Assert.That(cara, Is.Not.SameAs(ilustracion), $"{donde}: el texto va directo sobre la ilustración, sin tablilla (RNF-20)");
            var contraste = ContrastRatio(text.color, cara.color);
            TestContext.WriteLine($"{donde}: {contraste:F2}:1");
            Assert.That(contraste, Is.GreaterThanOrEqualTo(4.5), $"{donde}: {contraste:F2}:1 no alcanza el 4.5:1 de RNF-20");
        }

        private static void AssertContrasteDeLaPausa(string escena, Image ilustracion)
        {
            var botones = Object.FindObjectsByType<Button>(FindObjectsInactive.Include)
                .Where(b => b.GetComponentInChildren<Text>(true) is { } t
                            && new[] { "Reanudar", "Reiniciar", "Volver al menú de niveles", "Sí, reiniciar", "Cancelar" }.Contains(t.text.Trim()))
                .ToArray();
            Assert.That(botones, Has.Length.EqualTo(5), $"{escena}: el menú de pausa está en la escena con sus cinco botones (W17)");
            foreach (var boton in botones)
            {
                AssertContraste($"{escena} · pausa · {boton.GetComponentInChildren<Text>(true).text}", boton.GetComponentInChildren<Text>(true), ilustracion);
            }
        }

        private static double ContrastRatio(Color a, Color b)
        {
            var lighter = Math.Max(RelativeLuminance(a), RelativeLuminance(b));
            var darker = Math.Min(RelativeLuminance(a), RelativeLuminance(b));
            return (lighter + 0.05) / (darker + 0.05);
        }

        private static double RelativeLuminance(Color color) =>
            0.2126 * Linear(color.r) + 0.7152 * Linear(color.g) + 0.0722 * Linear(color.b);

        private static double Linear(float channel) =>
            channel <= 0.03928 ? channel / 12.92 : Math.Pow((channel + 0.055) / 1.055, 2.4);

        private static void WalkTo(RiverSceneController river, Vector2 target)
        {
            for (var step = 0; step < 2000 && !river.Zone.IsOpen && Vector2.Distance(river.Walk.Position, target) > 0.005f; step++)
            {
                river.Tick(target - river.Walk.Position, 0.02f);
            }
        }

        /// <summary>
        /// Guarda una captura si hay Game View; en batchmode no la hay y las aserciones valen igual.
        /// Con <paramref name="grises"/> la guarda desaturada: es la inspección de RNF-19.
        /// </summary>
        private static void Capturar(string nombre, bool grises = false)
        {
            if (Application.isBatchMode)
            {
                return;
            }

            var carpeta = $"{Application.persistentDataPath}/TestScreenshots";
            Directory.CreateDirectory(carpeta);
            var ruta = $"{carpeta}/{nombre}.png";
            File.Delete(ruta);
            var textura = ScreenCapture.CaptureScreenshotAsTexture();
            if (grises)
            {
                var pixeles = textura.GetPixels();
                for (var i = 0; i < pixeles.Length; i++)
                {
                    var gris = pixeles[i].grayscale;
                    pixeles[i] = new Color(gris, gris, gris, 1f);
                }

                textura.SetPixels(pixeles);
            }

            File.WriteAllBytes(ruta, textura.EncodeToPNG());
            Object.Destroy(textura);
            TestContext.WriteLine($"Captura: {ruta}");
        }
    }
}
