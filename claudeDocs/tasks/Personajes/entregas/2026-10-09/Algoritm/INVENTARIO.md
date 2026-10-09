# Inventario de la entrega de Algoritm del 09/10/2026 (INC-131)

Entrega: `claudeDocs/tasks/Personajes/entregas/2026-10-09/Algoritm/` (commit `545127e`), 21 PNG en LFS: las tres formas
—`Fuego/`, `Rueda/`, `Gota/`— en **siete piezas de frente** cada una. Hoja de contacto:
[`hoja_contacto.png`](hoja_contacto.png). Medido con Pillow sobre los píxeles (`git lfs pull`), fuera del Editor; comparado
con los prefabs `Algoritm_{Fuego,Rueda,Gota}.prefab`, las entradas `algoritm_*` de `rig_articulaciones.json`, los 9 clips de
`clips_personajes.json` (árbol de trabajo del 09/10) y los tres `_reposo` de `Assets/Game/Art/Characters/Algoritm/`.

Convenciones: las de la entrega de la familia. Coordenadas en px del **lienzo de la entrega** (1300×1500, origen arriba a
la izquierda, y hacia abajo); recuadro opaco = caja de la componente conexa más grande con alfa ≥ 12, con x1 e y1 exclusivos;
«rig» = el lienzo de 1024 del prefab (`cut.py`, `rig_articulaciones.json`).

## Resumen

| | Fuego | Rueda | Gota |
|---|---|---|---|
| Piezas | 7: torso, 2 brazos, 2 manos, 2 pies | 7, igual | 7: torso, 2 brazos, 2 manos, 2 **piernas** |
| Lienzo común registrado | sí, 1300×1500 | sí | sí |
| Extremidades | las **mismas siluetas** en las tres formas (XOR del alfa = 0 px), recoloreadas | ídem | ídem |
| Cara (ojos, boca) | **no hay** | **no hay** | **no hay** |
| Ojos cerrados / expresiones | no | no | no |
| Alto de la figura en la entrega | 1289 px (y 44 → 1333) | 1074 px (259 → 1333) | 1153 px (180 → 1333) |
| Cuerpo | llama de tres lenguas, núcleo amarillo en degradado | **disco de madera** con anillos | **gota** con brillos |
| Pantaloneta (sustituye a la franja) | verde `#277D4A`, cuadros amarillos | naranja `#FEAA40`, cuadros rojos | granate `#A13C4F`, cuadros rosa |
| Color de brazos, manos y piernas | `#FF9122` | `#5B4134` | `#6ED6FB` |
| Contorno | negro `#000000` | negro | negro |
| Memoria RGBA de las 7 partes (opción A) | 2,37 MB | 2,30 MB | 2,21 MB |

**Lo que cambia el plan:**

1. **No hay cara.** Ni en el torso (el interior de la llama, el disco y la gota no tiene un solo trazo de ojo ni de boca:
   los píxeles oscuros del interior son el contorno de las lenguas, un anillo y la cinturilla) ni en capas aparte. No hay
   nada que separar con `separa()`: las capas `Ojos` y `Boca` tienen que llegar dibujadas. Sin ellas, Algoritm sale **sin
   cara** en las 18 narrativas y en el botón de ayuda, y en el Nivel 2 rompe la condición 2 de §7.6 («Tiene cara. Ningún prop
   del juego tiene ojos ni boca»): un disco de madera sin cara es un prop del bosque.
2. **La pierna llega entera** (palo + pie, `pie_*` o `pierna_*`), **sin muslo ni rodilla**. Con siete piezas, la pierna
   va en `PiernaX` y `RodillaX`/`AntepiernaX` quedan vacíos (ver «Siete o nueve piezas»).
