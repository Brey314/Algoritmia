# Spec: Prototipo de videojuego educativo — Pensamiento Computacional

Contrato de desarrollo derivado de los seis documentos en `docs/`. Los identificadores
RF/RNF/CP/CT/CN/CU/HU/PG remiten a esos documentos y son la unidad de trazabilidad (CT-10).

**Orden de precedencia.** Cuando dos documentos se contradicen gana el de mayor prioridad, y se
corrige el otro. Verificado contra su estado del 30/08/2026 (rev. 5):

| # | Documento | Qué gobierna |
|---|---|---|
| 1 | `Trabajo_de_Grado_2026_ICONTEC_IEEE.docx` | Objetivos, KPI, alcance, marco jurídico, metodología Árcade |
| 2 | `OE1_Requerimientos (3).docx` | Lineamientos CP/CT/CN, RF-01..RF-47, RNF-01..RNF-23 |
| 3 | `Guion_Completo_Videojuego.docx` | Narrativa, mecánicas, parámetros y textos exactos |
| 4 | `OE2_historias_completas.docx` | CU-01..CU-12, HU-01..HU-18, matrices de trazabilidad |
| 5 | `Historias_de_Usuario_HU01_HU18_v2.docx` | HU detalladas (HU-01..HU-18): flujos, criterios y reglas de negocio |
| 6 | `arquitectura_videojuego_v2.docx` | Decisiones técnicas de implementación |

La precedencia no resuelve las contradicciones **internas** a un documento: esas se corrigen
editándolo. `INCONSISTENCIAS.md` (rev. 5, 30/08/2026) registra los 42 hallazgos históricos; **todos
están cerrados en los `.docx`** salvo dos residuos cosméticos (pies de página de HU-17/HU-18).
Este documento ya no necesita separar «lo que dice el documento» de «lo que implementa el
código»: coinciden.

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
iluminación progresiva). RF-46 subió de Media a Alta el 29/08/2026, con lo que la dependencia
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
| `progreso-registro` | F | Registro de indicadores, resumen de nivel, consulta docente, eliminación de datos | `sistema-navegacion`, C, D, E |

Las flechas apuntan en un solo sentido. Los tres niveles no se conocen entre sí: esa es la
condición que hace verificable RNF-16 (prueba de exclusión — retirar un nivel y comprobar
que los demás siguen ejecutándose).

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

Cada módulo recibe su propio `claudeDocs/SPEC-<id>.md`, escrito en orden de dependencia y solo
cuando le llega el turno. Este documento es la parte compartida: no se repite en ellos.

---

## Stack

| Elemento | Decisión |
|---|---|
| Motor | Unity 6000.5.10f1, plantilla 2D + URP 17.6.0 (CT-01) |
| Lenguaje | C# |
| Entrada | Input System 1.20.0 (`Assets/Settings/InputSystem_Actions.inputactions`) — nunca la clase `Input` legada |
| Datos de contenido | ScriptableObjects (CT-05, RNF-18) |
| Persistencia | JSON local, sin red (CT-07, RNF-10) |
| Pruebas | Unity Test Framework 1.7.0 (NUnit) |
| Destino | Windows 10+ 64 bits, carpeta portable, sin instalación ni internet (CT-03, RNF-07, RNF-08) |
| Control de versiones | Git; cada commit referido a su tarjeta del tablero Kanban (CT-11, RNF-17) |

**Presupuestos duros** (RNF-04..RNF-06): carga de escena < 10 s, memoria en ejecución < 2 GB,
paquete de distribución < 500 MB. Se verifican en el equipo de referencia, no se estiman.

---

## Comandos

No hay CLI de build, lint ni pruebas en este repositorio. Todo pasa por el Editor de Unity
y las herramientas MCP de Coplay (`mcp__coplay-mcp__*`).

| Tarea | Cómo |
|---|---|
| Ejecutar pruebas | Skill `unity-coding-skills:run-tests` → `run_unity_tests`. Nunca invocarla ad hoc. **Hoy esa herramienta no está conectada — ver la nota de abajo.** |
| Una sola prueba / assembly | `run_unity_tests` acepta filtro por assembly y por nombre de prueba; filtrar antes que ejecutar la suite completa. |
| Leer errores de compilación | `mcp__coplay-mcp__check_compile_errors` / `mcp__coplay-mcp__get_unity_logs`, no abrir el Editor a ciegas. |
| Editar escenas y prefabs | Skill `unity-coding-skills:edit-scene` (nunca editar `.unity`/`.prefab` a mano sin ella). |
| Build de entrega | Editor → Build Profiles → Windows x64, salida a carpeta portable. |

> Si el puente Coplay no conecta, no hay forma de manipular escenas: reiniciar el Editor y el
> puente antes de empezar cualquier slice.

