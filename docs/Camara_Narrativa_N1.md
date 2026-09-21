# Movimientos de cámara del Nivel 1 — diseño completo sobre el entorno duplicado

Mismo formato y las mismas reglas que `Camara_Narrativa_N2.md`. Cubre las cuatro narrativas del
Nivel 1 más la fase jugable. **35 encuadres**, todos validados.

Misma convención: foco `(x, y)` normalizado, **`y` desde abajo**, `zoom` 1.0 = la ilustración cubre el alto.

---

## 1. La diferencia de fondo con el Nivel 2

En el bosque la cámara era el único instrumento. Aquí no: **casi todo el Nivel 1 ocurre a oscuras**, y a
oscuras un encuadre no describe nada. Lo que el jugador ve no lo decide el zoom, lo decide hasta dónde
llega la luz. Así que cada parada de este documento lleva dos cosas: el encuadre **y** el estado de la
capa de oscuridad. Diseñar una sin la otra no significa nada — un paneo precioso sobre negro es negro.

Eso cambia además el reparto de trabajo: en el Nivel 1 la cámara se mueve **poco** y la luz se mueve
**mucho**. Un movimiento de cámara que el jugador no puede ver no es un movimiento, es una deriva que se
siente rara. Por eso hay escenas enteras con la cámara bloqueada.

---

## 2. La vista cenital no se toca

Como pediste, la cenital se queda en 1599×899 sin duplicar. Y no hay nada que diseñarle: **es exactamente
16:9**, así que a zoom 1 llena la pantalla justa y el foco queda clavado en (0.500, 0.500) — a ese zoom el
rango de foco es un punto. Cualquier movimiento exigiría cerrar el plano y descubrir borde.

La doy por asignada a la fase jugable (el panel de golpes sobre el montón de hojas), que es donde una
vista desde arriba tiene sentido. **Si en realidad la ibas a usar para otra cosa, dímelo**, porque
entonces la fase jugable necesita un encuadre de la lateral y eso sí lo tengo que recalcular.

---

## 3. El entorno duplicado

El sprite lateral mide 1599×899, igual que el del bosque, así que **el canvas duplicado vuelve a ser
3198×899 y toda la matemática del Nivel 2 sirve tal cual**: mismos límites, mismas reglas, mismo
`0.4998` de canvas visible a zoom 1.

Aquí el espejo puro va **sin ninguna pasada de luz horneada**, al contrario que en el bosque. El motivo:
en el Nivel 1 la fuente de luz real es la hoguera, que está al oeste y aparece al final. Un degradado
horneado pelearía con la capa de oscuridad dinámica y se notaría en cuanto la luz se mueva. El asset es
literalmente el sprite dos veces.

Por eso mismo, aquí la ruta sin asset nuevo es la más razonable: **dos `SpriteRenderer` con el mismo
sprite, el derecho con `flipX = true`**, desplazado un ancho. Te dejo igual el PNG por comodidad.

---

## 4. La capa de oscuridad

Un solo componente, cinco valores, todos animables por parada:

| Valor | Qué es |
|---|---|
| `centro (lx, ly)` | Dónde está la fuente de luz, **en coordenadas del canvas**, no de pantalla. Así la luz queda pegada al mundo: si la cámara panea, el charco se queda donde está el objeto que ilumina. |
| `radio` | Tamaño del charco, en unidades de **alto del canvas**. 0 = no hay fuente puntual. |
| `fondo` | Brillo fuera del charco. 0 = negro puro, 1 = plena luz. Es el valor que hace casi todo el trabajo dramático del nivel. |
| `tinte` | Multiplicador RGB. Tres en todo el nivel: frío `(0.55, 0.68, 1.00)` para la noche, ámbar `(1.00, 0.90, 0.72)` para Chispa, fuego `(1.00, 0.80, 0.55)` para la hoguera. |
| `borde` | Suavidad de la caída. Un solo valor global sirve. |

Se implementa como un hijo de la ilustración con un degradado radial y multiplicación — no hace falta
iluminación real ni materiales nuevos.

