namespace ProLinked.Application.Contracts.Identity.DTOs;

public class LoginRequest
{
    // <summary>The user's email address which acts as a user name.</summary>
    public required string Username { get; init; }

    /// <summary>The user's password.</summary>
    public required string Password { get; init; }
}