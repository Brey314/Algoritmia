# Inconsistencias entre los documentos fuente

Registro de los conflictos detectados entre los seis `.docx` de `docs/`, con la corrección
aplicada a cada uno. Documento hermano de `SPEC.md`: aquí está **qué estaba mal en los
documentos y cómo quedó**; allí está **qué implementa el código**.

**Verificación vigente: 30/08/2026, rev. 5.** Los seis documentos se releyeron de principio a
fin y se editaron directamente. **Todos los hallazgos INC-01 … INC-42 están cerrados**; quedan
solo dos residuos menores y los puntos abiertos del guion, listados al final.

> **Nota sobre esta revisión.** La rev. 4 dejaba veintitrés hallazgos abiertos. Entre esa
> revisión y esta se corrigieron en los `.docx`: los nueve que la rev. 4 mantenía abiertos
> (INC-01, 16, 21, 22, 24, 25, 26, 27, 28) y los catorce restantes (INC-29 … INC-42). Los
> cambios se registraron además en el control de cambios de OE1 (§6), OE2 (§4) y la arquitectura
> (§12). El guion y el trabajo de grado no tienen tabla de control de cambios; sus ediciones
> constan aquí.

---

## Cómo se resolvió un conflicto

**Orden de precedencia.** Cuando dos documentos se contradecían ganó el de mayor prioridad y se
corrigió el otro:

| # | Documento | Qué gobierna |
|---|---|---|
| 1 | `Trabajo_de_Grado_2026_ICONTEC_IEEE.docx` | Objetivos, KPI, alcance, marco jurídico, metodología |
| 2 | `OE1_Requerimientos (3).docx` | Lineamientos CP/CT/CN, RF-01..RF-47, RNF-01..RNF-23 |
| 3 | `Guion_Completo_Videojuego.docx` | Narrativa, mecánicas, parámetros y textos exactos |
| 4 | `OE2_historias_completas.docx` | CU-01..CU-12, HU-01..HU-18, matrices |
| 5 | `Historias_de_Usuario_HU01_HU18_v2.docx` | HU detalladas: flujos, criterios, reglas de negocio |
| 6 | `arquitectura_videojuego_v2.docx` | Decisiones técnicas de implementación |

Las contradicciones **internas** a un mismo documento se corrigieron editándolo.

---

## Resumen — estado a 30/08/2026 (rev. 5)

| ID | Hallazgo | Documentos | Estado |
|---|---|---|---|
| INC-01 | «Teclas de dirección» residual | Guion, HU, Arquitectura | **Cerrado** |
| INC-16 | La arquitectura no citaba `RNF-18` | Arquitectura | **Cerrado** |
| INC-21 | CN-04 trazaba a `RNF-22` (luego `RNF-21`) | OE2 | **Cerrado** |
| INC-22 | La introducción daba por concedidos los personajes | Trabajo de grado | **Cerrado** |
| INC-24 | Paginación de HU y cabecera de arquitectura | HU, Arquitectura | **Cerrado** (residuo menor: pies de página de HU-17/HU-18) |
| INC-25 | HU-17 llevaba el flujo y los criterios de otras historias | HU | **Cerrado** |
| INC-26 | HU-14 mostraba cifras al estudiante | HU | **Cerrado** |
| INC-27 | `RNF-09` no autorizaba el progreso que `RF-04` obliga a guardar | OE1 | **Cerrado** |
| INC-28 | La omisión de escenas limitada en OE1 y libre en el resto | OE1, OE2, HU | **Cerrado** |
| INC-29 | HU-10 nombraba indicadores fuera de la lista cerrada | HU | **Cerrado** |
| INC-30 | Tres fases de ensamblaje contra cuatro tareas | OE1, Guion, HU | **Cerrado** |
| INC-31 | Actores y referencias cruzadas erróneas | HU | **Cerrado** |
| INC-32 | `RF-19` recondicionaba «Soplar» tras la convergencia | OE1, HU, OE2 | **Cerrado** |
| INC-33 | Semántica de los bloques del laberinto sin definir | OE1, Guion | **Cerrado** (decisión: lectura relativa) |
| INC-34 | El fallback de persistencia no estaba en el criterio de `RNF-11` | Arquitectura | **Cerrado** |
| INC-35 | CU-11 presentaba los indicadores solo por nivel | OE2, Arquitectura | **Cerrado** |
| INC-36 | Datos internos inconsistentes en el trabajo de grado | Trabajo de grado | **Cerrado** |
| INC-37 | `RF-44` y `RF-46` no tenían historia de usuario | OE2, HU | **Cerrado** (absorbidos en HU-13 y HU-16) |
| INC-38 | La arquitectura citaba `TRAZABILIDAD.md`, que no existe | Arquitectura | **Cerrado** |
| INC-39 | Ninguna transición llevaba a `Credits` al terminar el Nivel 3 | Arquitectura | **Cerrado** |
| INC-40 | UI y Audio no tenían assembly en la lista de §9 | Arquitectura | **Cerrado** |
| INC-41 | HU-02 generalizaba la lista de tareas a todos los niveles | HU, OE1 | **Cerrado** |
| INC-42 | Norma de citación declarada distinta de la usada | Trabajo de grado | **Cerrado** |

