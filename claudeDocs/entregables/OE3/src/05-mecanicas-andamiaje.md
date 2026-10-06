# 5. MECÁNICAS IMPLEMENTADAS

Este capítulo contrasta las mecánicas de juego que especificó el guion del segundo objetivo (Solución OE2, apartados 1.4.3, 1.6 y 1.8, con la trazabilidad de su apartado 1.11) con las que contiene el prototipo en el corte del 1 de octubre de 2026. El capítulo 4 describe cada reto desde la perspectiva del estudiante; aquí se registra qué interacción lo sostiene, qué se ajustó frente al diseño radicado durante el desarrollo y qué prueba automatizada lo verifica. Con ello se documenta el primer indicador del objetivo, la tasa de implementación de mecánicas (véase el apartado 1.3).

## 5.1 Mecánicas planeadas y mecánicas implementadas

El guion especifica siete mecánicas principales (una para el Nivel 1, una por cada fase del Nivel 2 y tres para el Nivel 3), que el prototipo aloja en cinco escenas jugables, y dos mecanismos transversales: las escenas narrativas y la ayuda del guía. En la Tabla 5.1, la columna «Planeada en OE2» resume cada mecánica como la describía el guion antes de su alineación con el prototipo del 29/09/2026, y la columna «Evidencia» indica los requerimientos que atiende y una clase de pruebas representativa.

**Tabla 5.1.** Mecánicas planeadas en el guion frente a las implementadas en el prototipo.

| Mecánica | Nivel o fase | Planeada en OE2 | Implementada | Evidencia |
|---|---|---|---|---|
| Encendido por hipótesis y experimento | N1 | 1.4.3: deslizante de posición, «Golpear» y «Soplar» | Sí: reunir por arrastre y encender con fuerza y cercanía | RF-14 a RF-21; `FirePanelTests` |
| Selección de objetos por patrón | N2, fase 1 | 1.6.1.2: clic, contador, carga y «Empujar» | Sí; el rodado se ve en la escena 2.2 | RF-22 a RF-26; `PatternSelectionTests` |
| Ensamblaje secuencial de la carretilla | N2, fase 2 | 1.6.2.2: seis piezas y seis pasos | Sí, con la cuerda como séptima pieza | RF-27 a RF-29; `AssemblySequenceTests` |
| Programación por bloques | N2, fase 3 | 1.6.3.2: tres bloques, «Ejecutar» y retroceso | Sí, con cuenta y giro a los dos lados | RF-30 a RF-34; `SequenceExecutorTests` |
| Recolección con desplazamiento | N3 | 1.8.2: flechas, «Recoger», inventario y lista | Sí | RF-35 a RF-39; `RiverMovementTests` |
| Ensamblaje por fases bloqueantes | N3 | 1.8.3: base, amarre, mástil y vela | Sí, sobre el río | RF-40, RF-41; `RaftAssemblyTests` |
| Prueba y depuración de la balsa | N3 | 1.8.4: hundimiento, resaltado y devolución | Sí, con prueba anticipada en base y amarre; escena 3.2 al primer fallo de la fase 3 | RF-42, RF-43; `RaftValidatorTests` |
| Escenas narrativas con diálogo | Todos | 1.2: ilustración y diálogo secuencial | Sí, en 18 secuencias | RF-05, RF-06; `NarrativeSceneTests` |
| Ayuda y pista del guía | Todos | 1.3 y 1.4.3.6 | Sí, en las cinco escenas jugables | RF-13; `HintPolicyTests` |

Las siete mecánicas principales están implementadas, una tasa de siete sobre siete frente a lo planeado, y también los dos mecanismos transversales. Cinco de ellas se ajustaron durante el desarrollo por decisiones del equipo (INC-47, INC-50, INC-54, INC-55 e INC-122), y los documentos radicados se corrigieron el 29/09/2026 y el 01/10/2026 para recogerlas (véase el apartado 5.3). Todo requerimiento de los módulos C, D y E de OE1 (RF-14 a RF-44) tiene al menos una prueba que lo nombra, y `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra` lo comprueba en cada corrida sobre los 47 RF (véanse el apartado 9.4 y el Anexo A).

El indicador del trabajo de grado exige «al menos tres mecánicas principales» y pone como ejemplos el movimiento, los diálogos y la lógica de bloques. La Tabla 5.2 ubica lo implementado en los seis tipos de mecánica que distingue el marco conceptual del mismo documento (apartado 3.1.5).

**Tabla 5.2.** Tipos de mecánica del trabajo de grado (apartado 3.1.5) presentes en el prototipo.

