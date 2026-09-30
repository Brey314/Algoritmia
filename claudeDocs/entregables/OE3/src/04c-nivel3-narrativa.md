## 4.4 Nivel 3 — El Río

El tercer nivel cierra el juego. La familia llega con la carretilla al borde del bosque y encuentra el camino cortado por un río ancho; la herramienta que resolvió el problema anterior deja de servir: «La rueda los trajo hasta acá. Pero este problema es diferente» (escena puente II). El personaje jugable es Mamá, quien en el guion «plantea dividir el problema» (Solución OE2, §1.1.1). La faceta principal es la descomposición, y las secundarias, la depuración y la iteración (Solución OE2, §1.8).

El problema lógico tiene dos partes. La primera es de descomposición: el objetivo «cruzar el río» se convierte en una lista de cuatro tareas, y la balsa, en tres subestructuras dependientes —la base sostiene, el amarre la mantiene unida, el mástil con la vela la impulsa—, de modo que ninguna se resuelve sin haber resuelto la anterior. La segunda es de depuración: cuando la balsa terminada no pasa la prueba, el sistema señala el espacio defectuoso y el estudiante corrige solo esa parte, conservando lo que ya estaba bien.

El nivel se construyó en el Slice 3 (16 al 21/09/2026) y recibió después los personajes animados y el sonido (véase Anexo D, §4 y §A.3). Implementa RF-35 a RF-44, todos de prioridad alta: RF-35 a RF-43 en una sola escena jugable, `Level3_River`, que reúne los tres momentos de la Tabla 4.10, y RF-44 —el cruce automático y la escena de cierre del juego— en la escena narrativa (véase §4.5).

**Tabla 4.10.** Momentos del Nivel 3, faceta que ejercitan y requerimientos que implementan.

| Momento | Qué hace el estudiante | Faceta | Requerimientos |
|---|---|---|---|
| Recolección | Recorre la orilla, recoge ocho materiales y sigue la lista de tareas | Descomposición | RF-35 a RF-39 |
| Ensamblaje por fases | Coloca la base, el amarre y el mástil con la vela, una fase a la vez | Descomposición, iteración | RF-40, RF-41 |
| Prueba y depuración | Prueba la balsa, localiza el espacio señalado y corrige solo lo mal puesto | Depuración | RF-42, RF-43 |

### 4.4.1 Recolección

En la escena 3.1, Algoritm, en su forma de gota, descompone el objetivo mediante preguntas —«¿cuántas cosas necesitamos para cruzar?», «¿Qué necesita la balsa?»— hasta concluir «¡Ya tenemos nuestra lista!» (RF-10). Al abrirse la escena jugable, esa lista aparece arriba a la izquierda y permanece visible durante todo el nivel (Figura 4.8).

![Figura 4.8. Recolección al abrir el Nivel 3: lista de cuatro tareas sin marcar, instrucción del guía, inventario de cuatro casillas vacías (la de los troncos con cinco marcas apagadas), materiales repartidos por la orilla, zona de construcción junto a la familia, las cuatro flechas y el río asomando por la derecha.](fig/fig-04c-1-orilla.jpg){width=16cm}

**Movimiento.** Mamá se desplaza en dos dimensiones con cuatro flechas en pantalla: camina mientras se mantiene pulsada una flecha y se detiene al soltarla (RF-35), dentro de los límites de la orilla. La entrada se limita a clic y clic sostenido, sin teclado (RNF-02, CT-06). Desde el 25/09/2026 el plano se abrió para que el río asome por el borde derecho (véase Anexo D, §3, decisión 4).

**Materiales e inventario.** El guion dispone cuatro objetos —troncos, sogas, tela y mástil— ubicados de modo que obliguen a recorrer el escenario (Solución OE2, §1.8.2). En el acta D07 (Anexo H) se acordó que los troncos fueran cinco, encontrados uno a uno por la orilla, de modo que hay ocho materiales: cinco troncos, un rollo de sogas, una tela y un mástil. Al acercarse a uno aparece el botón «Recoger» (RF-37), y al pulsarlo el material pasa del suelo al inventario; recoger no puede fallar (CP-02). El inventario (RF-38) conserva los cuatro objetos del guion como sus cuatro casillas, una por clase de material; los cinco troncos comparten la suya, que enciende una marca por tronco (acta D07), de modo que el avance se ve sin una cifra (CP-03).

