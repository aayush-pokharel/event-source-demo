using event_sourcing_demo.cart.core.Interfaces;
using event_sourcing_demo.cart.domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace event_sourcing_demo.cart.domain.Commands
{
    public class AddCartLineItemQuantity : INotification
    {
        public AddCartLineItemQuantity(Guid lineItemId, int quantity)
        {
            LineItemId = lineItemId;
            Quantity = Quantity;
        }

        public Guid LineItemId { get; }
        public int Quantity { get; }
    }

    public class AddCartLineItemQuantityHandler : INotificationHandler<AddCartLineItemQuantity>
    {
        private readonly IEventsService<CartLineItem, Guid> _lineItemEventsService;
        public AddCartLineItemQuantityHandler(IEventsService<CartLineItem, Guid> lineItemEventsService) =>
            _lineItemEventsService = lineItemEventsService;
        public async Task Handle(AddCartLineItemQuantity command, CancellationToken cancellationToken)
        {
            var lineItem = await _lineItemEventsService.RehydrateAsync(command.LineItemId);
            if (lineItem == null)
                throw new ArgumentOutOfRangeException(nameof(AddCartLineItemQuantity.LineItemId), " invalid line item id.");
            lineItem.AddLineItemQuantity(command.Quantity);
            await _lineItemEventsService.PersistAsync(lineItem);
        }
    }
}
