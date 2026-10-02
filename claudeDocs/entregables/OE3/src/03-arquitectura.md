# 3. ARQUITECTURA E IMPLEMENTACIÓN BASE

Este capítulo corresponde a la actividad «Implementación de la estructura base del videojuego» del cronograma y a su entregable, el *build inicial*. Describe la arquitectura implementada en el prototipo, la compara con el primer diseño del segundo objetivo y documenta los tres elementos sobre los que se apoyan todos los niveles: el flujo de estados y escenas, el contenido separado del código y la persistencia local. Los conteos del capítulo (assemblies, escenas, assets y archivos) se verificaron en el corte del 1 de octubre de 2026. La construcción de los cimientos se detalla en el Anexo B, apartados 2.1 y 2.2.

## 3.1 Arquitectura implementada frente a la diseñada

El entregable del segundo objetivo (Solución OE2, apartado 4) eligió combinar una máquina de estados finitos (FSM), un gestor de escenas y una organización por capas, frente a las opciones de ECS y de MVC puro. El prototipo implementa esa combinación: `GameFlow` decide en qué estado está el juego, `SceneLoader` carga cada escena de forma asíncrona y los módulos de código dependen unos de otros en una sola dirección.

La primera redacción de ese capítulo describía, sin embargo, componentes que el juego no tiene. Al refundir los documentos fuente, el 14/09/2026, Solución OE2 recibió una versión del capítulo de arquitectura anterior a su alineación con el proyecto (INC-48). Ese hallazgo y los INC-94 a INC-96 se cerraron el 29/09/2026 corrigiendo los documentos: el apartado 4 de Solución OE2 resume hoy la arquitectura implementada y remite al documento de arquitectura del videojuego, alineado a su vez con el código. La Tabla 3.1 compara el primer diseño con lo implementado.

**Tabla 3.1.** Decisiones de arquitectura del primer diseño frente a la implementación.

| Aspecto | Primer diseño | Implementación (hoy en Solución OE2, apartado 4) | Justificación |
| --- | --- | --- | --- |
| Director del flujo | `GameManager` singleton con la FSM dentro | FSM en C# plano (`GameFlow`) y adaptador singleton (`GameFlowRunner`) | RNF-13, CT-10 |
| Estados | Ocho, uno por escena | Nueve, con `Narrative` y `Playing` parametrizados | RF-05, RF-22, RF-27, RF-30 |
| Escenas narrativas | Cinemáticas en `StreamingAssets` | Ilustración fija recorrida por la cámara, con el diálogo leído de un asset | RF-05, RNF-04, RNF-06 |
| Persistencia | `Application.persistentDataPath` | JSON en `Datos/` junto al ejecutable, con ruta de respaldo | RNF-07, RNF-11, INC-34 |
| Comunicación | `EventBus` global | Interfaz inyectada; evento solo con varios oyentes | Legibilidad del flujo |
| Entidades | `EntityManager` con enemigos | No existe | El guion no tiene enemigos |
| Interfaz y audio | Sin assembly propio | Assemblies `Game.UI` y `Game.Audio` | INC-40 |

La primera fila es la que más pesa en la verificación. `GameFlow` no depende de Unity: es una clase de C# que conoce los estados y las transiciones válidas entre ellos, y `GameFlowRunner` solo traduce cada estado a la escena que le corresponde. Gracias a esa separación, el recorrido completo del Golden Path se comprueba con una prueba automática que no carga escenas ni espera fotogramas (`GameFlow_RNF13_RecorreElGoldenPathCompletoSinEstadoIrrecuperable`, apartado 9.2). Los niveles siguen el mismo criterio: validadores, contadores y máquinas de estado de cada mecánica son C# plano, y el componente de Unity se limita a conectarlos con la escena.

Los criterios pedagógicos también dejaron huella en la arquitectura. El enumerado de estados no tiene ningún estado de derrota (CP-02) y el perfil guardado no tiene campo de puntaje (CP-03, RF-17); en los dos casos el código explica en un comentario la razón pedagógica, para que una modificación posterior no los reintroduzca.

