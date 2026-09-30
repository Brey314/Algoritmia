# Inconsistencias entre los documentos fuente

Registro de los conflictos detectados entre los `.docx` de `docs/`, con la corrección
aplicada a cada uno. Documento hermano de `SPEC.md`: aquí está **qué estaba mal en los
documentos y cómo quedó**; allí está **qué implementa el código**.

**Los documentos se refundieron el 14/09/2026: de seis pasaron a cuatro.** OE1 es ahora
`Solución OE1_Requerimientos.docx`, y `Solucion_OE2_Diseno_final.docx` absorbe en un solo archivo
el guion (§1), los casos de uso (§2), las historias de usuario (§2 bis), las matrices de
trazabilidad (§3) y la arquitectura (§4). El trabajo de grado sigue en `docs/`
(`Trabajo_de_Grado_2026_ICONTEC_IEEE (2).docx`, con su conversión en `docs/md/`). Las citas por
sección de este documento siguen valiendo con una traducción mecánica: «guion §N» → `Solucion_OE2`
§1.N, «OE2 §4» → `Solucion_OE2` §5 (control de cambios).

**Verificación vigente: 30/09/2026, rev. 15.** Se contrastaron todos los documentos —los `.docx`
de `docs/` y los de `claudeDocs/`— con el juego. Por decisión de Santiago, **todos los hallazgos
están cerrados**: ante un conflicto gana el juego y se edita el documento; lo que solo pide el
documento se implementa en el juego; lo que solo tiene el juego se añade al documento. Se cerraron
así los nueve que seguían abiertos (**INC-46** a **INC-54**), los tres residuos (`INC-24-r`,
`INC-44-r`, `INC-44-r2`) y el punto `PG-01` del guion, y se registraron y cerraron sesenta
hallazgos nuevos, **INC-55** a **INC-114**. Siguen abiertos solo `PG-05` y `PG-06`, puntos del guion
que exigen observar a estudiantes jugando, y tres **pendientes de Santiago** que el juego no zanja
(ver «Residuos y puntos abiertos»). El entregable del OE3 no se reescribe: recibe una nota
fechada con estos cierres. La rev. 15 (30/09/2026) registra y cierra **INC-115** a **INC-117**,
tres puntos que la verificación final dejó a criterio de Santiago y en los que él decidió corregir
el juego.

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
| 1 | Trabajo de grado — `docs/Trabajo_de_Grado_2026_ICONTEC_IEEE (2).docx` | Objetivos, KPI, alcance, marco jurídico, metodología |
| 2 | `Solución OE1_Requerimientos.docx` | Lineamientos CP/CT/CN, RF-01..RF-47, RNF-01..RNF-23 |
| 3 | `Solucion_OE2_Diseno_final.docx` §1 (guion) | Narrativa, mecánicas, parámetros y textos exactos |
| 4 | `Solucion_OE2_Diseno_final.docx` §2–§3 | CU-01..CU-12, HU-01..HU-18, matrices |
| 5 | `Historias_de_Usuario_HU01_HU18_v2 (1).docx` | HU detalladas: flujos, criterios, reglas de negocio |
| 6 | `arquitectura_videojuego_v2 (2).docx` · `Solucion_OE2` §4 | Decisiones técnicas de implementación |

Las filas 3 y 4 viven en el **mismo archivo**, así que un conflicto entre ellas ya no es un
conflicto entre documentos: se corrige editándolo (ver «contradicciones internas», abajo).

Las contradicciones **internas** a un mismo documento se corrigieron editándolo.

---

## Resumen — estado a 30/09/2026 (rev. 15)

| ID | Hallazgo | Documentos | Estado |
|---|---|---|---|
| INC-01 | «Teclas de dirección» residual | Guion, HU, Arquitectura | **Cerrado** |
| INC-16 | La arquitectura no citaba `RNF-18` | Arquitectura | **Cerrado** |
| INC-21 | CN-04 trazaba a `RNF-22` (luego `RNF-21`) | OE2 | **Cerrado** |
| INC-22 | La introducción daba por concedidos los personajes | Trabajo de grado | **Cerrado** |
| INC-24 | Paginación de HU y cabecera de arquitectura | HU, Arquitectura | **Cerrado** (residuo corregido el 29/09/2026) |
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
| INC-44 | El guía se llama Algoritm; los documentos decían «Chispa» | Guion, HU, OE1, OE2 | **Cerrado** (14/09/2026; residuos corregidos el 29/09/2026) |
| INC-45 | El guía cambia de forma por nivel; el guion fijaba una sola | Guion | **Cerrado** (14/09/2026) |
| INC-46 | La lista de tareas del Nivel 3: cuerda con nudos contra panel de casillas | Dirección de arte, Slice 3 | **Cerrado** (29/09/2026) |
| INC-47 | La mecánica del Nivel 1 reúne los materiales en un círculo y luego mide fuerza y cercanía de las piedras en dos deslizantes; el guion y RF-15 fijan un deslizante de posición | Guion, OE1, HU | **Cerrado** (29/09/2026) |
| INC-48 | Dos arquitecturas contradictorias en `docs/`: la refundición trajo de vuelta el capítulo anterior a la alineación | OE2 §4, Arquitectura | **Cerrado** (29/09/2026) |
| INC-49 | El menú de pausa implementado es el del mockup 6 (Reanudar · Reiniciar · Volver al menú de niveles); HU-17 dice «Continuar / Reiniciar nivel / Volver al menú principal» | HU, OE2 §3 | **Cerrado** (29/09/2026) |
| INC-50 | «Empujar» en el bosque no anima el rodado: sale a la escena 2.2, que lo cuenta; RF-26 y HU-08 lo describen dentro de la mecánica | OE1, HU, CU | **Cerrado** (29/09/2026) |
| INC-51 | El cierre del Nivel 3 nunca ofrece omitir; HU-14 FA-01 lo pide a quien repite el nivel | HU | **Cerrado** (29/09/2026) |
| INC-52 | El Algoritm entregado es una llama con brazos, piernas y franja de colores; §7.6 pide una estrella sin extremidades | Dirección de arte, Interfaces | **Cerrado** (29/09/2026) |
| INC-53 | Los personajes se animan por recorte con `Image` de uGUI; §13.1 fija rigging con el paquete 2D Animation | Dirección de arte | **Cerrado** (29/09/2026) |
| INC-54 | El taller del Nivel 2 tiene una séptima pieza, la cuerda, y un último paso, amarrar la caja; el guion §1.6.2.2 y RF-27/RF-29 cierran el armado con la caja | Guion, OE1, HU, CU | **Cerrado** (29/09/2026) |
| INC-55 | Los bloques del laberinto llevan una cuenta de 1 a 9 y un giro a los dos lados, y el editor inserta donde se suelta | OE1, OE2, HU v2, SPEC | **Cerrado** (29/09/2026) |
| INC-56 | La ejecución del laberinto sigue tras un tropiezo, termina al llegar al refugio y su respuesta inmediata es el recorrido | OE2, HU v2, OE1 | **Cerrado** (29/09/2026) |
| INC-57 | El laberinto es un seto con arbustos y su trazado se genera al azar con un camino garantizado | OE1, OE2, Inventario de arte, HU v2, juego | **Cerrado** (29/09/2026) |
| INC-58 | El lado elegido de «Girar» se distinguía solo por el color (RNF-19) | OE4, juego | **Cerrado** (29/09/2026) |
| INC-59 | Los indicadores del Nivel 2 no se definían como los cuenta el juego | OE1, HU v2 | **Cerrado** (29/09/2026) |
| INC-60 | El bosque del Nivel 2: reacción de los objetos al cursor, troncos que se alinean solos y caja soltada fuera | OE2, HU v2 | **Cerrado** (29/09/2026) |
| INC-61 | El taller del Nivel 2: el mazo perfora como «Mecanizar», el candado con «Aún no», el tronco ya perforado y la pieza soltada lejos | OE2, OE1, HU v2, OE4 | **Cerrado** (29/09/2026) |
| INC-62 | El guía plantea el objetivo con preguntas o con indicaciones paso a paso e interviene siempre igual | OE1, OE2, HU v2, OE4 | **Cerrado** (29/09/2026) |
| INC-63 | La ayuda escribe la instrucción en la tablilla y no abre un cuadro que haya que cerrar (HU-03) | HU v2 | **Cerrado** (29/09/2026) |
| INC-64 | Las pistas del guía del Nivel 2 no estaban en el guion | OE2 | **Cerrado** (29/09/2026) |
| INC-65 | Un fallo solo deshace lo del intento, nunca la fase activa (HU-04) | HU v2 | **Cerrado** (29/09/2026) |
| INC-66 | La mecánica del Nivel 1 se ve desde arriba y el Nivel 3 es un plano fijo con perspectiva por profundidad | OE2, Dirección de arte | **Cerrado** (29/09/2026) |
| INC-67 | HU-07 paso 5: la luz del Nivel 1 sube con cada golpe efectivo y termina con el soplido | HU v2 | **Cerrado** (29/09/2026) |
| INC-68 | No se veía ninguna chispa al golpear en el Nivel 1 | OE4, Inventario de arte, juego | **Cerrado** (29/09/2026) |
| INC-69 | Escenas narrativas: «ya vista» es haber completado el nivel, Omitir salta la escena en curso y Continuar avanza | HU v2, OE2 | **Cerrado** (29/09/2026) |
| INC-70 | RF-05: las narrativas son una ilustración fija que la cámara recorre, con personajes y objetos animados | OE1, OE2, Arquitectura, SPEC | **Cerrado** (29/09/2026) |
| INC-71 | El Nivel 2 dura un día y lo cuenta la luz | OE2, Dirección de arte, SPEC | **Cerrado** (29/09/2026) |
| INC-72 | Qué abre cada nivel y dónde se retoma | OE2, Arquitectura | **Cerrado** (29/09/2026) |
| INC-73 | El resumen de fin de nivel: qué muestra, qué hace y a dónde lleva | OE2, HU v2, Interfaces, OE4 | **Cerrado** (29/09/2026) |
| INC-74 | La fase del Nivel 1 y el desbloqueo del nivel siguiente se guardan al mostrarse el resumen | Arquitectura, OE1, HU v2, SPEC, OE4 | **Cerrado** (29/09/2026) |
| INC-75 | Repetir un nivel conserva los indicadores de la primera vez | HU v2, OE1, OE4 | **Cerrado** (29/09/2026) |
| INC-76 | Menú de niveles: el nivel completado no se marcaba y el bloqueado no dice qué falta | Interfaces, OE2, Arquitectura, OE4, juego | **Cerrado** (29/09/2026) |
| INC-77 | Salir no pedía confirmación ni avisaba de la ruta de respaldo | OE4, Interfaces, juego | **Cerrado** (29/09/2026) |
| INC-78 | Autoría: la Familia Anonaky está autorizada y se acredita, y los recursos sonoros también | OE4, OE1, Trabajo de grado, HU v2, Dirección de sonido, juego | **Cerrado** (29/09/2026) |
| INC-79 | Tipografía: sin Fredoka, el informe docente en Baloo 2 y Nunito, y los tamaños reales | Dirección de arte, Interfaces, OE4, juego | **Cerrado** (29/09/2026) |
| INC-80 | Entrada: solo clic y clic sostenido, con una única excepción y el mapa de controles en todas las escenas | HU v2, OE1, SPEC, juego | **Cerrado** (29/09/2026) |
| INC-81 | El menú principal tiene cuatro opciones: Jugar, Créditos, Progreso del equipo y Salir | Interfaces, OE1, HU v2, OE2, Arquitectura, juego | **Cerrado** (29/09/2026) |
| INC-82 | Cinco rótulos de trabajo «… · placeholder» visibles al estudiante | OE4, juego | **Cerrado** (29/09/2026) |
| INC-83 | Creación del perfil: validación del nombre y ejecución sin instalación | HU v2, OE2 | **Cerrado** (29/09/2026) |
| INC-84 | Actores: CU-01 y el resumen de HU-18 conservaban los actores anteriores a INC-31 | OE2 | **Cerrado** (29/09/2026) |
| INC-85 | El perfil también se borra desde el panel «¿Quién juega?» | Interfaces, OE2, HU v2, Arquitectura | **Cerrado** (29/09/2026) |
| INC-86 | Informe docente: indicadores del perfil elegido, sin agregados, y pantalla sin perfiles | OE1, OE2, HU v2 | **Cerrado** (29/09/2026) |
| INC-87 | Qué datos guarda el prototipo y qué se puede hacer con ellos | HU v2, OE1, Trabajo de grado | **Cerrado** (29/09/2026) |
| INC-88 | Nivel 3: la recolección no es una fase y el ensamblaje ocupa las tres | OE2, HU v2, Arquitectura, OE1 | **Cerrado** (29/09/2026) |
| INC-89 | Nivel 3: ocho materiales en cuatro casillas y una balsa de diecisiete espacios | OE1, OE2, HU v2, Dirección de sonido, Dirección de arte | **Cerrado** (29/09/2026) |
| INC-90 | Ensamblaje de la balsa: colocar no valida, el botón siempre está habilitado y se rotula «Listo» o «Probar balsa» | HU v2, OE4, OE2 | **Cerrado** (29/09/2026) |
| INC-91 | Controles del Nivel 3: cruceta abajo a la derecha, con clic sostenido | OE1, OE4, OE2, HU v2, SPEC | **Cerrado** (29/09/2026) |
| INC-92 | `Interfaces.md` tenía el inventario de pantallas desfasado | Interfaces, Dirección de arte | **Cerrado** (29/09/2026) |
| INC-93 | Narrativas del Nivel 3: la escena 3.2 tras el primer fallo y el cruce como cierre reflexivo | HU v2, OE2, Arquitectura | **Cerrado** (29/09/2026) |
| INC-94 | Arquitectura §6: los componentes con los nombres del prototipo | Arquitectura, SPEC | **Cerrado** (29/09/2026) |
| INC-95 | Arquitectura §4, §5 y §8: estados, transiciones y comunicación como los implementa el juego | Arquitectura, SPEC | **Cerrado** (29/09/2026) |
| INC-96 | Assemblies, carpetas y construcción por slices | Arquitectura, SPEC | **Cerrado** (29/09/2026) |
| INC-97 | Identidad y configuración del ejecutable (decisiones D2 y D3) | CLAUDE.md, SPEC, OE4, Arquitectura, juego | **Cerrado** (29/09/2026) |
| INC-98 | CT-04 y RNF-16: cada nivel en sus escenas y su módulo de código | OE1, Arquitectura | **Cerrado** (29/09/2026) |
| INC-99 | RNF-03 no enunciaba la presentación en pantalla que verifican sus pruebas | OE1, OE4 | **Cerrado** (29/09/2026) |
| INC-100 | Matrices de trazabilidad y referencias internas de OE1 y OE2 | OE1, OE2 | **Cerrado** (29/09/2026) |
| INC-101 | El control de cambios de HU v2 no registraba las ediciones del 30/08/2026 | juego | **Cerrado** (29/09/2026) |
| INC-102 | SPEC e INCONSISTENCIAS daban el trabajo de grado por fuera de `docs/` | SPEC | **Cerrado** (29/09/2026) |
| INC-103 | SPEC: la estrategia de pruebas no reflejaba las categorías ni las pruebas reales | SPEC | **Cerrado** (29/09/2026) |
| INC-104 | Quedaba texto visible escrito en C# | juego | **Cerrado** (29/09/2026) |
| INC-105 | Colores de estado: el juego usa una paleta de estado propia, sin rojo de error | Interfaces, Dirección de arte | **Cerrado** (29/09/2026) |
| INC-106 | Área táctil: algunos controles miden menos de 88×88 px | Interfaces, Dirección de arte | **Cerrado** (29/09/2026) |
| INC-107 | Mínima permanencia: el HUD fijo es más que la pausa | Interfaces, Dirección de arte | **Cerrado** (29/09/2026) |
| INC-108 | Cara de los personajes: una sola expresión y la emoción en el cuerpo | Interfaces, Dirección de arte, Inventario de arte | **Cerrado** (29/09/2026) |
| INC-109 | Los personajes no tenían sombra de contacto | juego | **Cerrado** (29/09/2026) |
| INC-110 | El hallazgo del resumen del Nivel 3 superaba las veinte palabras (RNF-01) | juego | **Cerrado** (29/09/2026) |
| INC-111 | El trabajo de grado prometía integrar la resolución de problemas matemáticos | Trabajo de grado | **Cerrado** (29/09/2026) |
| INC-112 | El trabajo de grado describía el patrón de unidades didácticas con Wix y para 7-8 años | Trabajo de grado | **Cerrado** (29/09/2026) |
| INC-113 | El trabajo de grado no recogía herramientas ni actividades del desarrollo | Trabajo de grado | **Cerrado** (29/09/2026) |
| INC-114 | El índice y la lista de figuras del trabajo de grado estaban desactualizados | Trabajo de grado | **Cerrado** (29/09/2026) |
| INC-115 | Soltar el mazo lejos del tronco resaltado contaba como intento | OE1, OE2, HU v2, OE4, juego | **Cerrado** (30/09/2026) |
| INC-116 | Tras un cierre durante la escena de cierre del Nivel 2, el Nivel 3 seguía bloqueado | Arquitectura, HU v2, SPEC, Interfaces, OE4, juego | **Cerrado** (30/09/2026) |
| INC-117 | Nivel 3: «Reiniciar» volvía a la recolección al repetir el nivel ya completado | HU v2, Arquitectura, SPEC, OE4, juego | **Cerrado** (30/09/2026) |

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