**Lista de tareas.** La lista (RF-36) es la representación visible de la descomposición: «Recoger troncos», «Encontrar sogas», «Ensamblar la balsa» y «Colocar el mástil y la vela». Según el guion, las tareas de recolección se marcan al recoger su material, y la tela y el mástil no marcan tarea, porque los consume la cuarta (Solución OE2, §1.8.2); como los troncos son cinco, la primera se marca con el quinto y no con el primero, porque la tarea es la clase completa (decisión de diseño del 20/09/2026). Según INC-30, la tercera se marca al confirmar el amarre y la cuarta al confirmar el mástil y la vela, y confirmar la base no marca ninguna. Una tarea marcada no se desmarca, y cada estado cambia de forma además de color (RNF-19). La forma gráfica definitiva de la lista sigue abierta en INC-46.

**Zona de construcción.** Junto al río, un círculo señala dónde se arma la balsa (RF-39). Si Mamá entra sin todos los materiales, el mensaje nombra lo que falta por su nombre, nunca por su número, con icono de alerta además del color; la familia anima y no hay penalización. Con todo, el mensaje confirma «Tienes todo lo de la lista. Aquí se arma la balsa.» y la cámara se acerca al agua. La recolección no se guarda como fase propia: un cierre forzado tras confirmar la base o el amarre retoma en la fase pendiente y la da por hecha (RNF-14; véase Anexo D, §3, decisión 1).

### 4.4.2 Ensamblaje por fases

El ensamblaje ocurre sobre el propio río, sin ventana superpuesta (acta D07). La balsa se compone de diecisiete espacios —cinco troncos para la base, diez amarres (dos por tronco, del único rollo de sogas), el mástil y la vela—, cada uno con la silueta de lo que espera hasta que se llena.

**Fases bloqueantes.** Las fases se habilitan en orden: base, amarre, y mástil con vela (RF-40). Solo se ven los espacios de la fase activa y los de las ya confirmadas. Es, según el guion, «lo que convierte el ensamblaje en un ejercicio de descomposición y no en un rompecabezas de ensayo y error» (Solución OE2, §1.8.3): el estudiante trabaja sobre un subproblema a la vez.

**Colocar no es validar.** Las piezas se arrastran desde su casilla con clic sostenido; cualquier pieza cabe en cualquier espacio abierto y vacío, y el veredicto lo da el botón: «Listo» en la base y el amarre, «Probar balsa» en la última fase. El estudiante completa así cada fase según su propio plan antes de conocer el resultado. El diseño incluye una trampa deliberada (acta D07): el mástil es el tronco más largo y cabe en la base; el acta pide que se distinga a simple vista por su longitud y no por el color. En el prototipo, ese error se detecta al pulsar «Listo» en la base, antes de la prueba de la balsa (Figura 4.9).

![Figura 4.9. Fase de base rechazada: el mástil se había colocado en uno de los espacios de los troncos. Al pulsar «Listo», ese espacio queda señalado con color e icono de alerta, el mástil vuelve a su casilla, los cuatro troncos bien puestos permanecen y el mensaje pregunta «¿esa pieza va ahí?».](fig/fig-04c-2-base-incorrecta.jpg){width=16cm}

**Rechazo y aprobación.** Si «Listo» no valida la fase, los espacios incorrectos quedan señalados con color e icono y solo las piezas mal puestas vuelven al inventario; las correctas se quedan, la fase sigue abierta y la familia anima, sin ningún gesto de derrota. Si la fase es correcta, un pulso de completado acompaña tres martillazos, la familia celebra y el mensaje anuncia el subproblema siguiente —«La base quedó firme. Ahora hay que sujetarla para que no se abra.»—. La fase queda consolidada: sus piezas ya no se retiran y ningún fallo posterior las toca (RF-41). Las fases 1 y 2 se guardan en el perfil al aprobarse, y la 3 al salir al cruce (véase Anexo D, §A.4).

**Ayuda.** El botón de ayuda repite la instrucción de la fase activa. Tras tres confirmaciones fallidas seguidas en la misma fase, el mensaje se sustituye por la pista del paso, una pregunta que orienta sin nombrar la pieza (RF-13, CP-06); la de la base es «¿Qué parte de la balsa toca el agua y sostiene todo lo demás?». En el sonido, cada pieza colocada suena como un martillazo —también la mal puesta— y una fase que no pasa no suena (CP-02).

