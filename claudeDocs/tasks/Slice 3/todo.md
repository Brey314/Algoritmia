# Tablero — Slice 3: El Río

Plan técnico: [`plan.md`](plan.md). Contrato: `claudeDocs/SPEC.md`.
Cada tarea se cierra con su commit asociado (RNF-17, CT-11).

**Leyenda:** `EM` = EditMode (lógica pura, sin escena) · `PM` = PlayMode (integración) ·
`VV` = VisualVerification · `MCP` = la verificación **exige** el corredor de pruebas conectado.

> ⚠️ **R2 — los Slices 1 y 2 no están hechos.** Este slice generaliza piezas que aún no existen.
> No abrir R02 antes del Checkpoint W-F del Slice 2. Solo **R01**, **R09** y **R10** son
> independientes.

> ⚠️ **R1 abierto — no hay corredor de pruebas MCP.** `run_unity_tests` sigue sin conectar.
> Toda casilla marcada `MCP` exige haberla corrido **a mano** en el Test Runner y **declarar el
> resultado**. No dar por hecho que la suite pasó.

> ⚠️ **Pregunta abierta 1 bloquea R02.** «Fase» significa dos cosas en el Nivel 3 y de ello
> depende el formato de datos persistidos, que es **«preguntar primero»** (`SPEC.md` §Límites).

> ✅ **16/09/2026 — los tres avisos de arriba están vencidos.** Los Slices 1 y 2 tienen el código
> cerrado (R2); las pruebas corren con el Editor abierto por el corredor efímero `TestRunnerApi`
> de `CLAUDE.md` §Comandos o por `unity test` con el Editor cerrado (R1); y la pregunta abierta 1
> quedó resuelta: **tres fases** —base · amarre · mástil y vela—, la recolección no se persiste
> (decisión de Santiago, registrada en `PhaseId.PhasesPerLevel`).

---

## Fase 0 — Cimientos del slice

- [x] **R01 · Assembly `Game.Levels.River` y exclusión con tres niveles** — `XS` · `EM`
      RNF-15, RNF-16, INC-40 · depende de: Slice 2 W01 — **16/09/2026.**
      `Game.Levels.River.asmdef` (referencia única `Game.Core`) + `AssemblyInfo.cs` con los dos
      `InternalsVisibleTo`, y `Game.Levels.River.Tests.asmdef` (EditMode). `AssemblyDependencyTest`
      suma el tercer nivel y exige que sean exactamente tres antes de recorrer la exclusión.
      El asmdef de PlayMode llega con R07, que es la primera tarea con escena.
- [x] **R02 · Desbloqueo del Nivel 3 y granularidad de fase** — `S` · `EM`
      RF-03, RF-04, RF-41, RNF-09, RNF-14, CP-02, HU-11, CU-09, CU-10, INC-27, supuestos 2/9/11
      · depende de: R01, Slice 2 W02 — **16/09/2026.** Sin código de producción nuevo: W02 ya
      dejó `PhasesPerLevel = {1, 3, 3}` y `LevelUnlockPolicy` genérica. Entran las pruebas que
      nombran al Nivel 3 —`SaveStore_RF04_ConfirmarUnaFaseDelNivel3SobreviveAlCierre`,
      `PlayerProfile_RF41_UnaFaseAprobadaNoSePierdeTrasUnaPruebaFallida`— y el «por qué no
      cuatro» en `PhaseId`. El desbloqueo lo cubrían ya
      `LevelUnlockPolicy_RF03_ElNivel3EsperaLasTresFasesDelNivel2` (EM) y
      `LevelSummary_RF03_DevuelveAlMenuConNivel3Desbloqueado` (PM); RNF-09 sigue en
      `SaveStore_RNF09_NoPersisteCampoAlgunoFueraDeLaListaCerrada`.

### ✅ Checkpoint R-A — Cimientos
- [x] Compila sin errores ni warnings nuevos (`check_compile_errors`) — 16/09/2026
- [x] Prueba de exclusión RNF-16 con **tres niveles reales**, corrida y **declarada** —
      EditMode **224/224** el 16/09/2026 (corredor efímero, Editor abierto): las cuatro de
      `AssemblyDependencyTest` en verde con `Fire`, `Wheel` y `River` en la tabla
- [x] El menú habilita el Nivel 3 solo tras completar el Nivel 2 — regla probada en EM y PM
      (ver R02); verlo jugando es de Santiago en la revisión
- [x] **Pregunta abierta 1 resuelta con el usuario** — 16/09/2026: tres fases, sin guardado
      al terminar la recolección
- [ ] Revisado con el usuario

---

## Fase 1 — Andamiaje del Nivel 3 (`andamiaje`)

- [x] **R03 · `HintPolicy` para recolección y ensamblaje** — `M` · `EM`
      RF-13, RF-10, RF-11, RNF-03, CP-06, HU-03, HU-04, CU-09, CU-10, INC-41 · depende de: R02
      — **16/09/2026.** `HintPolicy` no cambia: ya es genérica (qué cuenta como fallo lo decide
      la escena, que solo llama `RegisterFailedAttempt`). Entra el contenido
      `Assets/Game/Data/Guide/N3_Guia.asset` con **cuatro** pasos —`Recolectar` (cubre las
      tareas 1 y 2), `Base`, `Amarre`, `MastilYVela`— y las cuatro pruebas del plan en
      `HintPolicyTests`, que fijan sobre el asset lo que la pista no puede decir. La de
      inventario se apoya en RNF-16: `Game.Scaffolding` no puede referenciar `Game.Levels.River`.