3. **El diseño no es el del núcleo de identidad de §7.6** (INC-52): el cuerpo cambia de silueta por forma (llama, disco,
   gota), la franja de cinco colores pasa a ser una pantaloneta de cuadros con un color por forma, las extremidades dejan de
   ser palos del color del contorno (son gruesas y del color del material, y las manos también: ya no son color piel), los
   pies son óvalos y no trazos, el contorno es negro y no `#3B1205`, y el cuerpo lleva degradado y brillos (§2.2, color
   plano). §7.6 ya lo prevé —«si el arte definitivo cambia la silueta, esta tabla, §7.6 y el guion §1.1.1 se corrigen con
   él»—, pero cambia **todos** los rasgos del núcleo a la vez: pide un INC y la decisión de Santiago antes de entrar.
4. **`preparar_arte_final.py` no sirve tal cual:** solo acepta `papa|mama|nina|nino`, exige diez piezas con `cabeza`, toma
   `pie` por antepierna y `pierna` por muslo (las mismas piernas acaban en nodos distintos según la forma) y normaliza cada
   figura a 870 px por su cuenta (ver «Recomendaciones»).
5. **Los nombres dicen el lado de la pantalla**, no el del personaje: `derecho` está a la derecha de la pantalla. Coincide
   con la convención del rig (`Izq` = izquierda de la pantalla), así que no hay que espejar nada.

## Estructura de la entrega

```
2026-10-09/Algoritm/
├─ Fuego/ (7)  torso_fuego_algoritm, brazo_{derecho,izquierdo}_fuego, mano_{derecho,izquierdo}_fuego, pie_{derecho,izquierdo}_fuego
├─ Rueda/ (7)  torso_rueda,          brazo_{derecho,izquierdo}_rueda, mano_{derecho,izquierdo}_rueda, pie_{derecho,izquierdo}_rueda
└─ Gota/  (7)  torso_gota_algoritm,  brazo_{derecho,izquierdo}_gota,  mano_{derecha,izquierdo}_gota,  pierna_{derecha,izquierdo}_gota
```

Todos son RGBA de 1300×1500, el mismo lienzo para las siete piezas de una forma **y para las tres formas**: brazos, manos,
piernas y pantaloneta ocupan exactamente el mismo sitio en las tres (los recuadros y las filas de la pantaloneta coinciden
al píxel); solo cambia lo que hay por encima de la cintura. No hay `cabeza`, ni `Expresiones/`, ni `_reposo`.

## Piezas por forma

Papel por nombre: `torso` = cuerpo y pantaloneta, sin extremidades; `brazo` = húmero (del hombro al codo); `mano` = antebrazo
con la mano; `pie` (Fuego, Rueda) o `pierna` (Gota) = **la pierna entera con el pie**. Las extremidades de las tres formas son
la misma silueta: la tabla de Fuego vale para las otras dos salvo el torso.

#### Fuego (`algoritm_fuego`)

| Archivo (bajo `Fuego/`) | Papel en la entrega | Destino (`char_algoritm_fuego_`) | Nodo del rig | Recuadro opaco (x0, y0, x1, y1) | Px opacos | Normalizado (opción A) |
|---|---|---|---|---|---:|---:|
| `torso_fuego_algoritm.png` | cuerpo (llama) + pantaloneta | `parte_torso` | `Tronco/Torso` | (319, 44, 948, 1162) | 459 044 | 480×852 |
| `brazo_izquierdo_fuego.png` | húmero, izquierda de la pantalla | `parte_brazo_izq` | `Tronco/BrazoIzq` | (230, 772, 411, 971) | 13 734 | 138×152 |
| `brazo_derecho_fuego.png` | húmero, derecha de la pantalla | `parte_brazo_der` | `Tronco/BrazoDer` | (859, 775, 1049, 966) | 13 731 | 145×146 |
| `mano_izquierdo_fuego.png` | antebrazo + mano | `parte_antebrazo_izq` | `BrazoIzq/CodoIzq/AntebrazoIzq` | (19, 904, 296, 1164) | 30 547 | 212×199 |
| `mano_derecho_fuego.png` | antebrazo + mano | `parte_antebrazo_der` | `BrazoDer/CodoDer/AntebrazoDer` | (988, 901, 1271, 1159) | 30 544 | 216×197 |
| `pie_izquierdo_fuego.png` | pierna entera + pie | `parte_pierna_izq` | `Cuerpo/PiernaIzq` | (332, 1085, 534, 1328) | 21 390 | 154×186 |
| `pie_derecho_fuego.png` | pierna entera + pie | `parte_pierna_der` | `Cuerpo/PiernaDer` | (635, 1095, 841, 1333) | 21 389 | 157×182 |

