# Fase 3 — Nivel fuego: lo construido y sus resultados

**Slice 1 · Golden Path temprano** · Estado: **T12, T13 y T14 implementadas y verdes** — corte de
este documento: 10 de septiembre de 2026.
Verificación vigente: **EditMode 83/83 · PlayMode 44/44** (`FirePanelTests` 9/9), 0 fallos.
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
| T15 | Convergencia: «Soplar» → nacimiento del fuego | PlayMode + VV | pendiente |
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

### 3.4 Flujo test-first

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

### 3.5 Notas

- **El MCP de Rider respondió la mayor parte de la sesión** (`get_unity_compilation_result` y
  `run_unity_tests` contra el Editor abierto). R1 deja de bloquear el flujo test-first mientras
  Rider siga abierto; `unity test` (CLI) queda de respaldo. **T14 costó varias corridas** por el
  cuelgue intermitente del Test Runner (§2b).
- **PlayMode no se re-corrió** en T12 ni T13: lógica pura en un assembly aislado. T14 sí es
  PlayMode y de ahí la regresión completa **44/44**.
- **Compilación sin warnings nuevos** (solo los preexistentes de `Game.Audio` vacío). Rider no
  incluye los archivos nuevos en la solución, así que `/resolve-diagnostics` no corrió en ninguna
  de las tres tareas; la comprobación vinculante de «0 warnings» es la de Unity.

---

## 4. Lo que queda abierto

- **PG-06 / R2** — los valores de `FireLevelConfig` (Muy cerca, 3, 3) y el texto de los ocho
  mensajes son los propuestos por el guion §4.3.2/§4.3.4 y se validan jugando en el Checkpoint D.
- **Botón de ayuda (`HintPolicy`) sin cablear** — T14 trajo el panel y el registro (`FeedbackLogView`),
  pero **no** el botón de ayuda: `FireAttempt.ShouldOfferHint` y `HintPolicy` cuentan lo mismo por
  caminos distintos y hay que decidir cuál lo alimenta. Queda para cuando el andamiaje se cablee a
  la escena (T15/T18 según se distribuya).
- **Arte del Nivel 1 (A7–A9)** — la escena `Level1_Cave` va con placeholders. Montón de hojas =
  elipse blanca; badge y registro con formas planas. Los cuatro estados reales del montón y el
  humo/fuego son T15/T19.
- **Cuelgue intermitente del Test Runner** — el plugin Coplay reañade su botón de toolbar durante
  `TestJobRunner.ExecuteCallback` y ahí el corredor se traba a veces; se recupera solo o
  reiniciando el Editor. No lo arregla el parche de `PlayFromBoot`.
- **`ProjectSettings.asset` autoañade `SENTIS_ANALYTICS_ENABLED`** en cada reimport (revertido 3
  veces en T14). Toca a RNF-08 «sin telemetría» — decisión del usuario.
- **`StrikeOutcome` sin `Position` en golpes efectivos** — T13/T14 no la necesitaron; si T15 la pide
  en el resultado hay que ampliar la firma y anotarlo en el SPEC.
