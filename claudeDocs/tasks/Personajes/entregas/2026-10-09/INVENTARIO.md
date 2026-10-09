# Inventario de la entrega de perfil del 09/10/2026 (INC-134, INC-135)

Entrega: `claudeDocs/tasks/Personajes/entregas/2026-10-09/` (commit `3efa765`), 48 PNG en LFS: partes de **perfil** de
Papá, Mamá, Niña y Niño y una expresión de perfil con **ojos abiertos** y otra con **ojos cerrados** por personaje.
Inventario de la Fase 1 de `Plan: personajes de perfil al moverse y parpadeo` (09/10/2026). Hoja de contacto:
[`hoja_contacto.png`](hoja_contacto.png). Medido con Pillow sobre los píxeles (`git lfs pull`), fuera del Editor.

Convenciones: coordenadas en px del **lienzo de la entrega** (1300×1500, origen arriba a la izquierda, y hacia abajo);
recuadro opaco = caja de los píxeles con alfa ≥ 12, con x1 e y1 exclusivos; «tamaño normalizado» = ese recuadro a la
escala que deja la figura de perfil en 870 px (la misma regla que el frontal, `preparar_arte_final.py`).

## Resumen

| | Papá | Mamá | Niña | Niño |
|---|---|---|---|---|
| Partes de perfil | 10/10 | 10/10 | **9/10 (falta `mano_atras`)** | 10/10 (en `Perfil/Niño/`) |
| Lienzo común registrado | sí, 1300×1500 | sí | sí | sí |
| Mira hacia | **izquierda** | **izquierda** | **izquierda** | **izquierda** |
| Expresión de perfil, ojos abiertos / cerrados | sí / sí | sí / sí | sí / sí | sí / sí |
| Cara registrada sobre la cabeza de perfil | sí (100 % dentro de la silueta) | sí | sí | sí |
| Ojos cerrados **de frente** | **no** | **no** | **no** | **no** |
| Alto de la figura de perfil en la entrega | 1420 px | 1390 px | 1454 px | 1416 px |
| Alto del frontal del 06/10 en su entrega | 1455 px | 1444 px | 1403 px | 1115 px |
| Escala a 870 px (perfil / frontal) | 0,6127 / 0,5979 | 0,6259 / 0,6025 | 0,5983 / 0,6201 | 0,6144 / **0,7803** |
| Memoria RGBA sin comprimir (partes + cara) | 2,11 MB | 2,12 MB | 2,09 MB (+0,05 si se sintetiza la mano) | 2,35 MB |

**Lo que cambia el plan:**

1. **Los cuatro perfiles miran a la izquierda.** La vista canónica del plan es la derecha: `preparar_perfil.py` tiene que
   voltearlos (espejo horizontal de cada PNG y de las coordenadas) antes de escribir. No hay perfil derecho en la entrega.
2. **La Niña no trae `mano_atras`** (antebrazo + mano lejano). Decisión pendiente (ver «Decisiones»).
3. **No hay ojos cerrados de frente** en esta entrega ni en `Assets/` (cada `Expresiones/` solo tiene `ojos_neutra`,
   `boca_0` y `cara_base`). Con INC-135, el perfil parpadea; el frente no podrá parpadear hasta que llegue
   `Expresiones/char_<x>_ojos_parpadeo_cerrado` frontal.
4. **El registro del perfil no es el del frontal.** Los dos lienzos miden 1300×1500, pero no comparten escala ni sitio
   (el Niño de perfil mide 1416 px y su frontal 1115 px). El perfil se normaliza por su cuenta (870 px de figura), no con el
   `registro` del frontal guardado en `arte_final.json`.
5. **`separa()` de `preparar_expresion.py` no sirve tal cual para el perfil** (ver «Expresiones»): hace falta una
   separación de perfil.

## Estructura de la entrega

```
2026-10-09/
├─ Papá/ {Perfil/ (10), Expresiones/ (2)}
├─ Mamá/ {Perfil/ (10), Expresiones/ (2)}
├─ Niña/ {Perfil/ (9),  Expresiones/ (2)}
└─ Niño/ {Perfil/Niño/ (10)  ← carpeta anidada de más, Expresiones/ (2)}
```

No hay carpeta `Frente/` (el frontal sigue siendo el del 06/10). Los nombres de archivo y carpeta están en Unicode NFC
(`á`, `ñ` precompuestas). Todos los PNG son RGBA de 1300×1500, el mismo lienzo para las partes y las caras de cada personaje.

## Piezas por personaje

