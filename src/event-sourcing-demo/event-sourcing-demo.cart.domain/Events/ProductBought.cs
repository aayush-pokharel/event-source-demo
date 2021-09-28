using event_sourcing_demo.cart.core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.cart.domain.Events
{
    public class ProductBought : BaseDomainEvent<Product, Guid>
    {
        /// <summary>
        /// for deserialization
        /// </summary>
        private ProductBought() { }

        public ProductBought(Product product, int quantity) : base(product)
        {
            Quantity = quantity;
        }

        public int Quantity { get; private set; }
    }
}
