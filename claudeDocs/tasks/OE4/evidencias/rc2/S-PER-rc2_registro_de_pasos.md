# S-PER-rc2 · bloque D y cierre forzado en el instante del guardado (01/10/2026, 20:09–20:17)

- **Exe** rc2 (SHA-256 coincide), ventana sin marco 1920×1080, monitor único 1920×1080 al 125 %. 4 lanzamientos. Sonido silenciado durante la sesión y devuelto antes del cierre. La ventana de Webex ya no estaba; el rectángulo gris arriba a la derecha de las fotos es la máscara que `tapar.py` aplicó igual a todas las capturas (tapa media burbuja del botón de ayuda del N3; ningún veredicto depende de esa zona).
- **Pre:** `Preparar-Datos OE4_B OE4_Corrupto OE4_E` (`OE4_Corrupto.json` = 38 B truncados: `{"name":"OE4_Corrupto","reachedLevel":`).
- **Herramienta del cierre forzado:** `matar-al-guardar.ps1` (`FileSystemWatcher` sobre `Datos\` que llama a `TerminateProcess` en el primer evento que casa con el filtro). Se armó en segundo plano 2,5 s antes de pulsar «Probar balsa», que confirma la fase 3 del N3 y guarda el perfil.

| # | Hora | Acción | Observado | Veredicto |
|---|---|---|---|---|
| 1 | 20:09:59 | Lanz. 1 → «Jugar» | Lista: OE4_B, OE4_Corrupto, OE4_E, cada una con su papelera | OK |
| 2 | 20:10 | Clic en «OE4_Corrupto» | El juego sigue en el panel y aparece **«No se pudo abrir ese perfil. Avisa a tu profe.»** (sin cifras ni culpa: CP-02, CP-03). **Ninguna `Exception` en el `Player.log`** (rc1: `ArgumentException: JSON parse error` sin capturar) (`DEF-SPER-02_perfil_ilegible_aviso.png`) | **P (PF-RF02-04; cierra DEF-SPER-02, parte del aviso)** |
| 3 | 20:10 | Papelera de «OE4_Corrupto» → «Borrar» | Desaparece de la lista y del disco. El aviso **sigue** en la columna «Perfil nuevo» tras borrarlo (N4 de la revisión, cosmético) (`DEF-SPER-02_aviso_tras_borrar_N4.png`) | P |
| 4 | 20:11:17 | Campo → `Escribir CON` → «Continuar» | Se crea `Datos/CON.json` = `{"name":"CON","reachedLevel":1,"phases":[]}` y entra a `N1_Apertura` (0,780 s) | OK |
| 5 | 20:11:4x | `Matar`; lanz. 2 → «Jugar» → «CON» | Lista CON, OE4_B, OE4_E; «CON» abre «Elige un nivel» con el N1 habilitado y el N2 y el N3 «Bloqueado» (`PF-RF02-05_CON_listado_y_elegido.png`) | P |
| 6 | 20:12 | «Volver» → «Jugar» → papelera de «CON» → «Borrar» | `CON.json` desaparece; nada se cierra | **P (PF-RF02-05)** |
| 7 | 20:12–20:13 | «OE4_E» → «Nivel 3» → 19 «Continuar» por cuatro narrativas | `Level3_River` directo al ensamblaje en mástil y vela: base y amarres armados, tareas 1–3 con visto, tela y mástil en el inventario, Mamá en la zona | OK |
| 8 | 20:14:0x | Mástil y vela a sus siluetas | «Puesto. Cuando toda esta parte esté, pulsa el botón.» (`PF-RNF14-guardado_antes_de_probar.png`) | OK |
| 9 | 20:14:19,9Z–24,2Z | **Prueba 1:** vigilante con filtro `*.tmp`; «Probar balsa» | **MATADO 01:14:24.188Z tras `Created` de `OE4_E.json.tmp`.** En disco: `OE4_E.json` **intacto** (611 B, el de la semilla, 20:09:55,781) y `OE4_E.json.tmp` de 718 B **completo** (la fase 3/3 nueva). Los dos parsean | **P: el cierre entre la escritura del temporal y el reemplazo no deja un JSON truncado** |
| 10 | 20:14:4x | Lanz. 3 → «Jugar» | Lista **OE4_B y OE4_E**: el `.tmp` huérfano no aparece; «OE4_E» abre sin aviso; N1–N2 «Completado», N3 habilitado | P |
| 11 | 20:15 | «Nivel 3» → cuatro narrativas | Vuelve al ensamblaje en mástil y vela con base y amarres, tareas 1–3, tela y mástil en el inventario: lo aprobado se conserva y la fase sin confirmar empieza de nuevo (`PF-RNF14-guardado_tras_matar_a_mitad.png`) | P (RNF-14, invariante 3) |
| 12 | 20:15:49Z–53,3Z | **Prueba 2:** mástil y vela; vigilante con filtro `OE4_E.json`; «Probar balsa» | **MATADO 01:15:53.329Z tras `Renamed` de `OE4_E.json~RF71ee4d3.TMP`** (el renombrado interno de `ReplaceFile`). En disco: `OE4_E.json` de 719 B **completo y válido**, con la fase `{3, 3, 0, 0, 3, 28,03}`; **el `.tmp` huérfano de la prueba 1 ya no está** (lo pisó este guardado) y no queda ningún `~RF…TMP` | P. *(corr. 01/10/2026, 21:45, revisión final de rc2)* El vigilante despertó con el renombrado interno, pero el cierre llegó con el reemplazo ya hecho: la prueba muestra un cierre **justo después del reemplazo**. La ventana interna de `File.Replace`, sin ningún `OE4_E.json` en disco, no se alcanzó (riesgo residual R-RC2-A, `OE4-Resultados.md` §8.7) |
| 13 | 20:16:14 | Lanz. 4 → «OE4_E» | N1, N2 y **N3 «Completado»**; «Nivel 3» abre `N3_PuenteII` con «Omitir» (nivel terminado al confirmar la última fase) (`PF-RNF14-guardado_tras_matar_al_reemplazar.png`) | P |
| 14 | 20:17 | `Cerrar-Juego`; `Revisar-Log` (4 lanzamientos); `Medidas`; `Residuos`; `Restaurar-Registro` | **Ninguna Exception, Error ni Assert en los cuatro `Player.log`.** Red: 181/181 muestras sin sockets. Privada máx. 1109 MiB. LocalLow: solo `Player.log`. HKCU: los 21 valores con `unity_connect.*` (DEF-RC2-01), borrados | P · DEF-RC2-01 |

## Casos

| Caso | Veredicto | Nota |
|---|---|---|
| **PF-RF02-04** | **P** (antes P con DEF Menor) | Aviso al elegir el perfil ilegible, sin Exception; operable y borrable. **Cierra DEF-SPER-02** (aviso). N4: el aviso sale en la columna del perfil nuevo y no se borra al quitar el perfil |
| PF-RF02-05 | P | `CON` crea un perfil que funciona, se elige y se borra (Windows 11 de este equipo) |
| **Guardado atómico (DEF-SPER-02, segunda parte; RNF-14)** | **P** | Dos cierres forzados en el instante del guardado: **entre la escritura del temporal y el reemplazo** → perfil anterior intacto, se abre y retoma; **justo después del reemplazo** → perfil nuevo completo *(corr. 01/10/2026, 21:45, revisión final de rc2)*. En ningún caso un JSON truncado. El temporal huérfano no se lista y el siguiente guardado lo limpia |

Evidencias: `evidencias/rc2/PF-RNF14-guardado_matar{1,2}.txt` (salida del vigilante con el listado de `Datos\`),
`…_matar1_OE4_E.json` (perfil tras la prueba 1), `…_matar1_OE4_E.json.tmp.txt` (el temporal huérfano),
`…_matar2_OE4_E.json`, y `S-PER-rc2_lanz{1..4}_Player.txt`.
