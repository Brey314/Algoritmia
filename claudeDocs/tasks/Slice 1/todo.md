# Tablero — Slice 1: Golden Path temprano

Plan técnico: [`plan.md`](plan.md). Contrato: `claudeDocs/SPEC.md`.
Resultados: [`Fase-0-Resultados.md`](Fase-0-Resultados.md) · [`Fase-1-Resultados.md`](Fase-1-Resultados.md) ·
[`Fase-2-Resultados.md`](Fase-2-Resultados.md) · [`Fase-3-Resultados.md`](Fase-3-Resultados.md) (en curso).
Cada tarea se cierra con su commit asociado (RNF-17, CT-11).

**Leyenda:** `EM` = EditMode (lógica pura, sin escena) · `PM` = PlayMode (integración) ·
`VV` = VisualVerification.

> ⚠️ **R1 cerrado (06/09/2026).** Rider 2026.2 y el plugin «MCP Server Extension for Unity»
> (id 30357) instalados; el MCP `rider` quedó registrado. `mcp__rider__*` solo responde con
> **Rider abierto**: con Rider cerrado la sesión arranca con `ConnectionRefused` — eso no es que
> falte el plugin, es que no hay a quién preguntar. En ese caso las pruebas van por
> `unity test --mode {EditMode|PlayMode} --output <x>.xml` (CLI oficial, reemplaza al
> `Unity.exe -runTests` crudo) — deja XML de NUnit pero exige el Editor cerrado. Toda casilla de
> prueba marcada abajo exige **declarar el resultado**. No dar por hecho que la suite pasó.

---

## Fase 0 — Cimientos

- [x] **T01 · Estructura de carpetas y assemblies** — `S` · `EM`
      RNF-15, RNF-16, INC-40 · depende de: —
      Compila y `AssemblyDependencyTest` pasa: **4/4**.
- [x] **T02 · `PlayerProfile` y `SaveStore` (JSON en `Datos/`)** — `M` · `EM`
      RF-02, RF-04, RNF-07, RNF-09, RNF-11, RNF-14, HU-01, CU-01, INC-27, INC-34 · depende de: T01
      **12/12** en EditMode. `LevelId` se adelantó aquí (T02 lo necesita) en vez de en T03.
- [x] **T03 · `GameFlow`, la FSM en C# plano** — `M` · `EM`
      RF-01, RF-03, RF-05, RF-07, RF-08, RF-09, CP-02 · depende de: T01, T02
      **7/7** en EditMode. `Narrative` se parametriza con el **id** de la secuencia, no con el
      `NarrativeSequence`: ese SO vive en `Game.Scaffolding`, que depende de `Game.Core`.
- [x] **T04 · `SceneLoader`, `GameFlowRunner` y escena `Boot`** — `S` · `PM`
      RNF-04, RNF-16 · depende de: T03
      **3/3** en PlayMode (03/09/2026). La primera corrida falló: las pruebas esperaban el estado
      de la FSM y afirmaban la escena activa, pero `LoadScene` se aplica al final del frame. Se
      corrigió la espera y se añadió un `[TearDown]` — sin él, `GameFlowRunner` y `SceneLoader`
      sobrevivían entre pruebas y dos pasaban sin probar nada. Solo cambió código de prueba.
- [x] **T04b · Arreglo de la medición de RNF-04 en `SceneLoader`** — `XS` · `PM` (06/09/2026)
      RNF-04 · depende de: T04 · fix-bug test-first
      `LastLoadSeconds` cronometraba `SceneManager.LoadScene` (solo encola) → reportaba ≈ 0 s.
      RED (`Expected > 0.001 ... But was 0.00034530001f`) → fix con `LoadSceneAsync` + `completed`
      → GREEN **PlayMode 4/4, EditMode 23/23**. Detalle en `Fase-0-Resultados.md` §3.4.

### ✅ Checkpoint A — Cimientos
- [x] Compila sin errores ni warnings nuevos — **0 errores, 0 warnings** (re-verificado 06/09)
- [x] Pruebas EditMode de Core corridas y **declaradas** — **23/23** (re-verificado 06/09)
- [x] Pruebas PlayMode corridas y **declaradas** — **4/4** (06/09)
- [x] Arranca en `Boot` y llega a `MainMenu` — `BootFlow_RF01_…` pasa
- [x] Mecanismo de medición de RNF-04 correcto — corregido y verificado 06/09 (T04b, §3.4)
- [x] Cifra de RNF-04 sobre config vinculante — trasladada al Checkpoint D (build portable, equipo de referencia)
- [x] Revisado con el usuario — **06/09/2026**, Checkpoint A cerrado; se abre la Fase 1

**✅ Checkpoint A cerrado el 06/09/2026.**

---

## Fase 1 — Navegación mínima (`sistema-navegacion`)

