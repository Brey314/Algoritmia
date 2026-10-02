# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Qué es este repositorio

Prototipo de videojuego educativo 2D en Unity (trabajo de grado): tres niveles narrativos
—fuego, rueda, balsa— que ejercitan facetas del pensamiento computacional en estudiantes de
grado cuarto. Entrega = ejecutable portable de Windows, sin instalación ni internet.

Unity 6000.5.10f1, plantilla 2D + URP 17.6.0, Input System 1.20.0, Unity Test Framework 1.7.0.
Presupuestos duros que se verifican, no se estiman (RNF-04..RNF-06): carga de escena < 10 s,
memoria < 2 GB, paquete < 500 MB.

**Idioma:** identificadores y código en inglés; documentación, textos del jugador y
comunicación con el usuario en español.

**Al final de cada implementación** mencionar al inicio mi nombre «Santiago».

## Dónde empezar una sesión de código

**El estado del trabajo no se transcribe en este archivo.** Casillas, cifras de pruebas,
checkpoints, qué tarjeta está abierta y qué commit la cerró viven en el `todo.md` del slice y en
los documentos de resultados que existan a su lado (`Fase-N-Resultados.md`, `Slice-N-Resultados.md`).
Una copia de ese estado aquí se queda vieja en dos commits. Empezar leyendo ese `todo.md`: la
primera casilla sin marcar del slice abierto más bajo, salvo que la sesión trabaje otro carril.

Los cuatro slices están planeados y entre ellos cubren los 47 RF —`Slice 1` Golden Path y nivel
fuego · `Slice 2` La Rueda · `Slice 3` El Río y cierre del juego · `Slice 4` progreso, informe
docente y borrado de datos— y se planearon en orden, cada uno suponiendo terminado el anterior.
Desde el 10/09/2026 **corren varios en paralelo**, y **el reparto es por assembly, no por slice**
— es lo único que evita que los carriles se pisen:

| Carril | Toca |
|---|---|
| **Slice 1** | `Game.Levels.Fire`, `Game.Core`, escenas del N1, build portable |
| **Slice 2** | `Game.Levels.Wheel`, `Game.Scaffolding` |
| **Slice 3** | `Game.Levels.River`, `Game.Core`, `Game.Scaffolding`, escenas del N3 |
| **Slice 4** | `Game.Reporting`, `Game.Core`, `Game.UI` |

Además de los slices corre el **carril de arte y sonido** —toca `Assets/Game/Art/`,
`Assets/Game/Audio/`, `Game.Audio` y los tres niveles (`Game.Levels.River` desde el sonido del N3,
25/09/2026)—, así que se pisa con los tres primeros slices. Lo que lleva hecho, archivo por archivo, está en
`claudeDocs/tasks/Slice 3/Props-y-Sonidos-Resultados.md`; los personajes animados, que además tocan
`Game.Scaffolding`, `Game.UI`, los tres niveles, las 18 narrativas y las cinco escenas jugables, en
`claudeDocs/tasks/Personajes/Personajes-Resultados.md`; lo que sigue —personajes finales, perfiles, expresiones, habla— está planeado en `Plan-Personajes-Finales.md` (02/10/2026, aún sin ejecutar). Una regla suya que no caduca: los cuadros
de las animaciones del fuego y del humo conservan los nombres de entrega (`fuego_cenital_nivel_1_0000.png`, no
el `prop_n1_…` de `Direccion_de_Arte.md`) y los referencia la curva del `.anim`: renombrarlos es
trabajo del motor.

Desde el 29/09/2026 corre además el **carril OE4** —la evaluación funcional del prototipo—, con
su `plan.md`, `casos.md` (el catálogo `PF-*` con guion paso a paso) y `todo.md` en
`claudeDocs/tasks/OE4/`. Prueba el **ejecutable** compilado, no el Editor, y cada veredicto lleva
la versión en que se obtuvo. El candidato vigente es rc2 (01/10/2026, 479,0 MB), que sustituye a
rc1; los dos se compilaron sin etiqueta ni commit, desde el árbol de trabajo de la rama, y la
procedencia de rc2 —HEAD, huella de `git diff HEAD` y lista de archivos sin seguimiento— está en
`evidencias/build-rc2.md` (la de rc1, en `build-rc1.md`). Si un candidato se etiqueta, la etiqueta la pone
Claude **solo con el visto bueno de Santiago** (T01); los resultados van a `OE4-Resultados.md`.

**Tres cosas que los carriles comparten** — tocar cualquiera cambia más de un nivel a la vez:

- **El menú de pausa** (RF-07, HU-17) es el prefab `Assets/Game/Prefabs/UI/MenuPausa.prefab` (mockup 6: Reanudar ·
  Reiniciar · Volver al menú de niveles; `Time.timeScale = 0` mientras está abierto), instanciado
  en las **cinco** escenas jugables —`Level1_Cave`, `Level2_Forest`, `Level2_Maze`,
  `Level2_Workshop`, `Level3_River`—, y arrastra también las tipografías y los glifos del mockup.
- **`NarrativeVisitPolicy`** (`Game.Scaffolding`) decide si aparece el botón de omitir, y **no hay
  registro de escenas vistas** —la lista persistida es cerrada (RNF-09)—, así que lo deriva del
  progreso: «ya vista» es haber **terminado el nivel** antes —su última fase confirmada—, no haber
  confirmado una fase cualquiera; el cierre reflexivo usa `ReachedLevel` porque se llega a él justo
  después de confirmar la última (CP-07, RF-12). **Eso le impone un orden a quien cierre un nivel:
  primero el cierre reflexivo, después el desbloqueo.** `LevelSummary` elige sus mensajes por nivel
  (`LevelSummaryMessages.Level`, un asset por nivel en `LevelSummaryController.messagesByLevel`) y
  compone el relato sumando **todas** las fases del nivel.
- **`GameFlow`** acepta `Narrative → Narrative` y **retoma en la primera fase pendiente que tenga
  escena** cuando se pide una ya confirmada (RNF-14); sin fase pendiente jugable repite la pedida, y
  `GameFlowRunner` cae al menú si una fase no tiene escena. Al pasar de una narrativa a una
  mecánica, de una mecánica a una narrativa o entre dos narrativas encadenadas, `SceneLoader`
  **funde a negro y de vuelta** (`FadeSeconds`, 0,4 s por mitad, tiempo sin escalar); la regla
  vive en `GameFlowRunner.FadesBetween` y los menús cortan en seco. El negro se pinta con `OnGUI`
  porque `Game.Core` no referencia uGUI.

**Encadenar escenas narrativas es editar un asset, nunca tocar el controlador:**
`NarrativeSequence.NextSequenceId` las enlaza. **La ilustración es por secuencia**, así que cambiar
de fondo a mitad de escena son dos assets encadenados y no una rama — por eso las «cinco escenas»
del Nivel 3 viven en **siete** `N3_*.asset` (el puente II pasa de la cueva al horizonte y de ahí al río). Lo que depende del juego —la escena 3.2, que solo
aparece tras el primer fallo— lo decide `ConditionalNarrativeTrigger` (C# plano,
`Game.Scaffolding`), tampoco un `if` en el controlador.

