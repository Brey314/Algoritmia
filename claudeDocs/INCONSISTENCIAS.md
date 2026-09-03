# Inconsistencias entre los documentos fuente

Registro de los conflictos detectados entre los seis `.docx` de `docs/`, con la corrección
aplicada a cada uno. Documento hermano de `SPEC.md`: aquí está **qué estaba mal en los
documentos y cómo quedó**; allí está **qué implementa el código**.

**Verificación vigente: 02/09/2026, rev. 7.** Los seis documentos se releyeron de principio a
fin y se editaron directamente en la rev. 5. **Los hallazgos INC-01 … INC-42 están cerrados**;
quedan **cuatro abiertos**: **INC-43** (el guion aún declara `PG-07` pendiente después de
obtenerse la autorización), **INC-44** (el guía pasa a llamarse **Algoritm**), **INC-45** (el guía
cambia de forma en cada nivel) e **INC-46** (la lista de tareas del Nivel 3 está descrita como dos
objetos distintos). Los tres primeros se cierran editando los `.docx` a mano, cosa que el código
nunca hace; **INC-46 se cierra dentro de `claudeDocs/`, pero exige una decisión**. Más dos
residuos menores, listados al final.

> **Los hallazgos 44 y 45 no nacen de un conflicto entre documentos, sino de una decisión del
> autor tomada el 02/09/2026.** Se registran aquí igual, porque el efecto es el mismo: los
> `.docx` radicados dicen una cosa y el proyecto hace otra, y eso hay que dejarlo escrito antes
> de que alguien lo descubra leyendo el guion.

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
| INC-43 | `PG-07` sigue «Abierto» tras aprobarse la autorización | Guion | **Abierto** |
| INC-44 | El guía se llama Algoritm; los documentos dicen «Chispa» | Guion, HU, OE1, OE2 | **Abierto** |
| INC-45 | El guía cambia de forma por nivel; el guion fija una sola | Guion | **Abierto** |
| INC-46 | La lista de tareas del Nivel 3: cuerda con nudos contra panel de casillas | Dirección de arte, Slice 3 | **Abierto** |

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

### INC-43 · `PG-07` desactualizado en el guion — abierto
El guion §12 declara `PG-07` **Abierto**: «El uso de los personajes de la Familia Anonaky depende
de una autorización aún no obtenida». La autorización **ya fue concedida por escrito**
(confirmado el 30/08/2026), de modo que la fila quedó desactualizada.

**Por qué el permiso hacía falta, que es lo que conviene no volver a perder.** Los personajes del
prototipo **se rediseñaron pero partieron de los diseños Anonaky**: son **obra derivada**.
Cambiar proporciones, vestuario y paleta no extingue el derecho del autor original, y generarlos
con una IA tampoco. De ahí que se solicitara la autorización en vez de darla por innecesaria.

**Corrección a aplicar en el `.docx`** —el código nunca edita `docs/`, así que la edición es
manual—: en la tabla del guion §12, marcar `PG-07` como **Cerrado (30/08/2026)** y sustituir la
acción requerida por la constancia de la autorización escrita.

Mientras el `.docx` no se edite, gana lo que dice aquí: `PG-07` está cerrado. El reconocimiento
expreso de los personajes en la pantalla de créditos sigue siendo **obligatorio** (CT-09,
RNF-23), y la constancia escrita se archiva con los anexos del trabajo de grado.

### INC-44 · El guía se llama **Algoritm**, no «Chispa» — abierto

**Decisión del 02/09/2026.** El nombre del guía era el punto abierto `PG-02` del guion §12
—«Nombre provisional»—, de modo que fijarlo no contradice nada: lo cierra. El guía se llama
**Algoritm**, y `PG-02` queda **cerrado**.

**Qué dicen hoy los documentos.** «Chispa» aparece 48 veces en el guion —25 de ellas como
acotación de diálogo `CHISPA:`—, 6 en las historias de usuario, 4 en OE2 y 1 en OE1. El trabajo
de grado y el documento de arquitectura no lo nombran.

**El nombre no sale de la nada.** Los documentos fuente por nivel ya barajaban otros: el del
Nivel 2 llamaba al guía **«Algorim»** y el del Nivel 3 lo alternaba entre «Bubo» y «Sabio»
(anotado en `tasks/Slice 2/plan.md` §Preguntas y en `tasks/Slice 3/plan.md` §Preguntas). El guion
unificó en «Chispa» y dejó abierto `PG-02`. **Algoritm** cierra ese punto y recupera la raíz que
ya estaba en el material del Nivel 2, ahora coherente con lo que el guía hace en los tres niveles:
descomponer un problema en pasos.

**Corrección a aplicar en los `.docx`** —manual, el código nunca edita `docs/`—:

