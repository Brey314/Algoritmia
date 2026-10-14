# Resultados de las pruebas funcionales — OE4 (pasada rc3)

> **Pasada cerrada el 10/10/2026 sobre rc3** (HEAD `f49ccc9`, `evidencias/build-rc3.md`). De los 136 casos `PF-*`,
> **126 tienen veredicto** (P 122 · F 4) y **10 esperan a Santiago** (PS). Eficacia **96,8 %** (meta ≥ 90 %); los 45 RF
> de prioridad Alta, **sin ningún F Bloqueante ni Mayor** (100 %). Los cuatro F son Menores. La pasada anterior
> —rc1 y rc2, 01/10/2026— dejó 103 de 127 casos con veredicto (P 89, P parcial 13, F 0, NE 1), nueve defectos corregidos
> en rc2 y DEF-RC2-01 abierto como residuo del motor; vive en el commit `4e78f47` y no se repite aquí. Sus veredictos
> **no se heredan**: el juego cambió en 108 commits, así que cada caso se ejecutó de nuevo sobre rc3.
>
> **Quién ejecutó qué.** Claude ejecutó las sesiones EXE, INSP y SUITE con el arnés `herramientas/oe4.ps1`
> (el ejecutable como caja negra). **S-HUM y la observación con estudiantes (PG-05 y PG-06 del guion) las hace
> Santiago** con `Hoja-HUM.md`; Claude solo transcribe lo que él reporte y no completa nada que no esté en la hoja.

## 0. Metodología (resumen)

- **Instrumento:** `casos.md` (136 casos `PF-*`, 13 sesiones, con guion paso a paso). Reglas de veredicto,
  severidad y ejecutores: `plan.md` §3.2, §4 y §6.
- **Veredictos:** P pasa · PD pasa con la decisión de un INC abierto · F falla (con `DEF-nn`, severidad y tipo) ·
  B bloqueado (con motivo) · NA no aplica. Dos marcas de trabajo, fuera del denominador y que no son veredicto:
  **NE** (no ejecutado, de Claude; ninguno al cierre) y **PS** (pendiente de Santiago: el ejecutor es HUM o falta un
  documento suyo). «P parcial» no cuenta en el KPI.
- **Cómo se consolidó** (`claudeDocs/entregables/OE4/src/veredictos-rc3.csv`, una fila por caso):
  1. Un caso partido entre sesiones (`PF-RF05-04`, `-05`, `-06`, con partes N1, N2 y N3; también `PF-RF05-02`,
     `PF-RF11-02`, `PF-RF04-02`, `PF-RF03-02`) toma el **peor de sus partes**; todas salieron P.
  2. Un caso de S-INSP en B que luego se ejecutó en `S-RNF/veredictos-exe.csv` (RNF-02, 03, 16, 18, 19, 20, 21 y 10)
     toma el veredicto **del ejecutable**; `PF-RNF09-01` (NE en S-INSP) salió P en S-DOC y en el ejecutable, y
     `PF-RNF07-01` (NE en `S-RNF/veredictos.csv`) salió P en el ejecutable.
  3. S-INSP trae además ids que no son casos `PF-*` (`INSP-*`, `CT-10`, `SUITE-T05`): son verificaciones de apoyo
     y no cuentan en el KPI.
  4. Un caso que Claude no puede cerrar en esta pasada (S-HUM, `PF-RNF12-01`, `PF-RNF08-01` por el modo avión, y
     `PF-RNF22-01` y `PF-RNF23-01`, cuya parte INSP está hecha pero falta la confirmación o el soporte de Santiago)
     queda **PS**, no B ni NE.
- **Candidato:** el ejecutable `Build/Algoritmia/` compilado sin etiqueta, identificado por la procedencia de
  `evidencias/build-rc3.md` (HEAD, huella de `git diff HEAD`, SHA-256 y huella de la carpeta). Un resultado sin esa
  versión no vale.
- **Evidencias:** `evidencias/rc3/<sesión>/` (`resumen.md`, `veredictos.csv`, capturas, `Player.log` y CSV de
  memoria y red por lanzamiento). Defectos: `evidencias/rc3/defectos.md` y `evidencias/rc3/S-INSP/defectos.md`.

## 1. Versión que se prueba

Procedencia completa en `evidencias/build-rc3.md`.

| Dato | Valor |
|---|---|
| Candidato | rc3: `Build/Algoritmia/`, compilado el 10/10/2026 (17:24:53 a 17:26:31, `unity build`, 1 min 39 s) |
| Rama y HEAD | `feat/personajes-animados` sobre `f49ccc9cebc246c77975bccec88812391e249716` |
| Huella del árbol (`git diff HEAD`) | vacío (0 bytes) antes y después del build; `git status --short` vacío |
| Build | Unity 6000.5.10f1, StandaloneWindows64, sin development build; 12 escenas con `Boot` primera; 0 errores `error CS` |
| SHA-256 de `Algoritmia.exe` | `6cd16edfbe85284eb69cd2efe1170aa1cffdf0dbdb309c3269e2f8454811adab` (igual que rc1 y rc2: el `.exe` es solo el lanzador) |
| Tamaño de la carpeta sin `Datos/` | 426.633.111 bytes = 406,9 MiB = **426,6 MB** (RNF-06: < 500 MB) |
| Huella del contenido de `Algoritmia_Data` | `b8e30db6821b0107005f9eafba8a628e4a3d0a1bcf36b52a8e7c91875e291446` |
| Huella de la carpeta entregable | `616128366bf1fe15fcc604d14322ec98898c395f5ab435cbb02d589ad776bb9f`, 253 archivos; sin cambios después de todas las sesiones |
| Equipo 1 | AMD Ryzen 5 7600X, 32 GB de RAM, NVIDIA GeForce RTX 5070 Ti de 16 GB, Windows 11 Home Single Language 10.0.26200; ventana de 1920 × 1080 sin marco |
| Condiciones | Editor de Unity cerrado en toda sesión EXE; nada más abierto |

## 2. Sesiones de la pasada rc3

Columna de veredictos: P · PD · F · B · NA · NE · PS.

