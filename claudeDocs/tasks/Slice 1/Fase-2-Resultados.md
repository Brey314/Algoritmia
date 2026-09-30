# Fase 2 — Andamiaje mínimo: lo construido y sus resultados

**Slice 1 · Golden Path temprano** · Estado: **T09, T10 y T11 implementadas y verdes** —
corte de este documento: 8 de septiembre de 2026
Verificación vigente: **EditMode 57/57 · PlayMode 33/33 + 2 omitidas** (las omitidas son las de
verificación visual, que no corren en batchmode — §4.2) *(Vencido el 25/09/2026: es la cifra del
08/09; la vigente está en [`Slice-4-Resultados.md`](../Slice%204/Slice-4-Resultados.md), «Corrida
completa de la suite (25/09/2026)».)*
Plan técnico: [`plan.md`](plan.md) · Tablero: [`todo.md`](todo.md) · Contrato: `claudeDocs/SPEC.md`
Fase anterior: [`Fase-1-Resultados.md`](Fase-1-Resultados.md)

Este documento registra qué se implementó en la Fase 2, con qué pruebas se verificó y qué resultado
dieron. **El código de la fase está completo**; del Checkpoint C quedan dos casillas que no son de
código (§6). No reabre decisiones de `SPEC.md`: las cita.

> **Estado al 25/09/2026:** el texto de abajo es la foto del 08/09 y se conserva tal cual. Cinco
> cosas que aquí se leen como vigentes ya no lo son —dos de ellas, la ilustración a pantalla
> completa y la tipografía, desde el mismo 08/09—, y cada una lleva una nota fechada en su sitio:
> - la regla de «Omitir»: desde `d81cfc7` (17/09), «ya vista» es haber confirmado la **última**
>   fase del nivel, no una cualquiera (§3);
> - `N1_Guia` tiene tres pasos —`Reunir`, `Golpear`, `Soplar`— desde `863ef05` (15/09), y la ayuda
>   ya está cableada en la cueva (§2.4, §6);
> - la ilustración de `Narrative.unity` ocupa la pantalla entera desde `145a63e` (08/09), y la
>   salida de cada narrativa la declara su asset (§2.1, §2.2, §5);
> - `NarrativeScene_RF05_AvanzarLlegaHastaElFinalYSaleAOtraPantalla` ya no existe: la sustituyó
>   una prueba del encadenado del N1 (§4.2);
> - la tipografía dejó de ser la del sistema con `145a63e` (§6; el detalle, en el anexo de
>   [`Fase-1-Resultados.md`](Fase-1-Resultados.md)).
>
> El detalle está en el **Anexo** al final. La mecánica del Nivel 1 que hoy consume esta ayuda
> está en [`Fase-5-6-Resultados.md`](Fase-5-6-Resultados.md).

---

## 1. Alcance de la fase

La Fase 2 es el módulo `andamiaje`: la capa narrativa que envuelve al nivel jugable. Al cerrar esta
fase el juego se recorre desde el inicio hasta el menú de niveles pasando por la narrativa, pero
todavía no hay nada que jugar.

| Tarea | Qué entrega | Modo de prueba | Estado |
|---|---|---|---|
| T09 | `NarrativeSequence`, `DialogueRunner` y la política de «ya vista» | EditMode | ✅ terminada |
| T10 | Escena `Narrative` parametrizada y los tres assets del Nivel 1 | PlayMode | ✅ terminada |
| T10b | `playModeStartScene`: pulsar Play arranca siempre en `Boot` | manual | ✅ terminada |
| T11 | `HintPolicy`: ayuda a demanda y pista tras tres fallos | EditMode | ✅ terminada |

**T08b** (los botones de la Fase 1 lanzaban `NullReferenceException` fuera de `Boot`) se hizo justo
antes de abrir esta fase y está registrado en el `todo.md`; aquí se menciona porque su causa y la de
T10b son la misma y se cerraron juntas (§5).

---

## 2. Qué quedó en el repositorio

### 2.1 El contenido narrativo es un asset, no código (T09)

- **`DialogueLine`** — una línea: quién habla y qué dice. La acotación —lo que en el guion va en
  cursiva— es una línea más con el hablante vacío (`IsStageDirection`). Ambos campos son
  `[field: SerializeField]` con `[Tooltip]`, según CT-05 y RNF-18: corregir una línea es editar el
  asset, no recompilar.
