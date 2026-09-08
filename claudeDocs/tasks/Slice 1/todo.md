# Tablero — Slice 1: Golden Path temprano

Plan técnico: [`plan.md`](plan.md). Contrato: `claudeDocs/SPEC.md`.
Resultados: [`Fase-0-Resultados.md`](Fase-0-Resultados.md) · [`Fase-1-Resultados.md`](Fase-1-Resultados.md) ·
[`Fase-2-Resultados.md`](Fase-2-Resultados.md) (en curso).
Cada tarea se cierra con su commit asociado (RNF-17, CT-11).

**Leyenda:** `EM` = EditMode (lógica pura, sin escena) · `PM` = PlayMode (integración) ·
`VV` = VisualVerification.

> ⚠️ **R1 casi cerrado (06/09/2026).** Rider 2026.2 instalado; el MCP `rider` quedó registrado.
> Falta el plugin «MCP Server Extension for Unity» (id 30357) para tener `mcp__rider__run_unity_tests`,
> y reiniciar Claude Code para que la sesión lo vea. Mientras tanto las pruebas van por
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
- [ ] Cerrar y reabrir conserva el perfil y su progreso (RNF-14, manual sobre el ejecutable)
- [ ] `Datos/` aparece junto al ejecutable, sin residuos fuera de ella (RNF-07, manual)
- [ ] Revisado con el usuario

**Código de Fase 1 completo (T05–T08), 57/57 pruebas verde. Falta cerrar el Checkpoint B: las dos
comprobaciones manuales sobre el ejecutable + la revisión con el usuario.**

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
- [ ] **T11 · `HintPolicy`: ayuda a demanda + pista tras tres fallos** — `M` · `EM`
      RF-13, RNF-03, CP-06, HU-03, HU-04, guion §4.3.6 · depende de: T09

### ✅ Checkpoint C — Andamiaje
- [x] Las tres escenas narrativas se recorren completas — `NarrativeScene_RF05_*` verde en
      **PlayMode 31/33**: las tres resuelven en la misma escena y avanzar hasta el final sale a
      otra pantalla. `N1_Apertura` recorrida además a mano (7 líneas → `LevelSelect`)
- [~] El botón de omitir aparece **solo** en la segunda visita (INC-28) — la regla está probada en
      EditMode (`DialogueRunner_RF06_*`, `_INC28_*`) y la escena la respeta; falta verla en la
      segunda visita real, que exige una fase confirmada y por tanto **T17**
- [ ] Ninguna pista resuelve la tarea ni nombra «Muy cerca» (CP-06) — **T11, sin empezar**
- [ ] Revisado con el usuario

**Fase 2 a medias: T09 y T10 cerradas, T11 sin empezar.** El Checkpoint C no se puede cerrar sin
T11.

---

## Fase 3 — Nivel fuego (`nivel-fuego`)

- [ ] **T12 · `FireLevelConfig`, `StrikePosition`, `FireAttempt`** — `M` · `EM`
      RF-15, RF-16, RF-18, RF-19, CP-02, CT-05, RNF-18, HU-06, HU-07, CU-04, INC-32 · depende de: T11
- [ ] **T13 · `FireFeedbackLog`, mensajes sin repetición** — `M` · `EM`
      RF-11, RF-17, RF-18, CP-03, HU-05, HU-06, guion §4.3.4 · depende de: T12
- [ ] **T14 · Panel de encendido y escena `Level1_Cave`** — `M` · `PM`
      RF-14, RF-15, RF-17, RNF-02, RNF-03, RNF-19, CT-06, HU-06, CU-04, INC-41 · depende de: T13
- [ ] **T15 · Convergencia: «Soplar» → nacimiento del fuego** — `S` · `PM` + `VV`
      RF-19, RF-20, RF-04, RF-03, RNF-21, HU-07, CU-04 · depende de: T14
- [ ] **T16 · Menú de pausa** — `M` · `EM` + `PM`
      RF-07, RF-03, RF-04, CP-02, HU-17 (FA-01..FA-05), INC-25 · depende de: T15
- [ ] **T17 · Emisión de los cuatro indicadores del N1** — `M` · `EM`
      RF-45, RF-04, RNF-14, CP-03, CP-09, OE1 §3.6.1, INC-29 · depende de: T15, T16
- [ ] **T18 · Resumen de fin de nivel y cierre reflexivo** — `M` · `EM` + `PM`
      RF-45, RF-12, RF-17, RF-03, CP-03, CP-07, HU-14, INC-26 · depende de: T17
- [ ] **T19 · Iluminación progresiva del escenario** — `S` · `VV`
      **RF-21 (prioridad Baja)**, RNF-20, RNF-21, HU-07 · depende de: T15

### ✅ Checkpoint D — Slice 1 completo
- [ ] **Dos recorridos completos** del Golden Path sin incidencias (RNF-13)
- [ ] Cierre forzado a mitad de nivel → retoma desde la última fase confirmada (RNF-14)
- [ ] Carga de escena < 10 s y memoria < 2 GB, **medidas** en el equipo de referencia (RNF-04, RNF-05)
- [ ] Ejecución portable con el adaptador de red deshabilitado (RNF-07, RNF-08)
- [ ] Todo RF del slice tiene al menos una prueba que lo nombra (CT-10)
- [ ] Revisado con el usuario antes de abrir el Slice 2

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
      (id 30357) instalados el 06/09; MCP `rider` registrado. Falta **reiniciar Claude Code** para
      que la sesión vea `mcp__rider__run_unity_tests` (correr contra el Editor abierto). Entretanto,
      `unity test` (CLI, Editor cerrado) cubre el flujo test-first — usado en T04b sin fricción.
- [x] **PG-01** · título provisional para `GameTitleConfig` (T05) = **«Algoritm»** (confirmado por
      el usuario el 06/09/2026). Marcador; se cambia editando el asset sin recompilar.
- [ ] **T08** · confirmar que los créditos entran en el Slice 1 (RF-01 pone el botón en el inicio).
- [ ] **PG-06** · validar jugando los valores de `FireLevelConfig` en el Checkpoint D.