Varios componentes del documento de arquitectura tomaron otra forma al implementarse. No hay una clase `GameBootstrap`: `Boot` es una escena con un único objeto persistente, y el método `Start` de `GameFlowRunner` entrega el control a la pantalla de inicio (RF-01). El registro de indicadores se repartió entre un colector por nivel (`FireIndicatorCollector`, `WheelIndicatorCollector`, `RiverIndicatorCollector`) y el assembly `Game.Reporting`, que los ordena para el informe docente. Tampoco hay un controlador de interfaz único, porque cada escena jugable tiene el suyo.

Únicamente tres componentes llevan `DontDestroyOnLoad` y persisten de una escena a otra: `GameFlowRunner`, `SceneLoader` y `AudioManager`. Los tres viven en el objeto `Persistent` de la escena `Boot`, y cada uno se elimina a sí mismo si al despertar encuentra otra instancia activa, de modo que volver a una escena ya visitada no duplica el flujo, el cargador ni el audio. `AudioManager` contiene también el único `AudioListener` del juego, condición que vigila la prueba `AudioListener_RNF13_ElUnicoOyenteVaEnBootYNingunaOtraEscenaTieneElSuyo`.

## 3.2 Módulos y dependencias

El código de producción está repartido en ocho assemblies de ejecución y uno exclusivo del Editor, cada uno con su archivo `.asmdef`. La Tabla 3.2 los presenta con las referencias que declara cada archivo; «uGUI» abrevia `UnityEngine.UI`.

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
| `Game.EditorTools` | Reglas de importación, arranque desde `Boot`, licencias junto al ejecutable y servicios de Unity apagados al compilar | Test Runner, uGUI, Input System; ningún `Game.*` (solo Editor) |

De la tabla se desprenden tres reglas, cubiertas por pruebas que leen los `.asmdef` del disco:

- `Game.Core` no referencia ningún otro módulo del juego. La prueba `Architecture_RNF16_CoreNoDependeDeUINiDeAudioNiDeNiveles` impide que llegue a depender de la interfaz, del audio o de los niveles.
- Ningún assembly de nivel referencia a otro nivel, condición que hace posible la exclusión que pide RNF-16. `Architecture_RNF16_NingunAssemblyDeNivelReferenciaAOtroNivel` compara los tres niveles entre sí, y `Architecture_RNF16_RetirarUnNivelNoAfectaALosOtrosDos` comprueba, nivel por nivel, que ningún otro módulo depende de él y que ninguna escena ajena ni prefab compartido usa sus scripts.
- `Game.Reporting` referencia solo a `Game.Core`, así que retirar un nivel deja intacto el informe docente (`Architecture_RNF16_ReportingNoReferenciaANingunAssemblyDeNivel` y `Architecture_RNF16_RetirarUnNivelNoRompeElInformeDocente`).

Las pruebas de la suite verifican la exclusión sobre las dependencias declaradas y sobre las referencias serializadas en escenas y prefabs. El criterio de RNF-16 pide además retirar un nivel y comprobar que los demás se ejecutan, y eso se hizo en copias del proyecto, primero sin el Nivel 2 y luego sin el Nivel 1 (apartado 9.5). Las dos copias compilaron sin errores. Sin el Nivel 2 superaron 354 de 360 casos de EditMode y 245 de 271 de PlayMode, y sin el Nivel 1, 376 de 382 y 281 de 305. Los casos que fallaron no revelan ninguna dependencia entre niveles: son pruebas que exigen el repositorio con sus tres niveles, pruebas que cargan por su nombre una escena del nivel retirado y fallos de entorno de la corrida por lotes a 640 × 480, que se repiten igual sobre el árbol completo. Las copias se hicieron sobre el árbol del primer candidato del ejecutable; el del corte no cambia ninguna definición de assembly, y las pruebas de arquitectura de RNF-16 pasan en su suite. OE1 enuncia desde el 29/09/2026 CT-04 y RNF-16 en los mismos términos que el prototipo: cada nivel en sus propias escenas (el Nivel 2, una por fase) y en su propio módulo de código, sin referencias a otro nivel (INC-98).

