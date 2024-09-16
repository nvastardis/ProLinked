namespace ProLinked.Application.DTOs.Identity;

public class RefreshRequest
{
    public required Guid UserId { get; init; }
    public required string RefreshToken { get; init; }
}