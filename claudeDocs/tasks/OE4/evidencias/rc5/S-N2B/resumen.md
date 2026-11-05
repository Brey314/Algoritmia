# S-N2B · rc5 · resumen

- **Versión:** rc5, compilado desde `ca6fd91`. Carpeta `Build\Algoritmia_rc5\`, huella global `ab0c386e87045a4c525548109fa5f9e1905a7336dbd14f21f93b97c8bcb246c3` (253 archivos). SHA-256 del `.exe` `6CD16EDF…ADAB`.
- **Fecha:** 04/11/2026, 22:12–22:17 (hora local), continuación de S-N2A en la misma partida y el mismo proceso (perfil `OE4_B`, sesión de arnés `S-N2A-rc5`, ventana de 1920×1080 con marco). Las medidas de memoria, red, registro y residuos son las de `../S-N2A/` (`S-N2A-rc5_*`).
- **Casos:** 9 filas (los 4 de la tabla de S-N2B más `PF-RF05-02`, `PF-RF11-02`, `PF-RF04-02`, `PF-RF05-04` y `PF-RF06-01`, que se alimentan también de otras sesiones y se juzgaron solo en mi parte). **Veredictos: P 9 · PD 0 · F 0 · B 0 · NA 0 · NE 0.**
- **Pasos 1–17 ejecutados**, sin atajos.

## Cifras: previstas frente a obtenidas (N2F2)
| | Previsto | Obtenido (`../S-N2C/S-N2_OE4_B.json`) |
|---|---|---|
| intentos · errores corregidos · pasos | 5 · 3 · 6 | 5 · 3 · 6 |
| `resolutionSeconds` | T(última pieza) − T0 ± 5 s | 149,94 s (T0 = carga de `Level2_Workshop` 03:14:51,0 UTC; cuerda 03:17:19,7; diferencia 148,7 s, +1,3 s) |

Contaron intento: mazo sobre un tronco sin elegir (paso 2), tabla (5), caja (6), cuerda (7) y caja sobre la carretilla (12); no contaron la pieza ni el mazo soltados lejos (pasos 9 y 10). Corregidos: segunda y primera rueda y tabla montada (pasos 4, 10 y 13).

## Lo que se vio
- **`N2_Escena23_Construccion`:** 8 líneas (NIÑA, PAPÁ, NIÑO, MAMÁ y cuatro de ALGORITM), sin «Omitir» ni pausa; la última nombra la cuerda. Después `Level2_Workshop` cargó en 0,025 s.
- **Taller:** las siete piezas; «Mecanizar» deshabilitado con candado y «Aún no» hasta elegir un tronco corto; el elegido lleva contorno y crece. Mensajes exactos en todos los pasos; la pista llega al tercer rechazo seguido (tabla, caja, cuerda) y no antes (el clic de elegir y «Mecanizar» reinician la cuenta). La instrucción de tres líneas cabe en su tablilla.
- **Cierre:** «La cuerda sujeta la caja. La carretilla está completa.», la familia celebra con los brazos arriba y fundido a la 2.4.
- **Retrato animado:** presente en las 8 líneas; ver S-N2A para la medida de boca y parpadeo.
- **Grises:** `PF-RNF19-01_1_mecanizar_aunno_gris.png` («Mecanizar» con candado y «Aún no»), `_5_rechazo_gris.png`, `_3_resaltado_gris.png`.

## Medidas y límites
- RNF-04: `Level2_Workshop` 0,025 s; el resto, en `../S-N2A/S-N2A-rc5_Player.txt`. Memoria: privada máx 1.156 MiB en `Level2_Workshop`.
- **Salida 3 del arnés (una):** el primer `Arrastrar` del paso 2 (mazo sobre un tronco sin elegir) devolvió «FOCO: se perdió el primer plano durante el arrastre (se soltó el botón)». Al mirar, el juego había recibido la suelta y mostraba «Mecanizar necesita un tronco corto resaltado.»; `Estado` mostró el juego en primer plano. No se repitió el arrastre para no sumar un intento; el JSON (5 intentos) confirma que contó uno. Causa desconocida (¿una notificación de otra aplicación?). La foto del paso 2 se tomó entonces con una llamada aparte, ≈ 10 s después.
- Los mensajes de las fotos se leyeron a 0,53–0,85 s de cada acción; el límite de 1 s de `PF-RF11-02` se juzga por ese margen, no por un cronómetro.
- Los retratos se vieron en foto fija; no se midió la animación en este tramo.
- Datos para S-RNF/S-INSP: `PF-RNF03-01_1_taller.png` (pantalla completa del taller); sin solapes ni textos fuera de su recuadro.