La columna de dependencias refleja también la restricción de entrada (CT-06, RNF-02). `Game.Levels.River` no referencia el Input System a propósito, porque las flechas del Nivel 3 son botones en pantalla que responden al clic sostenido, y una prueba de arquitectura impide que algún assembly use la clase `Input` heredada.

Cada módulo tiene su assembly de pruebas: once de EditMode y seis de PlayMode. Dos son excepciones deliberadas. `Game.Architecture.Tests` no referencia ningún módulo del juego porque lee los `.asmdef` del disco, y `Game.Content.Tests` es el único que ve los tres niveles a la vez, para revisar el contenido completo (apartado 9.1). En el corte, el código de los ocho assemblies de ejecución suma 139 archivos C# y 17 128 líneas en `Assets/Game/Scripts/Runtime/`; las herramientas del Editor, 9 archivos y 841 líneas en `Assets/Game/Scripts/Editor/`, y las pruebas, 101 archivos y 22 127 líneas en `Assets/Tests/`. El módulo más extenso es `Game.Levels.Wheel` (24 archivos, 5 433 líneas), que contiene las tres escenas jugables del Nivel 2.

## 3.3 Flujo de estados y escenas

El enumerado `GameState` define nueve estados: `Boot`, `MainMenu`, `ProfileSelect`, `LevelSelect`, `Narrative`, `Playing`, `LevelSummary`, `Credits` y `TeacherReport`. Dos llevan parámetros. `Narrative` recibe el identificador de la secuencia narrativa que debe reproducir, y `Playing`, el nivel y la fase. Las quince escenas narrativas del guion y las siete fases jugables caben así en dos estados, sin multiplicar los estados ni las ramas de la máquina.

El estado guarda el identificador de la secuencia como una cadena porque el tipo `NarrativeSequence` pertenece a `Game.Scaffolding`, que ya depende de `Game.Core`; si el núcleo conociera ese tipo, cada módulo dependería del otro. La capa de arriba traduce el identificador al asset (Anexo B, apartado 2.1).

El juego tiene doce escenas de Unity, registradas en la configuración de compilación en el orden de la Tabla 3.3, con `Boot` en primer lugar.

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

`ProfileSelect` no tiene escena propia: es un panel dentro de `MainMenu`, y el paso entre los dos estados intercambia paneles sin recargar, como se decidió en la sesión del 06/09/2026 (acta D03); recargar la escena descartaría y volvería a construir sin necesidad todo lo que ya está en pantalla. El Nivel 3 completo se juega en `Level3_River`. La recolección, que no se guarda como fase, precede en la misma escena a la fase 1 (la base), y si el estudiante retoma el nivel en la fase 2 o la 3, la escena abre directamente en el ensamblaje (apartado 4.4).

Las transiciones válidas están en una tabla dentro de `GameFlow` (Tabla 3.4). Pedir una que no figura en ella no interrumpe el juego: la llamada devuelve falso y el estado se mantiene, de modo que un clic que llega cuando la pantalla ya cambió se ignora sin efecto visible.

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

Tres de esas transiciones necesitan explicación. `Narrative → Narrative` encadena dos secuencias seguidas del guion sin pasar por el menú. `Playing → Playing` sirve para reiniciar la fase desde el menú de pausa o para entrar en la fase siguiente del mismo nivel. `Narrative → Credits` cierra el juego: tras el resumen del Nivel 3 se reproduce la escena final y de ella se pasa a los créditos (RF-44, INC-39). Cuando se pide una fase ya confirmada, `GameFlow` retoma en la primera fase sin confirmar que tenga escena, y con ese mismo mecanismo el juego continúa tras un cierre inesperado sin necesitar una rama por nivel (RNF-14).

