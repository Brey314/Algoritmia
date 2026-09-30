# 3. ARQUITECTURA E IMPLEMENTACIÓN BASE

Este capítulo corresponde a la actividad «Implementación de la estructura base del videojuego» del cronograma y a su entregable, el *build inicial*. Describe la arquitectura que quedó implementada en el prototipo, la contrasta con la diseñada en el segundo objetivo y documenta los tres elementos que sostienen a todos los niveles: el flujo de estados y escenas, el contenido fuera del código y la persistencia local. Los conteos de este capítulo —assemblies, escenas, assets y archivos— se verificaron contra el repositorio en el commit `ccf77e6` del 25/09/2026; el detalle de la construcción de los cimientos está en el Anexo B, Fases 0 y 1.

## 3.1 Arquitectura implementada frente a la diseñada

El entregable del segundo objetivo (OE2 §4) adoptó la combinación de una máquina de estados finitos (FSM), un gestor de escenas y una arquitectura por capas, por encima de ECS y de MVC puro. El prototipo conserva esa decisión de fondo: una máquina de estados dirige el flujo del juego, un cargador asíncrono cambia de escena y el código se organiza en capas con dependencias en un solo sentido.

Lo que cambió no es la arquitectura sino su ajuste al juego concreto. El capítulo 4 del documento de diseño refundido reproduce una versión anterior a la alineación del documento de arquitectura (`arquitectura_videojuego_v2`), que ya había corregido los puntos que no correspondían a este proyecto. La contradicción está registrada como INC-48, todavía abierta en los documentos; la decisión del 14/09/2026 es que rige la versión alineada, que es la que implementa el código. La Tabla 3.1 resume las diferencias.

**Tabla 3.1.** Diferencias entre la arquitectura de OE2 §4 y la implementada.

| Aspecto | OE2 §4 | Prototipo implementado | Justificación |
| --- | --- | --- | --- |
| Director del flujo | `GameManager` singleton con la FSM dentro | FSM en C# plano (`GameFlow`) y adaptador singleton (`GameFlowRunner`) | RNF-13, CT-10 |
| Estados | Ocho, uno por escena | Nueve, con `Narrative` y `Playing` parametrizados | RF-05, RF-22, RF-27, RF-30 |
| Escenas narrativas | Cinemáticas en `StreamingAssets` | Ilustración fija y diálogo leídos de un asset | RF-05, RNF-04, RNF-06 |
| Persistencia | `Application.persistentDataPath` | JSON en `Datos/` junto al ejecutable | RNF-07, RNF-11, INC-34 |
| Comunicación | `EventBus` global | Interfaz inyectada; evento solo con varios oyentes | Legibilidad del flujo |
| Entidades | `EntityManager` con enemigos | No existe | El guion no tiene enemigos |
| Interfaz y audio | Sin assembly propio | Assemblies `Game.UI` y `Game.Audio` | INC-40 |

La primera fila es la de mayor efecto sobre la verificación. `GameFlow` no depende de Unity: es una clase de C# que conoce los estados y las transiciones legales, y `GameFlowRunner` se limita a traducir cada estado a una escena. Por eso el recorrido completo del Golden Path se comprueba en una prueba automática que no carga escenas ni espera cuadros (`GameFlow_RNF13_RecorreElGoldenPathCompletoSinEstadoIrrecuperable`, véase §9.2). El mismo criterio se aplicó a los niveles: la lógica de cada mecánica —validadores, contadores, máquinas de estado— es C# plano, y el componente de Unity actúa como adaptador.

Los criterios pedagógicos también tienen efecto arquitectónico. El enumerado de estados no contiene ningún estado de derrota (CP-02) y el perfil guardado no tiene campo de puntaje (CP-03, RF-17); en ambos casos el código deja escrita la razón pedagógica para que una modificación posterior no los reintroduzca.