*(Nota 29/09/2026: HU-02 FA-01/FA-02, su fila «Botón Omitir» y CU-03 2a se reescribieron con la
regla del juego —«ya vista» es haber completado el nivel (`NarrativeVisitPolicy`, cambio del
17/09) y Omitir salta solo la escena en curso—; HU-17 FA-04 y su regla de negocio dicen ahora que
las narrativas se avanzan con Continuar. RF-06 no cambia. Ver INC-69.)*

### INC-29 · HU-10 nombraba indicadores fuera de la lista cerrada — cerrado
La regla de negocio de HU-10 registra ahora «los cuatro indicadores de OE1 §3.6.1 con la
definición operativa de la fase 3»: intentos = ejecuciones que no alcanzan el refugio; errores
corregidos = bloques retirados o reordenados entre una ejecución fallida y la siguiente; pasos
utilizados = bloques de la secuencia ejecutada con éxito; tiempo de resolución.
(29/09/2026: OE1 §3.6.1 y HU-10 amplían los errores corregidos de la fase 3 a toda edición de la
secuencia —enganchar, retirar, tomar, cambiar cuenta o lado—, que es lo que cuenta
`WheelIndicatorCollector.RecordEdit`.)

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

*(Nota 29/09/2026, INC-55: desde el mockup 10 —Santiago, 13/09/2026— «Avanzar» y «Retroceder»
llevan una cuenta de 1 a 9 casillas y «Girar» rota 90° al lado elegido, izquierda o derecha
(`InstructionBlock`, `SequenceExecutor`). RF-31, el guion §1.6.3.2, CU-08 y HU-10 se alinearon
con esa letra; a HU-10 no había llegado la corrección del 30/08 y aún decía «avanzar izquierda,
avanzar derecha, girar».)*

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

*(Nota 29/09/2026: quedaban tres menciones de la lista en HU-02 —flujo básico paso 6, FA-01 y la
fila «Botón Omitir»—; se acotaron al Nivel 3 o se retiraron.)*

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

### INC-46 · La lista de tareas del Nivel 3 está especificada dos veces, y distinto — cerrado (29/09/2026)

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

**Corrección aplicada (29/09/2026).** Se corrigió `Direccion_de_Arte.md` §10.2 y §14.2 para
describir lo implementado —una tablilla de marfil `#F7EFE2` con una fila por tarea, un círculo liso
de pendiente y un círculo verde con visto de hecha, el texto teñido con el icono, sin borde de cuero
ni separadores—, con la razón por la que este componente se aparta de la vía diegética de §10.1; por
arrastre, `Interfaces.md` §1 y §2, la pieza `sfx_ui_tarea_marcada`, `SPEC.md` y PF-RF36-01. El guion
§1.8.2, CU-09 paso 5 y HU-11 precisan que «Recoger troncos» se marca con el último de los cinco
troncos y que una tarea marcada no se desmarca. `C5` es un `plan.md` y no se reescribe. En el juego,
una prueba vigila que la tarea no se marque antes del último tronco (tarea RI-01).

---

### INC-42 · Norma de citación — cerrado
El trabajo de grado §6 declara «elaborado conforme a la norma NTC 1486 y con citación bajo la
norma **IEEE**», que es la que usa el documento (citas numéricas entre corchetes, nota de la
bibliografía) y la que dice el nombre del archivo (`…ICONTEC_IEEE.docx`).

---

### INC-47 · La mecánica del Nivel 1 es de fuerza y disposición, no de posición — cerrado (29/09/2026)

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

**Corrección aplicada (29/09/2026).** Se reescribieron el guion §1.4.3 entero, la línea de §1.4.4,
§1.2.1, PG-05, PG-06 y §1.11; OE1 RF-14, RF-15 (nombre y texto), RF-16 y las celdas del Nivel 1 de
§3.6.1; HU-04 FA-03, HU-05, HU-06 y HU-07; el resumen de HU-06, §3.5 y CU-05 de OE2; en
`claudeDocs/`, `SPEC.md` (supuesto 7, ejemplos de código, invariante 4 y «Letra vigente»), la
dirección de arte (el montón pasa a tres estados), la dirección de sonido e `Interfaces.md`. El
historial `FireFeedbackLog.Entries` vive solo en memoria: la tablilla muestra el último mensaje y
nadie más lo lee. En el juego: el montón humea al converger (regla b, tarea FI-01) y se corrigen los
comentarios vencidos del Nivel 1 (FI-03). Se completó además: OE2: §1.2 PG-03 (anotado con la lectura que aplica el juego) · OE1: §3.3 RF-14 (el panel de encendido muestra «Soplar»; durante la reunión solo están «Pista» y la pausa).

### INC-48 · La refundición reintrodujo la arquitectura anterior a su alineación — cerrado (29/09/2026)

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

**Corrección aplicada (29/09/2026).** Se sustituyeron los cuerpos de §4.1-§4.5 de
`Solucion_OE2_Diseno_final.docx` por un resumen de lo que implementa el código —nueve assemblies,
nueve estados con `Narrative` y `Playing` parametrizados, doce escenas, `Datos/` con ruta de
respaldo y borrado en las dos, tres singletons en Boot, sin `EventBus` global— con remisión a
`arquitectura_videojuego_v2 (2).docx`, que a su vez se alineó con el juego (INC-94 a INC-96). El
aviso al docente cuando el guardado cae a la ruta de respaldo, que exige la arquitectura §7, se
implementa (tareas CORE-01, UI-06 y UI-04).

**Por qué pasó, que es lo que conviene no repetir.** La refundición consolidó seis documentos en
cuatro copiando capítulos enteros. Los capítulos §1 a §3 se tomaron de las versiones vigentes —su
control de cambios del 30/08/2026 está ahí, e INC-01, INC-33, INC-35 e INC-37 aparecen
aplicados—, pero §4 se tomó de un original sin alinear. **Al consolidar, verificar capítulo por
capítulo contra el control de cambios, no contra el nombre del archivo.**

---

### INC-49 · El menú de pausa de HU-17 no es el del mockup 6 — cerrado (29/09/2026)

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

**Corrección aplicada (29/09/2026).** RF-07, OE1 §3.6.1 nota 4, HU-17 (flujo, FA-01, FA-03,
criterios, reglas y datos), su resumen en OE2 y el párrafo de pausa de la arquitectura usan los
rótulos del mockup 6, y «Reiniciar» repite la fase activa sin repetir la narrativa. Los nombres de
RF-07 y HU-17 se conservan. `SPEC.md`, `Interfaces.md` y `casos.md` hablan de cinco escenas
jugables. En el juego, el comentario de `PauseMenuController` y una prueba de los tres rótulos
(tarea UI-09). Se completó además: HU v2: HU-17 FA-01 y su regla de negocio (el reinicio en el Nivel 3, que desde el 30/09/2026 vuelve a la fase activa también con el nivel completado, INC-117).

### INC-50 · El rodado del Nivel 2 solo se ve en la narrativa — cerrado (29/09/2026)

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

**Corrección aplicada (29/09/2026).** RF-26, el guion §1.6.1.2 y §1.11, CU-06 pasos 4-5 y HU-08
(flujo, criterio y datos) dicen que «Empujar» confirma la fase y que la escena narrativa 2.2 muestra
el rodado; los troncos los alinea el sistema.

### INC-51 · El cierre del Nivel 3 no se puede omitir al repetirlo — cerrado (29/09/2026)

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

**Corrección aplicada (29/09/2026).** HU-14 FA-01 y su fila «Botón Omitir (cierre)» y CU-03 2a
exceptúan el cruce y la escena final del Nivel 3. RF-06 no cambia. Se completó además: HU v2: HU-17 FA-04 y su regla de negocio (sin omitir en el cierre del Nivel 3 ni en la escena final).

### INC-52 · El Algoritm entregado no es la estrella de §7.6 — cerrado (29/09/2026)

**Qué llegó (24/09/2026).** Santiago entregó los sprites base de la familia y de Algoritm
(`…/assets a postproduccion/familia/`). Algoritm es una **llama** con cara, **brazos y piernas de
palo, manos** y una franja de colores en la base. Se usa tal cual como la forma del Nivel 1
(`char_algoritm_n1_fuego_reposo.png`), dentro del botón de ayuda de las cinco mecánicas y como
personaje en las narrativas.

**Qué dicen los documentos.** `Direccion_de_Arte.md` §7.6 e `Interfaces.md` §4.3: «estrella de
cinco puntas», «Extremidades: **Ninguna**», ojos de «dos óvalos negros», contorno `#E2571F` de 8 px
y una silueta que «se cuenta hasta cinco». El guion (§1.1.1, tras INC-45) sigue diciendo «estrella
de fuego en el Nivel 1».

**Formas de rueda y gota.** No hay arte. Los archivos `char_algoritm_n2_rueda_reposo.png` y
`char_algoritm_n3_gota_reposo.png` son **provisionales**: el fuego recoloreado en madera y en agua,
para que cada nivel muestre un guía distinto. El definitivo entra **sustituyendo el archivo con el
mismo nombre**, sin tocar escenas, prefabs ni assets.

**Corrección pendiente.** Decidir si §7.6 e `Interfaces.md` §4.3 pasan a describir el guía
entregado —llama con extremidades en el N1, y el mismo cuerpo en rueda y gota— o si el arte vuelve
a la estrella. Mientras tanto, el código no depende de la forma: todo lo que muestra al guía
referencia esos tres archivos.

**Corrección aplicada (29/09/2026).** El guion §1.1.1, §1.4.1, §1.4.4, §1.5 y §1.7,
`Direccion_de_Arte.md` §7.6 y §18, `Interfaces.md` §4.3, la dirección de sonido §6.1-6.2 e
`Inventario.md` describen la llama con extremidades, recoloreada en madera y agua; «del tamaño de
una palma» se conserva como frase figurada (decisión D4). En el juego se corrigen las dos
acotaciones del N1 que decían «estrella» (tarea DATA-03) y se implementa la estela de puntos de luz
(regla b, tarea SC-01). Se completó además (30/09/2026): la estela no llegaba a verse en las
narrativas, porque muestreaba la raíz del personaje y no la casilla que camina; ahora la sigue y se
borra al quedarse quieto (`GuideTrail_DA76_LaEstelaSigueALaCasillaQueCaminaYSeBorraAlDetenerse`).

