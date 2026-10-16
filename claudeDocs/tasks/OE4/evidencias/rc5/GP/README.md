# PF-RNF13-01 sobre rc5: los dos recorridos completos del arnés (16/10/2026)

Ejecutable rc5 (`Build/Algoritmia`, SHA-256 del exe `6cd16edf…adab`, huella de la carpeta `ab0c386e…46c3`, igual que `../../build-rc5.md`). Cada recorrido con un perfil nuevo, de la pantalla de inicio a los créditos, **sin «Omitir»**, con `oe4.ps1`.

| Sesión | Condición | Perfil | Inicio → créditos | Cargas en `Player.log` | Peor carga | Memoria máx. | `Exception`/`Error`/`Assert` |
|---|---|---|---|---|---|---|---|
| `GP-rc5-ventana` | Ventana 1920 × 1080 | GP5V | 12:31 → 12:55 (≈ 23 min) | 30 | 0,314 s (`Narrative`) | 1.087 MiB | 0 |
| `GP-rc5-completa` | Pantalla completa 2560 × 1440 | GP5C | 12:56 → 13:15 (≈ 19 min) | 30 | 0,239 s | 1.117 MiB | 0 |

Los tres niveles terminaron con su resumen; el Nivel 2 y el 3 quedaron desbloqueados en orden; en el laberinto (aleatorio en cada entrada) la primera secuencia del recorrido en ventana se quedó a una casilla del refugio y se corrigió; en pantalla completa llegó a la primera. **Veredicto: P.**

Límites: las duraciones incluyen la latencia del arnés y no miden a un estudiante (la referencia de 20 a 40 min se contrasta con H8). El arnés avisó de contaminación (un Word abierto en el equipo), así que cargas y memoria de estas sesiones son indicativas, no las medidas de RNF-04/05. `Restaurar-Registro` volvió a fallar (línea 1502, ya registrado como oportunidad en la Tabla 4.5 del OE4); los residuos son los valores de pantalla y sesión de Unity en `HKCU` (DEF-RC2-01). El perfil `Datos\` se borró con `Preparar-Datos -SinCarpeta`.

Archivos: `*_Player.txt`, `*_mem.csv`, `*_red.csv`, `*_registro.tsv`, `GP-rc5-*.txt` (residuos), fotos de hitos (`gp5v_*`, `gp5c_*`) y las hojas de la ejecución del laberinto (`*_r1_hoja.png`).
