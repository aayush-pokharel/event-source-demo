using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace event_sourcing_demo.console
{
    internal class WarehouseProductRepository
    {
        private readonly Dictionary<string, IList<IEvent>> _inMemoryStreams = new();

        internal WarehouseProduct Get(string sku)
        {
            var warehouseProduct = new WarehouseProduct(sku);
            if (_inMemoryStreams.ContainsKey(sku))
            {
                foreach (var evnt in _inMemoryStreams[sku])
                    warehouseProduct.AddEvent(evnt);
            }
            return warehouseProduct;
        }
        internal void Save(WarehouseProduct warehouseProduct)
        {
            _inMemoryStreams[warehouseProduct.Sku] = warehouseProduct.GetEvents();
        }
    }
}
