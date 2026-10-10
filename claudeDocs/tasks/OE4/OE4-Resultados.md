# Resultados de las pruebas funcionales — OE4 (pasada rc3)

> **Reiniciado el 10/10/2026.** Este documento es la plantilla vacía de la pasada sobre **rc3**, el candidato
> que se compilará desde el estado actual de la rama. La pasada anterior —rc1 y rc2, 01/10/2026— dejó 103 de 127
> casos con veredicto (P 89, P parcial 13, F 0, NE 1), nueve defectos corregidos en rc2 y DEF-RC2-01 abierto como
> residuo del motor; vive en el historial de git, en el commit `4e78f47`
> (`claudeDocs/tasks/OE4/OE4-Resultados.md`), y no se repite aquí. Sus veredictos **no se heredan**: el juego cambió
> en 108 commits desde entonces (personajes de perfil y con expresiones, retrato animado en el diálogo,
> Algoritm final, laberinto sin tinte, halo de las fogatas, portada nueva), así que cada caso se ejecuta de nuevo
> sobre rc3 y cada veredicto lleva la versión en que se obtuvo.
>
> **Quién ejecuta qué.** Claude ejecuta las sesiones EXE, INSP y SUITE con el arnés `herramientas/oe4.ps1`
> (el ejecutable como caja negra). **S-HUM y la observación con estudiantes (PG-05 y PG-06 del guion) las hace
> Santiago** con `Hoja-HUM.md`; Claude solo transcribe lo que él reporte y no completa nada que no esté en la
> hoja.

## 0. Metodología (resumen)

- **Instrumento:** `casos.md` (136 casos `PF-*`, 13 sesiones, con guion paso a paso). Reglas de veredicto,
  severidad y ejecutores: `plan.md` §3.2, §4 y §6.
- **Veredictos:** P pasa · PD pasa con la decisión de un INC abierto · F falla (con `DEF-nn`, severidad y tipo) ·
  B bloqueado (con motivo) · NE no ejecutado · NA no aplica. «P parcial» no cuenta en el KPI.
- **Candidato:** el ejecutable `Build/Algoritmia/` compilado sin etiqueta, identificado por la procedencia de
  `evidencias/build-rc3.md` (HEAD, huella de `git diff HEAD`, archivos sin seguimiento, SHA-256 y huella del
  contenido de la carpeta). Un resultado sin esa versión no vale.
- **Evidencias:** `evidencias/<sesión>/` (registro de pasos `*_registro_de_pasos.md`, capturas, JSON finales).
  La suite automática va en `evidencias/suites/`.

## 1. Versión que se prueba

Procedencia completa en `evidencias/build-rc3.md` (pendiente: se escribe al compilar).

| Dato | Valor |
|---|---|
| Candidato | rc3: `Build/Algoritmia/` — pendiente |
| Rama y HEAD | `feat/personajes-animados` sobre — pendiente |
| Huella del árbol (`git diff --binary --no-textconv --no-ext-diff HEAD \| sha256sum`) | pendiente |
| Build | pendiente (Unity 6000.5.10f1, StandaloneWindows64, Mono, sin development build) |
| SHA-256 de `Algoritmia.exe` | pendiente |
| Tamaño de la carpeta sin `Datos/` | pendiente (RNF-06: < 500 MB) |
| Huella del contenido | pendiente |
| Equipo 1 | AMD Ryzen 5 7600X, 32 GB de RAM, NVIDIA GeForce RTX 5070 Ti de 16 GB, Windows 11 Home Single Language 10.0.26200; monitor de 1920 × 1080 al 125 % (a confirmar el día de la pasada) |
| Condiciones | Editor de Unity cerrado en toda sesión EXE; nada más abierto |

## 2. Sesiones de la pasada rc3

| Sesión | Tarea | Casos | Fecha y horas | Veredictos (P · PD · F · NE · B) | Registro de pasos |
|---|---|---|---|---|---|
| S-INI · Inicio, perfiles, créditos y salida | T07 | 14 | pendiente | pendiente | pendiente |
| S-N1 · Nivel 1 completo | T08 | 25 | pendiente | pendiente | pendiente |
| S-N2A · Bosque | T09 | 10 | pendiente | pendiente | pendiente |
| S-N2B · Taller | T10 | 4 | pendiente | pendiente | pendiente |
| S-N2C · Laberinto y cierre del N2 | T11 | 10 | pendiente | pendiente | pendiente |
| S-N3 · Nivel 3 y cierre del juego | T12 | 20 | pendiente | pendiente | pendiente |
| S-PAU · Pausa y reinicio en las cinco escenas | T13 | 4 | pendiente | pendiente | pendiente |
| S-OMI · Omisión, rejuego y negativos del N1 | T14 | 8 | pendiente | pendiente | pendiente |
| S-PER · Persistencia, cierre forzado y robustez | T15 | 7 | pendiente | pendiente | pendiente |
| S-DOC · Informe docente, borrado y residuos | T16 | 9 | pendiente | pendiente | pendiente |
| S-RNF · Medidas sobre el ejecutable | T20 | 6 | pendiente | pendiente | pendiente |
| S-INSP · Inspecciones y accesibilidad | T06 / T17 | 13 | pendiente | pendiente | pendiente |
| S-HUM · Pruebas con personas y en otros equipos (**Santiago**) | T19 | 6 | pendiente | pendiente | pendiente |
| **Total** | | **136** | | | |

