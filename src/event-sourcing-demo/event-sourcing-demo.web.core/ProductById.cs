using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.web.core
{
    public class ProductById : IRequest<Queries.Models.ProductDetails>
    {
        public ProductById(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}
