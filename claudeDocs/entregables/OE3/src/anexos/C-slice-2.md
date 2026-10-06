# Slice 2: Nivel 2 «La Rueda»

Este anexo describe el segundo incremento del desarrollo: el Nivel 2, «La Rueda», con sus tres fases jugables (el bosque, el taller y el laberinto), sus siete secuencias narrativas y lo que la navegación y el andamiaje necesitaron para servir a un nivel de varias fases. Presenta el nivel tal como se juega en el corte del 1 de octubre de 2026. El slice se ejecutó del 10 al 15 de septiembre de 2026, en paralelo con la Fase 3 del Slice 1, y el nivel recibió ajustes hasta el 1 de octubre. Se redactó a partir del documento de resultados del slice, que el repositorio conserva en `claudeDocs/tasks/Slice 2/` con sus dos anexos fechados (25/09 y 01/10), de su plan y su tablero, de las actas de seguimiento y del registro de inconsistencias. Sirve de soporte al apartado 4.3 y a los capítulos 5 y 6 del documento principal.

## 1. Alcance y seguimiento

El plan del slice organizó el trabajo en dieciocho tareas, de W01 a W18, repartidas en seis fases con un punto de control cada una (W-A a W-F); el 15 de septiembre se añadió W19. El incremento cierra los requerimientos RF-22 a RF-34, todos de prioridad alta, y generaliza a un nivel de tres fases RF-03, RF-04, RF-05, RF-06, RF-07, RF-10, RF-11, RF-12, RF-13 y RF-45. Sus historias de usuario propias son HU-08, HU-09 y HU-10, y sus casos de uso, CU-06, CU-07 y CU-08. Con él se vuelven verificables por primera vez dos requerimientos no funcionales: RNF-16, porque la exclusión entre niveles solo puede fallar cuando existen dos, y RNF-19, cuyo criterio de verificación es la inspección de los estados de error de los niveles 2 y 3. Quedaron fuera el Nivel 3, el informe docente, RF-46, RF-47 y el nivel avanzado opcional del guion (Solución OE2, apartado 1.10), cuya presión de tiempo contradice CP-02.

El trabajo se hizo en la rama `feat/Slice-2`, abierta desde el cierre de la Fase 3 del Slice 1, y llegó a la rama principal con la solicitud de integración n.º 74 el 15 de septiembre. Todos sus commits son de Santiago Benavides Rey, que asumió el cierre del slice en el acta D05 (13/09/2026); Santiago Valdiri García tomó entonces el sonido del juego. La Tabla C.2 resume tareas, fechas y commits, y la Tabla C.3 los cambios que el nivel recibió después del cierre.

**Tabla C.1.** Actas que gobernaron el incremento.

| Acta | Fecha | Qué decidió para este incremento |
|---|---|---|
| D02 | 05/09/2026 | El Nivel 2 con sus tres fases es el segundo de cuatro incrementos verticales, con plan y tablero propios |
| D04 | 09/09/2026 | El Nivel 2 es un bosque de mediodía y el tono de madera trabajada se reserva a los objetos de la mecánica; asesoría de interfaz (hundido del botón, botón secundario neutro, sombra plana) que el nivel aplica en sus tres escenas |
| D05 | 13/09/2026 | Todo objeto nombrado se ve entero sobre el cuadro de diálogo, y si no concuerda con el encuadre se mueve el objeto; un corte funde a negro, y las escenas 2.1 y 2.3 abren desde negro; la escena 2.5 queda declarada fuera de la comprobación de encuadres hasta implementarse; las hojas de encuadres se generan desde el contenido; Santiago Benavides Rey asume el cierre del slice |
| D06 | 15/09/2026 | Aprueba la salida de «Empujar» hacia la narrativa y da por terminado el código de la rama; las cuatro familias de objetos se distinguen por silueta; cada familia vuelve a su sitio con un movimiento propio; el rodado pertenece a la escena narrativa; se fusiona la rama (D06-3) |
| D07 | 19/09/2026 | Da por finalizada D06-3: la rama del slice quedó fusionada |
| D08 | 23/09/2026 | Aprueba los sonidos del nivel, el refugio con fuego en la 2.5, el día contado por la luz, el fundido entre narrativa y juego, la barra de la lista que se pulsa y no se arrastra y el retiro de un bloque sin confirmación |
| D09 | 24/09/2026 | Los troncos del Nivel 2 se animan en el motor (D09-1); fogata en el cierre del nivel (D09-4) |
| D10 | 30/09/2026 | D09-1 finalizada; la 2.2 abre con la caja del bosque y la carretilla del taller pasa al dibujo amarrado (D10-2); el texto del nivel va siempre sobre marco (D10-5); PG-05 queda a cargo de Santiago Benavides Rey |

La tarjeta que abrió el trabajo fue D04-4, «Iniciar desarrollo del Slice 2 en paralelo de la finalización del primer Slice», a cargo de Santiago Benavides Rey, que el acta D05 registra con fecha límite del 20 de septiembre. D06-3, «Fusionar la rama del segundo slice conforme a lo aprobado hoy», a cargo de los dos estudiantes, se dio por finalizada en el acta D07. D09-1, «Realizar la propuesta de los troncos del nivel dos para animarlos en Unity», se cumplió por un camino distinto del propuesto y el acta D10 la dio por finalizada. Las correcciones de cierre son de las tarjetas D10-2 y D10-5.

**Tabla C.2.** Fases del slice, tareas y puntos de control.

| Fase | Tareas | Fecha | Commits | Punto de control |
|---|---|---|---|---|
| 0. Cimientos | W01, W02 | 10/09 | `04ee71a`, `e7278a2`, `a012515` | W-A |
| 1. Andamiaje generalizado | W03, W04 | 10/09 | `5f7a432`, `9f10485` | W-B |
| 2. Bosque | W05 a W07 y ajuste de cursor | 10 y 11/09 | `817a327`, `faaaa13`, `eb941c8`, `416ec79` | W-C |
| 3. Taller | W08, W09 y ampliación del mazo | 12 y 14/09 | `b6b886c`, `17ae49a` | W-D |
| 4. Laberinto | W10 a W14 | 13/09 | `b1b423a` | W-E |
| 5. Cierre del nivel | W15 a W18 | 15/09 | `8de614f` | W-F |
| Salida a la narrativa | W19 | 15/09 | `863ef05` | Sin punto de control |

**Tabla C.3.** Cambios del Nivel 2 después del cierre del slice.

