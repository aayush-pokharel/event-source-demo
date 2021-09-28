using event_sourcing_demo.cart.core.Interfaces;
using event_sourcing_demo.cart.domain.Exceptions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace event_sourcing_demo.cart.domain.Commands
{
    public class CreateCart : INotification 
    {
        public CreateCart(Guid cartId, string shopName)
        {
            ShopName = shopName;
            CartId = cartId;
        }
        public Guid CartId { get; set; }
        public string ShopName { get; }
    }
    public class CreateCartHandler : INotificationHandler<CreateCart>
    {
        private readonly IEventsService<Cart, Guid> _cartService;
        public CreateCartHandler(IEventsService<Cart, Guid> cartService) =>
            _cartService = cartService;
        public async Task Handle(CreateCart command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(command.ShopName))
                throw new ValidationException("Unable to create Cart", new ValidationError(nameof(command.ShopName), "email cannot be empty"));
            
            var cart = new Cart(command.CartId, command.ShopName);
            await _cartService.PersistAsync(cart);
        }
    }
}
