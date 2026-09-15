# Slice 2 — La Rueda: lo construido y sus resultados

Documento de cierre del segundo incremento. Es el hermano de los `Fase-N-Resultados.md` del
Slice 1, pero de slice entero: el Slice 2 se ejecutó en seis fases seguidas en cinco días de
trabajo y partirlo en seis archivos habría repetido el mismo contexto seis veces.

**Qué es y qué no es.** Es el registro de lo que se hizo, con qué se verificó y qué quedó
abierto. El tablero vivo sigue siendo [`todo.md`](todo.md) —si este documento y el tablero se
contradicen, gana el tablero— y el plan técnico es [`plan.md`](plan.md). Ninguno de los tres
rediscute `claudeDocs/SPEC.md`.

| Campo | Dato |
|---|---|
| **Incremento** | Slice 2 — La Rueda (Nivel 2) |
| **Rama** | `feat/Slice-2`, desde `44194a3` (cierre de la Fase 3 del Slice 1, 11/09/2026) |
| **Fechas de ejecución** | 10/09/2026 – 15/09/2026 |
| **Commits en la rama** | 16 (`04ee71a` … `17ae49a`), más el trabajo de la Fase 5 pendiente de commit |
| **Volumen frente a `main`** | 287 archivos, +37 707 / −2 550 líneas |
| **Código del módulo** | `Game.Levels.Wheel`: 21 archivos, 4 376 líneas |
| **Pruebas del módulo** | 15 archivos, 4 350 líneas — casi línea por línea con el código |
| **Verificación** | EditMode **212/212** · PlayMode **148/148** (15/09/2026) |
| **Fase de la metodología Árcade** | Desarrollo — ejecución del ciclo Diseño ↔ Desarrollo ↔ Pruebas |

---

## 1. Alcance: qué cierra este slice

El Nivel 2 completo —tres fases encadenadas, cada una con su escenario— más lo que le faltaba a
la navegación y al andamiaje cuando dejaron de servir a un solo nivel.

| Módulo | Qué entró | Qué no |
|---|---|---|
| `nivel-rueda` | Bosque (selección por patrón), taller (ensamblaje secuencial), laberinto (editor de bloques con lectura relativa) y los tres escenarios | — |
| `sistema-navegacion` | `PhaseId`, desbloqueo secuencial real, guardado por fase en un nivel de tres fases, pausa y reinicio sobre las tres escenas | Informe docente y borrado de datos (Slice 4) |
| `andamiaje` | Ayuda y pista **por fase**; las seis secuencias narrativas del Nivel 2; cierre reflexivo del guion §6.4 | Ayuda contextual del Nivel 3 |
| `progreso-registro` | **Emisión** de los cuatro indicadores del Nivel 2 con su definición por fase (OE1 §3.6.1) | Agregación y presentación docente (Slice 4) |

**Requerimientos cerrados:** `RF-22`..`RF-34`, todos de prioridad Alta. Cada uno tiene al menos
una prueba que lo nombra (CT-10), verificado por barrido sobre `Assets/Tests`: RF-22 (11 pruebas),
RF-23 (10), RF-24 (6), RF-25 (6), RF-26 (7), RF-27 (1), RF-28 (4), RF-29 (5), RF-30 (2), RF-31 (6),
RF-32 (1), RF-33 (2), RF-34 (3).

**Requerimientos generalizados más allá del Nivel 1:** RF-03, RF-04, RF-05, RF-06, RF-07, RF-10,
RF-11, RF-12, RF-13 y RF-45.

**Requerimientos verificables aquí por primera vez:** `RNF-16` (exclusión con dos niveles reales)
y `RNF-19`, cuyo criterio de verificación es, literalmente, «inspección de los estados de error de
los niveles 2 y 3».

**Fuera de alcance explícito:** `nivel-rio`, `TeacherReport`, RF-46, RF-47 y el nivel avanzado
opcional del guion §10 —que introduce presión de tiempo y contradice CP-02—.

---

## 2. Seguimiento metodológico — las actas del OE3

El slice no se planeó ni se cerró desde el código: cada tramo está radicado en un acta de la serie
`OE3` (`docs/actas/OE3/`), y el tablero Kanban vive en la §6 de cada una. Esta es la traza
completa de las decisiones que gobernaron el incremento.

| Acta | Fecha | Fase Árcade | Qué aportó al Slice 2 |
|---|---|---|---|
| **D01** | 02/09/2026 | Diseño — momento 4: identificación y elaboración de assets | Vincula a la colaboradora de assets y entrega la dirección de arte. De aquí sale el arte del Nivel 2 y la convención de nombres (`prop_n2_*`, `env_n2_*`) |
| **D02** | 05/09/2026 | Desarrollo — planeación del ciclo | Fija el desarrollo **por incrementos**: los cuatro slices y el orden en que se ejecutan. Es la sesión que hace legítimo empezar el Slice 2 antes de cerrar el 1 |
| **D03** | 06/09/2026 | Desarrollo — ejecución del ciclo | Menú de niveles con desbloqueo progresivo y bloqueo señalado por icono y texto **además del color** (RNF-19). El Slice 2 hereda esa regla y la extiende a sus tres estados de error |
| **D04** | 09/09/2026 | Desarrollo — ejecución del ciclo | Tarjeta **D04-2 · «Recibir la primera fase del primer slice y arrancar la segunda»** — el arranque formal de este slice. Además, la asesoría de interfaz (hundido del botón, jerarquía del secundario, sombra plana de un solo tono) que el Nivel 2 aplica en sus tres escenas |
| **D05** | 13/09/2026 | Desarrollo — ejecución del ciclo | Puesta en escena de la narrativa de los dos primeros niveles. Cierra **D04-2** (En proceso → Finalizado) y abre **D05-1 · «Terminar el segundo slice: la fase del laberinto y la fase de cierre del nivel dos, con sus puntos de control y las pruebas declaradas»**, con fecha límite 27/09/2026 |