### 4.4.3 Prueba y depuración

En la tercera fase el botón se rotula «Probar balsa» (RF-42). La validación examina cada espacio de la fase abierta —está mal si está vacío o contiene otra pieza— y su resultado es la lista de espacios defectuosos, no un veredicto global.

**Si la prueba falla,** la balsa gira y se hunde por un costado y vuelve a su lugar, con un movimiento continuo sin destellos (RNF-21). El espacio defectuoso queda señalado con color e icono, el mensaje dice qué revisar —«La balsa se volteó. Mira el espacio marcado: ¿qué debería ir ahí?»—, las piezas mal ubicadas regresan al inventario y la base y el amarre permanecen intactos (RF-43, RF-41; Figura 4.10). Al estudiante le corresponde interpretar el hundimiento como información y decidir qué va en el espacio señalado: el sistema dice dónde está el fallo, nunca cuál es la respuesta (CP-06). Los intentos son ilimitados, sin pantalla de derrota (CP-02).

![Figura 4.10. Prueba de la balsa fallida con el mástil y la vela intercambiados: la base y el amarre, ya aprobados, siguen en su sitio; los dos espacios aparecen señalados con color e icono de alerta, las dos piezas volvieron al inventario y el mensaje indica qué revisar. La lista conserva marcadas las tres primeras tareas.](fig/fig-04c-3-prueba-fallida.jpg){width=16cm}

**La escena 3.2.** La primera vez que la prueba falla se reproduce la escena 3.2 (Figura 4.11): la familia cae al agua poco profunda de la orilla y Algoritm nombra la habilidad: «No pasa nada. Esto se llama depurar. No hay que hacer todo otra vez: hay que encontrar la parte que falló. ¿Qué salió mal?». La decide `ConditionalNarrativeTrigger`, un componente del andamiaje y no una rama del controlador: se reproduce una sola vez por pasada por el nivel, y quien acierta al primer intento pasa directamente al cruce. La escena narra sin reiniciar: al volver, la fase 3 está como quedó, y su tiempo no cuenta como tiempo de resolución.

![Figura 4.11. Escena 3.2, que solo aparece tras el primer fallo de la prueba: la balsa hundida por un costado junto a la orilla, la familia en el agua y Algoritm, en forma de gota, nombrando la depuración con una pregunta en lugar de dar la respuesta.](fig/fig-04c-4-escena-3-2.jpg){width=16cm}

**Si la prueba pasa,** el mensaje es «La balsa flota derecha. ¡A cruzar!», se guarda la fase 3 y el juego pasa a la escena 3.3, donde la balsa cruza el río de forma automática (RF-44). Siguen el resumen del nivel, la escena final, los créditos y la pantalla de inicio (INC-39). El nivel entero se recorre de forma automatizada, con un perfil real, acertando al primer intento y fallando para ver la 3.2 y corregir (`RiverLevel_RNF13_RecorreElNivel3CompletoHastaElInicio`; véase §9.2). Los indicadores de desempeño que el nivel registra se describen en §6.4.

## 4.5 Escenas narrativas y puentes

El guion reparte la historia en quince escenas narrativas: la apertura y tres escenas del Nivel 1; el puente I y cinco del Nivel 2; el puente II, tres del Nivel 3 y la escena final (Solución OE2, §1.3 a §1.9). El prototipo las reproduce en una única escena reutilizable, `Narrative`, a partir de dieciocho secuencias de contenido (`NarrativeSequence`). Cada una es un asset que declara su ilustración, las paradas de cámara, las líneas de diálogo, los personajes y objetos con lo que hacen en cada línea, el sonido de ambiente y el destino al terminar. Hay más secuencias que escenas porque cada secuencia lleva una sola ilustración: el puente I ocupa dos y el puente II tres (del refugio con fuego al horizonte con las fogatas, y de ahí al río). La Tabla 4.11 las enumera en orden de juego; las líneas, incluidas las acotaciones sin hablante, se contaron en los assets (139 en total).

**Tabla 4.11.** Secuencias narrativas del prototipo, escena del guion que reproducen y destino al terminar.

