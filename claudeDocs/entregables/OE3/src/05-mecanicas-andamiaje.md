# 5. MECÁNICAS IMPLEMENTADAS

Este capítulo contrasta las mecánicas de juego que especificó el guion del segundo objetivo (Solución OE2, §1.4.3, §1.6 y §1.8, con la trazabilidad de su §1.11) con las que contiene el prototipo en el estado verificado del 25/09/2026 (commit `ccf77e6`). El capítulo 4 describe cada reto desde la perspectiva del estudiante; aquí se registra qué interacción lo sostiene, qué cambió respecto del diseño radicado y qué prueba automatizada lo verifica. Es la evidencia del primer indicador del objetivo, la tasa de implementación de mecánicas (véase §1.3).

## 5.1 Mecánicas planeadas y mecánicas implementadas

El guion especifica siete mecánicas principales —una para el Nivel 1 (§1.4.3), una por cada fase del Nivel 2 (§1.6.1 a §1.6.3) y tres para el Nivel 3 (§1.8.2 a §1.8.4)— y dos mecanismos transversales: las escenas narrativas y la ayuda del guía. El prototipo las aloja en cinco escenas jugables: la cueva, el bosque, el taller, el laberinto y la orilla del río. En la Tabla 5.1, la columna «Evidencia» indica los requerimientos que cada mecánica atiende y una clase de pruebas representativa entre las que los nombran.

**Tabla 5.1.** Mecánicas planeadas en el guion frente a las implementadas en el prototipo.

| Mecánica | Nivel o fase | Planeada en OE2 | Implementada | Evidencia |
|---|---|---|---|---|
| Encendido por hipótesis y experimento | N1 | §1.4.3: deslizante de posición, «Golpear» y «Soplar» | Sí: reunir por arrastre y encender con fuerza y cercanía (INC-47) | RF-14 a RF-21; `FirePanelTests` |
| Selección de objetos por patrón | N2, fase 1 | §1.6.1.2: clic, contador, carga y «Empujar» | Sí; el rodado se ve en la escena 2.2 (INC-50) | RF-22 a RF-26; `PatternSelectionTests` |
| Ensamblaje secuencial de la carretilla | N2, fase 2 | §1.6.2.2: seis piezas y seis pasos | Sí; séptima pieza, la cuerda (INC-54) | RF-27 a RF-29; `AssemblySequenceTests` |
| Programación por bloques | N2, fase 3 | §1.6.3.2: tres bloques, «Ejecutar» y retroceso | Sí; bloques con cuenta y lado de giro | RF-30 a RF-34; `SequenceExecutorTests` |
| Recolección con desplazamiento | N3 | §1.8.2: flechas, «Recoger», inventario y lista | Sí | RF-35 a RF-39; `RiverMovementTests` |
| Ensamblaje por fases bloqueantes | N3 | §1.8.3: base, amarre, mástil y vela | Sí, sobre el río | RF-40, RF-41; `RaftAssemblyTests` |
| Prueba y depuración de la balsa | N3 | §1.8.4: hundimiento, resaltado y devolución | Sí; escena 3.2 al primer fallo | RF-42, RF-43; `RaftValidatorTests` |
| Escenas narrativas con diálogo | Todos | §1.2: ilustración y diálogo secuencial | Sí, en 18 secuencias | RF-05, RF-06; `NarrativeSceneTests` |
| Ayuda y pista del guía | Todos | §1.3 y §1.4.3.6 | Sí, en las cinco escenas jugables | RF-13; `HintPolicyTests` |

Las siete mecánicas principales están implementadas —una tasa de siete sobre siete frente a lo planeado— y también los dos mecanismos transversales. Cuatro de las siete difieren del texto radicado —el encendido del Nivel 1, el rodado de la caja del bosque, el armado del taller y los bloques del laberinto—, cada una por una decisión documentada (véase §5.3). Ningún requerimiento de los módulos C, D y E de OE1 (RF-14 a RF-44) quedó sin una prueba que lo nombre, y una prueba de trazabilidad lo comprueba en cada corrida sobre los 47 RF (véase §9.4 y Anexo A).

El indicador del trabajo de grado exige «al menos tres mecánicas principales» y pone como ejemplos el movimiento, los diálogos y la lógica de bloques. El marco conceptual del mismo documento (§3.1.5) clasifica las mecánicas de un videojuego en seis tipos; la Tabla 5.2 ubica en ellos lo implementado.