**Movimiento del tablero que este documento respalda.** La tarjeta **D05-1** queda cumplida en su
parte de código el 15/09/2026 —doce días antes de su fecha límite—: la fase del laberinto (W10–W14)
y la fase de cierre del nivel (W15–W18) están implementadas, con sus puntos de control y las
pruebas declaradas. Lo que queda de la tarjeta es lo que exige la build portable o al usuario,
listado en la §7.

**Decisiones del acta D05 que este slice aplica y que no salieron del código:**

1. Todo objeto que el texto nombre debe verse **entero y por encima del cuadro de diálogo** en la
   parada de cámara en la que se lo nombra. Se comprueba automáticamente, parada por parada, con
   `NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo`.
2. Cuando un objeto y un encuadre no concuerdan, **se mueve el objeto**: los encuadres son el
   diseño registrado en `claudeDocs/Camara_Narrativa_N2.md` y los objetos no formaban parte de él.
3. La revisión del Nivel 2 alcanza hasta el §6.3.1 del guion. `N2_Escena25_Cierre` (§6.4) queda
   **fuera de la comprobación automática y declarada como tal**, no ignorada en silencio: «una
   verificación que se salta lo que no cumple no verifica nada».
4. Las hojas de verificación de encuadres se generan **desde el contenido y no a mano**
   (`docs/md/verificacion_encuadres_N2/`), para poder rehacerlas cuando el contenido cambie.

**Reparto acordado en D05.** Santiago Benavides Rey toma el cierre del segundo slice; Santiago
Valdiri García toma el sonido del juego (tarjeta D05-2). Por eso `Game.Audio` sigue siendo un
`.asmdef` vacío al cierre de este slice: no es un olvido, es un compromiso de otra persona con
fecha 27/09/2026.

---

## 3. La decisión que define el slice: dos carriles a la vez

El 10/09/2026, con el Slice 1 aún en su Fase 3, se decidió abrir el Slice 2 en paralelo. La
decisión completa está en el commit `04ee71a` y en `CLAUDE.md`.

**El reparto es por assembly, no por slice** — es lo único que evita que los dos carriles se
pisen:

| Carril | Toca |
|---|---|
| Slice 1 (Fases 3 y 5) | `Game.Levels.Fire`, `Game.Core`, escenas del N1, build portable |
| Slice 2 | `Game.Levels.Wheel`, `Game.Scaffolding` |

**Qué se reevaluó de la precondición del plan.** `plan.md` declaraba que el slice no era ejecutable
antes de cerrar el Checkpoint D del Slice 1. Se revisó tarea por tarea y resultó falso para seis de
ellas: **W01, W03, W04, W10, W11 y W12** no dependen de nada del Slice 1. **W02** tampoco —el freno
real era su colisión con T17 sobre `PlayerProfile.cs`, y T17 no había empezado, así que W02
escribió primero y T17 se apoyó en su API—. Solo **W15**, **W16** y **W17** esperaban código ajeno
(`ILevelReporter` de T17 y el menú de pausa de T16), y ese código llegó el 11/09/2026.

**Cruce en sentido contrario.** W09 (12/09/2026) tuvo que tocar `Game.Core` y `Game.UI` con prueba:
`GameFlow` acepta `Narrative → Narrative` y **retoma en la primera fase pendiente que tenga
escena** cuando se pide una ya confirmada (RNF-14), `GameFlowRunner` cae al menú si una fase no
tiene escena, y `NarrativeSequence.NextSequenceId` encadena dos escenas del guion sin un `if` en el
controlador. Ese cruce está declarado en `CLAUDE.md` para el otro carril.

---

## 4. Lo construido, fase por fase

### Fase 0 — Cimientos del slice (W01, W02 · 10/09/2026)

**W01 · `Game.Levels.Wheel` y la exclusión real.** El assembly nuevo depende solo de `Game.Core`
y no referencia a `Game.Levels.Fire` ni al revés. Eso es lo que hace **ejecutable** la prueba de
RNF-16: hasta este slice solo había un nivel y la prueba no podía fallar.

**W02 · `PhaseId` y desbloqueo secuencial.** Reescribe el modelo de progreso: `ConfirmPhase(LevelId,
int, …)` deja de existir y se pasa un `PhaseId` (nivel + fase en base 1). `PhaseId.PhaseCountOf`
sabe que el Nivel 1 tiene una fase y el Nivel 2 tres, y `LevelUnlockPolicy.UnlockAfterCompleting`
deja de creerle al llamante: exige que **todas** las fases del nivel estén confirmadas. Con un nivel
de una sola fase daba igual; con tres, aceptar la palabra del llamante habría abierto el Nivel 3 a
media rueda. **El formato del JSON del perfil no cambió** — sigue escribiendo `level` y `phase` por
separado, que es la lista cerrada ya radicada (RNF-09, §3.6.1 nota 5).

