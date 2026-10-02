# Slice 4: progreso, informe docente y eliminación de datos

Este anexo describe el cuarto incremento del prototipo, que cerró el módulo de progreso y registro de OE1 con el informe docente y la eliminación de datos, y el cierre del proyecto que siguió hasta el corte del 1 de octubre de 2026, con las corridas completas de la suite y las mediciones sobre el ejecutable. Sale del documento de resultados del cuarto slice (`claudeDocs/tasks/Slice 4/Slice-4-Resultados.md`), que el repositorio conserva como registro histórico con sus notas fechadas, de su tablero (`todo.md`), de los documentos del OE4 (`claudeDocs/tasks/OE4/`), de las actas D08 a D10 y del registro de inconsistencias (revisión 16). Sirve a los apartados 3.4 a 3.6, 6.4 y 7.1 del documento principal y a sus capítulos 8, 9 y 10.

## 1. Alcance y seguimiento

El incremento cubre el módulo F de OE1, «progreso-registro»: RF-45 («Registro de indicadores y resumen al finalizar el nivel»), RF-46 («Consulta del progreso por el docente») y RF-47 («Eliminación de datos del jugador»), con las historias HU-14, HU-16 y HU-18 y los casos de uso CU-11 y CU-12. Lo verifican además RNF-09 (lista cerrada de datos), RNF-11 (borrado sin residuos), RNF-16 (exclusión de niveles), CP-03 (sin cifras para el estudiante) y CT-10 (cada requerimiento con su prueba). El plan técnico lo organizó en trece tareas, P00 a P12, repartidas en cinco fases con los puntos de control P-A a P-E. Exportar el informe, graficar y comparar estudiantes quedaron fuera, porque RF-46 no los pide.

Santiago Valdiri García ejecutó el slice el 23 y el 24/09/2026 y lo entregó como un solo commit, `9c34924` («Slice 4», 24/09/2026), que la solicitud de integración n.º 85 fusionó en la rama principal (`7b08036`): 114 archivos, 57 nuevos y 57 modificados. Al cerrarse RF-46 y RF-47 quedaron implementados los 47 requerimientos funcionales, los 45 de prioridad alta y también RF-06 (media) y RF-21 (baja), que ya estaban implementados desde los slices anteriores (`NarrativeVisitPolicy` y `CaveLightingController`). Sobre el ejecutable del corte, ninguno de los casos de prueba funcional del OE4 ejecutados termina en fallo (documento principal, apartado 9.5). Del 29/09 al 01/10/2026, Santiago Benavides Rey llevó el cierre del proyecto sobre los mismos módulos: el plan de pruebas del OE4, las correcciones del cierre de inconsistencias, las decisiones del acta D10 y las correcciones que la evaluación funcional pidió para el ejecutable del corte.

**Tabla G.1.** Actas que gobernaron el incremento.

| Acta | Fecha | Qué decidió para este incremento |
|---|---|---|
| D08 | 23/09/2026 | Abre el cuarto slice a cargo de Santiago Valdiri García (tarjeta D08-1); confirma que el módulo del informe depende solo del núcleo; recuerda que las cifras existen únicamente en el informe docente y que el borrado se prueba sobre una carpeta temporal que la prueba crea y destruye |
| D09 | 24/09/2026 | Da por finalizada la tarjeta D08-1: el slice cerró y quedó fusionado, con los 47 requerimientos funcionales implementados y 321 de 321 pruebas de edición superadas |
| D10 | 30/09/2026 | Marca las revisiones con el usuario tras verificarlas con pruebas y capturas; acepta como definitivos los iconos de los indicadores; decide que el informe no lleve protección de acceso; encarga el formato de consentimiento (RNF-12), los dos recorridos automatizados sobre el ejecutable y las tarjetas D10-5 (registro de cargas) y D10-6 (arnés del ejecutable y perfiles semilla); deja ocho casillas a cargo de Santiago Benavides Rey |

La tarjeta que abrió el incremento es la D08-1, que pedía confirmar el corredor de pruebas, crear el módulo del informe con su frontera probada y cerrar el primer punto de control; el mensaje de `9c34924` no la cita, y la D09 la cerró. Las tarjetas D10-5 y D10-6, de Santiago Benavides Rey, cubren la instrumentación de las cargas y las herramientas de evaluación del ejecutable.

**Tabla G.2.** Commits del incremento y del cierre.

| Fecha | Commit | Autor | Qué entró |
|---|---|---|---|
| 24/09/2026 | `9c34924` (solicitud n.º 85) | Santiago Valdiri García | Módulo `Game.Reporting`, escena `TeacherReport`, borrado desde el informe y barridos de cierre |
| 29/09/2026 | `d105838` | Santiago Benavides Rey | Plan de pruebas del OE4; corrección del doble clic al salir de un cierre reflexivo (Anexo C, apartado 2.5) |
| 30/09/2026 | `37b3cb7` | Santiago Benavides Rey | Identidad del ejecutable, «Salir» con confirmación, «Completado» y arte real en los menús, informe con las tipografías y el mapa de controles del juego, textos en datos y desbloqueo por fases |
| 30/09/2026 | `5a22df1` | Santiago Benavides Rey | Documentos alineados con el juego (registro de inconsistencias, revisión 15) |
| 01/10/2026 | Árbol de trabajo del corte | Santiago Benavides Rey | Línea `RNF-04` por cada carga de escena (tarjeta D10-5) |
| 01/10/2026 | Árbol de trabajo del corte (ejecutable del corte) | Santiago Benavides Rey | Servicios de Unity apagados al compilar, guardado con temporal y reemplazo, aviso del perfil ilegible, panel de perfiles por páginas, «Cerrar el juego» en una línea y encabezado del informe con el perfil mirado |

`d105838`, `37b3cb7` y `5a22df1` entraron en la rama principal con la solicitud n.º 88 (`1c7f4ab`, 30/09/2026).

## 2. Lo implementado

### 2.1 Cimientos del módulo de informes

La primera tarea (P00) fijó el 23/09/2026 la línea de comandos `unity test` como corredor de pruebas principal. Desde el 30/09/2026 las suites completas se corren también con el Editor abierto mediante `editor.ps1`, un envoltorio en PowerShell sobre el servidor local de pruebas del Editor.

La segunda (P01) creó `Game.Reporting` con una sola referencia, `Game.Core`, y su módulo de pruebas de EditMode. Retirar un nivel no puede romper el informe docente, y lo vigilan dos pruebas negativas de arquitectura: `Architecture_RNF16_ReportingNoReferenciaANingunAssemblyDeNivel` y `Architecture_RNF16_RetirarUnNivelNoRompeElInformeDocente`. Los controladores de pantalla del informe se escribieron en `Game.UI`, donde ya vivían los del menú principal, la selección de perfil y el resumen de nivel, de modo que `Game.Reporting` quedó como dato y lógica, sin dependencia de la interfaz gráfica.

### 2.2 Resumen del estudiante