Algunos componentes del documento de arquitectura se materializaron con otra forma. No existe una clase `GameBootstrap`: `Boot` es una escena con un solo objeto persistente, y el `Start` de `GameFlowRunner` pasa el control a la pantalla de inicio (RF-01). El registro de indicadores se repartió entre un colector por nivel (`FireIndicatorCollector`, `WheelIndicatorCollector`, `RiverIndicatorCollector`) y el assembly `Game.Reporting`, que los agrega para el informe docente. No hay un controlador de interfaz único: cada escena jugable tiene el suyo.

Se mantienen exactamente tres servicios que sobreviven al cambio de escena, marcados con `DontDestroyOnLoad`: `GameFlowRunner`, `SceneLoader` y `AudioManager`. Los tres son componentes del objeto `Persistent` de la escena `Boot`, y en cada uno la copia que despierta cuando ya existe una instancia viva se destruye a sí misma, de modo que volver a una escena ya visitada no duplica el flujo, el cargador ni el audio. `AudioManager` aloja además el único `AudioListener` del juego, condición que vigila una prueba de arquitectura.

## 3.2 Módulos y dependencias

El código de producción se reparte en ocho assemblies de ejecución y uno exclusivo del Editor, cada uno con su archivo `.asmdef`. La Tabla 3.2 los enumera con las referencias que declara cada archivo; «uGUI» abrevia `UnityEngine.UI`.

**Tabla 3.2.** Assemblies del prototipo y sus dependencias.

| Módulo | Responsabilidad | Depende de |
| --- | --- | --- |
| `Game.Core` | FSM, carga de escenas, perfil y guardado | Ninguno |
| `Game.Scaffolding` | Narrativa, guía, pistas, encuadres y personajes | Core, uGUI |
| `Game.Levels.Fire` | Nivel 1: el encendido del fuego | Core, Scaffolding, Audio, uGUI |
| `Game.Levels.Wheel` | Nivel 2: bosque, taller y laberinto | Core, Scaffolding, Audio, uGUI, Input System |
| `Game.Levels.River` | Nivel 3: recolección y balsa | Core, Scaffolding, Audio, uGUI |
| `Game.Reporting` | Agregación de indicadores y lista de perfiles | Core |
| `Game.Audio` | Gestor de audio con cuatro buses | Core |
| `Game.UI` | Controladores de pantallas y de la narrativa | Core, Scaffolding, Audio, Reporting, uGUI |
| `Game.EditorTools` | Reglas de importación y arranque desde `Boot` | Test Runner, uGUI, Input System; ningún `Game.*` (solo Editor) |

Tres reglas se desprenden de la tabla y las tres están cubiertas por pruebas que leen los `.asmdef` del disco:

- `Game.Core` no declara referencias a ningún otro módulo del juego; la prueba `Architecture_RNF16_CoreNoDependeDeUINiDeAudioNiDeNiveles` vigila que no dependa de la interfaz, del audio ni de los niveles.
- Ningún assembly de nivel referencia a otro nivel. Es lo que hace ejecutable la prueba de exclusión de RNF-16 («agregar o retirar un nivel no requiera modificar los demás»): `Architecture_RNF16_NingunAssemblyDeNivelReferenciaAOtroNivel` comprueba los tres niveles entre sí, y `Architecture_RNF16_RetirarUnNivelNoAfectaALosOtrosDos` verifica, para cada nivel, que ningún otro módulo depende de él y que ninguna escena ajena ni prefab compartido referencia sus scripts.
- `Game.Reporting` referencia únicamente a `Game.Core`, de modo que retirar un nivel no rompe el informe docente (`Architecture_RNF16_ReportingNoReferenciaANingunAssemblyDeNivel` y `Architecture_RNF16_RetirarUnNivelNoRompeElInformeDocente`).

Conviene precisar el alcance de esa verificación: la exclusión se prueba sobre las dependencias declaradas y las referencias serializadas en escenas y prefabs, sin retirar físicamente un nivel del proyecto y ejecutar los restantes, que es la forma literal del criterio de RNF-16.