#### Rueda (`algoritm_rueda`)

| Archivo (bajo `Rueda/`) | Papel | Destino (`char_algoritm_rueda_`) | Recuadro opaco | Px opacos | Normalizado (A) |
|---|---|---|---|---:|---:|
| `torso_rueda.png` | cuerpo (disco de madera) + pantaloneta | `parte_torso` | (272, 259, 1018, 1162) | 498 522 | 569×688 |
| `brazo_{izquierdo,derecho}_rueda.png` | húmeros | `parte_brazo_{izq,der}` | como Fuego | 13 734 / 13 731 | 138×152 / 145×146 |
| `mano_{izquierdo,derecho}_rueda.png` | antebrazos + manos | `parte_antebrazo_{izq,der}` | como Fuego | 30 547 / 30 544 | 212×199 / 216×197 |
| `pie_{izquierdo,derecho}_rueda.png` | piernas enteras | `parte_pierna_{izq,der}` | como Fuego | 21 390 / 21 389 | 154×186 / 157×182 |

#### Gota (`algoritm_gota`)

| Archivo (bajo `Gota/`) | Papel | Destino (`char_algoritm_gota_`) | Recuadro opaco | Px opacos | Normalizado (A) |
|---|---|---|---|---:|---:|
| `torso_gota_algoritm.png` | cuerpo (gota) + pantaloneta | `parte_torso` | (299, 180, 946, 1162) | 439 797 | 493×749 |
| `brazo_{izquierdo,derecho}_gota.png` | húmeros (con brillos) | `parte_brazo_{izq,der}` | como Fuego | 13 734 / 13 731 | 138×152 / 145×146 |
| `mano_izquierdo_gota.png` · `mano_derecha_gota.png` | antebrazos + manos | `parte_antebrazo_{izq,der}` | como Fuego | 30 547 / 30 544 | 212×199 / 216×197 |
| `pierna_izquierdo_gota.png` · `pierna_derecha_gota.png` | piernas enteras | `parte_pierna_{izq,der}` | como Fuego | 21 390 / 21 389 | 154×186 / 157×182 |

**¿Se pueden compartir las extremidades entre formas?** Las piernas y la mano izquierda son un recoloreado exacto del fuego
(error ≤ 4 de 255 al multiplicar la versión gris por el color de la forma), así que bastaría una textura blanca teñida con
`Image.color`. Los brazos (los de la Rueda difieren en 112 a 182 px, los de la Gota llevan brillos) y la mano derecha de la
Gota no lo son. El ahorro sería de unos 1,5 MB a costa de tocar el generador; no se recomienda: se conservan los PNG por forma
de C.4.

## Correspondencia con el rig

| Nodo del prefab (C.2) | Hoy | Con esta entrega |
|---|---|---|
| `Lienzo/Cuerpo` (Image) | el sprite entero `_reposo` | se apaga solo: `BuildRigsFinal` («sprites») lo apaga cuando `Torso`, `BrazoIzq/Der` y `PiernaIzq/Der` tienen sprite, y con siete piezas los tienen |
| `Tronco/Torso` | apagado, rect [250, 20, 780, 812] | `torso_*` (la llama, el disco o la gota **más la pantaloneta**; §7.6 lo llamaba «la llama y el vientre de colores») |
| `Tronco/BrazoX` | apagado | `brazo_*` = húmero |
| `BrazoX/CodoX/AntebrazoX` | apagado | `mano_*` = antebrazo con la mano |
| `Cuerpo/PiernaX` | apagado, rect de muslo (y 795 → 1002) | `pie_*`/`pierna_*` = **pierna entera con el pie** |
| `PiernaX/RodillaX/AntepiernaX` | apagado | **sin pieza**: queda vacío (pivote sin imagen) |
| `Tronco/Ojos`, `Tronco/Boca` | apagados | **sin pieza**: la entrega no trae cara |
| `Cuello`, `Cabeza` | no existen (correcto) | no hacen falta |

