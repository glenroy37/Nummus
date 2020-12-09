using Microsoft.Extensions.Configuration;

namespace Nummus.Helper {
    public class IdentityHelper {
        private readonly IConfiguration _configuration;

        public IdentityHelper(IConfiguration configuration) {
            _configuration = configuration;
        }

        public bool UserRegistrationEnabled => _configuration.GetValue<bool>("UserRegistrationEnabled");
    }
}