# Dirección de arte — Videojuego educativo 2D

Última actualización: 2026-08-31
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
| Marfil | `#F7EFE2` | Fondo de globos de diálogo y paneles |
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

No hay escalado por altura de salto: **en este juego no se salta**. El único personaje
que se desplaza es Mamá en el Nivel 3, en vista superior y accionada con botones en
pantalla (RF-35, CT-06, RNF-02), donde la elipse mantiene escala constante.

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

### 7.3 Set de expresiones

Cada personaje necesita seis expresiones faciales para el prototipo. Se generan como
variaciones del sprite base manteniendo idénticos el cráneo, el peinado y el color.

| Expresión | Cejas | Ojos | Boca | Uso en juego |
| --- | --- | --- | --- | --- |
| Neutra | Rectas, separadas | Abiertos, redondos | Sonrisa cerrada suave | Estado de reposo |
| Alegre | Arqueadas hacia arriba | Entrecerrados en arco | Sonrisa abierta amplia | Reto resuelto |
| Sorpresa | Muy elevadas | Muy abiertos, pupila pequeña | Óvalo abierto | Descubrimiento, evento narrativo |
| Concentración | Ligeramente juntas | Entrecerrados horizontales | Línea recta corta | Durante un reto |
| Duda | Una elevada, otra baja | Uno más cerrado | Línea ondulada corta | Pista disponible |
| Ánimo | Arqueadas | Abiertos con brillo grande | Sonrisa abierta pequeña | Tras un intento fallido |

**Regla sobre la tristeza y el enfado:** no existen en el set. Un intento fallido nunca
produce una expresión negativa en los personajes; produce la expresión de ánimo. Esta
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
derivan todas las animaciones y porque permite recortar las extremidades para el
rigging sin tener que inventar dónde terminan.

Las poses expresivas para el documento de trabajo de grado y las capturas de
sustentación se generan aparte, usando el sprite base aprobado como referencia.

---

### 7.6 Algoritm — una forma por nivel

El guía se llama **Algoritm** (`PG-02` cerrado el 02/09/2026, INC-44) y **cambia de forma
en cada nivel**: fuego, rueda y agua, en ese orden (INC-45). Es el mismo personaje en los
tres —lo exige CN-03—, y lo que garantiza que se reconozca no es el contorno de su cuerpo
sino el núcleo de identidad que sigue abajo.

El guion ya lo empujaba: en §4.4 el guía aparece «en el corazón de las llamas […] hecho de
fuego esta vez». Su cuerpo es el material del descubrimiento que el nivel acaba de nombrar.

#### Núcleo de identidad — invariable en los tres niveles

Si uno solo de estos rasgos cambia, deja de leerse como el mismo personaje:

| Rasgo | Especificación |
| --- | --- |
| Tamaño | El de una palma de mano adulta; anchura total de poco más de una cabeza humana |
| Ojos | Dos óvalos negros grandes, muy separados, en el tercio superior, cada uno con un punto de luz blanco en su esquina superior izquierda |
| Boca | Una sola línea curva hacia arriba, sonrisa cerrada. Sin nariz, sin cejas |
| Extremidades | **Ninguna.** Sin brazos, sin piernas, sin manos, sin accesorios |
| Núcleo interior | Área clara que repite la forma del cuerpo en pequeño, a borde duro, sin degradado |
| Contorno | 8 px en `#E2571F`. **Cálido en los tres niveles**, aunque el cuerpo sea de madera o de agua: es la firma de que emite luz propia y su rastro de origen |
| Estela | Cinco a siete puntos sueltos `#FFE9A8`, circulares, de tamaño decreciente, en curva. Nunca una nube difuminada |
| Cuenta de cinco | La silueta siempre se cuenta hasta cinco: cinco puntas, cinco radios, cinco lóbulos |

#### Las tres formas

| Nivel | Forma | Cuerpo | Núcleo | Detalle |
| --- | --- | --- | --- | --- |
| 1 · La Oscuridad | **Estrella de cinco puntas**, todas de extremo redondeado, la superior en vertical | `#F5A62E` | `#FFE9A8` | La forma de origen. Es la que aparece en la fogata y se queda como brasa viva |
| 2 · La Rueda | **Rueda**: disco de canto redondeado con cinco radios romos y un buje central | `#C79A5E` con radios y buje `#A67C4A` | `#FFE9A8` en el buje | El disco gira sobre su eje al flotar, en vez de inclinarse |
| 3 · El Río | **Gota**: cuerpo redondeado de cinco lóbulos suaves, como una gota vista de frente | `#5AA8BF` | `#D6F0F5` | Su estela son gotas pequeñas, no puntos de luz |

