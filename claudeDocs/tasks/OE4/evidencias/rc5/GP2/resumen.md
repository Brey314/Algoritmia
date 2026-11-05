# PF-RNF13-01 sobre rc5: dos recorridos completos del arnés (05/11/2026)

Candidato rc5, huella global `ab0c386e87045a4c525548109fa5f9e1905a7336dbd14f21f93b97c8bcb246c3` (manifiesto coincide: 253 archivos antes y después). SHA-256 del exe `6cd16edf…adab`. Monitor único 1920×1080, así que «pantalla completa» es también 1920×1080. `Start-Juego` no avisó de contaminación en ninguno (lista vacía; Word y Unity cerrados): las medidas de carga y memoria valen para RNF-04 y RNF-05. Hora local (UTC-5).

| Recorrido | Condición | Perfil | Inicio → fin | Cargas | Peor carga | WorkingSet máx | Memoria privada máx | Exception/Error/Assert | Conexiones externas |
|---|---|---|---|---|---|---|---|---|---|
| `GP-rc5b-ventana` | Ventana 1920×1080 (sin marco) | GP6V | 08:28 → 08:43 | 31 | 0,680 s (`Narrative`) | 451 MiB | 1.184 MiB | 0 | 0 |
| `GP-rc5c-completa` | Pantalla completa 1920×1080 | GP7C | 12:01 → 12:15 | 31 | 0,714 s (`Narrative`) | 448 MiB | 1.186 MiB | 0 | 0 |

El recorrido en ventana lo hizo un agente anterior (mismas instrucciones); este lo revisé en su `Player.txt` y llega a `Credits` y `MainMenu` sin `Exception`. Sin «Omitir» en ninguno. Ambos terminan con `reachedLevel` 3 y las siete fases confirmadas.

## Peor carga por escena (s)

| Escena | Cargas (cada recorrido) | Ventana | Completa |
|---|---|---|---|
| MainMenu | 2 | 0,120 | 0,136 |
| LevelSelect | 3 | 0,085 | 0,086 |
| Narrative | 17 | 0,680 | 0,714 |
| Level1_Cave | 1 | 0,091 | 0,090 |
| LevelSummary | 3 | 0,035 | 0,036 |
| Level2_Forest | 1 | 0,053 | 0,054 |
| Level2_Workshop | 1 | 0,069 | 0,053 |
| Level2_Maze | 1 | 0,070 | 0,070 |
| Level3_River | 1 | 0,055 | 0,054 |
| Credits | 1 | 0,036 | 0,037 |

Límite RNF-04: 10 s. Límite RNF-05: 2.048 MiB (máximo 1.186 MiB). `Boot` no pasa por `SceneLoader`.

## Recorrido interrumpido

`GP-rc5b-completa` (perfil GP6C) se interrumpió por un límite de uso de otra sesión: el juego quedó abierto unas 3 h en `Level2_Maze`. No vale como medida; se guardó en `GP2/interrumpido/` y se hizo de nuevo.

## Incidencias

- Laberinto aleatorio: en la partida completa salió a la primera con 13 bloques (I=0 en JSON; el recorrido en ventana también).
- Amarre del Nivel 3: la primera vez coloqué las sogas solo en un extremo de los troncos, la balsa marcó la zona y completé el otro extremo; GP7C registra attempts 1 en las fases 2 y 3 del Nivel 3 (ventana: 0).
- Un bucle mío de «Continuar» con una función llamada `Rd` (alias de `Remove-Item`) falló, pero los clics se enviaron y no afectaron la partida; luego conté las líneas `RNF-04` nuevas para no pasarme de escena.
- DEF-RC5-14 (Observación): incoherencia de texto en el resumen del Nivel 3.
- Residuos (`residuos_*.txt`): solo `Player.log` y valores de pantalla y sesión de Unity en `HKCU` (DEF-RC2-01). `Restaurar-Registro` funcionó en ambas sesiones; `Preparar-Datos -SinCarpeta` dejó el build sin `Datos/`.
- Límites: las duraciones incluyen la latencia del arnés y no miden a un estudiante (la referencia de 20 a 40 min se contrasta con H8).

Archivos: `*_Player.txt`, `*_mem.csv`, `*_red.csv`, `*_registro.tsv`, JSON finales, `medidas_*.txt`, `residuos_*.txt` y fotos de hitos (`gp6v_*`, `gp7c_*`).