---

## Detalle de cada corrección

### INC-01 · «Teclas de dirección» residual — cerrado
`RF-35` ya decía «botones de dirección mostradas en los costados… de la pantalla». Se corrigió
lo que quedaba: **guion §2.1 y §8.2** («con los botones de dirección en pantalla»), **HU-11**
(datos de entrada «Botones de dirección · Acción (clic)» y se suprimió la regla que declaraba
una «única excepción al control de solo clic»), **arquitectura §1** («entrada limitada a clic y
clic sostenido, incluidos los botones de dirección en pantalla del Nivel 3»). No queda ninguna
salvedad que debilite el criterio de verificación de `RNF-02`.

### INC-16 · La arquitectura no citaba `RNF-18` — cerrado
Se añadió `RNF-18` junto a `CT-05` en la tabla de restricciones (§1), en la fila «Datos ·
ScriptableObjects» (§6) y en la trazabilidad (§11).

### INC-21 · CN-04 → `RNF-22`/`RNF-21` — cerrado
OE2 §3.4: `CN-04` (coherencia visual) traza ahora **solo a `RNF-20`** (contraste). Se retiró la
referencia a `RNF-22` (violencia/publicidad) y a `RNF-21` (destellos), ninguna de las cuales
trata de coherencia visual.

### INC-22 · La introducción daba por concedidos los personajes — cerrado
El párrafo de impactos de la introducción del trabajo de grado condiciona ahora el uso de la
Familia Anonaky «—sujeto a la autorización escrita de sus autores, o personajes originales en su
defecto—», con la misma fórmula del resumen y de §3.3.2. Se corrigió además la redacción, que
partía el nombre del libro en dos.

### INC-24 · Paginación de HU y cabecera de arquitectura — cerrado (residuo menor)
- El documento de HU se renombró a `Historias_de_Usuario_HU01_HU18_v2.docx` y contiene HU-01 a
  HU-18. Los pies de página pasaron de «Página N de 16» a «Página N de 18» en las páginas 1 a 16.
- La arquitectura ya no dice «Reemplaza a `arquitectura_videojuego_v2.docx`» (el archivo *es*
  ese): la cabecera dice «Versión alineada; reemplaza a la revisión anterior de este mismo
  documento».
- **Residuo menor:** HU-17 y HU-18 se añadieron sin encabezado de página propio, así que no
  llevan «Página 17 de 18» / «Página 18 de 18». Requiere insertar la fila de encabezado en el
  `.docx` a mano; no afecta al contenido.

### INC-25 · HU-17 corrupta por copia y pega — cerrado
- El flujo básico termina en el paso 5 («El estudiante acciona Continuar»). Se eliminaron los
  pasos 6–8, que eran de HU-16 (borrar datos).
- Se eliminó el `FA-01` duplicado; los flujos alternos son ahora los de la pausa (FA-01 a FA-05).
- Los criterios de aceptación se reescribieron a partir de `RF-07` y OE2 §2 HU-17: eran una
  copia de los de HU-18 (menú principal, Salir, créditos, contraste de créditos). Ahora
  describen el botón de pausa, las tres opciones del menú, la detención del nivel, la
  restitución exacta al continuar, la confirmación de reinicio sin re-bloqueo y la ausencia de
  pantalla de derrota.
- HU-17 y HU-18 ya tienen su tabla de datos de entrada. Se eliminó una tabla de control de
  cambios huérfana al final del documento.

### INC-26 · HU-14 mostraba cifras al estudiante — cerrado
- Flujo básico, paso 6: «el sistema muestra el resumen narrativo del nivel: qué hizo el
  estudiante, contado en lenguaje observacional y sin cifras (`RF-45`, `RF-17`)».
- Datos de entrada: «Resumen narrativo (sistema) · Descripción de lo realizado en el nivel, sin
  valores numéricos. Los indicadores se registran en el perfil y se consultan solo desde
  `TeacherReport` (`RF-46`)».
- Criterios y reglas de negocio: el resumen «no usa cifras, calificaciones ni puntajes; describe
  en lenguaje narrativo lo que hizo el estudiante (`CP-03`, `RF-45`)».

### INC-27 · `RNF-09` vs `RF-04` — cerrado
`RNF-09` (OE1) admite ya «el nombre o alias del estudiante, su progreso de avance —nivel
alcanzado y fases confirmadas— y sus indicadores de desempeño». La nota 5 de §3.6.1 se ajustó en
el mismo sentido. La arquitectura §7 ya lo recogía. La intención del requerimiento —no recoger
datos sensibles, imágenes, ubicación ni contacto— queda intacta.