| Tipo de mecánica | Dónde se implementa | Estado |
|---|---|---|
| Movimiento | Desplazamiento de Mamá por la orilla (N3) | Implementada |
| Interacción | Seleccionar, arrastrar, recoger y avanzar diálogos (todos) | Implementada |
| Secuenciación de acciones | Ensamblaje de la carretilla y secuencia de bloques (N2) | Implementada |
| Resolución de puzzles | Hipótesis del fuego (N1), patrón (N2) y depuración de la balsa (N3) | Implementada |
| Sistema de progreso | Desbloqueo por nivel y guardado por fase (RF-03, RF-04) | Implementada |
| Optimización de soluciones | — | Excluida por diseño |

El prototipo cubre cinco de los seis tipos, entre ellos los que abarcan los tres ejemplos del indicador: el movimiento; la interacción, que incluye los diálogos, y la secuenciación de acciones, que incluye la lógica de bloques. La optimización de soluciones se excluyó a propósito, porque el trabajo de grado la asocia a recompensas o puntajes mayores para la solución más eficiente, y eso choca con CP-03. La única propuesta del guion en esa dirección, el nivel avanzado opcional del apartado 1.10 (resolver con menos pasos bajo presión de tiempo), quedó fuera del alcance por no tener requerimiento asociado y por su tensión con CP-02.

## 5.2 Formas de interacción y control

RNF-02 y CT-06 limitan la interacción a clic y clic sostenido, sin combinaciones de teclas ni doble clic, con una sola excepción: escribir el nombre o alias al crear un perfil (RF-02). El hallazgo INC-01 retiró de los documentos las teclas de dirección, e INC-80 llevó la excepción del nombre al texto de CT-06 y RNF-02 el 29/09/2026. La Tabla 5.3 resume el esquema implementado.

**Tabla 5.3.** Esquema de control del prototipo.

| Control | Acción | Dónde |
|---|---|---|
| Clic | Seleccionar, confirmar, avanzar el diálogo y pulsar botones | Todas las pantallas |
| Clic sostenido y soltar | Arrastrar una pieza, la caja, un bloque o un material | N1 (reunir), N2 (tres fases), N3 (ensamblaje) |
| Clic o clic sostenido sobre un deslizante | Elegir la fuerza del golpe y la cercanía de las piedras | N1 (encendido) |
| Clic sostenido sobre una flecha | Desplazar a Mamá mientras se mantiene | N3 (recolección) |
| Clic sobre los botones de subir y bajar | Desplazar la secuencia de bloques cuando desborda | N2, fase 3 |
| Escritura con el teclado | Escribir el nombre o alias al crear un perfil, única entrada de teclado (CT-06, RNF-02) | Selección de perfil |

El código lo cumple y lo vigilan pruebas:

- Hay un solo mapa de controles: todas las escenas con módulo de entrada y el propio proyecto usan `ControlesJugables.inputactions`, sin vinculaciones de teclado, mando ni rueda del ratón, y ningún módulo usa la clase de entrada heredada del motor; lo vigilan pruebas de arquitectura (véase el apartado 9.1).
- Las flechas del Nivel 3 son botones de interfaz, en cruceta abajo a la derecha, que responden al clic sostenido (INC-91). El módulo del nivel no referencia el sistema de entrada, y `RiverScene_INC01_NoExisteVinculacionDeTecladoEnElMapaDeControles` vigila que no gane esa referencia.
- «Ejecutar» responde a un clic simple, como normalizó el guion al cerrar el punto PG-04 frente al doble clic que pedía el documento fuente del Nivel 2.
- Las listas jugables se desplazan con botones, porque un panel desplazable por arrastre competiría con el arrastre de la mecánica: la secuencia del laberinto usa dos botones y una barra que se pulsa y no se arrastra (Anexo C, apartado 2.4), y el panel «¿Quién juega?» muestra tres perfiles por página, con dos flechas para pasar de página. Los créditos y el informe docente, pantallas no jugables y sin arrastrar y soltar, se recorren con su barra, que responde al clic y al clic sostenido; es la excepción que recoge HU-18 desde el 29/09/2026 (INC-80).
- En el bosque el cursor no es un control: los objetos se apartan cuando pasa cerca, como indicio redundante del patrón, sin seleccionar, gastar intentos ni tocar el acopio (INC-60).

