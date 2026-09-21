# Movimientos de cámara del Nivel 2 — diseño completo sobre el entorno duplicado

Reemplaza el apartado 3 de `Camara_Narrativa.md`. Cubre las seis narrativas del Nivel 2 más las dos
fases jugables con entorno; el laberinto (fase 3) queda fuera porque usa vista superior.
**50 encuadres**, todos validados contra los límites reales de una pantalla 16:9.

Convención igual a la de los assets actuales: foco `(x, y)` normalizado sobre la ilustración,
**`y` medido desde abajo**, y `zoom` donde 1.0 = la ilustración cubre el alto de la pantalla.

---

## 1. Lo primero: tres encuadres actuales no se ven como están escritos

A un aspecto 16:9 la cámara no puede centrarse donde sea. Con la ilustración ajustada al alto,
el foco queda encerrado en `x ∈ [0.25/z, 1−0.25/z]` y `y ∈ [0.5/z, 1−0.5/z]`. Fuera de ahí se
descubriría un borde, así que el valor se recorta solo. Revisando los assets actuales:

| Asset | Valor escrito | Lo que realmente se ve | Efecto |
|---|---|---|---|
| `N2_Escena21_Bosque` inicio | (0.50, **0.33**) ×1.05 | y se recorta a **0.476** | **El plano del suelo con los objetos no ocurre.** A zoom 1.05 el foco solo puede moverse ±0.024 en vertical: es un plano general centrado, no un picado al suelo. Es el encuadre que pediste explícitamente y es el que no está pasando. |
| `N2_Escena22_ElPatron` familia | (**0.84**, 0.40) ×1.5 | x se recorta a **0.833** | Desplazamiento menor, casi imperceptible. |
| `N2_Escena24_Regreso` inicio y final | (0.75, **0.40**) ×1.2 → (**0.20**, 0.50) ×1.0 | y→0.417 ; x→0.250 | El paneo recorre menos de lo escrito. A zoom 1.0 el foco solo existe entre 0.25 y 0.75. |

Regla práctica para no volver a chocar con esto: **para bajar la cámara al suelo hay que cerrar el plano**.
Centrar en `y = 0.35` exige `zoom ≥ 1.43`; en `y = 0.30`, `zoom ≥ 1.67`. Un plano general (zoom ~1.1)
siempre está centrado a media altura — lo cual está bien, porque a zoom 1 el cuadro abarca todo el alto
y el suelo ocupa la mitad inferior de la pantalla por sí solo.

---

## 2. El entorno duplicado

Canvas final: **3198 × 899 px** (aspecto 3.557). A zoom 1 la pantalla muestra
**0.4998** del canvas — justo la mitad. El espejo va montado así:

- El original ocupa `x 0.000–0.500`; la copia espejada, `x 0.500–1.000`.
- **El espejo puro no tiene costura.** La última columna del original y la primera de la copia son la
  misma columna, así que el empalme es matemáticamente continuo: no hay nada que disimular ahí.
  Probé mezclas con degradado y troncos pegados encima: en arte de línea ambos ensucian más de lo que
  arreglan (fantasmas grises, cortes verticales en la copa). Se descartaron.
- Lo único que se le añade es **una pasada de luz asimétrica**: un solo foco alto a la izquierda
  (x≈0.30) que cae hacia el este, con un leve enfriamiento de color. Eso quita los dos focos gemelos
  de pasto que produce la duplicación y hace que las dos mitades se lean como dos sitios distintos:
  el bosque (cálido, claro) y el claro del refugio (más frío, más cerrado).

**Lo que de verdad impide que se note la repetición no es el arte, es el encuadre.** Ningún plano de
los 50 cruza `x = 0.500`, y por eso ningún plano contiene a la vez un tronco y su gemelo. La simetría
existe en el asset pero nunca entra en cuadro.

Dos formas de montarlo, la que prefieras:

1. **`entorno_n2_2x.png`** — listo, con la luz ya integrada. Se sustituye el sprite y ya.
2. **Sin asset nuevo** — dos `SpriteRenderer` con el mismo sprite, el de la derecha con `flipX = true`
   y desplazado exactamente un ancho. Es lo mismo geométricamente. Pierdes la corrección de luz
   (recuperable con un quad de degradado encima) pero ganas que cualquier reemplazo del arte
   se propague solo a las dos mitades.