**El Nivel 2 dura un día y lo cuenta la luz, no el arte.** En las narrativas lo hace
`NarrativeLight` (tinte y fondo del shader `fx_oscuridad`, que multiplica la ilustración):
amanecer azulado en `N2_PuenteI`, tarde sin tinte en la recolección y el armado, atardecer en la
2.4, y noche junto al fuego en la 2.5 y en el arranque de `N3_PuenteII`. En el laberinto, que no
tiene esa capa, lo hace `MazeLayout.LightTint`, que tiñe el entorno y todo lo que cuelga de él.
Lo vigilan `NarrativeSequence_RF05_ElNivel2TranscurreDelAmanecerALaNoche` y
`MazeScene_RF30_ElLaberintoEsAlAtardecer`. Las ilustraciones de 3840 de ancho (`env_n1_apertura`,
el bosque del N2) tienen una costura en x = 0,5 que ningún encuadre debe cruzar: lo avisa
`IllustrationFraming.Warnings` en el `OnValidate` de la secuencia.

**Los troncos del N2 ruedan en el motor, no por cuadros** (acta D09): `RollingLog`
(`Game.Scaffolding`) es un `Graphic` de uGUI que dibuja el tronco como cilindro en 3/4 a partir de
una sola textura (`prop_n2_tronco_textura`) y lee el giro que ya le dan `RollMotion` o el cursor.
La vista vive en `N2_TroncoRodante.asset` y se engancha con `NarrativeProp.Rolling` en las
narrativas y `WheelLevelConfig.LogLook` en el bosque — sin cámara ni modelo 3D.

**Cuántas fases tiene cada nivel** lo fija `PhaseId.PhasesPerLevel = { 1, 3, 3 }` — el Nivel 3 son
tres (base · amarre · mástil y vela) y su recolección **no se persiste**. **Al consumir
`PlayerProfile`:** `ConfirmPhase(LevelId, int, …)` no existe; se pasa un `PhaseId` (`Game.Core`).

## Documentos que gobiernan el trabajo — leer antes de codificar

| Archivo | Qué contiene |
|---|---|
| `claudeDocs/SPEC.md` | **El contrato.** Mapa de módulos, arquitectura, estructura de carpetas, estilo, estrategia de pruebas, límites (Siempre / Preguntar primero / Nunca), supuestos y preguntas abiertas. |
| `claudeDocs/INCONSISTENCIAS.md` | Los conflictos entre los `.docx` —y entre ellos y el juego— con la corrección aplicada a cada uno. Documento hermano de `SPEC.md`. **`INC-01`..`INC-130` están cerrados** (rev. 16, 01/10/2026): ante un conflicto ganó el juego y se editó el documento, lo que solo pedía el documento se implementó y lo que solo tenía el juego se añadió al documento; `INC-115`..`INC-117` son la excepción, decisiones de Santiago de corregir el juego. `INC-118`..`INC-130` recogen la sesión del 30/09 (acta D10) y el peso del paquete (RNF-06). **Algoritm** (el guía, `INC-44`) y sus tres formas por nivel —fuego, rueda, gota (`INC-45`): una llama con extremidades, recoloreada en madera y agua (`INC-52`)— rigen para todo texto y asset nuevo. Solo siguen abiertos `PG-05` y `PG-06` del guion, que exigen observar a estudiantes jugando, tres **pendientes de Santiago** del trabajo de grado (Anexo G con la autorización de la Familia Anonaky, herramientas de ilustración, colaboración en el arte) que no se redactan sin su dato y uno del carril de sonido (el silencio S3 de la escena final): están en «Residuos y puntos abiertos». |
| `claudeDocs/Direccion_de_Arte.md` | **La ley visual.** Paleta, grosor de línea, sombreado, personajes, entornos por nivel, UI, tipografía, VFX, nomenclatura de archivos (`char_`, `prop_`, `env_`, `ui_`) y checklist de aceptación (§17). Obligatorio antes de crear o generar cualquier asset visual; subordinado a `SPEC.md`, no introduce mecánicas. |
| `claudeDocs/Direccion_de_Musica_y_Sonido.md` | **La ley del audio.** Hermano de la dirección de arte: cinco pilares (ningún sonido de fallo, §2.1, es CP-02 en el oído), cuatro buses bajo `AudioManager`, nomenclatura `mus_`/`amb_`/`sfx_`, los tres silencios del guion como piezas con disparador (§5) y el inventario de 104 piezas por nivel. **Qué piezas están cableadas lo dice su §19**, no este archivo: estar en `Assets/Game/Audio/` no es estar aplicado —una pieza suena solo si la referencia un asset (`N1_Sonidos`, `N2_Sonidos`, los `N*_*.asset`)—, y los puntos `PS-*` abiertos están en el mismo documento. |
| `claudeDocs/Interfaces.md` | **Las pantallas.** Inventario de las superficies de interfaz con su escena, el RF que traza y su estado en código, más el estilo de personaje que se le pasa al generador de imágenes. Subordinado a `Direccion_de_Arte.md`; no introduce mecánicas ni requisitos. |
| `claudeDocs/Camara_Narrativa_N2.md` · `docs/Camara_Narrativa_N1.md` | **Los diseños de cámara que quedan en disco.** El del N1 es de Santiago (35 encuadres, cada uno con su estado de luz) y se trata como los `.docx`: no se edita desde código; solo el N1 usa la capa de oscuridad. El del N2 son 50 encuadres validados contra 16:9 — regla que evita el choque más común: para bajar la cámara al suelo hay que cerrar el plano (`y = 0.35` exige `zoom ≥ 1.43`). **El diseño del N3 ya no está en el equipo** (era `docs/md/Camara_Narrativa_N3.md`): 32 encuadres sobre dos sprites 16:9 **sin duplicar**, con el foco acotado a `[0.5/z, 1−0.5/z]` en los dos ejes —a zoom 1 solo cabe el centro, así que el trabajo lo hace el zoom y no el paneo—; lo aplicado sobrevive solo en los `N3_*.asset`. En los tres niveles los valores viven en los `N*_*.asset`: el documento explica el porqué, no sustituye al asset. |
| ~~`docs/md/Camara_Narrativa*.md` · `docs/md/verificacion_encuadres_N1|N2|N3/`~~ | **Perdidos: no están en este equipo** (20/09/2026). Eran los inventarios de lo aplicado escritos a mano —qué quedó en cada asset y en el motor, con quién lo decidió [S] Santiago / [C] Claude— y las hojas de verificación de encuadres (un PNG por parada, generado desde el contenido, con la franja del cuadro de diálogo en rojo). Estaban en `.gitignore`, así que no hay copia en git. Las hojas **sí** se rehacen generándolas desde el contenido; los inventarios no. Mientras no se rehagan, la única fuente de lo aplicado es el `N*_*.asset`, y los enlaces a `claudeDocs/verificacion_encuadres_*` que aparecen en otros documentos no resuelven. |
| `Assets/Game/Art/Inventario.md` | **El índice de sprites.** Qué archivo va en cada carpeta de `Assets/Game/Art/`, la nomenclatura (`char_`, `prop_`, `env_`, `ui_`, `fx_`, `ref_`; niveles `n1`/`n2`/`n3`) y la convención de `.anim`. No decide nada: manda el tablero de arte `Tareas.xlsx` (163 piezas, tareas `S01..S16c`), **que no está en el repo**, y luego los `plan.md` y `Direccion_de_Arte.md`. |
| `claudeDocs/Mockups de interfaz Algoritmia.html` | **Los mockups de pantalla**, numerados desde el 2 (no hay mockup 1). Los `todo.md` y los documentos de cámara los citan por número («mockup 7 · Nivel 1 · encendido»); es la referencia de disposición de una escena antes de tocar el `.unity`. |
| `claudeDocs/tasks/Slice N/plan.md` + `todo.md` | **El trabajo en curso.** `plan.md` es el plan técnico del slice (alcance, grafo de dependencias, tareas); `todo.md` es el tablero con casillas y checkpoints. **Los `plan.md` no se reescriben**, así que sus avisos de precondición («los Slices 1 y 2 no están hechos», «`Assets/` sigue sin código») están vencidos; lo que sigue valiendo de ellos es qué pieza previa generaliza cada tarea. Ninguno rediscute `SPEC.md`. |
| `docs/*.docx` + `docs/md/*.md` | Fuentes del trabajo de grado: requerimientos, guion, casos de uso, historias y arquitectura. El `.docx` es el original radicado; el `.md` del mismo nombre en `docs/md/` es su conversión con markitdown. **Nunca editar ninguno de los dos desde código** sin la autorización expresa de Santiago para ese cambio; con ella se editaron los radicados del 29/09 al 01/10/2026, cada uno con su fila fechada en el control de cambios, y el trabajo de grado se redacta por Word COM (PowerShell + `Word.Application`). |
| `docs/actas/OE3/Acta_D*_*.md` | **Las actas de seguimiento, rehechas** (`D01` 02/09 … `D10` 30/09/2026): qué se decidió, cuándo y por qué, y **el tablero Kanban vive en su §6**, no en un archivo aparte. Siguen en `.gitignore` —existen en este equipo y no en un clon—, así que se perdieron una vez y pueden volver a perderse; su versión en Word vive en el SharePoint de Santiago, y por eso el entregable del OE3 ya no las anexa. La serie `OE2/` (`O01..O03`) **no** se rehizo: al leer hacia atrás **las tarjetas se renumeran con la serie del acta que las abrió** —`O03-1` aparece como `D01-1` después—, así que una tarjeta se rastrea por su texto y no por su id, y el *porqué* de lo decidido antes del 02/09 se busca en los `.docx` y en `INCONSISTENCIAS.md`. En las actas hay **dos Santiagos**: «Santiago» a secas en este archivo es Santiago Benavides Rey; Santiago Valdiri García es el otro estudiante (Slice 4, entorno del N3, fogata del cierre del N2). |
| `docs/OE3/` + `claudeDocs/entregables/OE3/` | **El entregable del OE3 y su generador.** `docs/OE3/` guarda lo publicado: `Solucion_OE3_Prototipo_funcional.docx`, capturas en `fig/` (LFS) y **un `.docx` por anexo** —A matriz RF → pruebas (CT-10), B las cinco fases del Slice 1, C Slice 2, D Slice 3, E arte y sonido, F personajes, G Slice 4—. **No hay Anexo H**: las actas están en Word en el SharePoint de Santiago. **El entregable se reescribe al estado vigente del prototipo** (acta D10), lo que deja sin efecto la decisión D1 del 29/09 de darle solo una nota fechada. El generador se recuperó y está versionado en `claudeDocs/entregables/OE3/`: los capítulos en `src/`, los anexos B–G en `src/anexos/` y `tools/` con `build.py` (pandoc más `word_finalize.ps1` por Word COM, con la plantilla de estilos del OE2 extraída de `127fbc4` para que el aspecto no cambie) y `rf_matrix.py`, que escribe el Anexo A desde los nombres de las pruebas (`--worktree` cuenta las del árbol de trabajo). `python claudeDocs/entregables/OE3/tools/build.py` arma los ocho `.docx` en `build/` (ignorado) y corre las guardas —sin «§», sin Anexo H, las tablas que cita el trabajo de grado, figuras descargadas de LFS—; **solo `--publish`**, si pasan, los copia a `docs/OE3/`. Se edita `src/` y se publica: los `.docx` de `docs/OE3/` no se tocan a mano. |

