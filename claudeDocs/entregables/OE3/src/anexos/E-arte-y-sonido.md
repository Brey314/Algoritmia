# Carril de arte y sonido

Este anexo describe el carril que incorporó al prototipo el arte y el sonido definitivos: los entornos, los objetos, los efectos y la interfaz de los tres niveles; las reglas con que el motor importa cada imagen y cada pieza de sonido; las diecinueve piezas de sonido y dónde suena cada una, y la verificación del arte por programa. El carril no es un incremento como los slices: corrió en paralelo a ellos desde la vinculación de la colaboradora gráfica el 2 de septiembre de 2026 y tocó los tres niveles. Su contenido refleja el estado del carril en el corte del 1 de octubre de 2026 y sale del documento de resultados del carril, que el repositorio conserva como registro histórico con sus notas fechadas, del índice de sprites (`Assets/Game/Art/Inventario.md`), de los documentos de dirección de arte y de dirección de música y sonido, del registro de inconsistencias en su revisión 16 y de las actas D01 a D10. Sirve a los apartados 7.2, 7.3 y 7.4 y al capítulo 8 del documento principal.

## 1. Alcance y seguimiento

El carril cubre los recursos gráficos y sonoros del juego y su integración en el motor. Su trabajo vive en `Assets/Game/Art/`, `Assets/Game/Audio/` y el assembly `Game.Audio`, y alcanzó los módulos de los tres niveles, `Game.Levels.Fire` y `Game.Levels.Wheel` desde el 21/09/2026 y `Game.Levels.River` desde el 25/09/2026, además de piezas compartidas de `Game.Core`, `Game.Scaffolding` y `Game.UI`. Los requisitos que lo gobiernan son de presentación y de recursos: RNF-19 (segundo indicador además del color), RNF-20 (contraste de 4,5:1), RNF-21 (sin destellos), RNF-23 (recursos propios o autorizados, acreditados en los créditos) y, por su peso, RNF-05 y RNF-06. A ellos se suma el criterio pedagógico CP-02, que en el sonido significa que ningún intento sin éxito suena a fallo. Los personajes animados se trabajaron aparte y se describen en el Anexo F.

Tres personas llevaron el carril. Sofía Valentina Giraldo Segovia se vinculó el 02/09/2026 como colaboradora contratada para la producción y la asesoría gráfica, con revisiones semanales los miércoles a la 1:00 p. m.; produjo los entornos y los objetos definitivos, mientras las decisiones de diseño y de mecánica quedaban en el equipo de estudiantes. Santiago Valdiri García reunió el conjunto de sonidos del juego entre el 13 y el 21/09/2026 (tarjetas D05-2 y D07-2). Santiago Benavides Rey integró en el motor el arte, desde la llegada de los entornos, y el sonido, desde el 21/09/2026; desde el acta D08 quedó a cargo de todo el sonido restante. El carril no tuvo puntos de control propios: su avance se revisó en las actas.

**Tabla E.1.** Actas que gobernaron el carril.

| Acta | Fecha | Qué decidió para el carril |
|---|---|---|
| D01 | 02/09/2026 | Vinculación de la colaboradora y entrega de la dirección de arte. Ningún recurso introduce mecánicas; todo estado de color lleva un segundo indicador; sin destellos; contraste legible en proyector. Revisiones de los miércoles |
| D03 | 06/09/2026 | Se adoptan las panorámicas de los Niveles 1 y 2 con correcciones; la vista cenital se reorienta al laberinto; los tonos del fuego quedan reservados al fuego |
| D04 | 09/09/2026 | La luz del Nivel 1 se resuelve con una pieza base y una máscara, no con cuatro entornos; se ratifica la nomenclatura de la dirección de arte |
| D05 | 13/09/2026 | Santiago Valdiri García toma el sonido (tarjeta D05-2) |
| D06 | 15/09/2026 | Recepción de los entornos definitivos, con prefijo `env_`; los objetos entran sustituyendo el archivo y conservando el nombre; ninguna animación de salto, caída ni derrota |
| D07 | 19/09/2026 | La balsa del Nivel 3 se compone en el motor con ocho piezas; el sonido no puede tener efectos de derrota, error ni castigo |
| D08 | 23/09/2026 | Aprueba el sonido del Nivel 2, el día contado por la luz y los fundidos; el sonido restante pasa a Santiago Benavides Rey (tarjetas D07-2 y D08-2) |
| D09 | 24/09/2026 | Los troncos del Nivel 2 se animan en el motor (tarjeta D09-1); el tronco y los amarres de la balsa pasan a piezas separadas |
| D10 | 30/09/2026 | Chispa del Nivel 1 en un solo rayo, renombres, crédito de los glifos de pausa, limpieza de halos y verificación de todos los sprites (tarjeta D10-4); sonido del hundimiento y de la escena final; acepta como definitivos el conjunto genérico de interfaz, el anillo de la zona, la balsa hundida y los iconos de los indicadores del informe docente |

Las tarjetas del tablero siguen el carril de acta en acta y se rastrean por su texto, porque cada una se renumera con la serie del acta que la reabre. La D06-2, incorporar los entornos definitivos y poner al día el índice de sprites, continuó en la D10-4 con los cuatro entornos que conservaban el prefijo anterior. La D07-2, cerrar la implementación de sonidos, pasó de Santiago Valdiri García a Santiago Benavides Rey en el acta D08; el hundimiento de la balsa y el ambiente de la escena final entraron con la tarjeta D10-3, y lo que sigue abierto de ella se declara en el apartado 6. La D08-2, implementar los sprites y sonidos que se reciban sustituyendo archivos sin tocar escenas, continuó en la D10-4 y en la revisión 5 del historial de la dirección de sonido. La D09-1, animar los troncos del Nivel 2 en el motor, se dio por cumplida en el acta D10. El acta D10 no revisó las tarjetas D06-1, D07-3, D09-2, D09-3 y D09-4, que dependen de la colaboradora o de Santiago Valdiri García.

**Tabla E.2.** Commits del carril.

| Fecha | Commit | Tarjeta citada | Qué incorporó |
|---|---|---|---|
| 13 a 21/09/2026 | `cd561fc`, `6790c6b`, `3403483`, `83db0f2`, `022023a` (solicitudes n.º 79 y 81) | Ninguna | El conjunto de sonidos, organizado por nivel y global |
| 16/09/2026 | `a9236f1` | Ninguna | Regla de importación del arte sin comprimir |
| 21/09/2026 | `2cbe287` | D05-2, D07-2 | Gestor de audio, sonido del Nivel 1, regla de importación del sonido, piezas renombradas |
| 21/09/2026 | `dc51804` | Ninguna | Clips del fuego por cuadros |
| 22/09/2026 | `aca7acc`, `8ec5120` | D07-2 | Piezas, montón y fuego del Nivel 1; quemado desde el centro |
| 23/09/2026 | `03675b9`, `d7ace67` | D05-2, D07-2 | Sonido del Nivel 2; el día del Nivel 2 por la luz; fundidos; laberinto |
| 24/09/2026 | `b38c00b` | Campo sin rellenar | Troncos que ruedan en el motor |
| 24/09/2026 | `f801186` | D08-2 | Humo animado sobre cada fuego de las narrativas |
| 25/09/2026 | `1d5ce58`, `d3a6cc9` | Campo sin rellenar | Cuerda del taller, seto del laberinto; hoguera animada del horizonte y del final |
| 25/09/2026 | `ccf77e6` | D08-2 | Sonido del Nivel 3 |
| 25/09/2026 | `44fd479` | Campo sin rellenar | Objetos definitivos de los Niveles 2 y 3 y balsa en tres cuartos |
| 30/09/2026 | `37b3cb7` | Ninguna (cierre de INC-46 a INC-117) | Arte real en los menús, humo al converger, chispa visible |
| 01/10/2026 | árbol de trabajo | D10-1 a D10-4 | Rayo de la chispa, carretilla amarrada, sonido del hundimiento y de la escena final, renombres, halos y créditos |

