using AutoMapper;
using Microsoft.Extensions.Logging;

namespace ProLinked.Application.Services;

public abstract class ProLinkedServiceBase
{
    protected readonly IMapper ObjectMapper;

    protected ProLinkedServiceBase(
        IMapper objectMapper)
    {
        ObjectMapper = objectMapper;

    }
}