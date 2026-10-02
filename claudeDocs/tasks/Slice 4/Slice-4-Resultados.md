# Slice 4 — Progreso, informe docente y borrado de datos: lo construido y sus resultados

Documento de cierre del cuarto incremento, hermano de
[`../Slice 3/Slice-3-Resultados.md`](../Slice%203/Slice-3-Resultados.md). Registra lo hecho en la
rama `slice4`, con qué se verificó y qué quedó abierto. El tablero vivo sigue siendo
[`todo.md`](todo.md) —si este documento y el tablero se contradicen, gana el tablero— y el plan
técnico es [`plan.md`](plan.md). Ninguno de los tres rediscute `claudeDocs/SPEC.md`.

> **Estado al 25/09/2026:** el slice ya no está sin commitear. Entró a `main` como un solo
> commit, `9c34924` («Slice 4», 24/09/2026), fusionado con el PR #85 (merge `7b08036`). Desde
> entonces no ha cambiado ningún archivo propio del módulo: `Reporting/`, los tres controladores,
> `TeacherReport.unity`, `ReportContent.asset` y sus pruebas. Hay notas fechadas en la tabla de
> abajo, en §1, §2, §3 (decisión 5), §4 (Fase 4), §6 y §8. El detalle está en el «Anexo — lo que
> cambió después del cierre (25/09/2026)»: el commit y su tarjeta, los dos diálogos de borrado,
> la tipografía de `TeacherReport` y sus dos `ScrollRect`. La primera corrida completa de la
> suite tras el trabajo de arte, sonido y personajes del 24–25/09 está en la última sección,
> «Corrida completa de la suite (25/09/2026)».
>
> **01/10/2026:** el módulo cambió el 30/09 con el cierre de inconsistencias (`37b3cb7`): el informe
> docente avisa de la ruta de respaldo y sus textos viven en datos, «Salir» pide confirmación, el
> menú de niveles marca «Completado» y desbloquea por fases, y el ejecutable se identifica como
> «Algoritmia». Lo registra el [Anexo B](#anexo-b--lo-que-cambió-después-del-25092026-01102026), con
> la tabla de P12 de las doce escenas. Al final están la revisión uno por uno de los diez criterios
> de éxito de `SPEC.md` y la corrida completa de la suite del 01/10/2026, que es la vigente.
>
> *(01/10/2026, más tarde: la vigente pasa a ser la verificación final antes del build, en la
> última sección, «Verificación final y paquete (01/10/2026)», con el tamaño del ejecutable
> candidato.)*

| Campo | Dato |
|---|---|
| **Incremento** | Slice 4 — Progreso, informe docente (RF-46) y borrado de datos (RF-47) |
| **Rama** | `slice4`, desde `d5fa77f` (`main`, 23/09/2026) |
| **Fechas de ejecución** | 23/09/2026 – 24/09/2026 |
| **Commits en la rama** | 0 — todo el trabajo está sin commitear en el árbol de trabajo; mensaje de commit ya redactado, pendiente de que Santiago lo confirme *(Vencido el 25/09/2026: se commiteó como `9c34924`, 24/09/2026 13:54, y entró a `main` con el PR #85, merge `7b08036`; ver el Anexo, A.1.)* |
| **Volumen frente a `main`** | 43 archivos propios del slice (13 modificados + 30 nuevos; aparte, 44 `.meta` de `Assets/Game/Art/` se reserializaron solos al correr `unity test`, ruido del importador, no de este slice) · 736 inserciones / 69 borrados en lo modificado, +17 archivos `.cs` nuevos (744 líneas) y la escena `TeacherReport.unity` (6 312 líneas, generadas por el editor) *(Precisión del 25/09/2026: `9c34924` suma 114 archivos —57 nuevos y 57 modificados— con 10 310 inserciones y 69 borrados. Incluye este documento, `todo.md` y también los 44 `.meta` de `Assets/Game/Art/`, cada uno con un bloque de plataforma `WebGL` añadido; ver A.1.)* |
| **Código del módulo** | `Game.Reporting` (nuevo): 6 archivos, 248 líneas · controladores en `Game.UI`: `TeacherReportController`, `IndicatorTableView`, `EraseConfirmationDialog`, 314 líneas |
| **Pruebas del módulo** | `Game.Reporting.Tests`: 4 archivos, 256 líneas, 14 casos · `Game.Content.Tests` (nuevo): 1 archivo, 154 líneas · PlayMode nuevas: `TeacherReportTests` + `EraseDialogTests`, 387 líneas, 7 casos |
| **Verificación** | EditMode **321/321** (1 omitido declarado) · PlayMode dirigida a lo tocado (`Game.UI.PlayMode.Tests`, 5 clases): **23/25** (2 inconclusive preexistentes del Nivel 1, ajenas a este slice) · 24/09/2026 *(Nota del 25/09/2026: cifras del 24/09/2026. La corrida completa posterior está en «Corrida completa de la suite (25/09/2026)», al final.)* |
| **Presupuestos medidos** | No corresponden a este slice — sin arte de croma ni escena jugable nueva; P12 los mide sobre el proyecto completo |
| **Fase de la metodología Árcade** | Desarrollo — ejecución del ciclo Diseño ↔ Desarrollo ↔ Pruebas |

---

## 1. Alcance: qué cierra este slice

El módulo F (`progreso-registro`) completo: el resumen de fin de nivel unificado y barrido, la
consulta de progreso del docente por nivel y por fase, y la eliminación irreversible de un
perfil. Con RF-46 y RF-47 cerrados, **los 47 RF del proyecto quedan implementados** — no solo los
45 de prioridad Alta: RF-06 y RF-21, que este mismo slice podía declarar fuera, ya estaban
implementados desde slices anteriores (§4, Fase 4).

| Módulo | Qué entró | Qué no |
|---|---|---|
| `progreso-registro` | `Game.Reporting` (`ProfileRepository`, `IndicatorReport`, `PhaseIndicators`, `LevelReportSection`, `ReportContent`, `ResolutionTimeFormat`); escena `TeacherReport` con lista de perfiles y tabla de indicadores; eliminación con confirmación explícita; barrido de cierre de CP-03/RNF-09/CT-10 sobre el proyecto completo | Exportar a archivo, gráficas, comparación entre estudiantes (fuera de RF-46) |
| `sistema-navegacion` | `GameFlow`/`GameFlowRunner` ganan destino para `TeacherReport` (el estado ya existía desde el Slice 1, sin escena) | — |
| `Game.UI` | Botón «Progreso del equipo» en `MainMenu`; `TeacherReportController`, `IndicatorTableView`, `EraseConfirmationDialog` | — |

**Requerimientos cerrados:** `RF-46`, `RF-47`, cada uno con pruebas que lo nombran (CT-10,
`Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra` lo confirma sobre los 47). **No
funcionales cerrados con prueba:** RNF-16 (frontera de `Game.Reporting`), RNF-09 (lista cerrada
del JSON con un perfil de los tres niveles), CP-03 (barrido de cifras sobre once tipos de
contenido *(corrección del 25/09/2026: son diez; ver §4, Fase 4)*), RNF-19/RNF-20 sobre la tabla del informe, RNF-02 (el mapa de controles del proyecto,
corregido en la escena nueva — hallazgo real, §5).

---

## 2. Seguimiento metodológico

Las actas de `docs/actas/OE3/` no están en este equipo (están en `.gitignore` y, según
`CLAUDE.md`, se perdieron una vez y se rehicieron a mano en el equipo donde se escribieron). Este
slice se ejecutó completo en una sola sesión de Claude Code, a petición directa de Santiago
(«revisa y realiza el slice 4»), sin una tarjeta de Kanban de origen que citar aquí. Si existe una
en el tablero de las actas, se asocia a mano al commit cuando se cree.

*(Vencido el 25/09/2026: las actas `D01`–`D09` ya están en este equipo, rehechas. La tarjeta de
origen existe: es `D08-1` (acta D08, 23/09/2026), «Iniciar el cuarto slice…», con Santiago
Valdiri García como responsable. El acta D09 (24/09/2026) da el slice por cerrado y fusionado, y
`D08-1` ya no figura en su tablero. El mensaje de `9c34924` no la cita; ver A.1.)*

---

## 3. Decisiones que definieron el slice

Ninguna salió del código; quedaron registradas en `todo.md` con su fecha.

1. **`Game.Reporting` solo referencia `Game.Core`** (P01, confirma la decisión ya escrita en
   `plan.md`): retirar un nivel no puede romper el informe docente. Probado con dos pruebas
   negativas en `AssemblyDependencyTest`.
2. **Los controladores de pantalla viven en `Game.UI`, no en `Game.Reporting`** (P05): es donde ya
   viven `MainMenuController`, `ProfileSelectController` y `LevelSummaryController`.
   `Game.Reporting` se queda puro dato/lógica, sin `UnityEngine.UI`, coherente con la decisión 1.
3. **El resumen de fin de nivel no se rehizo** (P02): los Slices 1-3 ya lo unificaron sobre
   `LevelSummaryController`/`LevelSummaryComposer`, su propia tablilla — no el marco `A10` que
   `plan.md` asumía. Reescribirlo habría sido deshacer una arquitectura ya revisada en sus
   checkpoints; lo que faltaba de verdad era el barrido transversal, y eso sí se hizo.
4. **`ProfileEraser` no se construyó** (P08): `SaveStore.Delete` (`Game.Core`, Slice 1) ya borraba
   las dos rutas sin residuos, con pruebas que ya citaban RF-47 e INC-34. Escribir una clase nueva
   habría sido reinventar lógica ya probada; `EraseConfirmationDialog` la llama directamente.
5. **El diálogo de confirmación de P09 reutiliza el de `ProfileSelectController`** en vez de
   generar uno nuevo: alerta, aviso de irreversibilidad, «Cancelar» neutro y «Eliminar» marcado en
   ámbar con icono de papelera — un patrón ya construido y aprobado en el Slice 1.
   *(Precisión del 25/09/2026: del Slice 1 se reutiliza la disposición —velo, `ui_alerta`,
   confirmar en ámbar con `ui_papelera`— y la llamada a `ProfileSession.Delete`, no los textos
   ni el componente. «Cancelar» / «Eliminar datos» son del diálogo de `TeacherReport`
   (`TeacherReport.unity:3260` y `:3803`); el panel del estudiante en `MainMenu`, de `145a63e`
   (08/09/2026), dice «Conservar» / «Borrar» (`MainMenu.unity:4161` y `:4319`).
   `EraseConfirmationDialog` es un componente nuevo; ver A.2.)*
6. **La escena se construyó con un script de editor efímero** (`execute_script` de `coplay-mcp`),
   no llamada por llamada con las herramientas de escena una por una: para una pantalla de ~40
   objetos son cientos de llamadas equivalentes; el script se escribe una vez, se corre, y se
   borra. Mismo patrón que ya usa el proyecto para las escenas de niveles anteriores.
7. **D1 (los cuatro iconos) se resolvió con un boceto, no con el arte final**: el generador de
   imágenes de `coplay-mcp` devolvió `401 Unauthorized` con los dos proveedores — no es un
   problema que el código pueda resolver. A petición de Santiago, los cuatro iconos se dibujaron
   con Python/Pillow (formas geométricas, no la ilustración vectorial cartoon de la dirección de
   arte) y se cablearon de verdad en la tabla, con sufijo `_boceto` para que el reemplazo por el
   arte final sea sustituir el archivo, no volver a cablear nada.
8. **D2 y D3 no se generaron**: eran maquetas para guiar la construcción de la pantalla y del
   diálogo, y P05/P06/P09 ya construyeron las dos cosas reales directamente en Unity. Generar la
   maqueta después de construir lo real habría sido documentar algo que ya no hace falta para
   construirse.

---

## 4. Lo construido, fase por fase

### Fase 0 — Cimientos (P00, P01 · 23/09/2026)

`unity test` confirmado como corredor de pruebas primario (el bridge de `coplay-mcp`/Rider
requiere el Editor abierto e inactivo, comportamiento ya documentado, no un fallo). `Game.Reporting`
creado con su `.asmdef` (referencia única `Game.Core`) y su assembly de pruebas EditMode;
`AssemblyDependencyTest` ganó las dos pruebas negativas de RNF-16.

### Fase 1 — El resumen del estudiante (P02 · 23/09/2026)

Barrido transversal sobre los tres resúmenes reales (no un doble): cero dígitos, cero juicios de
valor, ninguna oración de más de 20 palabras, y una prueba de exclusión por nombre que impide que
exista una clase `Score`/`Points` en el proyecto. Cuatro pruebas nuevas en
`LevelSummaryComposerTests.cs`.

### Fase 2 — El lado del docente (P03–P07 · 23/09–24/09/2026)

`ProfileRepository` (enumera perfiles de las dos rutas sin duplicar, tolera un archivo corrupto) e
`IndicatorReport`/`PhaseIndicators`/`LevelReportSection` (agregación por nivel y por fase, un
nivel no jugado es «sin datos», no ceros; por reflexión se fija que `PerformanceIndicators` solo
tiene los cuatro campos de §3.6.1). La escena `TeacherReport` se construyó completa —lista de
perfiles con selección, tabla de indicadores clonada fila por fase, estado vacío con aviso y
vuelta al menú— y se cableó el botón de acceso en `MainMenu`. `GameFlowRunner` expone
`PortableRoot`/`FallbackRoot` para que `Game.UI` arme su propio `ProfileRepository` sin que
`Game.Core` conozca ese tipo.

### Fase 3 — Eliminación de datos (P08–P10 · 24/09/2026)

Sin código de borrado nuevo (decisión 4, §3): `EraseConfirmationDialog` llama a
`ProfileSession.Delete`, que ya limpiaba el perfil activo si era el borrado — resolviendo de paso
la pregunta abierta sobre qué pasa si el docente borra el perfil activo, en el mismo sentido que
proponía el plan. Prueba de residuos sobre disco real en un directorio temporal (nunca `Datos/`
del proyecto); el escenario de solo lectura se declaró omitido con motivo — este equipo no hace
cumplir el atributo de solo lectura de carpetas en NTFS.

### Fase 4 — Cierre del proyecto (P11 · 24/09/2026)

`Game.Content.Tests` (nuevo, el único assembly de pruebas que referencia los tres niveles a la
vez — justificado como auditoría de cierre, no código de producción) recorre el grafo serializado
de once tipos de contenido *(corrección del 25/09/2026: son diez; el arreglo `TiposDeContenido`,
`ContentInvariantTests.cs:35-41`, no ha cambiado desde `9c34924`. `ReportContent` y
`CreditsContent` quedan fuera a propósito, :29-34; ver A.5)* buscando cifras, saltando identificadores técnicos (campos que
terminan en «Id») y restando el índice de formato `{0}`/`{1}` antes de mirar dígitos. El JSON de
un perfil que jugó los tres niveles se probó contra la lista cerrada de RNF-09, y la trazabilidad
de CT-10 se deriva de los nombres de método: los 47 RF están citados. Al revisar el checkpoint se
encontró que **RF-06 y RF-21 —los dos que el slice podía declarar fuera— ya estaban
implementados** desde slices anteriores (`NarrativeVisitPolicy` y `CaveLightingController`).

---

## 5. Hallazgos reales encontrados al verificar

Ninguno salió de leer el código: los encontró una prueba, una corrida o una captura.

1. **`Has.Count` de NUnit no funciona por reflexión sobre un array** (P03): el mensaje
   `Property Count was not found` viene de que `Array` implementa `ICollection.Count`
   explícitamente, invisible a la reflexión pública que usa el constraint. Corregido accediendo a
   `.Count` directamente en el código de la prueba, que sí lo resuelve en tiempo de compilación
   contra la interfaz `IReadOnlyList<T>`.
2. **La cabecera de la tabla no alineaba con las filas** (P06): un `HorizontalLayoutGroup` con
   `childForceExpandWidth` reparte el espacio sobrante después de honrar el ancho preferido de
   cada hijo — y el texto de la cabecera («Errores corregidos») tiene un ancho preferido real,
   mientras que la plantilla de fila arranca con texto vacío. Mismo layout, columnas distintas.
   Corregido con `LayoutElement(preferredWidth=0, flexibleWidth=1)` en las seis celdas de ambos.
3. **La escena nueva traía el mapa de controles por defecto del paquete, no el del juego**
   (RNF-02): `InputSystemUIInputModule` se autoasignó un `InputActionAsset` genérico en vez de
   `ControlesJugables.inputactions`. Lo encontró
   `Architecture_RNF02_LasEscenasYElProyectoUsanSoloElMapaDeControlesDelJuego`
   (`InputSchemeTest`), que ya vigilaba esto desde el Slice 3 (R16) — la prueba hizo exactamente
   lo que se escribió para hacer. Corregido asignando el asset y `AssignDefaultActions()`.
4. **El primer barrido de CP-03 encontró identificadores técnicos, no cifras de desempeño**
   (P11): IDs de secuencia narrativa (`N1_Apertura`), IDs de objetos de catálogo (`tronco_1`) y
   los índices `{0}`/`{1}` de las cadenas de formato del contador de RF-24. Ninguno es una cifra
   que el estudiante lea como desempeño; la prueba se afinó para excluir campos que terminan en
   «Id» y restar los índices de formato antes de mirar dígitos, en vez de mantener una lista de
   excepciones por campo que crecería con cada asset nuevo.
5. **`EraseConfirmationDialog.Start()` no corre en el mismo fotograma que `Ask()` lo activa**
   (PlayMode): `Awake()` corre síncrono al llamar `SetActive(true)`, pero `Start()` —donde se
   cablean los botones— se difiere al siguiente fotograma. Un clic de prueba inmediatamente
   después de abrir el diálogo no hacía nada porque los listeners todavía no existían. No es un
   defecto del diálogo real —ningún humano hace dos clics en el mismo fotograma—, pero sí de la
   prueba: se corrigió con un `await Awaitable.NextFrameAsync()` entre abrir y hacer clic.
6. **Ejecutar pruebas sin refrescar la compilación corre contra el ensamblado viejo**: tras
   corregir el hallazgo 5, la primera repetición volvió a fallar exactamente igual porque
   `run_unity_tests` no había recompilado. `get_unity_compilation_result` antes de cada corrida
   lo evita, tal como avisa la propia herramienta.
7. **El generador de imágenes de `coplay-mcp` devuelve `401 Unauthorized`** con los dos
   proveedores (`gpt_image_1` y `gemini`): no hay credenciales activas en este equipo. Es una
   causa de infraestructura, no de código — no se puede arreglar desde aquí (decisión 7, §3).

---

## 6. Verificación declarada

**Cómo se corrió.** `coplay-mcp` no conectó al principio de la sesión (`CONNECT_TIMEOUT`); tras
reiniciar la sesión de Claude Code volvió a conectar. A partir de ahí, EditMode se corrió con
`unity test` (Editor cerrado) y con Rider (`run_unity_tests`, Editor abierto) según qué hacía
falta tocar a continuación; PlayMode, siempre con Rider contra el Editor vivo. Ni el silencio de
un MCP ni un `ConnectionRefused` son que la suite pase: todas las cifras de esta sección salen de
una corrida real declarada.

| Corrida | Momento | Resultado |
|---|---|---|
| EditMode, suite completa (11 assemblies, incluido `Game.Content.Tests`) | 24/09/2026 | **321/321**, 1 omitido declarado (INC-34 solo lectura, no aplicable en este equipo) |
| EditMode, `Game.Architecture.Tests` tras corregir el hallazgo 3 | 24/09/2026 | **15/15** |
| PlayMode, `Game.UI.PlayMode.Tests` (`TeacherReportTests`, `EraseDialogTests`, `MainMenuTests`, `ProfileSelectTests`, `LevelSummaryTests`) | 24/09/2026 | **23/25**, 2 inconclusive preexistentes de `LevelSummaryTests` (convergencia del Nivel 1, ajenas a este slice) |
| PlayMode, suite completa del proyecto | 24/09/2026 | Lanzada en segundo plano (Nivel 3, luego `Narrative`); no se esperó el resultado porque este slice no tocó `Game.Levels.*` — cubrir su régimen de pruebas no era necesario para verificar lo construido aquí *(25/09/2026: la primera corrida completa posterior está en «Corrida completa de la suite (25/09/2026)», al final.)* |

**Verificación visual.** `capture_ui_canvas` sobre la escena real, con los perfiles manuales del
equipo (`Ana`, con las siete fases jugadas; `pepe`, sin ninguna): cabecera alineada con las
filas tras el hallazgo 2, «Sin datos» en las cuatro columnas de un nivel no jugado, diálogo de
borrado con la advertencia y los dos botones distinguibles, y los cuatro iconos de D1 sobre el
nombre de cada indicador tras cablearlos.

**Casos declarados por archivo de prueba (nuevos o ampliados en este slice):**

| EditMode | Casos | PlayMode | Casos |
|---|---|---|---|
| `ProfileRepositoryTests` | 3 | `TeacherReportTests` | 4 |
| `IndicatorReportTests` | 3 | `EraseDialogTests` | 3 |
| `ResolutionTimeFormatTests` | 6 (1 método, 6 `TestCase`) | `MainMenuTests` (+1) | 7 |
| `ProfileEraserDiskTests` | 2 | | |
| `ContentInvariantTests` | 1 | | |
| `LevelSummaryComposerTests` (+4) | 13 | | |
| `AssemblyDependencyTest` (+2) | 7 | | |
| `GameFlowTests` (+2) | 13 | | |
| `SaveStoreTests` (+1) | 10 | | |
| `TraceabilityTests` | 1 | | |

*(Comprobado el 25/09/2026 con un conteo estático sobre `ccf77e6`: las trece cifras de esta tabla
siguen iguales.)*

---

## 7. Inventario de archivos — qué es cada cosa

### 7.1 `Assets/Game/Scripts/Runtime/Reporting/` — el módulo nuevo (6 archivos, 248 líneas)

| Archivo | Qué es |
|---|---|
| `ProfileRepository.cs` | Enumera los perfiles de las dos rutas (`Datos/` y respaldo) sin duplicar, tolerante a un archivo corrupto |
| `IndicatorReport.cs` | Agrega los indicadores de un perfil por nivel y por fase; no calcula ningún agregado fuera de §3.6.1 |
| `PhaseIndicators.cs` · `LevelReportSection.cs` | Los cuatro indicadores de una fase, o su ausencia (`Played`); un nivel con sus fases en orden |
| `ReportContent.cs` | SO: el vocabulario de la tabla — nombres de nivel, de fase y de los cuatro indicadores (CT-05) |
| `ResolutionTimeFormat.cs` | El tiempo de resolución, persistido en segundos y presentado en minutos y segundos |

### 7.2 `Assets/Game/Scripts/Runtime/UI/` — controladores de pantalla nuevos (3 archivos, 314 líneas)

| Archivo | Qué es |
|---|---|
| `TeacherReportController.cs` | Adaptador de la pantalla: lista de perfiles, selección, estado vacío, borrado, vuelta al menú |
| `IndicatorTableView.cs` | Clona una fila por fase (mismo patrón que `LevelSummaryController`), agrupadas por nivel |
| `EraseConfirmationDialog.cs` | Confirmación explícita del borrado; llama a `ProfileSession.Delete`, no reimplementa nada |

Ampliados: `GameFlowRunner.cs` (`PortableRoot`/`FallbackRoot`, entrada de escena para
`TeacherReport`), `MainMenuController.cs` (botón «Progreso del equipo»).

### 7.3 Pruebas nuevas o ampliadas

`Assets/Tests/EditMode/Reporting/` (4 archivos, 256 líneas), `Assets/Tests/EditMode/Content/`
(nuevo, `ContentInvariantTests.cs`, 154 líneas, único assembly que referencia
`Game.Levels.{Fire,Wheel,River}` a la vez — razón documentada en el propio archivo),
`Assets/Tests/EditMode/Architecture/TraceabilityTests.cs` (nuevo), y las ampliaciones de
`AssemblyDependencyTest`, `GameFlowTests`, `SaveStoreTests`, `LevelSummaryComposerTests`,
`MainMenuTests` de §6. `Assets/Tests/PlayMode/UI/TeacherReportTests.cs` y `EraseDialogTests.cs`
(nuevos, 387 líneas) — cada uno con su propio `IFileSystem` en memoria porque un assembly
PlayMode no puede referenciar el `Game.Core.Tests` de EditMode.

### 7.4 Contenido — lo que se ajusta sin recompilar (CT-05, RNF-18)

| Asset | Qué fija |
|---|---|
| `Data/Reporting/ReportContent.asset` | Vocabulario de la tabla del informe docente |

### 7.5 Escenas y arte

`Scenes/TeacherReport.unity` (nueva, en Build Settings). `Art/UI/Common/ui_ind_{intentos,errores,
pasos,tiempo}_boceto.png` — boceto provisional de D1, dibujado con Python/Pillow, Sprite/Single a
100 PPU, con alfa; no registrado en `CreditsContent.asset` porque no es autoría final (§3,
decisión 7). D2 y D3 no generaron archivo (§3, decisión 8).

### 7.6 Documentos del slice

`plan.md`, `todo.md` (actualizado con el estado real de las doce tareas y las cuatro preguntas
abiertas), este documento.

---

## 8. Lo que queda abierto

**Del Checkpoint P-E**, todo lo que exige a Santiago o a una persona jugando:

- [ ] **Cinco casillas «Revisado con el usuario»** en los checkpoints P-A a P-E — ninguna se marca
      sola.
- [ ] **PG-01** (título del producto) y **PG-02** (nombre del guía): última oportunidad del
      proyecto para cerrarlos.
- [ ] **Pregunta abierta 2**: si el informe docente necesita protección de acceso. Ningún RF la
      pide; el plan no la añade. Decisión de alcance, no de implementación.
- [ ] **RNF-12** (consentimiento informado) y **RNF-22** (inspección integral del contenido):
      verificación documental/manual, sin tarea de código.
- [ ] **CT-02**: el ejecutable en un equipo sin GPU dedicada, dentro de RNF-04/RNF-05.
- [ ] **Golden Path del juego entero, dos veces** (RNF-13) — se solapa con el recorrido visual
      pantalla por pantalla que pedía la mitad `PlayMode` de P11; no tiene sentido duplicarlo.
- [ ] **P12 completo**: la tabla de mediciones de `todo.md` (cargas, memoria, paquete) sobre el
      equipo de referencia.
- [ ] **D1 final**: regenerar los cuatro iconos con el prompt ya escrito en `plan.md` en cuanto el
      generador de imágenes de `coplay-mcp` tenga credenciales — mismos nombres de archivo sin el
      sufijo `_boceto`, sin volver a cablear nada. Registrar en `CreditsContent.asset` cuando sea
      arte final (CT-09).
- [ ] Repetir la corrida de PlayMode completa del proyecto y declarar su resultado (§6) — quedó
      lanzada pero sin esperar, porque no tocaba nada de `Game.Levels.*`.
      *(25/09/2026: corrida al final del documento, en «Corrida completa de la suite (25/09/2026)».)*
- [ ] **Revisar y confirmar el mensaje de commit** (ya redactado, sin tarjeta de Kanban asociada
      — §2) y decidir si se commitea como un solo commit o se divide.
      *(Resuelto; nota del 25/09/2026: el 24/09/2026 entró como un solo commit, `9c34924`, fusionado con el PR #85. La tarjeta es
      `D08-1`; ver §2 y A.1.)*

*(25/09/2026: se suman dos pendientes: la tipografía de `TeacherReport` y sus dos `ScrollRect`.
Ver A.3, A.4 y A.6.)*

**Sin bloqueantes de código.** Con RF-46 y RF-47 cerrados, los 47 RF del proyecto están
implementados; lo que queda es cierre de proyecto, no desarrollo.

*(01/10/2026: las revisiones con el usuario, D1 final y la pregunta abierta 2 las resolvió Santiago
el 30/09/2026 (acta D10, §5): las revisiones se marcan tras verificarlas con pruebas y capturas, el
boceto de D1 queda como arte definitivo y el informe docente no lleva protección de acceso. PG-01 y
PG-02 están cerrados. El formato de RNF-12 está escrito y la inspección textual de RNF-22, hecha;
el Golden Path doble, la parte visual de RNF-22 y la columna del equipo 1 de P12 se hacen sobre el
ejecutable candidato, y CT-02 y la columna del equipo 2 quedan a cargo de Santiago. Detalle en el
Anexo B, B.7 y B.8.)*

---

## 9. Cómo se reproduce

```bash
# Suites completas — exigen el Editor de Unity cerrado
unity test --mode EditMode --output test-results.xml --timeout 900 --no-banner --non-interactive
unity test --mode PlayMode --output play-results.xml --timeout 1800 --no-banner --non-interactive

# Solo lo de este slice (--filter es una expresión regular contra namespace.Clase.Método)
unity test --mode EditMode --filter "Game\.Reporting\.Tests|Game\.Content\.Tests"
unity test --mode PlayMode  --filter "TeacherReportTests|EraseDialogTests|MainMenuTests"

# Con el Editor abierto, vía Rider (no exige cerrarlo)
# run_unity_tests(testMode: EditMode, assemblyNames: ["Game.Reporting.Tests", "Game.Content.Tests"])
```

Con el Editor abierto, `unity test` falla con `another Unity instance is running`. Antes de correr
pruebas tras editar un `.cs`, llamar primero a `get_unity_compilation_result` (hallazgo 6, §5) —
si no, la corrida usa el ensamblado de antes del cambio.

---

## Anexo — lo que cambió después del cierre (25/09/2026)

Este documento se escribió el 24/09/2026, con el trabajo todavía sin commitear. El anexo registra
lo que pasó después con el slice y corrige lo que el texto de arriba decía mal o dejaba fuera.
Todo se comprobó contra `ccf77e6` (HEAD al 25/09/2026) y contra `git log`.

### A.1 El slice en git

| Campo | Dato |
|---|---|
| **Commit** | `9c34924` «Slice 4», 24/09/2026 13:54, autor `Santivaldiry`. Su padre es `d5fa77f`, el mismo punto de partida que da la cabecera |
| **Fusión** | PR #85, merge `7b08036` sobre `main` (24/09/2026 13:54) |
| **Volumen** | 114 archivos (57 nuevos, 57 modificados), 10 310 inserciones y 69 borrados. Incluye este documento y `todo.md`. También incluye los 44 `.meta` de `Assets/Game/Art/` que la cabecera daba por ruido del importador: entraron en el commit, cada uno con un bloque de plataforma `WebGL` de 13 líneas (`overridden: 0`) |
| **Tarjeta** | `D08-1` (acta D08, 23/09/2026), responsable Santiago Valdiri García. El mensaje del commit no la cita |
| **Cambios posteriores** | Ninguno en los archivos propios del módulo. `git log 9c34924..HEAD` no devuelve nada sobre `Scripts/Runtime/Reporting/`, `TeacherReportController.cs`, `IndicatorTableView.cs`, `EraseConfirmationDialog.cs`, `TeacherReport.unity`, `Data/Reporting/`, `Tests/EditMode/Reporting/`, `Tests/EditMode/Content/`, `TraceabilityTests.cs`, `TeacherReportTests.cs` ni `EraseDialogTests.cs` |

### A.2 Los dos diálogos de borrado (RF-47)

RF-47 tiene dos puertas y un solo borrado. Las dos llaman a `ProfileSession.Delete` (`Game.Core`,
`ProfileSession.cs:42-52`), que borra con `SaveStore.Delete` en las dos rutas y, si el perfil
borrado era el activo, deja de tenerlo como activo. Lo que cambia es la pantalla:

| | Panel del estudiante | Informe docente |
|---|---|---|
| Escena y componente | `MainMenu.unity` (`ProfilePanel/DeletePanel`), `ProfileSelectController` | `TeacherReport.unity` (`EraseDialogRoot`), `EraseConfirmationDialog` |
| Origen | `145a63e` (08/09/2026, Slice 1) | `9c34924` (24/09/2026, este slice) |
| Pregunta | «¿Borras el perfil de {nombre}?» (`ProfileSelectController.cs:133`) | «¿Eliminas definitivamente los datos de {nombre}? Esta acción no se puede deshacer.» (`EraseConfirmationDialog.cs:54`) |
| Aviso aparte | «Su avance se pierde y no se puede recuperar.» (`MainMenu.unity:6072`) | No hay: la irreversibilidad va dentro de la pregunta |
| Botones | «Conservar» (`MainMenu.unity:4161`) · «Borrar» (`:4319`) | «Cancelar» (`TeacherReport.unity:3260`) · «Eliminar datos» (`:3803`) |
| Tipografía | Baloo 2 y Nunito | Fuente integrada de Unity (A.3) |
| Pruebas | `ProfileSelect_RF47_BorrarPideConfirmacionAntesDeEliminarElPerfil`, `ProfileSelect_RF47_ConservarDejaElPerfilIntacto` | `EraseDialog_RF47_ExigeConfirmacionExplicitaYAdvierteIrreversibilidad`, `EraseDialog_CU12_CancelarNoRealizaNingunCambioEnDisco`, `EraseDialog_RF47_TrasConfirmarElPerfilDesapareceDeLaLista` |

Las dos comparten la disposición: velo, `ui_alerta`, el botón de cancelar en crema y el de
confirmar en ámbar con `ui_papelera`. En el informe, el botón de la lista que abre el diálogo
también dice «Eliminar datos» (`TeacherReport.unity:3882`).

### A.3 Tipografía de `TeacherReport`

Los 20 componentes `Text` de `TeacherReport.unity` usan la fuente integrada de Unity
(`m_Font: {fileID: 10102, guid: 0000000000000000e000000000000000}`). Ninguno referencia las
tipografías de `Assets/Game/Art/Fonts/` (Baloo 2 y Nunito, que entraron con `145a63e`). También
usa la fuente integrada la etiqueta del botón «Progreso del equipo» que este slice añadió a
`MainMenu` (`MainMenu.unity:3714`): es el único de los 24 textos de esa escena que no usa Baloo 2
ni Nunito. Las otras pantallas de flujo —`LevelSelect`, `Credits`, `Narrative` y
`LevelSummary`— no tienen ningún texto con la fuente integrada. El documento no lo registraba ni
lo dejaba pendiente; se suma en A.6.

### A.4 Desplazamiento con `ScrollRect`

`TeacherReport.unity` desplaza sus dos listas con dos `ScrollRect` verticales con barra:
`ProfileColumn/ProfileScroll` y `DataColumn/TableScroll`, ambos con `m_Vertical: 1` y
`m_Inertia: 1`. `CLAUDE.md` pide que una lista que desborda se desplace con botones y no con un
`ScrollRect`, porque este trae el arrastre de uGUI. En esta pantalla no hay arrastrar y soltar con
el que se pelee, pero la misma regla limita la entrada a clic y clic sostenido «sin excepciones».
Queda para revisar (A.6). `Credits.unity` tiene el mismo caso, con un `ScrollRect`.

### A.5 El barrido de CP-03: diez tipos, no once

`ContentInvariantTests.TiposDeContenido` (:35-41) enumera diez tipos: `LevelSummaryMessages`,
`NarrativeSequence`, `GuideContent`, `FireMessages`, `WheelLevelConfig`, `RiverLevelConfig`,
`RaftAssemblyContent`, `AssemblyContent`, `MazeLayout` y `GameTitleConfig`. El comentario de
:29-34 deja fuera a propósito dos tipos: `ReportContent`, la única pantalla donde una cifra es
correcta, y `CreditsContent`. El archivo no ha cambiado desde `9c34924`, así que «once» ya estaba
mal al cierre. Después entraron dos `ScriptableObject` más, `RollingLogLook` (`b38c00b`) y
`RiverSounds` (`ccf77e6`). Guardan una textura, clips de audio y parámetros numéricos, sin
texto visible, y no están en la lista.

### A.6 Pendientes que se suman a §8

- [x] Pasar a Baloo 2 y Nunito los 20 textos de `TeacherReport` y la etiqueta «Progreso del
      equipo» de `MainMenu`, o dejar escrito por qué no (A.3). *(29/09/2026: hecho; los textos
      de 22 px suben a 26 px y `Scenes_CN04_NingunTextoUsaLaFuenteIntegradaDelMotor` lo vigila.)*
- [x] Decidir si los dos `ScrollRect` de `TeacherReport` se cambian por botones de desplazamiento
      o si se deja escrita la excepción (A.4). *(29/09/2026: se conservan, igual que el de
      `Credits`: barra y arrastre son clic y clic sostenido (CT-06). Al revisarlo apareció que el
      módulo de entrada de `TeacherReport` llevaba incrustado el `DefaultInputActions` del paquete
      —rueda, teclado y mando— y que la prueba de arquitectura no lo veía por no llevar `guid`; se
      recableó a `ControlesJugables`, que no vincula la rueda. HU-18 se alineó en el `.docx`.)*

---

## Corrida completa de la suite (25/09/2026)

Registra la primera corrida completa de EditMode y PlayMode después del trabajo de arte, sonido
y personajes del 24–25/09/2026. Se hizo sobre `ccf77e6` (HEAD), dentro del Editor abierto y con
`TestRunnerApi`, porque Rider estaba caído.

**Conteo estático.** No es una corrida: son los `.cs` de `Assets/Tests/` leídos sobre `ccf77e6`.
Un método es uno marcado con `[Test]` o `[TestCase]`. Los casos son los que resultan al expandir
`[TestCase]` y `[Values]` (los `[Values]` están en tres métodos de `CharacterRigTests`, con siete
personajes cada uno). El conteo incluye las pruebas que se omiten o quedan inconclusas al
ejecutarse.

| Assembly EditMode | Métodos | Casos | Assembly PlayMode | Métodos | Casos |
|---|---|---|---|---|---|
| `Game.Architecture.Tests` | 15 | 15 | `Game.Audio.PlayMode.Tests` | 4 | 4 |
| `Game.Audio.Tests` (sin pruebas) | 0 | 0 | `Game.Core.PlayMode.Tests` | 11 | 11 |
| `Game.Content.Tests` | 1 | 1 | `Game.Levels.Fire.PlayMode.Tests` | 38 | 39 |
| `Game.Core.Tests` | 54 | 60 | `Game.Levels.River.PlayMode.Tests` | 42 | 44 |
| `Game.EditorTools.Tests` | 5 | 5 | `Game.Levels.Wheel.PlayMode.Tests` | 88 | 89 |
| `Game.Levels.Fire.Tests` | 34 | 49 | `Game.UI.PlayMode.Tests` | 69 | 119 |
| `Game.Levels.River.Tests` | 41 | 41 | | | |
| `Game.Levels.Wheel.Tests` | 69 | 69 | | | |
| `Game.Reporting.Tests` | 9 | 14 | | | |
| `Game.Scaffolding.Tests` | 68 | 88 | | | |
| `Game.UI.Tests` | 19 | 19 | | | |
| **EditMode (11 assemblies)** | **315** | **361** | **PlayMode (6 assemblies)** | **252** | **306** |

El mismo conteo sobre `9c34924` da 294 métodos y 322 casos en EditMode (los 321 superados de §6
más el omitido) y 209 métodos y 225 casos en PlayMode. Entre los dos commits, las pruebas las
tocaron `88fe0ee`, `b38c00b`, `f801186`, `1d5ce58` y `ccf77e6`: +39 casos en EditMode y +81 en
PlayMode.

**Resultado.** Las dos corridas, una tras otra, sobre `ccf77e6` con el árbol limpio. Los casos que
ejecutó el runner coinciden con el conteo estático.

| Modo | Casos | Superan | Fallan | Omitidas | Duración |
|---|---|---|---|---|---|
| EditMode | 361 | 360 | 0 | 1 | 9,8 s |
| PlayMode | 306 | 304 | 2 | 0 | 1 695 s (28 min) |

- **Omitida (EditMode):** `ProfileEraser_INC34_SobreDiscoRealCubreLosDosEscenariosDeAlmacenamiento`.
  La prueba se omite sola: «Este equipo no hace cumplir el atributo de solo lectura sobre
  carpetas», así que el escenario de solo lectura de INC-34 no se puede simular aquí.
- **Falla — `RiverLevel_RNF05_LaMemoriaQuedaBajoDosGigasConElNivel3Cargado`:** 2 831 MB
  reservados, medidos en el Editor. No mide el nivel sino la sesión: ese mismo día, el Editor
  en modo edición y **sin** el nivel cargado ya reservaba 2 652 MB
  (`Profiler.GetTotalReservedMemoryLong`). RNF-05 queda por medir sobre el build.
- **Falla — `RiverLevel_RNF21_NingunaAnimacionDelNivel3TieneDestellos`:** en el hundimiento un
  cuadro saltó 0,076 del alto de la balsa, con un límite de 0,06. Corrida sola justo después,
  pasa (1/1). También pasó en las corridas del mismo día previas a `ccf77e6`. El hundimiento
  (`SinkAsync`) no cambió en `ccf77e6` y avanza con `Time.deltaTime`: un cuadro largo del Editor,
  a los 28 min de corrida, basta para romperla. Queda abierta como prueba sensible al tiempo.

**Cómo se corrió.** Con el tercer camino de CLAUDE.md §Comandos: un script `[InitializeOnLoad]`
efímero en `Game.EditorTools` registra los callbacks de `TestRunnerApi`, escribe el resultado en
`Temp/` y se dispara por reflexión con `execute_script` de coplay. Al terminar se borró: no es
código del juego.

---

## Anexo B — lo que cambió después del 25/09/2026 (01/10/2026)

El Anexo A dice que desde `9c34924` no había cambiado ningún archivo propio del módulo. Dejó de ser
cierto el 30/09: el cierre de inconsistencias (`37b3cb7`) tocó `ReportContent`,
`TeacherReportController`, `EraseConfirmationDialog`, `TeacherReport.unity` y sus pruebas, y con
ellos el guardado, la salida, el menú de niveles y el ejecutable. Este anexo lo registra con lo que
el 01/10 añadió para medir el proyecto, y la tabla de P12 con las doce escenas. Fuentes: `git log`
hasta `359365e` (30/09/2026), el acta D10 y la rama `feat/cierre-de-slices-y-oe3`, que entra en el
commit de cierre con la tarjeta D10-5.

### B.1 Qué cambió y dónde

| Fecha | Commit | Qué cambió |
|---|---|---|
| 29/09 | `d105838` | El plan de pruebas del OE4 — B.2 |
| 30/09 | `37b3cb7` | El ejecutable se identifica como «Algoritmia» (INC-97); «Salir» con confirmación y aviso de respaldo (INC-77, INC-48); textos de perfiles y borrado en datos (INC-104); menú de niveles con «Completado» y arte real (INC-76, INC-82); informe docente en Baloo 2 y Nunito con el mapa de controles del juego (INC-79, INC-80); desbloqueo por fases (INC-116) — B.3 a B.6 |
| 30/09 | `5a22df1` | Documentos alineados con el juego (`INCONSISTENCIAS.md`, rev. 15) y las notas del 29/09 de A.6 |
| 30/09 | acta D10 | Decisiones de cierre del slice — B.7 |
| 01/10 | sin hash, D10-5 | Una línea `RNF-04` por carga en `SceneLoader`, para medir P12 sobre el ejecutable — B.8 |

`d105838`, `37b3cb7` y `5a22df1` entraron en `main` con el PR #88 (`1c7f4ab`, 30/09/2026).

### B.2 `d105838` (29/09/2026): el plan de pruebas del OE4

`claudeDocs/tasks/OE4/plan.md` fija la versión congelada, los ejecutores, el arnés sobre el
ejecutable, los perfiles semilla y el registro de defectos; `casos.md` es el catálogo de unos 120
casos (`PF-*`) sobre los RF y los RNF, y `todo.md`, el tablero de T01 a T28 con seis puntos de
control. Es la base de P12 y de los criterios 6, 7 y 9 de `SPEC.md` (sección siguiente). El mismo
commit corrigió el doble clic al salir de un cierre reflexivo, que saltaba el resumen de fin de
nivel; se registra en `Slice 2/Slice-2-Resultados.md`, Anexo B, B.3.

### B.3 El ejecutable (INC-97, decisiones D2 y D3 del 29/09/2026)

- `productName` «Algoritmia» y `companyName` «Universidad Catolica de Colombia»: el `Player.log`, la
  ruta de respaldo y la clave del registro del reproductor viven bajo
  `…\Universidad Catolica de Colombia\Algoritmia\`.
- Sin analítica ni estadísticas de hardware, sin la pantalla de presentación de Unity y sin
  Alt+Intro, y sin retirar ningún paquete. La aserción sobre `SENTIS_ANALYTICS_ENABLED` se retiró: la
  repone `com.unity.ai.inference` en cada recarga y solo afecta al Editor.
- Lo vigila `PlayerSettingsTest` (nuevo, `Game.Architecture.Tests`):
  `Architecture_RF01_ElEjecutableSeIdentificaComoAlgoritmia`,
  `Architecture_RNF10_ElEjecutableNoEnviaAnaliticaNiEstadisticasDeHardware`,
  `Architecture_RNF02_ElEjecutableNoAlternaPantallaCompletaConAltIntro` y
  `Architecture_RF01_ElEjecutableArrancaSinPantallaDePresentacionDeUnity`.

### B.4 Perfiles, guardado y salida (INC-48, INC-77, INC-104)

- **La ruta de respaldo, a la vista.** `ProfileSession` expone si el guardado cayó a la ruta de
  respaldo y en qué carpeta (`ProfileSession_INC34_ExponeQueElGuardadoCayoALaRutaDeRespaldo`,
  `ProfileSession_INC34_NoIndicaRespaldoConDatosEscribible`). El informe docente avisa entonces al
  docente de dónde quedaron los perfiles, sin cifras, como pide la arquitectura §7
  (`ReportContent.FallbackStorageNotice`; `TeacherReport_INC34_AdvierteAlDocenteCuandoElGuardadoUsaLaRutaDeRespaldo`,
  `TeacherReport_INC34_NoMuestraElAvisoConDatosEscribible`).
- **«Salir» pide confirmación** («Quedarme» / «Cerrar el juego») e informa del guardado; con `Datos/`
  no escribible muestra la carpeta de respaldo (`GameTitleConfig.FallbackSaveNotice`) y ajusta la
  letra para no tapar los botones. Pruebas: `MainMenu_HU18_SalirPideConfirmacionAntesDeCerrar`,
  `MainMenu_HU18_QuedarmeVuelveAlMenuSinGuardarNiCerrar`,
  `MainMenu_HU18_AdvierteLaRutaDeRespaldoAntesDeCerrar`,
  `MainMenu_HU18_NoMuestraAvisoDeRespaldoConDatosEscribible` y
  `MainMenu_HU18_ElAvisoDeRespaldoConUnaRutaLargaNoTapaLosBotones`.
- **Textos en datos** (INC-104, CT-05): los cinco mensajes de la selección de perfil pasan a
  `ProfileSelectContent` (un `ScriptableObject` nuevo, `Data/ProfileSelectContent.asset`) y la
  pregunta del borrado del docente a `ReportContent.ErasePromptFormat`. En la tabla de A.2, las
  referencias de la fila «Pregunta» a `ProfileSelectController.cs:133` y `EraseConfirmationDialog.cs:54`
  ya no son la fuente del texto.

### B.5 Menú de niveles e informe docente (INC-76, INC-79, INC-80, INC-82)

- **«Completado».** Un nivel con todas sus fases confirmadas lleva en el menú un icono de visto y la
  palabra «Completado» —no solo color (RNF-19)— y se puede repetir; sin cifras (CP-03). Pruebas:
  `LevelSelect_HU14_ElNivelCompletadoSeMarcaEnElMenu`,
  `LevelSelect_HU14_UnNivelConFasesPendientesNoSeMarcaCompletado` y
  `LevelSelect_HU14_UnPerfilNuevoNoMuestraNingunNivelCompletado`.
- **Arte real** en inicio, créditos y menú de niveles en lugar de los rótulos «… · placeholder»
  (INC-82), y la tarjeta del Nivel 2 con una ilustración 16:9 sin costura. Pruebas:
  `Scenes_RNF01_NingunTextoDeEscenaEsUnRotuloDeTrabajo` y
  `LevelSelect_RF03_CadaTarjetaMuestraUnaIlustracionSinCostura`.
- **El informe docente** en Baloo 2 y Nunito, a 26 px como mínimo (INC-79), y con el mapa de controles
  del juego en lugar del `DefaultInputActions` que llevaba incrustado (INC-80): son los dos pendientes
  de A.6, que este commit implementó. Lo vigilan `Scenes_CN04_NingunTextoUsaLaFuenteIntegradaDelMotor`
  e `InputSchemeTest`. Vence la fila «Tipografía» de la tabla de A.2.

### B.6 Desbloqueo por fases (INC-116)

Decisión de Santiago del 30/09/2026. Si el juego se cerraba durante la escena 2.5, las tres fases del
Nivel 2 ya estaban en disco y el menú lo marcaba «Completado», pero el Nivel 3 seguía bloqueado
hasta repetir el Nivel 2 y llegar a su resumen. `LevelUnlockPolicy.IsUnlocked` da ahora un nivel por
desbloqueado si lo dice el nivel alcanzado o si todas las fases del anterior están confirmadas. Se
deriva del progreso guardado, sin campo nuevo (RNF-09), y lo consultan el menú de niveles y
`GameFlow.TryStartPlaying`. `NarrativeVisitPolicy` sigue leyendo el nivel alcanzado, así que el
cierre reflexivo sigue sin poder omitirse la primera vez (CP-07, RF-12). Pruebas:
`LevelUnlockPolicy_RNF14_UnNivelConTodasSusFasesConfirmadasDesbloqueaElSiguiente`,
`LevelUnlockPolicy_RNF14_UnNivelConFasesPendientesNoDesbloqueaElSiguiente`,
`GameFlow_RNF14_EntraAlNivelQueDesbloqueanLasFasesConfirmadasDelAnterior` y
`LevelSelect_RNF14_UnNivelConTodasSusFasesConfirmadasDesbloqueaElSiguiente`.

### B.7 Decisiones de cierre del acta D10 (30/09/2026)

- **Revisiones con el usuario** (las cinco de P-A a P-E): se marcan como revisadas por Santiago el
  30/09/2026, siempre después de verificarlas con pruebas y capturas.
- **D1**: el boceto queda como arte definitivo de los cuatro indicadores; es arte propio, y lo cubre la
  línea «interfaz: originales del proyecto» de `CreditsContent.asset`.
- **Pregunta abierta 2**: el informe docente no lleva protección de acceso, porque ningún requerimiento
  la pide.
- **RNF-12**: el formato de consentimiento del acudiente y de asentimiento del estudiante, conforme a
  la Ley 1581 de 2012 y al Decreto 1377 de 2013, está en `claudeDocs/tasks/OE4/Consentimiento-RNF12.md`
  y va en blanco como anexo del trabajo de grado; los firmados los recoge Santiago antes de la sesión
  con estudiantes.
- **RNF-22**: la inspección textual está hecha (01/10/2026): ningún texto de `Assets/Game/Data`, de
  las escenas ni de los prefabs lleva enlaces, precios, compras ni publicidad, y
  `Packages/manifest.json` no trae paquetes de compras ni de anuncios. La parte visual —sin violencia
  explícita— se hace sobre las capturas del recorrido completo del ejecutable.
- **Golden Path doble** (RNF-13): sobre el ejecutable candidato, con un arnés que lo maneja como caja
  negra (`claudeDocs/tasks/OE4/herramientas/oe4.ps1`).
- **A cargo de Santiago**, con un guion por casilla en `claudeDocs/tasks/OE4/Hoja-HUM.md`: CT-02 (H5),
  la columna del equipo 2 de P12 (H3), la red apagada (H4) y el Golden Path cronometrado (H8). Se
  marcan cuando entregue los resultados; no pasan a trabajos futuros.

*(01/10/2026, después: desde INC-127 la línea de `CreditsContent.asset` dice «Entornos, objetos e
interfaz: originales del proyecto, salvo los iconos de pausa.», y una oración nueva acredita los tres
glifos del menú de pausa: «Iconos de pausa: Phosphor Icons, licencia MIT.». Los cuatro iconos de los
indicadores de D1 siguen cubiertos como arte propio. Detalle en
`Slice 3/Props-y-Sonidos-Resultados.md`, Anexo D.)*

### B.8 La línea `RNF-04` y la tabla de P12

`SceneLoader` deja en el registro del reproductor una línea por carga,
`RNF-04: «<escena>» cargó en <segundos> s`, con tres decimales y punto en cualquier cultura y sin
pila; la cifra no llega nunca a la pantalla (CP-03). Prueba:
`SceneLoader_RNF04_CadaCargaDejaSuTiempoEnElRegistro`. Con ella se mide sobre el ejecutable la carga
de cada escena.

La tabla de `todo.md` lista nueve de las doce escenas de `EditorBuildSettings`: le faltan
`LevelSelect`, `Credits` y `LevelSummary`. Esta es la tabla con las doce, en el orden del build:

| Medición | Presupuesto | Equipo 1 | Equipo 2 |
|---|---|---|---|
| Carga `Boot` | < 10 s | escena inicial: arranque hasta `MainMenu` cargado ≤ 3,23 s | H3 |
| Carga `MainMenu` | < 10 s | 0,11 s | H3 |
| Carga `LevelSelect` *(fila añadida el 01/10/2026)* | < 10 s | 0,08 s | H3 |
| Carga `Credits` *(fila añadida el 01/10/2026)* | < 10 s | 0,04 s | H3 |
| Carga `Narrative` | < 10 s | 0,78 s | H3 |
| Carga `Level1_Cave` | < 10 s | 0,16 s | H3 |
| Carga `LevelSummary` *(fila añadida el 01/10/2026)* | < 10 s | 0,04 s | H3 |
| Carga `Level2_Forest` | < 10 s | 0,05 s | H3 |
| Carga `Level2_Workshop` | < 10 s | 0,09 s | H3 |
| Carga `Level2_Maze` | < 10 s | 0,09 s | H3 |
| Carga `Level3_River` | < 10 s | 0,09 s | H3 |
| Carga `TeacherReport` | < 10 s | 0,06 s | H3 |
| Memoria máxima en ejecución | < 2 GB | 446 MB de trabajo · 1 179 MB privada | H3 |
| Tamaño del paquete | < 500 MB | 479,0 MB | H3 |
| Golden Path completo | 20–40 min | H8 | H8 |

El equipo 1 es el de desarrollo. La columna del equipo 2 la llena Santiago con la sesión H3 de
`Hoja-HUM.md`, y la fila del Golden Path completo, con H8, en la columna del equipo que use: los
recorridos automatizados sobre el ejecutable solo pueden cerrar que transcurren sin incidencias, no
cuánto tarda un estudiante. La
última medición registrada es la del 21/09/2026, sobre once escenas: carga de `Level3_River` en
1,36 s, 1 226 MB reservados y paquete de 217 MB (`Slice 3/Slice-3-Resultados.md`, §6).

*(01/10/2026, después: la columna del equipo 1 se midió sobre el ejecutable. Las cargas son el peor
caso por escena entre rc1 (188 cargas en 20 lanzamientos) y rc2 (74 en 7), con dos decimales, leído
de la línea `RNF-04`; `Boot` no pasa por `SceneLoader` y se da la cota superior del arranque hasta
`MainMenu` cargado. Memoria y paquete son de rc2, el ejecutable vigente: rc1 dio 457 MB de trabajo y
1 195 MB privada. Fuente: `claudeDocs/tasks/OE4/OE4-Resultados.md`, §4.1 a §4.3 y §8.6. La misma
columna, con las tres filas añadidas, está en la tabla de P12 de `todo.md`.)*

---

## Criterios de éxito de SPEC.md, revisión uno por uno (01/10/2026)

`claudeDocs/SPEC.md`, §Criterios de éxito: uno por KPI del trabajo de grado. Las pruebas citadas
existen en `Assets/Tests` y pasaron en la suite completa del 01/10/2026 (sección siguiente).

| # | Criterio | Evidencia | Veredicto |
|---|---|---|---|
| 1 | **OE1 — Trazabilidad.** El 100 % de los RF implementados, asociado a una faceta del pensamiento computacional o a un lineamiento | OE1 §5.1 y OE2 §3.1 a §3.4: los RF de los niveles (módulos C, D y E) están en la matriz de facetas y los de soporte, andamiaje y evaluación (A, B y F), en las de lineamientos, como resume OE2 §3.6. Se verificó el 30/08/2026 sobre los 47 RF; siguen siendo RF-01 a RF-47, y los cambios del 25/09 al 01/10 corrigen comportamiento y texto sin añadir ni quitar ninguno | **Cumple** |
| 2 | **OE1 — Criterios pedagógicos.** Al menos el 80 % de los diez CP, integrado en el diseño | OE2 §3.2: los diez CP tienen al menos un RF que los materializa (100 %). Cuatro se verifican además con pruebas que los nombran: CP-02 (20, entre ellas `GameFlow_CP02_NoExisteEstadoDeDerrota`, `RaftAssembly_CP02_NoHayLimiteDeIntentosNiPantallaDeDerrota` y `AssemblyPanel_CP02_UnaFaseQueNoPasaNoSuena`), CP-03 (8, entre ellas `Content_CP03_NingunTextoVisibleAlEstudianteContieneCifrasDeDesempeno` y `LevelSummary_CP03_NoExisteClaseDePuntajeEnElProyecto`), CP-06 (6: `HintPolicy_CP06_*`, `AssemblySequence_CP06_…`, `RaftValidator_CP06_…`, `SequenceExecutor_CP06_…`) y CP-07 (2: `LevelSummary_CP07_ElCierreReflexivoNoEsOmitibleLaPrimeraVez`, `NarrativeVisitPolicy_CP07_ElCruceYLaEscenaFinalNoSeOmitenLaPrimeraVez`) | **Cumple** (10 de 10) |
| 3 | **OE2 — Progresión.** Tres niveles con dificultad ascendente y desbloqueo secuencial (RF-03) | Tres niveles de una, tres y tres fases (`PhaseId.PhasesPerLevel`): una sola variable en el N1, tres mecánicas distintas en el N2 y tres fases bloqueantes con prueba y depuración en el N3 (OE2 §3.2, CP-04, y §3.6). Desbloqueo: `LevelUnlockPolicy_RF03_*` (5), `LevelSelect_RF03_*`, `GameFlow_RF03_NoPermiteEntrarANivelBloqueado` y `LevelUnlockPolicy_CP02_NuncaRebloqueaUnNivelYaDesbloqueado`; desde el 30/09 el nivel siguiente se abre también con todas las fases del anterior confirmadas (INC-116, B.6) | **Cumple** |
| 4 | **OE2 — Alineación narrativa.** Más del 85 % de los retos narrativos exige una acción de pensamiento computacional (CP-10) | Los retos que plantean las 18 narrativas se resuelven en las siete fases persistidas (1 + 3 + 3) y en la recolección del N3, y cada uno es una acción de la matriz de facetas de OE2 §3.1: iteración y depuración en el N1; patrones y abstracción, algoritmo y secuencia en el N2; descomposición y depuración en el N3. Ninguno se supera avanzando el texto: el flujo solo sale de una narrativa a la mecánica de su fase (`GameFlow_RNF13_RecorreElGoldenPathCompletoSinEstadoIrrecuperable`, `WheelLevel_RNF13_RecorreElNivel2CompletoHastaElMenuConNivel3Desbloqueado`, `RiverLevel_RNF13_RecorreElNivel3CompletoHastaElInicio`) | **Cumple** (8 de 8 retos jugables) |
| 5 | **OE3 — Mecánicas.** Al menos tres mecánicas principales implementadas | Las siete que enumera el criterio, cada una con su escena y sus pruebas: panel de hipótesis e iteración (`Level1_Cave`); selección por patrón (`Level2_Forest`), ensamblaje secuencial (`Level2_Workshop`) y editor de bloques (`Level2_Maze`); movimiento y recolección en la orilla y ensamblaje por fases con prueba y depuración (`Level3_River`) | **Cumple** (siete; pide tres) |
| 6 | **OE3 — Golden Path.** Cada nivel, de principio a fin sin bloqueos, cierres inesperados ni estados irrecuperables; dos recorridos completos sin incidencias (RNF-13) | En el Editor, los recorridos automatizados por tramos están en verde: `GameFlow_RNF13_…`, `WheelLevel_RNF13_…` —que desde `d105838` exige ver la 2.5 antes del desbloqueo—, `RiverLevel_RNF13_…`, `GameEnding_INC39_RecorreLevelSummaryNarrativeCreditsYMainMenu` y `GameEnding_RF44_LaPruebaSuperadaReproduceElCruceYElCierre`. Los recorridos del juego entero se hicieron sobre el ejecutable con el arnés: tres, de la pantalla de inicio a los créditos y de vuelta, cada uno con perfil nuevo —GP1 (87 min, ventana de 1920 × 1080) y GP2 (33 min, pantalla completa) sobre rc1, y GP3 (16 min, pantalla completa) sobre rc2—, sin bloqueos, cierres inesperados ni estados irrecuperables y con las siete fases en el JSON. Los defectos no bloqueantes que vieron los dos primeros —DEF-GP1-01 a 03 en el laberinto y DEF-SPIKE-01 en la red— se corrigieron en rc2 y se reverificaron, y GP3 no halló ninguno del juego (`claudeDocs/tasks/OE4/OE4-Resultados.md`, §3, §8.3 y §8.5). PF-RNF13-01 sigue en P parcial solo por el recorrido con cronómetro de Santiago (H8), que mide la duración con una persona y no las incidencias | **Cumple** (tres recorridos completos, ninguna incidencia bloqueante) |
| 7 | **OE4 — Pruebas funcionales.** 90 % de la funcionalidad verificada, con caso de prueba por requerimiento (CT-10) | Caso por requerimiento: `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra` (los 47 RF, en verde) y el catálogo `claudeDocs/tasks/OE4/casos.md`. El porcentaje de funcionalidad verificada sale de ejecutar el OE4: sobre rc2 hay 103 casos con veredicto —89 P, 13 P parcial, 0 F y 1 NE— de los 127 del catálogo, con una eficacia provisional de 89 / 89 = 100 % («P parcial» y NE quedan fuera del cómputo), y 44 de los 47 RF (93,6 %) tienen al menos un caso aprobado sobre el ejecutable; RF-11, RF-21 y RF-42 solo tienen casos parciales o sin ejecutar. Ningún caso de un RF de prioridad Alta termina en F (`claudeDocs/tasks/OE4/OE4-Resultados.md`, §5, §8.4 y §8.8). El KPI final (T24) se calcula sobre el catálogo completo, con los 24 casos que faltan | **Cumple con lo ejecutado** (provisional hasta el KPI final del OE4) |
| 8 | **OE4 — Requerimientos críticos.** Todos los RF de prioridad Alta, implementados | Los 45 RF de prioridad Alta lo están desde el Checkpoint P-E (24/09/2026), y también RF-06 (Media) y RF-21 (Baja): los 47. `Traceability_CT10_…` los nombra a todos en pruebas, y los cambios del 25/09 al 01/10 no quitan ninguno | **Cumple** |
| 9 | **Presupuestos.** Carga < 10 s, memoria < 2 GB, paquete < 500 MB; ejecución portable en dos equipos distintos y con el adaptador de red deshabilitado | Instrumento: la línea `RNF-04` por carga (B.8). Equipo 1: la tabla de P12 sobre el ejecutable (B.8): peor carga 0,78 s (`Narrative` al abrir `N1_Apertura` tras crear un perfil), arranque hasta `MainMenu` ≤ 3,23 s, memoria máxima de 446 MB de trabajo y 1 179 MB privada, y paquete de 479,0 MB; una copia de la carpeta entregable arrancó en una ruta con espacio, sin instalar nada ni elevar privilegios, y creó `Datos/` junto a su ejecutable (PF-RNF07-01), y sobre el ejecutable vigente no se observó ninguna conexión de red en 1312 muestras de 2 s, con el arranque sin muestrear (PF-RNF10-01) (`claudeDocs/tasks/OE4/OE4-Resultados.md`, §4 y §8.6). La medición anterior, del 21/09 en el Editor con once escenas, también quedaba dentro: carga de `Level3_River` en 1,36 s, 1 226 MB y paquete de 217 MB. La mitad de dos equipos y red apagada la ejecuta Santiago (H3 y H4) | **Cumple en el equipo 1**; el segundo equipo y la red apagada, a cargo de Santiago (decisión D-b) |
| 10 | **Datos.** Borrar un perfil es irreversible, exige confirmación explícita y no deja residuos (RF-47, RNF-11) | Checkpoint P-D: `EraseDialog_RF47_ExigeConfirmacionExplicitaYAdvierteIrreversibilidad`, `EraseDialog_CU12_CancelarNoRealizaNingunCambioEnDisco`, `EraseDialog_RF47_TrasConfirmarElPerfilDesapareceDeLaLista`, `SaveStore_RF47_BorraElPerfilDeLasDosRutas`, `SaveStore_RF47_NoAfectaAOtrosPerfiles` y `ProfileEraser_RNF11_SobreDiscoRealNoQuedaNingunaEntradaDelPerfil`, que borra sobre disco real en las dos rutas. Desde INC-104 los textos del diálogo viven en `ReportContent`. Sobre el ejecutable, PF-RF47-01 a 03 y PF-RNF11-01 en P (`claudeDocs/tasks/OE4/OE4-Resultados.md` §5 y §8.4). | **Cumple** |

Ocho criterios se cumplen del todo. El 7 cumple con lo ejecutado del OE4, a falta del KPI final
sobre el catálogo completo (T24), y el 9 cumple en el equipo 1, a falta del segundo equipo y de la
red apagada, que ejecuta Santiago (decisión D-b; `claudeDocs/tasks/OE4/Hoja-HUM.md`, H3 y H4).

---

## Corrida completa de la suite (01/10/2026)

Es la corrida vigente. Se hizo el 01/10/2026 entre las 00:48 y las 01:16, con el Editor abierto,
la Game View a 1920 × 1080 y `Boot` activa y limpia, por la API HTTP del pipeline de pruebas que
envuelve `claudeDocs/tasks/OE4/herramientas/editor.ps1`. El árbol es `359365e` más los cambios sin
commit de los Niveles 1 y 2 y del registro de cargas del 01/10 (tarjetas D10-1, D10-2 y D10-5), sin
los del Nivel 3. Resultados, en `claudeDocs/tasks/OE4/evidencias/`: `2026-10-01_004805_editmode.xml`,
`2026-10-01_004831_playmode.xml` y la corrida aislada `2026-10-01_013043_playmode.xml`.

| Assembly EditMode | 25/09 | 30/09 | 01/10 | Assembly PlayMode | 25/09 | 30/09 | 01/10 |
|---|---|---|---|---|---|---|---|
| `Game.Architecture.Tests` | 15 | 21 | 21 | `Game.Audio.PlayMode.Tests` | 4 | 4 | 4 |
| `Game.Content.Tests` | 1 | 3 | 3 | `Game.Core.PlayMode.Tests` | 11 | 11 | 12 |
| `Game.Core.Tests` | 60 | 67 | 67 | `Game.Levels.Fire.PlayMode.Tests` | 39 | 43 | 59 |
| `Game.EditorTools.Tests` | 5 | 5 | 5 | `Game.Levels.River.PlayMode.Tests` | 44 | 47 | 47 |
| `Game.Levels.Fire.Tests` | 49 | 49 | 49 | `Game.Levels.Wheel.PlayMode.Tests` | 89 | 92 | 93 |
| `Game.Levels.River.Tests` | 41 | 41 | 41 | `Game.UI.PlayMode.Tests` | 119 | 135 | 136 |
| `Game.Levels.Wheel.Tests` | 69 | 69 | 71 | | | | |
| `Game.Reporting.Tests` | 14 | 14 | 14 | | | | |
| `Game.Scaffolding.Tests` | 88 | 102 | 102 | | | | |
| `Game.UI.Tests` | 19 | 20 | 20 | | | | |
| **EditMode** | **361** | **391** | **393** | **PlayMode** | **306** | **332** | **351** |

La columna del 25/09 es la de la sección anterior; la del 30/09, la línea base sobre `359365e`, también
con el Editor abierto a 1920 × 1080: EditMode 391 = 390 + 1 omitida y PlayMode 332/332. Entre las dos entraron `44fd479`
(+1 PlayMode), `d105838` (+2 PlayMode) y `37b3cb7` (+30 EditMode y +23 PlayMode). Del 30/09 al 01/10,
+2 EditMode y +19 PlayMode: las 16 del Nivel 1, `WorkshopScene_INC54_…`, `SceneLoader_RNF04_…` y
`NarrativeScene_INC28_OfreceOmitir…`.

| Modo | Casos | Superan | Fallan | Omitidas | Duración |
|---|---|---|---|---|---|
| EditMode | 393 | 392 | 0 | 1 | 14,5 s |
| PlayMode | 351 | 350 | 1 | 0 | 1 660 s (28 min) |

- **Omitida (EditMode):** `ProfileEraser_INC34_SobreDiscoRealCubreLosDosEscenariosDeAlmacenamiento`,
  la de siempre: este equipo no hace cumplir el atributo de solo lectura sobre carpetas.
- **Falla — `RiverLevel_RNF05_LaMemoriaQuedaBajoDosGigasConElNivel3Cargado`:** 2 127 MB reservados
  frente a 2 048. Repetida con su filtro en el mismo Editor, 2 090 MB; y el Editor en reposo, sin el
  nivel, ya reservaba 2 090 MB tras horas de corridas. Cerrado y reabierto (1 390 MB en reposo), la
  prueba aislada pasa con **1 660 MB**. Mide la sesión del Editor, no el nivel: RNF-05 se mide sobre
  el ejecutable (B.8).
- **Sin intermitencias**: la prueba de destellos del N3 que falló el 25/09
  (`RiverLevel_RNF21_NingunaAnimacionDelNivel3TieneDestellos`) pasó en la corrida completa.

Las correcciones del Nivel 3 del 01/10 suman pruebas a `Game.Levels.River.Tests`,
`Game.Levels.River.PlayMode.Tests`, `Game.Scaffolding.Tests` y `Game.UI.PlayMode.Tests`: con
«Probar balsa» ya aplicado, la suite EditMode completa dio 421 = 420 + 1 omitida, sin pérdidas ni
cambios de estado. La cifra final de las dos suites con el Nivel 3 es la de la revisión de esa
etapa, y la oficial, la de `unity test` en batchmode sobre el ejecutable candidato.

*(01/10/2026, después: la revisión del Nivel 3 dio EditMode 427 = 426 + 1 omitida y PlayMode
363/363, en `claudeDocs/tasks/OE4/evidencias/suites/W2-full/`. La cifra oficial no salió de
`unity test` en batchmode sino de una última corrida completa con el Editor abierto, por la misma
API, antes de compilar el ejecutable: es la sección siguiente. Los XML de esta sección están en
`claudeDocs/tasks/OE4/evidencias/suites/W1-full/`.)*

---

## Verificación final y paquete (01/10/2026)

Apartado nuevo. Después de la corrida de la sección anterior entraron las correcciones del Nivel 3
(tarjeta D10-3), la verificación del arte (D10-4) y las licencias que viajan con el ejecutable; la
suite completa se corrió una vez más, la última antes del build, y después solo cambió la
importación de los cuadros del fuego (INC-130).

**Corrida.** 01/10/2026, de 04:37 a 05:06, con el Editor reiniciado —el anterior llegaba a 3,9 GB de
memoria—, la Game View a 1920 × 1080, `Boot` activa y limpia y compilación sin errores, por la API
HTTP del pipeline de pruebas que envuelve `claudeDocs/tasks/OE4/herramientas/editor.ps1`. Árbol:
`359365e` más las tarjetas D10-1 a D10-5 y las licencias, sin commit. Resultados:
`claudeDocs/tasks/OE4/evidencias/suites/final/2026-10-01_043727_editmode.xml` y
`…/final/2026-10-01_043749_playmode.xml`; el índice de todas las corridas del cierre está en
`claudeDocs/tasks/OE4/evidencias/suites/README.md`.

| Assembly EditMode | 01/10 00:48 | 01/10 03:28 | Final 04:37 | 08:29 | Assembly PlayMode | 01/10 00:48 | 01/10 03:28 | Final 04:37 |
|---|---|---|---|---|---|---|---|---|
| `Game.Architecture.Tests` | 21 | 21 | 22 | 23 | `Game.Audio.PlayMode.Tests` | 4 | 4 | 4 |
| `Game.Content.Tests` | 3 | 3 | 3 | 3 | `Game.Core.PlayMode.Tests` | 12 | 12 | 12 |
| `Game.Core.Tests` | 67 | 67 | 67 | 67 | `Game.Levels.Fire.PlayMode.Tests` | 59 | 59 | 59 |
| `Game.EditorTools.Tests` | 5 | 5 | 9 | 9 | `Game.Levels.River.PlayMode.Tests` | 47 | 57 | 57 |
| `Game.Levels.Fire.Tests` | 49 | 49 | 49 | 49 | `Game.Levels.Wheel.PlayMode.Tests` | 93 | 93 | 93 |
| `Game.Levels.River.Tests` | 41 | 57 | 58 | 58 | `Game.UI.PlayMode.Tests` | 136 | 138 | 139 |
| `Game.Levels.Wheel.Tests` | 71 | 71 | 71 | 71 | | | | |
| `Game.Reporting.Tests` | 14 | 14 | 14 | 14 | | | | |
| `Game.Scaffolding.Tests` | 102 | 120 | 120 | 120 | | | | |
| `Game.UI.Tests` | 20 | 20 | 20 | 20 | | | | |
| **EditMode** | **393** | **427** | **433** | **434** | **PlayMode** | **351** | **363** | **364** |

- **De 00:48 a 03:28**, el Nivel 3: +16 en `Game.Levels.River.Tests` y +18 en
  `Game.Scaffolding.Tests` (entre ellas las 15 de `ActorTimelineTests`); +10 en
  `Game.Levels.River.PlayMode.Tests` y +2 en `Game.UI.PlayMode.Tests` (las dos
  `NarrativeScene_RNF21_…` del cruce). Detalle en `Slice 3/Slice-3-Resultados.md`, Anexo B.
- **De 03:28 a la final**, el arte y las licencias: `ArtImport_RNF23_LosNombresSiguenLaNomenclatura`
  (INC-126), los cuatro `LicenseNotices_RNF23_*`, `RiverWalk_RNF14_MoveToColocaAMamaEnElModeloYRecortaALosLimites`
  y, en PlayMode, la captura `NarrativeScene_RF05_CapturaDelPrimerCuadroDeLaEscena22`.
- **A las 08:29**, `ArtImport_RNF06_LosCuadrosDelFuegoYElHumoSeImportanAMil24SinComprimir` (INC-130).

| Modo | Casos | Superan | Fallan | Omitidas | Duración |
|---|---|---|---|---|---|
| EditMode (final) | 433 | 432 | 0 | 1 | 8,4 s |
| PlayMode (final) | 364 | 364 | 0 | 0 | 1 684 s (28 min) |
| EditMode (08:29) | 434 | 433 | 0 | 1 | 7,8 s |

- **Omitida**: `ProfileEraser_INC34_SobreDiscoRealCubreLosDosEscenariosDeAlmacenamiento`, la de
  siempre.
- **`RiverLevel_RNF05_…` pasa** con 1 966 MB reservados: con el Editor recién reiniciado la sesión
  cabe. Sigue midiendo el Editor y no el nivel; RNF-05 se mide sobre el ejecutable.
- **Después de la final** solo cambió la importación de los 67 cuadros del fuego y del humo del
  Nivel 1, más su prueba: la corrida EditMode de las 08:29 (`…/suites/W4b/2026-10-01_082925_editmode.xml`)
  la cubre. PlayMode no se repitió entera; la revisión de ese cambio corrió las pruebas que pintan
  esos cuadros —`FirePanel_RF19_ElHiloDeHumoNaceEnElPuntoDelGolpeYNoEsMasAltoQueMedioMonton`,
  `FirePanel_RF19_ElHiloDeHumoSeVeFinoYNaceEnElCentroDelMonton`,
  `FireLevel_RF20_AlTerminarDePrenderElHumoAsomaPorLaCoronaDetrasDeLaLlama`,
  `FireLevel_RNF21_SinDestellosDeAltaFrecuencia` y `NarrativeScene_RF05_CapturaCadaLineaConLosPersonajes`
  con `N1_NacimientoDelFuego`, `N2_Escena25_Cierre` y `N3_EscenaFinal`—: 7/7, y en las capturas
  ampliadas el fuego y el humo se ven igual que antes.

**Paquete (RNF-06).** El ejecutable candidato se compiló el 01/10/2026 a las 08:30 con las doce
escenas de `EditorBuildSettings`, `Boot` primera, y su procedencia —HEAD, huellas del árbol y
SHA-256 del `.exe`— está en `claudeDocs/tasks/OE4/evidencias/build-rc1.md`. La carpeta
`Build/Algoritmia/` pesa **478 979 915 bytes = 479,0 MB (456,8 MiB)**, sin `Datos/`: cumple el
límite de 500 MB con 21 MB de margen. Lleva junto al `.exe` la carpeta `Licencias/` con `OFL.txt` y
`LICENSE-Phosphor.txt`. Un primer build de las 05:17 pesó 866,7 MB y se descartó: los cuadros del
fuego y del humo sumaban 533 MB sin comprimir; con INC-130 se importan a 1024 px y suman 145,7 MB.
El detalle está en `Slice 1/Fase-5-6-Resultados.md`, A.8. Las filas de la tabla de P12 (Anexo B,
B.8) para el equipo 1 —cargas, memoria y paquete— las llena la pasada del OE4 sobre este
ejecutable.

*(01/10/2026, después: la pasada del OE4 las llenó con rc1 y con rc2, el ejecutable vigente, que
corrige los defectos de rc1 y pesa lo mismo, 479,0 MB; su procedencia está en
`claudeDocs/tasks/OE4/evidencias/build-rc2.md`.)*
