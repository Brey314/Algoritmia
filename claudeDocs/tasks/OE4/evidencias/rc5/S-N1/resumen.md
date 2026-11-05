# S-N1 · rc5 · resumen

- **Versión:** rc5, compilado desde `ca6fd91` (`../../build-rc5.md`). Carpeta `Build\Algoritmia_rc5\`, huella global `ab0c386e87045a4c525548109fa5f9e1905a7336dbd14f21f93b97c8bcb246c3` (253 archivos, `manifiesto-rc5.tsv`). SHA-256 del `.exe` (solo el lanzador) `6CD16EDF…ADAB`.
- **Fecha:** 04/11/2026, 21:32–22:01 (hora local). `Preparar-Datos OE4_B` 21:32:39; primer `Start-Juego S-N1-rc5` 21:32:48 (cierre normal ≈ 21:57); segundo `Start-Juego S-N1b-rc5` 21:57:33 (cierre normal ≈ 22:00). Ejecutor EXE con `oe4.ps1`, ventana de 1920×1080 con marco (cliente en 1,31; monitor principal de 2560×1440).
- **Perfiles:** `OE4_B` (semilla, para que la lista no estuviera vacía) y `OE4N1`, creado en el juego (paso 1). `Datos/OE4N1.json` final: `{"name":"OE4N1","reachedLevel":2,"phases":[{"level":1,"phase":1,"attempts":5,"correctedErrors":1,"stepsUsed":3,"resolutionSeconds":320.50946044921877}]}` (`S-N1_OE4N1.json`).
- **Casos:** 33 filas (los 30 de la tabla de S-N1 más `PF-RF02-01`, `PF-RF07-01` y `PF-RF07-02`, que el guion alimenta pero no están en su tabla; van marcadas «fila adicional, solo mi parte»). **Veredictos: P 33 · PD 0 · F 0 · B 0 · NA 0 · NE 0.** Detalle en `veredictos.csv`.
- **Casos que se alimentan también de otra sesión y se juzgaron solo en mi parte:** `PF-RF02-01` (S-INI juzga los rechazos), `PF-RF05-04` y `PF-RF05-05` (S-N2A/B/C y S-N3), `PF-RF05-06` (S-N2A), `PF-RF06-01` (S-N2A/B/C y S-N3), `PF-RF07-01` y `-02` (S-PAU), `PF-RF10-04` (S-N2A y S-N3), `PF-RF13-06` (S-PAU `Level1_Cave`).
- **Pasos 1–26 ejecutados**, sin atajos, en el primer lanzamiento. El segundo lanzamiento (rejuego del perfil `OE4N1`, sin terminar el nivel) se hizo para dos comprobaciones que el primero no había podido hacer: el retrato animado de la tarjeta y los instantes de desaparición de los tres rayos efectivos.

## Cifras: previstas frente a obtenidas
| | Previsto | Obtenido |
|---|---|---|
| I · C · P | 5 · 1 · 3 | 5 · 1 · 3 |
| `reachedLevel` | 2 | 2 |
| `resolutionSeconds` | (T1 − T0 − 30 s) ± 5 s, con T0 al pulsar «Cerrar» del aviso | 320,509 s |

El tiempo, con detalle en `PF-RF04-01_24_tiempos.txt` (horas UTC de `acciones_oe4.tsv`): T0 = 02:40:59,32; «Soplar» 02:46:51,23; T1 (inicio del fundido, ~3,5 s después) ≈ 02:46:54,7; el aviso estuvo en pantalla 28,0 s y **la pausa duró 34,9 s reales, no 30**, porque cada llamada del arnés suma ~1–5 s a la espera pedida. Con la pausa real, (T1 − T0) − pausa = 320,5 s y el JSON dice 320,509 (diferencia +0,01 s); con los 30 s nominales saldrían 325,4 s (diferencia −4,9 s, en el borde de ±5). Si el juego no descontara la pausa habría marcado ≈ 355 s, y sin descontar tampoco el aviso, 383,4 s: descuenta ambos. En rc3 el mismo recorrido dio 225,84 s con 41 s de pausa.

## Lo nuevo de rc5 que se vio
- **«Atrás» (INC-160, `PF-RF05-09`):** ausente en la 1.ª línea de Apertura y de AparicionGuia; a la izquierda de «Continuar» y sin tocar el texto desde la 2.ª; vuelve sin transición. Los cuatro quedan de pie y de frente haciendo sus gestos de reposo —los mismos que al cargar la línea desde cero—, sin repetir la caminata de entrada; al avanzar, la 2.ª línea sale con la pose ya fija (DEF-RC5-02: «quietos» en el catálogo).
- **Caminata (INC-153, `PF-RF05-08`):** en la 8.ª línea de AparicionGuia (índice 7, «línea 7» del catálogo, DEF-RC5-01) los cuatro caminan de perfil a la izquierda y se desplazan a la vista; al llegar nadie queda tras Algoritm.
- **Abrazos y sueño de perfil (INC-154, `PF-RF05-07`):** Mamá en Hallazgo 13 y los abrazos y el sueño de Nacimiento, de perfil, mirando a quien abrazan; duermen boca arriba sin tapar la fogata.
- **Aviso de la ayuda (INC-158, `PF-RF13-06`):** al entrar a la cueva con el perfil sin la fase 1 confirmada, la escena se oscurece (zona de luz al 70 %, botón de pausa al 60 %) y solo el botón de ayuda queda encima, con su texto exacto y «Cerrar»; bloquea el arrastre y el clic. Con la fase ya confirmada no sale (tres entradas en el rejuego).
- **Luz del inicio (INC-152, `PF-RF21-03`):** las tres narrativas y la cueva se leen; lo negro es solo «Solo oscuridad.» (7.ª línea de Apertura).
- **Pausa de cinco botones (INC-159)** y **«Omitir» encima de «Continuar»** (en el rejuego): vistos.

## Medidas de la sesión (aportan a otros casos; aquí no se juzgan)
- **RNF-04** (`PF-RNF04-01`): 21 líneas `RNF-04:` en los dos `Player.txt` — Level1_Cave 0,039 · 0,032 · 0,035 s; LevelSummary 0,013 s; Narrative 0,013–0,244 s (la más lenta, la primera tras «Continuar» del perfil); MainMenu 0,122 y 0,124 s; LevelSelect 0,024–0,030 s. Peor valor 0,244 s. `cargas_oe4.csv` trae las que se esperaron con `Esperar-Carga`.
- **RNF-05** (`PF-RNF05-01`): primer lanzamiento, 631 muestras: memoria de trabajo máx 475 MiB (`Narrative`), privada máx 1.223 MiB (`Level1_Cave`); segundo, 68 muestras: 429 y 1.170 MiB. Frente a rc3 (privada máx 1.084 MiB) sube ≈ 13 %; sin juicio. Es la medida de un nivel completo, no de todo el juego.
- **RNF-10** (`PF-RNF10-01`): 631 + 68 muestras sin ninguna conexión del proceso. Límite del arnés: no mira los primeros ≈ 3 s de cada arranque.
- **RNF-11 / registro:** `Residuos` halló `Player.log` en LocalLow (el viejo lo aparta `Start-Juego`) y ocho valores cambiados en `HKCU\Software\Universidad Catolica de Colombia\Algoritmia` (resolución, pantalla completa, `unity.player_session_*`, `unity_connect.*`); la clave ya existía. Tras `Restaurar-Registro S-N1-rc5` dice «existía y no cambió» (`residuos_tras_restaurar.txt`). El segundo lanzamiento tomó su propia foto previa con otro nombre de sesión para no perder la original.
- **Registro del juego:** sin `Exception`, `Error` ni `Assert` en los dos lanzamientos.

## Datos para casos de otras sesiones
- `PF-RNF21-01`: ráfaga de 6 s de la llama en la escena de cierre (`PF-RNF21-01_22_rafaga_cierre_llama.csv`): luminancia global 63,63–64,51, salto máximo entre cuadros 0,24 (0,37 %), halo con oscilación sinusoidal de periodo 2,00 s; la cueva con llama dura solo ~3,5 s antes del fundido, así que la ráfaga de 6 s de la llama se tomó en el cierre (`…_21_rafaga_cueva_llama.csv` cubre la cueva). Ver `PF-RNF21-01_S-N1_analisis.txt` y DEF-RC5-03.
- `PF-RNF03-01`: una captura a pantalla completa por fase (`PF-RNF03-01_S-N1_*.png`: aviso, reunir, panel, pausa, llama, cuatro narrativas, resumen). A simple vista, en cada una hay una sola tarea activa y las líneas de tres renglones caben en el cuadro; no hice el barrido por parada de lo nombrado bajo el cuadro (lo juzga S-INSP): en Hallazgo 15–17 la Niña, Mamá o el Niño quedan parcialmente cortados por el borde derecho sin que el texto los nombre. El aviso de la ayuda tapa parte de la tablilla a propósito.
- `PF-RNF19-01`: «Soplar» con candado, nivel 2 y 3 bloqueados antes de terminar el N1 y tras terminarlo, en color y en grises (`PF-RNF19-01_S-N1_*.png`, `*_gris.png`).
- `PF-RNF20-01`: tablilla del N1 en su estado más oscuro, cuadro del diálogo con y sin retrato, resumen, y contraste muestreado de 10,52:1 a 16,54:1 (`PF-RNF20-01_S-N1_contraste_datos.txt`).
- `PF-RNF04-01` y `PF-RNF05-01`: las cifras de arriba.

## Defectos
Ninguno funcional. Tres observaciones en `../defectos.md`: `DEF-RC5-01` (numeración de líneas del catálogo, documento), `DEF-RC5-02` (descripciones de pose del catálogo, documento) y `DEF-RC5-03` (arco dentado rojo oscuro en la llama al nacer, juego, a confirmar).

## Avisos del arnés y límites
- `Start-Juego` no avisó de contaminación (Unity y Word cerrados). Salidas: todas 0 salvo una **2** de `Esperar-Carga Level1_Cave` que yo pedí mal (en el rejuego «Omitir» lleva a la siguiente narrativa, no a la cueva; no es un fallo del juego ni del arnés). Ninguna salida 3. El manifiesto coincidió antes de los dos lanzamientos y coincide al terminar.
- En una captura (`PF-RF05-05_3_hz14_15_16_17_hoja.png`, Hallazgo 14.ª línea) se ve una ventana de consola de Windows en la esquina superior izquierda: no es del juego, no tapaba ningún punto de clic y desapareció en la foto siguiente. Origen desconocido (¿otra sesión del equipo?).
- **Primer cuadro del acercamiento:** a +0,1 s del último arrastre la cámara aún no se había movido y a ~+1,9 s ya había terminado; capturé el estado previo, el final y, en el rejuego, un cuadro a mitad del acercamiento (+0,45 s), no el primero exacto.
- **Duración de los rayos:** la tomé por intervalos (visible a +0,1/+0,3/+0,5 s en el primer lanzamiento; ausente a +0,35/+0,55/+0,75 s en el segundo), no por medición directa.
- **Retrato animado:** solo se midió boca y parpadeo de la Niña en Hallazgo 1; en los demás hablantes, retrato y expresión vistos en foto fija.
- **Fundido y corte seco:** el fundido a negro entre escenas se vio en una transición (luminancia 0,003); el corte seco frente→perfil, el espejado del frente y la escala 0,95–1,05 del halo no se midieron.
- La captura de la pausa abierta se hizo a +1 s y a +34 s con 0 píxeles de diferencia: congelada. Las demás escenas con pausa las juzga S-PAU.
- Un indicador verde «G-SYNC» de NVIDIA aparece fijo en la esquina superior derecha de todas las capturas (entorno, no del juego).
- Las evidencias pesan ≈ 61 MB (rc3: 27 MB) porque se guardaron fotos a resolución completa de cada fase para las otras sesiones; las duplicadas se eliminaron y se referencian desde `veredictos.csv`.

## Para la siguiente sesión
- Para «esperar 30 s con la pausa abierta» no sumar `Start-Sleep` y fotos: medir con `acciones.tsv` desde el clic de pausa hasta el de «Reanudar» y esperar en una sola llamada (24 s de espera dieron 34,9 s reales).
- Los pasos con foto «a +x s» son fiables solo con `-ConFoto -FotoMs` en el mismo clic; una llamada de `Foto` aparte llega ≥ 1,3 s tarde.
- Las coordenadas de las piezas de la cueva cambian en cada entrada; tomarlas de la foto de cada entrada, como en rc3.
- En el rejuego, «Omitir» salta una sola secuencia (hacen falta dos clics para llegar de Apertura a la cueva).
