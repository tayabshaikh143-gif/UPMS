namespace UPMS.Api.Auth;

public class JwtOptions
{
    public string Issuer { get; set; } = "UPMS";
    public string Audience { get; set; } = "UPMS.Web";
    public string Key { get; set; } = string.Empty;
    public int ExpiryHours { get; set; } = 8;
}

public record RegisterRequest(string Email, string Password, string FullName);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, Guid UserId, string Email, string FullName, IReadOnlyList<string> Roles);