- [x] **T05 · Pantalla de inicio** — `S`→`M` · `PM` + `VV` (06/09/2026)
      RF-01, RF-09, RNF-01, RNF-20, HU-01, CU-01, PG-01 · depende de: T04
      Escena `MainMenu` con título (SO `GameTitleConfig` = «Algoritm»), Jugar / Créditos / Salir.
      Salir → `ProfileSession.SaveActive()` antes de `Application.Quit()` (RF-09). Persistencia real
      cableada: `DiskFileSystem`, `SaveStore` y `ProfileSession` en `GameFlowRunner` (lazy).
      **EditMode 27/27** (4 nuevas: ProfileSession ×2, DiskFileSystem ×2) · **PlayMode UI 5/5**
      (RF-01 presencia+raycast, RF-01 layout, RF-09 orden guardar→cerrar, RNF-18 título del SO,
      RNF-20 contraste — captura analizada: texto #3A1E18 sobre #F7EFE2 / #E8A33D / #E0D4C0,
      todos ≥ 4,5:1). Tipografía: fuente del sistema; Baloo 2 / Nunito (Dir. Arte §11.2) es tarea
      de assets. `/[Dd]atos/` al `.gitignore`.
- [x] **T06 · Perfil de un solo nombre** — `M` · `EM` + `PM` (06/09/2026)
      RF-02, RF-03, RNF-09, HU-01 (FA-01..FA-03), CU-01, CU-02 · depende de: T05
      **Panel dentro de `MainMenu`, no escena** (SPEC §Estructura no lista `ProfileSelect`; el
      plan sí — gana SPEC). `MainMenu` se parte en `MainPanel` / `ProfilePanel`; «Jugar» los
      intercambia sin recargar (`GameFlowRunner.Apply` no recarga la escena ya activa).
      `ProfileSelectController`: lista de perfiles guardados + campo único de nombre. Validación
      (FA-01 vacío, FA-02 duplicado) reutiliza `PlayerProfile.Create` de T02. Perfil nuevo →
      `SelectProfile` + `StartNarrative("N1_Apertura")` (FA-03; la escena Narrative es T10).
      `ProfileSession` pasó a ser la fachada de perfiles (listar/cargar/crear/guardar).
      **EditMode 30/30** (ProfileSession 5) · **PlayMode 16/16** (GameFlowRunner 2, ProfileSelect 5,
      MainMenu 5 sin regresión, BootFlow/SceneLoader 4). Se arregló una fragilidad de orden en el
      test RNF-04 de `SceneLoader` (T04b): esperaba la escena activa, ahora espera el dato del
      cronómetro.
- [x] **T07 · Menú de niveles con desbloqueo progresivo** — `M` · `EM` + `PM` + `VV` (07/09/2026)
      RF-03, RNF-19, RNF-20, HU-01, CU-02 · depende de: T06
      `LevelUnlockPolicy` (C# plano — completar un nivel habilita solo el siguiente, nunca
      re-bloquea) **4/4 EditMode**. Escena `LevelSelect` (Build Settings + `GameFlowRunner.Scenes`)
      con los 3 niveles siempre visibles. `LevelSelectController`: `Refresh()` pinta bloqueado/
      desbloqueado según el perfil activo. **RNF-19: candado (`ui_lock.png`, sprite generado a
      mano) + «Bloqueado» + atenuado**, y el botón no responde al clic. **PlayMode 5/5** (incl.
      captura RNF-19 analizada). `GameFlowRunner.Apply` tolera no tener `SceneLoader`.
- [x] **T08 · Pantalla de créditos mínima** — `XS` · `PM` (07/09/2026)
      RF-08, CT-09, RNF-18, RNF-23 · depende de: T05
      Escena `Credits` + `CreditsController` + SO `CreditsContent` (texto editable sin recompilar,
      con la **atribución a la Familia Anonaky** — obra derivada con autorización, PG-07 cerrado).
      «Volver» → `MainMenu`. **PlayMode 2/2** (contenido de autoría + navegación).

- [x] **T08b · Los botones lanzaban `NullReferenceException` fuera del arranque por `Boot`** — `XS` ·
      `PM` (07/09/2026)
      RNF-13 · depende de: T05–T08 · fix-bug test-first
      Al pulsar Play sobre una escena suelta, `GameFlowRunner.Instance` es `null` y **cada clic
      lanzaba**: `MainMenuController:50` (Créditos) y `:58` (Jugar), `LevelSelectController:47`
      (Volver), `CreditsController:32` (Volver) — las cuatro en `Logs/Editor.log`. Una sola causa:
      los adaptadores desreferencian el singleton que solo crea `Boot`. RED **4/4** con la misma
      excepción → nuevo `ScreenFlow` (punto único de comprobación, sostiene un nivel más arriba la
      promesa de `GameFlow` de no lanzar ante una transición imposible) → GREEN **PlayMode 25/27,
      EditMode 34/34**. `MainMenuController` pasó a `Runner` inyectable como los otros tres.
      Los 2 fallos restantes son los de `VisualVerification`: `ScreenCapture` no encuentra la vista
      de juego en batchmode — **preexistentes**, medidos 0/2 también sobre el código sin el arreglo.

### ✅ Checkpoint B — Navegación
- [x] Perfil nuevo → Nivel 1 habilitado, Niveles 2 y 3 bloqueados **con icono además de color** — `LevelSelect_RF03_*` + RNF-19
- [~] Cerrar y reabrir conserva el perfil y su progreso (RNF-14, manual sobre el ejecutable) —
      **build portable hecho el 08/09** (`unity build`, 137 MB, 5 escenas). Se creó un perfil
      jugando y quedó en `Build/Algoritmia/Datos/yo.json`; el archivo sobrevive al cierre. Falta
      **ver el perfil listado al reabrir**, que es un clic en «Jugar» y lo hace el usuario.
