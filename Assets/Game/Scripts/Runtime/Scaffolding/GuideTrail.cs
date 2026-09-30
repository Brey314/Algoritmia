using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scaffolding
{
    /// <summary>
    /// La estela de puntos luminosos que Algoritm deja al moverse (guion §1.1.1, Dirección de arte
    /// §7.6, INC-52): en <see cref="LateUpdate"/> muestrea la posición de su rig y coloca de 5 a 7
    /// <see cref="Image"/> hijas con escala y alfa decrecientes hacia atrás. Ninguna recibe clics
    /// (RNF-02); se apaga solo con el alfa, sin parpadeo (RNF-21), y sin cifras.
    /// </summary>
    /// <remarks>
    /// Cuelga como hijo del rig (hermano de <c>Lienzo</c>) y muestrea la posición de su propio
    /// padre: el <c>RectTransform</c> que mueve la escena o la mecánica. <c>Lienzo</c> y
    /// <c>Cuerpo</c> son estáticos dentro de él y no sirven para detectar el movimiento.
    /// </remarks>
    [RequireComponent(typeof(RectTransform))]
    public class GuideTrail : MonoBehaviour
    {
        private static readonly Color DotColor = new Color32(0xFF, 0xE9, 0xA8, 0xFF);
        private const float DotSize = 28f;

        [SerializeField]
        [Tooltip("Un punto circular (ui_circulo): cada punto de la estela es una Image con este sprite.")]
        private Sprite dotSprite;

        [field: SerializeField]
        [field: Tooltip("Distancia mínima, en unidades del lienzo, entre dos puntos de la estela.")]
        public float MinDistance { get; set; } = 24f;

        private TrailPath _path;
        private RectTransform _body;
        private readonly List<Image> _dots = new List<Image>();

        private void Awake()
        {
            _path = new TrailPath(MinDistance);
            _body = transform.parent as RectTransform;
        }

        private void LateUpdate() => Step(Time.unscaledTime);

        /// <summary>
        /// Muestrea y dibuja la estela. Cuerpo de <see cref="LateUpdate"/> aparte para poder
        /// probarlo sin Play Mode.
        /// </summary>
        /// <remarks>
        /// **Por qué no <c>_body.anchoredPosition</c> (como antes, INC-52):** en la narrativa el
        /// rig se estira sobre su casilla (<c>NarrativeSceneController.PlaceActor</c>) y lo que
        /// camina es la casilla (<c>WalkAsync</c>), así que <c>_body.anchoredPosition</c> —relativa
        /// a la propia casilla— vale siempre (0,0) y la estela nunca junta puntos. Muestrear en el
        /// espacio del contenedor de la casilla (su abuelo) sí capta el paneo de la casilla, y es
        /// además invariante al paneo y al zoom del propio contenedor: un punto pegado a él
        /// conserva su posición local aunque el contenedor se mueva o escale.
        /// </remarks>
        internal void Step(float time)
        {
            if (_body == null)
            {
                return;
            }

            var space = _body.parent != null ? _body.parent.parent as RectTransform : null;
            if (space == null)
            {
                return;
            }

            _path.Sample(space.InverseTransformPoint(_body.position), time);
            Render(_path.Points, space);
        }

        private void Render(IReadOnlyList<TrailPoint> points, RectTransform space)
        {
            for (var i = 0; i < points.Count; i++)
            {
                var dot = Dot(i);
                // En espacio de mundo, desde el punto guardado en el espacio del contenedor: así
                // no importa cuántos niveles de jerarquía haya entre la estela y ese contenedor.
                dot.rectTransform.position = space.TransformPoint(points[i].Position);
                dot.rectTransform.localScale = Vector3.one * points[i].Scale;
                dot.color = new Color(DotColor.r, DotColor.g, DotColor.b, points[i].Alpha);
                dot.gameObject.SetActive(true);
            }

            for (var i = points.Count; i < _dots.Count; i++)
            {
                _dots[i].gameObject.SetActive(false);
            }
        }

        private Image Dot(int index)
        {
            if (index < _dots.Count)
            {
                return _dots[index];
            }

            var dotObject = new GameObject($"Punto{index}", typeof(RectTransform), typeof(Image));
            dotObject.transform.SetParent(transform, false);
            var image = dotObject.GetComponent<Image>();
            image.sprite = dotSprite;
            image.raycastTarget = false;
            image.rectTransform.sizeDelta = new Vector2(DotSize, DotSize);
            _dots.Add(image);
            return image;
        }

#if UNITY_INCLUDE_TESTS
        /// <summary>Para probar sin pasar por <see cref="Awake"/> (Play Mode no llamado en EditMode).</summary>
        internal void InitializeForTest()
        {
            _path ??= new TrailPath(MinDistance);
            _body ??= transform.parent as RectTransform;
        }

        /// <summary>Cuántos puntos de la estela están visibles ahora mismo.</summary>
        internal int ActiveDotCount
        {
            get
            {
                var count = 0;
                foreach (var dot in _dots)
                {
                    if (dot.gameObject.activeSelf)
                    {
                        count++;
                    }
                }

                return count;
            }
        }
#endif
    }
}
