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

**Convención de clips, fijada por el tablero** (§15.4 aún no la recoge):
personajes `<sujeto>_anim_<accion>.anim` · todo lo demás `<sujeto>_<accion>.anim`.

**Muchos assets llegan como una lámina, no como archivos sueltos.** Los prompts piden varias
piezas en una sola imagen por filas; se importan con `Sprite Mode: Multiple` y se cortan en
Unity (§15.2). Los nombres de abajo son los de los sprites resultantes.

Leyenda: **✓** en disco · **◐** provisional en disco (sustituir conservando el nombre) · **○** pendiente. Estados del tablero: `·` pendiente · `G` generado ·
`R` recortado · `I` importado · `A` animado · `✔` aprobado. Hoy **todo está en `·`**.

---

## `Characters/`

**Cómo se animan (24/09/2026, INC-53).** Por **recorte**, no con el rig de 2D Animation que pide
§13.1: las escenas son uGUI en un Canvas overlay y `SpriteSkin` no deforma una `Image`. Cada
miembro de la familia está cortado en cinco partes —`torso`, `brazo_izq`, `brazo_der`,
`pierna_izq`, `pierna_der`, con izquierda y derecha **de pantalla**—. Son `Image` hijas con el
pivote en la articulación, sobre un lienzo de 1024 × 1024, el de los sprites base: la figura
ocupa y 77..947. Un `Animator` las gira y desplaza con un clip por acción; el componente es
`CharacterRig` (`Game.Scaffolding`) y los prefabs están en `Assets/Game/Prefabs/Characters/`. Las
partes salen de los sprites base entregados el 24/09/2026 (`…/assets a postproduccion/familia/`),
limpios de restos de croma verde en el pelo. **Para sustituirlas por arte definitivo, se reemplaza
cada `.png` conservando el nombre**; si cambia la silueta, hay que rehacer el prefab, porque el
tamaño y el pivote de cada parte viven en él.

Retratos: solo existe `neutra`, un recorte de la cabeza de 320², que es el que usa el cuadro de
diálogo. Las otras cinco expresiones de `S03a`/`S03b` siguen pendientes, y hoy ninguna línea pide
una expresión. **No hay tristeza ni enfado**: tras un intento sin éxito el personaje hace «ánimo»
(§7.3, CP-02). **No existen saltar, caer, aterrizar ni derrota** (CT-06, RNF-02, CP-02).

**Clips (21 por miembro de la familia).** `char_<x>_anim_<accion>.anim`, uno por estado del
`Animator`, cuyo nombre es el de `ActorAction`: `idle`, `caminar`, `correr`, `hablar`, `golpear`,
`martillar`, `soplar`, `recoger`, `arrodillarse`, `cargar`, `empujar`, `senalar`, `observar`,
`celebrar`, `animo`, `abrazar`, `sorpresa`, `dormir`, `oculto`, `aparicion` y `apagado`. Los genera
un constructor efímero con un tempo por personaje: Papá, amplio y lento; el Niño, rápido (§7.4). El
controlador `char_<x>.controller` está al lado.

### `Characters/Algoritm/` — el guía, en los tres niveles (CN-03)

| Archivo | Estado |
|---|---|
| ✓ `char_algoritm_n1_fuego_reposo.png` | Arte entregado (24/09/2026), recortado a cuadrado de 768². Es una llama con extremidades: ver **INC-52** |
| ◐ `char_algoritm_n2_rueda_reposo.png` | **Provisional**: el fuego recoloreado en madera. El definitivo entra sustituyendo el archivo |
| ◐ `char_algoritm_n3_gota_reposo.png` | **Provisional**: el fuego recoloreado en agua. Ídem |
| ○ `_girando`, `_atenuado`, `char_algoritm_n*_retrato_*` | `S15`/`S03b`. Hoy el retrato del guía **es** el sprite de su forma |