---

## 3. Cuatro reglas del encuadre en este entorno

Salieron de mirar los recortes reales, no de la teoría. Las tres primeras las cumplen los 50 encuadres.

**R1 — La línea de árboles nunca sale del cuadro: `y + 0.5/zoom ≥ 0.66`.**
Es la regla importante y no es evidente. El claro está vacío a propósito (es el terreno de juego):
no tiene textura, ni línea, ni referencia. Un primer plano bajo sobre el pasto da una pantalla verde
lisa, y peor: **un paneo lateral sobre pasto vacío parece una imagen congelada**, porque no hay nada
que se desplace. Los troncos del fondo son el único punto de referencia del movimiento. La primera
versión de este diseño tenía los primeros planos en `y ≈ 0.33` y los renders salieron en blanco;
por eso todos los valores de abajo viven entre `y 0.39` y `y 0.60`.

**R2 — Ningún encuadre cruza `x = 0.500`.** Ni un plano fijo ni el recorrido de un paneo.
Es lo que mantiene invisible la duplicación.

**R3 — Zoom narrativo entre 1.16 y 1.95.** Por debajo de ~1.15 el cuadro es casi el sprite original
entero y se lee como fondo repetido; por encima de ~2.0 el pasto vacío se come la pantalla.

**R4 — El corte entre escenas es gratis; el paneo largo no.** Cada escena es un asset con su propio
encuadre inicial, así que saltar de un extremo del canvas al otro entre escenas no cuesta nada y
además hace que el nivel parezca tener varios escenarios. Dentro de una escena, en cambio, conviene
no recorrer más de ~0.15 del canvas.

---

## 4. Mapa de zonas: un sprite, cuatro sitios

| Zona | x | Qué es | Dónde se usa |
|---|---|---|---|
| **Claro oeste** | 0.10 – 0.35 | El bosque de recolección. Suelo despejado, luz cálida. | 2.1 · fase 1 · 2.2 |
| **Corredor central** | 0.38 – 0.63 | El camino, con el seto en primer plano tapando la base. Zona de tránsito. | tramo de 2.4 |
| **Claro este** | 0.64 – 0.90 | Descampado junto al refugio, luz más fría. | Puente I · 2.3 · fase 2 · 2.4 · 2.5 |
| **Eje del espejo** | 0.500 | Zona prohibida para la cámara. | — |

La geografía queda coherente con el guion: la cueva del Nivel 1 queda fuera de cuadro al este, la
familia sale de ella al claro este (Puente I), camina al bosque a recolectar (2.1–2.2), vuelve al este
a construir (2.3) y sigue al este hacia el refugio (2.4–2.5). El seto del centro hace de barrido
natural cada vez que se cruza el corredor.

El primer plano del seto (`x 0.365–0.635`, alto `y 0.00–0.28`) sirve como oclusor: si lo pasas a una capa
`NarrativeProp` que se mueva a **1.15×** la cámara, ganas paralaje real y el corredor deja de ser plano.
Es el único añadido de arte que recomendaría.

---

## 5. Escena por escena

Cada línea es una parada (`CameraKeys`). La parada 0 hace de `CameraStart`; la cámara llega a cada
encuadre y se queda quieta hasta que el jugador avanza a la línea de la siguiente parada.

### 5.1 · `N2_PuenteI` — Del fuego al alimento

*Zona: claro este · 9 paradas · recorre 0.133 del canvas*

Nueve tiempos. **Arco: cerrado arriba → abre y baja → cierra en el suelo → abre del todo.** Empieza
pegada a los troncos con las brasas humeando y termina en el plano más abierto de la escena, mirando al
oeste hacia el claro vacío: la pregunta de la niña («¿y si el problema no es la fuerza… sino la forma?»)
queda apuntando literalmente al sitio donde está la respuesta, que es donde arranca 2.1.

