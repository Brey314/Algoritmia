# Fase 3 — Nivel fuego: lo construido y sus resultados

**Slice 1 · Golden Path temprano** · Estado: **T12–T16 implementadas y verdes** — corte de este
documento: 11 de septiembre de 2026.
Verificación vigente: **EditMode 84/84 · PlayMode 52/52** (`FirePanelTests` 13/13,
`PauseMenuTests` 4/4, `PauseMenuPolicyTests` 1/1), 0 fallos. *(Vencido el 25/09/2026: son las
cifras al cerrar T16; al cerrar T19 eran EditMode 94/94 · PlayMode 54/54 (§3.8), y la corrida
vigente está en `Slice-4-Resultados.md`.)*
Plan técnico: [`plan.md`](plan.md) · Tablero: [`todo.md`](todo.md) · Contrato: `claudeDocs/SPEC.md`
Fase anterior: [`Fase-2-Resultados.md`](Fase-2-Resultados.md)

La Fase 3 es el módulo `nivel-fuego`: el nivel jugable completo (panel de encendido, iteración,
depuración, convergencia, resolución, indicadores). Este documento se irá llenando tarea a tarea;
el código de la fase **no está completo**.

> **Estado al 25/09/2026:** el texto de abajo es la foto del 11/09 y se conserva tal cual. T17–T19
> también se terminaron ese día (§2e–§2g), aunque la tabla siguiente las deja «pendiente». La
> mecánica que describe —deslizante de posición de tres muescas (`StrikePosition`), ocho mensajes
> por distancia, registro con historial y rótulo «Aún no»— la rehicieron las **Fases 5 y 6** del
> Slice 1 (T20–T27: `fa5d032` del 12/09 y `863ef05` del 15/09, INC-47): hoy el nivel se juega en
> dos momentos, reunir las piezas en un círculo y encender con dos deslizantes, fuerza y cercanía de
> las piedras, y la ayuda está cableada. Lo construido entonces está en
> [`Fase-5-6-Resultados.md`](Fase-5-6-Resultados.md). Además, `4f69140` (11/09) quitó
> `NarrativeOutcome`, la cueva se ilumina con la capa `fx_oscuridad` y no con el tinte `#8A97AB`, y
> el arte, el sonido y los personajes del nivel llegaron después
> ([`Props-y-Sonidos-Resultados.md`](../Slice%203/Props-y-Sonidos-Resultados.md),
> [`Personajes-Resultados.md`](../Personajes/Personajes-Resultados.md)). Cada afirmación que se
> comprobó vencida lleva una nota fechada en su sitio; el resumen está en el **Anexo** al final. Las cifras de
> §3 son las del 10–11/09: la corrida vigente de la suite está en
> [`Slice-4-Resultados.md`](../Slice%204/Slice-4-Resultados.md), «Corrida completa de la suite
> (25/09/2026)».

| Tarea | Qué entrega | Modo de prueba | Estado |
|---|---|---|---|
| T12 | `StrikePosition`, `FireLevelConfig`, `StrikeOutcome`, `FireAttempt` — lógica pura del nivel *(25/09/2026: `StrikePosition` se borró en `fa5d032`)* | EditMode | ✅ terminada |
| T13 | `FireFeedbackLog` y los ocho mensajes del guion §4.3.4 *(25/09/2026: hoy doce; los ocho de fallo, de fuerza y cercanía, ya no son del guion)* | EditMode | ✅ terminada |
| T14 | Panel de encendido y escena `Level1_Cave` | PlayMode | ✅ terminada |
| T15 | Convergencia: «Soplar» → nacimiento del fuego | PlayMode + VV | ✅ terminada |
| T16 | Menú de pausa | EditMode + PlayMode | ✅ terminada |
| T17 | Emisión de los cuatro indicadores del N1 | EditMode | pendiente *(25/09/2026: terminada el 11/09, §2e)* |
| T18 | Resumen de fin de nivel y cierre reflexivo | EditMode + PlayMode | pendiente *(25/09/2026: terminada el 11/09, §2f)* |
| T19 | Iluminación progresiva del escenario (RF-21, Baja) | VV | pendiente *(25/09/2026: terminada el 11/09, §2g)* |

---

## 1. T12 — el corazón del Nivel 1, C# plano

El assembly `Game.Levels.Fire` existía como `.asmdef` vacío desde T01; T12 le pone el primer
código. Cuatro archivos en `Assets/Game/Scripts/Runtime/Levels/Fire/`:

- **`StrikePosition`** — enum de tres distancias del deslizante (guion §4.3.2): `Far`, `Near`,
  `VeryClose`. «Muy cerca» es la única efectiva por defecto. *(Vencido el 25/09/2026: se borró en
  `fa5d032`; hoy la fuerza se clasifica en `ForceBand` y la cercanía de las piedras en
  `SpacingBand` — `Fase-5-6-Resultados.md` §3.1.)*
- **`FireLevelConfig`** — ScriptableObject con los cuatro parámetros ajustables jugando
  (`AvailablePositions`, `EffectivePosition`, `MinimumEffectiveStrikes`, `AttemptsBeforeHint`),
  `[field: SerializeField]` + `[Tooltip]` con el texto del guion, valor por defecto = el propuesto
  (3 / Muy cerca / 3 / 3). Fuera del código para que retocarlos —PG-06 sigue abierto— no cueste
  recompilar (CT-05, RNF-18). **No se crea el `.asset`**: es de T14; los tests usan
  `ScriptableObject.CreateInstance`. *(Vencido el 25/09/2026: hoy son doce parámetros —los de
  fuerza, cercanía, acercamiento e ignición, más `MinimumEffectiveStrikes` y `AttemptsBeforeHint`,
  que siguen de T12— y ninguno de posición; `Fase-5-6-Resultados.md` §3.1 y §3.4.)*
- **`StrikeOutcome`** — `readonly struct` que devuelve un golpe: `Effective`, `Position`,
  `EffectiveStrikes`. Fábricas `SparksDied(position)` / `SparkLanded(effectiveStrikes)` con las
  firmas literales del `SPEC.md` §Estilo. Lo consumirá T13 para elegir el mensaje del log.
  *(Vencido el 25/09/2026: ya no hay `Position`; lleva `Force`, `Band`, `Spacing` y
  `EffectiveStrikes`, con las fábricas `SparksDied(force, band)`, `StonesMisplaced(force, band,
  spacing)` y `SparkLanded(force, n)`.)*
- **`FireAttempt`** — el código del `SPEC.md` §Estilo (líneas 334–365) tal cual, con
  `ConsecutiveFailures` expuesto como propiedad pública de solo lectura (patrón de `HintPolicy`).
  Lleva los dos contadores del guion §4.3.3/§4.3.5:
  - Golpe desde una posición no efectiva → `ConsecutiveFailures++`, devuelve `SparksDied`; **no
    toca los golpes efectivos** (RF-16). *(25/09/2026: hoy «no efectivo» es piedras separadas o
    demasiado encimadas, o fuerza fuera de 7–8, en `Strike(force, spacing)`; lo demás de esta
    lista sigue igual.)*
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
  (`unity-yaml-editing-guide`); el nombre sigue la convención `N1_*` del Nivel 1. *(Vencido el
  25/09/2026: hoy son doce; los ocho de fallo, de fuerza y de cercanía de las piedras, ya no son
  el texto del guion —INC-47—, y los tres de golpe efectivo y el del soplo siguen literales;
  `Fase-5-6-Resultados.md` §3.1 y §3.4.)*
