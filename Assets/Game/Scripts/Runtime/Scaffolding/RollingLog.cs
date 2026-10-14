using UnityEngine;
using UnityEngine.UI;

namespace Game.Scaffolding
{
    /// <summary>
    /// Un tronco que rueda, dibujado como el cilindro que es: una sola textura lo envuelve —la
    /// corteza por el costado, el corte en la cara— y gira con el objeto (RF-26).
    /// </summary>
    /// <remarks>
    /// **Es 2D, no 3D de motor**: una malla de uGUI calculada como la proyección ortográfica del
    /// cilindro, sin cámara, modelo ni <c>MeshRenderer</c> (SPEC deja el 3D fuera de alcance). El
    /// costado se parte en tiras pegadas a la superficie y solo se pintan las que miran a quien
    /// juega: la corteza se aprieta sola hacia los bordes y no hay costura que resolver.
    ///
    /// **Lee el giro, no lo manda.** Quien mueve el tronco —<c>RollMotion</c> en la narrativa, el
    /// cursor en el bosque— sigue girando el <c>RectTransform</c> del objeto, como siempre; aquí se
    /// deshace ese giro para que el cilindro siga apuntando al fondo, y se pasa a la textura.
    ///
    /// El contorno no va pintado porque depende de la vista y no de la superficie: es la silueta
    /// en oscuro debajo de todo, con el color del anillo del corte. Su borde se difumina un píxel
    /// porque la interfaz no tiene antialiasing (MSAA apagado en <c>UniversalRP.asset</c>) y una
    /// arista de malla se vería en escalera.
    /// </remarks>
    public class RollingLog : MaskableGraphic, ICanvasRaycastFilter
    {
        /// <summary>Tiras por vuelta. Con 48 ninguna arista se distingue al tamaño de juego.</summary>
        private const int Segments = 48;

        /// <summary>Diámetro del tronco sobre el lado del objeto: la proporción de los troncos entregados (236 de 256).</summary>
        private const float Fill = 0.92f;

        private RollingLogLook _look;
        private Image _face;
        private float _spin = float.NaN;
        private float _pixel;
        private bool _silhouetteOnly;

#if UNITY_INCLUDE_TESTS
        /// <summary>El giro que se pasa a la textura, en grados.</summary>
        internal float Spin => _spin;
#endif

        /// <inheritdoc/>
        public override Texture mainTexture =>
            !_silhouetteOnly && _look != null && _look.Texture != null ? _look.Texture : s_WhiteTexture;

        /// <summary>La vista con la que se dibuja.</summary>
        internal RollingLogLook Look => _look;

        /// <summary>La imagen del objeto, de la que se lee el giro y el espejo.</summary>
        internal Image Face => _face;

        /// <summary>Solo el contorno, de un color plano: la sombra de gota del tronco (<see cref="PropShadow"/>).</summary>
        internal bool SilhouetteOnly => _silhouetteOnly;

        /// <summary>Radio exterior, en unidades locales del objeto: lo que avanza por vuelta dividido entre 2π.</summary>
        public float Radius
        {
            get
            {
                var size = rectTransform.rect.size;
                return Mathf.Min(size.x, size.y) * Fill / 2f;
            }
        }