| Documento | Dónde | Qué |
| --- | --- | --- |
| Guion | §1.1, tabla de personajes | «Chispa (guía)» → «Algoritm (guía)»; retirar «Nombre provisional — ver punto abierto PG-02» |
| Guion | 25 líneas de diálogo | `CHISPA:` → `ALGORITM:` |
| Guion | §12, tabla de puntos abiertos | `PG-02` → **Cerrado (02/09/2026): Algoritm** |
| HU · OE1 · OE2 | 11 menciones en total | «Chispa» → «Algoritm» |

> **Trampa que hay que evitar.** En el Nivel 1, *chispa* en minúscula es **el destello que
> sueltan las piedras** (guion §4.3.3, `RF-16`, `RF-18`), y no tiene nada que ver con el guía.
> Una sustitución global rompe el nivel del fuego. Se distingue por contexto: mayúscula inicial
> y sujeto animado = el guía; minúscula = el destello. Los archivos `fx_n1_chispa_*` **no se
> renombran**.

**Ya aplicado en `claudeDocs/`** (que sí edita el código): `SPEC.md` supuesto 5 y la lista de
puntos abiertos · `Direccion_de_Arte.md` §7, §7.6, §10.2, §13.3, §14.2 y §18 ·
`tasks/Sprites/`. Los prompts `A1` del Slice 1 y las menciones de los Slices 2 y 3 se actualizan
junto con el rediseño de INC-45, no antes: son el mismo trabajo.

**Nomenclatura:** `char_chispa_*` → `char_algoritm_n1_estrella.png`, `_n2_rueda`, `_n3_gota`.

---

### INC-45 · El guía cambia de forma en cada nivel — abierto

**Decisión del 02/09/2026.** Algoritm deja de tener una forma única: es **fuego en el Nivel 1,
rueda en el Nivel 2 y agua en el Nivel 3**. Su cuerpo es el material del descubrimiento que el
nivel acaba de nombrar.

**Qué dice hoy el guion.** §1.1 lo describe como «pequeña figura luminosa con forma de estrella,
del tamaño de una palma», y §4.4 repite «una silueta pequeña con forma de estrella». Leídas
juntas, fijan **una sola forma para los tres niveles**. Ahí está el conflicto.

**Pero el guion ya empujaba en esta dirección**, y conviene no perderlo: en §4.4 el guía aparece
«en el corazón de las llamas […] hecho de fuego **esta vez**», y se recoge en la fogata «como una
brasa que sigue viva». El propio texto ata su cuerpo al descubrimiento del nivel.

**Cómo se sostiene CN-03** —«un guía constante en los tres niveles»—: lo constante no es el
contorno del cuerpo, sino el **núcleo de identidad** que fija `Direccion_de_Arte.md` §7.6 —
tamaño, ojos, boca, ausencia de extremidades, núcleo claro de borde duro, contorno cálido
`#E2571F`, estela de puntos y silueta que siempre se cuenta hasta cinco—. Con eso, las tres
formas se leen como el mismo personaje, y CN-03 se cumple.

**Corrección a aplicar en el `.docx`** —manual—:

| Documento | Dónde | Qué |
| --- | --- | --- |
| Guion | §1.1, caracterización del guía | Sustituir «con forma de estrella» por la forma cambiante: estrella de fuego en el Nivel 1, rueda en el 2 y gota de agua en el 3, con los rasgos invariables |
| Guion | §5 y §7 (escenas puente) | Añadir la acotación de la muta: el guía cruza la escena con la forma del nivel que termina y aparece con la del que empieza |
| Guion | §4.4 | **No se toca.** En el Nivel 1 el guía **es** una estrella de fuego; la acotación es correcta tal como está |

**Impacto en la producción.** El asset `A1` del Slice 1 deja de ser uno y pasa a ser tres, y los
Slices 2 y 3 dejan de poder reutilizarlo «tal cual» como declaran hoy sus secciones de assets. El
trabajo está planeado en `claudeDocs/tasks/Sprites/plan.md`, tarea `S15`.

**Riesgo detectado, y contenido.** El cuerpo de la rueda usa `#C79A5E`, el acento del Nivel 2,
que es la señal de «esto es interactivo». No infringe §4.2 —la prohibición recae sobre el
decorado, y el guía no lo es—, pero puede confundir. Las tres condiciones que lo separan de un
prop están en `Direccion_de_Arte.md` §7.6 y son obligatorias: no se posa nunca, tiene cara, y el
pulso de la pista es suyo y de nada más.

---

### INC-46 · La lista de tareas del Nivel 3 está especificada dos veces, y distinto — abierto

**El conflicto.** `RF-36` obliga a mostrar de forma permanente la descomposición del objetivo en
cuatro tareas y a marcar cada una al cumplirse. Ningún `.docx` dice **cómo se ve** esa lista: el
guion §8.1 solo dice «en pantalla aparece la lista de tareas». Los dos documentos del proyecto que
sí lo describen no coinciden:

| Documento | Qué dice | Tarea cumplida |
| --- | --- | --- |
| `Direccion_de_Arte.md` §10.2 | **Cuerda con nudos.** Cuerda `#C4A882`, un nudo por tarea | Nudo cerrado `#5FA842` **más** marca de forma |
| `tasks/Slice 3/plan.md` `C5` | **Panel vertical de marfil** `#F7EFE2` con borde de cuero cosido y cuatro filas, cada una con una casilla cuadrada | Casilla rellena `#5FA842`, borde engrosado y marca de verificación |

No son dos redacciones del mismo objeto: son dos objetos. Se detectó al enumerar los props del
Nivel 3 uno a uno (`tasks/Sprites/plan.md` §4.3).

**Qué manda.** La precedencia del proyecto es `SPEC.md` → `Direccion_de_Arte.md` → planes de
slice, así que **por regla gana la cuerda con nudos y lo que hay que corregir es `C5`**. Se
registra aquí en vez de aplicarlo de una porque los dos lados tienen argumento:

- **A favor de la cuerda (§10.2):** es la opción diegética que pide §10.1 —la interfaz imita
  materiales del mundo—, y el nudo es un objeto prehistórico creíble. Además, la balsa del nivel
  ya usa sogas: la lista y el reto hablarían el mismo idioma.
- **A favor del panel de casillas (`C5`):** el Nivel 3 es el escenario más claro y en vista
  superior, el caso más expuesto del juego para el contraste (RNF-20), y el resto de su interfaz
  —inventario, panel de ensamblaje— ya son marcos de marfil. Una cuerda suelta sobre el follaje
  es más difícil de leer que una placa, y `C5` la resuelve con casilla llena **más** borde
  engrosado **más** marca, que cumple RNF-19 igual de bien.

**Cualquiera de las dos salidas cierra el hallazgo**, y las dos son de una sola edición:

1. **Corregir `C5`** para que la lista sea una cuerda con cuatro nudos, manteniendo el inventario
   y el panel de ensamblaje como están. Es lo que dicta la precedencia.
2. **Corregir `Direccion_de_Arte.md` §10.2** para que la lista sea el panel de casillas, dejando
   constancia de por qué se abandona la vía diegética en este componente.

**Lo que no cambia, se decida lo que se decida:** la marca de tarea cumplida lleva **color y
forma**, nunca solo color (RNF-19), y la lista **no muestra cifras** (CP-03, RF-17).

**Bloquea:** la generación del asset `C5` y la tarea `S11b` de `tasks/Sprites/`.

---

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
`PG-01` (título del producto), `PG-05` (verificar en pruebas que el cambio de esquema de control
entre niveles no confunde), `PG-06` (validar jugando los valores del Nivel 1). **`PG-02` (nombre
del guía) está cerrado desde el 02/09/2026: se llama Algoritm** — el guion aún no lo refleja, ver
INC-44. `PG-03` y `PG-04` están cerrados (redacción de `RF-16` y `RF-32`), y
`PG-07` (autorización de los personajes) está **cerrado desde el 30/08/2026**: la autorización se
concedió por escrito. El guion aún no lo refleja — ver INC-43.

---

## Historial de revisiones

- **rev. 7 (02/09/2026)** — Decisión del autor sobre el guía: se llama **Algoritm** (`PG-02`
  cerrado) y **cambia de forma en cada nivel** —fuego, rueda, agua—. Se abren **INC-44** e
  **INC-45**; ambos se cierran editando los `.docx` a mano. `claudeDocs/` ya está alineado.
  Al enumerar los props del Nivel 2 y del Nivel 3 uno a uno se detectó además **INC-46**: la
  lista de tareas del Nivel 3 está descrita como dos objetos distintos en la dirección de arte y
  en el Slice 3. Queda abierto, a la espera de decisión.
- **rev. 6 (30/08/2026)** — Confirmada la autorización escrita de los personajes de la Familia
  Anonaky: `PG-07` se cierra y se abre **INC-43**, porque el guion §12 todavía lo declara
  pendiente.
- **rev. 5 (30/08/2026)** — Se cerraron los 42 hallazgos editando los seis `.docx`. Cambios
  registrados en el control de cambios de OE1 §6, OE2 §4 y arquitectura §12.
- **rev. 4 (30/08/2026)** — Reconstrucción releyendo los seis documentos; 23 hallazgos abiertos,
  6 nuevos (INC-37 a INC-42).
- **rev. 3 (29/08/2026)** — 17 hallazgos abiertos (archivo no conservado en el repositorio).
- Cerrados antes de la rev. 3: INC-03 a INC-12, INC-14, INC-15 (identificadores retirados, no se
  reutilizan). Cerrados en la rev. 3 y verificados después: INC-02, INC-13, INC-17, INC-18,
  INC-19, INC-20, INC-23.
