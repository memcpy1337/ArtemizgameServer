using Contracts.Common.Models;
using Contracts.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IServerNotifierService
{
    Task NotifyServerMatchStart(string serverId);
    Task NotifyServerMatchCancel(string serverId, MatchCancelEnum reason);
    Task NotifyServerMatchEnd(string serverId, List<MatchPlayerResult> matchResults);
    //Task NotifyServerMatchStatusUpdate(string serverId, MatchStatusEnum newStatus);
}
