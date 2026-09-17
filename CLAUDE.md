# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Qué es este repositorio

Prototipo de videojuego educativo 2D en Unity (trabajo de grado): tres niveles narrativos
—fuego, rueda, balsa— que ejercitan facetas del pensamiento computacional en estudiantes de
grado cuarto. Entrega = ejecutable portable de Windows, sin instalación ni internet.

Unity 6000.5.10f1, plantilla 2D + URP 17.6.0, Input System 1.20.0, Unity Test Framework 1.7.0.
Presupuestos duros que se verifican, no se estiman (RNF-04..RNF-06): carga de escena < 10 s,
memoria < 2 GB, paquete < 500 MB.

**Estado del código: no se transcribe aquí.** La única fuente es el `todo.md` del slice abierto
(desde el 16/09/2026 el abierto para código es `claudeDocs/tasks/Slice 3/todo.md`, con ninguna
casilla marcada) más los documentos de resultados **que existan** a su lado —el Slice 1 tiene
`Fase-N-Resultados.md` de las fases 0 a 3 y ninguno de la 5 ni de la 6; el Slice 2 tiene uno solo,
`Slice-2-Resultados.md`, de slice entero; el Slice 3 todavía no tiene ninguno—: casillas, cifras
de pruebas y checkpoints se leen ahí y no se duplican en este archivo — una copia se queda vieja en dos commits. Empezar leyéndolo.

**Rider 2026.2.0.2 con el plugin «MCP Server Extension for Unity» (id 30357)**, instalados el
06/09/2026 (vía winget). `mcp__rider__*` habla con el **Rider abierto** y con el proyecto cargado:
con Rider cerrado la sesión arranca con `ConnectionRefused`, que significa «no hay a quién
preguntar», no «no existe» — abrir Rider y reiniciar la sesión de Claude Code. Mientras tanto las
pruebas van por la CLI `unity` (ver §Comandos). Ni el silencio de un MCP ni un `ConnectionRefused`
son que la suite pase.

**Idioma:** identificadores y código en inglés; documentación, textos del jugador y
comunicación con el usuario en español.

**Al final de cada implementación** Mencionar al inicio mi nombre "Santiago"

## Documentos que gobiernan el trabajo — leer antes de codificar

