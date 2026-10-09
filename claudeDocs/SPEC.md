# Spec: Prototipo de videojuego educativo — Pensamiento Computacional

Contrato de desarrollo derivado de los documentos en `docs/`. Los identificadores
RF/RNF/CP/CT/CN/CU/HU/PG remiten a esos documentos y son la unidad de trazabilidad (CT-10).

**Los documentos se refundieron el 14/09/2026: de seis pasaron a cuatro.** OE1 es ahora
`Solución OE1_Requerimientos.docx`, y `Solucion_OE2_Diseno_final.docx` absorbe en un solo archivo
el guion, los casos de uso, las historias de usuario, las matrices de trazabilidad y la
arquitectura. El trabajo de grado sigue en `docs/`
(`Trabajo_de_Grado_Entrega_Plantilla_28jul.docx`, sobre la plantilla oficial del 28 de julio desde el
30/09/2026, con su conversión en `docs/md/`; `Trabajo_de_Grado_2026_ICONTEC_IEEE (2).docx` queda como
versión anterior).

**Orden de precedencia.** Cuando dos documentos se contradicen gana el de mayor prioridad, y se
corrige el otro. Verificado contra su estado del 14/09/2026 (rev. 8):

| # | Documento | Qué gobierna |
|---|---|---|
| 1 | Trabajo de grado — `docs/Trabajo_de_Grado_Entrega_Plantilla_28jul.docx` | Objetivos, KPI, alcance, marco jurídico, metodología Árcade |
| 2 | `Solución OE1_Requerimientos.docx` | Lineamientos CP/CT/CN, RF-01..RF-47, RNF-01..RNF-23 |
| 3 | `Solucion_OE2_Diseno_final.docx` §1 (guion) | Narrativa, mecánicas, parámetros y textos exactos |
| 4 | `Solucion_OE2_Diseno_final.docx` §2–§3 | CU-01..CU-12, HU-01..HU-18, matrices de trazabilidad |
| 5 | `Historias_de_Usuario_HU01_HU18_v2 (1).docx` | HU detalladas (HU-01..HU-18): flujos, criterios y reglas de negocio |
| 6 | `arquitectura_videojuego_v2 (2).docx` | Decisiones técnicas de implementación |

**Traducción de las citas anteriores a la refundición**, que siguen apareciendo en todo
`claudeDocs/`: «guion §N» → `Solucion_OE2_Diseno_final` **§1.N** («guion §4.3» es §1.4.3;
«guion §12», los puntos abiertos `PG-*`, es §1.2); «OE2 §4» —control de cambios— es §5.

> **La §4 del documento refundido resume la arquitectura implementada (`INC-48`, cerrado el
> 29/09/2026).** Hasta esa fecha reproducía el capítulo **anterior** a la alineación
> (`Application.persistentDataPath`, `CinematicsPlayer` sobre StreamingAssets, ocho estados fijos,
> `EntityManager` con enemigos, `EventBus` global); se sustituyó por un resumen de lo que implementa
> el código —nueve estados con `Narrative` y `Playing` parametrizados, doce escenas, `Datos/` con
> ruta de respaldo, tres singletons, los nueve assemblies— con remisión a
> `arquitectura_videojuego_v2 (2).docx`, que sigue siendo el detalle. Las filas 3 y 4 de la tabla
> viven además en el **mismo archivo**, así que un choque entre ellas no lo resuelve la precedencia
> sino la edición del documento.

La precedencia no resuelve las contradicciones **internas** a un documento: esas se corrigen
editándolo. `INCONSISTENCIAS.md` (rev. 15, 30/09/2026) registra los hallazgos `INC-01`..`INC-117`,
y **todos están cerrados**: el 29/09/2026 se cerraron los que seguían abiertos (`INC-46`..`INC-54` y
los residuos `INC-24-r`, `INC-44-r`, `INC-44-r2`) y los sesenta nuevos (`INC-55`..`INC-114`) con una
regla —ante un conflicto gana el juego y se edita el documento; lo que solo pide el documento se
implementa; lo que solo tiene el juego se añade al documento—, y el 30/09/2026 `INC-115`..`INC-117`,
tres puntos en los que Santiago decidió corregir el juego. Solo siguen abiertos `PG-05` y
`PG-06` del guion, que exigen observar a estudiantes jugando.

**La mecánica del Nivel 1 ya coincide en documentos y código** (`INC-47`, cerrado el 29/09/2026):
el guion §1.4.3–§1.4.4, `RF-14`–`RF-16` y OE1 §3.6.1, HU-04..HU-07 y CU-05 describen lo que hace
el juego —reunir los materiales en el círculo y encender con dos deslizantes, fuerza y cercanía de
las piedras, con la tablilla mostrando el último mensaje— (ver el supuesto 7).

**Numeración.** Los RF están cerrados en `RF-01..RF-47`. Los RNF **no**: el control de cambios de
OE1 del 24/08/2026 insertó `RNF-18` (parametrización de contenidos, el enunciado de CT-05) y
desplazó en uno todo lo que venía después. El rango vigente es `RNF-01..RNF-23`:

| Enunciado | Antes | **Vigente** |
|---|---|---|
| Diálogos, tareas y parámetros fuera del código | — | **RNF-18** |
| Doble indicador además del color | RNF-18 | **RNF-19** |
| Contraste texto/fondo ≥ 4.5:1 | RNF-19 | **RNF-20** |
| Sin destellos de alta frecuencia | RNF-20 | **RNF-21** |
| Sin violencia, publicidad ni enlaces externos | RNF-21 | **RNF-22** |
| Recursos propios o con autorización escrita | RNF-22 | **RNF-23** |

OE2 y el documento de arquitectura ya citan la numeración nueva, y la arquitectura menciona
`RNF-18` junto a `CT-05` en §1, §6 y §11 (INC-16, cerrado). Al verificar cualquier cita,
compararla contra el **nombre** del requerimiento y no contra su número: un desplazamiento mal
aplicado produce ids que existen y parecen correctos.

**Prioridades.** 45 RF de prioridad Alta, 1 Media (RF-06, omisión de diálogos) y 1 Baja (RF-21,
iluminación progresiva). RF-46 subió de Media a Alta el 24/08/2026, con lo que la dependencia
RF-47 → RF-46 deja de ser un riesgo de cronograma.

---

## Objetivo

Prototipo de videojuego educativo 2D en Unity para estudiantes de grado cuarto (9–11 años)
del Colegio El Libertador IED, que ejercite las facetas del pensamiento computacional
—iteración, depuración, abstracción, reconocimiento de patrones, modelado, pensamiento
algorítmico y generalización— a través de tres niveles narrativos encadenados: una familia
prehistórica descubre el fuego, la rueda y construye una balsa.

**Usuarios.** Estudiante (principal, juega); docente (secundario, crea perfiles, consulta
desempeño, elimina datos); equipo evaluador (ejecuta las pruebas funcionales).

**Éxito** = ejecutable portable de Windows que un docente copia a la sala de sistemas y un
estudiante termina de principio a fin en 20–40 minutos sin bloqueos, con los **45 RF de
prioridad Alta** implementados y trazados a pruebas. Los dos restantes: RF-06 (omisión de
diálogos) de prioridad Media y RF-21 (iluminación progresiva) de prioridad Baja.

