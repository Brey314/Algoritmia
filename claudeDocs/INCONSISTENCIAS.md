# Inconsistencias entre los documentos fuente

Registro de los conflictos detectados entre los seis `.docx` de `docs/`, con la corrección
propuesta para cada uno. Es el documento hermano de `SPEC.md`: aquí está **qué está mal en los
documentos**; allí está **qué implementa el código mientras tanto**.

**Verificación vigente: 30/08/2026, rev. 4.** Veintitrés hallazgos abiertos.

> **Nota sobre esta revisión.** El archivo de la rev. 3 no estaba en el repositorio: esta
> revisión se reconstruyó leyendo los seis documentos de nuevo, de principio a fin. Se conservan
> los identificadores que `SPEC.md` ya citaba, para que sus referencias cruzadas sigan siendo
> válidas. Respecto de la rev. 3 hay tres hallazgos que se **estrecharon** porque el documento
> ya fue corregido en parte (INC-01, INC-21, INC-24), uno cuyo enunciado **no se sostuvo** al
> recalcularlo (INC-36) y **seis nuevos** (INC-37 a INC-42).

---

## Cómo se resuelve un conflicto

**Orden de precedencia.** Cuando dos documentos se contradicen gana el de mayor prioridad y se
corrige el otro:

| # | Documento | Qué gobierna |
|---|---|---|
| 1 | `Trabajo_de_Grado_2026_ICONTEC_IEEE.docx` | Objetivos, KPI, alcance, marco jurídico, metodología |
| 2 | `OE1_Requerimientos (3).docx` | Lineamientos CP/CT/CN, RF-01..RF-47, RNF-01..RNF-23 |
| 3 | `Guion_Completo_Videojuego.docx` | Narrativa, mecánicas, parámetros y textos exactos |
| 4 | `OE2_historias_completas.docx` | CU-01..CU-12, HU-01..HU-18, matrices |
| 5 | `Historias_de_Usuario_HU01_HU18_v2.docx` | HU detalladas: flujos, criterios, reglas de negocio |
| 6 | `arquitectura_videojuego_v2.docx` | Decisiones técnicas de implementación |

**La precedencia no resuelve las contradicciones internas a un mismo documento.** Esas se
corrigen editándolo, y aquí va la redacción propuesta.

**Severidad.** *Alta* = hoy el documento ordena algo que viola un invariante pedagógico o deja un
requerimiento sin describir. *Media* = afecta al código o a una decisión de diseño. *Baja* = solo
afecta a la coherencia documental.

---

## Resumen de hallazgos abiertos

| ID | Hallazgo | Documentos | Severidad |
|---|---|---|---|
| INC-01 | «Teclas de dirección» residual en tres documentos | Guion, HU, Arquitectura | Media |
| INC-16 | La arquitectura no cita `RNF-18` en ninguna parte | Arquitectura | Baja |
| INC-21 | CN-04 (coherencia visual) sigue trazando a `RNF-22` | OE2 | Baja |
| INC-22 | La introducción da por concedidos los personajes | Trabajo de grado | Baja |
| INC-24 | Paginación de HU y encabezado de arquitectura desfasados | HU, Arquitectura | Baja |
| INC-25 | HU-17 lleva el flujo y los criterios de otras historias | HU | **Alta** |
| INC-26 | HU-14 muestra cifras al estudiante | HU | **Alta** |
| INC-27 | `RNF-09` no autoriza el progreso que `RF-04` obliga a guardar | OE1 | Media |
| INC-28 | La omisión de escenas está limitada en OE1 y libre en el resto | OE1, OE2, HU | Media |
| INC-29 | HU-10 nombra indicadores fuera de la lista cerrada | HU | Media |
| INC-30 | Tres fases de ensamblaje contra cuatro tareas | OE1, Guion, HU | Media |
| INC-31 | Actores y referencias cruzadas erróneas | HU | Baja |
| INC-32 | `RF-19` recondiciona «Soplar» tras la convergencia | OE1 | Media |
| INC-33 | Semántica de los bloques del laberinto sin definir | OE1, Guion | **Alta** (diseño) |
| INC-34 | El fallback de persistencia no está en el criterio de `RNF-11` | Arquitectura | Media |
| INC-35 | CU-11 presenta los indicadores solo por nivel | OE2 | Media |
| INC-36 | Datos internos inconsistentes en el trabajo de grado | Trabajo de grado | Baja |
| INC-37 | `RF-44` y `RF-46` no tienen historia de usuario | OE2, HU | Media |
| INC-38 | La arquitectura cita `TRAZABILIDAD.md`, que no existe | Arquitectura | Baja |
| INC-39 | Ninguna transición lleva a `Credits` al terminar el Nivel 3 | Arquitectura | Media |
| INC-40 | UI, Audio y Datos no tienen assembly en la lista de §9 | Arquitectura | Media |
| INC-41 | HU-02 generaliza la lista de tareas a todos los niveles | HU | Media |
| INC-42 | Norma de citación declarada distinta de la usada | Trabajo de grado | Baja |