### INC-53 · Los personajes se animan por recorte en uGUI, no con 2D Animation — cerrado (29/09/2026)

**Decisión técnica, 24/09/2026.** `Direccion_de_Arte.md` §13.1 pide «rigging 2D en Unity (paquete
2D Animation) sobre los sprites base en A-pose». No cabe: las once escenas son uGUI en un Canvas
*Screen Space Overlay*, y `SpriteSkin` solo deforma un `SpriteRenderer`, que queda **debajo** del
Canvas, tapado por la ilustración. Pasar las escenas a *Screen Space Camera* rompe supuestos del
código y de las pruebas, como `AssemblyPanelController.DragTo` y los `EnPantalla` con cámara nula.

**Qué se hizo.** Cada miembro de la familia se corta en cinco partes —torso, dos brazos, dos
piernas— que son `Image` hijas con el pivote en la articulación (`char_<x>_parte_<parte>.png`), y
un `Animator` las gira y desplaza. Hay un clip por acción (`char_<x>_anim_<accion>.anim`, 21 por
personaje) y un controlador por personaje. El componente es `CharacterRig` (`Game.Scaffolding`), y
lo usan igual la narrativa y las cinco mecánicas. Algoritm es una sola `Image`, para que
sustituirlo sea cambiar un archivo. Se mantiene lo que §13.1 busca: animación sobre el sprite
base, sin hojas de fotogramas; y §13.2–§13.3: respiración de reposo, sin saltos, caídas ni derrota.

**Corrección pendiente.** §13.1: «rigging por recorte con `Image` de uGUI (`CharacterRig`) sobre
los sprites base en A-pose», con la razón.

**Corrección aplicada (29/09/2026).** `Direccion_de_Arte.md` §13.1 describe el recorte con `Image`
de uGUI (`CharacterRig`) y su razón; se ajustaron §7.5, §9.1, §13.3 y §15.4, `Inventario.md` y los
pendientes del Slice 1 y de Personajes. El paquete 2D Animation sigue en el manifiesto, que no se
toca (decisión D3).

### INC-54 · La carretilla se termina amarrando la caja con una cuerda — cerrado (29/09/2026)

**Decisión de Santiago, 25/09/2026**: entra la pieza `prop_n2_pieza_2` —una cuerda— al taller de la
fase 2 del Nivel 2, como pieza que se coloca en la carretilla, y su paso va **después de la caja**.
El armado queda: perforar dos ruedas → eje → tabla → caja → **amarrar la caja con la cuerda**. La
carretilla se da por completa al amarrarla, no al poner la caja.

**Qué se hizo.** `WorkshopPiece.Rope` y `AssemblyStep.Rope`; la cuerda se lleva con clic
sostenido, como el eje, la tabla y la caja, y soltarla antes de que la caja esté encima se rechaza
con «La cuerda todavía no tiene nada que sujetar.», que dice qué falta sin dictar el paso (CP-06).
No hay dibujo de la carretilla con la cuerda: la pieza misma queda sobre la caja
(`AssemblyContent.RopePlacedPosition`). La instrucción del guía (`N2_Guia`, «Construir») y la
escena 2.3 (`N2_Escena23_Construccion`) la nombran.

**Qué dicen hoy los documentos.** Guion §1.6.2.2: «un área de trabajo con seis piezas
dispuestas» y la tabla de seis pasos, que termina en «la carretilla queda completa» al soltar la
caja; §1.6.2.1, la línea de Algoritm «coloca encima la tabla y sobre ella la caja de alimentos».
RF-27 y RF-29, HU (flujo de la historia de la fase 2) y CU-07 describen el mismo armado sin cuerda.
Las pruebas conservan RF-27 y RF-29 en el nombre; la del paso nuevo lleva INC-54.

**Corrección pendiente.** Guion §1.6.2.1 y §1.6.2.2 (siete piezas, paso 7 y su mensaje fuera de
orden), RF-27, RF-29, la historia de la fase 2 y CU-07.

**Corrección aplicada (29/09/2026).** El guion §1.6.2.1 y §1.6.2.2 (siete piezas, paso 7 y su
mensaje fuera de orden), RF-27 y RF-29, HU-09, su resumen en OE2 y CU-07 recogen la cuerda. En el
juego, la escena 2.3 pinta la cuerda que nombra Algoritm (regla b, tarea DATA-04).

### INC-55 · Los bloques del laberinto llevan una cuenta de 1 a 9 y un giro a los dos lados, y el editor inserta donde se suelta — cerrado (29/09/2026)

**El conflicto.** Los documentos describen otros bloques y otro editor que los del laberinto
implementado.

**Qué decían los documentos.** RF-31 y el guion §1.6.3.2 («Bloques disponibles»): «avanzar y
retroceder la mueven una casilla… girar la rota 90° en sentido horario»; guion «Composición de la
secuencia»: «los bloques se enganchan en el orden en que se sueltan»; «Edición»: se retiran o
reubican arrastrándolos; CU-08 paso 2 genérico; HU-10 aún «avanzar izquierda, avanzar derecha,
girar»; SPEC supuesto 8 e INC-33 con la letra vieja.

**Qué hace el juego.** `InstructionBlock` lleva una cuenta de 1 a 9 en «Avanzar» y «Retroceder» y un
lado izquierda o derecha en «Girar» (salen con 1 y a la derecha); la cuenta y el lado se ajustan
sobre el bloque colocado con «−»/«+» y dos flechas; el bloque se engancha donde se suelta; cajón
«Bloques» cerrado al empezar, papelera del bloque seleccionado, compresión con flecha que despliega
y desplazamiento con botones. Cada tipo tiene su silueta. Decisión de Santiago del 13/09/2026
(mockup 10). `BlockSequence.cs`, `MazeSceneController.cs`, `SequenceExecutor.cs`.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: OE1: RF-31 · OE2: §1.6.3.2 fila «Bloques
disponibles»; §2 CU-08 fila «Flujo principal»; §1.6.3.2 fila «Composición de la secuencia»; §1.6.3.2
fila «Edición» · HU v2: HU-10; HU-10 fila «Bloque de instrucción» · SPEC: Supuestos; fila «INC-33 ·
bloques del laberinto»; Párrafo «Cerrado desde la rev. 4».

### INC-56 · La ejecución del laberinto sigue tras un tropiezo, termina al llegar al refugio y su respuesta inmediata es el recorrido — cerrado (29/09/2026)

**El conflicto.** Los documentos no describen cómo sigue la ejecución ni qué ve el estudiante al
terminar, y RF-11/HU-05 exigen un mensaje distinto en menos de un segundo.

**Qué decían los documentos.** Guion §1.6.3.2 («Validación por retroceso», «Condición de éxito»),
CU-08 paso 5 y HU-10 pasos 5-6 no dicen que la ejecución continúa ni que la meta la corta; RF-11,
HU-05 (paso 2, criterios, regla) y su resumen en OE2 piden «un mensaje narrativo en menos de un
segundo» y «no hay un único mensaje genérico».

**Qué hace el juego.** `SequenceExecutor` intenta el movimiento inválido, vuelve y continúa con el
bloque siguiente (lo que faltaba del bloque no se intenta) y termina al alcanzar el refugio. Si no
llega, se resalta el primer bloque que tropezó (o el último), se muestra siempre «La carretilla no
llegó al refugio. Mira dónde se detuvo y corrige tu secuencia.» —la pista al tercer fallo seguido— y
la carretilla vuelve a la salida con la secuencia intacta. El recorrido avanza a 0,6 s por paso.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: OE2: §1.6.3.2 fila «Validación por retroceso»; §2
CU-08 fila «Flujo principal»; §2 HU-05; §1.6.3.2 · HU v2: HU-10; HU-05 · OE1: RF-11.

### INC-57 · El laberinto es un seto con arbustos y su trazado se genera al azar con un camino garantizado — cerrado (29/09/2026)

**El conflicto.** Los obstáculos y el trazado del laberinto no son los que describen los documentos.

**Qué decían los documentos.** RF-30 y guion §1.6.3.2: «obstáculos (piedras, curvas y pendientes)»;
ni el guion ni HU-10 dicen si el trazado es fijo; `Inventario.md` llamaba al sprite «copia de
`_piedra_a`».

**Qué hace el juego.** Un solo sprite de obstáculo, un arbusto (`prop_n2_laberinto_obstaculo`,
×1,65); el anillo exterior es un seto y el refugio es su hueco opuesto. `MazeGrid.Generate` siembra
al cargar la fase: entrada (0,8) y refugio (15,2) fijos, 24 obstáculos en tramos de hasta 5
casillas, rodeo mínimo 2 y camino garantizado; entre ejecuciones el trazado se conserva.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: OE1: RF-30 · OE2: §1.6.3.2 · Inventario de arte:
Nivel 2 · HU v2: HU-10. Tareas de código: WH-02.

### INC-58 · El lado elegido de «Girar» se distinguía solo por el color (RNF-19) — cerrado (29/09/2026)

**El conflicto.** RNF-19 exige un segundo indicador para toda información señalizada por color, y el
lado elegido de «Girar» solo cambia de tinte.

**Qué decían los documentos.** OE1 RNF-19: «La información señalizada por color debe estar
acompañada de un segundo indicador (icono, texto o forma)». `casos.md` PF-RNF19-01 lo anotaba como
observación.

**Qué hacía el juego.** `MazeSceneController.Side` solo cambia el fondo (marfil sombra → atención) y
el tinte del icono; `Izq` y `Der` no llevan contorno.

**Regla (b).** Solo en el documento: se implementa en el juego.

**Corrección aplicada (29/09/2026).** Documentos: OE4: PF-RNF19-01. Tareas de código: WH-01.
Se completó además (30/09/2026): «Empujar», en el bosque, lleva candado mientras está deshabilitado,
como «Soplar» y «Mecanizar» (`ForestScene_RNF19_EmpujarDeshabilitadoLlevaCandado`); PF-RNF19-01 ya
no lo anota como observación.

### INC-59 · Los indicadores del Nivel 2 no se definían como los cuenta el juego — cerrado (29/09/2026)

**El conflicto.** La tabla de OE1 §3.6.1 y HU-10 definen los pasos y los errores corregidos del
Nivel 2 de otro modo.

**Qué decían los documentos.** OE1 §3.6.1: «Pasos utilizados» sin definición para la fase 1;
«Errores corregidos» de las fases 1 y 2 = «acción rechazada seguida de la acción correcta sobre el
mismo elemento»; de la fase 3 = «bloques retirados o reordenados…» (igual en HU-10).

**Qué hace el juego.** `WheelIndicatorCollector`: la fase 1 registra 0 pasos; en las fases 1 y 2 un
rechazo (o varios seguidos) cerrado por la siguiente acción aceptada cuenta un error corregido, sin
comparar elementos; en la fase 3 cuenta toda edición de la secuencia —enganchar, retirar, tomar,
cambiar la cuenta o el lado— entre una ejecución fallida y la siguiente.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: OE1: §3.6.1 fila «Pasos utilizados»; §3.6.1 fila
«Errores corregidos» · registros de tareas: Bloqueantes y preguntas; Pregunta abierta 1 · Pasos
utilizados de la fase 1 · HU v2: HU-10.

### INC-60 · El bosque del Nivel 2: reacción de los objetos al cursor, troncos que se alinean solos y caja soltada fuera — cerrado (29/09/2026)

**El conflicto.** El juego tiene en la fase 1 del Nivel 2 comportamientos que el guion, CU-06 y
HU-08 no recogen.

**Qué decían los documentos.** Guion §1.6.1.2 («Objetos del escenario», «Colocación de la carga»),
CU-06 (alternos 3a y 4a) y HU-08 (FA-01, FA-02).

**Qué hace el juego.** Al acercar el cursor, cada objeto se aparta según su forma y con su sonido
(`ForestObjectNudge`); al completar el acopio se retiran los distractores, los troncos vuelan a una
fila junto a la caja, la cámara se acerca y aparece «Empujar» deshabilitado; soltar la caja fuera
deja la caja donde cayó con «Ahí la caja no toca los troncos. Déjala encima de ellos.» y, si ya
estaba colocada, «Empujar» no vuelve a deshabilitarse.

**Regla (c).** Solo en el juego: se añade al documento.

**Corrección aplicada (29/09/2026).** Documentos: OE2: §1.6.1.2; §1.6.1.2 fila «Colocación de la
carga»; §2 CU-06 fila «Flujos alternativos» · HU v2: HU-08. Se completó además: OE1: §3.4 RF-25 (la caja se toma con los cinco troncos reunidos y se deja al soltar el clic).

### INC-61 · El taller del Nivel 2: el mazo perfora como «Mecanizar», el candado con «Aún no», el tronco ya perforado y la pieza soltada lejos — cerrado (29/09/2026)

**El conflicto.** El taller tiene una segunda vía para mecanizar y respuestas que los documentos no
recogen al nivel de detalle de sus flujos.

**Qué decían los documentos.** Guion §1.6.2.2 (la herramienta no tiene papel; solo el botón
perfora), RF-28, CU-07 (alternos 3a/5a/6a) y HU-09 (FA-01..03 y datos de entrada).

**Qué hace el juego.** `AssemblySequence.Machine` atiende al botón y al mazo soltado sobre el tronco
resaltado, que vuelve a su sitio; «Mecanizar» exige un tronco corto resaltado y sin perforar y,
deshabilitado, lleva candado y «Aún no»; un tronco ya perforado responde «Ese tronco ya tiene su
agujero: es una rueda.»; soltar el tronco largo, la tabla, la caja o la cuerda lejos del lugar de
armado devuelve la pieza sin contar intento.