Las pruebas encontraron defectos reales en este frente. El 21/09/2026, nueve escenas usaban el mapa por defecto del paquete de entrada, con teclado y mando; se corrigieron y se añadieron cuatro pruebas de arquitectura (Anexo D, apartado 2.4). Una de ellas detectó el mismo defecto en el informe docente; el 29/09/2026 apareció en esa pantalla una variante que la prueba no veía, un mapa por defecto incrustado, y se recableó al del juego (Anexo G, apartado 2.3). El punto PG-05 del guion, que pide verificar que el cambio de esquema de control entre niveles no confunde, exige observar a estudiantes y se trata en el apartado 10.5.

## 5.3 Ajustes al diseño durante el desarrollo

Varias decisiones del equipo hicieron que el código se adelantara a los documentos radicados. Cada divergencia se registró como hallazgo de consistencia, y el 29/09/2026 Santiago Benavides Rey fijó la regla con que se cerraron todos: ante un conflicto gana el juego y se corrige el documento, lo que solo pide el documento se implementa y lo que solo tiene el juego se añade al documento. Los radicados se editaron el 29 y el 30/09/2026 y, para la prueba anticipada de la balsa, el 01/10/2026, cada uno con su fila en el control de cambios. Como los requerimientos recogen hoy lo implementado (RF-15, por ejemplo, se llama «Controles de fuerza y cercanía como hipótesis»), las pruebas que citan RF-15, RF-16, RF-26, RF-27 y RF-29 trazan al texto vigente. La Tabla 5.4 reúne los ajustes que tocan la experiencia del estudiante.

**Tabla 5.4.** Ajustes al diseño introducidos durante el desarrollo.

| Ajuste | Registro | Razón | Estado |
|---|---|---|---|
| N1: el deslizante de posición pasa a reunir y encender con fuerza y cercanía | INC-47 (12 y 15/09) | Decisión del equipo: el deslizante mide fuerza, no distancia | Corregidos guion, RF-14 a RF-16, HU-04 a HU-07 y CU-05 (29/09) |
| N2, fase 1: «Empujar» confirma y sale a la escena 2.2, que anima el rodado | INC-50 (15/09) | La mecánica y la escena repetían el mismo rodado | Corregidos guion, RF-26, CU-06 y HU-08 (29/09) |
| N2, fase 2: la cuerda como séptima pieza y paso final | INC-54 (25/09) | Decisión del equipo sobre el armado | Corregidos guion, RF-27, RF-29, CU-07 y HU-09 (29/09) |
| N2, fase 2: el mazo también perfora al arrastrarlo | INC-61 (14/09) | El mazo no participaba del ensamblaje | Recogido en guion, RF-28, CU-07 y HU-09 (29/09); se conserva el botón de RF-28 |
| N2, fase 3: bloques con cuenta de 1 a 9 y giro a ambos lados | INC-55 (13/09) | Disposición del mockup 10 | Corregidos guion, RF-31, CU-08 y HU-10 (29/09) |
| Pausa: Reanudar, Reiniciar (la fase activa) y Volver al menú de niveles | INC-49 (15/09) | Disposición del mockup 6 | Corregidos RF-07 y HU-17 (29/09) |
| N3: el cierre y la escena final no se omiten al repetir | INC-51 (23/09) | Sin nivel siguiente, el perfil no distingue la primera vuelta | Corregidas HU-14, HU-17 y CU-03 (29/09) |
| El guía se llama Algoritm y cambia de forma por nivel | INC-44, INC-45 (02/09) | El nombre era provisional (PG-02); su cuerpo es el material del descubrimiento | Corregidos guion, OE1, OE2 e historias (14/09); residuos, el 29/09 |
| Algoritm entregado como llama con extremidades | INC-52 (24/09) | Se usa el arte entregado | Corregidos guion y dirección de arte (29/09); estela de luz desde el 30/09 |
| Personajes animados por recorte en la interfaz | INC-53 (24/09) | El paquete 2D Animation no se dibuja sobre el lienzo de interfaz | Corregida la dirección de arte (29/09) |
| N2, fase 2: el mazo soltado lejos no cuenta como intento | INC-115 (30/09) | Un mismo fallo de puntería medía distinto según la pieza | Corregido el juego; recogido en OE1, guion, CU-07 y HU-09 (30/09) |
| N3: «Reiniciar» vuelve a la fase activa también con el nivel completo | INC-117 (30/09) | Al repetir el nivel se volvía a la recolección | Corregido el juego; recogido en HU-17 y la arquitectura (30/09) |
| N3: plano de la mecánica más abierto, a la escala de las narrativas | INC-118 (acta D10) | La orilla se veía a cuatro décimas de la escala de las narrativas | Recogido en dirección de arte, índice de sprites e Interfaces (01/10) |
| N1: la chispa es un solo rayo que nace en el punto del golpe | INC-119 (acta D10) | Mostrar si la chispa cae en las hojas o fuera del montón, como describe el guion | Recogido en dirección de arte e índice de sprites (01/10) |
| N2: la escena 2.2 abre con la caja que deja el bosque | INC-120 (acta D10) | El paso del bosque a la narrativa cambiaba de caja | Corregido el juego; recogido en diseño de cámara del Nivel 2 y dirección de arte (01/10) |
| N2, fase 2: al amarrar la cuerda, la carretilla pasa al dibujo amarrado | INC-121 (acta D10) | El taller terminaba con la cuerda suelta sobre la caja | Corregido el juego; recogido en dirección de arte e índice de sprites (01/10) |
| N3: «Probar balsa» también en base y amarre, sin aprobar la fase | INC-122 (acta D10) | Decisión del equipo: probar la balsa antes de terminarla | Corregidos CP-06, RF-11, RF-40, RF-42, guion (1.8.3 a 1.8.4.1), CU-03, CU-10, HU-12 y HU-13 (01/10) |
| N3: la balsa que se hunde suena | INC-123 (acta D10) | El hundimiento era mudo | Recogido en direcciones de sonido y de arte (01/10) |
| N3: la balsa del cruce navega bajo la espuma de la cascada | INC-124 (acta D10) | Cruzaba sobre la espuma | Excepción expresa a la regla del acta D05; recogido en dirección de arte, índice de sprites e Interfaces (01/10) |
| La escena final suena al bosque de día con las fogatas | INC-125 (acta D10) | La última escena del juego no sonaba | Recogido en la dirección de sonido (01/10) |
| Posterior al corte: el arte final de los personajes trae seis expresiones y extremidades en dos tramos | INC-131 (05/10) | Decisión de Santiago Benavides Rey que levanta INC-108 (una sola cara) y la regla de una sola imagen por forma de Algoritm, solo para el arte final | Recogido en dirección de arte, Interfaces e índice de sprites (05/10); el arte actual conserva la regla anterior |
| Posterior al corte: los brazos van detrás del torso y delante de la cabeza en la familia, y delante del torso al golpear | INC-132 (05 y 06/10) | Con el orden de dibujo anterior los brazos quedaban ocultos tras la cabeza y el cuerpo; en Algoritm las manos van sobre la cara | Corregido el juego (`ArmLayering`, prefabs y clips); sustituye para la familia la regla del cuello de INC-131; la coreografía de Strike con el brazo de una pieza espera el visto bueno de Santiago Benavides Rey |