El flujo recibió tres ajustes el 29 y el 30/09/2026. `GameFlow.TryStartPlaying` consulta la misma regla de desbloqueo que pinta el menú de niveles, `LevelUnlockPolicy.IsUnlocked`: un nivel se puede jugar si el nivel alcanzado lo incluye o si están confirmadas todas las fases del anterior, sin que el perfil gane ningún campo (RNF-09). Con esa regla, un cierre durante la escena de cierre del Nivel 2 ya no deja bloqueado el Nivel 3 (INC-116, prueba `GameFlow_RNF14_EntraAlNivelQueDesbloqueanLasFasesConfirmadasDelAnterior`). En el Nivel 3, cuyas tres fases comparten escena, confirmar una fase la registra como fase activa con `GameFlow.SetPlayingPhase`, sin cambiar de estado ni de escena, y por eso «Reiniciar» vuelve a la fase en curso y no a la recolección (INC-117). Por último, `NarrativeSceneController.Leave` sale de la escena narrativa una sola vez: desde el 29/09/2026 (commit `d105838`) el doble clic de un niño en «Continuar» u «Omitir» al terminar un cierre reflexivo ya no salta el resumen del nivel.

El menú de pausa (RF-07) funciona como una capa de interfaz sobre `Playing`, sin estado propio en la máquina. Es un mismo prefab instanciado en las cinco escenas jugables y detiene el tiempo del juego mientras está abierto. Ofrece «Reanudar», «Reiniciar» y «Volver al menú de niveles», los rótulos del mockup 6, que RF-07 y HU-17 recogen desde el 29/09/2026 (INC-49); el apartado 7.1 los describe.

`SceneLoader` carga cada escena con `SceneManager.LoadSceneAsync` y mide cuánto tarda la carga hasta que la escena queda activa. Desde el 30/09/2026 escribe en el registro del reproductor una línea por carga con el formato `RNF-04: «<escena>» cargó en <s> s`, con tres decimales y punto decimal en cualquier configuración regional, y la cifra nunca llega a la pantalla (CP-03). Esa línea es la que lee el arnés en la medición sobre el ejecutable (apartado 8.3), y la prueba `SceneLoader_RNF04_CadaCargaDejaSuTiempoEnElRegistro` vigila su formato. Desde el 23/09/2026, al pasar de una narrativa a una mecánica, de una mecánica a una narrativa o entre dos narrativas encadenadas, el cargador funde la pantalla a negro y de vuelta en 0,4 s por mitad; los demás cambios de escena son inmediatos.

## 3.4 Contenido parametrizable

El principio adoptado (CT-05, RNF-18) es que todo texto visible y todo parámetro que se ajusta jugando estén fuera del código. Las narrativas, el guía, los mensajes y parámetros de los tres niveles, los resúmenes de fin de nivel, el título del juego, los créditos, el panel de perfiles y los rótulos del informe docente se declaran en ScriptableObjects de Unity. Cada campo es serializado y lleva una descripción emergente en el Inspector, así que un diálogo, un mensaje de retroalimentación o un parámetro de nivel se cambian sin tocar ni recompilar el código C#. Con el cierre de INC-104 el principio quedó sin excepciones: los avisos y la confirmación de borrado del panel de perfiles pasaron a `ProfileSelectContent`, la confirmación de borrado del informe docente a `ReportContent` y los rótulos de los bloques del laberinto («Avanzar», «Retroceder», «Girar») a `MazeLayout`. En el ejecutable los ScriptableObjects van empaquetados, de modo que un cambio de contenido no toca el código pero sí exige volver a compilar el build. La carpeta `Assets/Game/Data/` contiene 39 assets de contenido, agrupados en la Tabla 3.5.

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
| Pantallas de flujo | `GameTitleConfig`, `CreditsContent`, `ReportContent`, `ProfileSelectContent` | 4 |
| **Total** | | **39** |

