# Fase 3 — Nivel fuego: lo construido y sus resultados

**Slice 1 · Golden Path temprano** · Estado: **T12–T15 implementadas y verdes** — corte de este
documento: 11 de septiembre de 2026.
Verificación vigente: **EditMode 83/83 · PlayMode 48/48** (`FirePanelTests` 13/13), 0 fallos.
Plan técnico: [`plan.md`](plan.md) · Tablero: [`todo.md`](todo.md) · Contrato: `claudeDocs/SPEC.md`
Fase anterior: [`Fase-2-Resultados.md`](Fase-2-Resultados.md)

La Fase 3 es el módulo `nivel-fuego`: el nivel jugable completo (panel de encendido, iteración,
depuración, convergencia, resolución, indicadores). Este documento se irá llenando tarea a tarea;
el código de la fase **no está completo**.

| Tarea | Qué entrega | Modo de prueba | Estado |
|---|---|---|---|
| T12 | `StrikePosition`, `FireLevelConfig`, `StrikeOutcome`, `FireAttempt` — lógica pura del nivel | EditMode | ✅ terminada |
| T13 | `FireFeedbackLog` y los ocho mensajes del guion §4.3.4 | EditMode | ✅ terminada |
| T14 | Panel de encendido y escena `Level1_Cave` | PlayMode | ✅ terminada |
| T15 | Convergencia: «Soplar» → nacimiento del fuego | PlayMode + VV | ✅ terminada |
| T16 | Menú de pausa | EditMode + PlayMode | pendiente |
| T17 | Emisión de los cuatro indicadores del N1 | EditMode | pendiente |
| T18 | Resumen de fin de nivel y cierre reflexivo | EditMode + PlayMode | pendiente |
| T19 | Iluminación progresiva del escenario (RF-21, Baja) | VV | pendiente |

---

## 1. T12 — el corazón del Nivel 1, C# plano

El assembly `Game.Levels.Fire` existía como `.asmdef` vacío desde T01; T12 le pone el primer
código. Cuatro archivos en `Assets/Game/Scripts/Runtime/Levels/Fire/`:

- **`StrikePosition`** — enum de tres distancias del deslizante (guion §4.3.2): `Far`, `Near`,
  `VeryClose`. «Muy cerca» es la única efectiva por defecto.
- **`FireLevelConfig`** — ScriptableObject con los cuatro parámetros ajustables jugando
  (`AvailablePositions`, `EffectivePosition`, `MinimumEffectiveStrikes`, `AttemptsBeforeHint`),
  `[field: SerializeField]` + `[Tooltip]` con el texto del guion, valor por defecto = el propuesto
  (3 / Muy cerca / 3 / 3). Fuera del código para que retocarlos —PG-06 sigue abierto— no cueste
  recompilar (CT-05, RNF-18). **No se crea el `.asset`**: es de T14; los tests usan
  `ScriptableObject.CreateInstance`.
- **`StrikeOutcome`** — `readonly struct` que devuelve un golpe: `Effective`, `Position`,
  `EffectiveStrikes`. Fábricas `SparksDied(position)` / `SparkLanded(effectiveStrikes)` con las
  firmas literales del `SPEC.md` §Estilo. Lo consumirá T13 para elegir el mensaje del log.
- **`FireAttempt`** — el código del `SPEC.md` §Estilo (líneas 334–365) tal cual, con
  `ConsecutiveFailures` expuesto como propiedad pública de solo lectura (patrón de `HintPolicy`).
  Lleva los dos contadores del guion §4.3.3/§4.3.5:
  - Golpe desde una posición no efectiva → `ConsecutiveFailures++`, devuelve `SparksDied`; **no
    toca los golpes efectivos** (RF-16).
  - Golpe desde la posición efectiva → `EffectiveStrikes++`, `ConsecutiveFailures = 0`, devuelve
    `SparkLanded`.
  - `CanBlow` deriva **solo** de `EffectiveStrikes`, que solo crece → una vez habilitado el soplo
    no vuelve a deshabilitarse ni tras un fallo posterior (INC-32, guion §4.3.6). Comentario «por
    qué no» pedagógico en el código.
  - Sin tope de intentos, sin excepción, sin contador de derrota (RF-18, CP-02).
  - **No guarda la posición del deslizante**: es estado de UI (T14). El modelo solo cambia al
    llamar `Strike` (RF-15).