La mecánica de cada nivel se describe en el capítulo 4. En el Nivel 1, la cercanía se juzga antes que la fuerza, porque sin choque no hay chispa (Anexo B, apartados 2.5 y 3). En el taller, la cuerda soltada antes de la caja se rechaza con «La cuerda todavía no tiene nada que sujetar.», que dice qué falta sin dictar el paso, y el mazo convive con el botón «Mecanizar» que exige RF-28 (Anexo C, apartado 2.3). Los bloques del laberinto conservan la lectura relativa a la orientación de la carretilla que fijó INC-33 (Anexo C, apartado 2.4). En el Nivel 3, la prueba anticipada cuenta como intento y suma a la pista, pero no aprueba la fase, porque solo «Listo» consolida, ni dispara la escena 3.2, reservada al primer fallo de la fase de mástil y vela (véase el apartado 4.4.2; Anexo D, apartado 2.3).

El acta D07 (19/09/2026) precisó la puesta en escena del Nivel 3. El ensamblaje ocurre sobre el río, para que el estudiante vea de dónde salió cada material. El mástil está entre los troncos como trampa deliberada de la base, de modo que el error tenga una causa localizable, y se detecta al validar la base con «Listo» o con la prueba anticipada. La escena 3.2 narra el fallo sin reiniciar el ensamblaje (Anexo D, apartado 3).

# 6. ANDAMIAJE PEDAGÓGICO, RETROALIMENTACIÓN Y REGISTRO

El andamiaje es una capa propia del prototipo, el módulo `Game.Scaffolding`, que comparten los tres niveles. Reúne la política de pistas, el contenido del guía, la regla que decide cuándo se puede omitir una escena narrativa, el reproductor de diálogos y los personajes animados, y sostiene que el guía pregunte y descomponga sin resolver (CP-06). Los demás criterios pedagógicos que no dependen de un reto concreto descansan en otros módulos: la máquina de estados del núcleo, sin estado de derrota (CP-02); los mensajes de cada nivel y el resumen de fin de nivel, que compone la capa de interfaz (CP-03 y, con la escena de cierre, CP-07), y los recolectores de indicadores de cada nivel y el informe docente (CP-09).

