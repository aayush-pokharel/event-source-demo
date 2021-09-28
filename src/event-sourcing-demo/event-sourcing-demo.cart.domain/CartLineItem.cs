using event_sourcing_demo.cart.core.Interfaces;
using event_sourcing_demo.cart.core.Models;
using event_sourcing_demo.cart.domain.Events;
using event_sourcing_demo.cart.domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.cart.domain
{
    public class CartLineItem : BaseAggregateRoot<CartLineItem, Guid>
    {
        public Guid ProductId { get; set; }
        public Guid CartId { get; set; }
        public int Quantity { get; set; }

        private CartLineItem() { }
        public CartLineItem(Guid id, Product product, Cart cart, int quantity) : base(id)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (quantity < 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "amount cannot be negative");
            if (quantity > product.Stock)
                throw new ProductTransactionException($"Unable to get {quantity} products from {product.Name}.", product);

            this.ProductId = product.Id;
            this.CartId = cart.Id;
            this.Quantity = quantity;

            this.AddEvent(new CartLineItemCreated(this));
        }
        public void AddLineItemQuantity(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "amount cannot be negative");

            this.AddEvent(new CartLineItemQuantityAdded(this, quantity));
        }
        public void RemoveLineItemQuantity(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "amount cannot be negative");
            if (quantity > Quantity)
                throw new CartLineItemTransactionException($"unable to remove {quantity} items from cart line item {this.Id}", this);
            this.AddEvent(new CartLineItemQuantityRemoved(this, quantity));
        }
        protected override void Apply(IDomainEvent<Guid> @event)
        {
            switch (@event)
            {
                case CartLineItemCreated c:
                    this.Id = c.AggregateId;
                    this.ProductId = c.ProductId;
                    this.CartId = c.CartId;
                    this.Quantity = c.Quantity;
                    break;
                case CartLineItemQuantityRemoved c:
                    this.Quantity -= c.Quantity;
                    break;
                case CartLineItemQuantityAdded c:
                    this.Quantity += c.Quantity;
                    break;
            }
        }
    }
}