> **Checkpoint W-A.** 0 errores y 0 warnings; exclusión RNF-16 verde con los dos niveles reales
> (EditMode 63/63). Queda en `[~]` ver el desbloqueo **jugando**, que no dependía de este carril.

### Fase 1 — Andamiaje generalizado (W03, W04 · 10/09/2026)

**W03 · `HintPolicy` por fase, no por nivel.** La ayuda a demanda y la pista tras tres fallos
dejan de ser del nivel y pasan a ser de la fase: el Nivel 2 tiene tres tareas distintas y una
política por nivel habría dado la pista del bosque en el laberinto.

**W04 · Las seis secuencias narrativas.** Seis `NarrativeSequence` en `Assets/Game/Data/Narrative/`,
no seis escenas ni seis ramas. **Hallazgo real:** `DialogueRunner` no necesitaba cambios —se
confirmó, no los necesitó—; quien estaba mal parametrizado era `NarrativeVisitPolicy`, que decidía
«ya vista» con una señal que el cierre reflexivo vuelve cierta justo antes de mostrarse. Se corrigió
también el comentario de `DialogueRunner` que afirmaba lo contrario.

> **Checkpoint W-B.** Las seis escenas se recorren completas; ninguna pista del Nivel 2 nombra la
> respuesta (CP-06), verificado con términos prohibidos por fase sobre `N2_Guia.asset`.

### Fase 2 — Bosque: selección por patrón (W05, W06, W07 · 10–11/09/2026)

La fase 1 del nivel: el estudiante mira un montón de objetos, **abstrae** qué tienen en común los
que sirven —son redondos y ruedan— y acopia cinco troncos, luego coloca la caja encima y empuja.

- **W05 · `PatternSelection`** — C# plano: qué es válido, qué es distractor, qué mensaje da cada
  categoría. Un rechazo no retira nada ni cierra ningún camino (CP-02, RF-18).
- **W06 · `Level2_Forest`** — la escena, con los objetos repartidos por el suelo, el acopio de cinco
  casillas y el contador permanente.
- **W07 · La caja y el rodado** — clic sostenido para llevar la caja, «Empujar» para la demostración:
  la caja **rueda** sobre sí misma mientras avanza y se ladea al caer. Al terminar, confirma la
  fase 1, guarda y sale a la escena narrativa que declara el asset.
- **W05/W06-R** — los objetos reaccionan al cursor: crecen al acercarse y se apartan al rozarlos.
  Es realce, no selección; el clic sigue siendo el único control (RNF-02).

> **Checkpoint W-C.** El bosque se juega entero; ningún rechazo penaliza, bloquea ni muestra cifra
> (CP-02, CP-03); el estado de error se distingue **sin depender del color** (RNF-19); el cierre
> forzado tras confirmar la fase 1 retoma en la 2 (RNF-14).

### Fase 3 — Taller: ensamblaje secuencial (W08, W09 · 12/09/2026, ampliada el 14/09)

La fase 2: perforar dos troncos, formar el eje, montar la tabla, colocar la caja. **El orden
importa** y un paso fuera de secuencia se describe sin deshacer nada de lo ya hecho.

- **W08 · `AssemblySequence`** — la máquina de estados del ensamblaje, C# plano.
- **W09 · `Level2_Workshop`** — la escena y el cableado; la carretilla **crece sobre lo anterior**,
  cambiando la ilustración en su sitio en vez de quitar piezas del suelo.
- **Ampliación del 14/09** — **hallazgo real que salió del grafo de conocimiento**: al revisar a mano
  las aristas `AMBIGUOUS` de graphify, la arista «Ensamblaje de la carretilla ↔ Pieza 5 · mazo de
  piedra» era correcta y señalaba que `WorkshopPiece.Tool` no participaba de nada — **el mazo estaba
  inerte**. Santiago decidió que se arrastre sobre el tronco para mecanizar. Se implementó
  **sumando** el gesto y **conservando el botón**, porque RF-28 dice «el **botón** de mecanizar» y el
  guion §6.2.2 paso 2 también: ni el RF ni el guion necesitan corrección y lo radicado se sigue
  cumpliendo al pie de la letra.

> **Checkpoint W-D.** El taller se juega entero; cada intento fuera de orden da el mensaje del guion
> y no deshace nada. Quedó en `[~]` la mitad de RNF-14 que no se podía afirmar hasta que existiera
> `Level2_Maze` — hoy ya existe y el recorrido completo la cubre.

### Fase 4 — Laberinto: editor de bloques (W10–W14 · 13/09/2026)

La fase 3 y la de mayor riesgo del slice: el estudiante **compone una secuencia de bloques** y la
ejecuta; la carretilla obedece paso a paso.

- **W10 · `MazeGrid` y `CartState`** — la orientación **relativa** (INC-33): «Avanzar» mueve según
  hacia dónde mire la carretilla, no hacia el norte. La matriz es 16 × 11 y cubre el seto entero;
  el anillo exterior son los arbustos, salvo la salida y el refugio. **El trazado es procedural**:
  `MazeGrid.Generate` siembra obstáculos en tramos de 1 a 5 en línea —piedras sueltas casi nunca
  obligan a rodear— y exige por BFS un camino con desvío mínimo; si no lo hay, siembra uno menos.
  `Solution()` es `internal` y solo para las pruebas (CP-06).
