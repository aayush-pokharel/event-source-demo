using event_sourcing_demo.cart.domain.Commands;
using event_sourcing_demo.web.api.DTO;
using event_sourcing_demo.web.core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace event_sourcing_demo.web.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartLineItemController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartLineItemController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet, Route("{id:guid}", Name = "GetCartLineItem")]
        public async Task<IActionResult> GetCartLineItem(Guid id, CancellationToken cancellationToken = default)
        {
            var query = new CartLineItemById(id);
            var result = await _mediator.Send(query, cancellationToken);
            if (null == result)
                return NotFound();
            return Ok(result);
        }

        [HttpPut, Route("{id:guid}/AddQuantity/{quantity:int}")]
        public async Task<IActionResult> AddQuantity([FromRoute] Guid id, [FromRoute] int quanity, CancellationToken cancellationToken = default)
        {
            var command = new AddCartLineItemQuantity(id, quanity);
            await _mediator.Publish(command, cancellationToken);
            return Ok();
        }

        [HttpPut, Route("{id:guid}/RemoveQuantity/{quantity:int}")]
        public async Task<IActionResult> Withdraw([FromRoute] Guid id, [FromRoute] int quanity, CancellationToken cancellationToken = default)
        {
            var command = new RemoveCartLineItemQuantity(id, quanity);
            await _mediator.Publish(command, cancellationToken);
            return Ok();
        }
    }
}