**Fuera de alcance** (declarado en el trabajo de grado, §1.6): medición del efecto pedagógico
en estudiantes, estudios experimentales con grupos de control, multijugador, IA, nube,
3D, plataformas distintas de Windows de escritorio, y el nivel avanzado opcional descrito
en §10 del guion (introduce presión de tiempo, que contradice CP-02).

---

## Mapa de capacidades

Los seis módulos ya están definidos en `OE1 §2.2`; se conservan sus letras y se les asigna
un id estable en kebab-case. Los ids —no los nombres de archivo— son el índice de qué existe.

| Módulo id | Letra | Responsabilidad | Depende de |
|---|---|---|---|
| `sistema-navegacion` | A | Inicio, perfil de jugador, menú de niveles, guardado, pausa, créditos, salida | — |
| `andamiaje` | B | Diálogos del guía, ayuda contextual, pista progresiva, retroalimentación, cierre reflexivo | `sistema-navegacion` |
| `nivel-fuego` | C | Nivel 1 «La Oscuridad» — iteración y depuración | `sistema-navegacion`, `andamiaje` |
| `nivel-rueda` | D | Nivel 2 «La Rueda» — abstracción, patrones, modelado, algoritmos | `sistema-navegacion`, `andamiaje` |
| `nivel-rio` | E | Nivel 3 «El Río» — descomposición y depuración | `sistema-navegacion`, `andamiaje` |
| `progreso-registro` | F | Registro de indicadores, resumen de nivel, consulta docente, eliminación de datos | `sistema-navegacion` — lee los indicadores del perfil persistido y no referencia ningún nivel (RNF-16; `Game.Reporting` → `Game.Core`) |

Las flechas apuntan en un solo sentido. Los tres niveles no se conocen entre sí: esa es la
condición que hace verificable RNF-16 (prueba de exclusión — retirar un nivel y comprobar
que los demás siguen ejecutándose). Los niveles tampoco alimentan a F por referencia: cada uno
registra sus indicadores con su recolector y los confirma en `PlayerProfile` (`Game.Core`); F los
lee del disco, así que retirar un nivel no rompe el informe docente. El resumen de nivel que ve
el estudiante es una pantalla de `Game.UI`.

### Orden de construcción — slice vertical

```
sistema-navegacion (mínimo) ─┐
                             ├─→ nivel-fuego (completo)   ← primer Golden Path
andamiaje (mínimo) ──────────┘
                             ↓
              nivel-rueda → nivel-rio → progreso-registro
                             ↑
   sistema-navegacion y andamiaje se completan al atravesar cada nivel
```

**Slice 1 (Golden Path temprano).** De pantalla de inicio a Nivel 1 terminado y su escena de
cierre: perfil con un solo nombre, menú con tres niveles (dos bloqueados), guardado al
completar fase, diálogo secuencial del guía, panel de encendido completo. Esto satisface el
KPI Golden Path de OE3 sobre un nivel real y valida el andamiaje pedagógico antes de
replicarlo dos veces.

**Slices 2 y 3.** `nivel-rueda` (tres fases encadenadas, cada una con su escenario) y
`nivel-rio`. Cada slice completa lo que le falte a A y B en vez de anticiparlo.

**Slice 4.** `progreso-registro`: los niveles ya emiten los indicadores; F los agrega,
los presenta al docente y añade la eliminación definitiva (RF-47, RNF-11).

El trabajo de cada slice vive en `claudeDocs/tasks/Slice N/` (`plan.md` + `todo.md`), escrito en
orden de dependencia y trazado a los ids de módulo de esta tabla. No hay un `SPEC-<id>.md` por
módulo: el plan del slice cumple ese papel y este documento es la parte compartida que ninguno
repite ni rediscute.

---

## Stack

| Elemento | Decisión |
|---|---|
| Motor | Unity 6000.5.10f1, plantilla 2D + URP 17.6.0 (CT-01) |
| Lenguaje | C# |
| Entrada | Input System 1.20.0 con un solo mapa de controles, `Assets/Game/Input/ControlesJugables.inputactions` (solo puntero: clic y clic sostenido, INC-01), declarado como mapa del proyecto y vigilado por `InputSchemeTest` — nunca la clase `Input` legada. El `InputSystem_Actions.inputactions` de `Assets/Settings/` es el de la plantilla y no lo usa nada |
| Datos de contenido | ScriptableObjects (CT-05, RNF-18) |
| Persistencia | JSON local, sin red (CT-07, RNF-10) |
| Pruebas | Unity Test Framework 1.7.0 (NUnit) |
| Destino | Windows 10+ 64 bits, carpeta portable, sin instalación ni internet (CT-03, RNF-07, RNF-08) |
| Control de versiones | Git; cada commit referido a su tarjeta del tablero Kanban (CT-11, RNF-17) |

**Presupuestos duros** (RNF-04..RNF-06): carga de escena < 10 s, memoria en ejecución < 2 GB,
paquete de distribución < 500 MB. Se verifican en el equipo de referencia, no se estiman.

**CT-02 — sin tarjeta gráfica dedicada.** El prototipo tiene que correr en los equipos de la
institución, que no la tienen. Es una restricción de aceptación, no una aspiración: acota las
opciones de URP (sin post-procesado costoso, sin luces 2D por objeto en masa) y se verifica
midiendo en un equipo sin GPU discreta, junto con los tres presupuestos de arriba.

---

## Comandos

La CLI oficial `unity` (`C:\Users\benab\AppData\Local\Unity\bin\unity`, v1.0.0-beta.5, ya en el
PATH) cubre **pruebas y build sin Editor**; el trabajo sobre escenas y assets sigue pasando por el
Editor abierto y sus MCP (`mcp__coplay-mcp__*` para escenas y assets, `mcp__rider__*` para compilar
y probar contra el Editor vivo).

| Tarea | Cómo |
|---|---|
| Ejecutar pruebas | `unity test --mode {EditMode\|PlayMode} --output <x>.xml --timeout 900 --no-banner --non-interactive`. Con Rider abierto, `mcp__rider__run_unity_tests` (skill `unity-coding-skills:run-tests`) corre contra el Editor sin cerrarlo. |
| Una sola prueba / assembly | `--filter`, que es una **expresión regular** contra `namespace.Clase.Método` y no un glob: `--filter "Game\.Core\.Tests"`, `--filter "LevelSelect_RF03"`. Un patrón que empiece por `*` aborta la corrida entera (`Quantifier {x,y} following nothing`, exit 2). Filtrar antes que correr la suite completa. |
| Build de entrega | `unity build --target StandaloneWindows64 -o "Build/Algoritmia/Algoritmia.exe" --log-file build.log --no-banner --non-interactive`. No hay Build Profile en `Assets/Settings/`, así que el destino lo fija `--target`. La carpeta de salida **es** el entregable portable: sobre ella se miden RNF-04, RNF-05 y RNF-06, y junto al `.exe` nace `Datos/` en la primera ejecución. Entran las escenas de `EditorBuildSettings`, con `Boot` de primera. |
| Leer errores de compilación | `mcp__coplay-mcp__check_compile_errors` / `mcp__coplay-mcp__get_unity_logs`, no abrir el Editor a ciegas. |
| Editar escenas y prefabs | Skill `unity-coding-skills:edit-scene` (nunca editar `.unity`/`.prefab` a mano sin ella). |
| Estado del puente | `unity status` lista los editores conectados —puerto, PID, estado—; `mcp__coplay-mcp__get_unity_editor_state` es la sonda real de si el puente está vivo. |

