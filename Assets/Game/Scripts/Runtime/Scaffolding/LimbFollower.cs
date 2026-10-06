using System.Collections.Generic;
using UnityEngine;

namespace Game.Scaffolding
{
    /// <summary>
    /// Un antebrazo que se dibuja DELANTE del torso aunque su húmero vaya detrás (Santiago,
    /// 06/10/2026, INC-133: «el húmero detrás del torso y el antebrazo delante del torso, del rostro y de
    /// las piernas»). Copia en cada cuadro la pose de un nodo vacío, el ancla, que sí cuelga del codo.
    /// C# plano; <see cref="CharacterRig"/> es el adaptador que decide cuándo.
    /// </summary>
    /// <remarks>
    /// **Por qué hace falta:** uGUI pinta los hijos de atrás adelante, y el antebrazo es hijo del codo,
    /// que es hijo del húmero. Mientras cuelgue de ellos no puede quedar delante del torso con el húmero
    /// detrás. Un <c>Canvas</c> anidado con <c>overrideSorting</c> lo resolvería, pero pintaría al
    /// personaje por encima del cuadro de diálogo (RNF-03). Por eso el antebrazo (el nodo con la Image,
    /// que conserva su fileID) cuelga de Tronco, justo donde se quiere dibujar, y el codo lleva a su lado
    /// un ancla sin Image con la pose que el antebrazo tendría si siguiera colgado de él.
    ///
    /// **Se compone la pose relativa a Tronco, no la del mundo.** El seguidor es hijo de Tronco y el
    /// ancla es su descendiente, así que todo lo que hay por encima de Tronco —el lienzo escalado para
    /// llenar la casilla, su espejo cuando el personaje mira al otro lado, la profundidad del río— es
    /// común a los dos y se cancela. Leer el mundo y dividir escalas fallaría justo ahí: con el lienzo
    /// espejado los signos de <c>lossyScale</c> no son fiables, y con el lienzo a escala cero se dividiría
    /// entre cero. Lo que sí varía en el cuadro —el giro y la escala de BrazoX y CodoX, y el «respirar» de
    /// Tronco— lo anima el Animator antes de <c>LateUpdate</c>, que es cuando corre la copia.
    ///
    /// **Supone escalas uniformes en el plano** (los clips escalan BrazoX por igual en X e Y, 0,45 al
    /// golpear): la composición posición-giro-escala no representa una cizalla. Con una escala desigual
    /// bajo un giro el antebrazo saldría un poco distinto del ancla, nunca fuera de sitio.
    ///
    /// **Solo actúa si existen los dos.** Con el arte provisional y en Algoritm —sus brazos ya van
    /// delante del cuerpo y su antebrazo sigue colgado del codo— no hay ancla y no se crea ningún
    /// seguidor: no se hace nada y nada lanza. Nada de esto toca los clips: ninguno anima el antebrazo ni
    /// el ancla.
    /// </remarks>
    internal sealed class LimbFollower
    {
        /// <summary>Prefijo del ancla: la de «AntebrazoIzq» es «AnclaAntebrazoIzq», hija del codo.</summary>
        internal const string AnchorPrefix = "Ancla";

        // Solo escribe lo que cambió, con una tolerancia mínima. Sin ella, el redondeo de ida y vuelta de un
        // RectTransform (la posición local se guarda como anclas y desplazamiento) reescribiría el antebrazo en
        // cada cuadro y ensuciaría el Canvas aunque el personaje esté quieto. Con el == de Vector3 o de
        // Quaternion habría el problema contrario: es aproximado y grueso —el de Quaternion da por iguales giros
        // de hasta ~0,16°—, y se comería el arranque lento de un gesto. Estas tolerancias son de una milésima
        // de unidad del lienzo (1024) y de ~0,001°: invisibles.
        private const float PositionEpsilon = 1e-3f;
        private const float RotationEpsilon = 1e-5f;
        private const float ScaleEpsilon = 1e-5f;

        /// <summary>Los antebrazos que se sueltan del codo: hijos directos de Tronco, de izquierda a derecha.</summary>
        internal static readonly string[] ForearmNames = { "AntebrazoIzq", "AntebrazoDer" };

        /// <summary>La ruta del ancla de cada antebrazo, desde Tronco y en el mismo orden que <see cref="ForearmNames"/>.</summary>
        private static readonly string[] AnchorPaths =
        {
            "BrazoIzq/CodoIzq/" + AnchorPrefix + "AntebrazoIzq",
            "BrazoDer/CodoDer/" + AnchorPrefix + "AntebrazoDer",
        };

        private readonly Transform _anchor;
        private readonly Transform _follower;
        private bool _warned;

