# Inconsistencias entre los documentos fuente

Registro de los conflictos detectados entre los `.docx` de `docs/`, con la corrección
aplicada a cada uno. Documento hermano de `SPEC.md`: aquí está **qué estaba mal en los
documentos y cómo quedó**; allí está **qué implementa el código**.

**Los documentos se refundieron el 14/09/2026: de seis pasaron a cuatro.** OE1 es ahora
`Solución OE1_Requerimientos.docx`, y `Solucion_OE2_Diseno_final.docx` absorbe en un solo archivo
el guion (§1), los casos de uso (§2), las historias de usuario (§2 bis), las matrices de
trazabilidad (§3) y la arquitectura (§4). El **trabajo de grado ya no está en `docs/`**; se
recupera con `git show HEAD:"docs/Trabajo_de_Grado_2026_ICONTEC_IEEE (2).docx"`. Las citas por
sección de este documento siguen valiendo con una traducción mecánica: «guion §N» → `Solucion_OE2`
§1.N, «OE2 §4» → `Solucion_OE2` §5 (control de cambios).

**Verificación vigente: 15/09/2026, rev. 10.** Los documentos refundidos se releyeron contra los
hallazgos que seguían abiertos. **Los hallazgos INC-01 … INC-45 están cerrados**: la refundición
del 14/09/2026 aplicó en los `.docx` las tres correcciones que esperaban edición manual —`PG-07`
cerrado (INC-43), el guía renombrado a **Algoritm** (INC-44) y su forma cambiante por nivel
(INC-45)—. Quedan **cinco abiertos**: **INC-46** (la lista de tareas del Nivel 3 está descrita
como dos objetos distintos; se cierra dentro de `claudeDocs/`, pero exige una decisión),
**INC-47** (la mecánica del Nivel 1 implementada mide fuerza, y los documentos refundidos
**mantienen** el deslizante de posición), **INC-48** (la refundición reintrodujo el capítulo de
arquitectura **anterior** a su alineación), **INC-49** (el menú de pausa implementado es el del
mockup 6 y HU-17 conserva los rótulos viejos) e **INC-50** (el rodado del Nivel 2 se ve solo en la
narrativa, no al pulsar «Empujar»). INC-47, INC-49 e INC-50 son el mismo caso: el código se
adelantó al `.docx` por decisión de Santiago, y lo que falta es editar el documento. Más tres
residuos menores, listados al final.

> **Los hallazgos 44 y 45 no nacieron de un conflicto entre documentos, sino de una decisión del
> autor tomada el 02/09/2026.** Se registraron aquí igual, porque el efecto era el mismo: los
> `.docx` radicados decían una cosa y el proyecto hacía otra. La refundición del 14/09/2026 los
> alineó.

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

| # | Documento (tras la refundición del 14/09/2026) | Qué gobierna |
|---|---|---|
| 1 | Trabajo de grado — **fuera de `docs/`**, en `git show HEAD:…` | Objetivos, KPI, alcance, marco jurídico, metodología |
| 2 | `Solución OE1_Requerimientos.docx` | Lineamientos CP/CT/CN, RF-01..RF-47, RNF-01..RNF-23 |
| 3 | `Solucion_OE2_Diseno_final.docx` §1 (guion) | Narrativa, mecánicas, parámetros y textos exactos |
| 4 | `Solucion_OE2_Diseno_final.docx` §2–§3 | CU-01..CU-12, HU-01..HU-18, matrices |
| 5 | `Historias_de_Usuario_HU01_HU18_v2 (1).docx` | HU detalladas: flujos, criterios, reglas de negocio |
| 6 | `arquitectura_videojuego_v2 (2).docx` · `Solucion_OE2` §4 | Decisiones técnicas de implementación |

