# Tablero — Slice 3: El Río

Plan técnico: [`plan.md`](plan.md). Contrato: `claudeDocs/SPEC.md`.
Cada tarea se cierra con su commit asociado (RNF-17, CT-11).

**Leyenda:** `EM` = EditMode (lógica pura, sin escena) · `PM` = PlayMode (integración) ·
`VV` = VisualVerification · `MCP` = la verificación **exige** el corredor de pruebas conectado.

> ⚠️ **R2 — los Slices 1 y 2 no están hechos.** Este slice generaliza piezas que aún no existen.
> No abrir R02 antes del Checkpoint W-F del Slice 2. Solo **R01**, **R09** y **R10** son
> independientes.

> ⚠️ **R1 abierto — no hay corredor de pruebas MCP.** `run_unity_tests` sigue sin conectar.
> Toda casilla marcada `MCP` exige haberla corrido **a mano** en el Test Runner y **declarar el
> resultado**. No dar por hecho que la suite pasó.

> ⚠️ **Pregunta abierta 1 bloquea R02.** «Fase» significa dos cosas en el Nivel 3 y de ello
> depende el formato de datos persistidos, que es **«preguntar primero»** (`SPEC.md` §Límites).

> ✅ **16/09/2026 — los tres avisos de arriba están vencidos.** Los Slices 1 y 2 tienen el código
> cerrado (R2); las pruebas corren con el Editor abierto por el corredor efímero `TestRunnerApi`
> de `CLAUDE.md` §Comandos o por `unity test` con el Editor cerrado (R1); y la pregunta abierta 1
> quedó resuelta: **tres fases** —base · amarre · mástil y vela—, la recolección no se persiste
> (decisión de Santiago, registrada en `PhaseId.PhasesPerLevel`).

---

## Fase 0 — Cimientos del slice

- [x] **R01 · Assembly `Game.Levels.River` y exclusión con tres niveles** — `XS` · `EM`
      RNF-15, RNF-16, INC-40 · depende de: Slice 2 W01 — **16/09/2026.**
      `Game.Levels.River.asmdef` (referencia única `Game.Core`) + `AssemblyInfo.cs` con los dos
      `InternalsVisibleTo`, y `Game.Levels.River.Tests.asmdef` (EditMode). `AssemblyDependencyTest`
      suma el tercer nivel y exige que sean exactamente tres antes de recorrer la exclusión.
      El asmdef de PlayMode llega con R07, que es la primera tarea con escena.
- [x] **R02 · Desbloqueo del Nivel 3 y granularidad de fase** — `S` · `EM`
      RF-03, RF-04, RF-41, RNF-09, RNF-14, CP-02, HU-11, CU-09, CU-10, INC-27, supuestos 2/9/11
      · depende de: R01, Slice 2 W02 — **16/09/2026.** Sin código de producción nuevo: W02 ya
      dejó `PhasesPerLevel = {1, 3, 3}` y `LevelUnlockPolicy` genérica. Entran las pruebas que
      nombran al Nivel 3 —`SaveStore_RF04_ConfirmarUnaFaseDelNivel3SobreviveAlCierre`,
      `PlayerProfile_RF41_UnaFaseAprobadaNoSePierdeTrasUnaPruebaFallida`— y el «por qué no
      cuatro» en `PhaseId`. El desbloqueo lo cubrían ya
      `LevelUnlockPolicy_RF03_ElNivel3EsperaLasTresFasesDelNivel2` (EM) y
      `LevelSummary_RF03_DevuelveAlMenuConNivel3Desbloqueado` (PM); RNF-09 sigue en
      `SaveStore_RNF09_NoPersisteCampoAlgunoFueraDeLaListaCerrada`.

### ✅ Checkpoint R-A — Cimientos
- [x] Compila sin errores ni warnings nuevos (`check_compile_errors`) — 16/09/2026
- [x] Prueba de exclusión RNF-16 con **tres niveles reales**, corrida y **declarada** —
      EditMode **224/224** el 16/09/2026 (corredor efímero, Editor abierto): las cuatro de
      `AssemblyDependencyTest` en verde con `Fire`, `Wheel` y `River` en la tabla
- [x] El menú habilita el Nivel 3 solo tras completar el Nivel 2 — regla probada en EM y PM
      (ver R02); verlo jugando es de Santiago en la revisión
- [x] **Pregunta abierta 1 resuelta con el usuario** — 16/09/2026: tres fases, sin guardado
      al terminar la recolección
- [x] Revisado con el usuario
      *(01/10/2026: Decisión — revisado por Santiago el 30/09/2026 (acta D10, §5), tras comprobar en
      verde en la suite completa del 01/10/2026 la exclusión de RNF-16 con los tres niveles
      (`Architecture_RNF16_NingunAssemblyDeNivelReferenciaAOtroNivel`,
      `Architecture_RNF16_RetirarUnNivelNoAfectaALosOtrosDos`) y el desbloqueo del Nivel 3
      (`LevelUnlockPolicy_RF03_ElNivel3EsperaLasTresFasesDelNivel2`,
      `LevelSummary_RF03_DevuelveAlMenuConNivel3Desbloqueado`); desde INC-116 (30/09) el menú lo
      desbloquea también si las tres fases del Nivel 2 quedaron confirmadas sin llegar al resumen
      (`LevelSelect_RNF14_UnNivelConTodasSusFasesConfirmadasDesbloqueaElSiguiente`).)*

---

## Fase 1 — Andamiaje del Nivel 3 (`andamiaje`)

- [x] **R03 · `HintPolicy` para recolección y ensamblaje** — `M` · `EM`
      RF-13, RF-10, RF-11, RNF-03, CP-06, HU-03, HU-04, CU-09, CU-10, INC-41 · depende de: R02
      — **16/09/2026.** `HintPolicy` no cambia: ya es genérica (qué cuenta como fallo lo decide
      la escena, que solo llama `RegisterFailedAttempt`). Entra el contenido
      `Assets/Game/Data/Guide/N3_Guia.asset` con **cuatro** pasos —`Recolectar` (cubre las
      tareas 1 y 2), `Base`, `Amarre`, `MastilYVela`— y las cuatro pruebas del plan en
      `HintPolicyTests`, que fijan sobre el asset lo que la pista no puede decir. La de
      inventario se apoya en RNF-16: `Game.Scaffolding` no puede referenciar `Game.Levels.River`.