| # | Línea | Foco (x, y) | Zoom | Movimiento |
|---|---|---|---|---|
| `L0` | Amanece. La familia sale de la cueva... | **(0.845, 0.600)** | **1.70** | cerrado y alto contra los arboles: las brasas humean |
| `L1` | CHISPA: Ahora que tienen fuego, pueden cocinar | **(0.824, 0.545)** | **1.55** | baja y abre: entra el suelo del claro |
| `L2` | MAMA: Comida. | **(0.812, 0.510)** | **1.45** | sigue bajando, la familia ya esta en el claro |
| `L3` | La familia sale a recolectar... | **(0.760, 0.490)** | **1.28** | deriva al oeste y abre: plano de la recoleccion |
| `L4` | PAPA: Esto pesa demasiado... | **(0.735, 0.455)** | **1.50** | cierra sobre el monton en el suelo |
| `L5` | NINO: Yo puedo arrastrarlo! | **(0.718, 0.440)** | **1.70** | empuje corto sobre el nino y la carga |
| `L6` | Lo intenta. Las cosas se atascan. | **(0.712, 0.430)** | **1.80** | lo mas cerrado de la escena: el roce con la tierra |
| `L7` | MAMA: Algo no esta funcionando... | **(0.742, 0.455)** | **1.42** | retrocede y recompone al grupo |
| `L8` | NINA: Y si el problema no es la fuerza... | **(0.714, 0.490)** | **1.22** | abre del todo mirando al oeste: el claro vacio por delante |

### 5.2 · `N2_Escena21_Bosque` — Objetos dispersos

*Zona: claro oeste · 6 paradas · recorre 0.140 del canvas*

**Corte al claro oeste.** Abre pegada al suelo con los catorce objetos llenando la mitad baja del cuadro
—es el encuadre que pediste y que hoy no ocurre— y va derivando al oeste, rozando el suelo, hasta llegar
a la caja. La última línea abre al plano de juego: **el encuadre final de 2.1 es idéntico al de la fase 1**,
así que el paso de narrativa a juego no tiene salto.

| # | Línea | Foco (x, y) | Zoom | Movimiento |
|---|---|---|---|---|
| `L0` | Bosque. Objetos dispersos por el suelo... | **(0.330, 0.400)** | **1.62** | CORTE al oeste. Suelo dominante: los objetos llenan la mitad baja |
| `L1` | CHISPA: Observen bien... | **(0.298, 0.430)** | **1.52** | deriva al oeste rozando el suelo |
| `L2` | MAMA: algunas cosas se mueven mas facil... | **(0.258, 0.405)** | **1.60** | baja otra vez sobre los troncos redondos |
| `L3` | NINO: Voy a probar todas. | **(0.228, 0.395)** | **1.68** | empujon corto, mas cerca del suelo |
| `L4` | NINA: Solo lo que realmente funciona. | **(0.190, 0.425)** | **1.45** | llega a la caja: 'a un lado, la caja' |
| `L5` | CHISPA: Selecciona... cuando tengas cinco | **(0.270, 0.500)** | **1.18** | ABRE al plano de juego (= frame exacto de la fase 1) |

### 5.3 · `Level2_Forest` — Fase 1 jugable

*Zona: claro oeste · 2 paradas · recorre 0.038 del canvas*

Plano general fijo durante toda la recolección, tal como pediste. Al acopiar el quinto tronco, el zoom
hacia la caja (1,2 s) deja la cámara exactamente en el encuadre con el que abre 2.2.

| # | Línea | Foco (x, y) | Zoom | Movimiento |
|---|---|---|---|---|
| `JUEGO` | Plano general fijo durante toda la recoleccion | **(0.270, 0.500)** | **1.18** | identico al final de 2.1: no hay salto |
| `CIERRE` | Al acopiar el quinto tronco | **(0.232, 0.420)** | **1.58** | zoom de 1.2s hacia la caja mientras los troncos vuelan |

### 5.4 · `N2_Escena22_ElPatron` — El patrón

*Zona: claro oeste · 6 paradas · recorre 0.154 del canvas*

