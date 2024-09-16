using AutoMapper;

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