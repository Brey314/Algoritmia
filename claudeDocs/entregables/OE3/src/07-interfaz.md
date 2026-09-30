# 7. INTERFAZ, ESCENARIOS Y ELEMENTOS AUDIOVISUALES

Este capítulo da cuenta de la actividad «Integración de la interfaz, escenarios y elementos visuales» y del entregable «Interfaz integrada» del cronograma (véase §1.4). Tres documentos del proyecto, subordinados a la especificación técnica, gobiernan esta parte: la dirección de arte (paleta, línea, personajes, entornos, interfaz y accesibilidad visual), la dirección de música y sonido (pilares, mezcla e inventario de piezas) y el inventario de interfaces, que asigna a cada pantalla el requerimiento que la origina. Los mockups de interfaz fijan la disposición de cada pantalla antes de construirla. El detalle archivo por archivo está en los Anexos B (Fases 1 a 3), C, E, F y G.

## 7.1 Pantallas del flujo

La configuración de compilación incluye doce escenas: las cinco jugables del capítulo 4 y siete de flujo, con `Boot` como primera (véase §3.3). Desde el acta D03 (06/09/2026; Anexo H), la interfaz se diseña sobre una resolución de referencia de 1920 × 1080 con escalado proporcional, para que se lea igual en el proyector del aula que en un portátil. Es diegética: los paneles son tablillas de piedra clara (marfil `#F7EFE2`), los botones son piedras redondeadas —ámbar `#E8A33D` el primario, marfil el secundario— y el texto va en carbón `#3A1E18`, con Baloo 2 en títulos y Nunito en el cuerpo (licencia SIL OFL 1.1). El inventario de interfaces excluye el texto de las imágenes: lo escribe el motor encima. Los diálogos, las tareas y la configuración de cada nivel viven en ScriptableObjects editables sin recompilar (CT-05, RNF-18); los rótulos fijos de los menús, como los de la pausa o los del panel de perfil, están escritos en la escena o en el prefab. Como respuesta al clic, la cara del botón baja cuatro píxeles mientras se mantiene presionada, conforme a la asesoría de interfaz del acta D04 (09/09/2026): el botón debe hundirse, no solo cambiar de color. Un botón bloqueado no se hunde, porque hundirse promete que el clic tuvo efecto. La Tabla 7.1 enumera las pantallas del flujo.

**Tabla 7.1.** Pantallas del flujo del prototipo y requerimientos que atienden.

| Pantalla | Escena | Qué ofrece | Traza |
|---|---|---|---|
| Arranque | `Boot` | Sin interfaz: crea el objeto persistente y pasa al inicio | RNF-04 |
| Inicio | `MainMenu` | Título, Jugar, Créditos, Salir y «Progreso del equipo» | RF-01, RF-09, RF-46 |
| Selección de perfil | `MainMenu` (panel) | Perfiles guardados, un solo campo de nombre, borrado confirmado | RF-02, RF-47, RNF-09 |
| Menú de niveles | `LevelSelect` | Tres niveles; los no alcanzados, bloqueados | RF-03, RNF-19 |
| Escena narrativa | `Narrative` | Ilustración, diálogo con retrato, Continuar y Omitir | RF-05, RF-06 |
| Pausa | Prefab en las cinco escenas jugables | Reanudar, Reiniciar, Volver al menú de niveles | RF-07, HU-17 |
| Resumen de nivel | `LevelSummary` | Relato sin cifras y habilidad nombrada | RF-12, RF-17, RF-45 |
| Créditos | `Credits` | Papeles, autores y reconocimiento de autoría | RF-08, RNF-23 |
| Informe docente | `TeacherReport` | Indicadores por nivel y fase; eliminación de datos | RF-46, RF-47 |