---

## INC-01 · «Teclas de dirección» residual en tres documentos

**Severidad:** Media. **Estado:** abierto, estrechado respecto de la rev. 3.

`CT-06` y `RNF-02` limitan la entrada a **clic y clic sostenido, sin excepciones**. El movimiento
del personaje en el Nivel 3 es el único punto donde los documentos se desvían.

**Ya corregido.** `RF-35` dice hoy: «desplazar al personaje en dos dimensiones mediante los
**botones de dirección mostradas en los costados derechos y izquierdos de la pantalla**». La
palabra «teclas» ya no aparece en OE1. El guion §2.1 («Botones en pantalla de cambio de
dirección») y CU-09 («desplaza al personaje con botones de dirección en pantalla») coinciden.

**Lo que queda mal:**

| Documento | Texto vigente | Corrección |
|---|---|---|
| Guion §8.2 | «El jugador desplaza a mamá **con las teclas de dirección**» | «con los botones de dirección en pantalla», para concordar con su propio §2.1 |
| HU-11, *Datos de entrada* | Campo «**Teclas de dirección** · Tipo: **Teclado (dirección)**» | «Botones de dirección · Tipo: Acción (clic)» |
| HU-11, *Reglas de negocio* | «es la **única excepción al control de solo clic** del videojuego, justificada por la necesidad de exploración libre» | Suprimir la frase entera. Un botón en pantalla se acciona con clic: **no hay excepción que justificar**, y declararla debilita `RNF-02` sin necesidad |
| Arquitectura §1 | «Entrada limitada a clic y clic sostenido; **teclas de dirección solo en Nivel 3**» | «Entrada limitada a clic y clic sostenido, incluidos los botones de dirección en pantalla del Nivel 3» |

HU-11 se contradice a sí misma: su flujo básico paso 2 ya dice «botones de dirección en
pantalla». Solo la tabla de datos de entrada y la regla de negocio dicen teclado.

**Efecto en el código:** ninguno — `SPEC.md` ya implementa botones en pantalla. El riesgo es que
la prueba de inspección del mapa de controles (criterio de verificación de `RNF-02`) se redacte
«con una salvedad» y deje de ser verificable.

---

## INC-16 · La arquitectura no cita `RNF-18`

**Severidad:** Baja. **Estado:** abierto.

El control de cambios de OE1 del 24/08/2026 añadió `RNF-18` («Los diálogos, las tareas y los
parámetros de configuración de nivel deben residir en archivos de datos externos al código
fuente»), con criterio de verificación propio: *modificar un parámetro de nivel y un texto de
diálogo sin recompilar*.

La arquitectura traza la parametrización de contenidos **solo a `CT-05`** — en §1 (tabla de
restricciones), §6 (fila «Datos · ScriptableObjects») y §11 (tabla de trazabilidad). `RNF-18` no
aparece ni una vez en el documento.

**Corrección:** añadir `RNF-18` junto a `CT-05` en esas tres tablas.

**Por qué importa:** un criterio es una regla de decisión; un RNF es una condición verificable. Al
citar solo `CT-05` la arquitectura deja fuera la prueba que hace exigible la parametrización.

---

## INC-21 · CN-04 sigue trazando a `RNF-22`

**Severidad:** Baja. **Estado:** abierto, estrechado respecto de la rev. 3.

OE2 §3.4 traza `CN-04` (coherencia visual: misma paleta, mismo estilo, misma tipografía) a
«`RNF-20`, `RNF-22`».

`RNF-20` (contraste ≥ 4.5:1) ya fue añadido y es correcto. `RNF-22` es «contenido libre de
violencia explícita, publicidad, compras integradas y enlaces externos» — no tiene relación
alguna con la coherencia visual.

**Corrección:** dejar `CN-04 → RNF-20`. Retirar `RNF-22`.

---

## INC-22 · La introducción da por concedidos los personajes

**Severidad:** Baja. **Estado:** abierto.

El trabajo de grado condiciona el uso de los personajes de la Familia Anonaky a la autorización
escrita de sus autores en §3.3.2, en §5.2 y en el resumen: «sujeto a la autorización escrita de
sus autores, o personajes originales en su defecto».

La introducción (§ sin numerar, párrafo de impactos) afirma en cambio, sin condición: «En lo
cultural, **usar personajes como la Familia Anonaky** del libro *Tecnología para Niños* […] busca
que los estudiantes se identifiquen con contenidos digitales cercanos a su contexto».

**Corrección:** condicionar también esa frase, con la misma fórmula del resumen.

**Efecto en el código:** ninguno. `SPEC.md` supuesto 3 ya toma personajes originales por defecto.

---

## INC-24 · Paginación de HU y encabezado de arquitectura

**Severidad:** Baja. **Estado:** abierto, estrechado respecto de la rev. 3.

**Ya corregido.** El documento de historias se renombró a
`Historias_de_Usuario_HU01_HU18_v2.docx` y contiene efectivamente HU-01 a HU-18.

**Lo que queda mal:**

