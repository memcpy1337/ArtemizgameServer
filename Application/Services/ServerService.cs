using Application.Common.Interfaces;
using Contracts.Common.Models;
using Contracts.Common.Models.Enums;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services;

public class ServerService : IServerService
{
    private readonly IServerRepository _serverRepository;
    private readonly IServerPublisher _serverPublisher;
    private readonly IServerNotifierService _serverNotifier;
    private readonly IDeployService _deployService;
    private readonly ILogger<ServerService> _logger;

    public ServerService(ILogger<ServerService> logger, IServerRepository serverRepository, IServerPublisher serverPublisher, IServerNotifierService serverNotifier, IDeployService deployService)
    {
        _serverRepository = serverRepository;
        _serverPublisher = serverPublisher;
        _serverNotifier = serverNotifier;
        _deployService = deployService;
        _logger = logger;
    }

    public async Task GameEndEvent(string serverId, PlayerTypeEnum wonSide)
    {
        _logger.LogInformation($"Game end on server {serverId}. Win side: {wonSide.ToString()}");

        var server = await _serverRepository.GetByServerId(serverId);

        await _serverPublisher.ServerRequestGameEnd(server.MatchId, wonSide);
    }

    public async Task UpEvent(string serverId)
    {
        await _serverRepository.SetServerReady(serverId);

        var server = await _serverRepository.GetByServerId(serverId);

        await _serverPublisher.ServerUp(server);
    }

    public async Task PlayerConnectedEvent(string serverId, string userId)
    {
        await _serverPublisher.PlayerConnected(serverId, userId);
    }

    public async Task PlayerDisconnectedEvent(string serverId, string userId)
    {
        await _serverPublisher.PlayerDisconnected(serverId, userId);
    }

    public async Task DownEvent(string serverId)
    {
        var server = await _serverRepository.GetByServerId(serverId);
        //Bad down
        if (server.IsActive)
        {
            _logger.LogInformation($"Server bad down {serverId}");
            await _serverPublisher.ServerBadDown(serverId, server.MatchId, ServerDownStatusEnum.DisconnectedFromMaster);
        }

    }

    public async Task CommandMatchStart(string matchId)
    {
        var server = await _serverRepository.GetByMatchId(matchId);

        await _serverNotifier.NotifyServerMatchStart(server.ServerId);
    }

    public async Task CommandMatchCancel(string matchId, MatchCancelEnum reason)
    {
        var server = await _serverRepository.GetByMatchId(matchId);

        if (server == null || !server.IsActive)
            return;

        await _serverRepository.SetServerInactive(server.ServerId);

        await _serverNotifier.NotifyServerMatchCancel(server.ServerId, reason);

        await _deployService.RemoveDeploy(server.ExternalRequestId);
    }

    public async Task CommandMatchEnd(string matchId, List<MatchPlayerResult> matchResults)
    {
        var server = await _serverRepository.GetByMatchId(matchId);

        if (server == null || !server.IsActive)
            return;

        await _serverRepository.SetServerInactive(server.ServerId);

        await _serverNotifier.NotifyServerMatchEnd(server.ServerId, matchResults);

        await _deployService.RemoveDeploy(server.ExternalRequestId);
    }
}