| Nivel | Secuencia | Escena del guion | Líneas | Al terminar |
|---|---|---|---|---|
| 1 | `N1_Apertura` | Apertura (§1.3.1) | 7 | Encadena la siguiente |
| 1 | `N1_AparicionGuia` | 1.1 (§1.4.1) | 11 | Encadena la siguiente |
| 1 | `N1_Hallazgo` | 1.2 (§1.4.2) | 18 | Fase jugable: la cueva |
| 1 | `N1_NacimientoDelFuego` | 1.3 (§1.4.4) | 17 | Resumen del nivel |
| 2 | `N2_PuenteI` | Puente I (§1.5) | 3 | Encadena la siguiente |
| 2 | `N2_PuenteI_Bosque` | Puente I (§1.5) | 9 | Encadena la siguiente |
| 2 | `N2_Escena21_Bosque` | 2.1 (§1.6.1.1) | 7 | Fase 1: el bosque |
| 2 | `N2_Escena22_ElPatron` | 2.2 (§1.6.1.3) | 7 | Encadena la siguiente |
| 2 | `N2_Escena23_Construccion` | 2.3 (§1.6.2.1) | 8 | Fase 2: el taller |
| 2 | `N2_Escena24_Regreso` | 2.4 (§1.6.3.1) | 8 | Fase 3: el laberinto |
| 2 | `N2_Escena25_Cierre` | 2.5 (§1.6.4) | 6 | Resumen del nivel |
| 3 | `N3_PuenteII` | Puente II (§1.7) | 2 | Encadena la siguiente |
| 3 | `N3_PuenteII_Horizonte` | Puente II (§1.7) | 5 | Encadena la siguiente |
| 3 | `N3_PuenteII_Rio` | Puente II (§1.7) | 4 | Encadena la siguiente |
| 3 | `N3_Escena31_Llegada` | 3.1 (§1.8.1) | 8 | Recolección en la orilla |
| 3 | `N3_Escena32_PrimerIntento` | 3.2 (§1.8.4.1) | 5 | Vuelve a la fase 3 |
| 3 | `N3_Escena33_Cruce` | 3.3 (§1.8.5) | 7 | Resumen del nivel |
| 3 | `N3_EscenaFinal` | Final (§1.9) | 7 | Créditos |

**El recorrido lo deciden los assets.** Al terminar una secuencia, la escena sale con una prioridad fija: la secuencia encadenada, si el asset la declara; si no, la fase jugable declarada; si es la escena final, los créditos; si es un cierre reflexivo, el resumen del nivel. La rama es una sola para las dieciocho secuencias (RF-05, CT-05): encadenar escenas o añadir una es editar o crear un asset, sin tocar el controlador. Las mecánicas declaran también en su contenido la secuencia que les sigue —por ejemplo, el primer fallo de la balsa lleva a la 3.2 y la prueba superada a la 3.3—, salvo el panel del Nivel 1, que nombra su escena de cierre en el código. Cada puente abre el nivel que empieza: el menú de niveles arranca el Nivel 2 en `N2_PuenteI` y el Nivel 3 en `N3_PuenteII`. En el puente II, la cámara pasa del refugio al horizonte —donde el guía conserva aún la forma de rueda— y el camino se corta ante el río: «¡La carretilla no sirve aquí!».

**Presentación.** Entre una narrativa y una mecánica la pantalla funde a negro y de vuelta (véase §3.3). El diálogo avanza con un clic en «Continuar» y el cuadro muestra el retrato de quien habla. Los personajes, animados por recorte (INC-53), caminan, señalan o celebran según la línea, y aparecen en las dieciocho secuencias (véase Anexo F). Rige la regla del acta D05 —lo que el texto nombra se ve entero y por encima del cuadro de diálogo—, que una prueba comprueba parada por parada en las dieciocho secuencias.

**Omitir.** El botón «Omitir» solo aparece en una escena ya vista (RF-06). Como no hay registro de escenas vistas (la lista de datos persistidos es cerrada, RNF-09), «ya vista» se deriva del progreso: una escena intermedia, si el perfil confirmó la última fase de su nivel; un cierre reflexivo, si el nivel siguiente está desbloqueado. La primera vuelta se lee entera y solo quien repite puede saltar. El Nivel 3 no tiene nivel siguiente, así que su cruce y su escena final no ofrecen omitir nunca; la diferencia con HU-14 FA-01 queda abierta como INC-51 (véase Anexo D, §A.2).

