using event_sourcing_demo.cart.core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.cart.domain.Events
{
    public class CartLineItemQuantityRemoved : BaseDomainEvent<CartLineItem, Guid>
    {
        /// <summary>
        /// for deserialization
        /// </summary>
        private CartLineItemQuantityRemoved() { }

        public CartLineItemQuantityRemoved(CartLineItem item, int quantity) : base(item)
        {
            Quantity = quantity;
            CartId = item.CartId;
            ProductId = item.ProductId;
        }

        public int Quantity { get; private set; }
        public Guid CartId { get; private set; }
        public Guid ProductId { get; private set; }
    }
}