- **W11 · `BlockSequence`** — los tres bloques del mockup 10 con su parámetro: «Avanzar» y
  «Retroceder» con cuenta de 1 a 9, «Girar» con lado. Sin tope de bloques ni de ediciones (CP-02).
- **W12 · `SequenceExecutor`** — paso a paso, con validación por retroceso: un movimiento inválido se
  intenta, vuelve **y la ejecución continúa**. El resultado no expone ninguna propiedad que nombre
  el bloque a corregir (CP-06), comprobado por reflexión.
- **W13 · `Level2_Maze`** — la escena, al pie de la letra del mockup 10. Cuando los bloques se
  acumulan **se comprimen** y una flecha despliega el que se edita; el entorno cabe entero en su
  panel sin deformarse, así que cambiar el PNG basta.
- **W14 · Reintento sin reiniciar el nivel** — al no llegar, el bloque donde se detuvo el avance
  queda resaltado por contorno **y** tamaño, la carretilla vuelve a la salida y la secuencia sigue
  en pantalla. `Executions` existe para el indicador docente y **no limita nada**, con su comentario
  de «por qué no» al lado.

> **Checkpoint W-E.** «Ejecutar» responde a clic simple, no a doble clic (PG-04, RNF-02); ninguna
> retroalimentación nombra el bloque a corregir (CP-06). Dos casillas quedaron en `[~]` a la espera
> de jugarlo en el Editor: eso es de Santiago.

### Fase 5 — Cierre del nivel (W15–W18 · 15/09/2026)

**W15 · Los cuatro indicadores del Nivel 2.** `WheelIndicatorCollector`, **un solo tipo para las
tres fases**: la mecánica de registro es la misma —una acción se acepta o se rechaza, una ejecución
llega o no— y lo que cambia por fase es qué cuenta como paso. La definición operativa de OE1 §3.6.1
se implementó literalmente:

| Indicador | Fase 1 (bosque) | Fase 2 (taller) | Fase 3 (laberinto) |
|---|---|---|---|
| **Intentos** | Selecciones de un objeto no válido | Acciones rechazadas por estar fuera de secuencia | Ejecuciones que no alcanzan el refugio |
| **Errores corregidos** | Rechazo seguido de la acción correcta | Ídem | Bloques retirados o reordenados entre una ejecución fallida y la siguiente |
| **Pasos utilizados** | **Sin definición en §3.6.1**: se emite 0 | Acciones de ensamblaje ejecutadas en orden | Bloques de la secuencia que llegó |
| **Tiempo de resolución** | Reloj menos la ventana de pausa (nota 1) | Ídem | Ídem |

Los tres controladores lo crean en `Start`, lo registran en `GameFlowRunner.ActiveReporter` y
confirman la fase con `_indicators.Complete()` donde antes iba `default`. Ningún indicador llega a
la UI del estudiante, comprobado con un barrido de reflexión sobre el assembly `Game.UI` cargado en
dominio (CP-03), sin añadir ninguna referencia de compilación.

**W16 · Resumen, cierre reflexivo y desbloqueo del Nivel 3.** `LevelSummaryMessages` gana un campo
`Level` y `LevelSummaryController` pasa de un asset único a `messagesByLevel[]`, eligiendo por el
nivel que se acaba de jugar. El relato se compone con una sobrecarga nueva que **suma los intentos
y las correcciones de las tres fases** antes de elegir la variante: sumar no crea una cifra a la
vista, porque el texto sigue siendo una línea fija del asset. El cierre reflexivo del §6.4 nombra
explícitamente **abstraer** y **pensar como un algoritmo**, y no es omitible la primera vez (CP-07)
porque `NarrativeVisitPolicy` mira el desbloqueo del nivel siguiente, no la fase confirmada.

**W17 · Pausa y reinicio sobre las tres escenas.** El menú de pausa deja de vivir dentro de
`Level1_Cave` y pasa a ser el prefab `Assets/Game/Prefabs/UI/MenuPausa.prefab`, instanciado en las
**cuatro** escenas jugables. Se rehízo con la disposición del mockup 6 y con el botón **Reiniciar**
que Santiago pidió el 15/09/2026, entre «Reanudar» y «Volver al menú de niveles» y con el estilo
secundario de este último. Dos cambios de comportamiento: el tercer botón sale ahora a la
**selección de niveles** (antes, al inicio) y **`Time.timeScale = 0`** mientras la pausa está
abierta —HU-17 dice «el nivel queda detenido», y sin eso el rodado y la ejecución paso a paso
seguían corriendo—, con vuelta a 1 al reanudar, al salir y en `OnDestroy`, que es el camino de la
recarga de «Reiniciar». `PauseMenuPolicy.Restart` no cambió: repite la **fase activa** y nunca
re-bloquea ni descarta lo confirmado (INC-25).

**W18 · Doble indicador y contraste.** Los tres estados de rechazo del nivel llevan color **más**
un segundo indicador; el contraste se mide con la fórmula WCAG real contra los colores efectivamente
aplicados en la escena, no contra la paleta nominal; y ninguna animación del nivel —rodado,
ejecución paso a paso, retroceso— parpadea, muestreando cuadro a cuadro que nada se apague y que
ningún salto sea mayor de lo que el ojo sigue.

> **Checkpoint W-F.** Dos recorridos completos del nivel, automatizados desde `Boot` con perfil
> real. Lo que queda es de la build o del usuario: §7.

