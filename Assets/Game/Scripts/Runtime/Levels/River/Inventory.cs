using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Levels.River
{
    /// <summary>
    /// El inventario de la fase de recolección: una casilla por clase de material —troncos,
    /// sogas, tela y mástil—, sin gestión de espacio ni sobrantes (RF-38, guion §1.8.2). Los
    /// cinco troncos comparten casilla y la llenan de a uno (decisión de Santiago del
    /// 20/09/2026).
    /// </summary>
    /// <remarks>
    /// C# plano: se prueba en EditMode sin escena. Recoger **no puede fallar** (CP-02): un
    /// material ya recogido no dice nada, y con todo recogido el sistema informa que ya cuenta
    /// con lo necesario (CU-09 FA-4a) — nunca rechaza con reproche. Una clase está «recogida»
    /// cuando están **todos** los del catálogo: el primer tronco no marca la tarea 1, el quinto sí.
    /// </remarks>
    public class Inventory
    {
        private readonly RiverLevelConfig _config;
        private readonly List<Collectible> _items = new List<Collectible>();

        public Inventory(RiverLevelConfig config)
        {
            _config = config != null ? config : throw new ArgumentNullException(nameof(config));
        }

        /// <summary>Las clases del catálogo en su orden: una casilla por clase (RF-38).</summary>
        public IReadOnlyList<MaterialKind> Kinds => _config.Collectibles.Select(item => item.Kind).Distinct().ToArray();

        /// <summary>Cuántas casillas hay: exactamente las clases del catálogo (RF-38).</summary>
        public int Capacity => Kinds.Count;

        public IReadOnlyList<Collectible> Items => _items;

        public bool IsFull => _items.Count >= _config.Collectibles.Length;

        /// <summary>Cuántos de esa clase hay que recoger: cinco troncos, un rollo de soga.</summary>
        public int Required(MaterialKind kind) => _config.Collectibles.Count(item => item.Kind == kind);

        public int Count(MaterialKind kind) => _items.Count(item => item.Kind == kind);

        /// <summary>Si esa clase está completa: todos los troncos, no el primero.</summary>
        public bool Has(MaterialKind kind) => Required(kind) > 0 && Count(kind) >= Required(kind);

        /// <summary>Una entrada por clase incompleta, en el orden del catálogo: es lo que se nombra en la zona de construcción.</summary>
        public IEnumerable<Collectible> Missing =>
            Kinds.Where(kind => !Has(kind)).Select(kind => _config.Collectibles.First(item => item.Kind == kind));

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