Las filas 3 y 4 viven en el **mismo archivo**, así que un conflicto entre ellas ya no es un
conflicto entre documentos: se corrige editándolo (ver «contradicciones internas», abajo).

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
| INC-43 | `PG-07` sigue «Abierto» tras aprobarse la autorización | Guion | **Cerrado** (14/09/2026) |
| INC-44 | El guía se llama Algoritm; los documentos decían «Chispa» | Guion, HU, OE1, OE2 | **Cerrado** (14/09/2026, residuo menor: una mención en la tabla E5) |
| INC-45 | El guía cambia de forma por nivel; el guion fijaba una sola | Guion | **Cerrado** (14/09/2026) |
| INC-46 | La lista de tareas del Nivel 3: cuerda con nudos contra panel de casillas | Dirección de arte, Slice 3 | **Abierto** |
| INC-47 | La mecánica del Nivel 1 reúne los materiales en un círculo y luego mide fuerza y cercanía de las piedras en dos deslizantes; el guion y RF-15 fijan un deslizante de posición | Guion, OE1, HU | **Abierto** |
| INC-48 | Dos arquitecturas contradictorias en `docs/`: la refundición trajo de vuelta el capítulo anterior a la alineación | OE2 §4, Arquitectura | **Abierto** (decisión tomada: gana la alineada) |
| INC-49 | El menú de pausa implementado es el del mockup 6 (Reanudar · Reiniciar · Volver al menú de niveles); HU-17 dice «Continuar / Reiniciar nivel / Volver al menú principal» | HU, OE2 §3 | **Abierto** |
| INC-50 | «Empujar» en el bosque no anima el rodado: sale a la escena 2.2, que lo cuenta; RF-26 y HU-08 lo describen dentro de la mecánica | OE1, HU, CU | **Abierto** |
| INC-51 | El cierre del Nivel 3 nunca ofrece omitir; HU-14 FA-01 lo pide a quien repite el nivel | HU | **Abierto** (decisión tomada: se acepta) |

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

### INC-43 · `PG-07` desactualizado en el guion — cerrado (14/09/2026)
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

**Cerrado el 14/09/2026.** `Solucion_OE2_Diseno_final.docx` §1.2 declara `PG-07` como **«Cerrado
(30/08/2026)»**, y su control de cambios (§5) registra la entrada del 14/09/2026 «Cierre PG-02 y
PG-07». El reconocimiento expreso de los personajes en la pantalla de créditos sigue siendo
**obligatorio** (CT-09, RNF-23), y la constancia escrita se archiva con los anexos del trabajo de
grado.

### INC-44 · El guía se llama **Algoritm**, no «Chispa» — cerrado (14/09/2026, residuo menor)

**Decisión del 02/09/2026.** El nombre del guía era el punto abierto `PG-02` del guion §12
—«Nombre provisional»—, de modo que fijarlo no contradice nada: lo cierra. El guía se llama
**Algoritm**, y `PG-02` queda **cerrado**.

**Qué decían los documentos antes de la refundición.** «Chispa» aparecía 48 veces en el guion
—25 de ellas como acotación de diálogo `CHISPA:`—, 6 en las historias de usuario, 4 en OE2 y 1 en
OE1. El trabajo de grado y el documento de arquitectura no lo nombraban.

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

**Cerrado el 14/09/2026.** La refundición aplicó la sustitución en los `.docx`: en
`Solucion_OE2_Diseno_final` la tabla de personajes (§1.1.1) dice «Algoritm(guía)», las
acotaciones de diálogo son `ALGORITM:` y `PG-02` (§1.2) figura como **«Cerrado (Algorítm)
(02/09/2026)»**. En `Solución OE1_Requerimientos.docx`, en las historias de usuario y en la
arquitectura no queda ninguna mención del guía como «Chispa»: las apariciones de *chispa* que
sobreviven son todas el **destello de las piedras** en minúscula, que es lo correcto.

**Residuo menor — `INC-44-r`, una sola mención.** `Solucion_OE2_Diseno_final` §1.4.3, tabla de
estados de la mecánica del Nivel 1, estado **E5**: «Pista del guía. **Chispa** formula una
pregunta orientadora sin resolver la tarea». Debe decir «Algoritm». No afecta al código ni a
ningún criterio de verificación.

