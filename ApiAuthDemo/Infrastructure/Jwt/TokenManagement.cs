namespace ApiAuthDemo.Infrastructure.Jwt;

public class TokenManagement
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessExpiration { get; set; }
    public int RefreshExpiration { get; set; }
}