**Inicio y perfil.** La pantalla de inicio muestra el título «Algoritmia», el subtítulo «Piensa el orden, enciende el fuego» y cuatro botones (Figura 7.1). «Salir» guarda el perfil activo antes de cerrar; al revés se perdería lo que el estudiante acababa de lograr (RF-09). El panel de perfil pregunta «¿Quién juega?», lista los perfiles guardados y pide un solo dato, el nombre, con el aviso «Solo el nombre: nada más se guarda» (RNF-09). Un nombre vacío o repetido produce un mensaje que orienta sin avanzar, y un perfil nuevo arranca en la apertura del Nivel 1 (HU-01). Borrar un perfil exige confirmar ante la advertencia de que su avance no se puede recuperar (RF-47). Lo vigilan, entre otras, `MainMenu_RF09_GuardaElPerfilActivoAntesDeCerrar` y `ProfileSelect_RNF09_ElFormularioPideUnSoloDato` (Anexo B, Fase 1).

![Figura 7.1. Pantalla de inicio capturada por la prueba de contraste: título sobre tablilla marfil, botón primario «Jugar», secundarios «Créditos» y «Salir», y el acceso del docente «Progreso del equipo». A la derecha, el recuadro de la ilustración de la familia y Algoritm, todavía provisional.](fig/fig-07-1-pantalla-de-inicio.jpg){width=16cm}

**Menú de niveles.** Los tres niveles se muestran siempre. Los que el perfil activo no ha alcanzado aparecen con el botón oscurecido, el candado y la palabra «Bloqueado», de modo que el estado no depende solo del color, y no responden al clic (Figura 7.2). Completar un nivel habilita el siguiente (RF-03); lo vigilan las cuatro pruebas `LevelSelect_RF03_*`.

![Figura 7.2. Menú de niveles con un perfil nuevo: el Nivel 1 disponible y los niveles 2 y 3 bloqueados, señalados por el botón oscurecido, el candado y el rótulo «Bloqueado» (RF-03, RNF-19). Los recuadros de arte conservan todavía el rótulo provisional.](fig/fig-07-2-menu-de-niveles.jpg){width=16cm}

**Escena narrativa.** Una sola escena reproduce las dieciocho secuencias narrativas, cada una definida en un asset (véase §4.5). La ilustración ocupa la pantalla completa y el cuadro de diálogo, en el cuarto inferior, muestra el retrato y el nombre de quien habla, el texto y «Continuar». Por decisión del acta D05 (13/09/2026), todo objeto que el texto nombra se ve entero y por encima del cuadro de diálogo en la parada de cámara en que se lo nombra, lo que comprueba parada por parada `NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo`; la misma acta fijó que un cambio brusco de cámara dentro de una escena funda a negro sin tapar el cuadro de diálogo. «Omitir» solo aparece si el nivel ya se terminó antes; el cierre reflexivo nunca se omite la primera vez (RF-06, CP-07), y el del Nivel 3 tampoco al repetirlo (INC-51; Anexo B, Fase 2, §B.1). Los cambios entre narrativa y mecánica, en ambos sentidos, y entre dos narrativas encadenadas funden a negro, mientras que los menús cortan en seco (véase §3.3).

**Pausa.** Su lugar en el flujo y la diferencia de sus rótulos con HU-17 (INC-49, abierta) se describen en §3.3. En pantalla sigue el mockup 6: un velo carbón al 72 % y una tablilla con «Reanudar», «Reiniciar» y «Volver al menú de niveles». El tiempo en pausa no suma al tiempo de resolución (RF-07). «Reiniciar» pide confirmación y repite solo la fase activa, sin tocar lo ya confirmado (RF-41, INC-25). Ninguna ruta de la pausa lleva a una pantalla de derrota (`PauseMenu_CP02_NingunaRutaDeLaPausaLlevaAUnaPantallaDeDerrota`).

**Resumen, créditos e informe docente.** El resumen de fin de nivel (mockups 13 y 13b) se describe en §6.3; tras el Nivel 3 conduce a la escena final y esta a los créditos (INC-39). Los créditos presentan pares de papel y persona, el reconocimiento de que los personajes se basan en diseños usados con autorización escrita y la licencia de las tipografías (RF-08, RNF-23). El informe docente (véase §6.4) es la única pantalla con cifras y solo se alcanza desde «Progreso del equipo» (`TeacherReport_CP03_NingunaRutaDelEstudianteAlcanzaLaPantallaDeCifras`); desde él también se eliminan los datos de un perfil, con confirmación propia (Anexo G).

