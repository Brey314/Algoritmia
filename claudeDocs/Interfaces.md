# Interfaces del juego y estilo de personaje

Subordinado a `Direccion_de_Arte.md`, que a su vez lo está a `SPEC.md`. Este archivo no
introduce mecánicas ni requisitos: enumera las superficies de interfaz que el juego necesita
según los RF ya radicados, y fija el estilo de personaje que se le pasa al generador.

**Sobre el `.fig`:** no se entrega. `.fig` es el formato binario propietario de Figma, sin
especificación pública; un archivo escrito con esa extensión no abriría en Figma ni serviría de
nada a Claude Code. Lo que sí es utilizable es este documento: cada interfaz lleva sus
componentes, su traza a RF y las reglas que la gobiernan, que es lo que hace falta para montarla
en Unity o para maquetarla en Figma a mano.

---

## 1. Inventario de interfaces

Diecisiete superficies. `LevelSummary` y `TeacherReport` son estados de la FSM
(`GameState.cs`) y cada uno tiene su escena (`LevelSummary.unity`, `TeacherReport.unity`). **No hay pantalla de derrota
y no es un olvido:** CP-02 la prohíbe, junto con el límite de intentos y la penalización.

| # | Interfaz | Escena | Traza | En código |
|---|---|---|---|---|
| 1 | Arranque | `Boot.unity` | RNF-04 | ✓ sin UI visible: instancia los tres persistentes y salta |
| 2 | Pantalla de inicio | `MainMenu.unity` | RF-01, RF-08, RF-09, RF-46 | ✓ `MainMenuController`: Jugar, Créditos, Progreso del equipo y Salir |
| 3 | Selección de perfil | `MainMenu.unity` | RF-02, RF-03, RF-04, RF-47, HU-01, HU-16, CU-01, CU-12 | ✓ `ProfileSelectController`, con el borrado de un perfil tras confirmación |
| 4 | Menú de niveles | `LevelSelect.unity` | RF-03, RNF-19, HU-14 | ✓ `LevelSelectController` |
| 5 | Escena narrativa | `Narrative.unity` | RF-05, RF-06 | ✓ `NarrativeSceneController`, parametrizada por `NarrativeSequence` |
| 6 | Pausa | superposición — prefab `Assets/Game/Prefabs/UI/MenuPausa.prefab` en las cinco escenas jugables | RF-07, HU-17, INC-25 | ✓ `PauseMenuController` (T16; W17 desde el 15/09/2026): no tiene estado propio en la FSM, va sobre `Playing` |
| 7 | Nivel 1 · panel de encendido | `Level1_Cave.unity` | RF-14, RF-15, RF-19, RF-21 | ✓ `FirePanelController` (Fase 5 del Slice 1, INC-47) |
| 8 | Nivel 2 · fase 1, selección por patrón | `Level2_Forest.unity` | RF-22..RF-26 | ✓ `ForestSceneController` (W06/W07) |
| 9 | Nivel 2 · fase 2, ensamblaje | `Level2_Workshop.unity` | RF-27..RF-29 | ✓ `WorkshopSceneController` (W09, 12/09/2026), contenido en `N2_AssemblyContent.asset` |
| 10 | Nivel 2 · fase 3, editor de secuencia | `Level2_Maze.unity` | RF-30..RF-34 | ✓ `MazeSceneController` (W13/W14, 13/09/2026), contenido en `N2_MazeLayout.asset` |
| 11 | Nivel 3 · recolección en la orilla | `Level3_River.unity` | RF-35..RF-39 | ✓ `RiverSceneController`, contenido en `N3_RiverLevelConfig.asset` |
| 12 | Nivel 3 · ensamblaje sobre el río | `Level3_River.unity` | RF-40..RF-43 | ✓ `AssemblyPanelController`, contenido en `N3_RaftAssemblyContent.asset` |
| 13 | Resumen de fin de nivel | `LevelSummary.unity` (una para los tres niveles) | RF-12, RF-17, RF-45 | ✓ `LevelSummaryController` y `LevelSummaryComposer`; textos en un `LevelSummaryMessages` por nivel (`LevelSummaryMessages`, `N2_ResumenNivel`, `N3_ResumenNivel`) |
| 14 | Créditos | `Credits.unity` | RF-08, CT-09, PG-07 | ✓ `CreditsController` |
| 15 | Informe docente | `TeacherReport.unity` | RF-46 | ✓ `TeacherReportController` e `IndicatorTableView` (Slice 4); rótulos en `Data/Reporting/ReportContent.asset` |
| 16 | Confirmación de borrado | superposición en `TeacherReport.unity` y en la selección de perfil (`MainMenu.unity`) | RF-47, RNF-11, CU-12 | ✓ `EraseConfirmationDialog` (informe) y panel de borrado de `ProfileSelectController` |
| 17 | Transiciones | entre escenas | RF-05, RNF-04 | ✓ fundido a negro de `SceneLoader` (0,4 s por mitad, tiempo sin escalar, pintado con `OnGUI`) al pasar entre narrativa y mecánica o entre dos narrativas; la regla está en `GameFlowRunner.FadesBetween` y los menús cortan en seco. La cortinilla y el barrido de Algoritm de `TR-05` y `TR-09` (`S02`) no están en el juego |

