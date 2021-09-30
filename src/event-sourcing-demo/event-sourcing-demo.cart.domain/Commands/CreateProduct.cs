using event_sourcing_demo.cart.core.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace event_sourcing_demo.cart.domain.Commands
{
    public class CreateProduct : INotification
    {
        public CreateProduct(Guid productId, string name, int stock, Money price)
        {
            ProductId = productId;
            Stock = stock;
            Price = price;
        }
        public Guid ProductId { get; }
        public string Name { get; }
        public int Stock { get; }
        public Money Price { get; private set; }
    }
    public class CreateProductHandler : INotificationHandler<CreateProduct>
    {
        private readonly IEventsService<Product, Guid> _productService;
        public CreateProductHandler(IEventsService<Product, Guid> productService) =>
            _productService = productService;
        public async Task Handle(CreateProduct command, CancellationToken cancellationToken)
        {
            var product = new Product(command.ProductId, command.Name, command.Stock, command.Price);
            await _productService.PersistAsync(product);
        }
    }
}
