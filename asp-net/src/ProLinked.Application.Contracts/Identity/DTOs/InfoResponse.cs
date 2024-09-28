namespace ProLinked.Application.Contracts.Identity.DTOs;

public class InfoResponse
{
    /// <summary>
    /// The email address associated with the authenticated user.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// The email address associated with the authenticated user.
    /// </summary>
    public required string UserName { get; init; }


    /// <summary>
    /// The email address associated with the authenticated user.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The email address associated with the authenticated user.
    /// </summary>
    public required string Surname { get; init; }


    /// <summary>
    /// The email address associated with the authenticated user.
    /// </summary>
    public required DateTime DateOfBirth { get; init; }

    /// <summary>
    /// The phone number associated with the authenticated user.
    /// </summary>
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// The Summary associated with the authenticated user.
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// The current job title associated with the authenticated user.
    /// </summary>
    public string? JobTitle{ get; init; }

    /// <summary>
    /// The company associated with the authenticated user.
    /// </summary>
    public string? Company { get; init; }

    /// <summary>
    /// The city associated with the authenticated user.
    /// </summary>
    public string? City { get; init; }
}