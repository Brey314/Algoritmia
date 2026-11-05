# S-N2C · rc5 · resumen

- **Versión:** rc5, compilado desde `ca6fd91`. Carpeta `Build\Algoritmia_rc5\`, huella global `ab0c386e87045a4c525548109fa5f9e1905a7336dbd14f21f93b97c8bcb246c3` (253 archivos). SHA-256 del `.exe` `6CD16EDF…ADAB`.
- **Fecha:** 04/11/2026. Partida principal 22:18–22:29 (hora local), continuación de S-N2B en el mismo proceso (perfil `OE4_B`, sesión de arnés `S-N2A-rc5`, ventana de 1920×1080 con marco; medidas, registro y residuos en `../S-N2A/`). **Repetición del laberinto** en una sesión aparte, `S-N2C2-rc5`, 22:30–22:35, con la semilla `OE4_B3` (`S-N2C2-rc5_*` en esta carpeta), solo para ver el mensaje de llegada que la ráfaga de la partida principal no alcanzó.
- **Casos:** 17 filas (los 11 de la tabla de S-N2C más `PF-RF05-02`, `PF-RF11-02`, `PF-RF04-02`, `PF-RF03-02`, `PF-RF06-01` y `PF-RF05-04`, que se alimentan también de otras sesiones y se juzgaron solo en mi parte). **Veredictos: P 16 · PD 1 (`PF-RF31-01`) · F 0 · B 0 · NA 0 · NE 0.**
- **Pasos 1–19 ejecutados**, sin atajos. Salida 0 en todas las llamadas de esta parte.

## Cifras: previstas frente a obtenidas (N2F3)
| | Previsto | Obtenido (`S-N2_OE4_B.json`) |
|---|---|---|
| intentos (ejecuciones fallidas con bloques) | 3 | 3 |
| errores corregidos (ediciones desde la última ejecución fallida) | 43 (conteo a mano, ver `tablero_paso2.txt`) | 43 |
| pasos (bloques de la secuencia ganadora) | 13 | 13 |
| `resolutionSeconds` | T(llegada) − T0 ± 5 s | 508,59 s (T0 = carga de `Level2_Maze` 03:18:16,4 UTC) |
| `reachedLevel` | 3 | 3 |

La llegada no se observó en la partida principal (la ráfaga terminó 3 s antes): del clic en «Ejecutar» (03:26:22,75) más ≈ 22 s de ejecución sale 508 s, de acuerdo con el JSON, pero no es una medida directa. En la repetición con `OE4_B3` el JSON dio `{attempts 0, correctedErrors 0, stepsUsed 13, resolutionSeconds 171,90}`: sin fallos previos no cuentan ediciones, como dice la definición de C.

**Tablero de la partida principal** (aleatorio; transcrito por muestreo de color, `tablero_paso2.txt`): salida (0,8), refugio (15,2), (1,8) y (14,2) libres, la «L» directa cerrada por (7,8). Camino ganador de 13 bloques: Avanzar ×1 · Girar izq · Avanzar ×1 · Girar der · Avanzar ×7 · Girar der · Avanzar ×2 · Girar izq · Avanzar ×3 · Girar der · Avanzar ×5 · Girar izq · Avanzar ×4. Ninguna ejecución falló por mala lectura del tablero.

## Lo que se pidió mirar
- **`PF-RF31-02` (flechas curvas, INC-156).** Cada lado de «Girar» lleva una flecha curva con su sentido, derechas y sin icono girado; el lado elegido se rellena de naranja. En grises el elegido sale a gris 172 y el otro a 213 y se sigue distinguiendo (`PF-RF31-02_5_zoom_color_y_gris.png`, `_5_izquierda_color.png` y `_gris.png`). Al pulsar la izquierda la carretilla giró a la izquierda y quedó mirando al norte.
- **`PF-RF45-02`.** Avatar de ALGORITM como **disco de madera** con pantaloneta de cuadros (`PF-RF45-02_16_avatar_zoom.png`); la tarjeta del resumen mide y 45–1043 de 1080, entera, con sus dos botones dentro; ningún dígito en los textos.
- **`PF-RF34-02` (⚠).** El bloque soltado fuera del panel se retiró y no se pegó al cursor (el siguiente arrastre desde el cajón funcionó); el reordenamiento dio «Avanzar ×2», «Avanzar ×1», «Girar». No hizo falta recuperar con la pausa.
- **`PF-RF30-02`.** Colores medidos por píxel en `PF-RF30-02_1_colores.txt`: fondo `#C4A882` en cinco puntos, marcos `#6A5A45` de 8 px completos, zona de soltar `#E0D4C0`, aro de la ayuda `#A0320D`; sin tinte de luz.

