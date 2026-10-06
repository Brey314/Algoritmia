using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scaffolding
{
    /// <summary>
    /// El orden de dibujo de los brazos de un personaje respecto de su torso, que cambia mientras
    /// golpea (Santiago, 05/10/2026, INC-132): en la familia los brazos van DETRÁS del torso —los
    /// hombros salen por detrás de él— y al golpear las piedras pasan DELANTE, que es donde las manos
    /// se juntan. Al terminar vuelven a su sitio. C# plano; <see cref="CharacterRig"/> es el adaptador
    /// que decide cuándo.
    /// </summary>
    /// <remarks>
    /// **Por qué hace falta:** uGUI pinta los hijos de atrás adelante. Con los brazos detrás del torso,
    /// el choque de las manos delante del pecho (Papá enciende la chispa así en el Nivel 1) queda
    /// tapado por el propio cuerpo y el gesto que enseña la mecánica no se ve.
    ///
    /// **Solo toca hermanos de Tronco, por nombre, y solo los brazos que están ANTES del torso.** En
    /// Algoritm los brazos ya se dibujan después del torso y no se tocan; la cara de Algoritm
    /// (Ojos, Boca) tampoco se mueve nunca. El resto de los hijos de Tronco —el cuello con la cabeza
    /// dentro— conserva su sitio, así que la cabeza sigue detrás de los brazos que suben.
    ///
    /// **Reordenar no cambia nada más:** las curvas de los clips direccionan los nodos por ruta de
    /// nombres, no por posición, y <c>SetSiblingIndex</c> no cambia ningún fileID del prefab.
    ///
    /// **Se registra el orden con el que nació Tronco y se vuelve exactamente a él.** Solo se
    /// deshace lo que esta clase movió: si <see cref="Apply"/> nunca tuvo nada que mover, pedir
    /// «brazos detrás» no toca la jerarquía.
    /// </remarks>
    internal sealed class ArmLayering
    {
        /// <summary>El nodo que se toma de referencia: los brazos se dibujan justo después de él.</summary>
        internal const string TorsoName = "Torso";

        /// <summary>Los hijos de Tronco que pasan delante del torso (entre ellos conservan el orden que ya tenían).</summary>
        internal static readonly string[] ArmNames = { "BrazoIzq", "BrazoDer" };

        private readonly Transform _trunk;
        private readonly List<Transform> _original = new List<Transform>();
        private bool _moved;
        private bool _warned;

        /// <param name="trunk">El nodo Tronco del personaje (<c>Lienzo/Cuerpo/Tronco</c>). Puede faltar: no rompe nada.</param>
        public ArmLayering(Transform trunk)
        {
            _trunk = trunk;
            if (_trunk == null)
            {
                return;
            }

            // El orden con el que nació, para devolverlo exacto (también el de los hijos que no se tocan).
            for (var i = 0; i < _trunk.childCount; i++)
            {
                _original.Add(_trunk.GetChild(i));
            }
        }

        /// <summary>
        /// Pone los brazos delante del torso (<paramref name="armsInFront"/> = <c>true</c>) o los
        /// devuelve a su sitio de origen (<c>false</c>). Idempotente. Si falta el torso o un brazo
        /// avisa una sola vez y no hace nada: la acción del guion nunca rompe la escena.
        /// </summary>
        public void Apply(bool armsInFront)
        {
            if (armsInFront)
            {
                PutInFront();
            }
            else
            {
                Restore();
            }
        }

        private void PutInFront()
        {
            if (!TryFind(out var torso, out var arms))
            {
                return;
            }

            var current = Children();
            var torsoIndex = torso.GetSiblingIndex();
            var target = new List<Transform>(current.Count);
            var moving = new List<Transform>();

            foreach (var child in current)
            {
                // Solo se mueve el brazo que se dibuja antes del torso; el que ya va después (Algoritm) se queda.
                if (Array.IndexOf(arms, child) >= 0 && child.GetSiblingIndex() < torsoIndex)
                {
                    moving.Add(child);
                }
                else
                {
                    target.Add(child);
                }
            }

            if (moving.Count == 0)
            {
                return; // ya están delante (Algoritm) o ya se movieron
            }

            // «moving» sale en el orden en que estaban, así que entre ellos se conserva.
            target.InsertRange(target.IndexOf(torso) + 1, moving);
            Reorder(target);
            _moved = true;
        }

        private void Restore()
        {
            if (!_moved || _trunk == null)
            {
                return;
            }

            // Se ocupan los mismos huecos que hoy ocupan los hijos conocidos, pero en su orden de
            // origen. Si alguien añadió un hijo a Tronco desde entonces, se queda donde está.
            var current = Children();
            var known = new List<Transform>(_original.Count);
            foreach (var child in _original)
            {
                if (child != null && child.parent == _trunk)
                {
                    known.Add(child);
                }
            }

            var target = new List<Transform>(current);
            var next = 0;
            for (var i = 0; i < current.Count; i++)
            {
                if (known.Contains(current[i]))
                {
                    target[i] = known[next++];
                }
            }

            Reorder(target);
            _moved = false;
        }

        /// <summary>Busca el torso y los dos brazos entre los hijos directos de Tronco; si falta alguno avisa una vez.</summary>
        private bool TryFind(out Transform torso, out Transform[] arms)
        {
            torso = null;
            arms = new Transform[ArmNames.Length];
            if (_trunk == null)
            {
                Warn("no hay nodo Tronco");
                return false;
            }

            torso = _trunk.Find(TorsoName);
            var missing = new List<string>();
            if (torso == null)
            {
                missing.Add(TorsoName);
            }

            for (var i = 0; i < ArmNames.Length; i++)
            {
                arms[i] = _trunk.Find(ArmNames[i]);
                if (arms[i] == null)
                {
                    missing.Add(ArmNames[i]);
                }
            }

            if (missing.Count == 0)
            {
                return true;
            }

            Warn("a Tronco le falta " + string.Join(", ", missing));
            return false;
        }

        private List<Transform> Children()
        {
            var children = new List<Transform>(_trunk.childCount);
            for (var i = 0; i < _trunk.childCount; i++)
            {
                children.Add(_trunk.GetChild(i));
            }

            return children;
        }

        /// <summary>
        /// Deja los hijos de Tronco en el orden de <paramref name="target"/>. Va de adelante atrás
        /// asignando cada índice: así cada hijo solo se mueve hacia el frente, que es el sentido en el
        /// que <c>SetSiblingIndex</c> calcula el destino sin sorpresas, y los ya colocados no se mueven.
        /// </summary>
        private static void Reorder(List<Transform> target)
        {
            for (var i = 0; i < target.Count; i++)
            {
                if (target[i].GetSiblingIndex() != i)
                {
                    target[i].SetSiblingIndex(i);
                }
            }
        }

        private void Warn(string reason)
        {
            if (_warned)
            {
                return;
            }

            _warned = true;
            Debug.LogWarning("ArmLayering: " + reason + "; los brazos se quedan en su sitio al golpear (INC-132).", _trunk);
        }
    }
}
