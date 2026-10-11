# RNF-21: destellos (`arte_check.py rnf21`)

- Comando: `arte_check.py rnf21 'n1_llama_*.png' 'n1_g12_*.png' 'n1_g13_*.png' 'n1_g15_*.png' --fps 10 --out C:/Dev/Algoritmia/claudeDocs/tasks/OE4/evidencias/rc3/S-RNF/rnf21-exe-n1.md`
- WCAG 2.3.1: tramo extremo a extremo con cambio de luminancia relativa ≥ 10 % y lado oscuro < 0,80; dos tramos opuestos = un destello; falla con más de 3 en cualquier segundo. Se evalúa el cuadro completo y cada baldosa de 3×3; se informa el peor caso. Los PNG con alfa se componen sobre #000000. Tiempo = número final del nombre / fps, o la fecha de modificación con --mtime.

| Secuencia | Cuadros | Duración | Luminancia (cuadro completo) | Tramos ≥ umbral | Destellos/s máx. | Veredicto |
|---|---|---|---|---|---|---|
| `n1_llama_*.png` | 60 | 5,90 s | 0,135–0,141 | 0 | 0 (cuadro completo) | OK |
| `n1_g12_*.png` | 25 | 2,40 s | 0,131–0,131 | 0 | 0 (cuadro completo) | OK |
| `n1_g13_*.png` | 25 | 2,40 s | 0,130–0,130 | 0 | 0 (cuadro completo) | OK |
| `n1_g15_*.png` | 25 | 2,40 s | 0,131–0,131 | 0 | 0 (cuadro completo) | OK |

**4 secuencias · 4 cumplen · 0 no cumplen.**