## 6.1 El guía Algoritm y las pistas

El guía se llama Algoritm, decisión del 02/09/2026 que cerró el punto PG-02 (INC-44), y cambia de forma en cada nivel porque su cuerpo es el material del descubrimiento (INC-45): es una llama con extremidades en el Nivel 1, y la misma figura recoloreada en madera y en agua en los Niveles 2 y 3 (INC-52; véase el apartado 7.2). Aparece animado en las narrativas, deja al caminar una estela de puntos de luz y ocupa, con la forma de su nivel, el botón de ayuda de las cinco escenas jugables (Anexo F, apartados 2.3 y 2.4). Después del corte, el 05 y el 06/10/2026, su rig pasó a tener brazos, piernas, codos y rodillas en dos tramos, y ojos y boca sobre el cuerpo, sin cuello (INC-131). Sus brazos van delante del cuerpo y sus manos sobre la cara (INC-132), y esas capas nuevas siguen apagadas hasta que llegue el arte final de sus tres formas (Anexo F).

El guía interviene de la misma manera en los tres niveles (INC-62). Al abrir cada nivel formula el objetivo y lo descompone (RF-10); en la llegada al río dice: «Primero, descompongamos el problema: ¿cuántas cosas necesitamos para cruzar?». Durante el juego escribe en la tablilla la instrucción de cada tarea al empezarla y la repite cuando el estudiante la pide. Al cerrar el nivel nombra la habilidad ejercitada (véase el apartado 6.3).

La ayuda durante el juego se rige por la política de pistas (`HintPolicy`), escrita en C# plano y probada sin escena, que mantiene separados los dos mecanismos de RF-13 (Anexo B, apartado 2.3):

- La ayuda a demanda es el botón del guía, que escribe en la tablilla la instrucción de la tarea activa, sin abrir un cuadro que haya que cerrar (INC-63) y sin alterar ningún contador. Si pedir ayuda contara como intento, adelantaría la pista y el andamiaje acabaría resolviendo por el estudiante.
- La pista llega sola al tercer intento fallido consecutivo en una misma tarea, con un umbral fijo; orienta con una pregunta y nunca nombra la respuesta. Un acierto o el cambio de tarea reinician la cuenta.

La política es por fase, porque el Nivel 2 tiene tres tareas distintas, y las pistas de ese nivel figuran en el guion desde el 29/09/2026 (INC-64). En el primer momento del Nivel 1, «Pista» dibuja además el círculo de reunión. En la recolección del Nivel 3 recoger no falla, así que solo opera la ayuda a demanda; en el ensamblaje cuentan para la pista la fase rechazada y la prueba de la balsa fallida, también la anticipada (INC-122). El botón de ayuda del Nivel 1 pulsa de forma continua y los de los Niveles 2 y 3 no pulsan, como recoge la dirección de arte. La Tabla 6.1 reúne las tareas del guía con un ejemplo de pista.

**Tabla 6.1.** Tareas del guía por nivel y ejemplo de pista.

| Nivel | Tareas del guía | Ejemplo de pista |
|---|---|---|
| 1 | Reunir, Golpear, Soplar | «Las chispas dependen de cómo chocan las piedras: de la fuerza y de qué tan juntas están. ¿Qué cambiarías antes del próximo golpe?» |
| 2 | Seleccionar, Construir, Programar | «Mira en qué paso se detuvo la carretilla. ¿Hacia dónde miraba justo antes?» |
| 3 | Recolectar, Base, Amarre, Mástil y vela | «¿Qué parte de la balsa toca el agua y sostiene todo lo demás?» |

Los textos viven en un recurso de contenido por nivel, editable sin recompilar (CT-05). Tres pruebas `HintPolicy_CP06_…` vigilan que ninguna pista nombre la respuesta, y el mismo criterio alcanza los mensajes de las mecánicas. El taller dice qué falta y no cuál es el paso correcto; la balsa dice qué revisar y no qué pieza va, y la prueba anticipada señala solo lo mal puesto, nunca los espacios vacíos, porque marcarlos daría un mapa de dónde va cada pieza. Otra prueba comprueba por reflexión que el resultado de ejecutar la secuencia del laberinto no expone el bloque a corregir (`SequenceExecutor_CP06_ElResultadoNoNombraElBloqueQueDebeCorregirse`).

## 6.2 Retroalimentación sin derrota ni puntajes

