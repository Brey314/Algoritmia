# S-PAU · rc3 · resumen

- **Versión:** rc3, HEAD `f49ccc9` (árbol limpio), SHA-256 del `.exe` `6cd16edf…adab`. 10/10/2026, 19:05–19:45. Ventana 1920×1080 sin marco, equipo 1. Dos lanzamientos con el arnés (`S-PAU-rc3`, `S-PAU2-rc3`).
- **Casos:** 4. **Veredictos: P 4 · PD 0 · F 0 · B 0 · NA 0.** Detalle en `veredictos.csv`.

## Cobertura de la tabla de «Cómo se llega a cada escena» (7 filas)
| Escena | Semilla | Cambio de estado (a) |
|---|---|---|
| `Level1_Cave` | OE4_Z | dos piezas en el círculo |
| `Level2_Forest` | OE4_Z | dos troncos en sus casillas |
| `Level2_Workshop` | OE4_B2 | tronco A mecanizado |
| `Level2_Maze` | OE4_B3 | ejecución en curso (pausa ~0,3 s después de «Ejecutar») |
| `Level3_River` orilla | OE4_Z | material recogido y flecha → sostenida durante la pausa |
| ensamblaje | OE4_D | tres sogas |
| ensamblaje, nivel completado | OE4_Z | tres sogas |

## Cómo se midió
- Cada paso con foto del área cliente y diferencia por píxel contra la foto de referencia (umbral 16/255): pausa abierta 0,000 % entre +0 s y +1,5 s, tras Esc y tras un clic o una flecha sostenida sobre el juego; al reanudar 0,2–1,4 % (solo la animación en reposo de los personajes).
- «Reiniciar» → confirmación → «Cancelar» / «Sí, reiniciar»; SHA-256 de `Datos/*.json` igual antes y después de reiniciar y de volver al menú.
- Laberinto: tras reiniciar, tablero distinto y secuencia vacía (`PF-RF07-01_laberinto_1.png`). Ensamblaje: reabre en el amarre con la base armada y Mamá esperando, también con el nivel completado.

## Límites de la observación
- La pausa del laberinto se tomó a ~0,3 s de pulsar «Ejecutar», no a mitad del recorrido (el arnés tarda ~1,3 s en tomar el primer cuadro); la carretilla ya se movía y siguió tras «Reanudar».
- En el bosque el clic «sobre el juego» cayó en el borde del panel de pausa; la parada se comprueba con la diferencia de píxeles y con Esc.
- Medidas: carga de las cinco jugables 0,015–0,049 s; log de ambos lanzamientos sin `Exception`, `Error` ni `Assert`.

## Defectos
- Ninguno nuevo en esta sesión.
