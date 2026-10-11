# PF-RNF16-01 · rc3 · pruebas en copias sin un nivel (10/10/2026)

Copia desechable `C:\Dev\Algoritmia-B` sincronizada con `C:\Dev\Algoritmia` (HEAD `f49ccc9`) por `robocopy /MIR` de Assets, Packages y ProjectSettings; se borraron `Levels/<Nivel>/` (código y pruebas EditMode y PlayMode), `Assets/Tests/EditMode/Content/` y las escenas del nivel de Build Settings. `unity test` con el Editor cerrado, EditMode y PlayMode. Registro: `registro-sinN2.txt`, `registro-sinN1.txt`; resultados NUnit `sinN*-EditMode.xml` y `sinN*-PlayMode.xml`.

## Sin N2 (Wheel) · EditMode: 663 pruebas, 657 pasan, 5 fallan, 1 omitidas, 0 no concluyentes

- `Architecture_RNF15_CadaModuloDeclaraSuAssemblyDeRuntimeYDePruebas` — estructura (exige los tres niveles / todos los RF)
- `Architecture_RNF15_ElNombreDelAssemblyCoincideConSuRutaBajoScripts` — estructura (exige los tres niveles / todos los RF)
- `Architecture_RNF16_NingunAssemblyDeNivelReferenciaAOtroNivel` — estructura (exige los tres niveles / todos los RF)
- `Architecture_RNF16_RetirarUnNivelNoAfectaALosOtrosDos` — estructura (exige los tres niveles / todos los RF)
- `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra` — estructura (exige los tres niveles / todos los RF)

## Sin N2 (Wheel) · PlayMode: 316 pruebas, 288 pasan, 18 fallan, 9 omitidas, 1 no concluyentes

- `Controls_RNF02_LasCincoEscenasJugablesSoloAceptanClicYClicSostenido` — esperada por el caso: carga por nombre una escena del nivel retirado
- `GameFlowRunner_RF22_JugarLaFase1DelNivel2CargaLaEscenaDelBosque` — esperada por el caso: carga por nombre una escena del nivel retirado
- `GameFlowRunner_RNF14_ConLasDosPrimerasFasesConfirmadasEntrarAlNivel2RetomaEnElLaberinto` — esperada por el caso: carga por nombre una escena del nivel retirado
- `AssemblyPanel_HU12_LaBalsaReflejaLasTresEtapasDeAvance` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `AssemblyPanel_RNF03_ConLaFase3AbiertaLaBalsaElBotonYLasTablillasCabenSinSolaparse` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `AssemblyPanel_RNF03_EnLaBaseLosDosBotonesLaBalsaYLasTablillasCabenSinSolaparse` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `RiverLevel_RNF19_LaListaDeTareasSeLeeEnEscalaDeGrises` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `RiverLevel_RNF19_LosEstadosDeErrorSeLeenSinColor` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `RiverLevel_RNF20_ContrasteSuficienteSobreElEscenarioClaro` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `RiverScene_RNF03_ControlesListaEInventarioCabenEnPantallaYNoSeSolapan` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `RiverScene_RF36_CapturaDeLaOrillaAlAbrir` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `RiverScene_RF39_CapturaConTodoRecogidoYLaZonaAbierta` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `Personajes_DA133_CapturaCadaMecanicaConSusPersonajes("Level2_Forest")` — esperada por el caso: carga por nombre una escena del nivel retirado
- `Personajes_DA133_CapturaCadaMecanicaConSusPersonajes("Level2_Maze")` — esperada por el caso: carga por nombre una escena del nivel retirado
- `Personajes_DA133_CapturaCadaMecanicaConSusPersonajes("Level2_Workshop")` — esperada por el caso: carga por nombre una escena del nivel retirado
- `NarrativeScene_RNF21_ElHaloDeLaFogataPulsaLentoYSinDestellos` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `NarrativeScene_RNF21_ElHaloDeLaFogataSeQuedaQuietoConLaPausa` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `NarrativeScene_RNF21_LaBalsaCruzaSinSaltosNiParpadeos` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)

