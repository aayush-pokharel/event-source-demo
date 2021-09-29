using event_sourcing_demo.cart.core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.cart.domain.Events
{
    public class CartLineItemCreated : BaseDomainEvent<CartLineItem, Guid>
    {
        /// <summary>
        /// for deserialization
        /// </summary>
        private CartLineItemCreated() { }

        public CartLineItemCreated(CartLineItem item) : base(item)
        {
            CartId = item.CartId;
            ProductId = item.ProductId;
            Quantity = item.Quantity;
        }

        public Guid CartId { get; private set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Money TotalAmount { get; set; }
    }
}
