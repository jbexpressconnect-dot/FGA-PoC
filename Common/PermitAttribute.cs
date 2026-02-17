using FGA_PoC_Login_Token.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FGA_PoC_Login_Token.Common;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class PermitAuthorizeAttribute : Attribute
{
    public string Action { get; }
    public string Resource { get; }
    public bool UseRouteId { get; set; } = false; // Para recursos específicos

    public PermitAuthorizeAttribute(string action, string resource)
    {
        Action = action ?? throw new ArgumentNullException(nameof(action));
        Resource = resource ?? throw new ArgumentNullException(nameof(resource));
    }
}

public class PermitAuthorizeFilter : IAsyncAuthorizationFilter
{
    private readonly IPermitAuthorizationService _permitService;

    public PermitAuthorizeFilter(IPermitAuthorizationService permitService)
    {
        _permitService = permitService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Get the PermitAuthorizeAttribute from the endpoint metadata
        var attribute = context.ActionDescriptor.EndpointMetadata
            .OfType<PermitAuthorizeAttribute>()
            .FirstOrDefault();

        if (attribute == null) return;

        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            await Task.CompletedTask;
            return;
        }

        var userKey = _permitService.GetUserKeyFromClaims(context.HttpContext.User);

        var allowed = await _permitService.IsAllowedAsync(
            userKey.key,
            attribute.Action,
            attribute.Resource);

        if (!allowed)
        {
            context.Result = new ForbidResult();
        }
    }
}