**Regla (c).** Solo en el juego: se añade al documento.

**Corrección aplicada (29/09/2026).** Documentos: OE2: §1.6.2.2; §2 CU-07 fila «Flujo principal»; §2
CU-07 fila «Flujos alternativos» · OE1: RF-28 · HU v2: HU-09; HU-09 fila «Botón Mecanizar»; HU-09
fila «Piezas de ensamblaje (arrastre)» · OE4: S-N2B PF-RF28-01; S-N2B PF-RF29-01. Desde el
30/09/2026, el mazo soltado lejos del tronco resaltado tampoco cuenta como intento (INC-115).

### INC-62 · El guía plantea el objetivo con preguntas o con indicaciones paso a paso e interviene siempre igual — cerrado (29/09/2026)

**El conflicto.** Los documentos piden un guía que formule el objetivo solo con preguntas, que no
intervenga salvo tras fallos y cuya intervención disminuya; el juego no lo hace así.

**Qué decían los documentos.** RF-10 («mediante diálogo formulado en preguntas»), guion §1.3
(«durante el juego no interviene salvo…»), CP-06 y HU-02 regla 1 («su intervención disminuye»),
HU-02 paso 3 y criterio 2, HU-08 paso 2 (cita «¿Qué tienen en común…?», que el juego no dice) y
HU-09 paso 2 («mediante preguntas»).

**Qué hace el juego.** El N1 abre con una pregunta, el N2 da indicaciones en imperativo («Selecciona
los objetos que se muevan con facilidad y únelos…») y el N3 descompone con preguntas. Cada mecánica
muestra la instrucción de su tarea al empezarla y la repite a demanda; la pista, siempre una
pregunta, llega al tercer fallo seguido con umbral fijo (`HintPolicy`). No hay retirada gradual.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: OE1: RF-10; §1.1 CP-06 · OE2: §1.3 §1.3.1 · HU v2:
HU-02; HU-08; HU-09 · OE4: PF-RF10-02. Se completó además: OE1: §1.1 CP-06 (el guía escribe la instrucción de cada tarea y habla en las narrativas que enlazan fases) · OE2: §2.3 CU-06 paso 1; §2 resumen de HU-02 · HU v2: HU-02 descripción y primer criterio (la lista permanente de tareas es solo del Nivel 3).

### INC-63 · La ayuda escribe la instrucción en la tablilla y no abre un cuadro que haya que cerrar (HU-03) — cerrado (29/09/2026)

**El conflicto.** HU-03 describe una ayuda que abre y cierra un cuadro de diálogo; el juego la
escribe en la tablilla sin interrumpir.

**Qué decían los documentos.** HU-03 pasos 3-5 («despliega el cuadro de diálogo… cierra el cuadro
con un clic»), regla 1 y fila «Botón Ayuda».

**Qué hace el juego.** El botón, con la figura de Algoritm y sin rótulo, llama
`HintPolicy.RequestHelp` y escribe la instrucción vigente en la tablilla con el icono de ayuda; no
hay nada que cerrar. En el N1, mientras se reúnen las piezas, además dibuja el círculo de reunión.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: HU v2: HU-03; HU-03 fila «Botón Ayuda». Se completó además: OE2: §2.3 CU-04 paso 2.

### INC-64 · Las pistas del guía del Nivel 2 no estaban en el guion — cerrado (29/09/2026)

**El conflicto.** El guion fija el texto de la pista del N1 y los demás mensajes del N2, pero no las
pistas del N2 ni su trazabilidad a RF-13.

**Qué decían los documentos.** Guion §1.6.1.2, §1.6.2.2 y §1.6.3.2 sin pista; §1.11 traza RF-13 solo
a 4.3.6.

**Qué hace el juego.** `N2_Guia.asset` define una pista por fase (Seleccionar, Construir,
Programar), que sustituye al mensaje del intento al tercer fallo seguido; un acierto reinicia la
cuenta y soltar una pieza lejos no cuenta.

**Regla (c).** Solo en el juego: se añade al documento.

**Corrección aplicada (29/09/2026).** Documentos: OE2: §1.6.2.2; §1.6.1.2; §1.6.3.2; §1.11. Se completó además: OE2: §1.8.4 (instrucciones y pistas del guía del Nivel 3); §1.11 fila de RF-13.

### INC-65 · Un fallo solo deshace lo del intento, nunca la fase activa (HU-04) — cerrado (29/09/2026)

**El conflicto.** HU-04 dice que un fallo retrocede al inicio de la fase activa o al último estado
aprobado en los niveles 2 y 3.

**Qué decían los documentos.** HU-04 regla 1, paso 4 y fila «Estado del nivel (sistema)».

**Qué hace el juego.** Ninguna mecánica reinicia la fase tras un fallo: el distractor no se acopia,
la pieza fuera de orden vuelve a su sitio, la carretilla vuelve a la salida con la secuencia intacta
y en la balsa solo vuelve lo mal puesto. La fase solo vuelve a empezar con «Reiniciar» de la pausa.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: HU v2: HU-04; HU-04 fila «Estado del nivel
(sistema)».

### INC-66 · La mecánica del Nivel 1 se ve desde arriba y el Nivel 3 es un plano fijo con perspectiva por profundidad — cerrado (29/09/2026)

**El conflicto.** La perspectiva declarada no es la de las escenas jugables.

**Qué decían los documentos.** Guion §1.1 «Perspectiva» (vista lateral en el N1; superior en el N3),
§1.8 «Escenario» («Vista superior») y `Direccion_de_Arte.md` §5.3 (Mamá en vista superior).

**Qué hace el juego.** `Level1_Cave` usa `entorno_n1_cueva_cenital` y el montón cenital;
`env_n3_rio` es un plano a ras de suelo con la cascada al fondo, con escala por profundidad (1 →
0,55) y la balsa en tres cuartos.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: OE2: §1.1 fila «Perspectiva»; §1.8 fila
«Escenario» · Dirección de arte: §5.3.

### INC-67 · HU-07 paso 5: la luz del Nivel 1 sube con cada golpe efectivo y termina con el soplido — cerrado (29/09/2026)

**El conflicto.** HU-07 paso 5 pone todo el paso de la oscuridad a la luz completa después de
soplar.

**Qué decían los documentos.** HU-07 paso 5: «El escenario transita de oscuridad total a iluminación
completa de forma gradual».

**Qué hace el juego.** La cueva parte de la penumbra con un charco de luz, sube un escalón por golpe
efectivo (`RefreshLighting`) y `PlayIgnitionAsync` barre del último escalón al máximo al soplar.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: HU v2: HU-07. Se completó además: OE1: §3.3 RF-21 · HU v2: HU-07 tercer criterio · OE2: §2 resumen de HU-07 (de la penumbra a la iluminación máxima, por golpe efectivo y al soplar).

### INC-68 · No se veía ninguna chispa al golpear en el Nivel 1 — cerrado (29/09/2026)

**El conflicto.** El guion, PG-03 (cerrado), RF-16 y la dirección de arte piden chispas visibles, y
el golpe no muestra ninguna.

**Qué decían los documentos.** Guion §1.4.3.3; PG-03 («chispas visibles que se apagan»); RF-16
(«consecuencia visible»); `Direccion_de_Arte.md` §12.2 (cuatro líneas radiales `#FFE9A8`).

**Qué hacía el juego.** `FirePanelController.Strike` da mensaje, sonido, el golpe de Papá y un
escalón de luz, pero ninguna chispa visible.

**Regla (b).** Solo en el documento: se implementa en el juego.

**Corrección aplicada (29/09/2026).** Documentos: OE4: S-N1 · Inventario de arte: FX. Tareas de
código: FI-02.

### INC-69 · Escenas narrativas: «ya vista» es haber completado el nivel, Omitir salta la escena en curso y Continuar avanza — cerrado (29/09/2026)

**El conflicto.** HU-02, HU-17 y CU-03 describen la omisión y el avance de las narrativas de otra
forma que el juego.

**Qué decían los documentos.** HU-02 FA-01/FA-02 y fila «Botón Omitir» (omitir si el nivel se
visitó, salto directo a la escena jugable «con la lista de tareas»); HU-02 paso 6 (lista de tareas
en todos los niveles, residuo de INC-41); CU-03 2a; HU-17 FA-04 y su regla 10 («para avanzarlas… el
botón de omitir»).

**Qué hace el juego.** `NarrativeVisitPolicy`: una escena cuenta como vista si el perfil ya completó
su nivel (el cierre reflexivo, si el nivel siguiente está desbloqueado); Omitir sale al
`NextSequenceId` de la escena en curso; los diálogos se avanzan con «Continuar»; solo el N3 tiene
lista de tareas.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: HU v2: HU-02 fila «Flujos Alternos»; HU-02 fila
«Botón Omitir»; HU-02 fila «Flujo Básico»; HU-17 · OE2: §2 CU-03 fila «Flujos alternativos».

### INC-70 · RF-05: las narrativas son una ilustración fija que la cámara recorre, con personajes y objetos animados — cerrado (29/09/2026)

**El conflicto.** RF-05, el guion §1.2 y la arquitectura dicen «ilustraciones estáticas»; el juego
recorre la ilustración y anima lo que hay sobre ella.

**Qué decían los documentos.** OE1 RF-05, guion §1.2 primera viñeta, arquitectura (fila
«CinematicsPlayer») y SPEC.

**Qué hace el juego.** `NarrativeSceneController` encuadra cada línea (`IllustrationFraming`), anima
personajes (`CharacterRig`) y objetos (`NarrativeProp`), muestra el retrato de quien habla y cambia
la luz (`NarrativeLight`). Sin video.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: OE1: §3.1 RF-05 · OE2: §1.2 · Arquitectura: fila
«CinematicsPlayer con VideoPlayer» · SPEC: §Arquitectura › Dos decisiones de la versión previa que contradecían requerimientos. Se completó además: OE2: §1.3.1 (la apertura queda en negro y en silencio hasta que el jugador avanza, sin un negro medido de dos segundos).

### INC-71 · El Nivel 2 dura un día y lo cuenta la luz — cerrado (29/09/2026)

**El conflicto.** El guion no marca la hora salvo «Amanece», la dirección de arte fija «mediodía» y
SPEC dice que la capa de luz es solo del N1.

**Qué decían los documentos.** Guion §1.5, §1.6.1.1, §1.6.3.1, §1.6.3.2, §1.6.4 y §1.7;
`Direccion_de_Arte.md` §8 y §8.2; SPEC §Estados.

**Qué hace el juego.** `NarrativeLight` tiñe de amanecer `N2_PuenteI`, deja sin tinte la tarde, pone
atardecer en la 2.4 y noche junto al fuego en la 2.5 y el arranque de `N3_PuenteII`; en el laberinto
lo hace `MazeLayout.LightTint`. Lo vigilan
`NarrativeSequence_RF05_ElNivel2TranscurreDelAmanecerALaNoche` y
`MazeScene_RF30_ElLaberintoEsAlAtardecer`.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: OE2: §1.6; §1.6.1.1; §1.6.3.1; §1.6.3.2; §1.6.4;
§1.7 · Dirección de arte: §8; §8.2 · SPEC: §Arquitectura › Estados.

### INC-72 · Qué abre cada nivel y dónde se retoma — cerrado (29/09/2026)

**El conflicto.** Los documentos no dicen que los puentes abren el nivel siguiente, ni que la
entrada retoma en la primera fase pendiente, ni que un perfil nuevo va directo a la apertura del N1.

**Qué decían los documentos.** Guion §1.5 y §1.7 (se leen como continuación del cierre anterior);
CU-02 paso 3 y postcondición; tabla de transiciones de la arquitectura.

**Qué hace el juego.** `LevelSelect.unity` fija `N1_Apertura`, `N2_PuenteI` y `N3_PuenteII` como
aperturas; las cadenas siguen por `NextSequenceId` hasta la fase 1; `GameFlow.TryStartPlaying`
retoma en la primera fase pendiente (RNF-14); `ProfileSelectController` lleva un perfil nuevo a
`N1_Apertura`.

**Regla (c).** Solo en el juego: se añade al documento.

**Corrección aplicada (29/09/2026).** Documentos: OE2: §1.5; §1.7; §2.3 CU-02 fila «Flujo
principal»; §2.3 CU-02 fila «Postcondiciones»; §2.3 CU-02 fila «Requerimientos asociados» ·
Arquitectura: §4.

### INC-73 · El resumen de fin de nivel: qué muestra, qué hace y a dónde lleva — cerrado (29/09/2026)

**El conflicto.** HU-14 y los CU que cierran nivel no describen la pantalla de resumen, y HU-14 da
un destino que el juego no sigue tras el Nivel 3.

**Qué decían los documentos.** HU-14 paso 6 (sin detalle), paso 7 y criterio 4 (siempre vuelve al
menú de niveles), fila «Botón Omitir (cierre)» («resumen de desempeño», residuo de INC-26); CU-05
paso 8, CU-08 paso 7 y CU-10 paso 7 y postcondición terminan en la escena de cierre.

**Qué hace el juego.** `LevelSummaryController` muestra título, hallazgo, dos frases elegidas por
los intentos y los errores corregidos de todas las fases, y la habilidad; al mostrarse confirma,
desbloquea y guarda. «Continuar» y «Volver al menú de niveles» hacen lo mismo: el menú de niveles en
el N1 y el N2; en el N3, `N3_EscenaFinal` y los créditos.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: OE2: §2.3 CU-05 fila «Flujo principal»; §2.3 CU-08
fila «Flujo principal»; §2.3 CU-10 fila «Flujo principal»; §2.3 CU-10 fila «Postcondiciones» · HU
v2: HU-14; HU-14 fila «Resumen narrativo (sistema)»; HU-14 fila «Botón Omitir (cierre)» ·
Interfaces: §1.1 · OE4: S-N3; S-N3 PF-RF45-03.

