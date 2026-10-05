# Inventario de sprites — `Assets/Game/Art/`

Listado de los archivos que van en cada carpeta, con la tarea o el asset que los produce.

**Este archivo no decide nada: es un índice.** Dos fuentes, y cuando se contradicen manda la
primera:

1. **El tablero de arte** (`Tareas.xlsx`, hoja `Inventario`): 163 piezas en tareas `S01..S16c`,
   con el nombre de archivo definitivo de cada una. Es lo más específico que existe y lo único
   que nombra los `.anim`.
2. **Los `plan.md` de los cuatro slices** (assets `A1..A10`, `B1..B10`, `C1..C10`, `D1..D3`) y
   `claudeDocs/Direccion_de_Arte.md`, para las piezas que el tablero todavía no programa.

Los nombres de carpeta van **en inglés**; los de archivo siguen la nomenclatura radicada, que es
en español.

**Convención (§15.4):** `[categoria]_[sujeto]_[variante]_[estado].png` · prefijos `char_`,
`prop_`, `env_`, `ui_`, `fx_`, más `ref_` que introduce `S01` · niveles `n1` (La Oscuridad),
`n2` (La Rueda), `n3` (El Río) · sin tildes, sin espacios, sin mayúsculas.

**Convención de clips, fijada por el tablero** (§15.4 la recoge):
personajes `<sujeto>_anim_<accion>.anim` · todo lo demás `<sujeto>_<accion>.anim`.

**Cada pieza llega como un archivo suelto y se importa en `Sprite Mode: Single`** (§15.2). Siguen
en `Multiple` doce PNG —`Environments/Narrative/env_enlace_n2` y once de `Props/Wheel/`—: los
assets los referencian por su sub-sprite y migrarlos rompería esas referencias (INC-128).

Leyenda: **✓** en disco · **◐** provisional en disco (sustituir conservando el nombre) · **○** pendiente · **—** no se genera: lo cubre otro archivo o el motor. Estados del tablero: `·` pendiente · `G` generado ·
`R` recortado · `I` importado · `A` animado · `✔` aprobado. Hoy **todo está en `·`**.

---

## `Characters/`

**Cómo se animan (24/09/2026, INC-53).** Por **recorte**, como fija §13.1, y no con el rig de
2D Animation: las escenas son uGUI en un Canvas overlay y `SpriteSkin` no deforma una `Image`. Cada
miembro de la familia está cortado en cinco partes —`torso`, `brazo_izq`, `brazo_der`,
`pierna_izq`, `pierna_der`, con izquierda y derecha **de pantalla**; con el arte final serán diez
y una cara, más abajo—. Son `Image` hijas con el
pivote en la articulación, sobre un lienzo de 1024 × 1024, el de los sprites base: la figura
ocupa y 77..947. Un `Animator` las gira y desplaza con un clip por acción; el componente es
`CharacterRig` (`Game.Scaffolding`) y los prefabs están en `Assets/Game/Prefabs/Characters/`. Las
partes salen de los sprites base entregados el 24/09/2026 (`…/assets a postproduccion/familia/`),
limpios de restos de croma verde en el pelo. El 01/10/2026 se limpió el halo de croma de siete PNG
—los tres `char_algoritm_n?_*_reposo` y los cuatro retratos `char_*_retrato_neutra`—: solo se tocaron píxeles
semitransparentes (α entre 1 y 199, a 3 o 4 px del borde), con el alfa idéntico al del original. **Quedan unas
12 o 13 motas verdes opacas (α ≥ 200) en las puntas del pelo** de los retratos de Niña y Papá y de Algoritm,
visibles a 1080p; limpiarlas exige retocar píxeles opacos a mano: **pendiente del carril de arte**. **Para sustituirlas por arte definitivo, se reemplaza
cada `.png` conservando el nombre**; si cambia la silueta, hay que rehacer el prefab, porque el
tamaño y el pivote de cada parte viven en él.

Retratos: solo existe `neutra`, un recorte de la cabeza de 320², que es el que usa el cuadro de
diálogo. **Con el arte actual es la única cara del juego**: la emoción la lleva el cuerpo con las
acciones del rig (§7.3). **No hay tristeza ni enfado**: tras un intento sin éxito el personaje hace
«ánimo» (`Encourage`, CP-02). **No existen saltar, caer, aterrizar ni derrota** (CT-06, RNF-02, CP-02).

**Arte final: partes articuladas y cara (decisión de Santiago, 05/10/2026, INC-131).** Los
personajes finales —Papá, Mamá, Niña y Niño, en vista frontal— llegan con más capas. Se **conservan
los nombres actuales** (`char_<x>_parte_brazo_*` pasa a ser el húmero y `char_<x>_parte_pierna_*`
el muslo, con el mismo GUID) y se **añaden** los siguientes. Hasta que lleguen, los nodos del prefab
existen con la `Image` apagada y el arte de hoy se ve igual. Esto sustituye las carpetas `Front/` y
los nombres `frente_` de `Plan-Personajes-Finales.md` §4.2. El estado de cada archivo es `○`
(pendiente de entrega):

| Qué | Archivos (`char_<x>_`, con `<x>` = `papa`, `mama`, `nina`, `nino`) |
|---|---|
| ○ Cabeza (separada del torso) | `parte_cabeza` |
| ○ Antebrazo y antepierna (el segundo tramo) | `parte_antebrazo_izq`, `parte_antebrazo_der`, `parte_antepierna_izq`, `parte_antepierna_der` |
| ○ Ojos, uno por expresión | `ojos_neutra`, `ojos_alegria`, `ojos_sorpresa`, `ojos_preocupacion`, `ojos_concentracion`, `ojos_sueno` |
| ○ Ojos al parpadear | `ojos_parpadeo_medio`, `ojos_parpadeo_cerrado` |
| ○ Boca al hablar | `boca_0` (cerrada, también el reposo de `neutra` y `sueno`), `boca_a`, `boca_e`, `boca_u` |
| ○ Boca de reposo por expresión | `boca_alegria`, `boca_sorpresa`, `boca_preocupacion`, `boca_concentracion` |