`SPEC.md` es la fuente de verdad para cualquier duda de alcance o diseño; este archivo no la
repite. Si algo del código contradice a `SPEC.md`, gana `SPEC.md` o se corrige el documento
explícitamente.

### Precedencia y nombres en disco

**Orden de precedencia** cuando se contradicen: trabajo de grado → OE1 → guion → casos de uso e
historias → historias de usuario detalladas → arquitectura. Gana el de mayor prioridad y se
corrige el otro. Las filas 3 y 4 viven en el **mismo archivo** desde la refundición del
14/09/2026 (de seis documentos pasaron a cuatro), así que un choque entre ellas ya no lo resuelve
la precedencia sino la edición del documento.

| # | Documento | Archivo en `docs/` |
|---|---|---|
| 1 | Trabajo de grado | `Trabajo_de_Grado_Entrega_Plantilla_28jul.docx` (desde el 30/09/2026) |
| 2 | OE1 requerimientos | `Solución OE1_Requerimientos.docx` |
| 3 | Guion | `Solucion_OE2_Diseno_final.docx` §1 |
| 4 | Casos de uso, historias y matrices | `Solucion_OE2_Diseno_final.docx` §2–§3 |
| 5 | Historias de usuario detalladas | `Historias_de_Usuario_HU01_HU18_v2 (1).docx` |
| 6 | Arquitectura | `arquitectura_videojuego_v2 (2).docx` y `Solucion_OE2_Diseno_final.docx` §4 |

**Traducción de las citas viejas**, que siguen apareciendo en `claudeDocs/` y en los `todo.md`:
«guion §N» → `Solucion_OE2_Diseno_final` **§1.N** («guion §4.3» es §1.4.3, «guion §12» —puntos
abiertos `PG-*`— es §1.2); «OE2 §4» —control de cambios— es §5. (`SPEC.md` §Arquitectura cita
`docs/arquitectura_videojuego_v2.docx`, sin el ` (2)`: es el nombre lógico, no la ruta real.)
El trabajo de grado pasó el 30/09/2026 a la plantilla oficial del 28 de julio
(`Plantilla Documento final 28 de julio de 2026 Pregrado.docx`, en blanco): los capítulos 6–8 son
OE1–OE3 y falta el del OE4. `Trabajo_de_Grado_2026_ICONTEC_IEEE (2).docx` sigue en `docs/` como
versión anterior; `SPEC.md` e `INCONSISTENCIAS.md` ya citan la plantilla.