**Tabla 5.2.** Tipos de mecánica del trabajo de grado (§3.1.5) presentes en el prototipo.

| Tipo de mecánica | Dónde se implementa | Estado |
|---|---|---|
| Movimiento | Desplazamiento de Mamá por la orilla (N3) | Implementada |
| Interacción | Seleccionar, arrastrar, recoger y avanzar diálogos (todos) | Implementada |
| Secuenciación de acciones | Ensamblaje de la carretilla y secuencia de bloques (N2) | Implementada |
| Resolución de puzzles | Hipótesis del fuego (N1), patrón (N2) y depuración de la balsa (N3) | Implementada |
| Sistema de progreso | Desbloqueo por nivel y guardado por fase (RF-03, RF-04) | Implementada |
| Optimización de soluciones | — | Excluida por diseño |

El prototipo cubre cinco de los seis tipos, entre ellos los que abarcan los tres ejemplos del indicador: movimiento; interacción, que incluye los diálogos («hablar», según §3.1.5), y secuenciación de acciones, que incluye la lógica de bloques. El umbral de tres mecánicas principales queda superado. La optimización de soluciones se excluyó a propósito: el trabajo de grado la define como premiar con mejores recompensas o mayores puntajes la solución más eficiente, lo que choca con CP-03. La única propuesta del guion en esa dirección, el nivel avanzado opcional de §1.10 (menor número de pasos bajo presión de tiempo), quedó fuera del alcance por no tener requerimiento asociado y por su tensión con CP-02.

## 5.2 Formas de interacción y control

RNF-02 y CT-06 limitan la interacción a clic y clic sostenido, sin combinaciones de teclas, doble clic ni controles simultáneos. El hallazgo INC-01 retiró de los documentos toda mención de teclas de dirección. La Tabla 5.3 resume el esquema implementado.

**Tabla 5.3.** Esquema de control del prototipo.

| Control | Acción | Dónde |
|---|---|---|
| Clic | Seleccionar, confirmar, avanzar el diálogo y pulsar botones | Todas las pantallas |
| Clic sostenido y soltar | Arrastrar una pieza, la caja, un bloque o un material | N1 (reunir), N2 (tres fases), N3 (ensamblaje) |
| Clic o clic sostenido sobre un deslizante | Elegir la fuerza del golpe y la cercanía de las piedras | N1 (encendido) |
| Clic sostenido sobre una flecha | Desplazar a Mamá mientras se mantiene | N3 (recolección) |
| Clic sobre los botones de subir y bajar | Desplazar la secuencia de bloques cuando desborda | N2, fase 3 |

El cumplimiento está garantizado en el código y vigilado por pruebas:

- **Un solo mapa de controles.** Todas las escenas con módulo de entrada y el propio proyecto usan `ControlesJugables.inputactions`, sin vinculaciones de teclado, mando ni rueda del ratón, y ningún módulo usa la clase de entrada legada de Unity; pruebas de arquitectura vigilan las dos condiciones (véase §9.1).
- **Las flechas del Nivel 3 son botones** de interfaz que responden al clic sostenido; el módulo del nivel ni siquiera referencia el sistema de entrada, y una prueba vigila que no gane esa referencia (Anexo D, §3).
- **«Ejecutar» responde a un clic simple**: el documento fuente del Nivel 2 pedía doble clic, y el guion lo normalizó al cerrar el punto abierto PG-04.
- **Las listas se desplazan con botones.** Un panel desplazable por arrastre competiría con el arrastre de la mecánica; por eso la secuencia del laberinto se desplaza con dos botones desde el 17/09/2026 y, desde el 23/09/2026, también con una barra que se pulsa pero no se arrastra (Anexo C, §A.6).
- **El cursor no es un control.** En el bosque los objetos se apartan cuando el cursor pasa cerca —lo redondo rueda lejos, lo anguloso vuelca y se detiene—, como indicio redundante del patrón; acercar el cursor no selecciona, no gasta intentos ni toca el acopio.

Las pruebas encontraron defectos reales en este frente. El 21/09/2026, nueve escenas usaban el mapa de acciones por defecto del paquete de entrada, con teclado y mando, y el mapa del proyecto era una plantilla con 23 vinculaciones de teclado; se corrigieron y se añadieron cuatro pruebas de arquitectura (Anexo D, §5). Durante el Slice 4, una de ellas detectó el mismo defecto en la pantalla nueva del informe docente (Anexo G, §5).