Cada invariante pedagógico está vigilado por pruebas cuyo nombre traza el criterio que verifican (véase el apartado 9.1), y donde el código rechaza el camino obvio por uno de ellos, un comentario deja escrita la razón pedagógica. La Tabla 6.2 resume las garantías.

**Tabla 6.2.** Garantías de retroalimentación no punitiva.

| Garantía | Cómo la cumple el prototipo | Criterio |
|---|---|---|
| Sin pantalla de derrota | Ninguno de los nueve estados del flujo es de derrota | CP-02 |
| Intentos ilimitados | Ninguna mecánica limita intentos, bloques ni ejecuciones | RF-18, CP-02 |
| Lo aprobado no se pierde | Fases, tareas y desbloqueos nunca retroceden | RF-41, RF-43 |
| Respuesta descriptiva | Cada mensaje narra lo ocurrido sin calificarlo | RF-11, RF-17 |
| Ni sonido ni gesto de fallo | Sonidos descriptivos (la balsa que se hunde suena a salpicadura); ningún sonido de error; tras un fallo, gesto de ánimo | CP-02 |
| Sin puntajes ni cifras | No hay clase ni campo de puntaje; los indicadores no llegan al estudiante | CP-03, RF-45 |

Ninguno de los nueve estados del flujo (véase el apartado 3.3) es de derrota (`GameFlow_CP02_NoExisteEstadoDeDerrota`). Hay pruebas de intentos sin límite en el encendido, el laberinto y la balsa, y ninguna ruta del menú de pausa lleva a una pantalla de derrota (`PauseMenu_CP02_NingunaRutaDeLaPausaLlevaAUnaPantallaDeDerrota`).

Cada mensaje narra una consecuencia observable: «Las piedras apenas se rozan. No sale ninguna chispa.» (cueva); «Esta piedra tiene esquinas. Cuando la empujas, se traba.» (bosque); «La tabla no tiene sobre qué apoyarse todavía.» (taller); «La carretilla no llegó al refugio. Mira dónde se detuvo y corrige tu secuencia.» (laberinto), y «La balsa se volteó. Mira el espacio marcado: ¿qué debería ir ahí?» (río). La prueba anticipada sin piezas mal puestas responde «La balsa se hundió: todavía no está terminada. ¿Qué parte falta armar?». En el Nivel 1, desde el segundo fallo seguido el mensaje alterna entre dos variantes, para que el mismo texto no aparezca dos veces seguidas (RF-18). Los estados de error llevan un segundo indicador además del color (RNF-19; véase el apartado 7.4).

Un fallo deshace solo lo del intento y nunca la fase activa (INC-65). Un paso fuera de orden en el taller no deshace lo armado; una prueba fallida de la balsa, también la anticipada, devuelve al inventario solo las piezas mal ubicadas y conserva las fases aprobadas (`RaftAssembly_RF41_ProbarLaBalsaNoTocaLasFasesAprobadas`); una tarea marcada no se desmarca, y el perfil no sobrescribe una fase confirmada ni vuelve a bloquear un nivel. Desde el 25/09/2026 las dos primeras fases de la balsa se guardan en el instante en que se aprueban, antes de su animación, para que reiniciar a mitad de ella no pierda lo aprobado (Anexo D, apartado 2.3).

El primer pilar de la dirección de sonido es que un intento sin éxito suena de forma descriptiva y nunca punitiva, y ninguna pieza de audio se llama derrota ni error (véase el apartado 7.3). Desde el 01/10/2026 la balsa que se hunde suena a salpicadura (INC-123): el sonido describe lo que le pasa a la balsa, y «Listo» rechazado y la pieza que vuelve al inventario siguen en silencio. Los personajes siguen la misma regla: tras un fallo ejecutan el gesto de ánimo, y ningún clip es de derrota, caída ni salto (véase el apartado 7.2).

No existe clase de puntaje en el proyecto (`LevelSummary_CP03_NoExisteClaseDePuntajeEnElProyecto`) ni miembro de puntaje en el perfil. Tres pruebas comprueban por reflexión que los indicadores de ningún nivel llegan a la interfaz del estudiante, y un barrido recorre diez tipos de contenido en busca de cifras en los textos que el estudiante ve (Anexo G, apartado 2.2). Los números que sí aparecen en pantalla no miden desempeño: el contador de avance que exige RF-24 en el bosque («Troncos redondos: n de 5») o la cuenta de casillas que el estudiante asigna a un bloque del laberinto.

## 6.3 Cierre reflexivo de cada nivel