La escena con paradas. Hereda el encuadre exacto del final de la fase 1 y **no se mueve** en la línea 0:
misma vista, mismos troncos, mismo sitio, la caja rodando sola. A partir de ahí:
cierra sobre el niño → paneo puro al este hasta papá (misma escala, 0,84 pantallas de recorrido: se lee
como un giro de cabeza) → sigue al este y abre con la familia → micro empuje sobre la niña, que es quien
nombra el patrón → abre y sube con Chispa. **Arco: medio → cerrado → paneo → abierto → cerrado → general.**

| # | Línea | Foco (x, y) | Zoom | Movimiento |
|---|---|---|---|---|
| `L0` | La caja rueda sobre los troncos... | **(0.232, 0.420)** | **1.58** | HEREDA EXACTO el frame de la fase 1. La camara no se mueve |
| `L1` | NINO: Este tronco rueda! | **(0.168, 0.440)** | **1.95** | cierra sobre el nino: detras, el tronco nudoso |
| `L2` | PAPA: Pero esa piedra no... | **(0.276, 0.440)** | **1.95** | paneo puro al este, misma escala: 0.84 pantallas |
| `L3` | MAMA: Que diferencia hay? | **(0.312, 0.460)** | **1.58** | sigue al este y abre: entra la familia |
| `L4` | NINA: Los que ruedan... son redondos. | **(0.322, 0.450)** | **1.78** | micro empuje sobre la nina: ella nombra el patron |
| `L5` | CHISPA: Acabas de encontrar un patron... | **(0.296, 0.500)** | **1.26** | abre y sube: el patron es el cuadro entero |

### 5.5 · `N2_Escena23_Construccion` — El taller

*Zona: claro este · 8 paradas · recorre 0.106 del canvas*

**Corte al claro este.** La luz más fría y el encuadre más cerrado lo separan del bosque.
La última línea de Chispa enumera cuatro pasos en orden, y la cámara los recorre uno por uno: cada pieza
entra en cuadro justo cuando se la nombra. Eso **exige partir esa línea en cuatro** (ver apartado 7).
El encuadre final es el de la fase 2.

| # | Línea | Foco (x, y) | Zoom | Movimiento |
|---|---|---|---|---|
| `L0` | NINA: Si hacemos algo redondo... | **(0.700, 0.450)** | **1.62** | CORTE al claro este. Plano medio sobre la nina |
| `L1` | PAPA: Construyamos uno. | **(0.745, 0.455)** | **1.52** | deriva al este y abre un poco |
| `L2` | NINO: Probemos! | **(0.762, 0.435)** | **1.78** | empuje rapido |
| `L3` | MAMA: Paso a paso... podemos lograrlo. | **(0.775, 0.470)** | **1.35** | retrocede: aparecen las seis piezas |
| `L4a` | CHISPA: Abre agujeros en los troncos cortos | **(0.694, 0.420)** | **1.95** | cierra sobre los dos troncos cortos |
| `L4b` | CHISPA: luego unelos con el tronco largo | **(0.748, 0.420)** | **1.95** | paneo al este: el tronco largo |
| `L4c` | CHISPA: coloca encima la tabla | **(0.800, 0.430)** | **1.88** | paneo al este: la tabla |
| `L4d` | CHISPA: y sobre ella la caja. En ese orden. | **(0.772, 0.470)** | **1.32** | retrocede al banco completo (= frame de la fase 2) |

### 5.6 · Fase 2 jugable — Construcción

*Zona: claro este · 2 paradas · recorre 0.013 del canvas*

Plano fijo del banco de trabajo, heredado de 2.3. Al terminar la carretilla, un empuje de 1,2 s que rima
con el cierre de la fase 1: el nivel cierra sus dos fases con el mismo gesto.

| # | Línea | Foco (x, y) | Zoom | Movimiento |
|---|---|---|---|---|
| `JUEGO` | Plano fijo del banco de trabajo | **(0.772, 0.470)** | **1.32** | identico al final de 2.3 |
| `CIERRE` | Carretilla terminada | **(0.785, 0.445)** | **1.56** | empuje de 1.2s sobre la carretilla: rima con la fase 1 |

