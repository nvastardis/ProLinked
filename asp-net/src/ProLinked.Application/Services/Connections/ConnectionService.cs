using AutoMapper;
using Microsoft.Extensions.Logging;
using ProLinked.Application.Contracts.Connections;
using ProLinked.Application.DTOs.Connections;
using ProLinked.Application.DTOs.Filtering;
using ProLinked.Domain.Contracts.Connections;
using ProLinked.Domain.DTOs.Connections;
using System.ComponentModel.DataAnnotations;

namespace ProLinked.Application.Services.Connections;

public class ConnectionService: ProLinkedServiceBase, IConnectionService
{
    private IConnectionManager ConnectionManager { get; }
    private IConnectionRepository ConnectionRepository { get; }

    public ConnectionService(
        IMapper mapper,
        ILogger<IConnectionService> logger,
        IConnectionManager connectionManager,
        IConnectionRepository connectionRepository)
        : base(mapper, logger)
    {
        ConnectionManager = connectionManager;
        ConnectionRepository = connectionRepository;
    }

    public async Task<IReadOnlyList<ConnectionLookUpDto>> GetListAsync(
        [Required] UserFilterDto input,
        CancellationToken cancellationToken = default)
    {
        var queryResult = await ConnectionRepository.GetListByUserAsync(
            input.UserId,
            input.Sorting,
            input.SkipCount,
            input.MaxResultCount,
            cancellationToken);
        var result = ObjectMapper.Map<List<ConnectionLookUp>, List<ConnectionLookUpDto>>(queryResult);
        return result.AsReadOnly();
    }

    public async Task DeleteAsync(
        [Required] Guid id,
        [Required] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var connectionToDelete = await ConnectionManager.GetConnectionAsync(id, userId, cancellationToken);
        await ConnectionRepository.DeleteAsync(connectionToDelete, autoSave: true, cancellationToken);
    }
}