La columna de dependencias refleja también la restricción de entrada (CT-06, RNF-02). `Game.Levels.River` no referencia el Input System a propósito: las flechas del Nivel 3 son botones en pantalla accionados con clic sostenido, y una prueba de arquitectura impide que ningún assembly use la clase `Input` heredada.

Cada módulo tiene su assembly de pruebas: once de EditMode y seis de PlayMode. Dos son excepciones deliberadas: `Game.Architecture.Tests` no referencia ningún módulo del juego porque lee los `.asmdef` del disco, y `Game.Content.Tests` es el único que ve los tres niveles a la vez, para auditar el contenido completo (véase §9.1). En `ccf77e6` el código de los ocho assemblies de ejecución suma 133 archivos C# y 15 762 líneas en `Assets/Game/Scripts/Runtime/` —139 archivos y 16 372 líneas con las herramientas del Editor—, y el de pruebas 91 archivos y 18 169 líneas en `Assets/Tests/`; el módulo más extenso es `Game.Levels.Wheel` (23 archivos, 5 209 líneas), que aloja las tres escenas jugables del Nivel 2.

## 3.3 Flujo de estados y escenas

El enumerado `GameState` define nueve estados: `Boot`, `MainMenu`, `ProfileSelect`, `LevelSelect`, `Narrative`, `Playing`, `LevelSummary`, `Credits` y `TeacherReport`. Dos de ellos están parametrizados. `Narrative` lleva el identificador de la secuencia narrativa que debe reproducir, y `Playing` lleva el nivel y la fase. Con ello, las quince escenas narrativas del guion y las siete fases jugables no multiplican los estados ni las ramas de la máquina.

El identificador de la secuencia es una cadena y no el asset mismo porque el tipo `NarrativeSequence` vive en `Game.Scaffolding`, que depende de `Game.Core`; pasar el asset por la FSM cerraría un ciclo entre assemblies. La traducción del identificador al asset la hace la capa superior (Anexo B, Fase 0, §2.3).

El juego se compone de doce escenas de Unity, registradas en la configuración de compilación en el orden de la Tabla 3.3, con `Boot` en primer lugar.

**Tabla 3.3.** Escenas del build y estado de la FSM que alojan.

| N.º | Escena | Estado de la FSM | Contenido |
| --- | --- | --- | --- |
| 1 | `Boot` | `Boot` | Los tres servicios persistentes |
| 2 | `MainMenu` | `MainMenu`, `ProfileSelect` | Inicio y panel de perfiles (RF-01, RF-02) |
| 3 | `LevelSelect` | `LevelSelect` | Menú de niveles con desbloqueo (RF-03) |
| 4 | `Credits` | `Credits` | Créditos (RF-08) |
| 5 | `Narrative` | `Narrative` | Las dieciocho secuencias narrativas (RF-05) |
| 6 | `Level1_Cave` | `Playing` (N1, fase 1) | Encendido del fuego |
| 7 | `LevelSummary` | `LevelSummary` | Resumen de fin de nivel (RF-45) |
| 8 | `Level2_Forest` | `Playing` (N2, fase 1) | El bosque |
| 9 | `Level2_Workshop` | `Playing` (N2, fase 2) | El taller |
| 10 | `Level2_Maze` | `Playing` (N2, fase 3) | El laberinto |
| 11 | `Level3_River` | `Playing` (N3, fases 1 a 3) | Recolección y balsa |
| 12 | `TeacherReport` | `TeacherReport` | Informe docente y borrado (RF-46, RF-47) |

`ProfileSelect` no tiene escena propia: es un panel dentro de `MainMenu`, y la transición se resuelve intercambiando paneles sin recargar, según se decidió en el acta D03 (06/09/2026; Anexo H): recargar la escena descartaría y volvería a levantar sin necesidad todo lo que ya está en pantalla. El Nivel 3 se juega entero en `Level3_River`: la recolección no es una fase persistida y precede, en la misma escena, a la fase 1 (la base); si se retoma en la fase 2 o 3, la escena abre directamente en el ensamblaje (véase §4.4).

