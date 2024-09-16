using System.ComponentModel.DataAnnotations;

namespace ProLinked.Application.DTOs.Filtering;

public class UserFilterDto: ListFilterDto
{
    [Required]
    public Guid UserId { get; set; }
}