| Archivo | Qué contiene |
|---|---|
| `claudeDocs/SPEC.md` | **El contrato.** Mapa de módulos, arquitectura, estructura de carpetas, estilo, estrategia de pruebas, límites (Siempre / Preguntar primero / Nunca), supuestos y preguntas abiertas. |
| `claudeDocs/INCONSISTENCIAS.md` | Los conflictos entre los `.docx` con la corrección aplicada a cada uno. Documento hermano de `SPEC.md`. Verificación vigente: 15/09/2026, rev. 10 — los hallazgos `INC-01`..`INC-45` están cerrados en los documentos: la refundición del 14/09/2026 aplicó `PG-07` cerrado (`INC-43`), el guía renombrado a **Algoritm** (`INC-44`) y su forma cambiante por nivel —fuego, rueda, gota— (`INC-45`). **Algoritm y las tres formas rigen para todo texto y asset nuevo.** Quedan abiertos `INC-46` (tareas del Nivel 3, exige decisión), `INC-47` (12 y 15/09/2026: la mecánica del Nivel 1 **reúne los materiales en un círculo y luego mide fuerza y cercanía de las piedras en dos deslizantes**, apartándose del guion §4.3 y de RF-15/RF-16 radicados; los nombres de prueba conservan esos RF), `INC-48` (el §4 del documento refundido es la arquitectura vieja), `INC-49` (15/09/2026: el menú de pausa es el del mockup 6 —Reanudar · Reiniciar · Volver al menú de niveles— y HU-17 aún dice «Continuar / Reiniciar nivel / Volver al menú principal») e `INC-50` (15/09/2026: «Empujar» en el bosque no anima el rodado, sale a la escena 2.2 que lo cuenta; RF-26/HU-08 lo describen dentro de la mecánica). |
| `claudeDocs/Direccion_de_Arte.md` | **La ley visual.** Paleta, grosor de línea, sombreado, personajes, entornos por nivel, UI, tipografía, VFX, nomenclatura de archivos (`char_`, `prop_`, `env_`, `ui_`) y checklist de aceptación (§17). Obligatorio antes de crear o generar cualquier asset visual; subordinado a `SPEC.md`, no introduce mecánicas. |
| `claudeDocs/Interfaces.md` | **Las pantallas.** Inventario de las diecisiete superficies de interfaz con su escena, el RF que traza y su estado en código, más el estilo de personaje que se le pasa al generador de imágenes. Subordinado a `Direccion_de_Arte.md`; no introduce mecánicas ni requisitos. |
| `claudeDocs/Camara_Narrativa_N2.md` + `docs/md/Camara_Narrativa.md` | **La cámara narrativa del Nivel 2.** El de `claudeDocs/` es el **diseño** (50 encuadres validados contra 16:9); el de `docs/md/` es el **inventario de lo aplicado**: cada movimiento de cámara (foco, zoom, paradas por línea) con quién lo decidió —[S] Santiago / [C] Claude—. Ojo: `docs/md/Camara_Narrativa_N2.md` **no** es un tercer documento, es una copia del diseño de `claudeDocs/`; el inventario del N2 es el que se llama `Camara_Narrativa.md` a secas. Los valores viven en `Assets/Game/Data/Narrative/N2_*.asset`; el documento explica el porqué, no sustituye al asset. Regla que evita el choque más común: para bajar la cámara al suelo hay que cerrar el plano (`y = 0.35` exige `zoom ≥ 1.43`). |
| `docs/Camara_Narrativa_N1.md` + `docs/md/Camara_Narrativa_N1.md` | **La cámara y la luz del Nivel 1** (desde el 11/09/2026). El de `docs/` es el **diseño de Santiago** (35 encuadres, cada uno con su estado de luz) y se trata como los `.docx`: no se edita desde código. El de `docs/md/` es su inventario: qué quedó en los `N1_*.asset` y en el motor (`NarrativeLight`, `CameraKey.HardCut`, `FlashSeconds`, `LightStart`, shader `Algoritm/Oscuridad`), el mapa parada → línea del asset y las desviaciones con su porqué. Solo el Nivel 1 usa la capa de oscuridad; el Nivel 2 no cambia. |
| `docs/md/Camara_Narrativa_N3.md` | **La cámara del Nivel 3** (16/09/2026): 32 encuadres sobre dos sprites **16:9 sin duplicar** (`env_n3_rio`, `env_final_fogatas`), donde el rango del foco es `[0.5/z, 1−0.5/z]` en los dos ejes y a zoom 1 solo cabe el centro — el trabajo lo hace el zoom, no el paneo. Aplicado en los `N3_*.asset` por R04; sus hojas están en `docs/md/verificacion_encuadres_N3/`. Los objetos van **más altos** que en su §6 para no quedar bajo el cuadro de diálogo. |
| `docs/md/verificacion_encuadres_N1/` + `_N2/` + `_N3/` | **Las hojas de verificación de encuadres** (acta D05, 13/09/2026): un PNG por escena y por parada, **generado desde el contenido y no a mano**, con los objetos donde el asset los pone y la franja del cuadro de diálogo marcada en rojo. Se rehacen cuando el contenido cambia; los dos inventarios de cámara las citan como `claudeDocs/verificacion_encuadres_*` — la ruta real es `docs/md/`. |
| `Assets/Game/Art/Inventario.md` | **El índice de sprites.** Qué archivo va en cada carpeta de `Assets/Game/Art/`, con la tarea que lo produce, la nomenclatura (`char_`, `prop_`, `env_`, `ui_`, `fx_`, `ref_`; niveles `n1`/`n2`/`n3`) y la convención de `.anim`. No decide nada: manda el tablero de arte `Tareas.xlsx` (163 piezas, tareas `S01..S16c`), **que no está en el repo**, y luego los `plan.md` y `Direccion_de_Arte.md`. **Del acta D06 (15/09/2026) quedan dos cosas pendientes sobre el arte:** los entornos ya recibidos siguen con el prefijo `entorno_` y hay que renombrarlos a `env_` **desde el motor** (`D06-3`), y los props definitivos del N1 y del N2 (`D06-1`, `D06-2`) entran **sustituyendo el archivo y conservando el nombre** — incorporarlos no toca escenas ni código. **Cómo entra una imagen (16/09/2026):** `Game.EditorTools/ArtImportRules.cs` fuerza en todo `Assets/Game/Art/` **sin comprimir** y `maxTextureSize` 4096 —comprimida, la ilustración plana enseña la rejilla de bloques de 4×4, y el N1 la multiplica por la capa de oscuridad—, y `ArtImport_RNF23_…` lo vigila; las 44 texturas ocupan así ~92 MB, lejos de RNF-05. El importador de fábrica trae `Sprite Mode: Multiple`, que para un fondo entero o un prop hace que `LoadAssetAtPath<Sprite>` devuelva nulo: cada imagen nueva se pasa a `Single` desde el motor (`TextureImporter.spriteImportMode`). Los dos entornos del N3 ya están renombrados y en `Single` (`River/env_n3_rio`, `Narrative/env_final_fogatas`, 16/09/2026); `Wheel/civilización_noche` sigue con nombre libre y sin uso. |
| `claudeDocs/Mockups de interfaz Algoritmia.html` | **Los mockups de pantalla** (uno por superficie, numerados). Los `todo.md` y `Camara_Narrativa_N1.md` los citan por número («mockup 7 · Nivel 1 · encendido»); es la referencia de disposición de una escena antes de tocar el `.unity`. |
| `claudeDocs/tasks/Slice N/plan.md` + `todo.md` | **El trabajo en curso.** `plan.md` es el plan técnico del slice (alcance, grafo de dependencias, tareas); `todo.md` es el tablero con casillas y checkpoints. Los cuatro slices están planeados y entre ellos cubren los 47 RF: `Slice 1` Golden Path y nivel fuego · `Slice 2` La Rueda · `Slice 3` El Río y cierre del juego · `Slice 4` progreso, informe docente y borrado de datos. **Se planean en orden y cada uno supone terminado el anterior**, pero desde el 10/09/2026 corren varios en paralelo — ver «Varios carriles a la vez» abajo. **Los `plan.md` no se reescriben:** el del Slice 3 es la rev. 1 del 30/08/2026 y sus avisos de precondición («los Slices 1 y 2 no están hechos», «`Assets/` sigue sin código», «no hay corredor de pruebas MCP») están vencidos; lo que sigue valiendo de ellos es qué pieza previa generaliza cada tarea. Ninguno rediscute `SPEC.md`. |
| `docs/*.docx` + `docs/md/*.md` | Fuentes del trabajo de grado: requerimientos, guion, casos de uso, historias y arquitectura. El `.docx` es el original radicado; el `.md` del mismo nombre en `docs/md/` es su conversión ya hecha con markitdown. **Los cuatro `.docx` refundidos están convertidos** (14/09/2026); el trabajo de grado, no. **Nunca editar ninguno de los dos desde código.** |
| `docs/actas/OE*/Acta_*.md` | Actas de seguimiento: qué se decidió, cuándo y por qué. Una carpeta por objetivo específico —`OE2/` serie `O01..O03` (ago 2026), `OE3/` serie `D01..D06` (sep 2026, la abierta)— y dentro `Acta_<serie><NN>_AAAA-MM-DD.md` junto a su `.docx` (las tres últimas, `D04`, `D05` y `D06`, solo tienen el `.md`). Son la trazabilidad de las decisiones; consultarlas cuando haga falta el *porqué* de un RF, no reabrirlas. **No hay tablero Kanban aparte**: el tablero vive en la §6 de cada acta («Compromisos y tablero Kanban»), con los compromisos nuevos y los movimientos al cierre. Ojo al leerlo hacia atrás: **las tarjetas se renumeran con la serie del acta que las abrió** —`O03-1` aparece como `D01-1` en los movimientos posteriores—, así que una tarjeta se rastrea por su texto, no por su id. |