### 5.7 · `N2_Escena24_Regreso` — El regreso

*Zona: corredor este · 6 paradas · recorre 0.101 del canvas*

La única escena que viaja. «Empujar / Girar / Evitar la piedra» son tres órdenes cortas, y la cámara las
ejecuta: **tres pasos al este, del mismo tamaño y con el mismo zoom**, como tres instrucciones puestas en
fila. La cámara hace lo que el jugador va a hacer con los bloques en el laberinto. Luego retrocede (los
tres pasos se ven como un solo plan) y termina en el plano más abierto del nivel: el camino por delante.

| # | Línea | Foco (x, y) | Zoom | Movimiento |
|---|---|---|---|---|
| `L0` | CHISPA: Ahora tienen la carretilla... | **(0.665, 0.445)** | **1.60** | sobre la carretilla, mirando el corredor |
| `L1` | PAPA: Empujar... | **(0.700, 0.440)** | **1.68** | PASO 1: la camara da un paso al este |
| `L2` | NINO: Girar... | **(0.733, 0.440)** | **1.68** | PASO 2: mismo tamano, mismo salto |
| `L3` | MAMA: Evitar la piedra... | **(0.766, 0.440)** | **1.68** | PASO 3: la piedra entra por el borde |
| `L4` | NINA: Si ordenamos bien los pasos... | **(0.740, 0.470)** | **1.36** | retrocede: los tres pasos se ven como uno solo |
| `L5` | CHISPA: Lleva la carretilla hasta el refugio | **(0.755, 0.500)** | **1.19** | el plano mas abierto del nivel: el camino por delante |

### 5.8 · `N2_Escena25_Cierre` — Cierre del nivel

*Zona: refugio (este) · 6 paradas · recorre 0.080 del canvas*

Un solo movimiento: **retroceso lento y continuo en seis tiempos**, del fuego a plano general. Empieza en
lo más cerrado de todo el nivel (2.10) y termina en lo más abierto (1.16), con la familia pequeña dentro
del claro. Que sea un único gesto sin paradas laterales es lo que lo hace leerse como cierre.

| # | Línea | Foco (x, y) | Zoom | Movimiento |
|---|---|---|---|---|
| `L0` | La familia en el refugio, alrededor del fuego | **(0.852, 0.455)** | **1.95** | lo mas cerrado del nivel: el fuego y la carretilla cargada |
| `L1` | CHISPA: Las grandes ideas nacen... | **(0.840, 0.465)** | **1.75** | empieza el retroceso lento |
| `L2` | CHISPA: ...observas, comparas y organizas | **(0.822, 0.475)** | **1.55** | sigue abriendo |
| `L3` | CHISPA: hicieron girar su pensamiento | **(0.808, 0.485)** | **1.40** | sigue |
| `L4` | CHISPA: Eso se llama abstraer | **(0.798, 0.492)** | **1.30** | sigue |
| `L5` | CHISPA: ...pensar como un algoritmo | **(0.772, 0.500)** | **1.16** | plano general: la familia pequena en el claro. Fundido |

---

## 6. Escena puente II (opcional)

Arranca en el refugio, así que puede reutilizar el claro este antes de cortar al arte del río. El guion
pide literalmente «la cámara se desplaza hacia el horizonte»: aquí eso es **subir a la copa**, al hueco
entre los árboles del borde este, y cerrar sobre él para el humo lejano.

| # | Línea | Foco (x, y) | Zoom | Movimiento |
|---|---|---|---|---|
| `L0` | La familia celebra en el refugio | **(0.800, 0.470)** | **1.52** | reencuadre desde el cierre del nivel |
| `L1` | CHISPA: Lo lograron!... | **(0.810, 0.490)** | **1.42** | abre hacia el este |
| `L2` | La camara se desplaza hacia el horizonte | **(0.832, 0.585)** | **1.62** | SUBE a la copa: el hueco entre los arboles |
| `L3` | CHISPA: Otros como ustedes. Mas adelante. | **(0.845, 0.620)** | **1.78** | cierra sobre el hueco: el humo lejano |
| `L4` | La familia avanza. El camino se corta. | **(0.800, 0.460)** | **1.35** | cae al suelo y frena en seco: corte al Nivel 3 |

