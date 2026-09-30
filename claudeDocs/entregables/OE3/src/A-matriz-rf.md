# ANEXO A. MATRIZ DE TRAZABILIDAD DE REQUERIMIENTOS FUNCIONALES A PRUEBAS

Esta matriz no se redactó a mano: la generó el script `rf_matrix.py`, que acompaña a este documento, a partir de los nombres de los métodos de prueba del proyecto en el commit `ccf77e6` (25/09/2026). Aplica la regla CT-10 del proyecto, según la cual el nombre de cada prueba sigue el patrón `<Sujeto>_<Requisito>_<QuéHace>` y el identificador del medio es la trazabilidad. El script recorre los archivos de prueba de las carpetas EditMode y PlayMode, toma cada método marcado con `[Test]` o `[TestCase]` y lo asigna al RF que su nombre cita. Los nombres de los requerimientos se toman de la tabla de requerimientos funcionales del documento de solución del objetivo específico 1.

La columna «Pruebas» de la Tabla A.1 cuenta métodos, no casos: un método parametrizado con varios `[TestCase]` cuenta una vez, y una prueba que verifica un RF sin citarlo en su nombre no se cuenta. Entre paréntesis separa los métodos del modo EditMode («EM», lógica en C# sin escena) de los del modo PlayMode («PM», escenas reales cargadas). La columna «Módulos» indica los módulos de pruebas donde viven esos métodos, ordenados de más a menos pruebas. La Tabla A.2 muestra, como ejemplo, una de las pruebas de cada RF: la de nombre más corto entre las que verifican su comportamiento, dejando atrás las que solo comprueban el sonido o toman capturas.

De los 567 métodos de prueba del proyecto, 314 citan un RF en su nombre y cubren 47 de los 47 RF. De los 253 restantes, 252 citan otro identificador: requerimientos no funcionales (RNF), 128; criterios pedagógicos (CP), 36; secciones de la dirección de arte, 33; hallazgos de consistencia (INC), 19; historias de usuario (HU), 15; casos de uso (CU), 8; secciones del guion, 6; restricciones técnicas (CT), 3; definición operativa de los indicadores de OE1, 3; puntos abiertos del guion (PG), 1. El otro no lleva identificador en su nombre: lo lleva en el de cada uno de sus casos parametrizados (véase el documento principal, §9.1). Que todo RF tenga al menos una prueba que lo nombre lo comprueba además, de forma automática, la prueba `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra` en cada corrida de la suite.

**Tabla A.1.** Pruebas automatizadas que nombran cada requerimiento funcional (RF-01 a RF-47) en el commit `ccf77e6`, por modo y por módulo.

| RF | Requerimiento | Pruebas | Módulos |
|----|--------------------|----------|--------------------|
| RF-01 | Pantalla de inicio | 3 (0 EM, 3 PM) | UI, Core |
| RF-02 | Registro de perfil de jugador | 7 (6 EM, 1 PM) | Core |
| RF-03 | Menú de niveles con desbloqueo progresivo | 15 (9 EM, 6 PM) | Core, UI |
| RF-04 | Guardado automático del progreso | 7 (5 EM, 2 PM) | Core, Levels.Wheel |
| RF-05 | Reproducción de escenas narrativas | 57 (34 EM, 23 PM) | Scaffolding, UI, Core, Audio, Levels.Wheel, Levels.River |
| RF-06 | Avance y omisión de diálogos | 5 (5 EM, 0 PM) | Scaffolding |
| RF-07 | Pausa y reinicio de nivel | 6 (6 EM, 0 PM) | Core, Levels.Fire, Levels.River, Levels.Wheel |
| RF-08 | Pantalla de créditos | 3 (1 EM, 2 PM) | UI, Core |
| RF-09 | Salida controlada | 3 (2 EM, 1 PM) | Core, UI |
| RF-10 | Presentación del objetivo del nivel | 2 (0 EM, 2 PM) | UI |
| RF-11 | Retroalimentación inmediata no punitiva | 2 (1 EM, 1 PM) | Audio, Levels.Wheel |
| RF-12 | Cierre reflexivo del nivel | 3 (3 EM, 0 PM) | UI |
| RF-13 | Pista progresiva | 12 (7 EM, 5 PM) | Scaffolding, Levels.Fire, Levels.River, Levels.Wheel |
| RF-14 | Panel de encendido | 10 (3 EM, 7 PM) | Levels.Fire |
| RF-15 | Control de posición como hipótesis | 6 (3 EM, 3 PM) | Levels.Fire |
| RF-16 | Acción de golpear | 9 (7 EM, 2 PM) | Levels.Fire |
| RF-17 | Registro de retroalimentación cualitativa | 5 (4 EM, 1 PM) | Levels.Fire, Levels.River, Levels.Wheel, UI |
| RF-18 | Intentos ilimitados | 3 (3 EM, 0 PM) | Levels.Fire |
| RF-19 | Desbloqueo condicional de la acción de soplar | 3 (1 EM, 2 PM) | Levels.Fire |
| RF-20 | Resolución del nivel | 4 (0 EM, 4 PM) | Levels.Fire, UI |
| RF-21 | Iluminación progresiva del escenario | 1 (0 EM, 1 PM) | Levels.Fire |
| RF-22 | Escenario de exploración | 14 (5 EM, 9 PM) | Levels.Wheel, Core |
| RF-23 | Selección de objetos por patrón | 10 (8 EM, 2 PM) | Levels.Wheel, UI |
| RF-24 | Contador de recolección | 7 (3 EM, 4 PM) | Levels.Wheel |
| RF-25 | Colocación de la carga | 6 (3 EM, 3 PM) | Levels.Wheel |
| RF-26 | Demostración del rodado | 12 (7 EM, 5 PM) | Levels.Wheel, Scaffolding, UI |
| RF-27 | Área de trabajo de construcción | 1 (0 EM, 1 PM) | Levels.Wheel |
| RF-28 | Acción de mecanizado | 4 (2 EM, 2 PM) | Levels.Wheel |
| RF-29 | Ensamblaje secuencial de la carretilla | 6 (2 EM, 4 PM) | Levels.Wheel |
| RF-30 | Escenario de laberinto | 4 (1 EM, 3 PM) | Levels.Wheel |
| RF-31 | Editor de bloques de instrucciones | 7 (4 EM, 3 PM) | Levels.Wheel |
| RF-32 | Ejecución de la secuencia | 2 (1 EM, 1 PM) | Levels.Wheel |
| RF-33 | Validación por retroceso | 2 (2 EM, 0 PM) | Levels.Wheel |
| RF-34 | Edición y reintento de la secuencia | 4 (1 EM, 3 PM) | Levels.Wheel |
| RF-35 | Movimiento del personaje | 4 (1 EM, 3 PM) | Levels.River |
| RF-36 | Lista de tareas visible | 3 (2 EM, 1 PM) | Levels.River |
| RF-37 | Interacción por proximidad | 4 (3 EM, 1 PM) | Levels.River |
| RF-38 | Inventario limitado | 4 (4 EM, 0 PM) | Levels.River |
| RF-39 | Zona de construcción | 3 (1 EM, 2 PM) | Levels.River |
| RF-40 | Ensamblaje por fases bloqueantes | 5 (3 EM, 2 PM) | Levels.River |
| RF-41 | Confirmación de fase | 4 (3 EM, 1 PM) | Core, Levels.River |
| RF-42 | Prueba de la balsa y depuración | 1 (1 EM, 0 PM) | Levels.River |
| RF-43 | Devolución de objetos tras el fallo | 3 (3 EM, 0 PM) | Levels.River |
| RF-44 | Animación de cruce | 4 (1 EM, 3 PM) | Levels.River, UI |
| RF-45 | Registro de indicadores y resumen al finalizar el nivel | 19 (19 EM, 0 PM) | UI, Levels.Fire, Levels.River, Levels.Wheel |
| RF-46 | Consulta del progreso por el docente | 8 (4 EM, 4 PM) | UI, Reporting, Core |
| RF-47 | Eliminación de datos del jugador | 7 (3 EM, 4 PM) | UI, Core |

**Tabla A.2.** Una prueba de ejemplo por requerimiento funcional en el commit `ccf77e6`.

| RF | Ejemplo de prueba |
|----|------------------------------------------|
| RF-01 | `BootFlow_RF01_ArrancaEnBootYLlegaSoloAMainMenu` |
| RF-02 | `PlayerProfile_RF02_RechazaNombreVacioYDuplicado` |
| RF-03 | `GameFlow_RF03_NoPermiteEntrarANivelBloqueado` |
| RF-04 | `PhaseId_RF04_CadaNivelDeclaraCuantasFasesTiene` |
| RF-05 | `DialogueRunner_RF05_AvanzaUnaLineaPorClic` |
| RF-06 | `DialogueRunner_RF06_NoOfreceOmitirLaPrimeraVez` |
| RF-07 | `FireIndicators_RF07_LaPausaNoSumaTiempoDeResolucion` |
| RF-08 | `Credits_RF08_VolverRegresaAlMenuPrincipal` |
| RF-09 | `MainMenu_RF09_GuardaElPerfilActivoAntesDeCerrar` |
| RF-10 | `NarrativeScene_RF10_TerminarLaEscena21EntraAJugarLaFase1DelBosque` |
| RF-11 | `WheelLevelConfig_RF11_ElAciertoDevuelveLaPreguntaQueAbreElPatron` |
| RF-12 | `LevelSummary_RF12_NombraLaHabilidadEjercitada` |
| RF-13 | `HintPolicy_RF13_AyudaADemandaNoAlteraElEstado` |
| RF-14 | `FireArrangement_RF14_SinPiezasNoHayNadaReunido` |
| RF-15 | `FireAttempt_RF15_CambiarLaFuerzaNoAlteraElEstado` |
| RF-16 | `StoneSpacing_RF16_ElGolpeCerteroEsSoloLaMuescaCinco` |
| RF-17 | `RaftValidator_RF17_NingunMensajeContieneDigitos` |
| RF-18 | `FireAttempt_RF18_AceptaIntentosIlimitados` |
| RF-19 | `FirePanel_RF19_SoplarAtenuadoNoRespondeNiRegistraError` |
| RF-20 | `FireLevel_RF20_SoplarEncadenaAnimacionYEscenaDeCierre` |
| RF-21 | `FireLevel_RF21_IluminacionSubeUnEscalonPorGolpeEfectivo` |
| RF-22 | `ForestObjectNudge_RF22_LaHojaDejaDeMoverseAlPosarse` |
| RF-23 | `NarrativeScene_RF23_LaEscena22AnimaLoQueCadaLineaCuenta` |
| RF-24 | `ForestScene_RF24_ElContadorEsVisibleDuranteTodaLaFase` |
| RF-25 | `ForestScene_RF25_LaCajaNoSeArrastraSinLosCincoTroncos` |
| RF-26 | `RollMotion_RF26_SinTiempoDeCaidaLaCajaSoloRueda` |
| RF-27 | `WorkshopScene_RF27_PresentaLasSeisPiezas` |
| RF-28 | `AssemblySequence_RF28_MecanizarExigeTroncoCortoSeleccionado` |
| RF-29 | `WorkshopScene_RF29_LaSecuenciaCompletaConfirmaYGuardaLaFase2` |
| RF-30 | `MazeScene_RF30_ElLaberintoEsAlAtardecer` |
| RF-31 | `CartState_RF31_RetrocederNoCambiaLaOrientacion` |
| RF-32 | `SequenceExecutor_RF32_RecorreLaSecuenciaPasoAPasoResaltandoElBloqueEnCurso` |
| RF-33 | `MazeGrid_RF33_UnMovimientoInvalidoDevuelveALaCasillaAnterior` |
| RF-34 | `MazeScene_RF34_SoltarSobreLaSecuenciaEnganchaYSoltarFueraRetira` |
| RF-35 | `RiverScene_RF35_ElPersonajeNoAtraviesaLosLimitesDelEscenario` |
| RF-36 | `TaskList_RF36_Tareas1y2SeMarcanAlRecogerTroncosYSogas` |
| RF-37 | `Inventory_RF37_UnMaterialNoSePuedeRecogerDosVeces` |
| RF-38 | `Inventory_RF38_CapacidadEsCuatroYNoHayObjetosSobrantes` |
| RF-39 | `BuildZone_RF39_AbreElPanelSoloConLosCuatroMateriales` |
| RF-40 | `AssemblyPanel_RF40_MuestraSoloLosEspaciosDeLaFaseActiva` |
| RF-41 | `PlayerProfile_RF41_UnaFaseConfirmadaNoSePierdeAlVolverAJugarla` |
| RF-42 | `RaftValidator_RF42_IdentificaElEspacioIncorrectoNoElEnsamblajeCompleto` |
| RF-43 | `TaskList_RF43_UnaTareaMarcadaNoSeDesmarcaTrasUnaPruebaFallida` |
| RF-44 | `GameEnding_RF44_LaPruebaSuperadaReproduceElCruceYElCierre` |
| RF-45 | `LevelSummary_RF45_NoContieneNingunDigito` |
| RF-46 | `MainMenu_RF46_OfreceLaOpcionDeProgresoDelDocente` |
| RF-47 | `SaveStore_RF47_NoAfectaAOtrosPerfiles` |

Todos los requerimientos funcionales tienen al menos una prueba que los nombra; no hay RF sin prueba.
