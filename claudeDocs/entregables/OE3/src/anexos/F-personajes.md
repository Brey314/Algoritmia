# Personajes animados

Este anexo describe cómo entraron al prototipo la familia y Algoritm, animados, en las dieciocho secuencias narrativas y en las cinco escenas jugables, desde el 24/09/2026 hasta el corte del 1 de octubre de 2026. Los apartados 2.6 a 2.9 recogen además lo que la rama `feat/personajes-animados` hizo del 02 al 06/10/2026: es trabajo posterior al corte y no está fusionado a la rama principal, de modo que las cifras del documento principal (Tablas 8.1 y 9.3, con sus 846 casos) siguen siendo las del corte y no lo incluyen. Sale del documento de resultados de los personajes animados (`claudeDocs/tasks/Personajes/Personajes-Resultados.md`), que el repositorio conserva como registro histórico con sus notas fechadas, del registro de inconsistencias (revisión 18), de los mensajes de los commits de la rama y del código y los assets, del corte y de la rama. Sirve a los capítulos 4 y 7 del documento principal (apartado 7.2) y al apartado 8.2, donde la animación por recorte figura entre las optimizaciones. Las tablas de los apartados 2.6 a 2.9 y de las corridas posteriores llevan los números F.9 a F.13, para no alterar los que citan otros capítulos.

## 1. Alcance y seguimiento

El pedido de Santiago Benavides Rey tuvo cinco partes: la familia y Algoritm animados en todo el juego, los objetos que el guion nombra puestos en escena, el retrato de quien habla en el cuadro de diálogo, el botón de ayuda con la forma de Algoritm de cada nivel y un arte que pudiera sustituirse por archivo. Todo entró con el commit `88fe0ee` del 24/09/2026 (333 archivos), que la solicitud de integración n.º 86 fusionó ese mismo día en la rama principal. Los sprites base de la familia, basada en la Familia Anonaky y usada con autorización escrita de sus autores, y la forma de fuego de Algoritm los entregó Santiago Benavides Rey. Después siguieron dos tandas: el cierre de inconsistencias del 30/09/2026 (`37b3cb7`, solicitud n.º 88), que añadió la estela de Algoritm y la sombra de la familia, y las correcciones del Nivel 3 del acta D10, en el árbol de trabajo del corte.

**Tabla F.1.** Actas que gobernaron el incremento.

| Acta | Fecha | Qué decidió o registró para este incremento |
|---|---|---|
| D06 | 15/09/2026 | Las animaciones de personaje se hacen sobre el dibujo en pose neutra y no con hojas de fotogramas; la familia y el guía aún no tenían arte |
| D07 | 19/09/2026 | Tarjeta D07-3: seguimiento de los sprites con la colaboradora, con prioridad en los personajes |
| D08 | 23/09/2026 | Los personajes figuran como el bloque más grande del arte; las narrativas se reproducían sin ellos |
| D09 | 24/09/2026 | Se presentan la familia y Algoritm animados en las dieciocho narrativas y las cinco escenas jugables, con retrato en el diálogo y Algoritm en el botón de ayuda |
| D10 | 30/09/2026 | Tarjeta D10-3: en la mecánica del Nivel 3 los personajes crecen hasta la escala de las narrativas del río, y la balsa del cruce baja con los cuatro viajeros |

El trabajo no tuvo tarjeta propia: `88fe0ee` es uno de los commits de funcionalidad sin tarjeta que el documento principal cuenta al tratar CT-11 (apartado 2.5). Las actas D07 y D08 lo siguieron con la tarjeta D07-3, el seguimiento del arte con la colaboradora, Sofía Valentina Giraldo Segovia, a cargo de Santiago Benavides Rey y Santiago Valdiri García; la D10 abrió la D10-3, a cargo de Santiago Benavides Rey.

Del 02 al 06/10/2026, fuera ya del corte, la rama sumó cuatro bloques de trabajo: el plan maestro de los personajes finales y la corrección de la pose en T (`d752bef`, 02/10/2026); el rig articulado con caras, con el arte final del Niño (`a12dbb3`, `e9eb6cb` y `d127ec9`, 05/10/2026, INC-131); el orden de dibujo de los brazos con la coreografía escrita fuera del motor (`8740a64` a `8cdd4c5`, INC-132); y el arte final frontal de Papá, Mamá y la Niña con las caras de los cuatro y el antebrazo delante del torso (`63ed2fc` a `433603f`, 06/10/2026, INC-133). INC-131, INC-132 e INC-133 son decisiones de Santiago Benavides Rey posteriores al acta D10 y no tienen acta: constan en `claudeDocs/INCONSISTENCIAS.md` (revisiones 17 y 18, del 05/10/2026, y revisión 19, del 06/10/2026).

## 2. Lo implementado

### 2.1 Arte y rig por recorte

Cada miembro de la familia (Papá, Mamá, la Niña y el Niño) se corta en cinco partes, torso, dos brazos y dos piernas, con el pivote en la articulación, y tiene un retrato con la expresión `neutra`; la emoción la llevan las acciones del cuerpo, como celebrar, sorprenderse u observar (INC-108). Esa es la descripción del arte provisional, vigente al corte. El 05/10/2026 INC-131 la levantó para el arte final, con la cabeza separada, las extremidades en dos tramos y seis expresiones (apartado 2.6), y desde el 06/10/2026 los cuatro tienen su arte final (apartados 2.7 y 2.10). Algoritm tiene tres formas: la llama con extremidades del Nivel 1 y la misma figura recoloreada en madera (rueda) y en agua (gota) para los Niveles 2 y 3, como la describen desde el 29/09/2026 el guion y la dirección de arte (INC-45, INC-52). Es una sola imagen por forma y todo lo que muestra al guía referencia esos tres archivos, así que una versión dibujada aparte entraría sustituyendo el archivo, sin tocar escenas, prefabs ni assets. Con el arte final de Algoritm la figura se parte en brazos, piernas, ojos y boca (apartado 2.6); hasta que llegue, el cuerpo entero sigue siendo lo visible. El 01/10/2026 se limpió el borde de las tres formas y de los cuatro retratos (Anexo E, apartado 2.4).

`CharacterRig` (`Game.Scaffolding`) es el componente de cada prefab. Su `Animator` tiene un estado por cada valor de `ActorAction`, un enumerado de 22 acciones cuyos valores son explícitos porque los assets guardan el número. `Play` pasa a una acción con un fundido de 0,18 s y cae al reposo si el controlador no tiene ese estado; `PlayFor` la sostiene unos segundos en tiempo escalado, de modo que la pausa la congela.

**Tabla F.2.** Arte, prefabs y clips de los personajes.

| Qué | Cuántos | Detalle |
|---|---|---|
| Partes y retratos de la familia | 24 imágenes | Cinco partes y un retrato `neutra` por personaje (`char_<x>_parte_*`, `char_<x>_retrato_neutra`) |
| Formas de Algoritm | 3 imágenes | `char_algoritm_n1_fuego_reposo`, `char_algoritm_n2_rueda_reposo` y `char_algoritm_n3_gota_reposo` |
| Prefabs | 7 | `Papa`, `Mama`, `Nina`, `Nino`, `Algoritm_Fuego`, `Algoritm_Rueda` y `Algoritm_Gota` |
| Clips de la familia | 84 | 21 por personaje: caminar, correr, hablar, golpear, martillar, soplar, recoger, empujar, señalar, celebrar, animar y otras |
| Clips de Algoritm | 9 | Ánimo, apagado, aparición, celebrar, flotar, girar, hablar, oculto y señalar |
| Controladores | 5 | Uno por miembro de la familia y uno compartido por las tres formas de Algoritm |

Ningún clip es de salto, caída ni derrota. Todo el arte de los personajes suma 16,3 MB en memoria. Las cifras de la Tabla F.2 y ese peso son las del corte. Desde el 05/10/2026 hay cinco imágenes más del Niño (cabeza, dos antebrazos y dos antepiernas) y sus cinco partes se sustituyeron por las del arte final; los prefabs siguen siendo siete y los clips 93, con las curvas reescritas (apartados 2.6 y 2.7). Desde el 06/10/2026 Papá, Mamá y la Niña tienen también sus piezas finales, 14 sprites cada uno (11 el Niño), y un juego de cara por personaje (apartado 2.10). Ese arte no se ha medido en un ejecutable.