Una sola `Image` por forma y ningún recorte, para que sustituir el archivo baste. Los tres prefabs
`Algoritm_Fuego`, `Algoritm_Rueda` y `Algoritm_Gota` comparten `Animations/char_algoritm.controller`
y sus clips `char_algoritm_anim_{flotar,hablar,senalar,girar,celebrar,animo,oculto,aparicion,apagado}`.
El mismo sprite va dentro del botón de ayuda circular de las cinco mecánicas.

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
la multiplica por la capa de oscuridad, que amplifica el error— y corre los colores. Sin comprimir
las 44 texturas de `Art/` ocupan ~92 MB, muy por debajo de RNF-05. `ArtImport_RNF23_…` lo vigila.
Un archivo nuevo entra ya bien: **sustituir la imagen basta**, la escala la calcula
`IllustrationFraming` con el tamaño real del sprite.

**Lo que hay en disco hoy** (entrega de entornos finales, acta D06): los de pantalla a 1920×1080 y
las panorámicas a 3840×1080, en la misma composición que los provisionales —comprobado: el
contenido cae en las mismas fracciones del lienzo, así que **ningún encuadre de
`Camara_Narrativa_N1/N2` cambia de valor**—. Cinco conservan el prefijo viejo `entorno_` y hay que
renombrarlos a `env_` **desde el motor** (tarjeta `D06-3`), que conserva el GUID y no toca escenas:
`entorno_n1_apertura`, `entorno_n1_cueva_2x`, `entorno_n1_cueva_cenital`, `entorno_n2_laberinto`
—`env_n2_bosque_claro` ya cumple—. Tres llegaron con nombre libre y en `Sprite Mode: Multiple`,
que para un fondo entero no aporta y hace que `LoadAssetAtPath<Sprite>` devuelva nulo. **Dos ya
están resueltos desde el motor (R04, 16/09/2026):** `River/rio_normal.png` es ahora
`River/env_n3_rio.png` y `River/civilización_final.png` es `Narrative/env_final_fogatas.png`,
los dos en `Single` y con su GUID intacto. Queda `Wheel/civilización_noche.png`, que sigue sin
usarse. **Ojo:** el importador de fábrica trae `Multiple`; un sprite nuevo hay que pasarlo a
`Single` a mano (los dos provisionales de `Props/River/` entraron así).

### `Environments/Fire/`

| Archivo | Origen |
|---|---|
| ○ `env_n1_cueva_luz1.png` … `env_n1_cueva_luz4.png` | `A6` (Slice 1) — ver punto abierto 4 |

### `Environments/Wheel/`

| Archivo | Origen |
|---|---|
| ✓ `env_n2_bosque_claro.png` — 1599×899, **provisional** (11/09/2026). Importado `Single`, `maxTextureSize` 8192: el definitivo se sustituye por nombre y cubre la pantalla sin tocar nada (`IllustrationFraming`) | `B1` (Slice 2) |
| ○ `env_n2_taller.png` — **no hace falta mientras el taller sea el claro este del entorno duplicado**: `Level2_Workshop` usa `env_n2_bosque_claro.png` con el encuadre de `Camara_Narrativa_N2.md` §5.6 (W09, 12/09/2026) | `B4` |
| ○ `env_n2_tablero.png` | `B8` |
| ○ `env_n2_refugio_noche.png` | `S16b` — refugio con fuego encendido, 21:9 |

**`Environments/Wheel/Animations/`** · ○ `env_n2_nubes.anim` (`S12`, deriva lenta)

### `Environments/River/`

| Archivo | Origen |
|---|---|
| ✓ `env_n3_rio.png` | Entregado en D06 como `rio_normal.png` (1920×1080, vista lateral con la cascada). **Todo el Nivel 3 se juega y se narra sobre él** —`docs/md/Camara_Narrativa_N3.md` §4—, así que la vista superior de `C1` y el `_lateral` de `S16c` quedan sin uso salvo decisión contraria de Santiago. |
| ○ `env_n3_espuma.png` | `S11a` — 4 frames |
| ○ `env_n3_zona_inactiva.png` · ◐ `env_n3_zona_disponible.png` | `C6` — la `_disponible` es **provisional** (R08, 17/09/2026): anillo ámbar discontinuo dibujado por código; `Zona_Construccion` en `Level3_River.unity`. Hoy la zona no tiene estado inactivo: se ve igual desde el principio (RF-39). |

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

---

## `Props/`