**Pendientes.** El inicio, el menú de niveles y los créditos conservan recuadros de arte provisionales. La tipografía del informe docente y de la etiqueta «Progreso del equipo», que aún usan la fuente integrada del motor (visible en la Figura 7.1), y el desplazamiento por arrastre de las listas del informe y de los créditos se tratan en §10.4.

## 7.2 Escenarios, personajes y animación

La dirección de arte parte de tres condiciones: el público es infantil, los equipos de la institución no tienen tarjeta gráfica dedicada (CT-02) y la legibilidad manda sobre el detalle. De ellas derivan sus pilares: la silueta primero, color plano sin degradados, el personaje siempre gana sobre el fondo, lo interactivo se señala con saturación y no con brillo, y redondez sobre angulosidad. Un error no se marca en rojo, no suena a fallo y no produce una expresión negativa; los únicos colores de estado son el verde de éxito `#5FA842` y el ámbar de atención `#E8A33D`.

**Escenarios.** El prototipo usa nueve ilustraciones de entorno (Tabla 7.2). Los encuadres se expresan en fracciones del lienzo y no en píxeles, así que una entrega de arte que conserve la composición entra sustituyendo el archivo, sin cambiar ningún valor de cámara.

**Tabla 7.2.** Ilustraciones de entorno por nivel y tratamiento de la luz.

| Nivel | Ilustraciones | Uso | Luz |
|---|---|---|---|
| 1 · La Oscuridad | Exterior nocturno; cueva frontal; cueva cenital | Apertura y puente I; narrativas 1.1 a 1.3, 2.5 y arranque del puente II; mecánica | Oscuridad que se abre con el avance (RF-21) |
| 2 · La Rueda | Bosque claro; laberinto cenital | Puente I en el bosque, narrativas 2.1 a 2.4, bosque y taller; laberinto | Del amanecer a la noche, por tinte |
| 3 · El Río | Río; zona de construcción; horizonte; poblado | Orilla y escenas del río; marca de la zona en la orilla; horizonte del puente II; escena final | Sin tinte, salvo el arranque en el refugio |

En el Nivel 1 lo que el jugador ve lo decide la luz: una capa de oscuridad multiplica la ilustración y en la mecánica se aclara conforme se acumulan golpes efectivos, como refuerzo del avance y nunca como temporizador ni como único canal (RF-21, CP-02). El Nivel 2 dura un día y lo cuenta la luz: amanecer azulado en el puente I, tarde sin tinte en la recolección y el armado, atardecer en la escena 2.4 y noche junto al fuego en la 2.5; el laberinto se tiñe de atardecer (`NarrativeSequence_RF05_ElNivel2TranscurreDelAmanecerALaNoche`, `MazeScene_RF30_ElLaberintoEsAlAtardecer`). El refugio es la cueva del Nivel 1 con la hoguera encendida, que reaparece en la 2.5 y al comienzo del puente II.

**Personajes.** La familia —Papá, Mamá, la Niña y el Niño— y el guía Algoritm aparecen en las cinco escenas jugables, y cada una de las dieciocho narrativas lleva al menos un personaje. El protagonista cambia por nivel: Papá en el fuego, la Niña en la rueda y Mamá en el río (CN-02). Se animan como marionetas por recorte: cada miembro de la familia está cortado en cinco partes con el pivote en la articulación, y un controlador de animación las gira y desplaza con un estado por acción. Hay siete prefabs, 21 clips por miembro de la familia y 9 para Algoritm. La dirección de arte preveía el paquete de animación 2D del motor, pero su deformación actúa sobre sprites que quedarían debajo del lienzo de interfaz, tapados por la ilustración; el recorte conserva la animación sobre el sprite base sin hojas de fotogramas (INC-53, abierta).

