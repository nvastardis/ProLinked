namespace ProLinked.Infrastructure.Identity.DTOs;

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
}