### INC-74 · La fase del Nivel 1 y el desbloqueo del nivel siguiente se guardan al mostrarse el resumen — cerrado (29/09/2026)

**El conflicto.** La arquitectura, OE1 §3.6.1 y HU-18 dicen que se guarda al completar cada fase; el
N1 y el desbloqueo se guardan después del cierre reflexivo.

**Qué decían los documentos.** Arquitectura §7 «Cuándo (RF-04)»; OE1 §3.6.1 nota 2; HU-18 FA-05;
SPEC supuesto 2; `casos.md` PF-RNF14-03/04 lo marcaban como riesgo.

**Qué hace el juego.** `FirePanelController.CompleteLevel` no confirma;
`LevelSummaryController.Show` confirma la fase del N1, desbloquea y guarda en los tres niveles. Los
niveles 2 y 3 guardan cada fase al completarla.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: Arquitectura: §7 RF-04; §4 · OE1: §3.6.1 · HU v2:
HU-18 · SPEC: Supuestos · OE4: S-DAT PF-RNF14-03; S-DAT PF-RNF14-04; S-DAT PF-RNF14-03 PF-RNF14-04. Se completó además: OE2: §4.3 viñeta LevelSummary · HU v2: HU-14 paso 6; HU-18 FA-05 · Arquitectura: §7 «Cuándo» (cierre durante la escena de cierre en los Niveles 2 y 3); §11 fila de guardado. Desde el 30/09/2026, el menú de niveles deriva además el desbloqueo de las fases confirmadas cuando un cierre impidió llegar al resumen (INC-116).

### INC-75 · Repetir un nivel conserva los indicadores de la primera vez — cerrado (29/09/2026)

**El conflicto.** HU-14 FA-01 dice que los indicadores del nuevo intento «se registran igual»; el
juego conserva los de la primera confirmación.

**Qué decían los documentos.** HU-14 FA-01; OE1 §3.6.1 nota 4 (solo habla de reiniciar).

**Qué hace el juego.** `PlayerProfile.ConfirmPhase` no reescribe una fase ya confirmada
(`PlayerProfile_RF41_UnaFaseConfirmadaNoSePierdeAlVolverAJugarla`); el resumen relata la primera
vez.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: HU v2: HU-14 fila «Flujos Alternos» · OE1: §3.6.1
· OE4: RF-06; RF-06 PF-RF45-04; Criterios del catálogo.

### INC-76 · Menú de niveles: el nivel completado no se marcaba y el bloqueado no dice qué falta — cerrado (29/09/2026)

**El conflicto.** HU-14 y HU-05 piden marcar el nivel completado (el juego no lo marca); CU-02 2a y
la arquitectura dicen que el sistema informa qué nivel falta (el juego muestra candado y «Bloqueado»
y no responde).

**Qué decían los documentos.** HU-14 paso 7 y criterio 4; HU-05 («queda marcado como completado»);
CU-02 2a; tabla de transiciones «permanece, informa cuál falta».

**Qué hacía el juego.** `LevelSelectController.Refresh` solo fija `interactable` y el candado con
«Bloqueado»; `PlayerProfile.IsLevelComplete` ya existe.

**Regla (b).** Solo en el documento: se implementa en el juego.

**Corrección aplicada (29/09/2026).** Documentos: Interfaces: §4; Tabla de inventario · OE2: §2
CU-02 fila «Flujos alternativos» · Arquitectura: fila «LevelSelect | nivel bloqueado» · OE4: S-INI;
S-INI PF-RF03-03. Tareas de código: UI-07. Se completó además: OE2: §2.3 CU-02 paso 1 (marca «Completado»).

### INC-77 · Salir no pedía confirmación ni avisaba de la ruta de respaldo — cerrado (29/09/2026)

**El conflicto.** HU-18 y la arquitectura piden confirmar la salida, informar del guardado y
advertir al docente si se guardó en la ruta de respaldo; el juego no hace nada de eso.

**Qué decían los documentos.** HU-18 pasos 3-4, FA-01, FA-04 y fila «Confirmación de salida»;
arquitectura §7 («se advierte al docente»).

**Qué hacía el juego.** `MainMenuController.Exit` guarda y cierra en el mismo clic;
`SaveStore.UsingFallback` no lo lee ninguna pantalla.

**Regla (b).** Solo en el documento: se implementa en el juego.

**Corrección aplicada (29/09/2026).** Documentos: OE4: PF-RF09-02; S-DOC; S-DOC PF-RF47-03; RF-09 ·
Interfaces: §1.1. Tareas de código: CORE-01, UI-03, UI-04, UI-06. Se completó además (30/09/2026):
con la ruta de respaldo real el aviso se montaba sobre «Quedarme» y «Cerrar el juego»; ahora la letra
se reduce hasta caber en su caja (`MainMenu_HU18_ElAvisoDeRespaldoConUnaRutaLargaNoTapaLosBotones`).

### INC-78 · Autoría: la Familia Anonaky está autorizada y se acredita, y los recursos sonoros también — cerrado (29/09/2026)

**El conflicto.** Los créditos no nombran a los autores ni la obra, varios documentos presentan como
condicional un uso ya autorizado y CT-09/RNF-23 olvidan el sonido.

**Qué decían los documentos.** HU-18 notas y TG §3.3.2 (nombrar autores y obra); OE1 §2.4, TG
(Resumen, Abstract, Introducción, §3.3.2, §5.2) y HU-18 (uso condicional); OE1 CT-09 y RNF-23
(«recursos gráficos»); TG sin anexo de la autorización (INC-43 dice archivarla allí).

**Qué hacía el juego.** `CreditsContent.asset`: «Personajes basados en diseños de la Familia
Anonaky, usados con autorización escrita.», sin autores ni obra; 19 piezas de sonido propias
acreditadas en «Música y sonido»; PG-07 cerrado el 30/08/2026.

**Regla (b).** Solo en el documento: se implementa en el juego.

**Corrección aplicada (29/09/2026).** Documentos: OE4: S-INI · OE1: §2.4; §1.2 CT-09; §4.6 RNF-23 ·
Trabajo de grado: Resumen; Abstract; Introducción; §3.3.2; §5.2 (LISTA DE ANEXOS y ANEXOS, final del documento: **pendientes** —falta el Anexo C con el
escaneo de la autorización—) · HU v2: HU-18 · Dirección de sonido: §17; §18 S-05. Tareas de código: DATA-01.
Se completó además (30/09/2026): el texto de créditos quedó en tres oraciones de 16, 10 y 8 palabras
(RNF-01) —«Personajes basados en la Familia Anonaky, de Bibiana Patricia Rey Barrote y Luis Eduardo
Benavides Porras. Vienen del libro Tecnología para niños: libro de actividades (2019). Se usan con
autorización escrita de sus autores.»— y el rol de acompañamiento escribe «asesoría»
(`CreditsContent_RNF01_NingunaOracionSupera20Palabras`) · OE4: S-INI paso 4.

### INC-79 · Tipografía: sin Fredoka, el informe docente en Baloo 2 y Nunito, y los tamaños reales — cerrado (29/09/2026)

**El conflicto.** La dirección de arte e Interfaces fijan Fredoka y unos tamaños que el juego no
tiene, y el informe docente usa la fuente integrada del motor.

**Qué decían los documentos.** `Direccion_de_Arte.md` §11.2-§11.3 (Fredoka para cifras; contadores
40 px; diálogo 34 px; mínimo absoluto 26 px); `Interfaces.md` §2; créditos que acreditan Fredoka.

**Qué hace el juego.** Solo Baloo 2 y Nunito en `Art/Fonts/`; contador del bosque en Baloo 2 a 30
px; diálogo a 26 px y rótulos secundarios a 22-24 px; `TeacherReport.unity` (20 textos) y la
etiqueta «Progreso del equipo» con la fuente integrada, 12 a 22 px.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: Dirección de arte: §11.2; §11.3; §11.3 fila
«Diálogo» · Interfaces: §11 · OE4: S-INI · registros de tareas: §8. Tareas de código: UI-02,
DATA-01.

### INC-80 · Entrada: solo clic y clic sostenido, con una única excepción y el mapa de controles en todas las escenas — cerrado (29/09/2026)

**El conflicto.** CT-06 y RNF-02 no admiten la escritura del nombre y cuentan tres escenas jugables;
HU-18 pide recorrer los créditos solo con clic; `TeacherReport` escapaba al mapa de controles; SPEC
nombraba otro mapa.

**Qué decían los documentos.** OE1 CT-06, RNF-02 (requerimiento y criterio); HU-18 criterio 6 y
FA-02; SPEC §Stack y árbol de carpetas.

**Qué hace el juego.** El nombre del perfil se escribe en un `InputField`; hay cinco escenas
jugables; los créditos y el informe se desplazan con `ScrollRect` (barra y arrastre);
`TeacherReport.unity` incrusta un `DefaultInputActions` con rueda, teclado y mando que
`InputSchemeTest` no ve; el mapa del proyecto es `Assets/Game/Input/ControlesJugables.inputactions`.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: HU v2: HU-18 · registros de tareas: §8 · OE1: §1.2
CT-06; §4.1 RNF-02 · SPEC: fila «Entrada»; §Estructura del proyecto. Tareas de código: UI-01. Se completó además: Trabajo de grado: §5.2 (Unity Test Framework) · Arquitectura: §1 fila «Entrada»; §6 fila «InputHandler con gamepad»; §11 fila de entrada; §12 cambio 9.

### INC-81 · El menú principal tiene cuatro opciones: Jugar, Créditos, Progreso del equipo y Salir — cerrado (29/09/2026)

**El conflicto.** RF-01, HU-01, HU-18, la arquitectura e `Interfaces.md` enumeran tres opciones, y
RF-46 exige una cuarta.

**Qué decían los documentos.** RF-01; HU-01 paso 1 y criterio 4; HU-16 paso 1 y CU-11 paso 1
(«opción de Progreso»); HU-18 paso 1 y criterio 1; arquitectura (enum y transición «Progreso»);
`Interfaces.md` §1.1-2.

**Qué hace el juego.** `MainMenu.unity`: «Jugar», «Créditos», «Progreso del equipo» y «Salir»
(`MainMenuTests.cs:22`).

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: Interfaces: §1; §1.1 · OE1: §3.1 RF-01 · HU v2:
HU-01; HU-16; HU-18 · OE2: §2 CU-11 · Arquitectura: §4. Tareas de código: UI-09.

### INC-82 · Cinco rótulos de trabajo «… · placeholder» visibles al estudiante — cerrado (29/09/2026)

**El conflicto.** RNF-01 prohíbe tecnicismos no explicados, y cinco textos de trabajo en inglés
están a la vista.

**Qué decían los documentos.** OE1 RNF-01; `casos.md` PF-RF01-02 lo anotaba como riesgo.

**Qué hacía el juego.** `MainMenu.unity:5066`, `Credits.unity:267` y
`LevelSelect.unity:481/1820/3189` muestran «… · placeholder» sobre un `ui_panel` vacío.

**Regla (b).** Solo en el documento: se implementa en el juego.

**Corrección aplicada (29/09/2026).** Documentos: OE4: S-INI; PF-RF01-02. Tareas de código: UI-05.
Se completó además (30/09/2026): la tarjeta del Nivel 2 muestra `entorno_n2_laberinto`, de 16:9, en
lugar del bosque de 3840 px, que se veía repetido y partido por la costura
(`LevelSelect_RF03_CadaTarjetaMuestraUnaIlustracionSinCostura`) · OE4: S-INI paso 13.

### INC-83 · Creación del perfil: validación del nombre y ejecución sin instalación — cerrado (29/09/2026)

**El conflicto.** HU-01 y CU-01 no recogen todas las validaciones del nombre, y la precondición de
CU-01 dice «instalado».

**Qué decían los documentos.** HU-01 paso 5, FA-01/FA-02, criterio 3 y fila «Nombre / Alias»; CU-01
paso 5, 4a y precondición («instalado y ejecutable»).

**Qué hace el juego.** `PlayerProfile` recorta espacios, rechaza caracteres no válidos para un
nombre de archivo con su mensaje, compara duplicados sin distinguir mayúsculas y el campo admite 24
caracteres; el juego corre desde su carpeta portable.

**Regla (c).** Solo en el juego: se añade al documento.

**Corrección aplicada (29/09/2026).** Documentos: HU v2: HU-01; HU-01 fila «Nombre / Alias» · OE2:
§2.3 CU-01 fila «Flujo principal»; §2.3 CU-01 fila «Flujos alternativos»; §2.3 CU-01 fila
«Precondiciones». Se completó además: HU v2: HU-01 pasos 3 y 4 («Perfil nuevo», «Continuar»).

### INC-84 · Actores: CU-01 y el resumen de HU-18 conservaban los actores anteriores a INC-31 — cerrado (29/09/2026)

**El conflicto.** La corrección de INC-31 no llegó a CU-01 ni al resumen de HU-18 en OE2.

**Qué decían los documentos.** CU-01 «Actor principal | Docente (con acompañamiento del
estudiante)»; OE2 tabla de historias, HU-18 «Como docente…».

**Qué hace el juego.** El panel de perfil se dirige al estudiante («¿Quién juega?», «Escribe tu
nombre»); la pantalla de inicio no distingue roles.

**Regla (crossDoc).** Entre documentos: se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: OE2: §2.3 CU-01 fila «Actor principal»; HU-18. Se completó además: OE2: §2.1 fila «Docente»; §2.2 fila CU-01.

### INC-85 · El perfil también se borra desde el panel «¿Quién juega?» — cerrado (29/09/2026)