| Fecha | Commit o tarjeta | Qué cambió |
|---|---|---|
| 16/09 | `a9236f1` | Auditoría de cámara: el bosque y el taller se juegan con el plano general |
| 17/09 | `d81cfc7` | Botones de desplazamiento del laberinto; caja y fila colgadas de la ilustración; regla de «Omitir»; estilo de la pausa |
| 23/09 | `03675b9`, `d7ace67` | Sonido del nivel; la 2.5 en el refugio con fuego; el día contado con luz; cuadrícula, barra y papelera del laberinto |
| 24/09 | `88fe0ee`, `b38c00b` | Personajes animados (Anexo F); troncos que ruedan como cilindros |
| 25/09 | `1d5ce58`, `44fd479` | La cuerda como séptima pieza; distractores variados; props definitivos a 256 y 512 px |
| 29/09 | `d105838` | El recorrido del nivel exige ver la 2.5 antes del desbloqueo; un doble clic ya no salta el resumen |
| 30/09 | `37b3cb7` | Contorno en «Girar», candado en «Empujar», rótulos del laberinto en datos, cuerda en la 2.3 y mazo soltado lejos sin intento |
| 01/10 | D10-2, D10-5 | Caja de la 2.2, carretilla amarrada y contraste con marco |
| 01/10 | Ejecutable del corte | Fila arrastrada que se aparta de la lista, lista que sigue al bloque en curso y papelera a mano tras retirar (`SequenceListRules`) |

## 2. Lo implementado

### 2.1 Cimientos y andamiaje generalizado (W01 a W04)

El 10 de septiembre, con el Slice 1 en su Fase 3, se decidió abrir este slice en paralelo (apartado 3), y esa misma madrugada entraron sus cuatro primeras tareas.

En el módulo del nivel (W01), `Game.Levels.Wheel` nació referenciando `Game.Core` y `Game.Scaffolding`; dentro del slice ganó `UnityEngine.UI` y `Unity.InputSystem`, este para leer la posición del cursor, y después `Game.Audio`. Nunca ha referenciado otro nivel. Con dos niveles reales, `Architecture_RNF16_NingunAssemblyDeNivelReferenciaAOtroNivel` pasó a poder fallar, y el punto de control W-A la declaró en verde con 63 de 63 pruebas EditMode.

En el progreso por fases (W02), `PhaseId` identifica una fase por nivel y número, y `PhaseId.PhasesPerLevel` fija una fase para el Nivel 1 y tres para los niveles 2 y 3. `PlayerProfile.ConfirmPhase` recibe un `PhaseId`, y `LevelUnlockPolicy` dejó de creerle al llamante: exige que todas las fases del nivel estén confirmadas, porque con tres fases aceptar su palabra habría abierto el Nivel 3 a media rueda. El JSON del perfil no cambió y sigue escribiendo nivel y fase por separado (RNF-09). En el taller (W09) el carril tuvo que tocar `Game.Core`: `GameFlow` acepta el paso de una narrativa a otra y, cuando se pide una fase ya confirmada, retoma en la primera fase sin confirmar que tenga escena (RNF-14); `GameFlowRunner` vuelve al menú si una fase no tiene escena.

En la ayuda por fase (W03), `HintPolicy` pasó a llevar la cuenta por fase, porque una por nivel habría dado la pista del bosque en el laberinto. `N2_Guia` tiene tres pasos, «Seleccionar», «Construir» y «Programar», cuyas pistas son preguntas que orientan sin resolver: «Mira cómo se mueve cada uno cuando lo empujas. ¿Cuáles se traban y cuáles no?», «Mira la pieza que quieres poner. ¿Sobre qué se apoyaría si la sueltas ahora?» y «Mira en qué paso se detuvo la carretilla. ¿Hacia dónde miraba justo antes?». Desde el 29 de septiembre esas pistas están en el guion (INC-64).

En las secuencias narrativas (W04), cada escena narrativa del nivel es un asset, sin escena de Unity ni rama propia; hoy son siete, porque el 23 de septiembre la escena puente se partió en dos para cambiar de fondo a mitad de escena (apartado 2.5). Al implementarlas apareció un defecto de la regla de «Omitir»: el cierre reflexivo se habría ofrecido para omitir desde la primera vez, contra CP-07. Desde entonces el cierre cuenta como visto solo si el nivel siguiente ya está desbloqueado, que es lo que vigila `NarrativeVisitPolicy_RF06_ElCierreDelNivel2NoEsOmitibleLaPrimeraVez`, y las demás escenas, desde el 17 de septiembre, cuando el perfil ya confirmó la última fase del nivel (`NarrativeVisitPolicy_RF06_UnaEscenaIntermediaNoSeOmiteEnLaPrimeraVuelta`). El 15 de septiembre se encontró que la escena puente no se veía nunca, porque no declaraba salida y el nivel abría en la 2.1. Desde entonces la ficha del menú abre `N2_PuenteI`, que encadena con `N2_PuenteI_Bosque` y esta con `N2_Escena21_Bosque`, siempre por el campo `NextSequenceId` del asset.

### 2.2 Bosque: selección por patrón (W05 a W07 y W19)

La primera fase pide abstraer: entre los objetos del claro, el estudiante reconoce qué tienen en común los que sirven, que son redondos y ruedan, reúne cinco troncos, coloca la caja de alimentos encima y la empuja.

En la regla y la escena (W05 y W06), `PatternSelection`, en C# plano, decide qué es válido y qué es distractor y elige el mensaje de cada categoría. Un rechazo no retira nada ni cierra ningún camino (CP-02, RF-18), y el objeto vuelve a su sitio con una frase que describe sin juzgar: «Esta piedra tiene esquinas. Cuando la empujas, se traba.», «Esto no soporta el peso de la caja.» o «Sirve para golpear, no para mover cosas pesadas.»; un tronco aceptado responde «Este rueda. ¿Qué tiene que los otros no tienen?». `Level2_Forest` reparte catorce objetos por el suelo: cinco troncos, tres piedras, tres plantas y tres herramientas. Cada uno se dibuja más pequeño cuanto más al fondo del claro está (a 0,6 en el borde alto) y ninguno queda bajo una tablilla ni encima de otro, porque un tronco tapado no recibiría el clic y la fase no podría terminarse. El acopio tiene cinco casillas y el contador «Troncos redondos: n de 5» se ve durante toda la fase (RF-24).