Papel por nombre: `torso`, `cabeza` (cara vacía: ojos, cejas, boca y rubor llegan en la expresión; la nariz y la oreja
**sí** están en la cabeza), `brazo` = húmero, `mano` = antebrazo con la mano, `muslo` (en la Niña, `pierna`) = muslo,
`pie` = pierna con el pie; `atras` = lejano, `frente` = cercano. Las piezas `atras` son la **misma silueta** que su
`frente` (mismo recuadro y casi los mismos píxeles opacos) pintada en un único tono de sombra plano (`#DE9563`, con el
contorno negro), mientras que las `frente` llevan luz y sombra (`#FFC69F` + `#DE9563`). En reposo la pieza lejana queda
entera detrás de la cercana; solo asoma al caminar.

#### Papá (`papa`)

| Archivo (bajo `Papá/`) | Papel en la entrega | Destino | Recuadro opaco (x0, y0, x1, y1) | Px opacos | Tamaño normalizado |
|---|---|---|---|---:|---:|
| `Perfil/brazo_atras_papa.png` | húmero, lejano (tono de sombra plano) | `char_papa_perfil_brazo_lejano` | (601, 669, 734, 995) | 34629 | 82×200 |
| `Perfil/brazo_frente_papa.png` | húmero, cercano | `char_papa_perfil_brazo_cercano` | (601, 669, 734, 995) | 34629 | 82×200 |
| `Perfil/cabeza_perfil_papa.png` | cabeza, cara vacía | `char_papa_perfil_cabeza` | (338, 20, 953, 826) | 295685 | 377×494 |
| `Perfil/mano_atras_papa.png` | antebrazo + mano, lejano (tono de sombra plano) | `char_papa_perfil_antebrazo_lejano` | (596, 858, 735, 1235) | 34987 | 86×231 |
| `Perfil/mano_frente_papa.png` | antebrazo + mano, cercano | `char_papa_perfil_antebrazo_cercano` | (582, 855, 731, 1243) | 38576 | 92×238 |
| `Perfil/muslo_atras_papa.png` | muslo, lejano (tono de sombra plano) | `char_papa_perfil_pierna_lejana` | (558, 921, 728, 1282) | 47265 | 105×222 |
| `Perfil/muslo_frente_papa.png` | muslo, cercano | `char_papa_perfil_pierna_cercana` | (562, 926, 723, 1280) | 44585 | 99×217 |
| `Perfil/pie_atras_papa.png` | pierna + pie, lejano (tono de sombra plano) | `char_papa_perfil_antepierna_lejana` | (556, 1129, 728, 1440) | 36031 | 106×191 |
| `Perfil/pie_frente_papa.png` | pierna + pie, cercano | `char_papa_perfil_antepierna_cercana` | (557, 1132, 733, 1437) | 35915 | 108×187 |
| `Perfil/torso_perfil_papa.png` | torso (con cuello) | `char_papa_perfil_torso` | (511, 519, 808, 1187) | 117655 | 182×410 |
| `Expresiones/exoresion_neutra_perfil_papa.png` | cara de perfil, ojos abiertos (capa de cara, sin cabeza) | `char_papa_perfil_ojos_neutra` + `…_perfil_boca_0` (sin rubor: no hay `cara_base`) | (403, 377, 610, 561) | 12858 | caja de cara 148×181 |
| `Expresiones/expresion_neutra_ojos_cerrados_perfil_papa.png` | cara de perfil, ojos cerrados (capa de cara, sin cabeza) | `char_papa_perfil_ojos_parpadeo_cerrado` (la boca es la misma de la abierta) | (403, 377, 601, 561) | 7714 | caja de cara 148×181 |

#### Mamá (`mama`)

| Archivo (bajo `Mamá/`) | Papel en la entrega | Destino | Recuadro opaco (x0, y0, x1, y1) | Px opacos | Tamaño normalizado |
|---|---|---|---|---:|---:|
| `Perfil/brazo_atras_mama.png` | húmero, lejano (tono de sombra plano) | `char_mama_perfil_brazo_lejano` | (576, 736, 674, 980) | 19339 | 62×153 |
| `Perfil/brazo_frente_mama.png` | húmero, cercano | `char_mama_perfil_brazo_cercano` | (574, 740, 676, 983) | 19828 | 64×153 |
| `Perfil/cabeza_perfil_mama.png` | cabeza, cara vacía | `char_mama_perfil_cabeza` | (356, 25, 973, 984) | 333768 | 387×601 |
| `Perfil/mano_atras_mama.png` | antebrazo + mano, lejano (tono de sombra plano) | `char_mama_perfil_antebrazo_lejano` | (563, 882, 662, 1152) | 17824 | 62×169 |
| `Perfil/mano_frente_mama.png` | antebrazo + mano, cercano | `char_mama_perfil_antebrazo_cercano` | (566, 887, 669, 1156) | 18511 | 65×169 |
| `Perfil/muslo_atras_mama.png` | muslo, lejano (tono de sombra plano) | `char_mama_perfil_pierna_lejana` | (542, 964, 682, 1260) | 32517 | 88×186 |
| `Perfil/muslo_frente_mama.png` | muslo, cercano | `char_mama_perfil_pierna_cercana` | (542, 964, 682, 1260) | 32515 | 88×186 |
| `Perfil/pie_atras_mama.png` | pierna + pie, lejano (tono de sombra plano) | `char_mama_perfil_antepierna_lejana` | (544, 1149, 691, 1415) | 27857 | 93×167 |
| `Perfil/pie_frente_mama.png` | pierna + pie, cercano | `char_mama_perfil_antepierna_cercana` | (544, 1149, 691, 1415) | 27853 | 93×167 |
| `Perfil/torso_perfil_mama.png` | torso (con cuello) | `char_mama_perfil_torso` | (472, 628, 790, 1153) | 93004 | 200×329 |
| `Expresiones/expresion_neutra_ojos_cerrados_perfil_mama.png` | cara de perfil, ojos cerrados (capa de cara, sin cabeza) | `char_mama_perfil_ojos_parpadeo_cerrado` (la boca es la misma de la abierta) | (431, 483, 647, 682) | 17185 | caja de cara 159×200 |
| `Expresiones/expresion_neutra_perfil_mama.png` | cara de perfil, ojos abiertos (capa de cara, sin cabeza) | `char_mama_perfil_ojos_neutra` + `…_perfil_boca_0` + `…_perfil_cara_base` (rubor) | (431, 483, 649, 682) | 25995 | caja de cara 159×200 |

