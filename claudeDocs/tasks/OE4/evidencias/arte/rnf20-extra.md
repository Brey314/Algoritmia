# RNF-20: contraste texto/fondo (`arte_check.py rnf20`)

- Comando: `arte_check.py rnf20 --file cajas-rnf20-extra.txt --out rnf20-extra.md`
- Contraste WCAG 2.x: percentil 1 de la luminancia del texto contra percentil 60 del fondo dentro de la caja (coordenadas normalizadas; en claro/oscuro se usan los percentiles espejo). Mínimo RNF-20 = 4,5:1. Los colores son los del píxel que cae en cada percentil.

| Región | Captura y caja | Polaridad | Texto | Fondo | Contraste | Mínimo | Veredicto |
|---|---|---|---|---|---|---|---|
| N1 instrucción (con Soplar atenuado) | `FirePanel_RNF19_SoplarAtenuado.png` @ 0.2865,0.0852,0.7135,0.1222 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| N1 botón Soplar atenuado | `FirePanel_RNF19_SoplarAtenuado.png` @ 0.3958,0.8796,0.4521,0.9167 | oscuro/claro | #3B1C16 | #E0D4C0 | 10,5:1 | 4,5:1 | OK |
| N1 botón Golpear | `FirePanel_RNF19_SoplarAtenuado.png` @ 0.5563,0.8796,0.6224,0.9167 | oscuro/claro | #3B1C16 | #E8A33D | 7,1:1 | 4,5:1 | OK |
| N1 rótulo Fuerza del golpe | `FirePanel_RNF19_SoplarAtenuado.png` @ 0.8766,0.2361,0.9792,0.2639 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| N1 rótulo Lejos | `FirePanel_RNF19_SoplarAtenuado.png` @ 0.3255,0.7824,0.3646,0.8074 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| N3 lista: tarea hecha | `RiverLevel_RNF19_ColocacionIncorrecta.png` @ 0.0760,0.0833,0.1938,0.1167 | oscuro/claro | #326638 | #F7EFE2 | 5,9:1 | 4,5:1 | OK |
| N3 lista: tarea pendiente | `RiverLevel_RNF19_ColocacionIncorrecta.png` @ 0.0760,0.1870,0.2073,0.2204 | oscuro/claro | #6A5347 | #F7EFE2 | 6,3:1 | 4,5:1 | OK |
| N3 mensaje de ayuda | `RiverLevel_RNF19_ColocacionIncorrecta.png` @ 0.4062,0.0870,0.7422,0.1593 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| N3 botón Listo | `RiverLevel_RNF19_ColocacionIncorrecta.png` @ 0.4792,0.9000,0.5198,0.9333 | oscuro/claro | #3B1C16 | #E8A33D | 7,1:1 | 4,5:1 | OK |
| N2 bosque: mensaje | `WheelLevel_RNF19_Bosque_ObjetoNoValido.png` @ 0.0927,0.0833,0.4870,0.1648 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| N2 bosque: contador de troncos | `WheelLevel_RNF19_Bosque_ObjetoNoValido.png` @ 0.0458,0.7556,0.2167,0.7870 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| N2 laberinto: instrucción | `Maze_01_reposo.png` @ 0.1328,0.0509,0.6328,0.1185 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| N2 laberinto: Tu secuencia | `Maze_01_reposo.png` @ 0.6917,0.0759,0.8031,0.1093 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| N2 laberinto: Ejecutar | `Maze_01_reposo.png` @ 0.7917,0.8759,0.8625,0.9130 | oscuro/claro | #3B1C16 | #E8A33D | 7,1:1 | 4,5:1 | OK |
| C8 icono de alerta sobre la casilla incorrecta | `RiverLevel_RNF19_ColocacionIncorrecta.png` @ 0.3984,0.4278,0.4469,0.5074 | oscuro/claro | #290200 | #D99942 | 7,8:1 | 3,0:1 | OK |
| A9 candado del botón Soplar atenuado | `FirePanel_RNF19_SoplarAtenuado.png` @ 0.3453,0.8824,0.3630,0.9167 | oscuro/claro | #130302 | #E0D4C0 | 13,8:1 | 3,0:1 | OK |
| C5 visto de la tarea hecha | `RiverLevel_RNF19_ColocacionIncorrecta.png` @ 0.0505,0.0880,0.0661,0.1157 | claro/oscuro | #F7EFE2 | #083407 | 12,2:1 | 3,0:1 | OK |
| B2 aviso de piedra con esquinas | `WheelLevel_RNF19_Bosque_ObjetoNoValido.png` @ 0.0458,0.0926,0.0833,0.1556 | oscuro/claro | #1F0500 | #F7EFE2 | 17,0:1 | 3,0:1 | OK |

**18 regiones · 18 cumplen · 0 no cumplen.**

---

## Lectura (revisión W3, 01/10/2026; escrita a mano, no sale del script)

Se revisaron a ojo los 28 recortes de `rnf20.md` y de este informe. **27 caen sobre el texto o el icono que nombran.**

La excepción es **«C8 icono de alerta sobre la casilla incorrecta»**: su caja de W0 cae hoy sobre los pies de Papá, porque W2 corrió la balsa, y el 7,8:1 de la tabla no mide el icono. Lo sustituye `rnf20-w3.md`:

- el «!» contra la silueta rosa da **5,6:1, OK**;
- el contorno naranja del triángulo contra esa silueta da 1,6:1 (medido aparte). La forma la lleva el «!».