**`unity test` y `unity build` abren su propia instancia en batchmode: exigen el Editor cerrado**
(si no, `another Unity instance is running`, exit 6). Exit 0 = todo pasa, 2 = fallos o error de
invocación; `--report-format` acepta `nunit` **o** `junit`, uno a la vez. El flujo de una sesión
es: Editor abierto para trabajar escenas con coplay-mcp → cerrarlo → `unity test` → reabrir.

**Que un MCP no conteste no es que la suite pase.** `coplay-mcp` solo responde con el Editor
abierto y ya inactivo —mientras compila o hace domain-reload devuelve `timed out`—; `mcp__rider__*`
solo con Rider abierto y el proyecto cargado, y con Rider cerrado la sesión arranca con
`ConnectionRefused`. En cualquiera de esos casos las pruebas van por la CLI y el resultado se
**declara**, nunca se supone.

> Sin el puente Coplay no hay forma de manipular escenas: reiniciar el Editor y el puente antes de
> empezar cualquier slice.

---

## Arquitectura

Adoptada de `docs/arquitectura_videojuego_v2.docx`: **FSM + Scene Loader + capas**. La decisión
es correcta y su justificación se conserva — el flujo del juego es conocido y acotado desde el
inicio, que es el caso de uso exacto de una máquina de estados, y la organización en capas
permite repartir módulos como tarjetas Kanban sin bloqueos (CT-11, RNF-17).

El documento fue reemplazado por su versión alineada, que ya incorpora los tres ajustes que
siguen y traza cada decisión a un RF/RNF como exige CT-10. Se conservan aquí porque son el
contrato que el código debe cumplir, no solo el historial de una revisión.

### Estados: parametrizados, no enumerados uno a uno

El documento de arquitectura define ocho estados fijos (`Cinematica_Intro`, `Level_01`,
`Cinematica_01`…). No alcanza: el guion tiene unas quince escenas narrativas, y el Nivel 2 son
**tres escenas jugables encadenadas** (bosque, área de trabajo, laberinto — RF-22, RF-27,
RF-30), no una.

En vez de multiplicar estados, se parametrizan:

```csharp
public enum GameState { Boot, MainMenu, ProfileSelect, LevelSelect, Narrative,
                        Playing, LevelSummary, Credits, TeacherReport }
```

`Narrative` lleva el **id** de un `NarrativeSequence` (ScriptableObject) —el asset vive en
`Game.Scaffolding`, que depende de `Game.Core`, y lo resuelve el controlador de la escena en
`Game.UI`— y se reproduce en **una sola escena reutilizable**; `Playing` lleva un `LevelId` y una fase. Añadir una escena narrativa pasa a ser
un asset, no un estado, una escena y una rama del FSM. Es menos código y menos peso en el
paquete (RNF-06).

Tres consecuencias de esa parametrización que el código ya ejerce y conviene no volver a
discutir:

- **La fase es un tipo, no un `int`.** `PhaseId` (`Game.Core`) es lo que atraviesa
  `PlayerProfile`, `GameFlow` y el desbloqueo secuencial. No existe
  `ConfirmPhase(LevelId, int, …)`; el formato del JSON persistido no cambió por ello.
- **`GameFlow` acepta `Narrative → Narrative`**, y cuando se le pide una fase **ya confirmada**
  retoma en la **primera fase pendiente que tenga escena** (RNF-14). El runner le pasa su tabla
  de escenas; sin fase pendiente jugable repite la pedida, y `GameFlowRunner` cae al menú si una
  fase no tiene escena. Eso es lo que hace verificable la recuperación tras cierre forzado sin
  una rama por nivel.
- **`NarrativeSequence.NextSequenceId` encadena dos escenas del guion** —la 2.2 con la 2.3—
  desde el asset. Encadenar escenas narrativas no cuesta un `if` en el controlador.

**La capa de luz de las narrativas es propia.** `NarrativeLight` (`Game.Scaffolding`), junto
con `CameraKey.HardCut`, `FlashSeconds` y `LightStart` en los `N*_*.asset`, sobre el shader
`Algoritm/Oscuridad`. En el Nivel 1 es la oscuridad de la cueva, y su progresión en la mecánica
es `RF-21`, de prioridad Baja y acotado al nivel del fuego. En el Nivel 2 solo tiñe: el día pasa
del amanecer a la noche (`N2_PuenteI` → `N2_Escena25_Cierre` y el arranque de `N3_PuenteII`,
prueba `NarrativeSequence_RF05_ElNivel2TranscurreDelAmanecerALaNoche`); el laberinto, que no tiene
esa capa, tampoco se tiñe (decisión de Santiago, 07/10/2026): el entorno queda tal cual el arte, y
`Canvas/Fondo_Escena` y `MazeLayout.BackdropColor` son un solo ámbar `#E8A33D` (revierte el marfil
del día 07, decisión de Santiago, 08/10/2026), y el entorno y la tarjeta de la secuencia llevan el
mismo marco redondeado de 8 px `#C4A882`. El
diseño de los encuadres del N1 vive en
`docs/Camara_Narrativa_N1.md` (versionado); su inventario, `docs/md/Camara_Narrativa_N1.md`, se
perdió el 20/09/2026 y lo aplicado solo queda en los `N1_*.asset`.

### El FSM es C# plano

`GameFlow` no es un MonoBehaviour: es una máquina de estados sin dependencias de Unity, con un
`GameFlowRunner` delgado que traduce transiciones a `SceneLoader`. Así los recorridos del
Golden Path (RNF-13) se prueban en EditMode, sin escenas ni frames — que es lo que hace
pagable el «un caso de prueba por requerimiento» de CT-10.

### Comunicación: interfaz donde hay un consumidor, evento solo dentro de una escena

El documento propone un `EventBus` global para todo. Se acota:

- **Interfaz inyectada** cuando el consumidor es conocido: la pantalla que guarda recibe un
  `IProfileSaver` (`MainMenuController` al «Salir», `LevelSummaryController`), y sus pruebas le
  pasan un doble; el nivel en curso se registra como `ILevelReporter` en
  `GameFlowRunner.ActiveReporter` para que la pausa (`Game.UI`) le avise sin que interfaz y
  niveles se referencien. `ILevelReporter` solo lleva `PauseOpened`/`PauseClosed`.
- **Evento** solo dentro de una escena, cuando una pieza que crea el controlador le avisa de un
  arrastre o un clic (`DraggablePiece.PickedUp`/`Dropped`, `RaftPieceHandle.Taken`/`Released`).
  No hay evento de fase confirmada: quien cierra la fase llama a `ConfirmPhase` y a
  `Session.SaveActive()` en la misma llamada (RF-04).

Un bus genérico para todo convierte el flujo en algo que no se puede seguir leyendo el código,
justo lo contrario de por qué se eligió una FSM.

### Módulos por capa

Se conserva el reparto del documento, corrigiendo lo que no aplica a este juego.

