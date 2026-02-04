namespace FGA_PoC_Login_Token.Services.Interfaces
{
    public interface IPermitAuthorizationService
    {
        Task<bool> IsAllowedAsync(string userId, string action, string resource);
    }
}