#### Niña (`nina`)

| Archivo (bajo `Niña/`) | Papel en la entrega | Destino | Recuadro opaco (x0, y0, x1, y1) | Px opacos | Tamaño normalizado |
|---|---|---|---|---:|---:|
| `Perfil/brazo_atras_niña.png` | húmero, lejano (tono de sombra plano) | `char_nina_perfil_brazo_lejano` | (585, 744, 701, 1029) | 26373 | 70×171 |
| `Perfil/brazo_frente_niña.png` | húmero, cercano | `char_nina_perfil_brazo_cercano` | (587, 743, 702, 1021) | 25853 | 69×167 |
| `Perfil/cabeza_perfil_niña.png` | cabeza, cara vacía | `char_nina_perfil_cabeza` | (299, 23, 1078, 756) | 315768 | 467×439 |
| `Perfil/mano_frente_niña.png` | antebrazo + mano, cercano | `char_nina_perfil_antebrazo_cercano` | (576, 923, 693, 1229) | 24030 | 71×184 |
| `Perfil/pie_atras_niña.png` | pierna + pie, lejano (tono de sombra plano) | `char_nina_perfil_antepierna_lejana` | (571, 1219, 715, 1477) | 26630 | 87×155 |
| `Perfil/pie_frente_niña.png` | pierna + pie, cercano | `char_nina_perfil_antepierna_cercana` | (571, 1219, 715, 1477) | 26630 | 87×155 |
| `Perfil/pierna_atras_niña.png` | muslo (se llama «pierna»), lejano (tono de sombra plano) | `char_nina_perfil_pierna_lejana` | (569, 1037, 706, 1330) | 31765 | 82×176 |
| `Perfil/pierna_frente_niña.png` | muslo (se llama «pierna»), cercano | `char_nina_perfil_pierna_cercana` | (569, 1037, 706, 1330) | 31765 | 82×176 |
| `Perfil/torso_perfil_niña.png` | torso (con cuello) | `char_nina_perfil_torso` | (439, 599, 845, 1199) | 120539 | 243×360 |
| `Expresiones/expresion_perfil _neutra_ojos_cerrados_niña.png` | cara de perfil, ojos cerrados (capa de cara, sin cabeza) | `char_nina_perfil_ojos_parpadeo_cerrado` (la boca es la misma de la abierta) | (384, 437, 611, 662) | 19251 | caja de cara 159×217 |
| `Expresiones/expresion_perfil_neutra_niña.png` | cara de perfil, ojos abiertos (capa de cara, sin cabeza) | `char_nina_perfil_ojos_neutra` + `…_perfil_boca_0` + `…_perfil_cara_base` (rubor) | (384, 437, 611, 662) | 27409 | caja de cara 159×217 |

#### Niño (`nino`)

