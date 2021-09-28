using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace event_sourcing_demo.console
{
    internal class WarehouseProduct
    {

        internal WarehouseProduct(string sku)
        {
            Sku = sku;
        }

        internal readonly CurrentState _currentState = new();
        internal string Sku { get; }
        private readonly IList<IEvent> _events = new List<IEvent>();

        internal void ShipProduct(int quantity)
        {
            if (quantity > _currentState.QuantityOnHand)
                throw new InvalidDomainException("Don't got no products to ship...");

            AddEvent(new ProductShipped(Sku, quantity, DateTime.UtcNow));
        }
        internal void ReceiveProduct(int quantity)
        {
            AddEvent(new ProductReceived(Sku, quantity, DateTime.UtcNow));
        }
        internal void AdjustInventory(int quantity, string reason)
        {
            if (_currentState.QuantityOnHand + quantity < 0)
                throw new InvalidDomainException("Dont got enough inverntory to adjust to this amount!");

            AddEvent(new InventoryAdjusted(Sku, quantity, reason, DateTime.UtcNow));
        }
        internal IList<IEvent> GetEvents() => _events;
        private void Apply(ProductReceived evnt)
        {
            _currentState.QuantityOnHand += evnt.Quantity;
        }
        private void Apply(ProductShipped evnt)
        {
            _currentState.QuantityOnHand -= evnt.Quantity;
        }
        private void Apply(InventoryAdjusted evnt)
        {
            _currentState.QuantityOnHand += evnt.Quantity;
        }

        internal void AddEvent(IEvent evnt)
        {
            switch (evnt)
            {
                case ProductReceived receiveProdcut:
                    Apply(receiveProdcut);
                    break;
                case ProductShipped shipProduct:
                    Apply(shipProduct);
                    break;
                case InventoryAdjusted adjustInventory:
                    Apply(adjustInventory);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported event addition!");
            }
            _events.Add(evnt);
        }

        internal object GetQuantityOnHand()
        {
            return _currentState.QuantityOnHand;
        }
    }
}