- El encabezado de cada página del documento de HU sigue diciendo «**Página N de 16**», de la
  página 1 a la 16, cuando hay dieciocho historias. HU-17 y HU-18 se añadieron sin rehacer la
  paginación, y de hecho comparten página con las anteriores.
- El documento de arquitectura declara en su primera línea: «**Reemplaza a
  arquitectura_videojuego_v2.docx**» — pero el archivo *es* `arquitectura_videojuego_v2.docx`.
  Un documento no puede reemplazarse a sí mismo. La redacción viene de cuando la versión
  alineada iba a ser un archivo aparte.

**Corrección:** rehacer la paginación de HU a 18 páginas; en la arquitectura, sustituir la línea
por «Versión alineada. Reemplaza a la revisión anterior de este mismo documento».

---

## INC-25 · HU-17 lleva el flujo y los criterios de otras historias

**Severidad: Alta.** **Estado:** abierto.

HU-17 («Pausar o reiniciar un nivel», `RF-07`) está corrompida por copiar y pegar. Cuatro
defectos distintos en la misma historia:

1. **El flujo básico termina en otra historia.** Los pasos 1 a 5 describen la pausa
   correctamente. Los pasos 6 a 8 son de HU-16 (borrar datos): «6. El docente confirma **la
   eliminación**. 7. El sistema **elimina el perfil y todos sus indicadores** del almacenamiento
   local. 8. El sistema regresa a la **pantalla de progreso** con la lista actualizada.»
2. **FA-01 aparece dos veces**, con el mismo texto palabra por palabra.
3. **Los criterios de aceptación son íntegramente los de HU-18.** Los ocho hablan del menú
   principal, de Salir, de la pantalla de créditos, del reconocimiento de autoría y del contraste
   de los créditos. **Ni uno solo menciona la pausa.**
4. **No tiene sección de datos de entrada.** Tras las reglas de negocio empieza directamente el
   control de cambios de HU-18. **HU-18 tampoco la tiene**: el documento termina en sus reglas de
   negocio. Son las dos únicas historias de las dieciocho sin esa tabla.

Las reglas de negocio de HU-17 sí son correctas y valiosas — conviene conservarlas tal cual.

**Corrección:**

- Truncar el flujo básico en el paso 5 (`5. El estudiante acciona Continuar y el sistema
  restituye el estado exacto en que se abrió la pausa.`).
- Borrar el FA-01 duplicado.
- Escribir los criterios de aceptación propios, derivados de `RF-07` y de OE2 §2 HU-17:
  el menú de pausa ofrece Continuar, Reiniciar nivel y Volver al menú principal; mientras está
  abierto el nivel queda detenido; Continuar restituye el estado exacto; Reiniciar pide
  confirmación y nunca vuelve a bloquear un nivel ya desbloqueado; el botón de pausa no aparece
  en escenas narrativas.
- Añadir la tabla de datos de entrada a **HU-17 y a HU-18**: en HU-17, botón Pausa, Continuar,
  Reiniciar nivel, Volver al menú principal, confirmación de reinicio y el menú de pausa como
  elemento de sistema; en HU-18, botón Salir, confirmación de salida, botón Créditos, la pantalla
  de créditos, el botón de volver y el estado del perfil activo que se persiste antes de cerrar.

**Por qué es Alta:** `RF-07` es de prioridad Alta y hoy **no tiene ninguna historia que lo
describa correctamente**. Es la tarjeta Kanban que un desarrollador tomaría para implementar la
pausa, y le diría que borre datos.

---

## INC-26 · HU-14 muestra cifras al estudiante

**Severidad: Alta.** **Estado:** abierto.

`RF-45` es explícito: «Al terminar un nivel, el sistema debe mostrar al estudiante un resumen
legible de lo que realizó, redactado en **lenguaje narrativo y sin cifras**, conforme a `RF-17`.
**Los valores numéricos de los indicadores se presentan únicamente en la consulta del docente
(`RF-46`).**» OE1 §3.6.1, nota 3, lo repite: «Ningún indicador se muestra al estudiante como
cifra, ni durante el juego ni en el resumen de cierre (`CP-03`, `RF-17`)».

HU-14 ordena lo contrario en dos lugares:

- Flujo básico, paso 6: «El sistema muestra el resumen de desempeño del nivel (**intentos,
  errores corregidos, tiempo**).»
- Datos de entrada: «Resumen de desempeño (sistema) · Datos calculados · **Indicadores del nivel:
  número de intentos, errores corregidos, pasos utilizados y tiempo de resolución** (`RF-45`).»

La regla de negocio de la propia HU intenta salvarlo — «El resumen no usa calificaciones ni
puntajes; los indicadores son descriptivos» — pero «número de intentos» es una cifra, la llame
descriptiva o no.

**Corrección:**

- Paso 6: «El sistema muestra el resumen narrativo del nivel: qué hizo el estudiante, contado en
  lenguaje observacional y **sin cifras** (`RF-45`, `RF-17`).»
