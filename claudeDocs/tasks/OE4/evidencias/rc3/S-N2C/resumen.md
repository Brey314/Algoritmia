# S-N2C · rc3 · resumen

- **Versión:** rc3, HEAD `f49ccc9`, SHA-256 `6cd16edf…adab`. 10/10/2026, 18:10–18:17, continuación de S-N2B (perfil `OE4_B`).
- **Casos:** 15 filas (los 10 de la tabla, PF-RF04-02, PF-RF03-02, PF-RF11-02, PF-RF05-02 y PF-RF05-04 parte N2). **Veredictos: P 15 · PD 0 · F 0 · B 0 · NA 0.**
- **Pasos 1–19 ejecutados.** Tableros aleatorios (dos vistos); la lectura ASCII del paso 2 (`tablero_paso2.txt`) es aproximada y los caminos se planearon sobre la foto. Ejecuciones fallidas 3 (la tercera dio la pista), pasos 10, ediciones 40 (conteo a mano 10 + 2 + 17 + 11 = 40), 327,5 s, `reachedLevel` 3 (`S-N2_OE4_B.json`).

## El laberinto sin tinte (lo que se pidió mirar)
- Medido por píxeles (1920×1080): fondo `#C4A882` en todo el contorno y entre el entorno y el panel; marco del entorno `#6A5A45` (documento `#6A5A44`) de 8 px completo por los cuatro lados; marco de «Tu secuencia» `#6A5A45`; zona de soltar `#E0D4C0`; aro de la pista `#A0320D`. Sin tinte: suelo naranja y setos con el color del arte. Ver DEF-RC3-07.
- Paneles tras la reserialización de f49ccc9: título, zona «Suelta un bloque aquí», cajón «Bloques», «Ejecutar» y pausa sin solapes ni recortes, con el cajón cerrado y abierto, con 0 a 10 bloques; ▼ funciona con la lista larga. El refugio (cueva con la familia) sale del cuadro por el borde derecho del entorno; el marco queda entero.

## Mecánica
- Ejecutar vacío, fallo contra el seto (gira, intenta, vuelve, mensaje y reaparece a ~0,6 s), pista a la tercera, papelera, retirar soltando fuera y reordenar: sin que ningún bloque se pegara al cursor (⚠ de PF-RF34-02 no se produjo). Ejecución ganadora con bloque en curso resaltado; cierre con las frases de abstraer y de algoritmo; resumen sin dígitos; menú con el N3 desbloqueado.

## Medidas y límites
- RNF-04: Level2_Maze 0,025 s; LevelSummary 0,013 s. Memoria, red y registro: los de `S-N2A-rc3`. Sin `Exception`.
- PF-RF34-01 y PF-RF32-01: el paso 13 (▲/▼) se probó una vez, no por separado.
