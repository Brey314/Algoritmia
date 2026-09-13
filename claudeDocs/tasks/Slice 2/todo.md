# Tablero — Slice 2: La Rueda

Plan técnico: [`plan.md`](plan.md). Contrato: `claudeDocs/SPEC.md`.
Cada tarea se cierra con su commit asociado (RNF-17, CT-11).

**Leyenda:** `EM` = EditMode (lógica pura, sin escena) · `PM` = PlayMode (integración) ·
`VV` = VisualVerification · `MCP` = la verificación **exige** el corredor de pruebas conectado.

> ⚠️ **R2 — el Slice 1 no está hecho.** Este slice generaliza ocho piezas del Slice 1 que aún no
> existen. **Actualizado el 10/09/2026:** el reparto entre carriles es **por assembly**, no por
> slice. Independientes de la Fase 3 del Slice 1: **W01, W03, W04, W10, W11, W12**. **W02 ya se
> hizo** —el freno era su colisión con T17, que aún no había tocado `PlayerProfile.cs`—. Siguen
> esperando código del Slice 1: **W15** (necesita `ILevelReporter`, de T17), **W17** (T16) y
> **W16** (ambas).

> ⚠️ **R1 abierto — no hay corredor de pruebas MCP.** `run_unity_tests` sigue sin conectar.
> Toda casilla marcada `MCP` exige haberla corrido **a mano** en la ventana Test Runner del
> Editor y **declarar el resultado**. No dar por hecho que la suite pasó.

---

## Fase 0 — Cimientos del slice

- [x] **W01 · Assembly `Game.Levels.Wheel` y prueba de exclusión real** — `XS` · `EM` (10/09/2026)
      RNF-15, RNF-16, INC-40 · depende de: Slice 1 T01
      `Game.Levels.Wheel.asmdef` (→ `Game.Core`, `Game.Scaffolding`) y
      `Game.Levels.Wheel.Tests.asmdef`, **copiados del par de `Fire`** y sin `.cs` todavía. Los
      cuatro `.meta` los generó Unity al importar; ninguno se escribió a mano.
      **El plan decía «referencia única: `Game.Core`»; se siguió la cadena de `SPEC.md`**
      (`Core` → `Scaffolding` → `Levels/*`) y la forma del `Fire` que ya está en disco: el nivel
      consume `DialogueRunner` y `HintPolicy`, que viven en `Game.Scaffolding`. Los criterios de
      aceptación de W01 solo vetan nivel→nivel y `Core`→nivel, y ambos siguen verdes.
      **No hizo falta una prueba nueva**: registrar el módulo en la tabla `RuntimeModules` de
      `AssemblyDependencyTest` hace que los cuatro casos existentes cubran el assembly nuevo, y es
      lo que convierte la exclusión de RNF-16 en real —antes solo había un nivel que comparar—.
      RED **3/4** (`Expected: collection containing "Game.Levels.Wheel"` y dos
      `KeyNotFoundException`) → GREEN **EditMode 63/63**, 0 fallos, 0 omitidas.
- [~] **W02 · `PhaseId` y desbloqueo secuencial del Nivel 2** — `M` · `EM` (10/09/2026)
      RF-03, RF-04, RNF-09, RNF-14, HU-14, CU-06, INC-27, CP-02 · depende de: W01, Slice 1 T02/T07
      **La lógica está cerrada; falta el consumidor en juego, que no es de este slice** — ver
      «lo que queda pendiente» abajo.
      `PhaseId` (`Game.Core`, `readonly struct` `LevelId` + fase en base 1) con la tabla de fases
      por nivel —**1 / 3 / 3**, Nivel 3 según RF-40 e INC-30— y `AllOf`. `PlayerProfile` pasa a
      hablar `PhaseId` en `ConfirmPhase`/`IsPhaseConfirmed`/`IndicatorsFor`/`ConfirmedPhases` y
      gana `IsLevelComplete` y `NextPendingPhase` (la fase de retoma de RNF-14, `null` si el
      nivel está completo). `LevelUnlockPolicy.UnlockAfterCompleting` **deja de creerle al
      llamante**: solo habilita el nivel siguiente si están confirmadas todas las fases del que
      se completó — con un nivel de una fase daba igual, con las tres del Nivel 2 abriría el
      Nivel 3 a media rueda.
      **`SaveStore` no necesitó cambios**, y es el punto: `PhaseId` es un tipo de tiempo de
      ejecución, no la forma del archivo. El JSON sigue escribiendo `level` y `phase` sueltos, así
      que la lista cerrada de RNF-09 queda intacta y los perfiles ya guardados se releen igual.
      **Deviación de nombre**: el plan pedía `LevelSelect_RF03_Nivel2BloqueadoHastaCompletarNivel1`
      en EditMode, pero el sujeto ahí no es la pantalla sino la regla; va como
      `LevelUnlockPolicy_RF03_…`. `LevelSelect` sigue cubierto por la prueba PlayMode que ya
      existía, ampliada para confirmar la fase antes de desbloquear.
      RED: la suite EditMode **no compilaba** (`CS0246: PhaseId`, `CS1061: NextPendingPhase`,
      `IsLevelComplete`) → GREEN **EditMode 70/70** (63 + 7 nuevas), 0 fallos, 0 omitidas, y
      **PlayMode 33/35**, 0 fallos, las 2 omitidas son las `VisualVerification` de siempre, que
      batchmode no ejecuta. 0 errores y 0 warnings de compilación (`unity test`, Editor cerrado).
      **Lo que queda pendiente y por qué**
      - `[ ]` **Nadie confirma una fase jugando todavía.** No hay una sola llamada a
        `ConfirmPhase` en runtime: la del Nivel 1 la trae **T17/T18** del Slice 1 y la del Nivel 2,
        **W05..W14**. Hasta entonces el desbloqueo real solo se puede verificar en pruebas, no
        jugando, y `NextPendingPhase` no tiene consumidor.
      - `[ ]` El «cierre y reapertura» de RNF-14 verificado **sobre el ejecutable** — está probado
        contra el guardado (`SaveStore_RF04_ConfirmarFase1DelNivel2SobreviveAlCierre`), no sobre
        el `.exe`. Cae en el Checkpoint D del Slice 1, donde ya está la casilla de RNF-14.
      **Aviso a quien haga T17**: `PlayerProfile` cambió de firma. `ConfirmPhase(LevelId, int, …)`
      ya no existe; se pasa un `PhaseId`. Es el cruce que la tabla de carriles anunciaba, y hoy se
      resuelve a favor de W02 porque T17 no había empezado a escribir el archivo.

### ✅ Checkpoint W-A — Cimientos
- [x] Compila sin errores ni warnings nuevos — **0 errores, 0 warnings** (10/09/2026; declarado
      desde `unity test` con el Editor cerrado, no desde `check_compile_errors`: el puente de
      coplay-mcp exige el Editor abierto y la CLI compila las dos suites igual)
- [x] Prueba de exclusión RNF-16 con **dos niveles reales**, corrida y **declarada** —
      `Architecture_RNF16_NingunAssemblyDeNivelReferenciaAOtroNivel` verde con `Fire` y
      `Wheel` en la tabla (10/09/2026, `unity test --mode EditMode`, **63/63**)
- [~] El menú habilita el Nivel 2 solo tras completar el Nivel 1 — la **regla** está cerrada y
      probada (`LevelUnlockPolicy_RF03_Nivel2BloqueadoHastaCompletarNivel1` en EditMode y
      `LevelSelect_RF03_CompletarElNivel1HabilitaElNivel2` en PlayMode, ambas verdes el
      10/09/2026). Falta verlo **jugando**, y eso no depende de este slice: nadie confirma una
      fase en runtime hasta T17/T18 del Slice 1
- [ ] Revisado con el usuario

---

## Fase 1 — Andamiaje generalizado (`andamiaje`)

- [x] **W03 · `HintPolicy` por fase, no por nivel** — `M` · `EM` (10/09/2026)
      RF-13, RF-10, RF-11, RNF-03, CP-06, HU-03, HU-04, CU-06..CU-08, INC-41 · depende de: W02
      **`HintPolicy` no necesitó una sola línea nueva**, y ese es el resultado: T11 ya la escribió
      por tarea y no por nivel —`ActiveStep` más `Activate`, que reinicia el contador—, así que
      «por fase» era pedirle lo que ya hacía. Lo que faltaba era el contenido y la prueba de que
      la generalización aguanta tres fases. Se añade `Assets/Game/Data/Guide/N2_Guia.asset` con
      **una tarea por fase**: `Seleccionar` (§6.1.1), `Construir` (§6.2.1) y `Programar` (§6.3.1),
      con la instrucción textual del guion y una pista que orienta sin resolver.
      Las instrucciones de `Construir` y `Programar` van **partidas en oraciones** respecto del
      guion, con las mismas palabras y en el mismo orden: la de §6.2.1 es una sola oración de 26
      palabras y RNF-01 corta en 20.
      **Deviación de nombre**: el plan pedía `WheelLevel_RNF03_UnaSolaTareaActivaPorFase`, pero
      `WheelLevel` no existe hasta W05; va como `WheelGuide_RNF03_…` sobre el asset y la política.
      **No se añadió un `StepFor(fase)`** a `GuideContent`: el mapeo tarea↔fase no es 1:1 en
      general —el Nivel 1 tiene dos tareas en una sola fase— así que quien lo resuelve es el
      nivel, en W05, y no el andamiaje.
      RED **2/4** (`HintPolicy_CP06_NingunaPistaDelNivel2NombraLaRespuesta` y
      `WheelGuide_RNF03_UnaSolaTareaActivaPorFase`, sin asset del N2; las dos de contador ya
      pasaban en verde, que es la prueba de que T11 estaba bien parametrizada) → GREEN
      **Scaffolding 21/21**.
