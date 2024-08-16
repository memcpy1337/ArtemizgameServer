using Contracts.Common.Models.Enums;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IServerPublisher
{
    Task ServerUp(Server server);
    Task ServerRequestGameEnd(string matchId, PlayerTypeEnum wonSide);
    Task ServerBadDown(string serverId, string matchId, ServerDownStatusEnum status);
    Task PlayerConnected(string serverId, string userId);
    Task PlayerDisconnected(string serverId, string userId);
}