### 2.2 Personajes en las narrativas

En una secuencia narrativa un personaje es un `NarrativeProp` con un `Actor` (el prefab), una acción de salida (reposo, u oculto para Algoritm antes de aparecer) y sus `Beats`, los pasos por línea de diálogo, cada uno con su acción, su destino en fracciones de la ilustración, su duración y la acción al llegar. Como objeto de la narrativa hereda la casilla, el orden de dibujo, el paneo de la cámara y la prueba que exige que lo nombrado no quede bajo el cuadro de diálogo (RNF-03).

`ActorTimeline`, una clase estática en C# plano, decide qué hace cada personaje en cada línea y entrega a `NarrativeSceneController` una indicación ya resuelta (`ActorCue`). Sus reglas son tres. Entre un paso y el siguiente el personaje conserva el sitio y la última acción. Quien dice la línea de pie y en reposo gesticula. Quien camina termina su camino aunque el texto avance, salvo que un paso nuevo lo interrumpa, en cuyo caso salta a su destino antes de empezarlo.

Esa tercera regla hacía saltar a los viajeros de la escena 3.3 si se pulsaba «Continuar» durante los 9 s del cruce. Desde el 01/10/2026 una marca del asset, `NarrativeProp.FinishesSteps`, señala a quien termina sus pasos: a ese personaje ningún paso nuevo le corta el camino (`ActorTimeline.Interrupts`), y lo que se lee mientras camina lo hace al llegar, empezando por el último paso con movimiento que se leyó entretanto (`ActorTimeline.PendingStep`). La llevan solo Papá, Mamá, la Niña y el Niño de `N3_Escena33_Cruce`; las otras diecisiete narrativas siguen la regla de siempre. Una réplica cuadro a cuadro del cruce dio cero saltos en 3 000 ritmos de lectura simulados. Con la misma corrección (INC-124), los cuatro bajaron 0,06 con la balsa y viajan en su sitio sobre ella (Anexo D, apartado 2.4).

El retrato del cuadro de diálogo es el del personaje en escena que dice la línea; si solo se oye su voz, sale del reparto de la escena (`cast`) y, para Algoritm, con la forma del nivel (`guideByLevel`). Las acotaciones no llevan retrato. Desde el 01/10/2026 la dirección de arte y el documento de interfaces describen ese cuadro fijo con retrato en lugar de un globo con cola (INC-129). Las dieciocho secuencias recibieron personajes, pasos y objetos tomados de sprites existentes, cada entrada con la cita del guion que la justifica; las fogatas del Nivel 3 y su humo son del carril de arte (Anexo E, apartado 2.1).

### 2.3 Personajes en las mecánicas

Cada controlador de escena jugable llama a `Play` o a `PlayFor` en sus puntos de enganche. Tras un intento sin avance el personaje jugable solo anima (`Encourage`), nunca otra cosa (CP-02).

**Tabla F.3.** Personajes en las cinco escenas jugables.

| Escena | Quién | Qué hace |
|---|---|---|
| `Level1_Cave` | Papá junto al círculo; la familia atrás | Papá recoge al tomar una pieza, golpea, anima tras un golpe sin chispa, sopla y se queda arrodillado; el Niño, inquieto, observa |
| `Level2_Forest` | La Niña junto a la caja; la familia al pie de los árboles | La Niña señala el tronco aceptado, anima tras elegir un distractor y empuja con la caja llena; todos celebran el acopio |
| `Level2_Workshop` | La familia detrás del banco | La Niña señala y martilla, Papá martilla con ella en el montaje y todos celebran la carretilla |
| `Level2_Maze` | La Niña junto a la salida; la familia en el refugio | La Niña observa, señala al ejecutar y anima si la carretilla no llega; al llegar celebran todos, sin tomar el tinte de atardecer del entorno |
| `Level3_River` | Mamá; la familia detrás de la zona de construcción | Mamá camina mientras se sostiene una flecha y recoge; la familia anima si falta algo o la balsa se hunde y celebra cada fase aprobada |

En las cinco escenas el botón de ayuda es el círculo del mockup con el Algoritm del nivel: fuego, rueda o gota.

### 2.4 Estela de Algoritm y sombra de contacto

El 30/09/2026 (`37b3cb7`) Algoritm ganó la estela de puntos de luz que piden el guion y la dirección de arte (INC-52). `GuideTrail` (`Game.Scaffolding`) cuelga de las tres formas como hijo `Estela` y dibuja de cinco a siete puntos de color `#FFE9A8` con escala y alfa decrecientes hacia atrás; `TrailPath`, en C# plano, guarda las últimas posiciones y olvida la estela cuando el cuerpo se queda quieto. La primera versión muestreaba la raíz del personaje, que en una narrativa no se mueve porque lo que camina es su casilla, y la estela no llegaba a verse; la corrección del mismo día la hizo seguir a la casilla. Ningún punto recibe clics (RNF-02) y se apagan solo por alfa, sin parpadeo (RNF-21).

La sombra de contacto (INC-109) es un hijo `Sombra` del lienzo de Papá, Mamá, la Niña y el Niño: una imagen negra al 25 % de opacidad, dibujada detrás del cuerpo y sin recibir clics. Algoritm flota y no la lleva. El mismo commit retiró `CharacterRig.Tint`, un método sin llamadores que habría teñido al personaje con la luz del laberinto y anulado la opacidad de la sombra. Estela y sombra entraron como hijos nuevos, sin reconstruir los prefabs, de modo que las referencias de las escenas y de los dieciocho assets no cambiaron.

### 2.5 La familia del río a la escala de las narrativas

En la mecánica del Nivel 3 la familia se veía a cuatro décimas de la escala con que la muestran las narrativas del mismo río. Con la tarjeta D10-3, el 01/10/2026 el plano se abrió (Anexo D, apartado 2.2) y la familia y Mamá crecieron hasta la escala de la escena 3.1 (INC-118). Los valores se fijaron al implementar la decisión del acta D10, cambiando tamaño, ancla y pivote de las instancias de `Level3_River` sin reconstruir prefabs.

**Tabla F.4.** Casillas de los personajes en `Level3_River`, en unidades de la ilustración, y su tamaño en pantalla a 1920 × 1080.

| Personaje | Hasta el 30/09/2026 | Desde el 01/10/2026 | En pantalla |
|---|---|---|---|
| Papá | 77,7 | 178,75 | De 148 a 250 px |
| Niña y Niño | 46,6 | 107,25 | De 89 a 150 px |
| Mamá | 104, con pivote centrado | 240, anclada por los pies en (0,5; 0,075) | De 121–185 a 193–315 px, según su altura en la orilla |

