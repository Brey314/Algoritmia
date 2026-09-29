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

- [ ] Pasar a Baloo 2 y Nunito los 20 textos de `TeacherReport` y la etiqueta «Progreso del
      equipo» de `MainMenu`, o dejar escrito por qué no (A.3).
- [ ] Decidir si los dos `ScrollRect` de `TeacherReport` se cambian por botones de desplazamiento
      o si se deja escrita la excepción (A.4).

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