**Segundo residuo — `INC-44-r2`, contradicción interna del mismo archivo.** La tabla de
personajes (§1.1.1) sigue cerrando la descripción con «Nombre provisional — ver punto abierto
PG-02», mientras que §1.2 declara `PG-02` cerrado. Sobra la cláusula.

---

### INC-45 · El guía cambia de forma en cada nivel — cerrado (14/09/2026)

**Decisión del 02/09/2026.** Algoritm deja de tener una forma única: es **fuego en el Nivel 1,
rueda en el Nivel 2 y agua en el Nivel 3**. Su cuerpo es el material del descubrimiento que el
nivel acaba de nombrar.

**Qué decía el guion antes de la refundición.** §1.1 lo describía como «pequeña figura luminosa
con forma de estrella, del tamaño de una palma», y §4.4 repetía «una silueta pequeña con forma de
estrella». Leídas juntas, fijaban **una sola forma para los tres niveles**. Ahí estaba el
conflicto.

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

**Cerrado el 14/09/2026.** `Solucion_OE2_Diseno_final` §1.1.1 describe ya la forma cambiante
—«estrella de fuego en el Nivel 1, rueda en el 2 y gota de agua en el 3, con los rasgos
invariables»— y §1.5 (escena puente I) trae la acotación de la muta: «el guía (Algoritm) cruza la
escena con la forma del nivel que termina (fuego) y aparece con la del que empieza (rueda)». El
control de cambios (§5) lo registra el 14/09/2026: «Ajuste a definición artistica diferente de
Algoritm (Una forma por nivel)».

**Impacto en la producción, que sigue pendiente.** El asset `A1` del Slice 1 deja de ser uno y
pasa a ser tres, y los Slices 2 y 3 dejan de poder reutilizarlo «tal cual» como declaran hoy sus
secciones de assets. El trabajo está planeado en `claudeDocs/tasks/Sprites/plan.md`, tarea `S15`.
Cerrar el hallazgo en los documentos **no** produce los sprites.

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

### INC-47 · La mecánica del Nivel 1 es de fuerza y disposición, no de posición — abierto

**Decisión de Santiago, 12/09/2026** (tablero del Slice 1, Fase 5, T21–T23): el deslizante del
panel de encendido **mide la fuerza del golpe** en diez muescas —efectivas la siete y la ocho,
`N1_Config.asset`— y no la distancia en tres; las hojas, el sílex y el pedernal aparecen regados
por la cueva y **se arrastran** hasta el punto del fuego. Un golpe solo es efectivo con fuerza
correcta y las dos piedras cerca; «Soplar» se habilita al converger, pero el fuego solo nace si las
hojas están amontonadas y cerca. Sin montón, el soplo se describe y no se penaliza (CP-02).

**Qué dicen hoy los documentos.** Guion §4.3.1–§4.3.5 (deslizante de posición: Lejos / Cerca / Muy
cerca; mensajes por distancia; E1 «deslizante en Lejos»), OE1 RF-15 («control deslizante de
posición… tres distancias») y RF-16, HU-06.

**Ampliación del 15/09/2026 (Fase 6, T25–T27).** La mecánica queda en **dos fases**: (1) reunir
todas las hojas y las dos piedras en el círculo del centro de la pantalla —«Pista» lo dibuja; su
radio llega a la mitad del botón de abajo— sin ninguna otra interfaz; (2) al reunirlo todo la
cámara se acerca al doble, las hojas se acomodan solas en fogata y aparecen **dos deslizantes**:
fuerza (0–10, efectiva 7–8) y **cercanía de las piedras** (0–10: de separadas la longitud de una
hoja a una encima de otra; certero cuando se rozan encimadas unos cinco píxeles). Las piedras ya no
se arrastran en el encendido y «Soplar» ya no puede fallar por hojas regadas. Sobre «Soplar» queda
solo el candado, sin el rótulo «Aún no». Todo en `N1_Config.asset`, `N1_Guia.asset` (paso
`Reunir`) y `N1_Mensajes.asset` (piedras separadas / encimadas).