Las seis expresiones son las de `Direccion_de_Arte.md` §7.3 y **ninguna es de tristeza, enfado ni
derrota** (CP-02). Todo vive en la carpeta del personaje, sin subcarpeta. La tabla de dónde va cada
articulación es `claudeDocs/tasks/Personajes/herramientas/rig_articulaciones.json` (provisional), y
el arte entra con los modos «sprites» y «clips» de `BuildRigsFinal.cs.txt`
(`claudeDocs/tasks/Personajes/Personajes-Resultados.md`, Anexo C). El generador crea además
`char_<x>_cara.asset`, un `CharacterFaceSet` que no es una imagen.

**Clips (21 por miembro de la familia).** `char_<x>_anim_<accion>.anim`, uno por estado del
`Animator`, cuyo nombre es el de `ActorAction`: `idle`, `caminar`, `correr`, `hablar`, `golpear`,
`martillar`, `soplar`, `recoger`, `arrodillarse`, `cargar`, `empujar`, `senalar`, `observar`,
`celebrar`, `animo`, `abrazar`, `sorpresa`, `dormir`, `oculto`, `aparicion` y `apagado`. Los genera
un constructor efímero con un tempo por personaje: Papá, amplio y lento; el Niño, rápido (§7.4). El
controlador `char_<x>.controller` está al lado.

### `Characters/Algoritm/` — el guía, en los tres niveles (CN-03)

| Archivo | Estado |
|---|---|
| ✓ `char_algoritm_n1_fuego_reposo.png` | Arte entregado (24/09/2026), recortado a cuadrado de 768². Es una llama con cara, brazos y piernas de palo, manos y franja de colores, tal como la describe `Direccion_de_Arte.md` §7.6 (INC-52, cerrado) |
| ◐ `char_algoritm_n2_rueda_reposo.png` | **Provisional**: el fuego recoloreado en madera. El definitivo entra sustituyendo el archivo |
| ◐ `char_algoritm_n3_gota_reposo.png` | **Provisional**: el fuego recoloreado en agua. Ídem |
| ○ `_girando`, `_atenuado`, `char_algoritm_n*_retrato_*` | `S15`/`S03b`. Hoy el retrato del guía **es** el sprite de su forma |

**Arte actual:** una sola `Image` por forma y ningún recorte, para que sustituir el archivo baste. Los tres prefabs
`Algoritm_Fuego`, `Algoritm_Rueda` y `Algoritm_Gota` comparten `Animations/char_algoritm.controller`
y sus clips `char_algoritm_anim_{flotar,hablar,senalar,girar,celebrar,animo,oculto,aparicion,apagado}`.
El mismo sprite va dentro del botón de ayuda circular de las cinco mecánicas.

**Arte final: recorte en partes (decisión de Santiago, 05/10/2026, INC-131).** Cada forma se
entrega además cortada, con el prefijo `char_algoritm_<fuego|rueda|gota>_` —uno por forma, porque
cada una va recoloreada (INC-52)—. Brazos, piernas, codos y rodillas en dos tramos, ojos y boca
sobre el cuerpo y **sin cuello ni cabeza**. Estado `○`:

| Qué | Archivos (`char_algoritm_<forma>_`) |
|---|---|
| ○ Torso (la llama y el vientre de colores, sin extremidades) | `parte_torso` |
| ○ Brazos en dos tramos | `parte_brazo_izq`, `parte_brazo_der` (húmero), `parte_antebrazo_izq`, `parte_antebrazo_der` |
| ○ Piernas en dos tramos | `parte_pierna_izq`, `parte_pierna_der` (muslo), `parte_antepierna_izq`, `parte_antepierna_der` |
| ○ Ojos y su parpadeo | `ojos_neutra`, `ojos_alegria`, `ojos_sorpresa`, `ojos_preocupacion`, `ojos_concentracion`, `ojos_sueno`, `ojos_parpadeo_medio`, `ojos_parpadeo_cerrado` |
| ○ Boca | `boca_0`, `boca_a`, `boca_e`, `boca_u`, `boca_alegria`, `boca_sorpresa`, `boca_preocupacion`, `boca_concentracion` |

Hasta que lleguen, el prefab lleva los nodos con las capas apagadas y se sigue viendo el
`_reposo`; cuando torso, brazos y piernas están puestos, el motor apaga la `Image` de `Cuerpo`. El
`_reposo` se conserva mientras no se decida otra cosa: es hoy el retrato del guía y lo que va
dentro del botón de ayuda.

### `Characters/Father/` · `Mother/` · `Girl/` · `Boy/`

| Carpeta | Partes (✓) | Retrato (✓) | Prefab |
|---|---|---|---|
| `Father/` — Papá, jugable en el N1 | `char_papa_parte_*.png` | `char_papa_retrato_neutra.png` | `Papa` |
| `Mother/` — Mamá, jugable en el N3 | `char_mama_parte_*.png` | `char_mama_retrato_neutra.png` | `Mama` |
| `Girl/` — la Niña, jugable en el N2 | `char_nina_parte_*.png` | `char_nina_retrato_neutra.png` | `Nina` (habla también como «NIÑOS») |
| `Boy/` — el Niño, acompaña | `char_nino_parte_*.png` | `char_nino_retrato_neutra.png` | `Nino` (habla también como «NIÑOS») |

`Mother/char_mama_cenital.png` (R07, provisional) **ya no se ve**: en el río, `Personaje_Mama`
lleva dentro el rig frontal de Mamá y su `Image` raíz queda de reserva. Los clips
`char_mama_anim_caminar_{norte,sur,este,oeste}` del tablero no existen: se camina con uno solo, que
se voltea a izquierda o derecha. `char_*_base_apose.png` no entra al proyecto: las partes lo
recomponen.

---

## `Environments/`

**Cómo entran (16/09/2026).** `Game.EditorTools/ArtImportRules.cs` fuerza en todo
`Assets/Game/Art/`: **sin comprimir** y `maxTextureSize` 4096. No es un gusto: comprimida, la
ilustración plana enseña la rejilla de bloques de 4×4 —se ve en la pared de la cueva, y el Nivel 1
la multiplica por la capa de oscuridad, que amplifica el error— y corre los colores.
`ArtImport_RNF23_…` lo vigila, y la memoria se mide sobre el ejecutable (RNF-05). La única excepción son los cuadros de
`Props/Fire/Animations/`, a 1024 px (INC-130, más abajo).
Un archivo nuevo entra ya bien: **sustituir la imagen basta**, la escala la calcula
`IllustrationFraming` con el tamaño real del sprite.