**Numeración de requerimientos.** Los RF están cerrados en `RF-01..RF-47`. Los RNF **no**: OE1
insertó `RNF-18` (parametrización de contenidos) el 24/08/2026 y desplazó en uno todo lo
posterior, de modo que el rango vigente es `RNF-01..RNF-23` — RNF-19 doble indicador, RNF-20
contraste, RNF-21 destellos, RNF-22 contenido, RNF-23 assets. Al verificar una cita, compararla
contra el **nombre** del requerimiento y no contra su número: un desplazamiento mal aplicado
produce ids que existen y parecen correctos.

## Fuentes, conversiones y el grafo de conocimiento

**Leer un documento fuente:** usar la conversión ya hecha en `docs/md/<mismo nombre>.md` — se lee
con Read y se busca con Grep, que es lo que hace viable citar un RF sin releer el documento
entero. Regenerarla solo si el `.docx` cambió:

```bash
PYTHONIOENCODING=utf-8 python -m markitdown "docs/<archivo>.docx" > "docs/md/<archivo>.md"
```

**Nunca** abrir un `.docx` con Read ni descomprimiendo el zip. markitdown se instala con
`python -m pip install "markitdown[docx]"` y **su ejecutable no queda en el `PATH`**: invocarlo
siempre como `python -m markitdown`. Sin `PYTHONIOENCODING=utf-8` los acentos salen como mojibake,
y el shell por defecto de este proyecto es PowerShell, donde ese prefijo no es sintaxis válida:
correrlo con la herramienta Bash, o fijando `PYTHONIOENCODING` en `$env:` antes de invocar.

**`graphify-out/` es el grafo de conocimiento del proyecto entero — el atajo para saber dónde está
cada cosa.** **Antes de rastrear a mano con Grep** cualquier pregunta de ubicación o de relación
—dónde vive un comportamiento, qué toca un cambio, por dónde pasa un RF, qué depende de qué—
preguntárselo al grafo:

```
graphify query "¿quién decide cuándo se muestra una pista?"
graphify path "SaveStore" "GameFlow"       # camino más corto entre dos conceptos
graphify explain "HintPolicy"
```

`GRAPH_REPORT.md` es el índice legible —empezar por «Community Hubs»—; `graph.json` es el grafo
para consumo de agente y `graph.html` la vista interactiva. Es una **foto**: su fecha y
tamaño están en la cabecera de `GRAPH_REPORT.md`.
**Los `docs/md/*.md` sí están en el grafo pero congelados**: los extrajo una corrida
vieja y `manifest.json` no los sigue (cero entradas, porque graphify respeta el `.gitignore`), así
que ningún `--update` los vuelve a leer. Sirven para **ubicar** un RF o una HU radicada; el texto
que se cita sale siempre del `.md` convertido, que es el que está al día.
Lo que el grafo señale se confirma en el código antes de citarlo: ubica, no sustituye a leer el
archivo. Se refresca con `/graphify . --update` tras un bloque de trabajo —el incremental solo
reextrae lo que cambió, y si lo cambiado es solo `.cs` no cuesta nada porque el AST no usa modelo—.
**Dos cosas que hay que rehacer a mano al reconstruirlo desde cero**, porque `/graphify .` a secas
no las hace:

1. **Graphify respeta el `.gitignore`**, así que los `docs/md/*.md` hay que sumarlos al corpus
   explícitamente o el grafo se queda sin los documentos radicados — que es justo lo que le pasa a
   la foto vigente. (Las exclusiones sí sobreviven a un clon: `.graphifyignore` está **versionado**
   desde `2cbe287` y sus comentarios explican cada línea —`Packages/`, `Assets/Game/Art/**/*.png`,
   `Assets/Game/Audio/**`—; leerlo antes de tocarlo, no reescribirlo de memoria.)
2. El AST crea un **nodo-stub por archivo** para cada símbolo externo (`…_cs_button`,
   `…_cs_recttransform`): unos 500 nodos de `UnityEngine`/`System`/NUnit que inflan el grado de los
   controladores. Se podan con esta regla, que no necesita lista de tipos: un stub sin
   `source_file` cuyo `label` **sí** existe como nodo con `source_file` se fusiona con él; si no
   existe, es externo y se borra.

## Comandos

Todo pasa por el Editor de Unity: sus MCP (`mcp__coplay-mcp__*` para escenas y assets;
`mcp__rider__*` para compilar y probar) y la CLI `unity`. **La tabla completa está en
`claudeDocs/SPEC.md` §Comandos** — no se duplica aquí. Lo que hay que saber antes de empezar:

- **Pruebas → la CLI `unity`** (`C:\Users\benab\AppData\Local\Unity\bin\unity`, ya en el PATH).
  Reemplaza al `Unity.exe -runTests -batchmode` crudo: maneja comillas, exit codes, `--filter`,
  `--timeout`, `--coverage`.
  ```
  unity test --mode EditMode --output test-results.xml --timeout 900 --no-banner --non-interactive
  unity test --mode PlayMode --output play-results.xml --timeout 900 --no-banner --non-interactive
  unity test --mode EditMode --filter "Game\.Core\.Tests"   # un assembly
  unity test --mode PlayMode --filter "LevelSelect_RF03"    # una prueba
  unity status    # editores conectados: puerto, PID, estado
  ```
  `--filter` es una **expresión regular** contra el nombre completo (`namespace.Clase.Método`), no
  un glob: un patrón que empiece por `*` aborta la corrida entera con `ArgumentException:
  Quantifier {x,y} following nothing` y exit 2. Abre su **propia** instancia batchmode → exige el
  **Editor cerrado** (si no: `another Unity instance is running`, exit 6). Exit 0 = todo pasa,
  2 = fallos o error de invocación. `--report-format` acepta `nunit` **o** `junit`, uno a la vez
  (`both` lo rechaza pese al `--help`). Dos carriles en la misma máquina **no** pueden correr
  pruebas a la vez. Flujo: Editor abierto para desarrollo con coplay-mcp → cerrarlo →
  `unity test` → reabrir.
- **Build de entrega → `unity build`**, el mismo binario y la misma exigencia de Editor cerrado.
  No hay Build Profile en `Assets/Settings/`, así que el destino se da con `--target`:
  ```
  unity build --target StandaloneWindows64 -o "Build/Algoritmia/Algoritmia.exe" --log-file build.log --no-banner --non-interactive
  ```
  La carpeta de salida **es** el entregable portable: sobre ella se miden RNF-04, RNF-05 y RNF-06
  —hoy pesa 479,0 MB, con 21 de margen (rc2, `claudeDocs/tasks/OE4/evidencias/build-rc2.md`)—;
  `LicenseNotices` deja junto al `.exe` la carpeta `Licencias/` con los avisos de la OFL y de
  Phosphor (MIT), y a su lado nace `Datos/` en la primera ejecución (RNF-07, RNF-11). Entran las escenas
  listadas en `EditorBuildSettings` —hoy doce: las cinco jugables más las siete de flujo (`Boot`,
  `MainMenu`, `LevelSelect`, `Credits`, `Narrative`, `LevelSummary`, `TeacherReport`)—, con `Boot`
  de primera; las escenas nuevas se añaden **desde el Editor** (Build Settings o
  un script de editor efímero de la skill `edit-scene`), no a mano. `/[Bb]uild[s]?/` está en
  `.gitignore`.