| Archivo (bajo `Niño/`) | Papel en la entrega | Destino | Recuadro opaco (x0, y0, x1, y1) | Px opacos | Tamaño normalizado |
|---|---|---|---|---:|---:|
| `Perfil/Niño/brazo_atras_niño.png` | húmero, lejano (tono de sombra plano) | `char_nino_perfil_brazo_lejano` | (614, 756, 747, 1011) | 27372 | 82×157 |
| `Perfil/Niño/brazo_frente_niño.png` | húmero, cercano | `char_nino_perfil_brazo_cercano` | (614, 757, 747, 1011) | 27176 | 82×157 |
| `Perfil/Niño/cabeza_perfil_niño.png` | cabeza, cara vacía | `char_nino_perfil_cabeza` | (241, 19, 1028, 732) | 425363 | 484×439 |
| `Perfil/Niño/mano_atras_niño.png` | antebrazo + mano, lejano (tono de sombra plano) | `char_nino_perfil_antebrazo_lejano` | (601, 903, 733, 1185) | 25009 | 82×174 |
| `Perfil/Niño/mano_frente_niño.png` | antebrazo + mano, cercano | `char_nino_perfil_antebrazo_cercano` | (601, 901, 736, 1181) | 25393 | 83×173 |
| `Perfil/Niño/muslo_atras_niño.png` | muslo, lejano (tono de sombra plano) | `char_nino_perfil_pierna_lejana` | (570, 1047, 729, 1329) | 35274 | 98×174 |
| `Perfil/Niño/muslo_frente_niño.png` | muslo, cercano | `char_nino_perfil_pierna_cercana` | (570, 1047, 729, 1329) | 35274 | 98×174 |
| `Perfil/Niño/pie_atras_niño.png` | pierna + pie, lejano (tono de sombra plano) | `char_nino_perfil_antepierna_lejana` | (606, 1203, 743, 1435) | 23967 | 85×143 |
| `Perfil/Niño/pie_frente_niño.png` | pierna + pie, cercano | `char_nino_perfil_antepierna_cercana` | (606, 1203, 743, 1435) | 23958 | 85×143 |
| `Perfil/Niño/torso_perfil_niño.png` | torso (con cuello) | `char_nino_perfil_torso` | (444, 603, 893, 1208) | 137358 | 276×372 |
| `Expresiones/expresion_ojos_cerrados_perfil_niño.png` | cara de perfil, ojos cerrados (capa de cara, sin cabeza) | `char_nino_perfil_ojos_parpadeo_cerrado` (la boca es la misma de la abierta) | (380, 429, 626, 696) | 23596 | caja de cara 176×228 |
| `Expresiones/expresion_perfil_neutro_niño.png` | cara de perfil, ojos abiertos (capa de cara, sin cabeza) | `char_nino_perfil_ojos_neutra` + `…_perfil_boca_0` + `…_perfil_cara_base` (rubor) | (380, 429, 626, 696) | 36277 | caja de cara 176×228 |

## Tabla de nombres destino

| Prefijo del archivo de entrega | Lado | Papel | Nodo del rig (`Lienzo/Perfil/Tronco/…`) | Sprite destino (`Assets/Game/Art/Characters/<Carpeta>/Perfil/`) |
|---|---|---|---|---|
| `torso_perfil_` | — | torso con cuello | `Torso` | `char_<id>_perfil_torso` |
| `cabeza_perfil_` | — | cabeza (pelo, oreja, nariz; cara vacía) | `Cuello/Cabeza` | `char_<id>_perfil_cabeza` |
| `brazo_atras_` | lejano | húmero | `BrazoLejano` | `char_<id>_perfil_brazo_lejano` |
| `brazo_frente_` | cercano | húmero | `BrazoCercano` | `char_<id>_perfil_brazo_cercano` |
| `mano_atras_` | lejano | antebrazo + mano | `BrazoLejano/CodoLejano/AntebrazoLejano` | `char_<id>_perfil_antebrazo_lejano` |
| `mano_frente_` | cercano | antebrazo + mano | `BrazoCercano/CodoCercano/AntebrazoCercano` | `char_<id>_perfil_antebrazo_cercano` |
| `muslo_atras_` · `pierna_atras_` (Niña) | lejana | muslo | `PiernaLejana` | `char_<id>_perfil_pierna_lejana` |
| `muslo_frente_` · `pierna_frente_` (Niña) | cercana | muslo | `PiernaCercana` | `char_<id>_perfil_pierna_cercana` |
| `pie_atras_` | lejana | pierna + pie | `PiernaLejana/RodillaLejana/AntepiernaLejana` | `char_<id>_perfil_antepierna_lejana` |
| `pie_frente_` | cercana | pierna + pie | `PiernaCercana/RodillaCercana/AntepiernaCercana` | `char_<id>_perfil_antepierna_cercana` |
| `expresion…` sin `cerrad` | — | ojos + cejas, boca, rubor | `Cuello/Cabeza/{Ojos, Boca, CaraBase}` | `char_<id>_perfil_ojos_neutra`, `char_<id>_perfil_boca_0`, `char_<id>_perfil_cara_base` |
| `expresion…` con `cerrad` | — | ojos cerrados + cejas | `Cuello/Cabeza/Ojos` | `char_<id>_perfil_ojos_parpadeo_cerrado` |

