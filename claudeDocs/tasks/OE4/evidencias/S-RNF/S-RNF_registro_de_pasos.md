# S-RNF — Consolidación de medidas sobre el ejecutable (T20 del OE4) — 01/10/2026

- **Exe:** rc1, `C:\Dev\Algoritmia\Build\Algoritmia\` (procedencia en `evidencias/build-rc1.md`). **No se lanzó el juego**: la sesión solo lee las evidencias de GP1, GP2, S-PER y S-DOC (con S-RNF07) y mide la carpeta. Editor cerrado (solo Unity Hub).
- **Herramientas de un solo uso** (scratchpad, no van al repo): `cierre\srnf\consolidar.py` (cargas, memoria y red → `consolidado.json`) y `cierre\srnf\valores.py` (→ `cierre\w8\valores.json`).
- **Evidencias:** `claudeDocs/tasks/OE4/evidencias/S-RNF/`.

| # | Hora | Acción | Observado | Veredicto |
|---|---|---|---|---|
| 1 | 12:33–12:38 | Lectura de `PROGRESO.md`, `casos.md` (S-RNF, índice, tablas de casos), `plan.md` §1–§12, `build-rc1.md`, `w8/esquema.md` §3 y los registros de pasos de S-SPIKE, GP1 (3 tramos), GP2, S-PER y S-DOC | Lista cerrada de 25 marcadores (3.4 sin `EQUIPO_DESARROLLO` ni `RNF16_*`); el arnés no registra el primer cuadro no negro de `Boot` | OK |
| 2 | 12:40 | `consolidar.py` sobre los 20 `Player.txt` (GP1 1, GP2 1, S-PER 15, S-DOC 2, S-RNF07 1) | 188 líneas `RNF-04`; las 11 escenas de `SceneLoader` con n ≥ 3; peor 0,765 s (`Narrative`, `N1_Apertura` tras crear el perfil, S-RNF07) | P (PF-RNF04-01, equipo 1) |
| 3 | 12:40 | Cota del arranque de `Boot`: hora de `Start-Process` (GP1 14:04:32,529Z, GP2 15:41:43,956Z, S-RNF07 17:28:41,888Z) → primera lectura del `Player.log` por el muestreador, que ya trae la línea de `MainMenu` | 2,62 s · 2,60 s · 3,23 s. Es cota superior: incluye la espera de la ventana y el arranque del muestreador | P (< 10 s) |
| 4 | 12:40 | `mem.csv` de las cinco sesiones | WorkingSet64 máx. 435,7 MiB (GP2, `Level3_River`); PrivateMemorySize64 máx. 1 140,1 MiB (GP1, `Narrative` al abrir `N3_PuenteII`); picos del SO iguales | P (PF-RNF05-01, equipo 1) |
| 5 | 12:40 | `red.csv` de las cinco sesiones | Filas `EXTERNA` en 20/20 lanzamientos: TCP a 34.8.90.77, 34.111.113.40, 34.107.172.168 :443 (y 34.160.10.162 en S-PER lanz. 13); ningún UDP ni `Listen` | **F (PF-RNF10-01, DEF-SPIKE-01)** |
| 6 | 12:45:29 | `oe4.ps1 Tamano` y `find -printf %s` por entrada | 478 979 915 B = 456,8 MiB = 479,0 MB sin `Datos/` (0 B) ni `*_DoNotShip` (no existe) → `S-RNF/S-RNF_tamano.txt` | P (PF-RNF06-01) |
| 7 | 12:45 | Residuos: `GP1/GP1_residuos.txt`, `GP2/GP2_residuos.txt`, `S-DOC/S-DOC_residuos_*.txt` | Cero rastros del perfil tras borrarlo; fuera de `Datos/`: `Player.log`, clave del reproductor y restos de analítica de DEF-SPIKE-01 | P (PF-RNF11-01), con DEF-SPIKE-01 |
| 8 | 12:45 | Copia a evidencias de `S-RNF_cargas.csv`, `S-RNF_consolidado.json`, `S-RNF_tamano.txt`; copia de los registros de pasos que faltaban (`GP2/GP2_registro_de_pasos.md`, `S-SPIKE/S-SPIKE_registro_de_pasos.md`) | Archivos en su sitio | OK |
| 9 | 12:46–12:51 | `claudeDocs/tasks/OE4/OE4-Resultados.md` | Cabecera de procedencia, sesiones, duración de GP1 y GP2, medidas, 99 casos (83 P, 12 P parcial, 3 F, 1 NE), seis DEF y lo pendiente | OK |
| 10 | 12:51 | `valores.py` → `cierre\w8\valores.json` | 25 marcadores más `_notas` (ajustes de texto para el integrador y dos sugeridos fuera de la lista) | OK |

## Decisiones de la consolidación

- **`CARGA_BOOT_S` = «escena inicial»**, como manda el esquema. La cota del arranque (≤ 3,23 s) va en `_notas` y en `OE4-Resultados.md` 4.1. Así `RNF04_PEOR_*` sigue siendo la peor carga de `SceneLoader`: 0,77 s.
- **`RNF05_PICO_MB` = memoria de trabajo (436)**, la que define el esquema y nombra el texto. La privada (1 140 MiB), que también juzga PF-RNF05-01, va en `_notas`, con la recomendación de añadirla al texto.
- **S-RNF07 entra en las cargas**: es el mismo build en el mismo equipo y está en la carpeta de S-DOC. Da la peor de `Narrative` (0,765 frente a 0,764 de S-PER).
- **Casos juzgados por consolidación**: PF-RF02-02, PF-RF06-02, PF-RF20-02, PF-RF20-03 y PF-RF45-04. Las sesiones los recorrieron sin nombrarlos y sus registros observan todos los esperados.
  No se juzgan PF-RF11-01, PF-RF21-01 ni PF-RF07-04: lo registrado no cubre todos sus esperados.
- **Severidad**: DEF-GP1-01 y DEF-GP1-02 pasan de «media», que no está en la escala de plan §6, a **Mayor (propuesta)**. DEF-SPIKE-01 queda en Mayor (propuesta). Las confirma Santiago en el triaje.
