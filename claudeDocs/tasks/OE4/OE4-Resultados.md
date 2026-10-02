# Resultados de las pruebas funcionales — OE4

> **Dos pasadas el 01/10/2026.**
> - **Primera, sobre rc1 (§1–§7):** el spike, los dos *Golden Path* (GP1 y GP2), S-PER, S-DOC con la copia
>   portable de RNF-07, la exclusión de RNF-16 y esta consolidación de medidas (S-RNF, T20). Incluye las
>   correcciones de la **revisión adversarial de W5** (14:00–14:45): siete veredictos reclasificados (marcados
>   «rev. W5» en §5), PF-RNF16-01 ejecutado (§4.7), dos defectos nuevos (DEF-W5R-01 y DEF-W5R-02) y «P parcial»
>   fuera del cómputo (§7).
> - **Segunda, sobre rc2 (§8):** corrección de los defectos de rc1, procedencia del build rc2 y reverificación
>   sobre el ejecutable (19:41–20:40), con un tercer *Golden Path* (GP3).
> - **Revisión final de rc2 (01/10, 21:00–21:45):** no cambia ningún veredicto. Corrige las frases que decían más
>   de lo medido (el segundo cierre forzado fue «justo después del reemplazo», la red «no se observó», el resto de
>   HKCU es «un contador y un identificador»), copia a `evidencias/rc2/` la evidencia que solo estaba en la carpeta
>   temporal de la sesión, añade el riesgo residual R-RC2-A y deja escritas las decisiones que Claude tomó por
>   delegación de Santiago (§8.10).
>
> **El resto del catálogo (`casos.md`) queda para el OE4**, igual que la matriz de trazabilidad (T25),
> el KPI (T24) y el triaje de defectos (T21), que decide Santiago (plan §6). Los defectos y observaciones hallados
> hasta rc2 ya tienen decisión, tomada por Claude por delegación de Santiago (§8.3 y §8.10).
> Reglas de veredicto: `plan.md` §3.2 y §4. Evidencias: `evidencias/<sesión>/`; los registros de pasos de
> cada sesión van en esa misma carpeta (`*_registro_de_pasos.md`).

## 1. Versión que se prueba

Procedencia completa en `evidencias/build-rc1.md`. No hay etiqueta ni commit: Santiago hace un solo commit al final.

| Dato | Valor |
|---|---|
| Candidato | rc1: `Build/Algoritmia/`, 12 escenas (`Boot` primera), con `Licencias/` (OFL y Phosphor) |
| Rama y HEAD | `feat/cierre-de-slices-y-oe3` sobre `359365e0a351bd2b8e64bbd0bb75b1c8cef9b2f5` |
| Huella del árbol (`git diff --binary --no-textconv --no-ext-diff HEAD \| sha256sum`) | completo `9f9c1548…291ae01c`; solo `Assets Packages ProjectSettings` `659bea01…c5658468` (iguales antes y después del build) |
| Build | 01/10/2026 08:29:53–08:30:10; Unity 6000.5.10f1, StandaloneWindows64, Mono, sin development build |
| SHA-256 de `Algoritmia.exe` | `6cd16edfbe85284eb69cd2efe1170aa1cffdf0dbdb309c3269e2f8454811adab`, comprobado por `Start-Juego -Sha256` en todos los lanzamientos del día |
| Tamaño de la carpeta sin `Datos/` | 478 979 915 B (456,8 MiB = 479,0 MB) |
| Huella del contenido (revisión de W5) | `af4278fbab6f09b0aa381b91e06a7ff3b4b909855f1e8704845f5fd0e70bb28e`, 252 archivos sin `Datos/`; método en `build-rc1.md` |
| Equipo 1 | AMD Ryzen 5 7600X (6 núcleos, 12 hilos), 32 GB de RAM (30,9 GiB), NVIDIA GeForce RTX 5070 Ti de 16 GB, Windows 11 Home Single Language 10.0.26200 de 64 bits; un monitor de 1920 × 1080 al 125 % |
| Condiciones | Editor de Unity cerrado en todas las sesiones (solo Unity Hub, que el arnés no cuenta como contaminación); nada más abierto |
| Arnés | `herramientas/oe4.ps1`, con los cuatro arreglos del spike (`Capture`, `Nudge`, lanzamiento sin marco, hora de escritura de la carga) y el de `Foto -Gris` (GP1 tramo 3). Es herramienta, no juego |

**Límite de la guarda.** El `.exe` es solo el lanzador: su SHA-256 es idéntico al del build descartado de 827 MB
y al de rc2. Lo que identifica el contenido es la **huella del contenido** de la carpeta (fila anterior), que la
revisión de W5 calculó después de las sesiones y que se recalculó igual el 01/10 por la noche. La huella del árbol
ya no se reproduce, porque el árbol siguió cambiando después del build (`build-rc1.md`).

## 2. Sesiones del 01/10/2026

