﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace event_sourcing_demo.web.api.DTO
{
    public class CreateCartLineItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
