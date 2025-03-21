﻿namespace ProLinked.Domain.Chats;

public class ChatMembershipLookUp
{
    public Guid UserId { get; set; }
    public Guid ChatId { get; set; }
    public Guid PhotoId { get; set; }
    public string UserFullName { get; set; } = null!;
    public DateTime CreationTime { get; set; }
}