- Datos de entrada: «Resumen narrativo (sistema) · Texto generado · Descripción de lo realizado
  en el nivel, sin valores numéricos. Los indicadores se registran en el perfil y se consultan
  solo desde `TeacherReport` (`RF-46`).»

**Por qué es Alta:** es el invariante pedagógico que más documentos sostienen —`CP-03`, `RF-17`,
`RF-45`, la nota 3 de OE1 §3.6.1 y la propia arquitectura, que prohíbe `ScoreManager`— y el
resumen de fin de nivel es el punto donde más fácil se cuela una cifra.

---

## INC-27 · `RNF-09` no autoriza el progreso que `RF-04` obliga a guardar

**Severidad:** Media. **Estado:** abierto.

`RNF-09`: «El sistema **únicamente** podrá almacenar el nombre o alias del estudiante y sus
indicadores de desempeño.» OE1 §3.6.1, nota 5, lo refuerza: «No se almacena ningún dato distinto
de estos cuatro indicadores y del nombre o alias del estudiante».

Pero `RF-04` exige guardar «el avance del perfil activo al completar cada fase» y `RF-03` exige
habilitar cada nivel «solo cuando el nivel anterior haya sido completado **por el perfil
activo**». Sin persistir el nivel alcanzado y las fases confirmadas, ninguno de los dos se puede
cumplir. `RNF-14` (recuperar tras cierre inesperado) tampoco.

Leída al pie de la letra, `RNF-09` prohíbe el dato que `RF-03`, `RF-04` y `RNF-14` obligan a
guardar.

**Corrección:** ampliar `RNF-09` a «el nombre o alias del estudiante, su progreso de avance
—nivel alcanzado y fases confirmadas— y sus indicadores de desempeño», y ajustar la nota 5 de
OE1 §3.6.1 en el mismo sentido. La intención del requerimiento —no recoger datos sensibles,
imágenes, ubicación ni contacto— queda intacta.

---

## INC-28 · La omisión de escenas está limitada en OE1 y libre en el resto

**Severidad:** Media. **Estado:** abierto.

`RF-06`: «El sistema debe permitir avanzar los diálogos con un clic y **omitir una escena ya
vista** mediante un botón visible de omisión.» La condición «ya vista» es deliberada: evita que
el estudiante se salte el planteamiento del problema la primera vez.

Los documentos de menor prioridad la pierden:

| Documento | Texto | Problema |
|---|---|---|
| CU-03, flujo alterno 2a | «El estudiante acciona el botón de omitir: el sistema salta directamente al final de la escena» | Sin condición de escena ya vista |
| HU-02, FA-01 | «El estudiante acciona el botón de omitir → el sistema salta directamente a la escena jugable» | Ídem |
| HU-02, FA-02 | «El nivel ya fue visitado previamente → el sistema permite omitir la introducción **sin mostrar el botón de bloqueo**» | Introduce un «botón de bloqueo» que no existe en ningún requerimiento |
| HU-14, FA-01 | «El estudiante **omite la escena de cierre** → el sistema salta al resumen» | El cierre reflexivo es la escena donde el guía nombra la habilidad practicada: omitirla la primera vez vacía `RF-12` y `CP-07` |

**Corrección:** en CU-03 y HU-02 FA-01, añadir «si la escena ya fue vista»; suprimir el «botón de
bloqueo» de HU-02 FA-02; y en HU-14 partir el flujo alterno en dos, con esta redacción:

> **FA-01:** El estudiante ya había completado este nivel antes y vuelve a jugarlo → la escena de
> cierre muestra el botón de omitir; al accionarlo el sistema salta al resumen narrativo sin
> reproducir los diálogos, y los indicadores del nuevo intento se registran igual (`RF-06`,
> `RF-04`).
>
> **FA-02:** Es la primera vez que el estudiante completa el nivel → **el botón de omitir no se
> muestra**: la escena de cierre se reproduce entera, porque es donde el guía nombra la habilidad
> de pensamiento computacional practicada (`RF-12`, `CP-07`).

Dos flujos alternos en vez de una condición dentro de uno, para que el plan de pruebas de OE4
pueda verificar los dos casos por separado. El resumen es **narrativo**, no de desempeño, conforme
a la corrección de INC-26.

**No hace falta un dato nuevo.** «Escena ya vista» es derivable del progreso que `RF-03` y `RF-04`
ya obligan a persistir: el cierre del nivel N se ha visto exactamente cuando el perfil tiene el
nivel N completado, y la introducción cuando el nivel fue iniciado antes. Un registro aparte de
escenas vistas ampliaría el alcance de `RNF-09` y agrandaría INC-27 sin necesidad.

---

## INC-29 · HU-10 nombra indicadores fuera de la lista cerrada

**Severidad:** Media. **Estado:** abierto.

OE1 §3.6.1 fija cuatro indicadores y declara que **la lista es cerrada**: «no se registra ningún
indicador adicional». Son intentos, errores corregidos, pasos utilizados y tiempo de resolución,
con definición operativa por nivel.