- **`FireFeedbackLog`** — C# plano, sin Unity. `Record(StrikeOutcome, consecutiveFailures)` elige
  el mensaje y lo acumula:
  - golpe efectivo → por ordinal: primero, segundo, o **final** al alcanzar el mínimo (guion
    §4.3.5 E6, el «hilo de humo»);
  - golpe no efectivo → por distancia (`Far`/`Near`) y racha: la variante «tras dos intentos» del
    guion §4.3.4 entra desde el **segundo** fallo seguido; *(vencido el 25/09/2026: hoy se elige
    primero por cercanía de las piedras y, si no, por franja de fuerza; la racha sigue igual)*
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
  Settings (**6 escenas**; *hoy doce, 25/09/2026*). Canvas Overlay 1920×1080; `EventSystem` con `InputSystemUIInputModule`
  (nunca el legado, CT-06); deslizante de tres posiciones (`Navigation.None`), «Golpear», «Soplar»
  atenuado con **badge candado + «Aún no»** (RNF-19), área de registro con `ScrollRect`, montón de
  hojas. Arte **placeholder** (A7–A9 sin generar). *(Vencido el 25/09/2026: desde `fa5d032` no hay
  `ScrollRect` y el deslizante es de fuerza, de diez muescas; `863ef05` sumó el de cercanía y quitó
  el rótulo «Aún no» —queda el candado—. `Fase-5-6-Resultados.md` §3.3.)*
- **`FirePanelController`** (`Game.Levels.Fire`, con `UnityEngine.UI` añadido al `.asmdef`) —
  adaptador delgado: el clic de «Golpear» llama a `FireAttempt.Strike` (T12), el resultado a
  `FireFeedbackLog.Record` (T13), y el estado a la UI. **Cero reglas del juego.** El deslizante no
  produce efecto hasta «Golpear» (RF-15). «Soplar» se habilita cuando `CanBlow` y su badge se
  oculta; no re-bloquea (INC-32). «Soplar» hoy solo registra el mensaje del soplo — la animación,
  el guardado y el salto a la escena de cierre son T15.
- **`FeedbackLogView`** — vista tonta que une `Entries` con saltos de línea y baja el scroll.
  *(Vencido el 25/09/2026: desde `fa5d032` muestra solo el último mensaje, en la tablilla superior
  del mockup 7; el historial sigue en `FireFeedbackLog.Entries`.)*
  Desviación del plan: vive en `Game.Levels.Fire`, no en `Game.UI` (así `Game.UI` no depende de
  ningún nivel y `Game.Levels.Fire` no depende de `Game.UI`).
- **`ControlesJugables.inputactions`** (nuevo) — mapa `UI` con **solo** bindings de puntero
  (`<Pointer>`, `<Mouse>`, `<Touchscreen>`, `<Pen>`): sin Navigate/Submit/Cancel, sin
  `<Keyboard>`/`<Gamepad>`. Es el `actionsAsset` del `EventSystem` de la escena jugable — la
  verificación de RNF-02 barre sus bindings. Las escenas de menú/narrativa quedan como están:
  RNF-02 acota «escenas **jugables**».
- **`GameFlowRunner.Scenes`** mapea `Playing → Level1_Cave` (un solo nivel hoy; por `LevelId`
  cuando lleguen Rueda/Río). **`NarrativeSceneController.Leave()`** entra a jugar el nivel de la
  secuencia (`StartPlaying`), cerrando el `// PROVISIONAL (T14)`. *(Vencido el 25/09/2026: desde
  `4f69140` la escena de `Playing` la elige la fase —`GameFlowRunner.PlayingScenes`— y `Leave()`
  entra a jugar por el `NextPhase` del asset.)*
- **`N1_Config.asset`** — el `FireLevelConfig` real (3 / Muy cerca / 3 / 3), YAML a mano.
  *(Vencido el 25/09/2026: hoy lleva doce valores —fuerza, cercanía, acercamiento, ignición y los
  dos de T12 que no cambiaron, 3 y 3—; `Fase-5-6-Resultados.md` §3.4.)*

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
  iluminación del resto del escenario son arte pendiente / T19. *(Vencido el 25/09/2026: desde
  `aca7acc`/`8ec5120` el nacimiento del fuego es la llama cenital animada sobre el montón y un
  quemado que crece en `IgnitionSeconds`; sigue siendo un solo barrido. Props-y-Sonidos §2.5.)*
- **`CompleteLevel`** — sin perfil activo, avisa y no hace nada (mismo criterio que `ScreenFlow`,
  sin acoplar `Game.Levels.Fire` a `Game.UI`). Con perfil: `PlayerProfile.ConfirmPhase(Fire, 1,
  default)` (**los cuatro indicadores reales son T17** — de momento se confirma la fase sin
  ellos, mismo patrón que T11 dejó el umbral de pista para T12), `LevelUnlockPolicy.UnlockAfterCompleting`
  (Nivel 2, RF-03), `(Saver ?? Runner.Session).SaveActive()` (RF-04, con el mismo patrón de
  `IProfileSaver` inyectable que `MainMenuController.Saver`, para que las pruebas usen un espía y
  no toquen disco), y `Runner.StartNarrative("N1_NacimientoDelFuego")` (RF-20). *(Vencido el
  25/09/2026: desde T18 `CompleteLevel` solo deja los indicadores y abre la narrativa; confirmar,
  desbloquear y guardar los hace `LevelSummaryController.Show()` (§2f), y
  `ConfirmPhase` recibe un `PhaseId` —firma de W02, `a012515`, que el N1 adoptó en `4f69140`—.)*
- **`NarrativeOutcome`** (nuevo, `Game.Scaffolding`): `EntersLevel` / `ReturnsToLevelSelect`.
  `NarrativeSceneController.Leave()` (T14) asumía que **toda** narrativa entra a jugar; con la de
  cierre del N1 eso ya no vale. La secuencia declara su propósito en el asset —sin `if` por
  secuencia, la regla del proyecto se mantiene— y `Leave()` decide por ese campo. Valor por
  defecto (`EntersLevel = 0`) igual al comportamiento anterior: **los tres assets existentes no
  se tocaron**. *(Vencido el 25/09/2026: `4f69140` (11/09) eliminó `NarrativeOutcome` al
  reconciliar con el Slice 2; hoy `Leave()` prueba en orden `NextSequenceId` → `NextPhase` →
  `EndsInCredits` → `IsReflectiveClosing` → menú de niveles.)*
