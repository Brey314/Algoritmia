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

Diecisiete superficies. `LevelSummary` y `TeacherReport` ya existen como estados de la FSM
(`GameState.cs`) aunque sus escenas lleguen en slices posteriores. **No hay pantalla de derrota
y no es un olvido:** CP-02 la prohíbe, junto con el límite de intentos y la penalización.

| # | Interfaz | Escena | Traza | En código |
|---|---|---|---|---|
| 1 | Arranque | `Boot.unity` | RNF-04 | ✓ sin UI visible: instancia los tres persistentes y salta |
| 2 | Pantalla de inicio | `MainMenu.unity` | RF-01, RF-09 | ✓ `MainMenuController` |
| 3 | Selección de perfil | `MainMenu.unity` | RF-02, RF-03, RF-04, HU-01, CU-01 | ✓ `ProfileSelectController` |
| 4 | Menú de niveles | `LevelSelect.unity` | RF-03, RNF-19 | ✓ `LevelSelectController` |
| 5 | Escena narrativa | `Narrative.unity` | RF-05, RF-06 | ✓ `NarrativeSceneController`, parametrizada por `NarrativeSequence` |
| 6 | Pausa | superposición | RF-07 | ○ no tiene estado propio en la FSM: va sobre `Playing` |
| 7 | Nivel 1 · panel de encendido | `Level1_Cave.unity` | RF-14, RF-15, RF-19, RF-21 | ○ Slice 1 |
| 8 | Nivel 2 · fase 1, selección por patrón | `Level2_Forest.unity` | RF-22..RF-26 | ○ Slice 2 |
| 9 | Nivel 2 · fase 2, ensamblaje | `Level2_Workshop.unity` | RF-27..RF-29 | ○ Slice 2 |
| 10 | Nivel 2 · fase 3, editor de secuencia | `Level2_Maze.unity` | RF-30..RF-33 | ○ Slice 2 |
| 11 | Nivel 3 · exploración cenital | `Level3_River.unity` | RF-35..RF-39 | ○ Slice 3 |
| 12 | Nivel 3 · panel de ensamblaje | `Level3_River.unity` | RF-40, RF-42, RF-43 | ○ Slice 3 |
| 13 | Resumen de fin de nivel | por nivel | RF-17, RF-45 | ○ `GameState.LevelSummary` ya existe |
| 14 | Créditos | `Credits.unity` | RF-08, CT-09, PG-07 | ✓ `CreditsController` |
| 15 | Informe docente | `TeacherReport.unity` | RF-46 | ○ Slice 4, estado ya en la FSM |
| 16 | Confirmación de borrado | superposición | Slice 4 · `D3` | ○ |
| 17 | Transiciones | todas | `TR-05`, `TR-09` | ○ `S02`: fundido, cortinilla y barrido de Algoritm |

### 1.1 Qué lleva cada una

**2 · Pantalla de inicio.** Título del juego (`GameTitleConfig`), botón primario de jugar,
secundario de créditos, salir. Nada más: RF-01 pide una pantalla de inicio, no un menú de
opciones.

**3 · Selección de perfil.** Un solo nombre por perfil (RF-02). Lista de perfiles existentes,
campo de creación, botón de continuar. Sin avatar, sin edad, sin curso: cualquier dato extra es
dato personal que RNF-08 y RNF-10 no quieren en disco.

**4 · Menú de niveles.** Tres entradas con desbloqueo progresivo. El bloqueo se comunica por
**dos canales**: el candado `ui_lock.png` y el estado deshabilitado (RNF-19). Nunca solo por
color.

**5 · Escena narrativa.** Globo de diálogo (§10.3), retrato del hablante, botón de continuar y
botón de omitir —este último **solo en escenas ya vistas** (RF-06)—. Máximo dos líneas por
globo y doce palabras por línea (§11.4).

**6 · Pausa.** Es lo único que permanece en pantalla junto con la lista del Nivel 3 (§10.1).
Reanudar, volver al menú de niveles. Sin ajustes de dificultad: no existen.

**7 · Nivel 1.** Desde el 12/09/2026 (Fase 5, INC-47): la cueva cenital a sangre con hojas, sílex y
pedernal regados que se **arrastran** al punto del fuego; deslizante vertical de **fuerza** con diez
muescas (el asa va de azul a rojo y crece con la fuerza), botón «Golpear», botón «Soplar»
—deshabilitado hasta que RF-19 lo permita, y la diferencia se lee por el candado grabado, no por el
color—, botón de pista arriba a la izquierda y pausa arriba a la derecha. Una sola tablilla arriba
muestra la instrucción y luego **el último mensaje** del registro (el historial ya no se ve). La
progresión de luz del nivel es retroalimentación en sí misma (RF-21), pero **nunca es el único
canal** (RNF-19).

**8–10 · Nivel 2.** Fase 1: objetos del bosque, contador de acopio, iconos de aceptado y
devuelto. Fase 2: seis piezas y el panel de ensamblaje. Fase 3: tablero cenital, editor con los
tres bloques de instrucción —que se distinguen **por forma**, criterio literal de RNF-19— y el
botón «Ejecutar», de clic simple (`PG-04`).