La rama `feat/implementación-de-props-y-sonidos` partió de la rama principal el 21/09/2026 y se integró en tres solicitudes: la n.º 82 (23/09/2026), la n.º 87 (29/09/2026), que llevó los cinco commits del 24 y el 25/09 y `44fd479`, y la n.º 88 (30/09/2026). De los commits de la tabla, no citan tarjeta los del conjunto de sonidos, `a9236f1`, `dc51804` y `37b3cb7`, y `b38c00b`, `1d5ce58`, `d3a6cc9` y `44fd479` dejaron el campo sin rellenar. El primero de esos cuatro corresponde por su texto a la tarjeta D09-1. La práctica de asociar cada commit a su tarjeta (CT-11) se evalúa en el apartado 2.5 del documento principal.

## 2. Lo implementado

### 2.1 Arte: entornos, objetos, efectos e interfaz

**Entornos.** El juego usa nueve archivos de entorno, todos con prefijo `env_` (Tabla E.7). La colaboradora los entregó para el acta D06 (15/09/2026): los de pantalla a 1920 × 1080 y las panorámicas del Nivel 1 y del bosque del Nivel 2 a 3840 × 1080, en la misma composición que las versiones de trabajo, de modo que ningún encuadre cambió de valor, porque los encuadres se expresan en fracciones del lienzo. Las panorámicas duplican el lienzo y tienen una costura en el centro que ningún encuadre debe cruzar; lo avisa la propia secuencia narrativa al validarse. Dos entornos que llegaron con nombre libre se renombraron desde el motor el 16/09/2026, conservando su identificador interno (GUID): el río pasó a `env_n3_rio` y el poblado de la escena final a `env_final_fogatas`. El horizonte del puente II, `env_enlace_n2`, sustituyó a una ilustración anterior del Nivel 2 conservando su archivo de metadatos, y el 25/09/2026 se reemplazó por una versión sin la hoguera central pintada, que pasó a ser un objeto animado. Los cuatro entornos que conservaban el prefijo antiguo `entorno_` se renombraron el 01/10/2026 (INC-126). El acta D04 resolvió la luz del Nivel 1 con una pieza base y una capa de oscuridad que la multiplica, en lugar de cuatro entornos por estado; esa capa es el shader `fx_oscuridad`. El Nivel 2 dura un día y lo cuenta la luz aplicada sobre la ilustración, sin dibujos nuevos por hora (acta D08). En el Nivel 3, todo se juega y se narra sobre un único plano del río a ras de suelo, con perspectiva por profundidad (INC-66).

**Objetos del Nivel 1.** La hoja, el sílex, el pedernal y los dos montones de hojas (de frente, para las narrativas, y desde arriba, para la mecánica) son imágenes de 2000 × 2000. La llama se anima por cuadros. La entrega llegó exportada a 30 cuadros por segundo pero dibujada a seises o a ochos, de modo que muchos archivos repetían el mismo dibujo; el 21/09/2026 cada clip se construyó con un solo cambio de sprite por dibujo distinto y se borraron los 215 cuadros duplicados. De 249 archivos quedaron 34 dibujos, 13 para la llama de frente (2,667 s en bucle) y 21 para la cenital (5,633 s), y las texturas cargadas bajaron de 1 469 MB a 235 MB. Cada clip es un `.anim` con una curva que cambia el sprite de una `Image` y su propio controlador de animación; los cuadros conservan su nombre de entrega, porque los referencia esa curva. El humo siguió la misma regla: la entrega de 284 cuadros quedó en 33 dibujos, recortados al mismo rectángulo de 1123 × 1933 para que el dibujo no salte de un cuadro a otro, en dos clips encadenados, el nacimiento (1,97 s, 7 dibujos) y el bucle (7,27 s, 26 dibujos). Desde el 24/09/2026 cada llama de las narrativas lleva su humo detrás, a 0,6 de su escala. El montón se quema desde el centro hacia afuera con una máscara circular generada en memoria (`BurnReveal`, 22/09/2026), y las piezas aparecen repartidas al azar fuera de la interfaz. La chispa del golpe no tiene archivo: es un solo rayo `#FFE9A8` de 8 px a 1080p que dibuja el motor, desde el 01/10/2026 en una dirección al azar y con un único barrido (INC-119). Su comportamiento en la mecánica, con el humo que nace al converger, se describe en el Anexo B, apartado 2.5.

**Objetos del Nivel 2.** Los objetos definitivos del Nivel 2 llegaron el 25/09/2026 y bajaron de resolución según cómo se ven: la regla es 256 × 256, y se sube a 512 × 512 solo donde el objeto se muestra a más de 256 px a 1080p, medido sobre los encuadres de los assets. Así quedaron a 512 el eje y la tabla del taller y los estados de la carretilla del `e2` al `e5`, que se sustituyen en el mismo sitio, y a 256 los demás objetos, salvo la textura del tronco. La reducción se hizo sobre alfa premultiplicado para que el borde no se oscureciera. El tronco es uno solo, `prop_n2_tronco_a`, y rueda en el motor: el componente `RollingLog` lo dibuja como un cilindro en tres cuartos a partir de una sola textura de 768 × 256 con la corteza desenrollada y el corte, sin cámara ni modelo tridimensional, y lee el giro que ya le dan el cursor en el bosque o el movimiento en las narrativas (acta D09, tarjeta D09-1). Su vista vive en `N2_TroncoRodante.asset`. Los distractores (cuatro piedras, tres plantas y tres herramientas) se distinguen del tronco por su silueta y por no llevar el tono de madera trabajada, que es exclusivo de los troncos. La caja de alimentos llena es la que usa la escena 2.2 desde el 01/10/2026 (INC-120), y la caja vacía quedó en disco sin uso. Al amarrar la cuerda en el taller, la carretilla pasa al dibujo amarrado `e5`, el mismo de la escena 2.4 (INC-121). El laberinto usa una carretilla cenital, un arbusto dibujado a 1,65 veces su casilla para que los contiguos se lean como seto, y un shader de contraste y saturación (`fx_contraste`) que mantiene la lectura al atardecer. Los cambios de la escena 2.2 y del taller se describen en el Anexo C, apartados 2.2 y 2.3.