- **Tras cualquier corrida batchmode (`unity test`, `unity build`), `ProjectSettings.asset` sale
  sin `SENTIS_ANALYTICS_ENABLED`** en los defines de Standalone, y `com.unity.ai.inference` lo
  repone en la siguiente recarga de dominio del Editor: ese diff es ruido y no se commitea. Solo
  compila código de Editor del paquete, así que no llega al ejecutable. Un build reescribe además
  `ProjectSettings.asset` (`preloadedAssets` con `ControlesJugables.inputactions`): también es
  ruido, y se restaura con `git checkout` antes de medir la huella del árbol. `UnityConnectSettings.asset`
  ya no cambia (rc1 lo dejaba en `m_Enabled: 1`; rc2 no, por `UnityServicesOff`). Lo que sí está apagado en
  `ProjectSettings` —analítica, estadísticas de hardware, pantalla de Unity, Alt+Intro (INC-97,
  decisión D3: no se quita ningún paquete del manifiesto)— lo vigila `PlayerSettingsTest`, y los
  servicios de Unity los fuerza además `UnityServicesOff` al compilar, sea cual sea la copia en
  memoria del Editor.
- **El ejecutable se maneja con `claudeDocs/tasks/OE4/herramientas/oe4.ps1`**, el arnés del carril
  OE4 (`pwsh -NoProfile -File … help`): trata `Algoritmia.exe` como caja negra —lo lanza tras
  comprobar su SHA-256 y hace clic, sostiene, arrastra y escribe en fracciones del área cliente,
  como `IllustrationFraming`; sin foco o con otra ventana encima no envía nada— y mide RNF-04 (la
  línea «RNF-04: … cargó en N s» que `SceneLoader` escribe en el `Player.log`), RNF-05, RNF-06
  (`Tamano`, sin `Datos/` ni `*_DoNotShip`), RNF-10 y los residuos de RNF-07 y RNF-11 contra una foto
  previa. Su estado vive en `%TEMP%\Algoritmia-OE4` (`OE4_TRABAJO`). No es código del juego.
- **Con Rider abierto**, `mcp__rider__run_unity_tests` / `mcp__rider__get_unity_compilation_result`
  corren contra el Editor **abierto** (sin cerrar/reabrir) y habilitan el flujo test-first del
  plugin `unity-coding-skills`. Con Rider cerrado la sesión arranca con `ConnectionRefused`, que
  significa «no hay a quién preguntar», no «no existe»: abrir Rider y reiniciar la sesión de Claude
  Code, o usar `unity test`.
- **Con el Editor abierto y sin Rider, las pruebas van por
  `claudeDocs/tasks/OE4/herramientas/editor.ps1`**, un envoltorio de la API HTTP de
  `com.unity.pipeline` (en el manifiesto; `unity cmd` no habla con su versión 0.5.0-exp.1):
  ```
  pwsh -NoProfile -File "claudeDocs/tasks/OE4/herramientas/editor.ps1" help
  pwsh -NoProfile -File "claudeDocs/tasks/OE4/herramientas/editor.ps1" tests-edit -              # EditMode entera
  pwsh -NoProfile -File "claudeDocs/tasks/OE4/herramientas/editor.ps1" tests-play RiverScene_    # PlayMode filtrada
  ```
  Lee puerto y token de `Library/Pipeline/.unity-pipeline-port` en cada llamada —el token no se
  imprime—, y el filtro es una subcadena del nombre completo (`-` = sin filtro). **Sus guardas:** toca
  `Temp/claude-active` antes de cada llamada (`ClaudeSceneAutosave`); se niega a correr pruebas con
  escenas sucias —nunca pulsa el diálogo— o con errores de compilación vigentes, porque Unity
  conservaría los assemblies viejos y saldría un verde falso (tras editar código, `recompile`); y un
  mutex por proyecto rechaza una segunda invocación que modifique el mismo Editor. `tests-play` fija la Game View a
  1920×1080 —sin ella fallan las pruebas de disposición— y abre `Boot` antes; PlayMode se sondea por
  `Temp/pipeline_test_status.json`, porque durante la recarga de dominio el HTTP no responde. Salidas:
  0 bien, 1 fallos, 2 infraestructura, 3 guarda (no tocó nada); resultados en
  `%TEMP%\Algoritmia-editor-runs` o `-Out`. Si una corrida se interrumpe, `exec cancel_tests`, y
  `exec editor_stop` si quedó en Play. `exec build` compila el ejecutable sin cerrar el Editor, pero
  ignora `output_path` y deja la salida en `Builds/StandaloneWindows64/`: se mueve a `Build/Algoritmia/`.
- **Cómo y cuándo correr las pruebas lo fija `claudeDocs/tasks/OE4/NORMA-PRUEBAS.md`**, con los comandos
  listos para lanzar. La suite completa es un comando en segundo plano, `herramientas/suite2.ps1`, que
  la reparte entre el Editor del proyecto y el de una copia desechable (`C:\Dev\Algoritmia-B`, que se
  sincroniza sola) y la cruza con `list_tests`: 21 min frente a ~30 en un solo Editor.
- **Si la API del pipeline no responde**, queda un camino más que tampoco obliga a cerrar
  el Editor: un script `[InitializeOnLoad]` efímero **dentro del proyecto** que re-registra
  `TestRunnerApi.RegisterCallbacks` tras cada recarga de dominio y escribe el resultado a un
  archivo, disparado por reflexión con `mcp__coplay-mcp__execute_script`. El archivo de resultados
  es la única señal fiable: **durante una corrida PlayMode el puente no responde** (expira a los
  60 s). Antes de lanzar PlayMode, dejar la escena activa limpia (`Boot`, sin cambios sin guardar)
  o el runner pide guardar. Todo el andamiaje se borra al terminar: no es código del juego.
- **`coplay-mcp` solo responde con el Editor abierto Y ya inactivo.** Mientras compila o hace
  domain-reload devuelve `timed out` o `A task was canceled` — esperar y reintentar (regla de 10 s
  del skill `run-tests`). `get_unity_editor_state` es la sonda real de «¿el puente está vivo?».