### 1.1 Qué lleva cada una

**2 · Pantalla de inicio.** Título del juego (`GameTitleConfig`), el lema «Piensa el orden, enciende el fuego», la figura de Algoritm en fuego, botón primario de jugar,
secundarios de créditos y de «Progreso del equipo» —el acceso del docente al informe (RF-46),
que lleva a `TeacherReport`—, salir. Nada más: RF-01 pide una pantalla de inicio, no un menú de
opciones. «Salir» no cierra de inmediato: abre una confirmación breve que informa que lo logrado
ya está guardado, con «Quedarme» (vuelve al menú sin tocar nada) y «Cerrar el juego» (guarda el
perfil activo y cierra, en ese orden); si la carpeta `Datos/` no admite escritura, la confirmación
muestra además la carpeta de respaldo donde quedó el progreso (HU-18 pasos 3-5, FA-01 y FA-04, RF-09).

**3 · Selección de perfil.** Un solo nombre por perfil (RF-02). Lista de perfiles existentes,
cada uno con su papelera, que pide confirmación («¿Borras el perfil de…?», Conservar / Borrar) y
borra con el mismo `ProfileSession.Delete` que el informe docente (RF-47); campo de creación,
botón de continuar. Sin avatar, sin edad, sin curso: cualquier dato extra es
dato personal que RNF-08 y RNF-10 no quieren en disco.

**4 · Menú de niveles.** Tres entradas con desbloqueo progresivo: un nivel se desbloquea cuando el
anterior está completado —todas sus fases confirmadas—, aunque un cierre inesperado haya impedido
llegar a su resumen (RF-03, RNF-14, INC-116). El bloqueo se comunica por
**dos canales**: el candado `ui_lock.png` con la palabra «Bloqueado» y el estado deshabilitado
(RNF-19); el nivel bloqueado no responde al clic. Nunca solo por color. Un nivel completado
—todas sus fases confirmadas— lleva la marca «Completado» con un icono de visto, también por
dos canales y sin cifras (HU-14 paso 7, HU-05, CP-03); sigue desbloqueado y se puede volver a jugar.

**5 · Escena narrativa.** Cuadro de diálogo en el cuarto inferior de la pantalla (§10.3), sin
cola: retrato y nombre del hablante, el texto, botón de continuar y botón de omitir —este último
**solo en escenas ya vistas** (RF-06)—. Máximo dos líneas por cuadro y doce palabras por línea
(§11.4).