**El conflicto.** CU-12, HU-16, HU-01, la arquitectura e `Interfaces.md` solo prevén borrar desde la
pantalla de progreso.

**Qué decían los documentos.** CU-12 paso 1 y 4a; HU-16 FA-01/FA-02 y criterio final; HU-01 paso 3;
arquitectura (enum ProfileSelect); `Interfaces.md` fila 3.

**Qué hace el juego.** Cada perfil de la lista lleva una papelera que pide confirmación («¿Borras el
perfil de…?», Conservar / Borrar) y borra con `ProfileSession.Delete`, que además desactiva el
perfil si era el activo.

**Regla (c).** Solo en el juego: se añade al documento.

**Corrección aplicada (29/09/2026).** Documentos: Interfaces: §1; § «3 · Selección de perfil» · OE2:
§2 CU-12 fila «Flujos alternativos»; §2 CU-12 fila «Postcondiciones» · HU v2: HU-16; HU-01 ·
Arquitectura: §4.

### INC-86 · Informe docente: indicadores del perfil elegido, sin agregados, y pantalla sin perfiles — cerrado (29/09/2026)

**El conflicto.** RF-46 pide un «resumen acumulado», y CU-11 y HU-16 describen otra entrada, otra
selección y otra salida sin perfiles.

**Qué decían los documentos.** RF-46; CU-11 precondición, pasos 2-4 y 2a; HU-16 FA-02.

**Qué hace el juego.** `TeacherReportController` lista los perfiles y presenta los del primero; una
fila por fase con los cuatro indicadores, «Sin datos» en lo no jugado y tiempo en m:ss; sin totales
ni promedios (`IndicatorReport`); sin perfiles muestra un aviso y se sale con «Volver al menú».

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: OE1: §3 RF-46 · OE2: §2 CU-11 · HU v2: HU-16. Se completó además: HU v2: HU-16 pasos 2 y 3.

### INC-87 · Qué datos guarda el prototipo y qué se puede hacer con ellos — cerrado (29/09/2026)

**El conflicto.** HU-16, OE1 §2.3 y el trabajo de grado §3.3.1 describen datos, facultades y medidas
que el juego no tiene.

**Qué decían los documentos.** HU-16 regla 1 (sin el progreso de avance, residuo de INC-27); OE1
§2.3 («reiniciar o eliminar datos», «registro de eventos»); TG §3.3.1 («confidencialidad e
integridad»).

**Qué hace el juego.** `PlayerProfile` guarda nombre, nivel alcanzado, fases confirmadas y cuatro
indicadores por fase, en JSON sin cifrar; solo existe el borrado definitivo; no hay registro de
eventos propio; el informe se abre sin clave.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: HU v2: HU-16 · OE1: §2.3 · Trabajo de grado:
§3.3.1.

### INC-88 · Nivel 3: la recolección no es una fase y el ensamblaje ocupa las tres — cerrado (29/09/2026)

**El conflicto.** CU-09/CU-10, HU-11/HU-12 y la arquitectura numeran la recolección como fase 1; el
guion §1.8.3, RF-40 y el juego tienen tres fases de ensamblaje, y OE1 §3.6.1 no dice cómo se
reparten los indicadores.

**Qué decían los documentos.** Títulos de CU-09 y CU-10, HU-11/HU-12 («fase 1», «fase 2»),
arquitectura §4 y §6 («dos fases»); OE1 §3.6.1 («por cada fase»).

**Qué hace el juego.** `PhaseId.PhasesPerLevel = {1,3,3}`; la recolección no se persiste; al retomar
la fase 2 o 3 se abre directamente el ensamblaje; `RiverIndicatorCollector` arranca al abrir el
panel y acumula los conteos entre fases.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: OE2: §2.3 CU-09; §2.3 CU-10; §2.3 CU-10 fila
«Precondiciones»; §2 CU-09 fila «Postcondiciones» · HU v2: HU-11 fila «Descripción de Historia de
usuario»; HU-11 fila «Reglas de negocio»; HU-12 fila «Descripción de Historia de usuario»; HU-12
fila «Flujos Alternos» · Arquitectura: §4; §6 · OE1: §3.6.1. Se completó además: OE2: §1.8.2 (título y su entrada en el índice).

### INC-89 · Nivel 3: ocho materiales en cuatro casillas y una balsa de diecisiete espacios — cerrado (29/09/2026)

**El conflicto.** Los documentos hablan de «cuatro objetos» y de «los cuatro materiales».

**Qué decían los documentos.** RF-38; guion §1.8.2 («Materiales», «Inventario») y §1.8.3; CU-09 y
CU-10; HU-11 y HU-12; resumen de HU-11 en OE2; `Direccion_de_Arte.md` §9.3 y §12.3;
`Direccion_de_Musica_y_Sonido.md`.

**Qué hace el juego.** Ocho recogibles (cinco troncos, sogas, tela, mástil) en cuatro casillas, la
de troncos con cinco marcas; 17 espacios en la balsa (5 troncos, 10 amarres, mástil y vela), un
rollo que da diez amarres; al fallar la prueba la balsa gira −14° y se hunde un poco durante 0,6 s,
sin salpicadura.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: OE1: §3.5 RF-38 · OE2: §1.8.2 fila «Materiales»;
§1.8.2 fila «Inventario»; §1.8.3 fila «Fase 1 — Base»; §1.8.3 fila «Fase 2 — Amarre»; §2 CU-09 fila
«Flujo principal»; §2 CU-09 fila «Postcondiciones»; §2 CU-10 fila «Flujo principal»; §2 HU-11 · HU
v2: HU-11 fila «Descripción de Historia de usuario»; HU-11 fila «Flujo Básico»; HU-11 fila «Flujos
Alternos»; HU-11 fila «Criterios de Aceptación»; HU-11 fila «Inventario (sistema)»; HU-12 fila
«Flujo Básico»; HU-12 fila «Pieza de construcción (arrastre)» · Dirección de sonido: Inventario de
piezas del Nivel 3 · Dirección de arte: §9.3; §12.3 fila «Prueba de balsa sin éxito (Nivel 3)».

### INC-90 · Ensamblaje de la balsa: colocar no valida, el botón siempre está habilitado y se rotula «Listo» o «Probar balsa» — cerrado (29/09/2026)

**El conflicto.** HU-12 valida al arrastrar, deshabilita el botón ante una colocación incorrecta y
nombra «Confirmar fase»; el guion, CU-10 y HU-12 no recogen retirar una pieza ni soltarla fuera.

**Qué decían los documentos.** HU-12 FA-01, regla 2, pasos 3-7 y fila «Botón Confirmar fase»; guion
§1.8.4; CU-10 2a.

**Qué hace el juego.** `RaftAssembly.Place` no valida; el botón está habilitado aunque la fase esté
incompleta y se rotula «Listo» en base y amarre y «Probar balsa» en mástil y vela; al pulsarlo se
marcan vacíos y mal puestos y vuelve solo lo mal puesto; una pieza de la fase abierta se retira con
clic sostenido; soltar fuera la devuelve al inventario y sobre un espacio ocupado no se coloca.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: HU v2: HU-12; HU-12 fila «Botón Confirmar fase» ·
OE4: Sesión del Nivel 3; PF-RF40-02 · OE2: §1.8.4; §2.3 CU-10 fila «Flujos alternativos».

### INC-91 · Controles del Nivel 3: cruceta abajo a la derecha, con clic sostenido — cerrado (29/09/2026)

**El conflicto.** RF-35 pone los botones a los dos costados y el esquema de controles, HU-11 y SPEC
dicen solo «clic».

**Qué decían los documentos.** RF-35; guion §1.2.1 y §1.8.2 «Movimiento»; HU-11 fila «Botones de
dirección»; SPEC.

**Qué hace el juego.** Cuatro `Flecha_*` en cruceta anclada abajo a la derecha; `DirectionPad`
avanza mientras se sostiene el clic.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: OE1: §3.5 RF-35 · OE4: PF-RF35-01 · OE2: §1.2.1
fila «Botones en pantalla de cambio de dirección»; §1.8.2 fila «Movimiento» · HU v2: HU-11 fila
«Botones de dirección» · SPEC: §Arquitectura; fila «INC-01 · controles»; Supuestos.

### INC-92 · `Interfaces.md` tenía el inventario de pantallas desfasado — cerrado (29/09/2026)

**El conflicto.** El inventario da por pendientes pantallas implementadas y describe otras
transiciones y otro Nivel 3.

**Qué decían los documentos.** `Interfaces.md` §1 (filas 11-17 y párrafo inicial), §1.1 (11-12, 13)
y §2 (marco de inventario).

**Qué hace el juego.** `LevelSummary.unity` y `TeacherReport.unity` existen, con
`EraseConfirmationDialog`; la recolección y el ensamblaje del N3 están en `Level3_River`; el fundido
a negro de `SceneLoader` existe y la cortinilla y el barrido no; un asset de resumen por nivel;
inventario en panel de arena con casillas cuadradas.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: Interfaces: §1; §1.1; §2 fila «Marco de
inventario» · Dirección de arte: §10.2 fila «Marco de inventario».

### INC-93 · Narrativas del Nivel 3: la escena 3.2 tras el primer fallo y el cruce como cierre reflexivo — cerrado (29/09/2026)

**El conflicto.** HU-13, CU-10, CU-03 y la arquitectura no recogen la escena 3.2 ni la vuelta a la
fase; la trazabilidad §1.11 trata el cruce como transición sin faceta.

**Qué decían los documentos.** HU-13 paso 7 y FA-01; CU-10 6a; CU-03 precondición; arquitectura
(transiciones y «No existe transición a derrota»); guion §1.11 fila 8.5.

**Qué hace el juego.** `ConditionalNarrativeTrigger` lleva al primer fallo de la prueba a
`N3_Escena32_PrimerIntento`, que vuelve a la fase 3 con lo bien puesto; `N3_Escena33_Cruce` es el
cierre reflexivo (`IsReflectiveClosing`) y sale al resumen.

**Regla (c).** Solo en el juego: se añade al documento.

**Corrección aplicada (29/09/2026).** Documentos: HU v2: HU-13 fila «Flujo Básico»; HU-13 fila
«Flujos Alternos» · OE2: §2.3 CU-10 fila «Flujos alternativos»; §2.3 CU-03 fila «Precondiciones»;
§1.11 fila «8.5» · Arquitectura: §4. Se completó además: OE1: §3.2 RF-11 (la escena 3.2 es la única acción que interrumpe la fase) · Arquitectura: §4 «No existe transición a derrota» · HU v2: HU-13 paso 7 (la escena 3.2 vuelve a aparecer en cada partida: no se guarda en el perfil).

### INC-94 · Arquitectura §6: los componentes con los nombres del prototipo — cerrado (29/09/2026)

**El conflicto.** La tabla de capas nombra componentes que no existen y describe mal otros.

**Qué decían los documentos.** Arquitectura §6, §8, §9, §10, §12 y SPEC «Módulos por capa»:
`GameBootstrap`, `GuideController`, `FeedbackLog` en el andamiaje,
`FireLevel`/`WheelLevel`/`RiverLevel`, `ProgressTracker`, `HUDController`, `InputHandler`;
`PlayerProfile` «con un solo dato»; `SceneLoader` y `AudioManager` sin su comportamiento.

**Qué hace el juego.** `GameFlowRunner` en Boot; `GuideContent` + `HintPolicy`; `FireFeedbackLog` en
el N1; un controlador por escena jugable; `IndicatorReport`/`ProfileRepository`; controladores de
pantalla; eventos de puntero sobre el Input System; `PlayerProfile` con la lista cerrada de RNF-09;
fundido de `SceneLoader`; cuatro buses de `AudioManager`.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: Arquitectura: §6; §8 fila «Referencia de
Inspector»; §9; §6 fila «Core | SceneLoader»; §6 fila «Audio | AudioManager»; §6 fila «Core |
PlayerProfile»; §10 · SPEC: §Arquitectura › Módulos por capa; §Arquitectura › Dos decisiones de la
versión previa que contradecían requerimientos; §Estructura del proyecto. Se completó además: Arquitectura: párrafo inicial (errata «F5SM»).

### INC-95 · Arquitectura §4, §5 y §8: estados, transiciones y comunicación como los implementa el juego — cerrado (29/09/2026)

**El conflicto.** La arquitectura y SPEC describen el estado Narrative, el código de GameFlow, las
transiciones y la comunicación de otra forma.

**Qué decían los documentos.** `Narrative` recibe el `NarrativeSequence`; fragmento con
`IProgressStore`/`TryEnterLevel`; tabla de transiciones incompleta; evento `PhaseConfirmed` e
`ILevelReporter` «que Core le pasa al arrancar».

**Qué hace el juego.** `Narrative` lleva el id de la secuencia; `GameFlow` valida contra una tabla y
usa `TrySelectProfile`/`TryStartNarrative`/`TryStartPlaying`; hay retornos al menú, encadenamiento
de narrativas, retoma y salida de respaldo; no hay evento de fase: se confirma y se guarda en la
misma llamada; el nivel se registra como `ILevelReporter` para la pausa; una escena del guion puede
ocupar varios assets.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: Arquitectura: §4 CT-05; §4; §5; §4 fila
«LevelSelect | nivel desbloqueado»; §8 fila «Interfaz inyectada»; §8 fila «Evento»; §8 · SPEC:
§Arquitectura.

### INC-96 · Assemblies, carpetas y construcción por slices — cerrado (29/09/2026)

**El conflicto.** La arquitectura y SPEC describen otro grafo de assemblies, otra estructura de
carpetas y otra organización del trabajo.

**Qué decían los documentos.** Arquitectura §6 («Cada capa depende solo de las inferiores»), §9
(assemblies, árbol), §10 (diez épicas), §11; SPEC mapa de capacidades (progreso-registro depende de
los niveles), árbol y assemblies de pruebas.