Sin `.asmdef` nuevos ni modificados: `Game.Levels.Fire` ya referenciaba `Game.Core` y
`Game.Scaffolding`, y `Game.Levels.Fire.Tests` ya estaba listo desde T01.

---

## 2. T13 — el log narrativo, C# plano

Dos archivos más en `Assets/Game/Scripts/Runtime/Levels/Fire/` y un asset:

- **`FireMessages`** — ScriptableObject con los **ocho mensajes del guion §4.3.4 literales**, ocho
  propiedades `[field: SerializeField, TextArea]` + `[Tooltip]` (patrón de `GuideContent`). El
  asset real es `Assets/Game/Data/Fire/N1_Mensajes.asset`, escrito como YAML a mano
  (`unity-yaml-editing-guide`); el nombre sigue la convención `N1_*` del Nivel 1.
- **`FireFeedbackLog`** — C# plano, sin Unity. `Record(StrikeOutcome, consecutiveFailures)` elige
  el mensaje y lo acumula:
  - golpe efectivo → por ordinal: primero, segundo, o **final** al alcanzar el mínimo (guion
    §4.3.5 E6, el «hilo de humo»);
  - golpe no efectivo → por distancia (`Far`/`Near`) y racha: la variante «tras dos intentos» del
    guion §4.3.4 entra desde el **segundo** fallo seguido;
  - **no repite el mismo mensaje dos veces seguidas** cuando hay alternativa aplicable (RF-18,
    «consecuencia observable distinta de la anterior»): en la misma distancia los golpes seguidos
    alternan «cualquiera» / «tras dos intentos».
  - `Entries` acumula **todo** el historial en orden, sin tope (RF-18, HU-06); el desborde visual
    del área de registro es de T14. `Latest` es el último mensaje.
  - `RecordBlowSuccess()` registra el mensaje del soplo (guion §4.3.4, HU-05 FA-01); lo llamará
    T15.

Sin cambios de `.asmdef`.

---

## 2b. T14 — el panel de encendido y la primera escena jugable

- **`Level1_Cave.unity`** — construida por script de Editor (skill `edit-scene`), alta en Build
  Settings (**6 escenas**). Canvas Overlay 1920×1080; `EventSystem` con `InputSystemUIInputModule`
  (nunca el legado, CT-06); deslizante de tres posiciones (`Navigation.None`), «Golpear», «Soplar»
  atenuado con **badge candado + «Aún no»** (RNF-19), área de registro con `ScrollRect`, montón de
  hojas. Arte **placeholder** (A7–A9 sin generar).
- **`FirePanelController`** (`Game.Levels.Fire`, con `UnityEngine.UI` añadido al `.asmdef`) —
  adaptador delgado: el clic de «Golpear» llama a `FireAttempt.Strike` (T12), el resultado a
  `FireFeedbackLog.Record` (T13), y el estado a la UI. **Cero reglas del juego.** El deslizante no
  produce efecto hasta «Golpear» (RF-15). «Soplar» se habilita cuando `CanBlow` y su badge se
  oculta; no re-bloquea (INC-32). «Soplar» hoy solo registra el mensaje del soplo — la animación,
  el guardado y el salto a la escena de cierre son T15.
- **`FeedbackLogView`** — vista tonta que une `Entries` con saltos de línea y baja el scroll.
  Desviación del plan: vive en `Game.Levels.Fire`, no en `Game.UI` (así `Game.UI` no depende de
  ningún nivel y `Game.Levels.Fire` no depende de `Game.UI`).
- **`ControlesJugables.inputactions`** (nuevo) — mapa `UI` con **solo** bindings de puntero
  (`<Pointer>`, `<Mouse>`, `<Touchscreen>`, `<Pen>`): sin Navigate/Submit/Cancel, sin
  `<Keyboard>`/`<Gamepad>`. Es el `actionsAsset` del `EventSystem` de la escena jugable — la
  verificación de RNF-02 barre sus bindings. Las escenas de menú/narrativa quedan como están:
  RNF-02 acota «escenas **jugables**».
- **`GameFlowRunner.Scenes`** mapea `Playing → Level1_Cave` (un solo nivel hoy; por `LevelId`
  cuando lleguen Rueda/Río). **`NarrativeSceneController.Leave()`** entra a jugar el nivel de la
  secuencia (`StartPlaying`), cerrando el `// PROVISIONAL (T14)`.
- **`N1_Config.asset`** — el `FireLevelConfig` real (3 / Muy cerca / 3 / 3), YAML a mano.

