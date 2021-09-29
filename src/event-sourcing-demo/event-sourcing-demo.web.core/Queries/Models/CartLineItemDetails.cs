using event_sourcing_demo.cart.domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.web.core.Queries.Models
{
    public class CartLineItemDetails
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public Guid CartId { get; set; }
        public int Quantity { get; private set; }
        public string ProductName { get; private set; }
        public Money TotalAmount { get; private set; }

        public CartLineItemDetails(Guid id, Guid cartId, Guid productId, int quantity, string productName, Money totalAmount)
        {
            this.Id = id;
            this.CartId = cartId;
            this.ProductId = productId;
            this.Quantity = quantity;
            this.ProductName = productName;
            this.TotalAmount = totalAmount;
        }
    }
}