**Qué hace el juego.** `Game.Reporting` y `Game.Audio` solo dependen de `Game.Core`; los niveles
usan `Game.Audio`; `Game.EditorTools` aplica las reglas de importación; `LevelSummary.unity`,
`Input/`, `Prefabs/` y `Scripts/Editor/` existen; el trabajo se hizo en cuatro slices más dos
carriles.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: Arquitectura: §6; §9; §11 fila «Un assembly por
nivel, sin referencias cruzadas»; §10 · SPEC: §Mapa de capacidades; §Estructura del proyecto.

### INC-97 · Identidad y configuración del ejecutable (decisiones D2 y D3) — cerrado (29/09/2026)

**El conflicto.** El ejecutable se presenta como la plantilla de Unity, envía estadísticas, muestra
la pantalla de Unity y admite Alt+Intro.

**Qué decían los documentos.** RF-01 (título), RNF-08/RNF-10 (sin transmisión), CT-06/RNF-02 (sin
combinaciones de teclas); OE3 §3.6 («sin tocar el registro del sistema»); CLAUDE.md y SPEC sobre
`productName`.

**Qué hacía el juego.** `ProjectSettings.asset`: `productName: My project`, `companyName:
DefaultCompany`, `submitAnalytics: 1`, `m_ShowUnitySplashScreen: 1`, `allowFullscreenSwitch: 1`,
`SENTIS_ANALYTICS_ENABLED`; el reproductor guarda `Player.log` en LocalLow y sus preferencias de
pantalla en el registro.

**Regla (b).** Solo en el documento: se implementa en el juego.

**Corrección aplicada (29/09/2026).** Documentos: CLAUDE.md: Comandos · SPEC: §Supuestos · OE4: §2;
Tabla de procedimientos; S-DOC PF-RNF11-01; S-DOC; §12 · Arquitectura: §11 fila «Sin módulos de
red»; §2 · registros de tareas: Checkpoint B. Tareas de código: PS-01. El define
`SENTIS_ANALYTICS_ENABLED` se retiró de `ProjectSettings`, pero la prueba no exige su ausencia: `com.unity.ai.inference` puede reponerlo en cada recarga
mientras la analítica del Editor esté activa (`AnalyticsDefineManager`), y solo compila código de
Editor del paquete —su `Runtime/` no lo usa—, así que no llega al ejecutable. Quitarlo de verdad
exige retirar el paquete del manifiesto, que D3 excluye. Arquitectura §2 lo dice así: la analítica que algunos paquetes activan en el Editor no entra en el ejecutable.

### INC-98 · CT-04 y RNF-16: cada nivel en sus escenas y su módulo de código — cerrado (29/09/2026)

**El conflicto.** CT-04, RNF-16 y la arquitectura dicen «cada nivel como escena independiente».

**Qué decían los documentos.** OE1 CT-04 y RNF-16; arquitectura §1.

**Qué hace el juego.** El Nivel 2 son tres escenas; las narrativas comparten `Narrative.unity`; la
independencia la da un assembly por nivel sin referencias cruzadas (`Architecture_RNF16_*`).

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: OE1: §1.2 CT-04; §4.5 RNF-16 · Arquitectura: §1
fila «RNF-16, CT-04».

### INC-99 · RNF-03 no enunciaba la presentación en pantalla que verifican sus pruebas — cerrado (29/09/2026)

**El conflicto.** Dieciocho pruebas trazadas a RNF-03 comprueban que nada se sale ni se solapa a
pantalla completa, y RNF-03 solo habla de la tarea activa.

**Qué decían los documentos.** OE1 RNF-03 (requerimiento y criterio).

**Qué hace el juego.** Pantalla completa a la resolución nativa, interfaz que escala, nada fuera ni
bajo el cuadro de diálogo (pruebas `*_RNF03_*`).

**Regla (c).** Solo en el juego: se añade al documento.

**Corrección aplicada (29/09/2026).** Documentos: OE1: §4.1 RNF-03 · OE4: PF-RNF03-01.

### INC-100 · Matrices de trazabilidad y referencias internas de OE1 y OE2 — cerrado (29/09/2026)

**El conflicto.** Las matrices omiten RF y facetas que el juego ejercita, y varias referencias
internas apuntan a capítulos equivocados.

**Qué decían los documentos.** OE1 §2.2 («capítulo 4»), §5 (anuncia casos de uso que no tiene) y
§5.1; OE2 §3.1 (sin RF-20, 21, 25, 28, 35, 37-39, 44; sin algoritmia en el taller ni iteración en el
laberinto; Generalización sin CU-05), §3.2 (CP-01 → «§4»), §3.6 (prioridad en «§1.3»; A, B y F solo
en 3.2); CU-07 y CU-08 «Habilidad de PC».

**Qué hace el juego.** El taller es un armado en orden (pensamiento algorítmico), el laberinto se
reejecuta sin límite (iteración) y el N1 cierra nombrando la habilidad como los otros dos.

**Regla (crossDoc).** Entre documentos: se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: OE1: §2.2; §5; §5.1; §5.1 fila «Pensamiento
algorítmico»; §5.1 fila «Iteración» · OE2: §3.1; §3.2 CP-01; §3.6 fila «Cumplimiento de
requerimientos (OE4)»; §3.6 fila «Validación de requerimientos (OE1)»; §3.1 fila «Pensamiento
algorítmico»; §3.1 fila «Iteración»; §2.3 CU-07 fila «Habilidad de PC asociada»; §2.3 CU-08 fila
«Habilidad de PC asociada»; §3.1 fila «Generalización». Se completó además: OE2: §4.5 (el contenido en datos cita RNF-18, no RNF-06).

### INC-101 · El control de cambios de HU v2 no registraba las ediciones del 30/08/2026 — cerrado (29/09/2026)

**El conflicto.** Doce historias cambiaron el 30/08/2026 y su tabla de control de cambios sigue en
1.0.

**Qué decían los documentos.** Tablas «Control de Cambios» y cabeceras «Version: 1.0» de HU v2.

**Qué hace el juego.** git 938fefc (30/08/2026) modificó HU-01, 02, 06, 07, 10, 11, 12, 13, 14, 16,
17 y 18.

**Regla (crossDoc).** Entre documentos: se corrige el documento.

**Corrección aplicada (29/09/2026).** Sin ediciones de documento.

### INC-102 · SPEC e INCONSISTENCIAS daban el trabajo de grado por fuera de `docs/` — cerrado (29/09/2026)

**El conflicto.** Los dos documentos del proyecto dicen que el trabajo de grado salió de `docs/`.

**Qué decían los documentos.** SPEC encabezado y tabla de precedencia; INCONSISTENCIAS encabezado y
tabla de precedencia.

**Qué hace el juego.** `docs/Trabajo_de_Grado_2026_ICONTEC_IEEE (2).docx` está versionado (938fefc)
y tiene su conversión en `docs/md/`.

**Regla (crossDoc).** Entre documentos: se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: SPEC: Encabezado (líneas 9-10); Tabla de
precedencia.

### INC-103 · SPEC: la estrategia de pruebas no reflejaba las categorías ni las pruebas reales — cerrado (29/09/2026)

**El conflicto.** SPEC no recoge la categoría Acceptance, dice que el contraste no admite aserción
estricta y cuenta tres CP con prueba automatizada.

**Qué decían los documentos.** SPEC §Estrategia de pruebas (fila «Verificación visual») y §Criterios
de éxito punto 2.

**Qué hace el juego.** 81 pruebas `Acceptance`; el contraste de los tres niveles se asevera ≥ 4,5:1;
CP-07 tiene dos pruebas.

**Regla (c).** Solo en el juego: se añade al documento.

**Corrección aplicada (29/09/2026).** Documentos: SPEC: fila «Verificación visual»; §Criterios de
éxito. Se completó además: OE1: §2.3 fila «Equipo evaluador» · Trabajo de grado: §5.2 (cada prueba nombra lo que verifica: requerimiento, criterio, caso de uso, historia o inconsistencia).

### INC-104 · Quedaba texto visible escrito en C# — cerrado (29/09/2026)

**El conflicto.** CT-05, RNF-18 y SPEC exigen todo texto visible en ScriptableObjects, y nueve
cadenas seguían en el código.

**Qué decían los documentos.** SPEC (contenido fuera del código); OE1 CT-05 y RNF-18.

**Qué hacía el juego.** `ProfileSelectController.cs` (cinco mensajes),
`EraseConfirmationDialog.cs:54` y el diccionario `Labels` de `MazeSceneController.cs`.

**Regla (b).** Solo en el documento: se implementa en el juego.

**Corrección aplicada (29/09/2026).** Sin ediciones de documento. Tareas de código: UI-08, WH-02.

### INC-105 · Colores de estado: el juego usa una paleta de estado propia, sin rojo de error — cerrado (29/09/2026)

**El conflicto.** `Interfaces.md` §3.3 y `Direccion_de_Arte.md` §12.3 solo admiten verde `#5FA842` y
ámbar `#E8A33D`.

**Qué decían los documentos.** `Interfaces.md` §3 regla 3; `Direccion_de_Arte.md` §12.3.

**Qué hace el juego.** Aceptado `#336638`, devuelto `#995C1A`, ayuda `#3D4C70`, pendiente `#6B5247`,
espacio equivocado de la balsa `#D96B29`, ámbar en el bloque elegido y el refugio; asa de fuerza del
N1 de azul a rojo como intensidad.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: Interfaces: §3 · Dirección de arte: §12.3.

### INC-106 · Área táctil: algunos controles miden menos de 88×88 px — cerrado (29/09/2026)

**El conflicto.** Los documentos exigen 88×88 px en todo elemento interactivo.

**Qué decían los documentos.** `Interfaces.md` §3 regla 4; `Direccion_de_Arte.md` §10.1 y §17.

**Qué hace el juego.** «Continuar» 260×76 y «Omitir» 200×76; en el laberinto «−»/«+» 64×64, flecha
52×52, lados de «Girar» 82×64 y flechas de la lista 56×56.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: Interfaces: §3 · Dirección de arte: §10.1; §17.

### INC-107 · Mínima permanencia: el HUD fijo es más que la pausa — cerrado (29/09/2026)

**El conflicto.** Los documentos dicen que solo permanece la pausa (y la lista del N3) y que el N1 y
el N2 no llevan indicador de progreso.

**Qué decían los documentos.** `Interfaces.md` §1.1-6 y §3 regla 6; `Direccion_de_Arte.md` §10.1.

**Qué hace el juego.** Pausa, pista y tablilla fijas en las cinco escenas; contador permanente del
bosque (RF-24); lista e inventario del N3; la cruceta solo durante la recolección.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: Interfaces: §1.1; §3 · Dirección de arte: §10.1.

### INC-108 · Cara de los personajes: una sola expresión y la emoción en el cuerpo — cerrado (29/09/2026)

**El conflicto.** Los documentos piden seis expresiones y una sonrisa con dientes.

**Qué decían los documentos.** `Interfaces.md` §4 (ROSTRO), §4.2 y §5; `Direccion_de_Arte.md` §7.3 y
§12.3; `Inventario.md` (expresiones pendientes).

**Qué hace el juego.** Un retrato `neutra` por personaje, sonrisa cerrada sin dientes; la emoción la
llevan las acciones del rig (`Celebrate`, `Surprise`, `Observe`, `Encourage`…).

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: Interfaces: §4.2; §4; §5 · Dirección de arte:
§7.3; §12.3 fila «Secuencia incorrecta» · Inventario de arte: Personajes.

### INC-109 · Los personajes no tenían sombra de contacto — cerrado (29/09/2026)

**El conflicto.** La dirección de arte e Interfaces fijan una elipse hija del personaje al 25 %, y
ningún prefab la tiene.

**Qué decían los documentos.** `Direccion_de_Arte.md` §5.3; `Interfaces.md` §4.

**Qué hacía el juego.** Los prefabs de la familia solo tienen Lienzo/Cuerpo/partes; no hay sprite de
sombra.

**Regla (b).** Solo en el documento: se implementa en el juego.

**Corrección aplicada (29/09/2026).** Sin ediciones de documento. Tareas de código: SC-02.
Se completó además (30/09/2026): se borró `CharacterRig.Tint`, sin llamadores, que ofrecía teñir al
personaje con la luz del laberinto y habría pisado el 25 % de alfa de la sombra.

### INC-110 · El hallazgo del resumen del Nivel 3 superaba las veinte palabras (RNF-01) — cerrado (29/09/2026)

**El conflicto.** RNF-01 fija oraciones de veinte palabras como máximo, y una línea tiene 23; su
prueba cortaba en «:».

**Qué decían los documentos.** OE1 RNF-01.

**Qué hacía el juego.** `N3_ResumenNivel.asset` Discovery; `LevelSummaryComposerTests.cs:274`.

**Regla (b).** Solo en el documento: se implementa en el juego.

**Corrección aplicada (29/09/2026).** Sin ediciones de documento. Tareas de código: DATA-02.

### INC-111 · El trabajo de grado prometía integrar la resolución de problemas matemáticos — cerrado (29/09/2026)

**El conflicto.** La justificación afirma un contenido matemático que el producto no tiene.

**Qué decían los documentos.** TG §1.4, último párrafo.

**Qué hace el juego.** Ningún reto matemático en `Assets/Game/Data`; el juego evita cifras (CP-03).

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: Trabajo de grado: §1.4.

### INC-112 · El trabajo de grado describía el patrón de unidades didácticas con Wix y para 7-8 años — cerrado (29/09/2026)

**El conflicto.** §3.2.4 atribuye al proyecto otra plataforma y otra edad.

**Qué decían los documentos.** TG §3.2.4, tercera viñeta.

