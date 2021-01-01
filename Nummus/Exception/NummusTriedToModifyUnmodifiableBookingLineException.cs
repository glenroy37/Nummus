using System;

namespace Nummus.Exception {
    public class NummusTriedToModifyUnmodifiableBookingLineException : ApplicationException {
        public NummusTriedToModifyUnmodifiableBookingLineException()
            : base("You mustn't modify a booking line after its part of an AccountStatement!") { }
    }
}