using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scaffolding
{
    /// <summary>
    /// Un encuadre de la cámara sobre la ilustración: qué punto queda en el centro de la pantalla
    /// y con cuánto acercamiento (RF-05, RNF-18).
    /// </summary>
    /// <remarks>
    /// Es contenido, no código: cada secuencia declara de dónde parte y a dónde llega su cámara,
    /// y el controlador solo interpola. Así una escena nueva trae su propio movimiento sin una
    /// rama ni una constante en el adaptador.
    /// </remarks>
    [Serializable]
    public class CameraFraming
    {
        [field: SerializeField]
        [field: Tooltip("Punto de la ilustración que queda en el centro de la pantalla, en fracciones: (0,0) abajo-izquierda, (1,1) arriba-derecha.")]
        public Vector2 Focus { get; private set; } = new Vector2(0.5f, 0.5f);

        [field: SerializeField]
        [field: Tooltip("Acercamiento sobre el encuadre que cubre la pantalla justo. 1 = cubrirla; 1.5 = ver dos tercios. Nunca baja de 1: no se descubren bordes.")]
        public float Zoom { get; private set; } = 1f;

        public CameraFraming()
        {
        }

        public CameraFraming(Vector2 focus, float zoom)
        {
            Focus = focus;
            Zoom = zoom;
        }

        public static CameraFraming Lerp(CameraFraming from, CameraFraming to, float t) =>
            new CameraFraming(Vector2.Lerp(from.Focus, to.Focus, t), Mathf.Lerp(from.Zoom, to.Zoom, t));
    }

    /// <summary>
    /// Una parada de la cámara: el encuadre al que va cuando se lee la línea y en el que se queda
    /// hasta la parada siguiente. Es lo que deja la vista quieta hasta que el texto la mueve.
    /// </summary>
    [Serializable]
    public class CameraKey
    {
        [field: SerializeField]
        [field: Tooltip("Línea, desde 0, en la que la cámara va a este encuadre.")]
        public int Line { get; private set; }

        [field: SerializeField]
        public CameraFraming Framing { get; private set; } = new CameraFraming();

        public CameraKey()
        {
        }

        public CameraKey(int line, CameraFraming framing)
        {
            Line = line;
            Framing = framing;
        }
    }

    /// <summary>
    /// La geometría de una ilustración que **cubre** la pantalla sin deformarse (RNF-03, RNF-23).
    /// </summary>
    /// <remarks>
    /// Cubrir y no encajar: la imagen se escala hasta que ningún borde de la ventana queda sin
    /// pintar, recortando lo que sobre por el otro eje. Por eso el arte puede llegar a la
    /// resolución que sea —hoy baja, mañana la definitiva— y **sustituir el archivo basta**: la
    /// escala se calcula con el tamaño real del sprite y el de la ventana en cada arranque.
    ///
    /// C# plano: se prueba en EditMode con ventanas anchas, altas y cuadradas sin abrir escena.
    /// </remarks>
    public static class IllustrationFraming
    {
        /// <summary>El tamaño con el que la imagen cubre la ventana entera, por el acercamiento.</summary>
        public static Vector2 CoverSize(Vector2 image, Vector2 viewport, float zoom)
        {
            if (image.x <= 0f || image.y <= 0f)
            {
                return viewport;
            }

            var scale = Mathf.Max(viewport.x / image.x, viewport.y / image.y) * Mathf.Max(zoom, 1f);
            return image * scale;
        }

        /// <summary>
        /// Cuánto se desplaza el centro de la imagen respecto al centro de la ventana para que
        /// <paramref name="focus"/> quede centrado, sin descubrir nunca un borde.
        /// </summary>
        public static Vector2 Offset(Vector2 size, Vector2 viewport, Vector2 focus)
        {
            var wanted = (new Vector2(0.5f, 0.5f) - focus) * size;
            var slack = Vector2.Max((size - viewport) / 2f, Vector2.zero);
            return new Vector2(
                Mathf.Clamp(wanted.x, -slack.x, slack.x),
                Mathf.Clamp(wanted.y, -slack.y, slack.y));
        }

        /// <summary>
        /// Aplica el encuadre a un elemento de interfaz centrado en su padre.
        /// </summary>
        /// <remarks>
        /// El elemento conserva el **tamaño nativo** de la imagen y se escala con
        /// <c>localScale</c>, en vez de cambiar su tamaño: así lo que cuelgue de él —los objetos
        /// que una escena narrativa pinta sobre el entorno— se coloca en fracciones de la imagen y
        /// acompaña el zoom y el paneo sin cálculo propio.
        /// </remarks>
        public static void Apply(RectTransform element, Vector2 image, CameraFraming framing)
        {
            var viewport = ((RectTransform)element.parent).rect.size;
            var size = CoverSize(image, viewport, framing.Zoom);
            var scale = image.x > 0f ? size.x / image.x : 1f;
            var center = new Vector2(0.5f, 0.5f);

            element.anchorMin = center;
            element.anchorMax = center;
            element.pivot = center;
            element.sizeDelta = image;
            element.localScale = new Vector3(scale, scale, 1f);
            element.anchoredPosition = Offset(size, viewport, framing.Focus);
        }

        /// <summary>
        /// Con qué pivote y escala hay que agrandar la pantalla entera —el «mundo»— para pasar
        /// del encuadre <paramref name="from"/> al encuadre <paramref name="to"/> sin tocar la
        /// ilustración: al terminar, el centro de la pantalla muestra el foco de destino con su
        /// acercamiento. Es lo que deja el cierre del bosque exactamente en la vista con la que
        /// abre la escena 2.2 (RF-05).
        /// </summary>
        /// <remarks>
        /// Se escala el mundo y no la ilustración porque los objetos del juego cuelgan del mundo,
        /// no de la imagen: escalar solo la imagen los dejaría atrás. Con la misma escala en los
        /// dos encuadres no hay pivote que sirva —una escala 1 no desplaza nada— y se devuelve el
        /// punto de destino sin más.
        /// </remarks>
        public static (Vector2 Pivot, float Scale) ScaleAbout(Vector2 image, Vector2 viewport, CameraFraming from, CameraFraming to)
        {
            var size = CoverSize(image, viewport, from.Zoom);
            var offset = Offset(size, viewport, from.Focus);
            var target = offset + (to.Focus - new Vector2(0.5f, 0.5f)) * size;
            var fraction = new Vector2(0.5f + target.x / viewport.x, 0.5f + target.y / viewport.y);
            var scale = Mathf.Max(to.Zoom, 1f) / Mathf.Max(from.Zoom, 1f);
            if (Mathf.Approximately(scale, 1f))
            {
                return (fraction, 1f);
            }

            var pivot = (new Vector2(0.5f, 0.5f) - fraction * scale) / (1f - scale);
            return (pivot, scale);
        }

        /// <summary>Altura de la línea de árboles del entorno del Nivel 2, en fracción de la ilustración.</summary>
        public const float TreeLine = 0.66f;

        /// <summary>Eje del espejo del entorno del Nivel 2: el original ocupa la mitad izquierda y su copia la derecha.</summary>
        public const float MirrorAxis = 0.5f;

        /// <summary>
        /// Lo que un encuadre no cumple, en palabras para el Inspector. Vacío si está bien.
        /// </summary>
        /// <remarks>
        /// Los focos fuera de <c>[0.25/z, 1−0.25/z] × [0.5/z, 1−0.5/z]</c> se recortan en silencio
        /// al aplicar —nunca se descubre un borde—, así que un valor escrito fuera no se ve como
        /// está escrito y nadie se entera: esto es el aviso que faltaba. Las dos reglas del
        /// entorno espejado (<paramref name="mirroredForest"/>) son de ese entorno y no del
        /// motor: el claro está vacío, sin troncos en cuadro un paneo parece una imagen congelada,
        /// y un plano que cruce el eje enseña un tronco y su gemelo a la vez.
        /// </remarks>
        // Punto decimal siempre, sea cual sea la configuración regional: las pruebas y el documento los leen así.
        public static IEnumerable<string> Warnings(CameraFraming framing, CameraFraming previous, bool mirroredForest)
        {
            var zoom = Mathf.Max(framing.Zoom, 1f);
            var halfWidth = 0.25f / zoom;
            var halfHeight = 0.5f / zoom;
            var focus = framing.Focus;

            if (focus.x < halfWidth || focus.x > 1f - halfWidth)
            {
                yield return FormattableString.Invariant($"x={focus.x:0.000} se recorta a [{halfWidth:0.000}, {1f - halfWidth:0.000}] a zoom {zoom:0.00}");
            }

            if (focus.y < halfHeight || focus.y > 1f - halfHeight)
            {
                yield return FormattableString.Invariant($"y={focus.y:0.000} se recorta a [{halfHeight:0.000}, {1f - halfHeight:0.000}] a zoom {zoom:0.00}");
            }

            if (!mirroredForest)
            {
                yield break;
            }

            if (focus.y + halfHeight < TreeLine)
            {
                yield return FormattableString.Invariant($"la línea de árboles queda fuera: y + 0.5/zoom = {focus.y + halfHeight:0.000} < {TreeLine}");
            }

            if (focus.x - halfWidth < MirrorAxis && focus.x + halfWidth > MirrorAxis)
            {
                yield return FormattableString.Invariant($"el cuadro cruza el eje del espejo x={MirrorAxis}: [{focus.x - halfWidth:0.000}, {focus.x + halfWidth:0.000}]");
            }
            else if (previous != null && (previous.Focus.x < MirrorAxis) != (focus.x < MirrorAxis))
            {
                yield return FormattableString.Invariant($"el paneo desde x={previous.Focus.x:0.000} hasta x={focus.x:0.000} cruza el eje del espejo");
            }
        }
    }
}
