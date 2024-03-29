﻿using System;
using System.Runtime.Serialization;

namespace event_sourcing_demo.console
{
    [Serializable]
    internal class InvalidDomainException : Exception
    {
        public InvalidDomainException()
        {
        }

        public InvalidDomainException(string message) : base(message)
        {
        }

        public InvalidDomainException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}