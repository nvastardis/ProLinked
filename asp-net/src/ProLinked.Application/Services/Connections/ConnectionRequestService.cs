using AutoMapper;
using Microsoft.Extensions.Logging;
using ProLinked.Application.Contracts.Connections;
using ProLinked.Application.DTOs;
using ProLinked.Application.DTOs.Connections;
using ProLinked.Application.DTOs.Filtering;
using ProLinked.Domain.Contracts.Connections;
using ProLinked.Domain.DTOs.Connections;
using ProLinked.Domain.Shared.Connections;
using System.ComponentModel.DataAnnotations;

namespace ProLinked.Application.Services.Connections;

public class ConnectionRequestService: ProLinkedServiceBase, IConnectionRequestService
{
    private IConnectionManager ConnectionManager { get; }
    private IConnectionRequestRepository ConnectionRequestRepository { get; }
    private IConnectionRepository ConnectionRepository { get; }

    public ConnectionRequestService(
        IMapper mapper,
        ILogger<IConnectionRequestService> logger,
        IConnectionManager connectionManager,
        IConnectionRepository connectionRepository,
        IConnectionRequestRepository connectionRequestRepository)
        : base(mapper, logger)
    {
        ConnectionManager = connectionManager;
        ConnectionRepository = connectionRepository;
        ConnectionRequestRepository = connectionRequestRepository;
    }

    public async Task<PagedAndSortedResultList<ConnectionRequestLookUpDto>> GetListPendingAsync(
        [Required] ListFilterDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var queryResult = await ConnectionRequestRepository.GetListByUserAsTargetAsync(
            userId,
            ConnectionRequestStatus.PENDING,
            input.SkipCount,
            input.MaxResultCount,
            cancellationToken);
        var items = ObjectMapper.Map<List<ConnectionRequestLookUp>, List<ConnectionRequestLookUpDto>>(queryResult);
        var itemCount = items.Count;
        return new PagedAndSortedResultList<ConnectionRequestLookUpDto>(itemCount, items.AsReadOnly());
    }

    public async Task<ConnectionRequestSearchResultDto> FindPendingForUserAsync(
        [Required] Guid currentUserId,
        [Required] Guid targetUserId,
        CancellationToken cancellationToken = default)
    {
        var requestFound = await ConnectionRequestRepository.FindAsync(
            e =>(
                    (e.SenderId == currentUserId && e.TargetId == targetUserId) ||
                    (e.SenderId == targetUserId && e.TargetId == currentUserId)
                ) &&
                e.Status == ConnectionRequestStatus.PENDING,
            true,
            cancellationToken);

        var result =
            new ConnectionRequestSearchResultDto
            {
                Found = requestFound is not null,
                RequestId = requestFound?.Id,
                CreationTime = requestFound?.CreationTime,
                Status = requestFound?.Status,
                SenderId = requestFound?.SenderId
            };

        return result;
    }

    public async Task CreateAsync(
        [Required] ConnectionRequestCreateDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var request = await ConnectionManager.CreateConnectionRequestAsync(
            userId,
            input.TargetId,
            cancellationToken);

        await ConnectionRequestRepository.InsertAsync(request, autoSave: true, cancellationToken);
    }

    public async Task AcceptAsync(
        [Required] Guid id,
        [Required] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var requestToAccept = await ConnectionManager.GetRequestAsync(id, userId, cancellationToken);
        var requestInfo = await ConnectionManager.UpdateRequestAsync(
            requestToAccept,
            ConnectionRequestStatus.ACCEPTED,
            cancellationToken);

        await ConnectionRequestRepository.UpdateAsync(requestInfo.Request, autoSave: true, cancellationToken);
        await ConnectionRepository.InsertAsync(requestInfo.Connection! , autoSave: true, cancellationToken);
    }

    public async Task RejectAsync(
        [Required] Guid id,
        [Required] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var requestToReject = await ConnectionManager.GetRequestAsync(id, userId, cancellationToken);
        var requestInfo = await ConnectionManager.UpdateRequestAsync(
            requestToReject,
            ConnectionRequestStatus.REJECTED,
            cancellationToken);

        await ConnectionRequestRepository.UpdateAsync(requestInfo.Request, autoSave: true, cancellationToken);
    }

    public async Task DeleteAsync(
        [Required] Guid id,
        [Required] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var requestToDelete = await ConnectionManager.GetRequestAsync(id, userId, cancellationToken);
        await ConnectionRequestRepository.DeleteAsync(requestToDelete, autoSave: true, cancellationToken);
    }
}