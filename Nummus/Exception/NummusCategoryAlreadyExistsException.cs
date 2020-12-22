using System;

namespace Nummus.Exception {
    public class NummusCategoryAlreadyExistsException : ApplicationException {
        public NummusCategoryAlreadyExistsException() : base ("A category with that name already exists!") { }
    }
}