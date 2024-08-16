using Contracts.Common.Models;
using Contracts.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IServerService
{
    Task UpEvent(string serverId);
    Task DownEvent(string serverId);
    Task GameEndEvent(string serverId, PlayerTypeEnum wonSide);
    Task PlayerConnectedEvent(string serverId, string userId);
    Task PlayerDisconnectedEvent(string serverId, string userId);
    Task CommandMatchEnd(string matchId, List<MatchPlayerResult> matchResults);
    Task CommandMatchCancel(string matchId, MatchCancelEnum reason);
    Task CommandMatchStart(string matchId);
}
