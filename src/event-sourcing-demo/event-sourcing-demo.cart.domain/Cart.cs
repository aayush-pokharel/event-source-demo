using event_sourcing_demo.cart.core.Interfaces;
using event_sourcing_demo.cart.core.Models;
using event_sourcing_demo.cart.domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.cart.domain
{
    public class Cart : BaseAggregateRoot<Cart, Guid>
    {
        public Cart(Guid id, string shopName) : base(id)
        {
            if (string.IsNullOrWhiteSpace(shopName))
                throw new ArgumentOutOfRangeException(nameof(shopName));
            ShopName = shopName;
            this.AddEvent(new CartCreated(this));
        }

        public string ShopName { get; set; }

        protected override void Apply(IDomainEvent<Guid> @event)
        {
            switch (@event)
            {
                case CartCreated c:
                    this.Id = c.AggregateId;
                    this.ShopName = c.ShopName;
                    break;
            }
        }
        public static Cart Create(string shopName)
        {
            return new Cart(Guid.NewGuid(), shopName);
        }
    }
}