        /// <param name="anchor">El nodo vacío que cuelga del codo. Puede faltar: <see cref="Sync"/> no hace nada.</param>
        /// <param name="follower">El antebrazo, hijo de Tronco, con la Image. Puede faltar.</param>
        public LimbFollower(Transform anchor, Transform follower)
        {
            _anchor = anchor;
            _follower = follower;
        }

        /// <summary>
        /// Los pares antebrazo-ancla que existen bajo <paramref name="trunk"/>, buscados por nombre: no hay
        /// referencias serializadas que cablear en los siete prefabs. Un antebrazo sin ancla (o al revés) no
        /// forma par, así que un personaje sin las anclas —arte provisional, Algoritm— devuelve una lista vacía.
        /// </summary>
        public static LimbFollower[] Discover(Transform trunk)
        {
            var found = new List<LimbFollower>(ForearmNames.Length);
            if (trunk == null)
            {
                return found.ToArray();
            }

            for (var i = 0; i < ForearmNames.Length; i++)
            {
                var follower = trunk.Find(ForearmNames[i]);
                var anchor = trunk.Find(AnchorPaths[i]);
                if (follower != null && anchor != null)
                {
                    found.Add(new LimbFollower(anchor, follower));
                }
            }

            return found.ToArray();
        }

        /// <summary>
        /// Pone el antebrazo en la pose del ancla, medida desde Tronco: posición, giro y escala locales
        /// del antebrazo. Solo escribe lo que cambió. Devuelve <c>false</c> —sin lanzar— si falta el ancla o
        /// el antebrazo o si el ancla no cuelga de Tronco (avisa una vez en ese último caso).
        /// </summary>
        public bool Sync()
        {
            if (_anchor == null || _follower == null)
            {
                return false;
            }

            var trunk = _follower.parent;
            if (trunk == null)
            {
                return false;
            }

            // De hijo a padre hasta llegar a Tronco, sin incluirlo: pose = padre ∘ pose.
            var pose = Pose.Of(_anchor);
            for (var node = _anchor.parent; node != trunk; node = node.parent)
            {
                if (node == null)
                {
                    Warn("el ancla no cuelga del mismo Tronco que el antebrazo");
                    return false;
                }

                pose = Pose.Of(node).Then(pose);
            }

            var wrote = false;
            if (!SameVector(_follower.localPosition, pose.Position, PositionEpsilon))
            {
                _follower.localPosition = pose.Position;
                wrote = true;
            }

            if (!SameRotation(_follower.localRotation, pose.Rotation))
            {
                _follower.localRotation = pose.Rotation;
                wrote = true;
            }

            if (!SameVector(_follower.localScale, pose.Scale, ScaleEpsilon))
            {
                _follower.localScale = pose.Scale;
                wrote = true;
            }

            return wrote;
        }

        private static bool SameVector(Vector3 a, Vector3 b, float epsilon) =>
            Mathf.Abs(a.x - b.x) <= epsilon && Mathf.Abs(a.y - b.y) <= epsilon && Mathf.Abs(a.z - b.z) <= epsilon;

        /// <summary>q y −q son el mismo giro: se compara con los dos signos.</summary>
        private static bool SameRotation(Quaternion a, Quaternion b) =>
            SameComponents(a, b.x, b.y, b.z, b.w) || SameComponents(a, -b.x, -b.y, -b.z, -b.w);

        private static bool SameComponents(Quaternion a, float x, float y, float z, float w) =>
            Mathf.Abs(a.x - x) <= RotationEpsilon && Mathf.Abs(a.y - y) <= RotationEpsilon
            && Mathf.Abs(a.z - z) <= RotationEpsilon && Mathf.Abs(a.w - w) <= RotationEpsilon;

        private void Warn(string reason)
        {
            if (_warned)
            {
                return;
            }

            _warned = true;
            Debug.LogWarning("LimbFollower: " + reason + "; el antebrazo se queda donde está (INC-133).", _follower);
        }

        /// <summary>Posición, giro y escala locales de un nodo: lo justo para componer la cadena codo-húmero.</summary>
        private readonly struct Pose
        {
            public readonly Vector3 Position;
            public readonly Quaternion Rotation;
            public readonly Vector3 Scale;

            private Pose(Vector3 position, Quaternion rotation, Vector3 scale)
            {
                Position = position;
                Rotation = rotation;
                Scale = scale;
            }

            public static Pose Of(Transform node) => new Pose(node.localPosition, node.localRotation, node.localScale);

            /// <summary>
            /// Compone dos eslabones de la cadena: <paramref name="child"/> está expresada en el espacio de este
            /// nodo y el resultado, en el del padre de este nodo.
            /// </summary>
            public Pose Then(Pose child) => new Pose(
                Position + Rotation * Vector3.Scale(Scale, child.Position),
                Rotation * child.Rotation,
                Vector3.Scale(Scale, child.Scale));
        }
    }
}
