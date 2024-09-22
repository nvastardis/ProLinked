using ProLinked.Application.DTOs;

namespace ProLinked.Application.Contracts.Chats.DTOs;

public class ChatMembershipLookUpDto: EntityDto
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public Guid? ProfilePictureId { get; set; }
    public string UserFullName { get; set; } = null!;
    public DateTime CreationTime { get; set; }
}