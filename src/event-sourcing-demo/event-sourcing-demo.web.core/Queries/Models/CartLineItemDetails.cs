using event_sourcing_demo.cart.domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.web.core.Queries.Models
{
    public class CartLineItemDetails
    {
        public Guid Id { get; private set; }
        public int Quantity { get; private set; }
        public string ProductName { get; private set; }
        public Money Price { get; private set; }

        public CartLineItemDetails(Guid id, int quantity, string productName, Money price)
        {
            this.Id = id;
            this.Quantity = quantity;
            this.ProductName = productName;
            this.Price = price;
        }
    }
}