El resumen de fin de nivel no se rehízo (P02). Los tres primeros slices ya lo habían unificado sobre `LevelSummaryController` y `LevelSummaryComposer`, con su propia tablilla, y el trabajo se puso en un barrido transversal sobre los tres resúmenes reales. Cuatro pruebas de `LevelSummaryComposerTests` lo hacen: ningún resumen contiene un dígito (`LevelSummary_RF45_NingunResumenDeLosTresNivelesContieneUnDigito`), ninguno emite juicios de valor (`LevelSummary_RF17_NingunResumenEmiteJuicioDeValor`), ninguna oración pasa de veinte palabras y no existe en el proyecto ninguna clase de puntaje (`LevelSummary_CP03_NoExisteClaseDePuntajeEnElProyecto`).

El barrido de cierre de P11 (24/09/2026) extiende la regla a todo el contenido. `Content_CP03_NingunTextoVisibleAlEstudianteContieneCifrasDeDesempeno`, en el módulo de pruebas `Game.Content.Tests`, el único que referencia los tres niveles a la vez, recorre el grafo serializado de diez tipos de contenido: `LevelSummaryMessages`, `NarrativeSequence`, `GuideContent`, `FireMessages`, `WheelLevelConfig`, `RiverLevelConfig`, `RaftAssemblyContent`, `AssemblyContent`, `MazeLayout` y `GameTitleConfig`. Deja fuera a propósito `ReportContent`, la única pantalla donde una cifra es correcta, y `CreditsContent`, cuyo año de producción no mide desempeño. Salta los campos cuyo nombre termina en «Id» y resta los índices de formato (`{0}`, `{1}`) antes de buscar dígitos. En la misma tarea, `SaveStore_RNF09_ElJsonDeUnPerfilCompletoNoTieneCampoFueraDeLaListaCerrada` comprobó la lista cerrada de RNF-09 con un perfil que jugó los tres niveles, y `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra` deriva la matriz de los nombres de los métodos de prueba y confirma que los 47 requerimientos funcionales están citados.

### 2.3 Informe docente

El informe (P03 a P07) se abre desde «Progreso del equipo», la tercera de las cuatro opciones del menú principal (INC-81), y es la única pantalla del juego con cifras. `ProfileRepository` enumera los perfiles de las dos rutas de almacenamiento, `Datos/` junto al ejecutable y la ruta de respaldo, sin duplicarlos y tolerando un archivo dañado. `IndicatorReport`, `PhaseIndicators` y `LevelReportSection` agrupan los cuatro indicadores del perfil elegido por nivel y por fase; un nivel no jugado aparece como «Sin datos» y no con ceros, y no se calcula ningún total, promedio ni agregado entre perfiles o fases (INC-35, INC-86). Una prueba fija por reflexión que `PerformanceIndicators` tiene exactamente los cuatro campos de la tabla de indicadores de OE1. El tiempo de resolución se guarda en segundos y se presenta en minutos y segundos (`ResolutionTimeFormat`).

La escena `TeacherReport` muestra la lista de perfiles con selección, presenta de entrada los indicadores del primero y clona una fila por fase, agrupadas por nivel. Desde el ejecutable del corte, un encabezado nombra el perfil que se está mirando («Perfil de {0}», en `ReportContent.SelectedProfileFormat`), y `Select` lo actualiza al abrir, al elegir otro perfil y tras un borrado, para que el docente no atribuya las cifras a otro estudiante (`TeacherReport_RF46_ElDetalleNombraElPerfilQueSeEstaMirando`). Sin perfiles, muestra «Todavía no hay perfiles registrados en este equipo.» y se sale con «Volver al menú». `GameFlowRunner` expone las dos raíces de almacenamiento para que `Game.UI` arme su propio `ProfileRepository` sin que el núcleo conozca ese tipo, y `TeacherReport_CP03_NingunaRutaDelEstudianteAlcanzaLaPantallaDeCifras` recorre cada estado del flujo y confirma que solo el menú principal puede pedir el informe. El punto de control P-C se verificó en vivo con un perfil de siete fases jugadas y otro sin ninguna.

El 30/09/2026 (`37b3cb7`) el informe pasó a las tipografías del juego, Baloo 2 y Nunito, con 26 px como tamaño mínimo (INC-79); lo vigila `Scenes_CN04_NingunTextoUsaLaFuenteIntegradaDelMotor`, que alcanza también la etiqueta «Progreso del equipo» del menú. La escena llevaba incrustado el mapa de acciones por defecto del paquete de entrada, con rueda, teclado y mando, y pasó al mapa del juego, `ControlesJugables` (INC-80). Sus dos listas se desplazan con barra; créditos e informe son pantallas no jugables, sin arrastrar y soltar, y conservan ese desplazamiento, que se maneja con clic y clic sostenido, como excepción decidida el 29/09/2026 y recogida en HU-18. Con ocho perfiles, la octava fila de la lista sale con el borde inferior recortado hasta que se desplaza, aunque se ve y se elige; es una observación menor (documento principal, apartado 10.4). Cuando el guardado cae a la ruta de respaldo, el informe avisa al docente: «Este equipo no deja guardar en la carpeta del juego: los perfiles se están guardando en la carpeta de datos de Windows.» (INC-34, INC-48). Todos sus textos viven en `ReportContent`.

Los cuatro iconos de los indicadores (intentos, errores corregidos, pasos utilizados y tiempo de resolución) se dibujaron con formas geométricas simples el 24/09/2026 y van sobre la cabecera de la tabla, encima del nombre de cada indicador; en «Errores corregidos» y «Tiempo de resolución», que parten en dos líneas, el icono queda sobre la primera palabra, observación menor que se corrige poniéndolo a la izquierda del rótulo. El acta D10 los aceptó como arte definitivo, cubierto por la línea de créditos «Entornos, objetos e interfaz: originales del proyecto, salvo los iconos de pausa.», y decidió que el informe no lleve protección de acceso, porque ningún requerimiento la pide.

### 2.4 Eliminación de datos

La eliminación (P08 a P10) no necesitó código de borrado nuevo. `SaveStore.Delete`, del núcleo, borraba desde el Slice 1 el perfil de las dos rutas, no reporta como éxito un borrado parcial y no toca otros perfiles, y `ProfileSession.Delete` deja sin perfil activo al juego si el borrado era el activo, lo que respondió la tercera pregunta abierta del plan. Escribir una clase de borrado aparte habría duplicado lógica ya probada, así que las dos pantallas llaman a esa operación (INC-85).

**Tabla G.3.** Los dos accesos a la eliminación de un perfil (RF-47).

| | Panel «¿Quién juega?» del menú principal | Informe docente |
|---|---|---|
| Componente | `ProfileSelectController`, desde el Slice 1 (`145a63e`, 08/09/2026) | `EraseConfirmationDialog`, desde `9c34924` |
| Pregunta | «¿Borras el perfil de {nombre}?» | «¿Eliminas definitivamente los datos de {nombre}? Esta acción no se puede deshacer.» |
| Aviso aparte | «Su avance se pierde y no se puede recuperar.» | Ninguno: la irreversibilidad va en la pregunta |
| Botones | «Conservar» y «Borrar» | «Cancelar» y «Eliminar datos» |
| Asset de la pregunta | `ProfileSelectContent` | `ReportContent` |

