using FGA_PoC_Login_Token.Models;
using FGA_PoC_Login_Token.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using PermitSDK;
using PermitSDK.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FGA_PoC_Login_Token.Services
{
    public class PermitAuthorizationService : IPermitAuthorizationService
    {
        private readonly Permit _permit;
        private readonly ILogger<PermitAuthorizationService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermitAuthorizationService(IConfiguration configuration,
            ILogger<PermitAuthorizationService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            var permitToken = configuration["Permit:ApiKey"];
            var permitPdpUrl = configuration["Permit:PdpUrl"];
            _permit = new Permit(permitToken, permitPdpUrl);
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> IsAllowedAsync(string userId, string action, string resource, string? resourceId = null)
        {
            try
            {
                var userKey = new UserKey(userId);

                var resourceObj = string.IsNullOrEmpty(resourceId)
                    ? new ResourceInput(resource)
                    : new ResourceInput(resource, resourceId);

                var permitted = await _permit.Check(userKey, action, resourceObj);

                _logger.LogInformation(
                    "Permission check: User={UserId}, Action={Action}, Resource={Resource}, ResourceId={ResourceId}, Result={Result}",
                    userId, action, resource, resourceId ?? "null", permitted);

                return permitted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> IsAllowedAsync(UserKey user, string action, string resource, string? resourceId = null)
        {
            try
            {
                var resourceObj = string.IsNullOrEmpty(resourceId)
                    ? new ResourceInput(resource)
                    : new ResourceInput(resource, resourceId);

                var permitted = await _permit.Check(user, action, resourceObj);

                _logger.LogInformation(
                    "Permission check: User={UserKey}, Action={Action}, Resource={Resource}, ResourceId={ResourceId}, Result={Result}",
                    user.key, action, resource, resourceId ?? "null", permitted);

                return permitted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission for user {UserKey}", user.key);
                return false;
            }
        }

        public UserKey GetUserKeyFromClaims(ClaimsPrincipal user)
        {
            var email = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst(ClaimTypes.Email)?.Value
                ?? user.FindFirst("email")?.Value
                ?? throw new InvalidOperationException("User email not found in claims");

            return new UserKey(
                key: email,
                firstName: string.Empty,
                lastName: string.Empty,
                email: email
            );
        }


        private async Task<UserInfo> DExtractUserInfoTokenAsync()
        {
            var idToken = await _httpContextAccessor?.HttpContext?.GetTokenAsync("id_token");

            if (string.IsNullOrEmpty(idToken))
            {
                throw new InvalidOperationException("No ID token found in the current context");
            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(idToken);

            // Extract email (or preferred_username) and name from claims
            var email = jsonToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value
                        ?? jsonToken.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;

            var name = jsonToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

            var usrInfo = new UserInfo(email, name);

            return usrInfo;
        }
    }
}