Las dieciocho secuencias narrativas (cuatro del Nivel 1, siete del Nivel 2 y siete del Nivel 3) cubren las quince escenas narrativas del guion. La diferencia se explica porque cada secuencia tiene una sola ilustración, y una escena que cambia de fondo se reparte en varias secuencias encadenadas. El encadenamiento se declara en el propio asset (`NextSequenceId`). La escena 3.2, que aparece solo tras el primer fallo en la prueba final de la balsa, la decide una regla aparte (`ConditionalNarrativeTrigger`) y no una condición dentro del controlador. Añadir o encadenar una escena narrativa consiste, por tanto, en crear o editar un asset, sin agregar un estado, una escena ni una rama de código (apartado 4.5).

![Figura 3.1. La escena `Narrative` reproduciendo la secuencia `N1_AparicionGuia` (escena 1.1 del guion): Algoritm, en su forma de fuego, se presenta a la familia en la cueva a oscuras, con la luz concentrada a su alrededor. La ilustración, la capa de oscuridad del Nivel 1, la posición y la animación de los personajes y la línea de diálogo («¡Hola, familia! No tengan miedo. Me llamo Algoritm, y vine a acompañarlos.») se leen del asset; el cuadro de diálogo muestra el retrato y el nombre de quien habla junto al botón «Continuar». La misma escena de Unity reproduce las dieciocho secuencias del juego.](fig/fig-03-1-narrativa-desde-asset.jpg){width=16cm}

La separación entre contenido y código se verifica también en sentido pedagógico: una prueba de contenido recorre el grafo serializado de los tipos de contenido que ve el estudiante y falla si encuentra una cifra, que CP-03 y RF-17 prohíben (Anexo G, apartado 2.2).

## 3.5 Persistencia de perfiles y privacidad

Cada perfil se guarda como un archivo JSON en una carpeta `Datos/` situada junto al ejecutable (`SaveStore`, `Game.Core`). El primer diseño proponía `Application.persistentDataPath`, que en Windows apunta a `%AppData%\LocalLow`, fuera de la carpeta portable. Con esa ruta, un perfil sobreviviría al borrado de la carpeta del juego, y la ejecución sin instalación (RNF-07) dejaría de garantizar la ausencia de residuos (RNF-11). `persistentDataPath` se conserva solo como ruta de respaldo, para el equipo en el que la carpeta del ejecutable no admita escritura. En ese caso `ProfileSession` lo informa a la interfaz: la confirmación de «Salir» muestra la carpeta de respaldo en uso y el informe docente presenta un aviso (INC-34, INC-77). Cuando el juego corre dentro del Editor, `Datos/` queda en la raíz del proyecto, excluida del control de versiones.

El perfil persistido es una lista cerrada de campos (RNF-09), recogida en la Tabla 3.6. No hay campo de puntaje, ni imágenes, ubicación o datos de contacto.

**Tabla 3.6.** Campos del JSON de un perfil.

| Campo | Contenido | Requerimiento |
| --- | --- | --- |
| `name` | Nombre o alias del estudiante | RF-02 |
| `reachedLevel` | Nivel más avanzado habilitado | RF-03 |
| `phases[].level`, `phases[].phase` | Fases confirmadas | RF-04 |
| `phases[].attempts`, `phases[].correctedErrors` | Intentos y errores corregidos de la fase | RF-45 |
| `phases[].stepsUsed`, `phases[].resolutionSeconds` | Pasos utilizados y tiempo de resolución de la fase | RF-45 |

