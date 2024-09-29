﻿using ProLinked.Domain.Shared.Notifications;

namespace ProLinked.Application.Contracts.Notifications.DTOs;

public class NotificationLookUpDto
{
    public Guid UserId;
    public Guid SourceId;
    public string UserFullName = null!;
    public NotificationTypeEnum NotificationType;
    public string Description = null!;
}