Los dos diálogos comparten la disposición: velo, icono de alerta, el botón de cancelar en crema y el de confirmar en ámbar con el icono de papelera, de modo que la acción destructiva se distingue por color y por forma (RNF-19) y no es la opción por defecto. Desde el 30/09/2026 sus textos viven en `ProfileSelectContent`, un asset nuevo, y en `ReportContent` (INC-104). La prueba de residuos (P10) borra sobre disco real en un directorio temporal que ella misma crea, con una guarda escrita antes que el borrado contra la carpeta `Datos/` del proyecto y contra la carpeta de datos persistentes del motor. Su escenario con `Datos/` de solo lectura se omite en el equipo de desarrollo, que no hace cumplir ese atributo sobre carpetas (apartado 4.2).

### 2.5 Cierre del proyecto

**El ejecutable (INC-97, decisiones D2 y D3 del 29/09/2026).** Desde el 30/09/2026 el ejecutable se identifica con `productName` «Algoritmia» y `companyName` «Universidad Catolica de Colombia». En la configuración del proyecto quedaron apagados la analítica, las estadísticas de hardware y los servicios de Unity, junto con la pantalla de presentación del motor y el cambio a pantalla completa con Alt+Intro, y nada de ello obligó a retirar paquetes del manifiesto. Lo vigilan las cuatro pruebas de `PlayerSettingsTest`, una por ajuste, que leen la configuración guardada y no el ejecutable. La compilación toma en cambio los ajustes que el Editor tiene en memoria, y así el primer candidato del ejecutable salió con los servicios de Unity encendidos, abriendo conexiones de red y dejando datos de analítica fuera de `Datos/`. Desde el ejecutable del corte lo impide `UnityServicesOff`, en `Game.EditorTools`: antes de compilar apaga nueve interruptores de servicios y, al terminar, hace fallar la compilación si el juego compilado nombra los servidores de Unity. Diez pruebas EditMode lo vigilan, y en el ejecutable del corte no se observó ninguna conexión de red (apartado 4.3). El símbolo de compilación de analítica que un paquete repone en cada recarga del Editor solo compila código de Editor y no llega al ejecutable. Fuera de `Datos/`, el reproductor guarda su registro (`Player.log`) en la carpeta de la compañía y del producto dentro de `%AppData%\LocalLow`, y sus preferencias de pantalla en la clave equivalente de `HKCU\Software`, donde el motor deja además un contador y un identificador de sesión y tres identificadores `unity_connect.*`, uno de ellos de instalación, aunque sus servicios estén apagados; en ninguno de los dos lugares hay datos del estudiante, y esos identificadores se documentan como residuo del motor (DEF-RC2-01; documento principal, apartados 3.6 y 10.4).

**Guardado y salida (INC-34, INC-48, INC-77).** `ProfileSession` expone si el guardado cayó a la ruta de respaldo y en qué carpeta. «Salir» pide confirmación con «Quedarme» y «Cerrar el juego» e informa del guardado; con `Datos/` no escribible muestra «Este equipo no deja guardar en la carpeta Datos del juego. El progreso quedó guardado en:» seguido de la carpeta, y reduce la letra del aviso hasta que cabe sin tapar los botones. Desde el ejecutable del corte «Cerrar el juego» va sin icono y su rótulo cabe en una línea, en un botón de 280 px (`MainMenu_RF09_CerrarElJuegoNoLlevaPapeleraYSuRotuloCabeEnUnaLinea`). El guardado escribe cada perfil en un temporal y lo pone en su sitio con un reemplazo, y un perfil cuyo archivo no se puede leer muestra en el panel de perfiles «No se pudo abrir ese perfil. Avisa a tu profe.» en lugar de detener el juego (documento principal, apartado 3.5). Lo vigilan cinco pruebas `MainMenu_HU18_*`, entre ellas `MainMenu_HU18_SalirPideConfirmacionAntesDeCerrar` y `MainMenu_HU18_ElAvisoDeRespaldoConUnaRutaLargaNoTapaLosBotones`.

**Menús (INC-76, INC-81, INC-82).** Un nivel con todas sus fases confirmadas lleva en el menú de niveles un icono de visto y la palabra «Completado», sin cifras, y se puede repetir (`LevelSelect_HU14_ElNivelCompletadoSeMarcaEnElMenu` y dos pruebas más). El inicio, los créditos y el menú de niveles muestran arte real en lugar de los cinco rótulos de trabajo que quedaban a la vista, y la tarjeta del Nivel 2 usa una ilustración de proporción 16:9 sin costura (`Scenes_RNF01_NingunTextoDeEscenaEsUnRotuloDeTrabajo`, `LevelSelect_RF03_CadaTarjetaMuestraUnaIlustracionSinCostura`).

**Desbloqueo por fases (INC-116).** Si el juego se cerraba durante la escena 2.5, las tres fases del Nivel 2 ya estaban en disco y el menú lo marcaba «Completado», pero el Nivel 3 seguía bloqueado hasta repetir el Nivel 2 entero. Desde el 30/09/2026 `LevelUnlockPolicy.IsUnlocked` da un nivel por desbloqueado si lo indica el nivel alcanzado o si todas las fases del anterior están confirmadas, derivándolo del progreso guardado y sin campo nuevo (RNF-09). La consultan el menú de niveles y `GameFlow.TryStartPlaying`: con la primera versión el menú pintaba el Nivel 3 desbloqueado y el flujo rechazaba empezarlo, defecto que dejó la prueba `GameFlow_RNF14_EntraAlNivelQueDesbloqueanLasFasesConfirmadasDelAnterior`. `NarrativeVisitPolicy` sigue leyendo el nivel alcanzado, de modo que el cierre reflexivo no puede omitirse la primera vez (CP-07, RF-12).

**Registro de cargas (tarjeta D10-5, 01/10/2026).** `SceneLoader` deja en el registro del reproductor una línea por carga, `RNF-04: «<escena>» cargó en <segundos> s`, con tres decimales, punto decimal en cualquier configuración regional y sin pila de llamadas. La cifra no llega nunca a la pantalla (CP-03); es el instrumento con que se midió sobre el ejecutable la carga de las doce escenas (apartado 4.3). Lo vigila `SceneLoader_RNF04_CadaCargaDejaSuTiempoEnElRegistro`, que antes del cambio falló porque la primera carga no dejaba ninguna línea.

**Contenido (RNF-22) y consentimiento (RNF-12).** El 01/10/2026 se hizo la inspección textual del contenido: ningún texto de `Assets/Game/Data`, de las escenas ni de los prefabs lleva enlaces, precios, compras ni publicidad, y el manifiesto de paquetes no trae paquetes de compras ni de anuncios. La parte visual, sin violencia explícita, se revisa sobre las capturas del recorrido completo del ejecutable en el OE4. El formato de consentimiento informado del acudiente y de asentimiento del estudiante, conforme a la Ley 1581 de 2012 y al Decreto 1377 de 2013, está en `claudeDocs/tasks/OE4/Consentimiento-RNF12.md` y va en blanco como anexo del trabajo de grado.

