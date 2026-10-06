using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Game.Scaffolding.Tests
{
    /// <summary>
    /// El antebrazo de la familia se dibuja delante del torso aunque su húmero vaya detrás (Santiago,
    /// 06/10/2026, INC-133): cuelga de Tronco y copia en cada cuadro la pose de un ancla que sí cuelga del
    /// codo (<see cref="LimbFollower"/>). Aquí las jerarquías se construyen en código, así que estas
    /// pruebas no dependen de que los prefabs ya tengan las anclas; lo que sí exige el prefab real vive en
    /// <c>CharacterRigTests</c> (<c>CharacterRig_INC133_*</c> sobre los siete prefabs).
    /// </summary>
    public class CharacterRigLimbTests
    {
        private const float PosicionTolerancia = 1e-3f;
        private const float GiroTolerancia = 1e-4f; // distancia entre los ejes girados (unos 0,006°): Quaternion.Angle da 0 por debajo de ~0,16° y no sirve para medir
        private const float EscalaTolerancia = 1e-4f;

        /// <summary>El orden de Tronco de Mamá, Niña y Niño: el cuello detrás del torso y los antebrazos al final.</summary>
        private static readonly string[] OrdenDeMama = { "Cuello", "BrazoIzq", "BrazoDer", "Torso", "AntebrazoIzq", "AntebrazoDer" };

        /// <summary>El de Papá: la cara va delante del torso, así que su cuello va después de él.</summary>
        private static readonly string[] OrdenDePapa = { "BrazoIzq", "BrazoDer", "Torso", "Cuello", "AntebrazoIzq", "AntebrazoDer" };

        private GameObject _raiz;

        [TearDown]
        public void TearDown()
        {
            if (_raiz != null)
            {
                Object.DestroyImmediate(_raiz);
                _raiz = null;
            }
        }

        // ---- La copia de pose ----

        [Test]
        public void CharacterRig_INC133_ElAntebrazoSigueAlAnclaAlGirarElHumeroYElCodo([Values("Izq", "Der")] string lado)
        {
            var m = Construir(OrdenDeMama);
            var signo = lado == "Izq" ? 1f : -1f;
            m.Lienzo.localScale = new Vector3(0.5f, 0.5f, 1f);
            Poner(m.Brazo(lado), signo * -120f, 300f, signo * 30f);
            Poner(m.Codo(lado), signo * -10f, -160f, signo * -50f);
            Poner(m.Ancla(lado), 4f, -2f, signo * 7f);
            var antebrazo = m.Antebrazo(lado);

            Assert.That(Vector3.Distance(antebrazo.position, m.Ancla(lado).position), Is.GreaterThan(1f),
                "antes de sincronizar el antebrazo está donde lo dejó el prefab (si no, la prueba no prueba nada)");

            m.Rig.SyncLimbs();

            AssertMismaPose(antebrazo, m.Ancla(lado), $"{lado}: el antebrazo copia la pose del ancla");
        }

        [Test]
        public void CharacterRig_INC133_ElAntebrazoConservaLaEscalaDelAnclaCuandoElBrazoSeEscala([Values("Izq", "Der")] string lado)
        {
            // Strike escala BrazoX a 0,45 en X e Y; el antebrazo, que ya no cuelga de él, tiene que verse igual de pequeño.
            var m = Construir(OrdenDeMama);
            var signo = lado == "Izq" ? 1f : -1f;
            m.Lienzo.localScale = new Vector3(0.3f, 0.3f, 1f);
            Poner(m.Brazo(lado), signo * -120f, 300f, signo * 12f, 0.45f);
            Poner(m.Codo(lado), signo * -10f, -160f, signo * 98f);
            var antebrazo = m.Antebrazo(lado);

            m.Rig.SyncLimbs();

            AssertMismaPose(antebrazo, m.Ancla(lado), $"{lado}: con BrazoX escalado");
            Assert.That(antebrazo.lossyScale.x, Is.EqualTo(0.3f * 0.45f).Within(EscalaTolerancia), $"{lado}: la escala del mundo es la del lienzo por la del brazo");
        }

        /// <summary>
        /// Con el personaje mirando al otro lado el lienzo se voltea (Mirrored) y el río le pone profundidad:
        /// todo eso está por encima de Tronco, común a antebrazo y ancla, y no puede descolocarlos. Con el
        /// signo de lossyScale no se podría garantizar; con la pose relativa a Tronco sí.
        /// </summary>
        [Test]
        public void CharacterRig_INC133_ElAntebrazoSigueAlAnclaConElLienzoEspejado([Values("Izq", "Der")] string lado)
        {
            var m = Construir(OrdenDeMama);
            m.Lienzo.localScale = new Vector3(-0.4f, 0.4f, 1f);
            Poner(m.Brazo(lado), -120f, 300f, 25f);
            Poner(m.Codo(lado), -10f, -160f, 40f);
            var antebrazo = m.Antebrazo(lado);

            m.Rig.SyncLimbs();

            Assert.That(Vector3.Distance(antebrazo.position, m.Ancla(lado).position), Is.LessThan(PosicionTolerancia), $"{lado}: misma posición con el lienzo espejado");
        }

        /// <summary>
        /// Los clips animan Tronco mismo (sube y baja, «respira» con la escala en Y): como el antebrazo es hijo
        /// de Tronco y el ancla su descendiente, Tronco se cancela y los dos se mueven con él.
        /// </summary>
        [Test]
        public void CharacterRig_INC133_ElAntebrazoSigueAlAnclaCuandoTroncoRespiraYGira()
        {
            var m = Construir(OrdenDeMama);
            m.Lienzo.localScale = new Vector3(0.5f, 0.5f, 1f);
            m.Tronco.localPosition = new Vector3(0f, 12f, 0f);
            m.Tronco.localRotation = Quaternion.Euler(0f, 0f, 8f);
            m.Tronco.localScale = new Vector3(1f, 1.04f, 1f);
            Poner(m.Brazo("Izq"), -120f, 300f, 20f);
            Poner(m.Codo("Izq"), -10f, -160f, -35f);

            m.Rig.SyncLimbs();

            AssertMismaPose(m.Antebrazo("Izq"), m.Ancla("Izq"), "con Tronco movido y estirado");
        }

        [Test]
        public void CharacterRig_INC133_ElAntebrazoSeEnganchaAlActivarseYEnCadaLateUpdate()
        {
            var m = Construir(OrdenDeMama);
            Poner(m.Brazo("Izq"), -120f, 300f, 30f);
            Poner(m.Codo("Izq"), -10f, -160f, -50f);

            Invocar(m.Rig, "OnEnable");
            AssertMismaPose(m.Antebrazo("Izq"), m.Ancla("Izq"), "al activarse el rig no hay un cuadro con el antebrazo despegado");

            Poner(m.Brazo("Izq"), -120f, 300f, 80f); // lo que haría el Animator en el cuadro siguiente: el brazo gira y arrastra al codo
            Assert.That(Vector3.Distance(m.Antebrazo("Izq").position, m.Ancla("Izq").position), Is.GreaterThan(1f), "el Animator mueve el ancla, no el antebrazo");

            Invocar(m.Rig, "LateUpdate");
            AssertMismaPose(m.Antebrazo("Izq"), m.Ancla("Izq"), "LateUpdate lo vuelve a pegar al ancla");
        }

        [Test]
        public void CharacterRig_INC133_UnaPoseQueNoCambiaNoSeVuelveAEscribir()
        {
            var m = Construir(OrdenDeMama);
            Poner(m.Brazo("Izq"), -120f, 300f, 30f);
            Poner(m.Codo("Izq"), -10f, -160f, -50f);
            var seguidor = new LimbFollower(m.Ancla("Izq"), m.Antebrazo("Izq"));

            Assert.That(seguidor.Sync(), Is.True, "la primera vez escribe");
            Assert.That(seguidor.Sync(), Is.False, "con el brazo quieto no vuelve a tocar el antebrazo (no ensucia el Canvas cada cuadro)");

            Poner(m.Codo("Izq"), -10f, -160f, -49.9f);
            Assert.That(seguidor.Sync(), Is.True, "un giro de una décima de grado sí se copia");
        }

        // ---- Lo que no hace nada ----

        /// <summary>Arte provisional y Algoritm: el antebrazo sigue colgando del codo y no hay ancla. Nada que copiar, nada que lance.</summary>
        [Test]
        public void CharacterRig_INC133_SinAnclaONiSeguidorNoHaceNadaNiLanza()
        {
            var m = Construir(OrdenDeMama, conAnclas: false);
            Poner(m.Brazo("Izq"), -120f, 300f, 30f);
            var antes = m.Antebrazo("Izq").localPosition;
            Assert.That(m.Antebrazo("Izq").parent, Is.SameAs(m.Codo("Izq")), "sin anclas el antebrazo cuelga del codo, como en Algoritm");

            Assert.That(LimbFollower.Discover(m.Tronco), Is.Empty, "sin anclas no hay pares");
            Assert.DoesNotThrow(() => m.Rig.SyncLimbs());
            Assert.DoesNotThrow(() => Invocar(m.Rig, "LateUpdate"));
            Assert.That(m.Antebrazo("Izq").localPosition, Is.EqualTo(antes), "no se mueve nada");

            Assert.That(new LimbFollower(null, m.Antebrazo("Izq")).Sync(), Is.False, "sin ancla");
            Assert.That(new LimbFollower(m.Codo("Izq"), null).Sync(), Is.False, "sin antebrazo");
            Assert.That(new LimbFollower(null, null).Sync(), Is.False, "sin nada");
            Assert.That(LimbFollower.Discover(null), Is.Empty, "sin Tronco");
        }

        [Test]
        public void CharacterRig_INC133_UnRigSinLienzoNoLanzaYReintentaCuandoLoTiene()
        {
            var m = Construir(OrdenDeMama);
            Poner(m.Brazo("Izq"), -120f, 300f, 30f);
            Poner(m.Codo("Izq"), -10f, -160f, -50f);
            var serializado = new SerializedObject(m.Rig);
            serializado.FindProperty("stage").objectReferenceValue = null;
            serializado.ApplyModifiedPropertiesWithoutUndo();

            Assert.DoesNotThrow(() => m.Rig.SyncLimbs(), "sin lienzo no hay dónde buscar");

            serializado.FindProperty("stage").objectReferenceValue = m.Lienzo;
            serializado.ApplyModifiedPropertiesWithoutUndo();
            m.Rig.SyncLimbs();

            AssertMismaPose(m.Antebrazo("Izq"), m.Ancla("Izq"), "con lienzo ya los sincroniza");
        }

        [Test]
        public void CharacterRig_INC133_UnAnclaQueNoCuelgaDeTroncoAvisaYNoMueveElAntebrazo()
        {
            var m = Construir(OrdenDeMama);
            var ajeno = new GameObject("Ajeno", typeof(RectTransform));
            try
            {
                var anclaAjena = new GameObject("AnclaAntebrazoIzq", typeof(RectTransform)).transform;
                anclaAjena.SetParent(ajeno.transform, false);
                anclaAjena.localPosition = new Vector3(50f, 50f, 0f);
                var antes = m.Antebrazo("Izq").localPosition;
                var seguidor = new LimbFollower(anclaAjena, m.Antebrazo("Izq"));

                LogAssert.Expect(LogType.Warning, new Regex("LimbFollower"));
                Assert.That(seguidor.Sync(), Is.False);
                Assert.DoesNotThrow(() => seguidor.Sync(), "la segunda vez tampoco lanza (y no repite el aviso)");
                Assert.That(m.Antebrazo("Izq").localPosition, Is.EqualTo(antes), "el antebrazo se queda donde está");
            }
            finally
            {
                Object.DestroyImmediate(ajeno);
            }
        }

        // ---- Orden de dibujo al golpear ----

        /// <summary>
        /// Mientras golpea, el húmero pasa delante del torso —la clase ArmLayering de INC-132— pero sigue
        /// detrás del antebrazo, que es lo que se ve chocar: «justo después del torso, antes de los
        /// antebrazos». Al terminar, el orden de origen vuelve exacto. Con los dos órdenes de la familia.
        /// </summary>
        [Test]
        public void CharacterRig_INC133_AlGolparElHumeroPasaDelanteDelTorsoPeroSigueDetrasDelAntebrazo([Values(0, 1)] int variante)
        {
            var orden = variante == 0 ? OrdenDeMama : OrdenDePapa;
            var m = Construir(orden);
            var inicial = OrdenDeDibujo(m.Tronco);
            var cuelloDetrasDelTorso = Indice(m.Tronco, "Cuello") < Indice(m.Tronco, "Torso");
            var capas = new ArmLayering(m.Tronco);

            capas.Apply(true);

            var torso = Indice(m.Tronco, "Torso");
            foreach (var lado in new[] { "Izq", "Der" })
            {
                Assert.That(Indice(m.Tronco, "Brazo" + lado), Is.GreaterThan(torso), $"{lado}: el húmero pasa delante del torso al golpear");
                Assert.That(Indice(m.Tronco, "Brazo" + lado), Is.LessThan(Indice(m.Tronco, "Antebrazo" + lado)), $"{lado}: el húmero sigue detrás del antebrazo, que es la mano que se ve");
                Assert.That(Indice(m.Tronco, "Antebrazo" + lado), Is.GreaterThan(torso), $"{lado}: el antebrazo sigue delante del torso");
            }

            Assert.That(Indice(m.Tronco, "BrazoDer"), Is.GreaterThan(Indice(m.Tronco, "BrazoIzq")), "entre los húmeros se conserva el orden");
            Assert.That(Indice(m.Tronco, "Cuello") < torso, Is.EqualTo(cuelloDetrasDelTorso), "golpear no cambia dónde está el cuello respecto del torso");
            Assert.That(OrdenDeDibujo(m.Tronco), Is.Not.EqualTo(inicial), "el golpe sí cambia el orden (si no, la prueba no prueba nada)");

            capas.Apply(false);

            Assert.That(OrdenDeDibujo(m.Tronco), Is.EqualTo(inicial), "al terminar el golpe vuelve el orden de origen, antebrazos incluidos");
        }

        // ---- Construcción de la jerarquía ----

        /// <summary>
        /// Lienzo/Cuerpo/Tronco con los hijos de Tronco en el orden dado y, bajo cada brazo, brazo → codo →
        /// (ancla). Con <paramref name="conAnclas"/> falso es el personaje de antes de INC-133 y el de
        /// Algoritm: el antebrazo cuelga del codo. Antes de sincronizar, el antebrazo suelto queda lejos.
        /// </summary>
        private Maniqui Construir(string[] ordenDeTronco, bool conAnclas = true)
        {
            _raiz = new GameObject("Rig", typeof(RectTransform), typeof(CharacterRig));
            var lienzo = Nuevo("Lienzo", _raiz.transform);
            var cuerpo = Nuevo("Cuerpo", lienzo);
            var tronco = Nuevo("Tronco", cuerpo);
            var m = new Maniqui { Rig = _raiz.GetComponent<CharacterRig>(), Lienzo = lienzo, Tronco = tronco };

            foreach (var nombre in ordenDeTronco)
            {
                if (nombre.StartsWith("Brazo"))
                {
                    var lado = nombre.Substring("Brazo".Length);
                    var brazo = Nuevo(nombre, tronco);
                    var codo = Nuevo("Codo" + lado, brazo);
                    if (conAnclas)
                    {
                        Nuevo(LimbFollower.AnchorPrefix + "Antebrazo" + lado, codo);
                    }
                    else
                    {
                        Nuevo("Antebrazo" + lado, codo);
                    }
                }
                else if (nombre.StartsWith("Antebrazo"))
                {
                    if (conAnclas)
                    {
                        Nuevo(nombre, tronco).localPosition = new Vector3(999f, 999f, 0f);
                    }
                }
                else
                {
                    Nuevo(nombre, tronco);
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

        private static void Poner(Transform nodo, float x, float y, float grados, float escala = 1f)
        {
            nodo.localPosition = new Vector3(x, y, 0f);
            nodo.localRotation = Quaternion.Euler(0f, 0f, grados);
            nodo.localScale = new Vector3(escala, escala, 1f);
        }

        private static void AssertMismaPose(Transform seguidor, Transform ancla, string mensaje)
        {
            Assert.That(Vector3.Distance(seguidor.position, ancla.position), Is.LessThan(PosicionTolerancia), $"{mensaje}: posición del mundo");
            Assert.That(DiferenciaDeGiro(seguidor.rotation, ancla.rotation), Is.LessThan(GiroTolerancia), $"{mensaje}: giro del mundo");
            Assert.That(Vector3.Distance(seguidor.lossyScale, ancla.lossyScale), Is.LessThan(EscalaTolerancia), $"{mensaje}: escala del mundo (lossyScale)");
        }

        /// <summary>Cuánto se separan dos giros, medido con el desvío de los ejes X e Y que giran (Quaternion.Angle redondea a 0 los giros pequeños).</summary>
        private static float DiferenciaDeGiro(Quaternion a, Quaternion b)
        {
            return Mathf.Max(Vector3.Distance(a * Vector3.right, b * Vector3.right), Vector3.Distance(a * Vector3.up, b * Vector3.up));
        }

        private static int Indice(Transform tronco, string hijo)
        {
            var nodo = tronco.Find(hijo);
            Assert.That(nodo, Is.Not.Null, $"Tronco tiene {hijo}");
            return nodo.GetSiblingIndex();
        }

        private static string OrdenDeDibujo(Transform tronco)
        {
            var nombres = new string[tronco.childCount];
            for (var i = 0; i < nombres.Length; i++)
            {
                nombres[i] = tronco.GetChild(i).name;
            }

            return string.Join(", ", nombres);
        }

        /// <summary>Llama a un método privado de ciclo de vida: en EditMode Unity no los dispara solo.</summary>
        private static void Invocar(CharacterRig rig, string metodo)
        {
            var info = typeof(CharacterRig).GetMethod(metodo, BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(info, Is.Not.Null, $"CharacterRig tiene {metodo}");
            info.Invoke(rig, null);
        }

        private sealed class Maniqui
        {
            public CharacterRig Rig;
            public Transform Lienzo;
            public Transform Tronco;

            public Transform Brazo(string lado) => Tronco.Find("Brazo" + lado);

            public Transform Codo(string lado) => Tronco.Find("Brazo" + lado + "/Codo" + lado);

            public Transform Ancla(string lado) => Tronco.Find("Brazo" + lado + "/Codo" + lado + "/" + LimbFollower.AnchorPrefix + "Antebrazo" + lado);

            /// <summary>El antebrazo donde esté: suelto en Tronco (INC-133) o, sin anclas, colgado del codo.</summary>
            public Transform Antebrazo(string lado)
            {
                var suelto = Tronco.Find("Antebrazo" + lado);
                return suelto != null ? suelto : Tronco.Find("Brazo" + lado + "/Codo" + lado + "/Antebrazo" + lado);
            }
        }
    }
}