**El truco que ahorra un fondo entero:** la apertura ocurre *fuera* de la cueva («noche helada, sin luna»)
y no hay arte de exterior. Pero una noche sin luna es, literalmente, casi negro. La misma cueva con
`fondo 0.06` y tinte frío se lee como roca y maleza a la intemperie; al entrar, el tinte pasa a ámbar y
el fondo sube. Mismo sprite, dos sitios, cero arte nuevo. Es el equivalente del truco de las tres zonas
del bosque.

---

## 5. Reglas

Las dos primeras son las del Nivel 2 y siguen aplicando; la tercera sustituye a la de la línea de árboles.

**R1 — Ningún encuadre cruza `x = 0.500`.** Los 35 cumplen.

**R2 — Foco dentro de `x ∈ [0.25/z, 1−0.25/z]`, `y ∈ [0.5/z, 1−0.5/z]`.** Con margen ≥ 0.008.

**R3 — La línea del suelo (`y ≈ 0.32`) tiene que estar en cuadro.** Es donde el muro se encuentra con el
piso y es la única referencia de orientación que tiene la cueva. En el bosque el problema era el pasto
vacío; aquí es peor, porque a oscuras un encuadre sin línea de suelo es una mancha sin arriba ni abajo.

**R4 — La cámara no se mueve mientras no haya luz suficiente para verlo.** No es comprobable por cálculo,
pero es la que más decisiones tomó: en la escena 1.2 hay once paradas y la cámara solo se mueve en cuatro.

---

## 6. Mapa de zonas

| Zona | x | Qué es | Dónde se usa |
|---|---|---|---|
| **La entrada** | 0.66 – 0.92 | La boca de la cueva. Única zona que ve luz exterior. | apertura |
| **El fondo de la cueva** | 0.12 – 0.36 | Donde Chispa los lleva, donde están las piedras y donde se hace el fuego. | 1.1 · 1.2 · juego · 1.3 |
| **Eje del espejo** | 0.500 | Zona prohibida. | — |

La familia entra por el este y avanza al oeste, que es coherente con el Nivel 2: en el puente I salen de
la cueva al claro este del bosque.

**Y la travesía del este al oeste ocurre con la pantalla en negro.** Es el regalo de este nivel: la última
parada de la apertura es el fundido a negro, así que la cámara puede saltar de `x 0.700` a `x 0.260` sin
que nadie lo vea. Un teletransporte gratis que en el bosque habría costado un paneo de ocho segundos.

---

## 7. Escena por escena

Cada línea es una parada (`CameraKeys`) con su estado de luz. Las hojas de verificación muestran el
**estado final** de cada línea; donde la luz cambia *dentro* de una línea está dicho en la nota.

### 7.1 · `N1_Apertura` — La noche y la cueva

*Zona: entrada (este) · 4 paradas*

Cuatro tiempos. Abre a la intemperie (mismo sprite, frío y aplastado a negro), empuja hacia la boca de
la cueva cuando corren, y luego hace el movimiento clave: **la cámara deriva al oeste mientras el filo de
luz de la entrada se encoge y muere**. Es el «como si alguien apagara el mundo» del guion, hecho con la
luz y no con un fundido de pantalla. La última parada es el negro de dos segundos, y ahí la cámara salta
al fondo de la cueva sin que se vea.

| # | Línea | Foco (x, y) | Zoom | Luz (centro · radio · fondo · tinte) | Movimiento |
|---|---|---|---|---|---|
| `L0` | Noche helada, sin luna. La familia camina a la intemperie... | **(0.780, 0.520)** | **1.22** | sin fuente · **0.06** · frío | Plano abierto, azul y aplastado a negro: las formaciones se leen como roca y maleza en la noche |
| `L1` | Algo se acerca entre la maleza. La familia corre y entra. | **(0.818, 0.480)** | **1.45** | (0.950, 0.42) · r **0.30** · **0.10** · frío | Empuje rapido al este, hacia la boca. Entra un filo de luz frio por el borde |
| `L2` | Avanzan hacia el fondo... la luz de la entrada desaparece. | **(0.700, 0.440)** | **1.60** | (0.920, 0.42) · r **0.14** · **0.04** · frío | La camara deriva al OESTE mientras el filo de luz se encoge y muere. Fundido a negro DENTRO de la linea |
| `L3` | La pantalla queda completamente negra durante dos segundos. | **(0.260, 0.430)** | **1.55** | sin fuente · **0.00** · frío | SALTO EN NEGRO al fondo de la cueva. Marcar esta parada como corte seco, sin suavizado |

