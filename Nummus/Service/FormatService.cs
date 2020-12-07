using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace Nummus.Service {
    public class FormatService {
        private readonly IConfiguration _configuration;
        
        public FormatService(IConfiguration configuration) {
            _configuration = configuration;
        }
        
        public string FormatToLocalCurrency(decimal amount) {
            return String.Format(CultureInfo.GetCultureInfo(_configuration.GetValue<string>("Locale")), "{0:C}", amount);
        }
    }
}