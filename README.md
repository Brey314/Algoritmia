# Algoritmia

*Piensa el orden, enciende el fuego.*

Videojuego educativo 2D para estudiantes de grado quinto de primaria. Con él practican el pensamiento computacional: descomposición, abstracción, reconocimiento de patrones, pensamiento algorítmico, depuración e iteración. Es el prototipo del trabajo de grado *Desarrollo de un prototipo de videojuego educativo como herramienta tecnológica didáctica para incentivar el pensamiento computacional en estudiantes de grado quinto de primaria del Colegio El Libertador IED* (Universidad Católica de Colombia, Programa de Ingeniería de Sistemas y Computación, 2026).

Una familia prehistórica resuelve tres problemas de supervivencia. Lo que descubre en un nivel crea el problema del siguiente.

| Nivel | Quién juega | Reto | Facetas |
|---|---|---|---|
| 1. La Oscuridad | Papá | Reunir hojas y piedras y encender el fuego ajustando la fuerza del golpe y la cercanía de las piedras | Iteración y depuración |
| 2. La Rueda | Niña | Elegir lo que rueda, armar una carretilla en orden y programar con bloques la ruta al refugio | Abstracción, patrones y pensamiento algorítmico |
| 3. El Río | Mamá | Reunir materiales, ensamblar una balsa por fases y encontrar la pieza que falló | Descomposición y depuración |

Algoritm, el guía, acompaña los tres niveles. Pregunta y descompone el problema, pero nunca da la respuesta.

## Principios pedagógicos

Son reglas del diseño, y cada una tiene pruebas automáticas que la verifican:

- No hay pantalla de derrota ni límite de intentos, y equivocarse no tiene penalización.
- El estudiante no ve puntajes ni cifras. Los indicadores de desempeño solo aparecen en el informe docente.
- Lo que ya se aprobó no se pierde por un fallo posterior.
- La entrada es solo clic y clic sostenido. Los controles de dirección del Nivel 3 son botones en pantalla.

## Cómo se entrega

Es un ejecutable portable para Windows de 64 bits: no se instala, funciona sin internet y no envía datos por red. El progreso se guarda en archivos JSON dentro de una carpeta `Datos/` que el juego crea junto al ejecutable. Desde el propio juego se puede borrar un perfil de forma definitiva.

| Presupuesto | Límite |
|---|---|
| Carga de una escena | menos de 10 s |
| Memoria en ejecución | menos de 2 GB |
| Tamaño del paquete | menos de 500 MB |

## Requisitos para desarrollar

- **Unity 6000.5.10f1**, con la plantilla 2D, URP 17.6.0, Input System 1.20.0 y Unity Test Framework 1.7.0.
- **Git LFS**: las imágenes (`*.png`) y los sonidos (`*.wav`) se guardan con LFS.

```bash
git lfs install
git clone https://github.com/Brey314/Algoritmia.git
```

Abre la carpeta con Unity Hub. Al pulsar Play, el juego arranca siempre desde la escena `Boot`.

## Estructura

```
Assets/
  Game/
    Scripts/   código (Runtime, Levels, Editor)
    Scenes/    12 escenas: Boot, MainMenu, LevelSelect, Narrative, LevelSummary,
               TeacherReport, Credits y las cinco jugables de los tres niveles
    Data/      contenido: textos, secuencias narrativas y parámetros (ScriptableObjects)
    Art/       sprites, ilustraciones y animaciones
    Audio/     música, ambientes y efectos
    Prefabs/   interfaz y personajes
  Tests/
    EditMode/  pruebas de lógica, arquitectura y contenido
    PlayMode/  pruebas de escenas e integración
claudeDocs/    especificación, dirección de arte y de sonido, planes y resultados
docs/          documentos del trabajo de grado y actas
```

### Arquitectura

- **Módulos.** Hay un assembly (`.asmdef`) por módulo, y las dependencias van en un solo sentido: `Game.Core` → `Game.Scaffolding` → `Game.Levels.{Fire,Wheel,River}`. Ningún nivel referencia a otro.
- **Flujo del juego.** Lo dirige una máquina de estados en C# plano, `GameFlow`, que se prueba sin cargar escenas. Los MonoBehaviour son adaptadores delgados.
- **Contenido fuera del código.** Los textos visibles y los parámetros ajustables viven en ScriptableObjects. Para añadir una escena narrativa basta con crear un asset; no hace falta tocar código.
- **Módulos aparte.** `Game.Reporting` (el informe docente) depende solo de `Game.Core`. `Game.UI` contiene los controladores de pantalla y `Game.Audio` el gestor de sonido.

La especificación completa está en [`claudeDocs/SPEC.md`](claudeDocs/SPEC.md).

## Pruebas

El nombre de cada prueba incluye el requisito que verifica, con la forma `<Sujeto>_<Requisito>_<QuéHace>`, por ejemplo `GameFlow_RNF13_RecorreElGoldenPathCompletoSinEstadoIrrecuperable`. Así se traza cada requisito funcional hasta sus pruebas.

Desde el Editor se corren en **Window → General → Test Runner**. Sin abrir el Editor, por línea de comandos:

```bash
Unity -batchmode -projectPath . -runTests -testPlatform EditMode -testResults test-results.xml
Unity -batchmode -projectPath . -runTests -testPlatform PlayMode -testResults play-results.xml
```

Las pruebas PlayMode de disposición de pantalla esperan una vista de juego de 1920×1080.

## Compilar el ejecutable

En Unity, ve a **File → Build Profiles**, elige Windows (64 bits) y compila en `Build/Algoritmia/`. Esa carpeta es el entregable completo. Junto al `.exe` el build copia la carpeta `Licencias/` con los avisos de las fuentes tipográficas y de los íconos. La carpeta `Build/` no se versiona.

## Autores

- Santiago Benavides Rey
- Santiago Valdiri García

Director: Juan Carlos Navarro Beltrán, M.Sc., Universidad Católica de Colombia.

## Créditos y licencias

- Los personajes se basan en la **Familia Anonaky**, de Bibiana Patricia Rey Barrote y Luis Eduardo Benavides Porras (*Tecnología para niños: libro de actividades*, 2019). Se usan con autorización de sus autores, solo con fines académicos y no comerciales.
- Las fuentes tipográficas se distribuyen con la SIL Open Font License y los íconos de [Phosphor](https://phosphoricons.com) con la licencia MIT. Sus avisos van en `Licencias/`.
- El código, el arte y el sonido de este repositorio no tienen una licencia abierta. Si quieres reutilizarlos, consulta antes a los autores.