- **Faltan:** las dos antepiernas (o, dicho de otro modo, los dos muslos: la pierna no está partida), `ojos_neutra`,
  `boca_0`, `ojos_parpadeo_{medio,cerrado}`, las otras cinco expresiones (§7.3) y las bocas del habla. También faltan los tres
  `_reposo` nuevos, que se pueden **componer** a partir de las piezas y la cara.
- **No sobra nada.** Las piezas se reparten 1:1 entre los nodos de C.2.
- **Orden de dibujo, comprobado en la hoja:** piernas detrás del torso (su tapón de arriba queda bajo la pantaloneta), brazos
  **delante** del cuerpo (su tapón del hombro está dibujado para verse sobre la llama) y la mano delante del brazo en el
  codo. Es el `orden_tronco` vigente de Algoritm (`Torso`, `Ojos`, `Boca`, `BrazoIzq`, `BrazoDer`) y el de la jerarquía de
  C.2 (el antebrazo es hijo del codo): no hay que cambiarlo, ni crear anclas de antebrazo (INC-133 es solo de la familia).
- **Segmentado:** `IsSegmented` mira la `Image` de `AntepiernaIzq`; con siete piezas seguirá en «no». A Algoritm le da
  igual: no tiene `Kneel` ni `Sleep`, que son los únicos clips que cambian con eso.

## Lado

`derecho`/`derecha` es la pieza de la **derecha de la pantalla** en las 21: `mano_derecho` en x 988 → 1271 y `mano_izquierdo`
en x 19 → 296; `brazo_derecho` en 859 → 1049; `pie_derecho` en 635 → 841. **No** es el lado del personaje (que, de frente,
caería a la izquierda de la pantalla). Coincide con la convención del rig (`BrazoIzq` es el de la izquierda de la pantalla,
x 0,285 del lienzo en el prefab), así que `clasifica()` y la posición dicen lo mismo y la herramienta no avisará. Como en
`preparar_arte_final.py`, el lado se decide por la posición, y la mezcla `derecho`/`derecha` e `izquierdo` en una misma forma
(Gota) no importa.

## Cara

- **No viene, ni horneada ni en capa.** Fuego, Rueda y Gota tienen el interior del cuerpo limpio: ningún blanco de ojo
  (solo los brillos de la Gota, 9 227 px casi blancos en manchas alargadas junto al contorno) y ningún oscuro fuera de las
  lenguas de la llama (y 300 a 450), un arco de anillo de la Rueda (y 650 a 760) y la cinturilla (y 900 a 980).
- **Consecuencia:** no hay nada que `separa()` deba separar, y tampoco cabe el reparto «horneada e inseparable». Las capas
  llegan aparte o no hay cara. **No hay ojos cerrados**, así que tampoco parpadeo (C.7).
- **Dónde va:** con la opción A de abajo, las cajas actuales de `Ojos` ([370, 380, 655, 505] del rig) y `Boca`
  ([410, 505, 610, 595]) caen dentro del cuerpo en las tres formas —en el núcleo amarillo del fuego, en el centro del disco y
  en el vientre de la gota—, así que los nodos que ya existen sirven para la neutra. Lo que fije el dibujo de la cara se
  ajusta en `rig_articulaciones.json`, una caja por forma: el cuerpo sube y baja de una forma a otra.
- **Para pedir** (en el mismo lienzo de 1300×1500, registradas sobre el torso de cada forma, como las expresiones de la
  familia): `ojos_neutra`, `boca_0` y `ojos_parpadeo_cerrado` por forma como mínimo; las otras cinco emociones y las bocas
  A, E y U cuando se pueda. Como los ojos, la boca y el contorno se dibujan iguales en las tres formas, puede bastar **un
  juego** y se coloca por forma.
