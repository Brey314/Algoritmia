# S-PER — Persistencia, cierre forzado y robustez (T15 del OE4) — 01/10/2026

- **Exe:** `C:\Dev\Algoritmia\Build\Algoritmia\Algoritmia.exe` (rc1, 479,0 MB; SHA-256 `6CD16EDF…ADAB`, comprobado en cada `Start-Juego`). Editor cerrado (solo Unity Hub).
- **Arnés:** `oe4.ps1`, sesión `S-PER`, ventana sin marco 1920×1080 en (0,0). `OE4_TRABAJO` = esta carpeta.
- **Evidencias:** `claudeDocs/tasks/OE4/evidencias/S-PER/`.
- **Encadenado:** un mismo perfil recorre varios cierres forzados seguidos (cada relanzamiento verifica el anterior y prepara el siguiente).

| # | Hora | Acción | Observado | Veredicto |
|---|---|---|---|---|
| **A** | | **Bloque A — N1 con perfil nuevo `OE4N1b`: cierre en la mecánica (A), en la narrativa de cierre (PF-RNF14-03), Salir y reabrir** | | |
| 1 | 11:23:36 | `Preparar-Datos` (sin perfiles); el `OE4GP2.json` que quedaba es idéntico a `evidencias/GP2/GP2_OE4GP2_final.json` | `Datos/` vacía | OK |
| 2 | 11:23:50 | `Start-Juego S-PER -SinMarco -Sha256 build-rc1.md` + `Esperar-Carga MainMenu` | SHA coincide; pid 49664, muestreador 22304; foto previa de residuos (279 entradas en LocalLow, clave HKCU no existe); MainMenu 0,100 s | OK |
| 3 | 11:24:05–49 | «Jugar» → campo → `Escribir OE4N1b` → «Continuar» | «Perfiles guardados» vacío (sper_001); el campo muestra «OE4N1b» (sper_002); Narrative 0,764 s; `Datos/OE4N1b.json` = `{"name":"OE4N1b","reachedLevel":1,"phases":[]}` (11:24:48,97) | OK |
| 4 | 11:25:52–26:50 | «Continuar» ×7, ×11, ×18 | `N1_Apertura` L1 (sper_003) → `N1_AparicionGuia` → `N1_Hallazgo` L1 «¿Se fue?» (sper_004) → `Level1_Cave` 0,154 s: 5 hojas, sílex, pedernal, tablilla «Reúne todas las hojas…» (sper_005) | OK |
| 5 | 11:27:24–31 | Arrastrar 3 de las 5 hojas al centro | Las 3 hojas juntas en el centro; quedan 2 hojas y las 2 piedras fuera (sper_006 → `S-PER/PF-RNF14-N1mec_antes_de_matar.png`) | OK |
| 6 | 11:27:51 | **`Matar`** (taskkill /F) a mitad de la mecánica del N1 | Proceso terminado. `OE4N1b.json` sin cambios (`{"reachedLevel":1,"phases":[]}`, LastWriteTime 11:24:48,97): no había fase confirmada que guardar | OK |
| 7 | 11:28:10–13 | `Start-Juego S-PER` (lanz. 2) → «Jugar» | MainMenu 0,090 s; **«Perfiles guardados» lista «OE4N1b»** con su papelera (sper_007 → `S-PER/PF-RNF14-N1mec_relanzar_perfil_listado.png`) | P |
| 8 | 11:28:25 | Clic en la fila «OE4N1b» + `Esperar-Carga *` | LevelSelect 0,070 s, sin error: «Nivel 1» habilitado sin «Completado»; «Nivel 2» y «Nivel 3» «Bloqueado» con candado (sper_008 → `PF-RNF14-N1mec_relanzar_menu_niveles.png`) | P |
| 9 | 11:28:48–29:49 | «Nivel 1» → «Continuar» ×7, ×11, ×18 | `N1_Apertura` L1 **sin «Omitir»** (el N1 no se ha terminado, sper_009) → `Level1_Cave` 0,155 s **desde cero**: las 7 piezas regadas en sitios nuevos, ninguna en el centro, tablilla «Reúne todas…» (sper_010 → `PF-RNF14-N1mec_cueva_de_cero_al_retomar.png`). Lo no confirmado (3 hojas en el centro) no se conserva, como corresponde: no era una fase aprobada | **P — cierre forzado a mitad de la mecánica del N1** |
| 10 | 11:30:05–49 | 5 hojas y 2 piedras al centro; fuerza muesca 7, cercanía muesca 5; «Golpear» ×3 | Panel de encendido (sper_011); al 3.º golpe «Un hilo de humo sube despacio…» y «Soplar» habilitado (sper_012) | OK |
| 11 | 11:31:02–12 | «Soplar» + `Esperar-Carga Narrative`; «Continuar» ×4 | `N1_NacimientoDelFuego` 0,385 s; L5 «Los niños salen corriendo y se lanzan encima de él…» (sper_013 → `PF-RNF14-03_antes_de_matar_N1_NacimientoDelFuego.png`). `OE4N1b.json` sigue sin fases (la del N1 se guarda al mostrarse el resumen, RF-04/arquitectura §7) | OK |
| 12 | 11:31:26 | **`Matar`** durante `N1_NacimientoDelFuego` | Proceso terminado. `OE4N1b.json` sin cambios (LastWriteTime 11:24:48,97) | OK |
| 13 | 11:31:29–36 | `Start-Juego` (lanz. 3) → «Jugar» → «OE4N1b» | MainMenu 0,096 s; LevelSelect 0,070 s **sin error**: el N1 sigue por completar (sin «Completado») y el N2 «Bloqueado» (sper_014 → `PF-RNF14-03_relanzar_N1_por_completar.png`; arriba a la derecha, el aviso de NVIDIA, ajeno al juego) | P |
| 14 | 11:31:51–33:41 | «Nivel 1» → narrativas (sin «Omitir») → cueva (T0 = 11:32:25,08) → reunir, muescas 7/5, «Golpear» ×3, «Soplar» | Igual que en 9–11: cueva desde cero (sper_015), panel (sper_016), «Soplar» habilitado al 3.º golpe (sper_017) | OK |
| 15 | 11:33:46–34:03 | `N1_NacimientoDelFuego` (0,369 s) → «Continuar» ×17 → `Esperar-Carga LevelSummary` | Resumen 0,035 s: «Esto es lo que pasó en la cueva», variante sin fallos, sin dígitos (sper_018 → `PF-RNF14-03_resumen_N1_tras_rejugar.png`). `OE4N1b.json` (11:34:00,01) = `reachedLevel` 2 y `{1,1, attempts 0, correctedErrors 0, stepsUsed 3, resolutionSeconds 80,21}`; previsto T ≈ («Soplar» 11:33:41 + 3,5 s) − T0 = 79,4 s (dif. 0,8 s) → `S-PER_OE4N1b_tras_resumen_N1.json` | P |
| 16 | 11:34:19 | «Continuar» del resumen | LevelSelect 0,068 s: **N1 «Completado» con visto, N2 habilitado**, N3 «Bloqueado» (sper_019 → `PF-RNF14-03_menu_N1_completado_N2_desbloqueado.png`) | **P — PF-RNF14-03** |
| 17 | 11:34:39 | «Volver» → inicio. `OE4N1b.json`: LastWriteTime 11:34:00,013, SHA-256 `9fbc2a92…4229` | MainMenu 0,068 s | OK |
| 18 | 11:34:42 | «Salir» | Diálogo «Lo que has logrado ya está guardado. ¿Cerramos el juego?» con «Quedarme» y «Cerrar el juego» (sper_020 → `PF-RF09-02_dialogo_salir.png`). **DEF-SPER-01 (Menor):** el botón «Cerrar el juego» lleva el **icono de la papelera** (el de borrar perfiles) y su rótulo parte en dos líneas que **se salen del botón** por arriba y por abajo (`DEF-SPER-01_boton_cerrar_el_juego_desborda.png`) | P (PF-RF09-02), con DEF-SPER-01 |
| 19 | 11:35:10 | «Quedarme» | Vuelve a la pantalla de inicio (sper_021 → `PF-RF09-02_quedarme_vuelve_al_inicio.png`); `OE4N1b.json` **no cambia** (LastWriteTime 11:34:00,013) | **P — PF-RF09-02** |
| 20 | 11:35:35–38,49 | «Salir» → «Cerrar el juego» (clic 16:35:38,490Z) | `OE4N1b.json` reescrito a las 11:35:38,693 (**LastWriteTime posterior**) con el **mismo contenido** (SHA-256 idéntico). El proceso termina en < 2 s: última muestra del muestreador 16:35:38,235Z, líneas de apagado del Player.log copiadas a las 16:35:39,254Z y ningún proceso `Algoritmia` en la muestra siguiente | **P — PF-RF09-01** |
| 21 | 11:36:41–54 | `Start-Juego` (lanz. 4) → «Salir» → «Cerrar el juego» **sin elegir perfil** | Mismo diálogo (sper_022 → `PF-RF09-03_salir_sin_perfil.png`); el proceso termina en < 4 s. `OE4N1b.json` **no cambia** (LastWriteTime 11:35:38,693) y no aparece ningún otro archivo. (La carpeta `Datos` sí cambia su fecha al pulsar «Jugar» o «Salir», 11:28:12, 11:31:31, 11:36:52, sin dejar archivos: parece la sonda de escritura de INC-34; no modifica ningún dato.) Observación: sin perfil activo, el diálogo dice igual «Lo que has logrado ya está guardado», cuando no hay nada que guardar | **P — PF-RF09-03** |
| 22 | 11:37:39–46 | `Start-Juego` (lanz. 5) → «Jugar» → «OE4N1b» | La lista muestra «OE4N1b» (sper_023 → `S1-cerrar-reabrir_perfil_listado.png`); LevelSelect 0,071 s con **N1 «Completado», N2 habilitado y N3 «Bloqueado»**, el mismo progreso que antes de «Salir» (sper_024 → `S1-cerrar-reabrir_progreso_conservado.png`) | **P — casilla S1 «Cerrar y reabrir conserva el perfil y su progreso»** |
| 23 | 11:38:09 | `Cerrar-Juego` (WM_CLOSE) | Cierre normal; `OE4N1b.json` sin cambios → `S-PER_OE4N1b_final.json` | OK |
| 24 | 11:38:09 | `Revisar-Log S-PER` (lanzamientos 1–5) | Sin Exception, Error ni Assert; solo líneas RNF-04 | OK |
| **B** | | **Bloque B — N2 con `OE4_B`: cierre en la 2.2 (PF-RNF14-01), con la pausa abierta en el taller (PF-RNF14-05), tras confirmar el taller (casilla S2) y durante el cierre del N2 (PF-RNF14-04)** | | |
| 25 | 11:38:45–56 | `Preparar-Datos OE4_B` → `Start-Juego` (lanz. 6) → «Jugar» → «OE4_B» → «Nivel 2» | Narrative 0,667 s: `N2_PuenteI` L1 «Amanece. La familia sale de la cueva…» **sin «Omitir»** (sper_025) | OK |
| 26 | 11:39:14–41:01 | «Continuar» ×3, ×9, ×7 → bosque (T0 11:39:38,54) → 5 troncos → caja sobre los troncos → «Empujar» | `Level2_Forest` 0,052 s (sper_026); «5 de 5», alineados (sper_027); «La caja quedó sobre los troncos. Ahora empújala.» (sper_028); `N2_Escena22_ElPatron` 0,702 s. `OE4_B.json` (11:40:59,75) gana `{2,1, 0,0,0, 81,41}` (previsto 81,2 s) | OK |
| 27 | 11:41:03–32 | «Continuar» ×2 en la 2.2 → **`Matar`** | L3 PAPÁ «Pero esa piedra no…» (sper_029 → `PF-RNF14-01_antes_de_matar_N2_Escena22.png`); proceso terminado; JSON con `{2,1}` → `S-PER_OE4_B_tras_matar_en_2.2.json` | OK |
| 28 | 11:41:35–43:29 | `Start-Juego` (lanz. 7) → «OE4_B» → «Nivel 2» → `N2_PuenteI` (×3) → `N2_PuenteI_Bosque` (×9) → `N2_Escena21_Bosque` (×7) | LevelSelect: N1 «Completado», N2 habilitado (sper_030). **Tres narrativas** (sper_031–033) y **`Level2_Workshop` 0,069 s, directo al taller y no al bosque** (sper_034 → `PF-RNF14-01_relanzar_entra_directo_al_taller.png`); `{2,1}` intacta | **P — PF-RNF14-01** |
| 29 | 11:44:00–23 | Tronco corto A + «Mecanizar» → pausa | «Se abrió un agujero en el centro: el tronco ya es una rueda.» (sper_035 → `PF-RNF14-05_rueda_A_mecanizada.png`); «Pausa» con «Reanudar», «Reiniciar» y «Volver al menú de niveles» (sper_036 → `PF-RNF14-05_pausa_abierta_antes_de_matar.png`) | OK |
| 30 | 11:44:43 | **`Matar`** con la pausa abierta | Proceso terminado; `OE4_B.json` sin cambios (LastWriteTime 11:40:59,75) | OK |
| 31 | 11:44:46–45:16 | `Start-Juego` (lanz. 8) → «OE4_B» → «Nivel 2» → tres narrativas | `Level2_Workshop` 0,069 s **desde cero**: el tronco A sin agujero, las 7 piezas en fila, «Mecanizar» con candado y «Aún no» (sper_037 → `PF-RNF14-05_relanzar_taller_de_cero.png`); `{2,1}` intacta | **P — PF-RNF14-05** (con `OE4_B` + fase 2.1 jugada en vez de la semilla `OE4_B2`: mismo estado) |
| 32 | 11:45:39–46:52 | Taller completo (T0 11:45:13,70): A y B «Mecanizar», eje, tabla, caja, cuerda | «…ya hay un eje.» (sper_038), «La caja va sobre la tabla.» (sper_039), cuerda (sper_040); `OE4_B.json` (11:46:51,19) gana `{2,2, 0,0,6, 97,66}` (previsto 97,5 s); `N2_Escena24_Regreso` 0,702 s | OK |
| 33 | 11:46:53–47:16 | «Continuar» ×2 en la 2.4 → **`Matar`** | L3 PAPÁ «Empujar…» (sper_041 → `S2-tras-fase2_antes_de_matar_N2_Escena24.png`); JSON → `S-PER_OE4_B_tras_confirmar_taller.json` | OK |
| 34 | 11:47:19–48:26 | `Start-Juego` (lanz. 9) → «OE4_B» → «Nivel 2» → tres narrativas | LevelSelect como antes (sper_042); `N2_PuenteI` sin «Omitir» (sper_043) y **`Level2_Maze` 0,054 s: retoma en la fase 3**, tablero nuevo, «Tu secuencia» vacía (sper_044 → `S2-tras-fase2_relanzar_retoma_en_el_laberinto.png`). Las fases 2.1 y 2.2 intactas | **P — casilla S2 «Cierre forzado tras confirmar la fase 2 → retoma en la fase 3»** |
| 35 | 11:48:30–55:24 | Transcripción del tablero (muestreo de la rejilla 16×11) y secuencia de 13 bloques solo desde el cajón: A5, G‹, A1, G›, A2, G›, A1, G‹, A6, G›, A6, G‹, A2 | Bloques comprobados en fotos (sper_046–056, `crop_b5_b7`, `crop_b8_b10`, `crop_b11_b13`); cajón cerrado (sper_057 → `PF-RNF14-04_secuencia_laberinto.png`) | OK |
| 36 | 11:55:48–56:11 | «Ejecutar» | Llega al refugio a la primera; `OE4_B.json` (11:56:10,61) gana `{2,3, 0,0,13, 466,48}` (T0 11:48:24,20 → 466,4 s); `reachedLevel` sigue en 2; `N2_Escena25_Cierre` 0,687 s | OK |
| 37 | 11:56:20–41 | «Continuar» ×2 en la 2.5 → **`Matar`** | L3 ALGORITM «Las grandes ideas nacen cuando observas, comparas y organizas.» (sper_059 → `PF-RNF14-04_antes_de_matar_N2_Escena25.png`); JSON idéntico (SHA-256 `9ad18f50…50b5`) → `S-PER_OE4_B_tras_laberinto.json` | OK |
| 38 | 11:56:44–51 | `Start-Juego` (lanz. 10) → «OE4_B» | LevelSelect: **N1 y N2 «Completado» y N3 desbloqueado** sin candado, aunque el JSON dice `reachedLevel` 2 (INC-116: el menú lo deriva de las fases) (sper_060 → `PF-RNF14-04_relanzar_N2_completado_N3_desbloqueado.png`) | P |
| 39 | 11:57:17–58:15 | «Nivel 3» → `N3_PuenteII` (×2) → `_Horizonte` (×5) → `_Rio` (×4) → `N3_Escena31_Llegada` (×9) | Narrativas sin «Omitir» (sper_061, sper_062) → `Level3_River` 0,086 s: la orilla con la recolección, lista de 4 tareas, inventario vacío, flechas (sper_063 → `PF-RNF14-04_N3_jugable_orilla_recoleccion.png`). (La 3.1 pidió un «Continuar» más de los que se contaron: sin efecto) | P |
| 40 | 11:58:44–59:41 | Pausa → «Volver al menú de niveles» → «Nivel 2» → «Omitir» ×3 | LevelSelect 0,067 s; `N2_PuenteI` **con «Omitir»** (el N2 está terminado, sper_065 → `PF-RNF14-04_rejugar_N2_narrativas_con_omitir.png`); cada «Omitir» salta una secuencia hasta `Level2_Forest` | OK |
| 41 | 11:59:55–04:30 | Rejugar bosque, taller (con «Omitir» en 2.2, 2.3 y 2.4) y un laberinto nuevo (9 bloques: A7, G›, A7, G‹, A2, G‹, A1, G›, A6; `crop_c1_c3`…`crop_c7_c9`) | Todo a la primera; tras cada fase el JSON **no cambia**: los indicadores solo se guardan la primera vez (`PlayerProfile.ConfirmPhase`) | OK |
| 42 | 12:04:30 | Llegada al refugio → `N2_Escena25_Cierre` | 0,704 s; **sin «Omitir»** (su resumen aún no se había mostrado, RF-12) (sper_081 → `PF-RNF14-04_N2_Escena25_sin_omitir.png`) | P |
| 43 | 12:05:03–13 | «Continuar» ×6 → resumen | `LevelSummary` 0,035 s: «Esto es lo que pasó con la rueda», variante sin errores, sin dígitos (sper_082 → `PF-RNF14-04_resumen_N2.png`). `OE4_B.json` (12:05:10,34): `reachedLevel` **3** y las cuatro fases **con sus indicadores sin tocar** → `S-PER_OE4_B_final.json` | **P — PF-RNF14-04** (con `OE4_B` jugado en vez de `OE4_B3`) |
| 44 | 12:05:29 | `Cerrar-Juego`; `Revisar-Log S-PER` (lanz. 1–10) | Cierre normal; sin Exception, Error ni Assert | OK |
| **C** | | **Bloque C — N3 con `OE4_E`: cierre durante el ensamblaje (PF-RNF14-02)** | | |
| 45 | 12:06:09–58 | `Preparar-Datos OE4_E` → `Start-Juego` (lanz. 11) → «OE4_E» → «Nivel 3» → `N3_PuenteII` (×2), `_Horizonte` (×5), `_Rio` (×4), `N3_Escena31_Llegada` (×9) | Narrativas sin «Omitir» (sper_084); `Level3_River` 0,053 s **directo al ensamblaje en mástil y vela**: base de 5 troncos y amarres hechos, tareas 1–3 con visto, tela y mástil en el inventario, siluetas de mástil y vela, Mamá en la zona, «Coloca lo que atrapa el viento y pulsa «Probar balsa».» (sper_085 → `PF-RNF14-02_ensamblaje_mastil_y_vela_al_entrar.png`) | OK |
| 46 | 12:07:28–08:02 | Arrastrar el mástil a su silueta → **`Matar`** | «Puesto. Cuando toda esta parte esté, pulsa el botón.»; el mástil en la balsa y su casilla vacía (sper_086 → `PF-RNF14-02_mastil_puesto_antes_de_matar.png`). Proceso terminado; `OE4_E.json` idéntico a la semilla | OK |
| 47 | 12:08:05–39 | `Start-Juego` (lanz. 12) → «OE4_E» → «Nivel 3» → cuatro narrativas | `Level3_River` 0,053 s: **vuelve al ensamblaje en mástil y vela** con la base y los amarres armados, las tareas 1–3 marcadas, **tela y mástil otra vez en el inventario** (el mástil puesto no estaba confirmado) y Mamá esperando en la zona (sper_087 → `PF-RNF14-02_relanzar_retoma_en_mastil_y_vela.png`) | **P — PF-RNF14-02** |
| 48 | 12:09 | `Cerrar-Juego` | Cierre normal; JSON → `S-PER_OE4_E_final.json` (= semilla) | OK |
| **D** | | **Bloque D — robustez: perfil ilegible (PF-RF02-04) y nombre reservado (PF-RF02-05)** | | |
| 49 | 12:09:05–11 | `Preparar-Datos OE4_B OE4_Corrupto` → `Start-Juego` (lanz. 13) → «Jugar» | La lista muestra «OE4_B» y «OE4_Corrupto», cada uno con su papelera (sper_088 → `PF-RF02-04_lista_con_corrupto.png`) | OK |
| 50 | 12:09:39 | Clic en «OE4_Corrupto» | **El juego no se cierra** y sigue en el panel, **sin ningún mensaje** (sper_089 → `PF-RF02-04_clic_en_corrupto_sigue_en_el_panel.png`). En `Player.log`: `ArgumentException: JSON parse error: Invalid value.` en `Game.Core.SaveStore.Load` ← `ProfileSession.Load` ← `ProfileSelectController.SelectExisting`, sin capturar → **DEF-SPER-02** | DEF |
| 51 | 12:10:32–39 | Clic en «OE4_B» → «Nivel 2» | LevelSelect 0,067 s con el progreso de la semilla; `N2_PuenteI` carga y se juega (`PF-RF02-04_OE4_B_sigue_jugable.png`) | OK |
| 52 | 12:10:57–11:19 | `Cerrar-Juego`; `Start-Juego` (lanz. 14) → «Jugar» → papelera de «OE4_Corrupto» → «Borrar» | «¿Borras el perfil de OE4_Corrupto?», «Su avance se pierde y no se puede recuperar.», «Conservar» / «Borrar» (`PF-RF02-04_papelera_confirmacion.png`); desaparece de la lista y `Datos/OE4_Corrupto.json` ya no existe (`PF-RF02-04_corrupto_borrado.png`) | **P — PF-RF02-04** (operable; con DEF-SPER-02 Menor por la Exception) |
| 53 | 12:11:36–41 | Campo → `Escribir CON` → «Continuar» | No se rechaza: se crea `Datos/CON.json` = `{"name":"CON","reachedLevel":1,"phases":[]}` y entra a `N1_Apertura` (`PF-RF02-05_CON_crea_perfil_y_entra.png`). El Windows 11 de este equipo admite `CON.json` como archivo normal | OK |
| 54 | 12:12:03–32 | `Cerrar-Juego`; `Start-Juego` (lanz. 15) → «Jugar» → «CON» → «Volver» → «Jugar» → papelera de «CON» → «Borrar» | La lista muestra «CON» y «OE4_B» (`PF-RF02-05_CON_listado_al_relanzar.png`); «CON» abre LevelSelect 0,070 s con el N1 habilitado y N2/N3 bloqueados; se borra con su papelera y `CON.json` desaparece (`PF-RF02-05_CON_elegido_y_borrado.png`). Nada se cierra; ningún perfil inservible | **P — PF-RF02-05** |
| 55 | 12:13:24 | `Cerrar-Juego`; `Revisar-Log S-PER` (15 lanzamientos) | Una sola Exception en las 15 corridas: la del perfil corrupto (lanz. 13, línea 43). Nada más | DEF-SPER-02 |
| 56 | 12:13:53 | `Medidas S-PER` | RNF-05: máx. 1128 MiB privados (Narrative), CUMPLE. RNF-04: todas las cargas < 1 s (peor 0,764 s, Narrative). **RNF-10: 2946 filas EXTERNA** en los 15 lanzamientos: TCP establecidas a `34.8.90.77:443`, `34.107.172.168:443`, `34.111.113.40:443` y `34.160.10.162:443` (Google Cloud) → **DEF-SPIKE-01 reproducido**. Las 2946 filas «escucha-local» son los extremos `Bound` de esas mismas conexiones salientes (0.0.0.0:55555/55556/55559); **ningún socket en `Listen`** y ningún UDP | RNF-10 F (DEF-SPIKE-01) |
| 57 | 12:14:2x | `Residuos S-PER`; `Restaurar-Registro S-PER` | LocalLow: `Player.log` y los archivos de Unity Analytics/Insights de DEF-SPIKE-01; %TEMP%: nada del juego; HKCU: la clave no existía y quedó con 22 valores (16 de pantalla + 6 de Unity Analytics/Connect) → borradas la clave del producto y la de la empresa; `Test-Path` False | OK |

