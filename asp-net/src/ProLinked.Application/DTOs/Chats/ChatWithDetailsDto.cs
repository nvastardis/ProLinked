using Microsoft.AspNetCore.Http;

namespace ProLinked.Application.DTOs.Chats;

public class ChatWithDetailsDto: EntityDto<Guid>
{
    public string Title { get; set; } = null!;
    public Guid? ImageId {get;set;}
}