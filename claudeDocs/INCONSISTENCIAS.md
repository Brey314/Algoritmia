# Inconsistencias entre los documentos fuente

Registro de los conflictos detectados entre los `.docx` de `docs/`, con la corrección
aplicada a cada uno. Documento hermano de `SPEC.md`: aquí está **qué estaba mal en los
documentos y cómo quedó**; allí está **qué implementa el código**.

**Los documentos se refundieron el 14/09/2026: de seis pasaron a cuatro.** OE1 es ahora
`Solución OE1_Requerimientos.docx`, y `Solucion_OE2_Diseno_final.docx` absorbe en un solo archivo
el guion (§1), los casos de uso (§2), las historias de usuario (§2 bis), las matrices de
trazabilidad (§3) y la arquitectura (§4). El trabajo de grado sigue en `docs/`: desde el 30/09/2026
el vigente es `Trabajo_de_Grado_Entrega_Plantilla_28jul.docx`, sobre la plantilla oficial del 28 de
julio; las correcciones anteriores se hicieron en `Trabajo_de_Grado_2026_ICONTEC_IEEE (2).docx`, y
los dos tienen su conversión en `docs/md/`. Las citas por sección de este documento siguen valiendo
con una traducción mecánica: «guion §N» → `Solucion_OE2` §1.N, «OE2 §4» → `Solucion_OE2` §5
(control de cambios).

**Verificación vigente: 09/10/2026, rev. 21.** La rev. 14 (29/09/2026) contrastó todos los
documentos —los `.docx` de `docs/` y los de `claudeDocs/`— con el juego. Por decisión de Santiago,
**todos los hallazgos están cerrados**: ante un conflicto gana el juego y se edita el documento; lo
que solo pide el documento se implementa en el juego; lo que solo tiene el juego se añade al
documento. Se cerraron así los nueve que seguían abiertos (**INC-46** a **INC-54**), los tres
residuos (`INC-24-r`, `INC-44-r`, `INC-44-r2`) y el punto `PG-01` del guion, y se registraron y
cerraron sesenta hallazgos nuevos, **INC-55** a **INC-114**. La rev. 15 (30/09/2026) registra y
cierra **INC-115** a **INC-117**, tres puntos que la verificación final dejó a criterio de Santiago y
en los que él decidió corregir el juego. La rev. 16 (01/10/2026) registra y cierra con la misma regla
**INC-118** a **INC-130**: los cambios de la sesión del 30/09/2026 (acta D10) que contradecían un
documento —las correcciones de los tres niveles, la prueba anticipada de la balsa y su sonido—,
cuatro hallazgos de la dirección de arte y el peso del paquete de entrega (RNF-06). La rev. 17
(05/10/2026) registra y cierra **INC-131**, una decisión de Santiago que levanta **INC-108** y la
regla de Algoritm de una sola imagen **para el arte final**: caras con seis expresiones y rig con
codos y rodillas. La rev. 18
(05/10/2026) registra y cierra **INC-132**, otra decisión suya: en la familia los brazos se dibujan detrás del
torso y delante de la cabeza (delante del torso al golpear); Algoritm, delante del cuerpo con las
manos sobre la cara. La rev. 19 (06/10/2026) registra y cierra **INC-133**, decisión de Santiago que
acota INC-132 en la familia: el húmero queda detrás del torso y el antebrazo se dibuja delante del
torso, de la cara y de las piernas. La rev. 20 (09/10/2026) registra y cierra **INC-137** a **INC-145**,
nueve decisiones de Santiago del 08/10/2026 que corrigen el juego o el documento: el laberinto con
un solo ámbar (revierte el marfil del 07/10), el diálogo en `#1F100C` a 30 px, el halo de las
fogatas con degradado y ciclo de 2 s, Algoritm por partes con arte provisional, su saludo `Wave`,
los créditos sin tarjeta, el inicio y el menú de niveles, el codo de Papá y la cámara que sigue a
la balsa. Con ella se cierran además dos pendientes de Santiago del trabajo de grado, las
herramientas de ilustración y la colaboración en el arte. La rev. 21 (09/10/2026) registra y
cierra **INC-134** a **INC-136**, tres decisiones de Santiago con la llegada del arte de perfil y del
nuevo Algoritm: la familia se ve de perfil al recorrer o trabajar el entorno (INC-134, levanta el
«ciclo único que se voltea» de la dirección de arte), el parpadeo es de dos cuadros, ojos abiertos y
cerrados (INC-135, acota §7.3), y el diseño nuevo de Algoritm —llama, disco de madera y gota, con
pantaloneta de cuadros— sustituye al núcleo de identidad de §7.6 (INC-136, levanta INC-52 y, para el
guía, la regla de color plano). Siguen abiertos
solo `PG-05` y `PG-06`, puntos del guion que exigen observar a estudiantes jugando, y los
**pendientes de Santiago** que el juego no zanja: uno del trabajo de grado y uno del sonido (ver
«Residuos y puntos abiertos»). El entregable del OE3 se reescribe al estado vigente del prototipo
(acta D10), lo que deja sin efecto la decisión D1 del 29/09/2026 de darle solo una nota fechada.

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
| 1 | Trabajo de grado — `docs/Trabajo_de_Grado_Entrega_Plantilla_28jul.docx` (desde el 30/09/2026; antes, `Trabajo_de_Grado_2026_ICONTEC_IEEE (2).docx`) | Objetivos, KPI, alcance, marco jurídico, metodología |
| 2 | `Solución OE1_Requerimientos.docx` | Lineamientos CP/CT/CN, RF-01..RF-47, RNF-01..RNF-23 |
| 3 | `Solucion_OE2_Diseno_final.docx` §1 (guion) | Narrativa, mecánicas, parámetros y textos exactos |
| 4 | `Solucion_OE2_Diseno_final.docx` §2–§3 | CU-01..CU-12, HU-01..HU-18, matrices |
| 5 | `Historias_de_Usuario_HU01_HU18_v2 (1).docx` | HU detalladas: flujos, criterios, reglas de negocio |
| 6 | `arquitectura_videojuego_v2 (2).docx` · `Solucion_OE2` §4 | Decisiones técnicas de implementación |

Las filas 3 y 4 viven en el **mismo archivo**, así que un conflicto entre ellas ya no es un
conflicto entre documentos: se corrige editándolo (ver «contradicciones internas», abajo).

Las contradicciones **internas** a un mismo documento se corrigieron editándolo.

---

## Resumen — estado a 09/10/2026 (rev. 21)

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
| INC-118 | Nivel 3: el plano de la mecánica se abre y la familia, los materiales y la balsa toman la escala de las narrativas | Dirección de arte, Inventario de arte, Interfaces, OE4, juego | **Cerrado** (01/10/2026) |
| INC-119 | Nivel 1: la chispa del golpe es un solo rayo que nace en el punto del golpe | Dirección de arte, Inventario de arte, OE4, juego | **Cerrado** (01/10/2026) |
| INC-120 | Nivel 2: la escena 2.2 abría con otra caja que la que deja la fase del bosque | Cámara N2, Dirección de arte, Inventario de arte, juego | **Cerrado** (01/10/2026) |
| INC-121 | Nivel 2: al amarrar la cuerda, la carretilla del taller pasa al dibujo amarrado con que abre la escena 2.4 | Inventario de arte, Dirección de arte, Cámara N2, juego | **Cerrado** (01/10/2026) |
| INC-122 | Nivel 3: «Probar balsa» también en la base y el amarre, sin aprobar la fase | OE1, OE2, HU v2, Dirección de arte, Inventario de arte, Interfaces, OE4, juego | **Cerrado** (01/10/2026) |
| INC-123 | Nivel 3: la balsa que se hunde suena | Dirección de sonido, Dirección de arte, Inventario de arte, juego | **Cerrado** (01/10/2026) |
| INC-124 | Nivel 3: la balsa del cruce navegaba sobre la espuma de la cascada | Dirección de arte, Inventario de arte, Interfaces, juego | **Cerrado** (01/10/2026) |
| INC-125 | La escena final suena al bosque de día con las fogatas | Dirección de sonido, OE4, juego | **Cerrado** (01/10/2026) |
| INC-126 | Cinco archivos de arte con nombre fuera de la nomenclatura | Dirección de arte, Inventario de arte, CLAUDE.md, juego | **Cerrado** (01/10/2026) |
| INC-127 | Los glifos del menú de pausa son de Phosphor Icons y los créditos los daban por originales | Dirección de arte, Inventario de arte, Interfaces, juego | **Cerrado** (01/10/2026) |
| INC-128 | La dirección de arte fijaba otros ajustes de importación que los que aplica el proyecto | Dirección de arte, Inventario de arte | **Cerrado** (01/10/2026) |
| INC-129 | El diálogo se lee en un cuadro con retrato, no en un globo con cola | Dirección de arte, Interfaces | **Cerrado** (01/10/2026) |
| INC-130 | El paquete de entrega superaba los 500 MB por los cuadros del fuego y del humo del Nivel 1 | Dirección de arte, Inventario de arte, CLAUDE.md | **Cerrado** (01/10/2026) |
| INC-131 | El arte final trae seis expresiones y extremidades en dos tramos; INC-108 fijaba una sola cara y Algoritm una sola imagen | Interfaces, Dirección de arte, Inventario de arte, Plan de personajes finales, CLAUDE.md, juego | **Cerrado** (05/10/2026) |
| INC-132 | Los brazos se escondían detrás de la cabeza y del cuerpo; la jerarquía los pintaba antes | Plan de personajes finales, Personajes-Resultados, CLAUDE.md, juego | **Cerrado** (05/10/2026) |
| INC-133 | En la familia el antebrazo debe dibujarse delante del torso, de la cara y de las piernas, con el húmero detrás del torso | Plan de personajes finales, Personajes-Resultados, CLAUDE.md, juego | **Cerrado** (06/10/2026) |
| INC-134 | La familia se ve de perfil al moverse en el entorno, hacia donde va; la dirección de arte (§13.3) daba un solo ciclo de frente que se voltea | Dirección de arte, Inventario de arte, Plan de personajes finales, Personajes-Resultados, CLAUDE.md, juego | **Cerrado** (09/10/2026) |
| INC-135 | El parpadeo es de dos cuadros, ojos abiertos y cerrados; la dirección de arte (§7.3) pedía tres | Dirección de arte, Inventario de arte, Personajes-Resultados, CLAUDE.md, juego | **Cerrado** (09/10/2026) |
| INC-136 | El diseño nuevo de Algoritm (llama, disco de madera y gota, con pantaloneta de cuadros) es el oficial; §7.6, §2.2 e INC-52 decían otro | Dirección de arte, Inventario de arte, Interfaces, Dirección de sonido, Plan de personajes finales, Personajes-Resultados, CLAUDE.md, guion y trabajo de grado, juego | **Cerrado** (09/10/2026), salvo los radicados |
| INC-137 | El laberinto tenía panel marfil y contorno suelto; el fondo es un solo ámbar y los marcos de entorno y tarjeta son iguales | CLAUDE.md, SPEC, Dirección de arte, Interfaces, Anexo C del OE3, juego | **Cerrado** (09/10/2026) |
| INC-138 | El texto del diálogo es `#1F100C` a 30 px en un cuadro de 216 px, y no `#3A1E18` a 26 px en uno de 180 | Dirección de arte, Interfaces, Anexo E del OE3, juego | **Cerrado** (09/10/2026) |
| INC-139 | El halo de las fogatas lleva degradado radial y ciclo de 2 s, y no es un círculo plano de 1,2 s | Dirección de arte, Inventario de arte, SPEC, CLAUDE.md, juego | **Cerrado** (09/10/2026) |
| INC-140 | Algoritm se dibuja por partes ya con el arte provisional; INC-131 lo pedía solo para el arte final | Dirección de arte, Inventario de arte, Interfaces, Plan de personajes finales, Personajes-Resultados, CLAUDE.md, juego | **Cerrado** (09/10/2026) |
| INC-141 | Algoritm tiene una acción nueva, `Wave` (saludo), que ningún documento nombraba | Dirección de arte, Interfaces, Personajes-Resultados, juego | **Cerrado** (09/10/2026) |
| INC-142 | La pantalla de créditos no lleva tarjeta detrás de Algoritm, que saluda | Interfaces, Dirección de arte, juego | **Cerrado** (09/10/2026) |
| INC-143 | El inicio y el menú de niveles cambian de composición: portada con los cinco personajes, título fuera de la tarjeta, icono de estado junto a la imagen | Interfaces, Dirección de arte, Inventario de arte, mockups 2 y 4, Anexo G del OE3, juego | **Cerrado** (09/10/2026) |
| INC-144 | El húmero de Papá asomaba por el codo; se corrige la tabla de articulaciones y no el arte entregado | Personajes-Resultados, Plan de personajes finales, herramientas, juego | **Cerrado** (09/10/2026) |
| INC-145 | La cámara de la 3.3 acompaña a la balsa; el encuadre fijo la dejaba salir del cuadro | Personajes-Resultados, CLAUDE.md, juego | **Cerrado** (09/10/2026) |

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

*Nota (09/10/2026): esta decisión vale para el **arte anterior**. La levanta INC-136: el guía pasa a tener
tres siluetas —llama, disco de madera y gota— con pantaloneta de cuadros, y deja de ser la llama
recoloreada.*

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
En el taller no había dibujo de la carretilla con la cuerda: la pieza misma quedaba sobre la caja
(`AssemblyContent.RopePlacedPosition`) hasta INC-121. La instrucción del guía (`N2_Guia`,
«Construir») y la escena 2.3 (`N2_Escena23_Construccion`) la nombran.

**Qué dicen hoy los documentos.** Guion §1.6.2.2: «un área de trabajo con seis piezas
dispuestas» y la tabla de seis pasos, que termina en «la carretilla queda completa» al soltar la
caja; §1.6.2.1, la línea de Algoritm «coloca encima la tabla y sobre ella la caja de alimentos».
RF-27 y RF-29, HU (flujo de la historia de la fase 2) y CU-07 describen el mismo armado sin cuerda.
Las pruebas conservan RF-27 y RF-29 en el nombre; la del paso nuevo lleva INC-54.

**Corrección pendiente.** Guion §1.6.2.1 y §1.6.2.2 (siete piezas, paso 7 y su mensaje fuera de
orden), RF-27, RF-29, la historia de la fase 2 y CU-07.

**Corrección aplicada (29/09/2026).** El guion §1.6.2.1 y §1.6.2.2 (siete piezas, paso 7 y su
mensaje fuera de orden), RF-27 y RF-29, HU-09, su resumen en OE2 y CU-07 recogen la cuerda. En el
juego, la escena 2.3 pinta la cuerda que nombra Algoritm (regla b, tarea DATA-04). Se completó
además (30/09/2026): al amarrar la cuerda, la carretilla del taller pasa a `prop_n2_carretilla_e5`,
el dibujo con que abre la escena 2.4, y se retiran `RopePlacedPosition` y `RopePlacedSize`
(INC-121).

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
código: FI-02. Se completó además (01/10/2026): la cruz pasa a ser un solo rayo que nace en el punto
del golpe y sale en una dirección al azar (INC-119).

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
atardecer en la 2.4 y noche junto al fuego en la 2.5 y el arranque de `N3_PuenteII`. Lo vigila
`NarrativeSequence_RF05_ElNivel2TranscurreDelAmanecerALaNoche`.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Nota (07/10/2026).** Decisión de Santiago: el laberinto (fase 3) sale de este reloj de luz.
`MazeLayout.LightTint` se eliminó y `MazeScene_RF30_ElLaberintoEsAlAtardecer` se retiró; el
entorno del laberinto queda tal cual el arte, sin tinte, con panel marfil `#F7EFE2` y contorno
`#C4A882`. El resto de la narrativa del Nivel 2 sigue como se describe arriba.

**Nota (08/10/2026).** Santiago revirtió el panel marfil y el contorno suelto: ahora todo lo que rodea
al laberinto es un solo ámbar `#E8A33D` y el entorno y la tarjeta llevan el mismo marco redondeado de 8 px.
Ver INC-137. El laberinto sigue sin teñirse.

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
Trabajo de grado: Resumen; Abstract; Introducción; §3.3.2; §5.2 (LISTA DE ANEXOS y ANEXOS, final del documento: **pendientes** —falta el anexo con el
escaneo de la autorización—) · HU v2: HU-18 · Dirección de sonido: §17; §18 S-05. Tareas de código: DATA-01.
Desde el 30/09/2026 el trabajo de grado vigente es la plantilla del 28/07, que no recogía estas
correcciones ni deja libre la letra C. Se aplicaron a ella el 01/10/2026 —Resumen, Abstract,
Introducción, §3.3.2 y la viñeta del libro en §5.1—; el anexo con la autorización sigue pendiente:
ver «Pendientes de Santiago», puntos 1 y 4.
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

**Nota (01/10/2026).** Los dos `ScrollRect` del informe docente y el de los créditos son la excepción
decidida el 29/09/2026 (`Slice 4/Slice-4-Resultados.md` A.6): esas pantallas no tienen arrastrar y
soltar, y su barra se maneja con clic y clic sostenido. La regla «una lista que desborda se desplaza
con botones, nunca con `ScrollRect`» rige donde hay arrastrar y soltar —la secuencia del laberinto
se desplaza con ▲/▼—. Con ocho perfiles, la octava fila del informe sale con el borde recortado,
pero se ve y se elige: es una observación menor, sin corrección en este corte.

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

