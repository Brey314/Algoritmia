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
- [x] Cerrar y reabrir conserva el perfil y su progreso (RNF-14, manual sobre el ejecutable) —
      **build portable hecho el 08/09** (`unity build`, 137 MB, 5 escenas). Se creó un perfil
      jugando y quedó en `Build/Algoritmia/Datos/yo.json`; el archivo sobrevive al cierre. Falta
      **ver el perfil listado al reabrir**, que es un clic en «Jugar» y lo hace el usuario.
      — cerrado el 01/10/2026: sobre el ejecutable rc1, el perfil `OE4N1b`, creado jugando el
      Nivel 1, salió con «Salir» y al relanzar y pulsar «Jugar» apareció en la lista y abrió con
      el mismo progreso —N1 «Completado», N2 habilitado, N3 bloqueado— (S-PER, paso 22,
      `claudeDocs/tasks/OE4/evidencias/S-PER/S1-cerrar-reabrir_perfil_listado.png` y
      `S1-cerrar-reabrir_progreso_conservado.png`); sobre rc2 lo repite PF-RF02-02, `OE4_Z` abre
      con su progreso exacto (`claudeDocs/tasks/OE4/evidencias/rc2/PF-RF02-02_OE4_Z_progreso.png`;
      `claudeDocs/tasks/OE4/OE4-Resultados.md` §5 y §8.4).
