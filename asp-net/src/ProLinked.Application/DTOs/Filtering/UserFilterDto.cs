using System.ComponentModel.DataAnnotations;

namespace ProLinked.Application.DTOs.Filtering;

public record UserFilterDto: ListFilterDto
{
    [Required]
    public Guid UserId { get; set; }
}