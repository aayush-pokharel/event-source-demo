using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.cart.domain.Exceptions
{
    public class ValidationError
    {
        public ValidationError(string context, string message)
        {
            if (string.IsNullOrWhiteSpace(context))
                throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            this.Message = message;
            this.Context = context;
        }

        public string Context { get; }
        public string Message { get; }
    }
}