El build portable del 08/09/2026 generó, al crear un perfil jugando, el archivo `Datos/yo.json` con el contenido `{"name":"yo","reachedLevel":1,"phases":[]}`, sin ningún campo adicional (Anexo B, apartado 2.2). El juego guarda al crear el perfil, al confirmar cada fase (RF-04) y al salir desde el menú principal (RF-09), y no en cada acción; esa regla es la que permite verificar la recuperación tras un cierre inesperado (RNF-14). Cada guardado escribe primero un temporal, `<perfil>.json.tmp`, y lo pone en su sitio con un reemplazo del sistema de archivos (`File.Replace`, o `File.Move` si el perfil es nuevo), de modo que un cierre a mitad de la escritura deja intacto el perfil anterior; si otro proceso retiene el perfil, el guardado lo copia en su lugar, y un temporal huérfano no se lista y el siguiente guardado lo pisa (`DiskFileSystem`). Un perfil cuyo archivo no se puede leer tampoco detiene el juego: el panel de perfiles avisa «No se pudo abrir ese perfil. Avisa a tu profe.», sin cifras ni culpa, y ese perfil se puede borrar. El nombre del perfil se valida al crearlo (INC-83). Se recortan los espacios de los extremos, el campo admite hasta 24 caracteres, un nombre que ya existe se rechaza sin distinguir mayúsculas y minúsculas, y también se rechaza uno con caracteres que no sirven en un nombre de archivo, porque un separador de ruta haría escribir el perfil fuera de `Datos/`.

Los dos accesos a la eliminación definitiva (RF-47, RNF-11), desde el panel de perfiles y desde el informe docente, se describen en el apartado 6.4. Ambos llaman al mismo método del almacén, que borra el archivo en las dos rutas, `Datos/` y la de respaldo, y devuelve falso si queda algún rastro, porque un borrado parcial también deja un residuo. Las pruebas cubren el borrado en las dos rutas, el borrado parcial informado como fallo y la caída a la ruta de respaldo con un sistema de archivos simulado. La prueba sobre disco real del escenario de solo lectura (`ProfileEraser_INC34_SobreDiscoRealCubreLosDosEscenariosDeAlmacenamiento`) se omite en el equipo de desarrollo, que no aplica el atributo de solo lectura a las carpetas (Anexo G, apartado 4.2).

## 3.6 Distribución portable

El entregable es la carpeta de salida del build de Windows, que puede generarse con la interfaz de línea de comandos de Unity:

`unity build --target StandaloneWindows64 -o "Build/Algoritmia/Algoritmia.exe"`

Entran las doce escenas de la Tabla 3.3, con `Boot` en primer lugar. Tras cada compilación de Windows, `LicenseNotices` (`Game.EditorTools`) deja junto al ejecutable la carpeta `Licencias/` con los avisos de la licencia SIL OFL de las tipografías y de la licencia MIT de los iconos de Phosphor, y hace fallar la compilación si falta uno (INC-127). La carpeta funciona sin instalación: en la primera ejecución el juego crea `Datos/` junto a `Algoritmia.exe`, como se comprobó con el build del 08/09/2026 (Anexo B, apartado 2.2; acta D04, 09/09/2026). Sobre esa carpeta se miden los presupuestos de carga, memoria y tamaño del paquete (capítulo 8).

Hay cuatro builds medidos, más un intento descartado. El del 08/09/2026 tenía cinco escenas y ocupaba 137 MB, y el del 21/09/2026, once escenas y 217 MB (Anexo B, apartado 2.2; Anexo D, apartado 4.2). Los dos últimos, del 01/10/2026, reúnen las doce escenas y ocupan 479,0 MB cada uno. El primero, el primer candidato de entrega, se compiló a las 08:30 y sobre él se hizo la primera pasada de pruebas funcionales del OE4. El del corte, que es el que se entrega, se compiló a las 18:04 con las correcciones de los defectos que esa pasada halló (apartado 9.5). Los dos se compilaron con el Editor abierto, por la interfaz programática del paquete `com.unity.pipeline`, la misma sobre la que trabaja `editor.ps1` (Tabla 2.5), y no con la línea de comandos. La huella del árbol al compilar el del corte, es decir, la SHA-256 de la diferencia entre el árbol de trabajo y `359365e` en las carpetas del juego (`Assets`, `Packages` y `ProjectSettings`), es `f5bb1efefddc69a23fc752604f2449474f4dfaecd38b14d43fc8f057af1fd726` (apartado 8.3). El intento descartado, de la madrugada del mismo día, ocupó 866,7 MB; lo corrigió la importación de los cuadros del fuego y del humo a 1 024 píxeles (INC-130, apartado 8.2).