Observación con estudiantes, PG-05 y PG-06 (`Hoja-HUM.md`, H1 y H2): **la hace Santiago**; pendiente.

### 2.1 Suite automática sobre rc3

| Corrida | Totales | Archivos |
|---|---|---|
| EditMode | pendiente | `evidencias/suites/rc3/` |
| PlayMode | pendiente | ídem |

Se corre con `herramientas/suite2.ps1` según `NORMA-PRUEBAS.md`, sobre el mismo árbol que se compila.

## 3. Duración de los recorridos completos (PF-RNF13-01)

| Recorrido | Modo | Duración | Hora | Resultado |
|---|---|---|---|---|
| Arnés n.º 1 | ventana 1920 × 1080 | pendiente | pendiente | pendiente |
| Arnés n.º 2 | pantalla completa | pendiente | pendiente | pendiente |
| Santiago con cronómetro (H8) | pendiente | pendiente | pendiente | pendiente |

## 4. Medidas sobre el ejecutable (S-RNF)

Los presupuestos duros (CLAUDE.md): carga de escena < 10 s, memoria < 2 GB, paquete < 500 MB.

| Medida | Límite | Resultado en rc3 | Veredicto |
|---|---|---|---|
| 4.1 Cargas de escena de las 12 escenas (RNF-04) | < 10 s | pendiente | pendiente |
| 4.2 Memoria, máximos de `WorkingSet64` y `PrivateMemorySize64` (RNF-05) | < 2 GB | pendiente | pendiente |
| 4.3 Tamaño de la carpeta sin `Datos/` ni `*_DoNotShip` (RNF-06) | < 500 MB | pendiente | pendiente |
| 4.4 Red: ninguna conexión (RNF-08, RNF-10) | 0 conexiones | pendiente | pendiente |
| 4.5 Residuos tras borrar un perfil y tras cerrar (RNF-07, RNF-11) | ninguno | pendiente | pendiente |
| 4.6 Portabilidad en el equipo 1 (RNF-07) | corre desde una ruta con espacio | pendiente | pendiente |
| 4.7 Exclusión de un nivel, en los dos sentidos (RNF-16) | compila y corre sin él | pendiente | pendiente |
| 4.8 Segundo equipo (RNF-07, CT-02) — **Santiago** | corre sin instalar | pendiente | pendiente |

## 5. Veredictos de los casos

Una fila por caso `PF-*` de `casos.md`, agrupadas por sesión. Columnas: caso · veredicto · versión · evidencia ·
`DEF-nn` si F. Se llena a medida que se cierra cada sesión; hoy están **todos pendientes**.

| Sesión | Casos | Veredicto | Evidencia |
|---|---|---|---|
| S-INI | PF-RF01-01, 02, 03 · PF-RF02-01, 02, 03, 06 · PF-RF03-01, 03 · PF-RF08-01, 02 · PF-RF09-01, 02, 03 | pendiente | pendiente |
| S-N1 | PF-RF05-01, 04, 05, 06 · PF-RF06-01 · PF-RF07-05, 06 · PF-RF10-01, 04 · PF-RF11-01 · PF-RF13-01 · PF-RF14-01 · PF-RF15-01 · PF-RF16-01 · PF-RF17-01 · PF-RF18-01 · PF-RF19-01, 02 · PF-RF20-01, 04 · PF-RF21-01 · PF-RF12-01 · PF-RF45-01 · PF-RF04-01 · PF-RF03-02 | pendiente | pendiente |
| S-N2A | PF-RF05-02 · PF-RF10-02 · PF-RF22-01 · PF-RF23-01 · PF-RF24-01 · PF-RF25-01 · PF-RF26-01 · PF-RF13-02 · PF-RF11-02 · PF-RF04-02 | pendiente | pendiente |
| S-N2B | PF-RF27-01 · PF-RF28-01 · PF-RF29-01 · PF-RF13-03 | pendiente | pendiente |
| S-N2C | PF-RF30-01, 02 · PF-RF31-01 · PF-RF32-01 · PF-RF33-01 · PF-RF34-01, 02 · PF-RF13-04 · PF-RF12-02 · PF-RF45-02 | pendiente | pendiente |
| S-N3 | PF-RF05-03 · PF-RF10-03 · PF-RF35-01, 02 · PF-RF36-01 · PF-RF37-01 · PF-RF38-01 · PF-RF39-01 · PF-RF40-01, 02 · PF-RF41-01 · PF-RF42-01, 02 · PF-RF43-01 · PF-RF44-01 · PF-RF13-05 · PF-RF11-03 · PF-RF12-03 · PF-RF45-03 · PF-RF04-03 | pendiente | pendiente |
| S-PAU | PF-RF07-01, 02, 03, 04 | pendiente | pendiente |
| S-OMI | PF-RF06-02, 03, 04 · PF-RF20-02, 03 · PF-RF21-02 · PF-RF45-04, 05 | pendiente | pendiente |
| S-PER | PF-RNF14-01 … 05 · PF-RF02-04, 05 | pendiente | pendiente |
| S-DOC | PF-RF46-01, 02, 03 · PF-RF47-01, 02, 03 · PF-RF09-04 · PF-RNF11-01 · PF-RNF09-01 | pendiente | pendiente |
| S-RNF | PF-RNF04-01 · PF-RNF05-01 · PF-RNF06-01 · PF-RNF07-01 · PF-RNF08-01 · PF-RNF10-01 | pendiente | pendiente |
| S-INSP | PF-RNF01-01 · PF-RNF02-01 · PF-RNF03-01 · PF-RNF15-01 … PF-RNF23-01 · PF-RNF12-01 | pendiente | pendiente |
| S-HUM (**Santiago**) | PF-RNF13-01 · PF-SON-01, 02, 03 · PF-RNF07-02 · PF-CT02-01 | pendiente | pendiente |

