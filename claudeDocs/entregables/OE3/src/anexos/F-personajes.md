# Personajes animados

Este anexo describe cómo entraron al prototipo la familia y Algoritm, animados, en las dieciocho secuencias narrativas y en las cinco escenas jugables, desde el 24/09/2026 hasta el corte del 1 de octubre de 2026. Sale del documento de resultados de los personajes animados (`claudeDocs/tasks/Personajes/Personajes-Resultados.md`), que el repositorio conserva como registro histórico con sus notas fechadas, del registro de inconsistencias (revisión 16) y del código y los assets del corte. Sirve a los capítulos 4 y 7 del documento principal (apartado 7.2) y al apartado 8.2, donde la animación por recorte figura entre las optimizaciones.

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

## 2. Lo implementado

### 2.1 Arte y rig por recorte

Cada miembro de la familia (Papá, Mamá, la Niña y el Niño) se corta en cinco partes, torso, dos brazos y dos piernas, con el pivote en la articulación, y tiene un retrato con la expresión `neutra`; la emoción la llevan las acciones del cuerpo, como celebrar, sorprenderse u observar (INC-108). Algoritm tiene tres formas: la llama con extremidades del Nivel 1 y la misma figura recoloreada en madera (rueda) y en agua (gota) para los Niveles 2 y 3, como la describen desde el 29/09/2026 el guion y la dirección de arte (INC-45, INC-52). Es una sola imagen por forma y todo lo que muestra al guía referencia esos tres archivos, así que una versión dibujada aparte entraría sustituyendo el archivo, sin tocar escenas, prefabs ni assets. El 01/10/2026 se limpió el borde de las tres formas y de los cuatro retratos (Anexo E, apartado 2.4).

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

Ningún clip es de salto, caída ni derrota. Todo el arte de los personajes suma 16,3 MB en memoria.

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

## 3. Decisiones de diseño y hallazgos de consistencia

**Recorte en uGUI (INC-53).** La dirección de arte pedía animar con el paquete 2D Animation. Las escenas del juego son de uGUI en un lienzo superpuesto a la pantalla, y ese paquete solo deforma un `SpriteRenderer`, que quedaría debajo del lienzo, tapado por la ilustración; pasar las escenas a un lienzo de cámara rompía supuestos del código y de las pruebas. Se optó por partes `Image` giradas y desplazadas por un `Animator`, que conservan lo pedido: animación sobre el sprite base y no por hojas de fotogramas, como fijó el acta D06, respiración en reposo y ningún salto, caída ni derrota.

**Reconstruir un prefab rompe sus referencias.** La herramienta que armó los rigs reconstruye prefabs, clips y controladores, y con el argumento `clips` solo reescribe los clips. Reconstruir un prefab cambia los identificadores de sus componentes y rompe las referencias de `Narrative.unity`, de los dieciocho assets y de las cinco escenas; por eso los retoques posteriores añadieron hijos o reescribieron clips.

**Revisión de las 138 capturas del 24/09/2026.** Se revisaron las capturas del motor de cada línea narrativa y de las cinco mecánicas. Se corrigieron clips (arrodillarse y dormir bajan casi al suelo, abrazar abre los brazos, celebrar no pasa de unos 100° para que las manos no queden detrás de las cabezas, señalar va casi horizontal, observar lleva las manos a la cintura) y la puesta en escena (personajes que pisaban objetos o flotaban sobre las rocas, materiales de la 3.1 que no apoyaban en el suelo, Algoritm pegado a la mano de Papá).

**Alcance de la marca de los viajeros.** El salto de la escena 3.3 existe en otras catorce narrativas; marcarlas habría cambiado la escenificación de escenas ya revisadas, y la corrección se limitó al cruce, donde el salto se veía sobre la balsa en movimiento.

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

## 4. Verificación

### 4.1 Pruebas que lo nombran

`88fe0ee` añadió 50 métodos de prueba, que el 25/09/2026, contados sobre `ccf77e6`, sumaban 106 casos (32 en EditMode y 74 en PlayMode). En los nombres, `DA133`, `DA76`, `DA53` y `DA83` citan los apartados 13.3, 7.6, 5.3 y 8.3 de la dirección de arte. Estas pruebas viven en los módulos de `Game.Scaffolding`, de `Game.UI` y de los tres niveles, cuyas cifras del corte da la Tabla 9.1 del documento principal.

**Tabla F.6.** Pruebas representativas de los personajes.

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

Las pruebas nuevas del cierre se escribieron contra el defecto que corrigen. `NarrativeScene_RNF21_AvanzarElTextoDuranteElCruceNoHaceSaltarALosViajeros` pulsa «Continuar» en cada cuadro del cruce y exige que el clic no mueva a nadie, lo que el código anterior incumplía. `RiverScene_INC118_LosPersonajesDeLaMecanicaTienenLaEscalaDeLaNarrativa` falló antes del cambio porque la casilla de Papá medía 77,7 en la orilla frente a 191,5 en la escena 3.1. Los ocho métodos nuevos de `ActorTimeline` se contrastaron con mutaciones que retiran la marca del código, y las pruebas que la protegen pasaron a rojo. Las capturas `NarrativeScene_RF05_CapturaCadaLineaConLosPersonajes` y `Personajes_DA133_CapturaCadaMecanicaConSusPersonajes` solo dejan imagen con la vista de juego del Editor; tras la limpieza de bordes del 01/10/2026 se repitieron las de las cinco mecánicas.

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

## 6. Evaluación del OE4 y verificaciones a cargo de Santiago Benavides Rey

Los personajes no dependen de la observación con estudiantes ni de otros equipos. Dos aspectos quedan para la vista de Santiago Benavides Rey. Uno es la lectura rápida del cruce: si el texto avanza durante el cruce, los niños celebran y Papá señala al llegar la balsa y no al leerse su línea; se aceptó así y Santiago lo comprueba jugando con el guion H9 de la hoja de verificaciones manuales del OE4, que pide además anotar cualquier salto de sitio. El otro, para su visto bueno, es la escala relativa de los materiales del río: un tronco mide cerca de 0,6 de la altura de Papá en la escena 3.1, cerca de 1 en la orilla y cerca de 1,3 en la balsa del cruce; el mástil está de pie en la orilla y tumbado en la 3.1, y la familia de la orilla no se escala por profundidad. Lo juzga en la orilla con el guion H10, que recorre la recolección con el plano abierto. La evaluación funcional del OE4 vio además, en la escena final, la llama y el humo de la fogata central saliendo detrás de la cabeza de la Niña; desde el ejecutable del corte esa fogata está a la derecha del Niño y ya no sale detrás de ninguna cabeza (Anexo D, apartado 2.4).

El capítulo 10 del documento principal registra como deuda técnica menor (apartado 10.4) el mismo salto de los viajeros en otras catorce narrativas (68 situaciones, por ejemplo en `N1_Apertura`), que se corrige marcando a esos personajes y queda como mejora recomendada, y la balsa de la línea 0 de la escena 3.3, que al final de su deslizamiento sale por el borde derecho del encuadre. En el carril de arte (apartado 10.3) siguen los objetos que el guion nombra sin sprite (comida, maleza y piedras en la mano), un objeto que no puede aparecer ni cambiar de sprite a mitad de escena (por eso la escena 3.2 muestra la balsa hundida desde la primera línea), la acción de señalar con una sola dirección y las figuras frontales en las vistas cenitales.