**Corrección pendiente.** Guion §4.3 (elementos, parámetros, comportamiento, mensajes, flujo y
pista) y §4.4 («cambiaste de lugar» → «cambiaste la fuerza o la cercanía»), OE1 RF-15/RF-16, HU-06.
El registro con historial (HU-06) pasa a una tablilla con **el último mensaje**; el historial
completo sigue en `FireFeedbackLog.Entries` para el informe docente. En código ya está aplicado: los
nombres de las pruebas conservan RF-15/RF-16 porque el requisito (hipótesis → experimento →
resultado) es el mismo.

### INC-48 · La refundición reintrodujo la arquitectura anterior a su alineación — abierto

**El conflicto.** `docs/` tiene hoy **dos capítulos de arquitectura que se contradicen**:

- `arquitectura_videojuego_v2 (2).docx` — la versión **alineada**. Su §12 enumera los catorce
  cambios aplicados, incluidos los de INC-34, INC-38, INC-39 e INC-40.
- `Solucion_OE2_Diseno_final.docx` **§4** — la versión **anterior** a esa alineación, tal cual
  estaba antes de la rev. 5.

Y lo que lo vuelve un hallazgo y no una redundancia: **el capítulo obsoleto está en el documento
de mayor precedencia**. Quien siga la tabla de precedencia encuentra primero la arquitectura mala.

**Qué reintroduce §4, punto por punto:**

| §4 del documento refundido | Contra qué choca |
| --- | --- |
| «el progreso se persiste en `Application.persistentDataPath`» | RNF-07, RNF-11 · INC-34 · supuesto 1 de `SPEC.md` |
| «las cinemáticas se empaquetan en StreamingAssets» + `CinematicsPlayer` | RF-05 (ilustraciones estáticas y diálogos), RNF-04, RNF-06 |
| Ocho estados fijos: `Cinematica_Intro`, `Level_01`, `Cinematica_01`… | El Nivel 2 son **tres** escenas jugables encadenadas (RF-22, RF-27, RF-30) |
| `EntityManager`: «jugador, **enemigos** y coleccionables» | No hay enemigos en este juego |
| `EventBus` global para todo | Decisión acotada en `SPEC.md` §Comunicación |
| Los módulos no incluyen `Game.UI` ni `Game.Audio` | INC-40 |
| Ninguna decisión trazada a un RF/RNF | CT-10 |

**Decisión (14/09/2026): gana `arquitectura_videojuego_v2 (2).docx`.** Es la versión alineada,
es la que `SPEC.md` §Arquitectura adopta y es la que el código implementa. **Ni `SPEC.md` ni el
código cambian por este hallazgo**: ya siguen la buena.

**Corrección a aplicar en el `.docx`** —manual, el código nunca edita `docs/`—: sustituir §4 de
`Solucion_OE2_Diseno_final.docx` por el contenido de `arquitectura_videojuego_v2 (2).docx`, o
reducir §4 a una remisión a ese documento. Mientras no se haga, **gana lo que dice aquí**.

**Por qué pasó, que es lo que conviene no repetir.** La refundición consolidó seis documentos en
cuatro copiando capítulos enteros. Los capítulos §1 a §3 se tomaron de las versiones vigentes —su
control de cambios del 30/08/2026 está ahí, e INC-01, INC-33, INC-35 e INC-37 aparecen
aplicados—, pero §4 se tomó de un original sin alinear. **Al consolidar, verificar capítulo por
capítulo contra el control de cambios, no contra el nombre del archivo.**

---

### INC-49 · El menú de pausa de HU-17 no es el del mockup 6 — abierto

**Decisión de Santiago, 15/09/2026** (Slice 2, W17): el menú de pausa es el del **mockup 6** —
«Reanudar», «Reiniciar» y «Volver al menú de niveles»—, el mismo prefab en las cuatro escenas
jugables. «Reiniciar» repite **la fase activa** (INC-25) y «Volver al menú de niveles» sale a la
selección de niveles, no al inicio.