Contorno de 7–9 px en `#3A1E18` para lo interactivo y de 4 px en `#5C4038` para lo decorativo;
el color de acento del nivel está prohibido en el decorado (§9.2, §4.2).

### `Props/Fire/`

| Archivo | Origen |
|---|---|
| ○ `prop_n1_hojas_intacto.png`, `_chispas`, `_humeante`, `_encendido` | `S07a` / `A7` |
| ○ `prop_n1_silex.png`, `prop_n1_pedernal.png`, `prop_n1_piedras_choque.png` | `A8` (Slice 1) |

**`Props/Fire/Animations/`** — `S07a`

| Archivo | Nota |
|---|---|
| ○ `prop_n1_hojas.anim` | cruce entre los cuatro estados |
| ○ `prop_n1_piedras_choque.anim`, `prop_n1_piedras_flotacion.anim` | |
| ✓ `fx_n1_humo_nacer.anim` + `fx_n1_humo.anim` · `Smoke/` (33 PNG) | El humo de `S07a`: vive junto al fuego y no en `FX/Animations/`. Entrega de 284 cuadros a 30 fps, animada a ochos y nueves: 33 dibujos distintos, que conservan el nombre de entrega (`humo_nivel_1_NNNN.png`, `Single`). **Recortados** del lienzo de 2144 × 2108 a 1123 × 1933 (desde x 485, y 134): el mismo rectángulo en todos, así el dibujo no salta de un cuadro a otro, y la mitad de memoria. Si llega una entrega nueva con el lienzo entero, hay que recortarla igual o recalcular `Position` y `Size` de los ocho humos. `nacer` (0009–0059, 1,97 s) sube del hilo a la voluta y pasa a `fx_n1_humo` (0067–0275, 7,27 s en bucle). Dos controladores: `fx_n1_humo_nacer` donde el fuego nace (`N1_NacimientoDelFuego`) y `fx_n1_humo` donde ya ardía. Va detrás de cada llama, a 0,6 de su escala |

### `Props/Wheel/`

| Archivo | Origen |
|---|---|
| ✓ `prop_n2_tronco_a.png` … `_e` (5, los válidos) | `B2` (Slice 2) — **provisional** |
| ✓ `prop_n2_piedra_a.png` … `_d` · `prop_n2_planta_a.png` … `_c` · `prop_n2_herramienta_a.png` … `_c` | `B2` — distractores, **provisionales** |
| ✓ `prop_n2_caja_suelo.png` — **provisional** (generado por código, W07, 11/09/2026); ○ `_sobre_troncos`, `_rodando` — hoy los tres estados usan el mismo sprite | `B3` |
| ✓ `prop_n2_pieza_1.png` (tronco corto; **los dos gemelos comparten este archivo**), `_2` (cuerda), `_3` (eje), `_4` (tabla), `_5` (herramienta) — definitivos (25/09/2026); `_1`, `_2` y `_5` a 256×256, `_3` y `_4` a 512×512 porque la 2.3 los enseña a más de 256 px. La caja (`_6`) **es** `prop_n2_caja_suelo.png` (B5 = B3). Referenciados desde `Assets/Game/Data/Wheel/N2_AssemblyContent.asset`: sustituir el `.png` conservando el nombre basta | `B5` |
| ✓ `prop_n2_carretilla_e1.png` … `_e5` — definitivos (25/09/2026): `e1` es la rueda perforada que sustituye al tronco al mecanizar (256×256, mismo encuadre que `pieza_1`); `e2`..`e4` los estados del conjunto en el lugar de armado —eje, tabla, caja— y `e5` la carretilla con la cuerda, que solo usan las narrativas: en el taller la cuerda es la pieza colgada sobre `e4` (INC-54). `e2`..`e5` a 512×512 y con el mismo encuadre, porque se sustituyen en el mismo sitio y el cierre del taller los acerca a ~500 px | `B6` |
| ✓ `prop_n2_laberinto_carretilla.png` — definitiva (25/09/2026), cenital, 256×256: los rodillos van arriba y abajo, así que el dibujo mira al norte, que es como lo espera la escena al girarla según la orientación. Referenciada desde `N2_MazeLayout.asset` (`CartArt`) | `B7` |
| ✓ `prop_n2_laberinto_obstaculo.png` — **provisional** (W13): copia de `_piedra_a`; `N2_MazeLayout.asset` (`ObstacleArt[0]`). ○ `prop_n2_obstaculo_piedra.png`, `_curva`, `_pendiente` | `B8` |