**Plan de pruebas del OE4 y arnés del ejecutable.** El plan entró con `d105838` (29/09/2026): `plan.md` fija la versión congelada, los ejecutores, el arnés, los perfiles semilla y el registro de defectos; `casos.md` reúne los casos `PF-*` sobre los requerimientos funcionales y no funcionales (127 al corte), y `todo.md` es el tablero de las tareas T01 a T28, con seis puntos de control. El arnés `oe4.ps1` (tarjeta D10-6) maneja el ejecutable como caja negra, igual que un jugador: lo lanza, toma capturas, hace clic, sostiene, arrastra y escribe el nombre del perfil, espera cada carga leyendo la línea `RNF-04` y muestrea la memoria y la red; con sus órdenes `Medidas`, `Tamano`, `Residuos` y `Revisar-Log` resume esas medidas, el tamaño de la carpeta entregable, los residuos fuera de `Datos/` y los errores del registro. Lo acompañan nueve perfiles semilla.

**Tablero del slice.** El acta D10 marcó como revisadas por Santiago Benavides Rey las cinco casillas de revisión con el usuario de P-A a P-E, después de verificarlas con pruebas y capturas. PG-01, el título «Algoritmia», en pantalla desde el 09/09/2026, se cerró en el guion el 29/09/2026; PG-02, el nombre del guía, lo estaba desde el 02/09/2026. Los diez criterios de éxito de la especificación se revisaron uno por uno el 01/10/2026 (apartado 4.4), y el registro de inconsistencias, en su revisión 16, no reabrió ningún hallazgo.

## 3. Decisiones de diseño y hallazgos de consistencia

Las decisiones del slice, descritas en el apartado 2, quedaron escritas en su tablero con su fecha: el informe depende solo del núcleo, sus pantallas viven en `Game.UI`, el resumen de nivel no se reconstruyó y el borrado reutiliza `SaveStore.Delete`. La escena del informe se armó con un script de editor temporal, borrado al terminar, que para una pantalla de unos cuarenta objetos evitaba cientos de operaciones sueltas.

Las pruebas encontraron defectos reales durante el slice. La cabecera de la tabla no se alineaba con las filas, porque el texto largo de una columna («Errores corregidos») recibía más ancho que las celdas vacías de la plantilla de fila; se corrigió fijando el mismo ancho flexible en las seis celdas de ambas. La escena nueva había tomado el mapa de controles genérico del paquete de entrada en lugar del del juego, y lo detectó `Architecture_RNF02_LasEscenasYElProyectoUsanSoloElMapaDeControlesDelJuego`, que vigilaba ese riesgo desde el Slice 3; el 29/09/2026 apareció un segundo caso, un mapa incrustado sin identificador que esa prueba no veía, y se recableó al de `ControlesJugables`. El primer barrido de CP-03 encontró identificadores de secuencia (`N1_Apertura`), de objetos del catálogo (`tronco_1`) y los índices de formato del contador de RF-24, ninguno de ellos una cifra de desempeño; la prueba se afinó con reglas generales en lugar de una lista de excepciones por campo. En las pruebas de interfaz, el diálogo de borrado cablea sus botones en el cuadro siguiente al que se activa, y las pruebas esperan ese cuadro antes de pulsar.

**Tabla G.4.** Hallazgos de consistencia del incremento.

| INC | Qué decidió | Documentos corregidos o implementación | Fecha de cierre |
|---|---|---|---|
| INC-26 | El resumen del estudiante no muestra cifras; los indicadores solo se consultan en el informe | HU-14 | 30/08/2026 |
| INC-34 | El borrado alcanza las dos rutas y RNF-11 se prueba con `Datos/` escribible y de solo lectura | Arquitectura, apartado 7; aviso al docente implementado el 30/09/2026 | 30/08/2026 |
| INC-35 | El informe presenta los indicadores por nivel y por fase | CU-11; arquitectura, apartado 6 | 30/08/2026 |
| INC-48 | Arquitectura del documento de diseño sustituida por la implementada | Solución OE2, apartados 4.1 a 4.5; aviso de la ruta de respaldo implementado | 29/09/2026 |
| INC-76 | El menú marca el nivel completado | Interfaces; CU-02; arquitectura; casos del OE4; implementado en el menú | 29/09/2026 |
| INC-77 | «Salir» pide confirmación y avisa de la ruta de respaldo | Casos del OE4; interfaces, apartado 1.1; implementado | 29/09/2026 |
| INC-78 | Los créditos nombran a los autores y la obra de la Familia Anonaky y acreditan los sonidos | OE1, apartados 1.2, 2.4 y 4.6; HU-18; dirección de sonido; trabajo de grado; texto de créditos sobre la Familia Anonaky en tres oraciones | 29/09/2026 |
| INC-79 | Solo Baloo 2 y Nunito; el informe a 26 px como mínimo | Dirección de arte, apartados 11.2 y 11.3; interfaces, apartado 11; casos del OE4; implementado | 29/09/2026 |
| INC-80 | Entrada solo con clic y clic sostenido, con la escritura del nombre como única excepción, y el mapa del juego en todas las escenas | OE1, CT-06 y RNF-02; HU-18; especificación; arquitectura; implementado en el informe | 29/09/2026 |
| INC-81 | El menú principal tiene cuatro opciones | RF-01; HU-01, HU-16 y HU-18; CU-11; arquitectura; interfaces | 29/09/2026 |
| INC-82 | Sin rótulos de trabajo a la vista: arte real en inicio, créditos y menú | Casos del OE4; implementado | 29/09/2026 |
| INC-85 | El perfil se borra también desde el panel «¿Quién juega?» | Interfaces; CU-12; HU-16 y HU-01; arquitectura | 29/09/2026 |
| INC-86 | El informe muestra los indicadores del perfil elegido, sin agregados, y una pantalla sin perfiles | RF-46; CU-11; HU-16 | 29/09/2026 |
| INC-97 | Identidad y configuración del ejecutable (decisiones D2 y D3) | Especificación; casos del OE4; arquitectura; guía del repositorio; implementado en la configuración del proyecto | 29/09/2026 |
| INC-104 | Ningún texto visible queda escrito en C# | Implementado: `ProfileSelectContent`, `ReportContent` y rótulos del laberinto en datos | 29/09/2026 |
| INC-116 | Un nivel se desbloquea también con todas las fases del anterior confirmadas | Arquitectura, apartados 7 y 12; HU-18; especificación; interfaces; casos y plan del OE4; implementado | 30/09/2026 |

## 4. Verificación

### 4.1 Pruebas que lo nombran

En el corte, `Game.Reporting.Tests` tiene 9 métodos y 14 casos, y `Game.Content.Tests`, 3 métodos y 3 casos. Las de las pantallas están en el módulo de interfaz de PlayMode, y las del ejecutable, los textos de escena y el mapa de controles, en el de arquitectura.