**6 · Pausa.** Está arriba a la derecha en las cinco escenas jugables y permanece toda la fase,
como el botón de pista y la tablilla de mensajes (regla 6 de §3).
Tablilla marfil sobre el velo carbón al 72 % con tres botones apilados: **Reanudar** (primario),
**Reiniciar** (secundario, añadido el 15/09/2026 por decisión de Santiago: pide confirmación de una
frase y repite **la fase activa**, INC-25) y **Volver al menú de niveles** (secundario). Sin ajustes
de dificultad: no existen. Con la pausa abierta el tiempo se detiene (`Time.timeScale = 0`) y no
suma al indicador de resolución (OE1 §3.6.1 nota 1). Los glifos del botón de pausa, de Reanudar y
de Reiniciar son de Phosphor Icons (MIT), acreditados en los créditos (INC-127).

**7 · Nivel 1.** Desde el 15/09/2026 (Fase 6, INC-47) son **dos momentos**. *Reunir*: la cueva
cenital a sangre con hojas, sílex y pedernal regados que se **arrastran** al centro; en pantalla
solo la tablilla, «Pista» (que además dibuja el círculo de reunión, con el borde a la altura de la
mitad del botón de abajo) y la pausa. *Encender*: la cámara al doble sobre la fogata —las hojas
sueltas se funden en el montón visto desde arriba y las piedras quedan sobre él, una a cada lado
del punto del fuego—, deslizante vertical de **fuerza** con diez
muescas (el asa va de azul a rojo y crece con la fuerza), deslizante horizontal de **cercanía de
las piedras** con diez muescas entre la mitad de «Soplar» y la mitad de «Golpear» («Lejos» a la
izquierda, «Cerca» a la derecha), botón «Golpear», botón «Soplar» —deshabilitado hasta que RF-19 lo
permita, y la diferencia se lee por el candado, sin rótulo, no por el color—, botón de pista
arriba a la izquierda y pausa arriba a la derecha. Una sola tablilla arriba
muestra la instrucción y luego **el último mensaje** del registro (el historial ya no se ve).
Mientras se reúne, la tablilla queda debajo del suelo de la cueva, para que una pieza soltada bajo
ella siga a la vista y al alcance del clic; al pasar al encendido sube sobre el suelo, y el humo
que sube a la corona de la llama pasa por detrás de ella (RNF-03). Desde que se sopla hasta que
termina el nivel, «Soplar», «Golpear» y los dos deslizantes dejan de responder y se atenúan; no
llevan candado, porque no es un «todavía no» sino el fuego naciendo (CP-02). La
progresión de luz del nivel es retroalimentación en sí misma (RF-21), pero **nunca es el único
canal** (RNF-19).

**8–10 · Nivel 2.** Fase 1: objetos del bosque, contador de acopio, iconos de aceptado y
devuelto. Fase 2: siete piezas —la séptima es la cuerda— y el panel de ensamblaje. Fase 3: tablero cenital, editor con los
tres bloques de instrucción —que se distinguen **por forma**, criterio literal de RNF-19— y el
botón «Ejecutar», de clic simple (`PG-04`).

**11–12 · Nivel 3.** La orilla en un plano fijo con perspectiva por profundidad, a ×1,4: el río y
el pie de la cascada a la derecha. Mamá, la familia —detrás de la zona de construcción—, los ocho
materiales y la zona se ven a la escala con que los muestra la escena 3.1, y lo que está más abajo
se dibuja delante (INC-118). Cuatro botones
de dirección en cruceta abajo a la derecha —Mamá avanza mientras se sostiene el clic— y
«Recoger»: son la entrada del nivel, no props, y materializan `INC-01` —el control es UI en pantalla, nunca teclado (RF-35, CT-06,
RNF-02)—. Lista de cuatro tareas en una tablilla de marfil arriba a la izquierda, con un círculo por
tarea que se cambia por un círculo verde con visto al cumplirla, **la única lista permanente del
juego** (RF-36, `INC-41`, `INC-46`), inventario de cuatro casillas cuadradas en rejilla de 2×2 abajo a la izquierda —la de los
troncos con cinco marcas que se encienden sin cifra—, y el ensamblaje sobre el propio río: la
cámara empuja del plano de juego al de la balsa, a ×1,6, con la familia en la orilla a la
izquierda y la balsa sobre el agua a la derecha, y una sombra negra al 30 % cubre la
ilustración, sin ventana superpuesta. Abajo, en la base y el amarre, «Listo» confirma la fase y a
su derecha «Probar balsa» la pone a prueba sin aprobarla: la balsa se hunde y vuelve, suena la
salpicadura, y se marca y vuelve al inventario solo lo mal puesto, nunca los espacios vacíos
(CP-06). En mástil y vela queda un solo botón centrado, «Probar balsa», que es el que confirma
(INC-122). Los dos miden 360×96. La balsa terminada cruza en la escena 3.3 por debajo de la
espuma de la cascada, a y 0,395 (INC-124).