Solo los troncos llevan la madera trabajada `#C79A5E`: es lo que separa lo fabricado de lo
natural y a la vez lo válido del distractor (§8.2).

> ⚠️ **Los quince `.png` de arriba están en disco pero son marcadores de posición y su contenido
> no corresponde al nombre** (10/09/2026): `prop_n2_planta_*` dibuja piedras y
> `prop_n2_herramienta_*` dibuja madera. Solo hay **dos ilustraciones distintas** repartidas entre
> las cuatro categorías, así que la fase 1 del Nivel 2 se puede recorrer pero **no se puede jugar
> de verdad**: el patrón se busca mirando, y hoy un distractor puede verse igual que un válido.
> `Level2_Forest` ya está cableada contra ellos a través del campo `Art` de cada objeto en
> `Assets/Game/Data/Wheel/N2_WheelLevelConfig.asset`, de modo que **sustituir el `.png` conservando
> el nombre basta**: el `guid` no cambia y no hay que tocar ni código ni escena. Al reemplazarlos,
> quitar esta nota y el `provisional` de la tabla.

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
| ✓ `prop_n3_tronco.png`, `_sogas`, `_tela`, `_mastil` | `C4` (Slice 3) — definitivos (25/09/2026; antes, provisionales de R06 por código). Referenciados desde `N3_RiverLevelConfig.asset` (`Art` de cada material) y reutilizados como icono de inventario: sustituir el `.png` conservando el nombre basta. Desde el 20/09/2026 los troncos son **cinco hallazgos sueltos** con el mismo sprite `prop_n3_tronco`; `prop_n3_troncos.png` (el montón) solo sale en la 3.1 y sigue en el estilo plano de antes. |
| ✓ `prop_n3_tronco.png`, `prop_n3_amarre.png`, `prop_n3_vela.png` (+ `prop_n3_mastil.png`) y sus `_silueta` | `C7` **rehecho como composición** (R11, decisión de Santiago del 20/09/2026): la balsa **no** son tres láminas de estado sino diecisiete espacios que se pintan uno a uno —silueta hasta que se llena, pieza después— con **ocho sprites**: cuatro piezas y cuatro siluetas dibujadas aparte. Los espacios, sus fracciones y el arte por clase viven en `N3_RaftAssemblyContent.asset`. **Definitivos** (25/09/2026), en 3/4 como `prop_n3_balsa_cruzando`: tronco en diagonal con el corte abajo a la izquierda (256), liana que cruza el tronco (128), mástil vertical (512) y vela de cuero (256); ninguno se gira en la balsa. Los ocho son **legibles** (Read/Write): el panel prueba su alfa al agarrar y al soltar, porque las cajas de los troncos diagonales se solapan. |
| ◐ `prop_n3_balsa_hundida.png` · ✓ `_cruzando` | `C9` — `_cruzando` definitiva (25/09/2026, 512×512, cuatro troncos: la mecánica arma cinco); `_hundida` sigue **provisional** (16/09/2026, por código, paleta de §8.3) y en el estilo plano de antes. Los usan `N3_Escena32_PrimerIntento` y `N3_Escena33_Cruce`; el definitivo entra **sustituyendo el archivo con el mismo nombre**, sin tocar el asset. |

