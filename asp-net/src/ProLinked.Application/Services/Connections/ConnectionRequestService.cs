using AutoMapper;
using ProLinked.Application.Contracts.Connections;
using ProLinked.Application.Contracts.Connections.DTOs;
using ProLinked.Application.Contracts.Filtering;
using ProLinked.Application.DTOs;
using ProLinked.Domain.Contracts.Connections;
using ProLinked.Domain.Contracts.Notifications;
using ProLinked.Domain.DTOs.Connections;
using ProLinked.Domain.Shared.Connections;

namespace ProLinked.Application.Services.Connections;

public class ConnectionRequestService: ProLinkedServiceBase, IConnectionRequestService
{
    private IConnectionManager ConnectionManager { get; }
    private IConnectionRequestRepository ConnectionRequestRepository { get; }
    private INotificationManager NotificationManager { get; }
    public ConnectionRequestService(
        IMapper mapper,
        IConnectionManager connectionManager,
        IConnectionRequestRepository connectionRequestRepository,
        INotificationManager notificationManager)
        : base(mapper)
    {
        ConnectionManager = connectionManager;
        ConnectionRequestRepository = connectionRequestRepository;
        NotificationManager = notificationManager;
    }

    public async Task<PagedAndSortedResultList<ConnectionRequestLookUpDto>> GetListPendingAsync(
        ListFilterDto input,
        Guid userId,
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
        Guid currentUserId,
        Guid targetUserId,
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
        ConnectionRequestCreateDto input,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var result = await ConnectionManager.CreateConnectionRequestAsync(
            userId,
            input.TargetId,
            cancellationToken);

        await NotificationManager.CreateNotificationForConnectionRequestAsync(
            userId,
            input.TargetId,
            result.Id,
            cancellationToken);
    }

    public async Task AcceptAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var requestToAccept = await ConnectionManager.GetRequestAsync(id, userId, cancellationToken);
        await ConnectionManager.UpdateRequestAsync(
            requestToAccept,
            ConnectionRequestStatus.ACCEPTED,
            cancellationToken);
    }

    public async Task RejectAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var requestToReject = await ConnectionManager.GetRequestAsync(id, userId, cancellationToken);
        await ConnectionManager.UpdateRequestAsync(
            requestToReject,
            ConnectionRequestStatus.REJECTED,
            cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await ConnectionManager.DeleteRequestAsync(id, userId, cancellationToken);
    }
}