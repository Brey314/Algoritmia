using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.UI.Tests
{
    /// <summary>
    /// El hundido del botón al presionarlo, según el mockup de interfaz y §10.2 de la dirección
    /// de arte: la cara baja cuatro píxeles y la sombra plana pasa de seis a dos.
    /// </summary>
    /// <remarks>
    /// EditMode y no PlayMode a propósito: el componente solo mueve un <c>RectTransform</c> y no
    /// depende de fotogramas ni de escena. Por eso tampoco captura la posición de reposo en
    /// <c>Awake</c> —que en EditMode no se invoca— sino que aplica y revierte el mismo desfase.
    /// </remarks>
    public class ButtonPressFeedbackTests
    {
        private GameObject root;

        [TearDown]
        public void DestruirElBoton()
        {
            if (root != null)
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void ButtonPressFeedback_RNF02_HundeLaCaraMientrasElClicSeSostiene()
        {
            var (sut, face) = UnBoton();
            var reposo = face.anchoredPosition;

            sut.OnPointerDown(UnClic());

            Assert.That(face.anchoredPosition, Is.EqualTo(reposo - new Vector2(0f, 4f)));
        }

        [Test]
        public void ButtonPressFeedback_RNF02_DevuelveLaCaraASuSitioAlSoltar()
        {
            var (sut, face) = UnBoton();
            var reposo = face.anchoredPosition;

            sut.OnPointerDown(UnClic());
            sut.OnPointerUp(UnClic());

            Assert.That(face.anchoredPosition, Is.EqualTo(reposo));
        }

        [Test]
        public void ButtonPressFeedback_RNF02_DevuelveLaCaraASuSitioSiElPunteroSaleSinSoltar()
        {
            var (sut, face) = UnBoton();
            var reposo = face.anchoredPosition;

            sut.OnPointerDown(UnClic());
            sut.OnPointerExit(UnClic());

            Assert.That(face.anchoredPosition, Is.EqualTo(reposo));
        }

        /// <summary>
        /// Un clic sostenido (RNF-02) repite <c>OnPointerDown</c> sin soltar: el desfase se aplica
        /// una sola vez o la cara se hundiría sin fondo.
        /// </summary>
        [Test]
        public void ButtonPressFeedback_RNF02_NoAcumulaElHundidoSiElClicSeSostiene()
        {
            var (sut, face) = UnBoton();
            var reposo = face.anchoredPosition;

            sut.OnPointerDown(UnClic());
            sut.OnPointerDown(UnClic());
            sut.OnPointerUp(UnClic());

            Assert.That(face.anchoredPosition, Is.EqualTo(reposo));
        }

        /// <summary>
        /// El nivel bloqueado del menú de niveles es un botón no interactivo: no se hunde, porque
        /// hundirse es la promesa de que el clic hizo algo.
        /// </summary>
        [Test]
        public void ButtonPressFeedback_RF03_NoHundeUnBotonNoInteractivo()
        {
            var (sut, face) = UnBoton();
            root.GetComponent<Button>().interactable = false;
            var reposo = face.anchoredPosition;

            sut.OnPointerDown(UnClic());

            Assert.That(face.anchoredPosition, Is.EqualTo(reposo));
        }

        /// <summary>
        /// La cara es el primer hijo por convención de la anatomía del botón (raíz = contorno y
        /// sombra, hijo = cara). Sin serializarla, el componente la encuentra igual.
        /// </summary>
        [Test]
        public void ButtonPressFeedback_RNF02_TomaElPrimerHijoComoCaraSiNoSeSerializaNinguna()
        {
            var (sut, face) = UnBoton();

            sut.OnPointerDown(UnClic());

            Assert.That(face.anchoredPosition.y, Is.EqualTo(-1f));
        }

        /// <summary>
        /// Botón del mockup: raíz con la <c>Image</c> de contorno y sombra, y un hijo «Fondo» que
        /// es la cara, desplazada tres píxeles hacia arriba para dejar los seis de sombra.
        /// </summary>
        private (ButtonPressFeedback sut, RectTransform face) UnBoton()
        {
            root = new GameObject("PlayButton", typeof(RectTransform), typeof(Image), typeof(Button));
            var face = new GameObject("Fondo", typeof(RectTransform), typeof(Image))
                .GetComponent<RectTransform>();
            face.SetParent(root.transform, false);
            face.anchoredPosition = new Vector2(0f, 3f);

            return (root.AddComponent<ButtonPressFeedback>(), face);
        }

        private static PointerEventData UnClic() => new PointerEventData(EventSystem.current);
    }
}