HU-10, reglas de negocio: «Al completar el nivel se registran: **número de ejecuciones, bloques
usados y errores de movimiento**, para el resumen de desempeño (`RF-45`).»

Ninguno de los tres es un indicador de la lista. Son, además, las magnitudes de las que se
derivan: la definición operativa del Nivel 2 fase 3 ya dice que «intentos» son las ejecuciones
que no alcanzan el refugio, «pasos utilizados» los bloques de la secuencia exitosa y «errores
corregidos» los bloques retirados o reordenados entre una ejecución fallida y la siguiente.

**Corrección:** reescribir la regla como «Al completar el nivel se registran los cuatro
indicadores de OE1 §3.6.1 con la definición operativa de la fase 3: intentos = ejecuciones que no
alcanzan el refugio; errores corregidos = bloques retirados o reordenados entre una ejecución
fallida y la siguiente; pasos utilizados = bloques de la secuencia ejecutada con éxito; tiempo de
resolución.»

---

## INC-30 · Tres fases de ensamblaje contra cuatro tareas

**Severidad:** Media. **Estado:** abierto.

Tres cuentas distintas para la misma mecánica del Nivel 3:

| Fuente | Qué dice |
|---|---|
| `RF-40` y guion §8.3 | **Tres fases** de ensamblaje: base, amarre, mástil y vela |
| Guion §8.1 y `RF-36` | **Cuatro tareas**: 1 recoger troncos · 2 encontrar sogas · 3 ensamblar la balsa · 4 colocar el mástil y la vela |
| OE1 §3.6.1, «Pasos utilizados», Nivel 3 | «Confirmaciones de fase aceptadas: **base, amarre, mástil y vela**» — enumeración que se lee como **cuatro** confirmaciones |

Las tareas 1 y 2 se marcan al recoger material. Quedan **dos** tareas de construcción para
**tres** fases de ensamblaje, y ningún documento dice cuál marca cuál. HU-11 propone una
correspondencia —«Las tareas 3 y 4 se marcan al confirmar las fases de **amarre** y de **mástil y
vela**»— que deja la fase de base sin marcar ninguna tarea, sin decirlo.

**Corrección propuesta:**

- En `RF-45` / OE1 §3.6.1, escribir la enumeración sin ambigüedad: «Confirmaciones de fase
  aceptadas, sobre un máximo de tres: base, amarre, y mástil y vela».
- Fijar la correspondencia en el guion §8.2 y en HU-11: **la tarea 3 («ensamblar la balsa») se
  marca al confirmar la fase de amarre**, porque es la que cierra la estructura de la balsa —base
  más amarre—; **la tarea 4 se marca al confirmar la fase de mástil y vela**. La fase de base
  queda dentro de la tarea 3 y no marca nada por sí sola.

Esta es la lectura de HU-11 y la más natural narrativamente: «ensamblar la balsa» no está hecho
cuando solo hay troncos sueltos.

---

## INC-31 · Actores y referencias cruzadas erróneas

**Severidad:** Baja. **Estado:** abierto.

Tres errores de dato en el documento de historias:

| Ubicación | Dice | Debe decir |
|---|---|---|
| HU-18, *Actores* | «Docente, **Docente**» | «Docente, Estudiante» — su propia descripción empieza «Como docente **o estudiante**» |
| HU-18, *Historias asociadas* | «**HU-15** · Borrar los datos de un estudiante» | «HU-16 · Borrar los datos de un estudiante». HU-15 ya figura en la fila anterior con su nombre correcto |
| HU-01, *Actores* | «Docente (con acompañamiento del estudiante)» | La descripción es «Como **estudiante** quiero entrar al juego con mi nombre». Debe incluir al estudiante como actor |

---

## INC-32 · `RF-19` recondiciona «Soplar» tras la convergencia

**Severidad:** Media. **Estado:** abierto.

`RF-19`: «El botón de soplar debe permanecer deshabilitado y activarse únicamente cuando el
jugador haya ubicado al personaje **en la posición adecuada** y acumulado el número mínimo de
golpes efectivos.»

La conjunción es problemática. El guion §4.3.5 modela la convergencia como un estado que, una vez
alcanzado, no se revierte: «E6 · Convergencia. El botón "Soplar" **se habilita** y el montón pasa
a estado humeante». Y §4.3.6 fija la regla: «El contador de golpes efectivos no se reduce por
intentos fallidos posteriores: **lo ganado permanece**».

Con la letra de `RF-19`, si el jugador alcanza los tres golpes efectivos y después mueve el
control deslizante a «Lejos», «Soplar» **se vuelve a deshabilitar** — lo ganado se pierde, que es
exactamente lo que `CP-02` prohíbe.

**Corrección:** suprimir la condición de posición, o precisarla: «se activa cuando el jugador ha
acumulado el número mínimo de golpes efectivos definido en la configuración del nivel, y
permanece activo a partir de ese momento».

---

## INC-33 · Semántica de los bloques del laberinto sin definir

**Severidad: Alta**, y es una **decisión de diseño**, no de redacción. **Estado:** abierto.