**Objetos del Nivel 3.** Los cuatro materiales de la orilla (tronco, sogas, tela y mástil) son definitivos desde el 25/09/2026 y hacen también de icono del inventario. La balsa del ensamblaje se compone en el motor con ocho sprites, la pieza y la silueta de cada clase, como acordó el acta D07, en lugar de las treinta y cuatro láminas que habría exigido dibujarla en cada estado. Desde el 25/09/2026 esos sprites están en tres cuartos, como la balsa del cruce: tronco en diagonal (256 px), liana (128), mástil vertical (512) y vela de cuero (256). Las ocho texturas son legibles por el programa, porque el panel prueba su transparencia al agarrar y al soltar, y eso suma cerca de 1,3 MB. La balsa del cruce, `prop_n3_balsa_cruzando` (512 × 512), navega en la escena 3.3 por debajo de la espuma de la cascada (INC-124). La balsa hundida de la escena 3.2 se dibujó por código el 16/09/2026 y el acta D10 la aceptó como definitiva. Con el plano abierto del 01/10/2026, la escena 3.1 pinta el tronco y el mástil de la mecánica, y el montón plano anterior, `prop_n3_troncos`, quedó en disco sin uso (Anexo D, apartado 2.2).

**Interfaz.** La interfaz se arma con un conjunto genérico de cinco piezas (`ui_panel`, `ui_boton`, `ui_circulo`, `ui_alerta` y `ui_papelera`), teñidas desde el inspector, con las que se construyen el cuadro de diálogo, los paneles de los tres niveles, el editor de bloques, las listas, el informe docente y los menús; el acta D10 lo aceptó como arte definitivo. Lo completan el candado `ui_lock`, la flecha `ui_flecha` de la lista del laberinto y de la cruceta del río, la casilla hecha del Nivel 3 (`ui_n3_casilla_hecha`), que es también la marca de «Completado» del menú de niveles, y los cuatro iconos de los indicadores del informe docente, aceptados como definitivos en el acta D10. El estado de un intento se lee por el icono que acompaña al mensaje, con una paleta de estado propia y sin rojo de error (INC-105). El diálogo de las narrativas se lee en un cuadro fijo con el retrato y el nombre de quien habla, que ocupa el cuarto inferior de la pantalla; la dirección de arte lo describía como un globo con cola y se corrigió el 01/10/2026 (INC-129). Desde el 30/09/2026 la pantalla de inicio y los créditos muestran a Algoritm en su forma de fuego, y las tarjetas del menú de niveles muestran la cueva cenital, el laberinto y el río, en lugar de los rótulos de trabajo que quedaban a la vista (INC-82). La tarjeta del Nivel 2 usa el laberinto de 16:9 porque el bosque panorámico se veía partido por la costura. Las tipografías son Baloo 2 y Nunito, con licencia SIL Open Font License 1.1.

**Glifos de la pausa y créditos.** Los tres glifos del menú de pausa (`ui_pausa`, `ui_reanudar` y `ui_reiniciar`) son iconos de la familia Phosphor Icons rasterizados a 128 px, con licencia MIT, y los créditos los daban por originales. Desde el 01/10/2026 se acreditan (INC-127): el cuerpo de los créditos dice «Entornos, objetos e interfaz: originales del proyecto, salvo los iconos de pausa.» y «Iconos de pausa: Phosphor Icons, licencia MIT.», y el texto de la licencia está en `Assets/Game/Art/UI/Common/LICENSE-Phosphor.txt`. Las demás piezas de la interfaz son del proyecto. Los créditos nombran también a los autores y la obra de la Familia Anonaky, de la que parten los personajes (INC-78), y a Santiago Benavides Rey y Santiago Valdiri García en «Música y sonido»: las piezas son de producción propia. Al compilar el ejecutable, la herramienta de editor `LicenseNotices` copia los avisos de licencia de Phosphor Icons y de las tipografías a una carpeta `Licencias/` junto al programa, porque esas licencias piden que el aviso acompañe la copia distribuida.

**Personajes.** La familia y Algoritm se animan por recorte en la interfaz del motor (INC-53). Algoritm es una llama con extremidades en el Nivel 1, recoloreada en madera y en agua para los Niveles 2 y 3 (INC-45, INC-52), y su arte puede sustituirse por archivo sin tocar escenas. El 01/10/2026 se limpió el halo verde del recorte en siete imágenes de personajes (apartado 2.4). Todo lo demás de los personajes se describe en el Anexo F.

### 2.2 Reglas de importación de imagen y sonido

**Imagen.** La regla `ArtImportRules`, un procesador de importación del editor (`Game.EditorTools`, 16/09/2026), fuerza en todo `Assets/Game/Art/` dos ajustes, también al reimportar: la imagen entra sin comprimir y con un tamaño máximo de 4 096 px. Comprimida, la ilustración plana enseña la rejilla de bloques de 4 × 4 del formato, que se veía en la pared de la cueva y que la capa de oscuridad del Nivel 1 amplifica; con el máximo de fábrica, de 2 048 px, las panorámicas de 3840 px se reducirían. La regla no toca el modo de sprite. El importador trae por defecto el modo `Multiple`, con el que el motor no devuelve el sprite de un fondo o de un objeto entero, así que cada imagen nueva se pasa a `Single` desde el motor. Los demás ajustes que aplica el proyecto, según la dirección de arte, son el filtro bilineal, sin mapas MIP, y la lectura por el programa desactivada salvo en las ocho piezas de la balsa. La dirección de arte pedía comprimir, limitar a 2 048 px y cortar hojas en `Multiple`, y desde el 01/10/2026 describe la regla que aplica el proyecto y su razón (INC-128). La vigila `ArtImport_RNF23_LasIlustracionesEntranSinComprimirYSinReducir`.

Doce imágenes del Nivel 2 siguen en `Multiple`, cada una con un solo sprite: el horizonte `env_enlace_n2` y once objetos del bosque, `prop_n2_tronco_a`, las cuatro piedras, las tres plantas y las tres herramientas. Diez de ellas las referencian la configuración del bosque y las narrativas del Nivel 2 y del puente II por ese sprite interno, con un identificador distinto del que tendría en `Single`; pasarlas de modo rompería esas referencias, así que se decidió no migrarlas (INC-128). Las piedras `c` y `d` no las usa ningún asset; pasarlas a `Single` desde el motor o retirarlas queda como deuda menor (documento principal, apartado 10.4). Si una de las doce se sustituye por un archivo de otro tamaño, el recorte del sprite interno se ajusta desde el motor conservando su identificador. Así se hizo con las piedras: el 23/09/2026 su recorte apuntaba a una esquina transparente de la versión anterior y no se veían, y el 25/09/2026 se escaló con la nueva resolución.

La única excepción al tope de 4 096 px son los 67 cuadros del fuego y del humo de `Props/Fire/Animations/`, que desde el 01/10/2026 entran a 1 024 px como máximo y siguen sin comprimir (INC-130). Con ellos a tamaño completo, el primer ejecutable de doce escenas, compilado esa madrugada y descartado, pesó 866,7 MB, por encima de los 500 MB de RNF-06. En memoria el humo queda en 595 × 1024 y el fuego de frente en 1024 × 1007; el cenital, de 500 × 278, no cambia, y como la densidad de píxeles por unidad se escala con la textura, el tamaño en pantalla es el mismo. La revisión comparó capturas de antes y de después, ampliadas sin interpolar, y no halló pérdida visible. La prueba `ArtImport_RNF06_LosCuadrosDelFuegoYElHumoSeImportanAMil24SinComprimir` exige ese tope en esa carpeta, y la regla general solo admite esa excepción.