---

## 7. Dónde va cada cosa

Para que se sostenga tu regla de que lo que se menciona concuerde con lo que se muestra. Coordenadas
sobre el mismo canvas; `y` es la **base** del objeto (donde toca el suelo). Los personajes miden ~0.22
de alto, así que con los pies en `y 0.30` la cabeza queda en `y ≈ 0.52`.

### Claro oeste — 2.1, fase 1 y 2.2

| Elemento | x | y | Comprobación |
|---|---|---|---|
| Caja de alimentos | 0.115 | 0.33 | «a un lado, la caja»: queda fuera de cuadro hasta 2.1·L4, que es donde el texto la nombra |
| 5 troncos redondos | 0.155 · 0.205 · 0.248 · 0.292 · 0.335 | 0.26–0.35 | los correctos |
| 4 piedras irregulares | 0.175 · 0.225 · 0.268 · 0.315 | 0.24–0.40 | distractores |
| 3 plantas | 0.140 · 0.190 · 0.300 | 0.27–0.38 | distractores |
| 2 herramientas | 0.230 · 0.345 | 0.31–0.36 | distractores |

Ninguno pasa de `x 0.350`: más al este empieza el seto del primer plano y quedarían tapados.
Los catorce caben en el plano de juego (`0.058–0.482`) con margen.

**Puesta en escena de 2.2** — es lo que hace que las paradas signifiquen algo:

| Elemento | x | Entra en cuadro en |
|---|---|---|
| Caja + 5 troncos rodando | 0.200 – 0.270 | L0 (heredado de la fase 1) |
| Niño con el tronco que rueda | 0.168 | L1, centrado |
| Papá con la piedra | 0.276 | L2, centrado tras el paneo |
| Mamá y niña | 0.305 · 0.325 | L3 |
| Chispa | 0.296 · y 0.60 | L5, al abrir y subir |

### Claro este — Puente I, 2.3, fase 2, 2.4 y 2.5

| Escena | Elemento | x | y |
|---|---|---|---|
| Puente I | Brasas / boca de la cueva (fuera de cuadro al este) | 0.86 | 0.45 |
| Puente I | Montón de alimentos y troncos | 0.735 | 0.32 |
| Puente I | Familia | 0.700 – 0.780 | 0.30 |
| 2.3 / fase 2 | Troncos cortos (×2) | 0.690 · 0.706 | 0.28 |
| 2.3 / fase 2 | Tronco largo | 0.748 | 0.29 |
| 2.3 / fase 2 | Herramienta | 0.772 | 0.27 |
| 2.3 / fase 2 | Tabla | 0.800 | 0.30 |
| 2.3 / fase 2 | Caja de alimentos | 0.828 | 0.31 |
| 2.4 | Carretilla | 0.672 | 0.30 |
| 2.4 | La piedra que hay que evitar | 0.870 | 0.29 |
| 2.5 | Fuego del refugio | 0.852 | 0.30 |
| 2.5 | Carretilla cargada | 0.790 | 0.31 |
| 2.5 | Familia alrededor del fuego | 0.800 – 0.900 | 0.30 |

El orden de las piezas de 2.3 en el suelo es de oeste a este **en el mismo orden en que Chispa las
nombra**. Por eso el paneo de L4a→L4c funciona: la cámara lee la lista.

---

## 8. Dos cambios que le pido al guion

**8.1 · Partir la instrucción de Chispa en 2.3 en cuatro líneas.** Hoy es una sola:

> CHISPA: Abre agujeros en el centro de los troncos cortos, luego únelos con el tronco largo, coloca
> encima la tabla y sobre ella la caja de alimentos. En ese orden: cada paso necesita que el anterior
> esté hecho.

Propuesta:

> CHISPA: Abre agujeros en el centro de los troncos cortos.  
> CHISPA: Luego únelos con el tronco largo.  
> CHISPA: Coloca encima la tabla.  
> CHISPA: Y sobre ella, la caja de alimentos. En ese orden: cada paso necesita que el anterior esté hecho.