---

## 5. Hallazgos reales encontrados al verificar

Ninguno de estos salió de leer el código: los encontró una prueba, el grafo o una corrida.

1. **`NarrativeVisitPolicy` mal parametrizada** (W04, 10/09) — el botón de omitir habría aparecido
   en el cierre reflexivo la primerísima vez, violando CP-07. Se derivó «ya vista» del desbloqueo
   del nivel siguiente, que solo ocurre al terminarlo.
2. **El mazo del taller estaba inerte** (14/09) — lo señaló una arista `AMBIGUOUS` del grafo de
   conocimiento. `WorkshopPiece.Tool` no participaba de ninguna mecánica.
3. **La escena puente no se veía nunca** (15/09) — `N2_PuenteI` no declaraba salida, así que caía al
   menú de niveles, y el Nivel 2 abría directamente en la escena 2.1. Hoy el nivel abre con el
   puente y el puente encadena con la 2.1 por su asset. La prueba que documentaba el
   comportamiento viejo (`_RF05_LaSecuenciaSinFaseSiguienteVuelveAlMenuDeNiveles`) pasó a ser
   `_RF05_LaEscenaPuenteEncadenaConLaEscena21`.
4. **Dos fallos reales de contraste** (15/09, RNF-20) — el contador «Troncos redondos: n de 5» se
   pintaba sobre el pasto sin tablilla, y la cara de «Ejecutar» iba en naranja Algoritm (`#E2571F`),
   que con el texto carbón da **4,07:1**. El contador cuelga ahora de una tablilla marfil y
   «Ejecutar» usa el ámbar de acción primaria (`#E8A33D`), el `PrimaryButton` del mockup 10.
5. **Dos pruebas del bosque dependen de dónde esté el ratón real** (14/09, **abierto**) —
   `ForestScene_RF22_LosObjetosCaenEntreElCincoYElSesentaPorCientoDeLaPantalla` y
   `_RF22_LosObjetosMasAltosSeVenMasPequenos` fallaron en una corrida completa por 1 px y por 0,8 %,
   y pasan solas. La causa no es aleatoria: el realce por cercanía del cursor lee `Mouse.current`, y
   en el Editor ese cursor está donde lo dejó el usuario. Arreglo de una línea cuando se aborde.

---

## 6. Verificación declarada

**Cómo se corrió.** El bloqueante **R1** —no hay servidor MCP de pruebas— siguió abierto todo el
slice. Hasta el 14/09 las corridas fueron por la CLI `unity test` con el Editor cerrado. El
15/09 se usó un paliativo que funcionó: un script de editor efímero con `[InitializeOnLoad]` que
re-registra `TestRunnerApi.RegisterCallbacks` tras cada recarga de dominio y escribe el resultado en
un archivo, lanzado con `coplay-mcp` contra el **Editor abierto** y sondeado desde la terminal. Se
borró al terminar. Ni el silencio de un MCP ni un `ConnectionRefused` son que la suite pase: todas
las cifras de abajo salen de un archivo de resultados real.

| Corrida | Fecha | Resultado |
|---|---|---|
| EditMode, suite completa | 15/09/2026 | **212/212** en 0,9 s |
| PlayMode, suite completa | 15/09/2026 | **146/148** en 638 s |
| PlayMode, las dos restantes tras actualizar su expectativa | 15/09/2026 | **2/2** |
| PlayMode, pausa + accesibilidad + resumen | 15/09/2026 | 12/13 → el fallo era el contraste del contador, corregido |
| PlayMode, contraste + recorridos + narrativa | 15/09/2026 | 32/33 → el fallo era el contraste de «Ejecutar», corregido |

Las dos pruebas que la suite completa marcó en rojo no eran regresiones: eran **expectativas que el
propio slice invalidó**. `ForestScene_RNF02_LaEscenaSoloRespondeAClicSimple` enumeraba lo que atiende
el clic sostenido y ahora el prefab de pausa añade su botón —que lo atiende solo para hundir su cara—,
y `LevelSelect_RF05_CadaNivelAbreLaSecuenciaDeAperturaDeSuFicha` esperaba que el Nivel 2 abriera por
la 2.1, que es justo lo que se corrigió. Las dos se actualizaron con su comentario del porqué y
pasaron.

**Casos declarados por archivo de prueba** (lo que cada uno cubre está en la §7 del inventario):

| EditMode | Casos | PlayMode | Casos |
|---|---|---|---|
| `PatternSelectionTests` | 14 | `ForestSceneTests` | 28 |
| `ForestObjectNudgeTests` | 10 | `MazeSceneTests` | 13 |
| `MazeGridTests` | 9 | `WorkshopSceneTests` | 11 |
| `AssemblySequenceTests` | 7 | `PauseMenuWheelTests` | 3 |
| `CargoPlacementTests` | 7 | `WheelAccessibilityTests` | 3 |
| `WheelIndicatorTests` | 7 | `WheelLevelJourneyTests` | 2 |
| `SequenceExecutorTests` | 5 | | |
| `BlockSequenceTests` | 4 | | |
| `WheelLevelConfigTests` | 3 | | |

Fuera del módulo, el slice añadió o amplió `HintPolicyTests` (11), `NarrativeSequenceTests` (5),
`PhaseProgressTests` (7), `LevelSummaryComposerTests` (7), `LevelSummaryTests` (3) y
`PauseMenuTests` (4).

