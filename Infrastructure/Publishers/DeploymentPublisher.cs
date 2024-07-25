using Application.Common.Interfaces;
using Contracts.Events.ServerEvents;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Publishers;

public class DeploymentPublisher : IDeploymentPublisher
{
    private readonly IBus _publishEndpoint;
    public DeploymentPublisher(IBus bus)
    {
        _publishEndpoint = bus;
    }

    public async Task DeploymentFailed(string matchId, string message)
    {
        await _publishEndpoint.Publish<ServerDeployEvent>(new ServerDeployEvent() 
        { 
            MatchId = matchId,
            IsSuccess = false,
            Message = message 
        });
    }

    public async Task DeploymentSuccess(string matchId)
    {
        await _publishEndpoint.Publish<ServerDeployEvent>(new ServerDeployEvent()
        {
            MatchId = matchId,
            IsSuccess = true
        });
    }
}