### 7.2 · `N1_AparicionGuia` — Escena 1.1, la aparición del guía

*Zona: fondo (oeste) · 6 paradas*

Aquí la luz **es** el personaje. Chispa nace como un punto, crece, se pasea y se apaga; la cámara solo lo
acompaña. El momento que manda es L4: Chispa nombra el sílex y el pedernal, así que el charco tiene que
ensancharse hasta cubrir las dos piedras justo en esa línea — es tu regla de que lo que se menciona se
vea, pero resuelta con el radio en vez de con el encuadre. En L5 la cámara se bloquea a propósito: el
apagón es el movimiento, y una cámara derivando durante un apagón se siente como un error.

| # | Línea | Foco (x, y) | Zoom | Luz (centro · radio · fondo · tinte) | Movimiento |
|---|---|---|---|---|---|
| `L0` | un pequeno destello amarillo parpadea... una figura con forma de estrella | **(0.260, 0.430)** | **1.55** | (0.255, 0.46) · r **0.18** · **0.02** · ámbar | Hereda el encuadre a ciegas de la apertura. El charco de luz NACE: radio 0 -> 0.18 |
| `L1` | CHISPA: Hola, familia! No tengan miedo... | **(0.272, 0.440)** | **1.42** | (0.258, 0.46) · r **0.22** · **0.03** · ámbar | Abre un poco: las siluetas de la familia entran al borde del charco |
| `L2` | La nina abre mucho los ojos. El nino suelta un Oh!... | **(0.300, 0.430)** | **1.58** | (0.285, 0.45) · r **0.24** · **0.03** · ámbar | Paneo corto al este y cierra: cuatro caras dentro del charco |
| `L3` | Chispa gira como un trompo y avanza hacia el fondo... | **(0.215, 0.445)** | **1.72** | (0.200, 0.47) · r **0.20** · **0.03** · ámbar | Chispa se lleva la luz al oeste y la camara va detras: el muro del fondo se acerca |
| `L4` | CHISPA: Ven esa piedra gris? Eso es silex. Y esa de alla... | **(0.208, 0.390)** | **1.78** | (0.205, 0.26) · r **0.30** · **0.03** · ámbar | Baja al suelo y ENSANCHA el charco: las dos piedras tienen que estar iluminadas cuando se las nombra |
| `L5` | Chispa parpadea una vez, dos veces, y se apaga. | **(0.208, 0.390)** | **1.78** | (0.205, 0.26) · r **0.06** · **0.01** · ámbar | La camara NO se mueve. El apagon es el movimiento: tres parpadeos y radio -> 0 |

### 7.3 · `N1_Hallazgo` — Escena 1.2, el hallazgo

*Zona: fondo (oeste) · 14 paradas*

La escena más larga del nivel y la que menos se mueve. Once de sus catorce paradas están en penumbra
(`fondo 0.07–0.08`: se leen siluetas y poco más) y la cámara solo se mueve cuatro veces, siempre por un
motivo físico: el niño se agacha, papá se levanta, mamá lo toma del hombro, papá vuelve al suelo.

**El centro de la escena es L5.** El ¡CLIC! es un destello de tres frames (~80 ms) a plena luz: la única
vez en todo el nivel que se ve la cueva entera antes del fuego. Para que pegue, L4 **baja** a `fondo 0.03`
justo antes — el salto es de 32×. Ese contraste es lo que lo vende, no el negro sostenido; por eso el
resto de la escena está en penumbra legible y no en negro absoluto. Después del destello el jugador ya
conoce el espacio, así que las paradas oscuras siguientes se leen sobre esa imagen recordada.