- **`NarrativeSequence`** — el ScriptableObject de una escena narrativa: `Id`, `Level`,
  `Illustration` y `Lines`. **No es video** (RNF-06, peso del paquete): una ilustración fija con
  cuadros de texto secuenciales. *(Ampliado al 25/09/2026: hoy el asset declara también cómo
  sale —`NextSequenceId`, `NextPhase`, `EndsInCredits`, `IsReflectiveClosing`—, su ambiente y su
  puesta en escena —cámara, luz y objetos—. Ver Anexo, B.3.)*
- **`DialogueRunner`** — C# plano, sin dependencias de Unity. `Advance()` pasa una línea por clic y
  devuelve `false` al pasar la última, que es la señal de salida; `Skip()` salta al final **solo si
  está permitido**, y pedirlo sin permiso no hace nada ni lanza.
- **`NarrativeVisitPolicy`** — decide si el perfil ya vio las escenas de un nivel.

Con esto, **añadir una escena narrativa es crear un asset**: ni una escena, ni un estado del flujo,
ni una rama. Es la razón de que `GameState.Narrative` sea un estado parametrizado, como fija
`SPEC.md` §Arquitectura.

### 2.2 Una sola escena para las quince escenas narrativas del guion (T10)

- **`NarrativeSceneController`** — adaptador delgado en `Game.UI`. Lee
  `GameFlow.NarrativeSequenceId`, busca la secuencia con ese id en su lista y la reproduce.
  **No tiene un solo `if` por secuencia**: las tres del Nivel 1 recorren exactamente el mismo
  código. No lleva botón de pausa, y es deliberado — HU-17 FA-04 lo pide solo durante el juego.
- **Escena `Narrative.unity`** — construida por script del Editor (nunca `.meta` a mano):
  `EventSystem` con `InputSystemUIInputModule` (no el módulo legado, CT-06), `Canvas` Screen
  Space-Overlay con `CanvasScaler` a 1920×1080 match 0.5, la ilustración en la mitad superior, el
  cuadro de diálogo en la franja inferior con hablante y cuerpo, y los botones **Continuar** y
  **Omitir**. Paleta de `Direccion_de_Arte.md`: pergamino `#F7EFE2`, tinta `#3A1E18`, ámbar
  `#E8A33D`, cuadro `#E0D4C0`. Añadida a Build Settings, que pasa a **5 escenas**. *(Vencido el
  25/09/2026 en cuanto a la ilustración: desde `145a63e` (08/09) `Ilustracion` está anclada de (0,0)
  a (1,1), a pantalla completa, y la encuadra `IllustrationFraming.Apply`; el cuadro de diálogo
  sigue abajo. Ver Anexo, B.3.)*
- **Tres assets del Nivel 1** en `Assets/Game/Data/Narrative/`: `N1_Apertura` (7 líneas, guion
  §3.1), `N1_AparicionGuia` (11 líneas, §4.1) y `N1_Hallazgo` (18 líneas, §4.2). *(Al 25/09/2026
  son cuatro: `ca6c8c6` (11/09, T15) sumó el cierre reflexivo `N1_NacimientoDelFuego`, de 17 líneas.
  Las tres de aquí conservan su número de líneas.)*
- **`GameState.Narrative` mapeado** a la escena `Narrative` en `GameFlowRunner.Scenes`. Antes no
  estaba, y esa ausencia es la que rompía el recorrido (§5).
- **`Game.UI` pasa a referenciar `Game.Scaffolding`.** El test de arquitectura solo veta que `Core`
  dependa de UI/Audio/niveles y que un nivel referencie a otro (RNF-15, RNF-16, INC-40); la
  dirección UI → Scaffolding es la natural y ninguna regla la prohíbe.

### 2.3 Pulsar Play arranca siempre en `Boot` (T10b)

`Assets/Game/Scripts/Editor/PlayFromBoot.cs` fija `EditorSceneManager.playModeStartScene`. Es la
función nativa de Unity para este caso exacto y **solo afecta al Editor**: en el ejecutable `Boot`
ya es la primera escena de Build Settings. Para depurar una escena aislada basta con vaciar el campo
en Project Settings.

---

### 2.4 La ayuda son dos mecanismos distintos, no uno (T11)

RF-13 pide **dos cosas separadas** y el andamiaje las mantiene separadas:

- **`GuideStep`** — una tarea vista desde el guía: `Id`, `Instruction` y `Hint`. Dos campos y no
  uno, porque confundirlos convertiría la ayuda en la respuesta (CP-06).