- [x] **R04 · Las cinco secuencias narrativas del N3** (una **condicional**) — `S` · `EM` + `PM`
      RF-05, RF-06, RF-10, RF-12, RNF-01, RNF-18, CP-07, HU-02, INC-28, INC-39,
      guion §7, §8.1, §8.4.1, §8.5, §9 · depende de: R03
      — **16/09/2026.** Cinco escenas en **seis** assets `N3_*.asset`: el puente II cambia de
      ilustración a mitad (bosque del N2 → río) y la ilustración es por secuencia, así que
      `N3_PuenteII` encadena con `N3_PuenteII_Rio`. Los 32 encuadres son los de
      `docs/md/Camara_Narrativa_N3.md`, con sus suavizados (1.4 · 1.0 · 1.6 · 2.5 s) y la línea
      larga de Algoritm en 3.1 partida en cinco (§7.1) y el recuento final en tres (§7.2).
      `ConditionalNarrativeTrigger` (C# plano) dispara la 3.2 una sola vez tras el primer fallo.
      `IllustrationFraming.Warnings` recibe ahora la proporción del sprite: con 16:9 sin duplicar
      el rango en x es `0.5/z`, no `0.25/z`. Objetos provisionales: la carretilla del N2 en el
      puente, `prop_n3_balsa_hundida` en la 3.2 y `prop_n3_balsa_cruzando` en la 3.3, colocados
      **más arriba de lo que propone §6 del documento de cámara** para no quedar bajo el cuadro
      de diálogo (regla de D05: se mueve el objeto, no el encuadre). Entornos: `env_n3_rio` y
      `env_final_fogatas`, renombrados desde el motor. Sin sprites de la familia, igual que el N2.
      `N3_EscenaFinal` se declara cierre reflexivo para que no sea omitible (CP-07); a dónde sale
      lo decide R14. Las seis están registradas en `Narrative.unity` y el Nivel 3 abre con
      `N3_PuenteII` desde `LevelSelect.unity` (script de editor efímero, ya borrado).
      **Suite del 16/09/2026:** EditMode 234/234; PlayMode `Game.UI.PlayMode.Tests` 63/63 con
      `NarrativeScene_RF05_ResuelveLasCincoSecuenciasDelNivel3SinRamas` en verde. Las hojas de
      `docs/md/verificacion_encuadres_N3/` son el diseño y ya coinciden con los assets; se
      rehacen desde el contenido cuando cambie un encuadre o entren los sprites definitivos.

### ✅ Checkpoint R-B — Andamiaje del Nivel 3
- [x] Las cinco escenas narrativas se recorren completas — PlayMode, 16/09/2026 (seis assets)
- [x] La escena 3.2 aparece tras un fallo y **no aparece** si se acierta al primer intento — la
      **regla** está probada (`NarrativeTrigger_Guion841_…`, EM); conectarla a la prueba de la
      balsa es de R10, que es donde existe el fallo
      *(01/10/2026: Implementación — R12 (`3fe1e2a`, 20/09) conectó la regla a la prueba de la
      balsa: `RiverScene_Guion841_LaEscena32SeReproduceSoloTrasElPrimerFallo` y
      `RiverScene_Guion841_AcertarAlPrimerIntentoVaDirectoAlCruce`, con
      `RiverScene_CP02_LaEscena32NoReiniciaElEnsamblaje` y los dos
      `RiverLevel_RNF13_RecorreElNivel3CompletoHastaElInicio_*`. Con «Probar balsa» también en la
      base y el amarre (INC-122, D10-3) la 3.2 sigue saliendo solo con el primer fallo de mástil y
      vela: `RiverScene_Guion841_ProbarAntesDeLaUltimaFaseNoGastaLaEscena32` y las otras
      `RiverScene_Guion841_*`, en verde en la suite completa del 01/10/2026.)*
- [x] Ninguna pista del Nivel 3 resuelve la tarea (CP-06) — probado sobre `N3_Guia.asset`
      (materiales, ubicaciones y orden prohibidos en la pista); la lectura de los textos es de
      Santiago en la revisión
      *(01/10/2026: Decisión — revisado por Santiago el 30/09/2026 (acta D10, §5), tras comprobar
      que las cuatro instrucciones y las cuatro pistas de `N3_Guia.asset` no nombran material, sitio
      ni orden —las del ensamblaje son texto del guion §1.8.4—:
      `HintPolicy_RF13_PistaDeRecoleccionNoNombraLaUbicacionDelMaterial` y
      `HintPolicy_CP06_PistaDeEnsamblajeNoDiceQuePiezaVaEnQueEspacio`, en verde en la suite completa
      del 01/10/2026. La prueba anticipada de la balsa (INC-122) no cambia esos textos.)*
- [x] Revisado con el usuario
      *(01/10/2026: Decisión — revisado por Santiago el 30/09/2026 (acta D10, §5) tras los cambios
      de D10-3 en las narrativas del río: la 3.1 muestra los materiales de la mecánica, la balsa de
      la 3.3 navega bajo la espuma de la cascada (INC-124) y la escena final suena al bosque con
      fogatas (INC-125); en verde en la suite completa del 01/10/2026
      `NarrativeSequence_RF44_LaBalsaDelCruceNavegaBajoLaEspumaDeLaCascada`,
      `RiverSounds_RF44_LaEscenaFinalSuenaAlBosqueConLasFogatas`,
      `NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo` y
      `NarrativeScene_RF05_ResuelveLasCincoSecuenciasDelNivel3SinRamas`, y revisadas en la revisión
      adversarial de W2 las capturas `Personajes_N3_*`.)*

---

## Fase 2 — Recolección (`nivel-rio`)

- [x] **R05 · `TaskList` — las cuatro tareas y su correspondencia exacta** — `S` · `EM`
      RF-36, RF-11, RF-43, RNF-03, RNF-19, CP-02, CP-03, **INC-30**, HU-11, CU-09 (FA-5a), CU-10,
      supuesto 11, guion §8.1/§8.2 · depende de: R04 — **17/09/2026.** `RiverTask` (enum de las
      cuatro tareas + la regla de cuándo se marca cada una) y `TaskList` (estado, **sin forma de
      desmarcar**). Las cinco pruebas del plan en `TaskListTests`, con la negativa de INC-30
      (`LaFaseDeBaseNoMarcaTareaPorSiSola`) y la de RF-43 comprobando por reflexión que la superficie
      pública solo marca. Los textos viven en `N3_RiverLevelConfig.asset` (`TaskLabels`, §1.8.1) y
      `RiverLevelConfigTests` los fija contra el guion.
- [x] **R06 · `Inventory`, `Collectible` y proximidad** — `M` · `EM`
      RF-37, RF-38, RF-11, CT-05, RNF-18, CP-02, HU-11, CU-09 (FA-4a) · depende de: R05 —
      **17/09/2026.** `Collectible` (id, clase, nombre, arte, posición **en fracciones de la
      ilustración**, `IsWithinReach` contra una posición inyectada), `Inventory` (capacidad = catálogo,
      recoger no falla, el cuarto ya informa «tienes todo») y `RiverLevelConfig` con radio, velocidad,
      orilla andable y textos. Las cuatro pruebas del plan en `InventoryTests` (+ `RiverWalk` y `BuildZone`
      como regla pura). Sprites **provisionales** por código (`prop_n3_troncos/_sogas/_tela/_mastil`).
- [x] **R07 · Escena `Level3_River` y movimiento con botones en pantalla** — `M` · `PM` `MCP`
      **RF-35**, RF-10, RF-13, RNF-02, RNF-03, CT-06, **INC-01**, supuesto 6, HU-11, CU-09,
      guion §2.1/§8.2 · depende de: R06 — **17/09/2026.** `RiverWalk` (C# plano: posición y límites
      en fracciones, `Step` recorta), `DirectionPad` (una flecha = un botón uGUI con `IPointerDown/Up`,
      **sin Input System**) y `RiverSceneController` (adaptador: plano fijo `(0.400, 0.410) ×1.50` de
      `Camara_Narrativa_N3.md` §5.3, Mamá/materiales/zona colgados de la ilustración). Disposición del
      mockup 11-12: lista arriba-izquierda, inventario 2×2 abajo-izquierda, flechas en cruz y «Recoger»
      abajo-derecha, tablilla del guía arriba, pausa (prefab `MenuPausa`). Escena construida por script
      de editor efímero y registrada en Build Settings (undécima). `RiverScene_INC01_…` inspecciona el
      **`.inputactions`** (cero `<Keyboard>`) **y** que el assembly no referencie `Unity.InputSystem` ni
      `UnityEngine.InputLegacyModule`. `Game.Levels.River.PlayMode.Tests.asmdef` creado.
      `char_mama_cenital.png` provisional (una postura, sin animación).
- [x] **R08 · Zona de construcción** — `S` · `PM` `MCP`
      RF-39, RF-11, RF-04, CP-02, CP-03, HU-11, CU-09 (FA-6a) · depende de: R07 — **17/09/2026.**
      `BuildZone` (C# plano): `Contains` por radio, `TryEnter` nombra lo que falta («sogas, tela y
      mástil», nunca cuántos) y abre una sola vez. **Desviación respecto al plan:** al abrirse **no se
      confirma ni guarda fase alguna** — la recolección no se persiste (decisión de R02); confirmar
      base/amarre/mástil es del panel (R11). El panel es hoy un hueco (`Panel_Assembly`, solo título)
      que se enciende y retira las flechas; R11 lo llena. `env_n3_zona_disponible.png` provisional.
      **Suite del 17/09/2026:** EditMode **251/251**; PlayMode `Game.Levels.River.PlayMode.Tests`
      **10/10** (8 de integración + 2 capturas `VisualVerification` en `TestScreenshots/RiverScene_*`).

### ✅ Checkpoint R-C — Recolección completa
- [x] Se recorre el mapa, se recogen los cuatro materiales y se entra a la zona de construcción —
      `BuildZone_RF39_AbreElPanelSoloConLosCuatroMateriales` (PM, 17/09/2026)
- [x] **Ninguna tecla mueve al personaje** — `RiverScene_INC01_NoExisteVinculacionDeTecladoEnElMapaDeControles`
      (asset de acciones + referencias del assembly). Queda `Assets/Settings/InputSystem_Actions.inputactions`,
      la plantilla de Unity con WASD, que **nadie referencia**: borrarla o no es de R16 (cierre de RNF-02)
- [x] Las tareas 1 y 2 quedan marcadas; las 3 y 4 siguen sin marcar — misma prueba de RF-39 (PM) y
      `TaskList_INC30_…` (EM)
- [x] Lista de tareas e inventario visibles todo el tiempo y sin solaparse —
      `RiverScene_RNF03_ControlesListaEInventarioCabenEnPantallaYNoSeSolapan`. Ojo: el hueco del panel
      de ensamblaje **tapa la lista** al abrirse (captura `RiverScene_RF39_ZonaAbierta`); R11 decide dónde va
- [x] Radio de proximidad de RF-37 validado jugando (pregunta abierta 3) — hoy `0.06` de la ilustración
      (~170 px a 1920) y velocidad `0.25`/s en `N3_RiverLevelConfig.asset`; se ajusta sin recompilar
      *(01/10/2026: Decisión — el radio ya no era 0,06 sino 0,04 desde el plano del bosque del 20/09
      (`3fe1e2a`, ≈145 px a ×1,9); con el plano abierto del 30/09 —decisión de Santiago, INC-118,
      D10-3— vuelve a 0,06 de la ilustración, igual que el de la zona: `ProximityRadius` y
      `BuildZoneRadius` valen 0,06 en `N3_RiverLevelConfig.asset`, ≈161 px a 1920×1080 con el plano
      ×1,4 (`PlayFraming` (0,3572; 0,3572));
      `Collectible_RF37_ElBotonRecogerSoloApareceDentroDelRadioDeProximidad` y
      `RiverLevelConfig_RF37_ElRepartoObligaARecorrerLaOrillaYCabeEnElPlanoFijo` en verde en la
      suite completa del 01/10/2026 —ningún material al alcance desde el arranque ni pegado a la
      zona—; en el ejecutable, «Recoger» no aparece lejos y sí al llegar al material (PF-RF37-01 en
      P, `claudeDocs/tasks/OE4/evidencias/GP1/PF-RF37-01_sin_recoger_lejos.png` y
      `_tronco1_al_inventario.png`; GP3 lo confirma, `OE4-Resultados.md` §8.5); revisado por Santiago
      el 30/09/2026 (acta D10, §5).)*
- [x] Revisado con el usuario — pregunta abierta 2: con el plano fijo **los ocho materiales se
      ven desde el arranque** (no hay paneo); el reparto obliga a recorrer la orilla igual
      (`RiverLevelConfig_RF37_ElRepartoObligaARecorrerLaOrillaYCabeEnElPlanoFijo`)
      *(01/10/2026: Decisión — revisado por Santiago el 30/09/2026 (acta D10, §5) sobre el plano
      abierto del 30/09 (INC-118, D10-3): los ocho materiales se ven desde el arranque, ninguno se
      recoge sin moverse y la zona se ve desde el principio (INC-66); en verde en la suite completa
      del 01/10/2026 `RiverLevelConfig_RF37_ElRepartoObligaARecorrerLaOrillaYCabeEnElPlanoFijo` y
      `RiverLevelConfig_Guion82_ElPlanoDeRecoleccionEsLaOrillaConElRio`, y revisada la captura de la
      orilla al abrir (`RiverScene_RF36_CapturaDeLaOrillaAlAbrir`, que guarda
      `RiverScene_RF36_OrillaAlAbrir`).)*
- [x] **Plano de la recolección rehecho (20/09/2026, decisión de Santiago):** solo el bosque, sin río —
      foco `(0.20, 0.20) ×2.5`, el cuadrante inferior izquierdo de `env_n3_rio`; el río entra con el
      empuje del ensamblaje (`RiverLevelConfig_Guion82_ElPlanoDeRecoleccionMuestraSoloElBosqueSinElRio`).
      **El piso termina en `GroundTop = 0.36`** (medido en la ilustración: el pasto llega a y ≈ 0.34–0.40
      antes de los arbustos); orilla andable `x 0.13–0.36 · y 0.05–0.31`, sin el seto ni las raíces;
      los ocho materiales, el arranque y la zona quedan en el piso. **Perspectiva por profundidad**
      (`DepthScaleAt`: 1.0 abajo → 0.55 donde termina el piso) para Mamá, materiales y zona
      (`RiverLevelConfig_DA83_…`, `RiverScene_DA83_…`). Radio de proximidad y de la zona a `0.04`
      (a zoom 2.5 son ≈ 190 px). Botón de ayuda: el circular de los niveles 1 y 2 (`Boton_Pista`).
- [x] **Plano de la recolección abierto (25/09/2026, decisión de Santiago):** que se vea un poco el
      río — foco `(0.2632, 0.2632) ×1.9`, recorte `[0, 0.526]²`: el bosque sigue llenando el plano y la
      orilla con el pie de la cascada asoma a la derecha (`RiverLevelConfig_Guion82_ElPlanoDeRecoleccionEsElBosqueConUnPocoDelRio`,
      que sustituye a la de «sin el río»). El empuje del ensamblaje pasa de alejar (×0,88) a acercar
      (×1,16) hacia el río. Los radios siguen en `0.04`: a zoom 1.9 son ≈ 145 px.

---

## Fase 3 — Ensamblaje y depuración (`nivel-rio`)

> **R09 y R10 no dependen de R05..R08** y se pueden adelantar. Ver pregunta abierta 6.

> **Decisiones de Santiago del 20/09/2026 que cambian R11 respecto al plan:** no hay modal. El
> ensamblaje se arma **sobre el río**: la cámara empuja del plano de juego a `(0.50, 0.23) ×2.2`
> —el río al 85 % del ancho, la orilla parte la pantalla (pasto a la izquierda, agua a la
> derecha)—, con una sombra negra al 30 % sobre la ilustración y la balsa en la mitad. La balsa
> son **cinco troncos** (los cinco hay que encontrarlos por la orilla; comparten una casilla que se
> llena con cinco marcas), **diez amarres** (uno en cada extremo de cada tronco, de un solo rollo
> de soga que se arrastra diez veces), el mástil (un tronco más largo: la trampa de la base) y la
> vela. **Composición, no láminas:** diecisiete espacios pintados uno a uno con ocho sprites
> (pieza + silueta dibujada aparte por clase) en lugar de las 34 láminas de estado. Colocar no
> valida; el botón sí. Error = pieza equivocada o espacio vacío (guion §1.8.4).

- [x] **R09 · `RaftAssembly` — tres fases bloqueantes** — `M` · `EM`
      RF-40, RF-41, RF-11, RF-17, RF-18, RNF-18, CP-02, CP-06, **INC-30**, HU-12, CU-10,
      guion §8.3 · depende de: R01 — **20/09/2026.** `RaftPhase`, `RaftSlot` (contenido: fase,
      material que acepta, posición y tamaño en fracciones del área de la balsa, giro),
      `RaftAssemblyContent` (espacios, arte por clase, amarres por rollo, encuadre, frases) y
      `RaftAssembly` (C# plano: solo se ven los espacios de la fase activa y las consolidadas;
      `Place` no valida; `TakeBack`; `Confirm` consolida o devuelve **solo** lo mal puesto; `Resume`
      para RNF-14; `Rejections` solo para RF-45, sin límite). Las cinco pruebas del plan en
      `RaftAssemblyTests` + `RNF14_Retomar…` + `RF38_ElRolloDeSoga…`.
- [x] **R10 · `RaftValidator` — prueba de balsa y depuración** — `M` · `EM`
      RF-42, RF-43, RF-11, RF-17, RF-18, CP-02, CP-03, CP-06, HU-13, CU-10 (FA-6a, FA-6b),
      guion §8.4 · depende de: R09 — **20/09/2026.** `RaftValidator.WrongSlots` (función pura:
      vacío o con otra pieza) y `ValidationResult`. Las cinco pruebas del plan en
      `RaftValidatorTests`, la de RF-17 y la de CP-06 leyendo el asset real, más
      `RaftAssemblyContent_RF40_ElAssetTraeLaBalsa…` (5 + 10 + 2 espacios, arte y silueta por clase).
- [x] **R11 · Panel de ensamblaje en escena** — `M` · `PM` + `VV` `MCP`
      RF-40, RF-41, RF-42, RF-43, RNF-02, RNF-03, RNF-19, RNF-21, CT-06, HU-12, HU-13,
      CU-10 · depende de: R10, R08 — **20/09/2026.** `AssemblyPanelController` (adaptador:
      `Sombra_Ensamblaje` y `Area_Balsa` cuelgan de la ilustración; espacios instanciados de
      `EspacioTemplate` con `RaftPieceHandle` e `Image_Alerta`; empuje de cámara con
      `IllustrationFraming.ScaleAbout` como el taller; pulso de completado y hundimiento por giro,
      continuos, RNF-21; `Button_Confirm` «Listo» → «Probar balsa»; confirma y guarda cada fase con
      `PhaseId`). Arrastre **sin Input System ni arrastre de uGUI**: asas `IPointerDown/Up` en las
      casillas y en los espacios, y `PointerTracker` (`IPointerMoveHandler` en el `Canvas`) para
      seguir el cursor. `InventoryView` separado (casilla por clase, marcas para los troncos), y
      `Inventory` ahora cuenta por clase (`Required`/`Count`/`Has` = clase completa).
      `GameFlowRunner.PlayingScenes` gana `River 2` y `River 3` → la escena retoma en el panel.
      Pruebas en `AssemblyPanelTests` (RF40, RNF19, RNF02, RNF03 de layout con la fase 3 abierta,
      HU12 `VisualVerification` con tres capturas `TestScreenshots/AssemblyPanel_HU12_*`).
- [x] **R12 · Escena 3.2, condicional al primer fallo** — `S` · `PM` `MCP`
      RF-05, RF-06, RF-11, RF-12, CP-02, CP-07, guion §8.4.1 · depende de: R11 — **20/09/2026.**
      `ConditionalNarrativeTrigger` cableado tras el hundimiento; para que «la escena narra, no
      reinicia» el disparador y las piezas de la fase abierta viven en **estáticos del assembly**
      (memoria de nivel: sobreviven a la recarga de la escena narrativa, no al proceso — RNF-09 no
      admite «escenas vistas»). Pruebas en `ConditionalNarrativeTests` con un `GameFlowRunner` de
      prueba y sin `SceneLoader`: las tres del plan.
      **Suite del 20/09/2026 (Rider MCP, Game View fijada a 1920×1080 con
      `PlayModeWindow.SetCustomRenderingResolution`):** EditMode **265/265** (todos los assemblies; tras el plano del bosque, `Game.Levels.River.Tests` **31/31**);
      PlayMode `Game.Levels.River.PlayMode.Tests` **20/20** (17 de integración + 3 capturas
      `AssemblyPanel_HU12_*`; la de `…PorRaycastYSueltaEnElEspacio` reproduce el camino real del clic —
      la casilla no era objetivo de raycast y el arrastre no arrancaba jugando, corregido el 20/09/2026), `Game.Core.PlayMode.Tests` **8/8**, `Game.UI.PlayMode.Tests` **63/63** (excede el tiempo del puente MCP:
      leer `TestResults.xml` en `persistentDataPath`). Ojo: con la Game View en otra
      proporción (880×405, «Free Aspect») fallan `RiverScene_RF35_…` y `AssemblyPanel_RNF03_…` porque
      el recorte 16:9 deja fuera el borde inferior — es el entorno, no el código.

### ✅ Checkpoint R-D — Ensamblaje completo
- [x] La balsa se construye por las tres fases y se prueba — `AssemblyPanel_RF40_…` (PM) y
      `RaftAssembly_RF40_LasTresFases…` (EM)
- [x] Un fallo devuelve **solo** lo mal puesto y conserva las fases aprobadas (RF-43) —
      `RaftValidator_RF43_…` (EM) y `AssemblyPanel_RNF19_…` (PM)
- [x] La escena 3.2 aparece tras el primer fallo y no aparece si se acierta de una —
      `RiverScene_Guion841_…` (PM)
- [x] Ningún mensaje nombra la pieza correcta (CP-06) — `RaftValidator_CP06_…` sobre el asset
- [x] Encuadre del ensamblaje validado jugando: `(0.50, 0.23) ×2.2` sale de medir el río en
      `env_n3_rio` (≈ 38 % del ancho en el tramo bajo → 84 % de pantalla; la orilla a esa altura
      pasa por x ≈ 0.50 y parte la pantalla en dos); se ajusta en
      `N3_RaftAssemblyContent.asset` sin recompilar
      *(01/10/2026: Decisión — el encuadre `(0.50, 0.23) ×2.2`, con la balsa en x 0,5 y medio casco
      sobre el pasto, quedó atrás: Santiago decidió el 30/09 (INC-118, D10-3) abrir el plano del
      ensamblaje y correr la balsa sobre el agua, en `N3_RaftAssemblyContent.asset` y sin
      recompilar: el encuadre final es `AssemblyFraming` (0,50; 0,3125) ×1,6 y la balsa va en x 0,58
      (`RaftPosition` (0,58; 0,23), `RaftSize` 0,28), con su área en (964, 156)–(1448, 639) px a
      1920×1080; `RaftAssemblyContent_RF44_LaBalsaDelEnsamblajeMideLoQueLaDelCruce` y
      `AssemblyPanel_RNF03_*` en verde en la suite completa del 01/10/2026, y revisadas las capturas
      `AssemblyPanel_HU12_*` de las tres fases; revisado por Santiago el 30/09/2026 (acta D10,
      §5).)*
- [x] Revisado con el usuario
      *(01/10/2026: Decisión — revisado por Santiago el 30/09/2026 (acta D10, §5) tras los cambios
      de D10-3 en el ensamblaje: el plano abierto con la balsa sobre el agua y «Probar balsa»
      también en la base y el amarre —la balsa incompleta se hunde y suena, cuenta como intento,
      devuelve solo lo mal puesto y no aprueba la fase (INC-122, INC-123)— lo vigilan, en verde en
      la suite completa del 01/10/2026, `RaftAssembly_RF42_*`,
      `RaftAssembly_RF43_ProbarAntesDeTiempoDevuelveSoloLoMalPuesto`,
      `RaftAssembly_RF41_ProbarLaBalsaNoTocaLasFasesAprobadas`, `AssemblyPanel_RF42_*`,
      `RiverIndicators_RF45_UnaPruebaDeBalsaIncompletaCuentaComoIntento` y
      `AssemblyPanel_CP02_UnaFaseQueNoPasaNoSuena`, y revisadas las capturas de las tres fases.)*

---

## Fase 4 — Cierre del nivel y del juego

- [x] **R13 · Emisión de los cuatro indicadores del N3** — `M` · `EM` (21/09/2026)
      RF-45, RF-04, RF-07, RNF-09, RNF-14, CP-03, CP-09, OE1 §3.6.1 (notas 1–5), INC-27,
      INC-30 · depende de: R12, Slice 2 W15
      `RiverIndicatorCollector` (`Game.Levels.River`, implementa `ILevelReporter`): **un solo
      recolector para las tres fases**, porque §3.6.1 define los indicadores del N3 sobre el nivel
      entero —«pasos utilizados» son las confirmaciones aceptadas *sobre un máximo de tres*— y las
      tres fases se juegan en el mismo panel; solo el tiempo es de cada fase y `Complete()` reinicia
      el reloj al cerrarla. `RecordConfirmation(ValidationResult, devueltas)`: rechazo = intento;
      aceptación = paso; error corregido = pieza devuelta en el intento anterior cuyo espacio ya no
      aparece señalado en este (un espacio vacío no devuelve nada, así que llenarlo no corrige).
      `PauseOpened`/`PauseClosed` (nota 1). **El reloj de la base arranca al abrir el ensamblaje**,
      no en la orilla: la recolección no es fase persistida. `AssemblyPanelController` lo guarda en
      la memoria de nivel (`s_indicators`, junto a `s_stash`) para que sobreviva a la escena 3.2 —a
      la que sale con `PauseOpened()` y de la que vuelve con `PauseClosed()`—, lo registra en
      `GameFlowRunner.ActiveReporter`, al retomar desde disco siembra las fases ya aceptadas como
      pasos (`confirmedPhases`, RNF-14) y persiste `Complete()` donde antes iba `default`.
      EditMode (`RiverIndicatorTests`, 8/8 el 21/09/2026 contra el Editor abierto):
      `RiverIndicators_RF45_IntentosCuentaFasesRechazadasYPruebasFallidas`,
      `_RF45_PasosUtilizadosNoSuperaTres`, `_RF45_RetomarEnUnaFaseCuentaLasConfirmadasAntes`,
      `_RF45_ErrorCorregidoExigeRecolocacionCorrectaPosterior`,
      `_RF07_LaPausaNoSumaTiempoDeResolucion`, `_OE1361_ElTiempoDeResolucionEsDeCadaFase`,
      `_OE1361_ReiniciarElNivelNoBorraLosIndicadoresRegistrados`,
      `_CP03_NingunIndicadorLlegaALaUIDelEstudiante` (barrido de reflexión sobre `Game.UI`).
      Regresión: EditMode completo 275/275 y `Game.Levels.River.PlayMode.Tests` 20/20.
- [x] **R15 · Doble indicador y contraste en los estados de error del N3** — `S` · `VV` `MCP` (21/09/2026)
      **RNF-19** (cierra su segunda mitad), RNF-20, RNF-21, CN-04, HU-13 · depende de: R11
      `RiverAccessibilityTests` (PlayMode, corrido contra el Editor abierto por Rider, **4/4**, y la
      suite del río **24/24**): `RiverLevel_RNF19_LosEstadosDeErrorSeLeenSinColor` (zona sin
      materiales, colocación incorrecta y prueba de balsa fallida: icono ≠ al del acierto + texto sin
      cifras en la tablilla, y el icono de alerta encendido sobre cada espacio señalado; tres
      capturas), `_RNF19_LaListaDeTareasSeLeeEnEscalaDeGrises` (visto y círculo son sprites
      distintos; captura **desaturada** por la propia prueba), `_RNF20_ContrasteSuficienteSobreElEscenarioClaro`
      (WCAG texto/cara medido en escena y **exigiendo que ningún texto cuelgue de la ilustración**:
      tablilla del guía 13,3:1 · tareas 6,3:1 · «Recoger», «Listo»/«Probar balsa» y los cinco
      botones de la pausa ≥ 7,1:1) y `_RNF21_NingunaAnimacionDelNivel3TieneDestellos` (empuje de
      cámara, pulso de fase confirmada y hundimiento muestreados cuadro a cuadro: nada se apaga,
      la sombra no parpadea, ningún salto mayor que el que sigue el ojo; recoger no anima; tres
      capturas). **El cruce (RF-44) no existe todavía: lo cubre R14.**
      **Hallazgo real de RNF-19:** entrar a la zona sin los materiales se mostraba con tono `Help`,
      cuyo icono está vacío en la escena (igual que en las tres del Nivel 2): solo palabras, sin
      icono ni color. Es una acción rechazada (CU-09 FA-6a), no una pista: `RiverSceneController.Enter`
      pasa a `MessageTone.Rejected` — icono de alerta más color, la frase no cambia (CP-02).
      **Falso hallazgo de RNF-20, documentado en la prueba:** el botón de confirmar medía 3,9:1 en
      las capturas porque se deshabilita en cada animación y vuelve con un `CrossFade` de 0,1 s; el
      corredor va a ~1000 fps y capturaba a 3 ms del desvanecido. En juego real es ámbar sólido:
      la prueba espera el desvanecido y exige el `CanvasRenderer` en blanco antes de medir.
      Capturas en `%AppData%\LocalLow\DefaultCompany\My project\TestScreenshots\RiverLevel_*.png`,
      revisadas.
- [x] **R16 · Cierre de RNF-02 y RNF-16 sobre el juego completo** — `S` · `PM` + `EM` `MCP` (21/09/2026)
      **RNF-02**, **RNF-16**, CT-06, **INC-01** · depende de: R11
      **Hallazgo real:** nueve escenas —las cuatro jugables del N2 y N3 y las cinco de flujo con
      módulo— usaban en su `InputSystemUIInputModule` el `DefaultInputActions` del paquete Input
      System (teclado y mando incluidos), y el mapa de acciones del proyecto (Input System →
      Project-wide Actions, que entra al ejecutable) era la plantilla `Assets/Settings/
      InputSystem_Actions.inputactions` con 23 vinculaciones de teclado; solo `Level1_Cave` estaba
      bien. Recableadas desde el motor las nueve a `Assets/Game/Input/ControlesJugables.inputactions`
      (el setter de `actionsAsset` remapea Point/Click/RightClick/MiddleClick/ScrollWheel por
      nombre; Move/Submit/Cancel quedan nulos: es lo que se quiere) y el proyecto apunta al mismo
      asset (`EditorBuildSettings.asset`). Al mapa se le quitó la vinculación `<Mouse>/scroll`
      —el criterio pide «ni rueda del ratón»; la acción sigue, sin binding—; quedan
      `<Pointer>/position`, `<Mouse>/leftButton|rightButton|middleButton`,
      `<Touchscreen>/primaryTouch/tap` y `<Pen>/tip`. La plantilla de `Assets/Settings/` sigue en
      disco sin que nada la use: borrarla es decisión de Santiago. EditMode (`Game.Architecture.Tests`,
      leen disco): `Architecture_RNF16_RetirarUnNivelNoAfectaALosOtrosDos` (por nivel: ningún
      módulo de runtime lo tiene en su cierre de dependencias y ninguna escena ajena, de flujo ni
      prefab compartido contiene guids de sus scripts — las tres combinaciones),
      `Architecture_RNF02_NingunAssemblyUsaLaClaseInputLegada` (`activeInputHandler: 1` en
      `ProjectSettings.asset` más barrido de `Input.GetKey|GetMouse|GetAxis|mousePosition…` en
      `Scripts/`), `Architecture_INC01_ElMapaDeControlesNoTieneTecladoMandoNiRueda` y
      `Architecture_RNF02_LasEscenasYElProyectoUsanSoloElMapaDeControlesDelJuego` (toda escena con
      `m_ActionsAsset` y el proyecto → `ControlesJugables`; las cinco jugables llevan módulo).
      PlayMode (`Core`, el asmdef ganó `Unity.InputSystem` y `UnityEngine.UI`):
      `Controls_RNF02_LasCincoEscenasJugablesSoloAceptanClicYClicSostenido` carga las cinco de
      verdad: módulo → `ControlesJugables`, vinculaciones solo de clic, puntero y clic cableados,
      sin `PlayerInput`, sin `ScrollRect`; lo que cada nivel deja hundir o arrastrar sigue en su
      propia `*_RNF02_*`. `unity test` (Editor cerrado): EditMode **282/282**; PlayMode
      `Core` + todas las `RNF02`/`INC01` de los cinco niveles **17/17**; PlayMode completo como
      regresión del recableado: **165/184**, 6 omitidos (visuales que piden Game View) y 13
      fallos, todos de disposición o captura medidos contra la pantalla de 640×480 de batchmode
      —los diez ya anotados en R14 más `ForestScene_RF26_LaCajaYLosTroncos…`,
      `MazeScene_RNF03_ConMasBloques…` y `WorkshopScene_RNF03_NadaSeSale…`—; los dos recorridos
      completos del N2 (RNF-13), que pasan por las nueve escenas recableadas, pasan. **Los trece
      se repiten en el Editor con Rider antes del checkpoint R-E.**
- [x] **R14 · Cruce, escena final y cierre del juego** — `M` · `EM` + `PM` `MCP` (21/09/2026)
      RF-44, RF-12, RF-45, RF-17, RF-08, RF-03, RNF-13, CP-03, CP-07, CP-10, HU-13, HU-14,
      CU-10, INC-26, INC-37, **INC-39**, guion §8.5/§9 · depende de: R13, R15, R16 (R16 sigue
      abierta: son pruebas de RNF-02/RNF-16, no código del cierre)
      **El flujo lo deciden los assets, no una rama por nivel.** `GameFlow` acepta
      `Narrative → Credits`. `NarrativeSequence.EndsInCredits` —marcado en `N3_EscenaFinal`, que
      **sigue siendo** cierre reflexivo: esa es la marca que le niega «Omitir» la primera vez sin
      tocar `NarrativeVisitPolicy`— y `NarrativeSceneController.Leave` lo atiende antes que al
      resumen. `LevelSummaryMessages.ClosingSequenceId` (vacío = menú de niveles, como N1 y N2)
      saca «Continuar» a la escena final; lo declara `N3_ResumenNivel.asset` (nuevo, creado desde
      el motor, cableado en `LevelSummary.unity`). El cruce (RF-44) es `PropMotion.Drift` —el
      mismo avance suavizado del rodado, sin giro ni caída— sobre la balsa de `N3_Escena33_Cruce`
      (0,12 del ancho a la orilla derecha, 9 s, desde la línea 0): no hizo falta
      `RiverCrossingSequence.cs`. `env_final_fogatas.png` ya era la ilustración de la escena final
      y sus seis paradas cierran sobre la fogata central (0,5 · 0,35 de la lámina): ningún encuadre
      cambió. EditMode: `GameFlow_INC39_TrasElResumenDelUltimoNivelLaEscenaFinalSaleALosCreditosYAlInicio`,
      `LevelSummary_RF45_ElResumenDelNivel3NoContieneNingunDigito`,
      `LevelSummary_RF12_NombraLaDescomposicionYLaDepuracion`, y
      `NarrativeVisitPolicy_CP07_ElCruceYLaEscenaFinalNoSeOmitenLaPrimeraVez` exige ahora
      `EndsInCredits` solo en la final. PlayMode (`GameEndingTests`, en `Levels/River` y no en
      `Core` como decía el plan, porque el cruce lo dispara el panel de ensamblaje):
      `GameEnding_INC39_RecorreLevelSummaryNarrativeCreditsYMainMenu` (Boot → N3 fase 3 → cruce →
      resumen → escena final sin «Omitir» → créditos → «Volver» → inicio, perfil íntegro en disco) y
      `GameEnding_RF44_LaPruebaSuperadaReproduceElCruceYElCierre` (balsa completa → cruce; la balsa
      avanza sin girar; el cierre llega con las tres fases), **2/2**. Regresión con `unity test`
      (batchmode, Editor cerrado): EditMode **279/279**; River PlayMode 17/26 y UI+Core 68/71 —
      los diez que fallan son los visuales y de disposición que miden contra la Game View, que
      batchmode abre a 640×480 y sin captura (`AssemblyPanel_RNF03`, `AssemblyPanel_HU12`,
      `RiverScene_RF35_…Limites`, `RiverScene_RNF03`, `RiverLevel_RNF19/RNF20`,
      `RiverScene_RF36/RF39`, `NarrativeScene_RNF01`): son los mismos que pasaron 24/24 a las
      13:05 en el Editor. **Pendiente: repetir esos diez en el Editor con Rider** antes del
      checkpoint R-E.

### ✅ Checkpoint R-E — Slice 3 completo
- [x] **Dos recorridos completos** del Nivel 3 sin incidencias (RNF-13) — 21/09/2026,
      `RiverLevelJourneyTests` (desde `Boot` con perfil real: puente II → llegada → orilla →
      recolección → zona → base → amarre → mástil y vela → prueba → cruce → resumen → escena
      final → créditos → inicio; narrativas línea a línea con «Continuar»; comprueba en disco las
      tres fases, intentos y errores corregidos de la fase 3 y el tiempo de la base):
      `RiverLevel_RNF13_RecorreElNivel3CompletoHastaElInicio_{AcertandoAlPrimerIntento,FallandoLaPrueba}`,
      **2/2** con `unity test` (Editor cerrado). El clic manual sobre el ejecutable sigue siendo de Santiago.
- [x] Un recorrido **acertando al primer intento** (sin escena 3.2) y otro **fallando** (con ella) —
      son los dos casos de arriba: el que falla cruza mástil y vela, ve la 3.2, vuelve a la fase 3 con
      la escena recargada y las piezas devueltas, y corrige (Intentos = 1, Errores corregidos > 0)
- [x] Cierre forzado en cada fase confirmada → retoma donde iba (RNF-14) —
      `RiverLevel_RNF14_CierreForzadoTrasCadaFaseConfirmadaRetomaDondeIba_{TrasLaBase,TrasElAmarre}`
      (guarda, destruye runner y cargador, arranca de nuevo desde `Boot`, carga el perfil de disco y
      pide la fase 1: el flujo retoma en la pendiente, el panel abre en ella con las anteriores
      consolidadas y la recolección dada por hecha), **2/2**. Tras la fase 3 el nivel está completo y
      repetir la pedida es legítimo (`GameFlow_RNF14_…`)
- [x] **RNF-02 cerrado**: cinco escenas jugables inspeccionadas, cero teclado (INC-01) — R16, 21/09/2026
- [x] **RNF-16 cerrado**: las tres combinaciones de exclusión — R16, 21/09/2026
- [x] Carga de `Level3_River` < 10 s y memoria < 2 GB, **medidas** (RNF-04, RNF-05) — 21/09/2026,
      `RiverLevel_RNF04_LaEscenaDelNivel3CargaEnMenosDeDiezSegundos`: **1,36 s** por
      `SceneLoader.LastLoadSeconds`; `RiverLevel_RNF05_LaMemoriaQuedaBajoDosGigasConElNivel3Cargado`:
      **1 226 MB reservados / 785 MB asignados** con el panel de ensamblaje abierto. Medidas en el
      Editor batchmode, que carga más que el ejecutable: son cota superior
- [x] Paquete < 500 MB con el arte de los **tres** slices (RNF-06) — 21/09/2026, `unity build
      --target StandaloneWindows64` con las once escenas: **217 MB** (`Algoritmia_Data` 150 MB,
      `UnityPlayer.dll` 36 MB, `DirectML.dll` 14 MB). Incluye la carpeta
      `My project_BurstDebugInformation_DoNotShip` (1 MB), que se quita del entregable a mano. El
      ejecutable **no se corrió** en esta sesión: `Datos/` junto al `.exe` (RNF-07/RNF-11) sigue
      siendo comprobación de Santiago. **Ojo:** la build dejó `SENTIS_ANALYTICS_ENABLED` en los
      símbolos de `Standalone` y `UnityConnectSettings.m_Enabled: 1` (revertidos), y mete
      `DirectML.dll` + `D3D12/` —los paquetes de IA del Editor entrando al ejecutable—; quitarlos
      del `manifest.json` es «preguntar primero» (RNF-08/RNF-10): decisión de Santiago
- [ ] **PG-05** verificado sobre los tres niveles — observación en la sesión con estudiantes, no una
      aserción: queda para Santiago
      — sigue abierta el 01/10/2026 por la decisión D-b (acta D10, §5): la observa Santiago con
      estudiantes de cuarto en los tres niveles con el guion H1 de `claudeDocs/tasks/OE4/Hoja-HUM.md`,
      en la misma sesión que H2 y precedida del consentimiento de los acudientes (RNF-12,
      `claudeDocs/tasks/OE4/Consentimiento-RNF12.md`); se marca con lo que devuelva su plantilla.
- [x] RF-35..RF-44 tienen cada uno al menos una prueba que los nombra (CT-10) — barrido sobre
      `Assets/Tests` el 21/09/2026: RF-35 (3), RF-36 (4), RF-37 (3), RF-38 (4), RF-39 (4), RF-40 (4),
      RF-41 (3), RF-42 (1), RF-43 (3), RF-44 (1)
- [ ] **Golden Path del juego entero**, de la pantalla de inicio a los créditos, en 20–40 minutos —
      el recorrido está automatizado por tramos (`LevelSummaryTests` N1, `WheelLevelJourneyTests` N2,
      `RiverLevelJourneyTests` N3 + cierre) pero el tiempo lo mide una persona jugando: de Santiago
      — sigue abierta el 01/10/2026 por la decisión D-b (acta D10, §5): la duración la cronometra
      Santiago con el guion H8 de `claudeDocs/tasks/OE4/Hoja-HUM.md`. Los tres recorridos de inicio a
      créditos que hizo el arnés sobre el ejecutable —GP1 (87 min) y GP2 (33 min) sobre rc1, GP3
      (16 min) sobre rc2— terminaron sin bloqueos, pero su duración no mide a un estudiante
      (`claudeDocs/tasks/OE4/OE4-Resultados.md`, §3 y §8.5).
- [x] Revisado con el usuario antes de abrir el Slice 4 — pendiente también en R-A..R-D. Antes de la
      revisión: repetir en el Editor con Rider (Game View 1920×1080) los trece de disposición/captura
      que batchmode no puede correr (ver R14 y R16)
      — revisado por Santiago el 30/09/2026 (acta D10, decisión D-a), tras la verificación de Claude
      del 01/10/2026: los trece de disposición y captura de R14 y R16 (`AssemblyPanel_RNF03_*`,
      `AssemblyPanel_HU12_*`, `RiverScene_RF35_*`, `RiverScene_RNF03_*`, `RiverLevel_RNF19_*`,
      `RiverLevel_RNF20_*`, `RiverScene_RF36_*`, `RiverScene_RF39_*`, `NarrativeScene_RNF01_*`,
      `ForestScene_RF26_*`, `MazeScene_RNF03_*` y `WorkshopScene_RNF03_*`) pasaron en el Editor con la
      Game View a 1920 × 1080 —por la API del pipeline (`editor.ps1`) en lugar de Rider— en la suite
      completa previa al build rc2: PlayMode 376/376 y EditMode 470 = 469 + 1 omitida
      (`claudeDocs/tasks/OE4/evidencias/suites/rc2-final/`). R-A a R-D están cerrados, y el juego
      entero se recorrió sobre el ejecutable sin incidencias bloqueantes: GP1 y GP2 sobre rc1, GP3
      sobre rc2 (`claudeDocs/tasks/OE4/OE4-Resultados.md`, §3 y §8.5).

---

## Assets visuales — `plan.md` §Assets visuales del Slice 3

Escenarios, props e interfaz **originales del proyecto**. **Los personajes no se rediseñan**:
Mamá es `A3` del Slice 1 —**obra derivada** con autorización concedida y mención obligatoria en
créditos (CT-09, RNF-23)— y `C2` solo genera su vista cenital **con sus rasgos copiados
literalmente**. Cada asset se registra en `CreditsContent.asset` (Slice 1, T08).

**Cinco bloques fijos por prompt**, copiados palabra por palabra antes de la descripción:
`[1 CONTEXTO] [2 ESTILO] [3 PALETA] [4 ENTREGA] [5 PROHIBICIONES]`. Un asset generado sin los
cinco se descarta y se vuelve a pedir. La paleta y las especificaciones salen de
`claudeDocs/Direccion_de_Arte.md`.

**Todo el nivel se dibuja en vista cenital pura de 90 grados**, salvo `C10`, que es la
ilustración lateral de la escena final.

**Chroma:** verde `#00FF00` por defecto; **magenta `#FF00FF`** en `C2`, porque el personaje se
recorta sobre un entorno de follaje. Los fondos de escena no llevan chroma.

- [x] **C1 · Escenario del río, vista superior** — chroma **no** — RF-35, RF-39, guion §8/§8.2
      *(01/10/2026: Decisión — el Nivel 3 es un plano fijo a ras de suelo con perspectiva por
      profundidad, no una vista cenital (INC-66; guion §1.8, fila «Escenario»): todo el nivel se
      juega y se narra sobre `env_n3_rio.png` (1920×1080), entregado y aprobado en el acta D06, sin
      el ámbar de interacción en el decorado. Lo vigila
      `RiverScene_DA83_MamaYLosMaterialesSeVenMasGrandesCuantoMasAbajoEstan`, en verde en la suite
      completa del 01/10/2026.)*
- [x] **C2 · Mamá vista superior, cuatro direcciones** — chroma **magenta** — RF-35, CU-09, HU-11
      ⚠️ pegar el bloque «RASGOS FÍSICOS FIJOS» de `A3` (Slice 1) literalmente
      *(01/10/2026: Decisión — con el plano a ras de suelo (INC-66), en el río Mamá es su rig
      frontal (INC-53) dentro de `Personaje_Mama` y camina con un solo clip que se voltea a
      izquierda o derecha; las cuatro vistas cenitales no se producen y `char_mama_cenital.png`
      queda de reserva. Lo vigila
      `RiverScene_DA133_MamaCaminaMientrasSeSostieneUnaFlechaYReposaAlSoltarla`, en verde en la
      suite completa del 01/10/2026.)*
- [x] **C3 · Botones de dirección y botón «Recoger»** — chroma verde — **RF-35**, RF-37, RNF-02,
      RNF-19, **INC-01**
      *(01/10/2026: Decisión — Santiago aceptó el 30/09/2026 el conjunto genérico de interfaz como
      arte final (acta D10, §5): la cruceta es `ui_flecha` sobre `ui_boton` teñidos, abajo a la
      derecha y con clic sostenido (INC-91), sin láminas `ui_n3_dir_*` ni `ui_n3_recoger_*`, y
      «Recoger» se distingue por aparecer y desaparecer, no por color. Lo vigilan
      `RiverScene_RF35_ElPersonajeSeDesplazaConLosBotonesEnPantalla`,
      `RiverScene_INC01_NoExisteVinculacionDeTecladoEnElMapaDeControles` y
      `Collectible_RF37_ElBotonRecogerSoloApareceDentroDelRadioDeProximidad`, en verde en la suite
      completa del 01/10/2026.)*
- [x] **C4 · Los cuatro materiales + sus iconos de inventario** — chroma verde — RF-37, RF-38, RNF-19
      *(01/10/2026: Implementación — `prop_n3_tronco`, `_sogas`, `_tela` y `_mastil` son definitivos
      (`44fd479`, 25/09), sin croma, y hacen también de icono de inventario
      (`N3_RiverLevelConfig.asset`). Con el plano abierto del 30/09 (INC-118, D10-3) se ven a ≈×1,8
      y la 3.1 pinta el tronco y el mástil de la mecánica
      (`RiverLevelConfig_INC118_LaEscena31PintaCadaMaterialConElArteDeLaOrilla`, en verde en la
      suite completa del 01/10/2026); la captura `RiverScene_RF36_OrillaAlAbrir` se revisó en la
      revisión de W2 («Cumple») y los materiales se ven también en el ejecutable
      (`claudeDocs/tasks/OE4/evidencias/GP1/PF-RF37-01_sin_recoger_lejos.png`).)*
- [x] **C5 · Lista de tareas e inventario** — chroma verde — RF-36, RF-38, RNF-19, RNF-20, INC-41
      *(01/10/2026: Decisión — INC-46 fijó la lista como una tablilla de marfil con un círculo liso
      para lo pendiente (`ui_circulo`) y un círculo verde con visto para lo hecho
      (`ui_n3_casilla_hecha`), y el inventario como un panel arena en rejilla de 2×2 (dirección de
      arte §10.2), así que `ui_n3_lista_marco` y `ui_n3_inventario` no se producen. Lo vigilan
      `RiverLevel_RNF19_LaListaDeTareasSeLeeEnEscalaDeGrises` y
      `RiverLevel_RNF20_ContrasteSuficienteSobreElEscenarioClaro`, en verde en la suite completa del
      01/10/2026.)*
- [x] **C6 · Zona de construcción, dos estados** — chroma verde — RF-39, CU-09 (FA-6a)
      *(01/10/2026: Decisión — Santiago aceptó el 30/09/2026 (acta D10, §5) el anillo ámbar
      discontinuo `env_n3_zona_disponible` como arte definitivo de la zona, y basta un estado porque
      RF-39 solo pide señalizarla. Con la zona ≈×1,5 del plano abierto (D10-3) sigue a la vista
      junto al agua: `BuildZone_RF39_AbreElPanelSoloConLosCuatroMateriales` en verde en la suite
      completa del 01/10/2026 y revisada la captura `RiverScene_RF39_ZonaAbierta` de
      `RiverScene_RF39_CapturaConTodoRecogidoYLaZonaAbierta`.)*
- [x] **C7 · La balsa en tres estados: base / amarre / mástil y vela** — chroma verde — RF-40,
      RF-41, HU-12, guion §8.3
      *(01/10/2026: Decisión — la balsa no son tres láminas sino diecisiete espacios que se pintan
      uno a uno con ocho sprites (R11, decisión de Santiago del 20/09; INC-89):
      `prop_n3_{tronco,amarre,mastil,vela}` y sus `_silueta`, definitivos en 3/4 (`44fd479`, 25/09).
      Con el plano abierto la balsa del ensamblaje mide lo que la del cruce
      (`RaftAssemblyContent_RF44_LaBalsaDelEnsamblajeMideLoQueLaDelCruce`, en verde en la suite
      completa del 01/10/2026) y las capturas `AssemblyPanel_HU12_*` de las tres fases se revisaron.
      Pruebas: `AssemblyPanel_HU12_LaBalsaReflejaLasTresEtapasDeAvance` y
      `AssemblyPanel_RF40_MuestraSoloLosEspaciosDeLaFaseActiva`.)*
- [x] **C8 · Panel de ensamblaje: espacio vacío / correcto / incorrecto** — chroma verde — RF-40,
      RF-42, **RNF-19**, HU-13
      *(01/10/2026: Decisión — desde R11 el panel es una sombra negra al 30 % sin marco: el espacio
      vacío es la silueta de la pieza, el correcto la pieza y el incorrecto la pieza con `ui_alerta`
      encima, así que no se producen láminas de panel ni de espacio. Probar antes de la última fase
      señala solo lo mal puesto (INC-122):
      `AssemblyPanel_RF42_ProbarAntesDeTiempoSenalaSoloLoMalPuesto` en verde en la suite completa
      del 01/10/2026. Pruebas: `AssemblyPanel_RNF19_ElEspacioIncorrectoSeResaltaConColorEIcono` y
      `RiverLevel_RNF19_LosEstadosDeErrorSeLeenSinColor`.)*
- [x] **C9 · Balsa hundiéndose y balsa cruzando** — chroma verde — RF-42, RF-44, RNF-21, guion §8.4/§8.5
      *(01/10/2026: Decisión — Santiago aceptó el 30/09/2026 (acta D10, §5) la balsa hundida
      dibujada por código (`prop_n3_balsa_hundida`, la de la 3.2) como arte definitivo; la que cruza
      es `prop_n3_balsa_cruzando`, definitiva desde el 25/09, y en la mecánica el hundimiento lo
      anima el motor. La balsa que se hunde suena (`sfx_n3_hundimiento`, INC-123) y la del cruce
      navega bajo la espuma (INC-124):
      `AssemblyPanel_RF42_LaBalsaQueSeHundeSuenaUnaSalpicaduraYNadaMas`,
      `NarrativeSequence_RF44_LaBalsaDelCruceNavegaBajoLaEspumaDeLaCascada` y
      `NarrativeScene_RNF21_LaBalsaCruzaSinSaltosNiParpadeos`, en verde en la suite completa del
      01/10/2026. La balsa del cruce tiene 4 troncos y la mecánica arma 5: queda documentado, con el
      arreglo de redibujarla con cinco en una entrega del carril de arte (decisión D-OBS,
      `claudeDocs/tasks/OE4/OE4-Resultados.md` §8.9).)*
- [x] **C10 · Escenario de la escena final, las fogatas** — chroma **no** — RF-44, RF-12, guion §9
      *(01/10/2026: Implementación — `env_final_fogatas.png` (1920×1080, vista lateral) se entregó y
      aprobó en el acta D06 y su versión vigente es de `8ec5120` (22/09); la hoguera central es
      animada y echa humo (`d3a6cc9`, 25/09), lo que vigila
      `NarrativeSequence_RF05_CadaLlamaEchaHumoPorDetrasYPorEncima`, y `N3_EscenaFinal` está entre
      las secuencias que comprueba
      `NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo`; las dos, en verde en la
      suite completa del 01/10/2026.)*
- [x] Postproceso: recorte del verde, alfa, halo, **mismo `Pixels Per Unit` que los Slices 1 y 2**
      — cerrado el 01/10/2026: `arte_check.py sprites` sobre los 13 PNG de `Props/River`, más
      `env_n3_zona_disponible` y `ui_n3_casilla_hecha`: todos RGBA con transparencia, sin halo de
      croma intenso —en `prop_n3_amarre`, 11 px tenues de α ≤ 32, informativos— y con el `.meta`
      esperado, Sprite · Single · 100 PPU · Bilinear, el mismo de los Slices 1 y 2
      (`claudeDocs/tasks/OE4/evidencias/arte/sprites.md`).
- [x] Desaturar `C3`, `C4`, `C5` y `C8` y verificar que se siguen distinguiendo (RNF-19)
      — cerrado el 01/10/2026: en escala de grises (`arte_check.py rnf19`: 1−IoU ≥ 0,20 o Δ gris
      ≥ 25) los cuatro materiales de C4 se separan por forma y gris (1−IoU 0,70–0,95); en C5, tarea
      hecha y pendiente, Δ 41,0; en C8, silueta y pieza, Δ 36–65 en los sprites y 46–47 en captura, y
      vacío frente a incorrecto, Δ 24,8, lo distingue el icono de alerta (5,6:1, `rnf20-w3.md`)
      (`claudeDocs/tasks/OE4/evidencias/arte/rnf19.md` y `rnf19-w3.md`). C3, sobre la captura en grises del ejecutable
      `claudeDocs/tasks/OE4/evidencias/GP1/PF-RNF19-01_lista_N3_gris.png` (GP1, rc1): las cuatro
      flechas de la cruceta se distinguen por la dirección del glifo, sin depender del color, y
      «Recoger» por su rótulo y porque solo aparece dentro del radio de un material
      (`GP1/PF-RF37-01_sin_recoger_lejos.png` frente a `GP1/PF-RF35-01_borde_izquierdo_recoger.png`). Lo vigilan además `RiverLevel_RNF19_LaListaDeTareasSeLeeEnEscalaDeGrises` y
      `AssemblyPanel_RNF19_ElEspacioIncorrectoSeResaltaConColorEIcono`, en verde en la suite de rc2
      (`claudeDocs/tasks/OE4/evidencias/suites/rc2-final/`).
- [x] Verificar RNF-20 sobre el arte final: escenario claro y cenital, el caso más expuesto
      *(01/10/2026: Implementación — el escenario es el plano a ras de suelo de `env_n3_rio`, no
      cenital (INC-66). `RiverLevel_RNF20_ContrasteSuficienteSobreElEscenarioClaro` exige que ningún
      texto vaya directo sobre la ilustración y ≥ 4,5:1 contra su cara real en la orilla, «Recoger»,
      las tareas, «Listo» y la pausa y en «Probar balsa» de la base y el amarre (D10-3), a 1920×1080
      con el plano abierto, en verde en la suite completa del 01/10/2026.)*
- [x] Verificar RNF-21 sobre las **animaciones** montadas con `C7` y `C9`, no sobre las láminas
      *(01/10/2026: Implementación — `RiverLevel_RNF21_NingunaAnimacionDelNivel3TieneDestellos`
      muestrea cuadro a cuadro el empuje de cámara, el pulso de fase aprobada (C7) y el hundimiento
      (C9); el cruce de la 3.3 lo muestrea `NarrativeScene_RNF21_LaBalsaCruzaSinSaltosNiParpadeos`
      (D10-3), y el hundimiento de la prueba anticipada (INC-122) reusa el de la última fase y sigue
      sin destellos: `AssemblyPanel_RF42_ProbarLaBalsaIncompletaLaHundeYSigueEnLaFase` comprueba que
      gira y vuelve derecha; las dos pruebas, en verde en la suite completa del 01/10/2026.)*

---

## Bloqueantes y decisiones pendientes

- [x] **R2 · Cerrar los Slices 1 y 2** antes de abrir R02. Bloqueante duro.
      *(01/10/2026: Decisión — se cumplió: R02 se abrió el 16/09 con el código de los Slices 1 y 2
      cerrado, que el acta D06 (15/09) dio por terminado al fusionar la rama del Slice 2 (aviso del
      16/09 al principio de este tablero). Lo que sigue abierto en esos tableros es de ejecutable o
      de revisión y se cierra en cada uno.)*
- [x] **Pregunta abierta 1 · ¿Qué es una «fase» del Nivel 3 para el guardado?** CU-09/CU-10 dicen
      dos; RF-40 y §3.6.1 dicen tres de ensamblaje. Propuesta: cuatro puntos de guardado
      —recolección, base, amarre, mástil y vela— con `Pasos utilizados` contando solo los tres de
      ensamblaje. **Cambiar el formato de datos persistidos es «preguntar primero».** Bloquea R02.
      *(01/10/2026: Decisión — resuelta con Santiago el 16/09: tres fases —base, amarre, mástil y
      vela— y la recolección no se guarda, sin cambiar el formato persistido
      (`PhaseId.PhasesPerLevel = {1, 3, 3}`, R02); el guion lo recoge en CU-09. Lo vigilan
      `SaveStore_RF04_ConfirmarUnaFaseDelNivel3SobreviveAlCierre` y
      `RiverLevel_RNF14_CierreForzadoTrasCadaFaseConfirmadaRetomaDondeIba_*`, en verde en la suite
      completa del 01/10/2026.)*
- [x] **R1 · Instalar el servidor MCP de Unity** (`run_unity_tests`). Siete tareas `MCP` en el
      slice que cierra el juego.
      *(01/10/2026: Implementación — el MCP de Rider está instalado desde el 06/09 y corrió las
      suites del 20 y el 21/09 con el Editor abierto (R12, R15); con Rider cerrado las pruebas van
      por `unity test` (P00 del Slice 4, 23/09) y, desde el 30/09, por
      `claudeDocs/tasks/OE4/herramientas/editor.ps1` (`649b4d6`) sobre la API de
      `com.unity.pipeline`, con la que se corrió la suite completa del 01/10/2026.)*
- [x] **Pregunta abierta 2 · Trazado del escenario del río.** Validar `N3_RiverLevelConfig.asset`
      jugando: ningún material visible desde la posición inicial, zona de construcción señalizada
      desde el principio.
      *(01/10/2026: Decisión — el plano fijo de la recolección (decisiones de Santiago del 20/09 y
      del 25/09; INC-66) sustituyó «ningún material visible desde la posición inicial»: los
      materiales se ven desde el arranque, pero ninguno queda al alcance y el reparto obliga a
      recorrer la orilla; la zona se ve desde el principio (RF-39). Con el plano abierto del 30/09
      (INC-118) lo vigilan
      `RiverLevelConfig_RF37_ElRepartoObligaARecorrerLaOrillaYCabeEnElPlanoFijo`, con sus dos
      aserciones nuevas —ningún alcance de material toca la zona y Mamá no arranca dentro de ella—,
      y `RiverLevelConfig_Guion82_ElPlanoDeRecoleccionEsLaOrillaConElRio`, en verde en la suite
      completa del 01/10/2026.)*
      — revisado por Santiago el 30/09/2026 (acta D10, decisión D-a), tras la verificación de Claude
      del 01/10/2026: en el ejecutable, al abrir la orilla los materiales se ven pero ninguno queda al
      alcance —no aparece «Recoger»—, y la zona de construcción ya está señalizada
      (`claudeDocs/tasks/OE4/evidencias/GP1/PF-RF37-01_sin_recoger_lejos.png`). PF-RF37-01 y
      PF-RF39-01 están en P sobre rc1 y GP3 los confirma sobre rc2
      (`claudeDocs/tasks/OE4/OE4-Resultados.md` §5 y §8.5).
- [x] **Pregunta abierta 3 · Radio de proximidad de RF-37**, sin validar. Revisar en R-C.
      *(01/10/2026: Decisión — se revisó en el Checkpoint R-C: el radio es 0,06 de la ilustración
      con el plano abierto del 30/09 (INC-118, D10-3) ≈161 px en pantalla a 1920×1080 (plano ×1,4),
      con `Collectible_RF37_ElBotonRecogerSoloApareceDentroDelRadioDeProximidad` y
      `RiverLevelConfig_RF37_ElRepartoObligaARecorrerLaOrillaYCabeEnElPlanoFijo` en verde en la
      suite completa del 01/10/2026; revisado por Santiago el 30/09/2026 (acta D10, §5).)*
- [x] **Pregunta abierta 6 · ¿Se adelantan R09 y R10?** Recomendación: sí, para descargar INC-30.
      *(01/10/2026: Decisión — sin objeto: R09 y R10 se hicieron el 20/09, después de R05 a R08
      (17/09). INC-30 está cerrado y lo vigilan `TaskList_INC30_LaFaseDeBaseNoMarcaTareaPorSiSola` y
      `RaftAssembly_INC30_ConfirmarElAmarreMarcaLaTarea3YLaBaseNoMarcaNada`, en verde en la suite
      completa del 01/10/2026.)*
- [x] **PG-02 · nombre del guía.** Última oportunidad antes de la entrega: la escena final lo nombra.
      *(01/10/2026: Decisión — PG-02 se cerró el 02/09 con «Algoritm» (INC-44; guion §1.2): la
      escena final habla como `ALGORITM` (`N3_EscenaFinal.asset`) y ningún asset de
      `Assets/Game/Data` usa «Chispa» como nombre del guía; las «chispas» que quedan son las del
      fuego del Nivel 1.)*
- [x] **PG-01 · título del producto.** Sigue abierto y el juego ya estaría completo (RF-01, RF-08). *(Cerrado el 29/09/2026: «Algoritmia», guion §1.2.)*