Las transiciones legales están fijadas en una tabla dentro de `GameFlow` (Tabla 3.4). Una transición que no figura en ella no lanza una excepción: devuelve falso y deja el estado como estaba, de manera que un clic a destiempo no puede dejar al estudiante en una pantalla sin salida.

**Tabla 3.4.** Transiciones permitidas por la máquina de estados.

| Estado | Puede pasar a |
| --- | --- |
| `Boot` | `MainMenu` |
| `MainMenu` | `ProfileSelect`, `Credits`, `TeacherReport` |
| `ProfileSelect` | `LevelSelect`, `MainMenu` |
| `LevelSelect` | `Narrative`, `Playing`, `MainMenu` |
| `Narrative` | `Narrative`, `Playing`, `LevelSummary`, `LevelSelect`, `Credits` |
| `Playing` | `Playing`, `Narrative`, `LevelSummary`, `LevelSelect`, `MainMenu` |
| `LevelSummary` | `Narrative`, `LevelSelect`, `MainMenu` |
| `Credits` | `MainMenu` |
| `TeacherReport` | `MainMenu` |

Tres de esas transiciones merecen explicación. `Narrative → Narrative` encadena dos secuencias seguidas del guion sin pasar por el menú. `Playing → Playing` es reiniciar la fase desde el menú de pausa o entrar a la fase siguiente del mismo nivel. `Narrative → Credits` es el cierre del juego: tras el resumen del Nivel 3 se reproduce la escena final y de ella se pasa a los créditos (RF-44, INC-39). Cuando se pide una fase ya confirmada, `GameFlow` retoma en la primera fase pendiente que tenga escena, que es lo que permite continuar tras un cierre inesperado sin una rama por nivel (RNF-14).

El menú de pausa (RF-07) no es un estado de la máquina sino una capa de interfaz: un mismo prefab instanciado en las cinco escenas jugables, que detiene el tiempo del juego mientras está abierto. Sus opciones siguen el mockup 6 y no los rótulos que nombran RF-07 y HU-17 (INC-49, abierta); se describen en §7.1.

`SceneLoader` carga cada escena con `SceneManager.LoadSceneAsync` y anota cuánto tardó la carga, dato que usa la medición de RNF-04 (véase §8.1). Desde el 23/09/2026, al pasar de una narrativa a una mecánica, de una mecánica a una narrativa o entre dos narrativas encadenadas, funde la pantalla a negro y de vuelta en 0,4 s por mitad; en los demás cambios de escena el corte es inmediato.

## 3.4 Contenido parametrizable

El principio adoptado (CT-05, RNF-18) es que todo texto visible y todo parámetro que se ajusta jugando vivan fuera del código. Las narrativas, el guía, los mensajes y parámetros de los tres niveles, los resúmenes de fin de nivel, el título del juego, los créditos y los rótulos del informe docente se declaran en ScriptableObjects de Unity; cada campo se declara serializado y con una descripción emergente en el Inspector, de modo que un diálogo, un mensaje de retroalimentación o un parámetro de nivel se modifican sin tocar ni recompilar el código C#. El principio tiene todavía excepciones, cuyos textos siguen escritos en C#: los avisos y la confirmación de borrado del panel de perfiles, la confirmación de borrado del informe docente y los rótulos de los bloques del laberinto («Avanzar», «Retroceder», «Girar»). En el ejecutable, en cambio, los ScriptableObjects quedan empaquetados: un cambio de contenido no toca el código, pero exige regenerar el build. La carpeta `Assets/Game/Data/` contiene 38 assets de contenido, agrupados en la Tabla 3.5.

**Tabla 3.5.** Assets de contenido en `Assets/Game/Data/`.

