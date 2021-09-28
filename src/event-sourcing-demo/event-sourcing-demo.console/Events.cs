using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.console
{
    internal interface IEvent { }
    internal record  ProductShipped(string Sku, int Quantity, DateTime Date): IEvent;
    internal record ProductReceived(string Sku, int Quantity, DateTime Date) : IEvent;
    internal record InventoryAdjusted(string Sku, int Quantity, string Reason, DateTime Date) : IEvent;
}