La importación sin comprimir es un costo asumido, porque se paga en memoria y en tamaño. El humo era el caso mayor: a 1123 × 1933, cada una de sus 33 texturas ocupaba unos 8,7 MB, y por eso la deduplicación de cuadros fue necesaria antes incluso de la reducción a 1 024 px, que deja cada una en unos 2,4 MB. La memoria que vale es la medida sobre el ejecutable del corte: la de trabajo llegó como máximo a 446 MB y la privada a 1 179 MB, y el paquete de doce escenas ocupa 479,0 MB, de ellos 276,4 MB de texturas del Nivel 1 (apartado 8.3 del documento principal).

**Sonido.** La regla `AudioImportRules` (21/09/2026) aplica a cada pieza los ajustes del apartado 4.3 de la dirección de sonido según el prefijo de su nombre: la música (`mus_`) en flujo continuo y en estéreo, con Vorbis de calidad 70; los ambientes (`amb_`) en flujo continuo y en mono, con calidad 60, y el resto como efecto, que si dura hasta 2 s se descomprime al cargar y se precarga en PCM, y si dura más se guarda comprimido en memoria. La duración la lee de la cabecera del archivo `.wav`. Un archivo sin prefijo cuenta como efecto, de modo que ninguna pieza queda sin regla. La vigila `AudioImport_RNF06_CadaFamiliaEntraConLosAjustesDeSuTabla`. Los nombres siguen el apartado 4.1 de la dirección de sonido, sin tildes ni mayúsculas, y una pieza se renombra desde el motor para conservar su identificador: mover un `.wav` fuera del motor sin su archivo de metadatos cambia el identificador y deja sin sonido a todo asset que lo referenciaba.

### 2.3 Sonido por nivel

**Gestor y contenido.** Todo el sonido pasa por `AudioManager`, del assembly `Game.Audio`, uno de los tres componentes persistentes del objeto `Persistent` de la escena `Boot`, junto al único oyente de audio (lo vigila `AudioListener_RNF13_ElUnicoOyenteVaEnBootYNingunaOtraEscenaTieneElSuyo`). Tiene cuatro canales con su volumen por omisión: música (0,8), ambiente (0,4), efectos (1) y voz (0,6). El ambiente admite dos capas, que entran con un fundido cruzado; pedir el ambiente que ya suena no lo reinicia, de modo que el paso entre una narrativa y una mecánica que comparten fondo no corta el sonido. Un clic sostenido puede llevar un sonido en bucle mientras dura, y los silencios del guion son cortes secos. Ningún nivel llama al gestor con clips propios: cada uno los toma de su asset de contenido (`N1_Sonidos`, `N2_Sonidos` y `N3_Sonidos`, instancias de `FireSounds`, `WheelSounds` y `RiverSounds`), y las narrativas declaran su sonido en los datos: el ambiente de cada secuencia, el efecto, el ambiente o el silencio de cada línea, el sonido de un objeto al posarse y el ambiente de un objeto mientras se mueve. Si el gestor no existe, como al abrir una escena sin pasar por `Boot`, el juego sigue en silencio, porque el sonido refuerza y nunca informa por sí solo.

**Las piezas.** El conjunto lo reunió Santiago Valdiri García entre el 13 y el 21/09/2026, organizado por nivel y global; el 21/09/2026 cada pieza recibió su archivo de metadatos con un identificador fijo y se renombró desde el motor a la nomenclatura de la dirección de sonido, y la chispa se rehízo sin el silencio que traía al principio. El sonido entró por niveles: el Nivel 1 el 21/09/2026, el Nivel 2 el 23/09/2026, el Nivel 3 el 25/09/2026 y, el 01/10/2026, el hundimiento de la balsa y la escena final. Hay diecinueve piezas en `Assets/Game/Audio/` y suenan dieciocho, las que referencia algún asset; la que no suena es `amb_n2_noche_intemperie`. Como la mayoría de las piezas que nombra el inventario de la dirección de sonido no se ha producido, varias piezas entregadas hacen de otras: la última columna de la Tabla E.3 indica de cuál, con el nombre que le da el inventario.

**Tabla E.3.** Piezas de sonido del juego y dónde suena cada una.

| Pieza | Dónde suena | Hace de |
|---|---|---|
| `amb_noche_intemperie` | Apertura del Nivel 1 | Su propia pieza |
| `amb_n1_cueva_oscura` | Escenas 1.1, 1.2 y 1.3, la cueva, la escena 2.5, el arranque del puente II y su horizonte | Su propia pieza; con el fuego encima, `amb_n2_refugio_fogata` |
| `amb_n1_cueva_fuego` | Segunda capa: al soplar, en la 1.3, en el arranque del puente I, en la 2.5, en el puente II y su horizonte y en la escena final | Su propia pieza |
| `amb_n2_bosque_dia` | Puente I, escenas 2.1 a 2.4 y las tres fases del Nivel 2; segunda capa en el río y sus escenas; fondo de la escena final | Su propia pieza; en el taller, `amb_n2_taller_claro` |
| `amb_n3_rio_orilla` | Orilla, ensamblaje y escenas del río (puente II en el río, 3.1, 3.2 y 3.3) | Su propia pieza |
| `amb_balsa_movimiento` | Segunda capa mientras la balsa cruza en la 3.3 | `sfx_n3_cruce` |
| `sfx_n1_pasos_eco` | Apertura, en la línea de los pasos | Su propia pieza |
| `sfx_n1_hojas_acomodo` | Arrastre de una hoja; escena 1.2; planta que se aparta en el bosque | Su propia pieza |
| `sfx_n1_pieza_tomar` | Paso de reunir a encender; cinco troncos reunidos; carretilla terminada; balsa terminada | Su propia pieza; `sfx_n2_carretilla_lista` |
| `sfx_n1_golpe` | Golpe con las piedras en contacto; escena 1.2 | `sfx_n1_golpe_a`, `_b` y `_c` |
| `sfx_n1_chispa` | Golpe efectivo; escena 1.2 | `sfx_n1_chispa_dentro_1` a `_3` |
| `sfx_n1_soplo` | «Soplar» | Su propia pieza |
| `sfx_n2_troncos` | Tronco que se aparta en el bosque; tronco que cae en la 2.2 | Sin fila en el inventario |
| `sfx_n2_piedra_cae` | Piedra o herramienta que se aparta; piedra y caja que caen en la 2.2 | Sin fila en el inventario |
| `sfx_n2_carretilla` | En bucle mientras la carretilla recorre la secuencia del laberinto | `sfx_n2_paso_*` y `sfx_n2_retroceso_bloqueado` |
| `sfx_encaje_pieza` | Tronco acopiado y tronco perforado; material recogido en la orilla | `sfx_n2_objeto_valido`, `sfx_n2_mecanizar`, `sfx_n3_recoger` |
| `sfx_martillo_madera` | Tres golpes por pieza encajada en la carretilla; uno por pieza puesta en la balsa y tres al aprobar una fase | `sfx_n2_eje_insertado`, `_tabla_colocada`, `_caja_colocada`, `sfx_n3_pieza_encaja`, `sfx_n3_fase_confirmada` |
| `sfx_n3_hundimiento` | Al empezar todo hundimiento de la balsa | Su propia pieza |
| `amb_n2_noche_intemperie` | No suena: ningún asset la referencia | — |

