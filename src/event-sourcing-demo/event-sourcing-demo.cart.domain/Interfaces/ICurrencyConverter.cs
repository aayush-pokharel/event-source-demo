using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.cart.domain.Interfaces
{
    public interface ICurrencyConverter
    {
        Money Convert(Money amount, Currency currency);
    }
}