| # | Línea | Foco (x, y) | Zoom | Luz (centro · radio · fondo · tinte) | Movimiento |
|---|---|---|---|---|---|
| `L0` | NINA: Se fue? | **(0.208, 0.390)** | **1.78** | sin fuente · **0.08** · ámbar | Penumbra: se leen las siluetas, nada mas. Camara bloqueada |
| `L1` | PAPA: Si. Pero nos dejo algo. | **(0.208, 0.390)** | **1.78** | sin fuente · **0.08** · ámbar | Sin movimiento |
| `L2` | El nino se arrodilla y busca con las manos las dos piedras. | **(0.212, 0.400)** | **1.55** | sin fuente · **0.07** · ámbar | ABRE un poco: hay que tener cuadro de sobra para el destello |
| `L3` | NINO: Por que seran especiales unas piedras? | **(0.212, 0.400)** | **1.55** | sin fuente · **0.07** · ámbar | Sin movimiento |
| `L4` | La nina las golpea una contra otra... Nada. Mas fuerte. | **(0.212, 0.400)** | **1.55** | sin fuente · **0.03** · ámbar | CAE a negro casi total justo antes del destello: el contraste es lo que lo vende |
| `L5` | CLIC! Un destello pequenisimo... | **(0.212, 0.400)** | **1.55** | (0.205, 0.20) · r **0.60** · **0.95** · chispa | EL DESTELLO: 3 frames (~80 ms) a plena luz y vuelta al negro. La camara no se mueve. Es la unica vez que se ve la cueva entera |
| `L6` | NINA: Lo viste? | **(0.212, 0.400)** | **1.55** | sin fuente · **0.04** · ámbar | Vuelve el negro, con post-imagen |
| `L7` | NINO: Si! Otra vez, otra vez! | **(0.212, 0.400)** | **1.55** | (0.205, 0.20) · r **0.22** · **0.06** · chispa | Dos destellos menores, mas debiles, puntuando la frase |
| `L8` | Papa se pone de pie despacio. | **(0.218, 0.450)** | **1.48** | sin fuente · **0.08** · ámbar | Sube con el: el unico movimiento motivado de la escena |
| `L9` | PAPA: Esperen. Estas piedras pueden ser peligrosas... | **(0.218, 0.450)** | **1.48** | sin fuente · **0.08** · ámbar | Sin movimiento |
| `L10` | El nino quiere protestar, pero mama lo toma del hombro. | **(0.258, 0.445)** | **1.52** | sin fuente · **0.08** · ámbar | Paneo corto al este: mama y el nino |
| `L11` | MAMA: Papa sabe lo que hace. | **(0.258, 0.445)** | **1.52** | sin fuente · **0.08** · ámbar | Sin movimiento |
| `L12` | Papa se arrodilla, busca a tientas hojas secas y ramitas. | **(0.222, 0.390)** | **1.62** | sin fuente · **0.06** · ámbar | Vuelve al oeste y baja al suelo. Sigue a tientas: nada de luz de cortesia |
| `L13` | CHISPA: Tienes las piedras y tienes las hojas... Desde donde vas a intentarlo? | **(0.215, 0.370)** | **1.70** | (0.213, 0.21) · r **0.16** · **0.05** · ámbar | Vuelve un rescoldo debil de Chispa sobre el monton: deja ver el montaje antes de jugar |

### 7.4 · `N1_NacimientoFuego` — Escena 1.3, el nacimiento del fuego

*Zona: fondo (oeste) · 11 paradas*

El pago. **La luz y la cámara abren a la vez, porque son el mismo gesto**: empieza cerradísima sobre la
llama con el charco a 0.10 y termina en el plano más abierto del nivel con la cueva entera visible. Todo
el nivel ha sido oscuro y estrecho para que este plano signifique algo.

L9 es el fotograma de pago: (0.270, 0.500) ×1.18, `fondo 0.85`. Y L10 no vuelve al negro: cierra a rescoldo.
El nivel abre en negro y termina en brasa, que es exactamente lo que pide el ajuste de continuidad del
guion (Chispa se queda vivo dentro de la fogata para reaparecer en el Nivel 2).