`RF-31` y el guion §6.3.2 definen tres bloques: «**Avanzar izquierda**», «**Avanzar derecha**» y
«**Girar**». Ninguno de los dos documentos dice:

1. Si «izquierda / derecha» es **absoluto** (respecto de la pantalla) o **relativo** (respecto de
   la orientación de la carretilla).
2. Cuánto rota «Girar», ni en qué sentido.

Las dos lecturas dan juegos distintos, y una de ellas no funciona:

- **Absoluta.** Los dos bloques de avance mueven en horizontal. **Ninguna secuencia se desplaza
  en vertical**, y en un laberinto en vista superior el refugio puede ser sencillamente
  inalcanzable. «Girar» quedaría sin efecto sobre la posición.
- **Relativa.** «Avanzar» mueve según la orientación actual —adelante y atrás— y «Girar» rota,
  con lo que la carretilla alcanza cualquier casilla. Es el modelo clásico de los editores de
  bloques para primaria.

**Corrección:** fijar la lectura **relativa** en el guion §6.3.2 y en `RF-31`, y precisar la
rotación de «Girar» (90° en un sentido fijo). Renombrar los bloques ayudaría: «Avanzar» /
«Retroceder» / «Girar» dice lo que hacen; «Avanzar izquierda» sugiere la lectura absoluta.

**Hay que decidirla antes de dibujar el laberinto**, porque el trazado depende de ella.

---

## INC-34 · El fallback de persistencia no está en el criterio de `RNF-11`

**Severidad:** Media. **Estado:** abierto.

La arquitectura §7 fija la ubicación en `Datos/`, junto al ejecutable, y añade: «Si `Datos/` no es
escribible —carpeta de red o unidad de solo lectura— **se cae a
`Application.persistentDataPath`** y se advierte al docente». HU-18 FA-04 recoge el mismo caso.

El criterio de verificación de `RNF-11` es «verificación de la **ausencia de residuos en el
almacenamiento local**» tras eliminar un perfil. Si existe una ruta de respaldo y la rutina de
eliminación solo borra `Datos/`, la prueba pasa mirando una carpeta mientras los datos siguen en
`%AppData%\LocalLow`.

**Corrección:** dejar escrito en la arquitectura §7 —y en el plan de pruebas de OE4— que **la
eliminación de un perfil borra las dos rutas**, y que la prueba de `RNF-11` se ejecuta en los dos
escenarios: con `Datos/` escribible y con `Datos/` de solo lectura.

---

## INC-35 · CU-11 presenta los indicadores solo por nivel

**Severidad:** Media. **Estado:** abierto.

`RF-46`: «presentando los valores numéricos de los indicadores definidos en la tabla 3.6.1 **por
nivel y por fase**». `RF-45` registra los indicadores «por cada **fase** de cada nivel».

CU-11, flujo principal, paso 4: «El sistema presenta los indicadores **por nivel**: intentos,
errores corregidos, pasos utilizados y tiempo de resolución.» Sin la desagregación por fase.

La diferencia no es cosmética: el Nivel 2 tiene tres fases con facetas distintas —patrón,
construcción, algoritmo— y un total agregado del nivel no le dice al docente en cuál se atascó el
estudiante, que es el propósito del informe.

**Corrección:** en CU-11 paso 4, «los indicadores por nivel y por fase».

---

## INC-36 · Datos internos inconsistentes en el trabajo de grado

**Severidad:** Baja. **Estado:** abierto. **Enunciado corregido respecto de la rev. 3.**

| Dato | Dónde dice una cosa | Dónde dice otra |
|---|---|---|
| Edad de la población | §1.2: «edades entre **9 y 10** años» | §3.4 y §5.3: «entre los **9 y 11** años». OE1 §2.3 y el guion §1 dicen 9 a 11 |
| Países en Bebras | Glosario: «Colombia participa en ella junto a **77** países» | §3.1.2: «**78** países» |
| Puntaje Bebras de grado cuarto | Introducción: «**4,57** puntos sobre 15 en 2025» | §1.2: «**4,6** puntos sobre en grado cuarto 15» |
| Duración del proyecto | Resumen, §5 y cronograma: «**catorce** semanas», S1 a S14 | §8 Presupuesto: recursos humanos facturados a **12** semanas |

La frase de §1.2 «apenas 4,6 puntos sobre en grado cuarto 15» además tiene las palabras
descolocadas.

**Corrección:** unificar la edad en **9 a 11 años** (es la que sostienen tres documentos), el
número de países, el puntaje con dos decimales, y elevar el presupuesto de recursos humanos a
catorce semanas o justificar por qué se facturan doce.

> **Retirado de este hallazgo:** la rev. 3 incluía «el subtotal de hardware». Al recalcularlo,
> cuadra: $10.061.206 × 2 = $20.122.411 (por el redondeo de $10.061.205,63) más $3.600.000 =
> $23.722.411, y el total del proyecto $39.135.811 también suma. No hay error.

---