## Casos de S-PER

| Caso | Veredicto | Evidencia | Nota |
|---|---|---|---|
| **Cierre forzado a mitad de la mecánica del N1** (casilla S1 Checkpoint D «Cierre forzado a mitad de nivel → retoma desde la última fase confirmada») | **P** | `PF-RNF14-N1mec_*.png` | Sin fase confirmada, retoma el N1 desde su comienzo; el perfil sigue listado y abre sin error |
| **PF-RNF14-03** · N1 durante su cierre | **P** | `PF-RNF14-03_*.png`, `S-PER_OE4N1b_tras_resumen_N1.json` | Abre sin error, N1 por completar y N2 bloqueado; rejugar hasta el resumen guarda la fase y desbloquea el N2 |
| **PF-RNF14-01** · N2 en la 2.2 | **P** | `PF-RNF14-01_*.png`, `S-PER_OE4_B_tras_matar_en_2.2.json` | Tres narrativas y directo al taller |
| **PF-RNF14-05** · con la pausa abierta | **P** | `PF-RNF14-05_*.png` | El taller empieza de cero; `{2,1}` intacta. Hecho con `OE4_B` + 2.1 jugada (equivale a `OE4_B2`) |
| **Casilla S2** · cierre forzado tras confirmar la fase 2 → retoma en la fase 3 | **P** | `S2-tras-fase2_*.png`, `S-PER_OE4_B_tras_confirmar_taller.json` | Retoma en `Level2_Maze` |
| **PF-RNF14-04** · N2 durante su cierre | **P** | `PF-RNF14-04_*.png`, `S-PER_OE4_B_tras_laberinto.json`, `S-PER_OE4_B_final.json` | Fases intactas; N2 «Completado» y N3 desbloqueado y jugable sin rejugar el N2; la 2.5 sin «Omitir» hasta su resumen; los indicadores no se reescriben. Hecho con `OE4_B` jugado (equivale a `OE4_B3`) |
| **PF-RNF14-02** · N3 en el ensamblaje | **P** | `PF-RNF14-02_*.png`, `S-PER_OE4_E_final.json` | Vuelve a mástil y vela con base y amarres, tareas 1–3, tela y mástil en el inventario y Mamá en la zona |
| **PF-RF09-01** · «Salir» reescribe el perfil y cierra | **P** | `PF-RF09-02_dialogo_salir.png`, paso 20 | LastWriteTime posterior, mismo SHA-256, cierre en < 2 s |
| **PF-RF09-02** · confirmación y «Quedarme» | **P** | `PF-RF09-02_*.png` | Con DEF-SPER-01 (Menor, aspecto del botón) |
| **PF-RF09-03** · salir sin perfil | **P** | `PF-RF09-03_salir_sin_perfil.png` | Ningún JSON cambia |
| **Casilla S1** · cerrar y reabrir conserva el perfil y su progreso | **P** | `S1-cerrar-reabrir_*.png` | Perfil creado jugando, listado al reabrir y con el mismo progreso |
| **PF-RF02-04** · JSON corrupto | **P** con **DEF-SPER-02** | `PF-RF02-04_*.png`, `logs/S-PER_lanz13_Player.txt` | Operable y borrable; la Exception sin capturar es DEF Menor según el catálogo |
| **PF-RF02-05** · nombre reservado `CON` | **P** | `PF-RF02-05_*.png` | Se crea un perfil que funciona (en este Windows 11) |
| Apoyo a RNF-08/RNF-10 (`red.csv`) | **F** (DEF-SPIKE-01) | `S-PER_red.csv`, `S-PER_medidas.txt` | Conexiones TCP salientes a :443 en los 15 lanzamientos; sin puertos en escucha |

