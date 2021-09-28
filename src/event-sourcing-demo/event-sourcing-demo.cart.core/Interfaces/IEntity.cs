using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.cart.core.Interfaces
{
    public interface IEntity<out TKey>
    {
        TKey Id { get; }
    }
}
