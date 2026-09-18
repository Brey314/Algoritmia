using System;
using System.Linq;
using UnityEngine;

namespace Game.Levels.River
{
    /// <summary>
    /// La zona señalizada junto al río (RF-39). Con los cuatro materiales abre el ensamblaje; sin
    /// ellos dice **cuáles** faltan, por su nombre y no cuántos (CU-09 FA-6a, CP-03), y deja
    /// salir sin penalización (CP-02).
    /// </summary>
    /// <remarks>
    /// C# plano. Una vez abierta se queda abierta: entrar de nuevo no repite nada.
    /// **No confirma ninguna fase**: la recolección no se persiste (decisión de Santiago del
    /// 16/09/2026, <c>PhaseId.PhasesPerLevel</c>); lo que se guarda son las tres fases del
    /// ensamblaje, y eso lo hace el panel (R11).
    /// </remarks>
    public class BuildZone
    {
        private readonly RiverLevelConfig _config;

        public BuildZone(RiverLevelConfig config)
        {
            _config = config != null ? config : throw new ArgumentNullException(nameof(config));
        }

        public bool IsOpen { get; private set; }

        /// <summary>Si ese punto de la ilustración está dentro de la zona.</summary>
        public bool Contains(Vector2 position) =>
            Vector2.Distance(_config.BuildZonePosition, position) <= _config.BuildZoneRadius;

        /// <summary>Ingresar con lo que se lleva. Abre solo con el inventario completo.</summary>
        public Outcome TryEnter(Inventory inventory)
        {
            if (IsOpen)
            {
                return new Outcome(true, string.Empty);
            }

            var missing = inventory.Missing.Select(item => item.DisplayName).ToArray();
            if (missing.Length > 0)
            {
                return new Outcome(false, string.Format(_config.MissingFormat, Enumerate(missing)));
            }

            IsOpen = true;
            return new Outcome(true, _config.ZoneOpenedMessage);
        }

        /// <summary>«sogas», «sogas y tela», «sogas, tela y mástil»: nombres, nunca un número.</summary>
        private static string Enumerate(string[] names) => names.Length == 1
            ? names[0]
            : string.Join(", ", names.Take(names.Length - 1)) + " y " + names[^1];
    }
}