**Lo que suena en cada nivel.** En el Nivel 1 suena la noche a la intemperie en la apertura y la cueva oscura dentro, con la hoguera como segunda capa desde que se sopla. Arrastrar una hoja suena mientras se mueve; el golpe suena solo si las piedras se tocan, con un volumen que crece con el contacto, y la chispa solo acompaña al golpe efectivo. En el Nivel 2 el bosque de día suena de fondo en el puente I, en las escenas 2.1 a 2.4 y en las tres fases, sin costura entre ellas. Cada objeto que el cursor aparta en el bosque suena una vez por acercamiento con su material; en el taller, perforar suena a encaje y cada pieza encajada a tres martillazos; en el laberinto la carretilla suena en bucle mientras recorre la secuencia, también en el intento que tropieza. En la escena 2.2 cada objeto suena al tocar el suelo. En el Nivel 3 el río suena con el bosque encima, recoger suena una vez a encaje, cada pieza puesta en la balsa suena a un martillazo, también la equivocada, porque colocar no valida, y aprobar una fase suena a tres; la balsa terminada suena antes de salir al cruce, y en el cruce la balsa suena mientras se mueve. La escena final suena desde el 01/10/2026 al bosque de día con el crepitar de las fogatas en la segunda capa, el par con que abre el Nivel 2 (INC-125).

**El hundimiento.** La pieza del hundimiento llegó al proyecto como una salpicadura, se renombró el 25/09/2026 fuera del motor, con un identificador nuevo y una errata en el nombre, y quedó sin referenciar. El 01/10/2026 se renombró desde el motor a `sfx_n3_hundimiento`, el nombre que le da la dirección de sonido, con el mismo identificador (`be2ba44e5cfd935448ca5a20ff9f2cb8`) y los mismos 306 434 bytes, y pasó a sonar al empezar todo hundimiento de la balsa, el de la prueba anticipada y el de la última fase (INC-123). Dura 1,74 s, más que los 0,6 s del hundimiento, así que su cola acompaña la vuelta de la balsa a su sitio. Describe lo que le pasa a la balsa y no castiga. Su comportamiento en la mecánica se describe en el Anexo D, apartado 2.3.

**Silencios y lo que calla.** El guion escribe tres silencios. Los dos del Nivel 1 están implementados como un campo de la línea en que ocurren: en la apertura, la línea «Solo oscuridad.» corta en seco todo el sonido (S1), y en la escena 1.1, cuando Algoritm se apaga, callan la música y la voz y queda la cueva (S2). El tercero, un segundo con el ambiente al mínimo antes de la última frase de Algoritm en la escena final (S3), no se puede expresar con el mecanismo actual, que solo calla la música o lo calla todo sin devolverlo; desde que la escena final tiene ambiente, tampoco se cumple por omisión (apartado 6). Callan a propósito, conforme al apartado 2.1 de la dirección de sonido, «Listo» rechazado, la pieza que vuelve al inventario, la entrada en la zona de construcción sin todos los materiales y el golpe con las piedras separadas. Ninguna pieza lleva en su nombre derrota, error ni fallo, lo que vigila `AudioAssets_CP02_NingunaPiezaSeLlamaDerrotaNiError`.

### 2.4 Verificación del arte por programa

El acta D10 encargó comprobar todos los sprites por programa contra el doble indicador, el contraste y los destellos, limpiar el halo de siete imágenes y renombrar los archivos fuera de la nomenclatura (tarjeta D10-4). La comprobación la hace la herramienta `arte_check.py`, guardada con las herramientas de evaluación del OE4 en `claudeDocs/tasks/OE4/herramientas/`, y sus informes, cada uno con una lectura escrita a mano, están en `claudeDocs/tasks/OE4/evidencias/arte/`.

**Sprites.** De las 163 imágenes de `Assets/Game/Art/`, la herramienta analiza 96; excluye los 67 cuadros del fuego y del humo, que conservan su nombre de entrega. Las 96 cumplen la nomenclatura, las que deben tener transparencia la tienen, ninguna tiene halo intenso y las 96 están sin comprimir y a 4 096 px. Los metadatos de 84 son los esperados; los 12 restantes son las imágenes en `Multiple` de la excepción de INC-128. Quedan 27 archivos con un halo tenue, verde oscuro y visible solo sobre fondo claro, casi todos partes de los personajes, que el informe registra como dato.

**Nombres.** La prueba `ArtImport_RNF23_LosNombresSiguenLaNomenclatura` recorre todos los sprites y exige el prefijo de categoría, sin tildes, espacios ni mayúsculas, con la excepción de los cuadros del fuego y del humo. Antes del cambio falló con exactamente cinco nombres: `entorno_n1_apertura`, `entorno_n1_cueva_2x`, `entorno_n1_cueva_cenital`, `entorno_n2_laberinto` y `prop_n2_caja_suelo_vacía`. Los cinco se renombraron desde el motor, primero en simulación, a `env_n1_apertura`, `env_n1_cueva_2x`, `env_n1_cueva_cenital`, `env_n2_laberinto` y `prop_n2_caja_suelo_vacia`; sus archivos de metadatos quedaron idénticos, identificador incluido, y como escenas y assets los referencian por identificador, ninguna referencia cambió (INC-126).

**Halos.** Las tres formas de Algoritm y los cuatro retratos de la familia traían restos verdes del fondo de croma en el borde semitransparente. Se limpiaron con la herramienta, limitada a los píxeles con transparencia por debajo de 200 sobre 255, con una copia de respaldo previa; la transparencia quedó idéntica y ningún píxel opaco cambió. Después de la limpieza, el informe no encuentra ningún archivo con halo intenso, y en las capturas de los retratos y de Algoritm ya no se ve el halo del borde; quedan unas motas opacas que se describen en el apartado 6.

**Doble indicador (RNF-19).** La herramienta compara en escala de grises 48 parejas de estados que el juego distingue por color, y las da por distinguibles si su silueta o su gris medio cambian lo suficiente. Cuarenta y cinco pasan por forma o por gris. Las otras tres se explicaron una por una. La del seto y el camino del laberinto midió una zona desfasada, porque el laberinto se sortea en cada partida, y medida de nuevo sobre la captura vigente pasa con holgura. Las otras dos son cambios localizados que lleva un icono: el espacio vacío frente al equivocado de la balsa, distinguido por el icono de alerta, cuyo signo contrasta 5,6:1 con la silueta, y «Soplar» atenuado frente a habilitado, distinguido por el candado, a 13,8:1. Las pruebas `AssemblyPanel_RNF19_ElEspacioIncorrectoSeResaltaConColorEIcono` y las de RNF-19 del Nivel 1 vigilan esos iconos.

**Contraste (RNF-20).** Diez regiones de texto sobre su fondo, en la tablilla del Nivel 1, el cuadro de diálogo y el menú principal, superan el mínimo de 4,5:1, la más baja con 6,2:1. Dieciocho regiones más de los demás niveles y pantallas también lo superan; en una de ellas, el icono de alerta de la balsa, la medida se repitió sobre el signo del icono, con 5,6:1.

**Destellos (RNF-21).** Las tres animaciones por cuadros del Nivel 1, la llama de frente (13 dibujos), la cenital (21) y el humo (33), no pasan de un destello por segundo, frente al umbral de más de tres. La herramienta mide los cuadros de entrega; lo que compone el motor (el quemado, el rayo, los fundidos, el cruce) lo vigilan pruebas en ejecución, como `FirePanel_RNF21_ElRayoDeLaChispaHaceUnSoloBarridoSinVolver` y `RiverLevel_RNF21_NingunaAnimacionDelNivel3TieneDestellos`.

## 3. Decisiones de diseño y hallazgos de consistencia