- [x] `Datos/` aparece junto al ejecutable, sin residuos fuera de ella (RNF-07, manual) —
      **`Datos/` sí nace junto al `.exe` en la primera ejecución** (RNF-07 ✅). **Pero hay residuo
      fuera** (RNF-11 ❌): el reproductor escribe en
      `%AppData%\LocalLow\DefaultCompany\My project\` un `Player.log` (PlayerSettings
      `usePlayerLog: 1`) y archivos de **Unity Analytics / Insights**, que el módulo
      `com.unity.modules.unityanalytics` sigue escribiendo **aunque `UnityConnectSettings` esté en
      `m_Enabled: 0`**. Ningún dato del jugador vive ahí —el perfil solo está en `Datos/`—, pero
      son residuos fuera de la carpeta portable y, en el caso de la telemetría, algo que RNF-08
      prohíbe. **Dos decisiones pendientes del usuario:** quitar
      `com.unity.modules.unityanalytics` de `Packages/manifest.json` y poner `usePlayerLog` en 0.
      *(29/09/2026, decididas por Santiago: el manifiesto no se toca; la analítica se apaga en la
      configuración del proyecto —`submitAnalytics: 0`; `SENTIS_ANALYTICS_ENABLED` lo repone el paquete
      de inferencia y solo afecta al Editor— y una prueba
      de arquitectura lo vigila; `usePlayerLog` sigue en 1 porque las sesiones del OE4 revisan el
      `Player.log` (`Revisar-Log`).)*
      Aparte: `UnityConnectSettings.asset` apareció con `m_Enabled: 0 → 1` (analítica
      **encendida**); se revirtió a 0 y **al reabrir el Editor volvió a 1** — revertir el archivo
      no arregla nada mientras el módulo siga en el manifiesto.
      — cerrado el 01/10/2026: una copia de la carpeta entregable puesta en una ruta con espacio
      fuera del repositorio arrancó sin instalar nada ni elevar privilegios y creó `Datos/` junto
      a su ejecutable (PF-RNF07-01, `claudeDocs/tasks/OE4/evidencias/S-DOC/S-RNF07_portabilidad.txt`).
      Sobre rc2, `UnityServicesOff` (`Game.EditorTools`) apaga los servicios de Unity antes del
      build y comprueba que el juego compilado no nombre sus servidores: en LocalLow solo queda
      `Player.log`, sin archivos nuevos de Analytics ni de Insights; en
      `HKCU\Software\Universidad Catolica de Colombia\Algoritmia` quedan los valores de pantalla,
      un contador y un identificador de sesión del reproductor y tres `unity_connect.*`, ninguno
      con datos del estudiante (`claudeDocs/tasks/OE4/evidencias/rc2/S-INI-rc2_residuos.txt`,
      `GP3_residuos.txt`). Los dos valores de sesión y los tres `unity_connect.*` son DEF-RC2-01
      (Menor, resto de DEF-SPIKE-01), documentado
      como residuo del motor para la entrega (decisión D-RC2-01, opción b); poner
      `m_InitializeOnStartup` a 0 en `UnityConnectSettings` o borrar esas claves al salir queda como
      mejora recomendada para un build futuro, porque pide build, residuos y SUITE
      (`claudeDocs/tasks/OE4/OE4-Resultados.md` §4.6, §8.6 y §8.7).
- [x] Revisado con el usuario
      — revisado por Santiago el 30/09/2026 (acta D10, decisión D-a), tras la verificación de
      Claude del 01/10/2026: las dos casillas anteriores, cerradas sobre el ejecutable (S-PER
      paso 22 y PF-RNF07-01); el `Player.log` y las claves de pantalla quedaron decididos el 29/09
      (INC-97) y el resto del registro es DEF-RC2-01, documentado como residuo del motor
      (`claudeDocs/tasks/OE4/OE4-Resultados.md` §8.7).

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
- [x] El botón de omitir aparece **solo** en la segunda visita (INC-28) — la regla está probada en
      EditMode (`DialogueRunner_RF06_*`, `_INC28_*`) y la escena la respeta; falta verla en la
      segunda visita real, que exige una fase confirmada y por tanto **T17**
      *(01/10/2026: Implementación — la segunda visita real la comprueba
      `NarrativeScene_INC28_OfreceOmitirEnLaSegundaVisitaTrasTerminarElNivel1` (D10-5): con el
      perfil que deja el Nivel 1 terminado —su fase confirmada y el Nivel 2 abierto—, la escena
      `Narrative` ofrece «Omitir» en `N1_Apertura`, y
      `NarrativeScene_INC28_NoMuestraOmitirLaPrimeraVezQueSeVeLaEscena` comprueba que la primera vez
      no aparece. Las dos, en verde en la suite completa del 01/10/2026. Sobre el ejecutable,
      PF-RF06-01 en P sobre rc1
      (`claudeDocs/tasks/OE4/evidencias/GP1/PF-RF06-01_N1_segunda_visita_omitir_INC28.png`) y
      PF-RF06-02 y 03 sobre rc2 (`claudeDocs/tasks/OE4/OE4-Resultados.md` §5 y §8.4).)*
- [x] Ninguna pista resuelve la tarea ni nombra «Muy cerca» (CP-06) —
      `HintPolicy_CP06_LaPistaNuncaNombraLaPosicionEfectiva` verde sobre el asset del N1 (08/09/2026)
- [x] Revisado con el usuario
      *(01/10/2026: Decisión — revisado por Santiago el 30/09/2026 (acta D10, §5), tras comprobar en
      verde en la suite completa del 01/10/2026 las tres casillas del checkpoint: las narrativas del
      Nivel 1 (`NarrativeScene_RF05_ResuelveTresSecuenciasDistintasSinRamas`,
      `NarrativeScene_RF10_LaAperturaEncadenaLasTresEscenasDelNivel1YEntraAJugar`), «Omitir» solo en
      la segunda visita y `HintPolicy_CP06_LaPistaNuncaNombraLaPosicionEfectiva`.)*

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
- [x] **Dos recorridos completos** del Golden Path sin incidencias (RNF-13)
      — cerrado el 01/10/2026: tres recorridos completos sobre el ejecutable con el arnés
      `claudeDocs/tasks/OE4/herramientas/oe4.ps1` (acta D10, §5), de la pantalla de inicio a los
      créditos con perfil nuevo: GP1 (87 min, ventana 1920×1080) y GP2 (33 min, pantalla completa)
      sobre rc1, y GP3 (16 min, pantalla completa) sobre rc2. Los tres terminaron sin bloqueos,
      cierres inesperados ni estados irrecuperables, sin `Exception` en `Player.log` y con las
      siete fases en el JSON; los defectos no bloqueantes que vieron GP1 y GP2 (DEF-GP1-01..03 en el
      laberinto, DEF-SPIKE-01 en la red) se corrigieron en rc2 y se reverificaron
      (`claudeDocs/tasks/OE4/OE4-Resultados.md` §3, §8.3 y §8.5;
      `claudeDocs/tasks/OE4/evidencias/GP1/GP1_OE4GP1_final.json`, `GP2/GP2_OE4GP2_final.json`,
      `rc2/GP3_registro_de_pasos.md`). La duración de 20–40 min con un estudiante es H8 de
      `claudeDocs/tasks/OE4/Hoja-HUM.md` (Slice 3).
- [x] Cierre forzado a mitad de nivel → retoma desde la última fase confirmada (RNF-14)
      — cerrado el 01/10/2026: sobre rc1, siete cierres forzados (`Matar`), entre ellos uno a
      mitad de la mecánica del Nivel 1 —retoma la cueva desde cero con el perfil listado—
      (`claudeDocs/tasks/OE4/evidencias/S-PER/PF-RNF14-N1mec_*.png`) y otro durante
      `N1_NacimientoDelFuego`, que deja el nivel por completar porque su fase se guarda al mostrarse
      el resumen (PF-RNF14-03, INC-74); en los siete el perfil reabrió sin error en la primera fase
      pendiente, con lo confirmado y sus indicadores intactos
      (`claudeDocs/tasks/OE4/evidencias/S-PER/S-PER_registro_de_pasos.md`). Sobre rc2, dos cierres
      más en el instante del guardado: entre la escritura del temporal y el reemplazo queda el
      perfil anterior intacto y se retoma la fase sin confirmar; justo después del reemplazo queda
      el perfil nuevo completo; en ningún caso un JSON truncado
      (`claudeDocs/tasks/OE4/evidencias/rc2/PF-RNF14-guardado_matar1.txt` y `_matar2.txt`;
      `claudeDocs/tasks/OE4/OE4-Resultados.md` §5 y §8.3). La ventana interna de `File.Replace`,
      sin `<perfil>.json` en disco, no se alcanzó y queda como riesgo residual Menor (R-RC2-A, `claudeDocs/tasks/OE4/OE4-Resultados.md` §8.7).
- [x] Carga de escena < 10 s y memoria < 2 GB, **medidas** en el equipo de referencia (RNF-04, RNF-05)
      — cerrado el 01/10/2026: medidas sobre el ejecutable en el equipo 1 (Ryzen 5 7600X, 32 GB,
      RTX 5070 Ti, Windows 11), leyendo la línea `RNF-04: «X» cargó en N s` que `SceneLoader` deja en
      `Player.log`: peor carga 0,765 s en 188 cargas de rc1 y 0,780 s en 74 de rc2, las dos de
      `Narrative` al abrir `N1_Apertura`; `Level1_Cave` 0,156 s; memoria máxima de rc2 425,5 MiB de
      trabajo y 1 124,6 MiB privada (rc1: 435,7 y 1 140,1 MiB), < 2048 MiB
      (`claudeDocs/tasks/OE4/OE4-Resultados.md` §4.1, §4.2 y §8.6;
      `claudeDocs/tasks/OE4/evidencias/S-RNF/S-RNF_cargas.csv`, `rc2/GP3_mem.csv`). La columna
      del segundo equipo es la de P12 del Slice 4 (H3) y la del equipo sin tarjeta dedicada, CT-02 (H5).
- [ ] Ejecución portable con el adaptador de red deshabilitado (RNF-07, RNF-08)
      — sigue abierta el 01/10/2026 por la decisión D-b (acta D10, §5): la ejecuta Santiago con H3
      (la carpeta portable en un segundo equipo) y H4 (sin conexión) de
      `claudeDocs/tasks/OE4/Hoja-HUM.md`, y se marca con su plantilla. En el equipo 1, sobre rc2,
      ya hay 0 conexiones de red en 1312 muestras (PF-RNF10-01) y la copia portable de rc1 arrancó
      desde una ruta con espacio (PF-RNF07-01) (`claudeDocs/tasks/OE4/OE4-Resultados.md` §4.6 y §8.6).
- [x] Todo RF del slice tiene al menos una prueba que lo nombra (CT-10)
      *(01/10/2026: Implementación — lo exige
      `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra` (`9c34924`, 24/09), que deriva la
      matriz de los nombres de método y pide los 47 RF; en verde en la suite completa del
      01/10/2026. RF-01 a RF-21 y RF-45, los de este slice, tienen cada uno al menos una prueba.)*
- [x] Revisado con el usuario antes de abrir el Slice 2
      — revisado por Santiago el 30/09/2026 (acta D10, decisión D-a), tras la verificación de
      Claude del 01/10/2026: a posteriori, porque el Slice 2 corre en paralelo desde el 10/09; los
      recorridos, el cierre forzado y las cargas y la memoria se comprobaron sobre el ejecutable
      (PF-RNF13-01 con GP1, GP2 y GP3; PF-RNF14-01..05; PF-RNF04-01 y PF-RNF05-01 en el equipo 1,
      `claudeDocs/tasks/OE4/OE4-Resultados.md` §3–§5 y §8), y la suite de rc2 pasa entera (PlayMode
      376/376, EditMode 470 = 469 + 1 omitida, `claudeDocs/tasks/OE4/evidencias/suites/rc2-final/`).
      La ejecución sin red y en un segundo equipo queda con Santiago (H3 y H4).

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
- [x] **T24 · Documentos y verificación** — `S` (12/09/2026: todo menos la revisión con el usuario)
      `Interfaces.md` §7, `Camara_Narrativa_N1.md` §3, `INCONSISTENCIAS.md` (INC-47), suites
      completas por CLI y captura del panel en Play revisada con el usuario.
      **Hecho el 12/09/2026 salvo la revisión con el usuario.** Santiago retiró además el registro
      con historial («estorba»): la tablilla superior muestra el último mensaje (`FeedbackLogView`).
      T21 ajustado: asa azul→rojo lineal por muesca, escala 0,8–1,2.
      *(01/10/2026: Decisión — la revisión con el usuario se hizo: Santiago vio el panel en Play el
      12/09 y pidió quitar el historial, que se quitó; la Fase 6 (T25–T27, `863ef05`, 15/09)
      sustituyó ese panel, y los dos estudiantes aprobaron la mecánica que quedó el mismo 15/09
      (acta D06, §3 y §5). Revisado por Santiago el 30/09/2026 (acta D10, §5). INC-47 está cerrado
      desde el 29/09.)*

### Checkpoint E — mecánica nueva del Nivel 1
- [x] El nivel se juega entero con la mecánica nueva: arrastrar → elegir fuerza → golpear → soplar — **EditMode 173/173 · PlayMode 95/95 (6 VV omitidas)** por CLI, 12/09/2026
- [x] Ningún fallo penaliza ni bloquea (CP-02); ninguna pista nombra la fuerza ni la distancia correctas (CP-06) — `FirePanel_RF19_SoplarConLasHojasRegadas…`, `FirePanel_RF13_…` (12/09/2026)
- [x] Revisado con el usuario
      *(01/10/2026: Decisión — la Fase 6 sustituyó la mecánica de esta fase —T26 reemplazó la regla
      de «piedras cerca» de T22 y retiró el soplo condicionado de T23—; la que quedó la aprobaron
      los dos estudiantes el 15/09 (acta D06, §3 y §5) y su revisión es la del Checkpoint F.
      Revisado por Santiago el 30/09/2026 (acta D10, §5).)*

---

## Fase 6 — Reunir y encender (abierta el 15/09/2026, pedido de Santiago)

> Amplía INC-47: la mecánica del Nivel 1 pasa a **dos fases**. Primero solo se reúnen los
> materiales en el centro de la pantalla; después la cámara se acerca al doble, las hojas se
> acomodan en fogata y la **cercanía de las piedras se elige con un segundo deslizante**, no
> arrastrándolas. El rótulo «Aún no» de «Soplar» desaparece: el candado basta.

- [x] **T25 · Reunir: círculo de reunión y acercamiento** — `M` · `EM` + `PM` `MCP` (15/09/2026)
      RF-13, RF-14, CT-06, RNF-21, CP-02, CP-06 · depende de: T22
      Al abrir solo hay piezas, tablilla y «Pista» (`ignitionUi` oculta). «Pista» escribe la
      instrucción de `N1_Guia · Reunir` y **dibuja el círculo** (`RingSprite`, generado en
      memoria): radio = distancia horizontal del centro a la mitad de «Golpear», así que el borde
      queda alineado con la mitad del botón de abajo (`FirePanelController.GatherRadius`; sale de
      la escena, no del asset). `FireArrangement.IsGathered` (C# plano): todas las piezas dentro.
      Al soltar la última pieza dentro (`DraggablePiece.Dropped`) arranca `EnterIgnitionAsync`:
      las piezas dejan de arrastrarse, `Fondo` y `Suelo` escalan a `GatherZoom` (×2) en
      `GatherSeconds` (0,8 s), las hojas van a un anillo tangente —juntas, no encimadas— y las
      piedras al centro; luego aparece la interfaz del encendido y la tablilla pasa a la
      instrucción de golpear. Una pieza fuera: no pasa nada, sin regaño (CP-02).
- [x] **T26 · Deslizante de cercanía de las piedras** — `M` · `EM` + `PM` `MCP` (15/09/2026)
      RF-15, RF-16, RF-17, RNF-18, RNF-19, CT-05 · depende de: T25
      `CercaniaSlider` horizontal (copia del de fuerza, sin `ForceSliderFeedback`), 456×120 a
      y=222, con el recorrido del asa de la mitad de «Soplar» a la mitad de «Golpear» (x = ±172)
      y etiquetas «Lejos» / «Cerca». Muesca 0 = piedras separadas **la longitud de una hoja**;
      muesca 10 = una encima de otra. `StoneSpacing` (C# plano): `Distance`, `Overlap`,
      `Classify` → `SpacingBand` (lejos / efectivo / encimadas); certero cuando se enciman
      `EffectiveOverlap` ± `OverlapTolerance` (5 ± 4 px del lienzo de referencia) — con las
      piezas de hoy, **la muesca 2**. `FireAttempt.Strike(force, spacing)` juzga la cercanía antes
      que la fuerza; `StrikeOutcome.StonesMisplaced`; `FireMessages` pierde `BlowNoPile` y gana
      `StonesTooClose(+Again)`. Soplar ya no puede fallar: las hojas siempre están en fogata.
- [x] **T27 · Limpieza y documentos** — `S` · `PM` `MCP` (15/09/2026)
      RNF-19, RNF-20 · depende de: T26
      Se retira `BadgeBloqueado/AunNo` (queda `IconoCandado`). `N1_Config` sin `PileRadius` /
      `StonesRadius`, con `SpacingLevels`, `EffectiveOverlap`, `OverlapTolerance`, `GatherZoom`,
      `GatherSeconds`. `N1_Guia` con el paso `Reunir` y el de `Golpear` reescrito. `Interfaces.md`
      §7, `Camara_Narrativa_N1.md` §3 e `INCONSISTENCIAS.md` (INC-47) al día.

### Checkpoint F — reunir y encender
- [x] El nivel se juega entero: reunir → acercamiento → fuerza y cercanía → golpear → soplar — **EditMode 221/221 · PlayMode 144/153** por CLI (`unity test`, Editor cerrado, 15/09/2026): 6 VV omitidas en batchmode y **3 fallos preexistentes de disposición** en la resolución 640×480 del batchmode (`MazeScene_RNF03_AlAcumularse…`, `WorkshopScene_RNF03_NadaSeSale…`, `NarrativeScene_RNF01_LaLineaMasLarga…`), medidos 0/3 también sobre el código sin la Fase 6 — pasan con el Editor abierto (W-F, 148/148)
- [x] Ningún fallo penaliza ni bloquea (CP-02); ninguna pista ni mensaje nombra la muesca correcta (CP-06, RF-17) — `FirePanel_RF16_ConLasPiedrasSeparadasOEncimadas…`, `FirePanel_RF13_…`
- [x] Revisado con el usuario: el radio del círculo, el tamaño de la fogata y que la muesca 2 sea la certera (PG-06 sigue abierto)
      *(01/10/2026: Decisión — revisado por Santiago el 30/09/2026 (acta D10, §5) sobre la mecánica
      vigente: el radio del círculo, con el borde en la mitad de «Golpear»
      (`FirePanel_RF13_AlReunirLaPistaDibujaElCirculoConElBordeEnLaMitadDelBotonDeAbajo`); la
      fogata, que desde el 22/09 es el montón cenital que se quema desde el centro (`aca7acc`,
      `8ec5120`), y la cercanía certera, que desde el 21/09 es la muesca 5 y no la 2
      (`EffectiveOverlap` 30 en `N1_Config`, `2cbe287`;
      `StoneSpacing_RF16_ElGolpeCerteroEsSoloLaMuescaCinco`). Las capturas del encendido con las
      correcciones de D10-1 se revisaron el 01/10. PG-06 sigue abierto en «Bloqueantes y decisiones
      pendientes».)*

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
animando el sprite por recorte con `CharacterRig` (`Direccion_de_Arte.md` §7.5 y §13.1). Pedirle a Gemini tres
poses del mismo personaje devuelve tres personajes distintos.

- [x] **A1 · Chispa, el guía** — chroma sí — guion §1.1/§4.1, PG-02, RF-10, RF-12, RF-13
      *(01/10/2026: Decisión — el guía es Algoritm (INC-44) y no la estrella de este pedido: es una
      llama con extremidades (INC-52). Su arte del Nivel 1, `char_algoritm_n1_fuego_reposo.png`,
      entró el 24/09 (`88fe0ee`) con el prefab `Algoritm_Fuego` y nueve clips; «girando» y
      «atenuado» son los clips `girar` y `apagado`. Pruebas:
      `CharacterRig_DA133_CadaPersonajeTieneUnEstadoPorAccion` y
      `FirePanel_DA76_LaPistaMuestraAAlgoritmConSuFormaDeFuego`. El halo de su contorno se limpia en
      la casilla de postproceso de esta sección.)*
- [x] **A2 · Papá (jugable N1)** — chroma sí — **A-pose** — guion §1.1/§4.2, RF-14, HU-06, CN-02
      *(01/10/2026: Implementación — el sprite base en A-pose del 24/09 se cortó en cinco partes con
      el pivote en la articulación y se anima por recorte con `CharacterRig` (INC-53, `88fe0ee`):
      `char_papa_parte_*.png`, `char_papa_retrato_neutra.png` y el prefab `Papa`, con 21 clips y el
      cuerpo apoyado en los pies. Pruebas:
      `FireLevel_DA133_PapaGolpeaAlPulsarGolpearYVuelveAlReposo` y
      `FireLevel_CP02_TrasUnGolpeSinChispaPapaSeAnimaYNuncaHaceUnGestoDeDerrota`. El halo del
      retrato se limpia en la casilla de postproceso.)*
- [x] **A3 · Mamá** — chroma sí — **A-pose** — guion §1.1/§4.2
      *(01/10/2026: Implementación — mismo origen y rig que A2 (INC-53, `88fe0ee`):
      `char_mama_parte_*.png`, `char_mama_retrato_neutra.png` y el prefab `Mama`, con 21 clips. Es
      la jugable del Nivel 3:
      `RiverScene_DA133_MamaCaminaMientrasSeSostieneUnaFlechaYReposaAlSoltarla`. El halo del retrato
      se limpia en la casilla de postproceso.)*
- [x] **A4 · Niña** — chroma sí — **A-pose**, penacho alto — guion §1.1/§4.2
      *(01/10/2026: Implementación — rig por recorte (INC-53, `88fe0ee`): `char_nina_parte_*.png`,
      `char_nina_retrato_neutra.png` y el prefab `Nina`, con 21 clips; habla también como «NIÑOS».
      Es la jugable del bosque: `ForestScene_DA133_LaNinaSenalaElTroncoQueEligeYVuelveAlReposo` y
      `CharacterRig_RF05_LosDosNinosHablanComoNinos`. El halo del retrato se limpia en la casilla de
      postproceso.)*
- [x] **A5 · Niño** — chroma sí — **A-pose**, copete hacia adelante — guion §1.1/§4.2
      *(01/10/2026: Implementación — rig por recorte (INC-53, `88fe0ee`): `char_nino_parte_*.png`,
      `char_nino_retrato_neutra.png` y el prefab `Nino`, con los 21 clips de la familia. En el Nivel
      1 observa: `FireLevel_DA133_LaFamiliaEsperaAtrasQuietaYElNinoObserva`. El halo del retrato se
      limpia en la casilla de postproceso.)*
- [x] **A6 · Cueva, cuatro escalones de luz** — chroma **no** — guion §3.1/§4, RF-21
      *(01/10/2026: Decisión — la luz del Nivel 1 es una sola ilustración base con la máscara del
      motor, no cuatro fondos (acta D06, 15/09). `CaveLightingController` pone sobre la vista
      cenital la capa del shader `fx_oscuridad` y la aclara un escalón por golpe efectivo, así que
      los cuatro `env_n1_cueva_luz*` no se producen; los entornos del nivel se entregaron y
      aprobaron en esa acta. Pruebas: `FireLevel_RF21_IluminacionSubeUnEscalonPorGolpeEfectivo` y
      `FirePanel_RNF20_ContrasteSuficienteEnElEstadoMasOscuro`.)*
- [x] **A7 · Montón de hojas, cuatro estados** — chroma sí — guion §4.3.1/§4.3.3, RF-14, RF-16
      *(01/10/2026: Corrección — los cuatro estados los compone el motor sobre
      `prop_n1_monton_hojas_cenital.png`: intacto; con chispas, un solo rayo que nace en el punto
      del golpe (D10-1, INC-119); humeante, un hilo de humo que nace en ese punto y no pasa de medio
      montón; encendido, el quemado radial con la llama cenital y el humo en su corona. Desde D10-1
      las piedras quedan siempre sobre las hojas. Pruebas:
      `FirePanel_RF14_DuranteElAcercamientoLasPiedrasNuncaQuedanBajoLasHojas`,
      `FirePanel_RF16_LaChispaEsUnRayoDelCentroQueCaeEnLasHojasEnUnaDireccionAlAzar`,
      `FirePanel_RF19_ElHiloDeHumoNaceEnElPuntoDelGolpeYNoEsMasAltoQueMedioMonton`,
      `FireLevel_RF20_AlSoplarPrendeLaLlamaCenitalSobreElMonton` y
      `FireLevel_RF20_AlPrenderElFuegoElHumoSubeALaCoronaDeLaLlama`, en verde en la suite completa
      del 01/10/2026.)*
- [x] **A8 · Sílex y pedernal** — chroma sí — guion §4.1/§4.2, RF-16, RNF-19
      *(01/10/2026: Implementación — `prop_n1_silex.png` y `prop_n1_pedernal.png` son definitivos
      (`2cbe287`, 21/09) y distintos entre sí a simple vista, como pidió el acta D06.
      `prop_n1_piedras_choque` no hace falta: el choque lo dan el golpe de Papá y el rayo que dibuja
      el motor (INC-68, INC-119). Desde D10-1 las piedras se dibujan sobre las hojas durante todo el
      acercamiento (`FirePanel_RF14_DuranteElAcercamientoLasPiedrasNuncaQuedanBajoLasHojas`, en
      verde en la suite completa del 01/10/2026).)*
- [x] **A9 · Controles del panel de encendido** — chroma sí — RF-14, RF-15, RF-19, RNF-19
      *(01/10/2026: Decisión — Santiago aceptó el 30/09/2026 el conjunto genérico de interfaz como
      arte final (acta D10, §5), así que no hay láminas `ui_n1_panel_*`. El panel del mockup 7 se
      arma con `ui_boton`, `ui_circulo` y `ui_lock` teñidos, y lo vigilan
      `FirePanel_RNF19_SoplarAtenuadoSeDistinguePorElCandadoAdemasDelColorYSinRotuloAunNo` —el
      candado es el segundo indicador— y `FirePanel_RNF20_ContrasteSuficienteEnElEstadoMasOscuro`,
      en verde en la suite completa del 01/10/2026.)*
- [x] **A10 · Marco de diálogo del guía** — chroma sí — RF-05, RF-06, RNF-20
      — revisado por Santiago el 30/09/2026 (acta D10, decisión D-a), tras la verificación de
      Claude del 01/10/2026: el marco final es el cuadro de diálogo del conjunto genérico de
      interfaz que D-a acepta como definitivo —`CuadroDialogo` de `Narrative.unity`, con
      `ui_panel` y `ui_boton` teñidos y el retrato de quien habla—, sin láminas `ui_dialogo_*`; la
      dirección de arte §10.3 lo describe como cuadro y no como globo con cola (INC-129). Contraste
      medido sobre píxeles: nombre del hablante 6,2:1, línea 13,5:1 y «Continuar» 7,1:1, todos
      ≥ 4,5:1 (`claudeDocs/tasks/OE4/evidencias/arte/rnf20.md`).
- [x] Cada asset pasa la **checklist de `Direccion_de_Arte.md` §17** y su línea «Verificación»
      — revisado por Santiago el 30/09/2026 (acta D10, decisión D-a), tras la verificación de
      Claude del 01/10/2026: las columnas que se miden las midió `arte_check.py` sobre los 96 PNG
      que no son cuadros de entrega —nombre §15.4 96/96, canal alfa 96/96, halo de croma
      intenso en 0, sin compresión con pérdida 96/96 (`claudeDocs/tasks/OE4/evidencias/arte/sprites.md`)—,
      con RNF-20 en 24 textos ≥ 4,5:1 y cuatro iconos ≥ 3:1 (`rnf20.md`, `rnf20-extra.md` y, para
      el icono de alerta del Nivel 3, `rnf20-w3.md`), RNF-19 en
      `rnf19.md` y `rnf19-w3.md` y RNF-21 en las tres secuencias del fuego y del humo, con 1
      destello/s como máximo (`rnf21.md`); los glifos de Phosphor (MIT) se acreditan en los
      créditos (INC-127). Las columnas de criterio quedan con la aceptación del arte entregado
      (acta D10, §4: «tablero de arte A1 a A10, cubierto por el arte entregado»); lo que el proceso
      ya no pide —fondo croma, margen del 10 % en piezas recortadas, cuatro fondos de luz— quedó sin
      objeto con INC-53 y el acta D06.
- [x] Postproceso: recorte del verde, alfa, halo, nombre según §15.4, import con los ajustes de
      §15.2 (**PPU 100**, pivot `Bottom` en personajes y `Center` en props e interfaz)
      — cerrado el 01/10/2026: el halo de las tres formas de Algoritm y de los cuatro retratos se
      limpió sin redibujar ni cambiar el GUID, y los cinco nombres fuera de §15.4 se renombraron
      desde el motor (`env_n1_apertura`, `env_n1_cueva_2x`, `env_n1_cueva_cenital`,
      `env_n2_laberinto`, `prop_n2_caja_suelo_vacia`; INC-126), lo que vigila
      `ArtImport_RNF23_LosNombresSiguenLaNomenclatura`, en verde en la suite de rc2. Todo
      `Assets/Game/Art/` va a 100 PPU y sin comprimir, a 4096 salvo los cuadros del fuego y del humo
      del Nivel 1, que entran a 1024 (`ArtImportRules`, INC-130,
      `ArtImport_RNF06_LosCuadrosDelFuegoYElHumoSeImportanAMil24SinComprimir`), y todo lo que no
      es entorno lleva transparencia (`claudeDocs/tasks/OE4/evidencias/arte/sprites.md`). §15.2
      describe ya esos ajustes (INC-128): el importador deja el pivote en el centro y en los
      personajes el que cuenta vive en el rig, en la articulación de cada parte y con el cuerpo
      apoyado en los pies (INC-53). Quedan documentadas, sin corregir en este corte, las motas
      verdes opacas en las puntas del pelo de los retratos de Niña y Papá, que piden limpieza
      manual del carril de arte (decisión D-OBS).
- [x] Verificar RNF-20 y RNF-19 sobre el arte final, no sobre el prompt
      — cerrado el 01/10/2026: medido sobre capturas del arte final con `arte_check.py`. RNF-20:
      tablilla de la cueva en su estado más oscuro 13,5:1, cuadro de diálogo ≥ 6,2:1, pantalla de
      inicio ≥ 6,2:1 y mandos del panel de encendido ≥ 7,1:1 (`claudeDocs/tasks/OE4/evidencias/arte/rnf20.md`,
      `rnf20-extra.md`). RNF-19 en escala de grises: montón, humo y fuego se separan por forma y
      por gris (A7), y «Soplar» atenuado, que en gris apenas cambia, lo distingue el candado, a
      13,8:1 (`rnf19.md` y su lectura). Lo vigilan además
      `FirePanel_RNF20_ContrasteSuficienteEnElEstadoMasOscuro`,
      `FirePanel_RNF19_SoplarAtenuadoSeDistinguePorElCandadoAdemasDelColorYSinRotuloAunNo` y
      `LevelSelect_RNF19_ElEstadoBloqueadoLlevaCandadoYTextoAdemasDelColor`, en verde en la suite
      de rc2 (`claudeDocs/tasks/OE4/evidencias/suites/rc2-final/`).

---

## Bloqueantes y decisiones pendientes

- [x] **R1 · Servidor MCP para pruebas.** Rider 2026.2 + plugin «MCP Server Extension for Unity»
      (id 30357) instalados el 06/09; MCP `rider` registrado. **Sigue abierto:** en la sesión del
      08/09 el MCP `rider` arrancó con `ConnectionRefused` pese a tener el Editor de Unity abierto
      — el puente lo sirve **Rider**, no Unity, así que con Rider cerrado no hay a quién preguntar.
      Entretanto `unity test` (CLI, Editor cerrado) cubre el flujo test-first: es lo que se usó en
      T04b y en T11.
      *(01/10/2026: Implementación — el servidor está instalado y responde con Rider abierto —T12 a
      T19 se corrieron por él—; `ConnectionRefused` significa «Rider cerrado» (`CLAUDE.md`
      §Comandos), como ya dice la cabecera de este tablero. El P00 del Slice 4 (23/09) fijó
      `unity test` como corredor confirmado, y desde el 30/09
      `claudeDocs/tasks/OE4/herramientas/editor.ps1` (`649b4d6`) corre las suites con el Editor
      abierto por la API de `com.unity.pipeline`: así se corrió la suite completa del 01/10/2026.)*
- [x] **PG-01** · título provisional para `GameTitleConfig` (T05) = **«Algoritm»** (confirmado por
      el usuario el 06/09/2026). Marcador; se cambia editando el asset sin recompilar.
- [x] **T08** · confirmar que los créditos entran en el Slice 1 (RF-01 pone el botón en el inicio).
      *(01/10/2026: Implementación — T08 (07/09) dejó la escena `Credits` con
      `CreditsContent.asset`, y entra en el ejecutable entre las doce escenas de
      `EditorBuildSettings`; «Créditos» está en la pantalla de inicio
      (`MainMenu_RF01_MuestraJugarCreditosYSalirAlcanzablesPorRaycast`,
      `Credits_RF08_MuestraElReconocimientoDeLaAutoria`,
      `Credits_RF08_VolverRegresaAlMenuPrincipal`, en verde en la suite completa del 01/10/2026).
      RF-01 radicado enumera Jugar, Créditos, Progreso del equipo y Salir (INC-81).)*
- [ ] **PG-06** · validar jugando los valores de `FireLevelConfig` en el Checkpoint D.
      — sigue abierta el 01/10/2026 por la decisión D-b (acta D10, §5): la valida Santiago jugando
      con estudiantes, con H2 de `claudeDocs/tasks/OE4/Hoja-HUM.md` —en la misma sesión que H1— y
      el consentimiento de RNF-12 (`claudeDocs/tasks/OE4/Consentimiento-RNF12.md`), y se marca con
      su plantilla. Los valores vigentes están en esa hoja y en `N1_Config`.
