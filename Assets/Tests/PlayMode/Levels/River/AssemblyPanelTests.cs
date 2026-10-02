using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Levels.River.Tests
{
    /// <summary>
    /// El panel de ensamblaje cableado en la escena (RF-40..RF-43, RNF-19, RNF-02, HU-12): qué
    /// espacios se ven, cómo se resalta el incorrecto y que la balsa crece fase a fase. Los
    /// arrastres entran por <see cref="AssemblyPanelController.Take"/> /
    /// <see cref="AssemblyPanelController.Release"/>, que es lo que el asa hace con el clic
    /// sostenido; que el EventSystem entregue el clic lo verifica <c>RiverMovementTests</c>.
    /// </summary>
    [Category("Integration")]
    public class AssemblyPanelTests
    {
        [TearDown]
        public void TearDown() => AssemblyPanelController.ForgetLevelMemory();

        [Test]
        [Timeout(60000)]
        public async Task AssemblyPanel_RF40_MuestraSoloLosEspaciosDeLaFaseActiva()
        {
            var (river, panel) = await OpenAssembly();
            var content = panel.Content;

            Assert.That(river.Assembly.gameObject.activeSelf, Is.True);
            Assert.That(Visibles(panel).Select(id => content.Slots.First(slot => slot.Id == id).Phase).Distinct(),
                Is.EqualTo(new[] { RaftPhase.Base }), "abre con los espacios de la base y nada más (RF-40)");
            Assert.That(Visibles(panel).Count, Is.EqualTo(5), "los cinco troncos");
            Assert.That(panel.ConfirmLabel.text, Is.EqualTo(content.ConfirmLabel));
            Assert.That(panel.Shade.gameObject.activeInHierarchy, Is.True, "la ilustración queda en sombra");
            Assert.That(panel.Shade.color.a, Is.EqualTo(0.3f).Within(0.01f), "negro al 30 % (decisión del 20/09/2026)");

            await FillPhase(panel, RaftPhase.Base);
            await Confirm(panel);

            Assert.That(panel.Assembly.ActivePhase, Is.EqualTo(RaftPhase.Lashing));
            Assert.That(Visibles(panel).Count, Is.EqualTo(15), "la base consolidada y los diez amarres (HU-12)");
            Assert.That(Visibles(panel).Any(id => content.Slots.First(slot => slot.Id == id).Phase == RaftPhase.MastAndSail),
                Is.False, "el mástil y la vela siguen sin verse");
            Assert.That(river.Tasks.IsDone(RiverTaskId.AssembleRaft), Is.False, "la base no marca tarea (INC-30)");

            await FillPhase(panel, RaftPhase.Lashing);
            await Confirm(panel);

            Assert.That(panel.Assembly.ActivePhase, Is.EqualTo(RaftPhase.MastAndSail));
            Assert.That(Visibles(panel).Count, Is.EqualTo(17), "todo a la vista en la última fase");
            Assert.That(panel.ConfirmLabel.text, Is.EqualTo(content.TestLabel), "y el botón pasa a «Probar balsa» (RF-42)");
            Assert.That(river.Tasks.IsDone(RiverTaskId.AssembleRaft), Is.True, "el amarre marca la tarea 3 (INC-30)");
            Assert.That(river.MessageLabel.text, Does.Not.Match(@"\d"), "ninguna cifra (CP-03)");
        }

        [Test]
        [Timeout(60000)]
        public async Task AssemblyPanel_RNF19_ElEspacioIncorrectoSeResaltaConColorEIcono()
        {
            var (_, panel) = await OpenAssembly();
            var logs = panel.Slots.Values.Where(entry => entry.Slot.Phase == RaftPhase.Base).ToArray();

            // El tronco largo entra en un espacio de la base: cabe, y es el botón quien lo rechaza.
            await Drag(panel, MaterialKind.Mast, logs[0].Image.rectTransform);
            foreach (var entry in logs.Skip(1))
            {
                await Drag(panel, MaterialKind.Logs, entry.Image.rectTransform);
            }

            Assert.That(panel.Assembly.PlacedIn(logs[0].Slot.Id), Is.EqualTo(MaterialKind.Mast), "colocar no valida (guion §1.8.4)");
            await Confirm(panel);

            Assert.That(panel.Assembly.ActivePhase, Is.EqualTo(RaftPhase.Base), "la fase sigue abierta");
            Assert.That(panel.Wrong, Is.EqualTo(new[] { logs[0].Slot.Id }), "se señala el espacio incorrecto, no el ensamblaje (RF-42)");
            Assert.That(logs[0].Alert.gameObject.activeInHierarchy, Is.True, "con un icono encima…");
            Assert.That(logs[0].Alert.sprite, Is.Not.Null);
            Assert.That(logs[0].Alert.color, Is.Not.EqualTo(Color.white), "…y color: dos indicadores (RNF-19)");
            Assert.That(logs.Skip(1).All(entry => !entry.Alert.gameObject.activeInHierarchy), Is.True, "los correctos no");
            Assert.That(panel.Assembly.Remaining(MaterialKind.Mast), Is.EqualTo(1), "el mástil volvió al inventario (RF-43)");
            Assert.That(logs.Skip(1).All(entry => panel.Assembly.PlacedIn(entry.Slot.Id) == MaterialKind.Logs), Is.True,
                "los cuatro troncos bien puestos se quedan");

            // Al poner el tronco que faltaba, el aviso de ese espacio se apaga.
            await Drag(panel, MaterialKind.Logs, logs[0].Image.rectTransform);
            Assert.That(logs[0].Alert.gameObject.activeInHierarchy, Is.False);
        }

        [Test]
        [Timeout(60000)]
        public async Task AssemblyPanel_RF40_SoltarSobreLaPuntaDeUnTroncoLoPoneEnEseTroncoYNoEnElVecino()
        {
            // Los troncos van en diagonal y sus cajas se solapan: desde la punta trasera de uno, el
            // centro del vecino queda más cerca que el suyo. Decide el dibujo bajo el puntero.
            var (_, panel) = await OpenAssembly();
            var tronco = RiverMovementTests.EnPantalla(panel.Slots["tronco_1"].Image.rectTransform);
            var vecino = RiverMovementTests.EnPantalla(panel.Slots["tronco_2"].Image.rectTransform);
            var punta = tronco.min + Vector2.Scale(tronco.size, new Vector2(0.8f, 0.65f));
            Assume.That(Vector2.Distance(punta, vecino.center), Is.LessThan(Vector2.Distance(punta, tronco.center)),
                "desde la punta, el centro del vecino está más cerca: por cercanía caería en él");

            await Drag(panel, MaterialKind.Logs, punta);

            Assert.That(panel.Assembly.PlacedIn("tronco_1"), Is.EqualTo(MaterialKind.Logs), "el tronco queda donde se soltó");
            Assert.That(panel.Assembly.PlacedIn("tronco_2"), Is.Null, "y no en el vecino");
        }

        [Test]
        [Timeout(60000)]
        public async Task AssemblyPanel_RNF02_ElMapaDeControlesSoloTieneClicYClicSostenido()
        {
            var (river, panel) = await OpenAssembly();
            var comportamientos = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include);

            Assert.That(comportamientos.Where(c => c is UnityEngine.EventSystems.IDragHandler || c is UnityEngine.EventSystems.IBeginDragHandler),
                Is.Empty, "nada usa el arrastre de uGUI ni con el panel abierto");
            var asas = comportamientos.OfType<RaftPieceHandle>().Select(asa => asa.name).ToArray();
            Assert.That(asas.Where(nombre => nombre.StartsWith("Casilla_")).Count(), Is.EqualTo(river.Inventory.Capacity),
                "cada casilla del inventario es un asa");
            Assert.That(asas.Count(nombre => nombre.StartsWith("Espacio_")), Is.EqualTo(panel.Content.Slots.Length),
                "y cada espacio de la balsa");
            Assert.That(asas, Does.Contain("EspacioTemplate"), "más el modelo, inactivo");
            Assert.That(panel.ConfirmButton.GetComponent<Selectable>(), Is.InstanceOf<Button>(), "confirmar es un botón: clic simple");
            Assert.That(panel.TestButton.GetComponent<Selectable>(), Is.InstanceOf<Button>(), "probar es un botón: clic simple");
            Assert.That(river.Pads.All(pad => !pad.gameObject.activeSelf), Is.True, "las flechas se retiraron");
        }

        [Test]
        [Timeout(60000)]
        public async Task AssemblyPanel_RNF02_LaCasillaDelInventarioRecibeElClicSostenidoPorRaycastYSueltaEnElEspacio()
        {
            // El camino real del jugador: el raycast del EventSystem sobre la casilla, no `Take` a
            // mano. Reproduce el fallo del 20/09/2026: la casilla no era objetivo de raycast.
            var (river, panel) = await OpenAssembly();
            var casilla = river.Slots.First();
            var data = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current)
            {
                position = RiverMovementTests.EnPantalla(casilla.rectTransform).center
            };
            var hits = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
            UnityEngine.EventSystems.EventSystem.current.RaycastAll(data, hits);

            Assert.That(hits, Is.Not.Empty, "algo recibe el clic sobre la casilla");
            var handler = UnityEngine.EventSystems.ExecuteEvents.GetEventHandler<UnityEngine.EventSystems.IPointerDownHandler>(hits[0].gameObject);
            Assert.That(handler, Is.Not.Null,
                $"el clic sobre la casilla cae en «{hits[0].gameObject.name}», que no atiende el clic sostenido (RNF-02, RF-40)");

            UnityEngine.EventSystems.ExecuteEvents.Execute(handler, data, UnityEngine.EventSystems.ExecuteEvents.pointerDownHandler);
            Assert.That(panel.Held, Is.EqualTo(MaterialKind.Logs), "al pulsar, el tronco queda en la mano");

            var espacio = panel.Slots.Values.First(entry => entry.Slot.Phase == RaftPhase.Base);
            data.position = RiverMovementTests.EnPantalla(espacio.Image.rectTransform).center;
            UnityEngine.EventSystems.ExecuteEvents.Execute(handler, data, UnityEngine.EventSystems.ExecuteEvents.pointerUpHandler);

            Assert.That(panel.Held, Is.Null, "al soltar, la mano queda vacía");
            Assert.That(panel.Assembly.PlacedIn(espacio.Slot.Id), Is.EqualTo(MaterialKind.Logs), "y el tronco quedó en la silueta");
        }

        [Test]
        [Timeout(60000)]
        public async Task AssemblyPanel_RNF03_ConLaFase3AbiertaLaBalsaElBotonYLasTablillasCabenSinSolaparse()
        {
            var (river, panel) = await OpenAssembly();
            await FillPhase(panel, RaftPhase.Base);
            await Confirm(panel);
            await FillPhase(panel, RaftPhase.Lashing);
            await Confirm(panel);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            AssertCabenSinSolaparse(
                ("balsa", panel.RaftArea),
                ("botón", (RectTransform)panel.ConfirmButton.transform),
                ("inventario", river.InventoryArea),
                ("lista de tareas", river.TaskArea),
                ("tablilla del guía", (RectTransform)river.MessageLabel.transform.parent));
        }

        /// <summary>
        /// En la base y el amarre «Probar balsa» se suma a «Listo» (RF-42, D-j del 30/09/2026): los
        /// dos botones, la balsa, el inventario y las tablillas siguen cabiendo enteros y sin tocarse.
        /// </summary>
        [Test]
        [Timeout(60000)]
        public async Task AssemblyPanel_RNF03_EnLaBaseLosDosBotonesLaBalsaYLasTablillasCabenSinSolaparse()
        {
            var (river, panel) = await OpenAssembly();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            // Assert y no Assume: si el botón no está en la escena el arreglo está roto y esto tiene que fallar.
            Assert.That(panel.TestButton.gameObject.activeInHierarchy, Is.True, "en la base «Probar balsa» está junto a «Listo»");

            AssertCabenSinSolaparse(
                ("balsa", panel.RaftArea),
                ("«Listo»", (RectTransform)panel.ConfirmButton.transform),
                ("«Probar balsa»", (RectTransform)panel.TestButton.transform),
                ("inventario", river.InventoryArea),
                ("lista de tareas", river.TaskArea),
                ("tablilla del guía", (RectTransform)river.MessageLabel.transform.parent));
        }

        [Test]
        [Timeout(90000)]
        [Category("VisualVerification")]
        [Description("Revisar las tres capturas: la balsa crece de la base con siluetas, a la base con amarres, al mástil y la vela; " +
                     "la familia en la orilla a la izquierda y la balsa sobre el agua a la derecha, a la escala de la 3.3, sin tocar a la " +
                     "familia ni los botones; la casilla de tronco a 1:1, sin pixelado; la sombra al 30 % (lectura B, INC-118). " +
                     "En _Base y _Amarre se ven «Listo» y «Probar balsa» lado a lado, con la misma tipografía, sin solaparse con la balsa y con " +
                     "contraste legible; en _MastilYVela hay un solo «Probar balsa», centrado (D-j del 30/09/2026).")]
        public async Task AssemblyPanel_HU12_LaBalsaReflejaLasTresEtapasDeAvance()
        {
            var (_, panel) = await OpenAssembly();
            await Pause(0.15f); // los botones se funden al aparecer: capturar a medio fundido los deja pálidos
            await Awaitable.EndOfFrameAsync();
            CaptureScreenshot("AssemblyPanel_HU12_Base");

            await FillPhase(panel, RaftPhase.Base);
            await Confirm(panel);
            await Pause(0.15f); // los botones se funden al aparecer: capturar a medio fundido los deja pálidos
            await Awaitable.EndOfFrameAsync();
            CaptureScreenshot("AssemblyPanel_HU12_Amarre");

            await FillPhase(panel, RaftPhase.Lashing);
            await Confirm(panel);
            await FillPhase(panel, RaftPhase.MastAndSail);
            await Pause(0.15f); // los botones se funden al aparecer: capturar a medio fundido los deja pálidos
            await Awaitable.EndOfFrameAsync();
            CaptureScreenshot("AssemblyPanel_HU12_MastilYVela");
        }

        [Test]
        [Timeout(60000)]
        public async Task AssemblyPanel_DA133_AlAprobarUnaFaseMamaYLaFamiliaCelebran()
        {
            var (river, panel) = await OpenAssembly();
            var todos = river.Family.Append(river.PlayerRig).ToArray();
            Assert.That(todos.Select(rig => rig.Current), Is.All.EqualTo(ActorAction.Idle), "esperan en reposo mientras se arma");

            await FillPhase(panel, RaftPhase.Base);
            panel.ConfirmButton.onClick.Invoke();

            Assert.That(todos.Select(rig => rig.Current), Is.All.EqualTo(ActorAction.Celebrate), "la fase aprobada se celebra (§13.3, RF-41)");
            await WaitIdle(panel);
        }

        [Test]
        [Timeout(60000)]
        public async Task AssemblyPanel_DA133_CuandoLaBalsaSeHundeLaFamiliaAnimaYNingunoHaceOtroGesto()
        {
            var (river, panel) = await OpenAssembly();
            await FillPhase(panel, RaftPhase.Base);
            await Confirm(panel);
            await FillPhase(panel, RaftPhase.Lashing);
            await Confirm(panel);
            // Mástil y vela cruzados: la balsa no pasa la prueba y se hunde (guion §1.8.4).
            var mastil = panel.Slots.Values.Single(entry => entry.Slot.Accepts == MaterialKind.Mast);
            var vela = panel.Slots.Values.Single(entry => entry.Slot.Accepts == MaterialKind.Cloth);
            await Drag(panel, MaterialKind.Cloth, mastil.Image.rectTransform);
            await Drag(panel, MaterialKind.Mast, vela.Image.rectTransform);

            panel.ConfirmButton.onClick.Invoke();

            var todos = river.Family.Append(river.PlayerRig).ToArray();
            Assert.That(panel.IsBusy, Is.True, "la balsa se hunde");
            Assert.That(todos.Select(rig => rig.Current), Is.All.EqualTo(ActorAction.Encourage),
                "el intento sin éxito se anima, nunca con un gesto de derrota (CP-02, §7.3)");
            await RiverMovementTests.AssertSoloAnimo(todos);
        }

        /// <summary>
        /// «Probar balsa» disponible en cada fase (RF-42, D-j del 30/09/2026): junto a «Listo» en la
        /// base y el amarre; en la última fase «Listo» ya no existe y confirmar es probar.
        /// </summary>
        [Test]
        [Timeout(60000)]
        [Category("Acceptance")]
        public async Task AssemblyPanel_RF42_ElBotonProbarBalsaEstaDisponibleEnCadaFase()
        {
            var (_, panel) = await OpenAssembly();
            var content = panel.Content;

            Assert.That(Rotulos(panel).Count(rotulo => rotulo == content.TestLabel), Is.EqualTo(1), "en la base hay un «Probar balsa»…");
            Assert.That(Rotulos(panel), Does.Contain(content.ConfirmLabel), "…junto a «Listo»");

            await FillPhase(panel, RaftPhase.Base);
            await Confirm(panel);
            Assert.That(Rotulos(panel).Count(rotulo => rotulo == content.TestLabel), Is.EqualTo(1), "en el amarre también…");
            Assert.That(Rotulos(panel), Does.Contain(content.ConfirmLabel), "…junto a «Listo»");

            await FillPhase(panel, RaftPhase.Lashing);
            await Confirm(panel);
            Assert.That(Rotulos(panel).Count(rotulo => rotulo == content.TestLabel), Is.EqualTo(1),
                "en la última fase queda un solo «Probar balsa»: el de confirmar…");
            Assert.That(Rotulos(panel), Does.Not.Contain(content.ConfirmLabel), "…porque «Listo» ya no aparece");
        }

        /// <summary>
        /// La balsa a medio armar se hunde como la terminada y mal armada —gira por un costado y
        /// vuelve— pero la fase sigue abierta y nada se aprueba (RF-40, RF-42, CP-02).
        /// </summary>
        [Test]
        [Timeout(60000)]
        [Category("Acceptance")]
        public async Task AssemblyPanel_RF42_ProbarLaBalsaIncompletaLaHundeYSigueEnLaFase()
        {
            var (river, panel) = await OpenAssembly();
            var troncos = panel.Slots.Values.Where(entry => entry.Slot.Phase == RaftPhase.Base).ToArray();
            await Drag(panel, MaterialKind.Logs, troncos[0].Image.rectTransform);

            panel.TestButton.onClick.Invoke();

            Assert.That(panel.IsBusy, Is.True, "la balsa se hunde");
            var giro = 0f;
            var limite = Time.realtimeSinceStartup + 10f;
            while (panel.IsBusy && Time.realtimeSinceStartup < limite)
            {
                giro = Mathf.Max(giro, Mathf.Abs(Mathf.DeltaAngle(0f, panel.RaftArea.localEulerAngles.z)));
                await Awaitable.NextFrameAsync();
            }

            Assert.That(panel.IsBusy, Is.False, "la animación del panel terminó");
            Assert.That(giro, Is.GreaterThan(1f), "gira por un costado");
            Assert.That(panel.RaftArea.localRotation, Is.EqualTo(Quaternion.identity), "y vuelve derecha");
            Assert.That(panel.RaftArea.anchoredPosition, Is.EqualTo(Vector2.zero), "a su sitio");
            Assert.That(panel.Assembly.ActivePhase, Is.EqualTo(RaftPhase.Base), "la fase sigue siendo la base…");
            Assert.That(panel.Assembly.IsConfirmed(RaftPhase.Base), Is.False, "…y sin confirmar: probar no aprueba (RF-40)");
            Assert.That(panel.Assembly.PlacedIn(troncos[0].Slot.Id), Is.EqualTo(MaterialKind.Logs), "lo bien puesto se queda");
            Assert.That(river.MessageLabel.text, Is.EqualTo(panel.Content.UnfinishedTestMessage), "la tablilla dice que falta armar");
            Assert.That(panel.gameObject.activeInHierarchy, Is.True, "el panel sigue abierto");
            Assert.That(panel.TestButton.interactable && panel.ConfirmButton.interactable, Is.True, "con los dos botones listos para otro intento");
        }

        /// <summary>
        /// Antes de la última fase solo se señala lo mal puesto, no los espacios vacíos: la balsa a
        /// medio armar está incompleta por construcción, y marcar sus vacíos sería un mapa de dónde
        /// va cada pieza (CP-06, guion §1.8.3).
        /// </summary>
        [Test]
        [Timeout(60000)]
        public async Task AssemblyPanel_RF42_ProbarAntesDeTiempoSenalaSoloLoMalPuesto()
        {
            var (_, panel) = await OpenAssembly();
            var troncos = panel.Slots.Values.Where(entry => entry.Slot.Phase == RaftPhase.Base).ToArray();
            await Drag(panel, MaterialKind.Mast, troncos[0].Image.rectTransform); // el tronco largo cabe en la base: se rechaza al probar
            await Drag(panel, MaterialKind.Logs, troncos[1].Image.rectTransform);

            await Probar(panel);

            Assert.That(troncos[0].Alert.gameObject.activeInHierarchy, Is.True, "el espacio con la pieza que no es lleva el icono de alerta");
            Assert.That(troncos.Skip(1).Select(entry => entry.Alert.gameObject.activeInHierarchy), Has.All.False,
                "ni el bien puesto ni los tres vacíos (CP-06)");
            Assert.That(panel.Wrong, Is.EquivalentTo(new[] { troncos[0].Slot.Id }), "el único señalado es el mal puesto");
            Assert.That(panel.Assembly.Remaining(MaterialKind.Mast), Is.EqualTo(1), "y su pieza volvió al inventario");
        }

        // --- utilería ---------------------------------------------------------------------------

        internal static async Task<(RiverSceneController River, AssemblyPanelController Panel)> OpenAssembly()
        {
            var river = await RiverMovementTests.OpenRiver();
            foreach (var (collectible, _) in river.Spawned.ToArray())
            {
                WalkTo(river, collectible.Position);
                river.CollectButton.onClick.Invoke();
            }

            WalkTo(river, river.Config.BuildZonePosition);
            Assume.That(river.Zone.IsOpen, Is.True);
            var panel = river.Assembly;
            await WaitIdle(panel);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            return (river, panel);
        }

        internal static void WalkTo(RiverSceneController river, Vector2 target)
        {
            for (var step = 0; step < 2000 && !river.Zone.IsOpen && Vector2.Distance(river.Walk.Position, target) > 0.005f; step++)
            {
                river.Tick(target - river.Walk.Position, 0.02f);
            }
        }

        /// <summary>Clic sostenido sobre la casilla, arrastre hasta el centro del espacio y soltar.</summary>
        internal static Task Drag(AssemblyPanelController panel, MaterialKind kind, RectTransform target)
        {
            Canvas.ForceUpdateCanvases();
            return Drag(panel, kind, RiverMovementTests.EnPantalla(target).center);
        }

        internal static async Task Drag(AssemblyPanelController panel, MaterialKind kind, Vector2 at)
        {
            panel.Take(kind, at);
            Assume.That(panel.Held, Is.EqualTo(kind), $"se agarró «{kind}»");
            // Sin cuadro entre agarrar y soltar: en el Editor hay un ratón real y `Update` seguiría
            // al cursor de verdad (misma lección que el taller, 12/09/2026).
            panel.Release(at);
            await Awaitable.NextFrameAsync();
        }

        /// <summary>Pone la pieza correcta en cada espacio abierto de la fase.</summary>
        internal static async Task FillPhase(AssemblyPanelController panel, RaftPhase phase)
        {
            foreach (var entry in panel.Slots.Values.Where(entry => entry.Slot.Phase == phase).ToArray())
            {
                if (panel.Assembly.PlacedIn(entry.Slot.Id) == null)
                {
                    await Drag(panel, entry.Slot.Accepts, entry.Image.rectTransform);
                }
            }
        }

        internal static async Task Confirm(AssemblyPanelController panel)
        {
            panel.ConfirmButton.onClick.Invoke();
            await WaitIdle(panel);
        }

        /// <summary>Pulsa el «Probar balsa» que se ve: el de probar en la base y el amarre, el de confirmar en la última fase.</summary>
        internal static async Task Probar(AssemblyPanelController panel)
        {
            var boton = panel.TestButton.gameObject.activeInHierarchy ? panel.TestButton : panel.ConfirmButton;
            boton.onClick.Invoke();
            await WaitIdle(panel);
        }

        private static async Task Pause(float seconds)
        {
            var end = Time.realtimeSinceStartup + seconds;
            while (Time.realtimeSinceStartup < end)
            {
                await Awaitable.NextFrameAsync();
            }
        }

        internal static async Task WaitIdle(AssemblyPanelController panel, float seconds = 10f)
        {
            var deadline = Time.realtimeSinceStartup + seconds;
            do
            {
                await Awaitable.NextFrameAsync();
            } while (panel.IsBusy && Time.realtimeSinceStartup < deadline);

            Assert.That(panel.IsBusy, Is.False, "la animación del panel terminó");
        }

        /// <summary>Los rótulos de los botones del panel que se ven y se pueden pulsar.</summary>
        private static string[] Rotulos(AssemblyPanelController panel) => panel.GetComponentsInChildren<Button>()
            .Where(boton => boton.isActiveAndEnabled && boton.interactable)
            .Select(boton => boton.GetComponentInChildren<Text>().text)
            .ToArray();

        /// <summary>Cada pieza cabe entera en pantalla y ningún par se solapa (RNF-03). Solo vale a 1920×1080: en el Editor con la Game View fija.</summary>
        private static void AssertCabenSinSolaparse(params (string Nombre, RectTransform Rect)[] piezas)
        {
            var pantalla = new Rect(0, 0, Screen.width, Screen.height);

            foreach (var (nombre, rect) in piezas)
            {
                var caja = RiverMovementTests.EnPantalla(rect);
                Assert.That(pantalla.Contains(caja.min) && pantalla.Contains(caja.max), Is.True, $"«{nombre}» cabe entero en pantalla — {caja}");
            }

            for (var i = 0; i < piezas.Length; i++)
            {
                for (var j = i + 1; j < piezas.Length; j++)
                {
                    Assert.That(RiverMovementTests.EnPantalla(piezas[i].Rect).Overlaps(RiverMovementTests.EnPantalla(piezas[j].Rect)),
                        Is.False, $"«{piezas[i].Nombre}» {RiverMovementTests.EnPantalla(piezas[i].Rect)} y «{piezas[j].Nombre}» " +
                                  $"{RiverMovementTests.EnPantalla(piezas[j].Rect)} no se solapan (RNF-03); pantalla {pantalla}");
                }
            }
        }

        private static System.Collections.Generic.List<string> Visibles(AssemblyPanelController panel) =>
            panel.Slots.Values.Where(entry => entry.Image.gameObject.activeSelf).Select(entry => entry.Slot.Id).ToList();

        /// <remarks>Copia del patrón de <c>CaveLightingTests</c>: cada archivo lleva su propio helper.</remarks>
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
