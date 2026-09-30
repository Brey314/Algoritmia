# 4. NIVELES Y DESAFÍOS PROGRESIVOS

El prototipo entregado se compone de tres niveles jugables encadenados por una misma historia: una familia prehistórica que avanza hacia la civilización resolviendo tres problemas de supervivencia —la oscuridad, el transporte y el río—, cada uno cerrado por un descubrimiento: el fuego, la rueda y la balsa (Solución OE2, §1.1 y §1.3). La estructura responde a dos criterios pedagógicos radicados en el primer objetivo específico. El criterio CP-01 exige que cada nivel desarrolle de manera explícita una o más facetas del pensamiento computacional y que ninguna mecánica se incorpore si no puede asociarse a una faceta identificable. El criterio CP-04 exige que los niveles se ordenen en dificultad ascendente —básico, intermedio y avanzado— buscando el equilibrio entre desafío y destreza que describe la Teoría del Flujo. La tabla de estructura de niveles del guion (Solución OE2, §1.1.2) asignó a cada nivel su personaje jugable y su faceta principal: el Nivel 1, «La Oscuridad», a Papá y a la iteración y la depuración; el Nivel 2, «La Rueda», a la Niña y a la abstracción y el pensamiento algorítmico; y el Nivel 3, «El Río», a Mamá y a la descomposición y la depuración.

En el prototipo esa estructura se materializó en siete fases jugables distribuidas como una, tres y tres por nivel, que se juegan en cinco escenas: `Level1_Cave` para el Nivel 1; `Level2_Forest`, `Level2_Workshop` y `Level2_Maze` para las tres fases del Nivel 2; y `Level3_River`, donde transcurren la recolección y las tres fases de ensamblaje del Nivel 3. Entre ellas se intercalan dieciocho secuencias narrativas —cuatro del Nivel 1, siete del Nivel 2 y siete del Nivel 3— que presentan cada reto, lo conectan con el anterior y lo cierran nombrando la habilidad practicada. Este capítulo describe primero cómo progresa el pensamiento computacional de un nivel a otro (§4.1) y después cada nivel tal como se juega a la fecha de este documento: el Nivel 1 (§4.2), el Nivel 2 (§4.3) y el Nivel 3 (§4.4), seguidos de las escenas narrativas (§4.5) y de una síntesis de la progresión de dificultad (§4.6).

## 4.1 Progresión del pensamiento computacional entre niveles

La progresión no se construyó sumando obstáculos a una misma mecánica, sino cambiando la naturaleza del problema lógico en cada nivel. En el Nivel 1 la solución es una combinación de valores que se descubre probando; en el Nivel 2 la solución deja de ser un valor y pasa a ser, sucesivamente, una propiedad común, un orden de montaje y un programa; en el Nivel 3 la solución es un artefacto compuesto que se construye por partes y se corrige localizando la parte que falló. La Tabla 4.1 resume esa progresión a partir de la matriz de facetas de la Solución OE1 (§5.1) y del guion (Solución OE2, §1.4, §1.6 y §1.8); la comparación de la dificultad dimensión por dimensión se presenta en la Tabla 4.12 (véase §4.6).

**Tabla 4.1.** Progresión de facetas y problemas lógicos entre los tres niveles.

| Nivel | Facetas | Problema lógico |
|---|---|---|
| 1 · La Oscuridad (básico) | Iteración; depuración | Hallar la fuerza y la cercanía que hacen prender la chispa |
| 2 · La Rueda (intermedio) | Abstracción; algoritmos; otras tres secundarias | Reconocer lo que rueda, armar en orden y programar la ruta |
| 3 · El Río (avanzado) | Descomposición; depuración e iteración | Reunir con una lista de tareas, ensamblar por fases y hallar la parte que falló |

