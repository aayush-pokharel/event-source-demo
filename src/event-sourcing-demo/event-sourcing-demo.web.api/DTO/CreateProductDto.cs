using event_sourcing_demo.cart.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace event_sourcing_demo.web.api.DTO
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public int Stock { get; private set; }
        public decimal Price { get; private set; }
    }
}