| # | Línea | Foco (x, y) | Zoom | Luz (centro · radio · fondo · tinte) | Movimiento |
|---|---|---|---|---|---|
| `L0` | Una llama crece despacio desde las hojas... la cueva se llena de luz. | **(0.215, 0.375)** | **1.85** | (0.213, 0.21) · r **0.45** · **0.35** · fuego | Arranca cerradisimo sobre la llama. El radio crece 0.10 -> 0.45 DENTRO de la linea |
| `L1` | Papa se queda quieto, mirando las llamas, con las manos temblorosas. | **(0.222, 0.400)** | **1.60** | (0.215, 0.22) · r **0.50** · **0.45** · fuego | Abre con la luz: son el mismo gesto |
| `L2` | NINOS: PAPAAAA! LO LOGRASTE! | **(0.268, 0.420)** | **1.45** | (0.222, 0.23) · r **0.60** · **0.55** · fuego | Paneo al este a por los ninos y sigue abriendo |
| `L3` | Los ninos se lanzan encima de el. Mama llega detras... | **(0.262, 0.440)** | **1.34** | (0.228, 0.24) · r **0.70** · **0.65** · fuego | Mas abierto: cabe la familia entera |
| `L4` | PAPA: Las piedras tenian razon. Eran especiales. | **(0.238, 0.420)** | **1.55** | (0.220, 0.23) · r **0.62** · **0.70** · fuego | Cierra sobre papa: el unico momento intimo con luz |
| `L5` | En el corazon de las llamas aparece Chispa, hecho de fuego. | **(0.222, 0.395)** | **1.80** | (0.215, 0.22) · r **0.34** · **0.60** · fuego | Empuje a la hoguera. El charco se aprieta y se vuelve mas intenso |
| `L6` | CHISPA: Yo no traje la luz. Solo les mostre donde buscar. | **(0.235, 0.430)** | **1.50** | (0.220, 0.23) · r **0.65** · **0.72** · fuego | Abre otra vez |
| `L7` | NINA: Papa, tu eres como Chispa! | **(0.272, 0.425)** | **1.58** | (0.225, 0.23) · r **0.66** · **0.72** · fuego | Paneo corto a la nina |
| `L8` | Papa se rie bajito y la abraza mas fuerte. | **(0.268, 0.418)** | **1.66** | (0.225, 0.23) · r **0.64** · **0.74** · fuego | Empuje minimo sobre el abrazo |
| `L9` | CHISPA: Eso tiene nombre: se llama iterar... | **(0.270, 0.500)** | **1.18** | (0.230, 0.24) · r **0.95** · **0.85** · fuego | EL PLANO DE PAGO: el mas abierto del nivel, la cueva por fin entera y visible. Todo el nivel ha sido a oscuras para llegar aqui |
| `L10` | Chispa se recoge dentro de la fogata, como una brasa viva. | **(0.252, 0.455)** | **1.38** | (0.232, 0.23) · r **0.40** · **0.32** · fuego | Cierra un poco y baja a rescoldo. El nivel abre en negro y cierra en brasa, no en apagon |

---

## 8. La fase jugable

Sobre la cenital, sin duplicar: **foco (0.500, 0.500), zoom 1.00, fija**. No hay margen para otra cosa.

Lo que sí propongo es atar la luz al progreso: `fondo` sube de **0.10 a 0.30** repartido entre los cinco
golpes efectivos. El jugador ve que va ganando *en la luz de la pantalla*, no solo en un contador — y
encaja con que el nivel entero trate de encender algo. Es un `Mathf.Lerp` contra el contador que ya existe.

El charco se queda fijo sobre el montón de hojas, en el centro del sprite, con radio ~0.30 y tinte ámbar.

---

## 9. Dónde va cada cosa

| Escena | Elemento | x | y |
|---|---|---|---|
| 1.1 | Chispa al aparecer | 0.255 | 0.46 |
| 1.1 | Chispa tras avanzar al fondo | 0.200 | 0.47 |
| 1.1 / 1.2 | Sílex (la piedra gris) | 0.178 | 0.20 |
| 1.1 / 1.2 | Pedernal (redonda y café) | 0.238 | 0.19 |
| 1.2 | Montón de hojas y ramitas | 0.213 | 0.19 |
| 1.2 | Niño y niña (agachados) | 0.190 · 0.232 | 0.22 |
| 1.2 | Papá y mamá (de pie, al este) | 0.255 · 0.278 | 0.20 |
| 1.3 | Hoguera | 0.215 | 0.21 |
| 1.3 | Familia alrededor | 0.200 – 0.290 | 0.20 |

Las dos piedras están puestas para que entren juntas en el charco de 1.1·L4 (centro 0.205, radio 0.30).
Si las mueves, revisa ese radio o Chispa nombrará algo que está a oscuras.

---