`C4` genera además el icono de inventario de cada material, en la misma lámina.

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
| ✓ `ui_lock.png` | T07 — candado del menú de niveles, segundo canal de RNF-19 |
| ✓ `ui_pausa.png`, `ui_reanudar.png`, `ui_reiniciar.png` | Glifos del menú de pausa (mockup 6). **No son arte del proyecto**: son los iconos Phosphor `pause`, `play` y `arrow-counter-clockwise` que usa el mockup, rasterizados a 128 px en blanco y teñidos desde el Inspector — ver la nota de licencia abajo |
| ✓ `ui_flecha.png` | Punta de flecha de los dos botones que desplazan la lista de la secuencia del Nivel 2 (se gira 180° para «bajar»). Dibujada para el proyecto |
| ○ `ui_dialogo_marco.png`, `ui_dialogo_continuar.png`, `ui_dialogo_omitir.png` | `A10` (Slice 1), reutilizado por los cuatro slices |
| ○ `ui_transicion_fundido.png`, `ui_cortinilla_tablilla.png` | `S02` — sistema de transición |
| ○ `ui_estado_aceptado.png`, `ui_estado_devuelto.png` | `B10` (Slice 2) |
| ○ `ui_ind_intentos.png`, `_errores`, `_pasos`, `_tiempo` | `D1` (Slice 4) — solo informe docente, RF-46 |
| ○ `ui_teacherreport_maqueta.png` | `D2` |
| ○ `ui_dialogo_eliminar.png` | `D3` |

> **Licencia de los tres glifos de pausa (17/09/2026).** Salen de la familia **Phosphor Icons**
> (MIT), la misma que dibuja los iconos del mockup. El cuerpo de `CreditsContent.asset` dice hoy
> «entornos, objetos e interfaz: originales del proyecto», que con ellos deja de ser exacto:
> decisión pendiente de Santiago —acreditarlos como se acreditan las tipografías (OFL) o
> sustituirlos por glifos propios—.

**`UI/Common/Animations/`** · ○ `ui_transicion_fundido.anim`, `ui_cortinilla.anim` (`S02`) ·
○ `ui_estado_aceptado.anim`, `ui_estado_devuelto.anim` (`S09a`)

### `UI/Fire/`

| Archivo | Origen |
|---|---|
| ○ `ui_n1_panel_deslizante.png`, `ui_n1_panel_tirador.png` | `A9` — riel de tres muescas (PG-06 abierto) |
| ○ `ui_n1_panel_golpear_reposo.png`, `_presionado` | `A9` |
| ○ `ui_n1_panel_soplar_deshabilitado.png`, `_habilitado` | `A9` — candado vs. líneas de aire: la forma es el segundo canal (RNF-19) |
| ○ `ui_n1_panel_registro_marco.png` | `A9` |
| ○ `ui_n1_mascara.png` | `S07b` — máscara de oscuridad `#0F1526` |

**`UI/Fire/Animations/`** · ○ `ui_n1_mascara.anim` (`S07b`, los cuatro escalones de RF-21)

### `UI/Wheel/`

| Archivo | Origen |
|---|---|
| ○ `ui_n2_bloque_avanzar_reposo.png`, `_resaltado` | `B9` (Slice 2) |
| ○ `ui_n2_bloque_retroceder_reposo.png`, `_resaltado` | `B9` |
| ○ `ui_n2_bloque_girar_reposo.png`, `_resaltado` | `B9` |
| ○ `ui_n2_ejecutar.png` | `B9` — un solo estado, clic simple (PG-04) |
| ○ `ui_n2_contador_marco.png` | `B10` |

**`UI/Wheel/Animations/`** — `S09b` · ○ `ui_n2_bloque_snap.anim`, `ui_n2_bloque_resaltado.anim`,
`ui_n2_boton_ejecutar.anim`

### `UI/River/`

| Archivo | Origen |
|---|---|
| ○ `ui_n3_dir_arriba_reposo.png`, `_presionado` (y `abajo`, `izquierda`, `derecha`) | `C3` (Slice 3) |
| ○ `ui_n3_recoger_disponible.png`, `_no_disponible` | `C3` — materializa INC-01: el control es UI, no teclado (CT-06) |
| ○ `ui_n3_lista_marco.png`, `ui_n3_inventario.png` · ◐ `ui_n3_casilla_hecha.png` | `C5` — única lista permanente del juego (INC-41). La casilla hecha es **provisional** (R05, 17/09/2026): círculo verde con visto; la pendiente reutiliza `Common/ui_circulo.png`. Son las dos formas de RNF-19 en la lista de tareas. Marco de lista, inventario, flechas y «Recoger» usan hoy `Common/ui_panel`, `ui_boton` y `ui_flecha`. |
| ○ `ui_n3_panel_marco.png`, `ui_n3_espacio_*` | `C8` — **parcialmente sin uso** desde R11 (20/09/2026): el panel es una sombra negra al 30 % sobre la ilustración (sin marco) y los espacios vacío/correcto/incorrecto son la silueta de la pieza, la pieza, y la pieza con `Common/ui_alerta` encima (RNF-19). Si `C8` se genera, solo el marco tendría dónde ir. |