**Lo que hay en disco hoy** (entrega de entornos finales, acta D06): los de pantalla a 1920×1080 y
las panorámicas a 3840×1080, en la misma composición que los provisionales —comprobado: el
contenido cae en las mismas fracciones del lienzo, así que **ningún encuadre de
`Camara_Narrativa_N1/N2` cambia de valor**—. Los cuatro que llegaron con el prefijo viejo
`entorno_` se renombraron a `env_` **desde el motor**, con el GUID intacto y sin tocar escenas
(INC-126, tarjeta `D10-4`, que continúa la `D06-2`): `env_n1_apertura`, `env_n1_cueva_2x`,
`env_n1_cueva_cenital` y `env_n2_laberinto`. Tres llegaron con nombre
libre y en `Sprite Mode: Multiple`, que para un fondo entero no aporta y hace que
`LoadAssetAtPath<Sprite>` devuelva nulo. Dos se resolvieron desde el motor (R04, 16/09/2026):
`River/rio_normal.png` es ahora `River/env_n3_rio.png` y `River/civilización_final.png` es
`Narrative/env_final_fogatas.png`, los dos en `Single` y con su GUID intacto. El tercero,
`Wheel/civilización_noche.png`, lo sustituyó `Narrative/env_enlace_n2.png` con su mismo `.meta`
(`2cbe287`): conserva el GUID **y** el modo `Multiple`, porque `N3_PuenteII_Horizonte` lo
referencia por su sub-sprite (INC-128). **Ojo:** el importador de fábrica trae `Multiple`; un
sprite nuevo hay que pasarlo a `Single` desde el motor (`TextureImporter.spriteImportMode`).

### `Environments/Fire/`

| Archivo | Origen |
|---|---|
| ✓ `env_n1_apertura.png` | Entregado en D06 como `entorno_n1_apertura` (3840×1080, lienzo duplicado). La apertura (`N1_Apertura`) y, con la luz del amanecer de `NarrativeLight`, el puente I (`N2_PuenteI`) |
| ✓ `env_n1_cueva_2x.png` | Entregado en D06 como `entorno_n1_cueva_2x` (3840×1080, lienzo duplicado). El interior de la cueva de las narrativas (`N1_AparicionGuia`, `N1_Hallazgo`, `N1_NacimientoDelFuego`) y el refugio de la 2.5 y del arranque de `N3_PuenteII` |
| ✓ `env_n1_cueva_cenital.png` | `A6` — entregado en D06 como `entorno_n1_cueva_cenital` (1920×1080). La cueva vista desde arriba, donde se juega el Nivel 1 (`Level1_Cave`), y la tarjeta del nivel en el menú de niveles |
| — ~~`env_n1_cueva_luz1.png` … `env_n1_cueva_luz4.png`~~ | `A6` — no se generan: la luz del nivel la pone la capa del shader `fx_oscuridad` (punto abierto 4) |

### `Environments/Wheel/`

| Archivo | Origen |
|---|---|
| ✓ `env_n2_bosque_claro.png` — 3840×1080, definitivo (acta D06), `Single`. El bosque de la recolección y el claro del taller y del refugio en un solo lienzo duplicado en espejo (`Camara_Narrativa_N2.md` §2), pintado con la luz de la tarde, sin tinte: el amanecer, el atardecer y la noche los pone `NarrativeLight`. Lo usan las narrativas del N2, `Level2_Forest` y `Level2_Workshop` | `B1` (Slice 2) |
| ○ `env_n2_taller.png` — **no hace falta mientras el taller sea el claro este del entorno duplicado**: `Level2_Workshop` usa `env_n2_bosque_claro.png` con el encuadre de `Camara_Narrativa_N2.md` §5.6 (W09, 12/09/2026) | `B4` |
| ✓ `env_n2_laberinto.png` — el tablero cenital del laberinto (1920×1080), entregado en D06 como `entorno_n2_laberinto`; lo usan `Level2_Maze` y la tarjeta del nivel en el menú de niveles. El tablero se llamaba `env_n2_tablero` en el plan | `B8` |
| ○ `env_n2_refugio_noche.png` | `S16b` — refugio con fuego encendido, 21:9 |

**`Environments/Wheel/Animations/`** · ○ `env_n2_nubes.anim` (`S12`, deriva lenta)

### `Environments/River/`

| Archivo | Origen |
|---|---|
| ✓ `env_n3_rio.png` | Entregado en D06 como `rio_normal.png` (1920×1080, vista lateral con la cascada). **Todo el Nivel 3 se juega y se narra sobre él**, sin duplicar: la recolección a ×1,4 —la orilla con el río y el pie de la cascada— y el ensamblaje a ×1,6 (INC-118). Lo aplicado vive en `N3_RiverLevelConfig`, `N3_RaftAssemblyContent` y los `N3_*.asset`: el diseño de cámara del N3 ya no está en el equipo. La vista superior de `C1` y el `_lateral` de `S16c` quedan sin uso. |
| ○ `env_n3_espuma.png` | `S11a` — 4 frames |
| ✓ `env_n3_zona_disponible.png` · — ~~`env_n3_zona_inactiva.png`~~ | `C6` — anillo ámbar discontinuo dibujado por código (R08, 17/09/2026), aceptado como definitivo por Santiago el 30/09/2026 (acta D10 §5). Es `Zona_Construccion` en `Level3_River.unity`, con `sizeDelta` 255 desde la lectura B (×1,5, INC-118). No hay estado inactivo: la zona se ve igual desde el principio (RF-39). |

**`Environments/River/Animations/`**

| Archivo | Origen |
|---|---|
| ○ `env_n3_corriente.anim`, `env_n3_espuma.anim`, `env_n3_niebla.anim` | `S11a` |
| ○ `env_n3_zona_pulso.anim` | `S11b` |
| ○ `env_n3_libelulas.anim` | `S12` |