Al acercar el cursor (W05/W06-R), cada objeto crece hasta 1,4 y se aparta según su forma: el tronco rueda lejos, la piedra vuelca una esquina y se detiene, la planta se levanta en un arco lento y vuelve a posarse y la herramienta apenas se arrastra, cada uno con su sonido. Es un realce visual: el clic sigue siendo el único control (RNF-02). INC-60 llevó ese comportamiento al guion, a CU-06 y a HU-08 el 29 de septiembre.

Con los cinco troncos reunidos (W07 y W19), los objetos que quedan en el suelo se ocultan, los troncos vuelan del acopio a una fila junto a la caja y el plano se cierra en 1,2 s, del encuadre general al del cierre (foco 0,232; 0,42 y zoom 1,58), que es exactamente el encuadre con que abre la escena 2.2. La caja se lleva con clic sostenido y se deja al soltarlo (RF-25); si cae fuera de los troncos se queda donde cayó con el mensaje «Ahí la caja no toca los troncos. Déjala encima de ellos.». «Empujar» se habilita solo con la caja colocada y, mientras está deshabilitado, lleva candado además del tono atenuado (INC-58, desde el 30/09). En la primera versión la caja rodaba dentro de la mecánica; desde W19 (15/09), a pedido de Santiago Benavides Rey, «Empujar» confirma la fase 1, guarda y sale a `N2_Escena22_ElPatron`, que es la que muestra el rodado, y la fila de troncos va pegada, como la dibuja esa escena. El acta D06 lo aprobó e INC-50 corrigió en consecuencia RF-26, el guion, CU-06 y HU-08.

Para contar ese rodado (W07) se construyó en `Game.Scaffolding` y `Game.UI` el mecanismo con que hoy se ponen en escena las dieciocho narrativas del juego. `IllustrationFraming` traduce un encuadre (foco en fracciones de la ilustración y zoom) contra el tamaño real del sprite y escala la ilustración, de modo que todo lo que cuelga de ella acompaña el paneo sin cálculo propio, y sustituir el archivo de arte basta. `NarrativeSequence` declara paradas de cámara por línea con un suavizado propio; `NarrativeProp` coloca objetos en fracciones de la ilustración y les da movimiento, y `RollMotion`, en C# plano, calcula el avance, la caída, el giro y el ladeo de lo que rueda. `DialogueRunner` ganó `Progress` e `Index` para llevar la cámara al ritmo de la lectura. El corte de cámara que funde a negro y la escena que abre desde negro llegaron el 13 de septiembre, conforme al acta D05.

Los troncos ruedan como cilindros (24 y 25/09; acta D09): `RollingLog` es un elemento gráfico de uGUI que dibuja el tronco como un cilindro visto en tres cuartos a partir de una sola textura (`prop_n2_tronco_textura`), sin cámara ni modelo 3D. Lee el giro que le dan `RollMotion` o el cursor y lo traslada a la textura, y su vista es contenido en `N2_TroncoRodante.asset`. Lo usan los troncos del bosque y los de las narrativas `N2_PuenteI_Bosque`, 2.1, 2.2 y 2.5. Por decisión de Santiago Benavides Rey, desde el 25 de septiembre las plantas y las herramientas usan cada una su propio dibujo, mientras los cinco troncos siguen siendo un solo sprite girado: la redondez es lo único que se repite.

Desde el 01/10 (INC-120), la 2.2 abre con la misma caja llena que la fase del bosque deja sobre los troncos, `prop_n2_caja_suelo`, en el punto (0,2; 0,5089) y con 0,148 del alto de la ilustración, que es el valor de `WheelLevelConfig.CargoPlacedPosition`. La caja se desliza sobre los troncos sin girar, y los que giran son los troncos. El cambio deshace exactamente el del 25 de septiembre, cuando la 2.2 había pasado a dibujar una caja vacía más pequeña y más alta; la caja vacía quedó sin uso y se renombró sin tilde (INC-126). La prueba `WheelLevelConfig_RF05_LaCajaColocadaEsLaQueLaEscena22DibujaAlAbrir` salió en rojo con la caja a 0,545 de altura frente a 0,5089, y `ForestScene_RF26_LaCajaYLosTroncosTerminanEnCuadroDondeLosDibujaLaEscena22` dejó una constante copiada y lee el asset. La captura `NarrativeScene_RF05_CapturaDelPrimerCuadroDeLaEscena22` muestra la caja entera al inicio de la fila, antes de rodar y por encima del cuadro de diálogo.

### 2.3 Taller: ensamblaje secuencial (W08 y W09)

La segunda fase pide ordenar: perforar dos troncos cortos para hacer ruedas, unirlas con el tronco largo como eje, montar la tabla, colocar la caja y amarrarla con la cuerda. `AssemblySequence`, en C# plano, sabe qué paso toca y rechaza el que llega fuera de orden con un mensaje que dice qué falta sin deshacer nada de lo hecho (CP-02, RF-29): «La tabla no tiene sobre qué apoyarse todavía.». `Level2_Workshop` monta la carretilla sobre lo anterior, cambiando el dibujo en su sitio en lugar de quitar piezas del suelo. «Mecanizar» se habilita solo con un tronco corto resaltado, y deshabilitado lleva candado y «Aún no»; un tronco ya perforado responde «Ese tronco ya tiene su agujero: es una rueda.» (INC-61). Las piezas se llevan con clic sostenido y una pieza soltada lejos del lugar de armado vuelve a su sitio sin contar como intento.

Al revisar las relaciones entre los componentes del taller (14/09) se vio que la pieza herramienta, el mazo, no participaba de ninguna mecánica. Santiago Benavides Rey decidió que perforara al soltarlo sobre el tronco resaltado, y el botón «Mecanizar» se conservó porque RF-28 lo nombra; los dos caminos producen la misma perforación. Desde el 30 de septiembre el mazo soltado lejos del tronco tampoco suma intento, igual que las demás piezas (INC-115).

Por decisión de Santiago Benavides Rey (25/09, INC-54), el armado termina amarrando la caja con una cuerda, la séptima pieza. Soltarla antes de que la caja esté encima se rechaza con «La cuerda todavía no tiene nada que sujetar.», y el paso final responde «La cuerda sujeta la caja. La carretilla está completa.». Con la cuerda, los pasos utilizados de la fase pasaron de cinco a seis. La escena 2.3 pinta la cuerda que nombra Algoritm desde el 30 de septiembre, y el 29 el guion, RF-27, RF-29, HU-09 y CU-07 la recogieron.

