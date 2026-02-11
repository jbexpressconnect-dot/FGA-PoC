namespace FGA_PoC_Login_Token.Models;

public sealed record UserInfo(string? Email, string? Name, string? LastName = null);