- **`GuideContent`** — un ScriptableObject por nivel con sus tareas. Igual que una escena narrativa
  es un asset, **añadir una tarea del guía es editar contenido** (CT-05, RNF-18).
- **`HintPolicy`** — C# plano, sin Unity. `RequestHelp()` devuelve la instrucción vigente y **no
  toca ningún contador**: si pedir ayuda contara como intento, usarla adelantaría la pista y el
  andamiaje pasaría a resolver (HU-03 FA-02). `RegisterFailedAttempt()` devuelve la pista al
  tercer fallo consecutivo y reinicia la cuenta (guion §4.3.5 E5); `RegisterSuccessfulAttempt()`
  también la reinicia, y `Activate()` igual —los tres fallos son «en una misma tarea» (RF-13), y
  solo hay una activa a la vez (RNF-03)—.
- **`Assets/Game/Data/Guide/N1_Guia.asset`** — las dos tareas del Nivel 1 (`Golpear`, `Soplar`). La
  pista de `Golpear` es la formulación del guion §4.3.6, palabra por palabra: «Las chispas caen
  hacia abajo y se apagan rápido. ¿Cuánto camino tienen que recorrer antes de tocar las hojas?»
  Orienta hacia la distancia sin nombrar la posición efectiva. *(Vencido el 25/09/2026: la pista de
  `Golpear` dejó de ser la del guion §4.3.6 con `fa5d032` (12/09), cuando la mecánica dejó de medir
  la distancia (INC-47); desde `863ef05` (15/09) son tres pasos —`Reunir`, `Golpear` y `Soplar`—, el
  encendido tiene dos deslizantes, fuerza y cercanía de las piedras, y la pista nombra las dos. Ver
  Anexo, B.2.)*

El umbral vive en `HintPolicy.DefaultAttemptsForHint = 3` (`intentosParaPista` del guion §4.3.2) y
el constructor admite otro valor, que es lo que usará `FireLevelConfig` en T12 sin tocar esta clase.

## 3. La decisión de diseño que cambió el plan

El plan pedía que `CanSkip` fuera falso «la primera vez que el perfil ve la escena» (INC-28, RF-06).
La lectura obvia —guardar en el perfil la lista de escenas vistas— **está prohibida**: RNF-09, con la
letra fijada en INC-27, cierra lo que se persiste en «nombre o alias, nivel alcanzado, fases
confirmadas y los cuatro indicadores. **Nada más**». Ampliar esa lista sería modificar un entregable
ya radicado, que `CLAUDE.md` marca como «preguntar primero».

Así que «ya vista» **se deriva del progreso** en vez de persistirse: si el perfil confirmó alguna
fase del nivel, ya pasó por sus escenas antes. *(Vencido el 25/09/2026: desde `d81cfc7` (17/09)
«ya vista» exige la **última** fase del nivel confirmada, no una cualquiera
(`NarrativeVisitPolicy.cs:53-54`); el cierre reflexivo va aparte y usa `ReachedLevel` desde
`9f10485` (10/09). Lo que sigue vale igual: la señal sale del progreso, no de un campo nuevo. Ver
Anexo, B.1.)* Es exactamente la consecuencia que pide HU-14 — la
primera vuelta se lee entera, porque es donde el guía nombra la habilidad practicada (RF-12, CP-07),
y solo quien repite puede saltar. La razón está escrita en el `<remarks>` de `NarrativeVisitPolicy`,
no solo aquí: sin esa nota, una futura «mejora» añade el campo al perfil y rompe RNF-09.

**Efecto secundario honesto:** el botón de omitir no se puede ver todavía en el juego real, porque
no hay forma de confirmar una fase hasta **T17**. La regla está probada en EditMode; su verificación
en pantalla queda anotada en el Checkpoint C. *(Al 25/09/2026: ver la nota de §6.)*

---

## 4. Verificación

### 4.1 EditMode — declarado

**57/57, 0 fallos** (`unity test --mode EditMode`, 08/09/2026). Eran 34 al cerrar la Fase 1, 44 tras
T09 y **50** tras el RF-47 del commit `145a63e`; las diecisiete nuevas son de esta fase:

