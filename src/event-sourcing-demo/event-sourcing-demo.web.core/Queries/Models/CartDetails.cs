using event_sourcing_demo.cart.domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.web.core.Queries.Models
{
    public class CartDetails
    {
        public Guid Id { get; private set; }
        public string ShopName { get; private set; }
        public ICollection<Guid> CartLineItems { get; private set; }
        public Money TotalAmount { get; private set; }

        public CartDetails(Guid id, string shopName, ICollection<Guid> cartLineItems, Money totalAmount)
        {
            Id = id;
            ShopName = shopName;
            CartLineItems = cartLineItems;
            TotalAmount = totalAmount;
        }
    }
}