Papá, la Niña y el Niño quedan en 0,933 del tamaño con que los pinta la escena 3.1, y Mamá, de pie en la zona de construcción, en 0,988. Mamá se ancla por los pies, como la familia, para que su área transitable sea el pasto que pisa y su escala por profundidad se mida donde apoya. La familia espera detrás de la zona, y Mamá espera en la zona al retomar el ensamblaje desde la escena 3.2, desde disco o al reiniciar la fase: `RiverSceneController.ResumeAt` la coloca allí junto con su modelo de marcha (`RiverWalk.MoveTo`). Cada vez que Mamá se coloca, ella, la familia y los materiales se ordenan por altura y lo que está más abajo se dibuja delante (`DepthOrder`, C# plano, que a igual altura conserva el orden previo).

### 2.6 Rig articulado y caras (INC-131)

El 05/10/2026 Santiago Benavides Rey decidió que los personajes finales llegan en vista frontal y con más capas: la cabeza separada del torso, ojos y boca en capas propias con seis expresiones y las extremidades en dos sprites, con codos y rodillas además de hombros, cadera y cuello (INC-131). Algoritm entra en la decisión con brazos y piernas en dos tramos y con ojos y boca sobre el cuerpo; no lleva cuello, porque la llama no tiene cabeza aparte. Ninguna expresión es de tristeza, enfado ni derrota (CP-02): `Worried` es la duda de quien pregunta o espera. La decisión levanta INC-108 y la regla de la imagen única de Algoritm solo para el arte final; para el arte provisional ambas siguen valiendo. El código y el generador entraron con `a12dbb3`, y `e9eb6cb` subió los prefabs y los clips regenerados en el Editor.

La primera causa de la pose en T se corrigió antes, el 02/10/2026 (`d752bef`): al reactivar la jerarquía de un personaje, el `Animator` podía pintar un cuadro con los brazos en cruz. `CharacterRig` ganó `OnEnable` y `Start`, que lo re-sincronizan con `Play` y `Update(0)`, y no da el arranque por hecho si el `Animator` está inactivo. La segunda causa estaba en los clips: los de hablar, señalar o animar dejaban huesos sin curva y, con `writeDefaultValues`, un hueso sin curva vuelve a la rotación del prefab, que es la pose en T. La regeneración de `e9eb6cb` reescribió los 93 clips en sitio con una curva de rotación en cada hueso de su personaje (doce en la familia y diez en Algoritm); el hueso que el clip no usa lleva una curva constante en su pose de reposo. Se conservaron los GUID, los controladores y los eventos.

Los nodos nuevos son lo que resume la Tabla F.9. Ninguno de los 182 objetos que ya existían cambió de identificador, y los 321 nuevos nacieron con su `Image` apagada y sin sprite, porque una `Image` sin sprite pinta un recuadro blanco, y sin `raycastTarget` (RNF-02). Codo, rodilla y cuello son pivotes de tamaño cero colocados en la articulación; el segmento cuelga de ellos, y girar el pivote gira el segmento. Mientras no llega el arte, el personaje se ve igual que al corte.

**Tabla F.9.** Nodos que el generador `BuildRigsFinal` añadió a los siete prefabs (`e9eb6cb`).

| Prefab | Nodos nuevos | Cuántos por prefab |
|---|---|---|
| `Papa`, `Mama`, `Nina` y `Nino` | `CodoIzq`, `CodoDer`, `AntebrazoIzq`, `AntebrazoDer`, `RodillaIzq`, `RodillaDer`, `AntepiernaIzq`, `AntepiernaDer`, `Cuello`, `Cabeza`, `Ojos`, `Boca` y el componente `CharacterFace` | 13 |
| `Algoritm_Fuego`, `Algoritm_Rueda` y `Algoritm_Gota` | `PiernaIzq`, `PiernaDer`, `RodillaIzq`, `RodillaDer`, `AntepiernaIzq`, `AntepiernaDer`, `Tronco`, `BrazoIzq`, `BrazoDer`, `CodoIzq`, `CodoDer`, `AntebrazoIzq`, `AntebrazoDer`, `Torso`, `Ojos`, `Boca` y `CharacterFace` | 17, sin `Cuello` ni `Cabeza` |

En Algoritm, `Cuerpo` sigue siendo una sola `Image` con el sprite entero, y es lo que se ve hasta que lleguen las partes.

Dónde cae cada nodo lo fija `rig_articulaciones.json`, que `articulaciones.py` estima leyendo los prefabs, sin abrir ninguna imagen: el codo en el punto medio entre el hombro y la esquina más lejana del rectángulo del brazo, la rodilla entre la cadera y la base de la pierna, el cuello en la base de la cabeza. Sus valores son estimaciones y se sustituyen por las medidas del arte final. Las coordenadas son las de `cut.py`: lienzo de 1024 × 1024, origen arriba a la izquierda y suelo en y = 947. El generador lee el JSON con un lector propio (`980936d`), porque `JsonUtility` devolvía la tabla vacía. Un personaje está segmentado cuando ya tiene sus piezas en dos tramos, y eso se deduce del prefab y no se guarda (apartado 3).

La cara vive en `Game.Scaffolding`. `FacialEmotion` tiene seis valores (`Neutral`, `Happy`, `Surprised`, `Worried`, `Focused` y `Sleeping`), con número explícito porque los assets lo guardan. `ActionEmotion.For` da la emoción por defecto de cada acción, y así las cinco mecánicas ganan expresión sin tocar sus controladores: celebrar, abrazar y animar son `Happy` (el ánimo llega tras un intento sin éxito y su cara es alegre), sorprenderse es `Surprised`, dormir es `Sleeping`, golpear, martillar, soplar, empujar, cargar, recoger y arrodillarse son `Focused`, y el resto es `Neutral`. `BlinkClock` parpadea cada 3,5 ± 1,2 s durante 0,12 s, pasando por medio cerrado, cerrado y medio cerrado, y se apaga en `Sleeping`. `MouthFlap` cicla las bocas A, E, U y cerrada, 0,09 s cada una y en orden fijo, y al callar cierra en seco a la boca de reposo de la emoción. `CharacterFaceSet` es el ScriptableObject con los sprites de ojos y bocas y sus tiempos (CT-05), uno por personaje y por forma de Algoritm. `CharacterFace` pone esos sprites en las dos `Image`; sin set, o sin el sprite pedido, deja la `Image` desactivada, y avanza con tiempo escalado, así que la pausa (RF-07) congela el parpadeo y la boca.

`CharacterRig` ganó `EmotionOverride`, `Emotion` y `Speaking` sin cambiar sus firmas públicas. En las narrativas, `ActorBeat` ganó `SetsEmotion` y `Emotion`, `ActorTimeline.EmotionAt` mantiene la emoción de un paso hasta que otro la cambia, y `NarrativeSceneController` la aplica y marca como hablante al rig de quien dice la línea; una acotación no mueve ninguna boca. `SetsEmotion` vale `false` por defecto, así que las dieciocho secuencias no cambiaron de comportamiento.

### 2.7 Arte final del Niño frontal

El primer arte final en entrar fue el del Niño en vista frontal (`d127ec9`, 05/10/2026). La entrega eran diez piezas: torso, cabeza, dos brazos, dos antebrazos, dos piernas y dos antepiernas. Se llevaron a una escala común, se limpiaron las motas de alfa que inflaban el muslo y la mano izquierdos, se renombraron según la convención de los resultados (la mano pasó a `antebrazo` y el pie a `antepierna`) y se escribió en `rig_articulaciones.json` el rectángulo y el pivote reales de cada una. Con eso el generador asignó los sprites a `Nino.prefab` sin tocar un identificador y reescribió ocho de sus clips (arrodillarse, caminar, cargar, correr, dormir, empujar, recoger y soplar). Desde ese commit el Niño es el único personaje segmentado. La Figura F.1 muestra su Idle con ese arte.

![Figura F.1. Idle del Niño con su arte final en ocho instantes. Vista previa generada con `pose_preview.py`, no captura del motor.](fig/fig-F-1-idle-nino.jpg){width=16cm}

La geometría de esa entrega tuvo una corrección el mismo día (`8740a64`, regenerada en `6f2b4cf`). El hombro del Niño pivotaba en el centro del pecho y los brazos no salían de la articulación. Ahora el hombro y el codo están en el centro del extremo redondo de cada cápsula: los hombros en x = 466, y = 560 y x = 556, y = 560; los codos en x = 366, y = 609 y x = 667, y = 605, en el lienzo de 1024 × 1024 de `cut.py`. Las rodillas pasaron a x = 483, y = 845 y x = 558, y = 842, los ojos al rectángulo (402, 380, 602, 470) y la boca a (447, 468, 557, 512). En el prefab, esos nodos cambiaron solo de `RectTransform`.

Al cierre de ese commit el Niño no tenía ojos ni boca, porque ningún sprite de cara estaba asignado; su cara entró el 06/10/2026 (apartado 2.10).

### 2.8 Orden de dibujo de los brazos (INC-132)

Santiago Benavides Rey vio un Idle frontal pobre y brazos escondidos detrás de la cabeza o del cuerpo. La causa estaba en el prefab y no en los clips: bajo `Lienzo/Cuerpo/Tronco` los hijos iban `BrazoIzq`, `BrazoDer`, `Torso` y `Cuello`, y uGUI pinta de atrás adelante, de modo que los brazos quedaban al fondo. La decisión del 05/10/2026 (INC-132) fijó el orden de la Tabla F.10 y sustituyó, para la familia, la regla de INC-131 de dibujar el cuello sobre el torso. Con el arte provisional la cabeza está pintada dentro del torso, así que hoy los brazos de la familia quedan tras el torso y tras la cara hasta que llegue el arte final.

**Tabla F.10.** Orden de dibujo de los hijos de `Tronco`, de atrás adelante, desde `0b76bbc` y `a5a887b`.

| Personaje | Orden | Qué se ve |
|---|---|---|
| Papá, Mamá, la Niña y el Niño | `Cuello`, `BrazoIzq`, `BrazoDer`, `Torso` | Los brazos quedan detrás del torso y delante de la cabeza, que cuelga de `Cuello`, al fondo |
| Algoritm, en sus tres formas | `Torso`, `Ojos`, `Boca`, `BrazoIzq`, `BrazoDer` | Los brazos quedan delante del cuerpo y las manos encima de la cara |

El orden pasó por etapas el mismo 05/10/2026: brazos delante en el Niño y en Algoritm (`8740a64`, `6f2b4cf`), delante en los siete (`590866f`, `5ce0797`) y, por fin, la regla de la tabla (`0b76bbc`, `117287c`). El modo `orden` del generador lo aplica con `SetSiblingIndex`, sin crear ni borrar nodos, y por eso no cambia ningún identificador. Con el orden definitivo, `117287c` subió cuatro prefabs, de los que solo cambió el orden de `Tronco`, y 77 clips (Papá 21, Mamá 16, la Niña 21 y el Niño 19).

El 06/10/2026 Santiago Benavides Rey añadió dos ajustes. En Algoritm, la coreografía y `pose_preview.py` impiden que una mano entre en la caja de ojos y boca, ampliada un 30 %, con al menos el 99,5 % libre. Y mientras un personaje de la familia golpea las piedras (`Strike`), los brazos pasan delante del torso (Figura F.2) y al cambiar de acción vuelven exactamente a su orden. Lo hace `ArmLayering`, una clase de C# plano que reordena los hijos de `Tronco` por nombre y restaura el orden de origen, y `CharacterRig.Apply` la llama según el campo serializado `armsInFrontActions`, cuyo valor es `{ Strike }`. El cambio de capa es seco al empezar la acción y no espera el fundido de 0,18 s; si falta un nodo, avisa y no mueve nada; en Algoritm no mueve nada, porque sus brazos ya van delante. Esos ajustes entraron con `a5a887b` y, ya regenerados en el Editor, con `8cdd4c5`: tres prefabs de Algoritm (el orden de `Tronco` y el campo serializado) y los cuatro clips de golpear de la familia.

![Figura F.2. Orden de dibujo: el Niño en reposo, el Niño en el choque de `Strike` con los brazos delante del torso y Algoritm con las manos sobre la cara. Vistas previas de `pose_preview.py`, no capturas del motor.](fig/fig-F-2-orden-de-dibujo.jpg){width=16cm}

La coreografía del golpe tiene un punto sin aprobar. Con el brazo partido, el choque ocurre delante del pecho. Con el brazo de una pieza del arte provisional (Papá, Mamá y la Niña hasta el 06/10/2026), el brazo se encoge hasta el 45 % durante el golpe para que el choque quede en el vientre y nunca bajo la cintura: por encima de la cabeza tapaba la cara, y a un costado no había contacto (Figura F.3). Santiago Benavides Rey aún no la aprueba; la alternativa es golpear sin juntar las manos hasta que llegue el arte final. Con `433603f` los cuatro tienen el brazo partido y ese caso ya no se da en la familia (apartado 2.10).

![Figura F.3. Golpe de `Strike`: el Niño, Papá con arte final simulado por `maqueta.py` y Papá con el arte de hoy y el brazo encogido. Vistas previas de `pose_preview.py`, no capturas del motor.](fig/fig-F-3-golpe.jpg){width=16cm}

### 2.9 Coreografía fuera de Unity y entrada del arte final

Hasta INC-132 las curvas de los clips las calculaba el C# del generador. Desde `8740a64` las escribe `coreografia.py` en `clips_personajes.json`, y el modo `clips` solo las aplica; no crea clips ni toca controladores, GUID ni eventos. Así hay una sola fuente de verdad, que se puede probar sin abrir el Editor. El primer paso fue un port del motor de clips: `coreografia_v0.py` se conserva solo para la regresión, y el port reproduce los 93 `.anim` anteriores con una diferencia máxima de 0,00002.

La coreografía se escribió una vez, para el arte final, como objetivos de la mano. Con el brazo partido se resuelve con cinemática inversa de dos tramos; con el brazo de una pieza, el hombro apunta la mano y el codo lleva ya su curva. La visibilidad de cada brazo se deriva de la geometría, con la silueta del torso, y el reposo se calcula por personaje. El Idle dura 6,4 s multiplicados por el tempo del personaje: dos respiraciones de 3,2 s, que conservan el ciclo de la dirección de arte (apartado 13.3), con un gesto de carácter por personaje. El Niño mira a los lados, rebota, balancea los brazos y se rasca junto a la oreja; la Niña balancea los brazos; Papá pone los brazos en jarra por fuera del torso e hincha el pecho; Mamá lleva las manos a la cintura y una mano a la oreja, por delante del pelo y sin tapar ojos ni boca; Algoritm parpadea como llama, con brazos alternos. `Encourage` es un puño fuera de la cabeza, con rebote y cabeceo, de 0,9 s multiplicados por el tempo (CP-02); los controladores de las mecánicas usan 0,9 s y 1,035 s. `Blow` es agachado, con tres soplos, y `Observe` lleva la visera desde la sien (el Niño, con brazos cortos y cabeza grande, no llega a la frente).

`pose_preview.py` renderiza el rig fuera de Unity a 30 cuadros por segundo y prueba cada clip contra las medidas de la Tabla F.11. Lee `armsInFrontActions` de `CharacterRig.cs` y dibuja cada clip con su orden. Tiene dos modos: `--hoy`, con el arte que hay en los prefabs, y `--maqueta`, con el arte final simulado por `maqueta.py`, que corta el provisional por las articulaciones de la tabla. `prefabs.py` simula lo que hará el modo `sprites`: quien ya tiene sus piezas se trata como segmentado, y simula también el orden de dibujo. En ambos modos pasó 174 de 174 comprobaciones (`a5a887b`); en su primera versión, `8740a64`, eran 111 de 111.

**Tabla F.11.** Comprobaciones de `pose_preview.py` sobre cada clip.

| Medida | Umbral |
|---|---|
| Brazos visibles | Al menos el 85 % |
| Cara visible | Al menos el 90 % |
| Extremidades | Sin hiperextensión |
| Pies | A lo sumo 2 px bajo el suelo |
| Manos | Fuera de la caja de la cara, salvo excepciones explícitas (hoy la lista está vacía) |
| Velocidad de giro | Hasta 1 300 °/s, salvo el martillazo |
| Torso sobre ojos y boca | No tapa más del 5 % |
| Algoritm | Ninguna mano entra en la cara |
| Choque de `Strike` | Queda sobre la cintura |

El arte final de cada personaje entra con `preparar_arte_final.py <id> <carpeta>`. Reconoce las capas por nombre (el lado, por su posición en el lienzo), descarta duplicados, limpia el alfa (umbral 12, componente mayor), normaliza la figura a 870 unidades con el torso en x = 512 y mide hombro, codo, cadera y rodilla sobre el alfa. Sin `--aplicar` entrega un informe y un composite (Figura F.4); con `--aplicar` escribe los PNG con los nombres de la convención, `arte_final.json`, `rig_articulaciones.json` y `clips_personajes.json`, y las órdenes para la sesión local. Su `--autoprueba` usa una entrega sintética del Niño a otra escala, con motas y un duplicado, y exige un error de medida de hasta 1 px; corrida el 06/10/2026 sobre el árbol de trabajo de la rama, pasa con un error máximo de 1,0 px.

![Figura F.4. Composite que genera `preparar_arte_final.py` al reconocer las capas de una entrega, previo a `--aplicar`. No es una captura del motor.](fig/fig-F-4-entrada-arte-final.jpg){width=16cm}

Con el arte ya preparado, la sesión local copia `BuildRigsFinal.cs.txt` a `Assets/Editor/ClaudeBuildRigsFinal.cs`, recompila y corre los modos de la Tabla F.12 en ese orden; después compara los prefabs objeto por objeto, corre las pruebas y borra el andamiaje.

**Tabla F.12.** Modos de `BuildRigsFinal`.

| Modo | Qué hace |
|---|---|
| `nodos` | Añade los nodos de la tabla que falten y el `CharacterFace`; nunca borra ni recrea uno |
| `sprites` | Asigna por nombre los PNG que existan, aplica rectángulo y pivote de la tabla, enciende las `Image` y crea el `CharacterFaceSet`; lo que no existe queda apagado y se anota |
| `orden` | Reordena los hijos de `Tronco` según `orden_tronco` de la tabla |
| `clips` | Aplica `clips_personajes.json` a los 84 clips de la familia y los 9 de Algoritm; se niega si faltan los nodos y se corre después de `sprites` y `orden`, porque lee del prefab si el personaje está segmentado |
| `todo` y `estado` | `todo` corre los cuatro anteriores en orden; `estado` solo informa, sin escribir |

### 2.10 Arte final de la familia, caras por registro y antebrazo delante (INC-133)

El 06/10/2026 llegó la entrega de Papá, Mamá, la Niña y el Niño: 45 PNG en carpetas `Frente` y `Expresiones` (2,4 MB), que `d26ea8f` guardó con sus originales en `claudeDocs/tasks/Personajes/entregas/2026-10-06/`, fuera de `Assets`. `OrganizarArtePersonajes.cs.txt` repartió el arte de cada personaje en tres subcarpetas, `Frontal/`, `Expresiones/` y `Perfil/`, usando solo `AssetDatabase`: `64e4b37` movió 25 PNG con los GUID intactos, y `Perfil/` queda vacía hasta que haya arte. En `63ed2fc`, `CharacterFaceSet` cae a los ojos neutros y la boca cerrada cuando una emoción no tiene sprite propio, de modo que una sola expresión entregada ya muestra cara en todas las acciones, y `CaraBase` pasó a ser el primer hijo de `Cabeza`. Lo vigilan `CharacterFace_DA73_SinLaExpresionDeLaEmocionUsaLaNeutra`, `CharacterFace_DA73_UnaEmocionAMediasCompletaConLaNeutra` y `CharacterRig_DA131_LaCaraBaseVaDetrasDeLosOjosYDeLaBoca`.

Las caras de los cuatro se colocan por registro (`233f2e5`). Los lienzos de 1300 × 1500 píxeles de la entrega están registrados entre sí, de modo que `preparar_expresion.py --registrada` coloca `Ojos`, `Boca` y `CaraBase` con la misma transformación con que `preparar_arte_final.py` colocó las partes de ese personaje, guardada como `registro` en `arte_final.json`. Sustituye la heurística de escala de `a538ab6`, que fijaba los ojos en el 60 % del óvalo de la cara. El Niño coincide con la foto de Santiago Benavides Rey dentro del 0,6 % de la altura: sus ojos pasaron de 188 a 295 px de ancho y los brazos quedaron entre 20 y 25 px hacia adentro, como en la entrega. Se sustituyeron en su sitio 12 PNG de `Expresiones/`, con los mismos nombres, `.meta` y GUID.

INC-133 (06/10/2026) fija en la familia el húmero detrás del torso y el antebrazo delante del torso, de la cara y de las piernas, y Papá con la cabeza (`Cuello`) delante del torso. uGUI pinta por orden de jerarquía, así que el antebrazo no podía quedar delante bajo un brazo que va detrás. `AntebrazoIzq` y `AntebrazoDer` (mismo objeto y mismo identificador) pasan a ser hijos de `Tronco`, al final, y siguen a un nodo vacío, `AnclaAntebrazoIzq` o `AnclaAntebrazoDer`, que queda bajo cada codo con la pose local anterior del antebrazo. `LimbFollower`, una clase de C# plano de `Game.Scaffolding`, compone `BrazoX`, `CodoX` y el ancla respecto de `Tronco` y escribe la pose local del antebrazo; `CharacterRig` descubre los pares por nombre y los sincroniza tras `Play` más `Update(0)` y en `LateUpdate`. No hay campos serializados nuevos y los clips no cambian, porque no animan el antebrazo. `ArmLayering` conserva su lógica: al golpear sigue poniendo el húmero justo tras `Torso`, todavía detrás de los antebrazos. Los personajes sin anclas, entre ellos Algoritm, no se tocan. `pose_preview.py` mide aparte la visibilidad del húmero (al menos el 40 %) y la del antebrazo (al menos el 85 %).

La ronda del Editor de `433603f` (06/10/2026) corrió el generador sobre los cuatro prefabs de la familia. El modo `sprites` asignó 14 sprites a Papá, Mamá y la Niña y 11 al Niño, y creó los juegos de cara `char_papa_cara`, `char_mama_cara` y `char_nina_cara`, con la expresión neutra hecha de `ojos_neutra` y `boca_0`. El modo `orden` creó las dos anclas bajo los codos, movió los antebrazos al final de `Tronco` y puso a Papá su `Cuello` tras `Torso`; el modo `clips` reescribió 93 clips. Comparados objeto por objeto contra el commit anterior, cada prefab de la familia ganó cuatro objetos nuevos, las dos anclas con su RectTransform, y no perdió ninguno, con `m_Script` y `m_Controller` iguales; Algoritm no cambió, y una segunda orden `orden` no guardó nada. Además, 24 `.meta` de PNG pasaron a `Single` (`spriteMode` de 2 a 1) con los GUID iguales. Las cifras de pruebas están en la Tabla F.13. Las capturas del 06/10/2026 están en el repositorio, en `claudeDocs/tasks/Personajes/capturas/2026-10-06/`, y no se incrustan: la familia en reposo en la escena 2.2 (`01` y `01b`), Papá en el Nivel 1 (`02`, `02b`, `02c` y `05`), Mamá hablando (`03`) y la Niña hablando (`04`). No hay cuadro del choque de Papá en `Strike`.

## 3. Decisiones de diseño y hallazgos de consistencia

En cuanto al recorte en uGUI (INC-53), la dirección de arte pedía animar con el paquete 2D Animation. Las escenas del juego son de uGUI en un lienzo superpuesto a la pantalla, y ese paquete solo deforma un `SpriteRenderer`, que quedaría debajo del lienzo, tapado por la ilustración; pasar las escenas a un lienzo de cámara rompía supuestos del código y de las pruebas. Se optó por partes `Image` giradas y desplazadas por un `Animator`, que conservan lo pedido: animación sobre el sprite base y no por hojas de fotogramas, como fijó el acta D06, respiración en reposo y ningún salto, caída ni derrota.

La herramienta que armó los rigs reconstruye prefabs, clips y controladores, y con el argumento `clips` solo reescribe los clips. Reconstruir un prefab cambia los identificadores de sus componentes y rompe las referencias de `Narrative.unity`, de los dieciocho assets y de las cinco escenas; por eso los retoques posteriores añadieron hijos o reescribieron clips.

El 24/09/2026 se revisaron las 138 capturas del motor de cada línea narrativa y de las cinco mecánicas. Se corrigieron clips (arrodillarse y dormir bajan casi al suelo, abrazar abre los brazos, celebrar no pasa de unos 100° para que las manos no queden detrás de las cabezas, señalar va casi horizontal, observar lleva las manos a la cintura) y la puesta en escena (personajes que pisaban objetos o flotaban sobre las rocas, materiales de la 3.1 que no apoyaban en el suelo, Algoritm pegado a la mano de Papá).

La marca de los viajeros no se extendió: el salto de la escena 3.3 existe en otras catorce narrativas y marcarlas habría cambiado la escenificación de escenas ya revisadas, de modo que la corrección se limitó al cruce, donde el salto se veía sobre la balsa en movimiento.

Tres decisiones técnicas del arte final no tienen INC propio. Solo se añaden nodos y nunca se reconstruye un prefab: el generador abre cada prefab, le añade hijos y componentes y lo guarda en la misma ruta, y reescribe las curvas de los clips cargados con `LoadAssetAtPath` sin tocar los controladores, de modo que siguen válidas las referencias de las dieciocho secuencias y de las cinco escenas; las cuatro rondas de `6f2b4cf`, `5ce0797`, `117287c` y `8cdd4c5` informaron intactos los identificadores, `m_Script`, `m_Controller` y `m_Sprite` de los siete prefabs. «Segmentado» se deduce del prefab, porque un campo por personaje en la tabla de articulaciones sería una segunda fuente de verdad que se desincroniza con el primer cambio de arte; un personaje es segmentado cuando la `Image` de `AntepiernaIzq` tiene sprite y está encendida, y de eso depende cómo se flexionan las rodillas en los clips. La coreografía vive en Python y no en el C# del generador, para que haya una sola fuente de verdad que se prueba sin el Editor (apartado 2.9).

**Tabla F.5.** Hallazgos de consistencia del incremento.

| INC | Qué decidió | Documentos corregidos o implementación | Fecha de cierre |
|---|---|---|---|
| INC-45 | El guía cambia de forma en cada nivel: fuego, rueda y gota | Guion | 14/09/2026 |
| INC-52 | Algoritm es una llama con extremidades, recoloreada en madera y agua, que deja una estela de luz | Guion, apartados 1.1.1, 1.4.1, 1.4.4, 1.5 y 1.7; dirección de arte, apartados 7.6 y 18; interfaces, apartado 4.3; dirección de sonido; índice de sprites. En el juego, acotaciones sin «estrella» y estela (30/09/2026) | 29/09/2026 |
| INC-53 | Los personajes se animan por recorte con `CharacterRig` | Dirección de arte, apartados 7.5, 9.1, 13.1, 13.3 y 15.4; índice de sprites | 29/09/2026 |
| INC-108 | Un retrato por personaje con una sola expresión; la emoción la lleva el cuerpo | Interfaces, apartados 4, 4.2 y 5; dirección de arte, apartados 7.3 y 12.3; índice de sprites | 29/09/2026 |
| INC-109 | La familia lleva sombra de contacto | Implementada en los cuatro prefabs; `CharacterRig.Tint` retirado (30/09/2026) | 29/09/2026 |
| INC-118 | La familia y Mamá se ven en la mecánica del río a la escala de las narrativas | Dirección de arte, apartados 5.3 y 8.3; índice de sprites; interfaces, apartado 1.1; casos del OE4; tarjeta D10-3 | 01/10/2026 |
| INC-124 | Los viajeros bajan con la balsa del cruce | Dirección de arte, apartados 8.3 y 9.3; índice de sprites; interfaces, apartado 1.1; `N3_Escena33_Cruce.asset` | 01/10/2026 |
| INC-129 | El diálogo se lee en un cuadro con retrato | Dirección de arte, apartados 4.3, 10.3 y 11.4; interfaces, apartado 1.1 | 01/10/2026 |
| INC-131 | El arte final trae seis expresiones y extremidades en dos tramos, con codos y rodillas; levanta INC-108 y la imagen única de Algoritm solo para el arte final | Dirección de arte, apartados 7.3, 7.6, 13.1, 15.4 y 17; interfaces, apartados 4 y 4.2; índice de sprites; plan de personajes finales; en el juego, `a12dbb3` y `e9eb6cb` | 05/10/2026 |
| INC-132 | En la familia los brazos se dibujan detrás del torso y delante de la cabeza, y al golpear delante del torso; en Algoritm, delante del cuerpo con las manos sobre la cara | Plan de personajes finales y resultados (apartado C.9); en el juego y las herramientas, de `8740a64` a `8cdd4c5` | 05/10/2026 |
| INC-133 | En la familia el húmero se dibuja detrás del torso y el antebrazo delante del torso, de la cara y de las piernas; Papá lleva la cabeza delante del torso | Plan de personajes finales y resultados (apartado C.10); en el juego y las herramientas, de `ef37dd9` a `433603f` | 06/10/2026 |

INC-131, INC-132 e INC-133 son decisiones de Santiago Benavides Rey posteriores a la D10, sin acta, y constan en `INCONSISTENCIAS.md`.

## 4. Verificación

### 4.1 Pruebas que lo nombran

`88fe0ee` añadió 50 métodos de prueba, que el 25/09/2026, contados sobre `ccf77e6`, sumaban 106 casos (32 en EditMode y 74 en PlayMode). En los nombres, `DA133`, `DA76`, `DA53` y `DA83` citan los apartados 13.3, 7.6, 5.3 y 8.3 de la dirección de arte. Estas pruebas viven en los módulos de `Game.Scaffolding`, de `Game.UI` y de los tres niveles, cuyas cifras del corte da la Tabla 9.1 del documento principal.

**Tabla F.6.** Pruebas representativas de los personajes. Las siete últimas filas son posteriores al corte.

| Qué se vigila | Prueba representativa | Modo |
|---|---|---|
| Ningún clip de derrota, caída ni salto (CP-02) | `CharacterRig_CP02_NingunClipEsDeDerrotaCaidaNiSalto` | EditMode |
| Un estado por acción; las partes no reciben clics | `CharacterRig_DA133_CadaPersonajeTieneUnEstadoPorAccion`, `CharacterRig_RNF02_NingunaParteDeUnPersonajeRecibeClics` | EditMode |
| Retrato de quien habla (RF-05) | `NarrativeScene_RF05_ElRetratoEsElDeQuienHablaYNoHayEnLasAcotaciones` | PlayMode |
| Reglas de las narrativas | `ActorTimeline_RF05_*` y `ActorTimeline_RNF21_*`: 17 métodos y 24 casos | EditMode |
| Cada personaje hace lo que dice su paso | `NarrativeScene_RF05_CadaPersonajeHaceLoQueDiceSuPasoCuandoSeLeeLaLinea`, un caso por secuencia | PlayMode |
| Los viajeros de la 3.3 no saltan | `NarrativeSequence_RF44_SoloLosViajerosDelCruceTerminanSusPasos`, `NarrativeScene_RNF21_AvanzarElTextoDuranteElCruceNoHaceSaltarALosViajeros` | EditMode y PlayMode |
| Tras un fallo, solo ánimo | `FireLevel_CP02_TrasUnGolpeSinChispaPapaSeAnimaYNuncaHaceUnGestoDeDerrota`, `AssemblyPanel_DA133_CuandoLaBalsaSeHundeLaFamiliaAnimaYNingunoHaceOtroGesto` | PlayMode |
| Estela y sombra | `GuideTrail_DA76_LaEstelaSigueALaCasillaQueCaminaYSeBorraAlDetenerse`, `TrailPath_DA76_OlvidaLaEstelaSiElCuerpoSeQuedaQuieto`, `CharacterRig_DA53_LaFamiliaLlevaSombraDeContactoBajoLosPies` | EditMode |
| Escala, ancla y orden del río | `RiverScene_INC118_LosPersonajesDeLaMecanicaTienenLaEscalaDeLaNarrativa`, `RiverScene_DA83_MamaSeAnclaPorLosPiesComoLaFamilia`, `DepthOrder_DA83_LoQueEstaMasAbajoSeDibujaDelante`, `RiverScene_RNF14_AlRetomarElEnsamblajeMamaEsperaEnLaZona` | EditMode y PlayMode |
| Codos, rodillas, cuello y cara del rig (INC-131) | `CharacterRig_DA131_LaFamiliaTieneCodosRodillasYCuello`, `CharacterRig_DA131_AlgoritmTieneCodosYRodillasYCaraSinCuello`, `CharacterRig_DA131_TodaCurvaApuntaAUnaParteQueExiste`, `CharacterRig_DA131_CadaClipFijaTodasLasArticulaciones`: seis métodos `CharacterRig_DA131_*` y tres `CharacterRig_DA73_*` | EditMode |
| Emoción, parpadeo y habla | `FacialEmotion_CP02_NingunaEmocionEsDeDerrotaNiTristeza`, `FacialEmotion_CP02_TrasUnIntentoSinExitoElAnimoEsUnaCaraAlegre`, `MouthFlap_RF05_LaBocaSeMueveMientrasHablaYSeCierraAlCallar`, `CharacterFace_RF05_HablandoLaBocaCambiaYAlCallarVuelveAlReposo`: cuatro métodos `FacialEmotion_*`, tres `BlinkClock_*`, uno `MouthFlap_*` y cinco `CharacterFace_*` | EditMode |
| La emoción de un paso (RF-05) | `ActorTimeline_RF05_LaEmocionDeUnPasoSeMantieneHastaQueOtroLaCambie`, `ActorTimeline_RF05_SinEmocionDeclaradaElCueNoLaFija` | EditMode |
| Quien habla habla y calla al avanzar | `NarrativeScene_RF05_QuienHablaHablaYCallaAlAvanzar` | PlayMode |
| Orden de dibujo de los brazos (INC-132) | `CharacterRig_INC132_LosBrazosSeDibujanDetrasDelTorsoYDelanteDeLaCabeza`, `CharacterRig_INC132_AlgoritmPintaLasManosEncimaDeLaCara`, `CharacterRig_INC132_AlGolpearLosBrazosPasanDelanteDelTorso` y otros cuatro `CharacterRig_INC132_*` | EditMode |
| Cara de reserva y base de la cara | `CharacterFace_DA73_SinLaExpresionDeLaEmocionUsaLaNeutra`, `CharacterRig_DA131_LaCaraBaseVaDetrasDeLosOjosYDeLaBoca` | EditMode |
| Antebrazo que sigue a su ancla (INC-133) | `CharacterRig_INC133_ElAntebrazoSigueAlAnclaAlGirarElHumeroYElCodo` y otros de `CharacterRigLimbTests`; los `CharacterRig_INC133_*` sobre los prefabs reales | EditMode |

Las pruebas nuevas del cierre se escribieron contra el defecto que corrigen. `NarrativeScene_RNF21_AvanzarElTextoDuranteElCruceNoHaceSaltarALosViajeros` pulsa «Continuar» en cada cuadro del cruce y exige que el clic no mueva a nadie, lo que el código anterior incumplía. `RiverScene_INC118_LosPersonajesDeLaMecanicaTienenLaEscalaDeLaNarrativa` falló antes del cambio porque la casilla de Papá medía 77,7 en la orilla frente a 191,5 en la escena 3.1. Los ocho métodos nuevos de `ActorTimeline` se contrastaron con mutaciones que retiran la marca del código, y las pruebas que la protegen pasaron a rojo. Las capturas `NarrativeScene_RF05_CapturaCadaLineaConLosPersonajes` y `Personajes_DA133_CapturaCadaMecanicaConSusPersonajes` solo dejan imagen con la vista de juego del Editor; tras la limpieza de bordes del 01/10/2026 se repitieron las de las cinco mecánicas.

Los métodos posteriores al corte suman 32, contados con `git grep` sobre el último commit de la rama al 06/10/2026 (`ead78c6`): 31 en EditMode y uno en PlayMode. Las pruebas `CharacterRig_DA131_*` leen los prefabs y los clips del disco, así que miden el resultado del generador y no el código. `CharacterRig_DA131_LaFamiliaTieneCodosRodillasYCuello` dejó de exigir el cuello sobre el torso cuando INC-132 sustituyó esa regla. `NarrativeScene_RF05_QuienHablaHablaYCallaAlAvanzar` comprueba el estado del rig (`Speaking`) y no los sprites.

### 4.2 Corridas declaradas

El documento de resultados de los personajes no registra corridas propias: sus pruebas corren dentro de las corridas completas de la suite, que el Anexo G (apartado 4.2) desglosa por módulo. El 24/09/2026, con los personajes ya en escena, la prueba de memoria del Nivel 3 marcó 2 191 MB en un Editor abierto durante horas; esa prueba mide la sesión del Editor, y el valor que verifica RNF-05 es el del ejecutable (documento principal, apartado 8.1).

**Tabla F.7.** Corridas completas que ejercitaron las pruebas de los personajes.

| Fecha | Corte | EditMode | PlayMode | Observaciones |
|---|---|---|---|---|
| 25/09/2026 | `ccf77e6` | 360 de 361 (1 omitida) | 304 de 306 | Primera con los personajes; ningún fallo es de ellos |
| 30/09/2026 | Línea base del cierre (`359365e`) | 390 de 391 (1 omitida) | 332 de 332 | Con la estela y la sombra |
| 01/10/2026 | Correcciones de los Niveles 1 y 2 | 392 de 393 (1 omitida) | 350 de 351 | El fallo es la prueba de memoria del Editor |
| 01/10/2026 | Correcciones del Nivel 3 | 426 de 427 (1 omitida) | 363 de 363 | Con la marca de los viajeros y la escala del río |
| 01/10/2026 | Primer candidato del ejecutable | 433 de 434 (1 omitida) | 364 de 364 | Corrida anterior al primer candidato |
| 01/10/2026 | Corte del entregable | 469 de 470 (1 omitida) | 376 de 376 | Corrida final, anterior al ejecutable del corte; con la fogata de la escena final apartada de las cabezas de la familia |

Después del corte hubo corridas en el Editor sobre la rama. No dejaron un XML en el repositorio: sus cifras constan en los apartados C.9 y C.10 de `Personajes-Resultados.md` y en los mensajes de los commits que se citan. Aparte, `pose_preview.py` pasó 174 de 174 comprobaciones en los modos `--hoy` y `--maqueta` (apartado 2.9), una prueba fuera de Unity que no sustituye a la suite.

**Tabla F.13.** Corridas posteriores al corte, sobre commits de `feat/personajes-animados` (fechas de commit, en hora local).

| Commit | Fecha | `Game.Scaffolding.Tests` | Suite completa | Observaciones |
|---|---|---|---|---|
| `e9eb6cb` | 05/10/2026 | Sin dato en el mensaje | 935 de 935 (1 omitida) | Nodos y clips regenerados; la omitida es `ProfileEraserDiskTests` |
| `6f2b4cf` | 05/10/2026 | 215 de 215 | 940 de 942 (1 omitida) | Un fallo en `ForestScene_RF22`: el ratón real del Editor hace rodar un tronco del bosque; pasa 3 de 3 aislada y 38 de 38 en su grupo |
| `5ce0797` | 05/10/2026 | 215 de 215 | 941 de 942 (1 omitida) | Sin fallos; ronda sustituida después por la regla definitiva de los brazos |
| `117287c` | 05/10/2026 | 215 de 215 | 940 de 942 (1 omitida), en un solo Editor | Fallo de `RiverLevel_RNF05`: 2 199 MB con el Editor abierto todo el día; recién abierto mide 1 533 MB reservados y 1 072 asignados, y `Game.Levels.River.PlayMode.Tests` pasa 57 de 57 |
| `8cdd4c5` | 05/10/2026 | 234 de 234 | 960 de 961 (1 omitida), en un solo Editor | Sin fallos; `ForestScene_RF22` y `RiverLevel_RNF05` en verde |
| `433603f` | 06/10/2026 | 278 de 278 | EditMode: 627 pasan, 1 omitida, 0 fallos; PlayMode: 376 de 377 | Falla `RiverLevel_RNF05` con 2 161 MB en un Editor de larga duración; pruebas del antebrazo y el ancla 14 de 14; `CharacterCapture` 5 de 5 |

En las dos últimas rondas, `suite2.ps1` no llegó a correr: el segundo Editor murió dos veces por memoria al abrirse.

## 5. Archivos principales

**Tabla F.8.** Dónde están los personajes en el repositorio.

| Carpeta o archivo | Qué es |
|---|---|
| `Assets/Game/Art/Characters/` | Partes, retratos, formas de Algoritm, clips y controladores |
| `Assets/Game/Prefabs/Characters/` | Los siete prefabs, con `Estela` en Algoritm y `Sombra` en la familia |
| `Assets/Game/Scripts/Runtime/Scaffolding/` | `CharacterRig`, `ActorAction`, `ActorBeat`, `ActorCue`, `ActorTimeline`, `NarrativeProp`, `GuideTrail` y `TrailPath` |
| `Assets/Game/Scripts/Runtime/UI/NarrativeSceneController.cs` | Coloca y mueve a los personajes de las narrativas y pinta el retrato |
| `Assets/Game/Scripts/Runtime/Levels/` | Controladores de las cinco mecánicas; en el río, `DepthOrder` y `RiverWalk` |
| `Assets/Game/Data/Narrative/` | Las dieciocho secuencias con sus personajes y pasos |
| `Assets/Tests/EditMode/Scaffolding/` y `Assets/Tests/PlayMode/UI/` | `CharacterRigTests`, `ActorTimelineTests`, `GuideTrailTests`, `NarrativeSceneTests` y `CharacterCaptureTests` |
| `claudeDocs/tasks/Personajes/herramientas/` | `cut.py` (corta cada personaje en partes con sus pivotes), `forms.py` (las tres formas de Algoritm), `preview.py` (lámina de poses) y `BuildRigs.cs.txt` (script de editor que arma prefabs, clips y controladores) |
| `Assets/Game/Scripts/Runtime/Scaffolding/FacialEmotion.cs` | Las seis emociones de la cara, con número explícito |
| `Assets/Game/Scripts/Runtime/Scaffolding/ActionEmotion.cs` | La emoción por defecto de cada `ActorAction` |
| `Assets/Game/Scripts/Runtime/Scaffolding/BlinkClock.cs` | El reloj del parpadeo, con el azar inyectado |
| `Assets/Game/Scripts/Runtime/Scaffolding/MouthFlap.cs` | El ciclo de bocas mientras el personaje habla |
| `Assets/Game/Scripts/Runtime/Scaffolding/CharacterFaceSet.cs` | El ScriptableObject con los sprites de ojos y bocas y sus tiempos |
| `Assets/Game/Scripts/Runtime/Scaffolding/CharacterFace.cs` | El componente que pone ojos y boca en sus `Image` y avanza parpadeo y habla |
| `Assets/Game/Scripts/Runtime/Scaffolding/ArmLayering.cs` | Pasa los brazos delante del torso en las acciones de `armsInFrontActions` y los devuelve a su orden |
| `Assets/Game/Scripts/Runtime/Scaffolding/LimbFollower.cs` | Hace que el antebrazo, hijo de `Tronco`, siga a su ancla bajo el codo (INC-133) |
| `Assets/Tests/EditMode/Scaffolding/CharacterRigLimbTests.cs` | Las pruebas de `LimbFollower` sobre una jerarquía armada en código |
| `Assets/Tests/EditMode/Scaffolding/CharacterFaceTests.cs` y `FacialEmotionTests.cs` | Las pruebas de la cara; las segundas incluyen `BlinkClock` y `MouthFlap` |
| `claudeDocs/tasks/Personajes/herramientas/BuildRigsFinal.cs.txt` | El generador del arte final, con los modos de la Tabla F.12; no es código del juego |
| `claudeDocs/tasks/Personajes/herramientas/articulaciones.py` | Estima la tabla de articulaciones leyendo los prefabs |
| `claudeDocs/tasks/Personajes/herramientas/rig_articulaciones.json` | Dónde va cada nodo, el rectángulo y el pivote de cada pieza y el `orden_tronco` |
| `claudeDocs/tasks/Personajes/herramientas/arte_final.json` | Las medidas del arte final de cada personaje, que lee `articulaciones.py` |
| `claudeDocs/tasks/Personajes/herramientas/coreografia.py` | La coreografía de los 93 clips y su salida, `clips_personajes.json` |
| `claudeDocs/tasks/Personajes/herramientas/coreografia_v0.py` | La versión anterior, solo para la regresión del port |
| `claudeDocs/tasks/Personajes/herramientas/clips_personajes.json` | Las curvas que aplica el modo `clips` |
| `claudeDocs/tasks/Personajes/herramientas/pose_preview.py` | Renderiza el rig fuera de Unity y prueba cada clip (Tabla F.11) |
| `claudeDocs/tasks/Personajes/herramientas/maqueta.py` | Simula el arte final cortando el provisional por las articulaciones |
| `claudeDocs/tasks/Personajes/herramientas/prefabs.py` | Simula el modo `sprites` y el orden de dibujo, para la maqueta y la vista previa |
| `claudeDocs/tasks/Personajes/herramientas/preparar_arte_final.py` | Prepara la entrega de arte final de un personaje: informe, composite y, con `--aplicar`, PNG y tablas |
| `claudeDocs/tasks/Personajes/herramientas/preparar_expresion.py` | Parte la expresión entregada en ojos, boca y base de la cara; con `--registrada` usa el registro del personaje |
| `claudeDocs/tasks/Personajes/herramientas/OrganizarArtePersonajes.cs.txt` | Reparte el arte de cada personaje en `Frontal/`, `Expresiones/` y `Perfil/`; no es código del juego |
| `claudeDocs/tasks/Personajes/entregas/2026-10-06/` | Los originales de la entrega de arte final del 06/10/2026 |

## 6. Evaluación del OE4 y verificaciones a cargo de Santiago Benavides Rey

Los personajes no dependen de la observación con estudiantes ni de otros equipos. Dos aspectos quedan para la vista de Santiago Benavides Rey. Uno es la lectura rápida del cruce: si el texto avanza durante el cruce, los niños celebran y Papá señala al llegar la balsa y no al leerse su línea; se aceptó así y Santiago lo comprueba jugando con el guion H9 de la hoja de verificaciones manuales del OE4, que pide además anotar cualquier salto de sitio. El otro, para su visto bueno, es la escala relativa de los materiales del río: un tronco mide cerca de 0,6 de la altura de Papá en la escena 3.1, cerca de 1 en la orilla y cerca de 1,3 en la balsa del cruce; el mástil está de pie en la orilla y tumbado en la 3.1, y la familia de la orilla no se escala por profundidad. Lo juzga en la orilla con el guion H10, que recorre la recolección con el plano abierto. La evaluación funcional del OE4 vio además, en la escena final, la llama y el humo de la fogata central saliendo detrás de la cabeza de la Niña; desde el ejecutable del corte esa fogata está a la derecha del Niño y ya no sale detrás de ninguna cabeza (Anexo D, apartado 2.4).

El capítulo 10 del documento principal registra como deuda técnica menor (apartado 10.4) el mismo salto de los viajeros en otras catorce narrativas (68 situaciones, por ejemplo en `N1_Apertura`), que se corrige marcando a esos personajes y queda como mejora recomendada, y la balsa de la línea 0 de la escena 3.3, que al final de su deslizamiento sale por el borde derecho del encuadre. En el carril de arte (apartado 10.3) siguen los objetos que el guion nombra sin sprite (comida, maleza y piedras en la mano), un objeto que no puede aparecer ni cambiar de sprite a mitad de escena (por eso la escena 3.2 muestra la balsa hundida desde la primera línea), la acción de señalar con una sola dirección y las figuras frontales en las vistas cenitales.

Después del corte quedan abiertos estos puntos, sin dependencia de estudiantes y a cargo de Santiago Benavides Rey. Falta el arte final de las tres formas de Algoritm, que conservan el arte provisional, y las demás expresiones y las bocas del habla de la familia, que hoy muestra ojos neutros y boca cerrada (apartado 2.10). Dos puntos de coreografía esperan su decisión: los gestos cerca de la cabeza, que se abren a los lados, y el reposo en A de los brazos, de 40 a 72°. El choque de `Strike` con el arte final se comprobó fuera del motor y no tiene captura del Editor. Siguen sin ejecutarse los perfiles (`CharacterOrientation`), el retrato animado del cuadro de diálogo y la carga de emociones explícitas en las dieciocho secuencias, que hoy no declaran ninguna. Quedan además tres puntos de verificación. Conviene reiniciar el Editor antes de correr la suite, porque `RiverLevel_RNF05` mide 2 199 MB con un Editor abierto muchas horas y 1 533 MB recién abierto; en la ronda de `433603f` falló con 2 161 MB. `ForestScene_RF22` falló en la corrida de `6f2b4cf` frente al ratón real del Editor y queda como tarea del carril del Nivel 2. Y `suite2.ps1` no corrió en las dos últimas rondas porque el segundo Editor murió por memoria al abrirse.