- **Norma: el aviso de guardar no debe aparecer mientras trabaja Claude.** El aviso «Scene(s) Have
  Been Modified» es **modal**: bloquea el hilo principal y con él al puente, así que ninguna
  herramienta MCP puede pulsarlo y todo expira como si el Editor estuviera cargando. Lo abren el
  Test Runner (en su segundo paso, antes de cualquier callback), abrir otra escena y entrar a Play.
  Guardar a mano antes **no basta**, por dos causas medidas el 23/09/2026: `execute_script` de
  coplay **marca la escena activa como modificada al terminar**, aunque el script solo lea o acabe
  de guardarla, y recompilar tras cambiar los campos serializados de un componente de una escena
  abierta la ensucia también — y `mcp__rider__run_unity_tests` compila dentro de la misma llamada.
  Lo resuelve `ClaudeSceneAutosave` (`Game.EditorTools`) con el hook `PreToolUse` de
  `.claude/settings.json`: el hook toca `Temp/claude-active` antes de cada herramienta
  `mcp__rider__*` / `mcp__coplay-mcp__*`, y durante los 2 minutos siguientes el script guarda toda
  escena que se ensucie. **Solo actúa cuando lo lanza Claude** (decisión de Santiago): lo que ya
  estaba sucio con Claude inactivo es de una persona y no se toca hasta que alguien lo guarde.
  Límite conocido: una edición humana hecha *durante* esos 2 minutos se guarda con lo demás. Una
  sesión abierta antes de que existiera el hook no lo carga hasta reiniciarse o abrir `/hooks`, y
  sin hook el script no hace nada. Si aun así aparece el aviso, el
  rescate es `~/.claude/scripts/unity-dialog.ps1` (**fuera del repo**, solo en este equipo): sin
  argumentos lista título, texto y botones del diálogo; con `-Click Save` lo pulsa, y se niega si hay
  más de un diálogo o el título no coincide con `-TitleLike` (`*Modified*`). **Pulsar requiere la
  confirmación de Santiago en ese momento**, tras decirle qué diálogo es y qué escena se guardaría:
  guarda todo lo modificado, incluido lo que quizá no debía quedar en el `.unity`.
  ```
  powershell.exe -NoProfile -ExecutionPolicy Bypass -File "$HOME/.claude/scripts/unity-dialog.ps1"
  ```
- **Si un MCP no responde, el Editor está cerrado, cargando, o el puente caído: eso NO es que la
  suite pase**, y hay que decirlo.
- Play Mode a mano: `mcp__coplay-mcp__play_game` / `stop_game`. Pulsar Play siempre arranca en
  `Boot`: lo fuerza `PlayFromBoot` (`Game.EditorTools`) vía `playModeStartScene`. Se **suspende
  solo** en batchmode y durante las corridas del Test Runner — si no, la suite PlayMode se cuelga
  entera porque el corredor espera su `InitTestScene`. **No quitar esas guardas.**
- Errores de compilación y consola: `mcp__coplay-mcp__check_compile_errors` /
  `mcp__coplay-mcp__get_unity_logs`, en vez de adivinar.
- El MCP nativo de Unity (`com.unity.ai.assistant`) arranca solo, pero Unity lo marca **deprecado**
  y **no trae runner de tests** — no conectarlo.
- Los `Assets/InitTestScene*.unity` son basura de corridas PlayMode interrumpidas: están en
  `.gitignore`, no los referencia nadie y se borran sin mirar. `Assets/Scenes/SampleScene.unity` y
  `Assets/Settings/Scenes/URP2DSceneTemplate.unity` son de la plantilla 2D + URP: **sí** están
  versionados, pero no entran al build ni los abre nada del juego.