### `Environments/Narrative/` — escenas narrativas, no pertenecen a un nivel

| Archivo | Origen |
|---|---|
| ○ `env_apertura_llanura.png` | `S16a` — llanura helada, 21:9 |
| ○ `env_apertura_entrada_noche.png` | `S16a` — boca de la cueva de noche |
| ○ `env_puente1_amanecer.png` | `S16b` — boca de la cueva al amanecer |
| ○ `env_puente1_recoleccion.png` | `S16b` — claro de la recolección |
| ✓ `env_final_fogatas.png` | Entregado en D06 como `River/civilización_final.png` (1920×1080, la aldea al atardecer); renombrado y movido aquí desde el motor el 16/09/2026. Escena final (§9). |
| ✓ `env_enlace_n2.png` | El horizonte del puente II (`N3_PuenteII_Horizonte`), 1920×1080, sin la hoguera central pintada, que pasa a objetos (`d3a6cc9`). Sustituyó a `Wheel/civilización_noche.png` con su mismo `.meta` y sigue en `Multiple` (INC-128) |

---

## `Props/`

Contorno de 7–9 px en `#3A1E18` para lo interactivo y de 4 px en `#5C4038` para lo decorativo;
el color de acento del nivel está prohibido en el decorado (§9.2, §4.2).

### `Props/Fire/`

| Archivo | Origen |
|---|---|
| ✓ `prop_n1_monton_hojas_cenital.png` | `S07a` / `A7` — el montón de la cueva visto desde arriba (2000×2000, `Level1_Cave`). Un solo dibujo para los cuatro estados del tablero: el hilo de humo, el rayo de la chispa, la llama y el quemado desde el centro (`BurnReveal`) los pone el motor encima, así que `prop_n1_hojas_intacto`, `_chispas`, `_humeante` y `_encendido` no se generan |
| ✓ `prop_n1_monton_hojas.png` | El montón de frente (2000×2000) de las narrativas con fuego: `N1_NacimientoDelFuego`, `N2_PuenteI`, la 2.5, `N3_PuenteII`, `N3_PuenteII_Horizonte` y la escena final |
| ✓ `prop_n1_hoja.png` | La hoja suelta (2000×2000) que se arrastra al centro al reunir (`Level1_Cave`) y la del hallazgo (`N1_Hallazgo`) |
| ✓ `prop_n1_silex.png`, `prop_n1_pedernal.png` · — ~~`prop_n1_piedras_choque.png`~~ | `A8` (Slice 1) — las dos piedras del panel (2000×2000): sílex anguloso y pedernal redondeado (§9.3). El choque no tiene dibujo: lo cuentan el golpe del rig de Papá y el rayo de la chispa (§12.2) |

**`Props/Fire/Animations/`** — `S07a`

**Excepción de importación (INC-130, RNF-06).** Los cuadros de esta carpeta entran a **1024 px de lado
máximo** y siguen **sin comprimir** (`ArtImportRules`, vigilado por `ArtImport_RNF06_…`). A hasta 2144 × 2108 y sin
comprimir eran 67 PNG referenciados (134 entradas en el informe del build) y 533 MB, y el paquete llegó a
866,7 MB (826,6 MiB) frente al tope de 500 MB (RNF-06). Ya son un dibujo por clave, sin duplicados, así que no había
arreglo sin pérdida. En pantalla el fuego se ve a unos 650 px como mucho y el humo a menos de 300, de modo
que no se nota: el humo queda en 595 × 1024 y el fuego normal en 1024 × 1007, en memoria; el fuego
cenital (500 × 278) no cambia. El PPU se escala con la textura, así que el tamaño en el mundo es el mismo.
Con la excepción el build pesa 479,0 MB (rc2, `evidencias/build-rc2.md`). Una entrega nueva de cuadros entra sola
a 1024; los `.png` de la entrega no se reducen a mano.

| Archivo | Nota |
|---|---|
| ✓ `prop_n1_fuego_cenital.anim` + `.controller` · `Fuego cenital/` (21 PNG) | La llama de la cueva vista desde arriba (`Level1_Cave`, objeto `Fuego`): 5,633 s a 30 fps en bucle, animada a ochos, con una clave por dibujo distinto |
| ✓ `prop_n1_fuego_normal.anim` + `.controller` · `Fuego normal/` (13 PNG) | La llama de frente de las narrativas con fuego: 2,667 s a 30 fps en bucle, animada a seises. Los 34 cuadros de las dos llamas conservan el nombre de entrega (`fuego_*_nivel_1_NNNN.png`, `Single`, pivote centrado): los referencia la curva del `.anim` |
| — ~~`prop_n1_hojas.anim`~~ | El cruce entre estados lo hace el motor: el quemado de `BurnReveal` crece desde donde cae la llama |
| — ~~`prop_n1_piedras_choque.anim`, `prop_n1_piedras_flotacion.anim`~~ | Las piedras no se animan: las coloca el deslizante de cercanía y el golpe lo da el rig |
| ✓ `fx_n1_humo_nacer.anim` + `fx_n1_humo.anim` · `Smoke/` (33 PNG) | El humo de `S07a`: vive junto al fuego y no en `FX/Animations/`. Entrega de 284 cuadros a 30 fps, animada a ochos y nueves: 33 dibujos distintos, que conservan el nombre de entrega (`humo_nivel_1_NNNN.png`, `Single`). **Recortados** del lienzo de 2144 × 2108 a 1123 × 1933 (desde x 485, y 134): el mismo rectángulo en todos, así el dibujo no salta de un cuadro a otro, y la mitad de memoria. Si llega una entrega nueva con el lienzo entero, hay que recortarla igual o recalcular `Position` y `Size` de los ocho humos y el tamaño del `Humo` de `Level1_Cave`. `nacer` (0009–0059, 1,97 s) sube del hilo a la voluta y pasa a `fx_n1_humo` (0067–0275, 7,27 s en bucle). Dos controladores: `fx_n1_humo_nacer` donde el fuego nace (`N1_NacimientoDelFuego` y la mecánica del N1) y `fx_n1_humo` donde ya ardía. En las narrativas va detrás de cada llama, a 0,6 de su escala. **En la mecánica** (`Level1_Cave`, `Suelo/Humo`) tiene el pivote en la base y mide 87 × 150, la proporción del recorte, en el punto del golpe: nace al converger, entre las hojas y las piedras, y al prender sube a la corona de la llama, detrás de ella, encogiéndose a 0,6 (Dirección de arte §12.2) |