Quedan dos puntos abiertos. Las pantallas del informe docente y de créditos desplazan su contenido con paneles de arrastre; son pantallas no jugables y está pendiente sustituirlos o registrar la excepción (Anexo G, §A.4; véase §10.4). Y el punto PG-05 del guion —verificar que el cambio de esquema de control entre niveles no confunde— exige observación con estudiantes (véase §10.5).

## 5.3 Ajustes al diseño durante el desarrollo

Varias decisiones del equipo hicieron que el código se adelantara a los documentos radicados. Las divergencias quedaron registradas como hallazgos en el registro de inconsistencias del proyecto, con su razón y la corrección documental pendiente, que se aplica a mano y nunca desde el código; la excepción son los parámetros de los bloques del laberinto, que dejan RF-31 por precisar (Anexo C, §8). Las pruebas conservan en su nombre el requerimiento original cuando su intención se mantiene: las del Nivel 1 siguen citando RF-15 y RF-16 porque la secuencia hipótesis, experimento y resultado observable no cambió. La Tabla 5.4 reúne los ajustes que tocan la experiencia del estudiante.

**Tabla 5.4.** Ajustes al diseño introducidos durante el desarrollo.

| Ajuste | Registro | Razón | Estado |
|---|---|---|---|
| N1: el deslizante de posición pasa a reunir y encender con fuerza y cercanía | INC-47 (12 y 15/09) | Decisión del equipo: el deslizante mide fuerza, no distancia | Abierto: guion, RF-15, RF-16, HU-06 |
| N2, fase 1: «Empujar» confirma y sale a la escena 2.2, que anima el rodado | INC-50 (15/09) | La mecánica y la escena repetían el mismo rodado | Abierto: RF-26, HU-08, CU-06 |
| N2, fase 2: la cuerda como séptima pieza y paso final | INC-54 (25/09) | Decisión del equipo sobre el armado | Abierto: guion, RF-27, RF-29, CU-07 |
| N2, fase 2: el mazo también perfora al arrastrarlo | Sin hallazgo (14/09) | El mazo no participaba del ensamblaje | Se conserva el botón de RF-28 |
| N2, fase 3: bloques con cuenta de 1 a 9 y giro a ambos lados | Sin hallazgo (13/09) | Disposición del mockup 10 | RF-31 por precisar |
| Pausa: Reanudar, Reiniciar (la fase activa) y Volver al menú de niveles | INC-49 (15/09) | Disposición del mockup 6 | Abierto: HU-17 |
| N3: el cierre y la escena final no se omiten al repetir | INC-51 (23/09) | Sin nivel siguiente, el perfil no distingue la primera vuelta | Abierto (aceptado 23/09): HU-14 |
| El guía se llama Algoritm y cambia de forma por nivel | INC-44, INC-45 (02/09) | El nombre era provisional (PG-02); su cuerpo es el material del descubrimiento | Cerrado (14/09); residuos INC-44-r e INC-44-r2 |
| Algoritm entregado como llama con extremidades | INC-52 (24/09) | Se usa el arte entregado | Abierto |
| Personajes animados por recorte en la interfaz | INC-53 (24/09) | El paquete 2D Animation no se dibuja sobre el lienzo de interfaz | Abierto |

Los cinco primeros ajustes son de mecánica:

- **Nivel 1 (INC-47).** Desde el 12/09/2026 el deslizante mide la fuerza del golpe en diez muescas, efectivo en la siete y la ocho. El 15/09 el nivel se dividió en dos momentos: reunir las hojas, el sílex y el pedernal dentro de un círculo y encender con dos deslizantes, fuerza y cercanía de las piedras. El golpe solo prende con las dos condiciones a la vez, y la cercanía se juzga primero porque sin choque no hay chispa (Anexo B, Fases 5 y 6, §2 y §4).
- **Bosque (INC-50).** El requisito de fondo se cumple —el estudiante ve rodar la caja sobre los troncos redondos—; cambia el lugar, que pasa a ser la escena narrativa 2.2 (Anexo C, §A.1).
- **Taller (INC-54 y mazo).** Soltar la cuerda antes de la caja se rechaza con «La cuerda todavía no tiene nada que sujetar.», que dice qué falta sin dictar el paso (Anexo C, §A.5). El mazo inerte lo reveló el grafo de conocimiento del proyecto; se sumó el gesto de arrastre y se conservó el botón «Mecanizar» que exigen RF-28 y el guion (Anexo C, §4).
- **Laberinto.** Los bloques con parámetro siguen el mockup 10 (decisión del equipo, 13/09/2026) y conservan la lectura relativa a la orientación de la carretilla que fijó INC-33, pero no el giro siempre horario que fijaban ese hallazgo y RF-31: el prototipo gira a ambos lados.

