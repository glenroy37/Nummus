using System;

namespace Nummus.Exception {
    public class NummusCategoryContainsBookingLinesException : ApplicationException {
        public NummusCategoryContainsBookingLinesException() : base("You mustn't delete a category which contains booking lines") { }
    }
}