**13 · Resumen de fin de nivel.** Mockups 13 y 13b, en código desde el 12/09/2026: tablilla
centrada sobre arena con el avatar del guía y el título («Esto es lo que pasó en la cueva»), el
hallazgo («Descubriste que el fuego necesita chispa y aire»), el relato en viñetas —una por frase
que compone `LevelSummaryComposer`—, el recuadro verde con la habilidad nombrada («probar y
ajustar», RF-12) y dos botones: «Volver al menú de niveles» y «Continuar», que hacen lo mismo. En los niveles 1 y 2 llevan al menú de niveles. En el Nivel 3 llevan los dos a la escena final del juego y de ahí a los créditos (INC-39); lo declara `LevelSummaryMessages.ClosingSequenceId` en `N3_ResumenNivel`. Todo en palabras:
**sin intentos, sin errores, sin pasos, sin tiempo y sin puntaje** (CP-03, RF-17, RF-45); esas
cifras existen, pero solo en el informe docente. Los textos viven en un asset por nivel —`LevelSummaryMessages.asset` (Nivel 1), `N2_ResumenNivel.asset` y `N3_ResumenNivel.asset`— y la escena es una sola, `LevelSummary.unity`; el del Nivel 3 declara además la escena final a la que sale «Continuar» (INC-39).

**15 · Informe docente.** El único sitio del juego donde hay números (RF-46): intentos, errores,
pasos y tiempo, con su iconografía propia (`D1`).

---

## 2. Componentes compartidos

De `Direccion_de_Arte.md` §10.2, que es la fuente:

| Componente | Material aparente | Color base |
|---|---|---|
| Panel de diálogo | Tablilla de piedra clara | `#F7EFE2`, borde `#C4A882`, esquinas de 32 px |
| Botón primario | Piedra redondeada | `#E8A33D`, borde `#3A1E18`, sombra plana inferior de 6 px |
| Botón secundario | Piedra clara | `#E0D4C0`, borde `#6B5248` |
| Lista de tareas (solo Nivel 3) | Tablilla de piedra clara | `#F7EFE2` sin borde; tarea pendiente, círculo liso `#6B5247`; cumplida, círculo verde con visto `#336638` |
| Icono de pista | Algoritm en pequeño | `#E8A33D`; pulso lento de escala continuo solo en el Nivel 1 (`ui_pulso_pista`); no cambia tras los fallos |
| Marco de inventario | Panel liso color arena (`ui_panel`) | `#C7A87C`; casillas cuadradas `#E0D4C0` en rejilla de 2×2 |

**Neutros de interfaz** (§4.3): marfil `#F7EFE2` · marfil sombra `#E0D4C0` · carbón `#3A1E18` ·
carbón suave `#6B5248` · éxito `#5FA842` · atención `#E8A33D`.

**Tipografía** (§11): Baloo 2 para títulos, contadores, el nombre del hablante y la tablilla del
guía de los niveles 2 y 3; Nunito para el diálogo, el cuerpo, la instrucción del Nivel 1 y las
cifras del informe docente. Las dos SIL OFL 1.1 y con soporte de `ñ`, tildes y `¿ ¡`. Tamaños en el
juego, a 1920×1080: diálogo a 26 px y texto de lectura desde 26 px; bajan de ahí el nombre del
hablante, el rótulo «Algoritm» del resumen, la etiqueta del laberinto y el «Aún no» del taller
(22 px) y la instrucción del Nivel 1 (24 px).

