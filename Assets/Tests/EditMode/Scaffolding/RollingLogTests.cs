using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Utils;
using UnityEngine.UI;

namespace Game.Scaffolding.Tests
{
    /// <summary>El cilindro del tronco que rueda en el bosque y en la 2.2 (RF-26).</summary>
    public class RollingLogTests
    {
        private const float Radio = 10f;
        private const float Largo = 20f;

        [Test]
        public void RollingLog_RF26_DeFrenteSoloSeVeElCorteYDeCostadoSoloLaCorteza()
        {
            foreach (var borde in new[] { 0f, 60f, 135f, 270f })
            {
                var cerca = RollingLog.Project(borde, 0f, Radio, 0f);
                var lejos = RollingLog.Project(borde, Largo, Radio, 0f);
                var circulo = new Vector2(Mathf.Cos(borde * Mathf.Deg2Rad), Mathf.Sin(borde * Mathf.Deg2Rad)) * Radio;
                Assert.That(Vector2.Distance(cerca, circulo), Is.LessThan(1e-4f), $"de frente el borde a {borde}° es el círculo del corte");
                Assert.That(Vector2.Distance(lejos, cerca), Is.LessThan(1e-4f), "y el costado queda escondido detrás");

                var deCostado = RollingLog.Project(borde, Largo, Radio, 90f);
                Assert.That(deCostado.x, Is.EqualTo(Largo).Within(1e-4f), "de costado el largo se ve entero");
                Assert.That(RollingLog.Project(borde, 0f, Radio, 90f).x, Is.EqualTo(0f).Within(1e-4f),
                    "y el corte se vuelve una línea: no se ve");
            }
        }

        [Test]
        public void RollingLog_RF26_En34ElCorteSeAchataYElCostadoSeAlejaPorElEje()
        {
            var inclinacion = 40f;
            var coseno = Mathf.Cos(inclinacion * Mathf.Deg2Rad);

            Assert.That(RollingLog.Project(0f, 0f, Radio, inclinacion).x, Is.EqualTo(Radio * coseno).Within(1e-4f),
                "el corte se achata en la dirección del eje");
            Assert.That(RollingLog.Project(90f, 0f, Radio, inclinacion).y, Is.EqualTo(Radio).Within(1e-4f),
                "y conserva el alto de través");

            var cerca = RollingLog.Project(30f, 0f, Radio, inclinacion);
            var lejos = RollingLog.Project(30f, Largo, Radio, inclinacion);
            Assert.That(lejos.y, Is.EqualTo(cerca.y).Within(1e-4f), "una línea del costado va paralela al eje");
            Assert.That(lejos.x - cerca.x, Is.EqualTo(Largo * Mathf.Sin(inclinacion * Mathf.Deg2Rad)).Within(1e-4f),
                "y se ve tan larga como el tronco inclinado");
        }

        [Test]
        public void RollingLog_RF26_EnEspejoLaTexturaGiraComoSeVeGirarElObjeto()
        {
            var look = ScriptableObject.CreateInstance<RollingLogLook>();
            try
            {
                foreach (var espejo in new[] { false, true })
                {
                    var objeto = new GameObject("Tronco", typeof(RectTransform), typeof(Image));
                    try
                    {
                        var rect = (RectTransform)objeto.transform;
                        rect.localScale = new Vector3(espejo ? -1f : 1f, 1f, 1f);
                        rect.localRotation = Quaternion.Euler(0f, 0f, -30f); // horario: rueda hacia la derecha

                        var sut = RollingLog.Attach(objeto.GetComponent<Image>(), look);

                        Assert.That(Mathf.DeltaAngle(0f, sut.Spin), Is.EqualTo(-30f).Within(1e-3f),
                            $"espejo={espejo}: la textura gira hacia donde se ve girar el objeto, o rueda al revés de su avance");
                        Assert.That(sut.transform.TransformVector(Vector3.right),
                            Is.EqualTo(Vector3.right).Using(Vector3EqualityComparer.Instance),
                            $"espejo={espejo}: el cilindro no gira ni se refleja con el objeto");
                        Assert.That(sut.transform.TransformVector(Vector3.up),
                            Is.EqualTo(Vector3.up).Using(Vector3EqualityComparer.Instance));
                    }
                    finally
                    {
                        Object.DestroyImmediate(objeto);
                    }
                }
            }
            finally
            {
                Object.DestroyImmediate(look);
            }
        }
    }
}
