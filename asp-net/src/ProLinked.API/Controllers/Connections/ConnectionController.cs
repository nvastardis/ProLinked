using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Connections;

namespace ProLinked.API.Controllers.Connections;

[ApiController]
[Route("api/connection")]
public class ConnectionController: ControllerBase
{
    private readonly IConnectionService _connectionService;

    public ConnectionController(IConnectionService connectionService)
    {
        _connectionService = connectionService;
    }
}