| Categoría | Tipos | Assets |
| --- | --- | --- |
| Secuencias narrativas | `NarrativeSequence` | 18 |
| Guía por nivel | `GuideContent` | 3 |
| Mecánica del Nivel 1 | `FireLevelConfig`, `FireMessages` | 2 |
| Mecánicas del Nivel 2 | `WheelLevelConfig`, `AssemblyContent`, `MazeLayout`, `RollingLogLook` | 4 |
| Mecánica del Nivel 3 | `RiverLevelConfig`, `RaftAssemblyContent` | 2 |
| Sonido por nivel | `FireSounds`, `WheelSounds`, `RiverSounds` | 3 |
| Mensajes del resumen de fin de nivel | `LevelSummaryMessages` | 3 |
| Pantallas de flujo | `GameTitleConfig`, `CreditsContent`, `ReportContent` | 3 |
| **Total** | | **38** |

Las dieciocho secuencias narrativas —cuatro del Nivel 1, siete del Nivel 2 y siete del Nivel 3— cubren las quince escenas narrativas del guion. La diferencia se debe a que la ilustración es una por secuencia, de modo que una escena que cambia de fondo se reparte en varias secuencias encadenadas. El encadenamiento se declara en el propio asset (`NextSequenceId`); la escena 3.2, que solo aparece tras el primer fallo en la prueba de la balsa, la decide una regla separada (`ConditionalNarrativeTrigger`) y no una condición dentro del controlador. En consecuencia, añadir o encadenar una escena narrativa consiste en crear o editar un asset, no en agregar un estado, una escena y una rama del código (véase §4.5).

![Figura 3.1. La escena `Narrative` reproduciendo la secuencia `N1_AparicionGuia` (escena 1.1 del guion). La ilustración, la posición y animación de los personajes, la línea de diálogo con el nombre de quien habla y la capa de oscuridad del Nivel 1 se leen del asset, y el retrato lo aporta el prefab del personaje que habla; la misma escena de Unity reproduce las dieciocho secuencias del juego.](fig/fig-03-1-narrativa-desde-asset.jpg){width=16cm}

La separación entre contenido y código se verifica además en sentido pedagógico: una prueba de contenido recorre el grafo serializado de los tipos de contenido que ve el estudiante y falla si encuentra una cifra, que CP-03 y RF-17 prohíben (Anexo G, §4).

## 3.5 Persistencia de perfiles y privacidad

Cada perfil se guarda como un archivo JSON en una carpeta `Datos/` situada junto al ejecutable (`SaveStore`, `Game.Core`). El diseño de OE2 §4 proponía `Application.persistentDataPath`, que en Windows escribe en `%AppData%\LocalLow`, fuera de la carpeta portable; con ella, «sin instalación» (RNF-07) y «sin residuos» (RNF-11) dejaban de significar lo mismo, porque un archivo en esa ruta sobrevive a borrar la carpeta del juego. `persistentDataPath` se conserva solo como ruta de respaldo, para el equipo en que la carpeta del ejecutable no sea escribible; en ese caso el almacén lo expone para poder advertir al docente (INC-34). Cuando el juego corre dentro del Editor, `Datos/` cae en la raíz del proyecto y está excluida del control de versiones.

El perfil persistido es una lista cerrada de campos (RNF-09), recogida en la Tabla 3.6. No hay campo de puntaje, ni imágenes, ubicación o datos de contacto.

**Tabla 3.6.** Campos del JSON de un perfil.

| Campo | Contenido | Requerimiento |
| --- | --- | --- |
| `name` | Nombre o alias del estudiante | RF-02 |
| `reachedLevel` | Nivel más avanzado habilitado | RF-03 |
| `phases[].level`, `phases[].phase` | Fases confirmadas | RF-04 |
| `phases[].attempts`, `phases[].correctedErrors` | Intentos y errores corregidos de la fase | RF-45 |
| `phases[].stepsUsed`, `phases[].resolutionSeconds` | Pasos utilizados y tiempo de resolución de la fase | RF-45 |