- **Mientras tanto** (si Santiago lo pide): extraer los ojos y la boca del `char_algoritm_n1_fuego_reposo.png` actual. Se
  separan bien —ojos crema con iris y la línea de la sonrisa sobre el núcleo plano `#FFE093`—, pero tienen contorno café
  `#3B1205` y otra mano: se verían de otro dibujo.

## Articulaciones y pivotes (estimados)

Medidos sobre el alfa como en la familia: centro del extremo redondo de cada cápsula (radio = la mitad del ancho máximo en el
tercio de ese extremo); codo = punto medio entre el tapón de abajo del brazo y el de arriba de la mano, y «holgura» la
distancia entre los dos. Valen para las tres formas (las extremidades coinciden). Columna «rig»: opción A de «Escala».

| Articulación | Entrega | Rig (opción A) | Notas |
|---|---|---|---|
| Hombro izquierdo | (385, 797) | (323, 595) | tapón de arriba del brazo, r ≈ 29 px, **sobre** el cuerpo |
| Hombro derecho | (885, 800) | (705, 597) | ídem |
| Codo izquierdo · holgura | (260, 939) · 1,7 px | (228, 703) | brazo r ≈ 31, mano r ≈ 39 |
| Codo derecho · holgura | (1021, 935) · 7,1 px | (808, 699) | |
| Cadera izquierda | (503, 1115) | (413, 837) | centro del tapón de arriba de la pierna; el borde de arriba está en y = 1085 |
| Cadera derecha | (662, 1125) | (534, 845) | el borde de arriba en y = 1095 |
| Rodilla | **no hay** | — | el palo es recto de la pantaloneta (y 1146 a 1160) al pie (y ≈ 1245) |
| Tronco (cintura) | (628, 1005) | (509, 753) | centro de la cinturilla; hoy el `Tronco` gira en (512, 700) |

- **Solapes con extremos redondos:** brazo sobre torso 4 261 a 5 620 px (el tapón del hombro), brazo con mano 2 973 a
  3 268 px (el codo), pierna bajo la pantaloneta 2 762 a 3 109 px, mano con torso 0 y pierna con pierna 0. Las
  articulaciones no abren huecos al girar: la entrega sigue la regla de `preparar_arte_final.py` («Los extremos redondos se
  SOLAPAN»).
- **El hombro es el centro del tapón**, no el borde del torso: el tapón del brazo se ve encima de la llama, así que no se
  aplica `hombro.py` (que lleva el pivote al borde del torso porque en la familia el húmero va detrás).
- **Asimetría del dibujo:** las lenguas de la llama se inclinan a la derecha y la pantaloneta y las piernas a la izquierda.
  El eje de las piernas está en x ≈ 583 y el del cuerpo en x ≈ 632 (filas 700 a 950 del torso): 49 px de la entrega, 37 del
  rig. No impide nada; se ve al espejar (`Mirrored`) y en `Spin`, que gira `Cuerpo` en torno al centro del lienzo.
- **Pose de reposo:** brazos en A a unos 40° de la vertical (más abiertos que los del sprite de hoy) con los codos casi rectos, y piernas
  rectas y separadas con los pies hacia fuera.

## Siete o nueve piezas

Otra sesión local tiene sin subir un corte de Algoritm en **nueve** piezas (27 PNG en `Algoritm/Frontal/`), que aquí no se
ve. Lo que esta entrega dice de cada lectura:

- **Si las nueve salen del `_reposo` actual** (torso, dos húmeros, dos antebrazos, dos muslos y dos antepiernas, o siete de
  cuerpo más ojos y boca): **quedan superadas**. El diseño de la entrega es otro (cuerpo, pantaloneta, extremidades,
  contorno), y ninguna pieza vieja casa con la nueva. Lo único aprovechable de ese corte es la cara, como solución temporal.
