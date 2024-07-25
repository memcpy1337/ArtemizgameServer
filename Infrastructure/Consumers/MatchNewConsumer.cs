using Application.Common.DTOs;
using Application.Common.Interfaces;
using Contracts.Events.MatchMakingEvents;
using Mapster;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Consumers;

public sealed class MatchNewConsumer : IConsumer<MatchNewEvent>
{
    private readonly IServerService _serverService;
    public MatchNewConsumer(IServerService serverService)
    {
        _serverService = serverService;
    }

    public async Task Consume(ConsumeContext<MatchNewEvent> context)
    {
        var match = context.Message.Adapt<MatchNewDTO>();

        await _serverService.CreateServer(match);
    }
}