El build portable del 08/09/2026 produjo, al crear un perfil jugando, el archivo `Datos/yo.json` con el contenido `{"name":"yo","reachedLevel":1,"phases":[]}`, sin ningún campo adicional (Anexo B, Fase 1, §4.1). El guardado ocurre al crear el perfil, al confirmar cada fase (RF-04) y al salir desde el menú principal (RF-09), no en cada acción; es lo que hace verificable la recuperación tras un cierre inesperado (RNF-14). El nombre del perfil se valida al crearlo: además del nombre vacío y del duplicado, se rechaza el que no sirve como nombre de archivo, porque un nombre con separadores de ruta escribiría fuera de `Datos/`.

Los dos accesos a la eliminación definitiva (RF-47, RNF-11), desde el panel de perfiles y desde el informe docente, se describen en §6.4. En lo que toca al almacén, ambos llaman al mismo método, que borra el archivo de las dos rutas —`Datos/` y la de respaldo— y devuelve falso si queda algún rastro, porque un borrado parcial es un residuo. Las pruebas cubren el borrado en las dos rutas, el borrado parcial reportado como fallo y la caída a la ruta de respaldo con un sistema de archivos simulado. La prueba sobre disco real del escenario de solo lectura se omite en el equipo de desarrollo, que no hace cumplir el atributo de solo lectura sobre carpetas (Anexo G, «Corrida completa de la suite»).

## 3.6 Distribución portable

El entregable es la carpeta de salida del build de Windows, que se genera con la interfaz de línea de comandos de Unity:

`unity build --target StandaloneWindows64 -o "Build/Algoritmia/Algoritmia.exe"`

Entran las doce escenas de la Tabla 3.3, con `Boot` en primer lugar. La carpeta no requiere instalación: en la primera ejecución el juego crea `Datos/` junto a `Algoritmia.exe`, sin tocar el registro del sistema, lo que se comprobó sobre el build del 08/09/2026 (Anexo B, Fase 1, §4.1; acta D04, Anexo H). Sobre esa carpeta se miden los presupuestos de carga, memoria y tamaño del paquete (véase el capítulo 8).

Constan dos builds medidos: el del 08/09/2026, con cinco escenas y 137 MB, y el del 21/09/2026, con once escenas y 217 MB (Anexo B, Fase 1, §4.1; Anexo D, §6). No consta un build posterior a la incorporación de `TeacherReport` el 24/09/2026, de modo que el paquete de doce escenas con el arte y el sonido finales está pendiente de generarse y medirse; las actas D06 a D09 mantienen abiertas las comprobaciones que lo exigen (véase §8.1).

Los builds dejaron además tres hallazgos abiertos, que dependen de decisiones sobre la configuración del proyecto y no del código del juego:

- El reproductor de Unity escribe `Player.log` y archivos del módulo de analítica en `%AppData%\LocalLow`, fuera de `Datos/`. Ningún dato del estudiante vive ahí, pero RNF-11 exige ausencia de residuos y la analítica es ajena a un prototipo que no debe transmitir datos (RNF-08, RNF-10). La corrección consiste en desactivar el registro del reproductor y retirar el módulo `com.unity.modules.unityanalytics` del manifiesto de paquetes; a la fecha de corte ambos siguen activos (Anexo B, Fase 1, nota inicial y §4.1). Mientras el módulo siga en el manifiesto, la analítica de la configuración del proyecto (`UnityConnectSettings`) vuelve a encenderse sola al reabrir el Editor, aunque se haya revertido a mano (Anexo B, Fase 1, §4.1).
- El paquete incluye `DirectML.dll` (14 MB) y la carpeta `D3D12/`, que provienen de los paquetes de inteligencia artificial del Editor (`com.unity.ai.*`); retirarlos del manifiesto está pendiente de decisión (Anexo D, §8). El build del 21/09/2026 activó además, sin que se pidiera, el símbolo de compilación `SENTIS_ANALYTICS_ENABLED` y la analítica de `UnityConnectSettings`; ambos se revirtieron a mano (Anexo D, §8), pero el símbolo figura de nuevo en la configuración del proyecto versionada en `ccf77e6`.
- El build genera una carpeta de información de depuración de Burst (1 MB) que no forma parte del entregable y se retira a mano (Anexo D, §6).

Estos puntos se retoman en el capítulo 10.