**Tabla G.5.** Pruebas representativas del incremento y del cierre.

| Requisito | Qué se verifica | Prueba representativa |
|---|---|---|
| RF-45 | Ningún resumen contiene un dígito | `LevelSummary_RF45_NingunResumenDeLosTresNivelesContieneUnDigito` |
| RF-46 | Acceso desde el menú sin perder el perfil activo; consultar no altera ningún perfil | `TeacherReport_RF46_SeAlcanzaDesdeElMenuYVuelveSinPerderElPerfilActivo`, `TeacherReport_RF46_ConsultarNoAlteraNingunPerfil` |
| RF-47 | Confirmación explícita y cancelar sin cambios en disco, en los dos accesos | `EraseDialog_RF47_ExigeConfirmacionExplicitaYAdvierteIrreversibilidad`, `EraseDialog_CU12_CancelarNoRealizaNingunCambioEnDisco`, `ProfileSelect_RF47_BorrarPideConfirmacionAntesDeEliminarElPerfil` |
| RF-47, RNF-11 | Borrado en las dos rutas, sin tocar otros perfiles y sin residuos en disco real | `SaveStore_RF47_BorraElPerfilDeLasDosRutas`, `SaveStore_RF47_NoAfectaAOtrosPerfiles`, `ProfileEraser_RNF11_SobreDiscoRealNoQuedaNingunaEntradaDelPerfil` |
| CP-03 | Ninguna ruta ni ningún texto del estudiante llega a las cifras | `TeacherReport_CP03_NingunaRutaDelEstudianteAlcanzaLaPantallaDeCifras`, `Content_CP03_NingunTextoVisibleAlEstudianteContieneCifrasDeDesempeno` |
| RNF-09, CT-10 | Lista cerrada del perfil y los 47 RF con prueba | `SaveStore_RNF09_ElJsonDeUnPerfilCompletoNoTieneCampoFueraDeLaListaCerrada`, `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra` |
| RNF-16 | El informe no depende de ningún nivel | `Architecture_RNF16_RetirarUnNivelNoRompeElInformeDocente` |
| INC-34 | Aviso de la ruta de respaldo en el núcleo y en el informe | `ProfileSession_INC34_ExponeQueElGuardadoCayoALaRutaDeRespaldo`, `TeacherReport_INC34_AdvierteAlDocenteCuandoElGuardadoUsaLaRutaDeRespaldo` |
| HU-18 | «Salir» con confirmación y aviso de respaldo | `MainMenu_HU18_SalirPideConfirmacionAntesDeCerrar`, `MainMenu_HU18_AdvierteLaRutaDeRespaldoAntesDeCerrar` |
| HU-14 | El nivel completado se marca en el menú | `LevelSelect_HU14_ElNivelCompletadoSeMarcaEnElMenu`, `LevelSelect_HU14_UnPerfilNuevoNoMuestraNingunNivelCompletado` |
| RNF-14 | Desbloqueo por fases confirmadas | `LevelUnlockPolicy_RNF14_UnNivelConTodasSusFasesConfirmadasDesbloqueaElSiguiente`, `LevelSelect_RNF14_UnNivelConTodasSusFasesConfirmadasDesbloqueaElSiguiente`, `GameFlow_RNF14_EntraAlNivelQueDesbloqueanLasFasesConfirmadasDelAnterior` |
| RF-01, RNF-02, RNF-10 | Identidad y configuración del ejecutable; servicios de Unity apagados al compilar | Las cuatro pruebas de `PlayerSettingsTest` y las diez `UnityServicesOff_RNF10_*` (apartado 2.5) |
| RF-46 | El informe nombra el perfil que se está mirando | `TeacherReport_RF46_ElDetalleNombraElPerfilQueSeEstaMirando` |
| RF-02, RF-09, RNF-14 | Panel de perfiles por páginas, «Cerrar el juego» en una línea, perfil ilegible y guardado con temporal | `ProfileSelect_RF02_ConOchoPerfilesTodosSeAlcanzanConLasFlechas`, `MainMenu_RF09_CerrarElJuegoNoLlevaPapeleraYSuRotuloCabeEnUnaLinea`, `ProfileSelect_RNF14_ElegirUnPerfilIlegibleAvisaYNoLanza`, `DiskFileSystem_RNF14_SiLaEscrituraFallaElPerfilAnteriorQuedaIntacto` |
| RNF-04 | Una línea de registro por carga | `SceneLoader_RNF04_CadaCargaDejaSuTiempoEnElRegistro` |

### 4.2 Corridas declaradas

La Tabla G.6 recoge las corridas del incremento y las corridas completas del cierre, con las cifras de la Tabla 9.2 del documento principal. Las del 25/09, el 30/09 y el 01/10/2026 se hicieron con el Editor abierto y la vista de juego a 1920 × 1080.

**Tabla G.6.** Corridas del incremento y corridas completas de la suite.

| Fecha | Corte | EditMode | PlayMode | Observaciones |
|---|---|---|---|---|
| 24/09/2026 | Slice 4, punto de control P-E | 321 superadas, 1 omitida | Dirigida a la interfaz: 23 de 25 (2 sin concluir) | PlayMode limitada al módulo de interfaz; las dos sin concluir son anteriores al slice (convergencia del Nivel 1 en las pruebas del resumen) |
| 25/09/2026 | Suite completa (`ccf77e6`) | 360 de 361 (1 omitida) | 304 de 306 | 9,8 s y 1 695 s. Pasó la prueba de carga de `Level3_River` (< 10 s). Fallaron la prueba de memoria (2 831 MB en un Editor que sin el nivel ya reservaba 2 652 MB) y la de destellos del hundimiento, por un cuadro largo; aislada, pasó |
| 30/09/2026 | Línea base del cierre (`359365e`) | 390 de 391 (1 omitida) | 332 de 332 | La corrida por lotes que acompañó a `37b3cb7` dio 313 de 332 en PlayMode: trece fallos de disposición por la vista de 640 × 480, que pasan con el Editor abierto |
| 01/10/2026 | Correcciones de los Niveles 1 y 2 (tarjetas D10-1, D10-2 y D10-5) | 392 de 393 (1 omitida) | 350 de 351 | 14,5 s y 1 660 s. Falló la prueba de memoria (2 127 MB tras horas de corridas); aislada y con el Editor recién abierto pasó con 1 660 MB |
| 01/10/2026 | Correcciones del Nivel 3 (tarjeta D10-3) | 426 de 427 (1 omitida) | 363 de 363 | 1 705 s; la prueba de memoria pasó con 1 982 MB y el Editor reiniciado |
| 01/10/2026 | Primer candidato del ejecutable | 433 de 434 (1 omitida) | 364 de 364 | 7,8 s y 28 min |
| 01/10/2026 | Corte del entregable (ejecutable del corte) | 469 de 470 (1 omitida) | 376 de 376 | 8,9 s y 29 min (1 743,5 s) |