| Prueba | Requisito |
|---|---|
| `DialogueRunner_RF05_AvanzaUnaLineaPorClic` | RF-05 |
| `DialogueRunner_RF05_TerminaDespuesDeLaUltimaLinea` | RF-05 |
| `DialogueRunner_RF05_UnaSecuenciaSinLineasNaceTerminada` | RF-05 |
| `DialogueRunner_RF06_NoOfreceOmitirLaPrimeraVez` | RF-06 |
| `DialogueRunner_RF06_OfreceOmitirSiLaEscenaYaFueVista` | RF-06 |
| `DialogueRunner_INC28_OmitirLaPrimeraVezNoSaltaLaEscena` | INC-28 |
| `DialogueRunner_INC28_OmitirUnaEscenaYaVistaLlegaAlFinal` | INC-28 |
| `NarrativeSequence_RNF01_NingunaOracionSupera20Palabras` | RNF-01 |
| `NarrativeSequence_RNF18_CadaSecuenciaTieneIdYLineas` | RNF-18 |
| `NarrativeSequence_RF05_LosIdentificadoresNoSeRepiten` | RF-05 |
| `HintPolicy_RF13_AyudaADemandaNoAlteraElEstado` | RF-13, HU-03 |
| `HintPolicy_RF13_PistaSeOfreceAlTercerFalloConsecutivo` | RF-13 |
| `HintPolicy_RF13_UnAciertoReiniciaLosFallosConsecutivos` | RF-13, HU-04 |
| `HintPolicy_RNF03_CambiarDeTareaReiniciaLosFallosYLaInstruccion` | RNF-03 |
| `HintPolicy_CP06_LaPistaNuncaNombraLaPosicionEfectiva` | CP-06, guion §4.3.6 |
| `HintPolicy_RNF18_CadaTareaTieneInstruccionYPista` | RNF-18 |
| `HintPolicy_RNF01_NingunaOracionDelGuiaSupera20Palabras` | RNF-01 |

Las tres últimas de `HintPolicy` y las tres de `NarrativeSequence` no prueban una clase sino **el
contenido de los assets**: los textos viven fuera
del código (CT-05, RNF-18), así que el límite de legibilidad de RNF-01 hay que verificarlo sobre el
asset. Por eso el guion se reescribió al pasarlo a los assets: varias frases de §3.1 y §4.1 pasaban
de 20 palabras y se partieron conservando el sentido y el diálogo literal.

### 4.2 PlayMode — declarado

**33/35, 0 fallos, 2 omitidas, 0 inconclusive** (`unity test --mode PlayMode`, 08/09/2026, 24 s).
Eran 27 pruebas al cerrar la Fase 1; las seis nuevas son de esta fase (T11 no toca PlayMode, así
que la corrida del 08/09 solo comprueba que no hay regresión):

| Prueba | Requisito |
|---|---|
| `NarrativeScene_RF05_ResuelveTresSecuenciasDistintasSinRamas` | RF-05 |
| `NarrativeScene_RF05_LaPrimeraLineaEsLaDelAssetPedido` | RF-05 |
| `NarrativeScene_RF05_AvanzarLlegaHastaElFinalYSaleAOtraPantalla` | RF-05, RNF-13 |
| `NarrativeScene_INC28_NoMuestraOmitirLaPrimeraVezQueSeVeLaEscena` | INC-28, RF-06 |
| `NarrativeScene_HU17_NoHayBotonDePausaEnUnaEscenaNarrativa` | HU-17 FA-04 |
| `NarrativeScene_RNF01_LaLineaMasLargaCabeEnSuCuadroDeDialogo` | RNF-01 |

*(Vencido el 25/09/2026: `NarrativeScene_RF05_AvanzarLlegaHastaElFinalYSaleAOtraPantalla` ya no
existe. `1009c5a` (13/09) la sustituyó por
`NarrativeScene_RF10_LaAperturaEncadenaLasTresEscenasDelNivel1YEntraAJugar`, que recorre
`N1_Apertura` → `N1_AparicionGuia` → `N1_Hallazgo` y comprueba que sale a jugar la fase 1 del
Nivel 1. Las otras cinco siguen en la clase. Ver Anexo, B.4.)*

**Las 2 omitidas son las de verificación visual** (`MainMenu_RNF20_*`, `LevelSelect_RNF19_*`):
`ScreenCapture` no encuentra la vista de juego en batchmode. En el corte del 07/09 **fallaban**;
desde el commit `145a63e` se saltan solas cuando no hay vista de juego, que es lo correcto — un
fallo ahí no distinguía «la captura no cumple RNF-20» de «no hay dónde capturar». Siguen
verificándose desde el Test Runner del Editor con la vista de juego abierta.

