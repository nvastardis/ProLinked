using Microsoft.AspNetCore.Identity;
using ProLinked.Domain.Contracts.Connections;
using ProLinked.Domain.Entities.Connections;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Shared;
using ProLinked.Domain.Shared.Connections;
using ProLinked.Domain.Shared.Exceptions;
using ConnectionInfo = ProLinked.Domain.DTOs.Connections.ConnectionInfo;

namespace ProLinked.Domain.Services;

public class ConnectionManager: IConnectionManager
{
    private readonly IConnectionRepository _connectionsRepository;
    private readonly IConnectionRequestRepository _connectionRequestsRepository;
    private readonly UserManager<AppUser> _userManager;

    public ConnectionManager(
        IConnectionRepository connectionsRepository,
        IConnectionRequestRepository connectionRequestsRepository,
        UserManager<AppUser> userManager)
    {
        _connectionsRepository = connectionsRepository;
        _connectionRequestsRepository = connectionRequestsRepository;
        _userManager = userManager;
    }

    public async Task<ConnectionRequest> CreateConnectionRequestAsync(
        Guid senderId,
        Guid targetId,
        CancellationToken cancellationToken = default)
    {
        _ = await FindUserAsync(targetId);

        var connection = await _connectionsRepository.FindAsync(e =>
                (e.UserAId == targetId && e.UserBId == senderId) ||
                (e.UserAId == senderId && e.UserBId == targetId),
            cancellationToken: cancellationToken);
        if (connection is not null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ConnectionAlreadyExists)
                .WithData(nameof(Connection.Id), connection.Id);
        }

        var connectionRequest = await _connectionRequestsRepository.FindAsync(e =>
            (e.SenderId == targetId && e.TargetId == senderId) ||
            (e.SenderId == senderId && e.TargetId == targetId),
            cancellationToken: cancellationToken);
        if (connectionRequest is not null && connectionRequest.Status != ConnectionRequestStatus.REJECTED)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ConnectionRequestAlreadyExists)
                .WithData(nameof(ConnectionRequest.Id), connectionRequest.Id);
        }


        cancellationToken.ThrowIfCancellationRequested();
        var newConnectionRequest = new ConnectionRequest(
            Guid.NewGuid(),
            senderId,
            targetId);

        await _connectionRequestsRepository.InsertAsync(newConnectionRequest, autoSave: true, cancellationToken);
        return newConnectionRequest;
    }

    public async Task<ConnectionInfo> UpdateRequestAsync(
        ConnectionRequest request,
        ConnectionRequestStatus status,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        switch (request.Status)
        {
            case ConnectionRequestStatus.ACCEPTED:
                throw new BusinessException(ProLinkedDomainErrorCodes.ConnectionRequestAccepted);
            case ConnectionRequestStatus.REJECTED:
                throw new BusinessException(ProLinkedDomainErrorCodes.ConnectionRequestRejected);
            default:
                request.SetStatus(status);
                await _connectionRequestsRepository.UpdateAsync(request, autoSave: true, cancellationToken);
                break;
        }

        var info = new ConnectionInfo
        {
            Request = request
        };

        if (status == ConnectionRequestStatus.ACCEPTED)
        {
           info.Connection = new Connection(
                Guid.NewGuid(),
                request.SenderId,
                request.TargetId);

           await _connectionsRepository.InsertAsync(info.Connection, autoSave: true, cancellationToken);
        }
        return info;
    }

    public async Task<Connection> GetConnectionAsync(
        Guid connectionId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var connection = await _connectionsRepository.GetAsync(connectionId, false, cancellationToken);
        if (connection.UserAId != userId && connection.UserBId != userId)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.UserNotFoundInConnection);
        }
        return connection;
    }

    public async Task<ConnectionRequest> GetRequestAsync(
        Guid requestId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var request = await _connectionRequestsRepository.GetAsync(requestId, false, cancellationToken);
        if (request.SenderId != userId && request.TargetId != userId)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.UserNotFoundInConnection);
        }
        return request;
    }

    public async Task DeleteRequestAsync(
        Guid requestId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var request = await GetRequestAsync(requestId, userId, cancellationToken);
        if (request.Status != ConnectionRequestStatus.PENDING)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ConnectionRequestInvalidStatus);
        }

        await _connectionRequestsRepository.DeleteAsync(request, autoSave: true, cancellationToken);
    }

    public async Task DeleteConnectionAsync(Guid connectionId, Guid userId, CancellationToken cancellationToken = default)
    {
        var connection = await GetConnectionAsync(connectionId, userId, cancellationToken);
        await _connectionsRepository.DeleteAsync(connection, autoSave: true, cancellationToken);
    }


    private async Task<AppUser> FindUserAsync(
        Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.UserNotFound)
                .WithData("UserId", userId);
        }

        return user;
    }
}