- [x] **R04 · Las cinco secuencias narrativas del N3** (una **condicional**) — `S` · `EM` + `PM`
      RF-05, RF-06, RF-10, RF-12, RNF-01, RNF-18, CP-07, HU-02, INC-28, INC-39,
      guion §7, §8.1, §8.4.1, §8.5, §9 · depende de: R03
      — **16/09/2026.** Cinco escenas en **seis** assets `N3_*.asset`: el puente II cambia de
      ilustración a mitad (bosque del N2 → río) y la ilustración es por secuencia, así que
      `N3_PuenteII` encadena con `N3_PuenteII_Rio`. Los 32 encuadres son los de
      `docs/md/Camara_Narrativa_N3.md`, con sus suavizados (1.4 · 1.0 · 1.6 · 2.5 s) y la línea
      larga de Algoritm en 3.1 partida en cinco (§7.1) y el recuento final en tres (§7.2).
      `ConditionalNarrativeTrigger` (C# plano) dispara la 3.2 una sola vez tras el primer fallo.
      `IllustrationFraming.Warnings` recibe ahora la proporción del sprite: con 16:9 sin duplicar
      el rango en x es `0.5/z`, no `0.25/z`. Objetos provisionales: la carretilla del N2 en el
      puente, `prop_n3_balsa_hundida` en la 3.2 y `prop_n3_balsa_cruzando` en la 3.3, colocados
      **más arriba de lo que propone §6 del documento de cámara** para no quedar bajo el cuadro
      de diálogo (regla de D05: se mueve el objeto, no el encuadre). Entornos: `env_n3_rio` y
      `env_final_fogatas`, renombrados desde el motor. Sin sprites de la familia, igual que el N2.
      `N3_EscenaFinal` se declara cierre reflexivo para que no sea omitible (CP-07); a dónde sale
      lo decide R14. Las seis están registradas en `Narrative.unity` y el Nivel 3 abre con
      `N3_PuenteII` desde `LevelSelect.unity` (script de editor efímero, ya borrado).
      **Suite del 16/09/2026:** EditMode 234/234; PlayMode `Game.UI.PlayMode.Tests` 63/63 con
      `NarrativeScene_RF05_ResuelveLasCincoSecuenciasDelNivel3SinRamas` en verde. Las hojas de
      `docs/md/verificacion_encuadres_N3/` son el diseño y ya coinciden con los assets; se
      rehacen desde el contenido cuando cambie un encuadre o entren los sprites definitivos.

### ✅ Checkpoint R-B — Andamiaje del Nivel 3
- [x] Las cinco escenas narrativas se recorren completas — PlayMode, 16/09/2026 (seis assets)
- [~] La escena 3.2 aparece tras un fallo y **no aparece** si se acierta al primer intento — la
      **regla** está probada (`NarrativeTrigger_Guion841_…`, EM); conectarla a la prueba de la
      balsa es de R10, que es donde existe el fallo
- [~] Ninguna pista del Nivel 3 resuelve la tarea (CP-06) — probado sobre `N3_Guia.asset`
      (materiales, ubicaciones y orden prohibidos en la pista); la lectura de los textos es de
      Santiago en la revisión
- [ ] Revisado con el usuario

---

## Fase 2 — Recolección (`nivel-rio`)

- [x] **R05 · `TaskList` — las cuatro tareas y su correspondencia exacta** — `S` · `EM`
      RF-36, RF-11, RF-43, RNF-03, RNF-19, CP-02, CP-03, **INC-30**, HU-11, CU-09 (FA-5a), CU-10,
      supuesto 11, guion §8.1/§8.2 · depende de: R04 — **17/09/2026.** `RiverTask` (enum de las
      cuatro tareas + la regla de cuándo se marca cada una) y `TaskList` (estado, **sin forma de
      desmarcar**). Las cinco pruebas del plan en `TaskListTests`, con la negativa de INC-30
      (`LaFaseDeBaseNoMarcaTareaPorSiSola`) y la de RF-43 comprobando por reflexión que la superficie
      pública solo marca. Los textos viven en `N3_RiverLevelConfig.asset` (`TaskLabels`, §1.8.1) y
      `RiverLevelConfigTests` los fija contra el guion.
- [x] **R06 · `Inventory`, `Collectible` y proximidad** — `M` · `EM`
      RF-37, RF-38, RF-11, CT-05, RNF-18, CP-02, HU-11, CU-09 (FA-4a) · depende de: R05 —
      **17/09/2026.** `Collectible` (id, clase, nombre, arte, posición **en fracciones de la
      ilustración**, `IsWithinReach` contra una posición inyectada), `Inventory` (capacidad = catálogo,
      recoger no falla, el cuarto ya informa «tienes todo») y `RiverLevelConfig` con radio, velocidad,
      orilla andable y textos. Las cuatro pruebas del plan en `InventoryTests` (+ `RiverWalk` y `BuildZone`
      como regla pura). Sprites **provisionales** por código (`prop_n3_troncos/_sogas/_tela/_mastil`).
- [x] **R07 · Escena `Level3_River` y movimiento con botones en pantalla** — `M` · `PM` `MCP`
      **RF-35**, RF-10, RF-13, RNF-02, RNF-03, CT-06, **INC-01**, supuesto 6, HU-11, CU-09,
      guion §2.1/§8.2 · depende de: R06 — **17/09/2026.** `RiverWalk` (C# plano: posición y límites
      en fracciones, `Step` recorta), `DirectionPad` (una flecha = un botón uGUI con `IPointerDown/Up`,
      **sin Input System**) y `RiverSceneController` (adaptador: plano fijo `(0.400, 0.410) ×1.50` de
      `Camara_Narrativa_N3.md` §5.3, Mamá/materiales/zona colgados de la ilustración). Disposición del
      mockup 11-12: lista arriba-izquierda, inventario 2×2 abajo-izquierda, flechas en cruz y «Recoger»
      abajo-derecha, tablilla del guía arriba, pausa (prefab `MenuPausa`). Escena construida por script
      de editor efímero y registrada en Build Settings (undécima). `RiverScene_INC01_…` inspecciona el
      **`.inputactions`** (cero `<Keyboard>`) **y** que el assembly no referencie `Unity.InputSystem` ni
      `UnityEngine.InputLegacyModule`. `Game.Levels.River.PlayMode.Tests.asmdef` creado.
      `char_mama_cenital.png` provisional (una postura, sin animación).
- [x] **R08 · Zona de construcción** — `S` · `PM` `MCP`
      RF-39, RF-11, RF-04, CP-02, CP-03, HU-11, CU-09 (FA-6a) · depende de: R07 — **17/09/2026.**
      `BuildZone` (C# plano): `Contains` por radio, `TryEnter` nombra lo que falta («sogas, tela y
      mástil», nunca cuántos) y abre una sola vez. **Desviación respecto al plan:** al abrirse **no se
      confirma ni guarda fase alguna** — la recolección no se persiste (decisión de R02); confirmar
      base/amarre/mástil es del panel (R11). El panel es hoy un hueco (`Panel_Assembly`, solo título)
      que se enciende y retira las flechas; R11 lo llena. `env_n3_zona_disponible.png` provisional.
      **Suite del 17/09/2026:** EditMode **251/251**; PlayMode `Game.Levels.River.PlayMode.Tests`
      **10/10** (8 de integración + 2 capturas `VisualVerification` en `TestScreenshots/RiverScene_*`).

### ✅ Checkpoint R-C — Recolección completa
- [x] Se recorre el mapa, se recogen los cuatro materiales y se entra a la zona de construcción —
      `BuildZone_RF39_AbreElPanelSoloConLosCuatroMateriales` (PM, 17/09/2026)
- [x] **Ninguna tecla mueve al personaje** — `RiverScene_INC01_NoExisteVinculacionDeTecladoEnElMapaDeControles`
      (asset de acciones + referencias del assembly). Queda `Assets/Settings/InputSystem_Actions.inputactions`,
      la plantilla de Unity con WASD, que **nadie referencia**: borrarla o no es de R16 (cierre de RNF-02)
- [x] Las tareas 1 y 2 quedan marcadas; las 3 y 4 siguen sin marcar — misma prueba de RF-39 (PM) y
      `TaskList_INC30_…` (EM)
- [x] Lista de tareas e inventario visibles todo el tiempo y sin solaparse —
      `RiverScene_RNF03_ControlesListaEInventarioCabenEnPantallaYNoSeSolapan`. Ojo: el hueco del panel
      de ensamblaje **tapa la lista** al abrirse (captura `RiverScene_RF39_ZonaAbierta`); R11 decide dónde va
- [ ] Radio de proximidad de RF-37 validado jugando (pregunta abierta 3) — hoy `0.06` de la ilustración
      (~170 px a 1920) y velocidad `0.25`/s en `N3_RiverLevelConfig.asset`; se ajusta sin recompilar
- [ ] Revisado con el usuario — pregunta abierta 2: con el plano fijo **los ocho materiales se
      ven desde el arranque** (no hay paneo); el reparto obliga a recorrer la orilla igual
      (`RiverLevelConfig_RF37_ElRepartoObligaARecorrerLaOrillaYCabeEnElPlanoFijo`)
- [x] **Plano de la recolección rehecho (20/09/2026, decisión de Santiago):** solo el bosque, sin río —
      foco `(0.20, 0.20) ×2.5`, el cuadrante inferior izquierdo de `env_n3_rio`; el río entra con el
      empuje del ensamblaje (`RiverLevelConfig_Guion82_ElPlanoDeRecoleccionMuestraSoloElBosqueSinElRio`).
      **El piso termina en `GroundTop = 0.36`** (medido en la ilustración: el pasto llega a y ≈ 0.34–0.40
      antes de los arbustos); orilla andable `x 0.13–0.36 · y 0.05–0.31`, sin el seto ni las raíces;
      los ocho materiales, el arranque y la zona quedan en el piso. **Perspectiva por profundidad**
      (`DepthScaleAt`: 1.0 abajo → 0.55 donde termina el piso) para Mamá, materiales y zona
      (`RiverLevelConfig_DA83_…`, `RiverScene_DA83_…`). Radio de proximidad y de la zona a `0.04`
      (a zoom 2.5 son ≈ 190 px). Botón de ayuda: el circular de los niveles 1 y 2 (`Boton_Pista`).
- [x] **Plano de la recolección abierto (25/09/2026, decisión de Santiago):** que se vea un poco el
      río — foco `(0.2632, 0.2632) ×1.9`, recorte `[0, 0.526]²`: el bosque sigue llenando el plano y la
      orilla con el pie de la cascada asoma a la derecha (`RiverLevelConfig_Guion82_ElPlanoDeRecoleccionEsElBosqueConUnPocoDelRio`,
      que sustituye a la de «sin el río»). El empuje del ensamblaje pasa de alejar (×0,88) a acercar
      (×1,16) hacia el río. Los radios siguen en `0.04`: a zoom 1.9 son ≈ 145 px.

---

## Fase 3 — Ensamblaje y depuración (`nivel-rio`)

> **R09 y R10 no dependen de R05..R08** y se pueden adelantar. Ver pregunta abierta 6.

> **Decisiones de Santiago del 20/09/2026 que cambian R11 respecto al plan:** no hay modal. El
> ensamblaje se arma **sobre el río**: la cámara empuja del plano de juego a `(0.50, 0.23) ×2.2`
> —el río al 85 % del ancho, la orilla parte la pantalla (pasto a la izquierda, agua a la
> derecha)—, con una sombra negra al 30 % sobre la ilustración y la balsa en la mitad. La balsa
> son **cinco troncos** (los cinco hay que encontrarlos por la orilla; comparten una casilla que se
> llena con cinco marcas), **diez amarres** (uno en cada extremo de cada tronco, de un solo rollo
> de soga que se arrastra diez veces), el mástil (un tronco más largo: la trampa de la base) y la
> vela. **Composición, no láminas:** diecisiete espacios pintados uno a uno con ocho sprites
> (pieza + silueta dibujada aparte por clase) en lugar de las 34 láminas de estado. Colocar no
> valida; el botón sí. Error = pieza equivocada o espacio vacío (guion §1.8.4).

- [x] **R09 · `RaftAssembly` — tres fases bloqueantes** — `M` · `EM`
      RF-40, RF-41, RF-11, RF-17, RF-18, RNF-18, CP-02, CP-06, **INC-30**, HU-12, CU-10,
      guion §8.3 · depende de: R01 — **20/09/2026.** `RaftPhase`, `RaftSlot` (contenido: fase,
      material que acepta, posición y tamaño en fracciones del área de la balsa, giro),
      `RaftAssemblyContent` (espacios, arte por clase, amarres por rollo, encuadre, frases) y
      `RaftAssembly` (C# plano: solo se ven los espacios de la fase activa y las consolidadas;
      `Place` no valida; `TakeBack`; `Confirm` consolida o devuelve **solo** lo mal puesto; `Resume`
      para RNF-14; `Rejections` solo para RF-45, sin límite). Las cinco pruebas del plan en
      `RaftAssemblyTests` + `RNF14_Retomar…` + `RF38_ElRolloDeSoga…`.
- [x] **R10 · `RaftValidator` — prueba de balsa y depuración** — `M` · `EM`
      RF-42, RF-43, RF-11, RF-17, RF-18, CP-02, CP-03, CP-06, HU-13, CU-10 (FA-6a, FA-6b),
      guion §8.4 · depende de: R09 — **20/09/2026.** `RaftValidator.WrongSlots` (función pura:
      vacío o con otra pieza) y `ValidationResult`. Las cinco pruebas del plan en
      `RaftValidatorTests`, la de RF-17 y la de CP-06 leyendo el asset real, más
      `RaftAssemblyContent_RF40_ElAssetTraeLaBalsa…` (5 + 10 + 2 espacios, arte y silueta por clase).
- [x] **R11 · Panel de ensamblaje en escena** — `M` · `PM` + `VV` `MCP`
      RF-40, RF-41, RF-42, RF-43, RNF-02, RNF-03, RNF-19, RNF-21, CT-06, HU-12, HU-13,
      CU-10 · depende de: R10, R08 — **20/09/2026.** `AssemblyPanelController` (adaptador:
      `Sombra_Ensamblaje` y `Area_Balsa` cuelgan de la ilustración; espacios instanciados de
      `EspacioTemplate` con `RaftPieceHandle` e `Image_Alerta`; empuje de cámara con
      `IllustrationFraming.ScaleAbout` como el taller; pulso de completado y hundimiento por giro,
      continuos, RNF-21; `Button_Confirm` «Listo» → «Probar balsa»; confirma y guarda cada fase con
      `PhaseId`). Arrastre **sin Input System ni arrastre de uGUI**: asas `IPointerDown/Up` en las
      casillas y en los espacios, y `PointerTracker` (`IPointerMoveHandler` en el `Canvas`) para
      seguir el cursor. `InventoryView` separado (casilla por clase, marcas para los troncos), y
      `Inventory` ahora cuenta por clase (`Required`/`Count`/`Has` = clase completa).
      `GameFlowRunner.PlayingScenes` gana `River 2` y `River 3` → la escena retoma en el panel.
      Pruebas en `AssemblyPanelTests` (RF40, RNF19, RNF02, RNF03 de layout con la fase 3 abierta,
      HU12 `VisualVerification` con tres capturas `TestScreenshots/AssemblyPanel_HU12_*`).
- [x] **R12 · Escena 3.2, condicional al primer fallo** — `S` · `PM` `MCP`
      RF-05, RF-06, RF-11, RF-12, CP-02, CP-07, guion §8.4.1 · depende de: R11 — **20/09/2026.**
      `ConditionalNarrativeTrigger` cableado tras el hundimiento; para que «la escena narra, no
      reinicia» el disparador y las piezas de la fase abierta viven en **estáticos del assembly**
      (memoria de nivel: sobreviven a la recarga de la escena narrativa, no al proceso — RNF-09 no
      admite «escenas vistas»). Pruebas en `ConditionalNarrativeTests` con un `GameFlowRunner` de
      prueba y sin `SceneLoader`: las tres del plan.
      **Suite del 20/09/2026 (Rider MCP, Game View fijada a 1920×1080 con
      `PlayModeWindow.SetCustomRenderingResolution`):** EditMode **265/265** (todos los assemblies; tras el plano del bosque, `Game.Levels.River.Tests` **31/31**);
      PlayMode `Game.Levels.River.PlayMode.Tests` **20/20** (17 de integración + 3 capturas
      `AssemblyPanel_HU12_*`; la de `…PorRaycastYSueltaEnElEspacio` reproduce el camino real del clic —
      la casilla no era objetivo de raycast y el arrastre no arrancaba jugando, corregido el 20/09/2026), `Game.Core.PlayMode.Tests` **8/8**, `Game.UI.PlayMode.Tests` **63/63** (excede el tiempo del puente MCP:
      leer `TestResults.xml` en `persistentDataPath`). Ojo: con la Game View en otra
      proporción (880×405, «Free Aspect») fallan `RiverScene_RF35_…` y `AssemblyPanel_RNF03_…` porque
      el recorte 16:9 deja fuera el borde inferior — es el entorno, no el código.

### ✅ Checkpoint R-D — Ensamblaje completo
- [x] La balsa se construye por las tres fases y se prueba — `AssemblyPanel_RF40_…` (PM) y
      `RaftAssembly_RF40_LasTresFases…` (EM)
- [x] Un fallo devuelve **solo** lo mal puesto y conserva las fases aprobadas (RF-43) —
      `RaftValidator_RF43_…` (EM) y `AssemblyPanel_RNF19_…` (PM)
- [x] La escena 3.2 aparece tras el primer fallo y no aparece si se acierta de una —
      `RiverScene_Guion841_…` (PM)
- [x] Ningún mensaje nombra la pieza correcta (CP-06) — `RaftValidator_CP06_…` sobre el asset
- [ ] Encuadre del ensamblaje validado jugando: `(0.50, 0.23) ×2.2` sale de medir el río en
      `env_n3_rio` (≈ 38 % del ancho en el tramo bajo → 84 % de pantalla; la orilla a esa altura
      pasa por x ≈ 0.50 y parte la pantalla en dos); se ajusta en
      `N3_RaftAssemblyContent.asset` sin recompilar
- [ ] Revisado con el usuario

---

## Fase 4 — Cierre del nivel y del juego

- [x] **R13 · Emisión de los cuatro indicadores del N3** — `M` · `EM` (21/09/2026)
      RF-45, RF-04, RF-07, RNF-09, RNF-14, CP-03, CP-09, OE1 §3.6.1 (notas 1–5), INC-27,
      INC-30 · depende de: R12, Slice 2 W15
      `RiverIndicatorCollector` (`Game.Levels.River`, implementa `ILevelReporter`): **un solo
      recolector para las tres fases**, porque §3.6.1 define los indicadores del N3 sobre el nivel
      entero —«pasos utilizados» son las confirmaciones aceptadas *sobre un máximo de tres*— y las
      tres fases se juegan en el mismo panel; solo el tiempo es de cada fase y `Complete()` reinicia
      el reloj al cerrarla. `RecordConfirmation(ValidationResult, devueltas)`: rechazo = intento;
      aceptación = paso; error corregido = pieza devuelta en el intento anterior cuyo espacio ya no
      aparece señalado en este (un espacio vacío no devuelve nada, así que llenarlo no corrige).
      `PauseOpened`/`PauseClosed` (nota 1). **El reloj de la base arranca al abrir el ensamblaje**,
      no en la orilla: la recolección no es fase persistida. `AssemblyPanelController` lo guarda en
      la memoria de nivel (`s_indicators`, junto a `s_stash`) para que sobreviva a la escena 3.2 —a
      la que sale con `PauseOpened()` y de la que vuelve con `PauseClosed()`—, lo registra en
      `GameFlowRunner.ActiveReporter`, al retomar desde disco siembra las fases ya aceptadas como
      pasos (`confirmedPhases`, RNF-14) y persiste `Complete()` donde antes iba `default`.
      EditMode (`RiverIndicatorTests`, 8/8 el 21/09/2026 contra el Editor abierto):
      `RiverIndicators_RF45_IntentosCuentaFasesRechazadasYPruebasFallidas`,
      `_RF45_PasosUtilizadosNoSuperaTres`, `_RF45_RetomarEnUnaFaseCuentaLasConfirmadasAntes`,
      `_RF45_ErrorCorregidoExigeRecolocacionCorrectaPosterior`,
      `_RF07_LaPausaNoSumaTiempoDeResolucion`, `_OE1361_ElTiempoDeResolucionEsDeCadaFase`,
      `_OE1361_ReiniciarElNivelNoBorraLosIndicadoresRegistrados`,
      `_CP03_NingunIndicadorLlegaALaUIDelEstudiante` (barrido de reflexión sobre `Game.UI`).
      Regresión: EditMode completo 275/275 y `Game.Levels.River.PlayMode.Tests` 20/20.
- [x] **R15 · Doble indicador y contraste en los estados de error del N3** — `S` · `VV` `MCP` (21/09/2026)
      **RNF-19** (cierra su segunda mitad), RNF-20, RNF-21, CN-04, HU-13 · depende de: R11
      `RiverAccessibilityTests` (PlayMode, corrido contra el Editor abierto por Rider, **4/4**, y la
      suite del río **24/24**): `RiverLevel_RNF19_LosEstadosDeErrorSeLeenSinColor` (zona sin
      materiales, colocación incorrecta y prueba de balsa fallida: icono ≠ al del acierto + texto sin
      cifras en la tablilla, y el icono de alerta encendido sobre cada espacio señalado; tres
      capturas), `_RNF19_LaListaDeTareasSeLeeEnEscalaDeGrises` (visto y círculo son sprites
      distintos; captura **desaturada** por la propia prueba), `_RNF20_ContrasteSuficienteSobreElEscenarioClaro`
      (WCAG texto/cara medido en escena y **exigiendo que ningún texto cuelgue de la ilustración**:
      tablilla del guía 13,3:1 · tareas 6,3:1 · «Recoger», «Listo»/«Probar balsa» y los cinco
      botones de la pausa ≥ 7,1:1) y `_RNF21_NingunaAnimacionDelNivel3TieneDestellos` (empuje de
      cámara, pulso de fase confirmada y hundimiento muestreados cuadro a cuadro: nada se apaga,
      la sombra no parpadea, ningún salto mayor que el que sigue el ojo; recoger no anima; tres
      capturas). **El cruce (RF-44) no existe todavía: lo cubre R14.**
      **Hallazgo real de RNF-19:** entrar a la zona sin los materiales se mostraba con tono `Help`,
      cuyo icono está vacío en la escena (igual que en las tres del Nivel 2): solo palabras, sin
      icono ni color. Es una acción rechazada (CU-09 FA-6a), no una pista: `RiverSceneController.Enter`
      pasa a `MessageTone.Rejected` — icono de alerta más color, la frase no cambia (CP-02).
      **Falso hallazgo de RNF-20, documentado en la prueba:** el botón de confirmar medía 3,9:1 en
      las capturas porque se deshabilita en cada animación y vuelve con un `CrossFade` de 0,1 s; el
      corredor va a ~1000 fps y capturaba a 3 ms del desvanecido. En juego real es ámbar sólido:
      la prueba espera el desvanecido y exige el `CanvasRenderer` en blanco antes de medir.
      Capturas en `%AppData%\LocalLow\DefaultCompany\My project\TestScreenshots\RiverLevel_*.png`,
      revisadas.
- [x] **R16 · Cierre de RNF-02 y RNF-16 sobre el juego completo** — `S` · `PM` + `EM` `MCP` (21/09/2026)
      **RNF-02**, **RNF-16**, CT-06, **INC-01** · depende de: R11
      **Hallazgo real:** nueve escenas —las cuatro jugables del N2 y N3 y las cinco de flujo con
      módulo— usaban en su `InputSystemUIInputModule` el `DefaultInputActions` del paquete Input
      System (teclado y mando incluidos), y el mapa de acciones del proyecto (Input System →
      Project-wide Actions, que entra al ejecutable) era la plantilla `Assets/Settings/
      InputSystem_Actions.inputactions` con 23 vinculaciones de teclado; solo `Level1_Cave` estaba
      bien. Recableadas desde el motor las nueve a `Assets/Game/Input/ControlesJugables.inputactions`
      (el setter de `actionsAsset` remapea Point/Click/RightClick/MiddleClick/ScrollWheel por
      nombre; Move/Submit/Cancel quedan nulos: es lo que se quiere) y el proyecto apunta al mismo
      asset (`EditorBuildSettings.asset`). Al mapa se le quitó la vinculación `<Mouse>/scroll`
      —el criterio pide «ni rueda del ratón»; la acción sigue, sin binding—; quedan
      `<Pointer>/position`, `<Mouse>/leftButton|rightButton|middleButton`,
      `<Touchscreen>/primaryTouch/tap` y `<Pen>/tip`. La plantilla de `Assets/Settings/` sigue en
      disco sin que nada la use: borrarla es decisión de Santiago. EditMode (`Game.Architecture.Tests`,
      leen disco): `Architecture_RNF16_RetirarUnNivelNoAfectaALosOtrosDos` (por nivel: ningún
      módulo de runtime lo tiene en su cierre de dependencias y ninguna escena ajena, de flujo ni
      prefab compartido contiene guids de sus scripts — las tres combinaciones),
      `Architecture_RNF02_NingunAssemblyUsaLaClaseInputLegada` (`activeInputHandler: 1` en
      `ProjectSettings.asset` más barrido de `Input.GetKey|GetMouse|GetAxis|mousePosition…` en
      `Scripts/`), `Architecture_INC01_ElMapaDeControlesNoTieneTecladoMandoNiRueda` y
      `Architecture_RNF02_LasEscenasYElProyectoUsanSoloElMapaDeControlesDelJuego` (toda escena con
      `m_ActionsAsset` y el proyecto → `ControlesJugables`; las cinco jugables llevan módulo).
      PlayMode (`Core`, el asmdef ganó `Unity.InputSystem` y `UnityEngine.UI`):
      `Controls_RNF02_LasCincoEscenasJugablesSoloAceptanClicYClicSostenido` carga las cinco de
      verdad: módulo → `ControlesJugables`, vinculaciones solo de clic, puntero y clic cableados,
      sin `PlayerInput`, sin `ScrollRect`; lo que cada nivel deja hundir o arrastrar sigue en su
      propia `*_RNF02_*`. `unity test` (Editor cerrado): EditMode **282/282**; PlayMode
      `Core` + todas las `RNF02`/`INC01` de los cinco niveles **17/17**; PlayMode completo como
      regresión del recableado: **165/184**, 6 omitidos (visuales que piden Game View) y 13
      fallos, todos de disposición o captura medidos contra la pantalla de 640×480 de batchmode
      —los diez ya anotados en R14 más `ForestScene_RF26_LaCajaYLosTroncos…`,
      `MazeScene_RNF03_ConMasBloques…` y `WorkshopScene_RNF03_NadaSeSale…`—; los dos recorridos
      completos del N2 (RNF-13), que pasan por las nueve escenas recableadas, pasan. **Los trece
      se repiten en el Editor con Rider antes del checkpoint R-E.**
- [x] **R14 · Cruce, escena final y cierre del juego** — `M` · `EM` + `PM` `MCP` (21/09/2026)
      RF-44, RF-12, RF-45, RF-17, RF-08, RF-03, RNF-13, CP-03, CP-07, CP-10, HU-13, HU-14,
      CU-10, INC-26, INC-37, **INC-39**, guion §8.5/§9 · depende de: R13, R15, R16 (R16 sigue
      abierta: son pruebas de RNF-02/RNF-16, no código del cierre)
      **El flujo lo deciden los assets, no una rama por nivel.** `GameFlow` acepta
      `Narrative → Credits`. `NarrativeSequence.EndsInCredits` —marcado en `N3_EscenaFinal`, que
      **sigue siendo** cierre reflexivo: esa es la marca que le niega «Omitir» la primera vez sin
      tocar `NarrativeVisitPolicy`— y `NarrativeSceneController.Leave` lo atiende antes que al
      resumen. `LevelSummaryMessages.ClosingSequenceId` (vacío = menú de niveles, como N1 y N2)
      saca «Continuar» a la escena final; lo declara `N3_ResumenNivel.asset` (nuevo, creado desde
      el motor, cableado en `LevelSummary.unity`). El cruce (RF-44) es `PropMotion.Drift` —el
      mismo avance suavizado del rodado, sin giro ni caída— sobre la balsa de `N3_Escena33_Cruce`
      (0,12 del ancho a la orilla derecha, 9 s, desde la línea 0): no hizo falta
      `RiverCrossingSequence.cs`. `env_final_fogatas.png` ya era la ilustración de la escena final
      y sus seis paradas cierran sobre la fogata central (0,5 · 0,35 de la lámina): ningún encuadre
      cambió. EditMode: `GameFlow_INC39_TrasElResumenDelUltimoNivelLaEscenaFinalSaleALosCreditosYAlInicio`,
      `LevelSummary_RF45_ElResumenDelNivel3NoContieneNingunDigito`,
      `LevelSummary_RF12_NombraLaDescomposicionYLaDepuracion`, y
      `NarrativeVisitPolicy_CP07_ElCruceYLaEscenaFinalNoSeOmitenLaPrimeraVez` exige ahora
      `EndsInCredits` solo en la final. PlayMode (`GameEndingTests`, en `Levels/River` y no en
      `Core` como decía el plan, porque el cruce lo dispara el panel de ensamblaje):
      `GameEnding_INC39_RecorreLevelSummaryNarrativeCreditsYMainMenu` (Boot → N3 fase 3 → cruce →
      resumen → escena final sin «Omitir» → créditos → «Volver» → inicio, perfil íntegro en disco) y
      `GameEnding_RF44_LaPruebaSuperadaReproduceElCruceYElCierre` (balsa completa → cruce; la balsa
      avanza sin girar; el cierre llega con las tres fases), **2/2**. Regresión con `unity test`
      (batchmode, Editor cerrado): EditMode **279/279**; River PlayMode 17/26 y UI+Core 68/71 —
      los diez que fallan son los visuales y de disposición que miden contra la Game View, que
      batchmode abre a 640×480 y sin captura (`AssemblyPanel_RNF03`, `AssemblyPanel_HU12`,
      `RiverScene_RF35_…Limites`, `RiverScene_RNF03`, `RiverLevel_RNF19/RNF20`,
      `RiverScene_RF36/RF39`, `NarrativeScene_RNF01`): son los mismos que pasaron 24/24 a las
      13:05 en el Editor. **Pendiente: repetir esos diez en el Editor con Rider** antes del
      checkpoint R-E.

### ✅ Checkpoint R-E — Slice 3 completo
- [x] **Dos recorridos completos** del Nivel 3 sin incidencias (RNF-13) — 21/09/2026,
      `RiverLevelJourneyTests` (desde `Boot` con perfil real: puente II → llegada → orilla →
      recolección → zona → base → amarre → mástil y vela → prueba → cruce → resumen → escena
      final → créditos → inicio; narrativas línea a línea con «Continuar»; comprueba en disco las
      tres fases, intentos y errores corregidos de la fase 3 y el tiempo de la base):
      `RiverLevel_RNF13_RecorreElNivel3CompletoHastaElInicio_{AcertandoAlPrimerIntento,FallandoLaPrueba}`,
      **2/2** con `unity test` (Editor cerrado). El clic manual sobre el ejecutable sigue siendo de Santiago.
- [x] Un recorrido **acertando al primer intento** (sin escena 3.2) y otro **fallando** (con ella) —
      son los dos casos de arriba: el que falla cruza mástil y vela, ve la 3.2, vuelve a la fase 3 con
      la escena recargada y las piezas devueltas, y corrige (Intentos = 1, Errores corregidos > 0)
- [x] Cierre forzado en cada fase confirmada → retoma donde iba (RNF-14) —
      `RiverLevel_RNF14_CierreForzadoTrasCadaFaseConfirmadaRetomaDondeIba_{TrasLaBase,TrasElAmarre}`
      (guarda, destruye runner y cargador, arranca de nuevo desde `Boot`, carga el perfil de disco y
      pide la fase 1: el flujo retoma en la pendiente, el panel abre en ella con las anteriores
      consolidadas y la recolección dada por hecha), **2/2**. Tras la fase 3 el nivel está completo y
      repetir la pedida es legítimo (`GameFlow_RNF14_…`)
- [x] **RNF-02 cerrado**: cinco escenas jugables inspeccionadas, cero teclado (INC-01) — R16, 21/09/2026
- [x] **RNF-16 cerrado**: las tres combinaciones de exclusión — R16, 21/09/2026
- [x] Carga de `Level3_River` < 10 s y memoria < 2 GB, **medidas** (RNF-04, RNF-05) — 21/09/2026,
      `RiverLevel_RNF04_LaEscenaDelNivel3CargaEnMenosDeDiezSegundos`: **1,36 s** por
      `SceneLoader.LastLoadSeconds`; `RiverLevel_RNF05_LaMemoriaQuedaBajoDosGigasConElNivel3Cargado`:
      **1 226 MB reservados / 785 MB asignados** con el panel de ensamblaje abierto. Medidas en el
      Editor batchmode, que carga más que el ejecutable: son cota superior
- [x] Paquete < 500 MB con el arte de los **tres** slices (RNF-06) — 21/09/2026, `unity build
      --target StandaloneWindows64` con las once escenas: **217 MB** (`Algoritmia_Data` 150 MB,
      `UnityPlayer.dll` 36 MB, `DirectML.dll` 14 MB). Incluye la carpeta
      `My project_BurstDebugInformation_DoNotShip` (1 MB), que se quita del entregable a mano. El
      ejecutable **no se corrió** en esta sesión: `Datos/` junto al `.exe` (RNF-07/RNF-11) sigue
      siendo comprobación de Santiago. **Ojo:** la build dejó `SENTIS_ANALYTICS_ENABLED` en los
      símbolos de `Standalone` y `UnityConnectSettings.m_Enabled: 1` (revertidos), y mete
      `DirectML.dll` + `D3D12/` —los paquetes de IA del Editor entrando al ejecutable—; quitarlos
      del `manifest.json` es «preguntar primero» (RNF-08/RNF-10): decisión de Santiago
- [ ] **PG-05** verificado sobre los tres niveles — observación en la sesión con estudiantes, no una
      aserción: queda para Santiago
- [x] RF-35..RF-44 tienen cada uno al menos una prueba que los nombra (CT-10) — barrido sobre
      `Assets/Tests` el 21/09/2026: RF-35 (3), RF-36 (4), RF-37 (3), RF-38 (4), RF-39 (4), RF-40 (4),
      RF-41 (3), RF-42 (1), RF-43 (3), RF-44 (1)
- [ ] **Golden Path del juego entero**, de la pantalla de inicio a los créditos, en 20–40 minutos —
      el recorrido está automatizado por tramos (`LevelSummaryTests` N1, `WheelLevelJourneyTests` N2,
      `RiverLevelJourneyTests` N3 + cierre) pero el tiempo lo mide una persona jugando: de Santiago
- [ ] Revisado con el usuario antes de abrir el Slice 4 — pendiente también en R-A..R-D. Antes de la
      revisión: repetir en el Editor con Rider (Game View 1920×1080) los trece de disposición/captura
      que batchmode no puede correr (ver R14 y R16)

---

## Assets visuales — `plan.md` §Assets visuales del Slice 3

Escenarios, props e interfaz **originales del proyecto**. **Los personajes no se rediseñan**:
Mamá es `A3` del Slice 1 —**obra derivada** con autorización concedida y mención obligatoria en
créditos (CT-09, RNF-23)— y `C2` solo genera su vista cenital **con sus rasgos copiados
literalmente**. Cada asset se registra en `CreditsContent.asset` (Slice 1, T08).

**Cinco bloques fijos por prompt**, copiados palabra por palabra antes de la descripción:
`[1 CONTEXTO] [2 ESTILO] [3 PALETA] [4 ENTREGA] [5 PROHIBICIONES]`. Un asset generado sin los
cinco se descarta y se vuelve a pedir. La paleta y las especificaciones salen de
`claudeDocs/Direccion_de_Arte.md`.

**Todo el nivel se dibuja en vista cenital pura de 90 grados**, salvo `C10`, que es la
ilustración lateral de la escena final.

**Chroma:** verde `#00FF00` por defecto; **magenta `#FF00FF`** en `C2`, porque el personaje se
recorta sobre un entorno de follaje. Los fondos de escena no llevan chroma.

- [ ] **C1 · Escenario del río, vista superior** — chroma **no** — RF-35, RF-39, guion §8/§8.2
- [ ] **C2 · Mamá vista superior, cuatro direcciones** — chroma **magenta** — RF-35, CU-09, HU-11
      ⚠️ pegar el bloque «RASGOS FÍSICOS FIJOS» de `A3` (Slice 1) literalmente
- [ ] **C3 · Botones de dirección y botón «Recoger»** — chroma verde — **RF-35**, RF-37, RNF-02,
      RNF-19, **INC-01**
- [ ] **C4 · Los cuatro materiales + sus iconos de inventario** — chroma verde — RF-37, RF-38, RNF-19
- [ ] **C5 · Lista de tareas e inventario** — chroma verde — RF-36, RF-38, RNF-19, RNF-20, INC-41
- [ ] **C6 · Zona de construcción, dos estados** — chroma verde — RF-39, CU-09 (FA-6a)
- [ ] **C7 · La balsa en tres estados: base / amarre / mástil y vela** — chroma verde — RF-40,
      RF-41, HU-12, guion §8.3
- [ ] **C8 · Panel de ensamblaje: espacio vacío / correcto / incorrecto** — chroma verde — RF-40,
      RF-42, **RNF-19**, HU-13
- [ ] **C9 · Balsa hundiéndose y balsa cruzando** — chroma verde — RF-42, RF-44, RNF-21, guion §8.4/§8.5
- [ ] **C10 · Escenario de la escena final, las fogatas** — chroma **no** — RF-44, RF-12, guion §9
- [ ] Postproceso: recorte del verde, alfa, halo, **mismo `Pixels Per Unit` que los Slices 1 y 2**
- [ ] Desaturar `C3`, `C4`, `C5` y `C8` y verificar que se siguen distinguiendo (RNF-19)
- [ ] Verificar RNF-20 sobre el arte final: escenario claro y cenital, el caso más expuesto
- [ ] Verificar RNF-21 sobre las **animaciones** montadas con `C7` y `C9`, no sobre las láminas

---

## Bloqueantes y decisiones pendientes

- [ ] **R2 · Cerrar los Slices 1 y 2** antes de abrir R02. Bloqueante duro.
- [ ] **Pregunta abierta 1 · ¿Qué es una «fase» del Nivel 3 para el guardado?** CU-09/CU-10 dicen
      dos; RF-40 y §3.6.1 dicen tres de ensamblaje. Propuesta: cuatro puntos de guardado
      —recolección, base, amarre, mástil y vela— con `Pasos utilizados` contando solo los tres de
      ensamblaje. **Cambiar el formato de datos persistidos es «preguntar primero».** Bloquea R02.
- [ ] **R1 · Instalar el servidor MCP de Unity** (`run_unity_tests`). Siete tareas `MCP` en el
      slice que cierra el juego.
- [ ] **Pregunta abierta 2 · Trazado del escenario del río.** Validar `N3_RiverLevelConfig.asset`
      jugando: ningún material visible desde la posición inicial, zona de construcción señalizada
      desde el principio.
- [ ] **Pregunta abierta 3 · Radio de proximidad de RF-37**, sin validar. Revisar en R-C.
- [ ] **Pregunta abierta 6 · ¿Se adelantan R09 y R10?** Recomendación: sí, para descargar INC-30.
- [ ] **PG-02 · nombre del guía.** Última oportunidad antes de la entrega: la escena final lo nombra.
- [ ] **PG-01 · título del producto.** Sigue abierto y el juego ya estaría completo (RF-01, RF-08).
