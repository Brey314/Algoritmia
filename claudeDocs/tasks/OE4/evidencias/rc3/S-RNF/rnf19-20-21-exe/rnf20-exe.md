# RNF-20: contraste texto/fondo (`arte_check.py rnf20`)

- Comando: `arte_check.py rnf20 --file ../cajas-rnf20-exe.txt --crops ../recortes_rnf20_exe --out ../rnf20-exe.md`
- Contraste WCAG 2.x: percentil 1 de la luminancia del texto contra percentil 60 del fondo dentro de la caja (coordenadas normalizadas; en claro/oscuro se usan los percentiles espejo). Mínimo RNF-20 = 4,5:1. Los colores son los del píxel que cae en cada percentil.

| Región | Captura y caja | Polaridad | Texto | Fondo | Contraste | Mínimo | Veredicto |
|---|---|---|---|---|---|---|---|
| N1 tablilla de instrucción (estado más oscuro) | `FirePanel_RNF20_EstadoMasOscuro.png` @ 0.3073,0.0852,0.6927,0.1222 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Diálogo: nombre del hablante (PAPÁ) | `Dialogo_N1_Hallazgo_L01.png` @ 0.2344,0.8083,0.2719,0.8306 | oscuro/claro | #1C0D0D | #F7EFE2 | 16,5:1 | 4,5:1 | OK |
| Diálogo: línea | `Dialogo_N1_Hallazgo_L01.png` @ 0.2344,0.8537,0.3917,0.8843 | oscuro/claro | #1C0D0D | #F7EFE2 | 16,5:1 | 4,5:1 | OK |
| Diálogo: botón Continuar | `Dialogo_N1_Hallazgo_L01.png` @ 0.7396,0.9074,0.8135,0.9333 | oscuro/claro | #3B1C16 | #E8A33D | 7,1:1 | 4,5:1 | OK |
| Menú: título Algoritmia (blanco con contorno): texto frente al contorno | colores dados | dado | #FFFFFF | #3B1C16 | 15,4:1 | 4,5:1 | OK |
| Menú: título Algoritmia (blanco con contorno): contorno frente al fondo ámbar | colores dados | dado | #3B1C16 | #E8C27D | 9,1:1 | 4,5:1 | OK |
| Menú: título Algoritmia (blanco sin contorno frente al ámbar, informativo, DA §7: contorno de 4 px) | colores dados | dado | #FFFFFF | #E8C27D | 1,7:1 | 4,5:1 | **FALLA** |
| Menú: subtítulo | `MainMenu_RNF20_ContrasteTextoFondo.png` @ 0.0615,0.3815,0.3333,0.4139 | oscuro/claro | #6A5349 | #F7EFE2 | 6,2:1 | 4,5:1 | OK |
| Menú: Jugar | `MainMenu_RNF20_ContrasteTextoFondo.png` @ 0.1979,0.5278,0.2396,0.5537 | oscuro/claro | #3B1C16 | #E8A33D | 7,1:1 | 4,5:1 | OK |
| Menú: Créditos | `MainMenu_RNF20_ContrasteTextoFondo.png` @ 0.0911,0.6333,0.1547,0.6593 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Menú: Salir | `MainMenu_RNF20_ContrasteTextoFondo.png` @ 0.2969,0.6333,0.3312,0.6593 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Menú: Progreso del equipo | `MainMenu_RNF20_ContrasteTextoFondo.png` @ 0.1437,0.7389,0.2932,0.7667 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| N1 instrucción (con Soplar atenuado) | `FirePanel_RNF19_SoplarAtenuado.png` @ 0.2865,0.0852,0.7135,0.1222 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| N1 botón Soplar atenuado | `FirePanel_RNF19_SoplarAtenuado.png` @ 0.3958,0.8796,0.4521,0.9167 | oscuro/claro | #3B1C16 | #E0D4C0 | 10,5:1 | 4,5:1 | OK |
| N1 botón Golpear | `FirePanel_RNF19_SoplarAtenuado.png` @ 0.5563,0.8796,0.6224,0.9167 | oscuro/claro | #3B1C16 | #E8A33D | 7,1:1 | 4,5:1 | OK |
| N1 rótulo Fuerza del golpe | `FirePanel_RNF19_SoplarAtenuado.png` @ 0.8766,0.2361,0.9792,0.2639 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| N1 rótulo Lejos | `FirePanel_RNF19_SoplarAtenuado.png` @ 0.3255,0.7824,0.3646,0.8074 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| N3 lista: tarea hecha | `RiverLevel_RNF19_ColocacionIncorrecta.png` @ 0.0760,0.0833,0.1938,0.1167 | oscuro/claro | #326638 | #F7EFE2 | 5,9:1 | 4,5:1 | OK |
| N3 lista: tarea pendiente | `RiverLevel_RNF19_ColocacionIncorrecta.png` @ 0.0760,0.1870,0.2073,0.2204 | oscuro/claro | #6A5347 | #F7EFE2 | 6,3:1 | 4,5:1 | OK |
| N3 mensaje de ayuda | `RiverLevel_RNF19_ColocacionIncorrecta.png` @ 0.4062,0.0870,0.7422,0.1593 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| N3 botón Listo | `RiverLevel_RNF19_ColocacionIncorrecta.png` @ 0.4792,0.9000,0.5198,0.9333 | oscuro/claro | #3B1C16 | #DF9C3B | 6,6:1 | 4,5:1 | OK |
| N2 bosque: mensaje | `WheelLevel_RNF19_Bosque_ObjetoNoValido.png` @ 0.0927,0.0833,0.4870,0.1648 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| N2 bosque: contador de troncos | `WheelLevel_RNF19_Bosque_ObjetoNoValido.png` @ 0.0458,0.7556,0.2167,0.7870 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| N2 laberinto: instrucción | `Maze_01_reposo.png` @ 0.1328,0.0509,0.6328,0.1185 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| N2 laberinto: Tu secuencia | `Maze_01_reposo.png` @ 0.6917,0.0759,0.8031,0.1093 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| N2 laberinto: Ejecutar | `Maze_01_reposo.png` @ 0.7917,0.8759,0.8625,0.9130 | oscuro/claro | #3B1C16 | #E8A33D | 7,1:1 | 4,5:1 | OK |
| A9 candado del botón Soplar atenuado | `FirePanel_RNF19_SoplarAtenuado.png` @ 0.3453,0.8824,0.3630,0.9167 | oscuro/claro | #130302 | #E0D4C0 | 13,8:1 | 3,0:1 | OK |
| C5 visto de la tarea hecha | `RiverLevel_RNF19_ColocacionIncorrecta.png` @ 0.0505,0.0880,0.0661,0.1157 | claro/oscuro | #F7EFE2 | #083407 | 12,2:1 | 3,0:1 | OK |
| B2 aviso de piedra con esquinas | `WheelLevel_RNF19_Bosque_ObjetoNoValido.png` @ 0.0458,0.0926,0.0833,0.1556 | oscuro/claro | #1F0500 | #F7EFE2 | 17,0:1 | 3,0:1 | OK |
| C8 icono de alerta sobre la casilla incorrecta | `RiverLevel_RNF19_ColocacionIncorrecta.png` @ 0.5547,0.5815,0.5885,0.6389 | oscuro/claro | #320904 | #BE8084 | 5,6:1 | 3,0:1 | OK |
| Resumen: título | `Resumen_N1.png` @ 0.259,0.139,0.718,0.190 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Resumen: descubrimiento | `Resumen_N1.png` @ 0.169,0.282,0.542,0.310 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Resumen: viñeta | `Resumen_N1.png` @ 0.2125,0.368,0.50,0.398 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Resumen: recuadro de la habilidad | `Resumen_N1.png` @ 0.244,0.662,0.578,0.690 | oscuro/claro | #3B1C16 | #E0D4C0 | 10,5:1 | 4,5:1 | OK |
| Resumen: Volver al menú de niveles | `Resumen_N1.png` @ 0.433,0.838,0.621,0.866 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Resumen: Continuar | `Resumen_N1.png` @ 0.708,0.838,0.7875,0.866 | oscuro/claro | #3B1C16 | #E8A33D | 7,1:1 | 4,5:1 | OK |
| Créditos: título | `Creditos_a.png` @ 0.0615,0.13,0.19,0.176 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Créditos: encabezado de columna | `Creditos_a.png` @ 0.0615,0.213,0.173,0.236 | oscuro/claro | #6A5349 | #F7EFE2 | 6,2:1 | 4,5:1 | OK |
| Créditos: nombre | `Creditos_a.png` @ 0.0615,0.276,0.224,0.304 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Créditos: nota de autoría | `Creditos_c.png` @ 0.0615,0.476,0.60,0.494 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Créditos: tipografías (texto más claro) | `Creditos_c.png` @ 0.0615,0.745,0.391,0.763 | oscuro/claro | #6A5349 | #F7EFE2 | 6,2:1 | 4,5:1 | OK |
| Créditos: Volver | `Creditos_a.png` @ 0.09,0.833,0.138,0.861 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Informe: título | `Informe_OE4N1.png` @ 0.395,0.069,0.605,0.102 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Informe: encabezado Nivel | `Informe_OE4N1.png` @ 0.383,0.255,0.415,0.282 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Informe: encabezado Intentos | `Informe_OE4N1.png` @ 0.583,0.255,0.634,0.282 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Informe: cifra | `Informe_OE4N1.png` @ 0.604,0.338,0.616,0.361 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Informe: Sin datos | `Informe_OE4N1.png` @ 0.58,0.393,0.636,0.419 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Informe: perfil en la lista | `Informe_OE4N1.png` @ 0.042,0.193,0.085,0.22 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Informe: Eliminar datos | `Informe_OE4N1.png` @ 0.131,0.792,0.218,0.818 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Informe: Perfil de | `Informe_OE4N1.png` @ 0.349,0.18,0.48,0.208 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Laberinto: bloque Avanzar (cajón abierto) | `Maze_Girar_cajon.png` @ 0.725,0.597,0.78,0.63 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Laberinto: bloque Girar (cajón abierto) | `Maze_Girar_cajon.png` @ 0.865,0.597,0.90,0.63 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Laberinto: bloque Retroceder (cajón abierto) | `Maze_Girar_cajon.png` @ 0.779,0.699,0.857,0.727 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Taller: instrucción | `Workshop_01_reposo.png` @ 0.19,0.07,0.80,0.18 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |

**54 regiones · 53 cumplen · 1 no cumplen.**
