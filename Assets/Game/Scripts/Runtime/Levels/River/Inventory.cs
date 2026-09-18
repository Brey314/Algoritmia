using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Levels.River
{
    /// <summary>
    /// El inventario de la fase de recolección: capacidad exactamente igual al catálogo —cuatro
    /// materiales, ni uno sobrante— y sin gestión de espacio (RF-38, guion §1.8.2).
    /// </summary>
    /// <remarks>
    /// C# plano: se prueba en EditMode sin escena. Recoger **no puede fallar** (CP-02): un
    /// material ya recogido no dice nada, y con todo recogido el sistema informa que ya cuenta
    /// con lo necesario (CU-09 FA-4a) — nunca rechaza con reproche.
    /// </remarks>
    public class Inventory
    {
        private readonly RiverLevelConfig _config;
        private readonly List<Collectible> _items = new List<Collectible>();

        public Inventory(RiverLevelConfig config)
        {
            _config = config != null ? config : throw new ArgumentNullException(nameof(config));
        }

        /// <summary>Cuántos caben: exactamente los del catálogo (RF-38).</summary>
        public int Capacity => _config.Collectibles.Length;

        public IReadOnlyList<Collectible> Items => _items;

        public bool IsFull => _items.Count >= Capacity;

        public bool Has(MaterialKind kind) => _items.Any(item => item.Kind == kind);

        /// <summary>Lo que falta del catálogo, en su orden: es lo que se nombra en la zona de construcción.</summary>
        public IEnumerable<Collectible> Missing => _config.Collectibles.Where(item => !Has(item.Kind));

        /// <summary>Mete el material al inventario y devuelve lo que hay que decir.</summary>
        public Outcome TryCollect(Collectible collectible)
        {
            if (collectible == null || _items.Any(item => item.Id == collectible.Id))
            {
                return new Outcome(false, string.Empty); // Ya estaba: no hubo intento que describir.
            }

            if (IsFull)
            {
                return new Outcome(false, _config.AllCollectedMessage);
            }

            _items.Add(collectible);
            return new Outcome(true, IsFull
                ? _config.AllCollectedMessage
                : string.Format(_config.CollectedFormat, collectible.DisplayName));
        }
    }
}
