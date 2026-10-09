using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scaffolding
{
    /// <summary>
    /// Un personaje animado por recorte: partes (torso, brazos, piernas) que un Animator gira y
    /// desplaza, con un estado por <see cref="ActorAction"/> (Dirección de arte §13.3). Lo usan por
    /// igual la escena narrativa y las cinco mecánicas.
    /// </summary>
    /// <remarks>
    /// **Por qué recorte con <c>Image</c> y no el rig de 2D Animation (§13.1):** todas las escenas son
    /// uGUI en un Canvas overlay, y <c>SpriteSkin</c> solo deforma un <c>SpriteRenderer</c>, que
    /// quedaría debajo del Canvas, tapado por la ilustración. Las partes son <c>Image</c> hijas, así
    /// que el personaje cuelga de la ilustración y acompaña el paneo y el zoom como cualquier objeto.
    ///
    /// Los clips trabajan en unidades del lienzo de los sprites base (1024 × 1024): el lienzo
    /// (<see cref="Stage"/>) se escala para llenar la casilla del personaje, y un balanceo mide lo
    /// mismo en un niño lejano que en Papá de cerca. La escala del lienzo la pone este componente y
    /// ningún clip la anima.
    ///
    /// **Dos cuerpos, un solo personaje (Santiago, 09/10/2026, INC-134).** La familia lleva, junto al
    /// cuerpo de frente (<c>Lienzo/Cuerpo</c>), un segundo cuerpo dibujado de perfil
    /// (<c>Lienzo/Perfil</c>). Cuál se ve lo decide la acción (<see cref="ActionView"/>): de perfil al
    /// recorrer o trabajar el entorno, de frente en todo lo demás; el corte es seco. Los dos cuerpos
    /// los anima el mismo Animator, con las mismas acciones. El perfil está dibujado mirando a la
    /// derecha y para mirar a la izquierda se voltea el lienzo (<see cref="Mirrored"/>), **solo en
    /// perfil**: de frente nunca se espeja por el rumbo. Un rig sin cuerpo de perfil utilizable
    /// (Algoritm, o un personaje cuyo arte de perfil todavía no llegó) ignora todo esto y se queda de
    /// frente (<see cref="HasProfile"/>).
    /// </remarks>
    [RequireComponent(typeof(RectTransform))]
    public class CharacterRig : MonoBehaviour
    {
        /// <summary>Lado del lienzo de los sprites base, en píxeles: la unidad de los clips.</summary>
        public const float CanvasUnits = 1024f;

        /// <summary>Segundos del fundido entre dos acciones: sin saltos de pose (RNF-21).</summary>
        private const float BlendSeconds = 0.18f;

        /// <summary>Dónde está Tronco, el padre de torso y brazos, contando desde el lienzo (<see cref="Stage"/>).</summary>
        private const string TrunkPath = "Cuerpo/Tronco";

        /// <summary>El cuerpo de frente, contando desde el lienzo. Se busca por ruta, como <see cref="TrunkPath"/>: el prefab no lo serializa.</summary>
        private const string FrontBodyPath = "Cuerpo";

        /// <summary>El cuerpo de perfil, contando desde el lienzo (INC-134).</summary>
        private const string ProfileBodyPath = "Perfil";

        /// <summary>
        /// El torso del perfil, contando desde <see cref="ProfileBodyPath"/>. Su sprite es lo que dice que el
        /// arte de perfil llegó: un prefab con los nodos de perfil pero sin dibujo seguiría caminando de
        /// frente, y no se volvería invisible a media escena.
        /// </summary>
        private const string ProfileTorsoPath = "Tronco/Torso";

        private static readonly int IdleState = Animator.StringToHash(nameof(ActorAction.Idle));

        [field: SerializeField]
        [field: Tooltip("Nombres con los que el guion lo hace hablar, tal como van en las líneas: «PAPÁ», y también «NIÑOS» en los dos niños.")]
        public string[] SpeakerNames { get; private set; } = Array.Empty<string>();

        [field: SerializeField]
        [field: Tooltip("Retrato del cuadro de diálogo cuando habla (char_*_retrato_neutra). En Algoritm es el sprite de su forma, así que sustituir el archivo cambia los dos.")]
        public Sprite Portrait { get; private set; }

        [SerializeField]
        [Tooltip("El Animator con un estado por acción (Idle, Walk, Strike…). Los clips animan las partes, nunca el lienzo.")]
        private Animator animator;

        [SerializeField]
        [Tooltip("El lienzo de 1024 × 1024 que contiene las partes. Se escala para llenar la casilla del personaje.")]
        private RectTransform stage;

        // Decisión de Santiago (05/10/2026, INC-132): en la familia los brazos se dibujan detrás del torso, y
        // al golpear las piedras las manos chocan delante del pecho: con los brazos detrás el choque no se
        // ve. Mientras dure una de estas acciones los brazos pasan delante del torso (ArmLayering); al pasar
        // a otra acción vuelven a su sitio. El valor por defecto vale para los siete prefabs sin editarlos:
        // si el campo no está serializado en el prefab, Unity conserva el del inicializador. Ojo: en cuanto
        // un prefab se vuelve a guardar desde el Editor (Inspector, el generador) el campo queda escrito con
        // el valor de ese momento, y un cambio posterior del inicializador ya no lo alcanza. Algoritm no se
        // ve afectado: sus brazos ya van delante del cuerpo.
        [SerializeField]
        [Tooltip("Acciones durante las que los brazos se dibujan DELANTE del torso, para que se vea el choque de las manos delante del pecho. Al terminarlas vuelven a su sitio. Vacío = nunca. En Algoritm no cambia nada: sus brazos ya van delante del cuerpo.")]
        private ActorAction[] armsInFrontActions = { ActorAction.Strike };

        private Vector2 _fittedSize = new Vector2(-1f, -1f);
        private bool _mirrored;
        private CharacterView _view = CharacterView.Front;
        private RectTransform _bodiesFor;
        private Transform _frontBody;
        private Transform _profileBody;
        private Image _profileTorso;
        private bool _started;
        private CancellationTokenSource _returning;
        private CharacterFace _face;
        private ArmLayering _armLayering;
        private LimbFollower[] _limbs;
        private FacialEmotion? _emotionOverride;
        private bool _speaking;

        /// <summary>La última acción pedida.</summary>
        public ActorAction Current { get; private set; } = ActorAction.Idle;

        /// <summary>
        /// La emoción que el guion fija para este personaje (<see cref="ActorBeat.SetsEmotion"/>);
        /// <c>null</c> = la que le corresponde a su acción (<see cref="ActionEmotion"/>). Asignarla
        /// refresca la cara al instante.
        /// </summary>
        public FacialEmotion? EmotionOverride
        {
            get => _emotionOverride;
            set
            {
                _emotionOverride = value;
                PushToFace();
            }
        }

        /// <summary>La emoción vigente: la que fija el guion o, si no, la de la acción en curso.</summary>
        public FacialEmotion Emotion => EmotionOverride ?? ActionEmotion.For(Current);

        /// <summary>
        /// Si el personaje está diciendo su línea: la boca aletea hasta que se apaga. La pone quien
        /// conduce la escena (<c>NarrativeSceneController</c>), no el rig, que no sabe qué línea se lee.
        /// Sin <see cref="CharacterFace"/> —o sin sprites de cara— no se ve nada, pero el valor se guarda.
        /// </summary>
        public bool Speaking
        {
            get => _speaking;
            set
            {
                _speaking = value;
                PushToFace();
            }
        }

        /// <summary>El lienzo que contiene las partes.</summary>
        public RectTransform Stage => stage;

        /// <summary>
        /// Si el personaje tiene un cuerpo de perfil utilizable (INC-134): existen <c>Lienzo/Cuerpo</c> y
        /// <c>Lienzo/Perfil</c> y el torso del perfil (<c>Perfil/Tronco/Torso</c>) tiene su sprite. Sin
        /// eso el personaje se queda de frente en cualquier acción. Se evalúa al pedirlo, no se cachea:
        /// el arte de perfil puede llegar después de crear el rig.
        /// </summary>
        public bool HasProfile
        {
            get
            {
                ResolveBodies();
                return _frontBody != null && _profileBody != null && _profileTorso != null && _profileTorso.sprite != null;
            }
        }

        /// <summary>
        /// Desde dónde se ve ahora al personaje (INC-134). Es de frente salvo que tenga perfil
        /// (<see cref="HasProfile"/>) y su acción en curso sea de perfil (<see cref="ActionView"/>).
        /// </summary>
        public CharacterView View => _view;

        /// <summary>
        /// Si mira a la izquierda: hacia dónde da la cara cuando se ve de perfil. Voltea el lienzo y no
        /// la raíz: en el río la escala de la raíz la pone la profundidad (<c>RiverSceneController</c>) y
        /// la vigila una prueba. **Solo se aplica en perfil** (INC-134): de frente el personaje se dibuja
        /// siempre igual y el valor solo se recuerda, para que al pasar a perfil mire al lado que tocaba.
        /// </summary>
        public bool Mirrored
        {
            get => _mirrored;
            set
            {
                _mirrored = value;
                Fit(true);
            }
        }

        private void Awake()
        {
            ShowView(Current);
            Fit(true);
        }

        private void OnEnable()
        {
            ShowView(Current); // un prefab guardado con los dos cuerpos encendidos no los enseña a la vez
            Fit(true);
            PushToFace();
            if (animator != null && animator.runtimeAnimatorController != null && animator.isActiveAndEnabled)
            {
                Apply(Current, immediate: true);
            }

            // Tras Play + Update(0): sin esto el primer cuadro enseñaría el antebrazo donde lo dejó el prefab.
            SyncLimbs();
        }

        private void Start()
        {
            if (!_started && animator != null && animator.runtimeAnimatorController != null && animator.isActiveAndEnabled)
            {
                Apply(Current, immediate: true);
            }

            SyncLimbs();
        }

        private void LateUpdate()
        {
            Fit(false);
            SyncLimbs(); // el Animator ya movió brazos y codos: LateUpdate corre después de él
        }

        /// <summary>
        /// Pone cada antebrazo en la pose de su ancla (<see cref="LimbFollower"/>). En la familia el
        /// antebrazo cuelga de Tronco para dibujarse delante del torso aunque el húmero vaya detrás
        /// (INC-133); el ancla, que sí cuelga del codo, es la que anima el Animator. Los pares se buscan
        /// por nombre la primera vez: un personaje sin anclas (arte provisional, Algoritm) no tiene ninguno
        /// y esto no hace nada. Sin lienzo no hay dónde buscar y se reintenta en el cuadro siguiente.
        /// </summary>
        internal void SyncLimbs()
        {
            if (_limbs == null)
            {
                if (stage == null)
                {
                    return;
                }

                _limbs = LimbFollower.Discover(stage.Find(TrunkPath));
            }

            foreach (var limb in _limbs)
            {
                limb.Sync();
            }
        }

        /// <summary>
        /// Pasa a la acción pedida con un fundido corto. Un rig sin ese estado hace
        /// <see cref="ActorAction.Idle"/>: la acción del guion nunca rompe la escena.
        /// </summary>
        public void Play(ActorAction action)
        {
            _returning?.Cancel();
            _returning = null;
            Apply(action);
        }

        /// <summary>
        /// Hace la acción durante unos segundos y vuelve sola a <paramref name="then"/>: el gesto
        /// de una mecánica (señalar lo elegido, el ánimo tras un intento) que no se queda puesto.
        /// El tiempo es escalado, así que la pausa lo congela (RF-07). Otra acción pedida antes
        /// lo cancela.
        /// </summary>
        public void PlayFor(ActorAction action, float seconds, ActorAction then = ActorAction.Idle)
        {
            Play(action);
            _returning = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            _ = ReturnAsync(seconds, then, _returning.Token);
        }

        private async Awaitable ReturnAsync(float seconds, ActorAction then, CancellationToken token)
        {
            try
            {
                await Awaitable.WaitForSecondsAsync(seconds, token);
            }
            catch (OperationCanceledException)
            {
                return; // otra acción ya ocupó su lugar
            }

            _returning = null;
            Apply(then);
        }

        private void Apply(ActorAction action, bool immediate = false)
        {
            // Pedir lo que ya hace no lo reinicia: una acción que se mantiene entre líneas —se
            // apagó, sigue arrodillado— no vuelve a empezar cada vez que avanza el texto.
            if (action == Current && _started && !immediate)
            {
                return;
            }

            Current = action;
            // La vista (frente o perfil) sigue a la acción como las capas y la cara: aunque no haya Animator.
            ShowView(action);
            // Las capas de los brazos y la cara siguen a la acción aunque no haya Animator que la ejecute.
            // El cambio de capa es seco, al empezar la acción, y no espera al fundido de BlendSeconds: al
            // entrar en Strike los brazos ya van delante mientras suben al pecho (cruzan delante del torso
            // de todos modos), y al salir de él vuelven detrás al empezar la acción siguiente, así que en
            // esos 0,18 s las manos pueden asomar un instante por detrás del torso camino del reposo. Se
            // acepta a cambio de no llevar un temporizador que cancelar con cada Play/PlayFor.
            LayerArms(action);
            PushToFace();
            if (animator == null || animator.runtimeAnimatorController == null || !animator.isActiveAndEnabled)
            {
                return;
            }

            var state = Animator.StringToHash(action.ToString());
            if (!animator.HasState(0, state))
            {
                state = IdleState;
            }

            if (!_started || immediate)
            {
                _started = true;
                animator.Play(state, 0, 0f);
                animator.Update(0f);
            }
            else
            {
                animator.CrossFadeInFixedTime(state, BlendSeconds, 0);
            }
        }

        /// <summary>
        /// Pone los brazos delante del torso si <paramref name="action"/> está en
        /// <c>armsInFrontActions</c> y los devuelve a su sitio si no. Sin lienzo no hay cuerpo que
        /// ordenar; y mientras ninguna acción pida los brazos delante no hay nada que restaurar, así que
        /// el <see cref="ArmLayering"/> ni se crea.
        /// </summary>
        private void LayerArms(ActorAction action)
        {
            var inFront = armsInFrontActions != null && Array.IndexOf(armsInFrontActions, action) >= 0;
            if (_armLayering == null)
            {
                if (!inFront || stage == null)
                {
                    return;
                }

                // El orden «de origen» que recuerda es el de este momento: nadie ha tocado Tronco antes.
                _armLayering = new ArmLayering(stage.Find(TrunkPath));
            }

            _armLayering.Apply(inFront);
        }

        /// <summary>
        /// Enciende el cuerpo que toca a <paramref name="action"/> y apaga el otro (INC-134): corte seco, sin
        /// fundido, porque los dos cuerpos se animan a la vez y cada uno ya está en la pose de la acción. Sin
        /// perfil utilizable la vista es de frente: enciende el cuerpo de frente y apaga el de perfil, si existe.
        /// Al cambiar de vista se vuelve a ajustar el lienzo, que solo se voltea en perfil.
        /// </summary>
        private void ShowView(ActorAction action)
        {
            var hasProfile = HasProfile;
            var view = hasProfile ? ActionView.For(action) : CharacterView.Front;
            // Siempre, y no solo al cambiar: SetActive con el mismo valor no cuesta nada, y así el primer cuadro
            // apaga el cuerpo que el prefab haya dejado encendido. Sin perfil utilizable (Perfil existe pero su
            // torso no tiene sprite, o el arte se quitó mientras se veía de perfil) el de perfil se APAGA y el de
            // frente se ENCIENDE: si no, los dos se dibujarían a la vez o el personaje quedaría invisible.
            if (_frontBody != null)
            {
                _frontBody.gameObject.SetActive(view == CharacterView.Front);
            }

            if (_profileBody != null)
            {
                _profileBody.gameObject.SetActive(view == CharacterView.Profile);
            }

            if (view == _view)
            {
                return;
            }

            _view = view;
            Fit(true); // el espejo depende de la vista
        }

        /// <summary>
        /// Busca por ruta los dos cuerpos y el torso del perfil, como <see cref="SyncLimbs"/> con Tronco. Se
        /// da por hecho cuando ya están los tres; si falta alguno se reintenta en cada llamada (una búsqueda
        /// de un hijo por nombre es barata): así el arte que llega después, o un lienzo asignado tarde, se
        /// encuentra. Sin lienzo no hay dónde buscar.
        /// </summary>
        private void ResolveBodies()
        {
            if (_bodiesFor == stage && _frontBody != null && _profileBody != null && _profileTorso != null)
            {
                return;
            }

            _bodiesFor = stage;
            _frontBody = stage != null ? stage.Find(FrontBodyPath) : null;
            _profileBody = stage != null ? stage.Find(ProfileBodyPath) : null;
            var torso = _profileBody != null ? _profileBody.Find(ProfileTorsoPath) : null;
            _profileTorso = torso != null ? torso.GetComponent<Image>() : null;
        }

        /// <summary>Le empuja a la cara opcional la emoción vigente y si habla.</summary>
        private void PushToFace()
        {
            if (_face == null)
            {
                _face = GetComponent<CharacterFace>();
            }

            if (_face == null)
            {
                return; // sin cara (arte provisional): nada que refrescar
            }

            _face.Emotion = Emotion;
            _face.Speaking = _speaking;
        }

        /// <summary>Si la línea la dice este personaje. Sin distinguir mayúsculas: el guion las escribe en versales.</summary>
        public bool Speaks(string speaker)
        {
            if (string.IsNullOrWhiteSpace(speaker))
            {
                return false;
            }

            foreach (var name in SpeakerNames)
            {
                if (string.Equals(name?.Trim(), speaker.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Escala el lienzo para llenar la casilla, como <c>preserveAspect</c> con un sprite cuadrado.</summary>
        private void Fit(bool force)
        {
            if (stage == null)
            {
                return;
            }

            var size = ((RectTransform)transform).rect.size;
            if (!force && size == _fittedSize)
            {
                return;
            }

            _fittedSize = size;
            var scale = Mathf.Min(size.x, size.y) / CanvasUnits;
            // INC-134: el perfil está dibujado mirando a la derecha y se voltea para mirar a la izquierda; el
            // frente no se espeja nunca por el rumbo (Santiago, 09/10/2026), aunque _mirrored lo recuerde.
            var flip = _mirrored && _view == CharacterView.Profile;
            stage.localScale = new Vector3(flip ? -scale : scale, scale, 1f);
        }
    }
}
