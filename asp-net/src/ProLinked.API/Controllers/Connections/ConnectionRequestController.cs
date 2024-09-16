using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Connections;

namespace ProLinked.API.Controllers.Connections;

[ApiController]
[Route("api/connection-request")]
public class ConnectionRequestController: Controller
{
    private readonly IConnectionRequestService _connectionRequestService;

    public ConnectionRequestController(
        IConnectionRequestService connectionRequestService)
    {
        _connectionRequestService = connectionRequestService;
    }
}