- **`N1_NacimientoDelFuego.asset`** (nuevo) — las 19 líneas del guion §4.4 (el nacimiento del
  fuego y el cierre reflexivo de Chispa, «se llama iterar»), añadido como cuarta entrada al
  arreglo `sequences` de `Narrative.unity` — un cambio de una sola línea en la escena, exactamente
  la promesa de T09/T10: «añadir una escena narrativa es crear un asset». *(Vencido el
  25/09/2026: T20 (`fa5d032`) lo reescribió con 17 líneas y el guía habla como ALGORITM, no
  «Chispa» (INC-44); el cierre sigue diciendo «se llama iterar». `Fase-5-6-Resultados.md` §3.5.)*

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

## 2d. T16 — el menú de pausa

**Sin tocar `Panel`/`FirePanelController`.** «Pausa» abre un overlay sobre `Playing` —no un estado
nuevo (HU-17)— con Continuar, Reiniciar (con confirmación de una frase) y Volver al menú:
*(Vencido el 25/09/2026: desde W17 del Slice 2 (`8de614f`, 15/09) es el menú del mockup 6
—«Reanudar», «Reiniciar» y «Volver al menú de niveles» (`MenuPausa.prefab:1520`, `:2752`, `:924`)—,
que sale a `LevelSelect` y no al menú principal (INC-49), y detiene el nivel con
`Time.timeScale = 0` mientras está abierto (`PauseMenuController.cs:68`).)*

- **Hallazgo de diseño previo a cualquier código**: `GameFlowRunner.Apply` solo recargaba una
  escena cuando su nombre **cambiaba**. `GameFlow.Allowed[Playing]` ya incluía `Playing` como
  destino legal desde T03, con el comentario «Playing → Playing es reiniciar el nivel o entrar a
  la fase siguiente (RF-07)» — pero como el nombre de escena no cambia al reentrar, la recarga
  real nunca ocurría: la FSM aceptaba la transición y no pasaba nada. Arreglado para que reentrar
  a `Playing` **siempre** recargue, sea cual sea el nombre; sin efecto sobre ningún otro estado
  (`Playing` es el único que se tiene a sí mismo como destino legal).
- **`PauseMenuPolicy.Restart(GameFlowRunner)`** (`Game.Core`) repite `runner.StartPlaying` con el
  mismo nivel y fase — nunca re-bloquea un nivel ni borra un indicador confirmado (RF-41, CP-02):
  esos solo cambian con `Reach`/`ConfirmPhase`, que ni esto ni `TryStartPlaying` llaman.
- **`PauseMenuController`** (`Game.UI`, no `Game.Levels.Fire`: navegación general como
  `LevelSelectController`/`NarrativeSceneController`, sin dependencia nueva para el nivel) vive en
  `Level1_Cave.unity` como una capa sobre el panel de T14. Un `Image` a pantalla completa
  (`raycastTarget = true`, último hermano del `Canvas`) bloquea el clic hacia «Golpear»/«Soplar»
  mientras está pausado — el panel de T14 no se toca en absoluto. *(Vencido el 25/09/2026: desde
  `8de614f` el controlador vive en el prefab `MenuPausa.prefab`, instanciado en las cinco escenas
  jugables; `Level1_Cave.unity` lleva esa instancia.)*

### Dos bugs reales encontrados al verificar (no ruido del Editor)

**#1 — el componente vivía en un GameObject que arrancaba inactivo.** `PauseMenuController` se
puso primero en `PanelPausa` (el overlay, oculto hasta que se abre la pausa). Unity **nunca llama
`Awake()`/`Start()`** en un componente de un GameObject inactivo al cargar la escena, así que el
listener de `pauseButton.onClick` nunca se registraba y el botón «Pausa» no hacía nada — 3
pruebas de comportamiento fallaron con el mismo síntoma en 2 corridas idénticas de código ya
compilado (descartado como *staleness* del Editor: los `Domain Reload Profiling` del log
confirman que sí recompiló entre corridas). Arreglo: mover el componente al botón «Pausa»
(siempre activo), sin tocar sus 9 referencias serializadas.

**#2 — `PauseMenuPolicy` llamaba a `GameFlow` directo, saltándose la recarga.** La primera versión
tomaba un `GameFlow` y llamaba `flow.TryStartPlaying(...)` sin pasar por
`GameFlowRunner.Apply()` — el único sitio que traduce una transición a una recarga de escena.
Ningún otro controlador del proyecto llama a métodos de `GameFlow` directamente; todos pasan por
los wrappers de `GameFlowRunner`. Arreglo: `PauseMenuPolicy.Restart` pasa a tomar `GameFlowRunner`
y llama `runner.StartPlaying(...)`. Su prueba EditMode instancia un `GameFlowRunner` desnudo (sin
escena, sin `SceneLoader`, sin fotogramas) — la primera prueba EditMode del proyecto que toca un
`MonoBehaviour`, justificada porque «Reiniciar» es intrínsecamente una recarga real, no solo un
cambio de estado puro.

## 2e. T17 — los cuatro indicadores del N1

**`CompleteLevel()` ya no confirma con `default`.** `ILevelReporter` (`Game.Core`, dos métodos:
`PauseOpened`/`PauseClosed`) + `FireIndicatorCollector` (`Game.Levels.Fire`, la implementa):

- **Intentos** = golpes ejecutados desde una posición no efectiva, uno por uno.
- **Errores corregidos** = un golpe efectivo que llega justo después de uno que no lo fue. No hace
  falta comparar `StrikePosition` aparte: la posición efectiva es única, así que cualquier golpe
  fallido está, por construcción, en una posición distinta a la del acierto que lo corrige.
  *(Vencido el 25/09/2026 en su justificación: `StrikePosition` ya no existe, la fuerza efectiva
  son dos muescas (7–8) y un fallo puede ser de cercanía. El cálculo no cambió y la prueba se llama
  `…ErrorCorregidoExigeCambioDeFuerzaSeguidoDeAcierto`; el comentario de
  `FireIndicatorCollector.cs:54-57` todavía repite este razonamiento.)*
- **Pasos utilizados** = golpes efectivos acumulados al cruzar `MinimumEffectiveStrikes`; se
  congela ahí, golpes efectivos posteriores no lo mueven.
- **Tiempo de resolución** = un reloj inyectado (`Func<float>`, `Time.realtimeSinceStartup` en
  producción) menos la ventana con la pausa abierta — determinista en prueba sin `Thread.Sleep`.

**La mediación de la pausa pasa por `Game.Core`, no por una referencia nueva entre assemblies.**
Solo `PauseMenuController` (`Game.UI`) sabe cuándo se abre/cierra la pausa, y solo
`Game.Levels.Fire` puede medir el tiempo del nivel — pero ninguna de las dos direcciones de
dependencia entre esos dos assemblies está permitida. `GameFlowRunner.ActiveReporter`
(`ILevelReporter`, en Core) es el punto de acceso ya compartido: `FirePanelController.Start()` lo
asigna a su `FireIndicatorCollector`, y `PauseMenuController.OpenPause()`/`ClosePause()` lo
notifican — mismo patrón que `IProfileSaver`, aplicado aquí a una interfaz de notificación en vez
de una de guardado.

Sin bugs de producción esta vez (a diferencia de T16): el diseño salió correcto en el skeleton, y
las seis pruebas de `FireIndicatorTests` pasaron en verde desde la primera corrida — deviación
deliberada del rojo-primero, documentada y verificada a mano línea por línea contra la aritmética
esperada (ver plan, sección Implementation Notes) para no aceptar un verde vacío.