- [~] `Datos/` aparece junto al ejecutable, sin residuos fuera de ella (RNF-07, manual) —
      **`Datos/` sí nace junto al `.exe` en la primera ejecución** (RNF-07 ✅). **Pero hay residuo
      fuera** (RNF-11 ❌): el reproductor escribe en
      `%AppData%\LocalLow\DefaultCompany\My project\` un `Player.log` (PlayerSettings
      `usePlayerLog: 1`) y archivos de **Unity Analytics / Insights**, que el módulo
      `com.unity.modules.unityanalytics` sigue escribiendo **aunque `UnityConnectSettings` esté en
      `m_Enabled: 0`**. Ningún dato del jugador vive ahí —el perfil solo está en `Datos/`—, pero
      son residuos fuera de la carpeta portable y, en el caso de la telemetría, algo que RNF-08
      prohíbe. **Dos decisiones pendientes del usuario:** quitar
      `com.unity.modules.unityanalytics` de `Packages/manifest.json` y poner `usePlayerLog` en 0.
      Aparte: `UnityConnectSettings.asset` apareció con `m_Enabled: 0 → 1` (analítica
      **encendida**); se revirtió a 0 y **al reabrir el Editor volvió a 1** — revertir el archivo
      no arregla nada mientras el módulo siga en el manifiesto.
- [ ] Revisado con el usuario

**Código de Fase 1 completo (T05–T08). Del Checkpoint B queda: el clic que confirma RNF-14 al
reabrir, la decisión sobre los dos residuos de `LocalLow` (RNF-11/RNF-08) y la revisión con el
usuario.**

---

## Fase 2 — Andamiaje mínimo (`andamiaje`)

- [x] **T09 · `NarrativeSequence` y `DialogueRunner`** — `M` · `EM` (07/09/2026)
      RF-05, RF-06, RF-10, RNF-01, RNF-18, HU-02, INC-28 · depende de: T07
      `DialogueLine`, `NarrativeSequence` (SO con `Id`, `Level`, ilustración y líneas),
      `DialogueRunner` en C# plano y `NarrativeVisitPolicy`. **EditMode 44/44** (10 nuevas: 7 del
      runner, 3 sobre el contenido de los assets). **Cambio de diseño frente al plan:** «escena ya
      vista» (INC-28) **no** es un campo nuevo del perfil — RNF-09/INC-27 cierran lo persistido en
      «nombre, nivel alcanzado, fases confirmadas y los cuatro indicadores, nada más», así que se
      **deriva del progreso**: si el perfil confirmó alguna fase del nivel, ya pasó por sus escenas.
- [x] **T10 · Escena `Narrative` parametrizada + tres secuencias del N1** — `M` · `PM` (07/09/2026)
      RF-05, RF-06, RNF-06, guion §3.1/§4.1/§4.2 · depende de: T09
      `NarrativeSceneController` **sin un solo `if` por secuencia**: resuelve por id contra la lista
      de assets. Escena `Narrative.unity` en Build Settings (5 escenas) y los tres assets del N1 con
      el texto del guion reescrito para cumplir RNF-01. `GameState.Narrative` mapeado en
      `GameFlowRunner.Scenes`; `Game.UI` pasa a referenciar `Game.Scaffolding` (el test de
      arquitectura solo veta Core→UI y nivel→nivel). Recorrido conducido a mano en el juego real:
      perfil nuevo → `Narrative` → 7 líneas → `LevelSelect` → nivel → `Narrative`.
      **PlayMode 31/33** (6 nuevas): los 2 fallos son los `VisualVerification` de la Fase 1, que no
      corren en batchmode y están medidos en 0/2 sobre el código sin cambios.
- [x] **T10b · Play arranca siempre en `Boot`** — `XS` · sin prueba automática (07/09/2026)
      RNF-13 · depende de: T10
      `Assets/Game/Scripts/Editor/PlayFromBoot.cs` fija `playModeStartScene`, función nativa de
      Unity: **solo afecta al Editor**, nunca al ejecutable. Verificado a mano — con `Narrative`
      abierta, Play aterrizó en `MainMenu` pasando por `Boot`. Cierra la confusión que originó T08b.
      **Colgó la suite PlayMode entera** (1500 s sin informe): secuestraba también la entrada a Play
      del Test Framework, que esperaba su `InitTestScene`. Arreglado con dos guardas —batchmode y
      callbacks de `TestRunnerApi`—; nuevo assembly `Game.EditorTools`. Detalle en
      `Fase-2-Resultados.md` §4.3.
- [x] **T11 · `HintPolicy`: ayuda a demanda + pista tras tres fallos** — `M` · `EM` (08/09/2026)
      RF-13, RNF-03, CP-06, HU-03, HU-04, guion §4.3.6 · depende de: T09
      `GuideStep` + `GuideContent` (un SO por nivel) + `HintPolicy` en C# plano. **Los dos
      mecanismos de RF-13 son distintos a propósito**: `RequestHelp()` repite la instrucción
      vigente y **no toca ningún contador** (CP-06, HU-03 FA-02) — si contara como intento, usar la
      ayuda adelantaría la pista; `RegisterFailedAttempt()` devuelve la pista al tercer fallo
      consecutivo y reinicia la cuenta (guion §4.3.5 E5). Un acierto la reinicia, y cambiar de tarea
      también: los tres fallos son «en una misma tarea», y solo hay una activa a la vez (RNF-03).
      Asset `Assets/Game/Data/Guide/N1_Guia.asset` con las dos tareas del N1 (Golpear, Soplar);
      la pista es la del guion §4.3.6 y **no nombra «Muy cerca»**, verificado sobre el asset y no
      sobre el código. **EditMode 57/57** (7 nuevas; la base eran 50, no las 44 del corte de la
      Fase 2 — el commit `145a63e` añadió las de RF-47). **PlayMode 33/33 + 2 omitidas**, sin
      regresión.

### ✅ Checkpoint C — Andamiaje
- [x] Las tres escenas narrativas se recorren completas — `NarrativeScene_RF05_*` verde en
      **PlayMode 31/33**: las tres resuelven en la misma escena y avanzar hasta el final sale a
      otra pantalla. `N1_Apertura` recorrida además a mano (7 líneas → `LevelSelect`)
- [~] El botón de omitir aparece **solo** en la segunda visita (INC-28) — la regla está probada en
      EditMode (`DialogueRunner_RF06_*`, `_INC28_*`) y la escena la respeta; falta verla en la
      segunda visita real, que exige una fase confirmada y por tanto **T17**
- [x] Ninguna pista resuelve la tarea ni nombra «Muy cerca» (CP-06) —
      `HintPolicy_CP06_LaPistaNuncaNombraLaPosicionEfectiva` verde sobre el asset del N1 (08/09/2026)
- [ ] Revisado con el usuario

**Código de Fase 2 completo (T09–T11), EditMode 57/57 · PlayMode 33/33 + 2 omitidas.** Del
Checkpoint C quedan dos casillas y ninguna es de código: ver el botón de omitir en la segunda visita
exige una fase confirmada (**T17**) y la revisión con el usuario.

---

## Fase 3 — Nivel fuego (`nivel-fuego`)

- [x] **T12 · `FireLevelConfig`, `StrikePosition`, `FireAttempt`** — `M` · `EM` (10/09/2026)
      RF-15, RF-16, RF-18, RF-19, CP-02, CT-05, RNF-18, HU-06, HU-07, CU-04, INC-32 · depende de: T11
      Lógica pura del Nivel 1 en C# plano, sin escena (assembly `Game.Levels.Fire`, antes vacío).
      `StrikePosition` (Lejos/Cerca/Muy cerca), `FireLevelConfig` (SO con los 4 parámetros del guion
      §4.3.2, valores por defecto 3 / Muy cerca / 3 / 3), `StrikeOutcome` (struct, `SparksDied` /
      `SparkLanded` del SPEC §Estilo) y `FireAttempt` (código del SPEC §Estilo tal cual, más
      `ConsecutiveFailures` público). `Strike` es la única vía de mutación: el deslizante es estado
      de UI (RF-15). `CanBlow` deriva solo de golpes efectivos → nunca vuelve a falso (INC-32,
      comentario «por qué no»); sin tope de intentos ni derrota (RF-18, CP-02). **EditMode 9/9** de
      la suite Fire (flujo test-first: RED 8 fail / 2 pass — las 2 verdes son un SO de datos y un
      invariante de estado inicial, sin lógica de Step 3 — → GREEN 9/9 tras fusionar una prueba de
      RF-19 con la de RNF-18). **EditMode total 72/72, sin regresión.** MCP de Rider operativo esta
      sesión: pruebas contra el Editor abierto. Sin `.asmdef` ni `.asset` nuevos (el
      `FireLevelConfig.asset` real es de T14). Detalle en `Fase-3-Resultados.md`.
- [x] **T13 · `FireFeedbackLog`, mensajes sin repetición** — `M` · `EM` (10/09/2026)
      RF-11, RF-17, RF-18, CP-03, HU-05, HU-06, guion §4.3.4 · depende de: T12
      El área de registro del Nivel 1, C# plano. `FireMessages` (SO) con los **ocho mensajes del
      guion §4.3.4 literales**, `[field: SerializeField, TextArea]` + `[Tooltip]`; asset real
      `Assets/Game/Data/Fire/N1_Mensajes.asset` (YAML a mano, convención `N1_*`). `FireFeedbackLog`
      traduce un `StrikeOutcome` a mensaje: por ordinal de golpe efectivo (primero/segundo/final al
      alcanzar el mínimo) o por distancia + racha (variante «tras dos intentos» desde el segundo
      fallo seguido). **No repite el mismo mensaje dos veces seguidas** cuando hay alternativa
      aplicable (RF-18): los golpes seguidos en la misma distancia alternan. `Entries` acumula todo
      el historial en orden (HU-06); sin tope (RF-18) — el scroll es de T14. `RecordBlowSuccess()`
      para T15. **EditMode suite Fire 20/20** (11 nuevas; flujo test-first: 9 rojas → verdes, las 2
      pruebas Editor sobre el asset nacen verdes como en T12). **EditMode total 83/83, sin
      regresión.** Desviación anotada: prueba de no-repetición nombrada con RF-18, no RF-17.
      Detalle en `Fase-3-Resultados.md`.
- [x] **T14 · Panel de encendido y escena `Level1_Cave`** — `M` · `PM` (10/09/2026)
      RF-14, RF-15, RF-17, RNF-02, RNF-03, RNF-19, RNF-20, CT-06, HU-06, CU-04, INC-41 · depende de: T13
      **Primera escena jugable del proyecto.** `Level1_Cave.unity` (Build Settings → 6 escenas):
      deslizante de 3 posiciones (`Navigation.None`), «Golpear», «Soplar» atenuado con badge
      **candado + «Aún no»** (RNF-19, verificado en captura VV), área de registro con `ScrollRect`,
      montón de hojas (placeholder). `FirePanelController` (`Game.Levels.Fire`, +`UnityEngine.UI`
      al asmdef) es adaptador puro: clic → `FireAttempt`/`FireFeedbackLog` de T12/T13, cero reglas.
      `FeedbackLogView` renderiza `Entries` (desviación del plan: va en `Game.Levels.Fire`, no
      `Game.UI`). **RNF-02:** asset propio `ControlesJugables.inputactions` con mapa **solo
      puntero** (sin teclado ni gamepad); el test barre `actionsAsset.bindings`. `GameFlowRunner`
      mapea `Playing → Level1_Cave`; `NarrativeSceneController.Leave()` entra al nivel de la
      secuencia (cierra el «PROVISIONAL (T14)»). **PlayMode: FirePanelTests 9/9; regresión EditMode
      83/83, PlayMode 44/44.** `N1_Config.asset` creado.
      **T14b (incluido):** `PlayFromBoot.cs` — la suite PlayMode se colgaba al lanzarla desde el
      MCP de Rider (`playModeStartScene` cargaba Boot, el juego arrancaba solo). El Test Framework
      `@1405238725ab` no gestiona `playModeStartScene`; nueva guarda por reflexión sobre
      `PlaymodeLauncher.IsRunning` en `ExitingEditMode`. Detalle en `Fase-3-Resultados.md`.
- [x] **T15 · Convergencia: «Soplar» → nacimiento del fuego** — `S` · `PM` + `VV` (11/09/2026)
      RF-19, RF-20, RF-04, RF-03, RNF-21, HU-07, CU-04 · depende de: T14
      «Soplar» encadena la convergencia: un barrido de color monótono (~0,6 s, sin oscilación —
      RNF-21) y al terminar `FirePanelController.CompleteLevel()` confirma la fase 1 del Nivel 1
      (`ConfirmPhase`, indicadores en `default` — los reales son T17), desbloquea el Nivel 2
      (`LevelUnlockPolicy`), guarda (`IProfileSaver`, override de prueba como en
      `MainMenuController`) y entra a `N1_NacimientoDelFuego` (guion §4.4, 19 líneas, cierre
      reflexivo con «se llama iterar» incluido). **Desviación:** sin `FireResolutionController`
      nuevo — la lógica se sumó a `FirePanelController` para no arriesgar otra pasada de
      `edit-scene`; T15 no toca `Level1_Cave.unity`. Nuevo `NarrativeOutcome`
      (`EntersLevel`/`ReturnsToLevelSelect`) en `NarrativeSequence`: `NarrativeSceneController.Leave()`
      ya no asume que toda narrativa entra al nivel — decide por el propósito declarado en el
      asset, sin un `if` por secuencia. **`FirePanelTests` 13/13** (4 nuevas) · **EditMode 83/83 ·
      PlayMode 48/48** (3 corridas limpias). De paso, arreglado un defecto real de aislamiento en
      el helper `LoadPanel()` de T14 (`FindAnyObjectByType` podía devolver el controlador de la
      prueba anterior en corridas de suite completa). Detalle en `Fase-3-Resultados.md`.
- [x] **T16 · Menú de pausa** — `M` · `EM` + `PM` (11/09/2026)
      RF-07, RF-03, RF-04, CP-02, HU-17 (FA-01..FA-05), INC-25 · depende de: T15
      Capa de UI sobre `Playing`, no un estado nuevo: «Pausa» abre un overlay con Continuar
      (restituye el estado exacto, no toca nada), Reiniciar (confirmación de una frase, Cancelar
      no cambia nada) y Volver al menú. **Hallazgo de diseño:** `GameFlowRunner.Apply` solo
      recargaba la escena si el nombre cambiaba — reentrar a `Playing` (RF-07, ya legal en
      `GameFlow` desde T03) no recargaba nunca. Arreglado: reentrar a `Playing` siempre recarga.
      `PauseMenuPolicy.Restart(GameFlowRunner)` (`Game.Core`) repite `StartPlaying` con el mismo
      nivel/fase — nunca re-bloquea ni borra indicadores (`Reach`/`ConfirmPhase` no se llaman).
      `PauseMenuController` (`Game.UI`, no `Game.Levels.Fire`: navegación general, sin dependencia
      nueva para el nivel) vive en `Level1_Cave.unity` sobre el panel de T14, sin tocarlo — un
      overlay bloquea el raycast hacia «Golpear»/«Soplar». **Dos bugs reales encontrados en
      verificación** (detalle en `Fase-3-Resultados.md`): el componente en un GameObject que
      arrancaba inactivo (Awake/Start nunca corrían), y `PauseMenuPolicy` llamando a `GameFlow`
      directo en vez de por `GameFlowRunner` (saltaba la recarga). **PauseMenuTests 4/4,
      PauseMenuPolicyTests 1/1** · **EditMode 84/84 · PlayMode 52/52** (2 corridas limpias).
- [x] **T17 · Emisión de los cuatro indicadores del N1** — `M` · `EM`
      RF-45, RF-04, RNF-14, CP-03, CP-09, OE1 §3.6.1, INC-29 · depende de: T15, T16 (11/09/2026).
      `ILevelReporter` (Core, dos métodos: `PauseOpened`/`PauseClosed`) + `FireIndicatorCollector`
      (Fire, implementa la interfaz): Intentos = golpes no efectivos, Errores corregidos = acierto
      justo tras un fallo, Pasos utilizados = golpes efectivos al cruzar el mínimo (se congela),
      Tiempo de resolución = reloj inyectado menos la ventana de pausa. Mediación por
      `GameFlowRunner.ActiveReporter` para que `Game.UI` (quien pausa) y `Game.Levels.Fire` (quien
      mide) no se referencien entre sí. `FirePanelController.CompleteLevel()` ya no usa `default`.
      **FireIndicatorTests 6/6** (incluye barrido de reflexión CP-03 sobre `Game.UI` cargado en
      dominio, sin referencia de compilación nueva) · **EditMode 90/90 · PlayMode 52/52** (una
      corrida PlayMode completa tuvo 1 fallo intermitente ajeno a T17, limpio en la repetición).
- [x] **T18 · Resumen de fin de nivel y cierre reflexivo** — `M` · `EM` + `PM`
      RF-45, RF-12, RF-17, RF-03, CP-03, CP-07, HU-14, INC-26 · depende de: T17 (11/09/2026).
      `GameState.LevelSummary` (ya declarado desde T03) por fin tiene escena y controlador. HU-14
      gobernó el diseño sobre el plan: el guía nombra la habilidad en `N1_NacimientoDelFuego`
      (contenido ya escrito desde antes de T15, sin tocar) y **después** el resumen sin cifras
      confirma la fase, desbloquea el Nivel 2 y guarda — no `FirePanelController.CompleteLevel()`
      como antes. **Bug real encontrado**: confirmar la fase antes de la narrativa de cierre hacía
      que `NarrativeVisitPolicy.AlreadySeen` diera verdadero desde la primerísima vez, así que el
      botón de omitir aparecía siempre, violando CP-07. Arreglado moviendo el efecto secundario al
      punto donde HU-14 ya lo sitúa (paso 6-7), mediado por `GameFlowRunner.PendingIndicators`
      (mismo patrón que `ActiveReporter`, T17). `FireLevel_RF04_.../FireLevel_RF03_...` (T15) se
      trasladaron a `LevelSummaryTests`, donde el efecto ahora vive de verdad.
      **LevelSummaryComposerTests 4/4 · LevelSummaryTests 2/2** (incluye
      `LevelSummary_CP07_ElCierreReflexivoNoEsOmitibleLaPrimeraVez`, la prueba de regresión directa
      del bug) · **EditMode 94/94 · PlayMode 52/52** (2 corridas limpias consecutivas).
- [x] **T19 · Iluminación progresiva del escenario** — `S` · `VV`
      **RF-21 (prioridad Baja)**, RNF-20, RNF-21, HU-07 · depende de: T15 (11/09/2026).
      `CaveLightingController` (nuevo, sobre `Panel/Fondo`) interpola el color de fondo entre un
      tono oscuro y el original según `golpesEfectivos / (mínimo + 1)` — un escalón por golpe
      efectivo (guion E4), reservando el último escalón para la ignición (`PlayIgnitionAsync`,
      E7), que ahora también anima la iluminación en el mismo barrido que el color del fuego.
      **Desviación deliberada del documento de arte**: el `#0F1526` al 65 % que sugiere
      `Direccion_de_Arte.md` §8.1 da ~1.2:1 de contraste contra el texto del panel — muy por
      debajo del 4.5:1 de RNF-20. Se ajustó a `#8A97AB` (≈5.14:1, verificado por la propia
      prueba, no de confianza). **`CaveLightingTests` 2/2** (incluye
      `FirePanel_RNF20_ContrasteSuficienteEnElEstadoMasOscuro`, cálculo real de contraste WCAG) ·
      **EditMode 94/94 · PlayMode 54/54** (2 corridas limpias; la flakiness ya conocida de
      `FirePanelTests`, §2c de `Fase-3-Resultados.md`, apareció con más frecuencia esta vez,
      siempre en pruebas preexistentes ajenas a T19 — anotado como pendiente de revisar).