**Cuándo muta.** En las dos transiciones entre niveles, que ya son suyas: el barrido de
Algoritm de `TR-05` y `TR-09`. Entra con la forma del nivel que termina y sale con la del
que empieza. En ningún otro momento cambia de cuerpo, y **nunca a la vista dentro de una
escena jugable**.

**El riesgo del nivel 2, y cómo se contiene.** El cuerpo de la rueda usa `#C79A5E`, que es
el acento del nivel y por tanto la señal de «esto es interactivo». La regla de §4.2 prohíbe
ese tono en el **decorado**, y el guía no es decorado, así que no la infringe — pero sí
puede confundir. Tres condiciones lo separan de un prop, y son obligatorias:

1. **Nunca se posa.** Flota siempre por encima de la línea de los objetos del reto, y no
   entra en la zona de ensamblaje.
2. **Tiene cara.** Ningún prop del juego tiene ojos ni boca.
3. **Pulsa.** El pulso de escala de la pista (§10.2) es suyo y de nada más.

#### Nomenclatura

```
char_algoritm_n1_estrella.png
char_algoritm_n2_rueda.png
char_algoritm_n3_gota.png
```

Sustituyen a `char_chispa_*`. La palabra `chispa` queda libre para lo que siempre fue en
este juego: el destello del Nivel 1 (`fx_n1_chispa_*`), que no tiene nada que ver con el
guía y **no se renombra**.

---

## 8. Entornos por nivel

Los tres niveles comparten sistema de línea, sombreado y proporción, y se diferencian
por acorde cromático, ambiente lumínico y vocabulario de formas. La progresión está
diseñada para leerse como un amanecer largo: de la noche del nivel 1 al mediodía del
nivel 2 y a la mañana húmeda del nivel 3.

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
**Momento del día:** mediodía despejado
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
violentos, no hay espuma turbulenta, no hay oscuridad bajo la superficie. Si el
personaje cae, la retroalimentación es una salpicadura y un reinicio inmediato de la
secuencia, sin animación de peligro. Se detalla en §12.3.

#### Niebla

Franja horizontal de color plano `#D6F0F5` al 30 % de opacidad sobre el plano medio,
con borde superior ondulado. No es un degradado ni un shader: es un sprite. Oscila
verticalmente 8 px en un ciclo de 6 s.

#### Elementos de decorado

Helechos como abanicos de tres o cuatro hojas planas, juncos en `#8CA84E` como líneas
gruesas con punta redondeada, flores pequeñas de cinco pétalos en `#B87FC4` y `#7FA8E0`
(nunca en ámbar, por la regla de acento), libélulas como dos óvalos y cuatro elipses
transparentes.

---

## 9. Props y objetos interactivos

### 9.1 Regla de separación

Ningún objeto se dibuja en la mano de un personaje. Todos los props son assets
independientes con su propio contorno cerrado. Razones:

1. Permite las mecánicas de recoger, soltar e intercambiar, necesarias para los retos
   de secuenciación.
2. Evita redibujar el prop en cada frame de animación.
3. El mismo asset sirve como icono de interfaz sin trabajo adicional.

En Unity se montan como hijo del hueso de la mano.

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
hay desplazamiento libre en los niveles 1 y 2 y el del 3 es por casillas con botones.