Ningún personaje salta, cae ni hace un gesto de derrota: tras un intento sin avance, el personaje jugable hace un gesto de ánimo (CP-02, `CharacterRig_CP02_NingunClipEsDeDerrotaCaidaNiSalto`). Las partes de los personajes no reciben clics, así que nunca le quitan el clic a un objeto del reto. En la narrativa, el retrato del cuadro de diálogo es el de quien dice la línea (Figura 7.3). El 24/09/2026 se revisaron las capturas del motor, una por línea de diálogo, y se corrigieron clips y posiciones (Anexo F). El botón de pista de cada mecánica es un círculo con el Algoritm del nivel. Algoritm se entregó como una llama con extremidades, no como la estrella que describe la dirección de arte; sus formas de rueda y gota son la llama recoloreada, provisionales, y el arte definitivo entrará sustituyendo el archivo (INC-52, abierta).

![Figura 7.3. Escena final del juego: la familia llega al poblado al atardecer, con fogatas y hoguera animadas y su humo. El cuadro de diálogo muestra el retrato y el nombre de Algoritm en su forma de gota, que dice esa línea.](fig/fig-07-3-escena-final.jpg){width=16cm}

**Animación de objetos.** El fuego se anima por cuadros —una llama frontal en las narrativas y una cenital en la mecánica del Nivel 1—, con una clave por dibujo distinto y no por archivo entregado (véase §8.2). El humo nace con el fuego en el cierre del Nivel 1 y sale en bucle detrás de cada llama de otras cinco narrativas (`NarrativeSequence_RF05_CadaLlamaEchaHumoPorDetrasYPorEncima`). Los troncos del Nivel 2 ruedan en el motor: un componente dibuja cada uno como cilindro en tres cuartos a partir de una sola textura, igual en el bosque y en las narrativas (INC-50). El acta D09 acordó animarlos en el motor y no por cuadros, separando el tronco en cuerpo y veta; la implementación lo resolvió con esa única textura (Anexo E, §A.1).

**Pendientes.** Siguen abiertos la decisión sobre figuras frontales en las vistas cenitales, los retratos con expresiones distintas de la neutra y varios objetos del guion sin sprite propio, como la comida o la estela de Algoritm (Anexos E y F).

## 7.3 Sonido

La dirección de sonido define el juego como «materia golpeada por manos humanas»: piedra, madera y agua. Dos de sus pilares tienen consecuencia pedagógica directa. No existe sonido de fallo: un intento sin éxito suena descriptivo, nunca punitivo (CP-02). Y el audio nunca es el único canal, espejo de RNF-19: un aula con los parlantes apagados debe poder terminar el juego sin perder información.

El gestor de audio es uno de los tres únicos componentes que persisten entre escenas; los tres comparten un mismo objeto de la escena de arranque, que lleva también el único oyente del juego. Mezcla cuatro buses —música, ambiente, efectos y voz—, reproduce el ambiente en dos capas con fundidos cruzados y aplica cortes secos para los silencios del guion. El sonido es dato y no código (CT-05): cada secuencia declara su ambiente, cada línea su efecto y su silencio, y cada nivel referencia sus piezas desde un ScriptableObject. Si el gestor falta, el juego sigue en silencio. Una regla de importación aplica a cada pieza, según su prefijo, los ajustes de compresión de la dirección de sonido (`AudioImport_RNF06_CadaFamiliaEntraConLosAjustesDeSuTabla`).

El inventario prevé 104 piezas; al 25/09/2026 hay 19 archivos en el proyecto, de los que 17 suenan en el juego (Tabla 7.3).

**Tabla 7.3.** Sonido incorporado por nivel.

