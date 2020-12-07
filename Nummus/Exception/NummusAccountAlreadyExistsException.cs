using System;

namespace Nummus.Exception {
    public class NummusAccountAlreadyExistsException : ApplicationException {
        public NummusAccountAlreadyExistsException(string message) : base (message) { }
    }
}