        /// <summary>
        /// Dibuja <paramref name="look"/> en lugar de <paramref name="face"/>, que sigue siendo la
        /// imagen del objeto —la que leen las pruebas— pero transparente. Con
        /// <paramref name="clickable"/> el clic lo recibe el cuerpo que se dibuja y no el cuadrado
        /// de <paramref name="face"/> (INC-155); sin él, el tronco no capta clics (la narrativa).
        /// </summary>
        public static RollingLog Attach(Image face, RollingLogLook look, bool clickable = false)
        {
            var part = new GameObject("Tronco", typeof(RectTransform), typeof(CanvasRenderer));
            part.layer = face.gameObject.layer;
            var rect = (RectTransform)part.transform;
            rect.SetParent(face.rectTransform, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;

            var log = part.AddComponent<RollingLog>();
            log._look = look;
            log._face = face;
            log.color = face.color;
            log.raycastTarget = false;
            // Alfa en el color y no la imagen apagada: apagada dejaría de recibir el clic, y el
            // tinte del botón pisa el alfa del CanvasRenderer pero no el del color.
            face.color = new Color(face.color.r, face.color.g, face.color.b, 0f);
            if (clickable)
            {
                log.MakeClickable();
            }

            log.LateUpdate();
            return log;
        }

        /// <summary>
        /// El cuerpo recibe el clic en lugar de <see cref="Face"/> (INC-155). El cuerpo se dibuja
        /// mucho más allá del cuadrado del objeto —hacia arriba y a la derecha, a lo largo del
        /// eje—, y ese cuadrado gira con el objeto mientras el cilindro no: un relleno fijo sobre la
        /// imagen del objeto no puede seguirlo. Este gráfico sí está alineado con la pantalla (se
        /// deshace el giro y el espejo en <c>LateUpdate</c>), así que su rectángulo se ensancha una
        /// vez con <see cref="Graphic.raycastPadding"/> hasta la caja del cuerpo y
        /// <see cref="IsRaycastLocationValid"/> recorta lo que sobra a la silueta exacta. La escala
        /// del objeto (lejanía, realce) viaja sola: todo se mide en unidades locales.
        /// </summary>
        private void MakeClickable()
        {
            // ponytail: el relleno se calcula una vez; si el tamaño del objeto cambiara en marcha
            // habría que recalcularlo en OnRectTransformDimensionsChange. Hoy no cambia.
            raycastTarget = true;
            _face.raycastTarget = false;

            var rect = rectTransform.rect;
            var body = BodyBounds(Radius, _look);
            // Solo ensancha (nunca encoge): lo que sobra lo recorta la silueta.
            raycastPadding = new Vector4(
                Mathf.Min(0f, body.xMin - rect.xMin), Mathf.Min(0f, body.yMin - rect.yMin),
                Mathf.Min(0f, rect.xMax - body.xMax), Mathf.Min(0f, rect.yMax - body.yMax));
        }

        /// <summary>
        /// Si <paramref name="screenPoint"/> cae sobre el cuerpo que se dibuja —el corte, el costado
        /// y el fondo—, no sobre el rectángulo del objeto (INC-155).
        /// </summary>
        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera) =>
            _look != null &&
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out var local) &&
            BodyContains(local, Radius, _look);

        /// <summary>
        /// Los semiejes de la silueta del cilindro (<paramref name="across"/>, a lo largo del eje) y
        /// lo que separa el corte del fondo (<paramref name="reach"/>). Son los números con los
        /// que <see cref="OnPopulateMesh"/> traza la silueta: la zona de clic no puede discrepar de
        /// lo que se ve.
        /// </summary>
        private static void SilhouetteSize(float outer, RollingLogLook look, out float across, out float reach)
        {
            var outline = outer * look.OutlineWidth;
            var tilt = look.TiltDegrees * Mathf.Deg2Rad;
            across = (outer - outline) * Mathf.Cos(tilt) + outline;
            reach = outer * look.Depth * Mathf.Sin(tilt);
        }

        /// <summary>
        /// Si un punto, en unidades locales desde el centro del corte, cae dentro de la silueta: el
        /// casco de dos elipses iguales —la del corte y la del fondo— separadas por <c>reach</c> a
        /// lo largo del eje, que es la suma de una elipse y un segmento.
        /// </summary>
        public static bool BodyContains(Vector2 point, float radius, RollingLogLook look)
        {
            SilhouetteSize(radius, look, out var across, out var reach);
            var axis = look.AxisDegrees * Mathf.Deg2Rad;
            var (cos, sin) = (Mathf.Cos(axis), Mathf.Sin(axis));
            var along = point.x * cos + point.y * sin; // en el marco del eje
            var side = point.y * cos - point.x * sin;
            // Lo más cerca que está del segmento corte-fondo; desde ahí, es una elipse.
            var gap = (along - Mathf.Clamp(along, 0f, reach)) / across;
            var rise = side / radius;
            return gap * gap + rise * rise <= 1f;
        }

        /// <summary>La caja, en unidades locales desde el centro del corte, que envuelve la silueta.</summary>
        public static Rect BodyBounds(float radius, RollingLogLook look)
        {
            SilhouetteSize(radius, look, out var across, out var reach);
            var axis = look.AxisDegrees * Mathf.Deg2Rad;
            var (cos, sin) = (Mathf.Cos(axis), Mathf.Sin(axis));
            // Media anchura de una elipse girada, y el fondo desplazado a lo largo del eje.
            var halfX = Mathf.Sqrt(across * across * cos * cos + radius * radius * sin * sin);
            var halfY = Mathf.Sqrt(across * across * sin * sin + radius * radius * cos * cos);
            var back = new Vector2(cos, sin) * reach;
            return Rect.MinMaxRect(
                Mathf.Min(0f, back.x) - halfX, Mathf.Min(0f, back.y) - halfY,
                Mathf.Max(0f, back.x) + halfX, Mathf.Max(0f, back.y) + halfY);
        }