| Sesión | Tarea | Casos | Fecha y horas | Veredictos | Registro |
|---|---|---|---|---|---|
| S-INI · Inicio, perfiles, créditos y salida | T07 | 14 | 10/10/2026, 17:27–17:33 | 14·0·0·0·0·0·0 | `evidencias/rc3/S-INI/resumen.md` |
| S-N1 · Nivel 1 completo | T08 | 25 | 10/10/2026, 17:33–17:45 | 25·0·0·0·0·0·0 | `evidencias/rc3/S-N1/resumen.md` |
| S-N2A · Bosque | T09 | 10 | 10/10/2026, 17:59–18:17 (un solo proceso con S-N2B y S-N2C) | 10·0·0·0·0·0·0 | `evidencias/rc3/S-N2A/resumen.md` |
| S-N2B · Taller | T10 | 4 | 10/10/2026, 18:05–18:09 | 4·0·0·0·0·0·0 | `evidencias/rc3/S-N2B/resumen.md` |
| S-N2C · Laberinto y cierre del N2 | T11 | 10 | 10/10/2026, 18:10–18:17 | 10·0·0·0·0·0·0 | `evidencias/rc3/S-N2C/resumen.md` |
| S-N3 · Nivel 3 y cierre del juego | T12 | 20 | 10/10/2026, 18:18–18:31 | 20·0·0·0·0·0·0 | `evidencias/rc3/S-N3/resumen.md` |
| S-PAU · Pausa y reinicio en las cinco escenas | T13 | 4 | 10/10/2026, 19:05–19:45 | 4·0·0·0·0·0·0 | `evidencias/rc3/S-PAU/resumen.md` |
| S-OMI · Omisión, rejuego y negativos del N1 | T14 | 8 | 10/10/2026, 19:46–20:05 | 7·0·1·0·0·0·0 | `evidencias/rc3/S-OMI/resumen.md` |
| S-PER · Persistencia, cierre forzado y robustez | T15 | 7 | 10/10/2026, 19:46–20:04 | 7·0·0·0·0·0·0 | `evidencias/rc3/S-PER/resumen.md` |
| S-DOC · Informe docente, borrado y residuos | T16 | 9 | 10/10/2026, 20:05–20:20 | 9·0·0·0·0·0·0 | `evidencias/rc3/S-DOC/resumen.md` |
| S-RNF · Medidas sobre el ejecutable | T20 | 6 | 10/10/2026, 17:23–17:58 (medidas) y el resto de `veredictos-exe.csv` | 5·0·0·0·0·0·1 | `evidencias/rc3/S-RNF/resumen.md y resumen-exe.md` |
| S-INSP · Inspecciones y accesibilidad | T06 / T17 | 13 | 10/10/2026, sin ejecutable | 7·0·3·0·0·0·3 | `evidencias/rc3/S-INSP/resumen.md` |
| S-HUM · Pruebas con personas y en otros equipos (**Santiago**) | T19 | 6 | pendiente | 0·0·0·0·0·0·6 | — |
| **Total** | | **136** | | **122·0·4·0·0·0·10** | |

Observación con estudiantes, PG-05 y PG-06 (`Hoja-HUM.md`, H1 y H2): **la hace Santiago**; pendiente.

### 2.1 Suite automática

| Corrida | Totales | Archivos |
|---|---|---|
| EditMode | 749 pruebas: 748 pasan, 0 fallan, 1 omitida por entorno (`ProfileEraser_INC34_SobreDiscoRealCubreLosDosEscenariosDeAlmacenamiento`) | `evidencias/rc3/S-INSP/suite-10oct-*` |
| PlayMode | 418 pruebas: 417 pasan, 1 falla (`RiverLevel_RNF21_NingunaAnimacionDelNivel3TieneDestellos`: salto de cuadro 0,056 frente a 0,05 con dos Editores compitiendo por CPU; **pasa sola, 1 de 1**) | ídem |

`suite2.ps1`, 10/10/2026, 25,9 min, 1.167 pruebas listadas y ejecutadas (cobertura completa). **Se corrió sobre HEAD `292b108` +
árbol `b2a4d015fe44` (registrada en `23d7e8c`), no sobre el contenido exacto de rc3**: hasta HEAD no cambió código ni pruebas de
`Assets/`, pero sí `Level2_Maze.unity` (reserialización) y se borraron 6 PNG de cara sin referencias (DEF-RC3-I04). Queda como sugerencia
una corrida dirigida de `Maze` en PlayMode antes de cerrar T05.

## 3. Duración de los recorridos completos (PF-RNF13-01)

El caso es de S-HUM y **no se ejecutó en esta pasada** (PS). Las sesiones EXE jugaron cada nivel con semillas y perfiles distintos, no
dos recorridos completos de la pantalla de inicio a los créditos con un perfil nuevo.

| Recorrido | Modo | Duración | Hora | Resultado |
|---|---|---|---|---|
| Arnés n.º 1 | ventana 1920 × 1080 | no ejecutado | — | pendiente |
| Arnés n.º 2 | pantalla completa | no ejecutado | — | pendiente |
| Santiago con cronómetro (H8) | pantalla completa | pendiente de Santiago | — | pendiente |

Referencia: el N1 de S-N1 con perfil nuevo dio 225,84 s de resolución (≈ 225,4 previstos con 41 s de pausa), dentro de lo esperado.

## 4. Medidas sobre el ejecutable (S-RNF)

Los presupuestos duros (CLAUDE.md): carga de escena < 10 s, memoria < 2 GB, paquete < 500 MB.