### `Props/Wheel/`

| Archivo | Origen |
|---|---|
| ✓ `prop_n2_tronco_a.png` (256×256) · `prop_n2_tronco_textura.png` (768×256) | `B2` (Slice 2) — definitivos (25/09/2026). `tronco_a` es el único tronco: lo comparten los cinco válidos del bosque, y ruedan en el motor con `RollingLog` sobre `tronco_textura`, la corteza desenrollada y el corte (`N2_TroncoRodante.asset`, acta D09). `_b`…`_e` se borraron en `1d5ce58` |
| ✓ `prop_n2_piedra_a.png` … `_d` · `prop_n2_planta_a.png` … `_c` · `prop_n2_herramienta_a.png` … `_c` | `B2` — distractores, definitivos (25/09/2026, 256×256). El bosque usa `piedra_a` en sus tres piedras y una planta y una herramienta distintas por objeto; `piedra_b` sale en `N3_PuenteII`, y `piedra_c` y `_d` no los usa nadie. Con `tronco_a`, son los once de `Props/Wheel/` que siguen en `Multiple` (INC-128) |
| ✓ `prop_n2_caja_suelo.png` · — ~~`_sobre_troncos`, `_rodando`~~ | `B3` — la caja llena, definitiva (25/09/2026, 256×256); los tres estados usan el mismo dibujo. La 2.2 abre con ella en el sitio y con el tamaño en que la deja el bosque (INC-120) |
| ✓ `prop_n2_caja_suelo_vacia.png` — la caja vacía (256×256, `1d5ce58`) con la que abrió la 2.2 del 25 al 30/09/2026: **sin uso** desde que la 2.2 volvió a la caja llena (INC-120). Llegó con tilde y se renombró desde el motor (INC-126) | — |
| ✓ `prop_n2_pieza_1.png` (tronco corto; **los dos gemelos comparten este archivo**), `_2` (cuerda), `_3` (eje), `_4` (tabla), `_5` (herramienta) — definitivos (25/09/2026); `_1`, `_2` y `_5` a 256×256, `_3` y `_4` a 512×512 porque la 2.3 los enseña a más de 256 px. La caja (`_6`) **es** `prop_n2_caja_suelo.png` (B5 = B3). Referenciados desde `Assets/Game/Data/Wheel/N2_AssemblyContent.asset`: sustituir el `.png` conservando el nombre basta | `B5` |
| ✓ `prop_n2_carretilla_e1.png` … `_e5` — definitivos (25/09/2026): `e1` es la rueda perforada que sustituye al tronco al mecanizar (256×256, mismo encuadre que `pieza_1`); `e2`..`e4` los estados del conjunto en el lugar de armado —eje, tabla, caja— y `e5` la carretilla amarrada: el taller pasa a ella al amarrar la cuerda (`AssemblyContent.TiedArt`, INC-121), y es la carretilla de la 2.4, la 2.5, `N3_PuenteII`, `N3_PuenteII_Rio` y la 3.1. `e2`..`e5` a 512×512 y con el mismo encuadre, porque se sustituyen en el mismo sitio y el cierre del taller los acerca a ~500 px | `B6` |
| ✓ `prop_n2_laberinto_carretilla.png` — definitiva (25/09/2026), cenital, 256×256: los rodillos van arriba y abajo, así que el dibujo mira al norte, que es como lo espera la escena al girarla según la orientación. Referenciada desde `N2_MazeLayout.asset` (`CartArt`) | `B7` |
| ✓ `prop_n2_laberinto_obstaculo.png` — un arbusto cenital (25/09/2026, `1d5ce58`), el único sprite de obstáculo; `N2_MazeLayout.asset` (`ObstacleArt[0]`), dibujado a ×1,65 (`ObstacleScale`) para que los arbustos contiguos se lean como seto. Los obstáculos del laberinto son arbustos (RF-30): no hay piedras, curvas ni pendientes | `B8` |

Solo los troncos llevan la madera trabajada `#C79A5E`: es lo que separa lo fabricado de lo
natural y a la vez lo válido del distractor (§8.2).

`Level2_Forest` está cableada contra los objetos del bosque a través del campo `Art` de cada uno en
`Assets/Game/Data/Wheel/N2_WheelLevelConfig.asset`: **sustituir el `.png` conservando el nombre y
el `.meta` basta**, sin tocar código ni escena. Como esos once siguen en `Multiple`, si el
archivo nuevo tiene otro tamaño hay que ajustar el recorte del sub-sprite desde el motor
conservando su `spriteID`, como se hizo con las piedras el 25/09/2026 (Dirección de arte §15.2).

**`Props/Wheel/Animations/`**

| Archivo | Origen |
|---|---|
| ○ `prop_n2_tronco_rodar.anim`, `prop_n2_piedra_vuelta.anim`, `prop_n2_planta_vuelta.anim`, `prop_n2_herramienta_vuelta.anim` | `S09a` — cada familia vuelve distinto |
| ○ `prop_n2_caja_arrastre.anim`, `_asiento`, `_rodando` | `S09a` |
| ○ `prop_n2_pieza_flotacion.anim`, `prop_n2_pieza_seleccion.anim`, `prop_n2_mecanizado.anim` | `S09a` |
| ○ `prop_n2_carretilla_e2.anim` … `_e5.anim` | `S09a` — un clip por encaje |
| ○ `prop_n2_carretilla_avanzar.anim`, `_girar`, `_retroceso` | `S09b` — fase 3 cenital |

### `Props/River/`