        /// <summary>
        /// La sombra del tronco de <paramref name="face"/>: el mismo cilindro reducido a su contorno,
        /// de <paramref name="color"/> plano, colgado de <paramref name="parent"/>. Lee el giro y el
        /// espejo de <paramref name="face"/> igual que el tronco, así que su padre tiene que copiar
        /// el giro y la escala del objeto —lo hace <see cref="PropShadow"/>—; la imagen del objeto
        /// no se toca.
        /// </summary>
        internal static RollingLog AttachSilhouette(RectTransform parent, Image face, RollingLogLook look, Color color)
        {
            var part = new GameObject("Tronco", typeof(RectTransform), typeof(CanvasRenderer));
            part.layer = parent.gameObject.layer;
            var rect = (RectTransform)part.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;

            var log = part.AddComponent<RollingLog>();
            log._look = look;
            log._face = face;
            log._silhouetteOnly = true;
            log.color = color;
            log.raycastTarget = false;
            log.LateUpdate();
            return log;
        }

        /// <summary>
        /// Dónde cae un punto del costado visto con <paramref name="tiltDegrees"/> de inclinación
        /// (0 de frente, 90 de costado): <paramref name="rimDegrees"/> alrededor del eje (0 el que da
        /// hacia el fondo) y <paramref name="along"/> a lo largo de él desde el corte. X apunta hacia
        /// donde se aleja el tronco en pantalla.
        /// </summary>
        public static Vector2 Project(float rimDegrees, float along, float radius, float tiltDegrees)
        {
            var rim = rimDegrees * Mathf.Deg2Rad;
            var tilt = tiltDegrees * Mathf.Deg2Rad;
            return new Vector2(radius * Mathf.Cos(rim) * Mathf.Cos(tilt) + along * Mathf.Sin(tilt),
                radius * Mathf.Sin(rim));
        }

        /// <summary>Después de quien mueve el tronco: el giro de este cuadro ya está puesto.</summary>
        private void LateUpdate()
        {
            if (_face == null)
            {
                return;
            }

            // Deshace el giro —y el espejo— del objeto. El espejo se aplica antes que el giro, así
            // que el objeto se ve girar su giro local tal cual: eso es lo que rueda la textura, con
            // espejo o sin él. Deshacerlo sí depende del espejo, que invierte el sentido del giro.
            var root = _face.rectTransform;
            var mirror = Mathf.Sign(root.localScale.x * root.localScale.y);
            var spin = root.localEulerAngles.z;
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, -mirror * spin);
            rectTransform.localScale = new Vector3(mirror, 1f, 1f);

            // El tinte del botón (al pasar el cursor, al pulsar) cae en la imagen del objeto.
            canvasRenderer.SetColor(_face.canvasRenderer.GetColor());

