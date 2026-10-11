# RNF-20: contraste texto/fondo (`arte_check.py rnf20`)

- Comando: `arte_check.py rnf20 --file cajas-rnf20-rc3.txt --crops recortes_rnf20_rc3 --out rnf20-rc3.md`
- Contraste WCAG 2.x: percentil 1 de la luminancia del texto contra percentil 60 del fondo dentro de la caja (coordenadas normalizadas; en claro/oscuro se usan los percentiles espejo). Mínimo RNF-20 = 4,5:1. Los colores son los del píxel que cae en cada percentil.

| Región | Captura y caja | Polaridad | Texto | Fondo | Contraste | Mínimo | Veredicto |
|---|---|---|---|---|---|---|---|
| N1 tablilla de instrucción (estado más oscuro) | `FirePanel_RNF20_EstadoMasOscuro.png` @ 0.3073,0.0852,0.6927,0.1222 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Diálogo: nombre del hablante (PAPÁ) | `C:/Dev/Algoritmia/claudeDocs/tasks/Personajes/capturas/2026-10-10/Personajes_N1_Hallazgo_L01.jpg` @ 0.2344,0.8083,0.2719,0.8306 | oscuro/claro | #120701 | #F6ECE2 | 17,1:1 | 4,5:1 | OK |
| Diálogo: línea | `C:/Dev/Algoritmia/claudeDocs/tasks/Personajes/capturas/2026-10-10/Personajes_N1_Hallazgo_L01.jpg` @ 0.2344,0.8537,0.3917,0.8843 | oscuro/claro | #130A01 | #F7EFE2 | 17,2:1 | 4,5:1 | OK |
| Diálogo: botón Continuar | `C:/Dev/Algoritmia/claudeDocs/tasks/Personajes/capturas/2026-10-10/Personajes_N1_Hallazgo_L01.jpg` @ 0.7396,0.9074,0.8135,0.9333 | oscuro/claro | #3B1500 | #DDA64B | 7,4:1 | 4,5:1 | OK |
| Menú: título Algoritmia (blanco con contorno): texto frente al contorno | colores dados | dado | #FFFFFF | #3B1C16 | 15,4:1 | 4,5:1 | OK |
| Menú: título Algoritmia (blanco con contorno): contorno frente al fondo ámbar | colores dados | dado | #3B1C16 | #E8C27D | 9,1:1 | 4,5:1 | OK |
| Menú: título Algoritmia (blanco sin contorno frente al ámbar, informativo, DA §7: contorno de 4 px) | colores dados | dado | #FFFFFF | #E8C27D | 1,7:1 | 4,5:1 | **FALLA** |
| Menú: subtítulo | `MainMenu_RNF20_ContrasteTextoFondo.png` @ 0.0615,0.3815,0.3333,0.4139 | oscuro/claro | #6A5349 | #F7EFE2 | 6,2:1 | 4,5:1 | OK |
| Menú: Jugar | `MainMenu_RNF20_ContrasteTextoFondo.png` @ 0.1979,0.5278,0.2396,0.5537 | oscuro/claro | #3B1C16 | #E8A33D | 7,1:1 | 4,5:1 | OK |
| Menú: Créditos | `MainMenu_RNF20_ContrasteTextoFondo.png` @ 0.0911,0.6333,0.1547,0.6593 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Menú: Salir | `MainMenu_RNF20_ContrasteTextoFondo.png` @ 0.2969,0.6333,0.3312,0.6593 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Menú: Progreso del equipo | `MainMenu_RNF20_ContrasteTextoFondo.png` @ 0.1437,0.7389,0.2932,0.7667 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |

**12 regiones · 11 cumplen · 1 no cumplen.**
