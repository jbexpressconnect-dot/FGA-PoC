using FGA_PoC_Login_Token.Services.Interfaces;
using PermitSDK;
using PermitSDK.Models;

namespace FGA_PoC_Login_Token.Services
{
    public class PermitAuthorizationService : IPermitAuthorizationService
    {
        private readonly Permit _permit;

        public PermitAuthorizationService(IConfiguration configuration)
        {
            var permitToken = configuration["Permit:ApiKey"];
            var permitPdpUrl = configuration["Permit:PdpUrl"];
            _permit = new Permit(permitToken, permitPdpUrl);
        }

        public async Task<bool> IsAllowedAsync(string userId, string action, string resource)
        {
            var user = new UserKey(
                userId,
                "PoC",
                "User",
                userId
            );

            return await _permit.Check(
                user.key, 
                action, 
                resource
            ); 
        }
    }
}