### T14b — `PlayFromBoot.cs`: el corredor PlayMode se colgaba desde el MCP de Rider

Al lanzar la suite PlayMode con `mcp__rider__run_unity_tests`, el Editor entraba a Play,
`playModeStartScene` cargaba `Boot`, **el juego arrancaba solo** (Boot → MainMenu → …) y el
corredor de pruebas nunca recibía el control: timeout tras timeout. El Test Framework de este
proyecto (`@1405238725ab`) **no gestiona `playModeStartScene`** —cero referencias en todo el
paquete—, así que la guarda de `PlayFromBoot` era lo único que lo impedía, y su callback
`RunStarted` no llega antes de `ExitingEditMode` en la ruta de Rider. Arreglo: una guarda en
`EditorApplication.playModeStateChanged` (`ExitingEditMode`) que consulta el flag interno
`PlaymodeLauncher.IsRunning` por reflexión y despeja `playModeStartScene` si el corredor está
arrancando esa entrada a Play. Verificado: la suite PlayMode entera corre sin colgarse.

*(Aparte: el Test Runner del Editor se cuelga de forma **intermitente** —siempre en
`TestJobRunner.ExecuteCallback` → el botón de toolbar del plugin Coplay— y se recupera solo tras
un rato o reiniciando el Editor. No es de `PlayFromBoot`.)*

---

## 2c. T15 — la convergencia: «Soplar» completa el nivel

**Sin escena nueva ni tocar `Level1_Cave.unity`.** «Soplar» ya se habilitaba en `CanBlow` desde
T14; T15 completa lo que faltaba al accionarlo en condiciones válidas (guion §4.3.5 E7, §4.4):

- **`FirePanelController.Blow()`** ahora encadena `ResolveAsync()`: anima la convergencia y, al
  terminar, marca la fase completada. **Desviación del plan:** no se creó
  `FireResolutionController.cs` — la lógica se sumó al controlador de T14 (mismo criterio que
  `NarrativeSceneController.Leave()` o `LevelSelectController.StartLevel()`: un adaptador puede
  orquestar guardar+desbloquear+navegar sin que eso sea «una regla del juego»). Evita una tercera
  pasada de `edit-scene` sobre un Editor que se había colgado varias veces en la fase.
- **`PlayIgnitionAsync`** — un único `Color.Lerp` del `leavesImage` hacia un naranja de fuego en
  ~0,6 s, monótono y en una sola dirección: RNF-21 prohíbe destellos de alta frecuencia y un
  `PingPong` o cualquier oscilación los produciría. Placeholder — el sprite de fuego real y la
  iluminación del resto del escenario son arte pendiente / T19.
- **`CompleteLevel`** — sin perfil activo, avisa y no hace nada (mismo criterio que `ScreenFlow`,
  sin acoplar `Game.Levels.Fire` a `Game.UI`). Con perfil: `PlayerProfile.ConfirmPhase(Fire, 1,
  default)` (**los cuatro indicadores reales son T17** — de momento se confirma la fase sin
  ellos, mismo patrón que T11 dejó el umbral de pista para T12), `LevelUnlockPolicy.UnlockAfterCompleting`
  (Nivel 2, RF-03), `(Saver ?? Runner.Session).SaveActive()` (RF-04, con el mismo patrón de
  `IProfileSaver` inyectable que `MainMenuController.Saver`, para que las pruebas usen un espía y
  no toquen disco), y `Runner.StartNarrative("N1_NacimientoDelFuego")` (RF-20).
- **`NarrativeOutcome`** (nuevo, `Game.Scaffolding`): `EntersLevel` / `ReturnsToLevelSelect`.
  `NarrativeSceneController.Leave()` (T14) asumía que **toda** narrativa entra a jugar; con la de
  cierre del N1 eso ya no vale. La secuencia declara su propósito en el asset —sin `if` por
  secuencia, la regla del proyecto se mantiene— y `Leave()` decide por ese campo. Valor por
  defecto (`EntersLevel = 0`) igual al comportamiento anterior: **los tres assets existentes no
  se tocaron**.
- **`N1_NacimientoDelFuego.asset`** (nuevo) — las 19 líneas del guion §4.4 (el nacimiento del
  fuego y el cierre reflexivo de Chispa, «se llama iterar»), añadido como cuarta entrada al
  arreglo `sequences` de `Narrative.unity` — un cambio de una sola línea en la escena, exactamente
  la promesa de T09/T10: «añadir una escena narrativa es crear un asset».