**Qué hace el juego.** Ejecutable de Unity sin red para 9-11 años; acceso secuencial por desbloqueo
de niveles y avance con un clic.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: Trabajo de grado: §3.2.4.

### INC-113 · El trabajo de grado no recogía herramientas ni actividades del desarrollo — cerrado (29/09/2026)

**El conflicto.** §5.2 y §5.1.3 omiten herramientas y bloques de trabajo que el proyecto usó.

**Qué decían los documentos.** TG §5.2 (instrumentos) y §5.1.3 (actividades de la Fase 3).

**Qué hace el juego.** Unity Test Framework, Input System, Git/GitHub, Claude; sonido, perfil con
informe docente y borrado, ejecutable portable medido.

**Regla (c).** Solo en el juego: se añade al documento.

**Corrección aplicada (29/09/2026).** Documentos: Trabajo de grado: §5.2; §5.1.3.

### INC-114 · El índice y la lista de figuras del trabajo de grado estaban desactualizados — cerrado (29/09/2026)

**El conflicto.** La tabla de contenido y la lista de figuras no corresponden al cuerpo.

**Qué decían los documentos.** TG, CONTENIDO y LISTA DE FIGURAS (PRISMA, numeración de §3.2, «5.
METODOLOGÍA»); §3.1.5 sin estilo de título.

**Qué hace el juego.** (Estructura interna del documento.)

**Regla (crossDoc).** Entre documentos: se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: Trabajo de grado: §3.1.5; CONTENIDO (tabla de
contenido) y LISTA DE FIGURAS.

### INC-115 · Soltar el mazo lejos del tronco resaltado contaba como intento — cerrado (30/09/2026)

**El conflicto.** En el taller, soltar el tronco largo, la tabla, la caja o la cuerda lejos del
lugar de armado no cuenta como intento, y soltar el mazo lejos del tronco resaltado sí contaba: el
mismo fallo de puntería, que no es un razonamiento fallido, medía distinto según la pieza.

**Qué decían los documentos.** OE1 §3.6.1 fila «Intentos», Nivel 2 («Fase 2: acciones rechazadas
por estar fuera de secuencia»); guion §1.6.2.2 y CU-07 5b, que solo nombran las cuatro piezas;
HU-09 FA-07 («una pieza lejos del lugar de armado»); `casos.md` S-N2B.

**Qué hacía el juego.** `AssemblySequence.Place` devuelve el mazo con `MissedMessage`, y
`WorkshopSceneController.Drill` lo registraba con `RecordRejected`, con el icono de rechazo y el
gesto de ánimo; las demás piezas soltadas lejos solo se describen.

**Decisión de Santiago, 30/09/2026.** Se corrige el juego: el mazo soltado lejos del tronco
resaltado se describe con el mismo mensaje y no suma intento, igual que las demás piezas.

**Regla (b).** Solo en el documento: se implementa en el juego.

**Corrección aplicada (30/09/2026).** Documentos: OE1: §3.6.1 fila «Intentos» · OE2: §1.6.2.2; §2
CU-07 fila «Flujos alternativos» (5b) · HU v2: HU-09 FA-07 · OE4: S-N2B paso 10; PF-RF29-01 ·
registros de tareas: Slice 2 `todo.md` W08 (nota fechada). Tareas de código: DEC-1
(`WorkshopSceneController.Drill`; prueba `WorkshopScene_RF29_SoltarElMazoLejosNoCuentaComoIntento`).

### INC-116 · Tras un cierre durante la escena de cierre del Nivel 2, el Nivel 3 seguía bloqueado — cerrado (30/09/2026)

**El conflicto.** RF-03 habilita cada nivel cuando el anterior está completado. Si el juego se
cerraba durante la escena 2.5, las tres fases del Nivel 2 ya estaban en disco y el menú lo marcaba
«Completado», pero el Nivel 3 seguía bloqueado hasta volver a jugar el Nivel 2 entero y llegar a
su resumen.

**Qué decían los documentos.** OE1 RF-03 y RNF-14; arquitectura §7 «Cuándo» y HU-18 FA-05, que
desde la edición del 29/09/2026 describían el bloqueo; `casos.md` PF-RNF14-04 y el riesgo del mismo
nombre en el `plan.md` del OE4 (Mayor).

**Qué hacía el juego.** Solo `LevelSummaryController` desbloqueaba, con
`LevelUnlockPolicy.UnlockAfterCompleting`, que avanza el nivel alcanzado; `LevelSelectController`
pintaba el menú con `PlayerProfile.IsUnlocked`, que solo lee ese nivel alcanzado.

**Decisión de Santiago, 30/09/2026.** Se corrige el juego: el menú de niveles desbloquea el nivel
siguiente cuando todas las fases del anterior están confirmadas, derivándolo del progreso guardado
y sin campo nuevo (RNF-09). Santiago aceptó que el cierre reflexivo pudiera volverse omitible al
repetir el nivel; no ocurre, porque `NarrativeVisitPolicy` sigue leyendo el nivel alcanzado: el
cierre del Nivel 2 se sigue leyendo entero hasta que se muestra su resumen (CP-07, RF-12).

**Regla (b).** Solo en el documento: se implementa en el juego.

**Corrección aplicada (30/09/2026).** Documentos: Arquitectura: §7 «Cuándo»; §12 fila 16 · HU v2:
HU-18 FA-05 · SPEC: Supuestos · Interfaces: §1.1 · OE4: S-PER PF-RNF14-04; plan §9. Tareas de
código: DEC-2 (`LevelUnlockPolicy.IsUnlocked`; pruebas
`LevelUnlockPolicy_RNF14_UnNivelConTodasSusFasesConfirmadasDesbloqueaElSiguiente`,
`LevelUnlockPolicy_RNF14_UnNivelConFasesPendientesNoDesbloqueaElSiguiente` y
`LevelSelect_RNF14_UnNivelConTodasSusFasesConfirmadasDesbloqueaElSiguiente`). La misma regla la
usa `GameFlow.TryStartPlaying`: con la primera versión, el menú pintaba el Nivel 3 desbloqueado y
el flujo rechazaba empezarlo, porque seguía consultando `PlayerProfile.IsUnlocked`. Lo vigila
`GameFlow_RNF14_EntraAlNivelQueDesbloqueanLasFasesConfirmadasDelAnterior`.

### INC-117 · Nivel 3: «Reiniciar» volvía a la recolección al repetir el nivel ya completado — cerrado (30/09/2026)

**El conflicto.** RF-07 y HU-17 FA-01: «Reiniciar» vuelve a cargar la fase activa. Las tres fases
del Nivel 3 comparten escena, y al repetir el nivel ya completado «Reiniciar» en el amarre o en el
mástil y vela volvía a la recolección.

**Qué decían los documentos.** OE1 RF-07; HU-17 FA-01 y su regla de negocio, que desde la edición
del 29/09/2026 describían ese comportamiento; `casos.md` S-PAU paso (e).

**Qué hacía el juego.** `GameFlow.PlayingPhase` solo cambiaba en `TryStartPlaying`; confirmar la
base o el amarre no lo actualizaba, y `PauseMenuPolicy.Restart` pedía la fase con la que se entró.
En la primera vuelta la retoma de RNF-14 lo disimulaba; con el nivel completo no hay fase pendiente
y se jugaba la recolección.

**Decisión de Santiago, 30/09/2026.** Se corrige el juego: al confirmar una fase del Nivel 3,
`AssemblyPanelController` registra la siguiente como fase activa, sin cambiar de estado ni de
escena; la retoma de RNF-14 y la recolección sin guardar no cambian.

**Regla (b).** Solo en el documento: se implementa en el juego.

**Corrección aplicada (30/09/2026).** Documentos: HU v2: HU-17 FA-01 · Arquitectura: §4 «Pausa»;
§12 fila 16 · SPEC: decisiones (INC-25 · HU-17) · OE4: S-PAU tabla y paso (e). Tareas de código:
DEC-3 (`GameFlow.SetPlayingPhase`, `GameFlowRunner.SetPlayingPhase`, `AssemblyPanelController`;
pruebas `GameFlow_HU17_SetPlayingPhaseActualizaLaFaseActivaSinNavegar`,
`GameFlow_HU17_SetPlayingPhaseSeRechazaFueraDePlayingOEnOtroNivelOFaseInvalida` y
`RiverScene_HU17_ReiniciarEnElAmarreOElMastilVuelveALaFaseActivaYNoALaRecoleccion`).

## Residuos y puntos abiertos

**Residuos menores — cerrados el 29/09/2026:**

1. **INC-24-r** — HU-17 y HU-18 llevan ya la tabla de encabezado «Página 17/18 de 18» en el `.docx`.
2. **INC-44-r** — `Solucion_OE2_Diseno_final` §1.4.3, estado **E5**: «Algoritm formula en la tablilla
   una pregunta orientadora», dentro de la reescritura de la tabla que hizo INC-47.
3. **INC-44-r2** — `Solucion_OE2_Diseno_final` §1.1.1: se retiró «Nombre provisional — ver punto
   abierto PG-02», y §1.2 escribe el estado de `PG-02` como «Cerrado (Algoritm)», sin tilde.

**Puntos abiertos del guion** —`Solucion_OE2_Diseno_final` §1.2— son del guion, no conflictos
entre documentos. Siguen **abiertos** `PG-05` (verificar en pruebas que el cambio de esquema de
control entre niveles no confunde) y `PG-06` (validar jugando los valores del Nivel 1): los dos
exigen observar a estudiantes jugando, que el OE4 no hace, y su columna «Situación» se actualizó a lo
que hace el juego. Están **cerrados**: `PG-01` (título «Algoritmia», en el juego desde el
09/09/2026 y en el `.docx` desde el 29/09/2026), `PG-02` (el guía se llama **Algoritm**, 02/09/2026 —
INC-44), `PG-03` y `PG-04` (redacción de `RF-16` y `RF-32`, 24/08/2026) y `PG-07` (autorización
escrita de los personajes, 30/08/2026 — INC-43).

**Pendientes de Santiago (30/09/2026).** Tres puntos del trabajo de grado que el juego no permite
redactar sin inventar; quedan abiertos hasta que Santiago aporte el dato:

1. **Anexo C** (INC-78). §3.3.2 dice que la constancia de la autorización de los autores de la
   Familia Anonaky «se incorpora como anexo», pero LISTA DE ANEXOS y ANEXOS solo tienen A
   (presupuesto) y B (cronograma). Falta el escaneo de la autorización y su enlace de SharePoint
   para añadir «Anexo C. Autorización escrita de uso de los personajes de la Familia Anonaky» en
   las dos listas.
2. **Herramientas de ilustración** (§5.2, viñeta de Adobe Illustrator y Photoshop). `Interfaces.md`
   §4 describe una especificación de estilo que se pega en un generador de imágenes, y el trabajo
   de grado solo nombra Illustrator y Photoshop. Falta saber qué herramientas se usaron de verdad.
3. **Colaboración en el arte.** Los créditos acreditan la «Producción de arte» a Sofía Valentina
   Giraldo Segovia (`CreditsContent.asset`), a quien el trabajo de grado no nombra ni en §5.2 ni en
   el presupuesto. Falta decidir cómo se nombra esa colaboración.

---

## Historial de revisiones

- **rev. 15 (30/09/2026)** — Verificación final. Por decisión de Santiago se corrige el juego en
  tres puntos que la verificación dejó a su criterio, y se registran y cierran **INC-115** (el mazo
  soltado lejos no cuenta como intento), **INC-116** (el menú deriva el desbloqueo de las fases
  confirmadas tras un cierre durante la escena de cierre) e **INC-117** (en el Nivel 3, «Reiniciar»
  vuelve a la fase activa). La misma verificación corrige siete defectos del juego sin hallazgo
  propio: la estela de Algoritm, que no llegaba a verse (INC-52); el humo del Nivel 1, que quedaba
  debajo del montón de hojas; el candado de «Empujar» (RNF-19, INC-58); el texto de créditos, en
  oraciones de veinte palabras como máximo (RNF-01, INC-78); la tarjeta del Nivel 2 del menú de
  niveles, sin costura (INC-82); el aviso de respaldo de «Salir», que se montaba sobre los botones
  con una ruta larga (INC-77), y el método sin uso `CharacterRig.Tint` (INC-109).
- **rev. 14 (29/09/2026)** — Cierre general por decisión de Santiago: ante un conflicto gana el
  juego y se edita el documento; lo que solo pide el documento se implementa; lo que solo tiene el
  juego se añade al documento. Se cierran **INC-46** a **INC-54**, los residuos `INC-24-r`, `INC-44-r`
  e `INC-44-r2` y `PG-01`. Un barrido de todos los documentos contra el juego registra y cierra
  **INC-55** a **INC-114** (sesenta hallazgos; 14 de ellos llevan además tareas de código). `PG-05` y
  `PG-06` siguen abiertos. Decisiones del mismo día: el entregable del OE3 solo recibe una nota
  fechada (D1); `productName` «Algoritmia» y `companyName` «Universidad Catolica de Colombia» (D2);
  analítica, pantalla de Unity y Alt+Intro apagados en `ProjectSettings` sin tocar el manifiesto (D3);
  «del tamaño de una palma» es una frase figurada (D4).
- **rev. 13 (25/09/2026)** — Se abre **INC-54**: el taller del Nivel 2 gana la cuerda como séptima
  pieza y un último paso, amarrar la caja; el guion, RF-27, RF-29 y CU-07 terminan el armado en la
  caja.
- **rev. 12 (24/09/2026)** — Entran los personajes animados. Se abren **INC-52**: el Algoritm
  entregado es una llama con extremidades y §7.6 pide una estrella sin ellas; se usa el arte
  entregado, con rueda y gota provisionales. Se abre **INC-53**: la animación es por recorte con
  `Image` de uGUI porque el rigging de 2D Animation no se dibuja sobre un Canvas overlay.

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
