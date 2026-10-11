# S-N1 · rc3 · resumen

- **Versión:** rc3, HEAD `f49ccc9` (árbol limpio), SHA-256 del `.exe` `6cd16edf…adab`; huella de la carpeta `61612836…bb9f`.
- **Fecha:** 10/10/2026, 17:33–17:45. Perfil nuevo `OE4N1` sobre `Datos/` con `OE4_B`; ventana 1920×1080 sin marco.
- **Casos:** 25 (todos los de la tabla de S-N1). **Veredictos: P 25 · PD 0 · F 0 · B 0 · NA 0.** Detalle en `veredictos.csv`.
- **Pasos 1–26 ejecutados.** Ruta completa del reto con las cifras previstas: I 5 · C 1 · P 3, `reachedLevel` 2 y `resolutionSeconds` 225,84 (esperado ≈ 225,4 con 41 s de pausa; `S-N1_OE4N1.json`).
- **Narrativas:** 53 líneas (7 + 11 + 18 + 17), una foto por línea (`sh_*.png`, 5 hojas de contacto con miniatura y tarjeta); sin «Omitir» ni pausa en la primera visita.

## Lo nuevo del 10/10 que se vio
- **Retrato animado (INC-148):** las 18 líneas con hablante llevan retrato con su expresión; ráfagas de 2 s a 10 fps (`PF-RF05-04_analisis_retrato.txt`) muestran boca en movimiento continuo y parpadeo. Familia sin brazos, Algoritm con brazos. Las líneas del narrador no llevan retrato ni nombre.
- **Perfil al moverse:** Apertura 1–6 de perfil caminando; Papá arrodillado de perfil mirando la llama en Hallazgo 15–18 y Nacimiento 1–3; de frente al hablar. Parpadeo de dos cuadros en las ráfagas.
- **Algoritm final:** llama con pantaloneta, brazos detrás, de frente (Aparición 1–11, Nacimiento 9–16).
- **Halo y luz:** en la escena de cierre la luz respira con periodo ≈ 2 s (`PF-RF20-04_n1_llama_rafaga.csv`), sin destellos; la luz de la cueva sube 19,7 → 26,5 → 31,4 → 35,9 y no baja con la fuerza 9.
- **Sombras:** elipse bajo cada personaje; los props pequeños del N1 no muestran sombra apreciable.

## Medidas
- RNF-04: Narrative 0,013–0,268 s, Level1_Cave 0,038 s, LevelSummary 0,013 s. RNF-05: privada máx 1.084 MiB. RNF-10: 356 muestras sin conexión. Log sin `Exception`.

## Defectos (ninguno funcional)
- DEF-RC3-01 (avatar «Algoritm» del resumen sin arte), DEF-RC3-03 (gesto del Niño y el criterio de PF-RF05-05), DEF-RC3-04 (las piezas del N1 se reparten al azar), todas Observación.

## Límites de la observación
- No se capturó el primer cuadro del acercamiento de la cueva (el arnés tarda ~1,3 s); el estado final sí (`m_zoom.png`).
- Los pasos 7–8 se rehicieron leyendo la pantalla: las coordenadas de casos.md no valen porque las piezas cambian de sitio en cada entrada.
- El paso 2 (clic en el centro de la ilustración) se hizo en la repetición del N1, donde ya hay «Omitir».