Las decisiones que dieron forma al carril salieron de las actas y del propio trabajo de integración:

- **Los recursos no introducen mecánicas** (acta D01). Ningún asset añade salto, desplazamiento libre, derrota ni puntaje, y ninguna animación de personaje expresa derrota; tras un fallo, el personaje anima (acta D06).
- **Sustituir el archivo basta** (acta D06). Los objetos entran con su nombre y su identificador, y como los encuadres y las posiciones se expresan en fracciones del lienzo, una entrega nueva no toca código ni escenas.
- **La luz la pone el motor** (actas D04 y D08). La oscuridad del Nivel 1 y el día del Nivel 2 se hacen con capas y tintes sobre una sola ilustración, sin multiplicar el encargo de arte.
- **Componer en lugar de dibujar estados** (actas D07 y D09). La balsa se compone con ocho piezas; el tronco del Nivel 2 rueda en el motor a partir de una sola textura, donde la propuesta del acta D09 separaba cuerpo y veta, y el humo, la chispa, la llama y el quemado se ponen encima de un solo montón de hojas.
- **Un cambio de sprite por dibujo distinto** (21/09/2026). Las animaciones por cuadros cargan solo los dibujos distintos, y su temporización vive en el clip.
- **La resolución sigue a lo que se ve** (25/09/2026). Un objeto se entrega al tamaño máximo con que aparece en pantalla.
- **Las piezas globales hacen de las no entregadas** (21 a 25/09/2026). El sonido de los Niveles 2 y 3 se armó con las piezas existentes antes que dejar un evento mudo, y la dirección de sonido registra cada sustitución en su historial.
- **Ningún sonido de fallo** (acta D07 y dirección de sonido, apartado 2.1). Un intento sin éxito suena solo cuando el sonido describe lo que pasa, como el hundimiento de la balsa; «Listo» rechazado y la pieza que vuelve al inventario no suenan.
- **Correcciones del acta D10** (30/09/2026): el rayo de la chispa, el sonido del hundimiento y de la escena final, los renombres, el crédito de Phosphor Icons, la limpieza de halos, la verificación por programa y la aceptación como definitivos del conjunto genérico de interfaz, del anillo de la zona, de la balsa hundida y de los iconos de los indicadores.

**Tabla E.4.** Hallazgos de consistencia del carril.

| INC | Qué decidió | Documentos corregidos o implementación | Fecha de cierre |
|---|---|---|---|
| INC-52 | Algoritm es una llama con extremidades, recoloreada en madera y agua | Guion, dirección de arte, Interfaces, dirección de sonido, índice de sprites | 29/09/2026 |
| INC-66 | La mecánica del Nivel 1 se ve desde arriba y el Nivel 3 es un plano fijo con perspectiva | Guion y dirección de arte | 29/09/2026 |
| INC-68 | La chispa del golpe se ve | Casos del OE4 e índice de sprites; chispa dibujada por el motor | 29/09/2026 |
| INC-82 | Arte real en los menús en lugar de rótulos de trabajo | Casos del OE4; escenas de inicio, créditos y menú de niveles | 29/09/2026 |
| INC-105 | Paleta de estado propia, sin rojo de error | Interfaces y dirección de arte | 29/09/2026 |
| INC-119 | La chispa es un solo rayo que nace en el punto del golpe | Dirección de arte, índice de sprites, casos del OE4; `Level1_Cave` | 01/10/2026 |
| INC-123 | La balsa que se hunde suena | Direcciones de sonido y de arte, índice de sprites; `sfx_n3_hundimiento` en `N3_Sonidos` | 01/10/2026 |
| INC-125 | La escena final suena al bosque con las fogatas | Dirección de sonido, casos del OE4; `N3_EscenaFinal` | 01/10/2026 |
| INC-126 | Cinco archivos renombrados a la nomenclatura | Dirección de arte, índice de sprites, guía del repositorio; prueba de nombres | 01/10/2026 |
| INC-127 | Los glifos de pausa se acreditan a Phosphor Icons | Dirección de arte, índice de sprites, Interfaces; `CreditsContent` y licencia | 01/10/2026 |
| INC-128 | La importación es sin comprimir, a 4 096 px y en `Single`, con doce excepciones | Dirección de arte e índice de sprites | 01/10/2026 |
| INC-129 | El diálogo es un cuadro con retrato | Dirección de arte e Interfaces | 01/10/2026 |
| INC-130 | Los cuadros del fuego y del humo entran a 1 024 px para que el paquete quepa en 500 MB | Dirección de arte, índice de sprites, guía del repositorio; `ArtImportRules` y su prueba | 01/10/2026 |

## 4. Verificación

### 4.1 Pruebas que lo nombran

Las pruebas del carril están repartidas por los módulos que tocó. Las reglas de importación, la nomenclatura y el oyente único son pruebas de arquitectura de `Game.Architecture.Tests`, que en el corte tiene 23 métodos y ejecutó 23 casos en la corrida final. El gestor de audio se prueba en `Game.Audio.PlayMode.Tests`, con 4 métodos y 4 casos; el módulo EditMode de `Game.Audio` existe y no tiene pruebas. El sonido de cada nivel se prueba en el módulo del nivel, comprobando qué clip sonó, y el arte de las narrativas en `Game.Scaffolding.Tests` y `Game.UI.PlayMode.Tests`.

**Tabla E.5.** Pruebas representativas del carril.

| Prueba | Qué vigila |
|---|---|
| `ArtImport_RNF23_LasIlustracionesEntranSinComprimirYSinReducir` | Todo el arte sin comprimir y a 4 096 px, salvo los cuadros del fuego y del humo |
| `ArtImport_RNF06_LosCuadrosDelFuegoYElHumoSeImportanAMil24SinComprimir` | Los cuadros del fuego y del humo, a 1 024 px y sin comprimir |
| `ArtImport_RNF23_LosNombresSiguenLaNomenclatura` | La nomenclatura de todos los sprites |
| `AudioImport_RNF06_CadaFamiliaEntraConLosAjustesDeSuTabla` | Los ajustes de importación de cada familia de sonido |
| `AudioAssets_CP02_NingunaPiezaSeLlamaDerrotaNiError` | Ninguna pieza con nombre de fallo |
| `AudioListener_RNF13_ElUnicoOyenteVaEnBootYNingunaOtraEscenaTieneElSuyo` | Un solo oyente de audio |
| `AudioManager_RF05_PedirElAmbienteQueYaSuenaNoLoReinicia` | El ambiente no se corta entre escenas que lo comparten |
| `FirePanel_CP02_SeparadasNoSuenanEnContactoSuenanAPiedraYSoloElEfectivoAnadeLaChispa` | El golpe y la chispa del Nivel 1 |
| `ForestScene_RF22_ElBosqueSuenaDeFondoYCadaObjetoSuenaALoQueEsAlApartarse` | El sonido del bosque |
| `WorkshopScene_RF29_PerforarSuenaAEncajeCadaPiezaATresMartillazosYElFinalAPiezaTomada` | El sonido del taller |
| `MazeScene_RF32_LaCarretillaSuenaMientrasRecorreLaSecuenciaYCallaAlTerminar` | El sonido del laberinto |
| `RiverSounds_RF44_CadaMomentoDelNivelSuenaConSuPieza` | Cada momento del Nivel 3 con su pieza |
| `AssemblyPanel_CP02_UnaFaseQueNoPasaNoSuena` | «Listo» rechazado no suena |
| `AssemblyPanel_RF42_LaBalsaQueSeHundeSuenaUnaSalpicaduraYNadaMas` | El hundimiento suena una vez |
| `RiverSounds_RF44_LaEscenaFinalSuenaAlBosqueConLasFogatas` | El ambiente de la escena final |
| `NarrativeScene_RF44_LaBalsaSuenaMientrasCruzaYAlLlegarVuelveElBosque` | El sonido del cruce |
| `NarrativeSequence_RF05_CadaLlamaEchaHumoPorDetrasYPorEncima` | El humo detrás de cada llama |
| `RollingLog_RF26_En34ElCorteSeAchataYElCostadoSeAlejaPorElEje` | El tronco rodante en tres cuartos |
| `FirePanel_RF16_LaChispaEsUnRayoDelCentroQueCaeEnLasHojasEnUnaDireccionAlAzar` | El rayo de la chispa |
| `NarrativeSequence_RF05_ElNivel2TranscurreDelAmanecerALaNoche` | La luz del día del Nivel 2 |
| `LevelSelect_RF03_CadaTarjetaMuestraUnaIlustracionSinCostura` | Las tarjetas del menú de niveles |
| `Credits_RNF23_NombraALosAutoresDeLosPersonajesYLaObraDeOrigen` | La autoría de los personajes en los créditos |
| `CreditsContent_RNF01_NingunaOracionSupera20Palabras` | Los créditos, con el de Phosphor Icons, en oraciones cortas |