`<id>` = `papa`, `mama`, `nina`, `nino`; `<Carpeta>` = `Father`, `Mother`, `Girl`, `Boy`. Son los nombres que ya usa
`rig_articulaciones.json` (clave `perfil`, en el árbol de trabajo de la herramienta); `char_<id>_perfil_boca_0` no figura
todavía en esa tabla y hay que añadirlo si la boca de perfil va en su propia capa.

## Faltas, sobras y nombres irregulares

| Personaje | Hallazgo | Efecto |
|---|---|---|
| Niña | **Falta `mano_atras`** (antebrazo + mano lejano) | Sin él, el brazo lejano queda sin antebrazo al caminar |
| Niña | Los muslos se llaman `pierna_atras_niña` y `pierna_frente_niña` (no `muslo_`) | `pierna` = muslo; el destino `…_pierna_*` coincide por suerte, pero el parser debe tratar `pierna` como muslo y `pie` como antepierna (igual que `preparar_arte_final.py`) |
| Niña | `expresion_perfil _neutra_ojos_cerrados_niña.png`: **espacio** tras `perfil` | Normalizar espacios antes de partir por `_` |
| Niño | Carpeta anidada de más: `Niño/Perfil/Niño/` | Buscar las piezas recursivamente bajo `Perfil/` |
| Niño | `expresion_perfil_neutro_niño` (**neutro**, masculino) y `expresion_ojos_cerrados_perfil_niño` (sin `neutra`) | No depender de `neutra`: abierta = la que no dice `cerrad` |
| Papá | `exoresion_neutra_perfil_papa.png` (**errata** `exoresion`) | Reconocer la expresión por la carpeta `Expresiones/`, no por el prefijo |
| Todos | El orden de las palabras varía (`expresion_neutra_perfil_…`, `expresion_perfil_neutra_…`); `ñ` y tildes en carpetas y nombres; `papa`/`mama` sin tilde en el archivo y con tilde en la carpeta | Comparar en ASCII sin tildes ni `ñ` (NFKD) y por tokens, no por posición |
| Todos | Ninguna pieza duplicada byte a byte (los md5 difieren todos); `atras` y `frente` comparten silueta, no píxeles | — |
| Todos | No hay `Frente/` ni expresiones frontales nuevas | El frontal sigue siendo el del 06/10; sin parpadeo frontal |

## Hacia dónde mira

Los cuatro miran a la **izquierda** de la pantalla, sin excepción, y todas sus piezas coinciden:

- **Cabeza:** la nariz es el punto más a la izquierda de la cara (Papá x ≈ 339, Mamá ≈ 358, Niña ≈ 301, Niño ≈ 243) y la
  oreja y la masa del pelo (melena de Mamá, coleta de la Niña) quedan a la derecha.
- **Pies:** la punta sale a la izquierda del eje de la pierna (Papá: eje x = 658, punta 559, talón 721; Mamá 627/547/682;
  Niña 652/574/706; Niño 665/609/734).
- **Caras:** la boca y el iris están en el lado izquierdo del recuadro de la expresión; el pliegue de la boca también.

La vista canónica del plan es la **derecha**: `preparar_perfil.py` debe espejar cada PNG (`ImageOps.mirror`) y las
coordenadas (`x' = 1300 − x` en la entrega, o `x' = 1024 − x` en el lienzo del rig) antes de medir o escribir. Así
`CharacterRig.Mirrored` voltea el `Lienzo` solo para la izquierda, como ya hace el runtime de `910c0f0`.

## Expresiones

| Personaje | Ojos abiertos | Ojos cerrados | Rubor | Boca | Ojos cerrados de frente |
|---|---|---|---|---|---|
| Papá | `exoresion_neutra_perfil_papa.png` | `expresion_neutra_ojos_cerrados_perfil_papa.png` | no lleva | trazo en el borde de la barba, (420, 534, 516, 564) | no |
| Mamá | `expresion_neutra_perfil_mama.png` | `expresion_neutra_ojos_cerrados_perfil_mama.png` | sí, (513, 570, 649, 676) | labios, (428, 616, 522, 665) | no |
| Niña | `expresion_perfil_neutra_niña.png` | `expresion_perfil _neutra_ojos_cerrados_niña.png` | sí, (393, 552, 589, 661) | labios, (381, 601, 487, 665) | no |
| Niño | `expresion_perfil_neutro_niño.png` | `expresion_ojos_cerrados_perfil_niño.png` | sí, (435, 551, 620, 691) | sonrisa, (376, 620, 482, 672) | no |

- **Son capas de cara, no cabezas enteras**, en el mismo lienzo que las partes: cejas, ojo, boca y rubor sobre
  transparente. La nariz y la oreja están dibujadas en la cabeza, no en la expresión.
- **Registro:** el 100 % de los píxeles opacos de cada expresión cae dentro de la silueta de la cabeza de perfil, y entre
  el 46 y el 87 % sobre piel (el resto son cejas sobre el flequillo y, en Papá, la boca sobre el borde de la barba). La
  hoja de contacto lo confirma: el ojo y la boca caen en su sitio sin mover nada. Se colocan **por registro**, como el
  frontal (`preparar_expresion.py --registrada`), con la misma transformación que las partes de perfil.