- **Si salen de esta entrega partiendo la pierna en la rodilla**: hay que inventar dos tapones con contorno negro donde el
  artista no dibujó ninguno. La pierna visible es corta —unos 100 px de palo de la entrega (76 del rig) entre la pantaloneta y
  el pie— y los clips de Algoritm doblan la rodilla como mucho 15,6° (`animo`), 14° (`celebrar`) y 8° (`girar`), así que
  apenas se notaría y la costura sí podría verse.
- **Recomendación: siete piezas.** La pierna entera en `PiernaX`, con `RodillaX` y `AntepiernaX` vacíos. Las curvas de
  rodilla siguen en los clips y no mueven nada; `Cuerpo` se apaga igual (no mira las antepiernas) y no hay que tocar el
  prefab ni los clips. Si Santiago quiere rodilla, que la dibuje el artista: es la única forma de que el tapón tenga su trazo.

## Escala

- **Hoy:** el `_reposo` (768×768) llena el `Lienzo` de 1024 y la figura ocupa y 21 → 1003 (**982 px**, más que los 870 de la
  familia) y x 11 → 1013. En las narrativas `NarrativeProp.Size` es el alto del lienzo, así que la figura mide hoy en pantalla
  `Size × 982/1024`. Algoritm tiene `Size` 0,12 frente a 0,34 de Papá en `N1_AparicionGuia` y, en la mayoría de las 16
  narrativas, de 0,35 a 0,40 veces el de Papá (§7.6: «entre un tercio y dos quintos»).
- **Opción A (recomendada): conservar el alto de hoy y una sola transformación para las tres formas.** Escala
  `s = 982/1289 = 0,76183` (el Fuego, la forma más alta, ocupa los mismos 982 px que el sprite de hoy), coronilla del Fuego en
  y = 21, eje del cuerpo (x = 632,5) en x = 512: `x' = 0,76183·x + 30,1`, `y' = 0,76183·y − 12,5`. Las tres formas quedan con
  los pies en y = 1003 y las extremidades idénticas; la Rueda mide 818 px y la Gota 878, que es lo que dibuja el artista (el
  disco y la gota son más bajos que la llama). **Ningún `Size` de las 16 narrativas cambia**, y la figura cabe en el lienzo
  (x 45 → 998).
- **Opción B: la regla de la familia** (`preparar_arte_final.py`: 870 px, coronilla en y = 77, pies en y = 947). Escala
  0,67494; el Fuego se vería un **11 % más pequeño** que hoy si no se suben los `Size` (16 assets).
- **Lo que no hay que hacer:** normalizar cada forma a su alto (870 o 982 px por separado). La Rueda tendría brazos y piernas
  un 20 % más grandes que el Fuego y la Gota un 12 %, y es el mismo personaje (CN-03); además, `coreografia.py` lee la geometría
  de Algoritm de `algoritm_fuego` y la aplica a las tres formas.

## Memoria (RNF-06)

| | Opción A | Opción B |
|---|---:|---:|
| 7 partes, Fuego | 2,37 MB | 1,86 MB |
| 7 partes, Rueda | 2,30 MB | 1,81 MB |
| 7 partes, Gota | 2,21 MB | 1,74 MB |
| **21 partes** | **6,89 MB** | **5,41 MB** |
| Cara mínima (`ojos_neutra`, `ojos_parpadeo_cerrado`, `boca_0`) × 3 formas, con las cajas de hoy | 1,07 MB | 0,84 MB |
| Cara completa (8 ojos + 8 bocas) × 3 formas | 5,16 MB | 4,05 MB |
| Los tres `_reposo` recompuestos a 768² | 0 (sustituyen a los de hoy, 7,08 MB) | 0 |

- Cálculo: ancho × alto × 4 bytes de cada pieza recortada y escalada, como en el inventario de la familia (`Art/` se importa
  sin comprimir y sin mipmaps). La pieza más cara es el torso (1,48 a 1,64 MB en la opción A).