En el Nivel 3, el acta D07 (19/09/2026; Anexo H) precisó la puesta en escena sin apartarse de los requerimientos: el ensamblaje ocurre sobre el río y no en una ventana superpuesta, para que el estudiante vea de dónde salió cada material; entre los troncos hay uno más largo, el mástil, como trampa deliberada de la base, de modo que el error tenga una causa localizable —en el prototipo se detecta al validar la base, antes de la prueba de la balsa (véase §4.4.2)—; y la escena 3.2 narra el fallo sin reiniciar el ensamblaje (Anexo D, §2). Siguen abiertos INC-46, sobre la forma visual de la lista de tareas del Nivel 3, e INC-48, sobre el capítulo de arquitectura (véase §3.1).

# 6. ANDAMIAJE PEDAGÓGICO, RETROALIMENTACIÓN Y REGISTRO

El andamiaje es una capa propia del prototipo, el módulo `Game.Scaffolding`, que comparten los tres niveles: reúne la política de pistas, el contenido del guía, la regla que decide cuándo se puede omitir una escena narrativa, el reproductor de diálogos y los personajes animados, y sostiene que el guía pregunte y descomponga sin resolver (CP-06). Los demás criterios pedagógicos que no dependen de un reto concreto descansan en otros módulos: la máquina de estados del núcleo, sin estado de derrota (CP-02); los mensajes de cada nivel y el resumen de fin de nivel, que compone la capa de interfaz (CP-03 y, con la escena de cierre, CP-07); y los recolectores de indicadores de cada nivel y el informe docente (CP-09).

## 6.1 El guía Algoritm y las pistas

El guía se llama Algoritm —decisión del 02/09/2026 que cerró el punto abierto PG-02 (INC-44)— y cambia de forma en cada nivel: su cuerpo es el material del descubrimiento, fuego en el Nivel 1, rueda en el 2 y gota de agua en el 3 (INC-45). Su arte entregado se aparta de la dirección de arte y las formas de rueda y gota son todavía provisionales (INC-52; véase §7.2). Algoritm aparece animado en las narrativas y ocupa, con la forma de su nivel, el botón de ayuda de las cinco escenas jugables (Anexo F).

El guía interviene en tres momentos. Al abrir cada nivel formula el objetivo con preguntas y lo descompone (RF-10); en la llegada al río dice: «Primero, descompongamos el problema: ¿cuántas cosas necesitamos para cruzar?». Durante el juego solo interviene si el estudiante lo pide o acumula fallos. Al cerrar, nombra la habilidad ejercitada (véase §6.3).

La ayuda durante el juego se rige por la política de pistas (`HintPolicy`), escrita en C# plano y probada sin escena, que mantiene separados los dos mecanismos de RF-13 (Anexo B, Fase 2, §2.4):

- **Ayuda a demanda.** El botón del guía repite la instrucción de la tarea activa sin alterar ningún contador. Si pedir ayuda contara como intento, adelantaría la pista y el andamiaje acabaría resolviendo por el estudiante.
- **Pista.** Llega sola al tercer intento fallido consecutivo en una misma tarea; orienta con una pregunta y nunca nombra la respuesta. Un acierto o el cambio de tarea reinician la cuenta.

La política es por fase, porque el Nivel 2 tiene tres tareas distintas (Anexo C, §4). En el primer momento del Nivel 1, «Pista» dibuja además el círculo de reunión, una orientación visual que no mueve ninguna pieza. En la recolección del Nivel 3 no existe el intento fallido —recoger no falla—, así que allí solo opera la ayuda a demanda. La Tabla 6.1 recoge las tareas del guía en cada nivel con un ejemplo de pista.

**Tabla 6.1.** Tareas del guía por nivel y ejemplo de pista.