Tres reglas acompañan la progresión y se mantienen iguales en los tres niveles, porque son las que hacen que la dificultad creciente no se convierta en frustración. Ningún nivel tiene pantalla de derrota, límite de intentos ni penalización, de modo que el error funciona como información (CP-02). Lo que el jugador ve nunca incluye puntajes ni cifras de desempeño (CP-03). Y el guía, Algoritm, pregunta y descompone, pero no resuelve (CP-06). Estas reglas se desarrollan en el capítulo 6 (véase §6.1 y §6.2). La progresión se refuerza además en la navegación: el menú de niveles habilita cada nivel solo cuando el perfil activo ha completado el anterior (RF-03), y el bloqueo se señala con icono y texto además del color (RNF-19). Por último, el guía cambia de forma con el problema: llama en el Nivel 1, rueda en el Nivel 2 y gota en el Nivel 3 (INC-45); en el prototipo las formas de rueda y de gota son provisionales (INC-52).

## 4.2 Nivel 1 — La Oscuridad

El Nivel 1 es el nivel básico del prototipo y el primero que encuentra el estudiante. Su faceta principal es la iteración y su faceta secundaria la depuración (Solución OE2, §1.4), y la idea que busca dejar se formula en el guion con estas palabras: «una hipótesis se prueba, se observa el resultado y se ajusta. El fracaso no es el final del proceso: es la información que hacía falta». Esta sección describe el nivel tal como está implementado a la fecha, que difiere en su mecánica del guion radicado por la decisión registrada como INC-47; el detalle técnico, archivo por archivo, se encuentra en el Anexo B, Fases 5 y 6, para la mecánica y en los Anexos E y F para el sonido, la disposición de las piezas, la llama, el quemado y los personajes.

### 4.2.1 Marco narrativo

El nivel se presenta como una secuencia continua de escenas narrativas y una única fase jugable, enlazadas entre sí sin intervención del jugador más allá de avanzar el diálogo. La Tabla 4.2 resume esa secuencia.

**Tabla 4.2.** Secuencia de escenas del Nivel 1 en el prototipo.

| Momento | Tipo | Qué ocurre |
|---|---|---|
| Apertura | Narrativa | La familia huye de noche hacia una cueva y queda a oscuras |
| Aparición del guía | Narrativa | Algoritm aparece como una luz, señala las piedras y se apaga |
| El hallazgo | Narrativa | La Niña arranca un destello a las piedras y Papá asume el reto |
| Encendido del fuego | Escena jugable | El jugador, como Papá, reúne los materiales, golpea y sopla |
| Nacimiento del fuego | Narrativa | La llama crece y Algoritm nombra la habilidad practicada |
| Resumen de fin de nivel | Pantalla | Relato sin cifras; se desbloquea el Nivel 2 |

El asset de cada narrativa, su número de líneas y su destino al terminar figuran en la Tabla 4.11 (véase §4.5). La escena del hallazgo termina con la intervención del guía que plantea el problema sin resolverlo: «Tienes las piedras y tienes las hojas. Sabes que algo puede pasar entre las tres cosas. Lo que no sabes todavía es cómo. ¿Desde dónde vas a intentarlo?» (sobre esta última pregunta, véase §4.2.7). Las narrativas del nivel se ven a través de una capa de oscuridad cuya luz cambia con cada encuadre de cámara, y esa misma capa se reutiliza en la fase jugable para representar el avance (véase §4.2.4).

### 4.2.2 El problema lógico

El problema que plantea el nivel consiste en encender fuego con dos piedras y un montón de hojas secas sin saber de antemano cómo. En el prototipo el resultado de un golpe depende de dos variables que el jugador controla: la fuerza del golpe, en un deslizante graduado de 0 a 10, y la cercanía entre el sílex y el pedernal, en un segundo deslizante también graduado de 0 a 10. Solo una región estrecha de esa combinación produce una chispa que cae en las hojas: la fuerza debe estar en la franja efectiva de 7 a 8 y las piedras deben rozarse en la medida justa, lo que con los tamaños actuales de las piezas solo ocurre en la posición 5 de cercanía, la única cuyo encimado cae dentro del roce efectivo. La franja de fuerza, el número de posiciones de cada deslizante y el roce efectivo con su margen no están escritos en el código sino en el contenido del nivel (`N1_Config`), conforme a CT-05 y RNF-18; la posición certera resulta de esos valores y del tamaño de las piedras y las hojas en la escena.