La omitida de las corridas del 24/09 al 01/10/2026 es `ProfileEraser_INC34_SobreDiscoRealCubreLosDosEscenariosDeAlmacenamiento`, que se omite sola con un motivo declarado: el equipo de desarrollo no hace cumplir el atributo de solo lectura sobre carpetas, así que el escenario de solo lectura de INC-34 no puede simularse en él. El mismo escenario está cubierto con un sistema de archivos simulado (`SaveStore_INC34_BorraDesdeLaRutaDeRespaldoAunqueDatosNoSeaEscribible`). La prueba de memoria del Nivel 3 lee la memoria reservada del Editor, de modo que mide la sesión y no el nivel; el valor que vale para RNF-05 es el del ejecutable (apartado 4.3).

El documento principal detalla la corrida final en su apartado 9.3. Se hizo justo antes de compilar el ejecutable del corte: la PlayMode, de 17:31 a 18:00, como primera corrida de una sesión del Editor recién abierta, y la EditMode a continuación. La anterior, que precedió al primer candidato, corrió la PlayMode entre las 04:37 y las 05:05 y repitió la EditMode a las 08:29, porque la excepción de importación de INC-130 había sumado una prueba. En la corrida final no falló ningún caso. La única omisión es la prueba de solo lectura ya citada, cuyo escenario se reprodujo además sobre el ejecutable, con `Datos` convertido en un archivo. Los resultados de esa corrida y de las anteriores del cierre se guardan con las evidencias del OE4, en `claudeDocs/tasks/OE4/evidencias/suites/`, con el XML y el resumen JSON de cada modo, también los de la corrida final.

**Tabla G.7.** Casos de EditMode por módulo de prueba en las corridas completas.

| Módulo | 25/09 | 30/09 | 01/10 (Niveles 1 y 2) | 01/10 (Nivel 3) | 01/10 (primer candidato) | Corte |
|---|---|---|---|---|---|---|
| `Game.Architecture.Tests` | 15 | 21 | 21 | 21 | 23 | 23 |
| `Game.Audio.Tests` | 0 | 0 | 0 | 0 | 0 | 0 |
| `Game.Content.Tests` | 1 | 3 | 3 | 3 | 3 | 3 |
| `Game.Core.Tests` | 60 | 67 | 67 | 67 | 67 | 74 |
| `Game.EditorTools.Tests` | 5 | 5 | 5 | 5 | 9 | 19 |
| `Game.Levels.Fire.Tests` | 49 | 49 | 49 | 49 | 49 | 49 |
| `Game.Levels.River.Tests` | 41 | 41 | 41 | 57 | 58 | 58 |
| `Game.Levels.Wheel.Tests` | 69 | 69 | 71 | 71 | 71 | 81 |
| `Game.Reporting.Tests` | 14 | 14 | 14 | 14 | 14 | 14 |
| `Game.Scaffolding.Tests` | 88 | 102 | 102 | 120 | 120 | 121 |
| `Game.UI.Tests` | 19 | 20 | 20 | 20 | 20 | 28 |
| **Total EditMode** | **361** | **391** | **393** | **427** | **434** | **470** |

**Tabla G.8.** Casos de PlayMode por módulo de prueba en las corridas completas.

| Módulo | 25/09 | 30/09 | 01/10 (Niveles 1 y 2) | 01/10 (Nivel 3) | 01/10 (primer candidato) | Corte |
|---|---|---|---|---|---|---|
| `Game.Audio.PlayMode.Tests` | 4 | 4 | 4 | 4 | 4 | 4 |
| `Game.Core.PlayMode.Tests` | 11 | 11 | 12 | 12 | 12 | 12 |
| `Game.Levels.Fire.PlayMode.Tests` | 39 | 43 | 59 | 59 | 59 | 60 |
| `Game.Levels.River.PlayMode.Tests` | 44 | 47 | 47 | 57 | 57 | 57 |
| `Game.Levels.Wheel.PlayMode.Tests` | 89 | 92 | 93 | 93 | 93 | 99 |
| `Game.UI.PlayMode.Tests` | 119 | 135 | 136 | 138 | 139 | 144 |
| **Total PlayMode** | **306** | **332** | **351** | **363** | **364** | **376** |

Entre el 25 y el 30/09/2026 las pruebas crecieron con `44fd479` (una de PlayMode), `d105838` (dos de PlayMode) y `37b3cb7` (30 de EditMode y 23 de PlayMode). Del 30/09 a la primera corrida del 01/10 entraron dos de EditMode, sobre la caja de la escena 2.2 y la carretilla amarrada, y diecinueve de PlayMode: las dieciséis del Nivel 1, la del taller que amarra la cuerda, la del registro de cargas y la del botón «Omitir» en la segunda visita. Las correcciones del Nivel 3 sumaron 34 casos de EditMode y doce de PlayMode. Hasta la corrida que precedió al primer candidato entraron además las pruebas de la copia de las licencias y de la excepción de importación de INC-130. Las correcciones del ejecutable del corte sumaron 36 casos de EditMode, del guardado con temporal y del perfil ilegible en el núcleo, del apagado de los servicios de Unity, de las reglas de la lista del laberinto, de la paginación de perfiles y de la fogata de la escena final, y doce de PlayMode, del laberinto, del panel de perfiles, del aviso de salida, del informe y de los deslizantes del Nivel 1.

### 4.3 Mediciones sobre el ejecutable

La tabla de mediciones de P12 cubre las doce escenas de la configuración de compilación, en su orden, más la memoria y el tamaño del paquete. El equipo 1 es el de desarrollo que describe el capítulo 8 del documento principal (procesador AMD Ryzen 5 7600X, 32 GB de memoria, tarjeta NVIDIA GeForce RTX 5070 Ti, Windows 11 Home de 64 bits y pantalla de 1920 × 1080). Cada carga es la peor de todas las medidas de esa escena en los dos candidatos del ejecutable, al menos cuatro, leída de las líneas `RNF-04` del registro del reproductor, y la memoria es la máxima del proceso en las sesiones sobre el ejecutable del corte. La columna del equipo 2 la llena Santiago Benavides Rey con el guion H3 de la hoja de verificaciones manuales del OE4.

**Tabla G.9.** Mediciones de P12 sobre el ejecutable.