### INC-28 · Omisión de escenas «ya vistas» — cerrado
- **CU-03** flujo alterno 2a y **HU-02** FA-01: se añadió «si la escena ya fue vista».
- **HU-02** FA-02: se suprimió el «botón de bloqueo» inexistente.
- **HU-14**: el flujo alterno se partió en dos — FA-01 (nivel ya completado antes → botón de
  omitir hacia el resumen narrativo) y FA-02 (primera vez → el botón no se muestra: la escena de
  cierre se reproduce entera, porque es donde el guía nombra la habilidad practicada, `RF-12`,
  `CP-07`).

### INC-29 · HU-10 nombraba indicadores fuera de la lista cerrada — cerrado
La regla de negocio de HU-10 registra ahora «los cuatro indicadores de OE1 §3.6.1 con la
definición operativa de la fase 3»: intentos = ejecuciones que no alcanzan el refugio; errores
corregidos = bloques retirados o reordenados entre una ejecución fallida y la siguiente; pasos
utilizados = bloques de la secuencia ejecutada con éxito; tiempo de resolución.

### INC-30 · Tres fases de ensamblaje contra cuatro tareas — cerrado
- **Guion §8.1:** la lista de tareas visible pasó de tres entradas a las **cuatro de `RF-36`**:
  1 recoger troncos · 2 encontrar sogas · 3 ensamblar la balsa · 4 colocar el mástil y la vela.
- **OE1 §3.6.1** («Pasos utilizados», Nivel 3): «Confirmaciones de fase aceptadas, sobre un
  máximo de tres: base, amarre, y mástil y vela» — ya no se lee como cuatro.
- **HU-11** ya fijaba la correspondencia: tarea 3 se marca al confirmar la fase de amarre (cierra
  la estructura base + amarre), tarea 4 al confirmar mástil y vela; la fase de base no marca
  tarea por sí sola.
