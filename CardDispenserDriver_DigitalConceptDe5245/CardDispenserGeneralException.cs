using System;

namespace CardDispenserDigitalConceptDe5245Driver
{
    public class CardDispenserException : Exception
    {
        public CardDispenserException(string message) : base(message) { }

        public CardDispenserException(Exception exception) : base(exception?.Message, exception) { }
    }
}
