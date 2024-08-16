using Application.Common.Interfaces;
using Contracts.Common.Models;
using Contracts.Common.Models.Enums;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.SignalRHubs.MessageBus;

public class ServerNotifierService : IServerNotifierService
{
    private readonly IHubContext<ServerHub, IServerHubClient> _hubContext;
    private readonly IDatabase _signalrStore;

    private const string KEY = "SignalRConnection:";
    public ServerNotifierService(IHubContext<ServerHub, IServerHubClient> hubContext, IConnectionMultiplexer redis)
    {
        _hubContext = hubContext;
        _signalrStore = redis.GetDatabase();
    }

    //public async Task NotifyServerMatchStatusUpdate(string serverId, MatchStatusEnum newStatus, string? data = null)
    //{
    //    var connectionId = await _signalrStore.StringGetAsync($"{KEY}{serverId}");

    //    if (connectionId == RedisValue.Null)
    //        return;

    //    await _hubContext.Clients.Client(connectionId.ToString()).MatchStatusUpdate(newStatus, data);
    //}

    public async Task NotifyServerMatchStart(string serverId)
    {
        var connectionId = await _signalrStore.StringGetAsync($"{KEY}{serverId}");

            if (connectionId == RedisValue.Null)
                return;

            await _hubContext.Clients.Client(connectionId.ToString()).ServerMatchStart();
    }

    public async Task NotifyServerMatchCancel(string serverId, MatchCancelEnum reason)
    {
        var connectionId = await _signalrStore.StringGetAsync($"{KEY}{serverId}");

        if (connectionId == RedisValue.Null)
            return;

        await _hubContext.Clients.Client(connectionId.ToString()).ServerMatchCancel(reason);
    }

    public async Task NotifyServerMatchEnd(string serverId, List<MatchPlayerResult> matchResults)
    {
        var connectionId = await _signalrStore.StringGetAsync($"{KEY}{serverId}");

        if (connectionId == RedisValue.Null)
            return;

        await _hubContext.Clients.Client(connectionId.ToString()).ServerMatchEnd(matchResults);
    }
}