## 10. Lo que hace falta en el motor

| Qué | Por qué |
|---|---|
| **La capa de oscuridad del apartado 4** | Es lo único realmente nuevo. Sin ella este documento no se puede aplicar. |
| **Marca de «corte seco» por parada** | La última parada de la apertura salta de `x 0.700` a `x 0.260`. Con el suavizado normal son ~4 s de deriva invisible que dejan la cámara llegando tarde a la escena 1.1. Necesita saltar de golpe. |
| **Animación de luz temporizada dentro de una línea** | Tres casos, y son los tres momentos más importantes del nivel: el filo de luz que muere en apertura·L2, el destello de 3 frames en 1.2·L5 y la llama que crece en 1.3·L0. No dependen del progreso de lectura sino del reloj, así que el mecanismo por `Progress` no sirve para estos. |
| **Constante de suavizado por escena** | Lo mismo que en el Nivel 2. Aquí: 1.2 s en la apertura y en 1.1, **2.0 s en 1.2** (los pocos movimientos deben ser lentísimos y casi imperceptibles) y 1.4 s en 1.3. |

---

## 11. Dos avisos sobre el guion

**11.1 · Los dos primeros tiempos de la apertura pasan fuera de la cueva.** No hay arte de exterior. Mi
propuesta es el truco del apartado 4 (mismo sprite, frío, al 6%) porque una noche sin luna es casi negra
y funciona. Pero es una decisión tuya: si prefieres arte de exterior, esos dos encuadres se caen y hay
que rehacerlos.

**11.2 · `N1_NacimientoFuego` puede no existir todavía.** El inventario anterior solo listaba tres
secuencias del Nivel 1 (`N1_Apertura`, `N1_AparicionGuia`, `N1_Hallazgo`). La escena 1.3 está en el guion
(apartado 4.4) y es el pago del nivel entero, así que la diseñé completa. Si el asset no está, hay que
crearlo.

---

## 12. Qué queda por ver a ojo

- **Cuánto negro aguanta un niño de cuarto.** Es el riesgo real del nivel. Dejé la escena 1.2 en penumbra
  legible (`0.07–0.08`) en vez de negro absoluto justo por esto, pero el número correcto solo sale
  probándolo con niños. Si se hace pesado, sube `fondo` en bloque: el destello sigue funcionando mientras
  el salto sea de 10× o más.
- **El tamaño del charco depende del zoom.** El radio está en unidades de canvas, así que al cerrar el
  plano el charco ocupa más pantalla. Está tenido en cuenta en los valores, pero es lo primero que se
  descuadra si cambias un zoom.
- **Los tres momentos temporizados** (apartado 10) no se pueden juzgar en una imagen fija por definición.
- **Las posiciones de personajes** son propuesta: no hay sprites de la familia todavía.

---

## Apéndice — valores para copiar

