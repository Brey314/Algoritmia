# Fases 5 y 6 — Nivel fuego rehecho, reunir y encender: lo construido y sus resultados

**Slice 1 · Golden Path temprano** · Estado: **T20–T23 y T25–T27 terminadas; de T24 y de los
Checkpoints E y F falta solo la revisión con el usuario** — documento escrito a posteriori el 25 de
septiembre de 2026 y verificado contra el código de `ccf77e6`: las dos fases cerraron el 12 y el
15/09 sin documento de resultados.
Verificación registrada al cerrar: **Checkpoint F — EditMode 221/221 · PlayMode 144/153** por CLI,
15/09/2026 (cifras del `todo.md`, §5.3). La corrida vigente de la suite está en
[`Slice-4-Resultados.md`](../Slice%204/Slice-4-Resultados.md), «Corrida completa de la suite
(25/09/2026)».
Plan técnico: estas dos fases **no tienen sección en [`plan.md`](plan.md)**; su plan son las casillas
de [`todo.md`](todo.md) (líneas 330–432) · Contrato: `claudeDocs/SPEC.md` · Hallazgo: `INC-47`
(abierto) en `claudeDocs/INCONSISTENCIAS.md`
Fase anterior: [`Fase-3-Resultados.md`](Fase-3-Resultados.md)
*(01/10/2026: INC-47 está cerrado desde el 29/09/2026, y después del cierre el Nivel 1 cambió en el
humo, la chispa, las piedras y los mandos al soplar; lo registra el
[Anexo](#anexo--lo-que-cambió-después-del-cierre-01102026) del final.)*

Las Fases 5 y 6 rehicieron la mecánica del Nivel 1 por pedido de Santiago («ese slider mide fuerza,
no distancia», 12/09/2026). El deslizante de **posición** de tres muescas del guion §4.3 (hoy
`Solucion_OE2_Diseno_final` §1.4.3) y de RF-15/RF-16 radicados pasó a un nivel en **dos momentos**:
primero se **reúnen** hojas, sílex y pedernal en un círculo; después la cámara se acerca y se
**enciende** con dos deslizantes, fuerza del golpe y cercanía de las piedras. Se aparta de lo
radicado y queda como **INC-47**, abierta hasta que se corrijan guion, OE1 y HU. Los nombres de las
pruebas conservan RF-15/RF-16 porque el requisito —hipótesis → experimento → resultado observable—
es el mismo; cambia el parámetro de la hipótesis.

Este documento registra lo que dejaron en el repositorio T20–T27 —commits `fa5d032` (12/09),
`1009c5a` (13/09, la parte de T20) y `863ef05` (15/09)— y cómo está hoy. **Lo que llegó encima no se
repite aquí**: el sonido, los sprites definitivos parciales, las animaciones del fuego, el reparto al
azar de las piezas, los montoncitos de hojas, el montón cenital, la llama al soplar y el quemado
(`2cbe287`, `dc51804`, `aca7acc`, `8ec5120`) están en
[`Props-y-Sonidos-Resultados.md`](../Slice%203/Props-y-Sonidos-Resultados.md) §1, §2.5, §4, §5, §6
y §8, y el humo de las narrativas (`f801186`) en su anexo; Papá, la familia y Algoritm en el botón
de ayuda (`88fe0ee`), en [`Personajes-Resultados.md`](../Personajes/Personajes-Resultados.md). Qué
afirmaciones de `Fase-3-Resultados.md` quedaron vencidas lo dice el anexo de ese documento. No
reabre decisiones de `SPEC.md`: las cita.

---

## 1. Alcance de las dos fases

| Tarea | Qué entrega | Modo de prueba | Commit | Estado |
|---|---|---|---|---|
| T20 | Las cuatro narrativas `N1_*` con el texto del guion, Algoritm en vez de «Chispa» (INC-44) y paradas de cámara remapeadas; después, el encadenado de la apertura | EditMode + PlayMode | `fa5d032`, `1009c5a` | ✅ terminada |
| T21 | Deslizante de fuerza de diez muescas: `ForceBand`, `ForceSliderFeedback`, mensajes de fallo por fuerza | EditMode + PlayMode | `fa5d032` | ✅ terminada |
| T22 | Hojas, sílex y pedernal arrastrables (`DraggablePiece`, `PieceKind`) | EditMode + PlayMode | `fa5d032` | ✅ terminada — su regla de «piedras cerca» la reemplazó T26 |
| T23 | «Soplar» condicionado al montón; fuera el marco «Hoguera», el `MontonHojas` placeholder y la etiqueta de instrucción fija | PlayMode | `fa5d032` | ✅ terminada — el soplo condicionado lo retiró T26 |
| T24 | INC-47, `Interfaces.md`, `docs/Camara_Narrativa_N1.md`, suites por CLI; Santiago retiró además el registro con historial | — | `fa5d032` | [~] falta la revisión con el usuario |
| — | Resumen de fin de nivel según los mockups 13 y 13b (sin casilla propia en el tablero) | PlayMode | `fa5d032` | ✅ en código |
| T25 | Reunir: círculo de reunión (`FireArrangement`, `RingSprite`) y acercamiento `GatherZoom` | EditMode + PlayMode | `863ef05` | ✅ terminada |
| T26 | Deslizante de cercanía de las piedras (`StoneSpacing`, `SpacingBand`) | EditMode + PlayMode | `863ef05` | ✅ terminada |
| T27 | Limpieza: sin el rótulo «Aún no», `N1_Config` y `N1_Guia` al día, documentos | PlayMode | `863ef05` | ✅ terminada |

**Supuestos que el tablero dejó por confirmar al abrir la Fase 5** (`todo.md:339-342`): fuerza
efectiva 7–8 de 10; soplar sin montón no penaliza. Los de «piedras cerca» y «hojas amontonadas» por
radio quedaron sin objeto con la Fase 6.

---

## 2. La mecánica, de punta a punta

Así se juega hoy `Level1_Cave`. Las referencias son al código de `ccf77e6`.

1. **Al abrir** (`FirePanelController.Start`, `FirePanelController.cs:198-267`) solo hay piezas,
   la tablilla y «Pista». La interfaz del encendido —`ignitionUi`: `FuerzaSlider`,
   `EtiquetasFuerza`, `Acciones` (con «Golpear» y «Soplar») y `CercaniaSlider`— arranca oculta. Los
   rangos de los dos deslizantes salen de `N1_Config`, no de la escena (`:213-218`, CT-05). La
   ayuda arranca en el paso `Reunir` de `N1_Guia` (`:210`).
2. **Reunir.** Cada pieza es un `DraggablePiece` que se arrastra con clic sostenido y se queda donde
   se suelta. Al soltarla (`Dropped`) el panel llama `TryFinishGathering` (`:330-338`): si
   `FireArrangement.IsGathered` dice que **todas** están dentro del círculo, arranca el paso al
   encendido; si falta una, no pasa nada y no hay regaño (CP-02). «Pista» escribe la instrucción en
   la tablilla y, mientras se reúne, **dibuja el círculo** (`RequestHelp`/`ShowRing`, `:527-549`).
   Su radio es la distancia horizontal del centro a la mitad de «Golpear» (`GatherRadius`,
   `:151-159`): sale de la escena, no del asset, porque es disposición.
3. **El acercamiento** (`EnterIgnitionAsync`, `:346-412`). Las piezas dejan de arrastrarse
   (`enabled = false`); el entorno —`Fondo`, `Suelo` y, desde `88fe0ee`, `Personajes`— escala de 1
   a `GatherZoom` (×2) en `GatherSeconds` (0,8 s) con una sola interpolación continua (RNF-21); las
   piedras van al centro según el deslizante de cercanía. Al terminar aparece la interfaz del
   encendido, la ayuda pasa al paso `Golpear` y la tablilla escribe su instrucción (`:409-411`).
   *En T25 las hojas se acomodaban en un anillo tangente —juntas, no encimadas—; desde `aca7acc`
   (22/09) convergen al punto del fuego y encima entra por fundido el montón cenital
   (Props-y-Sonidos §2.5).*
4. **Encender.** El deslizante de fuerza es vertical, a la derecha (`FuerzaSlider`, 0–10, etiquetas
   «Fuerte / Fuerza del golpe / Suave»); el de cercanía es horizontal, sobre los botones
   (`CercaniaSlider`, 0–10, «Lejos» / «Cerca»), y mueve las piedras al instante (`PlaceStones`,
   `:439-461`). Ninguno de los dos produce efecto hasta «Golpear» (RF-15). «Golpear» (`Strike`,
   `:470-521`) pasa la fuerza y la franja de cercanía a `FireAttempt.Strike`; el resultado va a
   los indicadores (`FireIndicatorCollector`), al registro (`FireFeedbackLog`) y a la tablilla, que
   muestra **el último mensaje**. Tras tres fallos seguidos la tablilla suma la pista del paso
   (`:515`); cada golpe efectivo sube un escalón la luz de la cueva (`RefreshLighting`, `:703-704`).
5. **Soplar.** Atenuado y con candado hasta el mínimo de golpes efectivos (`RefreshBlow`,
   `:688-698`); al converger, la ayuda pasa al paso `Soplar` (`:507-510`). Una vez habilitado no
   se vuelve a bloquear (INC-32). Al accionarlo (`Blow`, `:555-571`) registra el mensaje del soplo,
   anima el nacimiento del fuego y `CompleteLevel` (`:672-686`) deja los indicadores en
   `GameFlowRunner.PendingIndicators` y encadena a `N1_NacimientoDelFuego`. Confirmar, desbloquear
   y guardar siguen donde los puso T18: en el resumen de fin de nivel (§3.6).

---

## 3. Qué quedó en el repositorio

### 3.1 `Game.Levels.Fire` (`Assets/Game/Scripts/Runtime/Levels/Fire/`)

| Archivo | Fases 5 y 6 | Qué es hoy |
|---|---|---|
| `StrikePosition.cs` | **Borrado** (`fa5d032`) | — El enum `Far/Near/VeryClose` de T12 ya no existe. |
| `ForceBand.cs` | **Creado** (`fa5d032`) | Enum `TooSoft` / `Effective` / `TooHard`: dónde cae la fuerza respecto de la franja efectiva. |
| `SpacingBand.cs` | **Creado** (`863ef05`) | Enum `TooFar` / `Effective` / `TooClose`: separadas, se rozan, o tan encimadas que solo se frotan. |
| `PieceKind.cs` | **Creado** (`fa5d032`) | Enum `Leaf` / `Silex` / `Pedernal`; lo usan `DraggablePiece.Kind`, `CampfireLayout`, `PlaceStones` y el sonido de la hoja arrastrada. |
| `DraggablePiece.cs` | **Creado** (`fa5d032`), ampliado en `863ef05` | Arrastre con clic sostenido por `IBeginDragHandler`/`IDragHandler`/`IEndDragHandler` de uGUI (`InputSystemUIInputModule`, nunca la clase `Input` legada); `MoveTo` recorta la posición al rect del suelo (`DraggablePiece.cs:90-96`). T25 le dio el evento `Dropped` al soltar (`:52`) y `Width`. Deshabilitado, deja de recibir el arrastre: así se fijan las piezas al pasar al encendido. El giro al azar, el gesto de levantar y `PickedUp` son de `aca7acc` (Props-y-Sonidos §2.5). |
| `FireArrangement.cs` | **Creado** (`fa5d032`), reescrito en `863ef05` | C# plano. En T22 respondía `IsPiled` (hojas dentro de `PileRadius`) y `StonesNear` (piedras dentro de `StonesRadius`); desde T25 solo `IsGathered`: todas las piezas dentro del círculo, y sin piezas no hay nada reunido (`FireArrangement.cs:27-43`). |
| `StoneSpacing.cs` | **Creado** (`863ef05`) | C# plano. `Distance(notch)`: de la longitud de una hoja en la muesca 0 a 0 en la máxima; `Overlap(notch)`: ancho de la piedra menos esa distancia; `Classify(notch)`: efectiva si el encimado cae en `EffectiveOverlap` ± `OverlapTolerance` (`StoneSpacing.cs:33-52`). `Contact(notch)` es de `2cbe287`. |
| `ForceSliderFeedback.cs` | **Creado** (`fa5d032`) | El asa del deslizante de fuerza **crece y enrojece** con la muesca: escala `MinScale` 0,8 → `MaxScale` 1,2 y color azul → rojo lineal (`ForceSliderFeedback.cs:51-56`). Doble indicador a propósito (RNF-19): tamaño **y** color, nunca solo el color. No toca el valor ni ejecuta nada. Va en `FuerzaSlider`. |
| `RingSprite.cs` | **Creado** (`863ef05`) | `internal static`: genera en memoria el contorno del círculo de reunión al diámetro exacto, con el grosor de trazo que recibe —el panel le pasa 6 px (`FirePanelController.cs:542`)— y un píxel de suavizado (`RingSprite.cs:14-41`). No hay sprite de anillo en el arte. |
| `FireAttempt.cs` | Modificado (ambos) | `Classify(force)` → `ForceBand` (`FireAttempt.cs:35-38`); `Strike(int force, SpacingBand spacing)` juzga **primero la cercanía y después la fuerza** (`:45-68`). Siguen igual los dos contadores, `CanBlow` (INC-32) y el comentario pedagógico contra el tope de intentos. |
| `StrikeOutcome.cs` | Modificado (ambos) | Campos `Effective`, `Force`, `Band`, `Spacing`, `EffectiveStrikes`; fábricas `SparksDied(force, band)`, `StonesMisplaced(force, band, spacing)` (T26) y `SparkLanded(force, n)` (`StrikeOutcome.cs:35-47`). Ya no hay `Position`. |
| `FireFeedbackLog.cs` | Modificado (ambos) | El fallo se elige primero por cercanía (`SpacingMessage`) y si no por franja de fuerza (`FailureMessage`), cada lado con su par «por defecto / tras dos fallos» (`FireFeedbackLog.cs:77-86`). Nuevo `RecordGuide` (`:56-62`): la instrucción o la pista del guía entran al mismo registro. `Entries` sigue acumulando el historial entero. |
| `FireMessages.cs` | Modificado (ambos) | **Doce** mensajes (`FireMessages.cs:13-59`): `SoftNoSpark`, `SoftStonesGraze`, `HardSparksScatter`, `HardSparksFly`, `StonesFar`, `StonesFarAgain`, `StonesTooClose`, `StonesTooCloseAgain`, los tres de golpe efectivo y `BlowSuccess`. En `fa5d032` eran once, con `BlowNoPile`; T26 lo quitó y sumó el par de «encimadas». |
| `FireLevelConfig.cs` | Modificado (ambos) | De cuatro parámetros de posición a diez: `ForceLevels` 10, `EffectiveForceMin`/`Max` 7/8, `SpacingLevels` 10, `EffectiveOverlap`, `OverlapTolerance` 4, `GatherZoom` 2, `GatherSeconds` 0,8, `MinimumEffectiveStrikes` 3, `AttemptsBeforeHint` 3. `PileRadius`/`StonesRadius` de T22 se retiraron en T27. Hoy son doce: `IgnitionSeconds` y `BurnExtent` llegaron en `aca7acc`/`8ec5120`. |
| `FeedbackLogView.cs` | Modificado (`fa5d032`) | La tablilla muestra **solo la última** entrada (`FeedbackLogView.cs:22-28`). Santiago retiró el área con historial el 12/09 («no estaba en el mockup, estorba»); el historial sigue en `FireFeedbackLog.Entries`. |
| `CaveLightingController.cs` | Reescrito (`fa5d032`) | Ya no tiñe el `Fondo`: crea en `Awake` una capa «Oscuridad» con el material `fx_oscuridad` estirada sobre la vista cenital (`CaveLightingController.cs:44-64`) y le pasa `NarrativeLight.Lerp(darkest, brightest, progreso)` (`:67-83`). §4, decisión 7. |
| `FirePanelController.cs` | Modificado (ambos) | Adaptador delgado, sin reglas del juego: todo lo de §2. Hoy tiene 706 líneas porque encima llegaron el sonido, el reparto, el montón, la llama, el quemado y los personajes (Props-y-Sonidos §2.5, Personajes). El campo `forceSlider` conserva `FormerlySerializedAs("positionSlider")` (`:34`) para no perder la referencia de la escena. |
| `FireIndicatorCollector.cs` | Sin cambios de lógica | El cálculo de T17 no cambió: «Errores corregidos» sigue siendo un golpe efectivo justo tras uno fallido. Su comentario quedó viejo (§6). |
| `AssemblyInfo.cs` | Sin cambios | `InternalsVisibleTo` para `Game.Levels.Fire.Tests` y `Game.Levels.Fire.PlayMode.Tests`, desde T14 (`79c9b3d`). |

Hoy el assembly tiene 21 archivos `.cs` (1 843 líneas) y referencia `Game.Core`,
`Game.Scaffolding`, `UnityEngine.UI` (desde T14) y `Game.Audio` (desde `2cbe287`). **No referencia
`Unity.InputSystem`**: el arrastre va por los eventos de uGUI. `FireSounds`, `FloorScatter` y
`LeafPile` son del carril de arte y sonido.

### 3.2 La luz, fuera del assembly del nivel (`fa5d032`, `1009c5a`)

La capa de oscuridad nació en T20 para las narrativas del N1 y la cueva la reutiliza:

- **`Assets/Game/Art/FX/fx_oscuridad.shader`** (`Algoritm/Oscuridad`) + **`fx_oscuridad.mat`**: un
  charco de luz radial sobre un fondo atenuado, multiplicado sobre lo ya pintado (`Blend DstColor
  Zero`) por un único `Image` a pantalla completa. Parámetros `_Center`, `_Radius`, `_Ambient`,
  `_Tint`, `_Edge` y `_Aspect`.
- **`NarrativeLight`** (`Game.Scaffolding`, nuevo): el estado de esa capa —`Center`, `Radius`,
  `Ambient`, `Tint`, `FlashSeconds`— y `Lerp`. Su valor por defecto (plena luz, tinte blanco) no
  cambia nada, así que las secuencias que no declaran luz se ven igual.
- **`NarrativeSequence.LightStart`** (`NarrativeSequence.cs:57`) y **`CameraKey.Light`**
  (`IllustrationFraming.cs:57`): cada parada de cámara lleva su luz al lado. `CameraKey.HardCut`
  (`:61`) entró en el mismo commit; `OpensFromBlack` y `HardCutFadeSeconds`
  (`NarrativeSequence.cs:65-69`), en `1009c5a`.
- **`NarrativeSceneController`** crea la capa «Oscuridad» en ejecución con su `darknessMaterial`
  (`NarrativeSceneController.cs:347-364`); el material está cableado en `Narrative.unity` y en
  `Level1_Cave.unity`. Un `FlashSeconds` mayor que cero aplica esa luz de golpe y vuelve a la de la
  parada anterior (el ¡CLIC! del sílex). Lo usa una sola parada, la de «¡CLIC! Un destello
  pequeñísimo…» en `N1_Hallazgo` (0,08 s, `N1_Hallazgo.asset:69`, desde `fa5d032`); en todas las
  demás vale 0.

La cámara por paradas (`CameraKeys`, suavizado) es anterior, de W07 del Slice 2 (`416ec79`), y se
registra con ese slice.

### 3.3 Escena `Level1_Cave.unity`

Reconstruida en `fa5d032` y `863ef05`. Lo que dejaron las dos fases (jerarquía bajo `Canvas/Panel`,
salvo `Fondo`, que cuelga de `Canvas`):

| Elemento | Qué es |
|---|---|
| `Suelo` con `Hoja_1`…`Hoja_5`, `Silex`, `Pedernal` | Las siete piezas arrastrables (`DraggablePiece`), hermanas del punto del fuego. |
| `Suelo/PuntoDeFuego` | El centro de la pantalla: centro del círculo y de la fogata. |
| `Suelo/AnilloReunion` | El círculo de reunión (T25), oculto hasta pedir ayuda; el sprite lo genera `RingSprite`. |
| `FuerzaSlider` + `EtiquetasFuerza` | Vertical, 136×440, anclado a la derecha; lleva `ForceSliderFeedback`. |
| `CercaniaSlider` | Horizontal, 456×120 a y = 222, con «Lejos» / «Cerca»: el recorrido del asa va de la mitad de «Soplar» a la mitad de «Golpear» (x = ±172). |
| `Acciones/BotonGolpear` · `Acciones/BotonSoplar` | Los dos llevan `ButtonPressFeedback` desde `fa5d032` (el hundido de 4 px de `2893cdb`). |
| `BadgeBloqueado/IconoCandado` | El candado sobre «Soplar» atenuado. El rótulo «Aún no» (`AunNo`) se retiró en `863ef05`: el candado y el color atenuado bastan como doble indicador (RNF-19). |
| `Mensaje/Fondo/InstruccionLabel` | La tablilla superior (mockup 7): el `Text` donde `FeedbackLogView` escribe el último mensaje. Conserva el nombre de cuando era la etiqueta de instrucción fija. |
| `BotonPista` | «Pista», arriba a la izquierda, con la `Animation` legada `ui_pulso_pista.anim` (escala 1 → 1,06 → 1 en 2,2 s, en bucle, arranca sola) desde `fa5d032`. |
| `Fondo` | La vista cenital `entorno_n1_cueva_cenital.png` (`fa5d032`) con `CaveLightingController` y `fx_oscuridad.mat`. |

Sin `ScrollRect` (lo quitó `fa5d032`). El `EventSystem` sigue con `ControlesJugables.inputactions`
(T14, RNF-02). El menú de pausa es la instancia del prefab `MenuPausa.prefab` desde W17 del Slice 2
(`8de614f`). `MontonHojas`, `Fuego`, `Personajes` y `BotonPista/Fondo/Algoritm` son posteriores
(Props-y-Sonidos §5, Personajes).

### 3.4 Datos (`Assets/Game/Data/`)

| Asset | Hoy | Historia |
|---|---|---|
| `Fire/N1_Config.asset` | `ForceLevels` 10 · `EffectiveForceMin`/`Max` 7/8 · `SpacingLevels` 10 · `EffectiveOverlap` 30 · `OverlapTolerance` 4 · `GatherZoom` 2 · `GatherSeconds` 0,8 · `IgnitionSeconds` 3,5 · `BurnExtent` 0,5 · `MinimumEffectiveStrikes` 3 · `AttemptsBeforeHint` 3 | En `863ef05`, `EffectiveOverlap` valía **5**: con las piezas de entonces la muesca certera era la **2**. `2cbe287` lo subió a 30, y desde ahí es la **5** y solo ella (Props-y-Sonidos §4). |
| `Fire/N1_Mensajes.asset` | Los doce textos, observacionales y sin cifras (RF-17): de fuerza («Las piedras apenas se rozan. No sale ninguna chispa.»), de cercanía («El sílex y el pedernal están separados…», «Las piedras están tan encimadas que no chocan…»), los tres de golpe efectivo y el del soplo | Los ocho de fallo se reescribieron en `fa5d032` y `863ef05` y ya no son el texto del guion §4.3.4 (INC-47); los tres de golpe efectivo y el del soplo siguen siendo los literales de T13. |
| `Guide/N1_Guia.asset` | Tres pasos: `Reunir` («Reúne todas las hojas y las dos piedras en el centro de la pantalla.»), `Golpear` («Elige la fuerza y qué tan cerca van las piedras, y golpea para hacer chispas.») y `Soplar` | En T11 tenía dos (`Golpear`, `Soplar`), escritos para la distancia. `fa5d032` los reescribió para la fuerza y el montón sin nombrar la muesca correcta; `863ef05` añadió `Reunir` y volvió a escribir `Golpear` para la cercanía. La pista de `Golpear` pregunta sin resolver: «¿Qué cambiarías antes del próximo golpe?» (CP-06). |
| `LevelSummaryMessages.asset` (N1) | `Intro` «Esto es lo que pasó en la cueva:», `Discovery` «Descubriste que el fuego necesita chispa y aire.», cuatro variantes de relato con «fuerza» y «sitio», `SkillNamed` «Eso se llama probar y ajustar: es lo mismo que hace quien arma un plan paso a paso.» | `fa5d032` añadió `Discovery` y `SkillNamed` y reescribió el relato (antes hablaba de «lugar»). |
| `Narrative/N1_*.asset` | §3.5 | T20 (`fa5d032`) y `1009c5a`. |

### 3.5 Las cuatro narrativas del N1 (T20)

`fa5d032` reescribió las líneas con el texto completo del guion, cambió el hablante `CHISPA` por
`ALGORITM` (INC-44) en las tres donde habla el guía y remapeó las paradas de cámara
(`docs/Camara_Narrativa_N1.md` §7), cada una con su luz. `1009c5a` (13/09) las **encadenó por el
asset**: les puso `NextSequenceId`, el campo que el carril del Slice 2 había añadido en `b6b886c`
(12/09). Hoy `NarrativeSceneController.Leave()` (`NarrativeSceneController.cs:821-852`) prueba en
orden `NextSequenceId` → `NextPhase` → `EndsInCredits` (de `1179dae`, 21/09) →
`IsReflectiveClosing` → menú de niveles, sin un `if` por secuencia.

| Secuencia | Líneas | Paradas | Luz al abrir (`LightStart`) | Sale a |
|---|---|---|---|---|
| `N1_Apertura` | 7 | 4 | fondo 0,06, tinte frío (0,55 · 0,68 · 1) | `NextSequenceId` → `N1_AparicionGuia` |
| `N1_AparicionGuia` | 11 | 6 | fondo 0,02, tinte cálido (1 · 0,82 · 0,61) | `NextSequenceId` → `N1_Hallazgo` |
| `N1_Hallazgo` | 18 | 10 | fondo 0,08, tinte cálido (1 · 0,82 · 0,61); abre desde negro (`OpensFromBlack`) | `NextPhase` 1 → `Level1_Cave` |
| `N1_NacimientoDelFuego` | 17 | 11 | fondo 0,35 con charco (radio 0,1), tinte de fuego (1 · 0,73 · 0,47); las paradas suben hasta 0,85 | `IsReflectiveClosing` → `LevelSummary` |

Antes de T20 `N1_NacimientoDelFuego` tenía 19 líneas; las otras tres ya tenían 7, 11 y 18. Las
paradas de cámara no existían en ninguna (las introdujo `fa5d032`). `N1_Apertura` pasó a su propia
ilustración, `entorno_n1_apertura.png`, en `1009c5a`; las otras tres usan `entorno_n1_cueva_2x.png`
(`fa5d032`). Los tintes cálido y de fuego de la tabla y el `OpensFromBlack` de `N1_Hallazgo` son
de `a9236f1` (16/09, «tintes cálidos»; antes, 1 · 0,9 · 0,72 y 1 · 0,8 · 0,55); el sonido
(`2cbe287`), los personajes (`88fe0ee`) y el humo (`f801186`) de estas secuencias están en sus
documentos. El cierre reflexivo de `N1_NacimientoDelFuego` sigue diciendo, en boca de Algoritm,
«Cambiaste la fuerza, volviste a probar…» y «Eso tiene nombre: se llama iterar. Probar, mirar el
resultado y ajustar.».

### 3.6 Resumen de fin de nivel: mockups 13 y 13b (`fa5d032`)

`LevelSummaryController` (`Game.UI`) reparte el texto en una tablilla: título (la línea `Intro` sin
los dos puntos), el hallazgo (`Discovery`), **una viñeta por frase** del relato —clona una fila
plantilla inactiva— y la habilidad nombrada (`SkillNamed`) en el recuadro verde (RF-12, CP-07). Dos
botones, «Continuar» y «Volver al menú de niveles», salen al mismo sitio. `LevelSummary.unity` se
reconstruyó con dos `VerticalLayoutGroup` y un `ContentSizeFitter`; el `CanvasScaler` de referencia
1920×1080 ya estaba desde T18 (`f8ffcc8`). **Confirma la fase, desbloquea el nivel siguiente y guarda en `Show()`**, al mostrarse
(`LevelSummaryController.cs:79-100`), como desde T18. Sin cifras en ninguna rama (CP-03, RF-45).
Que hoy elija sus mensajes por nivel (`messagesByLevel`) y sume todas las fases es del Slice 2.

### 3.7 Arte que entró con las fases

- `Environments/Fire/entorno_n1_cueva_cenital.png` (la cueva vista desde arriba, fondo de la
  mecánica) y `entorno_n1_cueva_2x.png` (la cueva de las narrativas), `fa5d032`;
  `entorno_n1_apertura.png`, `1009c5a`. `a9236f1` los reimportó sin compresión.
- `Props/Fire/prop_n1_hoja.png`, `prop_n1_silex.png`, `prop_n1_pedernal.png`: **temporales**, con el
  estilo de los props del N2 (`fa5d032`); los sustituyeron los definitivos parciales de `2cbe287`.
- `UI/Common/ui_pulso_pista.anim` (`fa5d032`).
- `FX/fx_oscuridad.shader` y `.mat` (§3.2).

---

## 4. Decisiones

1. **Todo lo que decide es C# plano.** Si está todo reunido (`FireArrangement`), dónde van las
   piedras y si chocan (`StoneSpacing`), qué produjo el golpe (`FireAttempt`) y qué mensaje toca
   (`FireFeedbackLog`) se prueban en EditMode sin escena. `FirePanelController` solo pasa
   posiciones y muescas.
2. **La cercanía manda sobre la fuerza.** Con las piedras separadas o encimadas no prende con
   ninguna fuerza, y el mensaje habla de las piedras, no de la fuerza
   (`FireFeedbackLog_RF16_LaCercaniaMandaSobreLaFuerzaCuandoFallanLasDos`): primero tienen que
   chocar.
3. **El radio del círculo sale de la escena.** Es la mitad del botón de abajo (pedido de Santiago,
   15/09); si cambia la disposición, cambia el radio sin tocar el asset.
4. **«Soplar» ya no puede fallar.** T23 condicionaba el fuego a que las hojas estuvieran
   amontonadas y, sin montón, describía el soplo sin penalizar (`BlowNoPile`). Con la Fase 6 las
   hojas se acomodan solas en la fogata, así que T26 retiró la condición y el mensaje.
5. **La tablilla muestra solo el último mensaje.** Lo pidió Santiago el 12/09. El historial no se
   pierde (`Entries`), pero el estudiante ya no lo ve (HU-06 queda en INC-47).
6. **La ayuda, cableada.** `HintPolicy` (T11) dejó de estar suelta en `fa5d032`: «Pista» repite la
   instrucción del paso activo; tras `AttemptsBeforeHint` fallos seguidos la tablilla suma la
   pista; el paso cambia `Reunir` → `Golpear` al terminar el acercamiento y `Golpear` → `Soplar` al
   converger. Ningún texto nombra la muesca correcta (CP-06). `FireAttempt.ShouldOfferHint` sigue
   existiendo, pero el panel no lo usa: cuenta `HintPolicy`.
7. **La luz de la cueva es la misma capa que la de las narrativas.** El tinte `#8A97AB` de T19 se
   sustituyó por `fx_oscuridad`: un charco fijo sobre el montón (centro 0,5 · 0,5, radio 0,30, tinte
   ámbar) y un fondo que sube de 0,10 a 0,30 (`docs/Camara_Narrativa_N1.md` §8), en escalones de
   `golpes efectivos / (mínimo + 1)`; el último tramo lo da la ignición (guion E4/E7). La prueba de
   RNF-20 dejó de medir el texto contra la cueva: la instrucción va sobre su propia tablilla, que
   la oscuridad no toca, y se mide contra la cara de esa tablilla.
8. **El encadenado de la apertura es un asset, no una rama.** Lo que `Fase-3-Resultados.md` §4
   dejó «sin resolver» lo resolvió `1009c5a` con `NextSequenceId`; la prueba
   `NarrativeScene_RF10_LaAperturaEncadenaLasTresEscenasDelNivel1YEntraAJugar` recorre las tres y
   exige que se entre a jugar.

---

## 5. Pruebas

### 5.1 Las clases del Nivel 1, hoy

Conteo estático sobre `ccf77e6` (`[Test]` y cada `[TestCase]`). No es una corrida: la corrida
vigente está en `Slice-4-Resultados.md`.

| Clase | Modo | Métodos · casos | Origen |
|---|---|---|---|
| `FireAttemptTests` | EditMode | 9 · 16 | T12; reescrita en `fa5d032` y `863ef05` |
| `FireFeedbackLogTests` | EditMode | 8 · 12 | T13; reescrita en `fa5d032` y `863ef05` |
| `FireIndicatorTests` | EditMode | 6 · 6 | T17; `fa5d032` renombró una |
| `FireLevelConfigTests` | EditMode | 1 · 1 | T12; reescrita en `fa5d032` y `863ef05`; `2cbe287` le subió `EffectiveOverlap` a 30 |
| `FireMessagesTests` | EditMode | 2 · 2 | T13; reescrita en `fa5d032` y `863ef05` |
| `FireArrangementTests` | EditMode | 2 · 2 | `fa5d032` (tres pruebas, T22); reescrita en `863ef05` (T25) |
| `StoneSpacingTests` | EditMode | 5 · 9 | `863ef05` (cuatro métodos, T26); `2cbe287` renombró una y añadió la de `Contact` |
| `FloorScatterTests` | EditMode | 1 · 1 | `8ec5120` (Props-y-Sonidos §8) |
| `FirePanelTests` | PlayMode | 33 · 34 (2 de verificación visual) | T14; al cerrar la Fase 6 tenía 22 métodos. Después, 4 de `aca7acc`/`8ec5120` (Props-y-Sonidos §8) y 7 de `88fe0ee` (Personajes) |
| `CaveLightingTests` | PlayMode | 2 · 2 (las dos de verificación visual) | T19; ajustada en `fa5d032`, `863ef05` y `2cbe287` |
| `FireSoundsTests` | PlayMode | 3 · 3 | `2cbe287` (Props-y-Sonidos §8) |

EditMode: 34 métodos · 49 casos. PlayMode: 38 métodos · 39 casos.

### 5.2 Las pruebas que dejaron T20–T27

**EditMode (`Game.Levels.Fire.Tests`).**

| Prueba | Requisito | Qué fija |
|---|---|---|
| `FireAttempt_RNF18_ClasificaLaFuerzaSegunLaFranjaDeLaConfiguracion` (6 casos) | RNF-18 | Las franjas suave / efectiva / fuerte salen de la configuración: un `FireLevelConfig` que la prueba crea con `CreateInstance` y la franja 7–8, no el asset `N1_Config` |
| `FireAttempt_RF16_SiLasPiedrasNoChocanNoPrendeAunqueLaFuerzaSeaLaEfectiva` (2 casos) | RF-16 | Separadas o encimadas: no prende con la fuerza efectiva |
| `FireAttempt_RF15_CambiarLaFuerzaNoAlteraElEstado` | RF-15 | Renombre de `…CambiarPosicion…` |
| `FireFeedbackLog_guion434_CadaFranjaDeFuerzaNoEfectivaDevuelveSuMensaje` (2 casos) | guion §4.3.4 | Renombre de `…CadaDistancia…` |
| `FireFeedbackLog_RF18_TrasDosFallosSeguidosEnLaMismaFranjaEscalaElMensaje` | RF-18 | Renombre de `…EnLaMismaDistancia…` |
| `FireFeedbackLog_RF16_SiLasPiedrasNoChocanDevuelveSuMensajeYEscalaTrasDosFallos` (2 casos) | RF-16 | Cada lado de la cercanía con su par de mensajes |
| `FireFeedbackLog_RF16_LaCercaniaMandaSobreLaFuerzaCuandoFallanLasDos` | RF-16 | Decisión 2 |
| `FireIndicators_RF45_ErrorCorregidoExigeCambioDeFuerzaSeguidoDeAcierto` | RF-45 | Renombre de `…CambioDePosicion…` |
| `FireLevelConfig_CT05_ExponeLosDiezParametrosDelNivel` | CT-05 | Los diez parámetros de la Fase 6 con sus valores |
| `FireMessages_RNF18_LosDoceMensajesDelNivelEstanDefinidos` | RNF-18 | Los doce textos del asset |
| `FireArrangement_RF14_EstaTodoReunidoSoloSiCadaPiezaCaeDentroDelCirculo` | RF-14 | T25 |
| `FireArrangement_RF14_SinPiezasNoHayNadaReunido` | RF-14 | T25 |
| `StoneSpacing_RF15_LaMuescaCeroSeparaUnaHojaYLaMaximaEncimaLasPiedras` | RF-15 | T26 |
| `StoneSpacing_RF16_ElGolpeCerteroEsSoloLaMuescaCinco` (5 casos) | RF-16 | En T26 se llamaba `…ElGolpeCerteroEsCuandoLasPiedrasSeRozanUnosPocosPixeles`; `2cbe287` la renombró al subir `EffectiveOverlap` |
| `StoneSpacing_RNF18_ElRoceEfectivoYSuMargenSalenDeLaConfiguracion` | RNF-18 | T26 |
| `StoneSpacing_RF15_UnaMuescaFueraDeRangoNoRompeNada` | RF-15 | T26 |

**PlayMode — `FirePanelTests` al cerrar la Fase 6 (22 métodos, 23 casos).** Seis vienen de T14–T15
sin cambio de nombre: `FirePanel_RF19_SoplarSeHabilitaAlAlcanzarElMinimoDeGolpesEfectivos`,
`FirePanel_RF19_SoplarAtenuadoNoRespondeNiRegistraError`,
`FirePanel_RNF02_ElMapaDeControlesNoTieneNingunBindingDeTeclado`,
`FirePanel_RNF03_LaEscenaNoPresentaListaDeTareas`,
`FireLevel_RF20_SoplarEncadenaAnimacionYEscenaDeCierre` y
`FireLevel_RNF21_SinDestellosDeAltaFrecuencia` (verificación visual). Las demás son de estas fases:

| Prueba | Requisito | Commit |
|---|---|---|
| `FirePanel_RF14_AlAbrirSoloHayPiezasTablillaYPista` | RF-14 | `863ef05` |
| `FirePanel_RF13_AlReunirLaPistaDibujaElCirculoConElBordeEnLaMitadDelBotonDeAbajo` | RF-13 | `863ef05` |
| `FirePanel_RF14_ConUnaPiezaFueraDelCirculoNoPasaAlEncendido` | RF-14, CP-02 | `863ef05` |
| `FirePanel_RF14_ReunirTodoDentroDelCirculoAcercaLaCamaraYArmaLaFogata` | RF-14 | `863ef05` |
| `FirePanel_RF14_AlEncenderPresentaLosDosDeslizantesGolpearYSoplar` | RF-14 | `863ef05` (sustituye a `…PresentaDeslizanteBotonGolpearYRegistro`) |
| `FirePanel_RF15_MoverLosDeslizantesNoEjecutaNingunGolpe` | RF-15 | `863ef05` (sustituye a `…MoverElDeslizante…`) |
| `FirePanel_RF15_ElDeslizanteDeCercaniaVaDeLaMitadDeSoplarALaMitadDeGolpearYMueveLasPiedras` | RF-15 | `863ef05` |
| `FirePanel_RF15_ElDeslizanteDeFuerzaTieneDiezMuescasYElAsaCreceYEnrojeceConLaFuerza` | RF-15, RNF-19 | `fa5d032`, renombrada en `863ef05` |
| `FirePanel_RF16_GolpearEscribeEnElRegistroElMensajeDeLaFuerza` | RF-16 | `fa5d032` (sustituye a `…MensajeDeLaDistancia`) |
| `FirePanel_RF16_ConLasPiedrasSeparadasOEncimadasElGolpeNoPrendeNiPenaliza` (2 casos) | RF-16, CP-02 | `863ef05` (sustituye a `…ConLasPiedrasLejosDeLasHojas…` de T22) |
| `FirePanel_RF13_PistaADemandaEscribeLaInstruccionEnElRegistro` | RF-13 | `fa5d032` |
| `FirePanel_RF13_TrasTresFallosSeguidosElRegistroSumaLaPista` | RF-13 | `fa5d032` |
| `FirePanel_RF17_LaTablillaMuestraElUltimoMensajeSinDesbordarTrasDiezIntentos` | RF-17 | `fa5d032` (sustituye a `…ElRegistroNoDesborda…`) |
| `FirePanel_RNF19_SoplarAtenuadoSeDistinguePorElCandadoAdemasDelColorYSinRotuloAunNo` | RNF-19 | `863ef05` (sustituye a `…PorIconoYTexto…`) |
| `FirePanel_RNF19_ElEncendidoSeVeComoElMockupConElCandadoSinRotulo` | RNF-19 (verificación visual) | `863ef05` |
| `FirePanel_CT06_LasPiezasSeArrastranConClicSostenidoYNoSalenDelSuelo` | CT-06 | `fa5d032` |

`FirePanel_RF19_SoplarConLasHojasRegadasNoPrendeElFuegoYNoPenaliza` (T23) existió solo entre
`fa5d032` y `863ef05`: se fue con el soplo condicionado (decisión 4).

**Fuera del assembly del nivel.** En `NarrativeSceneTests`:
`NarrativeScene_RF10_LaAperturaEncadenaLasTresEscenasDelNivel1YEntraAJugar` (`1009c5a`),
`NarrativeScene_RF05_ElCorteSecoFundeANegroSaltaAOscurasYVuelve` (entró en `fa5d032` como
`…LaParadaDeCorteSecoSaltaSinSuavizado` y `1009c5a` la reescribió) y
`NarrativeScene_RF05_ElDestelloDuraSusSegundosYVuelveALaLuzAnterior` (`fa5d032`). En
`NarrativeSequenceTests`, `NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo`
(`1009c5a`), cuya lista `Verificadas` empieza por las cuatro `N1_*`. En `LevelSummaryTests`,
`LevelSummary_RF03_DevuelveAlMenuConNivel2Desbloqueado` ganó en `fa5d032` las aserciones de la
tablilla: título sin dos puntos, dos viñetas, «probar y ajustar» en la habilidad y ningún dígito.

### 5.3 Cifras registradas al cerrar (del `todo.md`)

No se volvieron a correr para este documento; se copian tal como las registró el tablero.

| Momento | EditMode | PlayMode | Fuente |
|---|---|---|---|
| T21 (12/09) | 167/167 | 92/92 (6 de verificación visual omitidas en batchmode) | `todo.md:356` |
| Checkpoint E (12/09) | 173/173 | 95/95 (6 omitidas) | `todo.md:385` |
| Checkpoint F (15/09) | 221/221 | 144/153: 6 de verificación visual omitidas en batchmode y 3 fallos de disposición preexistentes en la resolución 640×480 del batchmode (`MazeScene_RNF03_AlAcumularse…`, `WorkshopScene_RNF03_NadaSeSale…`, `NarrativeScene_RNF01_LaLineaMasLarga…`), que según el tablero fallaban también sin la Fase 6 y pasan con el Editor abierto (W-F, 148/148) | `todo.md:429` |

Las marcas de CP-02/CP-06 de los dos checkpoints se apoyan en pruebas distintas. La del E
(`todo.md:386`, «ninguna pista nombra la fuerza ni la distancia correctas»), en
`FirePanel_RF19_SoplarConLasHojasRegadas…` —retirada en `863ef05`— y en las `FirePanel_RF13_…`; la
del F (`todo.md:430`, «ninguna pista ni mensaje nombra la muesca correcta»), en
`FirePanel_RF16_ConLasPiedrasSeparadasOEncimadas…` y en las `FirePanel_RF13_…`.

---

## 6. Lo que queda abierto

- **INC-47 sigue abierta.** Guion §4.3 (§1.4.3 del documento refundido), OE1 RF-15/RF-16 y HU-06
  siguen describiendo el deslizante de posición, los mensajes por distancia y el registro con
  historial. La corrección es de los documentos radicados, no del código.
- **La revisión con el usuario**, única casilla sin marcar de T24 y de los Checkpoints E y F: el
  radio del círculo, el tamaño de la fogata y que la muesca certera sea la correcta. Al cerrar T26
  era la 2; desde `2cbe287` es la 5. PG-06 sigue abierto.
- **Comentarios vencidos en el código** (no se tocan desde aquí):
  `FireIndicatorCollector.cs:49` («golpes ejecutados desde una posición no efectiva») y `:54-57`
  (cita `StrikePosition` y «la posición efectiva es única»; hoy la fuerza efectiva son dos muescas y
  el fallo puede ser de cercanía). `FireFeedbackLog.cs:6-8` dice que el historial se acumula «para
  que el estudiante compare sus intentos», y el estudiante ya solo ve el último.
- **Dos parámetros sin prueba de valor.** `FireLevelConfig_CT05_ExponeLosDiezParametrosDelNivel`
  fija los diez de la Fase 6; `IgnitionSeconds` y `BurnExtent`, que llegaron después, no los fija
  ninguna prueba.
- **El pulso de «Pista» es permanente.** `ui_pulso_pista` arranca solo y va en bucle desde que se
  abre la escena, no tras tres fallos; lo recoge también `Personajes-Resultados.md` como pendiente.
- **Los tres fallos de disposición en batchmode** del Checkpoint F eran ajenos al N1; su estado de
  hoy lo dice la corrida del 25/09 en `Slice-4-Resultados.md`.

*(01/10/2026: de esta lista quedan vencidos tres puntos. INC-47 se cerró el 29/09/2026 corrigiendo
los documentos radicados; los comentarios de `FireIndicatorCollector.cs` y `FireFeedbackLog.cs` se
corrigieron en `37b3cb7` (30/09); y la revisión con el usuario la resolvió la decisión D-a del acta
D10 (30/09/2026), que la marca como hecha por Santiago tras verificarla con pruebas y capturas.
Siguen abiertos los dos parámetros sin prueba de valor y el pulso permanente de «Pista». Detalle en
el Anexo, A.2, A.3 y A.7.)*

---

## Anexo — lo que cambió después del cierre (01/10/2026)

Lo de arriba es el registro del 25/09/2026 sobre `ccf77e6` y se conserva. Este anexo recoge lo que
llegó después al Nivel 1 y, del carril del Slice 1, a `Game.Core` y a la regla de «Omitir», que
ningún otro documento de resultados registra. Fuentes: `git log` hasta `359365e` (30/09/2026) y el
árbol de trabajo de la rama `feat/cierre-de-slices-y-oe3` del 01/10/2026, que entra en el commit de
cierre con las tarjetas D10-1 y D10-5 del acta D10.

### A.1 Qué cambió y dónde

| Fecha | Commit | Qué cambió |
|---|---|---|
| 30/09 | `37b3cb7` | El montón humea al converger (INC-47), la chispa se ve (INC-68), las acotaciones dejan de llamar «estrella» a Algoritm (INC-52) y se corrigen los comentarios vencidos de §6 — A.3 |
| 30/09 | `5a22df1` | Documentos: INC-47 queda cerrado (decisión del 29/09) — A.2 |
| 01/10 | sin hash, D10-1 | Piedras sobre las hojas, la chispa como un solo rayo (INC-119), el humo hasta la corona de la llama y los mandos quietos desde «Soplar» — A.4 |
| 01/10 | sin hash, D10-5 | Una línea `RNF-04` por carga en `SceneLoader` y la prueba de «Omitir» en la segunda visita — A.5 |

`37b3cb7` y `5a22df1` entraron en `main` con el PR #88 (`1c7f4ab`, 30/09/2026).

### A.2 INC-47, cerrado (29/09/2026)

La cabecera y §6 dan INC-47 por abierto. Se cerró con la regla del 29/09: ante un conflicto gana el
juego y se corrige el documento. El guion §1.4.3, OE1 RF-14, RF-15 y RF-16 (nombre y texto) con las
celdas del Nivel 1 de §3.6.1, HU-04 a HU-07 y CU-05 describen ya los dos momentos —reunir en el
círculo y encender con fuerza y cercanía—, los mensajes por fuerza y por cercanía y la tablilla con
el último mensaje. El historial `FireFeedbackLog.Entries` vive solo en memoria y no lo lee nadie
fuera del panel (RNF-09). Lo único que el documento pedía y el juego no tenía —el montón humeante de
la convergencia— se implementó en `37b3cb7` (A.3). Detalle de la corrección en
`claudeDocs/INCONSISTENCIAS.md`, INC-47.

### A.3 `37b3cb7` (30/09/2026): el montón humea y la chispa se ve

- **Humo de la convergencia** (INC-47, guion §1.4.3.5, fila E6). Al alcanzar el mínimo de golpes
  efectivos entra el hilo de humo del montón (`pileSmoke`, `Humo` en `Level1_Cave`, con el clip
  `fx_n1_humo_nacer`), entre el montón y las piedras, y ya no se apaga: lo ganado no se retira por un
  fallo posterior (INC-32). Pruebas: `FirePanel_RF19_AlConvergerElMontonEmpiezaAHumearYNoDejaDeHacerlo`
  y `FirePanel_RF19_ElHumoSeDibujaEncimaDelMontonYDebajoDeLasPiedras`.
- **Chispa visible** (INC-68). Cada golpe efectivo muestra la chispa sobre el punto del fuego —0,2 s
  por golpe efectivo, hasta el mínimo— y el golpe de más con las piedras en su sitio la muestra
  desplazada hacia arriba, apagándose en el aire en 0,35 s; el golpe suave y las piedras mal puestas
  no dan chispa. En este commit era una cruz de dos `Image` (`RayoH`, `RayoV`) que se encogía hasta
  desaparecer; el 01/10 pasó a ser un solo rayo (A.4). Pruebas:
  `FirePanel_RF16_LaChispaSoloSeVeCuandoLasPiedrasChocanConFuerza` y
  `FirePanel_RF16_ElBrilloDeLaChispaDuraMasConCadaGolpeEfectivo`.
- **Algoritm deja de ser «estrella»** en las acotaciones de `N1_AparicionGuia` y
  `N1_NacimientoDelFuego` (INC-52). Lo vigila `Content_DA76_NingunTextoVisibleDescribeAAlgoritmComoUnaEstrella`.
- **Comentarios vencidos de §6, corregidos.** `FireIndicatorCollector`: «intentos» son los golpes no
  efectivos, por cercanía o por fuerza, y el error corregido se explica sin `StrikePosition`.
  `FireFeedbackLog`: el historial queda en memoria y la tablilla muestra solo el último mensaje. Las
  citas «guion §4.3» pasan a §1.4.3 en `FireLevelConfig`, `FireAttempt`, `FireMessages`,
  `StrikeOutcome` y `FeedbackLogView`.

### A.4 Correcciones del 01/10/2026 (tarjeta D10-1)

Las tres primeras son decisiones de Santiago del 30/09/2026 (acta D10, §5); la cuarta corrige dos
defectos que aparecieron al especificarlas. Solo el rayo contradice un documento (INC-119); las
otras tres no contradicen ninguno y no abren hallazgo.

- **Las piedras van sobre las hojas desde el primer cuadro del acercamiento.** `RaiseStones()` las sube
  justo después de poner el montón al frente en `EnterIgnitionAsync`, y `PlaceStones` ya solo las
  mueve: el deslizante de cercanía no las sube sobre la chispa ni sobre la llama. Pruebas:
  `FirePanel_RF14_DuranteElAcercamientoLasPiedrasNuncaQuedanBajoLasHojas` y
  `FirePanel_RF15_MoverLaCercaniaNoSubeLasPiedrasSobreLaChispa`.
- **La chispa es un solo rayo** (INC-119). Un trazo `#FFE9A8` de 4 u de grueso (8 px a 1080p) sale del
  punto del golpe en una dirección al azar (`SparkRandom`, un `System.Random` inyectable). El golpe
  efectivo cae en la mitad de abajo, a 0,205–0,22 del lado del montón, siempre sobre hojas y nunca
  sobre una piedra; el de más va a la mitad de arriba, a 0,52–0,6: pasa del montón y se apaga en el
  aire por debajo de la tablilla, con el mismo trazo y el mismo color, porque no es un castigo
  (CP-02). Un solo barrido cabeza-cola, sin volver (RNF-21). En la escena, `Chispa` lleva el pivote en
  la cola, `RayoH` se estira a sus cuatro lados y `RayoV` desaparece. Pruebas:
  `FirePanel_RF16_LaChispaEsUnRayoDelCentroQueCaeEnLasHojasEnUnaDireccionAlAzar`,
  `FirePanel_RF16_ConFuerzaDeMasElRayoPasaDeLasHojasYSeApagaEnElAire`,
  `FirePanel_RNF21_ElRayoDeLaChispaHaceUnSoloBarridoSinVolver` y la captura
  `FirePanel_RF16_ElRayoDeLaChispaSeVeSobreElMonton`; se ajustaron
  `FirePanel_RF16_LaChispaSoloSeVeCuandoLasPiedrasChocanConFuerza` y el ayudante `SparkDurationAsync`.
- **El humo nace en el punto del golpe y sube a la corona de la llama.** El `RectTransform` de `Humo`
  pasa a pivote en la base (0,5; 0), en el punto del fuego y de 87 × 150: es un hilo que no pasa de
  medio montón. Al soplar, `PlayIgnitionAsync` lo lleva hasta la corona de la llama
  (`FlameCrownFraction`, 0,25 de su alto), por detrás de ella, y lo encoge a 0,6
  (`SmokeCrownScale`) con el mismo avance que el quemado: un solo barrido, sin parpadeo. Al empezar el
  encendido la tablilla sube por código sobre el suelo, y con él sobre el humo; en la escena no se
  movió, para que una pieza soltada bajo la tablilla mientras se reúne siga encima y con clic.
  Pruebas: `FirePanel_RF19_ElHiloDeHumoNaceEnElPuntoDelGolpeYNoEsMasAltoQueMedioMonton`,
  `FireLevel_RF20_AlPrenderElFuegoElHumoSubeALaCoronaDeLaLlama`,
  `FireLevel_RF20_AlPrenderElFuegoElHumoNoSobrepasaElBordeDeArribaDeLaTablilla`,
  `FireLevel_RNF03_AlPrenderElFuegoLaTablillaQuedaPorEncimaDelHumo`,
  `FirePanel_RNF03_ReuniendoUnaPiezaBajoLaTablillaQuedaEncimaYAlAlcanceDelClic` y las capturas
  `FirePanel_RF19_ElHiloDeHumoSeVeFinoYNaceEnElCentroDelMonton` y
  `FireLevel_RF20_AlTerminarDePrenderElHumoAsomaPorLaCoronaDetrasDeLaLlama`. Los cuadros del humo
  conservan su nombre de entrega.
- **Desde «Soplar» los mandos descansan hasta que termina el nivel.** Un segundo «Soplar» relanzaba el
  encendido —el quemado volvía a cero y `CompleteLevel` corría dos veces— y un «Golpear» ponía la
  chispa sobre la llama y bajaba la luz un cuadro. La bandera `_igniting` guarda `Blow()` y
  `Strike()`, y `LockControls()` deja sin interacción «Soplar», «Golpear» y los dos deslizantes: se
  atenúan en un fundido de 0,1 s y no llevan candado, porque no es un «todavía no» ni un castigo
  (CP-02, comentado en el código). Pruebas: `FireLevel_RNF21_SoplarDosVecesNoReiniciaElEncendido`,
  `FireLevel_RF20_SoplarDosVecesCierraElNivelUnaSolaVez` y
  `FireLevel_RF20_DuranteElEncendidoLosMandosDelPanelNoResponden`.

Los cambios están en `FirePanelController.cs`, `Level1_Cave.unity` y `FirePanelTests.cs`. Unity
reordena el YAML de la escena al guardarla: el cambio real son tres `RectTransform` —`Humo`, `Chispa`
y `RayoH`— y los cuatro documentos de `RayoV`. `N1_Config`, los cuadros del fuego y del humo, los
`.anim` y `DraggablePiece` no cambian. La revisión adversarial del 01/10/2026 los aprobó con las
capturas a la vista: el rayo sale del centro y cae en las hojas, el humo es un hilo que nace entre las
piedras y, al prender, asoma por la corona detrás de la llama.

### A.5 Del carril del Slice 1, fuera del nivel (tarjeta D10-5)

- **`SceneLoader` deja una línea por carga** en el registro del reproductor:
  `RNF-04: «<escena>» cargó en <segundos> s`, con tres decimales y punto en cualquier cultura y sin
  pila (`LogOption.NoStacktrace`). Es el instrumento para medir RNF-04 sobre el ejecutable; la cifra
  solo va al registro, nunca a la pantalla (CP-03), y el aviso de más de 10 s se conserva. Prueba:
  `SceneLoader_RNF04_CadaCargaDejaSuTiempoEnElRegistro`. La tabla de mediciones está en
  `Slice 4/Slice-4-Resultados.md`, Anexo B.
- **«Omitir» en la segunda visita real** (INC-28).
  `NarrativeScene_INC28_OfreceOmitirEnLaSegundaVisitaTrasTerminarElNivel1` abre `N1_Apertura` con el
  perfil que deja el Nivel 1 terminado —su fase confirmada y el Nivel 2 alcanzado— y exige el botón;
  `NarrativeScene_INC28_NoMuestraOmitirLaPrimeraVezQueSeVeLaEscena` cubre la primera vez. El ayudante
  `OpenNarrative` de `NarrativeSceneTests` admite ahora las fases confirmadas del perfil. Sin cambio de
  código de producción: la regla es la de `NarrativeVisitPolicy`.

### A.6 Pruebas y cifras al 01/10/2026

| Assembly | 25/09 (`ccf77e6`) | 30/09 (`359365e`) | 01/10 | Qué lo movió |
|---|---|---|---|---|
| `Game.Levels.Fire.Tests` (EditMode) | 49 | 49 | 49 | — |
| `Game.Levels.Fire.PlayMode.Tests` | 39 | 43 | 59 | `37b3cb7`: +4 (humo y chispa). D10-1: +16 (`FirePanelTests` pasa de 38 a 54 casos; `FireSoundsTests` 3 y `CaveLightingTests` 2 no cambian) |

De las 16 nuevas de D10-1, las 13 de integración y aceptación salieron en rojo por el motivo
esperado antes del cambio, o —las guardas, que nacen en verde— con una mutación del código revertida
después; las tres de captura solo comprueban con `Assume` y se revisaron a ojo. La suite
del nivel pasó cinco veces seguidas sin intermitencias, y la suite completa del 01/10/2026, con el
Editor abierto a 1920 × 1080, dio EditMode 393 = 392 + 1 omitida de siempre y PlayMode 351 = 350 +
`RiverLevel_RNF05_…`, que mide la memoria del Editor y pasa aislada (1 660 MB). Las cifras por
assembly están en `Slice 4/Slice-4-Resultados.md`, «Corrida completa de la suite (01/10/2026)».

### A.7 Lo que sigue abierto al 01/10/2026

- **Dos parámetros sin prueba de valor** (§6): `IgnitionSeconds` y `BurnExtent`.
- **El pulso de «Pista»** sigue siendo permanente (§6).
- **«Pista» y la pausa** siguen activas durante el encendido, y «Pista» repite entonces la instrucción
  de soplar. Es inofensivo; se mira en el recorrido completo sobre el ejecutable.
- **Lo que asoma del humo en la corona**: a 16:9 quedan unos 15 px entre la última lengua de la llama
  y el humo. Se calibra con `FlameCrownFraction` y `SmokeCrownScale` mirando el ejecutable.
- **A 21:9** la tablilla baja y tapa la punta de la llama y el remate del rayo de más, y el humo
  asoma sobre ella. Queda fuera de alcance: el juego se mide a 16:9.
- **El golpe fuerte en plural.** El guion §1.4.3.4 dice «las chispas saltan por todas partes» y en
  pantalla hay un solo rayo; el texto no se toca sin Santiago.
- **PG-06** (los valores del Nivel 1 validados jugando con estudiantes) sigue a cargo de Santiago
  (acta D10, §5; `claudeDocs/tasks/OE4/Hoja-HUM.md`, H2).

### A.8 El ejecutable: licencias junto al `.exe` y cuadros del fuego a 1024 px (01/10/2026)

Apartado nuevo. Los dos cambios los pidió el ejecutable candidato, que es del carril de este slice
(el build portable), y los dos tocan el Nivel 1 o su entrega. La procedencia del candidato —HEAD
`359365e`, huellas del árbol sin commit y SHA-256 del `.exe`— está en
`claudeDocs/tasks/OE4/evidencias/build-rc1.md`.

- **Las licencias viajan con el ejecutable.** La MIT de Phosphor Icons, de los tres glifos del menú
  de pausa (INC-127), y la SIL OFL 1.1 de Nunito y Baloo 2 piden que el aviso acompañe cada copia
  que se distribuye, y los dos textos vivían solo en `Assets/`, que no viaja. `LicenseNotices`
  (`Game.EditorTools`, solo Editor) es un `IPostprocessBuildWithReport`: al terminar cada build de
  Windows de 64 bits copia `Assets/Game/Art/UI/Common/LICENSE-Phosphor.txt` y
  `Assets/Game/Art/Fonts/OFL.txt` a `Licencias/`, junto al `.exe`, y hace fallar el build si falta
  alguno, porque un entregable sin su licencia no debe pasar por bueno. La lógica es el método
  estático `CopyTo`; el callback solo le pasa la carpeta del build. `Game.EditorTools` gana un
  `AssemblyInfo.cs` con `InternalsVisibleTo("Game.EditorTools.Tests")`. Pruebas (EditMode, de 5 a 9
  en `Game.EditorTools.Tests`): `LicenseNotices_RNF23_ElEjecutableLlevaLasLicenciasDeTerceros`,
  `LicenseNotices_RNF23_UnBuildRepetidoSobrescribeLosAvisos`,
  `LicenseNotices_RNF23_SiFaltaUnAvisoElBuildFalla` y
  `LicenseNotices_RNF23_LosAvisosDelProyectoExistenDeVerdad`. El candidato trae `Licencias/` con
  `OFL.txt` y `LICENSE-Phosphor.txt`.
- **El paquete no cabía: los cuadros del fuego y del humo, a 1024 px** (INC-130, RNF-06). El primer
  build candidato (01/10, 05:17) pesó 866,7 MB frente al límite de 500 MB. Lo llenaban los cuadros de
  `Assets/Game/Art/Props/Fire/Animations/`: 67 PNG —33 de humo de 1123 × 1933, 13 de fuego normal de
  2144 × 2108 y 21 de fuego cenital de 500 × 278— que sumaban 533 MB sin comprimir. Ya eran un dibujo
  por clave, la convención de los `.anim`, así que no había arreglo sin pérdida. En pantalla el fuego
  normal se ve a unos 650 px como mucho y el humo a menos de 300, de modo que `ArtImportRules` hace
  una sola excepción: esa carpeta se importa con `maxTextureSize` 1024, **sin comprimir**, por el
  mismo motivo que la regla general (la rejilla de bloques de 4 × 4 sobre degradados largos). Los 67
  `.meta` cambian solo esa línea (4096 → 1024): el fuego normal queda en 1024 × 1007, el humo en
  595 × 1024 y el cenital no cambia. El tamaño en el mundo no se mueve, porque los píxeles por unidad
  se escalan con la textura. Los cuadros conservan su nombre de entrega. Prueba:
  `ArtImport_RNF06_LosCuadrosDelFuegoYElHumoSeImportanAMil24SinComprimir`, que salió en rojo antes
  del cambio; `ArtImport_RNF23_LasIlustracionesEntranSinComprimirYSinReducir` admite esa carpeta, y
  solo esa, después de comprobar que tampoco está comprimida. Resultado: los cuadros pasan de 533 a
  145,7 MB y el paquete, a **479,0 MB** (456,8 MiB), con 21 MB de margen. El build de 866,7 MB se
  conserva aparte como `Build/Algoritmia_2026-10-01_827MB`.
- **Lo comprobó la revisión del cambio**, con las pruebas que pintan esos cuadros en PlayMode
  (7/7) y las capturas ampliadas al doble, antes y después: el fuego y el humo del montón, del
  nacimiento del fuego, del cierre del Nivel 2 y de la escena final se ven igual, sin borrosidad ni
  dientes de sierra. Detalle en `Slice 4/Slice-4-Resultados.md`, «Verificación final y paquete
  (01/10/2026)».
- **Pruebas del nivel**: `Game.Levels.Fire.Tests` sigue en 49 y `Game.Levels.Fire.PlayMode.Tests` en
  59, todas en verde en la verificación final del 01/10/2026 (EditMode 433 = 432 + 1 omitida y
  PlayMode 364/364, antes de INC-130) y la EditMode completa siguiente (434 = 433 + 1 omitida).
- **Queda**: el comentario de `ArtImportRules` habla de «134 texturas»; son 67 PNG, y 134 es el
  número de entradas del informe del build. Corregirlo cambia la huella del árbol registrada en
  `build-rc1.md`, así que se corrige después de la pasada del OE4, anotando que solo cambia un
  comentario. Mirar el fuego y el humo a 1080p en el
  ejecutable, y la memoria (RNF-05), van con la pasada del OE4 sobre el candidato.
