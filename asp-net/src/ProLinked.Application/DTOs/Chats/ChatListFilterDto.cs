using ProLinked.Application.DTOs.Filtering;
using System.ComponentModel.DataAnnotations;

namespace ProLinked.Application.DTOs.Chats;

public record ChatListFilterDto: ListFilterDto
{
    [Required]
    public Guid ChatId { get; set; }

}