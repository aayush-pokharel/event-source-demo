using event_sourcing_demo.cart.core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.cart.domain.Events
{
    public class ProductCreated : BaseDomainEvent<Product, Guid>
    {
        /// <summary>
        /// for deserialization
        /// </summary>
        private ProductCreated() { }

        public ProductCreated(Product product) : base(product)
        {
            Name = product.Name;
            Stock = product.Stock;
            Price = product.Price;
            Currency = product.Price.Currency;
        }

        public string Name { get; private set; }
        public int Stock { get; private set; }
        public Money Price { get; private set; }
        public Currency Currency { get; private set; }
    }
}
