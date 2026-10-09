using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scaffolding.Tests
{
    /// <summary>
    /// El personaje de perfil al moverse y de frente en reposo (Santiago, 09/10/2026, INC-134): la tabla
    /// de vistas por acción (<see cref="ActionView"/>), la regla del lado (<see cref="Heading"/>) y el
    /// comportamiento de <see cref="CharacterRig"/>. Los rigs se construyen en código, como en
    /// <c>CharacterRigLimbTests</c>: estas pruebas no dependen de que los prefabs ya tengan el cuerpo de
    /// perfil (eso lo exige <c>CharacterRig_INC134_LaFamiliaTieneCuerpoDePerfilConArte</c>).
    /// </summary>
    public class CharacterViewTests
    {
        /// <summary>Las acciones de perfil, tal como las fijó Santiago: recorrer o trabajar el entorno.</summary>
        private static readonly ActorAction[] AccionesDePerfil =
        {
            ActorAction.Walk, ActorAction.Run, ActorAction.Carry, ActorAction.Push,
            ActorAction.PickUp, ActorAction.Kneel, ActorAction.Blow
        };

        private static readonly ActorAction[] TodasLasAcciones = (ActorAction[])Enum.GetValues(typeof(ActorAction));

        private readonly List<UnityEngine.Object> _creados = new List<UnityEngine.Object>();
        private GameObject _raiz;

        [TearDown]
        public void TearDown()
        {
            if (_raiz != null)
            {
                UnityEngine.Object.DestroyImmediate(_raiz);
                _raiz = null;
            }

            foreach (var objeto in _creados)
            {
                if (objeto != null)
                {
                    UnityEngine.Object.DestroyImmediate(objeto);
                }
            }

            _creados.Clear();
        }

        // ---- ActionView: la vista la decide la acción ----

        [Test]
        public void ActionView_INC134_LaTablaCubreTodasLasAcciones()
        {
            Assert.That(TodasLasAcciones, Has.Length.EqualTo(22), "si el enum crece, esta prueba pide decidir la vista de la acción nueva");

            foreach (var accion in TodasLasAcciones)
            {
                var esperada = AccionesDePerfil.Contains(accion) ? CharacterView.Profile : CharacterView.Front;
                Assert.That(ActionView.For(accion), Is.EqualTo(esperada), $"{accion}");
            }

            Assert.That(TodasLasAcciones.Where(accion => ActionView.For(accion) == CharacterView.Profile),
                Is.EquivalentTo(AccionesDePerfil), "de perfil exactamente: caminar, correr, cargar, empujar, recoger, arrodillarse y soplar");
        }

        /// <summary>
        /// Moverse con Idle (la balsa que cruza, Algoritm flotando) sigue de frente, y los gestos hacia el
        /// estudiante —hablar, señalar, celebrar, animar— también: manda la acción y no el desplazamiento.
        /// </summary>
        [Test]
        public void ActionView_INC134_LosGestosHaciaElEstudianteVanDeFrente()
        {
            foreach (var accion in new[]
                     {
                         ActorAction.Idle, ActorAction.Hidden, ActorAction.Talk, ActorAction.Strike, ActorAction.Hammer,
                         ActorAction.Point, ActorAction.Observe, ActorAction.Celebrate, ActorAction.Encourage,
                         ActorAction.Hug, ActorAction.Surprise, ActorAction.Sleep, ActorAction.Appear,
                         ActorAction.Vanish, ActorAction.Spin
                     })
            {
                Assert.That(ActionView.For(accion), Is.EqualTo(CharacterView.Front), $"{accion}");
            }
        }

        // ---- Heading: el lado de quien se mueve ----

        [Test]
        public void Heading_INC134_IzquierdaYArribaMiranALaIzquierdaDerechaYAbajoALaDerecha()
        {
            // Los cuatro ejes.
            Assert.That(Heading.FacesLeft(Vector2.left), Is.True, "izquierda");
            Assert.That(Heading.FacesLeft(Vector2.up), Is.True, "arriba mira a la izquierda");
            Assert.That(Heading.FacesLeft(Vector2.right), Is.False, "derecha");
            Assert.That(Heading.FacesLeft(Vector2.down), Is.False, "abajo mira a la derecha");

            // Las cuatro diagonales con los dos ejes iguales: en un empate manda el horizontal.
            Assert.That(Heading.FacesLeft(new Vector2(-1f, 1f)), Is.True, "arriba a la izquierda: empate, horizontal, izquierda");
            Assert.That(Heading.FacesLeft(new Vector2(1f, 1f)), Is.False, "arriba a la derecha: empate, horizontal, derecha");
            Assert.That(Heading.FacesLeft(new Vector2(-1f, -1f)), Is.True, "abajo a la izquierda: empate, horizontal, izquierda");
            Assert.That(Heading.FacesLeft(new Vector2(1f, -1f)), Is.False, "abajo a la derecha: empate, horizontal, derecha");

            // En una diagonal decide el eje dominante.
            Assert.That(Heading.FacesLeft(new Vector2(1f, 0.4f)), Is.False, "más a la derecha que arriba: derecha");
            Assert.That(Heading.FacesLeft(new Vector2(-1f, -0.4f)), Is.True, "más a la izquierda que abajo: izquierda");
            Assert.That(Heading.FacesLeft(new Vector2(0.4f, 1f)), Is.True, "más arriba que a la derecha: arriba, izquierda");
            Assert.That(Heading.FacesLeft(new Vector2(-0.4f, -1f)), Is.False, "más abajo que a la izquierda: abajo, derecha");

            // La longitud no importa, solo la dirección.
            Assert.That(Heading.FacesLeft(new Vector2(-0.001f, 0f)), Is.True, "un desplazamiento mínimo pero no nulo sí decide");
            Assert.That(Heading.FacesLeft(new Vector2(0f, -250f)), Is.False, "ni uno enorme");
        }

        [Test]
        public void Heading_INC134_UnVectorNuloNoDecideNada()
        {
            Assert.That(Heading.FacesLeft(Vector2.zero), Is.Null, "sin dirección no hay rumbo: quien llama conserva el anterior");
            Assert.That(Heading.FacesLeft(new Vector2(1e-7f, -1e-7f)), Is.Null, "ni con un residuo de coma flotante");
        }

        [Test]
        public void Heading_INC134_EncararUnObjetivoMiraHaciaSuLado()
        {
            Assert.That(Heading.FacesLeftToward(100f, 40f, fallback: false), Is.True, "el objetivo a la izquierda: izquierda");
            Assert.That(Heading.FacesLeftToward(100f, 400f, fallback: true), Is.False, "el objetivo a la derecha: derecha");
            Assert.That(Heading.FacesLeftToward(100f, 100f, fallback: true), Is.True, "en la misma vertical conserva el lado que tenía");
            Assert.That(Heading.FacesLeftToward(100f, 100f, fallback: false), Is.False);
        }

        // ---- CharacterRig: dos cuerpos, la acción decide ----

        [Test]
        public void CharacterRig_INC134_AlMoverseMuestraElPerfilYEnReposoElFrente()
        {
            var m = Construir(conPerfil: true, conArte: true);
            Assert.That(m.Rig.HasProfile, Is.True, "con los nodos y el sprite del torso, tiene perfil");

            foreach (var accion in TodasLasAcciones)
            {
                m.Rig.Play(accion);

                var perfil = AccionesDePerfil.Contains(accion);
                Assert.That(m.Rig.View, Is.EqualTo(perfil ? CharacterView.Profile : CharacterView.Front), $"{accion}: vista");
                Assert.That(m.Perfil.gameObject.activeSelf, Is.EqualTo(perfil), $"{accion}: el cuerpo de perfil solo se ve en las acciones de perfil");
                Assert.That(m.Cuerpo.gameObject.activeSelf, Is.EqualTo(!perfil), $"{accion}: y el de frente, en todas las demás");
            }

            m.Rig.Play(ActorAction.Walk);
            m.Rig.Play(ActorAction.Idle);
            Assert.That(m.Rig.View, Is.EqualTo(CharacterView.Front), "al quedarse quieto vuelve de frente");
            Assert.That(m.Cuerpo.gameObject.activeSelf, Is.True);
            Assert.That(m.Perfil.gameObject.activeSelf, Is.False);
        }

        /// <summary>El corte es seco y por acción: moverse con Idle (la balsa) no cambia la vista.</summary>
        [Test]
        public void CharacterRig_INC134_LaVistaLaDecideLaAccionYNoElDesplazamiento()
        {
            var m = Construir(conPerfil: true, conArte: true);

            m.Rig.Play(ActorAction.Idle);
            m.Rig.transform.localPosition = new Vector3(500f, 0f, 0f); // lo que haría quien lo desplaza
            m.Rig.Mirrored = true;

            Assert.That(m.Rig.View, Is.EqualTo(CharacterView.Front), "desplazado pero en Idle: de frente");
        }

        /// <summary>Algoritm: sin Perfil, ignora la regla y no se le toca la jerarquía.</summary>
        [Test]
        public void CharacterRig_INC134_SinCuerpoDePerfilSiempreDeFrente()
        {
            var m = Construir(conPerfil: false, conArte: false);
            Assert.That(m.Rig.HasProfile, Is.False, "sin Lienzo/Perfil no hay perfil");

            foreach (var accion in TodasLasAcciones)
            {
                m.Rig.Play(accion);

                Assert.That(m.Rig.View, Is.EqualTo(CharacterView.Front), $"{accion}: sin perfil, de frente");
                Assert.That(m.Cuerpo.gameObject.activeSelf, Is.True, $"{accion}: su cuerpo no se apaga nunca");
            }

            m.Rig.Mirrored = true;
            m.Rig.Play(ActorAction.Walk);
            Assert.That(m.Lienzo.localScale.x, Is.GreaterThan(0f), "y no se espeja: el frente no se voltea por el rumbo");
        }

        /// <summary>
        /// Un prefab con los nodos de perfil pero sin su dibujo (la ronda del Editor aún no asignó los sprites)
        /// sigue caminando de frente en vez de volverse invisible.
        /// </summary>
        [Test]
        public void CharacterRig_INC134_SinArteDePerfilSiempreDeFrente()
        {
            var m = Construir(conPerfil: true, conArte: false);
            Assert.That(m.Rig.HasProfile, Is.False, "el torso de perfil no tiene sprite: el arte no llegó");

            m.Rig.Play(ActorAction.Walk);

            Assert.That(m.Rig.View, Is.EqualTo(CharacterView.Front), "sin dibujo de perfil, camina de frente");
            Assert.That(m.Cuerpo.gameObject.activeSelf, Is.True, "y no se vuelve invisible");
            Assert.That(m.Perfil.gameObject.activeSelf, Is.False, "ni se dibuja el cuerpo de perfil vacío encima del de frente (nace encendido)");

            // Cuando el dibujo llega (se asigna el sprite) el mismo rig pasa a perfil sin reconstruirse.
            m.Torso.sprite = NuevoSprite("torso_perfil");
            Assert.That(m.Rig.HasProfile, Is.True, "con el sprite, ya tiene perfil");
            m.Rig.Play(ActorAction.Run);
            Assert.That(m.Rig.View, Is.EqualTo(CharacterView.Profile));
            Assert.That(m.Cuerpo.gameObject.activeSelf, Is.False);
            Assert.That(m.Perfil.gameObject.activeSelf, Is.True);

            // Y si el arte se quita mientras se ve de perfil, el frente vuelve: nunca queda el personaje invisible.
            m.Torso.sprite = null;
            m.Rig.Play(ActorAction.Walk);
            Assert.That(m.Rig.HasProfile, Is.False);
            Assert.That(m.Rig.View, Is.EqualTo(CharacterView.Front));
            Assert.That(m.Cuerpo.gameObject.activeSelf, Is.True, "el de frente se enciende");
            Assert.That(m.Perfil.gameObject.activeSelf, Is.False, "y el de perfil se apaga");
        }

        /// <summary>Sin lienzo (rig a medio armar) no hay dónde buscar los cuerpos: de frente y sin lanzar.</summary>
        [Test]
        public void CharacterRig_INC134_SinLienzoNoLanza()
        {
            _raiz = new GameObject("Rig", typeof(RectTransform), typeof(CharacterRig));
            var rig = _raiz.GetComponent<CharacterRig>();

            Assert.DoesNotThrow(() => rig.Play(ActorAction.Walk));
            Assert.DoesNotThrow(() => rig.Mirrored = true);
            Assert.That(rig.HasProfile, Is.False);
            Assert.That(rig.View, Is.EqualTo(CharacterView.Front));
        }

        [Test]
        public void CharacterRig_INC134_ElEspejoSoloVolteaElPerfil()
        {
            var m = Construir(conPerfil: true, conArte: true);
            m.Rig.Mirrored = true;
            Assert.That(m.Rig.Mirrored, Is.True, "el valor se conserva: mira a la izquierda");

            m.Rig.Play(ActorAction.Idle);
            Assert.That(m.Lienzo.localScale.x, Is.GreaterThan(0f), "de frente no se espeja aunque Mirrored sea cierto");

            m.Rig.Play(ActorAction.Walk);
            Assert.That(m.Lienzo.localScale.x, Is.LessThan(0f), "de perfil y mirando a la izquierda, el lienzo se voltea");
            Assert.That(Mathf.Abs(m.Lienzo.localScale.x), Is.EqualTo(m.Lienzo.localScale.y).Within(1e-5f), "sin deformarse");

            m.Rig.Play(ActorAction.Talk);
            Assert.That(m.Lienzo.localScale.x, Is.GreaterThan(0f), "al volver de frente, el lienzo recupera su sentido");

            m.Rig.Play(ActorAction.Push);
            Assert.That(m.Lienzo.localScale.x, Is.LessThan(0f), "y al volver a perfil, el lado que tocaba");

            m.Rig.Mirrored = false;
            Assert.That(m.Lienzo.localScale.x, Is.GreaterThan(0f), "mirando a la derecha, de perfil: en su sentido dibujado (canónico)");

            m.Rig.Mirrored = true;
            Assert.That(m.Lienzo.localScale.x, Is.LessThan(0f), "cambiar el lado en perfil se aplica al instante");
        }

        /// <summary>
        /// La escala del lienzo sale de la casilla, no del espejo: el perfil, sea cual sea su lado, mide lo mismo
        /// que el frente (los clips trabajan en unidades del lienzo).
        /// </summary>
        [Test]
        public void CharacterRig_INC134_ElLadoNoCambiaElTamanoDelLienzo()
        {
            var m = Construir(conPerfil: true, conArte: true);
            ((RectTransform)m.Rig.transform).sizeDelta = new Vector2(512f, 512f);
            m.Rig.Mirrored = false; // asignarlo ajusta el lienzo a la casilla (en EditMode no corren Awake ni OnEnable)
            m.Rig.Play(ActorAction.Idle);
            var frente = m.Lienzo.localScale.y;

            m.Rig.Mirrored = true;
            m.Rig.Play(ActorAction.Walk);

            Assert.That(frente, Is.EqualTo(0.5f).Within(1e-5f), "512 de casilla sobre 1024 de lienzo");
            Assert.That(Mathf.Abs(m.Lienzo.localScale.x), Is.EqualTo(frente).Within(1e-5f));
            Assert.That(m.Lienzo.localScale.y, Is.EqualTo(frente).Within(1e-5f));
        }

        // ---- Construcción de la jerarquía ----

        /// <summary>
        /// Lienzo con Cuerpo (Tronco con BrazoIzq, BrazoDer y Torso, para que ArmLayering encuentre lo suyo al
        /// golpear) y, con <paramref name="conPerfil"/>, Perfil/Tronco/Torso justo después, con una Image en el
        /// torso. Con <paramref name="conArte"/> el torso lleva sprite. Perfil nace encendido, como un prefab
        /// guardado sin apagarlo: el rig tiene que apagarlo.
        /// </summary>
        private Maniqui Construir(bool conPerfil, bool conArte)
        {
            _raiz = new GameObject("Rig", typeof(RectTransform), typeof(CharacterRig));
            ((RectTransform)_raiz.transform).sizeDelta = new Vector2(1024f, 1024f);
            var lienzo = Nuevo("Lienzo", _raiz.transform);
            var cuerpo = Nuevo("Cuerpo", lienzo);
            var tronco = Nuevo("Tronco", cuerpo);
            Nuevo("BrazoIzq", tronco);
            Nuevo("BrazoDer", tronco);
            Nuevo("Torso", tronco);
            var m = new Maniqui { Rig = _raiz.GetComponent<CharacterRig>(), Lienzo = lienzo, Cuerpo = cuerpo };

            if (conPerfil)
            {
                m.Perfil = Nuevo("Perfil", lienzo);
                var troncoDePerfil = Nuevo("Tronco", m.Perfil);
                var torso = new GameObject("Torso", typeof(RectTransform), typeof(Image));
                torso.transform.SetParent(troncoDePerfil, false);
                m.Torso = torso.GetComponent<Image>();
                if (conArte)
                {
                    m.Torso.sprite = NuevoSprite("torso_perfil");
                }
            }

            var serializado = new SerializedObject(m.Rig);
            serializado.FindProperty("stage").objectReferenceValue = lienzo;
            serializado.ApplyModifiedPropertiesWithoutUndo();
            return m;
        }

        private static Transform Nuevo(string nombre, Transform padre)
        {
            var go = new GameObject(nombre, typeof(RectTransform));
            go.transform.SetParent(padre, false);
            return go.transform;
        }

        private Sprite NuevoSprite(string nombre)
        {
            var textura = new Texture2D(2, 2) { name = nombre };
            var sprite = Sprite.Create(textura, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
            sprite.name = nombre;
            _creados.Add(textura);
            _creados.Add(sprite);
            return sprite;
        }

        private sealed class Maniqui
        {
            public CharacterRig Rig;
            public Transform Lienzo;
            public Transform Cuerpo;
            public Transform Perfil;
            public Image Torso;
        }
    }
}