| Capa | Componentes | Módulo |
|---|---|---|
| Core | `GameFlow` (FSM), `GameFlowRunner`, `SceneLoader`, `PlayerProfile`, `ProfileSession`, `SaveStore` | `sistema-navegacion` |
| Andamiaje | `DialogueRunner`, `NarrativeSequence`, `NarrativeVisitPolicy`, `ConditionalNarrativeTrigger`, `GuideContent`, `HintPolicy`, `IllustrationFraming`, `CharacterRig`, `FireGlow` (halo de las fogatas) | `andamiaje` |
| Gameplay | Un assembly por nivel y un controlador por escena jugable —`FirePanelController`; `ForestSceneController`, `WorkshopSceneController`, `MazeSceneController`; `RiverSceneController` y `AssemblyPanelController`— sobre clases de C# plano, con su recolector de indicadores (`FireIndicatorCollector`, `WheelIndicatorCollector`, `RiverIndicatorCollector`); el registro de mensajes `FireFeedbackLog` es del Nivel 1 | `nivel-*` |
| Reporting | `IndicatorReport`, `ProfileRepository`, `ReportContent` — solo depende de `Game.Core` (RNF-16) | `progreso-registro` |
| UI | Controladores de pantalla: menú principal y perfiles, menú de niveles, narrativa, resumen, pausa (RF-07), créditos, informe docente y confirmación de borrado; `FramedIllustration` encuadra la ilustración del inicio y de las tarjetas del menú de niveles | transversal |
| Audio | `AudioManager`, persiste entre escenas | transversal |
| Datos | ScriptableObjects de diálogo, tareas y configuración (CT-05) | transversal |

**Se descartan tres módulos del documento.** `EntityManager` («jugador, enemigos y
coleccionables») — no hay enemigos en este juego. `AssetLoader` sobre `Resources`/Asset
Bundles — referencias directas y ScriptableObjects bastan a esta escala, y `Resources` está
desaconsejado en Unity moderno. `CinematicsPlayer` con `VideoPlayer` — ver más abajo.

**Singletons con `DontDestroyOnLoad`, solo tres**, como propone el documento: `GameFlowRunner`,
`SceneLoader` y `AudioManager`. Ningún otro.

### Dos decisiones de la versión previa que contradecían requerimientos

**Las escenas narrativas no son video.** La versión previa proponía `VideoPlayer` con archivos en
`StreamingAssets`. **RF-05** especifica una ilustración fija que la cámara recorre, con
personajes y objetos animados sobre ella y cuadros de diálogo secuenciales, sin video (corregido
el 29/09/2026: decía «ilustraciones estáticas»), y el guion §1.2 lo confirma. Video comprometería
además RNF-06 (< 500 MB) y RNF-04 (carga < 10 s). Se implementa como `DialogueRunner` sobre
ilustración fija —encuadres de `IllustrationFraming`, personajes con `CharacterRig`, objetos con
`NarrativeProp`, luz con `NarrativeLight`, halo de las fogatas con `FireGlow`—, con avance por clic y botón de omitir (RF-06).

**La persistencia no usa `Application.persistentDataPath`.** Escribe en `%AppData%\LocalLow`,
fuera de la carpeta portable: choca con RNF-07 y con el criterio de verificación de RNF-11,
que exige «ausencia de residuos en el almacenamiento local» tras eliminar un perfil. Se
mantiene el supuesto 1: JSON en `Datos/` junto al ejecutable.

**Y dos que contradecían criterios pedagógicos**, heredadas del molde arcade genérico:
mencionaba pantalla de *Game Over* y *puntajes* en HUD y `SaveSystem`. **CP-02**
prohíbe pantallas de derrota y **CP-03** prohíbe puntajes; RF-17 prohíbe cifras en la
retroalimentación. No se implementan. Tampoco hay un `InputHandler` que abstraiga gamepad: la
entrada son los eventos de puntero de uGUI sobre el Input System y se limita a **clic y clic
sostenido**; la única excepción es escribir el nombre al crear un perfil (CT-06, RNF-02, INC-80). El desplazamiento del
personaje en el Nivel 3 usa botones de dirección **en pantalla**, accionados con clic sostenido — no el
teclado (RF-35, guion §2.1/§8.2, CU-09; INC-01, cerrado).

---

## Estructura del proyecto

```
Assets/
  Game/
    Scripts/
      Runtime/
        Core/            → FSM, SceneLoader, perfil, guardado          [sistema-navegacion]
        Scaffolding/     → diálogo, ayuda, pista progresiva, cierre     [andamiaje]
        Levels/
          Fire/          → nivel-fuego
          Wheel/         → nivel-rueda
          River/         → nivel-rio
        Reporting/       → progreso-registro
        UI/  Audio/      → pantallas (menús, perfiles, narrativa, resumen, pausa, informe docente); audio persistente
      Editor/            → Game.EditorTools, solo Editor: PlayFromBoot, ArtImportRules, AudioImportRules, ClaudeSceneAutosave, Sandbox/
    Data/                → ScriptableObjects: Fire/ Wheel/ River/ Narrative/ Guide/ Reporting/, más GameTitleConfig, CreditsContent, LevelSummaryMessages y ProfileSelectContent
    Prefabs/             → Characters/ (rigs de la familia y los tres Algoritm), UI/ (MenuPausa, en las cinco escenas jugables)
    Scenes/
      Boot.unity  MainMenu.unity  LevelSelect.unity  Credits.unity
      TeacherReport.unity
      Narrative.unity          ← única, parametrizada por NarrativeSequence
      LevelSummary.unity       ← única, cierre de cualquier nivel (RF-45, sin cifras)
      Level1_Cave.unity
      Level2_Forest.unity  Level2_Workshop.unity  Level2_Maze.unity
      Level3_River.unity
    Art/  Audio/         → assets propios o con autorización escrita (CT-09, RNF-23)
    Input/               → ControlesJugables.inputactions, el único mapa de controles (RNF-02, CT-06)
  Settings/              → URP 2D, Renderer2D (de la plantilla)
  Tests/
    EditMode/            → lógica pura, una carpeta por módulo, más Architecture/ (lee .asmdef y ajustes del disco), Content/ (barre el contenido de los tres niveles) y EditorTools/
    PlayMode/            → escenas, UI, integración (Scaffolding y Reporting no tienen assembly de PlayMode)
docs/                    → documentos fuente del trabajo de grado (no editar desde código)
claudeDocs/              → SPEC.md (este contrato), INCONSISTENCIAS.md (hallazgos), Direccion_de_Arte.md, Direccion_de_Musica_y_Sonido.md, Interfaces.md, Camara_Narrativa_N2.md y los mockups de interfaz (HTML)
  tasks/
    Slice 1/  Slice 2/  Slice 3/  Slice 4/  Personajes/  OE4/
      plan.md            → plan técnico del slice
      todo.md            → tablero de tareas del slice
```

Los namespaces siguen la ruta relativa a `Scripts`, elidiendo `Runtime`:
`Assets/Game/Scripts/Runtime/Levels/Fire/FirePanel.cs` → `namespace Game.Levels.Fire`.

**Assemblies** (`.asmdef`), uno por módulo con dependencia unidireccional:
`Game.Core`, `Game.Scaffolding`, `Game.Levels.Fire`, `Game.Levels.Wheel`,
`Game.Levels.River`, `Game.Reporting`, `Game.UI` y `Game.Audio`, más su assembly de pruebas de
EditMode (`<Módulo>.Tests`) y, salvo `Game.Scaffolding` y `Game.Reporting`, uno de PlayMode
(`<Módulo>.PlayMode.Tests`); aparte están `Game.Architecture.Tests`, `Game.Content.Tests` y
`Game.EditorTools.Tests`. Ningún assembly de nivel referencia a otro assembly de nivel: eso es lo que hace
ejecutable la prueba de exclusión de RNF-16.