El nivel no espera que el estudiante encuentre esa región por azar, sino que la descubra por un ciclo de hipótesis, experimento y observación. La hipótesis es la posición de los dos deslizantes: moverlos no produce ningún efecto sobre el estado del reto (RF-15). El experimento es el botón «Golpear», que ejecuta la hipótesis vigente. La observación es el mensaje que describe lo ocurrido, sin calificarlo. Ese ciclo es la iteración que el nivel ejercita.

La depuración aparece en la forma en que el sistema evalúa cada golpe. El prototipo resuelve primero la cercanía y después la fuerza: si las piedras quedan antes del roce efectivo o demasiado encimadas, el golpe no prende con ninguna fuerza, y el mensaje habla de las piedras, no de la fuerza. Solo con el roce justo el mensaje pasa a describir el efecto de la fuerza. Además, cada mensaje indica la dirección del desajuste: separadas o encimadas, demasiado suave o demasiado fuerte. Con ello un problema de dos variables se convierte, para quien lee con atención, en dos problemas de una variable que se corrigen uno después del otro: primero lograr que las piedras choquen y después ajustar la fuerza. La Tabla 4.3 muestra la respuesta del sistema en cada caso.

**Tabla 4.3.** Resolución de un golpe según la cercanía y la fuerza.

| Piedras | Fuerza | Primer mensaje de la tablilla | Efecto |
|---|---|---|---|
| Antes del roce | Cualquiera | «El sílex y el pedernal están separados…» | No efectivo |
| Encimadas de más | Cualquiera | «Las piedras están tan encimadas que no chocan…» | No efectivo |
| Roce justo | Suave | «Las piedras apenas se rozan. No sale ninguna chispa.» | No efectivo |
| Roce justo | Fuerte | «Las piedras chocan con fuerza y las chispas saltan…» | No efectivo |
| Roce justo | Efectiva | «Una chispa cae dentro de las hojas…» | Suma uno; la luz sube |

Ningún intento no efectivo resta lo ganado antes.

Cada caso no efectivo tiene una segunda formulación que aparece a partir del segundo fallo seguido —por ejemplo, «Otra vez las piedras no llegan a tocarse. Sin choque no hay chispa.»—, y el registro evita repetir el mismo mensaje dos veces seguidas cuando existe alternativa (RF-18). Los doce mensajes del nivel están en el contenido (`N1_Mensajes`) y ninguno contiene cifras (RF-17).

### 4.2.3 Qué hace el jugador, paso a paso

La fase jugable se desarrolla en tres momentos, cada uno asociado a un paso del guía: «Reunir», «Golpear» y «Soplar».

**Reunir.** Al abrir la escena, la cueva vista desde arriba muestra a Papá junto al charco de luz del centro, a la familia apartada a un lado y siete piezas regadas por el suelo: cinco montoncitos de hojas, el sílex y el pedernal. Las piezas aparecen en posiciones y giros al azar, sin pisar la interfaz ni el centro de la pantalla, de modo que cada partida empieza distinta. La tablilla superior muestra la instrucción del guía: «Reúne todas las hojas y las dos piedras en el centro de la pantalla.» El jugador arrastra cada pieza con clic sostenido y la suelta donde quiera. Mientras quede alguna fuera del círculo de reunión no ocurre nada, sin reproche ni penalización (CP-02); si el jugador pide ayuda, el guía repite la instrucción y el círculo se dibuja en el suelo. Cuando la última pieza queda dentro, las piezas dejan de poder arrastrarse y la cámara se acerca al doble en ocho décimas de segundo, mientras las hojas convergen en un montón sobre el punto del fuego y las piedras se colocan a cada lado según el deslizante de cercanía. La Figura 4.1 muestra el inicio de este momento.

