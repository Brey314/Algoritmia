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
        public void RollingLog_INC155_ElAreaDeClicCubreElCuerpoQueSeDibujaYNoSuHuecoEnLaCaja()
        {
            var look = ScriptableObject.CreateInstance<RollingLogLook>();
            try
            {
                // Los números del tronco entregado: eje a 35°, inclinación 40°, largo 1,9 radios.
                var radio = 100f;
                var eje = new Vector2(Mathf.Cos(35f * Mathf.Deg2Rad), Mathf.Sin(35f * Mathf.Deg2Rad));
                var costado = new Vector2(-eje.y, eje.x);
                var largo = radio * 1.9f * Mathf.Sin(40f * Mathf.Deg2Rad); // del corte al fondo, en pantalla

                Assert.That(RollingLog.BodyContains(Vector2.zero, radio, look), Is.True, "el corte, el disco");
                Assert.That(RollingLog.BodyContains(eje * (largo / 2f), radio, look), Is.True, "el costado, a media longitud");
                Assert.That(RollingLog.BodyContains(eje * (largo + 60f), radio, look), Is.True, "el fondo, la otra punta");
                Assert.That(RollingLog.BodyContains(costado * (radio * 0.95f), radio, look), Is.True, "el grosor del corte");

                Assert.That(RollingLog.BodyContains(eje * (largo + 120f), radio, look), Is.False, "más allá del fondo no hay tronco");
                Assert.That(RollingLog.BodyContains(costado * (radio * 1.2f), radio, look), Is.False, "ni más grueso de lo que es");
                Assert.That(RollingLog.BodyContains(-eje * 100f, radio, look), Is.False, "ni detrás del corte");

                var caja = RollingLog.BodyBounds(radio, look);
                Assert.That(caja.Contains(eje * (largo + 60f)), Is.True, "la caja alcanza el fondo, que sale de los 120 px del objeto");
                Assert.That(RollingLog.BodyContains(new Vector2(caja.xMin + 1f, caja.yMax - 1f), radio, look), Is.False,
                    "pero las esquinas vacías de la caja no son tronco: no le roban el clic al vecino");
            }
            finally
            {
                Object.DestroyImmediate(look);
            }
        }

        [Test]
        public void RollingLog_INC155_ElClicLoRecibeElCuerpoConElObjetoGiradoEspejadoYEscalado()
        {
            var look = ScriptableObject.CreateInstance<RollingLogLook>();
            try
            {
                foreach (var espejo in new[] { false, true })
                foreach (var escala in new[] { 0.6f, 1.4f }) // FarScale y HoverScale
                {
                    var objeto = new GameObject("Tronco", typeof(RectTransform), typeof(Image));
                    try
                    {
                        var rect = (RectTransform)objeto.transform;
                        rect.sizeDelta = new Vector2(120f, 120f);
                        rect.localScale = new Vector3(espejo ? -escala : escala, escala, 1f);
                        rect.localRotation = Quaternion.Euler(0f, 0f, 45f);
                        var cara = objeto.GetComponent<Image>();

                        var sut = RollingLog.Attach(cara, look, clickable: true);

                        var contexto = $"espejo={espejo}, escala={escala}";
                        Assert.That(sut.raycastTarget, Is.True, $"{contexto}: el cuerpo recibe el clic");
                        Assert.That(cara.raycastTarget, Is.False, $"{contexto}: y no el cuadrado del objeto");

                        // Un punto sobre el fondo, que cae fuera de los 120 px del objeto.
                        var eje = new Vector2(Mathf.Cos(35f * Mathf.Deg2Rad), Mathf.Sin(35f * Mathf.Deg2Rad));
                        var largo = sut.Radius * 1.9f * Mathf.Sin(40f * Mathf.Deg2Rad);
                        var fondo = (Vector2)sut.transform.TransformPoint(eje * (largo + 20f));
                        var fuera = (Vector2)sut.transform.TransformPoint(eje * (largo + 5f * sut.Radius));
                        var caja = (RectTransform)sut.transform;

                        Assert.That(RectTransformUtility.RectangleContainsScreenPoint(caja, fondo, null), Is.False,
                            $"{contexto}: el fondo queda fuera del rectángulo del objeto");
                        Assert.That(RectTransformUtility.RectangleContainsScreenPoint(caja, fondo, null, sut.raycastPadding), Is.True,
                            $"{contexto}: el relleno lo mete en el área que mira el GraphicRaycaster");
                        Assert.That(sut.IsRaycastLocationValid(fondo, null), Is.True, $"{contexto}: y el cuerpo lo acepta");
                        Assert.That(sut.IsRaycastLocationValid(fuera, null), Is.False, $"{contexto}: lo que está más allá no");
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

        [Test]
        public void RollingLog_INC155_EnLaNarrativaElTroncoNoCaptaClics()
        {
            // Sin `clickable` el tronco es dibujo: si captara rayos se tragaría el clic que avanza
            // el diálogo (no tiene un botón padre que lo recoja).
            var look = ScriptableObject.CreateInstance<RollingLogLook>();
            var objeto = new GameObject("Tronco", typeof(RectTransform), typeof(Image));
            try
            {
                var cara = objeto.GetComponent<Image>();
                var antes = cara.raycastTarget;

                var sut = RollingLog.Attach(cara, look);

                Assert.That(sut.raycastTarget, Is.False);
                Assert.That(cara.raycastTarget, Is.EqualTo(antes), "y la imagen del objeto queda como estaba");
            }
            finally
            {
                Object.DestroyImmediate(objeto);
                Object.DestroyImmediate(look);
            }
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