---

## 3. Reglas que aplican a todas las interfaces

1. **Sin cifras a la vista del estudiante** en ninguna pantalla del juego ni en el resumen de
   fin de nivel (CP-03, RF-17, RF-45). Los números viven en el informe docente y en ningún otro
   sitio.
2. **Dos canales siempre** (RNF-19): ningún estado se comunica solo con color. Habilitado vs.
   deshabilitado se lee por forma —candado, marco, desplazamiento—, y bloqueado por el candado
   más el estado apagado.
3. **No hay rojo de error en toda la interfaz** (§12.3). El estado del intento lo lleva el icono
   que acompaña al mensaje del guía, con forma y color propios (RNF-19): verde oscuro `#336638`
   para lo aceptado y la tarea cumplida, ocre `#995C1A` para lo devuelto y azul pizarra `#3D4C70`
   para la instrucción y la pista; en la lista del Nivel 3, la tarea pendiente va en carbón suave
   `#6B5247`. El naranja `#D96B29`, con su icono, marca el espacio equivocado de la balsa, y el
   ámbar `#E8A33D` el bloque elegido y el refugio del laberinto. La única gama que llega al rojo
   es el asa del deslizante de fuerza del Nivel 1, que mide intensidad y no error. Un intento sin
   éxito devuelve las piezas a su sitio y muestra ánimo, no falta.
4. **Área táctil de 88×88 px** a resolución de diseño 1920×1080 en todo botón de acción
   (§10.1). La motricidad fina de un niño de nueve años no es la de un adulto. Quedan por debajo
   los controles que van en una franja estrecha o dentro de un blanco mayor: «Continuar» (260×76)
   y «Omitir» (200×76) en el cuadro de diálogo, y en el editor de bloques del laberinto «−» y «+»
   de la cuenta (64×64), la flecha de giro (52×52), los dos lados de «Girar» (82×64) y las flechas
   que desplazan la lista (56×56).
5. **Contraste ≥ 4.5:1** (RNF-20). El par carbón sobre marfil lo sostiene; texto claro sobre
   fondo oscuro está prohibido, la legibilidad para lectores en formación es notablemente peor.
6. **Mínima permanencia** (§10.1): en toda escena jugable permanecen solo la pausa, el botón de
   pista y la tablilla de mensajes del guía. Además queda lo que un RF pide mostrar de forma
   permanente: el contador de acopio del bosque del Nivel 2 (RF-24) y, en el Nivel 3, la lista de
   cuatro tareas (RF-36) y el inventario; la cruceta del Nivel 3 solo está mientras se recorre la
   orilla. Los niveles 1 y 2 no llevan lista de tareas y ningún nivel lleva barra de progreso:
   añadirlas sería una mecánica que ningún RF pide (`INC-41`).
7. **Sin texto dentro de los `.png`**: el texto lo escribe Unity encima, para que sea
   parametrizable (CT-05) y traducible.
8. **Diegética cuando se pueda**: los paneles son tablillas, los botones son piedras, los marcos
   son cuerda trenzada.

---

## 4. Estilo de personaje

Especificación que se pega en el generador. Los bloques marcados **(crítico)** son los que la IA
rompe con más frecuencia y los que hay que verificar pieza a pieza.

**ESTILO.** Ilustración vectorial 2D, animación cartoon clásica americana de los años 40–50
(*golden age* / *rubber hose*). Personaje de cuerpo completo, vista frontal, centrado.

**LÍNEA.** Contorno limpio de grosor variable en marrón muy oscuro `#3A1E18`, más grueso en la
silueta exterior y más fino en los detalles internos. Sin líneas de arrugas, sin líneas de
expresión, sin textura de piel.

**COLOR.** Colores planos y saturados. Nada de degradados en ninguna parte de la imagen.