**Rutas siempre entre comillas.** La raíz del proyecto en este equipo es `C:\Dev\Algoritmia`
(hasta el 19/09/2026 fue `C:\Users\benab\My project`). Desde el 29/09/2026 el `productName` de
Unity es `Algoritmia` y el `companyName` `Universidad Catolica de Colombia`, así que el XML de
pruebas rotula la suite raíz «Algoritmia», y el `Player.log`, la ruta de respaldo (INC-34) y la
clave del registro del reproductor viven en `…\LocalLow\Universidad Catolica de Colombia\Algoritmia\`
y `HKCU\Software\Universidad Catolica de Colombia\Algoritmia`. Los `.docx` traen espacios y paréntesis en el
nombre: sin comillas, cualquier comando de shell falla o toca el archivo equivocado.

`docs/md/`, `graphify-out/`, `docs/actas/` y `claudeDocs/tasks/Sprites/` están en `.gitignore`:
existen en este equipo pero no en un clon limpio. Los dos primeros se rehacen solos —las
conversiones con markitdown, el grafo con `/graphify`—; los otros dos están **escritos a mano y ya
se perdieron una vez** (20/09/2026). Las actas se rehicieron; los inventarios de cámara no. Por eso
cualquier documento nuevo que no salga de un `.docx` ni de un generador va a `claudeDocs/`, que sí
se versiona. Sacar las actas del `.gitignore` es **decisión pendiente de Santiago**.

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
  `Game.Scaffolding` → `Game.Levels.{Fire,Wheel,River}`. **Ningún nivel referencia a otro nivel**
  — eso es lo que hace ejecutable la prueba de exclusión de RNF-16.
  `Game.Scaffolding` referencia además `UnityEngine.UI` (desde `8ec5120`, por `BurnReveal`).
  **`Game.Reporting` referencia solo `Game.Core`** (Slice 4 P01, no el final de la cadena que dibuja
  `SPEC.md`): retirar un nivel no puede romper el informe docente, y lo vigila
  `Architecture_RNF16_ReportingNoReferenciaANingunAssemblyDeNivel`. Es dato y lógica sin uGUI; sus
  pantallas (`TeacherReportController`, `EraseConfirmationDialog`) viven en `Game.UI`, y el borrado
  de un perfil es `SaveStore.Delete` (`Game.Core`), sin clase propia.
  Fuera de esa cadena cuelgan tres assemblies más: `Game.Audio` (→ `Game.Core`) con
  `AudioManager`, el tercer singleton; `Game.UI` (→ `Game.Core`, `Game.Scaffolding`, `Game.Audio`,
  `Game.Reporting`, `UnityEngine.UI`) con los controladores de pantalla, donde va la mayor parte del
  código de hoy;
  y `Game.EditorTools`, **solo Editor**, que no referencia ningún `Game.*` (trae `PlayFromBoot`,
  `ArtImportRules`, `AudioImportRules`, `ClaudeSceneAutosave`, `LicenseNotices` —tras cada build de
  Windows copia `OFL.txt` y `LICENSE-Phosphor.txt` a `Licencias/`, junto al `.exe`, y hace fallar el
  build si falta uno (INC-127)—, `UnityServicesOff` —antes de cada build apaga a la fuerza nueve
  interruptores de servicios de Unity (el general de Connect, Analytics, Insights, Cloud
  Diagnostics, `submitAnalytics`…) y, después, hace fallar el build si `globalgamemanagers` nombra
  `unity3d.com` (DEF-SPIKE-01, INC-97)— y `Sandbox/CharacterProbe*`, que no es código del juego).
  **Un nivel que suena referencia `Game.Audio`** —hoy los tres— y **no llama al
  gestor con clips propios sino con los de su ScriptableObject** (`FireSounds` → `N1_Sonidos`,
  `WheelSounds` → `N2_Sonidos`, `RiverSounds` → `N3_Sonidos`, CT-05); las escenas narrativas no tocan código: el ambiente es un campo de `NarrativeSequence`,
  el efecto y el silencio son campos de `DialogueLine`, y lo que suena **mientras un objeto se
  mueve** (la balsa que cruza) es `NarrativeProp.MotionAmbient`, en la capa de ambiente y solo lo
  que dura el movimiento. `AudioManager.Instance` puede ser nulo
  (escena abierta sin pasar por `Boot`, pruebas): todo consumidor lo comprueba y el juego sigue
  en silencio, porque el audio refuerza y nunca informa solo (§2.4). **El sonido restante es de Santiago Benavides Rey** desde el acta D08
  (tarjetas `D07-2`, `D08-2`): faltan el ambiente nocturno del Nivel 2 (`amb_n2_noche_intemperie`
  está en disco sin referenciar), la música, el sonido del diálogo y el silencio S3 de la escena
  final; el Nivel 1 entró el 21/09/2026, el Nivel 2 el 23/09, el Nivel 3 el 25/09 y, el 01/10, el
  hundimiento de la balsa (`sfx_n3_hundimiento`, INC-123) y el ambiente de la escena final, bosque
  con las fogatas en la segunda capa (INC-125).
- **`Game.Levels.River` a propósito no referencia `Unity.InputSystem`**: las flechas son botones
  uGUI con clic sostenido (`IPointerDown/Up`), y
  `RiverScene_INC01_NoExisteVinculacionDeTecladoEnElMapaDeControles` vigila que el assembly no gane
  ni esa referencia ni la del módulo `Input` legado. Lo genérico del N3 —guía, secuencias,
  `ConditionalNarrativeTrigger`— vive en `Game.Scaffolding` y `Assets/Game/Data/`.
- **La lógica es C# plano; el MonoBehaviour es un adaptador delgado.** `GameFlow` (la FSM) no es
  MonoBehaviour: se prueba en EditMode sin escena ni frames. Igual para validadores, contadores y
  máquinas de estado de cada nivel.
- **Estados parametrizados, no uno por escena.** `Narrative` lleva el **id** de la secuencia
  (`GameFlow.NarrativeSequenceId`, un `string`), no el `NarrativeSequence` en sí: ese SO vive en
  `Game.Scaffolding`, que depende de `Game.Core` y no al revés. Quien resuelve id → asset es la
  capa de arriba, y todo se reproduce en una única escena reutilizable; `Playing` recibe `LevelId`
  + fase. Añadir una escena narrativa = crear un asset, no un estado y una rama.
- **Los encuadres son fracciones del lienzo, no píxeles.** `IllustrationFraming`
  (`Game.Scaffolding`) traduce foco `(x, y)` + `zoom` contra el tamaño real del sprite, y lo usan
  por igual `NarrativeSceneController` y las escenas jugables. Por eso una entrega de arte entra
  **sustituyendo el archivo**: si la composición se conserva, ningún valor de los documentos de
  cámara cambia. Lo mismo vale para colocar un objeto sobre la ilustración
  (`WheelLevelConfig.CargoPlacedPosition`): en píxeles se sale del cuadro en cuanto la cámara
  cierra el plano.
- **Lo que el texto nombra se ve entero y por encima del cuadro de diálogo** (acta D05): el cuadro
  ocupa el cuarto inferior de la pantalla, y un objeto bien colocado al abrir se mete debajo en
  cuanto la cámara cierra el plano. Lo comprueba parada por parada
  `NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo` sobre la lista `Verificadas` de
  `NarrativeSequenceTests`. **Una escena entra en esa lista cuando su encuadre se revisó, no
  antes**: una prueba que se salta lo que no cumple no comprueba nada. Cuando objeto y encuadre no
  concuerdan **se mueve el objeto**: los encuadres son el diseño registrado en los documentos de
  cámara.
- **Los personajes son rigs por recorte en uGUI, no 2D Animation** (INC-53): `CharacterRig`
  (`Game.Scaffolding`) gira y desplaza las partes (`char_<x>_parte_*.png`) con un `Animator` de un
  estado por `ActorAction`; prefabs en `Assets/Game/Prefabs/Characters/`. En la narrativa un
  personaje es un `NarrativeProp` con `Actor` y `Beats` —hereda casilla, orden de dibujo y la
  prueba del cuadro de diálogo— y `ActorTimeline` (C# plano) decide qué hace en cada línea. En las
  mecánicas, cada controlador llama `Play`/`PlayFor` en sus puntos de enganche; tras un fallo,
  `Encourage` y nunca otra cosa (CP-02). **Reconstruir un prefab de personaje cambia sus fileID y
  rompe las referencias** de las escenas y los 18 assets: para retocar animaciones se reescriben
  los clips (`claudeDocs/tasks/Personajes/`). `CharacterRig` se re-sincroniza en `OnEnable`/`Start` (`Play` + `Update(0)`) para que reactivar la jerarquía no deje un cuadro en pose en T: la primera acción se aplica seca y las siguientes con `CrossFade`.
- **Interfaz inyectada donde hay un consumidor conocido; evento solo con varios oyentes.** No hay
  `EventBus` global.
- **Solo tres singletons con `DontDestroyOnLoad`**: `GameFlowRunner`, `SceneLoader`, `AudioManager`
  — los tres son componentes del objeto `Persistent` de `Boot`.
- **Contenido fuera del código** (CT-05): todo texto visible y todo parámetro ajustable jugando
  vive en ScriptableObjects con `[field: SerializeField]` + `[Tooltip]`.
- **Persistencia**: JSON por perfil en una carpeta `Datos/` junto al ejecutable — no
  `Application.persistentDataPath`, porque «portable» y «sin residuos» (RNF-07, RNF-11) deben
  significar lo mismo. Corriendo en el Editor esa carpeta cae en la raíz del proyecto, con perfiles
  reales de las pruebas manuales; está en `.gitignore`.
- **Cómo entra una imagen:** `Game.EditorTools/ArtImportRules.cs` fuerza en todo
  `Assets/Game/Art/` **sin comprimir** y `maxTextureSize` 4096 —comprimida, la ilustración plana
  enseña la rejilla de bloques de 4×4, y el N1 la multiplica por la capa de oscuridad— y
  `ArtImport_RNF23_…` lo vigila. **Única excepción:** los cuadros del fuego y del humo del N1
  (`Props/Fire/Animations/`) entran a **1024**, también sin comprimir (INC-130): a tamaño completo el
  paquete pesaba 866,7 MB frente al tope de 500 de RNF-06, y en pantalla no pasan de ~650 px; lo
  vigila `ArtImport_RNF06_…`. El importador de fábrica trae `Sprite Mode: Multiple`, que para un
  fondo entero o un prop hace que `LoadAssetAtPath<Sprite>` devuelva **nulo**: cada imagen nueva se
  pasa a `Single` desde el motor (`TextureImporter.spriteImportMode`). **Cómo entra una secuencia
  de cuadros:** un `.anim` con curva `PPtr` sobre `Image.m_Sprite` más un `.controller` por clip —
  la `Animation` legacy (la que usa `ui_pulso_pista`) **no reproduce curvas PPtr**, así que el
  intercambio de sprites exige `Animator`. Una clave por **dibujo distinto**, no por archivo: las
  entregas vienen exportadas a 30 fps pero animadas a seises u ochos, y una clave por archivo
  multiplica las texturas cargadas (en el fuego del N1, 249 archivos = 34 dibujos: 1469 MB → 235 MB,
  RNF-05 y RNF-06). La temporización vive en el `.anim`, así que **no se regenera contando
  archivos**. **Cómo entra un sonido:**
  `AudioImportRules.cs` aplica la tabla §4.3 de la dirección de sonido por prefijo del nombre
  —`mus_` streaming estéreo, `amb_` streaming mono, el resto efecto: hasta 2 s PCM precargado, más
  largo Vorbis en memoria— y `AudioImport_RNF06_…` lo vigila; un `.wav` sin prefijo cuenta como
  efecto, así que ninguna pieza queda sin regla. Los nombres siguen §4.1 (`sfx_n1_golpe`, sin
  tildes ni mayúsculas) y se renombran **desde el motor** (`AssetDatabase.RenameAsset`) para
  conservar el GUID. Las piezas viven en `Assets/Game/Audio/{Global,Level 1,Level 2,Level 3}/`
  (PR #81, 21/09/2026) y la regla las alcanza en cualquier subcarpeta. **Mover un `.wav` por el
  Explorador sin su `.meta` le cambia el GUID** y deja mudo todo asset que lo referenciaba
  (`N1_Sonidos`, los `N1_*.asset`): se mueve desde el Editor o se mueve el `.meta` con él.
- **Probar sin ampliar la superficie pública**: un `AssemblyInfo.cs` en la raíz del módulo con
  `[assembly: InternalsVisibleTo("<Módulo>.Tests")]`. No subir un miembro a `public` solo para que
  lo alcance una prueba. **El assembly de PlayMode se llama `<Módulo>.PlayMode.Tests` y necesita su
  propia línea**: sin ella el `internal` no se ve desde PlayMode aunque la de EditMode esté puesta.
  Hoy ese archivo existe en `Game.Core` (desde `d7ace67`), `Game.Levels.{Fire,Wheel,River}`,
  `Game.UI` (**solo** la línea de PlayMode), `Game.Audio` —que abre además sus internos a
  `Game.Levels.{Fire,Wheel,River}.PlayMode.Tests` y `Game.UI.PlayMode.Tests`, que comprueban **qué** clip
  sonó—, `Game.Scaffolding` (desde `1d5ce58`, **solo** la línea de EditMode: su PlayMode aún no
  ve internos) y `Game.EditorTools` (por `LicenseNotices`, hacia `Game.EditorTools.Tests`);
  `Game.Reporting` se prueba por superficie pública, así que ahí no hay
  `AssemblyInfo.cs` que buscar — y si una prueba nueva lo necesita, se crea.
- Raíz de código y assets: `Assets/Game/`, namespaces `Game.*` siguiendo la ruta bajo `Scripts/` y
  elidiendo `Runtime`. Tests en `Assets/Tests/{EditMode,PlayMode}/<Módulo>/`.
- **El nombre de la prueba es la trazabilidad.** Patrón `<Sujeto>_<Requisito>_<QuéHace>` en
  español: `GameFlow_RNF13_RecorreElGoldenPathCompletoSinEstadoIrrecuperable`,
  `SaveStore_INC34_CaeALaRutaDeRespaldoSiDatosNoEsEscribible`. El id del medio es lo que hace
  verificable CT-10; una prueba sin él está mal nombrada.
- **Los `.asmdef` de pruebas no se escriben a mano, se copian** de uno existente: EditMode va con
  `includePlatforms: ["Editor"]`, `autoReferenced: false`, `defineConstraints:
  ["UNITY_INCLUDE_TESTS"]` y `nunit.framework.dll` en `precompiledReferences`; los de PlayMode son
  idénticos pero con `includePlatforms` vacío. `Game.Architecture.Tests` es la excepción: no
  referencia ningún `Game.*` porque lee los `.asmdef` del disco — así puede exigir el assembly de
  un módulo que todavía no tiene código (RNF-15, RNF-16). Ahí vive también `ArtImportTest`
  (RNF-23, y RNF-06 para los cuadros del fuego), que recorre por `AssetDatabase` los `.png` de `Art/`. `Game.Content.Tests` es la
  excepción contraria: referencia los tres niveles y `Game.UI` para barrer el contenido del juego
  entero (`Content_CP03_…`, ninguna cifra de desempeño en lo que ve el estudiante) — es el único
  sitio donde un assembly ve todos los niveles a la vez, y solo porque es de pruebas.

## Invariantes pedagógicos — no son preferencias

Se prueban explícitamente en cada nivel:

1. **No existe pantalla de derrota**, límite de intentos ni penalización (CP-02). Nada de
   `GameOver` en el enum de estados.
2. **Nada de puntajes ni cifras** en lo que ve el estudiante — ni en la retroalimentación ni en el
   resumen de fin de nivel (CP-03, RF-17). No hay `ScoreManager` ni campo de puntuación en el
   guardado. Las cifras existen, pero solo en el informe docente (RF-46).
3. **Lo aprobado nunca se pierde** por un fallo posterior (RF-41, RF-43).
4. El guía pregunta y descompone, **no resuelve** (CP-06) — el andamiaje es una capa propia.

Cuando el código rechaza el camino obvio por uno de estos criterios, dejarlo escrito en un
comentario: la razón es pedagógica, no técnica, y sin la nota una futura «mejora» reintroduce la
pantalla de derrota.

**Otras reglas duras:** Input System nuevo, nunca la clase `Input` legada; entrada limitada a clic
y clic sostenido, sin excepciones — los controles de dirección del Nivel 3 son botones en pantalla
accionados con clic, no teclado (CT-06, RNF-02), y una lista que desborda se desplaza con
**botones** y no con un `ScrollRect`, que trae el arrastre de uGUI y se pelea con el arrastrar y
soltar de la mecánica — la única excepción, decidida el 29/09/2026 (INC-80), son los `ScrollRect` del
informe docente (2) y de los créditos (1): esas pantallas no tienen arrastrar y soltar, y su barra se
maneja con clic y clic sostenido; ningún dato por red (RNF-08, RNF-10); nada de `.meta` escritos a mano. Todo
RF necesita al menos un caso de prueba que lo nombre (CT-10).

## Commits y decisiones

**Mensajes de commit.** Al final de cada implementación dejar el texto de lo realizado para hacer
el commit. **No hacer commit automáticamente, únicamente dejar el mensaje.** Cada commit de código
se asocia a su tarjeta del Kanban (CT-11, RNF-17); los commits que solo tocan documentación citan
en cambio el hallazgo que resuelven (`dddecf2 Documentation adjustments under INC-28`).

**Preguntar antes de:** añadir un paquete a `Packages/manifest.json`; cambiar el formato o la
ubicación de los datos persistidos; modificar un RF/RNF o un criterio de aceptación (son
entregables ya radicados); añadir o quitar una mecánica que no esté en el guion.
