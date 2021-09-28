using event_sourcing_demo.cart.core.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace event_sourcing_demo.cart.domain.Commands
{
    public class RemoveCartLineItemQuantity : INotification
    {
        public RemoveCartLineItemQuantity(Guid lineItemId, int quantity)
        {
            LineItemId = lineItemId;
            Quantity = Quantity;
        }

        public Guid LineItemId { get; }
        public int Quantity { get; }
    }
    public class RemoveCartLineItemQuantityHandler : INotificationHandler<RemoveCartLineItemQuantity>
    {
        private readonly IEventsService<CartLineItem, Guid> _lineItemEventsService;
        public RemoveCartLineItemQuantityHandler(IEventsService<CartLineItem, Guid> lineItemEventsService) =>
            _lineItemEventsService = lineItemEventsService;
        public async Task Handle(RemoveCartLineItemQuantity command, CancellationToken cancellationToken)
        {
            var lineItem = await _lineItemEventsService.RehydrateAsync(command.LineItemId);
            if (lineItem == null)
                throw new ArgumentOutOfRangeException(nameof(AddCartLineItemQuantity.LineItemId), " invalid line item id.");
            
            lineItem.RemoveLineItemQuantity(command.Quantity);
            await _lineItemEventsService.PersistAsync(lineItem);
        }
    }
}