## INC-37 · `RF-44` y `RF-46` no tienen historia de usuario

**Severidad:** Media. **Estado:** abierto. **Nuevo en la rev. 4.**

`CT-10` exige que todo requerimiento tenga identificador único y caso de prueba. `RNF-17` exige
que cada commit se asocie a su tarjeta del tablero Kanban, y OE2 §2 declara que **las historias
de usuario son las tarjetas del tablero**.

Recorriendo la matriz de OE2 §3.5 y las dieciocho historias del documento detallado, dos
requerimientos no aparecen en ninguna:

| Requerimiento | Prioridad | Dónde sí está | Historia que lo cubre |
|---|---|---|---|
| `RF-44` · Animación de cruce y escena narrativa de cierre del juego | Alta | CU-10, guion §8.5 y §9 | **ninguna** |
| `RF-46` · Consulta del progreso por el docente | Alta | CU-11 | **ninguna** |

Además, **CU-11 es el único caso de uso sin el campo «Historias de usuario asociadas»**: la fila
falta por completo en su tabla, mientras los otros once la tienen.

Los dos son de prioridad Alta, y el KPI de OE4 exige que la totalidad de los Alta esté
implementada. Hoy no hay tarjeta que los reclame.

**Corrección:** añadir dos historias a OE2 §2 y al documento detallado —

- **HU-19** · «Como estudiante quiero ver a la familia cruzar el río cuando la balsa funcione,
  para saber que llegué al final de la historia» (`RF-44`, `RF-12`).
- **HU-20** · «Como docente quiero consultar el desempeño de cada estudiante por nivel y por
  fase, para saber dónde necesita apoyo» (`RF-46`, `RF-45`).

— y añadir a CU-11 la fila «Historias de usuario asociadas: HU-20». Si se prefiere no ampliar la
numeración, `RF-44` puede absorberse en HU-13 y `RF-46` en HU-16, dejándolo escrito en la matriz
de OE2 §3.5; lo que no puede quedar es un `RF` de prioridad Alta sin tarjeta.

---

## INC-38 · La arquitectura cita `TRAZABILIDAD.md`, que no existe

**Severidad:** Baja. **Estado:** abierto. **Nuevo en la rev. 4.**

La arquitectura declara en su cabecera: «**Documentos que gobiernan este:** OE1 (requerimientos),
el guion (mecánicas y escenas), `SPEC.md` (contrato de desarrollo), **`TRAZABILIDAD.md`
(matrices)**.»

`TRAZABILIDAD.md` no existe en el repositorio. Las matrices de trazabilidad viven en OE1 §5.1 y
en OE2 §3.1 a §3.5.

**Corrección:** sustituir la referencia por «OE1 §5.1 y OE2 §3.1–3.5 (matrices de
trazabilidad)», o crear el documento si se quiere una matriz consolidada aparte.

---

## INC-39 · Ninguna transición lleva a `Credits` al terminar el Nivel 3

**Severidad:** Media. **Estado:** abierto. **Nuevo en la rev. 4.**

El guion §9 cierra el juego así: «Chispa gira una última vez y se apaga suavemente. Fundido a
negro. **Créditos**.» `RF-44` habla de «la escena narrativa de **cierre del juego**», distinta del
cierre de nivel.

La tabla de transiciones de la arquitectura §4 no contempla ese final. Su última fila es:

> `LevelSummary` → continuar → `LevelSelect`, con el siguiente nivel desbloqueado (`RF-03`,
> `RF-04`)

Tras completar el Nivel 3 no hay «siguiente nivel», y el estado `Credits` solo es alcanzable
desde `MainMenu`. La escena final del guion no tiene camino en la máquina de estados.

**Corrección:** añadir a la tabla la transición

> `LevelSummary` → juego completado → `Narrative` (escena final) → `Credits` (`RF-44`, `RF-08`)

y, desde `Credits`, el retorno a `MainMenu`.

---

## INC-40 · UI, Audio y Datos no tienen assembly en la lista de §9

**Severidad:** Media. **Estado:** abierto. **Nuevo en la rev. 4.**

La arquitectura §6 reparte los componentes en capas e incluye tres filas con módulo
«transversal»: **UI** (`HUDController`, menús, pausa — `RF-07`, `RNF-03`, `RNF-19`), **Audio**
(`AudioManager`) y **Datos** (ScriptableObjects).

La lista de assemblies de §9 no las contempla: «`Game.Core`, `Game.Scaffolding`,
`Game.Levels.Fire`, `Game.Levels.Wheel`, `Game.Levels.River`, `Game.Reporting`, más un assembly
de pruebas por cada uno.»

No hay `Game.UI` ni `Game.Audio`. Pero `AudioManager` es uno de los **tres singletons con
`DontDestroyOnLoad`** que §8 autoriza, y `HUDController` aparece en §8 relacionándose con los
controladores de nivel por referencia de Inspector. Ambos tienen que compilar en algún sitio.