**Qué dicen hoy los documentos.** HU-17 (`Solucion_OE2_Diseno_final` §3 y
`Historias_de_Usuario_HU01_HU18_v2`): «El menú de pausa ofrece Continuar, Reiniciar nivel y Volver
al menú principal». RF-07 no fija los rótulos ni el destino.

**Corrección pendiente.** HU-17, criterio de aceptación y flujos FA-01..FA-05: «Continuar» →
«Reanudar», «Reiniciar nivel» → «Reiniciar» (la fase activa), «Volver al menú principal» → «Volver
al menú de niveles». En código ya está aplicado (`PauseMenuController`, `MenuPausa.prefab`); las
pruebas conservan HU-17 en el nombre porque la historia —detener, reanudar sin perder nada,
reiniciar con confirmación— es la misma.

### INC-50 · El rodado del Nivel 2 solo se ve en la narrativa — abierto

**Decisión de Santiago, 15/09/2026** (Slice 2, W19): al pulsar «Empujar» la mecánica del bosque
**no anima el rodado**; confirma la fase y sale a la escena 2.2, que es la que lo cuenta con
`RollMotion`. Además la fila de troncos del bosque va **pegada** (centros a 0,889 del tamaño del
tronco), igual que los props de esa escena, para que el corte entre las dos no se note.

**Qué dicen hoy los documentos.** RF-26 y HU-08: «al accionar Empujar se reproduce la
demostración del rodado»; CU-06 flujo principal, paso del empuje; guion §1.6.1.2 describe el
rodado dentro de la mecánica.

**Corrección pendiente.** RF-26, HU-08 y CU-06: «Empujar» cierra la fase y la demostración del
rodado es la escena narrativa 2.2 (guion §1.6.1.3). El requisito de fondo —el estudiante ve rodar
la caja sobre los troncos redondos— se sigue cumpliendo; cambia dónde.

### INC-51 · El cierre del Nivel 3 no se puede omitir al repetirlo — abierto

**Decisión de Santiago, 23/09/2026**: se acepta. En el Nivel 3 el cierre reflexivo
(`N3_Escena33_Cruce`) y la escena final (`N3_EscenaFinal`) **se leen enteros siempre**, también
cuando el estudiante repite el nivel.

**Por qué no hay otra salida sin ampliar lo persistido.** No existe registro de escenas vistas
(RNF-09, INC-28): `NarrativeVisitPolicy` deriva «ya visto» del progreso. Para un cierre reflexivo
la señal es que el nivel siguiente ya esté desbloqueado (`ReachedLevel > sequence.Level`), porque
se llega a él justo después de confirmar la última fase. El Nivel 3 no tiene nivel siguiente
—`LevelId` termina en `River` y `LevelUnlockPolicy.UnlockAfterCompleting` no alcanza más allá—,
así que la primera vuelta y las siguientes dejan **el mismo perfil** en disco y no se pueden
distinguir. La alternativa sin tocar el disco —anotar en memoria de sesión si el nivel ya estaba
completo al entrar a jugarlo— cumpliría FA-01 solo dentro de una misma sesión, no tras cerrar y
reabrir el juego.

**Qué dicen hoy los documentos.** HU-14 FA-01: «El estudiante ya había completado este nivel antes
y vuelve a jugarlo → la escena de cierre muestra el botón de omitir». Vale para los Niveles 1 y 2,
no para el 3.

**Corrección pendiente.** HU-14 FA-01: añadir que en el último nivel la escena de cierre y la
escena final se reproducen siempre enteras. RF-06 no cambia: habla de «una escena ya vista» y no
obliga a reconocer todas.

## Residuos y puntos abiertos

**Residuos menores** (no afectan al código ni a un criterio de verificación):

