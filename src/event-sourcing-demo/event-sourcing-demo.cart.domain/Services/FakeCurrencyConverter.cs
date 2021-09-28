using event_sourcing_demo.cart.domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.cart.domain.Services
{
    public class FakeCurrencyConverter : ICurrencyConverter
    {
        public Money Convert(Money amount, Currency currency)
        {
            return amount.Currency == currency ? amount : new Money(currency, amount.Value);
        }
    }
}