```
CANVAS 3198x899   y desde abajo   zoom 1 = alto de pantalla
luz = (centro_x, centro_y, radio, fondo, tinte)   radio en unidades de ALTO del canvas
tintes:  frio (0.55,0.68,1.00)   ambar (1.00,0.90,0.72)   fuego (1.00,0.80,0.55)   chispa (0.92,0.94,1.00)

N1_Apertura  (entrada (este))
  L0    cam 0.780 0.520 1.22   luz 0.000 0.00 0.00 0.06   Noche helada, sin luna. La familia camina a 
  L1    cam 0.818 0.480 1.45   luz 0.950 0.42 0.30 0.10   Algo se acerca entre la maleza. La familia c
  L2    cam 0.700 0.440 1.60   luz 0.920 0.42 0.14 0.04   Avanzan hacia el fondo... la luz de la entra
  L3    cam 0.260 0.430 1.55   luz 0.000 0.00 0.00 0.00   La pantalla queda completamente negra durant

N1_AparicionGuia  (fondo (oeste))
  L0    cam 0.260 0.430 1.55   luz 0.255 0.46 0.18 0.02   un pequeno destello amarillo parpadea... una
  L1    cam 0.272 0.440 1.42   luz 0.258 0.46 0.22 0.03   CHISPA: Hola, familia! No tengan miedo...
  L2    cam 0.300 0.430 1.58   luz 0.285 0.45 0.24 0.03   La nina abre mucho los ojos. El nino suelta 
  L3    cam 0.215 0.445 1.72   luz 0.200 0.47 0.20 0.03   Chispa gira como un trompo y avanza hacia el
  L4    cam 0.208 0.390 1.78   luz 0.205 0.26 0.30 0.03   CHISPA: Ven esa piedra gris? Eso es silex. Y
  L5    cam 0.208 0.390 1.78   luz 0.205 0.26 0.06 0.01   Chispa parpadea una vez, dos veces, y se apa

N1_Hallazgo  (fondo (oeste))
  L0    cam 0.208 0.390 1.78   luz 0.000 0.00 0.00 0.08   NINA: Se fue?
  L1    cam 0.208 0.390 1.78   luz 0.000 0.00 0.00 0.08   PAPA: Si. Pero nos dejo algo.
  L2    cam 0.212 0.400 1.55   luz 0.000 0.00 0.00 0.07   El nino se arrodilla y busca con las manos l
  L3    cam 0.212 0.400 1.55   luz 0.000 0.00 0.00 0.07   NINO: Por que seran especiales unas piedras?
  L4    cam 0.212 0.400 1.55   luz 0.000 0.00 0.00 0.03   La nina las golpea una contra otra... Nada. 
  L5    cam 0.212 0.400 1.55   luz 0.205 0.20 0.60 0.95   CLIC! Un destello pequenisimo...
  L6    cam 0.212 0.400 1.55   luz 0.000 0.00 0.00 0.04   NINA: Lo viste?
  L7    cam 0.212 0.400 1.55   luz 0.205 0.20 0.22 0.06   NINO: Si! Otra vez, otra vez!
  L8    cam 0.218 0.450 1.48   luz 0.000 0.00 0.00 0.08   Papa se pone de pie despacio.
  L9    cam 0.218 0.450 1.48   luz 0.000 0.00 0.00 0.08   PAPA: Esperen. Estas piedras pueden ser peli
  L10   cam 0.258 0.445 1.52   luz 0.000 0.00 0.00 0.08   El nino quiere protestar, pero mama lo toma 
  L11   cam 0.258 0.445 1.52   luz 0.000 0.00 0.00 0.08   MAMA: Papa sabe lo que hace.
  L12   cam 0.222 0.390 1.62   luz 0.000 0.00 0.00 0.06   Papa se arrodilla, busca a tientas hojas sec
  L13   cam 0.215 0.370 1.70   luz 0.213 0.21 0.16 0.05   CHISPA: Tienes las piedras y tienes las hoja

N1_NacimientoFuego  (fondo (oeste))
  L0    cam 0.215 0.375 1.85   luz 0.213 0.21 0.45 0.35   Una llama crece despacio desde las hojas... 
  L1    cam 0.222 0.400 1.60   luz 0.215 0.22 0.50 0.45   Papa se queda quieto, mirando las llamas, co
  L2    cam 0.268 0.420 1.45   luz 0.222 0.23 0.60 0.55   NINOS: PAPAAAA! LO LOGRASTE!
  L3    cam 0.262 0.440 1.34   luz 0.228 0.24 0.70 0.65   Los ninos se lanzan encima de el. Mama llega
  L4    cam 0.238 0.420 1.55   luz 0.220 0.23 0.62 0.70   PAPA: Las piedras tenian razon. Eran especia
  L5    cam 0.222 0.395 1.80   luz 0.215 0.22 0.34 0.60   En el corazon de las llamas aparece Chispa, 
  L6    cam 0.235 0.430 1.50   luz 0.220 0.23 0.65 0.72   CHISPA: Yo no traje la luz. Solo les mostre 
  L7    cam 0.272 0.425 1.58   luz 0.225 0.23 0.66 0.72   NINA: Papa, tu eres como Chispa!
  L8    cam 0.268 0.418 1.66   luz 0.225 0.23 0.64 0.74   Papa se rie bajito y la abraza mas fuerte.
  L9    cam 0.270 0.500 1.18   luz 0.230 0.24 0.95 0.85   CHISPA: Eso tiene nombre: se llama iterar...
  L10   cam 0.252 0.455 1.38   luz 0.232 0.23 0.40 0.32   Chispa se recoge dentro de la fogata, como u
```