| Nivel | Tareas del guía | Ejemplo de pista |
|---|---|---|
| 1 | Reunir, Golpear, Soplar | «Las chispas dependen de cómo chocan las piedras: de la fuerza y de qué tan juntas están. ¿Qué cambiarías antes del próximo golpe?» |
| 2 | Seleccionar, Construir, Programar | «Mira en qué paso se detuvo la carretilla. ¿Hacia dónde miraba justo antes?» |
| 3 | Recolectar, Base, Amarre, Mástil y vela | «¿Qué parte de la balsa toca el agua y sostiene todo lo demás?» |

Los textos viven en un recurso de contenido por nivel, editable sin recompilar (CT-05). Tres pruebas `HintPolicy_CP06_…` vigilan que ninguna pista nombre la respuesta en ninguno de los tres niveles, y el mismo criterio alcanza los mensajes de las mecánicas: el taller dice qué falta y no cuál es el paso correcto; la prueba de la balsa dice qué revisar y no qué pieza va; y una prueba comprueba por reflexión que el resultado de ejecutar la secuencia del laberinto no expone ninguna propiedad que nombre el bloque a corregir. Queda pendiente que el botón de ayuda pulse tras tres fallos, como pide la dirección de arte: hoy solo pulsa el del Nivel 1, y siempre (Anexo F; véase §10.4).

## 6.2 Retroalimentación sin derrota ni puntajes

Los invariantes pedagógicos no son preferencias de diseño: cada uno está implementado y vigilado por pruebas, cuyo nombre traza el criterio que verifican (véase §9.1), y donde el código rechaza el camino obvio por uno de ellos, un comentario deja escrita la razón pedagógica para que una mejora posterior no reintroduzca la derrota o el marcador. La Tabla 6.2 resume las garantías.

**Tabla 6.2.** Garantías de retroalimentación no punitiva.

| Garantía | Cómo la cumple el prototipo | Criterio |
|---|---|---|
| Sin pantalla de derrota | Ninguno de los nueve estados del flujo es de derrota | CP-02 |
| Intentos ilimitados | Ninguna mecánica limita intentos, bloques ni ejecuciones | RF-18, CP-02 |
| Lo aprobado no se pierde | Fases, tareas y desbloqueos nunca retroceden | RF-41, RF-43 |
| Respuesta descriptiva | Cada mensaje narra lo ocurrido sin calificarlo | RF-11, RF-17 |
| Ni sonido ni gesto de fallo | Sonidos descriptivos; tras un fallo, gesto de ánimo | CP-02 |
| Sin puntajes ni cifras | No hay clase ni campo de puntaje; los indicadores no llegan al estudiante | CP-03, RF-45 |

**Sin derrota.** Ninguno de los nueve estados del flujo (véase §3.3) es de derrota (`GameFlow_CP02_NoExisteEstadoDeDerrota`); hay pruebas de intentos sin límite en el encendido, el laberinto y la balsa, y ninguna ruta del menú de pausa lleva a una pantalla de derrota.

**Mensajes que describen.** Cada mensaje narra una consecuencia observable: «Las piedras apenas se rozan. No sale ninguna chispa.» (cueva); «Esta piedra tiene esquinas. Cuando la empujas, se traba.» (bosque); «La tabla no tiene sobre qué apoyarse todavía.» (taller); «La carretilla no llegó al refugio. Mira dónde se detuvo y corrige tu secuencia.» (laberinto); «La balsa se volteó. Mira el espacio marcado: ¿qué debería ir ahí?» (río). En el Nivel 1, los mensajes de fallo escalan tras dos fallos seguidos del mismo tipo y no se repiten dos veces seguidas. Los estados de error se distinguen con un segundo indicador además del color (RNF-19; véase §7.4).

**Lo aprobado permanece.** Un paso fuera de orden en el taller no deshace lo armado; una prueba fallida de la balsa devuelve al inventario solo las piezas mal ubicadas y conserva las fases aprobadas; una tarea marcada no se desmarca; el perfil no sobrescribe una fase confirmada ni vuelve a bloquear un nivel; y «Reiniciar», en la pausa, repite solo la fase activa. Desde el 25/09/2026 las dos primeras fases de la balsa se guardan en el instante en que se aprueban, antes de su animación, para que reiniciar a mitad de ella no pierda lo aprobado (Anexo D, §A.4).

**Sonido e imagen sin castigo.** El primer pilar de la dirección de sonido es que un intento sin éxito suena de forma descriptiva y nunca punitiva, y ninguna pieza de audio del proyecto se llama derrota ni error (véase §7.3). Los personajes siguen la misma regla: tras un fallo ejecutan el gesto de ánimo, y ningún clip es de derrota, caída ni salto (véase §7.2).

