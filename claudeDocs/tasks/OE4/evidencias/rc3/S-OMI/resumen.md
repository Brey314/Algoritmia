# S-OMI · rc3 · resumen

- **Versión:** rc3, HEAD `f49ccc9`, SHA-256 `6cd16edf…adab`. 10/10/2026, 19:46–20:05. Perfil semilla `OE4_Z` (dos lanzamientos: `S-OMI-rc3` pasos 1–9 y `S-OMI2-rc3` para el control de teclas y el paso 5 repetido).
- **Casos:** 8. **Veredictos: P 7 · PD 0 · F 1 · B 0 · NA 0.** El F es PF-RF45-05 (DEF-RC3-09, Menor).

## Pasos
- 1–2: tres «Omitir» (apertura, aparición, hallazgo) → cueva. 3: cuatro golpes efectivos, luz 16,5 → 22,2 → 27,0 → 30,9 → 34,2.
- 4: doble clic en «Soplar» a 100 ms: una ignición, mandos atenuados sin candado, ninguna narrativa duplicada, log limpio.
- 5: doble clic en «Omitir» de `N1_NacimientoDelFuego` en tres intentos: con el cursor en (0,65; 0,92) y (0,65; 0,905) se llega al menú de niveles (el resumen apenas aparece); con (0,65; 0,93) se queda el resumen. **Causa por geometría:** el resumen carga en 13 ms y su botón «Volver al menú de niveles» ocupa x 0,40–0,65, y 0,84–0,92, justo bajo el segundo clic. No reaparece el defecto de `Leave()`.
- 6: resumen con las viñetas de I 7 y C 3; `Datos/OE4_Z.json` idéntico a la semilla.
- 7–9: el N3 con cuatro «Omitir»; recolección y armado de base y amarre; mástil y vela cruzados → 3.2 con «Omitir» → vuelve al ensamblaje; acierto → `N3_Escena33_Cruce` y `N3_EscenaFinal` sin «Omitir».

## Observaciones sin defecto
- Tras «Soplar» la narrativa de cierre entra a ~1,3 s; el caso habla de ~3,5 s de subida de la luz.
- La luz sigue subiendo con el 4.º golpe efectivo (+3,3); el máximo posterior no se pudo medir (⚠ PF-RF21-02).
- Resumen del N1: el avatar «Algoritm» sigue siendo el círculo con texto (DEF-RC3-01, ya abierto).

## Medidas y límites
- RNF-04: `Narrative` 0,012–0,259 s, `Level1_Cave` 0,032 s, `LevelSummary` 0,013 s, `Level3_River` 0,020 s. Sin `Exception`.
- PF-RNF02-01 (control de teclas) en la cueva: ver `S-OMI/PF-RNF02-01_cueva_1.png` y `S-RNF/veredictos-exe.csv`.
- Ráfaga con el arnés (~1,3 s de retraso): no hay cuadro del instante de la ignición.

## Defectos
- DEF-RC3-09 (Menor, PF-RF45-05). Detalle en `../defectos.md`.
