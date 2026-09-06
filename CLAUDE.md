# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Qué es este repositorio

Prototipo de videojuego educativo 2D en Unity (trabajo de grado): tres niveles narrativos
—fuego, rueda, balsa— que ejercitan facetas del pensamiento computacional en estudiantes de
grado cuarto. Entrega = ejecutable portable de Windows, sin instalación ni internet.

Unity 6000.5.10f1, plantilla 2D + URP 17.6.0, Input System 1.20.0, Unity Test Framework 1.7.0.
Presupuestos duros que se verifican, no se estiman (RNF-04..RNF-06): carga de escena < 10 s,
memoria < 2 GB, paquete < 500 MB.

Estado del código: **Fase 0 del Slice 1 cerrada (T01–T04).** Existen los cinco assemblies de
runtime; `Game.Core` ya tiene `GameFlow`, `GameState`, `PlayerProfile`, `SaveStore`,
`IFileSystem`, `SceneLoader` y `GameFlowRunner`, más las escenas `Boot` y `MainMenu` y las
pruebas EditMode de Core y de arquitectura. `Game.Scaffolding`, `Game.Levels.Fire`, `Game.UI` y
`Game.Audio` son `.asmdef` todavía sin código. **Fase 0 verificada el 03/09/2026: 23/23 EditMode,
3/3 PlayMode, 0 warnings** — resultados en `claudeDocs/tasks/Slice 1/Fase-0-Resultados.md`. Del
Checkpoint A solo faltan la medición de RNF-04 y la revisión con el usuario.

**Idioma:** identificadores y código en inglés; documentación, textos del jugador y
comunicación con el usuario en español.

**Al final de cada implementación** Mencionar al inicio mi nombre "Santiago"

## Documentos que gobiernan el trabajo — leer antes de codificar

| Archivo | Qué contiene |
|---|---|
| `claudeDocs/SPEC.md` | **El contrato.** Mapa de módulos, arquitectura, estructura de carpetas, estilo, estrategia de pruebas, límites (Siempre / Preguntar primero / Nunca), supuestos y preguntas abiertas. |
| `claudeDocs/INCONSISTENCIAS.md` | Los conflictos entre los `.docx` con la corrección aplicada a cada uno. Documento hermano de `SPEC.md`. Verificación vigente: 30/08/2026, rev. 6 — los hallazgos `INC-01`..`INC-42` están cerrados en los documentos; queda abierto `INC-43` (el guion §12 aún declara `PG-07` pendiente pese a estar concedida la autorización), más residuos cosméticos y los puntos abiertos del guion. |
| `claudeDocs/Direccion_de_Arte.md` | **La ley visual.** Paleta, grosor de línea, sombreado, personajes, entornos por nivel, UI, tipografía, VFX, nomenclatura de archivos (`char_`, `prop_`, `env_`, `ui_`) y checklist de aceptación (§17). Obligatorio antes de crear o generar cualquier asset visual; subordinado a `SPEC.md`, no introduce mecánicas. |
| `claudeDocs/tasks/Slice N/plan.md` + `todo.md` | **El trabajo en curso.** `plan.md` es el plan técnico del slice (alcance, grafo de dependencias, tareas); `todo.md` es el tablero con casillas y checkpoints. Los cuatro slices están planeados y entre ellos cubren los 47 RF: `Slice 1` Golden Path y nivel fuego · `Slice 2` La Rueda · `Slice 3` El Río y cierre del juego · `Slice 4` progreso, informe docente y borrado de datos. **Se ejecutan en orden**: cada uno supone terminado el anterior. Ninguno rediscute `SPEC.md`. |
| `docs/*.docx` + `docs/md/*.md` | Fuentes del trabajo de grado: requerimientos, guion, casos de uso, historias y arquitectura. El `.docx` es el original radicado; el `.md` del mismo nombre en `docs/md/` es su conversión ya hecha con markitdown. **Nunca editar ninguno de los dos desde código.** |
| `docs/actas/*.md` | Actas de seguimiento semanales: qué se decidió, cuándo y por qué. Raíz = fase de conceptualización (abr–jun 2026), `requerimientos/` = OE1 (jun–ago), `objetivo2/` = OE2 (ago–sep). Son la trazabilidad de las decisiones; consultarlas cuando haga falta el *porqué* de un RF, no reabrirlas. |
| `docs/actas/tablero_kanban.md` + `.csv` | El tablero Kanban reconstruido desde la sección «Compromisos y tablero Kanban» de cada acta: tarjeta, responsable, fecha límite, acta de apertura y acta de cierre. Refleja el estado **a una fecha de corte** (hoy por omisión); las actas posteriores se ignoran. **Generado, no editable a mano** — `python docs/actas/tablero.py` lo rehace tras añadir un acta; `tablero.py --hasta AAAA-MM-DD` mueve la fecha de corte. `subir_tablero.py --dry-run` previsualiza y sin la bandera publica cada tarjeta como issue en GitHub Projects — requiere `gh` autenticado con `gh auth refresh -s project,read:project`; es idempotente, se puede recorrer tras cada acta nueva. |

**Dónde empezar una sesión de código:** en la primera casilla sin marcar del `todo.md` del
slice abierto más bajo. No abrir un slice sin haber cerrado el anterior.

**Leer un documento fuente:** usar la conversión ya hecha en `docs/md/<mismo nombre>.md` —
se lee con Read y se busca con Grep, que es lo que hace viable citar un RF sin releer el
documento entero. Regenerarla solo si el `.docx` cambió:

```bash
PYTHONIOENCODING=utf-8 markitdown "docs/<archivo>.docx" > "docs/md/<archivo>.md"
```