**Sin puntajes.** No existe clase de puntaje en el proyecto (`LevelSummary_CP03_NoExisteClaseDePuntajeEnElProyecto`) ni miembro de puntaje en el perfil. Tres pruebas comprueban por reflexión que los indicadores de ningún nivel llegan a la interfaz del estudiante, y un barrido recorre diez tipos de contenido en busca de cifras en los textos que el estudiante ve (Anexo G, §A.5). Los números que sí aparecen en pantalla no miden desempeño: el contador de avance que exige RF-24 en el bosque («Troncos redondos: n de 5») o la cuenta de casillas que el estudiante asigna a un bloque del laberinto.

## 6.3 Cierre reflexivo de cada nivel

CP-07 y RF-12 piden que, al cerrar cada nivel, el guía nombre la habilidad de pensamiento computacional ejercitada y la relacione con lo que el jugador hizo. El prototipo lo hace en dos momentos. Primero, la escena narrativa de cierre, en la que Algoritm la nombra en diálogo. Después, el resumen de fin de nivel (mockups 13 y 13b): una tablilla con un título, el hallazgo del nivel, una viñeta por frase del relato de lo que hizo el estudiante y la habilidad nombrada en un recuadro verde. El relato elige entre variantes fijas según haya habido intentos sin avance y errores corregidos, sumados sobre todas las fases del nivel; sumar no produce ninguna cifra visible, porque el texto sigue siendo una línea fija del contenido (Anexo C, §4). La Tabla 6.3 recoge lo que se nombra al cerrar cada nivel.

**Tabla 6.3.** Habilidad nombrada al cerrar cada nivel.

| Nivel | Escena de cierre | Lo que nombra Algoritm | Frase del resumen |
|---|---|---|---|
| 1 | 1.3, el nacimiento del fuego | «se llama iterar. Probar, mirar el resultado y ajustar» | «Eso se llama probar y ajustar…» |
| 2 | 2.5, cierre del nivel 2 | «Eso se llama abstraer»; «eso es pensar como un algoritmo» | «Eso se llama abstraer y pensar como un algoritmo…» |
| 3 | 3.3, el cruce, y escena final | «dividieron el problema en partes pequeñas»; «Eso es pensar computacionalmente» | «Eso se llama descomponer y depurar…» |

La escena final generaliza los tres niveles en voz de Algoritm: «Tres problemas. Tres formas distintas de pensar.». Tres pruebas, una por nivel, exigen que el cierre nombre la habilidad; la del Nivel 3, por ejemplo, busca «descompon» y «depurar» en el resumen y «partes pequeñas» en el cruce. Otra exige que ningún resumen contenga un dígito, y el barrido del Slice 4 sobre los tres resúmenes reales exige además cero juicios de valor y ninguna oración de más de veinte palabras (Anexo G, §4).

El cierre reflexivo no puede omitirse la primera vez (`LevelSummary_CP07_ElCierreReflexivoNoEsOmitibleLaPrimeraVez`). Como la lista cerrada de datos persistidos (RNF-09) no incluye las escenas vistas, para un cierre «ya visto» significa que el nivel siguiente está desbloqueado; por eso el flujo muestra primero el cierre reflexivo y después desbloquea (Anexo B, Fase 2, §3 y §B.1). El Nivel 3 no tiene nivel siguiente, así que el cruce y la escena final se leen enteros siempre, también al repetirlo; el equipo lo aceptó el 23/09/2026 como INC-51 (Anexo D, §A.2).

## 6.4 Progreso, indicadores de desempeño e informe docente

**Progreso por perfil.** El estudiante crea un perfil con un nombre o alias y ningún otro dato (RF-02); el avance se guarda al confirmar cada fase (RF-04), y un nivel se desbloquea solo cuando todas las fases del anterior están confirmadas (RF-03). Hay siete fases persistidas: una en el Nivel 1, tres en el Nivel 2 (bosque, taller y laberinto) y tres en el Nivel 3 (base, amarre, y mástil y vela). La recolección del Nivel 3 no se persiste: al retomar se da por hecha. Tras un cierre inesperado el juego retoma en la primera fase pendiente (RNF-14). La forma y la ubicación portable del archivo de perfil se describen en §3.5.