![Figura 4.1. Inicio de la fase jugable del Nivel 1: las hojas, el sílex y el pedernal están regados por la cueva; la tablilla superior formula la tarea de reunirlos y el botón de ayuda, arriba a la izquierda, lleva la forma de fuego del guía.](fig/fig-04a-1-reunir.jpg){width=16cm}

**Golpear.** Tras el acercamiento aparece la interfaz de encendido y el guía cambia de paso: «Elige la fuerza y qué tan cerca van las piedras, y golpea para hacer chispas.» El deslizante de fuerza es vertical, a la derecha, con los rótulos «Fuerte», «Fuerza del golpe» y «Suave»; su asa crece y pasa del azul al rojo a medida que aumenta la fuerza, de modo que el valor se lee por tamaño además de por color (RNF-19). El deslizante de cercanía es horizontal, con los rótulos «Lejos» y «Cerca», y desplaza las piedras sobre el montón en el momento en que se mueve, de modo que el jugador ve su hipótesis antes de ponerla a prueba. Ninguno de los dos ejecuta un golpe (RF-15). Al accionar «Golpear», Papá golpea las piedras y el resultado se escribe en la tablilla según la Tabla 4.3. El choque solo se oye cuando las piedras se tocan y la chispa suena únicamente en el golpe efectivo; no existe un sonido de error, y tras un golpe sin chispa Papá hace un gesto de ánimo en lugar de mostrar abatimiento (véase §7.2 y §7.3). La Figura 4.2 muestra la interfaz de este momento.

![Figura 4.2. Interfaz de encendido del Nivel 1: los deslizantes de fuerza (vertical) y de cercanía (horizontal) representan la hipótesis; «Golpear» la pone a prueba; «Soplar» permanece atenuado y con candado hasta la convergencia, un doble indicador que no depende solo del color.](fig/fig-04a-2-encendido.jpg){width=16cm}

**Soplar.** El botón «Soplar» permanece atenuado y con un candado hasta que el jugador acumula tres golpes efectivos, el mínimo configurado en el nivel (RF-19). Accionarlo antes no produce ningún mensaje de error. Al tercer golpe efectivo la tablilla describe que «un hilo de humo sube despacio, como si la cueva estuviera suspirando», el botón se habilita y el guía pasa al paso «Soplar», cuya instrucción —«Sopla despacio sobre el montón de hojas para avivar el humo.»— repite el botón de ayuda. Una vez habilitado, el botón no vuelve a bloquearse aunque el jugador siga golpeando y falle (INC-32). El guion presenta este botón como el momento de convergencia: se habilita «solo cuando existen condiciones que avivar» (Solución OE2, §1.4.3.1).

### 4.2.4 Retroalimentación e iluminación sin derrota

La retroalimentación del nivel descansa en tres canales que se complementan. El primero es la tablilla superior, que muestra el último mensaje en lenguaje narrativo y descriptivo, sin cifras ni juicios de valor (RF-17). El segundo es la escena misma: las piedras que se mueven con el deslizante, el sonido del choque y el de la chispa, y el gesto de Papá. El tercero es la luz de la cueva (RF-21). La escena arranca en penumbra, con un único charco de luz cálida sobre el punto del fuego; cada golpe efectivo aclara el fondo un escalón discreto, de un cuarto del recorrido entre el valor mínimo y el máximo configurados, de modo que con los tres golpes efectivos mínimos la cueva queda en tres cuartos de ese recorrido; el último tramo lo da el nacimiento del fuego, salvo que el jugador siga golpeando con acierto. Los golpes efectivos acumulados, y con ellos la luz ganada, nunca se reducen por un fallo posterior, conforme a la regla del guion según la cual «lo ganado permanece» (Solución OE2, §1.4.3.6; CP-02). Como recuerda el acta D04 (Anexo H), «la iluminación nunca es el único canal por el que el juego informa del avance: acompaña a la retroalimentación, no la sustituye». Cada golpe efectivo cambia la luz en un solo paso, mientras que el acercamiento de la cámara y el encendido final son barridos continuos; ninguno produce parpadeos ni destellos de alta frecuencia (RNF-21).