| Nivel | Prop | Función | Color dominante |
| --- | --- | --- | --- |
| 1 | Montón de hojas secas (4 estados) | Objetivo del reto: intacto, chispas apagadas, humeante, encendido | `#B08541` |
| 1 | Sílex | Pieza del panel, silueta angulosa | `#9BA0A8` |
| 1 | Pedernal | Pieza del panel, silueta redondeada | `#8B5A3C` |
| 1 | Hoguera | Resolución del nivel, única fuente de luz cálida | `#F5A62E` / `#E2571F` / `#FFE9A8` |
| 2 | Objetos del bosque (válidos y distractores) | Selección por patrón, fase 1 (RF-22..RF-26) | Verde vivo solo en los válidos |
| 2 | Caja de alimentos (3 estados) | Meta narrativa de la fase 1 | `#C4743E` |
| 2 | Seis piezas del taller | Ensamblaje secuencial, fase 2 (RF-27..RF-29) | `#8B5A3C` + acento verde |
| 2 | Rueda y carretilla (5 estados) | Resultado del ensamblaje | `#A89880` + `#5FA842` |
| 2 | Carretilla cenital (4 orientaciones) | Ejecución de la secuencia, fase 3 (RF-30..RF-33) | `#5FA842` |
| 2 | Bloques de instrucción y botón «Ejecutar» | Editor de secuencia, fase 3 (RF-31, RF-32) | `#5FA842` sobre marfil |
| 3 | Troncos y sogas | Materiales recolectables por casilla (RF-36..RF-39) | `#E8A33D` |
| 3 | Mástil y vela | Materiales de la tercera fase de ensamblaje (RF-40) | `#E8A33D` / `#F2C46B` |
| 3 | Balsa (3 estados de avance) | Construcción por fases: base, amarre, mástil y vela | `#8B5A3C` + `#E8A33D` |
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
(RF-07) y, **solo en el Nivel 3**, la lista de cuatro tareas (RF-36). Los niveles 1 y 2
**no llevan lista ni indicador permanente de progreso**: RNF-03 restringe la tarea
**activa** a una, y añadir un marcador que no pide ningún RF sería una mecánica nueva
(INC-41). Todo lo demás aparece por contexto y desaparece.

**Sin cifras a la vista del estudiante.** Ni intentos, ni pasos, ni tiempo, ni puntaje,
en ninguna pantalla del juego ni en el resumen de fin de nivel (CP-03, RF-17, RF-45).
Los números existen solo en el informe docente (RF-46).

**Área táctil generosa.** Ningún elemento interactivo mide menos de 88×88 px a
resolución de diseño. La motricidad fina de un niño de nueve años no es la de un
adulto.

### 10.2 Componentes

| Componente | Material aparente | Color base | Notas |
| --- | --- | --- | --- |
| Panel de diálogo | Tablilla de piedra clara | `#F7EFE2` con borde `#C4A882` | Esquinas muy redondeadas (32 px) |
| Botón primario | Piedra redondeada | `#E8A33D`, borde `#3A1E18` | Sombra plana inferior de 6 px |
| Botón secundario | Piedra clara | `#E0D4C0`, borde `#6B5248` | |
| Lista de tareas (**solo Nivel 3**) | Cuerda con nudos | Cuerda `#C4A882`, nudo cerrado `#5FA842` | Un nudo por tarea de RF-36. Tarea cumplida = nudo cerrado **más** marca de forma, nunca solo color (RNF-19). Sin cifras |
| Icono de pista | Algoritm en pequeño | `#E8A33D` | Pulso lento de escala cuando hay pista disponible. Es el guía quien ofrece la pista (CP-06), así que el icono es él |
| Marco de inventario | Cuerda trenzada | `#C4A882` | Casillas circulares |

### 10.3 Globos de diálogo

Forma ovalada de esquinas muy redondeadas, relleno `#F7EFE2`, contorno `#3A1E18` de
6 px, con cola triangular redondeada apuntando al hablante. Sin sombra proyectada,
sin degradado.

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
| Números e indicadores | **Fredoka** | SIL OFL 1.1 | Cifras muy diferenciadas, ideal para contadores y pasos |

Las tres soportan caracteres del español (tildes, `ñ`, signos de apertura `¿` `¡`),
requisito no negociable.

### 11.3 Escala tipográfica

Definida a resolución de diseño 1920×1080.

| Nivel | Tamaño | Familia | Peso | Interlineado |
| --- | --- | --- | --- | --- |
| Título de nivel | 72 px | Baloo 2 | 700 | 1.1 |
| Subtítulo | 48 px | Baloo 2 | 600 | 1.2 |
| Diálogo | 34 px | Nunito | 600 | 1.5 |
| Instrucción de reto | 30 px | Nunito | 700 | 1.4 |
| Texto secundario | 26 px | Nunito | 400 | 1.5 |
| Contadores | 40 px | Fredoka | 600 | 1.0 |

**Mínimo absoluto: 26 px.** Ningún texto del juego baja de ese tamaño.

### 11.4 Reglas de composición

- Máximo 2 líneas por globo de diálogo, 12 palabras por línea.
- Alineación a la izquierda, nunca justificada.
- Sin mayúsculas sostenidas en textos de más de tres palabras: entorpecen la lectura
  en formación.
- Contorno de texto: `#3A1E18` de 3 px cuando el texto va sobre el escenario y no
  sobre panel.

---

## 12. Efectos visuales y retroalimentación

### 12.1 Principios