Fuera de esa cadena cuelga **`Game.EditorTools`, solo Editor**, que **no referencia ningún
`Game.*`**: trae `PlayFromBoot` —que fuerza el arranque en `Boot` al pulsar Play, y se suspende
solo en batchmode y durante las corridas del Test Runner—, las reglas de importación
`ArtImportRules` y `AudioImportRules` (RNF-23, RNF-06), `ClaudeSceneAutosave` y la maqueta
`Sandbox/CharacterProbe*`, que no es código del juego. No entra en el paquete de entrega y por eso no cuenta para RNF-16.

`Game.UI` y `Game.Audio` están en la lista de la arquitectura §9 (INC-40, cerrado): los
controladores de pantalla y `AudioManager` tienen que compilar en algún sitio, y meterlos en `Game.Core` haría que el
núcleo dependiera de la UI — justo lo que impediría probar `GameFlow` en EditMode sin escena.
**Dependen de `Game.Core`; nunca al revés.**

---

## Estilo de código

Rige `unity-coding-skills:code-writing-guide` — cargarla antes de tocar cualquier `.cs`.
Lo que este proyecto añade encima:

**Identificadores en inglés, contenido en español.** El texto que ve el estudiante vive en
ScriptableObjects, nunca incrustado en una clase.

**La lógica del nivel es C# plano; el MonoBehaviour es un adaptador delgado.** Toda máquina
de estados, validador de secuencia y contador se prueba en EditMode sin escena ni frames.
El MonoBehaviour solo traduce clics a llamadas y estado a UI.

```csharp
namespace Game.Levels.Fire
{
    /// <summary>Estado del panel de encendido del Nivel 1. Sin dependencias de Unity.</summary>
    public class FireAttempt
    {
        private readonly FireLevelConfig _config;
        private int _effectiveStrikes;
        private int _consecutiveFailures;

        public FireAttempt(FireLevelConfig config) => _config = config;

        public int EffectiveStrikes => _effectiveStrikes;
        public bool CanBlow => _effectiveStrikes >= _config.MinimumEffectiveStrikes;
        public bool ShouldOfferHint => _consecutiveFailures >= _config.AttemptsBeforeHint;

        public ForceBand Classify(int force) =>
            force < _config.EffectiveForceMin ? ForceBand.TooSoft
            : force > _config.EffectiveForceMax ? ForceBand.TooHard
            : ForceBand.Effective;

        /// <summary>Resuelve un golpe con la fuerza y la cercanía dadas y devuelve lo observado.</summary>
        public StrikeOutcome Strike(int force, SpacingBand spacing = SpacingBand.Effective)
        {
            var band = Classify(force);
            if (spacing != SpacingBand.Effective)
            {
                _consecutiveFailures++;
                return StrikeOutcome.StonesMisplaced(force, band, spacing); // sin choque no hay chispa
            }

            if (band != ForceBand.Effective)
            {
                _consecutiveFailures++;
                return StrikeOutcome.SparksDied(force, band);
            }

            // Lo ganado permanece: un fallo posterior nunca reduce el contador (guion §1.4.3.6).
            _effectiveStrikes++;
            _consecutiveFailures = 0;
            return StrikeOutcome.SparkLanded(force, _effectiveStrikes);
        }
    }
}
```

Los parámetros ajustables jugando van en ScriptableObject con `[field: SerializeField]` y
`[Tooltip]`, nunca como literal en el código (CT-05, RNF-18):

```csharp
[CreateAssetMenu(menuName = "Algoritm/Configuración del nivel fuego", fileName = "N1_Config")]
public class FireLevelConfig : ScriptableObject
{
    [field: SerializeField, Tooltip("Muesca máxima del deslizante de fuerza (va de 0 a este valor).")]
    public int ForceLevels { get; set; } = 10;

    [field: SerializeField, Tooltip("Fuerza mínima con la que un golpe cuenta como efectivo.")]
    public int EffectiveForceMin { get; set; } = 7;

    // … EffectiveForceMax, SpacingLevels, EffectiveOverlap, OverlapTolerance, GatherZoom,
    // GatherSeconds, IgnitionSeconds, BurnExtent …

    [field: SerializeField, Tooltip("Golpes efectivos necesarios para habilitar el soplo.")]
    public int MinimumEffectiveStrikes { get; set; } = 3;

    [field: SerializeField, Tooltip("Fallos consecutivos tras los cuales el guía ofrece pista.")]
    public int AttemptsBeforeHint { get; set; } = 3;
}
```

**Comentarios «por qué no».** Cuando se rechaza el camino obvio, decir por qué en el código.
En este proyecto la razón suele ser un criterio pedagógico (CP-02 prohíbe penalizar, CP-03
prohíbe puntajes) y no una restricción técnica — dejarlo escrito evita que una futura
«mejora» reintroduzca una pantalla de derrota.

**Una tarea activa a la vez** (RNF-03) es una restricción de diseño de la UI, no una sugerencia.

---

## Estrategia de pruebas

Unity Test Framework. Rigen `unity-coding-skills:test-designing-guide` y `test-writing-guide`.

| Nivel | Dónde | Qué cubre |
|---|---|---|
| EditMode (unitarias) | `Assets/Tests/EditMode/<Módulo>/` | Máquinas de estado, validadores de secuencia, contadores, desbloqueos condicionales, selección de mensaje narrativo, serialización del perfil. Sin escena, sin frames. |
| PlayMode (integración) | `Assets/Tests/PlayMode/<Módulo>/` | Cableado de escena, flujo de UI, guardado y recarga, desbloqueo de nivel, recorridos del Golden Path. `[Category("Integration")]`. |
| Aserción de layout | PlayMode | Elemento dentro de pantalla, sin solapamientos, texto sin desbordar, botón alcanzable por raycast. `[Category("Integration")]`. |
| Aceptación | EditMode y PlayMode | El criterio de aceptación de un RF, RNF o HU aseverado de extremo a extremo; se suma a la categoría de su nivel. El contraste texto/fondo (RNF-20) de los tres niveles se asevera así: razón ≥ 4.5:1 sobre los colores renderizados. `[Category("Acceptance")]`. |
| Verificación visual | PlayMode | Solo lo que no admite aserción estricta, sobre capturas guardadas: doble indicador leído en escala de grises (RNF-19), ausencia de destellos rápidos (RNF-21) y la lectura del texto en las pantallas cuyo contraste no se mide (menú principal, laberinto, taller). `[Category("VisualVerification")]`. |
| Manual | Plan de pruebas OE4 | Presupuestos de rendimiento, ejecución portable en dos equipos, ejecución sin red, cierre forzado y recuperación. |

**Probar sin ampliar la superficie pública.** Un `AssemblyInfo.cs` en la raíz del módulo con
`[assembly: InternalsVisibleTo("<Módulo>.Tests")]`. Nunca subir un miembro a `public` solo para
que lo alcance una prueba. **El assembly de PlayMode se llama `<Módulo>.PlayMode.Tests` y
necesita su propia línea**: sin ella el `internal` no se ve desde PlayMode aunque la de EditMode
esté puesta. `Game.Architecture.Tests` es la excepción a todo lo anterior —no referencia ningún
`Game.*` porque lee los `.asmdef` del disco, y así puede exigir el assembly de un módulo que
todavía no tiene código (RNF-15, RNF-16).

**Regla de trazabilidad (CT-10): todo RF tiene al menos un caso de prueba que lo nombra.**
El nombre del método de prueba cita el identificador, de modo que la matriz del plan de
pruebas de OE4 se pueda derivar de la suite en vez de mantenerse a mano.

Los cuatro invariantes pedagógicos se prueban explícitamente en cada nivel, no se asumen:

