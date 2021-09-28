using event_sourcing_demo.cart.core.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace event_sourcing_demo.cart.domain.Commands
{
    public class BuyProduct : INotification
    {
        public BuyProduct(Guid productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }
        public Guid ProductId { get; }
        public int Quantity { get; }
    }
    public class BuyProductHandler : INotificationHandler<BuyProduct>
    {
        private readonly IEventsService<Product, Guid> _productService;
        public BuyProductHandler(IEventsService<Product, Guid> productService) =>
            _productService = productService;
        public async Task Handle(BuyProduct command, CancellationToken cancellationToken)
        {
            var product = await _productService.RehydrateAsync(command.ProductId);
            if (product == null)
                throw new ArgumentOutOfRangeException(nameof(AddCartLineItemQuantity.LineItemId), " invalid line item id.");

            product.BuyProduct(command.Quantity);
            await _productService.PersistAsync(product);
        }
    }
}