**Verificación visual.** Seis capturas guardadas y revisadas en
`%AppData%\LocalLow\DefaultCompany\My project\TestScreenshots\`: los tres estados de error con su
icono, el bloque detenido con su contorno, el rodado y la ejecución paso a paso.

---

## 7. Inventario de archivos — qué es cada cosa

Esta sección se construyó sobre el **grafo de conocimiento** del proyecto, refrescado el 15/09/2026
con `graphify update .` tras el trabajo de la Fase 5: **3 147 nodos, 6 744 aristas, 195 comunidades
sobre 158 archivos**. La columna «comunidad» es el nombre que el grafo le da al vecindario del
archivo —útil para saber con qué se le pregunta—, y el número de nodos es cuántos símbolos
extrajo de él.

Los cuatro hubs más conectados del proyecto entero son hoy, según `graphify god-nodes`, tres de
este slice: `MazeSceneController` (85 aristas), `ForestSceneController` (79), `GameFlowRunner` (75)
y `WorkshopSceneController` (64).

### 7.1 `Assets/Game/Scripts/Runtime/Levels/Wheel/` — el módulo del nivel

| Archivo | Nodos | Comunidad | Qué es |
|---|---|---|---|
| `ForestSceneController.cs` | 49 | ForestSceneController | Adaptador de la fase 1: reparte los objetos por el suelo, atiende el clic, lleva la caja con clic sostenido y anima el rodado. Confirma la fase y sale a la narrativa que declara el asset |
| `PatternSelection.cs` | 10 | PatternSelection | La regla de la fase 1 en C# plano: qué es válido, qué es distractor, qué mensaje da cada categoría y cuándo está completo el acopio |
| `ForestObject.cs` | 14 | ForestObject | El objeto del bosque y su categoría (tronco redondo, piedra, rama…) |
| `ForestObjectNudge.cs` | 18 | ForestObjectNudge | El realce por cercanía del cursor: crecer y apartarse. Adorno, no control |
| `CargoPlacement.cs` | 11 | CargoPlacement | Dónde puede quedar la caja y cuándo está bien colocada sobre los troncos |
| `CargoHandle.cs` | 4 | ButtonPressFeedback | El agarre por clic sostenido de la caja: avisa al soltar, no arrastra con uGUI |
| `WheelLevelConfig.cs` | 27 | WheelLevelConfig | ScriptableObject de la fase 1: cuántos troncos, cuántos distractores, mensajes por categoría, tiempos del rodado y encuadres (CT-05) |
| `WorkshopSceneController.cs` | 38 | WorkshopSceneController | Adaptador de la fase 2: selección, botón «Mecanizar», mazo arrastrable y montaje de la carretilla sobre lo ya hecho |
| `AssemblySequence.cs` | 16 | WorkshopPiece | La máquina de estados del ensamblaje: qué paso toca y qué se rechaza sin deshacer nada |
| `AssemblyStep.cs` | 8 | WorkshopPiece | Los pasos del ensamblaje, en orden |
| `WorkshopPiece.cs` | 8 | WorkshopPiece | Las seis piezas del taller |
| `WorkshopPiecePlacement.cs` | 6 | AssemblyContent | Dónde se dibuja cada pieza sobre el entorno, en fracciones de la ilustración |
| `AssemblyContent.cs` | 27 | AssemblyContent | ScriptableObject de la fase 2: piezas, ilustraciones de los cinco estados de la carretilla, mensajes del guion y encuadre |
| `MazeSceneController.cs` | 65 | MazeSceneController | Adaptador de la fase 3 y el archivo más conectado del proyecto: pinta el laberinto, edita la secuencia con bloques encajables, la comprime cuando se acumula y anima la ejecución |
| `MazeGrid.cs` | 18 | MazeGrid | La rejilla, el seto, la validación de movimiento y la generación procedural del trazado con desvío garantizado |
| `MazeLayout.cs` | 22 | MazeLayout | ScriptableObject del laberinto: esquinas del seto, número de obstáculos, semilla, arte y mensajes |
| `CartState.cs` | 17 | CartState | La carretilla como **valor**: casilla y orientación. `Ahead`/`Behind`/`Turned…` devuelven el estado siguiente sin tocar el anterior (INC-33) |
| `BlockSequence.cs` | 31 | InstructionBlock | Los tres tipos de bloque con su parámetro y la secuencia editable: insertar, retirar, mover, reemplazar |
| `SequenceExecutor.cs` | 20 | ExecutionStep | Ejecuta la secuencia paso a paso y devuelve qué pasó en cada bloque, sin nombrar nunca el que hay que corregir |
| `WheelIndicatorCollector.cs` | 9 | WheelIndicatorCollector | Los cuatro indicadores del nivel, con la definición por fase de §3.6.1. Implementa `ILevelReporter`, así que la pausa lo notifica sin que ningún assembly vea al otro |
| `AssemblyInfo.cs` | 1 | — | `InternalsVisibleTo` de las dos suites del módulo: probar sin ampliar la superficie pública |

### 7.2 Pruebas del módulo

| Archivo | Nodos | Qué cubre |
|---|---|---|
| `EditMode/…/PatternSelectionTests.cs` | 20 | La regla del acopio: válidos, distractores, mensajes, completitud |
| `EditMode/…/CargoPlacementTests.cs` | 12 | Cuándo la caja está sobre los troncos y cuándo no |
| `EditMode/…/ForestObjectNudgeTests.cs` | 19 | El realce por cercanía, con el cursor inyectado |
| `EditMode/…/AssemblySequenceTests.cs` | 12 | El orden del ensamblaje y qué pasa fuera de orden |
| `EditMode/…/MazeGridTests.cs` | 12 | Anillo exterior, validación por retroceso, determinismo de la semilla y existencia de solución |
| `EditMode/…/BlockSequenceTests.cs` | 6 | Composición y edición sin límites (CP-02) |
| `EditMode/…/SequenceExecutorTests.cs` | 9 | Paso a paso, retroceso y silencio sobre el bloque a corregir |
| `EditMode/…/WheelIndicatorTests.cs` | 10 | Las doce celdas de §3.6.1, la pausa y el barrido CP-03 |
| `EditMode/…/WheelLevelConfigTests.cs` | 5 | Que el asset de contenido esté completo y sea coherente |
| `PlayMode/…/ForestSceneTests.cs` | 39 | La escena del bosque entera, incluido el guardado por fase |
| `PlayMode/…/WorkshopSceneTests.cs` | 22 | El taller entero, incluido el paso fuera de orden y el mazo |
| `PlayMode/…/MazeSceneTests.cs` | 23 | El laberinto según el mockup 10, el reintento y la confirmación de la fase 3 |
| `PlayMode/…/PauseMenuWheelTests.cs` | 14 | La pausa sobre las tres escenas: reanudar, reiniciar la fase activa, salir sin perder nada |
| `PlayMode/…/WheelAccessibilityTests.cs` | 15 | RNF-19, RNF-20 y RNF-21 sobre las tres escenas, con capturas |
| `PlayMode/…/WheelLevelJourneyTests.cs` | 9 | Los dos recorridos completos del nivel desde `Boot` (RNF-13) |

### 7.3 Contenido — lo que se ajusta sin tocar código (CT-05, RNF-18)

| Archivo | Qué es |
|---|---|
| `Data/Wheel/N2_WheelLevelConfig.asset` | Parámetros y textos de la fase 1 |
| `Data/Wheel/N2_AssemblyContent.asset` | Piezas, estados y mensajes de la fase 2 |
| `Data/Wheel/N2_MazeLayout.asset` | Trazado, semilla y mensajes de la fase 3 |
| `Data/Wheel/N2_ResumenNivel.asset` | Los textos del resumen de fin de nivel del Nivel 2 (W16) |
| `Data/Guide/N2_Guia.asset` | Las preguntas del guía por fase: descomponen, no resuelven (CP-06) |
| `Data/Narrative/N2_PuenteI.asset` | Del fuego al alimento; aparece el problema del transporte. Abre el nivel y encadena con la 2.1 |
| `Data/Narrative/N2_Escena21_Bosque.asset` | Los objetos dispersos del claro; entra a la fase 1 |
| `Data/Narrative/N2_Escena22_ElPatron.asset` | El patrón: qué tienen en común los que ruedan. Encadena con la 2.3 |
| `Data/Narrative/N2_Escena23_Construccion.asset` | El taller; entra a la fase 2 |
| `Data/Narrative/N2_Escena24_Regreso.asset` | El regreso con la carretilla; entra a la fase 3 |
| `Data/Narrative/N2_Escena25_Cierre.asset` | El cierre reflexivo del §6.4: abstraer y pensar como un algoritmo |

### 7.4 Escenas y arte

- **`Level2_Forest.unity`**, **`Level2_Workshop.unity`** y **`Level2_Maze.unity`** — las tres fases
  jugables, en `EditorBuildSettings` (hoy diez escenas, con `Boot` de primera).
- **`Assets/Game/Prefabs/UI/MenuPausa.prefab`** — el menú de pausa compartido por las cuatro escenas
  jugables. Tocarlo cambia el Nivel 1 y el Nivel 2 a la vez.
- **29 archivos de arte del Nivel 2** bajo `Assets/Game/Art/` con la nomenclatura radicada:
  `env_n2_bosque_claro`, `entorno_n2_laberinto`, los cinco estados de la carretilla
  (`prop_n2_carretilla_e1..e5`), las piezas del taller, los troncos, las piedras, las plantas y los
  dos sprites propios del laberinto —duplicados a propósito para poder reemplazarlos por nombre—.
  El índice completo, con la tarea que produce cada pieza, está en `Assets/Game/Art/Inventario.md`.

### 7.5 Piezas compartidas que este slice tocó

| Archivo | Qué cambió y por qué |
|---|---|
| `Core/PhaseId.cs` | **Nuevo** (W02): identifica una fase y sabe cuántas tiene cada nivel |
| `Core/PlayerProfile.cs` | `ConfirmPhase(PhaseId, …)`, `NextPendingPhase`, `IsLevelComplete`. El JSON no cambió |
| `Core/LevelUnlockPolicy.cs` | Desbloquea solo con **todas** las fases confirmadas |
| `Core/GameFlow.cs` | Acepta `Narrative → Narrative` y retoma en la primera fase pendiente **que tenga escena** (RNF-14) |
| `Core/GameFlowRunner.cs` | Tabla de escenas por fase; cae al menú si una fase no tiene escena |
| `Scaffolding/HintPolicy.cs` | Por fase, no por nivel (W03) |
| `Scaffolding/NarrativeSequence.cs` | `NextSequenceId` y `NextPhase`: a dónde sale una escena es **contenido** |
| `Scaffolding/NarrativeVisitPolicy.cs` | «Ya vista» derivada del progreso, con la excepción del cierre reflexivo (CP-07) |
| `UI/PauseMenuController.cs` | Prefab compartido, `Time.timeScale = 0`, salida a la selección de niveles (W17) |
| `UI/LevelSummaryController.cs` | Un asset de mensajes por nivel; compone con todas las fases (W16) |
| `UI/LevelSummaryComposer.cs` | Sobrecarga que suma las fases de un nivel antes de elegir la variante |

### 7.6 Documentos del slice

| Archivo | Qué es |
|---|---|
| `plan.md` | El plan técnico: alcance, decisiones aplicadas, grafo de dependencias y las dieciocho tarjetas con sus criterios de aceptación |
| `todo.md` | El tablero vivo: casillas, cifras de pruebas, checkpoints y bloqueantes. **Es la fuente de verdad del estado** |
| `Slice-2-Resultados.md` | Este documento |
| `claudeDocs/Camara_Narrativa_N2.md` | El diseño de los 50 encuadres del Nivel 2, validados contra 16:9 |
| `docs/md/Camara_Narrativa.md` | El inventario de lo aplicado, con quién decidió cada movimiento |
| `docs/md/verificacion_encuadres_N2/` | Las hojas gráficas de cada parada, generadas desde el contenido (acta D05) |
| `claudeDocs/Mockups de interfaz Algoritmia.html` | Los mockups numerados; el 8, el 9 y el 10 son las tres fases de este nivel, y el 6 la pausa |

---

## 8. Lo que queda abierto

**Del Checkpoint W-F**, todo lo que exige la build portable o a una persona:

- [ ] Carga de las tres escenas < 10 s y memoria < 2 GB, **medidas** (RNF-04, RNF-05).
- [ ] Paquete acumulado < 500 MB con el arte del Slice 2 incluido (RNF-06).
- [~] Exclusión de RNF-16 **retirando el assembly a mano** en los dos sentidos. La regla está
      probada; ejecutar sin uno de los dos niveles es verificación manual.
- [ ] **PG-05**: que el paso del panel del Nivel 1 al arrastre del Nivel 2 no confunda. Es
      observación en la sesión de prueba con estudiantes, no una aserción.
- [ ] Revisión con el usuario antes de abrir el Slice 3 — pendiente también en W-A, W-B, W-C, W-D
      y W-E.

**Bloqueantes y preguntas:**

- **R1 · Servidor MCP de pruebas.** Sigue sin conectar; el paliativo de la §6 funcionó, pero
  instalarlo sigue siendo la acción de mayor retorno del proyecto.
- **Pregunta abierta 1 · `Pasos utilizados` de la fase 1.** OE1 §3.6.1 no lo define para el bosque.
  **No se decide desde el código**: W15 lo emite como 0 («no aplica») y lo deja escrito en el
  recolector y en su prueba. Falta la definición en el documento radicado.
- **Pregunta abierta 3 · Trazado del laberinto.** Validar `N2_MazeLayout.asset` jugando: al menos
  una solución que **exija girar**, ninguna que se resuelva con «Avanzar» repetido.
- **Las dos pruebas del bosque sensibles al ratón real** (§5.5).

**Inconsistencias que este slice abre o mantiene** (detalle en `claudeDocs/INCONSISTENCIAS.md`):

- **INC-49 · abierto (15/09/2026).** El menú de pausa es el del mockup 6 —Reanudar · Reiniciar ·
  Volver al menú de niveles— y HU-17 sigue diciendo «Continuar, Reiniciar nivel y Volver al menú
  principal». Decisión de Santiago: gana el mockup; se corrige HU-17. Las pruebas conservan HU-17
  en el nombre porque la historia es la misma.
- **INC-33 · cerrado.** Los bloques del laberinto son relativos a la orientación. Con lectura
  absoluta el refugio podía quedar inalcanzable.
- **INC-25 · aplicado.** «Reiniciar» reinicia la **fase activa**, nunca las confirmadas ni el
  desbloqueo.
- **PG-04 · cerrado.** «Ejecutar» responde a clic simple; el documento fuente decía doble clic.
- **RF-31 queda por precisar**, no por cambiar: los bloques llevan cuenta (1–9) y lado, y el
  requerimiento no los detalla. Candidato a hallazgo nuevo cuando se revise el documento.

---

## 9. Cómo se reproduce

```bash
# Suites completas — exigen el Editor de Unity cerrado
unity test --mode EditMode --output test-results.xml --timeout 900 --no-banner --non-interactive
unity test --mode PlayMode --output play-results.xml --timeout 900 --no-banner --non-interactive

# Solo el Nivel 2 (--filter es una expresión regular contra namespace.Clase.Método)
unity test --mode EditMode --filter "Game\.Levels\.Wheel\.Tests"
unity test --mode PlayMode  --filter "WheelLevelJourneyTests"

# El grafo de conocimiento sobre el que se hizo la §7
graphify update .                       # re-extracción de código, sin LLM
graphify god-nodes --top 15
graphify explain "MazeSceneController"
graphify path "MazeSceneController" "PlayerProfile"
```

`unity test` abre su propia instancia batchmode: con el Editor abierto falla con
`another Unity instance is running`. Para correr contra el Editor abierto, el paliativo de la §6.
