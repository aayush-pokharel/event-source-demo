using event_sourcing_demo.cart.core.Interfaces;
using event_sourcing_demo.cart.core.Models;
using event_sourcing_demo.cart.domain.Events;
using event_sourcing_demo.cart.domain.Exceptions;
using System;

namespace event_sourcing_demo.cart.domain
{
    public class Product : BaseAggregateRoot<Product, Guid>
    {
        public string Name { get; set; }
        public int Stock { get; set; }
        public Money Price { get; set; }

        private Product() { }
        public Product(Guid id, string name, int stock, Money price) : base(id)
        {
            Stock = stock;
            Name = name;
            Price = price;
        }
        public void BuyProduct(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "amount cannot be negative");

            if (quantity > this.Stock)
                throw new ProductTransactionException($"unable to take {quantity} from product {this.Id}", this);

            this.AddEvent(new ProductBought(this, quantity));
        }

        protected override void Apply(IDomainEvent<Guid> @event)
        {
            switch (@event)
            {
                case ProductCreated c:
                    this.Id = c.AggregateId;
                    this.Price = new Money(c.Currency, 0);
                    this.Name = c.Name;
                    this.Stock = c.Stock;
                    break;
                case ProductBought a:
                    this.Stock -= a.Quantity;
                    break;
            }
        }
    }
}
