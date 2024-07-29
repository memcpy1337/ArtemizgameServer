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
    private readonly IDeployService _deployService;
    public MatchNewConsumer(IDeployService serverService)
    {
        _deployService = serverService;
    }

    public async Task Consume(ConsumeContext<MatchNewEvent> context)
    {
        var match = context.Message.Adapt<MatchNewDTO>();

        await _deployService.Deploy(match);
    }
}