**Bloqueo de entorno, no de código**: el puente MCP de Rider (`get_unity_compilation_result`,
`unity_play_control`) estuvo en timeout total ~13 minutos con el patrón ya documentado (bucle de
`[Licensing::Client] Error: Code 404` en `Editor.log`, sin línea de finalización en `idea.log` para
`calling Rd model.getCompilationResult.startSuspending`). A diferencia de la vez anterior (memoria
de sesión, 07/09/2026), **dos reinicios completos del Editor no lo arreglaron** — el Editor mismo
reportaba `ready` por `unity status` (CLI) ambas veces, así que el problema está del lado del
puente RD de Rider, no de Unity. Se resolvió solo tras esperar; no se reinició Rider.

## 2f. T18 — el resumen de fin de nivel y el cierre reflexivo

**`GameState.LevelSummary` (declarado desde T03) por fin tiene escena y controlador.** HU-14
(líneas 590-612 de `Historias_de_Usuario_HU01_HU18_v2 (1).md`) resultó más específica que el plan
de fase y lo gobernó: el flujo básico ordena completar el reto → animación → **escena narrativa de
cierre** (el guía nombra la habilidad, la liga a lo hecho) → **resumen narrativo sin cifras** →
vuelve al menú con el siguiente nivel desbloqueado. `N1_NacimientoDelFuego.asset` ya traía el
cierre reflexivo escrito desde antes de T15 («Eso tiene nombre: se llama iterar. Probar, mirar el
resultado y ajustar.», ligado a «Cambiaste de lugar, volviste a probar…») — RF-12 quedó
satisfecho por contenido ya existente, sin código nuevo. *(25/09/2026: la línea dice hoy
«Cambiaste la fuerza, volviste a probar…»; «se llama iterar» sigue. El recuadro del resumen, desde
`fa5d032`, nombra la habilidad como «probar y ajustar» —`SkillNamed`—.)*

