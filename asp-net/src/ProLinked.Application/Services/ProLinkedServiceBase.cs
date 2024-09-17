using AutoMapper;
using Microsoft.Extensions.Logging;

namespace ProLinked.Application.Services;

public abstract class ProLinkedServiceBase
{
    protected readonly IMapper ObjectMapper;
    protected readonly ILogger Logger;

    protected ProLinkedServiceBase(
        IMapper objectMapper,
        ILogger logger)
    {
        ObjectMapper = objectMapper;
        Logger = logger;
    }
}