### 4.3 Dos defectos encontrados al verificar — y corregidos

Llegar a esa cifra costó tres corridas. Las dos primeras no dieron un número peor: no dieron
ninguno, y conviene que quede escrito por qué.

**a) `playModeStartScene` colgaba la suite entera.** La primera corrida se agotó a los 1500 s sin
producir informe. La causa está en el log, no en una hipótesis: el Test Framework importa su
`InitTestScene<guid>.unity` y acto seguido se carga `Boot.unity` — T10b secuestraba la entrada a
Play **de todo el mundo**, incluido el corredor, que se quedaba esperando una escena que ya no
llegaba. Corregido con dos guardas, porque hay dos puertas de entrada: batchmode (`unity test`) y
los callbacks de `TestRunnerApi` (la ventana Test Runner del Editor, que es la que hace falta para
las dos pruebas de verificación visual). Sin la segunda, la trampa seguiría puesta para quien corra
PlayMode desde el Editor. El porqué está escrito en `PlayFromBoot.cs` con la fecha y la evidencia.

**b) Una prueba nueva pasaba sin probar nada.**
`NarrativeScene_RF05_ResuelveTresSecuenciasDistintasSinRamas` recorre las tres secuencias en un
bucle y creaba un `GameFlowRunner` por vuelta, pero el `[TearDown]` solo limpia **entre** pruebas.
Como los objetos `DontDestroyOnLoad` sobreviven a `LoadSceneMode.Single`, en la segunda vuelta ya
había un runner vivo, `GameFlowRunner.Awake` destruía el duplicado —correcto, RNF-16— y la prueba
se quedaba con una referencia muerta cuya FSM nunca salió de `Boot`. Como la comprobación era un
`Assume`, el resultado no fue un fallo sino un **«inconclusive»**: la prueba pasó sin probar nada
durante una corrida completa. **Es el mismo defecto que ya costó una corrección en T04**
(`Fase-0-Resultados.md` §3.4). Dos cambios: la limpieza de persistentes se hace también al inicio
del helper, y esa comprobación sube de `Assume` a `Assert` con mensaje, para que un arreglo roto
falle fuerte en vez de callar.

Ambos defectos eran del andamiaje de verificación, no del código de producción — pero un número
verde obtenido con cualquiera de los dos en pie no habría significado nada.

### 4.4 Verificación manual, conducida sobre el juego real

Recorrido completo desde `Boot`, con el Editor en Play:

| Paso | Resultado |
|---|---|
| Play con `Narrative` abierta | → `Boot` → `MainMenu` ✅ |
| «Créditos» → «Volver» | `Credits` y vuelta a `MainMenu` ✅ |
| «Jugar» | abre el panel de perfil ✅ |
| Nombre repetido | «Ya hay un perfil con ese nombre. Elige otro.» (HU-01 FA-02) ✅ |
| Crear perfil nuevo | FSM `Narrative`, escena `Narrative`, perfil activo ✅ |
| 7 clics en «Continuar» | 7 líneas **distintas**, de «Noche helada…» a «Solo oscuridad.» ✅ |
| Fin de la narrativa | → `LevelSelect` ✅ |
| «La Oscuridad» en `LevelSelect` | → `Narrative` otra vez ✅ |

---

## 5. Por qué esta fase empezó arreglando la Fase 1

Al probar las escenas de la Fase 1 aparecieron **dos problemas superpuestos** que se leían como uno
solo («ninguno de los botones redirige»):

1. **Pulsar Play sobre una escena suelta.** Los tres objetos persistentes los crea `Boot`; sin él
   `GameFlowRunner.Instance` es `null` y cada clic lanzaba `NullReferenceException` —las cuatro
   escenas, con línea exacta en `Logs/Editor.log`—. Lo cerró **T08b** (`ScreenFlow`, un punto único
   de comprobación) y lo eliminó de raíz **T10b** (`playModeStartScene`).
2. **`Narrative` no tenía escena.** `ProfileSelectController.CreateNew` encadena dos transiciones:
   `SelectProfile` (→ `LevelSelect`, encola su carga) y `StartNarrative` (→ `Narrative`, que no
   estaba en `GameFlowRunner.Scenes`). El resultado era quedarse **de pie en `LevelSelect` con la
   FSM en `Narrative`**, y como desde `Narrative` solo son legales `Playing`, `LevelSummary` y
   `LevelSelect`, ningún botón de esa pantalla respondía. Es justo el «estado irrecuperable» que
   RNF-13 prohíbe. Lo cerró **T10** al crear la escena y mapear el estado.

