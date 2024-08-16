using Application.Common.Interfaces;
using Contracts.Events.MatchMakingEvents;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Consumers;

public sealed class MatchCancelConsumer : IConsumer<MatchCancelEvent>
{
    private readonly IServerService _serverService;
    public MatchCancelConsumer(IServerService serverService)
    {
        _serverService = serverService;
    }

    public async Task Consume(ConsumeContext<MatchCancelEvent> context)
    {
        await _serverService.CommandMatchCancel(context.Message.MatchId, context.Message.Reason);
    }
}