## 6. Defectos

Severidad según `plan.md` §6 (Bloqueante, Mayor, Menor). Los «propuesta» los confirma Santiago en el triaje (T21).
Ningún defecto ha sido hallado todavía en rc3.

| DEF | Caso | Severidad | Tipo propuesto | Resumen | Estado |
|---|---|---|---|---|---|
| — | — | — | — | Sin defectos: la pasada no ha empezado | — |

**Heredado de rc2, por re-verificar en rc3 (no son defectos de rc3 hasta que se observen):** DEF-RC2-01 (el
reproductor escribe `unity_connect.*` en el registro; residuo del motor, decisión D-RC2-01), el riesgo residual
R-RC2-A (ventana interna de `File.Replace`) y las observaciones menores de la decisión D-OBS. Su texto está en
`4e78f47`, §8.7 a §8.10 de la versión anterior de este documento.

**Defectos del arnés** (herramienta, no juego): ninguno registrado en esta pasada.

## 7. KPI

Fórmulas de `plan.md` §1, a la vista. Se calculan al cerrar las sesiones y se publican **aunque no lleguen a la
meta**.

| KPI | Meta | Fórmula | Valor en rc3 |
|---|---|---|---|
| 1 · Eficacia de pruebas funcionales | ≥ 90 % | `(P + PD) / (P + PD + F) × 100` sobre los casos `PF-*`; «P parcial», NE, B y NA fuera del cómputo | pendiente |
| 2 · Cumplimiento de requerimientos | 100 % de los 45 RF de prioridad Alta | un RF Alta está implementado si **ninguno** de sus casos termina en F Bloqueante o Mayor | pendiente |

Eficacia por etiqueta (el KPI nombra botones, sonidos y saltos de nivel):

| Etiqueta | P | PD | F | Eficacia |
|---|---|---|---|---|
| NAV | pendiente | | | |
| BOT | pendiente | | | |
| SON | pendiente (Santiago) | | | |
| RETO | pendiente | | | |
| DAT | pendiente | | | |
| RNF | pendiente | | | |
| **Global** | pendiente | | | |

Si hay correcciones tras la pasada, se reporta también la **pasada final** (rc4 o siguiente), con los veredictos
de rc3 sin tocar.

## 8. Matriz de trazabilidad (T25)

Una fila por requerimiento (47 RF y 23 RNF). Las pruebas automáticas que lo nombran se cuentan con
`claudeDocs/entregables/OE3/tools/rf_matrix.py --worktree`.

| Requerimiento | Casos `PF-*` | Veredictos en rc3 | Pruebas automáticas (nombran / pasan) | DEF / INC |
|---|---|---|---|---|
| RF-01 … RF-47 | ver `casos.md`, índice de cobertura | pendiente | pendiente | pendiente |
| RNF-01 … RNF-23 | ver `casos.md`, índice de cobertura | pendiente | pendiente | pendiente |

## 9. Oportunidades de mejora y trabajos futuros (T27)

Salen de los defectos no corregidos, los P parcial, las observaciones de las sesiones, OE3 §10.3–10.4, los puntos
PS-01..05 de la dirección de sonido y la evaluación con estudiantes (PG-05, PG-06, de Santiago). Pendiente.
