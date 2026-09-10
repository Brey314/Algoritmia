# Fase 3 — Nivel fuego: lo construido y sus resultados

**Slice 1 · Golden Path temprano** · Estado: **T12 implementada y verde** — corte de este
documento: 10 de septiembre de 2026.
Verificación vigente: **EditMode 72/72** (suite Fire 9/9), 0 fallos.
Plan técnico: [`plan.md`](plan.md) · Tablero: [`todo.md`](todo.md) · Contrato: `claudeDocs/SPEC.md`
Fase anterior: [`Fase-2-Resultados.md`](Fase-2-Resultados.md)

La Fase 3 es el módulo `nivel-fuego`: el nivel jugable completo (panel de encendido, iteración,
depuración, convergencia, resolución, indicadores). Este documento se irá llenando tarea a tarea;
el código de la fase **no está completo**.

| Tarea | Qué entrega | Modo de prueba | Estado |
|---|---|---|---|
| T12 | `StrikePosition`, `FireLevelConfig`, `StrikeOutcome`, `FireAttempt` — lógica pura del nivel | EditMode | ✅ terminada |
| T13 | `FireFeedbackLog` y los ocho mensajes del guion §4.3.4 | EditMode | pendiente |
| T14 | Panel de encendido y escena `Level1_Cave` | PlayMode | pendiente |
| T15 | Convergencia: «Soplar» → nacimiento del fuego | PlayMode + VV | pendiente |
| T16 | Menú de pausa | EditMode + PlayMode | pendiente |
| T17 | Emisión de los cuatro indicadores del N1 | EditMode | pendiente |
| T18 | Resumen de fin de nivel y cierre reflexivo | EditMode + PlayMode | pendiente |
| T19 | Iluminación progresiva del escenario (RF-21, Baja) | VV | pendiente |

---

## 1. T12 — el corazón del Nivel 1, C# plano

El assembly `Game.Levels.Fire` existía como `.asmdef` vacío desde T01; T12 le pone el primer
código. Cuatro archivos en `Assets/Game/Scripts/Runtime/Levels/Fire/`:

- **`StrikePosition`** — enum de tres distancias del deslizante (guion §4.3.2): `Far`, `Near`,
  `VeryClose`. «Muy cerca» es la única efectiva por defecto.
- **`FireLevelConfig`** — ScriptableObject con los cuatro parámetros ajustables jugando
  (`AvailablePositions`, `EffectivePosition`, `MinimumEffectiveStrikes`, `AttemptsBeforeHint`),
  `[field: SerializeField]` + `[Tooltip]` con el texto del guion, valor por defecto = el propuesto
  (3 / Muy cerca / 3 / 3). Fuera del código para que retocarlos —PG-06 sigue abierto— no cueste
  recompilar (CT-05, RNF-18). **No se crea el `.asset`**: es de T14; los tests usan
  `ScriptableObject.CreateInstance`.
- **`StrikeOutcome`** — `readonly struct` que devuelve un golpe: `Effective`, `Position`,
  `EffectiveStrikes`. Fábricas `SparksDied(position)` / `SparkLanded(effectiveStrikes)` con las
  firmas literales del `SPEC.md` §Estilo. Lo consumirá T13 para elegir el mensaje del log.
- **`FireAttempt`** — el código del `SPEC.md` §Estilo (líneas 334–365) tal cual, con
  `ConsecutiveFailures` expuesto como propiedad pública de solo lectura (patrón de `HintPolicy`).
  Lleva los dos contadores del guion §4.3.3/§4.3.5:
  - Golpe desde una posición no efectiva → `ConsecutiveFailures++`, devuelve `SparksDied`; **no
    toca los golpes efectivos** (RF-16).
  - Golpe desde la posición efectiva → `EffectiveStrikes++`, `ConsecutiveFailures = 0`, devuelve
    `SparkLanded`.
  - `CanBlow` deriva **solo** de `EffectiveStrikes`, que solo crece → una vez habilitado el soplo
    no vuelve a deshabilitarse ni tras un fallo posterior (INC-32, guion §4.3.6). Comentario «por
    qué no» pedagógico en el código.
  - Sin tope de intentos, sin excepción, sin contador de derrota (RF-18, CP-02).
  - **No guarda la posición del deslizante**: es estado de UI (T14). El modelo solo cambia al
    llamar `Strike` (RF-15).