**`UI/River/Animations/`**

| Archivo | Origen |
|---|---|
| ○ `ui_n3_dir_pulsado.anim`, `ui_n3_recoger_aparicion.anim`, `ui_n3_inventario_icono.anim` | `S10` |
| ○ `ui_n3_casilla_completada.anim`, `ui_n3_panel_entrada.anim`, `ui_n3_espacio_correcto.anim`, `ui_n3_espacio_incorrecto.anim` | `S11b` |

---

## `FX/`

Sprites de color plano y animación por fotogramas: sin sistemas de partículas, sin shaders
propios, sin posprocesado (§12.1). **No hay rojo de error en ningún efecto** (§12.3).

| Archivo | Origen |
|---|---|
| ○ `fx_algoritm_barrido.png` | `S02` — 8 frames |
| ○ `fx_n1_llama.png` | `S07b` — 4 frames |
| ○ `fx_n1_halo.png` | `S07b` |

**`FX/Animations/`**

| Archivo | Origen |
|---|---|
| ○ `fx_algoritm_barrido.anim` | `S02` |
| ○ `fx_algoritm_barrido_tr05.anim`, `fx_algoritm_barrido_tr09.anim` | `S06` — barridos con muta del guía |
| ○ `fx_n1_chispa_lejos.anim`, `_cerca`, `_muycerca` | `S07a` — un destello por posición del deslizante |
| ✓ `fx_n1_humo.anim` | `S07a` — en `Props/Fire/Animations/`, junto al fuego |
| ○ `fx_n1_llama.anim`, `fx_n1_halo.anim` | `S07b` — halo: escala 0.95–1.05, ciclo 1.2 s |
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

1. **`claudeDocs/tasks/Slice 1/plan.md:959` sigue nombrando `char_chispa_base_reposo.png`.** El
   tablero ya usa `char_algoritm_n1_estrella_reposo.png`; corregir `A1`.
2. **`§15.4` está desactualizada en cuatro cosas:** no recoge el prefijo `ref_` (`S01`), no recoge
   la convención de `.anim`, su ejemplo dice `char_nino_expr_sorpresa.png` cuando el tablero usa
   `char_nino_retrato_sorpresa.png`, y su ejemplo `fx_chispa_apagada.png` no lleva el `n1` que sí
   usan `§7.6` y el tablero.
3. **`§12.2` describe el icono de pista como «antorcha de UI»**, contra `§10.2` y el tablero.
4. **Dos implementaciones de la progresión de luz del Nivel 1:** `env_n1_cueva_luz1..4` (`A6`,
   cuatro fondos) y `ui_n1_mascara` + su clip (`S07b`, una máscara de opacidad variable). §8.1
   sanciona la máscara. Decidir antes de generar: son cuatro assets de diferencia.
5. **`char_mama_cenital.png` (`S10`, una lámina) frente a `char_mama_cenital_norte.png`, `_este`,
   `_sur`, `_oeste` (`C2`, cuatro archivos).** Probablemente la lámina se corta en los cuatro; hay
   que confirmarlo antes de importar.
6. **Tres efectos de `§12.2` no están en el tablero:** salpicadura de agua, recolección de objeto
   y reto resuelto. El de recolección puede estar cubierto por
   `prop_n3_material_recogida.anim` (`S10`), los otros dos no.
7. ~~`Characters/Boy/Animations/` está vacío~~ — resuelto el 24/09/2026: el Niño tiene los mismos
   21 clips que el resto de la familia.
8. **El tablero salta de `S12` a `S15`:** no hay `S13` ni `S14`. Verificar si faltan o si son
   tareas que no producen archivos.
9. Los tramos escritos con `…` (`prop_n2_piedra_a` … `_d`, `ui_n3_casilla_*`, `ui_n3_espacio_*`)
   salen de la **cantidad** que declara el prompt del slice, no de una lista literal. El tablero
   tampoco los enumera. Se fijan al generar la lámina.
