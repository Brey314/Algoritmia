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

Leyenda: **✓** en disco · **○** pendiente. Estados del tablero: `·` pendiente · `G` generado ·
`R` recortado · `I` importado · `A` animado · `✔` aprobado. Hoy **todo está en `·`**.

---

## `Characters/`

Seis retratos por personaje (`S03a`, `S03b`): `neutra`, `alegre`, `sorpresa`, `concentracion`,
`duda`, `animo`. **No hay tristeza ni enfado**: un intento sin éxito produce `animo`, nunca una
expresión negativa (§7.3, CP-02). Los clips son rigging 2D sobre el sprite en A-pose (§13.1),
no hojas de fotogramas, y **no existen saltar, caer, aterrizar ni derrota** (CT-06, RNF-02, CP-02).

### `Characters/Algoritm/` — el guía, en los tres niveles (CN-03)

| Archivo | Origen |
|---|---|
| ○ `char_algoritm_n1_estrella_reposo.png`, `_girando`, `_atenuado` | `S15` |
| ○ `char_algoritm_n2_rueda_reposo.png`, `_girando`, `_atenuado` | `S15` |
| ○ `char_algoritm_n3_gota_reposo.png`, `_girando`, `_atenuado` | `S15` |
| ○ `char_algoritm_n1_retrato_neutra.png` … `_animo.png` (6) | `S03b` |
| ○ `char_algoritm_n2_retrato_neutra.png` … `_animo.png` (6) | `S03b` |
| ○ `char_algoritm_n3_retrato_neutra.png` … `_animo.png` (6) | `S03b` |

Un cuerpo por nivel, mismo núcleo de identidad (§7.6). Solo muta en los barridos `TR-05` y
`TR-09`, nunca a la vista dentro de una escena jugable.

**`Characters/Algoritm/Animations/`** — `S06`

| Archivo | Nota |
|---|---|
| ○ `char_algoritm_n1_anim_flotar.anim`, `n2`, `n3` | flotación y giro, ciclo 2 s |
| ○ `char_algoritm_n1_anim_pulso.anim`, `n2`, `n3` | pulso de pista; es el guía quien la ofrece (CP-06) |
| ○ `char_algoritm_n1_anim_aparicion.anim` | escena 1.1 |
| ○ `char_algoritm_n3_anim_apagado.anim` | escena final |

Los otros dos clips de los diez de `S06` son los barridos con muta, y viven en `FX/Animations/`.

### `Characters/Father/` — Papá, jugable en el Nivel 1 (CN-02)

| Archivo | Origen |
|---|---|
| ○ `char_papa_base_apose.png` | `S04` / `A2` |
| ○ `char_papa_retrato_neutra.png` … `_animo.png` (6) | `S03a` |

**`Characters/Father/Animations/`**

| Archivo | Origen |
|---|---|
| ○ `char_papa_anim_idle.anim`, `char_papa_anim_animo.anim` | `S04` — clips universales |
| ○ `char_papa_anim_reubicar.anim`, `_golpear`, `_soplar`, `_celebrar` | `S05` |

### `Characters/Girl/` — la Niña, jugable en el Nivel 2

| Archivo | Origen |
|---|---|
| ○ `char_nina_base_apose.png` | `S08` / `A4` |
| ○ `char_nina_retrato_neutra.png` … `_animo.png` (6) | `S03a` |

**`Characters/Girl/Animations/`** — `S08`

| Archivo |
|---|
| ○ `char_nina_anim_idle.anim`, `_senalar`, `_observar`, `_celebrar`, `_animo` |

### `Characters/Mother/` — Mamá, jugable en el Nivel 3

| Archivo | Origen |
|---|---|
| ○ `char_mama_cenital.png` | `S10` — lámina del rig cenital |
| ○ `char_mama_retrato_neutra.png` … `_animo.png` (6) | `S03a` |
| ○ `char_mama_base_apose.png` | `A3` (Slice 1) — no está en el tablero |

**`Characters/Mother/Animations/`** — `S10`

| Archivo |
|---|
| ○ `char_mama_anim_caminar_norte.anim`, `_sur`, `_este`, `_oeste` |
| ○ `char_mama_anim_idle.anim`, `_recoger`, `_celebrar`, `_animo` |

### `Characters/Boy/` — el Niño, acompaña, no jugable

| Archivo | Origen |
|---|---|
| ○ `char_nino_retrato_neutra.png` … `_animo.png` (6) | `S03a` |
| ○ `char_nino_base_apose.png` | `A5` (Slice 1) — no está en el tablero |