La regla `LicenseNotices` tiene cuatro pruebas propias, entre ellas `LicenseNotices_RNF23_ElEjecutableLlevaLasLicenciasDeTerceros`.

### 4.2 Corridas declaradas

**Tabla E.6.** Corridas registradas por el carril.

| Fecha | Corte | EditMode | PlayMode | Observaciones |
|---|---|---|---|---|
| 22/09/2026 | `8ec5120` (quemado y reparto del Nivel 1) | Secuencias narrativas: 9 de 9; arquitectura en verde | 60 de 65 | Cuatro capturas sin ejecutar en modo por lotes; una prueba del cuadro de diálogo falló por 1 px en `N1_Hallazgo`, sin relación con el cambio |
| 25/09/2026 | `44fd479` (objetos definitivos) | 360 de 361 (1 omitida) | Niveles 2 y 3: 133 de 134 | Con el Editor abierto. Falló la prueba de memoria del Nivel 3, que medía la sesión del Editor (2 941 MB) |
| 01/10/2026 | Verificación del arte (tarjeta D10-4) | 428 de 429 (1 omitida) | Dirigida a lo afectado: 59 de 59 | Con el Editor abierto a 1920 × 1080 |

Los demás commits del carril no dejaron cifra de corrida propia; sus pruebas corrieron dentro de las corridas completas de la suite del 25/09/2026, el 30/09/2026 y el 01/10/2026 y de la corrida final, que presenta el Anexo G en su apartado 4.2. La omitida es siempre la misma prueba del borrado sobre disco real, que no puede simular una carpeta de solo lectura en el equipo de desarrollo. La prueba del cuadro de diálogo que falló el 22/09/2026 pasa con el Editor abierto a 1920 × 1080 desde las corridas del 01/10/2026.

## 5. Archivos principales

Este anexo reúne el inventario del arte y del sonido del juego, archivo por archivo.

**Tabla E.7.** Entornos (`Assets/Game/Art/Environments/`).

| Archivo | Qué es |
|---|---|
| `Fire/env_n1_apertura.png` | Exterior nocturno y cueva en una panorámica de 3840 × 1080; apertura y, al amanecer, puente I |
| `Fire/env_n1_cueva_2x.png` | Interior de la cueva, 3840 × 1080; narrativas 1.1 a 1.3, escena 2.5 y arranque del puente II |
| `Fire/env_n1_cueva_cenital.png` | La cueva desde arriba, 1920 × 1080; mecánica del Nivel 1 y su tarjeta en el menú |
| `Wheel/env_n2_bosque_claro.png` | Bosque y claro del taller, 3840 × 1080; narrativas del Nivel 2, bosque y taller |
| `Wheel/env_n2_laberinto.png` | Tablero cenital del laberinto, 1920 × 1080; laberinto y tarjeta del Nivel 2 |
| `River/env_n3_rio.png` | El río con la cascada, 1920 × 1080; todo el Nivel 3 y sus narrativas |
| `River/env_n3_zona_disponible.png` | Anillo ámbar de la zona de construcción, 256 × 256 |
| `Narrative/env_enlace_n2.png` | El horizonte del puente II, 1920 × 1080, en `Multiple` |
| `Narrative/env_final_fogatas.png` | El poblado al atardecer, 1920 × 1080; escena final |

**Tabla E.8.** Objetos, efectos y animaciones (`Assets/Game/Art/Props/` y `FX/`).

| Archivo | Qué es |
|---|---|
| `Props/Fire/prop_n1_hoja.png`, `prop_n1_silex.png`, `prop_n1_pedernal.png` | La hoja suelta y las dos piedras del Nivel 1, 2000 × 2000 |
| `Props/Fire/prop_n1_monton_hojas.png`, `prop_n1_monton_hojas_cenital.png` | El montón de frente (narrativas con fuego) y desde arriba (mecánica), 2000 × 2000 |
| `Props/Fire/Animations/prop_n1_fuego_normal.anim` y `prop_n1_fuego_cenital.anim`, con sus controladores | Las dos llamas por cuadros |
| `Props/Fire/Animations/Fuego normal/` (13) y `Fuego cenital/` (21) | Los cuadros de las llamas, con su nombre de entrega |
| `Props/Fire/Animations/fx_n1_humo_nacer.anim` y `fx_n1_humo.anim`, con sus controladores | El humo que nace y el que sigue en bucle |
| `Props/Fire/Animations/Smoke/` (33) | Los cuadros del humo, con su nombre de entrega |
| `Props/Wheel/prop_n2_tronco_a.png`, `prop_n2_tronco_textura.png` | El tronco del bosque y su textura para rodar (768 × 256) |
| `Props/Wheel/prop_n2_piedra_a.png` a `_d`, `prop_n2_planta_a.png` a `_c`, `prop_n2_herramienta_a.png` a `_c` | Los distractores del bosque, 256 × 256, en `Multiple` |
| `Props/Wheel/prop_n2_caja_suelo.png`, `prop_n2_caja_suelo_vacia.png` | La caja de alimentos llena; la vacía, sin uso |
| `Props/Wheel/prop_n2_pieza_1.png` a `_5` | Tronco corto, cuerda, eje, tabla y herramienta del taller |
| `Props/Wheel/prop_n2_carretilla_e1.png` a `_e5` | Rueda perforada y estados de la carretilla hasta la amarrada |
| `Props/Wheel/prop_n2_laberinto_carretilla.png`, `prop_n2_laberinto_obstaculo.png` | Carretilla cenital y arbusto del laberinto |
| `Props/River/prop_n3_tronco.png`, `_sogas`, `_tela`, `_mastil` | Los materiales de la orilla e iconos del inventario |
| `Props/River/prop_n3_tronco_silueta.png`, `prop_n3_amarre.png` y su silueta, `prop_n3_mastil_silueta.png`, `prop_n3_vela.png` y su silueta | Las piezas y siluetas de la balsa del ensamblaje |
| `Props/River/prop_n3_balsa_cruzando.png`, `prop_n3_balsa_hundida.png` | Las balsas de las escenas 3.3 y 3.2 |
| `Props/River/prop_n3_troncos.png` | El montón plano anterior, sin uso |
| `FX/fx_oscuridad.shader` y `.mat` | La capa de oscuridad del Nivel 1 |
| `FX/fx_contraste.shader` y `.mat` | Contraste y saturación del laberinto |