**El único servidor MCP configurado es `coplay-mcp`, y no trae corredor de pruebas.** Las
herramientas que la skill `run-tests` da por sentadas —`run_unity_tests`,
`get_unity_compilation_result`, `unity_play_control`— vienen de un servidor MCP de Unity aparte
que aún no está instalado. Mientras no lo esté, las pruebas se corren a mano desde la ventana
Test Runner del Editor y hay que **decirlo**, nunca dar por hecho que la suite pasó. Instalar ese
servidor es lo que desbloquea el flujo test-first del que depende toda la estrategia de pruebas
de más abajo.

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

`Narrative` se resuelve con un `NarrativeSequence` (ScriptableObject) y **una sola escena
reutilizable**; `Playing` con un `LevelId` y una fase. Añadir una escena narrativa pasa a ser
un asset, no un estado, una escena y una rama del FSM. Es menos código y menos peso en el
paquete (RNF-06).

### El FSM es C# plano

`GameFlow` no es un MonoBehaviour: es una máquina de estados sin dependencias de Unity, con un
`GameFlowRunner` delgado que traduce transiciones a `SceneLoader`. Así los recorridos del
Golden Path (RNF-13) se prueban en EditMode, sin escenas ni frames — que es lo que hace
pagable el «un caso de prueba por requerimiento» de CT-10.

### Comunicación: interfaz donde hay un consumidor, evento donde hay varios

El documento propone un `EventBus` global para todo. Se acota:

- **Interfaz inyectada** cuando el consumidor es conocido: un nivel reporta con
  `ILevelReporter` que Core le pasa al arrancar. Sirve además para probar el nivel con un doble.
- **Evento** solo cuando hay varios oyentes que no se conocen entre sí — HUD, audio y guardado
  reaccionando a la misma señal.

Un bus genérico para todo convierte el flujo en algo que no se puede seguir leyendo el código,
justo lo contrario de por qué se eligió una FSM.

### Módulos por capa

Se conserva el reparto del documento, corrigiendo lo que no aplica a este juego.

| Capa | Componentes | Módulo |
|---|---|---|
| Core | `GameBootstrap`, `GameFlow` (FSM), `SceneLoader`, `PlayerProfile`, `SaveStore` | `sistema-navegacion` |
| Andamiaje | `DialogueRunner`, `GuideController`, `HintPolicy`, `FeedbackLog` | `andamiaje` |
| Gameplay | `FireLevel`, `WheelLevel`, `RiverLevel` — un assembly cada uno | `nivel-*` |
| UI | `HUDController`, menús, pausa (RF-07) | transversal |
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
`StreamingAssets`. **RF-05** especifica «ilustraciones estáticas y cuadros de diálogo
secuenciales», y el guion §2 lo confirma. Video comprometería además RNF-06 (< 500 MB) y RNF-04
(carga < 10 s). Se implementa como `DialogueRunner` sobre ilustración fija, con avance por clic
y botón de omitir (RF-06).

**La persistencia no usa `Application.persistentDataPath`.** Escribe en `%AppData%\LocalLow`,
fuera de la carpeta portable: choca con RNF-07 y con el criterio de verificación de RNF-11,
que exige «ausencia de residuos en el almacenamiento local» tras eliminar un perfil. Se
mantiene el supuesto 1: JSON en `Datos/` junto al ejecutable.

**Y dos que contradecían criterios pedagógicos**, heredadas del molde arcade genérico:
mencionaba pantalla de *Game Over* y *puntajes* en HUD y `SaveSystem`. **CP-02**
prohíbe pantallas de derrota y **CP-03** prohíbe puntajes; RF-17 prohíbe cifras en la
retroalimentación. No se implementan. El `InputHandler` tampoco abstrae gamepad: la entrada se
limita a **clic y clic sostenido, sin excepciones** (CT-06, RNF-02). El desplazamiento del
personaje en el Nivel 3 usa botones de dirección **en pantalla**, accionados con clic — no el
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
        UI/  Audio/      → HUD, menús, pausa; audio persistente
      Editor/            → utilidades de editor, si aparecen
    Data/                → ScriptableObjects: diálogos, tareas, configuración de nivel
    Scenes/
      Boot.unity  MainMenu.unity  LevelSelect.unity  Credits.unity
      TeacherReport.unity
      Narrative.unity          ← única, parametrizada por NarrativeSequence
      Level1_Cave.unity
      Level2_Forest.unity  Level2_Workshop.unity  Level2_Maze.unity
      Level3_River.unity
    Art/  Audio/         → assets propios o con autorización escrita (CT-09, RNF-23)
  Settings/              → URP 2D, Renderer2D, Input Actions (ya existente)
  Tests/
    EditMode/            → lógica pura, una carpeta por módulo
    PlayMode/            → escenas, UI, integración