Los props del nivel (25/09 y 01/10) bajaron a 256 × 256 px, y a 512 × 512 donde se ven a más de 256 px a 1080p. El arte nuevo corrió un puesto las etapas de la carretilla: la primera es la rueda perforada, la segunda el eje, la tercera la tabla y la cuarta la caja. Desde el 1 de octubre (INC-121), al amarrar la cuerda la carretilla pasa a su quinto dibujo, `prop_n2_carretilla_e5`, el mismo con que abre la escena 2.4; el campo `AssemblyContent.TiedArt` lo declara. Los dibujos cuarto y quinto ocupan la misma caja de transparencia, así que la carretilla no salta de sitio. Hasta entonces la cuerda quedaba como una pieza suelta sobre la caja. `WorkshopScene_INC54_AlAmarrarLaCuerdaLaCarretillaPasaAlDibujoConCuerda` salió en rojo con el cuarto dibujo, y la prueba de captura del taller suma la vista de la carretilla amarrada.

### 2.4 Laberinto: editor de bloques (W10 a W14)

La tercera fase pide escribir un programa: componer una secuencia de bloques que lleve la carretilla al refugio y ejecutarla de una vez. Era la tarea de mayor riesgo del slice y se resolvió el 13 de septiembre, con el mockup 10 como referencia.

En el tablero (W10), `MazeGrid` es una matriz de 16 × 11 casillas cuyo anillo exterior es un seto, salvo la salida y el refugio. `MazeGrid.Generate` siembra al cargar la fase 24 obstáculos (arbustos) en tramos de hasta cinco casillas y exige, con una búsqueda en anchura, un camino con un rodeo mínimo de dos; si no lo hay, siembra uno menos. El trazado cambia en cada carga y se conserva entre ejecuciones (INC-57). La solución calculada solo la ven las pruebas, porque mostrarla resolvería la tarea (CP-06). `CartState` representa la carretilla como un valor (casilla y orientación) y los bloques se leen respecto de hacia dónde mira (INC-33): con la lectura absoluta el refugio podía quedar inalcanzable.

En los bloques y su ejecución (W11 a W14), «Avanzar» y «Retroceder» llevan una cuenta de 1 a 9 y «Girar» un lado, izquierda o derecha; la cuenta y el lado se ajustan sobre el bloque ya colocado, que se engancha donde se suelta (INC-55). `BlockSequence` no limita bloques ni ediciones (CP-02). `SequenceExecutor` recorre la secuencia paso a paso, a 0,6 s por paso, resaltando el bloque en curso; un movimiento contra un obstáculo se intenta, vuelve a la casilla anterior y la ejecución continúa, y termina al llegar al refugio (INC-56). Si no llega, el bloque donde se detuvo el avance queda resaltado por contorno y tamaño, la carretilla vuelve a la salida, la secuencia sigue en pantalla y la tablilla dice «La carretilla no llegó al refugio. Mira dónde se detuvo y corrige tu secuencia.». El resultado de la ejecución no tiene ninguna propiedad que nombre el bloque a corregir, y una prueba lo comprueba por reflexión (CP-06). «Ejecutar» responde a un clic simple, no a doble clic (PG-04), y el número de ejecuciones existe solo para el indicador docente: no limita nada.

En la escena, la pantalla se divide entre el laberinto y el panel de bloques, con el cajón «Bloques» cerrado al empezar. Cuando los bloques se acumulan se comprimen y una flecha despliega el que se edita; desde el 17 de septiembre, a partir de unos ocho bloques las filas conservan su alto y la lista se desplaza con dos botones, sin el componente de desplazamiento de uGUI, cuyo arrastre se pelearía con el de los bloques (RNF-02). El 23 de septiembre se añadieron una barra que se pulsa y no se arrastra, la cuadrícula dentro del seto y la papelera del bloque seleccionado (acta D08). El lado elegido de «Girar» lleva contorno además del tinte (INC-58, desde el 30/09), y los rótulos de los bloques salen de `N2_MazeLayout` (INC-104). El tablero encaja la ilustración entera y dejaba franjas del color del panel arriba y abajo; desde el 23 de septiembre el panel se pinta con el verde del borde del entorno, y la franja se lee como pradera.

