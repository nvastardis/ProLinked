namespace ProLinked.Infrastructure.Auth;

public static class AuthSettings
{
    public static string PrivateKey { get; set; } = null!;
    public static string Issuer { get; set; } = null!;
    public static string Audience { get; set; } = null!;
}