docs/                    → documentos fuente del trabajo de grado (no editar desde código)
claudeDocs/              → SPEC.md (este contrato) e INCONSISTENCIAS.md (hallazgos)
  tasks/
    Slice 1/  Slice 2/  Slice 3/  Slice 4/
      plan.md            → plan técnico del slice
      todo.md            → tablero de tareas del slice
```

Los namespaces siguen la ruta relativa a `Scripts`, elidiendo `Runtime`:
`Assets/Game/Scripts/Runtime/Levels/Fire/FirePanel.cs` → `namespace Game.Levels.Fire`.

**Assemblies** (`.asmdef`), uno por módulo con dependencia unidireccional:
`Game.Core`, `Game.Scaffolding`, `Game.Levels.Fire`, `Game.Levels.Wheel`,
`Game.Levels.River`, `Game.Reporting`, `Game.UI` y `Game.Audio`, más un assembly de pruebas por
cada uno. Ningún assembly de nivel referencia a otro assembly de nivel: eso es lo que hace
ejecutable la prueba de exclusión de RNF-16.

`Game.UI` y `Game.Audio` están en la lista de la arquitectura §9 (INC-40, cerrado): `HUDController`
y `AudioManager` tienen que compilar en algún sitio, y meterlos en `Game.Core` haría que el
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

        /// <summary>Resuelve un golpe desde la posición dada y devuelve lo observado.</summary>
        public StrikeOutcome Strike(StrikePosition position)
        {
            if (position != _config.EffectivePosition)
            {
                _consecutiveFailures++;
                return StrikeOutcome.SparksDied(position);
            }
            // Lo ganado permanece: un fallo posterior nunca reduce el contador (guion §4.3.6).
            _effectiveStrikes++;
            _consecutiveFailures = 0;
            return StrikeOutcome.SparkLanded(_effectiveStrikes);
        }
    }
}
```

Los parámetros ajustables jugando van en ScriptableObject con `[field: SerializeField]` y
`[Tooltip]`, nunca como literal en el código (CT-05, RNF-18):

