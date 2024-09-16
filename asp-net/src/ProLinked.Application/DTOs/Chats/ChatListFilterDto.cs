using ProLinked.Application.DTOs.Filtering;
using System.ComponentModel.DataAnnotations;

namespace ProLinked.Application.DTOs.Chats;

public class ChatListFilterDto: ListFilterDto
{
    [Required]
    public Guid ChatId { get; set; }

}