La salida de una escena narrativa es hoy `LevelSelect`, con un comentario que marca el cambio: la
salida natural es entrar a jugar, y pasa a `StartPlaying` cuando **T14** traiga `Level1_Cave`.
*(Vencido el 25/09/2026: hoy la salida la declara el asset, y `NarrativeSceneController.Leave()`
(`NarrativeSceneController.cs:821-852`) la resuelve en este orden: `NextSequenceId` → `NextPhase` →
`EndsInCredits` → `IsReflectiveClosing` → `LevelSelect`. Ver Anexo, B.3.)*

---

## 6. Lo que queda abierto

- **Ver el botón de omitir en la segunda visita** — la regla está probada en EditMode y la escena la
  respeta, pero verla en pantalla exige una fase confirmada, es decir **T17** (§3). Es la única
  casilla del Checkpoint C que no depende de esta fase. *(Al 25/09/2026: T17 está hecha y la segunda
  visita es alcanzable, pero la casilla sigue en `[~]` en el `todo.md` y ninguna prueba PlayMode
  comprueba que el botón aparezca; las de pantalla solo comprueban que no aparece la primera vez,
  entre ellas `NarrativeScene_INC28_NoMuestraOmitirLaPrimeraVezQueSeVeLaEscena`,
  `LevelSummary_CP07_ElCierreReflexivoNoEsOmitibleLaPrimeraVez`,
  `LevelSummary_RF03_DevuelveAlMenuConNivel3Desbloqueado` (`LevelSummaryTests.cs:144`, desde
  `8de614f`, 15/09) y `GameEnding_INC39_RecorreLevelSummaryNarrativeCreditsYMainMenu`
  (`GameEndingTests.cs:82`, desde `1179dae`, 21/09).)*
- **Cablear `HintPolicy` a una pantalla** — T11 entrega la regla y su contenido, no un botón: la
  escena jugable donde vive el botón de ayuda es **T14**, y quien cuenta los fallos que la alimentan
  es **T12**. Hasta entonces la política está probada pero no se ve. *(Resuelto al 25/09/2026: en la cueva
  la cablea `FirePanelController`, que crea la `HintPolicy` con el paso `Reunir` y activa `Golpear`
  y `Soplar` (`FirePanelController.cs:210`, `:409`, `:509`); ver
  [`Fase-5-6-Resultados.md`](Fase-5-6-Resultados.md). Los controladores de los niveles 2 y 3 crean
  la suya.)*
- **Ilustraciones** — las tres secuencias tienen el campo `Illustration` vacío y la escena oculta la
  imagen si no hay sprite. Son los assets A1–A6 del tablero, todavía sin generar. *(Vencido el
  25/09/2026: los cuatro `N1_*.asset` tienen ilustración: `N1_Apertura` usa `entorno_n1_apertura` y
  los otros tres, `entorno_n1_cueva_2x`.)*
- **Tipografía** — fuente del sistema, igual que en la Fase 1. Baloo 2 / Nunito
  (`Direccion_de_Arte.md` §11.2) sigue siendo tarea de assets. *(Vencido el 25/09/2026: las dos
  entraron con `145a63e` (08/09) y hoy las usan los cuatro textos de `Narrative.unity`; ver el anexo
  de [`Fase-1-Resultados.md`](Fase-1-Resultados.md), A.1.)*

---

## Anexo — lo que cambió después del cierre (25/09/2026)

Verificado contra el código y los assets de `ccf77e6`. Recoge lo que cambió en las piezas de esta
fase. La puesta en escena que se construyó encima de la escena `Narrative` —cámara por paradas,
luz, objetos y personajes— entró con otras tareas y aquí solo se nombra.

### B.1 La regla de «Omitir» (RF-06, INC-28)

`NarrativeVisitPolicy.AlreadySeen` tiene hoy dos ramas, y ninguna amplía lo persistido (RNF-09):