Todos los efectos se resuelven con sprites de color plano y animación por fotogramas.
No se usan sistemas de partículas complejos, shaders personalizados ni posprocesado:
la restricción de bajo consumo de recursos declarada en el alcance del proyecto lo
impide, y el estilo plano no los necesita.

### 12.2 Catálogo de efectos

| Efecto | Construcción | Duración |
| --- | --- | --- |
| Chispas que se apagan | 4 líneas cortas radiales `#FFE9A8` que se acortan hasta desaparecer, sin llama (RF-16) | 0.35 s |
| Chispa | 4 líneas cortas radiales `#FFE9A8` | 0.2 s |
| Salpicadura de agua | 5 óvalos `#D6F0F5` en arco ascendente | 0.4 s |
| Recolección de objeto | Círculo `#F7EFE2` que se expande y desaparece | 0.35 s |
| Reto resuelto | 6 destellos de 4 puntas `#5FA842` en corona | 0.8 s |
| Aparición de pista | Antorcha de UI que pulsa de escala 1.0 a 1.12 | Ciclo 1.5 s |

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
| Secuencia incorrecta | Las piezas vuelven a su posición inicial con una animación suave de 0.5 s; el personaje muestra la expresión de ánimo |
| Prueba de balsa sin éxito (Nivel 3) | Salpicadura plana y la balsa vuelve al punto de partida tras 0.6 s. Lo ya confirmado **no se pierde** (RF-41, RF-43) |
| Intento repetido sin éxito (3 veces) | El icono de pista comienza a pulsar en `#E8A33D` |

El único color de estado que existe es el verde de éxito (`#5FA842`) y el ámbar de
atención (`#E8A33D`). No hay rojo de error en toda la interfaz.

---

## 13. Animación

### 13.1 Enfoque técnico

Rigging 2D en Unity (paquete 2D Animation) sobre los sprites base en A-pose, no
animación fotograma a fotograma. Razones: el equipo es de dos personas con catorce
semanas, y los generadores de imagen no producen secuencias de frames consistentes
entre sí.

Consecuencia sobre el arte: los sprites base deben tener brazos y piernas
completamente separados del torso, con fondo visible entre ellos. Un brazo fundido
con el cuerpo obliga a inventar dónde termina al recortarlo para el hueso.

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
| Caminar en vista superior, 4 direcciones | Mamá (N3) | Ciclo 0.8 s | Crítica |
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
| Reto resuelto | Verde `#5FA842` | Icono de nudo cerrado en la cuerda de progreso |
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

| Ajuste | Valor |
| --- | --- |
| Texture Type | Sprite (2D and UI) |
| Sprite Mode | Single (personajes) / Multiple (hojas de props) |
| Pivot | Bottom (personajes) / Center (props) |
| Filter Mode | Bilinear |
| Compression | Normal Quality |
| Max Size | 2048 |
| Generate Physics Shape | Desactivado (los colisionadores se definen a mano) |

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

char_papa_base_apose.png
char_nina_base_apose.png
char_nino_expr_sorpresa.png
char_mama_cenital_norte.png
prop_n1_hojas_encendido.png
prop_n2_carretilla_e5.png
env_n1_cueva_luz4.png
env_n2_bosque_claro.png
env_n3_rio.png
ui_n1_panel_soplar_habilitado.png
fx_chispa_apagada.png
```

Prefijos: `char_`, `prop_`, `env_`, `ui_`, `fx_`.
Niveles: `n1` (La Oscuridad), `n2` (La Rueda), `n3` (El Río).

Sin tildes, sin espacios, sin mayúsculas en los nombres de archivo.

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
piedra fría #7A8290 con sombra #4E5561, cielo entre las copas #A8DCE6. Bosque de mediodía,
luz pareja y sin sombras largas.
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

- [ ] Área táctil mínima de 88 × 88 px
- [ ] Contraste de texto mínimo 4.5:1
- [ ] Información crítica con canal redundante además del color (§14.2)
- [ ] Texto oscuro sobre fondo claro, nunca al revés

---

## 18. Decisiones pendientes

Elementos que este documento no puede cerrar todavía y que bloquean parte de la
producción de assets.

| Pendiente | Impacto en arte | Estado |
| --- | --- | --- |
| **Título del videojuego** (`PG-01`) | Pantalla de título, logotipo, tipografía de marca | **Abierto.** Se cierra en el Slice 4 |
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
  nombre, antes provisional, queda cerrado: **Algoritm** (INC-44).

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