La configuración del ejecutable se fijó el 29/09/2026 con las decisiones D2 y D3 (INC-97). El ejecutable se identifica como «Algoritmia», con «Universidad Catolica de Colombia» como compañía; la configuración del proyecto desactiva la analítica, las estadísticas de hardware y los servicios de Unity, y el juego arranca sin la pantalla de presentación de Unity y no alterna a pantalla completa con Alt+Intro. Se obtuvo sin retirar paquetes del manifiesto (decisión D3), y lo vigilan las cuatro pruebas de `PlayerSettingsTest`, entre ellas `Architecture_RNF10_ElEjecutableNoEnviaAnaliticaNiEstadisticasDeHardware`. Esas pruebas leen la configuración guardada en el repositorio, y la compilación serializa en cambio los ajustes que el Editor tiene en memoria. Con el primer candidato se vio la diferencia: el interruptor general de los servicios de Unity estaba encendido en el Editor aunque el archivo dijera lo contrario, y ese ejecutable abría conexiones de red y dejaba datos de analítica fuera de `Datos/`. Desde el ejecutable del corte lo impide `UnityServicesOff` (`Game.EditorTools`), un paso de la compilación que antes de empezar apaga nueve interruptores de servicios (el general, la analítica, los informes de rendimiento y de fallos, los diagnósticos del motor, las compras, los anuncios, las estadísticas del equipo del jugador y la API de informes de fallos) y que al terminar hace fallar la compilación si el juego compilado nombra los servidores de Unity. En el ejecutable del corte no aparece ninguno, y no se observó ninguna conexión de red (apartado 8.3); diez pruebas EditMode vigilan ese paso, y `PlayerSettingsTest` exige además apagados en el archivo todos los servicios, los diagnósticos del motor y la API de informes de fallos. El símbolo de compilación `SENTIS_ANALYTICS_ENABLED` reaparece en la configuración porque el paquete `com.unity.ai.inference` lo repone en el Editor mientras la analítica del Editor está activa; solo afecta al código de Editor de ese paquete y no llega al ejecutable (INC-97).

Fuera de `Datos/`, el reproductor de Unity escribe su registro, `Player.log` (con las líneas de RNF-04), en `%AppData%\LocalLow\Universidad Catolica de Colombia\Algoritmia\`, y sus preferencias de pantalla en la clave `HKCU\Software\Universidad Catolica de Colombia\Algoritmia` del registro de Windows. Ninguno de los dos guarda datos del estudiante. Tras borrar cuatro perfiles sobre el primer candidato, desde el panel y desde el informe docente, sus nombres no aparecen en la carpeta portable, en `LocalLow` (con `Player.log`), en `%TEMP%` ni en el registro, y sobre el ejecutable del corte los perfiles borrados desaparecen de `Datos/`. Con el ejecutable del corte, en `LocalLow` solo queda `Player.log`. En la clave del reproductor quedan los valores de pantalla, un contador y un identificador de sesión del reproductor y tres identificadores `unity_connect.*`, uno de ellos de instalación, que el motor escribe aunque sus servicios estén apagados; ninguno lleva el nombre de un perfil, y no se observó que salgan del equipo. Se documentan como residuo del motor para la entrega (DEF-RC2-01, apartado 10.4).

De los 479,0 MB del paquete, 409,7 MB son los datos del juego (`Algoritmia_Data/`), con 276,4 MB de texturas del Nivel 1; el reparto completo está en el apartado 8.1.
