# Inventario de la entrega del 10/10/2026 (expresiones y tarjeta del diálogo; INC-148, INC-149)

Entrega: `claudeDocs/tasks/Personajes/entregas/2026-10-10/` (commit `636467a`), 45 PNG RGBA de 1300×1500 en LFS.
Es la tarjeta D11-11 de Sofía Valentina Giraldo Segovia. Hoja de contacto: [`hoja_contacto.png`](hoja_contacto.png).
Lo medí con Pillow sobre los píxeles, fuera del Editor. Las cajas se toman con alfa ≥ 12, porque por debajo de ese
valor hay halos sueltos casi invisibles; por ejemplo, la Niña tiene píxeles con alfa < 12 en x ≈ 73.

## Resumen

| | Papá | Mamá | Niña | Niño | Algoritm |
|---|---|---|---|---|---|
| Neutra de frente | sí, idéntica a la del 06/10 | sí, idéntica | sí, idéntica | sí, idéntica | sí, nueva |
| Concentración · preocupación · sorpresa | sí · sí · sí | sí · sí · sí | sí · sí · sí | sí · sí · sí | sí · sí · sí |
| Boca de hablar | 1 (abierta) | 1 | 1 | 1 | 1 |
| Ojos cerrados **de frente** | **sí** (nuevo) | **sí** | **sí** | **sí** | **sí** |
| Cara de perfil, abierta y cerrada | idénticas a las del 09/10 | idénticas | idénticas | idénticas | no tiene perfil |
| **Alegría · sueño** | **no** | **no** | **no** | **no** | **no** |
| Personaje entero para la tarjeta | `Dialogos/dialogo_papa` | `dialogo_mama` | `dialogo_niña` | `dialogo_niño` | una por forma en `Algoritm/Dialogos/` |

**Lo que se sigue de la tabla:**

1. **El frente ya parpadea.** Llegan los ojos cerrados de frente que pedía INC-135.
2. **Algoritm trae una sola cara para sus tres formas**, que sustituye la provisional. Sus tres `registro` son
   idénticos, así que una sola caja de cara vale para todas.
3. **Faltan alegría y sueño.** Los datos marcan `Happy` donde el guion lo pide, y se verá la cara neutra hasta que
   llegue `alegria`. `Sleeping` usa los ojos cerrados. Hay que pedírselas a la artista.
4. **Una sola boca de hablar.** El habla alterna entre boca abierta y cerrada.
5. **El perfil no cambia.** Solo se rehacen sus capas a 256 px como máximo, por la regla de Santiago (INC-149).

## Nombres: de la entrega a su destino

Los nombres de la entrega se casan por fichas NFKD-ASCII, como en el 09/10. Destinos en
`Assets/Game/Art/Characters/<Carpeta>/Expresiones/char_<id>_…`. Cada capa queda recortada a la caja común y a ≤ 256 px.

| Archivo de la entrega (familia, `<x>` = papa, mama, niña o niño) | Emoción | Capas que escribe |
|---|---|---|
| `expresion_<x>_neutro` | neutra | `ojos_neutra`, `boca_0`, `cara_base` |
| `<x>_concentrado` | concentración (`Focused`) | `ojos_concentracion`, `boca_concentracion` |
| `<x>_preocupado` | preocupación (`Worried`) | `ojos_preocupacion`, `boca_preocupacion` |
| `<x>_sorpresa` | sorpresa (`Surprised`) | `ojos_sorpresa`, `boca_sorpresa` |
| `<x>_neutro_boca_abierta` (Papá: `papa_neutro_boca_ abierta`) | boca de hablar | `boca_a` |
| `<x>_neutro_ojos_cerrados` | parpadeo, de frente | `ojos_parpadeo_cerrado` |
| `expresion_neutra_perfil_<x>` (Papá: `exoresion_…`) y `…_ojos_cerrados_perfil_<x>` (Niña: `expresion_perfil _neutra_…`) | perfil | `Perfil/char_<x>_perfil_*`; ya existen y se rehacen a ≤ 256 |

**Algoritm:** `expresion_neutra_algoritm`, `algoritm_concentrado`, `algoritm_preocupado`, `algoritm_sorpresa`,
`algoritm_nuetro_boca_abierta` (sic) y `algoritm_neutro_ojos_cerrados`. Van a
`Algoritm/Expresiones/char_algoritm_{ojos_*,boca_*}`, compartidas por las tres formas y sin el nombre de la forma. El
rubor de los cachetes va con los ojos, porque el rig de Algoritm no tiene `CaraBase`.

## Cajas unión de la cara

Son las cajas de todas las expresiones de frente, en px del lienzo de la entrega, con x1 e y1 exclusivos y alfa ≥ 12.

| | x | y | Lo que la ensancha |
|---|---|---|---|
| Papá | 454–819 | 305–599 | Las cejas de preocupación y sorpresa (y 305); los ojos cerrados (x 454–819); la boca abierta (y 599). |
| Mamá | 379–875 | 438–715 | El rubor marca el ancho; la boca abierta llega a y 715. |
| Niña | 425–884 | 384–669 | El rubor; la boca abierta llega a y 669. |
| Niño | 446–837 | 540–812 | La sorpresa llega a y 812. |
| Algoritm | 338–914 | 373–861 | La boca de la sorpresa llega a y 861. |

## El personaje entero para la tarjeta (`Dialogos/`)

| | Caja (alfa ≥ 12) | Registro con la parte sin cara | ¿Trae cara pintada? |
|---|---|---|---|
| Papá | 69–1167 × 28–1483 | Cubre el 100 % de la cabeza del 06/10; fuera de la cara, la diferencia media es 0,3 | Sí, la neutra |
| Mamá | 162–1110 × 13–1457 | Cubre el 100 %; diferencia fuera de la cara 40,8, por el pelo y los hombros (se revisa en la hoja de retratos) | Sí, la neutra |
| Niña | 172–1174 × 14–1417 | Cubre el 100 %; IoU 0,98 | Sí, la neutra |
| Niño | 266–1034 × 262–1377 | IoU 1,00 | Sí, **con la boca abierta** |
| Algoritm, fuego | 19–1271 × 44–1333 | IoU 0,989 con el torso del 09/10 | Sí, la neutra |
| Algoritm, rueda | 19–1271 × 259–1333 | IoU 0,974 | Sí, la neutra |
| Algoritm, gota | 19–1271 × 180–1333 | IoU 0,986 | Sí, la neutra |

**Consecuencia para el retrato animado.** Antes de ponerle encima los ojos y la boca vivos, la cara pintada se borra
dentro de la caja unión de la cara, cruzada con la máscara de la cabeza (en Algoritm, del torso). Para borrarla se pegan
encima los píxeles de la parte sin cara. Después se hornean la nariz y el rubor (`cara_base`). Así se hace en
`preparar_retrato.py`.

## Memoria

Cada capa de cara y cada retrato pesa como mucho 256 × 256 × 4 B = 256 KB sin comprimir:

- **Familia:** unas 14 capas por personaje (10 de frente y 4 de perfil).
- **Algoritm:** 10 capas.
- **Retratos:** 7 bases y 4 retratos neutros.

El peor caso suma unos 20 MB, frente a los 79 MB de margen de RNF-06 (el build del 09/10 pesa 420,6 MB). Las caras de
frente de hoy llegan hasta 512 px, así que bajarlas a 256 compensa parte del aumento.