### Un defecto real encontrado al verificar: `LoadPanel()` podía devolver el controlador equivocado

Verificando T15 contra la suite completa, `FirePanel_RF15_…`, `FirePanel_RF17_…` y
`FirePanel_RF19_…` (las tres de T14, sin tocar) fallaron en 2 de 3 corridas con mensajes de
**otra** prueba coladas en su registro. No era ruido del Editor: `LoadPanel()` (helper compartido
por las 13 pruebas desde T14) usaba `FindAnyObjectByType<FirePanelController>`, que en una corrida
de suite completa podía devolver el controlador de la prueba **anterior** —destruido por el
cambio de escena pero aún no purgado al final del fotograma— en vez del recién cargado. Arreglado
en el único sitio que lo causaba: un fotograma extra de margen tras el `Start()` de la escena, y
`FindObjectsByType` con `Assert.That(count == 1)` en vez de un `FindAnyObjectByType` silencioso
que podía acertar por casualidad. Confirmado con **3 corridas limpias seguidas (48/48)** tras el
arreglo.

---

## 3. Verificación — declarada

### 3.1 EditMode — T12

**Suite Fire: 9/9, 0 fallos** (`mcp__rider__run_unity_tests`, EditMode, 10/09/2026).

| Prueba | Requisito |
|---|---|
| `FireLevelConfig_CT05_ExponeLosCuatroParametrosDelGuion` | CT-05, guion §4.3.2 |
| `FireAttempt_RF16_GolpeNoEfectivoProduceConsecuenciaYNoSuma` (`Far`, `Near`) | RF-16 |
| `FireAttempt_RF16_GolpeEfectivoSumaYReiniciaLosFallos` | RF-16, guion §4.3.3 |
| `FireAttempt_RF15_CambiarPosicionNoAlteraElEstado` | RF-15 |
| `FireAttempt_RF18_AceptaIntentosIlimitados` | RF-18, CP-02 |
| `FireAttempt_INC32_SoplarSeHabilitaAlAlcanzarElMinimo` | INC-32 |
| `FireAttempt_RF19_SoplarNoSeDeshabilitaTrasFalloPosterior` | RF-19, INC-32 |
| `FireAttempt_RNF18_ElUmbralDePistaSaleDeLaConfiguracion` | RNF-18, RF-19 |

### 3.2 EditMode — T13

**Suite Fire: 20/20, 0 fallos** (11 nuevas). **EditMode total: 83/83, 0 fallos** — sin regresión.

| Prueba | Requisito |
|---|---|
| `FireMessages_RNF18_LosOchoMensajesDelGuionEstanDefinidos` | RNF-18, CT-05 |
| `FireMessages_RF17_NingunMensajeDelAssetContieneCifras` | RF-17, CP-03 |
| `FireFeedbackLog_guion434_CadaDistanciaNoEfectivaDevuelveSuMensaje` (`Far`, `Near`) | guion §4.3.4 |
| `FireFeedbackLog_RF18_TrasDosFallosSeguidosEnLaMismaDistanciaEscalaElMensaje` | RF-18, guion §4.3.4 |
| `FireFeedbackLog_HU05_CadaGolpeEfectivoDevuelveElMensajeDeSuOrdinal` (1, 2, 3) | HU-05, guion §4.3.4 |
| `FireFeedbackLog_RF18_NoRepiteElMismoMensajeDosVecesSeguidas` | RF-18 |
| `FireFeedbackLog_HU06_AcumulaElHistorialEnOrden` | HU-06, HU-05 |
| `FireFeedbackLog_HU05_ElSoploExitosoRegistraSuMensaje` | HU-05, guion §4.3.4 |

**Desviación de `plan.md` §T13:** la prueba de no-repetición se nombra con **RF-18**
(«consecuencia distinta de la anterior»), no RF-17; RF-17 («sin cifras ni juicios») queda en
`FireMessages_RF17_…ContieneCifras`. Ambos requisitos trazados.

### 3.3 PlayMode — T14

**`FirePanelTests` 9/9, 0 fallos.** **PlayMode total: 44/44, 0 fallos** — sin regresión (33/33 de
la Fase 2 + 2 de verificación visual que ahora sí capturan contra el Editor abierto + 9 de T14).
**EditMode 83/83** sin cambios.

