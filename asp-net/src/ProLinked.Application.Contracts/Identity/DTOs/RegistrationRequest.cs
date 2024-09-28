namespace ProLinked.Application.Contracts.Identity.DTOs;

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

    /// <summary>The user's phone number.</summary>
    public string? PhoneNumber { get; init; }

    /// <summary>The user's personal Summary</summary>
    public string? Summary { get; init; }

    /// <summary>The user's current job title </summary>
    public string? JobTitle{ get; init; }

    /// <summary>The user's current company</summary>
    public string? Company { get; init; }

    /// <summary>The user's current city residency</summary>
    public string? City { get; init; }
}