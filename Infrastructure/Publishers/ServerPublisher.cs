using Application.Common.Interfaces;
using Contracts.Common.Models.Enums;
using Contracts.Events.ServerEvents;
using Domain.Entities;
using MassTransit;
using System.Threading.Tasks;

namespace Infrastructure.Publishers;

public class ServerPublisher : IServerPublisher
{
    private readonly IBus _publishEndpoint;

    public ServerPublisher(IBus bus)
    {
        _publishEndpoint = bus;
    }

    public async Task ServerUp(Server server)
    {
        await _publishEndpoint.Publish<ServerReadyEvent>(new ServerReadyEvent() { 
            Address = server.ConnectionData.Ip,
            Port = server.ConnectionData.Port,
            MatchId = server.MatchId,
            ServerId = server.ServerId
        });
    }

    public async Task PlayerConnected(string serverId, string userId)
    {
        await _publishEndpoint.Publish<ServerPlayerConnectedEvent>(new ServerPlayerConnectedEvent() { ServerId = serverId, UserId = userId });
    }

    public async Task PlayerDisconnected(string serverId, string userId)
    {
        await _publishEndpoint.Publish<ServerPlayerDisconnectedEvent>(new ServerPlayerDisconnectedEvent() { ServerId = serverId, UserId = userId });
    }

    public async Task ServerBadDown(string serverId, ServerDownStatusEnum status)
    {
        await _publishEndpoint.Publish<ServerBadDownEvent>(new ServerBadDownEvent() { 
            ServerId =  serverId, 
            Status = status
        });
    }
}
