using Application.Common.Interfaces;
using Contracts.Events.MatchMakingEvents;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Consumers;

public sealed class MatchEndConsumer : IConsumer<MatchEndEvent>
{
    private readonly IServerService _serverService;
    public MatchEndConsumer(IServerService serverService)
    {
        _serverService = serverService;
    }

    public async Task Consume(ConsumeContext<MatchEndEvent> context)
    {
        await _serverService.CommandMatchEnd(context.Message.MatchId, context.Message.Results);
    }
}