Dos motivos. Uno, la cámara solo puede moverse entre líneas, así que sin el corte no puede recorrer las
piezas mientras se nombran. Dos, son cuatro instrucciones secuenciales para un niño de cuarto: cuatro
cuadros de diálogo cortos se retienen mejor que un párrafo, y el propio nivel trata de descomponer.

**8.2 · Confirmar el reparto de líneas largas.** Las tablas de arriba asumen que las dos intervenciones
largas de Chispa en 2.5 se parten en dos cada una (6 líneas, como ya figura en tu inventario). Las
paradas están ancladas **al texto de la línea, no al índice**, así que si el reparto cambia solo hay que
mover la parada a la línea que empieza con ese texto.

---

## 9. Lo que habría que tocar en el motor

Nada estructural; los tres son datos.

| Qué | Por qué |
|---|---|
| **Constante de suavizado por escena**, no global de 1,5 s | 2.4 necesita ~0,7 s para que los tres pasos se lean como pasos discretos; 2.5 necesita ~2,5 s para que el retroceso sea un solo gesto continuo. El resto se queda en 1,2–1,5 s. Con un único valor global, o los pasos se emborronan o el cierre se siente brusco. |
| **Aviso de recorte en el Inspector** | Un mensaje cuando un foco cae fuera de `[0.25/z, 1−0.25/z] × [0.5/z, 1−0.5/z]` habría cazado los tres casos del apartado 1 al escribirlos. Es una comparación. |
| **Capa de primer plano a 1,15× (opcional)** | El seto del centro con paralaje da profundidad real al corredor y refuerza el barrido al cruzarlo. |

---

## 10. Hojas de verificación

Los PNG que acompañan este documento están generados con los valores exactos de las tablas, recortando
el entorno real. No son bocetos: es lo que se va a ver.

| Archivo | Qué es |
|---|---|
| `entorno_n2_2x.png` | El entorno duplicado, 3198×899. El asset. |
| `00_mapa_zonas.png` | Las zonas y el eje del espejo sobre el canvas, con regla de coordenadas. |
| `10_recorrido_*.png` | El recorrido de cada escena dibujado sobre el canvas: cada rectángulo es una parada, la línea roja une los focos. |
| `20_frames_*.png` | **La importante.** Lo que ve el jugador en cada parada, recortado de verdad. |

---

## 11. Qué sigue sin estar verificado

Los 50 encuadres pasan las tres reglas comprobables por cálculo —dentro de límites, con línea de árboles
en cuadro, sin cruzar el eje— y los tengo mirados uno a uno en los recortes. Lo que **no** se puede saber
sin verlo en Play:

- **El ritmo.** Cuánto dura cada parada depende de a qué velocidad lea el niño, y eso no lo fija la cámara.
  Los arcos están pensados para lectura pausada; si los niños avanzan rápido, los movimientos se van a
  sentir atropellados y hay que bajar los zooms máximos.
- **El paneo de 2.2·L1→L2** (niño → papá, 0,84 pantallas). Es el movimiento más largo a zoom alto de todo
  el nivel. En el recorte se ve que los troncos del fondo cambian lo suficiente, pero es el primero que
  miraría en Play.
- **Los tres pasos de 2.4.** Dependen de la constante de suavizado; con 1,5 s se emborronan.
- **La posición de los personajes.** Las coordenadas del apartado 7 son propuesta: no hay sprites de la
  familia todavía, así que están calculadas para que cada uno quede centrado en su parada, no medidas
  contra arte real.

---

## Apéndice — valores para copiar