**`Characters/Boy/Animations/`** — vacío. §13.3 pide idle para todos los personajes; el tablero
no programa ningún clip suyo. Ver «Puntos abiertos».

---

## `Environments/`

### `Environments/Fire/`

| Archivo | Origen |
|---|---|
| ○ `env_n1_cueva_luz1.png` … `env_n1_cueva_luz4.png` | `A6` (Slice 1) — ver punto abierto 4 |

### `Environments/Wheel/`

| Archivo | Origen |
|---|---|
| ✓ `env_n2_bosque_claro.png` — 1599×899, **provisional** (11/09/2026). Importado `Single`, `maxTextureSize` 8192: el definitivo se sustituye por nombre y cubre la pantalla sin tocar nada (`IllustrationFraming`) | `B1` (Slice 2) |
| ○ `env_n2_taller.png` | `B4` |
| ○ `env_n2_tablero.png` | `B8` |
| ○ `env_n2_refugio_noche.png` | `S16b` — refugio con fuego encendido, 21:9 |

**`Environments/Wheel/Animations/`** · ○ `env_n2_nubes.anim` (`S12`, deriva lenta)

### `Environments/River/`

| Archivo | Origen |
|---|---|
| ○ `env_n3_rio.png` | `C1` (Slice 3) — vista superior |
| ○ `env_n3_rio_lateral.png` | `S16c` |
| ○ `env_n3_espuma.png` | `S11a` — 4 frames |
| ○ `env_n3_zona_inactiva.png`, `env_n3_zona_disponible.png` | `C6` |

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
| ○ `env_final_fogatas.png` | `C10` (Slice 3) |

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

### `Props/Wheel/`

| Archivo | Origen |
|---|---|
| ✓ `prop_n2_tronco_a.png` … `_e` (5, los válidos) | `B2` (Slice 2) — **provisional** |
| ✓ `prop_n2_piedra_a.png` … `_d` · `prop_n2_planta_a.png` … `_c` · `prop_n2_herramienta_a.png` … `_c` | `B2` — distractores, **provisionales** |
| ✓ `prop_n2_caja_suelo.png` — **provisional** (generado por código, W07, 11/09/2026); ○ `_sobre_troncos`, `_rodando` — hoy los tres estados usan el mismo sprite | `B3` |
| ○ `prop_n2_pieza_1.png` … `_6` | `B5` |
| ○ `prop_n2_carretilla_e1.png` … `_e5` | `B6` |
| ○ `prop_n2_carretilla_cenital_norte.png`, `_este`, `_sur`, `_oeste` | `B7` |
| ○ `prop_n2_obstaculo_piedra.png`, `_curva`, `_pendiente` | `B8` |

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
| ○ `prop_n3_troncos.png`, `_sogas`, `_tela`, `_mastil` | `C4` (Slice 3) |
| ○ `prop_n3_balsa_base.png`, `_amarre`, `_vela` | `C7` |
| ○ `prop_n3_balsa_hundida.png`, `_cruzando` | `C9` |

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
| ○ `ui_dialogo_marco.png`, `ui_dialogo_continuar.png`, `ui_dialogo_omitir.png` | `A10` (Slice 1), reutilizado por los cuatro slices |
| ○ `ui_transicion_fundido.png`, `ui_cortinilla_tablilla.png` | `S02` — sistema de transición |
| ○ `ui_estado_aceptado.png`, `ui_estado_devuelto.png` | `B10` (Slice 2) |
| ○ `ui_ind_intentos.png`, `_errores`, `_pasos`, `_tiempo` | `D1` (Slice 4) — solo informe docente, RF-46 |
| ○ `ui_teacherreport_maqueta.png` | `D2` |
| ○ `ui_dialogo_eliminar.png` | `D3` |

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
| ○ `ui_n3_lista_marco.png`, `ui_n3_casilla_*`, `ui_n3_inventario.png` | `C5` — única lista permanente del juego (INC-41) |
| ○ `ui_n3_panel_marco.png`, `ui_n3_espacio_*` | `C8` |

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
| ○ `fx_n1_humo.anim` | `S07a` |
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
7. **`Characters/Boy/Animations/` está vacío:** §13.3 pide idle para todos los personajes y el
   tablero no programa ningún clip del Niño.
8. **El tablero salta de `S12` a `S15`:** no hay `S13` ni `S14`. Verificar si faltan o si son
   tareas que no producen archivos.
9. Los tramos escritos con `…` (`prop_n2_piedra_a` … `_d`, `ui_n3_casilla_*`, `ui_n3_espacio_*`)
   salen de la **cantidad** que declara el prompt del slice, no de una lista literal. El tablero
   tampoco los enumera. Se fijan al generar la lámina.
