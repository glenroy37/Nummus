using System;

namespace Nummus.Exception {
    public class NummusInBetweenAccountStatementMissingException : ApplicationException {
        public NummusInBetweenAccountStatementMissingException() :
            base("There are account statements missing between the one you want to generate and the last one") { }
    }
}