- **Margen:** 21 MB (rc2, 479,0 MB). El perfil de la familia se lleva 8,7 MB, y 9,9 MB con los ojos cerrados de frente, así
  que quedan unos **11 MB**. Algoritm con la opción A y la cara mínima pide **7,96 MB** y deja unos **3 MB**; con la cara
  completa (12,05 MB) **no cabe**. Con la opción B: 6,25 MB (cara mínima) o 9,46 MB (completa).
- **Si hace falta sitio:** los `_reposo` solo se usan pequeños (el botón de ayuda de las cinco escenas jugables y el campo
  `Art` de las 16 narrativas); a 512² en vez de 768² liberan 3,9 MB. Hay que medirlo en un ejecutable antes de dar nada por
  bueno.

## Decisiones pendientes (para Santiago)

1. **El nuevo diseño de Algoritm** (llama, disco de madera y gota, con pantaloneta de cuadros por forma, extremidades del
   color del material, contorno negro y degradados) sustituye el núcleo de identidad de §7.6 y la regla de color plano de
   §2.2. Pide un INC nuevo que corrija §7.6 (tabla del núcleo y de las tres formas: «rueda» y «gota» dejan de ser nombres sin
   silueta), §2.2 o una excepción para Algoritm, `Inventario.md` y, con su autorización, el guion §1.1.1. El riesgo del Nivel
   2 cambia: el disco (`#BDA483`, `#9C715C`) parece la sección de un tronco y queda cerca del acento interactivo `#C79A5E`;
   sin cara, se lee como un prop.
2. **La cara.** Pedirla al artista (mínimo neutra, boca cerrada y ojos cerrados) o aceptar, provisionalmente, la cara del
   sprite de hoy.
3. **Siete o nueve piezas.** Recomendación: siete (la pierna entera); la rodilla, solo si la dibuja el artista.
4. **Escala:** opción A (el alto de hoy, sin tocar los `Size`) u opción B (870 px como la familia, y reajustar 16 assets).
5. **Los `_reposo`:** recomponerlos desde las piezas y la cara (mismo nombre, `.meta` y GUID, 768²) para que el botón de
   ayuda y los `Art` de las narrativas enseñen el diseño nuevo; o pedirlos al artista.

## Recomendaciones para la ingesta

**Una herramienta nueva, `preparar_algoritm.py`, y no una rama dentro de `preparar_arte_final.py`.** Lo que comparten es poco
(leer el lienzo, limpiar el alfa, medir tapones, escribir con el nombre de C.4), y lo distinto toca cada paso de la de la
familia: sin cabeza ni cuello, sin `hombro.py`, pierna entera, una transformación común a tres carpetas, cara opcional, el
`_reposo` que hay que componer, y `--autoprueba` con su propia entrega sintética. Importar sus funciones (`limpia`, `junta`,
la medida de extremos) en lugar de copiarlas.

- **Entrada:** `preparar_algoritm.py <carpeta_algoritm> [--escala actual|familia] [--aplicar]`, con `<carpeta_algoritm>` =
  `entregas/2026-10-09/Algoritm`, que contiene `Fuego/`, `Rueda/` y `Gota/` (o `fuego`, `madera`, `agua`; sin tildes ni
  mayúsculas). Exige las tres formas, siete piezas por forma y un lienzo común a las 21; si falta una forma, avisa y sigue
  con las demás.
- **Nombres:** ASCII en minúsculas (NFKD), partido por `_` y espacios. `torso`/`cuerpo` → torso; `brazo`/`humero` → húmero;
  `mano`/`antebrazo` → antebrazo; `pie`, `pies` **y `pierna`** → **pierna entera** (`parte_pierna_*`, nodo `PiernaX`); `muslo`
  y `antepierna`, si algún día llegan, → muslo y antepierna. Se ignoran `algoritm`, `fuego`, `rueda`, `gota`. El lado lo
  decide la posición; `derecho` y `derecha` se aceptan igual.
- **Comprobación entre formas:** que las extremidades de las tres coincidan (XOR del alfa ≈ 0); si no, avisar, porque los
  clips y `coreografia.py` suponen una sola geometría.