**Código de Fase 3 completo (T12–T19).** Del Checkpoint D no queda ninguna casilla de código —
todas las que siguen son verificación manual y revisión con el usuario.

### ✅ Checkpoint D — Slice 1 completo
- [ ] **Dos recorridos completos** del Golden Path sin incidencias (RNF-13)
- [ ] Cierre forzado a mitad de nivel → retoma desde la última fase confirmada (RNF-14)
- [ ] Carga de escena < 10 s y memoria < 2 GB, **medidas** en el equipo de referencia (RNF-04, RNF-05)
- [ ] Ejecución portable con el adaptador de red deshabilitado (RNF-07, RNF-08)
- [ ] Todo RF del slice tiene al menos una prueba que lo nombra (CT-10)
- [ ] Revisado con el usuario antes de abrir el Slice 2

---

## Fase 5 — Rediseño de la mecánica del Nivel 1 (abierta el 12/09/2026, pedido de Santiago)

> ⚠️ **Se aparta del guion §4.3 y de RF-15/RF-16 tal como están radicados** (deslizante de
> *posición* de tres muescas, mensajes por distancia). Santiago lo reafirmó el 12/09/2026: «ese
> slider mide fuerza, no distancia». Queda como **INC-47** pendiente de registrar en
> `INCONSISTENCIAS.md` y de corregir en el guion/OE1 cuando se cierre la fase. Los nombres de
> las pruebas conservan RF-15/RF-16 porque el requisito (hipótesis → experimento → resultado
> observable) es el mismo; cambia el parámetro de la hipótesis.

