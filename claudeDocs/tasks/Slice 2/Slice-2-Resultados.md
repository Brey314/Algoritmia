# Slice 2 — La Rueda: lo construido y sus resultados

Documento de cierre del segundo incremento. Es el hermano de los `Fase-N-Resultados.md` del
Slice 1, pero de slice entero: el Slice 2 se ejecutó en seis fases seguidas en cinco días de
trabajo y partirlo en seis archivos habría repetido el mismo contexto seis veces.

**Qué es y qué no es.** Es el registro de lo que se hizo, con qué se verificó y qué quedó
abierto. El tablero vivo sigue siendo [`todo.md`](todo.md) —si este documento y el tablero se
contradicen, gana el tablero— y el plan técnico es [`plan.md`](plan.md). Ninguno de los tres
rediscute `claudeDocs/SPEC.md`.

> **Estado al 25/09/2026:** lo que sigue es la foto del cierre (`8de614f`, 15/09/2026 13:33) y se
> conserva tal cual. Lo que cambió después —W19 e INC-50 (el rodado sale de la mecánica), la cuerda
> del taller (INC-54), los troncos que ruedan como cilindros, la caja vacía de la 2.2, el
> desplazamiento del laberinto, el día contado con luz, las cifras de pruebas— y el motor de puesta
> en escena que W07 construyó sin que el cierre lo describiera están en el
> [**Anexo**](#anexo--lo-que-cambió-después-del-cierre-25092026) al final. Cada afirmación que dejó de
> ser cierta lleva al lado una nota fechada. La cifra de suite vigente está en
> `claudeDocs/tasks/Slice 4/Slice-4-Resultados.md`, «Corrida completa de la suite (25/09/2026)».
>
> **01/10/2026:** lo que cambió del 25/09 al 01/10 —los props definitivos (`44fd479`), el recorrido
> del nivel que exige ver la 2.5 y el doble clic al salir de un cierre (`d105838`), el cierre de
> INC-46 a INC-117 (`37b3cb7`, `5a22df1`) y las correcciones de la caja de la 2.2 y de la carretilla
> amarrada— está en el [Anexo B](#anexo-b--lo-que-cambió-después-del-25092026-01102026). La suite
> vigente es la del 01/10/2026, en el mismo `Slice-4-Resultados.md`.

| Campo | Dato |
|---|---|
| **Incremento** | Slice 2 — La Rueda (Nivel 2) |
| **Rama** | `feat/Slice-2`, desde `44194a3` (cierre de la Fase 3 del Slice 1, 11/09/2026) |
| **Fechas de ejecución** | 10/09/2026 – 15/09/2026 |
| **Commits en la rama** | 16 (`04ee71a` … `17ae49a`), más el trabajo de la Fase 5 pendiente de commit |
| **Volumen frente a `main`** | 287 archivos, +37 707 / −2 550 líneas |
| **Código del módulo** | `Game.Levels.Wheel`: 21 archivos, 4 376 líneas *(al 25/09/2026: 23 archivos, 5 209 líneas —entraron `WheelSounds.cs` y `ClickRelay.cs`—; ver Anexo, A.12)* |
| **Pruebas del módulo** | 15 archivos, 4 350 líneas — casi línea por línea con el código *(al 25/09/2026: los mismos 15 archivos, 5 385 líneas)* |
| **Verificación** | EditMode **212/212** · PlayMode **148/148** (15/09/2026) *(al 25/09/2026, la corrida vigente está en `Slice 4/Slice-4-Resultados.md`, «Corrida completa de la suite (25/09/2026)»)* |
| **Fase de la metodología Árcade** | Desarrollo — ejecución del ciclo Diseño ↔ Desarrollo ↔ Pruebas |

---

## 1. Alcance: qué cierra este slice

El Nivel 2 completo —tres fases encadenadas, cada una con su escenario— más lo que le faltaba a
la navegación y al andamiaje cuando dejaron de servir a un solo nivel.

| Módulo | Qué entró | Qué no |
|---|---|---|
| `nivel-rueda` | Bosque (selección por patrón), taller (ensamblaje secuencial), laberinto (editor de bloques con lectura relativa) y los tres escenarios | — |
| `sistema-navegacion` | `PhaseId`, desbloqueo secuencial real, guardado por fase en un nivel de tres fases, pausa y reinicio sobre las tres escenas | Informe docente y borrado de datos (Slice 4) |
| `andamiaje` | Ayuda y pista **por fase**; las seis secuencias narrativas del Nivel 2 *(vencido el 25/09/2026: son siete desde `d7ace67`, con `N2_PuenteI_Bosque`; Anexo, A.3)*; cierre reflexivo del guion §6.4 | Ayuda contextual del Nivel 3 |
| `progreso-registro` | **Emisión** de los cuatro indicadores del Nivel 2 con su definición por fase (OE1 §3.6.1) | Agregación y presentación docente (Slice 4) |

**Requerimientos cerrados:** `RF-22`..`RF-34`, todos de prioridad Alta. Cada uno tiene al menos
una prueba que lo nombra (CT-10), verificado por barrido sobre `Assets/Tests`: RF-22 (11 pruebas),
RF-23 (10), RF-24 (6), RF-25 (6), RF-26 (7), RF-27 (1), RF-28 (4), RF-29 (5), RF-30 (2), RF-31 (6),
RF-32 (1), RF-33 (2), RF-34 (3). *(Al 25/09/2026, con el mismo barrido: RF-22 (14), RF-23 (10),
RF-24 (7), RF-25 (6), RF-26 (12), RF-27 (1), RF-28 (4), RF-29 (6), RF-30 (4), RF-31 (7), RF-32 (2),
RF-33 (2), RF-34 (4); ver Anexo, A.12.)*

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
   verificación que se salta lo que no cumple no verifica nada». *(Vencido el 25/09/2026: la 2.5
   entró en la lista `Verificadas` de `NarrativeSequenceTests` con `03675b9`, 23/09, al pasar al
   refugio; hoy están las siete secuencias del Nivel 2.)*
4. Las hojas de verificación de encuadres se generan **desde el contenido y no a mano**
   (`docs/md/verificacion_encuadres_N2/`), para poder rehacerlas cuando el contenido cambie.
   *(Vencido el 25/09/2026: esa carpeta se perdió el 20/09/2026 —estaba en `.gitignore`— y no se
   ha regenerado; ver Anexo, A.13.)*

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
y no referencia a `Game.Levels.Fire` ni al revés. *(Corregido el 25/09/2026: `e7278a2` ya lo creó
con `Game.Core` **y** `Game.Scaffolding`; lo que nunca referencia es otro nivel. Ver Anexo, A.11.)* Eso es lo que hace **ejecutable** la prueba de
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
también el comentario de `DialogueRunner` que afirmaba lo contrario. *(Nota del 25/09/2026: son
siete desde `d7ace67` —`N2_PuenteI_Bosque`, A.3—; `DialogueRunner` sí cambió después, en W07, que
le sumó `Progress` e `Index` —A.2—; y la regla de «ya vista» fuera del cierre reflexivo cambió con
`d81cfc7` —A.9—.)*

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
  fase 1, guarda y sale a la escena narrativa que declara el asset. *(Vencido el 25/09/2026: desde
  W19 —`863ef05`, 15/09 16:20, INC-50— «Empujar» no anima nada en el bosque: confirma la fase,
  guarda y sale a `N2_Escena22_ElPatron`, que cuenta el rodado con `RollMotion`. Ver Anexo, A.1; el
  motor de escena que W07 construyó para eso, en A.2.)*
- **W05/W06-R** — los objetos reaccionan al cursor: crecen al acercarse y se apartan al rozarlos.
  Es realce, no selección; el clic sigue siendo el único control (RNF-02).

> **Checkpoint W-C.** El bosque se juega entero; ningún rechazo penaliza, bloquea ni muestra cifra
> (CP-02, CP-03); el estado de error se distingue **sin depender del color** (RNF-19); el cierre
> forzado tras confirmar la fase 1 retoma en la 2 (RNF-14).

### Fase 3 — Taller: ensamblaje secuencial (W08, W09 · 12/09/2026, ampliada el 14/09)

La fase 2: perforar dos troncos, formar el eje, montar la tabla, colocar la caja. **El orden
importa** y un paso fuera de secuencia se describe sin deshacer nada de lo ya hecho. *(Vencido el
25/09/2026: desde `1d5ce58` hay un último paso, amarrar la caja con la cuerda —séptima pieza,
INC-54—; ver Anexo, A.5.)*

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
  panel sin deformarse, así que cambiar el PNG basta. *(Nota del 25/09/2026: desde `d81cfc7`, a
  partir de unos ocho bloques las filas conservan su alto y la lista se desplaza con dos botones
  ▲/▼; `d7ace67` sumó la barra, la cuadrícula y la papelera. Ver Anexo, A.6.)*
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
**cuatro** escenas jugables *(Nota del 25/09/2026: hoy cinco, con `Level3_River`; y desde `d81cfc7` con el estilo visual
del mockup 6 —Anexo, A.10—)*. Se rehízo con la disposición del mockup 6 y con el botón **Reiniciar**
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
ningún salto sea mayor de lo que el ojo sigue. *(Vencido el 25/09/2026: con W19 el rodado dejó la
mecánica y `WheelLevel_RNF21_NingunaAnimacionDelNivel2TieneDestellos` muestrea en su lugar el
acercamiento del cierre del acopio; Anexo, A.1.)*

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
   *(Nota del 25/09/2026: cerrado el 17/09/2026 con `d81cfc7`; las dos llaman a `ElCursorLejos(forest)` antes de medir.)*

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

*(Vencido el 25/09/2026: hoy el módulo tiene 69 casos EditMode y 89 PlayMode; `ForestSceneTests`
37, `MazeSceneTests` 26, `WorkshopSceneTests` 18, `ForestObjectNudgeTests` 12 y
`AssemblySequenceTests` 8. De dónde viene cada uno, en el Anexo, A.12.)*

Fuera del módulo, el slice añadió o amplió `HintPolicyTests` (11), `NarrativeSequenceTests` (5),
`PhaseProgressTests` (7), `LevelSummaryComposerTests` (7), `LevelSummaryTests` (3) y
`PauseMenuTests` (4). *(Al 25/09/2026, con el trabajo de otros carriles: 15, 11, 9, 13, 3 y 4.)*

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
| `ForestSceneController.cs` | 49 | ForestSceneController | Adaptador de la fase 1: reparte los objetos por el suelo, atiende el clic, lleva la caja con clic sostenido y anima el rodado. Confirma la fase y sale a la narrativa que declara el asset *(vencido el 25/09/2026: desde W19 no anima el rodado —A.1—; sí anima la transición del acopio y el tronco que el cursor aparta —A.4, A.7—)* |
| `PatternSelection.cs` | 10 | PatternSelection | La regla de la fase 1 en C# plano: qué es válido, qué es distractor, qué mensaje da cada categoría y cuándo está completo el acopio |
| `ForestObject.cs` | 14 | ForestObject | El objeto del bosque y su categoría (tronco redondo, piedra, rama…) *(corregido el 25/09/2026: las categorías son `RoundLog`, `Stone`, `Plant` y `Tool` —nunca hubo rama— y cada objeto lleva `RotationDegrees` y `Mirrored`; A.11)* |
| `ForestObjectNudge.cs` | 18 | ForestObjectNudge | El realce por cercanía del cursor: crecer y apartarse. Adorno, no control |
| `CargoPlacement.cs` | 11 | CargoPlacement | Dónde puede quedar la caja y cuándo está bien colocada sobre los troncos |
| `CargoHandle.cs` | 4 | ButtonPressFeedback | El agarre por clic sostenido de la caja: avisa al soltar, no arrastra con uGUI *(corregido el 25/09/2026: lo usan también las piezas del taller y los bloques del laberinto; A.11)* |
| `WheelLevelConfig.cs` | 27 | WheelLevelConfig | ScriptableObject de la fase 1: cuántos troncos, cuántos distractores, mensajes por categoría, tiempos del rodado y encuadres (CT-05) *(vencido el 25/09/2026: W19 le quitó `RollSeconds` y `FallSeconds`; después ganó `CargoPlacedPosition` y `LogLook`; A.1, A.4, A.7)* |
| `WorkshopSceneController.cs` | 38 | WorkshopSceneController | Adaptador de la fase 2: selección, botón «Mecanizar», mazo arrastrable y montaje de la carretilla sobre lo ya hecho |
| `AssemblySequence.cs` | 16 | WorkshopPiece | La máquina de estados del ensamblaje: qué paso toca y qué se rechaza sin deshacer nada |
| `AssemblyStep.cs` | 8 | WorkshopPiece | Los pasos del ensamblaje, en orden |
| `WorkshopPiece.cs` | 8 | WorkshopPiece | Las seis piezas del taller *(vencido el 25/09/2026: siete desde `1d5ce58`, con `Rope`; A.5)* |
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
| `Data/Narrative/N2_PuenteI.asset` | Del fuego al alimento; aparece el problema del transporte. Abre el nivel y encadena con la 2.1 *(vencido el 25/09/2026: desde `d7ace67` encadena con `N2_PuenteI_Bosque`, que encadena con la 2.1; A.3)* |
| `Data/Narrative/N2_Escena21_Bosque.asset` | Los objetos dispersos del claro; entra a la fase 1 |
| `Data/Narrative/N2_Escena22_ElPatron.asset` | El patrón: qué tienen en común los que ruedan. Encadena con la 2.3 |
| `Data/Narrative/N2_Escena23_Construccion.asset` | El taller; entra a la fase 2 |
| `Data/Narrative/N2_Escena24_Regreso.asset` | El regreso con la carretilla; entra a la fase 3 |
| `Data/Narrative/N2_Escena25_Cierre.asset` | El cierre reflexivo del §6.4: abstraer y pensar como un algoritmo |

*(Nota del 25/09/2026: después del cierre entraron `Data/Narrative/N2_PuenteI_Bosque.asset`,
`Data/Wheel/N2_Sonidos.asset` y `Data/Wheel/N2_TroncoRodante.asset`; ver Anexo, A.3 y A.7.)*

### 7.4 Escenas y arte

- **`Level2_Forest.unity`**, **`Level2_Workshop.unity`** y **`Level2_Maze.unity`** — las tres fases
  jugables, en `EditorBuildSettings` (hoy diez escenas, con `Boot` de primera). *(Al 25/09/2026 son
  doce.)*
- **`Assets/Game/Prefabs/UI/MenuPausa.prefab`** — el menú de pausa compartido por las cuatro escenas
  jugables. Tocarlo cambia el Nivel 1 y el Nivel 2 a la vez. *(Al 25/09/2026 son cinco escenas y
  tres niveles: también `Level3_River`.)*
- **29 archivos de arte del Nivel 2** bajo `Assets/Game/Art/` con la nomenclatura radicada:
  `env_n2_bosque_claro`, `entorno_n2_laberinto`, los cinco estados de la carretilla
  (`prop_n2_carretilla_e1..e5`), las piezas del taller, los troncos, las piedras, las plantas y los
  dos sprites propios del laberinto —duplicados a propósito para poder reemplazarlos por nombre—.
  El índice completo, con la tarea que produce cada pieza, está en `Assets/Game/Art/Inventario.md`.
  *(Nota del 25/09/2026: desde entonces entraron `prop_n2_tronco_textura` (`b38c00b`),
  `prop_n2_pieza_2` —la cuerda— y `prop_n2_caja_suelo_vacía` (`1d5ce58`) y
  `char_algoritm_n2_rueda_reposo` (`88fe0ee`), y `1d5ce58` borró
  `prop_n2_tronco_b..e`: los cinco troncos son un solo dibujo, `prop_n2_tronco_a`. Ver Anexo, A.4,
  A.5, A.7 y A.8.)*

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
| `Scaffolding/NarrativeVisitPolicy.cs` | «Ya vista» derivada del progreso, con la excepción del cierre reflexivo (CP-07) *(Nota del 25/09/2026: desde `d81cfc7`, fuera del cierre reflexivo «ya vista» exige la **última** fase del nivel confirmada; A.9)* |
| `UI/PauseMenuController.cs` | Prefab compartido, `Time.timeScale = 0`, salida a la selección de niveles (W17) |
| `UI/LevelSummaryController.cs` | Un asset de mensajes por nivel; compone con todas las fases (W16) |
| `UI/LevelSummaryComposer.cs` | Sobrecarga que suma las fases de un nivel antes de elegir la variante |

*(Nota del 25/09/2026: a esta tabla le faltaban desde el cierre las piezas del motor de escena que
W07 creó o amplió —`Scaffolding/IllustrationFraming.cs`, `NarrativeProp.cs`, `RollMotion.cs`, los
campos de cámara de `NarrativeSequence.cs`, `DialogueRunner.cs` y `UI/NarrativeSceneController.cs`—
y `UI/LevelSelectController.cs`, que declara la secuencia de apertura por ficha desde `faaaa13`.
Están en el Anexo, A.2 y A.11.)*

### 7.6 Documentos del slice

| Archivo | Qué es |
|---|---|
| `plan.md` | El plan técnico: alcance, decisiones aplicadas, grafo de dependencias y las dieciocho tarjetas con sus criterios de aceptación |
| `todo.md` | El tablero vivo: casillas, cifras de pruebas, checkpoints y bloqueantes. **Es la fuente de verdad del estado** |
| `Slice-2-Resultados.md` | Este documento |
| `claudeDocs/Camara_Narrativa_N2.md` | El diseño de los 50 encuadres del Nivel 2, validados contra 16:9 |
| `docs/md/Camara_Narrativa.md` | El inventario de lo aplicado, con quién decidió cada movimiento *(Nota del 25/09/2026: perdido el 20/09/2026, estaba en `.gitignore`; hoy la única fuente de lo aplicado son los `N2_*.asset`)* |
| `docs/md/verificacion_encuadres_N2/` | Las hojas gráficas de cada parada, generadas desde el contenido (acta D05) *(Nota del 25/09/2026: perdida el 20/09/2026; se puede regenerar desde el contenido)* |
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
  recolector y en su prueba. *(29/09/2026: cerrada; OE1 §3.6.1 define ya la fase 1: no se contabilizan pasos y el indicador se registra en cero.)*
- **Pregunta abierta 3 · Trazado del laberinto.** Validar `N2_MazeLayout.asset` jugando: al menos
  una solución que **exija girar**, ninguna que se resuelva con «Avanzar» repetido.
- **Las dos pruebas del bosque sensibles al ratón real** (§5.5). *(Nota del 25/09/2026: cerrado
  el 17/09/2026 con `d81cfc7`.)*

**Inconsistencias que este slice abre o mantiene** (detalle en `claudeDocs/INCONSISTENCIAS.md`):
*(Nota del 25/09/2026: después del cierre el Nivel 2 abrió dos más, las dos todavía abiertas: INC-50 —A.1— e INC-54
—A.5—.)*

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

---

## Anexo — lo que cambió después del cierre (25/09/2026)

Este documento se cerró en `8de614f` (15/09/2026, 13:33). Tres horas después entró W19, y desde
entonces el Nivel 2 siguió cambiando en otros carriles —la revisión del 16 y el 17/09, el arte y el
sonido, los personajes— sin que nadie volviera a este archivo. Además, W07 construyó el motor con el
que se ponen en escena **todas** las narrativas del juego y el cierre solo registró sus efectos. Lo
que sigue se verificó el 25/09/2026 contra el código, los assets y el historial de git en `ccf77e6`.
Las tablas archivo por archivo del carril de arte y sonido están en
`claudeDocs/tasks/Slice 3/Props-y-Sonidos-Resultados.md` y las de los personajes en
`claudeDocs/tasks/Personajes/Personajes-Resultados.md`: aquí se cuenta lo que toca al Nivel 2 y se
remite allí para el detalle.

| Commit | Fecha | Qué cambió en el Nivel 2 |
|---|---|---|
| `863ef05` | 15/09 16:20 | **W19** (INC-50): «Empujar» sale directo a la 2.2; la fila de troncos va pegada — A.1 |
| `a9236f1` | 16/09 | Auditoría de cámara: el bosque y el taller juegan a zoom 1 — A.4, A.5 |
| `d81cfc7` | 17/09 | Botones ▲/▼ del laberinto; caja y fila colgadas de la ilustración; «Omitir» exige el nivel terminado; estilo de la pausa; arreglo de las pruebas sensibles al ratón — A.4, A.6, A.9, A.10, A.12 |
| `1179dae` | 21/09 | El `EventSystem` de `Level2_Forest`, `Level2_Maze` y `Level2_Workshop` deja `DefaultInputActions` del paquete y pasa a `Assets/Game/Input/ControlesJugables.inputactions` (R16, RNF-02) |
| `03675b9` | 23/09 | Sonido del Nivel 2 (`N2_Sonidos`); la 2.5 pasa al refugio con fuego y entra en `Verificadas` — `Props-y-Sonidos-Resultados.md` |
| `d7ace67` | 23/09 | El día contado con luz; `N2_PuenteI_Bosque`; laberinto con cuadrícula, barra, papelera y `BackdropColor` — A.3, A.6 |
| `88fe0ee` | 24/09 | La familia y Algoritm animados en las tres fases y en las narrativas — `Personajes-Resultados.md` |
| `b38c00b` | 24/09 | Troncos que ruedan como cilindros (`RollingLog`) — A.7 |
| `f801186` | 24/09 | Humo en bucle (`fx_n1_humo`) detrás de la llama de `N2_PuenteI` y `N2_Escena25_Cierre` — `Props-y-Sonidos-Resultados.md` |
| `1d5ce58` | 25/09 | La cuerda del taller (INC-54); caja vacía en la 2.2; `ObstacleScale`; distractores variados — A.4, A.5, A.6, A.8 |

### A.1 W19 · «Empujar» sale directo a la narrativa (INC-50)

`863ef05`, 15/09/2026 16:20, a pedido de Santiago. Hasta entonces la caja recorría la fila en la
mecánica y la escena 2.2 repetía el mismo rodado; se quitó la copia.

- `ForestSceneController.Push` solo resuelve el empuje (`CargoPlacement.Push`, una sola vez) y
  llama a `ConfirmPhase1AndLeave`: confirma la fase 1 con sus cuatro indicadores, guarda y sale a
  `WheelLevelConfig.ClosingSequenceId` (`N2_Escena22_ElPatron`), que es la que anima el rodado. Sin
  `GameFlowRunner` —escena abierta sin pasar por `Boot`— lo avisa por consola y se queda.
- `WheelLevelConfig` perdió `RollSeconds` (2,5 s) y `FallSeconds` (0,45 s).
- La fila de troncos va **pegada**, como la dibuja la 2.2: el `GridLayoutGroup` de `Panel_LogRow`
  pasó de 12 a −12 px de hueco sobre celdas de 112 px (centros a 100/112 ≈ 0,893 del tronco), y
  desde `d81cfc7` son celdas de 75,6 px con −8,4 px: 67,2/75,6 = 0,889, exactamente la relación de
  la 2.2 (0,0175 × 3198 / 0,07 × 899). La prueba admite ±0,02, así que las dos cumplen.
- Pruebas. Salen `ForestScene_RF26_AlPasarElUltimoTroncoLaCajaCaeAlSueloPorLaDerecha`,
  `ForestScene_RF26_ElRodadoSeReproduceUnaSolaVezYNoDeshaceLaColocacion` y
  `ForestScene_RNF21_LaAnimacionDelRodadoNoTieneDestellos`. Entran
  `ForestScene_RF26_EmpujarNoAnimaElRodadoEnLaMecanicaSinoQueSaleALaNarrativa` —la caja no se mueve
  ni un píxel y el botón sigue habilitado (CP-02)— y
  `ForestScene_RF26_LosTroncosDeLaFilaVanPegadosComoEnLaEscena22`.
  `WheelLevel_RNF21_NingunaAnimacionDelNivel2TieneDestellos` muestrea ahora el acercamiento del
  cierre del acopio.
- **INC-50 sigue abierto**: RF-26, HU-08 y CU-06 describen el rodado dentro de la mecánica, y el
  guion §1.6.1.2 también. El código se
  adelantó al documento por decisión de Santiago; lo que falta es editar el documento radicado.

### A.2 El motor de puesta en escena de W07

W07 (`416ec79`, 11/09/2026) no se quedó en la caja: construyó en `Game.Scaffolding` y `Game.UI` el
mecanismo con el que se mueven las narrativas —la cámara por paradas, los objetos pintados y su
movimiento—, y el cierre solo registró sus efectos (las paradas del acta D05, §2). Es C# plano donde
se puede probar sin escena (`IllustrationFraming`, `RollMotion`) y contenido del asset donde se
ajusta mirando (CT-05, RNF-18).

**Encuadre: `CameraFraming` e `IllustrationFraming`** (`Scaffolding/IllustrationFraming.cs`).

- `CameraFraming` es un encuadre: `Focus` en fracciones de la ilustración —(0,0) abajo a la
  izquierda— y `Zoom` sobre el encuadre que cubre la pantalla justa, que nunca baja de 1.
- `IllustrationFraming.CoverSize` escala la imagen hasta **cubrir** la ventana sin deformarla, y
  `Offset` centra el foco recortándolo al margen que sobra: un foco imposible no descubre borde, se
  recorta en silencio.
- `Apply` deja la ilustración a su tamaño nativo y la escala con `localScale`, así que lo que cuelga
  de ella —los objetos de una narrativa, la caja y la fila del bosque— se coloca en fracciones y
  acompaña paneo y zoom sin cálculo propio. Por eso sustituir el PNG basta.
- `ScaleAbout` da el pivote y la escala con que agrandar el «mundo» para pasar de un encuadre a
  otro: es lo que deja el cierre del bosque exactamente en el encuadre con el que abre la 2.2. Lo
  usan también el taller (desde `b6b886c`) y el panel de ensamblaje del Nivel 3.
- `Warnings` es el aviso de ese recorte silencioso, que `NarrativeSequence.OnValidate` escribe en
  la consola del Editor: foco fuera de rango y, en el bosque espejado del Nivel 2, la línea de
  árboles fuera de cuadro (`TreeLine` 0,66) o el eje del espejo (`MirrorAxis` 0,5) cruzado por el
  cuadro o por el paneo. Desde `e3575bf` (16/09, R04) recibe la proporción del sprite: con el 16:9
  del Nivel 3 la media pantalla es 0,5/z del ancho y no 0,25/z.

**Cámara por paradas: los campos de `NarrativeSequence`**, que interpreta
`UI/NarrativeSceneController.cs`.

| Campo | Desde | Qué hace |
|---|---|---|
| `CameraStart` · `CameraEnd` | `416ec79` | Sin paradas, la cámara va del inicial al final al ritmo de `DialogueRunner.Progress` |
| `CameraKeys` (`CameraKey`: `Line`, `Framing`) | `416ec79` | Con paradas, al llegar a su línea la cámara va a su encuadre y se queda hasta la siguiente; el inicial vale hasta la primera y el final se ignora |
| `CameraSmoothingSeconds` | `416ec79` | Suavizado exponencial por escena (`1 − e^(−Δt/s)`: dos tercios del camino en `s` segundos). 1,3 s por defecto; hoy 0,7 en la 2.4 y 2,5 en la 2.5 |
| `CameraKey.HardCut` | `fa5d032` (12/09, T20–T24) | Corte seco: cámara y luz saltan al encuadre sin suavizado. El Nivel 2 no lo usa |
| `HardCutFadeSeconds` · `OpensFromBlack` | `1009c5a` (13/09, W07-R5) | El corte funde a negro, **salta en el punto negro** y vuelve (0,5 s por defecto); el negro lo pone la capa de oscuridad, que va por debajo del cuadro de diálogo, así que el texto se sigue leyendo. `OpensFromBlack` abre la escena en negro con media transición: en el Nivel 2, la 2.1 y la 2.3 |
| `LightStart` · `CameraKey.Light` (`NarrativeLight`) | `fa5d032` | La luz por parada: fuente, radio, fondo, tinte y destello. Nació para la cueva del Nivel 1 y se documenta en `claudeDocs/tasks/Slice 1/Fase-5-6-Resultados.md`; el Nivel 2 la usa para el día (A.3) y para el charco de la hoguera en `N2_PuenteI` y la 2.5 |

**`DialogueRunner.Progress` e `Index`** (`416ec79`). `Progress` va de 0 en la primera línea a 1 en
la última —es lo que lleva la cámara al ritmo de la narrativa y no del reloj— e `Index` es la línea
en curso, la que dispara paradas y movimientos. Es lo único que W07 le cambió a `DialogueRunner`,
que en W04 no había necesitado cambios (§4, Fase 1). Prueba:
`DialogueRunner_RF05_ElProgresoVaDeCeroAUnoAlRitmoDeLasLineas`.

**Objetos: `NarrativeProp`** (`Scaffolding/NarrativeProp.cs`, lista `NarrativeSequence.Props`).

- Nació en W07 con `Art`, `Position` en fracciones de la ilustración, `Size` como fracción de su
  **alto**, `RotationDegrees`, `Mirrored` y el movimiento: `Motion` (`PropMotion.None`, `Roll`,
  `LiftAndRoll`, `LiftAndStay`), `MotionLine`, `MotionDistance` (fracción del ancho),
  `MotionSeconds` y `MotionDrop` (la caída al pasar el último tronco).
- `NarrativeSceneController.PlaceProps` los cuelga de la ilustración —por eso heredan paneo y
  zoom— y `PlayMotions(línea)` lanza `MoveAsync` en los que se mueven en la línea que acaba de
  aparecer: lo que alguien levanta sube y cae en la primera mitad del tiempo, lo que rueda avanza
  según `RollMotion.Evaluate`, y todo es interpolación continua de posición y giro (RNF-21).
- Lo que se le sumó después se registra donde entró: `PropMotion.Drift` (`1179dae`, 21/09, la balsa
  del Nivel 3) en `Slice-3-Resultados.md`; `FrameAnimation` (`aca7acc`), `BurnExtent` (`8ec5120`) y
  `LandSound` (`03675b9`) en `Props-y-Sonidos-Resultados.md`; `Rolling` (`b38c00b`) en A.7; `Actor`,
  `ActorStart` y `Beats` (`88fe0ee`) en `Personajes-Resultados.md`; y `MotionAmbient` (`ccf77e6`,
  25/09: la balsa suena mientras cruza) en el carril de sonido.

**El rodado: `RollMotion` y `RollSample`** (`Scaffolding/RollMotion.cs`, `416ec79`). C# plano sin
coordenadas: `Evaluate(t, rollShare)` devuelve el avance suavizado sobre los troncos, la caída —que
acelera— al pasar el último, el giro (`Turns`: 1,5 vueltas en la fila entera) y el ladeo
(`TiltDegrees`: −18°). Nació compartido por el bosque y la 2.2; desde W19 solo lo llama
`NarrativeSceneController.MoveAsync`, y desde `1d5ce58` en la narrativa **gira el tronco** —el
objeto con `Rolling`— mientras la caja solo se desliza y se ladea al caer (A.8). Pruebas:
`RollMotion_RF26_LaCajaAvanzaSinRetrocederYLuegoCae` y
`RollMotion_RF26_SinTiempoDeCaidaLaCajaSoloRueda`.

**Pruebas del motor que el cierre no nombró.** EditMode: `IllustrationFramingTests` (hoy 12 casos en
10 métodos; el de la proporción 16:9 es de `e3575bf`) y `RollMotionTests` (2). PlayMode, en
`NarrativeSceneTests`: `NarrativeScene_RF05_LaCamaraSeMuevePorElEntornoAlRitmoDeLaNarrativa`,
`…_RF05_LaEscena21MuestraLosObjetosRepartidosPorElSuelo`,
`…_RF05_LaEscena22NoMueveLaVistaHastaQueElNinoHablaYLuegoPaneaAPapa` y
`…_RF23_LaEscena22AnimaLoQueCadaLineaCuenta` (`416ec79`);
`…_RF05_LaEscena22EncadenaConLa23YEstaEntraAlTaller`,
`…_RNF03_NingunObjetoQueSeMueveQuedaBajoElCuadroDeDialogoEnElNivel2` y
`…_RF05_CapturaCadaParadaDeLaEscenaDelNivel2` (`b6b886c`);
`…_RF05_ElCorteSecoFundeANegroSaltaAOscurasYVuelve` (`1009c5a`) y
`…_RF05_LaEscenaQueAbreEnNegroFundeAunqueSuPrimeraParadaSeaLaLinea0` (`a9236f1`).

### A.3 El día del Nivel 2, contado con luz (`d7ace67`, 23/09)

El nivel transcurre en un día y lo dice la luz, no el arte. El detalle por asset está en
`Props-y-Sonidos-Resultados.md` §4.

- En las narrativas, el `LightStart` de cada secuencia: amanecer azulado en `N2_PuenteI` (tinte
  0,68/0,78/1, fondo 0,8), tarde sin tinte en `N2_PuenteI_Bosque`, la 2.1, la 2.2 y la 2.3,
  atardecer en la 2.4 (1/0,76/0,58) y noche junto al fuego en la 2.5 (1/0,62/0,42, fondo 0,45).
- En el laberinto, que no tiene capa de luz, `MazeLayout.LightTint` (1/0,95/0,9) tiñe el entorno y
  lo que cuelga de él, salvo los personajes, cuya piel no cambia de tono.
- **Siete secuencias.** `N2_PuenteI` se partió en dos para cambiar de fondo a mitad de escena:
  queda sobre `entorno_n1_apertura` y encadena con `N2_PuenteI_Bosque` («La familia sale a
  recolectar», sobre `env_n2_bosque_claro`), que encadena con la 2.1. Encadenar es contenido
  (`NextSequenceId`), no un `if`.
- Lo vigilan `NarrativeSequence_RF05_ElNivel2TranscurreDelAmanecerALaNoche`,
  `NarrativeSequence_RF05_ElNivel2TieneSusSieteSecuencias` y `MazeScene_RF30_ElLaberintoEsAlAtardecer`.

### A.4 El bosque: lo que el cierre no contó y lo que cambió después

- **La transición del acopio** (`TransitionAsync`, W07). Un cuadro después del quinto tronco —para
  que se lo vea llegar a su casilla— se ocultan los objetos que quedan en el suelo, las casillas se
  vacían y los cinco troncos **vuelan** de ellas a la fila, a la derecha de la caja, mientras el
  «mundo» se escala con `IllustrationFraming.ScaleAbout` de `PlayFraming` a `CompletionFraming` en
  `TransitionSeconds` (1,2 s en el asset, con `SmoothStep`). Al terminar se oculta el acopio y
  aparece «Empujar»; el contador de RF-24 se queda. La cámara acaba en el encuadre con el que abre
  la 2.2 (`Camara_Narrativa_N2.md` §5.3).
- **Perspectiva y realce** (`Place`, W07). Un objeto se dibuja a escala 1 en el borde bajo del suelo
  y a `FarScale` (0,6) en el alto —el suelo sube hacia el fondo del claro—, y crece hasta
  `HoverScale` (1,4 en el asset) con el cursor a menos de `HoverRadius` (0,15 del suelo). Las dos
  escalas se multiplican; el espejo va en el signo.
- **`Despejar` y `Apartar`** (W06, `faaaa13`; los tres sitios, de W07). Al repartir, ningún objeto
  queda bajo una tablilla ni encima de otro ya colocado: se prueban tres sitios —a un lado, al otro
  y encima— y se toma el primero libre que quede dentro del suelo (RF-22). Sin esto, en una ventana más estrecha que el
  diseño un tronco tapado no recibe el clic y la fase no se puede terminar.
- **Postura** (`eb941c8`). `ForestObject.RotationDegrees` y `Mirrored` dan variedad sin más arte; el
  espejo es escala negativa en X, no un segundo sprite.
- **Distractores variados** (`1d5ce58`, decisión de Santiago). Las tres plantas y las tres
  herramientas usan cada una su dibujo (`prop_n2_planta_a..c`, `prop_n2_herramienta_a..c`); los
  cinco troncos siguen siendo un solo sprite girado (`prop_n2_tronco_a`) y las tres piedras
  comparten `prop_n2_piedra_a`. `WheelLevelConfig_RF23_CadaCategoriaDelBosqueSeDibujaConUnSoloSprite`
  pasó a ser `WheelLevelConfig_RF23_LosTroncosRedondosSeDibujanConUnSoloSprite`: la redondez es lo
  único que se repite.
- **Caja y fila colgadas de la ilustración** (`d81cfc7`, 17/09). Antes colgaban del mundo, en
  píxeles elegidos para el plano general, y el acercamiento del cierre las echaba del cuadro por la
  izquierda. Ahora sus anclas son fracciones del bosque y la caja colocada se asienta en
  `WheelLevelConfig.CargoPlacedPosition` (0,2; 0,5089). Prueba:
  `ForestScene_RF26_LaCajaYLosTroncosTerminanEnCuadroDondeLosDibujaLaEscena22`.
- **Auditoría de cámara** (`a9236f1`, 16/09). `PlayFraming` pasó de (0,27; 0,5) a zoom 1,18 a
  (0,25; 0,5) a zoom 1 —el plano general abarca el entorno— y `CompletionFraming` de y 0,40 a 0,42
  (zoom 1,58). Según su mensaje, el commit alinea 13 encuadres del Nivel 2 con
  `Camara_Narrativa_N2.md`.

### A.5 El taller: la cuerda, séptima pieza (INC-54)

`1d5ce58`, 25/09/2026, decisión de Santiago. El armado ya no termina al poner la caja: termina
**amarrándola**.

- `WorkshopPiece.Rope` (6) y `AssemblyStep.Rope` (6), después de `Cargo` (5).
  `AssemblySequence.IsComplete` exige ahora `LastCompleted == AssemblyStep.Rope`, y el nuevo
  `IsCargoPlaced` es la condición de la cuerda: soltarla antes de la caja se rechaza con
  `RopeTooEarlyMessage` («La cuerda todavía no tiene nada que sujetar.»), que dice qué falta sin
  dictar el paso (CP-06) y sin deshacer nada (CP-02). La cuerda se lleva con clic sostenido
  (`IsDraggable`), como el mazo, el tronco largo, la tabla y la caja.
- La carretilla no tiene dibujo con la cuerda: la pieza misma queda amarrada sobre la caja en
  `AssemblyContent.RopePlacedPosition` (0,681; 0,33), con `RopePlacedSize` 0,05 del alto, y pierde su
  `CargoHandle`. Los cinco estados de la carretilla (`prop_n2_carretilla_e1..e5`) no cambian. Arte:
  `prop_n2_pieza_2`.
- La nombran `N2_Guia` («…sobre ella la caja de alimentos, y amárrala con la cuerda») y la 2.3; el
  `CompleteMessage` dice «La cuerda sujeta la caja. La carretilla está completa.».
- **Pasos utilizados** de la fase 2 pasa de 5 a 6 acciones en orden (lo comprueba
  `WheelLevelJourneyTests`).
- Pruebas: nueva `AssemblySequence_INC54_LaCuerdaAmarraLaCajaYCompletaLaCarretilla`;
  `AssemblySequence_RF29_RechazaCadaPasoFueraDeSecuenciaConSuMensaje` rechaza la cuerda sin caja;
  `WorkshopScene_RF29_LaCarretillaCreceSobreLoAnteriorSinQueLaInterfazLaTape` exige la cuerda
  amarrada encima y `WorkshopScene_RNF02_ElMapaDeControlesSoloTieneClicYClicSostenido` suma
  `Pieza_Rope` a lo que atiende el clic sostenido.
- **INC-54 sigue abierto**: el guion §1.6.2.2 y RF-27/RF-29 cierran el armado con la caja.
  *(Nota del 29/09/2026: cerrado. El guion §1.6.2.1 y §1.6.2.2, RF-27, RF-29, HU-09 y CU-07 ya
  describen la cuerda como séptima pieza y amarrar la caja como último paso.)*
- Cámara (`a9236f1`): el taller juega a zoom 1 —`PlayFraming` pasó de (0,772; 0,47) a 1,32 a
  (0,75; 0,5) a 1— y `CompletionFraming` movió su foco de x 0,705 a 0,785 (zoom 1,56).

### A.6 El laberinto

- **Botones ▲/▼** (`d81cfc7`, 17/09). A partir de unos ocho bloques las filas no cabían ni
  comprimidas —se repartían el hueco y acababan de 26 px, una encima de otra—; ahora conservan su
  alto y la lista se desplaza en su ventana con `scrollUpButton` y `scrollDownButton`
  (`Boton_Subir`, `Boton_Bajar` en `Level2_Maze.unity`): el que no tiene a dónde ir se deshabilita y,
  si todo cabe, no se muestra ninguno. Sin `ScrollRect`, que trae el arrastre de uGUI (RNF-02,
  CT-06; lo vigila `MazeScene_RNF02_ElMapaDeControlesSoloTieneClicYClicSostenido`). Prueba:
  `MazeScene_RNF03_ConMasBloquesDeLosQueCabenLaListaSeDesplazaSinEncogerLasFilas`.
- `d7ace67` (23/09) sumó la barra de desplazamiento que se pulsa y no se arrastra (`ClickRelay`), la
  cuadrícula dentro del seto, la papelera del bloque seleccionado y el atardecer (A.3); el detalle
  está en `Props-y-Sonidos-Resultados.md` §2.6.
- **La franja lavanda** que `todo.md` dejó abierta el 16/09 —el tablero encaja la ilustración y
  deja 175 px del color del panel arriba y abajo— la resuelve desde `d7ace67`
  `MazeLayout.BackdropColor`: el panel se pinta (0,45; 0,45; 0,16) multiplicado por `LightTint`,
  como continuación del borde del entorno, que es la salida (a) de aquella nota.
- **`MazeLayout.ObstacleScale`** (`1d5ce58`): el arbusto se dibuja a 1,65 veces el tamaño de pieza.
  Trae margen transparente; con 1,3 llenaba su casilla justa y con 1,65 los contiguos se montan y se
  leen como seto.

### A.7 Troncos que ruedan como cilindros (`b38c00b`, 24/09; `1d5ce58`, 25/09; acta D09)

- `RollingLog` (`Game.Scaffolding`) es un `MaskableGraphic` de uGUI que dibuja el tronco como la
  proyección ortográfica de un cilindro visto en 3/4, envuelto en **una sola textura**
  (`prop_n2_tronco_textura`: la corteza desenrollada a la izquierda y el corte a la derecha). Sin
  cámara, modelo ni `MeshRenderer`: el costado va en 48 tiras por vuelta y solo se pintan las que
  miran a quien juega.
- **Lee el giro, no lo manda.** Quien mueve el tronco —`RollMotion` en la narrativa, el cursor en el
  bosque— sigue girando el `RectTransform` del objeto; `RollingLog` deshace ese giro para que el
  cilindro siga apuntando al fondo y lo pasa a la textura. `RollingLog.Attach(image, look)` lo
  cuelga como hijo `Tronco` de la imagen del objeto, que queda transparente y sigue recibiendo el
  clic.
- La vista es contenido: `RollingLogLook` (`AxisDegrees` 35, `TiltDegrees` 40, `Depth` 1,9 radios,
  `OutlineWidth` 0,16) en `Data/Wheel/N2_TroncoRodante.asset`. Lo enganchan
  `WheelLevelConfig.LogLook` —los troncos del suelo y los de la fila— y `NarrativeProp.Rolling`: en
  la 2.1 y la 2.2 desde `b38c00b`, en `N2_PuenteI_Bosque` y la 2.5 desde `1d5ce58`.
- En el bosque, el tronco que el cursor aparta **rueda**: gira lo que avanza en horizontal dividido
  entre su radio (`ForestSceneController.Roll`).
- `1d5ce58`: la textura gira como se ve girar el objeto, con espejo o sin él —el bosque dejó de
  compensarlo y el tronco del niño en la 2.2 ya no rueda al revés—, y se borraron
  `prop_n2_tronco_b..e`.
- Pruebas: `RollingLog_RF26_DeFrenteSoloSeVeElCorteYDeCostadoSoloLaCorteza` y
  `RollingLog_RF26_En34ElCorteSeAchataYElCostadoSeAlejaPorElEje` (`b38c00b`),
  `RollingLog_RF26_EnEspejoLaTexturaGiraComoSeVeGirarElObjeto` (`1d5ce58`). Para leer
  `RollingLog.Spin` desde la prueba, `1d5ce58` creó `Scaffolding/AssemblyInfo.cs` con
  `InternalsVisibleTo("Game.Scaffolding.Tests")`.

### A.8 La escena 2.2: una caja vacía que se desliza (`1d5ce58`)

La caja de la 2.2 pasó de `prop_n2_caja_suelo` en (0,2; 0,5089) con 0,148 del alto a
`prop_n2_caja_suelo_vacía` en (0,2; 0,545) con 0,12, apoyada sobre la fila; `MotionDrop` pasó de
0,074 a 0,08. Sigue con `PropMotion.Roll` en la línea 0 (0,07 del ancho en 2,95 s), pero sin
`Rolling`, así que `MoveAsync` ya no la gira: se desliza y solo se ladea al caer. Los que giran son
los troncos.

### A.9 «Omitir» y la 2.2 (`d81cfc7`, 17/09)

Al cierre, fuera del cierre reflexivo, «ya vista» era «el perfil confirmó alguna fase del nivel». Al
salir del bosque con la fase 1 recién confirmada, la 2.2 —que se veía por primera vez— ofrecía el
botón de omitir (lo vio Santiago el 17/09). Desde `d81cfc7`, `NarrativeVisitPolicy` exige haber
confirmado la **última** fase del nivel; el cierre reflexivo sigue mirando `ReachedLevel` (W16).
Prueba: `NarrativeVisitPolicy_RF06_UnaEscenaIntermediaNoSeOmiteEnLaPrimeraVuelta`.

### A.10 La pausa

`MenuPausa.prefab`, que W17 dejó con la disposición del mockup 6 y con el velo carbón al 72 % de
opacidad que ya tenía `PanelPausa` al cierre, recibió en `d81cfc7` su estilo visual: las
tipografías Baloo 2 y Nunito, los glifos `ui_reanudar`, `ui_reiniciar` y `ui_pausa` y los
recuadros `ui_panel` y `ui_boton`. Hoy está en las cinco escenas jugables, también `Level3_River`.

### A.11 Lo que este documento ya decía mal al cerrar

- **§4, W01.** `Game.Levels.Wheel` nació (`e7278a2`) referenciando `Game.Core` y
  `Game.Scaffolding`; en el mismo slice ganó `UnityEngine.UI` (`faaaa13`) y `Unity.InputSystem`
  (`eb941c8`), y después `Game.Audio` (`03675b9`). Nunca referenció otro nivel: la exclusión de
  RNF-16 sigue en pie.
- **§7.1, `ForestObject.cs`.** Las categorías son `RoundLog`, `Stone`, `Plant` y `Tool` desde W05
  (`817a327`); ninguna versión tuvo «rama».
- **§7.1, `CargoHandle.cs`.** El clic sostenido de la caja es el mismo de las piezas arrastrables del
  taller (`b6b886c`, W08/W09) y de los bloques del laberinto (`b1b423a`, W10–W14).
- **§4, W04.** `DialogueRunner` sí cambió dentro del slice, en W07 (A.2).
- **§7.5.** Faltaban las piezas del motor de escena (A.2) y `UI/LevelSelectController.cs`: desde W06
  (`faaaa13`) cada ficha declara su secuencia de apertura en `LevelEntry.openingSequenceId`, que es
  el campo que corrigió el hallazgo 3 de la §5 (el Nivel 2 pasó de `N2_Escena21_Bosque` a
  `N2_PuenteI`). Hoy `LevelSelect.unity` declara `N1_Apertura`, `N2_PuenteI` y `N3_PuenteII`.
- **W05/W06-R** (`eb941c8`) trajo también una maqueta de personaje, `CharacterProbe` y
  `CharacterProbeMotion` en `Game.EditorTools` (`Editor/Sandbox/`) —solo Editor, no es código del
  juego—, con `CharacterProbeMotionTests` (5).

### A.12 Pruebas y cifras al 25/09/2026

Casos por archivo del módulo, con el criterio de la §6 (cada `[Test]` más cada `[TestCase]`):

| Archivo | Al cierre | Hoy | De dónde vienen |
|---|---|---|---|
| `ForestSceneTests` | 28 | 37 | W19 (`863ef05`): −3, +2 · `d81cfc7`: +1 (`RF26_LaCajaYLosTroncosTerminanEnCuadro…`) y `RF25_LaCajaEsperaAlCuarentaPorCiento…` pasa a `RF25_LaCajaEsperaEnElSuelo…` · `03675b9`: +2 de sonido · `88fe0ee`: +7 de personajes |
| `MazeSceneTests` | 13 | 26 | `d81cfc7`: +1 (▲/▼) · `03675b9`: +1 de sonido · `d7ace67`: +6 (atardecer, salida, cuadrícula, lista abajo, barra, papelera) · `88fe0ee`: +5 de personajes |
| `WorkshopSceneTests` | 11 | 18 | `03675b9`: +1 de sonido · `88fe0ee`: +6 de personajes · `1d5ce58` tocó cinco por la cuerda: dos cambian su aserción (`RF29_LaCarretillaCreceSobreLoAnterior…`, `RNF02_ElMapaDeControles…`) y tres suman el arrastre de la cuerda (`RF29_PerforarSuena…`, `RF29_LaSecuenciaCompletaConfirmaYGuardaLaFase2`, `DA133_TodaLaFamiliaCelebra…`) |
| `ForestObjectNudgeTests` | 10 | 12 | `03675b9`: el aviso de acercamiento y la hoja que se posa |
| `AssemblySequenceTests` | 7 | 8 | `1d5ce58`: `AssemblySequence_INC54_…` |
| `PatternSelectionTests` · `MazeGridTests` · `CargoPlacementTests` · `WheelIndicatorTests` · `SequenceExecutorTests` · `BlockSequenceTests` · `WheelLevelConfigTests` | 14 · 9 · 7 · 7 · 5 · 4 · 3 | Igual | `1d5ce58` renombró una de `PatternSelectionTests` (A.4) |
| `PauseMenuWheelTests` · `WheelAccessibilityTests` · `WheelLevelJourneyTests` | 3 · 3 · 2 | Igual | W19 cambió lo que muestrea la de RNF-21; `1d5ce58` subió a 6 los pasos esperados de la fase 2 |
| **Módulo** | **EditMode 66 · PlayMode 60** | **EditMode 69 · PlayMode 89** | |

- **Fuera del módulo**, del Nivel 2 y sin nombrar al cierre: `RollMotionTests` (2) e
  `IllustrationFramingTests` (A.2); `RollingLogTests` (3, A.7);
  `DialogueRunner_RF05_ElProgresoVaDeCeroAUnoAlRitmoDeLasLineas` (A.2);
  `NarrativeVisitPolicy_RF06_UnaEscenaIntermediaNoSeOmiteEnLaPrimeraVuelta` (A.9);
  `GameFlowRunner_RF22_JugarLaFase1DelNivel2CargaLaEscenaDelBosque` (`faaaa13`) y
  `GameFlowRunner_RNF14_ConLasDosPrimerasFasesConfirmadasEntrarAlNivel2RetomaEnElLaberinto`
  (`b1b423a`); y `CharacterProbeMotionTests` (5, A.11).
- **Pruebas sensibles al ratón** (§5.5): cerradas en `d81cfc7`; las dos llaman a
  `ElCursorLejos(forest)` antes de medir.
- **Trazabilidad RF-22..RF-34**, con el mismo barrido que la §1 (nombres de método bajo
  `Assets/Tests`): RF-22 (14), RF-23 (10), RF-24 (7), RF-25 (6), RF-26 (12), RF-27 (1), RF-28 (4),
  RF-29 (6), RF-30 (4), RF-31 (7), RF-32 (2), RF-33 (2), RF-34 (4). Todos siguen con al menos una.
- **Tamaño del módulo**: `Game.Levels.Wheel` tiene 23 archivos y 5 209 líneas —entraron
  `WheelSounds.cs` (`03675b9`) y `ClickRelay.cs` (`d7ace67`)—; sus pruebas, los mismos 15 archivos
  con 5 385 líneas.
- **Cifra de suite**: este anexo no la da; la corrida completa del 25/09/2026 está en
  `claudeDocs/tasks/Slice 4/Slice-4-Resultados.md`, «Corrida completa de la suite (25/09/2026)».

### A.13 Lo que queda abierto al 25/09/2026

- **INC-50 e INC-54**, abiertos: falta editar RF-26, HU-08 y CU-06 (el rodado; el guion §1.6.1.2
  también lo cuenta dentro de la mecánica) y el guion §1.6.2.1 y §1.6.2.2, RF-27, RF-29, la
  historia de la fase 2 y CU-07 (la cuerda).
- **La caja del bosque y la de la 2.2 ya no coinciden.** `WheelLevelConfig.CargoPlacedPosition`
  (0,2; 0,5089) sigue diciendo en su tooltip que es «el punto en el que la escena 2.2 la dibuja al
  abrir», y el bosque la pinta con `prop_n2_caja_suelo`; desde `1d5ce58` la 2.2 la dibuja vacía en
  (0,2; 0,545) y más pequeña (A.8). `ForestScene_RF26_LaCajaYLosTroncosTerminanEnCuadroDondeLosDibujaLaEscena22`
  no lo ve porque compara con una constante copiada (`CajaColocadaDeLa22`), no con el asset. Queda
  por decidir si el salto al cortar a la narrativa es buscado.
- **`WorkshopScene_RF27_PresentaLasSeisPiezas`** conserva «Seis» en el nombre y su mensaje enumera
  las seis del guion, aunque compara contra los siete valores de `WorkshopPiece`.
- **Comentarios del código que quedaron atrás**: la cabecera de `RollMotion` («el mismo en la
  mecánica del bosque y en la escena narrativa»; hoy solo lo usa la narrativa), la de
  `RollingLogLook` («lo comparten el bosque y las escenas 2.1 y 2.2»; hoy también
  `N2_PuenteI_Bosque` y la 2.5) y el tooltip de `NarrativeSequence.LightStart` («el Nivel 2 no usa
  la capa»; desde `d7ace67` sí, A.3).
- **La casilla de la franja lavanda** sigue sin marcar en `todo.md`, aunque `BackdropColor` la
  resuelve (A.6).
- **`docs/md/Camara_Narrativa.md` y `docs/md/verificacion_encuadres_N2/`** se perdieron el
  20/09/2026. Las hojas de verificación se pueden regenerar desde el contenido; mientras tanto, lo
  aplicado solo vive en los `N2_*.asset`.
- Los puntos de la §8 que dependen de la build portable o del usuario (RNF-04..RNF-06, PG-05, la
  revisión con el usuario) no se revisaron para este anexo.

*(01/10/2026: de esta lista, INC-50 e INC-54 se cerraron el 29/09/2026 corrigiendo los documentos, y
la caja de la 2.2 volvió a ser la del bosque —la prueba lee ya el asset—. El resto sigue como está;
ver Anexo B, B.5, B.6 y B.8.)*

---

## Anexo B — lo que cambió después del 25/09/2026 (01/10/2026)

El Anexo A se verificó el 25/09/2026 sobre `ccf77e6`. Entre el 25/09 y el 01/10 el Nivel 2 cambió
en cuatro commits que ya están en `main` —uno de ellos solo de documentos— y en la rama
`feat/cierre-de-slices-y-oe3`, que entra en el commit de cierre con las tarjetas D10-2 y D10-5 del
acta D10. Todo se comprobó contra el código, los assets, `git log` hasta `359365e` y la suite
completa del 01/10/2026.

### B.1 Qué cambió y dónde

| Fecha | Commit | Qué cambió en el Nivel 2 |
|---|---|---|
| 25/09 | `44fd479` | Props definitivos a 256 y 512 px; las etapas del taller siguen al arte nuevo — B.2 |
| 29/09 | `d105838` | El recorrido del nivel exige ver la 2.5 antes del desbloqueo; un doble clic al salir de un cierre reflexivo ya no salta el resumen — B.3 |
| 30/09 | `37b3cb7` | Contorno en el lado de «Girar» y candado en «Empujar» (INC-58), rótulos del laberinto en `MazeLayout` (INC-104), la cuerda en la 2.3 (INC-54) y el mazo soltado lejos que no cuenta (INC-115) — B.4 |
| 30/09 | `5a22df1` | Documentos: INC-49, INC-50 e INC-54 cerrados — B.5 |
| 01/10 | sin hash, D10-2 y D10-5 | La 2.2 abre con la caja del bosque (INC-120), la carretilla pasa a `e5` al amarrar la cuerda (INC-121) y el contraste del nivel exige marco — B.6 |

`44fd479` entró en `main` con el PR #87 (`995b26d`, 29/09); `d105838`, `37b3cb7` y `5a22df1`, con el
PR #88 (`1c7f4ab`, 30/09).

### B.2 `44fd479`: props definitivos y etapas del taller

Los props del Nivel 2 bajan a 256 × 256, y a 512 × 512 donde se ven a más de 256 px a 1080p
(`prop_n2_carretilla_e2` a `_e5`, `prop_n2_pieza_3` y `_4`); el recorte de las piedras, en modo
Multiple, se escaló desde el motor conservando su `spriteID`. El arte nuevo corre las etapas del
taller un puesto y `N2_AssemblyContent` lo sigue: `e1` es la rueda perforada, `e2` el eje, `e3` la
tabla y `e4` la caja. `e5`, con la cuerda, quedó para las narrativas hasta el 01/10 (B.6). El detalle
archivo por archivo está en `Slice 3/Props-y-Sonidos-Resultados.md`, Anexo B.

### B.3 `d105838` (29/09/2026): la 2.5 se ve antes del desbloqueo, y el doble clic

- `WheelLevel_RNF13_RecorreElNivel2CompletoHastaElMenuConNivel3Desbloqueado` exige ahora que, al
  llegar a `N2_Escena25_Cierre`, el Nivel 3 siga bloqueado y no haya «Omitir» (CP-07, RF-12):
  `NarrativeVisitPolicy` da el cierre reflexivo por visto cuando el nivel siguiente ya está abierto,
  así que desbloquear al llegar al refugio le daría «Omitir» a la primera vuelta. Se validó con una
  mutación de `MazeSceneController` que desbloqueaba antes de tiempo.
- `NarrativeSceneController.Leave()` sale una sola vez: si el flujo ya no está en esa narrativa, no
  hace nada. Un doble clic en «Continuar» u «Omitir» al salir de un cierre reflexivo llegaba dos veces
  mientras cargaba el resumen; el segundo `GoTo(LevelSummary)` ya no era válido, el flujo caía al
  menú de niveles y se saltaba el resumen donde se nombra la habilidad (RF-12, RF-45). Pruebas:
  `LevelSummary_RF45_DobleClicEnContinuarMientrasCargaElResumenSeQuedaEnElResumen`, sobre la 2.5 y con
  `SceneLoader` real, y `NarrativeScene_RF45_UnClicDeMasAlSalirDelCierreReflexivoYaVistoLlegaAlResumen`,
  sobre `N1_NacimientoDelFuego` con «Continuar» y con «Omitir».
- El mismo commit trajo el plan de pruebas del OE4; se registra en `Slice 4/Slice-4-Resultados.md`,
  Anexo B.

### B.4 `37b3cb7` (30/09/2026): lo que el Nivel 2 ganó al cerrar INC-46 a INC-117

- **«Girar»** (INC-58, RNF-19): el lado elegido lleva contorno además del tinte.
  `MazeScene_RNF19_ElLadoElegidoDeGirarSeDistingueSinColor`.
- **«Empujar»** deshabilitado lleva candado, como «Soplar» (INC-58, completado el 30/09).
  `ForestScene_RNF19_EmpujarDeshabilitadoLlevaCandado`.
- **Rótulos del laberinto** («Avanzar», «Retroceder», «Girar»): salen del diccionario `Labels` de
  `MazeSceneController` y pasan a `MazeLayout` (`ForwardLabel`, `BackwardLabel`, `TurnLabel` en
  `N2_MazeLayout`), porque todo texto visible vive en datos (INC-104, CT-05).
- **La 2.3 pinta la cuerda** que nombra Algoritm (INC-54): `N2_Escena23_Construccion` gana el objeto.
  `Content_INC54_LaEscena23PintaTodasLasPiezasDelTaller`.
- **El mazo soltado lejos** del tronco resaltado se describe con el mismo mensaje y ya no suma
  intento, igual que las demás piezas (INC-115, decisión de Santiago del 30/09/2026).
  `WorkshopScene_RF29_SoltarElMazoLejosNoCuentaComoIntento`.
- **La pausa**: `PauseMenu_HU17_OfreceExactamenteReanudarReiniciarYVolverAlMenuDeNiveles` fija los tres
  rótulos del mockup 6 (INC-49), y el comentario de `PauseMenuController` dice cinco escenas
  jugables.

### B.5 `5a22df1` (30/09/2026): INC-49, INC-50 e INC-54, cerrados

Se cerraron el 29/09 corrigiendo los documentos, con la regla de ese día: gana el juego.

- **INC-49**: RF-07, HU-17 y la arquitectura usan los rótulos del mockup 6. Corrige el punto de §8.
- **INC-50**: RF-26, el guion §1.6.1.2, CU-06 y HU-08 dicen que «Empujar» confirma la fase y que la
  2.2 muestra el rodado. Corrige A.1.
- **INC-54**: el guion §1.6.2.1 y §1.6.2.2, RF-27, RF-29, HU-09 y CU-07 cuentan la cuerda como séptima
  pieza. Corrige A.5.

### B.6 Correcciones del 01/10/2026 (tarjetas D10-2 y D10-5)

Las dos primeras son decisiones de Santiago del 30/09/2026 (acta D10, §5).

- **La 2.2 abre con la caja del bosque** (INC-120). La caja de `N2_Escena22_ElPatron` vuelve a
  `prop_n2_caja_suelo` en (0,2; 0,5089), con 0,14814815 del alto y `MotionDrop` 0,074074075: es
  exactamente lo que `1d5ce58` había cambiado, puesto al revés —`git diff 1d5ce58~1` del asset sale
  vacío—. Sigue deslizándose sin girar (A.8), y `prop_n2_caja_suelo_vacía` queda sin uso. Con esto
  vuelve a ser cierto el tooltip de `WheelLevelConfig.CargoPlacedPosition`: es el punto donde la 2.2
  dibuja la caja al abrir. Pruebas: `WheelLevelConfig_RF05_LaCajaColocadaEsLaQueLaEscena22DibujaAlAbrir`
  (nueva) y `ForestScene_RF26_LaCajaYLosTroncosTerminanEnCuadroDondeLosDibujaLaEscena22`, que deja la
  constante copiada `CajaColocadaDeLa22` y lee el asset.
- **Al amarrar la cuerda, la carretilla pasa a `e5`** (INC-121). `AssemblyContent.TiedArt`
  (`prop_n2_carretilla_e5`, el dibujo con que abre la 2.4) y `case Rope` en
  `WorkshopSceneController.ShowAssembly`: la cuerda aceptada se desactiva como las demás piezas y la
  carretilla cambia de dibujo en el sitio —`e4` y `e5` tienen la misma caja de alfa, así que no salta—.
  Salen `RopePlacedPosition`, `RopePlacedSize` y la rama de la cuerda colgada de `Release`. Pruebas:
  `AssemblyContent_INC54_LaCarretillaAmarradaEsElDibujoConElQueAbreLaEscena24` (EditMode) y
  `WorkshopScene_INC54_AlAmarrarLaCuerdaLaCarretillaPasaAlDibujoConCuerda` (PlayMode); la de
  `WorkshopScene_RF29_LaCarretillaCreceSobreLoAnteriorSinQueLaInterfazLaTape` exige ahora `TiedArt` y no
  la cuerda colgada. Corrige A.5 («la carretilla no tiene dibujo con la cuerda»).
- **El texto va siempre sobre marco** (D10-5). `WheelLevel_RNF20_ContrasteSuficienteEnLasTresEscenas`
  mide cada texto del bosque, el taller y el laberinto contra su cara real y exige además que esa
  cara no sea la ilustración del entorno, como la prueba del río. La aserción nueva se puso en rojo
  con dos mutaciones —el mensaje del bosque colgado del entorno— antes de pasar.

La revisión adversarial del 01/10/2026 comprobó en capturas la caja llena de la 2.2 por encima del
cuadro de diálogo y el taller con `e4` y con `e5`. Faltan dos capturas en las pruebas de verificación
visual: el primer cuadro de la 2.2, con la caja sobre el primer tronco antes de rodar —la de la
prueba se toma con el rodado ya asentado—, y la del taller con la carretilla amarrada
(`Workshop_04_carretilla_amarrada`).

### B.7 Pruebas y cifras al 01/10/2026

| Assembly | 25/09 (`ccf77e6`) | 30/09 (`359365e`) | 01/10 | Qué lo movió |
|---|---|---|---|---|
| `Game.Levels.Wheel.Tests` | 69 | 69 | 71 | D10-2: `WheelLevelConfig_RF05_…` y `AssemblyContent_INC54_…` |
| `Game.Levels.Wheel.PlayMode.Tests` | 89 | 92 | 93 | `37b3cb7`: los dos RNF19 y `WorkshopScene_RF29_SoltarElMazoLejos…`; D10-2: `WorkshopScene_INC54_…` |

Fuera del módulo tocan el Nivel 2 las dos RF45 de `d105838` (`Game.UI.PlayMode.Tests`) y
`Content_INC54_…` (`Game.Content.Tests`). La suite completa del 01/10/2026, con el Editor abierto a
1920 × 1080, dio EditMode 393 = 392 + 1 omitida y PlayMode 351 = 350 + `RiverLevel_RNF05_…`, que mide
la memoria del Editor y pasa aislada; todas las del Nivel 2, en verde. El detalle está en
`Slice 4/Slice-4-Resultados.md`, «Corrida completa de la suite (01/10/2026)».

### B.8 Lo que queda abierto al 01/10/2026

Frente a A.13:

- **Cerrados**: INC-50 e INC-54 (B.5) y el desajuste entre la caja del bosque y la de la 2.2 (B.6).
- **Siguen**: `WorkshopScene_RF27_PresentaLasSeisPiezas` con «Seis» en el nombre; los comentarios de
  `RollMotion` y del tooltip de `NarrativeSequence.LightStart` («el Nivel 2 no usa la capa»); la
  casilla de la franja lavanda en `todo.md`, y las hojas de verificación de encuadres por regenerar.
- **Nuevos**: las dos capturas de B.6; el nombre de `prop_n2_caja_suelo_vacía`, que lleva tilde contra
  `Direccion_de_Arte.md` §15.4 y ya no usa nadie.
- **De la §8**, por decisión de Santiago del 30/09/2026 (acta D10, §5): las revisiones con el usuario
  se marcan como hechas por él tras verificarlas con pruebas y capturas; RNF-04 a RNF-06 y la
  exclusión de RNF-16 retirando un nivel se miden sobre el ejecutable candidato; **PG-05** queda a
  cargo de Santiago, en la sesión con estudiantes (`claudeDocs/tasks/OE4/Hoja-HUM.md`, H1).

### B.9 Lo que entró después, el mismo 01/10/2026 (tarjeta D10-4 y verificación final)

Apartado nuevo. Cierra los dos «Nuevos» de B.8.

- **Las dos capturas que faltaban.** `WorkshopScene_RNF20_CapturaDelTallerEnReposoYConLaCarretillaMontada`
  toma una cuarta, `Workshop_04_carretilla_amarrada`, tras arrastrar la caja y la cuerda: la
  carretilla en `e5`, con la caja atada. `NarrativeScene_RF05_CapturaDelPrimerCuadroDeLaEscena22`
  (nueva, `Game.UI.PlayMode.Tests`) fotografía `N2_Escena22_PrimerCuadro` en el primer cuadro, antes
  de que nadie lea: la caja llena entera sobre el pasto, sin rodar y por encima del cuadro de
  diálogo. La revisión las vio así. Las dos son `[Category("VisualVerification")]`.
- **Dos nombres del nivel, según la nomenclatura** (INC-126). `entorno_n2_laberinto` pasa a
  `env_n2_laberinto` y `prop_n2_caja_suelo_vacía` a `prop_n2_caja_suelo_vacia`, renombrados desde el
  motor con su GUID: escenas y assets los referencian por GUID y no cambian. Se corrige el comentario
  de `MazeLayout.cs`, la única cita por nombre en el código. Lo vigila
  `ArtImport_RNF23_LosNombresSiguenLaNomenclatura`. En este documento los nombres viejos (§7.4, A.8,
  B.6 y B.8) quedan como registro. La caja vacía sigue sin uso (INC-120).
- **Diez props del bosque siguen en modo `Multiple`** (excepción de INC-128). `env_enlace_n2`,
  `prop_n2_herramienta_a` a `_c`, `prop_n2_piedra_a` y `_b`, `prop_n2_planta_a` a `_c` y
  `prop_n2_tronco_a` llegaron con un solo sprite recortado (`<nombre>_0`), y las escenas y los assets
  los referencian por ese sub-sprite: pasarlos a `Single` cambiaría su `fileID` y rompería esas
  referencias. `prop_n2_piedra_c` y `_d` también están en `Multiple` y no los usa nadie.
- **Cifras.** `Game.Levels.Wheel.Tests` sigue en 71 y `Game.Levels.Wheel.PlayMode.Tests` en 93, todas
  en verde en la verificación final del 01/10/2026: EditMode 433 = 432 + 1 omitida y PlayMode
  364/364. Detalle en `Slice 4/Slice-4-Resultados.md`, «Verificación final y paquete (01/10/2026)».
- **Queda**: la `[Description]` de la prueba del taller dice que `_04` es «tras el empuje de
  cierre», y la captura es del cuadro siguiente al amarre, cuando el empuje apenas empieza. Es solo
  redacción de una prueba; se corrige después de la pasada del OE4, porque tocarla ahora cambia la
  huella del ejecutable candidato.


### B.10 Nota (01/10/2026, ~21:45): lo que añadió rc2 al laberinto

Apartado nuevo. Las cifras de B.7 y B.9 son las de rc1; esta nota da las vigentes.

- **`MazeSceneTests` tiene 33 pruebas** (cada `[UnityTest]`; el archivo no usa `[TestCase]`). Eran
  26 al 25/09 (A.12) y 27 en `359365e`, por `MazeScene_RNF19_ElLadoElegidoDeGirarSeDistingueSinColor`
  (`37b3cb7`). rc2 suma seis, que cubren los defectos que la pasada del OE4 encontró en rc1
  (`OE4/OE4-Resultados.md` §8.3):
  - DEF-GP1-01, el bloque que se quedaba pegado al cursor:
    `MazeScene_RF34_ReordenarUnBloqueEsUnSoloGestoYNoQuedaPegado`,
    `…_RetirarYReordenarArrastrandoCuentanSoloLasEdicionesHechas` y
    `…_UnClicSinMoverSobreUnBloqueLoDejaDondeEstabaYNoCuentaEdicion`;
  - DEF-GP1-02, el bloque en curso fuera de la vista al ejecutar:
    `MazeScene_RF32_AlEjecutarLaListaMuestraElBloqueEnCurso`;
  - DEF-GP1-03, la selección tras retirar un bloque:
    `MazeScene_RF34_TrasLaPapeleraConElCajonCerradoLasFilasLaOfrecen` y
    `…_TrasRetirarArrastrandoLasFilasOfrecenLaPapelera`. Además cambia la aserción de
    `MazeScene_RF34_ElBloqueSeleccionadoMuestraLaPapeleraYEstaLoRetira`, que exigía el defecto.
- **`SequenceListRulesTests` (10, EditMode, nueva)** prueba en C# plano `SequenceListRules`: el
  desplazamiento mínimo con ▲/▼ que deja ver el bloque en curso (`RevealScroll`, sin `ScrollRect`) y
  la fila que queda seleccionada tras retirar (`SelectionAfterRemoval`).
- **Cifras del módulo:** `Game.Levels.Wheel.Tests` pasa de 71 a 81 y
  `Game.Levels.Wheel.PlayMode.Tests` de 93 a 99. La suite completa antes del build de rc2 dio
  PlayMode 376/376 y EditMode 470 = 469 + 1 omitida (`ProfileEraser_INC34`, por entorno)
  (`OE4/evidencias/suites/rc2-final/`).
- **Sobre el ejecutable rc2**, PF-RF34-02, PF-RF32-01 y PF-RF34-01 pasan (P): reordenar y retirar son
  un solo gesto, nada sigue al cursor, la lista sube sola hasta el bloque en curso y tres papeleras
  seguidas con el cajón cerrado vacían la secuencia.
