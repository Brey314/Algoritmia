using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Audio
{
    /// <summary>
    /// El gestor de audio: el tercero de los tres únicos objetos con <c>DontDestroyOnLoad</c> del
    /// proyecto, junto a <c>GameFlowRunner</c> y <c>SceneLoader</c> (SPEC §Arquitectura). Cuatro
    /// buses bajo el maestro —Música, Ambiente, SFX y Voz— tal como los fija
    /// <c>Direccion_de_Musica_y_Sonido.md</c> §3; el de ambiente lleva dos capas, porque la
    /// hoguera suena **sobre** la cueva y no en su lugar.
    /// </summary>
    /// <remarks>
    /// No sabe qué suena en cada escena: eso es contenido (CT-05) y vive en los assets — la
    /// secuencia narrativa declara su ambiente y cada línea su sonido, y el nivel lleva sus piezas
    /// en un ScriptableObject propio. Aquí solo hay mezcla: las transiciones cruzadas de §3.3
    /// (800 ms de salida y 1200 ms de entrada, solapadas), los cortes secos de §5 y las
    /// ganancias relativas de §3.1.
    ///
    /// **Por qué no <c>AudioMixer</c>:** cuatro ganancias y dos fundidos no justifican un asset
    /// de mezcla con sus snapshots; con unas pocas <c>AudioSource</c> la ganancia de cada bus es
    /// un campo del Inspector y el gestor se prueba con un <c>AddComponent</c>.
    ///
    /// «Por qué no» pedagógico: no hay ningún método para un sonido de fallo, y no debe haberlo.
    /// Un intento sin éxito suena descriptivo —la piedra, la chispa que se apaga—, nunca
    /// punitivo (§2.1, CP-02). Quien añada aquí un «PlayError» reintroduce el pitido de derrota
    /// que el proyecto prohíbe.
    ///
    /// **Lleva el único <see cref="AudioListener"/> del juego.** Las escenas de flujo no tienen
    /// cámara —todo es uGUI en overlay— y sin oyente Unity no reproduce nada («There are no audio
    /// listeners in the scene»). Como este objeto sobrevive a todos los cambios de escena, el
    /// oyente va aquí y en ninguna cámara: dos oyentes vivos son la otra advertencia.
    /// </remarks>
    [RequireComponent(typeof(AudioListener))]
    public class AudioManager : MonoBehaviour
    {
        /// <summary>Segundos de salida de lo que suena en todo cambio de música o ambiente (§3.3).</summary>
        private const float FadeOutSeconds = 0.8f;

        /// <summary>Segundos de entrada de lo nuevo; se solapa con la salida (§3.3).</summary>
        private const float FadeInSeconds = 1.2f;

        public static AudioManager Instance { get; private set; }

        [field: SerializeField]
        [field: Range(0f, 1f)]
        [field: Tooltip("Ganancia del bus de música (§3.1: −20 LUFS, unos 2 dB bajo el maestro).")]
        public float MusicVolume { get; set; } = 0.8f;

        [field: SerializeField]
        [field: Range(0f, 1f)]
        [field: Tooltip("Ganancia del bus de ambiente, en sus dos capas (§3.1: −26 LUFS, fondo que se olvida a los diez segundos).")]
        public float AmbientVolume { get; set; } = 0.4f;

        [field: SerializeField]
        [field: Range(0f, 1f)]
        [field: Tooltip("Ganancia del bus de efectos: cada pieza viene normalizada a su familia (§3.1).")]
        public float SfxVolume { get; set; } = 1f;

        [field: SerializeField]
        [field: Range(0f, 1f)]
        [field: Tooltip("Ganancia del bus de voz: el blip del diálogo y la presencia de Algoritm, discretos (§3.1).")]
        public float VoiceVolume { get; set; } = 0.6f;

        private Bus _music;
        private Bus _ambient;
        private Bus _ambientLayer;
        private AudioSource _sfx;
        private AudioSource _held;
        private AudioSource _voice;

        /// <summary>El último efecto disparado. Las pruebas comprueban con él **qué** sonó, no cómo.</summary>
        internal AudioClip LastSfx { get; private set; }

        /// <summary>Cuántos efectos se han disparado: cuenta golpes repetidos del mismo clip, que <see cref="LastSfx"/> no distingue.</summary>
        internal int SfxCount { get; private set; }

        /// <summary>El efecto de clic sostenido que suena ahora mismo; nulo si ninguno.</summary>
        internal AudioClip HeldClip { get; private set; }

        /// <summary>El ambiente que suena o está entrando; nulo si ninguno o si se está yendo.</summary>
        internal AudioClip AmbientClip => _ambient.Clip;

        /// <summary>La capa superpuesta al ambiente (la hoguera sobre la cueva); nulo si ninguna.</summary>
        internal AudioClip AmbientLayerClip => _ambientLayer.Clip;

        /// <summary>Cuántas veces arrancó un ambiente desde cero: pedir el que ya suena no cuenta.</summary>
        internal int AmbientStarts => _ambient.Starts;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                // RNF-16: volver a Boot no puede dejar dos gestores vivos.
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            _music = new Bus(gameObject);
            _ambient = new Bus(gameObject);
            _ambientLayer = new Bus(gameObject);
            _sfx = Channel.NewSource(gameObject, loop: false);
            _held = Channel.NewSource(gameObject, loop: true);
            _voice = Channel.NewSource(gameObject, loop: false);
            // Las fuentes de fondo nacen a cero y el fundido las sube; las de un disparo no tienen
            // fundido y `PlayOneShot` multiplica por el volumen de la fuente: a cero, mudas.
            _sfx.volume = 1f;
            _voice.volume = 1f;
            SceneManager.sceneLoaded += SceneManager_SceneLoaded;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                SceneManager.sceneLoaded -= SceneManager_SceneLoaded;
            }
        }

        private void Update()
        {
            // Tiempo sin escalar: la pausa pone timeScale a cero y un fundido a medias se quedaría colgado.
            var dt = Time.unscaledDeltaTime;
            _music.Tick(dt, MusicVolume);
            _ambient.Tick(dt, AmbientVolume);
            _ambientLayer.Tick(dt, AmbientVolume);
        }

        /// <summary>
        /// Cambia el ambiente con fundido cruzado. Nulo: funde a salida el que suene. **Pedir el
        /// clip que ya suena no lo reinicia**: así dos escenas narrativas seguidas —o la última y
        /// el nivel— comparten su ambiente sin costura.
        /// </summary>
        public void PlayAmbient(AudioClip clip, float fadeInSeconds = FadeInSeconds) =>
            _ambient.Play(clip, fadeInSeconds, FadeOutSeconds);

        /// <summary>
        /// Una segunda capa **sobre** el ambiente —la hoguera sobre la cueva a oscuras (RF-20)—,
        /// con las mismas reglas que <see cref="PlayAmbient"/>. Nulo: se va la capa y queda el ambiente.
        /// </summary>
        public void PlayAmbientLayer(AudioClip clip, float fadeInSeconds = FadeInSeconds) =>
            _ambientLayer.Play(clip, fadeInSeconds, FadeOutSeconds);

        /// <summary>Cambia la música con fundido cruzado; mismas reglas que <see cref="PlayAmbient"/>.</summary>
        public void PlayMusic(AudioClip clip, float fadeInSeconds = FadeInSeconds) =>
            _music.Play(clip, fadeInSeconds, FadeOutSeconds);

        /// <summary>
        /// Un efecto, una vez. <paramref name="pitchJitter"/> varía el tono ± esa fracción para que
        /// un mismo golpe no suene idéntico dos veces seguidas cuando solo hay una toma (§14.6);
        /// <paramref name="volume"/> lo escala (el golpe suena más fuerte cuanto más se enciman las piedras).
        /// </summary>
        public void PlaySfx(AudioClip clip, float pitchJitter = 0f, float volume = 1f)
        {
            if (clip == null)
            {
                return;
            }

            _sfx.pitch = 1f + Random.Range(-pitchJitter, pitchJitter);
            _sfx.PlayOneShot(clip, SfxVolume * Mathf.Clamp01(volume));
            LastSfx = clip;
            SfxCount++;
        }

        /// <summary>
        /// Un efecto de **clic sostenido** (RNF-02): suena en bucle desde que se toma hasta que se
        /// suelta —arrastrar una hoja, mantener una flecha—. Pedir otro mientras dura lo sustituye.
        /// </summary>
        public void PlayHeld(AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }

            _held.clip = clip;
            _held.volume = SfxVolume;
            _held.Play();
            HeldClip = clip;
        }

        /// <summary>Se soltó el clic sostenido: el efecto en bucle se calla.</summary>
        public void StopHeld()
        {
            _held.Stop();
            HeldClip = null;
        }

        /// <summary>La voz del guía o el blip del diálogo, una vez.</summary>
        public void PlayVoice(AudioClip clip)
        {
            if (clip != null)
            {
                _voice.PlayOneShot(clip, VoiceVolume);
            }
        }

        /// <summary>
        /// Uno de los silencios del guion (§5): corte **seco**, sin el fundido de §3.3. Con
        /// <paramref name="keepAmbient"/> calla solo música y voz y queda el ambiente —S2, «la
        /// oscuridad vuelve de golpe» y sigue la cueva—; sin él calla todo salvo el efecto que ya
        /// suena, que termina solo —S1, «el último eco de los pasos»—.
        /// </summary>
        public void CutToSilence(bool keepAmbient = false)
        {
            _music.Cut();
            _voice.Stop();
            if (!keepAmbient)
            {
                _ambient.Cut();
                _ambientLayer.Cut();
            }
        }

        /// <summary>
        /// Todo cambio de escena funde a salida lo que suena (§3.3) y suelta el clic sostenido. La
        /// escena nueva vuelve a pedir su ambiente en su <c>Start</c>, un cuadro después: si es el
        /// mismo clip, el fundido se cancela sin que se note. Así ninguna escena tiene que saber
        /// callar lo de la anterior.
        /// </summary>
        private void SceneManager_SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _music.Play(null, FadeInSeconds, FadeOutSeconds);
            _ambient.Play(null, FadeInSeconds, FadeOutSeconds);
            _ambientLayer.Play(null, FadeInSeconds, FadeOutSeconds);
            StopHeld();
        }

        /// <summary>Un bus de fondo con dos fuentes: la que entra y la que sale se solapan (§3.3).</summary>
        private sealed class Bus
        {
            private readonly Channel _a;
            private readonly Channel _b;
            private Channel _front;

            public Bus(GameObject host)
            {
                _a = new Channel(host);
                _b = new Channel(host);
                _front = _a;
            }

            public AudioClip Clip => _front.Target > 0f ? _front.Source.clip : null;

            public int Starts { get; private set; }

            public void Play(AudioClip clip, float fadeIn, float fadeOut)
            {
                if (clip == null)
                {
                    _front.FadeTo(0f, fadeOut);
                    return;
                }

                if (_front.Source.clip == clip && _front.Active)
                {
                    _front.FadeTo(1f, fadeIn); // ya suena: se queda, y si se estaba yendo, vuelve
                    return;
                }

                _front.FadeTo(0f, fadeOut);
                _front = _front == _a ? _b : _a;
                _front.Start(clip, fadeIn);
                Starts++;
            }

            public void Cut()
            {
                _a.Stop();
                _b.Stop();
            }

            public void Tick(float dt, float gain)
            {
                _a.Tick(dt, gain);
                _b.Tick(dt, gain);
            }
        }

        /// <summary>Una fuente con su fundido: nivel 0..1 que camina hacia un objetivo, por la ganancia del bus.</summary>
        private sealed class Channel
        {
            public readonly AudioSource Source;
            private float _level;
            private float _seconds;

            public Channel(GameObject host) => Source = NewSource(host, loop: true);

            public float Target { get; private set; }

            // Estado propio y no `Source.isPlaying`: sin dispositivo de audio (batchmode) la fuente
            // nunca dice que suena, y el bus reiniciaría el ambiente que se le pide dos veces.
            public bool Active { get; private set; }

            public static AudioSource NewSource(GameObject host, bool loop)
            {
                var source = host.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.loop = loop;
                source.spatialBlend = 0f; // 2D: no hay espacialización que valga el doble de peso (§4.2)
                source.volume = 0f;
                return source;
            }

            public void Start(AudioClip clip, float fadeIn)
            {
                Source.clip = clip;
                _level = 0f;
                Source.volume = 0f;
                Source.Play();
                Active = true;
                FadeTo(1f, fadeIn);
            }

            public void FadeTo(float target, float seconds)
            {
                Target = target;
                _seconds = seconds;
            }

            public void Stop()
            {
                Source.Stop();
                Active = false;
                _level = 0f;
                Target = 0f;
                Source.volume = 0f;
            }

            public void Tick(float dt, float gain)
            {
                _level = Mathf.MoveTowards(_level, Target, _seconds > 0f ? dt / _seconds : 1f);
                Source.volume = _level * gain;
                if (Active && _level <= 0f && Target <= 0f)
                {
                    Stop();
                }
            }
        }
    }
}
