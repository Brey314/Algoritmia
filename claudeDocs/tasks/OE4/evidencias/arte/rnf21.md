# RNF-21: destellos (`arte_check.py rnf21`)

- Comando: `arte_check.py rnf21 'C:/Dev/Algoritmia/Assets/Game/Art/Props/Fire/Animations/Fuego normal' 'C:/Dev/Algoritmia/Assets/Game/Art/Props/Fire/Animations/Fuego cenital' C:/Dev/Algoritmia/Assets/Game/Art/Props/Fire/Animations/Smoke --fps 30 --out rnf21.md`
- WCAG 2.3.1: tramo extremo a extremo con cambio de luminancia relativa ≥ 10 % y lado oscuro < 0,80; dos tramos opuestos = un destello; falla con más de 3 en cualquier segundo. Se evalúa el cuadro completo y cada baldosa de 3×3; se informa el peor caso. Los PNG con alfa se componen sobre #000000. Tiempo = número final del nombre / fps, o la fecha de modificación con --mtime.

| Secuencia | Cuadros | Duración | Luminancia (cuadro completo) | Tramos ≥ umbral | Destellos/s máx. | Veredicto |
|---|---|---|---|---|---|---|
| `C:/Dev/Algoritmia/Assets/Game/Art/Props/Fire/Animations/Fuego normal` | 13 | 2,57 s | 0,011–0,018 | 0 | 0 (cuadro completo) | OK |
| `C:/Dev/Algoritmia/Assets/Game/Art/Props/Fire/Animations/Fuego cenital` | 21 | 5,60 s | 0,000–0,032 | 1 | 0 (baldosa fila 2, col. 2) | OK |
| `C:/Dev/Algoritmia/Assets/Game/Art/Props/Fire/Animations/Smoke` | 33 | 8,87 s | 0,003–0,147 | 8 | 1 (baldosa fila 2, col. 2, hacia 4,47 s) | OK |

**3 secuencias · 3 cumplen · 0 no cumplen.**

---

## Lectura (revisión W3, 01/10/2026; escrita a mano, no sale del script)

- **Resultado:** las tres animaciones del N1 en PNG cumplen WCAG 2.3.1, con un máximo de 1 destello/s (el umbral es más de 3).
  - Fuego normal: 13 dibujos.
  - Fuego cenital: 21 dibujos.
  - Humo: 33 dibujos.
- **Ráfagas:** en `TestScreenshots` no hay ráfagas de capturas. `RiverLevel_RNF21_*` y `WheelLevel_RNF21_*` son capturas sueltas.
- **Límites:** se miden los cuadros de entrega, no lo que compone el motor (BurnReveal, la chispa, los fundidos), y no se evalúa el salto del bucle. Eso queda para la ráfaga del ejecutable (PF-RNF21-01).
