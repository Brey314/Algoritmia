# Fase 2 — Andamiaje mínimo: lo construido y sus resultados

**Slice 1 · Golden Path temprano** · Estado: **T09 y T10 implementadas y verdes, T11 sin empezar** —
corte de este documento: 7 de septiembre de 2026
Verificación vigente: **EditMode 44/44 · PlayMode 31/33** (los 2 fallos son las pruebas de
verificación visual, que no corren en batchmode — §4.2)
Plan técnico: [`plan.md`](plan.md) · Tablero: [`todo.md`](todo.md) · Contrato: `claudeDocs/SPEC.md`
Fase anterior: [`Fase-1-Resultados.md`](Fase-1-Resultados.md)

Este documento registra qué se implementó en la Fase 2, con qué pruebas se verificó y qué resultado
dieron. **El Checkpoint C sigue abierto**: falta T11 entera. No reabre decisiones de `SPEC.md`: las
cita.

---

## 1. Alcance de la fase

La Fase 2 es el módulo `andamiaje`: la capa narrativa que envuelve al nivel jugable. Al cerrar esta
fase el juego se recorre desde el inicio hasta el menú de niveles pasando por la narrativa, pero
todavía no hay nada que jugar.

| Tarea | Qué entrega | Modo de prueba | Estado |
|---|---|---|---|
| T09 | `NarrativeSequence`, `DialogueRunner` y la política de «ya vista» | EditMode | ✅ terminada |
| T10 | Escena `Narrative` parametrizada y los tres assets del Nivel 1 | PlayMode | ✅ terminada |
| T10b | `playModeStartScene`: pulsar Play arranca siempre en `Boot` | manual | ✅ terminada |
| T11 | `HintPolicy`: ayuda a demanda y pista tras tres fallos | EditMode | ⬜ sin empezar |

**T08b** (los botones de la Fase 1 lanzaban `NullReferenceException` fuera de `Boot`) se hizo justo
antes de abrir esta fase y está registrado en el `todo.md`; aquí se menciona porque su causa y la de
T10b son la misma y se cerraron juntas (§5).

---

## 2. Qué quedó en el repositorio

### 2.1 El contenido narrativo es un asset, no código (T09)

- **`DialogueLine`** — una línea: quién habla y qué dice. La acotación —lo que en el guion va en
  cursiva— es una línea más con el hablante vacío (`IsStageDirection`). Ambos campos son
  `[field: SerializeField]` con `[Tooltip]`, según CT-05 y RNF-18: corregir una línea es editar el
  asset, no recompilar.
- **`NarrativeSequence`** — el ScriptableObject de una escena narrativa: `Id`, `Level`,
  `Illustration` y `Lines`. **No es video** (RNF-06, peso del paquete): una ilustración fija con
  cuadros de texto secuenciales.
- **`DialogueRunner`** — C# plano, sin dependencias de Unity. `Advance()` pasa una línea por clic y
  devuelve `false` al pasar la última, que es la señal de salida; `Skip()` salta al final **solo si
  está permitido**, y pedirlo sin permiso no hace nada ni lanza.
- **`NarrativeVisitPolicy`** — decide si el perfil ya vio las escenas de un nivel.

Con esto, **añadir una escena narrativa es crear un asset**: ni una escena, ni un estado del flujo,
ni una rama. Es la razón de que `GameState.Narrative` sea un estado parametrizado, como fija
`SPEC.md` §Arquitectura.

### 2.2 Una sola escena para las quince escenas narrativas del guion (T10)

- **`NarrativeSceneController`** — adaptador delgado en `Game.UI`. Lee
  `GameFlow.NarrativeSequenceId`, busca la secuencia con ese id en su lista y la reproduce.
  **No tiene un solo `if` por secuencia**: las tres del Nivel 1 recorren exactamente el mismo
  código. No lleva botón de pausa, y es deliberado — HU-17 FA-04 lo pide solo durante el juego.
