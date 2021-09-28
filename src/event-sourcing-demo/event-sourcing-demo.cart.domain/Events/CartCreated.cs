using event_sourcing_demo.cart.core.Models;
using System;

namespace event_sourcing_demo.cart.domain.Events
{
    public class CartCreated : BaseDomainEvent<Cart, Guid>
    {
        /// <summary>
        /// for deserialization
        /// </summary>
        private CartCreated() { }

        public CartCreated(Cart cart) : base(cart)
        {
            ShopName = cart.ShopName;
        }

        public string ShopName { get; private set; }
    }
}