**SOMBRAS (crítico).** Cel-shading plano de exactamente dos tonos por color: un tono base y un
único tono de sombra, ambos como áreas de color sólido separadas por un borde duro y nítido.
Prohibido: degradados, aerógrafo, transiciones suaves, difuminado, mezcla de tonos, luz difusa,
brillos suaves, volumen pintado. **Ante la duda, color plano sin sombra antes que sombra
difuminada.** Fuente de luz única desde arriba a la izquierda a 45°: la sombra ocupa el lado
derecho del rostro, el cuello bajo el mentón, el costado derecho del torso y la cara interna de
la pierna derecha. Cada sombra es una mancha recortada, como en un dibujo animado televisivo.
**No dibujar sombra proyectada sobre el suelo ni sombra de contacto bajo los pies** — la sombra
es un sprite independiente, elipse `#000000` al 25 %, hija del GameObject (§5.3); pintada en el
sprite viajaría pegada al personaje.

**CABELLO (crítico).** Color plano uniforme con una única zona de sombra plana de borde duro.
Prohibido dibujar mechones con degradado, transiciones de tono, brillos suaves o variación de
color entre mechones. Los mechones se definen únicamente con líneas de contorno, nunca con
cambios graduales de color.

**LÍNEAS INTERNAS (crítico).** Únicamente las indispensables. No dibujar pliegues ni arrugas en
la ropa, no dibujar líneas nasolabiales ni de mejilla, no dibujar músculos, pectorales,
clavículas, costillas ni ombligo. La ropa es una silueta de color plano con su contorno y sus
manchas, nada más.

**PROPORCIONES.** Cabeza grande y redondeada —un tercio de la altura total en adultos, dos
quintos en niños—, cráneo esférico, mejillas llenas, mentón corto y redondo. Cuerpo de formas
simples y redondeadas. Altura relativa tomando a papá como unidad: papá 1.00, mamá 0.92, niño
0.60, niña 0.60. Los dos niños comparten altura exacta y línea de suelo, para que sean
intercambiables sin reajustar cámara ni colisionadores (§7.1).

**MANOS (crítico).** Del mismo color de piel que los brazos, **sin ningún guante**, sin puño,
sin muñequera y sin ninguna prenda. **No dibujar guantes blancos de dibujo animado.** Forma
redondeada y simple, con cuatro dedos gruesos, cortos y romos, del mismo grosor entre sí, sin
uñas ni nudillos. La mano es continuación directa del antebrazo, sin línea divisoria en la
muñeca. Pies grandes y redondeados, descalzos, con los dedos apenas insinuados.

**ROSTRO.** Ojos grandes y redondos con mucha esclerótica blanca, pupila circular negra y un
brillo puntual blanco; párpado superior en curva alta y abierta, nunca caído ni rasgado. Cejas
gruesas, cortas y curvas, siempre separadas. Nariz pequeña de botón. Boca en sonrisa
cerrada y suave, sin dientes a la vista: es la cara `neutra`, la única del juego (§4.2).

**POSE.** A-pose de producción (§7.5): brazos extendidos hacia los lados y hacia abajo formando
unos 45° con el torso, **completamente separados del cuerpo**, con fondo visible entre cada
brazo y el costado y las axilas abiertas; piernas rectas y ligeramente separadas, con fondo
visible entre ellas. Se ve rígida a propósito: es el frame del que derivan todas las
animaciones, y el rig no puede recortar una extremidad fundida con el torso (§13.1).

**SILUETA (§2.1).** Cada personaje debe ser identificable relleno de negro sólido al 15 % de
tamaño: papá ancho y robusto, mamá esbelta y curva, el niño con copete puntiagudo, la niña con
penacho alto. Si dos no se distinguen en silueta, uno está mal diseñado.

### 4.1 Paleta — códigos exactos, no aproximar

| Elemento | Base | Sombra |
|---|---|---|
| Piel | `#F2D3BC` | `#D9AF95` |
| Cabello castaño rojizo muy oscuro | `#5C2B22` | `#3D1A14` |
| Piel de leopardo (adultos) | `#E8C07A` | `#C49A55` |
| Manchas de leopardo | `#2B1A12` | — |
| Túnica del niño (oliva amarillento) | `#C4C24E` | `#9BA03A` |
| Manchas de la túnica del niño | `#3F6B2E` | — |
| Conjunto de la niña (ocre dorado) | `#D9B23A` | `#B08A25` |
| Manchas del conjunto de la niña | `#7A5418` | — |
| Rubor infantil | `#F0A5A0` | — |
| Contorno de personaje | `#3A1E18` | — |