- [x] **W04 · Las seis secuencias narrativas del Nivel 2** — `S` · `EM` + `PM` (10/09/2026)
      RF-05, RF-06, RF-10, RF-12, RNF-01, RNF-18, HU-02, CP-07, INC-28, guion §5, §6.1.1, §6.1.3,
      §6.2.1, §6.3.1, §6.4 · depende de: W03
      Los seis assets en `Assets/Game/Data/Narrative/N2_*.asset`, añadidos a la lista del
      `NarrativeSceneController` de la escena `Narrative`. **El controlador no cambió**: las seis
      recorren el mismo código que las tres del Nivel 1, sin una rama nueva.
      **El plan decía «ni una línea de código nuevo» y hubo que escribir dos.** El hallazgo es el
      que el plan anticipaba, y no estaba en `DialogueRunner` —que no se tocó— sino en
      `NarrativeVisitPolicy`: derivar «ya la vio» de «confirmó alguna fase del nivel» **falla
      justo en el cierre reflexivo**, porque se llega a él la primera vez inmediatamente después
      de confirmar la última fase, y el botón de omitir aparecería precisamente donde CP-07 y
      RF-12 lo prohíben. `NarrativeSequence` gana la marca `IsReflectiveClosing` (contenido, no
      rama) y para esa escena la señal pasa a ser que el **nivel siguiente ya esté desbloqueado**,
      que solo ocurre habiendo terminado el nivel antes (HU-14 FA-01/FA-02) y sigue derivándose de
      la lista cerrada sin ampliarla (RNF-09). Esto **le impone un orden a W16 y a T18**: primero
      el cierre reflexivo, después el desbloqueo. Al revés, el botón vuelve a aparecer.
      **Pendiente para el Slice 3**: `LevelId.River` no desbloquea ningún nivel siguiente, así que
      con esta señal el cierre del Nivel 3 nunca ofrecería omitir ni en la segunda vuelta.
      **Dos correcciones que salieron de las pruebas, no de la revisión a ojo**:
      1. Un texto ASCII con «: » en un escalar YAML sin comillas hace que Unity lea la línea como
         otra clave: `N2_Escena21_Bosque` cargaba con la primera línea **vacía**. Lo cazó
         `NarrativeScene_RF05_…SinRamas`.
      2. El parlamento de Chispa de §6.2.1 **desbordaba el cuadro de diálogo** (296 px en una caja
         de 176). Lo cazó `NarrativeScene_RNF01_LaLineaMasLargaCabeEnSuCuadroDeDialogo`, que ya
         barría todas las secuencias del proyecto. Los parlamentos largos del guion quedan
         **repartidos en cuadros sucesivos** —mismas palabras, mismo orden— que es como funciona
         el medio: ilustración fija y cuadros de texto secuenciales, no video.
      **Deviación de nombre**: `NarrativeVisitPolicy_RF06_…` en vez de `NarrativeSequence_RF06_…`
      (el sujeto es la política). Las dos pruebas que el plan pedía nuevas —RNF-01 sobre los seis
      assets y la aserción de layout— **ya existían y barren todo el proyecto**, así que cubren
      los seis sin tocarlas; sí se añadió `NarrativeSequence_RF05_ElNivel2TieneSusSeisSecuencias`.
      RED: no compilaba (`CS1503`, `AlreadySeen` esperaba `LevelId`) → GREEN **EditMode 77/77** y
      **PlayMode 34/36**, 0 fallos, las 2 omitidas son las `VisualVerification` de siempre.

### ✅ Checkpoint W-B — Andamiaje generalizado
- [x] Las seis escenas narrativas se recorren completas —
      `NarrativeScene_RF05_ResuelveLasSeisSecuenciasDelNivel2SinRamas` abre cada una, avanza hasta
      la última línea y comprueba que termina (**PlayMode 34/36**, 10/09/2026)
- [x] Ninguna pista del Nivel 2 resuelve la tarea (CP-06) —
      `HintPolicy_CP06_NingunaPistaDelNivel2NombraLaRespuesta` verde sobre `N2_Guia.asset`, con
      términos prohibidos por fase
- [x] `DialogueRunner` no necesitó cambios — **confirmado, no los necesitó**. Quien sí estaba mal
      parametrizado era `NarrativeVisitPolicy`: ver W04. Se corrigió también el comentario de
      `DialogueRunner` que afirmaba que el cierre reflexivo no necesita regla propia
- [ ] Revisado con el usuario

---

## Fase 2 — Bosque: selección por patrón (`nivel-rueda`, fase 1)

- [x] **W05 · `WheelLevelConfig`, `ForestObject`, `PatternSelection`** — `M` · `EM` (10/09/2026)
      RF-23, RF-24, RF-11, RF-17, RF-18, CT-05, RNF-18, RNF-01, CP-02, HU-08, CU-06,
      guion §6.1.2 · depende de: W04
      Los tres tipos que pedía el plan más `Assets/Game/Data/Wheel/N2_WheelLevelConfig.asset`:
      cinco troncos redondos entre nueve distractores (tres piedras, tres plantas, tres
      herramientas) y los cuatro mensajes del guion §6.1.2, uno por categoría.
      **El patrón se quedó en el código a propósito.** Qué se pide acopiar —lo redondo— es la
      mecánica del nivel, no un parámetro que se ajuste jugando: moverlo al asset no lo haría
      configurable, lo haría rompible. Al asset fue todo lo que sí es parámetro: cuántos, cuáles,
      qué dice cada categoría y **el formato del contador**, que es lo que hace verificable RNF-18
      sin leer el código —`WheelLevelConfig_RNF18_…` corre la misma lógica contra dos
      configuraciones distintas y comprueba que sigue a la que le den.
      **El contador de RF-24 lleva una cifra y no contradice CP-03**: dice cuánto falta de la
      tarea, no qué tan bien lo hizo el estudiante. Queda escrito en el `<remarks>` de
      `CounterText` para que nadie lo «limpie», y el barrido de CP-03 excluye ese formato a
      propósito y solo mira los mensajes de retroalimentación.
      **Un tercer resultado que no estaba en el plan**: un objeto ya acopiado no devuelve
      `Accepted` ni mensaje —cadena vacía—, porque no hubo intento que describir. Era la única
      forma de cumplir «no se cuenta dos veces» sin inventar una frase de rechazo para algo que
      el estudiante hizo bien.
      **Deviación de nombre**: el plan pedía `PatternSelection_RF23_CadaCategoriaDeDistractor…` y
      así quedó, pero se añadieron cinco pruebas que los criterios de aceptación exigían y la
      lista de verificación no nombraba: el doble conteo (RF-24), el fin de fase (RF-23), el
      inventario del asset (RF-24), la pregunta del acierto (RF-11) y RNF-01 sobre los mensajes.
      `WheelLevelConfig.Create` es costura de prueba: `internal` tras
      `[assembly: InternalsVisibleTo]` y sellada con `#if UNITY_INCLUDE_TESTS`.
      RED **5/11** —las cinco que leen el asset, con el asset todavía sin crear; las seis de
      lógica pura ya pasaban— → GREEN **EditMode 88/88**, 0 errores, 0 warnings.
      **PlayMode no se corrió**: W05 no toca escena ni MonoBehaviour y `Game.Levels.Wheel` no lo
      referencia nadie todavía. Se declara así, no como suite verde.
