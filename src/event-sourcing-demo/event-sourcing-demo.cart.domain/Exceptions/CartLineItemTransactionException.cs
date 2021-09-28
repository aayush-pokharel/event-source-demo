using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.cart.domain.Exceptions
{
    public class CartLineItemTransactionException : Exception
    {
        public CartLineItem CartLineItem { get; }
        public CartLineItemTransactionException(string s, CartLineItem cartLineItem) : base(s) =>
            CartLineItem = cartLineItem;
    }
}