| Nivel | Fecha | Ambientes | Efectos |
|---|---|---|---|
| 1 | 21/09/2026 | Noche a la intemperie, cueva oscura, hoguera en segunda capa | Pasos, hojas, golpe con contacto, chispa del golpe efectivo, soplo |
| 2 | 23/09/2026 | Bosque de día en el puente I, las escenas 2.1 a 2.4 y las tres fases | Objetos del bosque al apartarse, carretilla, encaje y martillazos |
| 3 | 25/09/2026 | Río con el bosque de fondo; cruce de la balsa en segunda capa | Material recogido, martillazo por pieza, tres al aprobar una fase |

En el Nivel 1 están cableados dos de los tres silencios del guion: el corte seco de todo el sonido en la línea «Solo oscuridad.», salvo el eco de pasos que aún suene (S1), y el corte al apagarse Algoritm (S2). El golpe solo suena si las piedras se tocan y la chispa solo acompaña al golpe efectivo. Ninguna pieza lleva en su nombre derrota ni error (`AudioAssets_CP02_NingunaPiezaSeLlamaDerrotaNiError`), y en el Nivel 3 lo que no avanza no suena: una fase que no pasa, la balsa que se hunde o llegar a la zona de construcción sin todos los materiales (`AssemblyPanel_CP02_UnaFaseQueNoPasaNoSuena`).

**Pendientes.** No hay música ni sonido de diálogo; de los efectos de los niveles 2 y 3 que nombra el inventario, ninguno llegó con su nombre, y los suplen piezas globales y otras con nombre distinto; dos archivos están sin uso, y no hay control de volumen, porque RF-01 y RF-07 no lo piden (PS-01, abierta). El detalle está en §10.3. Además, la escena final no declara ambiente ni el tercer silencio. El acta D08 (23/09/2026) dejó el sonido restante a cargo de Santiago Benavides Rey.

## 7.4 Accesibilidad

La accesibilidad se apoya en cinco requerimientos no funcionales, un criterio pedagógico y un criterio técnico (Tabla 7.4). Las pruebas que la vigilan son aserciones automáticas y pruebas de verificación visual, que además guardan una captura para revisarla a mano; de las capturas de verificación visual del 25/09/2026 provienen las figuras de este documento.

**Tabla 7.4.** Criterios de accesibilidad, solución aplicada y verificación.

| Criterio | Requisito | Solución aplicada | Verificación |
|---|---|---|---|
| Doble indicador | RNF-19 | Candado, icono, forma o texto junto a todo color | Aserciones en los tres niveles y capturas |
| Contraste | RNF-20 | Texto carbón sobre tablilla marfil | Cálculo de al menos 4,5:1 en los tres niveles |
| Sin destellos | RNF-21 | Luz que no retrocede; movimientos continuos, sin parpadeo | Muestreo cuadro a cuadro y capturas |
| Carga cognitiva | CP-08, RNF-01 | Oraciones breves, una tarea activa, icono en la tablilla de los niveles 2 y 3 | Barridos de contenido y cuadro de diálogo |
| Solo clic | RNF-02, CT-06 | Clic y clic sostenido; botones en pantalla | Mapa de controles de las cinco escenas |

**Doble indicador (RNF-19).** Ningún estado se comunica solo con color. El menú de niveles marca el bloqueo con candado y rótulo; «Soplar» deshabilitado lleva solo el candado, desde que el acta D06 (15/09/2026) le retiró el rótulo «Aún no», y «Mecanizar» deshabilitado, candado y «Aún no». En los niveles 2 y 3 cada rechazo se describe en palabras y lleva un icono distinto del de acierto. En el laberinto los tres bloques se distinguen por forma, y el bloque donde se detuvo la carretilla, por contorno y tamaño; en la balsa, el espacio incorrecto lleva el icono de alerta. La lista de tareas del Nivel 3 marca lo hecho con una forma distinta, y su prueba la captura en escala de grises (Figura 7.4). Lo vigilan, entre otras, `WheelLevel_RNF19_LosTresEstadosDeErrorSeLeenSinColor`, `RiverLevel_RNF19_LosEstadosDeErrorSeLeenSinColor` y `RiverLevel_RNF19_LaListaDeTareasSeLeeEnEscalaDeGrises`.