- **Abierta y cerrada comparten todo menos el ojo:** las cejas son idénticas; en la caja de la boca difieren 0 px (Papá,
  Niño), 39 px (Mamá) y 53 px (Niña: el borde de la pestaña inferior). La boca y el rubor salen una sola vez, de la abierta.
- **Rastro en los ojos cerrados:** quedan píxeles casi blancos del ojo abierto bajo los párpados (Niño 241, Mamá 43, Niña
  25, Papá 0). En la hoja de contacto se ve como un arco tenue en el Niño. Conviene limpiarlos en la capa cerrada (alfa a
  cero para los píxeles con R, G, B > 215 que no estén en la abierta como contorno) o pedir que se reexporte.
- **¿Se separan boca y `cara_base`?** Sí, pero **no con `separa()` tal cual**. `separa()` supone una cara de frente: la boca
  es «la componente oscura más ancha de la franja central (±16 % del ancho) por debajo de la mitad» y la nariz «lo pequeño de
  la franja central». De perfil la boca está en el **borde delantero** del recuadro, no en el centro, y no hay nariz en la
  capa. Corrido sobre las ocho imágenes, `separa()` no encuentra la boca en las abiertas de Papá y la Niña y mete el ojo
  entero en `boca` en las de Mamá y el Niño; en las cerradas llama `boca` y `nariz` a los párpados. Regla que sí funciona en
  las ocho (comprobada): **rubor** = píxeles rosados (la misma prueba HSV de `separa`, excluyendo el blanco del ojo);
  **boca** = la componente oscura (dilatada) más grande cuyo centro cae en el 30 % inferior del recuadro de la cara;
  **ojos** = todo lo demás (ojo, párpados y cejas). `cara_base` de perfil = solo el rubor (Papá no tiene: se omite).
  `--asigna boca:…` con las cajas de la tabla también sirve como respaldo.
- **De frente no hay ojos cerrados.** El parpadeo de dos cuadros (INC-135) solo se puede aplicar al perfil; para el frente
  hay que pedir a Santiago `expresion_<x>_ojos_cerrados` frontal en el lienzo del 06/10.

## Articulaciones y pivotes (estimados)

Medidos sobre el alfa: el centro del extremo redondo de cada cápsula (radio = la mitad del ancho máximo en el tercio de ese
extremo). El codo y la rodilla son el punto medio entre el extremo inferior de la pieza de arriba y el extremo superior de
la de abajo; la «holgura» es la distancia entre los dos. El cuello sale del tapón del cuello del torso (el torso **trae el
cuello**: una franja de 75 a 90 px de ancho que sube hasta la barbilla) a la altura de la barbilla o del borde de arriba
del torso, la que esté más abajo. En la columna «rig» la figura ya está normalizada (870 px, coronilla en y = 77, cadera
cercana en x = 512) y **todavía mira a la izquierda**: tras el espejo, `x' = 1024 − x`.

| Articulación | Papá (entrega → rig) | Mamá | Niña | Niño |
|---|---|---|---|---|
| Hombro (cercano = lejano) | (664, 734) → (524, 514) | (622, 790) → (518, 556) | (642, 800) → (514, 542) | (678, 823) → (530, 571) |
| Codo cercano · holgura | (674, 923) → (530, 631) · 24 px | (631, 933) → (524, 645) · 10 px | (650, 969) → (519, 643) · 1 px | (684, 952) → (534, 650) · 3 px |
| Codo lejano · holgura | (677, 922) → (532, 630) · 30 px | (627, 927) → (521, 642) · 18 px | — (falta la mano) | (685, 952) → (534, 650) · 3 px |
| Cadera, extremo redondo del muslo | (644, 1007) → (512, 682) | (612, 1034) → (512, 709) | (638, 1106) → (512, 725) | (649, 1127) → (512, 758) |
| Cadera, borde de arriba del muslo | (642, 929) → (511, 634) | (612, 967) → (512, 667) | (638, 1039) → (512, 685) | (649, 1050) → (512, 710) |
| Rodilla · holgura | (657, 1204) → (520, 802) · 20 px | (626, 1204) → (521, 815) · 5 px | (651, 1274) → (520, 826) · 3 px | (666, 1263) → (523, 841) · 5 px |
| Cuello (tapón del torso) | (672, 560) → (529, 408) | (631, 688) → (524, 492) | (632, 692) → (508, 477) | (658, 716) → (518, 505) |