| Medición | Presupuesto | Equipo 1 | Equipo 2 |
|---|---|---|---|
| Carga de `Boot` | < 10 s | Escena inicial; arranque hasta `MainMenu` ≤ 3,23 s | Guion H3 del OE4 |
| Carga de `MainMenu` | < 10 s | 0,11 s | Guion H3 del OE4 |
| Carga de `LevelSelect` | < 10 s | 0,08 s | Guion H3 del OE4 |
| Carga de `Credits` | < 10 s | 0,04 s | Guion H3 del OE4 |
| Carga de `Narrative` | < 10 s | 0,78 s | Guion H3 del OE4 |
| Carga de `Level1_Cave` | < 10 s | 0,16 s | Guion H3 del OE4 |
| Carga de `LevelSummary` | < 10 s | 0,04 s | Guion H3 del OE4 |
| Carga de `Level2_Forest` | < 10 s | 0,05 s | Guion H3 del OE4 |
| Carga de `Level2_Workshop` | < 10 s | 0,09 s | Guion H3 del OE4 |
| Carga de `Level2_Maze` | < 10 s | 0,09 s | Guion H3 del OE4 |
| Carga de `Level3_River` | < 10 s | 0,09 s | Guion H3 del OE4 |
| Carga de `TeacherReport` | < 10 s | 0,06 s | Guion H3 del OE4 |
| Memoria máxima en ejecución | < 2 GB | 446 MB de memoria de trabajo, al abrir el cruce de la balsa; 1 179 MB de memoria privada, al abrir `N1_NacimientoDelFuego` | Guion H3 del OE4 |
| Tamaño del paquete | < 500 MB | 479,0 MB | Guion H3 del OE4 |
| Recorridos automatizados completos | Sin incidencias bloqueantes | 87 y 33 min sobre el primer candidato y 16 min sobre el ejecutable del corte, sin incidencias bloqueantes; los dos defectos del laberinto que vieron los dos primeros, corregidos | No aplica |
| Recorrido completo cronometrado | 20 a 40 min | Guion H8 del OE4 | Guion H8 del OE4 |

La peor carga fue de 0,78 s, en `Narrative` al abrir `N1_Apertura` tras crear un perfil nuevo; `Boot` no pasa por el cargador de escenas y su fila da la cota del arranque que registró el arnés sobre el primer candidato (documento principal, apartado 8.3). El tamaño del paquete se midió sin la carpeta `Datos/` y sin la de información de depuración que el motor marca como no distribuible (0,8 MB), que se retira al empaquetar.

El apartado 8.1 del documento principal desglosa esos 479,0 MB por carpetas y bibliotecas. El grueso, 409,7 MB, son los datos del juego, con las texturas del Nivel 1 como partida mayor (276,4 MB); del resto, la mayor parte corresponde a la biblioteca del reproductor y a las bibliotecas de DirectX que el build copia junto al ejecutable.

Otras tres condiciones se comprobaron sobre el ejecutable, y el documento principal las detalla. La carpeta entregable del primer candidato funcionó copiada sin `Datos/` a una ruta con espacios, sin instalación, y creó allí su propia carpeta de perfiles (apartado 10.2). En las siete ejecuciones del ejecutable del corte no se observó ninguna conexión de red, y RNF-10 se cumple en el equipo de desarrollo; el primer candidato abría en cada lanzamiento tres conexiones con direcciones de Google Cloud, por los servicios de Unity que llevaba activos (apartado 8.3). En cuanto a los residuos, después de borrar cuatro perfiles en el primer candidato sus nombres no quedaron en la carpeta portable ni en los lugares donde escribe el reproductor, y en el del corte los perfiles borrados desaparecen de `Datos/`; fuera de esa carpeta quedan su registro y su clave en el registro de Windows, con los valores de pantalla y los identificadores que el motor escribe aunque sus servicios estén apagados (apartado 3.6). La última medición anterior, del 21/09/2026 y con once escenas, dio 1,36 s de carga de `Level3_River` y un paquete de 217 MB.

### 4.4 Criterios de éxito de la especificación

La especificación del proyecto fija diez criterios de éxito, uno por indicador del trabajo de grado. Se revisaron uno por uno el 01/10/2026 y se completaron con las cifras del ejecutable.

**Tabla G.10.** Criterios de éxito de la especificación.

| # | Criterio | Evidencia | Veredicto |
|---|---|---|---|
| 1 | OE1, trazabilidad: el 100 % de los RF asociado a una faceta del pensamiento computacional o a un lineamiento | OE1, apartado 5.1, y Solución OE2, apartados 3.1 a 3.4 y su resumen en el 3.6: los RF de los niveles están en la matriz de facetas y los de soporte, andamiaje y evaluación, en las de lineamientos. Se verificó el 30/08/2026 sobre los 47 RF; los cambios del 25/09 al 01/10 corrigen comportamiento y texto sin añadir ni quitar ninguno | Cumple |
| 2 | OE1, criterios pedagógicos: al menos el 80 % de los diez CP integrado en el diseño | Los diez CP tienen al menos un RF que los materializa (Solución OE2, apartado 3.2). Cuatro se verifican además con pruebas que los nombran: CP-02 (20 pruebas, entre ellas `GameFlow_CP02_NoExisteEstadoDeDerrota`), CP-03 (8), CP-06 (6) y CP-07 (2) | Cumple (10 de 10) |
| 3 | OE2, progresión: tres niveles con dificultad ascendente y desbloqueo secuencial (RF-03) | Tres niveles de una, tres y tres fases: una variable en el Nivel 1, tres mecánicas distintas en el 2 y tres fases bloqueantes con prueba y depuración en el 3. Desbloqueo: cinco pruebas `LevelUnlockPolicy_RF03_*`, `GameFlow_RF03_NoPermiteEntrarANivelBloqueado` y `LevelUnlockPolicy_CP02_NuncaRebloqueaUnNivelYaDesbloqueado`; desde el 30/09 también por fases confirmadas (INC-116) | Cumple |
| 4 | OE2, alineación narrativa: más del 85 % de los retos narrativos exige una acción de pensamiento computacional (CP-10) | Los retos de las 18 narrativas se resuelven en las siete fases persistidas y en la recolección del Nivel 3, cada uno con una acción de la matriz de facetas; ninguno se supera avanzando el texto (`GameFlow_RNF13_RecorreElGoldenPathCompletoSinEstadoIrrecuperable`, `WheelLevel_RNF13_RecorreElNivel2CompletoHastaElMenuConNivel3Desbloqueado`, `RiverLevel_RNF13_RecorreElNivel3CompletoHastaElInicio`) | Cumple (8 de 8 retos jugables) |
| 5 | OE3, mecánicas: al menos tres mecánicas principales implementadas | Siete, cada una con su escena y sus pruebas: panel de hipótesis e iteración; selección por patrón; ensamblaje secuencial; editor de bloques; movimiento y recolección en la orilla; ensamblaje por fases con prueba y depuración | Cumple (siete; pide tres) |
| 6 | OE3, recorrido principal: cada nivel de principio a fin sin bloqueos, cierres inesperados ni estados irrecuperables; dos recorridos completos sin incidencias (RNF-13) | En el Editor, los recorridos automatizados por tramos, entre ellos `GameEnding_INC39_RecorreLevelSummaryNarrativeCreditsYMainMenu` y `GameEnding_RF44_LaPruebaSuperadaReproduceElCruceYElCierre`. Sobre el ejecutable, tres recorridos completos con el arnés: 87 y 33 min sobre el primer candidato y 16 min sobre el ejecutable del corte, que corrige los dos defectos no bloqueantes del laberinto que vieron los dos primeros | Cumple, en el Editor y sobre el ejecutable, sin incidencias bloqueantes |
| 7 | OE4, pruebas funcionales: 90 % de la funcionalidad verificada, con caso de prueba por requerimiento (CT-10) | Caso por requerimiento: `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra` y el catálogo de casos del OE4 | Se mide en el OE4 |
| 8 | OE4, requerimientos críticos: todos los RF de prioridad alta, implementados | Los 45 de prioridad alta desde el punto de control P-E (24/09/2026), y también RF-06 y RF-21; la prueba de trazabilidad los nombra a todos. Sobre el ejecutable del corte, ningún caso de prueba funcional ejecutado de un RF de prioridad alta termina en fallo | Cumple |
| 9 | Presupuestos: carga < 10 s, memoria < 2 GB, paquete < 500 MB; ejecución portable en dos equipos y con el adaptador de red deshabilitado | Equipo 1, sobre el ejecutable del corte: peor carga 0,78 s, memoria de trabajo máxima 446 MB (1 179 MB de privada) y paquete de 479,0 MB (apartado 4.3). El segundo equipo y la red apagada se verifican en el OE4 (guiones H3 y H4) | Equipo 1 medido y dentro de los tres presupuestos; segundo equipo y red apagada en el OE4 |
| 10 | Datos: borrar un perfil es irreversible, exige confirmación explícita y no deja residuos (RF-47, RNF-11) | Punto de control P-D: `EraseDialog_RF47_ExigeConfirmacionExplicitaYAdvierteIrreversibilidad`, `EraseDialog_CU12_CancelarNoRealizaNingunCambioEnDisco`, `SaveStore_RF47_BorraElPerfilDeLasDosRutas`, `SaveStore_RF47_NoAfectaAOtrosPerfiles` y `ProfileEraser_RNF11_SobreDiscoRealNoQuedaNingunaEntradaDelPerfil`; sobre el ejecutable, ningún rastro de los cuatro perfiles borrados en el primer candidato, dentro ni fuera de la carpeta portable, y en el del corte los perfiles borrados desaparecen de `Datos/` | Cumple. Fuera de `Datos/` quedan `Player.log` y la clave del reproductor, sin datos del estudiante, con identificadores del motor documentados como residuo (DEF-RC2-01) |

