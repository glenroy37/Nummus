using Microsoft.Extensions.Configuration;

namespace Nummus.Service {
    public class IdentityUserService {
        private readonly IConfiguration _configuration;

        public IdentityUserService(IConfiguration configuration) {
            _configuration = configuration;
        }

        public bool UserRegistrationEnabled => _configuration.GetValue<bool>("UserRegistrationEnabled");
    }
}