- Se limpió de `RF-40` la referencia incrustada «(Fase 2, #8.1. Gión, HU-12)».

### INC-31 · Actores y referencias cruzadas — cerrado
- **HU-18** *Actores*: «Docente o Estudiante» (antes «Docente, Docente»).
- **HU-18** *Historias asociadas*: la referencia a «borrar los datos» apunta a HU-16 (antes
  HU-15).
- **HU-01** *Actores*: «Estudiante (con acompañamiento del docente)».

### INC-32 · `RF-19` recondicionaba «Soplar» — cerrado
`RF-19` (OE1) activa «Soplar» «cuando el jugador haya acumulado el número mínimo de golpes
efectivos definido en la configuración del nivel, y permanece activo a partir de ese momento».
Se retiró la condición de posición. HU-07 (flujo, FA-01, criterios, reglas y datos de entrada) y
la HU-07 resumida de OE2 §2 se alinearon: «lo ganado permanece y el botón no vuelve a
deshabilitarse (guion §4.3.6, `CP-02`)».

### INC-33 · Semántica de los bloques del laberinto — cerrado (decisión de diseño)
`RF-31` (OE1) y el guion §6.3.2 fijan la **lectura relativa**: los bloques son «Avanzar»,
«Retroceder» y «Girar», interpretados respecto de la orientación actual de la carretilla;
«Avanzar» y «Retroceder» mueven una casilla adelante o atrás según hacia dónde mire, y «Girar»
la rota 90° en sentido horario. Con la lectura absoluta ninguna secuencia se desplazaba en
vertical y el refugio podía ser inalcanzable.

### INC-34 · Fallback de persistencia y criterio de `RNF-11` — cerrado
Arquitectura §7: «La eliminación de un perfil (`RF-47`) borra las dos rutas —`Datos/` y el
respaldo—, y la prueba de `RNF-11` se ejecuta en los dos escenarios: con `Datos/` escribible y
con `Datos/` de solo lectura».

### INC-35 · Granularidad del informe docente — cerrado
- **CU-11** (OE2), paso 4: «los indicadores por nivel y por fase».
- **Arquitectura §6**: `ProgressTracker` → «Indicadores por nivel y por fase».

### INC-36 · Datos internos del trabajo de grado — cerrado
- **Edad de la población:** §1.2 pasa de «9 y 10 años» a **9 y 11 años** (§3.4, §5.3, OE1 §2.3 y
  el guion ya decían 9–11).
- **Puntaje Bebras de grado cuarto:** §1.2 dice ahora «4,57 puntos sobre 15» (coincide con la
  introducción) y «5,08 de media entre todos los estudiantes»; se corrigió el orden de palabras.
- **Países en Bebras:** el glosario dice **78** (coincide con §3.1.2).
- **Duración / presupuesto:** los recursos humanos se facturan a **14 semanas** ($3.500.000 por
  integrante), el subtotal es **$7.000.000** y el total del proyecto **$40.135.811**, coherente
  con las «catorce semanas» del resumen, §5 y el cronograma.
- Se unificó el separador decimal de la media de Francia («7,5 sobre 15»).
- El subtotal de hardware ($23.722.411) y el resto del presupuesto ya cuadraban.

### INC-37 · `RF-44` y `RF-46` sin historia de usuario — cerrado (por absorción)
- **`RF-44`** (animación de cruce y cierre del juego) se asocia a **HU-13**, cuyo flujo ya
  reproduce «la animación del cruce del río y la escena de cierre (RF-44)». Añadido a su
  «Id. Requerimiento» y a la matriz OE2 §3.5.
- **`RF-46`** (consulta docente) se asocia a **HU-16**, cuyo flujo ya muestra «la lista de
  perfiles registrados en el equipo con sus indicadores de desempeño». Añadido a su
  «Id. Requerimiento» y a la matriz OE2 §3.5.
- **CU-11** tiene ahora la fila «Historias de usuario asociadas: HU-16», la única que faltaba
  entre los doce casos de uso.
- No se crearon HU-19/HU-20: la numeración de historias sigue cerrada en HU-01..HU-18.

### INC-38 · `TRAZABILIDAD.md` inexistente — cerrado
La cabecera de la arquitectura remite ahora a «las matrices de trazabilidad de OE1 §5.1 y OE2
§3.1–3.5».

### INC-39 · Nada llevaba a `Credits` al terminar el Nivel 3 — cerrado
La tabla de transiciones de la arquitectura §4 añade:
`LevelSummary` → juego completado (tras Nivel 3) → `Narrative` (escena final, guion §9) →
`Credits` → `MainMenu` (`RF-44`, `RF-08`).

### INC-40 · UI y Audio sin assembly — cerrado
La lista de assemblies de la arquitectura §9 incluye `Game.UI` y `Game.Audio`, «que dependen de
`Game.Core` y nunca a la inversa».

### INC-41 · HU-02 generalizaba la lista de tareas — cerrado
- Criterio de aceptación acotado: «En los niveles con lista de tareas (`RF-36`, Nivel 3) la
  lista permanece visible durante toda la escena jugable; los niveles 1 y 2 no tienen lista».
- Flujo básico, paso 4: «en el Nivel 3 las presenta como lista permanente en pantalla (`RF-36`)».
- Regla de negocio reconciliada con el criterio: «El guía mantiene una sola tarea activa a la
  vez (`RNF-03`); una lista que marque lo hecho y señale la siguiente no vulnera esa
  restricción».
- **OE1 `RNF-03`** lleva la misma aclaración: «la limitación recae sobre la tarea activa, no
  sobre cuántas se muestran».

### INC-42 · Norma de citación — cerrado
El trabajo de grado §6 declara «elaborado conforme a la norma NTC 1486 y con citación bajo la
norma **IEEE**», que es la que usa el documento (citas numéricas entre corchetes, nota de la
bibliografía) y la que dice el nombre del archivo (`…ICONTEC_IEEE.docx`).

---

## Residuos y puntos abiertos

**Residuos menores** (no afectan al código ni a un criterio de verificación):

1. **INC-24-r** — HU-17 y HU-18 no llevan el encabezado «Página 17/18 de 18». Se añaden
   insertando la fila de encabezado en el `.docx`.

**Puntos abiertos del guion (§12)** — son del guion, no conflictos entre documentos:
`PG-01` (título del producto), `PG-02` (nombre definitivo del guía), `PG-05` (verificar en
pruebas que el cambio de esquema de control entre niveles no confunde), `PG-06` (validar jugando
los valores del Nivel 1), `PG-07` (autorización de los personajes). `PG-03` y `PG-04` están
cerrados (redacción de `RF-16` y `RF-32`).

---

## Historial de revisiones

- **rev. 5 (30/08/2026)** — Se cerraron los 42 hallazgos editando los seis `.docx`. Cambios
  registrados en el control de cambios de OE1 §6, OE2 §4 y arquitectura §12.
- **rev. 4 (30/08/2026)** — Reconstrucción releyendo los seis documentos; 23 hallazgos abiertos,
  6 nuevos (INC-37 a INC-42).
- **rev. 3 (29/08/2026)** — 17 hallazgos abiertos (archivo no conservado en el repositorio).
- Cerrados antes de la rev. 3: INC-03 a INC-12, INC-14, INC-15 (identificadores retirados, no se
  reutilizan). Cerrados en la rev. 3 y verificados después: INC-02, INC-13, INC-17, INC-18,
  INC-19, INC-20, INC-23.