1. **No existe pantalla de derrota** ni límite de intentos (CP-02, RF-18, RF-42).
2. **La fase aprobada nunca se pierde** tras un fallo posterior (RF-41, RF-43).
3. **La retroalimentación es narrativa**: sin cifras, sin juicios de valor, y no repite el
   mismo mensaje dos veces seguidas cuando hay alternativa aplicable (CP-03, RF-17). El
   **resumen de fin de nivel que ve el estudiante tampoco lleva cifras** (RF-45): es el punto
   donde más fácil se cuela una, y donde HU-14 ya lo hizo (INC-26).
4. **El andamiaje orienta, no resuelve** (CP-06): la ayuda a demanda repite la instrucción
   vigente sin alterar el estado, y la pista tras tres fallos nunca nombra la respuesta —en el
   Nivel 1, nunca la fuerza ni la cercanía efectivas (guion §4.3.6).

Flujo de trabajo test-first por slice: `plan-feature` → `test-designer` → `failing-test-writer`
→ implementación → refactor y deduplicación. Para defectos, `fix-bug` (reproducir → diagnosticar
→ corregir).

---

## Límites

**Siempre**
- Cargar la skill de `unity-coding-skills` correspondiente antes de escribir código, tocar
  escenas o correr pruebas.
- Escribir la prueba antes que la implementación, y verla fallar.
- Externalizar a ScriptableObject todo texto visible y todo parámetro ajustable jugando.
- Mantener la lógica del nivel en C# plano, probable sin escena.
- Acompañar toda señal por color con un segundo indicador — icono, texto o forma (RNF-19).
- Redactar los textos del estudiante a nivel lector de grado cuarto: máximo 20 palabras por
  oración, sin tecnicismos sin explicar (RNF-01).
- Mantener toda instrucción en **máximo dos líneas** y acompañar todo texto instruccional con
  **refuerzo icónico** (CP-08). Es distinto del doble indicador de RNF-19: aquel acompaña al
  color, este acompaña a la instrucción. Se decide al crear el ScriptableObject, no después.
- Ofrecer los dos mecanismos de andamiaje por separado: ayuda a demanda que repite la
  instrucción vigente sin alterar el estado, y pista automática tras tres fallos consecutivos.
- Asociar cada commit a su tarjeta del tablero Kanban (RNF-17).

**Preguntar primero**
- Añadir un paquete a `Packages/manifest.json`.
- Cambiar el formato de datos persistidos del jugador, o dónde se guardan.
- Modificar un RF, un RNF o un criterio de aceptación de una HU: son entregables ya
  radicados del trabajo de grado, no notas internas.
- Introducir una mecánica que no esté en el guion, o retirar una que sí esté.
- Ampliar el esquema de control más allá de clic y clic sostenido (CT-06, RNF-02). Los botones
  de dirección del Nivel 3 están **dentro** de ese esquema: son UI accionada con clic.

**Nunca**
- Pantalla de derrota, puntaje, cuenta regresiva, penalización o pérdida de progreso confirmado.
- Mostrar cifras al estudiante —intentos, tiempo, pasos— ni en la retroalimentación ni en el
  resumen de fin de nivel. Las cifras existen, pero son del informe docente (RF-46).
- Almacenar más que nombre o alias, progreso de avance —nivel alcanzado y fases confirmadas— e
  indicadores de desempeño. Nada de imágenes, ubicación, contacto ni datos sensibles (RNF-09; el
  progreso hoy no está en su letra, ver INC-27).
- Transmitir dato alguno por red, ni requerir internet en ejecución (RNF-08, RNF-10).
- Usar la clase `Input` legada.
- Incluir violencia explícita, publicidad, compras integradas o enlaces externos (RNF-22). Se
  verifica por inspección integral del contenido en el cierre del proyecto, no por prueba.
- Incluir un asset gráfico o sonoro sin autoría propia ni autorización escrita, o sin su
  reconocimiento en la pantalla de créditos (CT-09, RNF-23).
- Editar los `.docx` de `docs/` desde el código.
- Crear archivos `.meta` a mano — los genera el Editor.

---

## Criterios de éxito

Verificables, uno por KPI del trabajo de grado (§2.3):

1. **OE1 — Trazabilidad.** El 100 % de los RF implementados está asociado a una faceta del
   pensamiento computacional o a un lineamiento, según las matrices de OE1 §5.1 y OE2 §3.1–3.4.
   Verificado el 30/08/2026 sobre los 47 RF: se cumple.
2. **OE1 — Criterios pedagógicos.** Al menos el 80 % de los diez criterios CP está explícitamente
   integrado en el diseño. Los diez tienen hoy al menos un RF que los materializa (OE2 §3.2), y
   cuatro de ellos —CP-02, CP-03, CP-06 y CP-07— se verifican además con pruebas automatizadas.
3. **OE2 — Progresión.** Los tres niveles están diseñados e implementados con dificultad
   ascendente y desbloqueo secuencial (RF-03).
4. **OE2 — Alineación narrativa.** Más del 85 % de los retos narrativos exige una acción de
   pensamiento computacional para resolverse (CP-10).
5. **OE3 — Mecánicas.** Al menos tres mecánicas principales implementadas: panel de hipótesis
   e iteración (N1), selección por patrón, ensamblaje secuencial y editor de bloques (N2),
   movimiento, recolección y ensamblaje por fases (N3).
6. **OE3 — Golden Path.** Cada nivel se completa de principio a fin sin bloqueos, cierres
   inesperados ni estados irrecuperables; dos recorridos completos por nivel sin incidencias
   (RNF-13).
7. **OE4 — Pruebas funcionales.** 90 % de la funcionalidad verificada, con caso de prueba por
   requerimiento (CT-10).
8. **OE4 — Requerimientos críticos.** La totalidad de los RF de prioridad **Alta** está
   implementada en la entrega final.
9. **Presupuestos.** Carga < 10 s, memoria < 2 GB, paquete < 500 MB, ejecución portable
   comprobada en dos equipos distintos y con el adaptador de red deshabilitado.
10. **Datos.** La eliminación de un perfil es irreversible, exige confirmación explícita y no
   deja residuos en el almacenamiento local (RF-47, RNF-11).

---

## Decisiones sobre los documentos en conflicto

`INCONSISTENCIAS.md` (rev. 15, 30/09/2026) registra los hallazgos `INC-01`..`INC-117` entre los
`.docx` y entre ellos y el juego. **Todos están cerrados en los documentos**: `INC-43`/`INC-44`/`INC-45`
desde la refundición del 14/09/2026, `INC-46`..`INC-114` el 29/09/2026 e `INC-115`..`INC-117` el
30/09/2026, cuando los `.docx` se
alinearon con el juego y se implementó lo que solo pedían los documentos. Ya no hay divergencias
deliberadas entre documento y código. Los que eran el código adelantándose al documento quedaron así:

- **`INC-46`** — la lista de tareas del Nivel 3 es la tablilla de marfil con un círculo por tarea que
  implementa el juego; se corrigió `Direccion_de_Arte.md` §10.2.
- **`INC-47`** — la mecánica del Nivel 1 (reunir y encender con fuerza y cercanía): ver el supuesto 7.
- **`INC-48`** — la §4 del documento refundido resume la arquitectura que implementa el código, con
  remisión a `arquitectura_videojuego_v2 (2).docx`, que también se alineó (INC-94..INC-96).