- **Las piezas se solapan en las articulaciones con extremos redondos**, como en el frontal: húmero y antebrazo comparten
  de 4 600 a 9 600 px opacos en el codo; muslo y pierna, de 7 700 a 13 300 px en la rodilla; el muslo entra bajo la falda
  del torso (15 000 a 29 600 px) y el tapón del cuello del torso entra bajo la cabeza (4 700 a 13 300 px). La de Papá es la
  más floja (holgura de 20 a 30 px en codo y rodilla): `pose_preview.py --mide` debe admitirla como en el frontal
  (`holguras`).
- **Pose de reposo:** brazos caídos y rectos pegados al costado, piernas rectas y juntas; la lejana idéntica a la cercana
  y detrás de ella. Es una pose neutra de la que sale bien un ciclo de paso.
- **Orden de dibujo comprobado** (hoja de contacto): brazo lejano, pierna lejana, torso, pierna cercana, cabeza, brazo
  cercano. Es el de `Lienzo/Perfil/Tronco` en el plan. La cabeza va **delante** del torso: la barba de Papá y la melena de
  Mamá caen sobre el pecho y la espalda, y tapan el tapón del cuello.

## Escala y memoria

| Personaje | Alto de perfil en la entrega | Escala a 870 px | Partes (10 PNG; Niña 9) | Caja de cara normalizada | Cara, 4 capas | Total |
|---|---:|---:|---:|---:|---:|---:|
| Papá | 1420 px (y 20 → 1440) | 0,612676 | 1,68 MB | 148 × 181 | 0,43 MB (0,32 sin `cara_base`) | 2,11 MB |
| Mamá | 1390 px (25 → 1415) | 0,625899 | 1,61 MB | 159 × 200 | 0,51 MB | 2,12 MB |
| Niña | 1454 px (23 → 1477) | 0,598349 | 1,54 MB | 159 × 217 | 0,55 MB | 2,09 MB |
| Niño | 1416 px (19 → 1435) | 0,614407 | 1,71 MB | 176 × 228 | 0,64 MB | 2,35 MB |
| **Total** | | | **6,54 MB** | | **2,13 MB** | **8,68 MB** |

- Cálculo: ancho × alto × 4 bytes (RGBA32), porque `ArtImportRules` importa `Art/` sin comprimir y los `.meta` de las
  partes llevan `enableMipMap: 0`. Sin compresión el peso en el paquete es el mismo que en memoria. La caja de cara aplica
  `MARGEN_CARA` (8 %, 25 %, 8 %, 35 %) al contenido de la expresión abierta, recortada a la cabeza, como `--registrada`.
- Comparación: hoy las partes frontales y las caras de los cuatro ocupan 10,74 MB. La cabeza de perfil es la pieza más
  cara (0,74 a 0,93 MB cada una).
- **Margen de RNF-06:** unos 21 MB (rc2, 479,0 MB). El perfil completo se lleva 8,7 MB. Con la mano lejana sintetizada de
  la Niña (+0,05 MB) y los ojos cerrados frontales cuando lleguen (4 capas de la caja frontal: 0,22 + 0,32 + 0,29 + 0,38 =
  1,21 MB), el total queda en unos **9,9 MB**: cabe, pero deja unos 11 MB de margen. Hay que medirlo en un ejecutable.
- La escala del perfil y la del frontal difieren menos del 4 % en Papá, Mamá y la Niña. En el Niño la diferencia es del
  27 %, porque su frontal se dibujó más pequeño en el lienzo. Normalizar los dos a 870 px de figura deja la misma altura
  en pantalla al girar. La cabeza de perfil queda a un 4 % de la frontal en el Niño (438 frente a 456 px de alto) y un 9 %
  mayor en Papá por la barba (494 frente a 453).

## Decisiones pendientes (para Santiago)

1. **Mano lejana de la Niña.** (a) Pedir `mano_atras_niña.png`; o (b) sintetizarla de `mano_frente_niña.png`, pasando la
   piel clara `#FFC69F` al tono de sombra `#DE9563` (así están hechas las otras doce piezas lejanas: misma silueta en tono
   plano). La (b) desbloquea ya y se sustituye cuando llegue el original.
2. **Ojos cerrados de frente.** Sin ellos el frente no parpadea. (a) Pedirlos a la vez para los cuatro, en el lienzo del
   06/10; o (b) aceptar que, por ahora, solo parpadee el perfil.
3. **Rastro blanco bajo los párpados cerrados** (Niño, sobre todo): limpiarlo en la herramienta o reexportarlo.
4. **Ancla horizontal al girar.** Propuesta: la cadera cercana en x = 512, para que los pies no resbalen al pasar de
   frente a perfil. La alternativa es el centro del recuadro del torso, que en perfil cae de 2 a 12 px más atrás en el rig (4 a 19 px en la entrega), por la falda y la espalda.
5. **Cadera:** el borde de arriba del muslo (el valor por defecto del frontal) o el centro de su extremo redondo, de 40 a
   48 px más abajo en el rig. En perfil la falda tapa los dos.