- [x] **W06 · Escena `Level2_Forest` y panel de selección** — `M` · `PM` `MCP` (10/09/2026)
      RF-22, RF-23, RF-24, RF-10, RF-13, RNF-02, RNF-03, RNF-19, CT-06, HU-08, CU-06 · depende de: W05
      `Assets/Game/Scenes/Level2_Forest.unity` (sexta escena de `EditorBuildSettings`) más
      `ForestSceneController`, adaptador delgado sin una sola regla. **Los catorce objetos no
      están puestos a mano**: se instancian del catálogo del asset, así que añadir un distractor
      es editar `N2_WheelLevelConfig` y no la escena — si estuvieran cableados uno a uno, RNF-18
      sería decorativo.
      **El segundo canal de RNF-19 son dos sprites de forma distinta**, `ui_circulo` para lo
      acopiado y `ui_alerta` para lo devuelto: se distinguen por forma y no solo por color, que es
      el criterio literal del requisito. Son provisionales y se cambian desde el Inspector.
      `ForestObject` gana `DisplayName` y `Art` —contenido, al asset—: **cambiar el arte de un
      objeto es repuntar su campo, sin tocar código ni escena**, que es lo que pedía dejar el
      archivo listo para sustituir.
      **Deviación de nombre**: el plan pedía `ForestScene_RNF02_ElMapaDeControlesSoloTieneClic`,
      pero `Assets/Settings/InputSystem_Actions.inputactions` es la plantilla de fábrica de Unity
      y no es lo que usa esta escena; afirmar RNF-02 contra ella sería falso en las dos
      direcciones. Va como `ForestScene_RNF02_LaEscenaSoloRespondeAClicSimple`, que sí es
      verificable: todo lo interactivo es `Button` y no hay ni un `IDragHandler` en la escena.
      Se añadieron tres pruebas que el plan no nombraba y los criterios sí exigían: RNF-19,
      RF-13 y la aserción de layout.
      RED: los 7 fallaron por `StandaloneInputModule` —el `EventSystem` que crea coplay-mcp trae
      el Input **legado**, prohibido por CT-06— y luego 6/7 al caer la aserción de layout →
      GREEN **PlayMode 43/43** y **EditMode 88/88**, 0 fallos, 0 omitidas.
      **Tres hallazgos de herramienta que no son del código y conviene no volver a pagar:**
      1. **`mcp__rider__run_unity_tests` no suspende `PlayFromBoot`.** El corredor entra a Play
         con `playModeStartScene` todavía en `Boot`, el Test Framework espera su `InitTestScene` y
         la corrida se cuelga entera — el mismo cuelgue de 1500 s del 07/09/2026, que la guarda
         `SuspendWhileTestsRun` solo evita por la ventana del Editor y por batchmode. Mientras no
         se arregle, hay que poner `playModeStartScene = null` antes de cada corrida por Rider.
      2. **`set_rect_transform` pierde el punto decimal**: `0.02` se guarda como `2` y `0.18`
         como `18`, mientras que los enteros pasan intactos —`1` sigue siendo `1`—, así que no es
         un factor de cien sino el separador decimal leído con otra configuración regional. El
         panel de retroalimentación quedó de 184320 px de ancho. Lo cazó la aserción de layout, no
         la vista. La escena **no usa ni una ancla fraccionaria**: todo va con anclas de
         estiramiento y desplazamientos enteros, que además es más legible.
      3. **`save_scene` guarda en `Assets/`**, no en la carpeta de la escena, dejando un duplicado.
      **Lo que no se verificó**: que el texto no desborde su cuadro. La aserción cubre que los
      tres elementos permanentes caben en pantalla y no se solapan; el desbordamiento de texto
      necesita el helper de layout que este proyecto no tiene instalado.

      **Acabado según los mockups (10/09/2026).** La escena se llevó al sistema de
      `claudeDocs/Mockups de interfaz Algoritmia (1).html`, pantalla **8 · Nivel 2 · recoger**:
      tablillas marfil `#F7EFE2` sobre `ui_panel` en 9-slice, botón de ayuda en atención `#E8A33D`
      sobre `ui_boton`, tipografía Baloo2-Bold, texto en carbón `#3A1E18`, margen de escena de
      64 px y alto de 96 px en el botón, por encima del mínimo táctil de 88. Los catorce objetos
      son tarjetas marfil con la ilustración arriba y el nombre debajo.
      **Un fallo de contraste que solo se vio mirando la pantalla**: teñir la tablilla entera con
      el color del estado dejaba el texto carbón sobre azul, ilegible. El color pasó a viajar
      **solo en el icono**; la tablilla es marfil siempre y la frase no cambia de color (RNF-20).
      Lo cubre ahora una aserción, pero lo encontró la captura, no la suite.
      **Los sprites son marcadores de posición y no coinciden con su nombre**: los archivos
      `prop_n2_planta_*` contienen piedras y `prop_n2_herramienta_*` contienen madera. Con solo
      dos ilustraciones distintas, **la fase no se puede jugar de verdad todavía** —el patrón se
      busca mirando—, pero el cableado es el definitivo: llega el arte, se sustituye el `.png` y
      no se toca nada más.
      **No verificado**: el color de fondo de escena. Se puso `m_ClearFlags: 2` y carbón en la
      cámara, pero la captura es del lienzo y no del render completo, así que el fondo de la
      captura sigue saliendo negro y no puedo afirmar que se vea el carbón.

      **Composición corregida contra el mockup (10/09/2026, segunda vuelta).** La primera versión
      puso los objetos en una rejilla de tarjetas con rótulo; el mockup no es eso.
      - **Los objetos están tirados por el suelo, sin tarjeta ni rótulo**: cada uno *es* su
        ilustración. El suelo es la mitad inferior de la pantalla menos la franja de la tablilla
        de diálogo, para que ningún objeto quede debajo de ella y sin poder pulsarse. La posición
        de cada uno es una **fracción del área jugable** guardada en `FloorPosition` dentro del
        asset, no píxeles en el código: sobrevive a cualquier resolución y sigue siendo contenido
        editable (CT-05, RNF-18).
      - **El acopio se llena conforme se recoge.** Cinco casillas circulares (`ui_circulo`) en una
        tablilla arriba: el tronco recogido deja el suelo y aparece en la casilla siguiente. Es lo
        que hace visible el avance **sin una sola cifra de desempeño** (CP-03) — el contador
        textual de RF-24 sigue ahí porque el guion §6.1.2 lo pide literalmente, y es estado de
        tarea, no puntaje.
      - **El botón de ayuda desaparece; la ayuda es una burbuja al señalar.** Pasar el ratón sobre
        un objeto asoma una burbuja con la instrucción vigente, y salir la retira. Esto **mantiene
        RF-13 en pie sin el botón que el mockup no tiene**: el mecanismo «instrucción a demanda»
        sigue existiendo, y la pista automática tras tres fallos no cambió.
        **Nota sobre RNF-02** («la interacción debe limitarse a clic y clic sostenido»): señalar
        **no es una acción del juego** —no selecciona, no cuenta como intento y no acerca la
        pista—, solo asoma una ayuda, y no añade ninguna entrada al mapa de controles, que es
        justamente lo que RNF-02 manda inspeccionar. Aun así queda anotado por si en la revisión
        se prefiere un acceso pulsable equivalente.
      Se usa `EventTrigger`, que ya trae uGUI, en vez de escribir un componente propio.
      Pruebas nuevas: `ForestScene_RF22_LosObjetosCaenEnLaMitadInferiorDeLaPantalla` y
      `ForestScene_RF24_ElAcopioSeLlenaConformeSeRecogenLosTroncos`;
      `ForestScene_RF13_…` pasa a comprobar la burbuja y `ForestScene_RNF23_…` que el objeto no
      lleva rótulo.
      **Las suites de esta segunda vuelta NO se corrieron**: el MCP de Rider está caído
      (`ConnectionRefused`) y `unity test` exige el Editor cerrado —«Multiple Unity instances
      cannot open the same project»—. Compila sin errores y la escena se verificó en Play con una
      captura, pero **eso no es que la suite pase**.
      **Los sprites de piedra traen el tablero de ajedrez pintado en el PNG** en vez de alfa: se
      ve un recuadro claro bajo cada piedra. Es del arte provisional, no del cableado.
      **Tercera vuelta (10/09/2026): el nivel no se podía jugar, y no era por el arte.** Al entrar
      al Nivel 2 la escena narrativa salía en blanco y el bosque era inalcanzable. Tres defectos
      encadenados, ninguno en `Game.Levels.Wheel`:
      1. `LevelSelectController` pedía la secuencia con la **fórmula** `N{nivel}_Apertura`, que
         solo acierta en el Nivel 1: el 2 pedía `N2_Apertura`, ninguna secuencia respondía y
         `Begin()` se iba por su aviso sin pintar una línea. Qué escena abre cada nivel pasa a ser
         **contenido de la ficha** (`openingSequenceId`), no una fórmula sobre el número.
      2. `NarrativeSceneController.Leave()` salía **siempre** a `LevelSelect` —lo provisional de
         T14—, así que ni terminando la escena 2.1 se llegaba a jugar. `NarrativeSequence` gana
         `NextPhase`: **a dónde sale la escena también es contenido**, y el controlador sigue sin
         un `if` por secuencia. `N2_Escena21_Bosque` declara la fase 1; las del Nivel 1 declaran
         0 y siguen saliendo al menú mientras `Level1_Cave` no exista.
      3. `GameState.Playing` **no tenía escena**: `GameFlowRunner` avisaba y no cargaba nada.
         Ahora hay una tabla aparte por `PhaseId` —una fase, una escena (RF-04)— con su única
         entrada de hoy, `(Rueda, 1) → Level2_Forest`.

      **La escena estaba escrita en números de 1920×1080 y montada en `ConstantPixelSize` sobre
      800×600** — la única de las seis así; las otras cinco van con `ScaleWithScreenSize` 1920×1080
      match 0.5. De ahí el aspecto «a medias»: márgenes de 64 y botones de 96 sin escalar.
      Corregido y clavado por `ForestScene_RNF03_LaInterfazEscalaConLaResolucionComoElRestoDelJuego`.

      **Los catorce sprites son ahora marcadores de posición propios, no arte ajeno mal rotulado.**
      Se generan por código —círculo (tronco), polígono anguloso (piedra), hoja apuntada (planta),
      hacha (herramienta)— con el contorno carbón de la Dirección de Arte, en los **mismos
      archivos y con los mismos GUID**: llega el arte, se sustituye el `.png` y no se toca nada.
      Se distinguen **por forma**, que es lo que hace jugable buscar «lo redondo» (RF-23, RNF-19).
      Hubo que ajustar los `.meta`: traían el rect del recorte de la lámina anterior y con una
      textura de 256 px el sprite «no se generaba porque el rect queda fuera».

      **Composición según la pantalla 8, esta vez entera**: suelo en la mitad inferior con los
      objetos tirados por él, tablilla del guía arriba a la izquierda, **acopio abajo a la
      izquierda** y **botón de ayuda abajo a la derecha, en el sitio donde el mockup pone «Esa sí
      sirve»**. La retroalimentación ya no tiene tablilla propia: comparte la del guía, que es la
      que dice lo que toca ahora —instrucción, respuesta o pista— con su icono (RNF-19).
      **Se fue la burbuja al señalar** y con ella la nota abierta sobre RNF-02: la ayuda es un
      botón, así que toda la escena vuelve a ser clic y nada más.

      **Un defecto que la vista del Editor no podía enseñar.** Repartir los objetos solo con las
      fracciones del asset **no es cierto en todas las resoluciones**: el suelo escala con la
      ventana y las tablillas miden lo mismo siempre, así que en batchmode tres objetos —entre
      ellos un tronco— quedaban debajo del acopio, del contador y de la ayuda, y **un objeto
      tapado no recibe el clic**. Lo cazó `ForestScene_RF22_NingunObjetoQuedaTapadoPorLaInterfaz`,
      no la captura. El adaptador reserva ahora esas tres tablillas y aparta lo que caiga debajo
      **en horizontal, hacia el centro** — subirlo lo sacaría de la mitad inferior (RF-22).

      **Lo que no se hizo, a propósito**: el botón de pausa que la pantalla 8 lleva arriba a la
      derecha. Es T16 del Slice 1 y ponerlo aquí sería una pausa muerta.

      Verificación: **EditMode 96/96** y **PlayMode 50/52** con `unity test` y el Editor cerrado
      (`test-results.xml`, `play-results.xml`, 10/09/2026); las 2 omitidas son las `VisualVerification`
      de siempre, que exigen Game View. 0 fallos.

