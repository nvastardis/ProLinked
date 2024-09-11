namespace ProLinked.Infrastructure.Identity.DTOs;

public class RegisterRequest
{
    /// <summary>The user's name.</summary>
    public required string Name { get; init; }

    /// <summary>The user's surname.</summary>
    public required string Surname { get; init; }

    /// <summary>The user's username.</summary>
    public required string UserName { get; init; }

    /// <summary>The user's email address which acts as a user name.</summary>
    public required string Email { get; init; }

    /// <summary>The user's password.</summary>
    public required string Password { get; init; }

    /// <summary>The user's Date of Birth </summary>
    public required DateTime DateOfBirth { get; init; }

}
