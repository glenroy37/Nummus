using System;

namespace Nummus.Exception {
    public class NummusAccountAlreadyExistsException : ApplicationException {
        public NummusAccountAlreadyExistsException() : base ("An account with that name already exists!") { }
    }
}