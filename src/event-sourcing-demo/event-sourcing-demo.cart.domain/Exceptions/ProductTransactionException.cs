using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.cart.domain.Exceptions
{
    public class ProductTransactionException : Exception
    {
        public Product Product { get; }
        public ProductTransactionException(string s, Product product) : base(s) =>
            Product = product;

    }
}