```
CANVAS 3198x899   y desde abajo   zoom 1 = alto de pantalla
limites:  x en [0.25/z, 1-0.25/z]   y en [0.5/z, 1-0.5/z]

PuenteI  (claro este)
  L0     0.845 0.600 1.70   Amanece. La familia sale de la cueva...
  L1     0.824 0.545 1.55   CHISPA: Ahora que tienen fuego, pueden cocinar
  L2     0.812 0.510 1.45   MAMA: Comida.
  L3     0.760 0.490 1.28   La familia sale a recolectar...
  L4     0.735 0.455 1.50   PAPA: Esto pesa demasiado...
  L5     0.718 0.440 1.70   NINO: Yo puedo arrastrarlo!
  L6     0.712 0.430 1.80   Lo intenta. Las cosas se atascan.
  L7     0.742 0.455 1.42   MAMA: Algo no esta funcionando...
  L8     0.714 0.490 1.22   NINA: Y si el problema no es la fuerza...

E21  (claro oeste)
  L0     0.330 0.400 1.62   Bosque. Objetos dispersos por el suelo...
  L1     0.298 0.430 1.52   CHISPA: Observen bien...
  L2     0.258 0.405 1.60   MAMA: algunas cosas se mueven mas facil...
  L3     0.228 0.395 1.68   NINO: Voy a probar todas.
  L4     0.190 0.425 1.45   NINA: Solo lo que realmente funciona.
  L5     0.270 0.500 1.18   CHISPA: Selecciona... cuando tengas cinco

F1  (claro oeste)
  JUEGO  0.270 0.500 1.18   Plano general fijo durante toda la recoleccion
  CIERRE 0.232 0.420 1.58   Al acopiar el quinto tronco

E22  (claro oeste)
  L0     0.232 0.420 1.58   La caja rueda sobre los troncos...
  L1     0.168 0.440 1.95   NINO: Este tronco rueda!
  L2     0.276 0.440 1.95   PAPA: Pero esa piedra no...
  L3     0.312 0.460 1.58   MAMA: Que diferencia hay?
  L4     0.322 0.450 1.78   NINA: Los que ruedan... son redondos.
  L5     0.296 0.500 1.26   CHISPA: Acabas de encontrar un patron...

E23  (claro este)
  L0     0.700 0.450 1.62   NINA: Si hacemos algo redondo...
  L1     0.745 0.455 1.52   PAPA: Construyamos uno.
  L2     0.762 0.435 1.78   NINO: Probemos!
  L3     0.775 0.470 1.35   MAMA: Paso a paso... podemos lograrlo.
  L4a    0.694 0.420 1.95   CHISPA: Abre agujeros en los troncos cortos
  L4b    0.748 0.420 1.95   CHISPA: luego unelos con el tronco largo
  L4c    0.800 0.430 1.88   CHISPA: coloca encima la tabla
  L4d    0.772 0.470 1.32   CHISPA: y sobre ella la caja. En ese orden.

F2  (claro este)
  JUEGO  0.772 0.470 1.32   Plano fijo del banco de trabajo
  CIERRE 0.785 0.445 1.56   Carretilla terminada

E24  (corredor este)
  L0     0.665 0.445 1.60   CHISPA: Ahora tienen la carretilla...
  L1     0.700 0.440 1.68   PAPA: Empujar...
  L2     0.733 0.440 1.68   NINO: Girar...
  L3     0.766 0.440 1.68   MAMA: Evitar la piedra...
  L4     0.740 0.470 1.36   NINA: Si ordenamos bien los pasos...
  L5     0.755 0.500 1.19   CHISPA: Lleva la carretilla hasta el refugio

E25  (refugio (este))
  L0     0.852 0.455 1.95   La familia en el refugio, alrededor del fuego
  L1     0.840 0.465 1.75   CHISPA: Las grandes ideas nacen...
  L2     0.822 0.475 1.55   CHISPA: ...observas, comparas y organizas
  L3     0.808 0.485 1.40   CHISPA: hicieron girar su pensamiento
  L4     0.798 0.492 1.30   CHISPA: Eso se llama abstraer
  L5     0.772 0.500 1.16   CHISPA: ...pensar como un algoritmo

PuenteII  (refugio -> horizonte)
  L0     0.800 0.470 1.52   La familia celebra en el refugio
  L1     0.810 0.490 1.42   CHISPA: Lo lograron!...
  L2     0.832 0.585 1.62   La camara se desplaza hacia el horizonte
  L3     0.845 0.620 1.78   CHISPA: Otros como ustedes. Mas adelante.
  L4     0.800 0.460 1.35   La familia avanza. El camino se corta.
```
