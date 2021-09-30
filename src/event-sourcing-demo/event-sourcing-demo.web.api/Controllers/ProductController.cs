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
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet, Route("{id:guid}", Name = "GetProduct")]
        public async Task<IActionResult> GetProduct(Guid id, CancellationToken cancellationToken = default)
        {
            var query = new ProductById(id);
            var result = await _mediator.Send(query, cancellationToken);
            if (null == result)
                return NotFound();
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto, CancellationToken cancellationToken = default)
        {
            if (null == dto)
                return BadRequest();
            var command = new CreateProduct(Guid.NewGuid(), dto.Name, dto.Stock, dto.Price);
            await _mediator.Publish(command, cancellationToken);

            return CreatedAtAction("Cart", new { id = command.ProductId }, command);
        }
    }
}
