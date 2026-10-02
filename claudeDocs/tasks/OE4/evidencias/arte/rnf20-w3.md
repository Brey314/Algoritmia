# RNF-20: contraste texto/fondo (`arte_check.py rnf20`)

- Comando: `arte_check.py rnf20 --file cajas-rnf20-w3.txt --out rnf20-w3.md`
- Contraste WCAG 2.x: percentil 1 de la luminancia del texto contra percentil 60 del fondo dentro de la caja (coordenadas normalizadas; en claro/oscuro se usan los percentiles espejo). Mínimo RNF-20 = 4,5:1. Los colores son los del píxel que cae en cada percentil.

| Región | Captura y caja | Polaridad | Texto | Fondo | Contraste | Mínimo | Veredicto |
|---|---|---|---|---|---|---|---|
| C8 icono de alerta sobre la casilla incorrecta | `RiverLevel_RNF19_ColocacionIncorrecta.png` @ 0.5547,0.5815,0.5885,0.6389 | oscuro/claro | #320904 | #BE8084 | 5,6:1 | 3,0:1 | OK |

**1 regiones · 1 cumplen · 0 no cumplen.**
