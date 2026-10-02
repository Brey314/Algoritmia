# Slice 3 — El Río y el cierre del juego: lo construido y sus resultados

Documento de cierre del tercer incremento, hermano de [`../Slice 2/Slice-2-Resultados.md`](../Slice%202/Slice-2-Resultados.md).
Registra lo hecho en la rama `feat/slice-3`, con qué se verificó y qué quedó abierto. El tablero
vivo sigue siendo [`todo.md`](todo.md) —si este documento y el tablero se contradicen, gana el
tablero— y el plan técnico es [`plan.md`](plan.md). Ninguno de los tres rediscute
`claudeDocs/SPEC.md`.

> **Estado al 25/09/2026:** lo que sigue es la foto del cierre del 21/09 y no se reescribe; lo que
> cambió después va en notas fechadas junto a cada afirmación vencida y en el **Anexo** del final.
> En corto: `d81cfc7` (17/09, dentro de la rama y sin registrar aquí) endureció la regla de
> «Omitir» y rediseñó el menú de pausa (§A.1); INC-51 fija que el cierre del N3 no se omite nunca,
> tampoco al repetirlo (§A.2); el puente II pasa por el horizonte —siete `N3_*.asset`— y las
> hogueras llevan humo y llama animados (§A.3); los personajes del río están en
> [`../Personajes/Personajes-Resultados.md`](../Personajes/Personajes-Resultados.md) y el sonido del
> N3 en [`Props-y-Sonidos-Resultados.md`](Props-y-Sonidos-Resultados.md), Anexo §A.2.1 (y §A.4,
> §A.7, §A.8); `ccf77e6` cambió
> cuándo se guarda cada fase del ensamblaje (§A.4). La corrida completa vigente de la suite está en
> [`../Slice 4/Slice-4-Resultados.md`](../Slice%204/Slice-4-Resultados.md), «Corrida completa de la
> suite (25/09/2026)».
>
> **01/10/2026:** lo que cambió del 25/09 al 01/10 —la balsa del ensamblaje en 3/4 (`44fd479`), el
> cierre de INC-46 a INC-117 (`37b3cb7`, `5a22df1`) y las correcciones del Nivel 3 del acta D10: la
> escena final con sonido, la balsa del cruce bajo la espuma, los viajeros que no saltan, «Probar
> balsa» en base y amarre y el plano abierto a la escala de las narrativas— está en el
> [Anexo B](#anexo-b--lo-que-cambió-después-del-25092026-01102026). La suite vigente es la del
> 01/10/2026, en el mismo `Slice-4-Resultados.md`.

| Campo | Dato |
|---|---|
| **Incremento** | Slice 3 — El Río (Nivel 3) y cierre del juego |
| **Rama** | `feat/slice-3`, desde `36442aa` (`main`, 15/09/2026) |
| **Fechas de ejecución** | 16/09/2026 – 21/09/2026 |
| **Commits en la rama** | 7 (`a9236f1` … `1179dae`), más el Checkpoint R-E, el acta D07 y este documento pendientes de commit |
| **Volumen frente a `main`** | 262 archivos, +17 733 / −866 líneas (hasta `1179dae`) |
| **Código del módulo** | `Game.Levels.River`: 23 archivos, 2 486 líneas *(Vencido el 25/09/2026: hoy 24 archivos y 2 750 líneas; ver §A.5.)* |
| **Pruebas del módulo** | 14 archivos, 2 642 líneas — 39 casos EditMode + 32 PlayMode *(Vencido el 25/09/2026: hoy 16 archivos y 3 070 líneas — 41 + 44; ver §A.5.)* |
| **Verificación** | EditMode **282/282** · PlayMode: recorridos del N3 **6/6**, `Core` + RNF-02 **17/17**, suite completa 165/184 (13 que solo corren con Game View, §6) · 21/09/2026 *(cifras del 21/09; la corrida vigente está en `Slice-4-Resultados.md`, «Corrida completa de la suite (25/09/2026)».)* |
| **Presupuestos medidos** | Carga de `Level3_River` **1,36 s** (< 10 s) · memoria **1 226 MB** reservados (< 2 GB) · paquete **217 MB** (< 500 MB) |
| **Fase de la metodología Árcade** | Desarrollo — ejecución del ciclo Diseño ↔ Desarrollo ↔ Pruebas |

---

## 1. Alcance: qué cierra este slice

El Nivel 3 completo —recolección con botones en pantalla, ensamblaje de la balsa en tres fases
bloqueantes, prueba y depuración— y el **cierre del juego**: cruce, resumen, escena final,
créditos y vuelta al inicio. Es además el primer momento en que RNF-02 y RNF-16 se pueden cerrar
con los tres niveles existiendo de verdad.

| Módulo | Qué entró | Qué no |
|---|---|---|
| `nivel-rio` | Orilla con cuatro flechas y «Recoger», recolección por proximidad, inventario por clase, lista de cuatro tareas, zona de construcción, ensamblaje sobre el río (base · amarre · mástil y vela), prueba de la balsa con hundimiento y devolución de solo lo mal puesto | — |
| `sistema-navegacion` | Cierre del juego `LevelSummary → Narrative → Credits → MainMenu` (INC-39); RNF-02 y RNF-16 sobre las cinco escenas jugables y el proyecto entero | Informe docente y borrado de datos (Slice 4) |
| `andamiaje` | Pistas por paso del N3 (`N3_Guia`), las cinco escenas narrativas en seis assets *(Vencido el 25/09/2026: siete desde el 23/09, con `N3_PuenteII_Horizonte`; ver §A.3.)*, una **condicional** al primer fallo, cierre reflexivo del río y escena final del juego | — |
| `progreso-registro` | **Emisión** de los cuatro indicadores del N3 con un solo recolector para las tres fases (OE1 §3.6.1) | Agregación y presentación docente (Slice 4) |

**Requerimientos cerrados:** `RF-35`..`RF-44`, todos de prioridad Alta, cada uno con al menos una
prueba que lo nombra (CT-10; barrido del 21/09/2026: RF-35 (3), RF-36 (4), RF-37 (3), RF-38 (4),
RF-39 (4), RF-40 (4), RF-41 (3), RF-42 (1), RF-43 (3), RF-44 (1)). Con ellos quedan implementados
los 45 RF de prioridad Alta salvo RF-46 y RF-47, que son el Slice 4. **No funcionales cerrados con
prueba:** RNF-02, RNF-16, RNF-13 (recorridos del N3), RNF-14 (retoma por fase), RNF-04/05/06
(medidos, §6), RNF-19/20/21 sobre el N3.

---

## 2. Seguimiento metodológico — las actas del OE3

La serie `OE3` (`docs/actas/OE3/`) está completa y restablecida: `D01`..`D07`. Cada tramo del
trabajo está radicado en un acta y el tablero Kanban vive en su §6. *(Al 25/09/2026 la serie
llega a `D09` (24/09); `D08` (23/09) cambia el responsable del sonido, ver §A.6.)*

| Acta | Fecha | Qué aportó al Slice 3 |
|---|---|---|
| **D05** | 13/09/2026 | Reparto: Santiago Benavides Rey cierra los slices; Santiago Valdiri García toma el sonido (D05-2). La regla «lo que el texto nombra se ve entero y por encima del cuadro de diálogo», que el N3 aplica en sus seis assets |
| **D06** | 15/09/2026 | Recepción de los entornos finales —`env_n3_rio`, sobre el que se juega y se narra todo el N3, y `env_final_fogatas`— y cierre de la rama del Slice 2. Tarjetas `D06-1` props (Sofía Giraldo), `D06-2` entornos definitivos e índice de sprites, `D06-3` fusionar la rama |
| **D07** | 19/09/2026 | [`Acta_D07_2026-09-19.md`](../../../docs/actas/OE3/Acta_D07_2026-09-19.md): define el tramo de ensamblaje y la escena 3.2 (R09–R12, commit `3fe1e2a`), revisa el avance del sonido y el de los sprites, da por finalizada `D06-3` y abre **D07-1** cerrar el Slice 3 (Benavides), **D07-2** cerrar la implementación de sonidos (Valdiri) y **D07-3** seguimiento de sprites (ambos), las tres con fecha límite 27/09/2026 |

**Movimiento del tablero que este documento respalda.** **D07-1** queda cumplida en su parte de
código el 21/09/2026 —seis días antes de su fecha límite—: las dieciséis tarjetas R01–R16
cerradas y el Checkpoint R-E con todo lo que se puede ejecutar en verde. Lo que queda de la
tarjeta es lo que exige la build portable o a una persona (§8). **D07-2** (sonido) y **D07-3**
(sprites) siguen en proceso y no son de este carril.

**Decisiones del acta D07 que este slice aplica y que no salieron del código:** el ensamblaje se
juega sobre el río sin ventana superpuesta; la balsa se **compone** en el motor con ocho piezas
—cada clase con su dibujo y su silueta— en lugar de producirse como treinta y cuatro láminas de
estado; colocar no valida, valida el botón; el tronco más largo es el mástil y es una trampa
deliberada de la base; y la escena 3.2 narra sin reiniciar.

---

## 3. Decisiones que definieron el slice

Ninguna salió del código; están registradas en el tablero con su fecha.

1. **Tres fases, y la recolección no se persiste** (16/09, pregunta abierta 1). `PhaseId.PhasesPerLevel = { 1, 3, 3 }`;
   al retomar, la recolección se da por hecha (`RiverSceneController.ResumeAt`).
2. **El ensamblaje se arma sobre el río, sin modal** (20/09). La cámara empuja del plano de juego
   a `(0.50, 0.23) ×2.2`, sombra negra al 30 % y la balsa en la mitad; **composición, no
   láminas**: diecisiete espacios pintados con ocho sprites (pieza + silueta por clase) en vez de
   34 láminas de estado. Cinco troncos, diez amarres de un solo rollo, mástil y vela.
3. **Arrastre sin Input System ni arrastre de uGUI.** Asas `IPointerDown/Up` en casillas y
   espacios y un `PointerTracker` en el `Canvas`; `Game.Levels.River` no referencia
   `Unity.InputSystem` y una prueba lo vigila (INC-01).
4. **El plano de la recolección es solo el bosque** (20/09): foco `(0.20, 0.20) ×2.5`, el piso
   termina en `GroundTop = 0.36` y hay perspectiva por profundidad; el río entra con el empuje.
   **Desde el 25/09** el plano se abre a `(0.2632, 0.2632) ×1.9` para que asome un poco el río
   (decisión de Santiago); el empuje del ensamblaje pasa a acercar hacia él.
5. **Un solo recolector de indicadores para las tres fases** (R13): §3.6.1 define «pasos
   utilizados» sobre el nivel entero; solo el tiempo es por fase y el reloj de la base arranca al
   abrir el ensamblaje, no en la orilla. *(Desde el 25/09/2026, `ccf77e6`: el reloj de RF-45 se
   para al confirmar y no al acabar la animación; las fases 1 y 2 se guardan al aprobarse y la 3
   al salir al cruce. Ver §A.4.)*
6. **El flujo del cierre lo deciden los assets** (R14): `NarrativeSequence.EndsInCredits`,
   `LevelSummaryMessages.ClosingSequenceId` y `PropMotion.Drift` para el cruce; ninguna rama por
   nivel en los controladores. `N3_EscenaFinal` lleva las dos marcas —final y cierre reflexivo—
   porque la segunda es la que le niega «Omitir» la primera vez (CP-07) sin tocar
   `NarrativeVisitPolicy`. *(Precisado el 25/09/2026: no solo la primera vez. El N3 no tiene
   nivel siguiente, así que el cruce y la escena final no se omiten nunca, tampoco al repetir el
   nivel; es INC-51, aceptado el 23/09. Ver §A.2.)*
7. **Reparto por assembly, no por slice** (heredado del 10/09): este carril tocó
   `Game.Levels.River`, `Game.Core`, `Game.Scaffolding`, `Game.UI` y las escenas, siempre con
   prueba, y lo cruzado quedó declarado en `CLAUDE.md`.

*(01/10/2026: los encuadres de las decisiones 2 y 4 los sustituye la lectura B del Nivel 3, decisión
de Santiago del 30/09/2026 (INC-118): la recolección pasa a (0.3572, 0.3572) ×1.4 —la orilla con el
río y el pie de la cascada— y el ensamblaje a (0.50, 0.3125) ×1.6, con la balsa en (0.58, 0.23); la
familia, los materiales y la zona toman la escala de las narrativas. Ver Anexo B, B.8.)*

---

## 4. Lo construido, fase por fase

### Fase 0 — Cimientos (R01, R02 · 16/09/2026)

`Game.Levels.River.asmdef` con referencia única a `Game.Core` y sus dos `InternalsVisibleTo`;
`AssemblyDependencyTest` exige exactamente tres niveles antes de recorrer la exclusión. R02 no
necesitó código: W02 ya dejaba `PhasesPerLevel` y `LevelUnlockPolicy` genéricos; entraron las
pruebas que nombran al Nivel 3 (`SaveStore_RF04_ConfirmarUnaFaseDelNivel3SobreviveAlCierre`,
`PlayerProfile_RF41_UnaFaseAprobadaNoSePierdeTrasUnaPruebaFallida`).

### Fase 1 — Andamiaje (R03, R04 · 16/09/2026)

`HintPolicy` no cambió: es genérica. Entró `N3_Guia.asset` con cuatro pasos —`Recolectar`,
`Base`, `Amarre`, `MastilYVela`— y las pruebas que fijan lo que la pista no puede decir (CP-06).
Las cinco escenas del guion viven en **seis** `N3_*.asset` porque la ilustración es por secuencia
y el puente II cambia de fondo a mitad (`N3_PuenteII` → `N3_PuenteII_Rio`). *(Vencido el
25/09/2026: desde el 23/09 son siete, `N3_PuenteII` → `N3_PuenteII_Horizonte` → `N3_PuenteII_Rio`;
ver §A.3.)* Los 32 encuadres son
los del diseño de cámara del N3 (perdido en disco; sobrevive en los assets).
`ConditionalNarrativeTrigger` (C# plano, `Game.Scaffolding`) dispara la 3.2 una sola vez.
`IllustrationFraming.Warnings` recibe la proporción del sprite: con 16:9 sin duplicar el rango en
x es `0.5/z`.

### Fase 2 — Recolección (R05–R08 · 17/09 y 20/09/2026)

`RiverTask`/`TaskList` (cuatro tareas, **sin forma de desmarcar**, la base no marca tarea por sí
sola — INC-30), `Collectible`/`Inventory` (posiciones en fracciones de la ilustración, capacidad =
catálogo, cuenta por clase), `RiverWalk` (posición y límites en fracciones), `DirectionPad` (una
flecha = un botón uGUI con clic sostenido), `BuildZone` (`TryEnter` nombra lo que falta, nunca
cuántos) y `RiverSceneController` sobre la disposición del mockup 11-12. La escena `Level3_River`
se construyó por script de editor efímero y entró a Build Settings como undécima.

### Fase 3 — Ensamblaje y depuración (R09–R12 · 20/09/2026, commit `3fe1e2a`)

`RaftAssembly` (solo se ven los espacios de la fase activa y las consolidadas; `Confirm`
consolida o devuelve **solo** lo mal puesto; `Resume` para RNF-14), `RaftValidator` (función pura:
vacío o con otra pieza), `RaftAssemblyContent` (espacios, arte por clase, encuadre, frases),
`AssemblyPanelController` (empuje de cámara, pulso de completado y hundimiento por giro
continuos —RNF-21—, «Listo» → «Probar balsa», confirma y guarda con `PhaseId` *(el orden de
guardado cambió el 25/09/2026; ver §A.4)*), `InventoryView`
y la escena 3.2 cableada tras el hundimiento con las piezas de la fase abierta en **memoria de
nivel** (estáticos del assembly: sobreviven a la recarga de la escena narrativa, no al proceso —
RNF-09 no admite «escenas vistas»).

### Fase 4 — Cierre del nivel y del juego (R13–R16 · 21/09/2026, commit `1179dae`)

- **R13** `RiverIndicatorCollector` (`ILevelReporter`): rechazo = intento; aceptación = paso;
  error corregido = pieza devuelta cuyo espacio ya no aparece señalado; `PauseOpened/Closed` para
  la pausa y la escena 3.2; retoma desde disco sembrando las fases confirmadas como pasos.
- **R14** `GameFlow` acepta `Narrative → Credits`; `N3_ResumenNivel.asset` (nuevo, sin cifras,
  nombra descomponer y depurar) sale a `N3_EscenaFinal`, que sale a los créditos; la balsa de
  `N3_Escena33_Cruce` cruza con `PropMotion.Drift`. `env_final_fogatas.png` ya era la
  ilustración de la escena final: ningún encuadre cambió.
- **R15** Los tres estados de error del N3 se leen sin color, la lista de tareas en escala de
  grises, contraste ≥ 6,3:1 medido en escena y ninguna animación con destellos.
- **R16** Cierre de RNF-02 y RNF-16 con pruebas que leen disco (§5.4) y una PlayMode que carga
  las cinco escenas jugables.

### Checkpoint R-E (21/09/2026)

`RiverLevelJourneyTests`: los dos recorridos completos del N3 desde `Boot` con perfil real —uno
acertando la prueba al primer intento, otro fallándola, viendo la 3.2 y corrigiendo—, el cierre
forzado tras la base y tras el amarre con retoma en la fase pendiente, y las dos medidas de
RNF-04/05. `unity build` para RNF-06. Detalle en §6.

---

## 5. Hallazgos reales encontrados al verificar

Ninguno salió de leer el código: los encontró una prueba o una corrida.

1. **La casilla del inventario no era objetivo de raycast** (R11, 20/09): el arrastre no arrancaba
   jugando aunque las pruebas que invocaban `Take` pasaban. Corregido y cubierto por
   `AssemblyPanel_RNF02_LaCasillaDelInventarioRecibeElClicSostenidoPorRaycastYSueltaEnElEspacio`,
   que reproduce el camino real del clic.
2. **Entrar a la zona sin los materiales se mostraba con tono `Help`** (R15, 21/09), cuyo icono
   está vacío en la escena: solo palabras. Es una acción rechazada (CU-09 FA-6a), no una pista:
   `RiverSceneController.Enter` pasa a `MessageTone.Rejected`; la frase no cambia (CP-02).
3. **Falso hallazgo de contraste** (R15): el botón de confirmar medía 3,9:1 porque se deshabilita
   en cada animación y vuelve con un `CrossFade` de 0,1 s; el corredor capturaba a 3 ms del
   desvanecido. La prueba espera el desvanecido antes de medir. Documentado en la prueba.
4. **Nueve escenas usaban el `DefaultInputActions` del paquete Input System** (R16, 21/09) —las
   cuatro jugables del N2 y N3 y las cinco de flujo con módulo—, con teclado y mando, y el mapa
   de acciones del proyecto era la plantilla de `Assets/Settings/` con 23 vinculaciones de
   teclado. Solo `Level1_Cave` estaba bien. Las diez con módulo y el proyecto apuntan ahora a
   `ControlesJugables`, al que se le retiró la rueda del ratón. Cuatro pruebas de arquitectura
   impiden que vuelva a pasar.
5. **`IllustrationFraming.Warnings` avisaba mal con 16:9 sin duplicar** (R04, 16/09): el rango
   en x es `0.5/z`, no `0.25/z`; ahora recibe la proporción del sprite.
6. **Batchmode abre la Game View a 640×480 y sin captura**: trece pruebas de disposición o de
   captura (`AssemblyPanel_RNF03/HU12`, `RiverScene_RF35_…Limites`, `RiverScene_RNF03`,
   `RiverLevel_RNF19/RNF20`, `RiverScene_RF36/RF39`, `NarrativeScene_RNF01`,
   `ForestScene_RF26`, `MazeScene_RNF03`, `WorkshopScene_RNF03`) fallan ahí y pasan con Rider y
   la Game View fijada a 1920×1080 (`PlayModeWindow.SetCustomRenderingResolution`, 20/09). Es el
   entorno, no el código; queda declarado en vez de ignorado.

---

## 6. Verificación declarada

**Cómo se corrió.** Rider conectó el 20/09 y la mañana del 21/09 (corridas contra el Editor
abierto con la Game View a 1920×1080); desde el mediodía del 21/09 la sesión arrancó con
`ConnectionRefused` y todo lo demás fue `unity test` con el Editor cerrado. Ni el silencio de un
MCP ni un `ConnectionRefused` son que la suite pase: todas las cifras salen de un XML real.

| Corrida | Fecha | Resultado |
|---|---|---|
| EditMode, suite completa | 21/09/2026 | **282/282** |
| PlayMode `Game.Levels.River.PlayMode.Tests` (Rider, 1920×1080) | 21/09/2026 13:05 | **24/24** |
| PlayMode `RiverLevelJourneyTests` (Checkpoint R-E) | 21/09/2026 | **6/6** en 37 s |
| PlayMode `Core` + todas las `RNF02`/`INC01` de los cinco niveles | 21/09/2026 | **17/17** |
| PlayMode, suite completa en batchmode | 21/09/2026 | **165/184**: 6 omitidos (visuales sin Game View) y los 13 de §5.6 |

**Presupuestos medidos, no estimados** (RNF-04, RNF-05, RNF-06):

| Presupuesto | Límite | Medida | Cómo |
|---|---|---|---|
| Carga de `Level3_River` | < 10 s | **1,36 s** | `SceneLoader.LastLoadSeconds`, `RiverLevel_RNF04_…` |
| Memoria con el N3 cargado y el panel abierto | < 2 GB | **1 226 MB** reservados, 785 MB asignados | `Profiler.GetTotalReservedMemoryLong`, `RiverLevel_RNF05_…` (Editor: cota superior del ejecutable) |
| Paquete portable, once escenas | < 500 MB | **217 MB** | `unity build --target StandaloneWindows64`; `Algoritmia_Data` 150 MB, `UnityPlayer.dll` 36 MB, `DirectML.dll` 14 MB. Incluye `My project_BurstDebugInformation_DoNotShip` (1 MB), que se retira del entregable a mano |

El ejecutable **no se corrió** en esta sesión: que `Datos/` nazca junto al `.exe` (RNF-07,
RNF-11) y el Golden Path cronometrado son de Santiago (§8).

**Casos declarados por archivo de prueba:**

| EditMode | Casos | PlayMode | Casos |
|---|---|---|---|
| `InventoryTests` | 8 | `AssemblyPanelTests` | 6 |
| `RiverIndicatorTests` | 8 | `RiverLevelJourneyTests` | 6 |
| `RaftAssemblyTests` | 7 | `RiverMovementTests` | 6 |
| `RaftValidatorTests` | 6 | `RiverAccessibilityTests` | 4 |
| `RiverLevelConfigTests` | 5 | `BuildZoneTests` | 3 |
| `TaskListTests` | 5 | `ConditionalNarrativeTests` | 3 |
| | | `GameEndingTests` | 2 |
| | | `RiverSceneVisualTests` | 2 |

*(Al 25/09/2026: `AssemblyPanelTests` 8, `RiverMovementTests` 8, `BuildZoneTests` 5, y dos archivos
nuevos, `RiverSoundsTests` (6, PlayMode) y `RiverSoundsAssetTests` (2, EditMode); ver §A.5.)*

Fuera del módulo, el slice añadió o amplió `AssemblyDependencyTest` (+1), `InputSchemeTest` (3,
nuevo), `ControlSchemeTests` (1, nuevo, en `Core`), `GameFlowTests` (+1), `NarrativeSequenceTests`,
`NarrativeVisitPolicyTests`, `HintPolicyTests` (+4), `LevelSummaryComposerTests` (+2),
`SaveStoreTests` y `PlayerProfileTests`.

**Verificación visual.** Capturas revisadas en `%AppData%\LocalLow\DefaultCompany\My project\TestScreenshots\`:
`RiverScene_RF36_OrillaAlAbrir`, `RiverScene_RF39_ZonaAbierta`, las tres `AssemblyPanel_HU12_*`,
las cuatro `RiverLevel_RNF19_*` (una desaturada por la propia prueba) y las tres `RiverLevel_RNF21_*`.

---

## 7. Inventario de archivos — qué es cada cosa

Según `graphify god-nodes` (foto del 20/09/2026), tres de los ocho hubs más conectados del
proyecto son de este slice: `AssemblyPanelController` (74 aristas), `RiverSceneController` (67) y
`BuildZone` como hub de su comunidad.

### 7.1 `Assets/Game/Scripts/Runtime/Levels/River/` — el módulo del nivel (23 archivos, 2 486 líneas)

*(Vencido el 25/09/2026: hoy 24 archivos y 2 750 líneas —entró `RiverSounds.cs`— y
`AssemblyPanelController.cs` tiene 784 líneas, `RiverSceneController.cs` 542 y
`RiverLevelConfig.cs` 199; ver §A.5.)*

| Archivo | Líneas | Qué es |
|---|---|---|
| `AssemblyPanelController.cs` | 678 | Adaptador del ensamblaje: espacios, arrastre por asas, empuje de cámara, animaciones, confirmación y guardado por fase, memoria de nivel, salida al cruce o a la 3.2 |
| `RiverSceneController.cs` | 442 | Adaptador de la orilla: flechas, recolección, lista, inventario, zona, retoma por fase, tono de los mensajes |
| `RiverLevelConfig.cs` | 197 | SO: materiales y posiciones en fracciones, orilla andable, `GroundTop`, perspectiva, radios, velocidad, textos (CT-05) |
| `RaftAssemblyContent.cs` | 189 | SO: los diecisiete espacios, arte y silueta por clase, encuadre del ensamblaje, frases, ids de las secuencias de salida |
| `RaftAssembly.cs` | 176 | C# plano: tres fases bloqueantes, `Place`/`TakeBack`/`Confirm`/`Resume`, rechazos solo para RF-45 |
| `RiverIndicatorCollector.cs` | 116 | `ILevelReporter` del nivel: intentos, pasos, errores corregidos, tiempo por fase con pausa |
| `InventoryView.cs` | 98 | Casilla por clase con marcas para los troncos |
| `Collectible.cs` · `Inventory.cs` | 74 · 70 | Material recogible con alcance; inventario por clase, recoger no falla |
| `RaftSlot.cs` · `RaftPhase.cs` | 60 · 13 | Un espacio de la balsa (fase, clase, posición, giro) y las tres fases |
| `BuildZone.cs` · `RiverWalk.cs` | 56 · 45 | Zona por radio que nombra lo que falta; paseo con límites en fracciones |
| `RiverTask.cs` · `TaskList.cs` | 46 · 38 | Las cuatro tareas y su regla de marcado; estado sin forma de desmarcar |
| `DirectionPad.cs` · `RaftPieceHandle.cs` · `PointerTracker.cs` | 38 · 31 · 27 | Las asas de clic sostenido: flecha, pieza/espacio y seguimiento del cursor sin Input System |
| `RaftValidator.cs` · `ValidationResult.cs` · `Outcome.cs` · `MessageTone.cs` | 24 · 29 · 22 · 13 | La prueba de la balsa como función pura y sus resultados; el tono con icono de cada mensaje |
| `AssemblyInfo.cs` | 4 | `InternalsVisibleTo` para EditMode y PlayMode |

### 7.2 Pruebas — `Assets/Tests/{EditMode,PlayMode}/Levels/River/` (14 archivos, 2 642 líneas)

*(Vencido el 25/09/2026: hoy 16 archivos y 3 070 líneas; ver §A.5.)*

Las de la tabla de §6. `AssemblyPanelTests` y `RiverMovementTests` exponen utilería `internal`
(`OpenAssembly`, `FillPhase`, `Drag`, `Confirm`, `WaitIdle`, `OpenRiver`) que reutilizan
`ConditionalNarrativeTests`, `GameEndingTests` y `RiverLevelJourneyTests`.

### 7.3 Contenido — lo que se ajusta sin recompilar (CT-05, RNF-18)

| Asset | Qué fija |
|---|---|
| `Data/River/N3_RiverLevelConfig.asset` | Orilla, materiales, radios, velocidad, `GroundTop`, textos de tareas y mensajes |
| `Data/River/N3_RaftAssemblyContent.asset` | Espacios, arte por clase, encuadre `(0.50, 0.23) ×2.2`, frases, `FirstFailureSequenceId`, `ClosingSequenceId`, `SinkSeconds` |
| `Data/River/N3_ResumenNivel.asset` | Resumen del nivel sin cifras, habilidad nombrada, `ClosingSequenceId = N3_EscenaFinal` |
| `Data/Guide/N3_Guia.asset` | Los cuatro pasos de ayuda y pista |
| `Data/Narrative/N3_PuenteII` · `_PuenteII_Rio` · `_Escena31_Llegada` · `_Escena32_PrimerIntento` · `_Escena33_Cruce` · `_EscenaFinal` | Las cinco escenas en seis assets, con sus 32 encuadres, objetos, y las marcas de cierre reflexivo y final *(Vencido el 25/09/2026: siete con `N3_PuenteII_Horizonte`, y se sumó `Data/River/N3_Sonidos.asset`; ver §A.3.)* |
| `Input/ControlesJugables.inputactions` | El único mapa de controles del juego: puntero, clic, táctil y lápiz |

### 7.4 Escenas y arte

`Scenes/Level3_River.unity` (undécima del build). Entornos finales `env_n3_rio.png` y
`env_final_fogatas.png` (D06); props **provisionales** del río (`prop_n3_troncos/_tronco/_sogas/
_tela/_mastil/_vela/_amarre` y sus siluetas, `_balsa_hundida`, `_balsa_cruzando`),
`env_n3_zona_disponible`, `ui_n3_casilla_hecha` y `char_mama_cenital` (una postura). El tablero
de arte C1–C10 sigue abierto (§8). *(Vencido el 25/09/2026: desde `88fe0ee` (24/09) la `Image`
de `char_mama_cenital` en `Personaje_Mama` está deshabilitada y Mamá es el rig `Rig_Mama`; ver
§A.3 y `Personajes-Resultados.md`.)*

### 7.5 Piezas compartidas que este slice tocó

| Pieza | Qué cambió | Con qué prueba |
|---|---|---|
| `GameFlow` (`Core`) | `Narrative → Credits` | `GameFlow_INC39_…` |
| `GameFlowRunner` (`Core`) | `PlayingScenes` con `River 2` y `River 3` | recorridos |
| `NarrativeSequence` · `NarrativeProp` · `NarrativeSceneController` | `EndsInCredits`, `PropMotion.Drift`, salida a créditos | `NarrativeVisitPolicy_CP07_…`, `GameEnding_RF44_…` |
| `IllustrationFraming` (`Scaffolding`) | `Warnings` con proporción del sprite | `IllustrationFramingTests` |
| `ConditionalNarrativeTrigger` (`Scaffolding`, nuevo) | La escena condicional | `ConditionalNarrativeTriggerTests` |
| `LevelSummaryMessages` · `LevelSummaryController` (`UI`) | `ClosingSequenceId` | `LevelSummary_RF12_…`, `GameEnding_INC39_…` |
| Nueve escenas + `EditorBuildSettings` | Módulo de entrada y mapa del proyecto → `ControlesJugables` | `InputSchemeTest`, `ControlSchemeTests` |
| `Game.Core.PlayMode.Tests.asmdef` | Referencia `Unity.InputSystem` y `UnityEngine.UI` | — |

### 7.6 Documentos del slice

`plan.md`, `todo.md`, este documento, `docs/actas/OE3/Acta_D07_2026-09-19.md` y las
entradas de `CLAUDE.md` que declaran lo cruzado (menú de pausa en cinco escenas,
`NarrativeVisitPolicy`, `GameFlow`, `PhasesPerLevel`, el N3 sin Input System).

---

## 8. Lo que queda abierto

**Del Checkpoint R-E**, todo lo que exige a una persona:

- [ ] **Golden Path del juego entero en 20–40 minutos**: los tramos están automatizados
      (`LevelSummaryTests`, `WheelLevelJourneyTests`, `RiverLevelJourneyTests`), el cronómetro lo
      lleva quien juega.
- [ ] **PG-05** (el cambio de esquema de control no confunde): observación con estudiantes.
- [ ] Correr el **ejecutable** de `Build/Algoritmia/` y ver nacer `Datos/` (RNF-07, RNF-11);
      retirar `My project_BurstDebugInformation_DoNotShip` del entregable.
- [ ] Repetir en el Editor con Rider (Game View 1920×1080) los **trece** de §5.6.
- [ ] Revisión con el usuario, pendiente en R-A..R-E: radio de proximidad (0,04), encuadre del
      ensamblaje jugando, lectura de las pistas de `N3_Guia` (CP-06).

**Decisiones de Santiago:**

- `Assets/Settings/InputSystem_Actions.inputactions` (la plantilla con teclado) ya no la usa
  nada: borrarla o no.
- Rehacer los **inventarios de cámara** de los tres niveles, perdidos el 20/09/2026 y no
  versionados: hoy la única fuente de lo aplicado son los `N*_*.asset`. Las actas ya se
  restablecieron (`docs/actas/OE3/`, `D01`..`D07`).
- **La build activó cosas que no pedimos:** `unity build` dejó `SENTIS_ANALYTICS_ENABLED` en los
  símbolos de `Standalone` y `UnityConnectSettings` en `m_Enabled: 1` (revertidos con `git checkout`
  antes de commitear), y el paquete lleva `DirectML.dll` (14 MB) y `D3D12/`: son los paquetes de IA
  del Editor (`com.unity.ai.*`, Sentis) entrando al ejecutable. Con RNF-08/RNF-10 (ningún dato por
  red) sobre la mesa, quitarlos de `Packages/manifest.json` es «preguntar primero»: decisión de
  Santiago antes de la entrega.
- **PG-01** título del producto y **PG-02** nombre del guía (INC-44 fija «Algoritm» para todo
  asset nuevo; falta el cierre formal en el documento).

**Inconsistencias abiertas** (`claudeDocs/INCONSISTENCIAS.md`): **INC-46** (la lista de tareas
del N3 está especificada dos veces y distinto), INC-47, INC-48, INC-49, INC-50. *(Al 25/09/2026
se suman INC-51 a INC-54, las cuatro abiertas; ver §A.2.)*

**Arte:** las diez piezas C1–C10 del tablero de arte siguen provisionales; los personajes sobre
las escenas narrativas del N3, como en el N2, esperan sprite. *(Vencido el 25/09/2026: los
personajes entraron con `88fe0ee` (24/09); ver `Personajes-Resultados.md`.)* **Sonido:** D07-2,
de Valdiri; `Game.Audio` no se toca desde este carril. *(Vencido el 25/09/2026: desde el acta D08
(23/09) el sonido restante es de Santiago Benavides Rey, y con `ccf77e6` `Game.Levels.River`
referencia `Game.Audio`; ver §A.6.)*

**Siguiente incremento:** Slice 4 — progreso, informe docente (RF-46) y borrado de datos (RF-47);
`Game.Reporting` todavía no existe en disco. *(Vencido el 25/09/2026: existe desde `9c34924`
(24/09); ver `../Slice 4/Slice-4-Resultados.md`.)*

*(01/10/2026: de esta sección quedan vencidos varios puntos. INC-46 a INC-54 se cerraron el 29/09 y
PG-01 y PG-02 están cerrados; la analítica, la pantalla de Unity y Alt+Intro se apagaron en
`37b3cb7` sin retirar ningún paquete (INC-97, decisión D3 del 29/09); el radio de proximidad y el
encuadre del ensamblaje los rehízo la lectura B (B.8); y las revisiones con el usuario las resuelve
la decisión D-a del acta D10. El estado de cada punto está en el Anexo B, B.10.)*

---

## 9. Cómo se reproduce

```bash
# Suites completas — exigen el Editor de Unity cerrado
unity test --mode EditMode --output test-results.xml --timeout 900 --no-banner --non-interactive
unity test --mode PlayMode --output play-results.xml --timeout 1800 --no-banner --non-interactive

# Solo el Nivel 3 y el cierre (--filter es una expresión regular contra namespace.Clase.Método)
unity test --mode EditMode --filter "Game\.Levels\.River\.Tests"
unity test --mode PlayMode  --filter "RiverLevelJourneyTests|GameEndingTests"
unity test --mode PlayMode  --filter "Game\.Core\.Tests|RNF02|INC01"   # RNF-02 en los cinco niveles

# El paquete portable (RNF-04/05/06 se miden sobre Build/Algoritmia/)
unity build --target StandaloneWindows64 -o "Build/Algoritmia/Algoritmia.exe" --log-file build.log --no-banner --non-interactive
```

Los trece de disposición y captura de §5.6 se corren desde Rider con la Game View a 1920×1080;
en batchmode fallan por el entorno. Con el Editor abierto, `unity test` falla con
`another Unity instance is running`.

---

## Anexo — lo que cambió después del cierre (25/09/2026)

Lo de arriba es el registro del 21/09 y se conserva. Este anexo recoge un commit de la propia
rama que ese registro no describe, `d81cfc7`, y lo que llegó al Nivel 3 y al cierre del juego
entre el 23 y el 25/09. Todo se comprobó contra el código, los assets y el historial en `ccf77e6`
(25/09/2026).

### A.1 `d81cfc7` (17/09): lo que la rama trajo y §4 no cuenta

`d81cfc7` («rediseño de pausa, scroll de laberinto y correcciones en narrativa y entorno») cae
entre R03/R04 (`e3575bf`) y R05–R08 (`3894426`). Está entre los siete commits de la cabecera, pero
ninguna sección lo describe. Dos de sus partes tocan piezas que comparten los tres niveles:

- **La regla de «Omitir»** (`NarrativeVisitPolicy`, `Game.Scaffolding`). Antes, fuera del cierre
  reflexivo, «ya vista» era haber confirmado **alguna** fase del nivel. Por eso, al salir del
  bosque con la fase 1 recién confirmada, la 2.2 ofrecía omitir aunque se veía por primera vez.
  Desde `d81cfc7` es haber confirmado la **última** fase del nivel (`PhaseId.PhaseCountOf`,
  `NarrativeVisitPolicy.cs:53-54`): solo puede saltar quien ya terminó el nivel, y como lo
  aprobado no se pierde (RF-41), lo sigue pudiendo en las vueltas siguientes. El cierre reflexivo
  conserva su propia señal, `ReachedLevel > sequence.Level` (`:43-46`), porque se llega a él justo
  después de confirmar la última fase. En el N3, ninguna escena ofrece omitir hasta que la fase 3
  (mástil y vela) está confirmada, y el cruce y la escena final no lo ofrecen nunca (§A.2). Lo
  vigila `NarrativeVisitPolicy_RF06_UnaEscenaIntermediaNoSeOmiteEnLaPrimeraVuelta`, que es nueva
  en el commit (`NarrativeVisitPolicyTests` tiene hoy 3 pruebas).
- **El menú de pausa** (`Assets/Game/Prefabs/UI/MenuPausa.prefab`). Desde W17 (`8de614f`), el
  prefab solo cambió en este commit y `PauseMenuController` no se tocó: el cambio es de aspecto,
  no de conducta.
  - Los siete textos dejan la fuente integrada: seis pasan a Baloo 2 Bold y el de la confirmación
    de reinicio a Nunito Regular (`Art/Fonts/`).
  - Los botones llevan glifos nuevos, `ui_reanudar`, `ui_reiniciar` y `ui_pausa`
    (`Art/UI/Common/`).
  - Cada botón con texto ajusta su ancho a él (`HorizontalLayoutGroup` y `ContentSizeFitter` al
    tamaño preferido).

  El mensaje del commit cuenta el velo carbón al 72 % (`#3A1E18`) como parte del rediseño, pero
  ya estaba en W17. Las cinco escenas jugables usan el mismo prefab, `Level3_River` incluida.

En el mismo commit entró `ui_flecha`, el glifo de las cuatro flechas de la orilla desde R05–R08.
El resto de `d81cfc7` es del Nivel 2 y va con los resultados del Slice 2: los botones ▲/▼ de la
secuencia del laberinto, sin `ScrollRect`; la caja y los troncos del bosque anclados en fracciones
(`CargoPlacedPosition`), y el sprite de la piedra de la 2.4.

### A.2 INC-51 y las inconsistencias abiertas

La decisión 6 (§3) dice que la marca de cierre reflexivo le niega «Omitir» a `N3_EscenaFinal`
«la primera vez». En realidad se lo niega siempre. Para un cierre reflexivo, «ya visto» es
`ReachedLevel > sequence.Level`, y como `LevelId` termina en `River = 3`, en el N3 eso nunca se
cumple. `N3_Escena33_Cruce` y `N3_EscenaFinal`, las dos con `IsReflectiveClosing`, se leen enteras
también cuando el estudiante repite el nivel, porque la primera vuelta y las siguientes dejan el
mismo perfil en disco. Santiago lo aceptó el 23/09 como **INC-51**, que sigue abierto hasta
corregir HU-14 FA-01, donde se pide el botón para quien repite. El comentario de
`NarrativeVisitPolicy.cs:39-42` lo registra desde `13e2986` (23/09).
`NarrativeVisitPolicy_CP07_ElCruceYLaEscenaFinalNoSeOmitenLaPrimeraVez` ya prueba con ese perfil
—todas las fases del N3 confirmadas y el Nivel 3 alcanzado—, que es el mismo en cualquier vuelta.

*(Nota 29/09/2026: INC-51 se cerró corrigiendo HU-14 FA-01, su tabla de datos de entrada y CU-03 2a para que digan lo que hace el juego; el código no cambió.)*

**Inconsistencias abiertas al 25/09/2026** (tabla de `INCONSISTENCIAS.md`): de INC-46 a INC-50,
como en §8, y además:

| INC | Qué | Estado |
|---|---|---|
| INC-51 | El cierre del N3 nunca ofrece omitir; HU-14 FA-01 lo pide a quien repite | Abierto; decisión tomada: se acepta (23/09) |
| INC-52 | El Algoritm entregado es una llama con brazos y piernas, no la estrella de §7.6 | Abierto; se usa el arte entregado (24/09) |
| INC-53 | Los personajes se animan por recorte en uGUI, no con el paquete 2D Animation | Abierto; decisión técnica (24/09) |
| INC-54 | El taller del N2 gana una séptima pieza, la cuerda | Abierto; decisión de Santiago (25/09) |

### A.3 El Nivel 3 después del 21/09

| Fecha | Commit | Qué cambió | Detalle en |
|---|---|---|---|
| 23/09 | `03675b9` | El puente II arranca en el refugio con fuego: `N3_PuenteII` pasa de `env_n2_bosque_claro` a `entorno_n1_cueva_2x`. Encadena al nuevo `N3_PuenteII_Horizonte` (sobre `env_enlace_n2`), que encadena a `N3_PuenteII_Rio`, así que ahora son siete assets. Los dos primeros suenan a cueva y hoguera (`amb_n1_cueva_oscura` y `amb_n1_cueva_fuego`). La prueba pasa a llamarse `NarrativeSequence_RF05_ElNivel3TieneSusSieteSecuencias` | `Props-y-Sonidos-Resultados.md` §1 y §4 |
| 24/09 | `88fe0ee` | Personajes animados en las siete narrativas del río y en `Level3_River`. Mamá es `Rig_Mama`, dentro de `Personaje_Mama`, y la `Image` de `char_mama_cenital` queda deshabilitada. La familia está junto a la zona y el botón de ayuda muestra a Algoritm en su forma de gota. Seis pruebas nuevas en el módulo (§A.5) | `../Personajes/Personajes-Resultados.md` |
| 24/09 | `f801186` | Humo en bucle (`fx_n1_humo`) sobre los fuegos de `N3_PuenteII`, `N3_PuenteII_Horizonte` y `N3_EscenaFinal`, vigilado por `NarrativeSequence_RF05_CadaLlamaEchaHumoPorDetrasYPorEncima` | `Props-y-Sonidos-Resultados.md` |
| 25/09 | `d3a6cc9` | La hoguera central del horizonte deja de estar pintada (`env_enlace_n2` se entrega sin ella) y pasa a ser objetos: montón de hojas, humo y llama animada. La misma hoguera entra en `N3_EscenaFinal` | `Props-y-Sonidos-Resultados.md` §4 |
| 25/09 | `ccf77e6` | Sonido del N3 (§A.6), orden de guardado del ensamblaje (§A.4) y plano de la recolección a `(0.2632, 0.2632) ×1.9` (decisión 4) | `Props-y-Sonidos-Resultados.md`, Anexo §A.2.1 (y §A.4, §A.7, §A.8); `Direccion_de_Musica_y_Sonido.md` §19 |

### A.4 Cuándo se guarda cada fase del ensamblaje (`ccf77e6`, 25/09)

Hasta el 21/09, `ConfirmedAsync` esperaba a que terminara el pulso de completado (0,35 s). Solo
entonces cerraba los indicadores (`_indicators.Complete()`, dentro de `Persist`) y guardaba la
fase. Desde `ccf77e6`:

1. **El reloj de RF-45 se para al confirmar**, no al acabar la animación: `_indicators.Complete()`
   se llama al entrar en `ConfirmedAsync`, antes del pulso (`AssemblyPanelController.cs:394-396`).
2. **Las fases 1 y 2 se guardan al aprobarse**, antes del pulso y de los martillazos (`:398-404`).
   Así, una pausa con «Reiniciar» a mitad de la animación no puede perder lo aprobado ni contar el
   paso dos veces al repetirlo (RF-41, RF-45).
3. **La fase 3 se guarda al salir al cruce**, después del último martillazo y del sonido de la
   balsa terminada (`:435-441`). El comentario de `:436-438` lo justifica por CP-07, para que el
   cruce no cuente como visto, aunque por INC-51 el cruce nunca cuenta como visto. En la práctica,
   lo que cambia es que la fase 3 no queda en disco hasta ese momento.
4. **El panel queda bloqueado hasta el último golpe.** `PulseAndHammerAsync` (`:467`) lleva el
   pulso y los `PhaseHits` martillazos con el mismo reloj, e `IsBusy` sigue activo hasta que
   termina. Así, la pieza siguiente no se clava encima de los golpes.

El resto de la decisión 5 no cambió: hay un solo recolector para las tres fases, y el reloj de la
base arranca al abrir el ensamblaje (`:190-194`).

### A.5 Cifras del módulo al 25/09/2026

Líneas contadas con `wc -l` y casos con `grep` de `[Test]`/`[TestCase]` sobre `ccf77e6`.

| Qué | 21/09 (este documento) | 25/09 | Qué lo movió |
|---|---|---|---|
| Código de `Game.Levels.River` | 23 archivos, 2 486 líneas | 24 archivos, 2 750 líneas | `RiverSounds.cs` (56, nuevo, `ccf77e6`). `AssemblyPanelController.cs` pasa de 678 a 784 líneas y `RiverSceneController.cs` de 442 a 542 (`88fe0ee` y `ccf77e6`); `RiverLevelConfig.cs`, de 197 a 199 |
| Pruebas del módulo | 14 archivos, 2 642 líneas | 16 archivos, 3 070 líneas | `RiverSoundsTests` y `RiverSoundsAssetTests` |
| Casos EditMode / PlayMode | 39 / 32 | 41 / 44 | Las filas siguientes |
| `AssemblyPanelTests` | 6 | 8 | `AssemblyPanel_DA133_AlAprobarUnaFaseMamaYLaFamiliaCelebran` y `AssemblyPanel_DA133_CuandoLaBalsaSeHundeLaFamiliaAnimaYNingunoHaceOtroGesto` (`88fe0ee`) |
| `BuildZoneTests` | 3 | 5 | `RiverScene_DA133_MamaRecogeAlPulsarRecogerYVuelveSolaAlReposo` y `BuildZone_DA133_EntrarSinTodoLosMaterialesDaAnimoYNingunGestoDeDerrota` (`88fe0ee`) |
| `RiverMovementTests` | 6 | 8 | `RiverScene_DA133_MamaCaminaMientrasSeSostieneUnaFlechaYReposaAlSoltarla` y `RiverScene_DA76_ElBotonDeAyudaMuestraAAlgoritmEnSuFormaDeGota` (`88fe0ee`) |
| `RiverSoundsTests` (PlayMode, nuevo) | — | 6 | `ccf77e6`: la orilla suena, recoger suena una vez, un martillazo por pieza, tres al aprobar, `AssemblyPanel_CP02_UnaFaseQueNoPasaNoSuena` y la balsa terminada |
| `RiverSoundsAssetTests` (EditMode, nuevo) | — | 2 | `ccf77e6`: `RiverSounds_RF05_…` y `RiverSounds_RF44_…` |
| `RiverLevelConfigTests` | 5 | 5 | `RiverLevelConfig_Guion82_ElPlanoDeRecoleccionEsElBosqueConUnPocoDelRio`, rehecha para el plano ×1.9 |

Fuera del módulo, desde el 21/09 entraron, entre otras, estas pruebas que tocan el N3:

- en `NarrativeSceneTests` (`88fe0ee`), `NarrativeScene_RF05_CadaPersonajeHaceLoQueDiceSuPasoCuandoSeLeeLaLinea`
  y `NarrativeScene_RF05_CapturaCadaLineaConLosPersonajes`, con siete `[TestCase]` cada una sobre
  los siete `N3_*` (`Assets/Tests/PlayMode/UI/NarrativeSceneTests.cs:919-925` y `:986-992`), y
  `NarrativeScene_RF05_QuienCaminaTerminaSuCaminoAunqueElTextoAvance`, que abre
  `N3_Escena33_Cruce` (`:1033`);
- `NarrativeScene_RF44_LaBalsaSuenaMientrasCruzaYAlLlegarVuelveElBosque` (`NarrativeSceneTests`,
  `ccf77e6`);
- `NarrativeSequence_RF05_CadaLlamaEchaHumoPorDetrasYPorEncima` (`f801186`).

Además, `…ElNivel3TieneSusSeisSecuencias` pasó a `…SieteSecuencias` (`03675b9`). La prueba de
«Omitir» de §A.1 es anterior al cierre; §6 nombra su archivo, `NarrativeVisitPolicyTests`, pero no
la prueba. Este anexo no repite
cifras de suite: la corrida completa de EditMode y PlayMode del 25/09/2026 está en
[`../Slice 4/Slice-4-Resultados.md`](../Slice%204/Slice-4-Resultados.md), «Corrida completa de la
suite (25/09/2026)».

### A.6 El sonido del N3 y quién lo lleva

Desde el acta D08 (23/09), el sonido restante es de Santiago Benavides Rey: D07-2 cambia de
responsable y D08-2 le encarga los sprites y sonidos que se reciban. `ccf77e6` (25/09) cablea el
sonido del Nivel 3, con piezas globales en lugar de las de §13 de la dirección de sonido, que aún no
están entregadas (`sfx_n3_salpicadura_undimiento.wav` está en disco y no lo referencia nada):

- **Dónde está.** `Data/River/N3_Sonidos.asset` es la instancia de `RiverSounds`
  (`Game.Levels.River`, nuevo), y en `Level3_River` lo referencian la orilla (`RiverSceneController`) y el panel
  (`AssemblyPanelController`).
- **Qué suena.** El río (`amb_n3_rio_orilla`) está de fondo, con el bosque de día en la segunda
  capa. Recoger suena a encaje. Cada pieza puesta suena a un martillazo, también la mal puesta,
  porque colocar no valida. Aprobar una fase suena a tres martillazos, y la balsa terminada suena
  (`RaftBuilt`) tras el último de la fase 3. Nada suena al fallar (CP-02).
- **Las narrativas del río.** `NarrativeProp.MotionAmbient` hace sonar la balsa de
  `N3_Escena33_Cruce` (`amb_balsa_movimiento`) mientras cruza. Las cuatro escenas del río
  (`N3_PuenteII_Rio`, 3.1, 3.2 y 3.3) piden el mismo fondo que la orilla, y `N3_EscenaFinal` no
  tiene ambiente.

Para eso, `Game.Levels.River` referencia `Game.Audio`, y `Game.Audio` abre sus internos a
`Game.Levels.River.PlayMode.Tests`. El detalle archivo por archivo está en
`Props-y-Sonidos-Resultados.md`, Anexo §A.2.1 (y §A.4, §A.7, §A.8), y lo aplicado, en
`Direccion_de_Musica_y_Sonido.md` §19.

### A.7 Módulo y herramientas

- **Referencias de `Game.Levels.River.asmdef`.** La Fase 0 dejó solo `Game.Core`. R05–R08
  (`3894426`, 17/09) sumó `Game.Scaffolding` y `UnityEngine.UI`, y `ccf77e6` sumó `Game.Audio`.
  Sigue sin `Unity.InputSystem`, y lo vigila
  `RiverScene_INC01_NoExisteVinculacionDeTecladoEnElMapaDeControles`.
- **`ArtImportRules`** (`a9236f1`, 16/09, el primer commit de la rama) es un `AssetPostprocessor`
  de `Game.EditorTools`. Importa todo `Assets/Game/Art/` sin comprimir y con `maxTextureSize` 4096
  (RNF-23), porque con compresión la ilustración plana muestra la rejilla de bloques de 4×4. Lo
  vigila `ArtImport_RNF23_LasIlustracionesEntranSinComprimirYSinReducir` (`ArtImportTest`, en
  `Game.Architecture.Tests`).

---

## Anexo B — lo que cambió después del 25/09/2026 (01/10/2026)

El Anexo A se verificó el 25/09/2026 sobre `ccf77e6`. Este anexo recoge lo que llegó al Nivel 3 y
al cierre del juego hasta el 01/10/2026: tres commits que ya están en `main` —uno de ellos solo de
documentos— y las correcciones del acta D10 en la rama `feat/cierre-de-slices-y-oe3`, que entran en
el commit de cierre con la tarjeta D10-3. Las de B.5 a B.8 se comprobaron en EditMode y por réplica;
sus pruebas PlayMode están escritas y se corren en la revisión de la etapa.

*(01/10/2026, después: la revisión de la etapa las corrió y están en verde, con la suite PlayMode
completa en 363/363; lo que entró tras la revisión está en B.11.)*

### B.1 Qué cambió y dónde

| Fecha | Commit | Qué cambió |
|---|---|---|
| 25/09 | `44fd479` | La balsa del ensamblaje se recompone sobre la forma de `prop_n3_balsa_cruzando` (3/4) y agarrar y soltar prueban el alfa — B.2 |
| 30/09 | `37b3cb7` | «Recoger troncos» se marca con el último tronco (INC-46), el hallazgo del resumen en dos oraciones (INC-110), «Reiniciar» vuelve a la fase activa (INC-117) y los créditos nombran a los autores de la Familia Anonaky (INC-78) — B.3 |
| 30/09 | `5a22df1` | Documentos: INC-46 a INC-54 cerrados — B.4 |
| 01/10 | sin hash, D10-3 | La escena final suena (INC-125), la balsa del cruce baja bajo la espuma (INC-124) y sus viajeros no saltan, «Probar balsa» en base y amarre con un hundimiento que suena (INC-122, INC-123) y el plano abierto a la escala de las narrativas (INC-118) — B.5 a B.8 |

`44fd479` entró en `main` con el PR #87 (`995b26d`, 29/09); `37b3cb7` y `5a22df1`, con el PR #88
(`1c7f4ab`, 30/09).

### B.2 `44fd479` (25/09/2026): la balsa del ensamblaje en 3/4

Los diecisiete espacios de `N3_RaftAssemblyContent` se recompusieron sobre la forma de
`prop_n3_balsa_cruzando`: cinco troncos en diagonal, de atrás a delante; dos lianas por tronco; el
mástil plantado en el tronco del centro y la vela a su derecha. Ningún espacio gira. El orden del
asset es el de dibujo, así que cada tronco va seguido de sus amarres y el de delante tapa las puntas
de las lianas del de atrás. Con troncos en diagonal las cajas se solapan: los espacios prueban ahora
el alfa del sprite y al soltar gana el de encima con dibujo bajo el puntero; el error medido pasó del
34 % de lo visible de cada tronco a 0 %. Prueba:
`AssemblyPanel_RF40_SoltarSobreLaPuntaDeUnTroncoLoPoneEnEseTroncoYNoEnElVecino`. El detalle de
resoluciones y siluetas está en `Props-y-Sonidos-Resultados.md`, Anexo B.

### B.3 `37b3cb7` (30/09/2026): lo que el Nivel 3 y el cierre ganaron al cerrar INC-46 a INC-117

- **La tarea «Recoger troncos» se marca con el último tronco**, no antes (INC-46).
  `RiverScene_RF36_RecogerTroncosSeMarcaConElUltimoTroncoYNoAntes`.
- **El hallazgo del resumen, en dos oraciones** (INC-110, RNF-01): «Descubriste que una balsa grande
  se arma por partes. Primero la base, luego el amarre y al final el mástil con la vela.».
  `LevelSummaryContent_RNF01_NingunaOracionSupera20Palabras` ya no parte las oraciones por los dos
  puntos.
- **«Reiniciar» vuelve a la fase activa** y no a la recolección, también al repetir el nivel ya
  completado (INC-117, decisión de Santiago del 30/09): al confirmar la base o el amarre,
  `AssemblyPanelController` registra la fase siguiente como activa
  (`GameFlowRunner.SetPlayingPhase`), sin cambiar de estado ni de escena. Pruebas:
  `GameFlow_HU17_SetPlayingPhaseActualizaLaFaseActivaSinNavegar`,
  `GameFlow_HU17_SetPlayingPhaseSeRechazaFueraDePlayingOEnOtroNivelOFaseInvalida` y
  `RiverScene_HU17_ReiniciarEnElAmarreOElMastilVuelveALaFaseActivaYNoALaRecoleccion`.
- **Los créditos** nombran a los autores y la obra de la Familia Anonaky, sin Fredoka y en oraciones
  de veinte palabras como máximo (INC-78, INC-79), con arte real en lugar de rótulos de trabajo
  (INC-82). Pruebas: `Credits_RNF23_NombraALosAutoresDeLosPersonajesYLaObraDeOrigen`,
  `CreditsContent_RNF01_NingunaOracionSupera20Palabras` y
  `Scenes_RNF01_NingunTextoDeEscenaEsUnRotuloDeTrabajo`.

### B.4 `5a22df1` (30/09/2026): las inconsistencias de §8 y A.2, cerradas

INC-46 a INC-54 se cerraron el 29/09 con la regla de ese día: gana el juego y se corrige el
documento. INC-46 corrigió `Direccion_de_Arte.md` §10.2 y §14.2, que describen ya el panel de
casillas del juego; INC-51 corrigió HU-14 FA-01 y CU-03 2a (nota de A.2); INC-52 e INC-53 corrigieron
la dirección de arte, `Interfaces.md` y el guion. La tabla de A.2 y la lista de §8 quedan vencidas:
desde el 30/09 solo siguen abiertos PG-05 y PG-06.

### B.5 La escena final suena al bosque con las fogatas (INC-125)

`N3_EscenaFinal` tenía `Ambient` y `AmbientLayer` vacíos (A.6). Ahora suena `amb_n2_bosque_dia` con
`amb_n1_cueva_fuego` en la segunda capa, el mismo par que `N2_PuenteI`: la familia camina hacia las
fogatas con el bosque de día detrás (guion §1.9). No hay código nuevo: `NarrativeSceneController`
ya pedía los dos ambientes al empezar. Prueba: `RiverSounds_RF44_LaEscenaFinalSuenaAlBosqueConLasFogatas`.

### B.6 El cruce de la 3.3: la balsa bajo la espuma (INC-124) y los viajeros que no saltan

- **La balsa baja** de y 0,455 a 0,395 y navega por debajo de la espuma de la cascada, que ocupa
  y ∈ [0,403; 0,544]; no baja a 0,39 porque tocaría el cuadro de diálogo en la parada L4. Con ella
  bajan los cuatro viajeros: posición y pasos de las líneas 0 y 2, −0,06. Los pasos de la línea 3, el
  desembarco, no se tocan: bajarlos metería a la Niña en el río. La parada L3 pasa de (0,65; 0,47) a
  (0,65; 0,41), con el mismo zoom ×1,55. Es una excepción expresa de Santiago (D-i) a la regla del
  acta D05 de mover el objeto y no el encuadre, y queda registrada en el asset porque el diseño de
  cámara del N3 está perdido. Pruebas: `NarrativeSequence_RF44_LaBalsaDelCruceNavegaBajoLaEspumaDeLaCascada`
  y `NarrativeSequence_RF44_LaFamiliaBajaConLaBalsaYViajaEnSuSitio`, y
  `NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo` con la lista `Verificadas` entera
  (EditMode); `NarrativeScene_RNF21_LaBalsaCruzaSinSaltosNiParpadeos` (PlayMode).
- **Los viajeros no saltan aunque el texto avance.** Pulsando «Continuar» durante los 9 s del cruce,
  la Niña y el Niño saltaban a su destino en la línea 1, Papá en la 2 y los cuatro en la 3 y la 4:
  cada paso nuevo cortaba el camino en curso. `NarrativeProp.FinishesSteps` marca a quien termina sus
  pasos: un paso nuevo no le corta el camino (`ActorTimeline.Interrupts`), lo que se lee mientras
  camina lo hace al llegar y, si algo lo movía, baja después desde donde llegó
  (`ActorTimeline.PendingStep`, C# plano); `NarrativeSceneController` solo pregunta y ejecuta. Llevan
  la marca los cuatro viajeros de la 3.3 y nadie más: las otras 17 narrativas siguen como estaban.
  Pruebas: ocho métodos nuevos en `ActorTimelineTests` (15 casos),
  `NarrativeSequence_RF44_SoloLosViajerosDelCruceTerminanSusPasos` y la ampliación de
  `NarrativeSequence_RNF03_…` a los sitios de paso de quien termina sus pasos (EditMode);
  `NarrativeScene_RNF21_AvanzarElTextoDuranteElCruceNoHaceSaltarALosViajeros` (PlayMode). La regla
  está en `Personajes-Resultados.md`, Anexo B.

### B.7 «Probar balsa» en base y amarre (INC-122) y la balsa que suena al hundirse (INC-123)

Decisión de Santiago del 30/09/2026 (acta D10, §5).

- **`RaftAssembly.Test()`** (C# plano). En la base y en el amarre valida la fase abierta, devuelve
  al inventario solo lo mal puesto (RF-43), cuenta un rechazo (RF-45) y nunca consolida ni abre fase
  (RF-40); en la fase de mástil y vela es `Confirm()`. Si solo había espacios vacíos, el mensaje es
  `UnfinishedTestMessage` —«La balsa se hundió: todavía no está terminada. ¿Qué parte falta armar?»—,
  que dice qué revisar sin cifras ni nombrar la pieza (CP-03, CP-06).
- **El panel.** `Button_Test` («Probar balsa») aparece junto a «Listo» en la base y el amarre; en la
  última fase el botón de confirmar sigue siendo «Probar balsa». Los dos botones pasan por un solo
  `Validate`: la prueba anticipada cuenta como intento y para la pista de los tres intentos seguidos,
  la familia anima y la balsa se hunde. Antes de la última fase solo se marca lo mal puesto y no los
  vacíos, porque marcarlos sería un mapa de dónde va cada pieza (CP-06). No llama a `AfterAttempt`:
  la 3.2 sigue saliendo solo con el primer fallo de la balsa terminada. «Listo» rechazado sigue sin
  hundimiento ni sonido. Probar con la base entera y bien puesta tampoco la aprueba: solo «Listo»
  consolida (RF-40).
- **El sonido.** `sfx_n3_salpicadura_undimiento` se renombró desde el motor a `sfx_n3_hundimiento`,
  con el mismo GUID, y es `RiverSounds.RaftSinking`: suena al empezar todo hundimiento, el anticipado y
  el de la última fase. Describe y no castiga (dirección de sonido §2.1, CP-02). Corrige A.6, donde la
  pieza estaba en disco sin referenciar.
- **Pruebas.** EditMode, en verde: `RaftAssembly_RF42_ProbarLaBalsaIncompletaNoConfirmaNiAvanzaDeFase`
  (3 casos), `RaftAssembly_RF43_ProbarAntesDeTiempoDevuelveSoloLoMalPuesto`,
  `RaftAssembly_RF41_ProbarLaBalsaNoTocaLasFasesAprobadas`,
  `RaftAssembly_RF42_EnLaUltimaFaseProbarEsConfirmar`,
  `RaftAssembly_RF42_EnLaUltimaFaseProbarConUnVacioSenalaElVacioConElMensajeDeLaPrueba`,
  `RaftAssemblyContent_RF42_ElAssetTraeElMensajeDeLaBalsaSinTerminar`,
  `RiverSounds_RF42_LaBalsaQueSeHundeSuenaASalpicadura` y las ampliaciones de
  `RaftValidator_RF17_NingunMensajeContieneDigitos` y
  `RaftValidator_CP06_ElMensajeDiceQueRevisarNoCualEsLaPiezaCorrecta`. PlayMode:
  `AssemblyPanel_RF42_ElBotonProbarBalsaEstaDisponibleEnCadaFase`,
  `AssemblyPanel_RF42_ProbarLaBalsaIncompletaLaHundeYSigueEnLaFase`,
  `AssemblyPanel_RF42_ProbarAntesDeTiempoSenalaSoloLoMalPuesto`,
  `AssemblyPanel_RNF03_EnLaBaseLosDosBotonesLaBalsaYLasTablillasCabenSinSolaparse`,
  `AssemblyPanel_RF42_LaBalsaQueSeHundeSuenaUnaSalpicaduraYNadaMas` (base y última fase),
  `RiverScene_Guion841_ProbarAntesDeLaUltimaFaseNoGastaLaEscena32` y
  `RiverIndicators_RF45_UnaPruebaDeBalsaIncompletaCuentaComoIntento`; siguen
  `AssemblyPanel_CP02_UnaFaseQueNoPasaNoSuena` y `RaftAssembly_CP02_NoHayLimiteDeIntentosNiPantallaDeDerrota`.
- **Los documentos.** RF-11, RF-40 y RF-42 de OE1, el guion §1.8.3 a §1.8.4.1, CU-10 y HU-12 y HU-13
  describían «Probar balsa» solo en la última fase; INC-122 registra su corrección, cada uno con su
  fila fechada en el control de cambios.

### B.8 La lectura B: el plano abierto y la escala de las narrativas (INC-118)

Decisión de Santiago del 30/09/2026 (D-c). Medida en la escena, la mecánica se veía a 0,4 de la
escala de las narrativas del mismo río: la casilla de Papá medía 77,7 px de ilustración frente a
191,5 en la 3.1.

| Qué | Hasta el 30/09 | Desde el 01/10 |
|---|---|---|
| `PlayFraming` de la recolección | (0.2632, 0.2632) ×1.9 | (0.3572, 0.3572) ×1.4: la orilla con el río y el pie de la cascada |
| `AssemblyFraming` | (0.50, 0.23) ×2.2 | (0.50, 0.3125) ×1.6 |
| `RaftPosition` · `RaftSize` | (0.5, 0.23) · 0.28 | (0.58, 0.23) · 0.28: sobre el agua, del tamaño de la balsa de la 3.3 |
| `WalkableArea` | x 0.13–0.36 · y 0.05–0.31 | x 0.18–0.40 · y 0.05–0.34, los pies de Mamá |
| `StartPosition` · `BuildZonePosition` | (0.15, 0.15) · (0.35, 0.21) | (0.21, 0.19) · (0.395, 0.22) |
| `ProximityRadius` · `BuildZoneRadius` | 0.04 · 0.04 | 0.06 · 0.06 |
| Casilla de Mamá · Papá · Niña y Niño | 104 · 77,7 · 46,6 | 240, anclada por los pies · 178,75 · 107,25 |
| Material · zona de construcción | 96 · 170 | 172,8 · 255 |

- **En pantalla**, a 1920 × 1080: Papá pasa de 148 a 250 px, los niños de 89 a 150 y Mamá de 121–185
  a 193–315 px según su altura. La balsa del ensamblaje pasa de 665 a 484 px, y la casilla de un
  tronco mide 256 px, 1:1 con su sprite (antes se ampliaba ×1,38).
- **La orilla.** Los ocho materiales se reacomodaron por la orilla, todos a más de
  `ProximityRadius + BuildZoneRadius` (0,12) de la zona, porque la zona se abre al cruzar su borde.
  La familia espera detrás de la zona.
- **Mamá** se ancla por los pies (pivote 0,075, como la familia), espera en la zona al retomar el
  ensamblaje —al volver de la 3.2, desde disco o al reiniciar la fase— (`RiverSceneController.ResumeAt`)
  y se dibuja por profundidad con la familia y los materiales: lo que está más abajo, delante
  (`DepthOrder`, C# plano). Los prefabs no se reconstruyeron: solo cambian los valores de las
  instancias.
- **La 3.1 coincide con la mecánica**: pinta `prop_n3_tronco` en 3/4 y `prop_n3_mastil` (0,07 del
  alto) en lugar de `prop_n3_troncos`, que queda sin uso.
- **Pruebas**: `RiverLevelConfig_Guion82_ElPlanoDeRecoleccionEsLaOrillaConElRio` (sustituye a
  `…EsElBosqueConUnPocoDelRio` de A.5), `RiverLevelConfig_RF37_ElRepartoObligaARecorrerLaOrillaYCabeEnElPlanoFijo`
  ampliada, `RiverScene_INC118_LosPersonajesDeLaMecanicaTienenLaEscalaDeLaNarrativa`,
  `RiverScene_DA83_MamaSeAnclaPorLosPiesComoLaFamilia`,
  `RiverLevelConfig_INC118_LaEscena31PintaCadaMaterialConElArteDeLaOrilla`,
  `RaftAssemblyContent_RF44_LaBalsaDelEnsamblajeMideLoQueLaDelCruce` y `DepthOrder_DA83_*` (EditMode);
  `RiverScene_RNF14_AlRetomarElEnsamblajeMamaEsperaEnLaZona` (PlayMode).
- **Vence** las decisiones 2 y 4 de §3, el encuadre `(0.50, 0.23) ×2.2` de §7.3 y el radio de 0,04 de
  §8. Entre el ensamblaje y la 3.3 la balsa salta de (0,58; 0,23) a (0,56; 0,395): lo cubre el fundido
  a negro de `SceneLoader`.

### B.9 Pruebas y cifras al 01/10/2026

| Assembly | 25/09 (`ccf77e6`) | 30/09 (`359365e`) | 01/10, antes del N3 | Qué lo movió |
|---|---|---|---|---|
| `Game.Levels.River.Tests` (EditMode) | 41 | 41 | 41 | Las correcciones del N3 la amplían (B.5, B.7, B.8) |
| `Game.Levels.River.PlayMode.Tests` | 44 | 47 | 47 | `44fd479`: `AssemblyPanel_RF40_SoltarSobreLaPunta…`; `37b3cb7`: `RiverScene_RF36_RecogerTroncos…` y `RiverScene_HU17_Reiniciar…` |

La suite completa del 01/10/2026, con el Editor abierto a 1920 × 1080 y antes de las correcciones del
N3, dio EditMode 393 = 392 + 1 omitida y PlayMode 351 = 350 + 1:
`RiverLevel_RNF05_LaMemoriaQuedaBajoDosGigasConElNivel3Cargado` midió 2 127 MB porque el Editor
llevaba horas abierto —en reposo y sin el nivel ya reservaba 2 090 MB— y, con el Editor recién
abierto, pasa aislada con 1 660 MB. RNF-05 se mide sobre el ejecutable. Tras «Probar balsa» la suite
EditMode completa dio 421 = 420 + 1 omitida, sin pérdidas ni cambios de estado; la cifra final, con
la lectura B y las PlayMode del N3, es la de la revisión de la etapa.

*(01/10/2026, después: la revisión de la etapa dio EditMode 427 = 426 + 1 omitida y PlayMode
363/363, con `Game.Levels.River.Tests` en 57 y `Game.Levels.River.PlayMode.Tests` en 57;
`RiverLevel_RNF05_…` pasó con 1 982 MB y el Editor reiniciado. La cifra final es la de B.11.)*

### B.10 Lo que queda abierto al 01/10/2026

Frente a §8 y a A.2:

- **Golden Path del juego entero**: los dos recorridos sin incidencias se hacen sobre el ejecutable
  candidato con el arnés (decisión de Santiago, acta D10); la duración de 20 a 40 minutos la
  cronometra Santiago (`claudeDocs/tasks/OE4/Hoja-HUM.md`, H8).
- **PG-05**, observación con estudiantes, a cargo de Santiago (H1).
- **El ejecutable**: `Datos/` junto al `.exe`, cargas, memoria y paquete se miden sobre el candidato
  (`Slice 4/Slice-4-Resultados.md`, Anexo B). Los trece fallos de disposición de §5.6 en batchmode
  siguen siendo de entorno y se declaran en esa corrida.
- **Revisiones con el usuario**: por decisión de Santiago del 30/09/2026 (acta D10, §5), se marcan
  como hechas por él tras verificarlas con pruebas y capturas. El radio y el encuadre que nombra §8
  son los de la lectura B.
- **Arte**: la misma decisión acepta como definitivos el anillo de la zona y la balsa hundida.
  `prop_n3_balsa_cruzando` tiene cuatro troncos y la mecánica arma cinco: se acepta como está y queda
  pendiente del carril de arte para la entrega de la colaboradora del 07/10/2026.
- **Sonido**: el silencio S3 de la escena final (dirección de sonido §5) no existe —`SilenceCut` no
  tiene «ambiente al mínimo» y hacerlo pide código en `AudioManager`—. Santiago oye si
  `amb_n1_cueva_fuego` suena a fogata al aire libre (H6) y si dos pruebas seguidas, con dos
  salpicaduras de 1,74 s, molestan (H7).
- **El salto de los viajeros en otras narrativas**: el mismo corte existe en otras 14 (68
  situaciones; por ejemplo, `N1_Apertura`). Arreglarlo es marcar a esos personajes con `FinishesSteps`, pero cambia la
  puesta en escena de escenas ya revisadas: queda recomendado, con el sí de Santiago. Con lectura
  rápida en la 3.3, los niños celebran y Papá señala al llegar y no al leerse su línea; lo comprueba
  Santiago jugando.
- **Decisiones que siguen como en §8**: la plantilla `InputSystem_Actions.inputactions` y rehacer los
  inventarios de cámara.

### B.11 Lo que entró después de la revisión de la etapa (01/10/2026)

Apartado nuevo. La revisión adversarial del Nivel 3 terminó sin bloqueantes: las dos suites en verde
a la primera (EditMode 427 = 426 + 1 omitida y PlayMode 363/363, en
`claudeDocs/tasks/OE4/evidencias/suites/W2-full/`) y las pruebas nuevas que más importaban puestas
en rojo con una mutación y vueltas a verde al revertirla —la marca `FinishesSteps` a 0 rompe
`NarrativeSequence_RF44_SoloLosViajerosDelCruceTerminanSusPasos` y
`NarrativeScene_RNF21_AvanzarElTextoDuranteElCruceNoHaceSaltarALosViajeros`; marcar los vacíos antes
de la última fase y gastar la 3.2 con la prueba anticipada rompen
`AssemblyPanel_RF42_ProbarAntesDeTiempoSenalaSoloLoMalPuesto` y
`RiverScene_Guion841_ProbarAntesDeLaUltimaFaseNoGastaLaEscena32`—. De sus hallazgos no
bloqueantes, dos se corrigieron el mismo día con la tarjeta D10-4:

- **Al retomar, Mamá se mueve en el modelo y no solo en pantalla.** `RiverSceneController.ResumeAt`
  ponía la vista de Mamá en la zona, pero el modelo de caminata (`RiverWalk`) seguía en el arranque:
  sin efecto mientras no se volviera a andar, pero el siguiente `Tick` la habría devuelto al
  arranque de un salto. `RiverWalk.MoveTo` (C# plano) coloca a Mamá en el modelo y la recorta a
  `WalkableArea`, y `ResumeAt` mueve modelo y vista juntos. `BuildZonePosition` (0,395; 0,22) cae
  dentro del área, así que el recorte no la desplaza. Pruebas:
  `RiverWalk_RNF14_MoveToColocaAMamaEnElModeloYRecortaALosLimites` (EditMode, nueva) y un aserto
  nuevo sobre `river.Walk.Position` en `RiverScene_RNF14_AlRetomarElEnsamblajeMamaEsperaEnLaZona`,
  que sin `MoveTo` falla con Mamá en `StartPosition`. Siguen en verde `RiverLevelJourneyTests` (7/7)
  y `GameEndingTests` (2/2), que retoman por ese camino.
- **Las capturas del panel, con los botones opacos.** `AssemblyPanel_HU12_*` capturaba dos cuadros
  después de que el panel quedara quieto, con «Listo» y «Probar balsa» a mitad del fundido de 0,1 s
  de `Selectable`; ahora espera 0,15 s antes de cada una de las tres. En las capturas, «Listo» y
  «Probar balsa» van lado a lado en la base y el amarre, y en mástil y vela queda un solo «Probar
  balsa», centrado.
- **Cifras.** `Game.Levels.River.Tests` pasa de 57 a 58 y `Game.Levels.River.PlayMode.Tests` sigue en
  57. La verificación final antes del build dio EditMode 433 = 432 + 1 omitida y PlayMode 364/364
  —detalle en `Slice 4/Slice-4-Resultados.md`, «Verificación final y paquete (01/10/2026)»—, y el
  ejecutable candidato pesa 479,0 MB (RNF-06).
- **A la Hoja-HUM** (`claudeDocs/tasks/OE4/Hoja-HUM.md`) van las comprobaciones manuales que pidió la
  revisión: H9, la 3.3 con lectura rápida (R-5, el punto del salto de B.10); H10, la recolección
  jugada con el plano abierto, con «Recoger» a 0,06 y los tres puntos de escala de abajo; H11,
  «Probar balsa» con la base completa y bien puesta, que la hunde y cuenta un intento porque solo
  «Listo» aprueba (RF-40); H12, la lista de PF-SON pieza por pieza. H6 añade que el bosque de la
  escena final es el de día en una escena al atardecer, y H7, la doble salpicadura de dos pruebas
  seguidas (R-10).
- **Para el visto bueno de Santiago (D-a)**, en H10: el tronco mide ≈ 0,6 de la altura de Papá en
  la 3.1, ≈ 1 en la orilla y ≈ 1,3 en la balsa de la 3.3; el mástil de la orilla está de pie, porque
  `Collectible` no tiene giro, y en la 3.1 está tumbado; la familia no se escala por profundidad.
- **Solo registro.** El margen de RNF-03 en la 3.3 es fino —la balsa queda +0,0098 sobre el cuadro de
  diálogo en L3 y +0,005 en L4—, así que cambiar el arte o el tamaño de la balsa obliga a
  recalcularlo. La 3.2 pinta la balsa hundida en (0,55; 0,343) y el ensamblaje la arma en
  (0,58; 0,23): como el salto hacia la 3.3 de B.8, lo cubre el fundido a negro.
