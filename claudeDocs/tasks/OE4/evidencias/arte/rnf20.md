# RNF-20: contraste texto/fondo (`arte_check.py rnf20`)

- Comando: `arte_check.py rnf20 --file cajas-rnf20.txt --out rnf20.md`
- Contraste WCAG 2.x: percentil 1 de la luminancia del texto contra percentil 60 del fondo dentro de la caja (coordenadas normalizadas; en claro/oscuro se usan los percentiles espejo). Mínimo RNF-20 = 4,5:1. Los colores son los del píxel que cae en cada percentil.

| Región | Captura y caja | Polaridad | Texto | Fondo | Contraste | Mínimo | Veredicto |
|---|---|---|---|---|---|---|---|
| N1 tablilla de instrucción (estado más oscuro) | `FirePanel_RNF20_EstadoMasOscuro.png` @ 0.3073,0.0852,0.6927,0.1222 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Diálogo: nombre del hablante | `Personajes_N1_Hallazgo_L00.png` @ 0.2318,0.8407,0.2630,0.8611 | oscuro/claro | #6A5349 | #F7EFE2 | 6,2:1 | 4,5:1 | OK |
| Diálogo: línea | `Personajes_N1_Hallazgo_L00.png` @ 0.2318,0.8648,0.2891,0.8907 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Diálogo: botón Continuar | `Personajes_N1_Hallazgo_L00.png` @ 0.7370,0.9056,0.8177,0.9352 | oscuro/claro | #3B1C16 | #E8A33D | 7,1:1 | 4,5:1 | OK |
| Menú: título Algoritmia | `MainMenu_RNF20_ContrasteTextoFondo.png` @ 0.0599,0.3194,0.3047,0.4120 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Menú: subtítulo | `MainMenu_RNF20_ContrasteTextoFondo.png` @ 0.0599,0.4167,0.3359,0.4556 | oscuro/claro | #6A5349 | #F7EFE2 | 6,2:1 | 4,5:1 | OK |
| Menú: Jugar | `MainMenu_RNF20_ContrasteTextoFondo.png` @ 0.1938,0.5815,0.2437,0.6130 | oscuro/claro | #3B1C16 | #E8A33D | 7,1:1 | 4,5:1 | OK |
| Menú: Créditos | `MainMenu_RNF20_ContrasteTextoFondo.png` @ 0.0896,0.6889,0.1562,0.7204 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Menú: Salir | `MainMenu_RNF20_ContrasteTextoFondo.png` @ 0.2958,0.6889,0.3333,0.7204 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |
| Menú: Progreso del equipo | `MainMenu_RNF20_ContrasteTextoFondo.png` @ 0.1417,0.7944,0.2958,0.8278 | oscuro/claro | #3B1C16 | #F7EFE2 | 13,5:1 | 4,5:1 | OK |

**10 regiones · 10 cumplen · 0 no cumplen.**