Ningún resultado del nivel termina la partida. Los intentos son ilimitados, no hay contador de derrota y el único efecto de un fallo sobre el reto es sumar a la racha de fallos consecutivos, que activa la segunda formulación del mensaje y la pista del guía (RF-18). Mientras tanto, cada golpe alimenta en segundo plano los indicadores de desempeño del perfil —intentos, errores corregidos, pasos utilizados y tiempo de resolución—, que no se muestran al estudiante y solo aparecen en el informe del docente (RF-45, RF-46; véase §6.4).

### 4.2.5 La ayuda del guía

El botón de ayuda está visible durante toda la escena, arriba a la izquierda, como un círculo con la figura de fuego de Algoritm. Pulsarlo repite la instrucción del paso activo sin alterar el estado de la partida ni contar como intento (RF-13), y mientras se reúnen las piezas dibuja además el círculo donde deben quedar. El paso activo avanza de «Reunir» a «Golpear» al terminar el acercamiento, y de «Golpear» a «Soplar» al alcanzar la convergencia. Si el jugador falla tres golpes seguidos, la tablilla muestra por sí sola, en lugar del mensaje del golpe, la pista del paso, que orienta con una pregunta y no nombra los valores correctos (CP-06): «Las chispas dependen de cómo chocan las piedras: de la fuerza y de qué tan juntas están. ¿Qué cambiarías antes del próximo golpe?» Tras ofrecerla, la cuenta de fallos se reinicia, y un golpe efectivo también la reinicia.

### 4.2.6 Resolución y cierre reflexivo

Al accionar «Soplar», la tablilla describe que «el humo se abre» y que «algo naranja tiembla entre las hojas»; Papá sopla y se queda arrodillado; una llama animada prende sobre el montón, las hojas se queman desde el centro hacia afuera durante tres segundos y medio, se suma el sonido de la hoguera y la cueva alcanza su iluminación completa en un solo barrido (RF-20, RF-21). Al terminar, el juego pasa por un fundido a la escena narrativa del nacimiento del fuego.

En esa escena Algoritm aparece hecho de fuego y nombra la habilidad practicada relacionándola con lo que el jugador acaba de hacer (RF-12, CP-07): «Y algo más. La primera vez no funcionó, ni la segunda. Cambiaste la fuerza, volviste a probar y miraste qué pasaba cada vez.» y «Eso tiene nombre: se llama iterar. Probar, mirar el resultado y ajustar.» (Figura 4.3). Le sigue el resumen de fin de nivel, un relato sin cifras de lo que hizo el estudiante —por ejemplo, «Cuando algo no funcionó, cambiaste la fuerza o el sitio y volviste a intentar.»— que vuelve a nombrar la habilidad, esta vez como «probar y ajustar» (CP-03, RF-45). Al mostrarse ese resumen se confirma la fase, se desbloquea el Nivel 2 y se guarda el progreso (RF-03, RF-04). El desbloqueo va después del cierre porque, para una escena de cierre, «ya vista» significa que el nivel siguiente está desbloqueado; así, la primera vez el cierre no puede omitirse (véase §6.3).

![Figura 4.3. Cierre reflexivo del Nivel 1: Algoritm, en su forma de fuego, nombra la iteración ante la familia reunida junto a la hoguera recién encendida («Eso tiene nombre: se llama iterar. Probar, mirar el resultado y ajustar.»).](fig/fig-04a-3-cierre-iterar.jpg){width=16cm}

### 4.2.7 Correspondencia con los requerimientos RF-13 a RF-21

