using Application.Common.Interfaces;
using Contracts.Common.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.SignalRHubs;

public class ServerHub : Hub
{
    private readonly IDatabase _database;
    private readonly IServerService _serverService;

    public ServerHub(IConnectionMultiplexer redis, IServerService serverService)
    {
        _database = redis.GetDatabase();
        _serverService = serverService;
    }

    public override async Task OnConnectedAsync()
    {
        var serverId = Context.User!.FindFirst(JwtRegisteredClaimNames.NameId)?.Value;
        var connectionId = Context.ConnectionId;

        await _database.StringSetAsync($"SignalRConnection:{serverId}", connectionId);

        await base.OnConnectedAsync();

        await _serverService.Up(serverId!);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var serverId = Context.User!.FindFirst(JwtRegisteredClaimNames.NameId)?.Value;

        await _database.KeyDeleteAsync($"SignalRConnection:{serverId}");

        await base.OnDisconnectedAsync(exception);

        await _serverService.Down(serverId);
    }

    public async Task PlayerConnected(string userId)
    {
        var serverId = Context.User!.FindFirst(JwtRegisteredClaimNames.NameId)?.Value;

        await _serverService.PlayerConnected(serverId, userId);
    }

    public async Task PlayerDisconnected(string userId)
    {
        var serverId = Context.User!.FindFirst(JwtRegisteredClaimNames.NameId)?.Value;

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