| Prueba | Requisito |
|---|---|
| `FirePanel_RF14_PresentaDeslizanteBotonGolpearYRegistro` | RF-14 |
| `FirePanel_RF15_MoverElDeslizanteNoEjecutaNingunGolpe` | RF-15 |
| `FirePanel_RF16_GolpearEscribeEnElRegistroElMensajeDeLaDistancia` | RF-16, RF-17 |
| `FirePanel_RF19_SoplarSeHabilitaAlAlcanzarElMinimoDeGolpesEfectivos` | RF-19, INC-32 |
| `FirePanel_RF19_SoplarAtenuadoNoRespondeNiRegistraError` | RF-19, guion §4.3.6 |
| `FirePanel_RNF02_ElMapaDeControlesNoTieneNingunBindingDeTeclado` | RNF-02, CT-06 |
| `FirePanel_RNF03_LaEscenaNoPresentaListaDeTareas` | RNF-03, INC-41 |
| `FirePanel_RF17_ElRegistroNoDesbordaTrasDiezIntentos` | RF-17, HU-06 (aserción de layout) |
| `FirePanel_RNF19_SoplarAtenuadoSeDistinguePorIconoYTextoAdemasDelColor` | RNF-19, RNF-20 (VV) |

**Verificación visual (RNF-19 / RNF-20).** La captura
`TestScreenshots/FirePanel_RNF19_SoplarAtenuado.png`, revisada a mano: el botón «Soplar» atenuado
muestra **icono de candado + la palabra «Aún no»** en un badge bajo el botón — distinguible sin
depender del color (RNF-19 ✅). Contraste de la instrucción, «Lejos», «Golpear», «Soplar» y el
badge sobre el pergamino suficiente; los glifos con tilde («montón», «golpéalas», «Aún») se dibujan
(RNF-20 ✅). La primera captura **falló** —el badge no se veía, «Soplar» solo se distinguía por
color— y se corrigió reparentando el badge fuera del botón (§2b).

### 3.4 PlayMode — T15

**`FirePanelTests` 13/13, 0 fallos** (4 nuevas). **PlayMode total: 48/48, 0 fallos** — 3 corridas
limpias consecutivas tras el arreglo de `LoadPanel()` (§2c). **EditMode 83/83** sin cambios.

| Prueba | Requisito |
|---|---|
| `FireLevel_RF20_SoplarEncadenaAnimacionYEscenaDeCierre` | RF-20 |
| `FireLevel_RF04_GuardaAlCompletarLaFase` | RF-04 |
| `FireLevel_RF03_DesbloqueaElNivel2` | RF-03 |
| `FireLevel_RNF21_SinDestellosDeAltaFrecuencia` | RNF-21 (VV) |

**Verificación visual (RNF-21).** La captura `TestScreenshots/FireLevel_RNF21_ConvergenciaAMitad.png`
muestra el panel a mitad de la animación de convergencia. El criterio en sí —sin parpadeo ni
oscilación— se verifica leyendo `PlayIgnitionAsync`: un único `Color.Lerp` monótono, sin
`PingPong` ni reinicio; una captura estática no puede mostrar parpadeo a lo largo del tiempo, así
que documenta el estado para quien la revise a mano y el código es la fuente de verdad del
criterio. RNF-19/RNF-20 (el badge de «Soplar», el contraste) siguen como en T14, sin cambios.

### 3.5 Flujo test-first

Esqueleto compilable → pruebas en rojo → implementación → refactor.

- **T12 RED: 8 fail / 2 pass.** Las dos verdes contra el esqueleto son correctas: `FireLevelConfig_CT05`
  (SO de datos puros, no hay lógica de Step 3) y `FireAttempt_RF15` (invariante: el estado inicial
  ya es el correcto y solo `Strike` lo cambia). Antes del refuerzo eran 6 fail / 4 pass: se
  endurecieron dos pruebas para que fallaran de verdad. **GREEN: 9/9** tras fusionar una segunda
  prueba de RF-19 en `RNF18`.
- **T13 RED: 9 fail / 2 pass.** Las dos verdes son las de `FireMessages` sobre el `.asset`, que ya
  existe con los ocho textos — validan un dato, no código de Step 3 (mismo caso que T12). Toda la
  lógica de `FireFeedbackLog` falló en rojo. **GREEN: 20/20.** En el refactor, `/simplify` fusionó
  tres helpers de selección de mensaje (`FailureMessage` + `FailureAlternative` + `AvoidRepeat`) en
  uno con deconstrucción de tupla; misma conducta.