| Archivo | Origen |
|---|---|
| ✓ `prop_n3_tronco.png`, `_sogas`, `_tela`, `_mastil` | `C4` (Slice 3) — definitivos (25/09/2026; antes, provisionales de R06 por código). Referenciados desde `N3_RiverLevelConfig.asset` (`Art` de cada material) y reutilizados como icono de inventario: sustituir el `.png` conservando el nombre basta. Desde el 20/09/2026 los troncos son **cinco hallazgos sueltos** con el mismo sprite `prop_n3_tronco`. En la orilla se ven a la escala de la 3.1 (`CollectibleTemplate` a 172,8, ×1,8 desde la lectura B, INC-118), y la 3.1 pinta los mismos materiales: `prop_n3_tronco` y `prop_n3_mastil` junto a las sogas y la tela. Por eso `prop_n3_troncos.png` (el montón, en el estilo plano de antes) queda **sin uso**; borrarlo es decisión de Santiago y del carril de arte. |
| ✓ `prop_n3_tronco.png`, `prop_n3_amarre.png`, `prop_n3_vela.png` (+ `prop_n3_mastil.png`) y sus `_silueta` | `C7` **rehecho como composición** (R11, decisión de Santiago del 20/09/2026): la balsa **no** son tres láminas de estado sino diecisiete espacios que se pintan uno a uno —silueta hasta que se llena, pieza después— con **ocho sprites**: cuatro piezas y cuatro siluetas dibujadas aparte. Los espacios, sus fracciones y el arte por clase viven en `N3_RaftAssemblyContent.asset`. **Definitivos** (25/09/2026), en 3/4 como `prop_n3_balsa_cruzando`: tronco en diagonal con el corte abajo a la izquierda (256), liana que cruza el tronco (128), mástil vertical (512) y vela de cuero (256); ninguno se gira en la balsa. Los ocho son **legibles** (Read/Write): el panel prueba su alfa al agarrar y al soltar, porque las cajas de los troncos diagonales se solapan. |
| ✓ `prop_n3_balsa_hundida.png` · ✓ `_cruzando` | `C9` — `_cruzando` definitiva (25/09/2026, 512×512), en la 3.3: navega por debajo de la espuma de la cascada, a y 0,395 (INC-124). Tiene cuatro troncos y la mecánica arma cinco: se acepta así, y uno de cinco queda pendiente del carril de arte. `_hundida` (16/09/2026, por código, paleta de §8.3, en el estilo plano de antes), en la 3.2, se acepta como definitiva (acta D10 §5). Un dibujo nuevo entra **sustituyendo el archivo con el mismo nombre**, sin tocar el asset. |

**`Props/River/Animations/`**

| Archivo | Origen |
|---|---|
| ○ `prop_n3_material_flotacion.anim`, `prop_n3_material_recogida.anim` | `S10` |
| ○ `prop_n3_balsa_base.anim`, `_amarre`, `_vela`, `_hundida`, `_cruzando` | `S11b` |

---

## `UI/`

Subdividido por nivel igual que `Props/` y `Environments/`; `Common/` es lo que reutilizan varias
pantallas. **Sin cifras a la vista del estudiante en ninguno de estos assets** (CP-03, RF-17,
RF-45) y **sin texto dentro de la imagen**: el texto lo escribe Unity encima.

### `UI/Common/`

| Archivo | Origen |
|---|---|
| ✓ `ui_panel.png`, `ui_boton.png`, `ui_circulo.png`, `ui_alerta.png`, `ui_papelera.png` | El conjunto genérico de interfaz (T05–T08, 08/09/2026): tablilla, botón, círculo, icono de alerta y papelera, teñidos desde el Inspector. Con él se arman el cuadro de diálogo, el panel del N1, el editor de bloques, el contador y los iconos de estado del N2, la cruceta y las listas del N3, el informe docente y los menús. Aceptado como definitivo por Santiago el 30/09/2026 (acta D10 §5) |
| ✓ `ui_lock.png` | T07 — candado del menú de niveles y de los botones que todavía no se pueden usar en los niveles 1 y 2, segundo canal de RNF-19 |
| ✓ `ui_pausa.png`, `ui_reanudar.png`, `ui_reiniciar.png` | Glifos del menú de pausa (mockup 6). **No son arte del proyecto**: son los iconos Phosphor `pause`, `play` y `arrow-counter-clockwise` que usa el mockup, rasterizados a 128 px en blanco y teñidos desde el Inspector — ver la nota de licencia abajo |
| ✓ `ui_flecha.png` | Punta de flecha de los dos botones que desplazan la lista de la secuencia del Nivel 2 (se gira 180° para «bajar») y de la cruceta del Nivel 3 (INC-91). Dibujada para el proyecto |
| ✓ `ui_pulso_pista.anim` | El pulso de escala 1.0–1.06 del botón de pista del Nivel 1, ciclo de 2,2 s (§12.2); `Animation` legacy, que sí reproduce curvas de escala |
| — ~~`ui_dialogo_marco.png`, `ui_dialogo_continuar.png`, `ui_dialogo_omitir.png`~~ | `A10` (Slice 1) — no se generan: el cuadro de diálogo (§10.3) se arma con `ui_panel` para el marco y el fondo y `ui_boton` para el retrato, «Continuar» y «Omitir» |
| — ~~`ui_transicion_fundido.png`~~ · ○ `ui_cortinilla_tablilla.png` | `S02` — sistema de transición. El fundido a negro no lleva imagen: lo pinta `SceneLoader` con `OnGUI`. La cortinilla no está en el juego |
| — ~~`ui_estado_aceptado.png`, `ui_estado_devuelto.png`~~ | `B10` (Slice 2) — no se generan: el icono de la frase del guía es `ui_circulo` para lo aceptado y `ui_alerta` para lo devuelto, cada uno con su color (RNF-19) |
| ✓ `ui_ind_intentos_boceto.png`, `_errores_boceto`, `_pasos_boceto`, `_tiempo_boceto` | `D1` (Slice 4) — solo informe docente, RF-46. Boceto dibujado con Pillow el 24/09/2026 y cableado en `TeacherReport.unity`; aceptado como definitivo por Santiago el 30/09/2026 (acta D10 §5) |
| — ~~`ui_teacherreport_maqueta.png`~~ · ~~`ui_dialogo_eliminar.png`~~ | `D2`, `D3` — maquetas que perdieron su objeto: la pantalla del informe y el diálogo de borrado se construyeron directamente en Unity (`Slice-4-Resultados.md`, decisión 8) |

