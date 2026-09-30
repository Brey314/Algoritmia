using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Scaffolding.Tests
{
    /// <summary>Prueba la lógica pura de <see cref="TrailPath"/>: la estela de Algoritm (INC-52).</summary>
    public class GuideTrailTests
    {
        private GameObject _contenedor;

        [TearDown]
        public void DestruirLaJerarquia()
        {
            if (_contenedor != null)
            {
                Object.DestroyImmediate(_contenedor);
            }
        }

        [Test]
        public void GuideTrail_DA76_DejaEntreCincoYSietePuntosDeTamanoDecreciente()
        {
            var trail = new TrailPath(minDistance: 10f);

            for (var i = 0; i < 10; i++)
            {
                trail.Sample(new Vector2(i * 20f, 0f));
            }

            var puntos = trail.Points;

            Assert.That(puntos.Count, Is.InRange(TrailPath.MinPoints, TrailPath.MaxPoints));
            for (var i = 1; i < puntos.Count; i++)
            {
                Assert.That(puntos[i].Scale, Is.GreaterThan(puntos[i - 1].Scale), "cada punto más nuevo es más grande");
                Assert.That(puntos[i].Alpha, Is.GreaterThan(puntos[i - 1].Alpha), "cada punto más nuevo es más opaco");
            }
        }

        [Test]
        public void GuideTrail_DA76_SinMovimientoNoDejaEstela()
        {
            var trail = new TrailPath(minDistance: 10f);

            for (var i = 0; i < 10; i++)
            {
                trail.Sample(Vector2.zero); // el cuerpo no se mueve
            }

            Assert.That(trail.Points, Is.Empty);
        }

        [Test]
        public void TrailPath_DA76_OlvidaLaEstelaSiElCuerpoSeQuedaQuieto()
        {
            var trail = new TrailPath(minDistance: 10f);
            for (var i = 0; i < TrailPath.MinPoints; i++)
            {
                trail.Sample(new Vector2(i * 20f, 0f), time: i * 0.05f);
            }

            Assume.That(trail.Points, Is.Not.Empty, "con movimiento sostenido ya hay estela");

            trail.Sample(new Vector2(80f, 0f), time: 0.25f); // quieto, pero menos de StillSeconds
            Assert.That(trail.Points, Is.Not.Empty, "quieto menos de 0,3 s: la estela sigue");

            trail.Sample(new Vector2(80f, 0f), time: 0.60f); // más de 0,3 s sin moverse
            Assert.That(trail.Points, Is.Empty, "quieto más de 0,3 s: la estela se olvida");
        }

        [Test]
        public void GuideTrail_DA76_LaEstelaSigueALaCasillaQueCaminaYSeBorraAlDetenerse()
        {
            var (trail, casilla, contenedor) = ConstruirJerarquia();

            // La casilla camina (como WalkAsync/SetAnchor): cada parada, un cuadro.
            for (var i = 0; i <= TrailPath.MinPoints; i++)
            {
                casilla.anchorMin = casilla.anchorMax = new Vector2(0.1f * i, 0.5f);
                trail.Step(i * 0.05f);
            }

            Assert.That(trail.ActiveDotCount, Is.GreaterThanOrEqualTo(TrailPath.MinPoints),
                "la casilla caminó lo suficiente: la estela debe tener puntos");

            // Mover el contenedor (paneo/zoom de la ilustración) sin mover la casilla no cuenta
            // como que el cuerpo caminó: la muestra es invariante a eso (INC-52).
            var puntosAntes = trail.ActiveDotCount;
            contenedor.anchoredPosition += new Vector2(500f, 0f);
            trail.Step(0.30f);
            Assert.That(trail.ActiveDotCount, Is.EqualTo(puntosAntes),
                "el contenedor se movió, no la casilla: ningún punto nuevo");

            // Quieta más de StillSeconds, la estela se borra.
            trail.Step(0.30f + TrailPath.StillSeconds + 0.05f);
            Assert.That(trail.ActiveDotCount, Is.Zero, "quieta más de 0,3 s: 0 puntos activos");
        }

        /// <summary>
        /// contenedor (la ilustración) → casilla (la posición del objeto) → rig estirado (como
        /// <c>NarrativeSceneController.PlaceActor</c>) → <see cref="GuideTrail"/>, hijo del rig.
        /// </summary>
        private (GuideTrail trail, RectTransform casilla, RectTransform contenedor) ConstruirJerarquia()
        {
            _contenedor = new GameObject("Contenedor", typeof(RectTransform));
            var contenedor = (RectTransform)_contenedor.transform;
            contenedor.sizeDelta = new Vector2(1000f, 1000f);

            var casillaGo = new GameObject("Casilla", typeof(RectTransform));
            var casilla = (RectTransform)casillaGo.transform;
            casilla.SetParent(contenedor, false);
            casilla.anchorMin = casilla.anchorMax = new Vector2(0.5f, 0.5f);
            casilla.sizeDelta = new Vector2(100f, 100f);

            var rigGo = new GameObject("Actor", typeof(RectTransform));
            var rig = (RectTransform)rigGo.transform;
            rig.SetParent(casilla, false);
            rig.anchorMin = Vector2.zero;
            rig.anchorMax = Vector2.one;
            rig.offsetMin = rig.offsetMax = Vector2.zero;

            var trailGo = new GameObject("Estela", typeof(RectTransform), typeof(GuideTrail));
            trailGo.transform.SetParent(rig, false);
            var trail = trailGo.GetComponent<GuideTrail>();
            trail.InitializeForTest();

            return (trail, casilla, contenedor);
        }
    }
}