**Tabla E.9.** Interfaz, tipografías y personajes (`Assets/Game/Art/UI/`, `Fonts/` y `Characters/`).

| Archivo o carpeta | Qué es |
|---|---|
| `UI/Common/ui_panel.png`, `ui_boton.png`, `ui_circulo.png`, `ui_alerta.png`, `ui_papelera.png` | El conjunto genérico de interfaz |
| `UI/Common/ui_lock.png`, `ui_flecha.png` | Candado y flecha |
| `UI/Common/ui_pausa.png`, `ui_reanudar.png`, `ui_reiniciar.png`, `LICENSE-Phosphor.txt` | Glifos de la pausa de Phosphor Icons y su licencia |
| `UI/Common/ui_ind_intentos_boceto.png`, `_errores_`, `_pasos_`, `_tiempo_` | Los iconos de los indicadores del informe docente |
| `UI/Common/ui_pulso_pista.anim` | El pulso del botón de pista del Nivel 1 |
| `UI/River/ui_n3_casilla_hecha.png` | La tarea hecha del Nivel 3 y la marca de «Completado» |
| `Fonts/` | Baloo 2 (Bold y ExtraBold) y Nunito (Regular, SemiBold y Bold), con `OFL.txt` |
| `Characters/` | Partes, retratos, formas de Algoritm y clips de los personajes (Anexo F, apartado 5) |

**Tabla E.10.** Sonido (`Assets/Game/Audio/`).

| Carpeta | Piezas |
|---|---|
| `Global/` | `sfx_encaje_pieza`, `sfx_martillo_madera`, `sfx_n1_pieza_tomar` |
| `Level 1/` | `amb_noche_intemperie`, `amb_n1_cueva_oscura`, `amb_n1_cueva_fuego`, `sfx_n1_pasos_eco`, `sfx_n1_hojas_acomodo`, `sfx_n1_golpe`, `sfx_n1_chispa`, `sfx_n1_soplo` |
| `Level 2/` | `amb_n2_bosque_dia`, `amb_n2_noche_intemperie`, `sfx_n2_troncos`, `sfx_n2_piedra_cae`, `sfx_n2_carretilla` |
| `Level 3/` | `amb_n3_rio_orilla`, `amb_balsa_movimiento`, `sfx_n3_hundimiento` |

**Tabla E.11.** Código y contenido del carril.

| Archivo | Qué es |
|---|---|
| `Scripts/Runtime/Audio/AudioManager.cs` | El gestor de audio: cuatro canales, ambiente en dos capas, cortes y bucles |
| `Scripts/Runtime/Levels/Fire/FireSounds.cs`, `Levels/Wheel/WheelSounds.cs`, `Levels/River/RiverSounds.cs` | El contenido sonoro de cada nivel |
| `Data/Fire/N1_Sonidos.asset`, `Data/Wheel/N2_Sonidos.asset`, `Data/River/N3_Sonidos.asset` | Las piezas asignadas a cada momento de cada nivel |
| `Scripts/Runtime/Scaffolding/SilenceCut.cs`, `DialogueLine.cs`, `NarrativeSequence.cs`, `NarrativeProp.cs` | Los campos de sonido y animación de las narrativas |
| `Scripts/Runtime/Scaffolding/BurnReveal.cs` | El quemado desde el centro |
| `Scripts/Runtime/Scaffolding/RollingLog.cs`, `RollingLogLook.cs`; `Data/Wheel/N2_TroncoRodante.asset` | El tronco que rueda y su vista |
| `Scripts/Runtime/Levels/Fire/FloorScatter.cs`, `LeafPile.cs` | El reparto al azar de las piezas y los montoncitos de hojas |
| `Scripts/Runtime/Levels/Wheel/ClickRelay.cs` | La barra de desplazamiento del laberinto, que se pulsa y no se arrastra |
| `Scripts/Editor/ArtImportRules.cs`, `AudioImportRules.cs`, `LicenseNotices.cs` | Las reglas de importación y la copia de las licencias junto al ejecutable |
| `Data/CreditsContent.asset` | Los créditos, con la autoría de los personajes, el sonido y los glifos de pausa |
| `Art/Inventario.md` | El índice de sprites, carpeta por carpeta |

## 6. Evaluación del OE4 y verificaciones a cargo de Santiago Benavides Rey

Lo que el carril deja al cuarto objetivo es de oído y de vista. Con el guion H6 de la hoja de verificaciones manuales del OE4, Santiago Benavides Rey escucha si el crepitar de las fogatas de la escena final, que es la pieza de la hoguera dentro de la cueva, suena a fogata al aire libre o a eco de cueva; si suena a cueva, se pide una toma de la fogata sola. Con el guion H7 escucha el hundimiento de la balsa en las tres fases y si dos pruebas seguidas, con dos salpicaduras encadenadas, resultan molestas. El guion H12 recorre el sonido entero: durante el recorrido cronometrado marca cada una de las 18 piezas en uso en el momento en que debe sonar, provoca una vez cada fallo que la dirección de sonido deja mudo y confirma que `amb_n2_noche_intemperie` no suena en ninguna parte. La segunda pasada de pruebas sobre el ejecutable del corte se hizo con el sonido silenciado, así que no adelanta ninguna de esas escuchas. La sesión con estudiantes y las ejecuciones en un segundo equipo, sin red y sin tarjeta gráfica dedicada afectan al juego entero y se describen en el capítulo 10 del documento principal.

Del sonido sigue abierto lo que recogen las tarjetas D07-2 y D08-2, a cargo de Santiago Benavides Rey: no hay música ni sonido del diálogo; falta el ambiente nocturno del Nivel 2, de modo que la escena 2.5 y el arranque del puente II suenan a la cueva con la hoguera, y la pieza nocturna que existe en disco no se usa; no se ha entregado la pieza propia de la prueba de la balsa, y el silencio S3 de la escena final exige un corte con duración en el gestor de audio, su valor en `SilenceCut` y partir la línea correspondiente de `N3_EscenaFinal`. La dirección de sonido mantiene abiertos el control de volumen, que ningún requerimiento pide, y la duración de los bucles de música (puntos PS-01 y PS-03).

Del arte, la balsa del cruce tiene cuatro troncos mientras la mecánica arma cinco; se acepta como está y su corrección corresponde a la entrega de la colaboradora del 07/10/2026. Varios objetos que nombra el guion no tienen sprite (la comida, la maleza y las piedras en la mano), y las narrativas no pueden hacer aparecer un objeto a mitad de escena. Los retratos de la Niña y de Papá conservan unas doce motas verdes opacas en las puntas del pelo, que la limpieza de halos no alcanza porque se limita a los píxeles semitransparentes; se resuelven con una limpieza a mano en el carril de arte, porque una automática tocaría píxeles opacos del dibujo. La caja vacía del Nivel 2 y el montón plano del Nivel 3 siguen en disco sin uso, y retirarlos es decisión del carril de arte.
