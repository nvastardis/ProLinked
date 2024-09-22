using ProLinked.Application.DTOs;

namespace ProLinked.Application.Contracts.Chats.DTOs;

public class ChatWithDetailsDto: EntityDto<Guid>
{
    public string Title { get; set; } = null!;
    public Guid? ImageId {get;set;}
}