**Dónde empezar una sesión de código:** en la primera casilla sin marcar del `todo.md` del
slice abierto más bajo, salvo que la sesión trabaje otro carril.

**Varios carriles a la vez (decisión del 10/09/2026).** La Fase 3 del Slice 1 (T12–T19, nivel fuego)
se cerró el 11/09/2026. El 12/09/2026 Santiago abrió en el Slice 1 la **Fase 5** (T20–T24,
rediseño de la mecánica del Nivel 1: arrastrar hojas y piedras → deslizante de **fuerza** 0–10 →
golpear → soplar; INC-47), que quedó hecha salvo la revisión con el usuario (Checkpoint E), y el
15/09/2026 la **Fase 6** (T25–T27: primero **reunir** todo en el círculo del centro, luego la
cámara al doble y **dos deslizantes**, fuerza y cercanía de las piedras; sin rótulo «Aún no»). Del
Slice 1 quedan abiertos los **Checkpoints E y F** (revisión) y el **Checkpoint D** (recorridos,
RNF-14, mediciones sobre la build portable). El **código** del Slice 2 se cerró y se fusionó a
`main` el 16/09/2026 (PR #74, acta D06 · tarjeta `D06-4`); de ese slice solo quedan casillas de
build y de revisión. El **Slice 3 (El Río)** abrió el 16/09/2026 en la rama `feat/slice-3`, sin
ninguna tarea cerrada: es el carril de código, y empieza por **R01** porque `R02` y todo lo que
cuelga de él están bloqueados por su pregunta abierta 1 (qué significa «fase» en el Nivel 3), que
decide el formato de los **datos persistidos** y por tanto es «preguntar primero». **El reparto es
por assembly, no por slice** — es lo único que evita que los carriles se pisen:

| Carril | Toca |
|---|---|
| **Slice 1 · Checkpoints D y E** | `Game.Levels.Fire`, `Game.Core`, escenas del N1, build portable |
| **Slice 2 · Checkpoint W-F** | `Game.Levels.Wheel`, `Game.Scaffolding` |
| **Slice 3** | `Game.Levels.River` (aún sin crear), `Game.Core`, `Game.Scaffolding`, escenas del N3 |

**El orden de las tarjetas y qué está abierto se lee en los `todo.md`, no aquí** — una copia
de ese estado en este archivo se queda vieja en dos commits. La Fase 5 del Slice 2 (W15–W18, más
W19 el mismo día) se cerró el 15/09/2026; del Checkpoint W-F quedan las mediciones sobre la build,
la exclusión manual de RNF-16, PG-05 y la revisión. **Dos cruces que ahora comparten los dos carriles:** el
menú de pausa es el prefab `Assets/Game/Prefabs/UI/MenuPausa.prefab` (mockup 6: Reanudar ·
Reiniciar · Volver al menú de niveles; `Time.timeScale = 0` mientras está abierto), instanciado
en las cuatro escenas jugables —tocarlo cambia el N1 y el N2 a la vez—, y `LevelSummary` elige
sus mensajes por nivel (`LevelSummaryMessages.Level`, un asset por nivel en
`LevelSummaryController.messagesByLevel`) y compone el relato sumando **todas** las fases del
nivel. En sentido contrario, W09 (12/09/2026) tocó
`Game.Core` y `Game.UI` con prueba: `GameFlow` acepta `Narrative → Narrative` y **retoma en la
primera fase pendiente que tenga escena** cuando se pide una ya confirmada (RNF-14; el runner le
pasa su tabla de escenas, y sin fase pendiente jugable se repite la pedida), `GameFlowRunner`
cae al menú si una fase no tiene escena, y `NarrativeSequence.NextSequenceId` encadena escenas
del guion sin un `if` en el controlador — la 2.2 con la 2.3, y desde el 13/09/2026 también
`N1_Apertura → N1_AparicionGuia → N1_Hallazgo`, que hasta entonces no las pedía nadie, y desde
el 15/09/2026 `N2_PuenteI → N2_Escena21_Bosque` (el Nivel 2 abre con el puente; antes abría en
la 2.1 y el puente caía al menú). Encadenar una escena es editar su asset, nunca tocar el
controlador.

**Al consumir `PlayerProfile`:** W02 lo reescribió. `ConfirmPhase(LevelId, int, …)` no existe; se
pasa un `PhaseId` (`Game.Core`). El formato del JSON no cambió.

**Del Slice 1 quedan además dos casillas `[~]` que no son de la Fase 3** y bloquean el
Checkpoint D: los residuos en `%AppData%\LocalLow\` (RNF-11/RNF-08 — decisiones pendientes sobre
`com.unity.modules.unityanalytics` y `usePlayerLog`) y el clic que confirma RNF-14 al reabrir el
ejecutable.

**`unity test` abre su propia instancia batchmode y exige el Editor cerrado**: dos carriles en la
misma máquina no pueden correr pruebas a la vez.

**Los `Assets/InitTestScene*.unity` son basura de corridas PlayMode interrumpidas** — el Test
Runner las crea y normalmente las borra al terminar. Están en `.gitignore`, no las referencia
nadie y se borran sin mirar.

**Leer un documento fuente:** usar la conversión ya hecha en `docs/md/<mismo nombre>.md` —
se lee con Read y se busca con Grep, que es lo que hace viable citar un RF sin releer el
documento entero. Regenerarla solo si el `.docx` cambió:

```bash
PYTHONIOENCODING=utf-8 markitdown "docs/<archivo>.docx" > "docs/md/<archivo>.md"
```

**`graphify-out/` es el grafo de conocimiento del proyecto entero — el atajo para saber dónde
está cada cosa.** Lo construye `/graphify` sobre las fuentes del repo (código, `claudeDocs/`,
`docs/`, `docs/md/`) menos lo que excluya `.graphifyignore` —hoy `Packages/`, que metía 527 nodos
del árbol de dependencias de Unity sin conectar con nada— y las resuelve en comunidades con
nombre: «Persistencia de perfiles (SaveStore)», «FirePanelController · adaptador del N1»,
«NarrativeSceneController · cámara y luz», «Menú de pausa (RF-07, HU-17)», «Dirección de arte ·
la ley visual». **Antes de rastrear a mano con Grep** cualquier pregunta de ubicación o de relación —dónde vive un comportamiento, qué toca un
cambio, por dónde pasa un RF, qué depende de qué— preguntárselo al grafo:

```
graphify query "¿quién decide cuándo se muestra una pista?"
graphify path "SaveStore" "GameFlow"       # camino más corto entre dos conceptos
graphify explain "HintPolicy"
```

`GRAPH_REPORT.md` es el índice legible —empezar por su sección «Community Hubs»—; `graph.json` es
el grafo para consumo de agente y `graph.html` la vista interactiva. Es una **foto**: la vigente es
del 15/09/2026 sobre el commit `863ef05`, 3300 nodos y 6315 aristas en 263 comunidades. Se refresca
con `/graphify . --update` tras un bloque de trabajo —el incremental solo reextrae lo que cambió, y
si lo cambiado es solo `.cs` no cuesta nada porque el AST no usa modelo—, y lo que el grafo señale
se confirma en el código antes de citarlo: ubica, no sustituye a leer el archivo.

`docs/md/`, `docs/actas/` y `graphify-out/` están en `.gitignore`: existen en este equipo pero no
en un clon limpio. Las conversiones se rehacen con markitdown, el grafo con `/graphify`; las
actas **no** se rehacen — y tampoco los dos inventarios de cámara `docs/md/Camara_Narrativa.md`
y `docs/md/Camara_Narrativa_N1.md`, que están escritos a mano y no salen de ningún `.docx`:
viven ignorados y sin copia. Las hojas de `docs/md/verificacion_encuadres_*` sí se rehacen, pero
desde el contenido de los `*.asset` y no a mano. Mover los inventarios a `claudeDocs/` o
excluirlos del `.gitignore` es una decisión pendiente de Santiago.

**Nunca** abrir un `.docx` con Read ni descomprimiendo el zip. markitdown ya está en el `PATH`;
sin `PYTHONIOENCODING=utf-8` los acentos salen como mojibake. El shell por defecto de este
proyecto es PowerShell, donde ese prefijo no es sintaxis válida: correrlo con la herramienta
Bash, o `$env:PYTHONIOENCODING='utf-8'` antes de invocar markitdown.

**Rutas siempre entre comillas.** La raíz del proyecto en este equipo es
`C:\Users\benab\My project` — **con espacio**, igual que el `productName` de Unity, por eso el XML
de pruebas rotula la suite raíz como `My project`. Ninguna ruta absoluta va sin comillas. Los `.docx` traen espacios y paréntesis en
el nombre: sin comillas, cualquier comando de shell falla o toca el archivo equivocado.

`SPEC.md` es la fuente de verdad para cualquier duda de alcance o diseño; este archivo no la
repite. Si algo del código contradice a `SPEC.md`, gana `SPEC.md` o se corrige el documento
explícitamente.

**Los documentos se refundieron el 14/09/2026: de seis pasaron a cuatro.** OE1 es ahora
`Solución OE1_Requerimientos.docx`, y `Solucion_OE2_Diseno_final.docx` absorbe en un solo archivo
el guion, los casos de uso, las historias de usuario, las matrices de trazabilidad y la
arquitectura. **El trabajo de grado volvió a `docs/` el 15/09/2026** (commit `8de614f`): está
versionado en `docs/Trabajo_de_Grado_2026_ICONTEC_IEEE (2).docx` y es el único `.docx` sin
conversión en `docs/md/` — si hace falta leerlo, convertirlo con markitdown como los demás.
(`INCONSISTENCIAS.md` todavía dice que hay que recuperarlo con `git show`: está vencido.)

**Orden de precedencia** cuando se contradicen: trabajo de grado → OE1 → guion → casos de uso e
historias → historias de usuario detalladas → arquitectura. Gana el de mayor prioridad y se
corrige el otro. Las filas 3 y 4 viven ahora en el **mismo archivo**, así que un choque entre
ellas ya no lo resuelve la precedencia sino la edición del documento. Las contradicciones internas
están resueltas y registradas en `claudeDocs/INCONSISTENCIAS.md` (rev. 10, 15/09/2026).

Los nombres en disco no coinciden con cómo se citan los documentos. El mapa, en orden de
precedencia:

| # | Documento | Archivo en `docs/` |
|---|---|---|
| 1 | Trabajo de grado | `Trabajo_de_Grado_2026_ICONTEC_IEEE (2).docx` — sin `.md` en `docs/md/` |
| 2 | OE1 requerimientos | `Solución OE1_Requerimientos.docx` |
| 3 | Guion | `Solucion_OE2_Diseno_final.docx` §1 |
| 4 | Casos de uso, historias y matrices | `Solucion_OE2_Diseno_final.docx` §2–§3 |
| 5 | Historias de usuario detalladas | `Historias_de_Usuario_HU01_HU18_v2 (1).docx` |
| 6 | Arquitectura | `arquitectura_videojuego_v2 (2).docx` y `Solucion_OE2_Diseno_final.docx` §4 |

**Traducción de las citas viejas**, que siguen apareciendo en `claudeDocs/` y en los `todo.md`:
«guion §N» → `Solucion_OE2_Diseno_final` **§1.N** («guion §4.3» es §1.4.3, «guion §12»
—puntos abiertos `PG-*`— es §1.2); «OE2 §4» —control de cambios— es §5.

(`SPEC.md` §Arquitectura cita `docs/arquitectura_videojuego_v2.docx`, sin el ` (2)`: es el
nombre lógico, no la ruta real.)

**Numeración de requerimientos.** Los RF están cerrados en `RF-01..RF-47`. Los RNF **no**: OE1
insertó `RNF-18` (parametrización de contenidos) el 24/08/2026 y desplazó en uno todo lo
posterior, de modo que el rango vigente es `RNF-01..RNF-23` — RNF-19 doble indicador, RNF-20
contraste, RNF-21 destellos, RNF-22 contenido, RNF-23 assets. OE2 y el documento de arquitectura
ya citan la numeración vigente. Al verificar una cita, compararla contra el **nombre**
del requerimiento y no contra su número: un desplazamiento mal aplicado produce ids que existen
y parecen correctos.

## Comandos

Todo pasa por el Editor de Unity: sus MCP (`mcp__coplay-mcp__*` para escenas y assets;
`mcp__rider__*` para compilar y probar cuando Rider esté configurado) y la CLI `unity`. **La tabla
completa está en `claudeDocs/SPEC.md` §Comandos** — no se duplica aquí. Lo que hay que saber antes
de empezar:

- **Pruebas → la CLI `unity`** (`C:\Users\benab\AppData\Local\Unity\bin\unity`, v1.0.0-beta.5,
  ya en el PATH). Reemplaza al `Unity.exe -runTests -batchmode` crudo: maneja comillas, exit codes,
  `--filter`, `--timeout`, `--coverage`.
  ```
  unity test --mode EditMode --output test-results.xml --timeout 900 --no-banner --non-interactive
  unity test --mode PlayMode --output play-results.xml --timeout 900 --no-banner --non-interactive
  unity test --mode EditMode --filter "Game\.Core\.Tests"   # un assembly
  unity test --mode PlayMode --filter "LevelSelect_RF03"    # una prueba
  unity status    # editores conectados: puerto, PID, estado
  ```
  `--filter` es una **expresión regular** contra el nombre completo (`namespace.Clase.Método`),
  no un glob: un patrón que empiece por `*` aborta la corrida entera con
  `ArgumentException: Quantifier {x,y} following nothing` y exit 2.
  Abre su **propia** instancia batchmode → exige el **Editor cerrado** (si no: `another Unity
  instance is running`, exit 6). Exit 0 = todo pasa, 2 = fallos o error de invocación.
  `--report-format` acepta `nunit` **o** `junit`, uno a la vez (`both` lo rechaza pese al `--help`).
  Flujo: Editor abierto para desarrollo con coplay-mcp → cerrarlo → `unity test` → reabrir.
- **Build de entrega → `unity build`**, el mismo binario de CLI y la misma exigencia de Editor
  cerrado. No hay Build Profile en `Assets/Settings/`, así que el destino se da con `--target`:
  ```
  unity build --target StandaloneWindows64 -o "Build/Algoritmia/Algoritmia.exe" --log-file build.log --no-banner --non-interactive
  ```
  La carpeta de salida **es** el entregable portable: sobre ella se miden RNF-04 (carga < 10 s),
  RNF-05 (memoria < 2 GB) y RNF-06 (paquete < 500 MB), y junto al `.exe` nace `Datos/` en la
  primera ejecución (RNF-07, RNF-11). Entran las escenas listadas en `EditorBuildSettings` —la
  lista crece con cada nivel—, con `Boot` de primera. Hoy son diez, `Level2_Forest` (entró en
  `4f69140`), `Level2_Workshop` (W09, 12/09/2026) y `Level2_Maze` (W13, 13/09/2026) incluidas; las escenas nuevas se añaden desde
  el Editor (Build Settings o un script de editor efímero de la skill `edit-scene`), no a mano.
  `/[Bb]uild[s]?/` está en `.gitignore`: el ejecutable no se versiona.
- **Con Rider abierto**, `mcp__rider__run_unity_tests` /
  `mcp__rider__get_unity_compilation_result` corren contra el Editor **abierto** (sin cerrar/reabrir)
  y habilitan el flujo test-first del plugin `unity-coding-skills`. `unity test` queda como
  respaldo para corridas limpias y CI.
- **Con Rider caído y el Editor abierto** (lo que pasó el 15/09/2026) queda un tercer camino, el
  único que no obliga a cerrar el Editor: un script `[InitializeOnLoad]` **dentro del proyecto**
  (`Assets/UnityCodingSkills/Editor/`, con su `.asmdef` referenciando `UnityEditor.TestRunner` +
  `nunit.framework.dll`) que re-registra `TestRunnerApi.RegisterCallbacks` tras cada recarga de
  dominio y escribe `TestRunnerResults/<etiqueta>.txt` en `RunFinished`; se dispara por reflexión
  desde un script del scratchpad con `mcp__coplay-mcp__execute_script` y se sondea el archivo con
  un `until`-loop. El archivo de resultados es la única señal fiable: **durante una corrida
  PlayMode el puente no responde** (`execute_script` y `get_unity_editor_state` expiran a los 60 s),
  así que el estado se lee de `TestRunnerResults/<etiqueta>.running`. Antes de lanzar PlayMode,
  dejar la escena activa limpia (`Boot`, sin cambios sin guardar) o el runner pide guardar. Todo
  —scripts, asmdef y carpeta de resultados— se borra al terminar: no es código del juego.
- **`coplay-mcp` solo responde con el Editor abierto Y ya inactivo.** Mientras compila o hace
  domain-reload devuelve `timed out` o `A task was canceled` — esperar a que quede inactivo y
  reintentar (regla de 10 s del skill `run-tests`). `list_unity_project_roots` responde aunque
  esté cargando; `get_unity_editor_state` es la sonda real de «¿el puente está vivo?».
- Si un MCP no responde, el Editor está cerrado, cargando, o el puente caído: eso **no** es que la
  suite pase, y hay que decirlo.
- Play Mode a mano: `mcp__coplay-mcp__play_game` / `stop_game`. Pulsar Play siempre arranca en
  `Boot`: lo fuerza `PlayFromBoot` (`Game.EditorTools`) vía `playModeStartScene`. Se **suspende
  solo** en batchmode y durante las corridas del Test Runner — si no, la suite PlayMode se cuelga
  entera porque el corredor espera su `InitTestScene`. No quitar esas guardas.
- Errores de compilación y consola: `mcp__coplay-mcp__check_compile_errors` /
  `mcp__coplay-mcp__get_unity_logs`, en vez de adivinar.
- El MCP nativo de Unity (`com.unity.ai.assistant`, relay en `~/.unity/relay/relay_win.exe`)
  arranca solo pero Unity lo marca **deprecado** y **no trae runner de tests** — no conectarlo.

## Workflow: plugin unity-coding-skills

Cargar la skill correspondiente **antes** de improvisar:

- Escribir/editar cualquier `.cs`: `code-writing-guide`.
- Feature nueva o cambio de spec en plan mode: `plan-feature` (plan → `test-designer` → `failing-test-writer` → refactor/dedup).
- Bug: `fix-bug` (reproducir → diagnosticar → corregir, test-first).
- `.unity` / `.prefab`: `edit-scene`. Otros YAML de Unity: `unity-yaml-editing-guide`.
- Tests: `test-writing-guide` / `test-designing-guide` / `refine-tests`.
- Warnings e inspecciones: `resolve-diagnostics`.

## Arquitectura en una pantalla

FSM + Scene Loader + capas. Detalle en `claudeDocs/SPEC.md` §Arquitectura; lo que hay que saber
antes de escribir la primera línea:

- **Un assembly (`.asmdef`) por módulo**, dependencias en un solo sentido: `Game.Core` →
  `Game.Scaffolding` → `Game.Levels.{Fire,Wheel,River}` → `Game.Reporting`. **Ningún nivel
  referencia a otro nivel** — eso es lo que hace ejecutable la prueba de exclusión de RNF-16.
  Fuera de esa cadena cuelgan tres assemblies más: `Game.UI` (→ `Game.Core`, `Game.Scaffolding`,
  `UnityEngine.UI`) con los controladores de pantalla, donde va la mayor parte del código de hoy;
  `Game.Audio` (→ `Game.Core`); y `Game.EditorTools`, **solo Editor**, que no referencia ningún
  `Game.*` (trae `PlayFromBoot`, `ArtImportRules` —la regla de importación del arte— y
  `Sandbox/CharacterProbe*`, una maqueta de personaje que no es código del juego).
  Hoy tienen código `Game.Core`, `Game.Scaffolding`, `Game.UI`, `Game.Levels.Fire`,
  `Game.Levels.Wheel` y `Game.EditorTools`: `Game.Audio` es un `.asmdef` vacío a la espera de su
  fase —igual que su `.asmdef` de prueba—, y `Game.Reporting` todavía no existe en disco (llega
  en el Slice 4). **El sonido del juego está comprometido con otra persona** (acta D05, tarjeta
  `D05-2`, 27/09/2026): no llenar `Game.Audio` sin que lo pidan.
- **La lógica es C# plano; el MonoBehaviour es un adaptador delgado.** `GameFlow` (la FSM) no
  es MonoBehaviour: se prueba en EditMode sin escena ni frames. Igual para validadores,
  contadores y máquinas de estado de cada nivel.
- **Estados parametrizados, no uno por escena.** `Narrative` lleva el **id** de la secuencia
  (`GameFlow.NarrativeSequenceId`, un `string`), no el `NarrativeSequence` en sí: ese SO vive en
  `Game.Scaffolding`, que depende de `Game.Core` y no al revés. Quien resuelve id → asset es la
  capa de arriba, y todo se reproduce en una única escena reutilizable; `Playing` recibe
  `LevelId` + fase. Añadir una escena narrativa = crear un asset, no un estado y una rama.
- **Lo que el texto nombra se ve entero y por encima del cuadro de diálogo** (acta D05,
  13/09/2026): el cuadro ocupa el cuarto inferior de la pantalla, y un objeto bien colocado al
  abrir se mete debajo en cuanto la cámara cierra el plano. Lo comprueba parada por parada
  `NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo` sobre la lista `Verificadas`
  de `NarrativeSequenceTests`. **Una escena entra en esa lista cuando su encuadre se revisó, no
  antes** —`N2_Escena25_Cierre` sigue fuera a propósito—: una prueba que se salta lo que no
  cumple no comprueba nada. Cuando objeto y encuadre no concuerdan **se mueve el objeto**: los
  encuadres son el diseño registrado en los documentos de cámara.
- **Interfaz inyectada donde hay un consumidor conocido; evento solo con varios oyentes.**
  No hay `EventBus` global.
- **Solo tres singletons con `DontDestroyOnLoad`**: `GameFlowRunner`, `SceneLoader`, `AudioManager`.
- **Los encuadres son fracciones del lienzo, no píxeles.** `IllustrationFraming`
  (`Game.Scaffolding`) traduce foco `(x, y)` + `zoom` contra el tamaño real del sprite, y lo usan
  por igual `NarrativeSceneController` y las tres escenas del Nivel 2. Por eso una entrega de arte
  entra **sustituyendo el archivo**: si la composición se conserva, ningún valor de los documentos
  de cámara cambia.
- **Contenido fuera del código** (CT-05): todo texto visible y todo parámetro ajustable jugando
  vive en ScriptableObjects con `[field: SerializeField]` + `[Tooltip]`.
- **Persistencia**: JSON por perfil en una carpeta `Datos/` junto al ejecutable — no
  `Application.persistentDataPath`, porque «portable» y «sin residuos» (RNF-07, RNF-11) deben
  significar lo mismo. Corriendo en el Editor esa carpeta cae en la raíz del proyecto
  (`My project/Datos/`), con perfiles reales de las pruebas manuales; está en `.gitignore`.
- **Probar sin ampliar la superficie pública**: un `AssemblyInfo.cs` en la raíz del módulo con
  `[assembly: InternalsVisibleTo("<Módulo>.Tests")]` — como los de `Game.Levels.Fire` y `Game.Levels.Wheel`. No subir un miembro
  a `public` solo para que lo alcance una prueba. **El assembly de PlayMode se llama
  `<Módulo>.PlayMode.Tests` y necesita su propia línea**: sin ella el `internal` no se ve desde
  PlayMode aunque la de EditMode esté puesta (`Fire` y `Wheel` tienen las dos; `Game.UI` solo la
  de PlayMode).
- Raíz de código y assets: `Assets/Game/`, namespaces `Game.*` siguiendo la ruta bajo
  `Scripts/` y elidiendo `Runtime`. Tests en `Assets/Tests/{EditMode,PlayMode}/<Módulo>/`.
- **El nombre de la prueba es la trazabilidad.** Patrón `<Sujeto>_<Requisito>_<QuéHace>` en
  español: `GameFlow_RNF13_RecorreElGoldenPathCompletoSinEstadoIrrecuperable`,
  `SaveStore_INC34_CaeALaRutaDeRespaldoSiDatosNoEsEscribible`. El id del medio es lo que hace
  verificable CT-10; una prueba sin él está mal nombrada.
- **Los `.asmdef` de pruebas no se escriben a mano, se copian** de uno existente: EditMode va con
  `includePlatforms: ["Editor"]`, `autoReferenced: false`, `defineConstraints:
  ["UNITY_INCLUDE_TESTS"]` y `nunit.framework.dll` en `precompiledReferences`; los de PlayMode
  son idénticos pero con `includePlatforms` vacío. `Game.Architecture.Tests` es la excepción: no
  referencia ningún `Game.*` porque lee los `.asmdef` del disco — así puede exigir el assembly
  de un módulo que todavía no tiene código (RNF-15, RNF-16). Ahí vive también `ArtImportTest`
  (RNF-23), que recorre por `AssetDatabase` los `.png` de `Art/` y avisa si una entrega nueva se
  cuela comprimida o reducida.

## Invariantes pedagógicos — no son preferencias

Se prueban explícitamente en cada nivel:

1. **No existe pantalla de derrota**, límite de intentos ni penalización (CP-02). Nada de
   `GameOver` en el enum de estados.
2. **Nada de puntajes ni cifras** en lo que ve el estudiante — ni en la retroalimentación ni en
   el resumen de fin de nivel (CP-03, RF-17). No hay `ScoreManager` ni campo de puntuación en el
   guardado. Las cifras existen, pero solo en el informe docente (RF-46).
3. **Lo aprobado nunca se pierde** por un fallo posterior (RF-41, RF-43).
4. El guía pregunta y descompone, **no resuelve** (CP-06) — el andamiaje es una capa propia.

Cuando el código rechaza el camino obvio por uno de estos criterios, dejarlo escrito en un
comentario: la razón es pedagógica, no técnica, y sin la nota una futura «mejora» reintroduce
la pantalla de derrota.

Otras reglas duras: Input System nuevo, nunca la clase `Input` legada; entrada limitada a clic
y clic sostenido, sin excepciones — los controles de dirección del Nivel 3 son botones en pantalla
accionados con clic, no teclado (CT-06, RNF-02); ningún dato por red (RNF-08,
RNF-10); nada de `.meta` escritos a mano. Todo RF necesita al menos un caso de prueba que lo
nombre (CT-10).

**Mensajes de commit.** Al final de cada implementacion dejar el texto de lo realizado para hacer el commit.
La regla radicada es que cada commit se asocia a su tarjeta del Kanban
(CT-11, RNF-17) — esa es la que rige para commits de código. Los commits que solo tocan
documentación vienen citando en cambio el hallazgo que resuelven (`dddecf2 Documentation
adjustments under INC-28`); mantener esa forma para trabajo sobre `claudeDocs/` y `docs/`.
Cuando el prototipo empiece a tener código y aparezcan tarjetas reales, unificar y borrar esta
nota. **No hacer commit automaticamente, unicamente dejar el mensaje**

**Preguntar antes de:** añadir un paquete a `Packages/manifest.json`; cambiar el formato o la
ubicación de los datos persistidos; modificar un RF/RNF o un criterio de aceptación (son
entregables ya radicados); añadir o quitar una mecánica que no esté en el guion.