CP-07 y RF-12 piden que, al cerrar cada nivel, el guía nombre la habilidad de pensamiento computacional ejercitada y la relacione con lo que el jugador hizo. El prototipo lo hace en dos momentos: la escena narrativa de cierre, en la que Algoritm la nombra en diálogo, y el resumen de fin de nivel (mockups 13 y 13b). El resumen es una tablilla con el título, el hallazgo del nivel, dos frases que relatan lo que hizo el estudiante y la habilidad nombrada en un recuadro verde. Las dos frases se eligen entre variantes fijas según haya habido intentos sin avance y errores corregidos, sumados sobre todas las fases del nivel, sin que la suma produzca ninguna cifra visible (Anexo C, apartado 2.5). La Tabla 6.3 recoge lo que se nombra al cerrar cada nivel.

**Tabla 6.3.** Habilidad nombrada al cerrar cada nivel.

| Nivel | Escena de cierre | Lo que nombra Algoritm | Frase del resumen |
|---|---|---|---|
| 1 | 1.3, el nacimiento del fuego | «se llama iterar. Probar, mirar el resultado y ajustar» | «Eso se llama probar y ajustar…» |
| 2 | 2.5, cierre del nivel 2 | «Eso se llama abstraer»; «eso es pensar como un algoritmo» | «Eso se llama abstraer y pensar como un algoritmo…» |
| 3 | 3.3, el cruce, y escena final | «dividieron el problema en partes pequeñas»; «Eso es pensar computacionalmente» | «Eso se llama descomponer y depurar…» |

La escena final generaliza los tres niveles en voz de Algoritm: «Tres problemas. Tres formas distintas de pensar.». Tres pruebas, una por nivel, exigen que el cierre nombre la habilidad. Otra exige que ningún resumen contenga un dígito, y los barridos de los tres resúmenes reales exigen cero juicios de valor y ninguna oración de más de veinte palabras. Por esa regla, desde el 29/09/2026 el hallazgo del Nivel 3 se escribe en dos oraciones: «Descubriste que una balsa grande se arma por partes. Primero la base, luego el amarre y al final el mástil con la vela.» (INC-110).

El cierre reflexivo no puede omitirse la primera vez (`LevelSummary_CP07_ElCierreReflexivoNoEsOmitibleLaPrimeraVez`). Como la lista cerrada de datos persistidos (RNF-09) no incluye las escenas vistas, un cierre reflexivo cuenta como visto cuando el nivel siguiente ya está alcanzado; por eso el flujo muestra primero el cierre y después desbloquea. Desde el 29/09/2026 el recorrido automatizado del Nivel 2 exige llegar a la escena 2.5 con el Nivel 3 todavía bloqueado y sin «Omitir», y ese día se corrigió un doble clic que, al salir de un cierre ya visto, saltaba el resumen (`NarrativeScene_RF45_UnClicDeMasAlSalirDelCierreReflexivoYaVistoLlegaAlResumen`). El Nivel 3 no tiene nivel siguiente, así que el cruce y la escena final se leen enteros siempre, como recoge HU-14 desde el 29/09/2026 (INC-51; Anexo D, apartado 2.4).

Al mostrarse, el resumen confirma la fase del Nivel 1, desbloquea el nivel siguiente y guarda el perfil (INC-74). «Continuar» y «Volver al menú de niveles» llevan al menú de niveles en los Niveles 1 y 2, y en el Nivel 3 a la escena final y de ahí a los créditos (INC-73, INC-39).

## 6.4 Progreso, indicadores de desempeño e informe docente

El estudiante crea un perfil con un nombre o alias y ningún otro dato (RF-02). El avance se guarda al confirmar cada fase (RF-04); la única fase del Nivel 1 se guarda, con el desbloqueo del nivel siguiente, al mostrarse el resumen (INC-74). Hay siete fases persistidas: una en el Nivel 1 y tres en cada uno de los otros dos. La recolección del Nivel 3 no se persiste: al retomar el amarre o el mástil se abre directamente el ensamblaje (INC-88). Un nivel queda desbloqueado cuando el nivel alcanzado lo marca o cuando todas las fases del anterior están confirmadas (RF-03); desde el 30/09/2026 el menú y el flujo derivan esa regla del progreso guardado, sin campo nuevo (RNF-09), y un cierre durante la escena de cierre del Nivel 2 ya no deja bloqueado el Nivel 3 (INC-116). Tras un cierre inesperado el juego retoma en la primera fase pendiente (RNF-14), y repetir un nivel conserva los indicadores de la primera vez (INC-75). La forma y la ubicación portable del archivo de perfil se describen en el apartado 3.5.