- **`INC-49`** — el menú de pausa del **mockup 6** (Reanudar · Reiniciar · Volver al menú de niveles)
  está ya en RF-07 y HU-17; «Reiniciar» repite la fase activa sin repetir la narrativa.
- **`INC-50`** — «Empujar» confirma la fase y sale a la escena 2.2, que anima el rodado; RF-26,
  HU-08, CU-06 y el guion ya lo describen así.

En todo lo demás el código sigue sencillamente lo que dicen los documentos.
Lo que el código materializa de cada decisión, para que no se pierda al leer solo el `.docx`:

| Hallazgo (cerrado) | Lo que el código materializa |
|---|---|
| INC-25 · HU-17 | Pausa como capa de UI sobre `Playing`, no un estado nuevo: `Time.timeScale = 0` mientras está abierta. Rótulos del mockup 6 (INC-49, cerrado): Reanudar (restituye el estado exacto), Reiniciar (confirmación, repite **la fase activa** sin repetir la narrativa, nunca re-bloquea un nivel desbloqueado, no borra indicadores; en el Nivel 3, cuyas tres fases comparten escena, `AssemblyPanelController` registra la fase activa al confirmar cada una con `GameFlow.SetPlayingPhase`, INC-117), Volver al menú de niveles. Un solo prefab `MenuPausa` en las cinco escenas jugables. Sin `GameOver`. |
| INC-26 · HU-14 | El resumen de fin de nivel es **narrativo y sin cifras** (RF-45, RF-17, CP-03). Las cifras solo viven en `TeacherReport` (RF-46); no hay `ScoreManager`. |
| INC-01 · controles | **Botones de dirección en pantalla, accionados con clic sostenido.** Nunca teclado. El mapa de controles se inspecciona sin salvedades (RNF-02, CT-06). |
| INC-27 · RNF-09 | Se persisten nombre o alias, nivel alcanzado, fases confirmadas y los cuatro indicadores. Nada más. |
| INC-28 · omisión | El botón de omitir aparece **solo si la escena ya fue vista** (RF-06). El cierre reflexivo no se omite la primera vez (CP-07, RF-12). En el Nivel 3 el cruce y la escena final no se omiten nunca, tampoco al repetirlo: sin nivel siguiente, el perfil no distingue la primera vuelta (INC-51, HU-14 FA-01). |
| INC-29 · HU-10 | El Nivel 2 fase 3 emite exactamente los cuatro indicadores de OE1 §3.6.1, con su definición operativa. |
| INC-30 · Nivel 3 | Lista de **cuatro tareas** (RF-36). Ensamblaje de **tres fases** (RF-40). Tarea 3 se marca al confirmar la fase de amarre; tarea 4, al confirmar mástil y vela; la fase de base no marca tarea por sí sola. |
| INC-32 · «Soplar» | Una vez habilitado, «Soplar» **no** vuelve a deshabilitarse: lo ganado permanece (guion §4.3.6, CP-02). El desbloqueo depende solo del número de golpes efectivos. |
| INC-33 · bloques del laberinto | «Avanzar ×n» y «Retroceder ×n» (n de 1 a 9) son relativos a la orientación de la carretilla; «Girar» rota 90° a la izquierda o a la derecha, según el lado elegido (mockup 10, 13/09/2026; INC-55). Con la lectura absoluta el refugio podía ser inalcanzable. |
| INC-34 · persistencia | La eliminación de un perfil borra `Datos/` **y** la ruta de respaldo; la prueba de RNF-11 corre en los dos escenarios (`Datos/` escribible y de solo lectura). |
| INC-35 · informe docente | `TeacherReport` presenta los indicadores **por nivel y por fase** (RF-45, RF-46). |
| INC-37 · RF-44 / RF-46 | `RF-44` (cruce y cierre del juego) trazado a HU-13; `RF-46` (consulta docente) a HU-16. La numeración de historias sigue cerrada en HU-01..HU-18. |
| INC-39 · fin del juego | Tras el Nivel 3: `LevelSummary` → `Narrative` (escena final, guion §9) → `Credits` → `MainMenu` (RF-44, RF-08). |
| INC-40 · assemblies | Existen `Game.UI` y `Game.Audio`, dependientes de `Game.Core` y nunca al revés. |
| INC-41 · lista de tareas | La lista permanente es del Nivel 3 (RF-36); los niveles 1 y 2 no tienen lista. RNF-03 restringe la tarea **activa**, no cuántas se muestran. |
| INC-16, 21, 22, 24, 31, 36, 38, 42 | Correcciones de coherencia documental sin efecto en el código (trazas de `RNF-18`, `CN-04→RNF-20`, condición de personajes en la introducción, paginación y cabecera, actores de HU, datos internos del trabajo de grado, referencia a `TRAZABILIDAD.md`, norma de citación IEEE). |

**Residuo cosmético:** HU-17 y HU-18 no llevan el encabezado «Página 17/18 de 18» (se añadieron
sin él). No afecta a ningún criterio de verificación.

---

## Supuestos

Corregir cualquiera de estos ahora sale más barato que después.

1. **Persistencia**: un archivo JSON por perfil en una carpeta `Datos/` junto al ejecutable, para
   que «portable» y «sin residuos» (RNF-07, RNF-11) signifiquen lo mismo: borrar la carpeta borra
   todo. Si la ruta no es escribible, se cae a `Application.persistentDataPath` y se advierte al
   docente. Ya no es solo un supuesto: la arquitectura §7 lo adopta con esa justificación e
   incluye que **la eliminación de un perfil borra las dos rutas** y que la prueba de RNF-11 corre
   en los dos escenarios (INC-34, cerrado).
2. **Guardado automático** al confirmar cada fase, no en cada acción (RF-04), que es lo que hace
   verificable la recuperación tras cierre forzado (RNF-14). Los cuatro indicadores de OE1 §3.6.1
   se persisten en ese mismo punto. Los Niveles 2 y 3 confirman cada fase al completarla; la única
   fase del Nivel 1 y el desbloqueo del nivel siguiente, en los tres, los hace `LevelSummary`
   después del cierre reflexivo (arquitectura §7, corregida el 29/09/2026). Si un cierre forzado
   impide llegar al resumen de un nivel con todas sus fases confirmadas, el menú de niveles deriva
   el desbloqueo de esas fases (`LevelUnlockPolicy.IsUnlocked`), sin campo nuevo en el perfil
   (RNF-09, INC-116, 30/09/2026).
3. **Los personajes son obra derivada de los diseños de la Familia Anonaky, con autorización
   escrita concedida** (PG-07 cerrado el 30/08/2026). Se rediseñaron —proporciones, vestuario,
   paleta y rasgos propios— pero **partieron** de esos personajes, y cambiar el diseño no
   extingue el derecho del autor original: por eso el permiso hacía falta, y por eso se pidió.
   Consecuencias vigentes: el reconocimiento expreso en la pantalla de créditos es **obligatorio**
   (CT-09, RNF-23, trabajo de grado §3.3.2 y §5.2), y la constancia escrita se archiva con los
   anexos del trabajo de grado. Lo que sí es **original del proyecto** y no depende de PG-07:
   entornos, props, interfaz, tipografía, efectos y animación (`Direccion_de_Arte.md` §19).
4. **Raíz de assets** `Assets/Game/` y namespace `Game.*`: el título del producto, «Algoritmia»
   (PG-01, cerrado), vive en `GameTitleConfig` y en el `productName` de Unity (junto al
   `companyName` «Universidad Catolica de Colombia», 29/09/2026), no en rutas ni namespaces.