**Nota (08/10/2026).** Vuelve el bosque, encuadrado con foco 0,25 por `FramedIllustration` para no cruzar la
costura, y el laberinto deja la tarjeta (INC-143). Los créditos pierden la tarjeta de Algoritm (INC-142).

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
§3.3.1. La plantilla del 28/07, vigente desde el 30/09/2026, no la recogía; se aplicó el 01/10/2026:
§3.3.1 dice ahora que los datos quedan en JSON sin cifrar y que el informe docente no tiene clave
(«Pendientes de Santiago», punto 4).

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
Se completó además (01/10/2026): el hundimiento suena (`sfx_n3_hundimiento`, INC-123); en imagen
sigue sin efecto de agua.

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
OE4: Sesión del Nivel 3; PF-RF40-02 · OE2: §1.8.4; §2.3 CU-10 fila «Flujos alternativos». Se completó
además (01/10/2026): en la base y el amarre, «Probar balsa» se ofrece también junto a «Listo» y no
aprueba la fase (INC-122).

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

**Nota (01/10/2026).** El ejecutable rc1 abría conexiones a :443 y dejaba restos de Analytics e
Insights en LocalLow y `unity_connect.*` en HKCU (DEF-SPIKE-01, `OE4-Resultados.md` §6 y §8.3)
aunque `UnityConnectSettings.asset` dijera `m_Enabled: 0`. **Causa raíz:** el interruptor general de
Unity Connect estaba encendido en la memoria del Editor, y el build serializa esa copia y no el
archivo; `m_EngineDiagnosticsEnabled` (Insights) estaba además en 1 también en disco. **Arreglo:**
`Assets/Game/Scripts/Editor/UnityServicesOff.cs` (`Game.EditorTools`) apaga a la fuerza nueve
interruptores antes del build —el general de Connect, Analytics, Performance Reporting, Cloud
Diagnostics, Insights, Purchasing, Ads, `submitAnalytics` y `enableCrashReportAPI`— y, después,
hace fallar el build si `globalgamemanagers` nombra `unity3d.com`; `m_EngineDiagnosticsEnabled` pasa
a 0, y `PlayerSettingsTest` se amplía para exigir apagados todos los servicios del archivo, Insights,
Cloud Diagnostics y `enableCrashReportAPI`. En rc2 hay cero conexiones y en LocalLow solo
`Player.log` (PF-RNF10-01, P). **Residuo (DEF-RC2-01, Menor):** el reproductor sigue escribiendo en
HKCU tres `unity_connect.*` —entre ellos un identificador de instalación— y un contador y un
identificador de sesión (`unity.player_session_count`, `unity.player_sessionid`). No sale del equipo
ni es un dato del estudiante, y se documenta como residuo del motor para la entrega, junto al
`Player.log` y las claves de pantalla. Para un build futuro queda como mejora recomendada
`m_InitializeOnStartup: 0` (Analytics) en `UnityConnectSettings.asset` o borrar esas claves de `PlayerPrefs` al
salir; las dos piden build nuevo, repetir los residuos y la SUITE. Desactivar el módulo
`com.unity.modules.unityanalytics` choca con D3.

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

*Nota (05/10/2026): esta decisión vale para el **arte actual**. Para el arte final la levanta
INC-131: seis expresiones, parpadeo y habla.*

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

**Corrección aplicada (29/09/2026).** Documentos: Trabajo de grado: §1.4. La plantilla del 28/07,
vigente desde el 30/09/2026, no la recogía (allí es §1.3); se aplicó el 01/10/2026 («Pendientes de
Santiago», punto 4).

### INC-112 · El trabajo de grado describía el patrón de unidades didácticas con Wix y para 7-8 años — cerrado (29/09/2026)

**El conflicto.** §3.2.4 atribuye al proyecto otra plataforma y otra edad.

**Qué decían los documentos.** TG §3.2.4, tercera viñeta.

**Qué hace el juego.** Ejecutable de Unity sin red para 9-11 años; acceso secuencial por desbloqueo
de niveles y avance con un clic.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (29/09/2026).** Documentos: Trabajo de grado: §3.2.4. La plantilla del
28/07, vigente desde el 30/09/2026, no la recogía; se aplicó el 01/10/2026 («Pendientes de
Santiago», punto 4).

### INC-113 · El trabajo de grado no recogía herramientas ni actividades del desarrollo — cerrado (29/09/2026)

**El conflicto.** §5.2 y §5.1.3 omiten herramientas y bloques de trabajo que el proyecto usó.

**Qué decían los documentos.** TG §5.2 (instrumentos) y §5.1.3 (actividades de la Fase 3).

**Qué hace el juego.** Unity Test Framework, Input System, Git/GitHub, Claude; sonido, perfil con
informe docente y borrado, ejecutable portable medido.

**Regla (c).** Solo en el juego: se añade al documento.

**Corrección aplicada (29/09/2026).** Documentos: Trabajo de grado: §5.2; §5.1.3. La plantilla del
28/07, vigente desde el 30/09/2026, no recogía la de los instrumentos (allí, §5.1); se aplicó el
01/10/2026 con cuatro viñetas nuevas —Unity Test Framework, Input System, Git y Claude Code—
(«Pendientes de Santiago», punto 4).

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

### INC-118 · Nivel 3: el plano de la mecánica se abre y la familia, los materiales y la balsa toman la escala de las narrativas — cerrado (01/10/2026)

**El conflicto.** La mecánica del Nivel 3 mostraba la orilla a cuatro décimas de la escala con que
la cuentan las narrativas del mismo río —la casilla de Papá medía 77,7 píxeles de la ilustración en
la recolección y 191,5 en la escena 3.1—, y los registros del Slice 3 fijaban esos planos como
decisiones.

**Qué decían los documentos.** `Slice-3-Resultados.md` §3, decisiones 2 y 4: el ensamblaje empuja a
(0,50; 0,23) ×2,2, con el río al 85 % del ancho (20/09/2026), y la recolección se abre a
(0,2632; 0,2632) ×1,9 para que asome un poco el río (decisión de Santiago del 25/09/2026);
`Slice 3/todo.md`, el plano del 25/09 con radios de 0,04 y las casillas del radio de RF-37 y del
encuadre del ensamblaje «validados jugando»; `casos.md` S-N3, las posiciones en la orilla y
«Recoger» a unos ±0,08 de pantalla; la prueba
`RiverLevelConfig_Guion82_ElPlanoDeRecoleccionEsElBosqueConUnPocoDelRio`, con el recorte derecho en
0,6 como máximo «para que siga siendo el bosque». El diseño de cámara del Nivel 3 ya no está en el
equipo: el encuadre de la 3.1 solo consta en `N3_Escena31_Llegada.asset`. El guion §1.8, fila
«Escenario» —un plano fijo a la altura de los personajes, con perspectiva por profundidad—, no fija
encuadre ni escala, y no cambia.

**Qué hacía el juego.** A 1920×1080, Papá medía 148 px en la recolección y 171 en el ensamblaje, la
Niña y el Niño 89, y Mamá de 121 a 185 según la profundidad; la balsa del ensamblaje ocupaba 665 px,
con el 56 % del casco sobre el agua, y un tronco de 256 px se ampliaba a 353. Mamá se anclaba por el
centro de su casilla y, al retomar el ensamblaje, quedaba en el arranque. La 3.1 pintaba el montón
plano `prop_n3_troncos`, en el estilo anterior a los materiales de la mecánica.

