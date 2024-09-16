namespace ProLinked.Application.DTOs.Chats;

public class ChatUpdateTitleDto: EntityDto<Guid>
{
    public string Title { get; set; } = null!;
}