**11–12 · Nivel 3.** Cuatro botones de dirección y «Recoger»: son la entrada del nivel, no
props, y materializan `INC-01` —el control es UI en pantalla, nunca teclado (RF-35, CT-06,
RNF-02)—. Lista de cuatro tareas en cuerda con nudos, **la única lista permanente del juego**
(RF-36, `INC-41`), inventario de casillas circulares y panel de ensamblaje.

**13 · Resumen de fin de nivel.** Mockups 13 y 13b, en código desde el 12/09/2026: tablilla
centrada sobre arena con el avatar del guía y el título («Esto es lo que pasó en la cueva»), el
hallazgo («Descubriste que el fuego necesita chispa y aire»), el relato en viñetas —una por frase
que compone `LevelSummaryComposer`—, el recuadro verde con la habilidad nombrada («probar y
ajustar», RF-12) y dos botones: «Volver al menú de niveles» y «Continuar». Todo en palabras:
**sin intentos, sin errores, sin pasos, sin tiempo y sin puntaje** (CP-03, RF-17, RF-45); esas
cifras existen, pero solo en el informe docente. Los textos viven en `LevelSummaryMessages.asset`.

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
| Lista de tareas (solo Nivel 3) | Cuerda con nudos | Cuerda `#C4A882`, nudo `#5FA842` |
| Icono de pista | Algoritm en pequeño | `#E8A33D`, pulso lento de escala |
| Marco de inventario | Cuerda trenzada | `#C4A882`, casillas circulares |

**Neutros de interfaz** (§4.3): marfil `#F7EFE2` · marfil sombra `#E0D4C0` · carbón `#3A1E18` ·
carbón suave `#6B5248` · éxito `#5FA842` · atención `#E8A33D`.

**Tipografía** (§11): Baloo 2 para títulos, Nunito para diálogo y cuerpo, Fredoka para las
cifras del informe docente. Todas SIL OFL 1.1 y con soporte de `ñ`, tildes y `¿ ¡`. **Mínimo
absoluto 26 px**; diálogo a 34 px.

---

## 3. Reglas que aplican a todas las interfaces

1. **Sin cifras a la vista del estudiante** en ninguna pantalla del juego ni en el resumen de
   fin de nivel (CP-03, RF-17, RF-45). Los números viven en el informe docente y en ningún otro
   sitio.
2. **Dos canales siempre** (RNF-19): ningún estado se comunica solo con color. Habilitado vs.
   deshabilitado se lee por forma —candado, marco, desplazamiento—, y bloqueado por el candado
   más el estado apagado.
3. **No hay rojo de error en toda la interfaz** (§12.3). Los únicos colores de estado son el
   verde de éxito `#5FA842` y el ámbar de atención `#E8A33D`. Un intento sin éxito devuelve las
   piezas a su sitio y muestra ánimo, no falta.
4. **Área táctil mínima de 88×88 px** a resolución de diseño 1920×1080 (§10.1). La motricidad
   fina de un niño de nueve años no es la de un adulto.
5. **Contraste ≥ 4.5:1** (RNF-20). El par carbón sobre marfil lo sostiene; texto claro sobre
   fondo oscuro está prohibido, la legibilidad para lectores en formación es notablemente peor.
6. **Mínima permanencia** (§10.1): solo el botón de pausa y, solo en el Nivel 3, la lista de
   cuatro tareas. Los niveles 1 y 2 no llevan indicador de progreso: añadirlo sería una mecánica
   que ningún RF pide (`INC-41`).
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
gruesas, cortas y curvas, siempre separadas. Nariz pequeña de botón. Boca ancha en sonrisa
cálida; los dientes se dibujan como una franja blanca continua o dos incisivos superiores, nunca
como una dentadura individualizada.

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

Seis por personaje, como variaciones del sprite base manteniendo idénticos el cráneo, el peinado
y el color (§7.3): `neutra`, `alegre`, `sorpresa`, `concentracion`, `duda`, `animo`.

**No existen la tristeza ni el enfado.** Un intento sin éxito nunca produce una expresión
negativa: produce `animo`. La decisión no es estética — sostiene el ensayo y error en entorno
seguro que fundamenta el enfoque de Aprendizaje Basado en Juegos del proyecto (§7.3, CP-02).

### 4.3 Algoritm no sigue esta especificación

El guía tiene su propio contrato (§7.6) y **no es un humano estilizado**: sin extremidades, sin
nariz, sin cejas, contorno de 8 px en `#E2571F` cálido en los tres niveles, dos óvalos negros
muy separados con un punto de luz en la esquina superior izquierda de cada uno, y una silueta
que siempre se cuenta hasta cinco. Cambia de cuerpo por nivel —estrella, rueda, gota— y solo en
los barridos `TR-05` y `TR-09`, nunca a la vista dentro de una escena jugable.

---

## 5. Divergencias con lo ya radicado — resolver antes de generar

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

Los tres apuntan a lo mismo: esta especificación describe una cara más expresiva que la
`neutra` de §7.3. O se aplica solo a las variantes `alegre` y `animo`, o §7.3 y `A2` se
actualizan. Es una decisión de dirección, no técnica.