**Decisión de Santiago, 30/09/2026 (acta D10).** Se abre el plano de la mecánica —la recolección de
×1,9 a cerca de ×1,4 y el ensamblaje de ×2,2 a cerca de ×1,6—; los personajes crecen cerca de 2,3
veces, los materiales 1,8 y la zona 1,5, la orilla se reacomoda y la 3.1 pasa a coincidir con la
mecánica. **Por delegación de Santiago (01/10/2026)**, el orquestador fijó los valores medidos sobre
`env_n3_rio.png` y los ajustes que los hacen posibles: Mamá se ancla por los pies, como la familia;
la balsa del ensamblaje se corre en x —no baja— para quedar sobre el agua sin tapar a Papá; al
retomar el ensamblaje, Mamá espera en la zona, y Mamá, la familia y los materiales se dibujan por
profundidad.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (01/10/2026).** Documentos: Dirección de arte: §5.3 (Mamá y la familia se
anclan por los pies; lo de abajo se dibuja delante); §8.3 «Plano y escala» · Inventario de arte:
`Environments/River/` (`env_n3_rio` y la zona); `Props/River/` (`prop_n3_troncos` queda sin uso) ·
Interfaces: §1.1 «11–12 · Nivel 3» · OE4: S-N3 (posiciones en la orilla y en el ensamblaje, zona
transitable y alcance de «Recoger») · registros de tareas: `Slice-3-Resultados.md`,
`Props-y-Sonidos-Resultados.md` y `Personajes-Resultados.md` (notas fechadas). Los radicados no fijan
encuadres y no cambian. Tareas de código: D10-3. `PlayFraming`
(0,3572; 0,3572) ×1,4 y `AssemblyFraming` (0,50; 0,3125) ×1,6; `RaftPosition` (0,58; 0,23), con
`RaftSize` 0,28, el de la balsa de la 3.3; `WalkableArea` x 0,18–0,40 · y 0,05–0,34, que ahora son
los pies de Mamá; `StartPosition` (0,21; 0,19); `BuildZonePosition` (0,395; 0,22); `ProximityRadius`
y `BuildZoneRadius` 0,06, y los ocho materiales repartidos por la orilla, ninguno a menos de 0,12 de
la zona (`N3_RiverLevelConfig`, `N3_RaftAssemblyContent`). En `Level3_River`, por `sizeDelta` de las
instancias y sin reconstruir prefabs: Mamá 240 con pivote (0,5; 0,075), Papá 178,75, la Niña y el
Niño 107,25, los materiales 172,8 y la zona 255. `RiverSceneController.ResumeAt` deja a Mamá en la
zona y `DepthOrder` (C# plano) pone delante, entre Mamá, la familia y los materiales, lo que está
más abajo. La 3.1 pinta `prop_n3_tronco` y
`prop_n3_mastil` (Size 0,07; el mástil girado −50°). A 1920×1080, Papá pasa a 250 px en la orilla y
a 286 en el ensamblaje, la balsa a 484 px y un tronco de la balsa a 256, sin ampliar el sprite.
Pruebas: `RiverLevelConfig_Guion82_ElPlanoDeRecoleccionEsLaOrillaConElRio` (sustituye a
`…EsElBosqueConUnPocoDelRio`), `RiverScene_INC118_LosPersonajesDeLaMecanicaTienenLaEscalaDeLaNarrativa`,
`RiverLevelConfig_INC118_LaEscena31PintaCadaMaterialConElArteDeLaOrilla`,
`RiverScene_DA83_MamaSeAnclaPorLosPiesComoLaFamilia`,
`RaftAssemblyContent_RF44_LaBalsaDelEnsamblajeMideLoQueLaDelCruce`,
`DepthOrder_DA83_LoQueEstaMasAbajoSeDibujaDelante`,
`DepthOrder_DA83_ALaMismaAlturaConservaElOrdenQueTraia`,
`RiverScene_DA83_LoQueEstaMasAbajoSeDibujaDelante` y
`RiverScene_RNF14_AlRetomarElEnsamblajeMamaEsperaEnLaZona`, más
`RiverLevelConfig_RF37_ElRepartoObligaARecorrerLaOrillaYCabeEnElPlanoFijo` ampliada. Siguen en verde
las pruebas de RNF-03 del Nivel 3 a 1920×1080 y
`RiverScene_INC01_NoExisteVinculacionDeTecladoEnElMapaDeControles`.

### INC-119 · Nivel 1: la chispa del golpe es un solo rayo que nace en el punto del golpe — cerrado (01/10/2026)

**El conflicto.** El juego dibuja la chispa del golpe como un solo rayo que sale del punto del golpe
en una dirección al azar; la dirección de arte la describe como cuatro líneas radiales, y el índice
de sprites y el catálogo de pruebas, como una cruz que se encoge sobre el montón.

**Qué decían los documentos.** `Direccion_de_Arte.md` §12.2, filas «Chispas que se apagan» (cuatro
líneas cortas radiales `#FFE9A8` que se acortan hasta desaparecer, 0,35 s) y «Chispa» (cuatro líneas
radiales, 0,2 s); `Inventario.md` §`FX/`, fila de los `fx_n1_chispa_*` retirados («dos `Image` planas
en cruz»); la corrección aplicada de INC-68; `casos.md` S-N1, pasos 17 y 18 («una chispa sobre el
montón que se encoge y desaparece»). El guion §1.4.3.3 —la chispa cae dentro del montón; con fuerza
de más, saltan lejos de las hojas y se apagan en el aire— ya describe el rayo y no cambia.

**Qué hacía el juego.** Desde `37b3cb7` (INC-68), una cruz de dos `Image` planas `#FFE9A8` (`RayoH`
y `RayoV`) que se encogía hasta desaparecer sobre el punto del golpe; la del golpe de más salía
desplazada hacia arriba (`DyingSparkOffset`).

**Decisión de Santiago, 30/09/2026 (acta D10).** La chispa pasa a ser un solo rayo que nace en el
punto del golpe y sale en una dirección al azar: con el golpe efectivo cae dentro del montón; con
fuerza de más es largo, pasa del montón y se apaga en el aire; en los dos casos hace un único
barrido, sin volver (RNF-21). **Por delegación de Santiago (01/10/2026)**, el orquestador fijó el
reparto sobre el arte medido: el rayo efectivo sale hacia la mitad de abajo y cae a 0,205–0,22 del
lado del montón, siempre sobre hojas y nunca sobre una piedra; el de más sale hacia la mitad de
arriba, llega a 0,52–0,6 y se apaga antes de la tablilla. El trazo mide 4 unidades, 8 px a 1080.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (01/10/2026).** Documentos: Dirección de arte: §7.6 «Nomenclatura»; §12.2, las
dos filas de la chispa y su barrido único · Inventario de arte: `FX/`, fila de `fx_n1_chispa_*` ·
OE4: S-N1 pasos 17 a 20; PF-RF16-01 · INC-68 (nota) · registros de tareas: `Fase-5-6-Resultados.md`
y `Props-y-Sonidos-Resultados.md` (notas fechadas). Tareas de código: D10-1. `FirePanelController` elige la dirección con `SparkRandom`
—el patrón de `FloorScatter.Place`— y barre cabeza y cola una sola vez, sin escalar; en
`Level1_Cave`, `Chispa` lleva el pivote en la cola, `RayoH` se estira sobre toda ella con el mismo
`#FFE9A8` y `RayoV` se borra. Pruebas:
`FirePanel_RF16_LaChispaEsUnRayoDelCentroQueCaeEnLasHojasEnUnaDireccionAlAzar`,
`FirePanel_RF16_ConFuerzaDeMasElRayoPasaDeLasHojasYSeApagaEnElAire`,
`FirePanel_RNF21_ElRayoDeLaChispaHaceUnSoloBarridoSinVolver` y la captura
`FirePanel_RF16_ElRayoDeLaChispaSeVeSobreElMonton`; se ajustan
`FirePanel_RF16_LaChispaSoloSeVeCuandoLasPiedrasChocanConFuerza` y `SparkDurationAsync`.

### INC-120 · Nivel 2: la escena 2.2 abría con otra caja que la que deja la fase del bosque — cerrado (01/10/2026)

**El conflicto.** Al pasar del bosque a la escena 2.2, la caja de alimentos cambiaba de dibujo, de
sitio y de tamaño, y los documentos piden que la narrativa herede el cuadro exacto con que termina
la fase.

**Qué decían los documentos.** Guion §1.6.1.1 («A un lado, la caja de alimentos…») y §1.6.1.3 («La
caja rueda sobre los troncos…»); `Camara_Narrativa_N2.md` §5.3 —el cierre de la fase 1 deja la
cámara en el encuadre con que abre la 2.2—, §5.4 fila `L0` («hereda exacto» el cuadro de la fase 1:
misma vista, mismos troncos, mismo sitio) y §7 «Puesta en escena de 2.2»; el tooltip de
`WheelLevelConfig.CargoPlacedPosition` («el punto en el que la escena 2.2 la dibuja al abrir»);
`Slice-2-Resultados.md` §A.13, que dejaba por decidir si el salto era buscado.

**Qué hacía el juego.** Desde `1d5ce58` (25/09/2026), la 2.2 dibujaba `prop_n2_caja_suelo_vacía` en
(0,2; 0,545) con 0,12 del alto, y el bosque deja `prop_n2_caja_suelo` en (0,2; 0,5089) con 0,148.
`ForestScene_RF26_…` no lo veía porque comparaba con una constante copiada.

**Decisión de Santiago, 30/09/2026 (acta D10).** Solo se corrige la caja de la 2.2: vuelve a ser la
caja llena que la fase del bosque deja sobre los troncos, en su mismo sitio y con su mismo tamaño.

**Regla (b).** Solo en el documento: se implementa en el juego.

**Corrección aplicada (01/10/2026).** Documentos: Cámara N2: §5.3; §5.4 y su fila `L0`; §7 «Puesta en
escena de 2.2» · Dirección de arte: §9.3 fila «Caja de alimentos (3 estados)» · Inventario de arte:
`Props/Wheel/` (la caja llena y la vacía, sin uso) · registros de tareas: `Slice-2-Resultados.md` y
`Props-y-Sonidos-Resultados.md` (notas fechadas). Tareas de código: D10-2.
La caja de `N2_Escena22_ElPatron` (`Prop_9`) vuelve a `prop_n2_caja_suelo`, en (0,2; 0,5089), con
Size 0,14814815 y `MotionDrop` 0,074074075 —la inversa exacta de `1d5ce58`—, y sigue deslizándose
sin girar; `prop_n2_caja_suelo_vacía` queda sin uso (su nombre lo corrige INC-126). Pruebas:
`WheelLevelConfig_RF05_LaCajaColocadaEsLaQueLaEscena22DibujaAlAbrir`;
`ForestScene_RF26_LaCajaYLosTroncosTerminanEnCuadroDondeLosDibujaLaEscena22` lee la caja de
`CargoPlacedPosition` en lugar de la constante.

### INC-121 · Nivel 2: al amarrar la cuerda, la carretilla del taller pasa al dibujo amarrado con que abre la escena 2.4 — cerrado (01/10/2026)

**El conflicto.** El guion y la dirección de arte terminan el taller con la carretilla completa y la
cuerda amarrada; el juego dejaba la cuerda como una pieza suelta sobre la caja, y el dibujo amarrado
solo salía en las narrativas.

**Qué decían los documentos.** El guion §1.6.2.2, paso 7 («La cuerda queda amarrada sobre la caja y
la carretilla queda completa»), y `Direccion_de_Arte.md` §9.3 («Rueda y carretilla (5 estados)») ya
lo pedían; INC-54 «Qué se hizo» («No hay dibujo de la carretilla con la cuerda: la pieza misma queda
sobre la caja») e `Inventario.md` §`Props/Wheel/`, fila `B6` («`e5` … que solo usan las narrativas:
en el taller la cuerda es la pieza colgada sobre `e4`»), describían lo que hacía el juego.

**Qué hacía el juego.** Al soltar la cuerda sobre la caja, el taller conservaba `e4` y dejaba la
pieza encima (`AssemblyContent.RopePlacedPosition` y `RopePlacedSize`). `prop_n2_carretilla_e5` solo
lo usaban `N2_Escena24_Regreso`, `N2_Escena25_Cierre`, `N3_PuenteII`, `N3_PuenteII_Rio` y
`N3_Escena31_Llegada`.

**Decisión de Santiago, 30/09/2026 (acta D10).** Al amarrar la cuerda, la carretilla pasa al dibujo
de la carretilla amarrada, el mismo con el que abre la escena 2.4.

**Regla (b).** Solo en el documento: se implementa en el juego.

**Corrección aplicada (01/10/2026).** Documentos: Inventario de arte: `Props/Wheel/` fila `B6` ·
Dirección de arte: §9.3 fila «Rueda y carretilla (5 estados)» · Cámara N2: §5.6 fila `CIERRE`; §7
«Claro este» · INC-54 (nota) · registros de tareas: `Slice-2-Resultados.md` y
`Props-y-Sonidos-Resultados.md` (notas fechadas). Tareas de código: D10-2.
Campo `AssemblyContent.TiedArt` —`prop_n2_carretilla_e5` en `N2_AssemblyContent`— y
`case WorkshopPiece.Rope` en `WorkshopSceneController.ShowAssembly`; se retiran la rama de la cuerda
colgada de `Release`, `RopePlacedPosition` y `RopePlacedSize`. `e4` y `e5` comparten encuadre, así
que la carretilla no salta de sitio. Pruebas:
`WorkshopScene_INC54_AlAmarrarLaCuerdaLaCarretillaPasaAlDibujoConCuerda` y
`AssemblyContent_INC54_LaCarretillaAmarradaEsElDibujoConElQueAbreLaEscena24`, que conservan INC54
porque cierran el paso que abrió INC-54; se ajusta
`WorkshopScene_RF29_LaCarretillaCreceSobreLoAnteriorSinQueLaInterfazLaTape`.

### INC-122 · Nivel 3: «Probar balsa» también en la base y el amarre, sin aprobar la fase — cerrado (01/10/2026)

**El conflicto.** Los documentos reservan «Probar balsa» a la fase de mástil y vela, donde probar es
confirmar, y hacen de la primera prueba fallida la única interrupción del nivel; el juego ofrece
además la prueba en la base y el amarre, sin aprobar la fase ni interrumpirla.

**Qué decían los documentos.** OE1 §3.2 RF-11 («La única acción que interrumpe la fase es la primera
prueba fallida de la balsa…»), §3.5 RF-40 («Cada fase dispone de un botón de confirmación…») y RF-42
(«El botón de probar balsa debe validar el ensamblaje…»), y §1.1 CP-06, que repite el disparo de la
3.2; el guion §1.8.3 (tabla de fases: «Listo» en la base y el amarre, «Probar balsa» solo en mástil y
vela, con la validación final), §1.8.4 (fila «Prueba de la balsa fallida» y párrafo «Pista del
guía») y §1.8.4.1 (la 3.2 «se dispara la primera vez que la prueba de la balsa falla»); CU-03
(precondiciones) y CU-10 (flujos alternativos 6a y 6b); HU-12 (paso 6 y fila «Botón «Listo» /
«Probar balsa»») y HU-13 (paso 7, FA-01 y fila «Botón Probar balsa»); INC-90;
`Direccion_de_Arte.md` §12.3 fila «Prueba de balsa sin éxito (Nivel 3)»; `casos.md` S-N3 y
PF-RF42-01. OE1 §3.6.1 ya cuenta las pruebas de balsa fallidas como intentos del Nivel 3 y no
cambia.

**Qué hacía el juego.** El botón del panel se rotulaba «Listo» en la base y el amarre y «Probar
balsa» en mástil y vela (INC-90): la balsa no se podía probar antes de la última fase.

**Decisión de Santiago, 30/09/2026 (acta D10).** En la base y el amarre, «Probar balsa» se ofrece
junto a «Listo». La balsa incompleta se hunde y suena la salpicadura (INC-123); la prueba cuenta como
intento y devuelve solo lo mal puesto, pero no aprueba la fase ni dispara la escena 3.2, que sigue
saliendo únicamente con el primer fallo de la fase de mástil y vela. **Por delegación de Santiago
(01/10/2026)**, el orquestador fijó el detalle: antes de la última fase se marca solo lo mal puesto y
no los espacios vacíos, porque una balsa a medio armar está incompleta por construcción y marcar sus
vacíos sería un mapa de dónde va cada pieza (CP-06); si solo hay vacíos, la tablilla dice «La balsa
se hundió: todavía no está terminada. ¿Qué parte falta armar?», sin cifras ni solución; la prueba
anticipada suma a las tres seguidas que traen la pregunta orientadora, y una base entera y bien
puesta tampoco se aprueba al probarla: solo «Listo» consolida (RF-40).

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (01/10/2026).** Documentos —los radicados, por automatización de Word (acta
D10), cada uno con su fila del 01/10/2026 en el control de cambios—: OE1: §1.1 CP-06; §3.2 RF-11;
§3.5 RF-40; §3.5 RF-42; §6 · OE2: §1.8.3, párrafo que abre el apartado; §1.8.4 fila «Prueba de la
balsa fallida» y párrafo «Pista del guía» (confirmar o probar tres veces seguidas sin que la fase
valide); §1.8.4.1, acotación final; §2.3 CU-03 fila «Precondiciones»; §2.3 CU-10 flujos alternativos
2c, nuevo, y 6a; §5 · HU v2: HU-12 paso 6 y fila «Botón «Listo» / «Probar balsa»», versión 1.3;
HU-13 paso 7, FA-01, FA-03, nuevo, y fila «Botón Probar balsa», versión 1.3 · Dirección de arte:
§12.3 fila «Prueba de balsa sin éxito (Nivel 3)» · Inventario de arte: `UI/River/` (los dos botones
del panel) · Interfaces: §1.1 «11–12 · Nivel 3» · OE4: S-N3; PF-RF42-01; PF-RF42-02, nuevo, para la
prueba anticipada · INC-90 (nota) · registros de tareas: `Slice-3-Resultados.md` (nota fechada).
CP-06 y CU-03 no estaban en la lista del acta D10: se editan por delegación de Santiago, porque
repetían el disparo de la 3.2 y habrían quedado en contradicción con RF-11 y CU-10.
Tareas de código: D10-3. `RaftAssembly.Test()` (C# plano) equivale a `Confirm()` en la última fase;
en la base y el amarre valida la fase abierta, devuelve lo mal puesto (RF-43), cuenta un
rechazo (RF-45) y nunca consolida ni abre fase (RF-40). `RaftAssemblyContent.UnfinishedTestMessage`
vive en `N3_RaftAssemblyContent`. En `AssemblyPanelController` y `Level3_River`, `Button_Test` aparece
junto a «Listo» en la base y el amarre y se oculta en mástil y vela; un solo `Validate` atiende los
dos botones, y la prueba anticipada no llama a `AfterAttempt`, así que no gasta la escena 3.2. El
código lleva el comentario pedagógico (CP-02, CP-06). Pruebas en EditMode:
`RaftAssembly_RF42_ProbarLaBalsaIncompletaNoConfirmaNiAvanzaDeFase`,
`RaftAssembly_RF43_ProbarAntesDeTiempoDevuelveSoloLoMalPuesto`,
`RaftAssembly_RF41_ProbarLaBalsaNoTocaLasFasesAprobadas`,
`RaftAssembly_RF42_EnLaUltimaFaseProbarEsConfirmar`,
`RaftAssembly_RF42_EnLaUltimaFaseProbarConUnVacioSenalaElVacioConElMensajeDeLaPrueba` y
`RaftAssemblyContent_RF42_ElAssetTraeElMensajeDeLaBalsaSinTerminar`; en PlayMode:
`AssemblyPanel_RF42_ElBotonProbarBalsaEstaDisponibleEnCadaFase`,
`AssemblyPanel_RF42_ProbarLaBalsaIncompletaLaHundeYSigueEnLaFase`,
`AssemblyPanel_RF42_ProbarAntesDeTiempoSenalaSoloLoMalPuesto`,
`AssemblyPanel_RNF03_EnLaBaseLosDosBotonesLaBalsaYLasTablillasCabenSinSolaparse`,
`RiverScene_Guion841_ProbarAntesDeLaUltimaFaseNoGastaLaEscena32` y
`RiverIndicators_RF45_UnaPruebaDeBalsaIncompletaCuentaComoIntento`. Siguen en verde
`AssemblyPanel_CP02_UnaFaseQueNoPasaNoSuena` y
`RaftAssembly_CP02_NoHayLimiteDeIntentosNiPantallaDeDerrota`.

### INC-123 · Nivel 3: la balsa que se hunde suena — cerrado (01/10/2026)

**El conflicto.** La dirección de sonido pone una pieza al hundimiento de la balsa y el juego lo
dejaba mudo; la dirección de arte se contradecía sobre la salpicadura.

**Qué decían los documentos.** `Direccion_de_Musica_y_Sonido.md` §13 fila `sfx_n3_hundimiento`
(disparador «Prueba de la balsa fallida», 2,5 s) y §19 rev. 4 («Una fase que no pasa, la balsa que
se hunde y entrar a la zona sin todo no suenan (§2.1)», con `sfx_n3_salpicadura_undimiento` en disco
y sin referenciar); `Direccion_de_Arte.md` §12.3 fila «Prueba de balsa sin éxito» («sin salpicadura
ni destellos») frente a §8.3 «Sobre el riesgo» («la retroalimentación es una salpicadura»). §2.1 de
la dirección de sonido pone la balsa que gira entre los sonidos descriptivos de un intento sin éxito:
se cumple.

**Qué hacía el juego.** El hundimiento de 0,6 s era mudo. La pieza en disco,
`sfx_n3_salpicadura_undimiento`, se había renombrado fuera del motor —con GUID nuevo y una errata en
el nombre— y no la referenciaba nada.

**Decisión de Santiago, 30/09/2026 (acta D10).** La balsa que se hunde suena a salpicadura en las tres
fases, también con la prueba anticipada (INC-122). La pieza se renombra desde el motor.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (01/10/2026).** Documentos: Dirección de sonido: §13 fila `sfx_n3_hundimiento`
(entregada, de 1,74 s; suena con toda prueba que hunde la balsa); §19, piezas cableadas y rev. 5 ·
Dirección de arte: §8.3 «Sobre el riesgo»; §12.2 fila «Salpicadura de la balsa (Nivel 3)»; §12.3
fila «Prueba de balsa sin éxito (Nivel 3)» (la salpicadura es sonora; en imagen no hay efecto de
agua) · Inventario de arte: «Puntos abiertos», punto 6 · registros de tareas:
`Props-y-Sonidos-Resultados.md` y `Slice-3-Resultados.md` (notas fechadas). Tareas de código: D10-3. La pieza se renombra a
`sfx_n3_hundimiento` con `AssetDatabase.RenameAsset` —el GUID y el `.meta` no cambian— y es
`RiverSounds.RaftSinking` en `N3_Sonidos`; suena al empezar todo hundimiento, y «Listo» rechazado y
la zona sin todos los materiales siguen mudos. El código lleva el comentario de §2.1 y CP-02: el
sonido describe, no castiga. Pruebas: `RiverSounds_RF42_LaBalsaQueSeHundeSuenaASalpicadura`
(EditMode) y `AssemblyPanel_RF42_LaBalsaQueSeHundeSuenaUnaSalpicaduraYNadaMas`, en la base y en
mástil y vela (PlayMode). Siguen en verde `AssemblyPanel_CP02_UnaFaseQueNoPasaNoSuena`,
`AudioAssets_CP02_NingunaPiezaSeLlamaDerrotaNiError` y
`AudioImport_RNF06_CadaFamiliaEntraConLosAjustesDeSuTabla`. El volumen y la doble salpicadura de dos
pruebas seguidas se comprueban de oído (guion H7 de `claudeDocs/tasks/OE4/Hoja-HUM.md`).

### INC-124 · Nivel 3: la balsa del cruce navegaba sobre la espuma de la cascada — cerrado (01/10/2026)

**El conflicto.** En la escena 3.3 la balsa cruzaba sobre la espuma del pie de la cascada, y bajarla
exige mover también una parada de cámara, contra la regla de la sesión D05: cuando objeto y encuadre
no concuerdan, se mueve el objeto.

**Qué decían los documentos.** `Slice 3/todo.md`, tarjeta R04 (la balsa del cruce, «más arriba de lo
que propone §6 del documento de cámara para no quedar bajo el cuadro de diálogo»); la regla de D05,
en `CLAUDE.md` §Arquitectura y en el acta D05; `Slice-3-Resultados.md`. El diseño de cámara del
Nivel 3 ya no está en el equipo: lo aplicado solo consta en `N3_Escena33_Cruce.asset`.

**Qué hacía el juego.** `prop_n3_balsa_cruzando` cruzaba con el centro en y 0,455 (x 0,56, Size 0,28),
con el 32 % de la cubierta y el pie del mástil sobre la espuma, que ocupa y 0,403–0,544 de
`env_n3_rio.png`.

**Decisión de Santiago, 30/09/2026 (acta D10).** De las balsas narrativas del Nivel 3 solo baja la del
cruce, y se autoriza retocar su parada de cámara como excepción a la regla de D05. **Por delegación de
Santiago (01/10/2026)**, el orquestador la dejó en 0,395 y no en 0,39: a 0,39 tocaba el cuadro de
diálogo en la parada L4, que no se mueve.

**Regla (a)**, con excepción expresa a la regla de D05, que sigue valiendo para las demás escenas.

**Corrección aplicada (01/10/2026).** Documentos: Dirección de arte: §8.3 «Plano y escala»; §9.3
fila de la balsa compuesta · Inventario de arte: `Props/River/` fila `C9` · Interfaces: §1.1 «11–12 ·
Nivel 3» · registros de tareas: `Slice-3-Resultados.md` y `Personajes-Resultados.md` (notas
fechadas). Sin el documento de cámara del Nivel 3, el registro del encuadre es el propio asset.
Tareas de código: D10-3. En
`N3_Escena33_Cruce.asset` la balsa baja de y 0,455 a 0,395, con el 3,9 % de la cubierta sobre la
espuma; los cuatro viajeros bajan 0,06 con ella —posición inicial y pasos de las líneas 0 y 2: Papá y
Mamá a 0,397, la Niña a 0,345 y el Niño a 0,341—, y los pasos de la línea 3, que bajan a la otra
orilla, no cambian (bajarlos metería a la Niña en el río); la parada L3 pasa de (0,65; 0,47) a
(0,65; 0,41) con el mismo zoom, ×1,55. Pruebas:
`NarrativeSequence_RF44_LaBalsaDelCruceNavegaBajoLaEspumaDeLaCascada`,
`NarrativeSequence_RF44_LaFamiliaBajaConLaBalsaYViajaEnSuSitio` y
`NarrativeScene_RNF21_LaBalsaCruzaSinSaltosNiParpadeos`;
`NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo` sigue en verde sin recortar
`Verificadas`.

### INC-125 · La escena final suena al bosque de día con las fogatas — cerrado (01/10/2026)

**El conflicto.** La dirección de sonido daba a la escena final una pieza que ya no existe, y el juego
la dejaba muda.

**Qué decían los documentos.** `Direccion_de_Musica_y_Sonido.md` §8 fila `amb_viento_horizonte`
(disparador «…; escena final»), una pieza que ya no existe (§19 rev. 4); `Props-y-Sonidos-Resultados.md`,
fila de `N3_EscenaFinal.asset` («No tiene ambiente ni ningún sonido»). El guion §1.9 —la familia
camina hacia las fogatas, con el río, el bosque y la cueva detrás— se cumple.

**Qué hacía el juego.** `N3_EscenaFinal.asset` tenía `Ambient` y `AmbientLayer` vacíos: la última
escena del juego no sonaba.

**Decisión de Santiago, 30/09/2026 (acta D10).** La escena final suena al bosque de día, con el
crepitar de las fogatas en una segunda capa.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (01/10/2026).** Documentos: Dirección de sonido: §5 (S3); §8 filas
`amb_n1_cueva_fuego`, `amb_n2_bosque_dia` y `amb_viento_horizonte`; §19, piezas cableadas y rev. 5 ·
OE4: S-N3, escena final · registros de tareas: `Props-y-Sonidos-Resultados.md` y
`Slice-3-Resultados.md` (notas fechadas). Tareas de código: D10-3. `N3_EscenaFinal.asset` lleva
`Ambient` = `amb_n2_bosque_dia` y `AmbientLayer` = `amb_n1_cueva_fuego`, el mismo par que
`N2_PuenteI`; no hace falta código, porque
`NarrativeSceneController.Begin` ya pide los dos. Prueba:
`RiverSounds_RF44_LaEscenaFinalSuenaAlBosqueConLasFogatas`. Con ambiente, el silencio S3 de la
dirección de sonido §5 —un segundo con el ambiente al mínimo antes de la última frase de Algoritm—
ya no se cumple por omisión y todavía no se aplica: queda en «Residuos y puntos abiertos». Si
`amb_n1_cueva_fuego` suena a cueva y no a fogata al aire libre se comprueba de oído (guion H6 de
`claudeDocs/tasks/OE4/Hoja-HUM.md`).

### INC-126 · Cinco archivos de arte con nombre fuera de la nomenclatura — cerrado (01/10/2026)

**El conflicto.** Cuatro entornos conservaban el prefijo `entorno_` y una caja llevaba tilde, contra
la nomenclatura de §15.4.

**Qué decían los documentos.** `Direccion_de_Arte.md` §15.4 (prefijo `env_`; «Sin tildes, sin
espacios, sin mayúsculas»); `Inventario.md` §`Environments/` (los que conservan el prefijo viejo «hay
que renombrarlos … desde el motor») y §`Props/Wheel/`; `Props-y-Sonidos-Resultados.md` («El nombre
lleva tilde, contra §15.4»); `CLAUDE.md`, que cita `entorno_n1_apertura` en la regla de la costura de
3840, y el comentario de `MazeLayout.cs`. INC-66 e INC-82 citan los nombres viejos y quedan como
registro.

**Qué hacía el juego.** `entorno_n1_apertura`, `entorno_n1_cueva_2x`, `entorno_n1_cueva_cenital`,
`entorno_n2_laberinto` y `prop_n2_caja_suelo_vacía`: los cinco nombres que el informe de
`arte_check.py` del 30/09/2026 marcó entre 96 PNG.

**Decisión de Santiago, 30/09/2026 (acta D10).** Los cinco se renombran desde el motor, sin perder su
identificador, y una prueba nueva vigila la regla.

**Regla (b).** Solo en el documento: se implementa en el juego.

**Corrección aplicada (01/10/2026).** Documentos: Dirección de arte: §15.4 · Inventario de arte:
`Environments/`; `Props/Wheel/` · CLAUDE.md: regla de la costura de 3840 · el comentario de
`MazeLayout.cs`. Tareas de código: D10-4. `env_n1_apertura`, `env_n1_cueva_2x`, `env_n1_cueva_cenital`, `env_n2_laberinto` y
`prop_n2_caja_suelo_vacia`, con `AssetDatabase.RenameAsset` —primero en simulación— y el GUID
intacto. Los cuadros del fuego y del humo conservan sus nombres de entrega, la excepción que §15.4 ya
escribe. Prueba: `ArtImport_RNF23_LosNombresSiguenLaNomenclatura`.

### INC-127 · Los glifos del menú de pausa son de Phosphor Icons y los créditos los daban por originales — cerrado (01/10/2026)

**El conflicto.** Los créditos daban por originales del proyecto toda la interfaz, y tres de sus
glifos son de una familia de iconos con licencia MIT.

**Qué decían los documentos.** OE1 §1.2 CT-09 y §4.6 RNF-23: los recursos son de creación propia o
cuentan con autorización escrita, «con reconocimiento expreso en los créditos»; `Inventario.md`
§`UI/Common/`, nota «Licencia de los tres glifos de pausa (17/09/2026)», que dejaba a Santiago
acreditarlos o sustituirlos; `Slice 1/todo.md` §Assets visuales («Entornos, props e interfaz son
originales del proyecto»).

**Qué hacía el juego.** `ui_pausa`, `ui_reanudar` y `ui_reiniciar` son los iconos `pause`, `play` y
`arrow-counter-clockwise` de Phosphor Icons rasterizados a 128 px, y el cuerpo de
`CreditsContent.asset` decía «Entornos, objetos e interfaz: originales del proyecto.».

**Decisión de Santiago, 30/09/2026 (acta D10).** Se acreditan.

**Regla (b).** Solo en el documento: se implementa en el juego.

**Corrección aplicada (01/10/2026).** Documentos: Dirección de arte: §19 «Nota legal» · Inventario de
arte: `UI/Common/` (se cierra la nota de licencia) · Interfaces: §1.1, punto 6 «Pausa». CT-09 y
RNF-23 ya lo pedían y no cambian. Tareas de código: D10-4. Las dos últimas oraciones de
`CreditsContent.asset` dicen «Entornos, objetos e interfaz: originales del proyecto, salvo los iconos
de pausa.» e «Iconos de pausa: Phosphor Icons, licencia MIT.»: nombran Phosphor Icons y su licencia
como `Credits.unity` nombra las tipografías y su licencia SIL OFL 1.1, sin pasar de 20 palabras
(`CreditsContent_RNF01_NingunaOracionSupera20Palabras`), y la interfaz sigue constando como original
salvo los tres glifos. Se completó además (01/10/2026): la MIT y la OFL piden que el aviso acompañe
la copia distribuida, así que `LicenseNotices` (`Game.EditorTools`) copia `LICENSE-Phosphor.txt` y
`OFL.txt` a `Licencias/`, junto al ejecutable, al terminar cada build de Windows, y lo hace fallar si
falta uno (`LicenseNotices_RNF23_ElEjecutableLlevaLasLicenciasDeTerceros` y
`LicenseNotices_RNF23_SiFaltaUnAvisoElBuildFalla`).

### INC-128 · La dirección de arte fijaba otros ajustes de importación que los que aplica el proyecto — cerrado (01/10/2026)

**El conflicto.** §15.2 pide comprimir, limitar a 2048, cortar hojas en `Multiple` y poner el pivote
abajo; el proyecto importa sin comprimir, a 4096 y en `Single`.

**Qué decían los documentos.** `Direccion_de_Arte.md` §15.2: Compression «Normal Quality», Max Size
2048, Sprite Mode «Single (personajes) / Multiple (hojas de props)» y Pivot «Bottom (personajes) /
Center (props)»; `Slice 1/todo.md` §Assets visuales («import con los ajustes de §15.2…»).

**Qué hace el juego.** `ArtImportRules` (`Game.EditorTools`) importa todo `Assets/Game/Art/` sin
comprimir y a 4096 —comprimida, la ilustración plana enseña la rejilla de bloques de 4×4, y la capa
de oscuridad del Nivel 1 la amplifica—, y lo vigila
`ArtImport_RNF23_LasIlustracionesEntranSinComprimirYSinReducir`. Cada imagen pasa a `Single` desde
el motor, porque con `Multiple`, el valor de fábrica, `LoadAssetAtPath<Sprite>` devuelve nulo. En
uGUI el punto de apoyo lo fija el `RectTransform`, no el importador: en los rigs, cada parte gira
sobre su articulación (§13.1, INC-53).

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (01/10/2026).** Documentos: Dirección de arte: §15.2 describe esa regla y su
razón · Inventario de arte: cabecera (cada pieza entra en `Single`), `Environments/` y `Props/Wheel/`
(los que siguen en `Multiple`). **Excepción, decidida por el orquestador por delegación de Santiago (01/10/2026):** doce PNG
del Nivel 2 llegaron en `Multiple` con un solo sprite (`<nombre>_0`) —`env_enlace_n2`,
`prop_n2_herramienta_a` a `_c`, `prop_n2_piedra_a` a `_d`, `prop_n2_planta_a` a `_c` y
`prop_n2_tronco_a`—. Se quedan en `Multiple` los diez que escenas y assets referencian por ese
sub-sprite: pasarlos a `Single` cambia el fileID de su sprite a 21300000 y rompería esas
referencias. `prop_n2_piedra_c` y `_d` no los referencia nada y siguen también en `Multiple`: conviene
pasarlos a `Single` antes de que una escena o un asset los use. Se completó además (01/10/2026): los
cuadros del fuego y del humo de `Props/Fire/Animations/` entran a 1024 px como máximo, la única
excepción al tope de 4096 (INC-130).

### INC-129 · El diálogo se lee en un cuadro con retrato, no en un globo con cola — cerrado (01/10/2026)

**El conflicto.** La dirección de arte e Interfaces describen el diálogo de las narrativas como un
globo con cola que apunta al hablante; el juego lo pone en un cuadro fijo con el retrato.

**Qué decían los documentos.** `Direccion_de_Arte.md` §10.3 «Globos de diálogo» (forma ovalada,
contorno de 6 px, cola triangular hacia el hablante), §11.4 («Máximo 2 líneas por globo de diálogo»)
y §4.3 (marfil, «Fondo de globos de diálogo y paneles»); `Interfaces.md` §1.1, punto 5 («Globo de
diálogo (§10.3), retrato del hablante…»).

**Qué hace el juego.** `CuadroDialogo`, en `Narrative.unity`, ocupa el cuarto inferior de la pantalla
con `Marco`, `Retrato`, `Hablante`, `Cuerpo`, `BotonContinuar` y `BotonOmitir`, sin cola: es el
«Panel de diálogo» de §10.2.

**Regla (a).** Conflicto: gana el juego y se corrige el documento.

**Corrección aplicada (01/10/2026).** Documentos: Dirección de arte: §4.3 fila «Marfil»; §10.3, que
pasa a «Cuadro de diálogo» y lo describe; §11.4 · Interfaces: §1.1, punto 5. La regla del texto
`#3A1E18` sobre marfil no cambia.

**Nota (08/10/2026).** El cuadro mide hoy 1400 × 216 px y su texto es `#1F100C` a 30 px (INC-138).

### INC-130 · El paquete de entrega superaba los 500 MB por los cuadros del fuego y del humo del Nivel 1 — cerrado (01/10/2026)

**El conflicto.** RNF-06 limita el ejecutable y sus recursos a 500 MB, y con la regla de importación
que fijan los documentos —todo `Assets/Game/Art/` sin comprimir y a 4096, sin excepción (INC-128)—
el paquete de entrega pesaba 866,7 MB.

**Qué decían los documentos.** OE1 §4.2 RNF-06 («El tamaño total del ejecutable y sus recursos no
debe superar los 500 MB»), que no cambia; `Direccion_de_Arte.md` §15.2 (fila «Max Size» y párrafo
«Sin comprimir y a 4096, por regla»); `Inventario.md`, párrafo «Cómo entran», y `CLAUDE.md`, «Cómo
entra una imagen», que dicen lo mismo para todo `Art/`; la prueba
`ArtImport_RNF23_LasIlustracionesEntranSinComprimirYSinReducir`, que lo exigía a toda textura de
`Art/`.

**Qué hacía el juego.** El primer ejecutable de entrega, del 01/10/2026, pesó 866 718 251 bytes
(866,7 MB; 826,6 MiB). `Props/Fire/Animations/` sumaba 533 MB en 67 cuadros sin comprimir y a
tamaño completo —33 de humo de 1123 × 1933, 13 de fuego normal de 2144 × 2108 y 21 de fuego
cenital de 500 × 278—, que son 134 entradas en el informe del build. Ya eran un dibujo por clave, sin
duplicados (la regla de las secuencias de cuadros), así que no había arreglo sin pérdida.

**Decisión del orquestador por delegación de Santiago (01/10/2026).** Los cuadros de
`Props/Fire/Animations/` se importan a 1024 px de lado como máximo y siguen sin comprimir:
comprimidos enseñarían la misma rejilla de bloques de 4×4 que evita la regla general. En pantalla el
fuego se ve a unos 650 px como mucho y el humo a menos de 300, así que la reducción no se nota: la
revisión comparó las capturas de antes y de después, ampliadas sin interpolar, y no halló pérdida
visible.

**Regla (a).** Conflicto: gana el juego y se corrige el documento. RNF-06 ya lo pedía y no cambia.

**Corrección aplicada (01/10/2026).** Documentos: Dirección de arte: §15.2 (fila «Max Size» y la
excepción) · Inventario de arte: «Cómo entran»; `Props/Fire/Animations/` · CLAUDE.md: «Cómo entra una
imagen» · INC-128 (nota). En el código, `ArtImportRules` (`Game.EditorTools`) da `maxTextureSize`
1024 a lo que está bajo `Assets/Game/Art/Props/Fire/Animations/` (`FireFramesMaxTextureSize`) y deja
el resto a 4096; los 67 `.meta` se reimportaron desde el motor. En memoria el humo queda en
595 × 1024 y el fuego normal en 1024 × 1007, y el cenital no cambia; el PPU se escala con la
textura, así que el tamaño en el mundo es el mismo. La carpeta pasa de 533 MB a 145,7 MB y el
paquete a 478 979 915 bytes (479,0 MB; 456,8 MiB), con 21 MB de margen
(`claudeDocs/tasks/OE4/evidencias/build-rc1.md`). Una entrega nueva de cuadros entra sola a 1024: los
`.png` de la entrega no se reducen a mano. Pruebas:
`ArtImport_RNF06_LosCuadrosDelFuegoYElHumoSeImportanAMil24SinComprimir`, nueva, y
`ArtImport_RNF23_LasIlustracionesEntranSinComprimirYSinReducir`, que sigue exigiendo que esa carpeta
entre sin comprimir y solo a ella le admite otro tope. La memoria (RNF-05) se mide sobre el
ejecutable.

### INC-131 · El arte final trae seis expresiones y extremidades en dos tramos — cerrado (05/10/2026)

**El conflicto.** Santiago decidió el 05/10/2026 que los personajes finales llegan en vista frontal
con más capas, y esa decisión contradice dos reglas que los documentos fijaban para el arte actual:
**INC-108** («una sola cara, la neutra; la emoción la lleva el cuerpo») y la de Algoritm de «una
sola imagen por forma, sin recorte en partes» (`Direccion_de_Arte.md` §7.6 y §13.1, `Inventario.md`).

**Qué decían los documentos.** `Direccion_de_Arte.md` §7.3 (una sola cara), §7.6 «Nomenclatura» (una
sola imagen por forma) y §13.1 (cinco partes por miembro de la familia; Algoritm, una `Image`);
`Interfaces.md` §4 (ROSTRO) y §4.2; `Inventario.md`, `Characters/` (retratos y Algoritm).

**Qué hace el juego.** Hasta ahora, cinco partes por miembro de la familia con la cara dentro del
torso, y una `Image` por forma de Algoritm. `Plan-Personajes-Finales.md` §5 ya pedía seis
expresiones, que INC-108 había rechazado.

**Decisión de Santiago (05/10/2026).**
- Papá, Mamá, Niña y Niño: vista frontal; **cabeza separada del torso**, con **ojos y boca en capas
  propias**; seis expresiones (Neutral, Happy, Surprised, Worried, Focused, Sleeping), parpadeo y
  cuatro bocas del habla; **extremidades en dos sprites** —húmero y antebrazo, muslo y antepierna—,
  con codos y rodillas además de hombros, cadera y cuello.
- Algoritm, en sus tres formas: brazos, piernas, codos y rodillas en dos tramos, y ojos y boca
  sobre el cuerpo, **sin cuello**.
- Mientras no llegue el arte, codos, rodillas y cuello son pivotes vacíos y las capas nuevas van
  apagadas: el arte provisional se ve igual.
- Nombres: se conservan los actuales (`char_<x>_parte_brazo_*` = húmero, `pierna_*` = muslo, mismo
  GUID) y se añaden `antebrazo_izq/der`, `antepierna_izq/der` y `cabeza`; la cara es
  `char_<x>_ojos_{neutra,alegria,sorpresa,preocupacion,concentracion,sueno}`,
  `_ojos_parpadeo_{medio,cerrado}`, `_boca_{0,a,e,u}` y `_boca_{alegria,sorpresa,preocupacion,concentracion}`;
  Algoritm lleva el prefijo `char_algoritm_<fuego|rueda|gota>_`. Esto sustituye las carpetas
  `Front/` y los nombres `frente_` de `Plan-Personajes-Finales.md` §4.2.
- **No cambia la regla de fondo**: ninguna expresión es de tristeza, enfado o derrota (CP-02), y
  `Worried` es duda, sin lágrimas ni mueca de llanto. Un intento sin éxito sigue produciendo el
  ánimo, ahora con la cara alegre.

**Regla.** Decisión de Santiago, como INC-115 a INC-117: levanta INC-108 y la regla de Algoritm
**para el arte final**; para el arte actual, ambas siguen valiendo. No hay `.docx` radicado que lo
diga, y ninguno se edita desde código.

**Corrección aplicada (05/10/2026).** Documentos: Dirección de arte: §7.3 (las seis expresiones, el
límite y la regla de no tristeza), §7.6 «Nomenclatura» (Algoritm deja de ser «sin recorte en partes»
para el arte final) y su introducción, §13.1 (diez partes y una cara; nombres), §15.4 (ejemplos de
nomenclatura) y la lista de §17 «Personajes» · Interfaces: §4 (ROSTRO) y §4.2 · Inventario de arte:
`Characters/` (cómo se animan, retratos, Algoritm y la tabla de nombres nuevos) · Plan de personajes
finales: estado, Fases 2 y 3, §3.1, §4.2, §5 y §7 · `Personajes-Resultados.md`: Anexo C · CLAUDE.md:
la viñeta de personajes, el párrafo del carril de arte y esta fila. En el código (commit `a12dbb3`,
rama `feat/personajes-animados`): `FacialEmotion`, `ActionEmotion`, `BlinkClock`, `MouthFlap`,
`CharacterFaceSet` y `CharacterFace`; `CharacterRig`, `ActorBeat`, `ActorCue`, `ActorTimeline` y
`NarrativeSceneController` ganan la emoción y el habla sin cambiar sus firmas; y el generador
`BuildRigsFinal.cs.txt`, con `rig_articulaciones.json` y `articulaciones.py`, que añade los nodos a
los siete prefabs sin reconstruirlos y reescribe los 93 clips. **Pruebas:** `CharacterRig_DA131_*`,
`CharacterRig_DA73_*`, `FacialEmotion_*`, `BlinkClock_*`, `MouthFlap_*`, `CharacterFace_*`,
`ActorTimeline_RF05_*` y `NarrativeScene_RF05_QuienHablaHablaYCallaAlAvanzar`. **Verificación en el
Editor: pendiente** (los prefabs y los clips aún no se han regenerado ni las pruebas se han
corrido).

*Nota (09/10/2026): el parpadeo de tres cuadros (`ojos_parpadeo_medio`) lo acota INC-135: la entrega trae solo
ojos abiertos y cerrados, y el cuadro medio es opcional. Las nueve partes de Algoritm que este hallazgo
pedía (torso y cuatro extremidades en dos tramos) las sustituye INC-136: siete piezas por forma, con la
pierna entera y sin rodilla.*

### INC-132 · Orden de dibujo de los brazos: detrás del torso y delante de la cabeza — cerrado (05/10/2026)

**El conflicto.** Santiago observó que el Idle frontal era pobre y que los brazos se escondían detrás
de la cabeza o del cuerpo. La causa estaba en el prefab, no en los clips: bajo `Lienzo/Cuerpo/Tronco`
los hijos iban `BrazoIzq`, `BrazoDer`, `Torso`, `Cuello` (uGUI pinta de atrás adelante), y el hombro
del Niño pivotaba en el centro del pecho.

**Decisión de Santiago (05/10/2026), definitiva.** En la **familia** (Papá, Mamá, Niña y Niño) los
brazos se dibujan **detrás del torso y delante de la cabeza**: `orden_tronco` = `Cuello`, `BrazoIzq`,
`BrazoDer`, `Torso`, con la cabeza al fondo. Con el arte provisional la cabeza está pintada dentro del
torso, así que hoy los brazos quedan tras el torso y tras la cara hasta que llegue el arte final.
(Etapas intermedias del mismo día: brazos delante en el Niño y Algoritm → delante en los siete,
commit `5ce0797` → regla definitiva.)

**Ajustes del 06/10/2026, también decisión de Santiago.**
- **Strike.** Mientras el personaje golpea las piedras, los brazos pasan **delante del torso**, y al
  cambiar de acción vuelven exactamente a su orden. Lo hace `ArmLayering` (C# plano,
  `Game.Scaffolding`) desde `CharacterRig.Apply`, según el campo serializado
  `armsInFrontActions = { Strike }` (el inicializador vale para los prefabs que no lo serializan); el
  cambio de capa es seco al empezar la acción, sin esperar el fundido de 0,18 s.
- **Algoritm.** `orden_tronco` = `Torso`, `Ojos`, `Boca`, `BrazoIzq`, `BrazoDer`: sus brazos van delante
  del cuerpo y **las manos se pintan encima de la cara**. La coreografía y `pose_preview.py` impiden
  que una mano entre en la caja de ojos y boca (ampliada un 30 %, con al menos el 99,5 % libre). En
  Algoritm el golpe no mueve nada.
- **Coreografía de Strike.** El choque va delante del pecho con brazo partido; con el brazo de una
  pieza del arte provisional (Papá, Mamá y Niña hoy) el brazo se encoge hasta el 45 % durante el golpe
  para que el choque quede en el vientre, nunca bajo la cintura (por encima de la cabeza tapaba la
  cara y a un costado no había contacto). **Queda pendiente de que Santiago lo apruebe**; la
  alternativa es golpear sin juntar las manos hasta el arte final.

**Regla.** Decisión de Santiago, como INC-115 a INC-117 e INC-131. **Sustituye** para la familia la
regla de INC-131 de que el cuello se dibuja sobre el torso (`CharacterRig_DA131_…` ya no la exige). No
contradice ningún `.docx` radicado ni cambia CP-02: `Encourage` sigue siendo un puño arriba, ahora
fuera de la cabeza.

**Corrección aplicada (05/10 y 06/10/2026).** Código y herramientas (commits `8740a64`, `6f2b4cf`,
`590866f`, `d817d95`, `5ce0797`, `0b76bbc`, `117287c`, `a5a887b` y `8cdd4c5`, rama `feat/personajes-animados`): clave `orden_tronco` en
`rig_articulaciones.json` y modo nuevo `"orden"` en `BuildRigsFinal.cs.txt` (`SetSiblingIndex`, sin
tocar fileIDs); la coreografía pasa del C# a `coreografia.py`, que escribe `clips_personajes.json`, y
el modo `"clips"` solo lo aplica. La coreografía deriva la visibilidad de la geometría (silueta del
torso por brazo) y calcula el reposo por personaje; el Idle es de 6,4 s (dos respiraciones de 3,2 s,
que conservan el ciclo de `Direccion_de_Arte.md` §13.3). Geometría del Niño corregida (hombros, codos,
rodillas, ojos y boca). `pose_preview.py` (que mide también que el torso no tape más del 5 % de ojos y
boca, que ninguna mano entre en la cara de Algoritm y que el choque de `Strike` quede sobre la cintura;
sin excepciones, 174/174), `maqueta.py` y
`preparar_arte_final.py` verifican fuera de Unity y preparan la entrada del arte final. Documentos:
`Personajes-Resultados.md` (C.5, C.6 y C.9), `Plan-Personajes-Finales.md` (§3.1 y Fase 3) y CLAUDE.md.
**Pruebas:** `CharacterRig_INC132_LosBrazosSeDibujanDetrasDelTorsoYDelanteDeLaCabeza`,
`CharacterRig_INC132_AlgoritmPintaLasManosEncimaDeLaCara`,
`CharacterRig_INC132_AlGolpearLosBrazosPasanDelanteDelTorso`,
`…_AlTerminarElGolpeLosBrazosVuelvenDetrasDelTorso`, `…_EnAlgoritmElGolpeNoCambiaElOrdenDeDibujo`,
`…_SoloElGolpePoneLosBrazosDelante` y `…_SiFaltaUnNodoElGolpeAvisaYNoMueveNada`.
**En el Editor:** `Game.Scaffolding.Tests` 215/215 y suite completa 941/942 (1 omitida preexistente, 0
fallos) en la ronda de `5ce0797`, que aún tenía los brazos delante en los siete. Suite completa de la
ronda de `0b76bbc` (regla definitiva), con sus prefabs y `.anim` subidos en `117287c` (4 prefabs, 77
`.anim`): `Game.Scaffolding.Tests` 215/215 y suite completa en un solo Editor 940/942 (1 omitida
preexistente, 1 fallo: `RiverLevel_RNF05`, por la memoria de un Editor abierto todo el día, 2199 MB;
con el Editor recién abierto mide 1533 MB reservados y 1072 asignados, y
`Game.Levels.River.PlayMode.Tests` pasa 57/57). Ronda de `8cdd4c5` (Strike con los brazos delante y las
manos de Algoritm sobre la cara; 3 prefabs de Algoritm, 4 `.anim` de golpear y el `.meta` de
`ArmLayering.cs`): `Game.Scaffolding.Tests` 234/234 y suite completa en un solo Editor 960/961 (1
omitida preexistente, 0 fallos; `ForestScene_RF22` y `RiverLevel_RNF05` en verde).

### INC-133 · Antebrazo delante del torso, de la cara y de las piernas — cerrado (06/10/2026)

**El conflicto.** Con el arte final de la familia, el brazo en dos tramos pedía dos órdenes de dibujo
distintos, y uGUI pinta por orden de jerarquía: con INC-132 el brazo entero iba detrás del torso, y
el antebrazo quedaba tapado por el cuerpo cuando debía verse (cara, torso y piernas por detrás de la
mano).

**Decisión de Santiago (06/10/2026).** En la familia (Papá, Mamá, Niña y Niño) el **húmero** se dibuja
detrás del torso y el **antebrazo** delante del torso, de la cara y de las piernas. Papá, además, con
la cabeza (`Cuello`) delante del torso. **Acota INC-132** en la familia, que sigue vigente para
`Strike` (el húmero pasa delante del torso al golpear, siempre detrás de los antebrazos) y para
Algoritm, donde no hay anclas ni cambio.

**Corrección aplicada (commit `ef37dd9`, rama `feat/personajes-animados`).** `AntebrazoX` (mismo
objeto y mismo fileID) pasa a ser hijo de `Tronco`, al final de su lista, y sigue a un nodo vacío,
`AnclaAntebrazoX`, que queda bajo `CodoX` con la pose local anterior del antebrazo. `LimbFollower`
(C# plano, `Game.Scaffolding`) compone `BrazoX`, `CodoX` y el ancla respecto de `Tronco` y escribe la
pose local del antebrazo; `CharacterRig` descubre los pares por nombre y los sincroniza tras
`Play` + `Update(0)` y en `LateUpdate`. No se añade ningún campo serializado y los clips no cambian
(no animan el antebrazo). Los personajes sin anclas (arte provisional, Algoritm) no se tocan.
`ArmLayering` no cambia de lógica. El modo `"orden"` de `BuildRigsFinal.cs.txt` crea las anclas y mueve
los antebrazos cuando `orden_tronco` los lista, sin reconstruir nada. El commit `233f2e5` pone
`AntebrazoIzq` y `AntebrazoDer` al final de `orden_tronco` de la familia y separa en `pose_preview.py` la
visibilidad del húmero (40 %) y del antebrazo (85 %).
**Pruebas:** `CharacterRigLimbTests` (jerarquía armada en código; pasan al escribirse) y
`CharacterRig_INC133_*` sobre los prefabs reales; `CharacterRig_DA131_*` y `CharacterRig_INC132_*` se
ajustaron para aceptar la jerarquía nueva. Las de prefabs fallan hasta correr el modo `"orden"` en el
Editor. **Verificación en el Editor** (`433603f`, 06/10/2026): el generador creó las anclas y soltó los
antebrazos en los cuatro prefabs de la familia (cuatro objetos nuevos por prefab, ninguno perdido) y
reescribió 93 clips; las pruebas del antebrazo y del ancla pasan 14 de 14, EditMode 627 pasan con 1
omitida y PlayMode 376 de 377 (falla `RiverLevel_RNF05` por la memoria del Editor, 2 161 MB); capturas en
`claudeDocs/tasks/Personajes/capturas/2026-10-06/`. Fuera del Editor: `pose_preview.py` 21/21 por miembro de la familia y 9/9 por
Algoritm (`233f2e5`).

### INC-134 · Los personajes se ven de perfil al recorrer o trabajar el entorno — cerrado (09/10/2026)

**El conflicto.** Con la llegada del arte de perfil, Santiago decidió el 09/10/2026 que el personaje se ve
**de perfil siempre que se mueve en el entorno** y de frente en reposo. Eso contradice la dirección de arte
(§13.3: «Caminar: un solo ciclo que se voltea a izquierda o derecha según la dirección») y la lectura del
inventario de arte de que Mamá camina en el río «con un solo clip volteado».

**Qué decían los documentos.** `Direccion_de_Arte.md` §13.3 (el ciclo único que se voltea) y §13.1 (un solo
cuerpo por personaje); `Assets/Game/Art/Inventario.md` (`Characters/`, la nota de `char_mama_cenital` y el
hallazgo 5 de los pendientes); `Plan-Personajes-Finales.md` §4 (el perfil izquierdo «derivado mediante
simetría controlada», `CharacterOrientation` y Fase 4.4, sin ejecutar) y su §4.3 (el orden de dibujo del
perfil, con la pierna cercana delante del torso).

**Qué hace el juego.** Hasta ahora, un solo cuerpo de frente. `CharacterRig.Mirrored` volteaba el lienzo
entero y solo lo usaba el río, y solo en el eje horizontal (una flecha vertical conservaba el último lado);
las narrativas volteaban la casilla con `NarrativeProp.Mirrored`, fijo para toda la escena, y nadie
calculaba hacia dónde camina el actor.

**Decisión de Santiago (09/10/2026).**
- **La vista la decide la acción, no el desplazamiento** (`ActionView`, C# plano): `Walk`, `Run`, `Carry`,
  `Push`, `PickUp`, `Kneel` y `Blow` muestran el cuerpo de perfil (`Lienzo/Perfil`); las demás acciones,
  incluida `Wave`, el de frente, con corte seco. Así la balsa (`Idle` mientras se mueve) y Algoritm
  flotando siguen de frente, y los gestos hacia el estudiante —hablar, señalar, animar— nunca se dan de
  costado (CP-02).
- **El lado** lo da una sola función pura, `Heading.FacesLeft`: a la izquierda o hacia arriba, mira a la
  izquierda; a la derecha o hacia abajo, a la derecha; en una diagonal manda el eje dominante y en un empate,
  el horizontal. «Arriba» es y positiva, como en las flechas del río.
- **Narrativas.** Cada paso decide su lado con `ActorTimeline.FacesLeftAt`, en este orden: el
  `ActorBeat.Facing` explícito (`Auto`, `Left`, `Right`; campo nuevo al final del beat, de modo que los 18
  assets no cambian), el desplazamiento del paso si mide al menos 0,01, el rumbo del paso anterior, el del
  siguiente y, si nada decide, hacia el centro de la ilustración. El resultado se combina con un O exclusivo
  con el volteo de la casilla (`NarrativeProp.Mirrored`), para que el perfil no se espeje dos veces.
- **Mecánicas.** En el río, Mamá sigue las flechas y las verticales también la giran (arriba a la
  izquierda, abajo a la derecha); en el Nivel 1, Papá mira hacia el montón al recoger, soplar y arrodillarse;
  en el bosque, la Niña mira hacia la caja que empuja y se da vuelta si la caja cruza al otro lado. El
  laberinto y el taller no tienen acciones de perfil.
- **El frente nunca se espeja por el rumbo.** `Mirrored` solo voltea el lienzo en perfil. Un rig sin torso de
  perfil con sprite (Algoritm, que no tiene arte de perfil, o un personaje antes de que entre el suyo) se
  queda de frente en cualquier acción.
- **El arte** llegó mirando a la **izquierda** y la ingesta lo espeja para que el canónico mire a la
  derecha; el ancla horizontal del corte frente↔perfil es la **cadera** (x = 512 del lienzo del rig), para que
  los pies no resbalen al girar; el rastro casi blanco del ojo abierto que quedó bajo los párpados cerrados se
  limpia solo. La mano lejana de la Niña, que faltaba, se pidió al artista y no se sintetizó; llegó el mismo
  día.
- **Orden de dibujo del tronco de perfil**, de atrás adelante: brazo lejano, pierna lejana, pierna cercana,
  torso, cuello y cabeza (con su cara) y brazo cercano. **Las dos piernas van siempre detrás del torso**:
  la primera versión dibujaba la pierna cercana delante de él y Santiago lo corrigió el mismo día. De frente
  ya se cumplía, porque las piernas son hijas de `Cuerpo`, que va antes de `Tronco`.

**Regla.** Decisión de Santiago, como INC-115 a INC-117 e INC-131 a INC-133: **levanta** el «ciclo único que
se voltea» de §13.3. No contradice ningún `.docx` radicado ni cambia CP-02, y los gestos de ánimo siguen de
frente. Cambia una prueba existente a propósito: `RiverScene_DA133_MamaCaminaMientrasSeSostieneUnaFlechaYReposaAlSoltarla`
exige ahora que las flechas verticales también giren a Mamá.

**Corrección aplicada (09/10/2026).** Documentos: Dirección de arte: §13.3 (fila «Caminar») y una subsección
nueva, §13.4, con las acciones, el rumbo, el orden de dibujo y los nombres del perfil · Inventario de arte:
`Perfil/`, `Characters/` y el hallazgo 5 · Plan de personajes finales: nota fechada y §4.3 · `Personajes-Resultados.md`:
C.13 · CLAUDE.md: el párrafo de personajes. En el código (`910c0f0`, rama `feat/personajes-animados`): `ActionView`,
`CharacterView`, `Heading` y `ActorFacing` nuevos; `CharacterRig` (`HasProfile`, `View`, y el espejo solo en
perfil; busca `Lienzo/Cuerpo` y `Lienzo/Perfil` por ruta, sin campos serializados nuevos), `CharacterFace`
(capas y set de la cara de perfil, con un solo reloj de parpadeo y de habla), `ActorBeat`, `ActorTimeline`,
`NarrativeSceneController` y los controladores del fuego, el bosque y el río; `123785e` pone `Wave` en la
tabla de vistas (de frente). En las herramientas (`claudeDocs/tasks/Personajes/herramientas/`, `beb1cfe`):
`preparar_perfil.py`, nuevo, y los modos y claves de perfil de `preparar_expresion.py`, `articulaciones.py`,
`coreografia.py`, `prefabs.py` y `BuildRigsFinal.cs.txt`. Arte: la entrega original en `3efa765`, la mano
de la Niña en `3a0f6ad`, su inventario en `a2d32e2` y los 55 PNG de `Perfil/` en `beb1cfe`. Ronda 1 del
Editor (`9966bcb`): `Lienzo/Perfil` en los cuatro prefabs de la familia (82 a 148 bloques, ningún fileID
perdido o cambiado), los sprites y las capas de cara de perfil cableados, los 84 clips de la familia
reescritos y los `.meta`. `2cd8a75` pone las piernas detrás del torso (`ORDEN_TRONCO_PERFIL`; en el Editor,
el modo `perfil` reordena los hijos existentes con `SetSiblingIndex`, sin cambiar fileID).
**Pruebas** (31 métodos nuevos de INC-134; los de INC-135 van en su entrada):
`ActionView_INC134_LaTablaCubreTodasLasAcciones`, `ActionView_INC134_LosGestosHaciaElEstudianteVanDeFrente`,
`Heading_INC134_IzquierdaYArribaMiranALaIzquierdaDerechaYAbajoALaDerecha`,
`Heading_INC134_UnVectorNuloNoDecideNada`, `Heading_INC134_EncararUnObjetivoMiraHaciaSuLado`,
`CharacterRig_INC134_AlMoverseMuestraElPerfilYEnReposoElFrente`,
`CharacterRig_INC134_LaVistaLaDecideLaAccionYNoElDesplazamiento`,
`CharacterRig_INC134_SinCuerpoDePerfilSiempreDeFrente`, `CharacterRig_INC134_SinArteDePerfilSiempreDeFrente`,
`CharacterRig_INC134_SinLienzoNoLanza`, `CharacterRig_INC134_ElEspejoSoloVolteaElPerfil`,
`CharacterRig_INC134_ElLadoNoCambiaElTamanoDelLienzo`,
`CharacterRig_INC134_LaFamiliaTieneCuerpoDePerfilConArte` (que comprueba además el orden de dibujo, con las
piernas detrás del torso), `CharacterFace_INC134_ElPerfilParpadeaAlCompasDelFrente`,
`CharacterFace_INC134_SinCaraDePerfilLaDeFrenteSigueYLaDePerfilNoSeDibuja`, nueve
`ActorTimeline_INC134_*` y `ActorBeat_INC134_PorDefectoElRumboEsAuto` en EditMode;
`NarrativeScene_INC134_QuienSeDesplazaVaDePerfilHaciaDondeCamina` (un caso por secuencia),
`RiverScene_INC134_MamaVaDePerfilAlCaminarYDeFrenteAlSoltarLaFlecha`, tres `FireLevel_INC134_*` y
`ForestScene_INC134_LaNinaMiraALaCajaMientrasLaEmpujaYSeGiraSiLaCajaCruza` en PlayMode.
**Verificación en el Editor: ronda 1 hecha, ronda 2 pendiente.** Tras `9966bcb`, `Game.Scaffolding.Tests` pasa
351/351 y `NarrativeScene_` 107/107 en PlayMode. Falta correr con el orden nuevo de las piernas el modo
`perfil` (y `sprites` y `clips`), el resto de los grupos de PlayMode, `suite2` y las capturas
(`Personajes-Resultados.md`, C.13).

### INC-135 · El parpadeo es de dos cuadros: ojos abiertos y ojos cerrados — cerrado (09/10/2026)

**El conflicto.** El arte de perfil trae la cara en dos cuadros, **ojos abiertos** y **ojos cerrados**, sin
párpado a medias, y la dirección de arte pide un parpadeo de tres: «ojos a medio cerrar y cerrados».

**Qué decían los documentos.** `Direccion_de_Arte.md` §7.3 (ojos a medio cerrar y cerrados para el
parpadeo); `Plan-Personajes-Finales.md` §5.2 (el parpadeo procedural pasa por medio cerrado); INC-131 y el
inventario de arte (`ojos_parpadeo_medio` y `ojos_parpadeo_cerrado`).

**Qué hace el juego.** `BlinkClock` recorre medio cerrado, cerrado y medio cerrado en 0,12 s; con solo el
cuadro cerrado, `CharacterFaceSet.Eyes(Half)` no devolvía nada y los ojos cerrados se verían apenas 0,04 s,
el tercio central.

**Decisión de Santiago (09/10/2026).** El parpadeo es de **dos cuadros**: «ojos abiertos» es `ojos_neutra` y
«ojos cerrados» es `ojos_parpadeo_cerrado`. `CharacterFaceSet.Eyes(Half)` devuelve el cuadro cerrado cuando
no hay cuadro medio (comparación explícita con `null`, que respeta los objetos destruidos), de modo que el
parpadeo enseña los ojos cerrados sus 0,12 s completos. `ojos_parpadeo_medio` queda **opcional**: si algún
día llega, vuelve a ser el cuadro medio. `preparar_expresion.py` acepta `abiertos` y `cerrados` como alias de
`neutra` y `parpadeo_cerrado`. **Hoy solo parpadea el perfil:** no hay ojos cerrados de frente en la entrega
(ni en `Assets/`) y el frente no parpadeará hasta que llegue `Expresiones/char_<x>_ojos_parpadeo_cerrado`,
que se pidió al artista. Algoritm tampoco parpadea mientras lleve la cara provisional (INC-136).

**Regla.** Decisión de Santiago. **Acota** §7.3 y el segundo cuadro de INC-131: el parpadeo de tres cuadros
vale solo mientras exista el cuadro medio. Mantiene CP-02 (el parpadeo no es una expresión) y el ciclo de
3,5 ± 1,2 s y 0,12 s.

**Corrección aplicada (09/10/2026).** Documentos: Dirección de arte: §7.3 y la lista de §17 · Inventario de
arte: la tabla del arte final (`ojos_parpadeo_cerrado` y `ojos_parpadeo_medio` opcional) · `Personajes-Resultados.md`:
C.13 · CLAUDE.md. En el código (`910c0f0`): `CharacterFaceSet.Eyes(Half)`, con el tooltip del campo al día.
**Pruebas:** `CharacterFaceSet_INC135_SinCuadroMedioElParpadeoUsaElCerrado` y
`CharacterFaceSet_INC135_ConCuadroMedioSigueSiendoElMedioYSinNingunoNoSeInventaNada`; el parpadeo de la cara
de perfil, a compás con el del frente: `CharacterFace_INC134_ElPerfilParpadeaAlCompasDelFrente`.
**Verificación en el Editor:** `Game.Scaffolding.Tests` 351/351 tras `9966bcb`, con los sets de cara de perfil
ya generados; la captura del parpadeo en el perfil está pendiente de la ronda 2.

### INC-136 · El diseño nuevo de Algoritm es el oficial: llama, disco de madera y gota — cerrado (09/10/2026), salvo los radicados

**El conflicto.** El artista entregó el 09/10/2026 el diseño nuevo de Algoritm —21 PNG, siete piezas por
forma— y Santiago decidió que **es el oficial**. No es el de la dirección de arte ni el que INC-52 describió
con el sprite del 24/09/2026, y cambia de una vez los rasgos que §7.6 declara invariables.

**Qué decían los documentos.** `Direccion_de_Arte.md` §7.6, que fija el «núcleo de identidad»: una llama de
tres lenguas en los tres niveles, brazos y piernas de palo finos del color del contorno, manos abiertas color
piel, pies de trazo corto, **franja de cinco bandas** y contorno café `#3B1205`; «rueda» y «gota» eran nombres
de nivel y no de silueta (INC-52). §2.2: color plano, sin degradados, también para los personajes.
`Interfaces.md` §4.3 y la dirección de sonido §6.1 (la «cuenta de cinco» como eco de las cinco bandas). Los
radicados: el guion `Solucion_OE2_Diseno_final.docx` §1.1.1 («Conserva el mismo cuerpo en los tres niveles y
cambia de material», con la franja de colores) y, en las notas de las dos escenas puente que abren los
Niveles 2 y 3, «Su forma de llama no cambia»; y el trabajo de grado, que lo repite en su descripción del guía.

**Qué trae la entrega** (`545127e`, inventario en `57fbc58`). Un solo personaje con **tres siluetas**: la
**llama** de tres lenguas con el núcleo amarillo en degradado (Nivel 1), un **disco de madera** con anillos
(Nivel 2) y una **gota** con brillos (Nivel 3). Comparten la cara —aún por llegar—, las extremidades —las
mismas siluetas en las tres formas, gruesas, con las manos y los pies óvalos **del color del material**, ya
no color piel—, el contorno **negro** `#000000` y una **pantaloneta de cuadros** que sustituye a la franja,
con un color por forma: verde `#277D4A` con cuadros amarillos, naranja `#FEAA40` con cuadros rojos y granate
`#A13C4F` con cuadros rosa. Brazos, manos y piernas son `#FF9122`, `#5B4134` y `#6ED6FB`. El cuerpo
lleva **degradados y brillos**.

**Decisión de Santiago (09/10/2026).**
- El diseño nuevo es el oficial, en las tres formas.
- **Siete piezas por forma**, sin cabeza, sin cuello y **sin rodilla**: torso (cuerpo y pantaloneta),
  húmero, antebrazo con la mano y **pierna entera con el pie**, a izquierda y derecha de la pantalla. En el
  rig, la pierna va en `PiernaX`, y `RodillaX` y `AntepiernaX` quedan como pivotes sin imagen; las curvas de
  rodilla de los clips siguen y no mueven nada. Si algún día se quiere rodilla, la dibuja el artista.
- **Escala, opción A:** la altura de hoy y una sola transformación para las tres formas
  (`x' = 0,76183·x + 29,7`, `y' = 0,76183·y − 12,5` sobre el lienzo de 1300 × 1500), de modo que el Fuego
  ocupa los mismos 982 px del lienzo del rig que el sprite de hoy y la Rueda y la Gota quedan más bajas
  (818 y 878 px), como las dibujó el artista. **Ningún `Size` de las narrativas cambia.**
- **Cara provisional:** los ojos y la boca se toman del sprite de hoy, se marcan `cara_provisional` y **no
  parpadean ni mueven la boca** hasta que el artista entregue `ojos_neutra`, `ojos_parpadeo_cerrado` y `boca_0` sobre
  el lienzo de 1300 × 1500, registrados sobre el torso de cada forma.
- Los tres `_reposo` (768²) se **rehicieron en su sitio**, con el mismo nombre y GUID, a partir de las piezas
  y de la cara provisional: el botón de ayuda, el retrato y los `Art` de las narrativas ya enseñan el diseño
  nuevo, sin tocar escenas ni assets.
- **El guion radicado no se toca sin una autorización aparte de Santiago.** Los párrafos afectados y su
  redacción propuesta irán a `claudeDocs/entregables/tools/pares/` cuando se pida esa autorización (ver
  «Residuos y puntos abiertos»); hoy no hay pares escritos.

**Regla.** Decisión de Santiago, como INC-115 a INC-117 e INC-131 a INC-135. **Levanta** INC-52 —el guía
deja de ser «la llama recoloreada en madera y agua»— y, para el guía, la regla de color plano de §2.2 y el
contorno `#3B1205`; el núcleo de identidad de §7.6 se reescribe. **Sustituye** el corte provisional en nueve
piezas de INC-140 (los dos defectos de las rótulas que la decisión D20 dejó para el arte final se esperan
resueltos sin la rodilla, y se comprueban en las capturas de la ronda 2). **Mantiene** lo que el guion exige
del guía: es el mismo personaje en los tres niveles (CN-03), cambia solo entre dos secuencias encadenadas
(nunca a la vista dentro de una escena jugable), flota, tiene cara y es el único que pulsa. Cambia el riesgo
del Nivel 2: el disco (`#BDA483` y `#9C715C`) parece la sección de un tronco y queda cerca del acento
interactivo `#C79A5E`, y **sin cara se lee como un prop**; las tres condiciones de §7.6 siguen siendo
obligatorias, y la segunda («tiene cara») depende de la entrega del artista.

**Corrección aplicada (09/10/2026).** Documentos: Dirección de arte: §7.6 (núcleo, tres formas, `_reposo`,
riesgo del Nivel 2, nomenclatura y cara provisional), §2.2 (excepción), §13.1 (siete piezas y no nueve), y las
listas de §17 · Inventario de arte: `Characters/Algoritm/` · Plan de personajes finales: nota fechada ·
`Personajes-Resultados.md`: C.13 · CLAUDE.md. **Pendientes de revisión:** `Interfaces.md` §4.3 (describe a
Algoritm con la franja y las extremidades de palo), la dirección de sonido §6.1 (la «cuenta de cinco» se
justifica en las cinco bandas de la franja; el motivo sonoro se conserva, su justificación cambia y es del
carril de sonido), el Anexo F del OE3 (§2.12 aún no nombra INC-136) y, con la autorización de Santiago, el
guion §1.1.1, las notas de las dos escenas puente de `Solucion_OE2_Diseno_final.docx` y la descripción del
guía del trabajo de grado. En las herramientas (`2cd8a75`): `preparar_algoritm.py` (la ingesta de las 21
piezas con la opción A, `--aplicar` y `--reposo`), `articulaciones.py`, `coreografia.py`, `prefabs.py`,
`maqueta.py`, `pose_preview.py` y `BuildRigsFinal.cs.txt`, cuyo modo `sprites` vacía y apaga el sprite de un
nodo sin arte y enciende `Ojos` y `Boca` de Algoritm con la cara provisional; en `Assets/`, las 21 piezas
sustituidas en su sitio, las seis `antepierna` borradas con su `.meta`, la cara provisional en `Expresiones/`
y los tres `_reposo` rehechos. **Pruebas:** `CharacterRig_DA131_AlgoritmSeDibujaPorPartesYSuSpriteEnteroSeApaga`
pasa a exigir las siete piezas, la `Antepierna` sin sprite y la cara provisional. **Verificación en el
Editor: pendiente** (ronda 2): hace falta correr `sprites` sobre los tres prefabs de Algoritm, cablear su
`CharacterFaceSet`, y revisar en capturas el disco del Nivel 2 y los dos defectos de D20.

### INC-137 · Laberinto: un solo ámbar y marcos iguales, no panel marfil — cerrado (09/10/2026)

**El conflicto.** La nota del 07/10/2026 de INC-71 sacó el laberinto del reloj de luz del Nivel 2 y
fijó que su entorno se viera tal cual el arte «con panel marfil `#F7EFE2` y contorno `#C4A882`».
Santiago, el 08/10/2026, revirtió esa parte: el marfil se perdía contra la tarjeta y el contorno se
cortaba en las esquinas.

**Qué decían los documentos.** `CLAUDE.md` (párrafo del Nivel 2), `SPEC.md` (la capa de luz de las
narrativas), `Direccion_de_Arte.md` §8.2, la nota del 07/10 de INC-71 y el Anexo C del OE3: panel
marfil `#F7EFE2` y contorno `#C4A882` como el del panel de diálogo.

**Qué hace el juego.** Detrás del entorno y de la tarjeta hay un solo color, `#E8A33D`
(`Canvas/Fondo_Escena` y `MazeLayout.BackdropColor`, que vigila `MazeSceneDataTests`). El entorno y
la tarjeta de la secuencia llevan el mismo marco redondeado `ui_boton` de 8 px y `#C4A882`, sin
cortes en las esquinas: el de la tarjeta es su `Fondo`; el del entorno, el anillo hermano
`Marco_Entorno`, que sustituye al `Outline`, con 40 px de margen (el tablero encoge 7,4 %). La zona
de la secuencia (`Ventana_Secuencia`) y las muescas de los bloques vuelven a `#E0D4C0`. El aro del
botón de pista pasa de `#E2571F` a `#A0330D` solo en este nivel: sobre el ámbar daba 1,73:1 y ahora
3,27:1; ese color no estaba en la paleta y se añade a §4.3.

**Decisión de Santiago (08/10/2026).** D1 a D4 y D15 de la ronda. Corrige el juego; no hay `.docx`
radicado que lo diga.

**Regla.** Decisión de Santiago, como INC-115 a INC-117: revierte la nota del 07/10 de INC-71 en
lo que decía del panel y del contorno. El resto de esa nota sigue: el laberinto no se tiñe.

**Corrección aplicada (09/10/2026).** Documentos: CLAUDE.md (párrafo del Nivel 2) · SPEC (capa de luz de
las narrativas) · Dirección de arte: §4.3 (dos filas nuevas), §8.2, §10.2 · Interfaces: §1.1, 8–10 y
§2 · Anexo C del OE3. En el código (commit `108f95e`): `MazeLayout`, `MazeSceneController`
(`environmentFrame`, `environmentBorderWidth`), `Level2_Maze.unity` y `N2_MazeLayout.asset`.
**Pruebas:** `MazeScene_RF30_ElEntornoYLaTarjetaLlevanElMismoMarcoRedondeado`,
`MazeScene_RF30_LaSalidaSeLeeEnElEntornoYElFondoEsUnoSolo`,
`MazeScene_RF31_LaZonaDeSoltarEsMarfilSombraSobreLaTarjeta`, y en EditMode `MazeSceneDataTests`
(`…ElFondoDeLaEscenaYElDelAssetSonElMismoAmbar`, `…ElAroDelBotonDePistaSeDistingueDelFondoAmbar`).

### INC-138 · Diálogo: `#1F100C` a 30 px en un cuadro de 216 px — cerrado (09/10/2026)

**El conflicto.** `Direccion_de_Arte.md` fijaba el texto del diálogo en `#3A1E18` a 26 px (nombre del
hablante a 22 px en `#6B5248`) dentro de un cuadro de 1400 × 180 px, con la regla de «máximo 2 líneas
por cuadro». Santiago pidió un texto más grande y más oscuro.

**Qué decían los documentos.** `Direccion_de_Arte.md` §10.3, §11.3 y §11.4 (que `INC-129` y `INC-79`
habían dejado así); `Interfaces.md` §1.1 (punto 5) y §2 (tipografía).

**Qué hace el juego.** El texto es Nunito SemiBold 30 px, interlineado 1,1, y el nombre del hablante
Baloo 2 Bold 30 px, los dos en `#1F100C`; el cuadro mide 1400 × 216 px, con la cima a 232 px de 1080
(el cuarto inferior acaba en 270), el retrato centrado en vertical y 32 px de blanco entre el nombre
y el texto. La línea más alta de las 139 de las 18 narrativas mide 131 px en una caja de 137 y ninguna
necesita cuatro renglones; 17 usan tres, de modo que la regla de dos líneas ya no se cumplía (a 26 px
la incumplían 9). El `#6B5248` deja de usarse en el nombre.

**Decisión de Santiago (08/10/2026).** D5, D6 y D17 de la ronda. El carbón más oscuro se añade a la
paleta como color solo del diálogo; `#3A1E18` sigue siendo el de contornos y botones.

**Regla.** Decisión de Santiago: gana el juego y se corrige el documento. La regla del texto oscuro sobre
marfil de INC-129 no cambia.

**Corrección aplicada (09/10/2026).** Documentos: Dirección de arte: §4.3 (fila `#1F100C`), §10.3,
§11.3 (filas de título de pantalla y de diálogo, y el mínimo de 26 px sin el nombre del hablante) y
§11.4 (hasta tres renglones) · Interfaces: §1.1 (punto 5) y §2 · Anexo E del OE3. En el código (commit
`08b6c63`): `Narrative.unity`. **Pruebas:**
`NarrativeScene_RNF01_TodasLasLineasDeLasDieciochoNarrativasCabenEnSuCuadro` (sustituye a la de la
línea más larga), `NarrativeScene_RF05_ElNombreDelHablanteCabeEnUnaLinea` y
`NarrativeScene_RNF03_ElNombreElTextoYLosBotonesNoSePisanDentroDelCuadro`.

### INC-139 · El halo de las fogatas: degradado radial y ciclo de 2 s — cerrado (09/10/2026)

**El conflicto.** `Direccion_de_Arte.md` §8.1 pedía un halo de «círculo plano, sin degradado» que oscilara
entre 0,95 y 1,05 en un ciclo de 1,2 s, y no estaba implementado. Santiago pidió un pulso lento y
suave, y las pruebas con el círculo plano mostraron que no se sostiene.

**Qué decían los documentos.** `Direccion_de_Arte.md` §8.1 (tabla de la hoguera y el párrafo que sigue);
`Assets/Game/Art/Inventario.md` (`fx_n1_halo.png` y `.anim`, pendientes); el plan del Slice 1.

**Qué hace el juego.** `FireGlow` (`Game.Scaffolding`) pone un halo `#F0A84E` al 20 % en el centro, un
disco con degradado radial generado por código (opacidad plena hasta 0,3 del radio y a cero en el
borde), hermano de la llama justo antes de ella. Respira entre 0,95 y 1,05 de escala en un ciclo de
2 s, sobre tiempo escalado y sin cambiar nunca la opacidad (RNF-21). Lo llevan las nueve llamas de las
narrativas (`NarrativeProp.Glows`) y la llama cenital de `Level1_Cave`, donde el halo va al fondo del
suelo para no lavar el montón ni las piedras. El círculo plano al 20 % se leía de noche como una
mancha naranja de unos 625 px que cortaba la pared, y a plena luz casi no se veía salvo por los aros
fantasma donde se cruzan dos discos; a plena luz el halo con degradado también se ve poco, y se dejó así.

**Decisión de Santiago (08/10/2026).** D8 y D18 de la ronda: halo en todas las fogatas encendidas, incluida la
cenital del Nivel 1, con ciclo de 2 s. El degradado lo decidió el orquestador de la ronda por delegación,
mirando las capturas (`capturas/03-narrativa/decision-halo-*.png`), y es la parte que contradice el documento.

**Regla.** Gana el juego y se corrige el documento. No hay `.docx` radicado que describa el halo.

**Corrección aplicada (09/10/2026).** Documentos: Dirección de arte: §8.1, §12.2 (fila nueva) y §14.3 ·
Inventario de arte (`FX/`) · SPEC (andamiaje) · CLAUDE.md · Anexo E del OE3. En el código (commit `08b6c63`):
`FireGlow`, `NarrativeProp.Glows`, `NarrativeSceneController` y `Level1_Cave.unity`. **Pruebas:** `FireGlowTests`
(12), `NarrativeSequence_RF05_CadaLlamaEmiteSuHalo`, `CaveSceneDataTests`,
`NarrativeScene_RNF21_ElHaloDeLaFogataPulsaLentoYSinDestellos` y
`FireLevel_RF20_LaLlamaCenitalEmiteSuHaloSinVelarElMonton`.

### INC-140 · Algoritm por partes con el arte provisional — cerrado (09/10/2026)

**El conflicto.** INC-131 levantó la regla de «una sola imagen por forma, sin recorte» **para el arte
final** de Algoritm y la dejó vigente para el actual. Santiago decidió el 08/10/2026 que sus brazos y
piernas se muevan ya, en todas sus escenas, sin esperar la entrega.

**Qué decían los documentos.** `Direccion_de_Arte.md` §7.6 y §13.1 («Arte actual (provisional): una sola
imagen por forma»), `Interfaces.md` §4.3, `Inventario.md` (`Characters/Algoritm/`), el Plan de personajes
finales y `Personajes-Resultados.md` (C.2 y C.6).

**Qué hace el juego.** Las tres formas se cortan de su `_reposo` en nueve piezas cada una (27 PNG en
`Frontal/`, con `pose_preview.py --exporta-maqueta`), con una rótula en cada articulación; `Cuerpo` queda
apagado y `Ojos` y `Boca` también, porque la cara va pintada en el torso. Cuando llegue el arte final
se sustituyen los PNG con el mismo nombre y se corre `sprites`. Dos defectos de las piezas provisionales
quedan **pendientes para el arte final** (decisión D20): los anillos oscuros de las rótulas en los fundidos
y el talón claro de unos 12 px en el hombro girado. El corte en nueve piezas sale del arte provisional y lo
sustituirá la ingesta del diseño nuevo de Algoritm (INC-136, del carril de perfil); esos dos defectos se
revisan con ese arte.

**Regla.** Decisión de Santiago, como INC-131: levanta la regla de la imagen única también para el arte
provisional de Algoritm. No hay `.docx` radicado que lo diga.

**Corrección aplicada (09/10/2026).** Documentos: Dirección de arte: §7.6 (arte actual y arte final), §13.1 ·
Interfaces: §4.3 · Inventario de arte · Plan de personajes finales (nota) · `Personajes-Resultados.md`: C.12
y notas en C.2 y C.6 · CLAUDE.md · Anexo F del OE3. En el código (commit `bd8b802`): los tres prefabs de
Algoritm, `rig_articulaciones.json` y las herramientas `maqueta.py`, `pose_preview.py`, `articulaciones.py` y
`coreografia.py`. **Pruebas:**
`CharacterRig_DA131_AlgoritmSeDibujaPorPartesYSuSpriteEnteroSeApaga`, `CharacterRig_DA133_*` y
`CharacterRig_CP02_NingunClipEsDeDerrotaCaidaNiSalto`.

*Nota (09/10/2026): el corte en nueve piezas lo sustituyó INC-136 con las siete piezas del diseño final,
en su sitio y con los mismos nombres de archivo; las seis `antepierna` se borraron.*

### INC-141 · `Wave`, el saludo de Algoritm — cerrado (09/10/2026)

**El conflicto.** Ningún documento nombraba un saludo. La tarjeta de los créditos (INC-82) mostraba a
Algoritm quieto en `Idle`; Santiago pidió que saludara.

**Qué decían los documentos.** `Direccion_de_Arte.md` §7.3 y §13.3 (el set mínimo no tenía saludo) e
`Interfaces.md` §1.1.

**Qué hace el juego.** `ActorAction.Wave = 22`, al final del enum para conservar los números que guardan
los assets, con emoción `Happy`. El clip `char_algoritm_anim_saludar.anim` (bucle de 4,8 s con un reposo de
unos 0,9 s) y su estado `Wave` están en `char_algoritm.controller`. Es solo del guía: a un miembro de la
familia le caería a `Idle`. El brazo es el derecho de pantalla y nunca tapa la cara (la prueba de
`pose_preview.py` exige libre el 99,5 % de ojos y boca).

**Regla.** Solo en el juego: se añade al documento (regla b).

**Corrección aplicada (09/10/2026).** Documentos: Dirección de arte §7.3 (fila «Saludo») y §13.3 ·
Interfaces §1.1 · `Personajes-Resultados.md` C.12. En el código (commit `bd8b802`): `ActorAction`,
`ActionEmotion`, el clip y el controlador. **Pruebas:** `CharacterRig_DA133_CadaPersonajeTieneUnEstadoPorAccion`
y `FacialEmotion` (`Wave` a `Happy`).

### INC-142 · Créditos sin tarjeta detrás de Algoritm — cerrado (09/10/2026)

**El conflicto.** INC-82 reemplazó el rótulo «Algoritm saluda · placeholder» de la pantalla de créditos
por Algoritm en una tarjeta de 520 × 952 px. Santiago decidió que la tarjeta sobra: el guía saluda solo.

**Qué decían los documentos.** `Interfaces.md` §1 (fila 14, Créditos); la nota de INC-82.

**Qué hace el juego.** `Credits.unity` pierde `Fondo`, `Marco`, `Sombra` y la imagen vacía
`AlgoritmSaluda`; el rig de Algoritm en fuego cuelga directo de `AlgoritmPanel` a su ancho de 520 px y
`CreditsController.guide` lo anima con `Wave` al abrir la pantalla.

**Regla.** Decisión de Santiago: gana el juego y se corrige el documento.

**Corrección aplicada (09/10/2026).** Documentos: Interfaces §1 (fila 14) y §1.1 · Dirección de arte §10.4
(sección nueva). En el código (commit `bd8b802`): `Credits.unity` y `CreditsController`. **Prueba:**
`Credits_RF08_AlgoritmSaludaOcupandoElLugarDeLaTarjetaSinElla`.

### INC-143 · Inicio y menú de niveles: portada, título y marcas de estado — cerrado (09/10/2026)

**El conflicto.** El inicio llevaba el título dentro de una tarjeta de 280 px y a Algoritm solo en un
cuadro marfil vacío (mockup 2); el menú de niveles, las insignias arriba a la derecha de la imagen
(mockup 4) y la tarjeta del Nivel 2 con el laberinto, que INC-82 había puesto en lugar del bosque. Santiago
pidió otra composición.

**Qué decían los documentos.** `Interfaces.md` §1.1 (puntos 2 y 4), `Direccion_de_Arte.md` §11.3 (el título a
96 px) y la fila `PG-01` de §18, los mockups 2 y 4 de `Mockups de interfaz Algoritmia.html` (que no se
editan: quedan superados) y la nota de INC-82.

**Qué hace el juego.** La portada del inicio es la mitad izquierda del bosque del Nivel 2
(`env_n2_bosque_claro`, foco 0,25, sin cruzar la costura de x = 0,5) en una ventana siempre cuadrada, con
Papá, Mamá, el Niño, la Niña y Algoritm en fuego en `Idle` desfasados (`CharacterRig.idlePhase`). El título
sale de la tarjeta: blanco, 112 px, con contorno `#3A1E18` de 4 px; la tarjeta baja de 280 a 120 px y solo
lleva el lema; título, tarjeta y botones forman un bloque centrado en vertical (222 px arriba y abajo). En el
menú de niveles, cada tarjeta lleva una ventana de 444 × 250 px; el icono de estado va abajo a la izquierda
de la imagen y su texto, centrado entre la imagen y el botón, sin la pastilla marfil; la imagen del Nivel 2
es el bosque con foco 0,25. `FramedIllustration` (`Game.UI`) aplica el encuadre desde la ventana.

**Decisión de Santiago (08/10/2026).** D9, D10 y D11 de la ronda.

**Regla.** Gana el juego y se corrige el documento; los mockups no se tocan.

**Corrección aplicada (09/10/2026).** Documentos: Interfaces §1.1 (puntos 2, 4 y 14) y §2 · Dirección de arte:
§10.4 (sección nueva), §11.3, §11.4 y §18 (`PG-01`) · Inventario de arte (`Environments/Wheel/`) · CLAUDE.md ·
SPEC · Anexos E y G del OE3 y el capítulo 7. En el código (commit `c96846f`): `MainMenu.unity`,
`LevelSelect.unity`, `FramedIllustration`, `CharacterRig.idlePhase`. **Pruebas:**
`MainMenu_RF01_LosCincoPersonajesEsperanEnReposoSinRespirarAlUnisono`,
`MainMenu_RF01_ElBosqueNoCruzaLaCosturaDelLienzo`,
`MainMenu_RF01_TituloTarjetaYBotonesQuedanCentradosEnVertical`,
`LevelSelect_RNF19_ElIconoDeEstadoVaAbajoALaIzquierdaDeLaImagenYSuTextoEntreImagenYBoton`,
`LevelSelect_RF03_CadaTarjetaMuestraUnaIlustracionSinCostura` (ajustada) y, en EditMode,
`FramedIllustrationTests` y `CharacterRigIdlePhaseTests`.

### INC-144 · El húmero de Papá ya no asoma por el codo — cerrado (09/10/2026)

**El conflicto.** El arte final de Papá, entregado el 06/10/2026, trae el húmero con una punta clara y sin
contorno que sobresale de su extremo redondo; con el antebrazo solapado, la punta asomaba por el codo al
doblarlo. `preparar_arte_final.py` lo dejaba como tolerancia (`holguras` de 42,0 y 20,9 px). Santiago pidió
que se subiera el húmero y se bajara un poco el antebrazo.

**Qué decían los documentos.** `Personajes-Resultados.md` C.9 a C.11, que daban la costura por aceptada.

**Qué hace el juego.** `codo.py` corrige `arte_final.json` sin tocar ningún PNG: el húmero izquierdo sube
19,2 px y los dos antebrazos bajan 37,4 px, con lo que lo que asoma pasa de 52,6 y 33,4 px a −3,2 y 1,0.
Los brazos miden unos 37 px más, y el choque de `Strike` baja de y = 549 a y = 595 con los codos más
abiertos; Santiago los aprobó (D16), y con ello cierra el punto de C.9 que el choque tenía pendiente.

**Regla.** Decisión de Santiago: corrige el juego; ningún documento radicado cambia.

**Corrección aplicada (09/10/2026).** Documentos: `Personajes-Resultados.md` C.12 · Plan de personajes
finales (nota del 08/10). En el código (commit `a938f9a`): `Papa.prefab`, los 21 `.anim` de Papá,
`arte_final.json`, `codo.py` y `pose_preview.py` (comprobación del codo).

### INC-145 · La cámara de la 3.3 acompaña a la balsa — cerrado (09/10/2026)

**El conflicto.** En el cruce de la 3.3 la cámara tenía paradas fijas y la balsa, que se desliza durante 9 s,
salía del cuadro cuando el texto tardaba: el pendiente «3.3, línea 0» de `Personajes-Resultados.md`, que
dejaba al criterio de Santiago. El diseño de cámara del Nivel 3 ya no está en el equipo.

**Qué hace el juego.** `NarrativeProp.CameraFollows`, marcado solo en la balsa de `N3_Escena33_Cruce`: el
foco de cada parada se corre lo que ella se ha movido, y el desplazamiento se queda al llegar. La parada de
la línea 1 sube de 0,47 a 0,52 y la apertura (`CameraStart` y `CameraEnd`) pasa de 0,38 a 0,50 (D19), para
que la balsa se deslice menos hacia la izquierda de la pantalla. Antes, sin avanzar el texto, la balsa
quedaba 204 px fuera del cuadro; ahora el margen mínimo es de 77 px.

**Decisión de Santiago (08/10/2026).** D7 y D19 de la ronda.

**Regla.** Corrige el juego; lo aplicado sobrevive solo en `N3_Escena33_Cruce.asset`.

**Corrección aplicada (09/10/2026).** Documentos: CLAUDE.md (párrafo del sonido y la balsa) ·
`Personajes-Resultados.md` (el punto pendiente queda resuelto por esta ronda) · Anexo F del OE3. En el código
(commit `08b6c63`): `NarrativeProp`, `NarrativeSceneController` y `N3_Escena33_Cruce.asset`. **Pruebas:**
`NarrativeSequence_RF44_LaCamaraDelCruceAcompanaALaBalsa` y
`NarrativeScene_RF44_LaCamaraSigueALaBalsaMientrasCruza`.

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
exigen observar a estudiantes jugando, y su columna «Situación» se actualizó a lo que hace el juego.
Los hará Santiago con estudiantes de cuarto en una misma sesión (acta D10), con los guiones H1 y H2
de `claudeDocs/tasks/OE4/Hoja-HUM.md` y, antes, el consentimiento informado de los acudientes
(`claudeDocs/tasks/OE4/Consentimiento-RNF12.md`, RNF-12); se cierran cuando entregue los resultados
y no pasan a trabajos futuros. Están **cerrados**: `PG-01` (título «Algoritmia», en el juego desde el
09/09/2026 y en el `.docx` desde el 29/09/2026), `PG-02` (el guía se llama **Algoritm**, 02/09/2026 —
INC-44), `PG-03` y `PG-04` (redacción de `RF-16` y `RF-32`, 24/08/2026) y `PG-07` (autorización
escrita de los personajes, 30/08/2026 — INC-43).

**Pendientes de Santiago (01/10/2026).** Puntos del trabajo de grado que el juego no zanja; quedan
abiertos hasta que Santiago aporte el dato o la decisión. Sigue abierto el primero; el cuarto se
cerró el 01/10/2026, y el segundo y el tercero el 09/10/2026 con el dato de Santiago. Las secciones son las del trabajo de grado vigente, la plantilla del
28/07 (`Trabajo_de_Grado_Entrega_Plantilla_28jul.docx`, desde el 30/09/2026):

1. **Anexo con la autorización de la Familia Anonaky** (INC-78). La letra C ya no está libre: en la
   plantilla, LISTA DE ANEXOS y ANEXOS tienen A (presupuesto), B (cronograma), C, D y E (las
   soluciones de OE1, OE2 y OE3) y, desde el 01/10/2026, F (el formato de consentimiento de RNF-12,
   tarjeta D10-8), así que la autorización será el **Anexo G**. Falta el escaneo y su enlace de
   SharePoint para añadirla en las dos listas; al añadirla, la viñeta de §3.3.2 que cita la
   autorización cambia su punto final por «; su constancia es el Anexo G.».
2. **Herramientas de ilustración — cerrado (09/10/2026).** El trabajo de grado solo nombraba
   Illustrator y Photoshop (§5.1, viñeta de Adobe Illustrator y Photoshop; era §5.2 en la versión
   anterior). **Dato de Santiago: el programa es CLIP Studio.** Cada sprite pasa por cinco pasos:
   (1) búsqueda de inspiración en equipo, mediante lluvia de ideas; (2) boceto; (3) limpieza de la
   línea; (4) aplicación de color y sombras; (5) entrega y correcciones. Consta en el trabajo de grado
   (§5.1 y el capítulo 8) y en el entregable del OE3 (Anexo E y capítulo 2).
3. **Colaboración en el arte — cerrado (09/10/2026).** Los créditos acreditan la «Producción de arte» a
   Sofía Valentina Giraldo Segovia (`CreditsContent.asset`). **Dato de Santiago: es estudiante de
   animación y diseño, contratada para el diseño y la asesoría de los sprites; se vinculó en el acta
   D01 (02/09/2026) y entregó entornos y props en las actas D03, D04, D06 y D09.** Su participación es
   de diseño y asesoría gráfica; la autoría del trabajo y las decisiones de diseño pedagógico y de
   mecánica son del equipo de estudiantes (acta D01). El trabajo de grado y el entregable la nombran así.
4. **Las correcciones del 29/09 en la plantilla — cerrado (01/10/2026).** La plantilla partía de un
   texto anterior a la rev. 14 y no recogía las correcciones que se hicieron en
   `Trabajo_de_Grado_2026_ICONTEC_IEEE (2).docx`: las de INC-78 —el Resumen, el Abstract, la
   Introducción, §3.3.2 y la viñeta del libro en §5.1 volvían a presentar la autorización de los
   autores como condicional—, INC-87 (§3.3.1, «confidencialidad e integridad»), INC-111 (§1.3, la
   integración con la resolución de problemas matemáticos), INC-112 (§3.2.4, Wix y niños de 7 a 8
   años) e INC-113 (§5.1, sin Unity Test Framework, Git ni Claude). La edición que autoriza el acta
   D10 alcanzaba solo el capítulo 8; **por delegación de Santiago (01/10/2026)** el orquestador las
   llevó a los capítulos 1 a 5, porque ya estaban decididas el 29/09 y no son decisiones nuevas. Se
   aplicaron el 01/10/2026 por automatización de Word —doce cambios, junto con el Anexo F de
   consentimiento—, sin tocar el resto del texto y con la tabla de contenido y las listas de figuras
   y tablas al día; §3.3.2 sigue sin anunciar la constancia como anexo hasta que exista (punto 1). El
   trabajo de grado no tiene tabla de control de cambios: la edición consta aquí y en INC-78, INC-87
   e INC-111 a INC-113.

**Pendiente del carril de sonido (01/10/2026).** El silencio S3 de la dirección de sonido §5 —un
segundo con el ambiente al mínimo y la música suspendida antes de la última frase de Algoritm— no se
aplica en la escena final, que desde INC-125 tiene ambiente. `SilenceCut` solo calla la música o lo
calla todo sin devolverlo, y «Y eso ya lo llevan puesto» comparte línea con las dos frases
anteriores: hacen falta un corte con duración en `AudioManager` (`Game.Audio`), su valor en
`SilenceCut` (`Game.Scaffolding`) y partir esa línea de `N3_EscenaFinal`, como explica §5. Es del
carril de sonido, que lleva Santiago desde el acta D08.

**Pendientes del carril de personajes (09/10/2026).** Dependen de cosas que el juego no zanja o de una
segunda ronda del Editor: (1) el artista debe entregar los **ojos cerrados de frente** de Papá, Mamá, la
Niña y el Niño (en el lienzo del 06/10/2026, `Expresiones/char_<x>_ojos_parpadeo_cerrado`); sin ellos el
frente no parpadea (INC-135); (2) debe entregar la **cara de Algoritm** —`ojos_neutra`,
`ojos_parpadeo_cerrado` y `boca_0` como mínimo, sobre el lienzo de 1300 × 1500—; hasta entonces lleva la cara
provisional del sprite anterior y no parpadea (INC-136); (3) la **autorización de Santiago para editar los
radicados** que describen el diseño anterior de Algoritm (guion §1.1.1, las notas de las dos escenas puente
y la descripción del guía del trabajo de grado); (4) los tonos de piel y el contorno que trae el arte final
(`#FFC69F`, `#DE9563` y trazo negro) no son los de `Direccion_de_Arte.md` §4.1 (`#F2D3BC`, `#D9AF95` y
`#3A1E18`): hay que decidir si se corrige §4.1 o se pide otro tono; (5) la **ronda 2 del Editor**
(`Personajes-Resultados.md`, C.13: el orden nuevo de las piernas, los sprites y la cara de Algoritm, las
pruebas, `suite2` y las capturas); (6) la **medición de RNF-06** con un ejecutable nuevo: el margen de 21 MB
es el de rc2 (01/10/2026), anterior al arte frontal final, y el perfil (unos 8,5 MB) y Algoritm (unos
3,4 MB netos) lo consumen en más de la mitad, sin contar los ojos cerrados de frente que faltan; y (7) las
fuentes del OE3 (Anexo F, §2.12) y la republicación en Word, que no nombran todavía INC-134 a INC-136.

---

## Historial de revisiones

- **rev. 21 (09/10/2026)** — Decisiones de Santiago con la llegada del arte de perfil y del nuevo
  Algoritm. Se registran y cierran **INC-134** (la familia se ve de perfil al recorrer o trabajar el entorno,
  hacia donde se mueve, con las dos piernas siempre detrás del torso; levanta el «ciclo único que se voltea»
  de la dirección de arte §13.3), **INC-135** (el parpadeo es de dos cuadros, ojos abiertos y cerrados; acota
  §7.3 y INC-131) e **INC-136** (el diseño nuevo de Algoritm —llama, disco de madera y gota, con pantaloneta
  de cuadros— sustituye al núcleo de identidad de §7.6; levanta INC-52 y, para el guía, la regla de color
  plano, y sustituye el corte provisional de INC-140). El runtime de INC-134 e INC-135 entró en `910c0f0`,
  las entregas de arte en `3efa765`, `3a0f6ad` y `545127e`, sus inventarios en `a2d32e2` y `57fbc58`, las
  herramientas y el arte de perfil en `beb1cfe`, la ronda 1 del Editor en `9966bcb` y la corrección de las
  piernas con el Algoritm final en `2cd8a75`. La ronda 2 del Editor queda pendiente. Los radicados que
  describen el diseño anterior de Algoritm (guion §1.1.1 y las notas de las dos escenas puente, y el
  trabajo de grado) **no se editan** sin la autorización de Santiago. Mantiene CP-02.

- **rev. 20 (09/10/2026)** — Ronda de ajustes de diseño del 08/10/2026. Decisiones de Santiago que
  corrigen el juego o el documento: se registran y cierran **INC-137** (laberinto con un solo ámbar y
  marcos iguales, que revierte el marfil del 07/10), **INC-138** (diálogo `#1F100C` a 30 px en un cuadro de
  216 px), **INC-139** (halo de las fogatas con degradado radial y ciclo de 2 s), **INC-140** (Algoritm por
  partes con arte provisional, que adelanta INC-131), **INC-141** (el saludo `Wave`), **INC-142** (créditos sin
  tarjeta), **INC-143** (portada del inicio, título fuera de la tarjeta, marcas del menú de niveles),
  **INC-144** (el codo de Papá) e **INC-145** (la cámara de la 3.3 acompaña a la balsa). Se cierran dos
  pendientes de Santiago del trabajo de grado con su dato: las herramientas de ilustración (CLIP Studio,
  proceso de cinco pasos) y la colaboración en el arte (Sofía Valentina Giraldo Segovia). Mantiene CP-02 y
  RNF-21. Ninguna regla anterior se levanta salvo la nota del 07/10 de INC-71 en lo del panel marfil y la
  parte de INC-131 que dejaba el arte provisional de Algoritm sin recorte.
- **rev. 19 (06/10/2026)** — Decisión de Santiago sobre el antebrazo de la familia. Se registra y cierra
  **INC-133**: el húmero detrás del torso y el antebrazo delante del torso, de la cara y de las piernas
  (Papá con la cabeza delante del torso), con `AnclaAntebrazoX` y `LimbFollower`. Acota INC-132 en la
  familia; no la levanta para `Strike` ni para Algoritm. Mantiene CP-02.
- **rev. 18 (05/10/2026)** — Decisión de Santiago sobre el orden de dibujo de los personajes. Se
  registra y cierra **INC-132**: en la familia, los brazos detrás del torso y delante de la cabeza
  (delante del torso solo al golpear, 06/10)
  (en Algoritm, delante del cuerpo y la cara encima), con la coreografía movida a `coreografia.py` y la entrada del arte final
  en `preparar_arte_final.py`. No levanta ninguna regla anterior; mantiene CP-02.
- **rev. 17 (05/10/2026)** — Decisión de Santiago sobre el arte final de los personajes. Se registra
  y cierra **INC-131**: cabeza separada, ojos y boca en capas propias, seis expresiones, y
  extremidades en dos tramos con codos y rodillas, también en Algoritm (sin cuello). Levanta
  **INC-108** y la regla de Algoritm de una sola imagen para el arte final, no para el actual;
  mantiene CP-02 (ninguna expresión de tristeza, enfado o derrota). Su código entró en el commit
  `a12dbb3` y su verificación en el Editor queda pendiente.
- **rev. 16 (01/10/2026)** — Cierre de la sesión del 30/09/2026 (acta D10). Con la regla del 29/09
  se registran y cierran **INC-118** a **INC-130**, los cambios de la sesión que contradecían un
  documento: el plano abierto del Nivel 3, con la escala de las narrativas (118); la chispa del
  Nivel 1 como un solo rayo (119); la caja de la escena 2.2 (120); la carretilla amarrada del taller
  (121); «Probar balsa» en la base y el amarre (122); la balsa que se hunde y suena (123); la balsa
  del cruce, bajo la espuma (124); el ambiente de la escena final (125); cinco nombres de archivo
  (126); el crédito de los glifos de pausa (127); los ajustes de importación de §15.2 (128); el
  cuadro de diálogo (129), y los cuadros del fuego y del humo del Nivel 1 a 1024 px, que bajan el
  paquete de entrega de 866,7 MB a 479,0 MB (130, RNF-06). Decide Santiago (30/09) y, en el detalle
  y en INC-130, el orquestador por delegación suya (01/10). Los valores de INC-118 a INC-128 se
  comprobaron contra el repositorio después de la revisión del código. Los radicados que cambian con la prueba anticipada —RF-11, RF-40 y RF-42; el guion
  §1.8.3 a §1.8.4.1 y CU-10; HU-12 y HU-13, y por coherencia CP-06 y CU-03— se editan por
  automatización de Word, con su fila en cada control de cambios. Ningún documento contradice, y por eso no abren hallazgo, las piedras que
  quedaban bajo las hojas al encender; el humo del Nivel 1, ahora un hilo que nace en el punto del
  golpe y, al soplar, sube a la corona de la llama por detrás de ella; los mandos del Nivel 1, que
  desde «Soplar» dejan de responder (un segundo «Soplar» relanzaba el encendido); la línea de
  registro de cada carga de `SceneLoader` (RNF-04), y el salto de los viajeros de la 3.3 cuando el
  texto avanza durante el cruce (`NarrativeProp.FinishesSteps`, solo en esa escena). Tampoco lo abre
  aceptar como definitivos el anillo de la zona, la balsa hundida, el conjunto genérico de interfaz
  y el boceto de la iconografía del informe docente: «provisionales» los llamaban los registros de
  tareas, no un documento de diseño. Como D1 no abrió hallazgo, tampoco lo abre la decisión que la
  sustituye: el entregable del OE3 se reescribe al estado vigente del prototipo, lo que **deja sin
  efecto la decisión D1** del 29/09 —darle solo una nota fechada—, y ya no lleva anexo H, porque las
  actas están en Word en el SharePoint de Santiago. «Residuos y puntos abiertos» cita ahora el trabajo
  de grado vigente, la plantilla del 28/07: el anexo con la autorización de la Familia Anonaky será
  el G, porque desde el 01/10 el F es el formato de consentimiento de RNF-12; las correcciones del
  29/09 que la plantilla no recogía se registran como pendiente 4 y se cierran el mismo día,
  aplicadas por automatización de Word por delegación de Santiago, y se suma el silencio S3 de la
  escena final. Sin reabrir ningún hallazgo, ganan una nota fechada INC-97 (la causa y el arreglo de
  DEF-SPIKE-01, con su residuo DEF-RC2-01) e INC-80 (la excepción de los `ScrollRect` del informe
  docente y de los créditos).
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
