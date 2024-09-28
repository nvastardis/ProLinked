using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProLinked.Domain.Entities.Identity;

public class AppUser: IdentityUser<Guid>
{
    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string Surname { get; set; } = null!;

    [Required]
    [Column(TypeName="date")]
    public DateTime? DateOfBirth { get; set;}

    [MaxLength(150)]
    public string? Summary { get; set;}

    [MaxLength(80)]
    public string? JobTitle { get; set;}

    [MaxLength(80)]
    public string? Company { get; set;}

    [MaxLength(80)]
    public string? City { get; set;}

    public Guid? PhotographId { get; set;}

    public Guid? CurriculumVitaeId { get; set;}

    [MaxLength(256)]
    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpirationDate { get; set; }
}