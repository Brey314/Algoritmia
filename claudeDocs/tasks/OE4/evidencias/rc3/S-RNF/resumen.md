# S-RNF · rc3 · resumen (medidas sobre el ejecutable)

- **Versión:** rc3, HEAD `f49ccc9`, `.exe` `6cd16edf…adab`, carpeta `61612836…bb9f` (sin cambios después de todas las sesiones). 10/10/2026, 17:23–17:58. Equipo 1, 1920×1080.
- **Casos:** 6. **Veredictos: P 4 · NE 2** (PF-RNF07-01 no ejecutado; PF-RNF08-01 es del modo avión de Santiago). `veredictos.csv`.

| Medida | Resultado | Tope |
|---|---|---|
| RNF-06 tamaño | **426.633.111 B = 426,6 MB** (406,9 MiB), sin `Datos/` ni `*_DoNotShip` (`rnf06_tamano.txt`); margen 73,4 MB | < 500 MB |
| RNF-04 carga, peor valor | **0,268 s** (Narrative); Boot→MainMenu ≈ 3,4 s | < 10 s |
| RNF-05 memoria pico | privada **1.094 MiB** (1,15 GB, en Level1_Cave); trabajo 434 MiB | < 2 GB |
| RNF-10 red | 723 muestras (4 lanzamientos): **0** conexiones | 0 externas |

## RNF-04 por escena (`rnf04_tabla.csv`, líneas del `Player.log`)
MainMenu n 16 · 0,123 s | LevelSelect n 13 · 0,035 | Credits n 3 · 0,022 | Narrative n 26 · 0,268 | LevelSummary n 3 · 0,013 | TeacherReport n 3 · 0,025 | Level1_Cave n 4 · 0,038 | Level2_Forest n 2 · 0,019 | Level2_Workshop n 2 · 0,020 | Level2_Maze n 2 · 0,026 | Level3_River n 2 · 0,019.
Las cuatro jugables de N2 y N3 quedan con n = 2 (carga inicial y una por «Reiniciar»), no 3: «Reiniciar» abre antes una confirmación (`reiniciar_confirmacion.png`).

## Límites
- La línea `RNF-04` de `SceneLoader` mide la carga, no el primer cuadro visible; la cota de `Boot` es de `Start-Process` a la primera lectura del log.
- El muestreo de red empieza ~2–8 s después del arranque y no ve conexiones de menos de 2 s.
- Equipo 2 (HUM) y RNF-07/RNF-08 no se midieron.
- Todo el registro de juego (todos los lanzamientos) sin `Exception`/`Error`/`Assert`. Residuos fuera de `Datos/`: solo `Player.log` y claves de `HKCU` de Unity.

## Defectos incidentales
DEF-RC3-05 (iconos de las cabeceras del informe docente sobre el texto), Menor, visto de paso en `rnf_teacher.png`; se comprueba en S-DOC.