- [x] **W07 · Colocación de la carga y demostración del rodado** — `M` · `PM` + `VV` `MCP` (11/09/2026)
      RF-25, RF-26, RF-04, RNF-02, RNF-21, CT-06, CP-02, HU-08, CU-06 (FA-4a) · depende de: W06
      **La lógica pura ya existía**: `CargoPlacement` y sus siete pruebas EditMode entraron con el
      commit de W06 (`faaaa13`). Lo de hoy es el cableado: `Objeto_Caja` (Image + `CargoHandle`) y
      `Button_Push` en `Level2_Forest`, `ForestSceneController` ampliado y el sprite provisional
      `prop_n2_caja_suelo.png`, generado por código como los otros catorce (§11: `#C4743E`,
      contorno carbón 8 px). Se sustituye el `.png` y no se toca nada más.
      **Pulsar y soltar, no arrastrar.** `CargoHandle` implementa solo `IPointerDownHandler` e
      `IPointerUpHandler`: el clic sostenido agarra, soltar el clic suelta, y mientras se sostiene
      la caja sigue al ratón leído por `Mouse.current` (CT-06). `EventTrigger`, que ya venía con
      uGUI, se descartó a propósito porque implementa el arrastre entero y habría hecho falsa la
      prueba RNF-02 — que ahora afirma las dos cosas: nadie implementa `IDragHandler` y lo único
      que responde al clic sostenido es la caja.
      **Los troncos alineados son la fila del acopio.** No hay una segunda fila de troncos en el
      suelo: la caja se suelta sobre la tablilla del acopio —que ya es los cinco troncos en línea—,
      se asienta en su arranque y el rodado la lleva hasta el otro extremo mientras las cinco
      casillas giran dos vueltas. Es una sola interpolación de posición y giro y **ningún gráfico
      se apaga, enciende ni cambia de color** durante el recorrido: así se cumple RNF-21 y así lo
      muestrea cada cuadro `ForestScene_RNF21_…`, que además guarda una captura a mitad del rodado
      cuando hay Game View (en batchmode sigue sin ella; no se omite la prueba entera).
      **A dónde sale el bosque es contenido**: `WheelLevelConfig.ClosingSequenceId`
      (`N2_Escena22_ElPatron`), y `WheelLevelConfig_RNF13_…` comprueba en EditMode que esa
      secuencia existe entre las narrativas del proyecto — un id sin asset detrás dejaría al
      estudiante en el bosque con la caja ya rodada y sin nada que pulsar. Como la escena 2.2 no
      declara `NextPhase` hasta que exista `Level2_Workshop` (W09), hoy sale al menú de niveles.
      **La fase se confirma con indicadores en cero**: `PerformanceIndicators` reales son de W15.
      Ojo: `ConfirmPhase` no sobreescribe una fase ya confirmada (OE1 §3.6.1 nota 4), así que los
      perfiles de desarrollo que pasen el bosque antes de W15 se quedan con ceros.
      **Deviación de nombre**: el plan ponía las pruebas de `CargoPlacement` en PlayMode; están en
      EditMode porque es C# plano (ver `CargoPlacementTests`). RF-25 se cubre con dos pruebas de
      escena en vez de una: la de «sin los cinco troncos» y la de «sigue al clic y se coloca».
      **Un fallo de la primera corrida que no era del código**: `ForestScene_RF04_…` esperaba con
      presupuesto de 900 cuadros y en batchmode, sin vsync, esos cuadros pasan antes de que el
      rodado de 2,5 s termine. El helper espera ahora por segundos.
      RED: 7 nuevas de escena fallaron por `cargo`/`pushButton` sin cablear (la escena aún no los
      tenía) y `WheelLevelConfig_RNF13_…` no compilaba (el asmdef EditMode de Wheel no referenciaba
      `Game.Scaffolding`; se añadió) → GREEN **EditMode 114/114** y **PlayMode 60/62** con
      `unity test` y el Editor cerrado (`test-results.xml`, `play-results.xml`, 11/09/2026); las 2
      omitidas son las `VisualVerification` de siempre. 0 fallos. `Datos/` quedó sin el perfil de
      prueba (`W07_Prueba` se borra en el `finally`).
      **No verificado**: la captura de RNF-21 (exige Game View; la suite corrió en batchmode) y el
      aspecto del rodado a ojo. El botón «Empujar» deshabilitado se distingue solo por atenuación:
      no es un estado de error y RNF-19 no lo cubre, pero queda anotado por si en la revisión se
      prefiere ocultarlo hasta colocar la caja.

      **W07-R · Revisión con el entorno del Nivel 2 (11/09/2026).** Llegó
      `env_n2_bosque_claro.png` (1599×899, provisional) y con él seis ajustes pedidos en revisión:
      1. **El entorno cubre la pantalla sin deformarse, a cualquier resolución del arte.**
         `IllustrationFraming` (`Game.Scaffolding`, C# plano): escala «cover» —cubrir, no
         encajar— calculada con el tamaño real del sprite y de la ventana en cada arranque, así
         que **sustituir el archivo por el definitivo basta**. El importador quedó en `Single` y
         `maxTextureSize` 8192 para que el arte grande no se reduzca al entrar. Lo usan el bosque
         (plano general, `Image_Environment`) y la escena narrativa.
      2. **La cámara narrativa se mueve por el entorno al ritmo de la narrativa, no del reloj.**
         Cada `NarrativeSequence` declara `CameraStart`/`CameraEnd` (foco en fracciones de la
         imagen + zoom ≥ 1: nunca se descubre un borde); el objetivo es la interpolación por
         `DialogueRunner.Progress` (línea en curso / última) y la ilustración se acerca a él con
         suavizado de 1,5 s. Sin avanzar, no se mueve —lo afirma
         `NarrativeScene_RF05_LaCamaraSeMuevePorElEntornoAlRitmoDeLaNarrativa`—. Las seis
         secuencias del N2 llevan el mismo entorno con encuadres distintos (contenido, se afinan
         en el Inspector); las tres del N1 siguen sin ilustración.
      3. **Suelo entre el 5 % y el 60 % de la pantalla** (`Panel_Floor`), y **perspectiva por
         altura**: escala de 1 abajo a `FarScale` (0.6) arriba, más un realce al acercar el cursor
         (`HoverScale` 1.15 dentro de `HoverRadius` 0.12). Los tres son parámetros del asset
         (CT-05). Crecer no selecciona: lo afirma la prueba (RNF-02).
      4. **Nudges**: la piedra vuelca menos (`StepSize` 0.01 → 0.004, radio 0.08) y la hoja
         describe un arco más visible (`Push` 0.35, `Lift` 1.6, `Drag` 1.8). Es tacto: se afina
         jugando en `N2_WheelLevelConfig`.
      5. **Al completar el acopio el bosque se despeja**: distractores y tablilla del acopio se
         apagan; los cinco troncos aparecen en `Panel_LogRow`, **a la derecha de la caja**, y la
         caja está **en el extremo izquierdo** desde el principio. La fila es ahora el destino
         del arrastre y del rodado (deja de serlo el acopio). El contador se queda (RF-24).
         Para dejar libre la esquina izquierda, **el acopio y su contador pasaron al centro
         inferior** — no estaba en la petición pero la caja y el acopio no cabían en la misma
         esquina; se cambia en la escena si se prefiere otro sitio.
      6. Pruebas nuevas: EditMode `IllustrationFramingTests` (6) y
         `DialogueRunner_RF05_ElProgresoVaDeCeroAUno…`; PlayMode
         `ForestScene_RF23_AlCompletarElAcopioQuedanLaCajaALaIzquierdaYLosTroncosASuDerecha`,
         `ForestScene_RF22_LosObjetosMasAltosSeVenMasPequenos`,
         `ForestScene_RF22_ElObjetoCreceAlAcercarseElCursorYVuelveAlAlejarse`,
         `ForestScene_RNF23_ElEntornoCubreLaPantallaSinDeformarse`,
         `NarrativeScene_RNF23_LaIlustracionCubreLaPantallaSinDeformarse` y la de cámara de arriba;
         `ForestScene_RF22_LosObjetosCaenEnLaMitadInferior…` pasa a
         `…EntreElCincoYElSesentaPorCientoDeLaPantalla`.
      Verificación con `unity test` y el Editor cerrado (11/09/2026): **PlayMode 66/68**, 0 fallos
      (las 2 omitidas, las `VisualVerification` de siempre); **EditMode 122/123** en la corrida
      completa — la que falló era una aserción mal planteada de la prueba nueva (la ventana de
      prueba no tenía exactamente la proporción del arte), corregida y **8/8** en la corrida
      filtrada de `IllustrationFramingTests`. La escena se editó por script con el Editor abierto
      (`N2Layout`, borrado) — **ojo**: `EditorSceneManager.OpenScene` falla si el Editor está en
      Play; hubo que detenerlo y relanzar.
      **No verificado a ojo**: el movimiento de cámara y la perspectiva en Play; los encuadres
      son una primera propuesta.

      **W07-R2 · Segunda revisión (11/09/2026).** Siete ajustes más:
      1. **La caja espera al 40 % de la altura**, pegada a la izquierda, y la fila de troncos a su
         altura. Anclada **por fracción y no por píxeles**: con el escalador a mitad ancho/alto,
         432 px no son el 40 % en una ventana 4:3 — lo cazó la prueba en batchmode (640×480).
      2. **El acopio y su contador vuelven a la esquina inferior izquierda.**
      3. **Los objetos apilados a la izquierda del acopio** eran el reparto: tres objetos bajo la
         misma tablilla salían al mismo punto, y en una ventana estrecha los empujones «hacia el
         centro» del acopio y de una piedra ya colocada se anulaban. `Apartar` prueba ahora tres
         sitios por estorbo —derecha, izquierda, encima— contra **todos** los estorbos a la vez,
         tablillas y objetos ya colocados, y toma el primero libre dentro del suelo. Lo afirma
         `ForestScene_RF25_LaCajaEsperaAlCuarentaPorCientoDeLaAlturaYNingunObjetoSeLeMonta`, que
         también exige que ningún objeto se monte sobre otro.
      4. **Transición al completar el acopio**: un cuadro después del quinto tronco los
         distractores se van, las casillas se vacían y los troncos **vuelan del acopio a la fila**
         mientras la cámara —`Panel_World`: entorno, suelo, caja y fila; las tablillas quedan
         fuera— se acerca a la caja con el pivote puesto en ella (`CompletionZoom` 1.6,
         `TransitionSeconds` 1.2, en el asset). La caja no se mueve de donde estaba. Mientras dura,
         la caja no se agarra. **«Empujar» no existe durante el acopio**: aparece al terminar la
         transición, deshabilitado hasta colocar la caja.
      5. **Hover más cartoon**: `HoverScale` 1.15 → 1.4, `HoverRadius` 0.12 → 0.15.
      6. **El cuadro de diálogo de la narrativa** cabe en el cuarto inferior: 1400×240 a 24 px del
         borde (antes 1040×400 a 64: llegaba al 43 %). Cuerpo a 28 pt en 620×150; la línea más
         larga sigue cabiendo (`NarrativeScene_RNF01_…`). Nueva
         `NarrativeScene_RNF03_ElCuadroDeDialogoNoSuperaElCuartoDeLaPantalla`.
      7. **Lo que dice el texto se ve**: `NarrativeSequence.Props` —sprite, posición en fracciones
         de la ilustración, tamaño en fracción de su alto, giro y espejo— se pintan como hijos de
         la ilustración y acompañan el paneo y el zoom sin cálculo propio. Para eso la ilustración
         conserva su tamaño nativo y se escala con `localScale`. La escena 2.1 lleva los catorce
         objetos del bosque **sacados del mismo catálogo que la fase jugable** más la caja, y abre
         con la cámara sobre el suelo; la 2.2 lleva la caja sobre los cinco troncos. Nueva
         `NarrativeScene_RF05_LaEscena21MuestraLosObjetosRepartidosPorElSuelo`.
      Verificación con `unity test` y el Editor cerrado (11/09/2026): **EditMode 123/123**;
      **PlayMode 69/71**, 0 fallos, las 2 omitidas de siempre. Hubo dos vueltas: la primera cayó
      por la caja anclada en píxeles (punto 1) y la segunda por el apilamiento (punto 3), que
      se diagnosticó con una traza temporal del reparto, ya retirada. La escena se editó una vez
      con el Editor abierto (coplay) y otra en batchmode con
      `Unity.exe -batchmode -executeMethod`, que funciona con el Editor cerrado.
      **No verificado a ojo**: la transición y los objetos pintados en la 2.1; posiciones y
      tamaños de esos objetos son una primera propuesta en el asset.

      **W07-R3 · Tercera revisión (11/09/2026).**
      1. **Al pasar el último tronco la caja cae al suelo por la derecha** (`FallSeconds` 0.45 en
         el asset): baja acelerando hasta la base de la fila, un poco ladeada. El rodado ya no
         termina con la caja flotando sobre el borde de la fila. `IsRolling` cubre también la
         caída, así que la fase se confirma cuando la caja ya está en el piso.
      2. **La narrativa anima lo que cuenta cada línea.** `NarrativeProp` gana `Motion`
         (`Roll` · `LiftAndRoll` · `LiftAndStay`), `MotionLine`, `MotionDistance` y
         `MotionSeconds`; `DialogueRunner.Index` dispara los movimientos al pintar la línea. En la
         2.2: línea 0, la caja **rueda sola** sobre cinco troncos que giran en el sitio —es
         narrativa, no hay botón—; línea 1, «¡Este tronco rueda!»: un tronco sube (el niño lo
         levanta; no hay sprite del niño todavía), cae y rueda; línea 2, «Pero esa piedra no…»: la
         piedra sube, cae y se queda con un bamboleo. Es RF-23 contado con movimiento. Todo es
         interpolación de posición y giro: nada cambia de color (RNF-21).
      3. Pruebas nuevas: `ForestScene_RF26_AlPasarElUltimoTroncoLaCajaCaeAlSueloPorLaDerecha` y
         `NarrativeScene_RF23_LaEscena22AnimaLoQueCadaLineaCuenta` (mide subir, rodar y quedarse
         línea a línea, esperando por segundos).
      Verificación con `unity test` y el Editor cerrado (11/09/2026): **EditMode 123/123**;
      **PlayMode 69/73** en la corrida completa con las 2 nuevas fallando por sus propias
      aserciones —la envolvente de la caja ladeada es más ancha, y `First(Roll)` cogía un tronco y
      no la caja—, corregidas y **2/2** en la corrida filtrada; las otras 69 ya estaban verdes y no
      se tocó código después. Las 2 omitidas, las `VisualVerification` de siempre.
      **No verificado a ojo**: la caída de la caja y las tres animaciones de la 2.2; posiciones,
      distancias y duraciones son una primera propuesta en `N2_Escena22_ElPatron.asset`.

      **W07-R4 · Cuarta revisión (11/09/2026).**
      1. **Un solo rodado para el bosque y la narrativa**: `RollMotion` (`Game.Scaffolding`, C#
         plano) devuelve avance suavizado, giro, caída acelerada y ladeo por instante; cada
         escena lo traduce a su espacio. La caja **gira sobre sí misma** mientras avanza (1,5
         vueltas, como en la narrativa) y al pasar el último tronco cae ladeada. Lo que se veía
         antes en Play era código sin recompilar (el Editor estaba en Play). `RowPoint` mide el
         ancho de la caja **sin girar**: con la envolvente girando, el avance temblaba y la prueba
         RNF-21 lo cazó (una regresión de 0,002 px; la aserción admite ahora una centésima).
      2. **Continuidad del bosque a la 2.2**: la escena abre con la vista final del bosque
         —zoom `CompletionZoom` con el pivote en la caja, foco calculado de la geometría de
         `Level2_Forest` a 1920×1080 (0.34, 0.46)—, los cinco troncos donde estaban (fila de
         112 px con 12 de hueco desde x=240 al 40 % de alto, girados 180° como acaban el rodado)
         y la caja al arranque de la fila, que rueda y cae en la línea 0 con la misma duración
         (`RollSeconds + FallSeconds`). `NarrativeSequence.CameraKeys`: paradas por línea; con
         paradas, la cámara se queda quieta hasta la siguiente. En la 2.2: línea 1 al niño
         (izquierda, zoom 2), línea 2 paneo a papá con la piedra (derecha), línea 3 en adelante a
         la familia hablando (más a la derecha, zoom 1.5). Sin paradas, las demás secuencias
         siguen con inicio → final por progreso.
      3. Pruebas: EditMode `RollMotionTests` (2) y
         `WheelLevelConfig_RF05_LaEscenaDeCierreAbreConLaMismaVistaConLaQueTerminaElBosque`;
         PlayMode `NarrativeScene_RF05_LaEscena22NoMueveLaVistaHastaQueElNinoHablaYLuegoPaneaAPapa`
         (y a la familia), y la de animaciones comprueba ahora la caída y el ladeo de la caja.
      Verificación con `unity test` y el Editor cerrado (11/09/2026): **EditMode 126/126**;
      **PlayMode 71/74** en la corrida completa, con `ForestScene_RNF21_…` cayendo por el
      temblor del punto 1 → corregido y **28/28** en la corrida filtrada de `Game.Levels.Wheel`;
      las otras 43 no se tocaron. 2 omitidas, las `VisualVerification` de siempre.
      **No verificado a ojo**: que la vista del bosque y la de la 2.2 coincidan de verdad al
      cambiar de escena (la equivalencia es geométrica, a 1920×1080) y los tres encuadres de la
      2.2.

### ✅ Checkpoint W-C — Fase 1 completa
- [x] El bosque se juega entero: seleccionar → acopiar cinco → colocar la caja → empujar —
      `ForestScene_RF04_…` lo recorre desde `Boot` con perfil real hasta salir a la narrativa
- [x] Ningún rechazo penaliza, bloquea ni muestra cifra de desempeño (CP-02, CP-03) —
      `CargoPlacement_CP02_…` ×2, `ForestScene_RF26_EmpujarSeHabilitaSoloConLaCajaColocada`
      (una vez habilitado no se deshabilita) y el barrido de dígitos de W05
- [x] El estado de error se distingue **sin depender del color** (RNF-19) — la caja habla por la
      misma tablilla del guía con icono: `ForestScene_RNF19_…`
- [x] Cierre forzado tras confirmar la fase 1 → retoma en la fase 2 (RNF-14) — la fase 1 queda
      en disco (`ForestScene_RF04_…` la relee con `Session.Load`) y desde W09 (12/09/2026) entrar
      al nivel con la fase 1 confirmada abre el taller:
      `WorkshopScene_RNF14_ConLaFase1EnDiscoEntrarAlNivelRetomaEnElTaller`, con la regla en
      `GameFlow` (`GameFlow_RNF14_…`).
- [ ] Revisado con el usuario

---

## Fase 3 — Taller: ensamblaje secuencial (`nivel-rueda`, fase 2)

- [x] **W08 · `AssemblySequence`, la máquina de ensamblaje** — `M` · `EM` (12/09/2026)
      RF-28, RF-29, RF-11, RF-17, RNF-18, CP-02, CP-06, CP-03, HU-09, CU-07, guion §6.2.2 · depende de: W07
      `WorkshopPiece` (las seis piezas), `AssemblyStep` (el último paso hecho; el orden del enum
      **es** el del guion y solo avanza), `AssemblySequence` (C# plano: `Select` → `Machine` →
      `Place(pieza, sobreElLugarDeArmado)`) y `AssemblyContent` + `WorkshopPiecePlacement`, el
      asset `Assets/Game/Data/Wheel/N2_AssemblyContent.asset` con las seis piezas, su sitio en
      el suelo, el arte de cada estado de la carretilla y los tres rechazos **literales** del
      guion §6.2.2. Ni una coordenada en la regla: si la pieza cayó sobre el lugar de armado lo
      mide la escena. Un rechazo no toca nada de lo hecho: está escrito en el `<remarks>` como
      razón pedagógica (CP-02) para que nadie añada un «deshacer».
      EditMode: `AssemblySequence_RF28_MecanizarExigeTroncoCortoSeleccionado`,
      `_RF29_RechazaCadaPasoFueraDeSecuenciaConSuMensaje`, `_RF29_SoltarLejosDelLugarDeArmadoNoEjecutaNada`,
      `_CP02_UnPasoFueraDeOrdenNoDeshaceLoYaHecho`, `_CP06_ElMensajeDiceQueFaltaAntesNoCualEsElPasoCorrecto`,
      `_RF17_NingunMensajeContieneDigitos` — 6/6 con mensajes de prueba distintos del asset (RNF-18).
- [x] **W09 · Escena `Level2_Workshop` y cableado del ensamblaje** — `M` · `PM` `MCP` (12/09/2026)
      RF-27, RF-28, RF-29, RF-04, RF-10, RNF-02, RNF-03, RNF-19, CT-06, HU-09, CU-07,
      INC-41 · depende de: W08
      `WorkshopSceneController` (adaptador delgado) y `Level2_Workshop.unity`, construida con un
      script de editor efímero (skill `edit-scene`) y añadida a Build Settings (nueve escenas).
      Las seis piezas se instancian del asset: los troncos cortos son botones (clic → resaltado
      con **contorno y tamaño**, RNF-19), el eje, la tabla y la caja llevan el mismo
      `CargoHandle` de la caja del bosque (pulsar y soltar, nunca el arrastre de uGUI, RNF-02) y
      la herramienta solo está. «Mecanizar» abajo a la izquierda con **candado** como segundo
      indicador; la tablilla del guía arriba y «Ayuda» arriba a la derecha. El lugar de armado
      es una placa traslúcida en el centro: al formar el eje las dos ruedas dejan el suelo y
      aparece la carretilla en su estado 3, luego 4, luego 5; la animación de terminado es un
      pulso de escala (nada parpadea, RNF-21) y al acabar se confirma la fase 2, se guarda y se
      sale a `N2_Escena24_Regreso` (contenido del asset).
      **Sprites provisionales** generados con PIL en la paleta de `Direccion_de_Arte.md`:
      `env_n2_taller.png`, `prop_n2_pieza_1/3/4/5.png` (los dos troncos cortos comparten
      `pieza_1`: son gemelos a propósito, B5), `prop_n2_carretilla_e1..e5.png`; la caja reutiliza
      `prop_n2_caja_suelo.png` (B5 = B3). Se sustituyen por nombre.
      **Tres cambios fuera de `Game.Levels.Wheel` que el taller necesitaba** (cruce de carril
      anotado, todos con prueba):
      1. `NarrativeSequence.NextSequenceId` + `GameFlow` acepta `Narrative → Narrative` +
         `GameFlowRunner` recarga la escena al reentrar: la 2.2 encadena con la 2.3 y la 2.3
         declara `NextPhase = 2`. Antes la 2.2 salía al menú y el taller era inalcanzable.
         `GameFlow_RF05_DosEscenasNarrativasSeEncadenanSinPasarPorElMenu`,
         `NarrativeScene_RF05_LaEscena22EncadenaConLa23YEstaEntraAlTaller`.
      2. `GameFlow.TryStartPlaying` **retoma en la primera fase pendiente** cuando la pedida ya
         está confirmada (RNF-14): la apertura del nivel siempre pide la fase 1, y con la 1 en
         disco se entra al taller. Con el nivel completo se juega la pedida (repetir es
         legítimo) y «Reiniciar» no cambia porque su fase no está confirmada. También rechaza una
         fase que el nivel no tiene. `GameFlow_RNF14_EntrarAUnaFaseYaConfirmadaRetomaEnLaPrimeraPendiente`.
         **Corrección del 12/09/2026 (bug de Santiago: «al terminar la primera narrativa del
         nivel 2 se sale automáticamente»):** con la 1 y la 2 confirmadas la retoma saltaba a la
         3, que no tiene escena hasta W13, y el runner caía al menú. `TryStartPlaying` recibe
         ahora un predicado «¿esta fase se puede jugar?» y solo salta a una pendiente jugable;
         el runner le pasa su tabla de escenas. Con las dos confirmadas se repite la fase 1.
         `GameFlow_RNF14_SiLaFasePendienteNoEsJugableTodaviaSeJuegaLaPedida`,
         `GameFlowRunner_RNF14_ConLasDosFasesExistentesConfirmadasEntrarAlNivel2VuelveAlBosque`.
      3. `GameFlowRunner`: `(Rueda, 2) → Level2_Workshop`, y una fase **sin escena** (la 3
         mientras W13 no exista) cae al menú de niveles en vez de dejar la FSM en `Playing`
         sobre la escena anterior (RNF-13).
      PlayMode: `WorkshopScene_RF27_PresentaLasSeisPiezas`,
      `_RF28_MecanizarSeHabilitaSoloConTroncoSeleccionadoYConDobleIndicador`,
      `_RF29_UnPasoFueraDeOrdenDevuelveLaPiezaYNoDeshaceNada`, `_RF29_LaCarretillaCreceSobreLoAnteriorEnCadaPaso`,
      `_RF29_LaSecuenciaCompletaConfirmaYGuardaLaFase2` (desde `Boot` con perfil real),
      `_RNF02_ElMapaDeControlesSoloTieneClicYClicSostenido`, `_RNF03_NadaSeSaleDePantallaNiSeSolapa`
      (aserción de layout: nada fuera de pantalla, ninguna pieza sobre otra ni sobre el lugar de
      armado en reposo), `_RNF14_ConLaFase1EnDiscoEntrarAlNivelRetomaEnElTaller` — 8/8 por el
      corredor de Rider. **Cazado por la prueba:** el arrastre de prueba no puede esperar un
      cuadro entre mover y soltar, porque en el Editor hay ratón real y `Update` sigue al cursor
      de verdad. Pendiente de ver a ojo con el usuario: el pulso de terminado y el candado.
      **Corrida del 12/09/2026 (corredor de Rider, Editor abierto):** EditMode 128/128
      (`Wheel`, `Core`, `Scaffolding`, `Architecture`) · PlayMode `WorkshopSceneTests` 8/8,
      `ForestSceneTests` 28/28, `NarrativeSceneTests` 18/18, `Game.Core.PlayMode.Tests` 7/7.
      **Segunda vuelta (12/09/2026, pedido de Santiago): el taller es el bosque.** El fondo
      generado (`env_n2_taller.png`) se descartó: la fase 2 usa `env_n2_bosque_claro.png` con el
      plano fijo de `Camara_Narrativa_N2.md` §5.6 —foco (0.772, 0.470), zoom 1.32, idéntico al
      último encuadre de la 2.3— aplicado con `IllustrationFraming.Apply`: cubre sin deformar y
      el arte definitivo se sustituye por nombre. **Las piezas cuelgan de la ilustración** en
      fracciones de ella (`WorkshopPiecePlacement.Position/Size`, la convención de
      `NarrativeProp`), de oeste a este en el orden en que el guía las nombra, y la carretilla
      crece en el sitio de las ruedas (`AssemblyPosition`); el cierre es el **empuje de cámara**
      del §5.6 (1,2 s) sobre el mundo, como el del bosque. **No hay zona fija de armado**: el eje
      se suelta sobre las ruedas y la tabla y la caja sobre el conjunto (o sobre los troncos
      si no existe aún), que es lo que hace posible el intento fuera de orden literal del guion.
      Interfaz con la anatomía del Nivel 1: tablilla arriba centrada (marco borde-tablilla, fondo
      marfil, icono), botón de pista circular arriba a la izquierda, «Mecanizar» abajo centrado
      con contorno carbón, cara clara, `ButtonPressFeedback` y tablilla «Aún no» con candado.
      Sin pausa: es W17. `CompletionFraming` va a x 0.705 y no al 0.785 del documento porque la
      carretilla crece donde estaban las ruedas, no donde estaba la tabla; anotado para
      Santiago. Capturas en `%AppData%\LocalLow\DefaultCompany\My project\TestScreenshots\
      Workshop_01..03.png` (prueba `_RNF20_Captura…`, categoría VisualVerification). PlayMode
      `WorkshopSceneTests` 10/10, con `_RNF23_ElEntornoEsElDelBosque…` (cubre la pantalla con el
      encuadre de la 2.3) y la aserción de que ni tablilla ni botones tapan la carretilla.
      **Hallazgo corregido en la tercera vuelta (12/09/2026, pedido de Santiago: «test completo
      de visibilidad del nivel 2»):** en la 2.2 el cuadro de diálogo tapaba el tronco del niño
      y la piedra de papá. Dos pruebas nuevas en `NarrativeSceneTests`, un caso por escena:
      `NarrativeScene_RF05_CapturaCadaParadaDeLaEscenaDelNivel2` (VisualVerification: captura
      cada parada asentada y escribe `N2_informe_<id>.txt` con dónde queda cada objeto respecto
      a la pantalla y al cuadro) y `NarrativeScene_RNF03_NingunObjetoQueSeMueveQuedaBajoElCuadro…`
      (ningún objeto que se mueve en su línea queda bajo el cuadro, ningún visible más del 15 %,
      R1 y R2 sobre la cámara real, y el foco nunca salta más de un canvas por segundo). Para
      que pasaran: el cuadro de diálogo de `Narrative.unity` baja de 240 a 180 px (cuerpo 680×134,
      la línea más larga del N1 sigue cabiendo, `NarrativeScene_RNF01_…`); las paradas de la 2.2
      bajan el foco y abren a 1.80 (detalle en `Camara_Narrativa.md`, con el `CompletionFraming`
      del bosque alineado al nuevo inicio); la 2.3 pinta las seis piezas donde las pone el
      taller y abre sus tres primeros planos a 1.60; la 2.4 pinta carretilla y piedra; la 2.5 la
      carretilla cargada con paradas más bajas. **Corrida:** RNF03 6/6 escenas, capturas 2.2 y
      2.3 revisadas a ojo, `NarrativeSceneTests` restantes 18/18, `ForestSceneTests` 28/28.
      Sin arte aún: familia, Algoritm, fuego y montón del Puente I, y la carretilla de la 2.4
      no avanza con las líneas.
      `GameFlow_RF05_NarrativeSeParametriza…` pedía la fase 2 del **Nivel 1**, que no existe:
      pasó a pedirla al Nivel 2. No se corrió la suite PlayMode completa de `Game.UI` ni la del
      Nivel 1: no se tocaron, y el corredor de Rider expira con tres assemblies a la vez.

### ✅ Checkpoint W-D — Fase 2 completa
- [x] El taller se juega entero: perforar → perforar → eje → tabla → caja —
      `WorkshopScene_RF29_LaSecuenciaCompletaConfirmaYGuardaLaFase2` lo recorre desde `Boot` con
      perfil real hasta salir a la narrativa (12/09/2026)
- [x] Cada intento fuera de orden da el mensaje del guion **y no deshace nada** (CP-02) —
      `AssemblySequence_RF29_…`, `_CP02_…` y `WorkshopScene_RF29_UnPasoFueraDeOrden…`
- [~] Cierre forzado tras confirmar la fase 2 → retoma en la fase 3 (RNF-14) — **la mitad que
      existe está probada**: la fase 2 queda en disco (`Session.Load`) y la regla de retoma ya
      vive en `GameFlow` (`GameFlow_RNF14_…`). Retomar **en la fase 3** no se puede afirmar hasta
      que `Level2_Maze` exista (W13); hoy esa entrada cae al menú de niveles (RNF-13).
- [ ] Revisado con el usuario

---

## Fase 4 — Laberinto: editor de bloques (`nivel-rueda`, fase 3)

> **W10, W11 y W12 no dependen de W02..W09.** Son la parte de mayor riesgo del slice (INC-33) y
> conviene adelantarlas. Ver R3 y la pregunta abierta 4 del plan.

- [ ] **W10 · `MazeGrid` y `CartState` — la orientación relativa** — `M` · `EM`
      RF-30, RF-31, RF-33, RNF-13, RNF-18, CT-05, **INC-33**, supuesto 8, guion §6.3.2,
      CU-08 · depende de: W01
- [ ] **W11 · `BlockSequence` — composición y edición** — `S` · `EM`
      RF-31, RF-34, RF-18, CP-02, HU-10, CU-08 (FA-3a, FA-6a) · depende de: W10
- [ ] **W12 · `SequenceExecutor` — paso a paso y validación por retroceso** — `M` · `EM`
      RF-32, RF-33, RF-34, RF-11, RNF-13, CP-02, CP-03, CP-06, HU-10, CU-08 · depende de: W11
- [ ] **W13 · Escena `Level2_Maze` y editor de bloques** — `M` · `PM` `MCP`
      RF-30, RF-31, RF-32, RF-13, RNF-02, RNF-03, RNF-19, CT-06, **PG-04**, HU-10,
      CU-08 · depende de: W12, W09
- [ ] **W14 · Reintento sin reiniciar el nivel** — `S` · `PM` `MCP`
      RF-34, RF-18, RF-04, CP-02, HU-10, CU-08 (FA-6a) · depende de: W13

### ✅ Checkpoint W-E — Fase 3 completa
- [ ] El laberinto se resuelve componiendo → ejecutando → corrigiendo → volviendo a ejecutar
- [ ] «Avanzar» produce desplazamientos distintos según la orientación, **verificado jugando** (INC-33)
- [ ] «Ejecutar» responde a **clic simple**, no a doble clic (PG-04, RNF-02)
- [ ] Ninguna retroalimentación nombra el bloque a corregir (CP-06)
- [ ] Revisado con el usuario

---

## Fase 5 — Cierre del nivel

- [ ] **W15 · Emisión de los cuatro indicadores del Nivel 2** — `M` · `EM`
      RF-45, RF-04, RF-07, RNF-09, RNF-14, CP-03, CP-09, OE1 §3.6.1 (notas 1–5), INC-27,
      INC-29 · depende de: W14, Slice 1 T17
      ⚠️ **`Pasos utilizados` de la fase 1 no está definido en §3.6.1** — ver pregunta abierta 1
- [ ] **W17 · Pausa y reinicio sobre las tres escenas del Nivel 2** — `S` · `PM` `MCP`
      RF-07, RF-03, RF-04, CP-02, HU-17 (FA-01..FA-05), INC-25 · depende de: W14, Slice 1 T16
- [ ] **W18 · Doble indicador y contraste en los estados de error** — `S` · `VV` `MCP`
      **RNF-19** (su criterio de verificación es este nivel), RNF-20, RNF-21, CN-04 · depende de: W07, W09, W14
- [ ] **W16 · Resumen, cierre reflexivo y desbloqueo del Nivel 3** — `M` · `EM` + `PM`
      RF-45, RF-12, RF-17, RF-03, CP-03, CP-07, CP-10, HU-14, CU-08, INC-26,
      guion §6.4 · depende de: W15, W17, W18

### ✅ Checkpoint W-F — Slice 2 completo
- [ ] **Dos recorridos completos** del Nivel 2 sin incidencias (RNF-13): puente → bosque → patrón
      → taller → regreso → laberinto → cierre → menú con Nivel 3 desbloqueado
- [ ] Cierre forzado **en cada una de las tres fases** → retoma desde la última confirmada (RNF-14)
- [ ] Prueba de exclusión RNF-16 en los dos sentidos: quitar `Wheel` y quitar `Fire`
- [ ] Carga de las tres escenas < 10 s y memoria < 2 GB, **medidas** (RNF-04, RNF-05)
- [ ] Paquete acumulado < 500 MB con el arte del Slice 2 incluido (RNF-06)
- [ ] Mapa de controles de las tres escenas: solo clic y clic sostenido (RNF-02, CT-06)
- [ ] **PG-05** verificado: el paso del panel del N1 al arrastre del N2 no confunde. Anotarlo
- [ ] RF-22..RF-34 tienen cada uno al menos una prueba que los nombra (CT-10)
- [ ] Revisado con el usuario antes de abrir el Slice 3

---

## Assets visuales — `plan.md` §Assets visuales del Slice 2

- [ ] **TODO · Personajes de la familia y Chispa sobre el entorno del Nivel 2** — sin sprite
      todavía; cuando existan, van como `NarrativeProp` en estas coordenadas de la ilustración
      (`Camara_Narrativa_N2.md` §7; `y` es la **base**, los personajes miden ~0.22 de alto):
      **Puente I** familia x 0.700–0.780 · y 0.30 · **2.2** niño con el tronco 0.168, papá con la
      piedra 0.276, mamá 0.305 y niña 0.325 (y 0.30), Chispa 0.296 · y 0.60 · **2.4** carretilla
      0.672 · y 0.30 y la piedra que hay que evitar 0.870 · y 0.29 · **2.5** fuego 0.852 · y 0.30,
      carretilla cargada 0.790 · y 0.31, familia alrededor del fuego x 0.800–0.900 · y 0.30.
      Las seis piezas del taller de la 2.3 (troncos cortos 0.690 · 0.706 · y 0.28, tronco largo
      0.748 · y 0.29, herramienta 0.772 · y 0.27, tabla 0.800 · y 0.30, caja 0.828 · y 0.31)
      tampoco existen como props: la tabla y el tronco largo no tienen sprite.

Escenarios, props e interfaz **originales del proyecto**. **Los personajes no se regeneran**: se
reutilizan `A1`..`A5` del Slice 1, que son **obra derivada** de los diseños Anonaky con
autorización concedida y mención obligatoria en créditos (CT-09, RNF-23). Cada asset se registra
en `CreditsContent.asset` (Slice 1, T08).

**Cinco bloques fijos por prompt**, copiados palabra por palabra antes de la descripción:
`[1 CONTEXTO] [2 ESTILO] [3 PALETA] [4 ENTREGA] [5 PROHIBICIONES]`. Un asset generado sin los
cinco se descarta y se vuelve a pedir. La paleta y las especificaciones salen de
`claudeDocs/Direccion_de_Arte.md`.

**El Nivel 2 es un BOSQUE**, no un cañón desértico (guion §6.1.1). El acento del nivel es la
**madera clara trabajada `#C79A5E`**, exclusiva de lo interactivo: ningún árbol ni suelo del
decorado la lleva (`Direccion_de_Arte.md` §8.2).

**Chroma:** verde `#00FF00` por defecto; **magenta `#FF00FF`** donde hay verde en el propio asset
(R5). Los fondos de escena no llevan chroma.

- [ ] **B1 · Escenario del bosque** — chroma **no** — RF-22, guion §6.1.1
- [ ] **B2 · Objetos del bosque: válidos y tres distractores** — chroma **magenta** — RF-22, RF-23, RNF-19
- [ ] **B3 · Caja de alimentos, tres estados** — chroma verde — RF-25, RF-26, guion §6.1.2
- [ ] **B4 · Escenario del área de trabajo** — chroma **no** — RF-27, guion §6.2.2
- [ ] **B5 · Las seis piezas del taller** — chroma verde — RF-27, RF-28
- [ ] **B6 · La rueda y la carretilla, cinco estados de ensamblaje** — chroma verde — RF-28, RF-29, HU-09
- [ ] **B7 · Carretilla vista superior, cuatro orientaciones** — chroma **magenta** — RF-30, RF-31, **INC-33**
- [ ] **B8 · Tablero del laberinto y tres obstáculos** — chroma **no** — RF-30, RF-33, guion §6.3.2
- [ ] **B9 · Bloques Avanzar/Retroceder/Girar y botón Ejecutar** — chroma verde — RF-31, RF-32, **RNF-19**, PG-04
- [ ] **B10 · Contador de acopio e iconos de resultado** — chroma verde — RF-24, RF-11, RNF-19, RNF-20
- [ ] Postproceso: recorte del chroma, alfa, halo, **mismo `Pixels Per Unit` que el Slice 1**
- [ ] Cada asset pasa la **checklist de `Direccion_de_Arte.md` §17** y su línea «Verificación»
- [ ] Desaturar B2, B7, B8, B9 y B10 y verificar que se siguen distinguiendo (RNF-19)
- [ ] Verificar RNF-20 sobre el arte final: el bosque es claro, el texto va siempre sobre marco

---

## Bloqueantes y decisiones pendientes

- [~] **R2 · Cerrar el Slice 1** hasta su Checkpoint D. **Dejó de ser bloqueante duro para W02**
      (decisión del 10/09/2026): el freno real era la colisión con T17 sobre `PlayerProfile.cs` y
      `SaveStore.cs`, y T17 no había empezado, así que W02 escribió primero y T17 se apoya en su
      API. Lo que sigue dependiendo del Slice 1 es el **consumidor en juego** del modelo de fase
      (T17/T18) y la verificación de RNF-14 sobre el ejecutable, ambos en el Checkpoint D.
- [ ] **R1 · Instalar el servidor MCP de Unity** (`run_unity_tests`). Este slice tiene ocho
      tareas `MCP` contra las seis del Slice 1: el costo de no tenerlo crece.
- [ ] **Pregunta abierta 1 · `Pasos utilizados` de la fase 1** sin definir en OE1 §3.6.1.
      Es un entregable radicado: **no se decide desde el código**. Hasta que se resuelva, W15 lo
      emite como «no aplica» y lo documenta.
- [ ] **Pregunta abierta 3 · Trazado del laberinto.** Validar `N2_MazeLayout.asset` jugando: al
      menos una solución que **exija girar**, ninguna que se resuelva con «Avanzar» repetido.
- [ ] **Pregunta abierta 4 · ¿Se adelantan W10..W12?** Recomendación: sí, para descargar INC-33.
- [ ] **PG-02 · nombre del guía.** El documento fuente del Nivel 2 lo llamaba «Algorim»; el guion
      adopta «Chispa». Confirmar antes de generar el arte de la escena 2.1.
- [ ] **PG-05 · cambio de esquema de control** N1 → N2. Verificar en el Checkpoint W-F.