Tres reglas de la lista entraron con el ejecutable del corte, el 01/10/2026. Mientras se arrastra un bloque ya enganchado, su fila se aparta de la lista, invisible pero activa, en lugar de reconstruirse, de modo que el soltar llega a ella: reordenar o retirar es un solo gesto, nada queda pegado al cursor, las ediciones se cuentan al soltar y un clic sin mover no cuenta ninguna. Al ejecutar, la lista se desplaza lo mínimo para que el bloque en curso se vea, con 8 px de margen y los mismos dos botones, y hace lo mismo con el bloque donde se detuvo un intento fallido (`SequenceListRules.RevealScroll`, en C# plano). Tras retirar un bloque queda seleccionado el que ocupa su lugar (`SequenceListRules.SelectionAfterRemoval`), así que la papelera sigue a mano aunque el cajón de bloques esté cerrado.

### 2.5 Cierre del nivel y el día contado con luz (W15 a W18)

En los indicadores (W15), `WheelIndicatorCollector` es un solo tipo para las tres fases, porque la mecánica de registro es la misma y lo que cambia por fase es qué cuenta como paso. Los tres controladores lo crean al empezar, lo publican en `GameFlowRunner.ActiveReporter` para que la pausa lo notifique y confirman la fase con sus cuatro indicadores. Ningún indicador llega a la interfaz del estudiante, y un barrido por reflexión sobre el assembly `Game.UI` lo comprueba sin añadir referencias de compilación (CP-03). La Tabla C.4 recoge la definición que aplica el juego, que OE1 y HU-10 describen desde el 29 de septiembre (INC-59).

**Tabla C.4.** Definición implementada de los cuatro indicadores del Nivel 2.

| Indicador | Fase 1 (bosque) | Fase 2 (taller) | Fase 3 (laberinto) |
|---|---|---|---|
| Intentos | Selecciones de un objeto que no es tronco redondo | Acciones rechazadas por estar fuera de secuencia; una pieza soltada lejos, el mazo incluido, no cuenta | Ejecuciones que no alcanzan el refugio |
| Errores corregidos | Un rechazo, o varios seguidos, que cierra la siguiente acción aceptada | Igual que en la fase 1 | Ediciones de la secuencia (enganchar, retirar, tomar, cambiar cuenta o lado) entre una ejecución fallida y la siguiente |
| Pasos utilizados | Se registra en cero | Acciones de ensamblaje en orden, seis con la cuerda | Bloques de la secuencia que llegó |
| Tiempo de resolución | Reloj menos la ventana de pausa | Igual | Igual |

En el resumen y el cierre reflexivo (W16), `LevelSummaryMessages` lleva el nivel al que pertenece y `LevelSummaryController` elige el asset del nivel recién jugado. El relato suma los intentos y las correcciones de las tres fases antes de elegir la variante de texto: sumar no pone ninguna cifra a la vista, porque el texto sigue siendo una línea fija del asset. El resumen del Nivel 2 dice «Descubriste que lo redondo rueda y que una carretilla se arma en orden.» y nombra la habilidad: «Eso se llama abstraer y pensar como un algoritmo: quedarte con lo que importa y ordenar los pasos.». La escena 2.5, en el refugio, cierra con dos líneas del guía: «Miraron muchas cosas y se quedaron solo con lo que importaba. Eso se llama abstraer.» y «Y después ordenaron los pasos antes de dar el primero: eso es pensar como un algoritmo.».

El menú de pausa (W17) dejó de vivir dentro de la escena del Nivel 1 y pasó a ser el prefab `MenuPausa.prefab`, hoy en las cinco escenas jugables. Sigue el mockup 6, con «Reanudar», «Reiniciar» y «Volver al menú de niveles» (INC-49, documentos corregidos el 29/09); el tercero sale a la selección de niveles y no al inicio. Mientras está abierto, `Time.timeScale` vale 0, porque HU-17 pide que el nivel quede detenido y sin eso la ejecución paso a paso seguía corriendo; vuelve a 1 al reanudar, al salir y cuando la escena se destruye al reiniciar. «Reiniciar» repite la fase activa y nunca vuelve a bloquear ni descarta lo confirmado (INC-25). El 17 de septiembre la pausa recibió las tipografías, los recuadros y los glifos del mockup; los tres glifos son de Phosphor Icons y se acreditan en los créditos (INC-127; Anexo E).

En el doble indicador y el contraste (W18), los tres estados de rechazo del nivel llevan color y un segundo indicador (`WheelLevel_RNF19_LosTresEstadosDeErrorSeLeenSinColor`). El contraste se mide con la fórmula de luminancia relativa contra los colores realmente aplicados en la escena, y esa medición encontró dos fallos reales el 15 de septiembre: el contador del bosque se pintaba sobre el pasto sin tablilla, y la cara de «Ejecutar», en el naranja de Algoritm (`#E2571F`), daba 4,07:1 con el texto oscuro. El contador pasó a colgar de una tablilla marfil y «Ejecutar» usa el ámbar de acción primaria (`#E8A33D`). Desde el 1 de octubre (D10-5), `WheelLevel_RNF20_ContrasteSuficienteEnLasTresEscenas` exige además que cada texto vaya sobre su marco y no directamente sobre la ilustración, como la prueba del río; la aserción nueva se puso en rojo con dos mutaciones antes de pasar. Ninguna animación del nivel parpadea, muestreado cuadro a cuadro (RNF-21).

Dos recorridos automatizados (`WheelLevel_RNF13_RecorreElNivel2CompletoHastaElMenuConNivel3Desbloqueado`, primero y segundo) juegan el nivel desde `Boot` con un perfil real, leen las narrativas línea a línea y comprueban los indicadores en disco. Desde el 29 de septiembre exigen que, al llegar a la escena 2.5, el Nivel 3 siga bloqueado y no se ofrezca «Omitir», porque desbloquear al llegar al refugio daría por vista la escena la primera vez; una mutación del controlador del laberinto que desbloqueaba antes de tiempo lo puso en rojo. El mismo día `NarrativeSceneController.Leave()` pasó a salir una sola vez: un doble clic en «Continuar» al salir del cierre reflexivo llegaba dos veces y el segundo dejaba el flujo en el menú, saltando el resumen donde se nombra la habilidad (RF-12, RF-45).

El nivel transcurre en un día y lo cuenta la luz (23/09, INC-71; acta D08), sin dibujos nuevos. `N2_PuenteI` amanece azulado, `N2_PuenteI_Bosque`, la 2.1, la 2.2 y la 2.3 transcurren de tarde sin tinte, la 2.4 es un atardecer y la 2.5 es de noche junto al fuego, en el refugio. Para cambiar de fondo a mitad de escena el puente se partió en dos assets encadenados: el primero sobre la ilustración de apertura del Nivel 1 y el segundo sobre el claro del bosque. En el laberinto, que no tiene capa de luz, `MazeLayout.LightTint` tiñe el entorno de atardecer sin cambiar la piel de los personajes. Lo vigilan `NarrativeSequence_RF05_ElNivel2TranscurreDelAmanecerALaNoche` y `MazeScene_RF30_ElLaberintoEsAlAtardecer`. Al pasar de una narrativa a una mecánica, o al revés, `SceneLoader` funde a negro y de vuelta, 0,4 s por mitad; los menús cortan en seco.

## 3. Decisiones de diseño y hallazgos de consistencia

Los dos carriles corrieron a la vez desde el 10/09. El plan declaraba que el slice no podía empezar antes de cerrar el punto de control D del Slice 1. Revisado tarea por tarea, resultó cierto solo para W15, W16 y W17, que esperaban la interfaz de los indicadores y el menú de pausa del Slice 1, y ese código llegó el 11 de septiembre; W02 escribió primero sobre `PlayerProfile` y la tarea de indicadores del Slice 1 se apoyó en su interfaz. El reparto se hizo por módulo, la única forma de que dos carriles trabajando a la vez no se pisen: el Slice 1 tocaba `Game.Levels.Fire`, `Game.Core`, las escenas del Nivel 1 y el ejecutable, y el Slice 2, `Game.Levels.Wheel` y `Game.Scaffolding`. Los cruces se declararon en la guía del repositorio, y el 11 de septiembre un commit reconcilió el cierre del Slice 1 con la rama del Slice 2.

Las demás decisiones que dieron forma al nivel fueron de Santiago Benavides Rey o de las actas: el menú de pausa del mockup 6, con «Reiniciar» entre las otras dos opciones (15/09); la salida de «Empujar» hacia la narrativa (15/09, acta D06); el mazo como segundo camino para perforar (14/09); la cuerda como séptima pieza (25/09); el día contado con luz (acta D08); los troncos que ruedan en el motor (acta D09), y las dos correcciones del acta D10. La regla de la sesión D05 de mover el objeto y no el encuadre rige todas las narrativas del nivel, y la prueba `NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo` la comprueba parada por parada sobre las siete secuencias.

**Tabla C.5.** Hallazgos de consistencia del incremento.

| INC | Qué decidió | Documentos corregidos o implementación | Fecha de cierre |
|---|---|---|---|
| INC-33 | Los bloques se leen respecto de la orientación de la carretilla | RF-31 y guion 1.6.3.2 | 30/08/2026 |
| INC-49 | Pausa del mockup 6: «Reanudar», «Reiniciar» y «Volver al menú de niveles» | RF-07, HU-17, OE1 3.6.1 y arquitectura | 29/09/2026 |
| INC-50 | «Empujar» confirma la fase y la escena 2.2 muestra el rodado | RF-26, guion 1.6.1.2, CU-06 y HU-08 | 29/09/2026 |
| INC-54 | La cuerda es la séptima pieza y amarrar la caja, el último paso | Guion 1.6.2.1 y 1.6.2.2, RF-27, RF-29, HU-09 y CU-07; cuerda en la 2.3 | 29/09/2026 |
| INC-55 | Bloques con cuenta de 1 a 9 y giro a los dos lados; se enganchan donde se sueltan | RF-31, guion 1.6.3.2, CU-08 y HU-10 | 29/09/2026 |
| INC-56 | La ejecución sigue tras un tropiezo y termina en el refugio | Guion 1.6.3.2, CU-08, HU-10, HU-05 y RF-11 | 29/09/2026 |
| INC-57 | Seto con arbustos y trazado al azar con camino garantizado | RF-30, guion 1.6.3.2, HU-10 e índice de sprites | 29/09/2026 |
| INC-58 | El lado de «Girar» y «Empujar» deshabilitado no dependen solo del color | Contorno y candado implementados; caso PF-RNF19-01 del OE4 | 29/09/2026 |
| INC-59 | Los indicadores del Nivel 2 se definen como los cuenta el juego | OE1 3.6.1 y HU-10 | 29/09/2026 |
| INC-60 | Reacción de los objetos al cursor, troncos que se alinean solos y caja soltada fuera | Guion 1.6.1.2, CU-06, HU-08 y RF-25 | 29/09/2026 |
| INC-61 | El mazo perfora como «Mecanizar»; candado con «Aún no»; tronco ya perforado | Guion 1.6.2.2, RF-28, CU-07 y HU-09 | 29/09/2026 |
| INC-64 | Las pistas del guía del Nivel 2 entran al guion | Guion 1.6.1.2, 1.6.2.2, 1.6.3.2 y 1.11 | 29/09/2026 |
| INC-71 | El Nivel 2 dura un día y lo cuenta la luz | Guion 1.6 y 1.7, dirección de arte y especificación | 29/09/2026 |
| INC-104 | Ningún texto visible queda escrito en el código | Rótulos de los bloques en `N2_MazeLayout` | 29/09/2026 |
| INC-115 | Soltar el mazo lejos no cuenta como intento | Implementado; OE1 3.6.1, guion 1.6.2.2, CU-07 y HU-09 | 30/09/2026 |
| INC-120 | La 2.2 abre con la caja que deja el bosque | Implementado (D10-2); cámara del Nivel 2, dirección de arte e índice de sprites | 01/10/2026 |
| INC-121 | Al amarrar la cuerda, la carretilla pasa al dibujo con que abre la 2.4 | Implementado (D10-2); índice de sprites, dirección de arte y cámara del Nivel 2 | 01/10/2026 |

## 4. Verificación

### 4.1 Pruebas que lo nombran

El código del nivel ocupa 24 archivos y 5 433 líneas en `Game.Levels.Wheel`, el módulo más extenso del prototipo, y sus pruebas, 17 archivos. En el corte, `Game.Levels.Wheel.Tests` tiene 78 métodos, que la corrida final ejecutó como 81 casos, y `Game.Levels.Wheel.PlayMode.Tests`, 98 métodos y 99 casos. Cada requerimiento de RF-22 a RF-34 tiene al menos una prueba que lo nombra; RF-27 tiene una sola. La Tabla C.6 muestra una representativa de cada uno, y la matriz completa está en el Anexo A.

**Tabla C.6.** Requerimientos del slice y una prueba representativa de cada uno.

| RF | Nombre en la Solución OE1 | Prueba representativa |
|---|---|---|
| RF-22 | Escenario de exploración | `ForestScene_RF22_ElObjetoSeApartaAlAcercarseElCursor` |
| RF-23 | Selección de objetos por patrón | `PatternSelection_RF23_AceptaTroncoRedondoYRechazaDistractor` |
| RF-24 | Contador de recolección | `ForestScene_RF24_ElContadorEsVisibleDuranteTodaLaFase` |
| RF-25 | Colocación de la carga | `ForestScene_RF25_LaCajaNoSeArrastraSinLosCincoTroncos` |
| RF-26 | Demostración del rodado | `ForestScene_RF26_EmpujarNoAnimaElRodadoEnLaMecanicaSinoQueSaleALaNarrativa` |
| RF-27 | Área de trabajo de construcción | `WorkshopScene_RF27_PresentaLasSeisPiezas`, la única que lo nombra |
| RF-28 | Acción de mecanizado | `WorkshopScene_RF28_ElMazoArrastradoSobreElTroncoResaltadoLoPerforaIgualQueElBoton` |
| RF-29 | Ensamblaje secuencial de la carretilla | `AssemblySequence_RF29_RechazaCadaPasoFueraDeSecuenciaConSuMensaje` |
| RF-30 | Escenario de laberinto | `MazeGrid_RF30_ElAnilloExteriorEsElSetoSalvoLaSalidaYElRefugio` |
| RF-31 | Editor de bloques de instrucciones | `CartState_RF31_AvanzarEsRelativoALaOrientacionNoAbsoluto` |
| RF-32 | Ejecución de la secuencia | `SequenceExecutor_RF32_RecorreLaSecuenciaPasoAPasoResaltandoElBloqueEnCurso` |
| RF-33 | Validación por retroceso | `SequenceExecutor_RF33_UnMovimientoInvalidoRetrocedeYLaEjecucionContinua` |
| RF-34 | Edición y reintento de la secuencia | `MazeScene_RF34_TrasUnaEjecucionFallidaLaSecuenciaPermaneceEnPantalla` |

La prueba de RF-27 conserva «Seis» en su nombre de cuando el taller tenía seis piezas, aunque compara contra las siete. Las pruebas que cierran el nivel desde el 29 de septiembre se recogen en la Tabla C.7, con el rojo que justificó cada una; las que vigilan una guarda nacieron en verde y se validaron con una mutación del código, revertida después.

**Tabla C.7.** Pruebas del Nivel 2 añadidas o ajustadas del 29 de septiembre al 1 de octubre de 2026.

| Prueba | Qué fija | Origen y rojo previo |
|---|---|---|
| `WheelLevel_RNF13_RecorreElNivel2CompletoHastaElMenuConNivel3Desbloqueado` | La 2.5 se ve con el Nivel 3 bloqueado y sin «Omitir» | `d105838`; mutación que desbloqueaba al llegar al refugio |
| `LevelSummary_RF45_DobleClicEnContinuarMientrasCargaElResumenSeQuedaEnElResumen` | Un doble clic al salir de la 2.5 no salta el resumen | `d105838`; el segundo clic llevaba al menú |
| `ForestScene_RNF19_EmpujarDeshabilitadoLlevaCandado` | «Empujar» deshabilitado lleva candado | `37b3cb7`, INC-58 |
| `MazeScene_RNF19_ElLadoElegidoDeGirarSeDistingueSinColor` | El lado de «Girar» lleva contorno | `37b3cb7`, INC-58 |
| `WorkshopScene_RF29_SoltarElMazoLejosNoCuentaComoIntento` | El mazo soltado lejos no suma intento | `37b3cb7`, INC-115 |
| `Content_INC54_LaEscena23PintaTodasLasPiezasDelTaller` | La 2.3 pinta la cuerda | `37b3cb7`, INC-54 |
| `PauseMenu_HU17_OfreceExactamenteReanudarReiniciarYVolverAlMenuDeNiveles` | Los tres rótulos del mockup 6 | `37b3cb7`, INC-49 |
| `WheelLevelConfig_RF05_LaCajaColocadaEsLaQueLaEscena22DibujaAlAbrir` | La caja de la 2.2 es la que deja el bosque | D10-2; la caja estaba a 0,545 frente a 0,5089 |
| `AssemblyContent_INC54_LaCarretillaAmarradaEsElDibujoConElQueAbreLaEscena24` | El dibujo amarrado es el de la 2.4 | D10-2; el campo estaba vacío |
| `WorkshopScene_INC54_AlAmarrarLaCuerdaLaCarretillaPasaAlDibujoConCuerda` | Al amarrar, la carretilla cambia de dibujo | D10-2; seguía el cuarto dibujo |
| `WheelLevel_RNF20_ContrasteSuficienteEnLasTresEscenas` | Cada texto va sobre su marco | D10-5; dos mutaciones que colgaban el mensaje del entorno |
| `NarrativeScene_RF05_CapturaDelPrimerCuadroDeLaEscena22` | Captura del primer cuadro de la 2.2 | 01/10; verificación visual |
| `MazeScene_RF34_ReordenarUnBloqueEsUnSoloGestoYNoQuedaPegado` | Reordenar es un solo gesto y nada sigue al cursor | Ejecutable del corte; el bloque quedaba pegado al cursor |
| `MazeScene_RF34_RetirarYReordenarArrastrandoCuentanSoloLasEdicionesHechas` y `MazeScene_RF34_UnClicSinMoverSobreUnBloqueLoDejaDondeEstabaYNoCuentaEdicion` | El indicador de errores corregidos cuenta solo lo que el estudiante editó | Ejecutable del corte; el bloque pegado fabricaba ediciones |
| `MazeScene_RF32_AlEjecutarLaListaMuestraElBloqueEnCurso` | Con la lista desplazada, el bloque en curso se ve | Ejecutable del corte; se ejecutaba fuera de la vista |
| `MazeScene_RF34_TrasLaPapeleraConElCajonCerradoLasFilasLaOfrecen` y `MazeScene_RF34_TrasRetirarArrastrandoLasFilasOfrecenLaPapelera` | Tras retirar, la papelera sigue a mano | Ejecutable del corte; ninguna fila la ofrecía con el cajón cerrado |
| `SequenceListRules_RF32_*` y `SequenceListRules_RF34_*` (EditMode) | Desplazamiento mínimo de la lista y fila seleccionada tras retirar, en C# plano | Ejecutable del corte |

### 4.2 Corridas declaradas

Hasta el 14 de septiembre las corridas del slice se hicieron por línea de comandos con el Editor cerrado. El 15 se usó un script de Editor efímero que registraba los resultados del corredor contra el Editor abierto y los escribía en un archivo, y se borró al terminar; todas las cifras salen de un archivo de resultados real.

**Tabla C.8.** Corridas del incremento.

| Fecha | Corte | EditMode | PlayMode | Observaciones |
|---|---|---|---|---|
| 10/09/2026 | Punto de control W-A | 63 de 63 | Sin corrida declarada | Exclusión de RNF-16 en verde con dos niveles |
| 10/09/2026 | Punto de control W-B | 77 de 77 | 34 de 36 (2 omitidas) | Las omitidas son de verificación visual |
| 15/09/2026 | Punto de control W-F | 212 de 212 | 146 de 148 (2 expectativas actualizadas) | Dos expectativas que el propio slice había invalidado se actualizaron y pasaron: 148 de 148 |

Las dos expectativas eran la lista de lo que atiende el clic sostenido en el bosque, que el prefab de pausa amplió, y la secuencia con que abre el Nivel 2, que pasó a ser la escena puente. Las seis capturas de verificación visual (los tres estados de error, el bloque detenido, el rodado y la ejecución paso a paso) se revisaron a mano. Las corridas completas posteriores se presentan en el Anexo G, apartado 4.2, y la Tabla C.9 sigue en ellas los casos de los dos assemblies del nivel.

**Tabla C.9.** Casos de los assemblies de prueba del Nivel 2 en cada corte.

| Assembly | 15/09 (cierre) | 25/09 (`ccf77e6`) | 30/09 (`359365e`) | 01/10 | Corte |
|---|---|---|---|---|---|
| `Game.Levels.Wheel.Tests` | 66 | 69 | 69 | 71 | 81 |
| `Game.Levels.Wheel.PlayMode.Tests` | 60 | 89 | 92 | 93 | 99 |

Para las casillas del punto de control W-F que pedían el ejecutable sirvieron los dos candidatos del 01/10/2026, de los cuales se entrega el segundo, el ejecutable del corte. La peor carga medida entre los dos fue de 0,05 s en `Level2_Forest`, 0,09 s en `Level2_Workshop` y 0,09 s en `Level2_Maze`; la memoria de trabajo máxima del proceso del corte, 446 MB, y la carpeta entregable, 479,0 MB. Los tres recorridos completos terminaron sin incidencias bloqueantes. Los dos primeros, sobre el primer candidato, vieron dos defectos del laberinto: un bloque tomado de «Tu secuencia» se quedaba pegado al cursor (RF-34) y, con la lista desplazada, el bloque en curso se ejecutaba fuera de la vista (RF-32). El ejecutable del corte los corrige (apartado 2.4): sobre él la evaluación funcional del OE4 repitió entera la sesión del laberinto, reordenó y retiró bloques en un solo gesto sin que nada siguiera al cursor, vio resaltado el bloque en curso con nueve y con diez bloques en la lista, vació la secuencia con tres papeleras seguidas y el cajón cerrado y comprobó en el perfil que las 40 ediciones de la fase eran todas del jugador. La exclusión de RNF-16 se ejecutó además retirando físicamente cada nivel en una copia del proyecto con el árbol del primer candidato; el del corte no cambia ninguna definición de assembly. Sin `Game.Levels.Wheel` ni sus pruebas, y con sus tres escenas fuera de la configuración de compilación, el proyecto compiló sin errores y pasaron 354 de 360 casos de EditMode y 245 de 271 de PlayMode; con el nivel restituido y el Nivel 1 retirado, 376 de 382 y 281 de 305. Sin el Nivel 2 fallaron, como se preveía, las seis pruebas transversales que piden por su nombre `Level2_Forest`, `Level2_Workshop` o `Level2_Maze`. En la otra copia, entre los fallos de entorno, figuran tres pruebas de disposición del propio Nivel 2, que no caben en la vista de 640 × 480 de la corrida por lotes y fallan igual en el árbol completo. Los demás fallos de las dos copias exigen el repositorio con sus tres niveles o se deben a ese mismo entorno, y ninguno apunta a una dependencia entre niveles. El método y las doce escenas se presentan en el documento principal, apartados 8.3 y 9.5.

## 5. Archivos principales

**Tabla C.10.** Carpetas y archivos del incremento.

| Carpeta o archivo | Qué es |
|---|---|
| `Assets/Game/Scripts/Runtime/Levels/Wheel/` | Lógica en C# plano (`PatternSelection`, `CargoPlacement`, `AssemblySequence`, `MazeGrid`, `CartState`, `BlockSequence`, `SequenceExecutor`, `SequenceListRules`, `WheelIndicatorCollector`), contenido (`WheelLevelConfig`, `AssemblyContent`, `MazeLayout`) y adaptadores de las tres escenas (`ForestSceneController`, `WorkshopSceneController`, `MazeSceneController`, `ForestObjectNudge`, `CargoHandle`, `ClickRelay`); `WheelSounds` es del carril de sonido |
| `Assets/Game/Scripts/Runtime/Core/` | `PhaseId` y los cambios de `PlayerProfile`, `LevelUnlockPolicy`, `GameFlow` y `GameFlowRunner` para un nivel de tres fases |
| `Assets/Game/Scripts/Runtime/Scaffolding/` | `HintPolicy` por fase y el motor de puesta en escena: `IllustrationFraming`, `NarrativeProp`, `RollMotion`, `RollingLog` y `RollingLogLook` |
| `Assets/Game/Scripts/Runtime/UI/` | `PauseMenuController`, `LevelSummaryController` y `LevelSummaryComposer` con mensajes por nivel, y `LevelSelectController` con la secuencia de apertura por ficha |
| `Assets/Game/Data/Wheel/` | `N2_WheelLevelConfig`, `N2_AssemblyContent`, `N2_MazeLayout`, `N2_ResumenNivel`, `N2_TroncoRodante` y `N2_Sonidos` |
| `Assets/Game/Data/Guide/N2_Guia.asset` y `Assets/Game/Data/Narrative/N2_*.asset` | Las tres tareas del guía y las siete secuencias del nivel |
| `Assets/Game/Scenes/` | `Level2_Forest`, `Level2_Workshop` y `Level2_Maze` |
| `Assets/Game/Prefabs/UI/MenuPausa.prefab` | Menú de pausa de las cinco escenas jugables |
| `Assets/Tests/EditMode/Levels/Wheel/` y `Assets/Tests/PlayMode/Levels/Wheel/` | Pruebas del nivel; los recorridos completos están en `WheelLevelJourneyTests` |
| `claudeDocs/tasks/Slice 2/` | `plan.md`, `todo.md` y `Slice-2-Resultados.md` |
| `claudeDocs/Camara_Narrativa_N2.md` | Diseño de los 50 encuadres del Nivel 2, validados contra 16:9 |

## 6. Evaluación del OE4 y verificaciones a cargo de Santiago Benavides Rey

Del tablero de este slice quedan abiertas por diseño dos casillas, ambas de PG-05: comprobar que el paso del panel de clics del Nivel 1 al arrastre del Nivel 2 no confunde a los estudiantes. Es una observación de la sesión con estudiantes, no una aserción, y el acta D10 (apartado 4) la deja a cargo de Santiago Benavides Rey, como guion H1 de la hoja de verificaciones manuales del OE4, precedida del consentimiento firmado de los acudientes (RNF-12). Las casillas se marcan cuando entregue los resultados y no pasan a trabajos futuros. La ejecución en un segundo equipo, con la red apagada y en un equipo sin tarjeta gráfica dedicada (guiones H3 a H5) cubre también este nivel.

En el carril de sonido sigue faltando el ambiente nocturno del Nivel 2: la escena 2.5 y el puente al Nivel 3 suenan con el ambiente de la cueva oscura del Nivel 1 (Anexo E, apartado 6). El estado de la tarjeta D09-4, la fogata del cierre del nivel a cargo de Santiago Valdiri García, no se revisó en el acta D10 y se confirma con él; la escena 2.5 tiene su fuego desde el 23 de septiembre. Los tres defectos del laberinto que halló la primera pasada de pruebas funcionales en el primer candidato, el bloque pegado al cursor, el bloque en curso fuera de la vista y la papelera que no volvía con el cajón cerrado, están corregidos en el ejecutable del corte (apartado 2.4). Quedan cuatro observaciones menores, documentadas con su arreglo y sin corregir en este corte: bajar un bloque una sola posición, soltándolo en la mitad de abajo del que subió, no hace nada; un doble clic en la papelera retira dos bloques; tras una ejecución fallida la lista queda donde deja ver el bloque donde se detuvo, y su papelera cambia de sitio, y la flecha de página del panel de perfiles se oculta donde las del laberinto se apagan (documento principal, apartado 10.4). La deuda técnica menor del nivel, como el nombre de la prueba de RF-27 o dos comentarios que quedaron atrás en el código, se recoge en el documento principal, apartado 10.4.