## Recomendaciones para `preparar_perfil.py`

- **Entrada:** `preparar_perfil.py <id> <carpeta_personaje>`, donde `<carpeta_personaje>` es, por ejemplo,
  `entregas/2026-10-09/Niño`. Las partes se buscan **recursivamente** bajo `Perfil/` (por el `Perfil/Niño/` anidado) y las
  expresiones en `Expresiones/`. La herramienta exige que todas midan lo mismo (1300×1500) y rechaza la entrega si no.
- **Nombres:** se pasan a ASCII en minúsculas (NFKD, sin tildes ni `ñ`) y se parten por `_` y espacios. El papel sale del
  primer token conocido: `torso`, `cabeza`, `brazo` (o `humero`) → húmero; `mano` (o `antebrazo`) → antebrazo;
  `muslo` (o `pierna`) → muslo; `pie` (o `antepierna`) → antepierna. El lado: `atras`, `lejano` o `lejana` → lejano;
  `frente`, `cercano` o `cercana` → cercano. Torso y cabeza no tienen lado, y `perfil` se ignora como token. Hay que
  comprobar el lado con el tono: la lejana es la de menos píxeles `#FFC69F`. En las expresiones, la abierta es la que no
  contiene `cerrad`; `exoresion`, `neutro` y `neutra` no se exigen. Debe avisar de cada pieza que falte y, para el
  antebrazo lejano, ofrecer `--sintetiza-lejana`, que recolorea el cercano.
- **Orientación:** detecta hacia dónde mira la figura (la punta del pie respecto del eje de la pierna; para comprobarlo,
  la nariz como punto más a la izquierda de la cara) y, si mira a la izquierda (hoy los cuatro), espeja todas las capas
  antes de medir. Debe dar un error si las piezas no coinciden entre sí.
- **Limpieza:** igual que `preparar_arte_final.py`: alfa < 12 a cero y solo la componente conexa más grande en las
  partes. En las expresiones **no** se limpia: tienen varias componentes por diseño. En la capa cerrada, opcionalmente, se
  borra el rastro casi blanco.
- **Normalización:** con una escala propia del perfil, `s = 870 / (pie más bajo − coronilla)` sobre la unión de las partes
  (0,6127, 0,6259, 0,5983 y 0,6144). La coronilla va a y = 77, y la cadera cercana (ya espejada) a x = 512, o la ancla que
  decida Santiago. Recorte al recuadro, LANCZOS y densidad 1:1 con el lienzo del rig. La transformación se guarda como
  `perfil.registro` (`escala`, `tx`, `ty`, `lienzo`, `espejo: true`, la caja de la cabeza y la de la cara), aparte del
  `registro` frontal.
- **Medidas:** hombro = extremo superior del húmero; codo y rodilla = punto medio de los extremos que se solapan, con su
  holgura en `perfil.holguras`; cadera = borde de arriba del muslo, o `--cadera capsula`; cuello = tapón del torso a la
  altura de la barbilla o del borde de arriba del torso; pivote del torso = la cadera, porque el plan cuelga todo de
  `Perfil/Tronco` con pivote en la cadera. Los valores de la tabla de arriba sirven de prueba (±3 px tras el espejo).
- **Caras:** la caja común sale de la abierta con `MARGEN_CARA`, recortada a la cabeza. La separación es la de perfil
  descrita arriba, en una función `separa_perfil()` y no tocando `separa()`. Escribe `char_<id>_perfil_ojos_neutra`,
  `char_<id>_perfil_ojos_parpadeo_cerrado`, `char_<id>_perfil_boca_0` y `char_<id>_perfil_cara_base` (solo si hay
  rubor). Todas comparten caja y rect, y van espejadas.
- **Salidas:** solo en `Assets/Game/Art/Characters/<Carpeta>/Perfil/`, nunca en `Frontal/` ni en `Expresiones/`: 10
  partes y de 3 a 4 capas de cara. Además, la clave `perfil` del personaje en `arte_final.json`, con `nodos`, `partes`,
  `holguras` y `registro`, sin tocar la entrada frontal. Después se regeneran `rig_articulaciones.json` y
  `clips_personajes.json`, y se imprime el bloque para la sesión local (`BuildRigsFinal` en modo `perfil`). Sin
  `--aplicar`, informe y composite, como el frontal.
- **`--autoprueba`:** una entrega sintética **mirando a la izquierda**, con el muslo llamado `pierna`, un espacio en un
  nombre, una carpeta anidada y una pieza lejana que falta. La herramienta debe recuperar las articulaciones (≤ 1 px) tras
  el espejo, avisar de la falta y separar la boca del borde delantero.

---
Medido el 09/10/2026 con Pillow 12.3 sobre los PNG de `3efa765` (LFS). No se compiló ni se abrió Unity.