Los criterios 6, 8, 9 y 10 se revisaron de nuevo con el ejecutable del corte, sobre el que la evaluación funcional del OE4 reverificó las correcciones de los defectos que había hallado en el primer candidato (documento principal, apartado 9.5).

## 5. Archivos principales

**Tabla G.11.** Dónde está el incremento en el repositorio.

| Carpeta o archivo | Qué es |
|---|---|
| `Assets/Game/Scripts/Runtime/Reporting/` | `ProfileRepository`, `IndicatorReport`, `PhaseIndicators`, `LevelReportSection`, `ReportContent` y `ResolutionTimeFormat` |
| `Assets/Game/Scripts/Runtime/UI/` | `TeacherReportController`, `IndicatorTableView`, `EraseConfirmationDialog`, `ProfileSelectContent`, `MainMenuController` y `LevelSelectController` |
| `Assets/Game/Scripts/Runtime/Core/` | `SaveStore`, `ProfileSession`, `LevelUnlockPolicy`, `SceneLoader` y `GameFlowRunner` |
| `Assets/Game/Data/Reporting/ReportContent.asset`, `Assets/Game/Data/ProfileSelectContent.asset`, `Assets/Game/Data/GameTitleConfig.asset` | Textos del informe, del panel de perfiles y del aviso de salida |
| `Assets/Game/Scenes/TeacherReport.unity`, `MainMenu.unity`, `LevelSelect.unity` | Informe docente, menú principal con el panel de perfiles y menú de niveles |
| `ProjectSettings/ProjectSettings.asset` | Identidad y configuración del ejecutable |
| `Assets/Tests/EditMode/Reporting/`, `Assets/Tests/EditMode/Content/` | Pruebas del módulo de informes y barrido de contenido |
| `Assets/Tests/EditMode/Architecture/` | `AssemblyDependencyTest`, `PlayerSettingsTest`, `SceneTextTest`, `InputSchemeTest` y `TraceabilityTests` |
| `Assets/Tests/PlayMode/UI/` y `Assets/Tests/PlayMode/Core/` | `TeacherReportTests`, `EraseDialogTests`, `MainMenuTests`, `LevelSelectTests`, `ProfileSelectTests` y `SceneLoaderTests` |
| `claudeDocs/tasks/OE4/` | Plan, catálogo de casos y tablero del OE4; hoja de verificaciones manuales; formato de consentimiento; arnés `oe4.ps1`, corredor `editor.ps1` y perfiles semilla en `herramientas/`; evidencias en `evidencias/` |

## 6. Evaluación del OE4 y verificaciones a cargo de Santiago Benavides Rey

Este objetivo entrega al OE4 la suite automatizada, la matriz del Anexo A, el ejecutable medido con su arnés y sus perfiles semilla, la hoja de verificaciones manuales con los guiones H1 a H13 y el formato de consentimiento. El OE4 ejecuta sobre el ejecutable el catálogo de casos del plan y calcula con ellos el porcentaje de funcionalidad verificada del criterio 7.

De las ocho casillas que siguen abiertas en los cuatro tableros, las de este incremento son la columna del segundo equipo y la fila del recorrido cronometrado de P12 y la de CT-02, a cargo de Santiago Benavides Rey, como fijó el acta D10. Con el guion H3 llena esa columna y comprueba la carpeta portable en otro equipo (RNF-07); con el H5, en un equipo sin tarjeta gráfica dedicada, comprueba CT-02 dentro de los presupuestos de carga y memoria, y con el H8 cronometra un recorrido completo, que debe durar entre 20 y 40 minutos. En la misma sesión, con el H4, ejecuta el juego con el adaptador de red deshabilitado (RNF-08), sobre el ejecutable del corte. La sesión con estudiantes, en la que se observan PG-05 y PG-06 (guiones H1 y H2), va precedida del consentimiento firmado por los acudientes y del asentimiento de los estudiantes (RNF-12), que Santiago recoge antes de la sesión. La parte visual de la inspección de contenido (RNF-22) se hace sobre las capturas del recorrido completo. Estas casillas se marcan cuando Santiago entregue los resultados y no pasan a trabajos futuros.

Lo que la primera pasada de la evaluación funcional halló en este incremento está corregido en el ejecutable del corte y se reverificó sobre él: los servicios de Unity que abrían conexiones de red, la lista de perfiles que con ocho no dejaba alcanzar los últimos, el botón «Cerrar el juego» con el icono de la papelera y el rótulo partido, el perfil ilegible que no mostraba ningún aviso y el guardado que escribía el perfil directamente, y el informe que no decía qué perfil se estaba mirando. Quedan, documentados con su arreglo propuesto y sin corregir en este corte, el residuo del motor en el registro (DEF-RC2-01), el riesgo residual de la ventana interna del reemplazo del guardado y las observaciones menores del panel de perfiles y del informe: el aviso del perfil ilegible en la columna «Perfil nuevo», la flecha de página que se oculta en vez de apagarse, la octava fila recortada, los iconos sobre la cabecera y el texto de muestra «Perfil de Ana» en la escena (documento principal, apartado 10.4).
