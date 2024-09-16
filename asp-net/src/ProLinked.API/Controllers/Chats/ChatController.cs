using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Chats;

namespace ProLinked.API.Controllers.Chats;

[ApiController]
[Route("api/chat")]
public class ChatController: ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }
}