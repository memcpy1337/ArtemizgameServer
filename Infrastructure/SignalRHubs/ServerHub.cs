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
    public ServerHub(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User!.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        var connectionId = Context.ConnectionId;

        await _database.StringSetAsync($"SignalRConnection:{userId}", connectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.User!.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

        await _database.KeyDeleteAsync($"SignalRConnection:{userId}");

        await base.OnDisconnectedAsync(exception);
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