5. **El guía se llama Algoritm** (decisión del 02/09/2026, `PG-02` cerrado; INC-44), siguiendo el guion, que es el único documento con
   escena de origen y caracterización visual (PG-02). Al vivir en ScriptableObjects, el nombre se
   cambia sin tocar código.
6. **Nivel 3 usa botones de dirección en pantalla**, accionados con clic sostenido: Mamá avanza mientras se mantiene pulsado y se detiene al soltar — no el teclado. Letra
   vigente en todos los documentos: RF-35, guion §2.1 y §8.2, CU-09, HU-11 y arquitectura §1.
   **No hay excepción alguna a RNF-02 ni a CT-06** (INC-01, cerrado).
7. **La mecánica del Nivel 1 mide fuerza y cercanía, no posición** (rediseño del 12/09/2026,
   T20–T24, ampliado el 15/09/2026, T25–T27; `INC-47`, cerrado el 29/09/2026 al alinear el guion
   §1.4.3–§1.4.4, OE1 `RF-14`–`RF-16` y §3.6.1, HU-04..HU-07 y CU-05 con el juego). Primero el
   estudiante **reúne** hojas y piedras en el círculo del centro (la ayuda lo dibuja); reunido todo,
   la cámara se acerca al doble, las hojas se funden en el montón y fija dos hipótesis en dos
   deslizantes de 0 a 10 —la **fuerza** del golpe y la **cercanía** de las piedras—, golpea y sopla.
   La tablilla muestra solo el último mensaje. Los nombres de las pruebas conservan `RF-15`/`RF-16`,
   que ahora enuncian lo mismo: hipótesis → experimento → resultado observable → ajuste.

   Valores vigentes en `FireLevelConfig` (`N1_Config.asset`), **ninguno validado jugando todavía**
   (`PG-06`, abierto: exige jugar con estudiantes, fuera del OE4):

   | Campo | Valor |
   |---|---|
   | `ForceLevels` | 10 |
   | `EffectiveForceMin` · `EffectiveForceMax` | 7 · 8 |
   | `SpacingLevels` | 10 |
   | `EffectiveOverlap` · `OverlapTolerance` | 30 · 4 (px del lienzo de 1920×1080; con las piezas actuales, solo la muesca 5) |
   | `GatherZoom` · `GatherSeconds` | 2 · 0.8 |
   | `IgnitionSeconds` · `BurnExtent` | 3.5 · 0.5 |
   | `MinimumEffectiveStrikes` | 3 |
   | `AttemptsBeforeHint` | 3 |

   Viven en un ScriptableObject (CT-05, RNF-18) para que ajustarlos no cueste una recompilación.
   La pista tras tres fallos **nunca nombra la fuerza ni la cercanía efectivas** (CP-06).
8. **Los bloques del laberinto** son relativos a la orientación de la carretilla: «Avanzar ×n» y
   «Retroceder ×n» (n de 1 a 9) mueven n casillas adelante o atrás respecto de esa orientación, y
   «Girar» rota 90° al lado elegido, izquierda o derecha (decisión de Santiago, 13/09/2026,
   mockup 10). Letra vigente en RF-31 y en el guion §1.6.3.2 (INC-33, cerrado; ajustada el
   29/09/2026 por INC-55). Con la lectura absoluta ninguna secuencia se desplazaba en vertical.
9. **Se guarda el progreso de avance** —nivel alcanzado y fases confirmadas— además del nombre y
   los cuatro indicadores. RF-03, RF-04 y ahora también RNF-09 lo contemplan (INC-27, cerrado).
10. Los equipos de la institución tienen Windows 10 o superior con audio funcional, y el docente
    acompaña la sesión.
11. **El Nivel 3 tiene tres fases de ensamblaje y cuatro tareas visibles.** Las tareas 1 y 2 se
    marcan al recoger troncos y sogas; la 3 al confirmar la fase de **amarre**, que es la que cierra
    la estructura de la balsa; la 4 al confirmar mástil y vela. La fase de base no marca tarea por
    sí sola. La correspondencia está fijada en el guion §8.1/§8.2 y en HU-11 (INC-30, cerrado).
12. **`Game.UI` y `Game.Audio` son assemblies propios**, dependientes de `Game.Core`. Listados en
    la arquitectura §9 (INC-40, cerrado): `Game.Core` no puede depender de la UI sin perder su
    testabilidad en EditMode.

Letra vigente de los documentos: el resumen de fin de nivel que ve el estudiante **no lleva
cifras** y las cifras son del informe docente, por nivel y por fase (RF-45, RF-46); **RF-46 es de
prioridad Alta**, así que la consulta docente y la eliminación de datos entran juntas en el mismo
slice; el botón de ejecutar del laberinto usa **clic simple** (PG-04 cerrado, RF-32); golpear
con demasiada fuerza **produce chispas visibles que se apagan** lejos de las hojas, con poca no
saca chispa, y con las piedras separadas o demasiado encimadas tampoco (PG-03 cerrado, RF-16;
guion §4.3.3/§4.3.4 y HU-06, alineados por `INC-47`); y la **definición operativa de los cuatro indicadores**
por nivel está fijada en OE1 §3.6.1, con lista cerrada.

---

## Preguntas abiertas

Ningún hallazgo de `INCONSISTENCIAS.md` queda abierto (rev. 15, 30/09/2026).

**Del guion (§1.2), sin resolver** — son del guion, no conflictos entre documentos, y los dos
exigen observar a estudiantes jugando, que el OE4 no hace: **PG-05** verificar en pruebas que el
cambio de esquema de control entre el Nivel 1 y el 2 no confunde · **PG-06** validar jugando los
valores del Nivel 1 (`FireLevelConfig`). Cerrados: ~~**PG-01** título del producto~~ **«Algoritmia»**
(09/09/2026, `GameTitleConfig`) · ~~**PG-02** nombre definitivo del guía~~ **Algoritm**
(02/09/2026, INC-44, INC-45).

**PG-07 cerrado (30/08/2026):** la autorización escrita de los personajes de la Familia Anonaky
fue concedida, y desde la refundición del 14/09/2026 el `.docx` ya lo refleja (INC-43, cerrado).

**Residuos cosméticos en `docs/`, corregidos el 29/09/2026:** el encabezado «Página 17/18 de 18»
de HU-17 y HU-18 (`INC-24-r`), «Chispa» en el estado E5 de `Solucion_OE2_Diseno_final` §1.4.3
(`INC-44-r`) y la cláusula «Nombre provisional» de §1.1.1 (`INC-44-r2`).

**Cerrado desde la rev. 4:** la semántica de los bloques del laberinto (lectura relativa, cuenta
de 1 a 9 y giro a los dos lados, fijada en RF-31 y guion §1.6.3.2 — INC-33 e INC-55); `RF-19` sin condición de posición
(INC-32); `RNF-09` admite el progreso de avance (INC-27); la restricción «escena ya vista» de
`RF-06` en todos los documentos (INC-28); `RF-44` y `RF-46` trazados a HU-13 y HU-16, con la fila
de historia asociada en CU-11 (INC-37); la definición operativa de los cuatro indicadores por
nivel y por fase, con lista cerrada (OE1 §3.6.1); y el alcance de las sesiones con estudiantes,
verificación funcional y de usabilidad sin medición del efecto pedagógico (trabajo de grado §1.6
y §5.3).