**Cierres reflexivos.** Cuatro secuencias llevan la marca de cierre reflexivo —`N1_NacimientoDelFuego`, `N2_Escena25_Cierre`, `N3_Escena33_Cruce` y `N3_EscenaFinal`—, y en ellas el guía nombra la habilidad practicada y la relaciona con lo que el estudiante acaba de hacer (RF-12, CP-07). En el cruce: «Primero dividieron el problema en partes pequeñas. Y cuando algo falló no se rindieron: lo encontraron y lo arreglaron.». El resumen del nivel, sin cifras, la vuelve a nombrar como «descomponer y depurar» (véase §6.3). La escena final generaliza sobre los tres niveles (Solución OE2, §1.9 y §1.11): «En la cueva probaste una y otra vez hasta que funcionó. En el bosque miraste muchas cosas y encontraste lo que tenían en común. En el río partiste un problema enorme en pedazos pequeños y, cuando algo se rompió, buscaste exactamente dónde.».

## 4.6 Síntesis de la progresión de dificultad

La Tabla 4.12 compara los tres niveles tal como están implementados, con datos de los Anexos B (Fases 5 y 6), C y D y del contenido de cada nivel.

**Tabla 4.12.** Comparación de los tres niveles según las dimensiones de la dificultad.

| Dimensión | Nivel 1 — La Oscuridad | Nivel 2 — La Rueda | Nivel 3 — El Río |
|---|---|---|---|
| Facetas (Solución OE2, §1.1.2) | Iteración y depuración | Abstracción y pensamiento algorítmico | Descomposición y depuración |
| Fases jugables | Una | Tres: bosque, taller y laberinto | Tres de ensamblaje, tras la recolección |
| Qué manipula | 7 piezas y 2 deslizantes de 0 a 10 | 14 objetos; armado de 6 pasos; bloques sobre una cuadrícula de 16 × 11 | 8 materiales en 4 clases; 17 espacios en 3 fases |
| Abstracción que exige | Relacionar dos variables con el efecto observado | Extraer una propiedad común; expresar una ruta con instrucciones relativas | Ver la balsa como tres subestructuras dependientes |
| Qué debe planear | El golpe siguiente; tres golpes efectivos antes de soplar | El orden del armado y el programa completo antes de ejecutarlo | El recorrido por la orilla y cada fase antes de confirmarla |
| Cómo se localiza el error | El mensaje describe el efecto y la dirección del desajuste | Mensaje por objeto o por paso; en el laberinto, el bloque donde se detuvo | Espacio exacto señalado; solo vuelve lo mal puesto |
| Ayuda del guía | 3 pasos; pista tras 3 fallos; círculo de reunión | 3 pasos, uno por fase; pista tras 3 fallos | 4 pasos; pista tras 3 fallos; escena 3.2 al primer fallo |

La dificultad crece en tres ejes a la vez. Crece la estructura que hay que manejar: una fase con siete piezas y dos variables en el Nivel 1; tres fases con lógicas distintas en el Nivel 2; desplazamiento libre, ocho materiales y diecisiete espacios en tres fases dependientes en el Nivel 3. Crece la abstracción: de ajustar valores a reconocer una propiedad común y representar un recorrido como programa, y de ahí a modelar un artefacto como partes con funciones distintas. Y crece el horizonte de planificación: del golpe siguiente a la secuencia completa del laberinto, y de ahí a la tarea entera organizada en subproblemas.

Al mismo tiempo, la señal del error se vuelve más precisa —el mensaje que describe el efecto en la cueva, el bloque resaltado en el laberinto, el espacio exacto en el río— sin que el sistema decida nunca qué corregir (CP-06). Lo que no cambia de un nivel a otro es lo que evita que la dificultad se convierta en frustración: intentos ilimitados sin pantalla de derrota (CP-02), ninguna cifra a la vista del estudiante (CP-03), lo aprobado nunca se pierde (RF-41, RF-43) y un mismo mecanismo de ayuda —instrucción a demanda y pista en forma de pregunta tras tres fallos seguidos en la misma tarea (RF-13)—. Esa combinación materializa el criterio CP-04 de progresión de dificultad, que la Solución OE2 (§3.2) vincula con RF-03, RF-30 y RF-40.
