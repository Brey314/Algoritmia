# S-PER · rc5 · resumen

- **Versión:** rc5, compilado desde `ca6fd91` (`../../build-rc5.md`). Carpeta `Build\Algoritmia_rc5\`, huella global `ab0c386e87045a4c525548109fa5f9e1905a7336dbd14f21f93b97c8bcb246c3` (253 archivos, `manifiesto-rc5.tsv`). SHA-256 del `.exe` (solo el lanzador) `6CD16EDF…ADAB`.
- **Fecha:** 05/11/2026, 07:17–08:01 (hora local, UTC−5). `Preparar-Datos` 07:17:26; primer `Start-Juego S-PER1-rc5` 07:17:28; último cierre del juego 07:59:56; `Residuos`, `Restaurar-Registro`, `Preparar-Datos -SinCarpeta` y la verificación del manifiesto, hacia las 08:01. Ejecutor EXE con `oe4.ps1`; ventana de 1920×1080 sin marco (`-popupwindow`: el monitor mide 1920×1080).
- **Lanzamientos:** 15 (cinco cierres forzados con `Matar` y diez cierres normales con `Cerrar-Juego`), cada uno con su sesión de arnés y su `Player.txt`: `S-PER1`/`1b` (PF-RNF14-01), `2`/`2b` (-02), `3`/`3b` (-03), `4`/`4b` (-04), `5`/`5b` (-05), `6` (PF-RF02-04), `7`/`7b`/`7c` (PF-RF02-05) y `8` (comprobación extra de DEF-RC5-11, sin veredicto). La sesión de arnés cuya foto previa de residuos vale para todo es `S-PER1-rc5` (la primera).
- **Perfiles:** `OE4_B`, `OE4_E`, `OE4_B3`, `OE4_B2` (semillas), `OE4_Corrupto` (semilla truncada) y dos creados en el juego: `OE4N1b` y `CON`.
- **Casos:** 7 (todos los de la tabla de S-PER). **Veredictos: P 7 · PD 0 · F 0 · B 0 · NA 0 · NE 0.** Detalle en `veredictos.csv`.
- **Pasos ejecutados** sin atajos, cada bloque con su `Preparar-Datos` y su `Revisar-Log`. Salida 0 en todas las llamadas del arnés.

## Cierres forzados: dónde cayó cada `Matar` y qué se comparó
Antes de cada `Matar` se tomó una foto de lo que había en pantalla y una instantánea de `Datos\` (nombre, tamaño, `LastWriteTime`, MD5, atributos y los `.json` copiados a esta carpeta); otra igual 0,8 s después y otra tras relanzar. Se listó `Datos\` con `-Force -Recurse` (también ocultos) buscando temporales.

| Caso | Dónde cayó `Matar` | JSON antes → después | Temporales en `Datos\` |
|---|---|---|---|
| PF-RNF14-01 | `N2_Escena22`, 1.ª línea («La caja rueda sobre los troncos y avanza sin esfuerzo.»), 07:20:29,5 | idéntico (246 B, MD5 `D4569D35…`, LWT 07:20:16,312) | ninguno |
| PF-RNF14-02 | Ensamblaje de mástil y vela, con el mástil ya soltado en su silueta, 07:25:58,3 | idéntico (611 B, MD5 `6EE29D12…`, LWT 07:24:10,747) | ninguno |
| PF-RNF14-03 | `N1_NacimientoDelFuego`, 1.ª línea («Una llama naranja y dorada crece despacio…»), 07:31:42,6 | idéntico (46 B, MD5 `923046EE…`) | ninguno |
| PF-RNF14-04 | `N2_Escena25_Cierre`, 1.ª línea («La familia está en su refugio alrededor del fuego…»), 07:42:44,1 | idéntico (436 B, MD5 `96891FCD…`, LWT 07:42:30,459: la fase {2,3} se escribió 1 s antes de la narrativa, al llegar al refugio) | ninguno |
| PF-RNF14-05 | Taller con tronco A mecanizado y la **pausa abierta**, 07:51:18,5 | idéntico (234 B, MD5 `0ADA2C05…`) | ninguno |

En ningún cierre cayó el aviso de la ayuda: en PF-RNF14-03 se cerró con «Cerrar» antes de recoger las piezas y `Matar` cayó ya en la narrativa de cierre. El aviso salió en las dos entradas a la cueva de ese caso (la primera y el rejuego), porque la fase 1 del Nivel 1 aún no estaba confirmada; no salió con `OE4_B` en PF-RF02-04 (fase confirmada). `Player.log` quedó entero hasta la última línea `RNF-04` en los cinco cierres (sin cierre de proceso, como se espera) y se copió antes del lanzamiento siguiente.

## Cifras: previstas frente a obtenidas
El guion de S-PER no prevé cifras de indicadores: lo que se compara es que el JSON no cambie con el cierre.

| Dato | Obtenido |
|---|---|
| `OE4_B` fase {2,1} (PF-RNF14-01, recorrido sin errores) | 0 intentos · 0 errores corregidos · 0 pasos · 63,48 s |
| `OE4N1b` fase {1,1} tras rejugar (PF-RNF14-03) | 0 · 0 · 3 · 67,27 s; `reachedLevel` 2 |
| `OE4_B3` fase {2,3} (PF-RNF14-04, primera partida) | 0 · 0 · 13 · 257,95 s. En el rejuego (9 bloques) el JSON **conserva** esos valores: no se reemplaza una fase ya confirmada (RF-41, RF-43); `reachedLevel` pasó de 2 a 3 solo al mostrarse el resumen |
| `OE4_B3`, `LastWriteTime` del JSON en el rejuego | 07:46:38 (al terminar el taller): se reescribe con el mismo MD5; 07:49:06 (resumen del N2): solo cambia `reachedLevel` de 2 a 3. No quedó temporal ni copia |

## Lo que se vio de nuevo desde rc3
- **Pausa de cinco botones** en la orilla del N3 y en el taller (Reanudar, Reiniciar, Volver a ver la historia, Volver al menú de niveles, Guardar y salir): con ella abierta `Matar` no dejó ningún rastro.
- **Aviso de la ayuda (INC-158)** en cada entrada a la cueva con la fase 1 sin confirmar; con la fase confirmada no sale.
- **Reanudar el Nivel 2** (ver DEF-RC5-11): las tres narrativas previas al taller o al laberinto son la apertura del bosque (19 líneas: `N2_PuenteI`, `N2_PuenteI_Bosque`, `N2_Escena21_Bosque`).
- **Perfil corrupto:** «No se pudo abrir ese perfil. Avisa a tu profe.», sin `Exception`, el archivo no se toca ni se repara, y no cambia ningún otro perfil.
- **«CON»:** el juego lo acepta (ver DEF-RC5-12).

## Medidas de la sesión (aportan a otros casos; aquí no se juzgan)
- **RNF-04** (`PF-RNF04-01`, lo consolida otra sesión): 102 líneas `RNF-04:` en los 15 `Player.txt`. `Level1_Cave` 0,072–0,089 s · `Level2_Forest` 0,053 · `Level2_Workshop` 0,053–0,070 · `Level2_Maze` 0,070 · `Level3_River` 0,054–0,055 · `LevelSelect` 0,064–0,086 · `LevelSummary` 0,035–0,036 · `MainMenu` 0,085–0,139 · `Narrative` 0,035–0,680 (las cargas con fundido desde un menú o una mecánica tardan 0,37–0,68 s; las encadenadas, 0,035 s). Peor valor 0,680 s. `cargas_oe4.csv` solo trae las 8 que se esperaron con `Esperar-Carga`.
- **RNF-05** (`PF-RNF05-01`): 1.150 muestras en 15 sesiones: memoria de trabajo máx 443,6 MiB (`S-PER4b`, `Narrative`) y privada máx 1.180,3 MiB (`S-PER3b`, `Narrative`). No es la medida de un nivel completo seguido.
- **RNF-10** (`PF-RNF10-01`): 1.150 muestras, todas `NINGUNA`, ninguna conexión externa. Límite del arnés: no mira los primeros ≈ 3 s de cada arranque.
- **RNF-11 / registro:** `Residuos S-PER1-rc5` (`residuos.txt`) halló `Player.log` nuevo en LocalLow (el viejo lo aparta `Start-Juego`) y nueve valores cambiados en `HKCU\Software\Universidad Catolica de Colombia\Algoritmia` (resolución, modo de pantalla, frecuencia numerador y denominador, contador e id de sesión, y los dos id de `unity_connect`); la clave ya existía. Tras `Restaurar-Registro -Sesion S-PER1-rc5`, `Residuos` dice «existía y no cambió» (`residuos_tras_restaurar.txt`). El rastreo de `OE4N1b`, `OE4_Corrupto` y `CON.json` no halló nada fuera de `Datos\`. Sin temporales en la carpeta portable (solo `Newtonsoft.Json.dll`, que es del juego) ni en LocalLow.
- **Registro del juego:** sin `Exception`, `Error` ni `Assert` en los 15 `Player.txt`.

## Datos para casos de otras sesiones (no se juzgan aquí)
- `PF-RF13-06`: el aviso salió en las dos entradas a la cueva de `OE4N1b` y no salió con `OE4_B` (`PF-RNF14-03_1_cueva_aviso.png`, `_2_cueva_aviso.png`, `PF-RF02-04_3_cueva_OE4_B.png`).
- `PF-RF06-01` y `PF-RF05-09`: ninguna narrativa de primera visita ofreció «Omitir» (N1 con `OE4N1b`, N2 con `OE4_B`/`OE4_B2`/`OE4_B3` sin el nivel terminado, N3 con `OE4_E`); con el nivel terminado (`OE4_B3` tras el cierre del N2, `OE4_B` en el N1) sí aparece, encima de «Continuar», y `N2_Escena25_Cierre` no lo ofrece hasta que se muestre su resumen (`PF-RNF14-04_3_e25_sin_omitir.png`).
- `PF-RF07-01`/`-07`: la pausa de cinco botones en `Level3_River` y `Level2_Workshop` (`PF-RNF14-04_2_pausa_desde_orilla.png`, `PF-RNF14-05_1_pausa_abierta.png`).
- `PF-RF45-01`/`-02`: resúmenes de N1 y N2 sin dígitos (`PF-RNF14-03_2_resumen.png`, `PF-RNF14-04_3_resumen.png`).
- `PF-RF03-02`: tras el cierre forzado de `N2_Escena25_Cierre` el N3 ya queda habilitado (derivado de las fases, sin pasar por el resumen).
- `PF-RF02-01` y `PF-RF02-02`: el perfil nuevo `OE4N1b` y `CON` crean `{"name":…,"reachedLevel":1,"phases":[]}` (46 y 43 B).

## Defectos (al final de `../defectos.md`)
- `DEF-RC5-11` · Observación · juego (a confirmar): al reanudar el Nivel 2 se repite la apertura del bosque antes del taller o el laberinto.
- `DEF-RC5-12` · Observación · juego (a confirmar): se acepta el nombre reservado «CON» y se crea `Datos\CON.json`.
- Ninguno funcional, ninguno Mayor ni Bloqueante. Ningún texto del catálogo de S-PER difiere de lo que se ve.

## Avisos del arnés y límites
- **Manifiesto:** coincidió en los 15 `Start-Juego` (todos con salida 0) y coincide al terminar (253 archivos, misma huella). `contaminacion: {}` en `estado.json` del último lanzamiento; en los demás leí solo las últimas líneas de la salida de `Start-Juego`, y Unity y Word no estaban abiertos al empezar ni al terminar (`Get-Process`). Hay procesos de VS Code abiertos en el equipo, que el arnés no cuenta como contaminación.
- **Salidas 3:** ninguna. **Salidas 1 o 2:** ninguna.
- **Un error mío, sin efecto:** en `S-PER1b-rc5` el bucle que cuenta «Continuar» comprobaba mal la línea `Level2_Workshop` del `Player.log` y no se detuvo: tras cargar el taller se pulsó unas 20 veces el punto (0,776; 0,925), que en el taller cae en suelo vacío. La foto siguiente muestra el taller intacto y el JSON no cambió (MD5 igual). Se corrigió el bucle (la expresión `RNF-04: [^\r\n]*Level…`) para el resto.
- **«Omitir» en los rejuegos del N2:** 3 clics para llegar al bosque, 2 para el taller y 1 para el laberinto, además de las pulsaciones de las mecánicas; el catálogo no lo dice y conviene anotarlo.
- **Laberinto:** el tablero es aleatorio; se transcribió por color con una calibración medida sobre la rejilla (x = 216 + 60,2·c; y = 845 − 61,3·r), que difiere de la fórmula de S-N2C; ver `tableros_laberinto.txt`. La primera partida usó 13 bloques y el rejuego 9.
- **Archivos auxiliares fuera del repo:** los scripts de apoyo (envoltorio del arnés, instantáneas de `Datos\`, copia de evidencias, transcripción del tablero) están en el directorio temporal de la sesión; no se tocó `oe4.ps1`.
- **Nombres reservados:** `Datos\CON.json` se leyó y copió siempre con el prefijo `\\?\` por precaución (con `Get-FileHash` y la ruta normal el nombre es un dispositivo); `Get-ChildItem` sí lo lista.
- **Límites:** no se probó un `Matar` con el aviso de la ayuda abierto; no se provocó un cierre en mitad de una escritura (el arnés no puede elegir ese instante, y ninguno de los cinco dejó temporales); el audio no se oyó; el parpadeo, la animación del retrato y las demás cosas de otras sesiones no se midieron aquí. Cada `Matar` terminó unos 1,5 s después de la instantánea de `Datos\` (el tiempo de las dos llamadas), con la escena estática; la comparación de antes y después es idéntica en los cinco, pero no puedo asegurar qué habría pasado si el cierre cayera justo durante una escritura.