**`LevelSummaryComposer`** (`Game.UI`, C# plano) compone el resumen a partir de
`PerformanceIndicators`: dos de los cuatro indicadores (Intentos, Errores corregidos) se traducen
a una de dos variantes de texto cada uno desde `LevelSummaryMessages` (SO); Pasos utilizados y
Tiempo de resolución no tienen una forma narrativa natural sin sonar a cifra disfrazada. Ninguna
rama formatea un número — «cero cifras» es estructural, no un cuidado en tiempo de ejecución.
`LevelSummaryController` pinta ese texto y, al continuar, confirma la fase, desbloquea el Nivel 2
y guarda. *(Corrección del 25/09/2026: confirma, desbloquea y guarda en `Show()`, al mostrarse
—como dice el arreglo más abajo y como está desde T18—, no al continuar. La tablilla de los mockups
13 y 13b es de `fa5d032`: `Fase-5-6-Resultados.md` §3.6.)*

### El bug real: el cierre reflexivo nunca era «la primera vez»

`FirePanelController.CompleteLevel()` (T15) confirmaba la fase **antes** de entrar a la narrativa
de cierre. Como el Nivel 1 tiene una sola fase, y `NarrativeVisitPolicy.AlreadySeen` decide si
ofrecer «Omitir» mirando si el perfil **ya tiene alguna fase del nivel confirmada**, esa
confirmación —que acababa de pasar un par de líneas antes— hacía que `AlreadySeen` diera
verdadero **desde la primerísima vez** que se completaba el nivel: el botón de omitir aparecía
siempre, violando CP-07 y el flujo alterno FA-02 de HU-14. Ninguna prueba existente lo atrapaba:
`FirePanelTests` no llegaba a mirar la escena narrativa, y `NarrativeSceneTests` arrancaba
`StartNarrative` directo con un perfil sin tocar, sin pasar por `CompleteLevel`. Un defecto real de
T15, dormido hasta que T18 miró el flujo completo. *(Vencido el 25/09/2026 en cuanto a la regla:
hoy el cierre reflexivo mira `ReachedLevel` (entró con W04, `9f10485`, en el carril del Slice 2) y
las demás escenas, la **última** fase confirmada (`d81cfc7`, 17/09). En el N1, de una sola fase,
«alguna» y «la última» coinciden. El orden que impone este arreglo sigue vigente.)*

**Arreglo, siguiendo HU-14 al pie de la letra**: `ConfirmPhase`/`LevelUnlockPolicy`/`SaveActive` se
movieron del paso 2 (donde estaban) al paso 6 (`LevelSummaryController.Show()`), justo donde el
flujo básico los sitúa. La mediación es `GameFlowRunner.PendingIndicators` (mismo patrón que
`ActiveReporter`, T17): `FirePanelController.CompleteLevel()` ahora solo calcula los indicadores y
los deja listos, sin confirmar nada. `NarrativeSceneController.Leave()` ganó una rama: las
secuencias de cierre (`Outcome == ReturnsToLevelSelect`) van a `LevelSummary`, no directo a
`LevelSelect` — sigue sin un `if` por secuencia, la rama depende del dato del asset. *(Vencido el
25/09/2026: desde `4f69140` la marca es `IsReflectiveClosing`.)*

Reordenar solo las *líneas* dentro de `CompleteLevel()` no habría alcanzado: la carga de escena es
asíncrona, así que el resto del método habría terminado de correr antes de que la narrativa
llegara a leer el perfil. Una bandera «esta es la primera vez, créeme» en `NarrativeVisitPolicy`
habría reintroducido el registro de «escenas vistas» que su propio comentario dice que no puede
existir (RNF-09, lista cerrada). Mover el efecto secundario al punto del flujo donde HU-14 ya lo
sitúa fue el cambio más chico que arregló la causa, no el síntoma.

**Costo del arreglo**: dos pruebas PlayMode de T15 (`FireLevel_RF04_GuardaAlCompletarLaFase`,
`FireLevel_RF03_DesbloqueaElNivel2`) verificaban ese efecto justo en `CompleteLevel()`; se
trasladaron a `LevelSummaryTests`, donde el efecto ahora vive de verdad.

### Un segundo hallazgo, en la regresión: la prueba de exclusión de T17 era demasiado estricta

`FireIndicators_CP03_NingunIndicadorLlegaALaUIDelEstudiante` (T17) prohibía `PerformanceIndicators`
en cualquier forma dentro de `Game.UI` — correcto cuando se escribió, porque entonces `Game.UI` no
tenía ningún motivo legítimo para tocarlo. `LevelSummaryComposer.Compose(messages, indicators)`
(T18) sí lo necesita, a propósito, como parámetro de entrada para componer el resumen sin cifras.
Arreglo: la prueba ahora distingue lo prohibido **en cualquier forma** (`ILevelReporter`,
`FireIndicatorCollector` — `Game.UI` no tiene motivo para conocerlos) de lo prohibido **salvo como
parámetro** (`PerformanceIndicators` — consumirlo para producir texto sin cifras es justo lo que
CP-03 permite; lo que prohíbe es que un dígito llegue a pantalla, garantía que cubre
`LevelSummary_RF45_NoContieneNingunDigito`).

## 2g. T19 — iluminación progresiva del escenario

**`CaveLightingController` (nuevo, sobre el `Fondo` del panel ya existente, sin GameObjects
nuevos).** Interpola el color entre un tono oscuro y el original del `Fondo` según
`golpesEfectivos / (mínimo + 1)`: un escalón por golpe efectivo (guion E4), sin llegar nunca al
máximo hasta la resolución (E7) — el `+1` reserva ese último escalón para la ignición, que ahora
anima la iluminación en el mismo barrido de color que ya animaba el nacimiento del fuego
(`PlayIgnitionAsync`, T15), sin temporizador ni animación nueva.

*(Vencido el 25/09/2026: desde `fa5d032` `CaveLightingController` ya no interpola el color del
`Fondo`: crea una capa «Oscuridad» con el material `fx_oscuridad` y anima un `NarrativeLight`
—charco fijo sobre el montón, fondo de 0,10 a 0,30—, con la misma fracción
`golpesEfectivos / (mínimo + 1)`. El `#8A97AB` de abajo ya no está en el código, y la prueba de
RNF-20 mide la instrucción contra su tablilla, que la oscuridad no toca, no contra la cueva.
`Fase-5-6-Resultados.md` §3.2 y §4, decisión 7.)*

### El ajuste de RNF-20: el color más oscuro no es el del documento de arte

`Direccion_de_Arte.md` §8.1 prescribe una máscara `#0F1526` al 65 % de opacidad sobre el fondo del
panel. Aplicado sobre `Fondo` (`#F7EFE2`, el mismo par que certifica RNF-20 con el texto
`#3A1E18`), ese valor da un contraste texto/fondo de **~1.2:1** — muy por debajo del 4.5:1 que
RNF-20 exige «en el estado más oscuro», uno de los cuatro criterios de aceptación de la tarea.
Documento de arte es «subordinado a SPEC.md, no introduce mecánicas»; RNF-20 es un requisito duro
con su propia prueba. Se ajustó el color más oscuro a `#8A97AB` (contraste calculado ≈5.14:1) —
tratamiento igual al que ya reciben los valores de `FireLevelConfig`/`FireMessages` (PG-06,
pendientes de revisión de contenido, no de código). La prueba de RNF-20 calcula el contraste real
contra el color efectivamente aplicado (fórmula WCAG de luminancia relativa) en vez de confiar en
un valor fijado de antemano: verificado mediante mutación deliberada durante Test First que el
`#0F1526` literal sí hace fallar la prueba (≈1.2:1 calculado).

La tabla de opacidad de cuatro anclas del documento (65/45/25/0 %) tampoco encaja con
`MinimumEffectiveStrikes = 3` del `N1_Config` actual — es ilustrativa, no literal. Se siguió el
mecanismo que describe (un color de fondo interpolado) con la fracción propia del plan en vez de
sus anclas numéricas.

---

## 3. Verificación — declarada

*(Vencido el 25/09/2026 en parte: trece de los nombres de estas tablas ya no existen —once los
renombró o sustituyó la mecánica de fuerza y cercanía, y dos los trasladó T18 a `LevelSummaryTests`
(§2f)—. La equivalencia está en el Anexo A.2 y las clases de
hoy, con su conteo, en `Fase-5-6-Resultados.md` §5.)*

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
color— y se corrigió reparentando el badge fuera del botón (§2b). *(Vencido el 25/09/2026: el
rótulo «Aún no» se retiró en `863ef05`; queda el candado, que con el color atenuado sigue siendo
doble indicador —`FirePanel_RNF19_SoplarAtenuadoSeDistinguePorElCandadoAdemasDelColorYSinRotuloAunNo`—.)*

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

### 3.5 EditMode + PlayMode — T16

**`PauseMenuPolicyTests` 1/1, `PauseMenuTests` 4/4, 0 fallos.** **EditMode total: 84/84 · PlayMode
total: 52/52** — 2 corridas limpias consecutivas.

| Prueba | Requisito |
|---|---|
| `PauseMenuPolicy_RF07_ReiniciarNoRebloqueaNiBorraIndicadores` (EditMode) | RF-07, RF-41, CP-02 |
| `PauseMenu_HU17_ContinuarRestituyeElEstadoExacto` | HU-17 |
| `PauseMenu_HU17_ReiniciarPideConfirmacionYCancelarNoCambiaNada` | HU-17 FA-01, FA-02 |
| `PauseMenu_HU17_ConfirmarReiniciarReinicioElIntentoSinTocarElProgreso` | RF-03, RF-04, RF-07 |
| `PauseMenu_HU17_NoSeMuestraEnEscenasNarrativas` | HU-17 FA-04 |

### 3.6 EditMode — T17

**`FireIndicatorTests` 6/6, 0 fallos.** **EditMode total: 90/90 · PlayMode total: 52/52** — 2
corridas EditMode limpias; PlayMode tuvo 1 fallo intermitente ajeno a T17 en una de tres corridas
(§3.8).

| Prueba | Requisito |
|---|---|
| `FireIndicators_RF45_IntentosCuentaSoloGolpesNoEfectivos` | RF-45 |
| `FireIndicators_RF45_ErrorCorregidoExigeCambioDePosicionSeguidoDeAcierto` | RF-45 |
| `FireIndicators_RF07_LaPausaNoSumaTiempoDeResolucion` | RF-07, RF-45 |
| `FireIndicators_CP03_NingunIndicadorLlegaALaUIDelEstudiante` | CP-03 |
| `FireIndicators_RF45_PasosUtilizadosSeCongelaAlCruzarElMinimo` | RF-45 |
| `FireIndicators_RF45_PerformanceIndicatorsExponeExactamenteLosCuatroCampos` | RNF-09 |

### 3.7 EditMode + PlayMode — T18

**`LevelSummaryComposerTests` 4/4, `LevelSummaryTests` 2/2, 0 fallos.** **EditMode total: 94/94 ·
PlayMode total: 52/52** (Fire −2 por el traslado, UI +2 por `LevelSummaryTests`: mismo total que
T17) — 2 corridas limpias consecutivas de cada suite.

| Prueba | Requisito |
|---|---|
| `LevelSummary_RF45_NoContieneNingunDigito` (EditMode) | RF-45, CP-03 |
| `LevelSummary_RF12_NombraLaHabilidadEjercitada` (EditMode) | RF-12 |
| `LevelSummaryComposer_RF45_SinErroresCorregidosNoUsaLaVarianteDeCambioDeEstrategia` (EditMode) | RF-45 |
| `LevelSummaryComposer_RF45_SinIntentosFallidosNoUsaLaVarianteDeVariasPosiciones` (EditMode) | RF-45 |
| `LevelSummary_RF03_DevuelveAlMenuConNivel2Desbloqueado` (PlayMode) | RF-03, RF-04, HU-14 |
| `LevelSummary_CP07_ElCierreReflexivoNoEsOmitibleLaPrimeraVez` (PlayMode) | CP-07 — regresión directa del bug de §2f |

### 3.8 PlayMode — T19

**`CaveLightingTests` 2/2, 0 fallos.** **EditMode total: 94/94 (sin cambios, T19 no toca EditMode) ·
PlayMode total: 54/54** — 2 corridas limpias de 4 (la flakiness preexistente de `FirePanelTests`,
§2c/§3.9, se disparó en las otras 2, siempre en pruebas ajenas a T19).

| Prueba | Requisito |
|---|---|
| `FireLevel_RF21_IluminacionSubeUnEscalonPorGolpeEfectivo` | RF-21, RNF-21 |
| `FirePanel_RNF20_ContrasteSuficienteEnElEstadoMasOscuro` | RNF-20 |

### 3.9 Flujo test-first

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
- **T16 RED: 3 fail / 1 pass** (de 4 PlayMode: la de ausencia en `Narrative` nace verde, sin
  código de producción que la condicione). `PauseMenuPolicyTests` nace verde también —
  `PauseMenuPolicy.Restart` no era un esqueleto vacío, era la implementación real desde el
  principio (una sola expresión). **GREEN: 4/4 PlayMode + 1/1 EditMode**, pero solo tras dos
  vueltas de arreglo (§2d): la primera implementación de los 6 métodos de `PauseMenuController`
  seguía en rojo porque el componente vivía en un GameObject inactivo (bug #1) y, tras moverlo,
  una tercera prueba seguía en rojo porque `PauseMenuPolicy` no pasaba por `GameFlowRunner.Apply`
  (bug #2). Dos rondas de diagnóstico y arreglo antes del verde real.
- **T17 GREEN desde la primera corrida, sin rojo previo** — `FireIndicatorCollector` se implementó
  completo en el skeleton (Step 1), mismo precedente que `PauseMenuPolicy` en T16: el cálculo
  queda fijado por el diseño del plan y stubearlo solo habría significado reescribirlo en Step 3.
  Se verificó a mano que cada aserción de `FireIndicatorTests` es sensible a una implementación
  incorrecta (p. ej. RF-07: `_startedAt=0, _pausedAt=10, _pausedSeconds=5` tras cerrar, `Complete()`
  en `t=20` → `20-0-5=15`, el valor exacto que afirma la prueba) para no aceptar un verde vacío.
- **T18**: `LevelSummaryComposer`/`LevelSummaryController` también se implementaron completos en
  el skeleton (mismo precedente que T16/T17), así que `LevelSummaryComposerTests` nació verde. Las
  dos pruebas de `LevelSummaryTests` (PlayMode) ejercitan el flujo real recién cableado —
  `LevelSummary_CP07_...` es, además, la prueba de regresión directa del bug de §2f: conduce el
  flujo real de `CompleteLevel()` → escena narrativa y comprueba el botón de omitir, así que un
  reordenamiento futuro que reintroduzca el bug la haría fallar.
- **T19 GREEN desde la primera corrida, sin rojo previo** — mismo precedente que T16/T17/T18:
  `CaveLightingController` se implementó completo en el skeleton. A diferencia de tareas
  anteriores, aquí la verificación de que cada aserción es sensible a una implementación
  incorrecta se hizo con **mutación deliberada**, no solo trazado a mano: se cambió temporalmente
  el divisor de `RefreshLighting()` de `mínimo + 1` a `mínimo` (simulando que converger ya
  ilumina al máximo) y se quitaron las dos líneas de `lighting?.SetProgress(...)` de
  `PlayIgnitionAsync` (simulando que la ignición nunca ilumina) — ambos mutantes hicieron fallar
  `FireLevel_RF21_...` exactamente donde se esperaba, revertidos después y confirmados limpios de
  nuevo. Para RNF-20, se verificó a mano (no en el motor) que sustituir `darkest` por el `#0F1526`
  literal del documento de arte da ≈1.2:1 calculado, muy por debajo del umbral — confirma que la
  prueba depende del color real, no de un valor de confianza.

### 3.10 Notas

- **El MCP de Rider respondió la mayor parte de la sesión** (`get_unity_compilation_result` y
  `run_unity_tests` contra el Editor abierto). R1 deja de bloquear el flujo test-first mientras
  Rider siga abierto; `unity test` (CLI) queda de respaldo. **T14 y T15 costaron varias corridas**
  por el cuelgue intermitente del Test Runner (§2b) — en T15 el Editor llegó a cerrarse solo y se
  reabrió con `execute_run_configuration("Start Unity")` de Rider. **T16 no tuvo cuelgues del
  Editor**, pero sí dos rondas de diagnóstico de bugs reales de producción (§2d). **T17 tuvo el
  peor cuelgue de la fase**: el puente MCP de Rider estuvo ~13 minutos en timeout total pese a dos
  reinicios completos del Editor (que sí levantó limpio cada vez, confirmado por `unity status`) —
  se resolvió solo, esperando, sin tocar Rider. Ver §2e y la memoria de sesión. **T18 lo repitió
  una segunda vez** durante el Step 3: el subagente `failing-test-writer` quedó bloqueado sin
  poder correr ninguna prueba (mismo bucle de `Error: Code 404`) y lo reportó como `STATUS:
  BLOCKED` en vez de forzar un resultado — correcto, en vez de fingir un verde o un rojo sin
  haber corrido nada. Esta vez un reinicio del Editor sí lo resolvió, pero el reinicio mismo
  tardó ~9 minutos en volver a «ready» (vs. ~3 min en T17), con el proceso aparentemente
  atascado un rato en «Opening project…» antes de arrancar de verdad — sigue sin causa raíz
  confirmada. **T19 no tuvo cuelgues del Editor.**
- **PlayMode no se re-corrió** en T12 ni T13: lógica pura en un assembly aislado. T14, T15, T16,
  T17, T18 y T19 sí son PlayMode y de ahí la regresión completa.
- **Compilación sin warnings nuevos** (solo los preexistentes de `Game.Audio` vacío y el falso
  positivo de «namespace no corresponde a la ubicación» en archivos de `Game.Core`, ya presente
  antes de T17). Rider no incluye los archivos nuevos en la solución, así que `/resolve-diagnostics`
  no corrió en ninguna de las ocho tareas; la comprobación vinculante de «0 warnings» es la de
  Unity.
- **La flakiness de `FirePanelTests` (§2c) subió de frecuencia en T19**: 1 fallo en tres corridas
  en T17, ninguno en T18, pero **2 de 4 corridas completas con 2-3 fallos** en T19 — siempre las
  mismas tres pruebas preexistentes de T15 (`FirePanel_RF16_...`, `FirePanel_RF19_...`,
  `FirePanel_RF17_...`), nunca las dos nuevas de T19. Mismo síntoma de siempre (contenido de una
  prueba anterior colándose antes de que termine de limpiarse la escena); ninguno de los cambios
  de T19 toca `FireFeedbackLog` ni el registro, así que no parece causal. Las corridas limpias
  (54/54, dos veces) bastan para dar T19 por bueno, pero la frecuencia más alta queda anotada para
  revisar antes de Slice 2 en vez de seguir postergándola tarea tras tarea.

---

## 4. Lo que queda abierto

- **PG-06 / R2** — los valores de `FireLevelConfig` (Muy cerca, 3, 3), el texto de los ocho
  mensajes, el resumen de `LevelSummaryMessages` (T18) y el color más oscuro de
  `CaveLightingController` (T19, `#8A97AB` — ajustado por RNF-20, no el `#0F1526` del documento de
  arte) son primer borrador y se validan jugando en el Checkpoint D. *(Vencido el 25/09/2026:
  los valores, los ocho mensajes de fallo y la luz son otros —`Fase-5-6-Resultados.md` §3.4 y §4,
  decisión 7—; PG-06 sigue abierto.)*
- **Botón de ayuda (`HintPolicy`) sin cablear** — el panel y el registro existen desde T14, pero
  **no** el botón de ayuda: `FireAttempt.ShouldOfferHint` y `HintPolicy` cuentan lo mismo por
  caminos distintos y hay que decidir cuál lo alimenta. Sigue sin resolverse tras T19 (fuera del
  alcance de todas las tareas de esta fase: RF-13, ninguna de ellas lo traza); queda para una
  tarea aparte. *(Vencido el 25/09/2026: cableado en `fa5d032`; lo alimenta `HintPolicy` con los
  pasos `Reunir`, `Golpear` y `Soplar` de `N1_Guia`, y `FireAttempt.ShouldOfferHint` quedó sin
  uso. `Fase-5-6-Resultados.md` §4, decisión 6.)*
- **Arte del Nivel 1 (A7–A9)** — la escena `Level1_Cave` va con placeholders: montón de hojas =
  elipse blanca, badge y registro con formas planas, el «fuego» de T15 es un barrido de color sin
  sprite propio. El *mecanismo* de iluminación progresiva ya existe (T19); los cuatro estados
  reales del montón, el sprite del fuego y el arte real de la cueva siguen pendientes.
  *(Vencido el 25/09/2026: la cueva cenital es de `fa5d032`; los sprites definitivos parciales, el
  montón cenital y el quemado, de `2cbe287`, `aca7acc` y `8ec5120`, y la llama animada, de los
  clips de `dc51804` que `aca7acc` puso en la escena —Props-y-Sonidos—.)*
- **El encadenado de las tres secuencias narrativas del N1 (§4.1/§4.2) antes del panel** sigue sin
  resolverse — `NarrativeSceneController.Leave()` entra al nivel tras **cualquier** secuencia de
  apertura del N1 (`Outcome = EntersLevel`), no específicamente tras `N1_Hallazgo`. *(Vencido el
  25/09/2026: resuelto en `1009c5a` (13/09) por el asset: `N1_Apertura` → `N1_AparicionGuia` →
  `N1_Hallazgo` con `NextSequenceId`, y `N1_Hallazgo` entra a jugar con `NextPhase` 1.)*
- **Cuelgue intermitente del Test Runner** — el plugin Coplay reañade su botón de toolbar durante
  `TestJobRunner.ExecuteCallback` y ahí el corredor se traba a veces; en T15 llegó a cerrar el
  Editor por completo. Se recupera reiniciando (`execute_run_configuration("Start Unity")` de
  Rider) o esperando. No lo arregla el parche de `PlayFromBoot`, que resuelve un problema distinto
  (`playModeStartScene` secuestrando la entrada a Play).
- **`ProjectSettings.asset` autoañade `SENTIS_ANALYTICS_ENABLED`** en cada reimport (revertido
  varias veces en T14, T15, T17, T18 y T19; no reapareció en T16). Toca a RNF-08 «sin
  telemetría» — decisión del usuario.
- **Flakiness de `FirePanelTests` por reutilización de controlador entre pruebas** —
  `FindAnyObjectByType` puede devolver el `FirePanelController` de la prueba anterior si la
  escena previa no ha terminado de destruirse; el fotograma extra en `LoadPanel()` (T15, §2c) lo
  mitiga pero no lo elimina. Frecuencia observada: 1/3 corridas en T17, 0/2 en T18, **2/4 en
  T19** — sube y baja sin patrón claro entre tareas que no tocan ese código, señal de que es un
  problema de temporización del Editor/Test Runner y no de la lógica de la prueba. Nunca afectó a
  una prueba de la tarea en curso, siempre a pruebas preexistentes de T15. Vale la pena
  investigarlo a fondo antes de Slice 2 si sigue subiendo.
- **`PauseMenuPolicy` depende de `GameFlowRunner`** (un `MonoBehaviour`), a diferencia de
  `LevelUnlockPolicy` (puro sobre `PlayerProfile`) — asimetría deliberada, «Reiniciar» necesita el
  efecto secundario real de recarga de escena. `FireIndicatorCollector` (T17) es igual de puro que
  `LevelUnlockPolicy` a pesar de implementar `ILevelReporter`: la única pieza con estado de Unity
  es el `Func<float>` que le inyecta `FirePanelController`, no la clase misma. No dar por hecho
  que toda política de `Game.Core`/`Game.Levels.*` es 100 % pura sin Unity — revisar caso por caso.

---

## Anexo — lo que cambió después del cierre (25/09/2026)

Verificado contra el código y los assets de `ccf77e6`. El grueso —la mecánica de reunir y encender
de las Fases 5 y 6 (T20–T27)— lo cuenta [`Fase-5-6-Resultados.md`](Fase-5-6-Resultados.md); aquí
va el índice de lo que este documento afirma y ya no es cierto, la equivalencia de las pruebas y lo
que la fase hizo sin registrarlo.

### A.1 Lo vencido, afirmación por afirmación

| Dónde | Decía | Hoy | Desde |
|---|---|---|---|
| Cabecera | Verificación vigente EditMode 84/84 · PlayMode 52/52 | Cifras del cierre de T16; al cerrar T19, 94/94 · 54/54 (§3.8); la vigente, en `Slice-4-Resultados.md` | — |
| Tabla inicial | T17–T19 «pendiente» | Terminadas el 11/09 | `d9c258a`, `f8ffcc8`, `2d2b7f4` |
| §1 | `StrikePosition` (`Far` / `Near` / `VeryClose`) | Borrado; `ForceBand` (fuerza) y `SpacingBand` (cercanía) | `fa5d032`, `863ef05` |
| §1, §2b | `FireLevelConfig` con cuatro parámetros; `N1_Config` 3 / Muy cerca / 3 / 3 | Doce parámetros; `N1_Config` 10 · 7–8 · 10 · 30 · 4 · 2 · 0,8 · 3,5 · 0,5 · 3 · 3 | `fa5d032`, `863ef05`, `2cbe287`, `aca7acc`, `8ec5120` |
| §1 | `StrikeOutcome` con `Position` | `Force`, `Band`, `Spacing`, `EffectiveStrikes`; fábrica nueva `StonesMisplaced` | `fa5d032`, `863ef05` |
| §2 | Ocho mensajes literales del guion, elegidos por distancia | Doce: ocho de fallo, de fuerza y de cercanía, que ya no son del guion, y los cuatro de acierto y soplo, iguales; manda la cercanía | `fa5d032`, `863ef05` |
| §2b | Deslizante de tres posiciones, badge «candado + Aún no», registro con `ScrollRect` | Deslizantes de fuerza y de cercanía (0–10), solo el candado, tablilla con el último mensaje | `fa5d032`, `863ef05` |
| §2b | Build Settings con 6 escenas | 12 en `EditorBuildSettings` | — |
| §2b | `GameFlowRunner.Scenes` mapea `Playing → Level1_Cave` | `GameFlowRunner.PlayingScenes`, una escena por fase | `4f69140` |
| §2c | Ignición = un `Color.Lerp` de ~0,6 s sobre las hojas | Llama cenital animada y quemado que crece en `IgnitionSeconds` | `aca7acc`, `8ec5120` |
| §2c | `CompleteLevel` llama `PlayerProfile.ConfirmPhase(Fire, 1, default)` | Confirma `LevelSummaryController.Show()`, con `ConfirmPhase(PhaseId, …)` | `f8ffcc8` (T18), `a012515`, `4f69140` |
| §2c, §2f, §4 | `NarrativeOutcome` (`EntersLevel` / `ReturnsToLevelSelect`) | Eliminado; `Leave()` decide por `NextSequenceId`, `NextPhase`, `EndsInCredits` e `IsReflectiveClosing` | `4f69140` lo eliminó; los campos son de `9f10485` (`IsReflectiveClosing`), `faaaa13` (`NextPhase`), `b6b886c` (`NextSequenceId`) y `1179dae` (`EndsInCredits`) |
| §2c | `N1_NacimientoDelFuego`: 19 líneas, cierre de «Chispa» | 17 líneas, el guía es ALGORITM | `fa5d032` |
| §2d | Continuar / Reiniciar / Volver al menú (principal); `PauseMenuController` en `Level1_Cave.unity` | Prefab `MenuPausa.prefab` en las cinco escenas jugables: Reanudar / Reiniciar / Volver al menú de niveles (INC-49), con `Time.timeScale = 0` | `8de614f` |
| §2e | «Errores corregidos» justificado por la posición efectiva única | Mismo cálculo; la justificación ya no vale | `fa5d032` |
| §2f | El resumen confirma «al continuar» | En `Show()`, al mostrarse —así desde T18— | — |
| §2f | «Ya vista» = alguna fase del nivel confirmada | Cierre reflexivo: `ReachedLevel`; lo demás: la última fase confirmada | `9f10485`, `d81cfc7` |
| §2g, §4 | Tinte del `Fondo` hacia `#8A97AB`; RNF-20 medido contra la cueva | Capa `fx_oscuridad` con `NarrativeLight`; RNF-20 medido contra la tablilla | `fa5d032` |
| §4 | Botón de ayuda (`HintPolicy`) sin cablear | Cableado, con los pasos `Reunir`, `Golpear` y `Soplar` | `fa5d032`, `863ef05` |
| §4 | Encadenado de las narrativas del N1 sin resolver | `NextSequenceId` en los assets | `1009c5a` |
| §4 | Arte del N1 con placeholders | Cueva cenital; sprites definitivos parciales, montón, llama y quemado | `fa5d032`, `2cbe287`, `dc51804` (clips de la llama), `aca7acc`, `8ec5120` |

### A.2 Pruebas de §3 que cambiaron de nombre

Trece nombres de §3 ya no existen en `Assets/Tests/`: los once de la tabla, que se renombraron o se
sustituyeron al cambiar la mecánica —ninguna se perdió—, y los dos que T18 trasladó (abajo).

| En §3 | Hoy | Commit |
|---|---|---|
| `FireLevelConfig_CT05_ExponeLosCuatroParametrosDelGuion` | `FireLevelConfig_CT05_ExponeLosDiezParametrosDelNivel` | `863ef05` (en `fa5d032`, `…LosCincoParametrosDelNivel`) |
| `FireAttempt_RF15_CambiarPosicionNoAlteraElEstado` | `FireAttempt_RF15_CambiarLaFuerzaNoAlteraElEstado` | `fa5d032` |
| `FireMessages_RNF18_LosOchoMensajesDelGuionEstanDefinidos` | `FireMessages_RNF18_LosDoceMensajesDelNivelEstanDefinidos` | `863ef05` (en `fa5d032`, `…LosOnceMensajesDelNivel…`) |
| `FireFeedbackLog_guion434_CadaDistanciaNoEfectivaDevuelveSuMensaje` | `FireFeedbackLog_guion434_CadaFranjaDeFuerzaNoEfectivaDevuelveSuMensaje` | `fa5d032` |
| `FireFeedbackLog_RF18_TrasDosFallosSeguidosEnLaMismaDistanciaEscalaElMensaje` | `FireFeedbackLog_RF18_TrasDosFallosSeguidosEnLaMismaFranjaEscalaElMensaje` | `fa5d032` |
| `FireIndicators_RF45_ErrorCorregidoExigeCambioDePosicionSeguidoDeAcierto` | `FireIndicators_RF45_ErrorCorregidoExigeCambioDeFuerzaSeguidoDeAcierto` | `fa5d032` |
| `FirePanel_RF14_PresentaDeslizanteBotonGolpearYRegistro` | `FirePanel_RF14_AlEncenderPresentaLosDosDeslizantesGolpearYSoplar` | `863ef05` |
| `FirePanel_RF15_MoverElDeslizanteNoEjecutaNingunGolpe` | `FirePanel_RF15_MoverLosDeslizantesNoEjecutaNingunGolpe` | `863ef05` |
| `FirePanel_RF16_GolpearEscribeEnElRegistroElMensajeDeLaDistancia` | `FirePanel_RF16_GolpearEscribeEnElRegistroElMensajeDeLaFuerza` | `fa5d032` |
| `FirePanel_RF17_ElRegistroNoDesbordaTrasDiezIntentos` | `FirePanel_RF17_LaTablillaMuestraElUltimoMensajeSinDesbordarTrasDiezIntentos` | `fa5d032` |
| `FirePanel_RNF19_SoplarAtenuadoSeDistinguePorIconoYTextoAdemasDelColor` | `FirePanel_RNF19_SoplarAtenuadoSeDistinguePorElCandadoAdemasDelColorYSinRotuloAunNo` | `863ef05` |

`FireLevel_RF04_GuardaAlCompletarLaFase` y `FireLevel_RF03_DesbloqueaElNivel2` (§3.4) ya los había
trasladado T18 a `LevelSummaryTests` (§2f); hoy solo los nombra un comentario de
`FirePanelTests.cs:491`. Los demás nombres de §3 siguen existiendo. Las clases del Nivel 1 de hoy,
con su conteo, están en `Fase-5-6-Resultados.md` §5.1.

### A.3 Lo que la fase hizo y no registró

- **`Assets/Game/Scripts/Runtime/Levels/Fire/AssemblyInfo.cs`**, creado en T14 (`79c9b3d`, 10/09):
  `InternalsVisibleTo` para `Game.Levels.Fire.Tests` y `Game.Levels.Fire.PlayMode.Tests`. Es lo que
  deja a `FirePanelTests` y `CaveLightingTests` leer los miembros `internal` del panel
  (`ForceSlider`, `Attempt`, `Lighting`…) sin subirlos a `public`. No ha cambiado desde entonces.
