using Application.Common.Interfaces;
using Contracts.Events.MatchMakingEvents;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Consumers;

public sealed class MatchStatusUpdateConsumer : IConsumer<MatchStatusUpdateEvent>
{
    private readonly IServerService _serverService;
    public MatchStatusUpdateConsumer(IServerService serverService)
    {
        _serverService = serverService;
    }

    public async Task Consume(ConsumeContext<MatchStatusUpdateEvent> context)
    {
        //await _serverService.SendNewMatchState(context.Message.MatchId, context.Message.NewStatus);
    }
}