- **T14 RED: 3 fail / 6 pass.** Las seis verdes contra el esqueleto son legítimas (presencia,
  inercia, comprobación de escena/config, VV) —la escena ya existe y nada de producción tiene que
  correr para que se cumplan—. Las tres rojas (`RF16`, `RF17`, `RF19 habilita`) fallan en su
  aserción de comportamiento. **GREEN: 9/9.**
- **T15 RED: 3 fail / 10 pass** (de 13: las 9 de T14 sin tocar + las 4 nuevas). Las 3 rojas
  (`RF20`, `RF04`, `RF03`) son el comportamiento aún sin cablear; la VV nace verde (solo escribe
  el archivo de la captura). Una de las 9 de T14 falló de forma intermitente en esta fase por el
  defecto de `LoadPanel()` (§2c), no por el rojo esperado. **GREEN: 13/13** tras cablear `Blow()`
  y arreglar `LoadPanel()`.

### 3.6 Notas

- **El MCP de Rider respondió la mayor parte de la sesión** (`get_unity_compilation_result` y
  `run_unity_tests` contra el Editor abierto). R1 deja de bloquear el flujo test-first mientras
  Rider siga abierto; `unity test` (CLI) queda de respaldo. **T14 y T15 costaron varias corridas**
  por el cuelgue intermitente del Test Runner (§2b) — en T15 el Editor llegó a cerrarse solo y se
  reabrió con `execute_run_configuration("Start Unity")` de Rider.
- **PlayMode no se re-corrió** en T12 ni T13: lógica pura en un assembly aislado. T14 y T15 sí son
  PlayMode y de ahí la regresión completa.
- **Compilación sin warnings nuevos** (solo los preexistentes de `Game.Audio` vacío). Rider no
  incluye los archivos nuevos en la solución, así que `/resolve-diagnostics` no corrió en ninguna
  de las cuatro tareas; la comprobación vinculante de «0 warnings» es la de Unity.

---

## 4. Lo que queda abierto

- **PG-06 / R2** — los valores de `FireLevelConfig` (Muy cerca, 3, 3) y el texto de los ocho
  mensajes son los propuestos por el guion §4.3.2/§4.3.4 y se validan jugando en el Checkpoint D.
- **Botón de ayuda (`HintPolicy`) sin cablear** — el panel y el registro existen desde T14, pero
  **no** el botón de ayuda: `FireAttempt.ShouldOfferHint` y `HintPolicy` cuentan lo mismo por
  caminos distintos y hay que decidir cuál lo alimenta. Sigue sin resolverse tras T15; queda para
  cuando el andamiaje se cablee a la escena (T16/T18 según se distribuya).
- **Arte del Nivel 1 (A7–A9)** — la escena `Level1_Cave` va con placeholders: montón de hojas =
  elipse blanca, badge y registro con formas planas, el «fuego» de T15 es un barrido de color sin
  sprite propio. Los cuatro estados reales del montón, el fuego y la iluminación del resto del
  escenario son T19 / arte pendiente.
- **Los cuatro indicadores de T15 son `default`** — `CompleteLevel` confirma la fase 1 del Nivel 1
  sin los indicadores reales (T17, `FireIndicatorCollector`); cuando T17 llegue, reemplaza ese
  `default` sin tocar el resto de `CompleteLevel`.
- **El encadenado de las tres secuencias narrativas del N1 (§4.1/§4.2) antes del panel** sigue sin
  resolverse — `NarrativeSceneController.Leave()` entra al nivel tras **cualquier** secuencia de
  apertura del N1 (`Outcome = EntersLevel`), no específicamente tras `N1_Hallazgo`.
- **Cuelgue intermitente del Test Runner** — el plugin Coplay reañade su botón de toolbar durante
  `TestJobRunner.ExecuteCallback` y ahí el corredor se traba a veces; en T15 llegó a cerrar el
  Editor por completo. Se recupera reiniciando (`execute_run_configuration("Start Unity")` de
  Rider) o esperando. No lo arregla el parche de `PlayFromBoot`, que resuelve un problema distinto
  (`playModeStartScene` secuestrando la entrada a Play).
- **`ProjectSettings.asset` autoañade `SENTIS_ANALYTICS_ENABLED`** en cada reimport (revertido
  varias veces en T14 y T15). Toca a RNF-08 «sin telemetría» — decisión del usuario.
