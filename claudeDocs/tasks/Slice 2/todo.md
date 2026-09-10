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

- [ ] **W07 · Colocación de la carga y demostración del rodado** — `M` · `PM` + `VV` `MCP`
      RF-25, RF-26, RF-04, RNF-02, RNF-21, CT-06, CP-02, HU-08, CU-06 (FA-4a) · depende de: W06

### ✅ Checkpoint W-C — Fase 1 completa
- [ ] El bosque se juega entero: seleccionar → acopiar cinco → colocar la caja → empujar
- [ ] Ningún rechazo penaliza, bloquea ni muestra cifra de desempeño (CP-02, CP-03)
- [ ] El estado de error se distingue **sin depender del color** (RNF-19)
- [ ] Cierre forzado tras confirmar la fase 1 → retoma en la fase 2 (RNF-14)
- [ ] Revisado con el usuario

---

## Fase 3 — Taller: ensamblaje secuencial (`nivel-rueda`, fase 2)

- [ ] **W08 · `AssemblySequence`, la máquina de ensamblaje** — `M` · `EM`
      RF-28, RF-29, RF-11, RF-17, RNF-18, CP-02, CP-06, CP-03, HU-09, CU-07, guion §6.2.2 · depende de: W07
- [ ] **W09 · Escena `Level2_Workshop` y cableado del ensamblaje** — `M` · `PM` `MCP`
      RF-27, RF-28, RF-29, RF-04, RF-10, RNF-02, RNF-03, RNF-19, CT-06, HU-09, CU-07,
      INC-41 · depende de: W08

### ✅ Checkpoint W-D — Fase 2 completa
- [ ] El taller se juega entero: perforar → perforar → eje → tabla → caja
- [ ] Cada intento fuera de orden da el mensaje del guion **y no deshace nada** (CP-02)
- [ ] Cierre forzado tras confirmar la fase 2 → retoma en la fase 3 (RNF-14)
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
