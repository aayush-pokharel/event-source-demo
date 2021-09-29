using event_sourcing_demo.cart.domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.web.core.Queries.Models
{
    public class ProductDetails
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public int Stock { get; private set; }
        public Money Price { get; private set; }

        public ProductDetails(Guid id, string name, int stock, Money price)
        {
            Id = id;
            Stock = stock;
            Name = name;
            Price = price;
        }
    }
}