![Figura 7.4. Orilla del Nivel 3 capturada en escala de grises por la prueba de RNF-19: sin color, las tareas hechas se distinguen de las pendientes por la marca dentro del círculo. Abajo a la derecha, los botones de dirección en pantalla que sustituyen al teclado (RNF-02, INC-01).](fig/fig-07-4-lista-en-grises.jpg){width=16cm}

**Contraste (RNF-20).** El par carbón sobre marfil alcanza aproximadamente 13:1 según la fórmula de luminancia relativa, y la dirección de arte prohíbe el texto claro sobre fondo oscuro. Tres pruebas calculan el contraste en la escena cargada y exigen al menos 4,5:1: `FirePanel_RNF20_ContrasteSuficienteEnElEstadoMasOscuro`, con la cueva en su estado más oscuro; `WheelLevel_RNF20_ContrasteSuficienteEnLasTresEscenas`, sobre mensajes, botones, bloques y pausa del Nivel 2, y `RiverLevel_RNF20_ContrasteSuficienteSobreElEscenarioClaro`, sobre la orilla, la lista y el ensamblaje. La pantalla de inicio se revisa por captura (Figura 7.1); su primera verificación, registrada en el acta D03, dio aproximadamente 13,5:1 en el título, 7:1 en el botón de jugar y 10:1 en los secundarios.

**Sin destellos (RNF-21).** La luz de la cueva sube un escalón por golpe efectivo, sin retroceder nunca, y alcanza la iluminación completa en un solo barrido al nacer el fuego; los movimientos ambientales del bosque tienen amplitud pequeña. Las pruebas de los niveles 2 y 3 siguen cuadro a cuadro el acercamiento del bosque, el recorrido de la carretilla, el empuje de cámara, el pulso de fase confirmada y el hundimiento de la balsa, y exigen que nada se apague y encienda y que ningún cuadro salte más de un umbral; la del Nivel 1 captura el nacimiento del fuego para revisarlo. En la corrida completa del 25/09/2026, `RiverLevel_RNF21_NingunaAnimacionDelNivel3TieneDestellos` falló una vez (un salto de 0,076 del alto de la balsa frente a 0,06) y pasó al correrla sola; como el hundimiento avanza con el tiempo de cada cuadro, queda abierta como prueba sensible al tiempo (véase §9.3).

**Carga cognitiva (CP-08).** Cada fase presenta una sola tarea activa (RNF-03) y el cuadro de diálogo no supera el cuarto inferior de la pantalla (`NarrativeScene_RNF03_ElCuadroDeDialogoNoSuperaElCuartoDeLaPantalla`). Cuatro barridos comprueban que narrativas, pistas, resúmenes y mensajes del bosque no tengan oraciones de más de veinte palabras (RNF-01), y otra prueba verifica que la línea más larga de las dieciocho secuencias cabe en su cuadro. En los niveles 2 y 3, cada respuesta aparece en una tablilla con icono, que las pruebas de RNF-19 exigen presente. Ninguna prueba cita CP-08 en su nombre: el límite de dos líneas por instrucción es una regla de redacción de la especificación técnica, sin prueba propia.

**Solo clic (RNF-02).** `Controls_RNF02_LasCincoEscenasJugablesSoloAceptanClicYClicSostenido` carga las cinco escenas jugables y comprueba que usen el mapa de controles del juego, que toda vinculación sea un clic del puntero y que no haya listas desplazables por arrastre. Otras pruebas prohíben la clase de entrada heredada del motor y cualquier vinculación de teclado en el Nivel 3 (`RiverScene_INC01_NoExisteVinculacionDeTecladoEnElMapaDeControles`). Por eso Mamá se mueve con botones de dirección en pantalla accionados con clic sostenido (Figura 7.4), y la lista de bloques del laberinto se desplaza con una barra que se pulsa y no se arrastra. La excepción pendiente está fuera de las escenas jugables: las listas del informe docente y de los créditos (véase §10.4).