Peor: si UI y Audio caen dentro de `Game.Core`, `Game.Core` pasa a depender de la UI, y la
promesa de §5 —probar `GameFlow` en EditMode sin escena ni frames— se vuelve más difícil de
sostener.

**Corrección:** añadir `Game.UI` y `Game.Audio` a la lista de §9, con dependencia hacia
`Game.Core` y nunca al revés. Los ScriptableObjects de contenido no necesitan assembly propio:
sus tipos viven en el assembly del módulo que los consume.

---

## INC-41 · HU-02 generaliza la lista de tareas a todos los niveles

**Severidad:** Media. **Estado:** abierto. **Nuevo en la rev. 4.**

`RF-36` sitúa la lista de tareas visible **en el Nivel 3**: «El sistema debe mostrar de forma
permanente la descomposición del objetivo en tareas (recoger troncos, encontrar sogas, ensamblar
la balsa, colocar el mástil y la vela)». El guion la introduce en §8.1, también en el Nivel 3.
Los niveles 1 y 2 no tienen lista: el Nivel 1 se resuelve en un panel y el Nivel 2 con un
contador y una secuencia de pasos.

HU-02 («El guía explica el objetivo del nivel») la generaliza a todos:

- Flujo básico, paso 4: «El NPC guía descompone el objetivo principal en tareas concretas y las
  muestra **visibles en pantalla**».
- Criterio de aceptación: «**La lista de tareas permanece visible durante toda la escena
  jugable**.»

Y a la vez, su regla de negocio dice: «El guía muestra **una tarea activa a la vez** en pantalla
(`RNF-03`)» — que es lo contrario de una lista permanente con cuatro entradas, salvo que se
distinga «visible» de «activa», distinción que ningún documento hace.

**Corrección:** en HU-02, acotar el criterio a «en los niveles que tengan lista de tareas
(`RF-36`, Nivel 3), la lista permanece visible durante toda la escena jugable», y precisar en
`RNF-03` que la restricción es sobre la tarea **activa**, no sobre cuántas se muestran: una lista
que marca lo hecho y señala la siguiente no vulnera `RNF-03`.

---

## INC-42 · Norma de citación declarada distinta de la usada

**Severidad:** Baja. **Estado:** abierto. **Nuevo en la rev. 4.**

El trabajo de grado §6 (*Productos a entregar*) declara: «Documento escrito del trabajo de grado,
elaborado conforme a la norma **NTC 1486** y con citación bajo la norma **ISO 690**».

El documento usa en realidad **citación numérica entre corchetes** —`[1]`, `[2]`, `[7]`…— que es
el estilo **IEEE**, y el propio nombre del archivo es `Trabajo_de_Grado_2026_ICONTEC_IEEE.docx`.
ISO 690 admite el sistema numérico, pero el formato de las referencias y el nombre del archivo
apuntan a IEEE.

**Corrección:** declarar en §6 la norma que efectivamente se usa —«citación bajo norma IEEE»— o,
si el requisito institucional es ISO 690, rehacer la bibliografía en ese formato. Es un dato de
entrega y conviene que el documento y su nombre digan lo mismo.

---

## Hallazgos cerrados

**Cerrados en la rev. 3** (verificados de nuevo el 30/08/2026: la corrección sigue en el
documento):

| ID | Hallazgo | Cómo quedó |
|---|---|---|
| INC-02 | Redacción de `RF-16` ambigua (= PG-03 del guion) | `RF-16` dice hoy que desde cualquier posición el golpe se ejecuta y produce consecuencia visible; solo desde la posición efectiva cuenta como golpe efectivo |
| INC-13 | `RF-32` especificaba doble clic (= PG-04 del guion) | Normalizado a clic simple |
| INC-17 | Prioridad de `RF-46` | Elevada de Media a Alta; `RF-47` deja de depender de una decisión de producto |
| INC-18 | `RF-45` no distinguía resumen del estudiante e informe docente | `RF-45` separa hoy el resumen narrativo sin cifras de los valores numéricos de `RF-46` |
| INC-19 | Indicadores sin definición operativa por nivel | OE1 §3.6.1 los define uno a uno, con lista cerrada y cinco notas |
| INC-20 | `RF-35` decía «teclas de dirección» | Reescrito a «botones de dirección mostradas en los costados… de la pantalla». Los residuos en otros documentos son INC-01 |
| INC-23 | Alcance de las sesiones con estudiantes | §1.6 y §5.3 declaran verificación funcional y de usabilidad, sin medición del efecto pedagógico |

**Cerrados antes de la rev. 3:** INC-03 a INC-12, INC-14 y INC-15. Su contenido no se conserva en
el repositorio; los identificadores quedan retirados y no se reutilizan.

**Puntos abiertos del guion (§12)**, que son del guion y no conflictos entre documentos: `PG-01`
(título del producto), `PG-02` (nombre definitivo del guía), `PG-05` (verificar en pruebas el
cambio de esquema de control entre niveles), `PG-06` (validar jugando los valores del Nivel 1),
`PG-07` (autorización de los personajes). `PG-03` y `PG-04` están cerrados (INC-02, INC-13).