**Supuestos que hay que confirmar** (valores en `N1_Config.asset`, se ajustan sin código):
fuerza efectiva 7–8 de 10 («supongamos un 7 o 8») · una piedra «cerca» = a menos de un radio
del montón · las hojas están «amontonadas» cuando todas caen dentro de ese radio · soplar sin
montón no penaliza: solo escribe en el registro lo que pasó (CP-02).

- [x] **T20 · Narrativa del Nivel 1 fiel al guion** — `S` · `EM` + `PM` (12/09/2026)
      guion §3.1, §4.1, §4.2, §4.4, INC-44 (el guía es **Algoritm**, no «Chispa») · depende de: nada
      Reescribir las líneas de los cuatro `N1_*.asset` con el texto completo del guion (faltan
      frases: «Son frías y pesadas», «Pero todos lo sienten», «con las manos temblorosas», «Antes
      de que alguien pueda responder»…), máximo dos líneas y doce palabras por línea (§11.4), y
      **remapear las paradas de cámara** (`Camara_Narrativa_N1.md` §4) a los índices nuevos.
      `N1_NacimientoDelFuego` dice `CHISPA`: pasa a `ALGORITM`.
- [x] **T21 · Deslizante de fuerza de diez posiciones** — `M` · `EM` + `PM` `MCP` (12/09/2026)
      RF-15, RF-16, RF-17, RNF-18, CT-05, mockup 7 · depende de: nada
      **Hecho:** `ForceBand`, `FireAttempt.Strike(int)`/`Classify`, `FireLevelConfig` (10 · 7–8),
      `FireMessages` con cuatro mensajes de fallo por fuerza, `ForceSliderFeedback`, escena con
      `FuerzaSlider` 0–10 y etiquetas del mockup; `N1_Guia` reescrita sin nombrar la fuerza.
      **EditMode 167/167 · PlayMode 92/92 (6 VV omitidas en batchmode)**, corridas por CLI.
      Pendiente de ver a ojo: el asa que crece y enrojece.
      `FireLevelConfig`: `ForceLevels` (10), `EffectiveForceMin`/`Max` (7–8). `FireAttempt.Strike(int
      force)` clasifica en `ForceBand` (suave / efectivo / fuerte); `StrikeOutcome` recuerda la
      fuerza. `FireMessages` cambia los cuatro mensajes de fallo (suave ×2, fuerte ×2), sin cifras.
      UI: deslizante 0–10 (rango tomado de la configuración), etiquetas «Fuerte / Fuerza del golpe
      / Suave», y el asa **crece y enrojece** con la fuerza (`ForceSliderFeedback`). La pista de
      `N1_Guia` deja de hablar de distancia y no nombra la fuerza correcta (CP-06).
