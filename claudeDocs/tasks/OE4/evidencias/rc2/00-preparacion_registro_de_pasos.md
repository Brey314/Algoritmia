# Reverificación rc2 · preparación (01/10/2026)

| # | Hora | Acción | Observado | Veredicto |
|---|---|---|---|---|
| 1 | 18:09 | Procesos | Editor de Unity **cerrado**; Rider abierto (no cuenta: el arnés solo avisa de Unity y Word); Unity Hub abierto | OK |
| 2 | 18:10 | Pantalla (`oe4.ps1 Estado`, DPI por monitor) | **Cambió respecto de W5:** dos monitores. Principal 2560×1440 al 100 % en (0,0); secundario 1920×1080 (1536×864 lógicos = 125 %) en (2560,165). Escritorio virtual 4480×1440. En W5 (rc1) era un solo monitor de 1920×1080 al 125 %; el revisor de W5 ya vio el de 2560×1440 | Línea base rehecha (abajo) |
| 3 | 18:10 | Uso del equipo | **Santiago está usando el equipo**: Hearts of Iron IV (pid 27536, desde las 14:04) en primer plano a pantalla completa en el principal; `GetLastInputInfo` da 0–266 ms de inactividad durante 20 s y el cursor se mueve | Bloquea las sesiones con el exe |
| 4 | 18:11 | Aviso a Santiago (notificación) | «la reverificación del rc2 necesita el ratón y la pantalla ~1-2 h. Arranco cuando el equipo lleve 5 min sin uso; si lo tocas, el arnés se detiene solo.» | — |
| 5 | 18:11 | `Tamano` | 478 987 619 B = 456,8 MiB = **479,0 MB** (< 500 MB); `_DoNotShip` 0,7 MiB aparte; `Datos` vacía o ausente | **P (PF-RNF06-01)** |
| 6 | 18:11 | Huella del contenido de `Algoritmia_Data` (comando de `build-rc2.md`) | `a613b851…c887a704`, 223 archivos: **es el rc2** de `build-rc2.md`. SHA-256 del exe `6CD16EDF…ADAB` (el lanzador, igual que rc1). `globalgamemanagers`: 0 apariciones de `unity3d.com` | OK |

## Línea base de coordenadas

Las coordenadas del arnés son fracciones del área cliente y la interfaz del juego se escala por altura
(16:9), así que las de `oe4/coords.json` (rc1, 1920×1080) valen en cualquier cliente 16:9: 1920×1080 en ventana
sobre el monitor de 2560×1440 y 2560×1440 a pantalla completa. Lo que cambia en rc2 se midió sobre las capturas
de la revisión (`review/rc2-capturas`, `rc2-rev2-capturas`, 1920×1080):

| Pantalla | Elemento | (x; y) |
|---|---|---|
| ¿Quién juega? | ▲ / ▼ de la lista (3 perfiles por página) | (0,409; 0,222) / (0,447; 0,222) |
| ¿Quién juega? | filas 1–3 (nombre) · papelera | y = 0,305 / 0,419 / 0,534 · x = 0,292 · papelera x = 0,439 |
| ¿Quién juega? | Volver · Continuar · campo | (0,41; 0,896) · (0,558; 0,893) · (0,683; 0,354) |
| Diálogo «Salir» | Quedarme · Cerrar el juego | (0,495; 0,624) · (0,633; 0,624) |
| Cueva (panel) | Soplar · Golpear | (0,410; 0,896) · (0,589; 0,896) |
| Laberinto | ▲ / ▼ · pausa · Ejecutar | (0,8667; 0,0926) / (0,9005; 0,0926) · (0,944; 0,096) · (0,827; 0,894) |

Se confirman con la primera foto de cada pantalla en cada sesión.

## Herramientas nuevas (no son juego; no tocan el arnés)

- `idle.ps1`: espera a que el equipo lleve N minutos sin entrada humana.
- `combo.ps1`: doble clic a < 150 ms y ráfaga **en el mismo proceso** (el arnés tarda 1–2 s por orden y su `Clic -Veces 2` da ~150 ms entre pulsaciones por el «Nudge»). Reutiliza la clase nativa `Oe4` extrayendo su fuente de `oe4.ps1`.
- `matar-al-guardar.ps1`: `FileSystemWatcher` sobre `Datos\` que mata el juego en el primer evento de escritura (cierre forzado en el instante del guardado).

## Espera

| Hora | Resultado |
|---|---|
| 18:17–18:27 | Sin 5 min seguidos de inactividad |
| 18:28–18:38 | Sin 4 min seguidos de inactividad |
