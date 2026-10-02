# Dirección de arte — Videojuego educativo 2D

Última actualización: 2026-10-01
Proyecto: prototipo de videojuego educativo 2D · Unity · C#
Fase Árcade: Diseño (OE2)
Documento de referencia obligatoria para todo asset visual del proyecto

**Subordinación.** `claudeDocs/SPEC.md` es la única fuente de verdad del proyecto. Este documento
desarrolla la capa visual **dentro** de ese contrato y no lo amplía: si algo de aquí contradice a
`SPEC.md`, a un RF/RNF o al guion, gana `SPEC.md` y esto se corrige. En particular, este documento
**no introduce mecánicas**: el juego no tiene salto, ni desplazamiento libre de plataformas, ni
pantalla de derrota, ni puntajes (CT-06, RNF-02, CP-02, CP-03), y la entrada se limita a clic y
clic sostenido.

---

## Índice

1. [Declaración de intención](#1-declaración-de-intención)
2. [Pilares de dirección de arte](#2-pilares-de-dirección-de-arte)
3. [Sistema de línea](#3-sistema-de-línea)
4. [Sistema de color](#4-sistema-de-color)
5. [Sistema de sombreado e iluminación](#5-sistema-de-sombreado-e-iluminación)
6. [Jerarquía de lectura](#6-jerarquía-de-lectura)
7. [Personajes](#7-personajes)
8. [Entornos por nivel](#8-entornos-por-nivel)
9. [Props y objetos interactivos](#9-props-y-objetos-interactivos)
10. [Interfaz de usuario](#10-interfaz-de-usuario)
11. [Tipografía](#11-tipografía)
12. [Efectos visuales y retroalimentación](#12-efectos-visuales-y-retroalimentación)
13. [Animación](#13-animación)
14. [Accesibilidad visual](#14-accesibilidad-visual)
15. [Especificaciones técnicas para Unity](#15-especificaciones-técnicas-para-unity)
16. [Prompts de generación de entornos](#16-prompts-de-generación-de-entornos)
17. [Checklist de aprobación de assets](#17-checklist-de-aprobación-de-assets)
18. [Decisiones pendientes](#18-decisiones-pendientes)
19. [Nota legal](#19-nota-legal)

---

## 1. Declaración de intención

El videojuego acompaña a una familia prehistórica a través de tres descubrimientos
—el fuego, la rueda y el cruce de un río— que funcionan como marco narrativo para retos
de pensamiento computacional dirigidos a estudiantes de grado cuarto de primaria
(9 a 11 años).

La dirección de arte responde a tres condiciones que no son negociables y que ordenan
todas las decisiones posteriores:

**El público es infantil.** Nada en pantalla debe intimidar, endurecer ni resultar
ambiguo. La prehistoria se representa como un mundo de descubrimiento y asombro, no
de supervivencia ni de peligro. No hay depredadores amenazantes, no hay violencia, no
hay representación de muerte.

**El prototipo debe correr con bajo consumo de recursos** en los equipos de la
institución, que **no tienen tarjeta gráfica dedicada** (CT-02), y dentro de los
presupuestos duros de RNF-04 (carga < 10 s), RNF-05 (memoria < 2 GB) y RNF-06
(paquete < 500 MB). El arte es gráficamente simple por diseño, no por limitación:
colores planos, pocas capas, sin efectos de posprocesado costosos. Esta restricción es
también la que produce la coherencia visual.

**La legibilidad manda sobre el detalle.** El estudiante debe distinguir de un vistazo
qué es personaje, qué es interactivo y qué es decorado. Cualquier elemento que compita
con esa lectura se simplifica o se elimina, por bonito que sea.

El resultado buscado es un mundo cálido, saturado y caricaturesco, con la claridad
gráfica de la animación televisiva clásica: formas grandes, contornos firmes y color
plano.

---

## 2. Pilares de dirección de arte

Cinco principios que resuelven las dudas cuando el documento no cubre un caso concreto.

### 2.1 La silueta primero

Todo elemento debe ser reconocible únicamente por su contorno, en negro sobre blanco,
a 128 píxeles de alto. Si dos elementos no se distinguen en silueta, uno de los dos
está mal diseñado. Este es el criterio que separa a papá (ancho y robusto) de mamá
(esbelta y curva), y al niño (copete puntiagudo) de la niña (penacho alto).

**Prueba de validación:** rellenar el asset de negro sólido y mirarlo al 15 % de
tamaño. Si sigue siendo identificable, pasa.

### 2.2 Color plano, sin excepciones

No hay degradados en ninguna parte del juego: ni en personajes, ni en fondos, ni en
la interfaz, ni en los efectos. El volumen se sugiere con un único tono de sombra de
borde duro. Cuando exista la duda entre poner una sombra difuminada o no poner sombra,
se elige no ponerla.

Esta regla es la que más se rompe accidentalmente al generar assets con IA, y es la
que más rápido delata la inconsistencia entre piezas.

### 2.3 El personaje siempre gana

Los personajes tienen el contorno más grueso, los colores más saturados y el mayor
contraste interno de toda la pantalla. El fondo se diseña para perder: menos contraste,
menos saturación, contorno más fino o inexistente. En ningún momento un elemento de
decorado debe atraer la mirada antes que un personaje.

### 2.4 Lo interactivo se señala con color, no con brillo

Todo objeto con el que el jugador puede interactuar comparte un tratamiento visual
común: contorno de personaje (grueso) y saturación alta, sobre un fondo deliberadamente
más apagado. No se usan halos, glows ni destellos para señalar interactividad, porque
son costosos y ensucian la lectura. Se usa contraste de saturación.

### 2.5 Redondez sobre angulosidad

Las formas del mundo son redondeadas: rocas con esquinas romas, troncos de sección
ovalada, montañas de cima curva. Los únicos elementos angulosos permitidos son los
que deben leerse como herramienta o construcción (la rueda, las lanzas de tender, las
piedras talladas), precisamente porque el contraste de forma los marca como
"fabricado por alguien" frente a "naturaleza".

---

## 3. Sistema de línea

El grosor de contorno es la herramienta principal de jerarquía visual. Se define en
píxeles a resolución de trabajo (1024 px de alto de personaje) y se escala
proporcionalmente.

| Capa | Grosor | Color | Uso |
| --- | --- | --- | --- |
| Personajes | 8–12 px (variable) | `#3A1E18` | Familia, NPC guía |
| Objetos interactivos | 7–9 px | `#3A1E18` | Recolectables, palancas, bloques |
| Primer plano decorativo | 6 px | `#4A2E24` | Matorrales y rocas delante del jugador |
| Plano medio (escenario) | 4 px | `#5C4038` | Plataformas, paredes de cueva |
| Fondo lejano | Sin contorno | — | Montañas, cielo, siluetas |

### Reglas de trazo

- **Grosor variable dentro de una misma pieza:** más grueso en la silueta exterior,
  más fino en los detalles internos. Esto da peso sin añadir líneas.
- **Contorno cerrado siempre.** Ninguna forma queda abierta; el recorte y el relleno
  dependen de ello.
- **Sin líneas internas innecesarias.** No se dibujan pliegues de ropa, arrugas,
  músculos, clavículas, ombligos, vetas de madera ni texturas de piedra. Una superficie
  es un color plano con su contorno.
- **Esquinas redondeadas.** Ningún vértice en ángulo agudo salvo en elementos
  intencionalmente "fabricados" (§2.5).
- **Sin línea en el fondo lejano.** La profundidad se construye eliminando el contorno,
  no oscureciéndolo.

---

## 4. Sistema de color

### 4.1 Paleta maestra de personajes

Fija para toda la familia. No se altera entre niveles ni entre escenas.

| Elemento | Base | Sombra |
| --- | --- | --- |
| Piel | `#F2D3BC` | `#D9AF95` |
| Cabello | `#5C2B22` | `#3D1A14` |
| Piel de leopardo (adultos) | `#E8C07A` | `#C49A55` |
| Manchas de leopardo | `#2B1A12` | — |
| Túnica del niño (oliva) | `#C4C24E` | `#9BA03A` |
| Manchas de la túnica del niño | `#3F6B2E` | — |
| Conjunto de la niña (ocre) | `#D9B23A` | `#B08A25` |
| Manchas del conjunto de la niña | `#7A5418` | — |
| Rubor infantil | `#F0A5A0` | — |
| Contorno de personaje | `#3A1E18` | — |

### 4.2 Reglas de color

**Regla de las tres familias cromáticas.** Cada nivel se construye sobre un acorde de
tres familias: una dominante (el 60 % de la pantalla), una secundaria (el 30 %) y una
de acento (el 10 %, reservada para lo interactivo). Ningún nivel usa más.

**El acento es propiedad de la mecánica.** El color de acento de cada nivel no aparece
en el decorado bajo ninguna circunstancia. Si el naranja del fuego es el acento del
nivel 1, ninguna roca, planta o elemento de fondo puede ser naranja. Esta es la regla
que hace que el estudiante localice lo interactivo sin necesidad de instrucciones.

**Saturación descendente por profundidad.** El mismo color se desatura y se aclara
conforme se aleja del jugador. Se aplica mezclando con el color de cielo del nivel:
plano medio al 15 %, fondo al 35 %, fondo lejano al 55 %.

**Piel siempre constante.** El tono de piel no se tiñe con la luz ambiente del nivel.
Es el ancla que mantiene a los personajes reconocibles en los tres entornos.

### 4.3 Neutros compartidos

Usados en interfaz y en elementos comunes a todos los niveles.

| Nombre | Hex | Uso |
| --- | --- | --- |
| Marfil | `#F7EFE2` | Fondo del cuadro de diálogo y de los paneles |
| Marfil sombra | `#E0D4C0` | Borde interior de paneles |
| Carbón | `#3A1E18` | Texto y contornos |
| Carbón suave | `#6B5248` | Texto secundario |
| Éxito | `#5FA842` | Confirmación de reto resuelto |
| Atención | `#E8A33D` | Pista disponible, reintento |

No se usa rojo para el error. Se explica en §12.3.

---

## 5. Sistema de sombreado e iluminación

### 5.1 Cel-shading de dos tonos

Cada color tiene exactamente dos valores: base y sombra. La sombra es una forma sólida
de borde duro, nunca difuminada, nunca degradada. No existe un tercer tono de sombra
profunda ni un tono de luz especular.

**Excepción única:** el fuego del nivel 1, que usa tres tonos por ser fuente de luz
(núcleo, cuerpo, borde). Se detalla en §8.1.

### 5.2 Dirección de luz

**Luz global fija: superior izquierda, 45°.** Constante en todo el juego, todos los
niveles y todos los assets. Consecuencias:

- La sombra ocupa el lado derecho del rostro y del cuerpo.
- El cuello queda sombreado bajo el mentón.
- La cara interna de la pierna derecha queda en sombra.
- Las plataformas tienen su cara superior iluminada y su canto derecho en sombra.

**Excepción del nivel 1:** en las zonas donde el fuego es la fuente de luz dominante,
la dirección se invierte hacia la posición de la hoguera. Es la única desviación
permitida y debe ser evidente y deliberada, nunca ambigua.

### 5.3 Sombras proyectadas

**No se pintan en el sprite.** Una sombra dibujada bajo los pies viaja pegada al
personaje al desplazarse y al cambiar de dirección, lo que rompe la ilusión.

Se resuelve con un sprite independiente: elipse de color plano `#000000` a 25 % de
opacidad, hijo del GameObject del personaje, con su posición vertical anclada al suelo.

No hay escalado por altura de salto: **en este juego no se salta**. En las mecánicas, el único personaje
que se desplaza es Mamá en el Nivel 3, sobre un plano fijo de la orilla con perspectiva por
profundidad y accionada con botones en pantalla (RF-35, CT-06, RNF-02): cuanto más abajo está,
más grande se ve (`N3_RiverLevelConfig`: `DepthScaleNear` 1, `DepthScaleFar` 0,55), y todo lo
que cuelga de ella escala con ella. Su casilla se ancla por los pies, como la de la familia
(pivote (0,5; 0,075)), y en la orilla Mamá, la familia y los materiales se ven a la escala de la
escena 3.1 (INC-118). Quién tapa a quién lo decide la profundidad: entre Mamá, la familia y los
materiales, lo que está más abajo se dibuja delante (`DepthOrder`). En las narrativas los
personajes sí caminan de una casilla a otra (`NarrativeSceneController.WalkAsync`), y la sombra,
hija del lienzo del rig, viaja con ellos anclada a los pies.

### 5.4 Iluminación ambiental por nivel

Cada nivel tiene un color de luz ambiente que se aplica **solo al decorado**, nunca a
los personajes ni a los objetos interactivos.

| Nivel | Color ambiente | Opacidad sobre decorado |
| --- | --- | --- |
| La Oscuridad | `#2A3A5C` (azul frío) | 30 % |
| La Rueda | `#F0C88A` (dorado cálido) | 15 % |
| El Río | `#8FC4B0` (verde húmedo) | 20 % |

---

## 6. Jerarquía de lectura

El orden en que la mirada del estudiante debe recorrer la pantalla, y los recursos que
lo garantizan.

| Prioridad | Elemento | Recursos que lo sostienen |
| --- | --- | --- |
| 1 | Personaje jugable | Contorno más grueso, mayor saturación, mayor contraste interno, animación de idle constante |
| 2 | Objeto interactivo del reto actual | Color de acento del nivel (exclusivo), contorno grueso, ligera animación de flotación |
| 3 | Plataformas y superficies navegables | Contraste medio, contorno intermedio, borde superior más claro |
| 4 | NPC y personajes de apoyo | Contorno de personaje pero saturación reducida un 10 % |
| 5 | Decorado de plano medio | Contorno fino, saturación reducida |
| 6 | Fondo lejano | Sin contorno, muy desaturado, sin detalle |

### Prueba de entrecerrado

Método de validación rápida: entrecerrar los ojos frente a una captura del nivel (o
aplicarle un desenfoque gaussiano de 12 px). Los primeros elementos que sigan siendo
distinguibles deben ser, en este orden, el personaje y el objetivo del reto. Si lo
primero que resalta es un elemento de decorado, el fondo está mal calibrado.

---

## 7. Personajes

Los prompts de generación de cada personaje viven en los planes de slice —
`claudeDocs/tasks/Slice 1/plan.md` §Assets visuales (`A1`..`A5`), reutilizados por los
Slices 2 y 3 sin volver a generarse. Esta sección cubre lo que esos prompts no abordan:
expresión, lenguaje corporal y coherencia entre miembros.

**Reparto por nivel, cerrado en el guion §1.2 (CN-02):** Papá es jugable en el Nivel 1,
la Niña en el Nivel 2 y Mamá en el Nivel 3; el Niño acompaña. Algoritm, el guía, está en
los tres (CN-03), con una forma distinta en cada uno (§7.6).

### 7.1 Escala relativa

Referencia de altura tomando a papá como unidad.

| Personaje | Altura relativa | Ancho de torso relativo |
| --- | --- | --- |
| Papá | 1.00 | 1.00 |
| Mamá | 0.92 | 0.68 |
| Niño | 0.60 | 0.55 |
| Niña | 0.60 | 0.52 |

Los dos niños comparten altura exacta y línea de suelo, para que sean intercambiables
como personaje jugable sin reajustar cámara ni colisionadores.

### 7.2 Proporción de cabeza

- Adultos: la cabeza ocupa 1/3 de la altura total.
- Niños: la cabeza ocupa 2/5 de la altura total.

La cabeza grande es lo que produce la lectura infantil y amable. Reducirla endurece
al personaje de inmediato.

### 7.3 Expresión y emoción

Cada personaje tiene **una sola cara, la neutra**, la del torso del rig y la del retrato del
cuadro de diálogo (`char_<x>_retrato_neutra.png`): cejas separadas y algo curvas, ojos
abiertos y redondos con brillo, sonrisa cerrada suave sin dientes. No se generan variantes
faciales. La emoción la lleva el cuerpo, con un clip del rig por acción (§13.3, `ActorAction`):

| Emoción | Acción del rig | Uso en juego |
| --- | --- | --- |
| Reposo | `Idle` | Estado de reposo |
| Alegría | `Celebrate` | Reto resuelto, cierre de fase |
| Sorpresa | `Surprise` | Descubrimiento, evento narrativo |
| Atención | `Observe` | Al empezar un reto, al mirar algo |
| Ánimo | `Encourage` | Tras un intento fallido |

**Regla sobre la tristeza y el enfado:** no existen, ni en la cara ni en el cuerpo. Un
intento fallido nunca produce un gesto negativo en los personajes; produce el de ánimo. Esta
decisión conecta con el principio de ensayo y error en entorno seguro que sostiene el
enfoque de Aprendizaje Basado en Juegos del proyecto.

### 7.4 Lenguaje corporal

- **Papá:** movimientos amplios y algo lentos. Gesticula con los brazos abiertos.
  Transmite calma y seguridad.
- **Mamá:** movimientos precisos y ligeros. Suele señalar o extender la mano.
  Transmite guía.
- **Niño:** movimientos rápidos y algo exagerados, con rebote. Transmite entusiasmo.
- **Niña:** movimientos curiosos, con inclinación de cabeza al observar. Transmite
  atención.

### 7.5 Pose base de producción

Todos los sprites base se generan en A-pose: brazos extendidos hacia los lados y hacia
abajo, axilas abiertas, fondo visible entre cada brazo y el torso. Es una pose de
producción, no de presentación: se ve rígida a propósito, porque es el frame del que
derivan todas las animaciones y porque permite recortar las extremidades en las partes
de la animación por recorte (§13.1) sin tener que inventar dónde terminan.

Las poses expresivas para el documento de trabajo de grado y las capturas de
sustentación se generan aparte, usando el sprite base aprobado como referencia.

---

### 7.6 Algoritm — un cuerpo, tres materiales

El guía se llama **Algoritm** (`PG-02` cerrado el 02/09/2026, INC-44) y **cambia de material
en cada nivel**: fuego, madera y agua, en ese orden (INC-45). Es el mismo personaje en los
tres —lo exige CN-03— y conserva en los tres **el mismo cuerpo**: una llama con cara, brazos y
piernas de palo, manos y una franja de colores en la base (INC-52). Entre niveles cambia solo
el color de la llama; la cara, las extremidades y la franja no cambian.

El guion ya lo empujaba: en §4.4 el guía aparece «en el corazón de las llamas […] hecho de
fuego esta vez». Su cuerpo es el material del descubrimiento que el nivel acaba de nombrar.

#### Núcleo de identidad — invariable en los tres niveles

Si uno solo de estos rasgos cambia, deja de leerse como el mismo personaje:

| Rasgo | Especificación |
| --- | --- |
| Cuerpo | Una llama de tres lenguas —la central, más alta— que se ensancha hacia abajo y apoya en una base redondeada. Es el mismo cuerpo en los tres niveles; lo que cambia es el material (tabla siguiente) |
| Tamaño | Pequeño frente a la familia. En las narrativas su `NarrativeProp.Size` está entre un tercio y dos quintos del de Papá y Mamá (0,12 frente a 0,313 y 0,34 en `N1_AparicionGuia`), y es menor cuando aparece en la fogata (0,07 en `N1_NacimientoDelFuego`) |
| Ojos | Dos ojos redondos grandes a media altura de la llama, blanco crema con iris café oscuro y un punto de luz blanco en cada uno |
| Boca | Una sola línea curva hacia arriba, sonrisa cerrada. Sin nariz, sin cejas |
| Extremidades | Brazos y piernas de palo, finos y del color del contorno, que salen de la base; manos abiertas color piel; cada pie es un trazo corto horizontal. Sin accesorios |
| Franja | Cinco bandas horizontales en la base —naranja, verde, amarilla, azul y roja—, **iguales en los tres niveles**: el material de madera o de agua no las cambia |
| Núcleo interior | Área más clara dentro de la llama que repite su silueta en pequeño y enmarca la cara |
| Contorno | Café oscuro (`#3B1205`, muestreado del archivo), el mismo en el cuerpo y en las extremidades |
| Estela | Cinco a siete puntos sueltos `#FFE9A8`, circulares, de tamaño decreciente, en curva. Nunca una nube difuminada |

#### Las tres formas

| Nivel | Material | Prefab · archivo | Cuerpo | Núcleo | Estado |
| --- | --- | --- | --- | --- | --- |
| 1 · La Oscuridad | **Fuego** | `Algoritm_Fuego` · `char_algoritm_n1_fuego_reposo.png` | Llama naranja `#FFA51E` | `#FFE093` | Arte entregado el 24/09/2026. Es la forma de origen: la que aparece en la fogata y se recoge en ella como brasa viva |
| 2 · La Rueda | **Madera** | `Algoritm_Rueda` · `char_algoritm_n2_rueda_reposo.png` | La misma llama en tono de madera `#DBA362` | `#DBC0A1` | **Provisional.** El prefab y el archivo se llaman «rueda», pero no tiene disco, radios ni buje |
| 3 · El Río | **Agua** | `Algoritm_Gota` · `char_algoritm_n3_gota_reposo.png` | La misma llama en tono de agua `#50C5EA` | `#A0D8EA` | **Provisional.** El prefab y el archivo se llaman «gota», pero no tiene forma de gota |

**Madera y agua son provisionales.** Salen del arte del Nivel 1 recoloreado por encima de la
franja (`claudeDocs/tasks/Personajes/herramientas/forms.py`), para que cada nivel muestre un
guía distinto sin arte nuevo. El definitivo entra **sustituyendo el archivo con el mismo
nombre**, sin tocar escenas, prefabs ni assets. Mientras no llegue, esta tabla describe lo que
el juego muestra; si el arte definitivo cambia la silueta, esta tabla, §7.6 y el guion §1.1.1
se corrigen con él.

**Cuándo muta.** En los dos puentes, al pasar de una secuencia narrativa a la siguiente,
que es donde el juego funde a negro (`GameFlowRunner.FadesBetween`): en `N2_PuenteI` es de
fuego y en `N2_PuenteI_Bosque` ya es de madera; en `N3_PuenteII` y `N3_PuenteII_Horizonte`
es de madera y en `N3_PuenteII_Rio` ya es de agua. Entra con el material del nivel que
termina y sale con el del que empieza. En ningún otro momento cambia, y **nunca a la vista
dentro de una escena jugable**: en las cinco mecánicas aparece solo dentro del botón de
ayuda, con el material de su nivel. El barrido de Algoritm de `TR-05` y `TR-09` todavía no
existe en el juego (`fx_algoritm_barrido`, pendiente en `Assets/Game/Art/Inventario.md`).

**El riesgo del nivel 2, y cómo se contiene.** El cuerpo de madera (`#DBA362`) queda muy
cerca de `#C79A5E`, el acento del nivel y por tanto la señal de «esto es interactivo». La regla de §4.2 prohíbe
ese tono en el **decorado**, y el guía no es decorado, así que no la infringe — pero sí
puede confundir. Tres condiciones lo separan de un prop, y son obligatorias:

1. **Nunca se posa.** Flota siempre por encima de la línea de los objetos del reto, y no
   entra en la zona de ensamblaje.
2. **Tiene cara.** Ningún prop del juego tiene ojos ni boca.
3. **Pulsa.** El pulso de escala de la pista (§10.2) es suyo y de nada más.

#### Nomenclatura

```
char_algoritm_n1_fuego_reposo.png
char_algoritm_n2_rueda_reposo.png
char_algoritm_n3_gota_reposo.png
```

Una sola imagen por forma, sin recorte en partes: los prefabs `Algoritm_Fuego`,
`Algoritm_Rueda` y `Algoritm_Gota` animan el cuerpo entero con `char_algoritm.controller`, así
que sustituir el archivo basta. «Rueda» y «gota» nombran el nivel, no la silueta.

Sustituyen a `char_chispa_*`. La palabra `chispa` queda libre para lo que siempre fue en
este juego: el rayo del golpe del Nivel 1 (§12.2), que dibuja el motor, no tiene archivo y no
tiene nada que ver con el guía.

---

## 8. Entornos por nivel

Los tres niveles comparten sistema de línea, sombreado y proporción, y se diferencian
por acorde cromático, ambiente lumínico y vocabulario de formas. La progresión está
diseñada para leerse como un amanecer largo: de la noche del nivel 1 al día luminoso del
nivel 2 —que la luz del motor recorre del amanecer a la noche (§8.2)— y a la mañana húmeda
del nivel 3.

---

### 8.1 Nivel 1 — La Oscuridad

**Descubrimiento:** el fuego
**Momento del día:** noche cerrada
**Sensación buscada:** un refugio pequeño y seguro rodeado de un exterior desconocido,
que se va abriendo conforme el jugador enciende luces.

#### Acorde cromático

| Función | Familia | Colores |
| --- | --- | --- |
| Dominante (60 %) | Azules fríos profundos | Cielo `#1B2A4A`, roca `#3E3550` / sombra `#2A2438` |
| Secundaria (30 %) | Marrones violáceos | Suelo `#5E4A52` / sombra `#42333A`, estalagmitas `#6B5A60` |
| **Acento (10 %)** | **Naranjas de fuego** | **`#F5A62E`, `#E2571F`, `#FFE9A8`** |

**Prohibición de acento:** ningún elemento de decorado del nivel 1 puede usar naranja,
amarillo ni rojo. Esos tonos pertenecen exclusivamente al fuego y a los objetos que
lo transportan.

#### Vocabulario de formas

Interior de cueva con techo de bóveda irregular pero de curvas suaves. Estalactitas y
estalagmitas de puntas romas, nunca afiladas. Rocas ovaladas y apiladas. Aberturas
hacia el exterior con forma de arco redondeado que dejan ver el cielo nocturno.

El exterior visible a través de las aberturas es una silueta plana muy oscura
(`#141F38`) sin contorno ni detalle, con estrellas como puntos de dos tamaños
(`#F7EFE2` y `#BFD4E8`) distribuidos irregularmente. Sin luna: la única fuente de luz
cálida debe ser el fuego.

#### La hoguera

Elemento central del nivel y única excepción al sombreado de dos tonos.

| Capa | Color | Forma |
| --- | --- | --- |
| Núcleo | `#FFE9A8` | Óvalo pequeño, borde duro |
| Cuerpo | `#F5A62E` | Lengua de llama redondeada, borde duro |
| Borde | `#E2571F` | Contorno de la llama, borde duro |
| Halo de luz | `#F0A84E` al 20 % | Círculo plano, sin degradado, escala oscilante |

El halo es un círculo de color plano, no un degradado radial. Oscila su escala entre
0.95 y 1.05 en un ciclo de 1.2 s para sugerir el parpadeo sin coste de cómputo.

#### Iluminación

En las zonas próximas a una fuente de fuego, la dirección de luz se invierte hacia la
hoguera: las sombras de personajes y objetos se proyectan en dirección opuesta a la
llama. Fuera del radio de luz, vuelve la luz global superior izquierda, pero muy tenue.

#### Progresión visual

El nivel debe oscurecer y aclarar de forma legible según el avance del reto. Se
implementa con un sprite de máscara de color plano `#0F1526` a opacidad variable sobre
el decorado (nunca sobre los personajes).

**Los escalones son los del panel de encendido, no los de unas antorchas** — en este
nivel no hay antorchas ni desplazamiento: el reto se resuelve desde un panel fijo
(guion §4.3, RF-14, RF-15, RF-19).

| Estado del reto (RF-21) | Opacidad de la máscara |
| --- | --- |
| Inicio del nivel, ningún golpe efectivo | 65 % |
| Primer golpe efectivo | 45 % |
| Golpes efectivos completos, «Soplar» habilitado | 25 % |
| Fuego encendido | 0 % |

Esta progresión es en sí misma retroalimentación del reto: el estudiante ve el
resultado de su razonamiento en la iluminación del entorno. `RF-21` es de prioridad
**Baja**: si se recorta, el nivel debe seguir siendo jugable y legible, porque la
iluminación **nunca es el único canal** de retroalimentación (RNF-19).

#### Elementos de decorado

Pinturas rupestres en las paredes (`#8C4A2F` sobre la roca, sin contorno, formas muy
simplificadas de manos y animales), musgo en `#4A5C42`, charcos de agua como óvalos
planos `#2E4258` con un reflejo de línea recta `#4A6B8C`.

---

### 8.2 Nivel 2 — La Rueda

**Descubrimiento:** la rueda
**Momento del día:** el día entero, del amanecer a la noche. Las ilustraciones se pintan con
luz de día despejado y sin tinte, que es la de la tarde (recolección y construcción); el
amanecer (puente I), el atardecer (escena 2.4 y laberinto) y la noche junto al fuego (escena
2.5 y arranque del puente II) los pone el motor encima —`NarrativeLight` en las narrativas,
`MazeLayout.LightTint` en el laberinto—, no el dibujo. Lo vigilan
`NarrativeSequence_RF05_ElNivel2TranscurreDelAmanecerALaNoche` y
`MazeScene_RF30_ElLaberintoEsAlAtardecer`.
**Sensación buscada:** claridad y espacio para observar, comparar y construir. Es el
nivel más luminoso de los tres.

> **Es un bosque, no un desierto.** El guion §6.1.1 sitúa la fase 1 en un «Bosque.
> Objetos dispersos por el suelo», la fase 2 en el área de trabajo junto al refugio y la
> fase 3 en un sendero cerrado por vegetación. No hay meseta, ni cañón, ni arena, ni
> cactus en ninguno de los documentos fuente. Una versión previa de esta sección
> describía un cañón desértico; se corrigió el 31/08/2026 por precedencia del guion.

#### Acorde cromático

| Función | Familia | Colores |
| --- | --- | --- |
| Dominante (60 %) | Verdes de follaje, escalonados por profundidad | Follaje cercano `#7FA05A`, medio `#5A7A3F`, lejano `#3C5429`, planta baja `#6E9B4E` |
| Secundaria (30 %) | Tierra y cielo | Suelo de tierra `#8A6B4A` / sombra `#6B5344`, cielo `#A8DCE6`, nubes `#F2F7F5` / sombra `#D8E4E8` |
| **Acento (10 %)** | **Madera trabajada, clara y cálida** | **`#C79A5E` base, `#A67C4A` sombra** — par exclusivo, no aparece en el decorado |

**Prohibición de acento.** El acento de este nivel no es un color cualquiera: es la
**madera cortada y trabajada** —la cara circular clara del tronco seccionado, la tabla,
el eje, la rueda—. Ningún elemento de decorado puede usar ese tono claro. Los troncos
de los árboles del fondo llevan corteza oscura y desaturada (`#5C4530`), nunca el
`#C79A5E` de la madera trabajada.

Esta elección hace doble trabajo. Separa lo interactivo de lo decorado como en los otros
niveles, y además dice lo que el nivel enseña: lo **fabricado** se distingue de lo
**natural**, que es exactamente el descubrimiento de la niña (§2.5).

**Distractores.** Lo que no rueda se pinta frío y mineral —piedra `#7A8290` / sombra
`#4E5561`— o vegetal `#6E9B4E`. Aun así, **la forma tiene que bastar por sí sola**: la
diferencia entre un cilindro y una piedra facetada se lee en negro sólido (RNF-19).

#### Vocabulario de formas

Claro de bosque abierto: troncos verticales de sección ovalada que enmarcan la escena
sin cerrarla, copas construidas con círculos superpuestos en tres profundidades, suelo
de tierra compacta con ondulaciones muy suaves. Arbustos como manchas planas
redondeadas.

Es el nivel donde aparecen las primeras formas **angulosas** del juego: la tabla, el
eje, la cuña, las herramientas. Su angulosidad es intencional y las marca como objetos
fabricados frente al mundo redondeado que las rodea (§2.5).

#### Profundidad

Cuatro capas de parallax, sin cordillera: en un bosque la profundidad la dan las capas
de follaje.

| Capa | Contenido | Desaturación | Velocidad |
| --- | --- | --- | --- |
| Primer plano | Arbustos y hojas grandes que enmarcan | 0 % | 1.2× |
| Juego | Personajes, objetos del reto, área de trabajo | 0 % | 1.0× |
| Plano medio | Troncos cercanos y follaje `#5A7A3F` | 15 % | 0.6× |
| Fondo | Masa de follaje `#3C5429`, sin contorno | 45 % | 0.25× |
| Cielo | Dos bandas planas visibles entre las copas | — | Fijo |

El cielo no usa degradado: son dos bandas de color plano separadas por un borde
ondulado resuelto como forma, no como transición, y solo se ve **a través de los huecos
del follaje**, nunca como un horizonte abierto.

#### Nubes

Formas de círculos superpuestos, color plano `#F2F7F5` con sombra inferior `#D8E4E8`.
Sin contorno. Tres tamaños, desplazamiento horizontal lento. Se ven poco: el follaje
tapa la mayor parte del cielo.

#### Elementos de decorado

Helechos y matas bajas `#6E9B4E`, hongos redondeados de sombrero plano, troncos caídos
cubiertos de musgo `#4A5C42`, piedras de canto rodado `#7A8290` semienterradas, y flores
pequeñas de cinco pétalos en tonos fríos. **Nada de madera clara trabajada en el
decorado**, por la regla de acento.

---

### 8.3 Nivel 3 — El Río

**Descubrimiento:** el cruce del río
**Momento del día:** mañana temprano, con niebla baja
**Sensación buscada:** frescura, movimiento y un obstáculo que exige planificar antes
de actuar. Es el nivel más denso en vegetación y el de paleta más fría de los tres
diurnos.

#### Acorde cromático

| Función | Familia | Colores |
| --- | --- | --- |
| Dominante (60 %) | Verdes de follaje | Follaje `#4E8C3F` / sombra `#37662B`, follaje claro `#6FA84E` / sombra `#54803A` |
| Secundaria (30 %) | Azules de agua | Agua `#3E8FA8` / sombra `#2B6B80`, espuma `#D6F0F5` |
| **Acento (10 %)** | **Ámbar cálido** | **`#E8A33D`, `#F2C46B`** |

**Prohibición de acento:** el ámbar se reserva para las piedras de paso, los troncos
utilizables y las lianas interactivas. Ningún elemento de vegetación o roca decorativa
puede usarlo.

#### Vocabulario de formas

Orillas de línea sinuosa y continua. Follaje construido con círculos superpuestos de
dos tonos de verde, nunca con hojas individuales. Troncos de sección ovalada con
contorno grueso. Rocas de río muy redondeadas y aplanadas, apiladas en grupos de dos
o tres.

#### El agua

Elemento con el mayor peso visual del nivel. Se construye en tres capas planas:

| Capa | Color | Comportamiento |
| --- | --- | --- |
| Cuerpo del agua | `#3E8FA8` | Estático |
| Bandas de corriente | `#5AA8BF` | Franjas horizontales de anchura irregular, desplazamiento continuo |
| Espuma de superficie | `#D6F0F5` | Arcos de línea gruesa cerca de rocas y orillas, ciclo de 4 frames |

El agua no es transparente ni tiene reflejos degradados. La sensación de profundidad
se logra oscureciendo la banda central del cauce con `#2B6B80` y aclarando los bordes
cerca de las orillas.

**Sobre el riesgo:** el agua nunca se representa como amenazante. No hay rápidos
violentos, no hay espuma turbulenta, no hay oscuridad bajo la superficie. Nadie cae al
agua: lo que no aguanta es la balsa, que se inclina, se hunde un poco y vuelve a su sitio.
La salpicadura es sonora (`sfx_n3_hundimiento`) y en imagen no hay efecto de agua ni
animación de peligro (INC-123). Se detalla en §12.3.

#### Niebla

Franja horizontal de color plano `#D6F0F5` al 30 % de opacidad sobre el plano medio,
con borde superior ondulado. No es un degradado ni un shader: es un sprite. Oscila
verticalmente 8 px en un ciclo de 6 s.

#### Elementos de decorado

Helechos como abanicos de tres o cuatro hojas planas, juncos en `#8CA84E` como líneas
gruesas con punta redondeada, flores pequeñas de cinco pétalos en `#B87FC4` y `#7FA8E0`
(nunca en ámbar, por la regla de acento), libélulas como dos óvalos y cuatro elipses
transparentes.

#### Plano y escala

El Nivel 3 se juega y se narra sobre una sola ilustración 16:9, `env_n3_rio`, sin duplicar.
La recolección la encuadra a ×1,4: la orilla con el río y el pie de la cascada a la derecha.
El ensamblaje empuja a ×1,6, con la familia en la orilla a la izquierda y la balsa sobre el agua
a la derecha. Con ese plano abierto, Mamá, la familia, los materiales y la zona de construcción
toman la escala con que los muestra la escena 3.1, y la balsa del ensamblaje mide lo que la que
cruza en la 3.3 (INC-118). La balsa del cruce navega por debajo de la espuma de la cascada, a
y 0,395, con los cuatro viajeros encima (INC-124).

---

## 9. Props y objetos interactivos

### 9.1 Regla de separación

Ningún objeto se dibuja en la mano de un personaje. Todos los props son assets
independientes con su propio contorno cerrado. Razones:

1. Permite las mecánicas de recoger, soltar e intercambiar, necesarias para los retos
   de secuenciación.
2. Evita redibujar el prop en cada frame de animación.
3. El mismo asset sirve como icono de interfaz sin trabajo adicional.

En Unity cada prop es un objeto propio sobre la ilustración —un `NarrativeProp` en la
escena narrativa, una `Image` en las mecánicas— y no un hijo de la mano del personaje: la
animación por recorte (§13.1) no tiene huesos, y ningún prop se monta sobre una parte del
cuerpo.

### 9.2 Tratamiento visual de lo interactivo

| Propiedad | Objeto interactivo | Objeto decorativo |
| --- | --- | --- |
| Grosor de contorno | 7–9 px | 4 px |
| Color de contorno | `#3A1E18` | `#5C4038` |
| Saturación | Máxima | Reducida 20–40 % |
| Color de acento del nivel | Permitido | Prohibido |
| Animación en reposo | Flotación vertical de 4 px, ciclo 2 s | Ninguna |

### 9.3 Inventario de props del prototipo

El inventario es el del juego que describen el guion y los RF: no hay recolectables
sueltos por el escenario, ni objetos empujables, ni plataformas colocables, porque no
hay desplazamiento libre en los niveles 1 y 2 y el del 3 es por la orilla con botones en
pantalla.

| Nivel | Prop | Función | Color dominante |
| --- | --- | --- | --- |
| 1 | Montón de hojas secas (3 estados) | Objetivo del reto: intacto; humeante al converger, con un hilo de humo que nace en el punto del golpe, entre las hojas y las piedras; y encendido al soplar: la llama cenital encima, el humo en su corona y las hojas quemándose desde el centro. Es un solo dibujo, `prop_n1_monton_hojas_cenital`, y los estados los pone el motor (§12.2) | `#B08541` |
| 1 | Sílex | Pieza del panel, silueta angulosa | `#9BA0A8` |
| 1 | Pedernal | Pieza del panel, silueta redondeada | `#8B5A3C` |
| 1 | Hoguera | Resolución del nivel, única fuente de luz cálida | `#F5A62E` / `#E2571F` / `#FFE9A8` |
| 2 | Objetos del bosque (válidos y distractores) | Selección por patrón, fase 1 (RF-22..RF-26) | Verde vivo solo en los válidos |
| 2 | Caja de alimentos (3 estados) | Meta narrativa de la fase 1. Un solo dibujo, la caja llena `prop_n2_caja_suelo`, en el suelo, sobre los troncos y rodando; la escena 2.2 abre con la misma caja, en el sitio y con el tamaño en que la deja el bosque (INC-120) | `#C4743E` |
| 2 | Siete piezas del taller (la séptima, la cuerda) | Ensamblaje secuencial, fase 2 (RF-27..RF-29) | `#8B5A3C` + acento verde |
| 2 | Rueda y carretilla (5 estados) | Resultado del ensamblaje: `prop_n2_carretilla_e1`…`_e5`. El taller termina en `e5` al amarrar la cuerda, el mismo dibujo con el que abre la escena 2.4 (INC-121) | `#A89880` + `#5FA842` |
| 2 | Carretilla cenital (4 orientaciones) | Ejecución de la secuencia, fase 3 (RF-30..RF-33) | `#5FA842` |
| 2 | Bloques de instrucción y botón «Ejecutar» | Editor de secuencia, fase 3 (RF-31, RF-32) | `#5FA842` sobre marfil |
| 3 | Troncos y sogas | Materiales recolectables en la orilla (RF-36..RF-39) | `#E8A33D` |
| 3 | Mástil y vela | Materiales de la tercera fase de ensamblaje (RF-40) | `#E8A33D` / `#F2C46B` |
| 3 | Balsa compuesta: 17 espacios (cinco troncos, diez amarres, mástil y vela) que se pintan uno a uno, silueta hasta llenarse y pieza después, con 8 sprites —una pieza y una silueta por clase—; más la balsa hundida (3.2) y la balsa cruzando (3.3) de las narrativas, que navega bajo la espuma de la cascada (INC-124) | Construcción por fases: base, amarre, mástil y vela | `#8B5A3C` + `#E8A33D` |
| 3 | Botones de dirección y «Recoger» | **Interfaz**, no props: la entrada del nivel (RF-35, CT-06) | `#E8A33D` sobre marfil |

**Ningún prop se dibuja en la mano de un personaje** (§9.1), y ninguno usa el color de
acento de su nivel si no es interactivo (§4.2).

---

## 10. Interfaz de usuario

### 10.1 Principios

**Diegética cuando sea posible.** Los elementos de interfaz imitan materiales del
mundo del juego: los paneles son tablillas de piedra o de madera, los botones son
piedras redondeadas, los marcos son cuerdas trenzadas. Esto reduce la ruptura entre
mundo e interfaz y refuerza la ambientación sin coste narrativo.

**Mínima permanencia.** En pantalla solo permanece lo indispensable: el botón de pausa
(RF-07), el de pista y la tablilla de mensajes del guía; en el bosque del Nivel 2, el
contador de acopio, que RF-24 pide permanente, y **solo en el Nivel 3**, la lista de cuatro
tareas (RF-36) y el inventario. Los niveles 1 y 2 **no llevan lista de tareas ni barra de
progreso**: RNF-03 restringe la tarea **activa** a una, y añadir un marcador que no pide
ningún RF sería una mecánica nueva (INC-41). Todo lo demás aparece por contexto y desaparece.

**Sin cifras a la vista del estudiante.** Ni intentos, ni pasos, ni tiempo, ni puntaje,
en ninguna pantalla del juego ni en el resumen de fin de nivel (CP-03, RF-17, RF-45).
Los números existen solo en el informe docente (RF-46).

**Área táctil generosa.** Ningún botón de acción mide menos de 88×88 px a
resolución de diseño. La motricidad fina de un niño de nueve años no es la de un
adulto. Las excepciones son los botones de avance del cuadro de diálogo, de 76 px de
alto, y los controles finos del editor de bloques del laberinto (cuenta, giro y
desplazamiento de la lista, de 52 a 82 px).

### 10.2 Componentes

| Componente | Material aparente | Color base | Notas |
| --- | --- | --- | --- |
| Panel de diálogo | Tablilla de piedra clara | `#F7EFE2` con borde `#C4A882` | Esquinas muy redondeadas (32 px) |
| Botón primario | Piedra redondeada | `#E8A33D`, borde `#3A1E18` | Sombra plana inferior de 6 px |
| Botón secundario | Piedra clara | `#E0D4C0`, borde `#6B5248` | |
| Lista de tareas (**solo Nivel 3**) | Tablilla de piedra clara | `#F7EFE2` sin borde; pendiente carbón suave `#6B5247`, cumplida verde `#336638` | Esquina superior izquierda, una fila por tarea de RF-36 con un círculo delante del texto. Tarea pendiente = círculo liso; tarea cumplida = círculo verde con marca de verificación, y el texto pasa al mismo verde: cambia la forma **más** el color, nunca solo el color (RNF-19). Sin cifras. Es el único componente que renuncia a la vía diegética de §10.1 (INC-46): el Nivel 3 es el escenario más claro, y una tablilla de marfil se lee sobre el follaje mejor que una cuerda suelta (RNF-20) |
| Icono de pista | Algoritm en pequeño | `#E8A33D` | Pulso lento de escala cuando hay pista disponible. Es el guía quien ofrece la pista (CP-06), así que el icono es él |
| Marco de inventario | Panel liso color arena | `#C7A87C` | Casillas cuadradas `#E0D4C0` en rejilla de 2×2, abajo a la izquierda; la de los troncos lleva cinco marcas que se encienden sin cifra (CP-03) |

### 10.3 Cuadro de diálogo

El diálogo se lee en un cuadro con retrato, no en un globo con cola (INC-129). Es el
`CuadroDialogo` de `Narrative.unity`, el panel de diálogo de §10.2: una tablilla de
1400 × 180 px centrada abajo, a 16 px del borde, dentro del cuarto inferior de la pantalla, con
relleno marfil `#F7EFE2` en un marco `#C4A882` y una sombra plana inferior de 6 px, sin
degradado. Arriba a la izquierda lleva el retrato del hablante en un recuadro de 128 px
(`char_<x>_retrato_neutra.png`, o la forma de Algoritm de su nivel). A su derecha van el nombre
del hablante y el texto (§11.3). Abajo a la derecha quedan «Continuar», botón primario de
260 × 76, y «Omitir», secundario de 200 × 76, que solo aparece en escenas ya vistas (RF-06).

No tiene cola: quién habla lo dicen el retrato y el nombre, y no una flecha hacia un personaje
que la cámara puede dejar fuera de cuadro. Lo que el texto nombra se ve por encima del cuadro
(acta D05; lo vigila `NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo`).

El texto es siempre `#3A1E18` sobre marfil. Nunca texto claro sobre fondo oscuro: la
legibilidad para lectores en formación es notablemente peor.

---

## 11. Tipografía

### 11.1 Criterios de selección

Para lectores de 9 a 11 años, la tipografía debe cumplir cuatro condiciones: formas
redondeadas y abiertas, distinción inequívoca entre caracteres confundibles
(`I` / `l` / `1`, `O` / `0`), altura de x generosa, y licencia libre que permita su
uso en un trabajo académico sin restricciones.

### 11.2 Familias recomendadas

| Uso | Familia | Licencia | Justificación |
| --- | --- | --- | --- |
| Títulos y encabezados | **Baloo 2** | SIL OFL 1.1 | Peso alto, formas redondeadas, carácter lúdico sin perder legibilidad |
| Diálogo y cuerpo | **Nunito** | SIL OFL 1.1 | Terminaciones redondeadas, excelente altura de x, muy legible a tamaño pequeño |
Los números e indicadores no llevan familia propia: el contador del bosque va en Baloo 2
Bold, y las cifras del informe docente, en Nunito.

Las dos soportan caracteres del español (tildes, `ñ`, signos de apertura `¿` `¡`),
requisito no negociable.

### 11.3 Escala tipográfica

Definida a resolución de diseño 1920×1080.

| Nivel | Tamaño | Familia | Peso | Interlineado |
| --- | --- | --- | --- | --- |
| Título de pantalla | 64 px (el título del juego en la portada, 96 px en Baloo 2 ExtraBold) | Baloo 2 | 700 | 1.15 |
| Subtítulo | 48 px | Baloo 2 | 600 | 1.2 |
| Diálogo | 26 px (nombre del hablante en Baloo 2 Bold a 22 px) | Nunito | 400 | 1.35 |
| Tablilla del guía en las mecánicas (instrucción, mensajes y pista) | 26 px en el taller y el laberinto, 28 px en el río, 34 px en el bosque; 24 px en el Nivel 1 | Baloo 2 (Nunito en el Nivel 1) | 700 (400 en el Nivel 1) | 1.0 |
| Texto secundario | 26 px | Nunito | 400 | 1.5 |
| Contadores | 30 px | Baloo 2 | 700 | 1.0 |

**Mínimo: 26 px para el texto que lee el estudiante.** Solo bajan de ese tamaño los rótulos
secundarios —nombre del hablante, rótulo «Algoritm» del resumen, etiqueta del laberinto, «Aún no»
del taller, a 22 px— y la instrucción del Nivel 1 (24 px).

### 11.4 Reglas de composición

- Máximo 2 líneas por cuadro de diálogo, 12 palabras por línea.
- Alineación a la izquierda, nunca justificada.
- Sin mayúsculas sostenidas en textos de más de tres palabras: entorpecen la lectura
  en formación.
- Contorno de texto: `#3A1E18` de 3 px cuando el texto va sobre el escenario y no
  sobre panel.

---

## 12. Efectos visuales y retroalimentación

### 12.1 Principios

Todos los efectos se resuelven con sprites de color plano y animación por fotogramas.
No se usan sistemas de partículas complejos ni posprocesado, y los únicos shaders propios son dos
de color plano —`fx_oscuridad`, la capa que oscurece e ilumina (Nivel 1 y `NarrativeLight`), y
`fx_contraste`, el contraste del laberinto (`MazeLayout.Contrast`)—:
la restricción de bajo consumo de recursos declarada en el alcance del proyecto lo
impide, y el estilo plano no los necesita.

### 12.2 Catálogo de efectos

| Efecto | Construcción | Duración |
| --- | --- | --- |
| Chispa (Nivel 1, golpe efectivo) | Un solo rayo recto `#FFE9A8` de 4 u de grosor (8 px a 1080p con la cámara al doble), sin halo ni llama. Nace en el punto del golpe y cae en las hojas, en una dirección al azar de la mitad de abajo del montón, a 0,205–0,22 de su lado | 0,2 s por golpe efectivo, hasta 0,6 s con el tercero |
| Chispa que se apaga (Nivel 1, fuerza de más con las piedras en su sitio) | El mismo rayo, más largo: sale hacia la mitad de arriba, a 0,52–0,6 del lado, pasa de la última hoja y se apaga en el aire, bajo la tablilla, sin prender nada (RF-16). Mismo trazo y mismo color: otro color lo volvería marca de error (CP-02, §12.3) | 0,35 s |
| Humo del montón (Nivel 1) | Hilo de humo por cuadros (`fx_n1_humo_nacer`, que sigue en bucle con `fx_n1_humo`) con el pivote en la base, en el punto del golpe y de 87 × 150 u. Nace al converger, entre las hojas y las piedras, y no pasa de medio montón (RF-19). Al soplar sube a la corona de la llama, detrás de ella, y se encoge a 0,6 en el mismo barrido que el quemado (RF-20) | Nacer, 1,97 s; bucle, 7,27 s |
| Salpicadura de la balsa (Nivel 3) | Solo sonora: `sfx_n3_hundimiento` al empezar cada hundimiento. En imagen no hay efecto de agua (§12.3, INC-123) | 1,74 s de sonido |
| Recolección de objeto | Círculo `#F7EFE2` que se expande y desaparece | 0.35 s |
| Reto resuelto | 6 destellos de 4 puntas `#5FA842` en corona | 0.8 s |
| Aparición de pista | El botón de pista del Nivel 1 pulsa de escala 1.0 a 1.06 en bucle desde que se abre la escena (`ui_pulso_pista`); los de los niveles 2 y 3 no pulsan | Ciclo 2.2 s |

Los dos rayos de la chispa hacen un único barrido (INC-119): la cabeza llega a la caída en la
primera mitad del tiempo, la cola la alcanza en la segunda y ahí se apaga, sin volver ni oscilar
en escala ni en alfa (RNF-21). Un golpe nuevo corta el rayo anterior. El humo de las narrativas
es el mismo `fx_n1_humo`, detrás de cada llama y a 0,6 de su escala.

### 12.3 Retroalimentación de error

**Decisión de dirección: el error no se marca en rojo, no produce sonido de fallo y no
genera expresión negativa en el personaje.**

El rojo y los indicadores de fallo comunican castigo. En un juego cuyo fundamento
pedagógico es el ensayo y error en un entorno seguro, marcar el error como falta
contradice el propio enfoque y desincentiva la exploración, que es exactamente la
conducta que el juego quiere provocar.

El tratamiento en su lugar:

| Situación | Respuesta visual |
| --- | --- |
| Secuencia incorrecta | La pieza vuelve a su sitio al soltarla, sin animación ni destello; el personaje hace el gesto de ánimo (`Encourage`) y la tablilla dice qué falta antes |
| Prueba de balsa sin éxito (Nivel 3) | «Probar balsa», en cualquiera de las tres fases (INC-122): la balsa gira hacia un costado, se hunde un poco y vuelve a su sitio en un solo movimiento continuo de 0.6 s (guion §1.8.4, RNF-21). Suena la salpicadura (`sfx_n3_hundimiento`), que describe y no castiga, y en imagen no hay efecto de agua ni destellos (INC-123). En la base y el amarre la prueba no aprueba la fase y marca solo lo mal puesto, nunca los espacios vacíos: marcarlos sería un mapa de dónde va cada pieza (CP-06). Lo ya confirmado **no se pierde** (RF-41, RF-43) |
| Intento repetido sin éxito (3 veces) | La tablilla muestra la pista del guía, una pregunta, con el icono de ayuda (`HintPolicy`); el botón de pista no cambia |

No hay rojo de error en toda la interfaz. El estado del intento lo lleva el icono que
acompaña al mensaje del guía, con forma y color propios: verde oscuro `#336638` para lo
aceptado y la tarea cumplida, ocre `#995C1A` para lo devuelto, azul pizarra `#3D4C70` para la
instrucción y la pista; carbón suave `#6B5247` para la tarea pendiente del Nivel 3; naranja
`#D96B29` para el espacio equivocado de la balsa, y ámbar de atención `#E8A33D` para el bloque
elegido y el refugio del laberinto. El asa del deslizante de fuerza del Nivel 1 va de azul a
rojo como escala de intensidad, no como error.

---

## 13. Animación

### 13.1 Enfoque técnico

Animación por recorte (*cut-out*) en uGUI sobre los sprites base en A-pose, no
animación fotograma a fotograma. Cada miembro de la familia se corta en cinco partes
—torso, brazo izquierdo, brazo derecho, pierna izquierda y pierna derecha, con izquierda y
derecha de pantalla— que son `Image` hijas de un lienzo de 1024 × 1024, el de los sprites
base, con el pivote en la articulación (`char_<x>_parte_<parte>.png`). Un `Animator` las gira
y desplaza con un estado por acción del juego (`ActorAction`) y un clip por estado
(`char_<x>_anim_<accion>.anim`, 21 por miembro de la familia, con el controlador
`char_<x>.controller` al lado). El componente es `CharacterRig` (`Game.Scaffolding`), hay un
prefab por personaje en `Assets/Game/Prefabs/Characters/`, y lo usan igual la escena narrativa
y las cinco mecánicas. Algoritm es una sola `Image` por forma, sin recorte, con nueve clips que
comparten las tres formas, para que sustituir su arte sea cambiar un archivo.

Razones: el equipo es de dos personas con catorce semanas, y los generadores de imagen no
producen secuencias de frames consistentes entre sí. No se usa el rigging con huesos del paquete
2D Animation: todas las escenas son uGUI en un Canvas *Screen Space Overlay*, y `SpriteSkin`
solo deforma un `SpriteRenderer`, que quedaría debajo del Canvas, tapado por la ilustración.
Como `Image` hijas, las partes cuelgan de la ilustración y acompañan el paneo y el zoom de la
cámara como cualquier otro objeto (INC-53).

Consecuencia sobre el arte: los sprites base deben tener brazos y piernas
completamente separados del torso, con fondo visible entre ellos. Un brazo fundido
con el cuerpo obliga a inventar dónde termina al recortarlo en su parte.

### 13.2 Principios de animación aplicados

- **Anticipación:** todo movimiento amplio se precede de un contramovimiento breve
  (echar el brazo atrás antes de golpear, tomar aire antes de soplar).
- **Exageración moderada:** el estilo admite deformación, pero no debe romper la
  silueta reconocible del personaje.
- **Squash and stretch:** aplicado en el golpe, el soplo y la celebración, con un
  máximo de 15 % de deformación. Más que eso rompe la lectura del personaje.
- **Arcos:** las extremidades se mueven en curva, nunca en línea recta.
- **Idle permanente:** el personaje jugable nunca queda completamente inmóvil.
  Respiración de 2 px de desplazamiento vertical en ciclo de 3 s.

### 13.3 Set mínimo de animaciones

Derivado de lo que el juego hace de verdad. **No hay saltar, caer ni aterrizar**: no
existe salto en ningún nivel (CT-06, RNF-02).

| Animación | Personaje | Duración | Prioridad |
| --- | --- | --- | --- |
| Idle (respiración de 2 px) | Todos | Ciclo 3 s | Crítica |
| Flotación y giro del guía | Algoritm | Ciclo 2 s | Crítica |
| Golpear las piedras | Papá (N1) | 0.6 s | Crítica |
| Soplar | Papá (N1) | 0.9 s | Crítica |
| Señalar / observar | Niña (N2), Algoritm | 0.6 s | Alta |
| Caminar: un solo ciclo que se voltea a izquierda o derecha según la dirección | Mamá (N3) | Ciclo 0.8 s | Crítica |
| Recoger material | Mamá (N3) | 0.5 s | Alta |
| Celebrar cierre de fase | El que corresponda | 1.2 s | Media |
| Ánimo tras un intento sin avance | Todos | 0.9 s | Media |

El **ánimo** sustituye a cualquier animación de derrota o desánimo: no existen (CP-02,
§7.3).

---

## 14. Accesibilidad visual

### 14.1 Contraste

Todo texto cumple una relación de contraste mínima de 4.5:1 sobre su fondo. El par
principal —`#3A1E18` sobre `#F7EFE2`— supera holgadamente ese umbral.

Los elementos interactivos mantienen al menos 3:1 respecto al fondo inmediato.

### 14.2 No depender del color

Aproximadamente uno de cada doce niños presenta alguna forma de daltonismo. Ninguna
información crítica del juego se transmite únicamente por color:

| Información | Canal de color | Canal redundante |
| --- | --- | --- |
| Objeto interactivo | Color de acento del nivel | Contorno más grueso + flotación |
| Reto resuelto | Verde `#5FA842` | En la lista de tareas del Nivel 3, el círculo liso de la tarea pendiente se cambia por un círculo con marca de verificación; en la frase del guía de los niveles 2 y 3, el acierto lleva un icono de forma distinta del de alerta |
| Pista disponible | Ámbar `#E8A33D` | Pulso de escala del icono de Algoritm |
| Casilla o espacio de ensamblaje válido | Contraste de valor | Borde más claro **y** marca de forma en la casilla |

**Validación:** revisar cada nivel con un simulador de deuteranopía y protanopía. Si
un objeto interactivo deja de distinguirse del fondo, se refuerza con forma o
movimiento, no con otro color.

### 14.3 Movimiento

Todas las animaciones ambientales (niebla, nubes, corriente, flotación de objetos)
tienen amplitudes pequeñas y ciclos lentos. No hay parpadeos rápidos ni destellos de
alta frecuencia, que pueden resultar molestos o desencadenar malestar.

---

## 15. Especificaciones técnicas para Unity

### 15.1 Resolución y unidades

| Parámetro | Valor |
| --- | --- |
| Resolución de diseño | 1920 × 1080 |
| Pixels Per Unit (PPU) | 100 |
| Altura de papá en unidades | 1.8 |
| Altura de los niños en unidades | 1.08 |
| Altura de cámara visible | 10 unidades |

### 15.2 Importación de sprites

Son los ajustes que aplica el proyecto (INC-128):

| Ajuste | Valor |
| --- | --- |
| Texture Type | Sprite (2D and UI) |
| Sprite Mode | Single. El importador de fábrica trae `Multiple`, y con él `LoadAssetAtPath<Sprite>` devuelve nulo para un fondo o un prop entero: cada imagen nueva se pasa a `Single` desde el motor (`TextureImporter.spriteImportMode`). Excepciones, abajo |
| Pivot | Center en el importador. El pivote que cuenta en un personaje vive en su prefab: cada parte del rig gira en su articulación (§13.1) y la casilla se ancla por los pies, en (0,5; 0,075) |
| Filter Mode | Bilinear |
| Generate Mip Maps | Desactivado |
| Compression | Sin comprimir (`ArtImportRules`) |
| Max Size | 4096 (`ArtImportRules`): las panorámicas llegan a 3840 y no se reducen. Excepción: los cuadros de `Props/Fire/Animations/` entran a 1024 (INC-130) |
| Read/Write | Desactivado, salvo en las ocho piezas y siluetas de la balsa del Nivel 3: el panel prueba su alfa al agarrar y al soltar |
| Generate Physics Shape | El valor de fábrica, sin efecto: todo es uGUI y no hay colisionadores |

**Sin comprimir y a 4096, por regla.** `ArtImportRules` (`Game.EditorTools`) los fuerza en todo
`Assets/Game/Art/`, también al reimportar: un archivo recién soltado entra con los valores de
fábrica (comprimido, 2048) y nadie lo nota hasta verlo en pantalla. Comprimida, la ilustración
plana enseña la rejilla de bloques de 4×4 —se ve en la pared de la cueva— y el Nivel 1 la
multiplica por la capa de oscuridad, que amplifica el error en los tonos oscuros. Lo vigila
`ArtImport_RNF23_LasIlustracionesEntranSinComprimirYSinReducir`; la memoria se mide sobre el
ejecutable (RNF-05).

**Excepción: los cuadros del fuego y del humo del Nivel 1 (INC-130, RNF-06).** `Props/Fire/Animations/`
entra a **1024 px de lado máximo**, también sin comprimir. Sin la excepción eran 67 PNG referenciados
(134 entradas en el informe del build) de hasta 2144 × 2108 y 533 MB, y el paquete pesaba 827 MB frente
al tope de 500 MB (RNF-06). Ya son un dibujo por clave de la curva, sin duplicados, así que no hay
arreglo sin pérdida. En pantalla el fuego se ve a unos 650 px como mucho y el humo a menos de 300:
el humo queda en 595 × 1024 y el fuego normal en 1024 × 1007, en memoria, sin pérdida visible; el fuego
cenital (500 × 278) no cambia. El PPU se escala con la textura y el tamaño en el mundo no se mueve. Lo
vigila `ArtImport_RNF06_LosCuadrosDelFuegoYElHumoSeImportanAMil24SinComprimir`; con la excepción el
build pesa 479,0 MB.

**Halo de croma.** Los sprites que salen de un fondo verde puro se limpian de halo verde en el
borde. El 01/10/2026 se limpiaron siete PNG —los tres `char_algoritm_n?_*_reposo` y los cuatro
retratos `char_*_retrato_neutra`— tocando solo píxeles semitransparentes, con el alfa idéntico al del
original. Quedan unas 12 o 13 motas verdes opacas (α ≥ 200) en las puntas del pelo de los retratos de
Niña y Papá y de Algoritm, visibles a 1080p: limpiarlas obliga a retocar píxeles opacos, y queda
pendiente del carril de arte.

**Doce PNG siguen en `Multiple`.** Son `Environments/Narrative/env_enlace_n2.png` y once de
`Props/Wheel/`: `prop_n2_tronco_a`, `prop_n2_piedra_a`…`_d`, `prop_n2_planta_a`…`_c` y
`prop_n2_herramienta_a`…`_c`. Diez los referencian `N2_WheelLevelConfig` y las narrativas del
Nivel 2 y del puente II por su sub-sprite, con un `fileID` distinto de `21300000`: pasarlos a
`Single` rompería esas referencias, así que no se migran. `prop_n2_piedra_c` y `_d` no los usa
ningún asset. Si uno de los doce se sustituye por un archivo de otro tamaño, el recorte del
sub-sprite se ajusta desde el motor conservando su `spriteID`, como se hizo con las piedras el
25/09/2026.

### 15.3 Orden de capas

| Sorting Layer | Order | Contenido |
| --- | --- | --- |
| `UI` | 100 | Interfaz |
| `Foreground` | 50 | Decorado delante del jugador |
| `Characters` | 30 | Familia y NPC |
| `Interactables` | 25 | Objetos del reto |
| `Platforms` | 20 | Superficies navegables |
| `Midground` | 10 | Decorado del plano medio |
| `Background` | 0 | Fondo lejano y cielo |

### 15.4 Nomenclatura de archivos

```
[categoria]_[sujeto]_[variante]_[estado].png

char_nino_retrato_neutra.png
char_mama_cenital.png
char_papa_parte_brazo_der.png
prop_n1_monton_hojas_cenital.png
prop_n2_carretilla_e5.png
env_n2_bosque_claro.png
env_n3_rio.png
```

Prefijos: `char_`, `prop_`, `env_`, `ui_`, `fx_` y `ref_` (`S01`). Una secuencia de cuadros entra como un
`.anim` con curva PPtr sobre `Image.m_Sprite` y un `.controller` por clip (convención en
`Assets/Game/Art/Inventario.md`); los cuadros del fuego y del humo conservan sus nombres de entrega.
Niveles: `n1` (La Oscuridad), `n2` (La Rueda), `n3` (El Río).

Sin tildes, sin espacios, sin mayúsculas en los nombres de archivo.

Todos los sprites de `Assets/Game/Art/` la cumplen, y lo vigila
`ArtImport_RNF23_LosNombresSiguenLaNomenclatura`. Los cinco que llegaron fuera de ella se
renombraron desde el motor (`AssetDatabase.RenameAsset`), con el GUID intacto y sin tocar escenas
ni assets (INC-126): `env_n1_apertura`, `env_n1_cueva_2x`, `env_n1_cueva_cenital` y
`env_n2_laberinto`, que traían el prefijo viejo `entorno_`, y `prop_n2_caja_suelo_vacia`, que
traía tilde. Solo se apartan de la regla los cuadros del fuego y del humo
(`fuego_cenital_nivel_1_0000.png`, `humo_nivel_1_0009.png`): conservan el nombre de entrega
porque los referencia la curva del `.anim`, y renombrarlos es trabajo del motor.

---

## 16. Prompts de generación de entornos

Estructura paralela a la de los personajes. El bloque base de entorno se usa completo
para el primer asset de cada nivel; los siguientes usan referencia adjunta.

### 16.1 Bloque base de entorno

```
CONTEXTO DEL ENCARGO
Estoy produciendo los assets de entorno de un videojuego educativo 2D para niños de 9 a 11
años (grado cuarto), desarrollado en Unity. El juego sigue a una familia prehistórica a través
de tres niveles de retos de pensamiento computacional: encender el fuego, inventar la rueda y
cruzar un río en balsa.

Necesito un elemento de escenario aislado, sobre fondo plano recortable, que después voy a
recortar e importar a Unity como pieza de decorado. No es una ilustración de escena
completa: es una pieza modular que se combinará con otras para construir el nivel.

Dos condiciones marcan el diseño. Primera: el público es infantil, así que nada puede
resultar amenazante, afilado ni sombrío. Segunda: el prototipo debe correr con bajo consumo
de recursos, así que el arte es gráficamente simple y de lectura clara a tamaño pequeño.

ESTILO: ilustración vectorial 2D, cartoon clásico americano de los años 40-50. Colores
completamente planos y saturados, sin degradados de ningún tipo.

SOMBRAS (CRÍTICO): exactamente dos tonos por color, base y sombra, separados por un borde
duro y nítido. PROHIBIDO degradados, aerógrafo, difuminado, transiciones suaves, textura,
ruido, volumen pintado. Luz desde arriba a la izquierda a 45 grados.

LÍNEA: contorno de 4 px en marrón medio-oscuro, más fino que el de los personajes. Los
elementos de decorado NO deben competir visualmente con los personajes.

FORMA: todo redondeado. Rocas de cantos romos, sin puntas afiladas. Sin texturas de
superficie: una roca es una forma de color plano con su contorno y su sombra, nada más.

FONDO: verde croma puro #00FF00, plano y uniforme, sin ningún otro elemento, sin sombra
proyectada sobre el fondo.

ENCUADRE: el elemento completo dentro del lienzo, con al menos un 10 % de margen vacío a
cada lado. Contorno cerrado en todo su perímetro.

FORMATO: PNG, cuadrado, sin compresión con pérdida.

──────────────────────────────────────────────────────────────

ELEMENTO A GENERAR
[pegar aquí la descripción del elemento]
```

### 16.2 Descriptores por nivel

Añadir al bloque base según el nivel:

**Nivel 1 — La Oscuridad**

```
PALETA DEL NIVEL: roca #3E3550 con sombra #2A2438, suelo #5E4A52 con sombra #42333A,
estalagmitas #6B5A60. Ambiente nocturno frío.
PROHIBIDO usar naranja, amarillo o rojo en este elemento: esos colores están reservados
exclusivamente para el fuego, que es el elemento interactivo del nivel.
```

**Nivel 2 — La Rueda**

```
PALETA DEL NIVEL: follaje cercano #7FA05A, follaje medio #5A7A3F, follaje lejano #3C5429,
planta baja #6E9B4E, suelo de tierra #8A6B4A con sombra #6B5344, corteza de árbol #5C4530,
piedra fría #7A8290 con sombra #4E5561, cielo entre las copas #A8DCE6. Bosque de tarde despejada,
luz pareja, sin tinte y sin sombras largas: el amanecer, el atardecer y la noche los pone el motor encima.
PROHIBIDO usar madera clara trabajada (#C79A5E) en este elemento: ese tono está reservado
para los objetos interactivos del nivel —troncos cortados, rueda, eje, tabla, carretilla—.
La corteza del decorado usa el marrón oscuro desaturado indicado arriba.
```

**Nivel 3 — El Río**

```
PALETA DEL NIVEL: follaje #4E8C3F con sombra #37662B, follaje claro #6FA84E con sombra
#54803A, agua #3E8FA8 con sombra #2B6B80, roca húmeda #6B7A72 con sombra #4C5850.
Ambiente de mañana húmeda.
PROHIBIDO usar ámbar o dorado (#E8A33D, #F2C46B) en este elemento: ese color está
reservado para las piedras de paso y troncos utilizables.
```

### 16.3 Nota sobre modularidad

Generar los elementos de decorado **por piezas sueltas**, no como escenas completas.
Una escena generada de una sola vez no es reutilizable, no permite parallax y no se
puede recomponer.

Piezas mínimas por nivel: 3 variantes de plataforma, 2 de pared o fondo estructural,
4 de vegetación o formación rocosa, 2 de elemento decorativo pequeño.

Los generadores actuales no producen tilesets ensamblables sin costuras. Las
superficies repetibles (suelo, paredes largas) conviene construirlas a mano en
Illustrator a partir de una pieza generada, o directamente con formas vectoriales
simples.

---

## 17. Checklist de aprobación de assets

Aplicar a cada pieza antes de darla por buena e importarla a Unity.

### Todos los assets

- [ ] Fondo croma uniforme, sin escenario ni sombra proyectada sobre el fondo
- [ ] Contorno cerrado en todo el perímetro
- [ ] Sombreado de dos tonos con borde duro, sin degradados
- [ ] Sin texturas, ruido ni detalle de superficie
- [ ] Formas redondeadas, sin puntas afiladas
- [ ] Colores dentro de la paleta del nivel correspondiente
- [ ] Color de acento del nivel respetado (presente solo si es interactivo)
- [ ] Margen de al menos 10 % en los cuatro lados
- [ ] PNG sin compresión con pérdida
- [ ] Nombre de archivo según la nomenclatura de §15.4
- [ ] Sin violencia, armas, publicidad, marcas ni enlaces (RNF-22)
- [ ] Sin destellos rápidos ni parpadeos de alta frecuencia (RNF-21)
- [ ] Toda señal por color lleva un segundo canal — forma, icono o texto (RNF-19)
- [ ] Sin cifras, puntajes ni contadores visibles para el estudiante (CP-03, RF-17)
- [ ] Todo texto instruccional lleva refuerzo icónico y cabe en dos líneas (CP-08)
- [ ] Registrado en `CreditsContent.asset` con su mención de autoría (CT-09, RNF-23)

### Personajes

- [ ] A-pose con brazos separados del torso y axilas abiertas
- [ ] Manos color piel, cuatro dedos, sin guante ni puño
- [ ] Sin líneas anatómicas internas (pectorales, clavículas, busto, ombligo)
- [ ] Cabello plano, sin degradado entre mechones
- [ ] Escala relativa correcta respecto a la familia (§7.1)
- [ ] Silueta distinguible de los otros tres personajes en negro sólido

### Entornos

- [ ] Contorno más fino que el de los personajes
- [ ] Saturación reducida respecto a la capa de juego
- [ ] Pieza modular reutilizable, no escena completa
- [ ] Pasa la prueba de entrecerrado (§6): el decorado no gana a los personajes

### Interfaz

- [ ] Área táctil mínima de 88 × 88 px en botones de acción (excepciones en §10.1)
- [ ] Contraste de texto mínimo 4.5:1
- [ ] Información crítica con canal redundante además del color (§14.2)
- [ ] Texto oscuro sobre fondo claro, nunca al revés

---

## 18. Decisiones pendientes

Elementos que este documento no puede cerrar todavía y que bloquean parte de la
producción de assets.

| Pendiente | Impacto en arte | Estado |
| --- | --- | --- |
| **Título del videojuego** (`PG-01`) | Pantalla de título, logotipo, tipografía de marca | **Cerrado (09/09/2026): «Algoritmia».** La pantalla de inicio lo rotula en Baloo 2 ExtraBold, 96 px, `#3A1E18`, sin logotipo en imagen; el texto sale de `GameTitleConfig` |
| **Nombre definitivo del guía** (`PG-02`) | Nombre y **forma**: se llama **Algoritm** y cambia de forma en cada nivel (§7.6) | **Cerrado (02/09/2026).** Ver INC-44 e INC-45 |
| **Valores del Nivel 1** (`PG-06`) | Número de muescas del control deslizante en `A9` | **Abierto** hasta validarlo jugando |

**Ya no bloquean, y conviene no reabrirlos:**

- **Autorización de los personajes (`PG-07`): concedida por escrito** el 30/08/2026. Los
  personajes son obra derivada de los diseños de la Familia Anonaky —se rediseñaron, pero
  partieron de ellos—, y por eso el permiso hacía falta. Su reconocimiento expreso en la
  pantalla de créditos es **obligatorio** (CT-09, RNF-23). Ver §19.
- **Forma del guía:** ya **no** es una sola. El guion §1.1 fijaba una estrella constante; la
  decisión del 02/09/2026 la sustituye por **tres formas, una por nivel** —fuego, rueda, agua—
  con un núcleo de identidad invariable. Se especifica en §7.6 y se registra en INC-45. El
  arte entregado el 24/09/2026 las resolvió como **un solo cuerpo de llama en tres
  materiales**, y el guía ya no es una estrella en ningún nivel (INC-52). El nombre, antes
  provisional, queda cerrado: **Algoritm** (INC-44).

  El guion ya empujaba en esa dirección: en §4.4 el guía aparece «en el corazón de las llamas
  […] hecho de fuego esta vez» y se recoge en la fogata «como una brasa que sigue viva».
  La forma del guía siempre fue el material del descubrimiento que el nivel acaba de nombrar.
- **Personaje jugable por nivel:** cerrado en el guion §1.2 y en `CN-02` — Papá en el Nivel 1,
  la Niña en el Nivel 2, Mamá en el Nivel 3. El set de animaciones se produce en ese orden,
  que es el de los slices.

---

## 19. Nota legal

El diseño de los personajes de este proyecto **parte de los personajes de la Familia
Anonaky**, obra protegida por derechos de autor. Se rediseñaron —proporciones, vestuario,
paleta y rasgos propios—, pero haber cambiado el diseño **no** extingue el derecho del autor
original: el resultado sigue siendo **obra derivada**, y el uso de una herramienta de
generación por inteligencia artificial tampoco elimina esa condición.

Por eso se solicitó autorización en vez de darla por innecesaria. **La autorización escrita
está concedida** (30/08/2026; `PG-07` cerrado, `SPEC.md` supuesto 3). Consecuencias vigentes:

1. Los personajes `A1`..`A5` pueden producirse y aprobarse como definitivos.
2. Su **reconocimiento expreso en la pantalla de créditos es obligatorio**, no opcional
   (CT-09, RNF-23), y se registra en `CreditsContent.asset` como cualquier otro asset.
3. La constancia escrita se archiva con los anexos del trabajo de grado, junto al formato de
   consentimiento informado de RNF-12.
4. El plan alternativo de la sección 3.3.2 del trabajo de grado —rehacer los personajes desde
   descripciones originales— queda **sin activar**, y solo volvería a la mesa si la
   autorización se revocara.

Las especificaciones de entorno, interfaz, tipografía, color y animación de este
documento son originales del proyecto y no dependen de esa autorización.

Las tipografías recomendadas en §11 se distribuyen bajo licencia SIL Open Font License
1.1, que permite su uso, modificación y distribución en proyectos académicos y
comerciales sin restricciones de atribución en el producto.

Los tres glifos del menú de pausa (`ui_pausa`, `ui_reanudar` y `ui_reiniciar`) no son
originales del proyecto: son iconos de la familia Phosphor Icons, con licencia MIT, y la
pantalla de créditos los acredita con su licencia (INC-127). Es el único crédito de Phosphor: solo los
iconos de pausa, no el resto de la interfaz. Los créditos dicen «Entornos, objetos e interfaz: originales
del proyecto, salvo los iconos de pausa.» e «Iconos de pausa: Phosphor Icons, licencia MIT.», y la
licencia está en `Assets/Game/Art/UI/Common/LICENSE-Phosphor.txt` (el build la copia a `Licencias/`).