- [x] **T22 · Hojas, sílex y pedernal arrastrables** — `L` · `EM` + `PM` `MCP` (12/09/2026)
      CT-06 (clic sostenido), RNF-02, mockup 7 · depende de: T21
      Sobre la cenital aparecen regados N hojas, el sílex y el pedernal (placeholders hasta A7/A8).
      Arrastre con puntero (Input System). `FireLayout` (C# plano): distancia de cada pieza al
      centro del montón → `LeavesPiled` (todas las hojas dentro del radio) y `StonesNear` (las dos
      piedras dentro del radio). Un golpe es efectivo solo con **fuerza efectiva y piedras cerca**;
      con las piedras lejos el registro lo dice sin penalizar.
- [x] **T23 · Soplar condicionado y limpieza de la interfaz** — `M` · `PM` `MCP` (12/09/2026)
      RF-19, RF-20, CP-02, RNF-19 · depende de: T22
      «Soplar» se habilita al converger (como hoy) pero el fuego solo nace si `LeavesPiled`; si
      no, mensaje observacional en el registro y el nivel sigue. Se retiran del `Level1_Cave` el
      marco «Hoguera», el `MontonHojas` placeholder y la etiqueta de instrucción fija (la
      instrucción vive en «Pista»); la prueba `RNF03` que exige una `InstruccionLabel` se ajusta.
- [~] **T24 · Documentos y verificación** — `S` (12/09/2026: todo menos la revisión con el usuario)
      `Interfaces.md` §7, `Camara_Narrativa_N1.md` §3, `INCONSISTENCIAS.md` (INC-47), suites
      completas por CLI y captura del panel en Play revisada con el usuario.
      **Hecho el 12/09/2026 salvo la revisión con el usuario.** Santiago retiró además el registro
      con historial («estorba»): la tablilla superior muestra el último mensaje (`FeedbackLogView`).
      T21 ajustado: asa azul→rojo lineal por muesca, escala 0,8–1,2.

### Checkpoint E — mecánica nueva del Nivel 1
- [x] El nivel se juega entero con la mecánica nueva: arrastrar → elegir fuerza → golpear → soplar — **EditMode 173/173 · PlayMode 95/95 (6 VV omitidas)** por CLI, 12/09/2026
- [x] Ningún fallo penaliza ni bloquea (CP-02); ninguna pista nombra la fuerza ni la distancia correctas (CP-06) — `FirePanel_RF19_SoplarConLasHojasRegadas…`, `FirePanel_RF13_…` (12/09/2026)
- [ ] Revisado con el usuario

---

## Assets visuales — `plan.md` §Assets visuales del Slice 1

Personajes = **obra derivada** de los diseños Anonaky con **autorización escrita concedida**
(PG-07 cerrado): su reconocimiento en créditos es obligatorio. Entornos, props e interfaz son
originales del proyecto (CT-09, RNF-23).
Cada asset generado se registra en `CreditsContent.asset` (T08).

**Cinco bloques fijos por prompt**, copiados palabra por palabra antes de la descripción:
`[1 CONTEXTO] [2 ESTILO] [3 PALETA] [4 ENTREGA] [5 PROHIBICIONES]`. Un asset generado sin los
cinco se descarta y se vuelve a pedir. La paleta y las especificaciones salen de
`claudeDocs/Direccion_de_Arte.md`.

**Los personajes se piden en A-pose**, no en poses de acción: las poses del nivel se producen
animando el sprite con 2D Animation (`Direccion_de_Arte.md` §7.5 y §13.1). Pedirle a Gemini tres
poses del mismo personaje devuelve tres personajes distintos.

- [ ] **A1 · Chispa, el guía** — chroma sí — guion §1.1/§4.1, PG-02, RF-10, RF-12, RF-13
- [ ] **A2 · Papá (jugable N1)** — chroma sí — **A-pose** — guion §1.1/§4.2, RF-14, HU-06, CN-02
- [ ] **A3 · Mamá** — chroma sí — **A-pose** — guion §1.1/§4.2
- [ ] **A4 · Niña** — chroma sí — **A-pose**, penacho alto — guion §1.1/§4.2
- [ ] **A5 · Niño** — chroma sí — **A-pose**, copete hacia adelante — guion §1.1/§4.2
- [ ] **A6 · Cueva, cuatro escalones de luz** — chroma **no** — guion §3.1/§4, RF-21
- [ ] **A7 · Montón de hojas, cuatro estados** — chroma sí — guion §4.3.1/§4.3.3, RF-14, RF-16
- [ ] **A8 · Sílex y pedernal** — chroma sí — guion §4.1/§4.2, RF-16, RNF-19
- [ ] **A9 · Controles del panel de encendido** — chroma sí — RF-14, RF-15, RF-19, RNF-19
- [ ] **A10 · Marco de diálogo del guía** — chroma sí — RF-05, RF-06, RNF-20
- [ ] Cada asset pasa la **checklist de `Direccion_de_Arte.md` §17** y su línea «Verificación»
- [ ] Postproceso: recorte del verde, alfa, halo, nombre según §15.4, import con los ajustes de
      §15.2 (**PPU 100**, pivot `Bottom` en personajes y `Center` en props e interfaz)
- [ ] Verificar RNF-20 y RNF-19 sobre el arte final, no sobre el prompt

---

## Bloqueantes y decisiones pendientes

- [~] **R1 · Servidor MCP para pruebas.** Rider 2026.2 + plugin «MCP Server Extension for Unity»
      (id 30357) instalados el 06/09; MCP `rider` registrado. **Sigue abierto:** en la sesión del
      08/09 el MCP `rider` arrancó con `ConnectionRefused` pese a tener el Editor de Unity abierto
      — el puente lo sirve **Rider**, no Unity, así que con Rider cerrado no hay a quién preguntar.
      Entretanto `unity test` (CLI, Editor cerrado) cubre el flujo test-first: es lo que se usó en
      T04b y en T11.
- [x] **PG-01** · título provisional para `GameTitleConfig` (T05) = **«Algoritm»** (confirmado por
      el usuario el 06/09/2026). Marcador; se cambia editando el asset sin recompilar.
- [ ] **T08** · confirmar que los créditos entran en el Slice 1 (RF-01 pone el botón en el inicio).
- [ ] **PG-06** · validar jugando los valores de `FireLevelConfig` en el Checkpoint D.