| Medida | Límite | Resultado en rc3 | Veredicto |
|---|---|---|---|
| 4.1 Cargas de escena (RNF-04) | < 10 s | Peor carga de `SceneLoader` por escena: **0,268 s** (Narrative; media 0,089 s, n 26). MainMenu 0,123 s (n 16), LevelSelect 0,035 s, Credits 0,022 s, LevelSummary 0,013 s, TeacherReport 0,025 s, Level1_Cave 0,038 s, Level2_Forest 0,019 s, Level2_Workshop 0,020 s (0,025 s con las repeticiones de `rnf04-n2n3.csv`), Level2_Maze 0,026 s, Level3_River 0,019 s (0,021 s con 8 mediciones). `Boot` no deja línea: Boot → MainMenu ≈ **3,4 s** (arranque, de `Start-Process` a la línea del registro). Las cuatro jugables de N2 y N3 tienen n = 2 en `rnf04_tabla.csv` (carga inicial y un «Reiniciar», que abre antes una confirmación); con las repeticiones de S-N2A, S-N2R y S-N3 llegan a n = 4 y 8 (`rnf04-n2n3.csv`) | **P** |
| 4.2 Memoria (RNF-05) | < 2 GB | Máximo de memoria privada **1.094 MiB (1,15 GB)** en Level1_Cave (`S-RNF2`); máximo de memoria de trabajo 438 MiB (434 MiB en las cuatro sesiones de S-RNF). 4.213 muestras de memoria en todas las sesiones; ninguna supera 2.048 MiB | **P** |
| 4.3 Tamaño (RNF-06) | < 500 MB | **426.633.111 B = 406,9 MiB = 426,6 MB** sin `Datos/` ni `*_DoNotShip` (esta salida no tiene `DoNotShip`); margen **73,4 MB**. Desglose: `Algoritmia_Data` 340,8 MiB, raíz 52,8, `MonoBleedingEdge` 8,7, `D3D12` 4,5. BuildReport: texturas 273,1 MB (95 % de los activos de usuario) | **P** |
| 4.4 Red (RNF-08, RNF-10) | 0 conexiones | RNF-10: 723 muestras cada 2 s de los cuatro lanzamientos de S-RNF y 4.516 muestras de 39 archivos en todas las sesiones, todas sin conexión TCP ni UDP del proceso; `globalgamemanagers` con 0 apariciones de `unity3d.com`; `UnityConnectSettings` `m_Enabled` 0. RNF-08 (modo avión): lo activa Santiago | RNF-10 **P** · RNF-08 **PS** |
| 4.5 Residuos (RNF-07, RNF-11) | ninguno | Tras borrar OE4N1 y OE4_C no queda rastro del perfil en la carpeta del build, LocalLow, registro ni `Player.log`. Fuera de `Datos/` quedan `Player.log` y valores `unity.*`, `unity_connect.*` y `Screenmanager*` en `HKCU\Software\Universidad Catolica de Colombia\Algoritmia`: el residuo del motor ya registrado como DEF-RC2-01 (decisión D-RC2-01) | **P** (con el residuo del motor) |
| 4.6 Portabilidad en el equipo 1 (RNF-07) | corre desde una ruta con espacio | Copia a `D:\User\Desktop\Algoritmia prueba\` (el escritorio de este equipo está en D:), usuario estándar, SHA-256 igual: arranca, crea `Datos/`, juega hasta `Level1_Cave` (36 clics) sin instalar ni pedir permisos; la copia se borró | **P** |
| 4.7 Exclusión de un nivel (RNF-16) | compila y corre sin él | En `C:\Dev\Algoritmia-B`, `unity test` en los dos sentidos: sin N2, 0 errores de compilación, EditMode 657/663 y PlayMode 288/316; sin N1, EditMode 690/696 y PlayMode 329/354. Los fallos están clasificados en `S-RNF/rnf16/resumen.md`: cinco pruebas de arquitectura que exigen los tres niveles (DEF-RC3-12), las que cargan por nombre una escena del nivel retirado (esperadas) y 15 de River, Wheel y UI que fallan igual en la copia completa por `unity test` en lote (640 × 480) | **P** (con desviación, DEF-RC3-12) |
| 4.8 Segundo equipo (RNF-07, CT-02) — **Santiago** | corre sin instalar | pendiente (H3, H5) | **PS** |

## 5. Veredictos de los casos

La lista completa, caso por caso, es `claudeDocs/entregables/OE4/src/veredictos-rc3.csv` (y el Anexo A del entregable). Aquí, por sesión:

| Sesión | Casos | Veredicto | Evidencia |
|---|---|---|---|
| S-INI | PF-RF01-01, 02, 03 · PF-RF02-01, 02, 03, 06 · PF-RF03-01, 03 · PF-RF08-01, 02 · PF-RF09-01, 02, 03 | 14 P | `rc3/S-INI/veredictos.csv` |
| S-N1 | los 25 de la sesión (PF-RF03-02, 04-01, 05-01, 05-04, 05-05, 05-06, 06-01, 07-05, 07-06, 10-01, 10-04, 11-01, 12-01, 13-01, 14-01 a 21-01, 45-01) | 25 P | `rc3/S-N1/veredictos.csv` |
| S-N2A | PF-RF05-02 · RF10-02 · RF22-01 · RF23-01 · RF24-01 · RF25-01 · RF26-01 · RF13-02 · RF11-02 · RF04-02 | 10 P | `rc3/S-N2A/veredictos.csv` |
| S-N2B | PF-RF27-01 · RF28-01 · RF29-01 · RF13-03 | 4 P | `rc3/S-N2B/veredictos.csv` |
| S-N2C | PF-RF30-01, 02 · RF31-01 · RF32-01 · RF33-01 · RF34-01, 02 · RF13-04 · RF12-02 · RF45-02 | 10 P | `rc3/S-N2C/veredictos.csv` |
| S-N3 | los 20 de la sesión (PF-RF05-03, 10-03, 35-01, 35-02, 36-01 a 45-03, 13-05, 11-03, 12-03, 04-03) | 20 P | `rc3/S-N3/veredictos.csv` |
| S-PAU | PF-RF07-01, 02, 03, 04 | 4 P | `rc3/S-PAU/veredictos.csv` |
| S-OMI | PF-RF06-02, 03, 04 · RF20-02, 03 · RF21-02 · RF45-04, 05 | 7 P · 1 F (PF-RF45-05, DEF-RC3-09) | `rc3/S-OMI/veredictos.csv` |
| S-PER | PF-RNF14-01 a 05 · PF-RF02-04, 05 | 7 P | `rc3/S-PER/veredictos.csv` |
| S-DOC | PF-RF46-01, 02, 03 · RF47-01, 02, 03 · RF09-04 · RNF11-01 · RNF09-01 | 9 P | `rc3/S-DOC/veredictos.csv` |
| S-RNF | PF-RNF04-01, 05-01, 06-01, 07-01, 08-01, 10-01 | 5 P · 1 PS (PF-RNF08-01) | `rc3/S-RNF/veredictos.csv`, `veredictos-exe.csv` |
| S-INSP | PF-RNF01-01, 02-01, 03-01, 15-01, 16-01, 17-01, 18-01, 19-01, 20-01, 21-01, 22-01, 23-01, 12-01 | 7 P · 3 F (RNF-03, 15, 17) · 3 PS (RNF-12, 22, 23) | `rc3/S-INSP/veredictos.csv`, `rc3/S-RNF/veredictos-exe.csv` |
| S-HUM (**Santiago**) | PF-RNF13-01 · PF-SON-01, 02, 03 · PF-RNF07-02 · PF-CT02-01 | 6 PS | — |

### 5.1 Los casos que no son P

| Caso | Sesión | Veredicto | Defecto o motivo |
|---|---|---|---|
| PF-RF45-05 | S-OMI | **F** (Menor) | DEF-RC3-09: un doble clic en «Omitir» del cierre deja en el menú de niveles sin ver el resumen |
| PF-RNF03-01 | S-INSP (ejecutable) | **F** (Menor) | DEF-RC3-10 (instrucción del bosque fuera de su tablilla) y DEF-RC3-05 (iconos del informe sobre el texto) |
| PF-RNF15-01 | S-INSP | **F** (Menor) | DEF-RC3-I01: identificadores en español en tres controladores del N2 |
| PF-RNF17-01 | S-INSP | **F** (Menor) | DEF-RC3-I02: 9 de 122 commits de código con tarjeta |
| PF-RNF08-01 | S-RNF | PS | el modo avión lo activa Santiago (H4) |
| PF-RNF12-01 | S-INSP | PS | formato de consentimiento: documento de Santiago; NA si el OE4 no usa estudiantes |
| PF-RNF22-01 | S-INSP | PS | INSP hecha (344 cadenas sin enlaces ni compras; 9 ilustraciones sin violencia); falta la confirmación de Santiago |
| PF-RNF23-01 | S-INSP | PS | créditos y licencias coinciden con lo esperado; falta el soporte de Santiago (autorización de la Familia Anonaky y de la colaboradora de arte) |
| PF-RNF13-01, PF-SON-01, 02, 03, PF-RNF07-02, PF-CT02-01 | S-HUM | PS | las hace Santiago con `Hoja-HUM.md` (H3, H5, H6 a H8, H12); PF-RNF13-01 incluye además los dos recorridos del arnés, **no ejecutados** |

Con desviación documentada y veredicto P: **PF-RNF16-01** (DEF-RC3-12; se probó en la copia desechable, no en el árbol del proyecto) y **PF-RNF18-01**
(variante: se cambió `MinimumEffectiveStrikes` a 2 y el texto de una línea en un ejecutable de la copia B, con solo 2 `.asset` modificados; no se
probó con el Editor abierto y Play).

## 6. Defectos

Severidad según `plan.md` §6 (Bloqueante, Mayor, Menor); «Observación» es lo que se anota sin que incumpla un RF ni un criterio de
aceptación. Los «propuesta» los confirma Santiago en el triaje (T21). **Ninguno es Bloqueante ni Mayor**; todos siguen abiertos.

**Por severidad: Bloqueante 0 · Mayor 0 · Menor 7 · Observación 9 (16 en total).**

| DEF | Caso | Severidad | Tipo propuesto | Resumen | Estado |
|---|---|---|---|---|---|
| DEF-RC3-01 | PF-RF45-01 | Observación | decisión de Santiago (documento o arte) | El avatar de Algoritm en el resumen de nivel es un círculo con el texto «Algoritm», pese al diseño final | Abierto en rc3 y en rc4; corregido en el código el 14/10/2026 (`86a9898`, posterior a rc4), ver §17 |
| DEF-RC3-02 | PF-RF01-03 | Observación | documento (casos.md) | casos.md describe la portada de otra manera; el criterio se cumple | Abierto |
| DEF-RC3-03 | PF-RF05-05 | Observación | documento o arte | El criterio «cara no tapada» no contempla el gesto en reposo del Niño (excepción `CARA_TAPADA`) | Abierto |
| DEF-RC3-04 | PF-RF14-01 | Observación | documento (casos.md) | Las siete piezas del N1 caen en sitios distintos en cada entrada; las coordenadas del caso no valen | Abierto |
| DEF-RC3-05 | PF-RNF03-01 (visto en PF-RF46-01, P) | **Menor** | código (disposición) o arte | Informe docente: los iconos de las cabeceras se montan sobre el texto; «Volver al menú» parte su texto | Abierto |
| DEF-RC3-06 | PF-RF44-01 | Observación | documento (casos.md) | El paso 21 de S-N3 no se puede reproducir como está escrito: la balsa arranca con la línea 4 | Abierto |
| DEF-RC3-07 | PF-RF30-02 | Observación | documento (admitir ±1) o ninguno | Los colores medidos del laberinto difieren en un nivel de 8 bits de los del documento | Abierto |
| DEF-RC3-08 | PF-RF05-04 | Observación | decisión de Santiago o documento | ALGORITM aparece con la forma del nivel anterior en las secuencias puente | Abierto |
| DEF-RC3-09 | PF-RF45-05 | **Menor** | código (ignorar la entrada ~300 ms tras cargar el resumen) o ajustar el caso | Un doble clic en «Omitir» del cierre deja en el menú de niveles sin ver el resumen (2 de 3 intentos); el progreso no se pierde | Abierto |
| DEF-RC3-10 | PF-RNF03-01 | **Menor** | código/escena (altura de la tablilla) o acortar la línea de `N2_Guia.asset` | Bosque del N2: la instrucción de tres líneas se sale de su tablilla | Abierto |
| DEF-RC3-11 | PF-RF47-02 | Observación | disposición | «Eliminar datos» se parte en dos líneas dentro de su botón; la ruta de respaldo queda a ~8 px de los botones | Abierto |
| DEF-RC3-12 | PF-RNF16-01 | Observación | documento (listar las 5 pruebas como esperadas) o prueba (tolerar un nivel ausente) | Cinco pruebas de EditMode fallan al retirar un nivel y el caso no las lista | Abierto |
| DEF-RC3-I01 | PF-RNF15-01 | **Menor** | estilo de código | Identificadores en español en `ForestSceneController` (20), `WorkshopSceneController` y `MazeSceneController` | Abierto |
| DEF-RC3-I02 | PF-RNF17-01 | **Menor** | proceso | 9 de 122 commits que tocan `Assets/` llevan tarjeta (7,4 %); 4 con el marcador sin resolver y 109 sin tarjeta | Abierto; no corregible sin reescribir la historia |
| DEF-RC3-I03 | INSP-ART-SPRITES (apoyo) | **Menor** (cosmético) | arte | Halo de croma en 9 PNG de caras de la familia (15 a 51 píxeles, α 10 a 25 de 255); rc2 tenía 0 | Abierto |
| DEF-RC3-I04 | SUITE-T05 (apoyo) | **Menor** (trazabilidad) | proceso | La suite no corrió sobre el contenido exacto de rc3 (HEAD `292b108`, no `f49ccc9`) | Abierto |

(DEF-RC3-05 aparece dos veces en `defectos.md`: como hallazgo incidental de S-RNF y confirmado en S-DOC; aquí se cuenta una.)

**Heredado de rc2, re-verificado en rc3:** DEF-RC2-01 (el reproductor escribe `unity_connect.*` en el registro; residuo del motor, decisión
D-RC2-01) **persiste** igual (S-INI, `residuos.txt`). El riesgo residual R-RC2-A (ventana interna de `File.Replace`) y las observaciones
menores de la decisión D-OBS no se volvieron a verificar en rc3; su texto está en `4e78f47`.

**Defectos del arnés** (herramienta, no juego): `oe4.ps1 Restaurar-Registro` sale con 2 al reponer un valor cambiado
(«The type of the value object did not match the specified RegistryValueKind», línea 1502): no repone los valores que cambiaron, solo borra
los nuevos. Un aviso de NVIDIA Superposición cae unos segundos sobre la esquina superior derecha tras el arranque.

## 7. KPI

Fórmulas de `plan.md` §1, a la vista. Se publican **aunque no lleguen a la meta**; aquí la superan.

| KPI | Meta | Fórmula | Valor en rc3 |
|---|---|---|---|
| 1 · Eficacia de pruebas funcionales | ≥ 90 % | `(P + PD) / (P + PD + F) × 100` sobre los casos `PF-*`; B, NA, NE y PS fuera del cómputo | **96,8 %** = 122 / (122 + 0 + 4) |
| 2 · Cumplimiento de requerimientos | 100 % de los 45 RF de prioridad Alta | un RF Alta está implementado si **ninguno** de sus casos termina en F Bloqueante o Mayor | **100 %** (45 de 45): ningún F Bloqueante ni Mayor. El único F que toca un RF Alta es PF-RF45-05 (RF-45), Menor |

Eficacia por etiqueta (el KPI nombra botones, sonidos y saltos de nivel):

| Etiqueta | P | PD | F | Eficacia |
|---|---|---|---|---|
| NAV | 16 | 0 | 1 | 94,1 % |
| BOT | 28 | 0 | 0 | 100,0 % |
| SON | 0 | 0 | 0 | pendiente de Santiago (3 PS) |
| RETO | 37 | 0 | 0 | 100,0 % |
| DAT | 30 | 0 | 0 | 100,0 % |
| RNF | 11 | 0 | 3 | 78,6 % |
| **Global** | 122 | 0 | 4 | **96,8 %** |

NAV (94,1 %) y BOT (100 %) superan la meta de 90 %. **SON queda sin calcular**: sus tres casos (`PF-SON-01..03`) son de oído y los hace Santiago
(H6, H7 y H12); sin ellos el KPI no se puede declarar cerrado para «sonidos». La etiqueta RNF (78,6 %) queda bajo 90 % por tres F Menores
(PF-RNF03-01, 15 y 17) sobre 14 casos con veredicto; no es una etiqueta que el KPI nombre.

Si hay correcciones tras la pasada, se reporta también la **pasada final** (rc4 o siguiente), con los veredictos de rc3 sin tocar.

## 8. Matriz de trazabilidad (T25)

Una fila por requerimiento (47 RF y 23 RNF), más CT-02 y el sonido. «Pruebas automáticas» cuenta **métodos** de `Assets/Tests` cuyo nombre
cita el requerimiento (RF: `rf_matrix.py --worktree`, Tabla A.1 de `S-INSP/ct10-matriz-rf.md`; RNF: el mismo patrón de nombre, `_RNFnn_`); todas
pasaron en la corrida del 10/10/2026, salvo `RiverLevel_RNF21_…`, que pasó sola. **CT-10: ningún RF ni RNF queda sin caso** (los 136 casos cubren
RF-01..47 y RNF-01..23) y **ningún RF queda sin prueba automática** (47 de 47; mínimo 1 en RF-21 y RF-27). Sin prueba automática que lo nombre:
RNF-08, RNF-12, RNF-17 y RNF-22, que se verifican por inspección o por documento.

| Requerimiento | Casos `PF-*` | Veredictos en rc3 | Pruebas automáticas que lo nombran (métodos) | DEF / INC |
|---|---|---|---|---|
| RF-01 | RF01-01, RF01-02, RF01-03 | 3 P | 16 | DEF-RC3-02 |
| RF-02 | RF02-01, RF02-02, RF02-03, RF02-06, RF02-04, RF02-05 | 6 P | 12 | — |
| RF-03 | RF03-01, RF03-03, RF03-02 | 3 P | 16 | — |
| RF-04 | RF04-01, RF04-02, RF04-03 | 3 P | 7 | — |
| RF-05 | RF05-01, RF05-04, RF05-05, RF05-06, RF05-02, RF05-03 | 6 P | 77 | DEF-RC3-03, -08 |
| RF-06 | RF06-01, RF06-02, RF06-03, RF06-04 | 4 P | 5 | — |
| RF-07 | RF07-05, RF07-06, RF07-01, RF07-02, RF07-03, RF07-04 | 6 P | 6 | — |
| RF-08 | RF08-01, RF08-02 | 2 P | 4 | — |
| RF-09 | RF09-01, RF09-02, RF09-03, RF09-04 | 4 P | 4 | — |
| RF-10 | RF10-01, RF10-04, RF10-02, RF10-03 | 4 P | 2 | — |
| RF-11 | RF11-01, RF11-02, RF11-03 | 3 P | 2 | — |
| RF-12 | RF12-01, RF12-02, RF12-03 | 3 P | 3 | DEF-RC3-01 |
| RF-13 | RF13-01, RF13-02, RF13-03, RF13-04, RF13-05 | 5 P | 12 | — |
| RF-14 | RF14-01 | 1 P | 11 | DEF-RC3-04 |
| RF-15 | RF15-01 | 1 P | 7 | — |
| RF-16 | RF16-01 | 1 P | 14 | — |
| RF-17 | RF17-01 | 1 P | 5 | DEF-RC3-01 |
| RF-18 | RF18-01 | 1 P | 3 | — |
| RF-19 | RF19-01, RF19-02 | 2 P | 7 | — |
| RF-20 | RF20-01, RF20-04, RF20-02, RF20-03 | 4 P | 12 | — |
| RF-21 | RF21-01, RF21-02 | 2 P | 1 | — |
| RF-22 | RF22-01 | 1 P | 14 | — |
| RF-23 | RF23-01 | 1 P | 10 | — |
| RF-24 | RF24-01 | 1 P | 7 | — |
| RF-25 | RF25-01 | 1 P | 6 | — |
| RF-26 | RF26-01 | 1 P | 12 | — |
| RF-27 | RF27-01 | 1 P | 1 | — |
| RF-28 | RF28-01 | 1 P | 4 | — |
| RF-29 | RF29-01 | 1 P | 7 | — |
| RF-30 | RF30-01, RF30-02 | 2 P | 6 | DEF-RC3-07 |
| RF-31 | RF31-01 | 1 P | 8 | — |
| RF-32 | RF32-01 | 1 P | 8 | — |
| RF-33 | RF33-01 | 1 P | 2 | — |
| RF-34 | RF34-01, RF34-02 | 2 P | 11 | — |
| RF-35 | RF35-01, RF35-02 | 2 P | 4 | — |
| RF-36 | RF36-01 | 1 P | 4 | — |
| RF-37 | RF37-01 | 1 P | 4 | — |
| RF-38 | RF38-01 | 1 P | 4 | — |
| RF-39 | RF39-01 | 1 P | 3 | — |
| RF-40 | RF40-01, RF40-02 | 2 P | 6 | — |
| RF-41 | RF41-01 | 1 P | 5 | — |
| RF-42 | RF42-01, RF42-02 | 2 P | 10 | — |
| RF-43 | RF43-01 | 1 P | 4 | — |
| RF-44 | RF44-01 | 1 P | 12 | DEF-RC3-06 |
| RF-45 | RF45-01, RF45-02, RF45-03, RF45-04, RF45-05 | 4 P · 1 F | 22 | DEF-RC3-09, -01 |
| RF-46 | RF46-01, RF46-02, RF46-03 | 3 P | 9 | DEF-RC3-05 |
| RF-47 | RF47-01, RF47-02, RF47-03 | 3 P | 7 | DEF-RC3-11 |
| RNF-01 | RNF01-01 | 1 P | 7 | — |
| RNF-02 | RNF02-01 | 1 P | 20 | — |
| RNF-03 | RNF03-01 | 1 F | 26 | DEF-RC3-10, -05 |
| RNF-04 | RNF04-01 | 1 P | 3 | — |
| RNF-05 | RNF05-01 | 1 P | 1 | — |
| RNF-06 | RNF06-01 | 1 P | 4 | — |
| RNF-07 | RNF07-01, RNF07-02 | 1 P · 1 PS | 2 | — |
| RNF-08 | RNF08-01 | 1 PS | — | — |
| RNF-09 | RNF09-01 | 1 P | 5 | — |
| RNF-10 | RNF10-01 | 1 P | 11 | — |
| RNF-11 | RNF11-01 | 1 P | 4 | — |
| RNF-12 | RNF12-01 | 1 PS | — | — |
| RNF-13 | RNF13-01 | 1 PS | 14 | DEF-RC3-I04 |
| RNF-14 | RNF14-01, RNF14-02, RNF14-03, RNF14-04, RNF14-05 | 5 P | 20 | — |
| RNF-15 | RNF15-01 | 1 F | 2 | DEF-RC3-I01 |
| RNF-16 | RNF16-01 | 1 P | 8 | DEF-RC3-12 |
| RNF-17 | RNF17-01 | 1 F | — | DEF-RC3-I02 |
| RNF-18 | RNF18-01 | 1 P | 9 | — |
| RNF-19 | RNF19-01 | 1 P | 12 | — |
| RNF-20 | RNF20-01 | 1 P | 7 | — |
| RNF-21 | RNF21-01 | 1 P | 20 | — |
| RNF-22 | RNF22-01 | 1 PS | — | — |
| RNF-23 | RNF23-01 | 1 PS | 25 | DEF-RC3-I03 |
| CT-02 | CT02-01 | 1 PS | — | — |
| Sonido (KPI) | SON-01, SON-02, SON-03 | 3 PS | — | — |

## 9. Oportunidades de mejora y trabajos futuros (T27)

Salen de los defectos abiertos, las observaciones de las sesiones y los límites de la pasada. Falta incorporar OE3 §10.3–10.4 y los puntos
PS-01..05 de la dirección de sonido, y lo que traiga la evaluación con estudiantes (PG-05, PG-06, de Santiago).

**Interfaz**
- DEF-RC3-05: dar a la cabecera del informe docente una altura propia para los iconos y ensanchar «Volver al menú» para que no parta su texto.
- DEF-RC3-10: acortar la instrucción del bosque en `N2_Guia.asset` o subir la altura de su tablilla, para que las tres líneas quepan.
- DEF-RC3-11: ensanchar el botón «Eliminar datos» (o acortar el rótulo) y dar más aire a la ruta de respaldo del diálogo de «Salir».
- DEF-RC3-01: sustituir el círculo con el texto «Algoritm» del resumen de nivel por su arte final (corregido en el código el 14/10/2026, ver §17).

**Mecánica**
- DEF-RC3-09: ignorar la entrada unos ~300 ms tras cargar el resumen, para que un doble clic en «Omitir» no pulse «Volver al menú de niveles».
- DEF-RC3-04: si la evaluación con estudiantes necesita partidas comparables, fijar la distribución de las siete piezas del N1 (hoy aleatoria).
- DEF-RC3-08: confirmar con Santiago que la forma de Algoritm en las secuencias puente (llama en `N2_PuenteI`, disco en `N3_PuenteII`) es intencional.
- DEF-RC3-03: decidir si el gesto del Niño en reposo se acepta como excepción o se corrige en el arte.

**Código y trazabilidad**
- DEF-RC3-I01: renombrar a inglés los identificadores de `ForestSceneController`, `WorkshopSceneController` y `MazeSceneController` (p. ej. `EnPantalla` → `ToScreenRect`).
- DEF-RC3-I02: exigir la tarjeta del Kanban en el mensaje de commit (plantilla o hook), porque corregirla en lo ya hecho exigiría reescribir la historia.
- DEF-RC3-I03: limpiar el halo de croma de las 9 caras con `arte_check.py halo --apply`, con copia de respaldo y decisión de Santiago.
- DEF-RC3-I04: correr `Maze` en PlayMode (o la suite entera) sobre el HEAD que se entregue.
- DEF-RC3-12: que las pruebas de arquitectura toleren un nivel ausente, para que RNF-16 pueda decir «cero fallos de acoplamiento»; y fijar la resolución de `unity test` en lote (15 pruebas de River, Wheel y UI fallan a 640 × 480 y pasan por `editor.ps1` a 1920 × 1080).
- RNF-10 (observación de S-INSP, sin id): el paquete trae `DirectML.dll`, `D3D12/` y módulos de red de Unity que nada del juego llama; valorar quitarlos (bajaría también el peso) o dejarlos documentados.

**Documento de casos**
- DEF-RC3-02: describir la portada como es (panel ámbar a la izquierda, bosque con los cinco personajes a la derecha).
- DEF-RC3-04: decir que el N1 se juega leyendo la pantalla y no prometer coordenadas de las piezas.
- DEF-RC3-06: en el paso 21 de S-N3, pedir los clics hasta la línea 4 y fotografiar entonces.
- DEF-RC3-07: admitir ±1 en los colores medidos por píxel.
- DEF-RC3-08: decir qué forma de Algoritm lleva cada secuencia puente.
- DEF-RC3-12: listar las cinco pruebas de arquitectura como esperadas en PF-RNF16-01.
- PF-RF21-02 (observaciones de S-OMI, sin id): la narrativa de cierre tras «Soplar» entra a ~1,3 s, no a los ~3,5 s que cita el caso, y el máximo de luz tras el 4.º golpe efectivo no se pudo medir; reformular el ⚠.

## 10. Límites de la pasada

- **RNF-18** se probó **por ejecutable aparte, en la copia B** (`C:\Dev\Algoritmia-B`), no con el Editor abierto y Play: el arnés no pudo conducir su Game View, y «sin recompilación» no es observable con `unity build`.
- **RNF-16** se probó con `unity test` en copias sin un nivel. Cinco pruebas de EditMode fallan en los dos sentidos porque exigen los tres assemblies y todos los RF (DEF-RC3-12), y **15 de River, Wheel y UI fallan también en la copia completa** por `unity test` en lote (640 × 480; 15 de 15 reproducidas sin quitar nada), mientras que pasan con la suite normal a 1920 × 1080.
- **El arnés `Restaurar-Registro` sale con 2** y no repone los valores de registro cambiados (solo borra los nuevos); los valores son de Unity (sesión) y de pantalla.
- **La primera captura llega con ~1,3 s de retraso**: no hay cuadro de la ignición, del primer cuadro del acercamiento de la cueva ni de la pausa a mitad del recorrido del laberinto (se tomó a ~0,3 s de pulsar «Ejecutar»).
- La línea `RNF-04` de `SceneLoader` mide la carga, no el primer cuadro visible; el muestreo de red empieza ~2–8 s después del arranque y no ve conexiones de menos de 2 s.
- **Un solo equipo** en las mediciones de RNF-04, RNF-05 y RNF-06; el equipo 2 (H3, H5) y el modo avión (H4) son de Santiago.
- La suite no corrió sobre el contenido exacto de rc3 (DEF-RC3-I04).
- Los mensajes de las fotos se leyeron a 0,5–0,9 s de cada acción: el límite de 1 s de RF-11 se juzga por ese margen, no por un cronómetro. El sonido no lo mide el arnés.
- Los dos recorridos del arnés de PF-RNF13-01 no se ejecutaron.

## 11. Pendientes de Santiago

| Pendiente | Casos o puntos | Qué falta |
|---|---|---|
| S-HUM, comprobaciones H1 a H13 (`Hoja-HUM.md`) | PF-RNF13-01, PF-SON-01..03, PF-RNF07-02, PF-CT02-01 (6 casos) | H3 equipo 2, H5 equipo sin GPU dedicada, H6 y H7 oído (escena final, hundimiento), H8 Golden Path con cronómetro, H9 a H11 vista, H12 oído pieza por pieza, H13 mano; y H1 y H2 con estudiantes. La hoja todavía cita rc2 y su huella de carpeta: se actualiza a rc3 antes de entregarla |
| PG-05 y PG-06 del guion | H1, H2 | Observar a estudiantes jugando (cambio de controles N1 → N2; valores del Nivel 1). Si no se hace, pasan a trabajos futuros |
| Consentimiento informado (RNF-12) | PF-RNF12-01 | Completar `Consentimiento-RNF12.md` (campos «por completar») y anexarlo en blanco; o declarar NA si no hay estudiantes |
| Modo avión (RNF-08) | PF-RNF08-01, H4 | Activarlo; Claude corre el recorrido corto |
| Confirmación de contenido (RNF-22) | PF-RNF22-01 | Confirmar que los textos y las ilustraciones no tienen violencia, publicidad, compras ni enlaces |
| Autorizaciones (RNF-23) | PF-RNF23-01 | Soporte escrito de la Familia Anonaky y de la colaboradora de arte |
| Triaje de los 16 defectos | T21 | Corregir en OE4, registrar un INC o dejar como trabajo futuro, con fecha |
| Etiqueta de rc3 | T01 | Decidir si se etiqueta; Claude solo la pone con su visto bueno |
| Recorridos del arnés de PF-RNF13-01 | — | Decidir si Claude ejecuta los dos (ventana y pantalla completa) sobre rc3 antes de que Santiago haga el suyo |

## 12. Nota del 13/10/2026 · sesión con estudiantes y hoja HUM

Nota añadida; no corrige las secciones anteriores, que conservan el corte del 10/10/2026 sobre rc3.

**Sesión.** El 13/10/2026, en el Colegio El Libertador IED, sede A, con 4 estudiantes de grado quinto. Un
observador miró cómo jugaban; los estudiantes pensaron en voz alta y comunicaron sus inquietudes al
observador. Autorización institucional: «Formato de autorización institucional» del 17/09/2026, firmado por el
rector Richard Maldonado, para grado quinto de primaria dentro de la investigación doctoral de la docente
Bibiana Patricia Rey Barrote, contacto del equipo en el colegio, con la condición del consentimiento
informado de padres o acudientes (el número de documento y las firmas no se publican). Consentimientos: 4
firmados, custodiados. Formato en blanco: `evidencias/consentimiento/Formato-consentimiento.md`. Constancia
de borrado: positiva; se borraron los perfiles de la sesión y `Datos\` quedó vacía en los equipos.
El grado queda registrado en INC-151.

| Id | Resultado |
|---|---|
| H1 (PG-05) | Aprobado |
| H2 (PG-06) | Aprobado |
| H3 | Aprobado |
| H4 | Aprobado |
| H5 | Aprobado (equipos abajo) |
| H6 | Aprobado: el sonido se escucha |
| H7 | Aprobado |
| H8 | Entre 40 y 45 min; la meta es de 20 a 40 min, así que supera el tope en hasta 5 min. Sin causa registrada |
| H9 | Aprobado, sin comentarios: todo se comprendió |
| H10 | Aprobado, sin comentarios: todo se comprendió |
| H11 | Aprobado: la balsa se hundió, se comprendió por qué, se arregló y finalmente cruzó |
| H12 | Aprobado, sin comentarios: todo se comprendió |
| H13 | No se realizó en la sesión; pasa a trabajo futuro (el clic tras cargar una escena, la papelera del laberinto y la octava fila del informe) |

Equipos de H5, ambos sin conexión a internet: (A) AMD A10-9600P, Radeon R5 con 66 MB de VRAM, 4 GB de RAM,
512 GB de disco; (B) Intel Celeron B830, Intel HD Graphics de 32 MB, 4 GB de RAM, 512 GB de disco. La sala
tenía internet disponible, de 20 Mb/s de bajada y 7 de subida, y no se usó.

**Pendiente de consolidar.** Los veredictos de los casos con ejecutor HUM (`PF-RNF13-01`, `PF-SON-01` a
`PF-SON-03`, `PF-RNF07-02`, `PF-CT02-01`, `PF-RNF08-01`) no están incorporados a las tablas 3.1 y 3.3 ni al
Anexo A; esas cifras (96,8 %, 10 casos en PS) siguen siendo las del 10/10/2026. El entregable
(`claudeDocs/entregables/OE4/src/05-estudiantes.md`) y el capítulo 9 ya recogen la sesión. Siguen
pendientes `PF-RNF22-01` y `PF-RNF23-01`.

## 13. Nota del 13/10/2026 (noche) · veredictos de la hoja HUM consolidados

Nota añadida; no corrige las secciones anteriores ni la §12. Con el commit `e9a26fc` los veredictos de la sesión manual del 13/10/2026 sobre rc3 entraron en `claudeDocs/entregables/OE4/src/veredictos-rc3.csv` y en `03-resultados.md`. El recuento del CSV es 128 P, 4 F y 4 PS.

**Pasaron a P seis casos:**
- `PF-RNF07-02` (H3, segundo equipo);
- `PF-RNF08-01` (H4, sin conexión a internet);
- `PF-CT02-01` (H5, equipo sin tarjeta gráfica dedicada);
- `PF-SON-01` a `PF-SON-03` (H12, con H6 y H7 para `PF-SON-03`).

**Siguen en PS cuatro casos:**
- `PF-RNF13-01`: H8 registró de 40 a 45 min, pero el caso pide tres recorridos y los dos del arnés sobre rc3 no se corrieron.
- `PF-RNF12-01`, `PF-RNF22-01` y `PF-RNF23-01`.

**H13 no se hizo.** `PF-RNF02-01`, `PF-RF34-01` y `PF-RF46-01` conservan su P de rc3.

| Cifra | 10/10/2026 | 13/10/2026 |
|---|---|---|
| Eficacia (KPI 1) | 96,8 % = 122 / (122 + 4), 10 PS | 97,0 % = 128 / (128 + 4), 4 PS |
| S-RNF (P · F · PS) | 5 · 0 · 1 | 6 · 0 · 0 |
| S-HUM (P · F · PS) | 0 · 0 · 6 | 5 · 0 · 1 |
| Etiqueta RNF | 78,6 % (11 de 14) | 82,4 % (14 de 17) |
| Etiqueta SON | sin veredicto | 100 % (3 de 3), juicio de oído |

Si los 4 PS fallaran, la eficacia quedaría en 94,1 % (128 de 136). Ningún dato de esta nota viene de PlayMode ni de rc4, que siguen en curso en local.

## 14. Nota del 13/10/2026 (cierre) · rc4 y la suite

Nota añadida; no corrige las secciones anteriores. La §13 decía que PlayMode y rc4 seguían en curso. Con el commit `56897fd` quedó así:

- **rc4:** 426 930 226 bytes = 426,9 MB, con 73,1 MB de margen frente a los 500 MB de RNF-06; compilado desde `0839352` (`evidencias/build-rc4.md`). rc3 pesó 426,6 MB. Las evidencias de rc4 no incluyen mediciones de carga ni de memoria: las cifras de RNF-04 y RNF-05 de este documento son las de rc3.
- **Suite completa** (`suite2.ps1`, 24,8 min; `evidencias/suites/rc4-previa-al-boton-atras/`): EditMode 785 de 786, con una omitida por entorno (`ProfileEraser_INC34_…`), y PlayMode 427 de 427; 1.213 pruebas listadas y las 1.213 ejecutadas. Corrió antes del último cambio de disposición del diálogo (INC-160). Lo que ese cambio afecta se volvió a correr aparte y pasó: `NarrativeScene_INC160` (3 pruebas), RNF-03 (9 pruebas) y las demás que lista `build-rc4.md`.
- **Arreglo de `HelpButtonIntro`** (INC-158): al descargarse la escena con el aviso abierto, Unity lanzaba «Destroying object multiple times»; ahora no. Prueba de regresión: `FireLevel_INC158_UnaEscenaQueSeDescargaConElAvisoAbiertoDevuelveElTiempoSinErrores`.
- **Disposición final del diálogo** (INC-160): «Omitir» encima de «Continuar» (260 × 76 px) y «Atrás» en la fila de «Continuar», a su izquierda (200 × 76 px). Se descartó «Atrás» a la izquierda del texto: el cuadro mide 216 px y la fila de botones, el nombre y dos líneas piden unos 240 px (`evidencias/rc4/Inc160_medidas_texto.txt`).
- **Capturas** (`claudeDocs/tasks/Personajes/capturas/2026-10-13-inc154/`): el abrazo de la línea 5 se lee; el de la línea 12 se lee a medias, porque Papá no rodea a la Niña; el sueño de perfil no tapa el fuego; en la caminata de la escena 1.1 Papá queda parcialmente detrás de Algoritm al llegar. El abrazo de la línea 12 y el solapamiento con Algoritm siguen sin corregir (responsable: equipo de desarrollo).

Los estudiantes jugaron rc3. rc4 no se probó con estudiantes, y los veredictos `PF-*` de este documento siguen siendo los de rc3.

## 15. Nota del 13/10/2026 (cierre) · `PF-RNF12-01` en P y triaje de los 16 defectos

Nota añadida; no corrige las secciones anteriores ni la §13, cuyo recuento (128 P, 4 F, 4 PS) era el del momento.

- **`PF-RNF12-01` pasa de PS a P** en `claudeDocs/entregables/OE4/src/veredictos-rc3.csv`: el formato de consentimiento informado consta en el Anexo F y hay 4 consentimientos firmados (sesión del 13/10/2026).
- **Recuento del CSV:** 129 P, 4 F y 3 PS. Siguen en PS `PF-RNF13-01`, `PF-RNF22-01` y `PF-RNF23-01`.

| Cifra | §13 | Esta nota |
|---|---|---|
| Eficacia (KPI 1) | 97,0 % = 128 / (128 + 4), 4 PS | 97,0 % = 129 / (129 + 4), 3 PS |
| S-INSP (P · F · PS) | 7 · 3 · 3 (70,0 %) | 8 · 3 · 2 (72,7 %) |
| Etiqueta RNF | 82,4 % (14 de 17) | 83,3 % (15 de 18) |
| Si los PS fallaran | 94,1 % (128 de 136) | 94,9 % (129 de 136) |

- **Triaje de los 16 defectos de rc3 (decisión del usuario):** *documento* DEF-RC3-02, -04, -06, -07 y -12, ya aplicado en `casos.md`; *código*, para rc4, DEF-RC3-05, -09, -10 y -11, cuya corrección confirmará el commit que la recoja; *trabajo futuro* DEF-RC3-01, -03, -08, -I01, -I02, -I03 y -I04. La tabla de defectos de §6 conserva su estado «Abierto» de la pasada.
- Ningún dato de esta nota viene de PlayMode ni de rc4.

## 16. Nota del 13/10/2026 (noche) · rc4 recompilado desde `0405eba`

Nota añadida; no corrige las secciones anteriores, que citan el rc4 compilado desde `0839352` (§14) y dicen que las cifras de RNF-04 y RNF-05 son las de rc3. Ese rc4 se sustituyó; el vigente es el de `evidencias/build-rc4.md`.

- **Procedencia:** compilado de 23:02 a 23:03 desde `0405eba`, con `git diff HEAD` vacío sobre `Assets/`, `Packages/` y `ProjectSettings/` y sin archivos sin seguimiento en ellas. Sin etiqueta. Incluye las correcciones de código del triaje de la §15: DEF-RC3-05 (cabeceras del informe docente), DEF-RC3-09 (`InputGuard`, 0,3 s sin clics tras cada carga de escena), DEF-RC3-10 (tablilla del bosque más ancha) y DEF-RC3-11 (textos de botones del informe docente y de los diálogos de borrar y salir), y los retoques de narrativa de INC-154 (Papá en el abrazo de la 1.4.4; nadie tras Algoritm en la 1.1).
- **RNF-06:** 426 932 086 bytes = 407,2 MiB = 426,9 MB (252 archivos), con 73,1 MB de margen frente a los 500 MB. rc3 pesó 426,6 MB.
- **RNF-04:** el peor valor fue 0,403 s, en la primera `Narrative`; 46 cargas en 4 lanzamientos, a 1920 × 1080. `Boot` a `MainMenu`, 2,76 s (cota desde `Start-Process`). Son una o dos cargas por escena (17 en `Narrative`), menos de las tres de `PF-RNF04-01`: la tabla de rc3 sigue siendo la evidencia de ese caso y la de rc4 la confirma.
- **RNF-05:** pico de memoria privada de 1 083,3 MiB (1,06 GiB) en `Level1_Cave` y de trabajo de 434,6 MiB en `Level3_River`, sobre 373 muestras; el límite es 2 048 MiB. rc3 dio 1 094 MiB y 438 MiB. **RNF-10:** 373 filas de red sin ninguna conexión TCP ni UDP a una dirección que no fuera de loopback.
- **Suite completa** sobre `0405eba` (`suite2.ps1`, 24,4 min; `evidencias/suites/rc4-0405eba/`): EditMode 793 = 792 aprobadas y 1 omitida por entorno, PlayMode 435 de 435, 0 fallos; 1 228 listadas y 1 228 ejecutadas. Sustituye a la de la §14 (785 de 786 y 427 de 427).
- **Veredictos `PF-*`:** siguen siendo los de rc3. rc4 no tuvo pasada de casos ni se probó con estudiantes, así que los cuatro F no se han vuelto a ejecutar; sus defectos se dan por corregidos en código, con pruebas nuevas en verde (`InputGuard`, `DEFRC3`, `TeacherReport`, `EraseDialog`, `MainMenu`, `ForestScene`).
- **Pendiente (equipo de desarrollo):** revisar las capturas `Personajes_N1_*_v2.png` del abrazo de la línea 12 y de la escena 1.1; la §14 las daba por sin corregir y el retoque de `0405eba` no tiene revisión visual registrada. Etiquetar rc4: lo decide Santiago Benavides Rey.

## 17. Nota del 14/10/2026 · DEF-RC3-01 corregido en el código, después de rc4; hallazgo DEF-RC4-01

Nota añadida; no corrige las secciones anteriores. La §15 dejó DEF-RC3-01 en trabajo futuro y la tabla de la §6 conserva su estado de la pasada.

- **DEF-RC3-01 (Observación, `PF-RF45-01`) corregido en el código en `86a9898` (14/10/2026), posterior a rc4.** El avatar del resumen de nivel ya no es un círculo con el texto «Algoritm»: es el prefab del guía en reposo, con su diseño final y la cara que parpadea, en la forma del nivel terminado (Nivel 1, llama; Nivel 2, disco de madera; Nivel 3, gota), y lo declara el asset de mensajes de cada nivel.
- **Pruebas:** `LevelSummary_DEFRC301_*`, 3 casos en EditMode y 3 en PlayMode, en verde. EditMode completa, 795 de 796 (1 omitida por entorno); `tests-play LevelSummary`, 8 de 8. Capturas: `evidencias/rc4/DEF-RC3-01_N1.png`, `_N2.png` y `_N3.png`.
- **rc4 no se recompiló.** rc4 está compilado desde `0405eba`, así que el ejecutable rc4 todavía muestra el círculo. La corrección entra en el siguiente candidato. El veredicto de `PF-RF45-01` sigue siendo el de rc3: el caso no se repitió sobre ningún ejecutable con la corrección.
- **DEF-RC4-01 (Observación, corregida el 14/10/2026 en `ca6fd91`, entra en rc5).** En el resumen del Nivel 3 a 1920 × 1080 la tarjeta medía más de 1080 px y se cortaban sus bordes superior e inferior (captura `evidencias/rc4/DEF-RC3-01_N3.png`). Se redujo el espaciado vertical de la tarjeta (48 → 32 px) y se ensanchó el texto de la habilidad (710 → 900 px): la tarjeta mide ahora 927 a 989 px en los tres niveles (capturas del Editor en `evidencias/rc5/`). rc5 (`build-rc5.md`, 426,9 MB) se compiló sin pasada de casos ni medidas de RNF-04/05.