> **Licencia de los tres glifos de pausa.** Salen de la familia **Phosphor Icons** (MIT), la
> misma que dibuja los iconos del mockup. Se acreditan en los créditos con su licencia, como las
> tipografías (OFL), y «originales del proyecto» ya no los abarca (INC-127). Es el único crédito de
> Phosphor: solo los iconos de pausa, no el resto de la interfaz (`ui_alerta`, `ui_lock`, `ui_papelera`,
> `ui_circulo` y `ui_flecha` son del proyecto). Los créditos dicen «Entornos, objetos e interfaz: originales
> del proyecto, salvo los iconos de pausa.» e «Iconos de pausa: Phosphor Icons, licencia MIT.». La licencia
> está en `Assets/Game/Art/UI/Common/LICENSE-Phosphor.txt` y el build la copia a `Licencias/`.

**`UI/Common/Animations/`** · — ~~`ui_transicion_fundido.anim`~~ (el fundido es de `SceneLoader`) ·
○ `ui_cortinilla.anim` (`S02`) · — ~~`ui_estado_aceptado.anim`, `ui_estado_devuelto.anim`~~
(`S09a`: no hay sprites de estado, `B10`)

### `UI/Fire/`

| Archivo | Origen |
|---|---|
| — ~~`ui_n1_panel_deslizante.png`, `ui_n1_panel_tirador.png`~~ | `A9` — no se generan: los dos rieles de diez muescas, fuerza y cercanía (INC-47), son `Slider` de uGUI con `Common/ui_circulo` y `ui_boton` teñidos (mockup 7). Sus valores siguen sin validar jugando (PG-06 abierto) |
| — ~~`ui_n1_panel_golpear_reposo.png`, `_presionado`~~ | `A9` — no se generan: «Golpear» es `Common/ui_boton` |
| — ~~`ui_n1_panel_soplar_deshabilitado.png`, `_habilitado`~~ | `A9` — no se generan: «Soplar» es `Common/ui_boton`, y deshabilitado lleva el candado `Common/ui_lock`, que es el segundo canal (RNF-19) |
| — ~~`ui_n1_panel_registro_marco.png`~~ | `A9` — no se genera: la tablilla (`Mensaje`) es `Common/ui_boton` y enseña solo el último mensaje |
| — ~~`ui_n1_mascara.png`~~ | `S07b` — no se genera: la oscuridad es la capa del shader `fx_oscuridad` (`CaveLightingController`, punto abierto 4) |

El panel del N1 es el conjunto genérico de interfaz, aceptado como definitivo (acta D10 §5).

**`UI/Fire/Animations/`** · — ~~`ui_n1_mascara.anim`~~ (`S07b`: la luz sube con el avance del reto, RF-21, en la capa `fx_oscuridad` que mueve `CaveLightingController`)

### `UI/Wheel/`

| Archivo | Origen |
|---|---|
| — ~~`ui_n2_bloque_avanzar_*`, `ui_n2_bloque_retroceder_*`, `ui_n2_bloque_girar_*`~~ (`_reposo`, `_resaltado`) | `B9` (Slice 2) — no se generan: los bloques del editor (mockup 10) son `Common/ui_boton` y `ui_circulo` teñidos, y las tres siluetas de RNF-19 —rectángulo para «Avanzar», píldora para «Girar» y rombo para «Retroceder»— las arma el motor (`MazeSceneController.SpawnBlock`, INC-55, INC-58) |
| — ~~`ui_n2_ejecutar.png`~~ | `B9` — no se genera: «Ejecutar» es `Common/ui_boton`, un solo estado, clic simple (PG-04) |
| — ~~`ui_n2_contador_marco.png`~~ | `B10` — no se genera: el contador de acopio del bosque es texto sobre una tablilla de marfil (`Common/ui_panel`) |

Los tres son el conjunto genérico de interfaz, aceptado como definitivo (acta D10 §5).

**`UI/Wheel/Animations/`** — `S09b` · ○ `ui_n2_bloque_snap.anim`, `ui_n2_bloque_resaltado.anim`,
`ui_n2_boton_ejecutar.anim`

### `UI/River/`

| Archivo | Origen |
|---|---|
| — ~~`ui_n3_dir_arriba_reposo.png`, `_presionado` (y `abajo`, `izquierda`, `derecha`)~~ | `C3` (Slice 3) — no se generan: la cruceta son cuatro `Common/ui_boton` con `ui_flecha` girada, abajo a la derecha (INC-91) |
| — ~~`ui_n3_recoger_disponible.png`, `_no_disponible`~~ | `C3` — no se generan: «Recoger» es un `Common/ui_boton` con su rótulo. La cruceta y «Recoger» materializan INC-01: el control es UI, no teclado (CT-06) |
| ✓ `ui_n3_casilla_hecha.png` · — ~~`ui_n3_lista_marco.png`, `ui_n3_inventario.png`~~ | `C5` — única lista permanente del juego (INC-41). La casilla hecha es un círculo verde con visto, dibujado por código (R05, 17/09/2026) y aceptado como definitivo con el conjunto genérico de interfaz (acta D10 §5); la pendiente es `Common/ui_circulo.png`. Son las dos formas de RNF-19 en la lista de tareas, y la casilla hecha es también la marca «Completado» de cada tarjeta del menú de niveles (INC-76). La lista y el inventario son `Common/ui_panel`, así que su marco no se genera. |
| — ~~`ui_n3_panel_marco.png`, `ui_n3_espacio_*`~~ | `C8` — no se generan desde R11 (20/09/2026): el panel es una sombra negra al 30 % sobre la ilustración, sin marco, y los espacios vacío, correcto e incorrecto son la silueta de la pieza, la pieza y la pieza con `Common/ui_alerta` encima (RNF-19). Los botones del panel, «Listo» y «Probar balsa», son `Common/ui_boton` (INC-122). |

**`UI/River/Animations/`**

| Archivo | Origen |
|---|---|
| ○ `ui_n3_dir_pulsado.anim`, `ui_n3_recoger_aparicion.anim`, `ui_n3_inventario_icono.anim` | `S10` |
| ○ `ui_n3_casilla_completada.anim`, `ui_n3_panel_entrada.anim`, `ui_n3_espacio_correcto.anim`, `ui_n3_espacio_incorrecto.anim` | `S11b` |