Sin `.asmdef` nuevos ni modificados: `Game.Levels.Fire` ya referenciaba `Game.Core` y
`Game.Scaffolding`, y `Game.Levels.Fire.Tests` ya estaba listo desde T01.

---

## 2. Verificación — declarada

### 2.1 EditMode

**Suite Fire: 9/9, 0 fallos** (`mcp__rider__run_unity_tests`, EditMode, 10/09/2026).
**EditMode total: 72/72, 0 fallos** — sin regresión sobre las demás suites.

| Prueba | Requisito |
|---|---|
| `FireLevelConfig_CT05_ExponeLosCuatroParametrosDelGuion` | CT-05, guion §4.3.2 |
| `FireAttempt_RF16_GolpeNoEfectivoProduceConsecuenciaYNoSuma` (`Far`, `Near`) | RF-16 |
| `FireAttempt_RF16_GolpeEfectivoSumaYReiniciaLosFallos` | RF-16, guion §4.3.3 |
| `FireAttempt_RF15_CambiarPosicionNoAlteraElEstado` | RF-15 |
| `FireAttempt_RF18_AceptaIntentosIlimitados` | RF-18, CP-02 |
| `FireAttempt_INC32_SoplarSeHabilitaAlAlcanzarElMinimo` | INC-32 |
| `FireAttempt_RF19_SoplarNoSeDeshabilitaTrasFalloPosterior` | RF-19, INC-32 |
| `FireAttempt_RNF18_ElUmbralDePistaSaleDeLaConfiguracion` | RNF-18, RF-19 |

### 2.2 Flujo test-first

Esqueleto compilable → pruebas en rojo → implementación → refactor.

- **RED: 8 fail / 2 pass.** Las dos verdes contra el esqueleto son correctas y están anotadas en
  el plan: `FireLevelConfig_CT05` (SO de datos puros, no hay lógica de Step 3) y
  `FireAttempt_RF15` (invariante: el estado inicial ya es el correcto y solo `Strike` lo cambia).
  Antes del refuerzo eran 6 fail / 4 pass: se endurecieron dos pruebas —`RF18` con
  `ConsecutiveFailures == 100`, y la de reinicio del contador con una precondición— para que
  fallaran de verdad sobre el esqueleto.
- **GREEN: 9/9.** En el refactor se fusionó una segunda prueba de RF-19
  (`…UnGolpeEfectivoReiniciaElContadorDePista`) dentro de `RNF18`: ambas comprobaban que un golpe
  efectivo reinicia `ConsecutiveFailures`. RF-19 conserva su prueba de traza
  (`…SoplarNoSeDeshabilitaTrasFalloPosterior`).

### 2.3 Notas

- **El MCP de Rider respondió toda la sesión** (`get_unity_compilation_result` y
  `run_unity_tests` contra el Editor abierto, sin cerrarlo). R1 deja de bloquear el flujo
  test-first mientras Rider siga abierto; `unity test` (CLI) queda de respaldo.
- **PlayMode no se re-corrió**: T12 es lógica pura en un assembly aislado, sin escena ni
  `MonoBehaviour`; no hay mecanismo por el que afecte a la suite PlayMode (33/33 + 2 omitidas en
  el corte de la Fase 2).
- **Compilación sin warnings nuevos** (solo los preexistentes de `Game.Audio` vacío). Rider aún no
  incluye los archivos nuevos en la solución, así que `/resolve-diagnostics` no pudo correr; la
  comprobación vinculante de «0 warnings» es la de Unity.

---

## 3. Lo que queda abierto

- **PG-06 / R2** — los valores de `FireLevelConfig` (Muy cerca, 3, 3) son los propuestos por el
  guion §4.3.2 y se validan jugando en el Checkpoint D.
- **`FireLevelConfig.asset`** — se crea en T14, con la escena `Level1_Cave`.
- **Cablear `HintPolicy` (T11) al nivel** — `FireAttempt.ShouldOfferHint` y `HintPolicy` cuentan
  lo mismo por caminos distintos; T14 decide cuál alimenta el botón de ayuda.