| Sesión | Horas (Bogotá) | Lanzamientos | Qué cubrió | Registro de pasos |
|---|---|---|---|---|
| S-SPIKE | 08:45–09:00 | 4 | Spike del arnés (T03): **pasa**. Halla DEF-SPIKE-01 | `S-SPIKE/S-SPIKE_registro_de_pasos.md` |
| GP1 | 09:04:32–10:31:30 | 1 proceso, 3 tramos | PF-RNF13-01 n.º 1, ventana sin marco 1920 × 1080: S-INI (parcial), S-N1, S-N2A–C, S-N3 y la segunda visita del N1 | `GP1/GP1_tramo{1,2,3}_registro_de_pasos.md` |
| GP2 | 10:41:44–11:14:41 | 1 | PF-RNF13-01 n.º 2, pantalla completa | `GP2/GP2_registro_de_pasos.md` |
| S-PER | 11:23:50–12:14 | 15 | Siete cierres forzados, «Salir», perfil ilegible, nombre reservado | `S-PER/S-PER_registro_de_pasos.md` |
| S-DOC | 12:16–12:27 | 2 | Informe docente, borrado, residuos, ruta de respaldo (INC-34), JSON persistidos | `S-DOC/S-DOC_registro_de_pasos.md` |
| S-RNF07 | 12:28–12:31 | 1 | PF-RNF07-01: copia portable en `D:\User\Desktop\Algoritmia prueba\` (dentro de S-DOC) | ídem |
| S-RNF | 12:40–12:50 | 0 | Esta consolidación (T20), con el juego cerrado: solo lee las evidencias y mide el tamaño | `S-RNF/S-RNF_registro_de_pasos.md` |
| RNF16 | ~13:50–14:19 | 0 | PF-RNF16-01 en los dos sentidos, sobre una copia del árbol del rc1 (§4.7) | `RNF16/README.md` |
| W5-REV | 14:27:55–14:29 | 1 | Revisión de W5: «¿Quién juega?» con 8 perfiles (PF-RF02-06, DEF-W5R-01) | `W5-REV/` (los pasos están en el informe de la revisión) |

## 3. Duración de los dos recorridos completos (PF-RNF13-01)

| Recorrido | Condición | Inicio → cierre (UTC) | Duración | Tiempo de resolución en el JSON (7 fases) |
|---|---|---|---|---|
| GP1 | Perfil nuevo `OE4GP1`, ventana sin marco 1920 × 1080, un solo proceso (pid 40152) | 14:04:32,529Z → 15:31:30,216Z | **1 h 26 min 57,7 s (87 min)** | 2137,94 s (35 min 38 s) |
| GP2 | Perfil nuevo `OE4GP2`, pantalla completa, un solo proceso (pid 44004) | 15:41:43,956Z → 16:14:40,762Z | **32 min 56,8 s (33 min)** | 792,33 s (13 min 12 s) |

- **GP1** lo jugaron tres agentes en serie sobre el mismo proceso: tramo 1 (S-INI y N1) 16 min 13 s, relevo 3 min 26 s,
  tramo 2 (N2) 37 min 29 s, relevo 5 min 32 s, tramo 3 (N3, cierre y segunda visita del N1) 24 min 18 s. Durante los
  relevos (8 min 58 s) el juego esperó en el menú de niveles. Incluye las pruebas de error a propósito del guion.
- **GP2** lo jugó un solo agente, sin errores a propósito.
- Las dos cifras incluyen la latencia del arnés (1–2 s por llamada), las fotos de control y la lectura de los
  tableros del laberinto. **No miden a un estudiante**: esa duración (OE1 §2.3, 20–40 min) sale del recorrido
  con cronómetro de Santiago (Hoja-HUM, H8).
- Los dos terminaron **sin bloqueos, cierres inesperados ni estados irrecuperables**, sin `Exception` ni `Error`
  en `Player.log` y con las siete fases en el JSON (`GP1/GP1_OE4GP1_final.json`, `GP2/GP2_OE4GP2_final.json`).

## 4. Medidas sobre el ejecutable (S-RNF, T20)

Fuentes: los `Player.txt` (líneas `RNF-04`), `mem.csv`, `red.csv` y `residuos` de GP1, GP2, S-PER y S-DOC (con S-RNF07).
Consolidado en `S-RNF/S-RNF_cargas.csv` (las 188 cargas, con la escena anterior), `S-RNF/S-RNF_consolidado.json`
y `S-RNF/S-RNF_tamano.txt`.

### 4.1 Cargas de escena (RNF-04, PF-RNF04-01)

188 cargas de 20 lanzamientos (GP1 37, GP2 31, S-PER 95, S-DOC 17, S-RNF07 8), leídas de la línea
`RNF-04: «<escena>» cargó en N s` que `SceneLoader` escribe en `Player.log`. Presupuesto: < 10 s.

| # | Escena | n | Mín. (s) | Mediana (s) | **Peor (s)** | Dónde fue la peor | < 10 s |
|---|---|---|---|---|---|---|---|
| 1 | `Boot` | 3 | — | — | **≤ 3,23** (cota del arranque, ver abajo) | S-RNF07 | Sí |
| 2 | `MainMenu` | 31 | 0,051 | 0,087 | **0,108** | S-PER lanz. 8, al arrancar | Sí |
| 3 | `LevelSelect` | 27 | 0,066 | 0,069 | **0,072** | S-PER lanz. 7 | Sí |
| 4 | `Credits` | 3 | 0,035 | 0,036 | **0,040** | GP1, desde el inicio | Sí |
| 5 | `Narrative` | 89 | 0,035 | 0,036 | **0,765** | S-RNF07, `N1_Apertura` tras crear el perfil | Sí |
| 6 | `Level1_Cave` | 7 | 0,154 | 0,155 | **0,156** | GP1 | Sí |
| 7 | `LevelSummary` | 8 | 0,035 | 0,035 | **0,035** | todas iguales | Sí |
| 8 | `Level2_Forest` | 4 | 0,052 | 0,052 | **0,052** | todas iguales | Sí |
| 9 | `Level2_Workshop` | 5 | 0,052 | 0,069 | **0,069** | GP1 | Sí |
| 10 | `Level2_Maze` | 4 | 0,053 | 0,069 | **0,071** | GP2 | Sí |
| 11 | `Level3_River` | 6 | 0,052 | 0,053 | **0,086** | S-PER lanz. 10 | Sí |
| 12 | `TeacherReport` | 4 | 0,051 | 0,054 | **0,057** | S-DOC lanz. 1 | Sí |

- **Peor carga medida por `SceneLoader`: 0,765 s**, `Narrative` al abrir `N1_Apertura` desde el menú de niveles.
  `Narrative` tarda ~0,7 s cuando llega del menú, de una mecánica o de un resumen, y 0,035 s entre dos narrativas
  encadenadas.
- **`Boot` no pasa por `SceneLoader`** y no deja línea `RNF-04`. El arnés tampoco mide el «primer cuadro no negro»
  que preveía el plan §5: su `Esperar-Carga` lee la línea del registro (`casos.md`, cabecera). Lo que sí registra
  es la hora de `Start-Process` y la hora a la que el muestreador lee por primera vez el `Player.log`, que ya trae la
  línea de `MainMenu`. Esa diferencia es una **cota superior del arranque hasta `MainMenu` cargado**, porque incluye
  la espera de una ventana estable (≥ 3 sondeos de 300 ms) y el arranque del muestreador:
  - GP1: 14:04:32,529Z → 14:04:35,150Z = **2,62 s**;
  - GP2: 15:41:43,956Z → 15:41:46,557Z = **2,60 s**;
  - S-RNF07 (copia en `D:`): 17:28:41,888Z → 17:28:45,117Z = **3,23 s**.

  Los otros 17 lanzamientos no conservan la hora exacta de `Start-Process`. El cronómetro del arranque en el
  equipo 2 es de Santiago (PF-RNF07-02, Hoja-HUM H3).
- **Veredicto PF-RNF04-01: P en el equipo 1.** Las once escenas que carga `SceneLoader` tienen ≥ 3 medidas y la peor
  queda < 1 s; el arranque con `Boot` queda < 3,3 s por su cota. Falta la columna del equipo 2 (HUM).

### 4.2 Memoria (RNF-05, PF-RNF05-01)

Muestras cada 2 s de `WorkingSet64`, `PrivateMemorySize64` y los picos del sistema operativo (MiB). La escena es la
última que `SceneLoader` registró.

| Sesión | Muestras | WorkingSet64 máx. (escena) | PrivateMemorySize64 máx. (escena) | Pico de WS del SO | Pico de *commit* del SO |
|---|---|---|---|---|---|
| GP1 | 2608 | 399,4 (`Narrative`, `N2_Escena24_Regreso`) | **1 140,1** (`Narrative`, `N3_PuenteII`) | 399,4 | 1 140,1 |
| GP2 | 987 | **435,7** (`Level3_River`, balsa terminada, antes del cruce) | 1 132,6 (`Narrative`, `N3_PuenteII`) | 435,7 | 1 132,6 |
| S-PER | 1364 | 430,6 (`Narrative`) | 1 128,0 (`Narrative`) | 430,6 | 1 128,0 |
| S-DOC | 216 | 422,6 (`MainMenu`, al volver del informe) | 1 123,0 (`Level1_Cave`) | 422,6 | 1 123,0 |
| S-RNF07 | 46 | 416,3 (`Level1_Cave`) | 1 121,2 (`Level1_Cave`) | 416,3 | 1 121,2 |

- **Picos de los dos recorridos (y de todas las sesiones):** memoria de trabajo **435,7 MiB** en `Level3_River`
  (GP2, 16:09:36Z, con la balsa recién probada); memoria privada **1 140,1 MiB** en `Narrative` al abrir
  `N3_PuenteII` (GP1, 15:07:49Z). Los dos < 2048 MiB: **RNF-05 cumple en el equipo 1**, con un 44 % de margen sobre
  la privada.
- La memoria privada es la medida estable: sube de ~700 MiB en el inicio a ~1,1 GiB en cuanto se abre la primera
  narrativa y se queda ahí. La de trabajo depende de lo que Windows deja residente: en GP1 bajó a ~210 MiB después
  del laberinto sin que el juego cambiara de comportamiento.
- **Veredicto PF-RNF05-01: P en el equipo 1.** Falta el Administrador de tareas del equipo 2 (HUM).

### 4.3 Tamaño del paquete (RNF-06, PF-RNF06-01)

`oe4.ps1 Tamano` (01/10/2026 12:45, juego cerrado): **478 979 915 B = 456,8 MiB = 479,0 MB** sin `Datos/` (vacía, 0 B)
ni `*_DoNotShip` (este build no la generó, así que el tamaño con ella es el mismo). Margen: 21 MB sobre 500 MB decimales.

| Parte | MB | Nota |
|---|---|---|
| `Algoritmia_Data/` | 409,7 | `sharedassets4.assets.resS` (texturas del Nivel 1) 276,4; `Managed/` 47,1 |
| `UnityPlayer.dll` | 37,3 | biblioteca del reproductor |
| `DirectML.dll` | 14,0 | decisión D3 de `build-rc1.md`: se queda |
| `MonoBleedingEdge/` | 9,1 | entorno Mono |
| `D3D12/` | 4,7 | `D3D12Core.dll` |
| `dstorage.dll` + `dstoragecore.dll` | 1,7 | |
| `UnityCrashHandler64.exe` | 1,7 | |
| `Algoritmia.exe` | 0,7 | |
| `Licencias/` | 0,006 | `OFL.txt`, `LICENSE-Phosphor.txt` |

**Veredicto PF-RNF06-01: P.**

### 4.4 Red (RNF-10, PF-RNF10-01)

| Sesión | Filas `EXTERNA` | Direcciones remotas (TCP) | Lanzamientos con conexión externa |
|---|---|---|---|
| GP1 | 3133 | 34.8.90.77, 34.111.113.40, 34.107.172.168 (:443) | 1/1 |
| GP2 | 1287 | las mismas tres | 1/1 |
| S-PER | 2946 | las tres y, en el lanzamiento 13, 34.160.10.162 (:443) | 15/15 |
| S-DOC | 455 | las mismas tres | 2/2 |
| S-RNF07 | 138 | las mismas tres | 1/1 |

- En **todos** los lanzamientos (20, y los 4 del spike) el proceso mantiene desde el primer segundo **tres conexiones
  TCP establecidas con direcciones de Google Cloud en el puerto 443**. Una cuarta, 34.160.10.162, aparece solo en el
  lanzamiento 13 de S-PER.
- **Ningún UDP y ningún socket en `Listen`.** Las filas «escucha-local» del muestreador (otras tantas) son los extremos
  `Bound` 0.0.0.0 de esas mismas conexiones salientes.
- La causa probable son los servicios de Unity (Analytics y Connect), que el reproductor llevó activos: `UnityConnectSettings`
  estaba en `m_Enabled: 1` durante el build. Lo apoyan los archivos `Unity\<id>\Analytics` e `Insights` en LocalLow, los
  valores `unity.*` y `unity_connect.*` del registro y las URL `*.cloud.unity3d.com` de `globalgamemanagers`
  (DEF-SPIKE-01).
- **Veredicto PF-RNF10-01: F (DEF-SPIKE-01). RNF-10 no se cumple con rc1**, y RNF-08 queda en riesgo hasta su guion H4
  (modo avión).

### 4.5 Residuos (RNF-11, PF-RNF11-01)

- **Rastro del perfil:** tras borrar OE4N1b (desde el panel) y OE4GP1, OE4_B y OE4_Z (desde el informe), **cero
  apariciones** de los cuatro nombres en `Build/Algoritmia/` entero, en `LocalLow\Universidad Catolica de Colombia\Algoritmia\`
  (con `Player.log` y los archivos de analítica), en lo nuevo de `%TEMP%` y en `HKCU\Software\Universidad Catolica de Colombia\Algoritmia`
  (`S-DOC/S-DOC_residuos_rastreo_tras_borrado.txt`).
- En GP1, GP2 y S-RNF07, el nombre del perfil solo aparece en `Datos/<nombre>.json` de su carpeta portable.
- **Fuera de `Datos/`** el juego deja:
  - `Player.log` (y `Player-prev.log`);
  - la clave del reproductor con 16 valores de pantalla;
  - y, por DEF-SPIKE-01, los archivos de Unity Analytics, Insights y `ShaderVariantAnalytics` en LocalLow y 6 valores
    `unity.*`/`unity_connect.*` en esa clave.

  `%TEMP%`: nada del juego. La decisión del 29/09 («fuera de `Datos/` solo el registro y las claves de pantalla») no
  se cumple por DEF-SPIKE-01.
- **Veredicto PF-RNF11-01: P** (el caso mide el rastro del perfil). La parte de portabilidad va con DEF-SPIKE-01.
- `Restaurar-Registro` dejó las claves del producto y de la empresa como estaban antes de cada sesión: no existían.

### 4.6 Portabilidad en el equipo 1 (RNF-07, PF-RNF07-01)

- **La copia.** La carpeta entregable sin `Datos/` (478 979 915 B = 479,0 MB = 456,8 MiB, 252 archivos, idéntica según `diff -rq`) se copió a
  `D:\User\Desktop\Algoritmia prueba\`.
  - Es el escritorio real: `%USERPROFILE%\Desktop` está redirigido a `D:` en este equipo.
  - La ruta tiene espacio y queda fuera del repositorio.
- **El arranque.**
  - Arrancó sin instalar nada.
  - El token no estaba elevado (`TokenElevation=0`) y el manifiesto pide `asInvoker`.
  - Creó `Datos/` junto a ese ejecutable al abrir «¿Quién juega?» y guardó allí `OE4P7.json`.
  - Se jugó hasta la cueva y «Salir» reescribió el perfil en la copia.
  - `Build/Algoritmia/Datos` no se tocó.
- **Al terminar.** La copia temporal se borró.
- **El proceso corrió sin elevación de privilegios** (`TokenElevation=0`, token limitado), pero desde una cuenta que es
  administradora del equipo: la prueba con una cuenta estándar queda para el equipo 2.
- **Veredicto PF-RNF07-01: P.** RNF-07 exige además PF-RNF07-02 (equipo 2, Santiago).

### 4.7 Exclusión de un nivel (RNF-16, PF-RNF16-01)

Se ejecutó el 01/10 (terminó a las 14:19), después de la primera versión de este documento. Se hizo sobre una copia
temporal del árbol del rc1 (`C:\Dev\Algoritmia-rnf16`, solo `Assets/`, `Packages/` y `ProjectSettings/`), con
`unity test` en batchmode a 640 × 480 y el método de `casos.md`. La copia se borró al terminar y el repositorio no cambió.
Detalle, `Editor.log` y XML: `RNF16/README.md`.

| Corrida | Total | Pasan | Fallan | Omitidas | Errores de compilación |
|---|---|---|---|---|---|
| Sin N2 · EditMode | 360 | 354 | 5 | 1 | 0 |
| Sin N2 · PlayMode | 271 | 245 | 17 | 9 | 0 |
| Sin N1 · EditMode | 382 | 376 | 5 | 1 | 0 |
| Sin N1 · PlayMode | 305 | 281 | 22 | 2 | 0 |
| Control: árbol completo, solo las 14 de entorno | 38 | 24 | 14 | 0 | 0 |

Los fallos son de tres clases, y ninguno señala una dependencia entre niveles:
- **A. Estructurales** (5 en cada sentido): `Architecture_RNF15_*` (2), `Architecture_RNF16_*` (2) y
  `Traceability_CT10_*`. Leen el árbol y exigen los tres niveles.
- **B. Transversales que cargan por nombre una escena retirada:** 6 sin N2 y 8 sin N1. Son las que prevé `casos.md`.
- **C. Entorno** (batchmode a 640 × 480, sin cuadros reales): 11 sin N2 y 14 sin N1. El control las reproduce en el
  árbol completo sin quitar nada.

**Veredicto PF-RNF16-01: P con reserva.** En los dos sentidos compila con 0 errores, y las pruebas de `Game.Core`,
`Game.Scaffolding`, `Game.Audio`, `Game.Reporting`, `Game.UI` y del nivel que queda pasan, salvo las listadas.
**Reserva:** la letra del esperado solo admite los fallos B. Los A y C quedan fuera de esa letra aunque no midan la
independencia: A mide el árbol completo y C el entorno, como demuestra el control. RNF-16 cumple.

## 5. Veredictos de los casos ejecutados sobre rc1

Versión **rc1**, fecha **01/10/2026**, en todos.

**Leyenda.**
- **P parcial:** lo observado cumple, pero falta parte de los pasos o del ejecutor que el caso agrupa (otra escena, la
  SUITE, el equipo 2). **No es un veredicto de la escala de `plan.md` §3.2** (P, PD, F, B, NA): es un caso a medio
  ejecutar. No cuenta como aprobado ni entra en el KPI hasta completarse (§7).
- **(consolidación):** el caso no estaba en el guion de la sesión que lo recorrió, pero su registro de pasos observa
  todos sus esperados; se juzga aquí con esa cita.
- **rev. W5:** veredicto corregido por la revisión adversarial de W5 al contrastarlo con su propia evidencia. Entre
  paréntesis va el veredicto que tenía en la primera versión.
- **P con reserva:** cumple el requisito, con una salvedad que se dice en la nota. Cuenta como P.

Rutas de evidencia relativas a `evidencias/`.

| Caso | Req | Etq | Ejec. | Veredicto | Evidencia | DEF | Nota |
|---|---|---|---|---|---|---|---|
| PF-RF01-01 | RF-01 | NAV | EXE | P | `GP1/PF-RF01-01_inicio.png`; GP1 t1 pasos 5–6 | | Teclas sin efecto |
| PF-RF02-01 | RF-02 | DAT | EXE | P parcial | GP1 t1 paso 12; `GP2/PF-RF02-01_perfil_nuevo_OE4GP2.png` | | Solo el nombre válido; los rechazos (vacío, espacios, duplicado, inválido) son de S-INI |
| PF-RF02-02 | RF-02 | DAT | EXE | P (consolidación) | `S-PER/S1-cerrar-reabrir_progreso_conservado.png`; `GP1/PF-RF03-02_menu_tres_completados.png` | | Elegir un perfil abre el menú con su progreso exacto |
| PF-RF02-04 | RF-02, RNF-13 | DAT | EXE | P | `S-PER/PF-RF02-04_*.png`; `S-PER/logs/S-PER_lanz13_Player.txt` | DEF-SPER-02 | Operable y borrable; la `Exception` es DEF Menor, como prevé el caso |
| PF-RF02-05 | RF-02 | DAT | EXE | P | `S-PER/PF-RF02-05_*.png` | | `CON.json` funciona en este Windows 11 (OBS-SPER-4: repetir en el equipo 2) |
| **PF-RF02-06** | RF-02, RNF-03 | DAT | EXE | **F** (rev. W5; caso nuevo) | `W5-REV/DEF-W5R-01_ocho_perfiles_desbordan_el_panel.png`; `W5-REV/W5-REV_Player.txt` | **DEF-W5R-01** | Caso propuesto por la revisión («con ≥ 8 perfiles, todos visibles y seleccionables»), que lo ejecutó sobre el exe: del 5.º perfil en adelante quedan fuera del panel o tapados, y ni el arrastre ni la rueda desplazan la lista |
| PF-RF03-02 | RF-03 | NAV | EXE | P | `GP1/PF-RF03-02_menu_tras_N1.png`, `_tras_N2.png`, `_tres_completados.png` | | |
| PF-RF03-03 | RF-03 | NAV | EXE | P | GP1 t1 paso 49; `GP1/PF-RF03-02_menu_tras_N1.png` | | Foto idéntica tras el clic; «Completado» con visto (OBS-5) |
| PF-RF04-01 | RF-04, RF-45 | DAT | EXE | P | `GP1/GP1_OE4GP1_tras_N1.json`; `GP2/GP2_OE4GP2_tras_N1.json` | | Tiempos ±0,7 s del cronómetro del arnés |
| PF-RF04-02 | RF-04 | DAT | EXE | P | `GP1/GP1_OE4GP1_tras_N2.json`; `GP2/GP2_OE4GP2_tras_N2.json` | | |
| PF-RF04-03 | RF-04 | DAT | EXE | P | `GP1/GP1_OE4GP1_final.json`; `GP2/GP2_OE4GP2_final.json` | | La 3.ª fase se guarda al salir al cruce |
| PF-RF05-01 | RF-05 | NAV | EXE | P | `GP1/PF-RF10-01_hallazgo_L18.png`; GP1 t1 pasos 14–19 y 45 | | |
| PF-RF05-02 | RF-05 | NAV | EXE | P | GP1 t2 pasos 2–5, 21–22, 41 y 61 | | Siete narrativas, una línea por clic |
| PF-RF05-03 | RF-05 | NAV | EXE | P | `GP1/PF-RF05-03_*.png`; GP2 paso 60 | | La 3.2 solo tras el primer fallo; sin fallo, directo al cruce (OBS-GP2-1) |
| PF-RF06-01 | RF-06 | BOT | EXE | P | `GP2/PF-RF06-01_N1_Apertura_sin_omitir.png`; `GP1/PF-RF06-01_N1_segunda_visita_omitir_INC28.png` | | Sin «Omitir» la primera vez; con él en la segunda visita (INC-28) |
| PF-RF06-02 | RF-06 | BOT | EXE | P parcial (rev. W5; antes P por consolidación) | `S-PER/PF-RNF14-04_rejugar_N2_narrativas_con_omitir.png`; S-PER pasos 40–41; S-SPIKE paso 30 | | «Omitir» visto y usado en el N2 y el N3; la cadena del N1 (tres «Omitir» hasta la cueva) no se ejercitó: GP1 t3 paso 40 solo ve el botón |
| PF-RF07-01 | RF-07 | BOT | EXE | P parcial | GP1 t1 paso 34; `S-PER/PF-RNF14-05_pausa_abierta_antes_de_matar.png`; S-PER paso 40 | | Visto en `Level1_Cave`, `Level2_Workshop` y `Level3_River`; faltan `Level2_Forest` y `Level2_Maze` (S-PAU) |
| PF-RF07-02 | RF-07 | BOT | EXE | P parcial | GP1 t1 paso 35 | | Solo en el N1, tras 32,1 s de pausa; los otros siete casos son de S-PAU |
| PF-RF07-05 | RF-07 | BOT | EXE | P | GP1 t1 paso 13 y todas las narrativas | | |
| PF-RF07-06 | RF-07 | DAT | EXE | P | GP1 t1 paso 47 | | 384,98 s en el JSON frente a 384,3 s sin la pausa |
| PF-RF08-01 | RF-08 | NAV | EXE | P | `GP1/PF-RF08-01_creditos.png`; `GP1/PF-RF44-01_creditos_tras_final.png` | | Textos exactos, sin enlaces |
| PF-RF09-01 | RF-09 | DAT | EXE | P | S-PER paso 20 | | Reescribe el perfil (mismo SHA-256) y cierra en < 2 s |
| PF-RF09-02 | RF-09 | BOT | EXE | P | `S-PER/PF-RF09-02_*.png` | DEF-SPER-01 | El defecto es de aspecto del botón |
| PF-RF09-03 | RF-09 | DAT | EXE | P | `S-PER/PF-RF09-03_salir_sin_perfil.png` | | OBS-SPER-2 |
| PF-RF09-04 | RF-09 | DAT | EXE | P | `S-DOC/PF-RF09-04_*.png` | (DEF-SPER-01) | Avisa y muestra la ruta entera sin solaparse |
| PF-RF10-01 | RF-10 | RETO | EXE | P | `GP1/PF-RF10-01_hallazgo_L18.png` | | |
| PF-RF10-02 | RF-10 | RETO | EXE | P | `GP1/PF-RF10-02_objetivo_bosque.png` | | |
| PF-RF10-03 | RF-10 | RETO | EXE | P | `GP1/PF-RF10-03_*.png` | | |
| PF-RF11-02 | RF-11 | RETO | EXE | P parcial (rev. W5; antes P) | GP1 t2 (fotos a +0,4–0,7 s) | | «< 1 s» medido solo en las acciones con foto a hora conocida; la mayoría no la tiene |
| PF-RF11-03 | RF-11 | RETO | EXE | P parcial (rev. W5; antes P) | GP1 t3 pasos 17 y 25 | | Igual que PF-RF11-02: los «Recoger», la zona y la base vacía sin hora medida; el 0,6 s sale del diseño |
| PF-RF12-01 | RF-12 | RETO | EXE | P | `GP1/PF-RF12-01_iterar.png` | | |
| PF-RF12-02 | RF-12 | RETO | EXE | P | `GP1/PF-RF12-02_abstraer.png`, `_pensar_como_un_algoritmo.png` | | |
| PF-RF12-03 | RF-12 | RETO | EXE | P | `GP1/PF-RF44-01_e33_lectura_rapida_sin_saltos.png` | | |
| PF-RF13-01 | RF-13 | BOT | EXE | P | GP1 t1 pasos 23, 27 y 31 | | |
| PF-RF13-02 | RF-13 | BOT | EXE | P | `GP1/PF-RF13-02_pista_bosque.png` | | |
| PF-RF13-03 | RF-13 | BOT | EXE | P | `GP1/PF-RF13-03_pista_taller.png` | | |
| PF-RF13-04 | RF-13 | BOT | EXE | P | GP1 t2 pasos 48–49 | | |
| PF-RF13-05 | RF-13 | BOT | EXE | P parcial | `GP1/PF-RF13-05_ayuda_orilla.png` | | Ayuda vista; la pista de tres rechazos seguidos no se dio |
| PF-RF14-01 | RF-14 | RETO | EXE | P | `GP1/PF-RF14-01_*.png`; `GP2/PF-RF14-01_cueva_inicio.png` | | Piedras sobre las hojas desde el primer cuadro (+100 ms) |
| PF-RF15-01 | RF-15 | RETO | EXE | P | GP1 t1 paso 28 | | OBS-4 (colores del asa) |
| PF-RF16-01 | RF-16 | RETO | EXE | P parcial (rev. W5; antes P) | `GP1/PF-RF16-01_rayo_efectivo1.png`, `_rayo_fuerza_de_mas.png` | | «Dura más con cada golpe» no se midió: fotos sueltas, sin ráfaga del rayo. OBS-2 (un rayo casi horizontal cruza el sílex) |
| PF-RF17-01 | RF-17 | RETO | EXE | P | `GP1/PF-RF45-01_resumen.png`; GP1 t1 pasos 29–39 | | Sin cifras ni juicios |
| PF-RF18-01 | RF-18 | RETO | EXE | P | GP1 t1 pasos 29–33 | | |
| PF-RF19-01 | RF-19 | BOT | EXE | P | `GP1/PF-RF19-01_*.png`; `GP2/PF-RF19-01_soplar_habilitado_hilo_de_humo.png` | | |
| PF-RF19-02 | RF-19 | BOT | EXE | P | GP1 t1 paso 32 | | |
| PF-RF20-01 | RF-20 | RETO | EXE | P | `GP1/PF-RF20-01_humo_corona_mandos_quietos.png`; `GP2/PF-RF20-01_soplar_llama.png` | | |
| PF-RF20-02 | RF-20 | RETO | EXE | NE (rev. W5; antes P por consolidación) | GP1 t1 paso 40; `oe4/acciones.tsv` 14:16:20,622Z | | El doble clic fue a 400 ms: el segundo llegó con «Soplar» ya deshabilitado y no ejercitó la carrera de < 150 ms que pide el caso |
| **PF-RF20-03** | RF-20, RNF-21 | BOT | EXE | **F** (rev. W5; antes P por consolidación) | GP1 t1 pasos 40–42; `GP1/PF-RF16-01_rayo_fuerza_de_mas.png` frente a `GP1/PF-RF20-01_humo_corona_mandos_quietos.png` | **DEF-W5R-02** | Botones atenuados sin candado y nada responde, pero asas y riel de los deslizantes conservan su color (230,0,28 → 230,0,28) |
| PF-RF22-01 | RF-22 | RETO | EXE | P | `GP1/PF-RF22-01_bosque_inicio.png` | | OBS-8 (tablilla llena) |
| PF-RF23-01 | RF-23 | RETO | EXE | P | `GP1/PF-RF23-01_*.png` | | |
| PF-RF24-01 | RF-24 | RETO | EXE | P | GP1 t2 pasos 7, 15–16 | | |
| PF-RF25-01 | RF-25 | BOT | EXE | P | `GP1/PF-RF25-01_*.png` | | |
| PF-RF26-01 | RF-26 | RETO | EXE | P | `GP1/PF-RF26-01_*.png` | | OBS-9 (x de arranque de la 2.2 inferida) |
| PF-RF27-01 | RF-27 | RETO | EXE | P | `GP1/PF-RF27-01_taller_siete_piezas.png` | | |
| PF-RF28-01 | RF-28 | BOT | EXE | P | `GP1/PF-RF28-01_*.png` | | |
| PF-RF29-01 | RF-29 | RETO | EXE | P | `GP1/PF-RF29-01_*.png` | | Carretilla amarrada (e5) comprobada |
| PF-RF30-01 | RF-30 | RETO | EXE | P | `GP1/PF-RF30-01_*.png`; `GP2/PF-RF30-01_laberinto_tablero.png` | | |
| PF-RF31-01 | RF-31 | BOT | EXE | P | `GP1/PF-RF31-01_cajon_bloques.png` | | OBS-10 («Retroceder» lleva un círculo) |
| **PF-RF32-01** | RF-32 | RETO | EXE | **F** | `GP1/DEF-GP1-02_*.png`; `GP2/DEF-GP1-02_reproducido_1s.png` | **DEF-GP1-02** | Reproducido en GP2 |
| PF-RF33-01 | RF-33 | RETO | EXE | P | GP1 t2 paso 47 | | |
| **PF-RF34-01** | RF-34 | BOT | EXE | **F** (rev. W5; antes P) | `GP1/PF-RF34-01_lista_desplazada_con_botones.png` | **DEF-GP1-03** | «Se retira con la papelera» solo se cumplió con el rodeo de abrir el cajón; `plan.md` §3.2 no admite un P con un DEF abierto contra el caso |
| **PF-RF34-02** | RF-34 | BOT | EXE | **F** | `GP1/DEF-GP1-01_*.png` | **DEF-GP1-01** | Riesgo ⚠ de `casos.md`, confirmado |
| PF-RF35-01 | RF-35 | BOT | EXE | P | `GP1/PF-RF35-01_borde_izquierdo_recoger.png` | | |
| PF-RF36-01 | RF-36 | RETO | EXE | P | `GP1/PF-RF36-01_*.png` | | OBS-14 (la 4.ª tarea se marca bajo el fundido) |
| PF-RF37-01 | RF-37 | BOT | EXE | P | `GP1/PF-RF37-01_*.png` | | |
| PF-RF38-01 | RF-38 | RETO | EXE | P | `GP1/PF-RF36-01_ocho_materiales_dos_tareas.png` | | |
| PF-RF39-01 | RF-39 | RETO | EXE | P | `GP1/PF-RF39-01_*.png` | | |
| PF-RF40-01 | RF-40 | RETO | EXE | P | `GP1/PF-RF40-01_*.png` | | OBS-11 |
| PF-RF40-02 | RF-40 | BOT | EXE | NE | — | | La tarea de GP1 armó el amarre bien a la primera; queda para el OE4 |
| PF-RF41-01 | RF-41 | RETO | EXE | P | `GP1/PF-RF41-01_*.png`; `GP2/PF-RF41-01_*.png` | | Los martillazos no se oyen con el arnés (Hoja-HUM) |
| PF-RF42-01 | RF-42 | RETO | EXE | P parcial | `GP1/PF-RF42-01_*.png` | | Primer fallo con la balsa terminada; el segundo fallo (la 3.2 no se repite) no se probó |
| PF-RF42-02 | RF-42, RF-40, RF-43, RF-45 | BOT | EXE | P parcial | `GP1/PF-RF42-02_*.png` | | Prueba anticipada en la base; falta la del amarre. Sonido del hundimiento: H7 |
| PF-RF43-01 | RF-43 | DAT | EXE | P | `GP1/PF-RF43-01_*.png` | | |
| PF-RF44-01 | RF-44 | NAV | EXE | P | `GP1/PF-RF44-01_*.png`; `GP2/PF-RF44-01_*.png` | | Lectura rápida de la 3.3 sin saltos; OBS-13 |
| PF-RF45-01 | RF-45 | DAT | EXE | P | `GP1/PF-RF45-01_resumen.png`; `GP2/PF-RF45-01_resumen_N1_sin_fallos.png` | | Las dos variantes (con y sin fallos), sin dígitos; OBS-6 |
| PF-RF45-02 | RF-45 | DAT | EXE | P | `GP1/PF-RF45-02_resumen_N2.png`; `GP2/PF-RF45-02_resumen_N2_sin_fallos.png` | | |
| PF-RF45-03 | RF-45 | NAV | EXE | P | `GP1/PF-RF45-03_*.png`; `GP2/PF-RF45-03_*.png` | | Los dos botones llevan a la escena final (INC-39); OBS-15 |
| PF-RF45-04 | RF-45 | DAT | EXE | P (consolidación) | S-PER pasos 41–43; `S-PER/S-PER_OE4_B_final.json` | | Rejugar el N2 no reescribe sus indicadores |
| PF-RF46-01 | RF-46 | DAT | EXE | P | `S-DOC/PF-RF46-01_*.png` | | Solo lectura: ningún `LastWriteTime` cambia; OBS-SDOC-1/2 |
| PF-RF46-02 | RF-46, RF-45 | DAT | EXE | P | `S-DOC/PF-RF46-02_*.png` | | Adaptado: JSON reales de GP1 y S-PER en lugar de los de S-N1/S-N3, que no existen |
| PF-RF46-03 | RF-46 | DAT | EXE | P | `S-DOC/PF-RF46-03_sin_perfiles.png` | | |
| PF-RF47-01 | RF-47 | DAT | EXE | P | `S-DOC/PF-RF47-01_*.png` | | |
| PF-RF47-02 | RF-47 | DAT | EXE | P | `S-DOC/PF-RF47-02_*.png` | | |
| PF-RF47-03 | RF-47 | DAT | EXE | P | `S-DOC/PF-RF47-03_*.png` | | Ruta de respaldo (INC-34) |
| PF-RNF02-01 | RNF-02 | BOT | EXE | P parcial | GP1 t1 paso 22, t2 paso 8, t3 paso 8 | | Cueva, bosque y orilla; faltan taller y laberinto, y la parte de SUITE |
| PF-RNF04-01 | RNF-04 | RNF | EXE | P parcial | apartado 4.1; `S-RNF/S-RNF_cargas.csv` | | Equipo 1 completo; falta el equipo 2 (HUM) |
| PF-RNF05-01 | RNF-05 | RNF | EXE | P parcial | apartado 4.2; `*/*_mem.csv` | | Equipo 1 completo; falta el equipo 2 (HUM) |
| PF-RNF06-01 | RNF-06 | RNF | EXE + INSP | P | `S-RNF/S-RNF_tamano.txt` | | 479,0 MB |
| PF-RNF07-01 | RNF-07 | RNF | EXE | P | `S-DOC/PF-RNF07-01_*.png`; `S-DOC/S-RNF07_portabilidad.txt` | | RNF-07 exige también PF-RNF07-02 |
| PF-RNF09-01 | RNF-09 | DAT | INSP | P | `S-DOC/S-DOC_PF-RNF09-01_inspeccion_json.txt` | | 15 perfiles de las evidencias, más `CON` y `OE4R` |
| **PF-RNF10-01** | RNF-10 | RNF | EXE + INSP | **F** | apartado 4.4; `*/*_red.csv`; `S-SPIKE/residuos.txt` | **DEF-SPIKE-01** | |
| PF-RNF11-01 | RNF-11 | DAT | EXE | P | `S-DOC/S-DOC_residuos_rastreo_tras_borrado.txt` | (DEF-SPIKE-01) | Cero rastros del perfil; los residuos de analítica van con el DEF |
| PF-RNF13-01 | RNF-13 | NAV | EXE | P parcial | apartado 3; `GP1/GP1_Player.txt`; `GP2/GP2_Player.txt` | | Los dos recorridos del arnés pasan; falta el de Santiago con cronómetro (H8) |
| PF-RNF14-01 | RNF-14 | DAT | EXE | P | `S-PER/PF-RNF14-01_*.png` | | |
| PF-RNF14-02 | RNF-14 | DAT | EXE | P | `S-PER/PF-RNF14-02_*.png` | | |
| PF-RNF14-03 | RNF-14, RF-04 | DAT | EXE | P | `S-PER/PF-RNF14-03_*.png` | | |
| PF-RNF14-04 | RNF-14, RF-03 | DAT | EXE | P | `S-PER/PF-RNF14-04_*.png` | | Con `OE4_B` jugado en vez de `OE4_B3` |
| PF-RNF14-05 | RNF-14 | DAT | EXE | P | `S-PER/PF-RNF14-05_*.png` | | Con `OE4_B` y la 2.1 jugada en vez de `OE4_B2` |
| PF-RNF16-01 | RNF-16, CT-04 | RNF | SUITE | P con reserva | apartado 4.7; `RNF16/README.md`, `RNF16/*.xml` | | Ejecutado tras la primera versión de este documento; la reserva, en 4.7 |
| PF-RNF19-01 | RNF-19 | RNF | INSP | P parcial | `GP1/PF-RNF19-01_lista_N3_gris.png` | | Solo la lista del N3 pasada a grises; el resto juzgado por forma sobre las fotos en color |
| PF-RNF21-01 | RNF-21 | RNF | INSP | P parcial | `GP1/PF-RNF21-01_rafaga_llama.csv` | | Solo la llama, ya sobre la narrativa (OBS-3). La ignición, la subida de la luz en 3,5 s, no se grabó (rev. W5), y 10 fps solo resuelve hasta 5 Hz. Falta la parte de SUITE |

**Cierres forzados (RNF-14).** S-PER hizo siete `Matar`:
- los cinco casos PF-RNF14;
- un cierre a mitad de la mecánica del N1 (casilla S1);
- un cierre tras confirmar el taller (casilla S2).

En los siete, el perfil reabrió sin error y el juego retomó en la primera fase pendiente, con lo confirmado y sus
indicadores intactos. Lo no confirmado se rehízo desde el inicio de su fase.

## 6. Defectos

La severidad es la de `plan.md` §6. Los marcados «propuesta» los confirma Santiago en el triaje (T21). Durante las
sesiones no se corrigió ningún archivo del juego. El detalle de cada defecto (reproducción, causa probable y
evidencia) está en el registro de su sesión. Los dos que halló la revisión de W5 están en su informe y en `W5-REV/`.
La última columna remite a la segunda pasada (§8.3).

| DEF | Caso | Severidad | Tipo propuesto | Resumen | Tras rc2 |
|---|---|---|---|---|---|
| **DEF-SPIKE-01** | PF-RNF10-01 (y RNF-08, RNF-11) | Mayor (propuesta) | Configuración del proyecto y del build | **Qué pasa:** el reproductor rc1 abre conexiones TCP a Google Cloud :443 en cada lanzamiento y deja datos de Unity Analytics/Insights en LocalLow y valores `unity_connect.*` en HKCU. **La revisión de W5 lo refuerza:** los `Analytics\ArchivedEvents\*` desaparecen entre sesiones, es decir, los eventos se enviaron; viajan `unity.cloud_userid` e `installation_id`, un identificador persistente del equipo; y la 4.ª IP solo sale en el lanzamiento con la `Exception`. Así no se puede entregar a un colegio (RNF-10, RNF-12). **Arreglo recomendado:** desactivar Analytics y Connect, comprobar que el build no vuelva a poner `m_Enabled: 1`, valorar quitar del player `Unity.AI.*` (`com.unity.ai.assistant`) y recompilar. Registro: `S-SPIKE/S-SPIKE_registro_de_pasos.md` | Corregido. Red y LocalLow: **P**. Queda un resto en HKCU: DEF-RC2-01 |
| **DEF-W5R-01** | PF-RF02-06 (RF-02, RNF-03) | Mayor | Código y escena (`ProfileSelectController`, `MainMenu.unity`) | «¿Quién juega?» dibuja todas las filas en un `VerticalLayoutGroup` de 564 × 430 px, sin máscara ni desplazamiento. Con 8 perfiles, la 5.ª fila queda fuera del panel, la papelera de la 6.ª bajo «Volver», la 7.ª cortada y la 8.ª no se ve; ni el arrastre ni la rueda mueven nada. En un aula, del 7.º estudiante en adelante nadie puede abrir ni borrar su perfil desde el panel. S-DOC lo había dejado como observación (OBS-SDOC-3). Evidencia: `W5-REV/` | Corregido: **P** |
| **DEF-GP1-01** | PF-RF34-02 | Mayor (propuesta; la sesión la anotó «media», que no está en la escala) | Código (`MazeSceneController`) | Un bloque tomado de «Tu secuencia» se queda pegado al cursor: `RefreshRows` destruye la fila que recibió el pulsar y el soltar no llega. El siguiente arrastre engancha ese bloque en lugar del pedido, y reordenar exige dos gestos. **Además infla el indicador «errores corregidos»** que ve el docente: de las 40 ediciones de GP1 en N2F3, 3 las fabricó el defecto (RF-45, RF-46). Registro: `GP1/GP1_tramo2_registro_de_pasos.md` | Corregido: **P** |
| **DEF-GP1-02** | PF-RF32-01 | Mayor (propuesta; anotada «media») | Código | Con la lista desplazada, el resaltado del bloque en curso no se ve: la lista no se desplaza hasta él, y los bloques 1 y 2 se ejecutaron fuera de la vista. Reproducido en GP2 | Corregido: **P** |
| DEF-GP1-03 | PF-RF34-01 (F, rev. W5) | Menor | Código | Tras usar la papelera con el cajón cerrado, ninguna fila ofrece papelera (`_expanded = -1`). El rodeo es abrir el cajón | Corregido: **P** |
| DEF-SPER-01 | PF-RF09-02, PF-RF09-04 | Menor | Código (prefab del diálogo) | El botón «Cerrar el juego» lleva el icono de la papelera y su rótulo, partido en dos líneas, se sale del botón | Corregido: **P** |
| DEF-SPER-02 | PF-RF02-04 | Menor (la revisión: impacto subestimado) | Código (`SaveStore`, `DiskFileSystem`, `ProfileSelectController.SelectExisting`) | Elegir un perfil ilegible lanza una `ArgumentException` sin capturar y no muestra ningún aviso. El juego sigue operable. **Además** (revisión de W5, lectura de código): el perfil se escribe con `File.WriteAllText` directo, así que un cierre o un corte de luz durante el guardado puede dejar el JSON truncado, que es justo este caso, con todo el progreso del estudiante inaccesible (RNF-14, invariante 3). Los siete `Matar` de S-PER no cayeron en esa ventana | Corregido el aviso y el guardado atómico: **P** |
| **DEF-W5R-02** | PF-RF20-03 (F, rev. W5) | Menor | Código (`FirePanelController`) | Durante el encendido los deslizantes del N1 no se ven atenuados, aunque no responden: asas y riel conservan su color antes y 1,8 s después de soplar; solo los botones pasan a gris | Corregido: **P**, con N6 (§8.9) |
| Candidato · OBS-SDOC-2 | PF-RF46-01 | Menor (propuesta) | Código (`TeacherReportController`) | El informe no dice qué perfil se está mirando: el docente puede atribuir las cifras a otro estudiante (RF-46, «para el perfil que se seleccione») | Corregido: **P** |

**Efecto sobre el segundo KPI (plan §1):** con rc1, RF-02 (DEF-W5R-01), RF-32 (DEF-GP1-02) y RF-34 (DEF-GP1-01), los
tres de prioridad Alta, tenían un Mayor abierto y contaban como no implementados. Los tres se corrigieron y se
reprobaron sobre rc2 (§8).

**Riesgo sin clasificar (revisión de W5).** En el spike, sin mover el ratón, se perdieron 12 clics en «Continuar» al
pasar de una narrativa a otra (cada escena trae su propio `EventSystem`). Puede pasarle a un niño con *touchpad* que
toca sin deslizar, y el «Nudge» del arnés lo enmascara en todas las sesiones. Va a la Hoja-HUM: tocar «Continuar» sin
mover el puntero a través de un cambio de escena (guion H13, paso 1).

**Defectos del arnés** (herramienta, no juego), ya corregidos en `oe4.ps1`: los cuatro del spike y `Foto -Gris` (GP1 tramo 3).

## 7. Recuento de rc1 y lo que quedaba para el OE4

Recuento de la primera pasada con las correcciones de la revisión de W5. El vigente, sobre rc2, está en §8.8.

| Veredicto | Casos |
|---|---|
| P | 77: 2 por consolidación (PF-RF02-02, PF-RF45-04), 1 con la precondición adaptada (PF-RF46-02) y 1 con reserva (PF-RNF16-01) |
| P parcial | 16: PF-RF02-01, PF-RF06-02, PF-RF07-01, PF-RF07-02, PF-RF11-02, PF-RF11-03, PF-RF13-05, PF-RF16-01, PF-RF42-01, PF-RF42-02, PF-RNF02-01, PF-RNF04-01, PF-RNF05-01, PF-RNF13-01, PF-RNF19-01, PF-RNF21-01 |
| F | 6: PF-RF02-06, PF-RF20-03, PF-RF32-01, PF-RF34-01, PF-RF34-02, PF-RNF10-01 |
| NE | 2: PF-RF20-02, PF-RF40-02 |
| B / NA | 0 |
| **Total** | **101** (100 del catálogo y PF-RF02-06, el caso nuevo de la revisión) |

**Qué cambió respecto de la primera versión** (99 casos: 83 P, 12 P parcial, 3 F y 1 NE):
- **siete reclasificaciones** de la revisión: PF-RF20-03 y PF-RF34-01 pasan a F, PF-RF20-02 a NE, y PF-RF06-02,
  PF-RF11-02, PF-RF11-03 y PF-RF16-01 a P parcial;
- **dos casos más**: PF-RNF16-01, P con reserva (§4.7), y PF-RF02-06, F (DEF-W5R-01).

**Cómo se cuenta.** «P parcial» no es un veredicto de la escala de `plan.md` §3.2: son casos a medio ejecutar y quedan
**fuera del cómputo**, ni en el numerador ni en el denominador, porque contarlos como aprobados inflaría la eficacia.
NE tampoco cuenta. Eficacia provisional de rc1, `(P + PD) / (P + PD + F)` con PD = 0:
**77 / 83 = 92,8 %** (77 / 82 = 93,9 % sin PF-RF02-06). No es el KPI: el KPI (T24) se calcula sobre el catálogo
completo, con la versión final. PF-RNF07-01 es P, pero RNF-07 sigue abierto hasta PF-RNF07-02 (equipo 2).

**Sin ejecutar sobre rc1** (26 casos del catálogo; PF-RF06-03 y PF-RF45-05 se ejecutaron después, sobre rc2, §8.4):

| Grupo | Casos |
|---|---|
| S-INI | PF-RF01-02, los rechazos de PF-RF02-01, PF-RF02-03 y PF-RF03-01 |
| Casos de N1 a N3 sin veredicto propio en las sesiones | PF-RF11-01, PF-RF21-01, PF-RF07-04 |
| S-PAU | PF-RF07-01..04 en las cinco escenas |
| S-OMI | PF-RF06-03, PF-RF06-04, PF-RF21-02, PF-RF45-05 |
| S-INSP | PF-RNF01-01, PF-RNF03-01, PF-RNF15-01, PF-RNF17-01, PF-RNF18-01, PF-RNF20-01, PF-RNF22-01, PF-RNF23-01 y PF-RNF12-01 |
| S-HUM | H3–H12: PF-RNF07-02, PF-RNF08-01, PF-CT02-01, PF-SON-01..03 y el recorrido cronometrado de PF-RNF13-01 |

**Observaciones** (no son defectos): están en el registro de cada sesión y van al triaje y al carril de arte e interfaz.
- GP1: OBS-1 a OBS-16.
- GP2: OBS-GP2-1, la 3.3 da por hecho un fallo en el camino sin errores.
- S-PER: OBS-SPER-1 a OBS-SPER-6.
- S-DOC: OBS-SDOC-1 a OBS-SDOC-5.

Para Santiago había cinco puntos de decisión. Dos se resolvieron en rc2 y uno quedó absorbido por un defecto
(Claude decide lo recomendado, regla de Santiago del 01/10):
- OBS-2, el abanico del rayo: **sigue abierto**;
- OBS-13, la fogata tras la Niña: **resuelta en rc2** (§8.3);
- OBS-GP2-1, una variante de la 3.3: **sigue abierto**;
- OBS-SDOC-2, el nombre del perfil en el informe: **resuelta en rc2** (§8.3);
- OBS-SDOC-3, la lista de perfiles con un curso entero: en el panel de perfiles pasó a DEF-W5R-01 y se resolvió con
  páginas de tres; en el informe docente **sigue abierta** (OBS-rc2-2, §8.9).

**Ajustes al guion que piden las sesiones** (se aplican a `casos.md` antes de la próxima pasada, nunca durante):
- S-N1 paso 21: la ráfaga va sobre la narrativa (OBS-3).
- S-N3 paso 20: la 4.ª tarea se marca al salir al cruce (OBS-14).
- S-N2C: «Retroceder» lleva un círculo, no un rombo (OBS-10).
- S-DOC: la precondición, porque no existen los JSON de S-N1, S-N2 y S-N3.
- PF-RNF07-01: la ruta del escritorio, porque `%USERPROFILE%\Desktop` puede estar redirigido.

## 8. Segunda pasada sobre rc2 (01/10/2026)

La revisión de W5 recomendó un rc2, y se hizo así:
1. Se corrigieron los defectos de §6 con `fix-bug`: cada corrección lleva una prueba nueva que nombra su requisito y se
   midió de rojo a verde.
2. El código pasó **tres revisiones**:
   - la primera dio FAIL con tres bloqueantes: B1, el guardado atómico lanzaba si otro proceso (un antivirus) retenía el
     perfil; B2, con cuatro perfiles por página la 4.ª fila quedaba cortada; B3, una prueba vieja exigía el
     comportamiento de DEF-GP1-03;
   - la segunda dio FAIL con uno: B1-bis, con el temporal retenido el respaldo guardaba y aun así lanzaba;
   - la tercera dio PASS, con la SUITE completa en verde.
3. Se construyó rc2.
4. Se reverificó sobre el ejecutable lo que pidió la revisión de W5 («Qué repetir»): la SUITE completa, S-N2C entera,
   S-INI con 8 perfiles, el bloque D de S-PER, los pasos 17–22 de S-N1, red y residuos, y un *Golden Path* a pantalla
   completa (GP3). Es la regresión de `plan.md` §6.

Durante las sesiones no se tocó código, escenas, `Packages/manifest.json` ni commits.

### 8.1 Versión que se prueba (procedencia en `evidencias/build-rc2.md`)

| Dato | Valor |
|---|---|
| Candidato | rc2: `Build/Algoritmia/`, 12 escenas (`Boot` primera), con `Licencias/` (OFL y Phosphor) y sin `Datos/`. rc1 se conserva sin tocar en `Build/Algoritmia_rc1` |
| Rama y HEAD | `feat/cierre-de-slices-y-oe3` sobre `359365e0a351bd2b8e64bbd0bb75b1c8cef9b2f5`, sin commits; 262 entradas en `git status`, antes y después del build |
| Huella del árbol, solo `Assets Packages ProjectSettings` | `f5bb1efe…1fd726`, igual antes (18:03) y después (18:06, tras restaurar `ProjectSettings.asset`). Archivos sin seguimiento de esas carpetas: `14f713bb…355cef` |
| Build | 01/10/2026 18:03:59–18:04:43 (43,9 s); API del pipeline con el Editor abierto; buildId `build_818982e2cf22`, Succeeded, 0 errores, 486 avisos; Unity 6000.5.10f1, StandaloneWindows64, Mono, sin development build |
| SHA-256 de `Algoritmia.exe` | `6cd16edf…adab`, igual que rc1, porque es solo el lanzador |
| **Huella del contenido** | `Algoritmia_Data` (223 archivos, método de `build-rc2.md`): `a613b851cd05d21d9daded0ee8c93c1f483fed390941b69d71cd6c29c887a704` (rc1: `c3787fd2…368d0d`). Carpeta entregable (252 archivos sin `Datos/` ni `*_DoNotShip`, método de la revisión de W5, `build-rc1.md`): `024076a1fd0f576ac35eb777fd32c8c61eb242a17c65898a11e0e7118a3057e0` (rc1: `af4278fb…bb28e`). La primera se comprobó a las 18:11 y a las 20:35: las sesiones no tocaron el paquete |
| Tamaño sin `Datos/` ni `*_DoNotShip` | 478 987 619 B = 456,8 MiB = **479,0 MB** (rc1: 478 979 915 B) |
| Servicios de Unity | `UnityServicesOff` (`Game.EditorTools`) apaga antes del build los interruptores que el Editor tenga encendidos en memoria. Tras el build, lo hace fallar si `globalgamemanagers` nombra `unity3d.com`. En rc2 hay **0** apariciones (en rc1, seis URL). `UnityConnectSettings.asset` sigue con `m_Enabled: 0` y su huella no cambió |
| Fuera del entregable | `Algoritmia_BurstDebugInformation_DoNotShip` (784 239 B), que este build sí generó: se retira al empaquetar |
| Sin cambios, por la decisión D3 | `Packages/manifest.json` intacto. Siguen `DirectML.dll`, `D3D12/` y `dstorage*.dll`, y en `Managed/` siguen `Unity.AI.MCP.Runtime.dll` y `Unity.AI.Tracing.dll`; en las siete ejecuciones no se observó ninguna conexión de red (§8.6) |
| SUITE antes del build (tercera revisión, Editor recién abierto) | PlayMode **376/376** y EditMode **470 = 469 + 1 omitida** (`ProfileEraser_INC34`, por entorno), 0 fallos: `suites/rc2-final/`. La PlayMode duró **1743,5 s según el runner** (`duration` del XML, de 22:31:47Z a 23:00:51Z); los 1755,5 s del resumen JSON son el reloj de pared de `editor.ps1`, desde la petición (22:31:41Z) hasta el último sondeo (23:00:57Z), con la apertura de `Boot` y la recarga de dominio |
| Equipo y condiciones | Equipo 1 (§1), con un solo monitor de 1920 × 1080 al 125 %, como en rc1: el de 2560 × 1440 que vio la revisión de W5 ya no estaba al empezar. El Editor de Unity estuvo cerrado en todas las sesiones; Rider siguió abierto, y el arnés no lo cuenta. **El sonido del juego se silenció** en el mezclador de Windows por una reunión de Webex de Santiago, así que esta pasada no juzga nada que se oiga. La ventana de Webex va tapada en gris en las fotos; la máscara tapa media burbuja del botón de ayuda del N3 y ningún veredicto depende de esa zona |
| Arnés | `herramientas/oe4.ps1` sin cambios, más herramientas nuevas que no son juego (`rc2/herramientas/`): `combo.ps1` (doble clic a < 150 ms y ráfagas en el mismo proceso), `matar-al-guardar.ps1` (cierre forzado en el instante del guardado), `destellos.py`, `rayo.py`, `tablero.py`, `tapar.py`, `idle.ps1` y `silencio.ps1` |

### 8.2 Sesiones sobre rc2

| Sesión | Horas (Bogotá) | Lanzamientos | Qué cubrió | Registro de pasos (`rc2/`) |
|---|---|---|---|---|
| Preparación | 18:09–19:41 | 0 | Tamaño y huella del paquete; línea base de coordenadas; espera hasta que el equipo quedó libre | `00-preparacion_registro_de_pasos.md` |
| S-INI-rc2 | 19:41–19:56 | 1 | 8 perfiles semilla; S-N1 pasos 17–22 con `OE4_Z`; doble «Omitir»; informe docente; «Salir»; red y residuos | `S-INI-rc2_registro_de_pasos.md` |
| S-N2C-rc2 | 19:56–20:08 | 1 | S-N2C entera con `OE4_B3` | `S-N2C-rc2_registro_de_pasos.md` |
| S-PER-rc2 | 20:09–20:17 | 4 | Bloque D (perfil ilegible, `CON`) y dos cierres forzados en el instante del guardado | `S-PER-rc2_registro_de_pasos.md` |
| GP3 | 20:17:53–20:34:01 | 1 | PF-RNF13-01 por tercera vez: perfil nuevo `OE4GP3`, pantalla completa | `GP3_registro_de_pasos.md` |
| S-RNF-rc2 | 20:35–20:40 | 0 | Tamaño, huella, registros, cargas, memoria, red y residuos de las siete ejecuciones | `S-RNF-rc2_registro_de_pasos.md` |

### 8.3 Defectos de rc1: corrección y reverificación

Rutas de evidencia relativas a `evidencias/rc2/`.

| DEF | Corrección en rc2 | Pruebas nuevas (rojo → verde) | Reverificación sobre el exe | Veredicto |
|---|---|---|---|---|
| **DEF-SPIKE-01** | **Causa medida:** el build serializa los ajustes **en memoria** del Editor, no el archivo, y el interruptor general de los servicios estaba en `True` aunque el archivo dijera 0; `m_EngineDiagnosticsEnabled` (Insights) estaba en 1 también en disco. **Arreglo:** `UnityServicesOff` (antes del build apaga 9 interruptores; después verifica `globalgamemanagers`); `UnityConnectSettings.asset` con `m_EngineDiagnosticsEnabled: 0` | 10 `UnityServicesOff_RNF10_*`; `Architecture_RNF10_ElEjecutableNoEnviaAnaliticaNiEstadisticasDeHardware` ampliada | **Ninguna conexión observada**, TCP ni UDP, ni de escucha, en 1312 muestras de 2 s de los 7 lanzamientos (unos 44 min de juego); el arranque de cada lanzamiento queda sin muestrear (§8.6). En LocalLow solo aparece `Player.log`. En HKCU ya no está `unity.cloud_userid`, pero siguen tres `unity_connect.*` → DEF-RC2-01. Evidencia: `*_red.csv`, `*_residuos.txt` | **P** (PF-RNF10-01). Corregido en la red y en LocalLow; el resto del registro sigue abierto como DEF-RC2-01 |
| **DEF-W5R-01** | Páginas de tres perfiles con ▲/▼ (`ProfilePaging`, C# plano, y dos botones en `MainMenu.unity`), sin máscara ni `ScrollRect`. Con cuatro por página la 4.ª fila salía cortada (B2): la fila mide 100 px y no los 88 que declara su `LayoutElement` | `ProfilePagingTests` (8, que contaban páginas de cuatro; tras el build pasaron a tres por página y 9 casos, decisión D-PAG, §8.10); `ProfileSelect_RF02_ConOchoPerfilesTodosSeAlcanzanConLasFlechas`; `ProfileSelect_RF02_BorrarElUnicoPerfilDeLaUltimaPaginaVuelveALaAnterior` | 3 + 3 + 2 filas enteras, ninguna papelera bajo «Volver». La 8.ª (`OE4_Z`) se elige y abre su progreso, y su papelera responde. El arrastre y la rueda no mueven la lista. Evidencia: `PF-RF02-06_pagina{1,2,3}_de_3.png`, `PF-RF02-02_OE4_Z_progreso.png`, `PF-RF47-01_papelera_octava_fila.png` | **P** (PF-RF02-06) |
| **DEF-GP1-01** | La fila que recibió el pulsar se **aparta** (sigue activa, con alfa 0) en vez de destruirse, así que el soltar llega. `CargoHandle.ReleasedAt(Vector2)` toma la posición del evento. Las ediciones se cuentan al soltar, y un clic sin mover vale 0 | `MazeScene_RF34_ReordenarUnBloqueEsUnSoloGestoYNoQuedaPegado`; `…_RetirarYReordenarArrastrandoCuentanSoloLasEdicionesHechas`; `…_UnClicSinMoverSobreUnBloqueLoDejaDondeEstabaYNoCuentaEdicion` | Retirar soltando fuera y reordenar se hacen en **un solo gesto**, y nada sigue al cursor. El JSON da C = 40, sin ediciones fabricadas por el defecto. Evidencia: `PF-RF34-02_*.png`, `S-N2C-rc2_OE4_B3_final.json` | **P** (PF-RF34-02) |
| **DEF-GP1-02** | `Highlight` termina en `RevealRow`, que desplaza la lista lo mínimo para que el bloque en curso se vea, con 8 px de margen y los mismos ▲/▼ (`SequenceListRules.RevealScroll`, C# plano), sin `ScrollRect`. También vale para el bloque donde se detuvo un fallo | `MazeScene_RF32_AlEjecutarLaListaMuestraElBloqueEnCurso`; `SequenceListRulesTests` (10, con DEF-GP1-03) | Con la lista abajo del todo, a +0,5 s ya subió y el bloque 1 se ve resaltado; el resaltado baja con la lista hasta el último. Visto con 9 bloques (S-N2C) y con 10 (GP3). Evidencia: `PF-RF32-01_ejecucion_2fps.png`, `DEF-GP1-02_bloque_en_curso_visible.png`, `GP3_DEF-GP1-02_bloque_en_curso.png` | **P** (PF-RF32-01) |
| DEF-GP1-03 | Tras retirar un bloque queda seleccionada la fila que ocupa su lugar (`SequenceListRules.SelectionAfterRemoval`); −1 solo si la secuencia queda vacía. Se actualizó `MazeScene_RF34_ElBloqueSeleccionadoMuestraLaPapeleraYEstaLoRetira`, que exigía el defecto (B3) | `MazeScene_RF34_TrasLaPapeleraConElCajonCerradoLasFilasLaOfrecen`; `…_TrasRetirarArrastrandoLasFilasOfrecenLaPapelera` | Con el cajón cerrado, tres papeleras seguidas en el mismo sitio vacían la secuencia sin rodeo. Evidencia: `DEF-GP1-03_papelera_con_cajon_cerrado_x3.png`, `PF-RF34-01_papelera_avanzar.png` | **P** (PF-RF34-01) |
| DEF-SPER-01 | `MainMenu.unity` (`ExitConfirmPanel`): sin icono y con el botón de 240 a 280 px | `MainMenu_RF09_CerrarElJuegoNoLlevaPapeleraYSuRotuloCabeEnUnaLinea` | «Cerrar el juego» va en una línea, dentro del botón y sin papelera; «Quedarme» no escribe nada; cerrar tarda 1,8 s. Evidencia: `DEF-SPER-01_dialogo_salir_rc2.png` | **P** (PF-RF09-02) |
| DEF-SPER-02 | **Aviso:** `SaveStore.TryLoad` y `ProfileSession.TryLoad`; si el perfil no se lee, `SelectExisting` muestra «No se pudo abrir ese perfil. Avisa a tu profe.», sin cifras ni culpa (CP-02, CP-03). **Guardado atómico** en `DiskFileSystem.WriteAllText`: escribe `<perfil>.json.tmp` y lo pone en su sitio con `File.Replace` (o `File.Move` si es nuevo). Si `Replace` falla porque otro proceso retiene el perfil, cae a `File.Copy` (B1) y borra el temporal solo si puede (B1-bis); un `.tmp` huérfano no se lista y el siguiente guardado lo pisa | `ProfileSelect_RNF14_ElegirUnPerfilIlegibleAvisaYNoLanza`; `SaveStore_RNF14_UnJsonTruncadoNoLanzaYSeReportaComoIlegible`; `SaveStore_RNF14_UnPerfilSanoSeCargaPorTryLoad`; `DiskFileSystem_RNF14_*` (4, entre ellas `…ConElPerfilAbiertoPorOtroProcesoElGuardadoNoFallaNiDejaTemporal` y `…ConElTemporalRetenidoPorOtroProcesoElGuardadoNoFalla`) | Perfil truncado de 38 B: aparece el aviso, no hay `Exception` y se puede borrar. **Dos cierres forzados en el instante del guardado** (un vigilante de archivos mata el juego en el primer evento): entre la escritura del temporal y el reemplazo, el perfil anterior queda intacto, con un `.tmp` completo que no se lista, y se retoma la fase sin confirmar; inmediatamente **después** del reemplazo (el vigilante despertó con el renombrado interno de `File.Replace`, pero el cierre llegó con el reemplazo ya hecho), queda el perfil nuevo completo. La ventana interna de `File.Replace` no se alcanzó (riesgo residual R-RC2-A, §8.7). En ningún caso un JSON truncado. Evidencia: `DEF-SPER-02_perfil_ilegible_aviso.png`, `PF-RNF14-guardado_matar{1,2}.txt`, `PF-RNF14-guardado_matar{1,2}_OE4_E.json`, `PF-RNF14-guardado_tras_matar_*.png` | **P** (PF-RF02-04; guardado atómico) |
| DEF-W5R-02 | **Causa medida:** el `targetGraphic` del `Slider` es el contorno del asa, tapado por su hijo `Fondo`, así que el tinte de deshabilitado no se veía. **Arreglo:** `FirePanelController.LockControls` deja cada deslizante con `interactable = false` y un `CanvasGroup` de alfa 0,5 (`restingSliderAlpha`, ajustable) | `FireLevel_RF20_DuranteElEncendidoLosDeslizantesSeVenAtenuados` | Desde +0,14 s tras soplar: asa de fuerza de (230,0,28) a (189,80,72); asa de cercanía de (156,71,64) a (144,94,82); riel, «Lejos» y «Cerca» atenuados; botones grises y sin candado. «Fuerza del golpe», «Fuerte» y «Suave» no se atenúan (N6, cosmético). Evidencia: `PF-RF20-03_mandos_atenuados_al_soplar.png` | **P** (PF-RF20-03) |
| OBS-SDOC-2 | Encabezado «Perfil de {0}» (`ReportContent.SelectedProfileFormat`) en `TeacherReport.unity`, que `Select` rellena al elegir, al cambiar y tras borrar | `TeacherReport_RF46_ElDetalleNombraElPerfilQueSeEstaMirando` | «Perfil de OE4_A» por defecto y «Perfil de OE4_Z» al elegir la 8.ª fila, con exactamente las cifras de la semilla. Evidencia: `PF-RF46-01_informe_perfil_de_OE4_A.png`, `OBS-SDOC-2_informe_perfil_de_OE4_Z.png` | **P** (PF-RF46-01) |
| OBS-13 | `N3_EscenaFinal.asset`: la fogata central (troncos, llama y humo) pasa de x ≈ 0,50 a 0,60, sin tocar la y ni ningún encuadre | `NarrativeSequence_OBS13_NingunaLlamaDeLaEscenaFinalSaleDeTrasLaCabezaDeLaFamilia` | En ninguna de las siete líneas la fogata sale detrás de una cabeza: queda a la derecha del Niño. En L6, con los brazos en alto, la mano del Niño queda cerca de las hojas, y se lee «al lado» (N10). Evidencia: `OBS-13_fogata_junto_al_Nino_L5_L6.png`, `GP3_resumen_N3_y_escena_final.png` | Resuelta. La figura 7.3 del OE3 hay que recapturarla desde rc2 |

**Defectos de rc1 reabiertos: ninguno.**

### 8.4 Casos repetidos sobre rc2

Versión **rc2**, fecha **01/10/2026**. La columna rc1 es el veredicto tras la revisión de W5 (§5). Rutas relativas a
`evidencias/rc2/`.

| Caso | rc1 | rc2 | Evidencia | Nota |
|---|---|---|---|---|
| **PF-RF02-06** | F (DEF-W5R-01) | **P** | `PF-RF02-06_pagina{1,2,3}_de_3.png` | 3 + 3 + 2; la 8.ª se elige con las flechas |
| PF-RF02-02 | P | P | `PF-RF02-02_OE4_Z_progreso.png` | `OE4_Z` abre con su progreso exacto |
| PF-RF02-04 | P (DEF-SPER-02) | **P** | `DEF-SPER-02_perfil_ilegible_aviso.png`; `S-PER-rc2_lanz1_Player.txt` | Aviso sin `Exception`; borrable. N4 |
| PF-RF02-05 | P | P | `PF-RF02-05_CON_*.png` | `CON` crea un perfil que funciona y se borra |
| **PF-RF06-02** | P parcial | **P** | `PF-RF06-02_N1_Apertura_con_omitir.png`, `_cueva_tras_tres_omitir.png` | La cadena del N1: tres «Omitir» hasta la cueva |
| **PF-RF06-03** | sin ejecutar | **P** | `PF-RF45-05_doble_omitir_al_resumen.png` | Al repetir el N1, «Omitir» en el cierre lleva al resumen |
| PF-RF06-01 | P | P | S-N2C-rc2 paso 16; GP3 pasos 2–3 | Sin «Omitir» la primera vez |
| PF-RF09-01 | P | P | S-INI-rc2 paso 28 | Cierra en 1,8 s y reescribe el perfil sin cambiar su contenido |
| PF-RF09-02 | P (DEF-SPER-01) | **P** | `DEF-SPER-01_dialogo_salir_rc2.png` | |
| **PF-RF16-01** | P parcial | **P** | `PF-RF16-01_rayo1_20fps.png`, `_rayo3_20fps.png`, `_rayo_fuerza_de_mas_20fps.png` | Medido a 20 fps: el efectivo dura ≈ 0,15 → 0,25 → 0,55 s; el de fuerza de más sube y se apaga en el aire. OBS-2 sigue |
| PF-RF19-01 | P | P | `PF-RF19-01_soplar_habilitado.png` | |
| **PF-RF20-02** | NE | **P** | `PF-RNF21-01_ignicion_10fps.png`; S-INI-rc2 paso 19 | Doble clic a **85 ms**: una sola ignición, una sola narrativa, sin `Exception` |
| **PF-RF20-03** | F (DEF-W5R-02) | **P** | `PF-RF20-03_mandos_atenuados_al_soplar.png` | N6 |
| PF-RF30-01 | P | P | `PF-RF30-01_tablero_rc2.png`; `S-N2C-rc2_tablero.txt` | |
| PF-RF31-01 | P | P | `PF-RF31-01_cajon.png` | |
| **PF-RF32-01** | F (DEF-GP1-02) | **P** | `PF-RF32-01_*.png`; `GP3_DEF-GP1-02_bloque_en_curso.png` | Secuencia vacía informada sin mover la carretilla; bloque en curso a la vista |
| PF-RF33-01 | P | P | S-N2C-rc2 pasos 6–7 | |
| PF-RF13-04 | P | P | `PF-RF34-01_tres_fallos_y_pista.png` | |
| **PF-RF34-01** | F (DEF-GP1-03) | **P** | `PF-RF34-01_*.png`; `DEF-GP1-03_papelera_con_cajon_cerrado_x3.png` | Sin rodeo, con el cajón abierto o cerrado |
| **PF-RF34-02** | F (DEF-GP1-01) | **P** | `PF-RF34-02_*.png` | Un gesto, nada pegado |
| PF-RF04-02 | P | P | `S-N2C-rc2_OE4_B3_final.json`; `GP3_OE4GP3_final.json` | N2F3 3 · 40 · 9 · 497,81 s, previsto 497,70 s |
| PF-RF03-02 | P | P | `PF-RF03-02_N3_desbloqueado.png` | |
| PF-RF12-02 | P | P | S-N2C-rc2 paso 16 | |
| PF-RF45-02 | P | P | `PF-RF45-02_cierre_y_resumen_N2.png` | Sin dígitos |
| PF-RF45-04 | P | P | S-INI-rc2 pasos 22–23 | Repetir el N1 no reescribe sus indicadores |
| **PF-RF45-05** | sin ejecutar | **P** | `PF-RF45-05_doble_omitir_al_resumen.png` | Doble clic a 95 ms sobre «Omitir» del cierre (no sobre el último «Continuar»): llega al resumen sin saltárselo |
| PF-RF46-01 | P | P | `OBS-SDOC-2_informe_perfil_de_OE4_Z.png` | El encabezado nombra el perfil |
| PF-RF47-01 | P | P | `PF-RF47-01_papelera_octava_fila.png` | En la 8.ª fila |
| PF-RNF02-01 | P parcial | P parcial | `PF-RNF02-01_lista_con_flechas.png`; S-INI-rc2 paso 5 | Las listas de perfiles y del laberinto solo se mueven con clic; la parte de SUITE ya está (376/376). Faltan los pasos de teclas en el taller y el laberinto |
| PF-RNF04-01 | P parcial | P parcial | §8.6 | Falta el equipo 2 |
| PF-RNF05-01 | P parcial | P parcial | §8.6 | Falta el equipo 2 |
| PF-RNF06-01 | P | P | `S-RNF-rc2_registro_de_pasos.md` | 479,0 MB |
| **PF-RNF10-01** | F (DEF-SPIKE-01) | **P** | `*_red.csv` | Ninguna conexión observada en 1312 muestras; el arranque no se muestrea (§8.6) |
| PF-RNF11-01 | P | P | `*_residuos.txt`; S-PER-rc2 pasos 3 y 6; GP3 paso 27 | Es el veredicto de rc1 (S-DOC buscó los nombres de los perfiles borrados en la carpeta, LocalLow, `%TEMP%` y el registro) más lo que se comprobó sobre rc2: los perfiles borrados (`OE4_Corrupto`, `CON`) desaparecen de `Datos/`. Sobre rc2 no se repitió la búsqueda de esos nombres. «`OE4GP3` solo en su JSON» consta en el registro de GP3 (paso 27): `GP3_residuos.txt` no trae la salida de `-Buscar`. Portabilidad: DEF-RC2-01 |
| PF-RNF13-01 | P parcial | P parcial | §8.5 | GP3 pasa; falta el recorrido de Santiago (H8) |
| **PF-RNF21-01** | P parcial | **P con reserva** | `PF-RNF21-01_ignicion_10fps.png`, `PF-RNF21-01_ignicion_{a,b}.csv` | Ignición grabada: la luz sube sin saltos ≥ 0,02 por cuadro, y el único par opuesto ≥ 0,10 es el fundido (1 por segundo, frente al umbral de 3). La parte de SUITE ya está. **Reserva:** 10 fps solo resuelve hasta 5 Hz, y quedó sin grabar el tramo de +0,67 a +1,23 s tras soplar |

**Lo que no se repitió** conserva el veredicto de rc1, con la SUITE de rc2 en verde y GP3 como regresión:
- S-PER bloques A–C (los cinco PF-RNF14 y los siete cierres forzados);
- S-DOC (PF-RF46-02, PF-RF46-03, PF-RF47-02, PF-RF47-03, PF-RNF09-01) y PF-RNF07-01;
- los casos de los tramos de GP1 que rc2 no toca.

**PF-RNF16-01 no se repitió sobre rc2:** rc2 no cambia ningún `.asmdef` (comprobado con `git status`), y
`Architecture_RNF16_*` pasa en la SUITE de rc2.

### 8.5 Tercer recorrido completo (GP3, PF-RNF13-01)

| Recorrido | Condición | Inicio → cierre (UTC) | Duración | Tiempo de resolución en el JSON (7 fases) |
|---|---|---|---|---|
| GP3 | Perfil nuevo `OE4GP3`, **pantalla completa** 1920 × 1080, un solo proceso (pid 18436), un solo agente, sin errores a propósito | 2026-10-02 01:17:53,251Z → 01:34:01,371Z | **16 min 8,1 s (16 min)** | 400,40 s (6 min 40 s) |

De la pantalla de inicio a los créditos y de vuelta al inicio: N1, N2 con sus tres fases, N3 con la recolección y sus
tres fases, y la escena final. Lo hizo sin bloqueos, cierres inesperados ni estados irrecuperables, sin
`Exception`, `Error` ni `Assert` en `Player.log`, sin ninguna conexión de red, y en LocalLow solo dejó el `Player.log`.

| Fase | JSON (I · C · P · T) | Previsto | Veredicto |
|---|---|---|---|
| N1F1 | 0 · 0 · 3 · 61,10 s | 0 · 0 · 3 · ≈ 60,8 s | P |
| N2F1 | 0 · 0 · 0 · 76,83 s | 76,5 s | P |
| N2F2 | 0 · 0 · 6 · 62,76 s | 62,38 s | P |
| N2F3 | 0 · 0 · 10 · 104,14 s | 104,13 s | P |
| N3F1 | 0 · 0 · 1 · 46,12 s | ≈ 45,96 s | P |
| N3F2 | 0 · 0 · 2 · 32,21 s | 32,2 s | P |
| N3F3 | 0 · 0 · 3 · 17,23 s | 17,25 s | P |

- **Incidencias del juego: ninguna.** Del arnés hubo una: un «Continuar» de más cayó en «Ejecutar» del laberinto recién
  cargado, con la secuencia vacía; salió el aviso y no sumó intento (attempts 0 en el JSON).
- **Por qué dura la mitad que GP2** (32 min 57 s por el mismo camino sin errores): la diferencia es del arnés, no del
  juego. GP3 hizo bucles de «Continuar» sin foto, no transcribió el tablero a mano y sacó menos fotos de control.
  **No mide a un estudiante**: eso es la Hoja-HUM, H8.
- **Confirma sobre rc2,** sin cambio de veredicto: PF-RF02-01 (nombre válido), PF-RF03-02, PF-RF04-01..03, PF-RF05-01,
  PF-RF06-01, PF-RF08-01, PF-RF14-01, PF-RF19-01, PF-RF20-01, PF-RF22-01..PF-RF29-01, PF-RF35-01..PF-RF39-01, PF-RF40-01,
  PF-RF41-01, PF-RF44-01 y PF-RF45-01..03.
- **Observaciones:** sigue OBS-GP2-1 (la 3.3 dice «la balsa corregida» sin que hubiera fallo). Las demás de rc1 (OBS-5,
  -6, -8, -12, -15) no se revisaron una a una: rc2 no tocó esas pantallas.
- **Veredicto:** la parte del arnés de PF-RNF13-01 pasa por tercera vez, ahora sobre rc2 y a pantalla completa. El caso
  sigue en **P parcial** hasta el recorrido cronometrado de Santiago (H8).

### 8.6 Medidas sobre rc2

Fuentes, todas en `rc2/`: los siete `Player.txt` (S-INI-rc2, S-N2C-rc2, S-PER-rc2 lanz. 1–4 y GP3), y de cada una de
las cuatro sesiones su `*_mem.csv`, su `*_red.csv`, su `*_residuos.txt` y su `*_registro.tsv` (la cola del
`Player.log` con la hora de cada línea, que da la hora de cada lanzamiento), más `S-RNF-rc2_registro_de_pasos.md`.
MiB = 2²⁰ B y MB = 10⁶ B.

*(Nota del 01/10/2026, 21:45.)* `S-PER-rc2_red.csv`, `S-N2C-rc2_mem.csv`, `S-PER-rc2_mem.csv`,
`S-N2C-rc2_residuos.txt`, `S-PER-rc2_residuos.txt` y los cuatro `*_registro.tsv` estaban solo en la carpeta temporal
de la sesión y se copiaron a `rc2/` sin cambios; los demás ya estaban y son idénticos a sus originales. Con ellos se
reproducen desde el repositorio las 1312 muestras y la frase «en las cuatro sesiones» de §8.7. `GP3_residuos.txt`
no trae la salida de `Residuos GP3 -Buscar OE4GP3`: que el nombre solo aparece en su JSON consta únicamente en
`GP3_registro_de_pasos.md`, paso 27.

| Medida | rc1 | rc2 | Veredicto (rc2) |
|---|---|---|---|
| RNF-04, peor carga de `SceneLoader` | 0,765 s; 188 cargas en 20 lanzamientos | **0,780 s**, `Narrative` al abrir `N1_Apertura` tras crear el perfil `CON` (S-PER-rc2 lanz. 1); 74 cargas en 7 lanzamientos, todas < 1 s | P parcial (falta el equipo 2) |
| RNF-05, memoria de trabajo máx. | 435,7 MiB = 456,9 MB (GP2, `Level3_River`) | **425,5 MiB = 446,2 MB** (GP3, `Narrative` en `N3_Escena33_Cruce`) | P parcial (falta el equipo 2) |
| RNF-05, memoria privada máx. | 1 140,1 MiB = 1 195,5 MB (GP1, `N3_PuenteII`) | **1 124,6 MiB = 1 179,2 MB** (GP3, `Narrative` en `N1_NacimientoDelFuego`) | ídem; < 2048 MiB |
| RNF-06, tamaño | 478 979 915 B = 479,0 MB | **478 987 619 B = 479,0 MB** (456,8 MiB) | P |
| RNF-10, red | 3 TCP :443 a Google Cloud en 20/20 lanzamientos | **No se observó ninguna conexión** TCP o UDP, ni de escucha local, en 1312 muestras de 2 s (325 + 323 + 181 + 483) | P |
| RNF-11, residuos fuera de `Datos/` | `Player.log`; clave del reproductor con 16 valores de pantalla y 6 `unity.*`/`unity_connect.*`; Analytics, Insights y `ShaderVariantAnalytics` en LocalLow | `Player.log`; clave con los valores de pantalla (16 en ventana, 14 a pantalla completa, contado `UnitySelectMonitor`), un contador y un identificador de sesión del reproductor (`unity.player_session_count`, `unity.player_sessionid`) y tres `unity_connect.*`; nada nuevo en `Unity\…\Analytics`, `Insights` ni `ShaderVariantAnalytics`; sin `unity.cloud_userid`; en `%TEMP%`, nada del juego | P (rastro del perfil; la búsqueda de los nombres borrados es de rc1, §8.4) · DEF-RC2-01 |
| Registros | 1 `Exception` (el perfil ilegible, S-PER lanz. 13) | **0** líneas con `Exception`, `Error` o `Assert` en las siete ejecuciones | P |

- **Peor carga por escena en rc2:**

  | Escena | Peor carga (n) |
  |---|---|
  | `Narrative` | 0,780 s (35) |
  | `Level1_Cave` | 0,154 s (2) |
  | `MainMenu` | 0,104 s (11) |
  | `Level2_Maze` | 0,088 s (2) |
  | `Level2_Workshop` | 0,086 s (1) |
  | `Level3_River` | 0,085 s (3) |
  | `LevelSelect` | 0,081 s (12) |
  | `TeacherReport` | 0,053 s (1) |
  | `Level2_Forest` | 0,052 s (1) |
  | `Credits` | 0,037 s (1) |
  | `LevelSummary` | 0,035 s (5) |

  Varias escenas tienen menos de tres cargas en rc2. Las dos versiones empaquetan las mismas texturas de escena, así
  que la tabla de §4.1 sigue siendo la que tiene n ≥ 3.
- **Memoria por sesión** (trabajo / privada, MiB): S-INI-rc2 418,0 / 1 116,4; S-N2C-rc2 413,8 / 1 093,3; S-PER-rc2
  411,6 / 1 109,0; GP3 425,5 / 1 124,6. La privada sigue la misma forma que en rc1: ~700 MiB en el inicio y ~1,1 GiB
  desde la primera narrativa.
- **Qué no ve el muestreo de red.** El muestreador empieza cuando el arnés lee por primera vez el `Player.log`, así que
  el arranque de cada lanzamiento queda sin muestrear: 2,7 s en GP3, 2,8 s en S-N2C-rc2 y en el primero de S-PER-rc2 y
  8,1 s en S-INI-rc2 (foto previa a las 00:42:15,4Z, primera muestra a las 00:42:23,5Z). Además, una conexión que
  durara menos de 2 s podría caer entre dos muestras. Por eso se escribe «no se observó» y no «no abrió». El veredicto P
  se sostiene por tres razones: en rc1 las conexiones eran persistentes y salían en todas las muestras;
  `globalgamemanagers` no nombra `unity3d.com` (0 apariciones), y `UnityServicesOff` haría fallar el build si lo
  nombrara. RNF-08, el juego sin red, sigue pendiente del guion H4 de la Hoja-HUM.

### 8.7 Defecto abierto (resto de DEF-SPIKE-01) y riesgo residual

#### DEF-RC2-01 · El reproductor sigue escribiendo `unity_connect.*` en el registro

*(Título corregido el 01/10/2026, 21:45: decía «Defecto nuevo». No lo es: DEF-SPIKE-01 ya nombraba los
`unity_connect.*` de HKCU, y esto es lo que queda de él tras rc2.)*

- **Caso:** la comprobación de DEF-SPIKE-01 en el registro (PF-RNF11-01, parte de portabilidad; RNF-07, RNF-11).
  **Versión:** rc2. **Severidad: Menor.** **Tipo:** configuración del motor.
- **Reproducción:**
  1. Lanzar rc2 y usar cualquier pantalla.
  2. Cerrar el juego.
  3. Mirar `HKCU\Software\Universidad Catolica de Colombia\Algoritmia`.
- **Esperado** (decisión del 29/09, §4.5): fuera de `Datos/` solo el registro y las claves de pantalla.
- **Observado:** en las cuatro sesiones (21, 21, 21 y 19 valores) aparecen además
  `unity_connect.installation_id_h932087899` (`790d6a9c-8649-0aa4-fa9a-2f9d7194021d`),
  `unity_connect.session_id_h4145606137` y `unity_connect.mega_session_id_h1802243016`, y un contador y un
  identificador de sesión del reproductor, `unity.player_session_count_h922449978` y
  `unity.player_sessionid_h1351336811`. `unity.cloud_userid` ya no está. *(Corregido el 01/10/2026, 22:00: el
  «Observado» solo listaba los tres `unity_connect.*`; los dos `unity.player_*` constan en los cuatro
  `*_residuos.txt` y en §8.6.)*
- **Impacto:** un identificador de instalación persistente queda en `HKCU`, fuera de la carpeta portable. No se
  observó que salga del equipo (ninguna conexión en 1312 muestras) ni es un dato del estudiante, así que no incumple
  RNF-10 ni el rastro del perfil de RNF-11.
- **Causa probable (sin verificar):** el motor inicializa esos identificadores aunque los servicios estén apagados.
  Que el responsable sea el módulo integrado `com.unity.modules.unityanalytics` tampoco está probado: podrían
  escribirlos partes del motor que no salen de ese módulo.
- **Opciones:**
  - **(a)** desactivar `com.unity.modules.unityanalytics` en `Packages/manifest.json`. Choca con la decisión D3 y no
    está probado que baste.
  - **(b)** documentarlo como residuo conocido del motor, junto al `Player.log` y las claves de pantalla.
  - **(c)** poner a 0 `UnityAnalyticsSettings.m_InitializeOnStartup` en `ProjectSettings/UnityConnectSettings.asset`
    (hoy en 1, igual que el de `UnityAdsSettings`). `UnityServicesOff` apaga nueve interruptores, pero no ese. Es un
    ajuste del proyecto, compatible con D3.
  - **(d)** borrar esas tres claves de `PlayerPrefs` al salir (`Application.quitting`), tras comprobar que el motor no
    las vuelve a escribir después. También compatible con D3.
- **Decisión D-RC2-01** (Claude, por delegación de Santiago, 01/10/2026): **(b)** para la entrega. (c) y (d) quedan
  como mejora recomendada para un build futuro, porque cada una pide un build, repetir los residuos y pasar la SUITE.
  Detalle en §8.10.
- **Evidencia:** `rc2/S-INI-rc2_residuos.txt`, `rc2/S-N2C-rc2_residuos.txt`, `rc2/S-PER-rc2_residuos.txt`,
  `rc2/GP3_residuos.txt` y S-INI-rc2 paso 32.

#### R-RC2-A · La ventana interna de `File.Replace` deja un instante sin `<perfil>.json` en disco

- **Qué es:** `File.Replace` (`ReplaceFileW`) primero aparta el perfil anterior con un nombre temporal
  (`<perfil>.json~RF….TMP`) y después pone el `<perfil>.json.tmp` en su sitio. Entre esos dos pasos no hay ningún
  `<perfil>.json` en `Datos/`: solo el `.tmp` (lo nuevo) y el `~RF….TMP` (lo anterior). `GetFiles("*.json")` no lista
  ninguno de los dos y `SaveStore` no recupera desde el `.tmp`.
- **Qué pasaría:** un corte de luz o un cierre forzado justo en esos milisegundos deja el perfil **fuera de la lista**,
  no truncado: su avance sigue entero en el `.tmp`. Si después se crea un perfil con el mismo nombre, el nuevo pisa el
  `.tmp` con su avance, y el `~RF….TMP` queda huérfano en `Datos/`, también tras borrar (RNF-11). Choca con el
  invariante 3 tal como lo enuncia DEF-SPER-02.
- **Por qué no se vio:** la segunda prueba de S-PER-rc2 (paso 12) despertó con ese renombrado, pero el cierre llegó con
  el reemplazo ya hecho. La ventana no se ejercitó.
- **Severidad: Menor.** La probabilidad es ínfima (milisegundos) y no hay ningún caso que falle.
- **Decisión D-RRA** (Claude, por delegación de Santiago, 01/10/2026): queda como **riesgo residual documentado**. No
  se corrige en este corte, porque rc2 es el ejecutable verificado. **Arreglo mínimo propuesto:** al listar o cargar
  perfiles, si falta `X.json` y `X.json.tmp` se lee bien, ponerlo en su sitio (`File.Move`), con su prueba
  `SaveStore_RNF14_…` de rojo a verde.
- **Evidencia:** `rc2/PF-RNF14-guardado_matar2.txt` (el vigilante despierta con `Renamed` de `OE4_E.json~RF71ee4d3.TMP`
  y el listado posterior ya no tiene ni `~RF…TMP` ni `.tmp`).

### 8.8 Recuento vigente (rc2)

Vale el veredicto de rc2 donde el caso se repitió, y el de rc1 tras la revisión de W5 donde no (§8.4).

| Veredicto | Casos |
|---|---|
| P | 89: los 77 de rc1, más PF-RF02-06, PF-RF20-03, PF-RF32-01, PF-RF34-01, PF-RF34-02 y PF-RNF10-01 (antes F), PF-RF20-02 (antes NE), PF-RF06-02, PF-RF16-01 y PF-RNF21-01 (antes P parcial; PF-RNF21-01 con reserva) y PF-RF06-03 y PF-RF45-05 (nuevos) |
| P parcial | 13: PF-RF02-01, PF-RF07-01, PF-RF07-02, PF-RF11-02, PF-RF11-03, PF-RF13-05, PF-RF42-01, PF-RF42-02, PF-RNF02-01, PF-RNF04-01, PF-RNF05-01, PF-RNF13-01, PF-RNF19-01 |
| F | 0 |
| NE | 1: PF-RF40-02 |
| B / NA | 0 |
| **Total** | **103** de los **127** casos del catálogo: los 102 que ya estaban y PF-RF02-06, que propuso la revisión de W5 y entró en `casos.md` el 01/10/2026 |

- **Eficacia provisional:** `(P + PD) / (P + PD + F)` = **89 / 89 = 100 %**, con «P parcial» y NE fuera del cómputo.
  **No es el KPI** (T24), que se calcula sobre el catálogo completo de 127 casos (126 hasta que PF-RF02-06 entró en
  `casos.md`; el recuento no cambia). Faltan:
  - 13 casos a medio ejecutar y 1 NE;
  - 24 sin ejecutar: los de §7 salvo PF-RF06-03 y PF-RF45-05, que se hicieron sobre rc2. Son PF-RF01-02, PF-RF02-03,
    PF-RF03-01, PF-RF06-04, PF-RF07-03, PF-RF07-04, PF-RF11-01, PF-RF21-01, PF-RF21-02, PF-RNF01-01, PF-RNF03-01,
    PF-RNF07-02, PF-RNF08-01, PF-RNF12-01, PF-RNF15-01, PF-RNF17-01, PF-RNF18-01, PF-RNF20-01, PF-RNF22-01,
    PF-RNF23-01, PF-CT02-01 y PF-SON-01..03.
- **Segundo KPI:** ningún caso ejecutado de un RF de prioridad Alta termina en F. RF-02, RF-32 y RF-34 ya no tienen
  ningún Mayor abierto. DEF-RC2-01 es Menor y no toca un RF.

### 8.9 Lo que queda abierto tras rc2

- **DEF-RC2-01:** se entrega documentado como residuo del motor (D-RC2-01, §8.10). Las opciones (c) y (d) de §8.7
  quedan para un build futuro.
- **R-RC2-A:** riesgo residual Menor, documentado con su arreglo mínimo (D-RRA, §8.7).
- **RNF-03 no se probó en otra resolución.** Todas las sesiones fueron a 1920 × 1080, y los tres recorridos a 16:9. En
  batchmode a 640 × 480 fallan 14 pruebas de disposición (control de §4.7). Queda para el equipo 2.
- *(Hecho el 01/10/2026, 21:45.)* PF-RF02-06 ya está en `casos.md` (S-INI, pasos 18–21), y S-N2C recoge que un clic
  sin mover sobre un bloque no cuenta como edición y que en el paso 11 vaciar con la papelera ya no exige abrir el
  cajón.
- **Observaciones abiertas.** Ninguna es bloqueante. Se documentan con su arreglo propuesto y no se corrigen en este
  corte (decisión D-OBS, §8.10). Las que se ven jugando van a la Hoja-HUM, guion H13.

  | Id | Qué pasa | Arreglo propuesto |
  |---|---|---|
  | **OBS-rc2-2** | El informe docente desplaza con `ScrollRect` la lista de perfiles y la tabla, y los créditos su texto (tres en total). *Corrección del 01/10, 21:45:* esta sección decía que iba «contra la regla de `CLAUDE.md`». No es así: es la **excepción decidida el 29/09/2026** (`Slice 4/Slice-4-Resultados.md` A.6; `INCONSISTENCIAS.md` INC-80, nota del 01/10). Esas pantallas no tienen arrastrar y soltar, y su barra se maneja con clic y clic sostenido. La regla «listas con botones, nunca `ScrollRect`» rige donde hay arrastrar y soltar, como en la secuencia del laberinto (▲/▼) | Ninguno (decisión D-SCR) |
  | 8.ª fila del informe | Con ocho perfiles, la 8.ª fila de la lista del informe sale con el borde inferior recortado hasta que se desplaza la lista; se ve y se elige (`rc2/OBS-SDOC-2_informe_perfil_de_OE4_Z.png`) | Que el alto de la ventana de la lista sea un número entero de filas con su espaciado, o un relleno inferior igual al espaciado, para que la última fila visible quede entera |
  | N7 | `PerfilSeleccionado` trae en `TeacherReport.unity` el texto fijo «Perfil de Ana». `Select` lo reemplaza al abrir, así que no se ve | Dejar ese texto vacío en la escena |
  | **OBS-rc2-5** | Tras una ejecución fallida en el laberinto, la papelera del bloque seleccionado ya no está donde estaba antes de «Ejecutar»: en S-N2C-rc2 (paso 9), 50 px más abajo, y el primer clic cayó fuera. **Triaje (01/10, lectura de `MazeSceneController.Highlight` y `SequenceListRules.RevealScroll`): la introduce la corrección de DEF-GP1-02.** Antes de rc2, `Highlight` ya agrandaba ×1,04 el bloque donde se detuvo un fallo, lo que mueve unos pocos píxeles. Desde rc2 termina además en `RevealRow`: durante la ejecución la lista sigue al bloque en curso, y al fallar queda en la posición que deja ver el bloque donde se detuvo, con 8 px de margen, que ya no es la de antes de ejecutar. **Severidad: observación**, equivalente a Trivial (la escala de `plan.md` §6 no tiene grado por debajo de Menor). El bloque y su papelera se ven, el clic errado no hace nada ni cuenta edición, y editar no cuesta (CP-02) | Guardar el desplazamiento al pulsar «Ejecutar» y, tras un fallo, volver a él si el bloque donde se detuvo cabe en esa vista; si no cabe, dejar el de `RevealRow` |
  | N2 | Bajar un bloque una sola posición, soltándolo en la mitad de abajo del que subió a su sitio, no hace nada; hay que soltarlo más abajo | Reconocer el «clic sin mover» por la distancia entre `pressPosition` y `position` frente a `EventSystem.pixelDragThreshold`, en lugar de la regla del hueco de origen (`_pickupSlot`) |
  | N3 | Un doble clic en la papelera del laberinto retira dos bloques: tras retirar, la papelera de la fila siguiente queda bajo el cursor. Aceptable, porque editar no cuesta (CP-02) | Si se quiere, que la papelera ignore un segundo clic que llegue antes de unos 0,3 s |
  | N4 | El aviso del perfil ilegible sale en la columna «Perfil nuevo» y sigue ahí después de borrar ese perfil | Mostrarlo en el panel de perfiles guardados y limpiarlo al borrar ese perfil o al elegir otro |
  | N5 | La flecha de página que no lleva a ninguna parte se oculta en vez de apagarse; el laberinto las apaga | Dejarla visible con `interactable = false`, como ▲/▼ del laberinto |
  | N6 | «Fuerza del golpe», «Fuerte» y «Suave» no se atenúan al soplar, porque no cuelgan del `Slider` | Atenuarlos con el mismo `CanvasGroup` de alfa 0,5 que `LockControls` pone en cada deslizante |
  | N11 · N13 | Un `.tmp` huérfano que otro proceso retiene puede hacer fallar un guardado (N11) o un borrado (N13). Muy improbable | Reintentar unas pocas veces, con una espera corta, la escritura o el borrado del temporal antes de rendirse |
  | N12 | Ningún llamador de `SaveStore.Save` captura un fallo de disco (disco lleno, `Datos/` que deja de admitir escritura). Riesgo anterior a rc2 | Capturar el fallo en un solo sitio y mostrar un aviso sin culpa, como el del perfil ilegible, sin dejar el juego atascado (RNF-13) |
  | OBS-2 | Un rayo efectivo casi horizontal, en el borde del abanico, se dibuja sobre el sílex antes de llegar a las hojas | Estrechar el abanico de direcciones o dibujar el rayo bajo las piedras |
  | OBS-GP2-1 | Sin fallo en la balsa, la 3.3 dice «la balsa corregida» y Algoritm habla de algo que falló. Es el texto del guion §1.8.5, radicado | Una variante de la 3.3 para el camino sin fallo, elegida por `ConditionalNarrativeTrigger` como ya se elige la 3.2 (un asset más, sin tocar el controlador); el texto nuevo lo aprueba Santiago |
  | OBS-SDOC-1 (= OBS-rc2-1) | En la cabecera del informe, el icono de «Errores corregidos» y «Tiempo de resolución» se dibuja sobre la primera palabra | Con los iconos definitivos (Slice 4, D1): icono a la izquierda del rótulo, con su propio ancho |
  | R-6 | En 14 narrativas, pulsar «Continuar» mientras un personaje camina lo hace saltar a su destino; en la 3.3 ya no pasa | Marcar a esos personajes con `NarrativeProp.FinishesSteps`, asset por asset, y revisar la escenificación de esas escenas, que ya estaban revisadas |
  | Retratos de Niña y Papá | Motas verdes opacas (α ≥ 200) en las puntas del pelo, unas 12–13 visibles a 1080p | Limpieza a mano en el carril de arte: una automática tocaría píxeles opacos del dibujo |
  | Balsa del cruce | La balsa de la 3.3 tiene 4 troncos y la mecánica arma 5. Arte aceptado (D-a) | Redibujarla con cinco troncos en una entrega del carril de arte |
  | `prop_n2_piedra_c` y `_d` | Siguen en `Sprite Mode: Multiple` y no los usa nada (excepción de INC-128) | Pasarlos a `Single` desde el motor (`TextureImporter.spriteImportMode`) o retirarlos |
  | `Unity.AI.MCP.Runtime` y `Unity.AI.Tracing` | Entran al player en `Managed/`; no se observó que abran red (§8.6) | Excluirlos del player en un build futuro. Hoy eso pide tocar el manifiesto (D3), así que lo decide Santiago |

- **Riesgo para la Hoja-HUM:** el clic perdido tras cargar una escena sin mover el puntero (§6), en H13.
- **Al empaquetar la entrega:** se quitan `Datos/` y la carpeta `Algoritmia_BurstDebugInformation_DoNotShip`.
  `Build/Algoritmia` quedó como salió del build, sin `Datos/` (S-RNF-rc2 paso 1), y la clave del registro se borró
  tras cada sesión.
- **Sin ejecutar:** los 24 casos de §8.8, entre ellos todo S-HUM (equipo 2, sonido, modo avión y el recorrido
  cronometrado).

### 8.10 Decisiones de Claude por delegación de Santiago (01/10/2026)

Santiago delegó el 01/10/2026 las decisiones del cierre: se toma la más recomendada y acorde a la documentación, y
ninguna que comprometa el estado del proyecto. Las cinco que siguen se tomaron así, después de la revisión final de
rc2. Ninguna cambia el ejecutable, `Packages/manifest.json` ni un veredicto.

| Id | Decisión | Por qué |
|---|---|---|
| **D-RC2-01** | DEF-RC2-01 se documenta como residuo del motor para la entrega (opción (b) de §8.7). Las opciones (c), `m_InitializeOnStartup = 0` en `UnityConnectSettings.asset`, y (d), borrar esas claves de `PlayerPrefs` al salir, quedan como mejora recomendada para un build futuro | Las dos piden un build nuevo, repetir los residuos y pasar la SUITE, y rc2 es el ejecutable verificado. El residuo no sale del equipo ni es un dato del estudiante (§8.7) |
| **D-RRA** | R-RC2-A queda como riesgo residual Menor documentado, con el arreglo mínimo propuesto: al listar o cargar, si falta `X.json` y `X.json.tmp` se lee bien, ponerlo en su sitio. Los textos no hablan de un cierre dentro del reemplazo: el cierre forzado se probó «entre la escritura del temporal y el reemplazo» y «justo después del reemplazo» | No se corrige en este corte porque rc2 es el ejecutable verificado; la ventana dura milisegundos y deja el avance entero en el `.tmp` |
| **D-SCR** | Los `ScrollRect` del informe docente (2) y de los créditos (1) son la excepción decidida el 29/09/2026 (`Slice 4/Slice-4-Resultados.md` A.6, INC-80). La regla «listas con botones, nunca `ScrollRect`» rige donde hay arrastrar y soltar. La 8.ª fila recortada del informe con ocho perfiles es una observación menor | Esas pantallas no tienen arrastrar y soltar, y su barra se maneja con clic y clic sostenido (CT-06) |
| **D-OBS** | Las observaciones menores abiertas se documentan con su arreglo propuesto (§8.9) y no se corrigen en este corte: N2–N7, N11–N13, OBS-2, OBS-GP2-1, OBS-SDOC-1, OBS-rc2-5, la 8.ª fila del informe, R-6 en 14 narrativas, las motas del pelo de los retratos de Niña y Papá, la balsa del cruce con 4 troncos frente a 5, `prop_n2_piedra_c`/`_d` en `Multiple` sin uso y `Unity.AI.MCP.Runtime`/`Unity.AI.Tracing` dentro del player | Ninguna es bloqueante ni niega un RF; corregirlas cambiaría el ejecutable verificado |
| **D-PAG** | La prueba de paginación de perfiles pasa a tres perfiles por página (`ProfilePaging_RF02_ContarPaginasDeTresPerfiles`: 8 perfiles, 3 páginas, como en el exe), y el comentario de `ArtImportRules.cs` se corrige: 67 PNG, que son 134 entradas del BuildReport, y 866,7 MB | Son cambios posteriores al build en una prueba y en un script de Editor, y no alteran el ejecutable (`build-rc2.md`, «Cambios posteriores al build»). Las 9 pruebas de `ProfilePaging` y las 3 de `ArtImport` pasan (01/10, 21:38; `suites/rc2-posterior/`) |
