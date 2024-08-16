using Application.Common.Interfaces;
using Contracts.Common.Models.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using StackExchange.Redis;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace Infrastructure.SignalRHubs;

public class ServerHub : Hub<IServerHubClient>
{
    private readonly IDatabase _database;
    private readonly IServerService _serverService;
    private readonly ILogger<ServerHub> _logger;

    public ServerHub(IConnectionMultiplexer redis, IServerService serverService, ILogger<ServerHub> logger)
    {
        _database = redis.GetDatabase();
        _serverService = serverService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var serverId = Context.User!.Identity!.Name;

        if (serverId == null) 
        {
            return;
        }

        var connectionId = Context.ConnectionId;

        await _database.StringSetAsync($"SignalRConnection:{serverId}", connectionId);

        await base.OnConnectedAsync();

        _logger.LogInformation($"Server connected. Id: {serverId}. ConnId: {connectionId}");

        await _serverService.UpEvent(serverId!);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var serverId = Context.User!.Identity!.Name;

        if (serverId == null)
            return;

        _logger.LogInformation($"Server {serverId} disconnect from hub");

        await _database.KeyDeleteAsync($"SignalRConnection:{serverId}");

        await base.OnDisconnectedAsync(exception);

        await _serverService.DownEvent(serverId);
    }

    public async Task PlayerConnected(string userId)
    {
        var serverId = Context.User!.Identity!.Name;
        if (serverId == null)
            return;

        await _serverService.PlayerConnectedEvent(serverId, userId);
    }

    public async Task PlayerDisconnected(string userId)
    {
        var serverId = Context.User!.Identity!.Name;
        if (serverId == null)
            return;

        await _serverService.PlayerDisconnectedEvent(serverId, userId);
    }

    public async Task GameEnd(PlayerTypeEnum wonSide)
    {
        var serverId = Context.User!.Identity!.Name;
        if (serverId == null)
            return;

        await _serverService.GameEndEvent(serverId, wonSide);
    }
}