- **Normalización:** una sola `s` y una sola `(tx, ty)` para las tres formas, sacadas del Fuego. Por defecto la opción A
  (`s = 982 / alto del Fuego`, coronilla en y = 21, eje del cuerpo en x = 512; el eje se mide como el centro de las filas del
  torso entre el 45 % y el 70 % de su alto, no del recuadro, que está sesgado por las lenguas). `--escala familia`, la B.
  Recorte al recuadro, LANCZOS, y guardar `registro` (`escala`, `tx`, `ty`, `lienzo`) en `arte_final.json` bajo
  `algoritm_<forma>`, para colocar la cara después con `preparar_expresion.py --registrada`.
- **Medidas** (las de la tabla de arriba, ±2 px): hombro = centro del tapón de arriba del húmero; codo = punto medio de los
  tapones que se solapan, con su holgura; cadera = centro del tapón de arriba de la pierna; pivote de `Tronco` = centro de la
  cinturilla. Con pierna entera, la rodilla se escribe en el punto medio del palo visible (el nodo existe y los clips la
  animan) y `AntepiernaX` sin sprite ni rect nuevo.
- **Salidas:** las partes en `Assets/Game/Art/Characters/Algoritm/Frontal/char_algoritm_<forma>_parte_{torso,brazo_*,antebrazo_*,pierna_*}.png`
  (7 por forma; las que ya existan se sustituyen donde estén, mismo `.meta`); los nodos de las tres entradas `algoritm_*` de
  `rig_articulaciones.json` con el rect y el punto medidos (`punto` de `BrazoX` = hombro, de `CodoX` = codo, de `PiernaX` =
  cadera, de `Tronco` = cintura), conservando `Ojos` y `Boca` y el `orden_tronco`; después `coreografia.py` y
  `pose_preview.py`. Sin `--aplicar`, informe y un composite por forma.
- **Los `_reposo`:** `--reposo` compone piezas y cara a 768² con la misma transformación (×0,75 del lienzo del rig) y los
  escribe sobre `char_algoritm_n{1,2,3}_<forma>_reposo.png`. Lo referencian 3 prefabs, 5 escenas y 16 assets narrativos: el
  nombre y el GUID no pueden cambiar. Mientras no haya cara, **no** recomponerlos.
- **La cara, cuando llegue:** `preparar_expresion.py --registrada algoritm_<forma>` con el `registro` de arriba, un juego por
  forma (o el mismo juego colocado en las tres). Sin `cara_base`, sin nariz y con el `separa()` de frente, que sirve si los
  ojos y la boca llegan en una sola capa.
- **Orden en el Editor:** el de C.5, pero sin «nodos» (ya están) ni «orden» (ya vale): `sprites` y después `clips`, y luego
  `estado`. Comprobar que `Cuerpo` queda apagado en los tres prefabs y que `git diff` no quita ningún `--- !u!`.
- **`--autoprueba`:** una entrega sintética de tres formas con un torso distinto por forma y las mismas extremidades, una con
  `pie_` y otra con `pierna_`, un `derecha` y una mota suelta. Debe recuperar los pivotes (≤ 1 px), mandar las dos palabras a
  `PiernaX` y avisar de que falta la cara.

## Qué pedir al artista

1. **La cara**, en el lienzo de 1300×1500 y registrada sobre el torso de cada forma, en capas separadas: `ojos_neutra`,
   `boca_0` y `ojos_cerrados` como mínimo; después alegría, sorpresa, preocupación, concentración y sueño, y las bocas A, E y U
   (§7.3, sin tristeza ni enfado). Si es la misma para las tres formas, basta un juego.
2. Solo si Santiago quiere rodilla: la pierna partida en muslo y pierna con pie, con los tapones redondos solapados.
3. Opcional: los tres `_reposo` como figura entera, si no se componen en la herramienta.

---
Medido el 09/10/2026 con Pillow sobre los PNG de `545127e` (LFS). No se compiló ni se abrió Unity, y no se tocó `herramientas/`.
