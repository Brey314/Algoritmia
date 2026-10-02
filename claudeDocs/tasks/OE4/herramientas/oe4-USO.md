# `oe4.ps1` · guía de uso del arnés del ejecutable

`oe4.ps1` maneja `Algoritmia.exe` como **caja negra**, igual que un jugador: lo lanza, mira la pantalla, hace clic,
sostiene, arrastra y escribe. Mientras tanto mide cargas, memoria, red y residuos. Con él se ejecutaron sobre el
ejecutable las sesiones del carril OE4 (`../casos.md`, `../OE4-Resultados.md`) y los tres *Golden Path* (decisión D-f
de Santiago). **No es código del juego** y no entra en el build.

```
pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/oe4.ps1 <Subcomando> [argumentos] [-Opciones]
pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/oe4.ps1 help
```

Cada llamada es un proceso nuevo y no guarda nada en memoria. **Salidas:** 0 bien · 1 hallazgo (`Exception` en el
registro, residuos encontrados, una carga ≥ 10 s, el paquete ≥ 500 MB) · 2 fallo de infraestructura (no hay juego,
argumento inválido, tiempo agotado) · 3 guarda (sin foco, otra ventana encima, SHA-256 distinto, juego ya en marcha,
otra invocación en curso): **con la salida 3 no se envió nada**.

## Requisitos

- **PowerShell 7** (`#requires -Version 7.0`) y Windows con escritorio interactivo: con la pantalla bloqueada o un
  escritorio remoto desconectado no hay captura.
- **Un monitor visible.** El juego se lanza en el monitor 1. Si el cliente pedido no cabe con marco (1920 × 1080 en un
  monitor de 1920 × 1080), se lanza sin marco (`-popupwindow`). El arnés se declara DPI-aware por monitor, así que el
  escalado de Windows (125 % en el equipo 1) no mueve los clics.
- **El foco.** El Input System del juego ignora la entrada sin foco. Antes de cada entrada y de cada foto el arnés
  pone la ventana en primer plano. Si no lo consigue, o si otra ventana tapa el punto, sale con 3 y no pulsa.
- **Nadie toca el ratón ni el teclado** mientras corre una sesión. Si el cursor se movió entre mover y pulsar, sale
  con 3 («¿alguien tocó el ratón?»). En el equipo de desarrollo las sesiones se hicieron con el equipo libre y con el
  Editor de Unity y Word cerrados: `Start-Juego` avisa de contaminación si los encuentra, porque falsean RNF-04 y
  RNF-05.
- **Coordenadas** en fracciones 0–1 del **área cliente**, con origen arriba a la izquierda (admite `0.5` y `0,5`), como
  `IllustrationFraming`: no dependen de la resolución.

## Carpeta de trabajo (`OE4_TRABAJO`)

Todo lo que produce el arnés va a `$env:OE4_TRABAJO`, por defecto `%TEMP%\Algoritmia-OE4`:

| Qué | Dónde |
|---|---|
| Estado entre llamadas (sesión, pid, offset del `Player.log`, foto previa) | `estado.json` |
| Fotos y ráfagas | `fotos\`, `rafagas\` |
| Muestras por sesión: memoria, red y cola del `Player.log` con hora | `muestras\<sesión>_mem.csv`, `_red.csv`, `_registro.tsv` |
| Copias del `Player.log` | `logs\<sesión>_Player.txt` (y `_lanzN_Player.txt` por lanzamiento) |
| Foto previa y diferencia de residuos | `residuos\previo-<sesión>.json`, `residuos\<sesión>.txt` |
| Cargas medidas y acciones enviadas | `cargas.csv`, `acciones.tsv` |

Es temporal: lo que respalda un veredicto se copia a `../evidencias/<sesión>/` al cerrar la sesión. Otras variables:
`OE4_EXE` (por defecto `C:\Dev\Algoritmia\Build\Algoritmia\Algoritmia.exe`), `OE4_SHA256`, `OE4_LOGDIR`, `OE4_REGKEY` y
`OE4_STRICT=1` (para depurar el arnés).

## Subcomandos

Los ejemplos son de las sesiones del 01/10/2026. Se abrevia `oe4` por `pwsh -NoProfile -File …\oe4.ps1`.

| Subcomando | Qué hace | Ejemplo |
|---|---|---|
| `Start-Juego [sesión] [-Ancho -Alto \| -Completa] [-SinMarco] [-Sha256 h]` | Comprueba el SHA-256 del exe, aparta los `Player.log` anteriores, toma la **foto previa** de residuos, lanza el juego, espera una ventana estable y arranca el muestreador oculto | `oe4 Start-Juego S-INI-rc2 -Sha256 6cd16edf…adab` · `oe4 Start-Juego GP3 -Completa` |
| `Foto <nombre> [-Gris] [-Espera ms]` | PNG del área cliente; con `-Gris`, también en grises (RNF-19). Avisa si el cuadro sale casi negro | `oe4 Foto rc2ini_002` |
| `Clic <x> <y> [-Veces n] [-Intervalo ms] [-ConFoto f] [-FotoMs ms]` | Clic; con `-ConFoto`, foto en la misma llamada | `oe4 Clic 0.447 0.222 -ConFoto rc2ini_003` |
| `Sostener <x> <y> <ms>` | Clic sostenido sin moverse (las flechas del N3) | `oe4 Sostener 0.833 0.787 800` (una flecha del N3, en GP3) |
| `Arrastrar <x1> <y1> <x2> <y2> [ms=700]` | Pulsa, unos 20 movimientos, suelta: supera el umbral de arrastre de uGUI | `oe4 Arrastrar 0.254 0.266 0.48 0.48 600` (una hoja al círculo del N1) |
| `Escribir <texto>` | Texto Unicode (solo el nombre del perfil) | `oe4 Escribir CON` |
| `Teclas <t1 t2 …> [-Pausa ms]` | Teclas, combinaciones, repeticiones y la rueda; sirve para los controles negativos | `oe4 Teclas RuedaAbajo*3` · `oe4 Teclas Esc Enter Espacio Arriba Abajo Izquierda Derecha W A S D Rueda` |
| `Esperar-Carga <escena\|*> [s=30]` (alias `Medir-Carga`) | Espera en el `Player.log` la línea `RNF-04: «<escena>» cargó en N s` y devuelve N | `oe4 Esperar-Carga Narrative` |
| `Rafaga <prefijo> <s> <fps>` | Cuadros seguidos y un CSV con la luminancia de cada uno (RNF-21) | `oe4 Rafaga gp1_rafaga_llama 6 10` (la llama del N1, en GP1) |
| `Preparar-Datos <perfil…> [-SinCarpeta]` | Con el juego **cerrado**: vacía `<exe>\Datos`, valida cada semilla de `perfiles\` contra el formato persistido y la copia; borra también perfiles sueltos en la ruta de respaldo (INC-34). `-SinCarpeta` deja el build sin `Datos/` | `oe4 Preparar-Datos OE4_A OE4_B OE4_B2 OE4_B3 OE4_C OE4_D OE4_E OE4_Z` · `oe4 Preparar-Datos -SinCarpeta` |
| `Matar [-Todos]` | `taskkill /F /T`: el cierre forzado de RNF-14 | `oe4 Matar` |
| `Cerrar-Juego` | Cierre normal (`WM_CLOSE`); si no responde en 10 s, lo mata. Para el muestreador | `oe4 Cerrar-Juego` |
| `Revisar-Log [sesión]` | Copia `Player.log` y `Player-prev.log` como `.txt` y lista `Exception`, `Error`, `Assert` y las líneas `RNF-04:` | `oe4 Revisar-Log S-PER-rc2` |
| `Residuos [sesión] [-Previo] [-Buscar t1,t2]` | Diferencia contra la foto previa en LocalLow, el primer nivel de `%TEMP%` y la clave del reproductor; con `-Buscar`, rastrea esos textos en nombres y contenido (UTF-8 y UTF-16) | `oe4 Residuos GP3 -Buscar OE4GP3` |
| `Restaurar-Registro [sesión]` | Deja la clave del reproductor como en la foto previa (ver abajo) | `oe4 Restaurar-Registro S-INI-rc2` |
| `Tamano [-Carpeta r]` | Tamaño de la carpeta sin `Datos/` ni `*_DoNotShip` (RNF-06) | `oe4 Tamano` |
| `Medidas [sesión\|-]` | Resume memoria, red y cargas de lo muestreado (RNF-04, RNF-05, RNF-10) | `oe4 Medidas S-N2C-rc2` |
| `Estado` | Qué hay lanzado, ventana, monitores y DPI | `oe4 Estado` |

`Muestrear` es interno: lo lanza `Start-Juego` y muere con el juego.

## Cómo se midieron los RNF

- **RNF-04 (carga < 10 s).** `SceneLoader` escribe en el `Player.log` `RNF-04: «<escena>» cargó en N s`.
  `Esperar-Carga` lee esa línea y la anota en `cargas.csv`. La consolidación (S-RNF, S-RNF-rc2) cuenta **todas** las
  líneas de los `Player.txt` de cada sesión, también las que nadie esperó. `Boot` no pasa por `SceneLoader`: su cota
  es la diferencia entre `Start-Process` y la primera lectura del `Player.log`, que ya trae `MainMenu`. El arnés no mide
  el «primer cuadro no negro».
- **RNF-05 (memoria < 2 GB).** Cada 2 s, el muestreador guarda `WorkingSet64`, `PrivateMemorySize64` y los picos del
  sistema (`PeakWorkingSet64`, `PeakPagedMemorySize64`), con la última escena que registró `SceneLoader`
  (`*_mem.csv`). Se informan la memoria de trabajo y la privada, en MiB y en MB.
- **RNF-06 (paquete < 500 MB).** `Tamano` suma la carpeta del build sin `Datos/` ni `*_DoNotShip` y juzga con
  megabytes decimales (500 000 000 B), la lectura más exigente.
- **RNF-10 (sin datos por red).** Cada 2 s, `Get-NetTCPConnection` y `Get-NetUDPEndpoint` del pid del juego. Cada fila
  se clasifica como `loopback`, `escucha-local` o `EXTERNA`, y una muestra sin ninguna fila se anota `NINGUNA`
  (`*_red.csv`). **Límites:** el muestreo empieza cuando el arnés lee por primera vez el `Player.log`, así que el
  arranque queda sin mirar (de 2,7 a 8,1 s en las sesiones de rc2), y una conexión de menos de 2 s puede caer entre dos
  muestras. Por eso los resultados dicen «no se observó ninguna conexión». RNF-08, el juego con la red deshabilitada,
  no lo mide el arnés: es el guion H4 de `../Hoja-HUM.md`.
- **RNF-11 (sin residuos fuera de `Datos/`).** `Start-Juego` toma la foto previa de LocalLow
  (`…\LocalLow\Universidad Catolica de Colombia\Algoritmia`), del primer nivel de `%TEMP%` y de
  `HKCU\Software\Universidad Catolica de Colombia\Algoritmia`. `Residuos` informa lo nuevo, lo cambiado y lo quitado;
  con `-Buscar` rastrea los nombres de los perfiles borrados en la carpeta portable, LocalLow, lo nuevo de `%TEMP%` y
  el registro. **Guardar su salida** en el registro de pasos: el informe de `residuos\<sesión>.txt` solo lleva el
  rastreo si se pidió en esa llamada.

## Cómo se restaura el registro

El juego crea al primer arranque `HKCU\Software\Universidad Catolica de Colombia\Algoritmia` (las claves de pantalla
del reproductor y, en rc2, los valores de DEF-RC2-01). `Restaurar-Registro <sesión>`, con el juego cerrado, la deja
como estaba en la foto previa de esa sesión:

- si la clave **no existía**, la borra entera y borra también la de la empresa si quedó vacía y tampoco existía;
- si **existía**, borra los valores nuevos y repone los que cambiaron.

Se corre al terminar cada sesión, después de `Residuos`, que necesita ver la clave tal como la dejó el juego.

## Precauciones

- **SHA-256 del exe.** Con `-Sha256` (un hash o un archivo que lo contenga), `Start-Juego` no lanza si el exe no
  coincide. Pero el `.exe` es solo el lanzador de Unity: su huella es la misma en rc1 y rc2. La versión se identifica
  por la **huella del contenido** de la carpeta (`../evidencias/build-rc1.md`, `../evidencias/build-rc2.md`), que se
  comprueba antes y después de las sesiones.
- **Sin clics a ciegas.** Antes de cada clic se mira la pantalla (`Foto` o `-ConFoto`) y se confirma la coordenada
  con la primera captura de cada pantalla; las de `casos.md` son aproximadas. Ninguna acción se encadena sin mirar el
  resultado de la anterior.
- **El Nudge.** Antes de pulsar, el arnés mueve el cursor un píxel y lo devuelve: tras cargar una escena, su
  `EventSystem` no sabe dónde está el puntero hasta que el ratón se mueve, y en el spike se perdieron 12 clics en el
  mismo punto (11 seguidos y uno suelto; `../evidencias/S-SPIKE/S-SPIKE_registro_de_pasos.md`, paso 16). **El Nudge esconde ese riesgo** (un niño con *touchpad* que toca sin deslizar): lo comprueba
  Santiago a mano (`../Hoja-HUM.md`, H13).
- **Un solo manejador.** Un mutex rechaza con salida 3 una segunda llamada que maneje el juego mientras otra sigue en
  curso.
- **`Preparar-Datos` y `Restaurar-Registro` exigen el juego cerrado**: con él abierto, «Salir» reescribiría `Datos/`.
- **Antes de entregar** la carpeta del build: `Preparar-Datos -SinCarpeta` (sin `Datos/`) y retirar
  `Algoritmia_BurstDebugInformation_DoNotShip`.
