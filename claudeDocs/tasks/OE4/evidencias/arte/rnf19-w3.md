# RNF-19: parejas en escala de grises (`arte_check.py rnf19`)

- Comando: `arte_check.py rnf19 --file parejas-rnf19-w3.txt --out rnf19-w3.md`
- Criterio: se distinguen si **1−IoU de la silueta ≥ 0,20** (forma) o **diferencia media de gris ≥ 25** de 255 (brillo). Gris Rec. 601 sobre gris medio; lienzo de 256×256 (con alfa, encajado; captura, estirada). «n/d»: sin transparencia en alguna de las dos, solo cuenta el gris. La columna «Δ ≥ 25» es informativa: un cambio pequeño y localizado (candado, visto) da diferencia media baja aunque se vea.

| Grupo | A | B | 1−IoU | Δ gris medio | Δ ≥ 25 | Veredicto |
|---|---|---|---|---|---|---|
| B8 seto interior/camino | `Maze_01_reposo.png@0.2161,0.3519,0.2474,0.4148` | `Maze_01_reposo.png@0.2500,0.2315,0.2813,0.2870` | n/d | 85,0 | 100,0 % | OK (gris) |
| B8 seto del borde/camino | `Maze_01_reposo.png@0.1562,0.1852,0.5208,0.2176` | `Maze_01_reposo.png@0.2500,0.2315,0.2813,0.2870` | n/d | 133,9 | 100,0 % | OK (gris) |
| C8 vacío/correcto (casilla 5) | `AssemblyPanel_HU12_Base.png@0.6200,0.6480,0.7470,0.8330` | `RiverLevel_RNF19_ColocacionIncorrecta.png@0.6200,0.6480,0.7470,0.8330` | n/d | 47,2 | 61,4 % | OK (gris) |
| C8 vacío/incorrecto (casilla 1) | `AssemblyPanel_HU12_Base.png@0.5080,0.5350,0.6350,0.7200` | `RiverLevel_RNF19_ColocacionIncorrecta.png@0.5080,0.5350,0.6350,0.7200` | n/d | 24,8 | 34,5 % | **FALLA** |
| C8 incorrecto/correcto | `RiverLevel_RNF19_ColocacionIncorrecta.png@0.5080,0.5350,0.6350,0.7200` | `RiverLevel_RNF19_ColocacionIncorrecta.png@0.6200,0.6480,0.7470,0.8330` | n/d | 46,3 | 75,8 % | OK (gris) |

**5 parejas · 4 se distinguen · 1 no cumplen el umbral.**
