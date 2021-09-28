using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.web.core
{
    public class CartLineItemById : IRequest<Queries.Models.CartLineItemDetails>
    {
        public CartLineItemById(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}
