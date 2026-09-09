using Game.Core;
using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// Comprueba que una pantalla tenga lo que necesita para navegar antes de atender un clic.
    /// </summary>
    /// <remarks>
    /// Los controladores de pantalla son adaptadores: no construyen el flujo ni la sesión, los
    /// reciben. En el ejecutable los crea la escena <c>Boot</c> y sobreviven al cambio de escena,
    /// así que nunca faltan; al pulsar Play sobre una escena suelta durante el desarrollo, en
    /// cambio, no existen. Sin esta comprobación cada manejador desreferenciaba el singleton nulo
    /// y el clic lanzaba <c>NullReferenceException</c> — las cuatro escenas de la Fase 1 en el
    /// <c>Editor.log</c> del 07/09/2026.
    ///
    /// Un clic no puede lanzar: <see cref="GameFlow"/> ya se compromete a que una transición
    /// ilegal devuelva <c>false</c> y deje el estado como estaba (RNF-13). Aquí se sostiene esa
    /// misma promesa un nivel más arriba — sin flujo el clic no navega, y el aviso dice por qué
    /// en vez de dejar un botón mudo.
    /// </remarks>
    internal static class ScreenFlow
    {
        internal static bool Ready(GameFlowRunner runner, Component screen) =>
            Check(runner != null, screen, nameof(GameFlowRunner));

        internal static bool Ready(ProfileSession session, Component screen) =>
            Check(session != null, screen, nameof(ProfileSession));

        private static bool Check(bool available, Component screen, string dependency)
        {
            if (available)
            {
                return true;
            }

            Debug.LogWarning(
                $"«{screen.gameObject.scene.name}» se abrió sin pasar por «Boot»: no hay " +
                $"{dependency}, así que el clic no navega. Para recorrer el juego, arranca en la " +
                "escena Boot.", screen);
            return false;
        }
    }
}
