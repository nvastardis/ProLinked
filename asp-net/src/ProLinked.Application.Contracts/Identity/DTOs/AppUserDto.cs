namespace ProLinked.Application.Contracts.Identity.DTOs;

public class AppUserDto
{
    public Guid Id;
    public string? Email { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
}