**Indicadores de desempeño.** RF-45 obliga a registrar, por perfil y por fase, los cuatro indicadores de OE1 §3.6.1: intentos, errores corregidos, pasos utilizados y tiempo de resolución. La lista es cerrada, y una prueba fija por reflexión que la estructura que los transporta tiene exactamente esos cuatro campos. Cada nivel tiene un recolector propio en C# plano, probado sin escena con un reloj simulado; el tiempo excluye la pausa, y reiniciar una fase no borra lo registrado porque el perfil no sobrescribe una fase confirmada (OE1 §3.6.1, nota 4). La Tabla 6.4 recoge la definición tal como quedó implementada.

**Tabla 6.4.** Definición implementada de los cuatro indicadores por nivel.

| Indicador | Nivel 1 | Nivel 2 | Nivel 3 |
|---|---|---|---|
| Intentos | Golpes no efectivos | F1: objetos no válidos. F2: pasos fuera de orden. F3: ejecuciones que no llegan | Fases rechazadas y pruebas fallidas |
| Errores corregidos | Golpe efectivo justo tras uno fallido | F1 y F2: rechazo seguido del acierto. F3: ediciones entre ejecuciones | Pieza devuelta que se recoloca bien |
| Pasos utilizados | Golpes efectivos al habilitar «Soplar» | F1: sin definición en OE1, se registra 0. F2: acciones en orden. F3: bloques de la secuencia que llega | Fases aceptadas, máximo tres |
| Tiempo de resolución | Sin la pausa | Por fase, sin la pausa | Por fase, sin la pausa ni la escena 3.2; el de la base, desde que se abre el ensamblaje |

Tres puntos merecen aclaración. En el Nivel 1, OE1 define los errores corregidos como cambios del deslizante tras un golpe no efectivo que terminan en uno efectivo; la implementación cuenta el golpe efectivo que sigue de inmediato a uno fallido, y ambas lecturas coinciden porque el resultado depende solo de la fuerza y la cercanía elegidas: acertar justo después de fallar implica haber cambiado la hipótesis. En el bosque, OE1 §3.6.1 no define los pasos utilizados, y el prototipo registra cero en lugar de inventar una definición desde el código (Anexo C, §4). Y el Nivel 3 usa un solo recolector para sus tres fases, porque OE1 §3.6.1 define sus pasos utilizados sobre el nivel entero —confirmaciones aceptadas, con un máximo de tres— y solo el tiempo es por fase (Anexo D, §3).

**Informe docente (RF-46).** El Slice 4 lo implementó el 23 y el 24/09/2026 (commit `9c34924`, fusionado con el PR #85). Se accede desde el botón «Progreso del equipo» del menú principal. Lista los perfiles registrados en el computador —leídos de las dos rutas de almacenamiento, sin duplicados y tolerando un archivo dañado— y, al elegir uno, presenta una tabla con los cuatro indicadores por nivel y por fase, con «Sin datos» para un nivel no jugado en lugar de ceros y el tiempo en minutos y segundos. Consultarlo no altera ningún perfil, y ninguna ruta del estudiante alcanza esa pantalla: es el único lugar del juego donde las cifras existen. El módulo del informe depende solo del núcleo, de modo que retirar un nivel no puede romperlo (RNF-16; véase §3.2). Los cuatro iconos de los indicadores son todavía un boceto provisional, y la protección del acceso al informe, que ningún requerimiento pide, quedó como pregunta abierta (Anexo G, §8).

**Eliminación de datos (RF-47).** El derecho de supresión tiene dos accesos y un solo borrado: el estudiante puede borrar su perfil desde el panel de perfiles del menú principal, y el docente puede eliminar los datos de un estudiante desde el informe. En ambos casos un diálogo pide confirmación explícita y advierte que el borrado no se puede deshacer. Los dos invocan la misma operación del núcleo, que elimina el perfil en las dos rutas de almacenamiento; el Slice 4 no escribió código de borrado nuevo porque esa operación ya existía, probada, desde el Slice 1 (Anexo G, §3 y §A.2). Las pruebas cubren las dos rutas, la preservación de los demás perfiles y el borrado parcial reportado como fallo (véanse §3.5 y §9.3).

Con RF-46 y RF-47 quedan implementados los 47 requerimientos funcionales del proyecto (Anexo G, §1). Lo que queda en este frente es de cierre y no de desarrollo, y se recoge en el capítulo 10.