1. **Cierre reflexivo** (`IsReflectiveClosing`): `profile.ReachedLevel > sequence.Level`
   (`NarrativeVisitPolicy.cs:43-46`), desde `9f10485` (10/09, W04). Al cierre se llega la primera vez
   justo después de confirmar la última fase, así que la señal de las demás escenas ya sería cierta
   y ofrecería omitir justo donde CP-07 y RF-12 lo prohíben. Lo que distingue la primera vuelta es
   que el nivel siguiente aún no está desbloqueado. De ahí el orden que se le impone a quien cierre
   un nivel: primero el cierre reflexivo, después el desbloqueo.
2. **Cualquier otra escena**: haber confirmado la **última** fase del nivel,
   `PhaseId.PhaseCountOf(sequence.Level)` (`NarrativeVisitPolicy.cs:53-54`), desde `d81cfc7` (17/09,
   «las escenas solo se pueden saltar si el nivel completo ha sido terminado previamente»). Con la
   regla de §3, al salir del bosque con la fase 1 recién confirmada, la 2.2 ofrecía «Omitir» la
   primera vez que se veía; el comentario del código anota que lo vio Santiago el 17/09/2026. Como lo
   aprobado no se pierde (RF-41), en las vueltas siguientes la última fase sigue confirmada.

En el Nivel 1, que tiene una sola fase (`PhaseId.PhasesPerLevel = { 1, 3, 3 }`), las dos reglas
coinciden; la diferencia está en los niveles 2 y 3.

**INC-51** (abierto, decisión tomada: se acepta). El Nivel 3 no tiene nivel siguiente, así que su
cierre (`N3_Escena33_Cruce`) y la escena final (`N3_EscenaFinal`) nunca cuentan como vistos y se leen
enteros también al repetir el nivel. Lo anota el comentario de `NarrativeVisitPolicy.cs:39-42`,
añadido con `13e2986` (23/09). *(Nota 29/09/2026: cerrado; HU-14 FA-01 y CU-03 2a ya recogen la excepción del Nivel 3.)*

Pruebas de la regla:

| Prueba | Requisito | Entró con |
|---|---|---|
| `NarrativeVisitPolicy_RF06_ElCierreDelNivel2NoEsOmitibleLaPrimeraVez` | RF-06, CP-07 | `9f10485` (10/09) |
| `NarrativeVisitPolicy_RF06_UnaEscenaIntermediaNoSeOmiteEnLaPrimeraVuelta` | RF-06 | `d81cfc7` (17/09) |
| `NarrativeVisitPolicy_RF06_SinPerfilActivoNuncaSeOfreceOmitir` | RF-06 | `9f10485` (10/09) |
| `NarrativeVisitPolicy_CP07_ElCruceYLaEscenaFinalNoSeOmitenLaPrimeraVez` (en `NarrativeSequenceTests`) | CP-07, RF-12 | `e3575bf` (16/09) |
| `LevelSummary_CP07_ElCierreReflexivoNoEsOmitibleLaPrimeraVez` (PlayMode) | CP-07 | `f8ffcc8` (11/09, T18) |

La segunda es la que vigila el cambio de `d81cfc7`: con la fase 1 del Nivel 2 confirmada, la 2.2 y
la 2.3 no se omiten; con la 2, tampoco la 2.4; con la 3, `N2_PuenteI`, la 2.2 y la 2.4 ya sí.

### B.2 `N1_Guia`: tres pasos

Desde `863ef05` (15/09), `Assets/Game/Data/Guide/N1_Guia.asset` tiene tres pasos:

| Paso | Instrucción | Pista |
|---|---|---|
| `Reunir` | «Reúne todas las hojas y las dos piedras en el centro de la pantalla.» | «Todo tiene que quedar dentro del círculo. ¿Qué pieza sigue fuera?» |
| `Golpear` | «Elige la fuerza y qué tan cerca van las piedras, y golpea para hacer chispas.» | «Las chispas dependen de cómo chocan las piedras: de la fuerza y de qué tan juntas están. ¿Qué cambiarías antes del próximo golpe?» |
| `Soplar` | «Sopla despacio sobre el montón de hojas para avivar el humo.» | «El humo ya está ahí. ¿Está todo junto y cerca? ¿Qué le falta para volverse llama?» |

`HintPolicy` no cambió para esto: el paso es contenido (CT-05). La cablea `FirePanelController`
(`new HintPolicy(Step("Reunir"), config.AttemptsBeforeHint)` en `FirePanelController.cs:210`), con
el umbral de `N1_Config.asset`, que sigue en 3. `HintPolicy_CP06_LaPistaNuncaNombraLaPosicionEfectiva`
sigue en la suite y sigue comprobando que ninguna pista del N1 diga «muy cerca». La mecánica que
consume estos pasos está en [`Fase-5-6-Resultados.md`](Fase-5-6-Resultados.md).