RF-45 obliga a registrar, por perfil y por fase, los cuatro indicadores de OE1 (apartado 3.6.1): intentos, errores corregidos, pasos utilizados y tiempo de resolución. La lista es cerrada, y una prueba la fija por reflexión. Cada nivel tiene un recolector propio en C# plano, probado sin escena con un reloj simulado; el tiempo excluye la pausa, y reiniciar una fase no borra lo registrado. La Tabla 6.4 recoge la definición implementada, que desde el 29/09/2026 es también la de OE1 (INC-47 e INC-59).

**Tabla 6.4.** Definición implementada de los cuatro indicadores por nivel.

| Indicador | Nivel 1 | Nivel 2 | Nivel 3 |
|---|---|---|---|
| Intentos | Golpes no efectivos | F1: objetos no válidos. F2: pasos fuera de orden; una pieza soltada lejos, el mazo incluido, no cuenta. F3: ejecuciones que no llegan | Fases rechazadas y pruebas de la balsa fallidas, también las anticipadas |
| Errores corregidos | Golpe efectivo justo tras uno fallido | F1 y F2: uno o varios rechazos seguidos y la acción aceptada que los cierra. F3: ediciones entre una ejecución fallida y la siguiente | Pieza devuelta que se recoloca bien |
| Pasos utilizados | Golpes efectivos al habilitar «Soplar» | F1: no se cuentan; se registra 0. F2: acciones en orden. F3: bloques de la secuencia que llega | Fases aceptadas, máximo tres |
| Tiempo de resolución | Sin la pausa | Por fase, sin la pausa | Por fase, sin la pausa ni la escena 3.2; el de la base, desde que se abre el ensamblaje |

Tres definiciones se explican por la mecánica. En el Nivel 1, acertar justo después de fallar implica haber cambiado la fuerza o la cercanía, de modo que no hace falta guardar qué combinación se usó. En el bosque la selección por patrón no tiene una secuencia de solución, y los pasos se registran en cero. El Nivel 3 usa un solo recolector para sus tres fases, porque OE1 define sus pasos sobre el nivel entero y los demás conteos se acumulan entre fases; solo el tiempo es propio de cada fase (Anexo D, apartado 2.3).

El informe docente (RF-46) lo implementó Santiago Valdiri García en el Slice 4, el 23 y el 24/09/2026 (commit `9c34924`, fusionado con el PR #85). Se accede desde el botón «Progreso del equipo» del menú principal. Lista los perfiles de las dos rutas de almacenamiento, sin duplicados, y presenta los del perfil elegido, bajo un encabezado que lo nombra («Perfil de …») y que se actualiza al elegir otro y tras un borrado: una fila por fase con los cuatro indicadores, «Sin datos» en un nivel no jugado, el tiempo en minutos y segundos y ningún total ni promedio (INC-86). Sin perfiles, lo informa y ofrece volver al menú. Consultarlo no altera ningún perfil, y ninguna ruta del estudiante llega a esa pantalla, la única con cifras. El módulo depende solo del núcleo, de modo que retirar un nivel no puede romperlo (RNF-16; véase el apartado 3.2). Desde el 30/09/2026 usa Baloo 2 y Nunito, con 26 px como mínimo (INC-79), responde al mapa de controles del juego (INC-80) y, si el guardado cayó a la ruta de respaldo, avisa al docente de dónde quedaron los perfiles (INC-34). El acta D10 (30/09/2026) aceptó como definitivos los iconos de los cuatro indicadores y estableció que el informe no lleva protección de acceso, porque ningún requerimiento la pide (Anexo G, apartado 2.3).

La eliminación de datos (RF-47), el derecho de supresión, tiene dos accesos y un solo borrado (INC-85). El estudiante puede borrar su perfil desde el panel «¿Quién juega?», con la papelera de cada perfil y la pregunta «¿Borras el perfil de…?», que se responde con «Conservar» o «Borrar»; el docente puede eliminar los datos de un estudiante desde el informe, ante la advertencia de que la acción no se puede deshacer. Los dos invocan la misma operación del núcleo (`ProfileSession.Delete`, sobre `SaveStore.Delete`), que elimina el perfil en las dos rutas de almacenamiento y existía, probada, desde el Slice 1. Los textos de los dos diálogos viven en datos desde el 30/09/2026 (INC-104). Las pruebas cubren las dos rutas y la preservación de los demás perfiles (Anexo G, apartado 2.4); el borrado sobre el ejecutable se presenta en el apartado 9.5.

Con RF-46 y RF-47, implementados el 24/09/2026, el prototipo cubre los 47 requerimientos funcionales del proyecto (Anexo G, apartado 1).
