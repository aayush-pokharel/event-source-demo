
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace event_sourcing_demo.web.core
{
    public class CartLineItems : IRequest<IEnumerable<Queries.Models.CartLineItemDetails>> { }
}