El Módulo C de la Solución OE1 (§3.3) agrupa los requerimientos propios del Nivel 1 (RF-14 a RF-21), y el RF-13 del andamiaje se aplica a él como a toda escena jugable. La Tabla 4.4 relaciona cada requerimiento, identificado por su nombre radicado, con su implementación actual y con el número de métodos de prueba de los assemblies de prueba del nivel —EditMode y PlayMode— que lo nombran. Esos métodos suman cuarenta; la prueba de RF-21 comprueba con aserciones los tres escalones, que la convergencia no alcanza todavía la luz máxima y que esta llega tras soplar, y deja además una captura de la iluminación completa para revisión visual. La trazabilidad completa se presenta en el capítulo 9 y en el Anexo A, que suma además las pruebas de otros módulos que nombran estos requerimientos; por eso sus cifras de RF-13, RF-17 y RF-20 son mayores que las de la Tabla 4.4.

**Tabla 4.4.** Requerimientos del Nivel 1 frente a su implementación.

| RF | Nombre en OE1 | Cómo lo cumple el prototipo | Pruebas |
|---|---|---|---|
| RF-13 | Pista progresiva | Repite la instrucción del paso; pista al tercer fallo seguido | 3 |
| RF-14 | Panel de encendido | Reunir en un círculo; dos deslizantes, «Golpear», «Soplar» y tablilla | 10 |
| RF-15 | Control de posición como hipótesis | Dos deslizantes que no ejecutan nada hasta golpear | 6 |
| RF-16 | Acción de golpear | Juzga cercanía y luego fuerza; solo la combinación efectiva cuenta | 9 |
| RF-17 | Registro de retroalimentación cualitativa | Doce mensajes descriptivos sin cifras; la tablilla muestra el último | 2 |
| RF-18 | Intentos ilimitados | Sin tope ni derrota; segunda formulación desde el segundo fallo | 3 |
| RF-19 | Desbloqueo condicional de la acción de soplar | Candado hasta tres golpes efectivos; no se vuelve a bloquear | 3 |
| RF-20 | Resolución del nivel | Llama animada, quemado de las hojas y paso a la narrativa de cierre | 3 |
| RF-21 | Iluminación progresiva del escenario | Un escalón de luz por golpe efectivo e iluminación completa al soplar | 1 |

RF-15 y RF-16 se cumplen con una mecánica distinta de la radicada: esos requerimientos, el guion (Solución OE2, §1.4.3 y §1.4.4) y la historia de usuario HU-06 describen un único deslizante de posición con tres distancias respecto de las hojas —«Lejos», «Cerca» y «Muy cerca»— y un registro con historial de intentos. La diferencia está registrada en la inconsistencia INC-47, abierta, y el acta D06 (15/09/2026) recoge su ampliación a los dos momentos actuales, reunir y encender; la cronología de la decisión, la razón por la que las pruebas conservan los identificadores RF-15 y RF-16 y la corrección documental pendiente se exponen en §5.3. Como observación ajena a INC-47, cabe señalar que el texto radicado de RF-14 también describe un panel con «un control deslizante de posición» (Solución OE1, §3.3).

Quedan abiertos dos aspectos menores ya registrados: la revisión con el usuario del radio del círculo, del tamaño de la fogata y de la posición certera de cercanía (Anexo B, Fases 5 y 6, §6), y el pulso del botón de ayuda, que hoy es permanente desde que se abre la escena y no solo tras tres fallos (véase §10.4). Del código y del contenido actuales se desprenden, por último, dos observaciones que no figuran en INC-47 ni en el Anexo B, Fases 5 y 6. Con los tamaños actuales de las piezas, en las posiciones 2 a 4 de cercanía las piedras ya se tocan y el choque se oye, pero el golpe se clasifica como de piedras separadas y la tablilla lo describe así. Y la última pregunta del guía en la escena del hallazgo, «¿Desde dónde vas a intentarlo?», conserva la redacción del guion (Solución OE2, §1.4.2), cuya mecánica era de posición.
