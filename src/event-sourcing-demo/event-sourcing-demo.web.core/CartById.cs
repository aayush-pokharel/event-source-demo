using MediatR;
using System;

namespace event_sourcing_demo.web.core
{
    public class CartById : IRequest<Queries.Models.CartDetails>
    {
        public CartById(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}