**El cabello no admite castaño claro ni medio.** Y **la piel nunca se tiñe con la luz ambiente
del nivel** (§4.2): es el ancla que mantiene reconocibles a los personajes en los tres entornos,
que tienen paletas muy distintas.

### 4.2 Expresiones

Una sola cara por personaje, la `neutra`: la del torso del rig y la del retrato del cuadro de
diálogo (`char_<x>_retrato_neutra.png`). No se generan variantes faciales. La emoción la lleva
el cuerpo, con un clip del rig por acción (`ActorAction`, §7.3): `Encourage` (ánimo, puño
arriba), `Celebrate`, `Surprise`, `Observe`, `Point`, `Talk`…

**No existen la tristeza ni el enfado**, ni un gesto de derrota. Tras un intento sin éxito el
personaje hace `Encourage` y nunca otra cosa. La decisión no es estética — sostiene el ensayo y
error en entorno seguro que fundamenta el enfoque de Aprendizaje Basado en Juegos del proyecto
(§7.3, CP-02).

### 4.3 Algoritm no sigue esta especificación

El guía tiene su propio contrato (§7.6) y **no es un humano estilizado**: es una llama con cara,
brazos y piernas de palo, manos abiertas color piel y una franja de cinco colores en la base;
sin nariz ni cejas, con dos ojos redondos grandes y contorno café oscuro. Conserva el mismo
cuerpo en los tres niveles y cambia de material —fuego, madera y agua, en los prefabs
`Algoritm_Fuego`, `Algoritm_Rueda` y `Algoritm_Gota`—; madera y agua son la llama recoloreada,
provisionales hasta que el arte definitivo sustituya el archivo. Cambia solo entre dos
secuencias encadenadas de los puentes, nunca a la vista dentro de una escena jugable.

---

## 5. Divergencias con lo ya radicado — resueltas por el arte entregado

La paleta de §4.1 coincide **exactamente** con `Direccion_de_Arte.md` §4.1, y la regla de
sombras coincide con §5.1, §5.2 y §5.3. Tres puntos del rostro sí divergen del prompt `A2` que
está en `claudeDocs/tasks/Slice 1/plan.md`:

1. **Ojos.** Aquí: «grandes y redondos con mucha esclerótica blanca, pupila circular negra y
   brillo puntual». `A2` dice de papá «ojos negros, ovalados, medianos». Son dos personajes
   distintos. §7.3 describe la expresión neutra como «abiertos, redondos», lo que apoya esta
   versión, pero hay que corregir `A2` explícitamente.
2. **Cejas.** Aquí: «gruesas, cortas y **curvas**». `A2` y §7.3 dicen **rectas** para la
   expresión neutra. Las curvas son, en §7.3, la ceja de `alegre` y de `animo`.
3. **Boca.** Aquí: «sonrisa cálida» con dientes visibles como franja o dos incisivos. §7.3 fija
   para `neutra` una «sonrisa cerrada suave». Probablemente esta descripción corresponde a
   `alegre` y no a la pose base.

Los tres apuntaban a lo mismo: esta especificación describía una cara más expresiva que la
`neutra` de §7.3. **Lo resolvió el arte entregado** (24/09/2026): ojos redondos con esclerótica
y brillo y cejas curvas, como pide esta especificación, y boca en sonrisa cerrada sin dientes,
como la `neutra` de §7.3. Es la única cara del juego (§4.2), así que no hay variantes `alegre`
ni `animo` a las que reservar la otra. §7.3 se actualizó con esa cara; el prompt `A2` de
`claudeDocs/tasks/Slice 1/plan.md` queda superado y no se edita (los `plan.md` no se reescriben).
