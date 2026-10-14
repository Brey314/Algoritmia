using System;
using System.Linq;
using System.Threading.Tasks;
using Game.Core;
using Game.Reporting;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.UI.Tests
{
    /// <summary>
    /// RF-46: consulta de progreso del docente. La agregación y la enumeración de perfiles ya
    /// están probadas en EditMode contra un <see cref="IFileSystem"/> en memoria
    /// (<c>ProfileRepositoryTests</c>, <c>IndicatorReportTests</c>); aquí solo se prueba el
    /// cableado — que la pantalla llega, navega y se rellena con lo que el repositorio devuelve.
    /// </summary>
    [Category("Integration")]
    public class TeacherReportTests
    {
        private const string SceneName = "TeacherReport";

        [TearDown]
        public void DestruirLosObjetosPersistentes()
        {
            foreach (var runner in Object.FindObjectsByType<GameFlowRunner>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(runner.gameObject);
            }

            foreach (var loader in Object.FindObjectsByType<SceneLoader>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(loader.gameObject);
            }
        }

        [Test]
        [Timeout(20000)]
        public async Task TeacherReport_RF46_SeAlcanzaDesdeElMenuYVuelveSinPerderElPerfilActivo()
        {
            var profile = PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;
            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync(); // GameFlowRunner.Start() navega solo a MainMenu

            runner.GoTo(GameState.ProfileSelect);
            runner.SelectProfile(profile);
            runner.GoTo(GameState.MainMenu); // el estudiante vuelve al inicio con perfil activo
            Assert.That(runner.Flow.ActiveProfile, Is.SameAs(profile));

            Assert.That(runner.GoTo(GameState.TeacherReport), Is.True);
            await LoadSceneOf(runner);

            var controller = await WaitForComponentAsync<TeacherReportController>(10f);
            Assert.That(controller, Is.Not.Null, "no apareció TeacherReportController");
            Assert.That(runner.Flow.ActiveProfile, Is.SameAs(profile), "consultar no puede tocar el perfil activo");

            Click(controller.BackButton);
            Assert.That(await WaitUntilAsync(() => runner.Flow.Current == GameState.MainMenu, 5f), Is.True,
                "«Volver al menú» no volvió a MainMenu");
            Assert.That(runner.Flow.ActiveProfile, Is.SameAs(profile), "y el perfil activo sigue siendo el mismo");
        }

        [Test]
        [Timeout(20000)]
        public async Task TeacherReport_CU11_SinPerfilesInformaYOfreceVolver()
        {
            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync();
            runner.GoTo(GameState.TeacherReport);
            await LoadSceneOf(runner);

            var controller = await WaitForComponentAsync<TeacherReportController>(10f);
            Assert.That(controller, Is.Not.Null);

            // Repositorio vacío de verdad, no el disco real de la máquina de desarrollo.
            controller.Repository = new ProfileRepository(new FakeFileSystem(), "C:/Vacio/Datos", "C:/Vacio/Respaldo");
            controller.Show();

            Assert.That(controller.EmptyStatePanel.activeInHierarchy, Is.True, "CU-11 FA-2a: informa que no hay perfiles");
            Assert.That(controller.DataPanel.activeInHierarchy, Is.False);

            Click(controller.BackButton);
            Assert.That(await WaitUntilAsync(() => runner.Flow.Current == GameState.MainMenu, 5f), Is.True,
                "sin perfiles, «Volver al menú» sigue funcionando");
        }

        [Test]
        [Timeout(20000)]
        public async Task TeacherReport_RF46_PresentaLosCuatroIndicadoresPorNivelYPorFase()
        {
            var profile = PlayerProfile.Create("Beto", Array.Empty<string>()).Profile;
            profile.ConfirmPhase(new PhaseId(LevelId.Fire, 1), new PerformanceIndicators(4, 2, 3, 91f));

            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync();
            runner.GoTo(GameState.TeacherReport);
            await LoadSceneOf(runner);

            var controller = await WaitForComponentAsync<TeacherReportController>(10f);
            var fileSystem = new FakeFileSystem();
            fileSystem.Write("C:/Vacio/Datos/Beto.json", JsonUtility.ToJson(profile));
            controller.Repository = new ProfileRepository(fileSystem, "C:/Vacio/Datos", "C:/Vacio/Respaldo");
            controller.Show();

            Assert.That(controller.DataPanel.activeInHierarchy, Is.True);
            var tableView = Object.FindAnyObjectByType<IndicatorTableView>();
            // Nivel 1 (una fase) + Nivel 2 (tres, sin datos) + Nivel 3 (tres, sin datos) = siete filas.
            Assert.That(tableView.Rows, Has.Count.EqualTo(7));

            var primeraFila = tableView.Rows[0];
            Assert.That(TextOf(primeraFila, "Nivel"), Is.EqualTo("Nivel 1"));
            Assert.That(TextOf(primeraFila, "Fase"), Is.EqualTo("Fase 1"));
            Assert.That(TextOf(primeraFila, "Intentos"), Is.EqualTo("4"));
            Assert.That(TextOf(primeraFila, "ErroresCorregidos"), Is.EqualTo("2"));
            Assert.That(TextOf(primeraFila, "PasosUtilizados"), Is.EqualTo("3"));
            Assert.That(TextOf(primeraFila, "Tiempo"), Is.EqualTo("1:31"));

            var segundaFila = tableView.Rows[1];
            Assert.That(TextOf(segundaFila, "Nivel"), Is.EqualTo("Nivel 2"));
            Assert.That(TextOf(segundaFila, "Intentos"), Is.EqualTo("Sin datos"),
                "RF-46: un nivel no jugado se presenta sin datos, no con ceros");
        }

        // OBS-SDOC-2: el detalle no decía de quién eran las cifras. Rojo esperado sin la corrección:
        // SelectedProfileLabel no existe (no compila) o queda vacío.
        [Test]
        [Timeout(20000)]
        public async Task TeacherReport_RF46_ElDetalleNombraElPerfilQueSeEstaMirando()
        {
            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync();
            runner.GoTo(GameState.TeacherReport);
            await LoadSceneOf(runner);

            var controller = await WaitForComponentAsync<TeacherReportController>(10f);
            var fileSystem = new FakeFileSystem();
            foreach (var nombre in new[] { "Ana", "Beto" })
            {
                fileSystem.Write($"C:/Vacio/Datos/{nombre}.json",
                    JsonUtility.ToJson(PlayerProfile.Create(nombre, Array.Empty<string>()).Profile));
            }

            controller.Repository = new ProfileRepository(fileSystem, "C:/Vacio/Datos", "C:/Vacio/Respaldo");
            controller.Show();
            await Awaitable.NextFrameAsync();

            Assert.That(controller.SelectedProfileLabel.text, Does.Contain(controller.Selected.Name), "nombra el perfil elegido por defecto");

            var beto = controller.Entries.First(entrada => entrada.name == "Perfil(Beto)");
            Click(beto);

            Assert.That(controller.Selected.Name, Is.EqualTo("Beto"));
            Assert.That(controller.SelectedProfileLabel.text, Does.Contain("Beto"), "y cambia al elegir otro");
            Assert.That(controller.SelectedProfileLabel.text, Does.Not.Contain("Ana"));
            Assert.That(controller.SelectedProfileLabel.gameObject.activeInHierarchy, Is.True);
        }

        [Test]
        [Timeout(20000)]
        public async Task TeacherReport_INC34_AdvierteAlDocenteCuandoElGuardadoUsaLaRutaDeRespaldo()
        {
            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync();
            runner.GoTo(GameState.TeacherReport);
            await LoadSceneOf(runner);

            var controller = await WaitForComponentAsync<TeacherReportController>(10f);
            controller.Repository = new ProfileRepository(new FakeFileSystem(), "C:/Vacio/Datos", "C:/Vacio/Respaldo");
            controller.UsingFallbackStorage = () => true;
            controller.Show();

            Assert.That(controller.FallbackNoticeLabel.gameObject.activeSelf, Is.True,
                "no avisó de que el guardado está usando la ruta de respaldo");
            Assert.That(controller.FallbackNoticeLabel.text, Is.Not.Empty);
        }

        [Test]
        [Timeout(20000)]
        public async Task TeacherReport_INC34_NoMuestraElAvisoConDatosEscribible()
        {
            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync();
            runner.GoTo(GameState.TeacherReport);
            await LoadSceneOf(runner);

            var controller = await WaitForComponentAsync<TeacherReportController>(10f);
            controller.Repository = new ProfileRepository(new FakeFileSystem(), "C:/Vacio/Datos", "C:/Vacio/Respaldo");
            controller.UsingFallbackStorage = () => false;
            controller.Show();

            Assert.That(controller.FallbackNoticeLabel.gameObject.activeSelf, Is.False,
                "avisó de una ruta de respaldo que no existe");
        }

        [Test]
        [Timeout(20000)]
        public async Task TeacherReport_RF46_ConsultarNoAlteraNingunPerfil()
        {
            var profile = PlayerProfile.Create("Caro", Array.Empty<string>()).Profile;
            profile.ConfirmPhase(new PhaseId(LevelId.Fire, 1), new PerformanceIndicators(1, 0, 2, 10f));
            var fileSystem = new FakeFileSystem();
            var originalJson = JsonUtility.ToJson(profile);
            fileSystem.Write("C:/Vacio/Datos/Caro.json", originalJson);

            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync();
            runner.GoTo(GameState.TeacherReport);
            await LoadSceneOf(runner);

            var controller = await WaitForComponentAsync<TeacherReportController>(10f);
            controller.Repository = new ProfileRepository(fileSystem, "C:/Vacio/Datos", "C:/Vacio/Respaldo");
            controller.Show();

            Assert.That(fileSystem.Read("C:/Vacio/Datos/Caro.json"), Is.EqualTo(originalJson),
                "consultar el progreso es solo lectura: ningún archivo cambia");
        }

        // DEF-RC3-05: los iconos de «Errores corregidos» y «Tiempo de resolución» cruzaban la primera línea de su
        // cabecera de dos líneas y los de «Intentos» y «Pasos utilizados», de una línea, quedaban más lejos y
        // pequeños. Rojo esperado sin la corrección: el icono se mete en la celda del texto y la distancia
        // de la cabecera a la primera línea cambia según las líneas que tenga.
        [Test]
        [Timeout(20000)]
        public async Task TeacherReport_RF46_DEFRC305_LosCuatroIconosQuedanSobreSuCabeceraALaMismaDistancia()
        {
            await AbrirConUnPerfil();

            var cabeceras = Object.FindObjectsByType<Text>(FindObjectsInactive.Exclude)
                .Where(texto => texto.name.StartsWith("Header_", StringComparison.Ordinal))
                .ToArray();
            Assume.That(cabeceras, Has.Length.EqualTo(4), "Intentos, Errores corregidos, Pasos utilizados y Tiempo de resolución");

            var celdas = cabeceras.Select(texto => EnPantalla(texto.rectTransform)).ToArray();
            var iconos = cabeceras.Select(texto => EnPantalla(texto.transform.Find("Icono").GetComponent<Image>().rectTransform)).ToArray();
            var distancias = celdas.Select((celda, i) => celda.yMax - iconos[i].yMin).ToArray();

            Assert.That(iconos.Select((icono, i) => icono.yMin - celdas[i].yMax), Has.All.GreaterThanOrEqualTo(-0.5f),
                "el icono queda entero sobre la celda del texto: no cruza ninguna línea");
            Assert.That(distancias, Has.All.EqualTo(distancias[0]).Within(0.5f),
                "los cuatro iconos quedan a la misma distancia de su cabecera");
            Assert.That(cabeceras.Select(texto => texto.alignment), Has.All.EqualTo(TextAnchor.UpperCenter),
                "la primera línea arranca arriba de la celda, tenga una o dos líneas");
            Assert.That(cabeceras.Select(texto => texto.preferredHeight <= texto.rectTransform.rect.height), Has.All.True,
                "y el texto no se sale de su celda hacia el icono");
            Assert.That(iconos.Select(icono => icono.height / Screen.height), Has.All.GreaterThanOrEqualTo(0.04f),
                "los iconos se leen: ni «muy pequeños» ni sueltos");
        }

        [Test]
        [Timeout(20000)]
        public async Task TeacherReport_RF46_DEFRC305_LaCabeceraNoSeMontaSobreLaTablaNiSobreElNombreDelPerfil()
        {
            await AbrirConUnPerfil();

            var cabecera = Object.FindObjectsByType<Text>(FindObjectsInactive.Exclude)
                .Single(texto => texto.name == "HeaderNivel").transform.parent;
            var fila = EnPantalla((RectTransform)cabecera);
            var iconos = cabecera.GetComponentsInChildren<Image>(true).Select(imagen => EnPantalla(imagen.rectTransform)).ToArray();
            var tabla = EnPantalla((RectTransform)Object.FindObjectsByType<ScrollRect>(FindObjectsInactive.Exclude)
                .Single(scroll => scroll.name == "TableScroll").transform);
            var perfil = EnPantalla(Object.FindAnyObjectByType<TeacherReportController>().SelectedProfileLabel.rectTransform);

            Assert.That(fila.yMin, Is.GreaterThanOrEqualTo(tabla.yMax - 0.5f), "la tabla empieza bajo la cabecera");
            Assert.That(iconos.Select(icono => icono.Overlaps(perfil)), Has.All.False,
                "ningún icono pisa el nombre del perfil");
        }

        // DEF-RC3-05: «Volver al menú» partía su rótulo en dos líneas dentro de un botón de 200 px.
        [Test]
        [Timeout(20000)]
        public async Task TeacherReport_RF46_DEFRC305_VolverAlMenuCabeEnUnaLineaDentroDeSuBoton()
        {
            var controller = await AbrirConUnPerfil();

            var rotulo = controller.BackButton.GetComponentInChildren<Text>();

            Assert.That(rotulo.preferredWidth, Is.LessThanOrEqualTo(rotulo.rectTransform.rect.width), "una sola línea");
        }

        // --- helpers -----------------------------------------------------------------------

        private static async Task<TeacherReportController> AbrirConUnPerfil()
        {
            var profile = PlayerProfile.Create("Beto", Array.Empty<string>()).Profile;
            profile.ConfirmPhase(new PhaseId(LevelId.Fire, 1), new PerformanceIndicators(4, 2, 3, 91f));

            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync();
            runner.GoTo(GameState.TeacherReport);
            await LoadSceneOf(runner);

            var controller = await WaitForComponentAsync<TeacherReportController>(10f);
            var fileSystem = new FakeFileSystem();
            fileSystem.Write("C:/Vacio/Datos/Beto.json", JsonUtility.ToJson(profile));
            controller.Repository = new ProfileRepository(fileSystem, "C:/Vacio/Datos", "C:/Vacio/Respaldo");
            controller.Show();

            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            return controller;
        }

        private static Rect EnPantalla(RectTransform rect)
        {
            var esquinas = new Vector3[4];
            rect.GetWorldCorners(esquinas);
            return new Rect(esquinas[0].x, esquinas[0].y, esquinas[2].x - esquinas[0].x, esquinas[2].y - esquinas[0].y);
        }

        private static async Task LoadSceneOf(GameFlowRunner runner)
        {
            var load = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            while (load is { isDone: false })
            {
                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();
            new GameObject("TestSceneLoader").AddComponent<SceneLoader>();
        }

        private static string TextOf(GameObject row, string childName) =>
            row.transform.Find(childName).GetComponent<Text>().text;

        private static void Click(Button button) =>
            ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);

        private static async Task<bool> WaitUntilAsync(Func<bool> condition, float timeoutSeconds)
        {
            var deadline = Time.realtimeSinceStartup + timeoutSeconds;
            while (!condition())
            {
                if (Time.realtimeSinceStartup >= deadline)
                {
                    return false;
                }

                await Awaitable.NextFrameAsync();
            }

            return true;
        }

        private static async Task<T> WaitForComponentAsync<T>(float timeoutSeconds) where T : Object
        {
            var deadline = Time.realtimeSinceStartup + timeoutSeconds;
            T found;
            while ((found = Object.FindAnyObjectByType<T>(FindObjectsInactive.Include)) == null)
            {
                if (Time.realtimeSinceStartup >= deadline)
                {
                    return null;
                }

                await Awaitable.NextFrameAsync();
            }

            return found;
        }

        /// <summary>
        /// Sistema de archivos en memoria, local a esta suite: PlayMode no puede referenciar el
        /// assembly de pruebas EditMode donde vive el doble de <c>SaveStoreTests</c>.
        /// </summary>
        private sealed class FakeFileSystem : IFileSystem
        {
            private readonly System.Collections.Generic.Dictionary<string, string> _files = new();

            public void Write(string path, string contents) => _files[path] = contents;

            public string Read(string path) => _files.TryGetValue(path, out var contents) ? contents : null;

            public bool TryPrepareDirectory(string directory) => true;

            public void WriteAllText(string path, string contents) => _files[path] = contents;

            public string ReadAllText(string path) => _files[path];

            public bool FileExists(string path) => _files.ContainsKey(path);

            public bool DeleteFile(string path)
            {
                _files.Remove(path);
                return true;
            }

            public string[] GetFiles(string directory, string extension) => _files.Keys
                .Where(path => path.StartsWith(directory + "/") && path.EndsWith(extension))
                .ToArray();
        }
    }
}
