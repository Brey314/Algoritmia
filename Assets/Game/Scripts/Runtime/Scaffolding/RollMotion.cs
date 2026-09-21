using UnityEngine;

namespace Game.Scaffolding
{
    /// <summary>Un instante del rodado: cuánto lleva recorrido, cuánto ha caído, cuánto gira y cuánto se ladea.</summary>
    public readonly struct RollSample
    {
        /// <summary>Avance sobre los troncos, de 0 a 1, ya suavizado.</summary>
        public float Along { get; }

        /// <summary>Avance de la caída al pasar el último tronco, de 0 a 1, lineal en el tiempo.</summary>
        public float Fall { get; }

        /// <summary>Cuánto ha bajado, de 0 a 1: acelera, como cae lo que pesa.</summary>
        public float Drop { get; }

        /// <summary>Giro de los troncos bajo la caja, en grados.</summary>
        public float Spin { get; }

        /// <summary>Ladeo de la caja al caer, en grados.</summary>
        public float Tilt { get; }

        public RollSample(float along, float fall, float drop, float spin, float tilt)
        {
            Along = along;
            Fall = fall;
            Drop = drop;
            Spin = spin;
            Tilt = tilt;
        }
    }

    /// <summary>
    /// **El** rodado de la caja: el mismo en la mecánica del bosque y en la escena narrativa que
    /// la cuenta (RF-26, guion §6.1.3). Recorre los troncos suavizado mientras giran bajo ella y,
    /// pasado el último, cae al suelo acelerando y un poco ladeada.
    /// </summary>
    /// <remarks>
    /// C# plano y sin coordenadas: devuelve fracciones y grados, y cada escena los traduce a su
    /// espacio. Así la caja rueda igual jugando que en la narrativa, y cambiar el tacto es tocar
    /// un solo sitio. Es interpolación continua: nada parpadea (RNF-21).
    /// </remarks>
    public static class RollMotion
    {
        /// <summary>Vueltas que da cada tronco mientras la caja recorre la fila entera.</summary>
        public const float Turns = 1.5f;

        /// <summary>Cuánto se ladea la caja al caer, en grados.</summary>
        public const float TiltDegrees = -18f;

        /// <summary>
        /// El instante <paramref name="t"/> (0..1 sobre rodado y caída juntos) cuando el rodado
        /// ocupa <paramref name="rollShare"/> del tiempo total y la caída el resto.
        /// </summary>
        public static RollSample Evaluate(float t, float rollShare)
        {
            t = Mathf.Clamp01(t);
            rollShare = Mathf.Clamp01(rollShare);

            var roll = rollShare > 0f ? Mathf.Clamp01(t / rollShare) : 1f;
            var fall = rollShare < 1f ? Mathf.Clamp01((t - rollShare) / (1f - rollShare)) : 0f;
            var along = Mathf.SmoothStep(0f, 1f, roll);

            return new RollSample(along, fall, fall * fall, -360f * Turns * along,
                TiltDegrees * Mathf.SmoothStep(0f, 1f, fall));
        }
    }
}