## Lo que se vio
- **`N2_Escena24_Regreso`** (8 líneas; con el atardecer de la luz de la narrativa) y **`N2_Escena25_Cierre`** (6), sin «Omitir» ni pausa; las frases «…Eso se llama abstraer.» y «…eso es pensar como un algoritmo.» (`PF-RF12-02_15_1.png` y `_2.png`). En la 2.5 quien está junto a la fogata la mira.
- **Mecánica:** «Ejecutar» vacío, tres fallos contra el seto con la pista a la tercera, papelera con el cajón abierto y cerrado, retirar soltando fuera, reordenar, ▲/▼ y barra con 13 bloques, ejecución ganadora con el bloque en curso resaltado y visible en los 57 cuadros de la ráfaga (`PF-RF32-01_14_analisis_resaltado.txt`), resumen sin dígitos y menú con el Nivel 3 habilitado.
- **Mensaje de llegada (repetición):** «¡La carretilla llegó al refugio! Los pasos, en ese orden, funcionaron.» con icono de acierto en el cuadro 61 de una ráfaga a 3 fps (`PF-RF32-01_14_llegada_f61_mensaje.png`), la carretilla entra a la cueva, la familia celebra con los brazos arriba y a los ~0,7 s empieza el fundido a negro y entra la 2.5.
- **Primer fallo:** la carretilla gira a la izquierda, avanza unos 10 px contra el seto y vuelve en 3 cuadros a 8 fps; a ~2 s el mensaje y ~0,5 s después reaparece en la salida mirando al este.

## Defectos
- `DEF-RC5-04` · Observación · documento: «Retroceder (con rombo)» (es un círculo) y «el refugio no se dibuja» (se dibuja una cueva). Por (a), `PF-RF31-01` queda en PD.
- `DEF-RC5-05` · Observación · juego (a confirmar): con el cajón abierto o con la secuencia larga, los bloques no seleccionados no muestran su cuenta ni su lado.
- Ninguno funcional, ninguno Mayor ni Bloqueante.

## Medidas y avisos del arnés
- RNF-04: `Level2_Maze` 0,027 s (0,021 s en la repetición) y `LevelSummary` 0,014 s; `S-N2C2-rc5_Player.txt` trae 7 líneas con el peor valor 0,200 s. Memoria de la repetición: 136 muestras, de trabajo máx 438 MiB y privada máx 1.165 MiB; de la partida principal, privada máx en el laberinto 1.155 MiB y en `LevelSummary` 1.123 MiB. RNF-10: 136 muestras `NINGUNA`. Sin `Exception`, `Error` ni `Assert`. `Residuos` y `Restaurar-Registro` de la segunda sesión: mismos ocho valores del registro de Unity, restaurados (`residuos_S-N2C2.txt`). `Preparar-Datos -SinCarpeta` al final y el manifiesto coincide (253 archivos, misma huella).
- La repetición no pasó por `Esperar-Carga` para el laberinto (la carga está en su `Player.txt`, no en `cargas_oe4.csv`).
- **Error mío al armar la secuencia de la repetición:** una función de PowerShell llamada `Gi` chocó con el alias `Get-Item` y no soltó los bloques «Girar»; se vaciaron con la papelera y se rehízo. No afecta a la partida principal; solo a las ediciones de la repetición, que no cuentan.
- Con tantos bloques los pasos 11 y 12 usaron posiciones de arrastre leídas de las capturas del cajón abierto (zona «Suelta un bloque aquí» en y ≈ 235 con la lista vacía y ≈ 328 después); la lista se recoloca sola al soltar.
- Límites: el retrato animado se vio en foto fija (la medida de boca y parpadeo está en S-N2A); no se probó la rueda ni el arrastre sobre la lista de bloques; el tiempo del fundido final se vio por fotos.
- Datos para S-INSP: `PF-RNF03-01_1_laberinto.png` (pantalla completa del laberinto; el refugio sale del cuadro por el borde derecho del entorno, con el marco entero) y grises `PF-RNF19-01_3_ejecutar_vacio_gris.png`, `_4_cajon_gris.png`, `_7_bloque_detenido_gris.png` (el bloque en curso o detenido se distingue por su contorno).