- **Escena `Narrative.unity`** — construida por script del Editor (nunca `.meta` a mano):
  `EventSystem` con `InputSystemUIInputModule` (no el módulo legado, CT-06), `Canvas` Screen
  Space-Overlay con `CanvasScaler` a 1920×1080 match 0.5, la ilustración en la mitad superior, el
  cuadro de diálogo en la franja inferior con hablante y cuerpo, y los botones **Continuar** y
  **Omitir**. Paleta de `Direccion_de_Arte.md`: pergamino `#F7EFE2`, tinta `#3A1E18`, ámbar
  `#E8A33D`, cuadro `#E0D4C0`. Añadida a Build Settings, que pasa a **5 escenas**.
- **Tres assets del Nivel 1** en `Assets/Game/Data/Narrative/`: `N1_Apertura` (7 líneas, guion
  §3.1), `N1_AparicionGuia` (11 líneas, §4.1) y `N1_Hallazgo` (18 líneas, §4.2).
- **`GameState.Narrative` mapeado** a la escena `Narrative` en `GameFlowRunner.Scenes`. Antes no
  estaba, y esa ausencia es la que rompía el recorrido (§5).
- **`Game.UI` pasa a referenciar `Game.Scaffolding`.** El test de arquitectura solo veta que `Core`
  dependa de UI/Audio/niveles y que un nivel referencie a otro (RNF-15, RNF-16, INC-40); la
  dirección UI → Scaffolding es la natural y ninguna regla la prohíbe.

### 2.3 Pulsar Play arranca siempre en `Boot` (T10b)

`Assets/Game/Scripts/Editor/PlayFromBoot.cs` fija `EditorSceneManager.playModeStartScene`. Es la
función nativa de Unity para este caso exacto y **solo afecta al Editor**: en el ejecutable `Boot`
ya es la primera escena de Build Settings. Para depurar una escena aislada basta con vaciar el campo
en Project Settings.

---

## 3. La decisión de diseño que cambió el plan

El plan pedía que `CanSkip` fuera falso «la primera vez que el perfil ve la escena» (INC-28, RF-06).
La lectura obvia —guardar en el perfil la lista de escenas vistas— **está prohibida**: RNF-09, con la
letra fijada en INC-27, cierra lo que se persiste en «nombre o alias, nivel alcanzado, fases
confirmadas y los cuatro indicadores. **Nada más**». Ampliar esa lista sería modificar un entregable
ya radicado, que `CLAUDE.md` marca como «preguntar primero».

Así que «ya vista» **se deriva del progreso** en vez de persistirse: si el perfil confirmó alguna
fase del nivel, ya pasó por sus escenas antes. Es exactamente la consecuencia que pide HU-14 — la
primera vuelta se lee entera, porque es donde el guía nombra la habilidad practicada (RF-12, CP-07),
y solo quien repite puede saltar. La razón está escrita en el `<remarks>` de `NarrativeVisitPolicy`,
no solo aquí: sin esa nota, una futura «mejora» añade el campo al perfil y rompe RNF-09.

**Efecto secundario honesto:** el botón de omitir no se puede ver todavía en el juego real, porque
no hay forma de confirmar una fase hasta **T17**. La regla está probada en EditMode; su verificación
en pantalla queda anotada en el Checkpoint C.

---

## 4. Verificación

### 4.1 EditMode — declarado

**44/44, 0 fallos** (`unity test --mode EditMode`, 07/09/2026). Eran 34 al cerrar la Fase 1; las
diez nuevas son de esta fase:

| Prueba | Requisito |
|---|---|
| `DialogueRunner_RF05_AvanzaUnaLineaPorClic` | RF-05 |
| `DialogueRunner_RF05_TerminaDespuesDeLaUltimaLinea` | RF-05 |
| `DialogueRunner_RF05_UnaSecuenciaSinLineasNaceTerminada` | RF-05 |
| `DialogueRunner_RF06_NoOfreceOmitirLaPrimeraVez` | RF-06 |
| `DialogueRunner_RF06_OfreceOmitirSiLaEscenaYaFueVista` | RF-06 |
| `DialogueRunner_INC28_OmitirLaPrimeraVezNoSaltaLaEscena` | INC-28 |
| `DialogueRunner_INC28_OmitirUnaEscenaYaVistaLlegaAlFinal` | INC-28 |
| `NarrativeSequence_RNF01_NingunaOracionSupera20Palabras` | RNF-01 |
| `NarrativeSequence_RNF18_CadaSecuenciaTieneIdYLineas` | RNF-18 |
| `NarrativeSequence_RF05_LosIdentificadoresNoSeRepiten` | RF-05 |

Las tres últimas no prueban una clase sino **el contenido de los assets**: los textos viven fuera
del código (CT-05, RNF-18), así que el límite de legibilidad de RNF-01 hay que verificarlo sobre el
asset. Por eso el guion se reescribió al pasarlo a los assets: varias frases de §3.1 y §4.1 pasaban
de 20 palabras y se partieron conservando el sentido y el diálogo literal.

### 4.2 PlayMode — declarado

**31/33, 2 fallos, 0 inconclusive** (`unity test --mode PlayMode`, 07/09/2026, 26 s). Eran 27
pruebas al cerrar la Fase 1; las seis nuevas son de esta fase:

| Prueba | Requisito |
|---|---|
| `NarrativeScene_RF05_ResuelveTresSecuenciasDistintasSinRamas` | RF-05 |
| `NarrativeScene_RF05_LaPrimeraLineaEsLaDelAssetPedido` | RF-05 |
| `NarrativeScene_RF05_AvanzarLlegaHastaElFinalYSaleAOtraPantalla` | RF-05, RNF-13 |
| `NarrativeScene_INC28_NoMuestraOmitirLaPrimeraVezQueSeVeLaEscena` | INC-28, RF-06 |
| `NarrativeScene_HU17_NoHayBotonDePausaEnUnaEscenaNarrativa` | HU-17 FA-04 |
| `NarrativeScene_RNF01_LaLineaMasLargaCabeEnSuCuadroDeDialogo` | RNF-01 |

**Los 2 fallos son los conocidos de la Fase 1**, no regresiones: las pruebas
`[Category("VisualVerification")]` (`MainMenu_RNF20_*`, `LevelSelect_RNF19_*`) fallan en batchmode
porque `ScreenCapture` no encuentra la vista de juego. Está medido: **0/2 también sobre el código
sin cambios**. Pasan desde el Test Runner del Editor con la vista de juego abierta.

### 4.3 Dos defectos encontrados al verificar — y corregidos

Llegar a esa cifra costó tres corridas. Las dos primeras no dieron un número peor: no dieron
ninguno, y conviene que quede escrito por qué.

**a) `playModeStartScene` colgaba la suite entera.** La primera corrida se agotó a los 1500 s sin
producir informe. La causa está en el log, no en una hipótesis: el Test Framework importa su
`InitTestScene<guid>.unity` y acto seguido se carga `Boot.unity` — T10b secuestraba la entrada a
Play **de todo el mundo**, incluido el corredor, que se quedaba esperando una escena que ya no
llegaba. Corregido con dos guardas, porque hay dos puertas de entrada: batchmode (`unity test`) y
los callbacks de `TestRunnerApi` (la ventana Test Runner del Editor, que es la que hace falta para
las dos pruebas de verificación visual). Sin la segunda, la trampa seguiría puesta para quien corra
PlayMode desde el Editor. El porqué está escrito en `PlayFromBoot.cs` con la fecha y la evidencia.

