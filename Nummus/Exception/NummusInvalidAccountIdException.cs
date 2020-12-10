using System;

namespace Nummus.Exception {
    public class NummusInvalidAccountIdException : ApplicationException {
        public NummusInvalidAccountIdException() : base("The provided account id is invalid!") { }
    }
}