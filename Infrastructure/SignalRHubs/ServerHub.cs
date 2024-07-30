using Application.Common.Interfaces;
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

public class ServerHub : Hub
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

        await _serverService.Up(serverId!);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var serverId = Context.User!.Identity!.Name;

        await _database.KeyDeleteAsync($"SignalRConnection:{serverId}");

        await base.OnDisconnectedAsync(exception);

        await _serverService.Down(serverId);
    }

    public async Task PlayerConnected(string userId)
    {
        var serverId = Context.User!.Identity!.Name;

        await _serverService.PlayerConnected(serverId, userId);
    }

    public async Task PlayerDisconnected(string userId)
    {
        var serverId = Context.User!.Identity!.Name;

        await _serverService.PlayerDisconnected(serverId, userId);
    }

    public async Task SendMessageToUser(string userId, string message)
    {
        var connectionId = await _database.StringGetAsync($"SignalRConnection:{userId}");

        if (!string.IsNullOrEmpty(connectionId))
        {
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
        }
    }

}