**b) Una prueba nueva pasaba sin probar nada.**
`NarrativeScene_RF05_ResuelveTresSecuenciasDistintasSinRamas` recorre las tres secuencias en un
bucle y creaba un `GameFlowRunner` por vuelta, pero el `[TearDown]` solo limpia **entre** pruebas.
Como los objetos `DontDestroyOnLoad` sobreviven a `LoadSceneMode.Single`, en la segunda vuelta ya
había un runner vivo, `GameFlowRunner.Awake` destruía el duplicado —correcto, RNF-16— y la prueba
se quedaba con una referencia muerta cuya FSM nunca salió de `Boot`. Como la comprobación era un
`Assume`, el resultado no fue un fallo sino un **«inconclusive»**: la prueba pasó sin probar nada
durante una corrida completa. **Es el mismo defecto que ya costó una corrección en T04**
(`Fase-0-Resultados.md` §3.4). Dos cambios: la limpieza de persistentes se hace también al inicio
del helper, y esa comprobación sube de `Assume` a `Assert` con mensaje, para que un arreglo roto
falle fuerte en vez de callar.

Ambos defectos eran del andamiaje de verificación, no del código de producción — pero un número
verde obtenido con cualquiera de los dos en pie no habría significado nada.

### 4.4 Verificación manual, conducida sobre el juego real

Recorrido completo desde `Boot`, con el Editor en Play:

| Paso | Resultado |
|---|---|
| Play con `Narrative` abierta | → `Boot` → `MainMenu` ✅ |
| «Créditos» → «Volver» | `Credits` y vuelta a `MainMenu` ✅ |
| «Jugar» | abre el panel de perfil ✅ |
| Nombre repetido | «Ya hay un perfil con ese nombre. Elige otro.» (HU-01 FA-02) ✅ |
| Crear perfil nuevo | FSM `Narrative`, escena `Narrative`, perfil activo ✅ |
| 7 clics en «Continuar» | 7 líneas **distintas**, de «Noche helada…» a «Solo oscuridad.» ✅ |
| Fin de la narrativa | → `LevelSelect` ✅ |
| «La Oscuridad» en `LevelSelect` | → `Narrative` otra vez ✅ |

---

## 5. Por qué esta fase empezó arreglando la Fase 1

Al probar las escenas de la Fase 1 aparecieron **dos problemas superpuestos** que se leían como uno
solo («ninguno de los botones redirige»):

1. **Pulsar Play sobre una escena suelta.** Los tres objetos persistentes los crea `Boot`; sin él
   `GameFlowRunner.Instance` es `null` y cada clic lanzaba `NullReferenceException` —las cuatro
   escenas, con línea exacta en `Logs/Editor.log`—. Lo cerró **T08b** (`ScreenFlow`, un punto único
   de comprobación) y lo eliminó de raíz **T10b** (`playModeStartScene`).
2. **`Narrative` no tenía escena.** `ProfileSelectController.CreateNew` encadena dos transiciones:
   `SelectProfile` (→ `LevelSelect`, encola su carga) y `StartNarrative` (→ `Narrative`, que no
   estaba en `GameFlowRunner.Scenes`). El resultado era quedarse **de pie en `LevelSelect` con la
   FSM en `Narrative`**, y como desde `Narrative` solo son legales `Playing`, `LevelSummary` y
   `LevelSelect`, ningún botón de esa pantalla respondía. Es justo el «estado irrecuperable» que
   RNF-13 prohíbe. Lo cerró **T10** al crear la escena y mapear el estado.

La salida de una escena narrativa es hoy `LevelSelect`, con un comentario que marca el cambio: la
salida natural es entrar a jugar, y pasa a `StartPlaying` cuando **T14** traiga `Level1_Cave`.

---

## 6. Lo que queda abierto

- **T11 · `HintPolicy`** — sin empezar. Sin ella no se cierra el Checkpoint C.
- **Ilustraciones** — las tres secuencias tienen el campo `Illustration` vacío y la escena oculta la
  imagen si no hay sprite. Son los assets A1–A6 del tablero, todavía sin generar.
- **Tipografía** — fuente del sistema, igual que en la Fase 1. Baloo 2 / Nunito
  (`Direccion_de_Arte.md` §11.2) sigue siendo tarea de assets.
- **Ver el botón de omitir en pantalla** — exige una fase confirmada, es decir **T17** (§3).