            var scale = Mathf.Abs(rectTransform.lossyScale.y);
            var pixel = scale > 0f ? 1f / scale : 0f; // la interfaz es superpuesta: una unidad del mundo es un píxel
            if (!Mathf.Approximately(spin, _spin) || !Mathf.Approximately(pixel, _pixel))
            {
                _spin = spin;
                _pixel = pixel;
                SetVerticesDirty();
            }
        }

        /// <inheritdoc/>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (_look == null || float.IsNaN(_spin) || (!_silhouetteOnly && _look.Texture == null))
            {
                return;
            }

            var outer = Radius;
            var outline = outer * _look.OutlineWidth;
            var inner = outer - outline;
            var tilt = _look.TiltDegrees;
            var length = outer * _look.Depth;
            SilhouetteSize(outer, _look, out var across, out var reach);
            var toLocal = Quaternion.Euler(0f, 0f, _look.AxisDegrees);
            // σ, el ángulo en la textura, es ψ − turn: el corte gira con el objeto y el eje no.
            var turn = _spin - _look.AxisDegrees;
            Color32 tint = color;

            if (_silhouetteOnly)
            {
                // La sombra: el contorno entero, en el color plano del gráfico y sin textura.
                AddSilhouette(vh, toLocal, across, outer, reach, tint, Vector2.zero);
                return;
            }

            // El corte es el cuadrado de la derecha; la corteza, el resto. Un texel de margen en la
            // corteza para que el filtrado no traiga el borde del corte.
            var texture = _look.Texture;
            var capShare = (float)texture.height / texture.width;
            var texel = 1f / texture.width;
            var bark = new Rect(texel, 0f, 1f - capShare - 2f * texel, 1f);
            var capCenter = new Vector2(1f - capShare / 2f, 0.5f);
            var capRadius = new Vector2(capShare / 2f, 0.5f);

            // 1. La silueta, en el color del anillo del corte, que es el del contorno.
            var ring = capCenter + new Vector2(capRadius.x * (1f - _look.OutlineWidth / 2f), 0f);
            AddSilhouette(vh, toLocal, across, outer, reach, tint, ring);

            // 2. El costado: las tiras de la mitad que mira a quien juega, recortadas en ±90°.
            var step = 360f / Segments;
            for (var i = 0; i < Segments; i++)
            {
                var from = Mathf.DeltaAngle(0f, i * step + turn);
                var low = Mathf.Max(from, -90f);
                var high = Mathf.Min(from + step, 90f);
                if (low >= high)
                {
                    continue;
                }

                var first = vh.currentVertCount;
                AddLine(low);
                AddLine(high);
                vh.AddTriangle(first, first + 1, first + 3);
                vh.AddTriangle(first + 3, first + 2, first);

                // Una línea del costado, del corte al fondo, con su vuelta en la textura.
                void AddLine(float rim)
                {
                    var v = bark.yMin + (i * step + rim - from) / 360f * bark.height;
                    vh.AddVert(toLocal * Project(rim, 0f, inner, tilt), tint, new Vector2(bark.xMin, v));
                    vh.AddVert(toLocal * Project(rim, length, inner, tilt), tint, new Vector2(bark.xMax, v));
                }
            }

            // 3. El corte, encima: la elipse que deja la inclinación, girando con el objeto.
            if (tilt >= 90f)
            {
                return; // de costado no se ve
            }

            var center = vh.currentVertCount;
            vh.AddVert(Vector3.zero, tint, capCenter);
            for (var i = 0; i <= Segments; i++)
            {
                var rim = i * step;
                var angle = (rim - turn) * Mathf.Deg2Rad;
                vh.AddVert(toLocal * Project(rim, 0f, outer, tilt), tint,
                    capCenter + Vector2.Scale(capRadius, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle))));
                if (i > 0)
                {
                    vh.AddTriangle(center, center + i, center + i + 1);
                }
            }
        }

        /// <summary>
        /// La silueta del cilindro: la mitad trasera de la elipse del corte, la de la tapa del fondo
        /// y las dos rectas que las unen, con un píxel de borde que se desvanece.
        /// </summary>
        private void AddSilhouette(VertexHelper vh, Quaternion toLocal, float across, float up, float reach,
            Color32 tint, Vector2 uv)
        {
            var clear = new Color32(tint.r, tint.g, tint.b, 0);
            var center = vh.currentVertCount;
            vh.AddVert(toLocal * new Vector3(reach / 2f, 0f), tint, uv);

            var half = Segments / 2;
            var points = 2 * (half + 1);
            for (var i = 0; i < points; i++)
            {
                // De 90° a 270° en la tapa del corte y de −90° a 90° en la del fondo.
                var back = i > half;
                var angle = (back ? -90f + (i - half - 1) * 360f / Segments : 90f + i * 360f / Segments) * Mathf.Deg2Rad;
                var (cos, sin) = (Mathf.Cos(angle), Mathf.Sin(angle));
                var edge = new Vector2((back ? reach : 0f) + across * cos, up * sin);
                var normal = new Vector2(up * cos, across * sin).normalized;
                vh.AddVert(toLocal * edge, tint, uv);
                vh.AddVert(toLocal * (edge + normal * _pixel), clear, uv);
            }

            for (var i = 0; i < points; i++)
            {
                var here = center + 1 + 2 * i;
                var next = center + 1 + 2 * ((i + 1) % points);
                vh.AddTriangle(center, here, next);
                vh.AddTriangle(here, here + 1, next + 1);
                vh.AddTriangle(next + 1, next, here);
            }
        }
    }
}
