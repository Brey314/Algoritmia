# S-N2A · rc3 · resumen

- **Versión:** rc3, HEAD `f49ccc9`, SHA-256 del `.exe` `6cd16edf…adab`. 10/10/2026, 17:59–18:17. Perfil semilla `OE4_B`, ventana 1920×1080 sin marco. La misma partida siguió por S-N2B y S-N2C (un solo proceso, sesión de arnés `S-N2A-rc3`).
- **Casos:** 13 filas (los 10 de la tabla más PF-RF05-04, PF-RF05-05 y PF-RF05-06, parte N2). **Veredictos: P 13 · PD 0 · F 0 · B 0 · NA 0.**
- **Pasos 1–15 ejecutados.** Cifras N2F1 `3 / 1 / 0` y 154,4 s (T(«Empujar») − T0 ≈ 153 s), iguales a lo previsto (`../S-N2C/S-N2_OE4_B.json`).
- **Narrativas del bosque:** `N2_PuenteI` 3, `N2_PuenteI_Bosque` 9, `N2_Escena21_Bosque` 7 y, tras «Empujar», `N2_Escena22_ElPatron` 7; sin «Omitir». Una foto por línea en las hojas `PF-RF05-04_1_hoja_*.png`.
- **Mecánica:** mensajes de piedra, planta, herramienta (pista al tercer rechazo), ayuda, cinco troncos, caja lejos y sobre los troncos, todos con el texto del caso. Control negativo sin efecto en la mecánica (solo se anima la familia en reposo).

## Lo nuevo del 10/10 que se vio
- **Retrato animado:** todo hablante lleva su retrato con la expresión; boca constante y parpadeo en las ráfagas (`analisis_retrato.txt`). ALGORITM, disco de madera con pantaloneta, con brazos; familia sin brazos.
- **Perfil:** la Niña empuja la caja de perfil en la 2.2; el resto, de frente al hablar. **Sombras:** elipse bajo personajes y troncos; la «se encoge y se aclara» de la caja no se pudo aislar.

## Medidas y registro
- RNF-04: Level2_Forest 0,019 s (n=1 en esta sesión; n=4 en `../S-RNF/rnf04-n2n3.csv`), Level2_Workshop 0,025 s, Level2_Maze 0,025 s, LevelSummary 0,013 s. RNF-05: privada máx 1.081 MiB. RNF-10: 520 muestras sin conexión externa. Log sin `Exception`. Residuos: solo `Player.log` nuevo y los valores de sesión del registro de Unity.

## Defectos y límites
- DEF-RC3-08 (forma del guía en las secuencias puente) y los de `defectos.md`; ningún defecto funcional.
- La línea 1 de la 2.2 no tiene ráfaga de retrato (es del narrador). Los pasos 11 y 12 usaron coordenadas leídas de la captura.
