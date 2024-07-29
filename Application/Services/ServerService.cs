using Application.Common.Interfaces;
using Contracts.Common.Models.Enums;
using System.Threading.Tasks;

namespace Application.Services;

public class ServerService : IServerService
{
    private readonly IServerRepository _serverRepository;
    private readonly IServerPublisher _serverPublisher;

    public ServerService(IServerRepository serverRepository, IServerPublisher serverPublisher)
    {
        _serverRepository = serverRepository;
        _serverPublisher = serverPublisher;
    }

    public async Task Up(string serverId)
    {
        await _serverRepository.SetServerReady(serverId);

        var server = await _serverRepository.GetByServerId(serverId);

        await _serverPublisher.ServerUp(server);
    }

    public async Task PlayerConnected(string serverId, string userId)
    {
        await _serverPublisher.PlayerConnected(serverId, userId);
    }

    public async Task PlayerDisconnected(string serverId, string userId)
    {
        await _serverPublisher.PlayerDisconnected(serverId, userId);
    }

    public async Task Down(string serverId)
    {
        var server = await _serverRepository.GetByServerId(serverId);
        //Bad down
        if (server.IsActive)
        {
            await _serverRepository.SetServerInactive(serverId);
            await _serverPublisher.ServerBadDown(serverId, ServerDownStatusEnum.DisconnectedFromMaster);
        }

    }
}