1. **INC-24-r** — HU-17 y HU-18 no llevan el encabezado «Página 17/18 de 18». Se añaden
   insertando la fila de encabezado en el `.docx`.
2. **INC-44-r** — `Solucion_OE2_Diseno_final` §1.4.3, estado **E5** de la tabla de la mecánica del
   Nivel 1: «Chispa formula una pregunta orientadora» → «Algoritm».
3. **INC-44-r2** — `Solucion_OE2_Diseno_final` §1.1.1, tabla de personajes: sobra «Nombre
   provisional — ver punto abierto PG-02», porque §1.2 ya declara `PG-02` cerrado.

**Puntos abiertos del guion** —ahora `Solucion_OE2_Diseno_final` §1.2— son del guion, no
conflictos entre documentos. Siguen **abiertos**: `PG-01` (título del producto), `PG-05`
(verificar en pruebas que el cambio de esquema de control entre niveles no confunde) y `PG-06`
(validar jugando los valores del Nivel 1). Están **cerrados y así consta ya en el `.docx`**:
`PG-02` (el guía se llama **Algoritm**, 02/09/2026 — INC-44), `PG-03` y `PG-04` (redacción de
`RF-16` y `RF-32`, 24/08/2026) y `PG-07` (autorización escrita de los personajes, 30/08/2026 —
INC-43).

---

## Historial de revisiones

- **rev. 11 (23/09/2026)** — Se abre **INC-51**: el cierre reflexivo y la escena final del Nivel 3
  no ofrecen omitir ni al repetir el nivel, porque sin nivel siguiente la primera vuelta y las
  demás dejan el mismo perfil. Decisión de Santiago el mismo día: se acepta; se corrige HU-14 FA-01.
- **rev. 10 (15/09/2026, segunda entrada)** — Fase 6 del Slice 1 y W19 del Slice 2. **INC-47** se
  amplía (dos fases: reunir en el círculo y encender con dos deslizantes, fuerza y cercanía; sin
  rótulo «Aún no»). Se abre **INC-50**: «Empujar» no anima el rodado en la mecánica del bosque; lo
  cuenta la escena 2.2, y la fila de troncos va pegada como en ella.
- **rev. 9 (15/09/2026)** — Cierre de la Fase 5 del Slice 2. Se abre **INC-49**: el menú de
  pausa sigue el mockup 6 (Reanudar · Reiniciar · Volver al menú de niveles) y HU-17 todavía dice
  «Continuar, Reiniciar nivel y Volver al menú principal». Decisión de Santiago el mismo día:
  gana el mockup; se corrige HU-17.
- **rev. 8 (14/09/2026, segunda entrada)** — Auditoría de `SPEC.md` contra los documentos
  refundidos. Se abre **INC-48**: `Solucion_OE2_Diseno_final` §4 es el capítulo de arquitectura
  **anterior** a la alineación, y está en el documento de mayor precedencia. Decisión tomada el
  mismo día: gana `arquitectura_videojuego_v2 (2).docx`. Los capítulos §1 a §3 del documento
  refundido sí están al día.
- **rev. 8 (14/09/2026)** — **Refundición de los documentos fuente**: de seis `.docx` a cuatro
  (`Solución OE1_Requerimientos.docx` y `Solucion_OE2_Diseno_final.docx`, que absorbe guion, casos
  de uso, historias, matrices y arquitectura; el trabajo de grado sale de `docs/`). Verificados
  contra los documentos nuevos, se cierran **INC-43**, **INC-44** e **INC-45**. Quedan abiertos
  **INC-46** e **INC-47**; se abren los residuos menores `INC-44-r` y `INC-44-r2`. **INC-47 no se
  cerró con la refundición**: los documentos nuevos mantienen el deslizante de posición
  (`Solucion_OE2` §1.4.3 «Lejos / Cerca / Muy cerca», OE1 `RF-16` «en función de la distancia
  seleccionada en el control de posición», HU-06 «control deslizante de posición»), mientras el
  código implementa el deslizante de fuerza.
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