## Sin N1 (Fire) · EditMode: 696 pruebas, 690 pasan, 5 fallan, 1 omitidas, 0 no concluyentes

- `Architecture_RNF15_CadaModuloDeclaraSuAssemblyDeRuntimeYDePruebas` — estructura (exige los tres niveles / todos los RF)
- `Architecture_RNF15_ElNombreDelAssemblyCoincideConSuRutaBajoScripts` — estructura (exige los tres niveles / todos los RF)
- `Architecture_RNF16_NingunAssemblyDeNivelReferenciaAOtroNivel` — estructura (exige los tres niveles / todos los RF)
- `Architecture_RNF16_RetirarUnNivelNoAfectaALosOtrosDos` — estructura (exige los tres niveles / todos los RF)
- `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra` — estructura (exige los tres niveles / todos los RF)

## Sin N1 (Fire) · PlayMode: 354 pruebas, 329 pasan, 23 fallan, 2 omitidas, 0 no concluyentes

- `Controls_RNF02_LasCincoEscenasJugablesSoloAceptanClicYClicSostenido` — esperada por el caso: carga por nombre una escena del nivel retirado
- `AssemblyPanel_HU12_LaBalsaReflejaLasTresEtapasDeAvance` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `AssemblyPanel_RNF03_ConLaFase3AbiertaLaBalsaElBotonYLasTablillasCabenSinSolaparse` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `AssemblyPanel_RNF03_EnLaBaseLosDosBotonesLaBalsaYLasTablillasCabenSinSolaparse` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `RiverLevel_RNF19_LaListaDeTareasSeLeeEnEscalaDeGrises` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `RiverLevel_RNF19_LosEstadosDeErrorSeLeenSinColor` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `RiverLevel_RNF20_ContrasteSuficienteSobreElEscenarioClaro` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `RiverScene_RNF03_ControlesListaEInventarioCabenEnPantallaYNoSeSolapan` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `RiverScene_RF36_CapturaDeLaOrillaAlAbrir` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `RiverScene_RF39_CapturaConTodoRecogidoYLaZonaAbierta` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `ForestScene_RF26_LaCajaYLosTroncosTerminanEnCuadroDondeLosDibujaLaEscena22` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `MazeScene_RNF03_ConMasBloquesDeLosQueCabenLaListaSeDesplazaSinEncogerLasFilas` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `WorkshopScene_RNF03_NadaSeSaleDePantallaNiSeSolapa` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `Personajes_DA133_CapturaCadaMecanicaConSusPersonajes("Level1_Cave")` — esperada por el caso: carga por nombre una escena del nivel retirado
- `LevelSummary_CP07_ElCierreReflexivoNoEsOmitibleLaPrimeraVez` — esperada por el caso: carga por nombre una escena del nivel retirado
- `LevelSummary_RF03_DevuelveAlMenuConNivel2Desbloqueado` — esperada por el caso: carga por nombre una escena del nivel retirado
- `NarrativeScene_RNF21_ElHaloDeLaFogataPulsaLentoYSinDestellos` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `NarrativeScene_RNF21_ElHaloDeLaFogataSeQuedaQuietoConLaPausa` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `NarrativeScene_RNF21_LaBalsaCruzaSinSaltosNiParpadeos` — ambiental: falla igual en la copia completa (15 de 15, resolución 640×480 y tiempos de `unity test`)
- `PauseMenu_HU17_ConfirmarReiniciarReinicioElIntentoSinTocarElProgreso` — esperada por el caso: carga por nombre una escena del nivel retirado
- `PauseMenu_HU17_ContinuarRestituyeElEstadoExacto` — esperada por el caso: carga por nombre una escena del nivel retirado
- `PauseMenu_HU17_OfreceExactamenteReanudarReiniciarYVolverAlMenuDeNiveles` — esperada por el caso: carga por nombre una escena del nivel retirado
- `PauseMenu_HU17_ReiniciarPideConfirmacionYCancelarNoCambiaNada` — esperada por el caso: carga por nombre una escena del nivel retirado