## Defectos

### DEF-SPER-01 · El botón «Cerrar el juego» lleva la papelera y su rótulo se sale del botón (PF-RF09-02) — Menor
- **Caso:** PF-RF09-02, paso 18 (y paso 21). Versión rc1.
- **Reproducción:** pantalla de inicio → «Salir».
- **Esperado** (HU-18 paso 3; Dirección de arte, iconografía): un diálogo de confirmación cuyo botón de salida se lee entero y no se confunde con borrar.
- **Observado:** el botón de confirmar reutiliza el del diálogo de borrado de perfiles (icono de papelera, el mismo que «Borrar»). El rótulo «Cerrar el juego» no cabe en una línea: parte en «Cerrar el» / «juego», y las dos líneas sobresalen por arriba y por abajo del botón (`DEF-SPER-01_boton_cerrar_el_juego_desborda.png`). El diálogo sí funciona.
- **Tipo propuesto:** código (prefab del diálogo: icono propio o ninguno para «Cerrar el juego», y ancho o tamaño de letra que quepa).
- **Evidencia:** `evidencias/S-PER/PF-RF09-02_dialogo_salir.png`, `DEF-SPER-01_boton_cerrar_el_juego_desborda.png`, `PF-RF09-03_salir_sin_perfil.png`.

### DEF-SPER-02 · Elegir un perfil ilegible lanza una Exception sin capturar y no dice nada (PF-RF02-04) — Menor
- **Caso:** PF-RF02-04, paso 50. Versión rc1.
- **Reproducción:** `Preparar-Datos OE4_B OE4_Corrupto` → «Jugar» → clic en «OE4_Corrupto».
- **Esperado** (casos.md PF-RF02-04; RNF-13): el juego sigue operable, y «una Exception en el log es un DEF Menor aunque el juego siga».
- **Observado:** el juego sigue operable (no se cierra, deja elegir y jugar «OE4_B» y borrar el corrupto), pero `Player.log` registra `ArgumentException: JSON parse error: Invalid value.` desde `JsonUtility.FromJson` en `Game.Core.SaveStore.Load` ← `ProfileSession.Load` ← `Game.UI.ProfileSelectController.SelectExisting`, sin capturar. En pantalla no pasa nada: ningún aviso de que ese perfil no se puede abrir. El informe docente sí lo tolera (`ProfileRepository_RNF13_UnArchivoCorruptoNoTumbaLaEnumeracion`); la selección del panel no.
- **Tipo propuesto:** código (capturar el error de lectura en `SaveStore.Load` o en `SelectExisting` y mostrar un mensaje neutro; prueba nueva p. ej. `ProfileSelect_RF02_UnPerfilIlegibleNoLanzaExcepcion`).
- **Evidencia:** `evidencias/S-PER/logs/S-PER_lanz13_Player.txt` (línea 43), `PF-RF02-04_clic_en_corrupto_sigue_en_el_panel.png`.

