using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ProLinked.Application.Contracts.Chats.DTOs;

public class ChatCreateDto
{
    [Required]
    [Length(1, 10)]
    public List<Guid> UserIds { get; set; } = null!;
    public string? Title { get; set; }
    public IFormFile? Image { get; set; }
}