`docs/md/`, `docs/actas/` y `claudeDocs/tasks/Sprites/` están en `.gitignore`: existen en este
equipo pero no en un clon limpio. Las conversiones se rehacen con markitdown; las actas no.

**Nunca** abrir un `.docx` con Read ni descomprimiendo el zip. markitdown ya está en el `PATH`;
sin `PYTHONIOENCODING=utf-8` los acentos salen como mojibake. El shell por defecto de este
proyecto es PowerShell, donde ese prefijo no es sintaxis válida: correrlo con la herramienta
Bash, o `$env:PYTHONIOENCODING='utf-8'` antes de invocar markitdown.

**Rutas siempre entre comillas.** La raíz del proyecto es `C:\Users\benab\My project` —con
espacio— y los `.docx` traen espacios y paréntesis en el nombre. Sin comillas, cualquier comando
de shell falla o toca el archivo equivocado.

`SPEC.md` es la fuente de verdad para cualquier duda de alcance o diseño; este archivo no la
repite. Si algo del código contradice a `SPEC.md`, gana `SPEC.md` o se corrige el documento
explícitamente.

**Orden de precedencia entre los `.docx`** cuando se contradicen: trabajo de grado → OE1 →
guion → OE2 → historias de usuario → arquitectura. Gana el de mayor prioridad y se corrige el
otro. Las contradicciones internas a un mismo documento no las resuelve la precedencia: están
resueltas y registradas en `claudeDocs/INCONSISTENCIAS.md` (rev. 6, 30/08/2026).

Los nombres en disco no coinciden con cómo se citan los documentos — vienen con sufijos de
descarga. El mapa, en orden de precedencia:

| # | Documento | Archivo en `docs/` |
|---|---|---|
| 1 | Trabajo de grado | `Trabajo_de_Grado_2026_ICONTEC_IEEE (2).docx` |
| 2 | OE1 requerimientos | `OE1_Requerimientos (3) (1).docx` |
| 3 | Guion | `Guion_Completo_Videojuego (1).docx` |
| 4 | OE2 historias completas | `OE2_historias_completas (1).docx` |
| 5 | Historias de usuario | `Historias_de_Usuario_HU01_HU18_v2 (1).docx` |
| 6 | Arquitectura | `arquitectura_videojuego_v2 (2).docx` |

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

No hay CLI de build, lint ni pruebas: todo pasa por el Editor de Unity y sus MCP
(`mcp__rider__*` para compilar y probar, `mcp__coplay-mcp__*` para escenas y assets). **La tabla completa está en `claudeDocs/SPEC.md` §Comandos**
— no se duplica aquí. Lo que hay que saber antes de empezar:

- **Hay dos MCP: `coplay-mcp` y `rider`.** El de Rider trae el corredor de pruebas
  (`mcp__rider__run_unity_tests`, `mcp__rider__get_unity_compilation_result`) pero **a 03/09/2026
  no conecta**: falta `Library/ProtocolInstance.json`, o sea el editor externo configurado en
  Unity no es Rider. Arreglarlo es Edit -> Preferences -> External Tools en el Editor; es cosa
  del usuario, no se toca desde aquí.
- **La ruta que sí funciona hoy son las pruebas por línea de comandos**, que dejan XML de NUnit:
  `Unity.exe -runTests -batchmode -projectPath "<raíz>" -testPlatform {EditMode|PlayMode}
  -testResults <salida>.xml -logFile <log>`. Exige el **Editor cerrado** (bloquea el proyecto), y
  en PowerShell hay que pasar las rutas con comillas dentro del `-ArgumentList` o «My project» se
  parte por el espacio. Sale 0 si todo pasa, 2 si algo falla. `Unity.exe` no es una app de
  consola: `&` no espera a que termine — usar `Start-Process -Wait`.
- Si el MCP no responde, el Editor está cerrado o el puente caído: eso **no** es que la suite
  pase, y hay que decirlo.
- Play Mode a mano: `mcp__coplay-mcp__play_game` / `stop_game`.
- Errores de compilación y consola: `mcp__coplay-mcp__check_compile_errors` /
  `mcp__coplay-mcp__get_unity_logs`, en vez de adivinar.
- Si el puente Coplay no conecta, no hay forma de tocar escenas: reiniciar el Editor y el
  puente antes de empezar.

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
- **La lógica es C# plano; el MonoBehaviour es un adaptador delgado.** `GameFlow` (la FSM) no
  es MonoBehaviour: se prueba en EditMode sin escena ni frames. Igual para validadores,
  contadores y máquinas de estado de cada nivel.
- **Estados parametrizados, no uno por escena.** `Narrative` recibe un `NarrativeSequence`
  (ScriptableObject) y se resuelve en una única escena reutilizable; `Playing` recibe
  `LevelId` + fase. Añadir una escena narrativa = crear un asset, no un estado y una rama.
- **Interfaz inyectada donde hay un consumidor conocido; evento solo con varios oyentes.**
  No hay `EventBus` global.
- **Solo tres singletons con `DontDestroyOnLoad`**: `GameFlowRunner`, `SceneLoader`, `AudioManager`.
- **Contenido fuera del código** (CT-05): todo texto visible y todo parámetro ajustable jugando
  vive en ScriptableObjects con `[field: SerializeField]` + `[Tooltip]`.
- **Persistencia**: JSON por perfil en una carpeta `Datos/` junto al ejecutable — no
  `Application.persistentDataPath`, porque «portable» y «sin residuos» (RNF-07, RNF-11) deben
  significar lo mismo.
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
  de un módulo que todavía no tiene código (RNF-15, RNF-16).

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