### B.3 La escena `Narrative` y lo que declara cada secuencia

- **Ilustración a pantalla completa** desde `145a63e` (08/09): el `RectTransform` de `Ilustracion`
  va anclado de (0,0) a (1,1); en `2a588f3` iba anclado arriba al centro. La encuadra
  `IllustrationFraming.Apply` (`NarrativeSceneController.cs:743`). Encima, el controlador crea al
  arrancar la capa `Oscuridad` con `darknessMaterial` (`fx_oscuridad.mat`,
  `NarrativeSceneController.cs:41-42`, `:354`). `CuadroDialogo` sigue abajo al centro, 1400×180.
- **La salida la declara el asset.** `Leave()` (`NarrativeSceneController.cs:821-852`) prueba, en
  orden: `NextSequenceId` (`b6b886c`, 12/09) → `NextPhase` (`faaaa13`, 10/09) → `EndsInCredits`
  (`1179dae`, 21/09) → `IsReflectiveClosing` (`9f10485`, 10/09) → `LevelSelect`. En el Nivel 1,
  `N1_Apertura` → `N1_AparicionGuia` → `N1_Hallazgo` se encadenan por `NextSequenceId` desde
  `1009c5a` (13/09), y `N1_Hallazgo` entra a jugar con `NextPhase: 1`. `N1_NacimientoDelFuego`
  (`ca6c8c6`, 11/09, T15) es el cierre reflexivo y sale al resumen del nivel.
- **Ilustraciones del N1.** `N1_Apertura` usa `entorno_n1_apertura`; `N1_AparicionGuia`,
  `N1_Hallazgo` y `N1_NacimientoDelFuego`, `entorno_n1_cueva_2x`.
- **Puesta en escena**, fuera del alcance de esta fase: `NarrativeSequence` declara además
  `Ambient` y `AmbientLayer`; `CameraStart`, `CameraEnd`, `CameraKeys` y `Props` (`416ec79`, 11/09,
  W07); `LightStart` (`fa5d032`, 12/09); `CameraSmoothingSeconds`, `OpensFromBlack` y
  `HardCutFadeSeconds`. `DialogueLine` suma `Sound`, `Ambient` y `Silence`.
- **`DialogueRunner`** ganó en `416ec79` (11/09, W07) `Progress` —cuánto va leído, de 0 a 1, «lo que
  lleva la cámara por la ilustración al ritmo de la narrativa y no del reloj»— e `Index`, la línea
  en curso. `Advance` y `Skip` no cambiaron.

### B.4 Las suites de esta fase, hoy

Contadas por grep sobre `Assets/Tests/` en `ccf77e6`:

| Suite | 08/09 | Hoy | Qué se sumó |
|---|---|---|---|
| `DialogueRunnerTests` | 7 | 8 | `DialogueRunner_RF05_ElProgresoVaDeCeroAUnoAlRitmoDeLasLineas` (`416ec79`) |
| `HintPolicyTests` | 7 | 15 | cuatro del Nivel 2 (`5f7a432`, 10/09, W03) y cuatro del Nivel 3 (`e3575bf`, 16/09) |
| `NarrativeSequenceTests` | 3 | 11 | ocho de otras tareas: cuántas secuencias tienen el N2 y el N3, la luz del día del N2 (`ElNivel2TranscurreDelAmanecerALaNoche`, `d7ace67`), ningún encuadre del N3 se recorta (`e3575bf`), objetos bajo el cuadro de diálogo (`1009c5a`), cada objeto pintado tiene su ilustración (`d81cfc7`), humo (`f801186`) y la CP-07 de B.1 |
| `NarrativeVisitPolicyTests` | — | 3 | la clase entera (B.1) |
| `NarrativeSceneTests` (PlayMode) | 6 | 29 pruebas, 75 casos | la del encadenado del N1 (§4.2) y las de la puesta en escena de otras tareas; de las seis de §4.2 se retiró una |

Las diecisiete EditMode de §4.1 siguen todas en la suite. La cifra de la suite completa está en
[`Slice-4-Resultados.md`](../Slice%204/Slice-4-Resultados.md), «Corrida completa de la suite
(25/09/2026)».
