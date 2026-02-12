using PermitSDK.Models;
using System.Security.Claims;

namespace FGA_PoC_Login_Token.Services.Interfaces;

public interface IPermitAuthorizationService
{
    Task<bool> IsAllowedAsync(UserKey user, string action, string resource, string? resourceId = null);
    Task<bool> IsAllowedAsync(string userId, string action, string resource, string? resourceId = null);
    UserKey GetUserKeyFromClaims(ClaimsPrincipal user);
}
