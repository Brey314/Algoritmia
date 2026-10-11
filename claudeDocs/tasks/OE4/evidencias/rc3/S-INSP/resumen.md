# S-INSP · rc3 · resumen (10/10/2026, HEAD f49ccc9; 399e149 solo cambia el .docx)

Sin Unity, ejecutable, pantalla ni escritura fuera de `evidencias/rc3/S-INSP/`. Veredictos en `veredictos.csv` (23 filas), defectos en `defectos.md`.

| Veredicto | N | Casos |
|---|---|---|
| P | 9 | PF-RNF01-01; INSP-RNF16-ASM, INSP-RNF08-10-RED, INSP-CT06, INSP-PERSIST, INSP-CP02, INSP-CP03, CT-10, SUITE-T05 |
| F | 3 | PF-RNF15-01 (I01), PF-RNF17-01 (I02), INSP-ART-SPRITES (I03) |
| B | 10 | PF-RNF02, 03, 16, 18, 19, 20, 21, 22, 23, 10 |
| NE | 1 | PF-RNF09-01 (S-DOC, exe) |

## SUITE
- Corrida confirmada: `suite2.ps1` del 10/10, 25,9 min, 1.167 pruebas. EditMode 748/749 + 1 omitida de entorno; PlayMode 417/418; falla `RiverLevel_RNF21` (0,056 > 0,05, CPU compartida), pasa sola 1/1. Coincide con `Personajes-Resultados.md` (C.15) y con `suite-10oct-resumen.md`.
- **Commit de la corrida:** HEAD `292b108` + árbol `b2a4d015fe44`, registrada en `23d7e8c`. El mensaje de `fd9adf7` no trae cifras (solo `Personajes-Resultados`).
- `23d7e8c`→HEAD en `Assets/`: sin código ni pruebas; sí `Level2_Maze.unity` (reserialización) y 6 PNG de cara sin referencias borrados (DEF-RC3-I04).
- Por prueba: RNF-02 6/6, RNF-03 34/34, RNF-16 8/8, RNF-19 12/12, RNF-20 7/7, RNF-21 23/24, CP-02 23/23, CP-03 8/8, RNF-10 11/11, RNF-23 25/25.

## INSP
- RNF-01: 477 oraciones de lo que ve el estudiante, máximo 20 palabras; tecnicismos explicados.
- RNF-16/08/10/CT-06/CP-02/CP-03/persistencia: sin hallazgos (asmdef, escenas, código y `Game.*.dll` de rc3).
- Observaciones RNF-10: `SENTIS_ANALYTICS_ENABLED` en defines de Standalone (ruido conocido); rc3 trae `DirectML.dll`, `D3D12/` y módulos de red de Unity que nada del juego llama.
- RNF-15: 3 de 10 clases con identificadores en español (I01). RNF-17: 9 de 122 commits con tarjeta (I02).
- RNF-19/20/21 con `arte_check.py` sobre capturas del Editor del 10/10 y los sprites: 44/46 parejas en gris (las 2 restantes, cubiertas por icono 5,6:1 y 13,8:1); contraste ≥ 4,5:1 en todas las superficies medidas (título blanco con contorno: 15,4:1 y 9,1:1); fuego y humo ≤ 1 destello/s; `FireGlow` solo escala.
- CT-10: 47/47 RF con prueba (mín. 1: RF-21, RF-27); 136 casos cubren RF-01..47 y RNF-01..23.

## Sin el ejecutable o sin Santiago (B)
- PF-RNF02-01 (teclas), PF-RNF03-01, PF-RNF19-01, PF-RNF20-01 (resumen, créditos, informe) y PF-RNF21-01 (ráfaga): capturas del exe; las listas `rnf19-20-21/*.txt` se reutilizan tal cual.
- PF-RNF10-01 (`red.csv`), PF-RNF09-01 (JSON de las sesiones).
- PF-RNF16-01 (dos copias con `unity test`) y PF-RNF18-01 (Editor y Play): requieren Unity.
- PF-RNF22-01 y PF-RNF23-01: parte INSP lista, falta lo de Santiago (confirmación, autorizaciones).