```csharp
[CreateAssetMenu(menuName = "Game/Levels/Fire Config")]
public class FireLevelConfig : ScriptableObject
{
    [field: SerializeField, Tooltip("Distancias disponibles en el control deslizante.")]
    public int AvailablePositions { get; set; } = 3;

    [field: SerializeField, Tooltip("Única posición desde la que un golpe cuenta como efectivo.")]
    public StrikePosition EffectivePosition { get; set; } = StrikePosition.VeryClose;

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
| Verificación visual | PlayMode | Solo lo que no admite aserción estricta: contraste (RNF-20), doble indicador color+icono (RNF-19), ausencia de destellos rápidos (RNF-21). `[Category("VisualVerification")]`. |
| Manual | Plan de pruebas OE4 | Presupuestos de rendimiento, ejecución portable en dos equipos, ejecución sin red, cierre forzado y recuperación. |

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
   Nivel 1, nunca la posición efectiva (guion §4.3.6).

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
   tres de ellos —CP-02, CP-03 y CP-06— se verifican además con pruebas automatizadas.
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

`INCONSISTENCIAS.md` (rev. 5, 30/08/2026) registra los 42 hallazgos históricos entre los seis
`.docx`. **Todos están cerrados en los documentos**; el código sigue sencillamente lo que dicen.
Lo que el código materializa de cada decisión, para que no se pierda al leer solo el `.docx`:

| Hallazgo (cerrado) | Lo que el código materializa |
|---|---|
| INC-25 · HU-17 | Pausa como capa de UI sobre `Playing`: Continuar (restituye el estado exacto), Reiniciar nivel (confirmación, nunca re-bloquea un nivel desbloqueado, no borra indicadores), Volver al menú. Sin `GameOver`. |
| INC-26 · HU-14 | El resumen de fin de nivel es **narrativo y sin cifras** (RF-45, RF-17, CP-03). Las cifras solo viven en `TeacherReport` (RF-46); no hay `ScoreManager`. |
| INC-01 · controles | **Botones de dirección en pantalla, accionados con clic.** Nunca teclado. El mapa de controles se inspecciona sin salvedades (RNF-02, CT-06). |
| INC-27 · RNF-09 | Se persisten nombre o alias, nivel alcanzado, fases confirmadas y los cuatro indicadores. Nada más. |
| INC-28 · omisión | El botón de omitir aparece **solo si la escena ya fue vista** (RF-06). El cierre reflexivo no se omite la primera vez (CP-07, RF-12). |
| INC-29 · HU-10 | El Nivel 2 fase 3 emite exactamente los cuatro indicadores de OE1 §3.6.1, con su definición operativa. |
| INC-30 · Nivel 3 | Lista de **cuatro tareas** (RF-36). Ensamblaje de **tres fases** (RF-40). Tarea 3 se marca al confirmar la fase de amarre; tarea 4, al confirmar mástil y vela; la fase de base no marca tarea por sí sola. |
| INC-32 · «Soplar» | Una vez habilitado, «Soplar» **no** vuelve a deshabilitarse: lo ganado permanece (guion §4.3.6, CP-02). El desbloqueo depende solo del número de golpes efectivos. |
| INC-33 · bloques del laberinto | «Avanzar» y «Retroceder» son relativos a la orientación de la carretilla; «Girar» rota 90° en sentido horario. Con la lectura absoluta el refugio podía ser inalcanzable. |
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
2. **Guardado automático** al completar cada fase, no en cada acción (RF-04), que es lo que hace
   verificable la recuperación tras cierre forzado (RNF-14). Los cuatro indicadores de OE1 §3.6.1
   se persisten en ese mismo punto.
3. **Personajes originales** por defecto. Los de la Familia Anonaky autorización para trabajar en esos diseños ya ha sido aprobada por
   escrito (PG-07, CT-09, RNF-23), como establecen el trabajo de grado §3.3.2 y §5.2.
4. **Raíz de assets** `Assets/Game/` y namespace `Game.*`: el título aún no está definido (PG-01)
   y no conviene atar la estructura de carpetas a una decisión pendiente.
5. **El guía se llama Chispa** provisionalmente, siguiendo el guion, que es el único documento con
   escena de origen y caracterización visual (PG-02). Al vivir en ScriptableObjects, el nombre se
   cambia sin tocar código.
6. **Nivel 3 usa botones de dirección en pantalla**, accionados con clic — no el teclado. Letra
   vigente en todos los documentos: RF-35, guion §2.1 y §8.2, CU-09, HU-11 y arquitectura §1.
   **No hay excepción alguna a RNF-02 ni a CT-06** (INC-01, cerrado).
7. **Los valores del Nivel 1** (`posicionEfectiva` = Muy cerca, `golpesEfectivosMinimos` = 3,
   `intentosParaPista` = 3) son los propuestos por el guion §4.3.2 y siguen sin validarse jugando
   (PG-06, hoy correctamente abierto). Viven en `FireLevelConfig` para que ajustarlos no cueste
   una recompilación.
8. **Los bloques del laberinto** son relativos a la orientación de la carretilla: «Girar» rota 90°
   en sentido horario y «Avanzar» / «Retroceder» mueven adelante y atrás respecto de esa
   orientación. Letra vigente en RF-31 y en el guion §6.3.2 (INC-33, cerrado). Con la lectura
   absoluta ninguna secuencia se desplazaba en vertical.
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
desde una posición incorrecta **produce chispas visibles que se apagan** (PG-03 cerrado, RF-16;
guion §4.3.3/§4.3.4 y HU-06 coinciden); y la **definición operativa de los cuatro indicadores**
por nivel está fijada en OE1 §3.6.1, con lista cerrada.

---

## Preguntas abiertas

Ya no hay preguntas de diseño ni de redacción con efecto en el código: los 42 hallazgos de
`INCONSISTENCIAS.md` están cerrados en los `.docx` y este documento sigue su resolución.

**Del guion (§12), sin resolver** — son del guion, no conflictos entre documentos:
**PG-01** título del producto · **PG-02** nombre definitivo del guía (Chispa, provisional) ·
**PG-05** verificar en pruebas que el cambio de esquema de control entre el Nivel 1 y el 2 no
confunde · **PG-06** validar jugando los valores del Nivel 1 (`FireLevelConfig`) · **PG-07**
autorización de los personajes de la Familia Anonaky (Ya aprobado).

**Residuo cosmético en `docs/`:** HU-17 y HU-18 no llevan el encabezado «Página 17/18 de 18».

**Cerrado desde la rev. 4:** la semántica de los bloques del laberinto (lectura relativa, giro
90° horario, fijada en RF-31 y guion §6.3.2 — INC-33); `RF-19` sin condición de posición
(INC-32); `RNF-09` admite el progreso de avance (INC-27); la restricción «escena ya vista» de
`RF-06` en todos los documentos (INC-28); `RF-44` y `RF-46` trazados a HU-13 y HU-16, con la fila
de historia asociada en CU-11 (INC-37); la definición operativa de los cuatro indicadores por
nivel y por fase, con lista cerrada (OE1 §3.6.1); y el alcance de las sesiones con estudiantes,
verificación funcional y de usabilidad sin medición del efecto pedagógico (trabajo de grado §1.6
y §5.3).
