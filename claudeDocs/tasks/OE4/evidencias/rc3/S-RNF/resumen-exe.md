# S-RNF · rc3 · resumen de los casos de RNF que necesitaban el ejecutable o Unity (10/10/2026)

Complementa `resumen.md` y `veredictos.csv` (los casos de PF-RNF04, 05, 06, 07 parte 1 y 10 de la pasada anterior). Aquí, `veredictos-exe.csv`: **10 casos, P 9 · F 1**.

| Caso | Veredicto | Cifras |
|---|---|---|
| PF-RNF02-01 | P | teclas y rueda sin efecto en las 5 escenas jugables (EXE) y 6/6 en la SUITE |
| PF-RNF03-01 | **F (Menor)** | DEF-RC3-10 (tablilla del bosque desborda) y DEF-RC3-05 (iconos del informe) |
| PF-RNF19-01 | P | 52 parejas, 49 distinguibles en gris; 3 cubiertas por icono y texto |
| PF-RNF20-01 | P | 54 regiones con capturas del exe, 53 ≥ 4,5:1 (la restante es informativa); peor texto 5,9:1 |
| PF-RNF21-01 | P | 0 destellos/s en 70 cuadros de la ignición y en las ráfagas de S-N1 |
| PF-RNF10-01 | P | 4.516 muestras de red de 39 archivos, todas NINGUNA |
| PF-RNF09-01 | P | 14 JSON, solo los campos permitidos |
| PF-RNF07-01 | P | copia a `D:\User\Desktop\Algoritmia prueba\`, usuario estándar, crea `Datos/`, juega hasta la cueva |
| PF-RNF16-01 | P (con desviación, DEF-RC3-12) | sin N2: EditMode 657/663, PlayMode 288/316; sin N1: 690/696 y 329/354 |
| PF-RNF18-01 | P (variante) | `MinimumEffectiveStrikes` 2 y texto nuevo en un exe de la copia B; solo 2 `.asset` cambiados |

## Cómo se hizo
- **RNF-19 y RNF-20:** las listas de S-INSP (`parejas-rnf19-rc3.txt`, `cajas-rnf20-rc3.txt`, `-extra`, `-w3`) se aplicaron con `arte_check.py` a capturas 1920×1080 del ejecutable (carpeta `rnf19-20-21-exe/capturas/`), más las superficies que solo existen en el exe: resumen de nivel, créditos e informe docente. Los recortes de cada caja están en `recortes_rnf20_exe/`; B8 (tablero del laberinto, aleatorio en el exe) se re-apuntó a un seto y un camino de la captura.
- **RNF-21:** `Rafaga rf3_ign 7 10` justo después de «Soplar» (`rnf21-exe-ignicion.md`) y las ráfagas de S-N1 (`rnf21-exe-n1.md`).
- **RNF-07:** el escritorio de este equipo está en `D:\User\Desktop` (el caso dice `%USERPROFILE%\Desktop`). Copia, prueba con el SHA-256 del exe y `Residuos`; la copia se borró.
- **RNF-16 y RNF-18:** solo en `C:\Dev\Algoritmia-B` (los Assets del proyecto no se tocaron). Ver `rnf16/resumen.md`. Los 15 fallos ambientales de `unity test` (640×480) se reprodujeron sin quitar nada: `base-PlayMode-filtrada.xml` (15 de 15 fallan).

## Límites
- RNF-04/05 de esta pasada: máx. de carga 0,259 s y memoria privada 1.090 MiB (`S-PER6`); equipo 2 sigue sin medirse (HUM).
- RNF-18 no se probó con el Editor abierto y Play: no se pudo conducir su Game View con el arnés; «sin recompilación» no es observable con `unity build`.
- Estado final: `Build/Algoritmia` sin cambios (huella de la carpeta `61612836…bb9f`, 253 archivos); `Datos/` restaurado (`OE4_C3.json`); clave de registro del reproductor reimportada desde una copia hecha antes; `C:\Dev\Algoritmia-B` es otra vez copia exacta del proyecto.