---

## `FX/`

Sprites de color plano y animación por fotogramas: sin sistemas de partículas ni posprocesado, y
sin más shaders propios que los dos de color plano que viven aquí, `fx_oscuridad` y `fx_contraste`
(`.shader` + `.mat`, §12.1). **No hay rojo de error en ningún efecto** (§12.3).

| Archivo | Origen |
|---|---|
| ○ `fx_algoritm_barrido.png` | `S02` — 8 frames |
| — ~~`fx_n1_llama.png`~~ | `S07b` — no se genera: la llama es `prop_n1_fuego_cenital` y `prop_n1_fuego_normal` (`Props/Fire/Animations/`) |
| ○ `fx_n1_halo.png` | `S07b` |

**`FX/Animations/`**

| Archivo | Origen |
|---|---|
| ○ `fx_algoritm_barrido.anim` | `S02` |
| ○ `fx_algoritm_barrido_tr05.anim`, `fx_algoritm_barrido_tr09.anim` | `S06` — barridos con muta del guía |
| — ~~`fx_n1_chispa_lejos.anim`, `_cerca`, `_muycerca`~~ | `S07a` — retirados (INC-47, INC-68): la chispa del golpe la dibuja el motor, y no hay archivo. Es un solo rayo `#FFE9A8` de 4 u de grosor que nace en el punto del golpe y hace un único barrido de cabeza y cola, sin volver (RNF-21): el efectivo cae en las hojas y el de fuerza de más se apaga en el aire (INC-119, Dirección de arte §12.2). En `Level1_Cave` es `Suelo/Chispa`, con el pivote en la cola, y su `Image` `RayoH` estirada; `FirePanelController` le da dirección, largo y tiempo |
| ✓ `fx_n1_humo_nacer.anim`, `fx_n1_humo.anim` | `S07a` — en `Props/Fire/Animations/`, junto al fuego. En la mecánica del N1 (`Level1_Cave`, `Suelo/Humo`) el humo tiene el pivote en la base y mide 87 × 150 en el punto del golpe: nace al converger y al prender sube a la corona de la llama, detrás de ella, encogiéndose a 0,6 (§12.2) |
| — ~~`fx_n1_llama.anim`~~ · ○ `fx_n1_halo.anim` | `S07b` — la llama anima con `prop_n1_fuego_*.anim`. Halo: escala 0.95–1.05, ciclo 1.2 s |
| ○ `fx_n2_polvo.anim` | `S09a` — polvo del mecanizado |
| ○ `fx_vaho.anim` | `S16a` — vaho de la noche helada |

---

## `Reference/`

No entran al juego: son material de referencia para mantener la coherencia entre generaciones.

| Archivo | Origen |
|---|---|
| ○ `ref_paleta_e1_cueva.png`, `_e2_bosque`, `_e3_taller`, `_e4_tablero`, `_e5_rio`, `_e6_fogatas` | `S01` — muestrarios de paleta |

---

## Puntos abiertos — no se resuelven en este archivo

Cerrados por el tablero: los tres cuerpos de Algoritm con sus estados (`S15`), la hoguera
(`S07b`: `fx_n1_llama` + `fx_n1_halo`), el icono de pista —es Algoritm, `char_algoritm_n*_anim_pulso`
de `S06`, como manda §10.2— y la convención de nombres de los `.anim`.

1. **`claudeDocs/tasks/Slice 1/plan.md:959` sigue nombrando `char_chispa_base_reposo.png`** y
   su prompt `A1` pide una estrella de cinco puntas. El juego usa
   `char_algoritm_n1_fuego_reposo.png`, una llama (`Direccion_de_Arte.md` §7.6, INC-52). Los
   `plan.md` no se reescriben, así que `A1` queda como registro de lo que se pidió.
2. ~~`§15.4` está desactualizada en cuatro cosas~~ — resuelto el 30/09/2026: §15.4 recoge el prefijo
   `ref_` y la convención de `.anim`, y sus ejemplos son archivos que existen (una sola expresión,
   `retrato_neutra`, INC-108).
3. ~~`§12.2` describe el icono de pista como «antorcha de UI»~~ — resuelto el 30/09/2026: §12.2 describe el pulso del botón de pista del Nivel 1 (`ui_pulso_pista`).
4. ~~Dos implementaciones de la progresión de luz del Nivel 1~~ — resuelto el 30/09/2026: el juego no
   usa ni `env_n1_cueva_luz1..4` ni `ui_n1_mascara`, sino la capa del shader `fx_oscuridad`
   (`CaveLightingController`); ninguno de los dos assets se genera.
5. ~~`char_mama_cenital.png` frente a `char_mama_cenital_norte.png`, `_este`, `_sur`, `_oeste`~~ —
   resuelto el 30/09/2026: no hay láminas por dirección. En el río Mamá es su rig frontal, que
   camina con un solo clip volteado; `Characters/Mother/char_mama_cenital.png` sigue en la `Image`
   raíz de `Personaje_Mama`, apagada, de reserva, y no se ve.
6. **Dos efectos de `§12.2` no están en el tablero:** recolección de objeto y reto resuelto. El
   de recolección puede estar cubierto por `prop_n3_material_recogida.anim` (`S10`), el otro no.
   La salpicadura de la balsa no lleva imagen: es solo sonora (INC-123).
7. ~~`Characters/Boy/Animations/` está vacío~~ — resuelto el 24/09/2026: el Niño tiene los mismos
   21 clips que el resto de la familia.
8. **El tablero salta de `S12` a `S15`:** no hay `S13` ni `S14`. Verificar si faltan o si son
   tareas que no producen archivos.
9. ~~Los tramos escritos con `…` salen de la cantidad que declara el prompt~~ — resuelto: las
   piedras son cuatro archivos (`prop_n2_piedra_a` … `_d`), la casilla del Nivel 3 es solo
   `ui_n3_casilla_hecha` (la pendiente es `Common/ui_circulo`) y los `ui_n3_espacio_*` no se
   generan (`C8`).
