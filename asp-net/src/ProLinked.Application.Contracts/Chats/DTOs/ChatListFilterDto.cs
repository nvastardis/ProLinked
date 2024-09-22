using ProLinked.Application.Contracts.Filtering;
using System.ComponentModel.DataAnnotations;

namespace ProLinked.Application.Contracts.Chats.DTOs;

public record ChatListFilterDto: ListFilterDto
{
    [Required]
    public Guid ChatId { get; set; }

}