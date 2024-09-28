using Microsoft.AspNetCore.Identity.Data;

namespace ProLinked.Application.Contracts.Identity.DTOs;

public class UpdateIdentityDto
{
    public string? NewName { get; init; }
    public string? NewSurname { get; init; }
    public DateTime? NewDateOfBirth { get; init; }
    public string? NewPhoneNumber { get; init; }
    public string? NewSummary { get; init; }
    public string? NewJobTitle{ get; init; }
    public string? NewCompany { get; init; }
    public string? NewCity { get; init; }
    public string? NewEmail { get; init; }
    public string? NewPassword { get; init; }
    public string? OldPassword { get; init; }
}