### Reproducido: DEF-SPIKE-01 (Unity Analytics/Connect activo en el rc1)
En los 15 lanzamientos de S-PER: conexiones TCP a 4 direcciones de Google Cloud en :443 (`S-PER_red.csv`), archivos `Unity\<id>\Analytics` e `Insights` en LocalLow y 6 valores `unity.*`/`unity_connect.*` en HKCU (`S-PER_residuos.txt`).

## Observaciones (no son defectos)

- **OBS-SPER-1 · Retomar el N2 salta las narrativas intermedias.** Tras un cierre forzado, «Nivel 2» vuelve a contar `N2_PuenteI`, `_Bosque` y `N2_Escena21` (las del bosque) y salta directo a la fase pendiente (taller o laberinto), sin `N2_Escena23` ni `N2_Escena24`, que son las que presentan esas mecánicas. Es lo que el catálogo espera («tres narrativas → entra directo al taller») y la tablilla da la instrucción completa, pero el niño retoma sin la escena que introduce la fase. Lo mismo en el N3: las cuatro de apertura y luego la fase.
- **OBS-SPER-2 · El diálogo de «Salir» sin perfil activo** dice igual «Lo que has logrado ya está guardado», cuando no hay nada guardado (HU-18 FA-03 solo exige no escribir).
- **OBS-SPER-3 · `Cerrar-Juego` (WM_CLOSE, Alt+F4) no reescribe el perfil**, a diferencia de «Salir». No hace falta (el guardado es por fase), y lo confirmado nunca se pierde.
- **OBS-SPER-4 · `CON.json`.** En este Windows 11 el nombre reservado funciona como cualquier otro. En Windows 10 el nombre `CON` con extensión todavía puede tratarse como el dispositivo: conviene repetir PF-RF02-05 en el equipo 2 (Hoja-HUM).
- **OBS-SPER-5 · La carpeta `Datos` cambia de fecha** al abrir «¿Quién juega?» y al pulsar «Salir», sin crear ni cambiar archivos (sonda de escritura de INC-34, previsible).
- **OBS-SPER-6 · El aviso de NVIDIA** asoma unos segundos al arrancar en varias fotos (OBS-1 de GP1, ajeno al juego).

## Estado al terminar

- **Juego:** cerrado con cierre normal; muestreador terminado.
- **Registro:** restaurado (borradas la clave del producto y la de la empresa, como antes de S-PER).
- **Datos:** queda `OE4_B.json` (semilla sin jugar, del bloque D). S-DOC la vacía con `Preparar-Datos`.
- **LocalLow:** `Player.log` y los restos de Unity Analytics de DEF-SPIKE-01 (evidencia del defecto; no se borran).
- **Editor:** cerrado todo el tiempo. No se tocó código.
