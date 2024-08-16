using Application.Common.Interfaces;
using Contracts.Events.MatchMakingEvents;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Consumers;

public sealed class MatchStartConsumer : IConsumer<MatchStartEvent>
{
    private readonly IServerService _serverService;
    public MatchStartConsumer(IServerService serverService)
    {
        _serverService = serverService;
    }

    public async Task Consume(ConsumeContext<MatchStartEvent> context)
    {
        await _serverService.CommandMatchStart(context.Message.MatchId);
    }
}
