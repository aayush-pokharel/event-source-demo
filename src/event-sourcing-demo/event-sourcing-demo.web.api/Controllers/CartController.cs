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
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet, Route("{id:guid}", Name = "GetCart")]
        public async Task<IActionResult> GetCart(Guid id, CancellationToken cancellationToken = default)
        {
            var query = new CartById(id);
            var result = await _mediator.Send(query, cancellationToken);
            if (null == result)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCartDto dto, CancellationToken cancellationToken = default)
        {
            if (null == dto)
                return BadRequest();
            var command = new CreateCart(Guid.NewGuid(), dto.ShopName);
            await _mediator.Publish(command, cancellationToken);

            return CreatedAtAction("Cart", new { id = command.CartId }, command);
        }

        [HttpPost, Route("{id:guid}/cartLineItems")]
        public async Task<IActionResult> CreateCartLineItem([FromRoute] Guid id, [FromBody] CreateCartLineItemDto dto, CancellationToken cancellationToken = default)
        {
            if (null == dto)
                return BadRequest();
            var command = new CreateCartLineItem(Guid.NewGuid(), id, dto.ProductId, dto.Quantity);
            await _mediator.Publish(command, cancellationToken);
            return CreatedAtAction("GetCart", "Cart", new { id = command.LineItemId }, command);
        }
    }
}
