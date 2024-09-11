namespace ProLinked.Infrastructure.Identity.Auth;

public static class AuthSettings
{
    public static string PrivateKey { get; set; } = null!;
    public static int PrivateKeyExpirationInHours { get; set; } = 1;
    public static string RefreshKey { get; set; } = null!;
    public static int RefreshKeyExpirationInHours { get; set; } = 24;
    public static string Issuer { get; set; } = null!;
    public static string Audience { get; set; } = null!;
}
