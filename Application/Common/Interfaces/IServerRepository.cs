using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IServerRepository
{
    Task Create(Server server);
    Task<Server?> GetByDeployId(string deployId);
    Task SetConnectionData(string requestId, ConnectionData connectionData);
    Task UpdateStatus(string requestId, ServerStatus newStatus);
    Task SetServerReady(string serverId);
    Task<Server> GetByServerId(string serverId);
    Task SetServerInactive(string serverId);
    Task<Server?> GetByMatchId(string matchId);
}
